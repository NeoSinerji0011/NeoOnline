using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMIPBaglanti
    {
        public int TUMKodu { get; set; }
        public int SiraNo { get; set; }
        public string BaslangicIP { get; set; }
        public string BitisIP { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public byte Durum { get; set; }
    }
}
