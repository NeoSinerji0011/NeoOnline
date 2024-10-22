using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1 : ANADOLUMessage
    {
        public PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.ZKTMS_SIRKET_KODU = String.Empty;
            this.ZKTMS_ACENTE_KODU = String.Empty;
            this.ZKTMS_ESKI_POLICE_NO = String.Empty;
            this.ZKTMS_YENILEME_NO = String.Empty;
            this.ONCEKI_ACENTE_KODU = String.Empty;
            this.ONCEKI_POLICE_NO = String.Empty;
            this.ONCEKI_SIRKET_KODU = String.Empty;
            this.ONCEKI_YENILEME_NO = String.Empty;
            this.TASIMA_YETKI_BELGE_NO = String.Empty;
            this.TASIMA_YETKI_BELGE_SERI_NO = String.Empty;
            this.TCKN = String.Empty;
            this.VKN = String.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<cms:PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("ARAC_KODU", this.ARAC_KODU));
            sb.Append(MsgParameter("ARAC_TESCIL_KODU", this.ARAC_TESCIL_KODU));
            sb.Append(MsgParameter("ZKTMS_YENILEME_NO", this.ZKTMS_YENILEME_NO));
            sb.Append(MsgParameter("ZKTMS_SIRKET_KODU", this.ZKTMS_SIRKET_KODU));
            sb.Append(MsgParameter("ZKTMS_ESKI_POLICE_NO", this.ZKTMS_ESKI_POLICE_NO));
            sb.Append(MsgParameter("ZKTMS_ACENTE_KODU", this.ZKTMS_ACENTE_KODU));
            sb.Append(MsgParameter("WEB_KULLANICI_ADI", this.WEB_KULLANICI_ADI));
            sb.Append(MsgParameter("WEB_GIRIS", this.WEB_GIRIS));
            sb.Append(MsgParameter("VKN", this.VKN));
            sb.Append(MsgParameter("TRAMER_TARIFE_KODU", this.TRAMER_TARIFE_KODU));
            sb.Append(MsgParameter("TRAFIK_TESCIL_TARIHI", this.TRAFIK_TESCIL_TARIHI));
            sb.Append(MsgParameter("TCKN", this.TCKN));
            sb.Append(MsgParameter("TASIMA_YETKI_BELGE_SERI_NO", this.TASIMA_YETKI_BELGE_SERI_NO));
            sb.Append(MsgParameter("ARAC_TESCIL_SERI_NO", this.ARAC_TESCIL_SERI_NO));
            sb.Append(MsgParameter("BITIS_TARIHI", this.BITIS_TARIHI));
            sb.Append(MsgParameter("DOGUM_TARIHI", this.DOGUM_TARIHI));
            sb.Append(MsgParameter("CINSIYET", this.CINSIYET));
            sb.Append(MsgParameter("MUSTERI_ILCE_KODU", this.MUSTERI_ILCE_KODU));
            sb.Append(MsgParameter("MUSTERI_IL_KODU", this.MUSTERI_IL_KODU));
            sb.Append(MsgParameter("MUSTERI_TIPI", this.MUSTERI_TIPI));
            sb.Append(MsgParameter("MODEL_YILI", this.MODEL_YILI));
            sb.Append(MsgParameter("ONCEKI_ACENTE_KODU", this.ONCEKI_ACENTE_KODU));
            sb.Append(MsgParameter("ONCEKI_POLICE_NO", this.ONCEKI_POLICE_NO));
            sb.Append(MsgParameter("ONCEKI_SIRKET_KODU", this.ONCEKI_SIRKET_KODU));
            sb.Append(MsgParameter("ONCEKI_YENILEME_NO", this.ONCEKI_YENILEME_NO));
            sb.Append(MsgParameter("PLAKA", this.PLAKA));
            sb.Append(MsgParameter("PLAKA_IL_KODU", this.PLAKA_IL_KODU));
            sb.Append(MsgParameter("PLAKA_TIPI", this.PLAKA_TIPI));
            sb.Append(MsgParameter("TASIMA_YETKI_BELGE_NO", this.TASIMA_YETKI_BELGE_NO));
            sb.Append(MsgParameter("SESSION_ID", String.Empty));
            sb.Append("</cms:PR_SWEP_URT_OTO_WS_TRAFIK_PRIM_HESAPLA_V1>");
            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string ARAC_KODU { get; set; }
        public string ARAC_TESCIL_KODU { get; set; }
        public string ZKTMS_YENILEME_NO { get; set; }
        public string ZKTMS_SIRKET_KODU { get; set; }
        public string ZKTMS_ESKI_POLICE_NO { get; set; }
        public string ZKTMS_ACENTE_KODU { get; set; }
        public string WEB_KULLANICI_ADI { get; set; }
        public string WEB_GIRIS { get; set; }
        public string VKN { get; set; }
        public string TRAMER_TARIFE_KODU { get; set; }
        public string TRAFIK_TESCIL_TARIHI { get; set; }
        public string TCKN { get; set; }
        public string TASIMA_YETKI_BELGE_SERI_NO { get; set; }
        public string ARAC_TESCIL_SERI_NO { get; set; }
        public string BITIS_TARIHI { get; set; }
        public string DOGUM_TARIHI { get; set; }
        public string CINSIYET { get; set; }
        public string MUSTERI_ILCE_KODU { get; set; }
        public string MUSTERI_IL_KODU { get; set; }
        public string MUSTERI_TIPI { get; set; }
        public string MODEL_YILI { get; set; }
        public string ONCEKI_ACENTE_KODU { get; set; }
        public string ONCEKI_POLICE_NO { get; set; }
        public string ONCEKI_SIRKET_KODU { get; set; }
        public string ONCEKI_YENILEME_NO { get; set; }
        public string PLAKA { get; set; }
        public string PLAKA_IL_KODU { get; set; }
        public string PLAKA_TIPI { get; set; }
        public string TASIMA_YETKI_BELGE_NO { get; set; }
    }
}
