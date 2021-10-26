using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

using System.Threading;
using ConfigurationTool;

using Studio_Log;

using PrintReport;
using SYS_DB;
using System.IO;

namespace SLXW
{
    public partial class FormMain : Form
    {

        private ManualResetEvent m_MarkEvent = new ManualResetEvent(false);

        private Thread m_MarkThread = null;
        private bool m_bExit = false;

        private Print m_Print = new Print();
        public FormMain()
        {
            InitializeComponent();
            Log_RichTextBoxEx.BindLogControl(richTextBox1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            numericUpDown_count.Value = Convert.ToDecimal(Configure.ReadConfig("SET", "COUNT", 1));

            textBox_Name.Text = Configure.ReadConfig("SET", "NAME", "SN");
            textBox_Total.Text = Configure.ReadConfig("SET", "TOTAL", "0");
            textBox_model_grf.Text = Configure.ReadConfig("SET", "MODEL_GRF", "");
            comboBox_communicate.SelectedIndex = Configure.ReadConfig("SET", "COMMUNICATE", 0);
            textBox_ip.Text= Configure.ReadConfig("SET", "IP", "127.0.0.1");
            textBox_port.Text= Configure.ReadConfig("SET", "PORT", "20000");
            textBox_end.Text = Configure.ReadConfig("SET", "END", "09");

            if (!AccessHelper.CompactAccessDB())
            {
                MessageBox.Show("连接数据库失败，请检查是否有Data.mdb文件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            AccessHelper.ConnectToDatabase();
           
        }
      
        private void PrintData(string strData)
        {
            string strDir="";
            int nLoops = 1;
            int nCurCount = 0;
            string strName = "SN";
            this.Invoke((EventHandler)(delegate
            {
                strDir = textBox_model_grf.Text + "\\";
                nLoops = Convert.ToInt32(numericUpDown_count.Value);
                nCurCount= Convert.ToInt32(textBox_Total.Text);
                strName = textBox_Name.Text;
            }));
            string strPrintName = strDir + strData.Length.ToString() + ".grf";

            if(!File.Exists(strPrintName))
            {
                Log_RichTextBoxEx.WriteMessage("打印文件不存在:" + strPrintName);
                return;
            }
           
            int nResult = IsExist(strData);
            if (nResult == -1)
                return;
            else if (nResult == 1)
            {
                if (MessageBox.Show("重复数据:" + strData + ",是否继续打印?", "检测到重码", MessageBoxButtons.OKCancel, MessageBoxIcon.Error)!=DialogResult.OK)
                {
                    return;
                }
            }
            Log_RichTextBoxEx.WriteMessage("条码内容：" + strData);
            //界面显示
            this.Invoke((EventHandler)(delegate
            {
                label_TXT.Text = strData;
            }));
            try
            {
                if (!m_Print.ChangeTextByName(strName, strData))
                {
                    Log_RichTextBoxEx.WriteMessage("打印模板替换内容失败",true);return;
                }
                for (int i=0;i< nLoops; i++)
                {
                    m_Print.PrintDoc(false);
                }
            }
            catch (Exception ex)
            {
                Log_RichTextBoxEx.WriteMessage("打印异常:" + ex.Message,true);
               return;
            }
            this.Invoke((EventHandler)(delegate
            {
                textBox_Total.Text = (nCurCount + 1).ToString();
                Configure.WriteConfig("SET", "TOTAL", textBox_Total.Text);
            }));
            if (!SaveData(strData))
            {
                Log_RichTextBoxEx.WriteMessage("保存数据失败:" + strData);
            }
        }
        private bool SaveData(string strBarcode)
        {
            return AccessHelper.InsertData(String.Format("insert into records values('{0}')", strBarcode));
        }
        private int IsExist(string strBarcode)
        {
            string strSql = String.Format("select * from records where code ='{0}'", strBarcode);
            bool bExist=false;
            if (!AccessHelper.IsExist(strSql, ref bExist))
            {
                Log_RichTextBoxEx.WriteMessage("查询失败:" + strSql);
                return -1;
            }
            return bExist ? 1 : 0;
        }

       
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Configure.WriteConfig("SET", "COUNT", numericUpDown_count.Value.ToString());
            Configure.WriteConfig("SET", "NAME", textBox_Name.Text);
            Configure.WriteConfig("SET", "TOTAL", textBox_Total.Text);
            Configure.WriteConfig("SET", "MODEL_GRF", textBox_model_grf.Text);

            Configure.WriteConfig("SET", "COMMUNICATE", comboBox_communicate.SelectedIndex);
            Configure.WriteConfig("SET", "IP", textBox_ip.Text);
            Configure.WriteConfig("SET", "PORT", textBox_port.Text);
            Configure.WriteConfig("SET", "END", textBox_end.Text);

        }
      
      
       
        private void FormMain_Shown(object sender, EventArgs e)
        {
            
        }
        private void button_grf_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
           
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox_model_grf.Text = dialog.SelectedPath;
            }
        }

      

        private void button_remark_Click(object sender, EventArgs e)
        {
           
        }

        private void comboBox_communicate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
