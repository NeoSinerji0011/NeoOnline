using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class RaporMapfreBolge
    {
        public int TVMKodu { get; set; }
        public System.DateTime Gun { get; set; }
        public Nullable<int> TrafikTeklif { get; set; }
        public Nullable<int> TrafikPolice { get; set; }
        public Nullable<decimal> TrafikBrutPrim { get; set; }
        public Nullable<decimal> TrafikKomisyon { get; set; }
        public Nullable<int> KaskoTeklif { get; set; }
        public Nullable<int> KaskoPolice { get; set; }
        public Nullable<decimal> KaskoBrutPrim { get; set; }
        public Nullable<decimal> KaskoKomisyon { get; set; }
    }
}
