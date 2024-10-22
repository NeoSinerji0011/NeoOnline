using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifVergi
    {
        public int TeklifId { get; set; }
        public int VergiKodu { get; set; }
        public Nullable<decimal> VergiTutari { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
}
