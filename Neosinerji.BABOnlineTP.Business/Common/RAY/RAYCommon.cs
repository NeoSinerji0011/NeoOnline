using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common.RAY
{
    public class RAY_ProcesTipleri
    {
        public const string MusteriDataBaseSorgula = "101";
        public const string MusteriTramerSorgula = "100";
        public const string MusteriKaydetme = "102";
        public const string EGMSorgu = "103";
        public const string TeklifKayit = "54";
        public const string Policelestirme = "54";
        public const string PolicePDF = "55";
    }

    public class RAY_PDFBasimTipleri
    {
        public const string PolicePDF = "1";
        public const string DekontPDF = "2";

    }

    public class RAY_TrafikSoruTipleri
    {
        public const string AracMarkaKodu = "5073";
        public const string KullanimSekli = "5066";
        public const string KullanimTarzi = "5000";
        public const string Model = "5004";
        public const string MotorNo = "5010";
        public const string SasiNo = "5011";
        public const string TescilBelgeNo = "5142";
        public const string TescilBelgeSeriKod = "5141";
        public const string OncekiAcenteNo = "17";
        public const string OncekiPoliceNo = "18";
        public const string OncekiSirketKodu = "15";
        public const string OncekiYenilemeNo = "19";
        public const string ASBISMI= "10150";
        public const string YeniIsletenMi = "7024";
        public const string AracGrubuIsMakinesiMi = "5169";
        public const string AracMarka = "5002";
        public const string Tip = "5003";
        public const string YerliYabanci = "5013";
        public const string TrafikTescilTarihi = "5017";
        public const string OncekiPoliceMotorNo = "5145";
        public const string OncekiPoliceSasiNo = "5146";

    }

    public class RAY_TrafikResponseSoruKodlari
    {
        public const string GecikmeOrani = "588";
        public const string HasarsizlikOrani = "522";
        public const string ZKYTMSIndirimi = "524";      

    }

    public class RAY_AracKullanimSeklilleri
    {
        public const string Ozel = "1";
        public const string Resmi = "2";
        public const string Ticari = "3";
        public const string UzunSureliAracKiralama = "4";

    }

    public class RAY_AdresTipleri
    {
        public const string Ulke = "UL";
        public const string Il = "İL";
        public const string Ilce = "İÇ";
        public const string Cadde = "CD";
        public const string Mahalle = "MH";
        public const string Sokak = "SK";
        public const string Bina = "BN";
        public const string PostaKodu = "PK";
    }

    public class RAY_UlkeKodlari
    {
        public const string Turkiye = "TÜRKİYE";
        public const string Yabanci = "YABANCI";
     
    }

    public class RAY_TrafikKesitiTipKodlari
    {
        public const string Vergiler = "1";
        public const string Komisyonlar = "2";        

    }

    public class RAY_TrafikVergiKesintiKodlari
    {
        public const string GiderVergisi = "1";
        public const string GuvenceHesabi = "3";
        public const string THGF = "4";
    }

    public class RAY_TrafikKomisyonKesintiKodlari
    {
        public const string AcenteKomisyonu = "1";
        public const string KaynakKomisyonu = "2";
        public const string TaliAcenteKomisyonu = "3";
        public const string KomisyonerKomisyonu = "4";
    }

    public class RAY_TrafikTeminatlar
    {
        public const string ZorunluMaliSorumluluk = "7000";
        public const string MaddiAracBasina = "7022";
        public const string MaddiKazaBasina = "7021";
        public const string SaglikGideriKisiBasina = "7019";
        public const string SaglikGideriKazaBasina = "7020";
        public const string OlumSakatlikKisiBasina = "7017";
        public const string OlumSakatlikKazaBasina = "7018";
    }

    public class RAY_KaskoTeminatlar
    {
        public const string Kasko = "5000";
        public const string LPGTuru = "5008";
        public const string DigerEkCihazlar = "5060";
        public const string SesGoruntuIletisimCihazlari = "5061";
        public const string KasaTankBedeli = "5011";
        public const string TasinanEmtea = "5009";
        public const string EskimePayiDusmeme = "5016";
        public const string DepremYanardagPuskurme = "5021";
        public const string SelveSuBasmasi = "5020";
        public const string GLKHHT = "5019";
        public const string AnahtarlaAracCalinmasi = "5062";
        public const string HasarsizlikIndirimKoruma = "5017";
        public const string YUKKaymasiZaralari = "5053";
        public const string YurtDisiKasko = "5022";
        public const string KullanimGelirZararlari = "5010";
        public const string SigaraMaddeZararlari = "5028";
        public const string CekmeCekilmeZararlari = "5029";
        public const string AnahtarKaybi = "5063";
        public const string PatlayiciMaddeZararlari = "5064";
        public const string KemirgenHayvanZararlari = "5065";
        public const string IMM = "5025";
        public const string OlumSakatlikKisiBasina = "7017";
        public const string OlumSakatlikKazaBasina = "7018";
        public const string MaddiKazaBasina = "7021";
        public const string MaddiBedeniAyrimsiz = "7026";
        public const string ManeviTazminatTalepleri = "5048";
        public const string FerdiKazaKoltuk = "5026";
        public const string Vefat = "1042";
        public const string SurekliSakatlik = "1043";
        public const string TedaviGiderleri = "1044";
        public const string HukuksalKoruma = "5018";
        public const string HukuksalKorumaMotorluAracaBagli = "5043";
        public const string HukuksalKorumaSurucuyeBagli = "5044";
        public const string IkameAracHizmeti15Gun = "5055";
        public const string RAYKulupHizmetleri = "5024";
        public const string RAYOtoEstetikOnarim = "5704";
    }

    public class RAY_KaskoSorular
    {
        public const string UrunSecimi = "360";
        public const string OnarimYapilacakServis = "275";
        public const string Meslek = "333";
        public const string PersonelMusteriGrubu = "5061";
        public const string OncekiSirketKodu = "15";
        public const string OncekiAcenteKodu = "17";
        public const string OncekiPolicePolice = "18";
        public const string OncekiYenilemeNo = "19";
        public const string TescilBelgeSeriKod = "5141";
        public const string TescilBelgeSeriNoAsbis = "5142";
        public const string ModelYili = "5004";
        public const string AracMarkaKodu = "5073";
        public const string KullanimTarzi = "5000";
        public const string TarifeSinifi = "5001";
        public const string KullanimSekli = "5066";
        public const string AracBedeli = "5005";
        public const string MotorNo = "5010";
        public const string SasiNo = "5011";
        public const string YolcuAdedi = "5006";
        public const string SurucuAdedi = "5007";
        public const string GorevliAdedi = "5008";
        public const string IMMKademeNo = "5036";
        public const string FerdiKazaKoltuk = "5040";
        public const string TedaviMasraflari = "5043";
        public const string DainiMurteinVarmi = "1990";
        public const string DainiMurteinTCKimlikNo = "5383";
        public const string DainiMurteinVergiNo = "5384";
        public const string TekSurucuIndirim = "5117";
        public const string EnAz5YillikEhliyetIndirimi = "5116";
        public const string SesliAlarm = "381";
        public const string SehirIciIndirimi = "5028";
        public const string SurucuKursuEkPrimi = "342";
        public const string DamperVarmi = "5032"; //Araçta Damper var mı? Damperli Römork Çekiyor mu?
        public const string LPGMi = "5030";
        public const string DepremMuafiyeti = "5111";
        public const string SelMuafiyeti = "5112";
        public const string KaskoMuafiyeti = "5122";
        public const string EkPrimOrani = "341";
        public const string TasinanEmteaKodu = "5049";
        public const string YurtDisindaKalmaSuresi = "5033";
        public const string YurtDisinaCikisTarihi = "5034";
        public const string YurtDisiTeminatiBitisTarihi = "5035";
    }

    public class RAY_Urunler
    {
        public const string BirlesikKasko = "1";
        public const string MeslegeOzelKasko = "2";
        public const string GrupKasko = "3";
        public const string PrensesKasko = "5";
        public const string FiloKasko = "7";

    }

    public class RAY_UrunKodlari
    {
       // public const string Trafik = "550"; Eski Trafik Ürünü Kodu
        public const string Trafik = "555";
        public const string Kasko = "525";

    }

    public class RAY_KaskoResponseSoruKodlari
    {
        public const string HasarsizlikIndirimOrani = "376";
        public const string HasarsizlikArttirimOrani = "377";

    }
}
