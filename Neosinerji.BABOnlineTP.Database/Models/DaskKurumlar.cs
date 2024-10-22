using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DaskKurumlar
    {
        public DaskKurumlar()
        {
            this.DaskSubelers = new List<DaskSubeler>();
        }

        public int KurumKodu { get; set; }
        public string KurumAdi { get; set; }
        public virtual ICollection<DaskSubeler> DaskSubelers { get; set; }
    }
}
