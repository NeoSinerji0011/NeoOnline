using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class MusteriNot
    {
        public int MusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public string Konu { get; set; }
        public string NotAciklamasi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public int TVMKodu { get; set; }
        public int TVMPersonelKodu { get; set; }
        public virtual MusteriGenelBilgiler MusteriGenelBilgiler { get; set; }
    }
}
