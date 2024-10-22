using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    [Serializable]
    public class PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET : ANADOLUMessage
    {
        public PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET()
        {
            this.CORE_CLIENT = "ERE";
            this.CORE_DOMAIN = "ANADOLUSIGORTA";
            this.CORE_LANGUAGE = "tr";
            this.TCKN = String.Empty;
            this.VKN = String.Empty;
            this.KIBRIS_KIMLIK_NO = String.Empty;
            this.YABANCI_KIMLIK_NO = String.Empty;
            this.ULKE_KODU = String.Empty;
            this.PASAPORT_NO = String.Empty;
            this.PASAPORT_GECERLILIK_TARIHI = String.Empty;
            this.DOGUM_TARIHI = String.Empty;
            this.MUSTERI_BINA_ADI = String.Empty;
            this.FAALIYET_OLCEGI = String.Empty;
            this.ANA_SEKTOR_KODU = String.Empty;
            this.ALT_SEKTOR_KODU = String.Empty;
            this.SABIT_VARLIK_ARALIGI = String.Empty;
            this.CIRO_ARALIGI = String.Empty;
            this.BAGLANTI_TIPI_KODU = String.Empty;
            this.BAGLANTILI_KURUM_KODU = String.Empty;
            this.BAGLANTILI_MUSTERI_KURUM = String.Empty;
            this.BELDE_ADI = String.Empty;
            this.CINSIYET = String.Empty;
            this.MESLEK_GRUBU_KODU = String.Empty;
            this.MESLEK_TIPI_KODU = String.Empty;
            this.MUSTERI_ADI = String.Empty;
            this.MUSTERI_KAPI_NO = String.Empty;
            this.MUSTERI_SOYADI = String.Empty;
            this.MUSTERI_TEL_DAHILI = String.Empty;
        }

        public override string GetSoapAction()
        {
            return base.GetSoapAction() + "PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET";
        }

        public override string GetMessageBody()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<cms:PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET>");
            sb.Append(MsgParameter("CORE_CLIENT", this.CORE_CLIENT));
            sb.Append(MsgParameter("CORE_DOMAIN", this.CORE_DOMAIN));
            sb.Append(MsgParameter("CORE_LANGUAGE", this.CORE_LANGUAGE));
            sb.Append(MsgParameter("PASAPORT_GECERLILIK_TARIHI", this.PASAPORT_GECERLILIK_TARIHI));
            sb.Append(MsgParameter("PASAPORT_NO", this.PASAPORT_NO));
            sb.Append(MsgParameter("ADRES_IL_KODU", this.ADRES_IL_KODU));
            sb.Append(MsgParameter("ILCE_ADI", this.ILCE_ADI));
            sb.Append(MsgParameter("ILCE_KODU", this.ILCE_KODU));
            sb.Append(MsgParameter("BELDE_ADI", this.BELDE_ADI));
            sb.Append(MsgParameter("ADRES_TIPI", this.ADRES_TIPI));
            sb.Append(MsgParameter("MUSTERI_ADI", this.MUSTERI_ADI));
            sb.Append(MsgParameter("MUSTERI_SOYADI", this.MUSTERI_SOYADI));
            sb.Append(MsgParameter("DOGUM_TARIHI", this.DOGUM_TARIHI));
            sb.Append(MsgParameter("CINSIYET", this.CINSIYET));
            sb.Append(MsgParameter("UNVAN", this.UNVAN));
            sb.Append(MsgParameter("MUSTERI_BINA_ADI", this.MUSTERI_BINA_ADI));
            sb.Append(MsgParameter("MUSTERI_MAHALLE_KOY", this.MUSTERI_MAHALLE_KOY));
            sb.Append(MsgParameter("MUSTERI_SOKAK", this.MUSTERI_SOKAK));
            sb.Append(MsgParameter("MUSTERI_CADDE", this.MUSTERI_CADDE));
            sb.Append(MsgParameter("MUSTERI_DAIRE_NO", this.MUSTERI_DAIRE_NO));
            sb.Append(MsgParameter("MUSTERI_KAPI_NO", this.MUSTERI_KAPI_NO));
            sb.Append(MsgParameter("MUSTERI_SERBESTADRES", this.MUSTERI_SERBESTADRES));
            sb.Append(MsgParameter("MUSTERI_TEL_ALAN", this.MUSTERI_TEL_ALAN));
            sb.Append(MsgParameter("MESLEK_GRUBU_KODU", this.MESLEK_GRUBU_KODU));
            sb.Append(MsgParameter("MESLEK_TIPI_KODU", this.MESLEK_TIPI_KODU));
            sb.Append(MsgParameter("MUSTERI_TEL_NO", this.MUSTERI_TEL_NO));
            sb.Append(MsgParameter("MUSTERI_TEL_TIP", this.MUSTERI_TEL_TIP));
            sb.Append(MsgParameter("MUSTERI_TEL_DAHILI", this.MUSTERI_TEL_DAHILI));
            sb.Append(MsgParameter("BAGLANTI_TIPI_KODU", this.BAGLANTI_TIPI_KODU));
            sb.Append(MsgParameter("BAGLANTILI_KURUM_KODU", this.BAGLANTILI_KURUM_KODU));
            sb.Append(MsgParameter("BAGLANTILI_MUSTERI_KURUM", this.BAGLANTILI_MUSTERI_KURUM));
            sb.Append(MsgParameter("KIBRIS_KIMLIK_NO", this.KIBRIS_KIMLIK_NO));
            sb.Append(MsgParameter("YABANCI_KIMLIK_NO", this.YABANCI_KIMLIK_NO));
            sb.Append(MsgParameter("WEB_KULLANICI_ADI", this.WEB_KULLANICI_ADI));
            sb.Append(MsgParameter("WEB_SIFRE", this.WEB_SIFRE));
            sb.Append(MsgParameter("MUSTERI_EPOSTA", this.MUSTERI_EPOSTA));
            sb.Append(MsgParameter("ULKE_KODU", this.ULKE_KODU));
            sb.Append(MsgParameter("VKN", this.VKN));
            sb.Append(MsgParameter("TCKN", this.TCKN));
            sb.Append(MsgParameter("ANA_SEKTOR_KODU", this.ANA_SEKTOR_KODU));
            sb.Append(MsgParameter("ALT_SEKTOR_KODU", this.ALT_SEKTOR_KODU));
            sb.Append(MsgParameter("FAALIYET_OLCEGI", this.FAALIYET_OLCEGI));
            sb.Append(MsgParameter("CIRO_ARALIGI", this.CIRO_ARALIGI));
            sb.Append(MsgParameter("SABIT_VARLIK_ARALIGI", this.SABIT_VARLIK_ARALIGI));
            sb.Append(MsgParameter("MUSTERI_TIPI", this.MUSTERI_TIPI));
            sb.Append("</cms:PR_SWEP_MUS_WEB_SWEPTE_MUSTERI_KAYDET>");

            return sb.ToString();
        }

        public string CORE_CLIENT { get; set; }
        public string CORE_DOMAIN { get; set; }
        public string CORE_LANGUAGE { get; set; }
        public string PASAPORT_GECERLILIK_TARIHI { get; set; }
        public string PASAPORT_NO { get; set; }
        public string ADRES_IL_KODU { get; set; }
        public string ILCE_ADI { get; set; }
        public string ILCE_KODU { get; set; }
        public string BELDE_ADI { get; set; }
        public string ADRES_TIPI { get; set; }
        public string MUSTERI_ADI { get; set; }
        public string MUSTERI_SOYADI { get; set; }
        public string DOGUM_TARIHI { get; set; }
        public string CINSIYET { get; set; }
        public string UNVAN { get; set; }
        public string MUSTERI_BINA_ADI { get; set; }
        public string MUSTERI_MAHALLE_KOY { get; set; }
        public string MUSTERI_SOKAK { get; set; }
        public string MUSTERI_CADDE { get; set; }
        public string MUSTERI_DAIRE_NO { get; set; }
        public string MUSTERI_KAPI_NO { get; set; }
        public string MUSTERI_SERBESTADRES { get; set; }
        public string MUSTERI_TEL_ALAN { get; set; }
        public string MESLEK_GRUBU_KODU { get; set; }
        public string MESLEK_TIPI_KODU { get; set; }
        public string MUSTERI_TEL_NO { get; set; }
        public string MUSTERI_TEL_TIP { get; set; }
        public string MUSTERI_TEL_DAHILI { get; set; }
        public string BAGLANTI_TIPI_KODU { get; set; }
        public string BAGLANTILI_KURUM_KODU { get; set; }
        public string BAGLANTILI_MUSTERI_KURUM { get; set; }
        public string KIBRIS_KIMLIK_NO { get; set; }
        public string YABANCI_KIMLIK_NO { get; set; }
        public string WEB_KULLANICI_ADI { get; set; }
        public string WEB_SIFRE { get; set; }
        public string MUSTERI_EPOSTA { get; set; }
        public string ULKE_KODU { get; set; }
        public string VKN { get; set; }
        public string TCKN { get; set; }
        public string ANA_SEKTOR_KODU { get; set; }
        public string ALT_SEKTOR_KODU { get; set; }
        public string FAALIYET_OLCEGI { get; set; }
        public string CIRO_ARALIGI { get; set; }
        public string SABIT_VARLIK_ARALIGI { get; set; }
        public string MUSTERI_TIPI { get; set; }
    }
}
