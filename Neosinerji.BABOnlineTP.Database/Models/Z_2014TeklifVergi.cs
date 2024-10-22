using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_2014TeklifVergi
    {
        public int TableId { get; set; }
        public int TeklifId { get; set; }
        public int VergiKodu { get; set; }
        public Nullable<decimal> VergiTutari { get; set; }
    }
}
