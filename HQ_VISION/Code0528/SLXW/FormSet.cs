using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using Laser_JCZ;
using System.IO.Ports;
using ConfigurationTool;

namespace WindowsFormsApplication_media
{
    public partial class FormSet : Form
    {
        public FormSet()
        {
            InitializeComponent();
        }

        private void FormSet_Load(object sender, EventArgs e)
        {
           textBox_bar.Text = Configure.ReadConfig("SET", "BARLEN", "11,15");
           numericUpDown_timeout.Value = Convert.ToDecimal(Configure.ReadConfig("SET", "TIMEOUT", 500));

           comboBox_com.SelectedIndex= Configure.ReadConfig("SET", "COM", 0);
           comboBox_baund.SelectedIndex = Configure.ReadConfig("SET", "BAUND", 0);
           comboBox_data.SelectedIndex = Configure.ReadConfig("SET", "DATA", 0);
           comboBox_parity.SelectedIndex = Configure.ReadConfig("SET", "PARITY", 0);
           comboBox_stop.SelectedIndex = Configure.ReadConfig("SET", "STOP", 0);
           comboBox_mark2.SelectedIndex = Configure.ReadConfig("SET", "START2", 5);
           comboBox_mark.SelectedIndex = Configure.ReadConfig("SET", "START", 5);
       
           comboBox_finish.SelectedIndex = Configure.ReadConfig("SET", "FINISH", 5);
           textBox_modelDir.Text = Configure.ReadConfig("SET", "DIR", "");

           textBox_tool.Text = Configure.ReadConfig("SET", "TOOL","");
           textBox_action.Text = Configure.ReadConfig("SET", "ACTION", "");

           comboBox_model.SelectedIndex = Configure.ReadConfig("SET", "WORKMODEL", 0);
           textBox_manualModel.Text = Configure.ReadConfig("SET", "MANUALMODEL1", "");
           textBox_manualMode2.Text = Configure.ReadConfig("SET", "MANUALMODEL2", "");

           textBox_IMEI1.Text = Configure.ReadConfig("SET", "IMEI1", "");
           textBox_IMEI2.Text = Configure.ReadConfig("SET", "IMEI2", "");
           textBox_char.Text = Configure.ReadConfig("SET", "CHAR", "6@/;9@/;16@/;");



           numericUpDown_delay.Value = Convert.ToDecimal(Configure.ReadConfig("SET", "DELAY", 300));
           comboBox_startLaser.SelectedIndex = Configure.ReadConfig("SET", "STARTLASER", 5);
           comboBox_LaserOut.SelectedIndex = Configure.ReadConfig("SET", "LASEROUT", 5);
           comboBox_edge.SelectedIndex = Configure.ReadConfig("SET", "EDGE", 0);


        }

        private void FormSet_FormClosing(object sender, FormClosingEventArgs e)
        {
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configure.WriteConfig("SET", "COM", comboBox_com.SelectedIndex);
            Configure.WriteConfig("SET", "BAUND", comboBox_baund.SelectedIndex);
            Configure.WriteConfig("SET", "DATA", comboBox_data.SelectedIndex);
            Configure.WriteConfig("SET", "PARITY", comboBox_parity.SelectedIndex);
            Configure.WriteConfig("SET", "STOP", comboBox_stop.SelectedIndex);
            Configure.WriteConfig("SET", "TIMEOUT", numericUpDown_timeout.Value.ToString());

            
            Configure.WriteConfig("SET", "START", comboBox_mark.SelectedIndex);
            Configure.WriteConfig("SET", "START2", comboBox_mark2.SelectedIndex);

            Configure.WriteConfig("SET", "FINISH", comboBox_finish.SelectedIndex);
  
            Configure.WriteConfig("SET", "TOOL", textBox_tool.Text);
            Configure.WriteConfig("SET", "ACTION", textBox_action.Text);

            Configure.WriteConfig("SET", "DIR", textBox_modelDir.Text);

            Configure.WriteConfig("SET", "WORKMODEL", comboBox_model.SelectedIndex);

            Configure.WriteConfig("SET", "MANUALMODEL1", textBox_manualModel.Text);
            Configure.WriteConfig("SET", "MANUALMODEL2", textBox_manualMode2.Text);

            Configure.WriteConfig("SET", "IMEI1", textBox_IMEI1.Text);
            Configure.WriteConfig("SET", "IMEI2", textBox_IMEI2.Text);
            Configure.WriteConfig("SET", "CHAR", textBox_char.Text);

            Configure.WriteConfig("SET", "DELAY", numericUpDown_delay.Value.ToString());
            Configure.WriteConfig("SET", "STARTLASER", comboBox_startLaser.SelectedIndex);
            Configure.WriteConfig("SET", "LASEROUT", comboBox_LaserOut.SelectedIndex);
            Configure.WriteConfig("SET", "EDGE", comboBox_edge.SelectedIndex);

            Configure.WriteConfig("SET", "BARLEN", textBox_bar.Text);

      

            MessageBox.Show("参数保存成功");
            this.DialogResult = DialogResult.OK;
            this.Close();
           
        }

  

        private void button_Dir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox_modelDir.Text = dialog.SelectedPath;
            }
        }

        private void checkBox_pass_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = null;
            openFileDialog.Filter = "ezd文件|*.ezd|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_manualModel.Text = openFileDialog.FileName;
               
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = null;
            openFileDialog.Filter = "ezd文件|*.ezd|所有文件|*.*";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox_manualMode2.Text = openFileDialog.FileName;

            }
        }
    }
}
