using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    //1. Çelik - Betonarme - Karkas / 2. Diğer / 3. Yığma Kagir 
    public class DaskBinaYapiTazrlari
    {
        public const byte Celik_Betonarme_Karkas = 1;
        public const byte Yigma_Kagir = 2;
        public const byte Diger = 3;
    }
    public class DaskYapiTarzlari
    {
        public const string Celik_Betonarme_Karkas = "ÇELİK, BETONARME KARKAS";
        public const string Yigma_Kagir = "YIĞMA KAGİR";
        public const string Diger = "DİĞER";

        public static string YapiTarzi(byte Kodu)
        {
            string result = String.Empty;
            switch (Kodu)
            {
                case DaskBinaYapiTazrlari.Celik_Betonarme_Karkas: result = ResourceHelper.GetString("SteelConcreteFramework"); break;
                case DaskBinaYapiTazrlari.Yigma_Kagir: result = ResourceHelper.GetString("BrickMasonry"); break;
                case DaskBinaYapiTazrlari.Diger: result = ResourceHelper.GetString("Other"); break;
            }

            return result;

       }
    }


    //Banka / Finansal Kurum
    public class DaskTipleri
    {
        public const byte Banka = 1;
        public const byte FinansalKurum = 2;
    }

    //1. 01-04 Arası Kat / 2. 05-07 Arası Kat / 3. 08-19 Arası Kat / 4. 20 - Üzeri Katlar
    public class DaskBinaKatSayisi
    {
        public const byte KatArasi_01_04 = 1;
        public const byte KatArasi_05_07 = 2;
        public const byte KatArasi_08_19 = 3;
        public const byte KatArasi_20_Uzeri = 4;
    }
    public class DaskBinaKatSayilari
    {
       

        public const string KatArasi_01_04 = "01-04 Arası Kat";
        public const string KatArasi_05_07 = "05-07 Arası Kat";
        public const string KatArasi_08_19 = "08-19 Arası Kat";
        public const string KatArasi_20_Uzeri = "20 Üzeri";

        public static string BinaKatSayisi(byte Kod)
        {
            System.Web.HttpContext.GetGlobalResourceObject("babonline", "");
            string result = String.Empty;
            switch (Kod)
            {
                case DaskBinaKatSayisi.KatArasi_01_04: result = ResourceHelper.GetString("BetweenFloor_01_04"); break;
                case DaskBinaKatSayisi.KatArasi_05_07: result = ResourceHelper.GetString("BetweenFloor_05_07"); break;
                case DaskBinaKatSayisi.KatArasi_08_19: result = ResourceHelper.GetString("BetweenFloor_08_19"); break;
                case DaskBinaKatSayisi.KatArasi_20_Uzeri: result = ResourceHelper.GetString("Over20"); break;
            }
            return result;
        }
    }


    //    1. 1975 - Öncesi / 2. 1976 - 1996 / 3. 1997 - 1999 / 4. 2000 - 2006 / 5. 2007 ve Sonrası
    public class DaskBinaInsaYili
    {
        public const byte Oncesi_1975 = 1;
        public const byte Arasi_1976_1996 = 2;
        public const byte Arasi_1997_1999 = 3;
        public const byte Arasi_2000_2006 = 4;
        public const byte Sonrasi_2007 = 5;
    }
    public class DaskBinaInsaYillari
    {

        public const string Oncesi_1975 = "1975 - Öncesi";
        public const string Arasi_1976_1996 = "1976 - 1996";
        public const string Arasi_1997_1999 = "1997 - 1999";
        public const string Arasi_2000_2006 = "2000 - 2006";
        public const string Sonrasi_2007 = "2007 ve Sonrası";

        public static string BinaInsaYili(byte Kod)
        {
            string result = string.Empty;
            switch (Kod)
            {
                case DaskBinaInsaYili.Oncesi_1975: result = ResourceHelper.GetString("Preview_1975"); break;
                case DaskBinaInsaYili.Arasi_1976_1996: result = "1976 - 1996"; break;
                case DaskBinaInsaYili.Arasi_1997_1999: result = "1997 - 1999"; break;
                case DaskBinaInsaYili.Arasi_2000_2006: result = "2000 - 2006"; break;
                case DaskBinaInsaYili.Sonrasi_2007: result = ResourceHelper.GetString("Post_2007"); break;
            }
            return result;
        }
    }



    //1. Mesken / 2. Büro / 3. Ticarethane / Diğer
    public class DaskBinaKullanimSekli
    {
        public const byte Mesken = 1;
        public const byte Buro = 2;
        public const byte Ticarethane = 3;
        public const byte Diger = 4;
    }
    public class DaskBinaKullanimSeklilleri
    {
        public const string Mesken = "Mesken";
        public const string Buro = "Büro";
        public const string Ticarethane = "Ticarethane";
        public const string Diger = "Diğer";

        public static string KullanimSekli(byte Kod)
        {
            string result = string.Empty;
            switch (Kod)
            {
                case DaskBinaKullanimSekli.Mesken: result = ResourceHelper.GetString("Dwelling"); break;
                case DaskBinaKullanimSekli.Buro: result = ResourceHelper.GetString("Bureau"); break;
                case DaskBinaKullanimSekli.Ticarethane: result = ResourceHelper.GetString("Commercial"); break;
                case DaskBinaKullanimSekli.Diger: result = ResourceHelper.GetString("Other"); break;
            }
            return result;
        }
    }



    //1. Hasarsız / 2. Az Hasarlı / 3. Orta Hasarlı
    public class DaskBinaHasarDurumu
    {
        public const byte Hasarsiz = 1;
        public const byte AzHasarli = 2;
        public const byte OrtaHasarli = 3;
    }
    public class DaskBinaHasarDurumlari
    {
        public const string Hasarsiz = "Hasarsız";
        public const string AzHasarli = "Az Hasarlı";
        public const string OrtaHasarli = "Orta Hasarlı";

        public static string HasarDurumu(byte Kod)
        {
            string result=string.Empty;
            switch (Kod)
            {
                case DaskBinaHasarDurumu.Hasarsiz: result = ResourceHelper.GetString("Undamaged"); break;
                case DaskBinaHasarDurumu.AzHasarli: result = ResourceHelper.GetString("LessDamaged"); break;
                case DaskBinaHasarDurumu.OrtaHasarli: result = ResourceHelper.GetString("CentralDamaged"); break;
            }
            return result;
        }
    }



    //1. Mal Sahibi / 2. Kiracı / 3. İnfifa Hakkı Sahibi / 4. Yönetici / 5. Akraba / 6. Daini Mürtehin / 7. Diğer
    public class Dask_S_EttirenSifati
    {
        public const byte MalSahibi = 1;
        public const byte Kiraci = 2;
        public const byte InfifaHakkiSabihi = 3;
        public const byte Yonetici = 4;
        public const byte Akraba = 5;
        public const byte DainiMurtehin = 6;
        public const byte Diger = 7;
    }
    public class Dask_S_EttirenSifatlari
    {
        public const string MalSahibi = "Mal Sahibi";
        public const string Kiraci = "Kiracı";
        public const string InfifaHakkiSabihi = "İnfifa Hakkı Sahibi";
        public const string Yonetici = "Yönetici";
        public const string Akraba = "Akraba";
        public const string DainiMurtehin = "Daini Mürtehin";
        public const string Diger = "Diğer";

        public static string Sifati(byte Kod)
        {
            string result = String.Empty;

            switch (Kod)
            {
                case Dask_S_EttirenSifati.MalSahibi: result = ResourceHelper.GetString("Owner"); break;
                case Dask_S_EttirenSifati.Kiraci: result = ResourceHelper.GetString("Tenant"); break;
                case Dask_S_EttirenSifati.InfifaHakkiSabihi: result = ResourceHelper.GetString("InfifaRightsOwner"); break;
                case Dask_S_EttirenSifati.Yonetici: result = ResourceHelper.GetString("Manager"); break;
                case Dask_S_EttirenSifati.Akraba: result = ResourceHelper.GetString("Relative"); break;
                case Dask_S_EttirenSifati.DainiMurtehin: result = ResourceHelper.GetString("DainiLossPayee"); break;
                case Dask_S_EttirenSifati.Diger: result = ResourceHelper.GetString("Other"); break;
            }
            return result;
        }
    }



    //DaskDoviz Tipi
    public class DaskDovizTipi
    {
        public const byte TL_TurkLirasi = 1;
        public const byte USD_AmerikanDolari = 2;
        public const byte EUR_Euro = 3;
    }
    public class DaskDovizTipleri
    {
        public const string TL_TurkLirasi = "TL - TÜRK LİRASI";
        public const string USD_AmerikanDolari = "USD - AMERİKAN DOLARI";
        public const string EUR_Euro = "EUR -EURO";

        public static string DovizTipi(byte DovizTipi)
        {
            string result = String.Empty;

            switch (DovizTipi)
            {
                case DaskDovizTipi.TL_TurkLirasi: result = ResourceHelper.GetString("TL_TurkishLiras"); break;
                case DaskDovizTipi.USD_AmerikanDolari: result = ResourceHelper.GetString("USD_AmericanDolar"); break;
                case DaskDovizTipi.EUR_Euro: result = ResourceHelper.GetString("EUR_Euro"); break;
            }

            return result;
        }
    }

}
