using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMSMSKullaniciBilgi
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string Gonderen { get; set; }
        public short SmsSuresiDK { get; set; }
    }
}
