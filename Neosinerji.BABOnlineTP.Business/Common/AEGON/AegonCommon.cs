using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class AegonPrimDonemleri
    {
        public const byte Aylik = 1;
        public const byte Aylik_3 = 2;
        public const byte Aylik_6 = 3;
        public const byte Yillik = 4;

        public static string PrimDonemleriText(string kod)
        {
            string result = String.Empty;
            switch (kod)
            {
                case "1": result = "Aylık"; break;
                case "2": result = "3 Aylık"; break;
                case "3": result = "6 Aylık"; break;
                case "4": result = "Yıllık"; break;
            }

            return result;
        }
    }

    public class AegonParaBirimleri
    {
        public const int EUR = 1;
        public const int USD = 2;
        public const int TL = 3;

        public static string ParaBirimiText(string kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case "1": result = "EUR"; break;
                case "2": result = "USD"; break;
                case "3": result = "TL"; break;
            }

            return result;
        }
    }

    public class AegonCommon
    {
        public const string LogoSrc = "https://neobabstoragetest.blob.core.windows.net/musteri-dokuman/4648/aegonlogo.png";
        public const string LogoURL = "http://www.aegon.com.tr";
        public const string FirmaKisaAdi = "NEO";
    }

    public class AegonOnProvizyonModelDetay
    {
        public string partajNo { get; set; }
        public string basvuruNo { get; set; }
        public string tckn { get; set; }
        public string tutar { get; set; }
        public string paraBirimi { get; set; }
        public string krediKarti { get; set; }
        public string onayKodu { get; set; }
    }
}
