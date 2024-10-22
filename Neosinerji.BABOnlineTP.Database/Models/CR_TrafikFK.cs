using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_TrafikFK
    {
        public int TumKodu { get; set; }
        public string KullanimTarziKodu { get; set; }
        public string Kod2 { get; set; }
        public short Kademe { get; set; }
        public decimal Vefat { get; set; }
        public decimal Sakatlik { get; set; }
        public decimal Tedavi { get; set; }
        public Nullable<decimal> Kombine { get; set; }
        public string Text { get; set; }
    }
}
