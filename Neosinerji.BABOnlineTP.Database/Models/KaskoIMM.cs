using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class KaskoIMM
    {
        public int Id { get; set; }
        public string KullanimTarziKodu { get; set; }
        public string Kod2 { get; set; }
        public Nullable<decimal> BedeniSahis { get; set; }
        public Nullable<decimal> BedeniKaza { get; set; }
        public Nullable<decimal> Maddi { get; set; }
        public Nullable<decimal> Kombine { get; set; }
        public string Text { get; set; }
    }
}
