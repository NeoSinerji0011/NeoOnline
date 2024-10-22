using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.PDF
{
    [XmlRootAttribute("REQUEST")]
    public class GULF_PDF_Request
    {
        [XmlElement("AUTH")]
        public AUTH AUTH { get; set; }
        [XmlElement("POLICY_PRINTOUT")]
        public POLICY_PRINTOUT POLICY_PRINTOUT { get; set; }
    }
    public class POLICY_PRINTOUT
    {
        public string FIRM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string PRODUCT_NO { get; set; }
        public string POLICY_NO { get; set; }
        public string RENEWAL_NO { get; set; }
        public string ENDORS_NO { get; set; }
        public string PRINT_TYPE { get; set; }
        public string OUTPUT_FORMAT { get; set; }
        public string CLIENT_IP { get; set; }
    }
}
