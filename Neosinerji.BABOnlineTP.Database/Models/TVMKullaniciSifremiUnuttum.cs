using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMKullaniciSifremiUnuttum
    {
        public int Id { get; set; }
        public int KullaniciKodu { get; set; }
        public string PasswordVerificationToken_ { get; set; }
        public System.DateTime SendDate { get; set; }
        public Nullable<System.DateTime> ResetDate { get; set; }
        public string EskiSifre { get; set; }
        public string YeniSifre { get; set; }
        public byte Status { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
