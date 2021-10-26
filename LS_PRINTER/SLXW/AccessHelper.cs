using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace SYS_DB
{
    class AccessHelper
    {
        public static bool m_bConnectSuccess = false;
        public static OleDbConnection odcConnection = null;

        public static string m_strDBpath = System.Windows.Forms.Application.StartupPath +"\\data.mdb";
        /// <summary>
        /// 连接Access数据库
        /// </summary>
        /// <param name="mdbPath">数据库文件路径</param>
        /// <returns></returns>
        public static bool ConnectToDatabase()
        {
            try
            {
                // 判断数据库文件是否存在
                if (!File.Exists(m_strDBpath))
                {
                    m_bConnectSuccess = false;
                    return false;
                }
                // 建立连接  
                string strConn = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + m_strDBpath;
                odcConnection = new OleDbConnection(strConn);
                if (odcConnection.State == ConnectionState.Open)
                {
                    m_bConnectSuccess = true;
                    return false;
                }
                // 打开连接 
                odcConnection.Open();

                m_bConnectSuccess = true;


                return true;
            }
            catch (System.Exception )
            {
                m_bConnectSuccess = false;
                return false;
            }
        }
        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="strSql">sql查询语句</param>
        /// <param name="dt">查询到的数据表</param>
        /// <returns></returns>
        public static bool ReadData(string strSql, ref DataTable dt)
        {
            if (!m_bConnectSuccess)
                return false;
            if (strSql == null)
                return false;

            try
            {
                // 建立SQL查询   
                OleDbCommand odCommand = odcConnection.CreateCommand();

                OleDbDataAdapter sda = new OleDbDataAdapter(strSql, odcConnection);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                dt = ds.Tables[0];
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsExist(string strSql, ref bool bExist)
        {
            if (!m_bConnectSuccess)
                return false;
            if (strSql == null)
                return false;

            try
            {
                // 建立SQL查询   
                OleDbCommand odCommand = odcConnection.CreateCommand();

                OleDbDataAdapter sda = new OleDbDataAdapter(strSql, odcConnection);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                if (ds.Tables[0].Rows.Count>0)
                {
                    bExist = true;
                }
                else
                {
                    bExist = false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool UpdateData(string strUpdate)
        {
            if (!m_bConnectSuccess)
                return false;

            try
            {

                string sql = strUpdate;
                OleDbCommand cmd = new OleDbCommand(strUpdate, odcConnection);
                cmd.ExecuteNonQuery();

                return true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
        }
        public static bool InsertData(string strInsert)
        {

            if (!m_bConnectSuccess)
                return false;
            try
            {

                string sql = strInsert;
                OleDbCommand cmd = new OleDbCommand(strInsert, odcConnection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
        }
        public static bool DeleteData(string strDelete)
        {

            if (!m_bConnectSuccess)
                return false;
            try
            {

                string sql = strDelete;
                OleDbCommand cmd = new OleDbCommand(strDelete, odcConnection);
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <returns></returns>
        public static bool CloseDatabase()
        {
            try
            {
                if (odcConnection != null )
                {
                    odcConnection.Close();
                }
                
                return true;
            }
            catch (System.Exception )
            {
                return false;
            }
        }

        //将记录集显示
        public static void ShowTableToListView(System.Windows.Forms.ListView listview, DataTable dt)
        {
            try
            {
                listview.Invoke((EventHandler)(delegate
                {
                    //清空列名
                    listview.Clear();
                    listview.BeginUpdate();
                    //添加列名
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        System.Windows.Forms.ColumnHeader header = new System.Windows.Forms.ColumnHeader();
                        header.Text = dt.Columns[i].Caption;
                        header.Width = listview.Width / dt.Columns.Count;
                        listview.Columns.Add(header);
                    }
                    System.Windows.Forms.ListViewItem[] item = new System.Windows.Forms.ListViewItem[dt.Rows.Count];

                    string[] strColumnName = new string[dt.Columns.Count];

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        for (int n = 0; n < dt.Columns.Count; n++)
                        {
                            strColumnName[n] = dt.Rows[j].ItemArray[n].ToString();
                        }

                        item[j] = new System.Windows.Forms.ListViewItem(strColumnName);
                        listview.Items.Add(item[j]);
                    }
                    listview.EndUpdate();
                }));
            }
            catch (System.Exception)
            {
                return;
            }
        }
        public static void ShowTableDataGridView(System.Windows.Forms.DataGridView DataGridView, DataTable dt)
        {
            try
            {
                DataGridView.Invoke((EventHandler)(delegate
                {
                    DataGridView.DataSource = dt;
                }));
            }
            catch(Exception )
            {

            }
        }

        //获取文件所有表
        public static List<string> GetAllTableName()
        {
            List<string> templist = new List<string>();
            DataTable dt = odcConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            for (int i = 0; i < dt.Rows.Count;i++ )
            {
                string str=dt.Rows[i]["TABLE_NAME"].ToString();
               templist.Add(String.Format("{0}\n", str));
            }
            return templist;
        }

	//压缩数据库
        public static bool CompactAccessDB()
        {
            try
            {
                CloseDatabase();
                string TempMdbName = System.Windows.Forms.Application.StartupPath + @"\Temp.mdb";

                //创建 Jet 引擎对象
                object objJetEngine = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));

                //设置参数数组
                //根据你所使用的Access版本修改 "Jet OLEDB:Engine Type=5" 中的数字.
                //5 对应 JET4X 格式 (access 2000,2002)

                object[] objParams = new object[] {
                String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0}",m_strDBpath), //输入连接字符串
                String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Jet OLEDB:Engine Type=5",TempMdbName) //输出连接字符串
                };
                //通过反射调用 CompactDatabase 方法
                objJetEngine.GetType().InvokeMember("CompactDatabase",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    objJetEngine,
                    objParams);

                //删除原数据库文件
                System.IO.File.Delete(m_strDBpath);
                //重命名压缩后的数据库文件
                System.IO.File.Move(TempMdbName, m_strDBpath);
                //释放Com组件
                System.Runtime.InteropServices.Marshal.ReleaseComObject(objJetEngine);
                objJetEngine = null;
                return ConnectToDatabase();
            }
            catch (Exception)
            {
                return false;
            }
           
        }
        //批量增加
        public static bool BachInsert(List<string> ListInsertSql)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = odcConnection;
            cmd.Transaction = odcConnection.BeginTransaction();
            try
            {
                foreach (string strSql in ListInsertSql)
                {
                    cmd.CommandText = strSql;
                    cmd.ExecuteNonQuery();
                }
                cmd.Transaction.Commit();  //提交事务
                return true;
            }
            catch (Exception)
            {
                cmd.Transaction.Rollback();
                return false;
            }
        }
    }
}
