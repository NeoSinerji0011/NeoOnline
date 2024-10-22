using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMKullaniciSifreTarihcesi
    {
        public int TVMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public int SifreDegistirmeNo { get; set; }
        public System.DateTime DegistirmeTarihi { get; set; }
        public string OncekiSifre { get; set; }
        public string YeniSifre { get; set; }
    }
}
