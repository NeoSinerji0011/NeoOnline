using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_Aegon_TeklifSoruMap : EntityTypeConfiguration<Z_Aegon_TeklifSoru>
    {
        public Z_Aegon_TeklifSoruMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TeklifId, t.SoruKodu });

            // Properties
            this.Property(t => t.TeklifId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SoruKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Cevap)
                .IsRequired()
                .HasMaxLength(250);

            // Table & Column Mappings
            this.ToTable("Z_Aegon_TeklifSoru");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.SoruKodu).HasColumnName("SoruKodu");
            this.Property(t => t.CevapTipi).HasColumnName("CevapTipi");
            this.Property(t => t.Cevap).HasColumnName("Cevap");
        }
    }
}
