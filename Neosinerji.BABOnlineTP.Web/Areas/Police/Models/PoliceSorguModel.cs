using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Police.Models
{
    public class PoliceSorguModel
    {
        public bool TeklifMi { get; set; }
        public bool PoliceMi { get; set; }
        public string KimlikNo { get; set; }
        public string PlakaKodu { get; set; }
        public string PlakaNo { get; set; }
        public int SorguTipi { get; set; }

        public SelectList SorguTipleri { get; set; }
    }
}