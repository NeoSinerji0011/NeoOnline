using System;
using AutoMapper;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class DuyuruEkleModel
    {
        public int DuyuruId { get; set; }

        [AllowHtml]
        public string Aciklama { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Konu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BaslangisTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BitisTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Oncelik { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] TVMLerSelectList { get; set; }

        public MultiSelectList TVMLerItems { get; set; }
        public SelectList Oncelikler { get; set; }
    }

    public class DuyuruDetayModel
    {
        public int DuyuruId { get; set; }
        public string Aciklama { get; set; }
        public string Konu { get; set; }
        public DateTime BaslangisTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public DateTime EklemeTarihi { get; set; }

        [IgnoreMap]
        public string Ekleyen { get; set; }

        [IgnoreMap]
        public string Oncelik { get; set; }

        [IgnoreMap]
        public List<String> EkliTVMler { get; set; }
    }

    public class DuyuruListeModel
    {
        public List<DuyuruDetayModel> Items { get; set; }
    }
}