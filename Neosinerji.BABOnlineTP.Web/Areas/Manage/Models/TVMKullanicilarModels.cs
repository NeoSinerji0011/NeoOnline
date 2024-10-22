using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMKullanicilarListModel
    {
        public List<TVMKullanicilarModel> Items { get; set; }
    }
    public class TVMKullanicilarModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KullaniciKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Gorevi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte YetkiGrubu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Adi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Soyadi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TCKN { get; set; }

        public string Telefon { get; set; }
        public string Email { get; set; }
        public string CepTelefon { get; set; }
        public Nullable<DateTime> SifreGondermeTarihi { get; set; }
        public Nullable<DateTime> KayitTarihi { get; set; }
        public string Sifre { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte SifreDurumKodu { get; set; }
        public int? HataliSifreGirisSayisi { get; set; }

        public int? DepartmanKodu { get; set; }
        public List<SelectListItem> DepartmanlarList { get; set; }

        public int? YoneticiKodu { get; set; }
        public string MTKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte TeklifPoliceUretimi { get; set; }
        public string TeknikPersonelKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Durum { get; set; }
        public SelectList Durumlar { get; set; }
    }
}