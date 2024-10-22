using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMKullaniciNotlar
    {
        public int KullaniciNotId { get; set; }
        public int KullaniciKodu { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public System.DateTime EklemeTarihi { get; set; }
        public Nullable<System.DateTime> DegistirmeTarihi { get; set; }
        public byte Oncelik { get; set; }
        public Nullable<System.DateTime> BitisTarihi { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
