using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DuyuruTVM
    {
        public int DuyuruTvmId { get; set; }
        public int DuyuruId { get; set; }
        public int TVMId { get; set; }
        public int EkleyenKullanici { get; set; }
        public System.DateTime EklemeTarihi { get; set; }
        public virtual Duyurular Duyurular { get; set; }
        public virtual TVMDetay TVMDetay { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
