using CommonLibrarySharp;
using System;
using System.Threading;
using System.Windows.Forms;

namespace CommonLaserFrameWork
{
    static class Program
    {
        static Mutex m_mutex = new Mutex(true, "7D0EB345-A465-4756-A8E9-9E92A8C4402E");

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (m_mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.ThreadException += Application_ThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                // 自行根据项目添加加密狗
                Application.Run(new FormMain());
            }
            else
            {
                MessageBox.Show("已经有软件实例在运行");
            }  
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.Exception;
                string strMsg = string.Format(" Application_ThreadException 捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}", ex.GetType(), ex.Message, ex.StackTrace);
                Log.WriteMessage(strMsg);
            }
            catch (Exception ex)
            {
                Log.WriteMessage("Application_ThreadException: " + ex.Message);
            }
        }

        /// <summary>
        /// 处理应用程序域内的未处理异常（非UI线程异常）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                string strMsg = string.Format(" CurrentDomain_UnhandledException 捕获到未处理异常：{0}\r\n异常信息：{1}\r\n异常堆栈：{2}", ex.GetType(), ex.Message, ex.StackTrace);
                Log.WriteMessage(strMsg);
            }
            catch (Exception ex)
            {
                Log.WriteMessage("CurrentDomain_UnhandledException: " + ex.Message);
            }
        }
    }
}
