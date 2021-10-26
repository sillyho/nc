using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ConfigurationTool;
using Laser_JCZ;
using Studio_Log;
using WindowsFormsApplication_media;
using System.IO.Ports;
using System.Net;
using System.IO;
namespace SLXW
{
    public partial class FormMain : Form
    {
 

        private Thread m_MarkCheck = null;


        private bool m_bIsWorking = false;

        private int m_StartIO = 0;
        private int m_LaserOut = 0;
        private int m_ArrivedDelay = 0;
        private bool m_bedge = false;

        private int m_MarkIO1 = 2;
        private int m_MarkIO2 = 3;
        private int m_CurWorkModel = 0;

        private string m_manualModel1;
        private string m_manualModel2;

        private string m_manualIMEI1;
        private string m_manualIMEI2;
        private string m_changeChar;

        private int m_FinishIO = 5;
        private int m_FinishIODelay = 300;

        private int m_CurWorkStation;
        //MES
        private string m_MesAction = "";
        private string m_MesDir = "";
        private string m_MesTool = "";


        private int m_CheckTimeOut = 5000;
        private string m_LocalIP = "";

        private SerialPort m_Port = new SerialPort();
        public FormMain()
        {
            InitializeComponent();
            Log_RichTextBoxEx.BindLogControl(richTextBox1);
   
        }
        public string GetLocalIp()
        {
            ///获取本地的IP地址
            string AddressIP = "";
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    AddressIP = _IPAddress.ToString();
                }
            }
            return AddressIP;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
 
      
            bool bInit = MarkJcz.InitLaser(this.Handle);
            if (!bInit)
            {
                MessageBox.Show("初始化激光器失败", "请检查Markezd.dll文件", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log_RichTextBoxEx.WriteMessage("初始化激光器失败", true);
            }
     
            if (!MesHelper.InitMes())
            {

            }
            ReadPrm();

            MarkJcz.WritePort(m_LaserOut, !m_bedge);
        }
        private void WaitForMark()
        {
            MarkJcz.WritePort(m_LaserOut, m_bedge);
            Thread.Sleep(m_ArrivedDelay);
            this.Invoke((EventHandler)(delegate
            {
                label_result.Text = "";
                textBox_code.Enabled = false;
            }));
            bool bMarkResult = LaserMark();
            if (bMarkResult)
            {
                this.Invoke((EventHandler)(delegate
                {
                    label_result.ForeColor = Color.Green;
                    label_result.Text = "PASS";
                    textBox_code.Enabled = true;
                }));
            }
            else
            {
                this.Invoke((EventHandler)(delegate
                {
                    label_result.ForeColor = Color.Red;
                    label_result.Text = "FAIL";
                    textBox_code.Enabled = true;
                }));
            }
            if(bMarkResult)
            {
                Vision();
            }
            MarkJcz.WritePort(m_LaserOut, !m_bedge);
            this.Invoke((EventHandler)(delegate
            {
                textBox_code.Text = "";
                textBox_code.Focus();
            }));
            Thread.Sleep(100);
            MarkJcz.WritePort(m_FinishIO, true);
            Thread.Sleep(m_FinishIODelay);
            MarkJcz.WritePort(m_FinishIO, false);

        }
        private void Vision()
        {
            Log_RichTextBoxEx.WriteMessage("发送视觉打标完成数据:A");
            if (!WriteCom("A"))
            {
                return ;
            }
            string strResult;
            if(!ReadCom(m_CheckTimeOut,out strResult))
            {
                return;
            }
            if(strResult.Contains("OK"))
            {
                Log_RichTextBoxEx.WriteMessage("视觉检测成功:" + strResult);
            }
            else
            {
                Log_RichTextBoxEx.WriteMessage("视觉检测失败:" + strResult);
            }
        }
        private bool CheckIP()
        {
            m_LocalIP = GetLocalIp();
            if (m_LocalIP == "")
            {
                Log_RichTextBoxEx.WriteMessage("获取本机IP地址失败，请检查网络连接");
                MessageBox.Show("获取本机IP地址失败，请检查网络连接", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; 
            }
            string strIP = Configure.ReadConfig("SET", "LOCALIP", "");
            if (strIP == "")
            {
                Configure.WriteConfig("SET", "LOCALIP", m_LocalIP);
            }
            else
            {
                if (strIP != m_LocalIP)
                {
                    Log_RichTextBoxEx.WriteMessage("发现IP地址有变化,，请更新手动IP地址");
                    MessageBox.Show("发现IP地址有变化,，请更新手动IP地址", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }
            }
            return true;
        }
      

        private bool Laser_onLine()
        {
            //获取流水码
            if (textBox_code.Text == "")
            {
                Log_RichTextBoxEx.WriteMessage("请先手动扫码");
                MessageBox.Show("请先扫码，再执行转盘启动工作!", "操作错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            string strCodeData = textBox_code.Text;
            Log_RichTextBoxEx.WriteMessage("条码内容:" + textBox_code.Text);
            //获取信息
            string strOutInformation = "";
            Log_RichTextBoxEx.WriteMessage("开始获取MES数据...");
            if (!MesHelper.GetMesInformation(strCodeData, m_MesAction, m_MesTool, ref strOutInformation))
            {
                return false;
            }
            string strConvertErrorResult = "";

            MesDataRecv mr = MesDataRecv.JsonToClass(strOutInformation, ref strConvertErrorResult);
            if (mr == null)
            {
                Log_RichTextBoxEx.WriteMessage("转换异常:" + strConvertErrorResult);
                return false;
            }
            this.Invoke((EventHandler)(delegate
            {
                //显示数据
                textBox_IMEI1.Text = mr.DATA.IMEI1 == null ? "" : mr.DATA.IMEI1;
                if (mr.DATA.IMEI2 != null)
                    textBox_IMEI2.Text = mr.DATA.IMEI2 == null ? "" : mr.DATA.IMEI2;
                else
                    textBox_IMEI2.Text = "";
                textBox_BatteryPN.Text = mr.DATA.Battery_CoverPN;
                textBox_CSN.Text = mr.DATA.CSN;
                textBox_BTMAC.Text = mr.DATA.BTMAC;
                textBox_MODEL1.Text = mr.DATA.LaserTemplate;
                textBox_MODEL2.Text = mr.DATA.LaserTemplate2;
            }));
            string strEzdFile = "";
            string strLocalFile = "";
            if (m_CurWorkModel==0)
            {
                if (m_CurWorkStation == 1)
                    strEzdFile = mr.DATA.LaserTemplate_Path + "\\" + mr.DATA.LaserTemplate;
                else
                    strEzdFile = mr.DATA.LaserTemplate_Path + "\\" + mr.DATA.LaserTemplate2;
                string strCurTmpFile = Application.StartupPath + "\\tmp.ezd";
                Log_RichTextBoxEx.WriteMessage("服务器文件路径:" + strEzdFile);
                try
                {
                    File.Copy(strEzdFile, strCurTmpFile, true);
                }
                catch (Exception ex)
                {
                    Log_RichTextBoxEx.WriteMessage("下载文件失败:" + ex.Message);
                    MessageBox.Show("下载文件失败:" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                strLocalFile = strCurTmpFile;
                Log_RichTextBoxEx.WriteMessage("下载文件到本地成功:" + strCurTmpFile);
            }
            else if (m_CurWorkModel == 1)
            {
                if (m_CurWorkStation == 1)
                    strEzdFile = m_MesDir + "\\" + mr.DATA.LaserTemplate;
                else
                    strEzdFile = m_MesDir + "\\" + mr.DATA.LaserTemplate2;
                strLocalFile = strEzdFile;
            }
            else
            {
                Log_RichTextBoxEx.WriteMessage("程序调用错误");
                return false;
            }

            if (!File.Exists(strLocalFile))
            {
                Log_RichTextBoxEx.WriteMessage("文件不存在:" + strLocalFile);
                MessageBox.Show("文件不存在:" + strLocalFile + ",请检查文件路径是否有拼写错误等", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
            if (!MarkJcz.LoadEzdFile(strLocalFile))
            {
                Log_RichTextBoxEx.WriteMessage("加载模板文件失败:" + strLocalFile);
                return false;
            }
            Log_RichTextBoxEx.WriteMessage("MODEL_SN:" +(mr.DATA.MODEL_SN == null ? "" : mr.DATA.MODEL_SN));

            if (mr.DATA.MODEL_SN != null )
            {
                if (mr.DATA.MODEL_SN.Length>=10)
                {
                    MarkJcz.ChangeTextByName("MODEL_SN", mr.DATA.MODEL_SN.Substring(3, 7));
                }
            }
            MarkJcz.ChangeTextByName("BTMAC", mr.DATA.BTMAC);
            Log_RichTextBoxEx.WriteMessage("BTMAC:" + mr.DATA.BTMAC);

            MarkJcz.ChangeTextByName("Battery_CoverPN", mr.DATA.Battery_CoverPN);
            Log_RichTextBoxEx.WriteMessage("Battery_CoverPN:" + mr.DATA.Battery_CoverPN);

            
            Log_RichTextBoxEx.WriteMessage("CSN:" + mr.DATA.CSN);
            MarkJcz.ChangeTextByName("CSN", mr.DATA.CSN);


            MarkJcz.ChangeTextByName("CUSTMODEL", mr.DATA.CUSTMODEL);
            Log_RichTextBoxEx.WriteMessage("CUSTMODEL:" + mr.DATA.CUSTMODEL);

            MarkJcz.ChangeTextByName("G_CHECKFLOWID", mr.DATA.G_CHECKFLOWID);
            Log_RichTextBoxEx.WriteMessage("G_CHECKFLOWID:" + mr.DATA.G_CHECKFLOWID);

            if (mr.DATA.IMEI1_SPLIT != null && mr.DATA.IMEI1_SPLIT != "")
            {
                MarkJcz.ChangeTextByName("IMEI1_SPLIT", mr.DATA.IMEI1_SPLIT);
                Log_RichTextBoxEx.WriteMessage("IMEI1_SPLIT:" + mr.DATA.IMEI1_SPLIT);
            }
            if (mr.DATA.IMEI1 != null && mr.DATA.IMEI1 != "")
            {
                MarkJcz.ChangeTextByName("IMEI1", mr.DATA.IMEI1);
                Log_RichTextBoxEx.WriteMessage("IMEI1:" + mr.DATA.IMEI1);
            }

            if (mr.DATA.IMEI2_SPLIT != null && mr.DATA.IMEI2_SPLIT != "")
            {
                MarkJcz.ChangeTextByName("IMEI2_SPLIT", mr.DATA.IMEI2_SPLIT);
                Log_RichTextBoxEx.WriteMessage("IMEI2_SPLIT:" + mr.DATA.IMEI2_SPLIT);
            }
            if (mr.DATA.IMEI2 != null && mr.DATA.IMEI2 != "")
            {
                MarkJcz.ChangeTextByName("IMEI2", mr.DATA.IMEI2);
                Log_RichTextBoxEx.WriteMessage("IMEI2:" + mr.DATA.IMEI2);
            }
            MarkJcz.ChangeTextByName("WIFIMAC", mr.DATA.WIFIMAC);
            Log_RichTextBoxEx.WriteMessage("WIFIMAC:" + mr.DATA.WIFIMAC);

            MarkJcz.ShowPreviewBmp(pictureBox1);
            Log_RichTextBoxEx.WriteMessage("开始标刻");

            if (!MarkJcz.Mark())
            {
                Log_RichTextBoxEx.WriteMessage("激光器出光失败");
                return false;
            }
            Log_RichTextBoxEx.WriteMessage("标刻完成");

            return true;
        }

        private bool Laser_Offline()
        {
            Log_RichTextBoxEx.WriteMessage("手动输入模式");
            Log_RichTextBoxEx.WriteMessage("IMEI1:"+m_manualIMEI1);
            Log_RichTextBoxEx.WriteMessage("IMEI2:" + m_manualIMEI2);
            if (m_manualIMEI1=="")
            {
                Log_RichTextBoxEx.WriteMessage("请输入IMEI1的值");
                return false;
            }
            if (m_CurWorkStation==1)
            {
                Log_RichTextBoxEx.WriteMessage("模板1:" + m_manualModel1);
                if (!MarkJcz.LoadEzdFile(m_manualModel1))
                {
                    Log_RichTextBoxEx.WriteMessage("加载模板失败"); 
                    return false;
                }
            }
            else
            {
                Log_RichTextBoxEx.WriteMessage("模板2:" + m_manualModel2);
                if (!MarkJcz.LoadEzdFile(m_manualModel2))
                {
                    Log_RichTextBoxEx.WriteMessage("加载模板失败");
                    return false;
                }
            }

            MarkJcz.ChangeTextByName("IMEI1_SPLIT", m_manualIMEI1);
            Log_RichTextBoxEx.WriteMessage("IMEI1_SPLIT:" + m_manualIMEI1);
            if (m_manualIMEI2!="")
            {
                MarkJcz.ChangeTextByName("IMEI2_SPLIT", m_manualIMEI2);
                Log_RichTextBoxEx.WriteMessage("IMEI2_SPLIT:" + m_manualIMEI2);
            }
            this.Invoke((EventHandler)(delegate
            {
                //显示数据
                textBox_IMEI1.Text = m_manualIMEI1;
     
                textBox_IMEI2.Text = m_manualIMEI2;
                textBox_BatteryPN.Text = "";
                textBox_CSN.Text = "";
                textBox_BTMAC.Text ="";
                textBox_MODEL1.Text = "";
                textBox_MODEL2.Text = "";
            }));
            MarkJcz.ShowPreviewBmp(pictureBox1);
            if (!MarkJcz.Mark())
            {
                Log_RichTextBoxEx.WriteMessage("激光器出光失败");
                return false;
            }
            Log_RichTextBoxEx.WriteMessage("手动镭雕完成");
            return true;

        }
        private bool LaserMark()
        {
            if (m_CurWorkModel==0||m_CurWorkModel==1)
            {
                if (!Laser_onLine())
                    return false;

                Log_RichTextBoxEx.WriteMessage("开始上传保存MES数据...");
                if (!MesHelper.UpLoadInformation(textBox_code.Text, m_MesAction, m_MesTool, "0", ""))
                    return false;

                return true;
            }
            else if (m_CurWorkModel==2)//手动输入信息
            {
                return Laser_Offline();
            }
            else if (m_CurWorkModel==3)//仅过站
            {
                //获取流水码
                if (textBox_code.Text == "")
                {
                    Log_RichTextBoxEx.WriteMessage("请先手动扫码");
                    MessageBox.Show("请先扫码，再执行转盘启动工作!", "操作错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                this.Invoke((EventHandler)(delegate
                {
                    //显示数据
                    textBox_IMEI1.Text = "";
                    textBox_IMEI2.Text = "";
                    textBox_BatteryPN.Text = "";
                    textBox_CSN.Text = "";
                    textBox_BTMAC.Text = "";
                    textBox_MODEL1.Text = "";
                    textBox_MODEL2.Text = "";
                }));
                Log_RichTextBoxEx.WriteMessage("仅过站模式,开始上传保存MES数据...");
                if (!MesHelper.UpLoadInformation(textBox_code.Text, m_MesAction, m_MesTool, "0", ""))
                    return false;
                return true;
            }
            return false;
        }
      
        private string AddBias(string strImei1)
        {
            string[] strArry = m_changeChar.Split(new string[] { ";" },StringSplitOptions.None);
            try
            {
                for (int i = 0; i < strArry.Length-1;i++ )
                {
                    string[] dataarry = strArry[i].Split(new string[] { "@" }, StringSplitOptions.None);
                    strImei1 = strImei1.Insert(Convert.ToInt16(dataarry[0]), dataarry[1]);
                }
            }
            catch (Exception ex)
            {
                Log_RichTextBoxEx.WriteMessage("手动设置IMEI规则解析异常:" + ex.Message);
                return "";

            }
            return strImei1;
        }

        private void ThreadCheckIO()
        {
            int nBegin = 0;
            int nEnd = 0;
            int nFlagState = 0;
            while (m_bIsWorking)
            {
                if (MarkJcz.ReadPort(m_StartIO))
                    nBegin = 1;
                else
                    nBegin = 0;

                if (nEnd == 0 && nBegin == 1)
                {
                    Log_RichTextBoxEx.WriteMessage("收到开始打标信号");

                    if (MarkJcz.ReadPort(m_MarkIO1))
                    {
                        m_CurWorkStation = 1;
                        Log_RichTextBoxEx.WriteMessage("转盘到位1信号");
                        if (nFlagState!=1)
                        {
                            nFlagState = 1;
                            WaitForMark();
                        }
                        else
                        {
                            Log_RichTextBoxEx.WriteMessage("上一次触发为工位1，请停止后重试!");

                        }
                        
                    }
                    else if (MarkJcz.ReadPort(m_MarkIO2))
                    {
                        m_CurWorkStation = 2;
                        Log_RichTextBoxEx.WriteMessage("转盘到位2信号");
                        if ( nFlagState !=2)
                        {
                            nFlagState = 2;
                            WaitForMark();
                        }
                        else
                        {
                            Log_RichTextBoxEx.WriteMessage("上一次触发为工位2，请停止后重试!");
                        }
                    }
                    else
                    {
                        Log_RichTextBoxEx.WriteMessage("收到开始打标信号,但未检测到工位信号");
                    }
                }
                nEnd = nBegin;

                Thread.Sleep(50);
               
            }

        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_bIsWorking = false;

            MesHelper.CloseMes();
        }

      

        private void button_manual_Click(object sender, EventArgs e)
        {
            textBox_code.Focus();
        }
    

      

        private void FormMain_Shown(object sender, EventArgs e)
        {
            textBox_code.Focus();
        }

        private void button_set_Click(object sender, EventArgs e)
        {
            FormPWD fp = new FormPWD();
            if (fp.ShowDialog()==DialogResult.OK)
            {
                FormSet fs = new FormSet();
                fs.ShowDialog();
                ReadPrm();

            }
      
        }

        private List<string> ListBarLen = new List<string>();
        public bool ReadPrm()
        {
        
            m_MarkIO1 = Configure.ReadConfig("SET", "START", 4);
            m_MarkIO2 = Configure.ReadConfig("SET", "START2", 4);

            m_FinishIO = Configure.ReadConfig("SET", "FINISH", 5);
            m_FinishIODelay = Configure.ReadConfig("SET", "DELAY", 200);

            m_MesAction =Configure.ReadConfig("SET", "ACTION", "");
            m_MesTool = Configure.ReadConfig("SET", "TOOL", "");
          
            m_MesDir = Configure.ReadConfig("SET", "DIR", "");

            m_CurWorkModel = Configure.ReadConfig("SET", "WORKMODEL", 0);
            m_manualModel1 = Configure.ReadConfig("SET", "MANUALMODEL1", "");
            m_manualModel2 = Configure.ReadConfig("SET", "MANUALMODEL2", "");

            m_manualIMEI1 = Configure.ReadConfig("SET", "IMEI1",  "");
            m_manualIMEI2 = Configure.ReadConfig("SET", "IMEI2",  "");
            m_changeChar = Configure.ReadConfig("SET", "CHAR", "6@/;9@/;16@/;");

 
            m_ArrivedDelay = Configure.ReadConfig("SET", "DELAY", 300);
            m_StartIO = Configure.ReadConfig("SET", "STARTLASER", 5);
            m_LaserOut = Configure.ReadConfig("SET", "LASEROUT", 5);
            m_bedge = Configure.ReadConfig("SET", "EDGE", 0)==0?false:true;

            m_CheckTimeOut = Configure.ReadConfig("SET", "TIMEOUT", 5000);


            string str = Configure.ReadConfig("SET", "BARLEN", "");

            ListBarLen = new List<string>(str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
           

            return false;


        }

        private bool OpenCom()
        {
            m_CheckTimeOut=Configure.ReadConfig("SET", "TIMEOUT", 500);
            int nCom = Configure.ReadConfig("SET", "COM", 0)+1;
            int nBaund = Configure.ReadConfig("SET", "BAUND", 9600);
            if (nBaund == 0)
                nBaund = 9600;
            else if (nBaund == 1)
                nBaund = 19200;
            else if (nBaund == 2)
                nBaund = 115200;
            else
                nBaund = 9600;
            int nData = Configure.ReadConfig("SET", "DATA", 0)+7;
            int nparity = Configure.ReadConfig("SET", "PARITY",0);
            Parity parity = Parity.None;
            if (nparity == 0)
                parity = Parity.None;
            else if (nparity == 1)
                parity = Parity.Odd;
            else if (nparity == 2)
                parity = Parity.Even;
            int nstop = Configure.ReadConfig("SET", "STOP", 0);
            StopBits sb = StopBits.None;
            if (nstop == 0)
                sb = StopBits.None;
            else if (nstop == 1)
                sb = StopBits.One;
            else if (nstop == 2)
                sb = StopBits.Two;
            else if (nstop == 3)
                sb = StopBits.OnePointFive;
            try
            {
                if (m_Port.IsOpen)
                    m_Port.Close();
                m_Port.PortName = "COM" + nCom.ToString();
                m_Port.BaudRate = nBaund;
                m_Port.DataBits = nData;
                m_Port.Parity = parity;
                m_Port.StopBits = sb;
                m_Port.Open();
                return true;
            }
            catch(Exception ex)
            {
                Log_RichTextBoxEx.WriteMessage("打开串口异常:" + ex.Message);
                return false;
            }
        }
        private bool WriteCom(string str)
        {
            byte[] bData = System.Text.Encoding.Default.GetBytes(str);
            if(!m_Port.IsOpen)
            {
                Log_RichTextBoxEx.WriteMessage("串口未打开!");
                return false;
            }
            try
            {
                m_Port.Write(bData, 0, bData.Length);
                return true;
            }
            catch(Exception ex)
            {
                Log_RichTextBoxEx.WriteMessage("发送数据异常:"+ex.Message);
                return false;
            }
        }
        private bool ReadCom(int nTimeout,out string str)
        {
            str = "NG";
            int nTimeOutIndex = 0;
            int nTimeOutMax = nTimeout / 100;
            try
            {
                while (true)
                {
                    if (nTimeOutMax < nTimeOutIndex)
                    {
                        Log_RichTextBoxEx.WriteMessage("等待视觉检测超时!");
                        return false;
                    }
                    if (m_Port.BytesToRead <= 0)
                    {
                        Thread.Sleep(100); nTimeOutIndex++; continue;
                    }
                    Thread.Sleep(100);
                    byte[] breadString = new byte[m_Port.BytesToRead];
                    m_Port.Read(breadString, 0, m_Port.BytesToRead);
                    str = System.Text.Encoding.Default.GetString(breadString);
                    break;
                }
                return true;
            }
            catch (Exception ex)
            {
                Log_RichTextBoxEx.WriteMessage("串口读取异常:" + ex.Message);
                return false;
            }

        }
        private void button_start_Click(object sender, EventArgs e)
        {
            ReadPrm();
            //打开串口
            if(!OpenCom())
            {
                return;
            }

            if (m_MarkCheck != null)
            {
                if (m_MarkCheck.IsAlive)
                {
                    m_MarkCheck.Abort();
                    Thread.Sleep(100);
                }
            }
            m_bIsWorking = true;
            m_MarkCheck = new Thread(ThreadCheckIO);
            m_MarkCheck.Start();


    
            button_set.Enabled = false;
            button_start.Enabled = false;
            textBox_code.Text = "";
            textBox_code.Focus();
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            m_bIsWorking = false;

     
            button_set.Enabled = true;
            button_start.Enabled = true;

        }

        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void textBox_code_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==13)
            {
                if (ListBarLen.Count > 0 && !ListBarLen.Contains(textBox_code.Text.Length.ToString()))
                {
                    Log_RichTextBoxEx.WriteMessage("条码长度错误!",true);

                    textBox_code.Focus();
                    textBox_code.SelectAll();
                    return;
                } 
             
                new Thread(() =>
                {
                    string strOutInformation = "";
                    Log_RichTextBoxEx.WriteMessage("开始验证条码数据");
                    if (!MesHelper.CheckMesInformation(textBox_code.Text, m_MesAction, m_MesTool, ref strOutInformation))
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            textBox_code.Text = "";
                            textBox_code.Focus();
                        }));
                        return;
                    }
                    Log_RichTextBoxEx.WriteMessage("校验数据完成");
         
                }).Start();
           
            }
        }
    }
}
