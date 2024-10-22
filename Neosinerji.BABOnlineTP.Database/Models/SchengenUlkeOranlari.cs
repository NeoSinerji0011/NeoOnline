using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class SchengenUlkeOranlari
    {
        public int Yas1 { get; set; }
        public int Yas2 { get; set; }
        public int Gun1 { get; set; }
        public int Gun2 { get; set; }
        public decimal Oran { get; set; }
        public Nullable<decimal> SuprimOrani { get; set; }
    }
}
