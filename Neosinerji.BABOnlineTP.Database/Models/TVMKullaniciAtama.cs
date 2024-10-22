using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMKullaniciAtama
    {
        public int TVMKullaniciKodu { get; set; }
        public int AtamaNo { get; set; }
        public int AtandigiDepartmanKodu { get; set; }
        public int OncekiDepartmanKodu { get; set; }
        public System.DateTime BaslamaTarihi { get; set; }
        public System.DateTime BitisTarihi { get; set; }
        public System.DateTime AtamaTarihi { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
