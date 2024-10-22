using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_2014TeklifVergiMap : EntityTypeConfiguration<Z_2014TeklifVergi>
    {
        public Z_2014TeklifVergiMap()
        {
            // Primary Key
            this.HasKey(t => t.TableId);

            // Properties
            // Table & Column Mappings
            this.ToTable("Z_2014TeklifVergi");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.VergiKodu).HasColumnName("VergiKodu");
            this.Property(t => t.VergiTutari).HasColumnName("VergiTutari");
        }
    }
}
