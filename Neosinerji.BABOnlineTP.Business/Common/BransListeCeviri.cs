using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class BransListeCeviri
    {

        public static int TanimsizBransKodu = 18;
        public static string TanimsizBransAciklama = "Tanımsız";
        public static int Dask = 11;
        public static string BransTipi(int kod)
        {
            string result = "";

            int dilkodu = 1;
            string lang = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            switch (lang)
            {
                case "tr": dilkodu = 1; break;
                case "en": dilkodu = 2; break;
                case "es": dilkodu = 3; break;
                case "fr": dilkodu = 4; break;
                case "it": dilkodu = 5; break;

            }
            if (dilkodu != 1)
            {
                switch (kod)
                {
                    case 1: result = "Traffic"; break;
                    case 2: result = "Insurance"; break;
                    case 3: result = "Accidents-Non Auto "; break;
                    case 4: result = "Health"; break;
                    case 5: result = "Life"; break;
                    case 6: result = "Fire"; break;
                    case 7: result = "Transport"; break;
                    case 8: result = "Non Auto Accidents"; break;
                    case 9: result = "Engineering"; break;
                    case 10: result = "Responsibility"; break;
                    case 11: result = "Earthquake"; break;
                    case 12: result = "Farming"; break;
                    case 13: result = "Boat"; break;
                    case 14: result = "Personal Accident"; break;
                    case 15: result = "Legal Protection"; break;
                    case 16: result = "Credit"; break;
                    case 17: result = "BES"; break;
                    case 18: result = "Undefined"; break;
                    case 19: result = "Group Life"; break;
                    case 20: result = "Annual Life"; break;
                }
            }



            return result;
        }

    }
    public class BransKodlari
    {
        public const int Trafik = 1;
        public const int Kasko = 2;
        public const int KazaOtoDisi = 3;
        public const int Saglik = 4;
        public const int Hayat = 5;
        public const int Yangin = 6;
        public const int Nakliyat = 7;
        public const int TamSaglik = 8;
        public const int Mühendislik = 9;
        public const int Sorumluluk = 10;
        public const int DASK = 11;
        public const int Tarim = 12;
        public const int Tekne = 13;
        public const int FerdiKaza = 14;
        public const int HukuksalKoruma = 15;
        public const int Kredi = 16;
        public const int BES = 17;
        public const int TANIMSIZ = 18;
        public const int GrupHayat = 19;
        public const int YillikHayat = 20;
    }
}
















