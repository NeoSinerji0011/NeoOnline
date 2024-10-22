using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_2014TeklifOdemePlaniMap : EntityTypeConfiguration<Z_2014TeklifOdemePlani>
    {
        public Z_2014TeklifOdemePlaniMap()
        {
            // Primary Key
            this.HasKey(t => t.TableOdemePlaniId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Z_2014TeklifOdemePlani");
            this.Property(t => t.TableOdemePlaniId).HasColumnName("TableOdemePlaniId");
            this.Property(t => t.OdemePlaniId).HasColumnName("OdemePlaniId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.TaksitNo).HasColumnName("TaksitNo");
            this.Property(t => t.VadeTarihi).HasColumnName("VadeTarihi");
            this.Property(t => t.TaksitTutari).HasColumnName("TaksitTutari");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");
        }
    }
}
