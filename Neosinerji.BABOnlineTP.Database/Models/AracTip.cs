using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracTip
    {
        public string MarkaKodu { get; set; }
        public string TipKodu { get; set; }
        public string TipAdi { get; set; }
        public string KullanimSekli1 { get; set; }
        public string KullanimSekli2 { get; set; }
        public string KullanimSekli3 { get; set; }
        public string KullanimSekli4 { get; set; }
        public Nullable<short> KisiSayisi { get; set; }
        public virtual AracMarka AracMarka { get; set; }
    }
}
