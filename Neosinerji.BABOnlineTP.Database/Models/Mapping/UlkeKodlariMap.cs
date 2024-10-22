using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class UlkeKodlariMap : EntityTypeConfiguration<UlkeKodlari>
    {
        public UlkeKodlariMap()
        {
            // Primary Key
            this.HasKey(t => t.UlkeKodu);

            // Properties
            this.Property(t => t.UlkeKodu)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.UlkeAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("UlkeKodlari");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.UlkeAdi).HasColumnName("UlkeAdi");
            this.Property(t => t.Garanti).HasColumnName("Garanti");
            this.Property(t => t.SuprimOrani).HasColumnName("SuprimOrani");
            this.Property(t => t.SchengenMi).HasColumnName("SchengenMi");
        }
    }
}
