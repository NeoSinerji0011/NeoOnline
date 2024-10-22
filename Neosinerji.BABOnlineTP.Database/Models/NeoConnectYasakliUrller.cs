using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class NeoConnectYasakliUrller
    {
        public int id { get; set; }
        public int TvmKodu { get; set; }
        public int AltTvmKodu { get; set; }
        public int SigortaSirketKodu { get; set; }
        public string YasaklanacakUrl { get; set; }
    }
}
