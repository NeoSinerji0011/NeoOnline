using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_2014TeklifAracEkSoru
    {
        public int TableEkSoruId { get; set; }
        public int TeklifAracEkSoruId { get; set; }
        public int TeklifId { get; set; }
        public int TUMKodu { get; set; }
        public string SoruTipi { get; set; }
        public string SoruKodu { get; set; }
        public string Aciklama { get; set; }
        public Nullable<decimal> Bedel { get; set; }
        public Nullable<decimal> Fiyat { get; set; }
    }
}
