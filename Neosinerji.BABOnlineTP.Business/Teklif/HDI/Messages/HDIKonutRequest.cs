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
    public class HDIKonutRequest : HDIMessage
    {
        public string user { get; set; }
        public string pwd { get; set; }
        public string Uygulama { get; set; }
        public string Refno { get; set; }
        public string IstekTip { get; set; }
        public string OrjinalRefNo { get; set; }
        public string HDIPoliceNumara { get; set; }
        public string HDIYenilemeNo { get; set; }
        public string IptalNedeni { get; set; }
        public string Tarih { get; set; }


        public string OzelTuzel { get; set; }
        public string MSCnsTp { get; set; }
        public string MSDogYL { get; set; }
        public string MSEgKd { get; set; }
        public string MSMesKd { get; set; }
        public string MSMedKd { get; set; }
        public string MSCocKd { get; set; }
        public string MSEhlYL { get; set; }
        public string MSSekKd { get; set; }
        public string TcKimlikNo { get; set; }
        public string VergiKimlikNo { get; set; }
        public string Uyruk { get; set; }
        public string YabanciKimlikNo { get; set; }
        public string PasaportNo { get; set; }
        public string LMUSNO { get; set; }
        public string IsTel1 { get; set; }
        public string IsTel2 { get; set; }
        public string EvTel1 { get; set; }
        public string EvTel2 { get; set; }
        public string Adres { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Semt { get; set; }
        public string KoyMahalle { get; set; }
        public string BinaNo { get; set; }
        public string HanApartmanAd { get; set; }
        public string Daire { get; set; }
        public string Kat { get; set; }
        public string PostaKod { get; set; }
        public string Ilce { get; set; }
        public string IlKod { get; set; }

        public string OdemeTipi { get; set; }
        public string TaksitSayisi { get; set; }
        public string TckR { get; set; }
        public string SubeKod { get; set; }
        public string TemsilciNo { get; set; }
        public string TemsilciAd { get; set; }
        public string TemsilciSoyAd { get; set; }


        //GENEL (WSR01)
        public string WSDVCN { get; set; }
        public string WSDVKR { get; set; }
        public string WSFRNT { get; set; }
        public string WSSIGE { get; set; }
        public string WSKIS { get; set; }
        public string WSVER { get; set; }
        public string WSOS { get; set; }
        public string WSIND { get; set; }
        public string WSDAIN { get; set; }
        public string WSDAMU { get; set; }
        public string WSRIZ { get; set; }
        public string WSYVER { get; set; }
        public string WSDSEH { get; set; }
        public string WSYAPT { get; set; }
        public string WSHAS { get; set; }
        public string WSSAG { get; set; }

        // SAĞLIK KİŞİLERİ (SF07)
        public List<WSF07> SKS { get; set; }
        public string WSMETK { get; set; } //Daire Metrekaresi                    
        public string WSELST { get; set; } //Değerli Eşya Liste (E/H) (Değerli eşya için)        


        // RİZİKO ADRES (WSP05 )    (WSRIZ 'H' ise Riziko ve müşteri adresi farklıysa istenir)
        public string WSADR { get; set; }
        public string WSKOY { get; set; }
        public string WSCAD { get; set; }
        public string WSSOK { get; set; }
        public string WSHTIP { get; set; }
        public string WSHAN { get; set; }
        public string WSBINA { get; set; }
        public string WSDRNO { get; set; }
        public string WSKAT { get; set; }
        public string WSPOSK { get; set; }
        public string WSSEMT { get; set; }
        public string WSILCK { get; set; }
        public string WSILK { get; set; }


        //ANA TEMİNAT BEDELLERİ (SF01)
        public string ATBINA { get; set; } //Sigorta Kapsamı BİNA (YANGIN) Bedel girildiğinde teminat eklenir

        public string ATESYA { get; set; } //Sigorta Kapsamı EŞYA (YANGIN) Bedel girildiğinde teminat eklenir

        public string ATDGES { get; set; } //Sigorta Kapsamı DEĞERLİ EŞYA (YANGIN) Bedel girildiğinde teminat eklenir (Değerli Eşya Liste.E ise girilir)

        public string ATTEMEL { get; set; } //Sigorta Kapsamı TEMELLER (YANGIN) (Tek başına seçilemez) Bedel girildiğinde teminat eklenir



        // EK TEMİNAT BEDELLERİ/SEÇİMLERİ (SF02)
        public List<S2> EKTM { get; set; }

        //SORULAR CEVAPLAR (WSR04)
        public string WSENF { get; set; }
        public string WSBEL { get; set; }
        public string WSMUD { get; set; }
        public string WSMUBD { get; set; }
        public string WSMUMD { get; set; }
        public string WSRSKD { get; set; }
        public string WSPLNO { get; set; }
        public string WSCKAP { get; set; }
        public string WSDPAR { get; set; }
        public string WSOGUV { get; set; }
        public string WSYKKAT { get; set; }
        public string WSDGIN { get; set; }
        public string WSFBAS { get; set; }
        public string WSSUR { get; set; }


        //WSF03 Özel fiyat giriş alanları
        public List<S3> OFY { get; set; }


        // POLİÇE NOTLARI
        public List<S5> NTS { get; set; }
    }


    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class WSF07 : HDIMessage
    {
        public string KWSAD { get; set; }
        public string KWSSY { get; set; }
        public string KWSDGTR { get; set; }
        public string KWSCN { get; set; }
        public string KWSYDRC { get; set; }
    }



    //[Serializable]
    //[XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    //public class S2 : HDIMessage
    //{
    //    public string S2KD { get; set; }
    //    public string S2SC { get; set; }
    //    public string S2BD { get; set; }
    //}


    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class S3 : HDIMessage
    {
        public string OFTKD { get; set; }
        public string OFFYT { get; set; }

    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class NTS : HDIMessage
    {
        public List<S5> S5 { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class S5 : HDIMessage
    {
        public string TX { get; set; }
    }
}


