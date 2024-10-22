using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class NeoConnectLogModels
    {
        public List<NeoConnectLogListModel> raporList = new List<NeoConnectLogListModel>();

        public int LogId { get; set; }
        public int TvmKodu { get; set; }
        public Nullable<int> KullaniciKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string Kullanici { get; set; }
        public string SigortaSirketKodu { get; set; }
        public string IPAdresi { get; set; }
        public string MACAdresi { get; set; }
        public Nullable<System.DateTime> KullaniciGirisTarihi { get; set; }
        public Nullable<System.DateTime> KullaniciCikisTarihi { get; set; }
        
        public List<SelectListItem> KullanicilarList { get; set; }
        public string[] KullanicilarSelectList { get; set; }
        public string Kullanicilar { get; set; }
        
        public string SigortaSirket { get; set; }
        public SelectList SigortaSirketleri { get; set; }
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] SigortaSirketleriSelectList { get; set; }

        public List<SelectListItem> Aylar { get; set; }
        public int Ay { get; set; }
        public List<SelectListItem> Yillar { get; set; }
        public int Yil { get; set; }
        public List<int> TVMListe { get; set; }
        public List<string> SigortaSirketleriListe { get; set; }
        public SelectList tvmler { get; set; }
        // [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string[] tvmList { get; set; }
        public List<SelectListItem> TVMSelectList { get; set; }

    }
    public class NeoConnectLogListModel
        {
         public string Kullanici { get; set; }
         public string SigortaSirketKodu { get; set; }
         public string IPAdresi  { get; set; }
         public string MACAdresi { get; set; }
         public Nullable<System.DateTime> KullaniciGirisTarihi { get; set; }
         public Nullable<System.DateTime> KullaniciCikisTarihi { get; set; }
        public string SirketKullaniciAdi { get; set; }
        public string SirketKullaniciSifre { get; set; }
    }
}