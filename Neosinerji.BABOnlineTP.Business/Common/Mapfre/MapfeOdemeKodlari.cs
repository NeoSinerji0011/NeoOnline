using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class MapfeOdemeKodlari
    {
        public const int PESIN = 10;
        public const int TAKSIT4 = 13;
        public const int TAKSIT5 = 14;
        public const int TAKSIT6 = 15;
        public const int TAKSIT7 = 16;
        public const int TAKSIT8 = 26;
        public const int TAKSIT9 = 18;
        public const int TAKSIT10 = 24;
        public const int TAKSIT12 = 27;

        public static int OdemeKodu(int taksit)
        {
            switch (taksit)
            {
                case 1: return MapfeOdemeKodlari.PESIN;
                case 4: return MapfeOdemeKodlari.TAKSIT4;
                case 5: return MapfeOdemeKodlari.TAKSIT5;
                case 6: return MapfeOdemeKodlari.TAKSIT6;
                case 7: return MapfeOdemeKodlari.TAKSIT7;
                case 8: return MapfeOdemeKodlari.TAKSIT8;
                case 9: return MapfeOdemeKodlari.TAKSIT9;
                case 10: return MapfeOdemeKodlari.TAKSIT10;
                case 12: return MapfeOdemeKodlari.TAKSIT12;
                default: return MapfeOdemeKodlari.PESIN;
            }
        }
    }
}
