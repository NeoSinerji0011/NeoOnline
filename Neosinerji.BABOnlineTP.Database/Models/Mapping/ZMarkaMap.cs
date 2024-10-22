using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class ZMarkaMap : EntityTypeConfiguration<ZMarka>
    {
        public ZMarkaMap()
        {
            // Primary Key
            this.HasKey(t => t.MarkaKod);

            // Properties
            this.Property(t => t.MarkaKod)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.MarkaAd)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("ZMarka");
            this.Property(t => t.MarkaKod).HasColumnName("MarkaKod");
            this.Property(t => t.MarkaAd).HasColumnName("MarkaAd");
        }
    }
}
