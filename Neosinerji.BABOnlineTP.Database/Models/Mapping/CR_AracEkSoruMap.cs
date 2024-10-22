using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_AracEkSoruMap : EntityTypeConfiguration<CR_AracEkSoru>
    {
        public CR_AracEkSoruMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.SoruTipi, t.SoruKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SoruTipi)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SoruKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SoruAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CR_AracEkSoru");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.SoruTipi).HasColumnName("SoruTipi");
            this.Property(t => t.SoruKodu).HasColumnName("SoruKodu");
            this.Property(t => t.SoruAdi).HasColumnName("SoruAdi");
        }
    }
}
