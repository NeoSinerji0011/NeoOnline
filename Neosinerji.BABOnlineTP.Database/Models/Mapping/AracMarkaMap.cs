using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracMarkaMap : EntityTypeConfiguration<AracMarka>
    {
        public AracMarkaMap()
        {
            // Primary Key
            this.HasKey(t => t.MarkaKodu);

            // Properties
            this.Property(t => t.MarkaKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.MarkaAdi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AracMarka");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.MarkaAdi).HasColumnName("MarkaAdi");
        }
    }
}
