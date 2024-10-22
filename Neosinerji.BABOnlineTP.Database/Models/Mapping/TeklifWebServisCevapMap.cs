using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifWebServisCevapMap : EntityTypeConfiguration<TeklifWebServisCevap>
    {
        public TeklifWebServisCevapMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TeklifId, t.CevapKodu });

            // Properties
            this.Property(t => t.TeklifId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CevapKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Cevap)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("TeklifWebServisCevap");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.CevapKodu).HasColumnName("CevapKodu");
            this.Property(t => t.CevapTipi).HasColumnName("CevapTipi");
            this.Property(t => t.Cevap).HasColumnName("Cevap");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifWebServisCevaps)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}
