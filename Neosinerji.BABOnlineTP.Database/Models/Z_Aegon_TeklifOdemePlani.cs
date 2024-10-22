using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_Aegon_TeklifOdemePlani
    {
        public int OdemePlaniId { get; set; }
        public int TeklifId { get; set; }
        public int TaksitNo { get; set; }
        public Nullable<System.DateTime> VadeTarihi { get; set; }
        public Nullable<decimal> TaksitTutari { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
    }
}
