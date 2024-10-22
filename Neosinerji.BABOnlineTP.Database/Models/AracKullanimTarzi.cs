using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class AracKullanimTarzi
    {
        public string KullanimTarziKodu { get; set; }
        public string Kod2 { get; set; }
        public Nullable<short> KullanimSekliKodu { get; set; }
        public string KullanimTarzi { get; set; }
        public Nullable<byte> Durum { get; set; }
    }
}
