using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class X_AracDegerAraTable
    {
        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public string MarkaAdi { get; set; }
        public string TipAdi { get; set; }
        public int Yil { get; set; }
        public decimal Fiyat { get; set; }
    }
}
