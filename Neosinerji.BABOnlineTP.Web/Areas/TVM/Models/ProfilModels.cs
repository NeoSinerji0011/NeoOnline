using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Models
{
    public class ProfilModel
    {
        public int KullaniciKodu { get; set; }
        public string TCKN { get; set; }
        public string URL { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public string YetkiGrubu { get; set; }
        public string TVMAdi { get; set; }
        public string Departman { get; set; }
        public string Yonetici { get; set; }
        public string Gorevi { get; set; }
        public string MTKodu { get; set; }
        public int SifreGecerlilikSuresi { get; set; }
        public DateTime KayitTarihi { get; set; }
    }

    public class SifreDegistirModel : ProfilModel
    {
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Password_MaxLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string EskiSifre { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Password_MaxLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YeniSifre { get; set; }

        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("YeniSifre", ErrorMessageResourceName = "Message_PasswordNew_Confirm", ErrorMessageResourceType = typeof(babonline))]
        public string YeniSifreTekrar { get; set; }
    }
}