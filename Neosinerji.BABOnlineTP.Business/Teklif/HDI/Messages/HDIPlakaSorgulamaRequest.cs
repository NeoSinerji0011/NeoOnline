using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    [XmlRoot(ElementName="HDISIGORTA", Namespace="")]
    public class HDIPlakaSorgulamaRequest : HDIMessage
    {
        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string Refno { get; set; }
        public string PlakaIlKd { get; set; }
        public string PlakaNo { get; set; }
        public string KimlikTipi { get; set; }
        public string KimlikNo { get; set; }       
    }
}
