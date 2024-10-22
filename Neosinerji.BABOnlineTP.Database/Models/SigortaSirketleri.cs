using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class SigortaSirketleri
    {
        public SigortaSirketleri()
        {
            this.PoliceGenels = new List<PoliceGenel>();
        }

        public string SirketKodu { get; set; }
        public string SirketAdi { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public string SirketLogo { get; set; }
        public byte? UygulamaKodu { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public virtual ICollection<PoliceGenel> PoliceGenels { get; set; }
    }
}
