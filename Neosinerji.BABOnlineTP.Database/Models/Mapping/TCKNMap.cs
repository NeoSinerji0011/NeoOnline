using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TCKNMap : EntityTypeConfiguration<TCKN>
    {
        public TCKNMap()
        {
            // Primary Key
            this.HasKey(t => t.KimlikNo);

            // Properties
            this.Property(t => t.BabaAdi)
                .HasMaxLength(50);

            this.Property(t => t.Ad)
                .HasMaxLength(50);

            this.Property(t => t.VerilisNedeni)
                .HasMaxLength(50);

            this.Property(t => t.IlceAd)
                .HasMaxLength(50);

            this.Property(t => t.Cinsiyet)
                .HasMaxLength(50);

            this.Property(t => t.AnaAdi)
                .HasMaxLength(50);

            this.Property(t => t.Durum)
                .HasMaxLength(50);

            this.Property(t => t.Seri)
                .HasMaxLength(50);

            this.Property(t => t.DogumYeri)
                .HasMaxLength(50);

            this.Property(t => t.KimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.VerildigiIlceAdi)
                .HasMaxLength(50);

            this.Property(t => t.CiltAd)
                .HasMaxLength(50);

            this.Property(t => t.Soyad)
                .HasMaxLength(50);

            this.Property(t => t.SorgulamaYeri)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TCKN");
            this.Property(t => t.KayitNo).HasColumnName("KayitNo");
            this.Property(t => t.BabaAdi).HasColumnName("BabaAdi");
            this.Property(t => t.Ad).HasColumnName("Ad");
            this.Property(t => t.VerilisNedeni).HasColumnName("VerilisNedeni");
            this.Property(t => t.AileSiraNo).HasColumnName("AileSiraNo");
            this.Property(t => t.CiltKod).HasColumnName("CiltKod");
            this.Property(t => t.VerildigiIlce).HasColumnName("VerildigiIlce");
            this.Property(t => t.IlceAd).HasColumnName("IlceAd");
            this.Property(t => t.IlceKod).HasColumnName("IlceKod");
            this.Property(t => t.VerilisTarihi).HasColumnName("VerilisTarihi");
            this.Property(t => t.Cinsiyet).HasColumnName("Cinsiyet");
            this.Property(t => t.BireySiraNo).HasColumnName("BireySiraNo");
            this.Property(t => t.Num).HasColumnName("Num");
            this.Property(t => t.AnaAdi).HasColumnName("AnaAdi");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.DogumTarihi).HasColumnName("DogumTarihi");
            this.Property(t => t.OlumTarihi).HasColumnName("OlumTarihi");
            this.Property(t => t.Seri).HasColumnName("Seri");
            this.Property(t => t.DogumYeri).HasColumnName("DogumYeri");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.KimlikNo).HasColumnName("KimlikNo");
            this.Property(t => t.VerildigiIlceAdi).HasColumnName("VerildigiIlceAdi");
            this.Property(t => t.CiltAd).HasColumnName("CiltAd");
            this.Property(t => t.Soyad).HasColumnName("Soyad");
            this.Property(t => t.SorgulamaYeri).HasColumnName("SorgulamaYeri");
            this.Property(t => t.ExpireDate).HasColumnName("ExpireDate");
        }
    }
}
