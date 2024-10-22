using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_KaskoIMM
    {
        public int TUMKodu { get; set; }
        public string KullanimTarziKodu { get; set; }
        public string Kod2 { get; set; }
        public string Kademe { get; set; }
        public Nullable<decimal> BedeniSahis { get; set; }
        public Nullable<decimal> BedeniKaza { get; set; }
        public Nullable<decimal> Maddi { get; set; }
        public Nullable<byte> Durum { get; set; }
        public Nullable<decimal> Kombine { get; set; }
        public string Text { get; set; }
    }
}
