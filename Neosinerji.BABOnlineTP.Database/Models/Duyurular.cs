using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Duyurular
    {
        public Duyurular()
        {
            this.DuyuruDokumen = new List<DuyuruDokuman>();
            this.DuyuruTVMs = new List<DuyuruTVM>();
        }

        public int DuyuruId { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public int EkleyenKullanici { get; set; }
        public System.DateTime EklemeTarihi { get; set; }
        public Nullable<int> DegistirenKullanici { get; set; }
        public Nullable<System.DateTime> DegistirmeTarihi { get; set; }
        public byte Oncelik { get; set; }
        public System.DateTime BaslangisTarihi { get; set; }
        public System.DateTime BitisTarihi { get; set; }
        public virtual ICollection<DuyuruDokuman> DuyuruDokumen { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar1 { get; set; }
        public virtual ICollection<DuyuruTVM> DuyuruTVMs { get; set; }
    }
}
