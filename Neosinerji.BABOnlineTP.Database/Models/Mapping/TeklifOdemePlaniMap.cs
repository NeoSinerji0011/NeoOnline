using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifOdemePlaniMap : EntityTypeConfiguration<TeklifOdemePlani>
    {
        public TeklifOdemePlaniMap()
        {
            // Primary Key
            this.HasKey(t => t.OdemePlaniId);

            // Properties
            // Table & Column Mappings
            this.ToTable("TeklifOdemePlani");
            this.Property(t => t.OdemePlaniId).HasColumnName("OdemePlaniId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.TaksitNo).HasColumnName("TaksitNo");
            this.Property(t => t.VadeTarihi).HasColumnName("VadeTarihi");
            this.Property(t => t.TaksitTutari).HasColumnName("TaksitTutari");
            this.Property(t => t.DovizliTaksitTutari).HasColumnName("DovizliTaksitTutari");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifOdemePlanis)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}
