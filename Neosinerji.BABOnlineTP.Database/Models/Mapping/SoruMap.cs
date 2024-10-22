using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class SoruMap : EntityTypeConfiguration<Soru>
    {
        public SoruMap()
        {
            // Primary Key
            this.HasKey(t => t.SoruKodu);

            // Properties
            this.Property(t => t.SoruKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SoruAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Soru");
            this.Property(t => t.SoruKodu).HasColumnName("SoruKodu");
            this.Property(t => t.SoruAdi).HasColumnName("SoruAdi");
            this.Property(t => t.SoruCevapTipi).HasColumnName("SoruCevapTipi");
            this.Property(t => t.SoruCevapUzunlugu).HasColumnName("SoruCevapUzunlugu");
        }
    }
}
