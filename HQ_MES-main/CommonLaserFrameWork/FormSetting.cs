using CommonLibrarySharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonLaserFrameWork
{
    public partial class FormSetting : Form
    {
        private  Configure _configure = new Configure();
        public FormSetting()
        {
            InitializeComponent();
        }

        private void FormSetting_Load(object sender, EventArgs e)
        {
            textBox_url.Text = _configure.ReadConfig("MES", "url", "");
            textBox_station.Text = _configure.ReadConfig("MES", "station", "");
            textBox_uid.Text = _configure.ReadConfig("MES", "uid", "");
            textBox_pwd.Text = _configure.ReadConfig("MES", "pwd", "");
            textBox_nopwd.Text= _configure.ReadConfig("MES", "nopwd", "");
            textBox_toolname.Text= _configure.ReadConfig("MES", "toolname", "");
            textBox_requesttype.Text= _configure.ReadConfig("MES", "requesttype", "GET");

            comboBox_begin.SelectedIndex = _configure.ReadConfig("SET", "StartIO", 4);
            comboBox_finish.SelectedIndex = _configure.ReadConfig("SET", "EndIO", 5);
            comboBox_station1.SelectedIndex = _configure.ReadConfig("SET", "Station1", 6);
            comboBox_station2.SelectedIndex = _configure.ReadConfig("SET", "Station2", 7);
            textBox_code.Text = _configure.ReadConfig("BR", "contextformat", @"https://ring.com/s?m={MAC}&d=j1&n={DSN}&v=2");

            checkBox_update.Checked = Convert.ToBoolean(_configure.ReadConfig("SET", "update", true));
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            _configure.WriteConfig("MES", "url", textBox_url.Text);
            _configure.WriteConfig("MES", "station", textBox_station.Text );
            _configure.WriteConfig("MES", "uid", textBox_uid.Text );
            _configure.WriteConfig("MES", "pwd", textBox_pwd.Text );
            _configure.WriteConfig("MES", "nopwd", textBox_nopwd.Text );
            _configure.WriteConfig("MES", "toolname", textBox_toolname.Text );
            _configure.WriteConfig("MES", "requesttype", textBox_requesttype.Text );

            _configure.WriteConfig("SET", "StartIO", comboBox_begin.SelectedIndex );
            _configure.WriteConfig("SET", "EndIO", comboBox_finish.SelectedIndex );
           _configure.WriteConfig("SET", "Station1", comboBox_station1.SelectedIndex);
            _configure.WriteConfig("SET", "Station2", comboBox_station2.SelectedIndex);
            _configure.WriteConfig("BR", "contextformat", textBox_code.Text);
            _configure.WriteConfig("SET", "update", checkBox_update.Checked.ToString());
            WorkProcess._updateData = checkBox_update.Checked;
            MessageBox.Show("保存成功，将在下一次重启时生效！");
        }
    }
}
