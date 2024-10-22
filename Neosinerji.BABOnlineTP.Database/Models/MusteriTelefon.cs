using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class MusteriTelefon
    {
        public int MusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public short IletisimNumaraTipi { get; set; }
        public string Numara { get; set; }
        public string NumaraSahibi { get; set; }
        public virtual MusteriGenelBilgiler MusteriGenelBilgiler { get; set; }
    }
}
