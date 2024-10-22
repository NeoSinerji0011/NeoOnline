using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_2014TeklifTeminat
    {
        public int TableId { get; set; }
        public int TeklifId { get; set; }
        public int TeminatKodu { get; set; }
        public Nullable<decimal> TeminatBedeli { get; set; }
        public Nullable<decimal> TeminatVergi { get; set; }
        public Nullable<decimal> TeminatNetPrim { get; set; }
        public Nullable<decimal> TeminatBrutPrim { get; set; }
        public Nullable<decimal> Komisyon_ { get; set; }
        public Nullable<int> Adet { get; set; }
    }
}
