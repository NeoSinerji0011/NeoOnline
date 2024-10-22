using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracTrafikTeminat
    {
        public string AracGrupKodu { get; set; }
        public System.DateTime GecerlilikBaslamaTarihi { get; set; }
        public string AracGrubu { get; set; }
        public Nullable<decimal> MaddiAracBasina { get; set; }
        public Nullable<decimal> MaddiKazaBasina { get; set; }
        public Nullable<decimal> TedaviKisiBasina { get; set; }
        public Nullable<decimal> TedaviKazaBasina { get; set; }
        public Nullable<decimal> SakatlikKisiBasina { get; set; }
        public Nullable<decimal> SakatlikKazaBasina { get; set; }
    }
}
