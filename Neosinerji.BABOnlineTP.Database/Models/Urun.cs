using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Urun
    {
        public Urun()
        {
            this.TUMUrunleris = new List<TUMUrunleri>();
            this.TVMUrunYetkileris = new List<TVMUrunYetkileri>();
            this.UrunSorus = new List<UrunSoru>();
            this.UrunTeminats = new List<UrunTeminat>();
            this.UrunVergis = new List<UrunVergi>();
        }

        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public int BransKodu { get; set; }
        public byte Durum { get; set; }
        public virtual Bran Bran { get; set; }
        public virtual ICollection<TUMUrunleri> TUMUrunleris { get; set; }
        public virtual ICollection<TVMUrunYetkileri> TVMUrunYetkileris { get; set; }
        public virtual ICollection<UrunSoru> UrunSorus { get; set; }
        public virtual ICollection<UrunTeminat> UrunTeminats { get; set; }
        public virtual ICollection<UrunVergi> UrunVergis { get; set; }
    }
}
