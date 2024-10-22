using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Models
{
    public static class MuhasebeListModels
    {
        public static List<SelectListItem> BorcAlacakTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "B", Text = babonline.Debt }, // Borç
                new SelectListItem() { Value = "A", Text = babonline.Receivables } // Alacak
            });

            return list;
        }
        public static List<SelectListItem> EvrakTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1", Text = babonline.Transfer_Record }, // Devir Kaydı
                new SelectListItem() { Value = "2", Text = babonline.Customer + " " + babonline.Credit_Card }, // Müşteri Kredi Kartı
                new SelectListItem() { Value = "3", Text = babonline.Firma + " " + babonline.Credit_Card }, // Firma Kredi Kartı
                new SelectListItem() { Value = "4", Text = babonline.Cash }, // Nakit
                new SelectListItem() { Value = "5", Text = babonline.Transfer }, //Havale
                new SelectListItem() { Value = "6", Text = babonline.Premium + " " + babonline.Accrual }, // Prim Tahakkuk
                new SelectListItem() { Value = "7", Text = babonline.Premium_Refund }, // Prim İadesi
                new SelectListItem() { Value = "8", Text = babonline.Commission + " " + babonline.Accrual }, // Komisyon Tahakkuk
                new SelectListItem() { Value = "9", Text = babonline.Commission_Payment }, // Komisyon Ödemesi
                new SelectListItem() { Value = "10", Text = babonline.Purchase_Invoice }, // Alış Faturası
                new SelectListItem() { Value = "11", Text = babonline.Sales_Invoice } // Satış Faturası
            });

            return list;
        }
    }
}