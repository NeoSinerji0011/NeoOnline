using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracTipTest
    {
        public int Id { get; set; }
        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public string TipAdi { get; set; }
        public string KullanimSekli1 { get; set; }
        public string KullanimSekli2 { get; set; }
        public string KullanimSekli3 { get; set; }
        public string KullanimSekli4 { get; set; }
        public string KisiSayisi { get; set; }
    }
}
