using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarZorunluEvrakListesi
    {
        public int EvrakKodu { get; set; }
        public string EvrakAdi { get; set; }
        public Nullable<short> Durum { get; set; }
    }
}
