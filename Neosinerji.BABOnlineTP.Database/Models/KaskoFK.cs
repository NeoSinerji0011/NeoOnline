using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class KaskoFK
    {
        public int Id { get; set; }
        public string KullanimTarziKodu { get; set; }
        public string Kod2 { get; set; }
        public Nullable<decimal> Vefat { get; set; }
        public Nullable<decimal> Sakatlik { get; set; }
        public Nullable<decimal> Tedavi { get; set; }
        public Nullable<decimal> Kombine { get; set; }
        public string Text { get; set; }
    }
}
