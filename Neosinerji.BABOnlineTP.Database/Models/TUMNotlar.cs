using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMNotlar
    {
        public int TUMKodu { get; set; }
        public int SiraNo { get; set; }
        public string KonuAdi { get; set; }
        public System.DateTime EklemeTarihi { get; set; }
        public int EkleyenPersonelKodu { get; set; }
        public string NotAciklamasi { get; set; }
    }
}
