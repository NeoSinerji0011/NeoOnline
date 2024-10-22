using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipSoruMap : EntityTypeConfiguration<IsTakipSoru>
    {
        public IsTakipSoruMap()
        {
            // Primary Key
            this.HasKey(t => t.IsTakipSoruId);

            // Properties
            this.Property(t => t.Cevap)
                .IsRequired()
                .HasMaxLength(300);

            // Table & Column Mappings
            this.ToTable("IsTakipSoru");
            this.Property(t => t.IsTakipSoruId).HasColumnName("IsTakipSoruId");
            this.Property(t => t.IsTakipDetayId).HasColumnName("IsTakipDetayId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.SoruKodu).HasColumnName("SoruKodu");
            this.Property(t => t.CevapTipi).HasColumnName("CevapTipi");
            this.Property(t => t.Cevap).HasColumnName("Cevap");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

            // Relationships
            this.HasRequired(t => t.IsTakipDetay)
                .WithMany(t => t.IsTakipSorus)
                .HasForeignKey(d => d.IsTakipDetayId);

        }
    }
}
