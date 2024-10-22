using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class NeoConnectSirketGrupKullaniciDetay
    {
        public int GrupKodu { get; set; }
        public string GrupAdi { get; set; }
        public string SirketKodu { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public int TvmKodu { get; set; }
    }
}
