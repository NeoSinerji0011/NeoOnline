using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracModelTestMap : EntityTypeConfiguration<AracModelTest>
    {
        public AracModelTestMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.markakodu)
                .HasMaxLength(50);

            this.Property(t => t.tipkodu)
                .HasMaxLength(50);

            this.Property(t => t.model)
                .HasMaxLength(50);

            this.Property(t => t.fiyat)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AracModelTest");
            this.Property(t => t.id).HasColumnName("id");
            this.Property(t => t.markakodu).HasColumnName("markakodu");
            this.Property(t => t.tipkodu).HasColumnName("tipkodu");
            this.Property(t => t.model).HasColumnName("model");
            this.Property(t => t.fiyat).HasColumnName("fiyat");
        }
    }
}
