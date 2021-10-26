using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Timers;

namespace Laser_JCZ
{
    public class MarkJcz : LoadDll
    {
        
        public static bool mIsInitLaser = false;
        private static IntPtr m_Ptr ;
        private static string mMarkEzdFile = null;
        private static LmcErrCode mLastError = LmcErrCode.LMC1_ERR_SUCCESS;
        private static Thread tRedLight = null;
        private static Thread threadInitLaser = null;
        private static object ObjectLock = new object();
        private static bool m_bRed = false;

        public static string GetLastError()
        {
            string strErroeMessage = "Success";
            switch(mLastError)
            {
                case LmcErrCode.LMC1_ERR_EZCADRUN:
                    {
                        strErroeMessage = "已有一个程序在使用EzdCad";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_NOFINDCFGFILE:
                    {
                        strErroeMessage = "未找到激光配置文件EZCAD.CFG";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_FAILEDOPEN:
                    {
                        strErroeMessage = "打开LMC1卡失败";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_NODEVICE:
                    {
                        strErroeMessage = "没有有效的lmc1设备";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_HARDVER:
                    {
                        strErroeMessage = "lmc1版本错误";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_DEVCFG:
                    {
                        strErroeMessage = "找不到设备配置文件";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_STOPSIGNAL:
                    {
                        strErroeMessage = "报警信号";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_USERSTOP:
                    {
                        strErroeMessage = "用户停止";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_NOFINDFONT:
                    {
                        strErroeMessage = "找不到该实体";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_UNKNOW:
                    {
                        strErroeMessage = "不明错误";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_OUTTIME:
                    {
                        strErroeMessage = "超时";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_SAVEFILE:
                    {
                        strErroeMessage = "保存文件失败";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_STATUE:
                    {
                        strErroeMessage = "当前状态下不能执行此操作";
                        break;
                    }
                case LmcErrCode.LMC1_ERR_BRAND:
                    {
                        strErroeMessage = "未连接HGLASER打标卡";
                        break;
                    }
                case LmcErrCode.LMC1_ERROR_NOEZDFILE:
                    {
                        strErroeMessage = "Ezd文件路径不存在";
                        break;
                    }
                case LmcErrCode.LMC1_ERROR_NOINIT:
                    {
                        strErroeMessage = "打标卡未初始化";
                        break;
                    }
                case LmcErrCode.LMC1_ERROR_NOFINDMARKEZD:
                    {
                        strErroeMessage = "文件Markezd.dll丢失";
                        break;
                    }
                    default:
                        break;
            }
            return strErroeMessage;
        }
        //初始化及加载保存
        public static bool InitLaser(IntPtr hwnd)
        {
            mIsInitLaser = false;
            m_Ptr = hwnd;
            string strEzCadPath = Application.StartupPath + "\\";
            if (!File.Exists(strEzCadPath+"markezd.dll"))
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOFINDMARKEZD;
                return false;
            }
            LmcErrCode Ret = LMC1_INITIAL(strEzCadPath, 0, hwnd);
            if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
            {
                mIsInitLaser = true;
                return true;
            }
            else
            {
                mLastError = Ret;
                return false;
            }
            
        }
        public static bool LoadEzdFile(string strFile, bool bDialog = false)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            if (!System.IO.File.Exists(strFile))
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOEZDFILE;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_LOADEZDFILE(strFile);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    mMarkEzdFile = strFile;
                    return true;
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_ADDFILETOLIB(strFilePath, strEntName, dLeftDownXPos, dLeftDowmYPos, dZpos, nAlign, 1, 0, false);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    return true;
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_SAVEENTLIBTOFILE(strFileName);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
        //对象操作
        public static bool DeleteAllEnt()
        {
            int nCount = GetEntityCount();
            for (int i = 0; i < nCount;i++ )
            {
                SetEntName(i,"delete");
            }
            return DeleteEnt("delete");
        }
        public static bool AddCircleToLib(string pEntName, double dCenterX, double dCenterY, double dR,int nPointNum,int nPenNo,int nHach)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                double[,] dAddCirclePoint = new double[nPointNum, 2];
                for (int i = 1; i <= nPointNum; i++)
                {
                    dAddCirclePoint[i - 1, 0] = dR * Math.Cos((360*i / nPointNum) * Math.PI / 180) + dCenterX;
                    dAddCirclePoint[i - 1, 1] = dR * Math.Sin((360*i / nPointNum) * Math.PI / 180) + dCenterY;
                }
                //最后一个点需要时第一个点，暂时忽略
                LmcErrCode lec = LMC1_ADDCURVETOLIB(dAddCirclePoint, nPointNum, pEntName, nPenNo, nHach);
                if (lec == LmcErrCode.LMC1_ERR_SUCCESS)
                {
                    return true;
                }
                else
                {
                    mLastError = lec;
                    return false;
                }
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
                LmcErrCode lec = LMC1_ADDCURVETOLIB(dPointArry, nPointNum, pEntName, nPenNo, nHach);
                if (lec == LmcErrCode.LMC1_ERR_SUCCESS)
                {
                    return true;
                }
                else
                {
                    mLastError = lec;
                    return false;
                }
            }
        }
        public static bool SetHatchParams(bool bEnableContour,//使能轮廓本身
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
                                          double dHatchAngle2)
        {

            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_SETHATCHPARAM( bEnableContour,//使能轮廓本身
                                           bEnableHacth1,
                                           nPenNo1,
                                           nHatchAttribute1,
                                           dHatchEdge1,
                                           dHatchDist1,
                                           dHatchStartOffset1,
                                           dHatchEndOffset1,
                                           dHatchAngle1,
                                           bEnableHacth2,
                                           nPenNo2,
                                           nHatchAttribute2,
                                           dHatchEdge2,
                                           dHatchDist2,
                                           dHatchStartOffset2,
                                           dHatchEndOffset2,
                                           dHatchAngle2);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }
        }
        public static bool SetHatchParam2(bool bEnableContour,//使能轮廓本身
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
                                          double dHatchRotateAngle)//自动旋转角度  
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_SETHATCHPARAM2(bEnableContour,//使能轮廓本身
                                           nParamIndex,
                                           bEnableHatch,
                                           nPenNo,
                                           nHatchType,
                                           bHatchAllCalc,
                                           bHatchEdge,
                                           bHatchAverageLine,
                                           dHatchAngle,
                                           dHatchLineDist,
                                           dHatchEdgeDist,
                                           dHatchStartOffset,
                                           dHatchEndOffset,
                                           dHatchLineReduction,
                                           dHatchLoopDist,
                                           nEdgeLoop,
                                           nHatchLoopRev,
                                           bHatchAutoRotate, 
                                           dHatchRotateAngle);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
            
        public static bool HatchEnt(string strName,string strNewName)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_HATCHENT(strName, strNewName);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }
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
                if (pEntName=="")
                {
                    for (int i = 0; i < GetEntityCount();i++ )
                    {
                        string strTempname = "AutoOperator";
                        char[] strArryName=new char[256];
                        LMC1_GETENTITYNAME(i, strArryName);
                        SetEntName(i, strTempname);
                        LMC1_MOVEENT(strTempname, dMovex, dMovey);
                        SetEntName(i, new string(strArryName));
                    }
                    return true;
                }
                LmcErrCode Ret = LMC1_MOVEENT(pEntName, dMovex, dMovey);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_ROTATEENT(strEntName, dCenx, dCeny, dAngle);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_GROUPENT(strEntName1, strEntName2, strNewGroupName, nPenNo);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_UNGROUPENT(strEntName);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_CHANGETEXTBYNAME(strEntName, strText);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
        public static void ShowPreviewBmp(System.Windows.Forms.PictureBox pictureBox)
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
                    pictureBox.Invoke((EventHandler)(delegate
                    {
                        IntPtr ptr = LMC1_GETPREVBITMAP2(width, height);
                        pictureBox.Image = Bitmap.FromHbitmap(ptr);
                        DeleteObject(ptr);
                    }));
                }
                else
                {
                    IntPtr ptr = LMC1_GETPREVBITMAP2(width, height);
                    pictureBox.Image = Bitmap.FromHbitmap(ptr);
                    DeleteObject(ptr);
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
                LmcErrCode Ret = LMC1_DELETEENT(strEntName);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_SCALEENT(strEntName, dCenterX, dCenterY, dScaleX, dScaleY);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    return true;
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_SETENTITYNAME(nIndex, strEntName);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
        public static bool GetEntityNameByIndex(int nIndex, ref string strName)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            char[] strTemp = new char[256];
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_GETENTITYNAME(nIndex, strTemp);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    strName = new string(strTemp);
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
                return true;
            }

        }
        public static bool GetTextByName(string strEntName, string strValue)
        {
   
            char[] chEnt = new char[256];
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_GETTEXTBYNAME(strEntName,chEnt);

                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    strValue = chEnt.ToString();
                    return true;
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }
      
        }
        //对象属性
        
        public static int GetEntityCount()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return 0;
            }
            lock (ObjectLock)
            {
                return LMC1_GETENTTITYCOUNT();
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
                double dMinx=0,   dMiny=0,   dMaxx=0,   dMaxy=0,   dZ=0;
                LmcErrCode Ret = LMC1_GETENTSIZE(strEntName, ref dMinx, ref dMiny, ref dMaxx, ref dMaxy, ref dZ);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    dx = (dMinx + dMaxx) / 2;
                    dy = (dMiny + dMaxy) / 2;
                    return true;
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
        public static bool GetEntitySize(string strEntName, ref double dx, ref double dy)
        {
            double dMinx = 0, dMaxx = 0, dMiny = 0, dMaxy = 0, dR = 0;
            if (!GetEntSize(strEntName,ref dMinx,ref  dMiny,ref dMaxx,ref dMaxy,ref dR))
            {
                return false;
            }
            dx = (dMaxx - dMinx);
            dy = (dMaxy - dMiny);
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
                LmcErrCode Ret = LMC1_GETENTSIZE(strEntName, ref dMinx, ref dMiny, ref dMaxx, ref dMaxy, ref dZ);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
        public static bool SetBitMapEntParam2(string EntName,
                                                          string strImageFileName,
                                                           int nBmpAttrib,
                                                           int nScanAttrib,
                                                           double dBrightness,
                                                           double dContrast,
                                                           double dPointTime,
                                                           int nImportDpi,
                                                           bool bDisableMarkLowGrayPt,
                                                           int nMinLowGrayPt)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {

                LmcErrCode Ret = LMC1_SETBITMAPENTPARAM2(EntName, strImageFileName, nBmpAttrib, nScanAttrib, dBrightness, dContrast, dPointTime, nImportDpi, bDisableMarkLowGrayPt, nMinLowGrayPt);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    return true;
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }
        }
        public static bool GetBitMapEntParam2(string EntName,StringBuilder strImageFileName,
                                                         ref int nBmpAttrib,
                                                         ref int nScanAttrib,
                                                         ref double dBrightness,
                                                         ref double dContrast,
                                                         ref double dPointTime,
                                                         ref int nImportDpi,
                                                         ref int bDisableMarkLowGrayPt,
                                                         ref int nMinLowGrayPt)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {

                LmcErrCode Ret = LMC1_GETBITMAPENTPARAM2(EntName, strImageFileName, ref nBmpAttrib, ref nScanAttrib, ref dBrightness, ref dContrast, ref dPointTime, ref nImportDpi, ref bDisableMarkLowGrayPt, ref nMinLowGrayPt);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    return true;
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }
        }
        public static bool GetBitmapEntParam3(string strEntName, ref double dDpiX, ref double dDpiY, Byte[] bGrayScaleBuff, int nArryLen)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_GETBITMAPENTPARAM3(strEntName, ref dDpiX, ref dDpiY, bGrayScaleBuff);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

            return true;
        }
        public static bool SetBitmapEntParam3(string strEntName, double dDpiX, double dDpiY, Byte[] bGrayScaleBuff, int nArryLen)
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_SETBITMAPENTPARAM3(strEntName, dDpiX, dDpiY, bGrayScaleBuff);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                }
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

            return true;

        }
        public static bool GetPenParam(int nPenNo,//要设置的笔号(0-255)					 
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
           ref double dFlySpeed)//流水线速度
         {
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_GETPENPARAM(nPenNo, ref nMarkLoop,//加工次数
                ref dMarkSpeed,//标刻次数mm/s
                ref dPowerRatio,//功率百分比(0-100%)	
                ref dCurrent,//电流A
                ref nFreq,//频率HZ
                ref dQPulseWidth,//Q脉冲宽度us	
                ref nStartTC,//开始延时us
                ref nLaserOffTC,//激光关闭延时us 
                ref nEndTC,//结束延时us
                ref nPolyTC,//拐角延时us   //	
                ref dJumpSpeed, //跳转速度mm/s
                ref nJumpPosTC, //跳转位置延时us
                ref  nJumpDistTC,//跳转距离延时us	
                ref  dEndComp,//末点补偿mm
                ref  dAccDist,//加速距离mm	
                ref  dPointTime,//打点延时 ms						 
                ref  bPulsePointMode,//脉冲点模式 
                ref  nPulseNum,//脉冲点数目
                ref  dFlySpeed);//流水线速度

                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }
           
        }
        public static bool SetPenParams(int nPenNo,int nMarkLoop,//加工次数
                                        double dMarkSpeed, //标刻次数mm/s
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
                                        double dFlySpeed)//流水
      {
          if (!mIsInitLaser)
          {
              mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
              return false;
          }
          lock (ObjectLock)
          {
                 LmcErrCode Ret= LMC1_SETPENPARAM(nPenNo, nMarkLoop, dMarkSpeed, dPowerRatio, dCurrent, nFreq, 
                      dQPulseWidth,nStartTC, nLaserOffTC, nEndTC, nPolyTC, dJumpSpeed, nJumpPosTC, 
                      nJumpDistTC, dEndComp,dAccDist, dPointTime, bPulsePointMode, nPulseNum, dFlySpeed);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }

            }
       }
        //IO接口
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
                LmcErrCode Ret = LMC1_READPORT(ref nValue);
                if (LmcErrCode.LMC1_ERR_SUCCESS != Ret)
                {
                    mLastError = Ret;
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
            int nState = 0;
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_READPORT(ref nState);
                if (LmcErrCode.LMC1_ERR_SUCCESS != Ret)
                {
                    mLastError = Ret;
                    return false;
                }
                if (((nState >> nPort) & 0x01) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
            int nState = 0;
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_GETOUTPORT(ref nState);
                if (LmcErrCode.LMC1_ERR_SUCCESS != Ret)
                {
                    mLastError = Ret;
                    return false;
                }
                int dbuff = 0;
                if (bState)
                {
                    dbuff = 0x0001 << nPort;
                    nState |= dbuff;
                }
                else
                {
                    dbuff = ~(0x0001 << nPort);
                    nState &= dbuff;
                }
                Ret = LMC1_WRITEPORT(nState);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
        //标刻,红光
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
                LmcErrCode Ret = LMC1_MARK(nFly);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }
       
        }
        public static void StartRed(int nMs = 100)
        {
            if (tRedLight!=null)
            {
                if (tRedLight.IsAlive)
                {
                    m_bRed = false;
                }
                
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
                    LmcErrCode Ret = LMC1_REDLIGHTMARK();
                    Thread.Sleep(50);
                }
            }
        }
        public static void StopRed()
        {
            m_bRed = false;
        }
        //飞动部分
        public static bool ContinueBufferClear()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_CONTINUEBUFFERCLEAR();
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_CONTINUEBUFFERFLYGETPARAM(ref nTotalMarkCount, ref nBufferLeftCount);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_CONTINUEBUFFERFLYADD(nNum, Text1, Text2, Text3, Text4, Text5, Text6);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_CONTINUEBUFFERSETTEXTNAME(Name1, Name2, Name3, Name4, Name5, Name6);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_CONTINUEBUFFERFLYSTART(nCount);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_CONTINUEBUFFERSETADDMODE(bFullModel);
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
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
                LmcErrCode Ret = LMC1_CONTINUEBUFFERPARTFINISH();
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                    return true;
                else
                {
                    mLastError = Ret;
                    return false;
                }
            }

        }
        //关闭停止
        public static bool Close()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
            LmcErrCode Ret = LMC1_CLOSE();
            if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
            {
                mIsInitLaser = false;
                return true;
            }
            else
            {
                mLastError = Ret;
                return false;
            }

        }
        public static void StopMark()
        {
            if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return;
            }
            LmcErrCode Ret = LMC1_STOPMARK();
            if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                return;
            else
            {
                mLastError = Ret;
                return;
            }
        }
        //扩展轴
        public static int GetAxisCoorPulse(int nAxis)
        {
	        if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return -1;
            }
	        return LMC1_GETAXISCOORPULSE(nAxis);
        }
        public static  bool AxisMoveTOPulse(int nAxis,int nPulse)
       {
	        if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
	        LmcErrCode Ret=LMC1_AXISMOVETOPULSE(nAxis,nPulse);
            if (Ret!= LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mLastError=Ret;
                return false;
            }
	       return true;
       }
        public static bool Reset(bool nAxis1,bool nAxis2)
        {
	        if (!mIsInitLaser)
            {
                mLastError = LmcErrCode.LMC1_ERROR_NOINIT;
                return false;
            }
	        LmcErrCode Ret=LMC1_RESET(nAxis1,nAxis2);
            if (Ret!= LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mLastError=Ret;
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

            LmcErrCode Ret = LMC1_AXISCORRECTORIGIN(Axis);
            if (Ret != LmcErrCode.LMC1_ERR_SUCCESS)
            {
                mLastError = Ret;
                return -1;
            }
	        return 0;
        }
        //其他
        public static bool EditMarkEzdForm(string strFilePath,string strEzdCadName)
        {
            if (threadInitLaser != null)
            {
                //判断当前线程是否结束
                if (threadInitLaser.IsAlive)
                {
                    return false;
                }
            }
            if (!File.Exists(Application.StartupPath + "\\" + strEzdCadName))
            {
                MessageBox.Show(strEzdCadName.ToString()+":文件不存在");
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
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.CreateNoWindow = true;//设不显示窗口
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = Application.StartupPath + @"\" + strEzdCadName.ToString();
            process.StartInfo.Arguments = mMarkEzdFile;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            try
            {
                process.Start();
            }
            catch (System.ComponentModel.Win32Exception we)
            {
                process.Dispose();
                MessageBox.Show(we.Message);
                return;
            }
            process.WaitForExit();
            process.Dispose();
            if (!InitLaser(m_Ptr))
            {
                MessageBox.Show("初始化激光器失败!");
                return;
            }
            return;
        }
        public static void SetDeviceParams()
        {
            if (!mIsInitLaser)
            {
                MessageBox.Show("激光器没有初始化");
                return;
            }
            lock (ObjectLock)
            {
                LmcErrCode Ret = LMC1_SETDEVCFG();
                if (LmcErrCode.LMC1_ERR_SUCCESS == Ret)
                {
                    return;
                }
                else
                {
                    mLastError = Ret;
                    return;
                }

            }
     
        }

        public static bool PostMessage(IntPtr ptr, int msg, uint wParam, uint lParam)
        {
            return POSTMESSAGE(ptr, msg, wParam, lParam);
        }
    }
}
