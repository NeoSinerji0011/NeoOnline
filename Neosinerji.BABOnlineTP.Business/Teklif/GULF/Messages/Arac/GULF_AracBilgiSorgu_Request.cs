using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Arac
{
    [XmlRootAttribute("REQUEST")]
    public class GULF_AracBilgiSorgu_Request
    {
        [XmlElement("AUTH")]
        public AUTH AUTH { get; set; }
        [XmlElement("POLICY")]
        public MASTER MASTER { get; set; }
    }

    public class MASTER
    {
        public string KIMLIKNO { get; set; }
        public string KIMLIKTIPI { get; set; }
        public string PLAKAILKODU { get; set; }
        public string PLAKANO { get; set; }
    }
}
