using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF
{
    [XmlRootAttribute("RESPONSE", Namespace = "", IsNullable = false)]
    public class GULF_PDF_Response
    {
        public string OPERATION_ID { get; set; }
        public string RESULT { get; set; }
        public string ERROR { get; set; }
        public DATA DATA { get; set; }
    }
    public class DATA
    {
        public POLICY_PRINTOUT POLICY_PRINTOUT { get; set; }
    }
    public class POLICY_PRINTOUT
    {
        public string FILE_FORMAT { get; set; }
        public string FILE { get; set; }
    }
}
