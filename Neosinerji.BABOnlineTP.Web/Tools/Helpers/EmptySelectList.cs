using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class EmptySelectList
    {
        public static List<SelectListItem> EmptyList()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
            return list;
        }
    }
}