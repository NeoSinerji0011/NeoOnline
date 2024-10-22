using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.ONCEKI_ACENTE_KODU = String.Empty;
            this.ONCEKI_POLICE_NO = String.Empty;
            this.ONCEKI_SIRKET_KODU = String.Empty;
            this.ONCEKI_YENILEME_NO = String.Empty;
            this.SES_CIHAZ_BEDEL = String.Empty;
            this.DONANIM_LISTESI = String.Empty;
            this.GORUNTU_CIHAZ_BEDEL = String.Empty;
            this.ILETISIM_CIHAZ_BEDEL = String.Empty;
            this.IMM_SECILI_KADEME = String.Empty;
            this.IMM_TEMINAT_YAPISI = String.Empty;
            this.SOFOR_SAYISI = String.Empty;
            this.MUAVIN_SAYISI = String.Empty;
            this.YOLCU_SAYISI = String.Empty;
            this.KFK_TEMINAT_LISTESI = String.Empty;
            this.HASARSIZLIK_KORUMA_SZLM_SECIM = String.Empty;
            this.KASKO_MUAFIYET_KADEME = String.Empty;
            this.DEPREM_MUAFIYET_SECIM = String.Empty;
            this.KULLANIM_GELIR_KAYBI_SECIM = String.Empty;
            this.SIGORTALI_SIFATI = String.Empty;
            this.MENFAAT_SAHIBI_LISTESI = String.Empty;
            this.KISISEL_ESYA = String.Empty;
            this.RETURN_TEKLIF_PDF = String.Empty;
            this.RETURN_BILGILENDIRME_FORMU_PDF = String.Empty;
            this.RETURN_GENEL_SART_PDF = String.Empty;
            this.SEND_EPOSTA = String.Empty;
            this.ARAC_TESCIL_KODU = String.Empty;
            this.ARAC_TESCIL_SERI_NO = String.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("KISISEL_ESYA", this.KISISEL_ESYA));
            sb.Append(MsgParameter("ONCEKI_POLICE_NO", this.ONCEKI_POLICE_NO));
            sb.Append(MsgParameter("ONCEKI_YENILEME_NO", this.ONCEKI_YENILEME_NO));
            sb.Append(MsgParameter("BASLANGIC_TARIHI", this.BASLANGIC_TARIHI));
            sb.Append(MsgParameter("BITIS_TARIHI", this.BITIS_TARIHI));
            sb.Append(MsgParameter("GRUP_URUN_ADI", this.GRUP_URUN_ADI));
            sb.Append(MsgParameter("URUN_DOVIZ_CINSI", this.URUN_DOVIZ_CINSI));
            sb.Append(MsgParameter("PLAKA_TURU", this.PLAKA_TURU));
            sb.Append(MsgParameter("PLAKA_NO", this.PLAKA_NO));
            sb.Append(MsgParameter("PLAKA_IL_KODU", this.PLAKA_IL_KODU));
            sb.Append(MsgParameter("MODEL_YILI", this.MODEL_YILI));
            sb.Append(MsgParameter("KULLANIM_TIPI_KODU", this.KULLANIM_TIPI_KODU));
            sb.Append(MsgParameter("KULLANIM_SEKLI_KODU", this.KULLANIM_SEKLI_KODU));
            sb.Append(MsgParameter("KULLANIM_AMACI", this.KULLANIM_AMACI));
            sb.Append(MsgParameter("KASKO_MUAFIYET_KADEME", this.KASKO_MUAFIYET_KADEME));
            sb.Append(MsgParameter("KULLANIM_GELIR_KAYBI_SECIM", this.KULLANIM_GELIR_KAYBI_SECIM));
            sb.Append(MsgParameter("YENI_ARAC_BEDELI", this.YENI_ARAC_BEDELI));
            sb.Append(MsgParameter("IMM_TEMINAT_YAPISI", this.IMM_TEMINAT_YAPISI));
            sb.Append(MsgParameter("IMM_SECILI_KADEME", this.IMM_SECILI_KADEME));
            sb.Append(MsgParameter("WEB_KULLANICI_ADI", this.WEB_KULLANICI_ADI));
            sb.Append(MsgParameter("WEB_SIFRE", this.WEB_SIFRE));
            sb.Append(MsgParameter("SOFOR_SAYISI", this.SOFOR_SAYISI));
            sb.Append(MsgParameter("MUAVIN_SAYISI", this.MUAVIN_SAYISI));
            sb.Append(MsgParameter("YOLCU_SAYISI", this.YOLCU_SAYISI));
            sb.Append(MsgParameter("IKAME_ARAC_BILGILERI", this.IKAME_ARAC_BILGILERI));
            sb.Append(MsgParameter("DONANIM_LISTESI", this.DONANIM_LISTESI));
            sb.Append(MsgParameter("GORUNTU_CIHAZ_BEDEL", this.GORUNTU_CIHAZ_BEDEL));
            sb.Append(MsgParameter("ILETISIM_CIHAZ_BEDEL", this.ILETISIM_CIHAZ_BEDEL));
            sb.Append(MsgParameter("SES_CIHAZ_BEDEL", this.SES_CIHAZ_BEDEL));
            sb.Append(MsgParameter("KFK_TEMINAT_LISTESI", this.KFK_TEMINAT_LISTESI));
            sb.Append(MsgParameter("TESCIL_TARIHI", this.TESCIL_TARIHI));
            sb.Append(MsgParameter("ARAC_TESCIL_KODU", this.ARAC_TESCIL_KODU));
            sb.Append(MsgParameter("ARAC_TESCIL_SERI_NO", this.ARAC_TESCIL_SERI_NO));
            sb.Append(MsgParameter("MUSTERI_NO", this.MUSTERI_NO));
            sb.Append(MsgParameter("E_POSTA", this.E_POSTA));
            sb.Append(MsgParameter("ARAC_KODU", this.ARAC_KODU));
            sb.Append(MsgParameter("HASARSIZLIK_KORUMA_SZLM_SECIM", this.HASARSIZLIK_KORUMA_SZLM_SECIM));
            sb.Append(MsgParameter("DEPREM_MUAFIYET_SECIM", this.DEPREM_MUAFIYET_SECIM));
            sb.Append(MsgParameter("ARAC_OPERASYONEL_KIRALIK", this.ARAC_OPERASYONEL_KIRALIK));
            sb.Append(MsgParameter("SIGORTAETTIRENAYNIZAMANDASIGORTALIMI", this.SIGORTAETTIRENAYNIZAMANDASIGORTALIMI));
            sb.Append(MsgParameter("SIGORTALI_SIFATI", this.SIGORTALI_SIFATI));
            sb.Append(MsgParameter("MENFAAT_SAHIBI_LISTESI", this.MENFAAT_SAHIBI_LISTESI));
            sb.Append(MsgParameter("ACENTE_KODU", this.ACENTE_KODU));
            sb.Append(MsgParameter("ALT_ACENTE_KODU", this.ALT_ACENTE_KODU));
            sb.Append(MsgParameter("ONCEKI_SIRKET_KODU", this.ONCEKI_SIRKET_KODU));
            sb.Append(MsgParameter("ONCEKI_ACENTE_KODU", this.ONCEKI_ACENTE_KODU));
            sb.Append(MsgParameter("IKAME_ARAC_SECIM", this.IKAME_ARAC_SECIM));
            sb.Append(MsgParameter("TAKSIT_SAYISI", this.TAKSIT_SAYISI));
            sb.Append(MsgParameter("ODEME_TIPI", this.ODEME_TIPI));
            sb.Append(MsgParameter("RETURN_TEKLIF_PDF", this.RETURN_TEKLIF_PDF));
            sb.Append(MsgParameter("RETURN_GENEL_SART_PDF", this.RETURN_GENEL_SART_PDF));
            sb.Append(MsgParameter("RETURN_BILGILENDIRME_FORMU_PDF", this.RETURN_BILGILENDIRME_FORMU_PDF));
            sb.Append(MsgParameter("SEND_EPOSTA", this.SEND_EPOSTA));
            sb.Append(MsgParameter("SESSION_ID", this.SESSION_ID));
            sb.Append(MsgParameter("TASINAN_EMTEA", this.TASINAN_EMTEA));
            sb.Append(MsgParameter("YURTDISI_TEMINATI", this.YURTDISI_TEMINATI));
            sb.Append(MsgParameter("YURTICI_NAKLIYECI_SORUMLULUK_KADEME", this.YURTICI_NAKLIYECI_SORUMLULUK_KADEME));
            sb.Append(MsgParameter("YURTICI_NAKLIYECI_SORUMLULUK_LIMIT", this.YURTICI_NAKLIYECI_SORUMLULUK_LIMIT));
            sb.Append(MsgParameter("CAM_HASARI_KORUMA_SZLM_SECIM", this.CAM_HASARI_KORUMA_SZLM_SECIM));
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_KASKO_TEKLIF_KAYDET>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string KISISEL_ESYA { get; set; }
        public string ONCEKI_POLICE_NO { get; set; }
        public string ONCEKI_YENILEME_NO { get; set; }
        public string BASLANGIC_TARIHI { get; set; }
        public string BITIS_TARIHI { get; set; }
        public string GRUP_URUN_ADI { get; set; }
        public string URUN_DOVIZ_CINSI { get; set; }
        public string PLAKA_TURU { get; set; }
        public string PLAKA_NO { get; set; }
        public string PLAKA_IL_KODU { get; set; }
        public string MODEL_YILI { get; set; }
        public string KULLANIM_TIPI_KODU { get; set; }
        public string KULLANIM_SEKLI_KODU { get; set; }
        public string KULLANIM_AMACI { get; set; }
        public string KASKO_MUAFIYET_KADEME { get; set; }
        public string KULLANIM_GELIR_KAYBI_SECIM { get; set; }
        public string YENI_ARAC_BEDELI { get; set; }
        public string IMM_TEMINAT_YAPISI { get; set; }
        public string IMM_SECILI_KADEME { get; set; }
        public string WEB_KULLANICI_ADI { get; set; }
        public string WEB_SIFRE { get; set; }
        public string SOFOR_SAYISI { get; set; }
        public string MUAVIN_SAYISI { get; set; }
        public string YOLCU_SAYISI { get; set; }
        public string IKAME_ARAC_BILGILERI { get; set; }
        public string DONANIM_LISTESI { get; set; }
        public string GORUNTU_CIHAZ_BEDEL { get; set; }
        public string ILETISIM_CIHAZ_BEDEL { get; set; }
        public string SES_CIHAZ_BEDEL { get; set; }
        public string KFK_TEMINAT_LISTESI { get; set; }
        public string TESCIL_TARIHI { get; set; }
        public string ARAC_TESCIL_KODU { get; set; }
        public string ARAC_TESCIL_SERI_NO { get; set; }
        public string MUSTERI_NO { get; set; }
        public string E_POSTA { get; set; }
        public string ARAC_KODU { get; set; }
        public string HASARSIZLIK_KORUMA_SZLM_SECIM { get; set; }
        public string DEPREM_MUAFIYET_SECIM { get; set; }
        public string ARAC_OPERASYONEL_KIRALIK { get; set; }
        public string SIGORTAETTIRENAYNIZAMANDASIGORTALIMI { get; set; }
        public string SIGORTALI_SIFATI { get; set; }
        public string MENFAAT_SAHIBI_LISTESI { get; set; }
        public string ACENTE_KODU { get; set; }
        public string ALT_ACENTE_KODU { get; set; }
        public string ONCEKI_SIRKET_KODU { get; set; }
        public string ONCEKI_ACENTE_KODU { get; set; }
        public string IKAME_ARAC_SECIM { get; set; }
        public string TAKSIT_SAYISI { get; set; }
        public string ODEME_TIPI { get; set; }
        public string RETURN_TEKLIF_PDF { get; set; }
        public string RETURN_GENEL_SART_PDF { get; set; }
        public string RETURN_BILGILENDIRME_FORMU_PDF { get; set; }
        public string SEND_EPOSTA { get; set; }
        public string SESSION_ID { get; set; }
        public string TASINAN_EMTEA { get; set; }
        public string YURTDISI_TEMINATI { get; set; }
        public string YURTICI_NAKLIYECI_SORUMLULUK_KADEME { get; set; }
        public string YURTICI_NAKLIYECI_SORUMLULUK_LIMIT { get; set; }
        public string CAM_HASARI_KORUMA_SZLM_SECIM { get; set; }
        
    }
}
