using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class SaglikBolgeleri
    {
        public SaglikBolgeleri()
        {
            this.SaglikTeminatPrimleris = new List<SaglikTeminatPrimleri>();
        }

        public string Kodu { get; set; }
        public string BolgeAciklamasi { get; set; }
        public Nullable<short> Durum { get; set; }
        public virtual ICollection<SaglikTeminatPrimleri> SaglikTeminatPrimleris { get; set; }
    }
}
