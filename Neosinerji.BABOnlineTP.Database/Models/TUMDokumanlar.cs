using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMDokumanlar
    {
        public int TUMKodu { get; set; }
        public int SiraNo { get; set; }
        public string DokumanTuru { get; set; }
        public System.DateTime EklemeTarihi { get; set; }
        public int EkleyenPersonelKodu { get; set; }
        public string Dokuman { get; set; }
    }
}
