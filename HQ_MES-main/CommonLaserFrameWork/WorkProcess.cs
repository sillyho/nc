using CommonLibrarySharp;
using MyMarkEzd;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CommonLaserFrameWork
{
    public class WorkProcess
    {
        // 金橙子
        private static MyJCZ _MarkJcz = new MyJCZ();
        private static PictureBox _pictureBox1;
        private static int _endIO = 5;
        private static int _valid = 1;
        private static Configure _configure = new Configure();

        private static string _strSN = "";
        public static bool _updateData = true;

        // 打开主窗体前的初始化
        public static bool InitForm(PictureBox pictureBox1)
        {
            _pictureBox1 = pictureBox1;
            try
            {
                if (!_MarkJcz.InitLaserMark())
                {
                    string strMsg = string.Format("初始化激光器失败,错误信息:{0}", _MarkJcz.GetLastError());
                    MessageBox.Show(strMsg, "初始化激光器失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.WriteMessage(strMsg, true);

                    return false;
                }

                _endIO = _configure.ReadConfig("SET", "EndIO", 4);
                _valid = _configure.ReadConfig("SET", "Valid", 1);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("CheckBeforeMark 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }

        public static bool TreggerReadPort(int nPort)
        {
            try
            {
                return _MarkJcz.TreggerReadPort(nPort);
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("TreggerReadPort 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }
        public static bool ReadPort(int nPort)
        {
            try
            {
                return _MarkJcz.ReadPort(nPort);
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("ReadPort 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }
        public static int  ReadPort()
        {
            try
            {
                return _MarkJcz.ReadPort();
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("ReadPort 捕获到异常:{0}", ex.Message.ToString()), true);
                return 0;
            }

        }
        public static void SetOutPort(int nport,int nplusewidth)
        {
            _MarkJcz.SetOutPort(nport, 1, nplusewidth);
        
        }
        // 加载模板
        public static bool LoadEzdFile(string strEzdPath)
        {
            try
            {
                _MarkJcz.LoadEzdFile(strEzdPath);
                _MarkJcz.ShowPreviewBmp(_pictureBox1);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("LoadEzdFile 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }
        public static bool CheckBeforeMark(string strSN)
        {
            if(!_updateData)
            {
                Log.WriteMessage("不上传模式不进行检测");
                return true;
            }
            Log.WriteMessage("打标前检测");
            try
            {
                string strResponse = "";
                bool bRes = GetMesInfo("start", strSN, ref strResponse);
                if (!bRes) return false;
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("CheckBeforeMark 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }
        // 打标前的校验
        public static bool CheckBeforeMark()
        {
            Log.WriteMessage("打标前检测");
            try
            {
                string strResponse = "";
                bool bRes = GetMesInfo("start", _strSN, ref strResponse);
                if (!bRes) return false;
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("CheckBeforeMark 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }

        // 加载模板替换打标
        private static bool ChangeVariableAndMark(Dictionary<string, string> dicControlValue)
        {
           
            try
            {
                Log.WriteMessage("开始获取信息");
                string strResponse = "";
                bool bRes = GetMesInfo("getinfo", _strSN, ref strResponse);
                if (!bRes)
                    return false;
                //strResponse = "{\"DSN\":\"GCC1L8321382005E\",\"PA_WO\":\"DGSSC210913N3013_ZJ#2115BEX\",\"FA_PN\":\"HQ3110AK79000\",\"PSN\":\"AT210917026205\",\"LaserTemplatePath\":\"Jaws-ARTWORK-Black\",\"G_CHECKFLOWID\":\"\",\"G_WO_GUID\":\"\",\"G_WOCONF_GUID\":\"\",\"MB\":\"PCC28A32137200N2\",\"Software_Version\":\"Socring99.0.34mfg+Nordic1.0.0-4+PGE6473+NXP43\",\"AmazonSetName\":\"D2F2A-1\",\"SHA1\":\"74869\",\"MAC\":\"343EA4901D20\",\"IsRepairVer\":\"N\",\"BLEMAC\":\"343EA4901D21\",\"RSN\":\"BHCA12137HN003730\",\"WifiAddr\":\"343EA4901D20\"}";
                // 解析打标内容
                Log.WriteMessage("开始解析打标内容");
                Dictionary<string, string> dicRes = AnalyzeMesInfo(strResponse);
               
                Log.WriteMessage("加载模板并替换打标");
                // 加载模板
                if(!dicRes.ContainsKey("LaserTemplatePath"))
                {
                    Log.WriteMessage("返回的信息中不包含模板信息LaserTemplatePath",true);
                    return false;
                }
                string strEzdPath = String.Format("{0}\\{1}.ezd", dicControlValue["dir"].ToString(), dicRes["LaserTemplatePath"].ToString());
                Log.WriteMessage("模板路径:" + strEzdPath);
                if (!File.Exists(strEzdPath))
                {
                    Log.WriteMessage("文件不存在:" + strEzdPath,true);
                    return false;

                }
                if (!_MarkJcz.LoadEzdFile(strEzdPath))
                {
                    Log.WriteMessage("加载模板失败", true);
                    return false;
                }

                // 替换打标
                string strBrFormat = _configure.ReadConfig("BR", "contextformat", "");
                foreach (KeyValuePair<string, string> kvp in dicRes)
                {
                    string strKey = kvp.Key;
                    string strValue = kvp.Value;

                    if (strBrFormat.IndexOf(strKey) >= 0)
                    {
                        string strChangeKey = string.Format("{{{0}}}", strKey);
                        strBrFormat = strBrFormat.Replace(strChangeKey, strValue);
                        Log.WriteMessage(string.Format("BR中查找到key:{0}, 替换内容为:{1}", strChangeKey, strValue));
                    }
                    if(strKey=="SHA1")
                    {
                        strKey = "Laser2";
                        Log.WriteMessage(string.Format("ezd模板替换对象:{0}, 替换内容为:{1}", "Laser2", strValue));
                        _MarkJcz.ChangeTextByName(strKey, strValue);

                    }
                    else if (strKey == "DSN")
                    {
                        strKey = "Laser3";
                        Log.WriteMessage(string.Format("ezd模板替换对象:{0}, 替换内容为:{1}", "Laser3", strValue));
                        _MarkJcz.ChangeTextByName(strKey, strValue);
                    }
                }
                string strBrContext = strBrFormat;
                Log.WriteMessage(string.Format("最终生成的二维码内容为Laser1:{0}", strBrContext));

                _MarkJcz.ChangeTextByName("Laser1", strBrContext);
                _MarkJcz.ShowPreviewBmp(_pictureBox1);
                Log.WriteMessage("开始打标");
                if (_MarkJcz.Mark())
                {
                    Log.WriteMessage("打标成功");
                }
                else
                {
                    Log.WriteMessage("打标失败");
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("ChangeVariableAndMark 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }

        // 打标流程
        public static bool MarkProcessImpl(Dictionary<string, string> dicControlValue)
        {
            try
            {
                _strSN = dicControlValue["SN"].ToString();
               
                if (ChangeVariableAndMark(dicControlValue))
                {
                    if (WorkProcess._updateData)
                    {
                        if (!UploadAfterMark())
                        {
                            Log.WriteMessage("上传失败", true);
                            return false;
                        }
                    }
                    else
                    {
                        Log.WriteMessage("不上传数据信息");
                    }

                    _MarkJcz.SetOutPort(_endIO, _valid, 500);
                }
                else
                {
                    Log.WriteMessage("打标流程执行失败", true);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("MarkProcessImpl 捕获到异常:{0}", ex.Message.ToString()), true);
            }
            finally
            {

            }

            return false;
        }

        // 打标后的保存上传等操作
        private static bool UploadAfterMark()
        {
            Log.WriteMessage("打标后上传保存");
            try
            {
                string strResponse = "";
                bool bRes = GetMesInfo("complete", _strSN, ref strResponse);
                if (!bRes) return false;
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("CheckBeforeMark 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }

        // 关闭主窗体后执行
        public static bool CloseForm()
        {
            try
            {
                _MarkJcz.StopMark();
                _MarkJcz.CloseEZD();

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("CloseForm 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }

        public static void StopMark()
        {
            _MarkJcz.StopMark();
        }

        private static bool GetMesInfo(string strMethod, string strSN, ref string strResponse)
        {
            strResponse = "";

            try
            {
                string strRequest = "";

                string strUrl = _configure.ReadConfig("MES", "url", "");
                string strStation = _configure.ReadConfig("MES", "station", "");
                string strUid = _configure.ReadConfig("MES", "uid", "");
                string strPwd = _configure.ReadConfig("MES", "pwd", "");
                string strNoPwd = _configure.ReadConfig("MES", "nopwd", "");
                string strToolName = _configure.ReadConfig("MES", "toolname", "");

                if (strMethod == "start")
                {
                    strRequest = string.Format("{0}?type={1}&action={2}&station={3}&uid={4}&pwd={5}&nopwd={6}&sn={7}",
                                    strUrl, "20", strMethod, strStation, strUid, strPwd, strNoPwd, strSN);
                }
                else if (strMethod == "getinfo")
                {
                    strRequest = string.Format("{0}?type={1}&action={2}&uid={3}&pwd={4}&json=1&IsReadOnly=Y&sn={5}",
                                    strUrl, "21", strMethod, strUid, strPwd, strSN);
                }
                else if (strMethod == "complete")
                {
                    strRequest = string.Format("{0}?type={1}&action={2}&station={3}&uid={4}&pwd={5}&nopwd={6}&ToolName={7}&sn={8}&code=0",
                                    strUrl, "35", strMethod, strStation, strUid, strPwd, strNoPwd, strToolName, strSN);
                }
                else
                {
                    Log.WriteMessage(string.Format("不支持的接口请求:{0}", strMethod), true);
                    return false;
                }

                Log.WriteMessage(string.Format("接口请求参数为:{0}", strRequest));
                // url 请求类型
                string strRequestType = _configure.ReadConfig("MES", "requesttype", "");
                string strMsg = httpHelper.CommonUrl(strRequest, strRequestType.ToUpper());
                Log.WriteMessage(string.Format("接口返回数据为:{0}", strMsg));

                if (strMethod == "getinfo")
                {
                    JObject objJson = JObject.Parse(strMsg);
                    JObject objData = (JObject)(objJson.SelectToken("data") as JArray)[0];
                    if (objJson["error"].ToString() == "0" || objJson["error"].ToString() == "1")
                    {
                        strResponse = JsonConvert.SerializeObject(objData);
                        Log.WriteMessage("getinfo返回成功");
                    }
                    else
                    {
                        Log.WriteMessage(string.Format("getinfo返回失败,错误信息为:{0}", objJson["msg"].ToString()), true);
                        return false;
                    }
                }
                else
                {
                    string[] strResArray = strMsg.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    strResponse = strMsg.Substring(strMsg.LastIndexOf('|') + 1);
                    if (strResArray[0] != "1")
                    {
                        Log.WriteMessage(string.Format("接口返回失败，错误信息为:{0}", strResponse), true);
                        return false;
                    }
                }

                Log.WriteMessage(string.Format("截取数据信息为:{0}", strResponse));
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("GetMesInfo 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return false;
        }

        private static string GetEncryptedSN()
        {
            string strResult = "";
            try
            {
                var strRes = Encoding.Default.GetBytes(_strSN);
                HashAlgorithm iSha = new SHA1CryptoServiceProvider();
                strRes = iSha.ComputeHash(strRes);
                var enText = new StringBuilder();
                foreach (byte iByte in strRes)
                {
                    enText.AppendFormat("{0:x2}", iByte);
                }

                string strSHA1 = enText.ToString();
                Log.WriteMessage(string.Format("原数据:{0},SHA1后内容为:{1}", _strSN, strSHA1));
                string strHex = strSHA1.Substring(strSHA1.Length - 8);
                int nNumber = int.Parse(strHex, System.Globalization.NumberStyles.HexNumber);
                string strNumber = nNumber.ToString();
                strResult = strNumber.Substring(strNumber.Length - 5);
                Log.WriteMessage("最终计算明码内容为:" + strResult);
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("GetEncryptedSN 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return strResult;
        }

        private static Dictionary<string, string> AnalyzeMesInfo(string strMesInfo)
        {
            Dictionary<string, string> dicRes = new Dictionary<string, string>();

            try
            {
                foreach (JProperty jToken in JObject.Parse(strMesInfo).Properties())
                {
                    string strKey = jToken.Name.ToString();
                    string strValue = "";
                    if (jToken.Value.GetType().Name == "JObject")
                    {
                        strValue = JsonConvert.SerializeObject(jToken.Value);
                    }
                    else
                    {
                        strValue = jToken.Value.ToString();
                    }

                    dicRes.Add(strKey, strValue);
                    Log.WriteMessage(string.Format("获取MES接口数据key:{0},value为:{1}", strKey, strValue));
                }
            }
            catch (Exception ex)
            {
                Log.WriteMessage(string.Format("AnalyzeMesInfo 捕获到异常:{0}", ex.Message.ToString()), true);
            }

            return dicRes;
        }
    }
}
