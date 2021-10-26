using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
namespace Laser_JCZ
{
    #region ErrorCode
    public enum LmcErrCode
    {
        LMC1_ERR_SUCCESS = 0,
        LMC1_ERR_EZCADRUN = 1,
        LMC1_ERR_NOFINDCFGFILE = 2,
        LMC1_ERR_FAILEDOPEN = 3,
        LMC1_ERR_NODEVICE = 4,
        LMC1_ERR_HARDVER = 5,
        LMC1_ERR_DEVCFG = 6,
        LMC1_ERR_STOPSIGNAL = 7,
        LMC1_ERR_USERSTOP = 8,
        LMC1_ERR_UNKNOW = 9,
        LMC1_ERR_OUTTIME = 10,
        LMC1_ERR_NOINITIAL = 11,
        LMC1_ERR_READFILE = 12,
        LMC1_ERR_OWENWNDNULL = 13,
        LMC1_ERR_NOFINDFONT = 14,
        LMC1_ERR_PENNO = 15,
        LMC1_ERR_NOTTEXT = 16,
        LMC1_ERR_SAVEFILE = 17,
        LMC1_ERR_NOFINDENT = 18,
        LMC1_ERR_STATUE = 19,
        LMC1_ERR_PARAM = 20,
        LMC1_ERR_BRAND=21,
        LMC1_ERROR_NOEZDFILE=22,
        LMC1_ERROR_NOINIT=23,
        LMC1_ERROR_OUTOFPORTRANGE=24,
        LMC1_ERROR_NOFINDMARKEZD=25
    }
    #endregion

    public class LoadDll
    {
        /*******************************************初始化******************************************/

        [DllImport("MarkEzd.dll", CharSet=CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Initial")]
        protected static extern LmcErrCode LMC1_INITIAL(string strEzCadPath, int bTestMode,IntPtr hOwenWnd);

        /****************************************停止关闭*******************************************/

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Close")]
        protected static extern LmcErrCode LMC1_CLOSE();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_StopMark")]
        protected static extern LmcErrCode LMC1_STOPMARK();

        /****************************************文件保存,添加及加载*******************************************/

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_LoadEzdFile")]
        protected static extern LmcErrCode LMC1_LOADEZDFILE(string strFileName);
        

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AddFileToLib")]
        protected static extern LmcErrCode LMC1_ADDFILETOLIB(string strFilePath, string strEntName, double dLeftDownXPos, double dLeftDowmYPos, double dZpos, int nAlign, double dScaleRation, int nPenNo, bool bHachFile);


        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SaveEntLibToFile")]
        protected static extern LmcErrCode LMC1_SAVEENTLIBTOFILE(string strFileName);


        /***********************添加曲线和填充************************************/

        /// 向数据库添加一条曲线对象
        /// 注意PtBuf必须为2维数组,且第一维为2,如 double[5,2],double[n,2],
        /// ptNum为PtBuf数组的第2维,如PtBuf为double[5,2]数组,则ptNum=5
        /// strEntName 添加曲线对象名称
        /// bHatch 如果是闭合曲线是否填充 1填充 0不填充
        /// </summary> 
        [DllImport("MarkEzd", EntryPoint = "lmc1_AddCurveToLib", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_ADDCURVETOLIB([MarshalAs(UnmanagedType.LPArray)] double[,] PtBuf, int ptNum, string strEntName, int nPenNo, int bHatch);

        [DllImport("MarkEzd", EntryPoint = "lmc1_SetHatchParam", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_SETHATCHPARAM(bool bEnableContour,
                                          int bEnableHacth1,
                                          int nPenNo1,
                                          int nHatchAttribute1,
                                          double dHatchEdge1,
                                          double dHatchDist1,
                                          double dHatchStartOffset1,
                                          double dHatchEndOffset1,
                                          double dHatchAngle1,
                                          int bEnableHacth2,
                                          int nPenNo2,
                                          int nHatchAttribute2,
                                          double dHatchEdge2,
                                          double dHatchDist2,
                                          double dHatchStartOffset2,
                                          double dHatchEndOffset2,
                                          double dHatchAngle2);

        //填充
        [DllImport("MarkEzd.dll", EntryPoint = "lmc1_SetHatchParam2", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_SETHATCHPARAM2(bool bEnableContour,//使能轮廓本身
                                                          int nParamIndex,//填充参数序号值为1,2,3
                                                          int bEnableHatch,//使能填充
                                                          int nPenNo,//填充参数笔号
                                                          int nHatchType,//填充类型 0单向 1双向 2回形 3弓形 4弓形不反向
                                                          bool bHatchAllCalc,//是否全部对象作为整体一起计算
                                                          bool bHatchEdge,//绕边一次
                                                          bool bHatchAverageLine,//自动平均分布线
                                                          double dHatchAngle,//填充线角度 
                                                          double dHatchLineDist,//填充线间距
                                                          double dHatchEdgeDist,//填充线边距    
                                                          double dHatchStartOffset,//填充线起始偏移距离
                                                          double dHatchEndOffset,//填充线结束偏移距离
                                                          double dHatchLineReduction,//直线缩进
                                                          double dHatchLoopDist,//环间距
                                                          int nEdgeLoop,//环数
                                                          bool nHatchLoopRev,//环形反转
                                                          bool bHatchAutoRotate,//是否自动旋转角度
                                                          double dHatchRotateAngle);//自动旋转角度   
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_HatchEnt")]
        protected static extern LmcErrCode LMC1_HATCHENT(string pEntName,string  strNewName);

        /******************Entity对象操作(移动,旋转,群组,缩放,替换,预览,删除)**************************************/
        
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_GetTextByName")]
        protected static extern LmcErrCode LMC1_GETTEXTBYNAME(string pEntName, char[] strText);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_MoveEnt")]
        protected static extern LmcErrCode LMC1_MOVEENT(string pEntName, double dMovex, double dMovey);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_RotateEnt")]
        protected static extern LmcErrCode LMC1_ROTATEENT(string strEntName, double dCenx, double dCeny, double dAngle);

        //两两群组 
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GroupEnt")]
        protected static extern LmcErrCode LMC1_GROUPENT(string strEntName1, string strEntName2, string strNewGroupName, int nPenNo);
        //解散群组 
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_UnGroupEnt")]
        protected static extern LmcErrCode LMC1_UNGROUPENT(string strGroupEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ChangeTextByName")]
        protected static extern LmcErrCode LMC1_CHANGETEXTBYNAME(string strEntName, string strText);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPrevBitmap2")]
        protected static extern IntPtr LMC1_GETPREVBITMAP2(int nBMPWIDTH, int nBMPHEIGHT);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_DeleteEnt")]
        protected static extern LmcErrCode LMC1_DELETEENT(string strEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ScaleEnt")]
        protected static extern LmcErrCode LMC1_SCALEENT(string strEntName, double dCenterX, double dCenterY, double dScaleX, double dScaleY);

        /*************************************获取设置Entity对象属性()**************************************/

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntCount")]
        protected static extern int LMC1_GETENTCOUNT();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntSize")]
        protected static extern LmcErrCode LMC1_GETENTSIZE(string strEntName, ref double dMinx, ref double dMiny, ref double dMaxx, ref double dMaxy, ref double dZ);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntityCount")]
        protected static extern int LMC1_GETENTTITYCOUNT();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetEntityName")]
        protected static extern LmcErrCode LMC1_GETENTITYNAME(int nEntityIndex, char[] strEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetEntityName")]
        protected static extern LmcErrCode LMC1_SETENTITYNAME(int nEntityIndex, string strEntName);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetBitmapEntParam3")]
        protected static extern LmcErrCode LMC1_GETBITMAPENTPARAM3(string strEntName, ref double dDpiX, ref double dDpiY, Byte[] bGrayScaleBuff);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetBitmapEntParam3")]
        protected static extern LmcErrCode LMC1_SETBITMAPENTPARAM3(string strEntName, double dDpiX, double dDpiY, Byte[] bGrayScaleBuff);

        [DllImport("MarkEzd", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_SetBitmapEntParam2")]
        public static extern LmcErrCode LMC1_SETBITMAPENTPARAM2(string strEntName,
                                                          string strImageFileName,
                                                          int nBmpAttrib,
                                                          int nScanAttrib,
                                                          double dBrightness,
                                                          double dContrast,
                                                          double dPointTime,
                                                          int nImportDpi,
                                                          bool bDisableMarkLowGrayPt,
                                                          int nMinLowGrayPt
                                                           );
        [DllImport("MarkEzd", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_GetBitmapEntParam2")]
        public static extern LmcErrCode LMC1_GETBITMAPENTPARAM2(string strEntName,
                                                         StringBuilder strImageFileName,
                                                         ref int nBmpAttrib,
                                                         ref int nScanAttrib,
                                                         ref double dBrightness,
                                                         ref double dContrast,
                                                         ref double dPointTime,
                                                         ref int nImportDpi,
                                                         ref int bDisableMarkLowGrayPt,
                                                         ref int nMinLowGrayPt
                                                         );


        //根据对象名字获取笔号
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenNumberFromEnt")]
        protected static extern int LMC1_GETPENNUMBERFROMENT(string strEntName);

        //根据对象名字获取笔号
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenNumberFromName")]
        protected static extern int LMC1_GETPENNUMBERFROMNAME(string strEntName);


        /***************************************出激光,红光***********************************************/

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Mark")]
        protected static extern LmcErrCode LMC1_MARK(int nFly);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_RedLightMark")]
        protected static extern LmcErrCode LMC1_REDLIGHTMARK();

        /*******************************************IO操作***********************************************/

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_WritePort")]
        protected static extern LmcErrCode LMC1_WRITEPORT(int nData);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_ReadPort")]
        protected static extern LmcErrCode LMC1_READPORT(ref int nData);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetOutPort")]
        protected static extern LmcErrCode LMC1_GETOUTPORT(ref int nData);

        /**************************************参数设置***************************************************/

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_SetDevCfg")]
        protected static extern LmcErrCode LMC1_SETDEVCFG();

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetPenParam")]
        protected static extern LmcErrCode LMC1_GETPENPARAM(int nPenNo,//要设置的笔号(0-255)					 
                    ref int nMarkLoop,//加工次数
                        ref double dMarkSpeed,//标刻次数mm/s
                        ref double dPowerRatio,//功率百分比(0-100%)	
                        ref double dCurrent,//电流A
                        ref int nFreq,//频率HZ
                    ref double dQPulseWidth,//Q脉冲宽度us	
                        ref int nStartTC,//开始延时us
                        ref int nLaserOffTC,//激光关闭延时us 
                        ref int nEndTC,//结束延时us
                        ref int nPolyTC,//拐角延时us   //	
                        ref double dJumpSpeed, //跳转速度mm/s
                        ref int nJumpPosTC, //跳转位置延时us
                    ref int nJumpDistTC,//跳转距离延时us	
                        ref double dEndComp,//末点补偿mm
                        ref double dAccDist,//加速距离mm	
                        ref double dPointTime,//打点延时 ms						 
                    ref bool bPulsePointMode,//脉冲点模式 
                    ref int nPulseNum,//脉冲点数目
                        ref double dFlySpeed);//流水线速度

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, EntryPoint = "lmc1_SetPenParam")]
        protected static extern LmcErrCode LMC1_SETPENPARAM(int nPenNo,//要设置的笔号(0-255)					 
                        int nMarkLoop,//加工次数
                        double dMarkSpeed,//标刻次数mm/s
                        double dPowerRatio,//功率百分比(0-100%)	
                        double dCurrent,//电流A
                            int nFreq,//频率HZ
                        double dQPulseWidth,//Q脉冲宽度us	
                        int nStartTC,//开始延时us
                        int nLaserOffTC,//激光关闭延时us 
                            int nEndTC,//结束延时us
                        int nPolyTC,//拐角延时us   //	
                        double dJumpSpeed, //跳转速度mm/s
                        int nJumpPosTC, //跳转位置延时us
                        int nJumpDistTC,//跳转距离延时us	
                        double dEndComp,//末点补偿mm
                        double dAccDist,//加速距离mm	
                        double dPointTime,//打点延时 ms						 
                        bool bPulsePointMode,//脉冲点模式 
                        int nPulseNum,//脉冲点数目
                        double dFlySpeed);//流水线速度


        /******************************************JCZ扩展轴*******************************/

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AxisMoveTo")]
        public static extern LmcErrCode LMC1_AXISMOVETO(int axis, double GoalPos);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AxisMoveToPulse")]
        public static extern LmcErrCode LMC1_AXISMOVETOPULSE(int axis, int nPluse);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_AxisCorrectOrigin")]
        public static extern LmcErrCode LMC1_AXISCORRECTORIGIN(int axis);

        //需先调用Reset之后才可以调用AxisCorrectOrigin回零
        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_Reset")]
        public static extern LmcErrCode LMC1_RESET(bool bAxis1, bool bAxis2);

        [DllImport("MarkEzd.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "lmc1_GetAxisCoorPulse")]
        public static extern int LMC1_GETAXISCOORPULSE(int axis);
        

        /*****************************************飞动部分******************************/

        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferClear", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERCLEAR();
   
        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferFlyGetParam", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYGETPARAM(ref int nTotalMarkCount, ref int nBufferLeftCount);

        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferPartFinish", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERPARTFINISH();

        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferFlyAdd", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYADD(int nNum, string Text1, string Text2, string Text3, string Text4, string Text5, string Text6);

        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferSetAddMode", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERSETADDMODE(bool FullMode);

        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferSetTextName", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERSETTEXTNAME(string Name1, string Name2, string Name3, string Name4, string Name5, string Name6);

        [DllImport("MarkEzd", EntryPoint = "lmc1_ContinueBufferFlyStart", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYSTART(int nCount);

        /****************************其他*****************************************/

        [DllImport("gdi32.dll")]
        protected static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll",EntryPoint="PostMessage",CharSet = CharSet.Auto,CallingConvention=CallingConvention.Winapi)]
        protected static extern bool POSTMESSAGE(IntPtr hwnd, int msg, uint wParam, uint lParam); 
    }
}
