using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class ZMarkaTipMap : EntityTypeConfiguration<ZMarkaTip>
    {
        public ZMarkaTipMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MarkaKod, t.TipKod });

            // Properties
            this.Property(t => t.MarkaKod)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.TipKod)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.TipAd)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ZMarkaTip");
            this.Property(t => t.MarkaKod).HasColumnName("MarkaKod");
            this.Property(t => t.TipKod).HasColumnName("TipKod");
            this.Property(t => t.TipAd).HasColumnName("TipAd");
        }
    }
}
