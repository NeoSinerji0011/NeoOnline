using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracMarkaTestMap : EntityTypeConfiguration<AracMarkaTest>
    {
        public AracMarkaTestMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.MarkaKodu)
                .HasMaxLength(10);

            this.Property(t => t.MarkaAdi)
                .HasMaxLength(70);

            // Table & Column Mappings
            this.ToTable("AracMarkaTest");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.MarkaAdi).HasColumnName("MarkaAdi");
        }
    }
}
