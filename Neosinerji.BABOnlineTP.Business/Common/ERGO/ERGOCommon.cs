using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class ERGOKimlikTipleri
    {
        public const short TC = 1;
        public const short Vergi = 2;
        public const short Pasaport = 3;
        public const short Yabanci = 4;
    }

    public class ERGOPrimTipleri
    {
        public const string GunEsasi = "1";
        public const string KisaVade = "2";
    }

    public class ERGOOdemeTipleri
    {
        public const short KrediKarti = 1;
        public const short BlokeKart = 2;
        public const short Cek = 3;
        public const short Senet = 4;
        public const short Nakit = 5;
        public const short Havale = 6;
    }

    public class ERGOMusteriTipleri
    {
        public const short Sigortali = 1;
        public const short SigortaEttiren = 2;
    }

    public class ERGOAdresTipleri
    {
        public const string Ev = "1";
        public const string Is = "2";
        public const string Diger = "3";
        public const string Iletisim = "4";
    }

    public class ERGOAcenteTipleri
    {
        public const string Acente = "1";
        public const string Broker = "2";
    }

    public class ERGOTelefonTipleri
    {
        public const string Ev = "1";
        public const string Is = "2";
        public const string Iletisim = "3";
        public const string Fax = "4";
        public const string Cep = "5";

    }
    public class ERGOUrunKodlari
    {
        public const short Trafik = 310;
        public const short Kasko = 344;
    }

    public class ERGOVergiTipleri
    {
        public const short GelistirmeFonu = 4;
        public const short GarantiFonu = 2;
        public const short GiderVerigisi = 1;
    }

    public class ERGOTrafikSorular
    {
        public const string HukuksalKorumaSurucu = "@hukuksalKorumaSurucu";
        public const string HukuksalKorumaArac = "@hukuksalKorumaArac";
        public const string AracKullanimTarzi = "@aracKullanimTarzi";
        public const string AracAltKullanimTarzi = "@aracAltKullanimTarzi";
        public const string AracTip = "@aracTip";
        public const string AracModelYili = "@aracModelYili";
        public const string AracBirlikKod = "@aracBirlikKod";
        public const string AracKoltukAdet = "@aracKoltukAdet";
        public const string Plaka1 = "@plaka1";
        public const string Plaka2 = "@plaka2";
        public const string Plaka3 = "@plaka3";
        public const string AracMotorNo = "@aracMotorNo";
        public const string AracSasiNo = "@aracSasiNo";
        public const string AracTescilTarihi = "@aracTescilTarihi";
        public const string TescilYeri = "@tescilYeri";
        public const string AracRenk = "@aracRenk";
        public const string TrafikReferansAcenteNo = "@trafikReferansAcenteNo";
        public const string TrafikReferansBaslangicTarihi = "@trafikReferansBaslangicTarihi";
        public const string TrafikReferansBitisTarihi = "@trafikReferansBitisTarihi";
        public const string TrafikReferansPoliceHasarSayisi = "@trafikReferansPoliceHasarSayisi";
        public const string TrafikReferansPoliceNo = "@trafikReferansPoliceNo";
        public const string TrafikReferansSirketKodu = "@trafikReferansSirketKodu";
        public const string TrafikReferansPoliceYenilemeNo = "@trafikReferansPoliceYenilemeNo";

    }

    public class ERGOKaskoBenefitList //Menfaat Listesi
    {
        //Zorunlu Menfaat
        public const string hukuksalKorumaSurucu = "@hukuksalKorumaSurucu";
        public const string hukuksalKorumaArac = "@hukuksalKorumaArac";
        public const string tefriksizBedeniMaddiKazaBasi = "@tefriksizBedeniMaddiKazaBasi";
        public const string olumSurekliSakatlikYolcu = "@olumSurekliSakatlikYolcu";
        public const string olumSurekliSakatlikSurucu = "@olumSurekliSakatlikSurucu";
        public const string tedaviMasraflariYolcu = "@tedaviMasraflariYolcu";
        public const string tedaviMasraflariSurucu = "@tedaviMasraflariSurucu";

        //Ana Menfaat
        public const string arac = "@arac";

        //Seçimli Menfaat
        public const string anahtar = "@anahtar";
        public const string yuk = "@yuk";
        public const string kasa = "@kasa";
        public const string bireyselSorumluluk = "@bireyselSorumluluk";
        public const string lpgTanki = "@lpgTanki";
        public const string yuk3kisi = "@yuk3kisi";
        public const string alarm = "@alarm";
        public const string aracTelefonu = "@aracTelefonu";
        public const string celikJant = "@celikJant";
        public const string deriDoseme = "@deriDoseme";
        public const string dvdVcdOynatici = "@dvdVcdOynatici";
        public const string klima = "@klima";
        public const string kolon = "@kolon";
        public const string ozelLastik = "@ozelLastik";
        public const string parkSensoru = "@parkSensoru";
        public const string radyoTeyp = "@radyoTeyp";
        public const string spoiler = "@spoiler";
        public const string televizyon = "@televizyon";
        public const string telsiz = "@telsiz";
        public const string xenonFar = "@xenonFar";
        public const string digerAksesuar = "@digerAksesuar";
        public const string ucuncuSahisBedeniZararKisiBasi = "@ucuncuSahisBedeniZararKisiBasi";
        public const string ucuncuSahisBedeniZararKazaBasi = "@ucuncuSahisBedeniZararKazaBasi";
        public const string ucuncuSahisMaddiZararKazaBasi = "@ucuncuSahisMaddiZararKazaBasi";
        public const string taksimetre = "@taksimetre";
        public const string gap = "@gap";
        public const string isDurmasi = "@isDurmasi";

        //Varsayılan Seçimli Menfaat
        public const string acilSaglik = "@acilSaglik";
        public const string ozelEsya = "@ozelEsya";
        // public const string anahtar = "@anahtar";
    }

    public class ERGOCoverageList
    {
        //Zorunlu Teminatlar
        public const string maddi = "@maddi";
        public const string sakatlanmaVeOlum = "@sakatlanmaVeOlum";
        public const string asistans = "@asistans";
        public const string tefriksiz = "@tefriksiz";
        public const string ucuncuSahisMaddiZarar = "@ucuncuSahisMaddiZarar";
        public const string ucuncuSahisBedeniZarar = "@ucuncuSahisBedeniZarar";
        public const string tedaviMasraflari = "@tedaviMasraflari";
        public const string olumsurekliSakatlik = "@olumsurekliSakatlik";
        public const string acilSaglik = "@acilSaglik";
        public const string yukKasko = "@yukKasko";
        public const string hukuksalKoruma = "@hukuksalKoruma";
        public const string yuk3Kisi = "@yuk3Kisi";
        public const string saglikGideri = "@saglikGideri";
        public const string GLKHHKNHT = "@GLKHHKNHT";
        public const string depremVeYanardag = "@depremVeYanardag";
        public const string ferdiKaza = "@ferdiKaza";
        public const string ihtiyariMaliSorumluluk = "@ihtiyariMaliSorumluluk";
        public const string bireyselSorumluluk = "@bireyselSorumluluk";
        public const string anahtarKaybi = "@anahtarKaybi";
        public const string kucukHasarOnarim = "@kucukHasarOnarim";
        public const string ozelEsya = "@ozelEsya";
        public const string kaskVeMotorKorumaKiyafetleri = "@kaskVeMotorKorumaKiyafetleri";

        //Ana Teminatlar        
        public const string kasko = "@kasko";

        // SEÇİMLİ TEMİNAT
        public const string asistansIkameAracYilda15Gun2Kez = "@asistansIkameAracYilda15Gun2Kez"; //ASİSTANS-İKAME ARAÇ (YILDA 15 GÜN/2 KEZ)
        public const string asistansYolcu = "@asistansYolcu";

        //VARSAYILAN SEÇİMLİ TEMİNAT
        public const string selSuBaskini = "@selVeSuBaskini";
        public const string asistansIkameAracYilda7Gun3Kez = "@asistansIkameAracYilda7Gun3Kez";//ASİSTANS-İKAME ARAÇ (YILDA 7 GÜN/3 KEZ)

    }

    public class ERGOKaskoDiscountList //Ek Teminatlar Listesi Request
    {
        public const string sadeceAnlasmaliOzelServisler = "@sadeceAnlasmaliOzelServisler";
        public const string tumAnlasmaliServislerMuafiyet = "@bedelMuafiyet";
        public const string tumAnlasmaliServisler = "@tumAnlasmaliServisler";
        public const string tumServisler = "@tumServisler";
        public const string depremMuafiyetsiz = "@depremMuafiyetsiz";
        public const string hasarsizlikKoruma = "@hasarsizlikKoruma";
        public const string yeniDeger1Yil = "@yeniDeger1Yil";
        public const string patlayiciParlayiciMadde = "@patlayiciParlayiciMadde";
        public const string yurtDisi = "@yurtDisi";
        public const string sigaraYanigi = "@sigaraYanigi";
        public const string anahtarlaCalinma = "@anahtarlaCalinma";
        public const string kiymetKazanma2 = "@kiymetKazanma2";
        public const string seylapMuafiyetsiz = "@seylapMuafiyetsiz";
        public const string yetkiliOlmayanCekme = "@yetkiliOlmayanCekme";
        public const string bedelMuafiyet = "@bedelMuafiyet";
        public const string meslekIndirimi = "@meslekIndirimi";
        public const string pesinOdeme = "@pesinOdeme";
        public const string muafiyetsizCam = "@muafiyetsizCam";
        

    }

    public class ERGOKaskoPropertyList //Property Listesi
    {
        public const string aracKullanimTarzi = "@aracKullanimTarzi";
        public const string aracAltKullanimTarzi = "@aracAltKullanimTarzi";
        public const string plaka1 = "@plaka1";
        public const string plaka2 = "@plaka2";
        public const string plaka3 = "@plaka3";
        public const string aracModelYili = "@aracModelYili";
        public const string aracBirlikKod = "@aracBirlikKod";
        public const string aracSasiNo = "@aracSasiNo";
        public const string aracMotorNo = "@aracMotorNo";
        public const string aracTescilTarihi = "@aracTescilTarihi";
        public const string aracKoltukAdet = "@aracKoltukAdet";
        public const string tescilBelgeSeriNo1 = "@tescilBelgeSeriNo1";
        public const string tescilBelgeSeriNo2 = "@tescilBelgeSeriNo2";
        public const string meslekBilgisi = "@meslekBilgisi";


        //public const string kasKoKampanyaVarMi = "@ergoBaskaKaskoKampanyaVarMi";
        //public const string aracSigortaBedel = "@aracSigortaBedel";        
        //public const string aracTip = "@aracTip";
        //public const string KaskoReferansAcenteNo = "@kaskoReferansAcenteNo";
        //public const string KaskoReferansPoliceNo = "@kaskoReferansPoliceNo";
        //public const string KaskoReferansSirketKodu = "@kaskoReferansSirketKod";
        //public const string KaskoReferansPoliceYenilemeNo = "@kaskoReferansPoliceYenilemeNo";
    }

    public class ERGOKaskoCoverageList //Ergo Kasko Teminatlar Listesi (Response)
    {
        public const short depremVeYanardag = 1001;
        public const short GLKHHKNHT = 1002;
        public const short selVeSuBaskini = 1003;
        public const short kucukHasarOnarim = 1004;
        public const short hukuksalKoruma = 1005;
        public const short kasko = 1006;
        public const short anahtarKaybi = 1010;
        public const short ozelEsya = 1011;
        public const short olumsurekliSakatlik = 1017;
        public const short tedaviMasraflari = 1018;
        public const short tefriksiz = 1021;
        public const short asistansIkameAracYilda15Gun2Kez = 1023;
        public const short asistansIkameAracYilda7Gun3Kez = 1022;
    }

    public class ERGOKaskoiscountSurchargeList //Ek Teminatlar Listesi Request
    {
        public const string kiymetKazanma2 = "@kiymetKazanma2";
        public const string enflasyon = "@enflasyon";
        public const string yetkiliOlmayanCekme = "@yetkiliOlmayanCekme";
        public const string aracYakit = "@aracYakit";
        public const string bolgeSurprim = "@bolgeSurprim";
        public const string bazGuncelleme = "@bazGuncelleme";

        public const string aracSinif = "@aracSinif";
        public const string surucuYas = "@surucuYas";
        public const string bonus = "@bonus";
        public const string hasarsizlik = "@hasarsizlik";
        public const string eskiModel = "@eskiModel";
        public const string aracKasaTip = "@aracKasaTip";

        public const string ilce = "@ilce";
        public const string ilPlakaKodu = "@ilPlakaKodu";
        public const string aracBedel = "@aracBedel";
        public const string aracMarka = "@aracMarka";
        public const string trafikKatsayisi = "@trafikKatsayisi";
        public const string sigaraYanigi = "@sigaraYanigi";

        public const string aracModel = "@aracModel";
        public const string musteriIndirimi = "@musteriIndirimi";
        public const string tumServisler = "@tumServisler";
        public const string depremMuafiyetsiz = "@depremMuafiyetsiz";
        public const string anahtarlaCalinma = "@anahtarlaCalinma";
        public const string seylapMuafiyetsiz = "@seylapMuafiyetsiz";
    }

    public class ErgoAltKullanimTarzlari
    {
        //Parametre ismindeki Rakamlar Koltuk Sayısını Belirtiyor
        public const string Kamyonet = "49";
        public const string Otomobil9Koltuk = "24";
        public const string OtobusDolmusHatli = "33";
        public const string Taksi = "26";
        public const string OzelKullanim1830 = "37";
        public const string Dolmus9Koltuk = "40";
        public const string OzelKullanim = "39";
        public const string Turizm1830 = "35";
        public const string DolmusHatli1017 = "38";
        public const string Sirket1830 = "36";
        public const string Sirket = "43";
        public const string Servis1830 = "34";
        public const string Servis = "41";
    }
}
