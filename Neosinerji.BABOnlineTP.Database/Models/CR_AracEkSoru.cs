using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_AracEkSoru
    {
        public int TUMKodu { get; set; }
        public string SoruTipi { get; set; }
        public string SoruKodu { get; set; }
        public string SoruAdi { get; set; }
    }
}
