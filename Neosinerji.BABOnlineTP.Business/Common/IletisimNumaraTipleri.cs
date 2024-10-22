using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class IletisimNumaraTipleri
    {
        public const byte Yok = 0;
        public const byte Cep = 11;
        public const byte Is = 12;
        public const byte Ev = 13;
        public const byte Fax = 14;
        public const byte Diger = 15;

        public static string IletisimNumaraTipi(byte kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case IletisimNumaraTipleri.Cep: ResourceHelper.GetString("Mobile_Phone"); break;
                case IletisimNumaraTipleri.Is: ResourceHelper.GetString("Work_Phone"); break;
                case IletisimNumaraTipleri.Ev: ResourceHelper.GetString("Home_Phone"); break;
                case IletisimNumaraTipleri.Fax: ResourceHelper.GetString("Fax_Number"); break;
                case IletisimNumaraTipleri.Diger: ResourceHelper.GetString("Other"); break;
            }

            return result;
        }
    }
}
