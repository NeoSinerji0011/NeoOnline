using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Models
{
    public class AegonLoginModel
    {
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [StringLength(11, ErrorMessage = "Acente Numarası 11 karakter olabilir.")]
        public string AcenteNo { get; set; }

        [EPostaAdresi]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required_Email")]
        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required_Email_length")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_PasswordRequired")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class AegonSifremiUnuttum
    {
        [StringLength(11, ErrorMessage = "Partaj Numarası 11 karakter olabilir.")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required_Email")]
        public string PartajNo { get; set; }
    }
}