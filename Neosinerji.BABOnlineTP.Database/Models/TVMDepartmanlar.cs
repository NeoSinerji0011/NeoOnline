using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMDepartmanlar
    {
        public int TVMKodu { get; set; }
        public int DepartmanKodu { get; set; }
        public string Adi { get; set; }
        public Nullable<int> BolgeKodu { get; set; }
        public byte MerkezYetkisi { get; set; }
        public byte Durum { get; set; }
    }
}
