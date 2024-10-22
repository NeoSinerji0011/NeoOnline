using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Teminat
    {
        public Teminat()
        {
            this.UrunTeminats = new List<UrunTeminat>();
        }

        public int TeminatKodu { get; set; }
        public string TeminatAdi { get; set; }
        public virtual ICollection<UrunTeminat> UrunTeminats { get; set; }
    }
}
