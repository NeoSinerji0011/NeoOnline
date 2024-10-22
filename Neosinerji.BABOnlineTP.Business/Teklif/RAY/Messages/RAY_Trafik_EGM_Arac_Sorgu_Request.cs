using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace  Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("REQUEST")]
    public class RAY_Trafik_EGM_Arac_Sorgu_Request
    {
        public string PROCESS_ID;
        public string USER_NAME;
        public string PASSWORD;
        public string CHANNEL;
        public string PRODUCT_TYPE;
        public string PLATE1;
        public string PLATE2;
        public string CITIZENSHIP_NUMBER;
        public string TAX_NUMBER;
        public string PASSPORT_NUMBER;
        public string REGISTRY_CODE;
        public string REGISTRY_NO;
        public string CONNECTION;
    }
}
