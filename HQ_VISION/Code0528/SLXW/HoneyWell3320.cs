using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using Studio_Log;
namespace SLXW
{
    public class HoneyWell3320
    {

        private static SerialPort m_port = new SerialPort();

        private static int _ReadTimeOut = 500;
        private static int _WriteTimeOut = 500;


        private static byte[] TriggerCode = new byte[3] { 0x16, 0x54, 0x0D };
        private static byte[] StopTrigger = new byte[3] { 0x16, 0x55, 0x0D };

        private static int _BarcodeLen = 10;
        public static  bool Open(string PortName, int BaudRate, int DataBits, Parity parity, StopBits stopbits,int nBarcodeLen)
        {
            if (m_port.IsOpen)
                m_port.Close();
            try
            {
                m_port.PortName = PortName;
                m_port.BaudRate = BaudRate;
                m_port.DataBits = DataBits;
                m_port.Parity = parity;
                m_port.StopBits = stopbits;
                m_port.ReadTimeout = _ReadTimeOut;
                _BarcodeLen = nBarcodeLen;
                m_port.WriteTimeout = _WriteTimeOut;
           
                m_port.Open();
                m_port.DiscardInBuffer();
                m_port.DiscardOutBuffer();
                return true;
            }
            catch (System.Exception )
            {
                return false;
            }
        }


        public static bool TriggerRead(int nFailtTimes,ref string strHexData,ref string strData, ref string strErrorInfo)
        {
            if (!m_port.IsOpen)
            {
                strErrorInfo = "请先打开串口";
                return false;
            }
            try
            {
                 m_port.DiscardOutBuffer();
                 m_port.DiscardInBuffer();
                 m_port.Write(TriggerCode, 0, 3);
                int nRecvLen = 0;
                char[] bReadData = new char[_BarcodeLen];
                int nCurTimeOut = 0;
                while(nRecvLen<_BarcodeLen)
                {
                   try
                   {
                         nRecvLen+=m_port.Read(bReadData,nRecvLen,1);
                   }
                   catch(TimeoutException )
                   {
                       m_port.DiscardOutBuffer();
                       m_port.DiscardInBuffer();
                       m_port.Write(StopTrigger, 0, 3);
                       nRecvLen=0;
                       nCurTimeOut++;
                       if (nCurTimeOut>=nFailtTimes)
                       {
                           strErrorInfo="读取条码超时";
                           return false;
                       }
                       continue;
                    }
                }
                for (int k = 0; k < _BarcodeLen; k++)
                {
                     strHexData += (((byte)bReadData[k]).ToString("X2") + ";");
                 }
                strData = new string(bReadData);

                return true;
            }
            catch (Exception ex)
            {
                strErrorInfo = "异常:" + ex.Message;
                return false;
            }
           
        }
         public static bool TriggerReadEnd(int nFailtTimes, ref string strHexData, ref string strData, ref string strErrorInfo)
        {
            if (!m_port.IsOpen)
            {
                strErrorInfo = "请先打开串口";
                return false;
            }
            try
            {
                m_port.DiscardOutBuffer();
                m_port.DiscardInBuffer();
                m_port.Write(TriggerCode, 0, 3);
                Thread.Sleep(500);
                int nRecvLen = 0;
                char[] bReadData = new char[4096];
                int nCurTimeOut = 0;
                while (nRecvLen < _BarcodeLen)
                {
                    try
                    {
                        if (m_port.BytesToRead<=0)
                        {
                            continue; 
                        }
                        nRecvLen += m_port.Read(bReadData, nRecvLen, _BarcodeLen);
                    }
                    catch (TimeoutException)
                    {
                        nCurTimeOut++;
                        if (nCurTimeOut >= nFailtTimes)
                        {
                            nRecvLen = 0;
                            m_port.DiscardOutBuffer();
                            m_port.DiscardInBuffer();
                            m_port.Write(StopTrigger, 0, 3);

                            strErrorInfo = "读取条码超时";
                            return false;
                        }
                        continue;
                    }
                }
                char[] bData = new char[_BarcodeLen];
                Array.Copy(bReadData, bData, _BarcodeLen);
                for (int k = 0; k < _BarcodeLen; k++)
                {
                    strHexData += (((byte)bData[k]).ToString("X2") + ";");
                }
                strData = new string(bData);

                return true;
            }
            catch (Exception ex)
            {
                strErrorInfo = "异常:" + ex.Message;
                return false;
            }

        }
    
    }
}
