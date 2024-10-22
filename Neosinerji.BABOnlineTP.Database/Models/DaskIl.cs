using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DaskIl
    {
        public DaskIl()
        {
            this.DaskBeldes = new List<DaskBelde>();
            this.DaskIlces = new List<DaskIlce>();
        }

        public int IlKodu { get; set; }
        public string IlAdi { get; set; }
        public virtual ICollection<DaskBelde> DaskBeldes { get; set; }
        public virtual ICollection<DaskIlce> DaskIlces { get; set; }
    }
}
