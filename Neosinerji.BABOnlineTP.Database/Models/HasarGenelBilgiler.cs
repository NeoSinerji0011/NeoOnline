using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class HasarGenelBilgiler
    {
        public HasarGenelBilgiler()
        {
            this.HasarBankaHesaplaris = new List<HasarBankaHesaplari>();
            this.HasarEksperIslemleris = new List<HasarEksperIslemleri>();
            this.HasarIletisimYetkilileris = new List<HasarIletisimYetkilileri>();
            this.HasarNotlaris = new List<HasarNotlari>();
            this.HasarZorunluEvraklaris = new List<HasarZorunluEvraklari>();
        }

        public int HasarId { get; set; }
        public int PoliceId { get; set; }
        public string BransKodu { get; set; }
        public string UrunKodu { get; set; }
        public string SigortaSirketNo { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string ZeyilNo { get; set; }
        public string SigortaliTCVKN { get; set; }
        public string PlakaNo { get; set; }
        public int TVMKodu { get; set; }
        public Nullable<int> AltTVMKodu { get; set; }
        public int KullaniciKodu { get; set; }
        public string HasarDosyaNo { get; set; }
        public System.DateTime IhbarTarihi { get; set; }
        public System.DateTime HasarTarihi { get; set; }
        public string HasarSaati { get; set; }
        public string HasarMevki { get; set; }
        public string HasarMevkiEnlemKodu { get; set; }
        public string HasarMevkiBoylamKodu { get; set; }
        public string HasarTuruNedeni { get; set; }
        public Nullable<short> HasarDosyaDurumu { get; set; }
        public string RedNedeni { get; set; }
        public Nullable<int> DosyayaAtananMTKodu { get; set; }
        public Nullable<int> AnlasmasiServisKodu { get; set; }
        public short KayitTipi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public virtual ICollection<HasarBankaHesaplari> HasarBankaHesaplaris { get; set; }
        public virtual ICollection<HasarEksperIslemleri> HasarEksperIslemleris { get; set; }
        public virtual PoliceGenel PoliceGenel { get; set; }
        public virtual ICollection<HasarIletisimYetkilileri> HasarIletisimYetkilileris { get; set; }
        public virtual ICollection<HasarNotlari> HasarNotlaris { get; set; }
        public virtual ICollection<HasarZorunluEvraklari> HasarZorunluEvraklaris { get; set; }
    }
}
