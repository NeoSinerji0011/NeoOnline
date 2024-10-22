using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarBankaHesaplariMap : EntityTypeConfiguration<HasarBankaHesaplari>
    {
        public HasarBankaHesaplariMap()
        {
            // Primary Key
            this.HasKey(t => t.BankaHesapId);

            // Properties
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
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.IBAN)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.HesapAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("HasarBankaHesaplari");
            this.Property(t => t.BankaHesapId).HasColumnName("BankaHesapId");
            this.Property(t => t.HasarId).HasColumnName("HasarId");
            this.Property(t => t.BankaKodu).HasColumnName("BankaKodu");
            this.Property(t => t.BankaAdi).HasColumnName("BankaAdi");
            this.Property(t => t.SubeKodu).HasColumnName("SubeKodu");
            this.Property(t => t.SubeAdi).HasColumnName("SubeAdi");
            this.Property(t => t.HesapNo).HasColumnName("HesapNo");
            this.Property(t => t.IBAN).HasColumnName("IBAN");
            this.Property(t => t.HesapAdi).HasColumnName("HesapAdi");

            // Relationships
            this.HasRequired(t => t.HasarGenelBilgiler)
                .WithMany(t => t.HasarBankaHesaplaris)
                .HasForeignKey(d => d.HasarId);

        }
    }
}
