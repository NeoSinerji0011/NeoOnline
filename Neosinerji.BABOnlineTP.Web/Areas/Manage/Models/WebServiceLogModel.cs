using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class WebServiceLogListModel
    {
        public int? TeklifId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte IstekTipi { get; set; }

        public SelectList IstekTipleri { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BitisTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TvmHQKodu { get; set; }

        public int? TvmKodu { get; set; }

        public List<SelectListItem> TvmHQList { get; set; }
        public List<SelectListItem> TvmList { get; set; }
    }

    /*
    public class WebServiceLogModel
    {
        public int LogId { get; set; }
        public int? TeklifId { get; set; }
        public byte IstekTipi { get; set; }
        public System.DateTime IstekTarihi { get; set; }
        public System.DateTime CevapTarihi { get; set; }
        public byte BasariliBasarisiz { get; set; }
        public string IstekUrl { get; set; }
        public string CevapUrl { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
     */

    public static class WebServiceLogListProvider
    {
        public static List<SelectListItem> IstekTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "1", Text = babonline.Proposal },
                new SelectListItem() { Value = "2", Text = babonline.Policy },
                //new SelectListItem() { Value = "3", Text = babonline.Customer_Save },
                new SelectListItem() { Value = "4", Text = babonline.Accounting},
                new SelectListItem() { Value = "5", Text = "Kimlik Sorgu"},
                new SelectListItem() { Value = "6", Text = "Plaka Sorgu"},
                new SelectListItem() { Value = "7", Text = "EGM Sorgu"},
                new SelectListItem() { Value = "8", Text = "Otorizasyon Sorgu"},
                new SelectListItem() { Value = "9", Text = "Ön Provizyon"}
            });

            return list;
        }
    }
}