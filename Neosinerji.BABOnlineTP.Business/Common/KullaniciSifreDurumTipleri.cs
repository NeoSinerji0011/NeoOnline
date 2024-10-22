using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class KullaniciSifreDurumTipleri
    {
        public const byte SorunYok = 0;
        public const byte YanlisGiris = 1;
        public const byte Kilitli = 2;
        public const byte GeciciSifre = 3;
    }

    public class KullaniciSifremiUnuttumTipleri
    {
        public const byte LinkGonderildi = 1;
        public const byte SifreResetlendi = 2;
        public const byte SureDoldu = 3;
    }
}
