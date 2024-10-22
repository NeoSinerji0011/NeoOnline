using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMBolgeleriListModel
    {
        public List<TVMBolgeleriModel> Items { get; set; }
    }
    public class TVMBolgeleriModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMBolgeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BolgeAdi { get; set; }

        public string Aciklama { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Durum { get; set; }
        public SelectList Durumlar { get; set; }
    }
}