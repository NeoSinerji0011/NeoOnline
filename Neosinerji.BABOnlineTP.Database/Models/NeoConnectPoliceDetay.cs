using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class NeoConnectPoliceDetay
    {
        public int Id { get; set; }
        public Nullable<int> TvmKodu { get; set; }
        public Nullable<int> AltTvmKodu { get; set; }
        public Nullable<int> KullaniciKodu { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<int> YenilemeNo { get; set; }
        public Nullable<int> EkNo { get; set; }
        public string SirketNo { get; set; }
    }
}
