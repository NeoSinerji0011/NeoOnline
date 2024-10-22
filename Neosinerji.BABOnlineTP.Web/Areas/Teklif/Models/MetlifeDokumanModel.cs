using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class MetlifeDokumanListModel
    {
        public List<MetlifeDokumanModel> Items { get; set; }
    }
    public class MetlifeDokumanModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TeklifId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DokumanTuru { get; set; }

       public string Dokuman { get; set; }
       

    }
}