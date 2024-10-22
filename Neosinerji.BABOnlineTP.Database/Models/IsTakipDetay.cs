using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class IsTakipDetay
    {
        public IsTakipDetay()
        {
            this.IsTakipSorus = new List<IsTakipSoru>();
        }

        public int IsTakipDetayId { get; set; }
        public int IsTakipId { get; set; }
        public int IsTipiDetayId { get; set; }
        public int TvmKullaniciId { get; set; }
        public int HareketTipi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public virtual IsTakip IsTakip { get; set; }
        public virtual ICollection<IsTakipSoru> IsTakipSorus { get; set; }
    }
}
