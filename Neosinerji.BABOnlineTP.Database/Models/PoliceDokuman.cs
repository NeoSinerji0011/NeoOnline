using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceDokuman
    {
        public int Id { get; set; }
        public Nullable<int> Police_ID { get; set; }
        public string DokumanAdi { get; set; }
        public string DokumanURL { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public int TVMKodu { get; set; }
        public string TVMKullaniciKodu { get; set; }
    }
}
