using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    //1. Çelik - Betonarme - Karkas / 2. Diğer / 3. Yığma Kagir 
    public class KonutBinaYapiTazrlari
    {
        public const byte Celik_Betonarme_Karkas = 1;
        public const byte Yigma_Kagir = 2;
        public const byte Yarim_Kagir = 3;
    }

    public class KonutYapiTarzlari
    {
        public const string Celik_Betonarme_Karkas = "ÇELİK, BETONARME KARKAS";
        public const string Yigma_Kagir = "YIĞMA KAGİR";
        public const string YarimKagir = "YARIM(ADİ) KAGİR";

        public static string YapiTarzi(byte Kodu)
        {
            string result = String.Empty;
            switch (Kodu)
            {
                case KonutBinaYapiTazrlari.Celik_Betonarme_Karkas: result = ResourceHelper.GetString("SteelConcreteFramework"); break;
                case KonutBinaYapiTazrlari.Yigma_Kagir: result = ResourceHelper.GetString("BrickMasonry"); break;
                case KonutBinaYapiTazrlari.Yarim_Kagir: result = ResourceHelper.GetString("HALF_ASI_STONE"); break;
            }

            return result;

        }
    }

    public class BosKalmaSureleri
    {
        public const byte bir = 1;
        public const byte iki = 2;
        public const byte uc = 3;
        public const byte dort = 4;
        public const byte bes = 5;
        public const byte alti = 6;
        public const byte yedi = 7;
        public const byte sekiz = 8;
        public const byte dokuz = 9;
        public const byte on = 10;
        public const byte onbir = 11;

    }

    public class BosKalmaSuresi
    {
        public const string bir = "1 ay";
        public const string iki = "2 ay";
        public const string uc = "3 ay";
        public const string dort = "4 ay";
        public const string bes = "5 ay";
        public const string alti = "6 ay";
        public const string yedi = "7 ay";
        public const string sekiz = "8 ay";
        public const string dokuz = "9 ay";
        public const string on = "10 ay";
        public const string onbir = "11 ay";

        public static string Sure(byte Kodu)
        {
            string result = String.Empty;
            switch (Kodu)
            {
                case BosKalmaSureleri.bir: result = ResourceHelper.GetString("Month1"); break;
                case BosKalmaSureleri.iki: result = ResourceHelper.GetString("Month2"); break;
                case BosKalmaSureleri.uc: result = ResourceHelper.GetString("Month3"); break;
                case BosKalmaSureleri.dort: result = ResourceHelper.GetString("Month4"); break;
                case BosKalmaSureleri.bes: result = ResourceHelper.GetString("Month5"); break;
                case BosKalmaSureleri.alti: result = ResourceHelper.GetString("Month6"); break;
                case BosKalmaSureleri.yedi: result = ResourceHelper.GetString("Month7"); break;
                case BosKalmaSureleri.sekiz: result = ResourceHelper.GetString("Month8"); break;
                case BosKalmaSureleri.dokuz: result = ResourceHelper.GetString("Month9"); break;
                case BosKalmaSureleri.on: result = ResourceHelper.GetString("Month10"); break;
                case BosKalmaSureleri.onbir: result = ResourceHelper.GetString("Month11"); break;
            }

            return result;

        }
    }

    public class KonutMuafiyetTeminatKodu
    {
        public static int DepremMuafiyetiSigortasi = 1461;
        public static int DepremMuafiyetiBinaSigortasi = 1465;
        public static int DepremMuafiyetiEsyaSigortasi = 1466;
    }

    public class HDIKonutTeminatKod
    {
        public const string BINA_YANGIN = "1101";
        public const string ESYA_YANGIN = "1102";
        public const string DEGERLI_ESYA_YANGIN = "1103";
        public const string TEMELLER_YANGIN = "1122";
        public const string YANGIN_MALI_SORUMLULUK = "1312";


        public const string BINA_EK_TEMINAT = "1324";
        public const string ESYA_EK_TEMINAT = "1325";
        public const string DEPREMVEYANARDAG_PUSK = "1301";
        public const string DEPREMVEYANARDAG_PUSK_BINA = "1331";
        public const string DEPREMVEYANARDAG_PUSK_MUHTEVIYAT_ESYA = "1332";

        public const string IZOLASYON_OLAY_BAS = "1342";
        public const string EK_TEMINAT_EK_PRIMI = "1998";
        public const string ASISTAN = "1999";
        public const string HIRSIZLIK_DEGERLI_ESYA = "3100";
        public const string HIRSIZLIK = "3101";

        public const string CAM_KIRILMASI = "3102";
        public const string ACIL_TIBBI_HASTANE_FERDIKAZA = "7101";
        public const string PROMED = "7998";
        public const string ELEKTRONIK_CIHAZ = "5101";
        public const string SAHIS_MALI_SORUMLULUK_3 = "3104";

        public const string FERDI_KAZA = "3120";
        public const string MEDLINE = "7999";
        public const string KAPKAC_TEMINATI = "3325";

        public const string YER_KAYMASI = "1306";
        public const string SEL_VE_SU_BASMASI = "1303";
        public const string FIRTINA = "1305";
        public const string HUKUKSAL_KORUMA = "2305";

        //IS YERI 
        public const string GLKHHKNH_VE_TEROR = "1302";
        public const string KAR_AGIRLIGI = "1308";
        public const string KIRA_KAYBI = "1361";
        public const string IS_DURMASI = "1316";
        public const string TASINAN_PARA_BEHER_SEFER_LIMIT = "3127";
        public const string TASINAN_PARA_YILLIK_TOPLAM_LIMIT = "3127";
        public const string EMNIYET_SUISTIMAL_KISI_BASINA_YILLIK = "3112";

        public const string ISVEREN_MALI_MESULIYET_KISI_BASI_BEDENI = "3103";
        public const string ISVEREN_MALI_MESULIYET_KAZA_BASI_BEDENI = "3103";

        public const string SAHIS_MALI_SORUMLULUK_KISI_BASI_BEDENI = "3104";
        public const string SAHIS_MALI_SORUMLULUK_KAZE_BASI_BEDENI = "3104";
        public const string SAHIS_MALI_SORUMLULUK_KAZA_BASI_MADDI = "3104";

        public const string EMTEA_YANGIN = "1104";
        public const string MAKINA_VE_TECHIZAT_YANGIN = "1105";
        public const string DEMIRBAS_YANGIN = "1106";
        public const string KASA_MUHTEVIYATI_YANGIN = "1109";
        public const string SAHIS_MALLARI_YANGIN_3 = "1111";
        public const string DEKORASYON_YANGIN = "1112";
        public const string MUHTEVIYAT_EKTEMINAT = "1326";//(MUHTEVİYAT) EKTEMİNAT (*)
        public const string KOMSULUK_MALI_SORUMLULUK_YANGIN_SU_DUMAN = "1354"; // KOMŞULUK M.SORUM.(YANGIN,D.SU,DUMAN)
        public const string KOMSULUK_MALI_SORUMLULUK_TEROR = "1355"; // KOMŞULUK M.SORUM.(TERÖR)
        public const string KIRACI_MALI_SORUMLULUK_YANGIN_DUMAN = "1356"; // KİRACI M.SORUM.(YANGIN,D.SU,DUMAN)
        public const string KIRACI_MALI_SORUMLULUK_TEROR = "1357"; // KİRACI M.SORUM.( TERÖR)

        public const string KASA_HIRSIZLIK = "3319"; // KASA HIRSIZLIK
        public const string MAKINA_KIRILMASI = "5102"; // MAKİNA KIRILMASI
    }
}
