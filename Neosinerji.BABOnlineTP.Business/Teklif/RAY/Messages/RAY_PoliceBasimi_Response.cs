using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("POLICY_PRINTOUT", Namespace = "", IsNullable = false)]
    public class RAY_PoliceBasimi_Response
    {
        [XmlElement]
        public string STATUS_CODE { get; set; }

        [XmlElement]
        public string STATUS_DESC { get; set; }

        [XmlElement]
        public string LINK { get; set; }

        [XmlElement]
        public string FILE_TYPE { get; set; }
    }
}
