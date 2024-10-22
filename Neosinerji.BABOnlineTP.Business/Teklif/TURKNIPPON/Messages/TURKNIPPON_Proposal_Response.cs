using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON
{
    [XmlRootAttribute("EXTRA", Namespace = "", IsNullable = false)]
    public class TURKNIPPON_Proposal_Response
    {
        public POLDID POLDID { get; set; }
    }
    public class POLDID
    {
        [XmlElement]
        public string DEDUCTION_TYPE_CODE { get; set; }

        [XmlElement]
        public string DEDUCTION_CODE { get; set; }

        [XmlElement]
        public string DEDUCTION_AMOUNT { get; set; }

        [XmlElement]
        public string LC_DEDUCTION_AMOUNT { get; set; }

        [XmlElement]
        public string DEDUCTION_DESCRIPTION { get; set; }
    }

}
