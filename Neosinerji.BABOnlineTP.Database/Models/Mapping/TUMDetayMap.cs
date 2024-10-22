using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TUMDetayMap : EntityTypeConfiguration<TUMDetay>
    {
        public TUMDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.Kodu);

            // Properties
            this.Property(t => t.Kodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Unvani)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.BirlikKodu)
                .HasMaxLength(50);

            this.Property(t => t.VergiDairesi)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.VergiNumarasi)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Telefon)
                .HasMaxLength(20);

            this.Property(t => t.Fax)
                .HasMaxLength(20);

            this.Property(t => t.Email)
                .HasMaxLength(50);

            this.Property(t => t.WebAdresi)
                .HasMaxLength(50);

            this.Property(t => t.UlkeKodu)
                .HasMaxLength(3);

            this.Property(t => t.IlKodu)
                .HasMaxLength(5);

            this.Property(t => t.Semt)
                .HasMaxLength(50);

            this.Property(t => t.Adres)
                .HasMaxLength(200);

            this.Property(t => t.Banner)
                .HasMaxLength(300);

            this.Property(t => t.Logo)
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("TUMDetay");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.Unvani).HasColumnName("Unvani");
            this.Property(t => t.BirlikKodu).HasColumnName("BirlikKodu");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.VergiNumarasi).HasColumnName("VergiNumarasi");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.DurumGuncellemeTarihi).HasColumnName("DurumGuncellemeTarihi");
            this.Property(t => t.TUMBaslangicTarihi).HasColumnName("TUMBaslangicTarihi");
            this.Property(t => t.TUMBitisTarihi).HasColumnName("TUMBitisTarihi");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.WebAdresi).HasColumnName("WebAdresi");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.Semt).HasColumnName("Semt");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.BaglantiSiniri).HasColumnName("BaglantiSiniri");
            this.Property(t => t.UcretlendirmeKodu).HasColumnName("UcretlendirmeKodu");
            this.Property(t => t.Banner).HasColumnName("Banner");
            this.Property(t => t.Logo).HasColumnName("Logo");
            this.Property(t => t.UygulamaKodu).HasColumnName("UygulamaKodu");
        }
    }
}
