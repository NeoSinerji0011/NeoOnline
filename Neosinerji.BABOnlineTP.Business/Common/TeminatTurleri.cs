using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class TeminatTurleri
    {
        public const byte Normal = 1;
        public const byte Yildiz_3 = 2;
        public const byte Yildiz_5 = 3;

        public static List<SelectListItem> TeminatTurleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem() { Value = "1", Text ="Normal"},
                new SelectListItem() { Value = "2", Text = ResourceHelper.GetString("Stars_3")},
                new SelectListItem() { Value = "3", Text = ResourceHelper.GetString("Stars_5")}
            });

            return list;
        }

        public static string TeminatTuru(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case TeminatTurleri.Normal: result = "Normal"; break;
                    case TeminatTurleri.Yildiz_3: result = ResourceHelper.GetString("Stars_3"); break;
                    case TeminatTurleri.Yildiz_5: result = ResourceHelper.GetString("Stars_5"); break;
                }

            return result;
        }
    }
}
