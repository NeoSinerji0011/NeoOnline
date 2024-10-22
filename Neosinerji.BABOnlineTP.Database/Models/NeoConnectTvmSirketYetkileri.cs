using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class NeoConnectTvmSirketYetkileri
    {
        public int Id { get; set; }
        public int TvmKodu { get; set; }
        public int? TumKodu2 { get; set; }
        public string TumKodu { get; set; }
        public byte Durum { get; set; }
    }
}
