using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracModel
    {
        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public int Model { get; set; }
        public Nullable<decimal> Fiyat { get; set; }
        public virtual AracMarka AracMarka { get; set; }
    }
}
