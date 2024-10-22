using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DaskSubelerMap : EntityTypeConfiguration<DaskSubeler>
    {
        public DaskSubelerMap()
        {
            // Primary Key
            this.HasKey(t => new { t.KurumKodu, t.SubeKodu });

            // Properties
            this.Property(t => t.KurumKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SubeKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SubeAdi)
                .IsRequired()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("DaskSubeler");
            this.Property(t => t.KurumKodu).HasColumnName("KurumKodu");
            this.Property(t => t.SubeKodu).HasColumnName("SubeKodu");
            this.Property(t => t.SubeAdi).HasColumnName("SubeAdi");

            // Relationships
            this.HasRequired(t => t.DaskKurumlar)
                .WithMany(t => t.DaskSubelers)
                .HasForeignKey(d => d.KurumKodu);

        }
    }
}
