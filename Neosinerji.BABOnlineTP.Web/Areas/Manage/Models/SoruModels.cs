using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class SoruTipProvider
    {
        public static List<SelectListItem> SoruCevapTipleriList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "", Text = babonline.PleaseSelect },
                new SelectListItem() { Value = "1", Text = babonline.YesNo},
                new SelectListItem() { Value = "2", Text = babonline.Amount },
                new SelectListItem() { Value = "3", Text = babonline.Date },
                new SelectListItem() { Value = "4", Text = babonline.Text }
            });

            return list;
        }

        public static string SoruCevapTipiText(short soruCevapTipi)
        {
            switch (soruCevapTipi)
            {
                case SoruCevapTipleri.EvetHayir: return babonline.YesNo;
                case SoruCevapTipleri.Tutar: return babonline.Amount;
                case SoruCevapTipleri.Tarih: return babonline.Date;
                case SoruCevapTipleri.Metin: return babonline.Text;
                default: return String.Empty;
            }
        }
    }

    public class SoruModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? SoruKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SoruAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? SoruCevapTipi { get; set; }

        [IgnoreMap]
        public string SoruCevapTipiAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? SoruCevapUzunlugu { get; set; }

        [IgnoreMap]
        public List<SelectListItem> SoruCevapTipLeri { get; set; }
    }
}