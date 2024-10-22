using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class X_AracDegerAraTableMap : EntityTypeConfiguration<X_AracDegerAraTable>
    {
        public X_AracDegerAraTableMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MarkaKodu, t.TipKodu });

            // Properties
            this.Property(t => t.MarkaKodu)
                .IsRequired()
                .HasMaxLength(4);

            this.Property(t => t.TipKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.MarkaAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TipAdi)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("X_AracDegerAraTable");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.MarkaAdi).HasColumnName("MarkaAdi");
            this.Property(t => t.TipAdi).HasColumnName("TipAdi");
            this.Property(t => t.Yil).HasColumnName("Yil");
            this.Property(t => t.Fiyat).HasColumnName("Fiyat");
        }
    }
}
