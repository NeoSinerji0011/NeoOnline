using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMUrunleri
    {
        public int TUMKodu { get; set; }
        public string TUMUrunKodu { get; set; }
        public string TUMUrunAdi { get; set; }
        public string TUMBransKodu { get; set; }
        public string TUMBransAdi { get; set; }
        public int BABOnlineUrunKodu { get; set; }
        public virtual TUMDetay TUMDetay { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
