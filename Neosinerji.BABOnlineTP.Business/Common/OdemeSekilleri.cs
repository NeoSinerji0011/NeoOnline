using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class OdemeSekilleri
    {
        public const byte Yok = 0;
        public const byte Pesin = 1;
        public const byte Vadeli = 2;

        public static List<SelectListItem> OdemeSekilleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                    new SelectListItem(){ Value = "1", Text = ResourceHelper.GetString("SinglePayment")},
                    new SelectListItem(){ Value = "2", Text = ResourceHelper.GetString("Forward")}
            });

            return list;
        }

        public static string OdemeSekli(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case OdemeSekilleri.Pesin: result = ResourceHelper.GetString("SinglePayment"); break;
                    case OdemeSekilleri.Vadeli: result = ResourceHelper.GetString("Forward"); break;
                }

            return result;
        }
    }
}
