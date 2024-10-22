using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AutoPoliceTransfer
    {
        public int Id { get; set; }
        public int TvmKodu { get; set; }
        public string SirketKodu { get; set; }
        public string PoliceTransferUrl { get; set; }
        public System.DateTime TanzimBaslangicTarihi { get; set; }
        public System.DateTime TanzimBitisTarihi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public int KaydiEkleyenKullaniciKodu { get; set; }
    }
}
