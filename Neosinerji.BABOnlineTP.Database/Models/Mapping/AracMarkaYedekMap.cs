using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracMarkaYedekMap : EntityTypeConfiguration<AracMarkaYedek>
    {
        public AracMarkaYedekMap()
        {
            // Primary Key
            this.HasKey(t => t.MarkaKodu);

            // Properties
            this.Property(t => t.MarkaKodu)
                .IsRequired()
                .HasMaxLength(4);

            this.Property(t => t.MarkaAdi)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("AracMarkaYedek");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.MarkaAdi).HasColumnName("MarkaAdi");
        }
    }
}
