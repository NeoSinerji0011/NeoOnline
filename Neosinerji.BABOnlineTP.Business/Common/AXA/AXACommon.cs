using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class AXA_UrunKodlari
    {
        public const string Trafik = "TR1";
        public const string Kasko = "KAS";
        public const string IsYeri = "İŞY";
        public const string Konut = "KNT";

    }

    public class AXA_IslemTipleri
    {
        public const decimal Police = 1;
        public const decimal MebdeindenIptal = 13;

    }

    public class AXA_TrafikMatbuList
    {
        public const string MotorNo = "MOTOR NO";
        public const string SasiNo = "ŞASİ NO";
        public const string YolcuYerAdedi = "YOLCU YER ADEDİ";
        public const string TrafigeCikisTarihi = "TRAFİĞE ÇIKIŞ TARİHİ";
        public const string TrafikTescilTarihi = "TRAFİK TESCİL TARİHİ";
        public const string SilindirHacmi = "SİLİNDİR HACMİ";
        public const string MotorHacmi = "MOTOR HACMİ";
        public const string Renk = "RENGİ";
        public const string PlakaNo = "PLAKA NO (TRAFİK)";

        public const string EskiAcenteNo = "ESKİ ACENTE NO";
        public const string EskiPoliceNo = "ESKİ POLİÇE NO";
        public const string EskiYenilemeNo = "ESKİ YENİLEME NO";
        public const string TramerNo = "SBM TRAMER NO";
        public const string TramerBelgeTarihi = "TRAMER BELGE TARİHİ";
        public const string RuhsatBelgeNumarasi = "RUHSAT BELGE/ASBIS NUMARASI";
        public const string EskiPlakaNo = "ESKI PLAKA NO";
        public const string TSPoliceNo = "TS POLİÇE NO";
        public const string TSAcenteNo = "TS ACENTE NO";
        public const string TSYenilemeNo = "TS YENİLEME NO";


    }
    public class AXA_TrafikSoruKodlari
    {
        public const string KullanimTarzi = "TRL";
        public const string KullanimSekli = "KŞK";
        public const string Marka = "MRK";
        public const string MarkaTip = "MRT";
        public const string Model = "MDL";
        public const string PlakaIlKodu = "PL6";
        public const string ImalatYeri = "İML";
        public const string PlakaYeniKayitMi = "YPL";
        public const string OdemeSekli = "PV5";
        public const string OdemeTipi = "KKK";

        public const string TarifeBasamagi = "TB1";
        public const string EskiSigortaSirketi = "EŞ6";
        public const string HasarsizlikIndirimi = "Tİ3";
        public const string HasarsizlikSurprimi = "S0S";

    }

    public class AXA_KaskoSoruKodlari
    {
        public const string KullanimTarzi = "TRL";
        public const string KullanimSekli = "KLŞ";
        public const string Marka = "MRK";
        public const string MarkaTip = "MRT";
        public const string Model = "MD3";
        public const string MarkaKasko = "RNK";
        public const string OnarimSecimi = "OTY";
        public const string PlakaYeniKayitMi = "YPL";
        public const string PlakaIlKodu = "PLK";
        public const string YeniDegerKlozu = "İK5";
        public const string DepremSelKoasuransi = "DS8";
        public const string MuafiyetTutari = "MO0";
        public const string OdemeSekli = "PV4";
        public const string CamFilmiLogoVarMi = "CMF";
        public const string DainiMurtein = "DMÜ";
        public const string SigortaliListesi = "LİS";
        public const string EngelliAraciMi = "OAC";
        public const string SorumlulukLimiti = "SOR";
        public const string DigerAksesuar = "Dİ1";
        public const string KasaTank = "KT1";
        public const string EmtiaCinsi = "EM2";
        public const string YurtDisiSuresi = "YÇS";
        public const string IMS3lü = "M1";
        public const string IMSKombine = "M2";
        public const string ManeviTazminat = "MNV";
        public const string AsistansSecimi = "AYR";
        public const string KiralikAracSuresi = "YRX";
        public const string HayatTeminatLimiti = "KHY";
        public const string HasarsizlikIndirimi = "İHK";
        public const string FaliyetKodu = "FK3";


    }

    public class AXA_TrafikTeminatlar
    {
        public const string Trafik = "TR0";
        public const string KisiBasiSakatlanmaOlum = "TR1";
        public const string KazaBasiSakatlanmaOlum = "TR2";
        public const string KisiBasiTedaviMasraflari = "TR3";
        public const string KazaBasiTedaviMasraflari = "TR4";
        public const string AracBasinaMaddiZaralar = "TR6";
        public const string KazaBasinaMaddiZaralar = "TR5";
        public const string DegerKaybi = "TR7";
    }

    public class AXA_OdemeTipleri
    {
        public const string KrediKartiTaksitli = "1";
        public const string KrediKartiPesin = "2";
        public const string Nakit = "0";
    }

    public class AXA_OdemeSekilleri
    {
        public const string Bir = "1";
        public const string Iki = "2";
        public const string Uc = "3";
        public const string Dort = "4";
        public const string Bes = "5";
        public const string Alti = "6";
        public const string Yedi = "7";
        public const string Pesin = "0";
    }
    public class AXA_IkameSecenekleri
    {
        public const string Uc = "3";
        public const string Yedi = "7";
        public const string On = "10";
        public const string OnBes = "15";
        public const string OnBesLüxArac = "IIS";
    }

    public class AXA_ImalatYerleri
    {
        public const string Yerli = "0";
        public const string Yabanci = "1";
    }

    public class AXA_KayitTipleri
    {
        public const string Evet = "1";
        public const string Hayir = "0";
    }

    public class AXA_KullanimSekilleri
    {
        public const string Ozel = "0";
        public const string Resmi = "1";
        public const string Ticari = "2";
    }

    public class AXA_KaskoTeminatlar
    {
        public const string Arac = "KAS";                   //Zorunlu
        public const string AracaBagliKaravan = "ÇBK";      //Seçimli
        public const string SesGoruntuCihazlari = "RT1";    //Seçimli
        public const string DigerAksesuarlar = "Dİ1";       //Seçimli
        public const string KasaTank = "KT1";               //Seçimli
        public const string TasinanYuk = "TA1";             //Seçimli
        public const string KullanimGelirKaybi = "KG1";     //Seçimli
        public const string YurtDisiKasko = "YU1";          //Seçimli
        public const string AnahtarKaybiZararlari = "ANY";  //Zorunlu
        public const string IMS3Limit = "İM1";              //Seçimli-Default
        public const string IMSKombine = "İM2";             //Seçimli
        public const string FerdiKaza = "FK0";              //Zorunlu
        public const string FerdiKazaOlum = "FK1";              //Zorunlu
        public const string FerdiKazaSurekliSakatlik = "FK2";              //Zorunlu
        public const string TedaviMasraflari = "FK3";       //Seçimli  
        public const string ElektrikliAracBedeli = "ELK";   //Seçimli  
        public const string HayatTeminati = "KHY";          //Seçimli  
        public const string HukuksalKoruma = "MA2";         //Zorunlu  
        public const string Asistans = "YR1";               //Zorunlu
        public const string BedeniZararKazaBasi = "KB2";
        public const string KazaBasMaddiZarar = "KM2";
        public const string BedeniZararSahisBasi = "SB1";
        public const string OlayBasiToplamLimit = "TPL";
        public const string HukuksalKorumaMotorluArac = "MA2";
        public const string HukuksalKorumaSurucu = "SÜ2";
        public const string AxaAcilYardim = "YR1";
        public const string AracCekmeHizmeti = "YR2";
        public const string AracCekmeKurtarma = "YR3";
        public const string KaskoReasuransLimitKontrolu = "BED";    

    }

    public class AXA_HasarsizlikIndirimleri
    {
        public const string Bir = "0";
        public const string Iki = "30";
        public const string Uc = "40";
        public const string Dort = "50";
        public const string Bes = "60";
    }

    public static class AxaHayatLimiti
    {
        public const decimal BesBin = 5000;
        public const decimal YediBinBesYuz = 7500;
    }

}
