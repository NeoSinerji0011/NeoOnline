using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class PoliceSureleri
    {
        public const byte Ay_6 = 1;
        public const byte Ay_12 = 2;
        public const byte Ay_24 = 3;

        public static List<SelectListItem> PoliceSureleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem() { Value = "1", Text = ResourceHelper.GetString("Month_6")},
                new SelectListItem() { Value = "2", Text = ResourceHelper.GetString("Month_12")},
                new SelectListItem() { Value = "3", Text = ResourceHelper.GetString("Month_24")}
            });

            return list;
        }

        public static string PoliceSuresi(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case PoliceSureleri.Ay_6: result = ResourceHelper.GetString("Month_6"); break;
                    case PoliceSureleri.Ay_12: result = ResourceHelper.GetString("Month_12"); break;
                    case PoliceSureleri.Ay_24: result = ResourceHelper.GetString("Month_24"); break;
                }

            return result;
        }
    }
}
