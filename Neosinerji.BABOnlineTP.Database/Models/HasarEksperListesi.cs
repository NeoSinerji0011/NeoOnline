using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarEksperListesi
    {
        public int EksperKodu { get; set; }
        public string EksperAdSoyadUnvan { get; set; }
        public string CepTel { get; set; }
        public string IsTel { get; set; }
        public string Fax { get; set; }
        public string Adres { get; set; }
        public short Durumu { get; set; }
    }
}
