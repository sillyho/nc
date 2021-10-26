using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace Studio_Log
{
    public class Log
    {
        private static string mPathName = Application.StartupPath + "\\" + "Records\\";


        public static void WriteLog(string strData)
        {
            StringBuilder strFile = new StringBuilder();
            strFile.AppendFormat("{0}", mPathName);
            if (!Directory.Exists(strFile.ToString()))
            {
                Directory.CreateDirectory(strFile.ToString());
            }
            strFile.Append(DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
            using (StreamWriter swAppend = File.AppendText(strFile.ToString()))
            {
                StringBuilder str = new StringBuilder();
                str.AppendFormat("{0}",strData);
                swAppend.WriteLine(str.ToString());
            }
        }

    }
}
