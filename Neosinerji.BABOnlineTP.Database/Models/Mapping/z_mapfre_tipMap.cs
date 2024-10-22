using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class z_mapfre_tipMap : EntityTypeConfiguration<z_mapfre_tip>
    {
        public z_mapfre_tipMap()
        {
            // Primary Key
            this.HasKey(t => t.MarkaKodu);

            // Properties
            this.Property(t => t.MarkaKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.TipKodu)
                .HasMaxLength(5);

            this.Property(t => t.TipAdi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("z_mapfre_tip");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.TipAdi).HasColumnName("TipAdi");
        }
    }
}
