using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET_Response : ANADOLUMessage
    {
        public override void ReadXml(string xml, string name)
        {
            base.ReadXml(xml, "PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET_Response");

            this.TEKLIF_NO = XmlValueForKey("TEKLIF_NO");
            this.MUSTERI_BAGLANTI_INDIRIMI = XmlValueForKey("MUSTERI_BAGLANTI_INDIRIMI");
            this.HASARSIZLIK_ORANI = XmlValueForKey("HASARSIZLIK_ORANI");
            this.MUSTERI_BAGLANTI_KURUM_ADI = XmlValueForKey("MUSTERI_BAGLANTI_KURUM_ADI");
            this.BILESEN_PRIM_DETAY = XmlValueForKey("BILESEN_PRIM_DETAY");
            this.BILGI_KODU = XmlValueForKey("MUSTERI_BAGLANTI_KURUM_ADI");           
            this.HATA_TEXT = XmlValueForKey("HATA_TEXT");
            this.HATA_KODU = XmlValueForKey("HATA_KODU");
            this.BILGI_TEXT = XmlValueForKey("BILGI_TEXT");
            this.TAKSITLI_BRUT_PRIM = XmlValueForKey("TAKSITLI_BRUT_PRIM");
            this.TAKSITLI_BSMV = XmlValueForKey("TAKSITLI_BSMV");
            this.TEKLIF_PDF = XmlValueForKey("TEKLIF_PDF");
            this.BILGILENDIRME_FORMU_PDF = XmlValueForKey("BILGILENDIRME_FORMU_PDF");
            this.GENEL_SART_PDF = XmlValueForKey("GENEL_SART_PDF");
            this.PESIN_BRUT_PRIM = XmlValueForKey("PESIN_BRUT_PRIM");
            this.PESIN_NET_PRIM = XmlValueForKey("PESIN_NET_PRIM");
            this.PESIN_BSMV = XmlValueForKey("PESIN_BSMV");
            this.TAKSITLI_NET_PRIM = XmlValueForKey("TAKSITLI_NET_PRIM");
            this.TAKSIT_SAYISI = XmlValueForKey("TAKSIT_SAYISI");
            this.ODEME_TIPI = XmlValueForKey("ODEME_TIPI");
            this.SESSION_ID = XmlValueForKey("SESSION_ID");
            this.KOMISYON_TUTARI = XmlValueForKey("KOMISYON_TUTARI");
           
            
        }

        public string TEKLIF_NO { get; set; }
        public string MUSTERI_BAGLANTI_INDIRIMI { get; set; }
        public string HASARSIZLIK_ORANI { get; set; }
        public string MUSTERI_BAGLANTI_KURUM_ADI { get; set; }
        public string BILESEN_PRIM_DETAY { get; set; }
        public string BILGI_KODU { get; set; }
        public string HATA_TEXT { get; set; }
        public string HATA_KODU { get; set; }
        public string BILGI_TEXT { get; set; }
        public string TAKSITLI_BRUT_PRIM { get; set; }
        public string TAKSITLI_BSMV { get; set; }
        public string TEKLIF_PDF { get; set; }
        public string BILGILENDIRME_FORMU_PDF { get; set; }
        public string GENEL_SART_PDF { get; set; }
        public string PESIN_BRUT_PRIM { get; set; }
        public string PESIN_NET_PRIM { get; set; }
        public string PESIN_BSMV { get; set; }
        public string TAKSITLI_NET_PRIM { get; set; }
        public string TAKSIT_SAYISI { get; set; }
        public string ODEME_TIPI { get; set; }
        public string SESSION_ID { get; set; }
        public string KOMISYON_TUTARI { get; set; }
        
        
    }
}
