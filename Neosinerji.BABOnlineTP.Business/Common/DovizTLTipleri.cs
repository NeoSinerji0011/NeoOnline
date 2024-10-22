using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class DovizTLTipleri
    {
        public const byte Yok = 0;
        public const byte TL = 1;
        public const byte Doviz = 2;


        public static string DovizTLTip(byte kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case DovizTLTipleri.TL: result = "TL"; break;
                case DovizTLTipleri.Doviz: result = ResourceHelper.GetString("ForeignCurrency"); break;
            }

            return result;
        }
    }
}
