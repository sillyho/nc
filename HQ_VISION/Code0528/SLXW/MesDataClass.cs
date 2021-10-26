using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SLXW
{
    public class G_SPEC
    {
    }

    public class DATA
    {
        /// <summary>
        /// 
        /// </summary>
        public string BTMAC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Battery_CoverPN { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CSN { get; set; }
        /// <summary>
        /// 
        /// </summary>
    
        /// <summary>
        /// 
        /// </summary>
        public string CUSTMODEL { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string G_CHECKFLOWID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public G_SPEC G_SPEC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string G_WOCONF_GUID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string G_WOID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string G_WO_GUID { get; set; }
        public string IMEI1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IMEI1_SPLIT { get; set; }
        public string IMEI2{ get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IMEI2_SPLIT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LaserTemplate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LaserTemplate2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string LaserTemplate_Path { get; set; }
        /// <summary>
        /// 
        public string MODEL_SN { get; set; }
        /// </summary>
        public string SettingName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WIFIMAC { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string hardware { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ram { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rom { get; set; }
    }

    public class G_PHONE_DO
    {
    }

    public class G_RET_DATA
    {
    }

    public class MesDataRecv
    {
        /// <summary>
        /// 
        /// </summary>
        public DATA DATA { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> G_FILE_PATH { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public G_PHONE_DO G_PHONE_DO { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public G_RET_DATA G_RET_DATA { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string H_MSG { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NeedLoad { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string needDownLoad { get; set; }


        public static MesDataRecv JsonToClass(string strJsonString,ref string errorinfo)
        {
            try
            {
                return JsonConvert.DeserializeObject<MesDataRecv>(strJsonString);
            }
            catch (Exception ex)
            {
                errorinfo = ex.Message;
                return null;
            }
    
            
        }
    }


}
