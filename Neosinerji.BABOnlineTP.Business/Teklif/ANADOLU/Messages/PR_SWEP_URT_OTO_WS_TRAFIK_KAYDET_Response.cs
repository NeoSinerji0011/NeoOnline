using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_TRAFIK_KAYDET_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1_Response");

            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.THG_FON = XmlValueForKey("THG_FON");
            this.KTGS = XmlValueForKey("KTGS");
            this.POLICE_NO = XmlValueForKey("POLICE_NO");
            this.BRUT_PRIM = XmlValueForKey("BRUT_PRIM");
            this.BSMV = XmlValueForKey("BSMV");
            this.POLICE_PDF = XmlValueForKey("POLICE_PDF");
            this.GENEL_SART_PDF = XmlValueForKey("GENEL_SART_PDF");
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.ODEME_PLANI = XmlValueForKey("ODEME_PLANI");
        }

        public string SESSION_ID { get; set; }
        public string THG_FON { get; set; }
        public string KTGS { get; set; }
        public string POLICE_NO { get; set; }
        public string BRUT_PRIM { get; set; }
        public string BSMV { get; set; }
        public string POLICE_PDF { get; set; }
        public string GENEL_SART_PDF { get; set; }
        public string HATA_KODU { get; set; }
        public string HATA_TEXT { get; set; }
        public string ODEME_PLANI { get; set; }
    }
}
