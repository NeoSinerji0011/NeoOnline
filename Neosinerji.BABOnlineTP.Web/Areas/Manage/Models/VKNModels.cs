using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class VKNListModel
    {
        public List<VKNModel> Items { get; set; }
    }
    public class VKNModel
    {
        public DateTime? DogumTarihi { get; set; }
        public string BabaAdi { get; set; }
        public string Durum { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string VergiNo { get; set; }
        public string SirketTuru { get; set; }
        public string DogumYeri { get; set; }

        public SelectList Durumlar { get; set; }
    }
}