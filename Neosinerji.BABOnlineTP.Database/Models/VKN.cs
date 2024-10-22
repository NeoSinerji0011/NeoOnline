using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class VKN
    {
        public Nullable<System.DateTime> DogumTarihi { get; set; }
        public string BabaAdi { get; set; }
        public string Durum { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string VergiNo { get; set; }
        public string SirketTuru { get; set; }
        public string DogumYeri { get; set; }
    }
}
