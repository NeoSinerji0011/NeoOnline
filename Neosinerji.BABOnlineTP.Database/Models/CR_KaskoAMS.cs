using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_KaskoAMS
    {
        public int TUMKodu { get; set; }
        public int TVMKodu { get; set; }
        public string AMSKodu { get; set; }
        public string KullanimTarziKodu { get; set; }
        public System.DateTime GecerliOlduguTarih { get; set; }
        public Nullable<decimal> BedeniSahis { get; set; }
        public string Aciklama { get; set; }
    }
}
