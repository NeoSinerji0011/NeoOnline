using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Vergi
    {
        public Vergi()
        {
            this.UrunVergis = new List<UrunVergi>();
        }

        public int VergiKodu { get; set; }
        public string VergiAdi { get; set; }
        public virtual ICollection<UrunVergi> UrunVergis { get; set; }
    }
}
