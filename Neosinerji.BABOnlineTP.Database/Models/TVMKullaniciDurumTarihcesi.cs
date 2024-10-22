using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMKullaniciDurumTarihcesi
    {
        public int TVMKullaniciKodu { get; set; }
        public int SiraNo { get; set; }
        public byte Durum { get; set; }
        public System.DateTime BaslamaTarihi { get; set; }
        public System.DateTime BitisTarihi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
