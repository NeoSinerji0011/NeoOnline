using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceTaliAcenteler
    {
        public int Id { get; set; }
        public Nullable<int> TVMKodu { get; set; }
        public string KimlikNo { get; set; }
        public string AdUnvan_ { get; set; }
        public string SoyadUnvan { get; set; }
        public string PoliceNo { get; set; }
        public Nullable<int> EkNo { get; set; }
        public string SigortaSirketNo_ { get; set; }
        public Nullable<System.DateTime> KayitTarihi_ { get; set; }
        public Nullable<System.DateTime> GuncellemeTarihi { get; set; }
        public Nullable<byte> PoliceTransferEslestimi { get; set; }
    }
}
