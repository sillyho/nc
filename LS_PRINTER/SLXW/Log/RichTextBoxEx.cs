using System;
using System.Collections.Generic;

using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Studio_Log
{
    public class Log_RichTextBoxEx : Log
    {
        private static RichTextBox richtextBox = null;

        public static void BindLogControl(RichTextBox rtBox)
        {
            richtextBox = rtBox;
        }

        public static void WriteMessage(string text,bool Error = false)
        {

            text = DateTime.Now.ToString("【HH:mm:ss】 ") + text;
            if (!richtextBox.IsHandleCreated)
            {
                return;
            }
            if (richtextBox.InvokeRequired)
            {
                richtextBox.Invoke((EventHandler)(delegate
                {
                    if (richtextBox.Lines.Length > 100)
                    {
                        richtextBox.Clear();
                    }
                    text += Environment.NewLine;
                    if (Error)
                    {
                        richtextBox.SelectionColor = Color.Red;
                    }
                    richtextBox.SelectionStart = richtextBox.TextLength;
                    richtextBox.SelectionLength = 0;
                    richtextBox.AppendText(text);
                    richtextBox.SelectionColor = richtextBox.ForeColor;
                    richtextBox.ScrollToCaret();
                }));
            }
            else
            {
                if (richtextBox.Lines.Length > 100)
                {
                    richtextBox.Clear();
                }
                text += Environment.NewLine;
                if (Error)
                {
                    richtextBox.SelectionColor = Color.Red;
                }
                richtextBox.SelectionStart = richtextBox.TextLength;
                richtextBox.SelectionLength = 0;
                richtextBox.AppendText(text);
                richtextBox.SelectionColor = richtextBox.ForeColor;
                richtextBox.ScrollToCaret();
            }
           
         }
    }
}
