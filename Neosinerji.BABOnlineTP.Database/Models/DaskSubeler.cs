using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DaskSubeler
    {
        public int KurumKodu { get; set; }
        public int SubeKodu { get; set; }
        public string SubeAdi { get; set; }
        public virtual DaskKurumlar DaskKurumlar { get; set; }
    }
}
