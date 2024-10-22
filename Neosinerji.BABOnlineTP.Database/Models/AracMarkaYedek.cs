using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracMarkaYedek
    {
        public AracMarkaYedek()
        {
            this.AracModelYedeks = new List<AracModelYedek>();
            this.AracTipYedeks = new List<AracTipYedek>();
        }

        public string MarkaKodu { get; set; }
        public string MarkaAdi { get; set; }
        public virtual ICollection<AracModelYedek> AracModelYedeks { get; set; }
        public virtual ICollection<AracTipYedek> AracTipYedeks { get; set; }
    }
}
