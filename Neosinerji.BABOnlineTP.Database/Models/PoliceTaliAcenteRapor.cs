using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceTaliAcenteRapor
    {
        public int Id { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public Nullable<byte> UretimVAR_YOK { get; set; }
        public Nullable<int> Police_EkAdedi { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public Nullable<System.DateTime> GuncellemeTarihi { get; set; }
        public Nullable<byte> GunKapamaDurumu { get; set; }
    }
}
