using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMBankaHesaplari
    {
        public int TVMKodu { get; set; }
        public int SiraNo { get; set; }
        public string BankaKodu { get; set; }
        public string BankaAdi { get; set; }
        public string SubeKodu { get; set; }
        public string SubeAdi { get; set; }
        public string HesapNo { get; set; }
        public string IBAN { get; set; }
        public string HesapAdi { get; set; }
        public string AcenteKrediKartiNo { get; set; }
        public string CariHesapNo { get; set; }
        public Nullable<int> HesapTipi { get; set; }

    }
}
