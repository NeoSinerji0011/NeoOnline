using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMBolgeleri
    {
        public int TVMKodu { get; set; }
        public int TVMBolgeKodu { get; set; }
        public string BolgeAdi { get; set; }
        public string Aciklama { get; set; }
        public byte Durum { get; set; }
    }
}
