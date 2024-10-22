using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class GenelTanimlarMap : EntityTypeConfiguration<GenelTanimlar>
    {
        public GenelTanimlarMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TanimTipi, t.TanimId });

            // Properties
            this.Property(t => t.TanimTipi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TanimId)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Aciklama)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("GenelTanimlar");
            this.Property(t => t.TanimTipi).HasColumnName("TanimTipi");
            this.Property(t => t.TanimId).HasColumnName("TanimId");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
