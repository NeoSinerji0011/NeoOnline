using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
   public class PoliceDurumlari
    {
        public const byte Hepsi = 0;
        public const byte Online = 1;
        public const byte Offline = 2;
        public const byte ManuelGiris = 3;

        public static List<SelectListItem> PoliceDurumlariList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                    new SelectListItem(){ Value = "0", Text = ResourceHelper.GetString("All")},
                    new SelectListItem(){ Value = "1", Text = ResourceHelper.GetString("Online")},
                      new SelectListItem(){ Value = "2", Text = ResourceHelper.GetString("Offline")}
            });

            return list;
        }

        public static string PoliceDurumu(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case PoliceDurumlari.Hepsi: result = ResourceHelper.GetString("All"); break;
                    case PoliceDurumlari.Online: result = ResourceHelper.GetString("Online"); break;
                    case PoliceDurumlari.Offline: result = ResourceHelper.GetString("Offline"); break;
                }

            return result;
        }
    }
}
