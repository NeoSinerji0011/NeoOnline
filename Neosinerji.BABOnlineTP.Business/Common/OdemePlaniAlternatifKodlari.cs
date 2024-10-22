using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class OdemePlaniAlternatifKodlari
    {
        public const int Yok = 0;
        public const int Pesin = 1;
        public const int Taksit2 = 2;
        public const int Taksit3 = 3;
        public const int Taksit4 = 4;
        public const int Taksit5 = 5;
        public const int Taksit6 = 6;
        public const int Taksit7 = 7;
        public const int Taksit8 = 8;
        public const int Taksit9 = 9;
        public const int Taksit10 = 10;
        public const int Taksit11 = 11;
        public const int Taksit12 = 12;

        public static int TaksitSayisi(int odemePlaniAlternatifKodu)
        {
            switch (odemePlaniAlternatifKodu)
            {
                case OdemePlaniAlternatifKodlari.Pesin: return 1;
                case OdemePlaniAlternatifKodlari.Taksit2: return 2;
                case OdemePlaniAlternatifKodlari.Taksit3: return 3;
                case OdemePlaniAlternatifKodlari.Taksit4: return 4;
                case OdemePlaniAlternatifKodlari.Taksit5: return 5;
                case OdemePlaniAlternatifKodlari.Taksit6: return 6;
                case OdemePlaniAlternatifKodlari.Taksit7: return 7;
                case OdemePlaniAlternatifKodlari.Taksit8: return 8;
                case OdemePlaniAlternatifKodlari.Taksit9: return 9;
                case OdemePlaniAlternatifKodlari.Taksit10: return 10;
                case OdemePlaniAlternatifKodlari.Taksit11: return 11;
                case OdemePlaniAlternatifKodlari.Taksit12: return 12;
                default: return 0;
            }
        }
    }
}
