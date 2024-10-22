using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class CinsiyetTipleri
    {
        public static string CinsiyetTipi(string kod)
        {
            string result = String.Empty;

            switch (kod)
            {
                case "E": result = ResourceHelper.GetString("Man"); break;
                case "K": result = ResourceHelper.GetString("Women"); break;
                default: ResourceHelper.GetString("NULL"); break;
            }

            return result;
        }

    }
}
