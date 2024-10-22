using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Models
{
    public class HizliTeklifModel
    {
        [Required(ErrorMessage="Kimlik No giriniz.")]
        public string kimlikNo { get; set; }

        [Required(ErrorMessage = "İsim giriniz.")]
        public string adi { get; set; }

        [Required(ErrorMessage = "Soyisim giriniz.")]
        public string soyadi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string email { get; set; }

        [Required(ErrorMessage = "Acente seçiniz.")]
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }

        public List<SelectListItem> TVMList { get; set; }


    }
    public class TVMParametreModel
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
}