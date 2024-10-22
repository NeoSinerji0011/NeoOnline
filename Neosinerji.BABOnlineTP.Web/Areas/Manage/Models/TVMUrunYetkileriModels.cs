using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{

    public class TVMUrunYetkileriModel
    {
        public int TVMKodu { get; set; }
        public string TVMAdi { get; set; }
    }

    public class TVMUrunYetkileriEkleModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public SelectList TVMListesi { get; set; }

        public List<BabOnlineListe> BabOnlineUrunListesi { get; set; }
    }

    public class BabOnlineListe
    {
        public int BabOnlineUrunKodu { get; set; }
        public string BabOnlineUrunAdi { get; set; }

        public List<TVMUrunYetkileriModel_Urun> TUMUrunList { get; set; }
    }

    public class TVMUrunYetkileriModel_Urun
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }


        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public int BabOnlineUrunKodu { get; set; }

        public bool Teklif { get; set; }
        public bool Police { get; set; }
        public bool Rapor { get; set; }
        public bool ManuelHavale { get; set; }
        public bool HavaleEntegrasyon { get; set; }
        public bool KrediKartiTahsilat { get; set; }
        public bool AcikHesapTahsilatGercek { get; set; }
        public bool AcikHesapTahsilatTuzel { get; set; }
    }

    public class TVMUrunYetkileriModel_Detay
    {
        public int TVMKodu { get; set; }
        public string TVMAdi { get; set; }

        public List<TVMUrunYetkileriModel_Urun> TUMUrunList { get; set; }
        public SelectList BabOnlineUrunListesi { get; set; }
    }
}