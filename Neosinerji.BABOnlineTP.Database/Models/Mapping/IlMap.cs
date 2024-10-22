using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IlMap : EntityTypeConfiguration<Il>
    {
        public IlMap()
        {
            // Primary Key
            this.HasKey(t => new { t.IlKodu, t.UlkeKodu });

            // Properties
            this.Property(t => t.IlKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.UlkeKodu)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.IlAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Il");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");

            // Relationships
            this.HasRequired(t => t.Ulke)
                .WithMany(t => t.Ils)
                .HasForeignKey(d => d.UlkeKodu);

        }
    }
}
