using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMWebServisKullanicilari
    {
        public int TVMKodu { get; set; }
        public int TUMKodu { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public string KullaniciAdi2 { get; set; }
        public string Sifre2 { get; set; }
        public string PartajNo_ { get; set; }
        public string SubAgencyCode { get; set; }
        public string SourceId { get; set; }
        public string CompanyId { get; set; }
    }
}
