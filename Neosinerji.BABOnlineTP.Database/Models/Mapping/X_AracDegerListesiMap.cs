using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class X_AracDegerListesiMap : EntityTypeConfiguration<X_AracDegerListesi>
    {
        public X_AracDegerListesiMap()
        {
            // Primary Key
            this.HasKey(t => t.TaskId);

            // Properties
            this.Property(t => t.MarkaKodu)
                .HasMaxLength(50);

            this.Property(t => t.TipKodu)
                .HasMaxLength(50);

            this.Property(t => t.Model)
                .HasMaxLength(50);

            this.Property(t => t.MarkaAdi)
                .HasMaxLength(50);

            this.Property(t => t.TipAdi)
                .IsFixedLength()
                .HasMaxLength(100);

            this.Property(t => t.Fiyat)
                .IsFixedLength()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("X_AracDegerListesi");
            this.Property(t => t.TaskId).HasColumnName("TaskId");
            this.Property(t => t.MarkaKodu).HasColumnName("MarkaKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.MarkaAdi).HasColumnName("MarkaAdi");
            this.Property(t => t.TipAdi).HasColumnName("TipAdi");
            this.Property(t => t.Fiyat).HasColumnName("Fiyat");
        }
    }
}
