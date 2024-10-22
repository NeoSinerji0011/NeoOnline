using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class UrunListModel
    {
        public List<UrunModel> Items { get; set; }
    }

    public class UrunModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? UrunKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UrunAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int BransKodu { get; set; }

        public byte Durum { get; set; }

        public SelectList Durumlar { get; set; }

        public string BransAdi { get; set; }

        public List<SelectListItem> BransList { get; set; }

        public List<UrunTeminatModel> Teminatlari { get; set; }

        public List<UrunVergiModel> Vergileri { get; set; }

        public List<UrunSoruModel> Sorulari { get; set; }
    }

    public class UrunSoruModel
    {
        public int UrunKodu { get; set; }

        public int SoruKodu { get; set; }

        public int SiraNo { get; set; }

        public string SoruAdi { get; set; }

        public short SoruCevapTipi { get; set; }
    }

    public class UrunTeminatModel
    {
        public int UrunKodu { get; set; }

        public int TeminatKodu { get; set; }

        public int SiraNo { get; set; }

        public string TeminatAdi { get; set; }
    }

    public class UrunVergiModel
    {
        public int UrunKodu { get; set; }

        public int VergiKodu { get; set; }

        public int SiraNo { get; set; }
        public string VergiAdi { get; set; }
    }
}