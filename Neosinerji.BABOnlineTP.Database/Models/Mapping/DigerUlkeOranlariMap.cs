using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DigerUlkeOranlariMap : EntityTypeConfiguration<DigerUlkeOranlari>
    {
        public DigerUlkeOranlariMap()
        {
            // Primary Key
            this.HasKey(t => new { t.Gun1, t.Gun2, t.KisiTipi, t.PlanTipi });

            // Properties
            this.Property(t => t.Gun1)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Gun2)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("DigerUlkeOranlari");
            this.Property(t => t.Gun1).HasColumnName("Gun1");
            this.Property(t => t.Gun2).HasColumnName("Gun2");
            this.Property(t => t.KisiTipi).HasColumnName("KisiTipi");
            this.Property(t => t.PlanTipi).HasColumnName("PlanTipi");
            this.Property(t => t.Extra).HasColumnName("Extra");
            this.Property(t => t.Yil_1).HasColumnName("Yil_1");
            this.Property(t => t.Oran).HasColumnName("Oran");
        }
    }
}
