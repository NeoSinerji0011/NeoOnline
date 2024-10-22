using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Models
{
    public class LoginModel
    {
        //[Required(ErrorMessageResourceType=typeof(babonline), ErrorMessageResourceName="Message_TCKNRequired")]
        //[StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TCKNLength", MinimumLength = 11)]
        //public string TCKN { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string Email { get; set; }
    }
    public class LilyumLoginModel
    {
        //[Required(ErrorMessageResourceType=typeof(babonline), ErrorMessageResourceName="Message_TCKNRequired")]
        //[StringLength(11, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TCKNLength", MinimumLength = 11)]
        //public string TCKN { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string Email { get; set; }

        public LilyumMusteriKayitModel musteriModel = new LilyumMusteriKayitModel();
    }
    public class RecoverPassword
    {
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TCKNRequired")]
        //public string TCKN { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string Email { get; set; }
    }

    public class MapfreRecoverPassword
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullaniciKodu { get; set; }
    }

    public class LoginModelAEGON
    {
        [Required(ErrorMessage = "Partaj Numarası giriniz.")]
        [StringLength(11, ErrorMessage = "Partaj Numarası 11 karakterden uzun olamaz")]
        public string AcenteNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

    public class LoginModel_SSO_AEGON
    {
        public string NAME { get; set; }
        public string EMAIL { get; set; }

        [Required]
        public string USER_NAME { get; set; }

        [Required]
        public string SESSION_TOKEN { get; set; }

        [Required]
        public string CHECKSUM { get; set; }
    }

    public class LoginModelMAPFRE
    {
        [Required(ErrorMessage = "Partaj Numarası giriniz.")]
        [StringLength(11, ErrorMessage = "Partaj Numarası 11 karakterden uzun olamaz")]
        public string PartajNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }

    public class InternetSatisModel
    {
        public int TVMKodu { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string Email { get; set; }

        public HaritaTeklifPoliceFormModel teklifPoliceModel = new HaritaTeklifPoliceFormModel();
    }

    public class LilyumMusteriKayitModel
    {
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string tcVkn { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Email_Length")]
        [EPostaAdresi]
        public string EMail { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Telefon { get; set; }
    }
}
