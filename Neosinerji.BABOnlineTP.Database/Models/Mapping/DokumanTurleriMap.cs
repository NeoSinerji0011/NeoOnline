using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DokumanTurleriMap : EntityTypeConfiguration<DokumanTurleri>
    {
        public DokumanTurleriMap()
        {
            // Primary Key
            this.HasKey(t => t.DokumanTurKodu);

            // Properties
            this.Property(t => t.DokumanTurAciklama)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DokumanTurleri");
            this.Property(t => t.DokumanTurKodu).HasColumnName("DokumanTurKodu");
            this.Property(t => t.DokumanTurAciklama).HasColumnName("DokumanTurAciklama");
            this.Property(t => t.ZorunlulukTipi).HasColumnName("ZorunlulukTipi");
        }
    }
}
