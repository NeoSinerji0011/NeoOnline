using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMAcentelikleriListModel
    {
        public List<TVMAcentelikleriModel> Items { get; set; }
    }
    public class TVMAcentelikleriModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int BransKodu { get; set; }
        public MultiSelectList Branslar { get; set; }
        public string[] BranslarSelectList { get; set; }
        public string BransUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketKodu { get; set; }
        public SelectList SigortaSirketleriList { get; set; }
        public string SigortaSirketUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int OdemeTipi { get; set; }
        public SelectList OdemeTipleri { get; set; }
        public string OdemeTipiAdi { get; set; }
        public byte Durum { get; set; }
        public SelectList Durumlar { get; set; }
    }
}