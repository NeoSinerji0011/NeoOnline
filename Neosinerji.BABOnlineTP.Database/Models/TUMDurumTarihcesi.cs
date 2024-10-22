using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMDurumTarihcesi
    {
        public int TUMKodu { get; set; }
        public int SiraNo { get; set; }
        public byte DurumKodu { get; set; }
        public System.DateTime BaslamaTarihi { get; set; }
        public System.DateTime BitisTarihi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
    }
}
