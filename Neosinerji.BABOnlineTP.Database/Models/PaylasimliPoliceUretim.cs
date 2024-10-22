using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PaylasimliPoliceUretim
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public Nullable<int> TaliTVMKodu { get; set; }
        public string SigortaSirketNo { get; set; }
        public string BransKodu { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeylNo { get; set; }
        public Nullable<System.DateTime> TanzimTarihi { get; set; }
        public decimal BrutPrim { get; set; }
        public decimal NetPrim { get; set; }
        public Nullable<decimal> PoliceKomisyonTutari { get; set; }
        public Nullable<decimal> TvmKomisyonTutari { get; set; }
        public Nullable<int> TvmKomisyonOrani { get; set; }
        public Nullable<decimal> TaliTvmKomisyonTutari { get; set; }
        public Nullable<int> TaliTvmKomisyonOrani { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public System.DateTime GuncellemeTarihi { get; set; }
        public int KaydiEkleyenKullaniciKodu { get; set; }
    }
}
