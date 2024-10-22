using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET_Response");

            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.THG_FON = XmlValueForKey("THG_FON");
            this.KTGS = XmlValueForKey("KTGS");
            this.BRUT_PRIM = XmlValueForKey("BRUT_PRIM");
            this.BSMV = XmlValueForKey("BSMV");
            this.GENEL_SART_PDF = XmlValueForKey("GENEL_SART_PDF");
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.ODEME_PLANI = XmlValueForKey("ODEME_PLANI");
            this.BILESEN_PRIM_DETAY = XmlValueForKey("BILESEN_PRIM_DETAY");
            this.BILGILENDIRME_FORMU_PDF = XmlValueForKey("BILGILENDIRME_FORMU_PDF");
            this.GECIKME_SURPRIM_ORANI = XmlValueForKey("GECIKME_SURPRIM_ORANI");
            this.HASARSIZLIK_ORANI = XmlValueForKey("HASARSIZLIK_ORANI");
            this.KOMISYON_TUTARI = XmlValueForKey("KOMISYON_TUTARI");
            this.NET_PRIM = XmlValueForKey("NET_PRIM");
            this.TEKLIF_NO = XmlValueForKey("TEKLIF_NO");
            this.TEKLIF_PDF = XmlValueForKey("TEKLIF_PDF");
        }

        public string SESSION_ID { get; set; }
        public string THG_FON { get; set; }
        public string KTGS { get; set; }
        public string BRUT_PRIM { get; set; }
        public string BSMV { get; set; }
        public string GENEL_SART_PDF { get; set; }
        public string HATA_KODU { get; set; }
        public string HATA_TEXT { get; set; }
        public string ODEME_PLANI { get; set; }
        public string BILESEN_PRIM_DETAY { get; set; }
        public string BILGILENDIRME_FORMU_PDF { get; set; }
        public string GECIKME_SURPRIM_ORANI { get; set; }
        public string HASARSIZLIK_ORANI { get; set; }
        public string KOMISYON_TUTARI { get; set; }
        public string NET_PRIM { get; set; }
        public string TEKLIF_NO { get; set; }
        public string TEKLIF_PDF { get; set; }

    }
}
