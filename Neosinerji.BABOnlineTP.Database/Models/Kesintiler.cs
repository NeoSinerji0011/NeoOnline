using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Kesintiler
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public Nullable<int> TVMKoduTali { get; set; }
        public int Donem { get; set; }
        public int KesintiKodu { get; set; }
        public Nullable<decimal> Borc1 { get; set; }
        public Nullable<decimal> Alacak1 { get; set; }
        public Nullable<decimal> Borc2 { get; set; }
        public Nullable<decimal> Alacak2 { get; set; }
        public Nullable<decimal> Borc3 { get; set; }
        public Nullable<decimal> Alacak3 { get; set; }
        public Nullable<decimal> Borc4 { get; set; }
        public Nullable<decimal> Alacak4 { get; set; }
        public Nullable<decimal> Borc5 { get; set; }
        public Nullable<decimal> Alacak5 { get; set; }
        public Nullable<decimal> Borc6 { get; set; }
        public Nullable<decimal> Alacak6 { get; set; }
        public Nullable<decimal> Borc7 { get; set; }
        public Nullable<decimal> Alacak7 { get; set; }
        public Nullable<decimal> Borc8 { get; set; }
        public Nullable<decimal> Alacak8 { get; set; }
        public Nullable<decimal> Borc9 { get; set; }
        public Nullable<decimal> Alacak9 { get; set; }
        public Nullable<decimal> Borc10 { get; set; }
        public Nullable<decimal> Alacak10 { get; set; }
        public Nullable<decimal> Borc11 { get; set; }
        public Nullable<decimal> Alacak11 { get; set; }
        public Nullable<decimal> Borc12 { get; set; }
        public Nullable<decimal> Alacak12 { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public Nullable<System.DateTime> GuncellemeTarihi { get; set; }
        public virtual KesintiTurleri KesintiTurleri { get; set; }
    }
}
