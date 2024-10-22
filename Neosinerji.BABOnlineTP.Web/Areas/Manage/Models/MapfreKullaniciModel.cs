using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class MapfreKullaniciModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Bolge { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TVMUnvan { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AnaPartaj { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TaliPartaj { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullaniciAdi { get; set; }

        [EPostaAdresi]
        public string Email { get; set; }

        public List<SelectListItem> Bolgeler { get; set; }
    }
    public class MapfreKullaniciGuncelleModel
    {
        public int KullaniciId { get; set; }
        public string Bolge { get; set; }
        public string TVMUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AnaPartaj { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TaliPartaj { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullaniciAdi { get; set; }

        [EPostaAdresi]
        public string Email { get; set; }


    }
    public class MapfreKullaniciDetayModel
    {
        public int KullaniciId { get; set; }
        public string Bolge { get; set; }
        public string TVMUnvan { get; set; }
        public string AnaPartaj { get; set; }
        public string TaliPartaj { get; set; }
        public string KullaniciAdi { get; set; }
        public string Email { get; set; }

    }
    public class MapfreKullaniciListeEkranModel
    {
        public string AnaPartaj { get; set; }
        public string KullaniciAdi { get; set; }
        public bool Olusturuldu { get; set; }
    }

}