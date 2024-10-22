using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Soru
    {
        public Soru()
        {
            this.UrunSorus = new List<UrunSoru>();
        }

        public int SoruKodu { get; set; }
        public string SoruAdi { get; set; }
        public short SoruCevapTipi { get; set; }
        public short SoruCevapUzunlugu { get; set; }
        public virtual ICollection<UrunSoru> UrunSorus { get; set; }
    }
}
