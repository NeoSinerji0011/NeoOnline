using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMBankaHesaplariListModel
    {
        public List<TVMBankaHesaplariModel> Items { get; set; }
    }

    public class TVMBankaHesaplariModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int SiraNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BankaAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SubeAdi { get; set; }
        public string HesapNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string CariHesapNo { get; set; }
        public string HesapTipi { get; set; }
        public string HesapTipiAdi { get; set; }
        public string IBAN { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HesapAdi { get; set; }
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AcenteKrediKartiNo { get; set; }     
        public List<SelectListItem> Bankalar { get; set; }
        public SelectList HesapTipleri { get; set; }
        public List<SelectListItem> Subeler { get; set; }
    }
}