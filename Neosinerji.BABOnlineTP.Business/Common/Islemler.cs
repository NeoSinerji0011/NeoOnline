using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class Islemler
    {
        public static List<SelectListItem> PoliceOnaylamaIslemTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text ="Police Onaylama" },
                new SelectListItem() { Value = "1", Text = "Hesaplanmis Poliçeler" }
            });

            return list;
        }

        public static List<SelectListItem> PoliceTransferIslemTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text ="Dosyadan Transfer" },
                new SelectListItem() { Value = "1", Text = "Otomatik Transfer" }
            });

            return list;
        }

        public static List<SelectListItem> PoliceTransferTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
              new SelectListItem() { Value = "", Text ="Lütfen Seçiniz" },
                new SelectListItem() { Value = "0", Text ="Poliçe Transfer" },
                new SelectListItem() { Value = "1", Text = "Tahsilat Kapatma" }
            });

            return list;
        }
        public static List<SelectListItem> MasrafGirisiIslemTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text ="Sorgula / Kaydet / Güncelle" },
                new SelectListItem() { Value = "1", Text = "Toplu Kaydet" }
            });

            return list;
        }
    }
    public class PoliceTransferTipi
    {
        public const string PoliceTransfer = "0";
        public const string TahsilatKapatma = "1";
    }
    public class PoliceIslemTipi
    {
        public const byte DosyadanTransfer = 0;
        public const byte OtomatikTransfer = 1;
    }
}
