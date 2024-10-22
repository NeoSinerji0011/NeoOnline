using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class SeyehatPlanlari
    {
        public const byte Gumus = 1;
        public const byte Altin = 2;
        public const byte Platin = 3;
    }

    public class SeyehatPlanlariList
    {
        public const string Gumus = "Gümüş";
        public const string Altın = "Altın";
        public const string Platin = "Platin";

        public static string SeyehatPlani(byte planKodu)
        {
            string result = string.Empty;
            switch (planKodu)
            {
                case SeyehatPlanlari.Gumus: result = ResourceHelper.GetString("Silver"); break;
                case SeyehatPlanlari.Altin: result = ResourceHelper.GetString("Gold"); break;
                case SeyehatPlanlari.Platin: result = ResourceHelper.GetString("Platinum"); break;
            }
            return result;
        }
    }
}
