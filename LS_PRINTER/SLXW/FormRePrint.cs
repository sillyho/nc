using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConfigurationTool;

namespace SLXW
{
    public partial class FormRePrint : Form
    {
        public FormRePrint()
        {
            InitializeComponent();
        }

        private void textBox_SN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar==13)
            {
                Configure.WriteConfig("SET", "REMARK", textBox_SN.Text);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            Configure.WriteConfig("SET", "REMARK", textBox_SN.Text);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
