using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMUrunYetkileri
    {
        public int TVMKodu { get; set; }
        public int BABOnlineUrunKodu { get; set; }
        public int TUMKodu { get; set; }
        public string TUMUrunKodu { get; set; }
        public byte Teklif { get; set; }
        public byte Police { get; set; }
        public byte Rapor { get; set; }
        public byte ManuelHavale { get; set; }
        public byte HavaleEntegrasyon { get; set; }
        public byte KrediKartiTahsilat { get; set; }
        public byte AcikHesapTahsilatGercek { get; set; }
        public byte AcikHesapTahsilatTuzel { get; set; }
        public virtual TUMDetay TUMDetay { get; set; }
        public virtual TVMDetay TVMDetay { get; set; }
        public virtual Urun Urun { get; set; }
    }
}
