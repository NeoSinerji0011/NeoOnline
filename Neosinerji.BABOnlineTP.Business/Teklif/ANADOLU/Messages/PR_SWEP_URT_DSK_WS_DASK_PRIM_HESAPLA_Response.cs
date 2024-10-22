using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_DSK_WS_DASK_PRIM_HESAPLA_Response");

            this.TAKSIT_SAYISI = XmlValueForKey("TAKSIT_SAYISI");
            this.ODEME_TIPI = XmlValueForKey("ODEME_TIPI");
            this.TOPLAM_SIGORTA_BEDELI = XmlValueForKey("TOPLAM_SIGORTA_BEDELI");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.NET_PRIM = XmlValueForKey("NET_PRIM");
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.SESSION_ID = XmlValueForKey("SESSION_ID");
        }

        public string TAKSIT_SAYISI { get; set; }
        public string ODEME_TIPI { get; set; }
        public string TOPLAM_SIGORTA_BEDELI { get; set; }
        public string HATA_KODU { get; set; }
        public string NET_PRIM { get; set; }
        public string HATA_TEXT { get; set; }
        public string SESSION_ID { get; set; }
    }
}

