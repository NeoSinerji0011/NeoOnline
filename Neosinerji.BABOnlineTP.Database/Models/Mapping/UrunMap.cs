using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class UrunMap : EntityTypeConfiguration<Urun>
    {
        public UrunMap()
        {
            // Primary Key
            this.HasKey(t => t.UrunKodu);

            // Properties
            this.Property(t => t.UrunKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UrunAdi)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("Urun");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.UrunAdi).HasColumnName("UrunAdi");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.Durum).HasColumnName("Durum");

            // Relationships
            this.HasRequired(t => t.Bran)
                .WithMany(t => t.Uruns)
                .HasForeignKey(d => d.BransKodu);

        }
    }
}
