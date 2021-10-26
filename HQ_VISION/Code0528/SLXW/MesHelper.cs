using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Studio_Log;
using System.Windows.Forms;

namespace SLXW
{
    public class MesHelper
    {

        public static int _mesHandler = 0;
        private static MESBackFunc tempFunc;// 必须要加一个变量，这样不会被回收
#region 接口导出
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MESBackFunc(StringBuilder data);

        private const string DLLPATH = "HQMES.dll";
        [DllImport(DLLPATH)]
        private static extern int MesInit(MESBackFunc func, ref int hMes, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        private static extern int MesStart(int hMes, string SN, string ActionName, string Tools, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        private static extern int MesStart2(int hMes, string SN, string SNType, string ActionName, string Tools, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        private static extern int MesEnd(int hMes, string SN, string ActionName, string Tools, string ErrorCode, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        private static extern int MesEnd2(int hMes, string SN, string SNType, string ActionName, string Tools, string ErrorCode, string AllData, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        private static extern int MesSaveAndGetExtraInfo(int hMes, string G_TYPE, string G_POSITION, string G_KEY, string G_VALUE, string G_EXTINFO, StringBuilder sInfo, ref int InfoLen);
        [DllImport(DLLPATH)]
        private static extern int MesUnInit(int hMes);
#endregion
       
        ///日志回调函数
        public static int WriteLogFile(StringBuilder data)
        {
            return 0;
        }

        public static bool InitMes()
        {
            tempFunc = WriteLogFile;
            int len = 102400;
            StringBuilder strdata = new StringBuilder(len);
            if (_mesHandler == 0)
            {
                if (0 == MesInit(tempFunc, ref _mesHandler, strdata, ref len))
                {
                    Studio_Log.Log_RichTextBoxEx.WriteMessage("初始化MES成功:" + strdata.ToString());
                    return true;
                }
                else
                {
                    _mesHandler = 0;
                    Studio_Log.Log_RichTextBoxEx.WriteMessage("初始化MES失败 :" + strdata.ToString());
                    return false;
                }
            }
            else
            {
                return true;
            }

        }
        public static bool CheckMesInformation(string strInputCode, string strActionName, string strToolName, ref string strOutInformation)
        {
            if (_mesHandler == 0)
            {
                Log_RichTextBoxEx.WriteMessage("MES未初始化");
                return false;
            }
            int len = 102400;
            StringBuilder strdata = new StringBuilder(len);
            if (0 == MesStart(_mesHandler, strInputCode, strActionName, strToolName, strdata, ref len))
            {
                strOutInformation = strdata.ToString();
                return true;
            }
            else
            {
                strOutInformation = strdata.ToString();
                Log_RichTextBoxEx.WriteMessage("验证MES信息失败:" + strdata.ToString());
                MessageBox.Show("验证条码失败:" + strdata.ToString());
                return false;
            }
        }


        public static bool GetMesInformation(string strInputCode, string strActionName, string strToolName, ref string strOutInformation)
        {
            if (_mesHandler==0)
            {
                Log_RichTextBoxEx.WriteMessage("MES未初始化");
                return false;
            }
            int len = 102400;
            StringBuilder strdata = new StringBuilder(len);
            if (0 == MesStart(_mesHandler, strInputCode, strActionName, strToolName, strdata, ref len))
            {
                strOutInformation = strdata.ToString();
                Log_RichTextBoxEx.WriteMessage("获取MES信息成功:"+strdata.ToString());
                return true;
            }
            else
            {
                strOutInformation = strdata.ToString();
                Log_RichTextBoxEx.WriteMessage("获取MES信息失败:" + strdata.ToString());
                return false;
            }
        }

        public static bool UpLoadInformation(string strInputCode, string strActionName, string strToolName, string strErrorCode, string strUploadInfo)
        {
            if (_mesHandler == 0)
            {
                Log_RichTextBoxEx.WriteMessage("MES未初始化");
                return false;
            }
            int len = 102400;
            StringBuilder strdata = new StringBuilder(len);
            strdata.Append(strUploadInfo);

            if (0 == MesEnd(_mesHandler, strInputCode, strActionName, strToolName, strErrorCode, strdata, ref len))
            {
                Log_RichTextBoxEx.WriteMessage("上传MES信息成功:" + strdata.ToString());
                return true;
       
            }
            else
            {
                Log_RichTextBoxEx.WriteMessage("上传MES信息失败:" + strdata.ToString());
                return true;
            }

        }


        public static bool CloseMes()
        {
            if (_mesHandler!=0)
            {
                return MesUnInit(_mesHandler) == 0;
            }
            return false;
            
        }
    }
}
