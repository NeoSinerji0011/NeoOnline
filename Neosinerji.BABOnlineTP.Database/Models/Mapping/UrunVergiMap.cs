using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class UrunVergiMap : EntityTypeConfiguration<UrunVergi>
    {
        public UrunVergiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.UrunKodu, t.VergiKodu });

            // Properties
            this.Property(t => t.UrunKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.VergiKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("UrunVergi");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.VergiKodu).HasColumnName("VergiKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");

            // Relationships
            this.HasRequired(t => t.Urun)
                .WithMany(t => t.UrunVergis)
                .HasForeignKey(d => d.UrunKodu);
            this.HasRequired(t => t.Vergi)
                .WithMany(t => t.UrunVergis)
                .HasForeignKey(d => d.VergiKodu);

        }
    }
}
