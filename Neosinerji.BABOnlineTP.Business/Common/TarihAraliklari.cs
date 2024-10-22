using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class TarihAraliklari
    {
        public const byte Yok = 0;
        public const byte son7 = 1;
        public const byte son15 = 2;
        public const byte son30 = 3;
        public const byte son45 = 4;

        public static List<SelectListItem> TarihAraliklariList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            //list.AddRange(new SelectListItem[]{
            //        new SelectListItem(){ Text=ResourceHelper.GetString("TheLast7Days"), Value="1"},
            //        new SelectListItem(){ Text=ResourceHelper.GetString("TheLast15Days"), Value="2"},
            //        new SelectListItem(){ Text=ResourceHelper.GetString("TheLast30Days"), Value="3"},
            //        new SelectListItem(){ Text=ResourceHelper.GetString("TheLast45Days"), Value="4"}
            //});
            list.AddRange(new SelectListItem[]{
                    new SelectListItem(){ Text="Son 7 gün", Value="1"},
                    new SelectListItem(){ Text="Son 15 gün", Value="2"},
                    new SelectListItem(){ Text="Son 30 gün", Value="3"},
                    new SelectListItem(){ Text="Son 45 gün", Value="4"}
            });



            return list;
        }

        public static string TarihAraligi(byte? kod)
        {
            string result = String.Empty;

            if (kod.HasValue)
                switch (kod)
                {
                    case TarihAraliklari.son7: result = ResourceHelper.GetString("TheLast7Days");   break;
                    case TarihAraliklari.son15: result = ResourceHelper.GetString("TheLast15Days"); break;
                    case TarihAraliklari.son30: result = ResourceHelper.GetString("TheLast30Days"); break;
                    case TarihAraliklari.son45: result = ResourceHelper.GetString("TheLast45Days"); break;
                }

            return result;
        }
    }
}
