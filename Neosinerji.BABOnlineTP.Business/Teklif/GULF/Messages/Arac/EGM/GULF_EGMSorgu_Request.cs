using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko;
using Neosinerji.BABOnlineTP.Business.GULF.Messages.Kasko.Teklif;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.GULF.Messages.Arac.EGM
{
    [XmlRootAttribute("REQUEST")]
    public class GULF_EGMSorgu_Request
    {
        [XmlElement("AUTH")]
        public AUTH AUTH { get; set; }

        [XmlElement("MASTER")]
        public MASTER MASTER { get; set; }
    }
    public class MASTER
    {
        public string CACHESTYLE { get; set; }
        public string MODELYILI { get; set; }
        public string MOTORNO { get; set; }
        public string SASINO { get; set; }
    }
}
