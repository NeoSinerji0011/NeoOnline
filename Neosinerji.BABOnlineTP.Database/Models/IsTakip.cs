using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsTakip
    {
        public IsTakip()
        {
            this.IsTakipDetays = new List<IsTakipDetay>();
            this.IsTakipDokumen = new List<IsTakipDokuman>();
        }

        public int IsTakipId { get; set; }
        public int TeklifId { get; set; }
        public int Asama { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public int TVMKullaniciId { get; set; }
        public int MusteriKodu { get; set; }
        public virtual ICollection<IsTakipDetay> IsTakipDetays { get; set; }
        public virtual ICollection<IsTakipDokuman> IsTakipDokumen { get; set; }
    }
}
