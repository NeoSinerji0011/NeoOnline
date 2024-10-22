using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public static class MerkezYetkisiListesiAktifPasif
    {
        public static List<SelectListItem> DurumTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.UseCentralAuthority },
                new SelectListItem() { Value = "0", Text = babonline.DontUseCentralAuthority }
            });
            return list;
        }
    }

    //Deneme Amaçlı
    public static class TelefonTipler
    {
        public static List<SelectListItem> TelefonTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.Mobile_Phone },
                new SelectListItem() { Value = "0", Text = babonline.Work_Phone}
            });
            return list;
        }
    }
}