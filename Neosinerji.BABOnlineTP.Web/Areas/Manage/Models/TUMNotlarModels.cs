using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TUMNotlarListModel
    {
        public List<TUMNotlarModel> Items { get; set; }
    }
    public class TUMNotlarModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TUMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int SiraNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KonuAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public System.DateTime EklemeTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkleyenPersonelKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string NotAciklamasi { get; set; }
    }
}