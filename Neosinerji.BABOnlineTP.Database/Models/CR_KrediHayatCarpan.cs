using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_KrediHayatCarpan
    {
        public int TUMKodu { get; set; }
        public string TipKodu { get; set; }
        public int MusteriYas { get; set; }
        public int KrediVade { get; set; }
        public Nullable<decimal> Carpan { get; set; }
    }
}
