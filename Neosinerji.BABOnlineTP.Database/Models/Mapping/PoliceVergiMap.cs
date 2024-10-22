using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceVergiMap : EntityTypeConfiguration<PoliceVergi>
    {
        public PoliceVergiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PoliceId, t.VergiKodu });

            // Properties
            this.Property(t => t.PoliceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VergiKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("PoliceVergi");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.VergiKodu).HasColumnName("VergiKodu");
            this.Property(t => t.VergiTutari).HasColumnName("VergiTutari");

            // Relationships
            this.HasRequired(t => t.PoliceGenel)
                .WithMany(t => t.PoliceVergis)
                .HasForeignKey(d => d.PoliceId);

        }
    }
}
