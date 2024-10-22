using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class AegonDetayMusteriModel
    {
        public int MusteriKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Uyruk { get; set; }
        public string AdiUnvan { get; set; }
        public string SoyadiUnvan { get; set; }
        public string DogumTarihi { get; set; }
        public string GelirVergisiOrani { get; set; }
        public string Cinsiyet { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string Email { get; set; }
        public string CepTelefonu { get; set; }
    }

    public class AegonOnProvizyonModel
    {
        public int teklifid { get; set; }
        public int pTeklifNox { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int pPartajNox { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "Başvuru numarası 10 hane olmalı!")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string pBasvuruNox { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik numarası 11 hane olmalı!")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string pTCKx { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string pSKT_Ayx { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string pSKT_Yilx { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string pCVVx { get; set; }

        public decimal pTutarx { get; set; }
        public string pParaBirimix { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime pProvTarx { get; set; }

        [KrediKartiValidation]
        public KrediKartiModel pKKNox { get; set; }

        public List<SelectListItem> Aylar { get; set; }
        public List<SelectListItem> Yillar { get; set; }
    }
}