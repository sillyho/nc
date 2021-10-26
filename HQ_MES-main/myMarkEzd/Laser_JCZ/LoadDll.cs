using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Laser_JCZ
{
	public class LoadDll
	{
		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_Initial2")]
		protected static extern LmcErrCode LMC1_INITIAL(string strEzCadPath, int bTestMode);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_Close")]
		protected static extern LmcErrCode LMC1_CLOSE();

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_StopMark")]
		protected static extern LmcErrCode LMC1_STOPMARK();

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_LoadEzdFile")]
		protected static extern LmcErrCode LMC1_LOADEZDFILE(string strFileName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_AddFileToLib")]
		protected static extern LmcErrCode LMC1_ADDFILETOLIB(string strFilePath, string strEntName, double dLeftDownXPos, double dLeftDowmYPos, double dZpos, int nAlign, double dScaleRation, int nPenNo, bool bHachFile);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SaveEntLibToFile")]
		protected static extern LmcErrCode LMC1_SAVEENTLIBTOFILE(string strFileName);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_AddCurveToLib")]
		public static extern LmcErrCode LMC1_ADDCURVETOLIB([MarshalAs(UnmanagedType.LPArray)] double[,] PtBuf, int ptNum, string strEntName, int nPenNo, int bHatch);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SetHatchParam")]
		public static extern LmcErrCode LMC1_SETHATCHPARAM(bool bEnableContour, int bEnableHacth1, int nPenNo1, int nHatchAttribute1, double dHatchEdge1, double dHatchDist1, double dHatchStartOffset1, double dHatchEndOffset1, double dHatchAngle1, int bEnableHacth2, int nPenNo2, int nHatchAttribute2, double dHatchEdge2, double dHatchDist2, double dHatchStartOffset2, double dHatchEndOffset2, double dHatchAngle2);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SetHatchParam2")]
		public static extern LmcErrCode LMC1_SETHATCHPARAM2(bool bEnableContour, int nParamIndex, int bEnableHatch, int nPenNo, int nHatchType, bool bHatchAllCalc, bool bHatchEdge, bool bHatchAverageLine, double dHatchAngle, double dHatchLineDist, double dHatchEdgeDist, double dHatchStartOffset, double dHatchEndOffset, double dHatchLineReduction, double dHatchLoopDist, int nEdgeLoop, bool nHatchLoopRev, bool bHatchAutoRotate, double dHatchRotateAngle);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_HatchEnt")]
		protected static extern LmcErrCode LMC1_HATCHENT(string pEntName, string strNewName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_CopyEnt")]
		protected static extern LmcErrCode LMC1_COPYENT(string strEntName, string strNewName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetTextByName")]
		protected static extern LmcErrCode LMC1_GETTEXTBYNAME(string pEntName, char[] strText);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_MoveEnt")]
		protected static extern LmcErrCode LMC1_MOVEENT(string pEntName, double dMovex, double dMovey);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_RotateEnt")]
		protected static extern LmcErrCode LMC1_ROTATEENT(string strEntName, double dCenx, double dCeny, double dAngle);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GroupEnt")]
		protected static extern LmcErrCode LMC1_GROUPENT(string strEntName1, string strEntName2, string strNewGroupName, int nPenNo);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_UnGroupEnt")]
		protected static extern LmcErrCode LMC1_UNGROUPENT(string strGroupEntName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ChangeTextByName")]
		protected static extern LmcErrCode LMC1_CHANGETEXTBYNAME(string strEntName, string strText);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetPrevBitmap2")]
		protected static extern IntPtr LMC1_GETPREVBITMAP2(int nBMPWIDTH, int nBMPHEIGHT);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_DeleteEnt")]
		protected static extern LmcErrCode LMC1_DELETEENT(string strEntName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ScaleEnt")]
		protected static extern LmcErrCode LMC1_SCALEENT(string strEntName, double dCenterX, double dCenterY, double dScaleX, double dScaleY);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetEntityCount")]
		protected static extern int LMC1_GETENTCOUNT();

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetEntSize")]
		protected static extern LmcErrCode LMC1_GETENTSIZE(string strEntName, ref double dMinx, ref double dMiny, ref double dMaxx, ref double dMaxy, ref double dZ);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetEntityCount")]
		protected static extern int LMC1_GETENTTITYCOUNT();

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetEntityName")]
		protected static extern LmcErrCode LMC1_GETENTITYNAME(int nEntityIndex, char[] strEntName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SetEntityName")]
		protected static extern LmcErrCode LMC1_SETENTITYNAME(int nEntityIndex, string strEntName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetBitmapEntParam3")]
		protected static extern LmcErrCode LMC1_GETBITMAPENTPARAM3(string strEntName, ref double dDpiX, ref double dDpiY, byte[] bGrayScaleBuff);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SetBitmapEntParam3")]
		protected static extern LmcErrCode LMC1_SETBITMAPENTPARAM3(string strEntName, double dDpiX, double dDpiY, byte[] bGrayScaleBuff);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SetBitmapEntParam2")]
		public static extern LmcErrCode LMC1_SETBITMAPENTPARAM2(string strEntName, string strImageFileName, int nBmpAttrib, int nScanAttrib, double dBrightness, double dContrast, double dPointTime, int nImportDpi, bool bDisableMarkLowGrayPt, int nMinLowGrayPt);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetBitmapEntParam2")]
		public static extern LmcErrCode LMC1_GETBITMAPENTPARAM2(string strEntName, StringBuilder strImageFileName, ref int nBmpAttrib, ref int nScanAttrib, ref double dBrightness, ref double dContrast, ref double dPointTime, ref int nImportDpi, ref int bDisableMarkLowGrayPt, ref int nMinLowGrayPt);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetPenNumberFromEnt")]
		protected static extern int LMC1_GETPENNUMBERFROMENT(string strEntName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetPenNumberFromName")]
		protected static extern int LMC1_GETPENNUMBERFROMNAME(string strEntName);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_Mark")]
		protected static extern LmcErrCode LMC1_MARK(int nFly);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_RedLightMark")]
		protected static extern LmcErrCode LMC1_REDLIGHTMARK();

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_WritePort")]
		protected static extern LmcErrCode LMC1_WRITEPORT(int nData);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ReadPort")]
		protected static extern LmcErrCode LMC1_READPORT(ref int nData);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetOutPort")]
		protected static extern LmcErrCode LMC1_GETOUTPORT(ref int nData);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SetDevCfg")]
		protected static extern LmcErrCode LMC1_SETDEVCFG();

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetPenParam")]
		protected static extern LmcErrCode LMC1_GETPENPARAM(int nPenNo, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double dQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nJumpPosTC, ref int nJumpDistTC, ref double dEndComp, ref double dAccDist, ref double dPointTime, ref bool bPulsePointMode, ref int nPulseNum, ref double dFlySpeed);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetPenParam2")]
		protected static extern LmcErrCode LMC1_GETPENPARAM2(int nPenNo, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double dQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nJumpPosTC, ref int nJumpDistTC, ref double dPointTime, ref int nSpiWave, ref bool bWobbleMode, ref double dWobbleDiameter, ref double bWobbleDist);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_SetPenParam")]
		protected static extern LmcErrCode LMC1_SETPENPARAM(int nPenNo, int nMarkLoop, double dMarkSpeed, double dPowerRatio, double dCurrent, int nFreq, double dQPulseWidth, int nStartTC, int nLaserOffTC, int nEndTC, int nPolyTC, double dJumpSpeed, int nJumpPosTC, int nJumpDistTC, double dEndComp, double dAccDist, double dPointTime, bool bPulsePointMode, int nPulseNum, double dFlySpeed);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_AxisMoveTo")]
		public static extern LmcErrCode LMC1_AXISMOVETO(int axis, double GoalPos);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_AxisMoveToPulse")]
		public static extern LmcErrCode LMC1_AXISMOVETOPULSE(int axis, int nPluse);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_AxisCorrectOrigin")]
		public static extern LmcErrCode LMC1_AXISCORRECTORIGIN(int axis);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_Reset")]
		public static extern LmcErrCode LMC1_RESET(bool bAxis1, bool bAxis2);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_GetAxisCoorPulse")]
		public static extern int LMC1_GETAXISCOORPULSE(int axis);

		[DllImport("MarkEzd.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "lmc1_MarkEntity")]
		public static extern LmcErrCode LMC1_MARKENTITY(string strEntName);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ContinueBufferClear")]
		public static extern LmcErrCode LMC1_CONTINUEBUFFERCLEAR();

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ContinueBufferFlyGetParam")]
		public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYGETPARAM(ref int nTotalMarkCount, ref int nBufferLeftCount);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ContinueBufferPartFinish")]
		public static extern LmcErrCode LMC1_CONTINUEBUFFERPARTFINISH();

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ContinueBufferFlyAdd")]
		public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYADD(int nNum, string Text1, string Text2, string Text3, string Text4, string Text5, string Text6);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ContinueBufferSetAddMode")]
		public static extern LmcErrCode LMC1_CONTINUEBUFFERSETADDMODE(bool FullMode);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ContinueBufferSetTextName")]
		public static extern LmcErrCode LMC1_CONTINUEBUFFERSETTEXTNAME(string Name1, string Name2, string Name3, string Name4, string Name5, string Name6);

		[DllImport("MarkEzd", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "lmc1_ContinueBufferFlyStart")]
		public static extern LmcErrCode LMC1_CONTINUEBUFFERFLYSTART(int nCount);

		[DllImport("gdi32.dll")]
		protected static extern bool DeleteObject(IntPtr hObject);

		[DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "PostMessage")]
		protected static extern bool POSTMESSAGE(IntPtr hwnd, int msg, uint wParam, uint lParam);
	}
}
