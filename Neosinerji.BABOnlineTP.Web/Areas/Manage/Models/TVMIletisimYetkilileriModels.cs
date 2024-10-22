using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class TVMIletisimYetkilileriListModel
    {
        public List<TVMIletisimYetkilileriModel> Items { get; set; }
    }
    public class TVMIletisimYetkilileriModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int SiraNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string GorusulenKisi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Gorevi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte TelefonTipi { get; set; }
        public SelectList TelefonTipleri { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TelefonNo { get; set; }

        public string Email { get; set; }
    }
}