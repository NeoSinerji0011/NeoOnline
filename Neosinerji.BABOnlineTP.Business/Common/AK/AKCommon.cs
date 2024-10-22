using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
   
    public class Ak_KaskoTeminatlar
    {
        public const string KASKO = "450";
        public const string GLKHH_TEROR =" 457";
        public const string DOGAL_AFET = "462";
        public const string DEPREM = "455";
        public const string SEL_VE_SU_BASKINI = "456";
        public const string YANGIN = "452";

        public const string HUKUKSAL_KORUMA_ARAC = "496";
        public const string HUKUKSAL_KORUMA_SURUCU = "497";
        public const string FERDI_KAZA =" 900";
        public const string KAZAEN_OLUM = "571";
        public const string KAZAEN_SUREKLI_SAKATLIK = "572";
        public const string TEDAVI_MASRAFLARI = "570";

        public const string AKSIGORTA_YARDIM = "460";
        public const string MINI_HASAR_ONARIM = "MHS";
        public const string ACIL_TIBBI_YARDIM = "382";
       

    }

    public class Ak_KaskoTarifeKodlari
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

    public class Ak_KaskoTeminatKodlari
    {
        public const int FerdiKazaTedaviMasraflari = 615;
        public const int FerdiKazaVefatTeminati = 635;
    }

    public class Ak_KaskoSoruCevapTipleri
    {
        public const string Kombine = "1";
        public const string KisiKazaBasina = "2";
        public const int Diger = 99;
    }

    public class AK_OdemeTipleri
    {
        public const string KrediKartiTaksitli = "1";
        public const string KrediKartiPesin = "2";
        public const string Nakit = "0";
    }

    public class AK_OdemeSekilleri
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

    public class Ak_KaskoSoruTipleri
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

    public class Ak_AracGrupKodlari
    {
        public const string Otomobil9 = "1";
        public const string Taksi = "2";
        public const string Kamyonet = "3";
        public const string Minibus1017 = "4";
        public const string IsMakinesi = "5";
        public const string Kamyon = "6";
        public const string Otobus1830 = "7";
        public const string Otobus31 = "8";
        public const string Motorsiklet = "12";
        public const string Traktor = "14";
        public const string Romork = "15";
        public const string Tanker = "16";
        public const string Cekici = "17";
        public const string Ozel = "18";
        public const string TarimMakinesi = "23";

    }

    public class AK_AdresKodlari
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


    public class AK_KaskoSoruKodlari
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

    public class AK_HasarsizlikIndirimleri
    {
        public const string Bir = "0";
        public const string Iki = "30";
        public const string Uc = "40";
        public const string Dort = "50";
        public const string Bes = "60";
    }

    public class AK_UrunKodlari
    {
        public const string Kasko = "K11";
       
    }
}
