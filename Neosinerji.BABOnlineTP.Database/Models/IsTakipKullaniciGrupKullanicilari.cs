using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsTakipKullaniciGrupKullanicilari
    {
        public int Id { get; set; }
        public int IsTakipKullaniciGrupId { get; set; }
        public int KullaniciKodu { get; set; }
        public virtual IsTakipKullaniciGruplari IsTakipKullaniciGruplari { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
