using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class MapfreKullanici
    {
        public int MapfeKullaniciId { get; set; }
        public string Bolge { get; set; }
        public string TVMUnvan { get; set; }
        public string AnaPartaj { get; set; }
        public string TaliPartaj { get; set; }
        public string KullaniciAdi { get; set; }
        public string EMail { get; set; }
        public Nullable<bool> Olusturuldu { get; set; }
    }
}
