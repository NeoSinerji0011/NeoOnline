﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.TASIMA_YETKI_BELGE_NO = string.Empty;
            this.TASIMA_YETKI_BELGE_SERI_NO = string.Empty;
            this.PUL_NO = string.Empty;
            this.ISTIAP_HADDI_KG = string.Empty;
            this.ISTIAP_HADDI_KISI = string.Empty;
            this.KIRALIK_ARAC = string.Empty;
            this.KILOMETRE = string.Empty;
            this.ARAC_RENGI = string.Empty;
            this.ARAC_ALT_RENGI = string.Empty;
            this.ACENTE_KODU = string.Empty;
            this.ALT_ACENTE_KODU = string.Empty;
            this.MUAYENE_GECERLILIK_TARIHI = string.Empty;
            this.TAKOMETRE = string.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("ARAC_ALT_RENGI", this.ARAC_ALT_RENGI));
            sb.Append(MsgParameter("ISTIAP_HADDI_KISI", this.ISTIAP_HADDI_KISI));
            sb.Append(MsgParameter("ISTIAP_HADDI_KG", this.ISTIAP_HADDI_KG));
            sb.Append(MsgParameter("ODEME_TIPI", this.ODEME_TIPI));
            sb.Append(MsgParameter("TAKSIT_SAYISI", this.TAKSIT_SAYISI));
            sb.Append(MsgParameter("PUL_NO", this.PUL_NO));
            sb.Append(MsgParameter("ONCEKI_ACENTE_KODU", this.ONCEKI_ACENTE_KODU));
            sb.Append(MsgParameter("ONCEKI_YENILEME_NO", this.ONCEKI_YENILEME_NO));
            sb.Append(MsgParameter("ONCEKI_POLICE_NO", this.ONCEKI_POLICE_NO));
            sb.Append(MsgParameter("ONCEKI_SIRKET_KODU", this.ONCEKI_SIRKET_KODU));
            sb.Append(MsgParameter("ALT_ACENTE_KODU", this.ALT_ACENTE_KODU));
            sb.Append(MsgParameter("ACENTE_KODU", this.ACENTE_KODU));
            sb.Append(MsgParameter("KULLANIM_AMACI", this.KULLANIM_AMACI));
            sb.Append(MsgParameter("KULLANIM_TIPI_KODU", this.KULLANIM_TIPI_KODU));
            sb.Append(MsgParameter("KULLANIM_SEKLI_KODU", this.KULLANIM_SEKLI_KODU));
            sb.Append(MsgParameter("TESCIL_TARIHI", this.TESCIL_TARIHI));
            sb.Append(MsgParameter("PLAKA_TURU", this.PLAKA_TURU));
            sb.Append(MsgParameter("PLAKA_NO", this.PLAKA_NO));
            sb.Append(MsgParameter("BITIS_TARIHI", this.BITIS_TARIHI));
            sb.Append(MsgParameter("KILOMETRE", this.KILOMETRE));
            sb.Append(MsgParameter("MOTOR_NO", this.MOTOR_NO));
            sb.Append(MsgParameter("TAKOMETRE", this.TAKOMETRE));
            sb.Append(MsgParameter("PLAKA_IL_KODU", this.PLAKA_IL_KODU));
            sb.Append(MsgParameter("TASIMA_YETKI_BELGE_NO", this.TASIMA_YETKI_BELGE_NO));
            sb.Append(MsgParameter("ZKTMS_ACENTE_KODU", this.ZKTMS_ACENTE_KODU));
            sb.Append(MsgParameter("ZKTMS_ESKI_POLICE_NO", this.ZKTMS_ESKI_POLICE_NO));
            sb.Append(MsgParameter("ZKTMS_SIRKET_KODU", this.ZKTMS_SIRKET_KODU));
            sb.Append(MsgParameter("ZKTMS_YENILEME_NO", this.ZKTMS_YENILEME_NO));
            sb.Append(MsgParameter("MUSTERI_NO", this.MUSTERI_NO));
            sb.Append(MsgParameter("ARAC_KODU", this.ARAC_KODU));
            sb.Append(MsgParameter("MODEL_YILI", this.MODEL_YILI));
            sb.Append(MsgParameter("MUAYENE_GECERLILIK_TARIHI", this.MUAYENE_GECERLILIK_TARIHI));
            sb.Append(MsgParameter("ARAC_RENGI", this.ARAC_RENGI));
            sb.Append(MsgParameter("TRAFIGE_CIKIS_TARIHI", this.TRAFIGE_CIKIS_TARIHI));
            sb.Append(MsgParameter("SASI_NO", this.SASI_NO));
            sb.Append(MsgParameter("WEB_KULLANICI_ADI", this.WEB_KULLANICI_ADI));
            sb.Append(MsgParameter("WEB_SIFRE", this.WEB_SIFRE));
            sb.Append(MsgParameter("BASLANGIC_TARIHI", this.BASLANGIC_TARIHI));
            sb.Append(MsgParameter("ARAC_TESCIL_KODU", this.ARAC_TESCIL_KODU));
            sb.Append(MsgParameter("ARAC_TESCIL_SERI_NO", this.ARAC_TESCIL_SERI_NO));
            sb.Append(MsgParameter("E_POSTA", this.E_POSTA));
            sb.Append(MsgParameter("SEND_EPOSTA", this.SEND_EPOSTA));
            sb.Append(MsgParameter("RETURN_GENEL_SART_PDF", this.RETURN_GENEL_SART_PDF));
            sb.Append(MsgParameter("KIRALIK_ARAC", this.KIRALIK_ARAC));
            sb.Append(MsgParameter("RETURN_BILGILENDIRME_FORMU_PDF", this.RETURN_BILGILENDIRME_FORMU_PDF));
            sb.Append(MsgParameter("RETURN_TEKLIF_PDF", this.RETURN_TEKLIF_PDF));
            sb.Append(MsgParameter("SESSION_ID", this.SESSION_ID));
            sb.Append(MsgParameter("TASIMA_YETKI_BELGE_SERI_NO", this.TASIMA_YETKI_BELGE_SERI_NO));
            sb.Append(MsgParameter("TRAFIK_PAKET_ID", this.TRAFIK_PAKET_ID));
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_YENI_TRAFIK_TEKLIF_KAYDET>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string ARAC_ALT_RENGI { get; set; }
        public string ISTIAP_HADDI_KISI { get; set; }
        public string ISTIAP_HADDI_KG { get; set; }
        public string ODEME_TIPI { get; set; }
        public string TAKSIT_SAYISI { get; set; }
        public string PUL_NO { get; set; }
        public string ONCEKI_ACENTE_KODU { get; set; }
        public string ONCEKI_YENILEME_NO { get; set; }
        public string ONCEKI_POLICE_NO { get; set; }
        public string ONCEKI_SIRKET_KODU { get; set; }
        public string ALT_ACENTE_KODU { get; set; }
        public string ACENTE_KODU { get; set; }
        public string KULLANIM_AMACI { get; set; }
        public string KULLANIM_TIPI_KODU { get; set; }
        public string KULLANIM_SEKLI_KODU { get; set; }
        public string TESCIL_TARIHI { get; set; }
        public string PLAKA_TURU { get; set; }
        public string PLAKA_NO { get; set; }
        public string BITIS_TARIHI { get; set; }
        public string KILOMETRE { get; set; }
        public string MOTOR_NO { get; set; }
        public string TAKOMETRE { get; set; }
        public string PLAKA_IL_KODU { get; set; }
        public string TASIMA_YETKI_BELGE_NO { get; set; }
        public string ZKTMS_ACENTE_KODU { get; set; }
        public string ZKTMS_ESKI_POLICE_NO { get; set; }
        public string ZKTMS_SIRKET_KODU { get; set; }
        public string ZKTMS_YENILEME_NO { get; set; }
        public string MUSTERI_NO { get; set; }
        public string ARAC_KODU { get; set; }
        public string MODEL_YILI { get; set; }
        public string MUAYENE_GECERLILIK_TARIHI { get; set; }
        public string ARAC_RENGI { get; set; }
        public string TRAFIGE_CIKIS_TARIHI { get; set; }
        public string SASI_NO { get; set; }
        public string WEB_KULLANICI_ADI { get; set; }
        public string WEB_SIFRE { get; set; }
        public string BASLANGIC_TARIHI { get; set; }
        public string ARAC_TESCIL_KODU { get; set; }
        public string ARAC_TESCIL_SERI_NO { get; set; }
        public string E_POSTA { get; set; }
        public string SEND_EPOSTA { get; set; }
        public string RETURN_GENEL_SART_PDF { get; set; }
        public string KIRALIK_ARAC { get; set; }
        public string RETURN_BILGILENDIRME_FORMU_PDF { get; set; }
        public string RETURN_TEKLIF_PDF { get; set; }
        public string SESSION_ID { get; set; }
        public string TASIMA_YETKI_BELGE_SERI_NO { get; set; }
        public string TRAFIK_PAKET_ID { get; set; }

    }
}