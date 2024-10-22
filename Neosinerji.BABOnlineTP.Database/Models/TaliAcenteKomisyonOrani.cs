using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TaliAcenteKomisyonOrani
    {
        public int KomisyonOranId { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public Nullable<int> TaliTVMKodu { get; set; }
        public string SigortaSirketKodu { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public Nullable<System.DateTime> GecirlilikBaslangicTarihi { get; set; }
        public Nullable<int> KomisyonOrani { get; set; }
        public Nullable<decimal> KomisyonOran { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public Nullable<int> KayitKullaniciKodu { get; set; }
        public Nullable<System.DateTime> GuncellemeTarihi { get; set; }
        public Nullable<int> GuncellemeKullaniciKodu { get; set; }
        public Nullable<decimal> MinUretim { get; set; }
        public Nullable<decimal> MaxUretim { get; set; }
        public Nullable<int> DisKaynakKodu { get; set; }
    }
}
