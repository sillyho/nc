using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Studio_Log
{
    public class Log
    {
        private static Log instance = null;

        private static Mutex mutexFile = new Mutex();

        private static object lockObject = new object();

        private static string dirLog = System.Windows.Forms.Application.StartupPath + @"\log";


        static Log()
        {
            lock (lockObject)
            {
                if (instance != null)
                {
                    lock (lockObject)
                    {
                        instance = new Log();
                        //文件夹创建
                        if (!Directory.Exists(dirLog))
                        {
                            Directory.CreateDirectory(dirLog);
                        }
                    }
                }
            }

        }
        /// <summary>
        /// 设置日志存放路径
        /// </summary>
        public static string PathName
        {
            set
            {
                dirLog = Application.StartupPath + "\\" + value;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="strData"></param>
        public static void WriteLog(string strData, bool Error = false)
        {
            using (Mutex mut = new Mutex(false, "lockfilemutex"))
            {
                try
                {
                    //上锁，其他线程需等待释放锁之后才能执行处理；若其他线程已经上锁或优先上锁，则先等待其他线程执行完毕
                    mut.WaitOne();
                    StringBuilder strFile = new StringBuilder();
                    strFile.AppendFormat("{0}\\{1}-{2}", dirLog, DateTime.Now.Year, DateTime.Now.Month.ToString("D2"));
                    if (!Directory.Exists(strFile.ToString()))
                    {
                        Directory.CreateDirectory(strFile.ToString());
                    }
                    StringBuilder strFile2 = new StringBuilder();

                    strFile2.AppendFormat("{0}\\{1}-{2}\\{3}", dirLog, DateTime.Now.Year, DateTime.Now.Month.ToString("D2"), DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
                    using (StreamWriter swAppend = File.AppendText(strFile2.ToString()))
                    {
                        StringBuilder str = new StringBuilder();
                        str.AppendFormat("[{0}][{1}][{2}][ThreadID:{3}]", DateTime.Now, DateTime.Now.Millisecond.ToString("d4"), strData, Thread.CurrentThread.ManagedThreadId);
                        swAppend.WriteLine(str.ToString());
                    }
                }
                catch (AbandonedMutexException)
                {

                }
                finally
                {
                    //释放锁，让其他进程(或线程)得以继续执行
                    mut.ReleaseMutex();
                }
            }
        }

    }
}
