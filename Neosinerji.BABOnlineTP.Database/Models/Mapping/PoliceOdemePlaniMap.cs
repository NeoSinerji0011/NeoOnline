using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceOdemePlaniMap : EntityTypeConfiguration<PoliceOdemePlani>
    {
        public PoliceOdemePlaniMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PoliceId, t.TaksitNo });

            // Properties
            this.Property(t => t.PoliceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TaksitNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("PoliceOdemePlani");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.TaksitNo).HasColumnName("TaksitNo");
            this.Property(t => t.VadeTarihi).HasColumnName("VadeTarihi");
            this.Property(t => t.TaksitTutari).HasColumnName("TaksitTutari");
            this.Property(t => t.DovizliTaksitTutari).HasColumnName("DovizliTaksitTutari");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");

            // Relationships
            this.HasRequired(t => t.PoliceGenel)
                .WithMany(t => t.PoliceOdemePlanis)
                .HasForeignKey(d => d.PoliceId);

        }
    }
}
