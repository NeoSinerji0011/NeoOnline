using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class ZzzPolicelerMap : EntityTypeConfiguration<ZzzPoliceler>
    {
        public ZzzPolicelerMap()
        {
            // Primary Key
            this.HasKey(t => t.TableId);

            // Properties
            // Table & Column Mappings
            this.ToTable("ZzzPoliceler");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.TVM).HasColumnName("TVM");
            this.Property(t => t.TNo).HasColumnName("TNo");
        }
    }
}
