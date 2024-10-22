using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_2014TeklifNotMap : EntityTypeConfiguration<Z_2014TeklifNot>
    {
        public Z_2014TeklifNotMap()
        {
            // Primary Key
            this.HasKey(t => t.TableId);

            // Properties
            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("Z_2014TeklifNot");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
