using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class SatisTurleri
    {
        public const byte YeniSatis = 1;
        public const byte Yenileme = 2;

        public static List<SelectListItem> SatisTurleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem() { Value = "1", Text = ResourceHelper.GetString("NewSales")},
                new SelectListItem() { Value = "2", Text = ResourceHelper.GetString("Renovation")}
            });

            return list;
        }

        public static string SatisTuru(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case SatisTurleri.YeniSatis: result = ResourceHelper.GetString("NewSales"); break;
                    case SatisTurleri.Yenileme: result = ResourceHelper.GetString("Renovation"); break;
                }

            return result;
        }
    }
}
