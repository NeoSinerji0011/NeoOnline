using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class GULF_KaskoSorular
    {
        // ***Yeni Kayıt ve İkinci El Yeni Araç Sorular
        public const string ModelYili = "883";
        public const string KullanimSekli = "196";
        public const string MarkaKodu = "882";
        public const string MotorNo = "185";
        public const string SasiNo = "190";

        // ***Ortak Sorular
        public const string FerdiKazaKoltukBasina = "1044";
        public const string IMMLimit = "2043";
        public const string YakitTuru = "12401";
        public const string KiralikAracSuresi = "1771";
        public const string MeslekIndirimi = "12525";

        // ***İkinci El Yeni Araç Ek Soru
        public const string TescilBelgeSeriNoAsbis = "11600";

        public const string TaksitSecenegi = "13510";
        public const string AlternatifKasko = "12406"; //Soru E veya H değerlerini alabilir.
        public const string HukuksalKoruma = "12535"; //1-5000 ve 2-10000 seçenekleri bulunmaktadır.



    }
    public class GULF_MusteriTipleri
    {
        public const string KimlikNo = "C";
        public const string PasaportNo = "I";
        public const string VergiNo = "T";
        public const string YabanciKimlikNo = "F";
    }
    public class GULF_KimlikTipleri
    {
        public const string TCKimlik = "1";
        public const string VergiKimlik = "2";
        public const string Pasaport = "3";
        public const string YabanciKimlik = "4";
        public const string Kimliksiz = "9";
    }
    public class GULF_OdemeTipleri
    {
        public const string Nakit = "NK";
        public const string KrediKarti = "TK";
        public const string OtomatikOdeme = "OO";
    }
    public class GULF_IkameTurleri
    {
        public const string YediGun = "7 Gün";
        public const string OndortGun = "14 Gün";
        public const string OtuzGunServisParcaDahil = "30 Gün (Servis ve Parça Dahil)";
    }
    public class GULF_KaskoTeminatlar
    {
        public const string AracTemianti = "60";
        public const string TasinanEmtia = "10025";
        public const string GLKHHT = "2600";
        public const string Deprem = "240";
        public const string Seylap = "230";
        public const string ArtanMaliSorumluluk = "180";
        public const string FerdiKaza = "185";
        public const string HukuksalKoruma = "4475";
        public const string Yardim = "4121";
        public const string AnahtarKaybi = "2117";
        public const string MedLine = "1475";
        public const string MiniOnarim = "9170";
        public const string HataliAkaryakitDolumu = "1254";      
    }
    public class GULF_Basimtipleri
    {
        public const string TeklifPolice = "1";
        public const string KrediKartiSlip = "19";
        public const string PoliceBilgilendirme = "6";
    }
}
