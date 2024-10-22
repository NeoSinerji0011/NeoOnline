using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class KaskoYurticiTasiyiciKademeleri
    {
        public int Id { get; set; }
        public int TUMKodu { get; set; }
        public string KullanimTarziKodu { get; set; }
        public string KullanimTarziKodu2 { get; set; }
        public string Kademe { get; set; }
        public Nullable<decimal> YillikLimit { get; set; }
        public Nullable<decimal> SeferLimiti { get; set; }
        public Nullable<int> Prim { get; set; }
        public string KullanimTarziAciklama { get; set; }
        public string KullanimTarziAciklama2 { get; set; }
    }
}
