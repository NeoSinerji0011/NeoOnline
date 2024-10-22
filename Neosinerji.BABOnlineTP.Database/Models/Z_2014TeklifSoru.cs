using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_2014TeklifSoru
    {
        public int TableId { get; set; }
        public int TeklifId { get; set; }
        public int SoruKodu { get; set; }
        public byte CevapTipi { get; set; }
        public string Cevap { get; set; }
    }
}
