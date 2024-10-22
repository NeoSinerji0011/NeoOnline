using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{

    [XmlRootAttribute("POLICY_PRINTOUT")]
    public class RAY_PoliceBasimi_Request
    {
        public string USER_NAME;
        public string PASSWORD;
        public string PROCESS_ID;
        public string FIRM_CODE;
        public string COMPANY_CODE;
        public string PRODUCT_NO;
        public string POLICY_NO;
        public string RENEWAL_NO;
        public string ENDORS_NO;
        public string PRINT_TYPE;
    }
}
