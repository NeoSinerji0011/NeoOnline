using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMBankaHesaplari
    {
        public int TUMKodu { get; set; }
        public int SiraNo { get; set; }
        public string BankaKodu { get; set; }
        public string BankaAdi { get; set; }
        public string SubeKodu { get; set; }
        public string SubeAdi { get; set; }
        public string HesapNo { get; set; }
        public string IBAN { get; set; }
        public string HesapAdi { get; set; }
    }
}
