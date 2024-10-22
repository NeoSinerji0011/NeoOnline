using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class MapfreBolgeRaporuIcmal
    {
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public Nullable<int> BolgeKodu { get; set; }
        public string BolgeAdi { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public Nullable<byte> Profili { get; set; }
        public Nullable<int> KaskoTeklif { get; set; }
        public Nullable<int> TrafikTeklif { get; set; }
        public Nullable<int> KaskoPolice { get; set; }
        public Nullable<int> TrafikPolice { get; set; }
        public Nullable<decimal> KaskoBrutPrim { get; set; }
        public Nullable<decimal> KaskoKomisyon { get; set; }
        public Nullable<decimal> TrafikBrutPrim { get; set; }
        public Nullable<decimal> TrafikKomisyon { get; set; }
        public int Id { get; set; }
    }
}
