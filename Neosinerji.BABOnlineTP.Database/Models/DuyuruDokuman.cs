using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class DuyuruDokuman
    {
        public int DuyuruDokumanId { get; set; }
        public int DuyuruId { get; set; }
        public string DosyaAdi { get; set; }
        public string DokumanURL { get; set; }
        public int EkleyenKullanici { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public virtual Duyurular Duyurular { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
    }
}
