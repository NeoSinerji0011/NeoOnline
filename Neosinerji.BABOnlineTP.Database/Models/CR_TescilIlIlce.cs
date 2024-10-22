using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CR_TescilIlIlce
    {
        public int TUMKodu { get; set; }
        public string IlKodu { get; set; }
        public string IlceKodu { get; set; }
        public string TescilIlAdi { get; set; }
        public string TescilIlceAdi { get; set; }
    }
}
