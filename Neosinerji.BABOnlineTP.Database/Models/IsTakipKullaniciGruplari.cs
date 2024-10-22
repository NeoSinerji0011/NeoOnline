using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsTakipKullaniciGruplari
    {
        public IsTakipKullaniciGruplari()
        {
            this.IsTakipKullaniciGrupKullanicilaris = new List<IsTakipKullaniciGrupKullanicilari>();
        }

        public int KullaniciGrupId { get; set; }
        public string GrupAdi { get; set; }
        public string Aciklama { get; set; }
        public virtual ICollection<IsTakipKullaniciGrupKullanicilari> IsTakipKullaniciGrupKullanicilaris { get; set; }
    }
}
