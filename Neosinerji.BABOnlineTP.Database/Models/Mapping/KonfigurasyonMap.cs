using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class KonfigurasyonMap : EntityTypeConfiguration<Konfigurasyon>
    {
        public KonfigurasyonMap()
        {
            // Primary Key
            this.HasKey(t => t.Kod);

            // Properties
            this.Property(t => t.Kod)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Deger)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Konfigurasyon");
            this.Property(t => t.Kod).HasColumnName("Kod");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Deger).HasColumnName("Deger");
        }
    }
}
