﻿using CommonLibrarySharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;


namespace SLXW
{
    public partial class FormPWD : Form
    {
        private Configure _configure = new Configure();
        public FormPWD()
        {
            InitializeComponent();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            string strpwd= _configure.ReadConfig("SET","PWD","123");
            if(strpwd=="")
            {
                _configure.WriteConfig("SET", "PWD", "123");
                strpwd = "123";
            }
            if (strpwd==textBox_pwd.Text)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("密码输入错误!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormPWD_Shown(object sender, EventArgs e)
        {
            textBox_pwd.Focus();
        }

        private void textBox_pwd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==13)
            {
                string strpwd = _configure.ReadConfig("SET", "PWD", "123");
                if (strpwd == textBox_pwd.Text)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("密码输入错误!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
