using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracModelYedek
    {
        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public int Model { get; set; }
        public decimal Fiyat { get; set; }
        public virtual AracMarkaYedek AracMarkaYedek { get; set; }
    }
}
