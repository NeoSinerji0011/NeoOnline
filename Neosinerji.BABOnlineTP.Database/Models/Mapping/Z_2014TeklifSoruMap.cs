using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_2014TeklifSoruMap : EntityTypeConfiguration<Z_2014TeklifSoru>
    {
        public Z_2014TeklifSoruMap()
        {
            // Primary Key
            this.HasKey(t => t.TableId);

            // Properties
            this.Property(t => t.Cevap)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Z_2014TeklifSoru");
            this.Property(t => t.TableId).HasColumnName("TableId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.SoruKodu).HasColumnName("SoruKodu");
            this.Property(t => t.CevapTipi).HasColumnName("CevapTipi");
            this.Property(t => t.Cevap).HasColumnName("Cevap");
        }
    }
}
