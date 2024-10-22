using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DaskBelde
    {
        public int BeldeKodu { get; set; }
        public int IlKodu { get; set; }
        public int IlceKodu { get; set; }
        public string BeldeAdi { get; set; }
        public virtual DaskIl DaskIl { get; set; }
    }
}
