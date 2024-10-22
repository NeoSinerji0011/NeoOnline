using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR_Response");

            this.POLICE_NO = XmlValueForKey("POLICE_NO");
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.BSMV = XmlValueForKey("BSMV");
            this.POLICE_PDF = XmlValueForKey("POLICE_PDF");
            this.BILGILENDIRME_FORMU_PDF = XmlValueForKey("BILGILENDIRME_FORMU_PDF");
            this.GENEL_SART_PDF = XmlValueForKey("GENEL_SART_PDF");
            this.KOMISYON_TUTARI = XmlValueForKey("KOMISYON_TUTARI");
            this.NET_PRIM = XmlValueForKey("NET_PRIM");
            this.BILESEN_PRIM_DETAY = XmlValueForKey("BILESEN_PRIM_DETAY");
            this.BRUT_PRIM = XmlValueForKey("BRUT_PRIM");
            this.ODEME_PLANI = XmlValueForKey("ODEME_PLANI");
            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.KTGS = XmlValueForKey("KTGS");
            this.THG_FON = XmlValueForKey("THG_FON");
            this.YENILEME_NO = XmlValueForKey("YENILEME_NO");
        }

        public string POLICE_NO { get; set; }
        public string HATA_TEXT { get; set; }
        public string HATA_KODU { get; set; }
        public string BSMV { get; set; }
        public string POLICE_PDF { get; set; }
        public string BILGILENDIRME_FORMU_PDF { get; set; }
        public string GENEL_SART_PDF { get; set; }
        public string KOMISYON_TUTARI { get; set; }
        public string NET_PRIM { get; set; }
        public string BILESEN_PRIM_DETAY { get; set; }
        public string BRUT_PRIM { get; set; }
        public string ODEME_PLANI { get; set; }
        public string SESSION_ID { get; set; }
        public string KTGS { get; set; }
        public string THG_FON { get; set; }
        public string YENILEME_NO { get; set; }
    }
}
