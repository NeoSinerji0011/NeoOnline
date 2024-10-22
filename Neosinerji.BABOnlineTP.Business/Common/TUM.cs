using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class TeklifUretimMerkezleri
    {
        public const int HDI = 1;
        public const int AXA = 2;
        public const int MAPFRE = 3;
        public const int ANADOLU = 4;
        public const int DEMIR = 5;
        public const int GULF = 6;
        public const int AEGON = 7;
        public const int METLIFE = 8;
        public const int RAY = 9;
        public const int SOMPOJAPAN = 10;
        public const int TURKNIPPON = 11;
        public const int EUREKO = 23;
        public const int ALLIANZ = 14;
        public const int ERGO = 21;
        public const int UNICO = 17;
        public const int GROUPAMA = 26;
        public const int KORU = 36;
        public const int AK = 13;
    }

    public class TUMKisaIsim
    {
        public const string HDI = "HDI";
        public const string AXA = "AXA";
        public const string MAPFRE = "MAPFRE";
        public const string ANADOLU = "ANADOLU";
        public const string DEMIR = "DEMİR";
        public const string GULF = "GULF";
        public const string AEGON = "AEGON";
        public const string METLIFE = "METLIFE";
        public const string RAY = "RAY";
        public const string SOMPOJAPAN = "SOMPOJAPAN";
        public const string TURKNIPPON = "TURKNIPPON";
        public const string EUREKO = "EUREKO";
        public const string ALLIANZ = "ALLIANZ";
        public const string ERGO = "ERGO";
        public const string UNICO = "UNICO";
        public const string GROUPAMA = "GROUPAMA";
        public const string KORU = "KORU";
        public const string AK = "AK";

    }

    public class TUMImgUrlAdres
    {
        public const string HDI = "/Content/img/hdi.png";
        public const string AXA = "/Content/img/axa.png";
        public const string MAPFRE = "/Content/img/mapfre.png";
        public const string ANADOLU = "/Content/img/anadolu.png";
        public const string DEMIR = "/Content/img/demir.png";
        public const string GULF = "/Content/img/gulf-logo.png";
        public const string AEGON = "/Content/img/Aegon/aegonlogo.jpg";
        public const string METLIFE = "/Content/img/metlife-tvm-logo.png";
        public const string RAY = "/Content/img/ray.jpg";
        public const string SOMPOJAPAN = "/Content/img/SompoJapan.jpg";
        public const string TURKNIPPON = "/Content/img/Turknippon.png";
        public const string EUREKO = "/Content/img/eureko-logo.png";
        public const string ERGO = "/Content/img/ergo-logo.png";
        public const string ALLIANZ = "/Content/img/allianz-logo.png";
        public const string UNICO = "/Content/img/unico-logo.png";
        public const string GROUPAMA = "/Content/img/Groupama_Logolar.png";
        public const string KORU = "/Content/img/koru.jpg";
        public const string AK = "/Content/img/ak.png";

        public static string GetTUMUrl(int tumkodu)
        {
            string result = "";
            switch (tumkodu)
            {
                case TeklifUretimMerkezleri.HDI: result = TUMImgUrlAdres.HDI; break;
                case TeklifUretimMerkezleri.AXA: result = TUMImgUrlAdres.AXA; break;
                case TeklifUretimMerkezleri.MAPFRE: result = TUMImgUrlAdres.MAPFRE; break;
                case TeklifUretimMerkezleri.ANADOLU: result = TUMImgUrlAdres.ANADOLU; break;
                case TeklifUretimMerkezleri.DEMIR: result = TUMImgUrlAdres.DEMIR; break;
                case TeklifUretimMerkezleri.GULF: result = TUMImgUrlAdres.GULF; break;
                case TeklifUretimMerkezleri.AEGON: result = TUMImgUrlAdres.AEGON; break;
                case TeklifUretimMerkezleri.METLIFE: result = TUMImgUrlAdres.METLIFE; break;
                case TeklifUretimMerkezleri.RAY: result = TUMImgUrlAdres.RAY; break;
                case TeklifUretimMerkezleri.SOMPOJAPAN: result = TUMImgUrlAdres.SOMPOJAPAN; break;
                case TeklifUretimMerkezleri.TURKNIPPON: result = TUMImgUrlAdres.TURKNIPPON; break;
                case TeklifUretimMerkezleri.EUREKO: result = TUMImgUrlAdres.EUREKO; break;
                case TeklifUretimMerkezleri.ALLIANZ: result = TUMImgUrlAdres.ALLIANZ; break;
                case TeklifUretimMerkezleri.ERGO: result = TUMImgUrlAdres.ERGO; break;
                case TeklifUretimMerkezleri.UNICO: result = TUMImgUrlAdres.UNICO; break;
                case TeklifUretimMerkezleri.GROUPAMA: result = TUMImgUrlAdres.GROUPAMA; break;
                case TeklifUretimMerkezleri.KORU: result = TUMImgUrlAdres.KORU; break;
                case TeklifUretimMerkezleri.AK: result = TUMImgUrlAdres.AK; break;

            }
            return result;
        }

        public static string GetTUMKisaIsim(int tumkodu)
        {
            string result = "";
            switch (tumkodu)
            {
                case TeklifUretimMerkezleri.HDI: result = TUMKisaIsim.HDI; break;
                case TeklifUretimMerkezleri.AXA: result = TUMKisaIsim.AXA; break;
                case TeklifUretimMerkezleri.MAPFRE: result = TUMKisaIsim.MAPFRE; break;
                case TeklifUretimMerkezleri.ANADOLU: result = TUMKisaIsim.ANADOLU; break;
                case TeklifUretimMerkezleri.DEMIR: result = TUMKisaIsim.DEMIR; break;
                case TeklifUretimMerkezleri.GULF: result = TUMKisaIsim.GULF; break;
                case TeklifUretimMerkezleri.AEGON: result = TUMKisaIsim.AEGON; break;
                case TeklifUretimMerkezleri.METLIFE: result = TUMKisaIsim.METLIFE; break;
                case TeklifUretimMerkezleri.RAY: result = TUMKisaIsim.RAY; break;
                case TeklifUretimMerkezleri.SOMPOJAPAN: result = TUMKisaIsim.SOMPOJAPAN; break;
                case TeklifUretimMerkezleri.TURKNIPPON: result = TUMKisaIsim.TURKNIPPON; break;
                case TeklifUretimMerkezleri.EUREKO: result = TUMKisaIsim.EUREKO; break;
                case TeklifUretimMerkezleri.ERGO: result = TUMKisaIsim.ERGO; break;
                case TeklifUretimMerkezleri.ALLIANZ: result = TUMKisaIsim.ALLIANZ; break;
                case TeklifUretimMerkezleri.UNICO: result = TUMKisaIsim.UNICO; break;
                case TeklifUretimMerkezleri.GROUPAMA: result = TUMKisaIsim.GROUPAMA; break;
                case TeklifUretimMerkezleri.KORU: result = TUMKisaIsim.KORU; break;
                case TeklifUretimMerkezleri.AK: result = TUMKisaIsim.AK; break;

            }
            return result;
        }
    }
}
