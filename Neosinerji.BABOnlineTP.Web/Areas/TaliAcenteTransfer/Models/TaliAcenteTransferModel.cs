using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TaliAcenteTransfer.Models
{
    public class TaliAcenteTransferModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? BagliOlduguTVMKodu { get; set; }


        public int? BolgeKodu { get; set; }

        public List<SelectListItem> Bolgeler { get; set; }

        public string TVMUnvani { get; set; }


    }


    public class TVMParametreler
    {
        public TVMDetay tvmDetay { get; set; }
        public List<TVMDepartmanlar> departmanlar { get; set; }
        public List<TVMBolgeleri> bolgeler { get; set; }
        public List<TVMUrunYetkileri> urunYetkileri { get; set; }
        public List<TVMWebServisKullanicilari> servisKullanicilar { get; set; }
        public List<TVMKullanicilar> kullanicilar { get; set; }
        public List<TVMYetkiGruplari> yetkiGruplari { get; set; }

        public List<TVMYetkiGrupYetkileri> yetkiGrupYetkileri { get; set; }
    }

    public class TaliTransferKayitModel
    {
        public int taliCount { get; set; }
        public int basariliKayitlar { get; set; }
        public int basarisizKayitlar { get; set; }
        public string Path { get; set; }
        public string tvmKodu { get; set; }
    }
}