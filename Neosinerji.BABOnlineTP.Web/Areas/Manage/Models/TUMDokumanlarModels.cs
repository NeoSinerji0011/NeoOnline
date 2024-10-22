using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TUMDokumanlarListModel
    {
        public List<TUMDokumanlarModel> Items { get; set; }
    }
    public class TUMDokumanlarModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TUMKodu { get; set; }

        public int SiraNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DokumanTuru { get; set; }

        public DateTime EklemeTarihi { get; set; }

        public int EkleyenPersonelKodu { get; set; }

        public string Dokuman { get; set; }
    }
}