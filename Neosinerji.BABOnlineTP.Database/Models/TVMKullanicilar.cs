using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TVMKullanicilar
    {
        public TVMKullanicilar()
        {
            this.DuyuruDokumen = new List<DuyuruDokuman>();
            this.Duyurulars = new List<Duyurular>();
            this.Duyurulars1 = new List<Duyurular>();
            this.DuyuruTVMs = new List<DuyuruTVM>();
            this.IsTakipKullaniciGrupKullanicilaris = new List<IsTakipKullaniciGrupKullanicilari>();
            this.OfflinePolice = new List<OfflinePolouse>();
            this.TeklifGenels = new List<TeklifGenel>();
            this.TeklifProvizyons = new List<TeklifProvizyon>();
            this.TVMKullaniciAtamas = new List<TVMKullaniciAtama>();
            this.TVMKullaniciDurumTarihcesis = new List<TVMKullaniciDurumTarihcesi>();
            this.TVMKullaniciNotlars = new List<TVMKullaniciNotlar>();
            this.TVMKullaniciSifremiUnuttums = new List<TVMKullaniciSifremiUnuttum>();
        }

        public int KullaniciKodu { get; set; }
        public int TVMKodu { get; set; }
        public byte Gorevi { get; set; }
        public int YetkiGrubu { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string TCKN { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string CepTelefon { get; set; }
        public Nullable<System.DateTime> SifreGondermeTarihi { get; set; }
        public Nullable<System.DateTime> KayitTarihi { get; set; }
        public string Sifre { get; set; }
        public Nullable<System.DateTime> SifreTarihi { get; set; }
        public byte SifreDurumKodu { get; set; }
        public Nullable<int> HataliSifreGirisSayisi { get; set; }
        public Nullable<System.DateTime> HataliSifreGirisTarihi { get; set; }
        public Nullable<int> DepartmanKodu { get; set; }
        public Nullable<int> YoneticiKodu { get; set; }
        public string MTKodu { get; set; }
        public byte TeklifPoliceUretimi { get; set; }
        public string TeknikPersonelKodu { get; set; }
        public byte Durum { get; set; }
        public string EmailOnayKodu { get; set; }
        public string FotografURL { get; set; }
        public Nullable<System.DateTime> SonGirisTarihi { get; set; }
        public string SkypeNumara { get; set; }
        public Nullable<bool> AYPmi { get; set; }
        public string MobilDogrulamaKodu { get; set; }
        public string MobilDogrulamaOnaylandiMi { get; set; }
        public virtual ICollection<DuyuruDokuman> DuyuruDokumen { get; set; }
        public virtual ICollection<Duyurular> Duyurulars { get; set; }
        public virtual ICollection<Duyurular> Duyurulars1 { get; set; }
        public virtual ICollection<DuyuruTVM> DuyuruTVMs { get; set; }
        public virtual ICollection<IsTakipKullaniciGrupKullanicilari> IsTakipKullaniciGrupKullanicilaris { get; set; }
        public virtual ICollection<OfflinePolouse> OfflinePolice { get; set; }
        public virtual ICollection<TeklifGenel> TeklifGenels { get; set; }
        public virtual ICollection<PoliceGenel> PoliceGenels { get; set; }
        public virtual ICollection<TeklifProvizyon> TeklifProvizyons { get; set; }
        public virtual ICollection<TVMKullaniciAtama> TVMKullaniciAtamas { get; set; }
        public virtual ICollection<TVMKullaniciDurumTarihcesi> TVMKullaniciDurumTarihcesis { get; set; }
        public virtual ICollection<TVMKullaniciNotlar> TVMKullaniciNotlars { get; set; }
        public virtual ICollection<TVMKullaniciSifremiUnuttum> TVMKullaniciSifremiUnuttums { get; set; }
    }
}
