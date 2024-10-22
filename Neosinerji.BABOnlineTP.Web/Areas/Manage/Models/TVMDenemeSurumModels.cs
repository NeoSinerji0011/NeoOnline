using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMDenemeSurumModels
    {
    }


    public class TVMDenemeParametreler
    {
        public TVMDetay tvmDetay { get; set; }
        public List<TVMDepartmanlar> departmanlar { get; set; }
        public List<TVMBolgeleri> bolgeler { get; set; }
        public List<TVMUrunYetkileri> urunYetkileri { get; set; }
        public List<TVMWebServisKullanicilari> servisKullanicilar { get; set; }
        public List<TVMKullanicilar> kullanicilar { get; set; }
        public TVMYetkiGruplari yetkiGrup { get; set; }

        public List<TVMYetkiGrupYetkileri> yetkiGrupYetkileri { get; set; }
    }

    public class TVMPDenemeEkranModel
    {
        public string TvmUnvan { get; set; }
        public string LevhaNo { get; set; }
        public string TeknikPersonelAd { get; set; }
        public string Email { get; set; }
    }
}