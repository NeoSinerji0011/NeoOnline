using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class BransUrunMap : EntityTypeConfiguration<BransUrun>
    {
        public BransUrunMap()
        {
            // Primary Key
            this.HasKey(t => t.BransUrunId);

            // Properties
            this.Property(t => t.SigortaSirketBirlikKodu)
                .HasMaxLength(5);

            this.Property(t => t.SigortaSirketUrunKodu)
                .HasMaxLength(20);

            this.Property(t => t.SigortaSirketUrunAdi)
                .HasMaxLength(150);

            this.Property(t => t.SigortaSirketBransKodu)
                .HasMaxLength(20);

            this.Property(t => t.SigortaSirketBransAdi)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("BransUrun");
            this.Property(t => t.BransUrunId).HasColumnName("BransUrunId");
            this.Property(t => t.SigortaSirketBirlikKodu).HasColumnName("SigortaSirketBirlikKodu");
            this.Property(t => t.SigortaSirketUrunKodu).HasColumnName("SigortaSirketUrunKodu");
            this.Property(t => t.SigortaSirketUrunAdi).HasColumnName("SigortaSirketUrunAdi");
            this.Property(t => t.SigortaSirketBransKodu).HasColumnName("SigortaSirketBransKodu");
            this.Property(t => t.SigortaSirketBransAdi).HasColumnName("SigortaSirketBransAdi");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
        }
    }
}
