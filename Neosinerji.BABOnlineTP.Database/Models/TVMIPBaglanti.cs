using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMIPBaglanti
    {
        public int TVMKodu { get; set; }
        public int SiraNo { get; set; }
        public string BaslangicIP { get; set; }
        public string BitisIP { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public byte Durum { get; set; }
    }
}
