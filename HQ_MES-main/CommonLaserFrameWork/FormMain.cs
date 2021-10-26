using CommonLaserFrameWork.SplashScreen;
using CommonLibrarySharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using SLXW;

namespace CommonLaserFrameWork
{
    public partial class FormMain : Form
    {
        // 通用读取配置文件
        private Configure _configure = new Configure();
        // IO
        private static int _startIO = 4;
        private static int _startStation1 = 5;
        private static int _startStation2 = 6;
 

        private static bool _parityOK = false;

        private static Thread _ThreadIO = null;
        // 流程控制
        private static bool _IsMarking = false;
        private static Object _oLock = new Object();
        private static bool _bExit = false;
        private static bool _bManual = false;

        public FormMain()
        {
            SplashScreen.SplashScreen.Show(typeof(FormSplashShow));
            Thread.Sleep(500);
            SplashScreen.SplashScreen.Close();

            InitializeComponent();
            // 绑定日志控件  添加 NLog.dll NLog.config 文件到 exe目录即可
            Log.BindLogControl(richTextBox_Log);
        }

        #region 窗体相关事件
        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadConfig();
            if (WorkProcess.InitForm(this.pictureBox1))
            {
                Log.WriteMessage("启动脚踏检测线程");
                _ThreadIO = new Thread(ThreadIO);
                _ThreadIO.Start();
            }

            Log.WriteMessage(string.Format("开启软件, 工位1:{0},工位2:{1},开始信号:{2}", _startStation1.ToString(), _startStation2.ToString(),_startIO.ToString()));
        }


        private string ToHexString(int nStatus)
        {
            string str = "";
            for(int i=0;i<16;i++)
            {
                if (((nStatus >> i) & 1) >0)
                    str += "1";
                else
                    str += "0";
            }
            return str;
        }
        private void ThreadIO()
        {
            try
            {
              
                bool bStation1Old = false;
                bool bStation1New = false;

                bool bStation1 = false;
                bool bStation2 = false;

                int nStatus = 0;
       
                while (!_bExit)
                {
                    lock (_oLock)
                    {
                        if (_IsMarking)
                        {
                            continue;
                        }
                    }
                    nStatus = WorkProcess.ReadPort();

                    this.Invoke((EventHandler)(delegate
                    {
                        label_io.Text = ToHexString(nStatus);
                    }));

                    bStation1New = ((nStatus >> _startIO) & 1) > 0;
       
                    if (bStation1New && !bStation1Old)
                    {
                        Log.WriteMessage("接收到开始打标信号!");
                        bStation1 = WorkProcess.ReadPort(_startStation1);
                        if (bStation1)
                        {
                            Log.WriteMessage("执行工位1");
                            MarkProcess(1);
                            Thread.Sleep(500);
                        }
                        bStation2 = WorkProcess.ReadPort(_startStation2);
                        if (bStation2)
                        {
                            Log.WriteMessage("执行工位2");
                            MarkProcess(2);
                            Thread.Sleep(500);
                        } 
                        if(!bStation1&&!bStation2)
                        {
                            Log.WriteMessage("收到开始打标信号,但是未收到工位信号!",true);
                        }
                    }
                    bStation1Old = bStation1New;


                    if (_bManual)
                    {
                        Log.WriteMessage("接收到手动信号，使用工位1!");
                        MarkProcess(1);
                        Thread.Sleep(500);
                    }
                    Thread.Sleep(20);
                }
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("ThreadIO 捕获到异常:{0}", ex.Message.ToString()), true);
            }
        }

        private void button_hand_Click(object sender, EventArgs e)
        {
            _bManual = true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (MessageBox.Show("是否退出程序?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }

                _bExit = true;
                _IsMarking = false;
                _bManual = false;
                WriteConfig();
                WorkProcess.CloseForm();
            }
            catch (Exception ex)
            {
                Log.WriteMessage("FormMain_FormClosing 异常:" + ex.Message, true);
                return;
            }
        }

        private void button_load_Click(object sender, EventArgs e)
        {
            FormPWD fp = new FormPWD();
            if (fp.ShowDialog() == DialogResult.OK)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_model.Text = dialog.SelectedPath;
                    _configure.WriteConfig("SET", "EzdModel", textBox_model.Text);
                }
            }

           

          
        }
        private void FormMain_Shown(object sender, EventArgs e)
        {
            textBox_SN.Focus();
        }


        public void SetParityStatus(int nType)
        {

            this.Invoke((EventHandler)(delegate
            {
                if (nType == 1)
                {
                    label_status.Text = "OK";
                    label_status.ForeColor = Color.Lime;
                }
                else if(nType==-1)
                {
                    label_status.Text = "NG";
                    label_status.ForeColor = Color.Red;
                }
                else if (nType == 0)
                {
                    label_status.Text = "";
                }

              
            }));
        }
        private void textBox_SN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                new Thread(() =>
                {
                    if (!WorkProcess.CheckBeforeMark(textBox_SN.Text))
                    {
                        _parityOK = false;
                        SetParityStatus(-1);
                        Log.WriteMessage("校验失败", true);
                         this.Invoke((EventHandler)(delegate
                        {
                            textBox_SN.Text = "";
                        }));
                    }
                    else
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            textBox_SN.Enabled = false;
                        }));
                        _parityOK = true;
                        SetParityStatus(1);
                    }
                
                }).Start();
               
            }
        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)(delegate
            {
                textBox_SN.Enabled = true;
            }));
            WorkProcess.StopMark();
        }
        #endregion

        private void ReadConfig()
        {
            _startIO = _configure.ReadConfig("SET", "StartIO", 3);
            _startStation1 = _configure.ReadConfig("SET", "Station1", 5);
            _startStation2 = _configure.ReadConfig("SET", "Station2", 6);
        


            textBox_model.Text = _configure.ReadConfig("SET", "EzdModel", "");
            textBox_model2.Text = _configure.ReadConfig("SET", "EzdModel2", "");

        }

        private void WriteConfig()
        {
            _configure.WriteConfig("SET", "EzdModel", textBox_model.Text);
        }

        // 获取主窗体控件内容
        public Dictionary<string, string> GetControlValue(int nstation)
        {
            Dictionary<string, string> dicControlValue = new Dictionary<string, string>();

            try
            {
                this.Invoke((EventHandler)(delegate
                {
                    if (nstation==1)
                        dicControlValue.Add("dir", textBox_model.Text);
                    else
                        dicControlValue.Add("dir", textBox_model2.Text);

                    dicControlValue.Add("SN", textBox_SN.Text);
                }));
            }
            catch (Exception ex)
            {
                Log.WriteMessage("GetControlValue 异常:" + ex.Message, true);
            }

            return dicControlValue;
        }

        public bool MarkProcess(int nstation)
        {
            if (!_parityOK)
            {
                Log.WriteMessage("扫码校验失败，请先校验DSN后执行标刻!");
                return false;
            }
            _IsMarking = true;
            Log.WriteMessage("开始执行打标流程");

            try
            {
                Dictionary<string, string> dicControlValue = GetControlValue(nstation);

                WorkProcess.MarkProcessImpl(dicControlValue);
                Log.WriteMessage("++++++++++结束本次打标流程++++++++++");
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("MarkProcess 捕获到异常:{0}", ex.Message.ToString()), true);
            }
            finally
            {
                _bManual = false;
                _IsMarking = false;
                _parityOK = false;
                SetParityStatus(0);
                this.Invoke((EventHandler)(delegate
                {
                    textBox_SN.Enabled = true;
                    textBox_SN.Text = "";
                    textBox_SN.Focus();
                }));
            }

            return false;
        }

        private void button_load2_Click(object sender, EventArgs e)
        {
            FormPWD fp = new FormPWD();
            if (fp.ShowDialog() == DialogResult.OK)
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBox_model2.Text = dialog.SelectedPath;
                    _configure.WriteConfig("SET", "EzdModel2", textBox_model2.Text);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button_set_Click(object sender, EventArgs e)
        {
            FormPWD fp = new FormPWD();
            if(fp.ShowDialog()==DialogResult.OK)
            {
                FormSetting fs = new FormSetting();
                fs.ShowDialog();
                textBox_SN.Focus();
            }
  
        }
    }
}
