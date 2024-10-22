using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class X_AracDegerListesi
    {
        public int TaskId { get; set; }
        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public string Model { get; set; }
        public string MarkaAdi { get; set; }
        public string TipAdi { get; set; }
        public string Fiyat { get; set; }
    }
}
