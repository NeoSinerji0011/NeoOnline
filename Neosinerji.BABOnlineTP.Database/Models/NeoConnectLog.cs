using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class NeoConnectLog
    {
        public int LogId { get; set; }
        public Nullable<int> TvmKodu { get; set; }
        public Nullable<int> KullaniciKodu { get; set; }
        public string Kullanici { get; set; }
        public string SigortaSirketKodu { get; set; }
        public string IPAdresi { get; set; }
        public string MACAdresi { get; set; }
        public Nullable<System.DateTime> KullaniciGirisTarihi { get; set; }
        public Nullable<System.DateTime> KullaniciCikisTarihi { get; set; }
        public string SirketKullaniciAdi { get; set; }
        public string SirketKullaniciSifresi { get; set; }
        public Nullable<System.Int32>  GrupKodu { get; set; }
    }
}
