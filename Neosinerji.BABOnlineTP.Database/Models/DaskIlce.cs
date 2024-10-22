using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DaskIlce
    {
        public int IlceKodu { get; set; }
        public int IlKodu { get; set; }
        public string IlceAdi { get; set; }
        public virtual DaskIl DaskIl { get; set; }
    }
}
