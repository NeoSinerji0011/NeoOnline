using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_Aegon_TeklifOdemePlaniMap : EntityTypeConfiguration<Z_Aegon_TeklifOdemePlani>
    {
        public Z_Aegon_TeklifOdemePlaniMap()
        {
            // Primary Key
            this.HasKey(t => t.OdemePlaniId);

            // Properties
            this.Property(t => t.OdemePlaniId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("Z_Aegon_TeklifOdemePlani");
            this.Property(t => t.OdemePlaniId).HasColumnName("OdemePlaniId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.TaksitNo).HasColumnName("TaksitNo");
            this.Property(t => t.VadeTarihi).HasColumnName("VadeTarihi");
            this.Property(t => t.TaksitTutari).HasColumnName("TaksitTutari");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");
        }
    }
}
