using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class SilindirHacimleri
    {
        public const byte To_2000 = 1;
        public const byte Then_2000_to_3000 = 2;

        public static List<SelectListItem> SilindirHacimleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem() { Value = "1", Text = ResourceHelper.GetString("To_2000")},
                new SelectListItem() { Value = "2", Text = ResourceHelper.GetString("Then2000To3000")},
            });

            return list;
        }

        public static string SilindirHacmi(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case SilindirHacimleri.To_2000: result = ResourceHelper.GetString("To_2000"); break;
                    case SilindirHacimleri.Then_2000_to_3000: result = ResourceHelper.GetString("Then2000To3000"); break;
                }

            return result;
        }
    }
}
