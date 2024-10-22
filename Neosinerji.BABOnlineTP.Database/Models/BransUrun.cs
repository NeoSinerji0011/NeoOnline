using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class BransUrun
    {
        public int BransUrunId { get; set; }
        public string SigortaSirketBirlikKodu { get; set; }
        public string SigortaSirketUrunKodu { get; set; }
        public string SigortaSirketUrunAdi { get; set; }
        public string SigortaSirketBransKodu { get; set; }
        public string SigortaSirketBransAdi { get; set; }
        public Nullable<int> BransKodu { get; set; }
    }
}
