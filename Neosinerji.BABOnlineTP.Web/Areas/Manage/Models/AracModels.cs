using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class AracDetayListModel
    {
        public string MarkaKodu { get; set; }
        public SelectList MarkaList { get; set; }

        public string TipKodu { get; set; }
        public SelectList TipList { get; set; }

        public string Model { get; set; }

        public List<AracDetayModel> Items { get; set; }
    }

    public class AracDetayModel
    {
        public int? TaskId { get; set; }
        public string MarkaKodu { get; set; }
        public string MarkaAdi { get; set; }
        public string TipKodu { get; set; }
        public string TipAdi { get; set; }
        public string Model { get; set; }
        public string Fiyat { get; set; }
    }
}