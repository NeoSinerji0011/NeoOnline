using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Ulke
    {
        public Ulke()
        {
            this.Ils = new List<Il>();
            this.Ilces = new List<Ilce>();
        }

        public string UlkeKodu { get; set; }
        public string UlkeAdi { get; set; }
        public virtual ICollection<Il> Ils { get; set; }
        public virtual ICollection<Ilce> Ilces { get; set; }
    }
}
