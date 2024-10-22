using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMBankaHesaplariMap : EntityTypeConfiguration<TVMBankaHesaplari>
    {
        public TVMBankaHesaplariMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BankaKodu)
                .HasMaxLength(30);

            this.Property(t => t.BankaAdi)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.SubeKodu)
                .HasMaxLength(30);

            this.Property(t => t.SubeAdi)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.HesapNo)
                .HasMaxLength(50);

            this.Property(t => t.AcenteKrediKartiNo)
               .HasMaxLength(20);

            this.Property(t => t.IBAN)
                .HasMaxLength(50);

            this.Property(t => t.HesapAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CariHesapNo)
               .HasMaxLength(20);

            this.Property(t => t.HesapTipi);

            // Table & Column Mappings
            this.ToTable("TVMBankaHesaplari");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.BankaKodu).HasColumnName("BankaKodu");
            this.Property(t => t.BankaAdi).HasColumnName("BankaAdi");
            this.Property(t => t.SubeKodu).HasColumnName("SubeKodu");
            this.Property(t => t.SubeAdi).HasColumnName("SubeAdi");
            this.Property(t => t.HesapNo).HasColumnName("HesapNo");
            this.Property(t => t.AcenteKrediKartiNo).HasColumnName("AcenteKrediKartiNo");
            this.Property(t => t.IBAN).HasColumnName("IBAN");
            this.Property(t => t.HesapAdi).HasColumnName("HesapAdi");
            this.Property(t => t.CariHesapNo).HasColumnName("CariHesapNo");
            this.Property(t => t.HesapTipi).HasColumnName("HesapTipi");
        }
    }
}
