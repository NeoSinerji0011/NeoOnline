using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class TeklifOdemeTipleri
    {
        public const string Nakit = "Nakit";
        public const string KrediKarti = "KrediKarti";
        public const string Havale = "Havale";

        public static string OdemeTipi(byte odemetipi)
        {
            string result = string.Empty;
            switch (odemetipi)
            {
                case OdemeTipleri.Havale: result = ResourceHelper.GetString("Cash"); break;
                case OdemeTipleri.KrediKarti: result = ResourceHelper.GetString("Credit_Card"); break;
                case OdemeTipleri.Nakit: result = ResourceHelper.GetString("Transfer"); break;
            }
            return result;
        }
    }
}
