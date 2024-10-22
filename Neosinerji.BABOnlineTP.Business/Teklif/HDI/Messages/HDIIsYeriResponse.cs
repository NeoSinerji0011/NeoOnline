using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIIsYeriResponse
    {
        public string Durum { get; set; }
        public string DurumMesaj { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string HDIMusteriNo { get; set; }
        public string tcKmAd { get; set; }
        public string tcKmSyAd { get; set; }
        public string WSNPRM { get; set; }
        public string WSFON { get; set; }
        public string WSVERG { get; set; }
        public string WSBRUT { get; set; }

        //Sistem ön tanımlı ANA TEMİNATLAR alttaki TAG ile listelenir
        public List<ANATeminat> ANATeminatListe { get; set; }

        //Sistem ön tanımlı Ek teminatlarlar alttaki TAG ile listelenir
        public List<EKTeminat> EkTeminatListe { get; set; }


        public List<Asansor> Asansorler { get; set; }

        public string XPIFKD { get; set; }

        //Özel Fiyat Girilecek Teminatlar Listelenir
        public List<WSF03> OzelFiyatListe { get; set; }

        //OTORİZASYONA DÜŞTÜĞÜNEDE AŞAĞIDAKİ TAG DÖNER
        public List<Sebep> OTORIZASYONLISTE { get; set; }


        //KLOZ İSTENDİĞİNDEN AŞAĞIDAKİ TAG DÖNECEKTİR
        public List<Kloz> KLOZLISTE { get; set; }

        public List<OdemeVadeTutar> Taksitler { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class ANATeminat
    {
        public string ANAWSSIK2 { get; set; }
        public string ANAWSETK2 { get; set; }
        public string ANAWSTEA2 { get; set; }
        public string ANAWSTEM2 { get; set; }

    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class EKTeminat
    {
        public string WSSIK2 { get; set; }
        public string WSETK2 { get; set; }
        public string WSTEA2 { get; set; }
        public string WSTEM2 { get; set; }
        public string SecimProtect { get; set; }
        public string BedelProtect { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class Asansor
    {
        public string WSSIK { get; set; }
        public string WSSAH4 { get; set; }
        public string WSBKZ4 { get; set; }
        public string WSMKZ4 { get; set; }
        public string WSPRM4 { get; set; }
        public string WSKD4 { get; set; }
    }
    //[Serializable]
    //[XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    //public class WSF03
    //{
    //    public string WSETK3 { get; set; }
    //    public string WSTE13 { get; set; }
    //    public string WSTEM3 { get; set; }
    //    public string WSFYT3 { get; set; }
    //    public string Protec { get; set; }
    //    public string WSPRM { get; set; }
    //    public string WSOFY3 { get; set; }
    //    public string WSDR3 { get; set; }
    //    public string WSKD3 { get; set; }
    //    public string WSBRA3 { get; set; }
    //    public string WSTRA3 { get; set; }
    //    public string WSTRTR { get; set; }

    //}
    
    
    
}

