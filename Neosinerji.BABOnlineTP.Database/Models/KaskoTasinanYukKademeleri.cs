using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class KaskoTasinanYukKademeleri
    {
        public int Id { get; set; }
        public int TUMKodu { get; set; }
        public string KullanimTarziKodu { get; set; }
        public string KullanimTarziKodu2 { get; set; }
        public string Kademe { get; set; }
        public string Aciklama { get; set; }
    }
}
