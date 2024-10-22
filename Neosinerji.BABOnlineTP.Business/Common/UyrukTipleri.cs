using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class UyrukTipleri
    {
        public const short TC = 0;
        public const short Yabanci = 1;

        public static string UyrukTipi(short kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case UyrukTipleri.TC: result = "TC"; break;
                case UyrukTipleri.Yabanci: result = ResourceHelper.GetString("Foreign"); break;
            }

            return result;
        }
    }
}
