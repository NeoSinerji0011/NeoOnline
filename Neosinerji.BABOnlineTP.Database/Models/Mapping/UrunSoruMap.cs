using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class UrunSoruMap : EntityTypeConfiguration<UrunSoru>
    {
        public UrunSoruMap()
        {
            // Primary Key
            this.HasKey(t => new { t.UrunKodu, t.SoruKodu });

            // Properties
            this.Property(t => t.UrunKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SoruKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("UrunSoru");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.SoruKodu).HasColumnName("SoruKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");

            // Relationships
            this.HasRequired(t => t.Soru)
                .WithMany(t => t.UrunSorus)
                .HasForeignKey(d => d.SoruKodu);
            this.HasRequired(t => t.Urun)
                .WithMany(t => t.UrunSorus)
                .HasForeignKey(d => d.UrunKodu);

        }
    }
}
