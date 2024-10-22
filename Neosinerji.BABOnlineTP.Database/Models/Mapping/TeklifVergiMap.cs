using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifVergiMap : EntityTypeConfiguration<TeklifVergi>
    {
        public TeklifVergiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TeklifId, t.VergiKodu });

            // Properties
            this.Property(t => t.TeklifId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VergiKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TeklifVergi");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.VergiKodu).HasColumnName("VergiKodu");
            this.Property(t => t.VergiTutari).HasColumnName("VergiTutari");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifVergis)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}
