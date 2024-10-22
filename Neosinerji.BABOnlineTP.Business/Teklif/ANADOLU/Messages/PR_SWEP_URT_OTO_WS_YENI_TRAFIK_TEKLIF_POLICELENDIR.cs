using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("WEB_KULLANICI_ADI", this.WEB_KULLANICI_ADI));
            sb.Append(MsgParameter("WEB_SIFRE", this.WEB_SIFRE));
            sb.Append(MsgParameter("CVV2", this.CVV2));
            sb.Append(MsgParameter("KART_TEL_NO", this.KART_TEL_NO));
            sb.Append(MsgParameter("KART_TEL_ALAN", this.KART_TEL_ALAN));
            sb.Append(MsgParameter("TAKSIT_SAYISI", this.TAKSIT_SAYISI));
            sb.Append(MsgParameter("KREDI_KART_NO", this.KREDI_KART_NO));
            sb.Append(MsgParameter("TEKLIF_NO", this.TEKLIF_NO));
            sb.Append(MsgParameter("REVIZYON_NO", this.REVIZYON_NO));
            sb.Append(MsgParameter("SECILEN_ODEME_TIPI", this.SECILEN_ODEME_TIPI));
            sb.Append(MsgParameter("SON_KULLANMA_TARIHI", this.SON_KULLANMA_TARIHI));
            sb.Append(MsgParameter("RETURN_POLICE_PDF", this.RETURN_POLICE_PDF));
            sb.Append(MsgParameter("RETURN_GENEL_SART_PDF", this.RETURN_GENEL_SART_PDF));
            sb.Append(MsgParameter("RETURN_BILGILENDIRME_FORMU_PDF", this.RETURN_BILGILENDIRME_FORMU_PDF));
            sb.Append(MsgParameter("SESSION_ID", this.SESSION_ID));
            sb.Append(MsgParameter("KART_SAHIBI_ADI", this.KART_SAHIBI_ADI));
            sb.Append(MsgParameter("KART_SAHIBI_SOYADI", this.KART_SAHIBI_SOYADI));
            sb.Append(MsgParameter("OTORIZASYON_KODU", this.OTORIZASYON_KODU));
            sb.Append(MsgParameter("REFERANS_ID", this.REFERANS_ID));
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_POLICELENDIR>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string WEB_KULLANICI_ADI { get; set; }
        public string WEB_SIFRE { get; set; }
        public string CVV2 { get; set; }
        public string KART_TEL_NO { get; set; }
        public string KART_TEL_ALAN { get; set; }
        public string TAKSIT_SAYISI { get; set; }
        public string KREDI_KART_NO { get; set; }
        public string TEKLIF_NO { get; set; }
        public string REVIZYON_NO { get; set; }
        public string SECILEN_ODEME_TIPI { get; set; }
        public string SON_KULLANMA_TARIHI { get; set; }
        public string RETURN_POLICE_PDF { get; set; }
        public string RETURN_GENEL_SART_PDF { get; set; }
        public string RETURN_BILGILENDIRME_FORMU_PDF { get; set; }
        public string SESSION_ID { get; set; }
        public string KART_SAHIBI_ADI { get; set; }
        public string KART_SAHIBI_SOYADI { get; set; }
        public string OTORIZASYON_KODU { get; set; }
        public string REFERANS_ID { get; set; }
    }
}
