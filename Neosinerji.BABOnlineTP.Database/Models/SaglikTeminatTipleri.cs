using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class SaglikTeminatTipleri
    {
        public SaglikTeminatTipleri()
        {
            this.SaglikTeminatPrimleris = new List<SaglikTeminatPrimleri>();
        }

        public string Kodu { get; set; }
        public string TeminatTipi { get; set; }
        public short Durum { get; set; }
        public virtual ICollection<SaglikTeminatPrimleri> SaglikTeminatPrimleris { get; set; }
    }
}
