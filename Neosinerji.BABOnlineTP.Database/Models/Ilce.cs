using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Ilce
    {
        public int IlceKodu { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public string IlceAdi { get; set; }
        public virtual Il Il { get; set; }
        public virtual Ulke Ulke { get; set; }
    }
}
