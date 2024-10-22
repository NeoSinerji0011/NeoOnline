using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Bran
    {
        public Bran()
        {
            this.Uruns = new List<Urun>();
        }

        public int BransKodu { get; set; }
        public string BransAdi { get; set; }
        public byte Durum { get; set; }
        public virtual ICollection<Urun> Uruns { get; set; }
    }
}
