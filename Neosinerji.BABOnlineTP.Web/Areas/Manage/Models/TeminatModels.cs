using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TeminatListModel
    {
        public List<TeminatModel> Items { get; set; }
    }
    public class TeminatModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? TeminatKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TeminatAdi { get; set; }
    }

    public class UrunTeminatListesi
    {
        public int TeminatKodu { get; set; }
        public string TeminatAdi { get; set; }
        public string SiraNo { get; set; }
        public int UrunKodu { get; set; }
    }

}