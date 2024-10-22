using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.ComponentModel.DataAnnotations;
using Neosinerji.BABOnlineTP.Business.Common;


namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{

    public class HataLogModel
    {
        public string TVMKullaniciAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string LogType { get; set; }
        public List<SelectListItem> LogTypeTipleri { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Saat { get; set; }

        public List<YetkiHataServisModel> HataList { get; set; }
    }

    public static class HataLogListProvider
    {
        public static List<SelectListItem> HataLogTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "Visit", Text = "Visit" },
                new SelectListItem() { Value = "Info", Text = "Info" },
                new SelectListItem() { Value = "Warning", Text = "Warning" },
                new SelectListItem() { Value = "Error", Text = "Error"},
            });

            return list;
        }
    }
}
