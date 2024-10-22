using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;


namespace Neosinerji.BABOnlineTP.Business.RAY
{
    [XmlRootAttribute("TRAFIK_EGM", Namespace = "", IsNullable = false)]
    public class RAY_Trafik_EGM_Arac_Sorgu_Response
    {
        public TRAFIK TRAFIK { get; set; }
        public EGM_TESCIL_SORGU EGM_TESCIL_SORGU { get; set; }

        [XmlElement]
        public string CORRELATION_ID { get; set; }
    }

    public class TRAFIK
    {
        public GecmisPoliceBilgileri GeçmişPoliceBilgileri { get; set; }
        public YururPoliceBilgileri YururPoliceBilgileri { get; set; }
    }

    public class EGM_TESCIL_SORGU
    {
        [XmlElement]
        public string faultcode { get; set; }

        [XmlElement]
        public string faultstring { get; set; }

        private detail _detail;
        [XmlElement]
        public detail detail { get { return _detail; } set { _detail = value; } }
    }

    public class detail
    {
        private failure[] _failureList;

        [XmlElement]
        public string code { get; set; }

        public failure[] failureList { get { return _failureList; } set { _failureList = value; } }

        [XmlElement]
        public string message { get; set; }


    }
    public class failure
    {
        private string _code;
        private string _message;

        [XmlElement]
        public string code { get { return _code; } set { _code = value; } }

        [XmlElement]
        public string message { get { return _message; } set { _message = value; } }
    }

    //-----Geçmiş Poliçe Bilgileri
    public class GecmisPoliceBilgileri
    {
        private aracTemelBilgileri _aracTemelBilgileri;
        [XmlElement]
        public aracTemelBilgileri aracTemelBilgileri { get { return _aracTemelBilgileri; } set { _aracTemelBilgileri = value; } }


        private belgeBilgileri _belgeBilgileri;
        [XmlElement]
        public belgeBilgileri belgeBilgileri { get { return _belgeBilgileri; } set { _belgeBilgileri = value; } }

        [XmlElement]
        public string ilkPoliceNo { get; set; }

        [XmlElement]
        public string islemTarihi { get; set; }

        [XmlElement]
        public string policeAcenteBilgileri { get; set; }

        private policeAnahtari _policeAnahtari;
        [XmlElement]
        public policeAnahtari policeAnahtari { get { return _policeAnahtari; } set { _policeAnahtari = value; } }

        [XmlElement]
        public string policeEkiNo { get; set; }

        [XmlElement]
        public string policeEkiTuru { get; set; }

        private policeTeminatBilgileri _policeTeminatBilgileri;
        [XmlElement]
        public policeTeminatBilgileri policeTeminatBilgileri { get { return _policeTeminatBilgileri; } set { _policeTeminatBilgileri = value; } }

        [XmlElement]
        public string primBilgileri { get; set; }

        [XmlElement]
        public string sbmTramerNo { get; set; }

        private sigortali _sigortali;
        [XmlElement]
        public sigortali sigortali { get { return _sigortali; } set { _sigortali = value; } }

        [XmlElement]
        public string sistemSaati { get; set; }

        [XmlElement]
        public string sistemTarihi { get; set; }

        [XmlElement]
        public string tahakkukIptalKodu { get; set; }

        [XmlElement]
        public string tanzimIlKodu { get; set; }

        private tarihBilgileri _tarihBilgileri;
        [XmlElement]
        public tarihBilgileri tarihBilgileri { get { return _tarihBilgileri; } set { _tarihBilgileri = value; } }

        [XmlElement]
        public string urunKodu { get; set; }

        [XmlElement]
        public string zkytmsIndirimYuzde { get; set; }
    }

    //----Araç Temel Bilgileri
    public class aracTemelBilgileri
    {
        [XmlElement]
        public string aracTarifeGrupKod { get; set; }

        [XmlElement]
        public string bilgiKaynagi { get; set; }

        [XmlElement]
        public string kullanimSekli { get; set; }

        private marka _marka;
        [XmlElement]
        public marka marka { get { return _marka; } set { _marka = value; } }

        [XmlElement]
        public string modelYili { get; set; }

        [XmlElement]
        public string motorGucu { get; set; }

        [XmlElement]
        public string motorNo { get; set; }

        private plaka _plaka;
        [XmlElement]
        public plaka plaka { get { return _plaka; } set { _plaka = value; } }

        [XmlElement]
        public string sasiNo { get; set; }

        [XmlElement]
        public string silindirHacmi { get; set; }

        [XmlElement]
        public string tescilTarihi { get; set; }

        private tip _tip;
        [XmlElement]
        public tip tip { get { return _tip; } set { _tip = value; } }

        [XmlElement]
        public string yolcuKapasitesi { get; set; }

        [XmlElement]
        public string yukKapasitesi { get; set; }

    }

    public class marka
    {
        private string _aciklama;
        private string _kod;

        [XmlElement]
        public string aciklama { get { return _aciklama; } set { _aciklama = value; } }

        [XmlElement]
        public string kod { get { return _kod; } set { _kod = value; } }
    }

    public class plaka
    {
        private string _ilKodu;
        private string _no;


        [XmlElement]
        public string ilKodu { get { return _ilKodu; } set { _ilKodu = value; } }

        [XmlElement]
        public string no { get { return _no; } set { _no = value; } }
    }

    public class tip
    {
        private string _aciklama;
        private string _kod;

        [XmlElement]
        public string aciklama { get { return _aciklama; } set { _aciklama = value; } }

        [XmlElement]
        public string kod { get { return _kod; } set { _kod = value; } }
    }


    //----Belge Bilgileri
    public class belgeBilgileri
    {
        [XmlElement]
        public string belgeNo { get; set; }

        [XmlElement]
        public string belgeSiraNo { get; set; }

        [XmlElement]
        public string belgeTarih { get; set; }

        private oncekiPoliceAnahtari _oncekiPoliceAnahtari;


        public oncekiPoliceAnahtari oncekiPoliceAnahtari { get { return _oncekiPoliceAnahtari; } set { _oncekiPoliceAnahtari = value; } }

        [XmlElement]
        public string uygulanmasiGerekenGecikmeSurprimYuzde { get; set; }

        [XmlElement]
        public string uygulanmasiGerekenIndirimYuzde { get; set; }

        [XmlElement]
        public string uygulanmasiGerekenSurprimYuzde { get; set; }

        [XmlElement]
        public string uygulanmisIlPlakaIndirimYuzde { get; set; }

        [XmlElement]
        public string uygulanmisTarifeBasamakKodu { get; set; }
    }

    public class oncekiPoliceAnahtari
    {
        private string _acenteKod;
        private string _policeNo;
        private string _sirketKodu;
        private string _yenilemeNo;

        [XmlElement]
        public string acenteKod { get { return _acenteKod; } set { _acenteKod = value; } }

        [XmlElement]
        public string policeNo { get { return _policeNo; } set { _policeNo = value; } }

        [XmlElement]
        public string sirketKodu { get { return _sirketKodu; } set { _sirketKodu = value; } }

        [XmlElement]
        public string yenilemeNo { get { return _yenilemeNo; } set { _yenilemeNo = value; } }
    }

    public class policeAnahtari
    {
        public string _acenteKod;
        public string _policeNo;
        public string _sirketKodu;
        public string _yenilemeNo;

        [XmlElement]
        public string acenteKod { get { return _acenteKod; } set { _acenteKod = value; } }

        [XmlElement]
        public string policeNo { get { return _policeNo; } set { _policeNo = value; } }

        [XmlElement]
        public string sirketKodu { get { return _sirketKodu; } set { _sirketKodu = value; } }

        [XmlElement]
        public string yenilemeNo { get { return _yenilemeNo; } set { _yenilemeNo = value; } }
    }

    public class policeTeminatBilgileri
    {
        private string _aracMaddiTeminat;
        private string _kazaMaddiTeminat;
        private string _kazaSakatlikOlumTeminat;
        private string _kazaTedaviTeminat;
        private string _kisiSakatlikOlumTeminat;
        private string _kisiTedaviTeminat;

        [XmlElement]
        public string aracMaddiTeminat { get { return _aracMaddiTeminat; } set { _aracMaddiTeminat = value; } }

        [XmlElement]
        public string kazaMaddiTeminat { get { return _kazaMaddiTeminat; } set { _kazaMaddiTeminat = value; } }

        [XmlElement]
        public string kazaSakatlikOlumTeminat { get { return _kazaSakatlikOlumTeminat; } set { _kazaSakatlikOlumTeminat = value; } }

        [XmlElement]
        public string kazaTedaviTeminat { get { return _kazaTedaviTeminat; } set { _kazaTedaviTeminat = value; } }

        [XmlElement]
        public string kisiSakatlikOlumTeminat { get { return _kisiSakatlikOlumTeminat; } set { _kisiSakatlikOlumTeminat = value; } }

        [XmlElement]
        public string kisiTedaviTeminat { get { return _kisiTedaviTeminat; } set { _kisiTedaviTeminat = value; } }
    }

    public class sigortali
    {
        private string _adUnvan;
        private string _adres;
        private string _soyad;
        private string _tckimlikNo;
        private string _turKodu;
        private string _uyruk;

        [XmlElement]
        public string adUnvan { get { return _adUnvan; } set { _adUnvan = value; } }

        [XmlElement]
        public string adres { get { return _adres; } set { _adres = value; } }

        [XmlElement]
        public string soyad { get { return _soyad; } set { _soyad = value; } }

        [XmlElement]
        public string tckimlikNo { get { return _tckimlikNo; } set { _tckimlikNo = value; } }

        [XmlElement]
        public string turKodu { get { return _turKodu; } set { _turKodu = value; } }

        [XmlElement]
        public string uyruk { get { return _uyruk; } set { _uyruk = value; } }
    }

    public class tarihBilgileri
    {
        private string _baslangicTarihi;
        private string _bitisTarihi;
        private string _tanzimTarihi;


        [XmlElement]
        public string baslangicTarihi { get { return _baslangicTarihi; } set { _baslangicTarihi = value; } }

        [XmlElement]
        public string bitisTarihi { get { return _bitisTarihi; } set { _bitisTarihi = value; } }

        [XmlElement]
        public string tanzimTarihi { get { return _tanzimTarihi; } set { _tanzimTarihi = value; } }
    }

    public class YururPoliceBilgileri
    {
        private AracTemelBilgileri _AracTemelBilgileri;
        private BelgeBilgileri _BelgeBilgileri;

        [XmlElement]
        public AracTemelBilgileri aracTemelBilgileri { get { return _AracTemelBilgileri; } set { _AracTemelBilgileri = value; } }

        [XmlElement]
        public BelgeBilgileri belgeBilgileri { get { return _BelgeBilgileri; } set { _BelgeBilgileri = value; } }

        [XmlElement]
        public string ilkPoliceNo { get; set; }

        [XmlElement]
        public string islemTarihi { get; set; }

        [XmlElement]
        public string policeAcenteBilgileri { get; set; }

        private policeAnahtari _policeAnahtari;
        [XmlElement]
        public policeAnahtari policeAnahtari { get { return _policeAnahtari; } set { _policeAnahtari = value; } }

        [XmlElement]
        public string policeEkiNo { get; set; }

        [XmlElement]
        public string policeEkiTuru { get; set; }

        private policeTeminatBilgileri _policeTeminatBilgileri;
        [XmlElement]
        public policeTeminatBilgileri policeTeminatBilgileri { get { return _policeTeminatBilgileri; } set { _policeTeminatBilgileri = value; } }

        [XmlElement]
        public string primBilgileri { get; set; }

        [XmlElement]
        public string sbmTramerNo { get; set; }

        private sigortali _sigortali;
        [XmlElement]
        public sigortali sigortali { get { return _sigortali; } set { _sigortali = value; } }

        [XmlElement]
        public string sistemSaati { get; set; }

        [XmlElement]
        public string sistemTarihi { get; set; }

        [XmlElement]
        public string tahakkukIptalKodu { get; set; }

        [XmlElement]
        public string tanzimIlKodu { get; set; }

        private tarihBilgileri _tarihBilgileri;
        [XmlElement]
        public tarihBilgileri tarihBilgileri { get { return _tarihBilgileri; } set { _tarihBilgileri = value; } }

        [XmlElement]
        public string urunKodu { get; set; }

        [XmlElement]
        public string zkytmsIndirimYuzde { get; set; }
    }

    public class AracTemelBilgileri
    {
        [XmlElement]
        public string aracRengi { get; set; }

        [XmlElement]
        public string aracTarifeGrupKod { get; set; }

        [XmlElement]
        public string bilgiKaynagi { get; set; }

        [XmlElement]
        public string kullanimSekli { get; set; }

        private marka _marka;
        [XmlElement]
        public marka marka { get { return _marka; } set { _marka = value; } }

        [XmlElement]
        public string modelYili { get; set; }

        [XmlElement]
        public string motorGucu { get; set; }

        [XmlElement]
        public string motorNo { get; set; }

        private plaka _plaka;
        [XmlElement]
        public plaka plaka { get { return _plaka; } set { _plaka = value; } }

        [XmlElement]
        public string sasiNo { get; set; }

        [XmlElement]
        public string silindirHacmi { get; set; }

        [XmlElement]
        public string tescilTarihi { get; set; }

        private tip _tip;
        [XmlElement]
        public tip tip { get { return _tip; } set { _tip = value; } }

        [XmlElement]
        public string yolcuKapasitesi { get; set; }

        [XmlElement]
        public string yukKapasitesi { get; set; }

    }

    public class BelgeBilgileri
    {
        [XmlElement]
        public string belgeNo { get; set; }

        [XmlElement]
        public string belgeSiraNo { get; set; }

        [XmlElement]
        public string oncekiPoliceAnahtari { get; set; }

        [XmlElement]
        public string uygulanmasiGerekenGecikmeSurprimYuzde { get; set; }

        [XmlElement]
        public string uygulanmasiGerekenIndirimYuzde { get; set; }

        [XmlElement]
        public string uygulanmasiGerekenSurprimYuzde { get; set; }

        [XmlElement]
        public string uygulanmasiGerekenTarifeBasamakKodu { get; set; }

        [XmlElement]
        public string uygulanmisGecikmeSurprimYuzde { get; set; }

        [XmlElement]
        public string uygulanmisIlPlakaIndirimYuzde { get; set; }

        [XmlElement]
        public string uygulanmisIndirimYuzde { get; set; }

        [XmlElement]
        public string uygulanmisSurprimYuzde { get; set; }

        [XmlElement]
        public string uygulanmisTarifeBasamakKodu { get; set; }


    }

}
