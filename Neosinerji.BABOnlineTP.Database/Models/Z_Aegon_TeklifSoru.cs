using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_Aegon_TeklifSoru
    {
        public int TeklifId { get; set; }
        public int SoruKodu { get; set; }
        public byte CevapTipi { get; set; }
        public string Cevap { get; set; }
    }
}
