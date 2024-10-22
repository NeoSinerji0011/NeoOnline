using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarBankaHesaplari
    {
        public int BankaHesapId { get; set; }
        public int HasarId { get; set; }
        public string BankaKodu { get; set; }
        public string BankaAdi { get; set; }
        public string SubeKodu { get; set; }
        public string SubeAdi { get; set; }
        public string HesapNo { get; set; }
        public string IBAN { get; set; }
        public string HesapAdi { get; set; }
        public virtual HasarGenelBilgiler HasarGenelBilgiler { get; set; }
    }
}
