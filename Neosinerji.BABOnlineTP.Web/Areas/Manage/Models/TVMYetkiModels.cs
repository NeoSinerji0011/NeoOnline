using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class MenuKok
    {
        public short AnaMenuKodu { get; set; }
        public bool YeniKayit { get; set; }
        public bool Gorme { get; set; }
        public bool Silme { get; set; }
        public bool Degistirme { get; set; }
    }

    public class AnaMenuYetkiModel : MenuKok
    {
        public AnaMenuYetkiModel()
        {
            this.AltMenuler = new List<AltMenuYetkiModel>();
        }

        public string MenuAdi { get; set; }
        public List<AltMenuYetkiModel> AltMenuler { get; set; }
    }

    public class AltMenuYetkiModel : MenuKok
    {
        public AltMenuYetkiModel()
        {
            this.Sekmeler = new List<SekmeYetkiModel>();
        }

        public short AltMenuKodu { get; set; }
        public string MenuAdi { get; set; }

        public List<SekmeYetkiModel> Sekmeler { get; set; }
    }

    public class SekmeYetkiModel : MenuKok
    {
        public short AltMenuKodu { get; set; }
        public short SekmeKodu { get; set; }
        public string MenuAdi { get; set; }
    }

    public class YetkiEklemeModel
    {
        public YetkiEklemeModel()
        {
            this.AnaMenuler = new List<AnaMenuYetkiModel>();
        }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string GrupAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool YetkiSeviyesi { get; set; }

        public int YetkiGrupKodu { get; set; }
        public string TVMUnvani { get; set; }
        public List<AnaMenuYetkiModel> AnaMenuler { get; set; }
    }

    public class YetkiGruplariListModel
    {
        public List<YetkiGruplariModel> Items { get; set; }
    }
    public class YetkiGruplariModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }

        public int TVMUnvani { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short YetkiGrupKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YetkiGrupAdi { get; set; }
    }

    public class YetkiGrupYetkileriListModel
    {
        public List<YetkiGrupYetkileriModel> Items { get; set; }
    }

    public class YetkiGrupYetkileriModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short YetkiGrupKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short AnaMenuKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short AltMenuKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short SekmeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Gorme { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte YeniKayit { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Degistirme { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Silme { get; set; }
    }
}