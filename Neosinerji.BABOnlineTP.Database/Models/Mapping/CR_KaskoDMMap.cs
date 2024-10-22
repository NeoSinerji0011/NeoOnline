using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_KaskoDMMap : EntityTypeConfiguration<CR_KaskoDM>
    {
        public CR_KaskoDMMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.KurumTipi, t.KurumKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KurumTipi)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KurumKodu)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.KurumAdi)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("CR_KaskoDM");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.KurumTipi).HasColumnName("KurumTipi");
            this.Property(t => t.KurumKodu).HasColumnName("KurumKodu");
            this.Property(t => t.KurumAdi).HasColumnName("KurumAdi");
        }
    }
}
