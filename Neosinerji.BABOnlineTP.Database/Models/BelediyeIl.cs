using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class BelediyeIl
    {
        public BelediyeIl()
        {
            this.Belediyes = new List<Belediye>();
        }

        public int IlKodu { get; set; }
        public string Adi { get; set; }
        public virtual ICollection<Belediye> Belediyes { get; set; }
    }
}
