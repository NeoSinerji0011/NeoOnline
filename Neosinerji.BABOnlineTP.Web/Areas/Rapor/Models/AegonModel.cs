using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models
{
    public class AegonTeklifRaporuModel
    {
        public Nullable<int> TeklifNo { get; set; }

        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        public int TeklifTarihi { get; set; }
        public int ParaBirimi { get; set; }
        public int? BolgeKodu { get; set; }
        public int? YillikPrimMin { get; set; }
        public int? YillikPrimMax { get; set; }

        public int[] UrunSelectList { get; set; }
        public int[] TVMLerSelectList { get; set; }

        public List<SelectListItem> TVMLerItems { get; set; }
        public MultiSelectList UrunlerItems { get; set; }

        public List<SelectListItem> BolgeList { get; set; }
        public List<SelectListItem> ParaBirimiList { get; set; }
        public List<SelectListItem> TeklifTarihiTipleri { get; set; }
        public List<TeklifRaporuProcedureModel> RaporSonuc { get; set; }
    }

    public class AegonRaporListProvider
    {
        public static List<SelectListItem> ParaBirimleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[]{
                new SelectListItem() { Value = "0" , Text = babonline.All},
                new SelectListItem() { Value = "1" , Text = "EUR"},
                new SelectListItem() { Value = "2" , Text = "USD"},
                new SelectListItem() { Value = "3" , Text = "TL"},
            });

            return list;
        }

        public static List<SelectListItem> TarihTipleri()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "1" , Text = "Teklif"},
                new SelectListItem() { Value = "2" , Text = babonline.Starting},
                new SelectListItem() { Value = "3" , Text = babonline.Finish}
            });

            return list;
        }
    }
}