using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMWebServisKullanicilariModel
    {
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TUMKodu { get; set; }

        public string TUMUnvan { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullaniciAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Sifre { get; set; }

        public string KullaniciAdi2 { get; set; }

        public string Sifre2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PartajNo_ { get; set; }

        public List<SelectListItem> TUMListesi { get; set; }
    }
}