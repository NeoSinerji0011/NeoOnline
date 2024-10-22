using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceOdemePlani
    {
        public int PoliceId { get; set; }
        public int TaksitNo { get; set; }
        public Nullable<System.DateTime> VadeTarihi { get; set; }
        public Nullable<decimal> TaksitTutari { get; set; }
        public Nullable<decimal> DovizliTaksitTutari { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public virtual PoliceGenel PoliceGenel { get; set; }
    }
}
