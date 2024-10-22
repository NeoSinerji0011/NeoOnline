using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracModelYedekMap : EntityTypeConfiguration<AracModelYedek>
    {
        public AracModelYedekMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MarkaKodu, t.TipKodu, t.Model });

            // Properties
            this.Property(t => t.MarkaKodu)
                .IsRequired()
                .HasMaxLength(4);

            this.Property(t => t.TipKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Model)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("AracModelYedek");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.Fiyat).HasColumnName("Fiyat");

            // Relationships
            this.HasRequired(t => t.AracMarkaYedek)
                .WithMany(t => t.AracModelYedeks)
                .HasForeignKey(d => d.MarkaKodu);

        }
    }
}
