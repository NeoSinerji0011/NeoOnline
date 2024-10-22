using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceVergi
    {
        public int PoliceId { get; set; }
        public int VergiKodu { get; set; }
        public Nullable<decimal> VergiTutari { get; set; }
        public virtual PoliceGenel PoliceGenel { get; set; }
    }
}
