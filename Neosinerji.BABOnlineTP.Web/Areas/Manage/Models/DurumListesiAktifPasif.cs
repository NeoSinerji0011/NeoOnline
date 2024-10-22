using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public static class DurumListesiAktifPasif
    {
        public static List<SelectListItem> DurumTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.Active },
                new SelectListItem() { Value = "0", Text = babonline.Pasive }
            });
            return list;
        }

        public static List<SelectListItem> TUMDurumTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "2", Text = babonline.Suspended },
                new SelectListItem() { Value = "1", Text = babonline.Active },
                new SelectListItem() { Value = "0", Text = babonline.Pasive }
            });
            return list;
        }
        public static List<SelectListItem> FirmamiSahismi()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                 new SelectListItem() { Value = "0", Text =babonline.Person /*"Şahıs"*/ },
                new SelectListItem() { Value = "1", Text =babonline.Company /*"Firma"*/ }
               
            });
            return list;
        }

        public static List<SelectListItem> TeklifPolice()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[] {

                new SelectListItem() { Value = "2", Text = babonline.Policy_Input + " &nbsp; &nbsp; &nbsp;" }, // Poliçe Girişi
                new SelectListItem() { Value = "3", Text = babonline.Policy_Attachment_Input + " &nbsp; &nbsp; &nbsp;" }, // Poliçe Eki Girişi
                new SelectListItem() { Value = "1", Text = babonline.Offer_Input } // Teklif Girişi

            });
            return list;
        }
        public static List<SelectListItem> TeklifPoliceListesi()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "2", Text = babonline.Policy + " &nbsp; &nbsp; &nbsp;" }, // Poliçe
                new SelectListItem() { Value = "1", Text = babonline.Offer } // Teklif

            });
            return list;
        }
    }
}