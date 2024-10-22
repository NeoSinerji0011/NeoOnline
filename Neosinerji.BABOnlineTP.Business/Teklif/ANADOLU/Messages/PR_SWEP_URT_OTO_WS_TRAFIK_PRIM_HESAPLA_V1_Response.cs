using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1_Response");

            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.KTGS = XmlValueForKey("KTGS");
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.HASARSIZLIK_ORANI = XmlValueForKey("HASARSIZLIK_ORANI");
            this.GECIKME_SURPRIM_ORANI = XmlValueForKey("GECIKME_SURPRIM_ORANI");
            this.BSMV = XmlValueForKey("BSMV");
            this.THG_FON = XmlValueForKey("THG_FON");
            this.TAKSIT_SAYISI = XmlValueForKey("TAKSIT_SAYISI");
            this.SONUC = XmlValueForKey("SONUC");
            this.ODEME_TIPI = XmlValueForKey("ODEME_TIPI");
        }

        public string SESSION_ID { get; set; }
        public string KTGS { get; set; }
        public string HATA_TEXT { get; set; }
        public string HATA_KODU { get; set; }
        public string HASARSIZLIK_ORANI { get; set; }
        public string GECIKME_SURPRIM_ORANI { get; set; }
        public string BSMV { get; set; }
        public string THG_FON { get; set; }
        public string TAKSIT_SAYISI { get; set; }
        public string SONUC { get; set; }
        public string ODEME_TIPI { get; set; }
    }
}
