using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class YetkiModel
    {
        public int TVMKodu { get; set; }
        public List<SelectListItem> TVMSelectList { get; set; }

        public List<YetkiListeServiceModel> TVMtable { get; set; }

        public bool YetkiYeniKayit { get; set; }
        public bool YetkiGuncelleme { get; set; }
    }
}