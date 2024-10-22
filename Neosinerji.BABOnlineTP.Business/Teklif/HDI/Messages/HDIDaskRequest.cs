using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIDaskRequest : HDIMessage
    {
        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string IstekTipi { get; set; }
        public string HDIPoliceNumara { get; set; }
        public string DASKPoliceNumara { get; set; }
        public string ZeyilTuru { get; set; }
        public string BaslangicTarihi { get; set; }
        public string SEOzelTuzel { get; set; }
        public string SEAd { get; set; }
        public string SESoyad { get; set; }
        public string SEUyruk { get; set; }
        public string SETcKimlikNo { get; set; }
        public string SEYabanciKimlikNo { get; set; }
        public string SEPasaportNo { get; set; }
        public string SEVergiKimlikNo { get; set; }
        public string SEEvTel { get; set; }
        public string SECepTel { get; set; }

        public string SESifat { get; set; }
        public string SEEMail { get; set; }

        public string SigortaliSayisi { get; set; }
        public string SgOzelTuzel1 { get; set; }
        public string SgAd1 { get; set; }
        public string SgSoyad1 { get; set; }
        public string SgUyruk1 { get; set; }
        public string SgTcKimlikNo1 { get; set; }
        public string SgYabanciKimlikNo1 { get; set; }
        public string SgPasaportNo1 { get; set; }
        public string SgVergiKimlikNo1 { get; set; }
        public string SgEvTel1 { get; set; }
        public string SgCepTel1 { get; set; }
        public string SgEMail1 { get; set; }

        public string WSRIZ { get; set; }
        public string RiskSemt { get; set; }
        public string RiskMahalle { get; set; }
        public string RiskCadde { get; set; }
        public string RiskSokak { get; set; }
        public string RiskSiteApartmanAd { get; set; }
        public string RiskBinaNo { get; set; }
        public string RiskKat { get; set; }
        public string RiskDaire { get; set; }
        public string RiskPostaKod { get; set; }
        public string RiskIlKod { get; set; }
        public string RiskIlce { get; set; }
        public string RiskBelde { get; set; }

        //WSRIZ = H ise iletişim bilgileri gönderilmelidir. WSRIZ = E ise iletişim bilgilerine ait herhangi bir alanın gönderilmesine gerek yoktur…!									

        public string IletisimSemt { get; set; }
        public string IletisimMahalle { get; set; }
        public string IletisimCadde { get; set; }
        public string IletisimSokak { get; set; }
        public string IletisimSiteApartmanAd { get; set; }
        public string IletisimBinaNo { get; set; }
        public string IletisimKat { get; set; }
        public string IletisimDaire { get; set; }
        public string IletisimPostaKod { get; set; }
        public string IletisimIlKod { get; set; }
        public string IletisimIlce { get; set; }
        public string IletisimBelde { get; set; }
        public string DaireYuzOlcumu { get; set; }
        public string BinaYapiTarzi { get; set; }
        public string BinaInsaatYili { get; set; }
        public string ToplamKatSayisi { get; set; }
        public string DaireKullanimSekli { get; set; }
        public string BinaHasar { get; set; }
        public string EvrakTarihSayi { get; set; }
        public string Ada { get; set; }

        public string Pafta { get; set; }
        public string Parsel { get; set; }
        public string Sayfa { get; set; }
        public string RehinAlacak { get; set; }
        public string Kurum { get; set; }
        public string KurumID { get; set; }
        public string SubeID { get; set; }
        public string HesapSozlesmeNo { get; set; }
        public string KrediBitisTarih { get; set; }
        public string KrediTutari { get; set; }
        public string DovizKodu { get; set; }
        public string OdemeTipi { get; set; }

        //OdemeTipi = 1 ise aşağıdaki alanların tümü zorunlu olur		
        public string TaksitSayisi { get; set; }
        public string TckR { get; set; }
        //public string 1..16 Kart No 17..19 Güvenlik No 20..21 Ay 22..25 Yıl { get; set; }
        public string KartAd { get; set; }
        public string KartSoyad { get; set; }

        public string UAVTKodu { get; set; }
    }



    public class HDIIllerRequest : HDIMessage
    {
        public HDIIllerRequest()
        {
            Uygulama = "il_liste";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
    }

    public class HDIIlcelerRequest : HDIMessage
    {
        public HDIIlcelerRequest()
        {
            Uygulama = "ilce_liste";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string code { get; set; }
    }

    public class HDIBeldelerRequest : HDIMessage
    {
        public HDIBeldelerRequest()
        {
            Uygulama = "belde_liste";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string code { get; set; }
    }

    public class HDIMahallelerRequest : HDIMessage
    {
        public HDIMahallelerRequest()
        {
            Uygulama = "mahalle_liste";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string code { get; set; }
    }

    public class HDICaddeSokakBulvarMeydanRequest : HDIMessage
    {
        public HDICaddeSokakBulvarMeydanRequest()
        {
            Uygulama = "csbm_liste";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string code { get; set; }
        public string aciklama { get; set; }
    }

    public class HDICaddeSokakBulvarMeydanBinaAdRequest : HDIMessage
    {
        public HDICaddeSokakBulvarMeydanBinaAdRequest()
        {
            Uygulama = "csbm_bina_ad_liste";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string code { get; set; }
        public string aciklama { get; set; }
    }

    public class HDIDairelerRequest : HDIMessage
    {
        public HDIDairelerRequest()
        {
            Uygulama = "daire_liste";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string code { get; set; }
    }

    public class HDIUAVTAdresRequest : HDIMessage
    {
        public HDIUAVTAdresRequest()
        {
            Uygulama = "uavt_adres";
        }

        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string code { get; set; }
    }



}
