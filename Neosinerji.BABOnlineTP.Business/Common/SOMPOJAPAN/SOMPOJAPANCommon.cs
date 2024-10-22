using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common.SOMPOJAPANCommon
{
    public class SOMPOJAPAN_AdresKodlari
    {
        public const string Apartmani = "AP";
        public const string Beldesi = "BE";
        public const string Belediyesi = "BD";
        public const string BinaNo = "BN";
        public const string Blok = "BK";
        public const string Bolgesi = "BG";
        public const string Bulvari = "BL";
        public const string Cadde = "CD";
        public const string Cesmesi = "ÇŞ";
        public const string Cikmazi = "ÇK";
        public const string DaireNo = "DR";
        public const string Diger = "DG";
        public const string Evleri = "EV";
        public const string Han = "HN";
        public const string Hastansi = "HS";
        public const string Il = "İL";
        public const string Ilce = "İÇ";
        public const string IsHani = "İŞ";
        public const string KatNo = "KT";
        public const string Komutanligi = "KĞ";
        public const string Kooperatifi = "KO";
        public const string KoyBucak = "KKA";
        public const string Koyu = "KY";
        public const string KumeEvler = "KE";
        public const string Lojmani = "LJ";
        public const string Mahallesi = "MH";
        public const string Mevkii = "MV";
        public const string Okul = "OKL";
        public const string Pasaji = "PJ";
        public const string Plaza = "PL";
        public const string Semt = "SM";
        public const string Sitesi = "ST";
        public const string Sokak = "SK";
        public const string TicaretMerkezi = "TM";
    }

    public class SOMPOJAPAN_AracGrupKodlari
    {
        public const string Otomobil9="1";
        public const string Taksi = "2";
        public const string Kamyonet = "3";
        public const string Minibus1017 = "4";
        public const string IsMakinesi = "5";
        public const string Kamyon = "6";
        public const string Otobus1830 = "7";
        public const string Otobus31="8";
        public const string Motorsiklet = "12";
        public const string Traktor = "14";
        public const string Romork = "15";
        public const string Tanker = "16";
        public const string Cekici = "17";
        public const string Ozel = "18";
        public const string TarimMakinesi = "23";

    }

    public class SOMPOJAPAN_TrafikTeminatlar
    {
        public const int KisiBasinaTedaviMasraflari = 650;
        public const int KazaBasinaTedaviMasraflari = 651;
        public const int KisiBasiSakatOlum = 810;
        public const int KazaBasiSakatOlum = 820;
        public const int KazaBasinaMaddi = 930;
        public const int AracBasinaMaddi = 932;
       
    }

    public class SompoJapan_KaskoSoruTipleri
    {
        public const string MeslekKodu = "242";
        public const string KombineKisiKazaBasinaMi = "347";
        public const string IMMBedelSecimi = "348";
        public const string MarkaAdi = "170";
        public const string TrafigeCikisTarihi = "203"; // gg/aa/yyyy
        public const string KullanimKodu = "83";
        public const string Koltukadedi = "635";
        public const string AnahtarKaybiTeminatLimiti = "716";
        public const string FerdiKazaTedaviMasraflari = "T615";
        public const string FerdiKazaVefatTeminati = "T635";
        public const string FaaliyetKodu = "5600";
        public const string KasaTipi = "5601";
        public const string ArtiTeminatIstiyorMu = "848";
        public const string ArtiTeminatDegeri = "8148 ";
        public const string KasaTeminati = "750";
    }

    public class SompoJapan_TrafikSoruTipleri
    {
        public const string MarkaModel = "170";
        public const string TrafigeCikisTarihi = "1023"; // gg/aa/yyyy
        public const string TrafikGrupKodu = "174";
        public const string Koltukadedi = "200";
        public const string TescilBelgeKod = "941";
        public const string AsbisTescilSeriNo = "940";
    }

    public class SompoJapan_KaskoSoruCevapTipleri
    {
        public const string Kombine = "1";
        public const string KisiKazaBasina = "2";
        public const int Diger = 99;
    }

    public class SompoJapan_KaskoTeminatKodlari
    {
        public const int FerdiKazaTedaviMasraflari = 615;
        public const int FerdiKazaVefatTeminati = 635;
    }

    //Sompo Japan Trafik ve Kasko teklif WS lerinde araç tarife kodaları tutmuyor 
    //Trafik tarife kodalrı CR_KullanimTarzi tab. kayıtlı, Kasko tarife kodları burada
    public class SompoJapan_KaskoTarifeKodlari
    {
        public const string Otomobil = "1";
        public const string Minibus = "2";// Minibüs/Midibüs/Küçük Otobüs
        public const string Panelvan = "3";
        public const string Kamyonet = "4";
        public const string Traktor = "5";
        public const string Motorsiklet = "6";
        public const string Kamyon = "7";
        public const string BOtobus = "8";
        public const string Jeep = "10";
        public const string Romork = "12";
        public const string Cekici = "13";
        public const string IsMakinesi = "14";      
    }

    public class SOMPOJAPAN_KaskoTeminatlar
    {
        public const int Arac724Hizmet = 56;
        public const int CarpmaCarpilma = 410;
        public const int Yanma = 411;
        public const int Calinma = 412;
        public const int GLKHHT = 413;
        public const int SelBaskini = 415;

        public const int Deprem = 416;
        public const int AnahtarAracCalinma = 422;
        public const int HayvanZarari = 423;
        public const int SigaraBenzeri = 438;
        public const int CekmeCekilme = 439;
        public const int FK_Vefat = 635;

        public const int FK_SurekliSakat = 640;
        public const int HK_MotorluArac = 710;
        public const int HK_SurucuyeBagli = 715;
        public const int IMM_Sahis = 900;
        public const int IMM_Kaza = 901;
        public const int IMM_Maddi = 902;
        public const int IMM_Kombine = 672;

        public const int HasarsizlikKoruma = 906;
        public const int AnahtarKaybiDiger = 1050;
        public const int SompoJapanOzelOnarim = 2105;
        public const int YanlisAYakit = 2106;
      

    }
}
