using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class UrunParametreleriMap : EntityTypeConfiguration<UrunParametreleri>
    {
        public UrunParametreleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Kod);

            // Properties
            this.Property(t => t.Kod)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Aciklama)
                .HasMaxLength(250);

            this.Property(t => t.Deger)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("UrunParametreleri");
            this.Property(t => t.Kod).HasColumnName("Kod");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Deger).HasColumnName("Deger");
        }
    }
}
