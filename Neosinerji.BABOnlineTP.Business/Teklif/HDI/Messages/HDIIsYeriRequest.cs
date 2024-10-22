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
    public class HDIIsYeriRequest : HDIMessage
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

        //Adres Bilgileri
        public AdresBilgi AdresBilgi { get; set; }

        public string OdemeTipi { get; set; }
        public string TaksitSayisi { get; set; }
        public string TckR { get; set; }
        public string SubeKod { get; set; }
        public string TemsilciNo { get; set; }
        public string TemsilciAd { get; set; }
        public string TemsilciSoyAd { get; set; }
        public string XDOTDT { get; set; }
        public string WSSTC { get; set; }

        //GENEL (WSR01)
        public string WSYAPT { get; set; }
        public string WSISTK { get; set; }
        public string WSOS { get; set; }
        public string WSVER { get; set; }
        public string WSDVCN { get; set; }
        public string WSIND { get; set; }
        public string WSYVER { get; set; }
        public string WSDVKR { get; set; }
        public string WSSAG { get; set; }
        public string WSFRNT { get; set; }
        public string WSDSEH { get; set; }
        public string WSMETK { get; set; }
        public string WSASAN { get; set; }
        public string WSKSSY { get; set; }
        public string WSKLIM { get; set; }
        public string WSRIZ { get; set; }
        public string WSDAIN { get; set; }
        public string WSDAMU { get; set; }
        public string WSSIGE { get; set; }



        // SAĞLIK KİŞİLERİ (SF07)
        public List<WSF07> SKS { get; set; }

        //TEMİNAT VE EK TEMİNATLAR BEDELLERİ/SEÇİMLERİ (SF04 ve SF05)
        public List<S2> EKTM { get; set; }

        // public string WSELST { get; set; } //Değerli Eşya Liste (E/H) (Değerli eşya için)  

        //RİZİKO ADRES (WSP09 )    (WSRIZ 'H' ise Riziko ve müşteri adresi farklıysa istenir)
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


        //SORULAR CEVAPLAR (WSR04)
        public string WSENF { get; set; }
        public string WSBEL { get; set; }
        public string WSCAT { get; set; }
        public string WSKKAT { get; set; }
        public string WSPLET { get; set; }
        public string WSKIRB { get; set; }
        public string WSKIRA { get; set; }
        public string WSPLTH { get; set; }
        public string WSKEP { get; set; }
        public string WSPSAJ { get; set; }
        public string WSALRM { get; set; }
        public string WSDGIN { get; set; }
        public string WSBEKC { get; set; }
        public string WSHMUA { get; set; }
        public string WSKAME { get; set; }
        public string WSTCAM { get; set; }
        public string WSMUD { get; set; }
        public string WSMUBD { get; set; }
        public string WSMUMD { get; set; }
        public string WSMSMD { get; set; }
        public string WSMSD { get; set; }
        public string WSRSKD { get; set; }
        public string WSPLNO { get; set; }
        public string WSFBAS { get; set; }
        public string WSSUR { get; set; }



        //WSF03 Özel fiyat giriş alanları
        public List<S3> OFY { get; set; }

        //POLİÇE NOTLARI
        public List<S5> NTS { get; set; }

        //ASANSÖR BİLGİLER XPACEN 0 Olduğu durumlarda seçim alınacak
        public string ASBZSBA { get; set; }
        public string ASBZKBA { get; set; }
        public string ASMZKBA { get; set; }
        public string ASPRM4 { get; set; }
        public string ASWSKD4 { get; set; }
        public string XPIFKD { get; set; }
        public string W2UCTP { get; set; }
        public string WSPRSA { get; set; }


    }


    //TEMİNAT VE EK TEMİNATLAR BEDELLERİ/SEÇİMLERİ (SF04 ve SF05)
    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class S2 : HDIMessage
    {
        public S2()
        {
            S2BD = "";
            S2KD = "";
            S2SC = "";
        }
        public string S2KD { get; set; }
        public string S2SC { get; set; }
        public string S2BD { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class AdresBilgi
    {
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
    }


}






