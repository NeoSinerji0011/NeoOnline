using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_TrafikIMM
    {
        public int TUMKodu { get; set; }
        public string KullanimTarziKodu { get; set; }
        public string Kod2 { get; set; }
        public short Kademe { get; set; }
        public decimal BedeniSahis { get; set; }
        public decimal BedeniKaza { get; set; }
        public decimal Maddi { get; set; }
        public Nullable<decimal> Kombine { get; set; }
        public string Text { get; set; }
    }
}
