using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMAcentelikleri
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public int BransKodu { get; set; }
        public string SigortaSirketKodu { get; set; }
        public int OdemeTipi { get; set; }
        public byte Durum { get; set; }
    }
}
