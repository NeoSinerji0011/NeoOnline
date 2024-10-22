using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMKullanicilarMap : EntityTypeConfiguration<TVMKullanicilar>
    {
        public TVMKullanicilarMap()
        {
            // Primary Key
            this.HasKey(t => t.KullaniciKodu);

            // Properties
            this.Property(t => t.Adi)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Soyadi)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.TCKN)
                .HasMaxLength(11);

            this.Property(t => t.Telefon)
                .HasMaxLength(20);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.CepTelefon)
                .HasMaxLength(20);

            this.Property(t => t.Sifre)
                .HasMaxLength(30);

            this.Property(t => t.MTKodu)
                .HasMaxLength(15);

            this.Property(t => t.TeknikPersonelKodu)
                .HasMaxLength(30);

            this.Property(t => t.EmailOnayKodu)
                .HasMaxLength(150);

            this.Property(t => t.FotografURL)
                .HasMaxLength(150);

            this.Property(t => t.SkypeNumara)
                .HasMaxLength(20);

            this.Property(t => t.MobilDogrulamaKodu)
                .HasMaxLength(10);

            this.Property(t => t.MobilDogrulamaOnaylandiMi)
                .IsFixedLength()
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("TVMKullanicilar");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.Gorevi).HasColumnName("Gorevi");
            this.Property(t => t.YetkiGrubu).HasColumnName("YetkiGrubu");
            this.Property(t => t.Adi).HasColumnName("Adi");
            this.Property(t => t.Soyadi).HasColumnName("Soyadi");
            this.Property(t => t.TCKN).HasColumnName("TCKN");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.CepTelefon).HasColumnName("CepTelefon");
            this.Property(t => t.SifreGondermeTarihi).HasColumnName("SifreGondermeTarihi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.Sifre).HasColumnName("Sifre");
            this.Property(t => t.SifreTarihi).HasColumnName("SifreTarihi");
            this.Property(t => t.SifreDurumKodu).HasColumnName("SifreDurumKodu");
            this.Property(t => t.HataliSifreGirisSayisi).HasColumnName("HataliSifreGirisSayisi");
            this.Property(t => t.HataliSifreGirisTarihi).HasColumnName("HataliSifreGirisTarihi");
            this.Property(t => t.DepartmanKodu).HasColumnName("DepartmanKodu");
            this.Property(t => t.YoneticiKodu).HasColumnName("YoneticiKodu");
            this.Property(t => t.MTKodu).HasColumnName("MTKodu");
            this.Property(t => t.TeklifPoliceUretimi).HasColumnName("TeklifPoliceUretimi");
            this.Property(t => t.TeknikPersonelKodu).HasColumnName("TeknikPersonelKodu");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.EmailOnayKodu).HasColumnName("EmailOnayKodu");
            this.Property(t => t.FotografURL).HasColumnName("FotografURL");
            this.Property(t => t.SonGirisTarihi).HasColumnName("SonGirisTarihi");
            this.Property(t => t.SkypeNumara).HasColumnName("SkypeNumara");
            this.Property(t => t.AYPmi).HasColumnName("AYPmi");
            this.Property(t => t.MobilDogrulamaKodu).HasColumnName("MobilDogrulamaKodu");
            this.Property(t => t.MobilDogrulamaOnaylandiMi).HasColumnName("MobilDogrulamaOnaylandiMi");
        }
    }
}
