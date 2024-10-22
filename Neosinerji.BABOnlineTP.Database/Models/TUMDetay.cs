using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TUMDetay
    {
        public TUMDetay()
        {
            this.TUMUrunleris = new List<TUMUrunleri>();
            this.TVMUrunYetkileris = new List<TVMUrunYetkileri>();
        }

        public int Kodu { get; set; }
        public string Unvani { get; set; }
        public string BirlikKodu { get; set; }
        public string VergiDairesi { get; set; }
        public string VergiNumarasi { get; set; }
        public byte Durum { get; set; }
        public Nullable<System.DateTime> DurumGuncellemeTarihi { get; set; }
        public Nullable<System.DateTime> TUMBaslangicTarihi { get; set; }
        public Nullable<System.DateTime> TUMBitisTarihi { get; set; }
        public string Telefon { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebAdresi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public Nullable<short> IlceKodu { get; set; }
        public string Semt { get; set; }
        public string Adres { get; set; }
        public byte BaglantiSiniri { get; set; }
        public Nullable<short> UcretlendirmeKodu { get; set; }
        public string Banner { get; set; }
        public string Logo { get; set; }
        public byte? UygulamaKodu { get; set; }
        public virtual ICollection<TUMUrunleri> TUMUrunleris { get; set; }
        public virtual ICollection<TVMUrunYetkileri> TVMUrunYetkileris { get; set; }
    }
}
