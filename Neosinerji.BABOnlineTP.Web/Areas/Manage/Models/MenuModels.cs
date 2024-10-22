using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class MenuModelOzel
    {
        public int? AnaMenuKodu { get; set; }
        public short SiraNumarasi { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_MenuExplanation_Required")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Aciklama { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_MenuHelpExplanation_Required")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YardimAciklama { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? IslemKodu { get; set; }

        public string URL { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Durum { get; set; }

        public List<SelectListItem> MenuIslemleri { get; set; }
        public SelectList Durumlar { get; set; }
    }

    public class AnaMenuListModel
    {
        public List<AnaMenuModel> Items { get; set; }
    }

    public class AnaMenuModel : MenuModelOzel
    {
        public List<AltMenuModel> AltMenuler { get; set; }
    }

    public class ALtMenuListModel
    {
        List<AltMenuModel> Items { get; set; }
    }

    public class AltMenuModel : MenuModelOzel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int AltMenuKodu { get; set; }

        public List<AltMenuSekmeModel> AltMenuSekmeler { get; set; }
    }

    public class AltMenuSekmeListModel
    {
        public List<AltMenuSekmeModel> Items { get; set; }
    }

    public class AltMenuSekmeModel : MenuModelOzel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int AltMenuKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int SekmeKodu { get; set; }
    }

    public class MenulerModel
    {
        public List<AnaMenuModel> AnaMenuler { get; set; }
        public List<AltMenuModel> AltMenuler { get; set; }
        public List<AltMenuSekmeModel> AltMenuSekmeler { get; set; }
    }

    public class MenuIslemModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int IslemKodu { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_MenuOperationMVCController_Required")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IslemId { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_MenuOperationURL_Required")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string URL { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_MenuOperationMVCAActionParameter_Required")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Icon { get; set; }
    }

    public class MenuIslemListModel
    {
        public List<MenuIslemModel> Items { get; set; }
    }
}