using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PotansiyelMusteriTelefon
    {
        public int PotansiyelMusteriKodu { get; set; }
        public int SiraNo { get; set; }
        public short IletisimNumaraTipi { get; set; }
        public string Numara { get; set; }
        public string NumaraSahibi { get; set; }
        public virtual PotansiyelMusteriGenelBilgiler PotansiyelMusteriGenelBilgiler { get; set; }
    }
}
