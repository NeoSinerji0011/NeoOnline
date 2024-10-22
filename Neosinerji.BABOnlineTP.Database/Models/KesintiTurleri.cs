using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class KesintiTurleri
    {
        public KesintiTurleri()
        {
            this.Kesintilers = new List<Kesintiler>();
        }

        public int KesintiKodu { get; set; }
        public string KesintiAciklamasi { get; set; }
        public Nullable<System.DateTime> OdemeGunu { get; set; }
        public string YetkiliMail { get; set; }
        public int TVMKodu { get; set; }
        public virtual ICollection<Kesintiler> Kesintilers { get; set; }
    }
}
