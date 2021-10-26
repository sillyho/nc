using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Laser_JCZ
{
	public class MarkJcz : LoadDll
	{
		public static bool mIsInitLaser = false;

		private static string mMarkEzdFile = null;

		public static LmcErrCode mLastError = LmcErrCode.LMC1_ERR_SUCCESS;

		private static Thread tRedLight = null;

		private static Thread threadInitLaser = null;

		private static object ObjectLock = new object();

		private static bool m_bRed = false;

		public static string GetLastError()
		{
			string result = "Success";
			switch (mLastError)
			{
			case LmcErrCode.LMC1_ERR_EZCADRUN:
				result = "已有一个程序在使用EzdCad";
				break;
			case LmcErrCode.LMC1_ERR_NOFINDCFGFILE:
				result = "未找到激光配置文件EZCAD.CFG";
				break;
			case LmcErrCode.LMC1_ERR_FAILEDOPEN:
				result = "打开LMC1卡失败";
				break;
			case LmcErrCode.LMC1_ERR_NODEVICE:
				result = "没有有效的lmc1设备";
				break;
			case LmcErrCode.LMC1_ERR_HARDVER:
				result = "lmc1版本错误";
				break;
			case LmcErrCode.LMC1_ERR_DEVCFG:
				result = "找不到设备配置文件";
				break;
			case LmcErrCode.LMC1_ERR_STOPSIGNAL:
				result = "报警信号";
				break;
			case LmcErrCode.LMC1_ERR_USERSTOP:
				result = "用户停止";
				break;
			case LmcErrCode.LMC1_ERR_NOFINDFONT:
				result = "找不到该实体";
				break;
			case LmcErrCode.LMC1_ERR_UNKNOW:
				result = "不明错误";
				break;
			case LmcErrCode.LMC1_ERR_OUTTIME:
				result = "超时";
				break;
			case LmcErrCode.LMC1_ERR_SAVEFILE:
				result = "保存文件失败";
				break;
			case LmcErrCode.LMC1_ERR_STATUE:
				result = "当前状态下不能执行此操作";
				break;
			case LmcErrCode.LMC1_ERR_BRAND:
				result = "未连接HGLASER打标卡";
				break;
			case LmcErrCode.LMC1_ERROR_NOEZDFILE:
				result = "Ezd文件路径不存在";
				break;
			case LmcErrCode.LMC1_ERROR_NOINIT:
				result = "打标卡未初始化";
				break;
			case LmcErrCode.LMC1_ERROR_NOFINDMARKEZD:
				result = "文件Markezd.dll丢失";
				break;
			case LmcErrCode.LMC1_ERROR_NOUSBDOG:
				result = "无软件加密狗";
				break;
			}
			return result;
		}

		public static bool InitLaser()
		{
			mIsInitLaser = false;
			string text = Application.StartupPath + "\\";
			if (!File.Exists(text + "markezd.dll"))
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOFINDMARKEZD;
				return false;
			}
			LmcErrCode lmcErrCode = LoadDll.LMC1_INITIAL(text, 0);
			if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
			{
				mIsInitLaser = true;
				return true;
			}
			mLastError = lmcErrCode;
			return false;
		}

		public static bool LoadEzdFile(string strFile, bool bDialog = false)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			if (!File.Exists(strFile))
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOEZDFILE;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_LOADEZDFILE(strFile);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					mMarkEzdFile = strFile;
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool AddFileToLib(string strFilePath, string strEntName, double dLeftDownXPos, double dLeftDowmYPos, double dZpos, int nAlign)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_ADDFILETOLIB(strFilePath, strEntName, dLeftDownXPos, dLeftDowmYPos, dZpos, nAlign, 1.0, 0, false);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool SaveFile(string strFileName = "")
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			if (strFileName == "")
			{
				strFileName = mMarkEzdFile;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SAVEENTLIBTOFILE(strFileName);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool DeleteAllEnt()
		{
			int entityCount = GetEntityCount();
			for (int i = 0; i < entityCount; i++)
			{
				SetEntName(i, "delete");
			}
			return DeleteEnt("delete");
		}

		public static bool AddCircleToLib(string pEntName, double dCenterX, double dCenterY, double dR, int nPointNum, int nPenNo, int nHach)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				double[,] array = new double[nPointNum, 2];
				for (int i = 1; i <= nPointNum; i++)
				{
					array[i - 1, 0] = dR * Math.Cos((double)(360 * i / nPointNum) * Math.PI / 180.0) + dCenterX;
					array[i - 1, 1] = dR * Math.Sin((double)(360 * i / nPointNum) * Math.PI / 180.0) + dCenterY;
				}
				LmcErrCode lmcErrCode = LoadDll.LMC1_ADDCURVETOLIB(array, nPointNum, pEntName, nPenNo, nHach);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool AddCurveToLib(string pEntName, double[,] dPointArry, int nPointNum, int nPenNo, int nHach)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_ADDCURVETOLIB(dPointArry, nPointNum, pEntName, nPenNo, nHach);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool SetHatchParams(bool bEnableContour, int bEnableHacth1, int nPenNo1, int nHatchAttribute1, double dHatchEdge1, double dHatchDist1, double dHatchStartOffset1, double dHatchEndOffset1, double dHatchAngle1, int bEnableHacth2, int nPenNo2, int nHatchAttribute2, double dHatchEdge2, double dHatchDist2, double dHatchStartOffset2, double dHatchEndOffset2, double dHatchAngle2)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SETHATCHPARAM(bEnableContour, bEnableHacth1, nPenNo1, nHatchAttribute1, dHatchEdge1, dHatchDist1, dHatchStartOffset1, dHatchEndOffset1, dHatchAngle1, bEnableHacth2, nPenNo2, nHatchAttribute2, dHatchEdge2, dHatchDist2, dHatchStartOffset2, dHatchEndOffset2, dHatchAngle2);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool SetHatchParam2(bool bEnableContour, int nParamIndex, int bEnableHatch, int nPenNo, int nHatchType, bool bHatchAllCalc, bool bHatchEdge, bool bHatchAverageLine, double dHatchAngle, double dHatchLineDist, double dHatchEdgeDist, double dHatchStartOffset, double dHatchEndOffset, double dHatchLineReduction, double dHatchLoopDist, int nEdgeLoop, bool nHatchLoopRev, bool bHatchAutoRotate, double dHatchRotateAngle)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SETHATCHPARAM2(bEnableContour, nParamIndex, bEnableHatch, nPenNo, nHatchType, bHatchAllCalc, bHatchEdge, bHatchAverageLine, dHatchAngle, dHatchLineDist, dHatchEdgeDist, dHatchStartOffset, dHatchEndOffset, dHatchLineReduction, dHatchLoopDist, nEdgeLoop, nHatchLoopRev, bHatchAutoRotate, dHatchRotateAngle);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool HatchEnt(string strName, string strNewName)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_HATCHENT(strName, strNewName);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool MoveRotateAllEnt(double dOffsetX, double doffsetY, double dCenterX, double dCenterY, double dR)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				char[] array = new char[256];
				int num = LoadDll.LMC1_GETENTCOUNT();
				for (int i = 0; i < num; i++)
				{
					LoadDll.LMC1_GETENTITYNAME(i, array);
					string text = new string(array).Replace("\0", "") + "moveallent" + i.ToString();
					LoadDll.LMC1_SETENTITYNAME(i, text);
					RoTateEnt(text, dCenterX, dCenterY, dR);
					MoveEnt(text, dOffsetX, doffsetY);
				}
			}
			return true;
		}

		public static bool MoveEnt(string pEntName, double dMovex, double dMovey)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				if (pEntName == "")
				{
					for (int i = 0; i < GetEntityCount(); i++)
					{
						string text = "AutoOperator";
						char[] array = new char[256];
						LoadDll.LMC1_GETENTITYNAME(i, array);
						SetEntName(i, text);
						LoadDll.LMC1_MOVEENT(text, dMovex, dMovey);
						SetEntName(i, new string(array));
					}
					return true;
				}
				LmcErrCode lmcErrCode = LoadDll.LMC1_MOVEENT(pEntName, dMovex, dMovey);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool RoTateEnt(string strEntName, double dCenx, double dCeny, double dAngle)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_ROTATEENT(strEntName, dCenx, dCeny, dAngle);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool RoTateEnt(string strEntName, double dAngle)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			double dx = 0.0;
			double dy = 0.0;
			lock (ObjectLock)
			{
				GetEntPos(strEntName, ref dx, ref dy);
				LmcErrCode lmcErrCode = LoadDll.LMC1_ROTATEENT(strEntName, dx, dy, dAngle);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool GroupEnt(string strEntName1, string strEntName2, string strNewGroupName, int nPenNo)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GROUPENT(strEntName1, strEntName2, strNewGroupName, nPenNo);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool UnGroupEnt(string strEntName)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_UNGROUPENT(strEntName);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ChangeTextByName(string strEntName, string strText)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CHANGETEXTBYNAME(strEntName, strText);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static void ShowPreviewBmp(PictureBox pictureBox)
		{
			if (!mIsInitLaser)
			{
				MessageBox.Show("激光器没有初始化");
				return;
			}
			int width = pictureBox.Size.Width;
			int height = pictureBox.Size.Height;
			lock (ObjectLock)
			{
				if (pictureBox.InvokeRequired)
				{
					pictureBox.Invoke((EventHandler)delegate
					{
						IntPtr intPtr2 = LoadDll.LMC1_GETPREVBITMAP2(width, height);
						pictureBox.Image = Image.FromHbitmap(intPtr2);
						LoadDll.DeleteObject(intPtr2);
					});
				}
				else
				{
					IntPtr intPtr = LoadDll.LMC1_GETPREVBITMAP2(width, height);
					pictureBox.Image = Image.FromHbitmap(intPtr);
					LoadDll.DeleteObject(intPtr);
				}
			}
		}

		public static bool DeleteEnt(string strEntName)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_DELETEENT(strEntName);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ScaleEnt(string strEntName, double dCenterX, double dCenterY, double dScaleX, double dScaleY)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SCALEENT(strEntName, dCenterX, dCenterY, dScaleX, dScaleY);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool SetEntName(int nIndex, string strEntName)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SETENTITYNAME(nIndex, strEntName);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool GetEntityNameByIndex(int nIndex, ref string strName)
		{
			string text = "";
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			char[] array = new char[256];
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETENTITYNAME(nIndex, array);
				if (lmcErrCode != 0)
				{
					mLastError = lmcErrCode;
					return false;
				}
				text = new string(array);
				strName = text.Trim("\0".ToCharArray());
				return true;
			}
		}

		public static bool GetTextByName(string strEntName, string strValue)
		{
			char[] array = new char[256];
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETTEXTBYNAME(strEntName, array);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					strValue = array.ToString();
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static int GetEntityCount()
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return 0;
			}
			lock (ObjectLock)
			{
				return LoadDll.LMC1_GETENTTITYCOUNT();
			}
		}

		public static bool GetEntPos(string strEntName, ref double dx, ref double dy)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				double dMinx = 0.0;
				double dMiny = 0.0;
				double dMaxx = 0.0;
				double dMaxy = 0.0;
				double dZ = 0.0;
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETENTSIZE(strEntName, ref dMinx, ref dMiny, ref dMaxx, ref dMaxy, ref dZ);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					dx = (dMinx + dMaxx) / 2.0;
					dy = (dMiny + dMaxy) / 2.0;
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool GetEntitySize(string strEntName, ref double dx, ref double dy)
		{
			double dMinx = 0.0;
			double dMaxx = 0.0;
			double dMiny = 0.0;
			double dMaxy = 0.0;
			double dZ = 0.0;
			if (!GetEntSize(strEntName, ref dMinx, ref dMiny, ref dMaxx, ref dMaxy, ref dZ))
			{
				return false;
			}
			dx = dMaxx - dMinx;
			dy = dMaxy - dMiny;
			return true;
		}

		public static bool GetEntSize(string strEntName, ref double dMinx, ref double dMiny, ref double dMaxx, ref double dMaxy, ref double dZ)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETENTSIZE(strEntName, ref dMinx, ref dMiny, ref dMaxx, ref dMaxy, ref dZ);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool SetBitMapEntParam2(string EntName, string strImageFileName, int nBmpAttrib, int nScanAttrib, double dBrightness, double dContrast, double dPointTime, int nImportDpi, bool bDisableMarkLowGrayPt, int nMinLowGrayPt)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SETBITMAPENTPARAM2(EntName, strImageFileName, nBmpAttrib, nScanAttrib, dBrightness, dContrast, dPointTime, nImportDpi, bDisableMarkLowGrayPt, nMinLowGrayPt);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool GetBitMapEntParam2(string EntName, StringBuilder strImageFileName, ref int nBmpAttrib, ref int nScanAttrib, ref double dBrightness, ref double dContrast, ref double dPointTime, ref int nImportDpi, ref int bDisableMarkLowGrayPt, ref int nMinLowGrayPt)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETBITMAPENTPARAM2(EntName, strImageFileName, ref nBmpAttrib, ref nScanAttrib, ref dBrightness, ref dContrast, ref dPointTime, ref nImportDpi, ref bDisableMarkLowGrayPt, ref nMinLowGrayPt);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool GetBitmapEntParam3(string strEntName, ref double dDpiX, ref double dDpiY, byte[] bGrayScaleBuff, int nArryLen)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETBITMAPENTPARAM3(strEntName, ref dDpiX, ref dDpiY, bGrayScaleBuff);
				if (lmcErrCode != 0)
				{
					mLastError = lmcErrCode;
					return false;
				}
			}
			return true;
		}

		public static bool SetBitmapEntParam3(string strEntName, double dDpiX, double dDpiY, byte[] bGrayScaleBuff, int nArryLen)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SETBITMAPENTPARAM3(strEntName, dDpiX, dDpiY, bGrayScaleBuff);
				if (lmcErrCode != 0)
				{
					mLastError = lmcErrCode;
					return false;
				}
			}
			return true;
		}

		public static bool GetPenParam(int nPenNo, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double dQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nJumpPosTC, ref int nJumpDistTC, ref double dEndComp, ref double dAccDist, ref double dPointTime, ref bool bPulsePointMode, ref int nPulseNum, ref double dFlySpeed)
		{
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETPENPARAM(nPenNo, ref nMarkLoop, ref dMarkSpeed, ref dPowerRatio, ref dCurrent, ref nFreq, ref dQPulseWidth, ref nStartTC, ref nLaserOffTC, ref nEndTC, ref nPolyTC, ref dJumpSpeed, ref nJumpPosTC, ref nJumpDistTC, ref dEndComp, ref dAccDist, ref dPointTime, ref bPulsePointMode, ref nPulseNum, ref dFlySpeed);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool GetPenParam2(int nPenNo, ref int nMarkLoop, ref double dMarkSpeed, ref double dPowerRatio, ref double dCurrent, ref int nFreq, ref double dQPulseWidth, ref int nStartTC, ref int nLaserOffTC, ref int nEndTC, ref int nPolyTC, ref double dJumpSpeed, ref int nJumpPosTC, ref int nJumpDistTC, ref double dPointTime, ref int nSpiWave, ref bool bWobbleMode, ref double bWobbleDiameter, ref double bWobbleDist)
		{
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETPENPARAM2(nPenNo, ref nMarkLoop, ref dMarkSpeed, ref dPowerRatio, ref dCurrent, ref nFreq, ref dQPulseWidth, ref nStartTC, ref nLaserOffTC, ref nEndTC, ref nPolyTC, ref dJumpSpeed, ref nJumpPosTC, ref nJumpDistTC, ref dPointTime, ref nSpiWave, ref bWobbleMode, ref bWobbleDiameter, ref bWobbleDist);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool SetPenParams(int nPenNo, int nMarkLoop, double dMarkSpeed, double dPowerRatio, double dCurrent, int nFreq, double dQPulseWidth, int nStartTC, int nLaserOffTC, int nEndTC, int nPolyTC, double dJumpSpeed, int nJumpPosTC, int nJumpDistTC, double dEndComp, double dAccDist, double dPointTime, bool bPulsePointMode, int nPulseNum, double dFlySpeed)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_SETPENPARAM(nPenNo, nMarkLoop, dMarkSpeed, dPowerRatio, dCurrent, nFreq, dQPulseWidth, nStartTC, nLaserOffTC, nEndTC, nPolyTC, dJumpSpeed, nJumpPosTC, nJumpDistTC, dEndComp, dAccDist, dPointTime, bPulsePointMode, nPulseNum, dFlySpeed);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ReadPort(ref int nValue)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			nValue = 0;
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_READPORT(ref nValue);
				if (lmcErrCode != 0)
				{
					mLastError = lmcErrCode;
					return false;
				}
				return true;
			}
		}

		public static bool ReadPort(int nPort)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			int nData = 0;
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_READPORT(ref nData);
				if (lmcErrCode != 0)
				{
					mLastError = lmcErrCode;
					return false;
				}
				if (((nData >> nPort) & 1) > 0)
				{
					return true;
				}
				return false;
			}
		}

		public static bool WritePort(int nPort, bool bState)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			if (nPort < 0 || nPort > 15)
			{
				mLastError = LmcErrCode.LMC1_ERROR_OUTOFPORTRANGE;
				return false;
			}
			int nData = 0;
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_GETOUTPORT(ref nData);
				if (lmcErrCode != 0)
				{
					mLastError = lmcErrCode;
					return false;
				}
				int num = 0;
				if (bState)
				{
					num = 1 << nPort;
					nData |= num;
				}
				else
				{
					num = ~(1 << nPort);
					nData &= num;
				}
				lmcErrCode = LoadDll.LMC1_WRITEPORT(nData);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool Mark(bool bFlay = false)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			int nFly = 0;
			if (bFlay)
			{
				nFly = 1;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_MARK(nFly);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool MarkEntity(string strEntityName)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_MARKENTITY(strEntityName);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static void StartRed(int nMs = 100)
		{
			if (tRedLight != null && tRedLight.IsAlive)
			{
				m_bRed = false;
			}
			m_bRed = true;
			tRedLight = new Thread(RedOn);
			tRedLight.Start();
		}

		public static void RedOn()
		{
			while (m_bRed)
			{
				lock (ObjectLock)
				{
					LoadDll.LMC1_REDLIGHTMARK();
					Thread.Sleep(50);
				}
			}
		}

		public static void StopRed()
		{
			m_bRed = false;
		}

		public static bool ContinueBufferClear()
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CONTINUEBUFFERCLEAR();
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ContinueBufferFlyGetParam(ref int nTotalMarkCount, ref int nBufferLeftCount)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CONTINUEBUFFERFLYGETPARAM(ref nTotalMarkCount, ref nBufferLeftCount);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ContinueBufferFlyAdd(int nNum, string Text1, string Text2, string Text3, string Text4, string Text5, string Text6)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CONTINUEBUFFERFLYADD(nNum, Text1, Text2, Text3, Text4, Text5, Text6);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ContinueBufferSetTextName(string Name1, string Name2, string Name3, string Name4, string Name5, string Name6)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CONTINUEBUFFERSETTEXTNAME(Name1, Name2, Name3, Name4, Name5, Name6);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ContinueBufferFlyStart(int nCount)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CONTINUEBUFFERFLYSTART(nCount);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ContinueBufferSetAddMode(bool bFullModel)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CONTINUEBUFFERSETADDMODE(bFullModel);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool ContinueBufferPartFinish()
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_CONTINUEBUFFERPARTFINISH();
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}

		public static bool Close()
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			LmcErrCode lmcErrCode = LoadDll.LMC1_CLOSE();
			if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
			{
				mIsInitLaser = false;
				return true;
			}
			mLastError = lmcErrCode;
			return false;
		}

		public static void StopMark()
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return;
			}
			LmcErrCode lmcErrCode = LoadDll.LMC1_STOPMARK();
			if (lmcErrCode != 0)
			{
				mLastError = lmcErrCode;
			}
		}

		public static int GetAxisCoorPulse(int nAxis)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return -1;
			}
			return LoadDll.LMC1_GETAXISCOORPULSE(nAxis);
		}

		public static bool AxisMoveTOPulse(int nAxis, int nPulse)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			LmcErrCode lmcErrCode = LoadDll.LMC1_AXISMOVETOPULSE(nAxis, nPulse);
			if (lmcErrCode != 0)
			{
				mLastError = lmcErrCode;
				return false;
			}
			return true;
		}

		public static bool Reset(bool nAxis1, bool nAxis2)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			LmcErrCode lmcErrCode = LoadDll.LMC1_RESET(nAxis1, nAxis2);
			if (lmcErrCode != 0)
			{
				mLastError = lmcErrCode;
				return false;
			}
			return true;
		}

		public static int AxisCorrectOrigin(int Axis)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return -1;
			}
			LmcErrCode lmcErrCode = LoadDll.LMC1_AXISCORRECTORIGIN(Axis);
			if (lmcErrCode != 0)
			{
				mLastError = lmcErrCode;
				return -1;
			}
			return 0;
		}

		public static bool EditMarkEzdForm(string strFilePath, string strEzdCadName)
		{
			if (threadInitLaser != null && threadInitLaser.IsAlive)
			{
				return false;
			}
			if (!File.Exists(Application.StartupPath + "\\" + strEzdCadName))
			{
				MessageBox.Show(strEzdCadName.ToString() + ":文件不存在");
				return false;
			}
			LoadEzdFile(strFilePath);
			mMarkEzdFile = strFilePath;
			threadInitLaser = new Thread(threadInitHglaser);
			threadInitLaser.Start(strEzdCadName);
			return true;
		}

		private static void threadInitHglaser(object strEzdCadName)
		{
			Close();
			Process process = new Process();
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.FileName = Application.StartupPath + "\\" + strEzdCadName.ToString();
			process.StartInfo.Arguments = mMarkEzdFile;
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			try
			{
				process.Start();
			}
			catch (Win32Exception ex)
			{
				process.Dispose();
				MessageBox.Show(ex.Message);
				return;
			}
			process.WaitForExit();
			process.Dispose();
			if (!InitLaser())
			{
				MessageBox.Show("初始化激光器失败!");
			}
		}

		public static void SetDeviceParams()
		{
			if (!mIsInitLaser)
			{
				MessageBox.Show("激光器没有初始化");
			}
			else
			{
				lock (ObjectLock)
				{
					LmcErrCode lmcErrCode = LoadDll.LMC1_SETDEVCFG();
					if (lmcErrCode != 0)
					{
						mLastError = lmcErrCode;
					}
				}
			}
		}

		public static bool PostMessage(IntPtr ptr, int msg, uint wParam, uint lParam)
		{
			return LoadDll.POSTMESSAGE(ptr, msg, wParam, lParam);
		}

		public static bool CopyEnt(string strSourceName, string strDesName)
		{
			if (!mIsInitLaser)
			{
				mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
				return false;
			}
			lock (ObjectLock)
			{
				LmcErrCode lmcErrCode = LoadDll.LMC1_COPYENT(strSourceName, strDesName);
				if (lmcErrCode == LmcErrCode.LMC1_ERR_SUCCESS)
				{
					return true;
				}
				mLastError = lmcErrCode;
				return false;
			}
		}
	}
}
