using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class OfflinePoliceNumara
    {
        public int TVMKodu { get; set; }
        public int UrunKodu { get; set; }
        public long Baslangic { get; set; }
        public long Bitis { get; set; }
        public long Numara { get; set; }
    }
}
