using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Models
{
    public class NeoConnectYasakliUrlModels
    {
        public int TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public List<SelectListItem> TVMListesi { get; set; }
        public int Id { get; set; }
        public string YasaklanacakUrl { get; set; }
        public string SigortaSirketKodu { get; set; }
        public string TUMUnvan { get; set; }
        public string SigortaSirketAdi { get; set; }
        public List<SelectListItem> TUMListesi { get; set; }
        public int AltTvmKodu { get; set; }
        public string AltTvmUnvan { get; set; }
        public MultiSelectList SatisKanallari { get; set; }
        public string[] SatisKanallariSelectList { get; set; }

        public string SatisKanali { get; set; }
    }
}