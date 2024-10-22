using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifAracEkSoruMap : EntityTypeConfiguration<TeklifAracEkSoru>
    {
        public TeklifAracEkSoruMap()
        {
            // Primary Key
            this.HasKey(t => t.TeklifAracEkSoruId);

            // Properties
            this.Property(t => t.SoruTipi)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SoruKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TeklifAracEkSoru");
            this.Property(t => t.TeklifAracEkSoruId).HasColumnName("TeklifAracEkSoruId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.SoruTipi).HasColumnName("SoruTipi");
            this.Property(t => t.SoruKodu).HasColumnName("SoruKodu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Bedel).HasColumnName("Bedel");
            this.Property(t => t.Fiyat).HasColumnName("Fiyat");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifAracEkSorus)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}
