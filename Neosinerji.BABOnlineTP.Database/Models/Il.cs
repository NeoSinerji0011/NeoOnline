using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Il
    {
        public Il()
        {
            this.Ilces = new List<Ilce>();
        }

        public string IlKodu { get; set; }
        public string UlkeKodu { get; set; }
        public string IlAdi { get; set; }
        public virtual Ulke Ulke { get; set; }
        public virtual ICollection<Ilce> Ilces { get; set; }
    }
}
