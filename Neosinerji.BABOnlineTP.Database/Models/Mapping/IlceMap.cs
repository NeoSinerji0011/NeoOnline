using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IlceMap : EntityTypeConfiguration<Ilce>
    {
        public IlceMap()
        {
            // Primary Key
            this.HasKey(t => t.IlceKodu);

            // Properties
            this.Property(t => t.UlkeKodu)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.IlKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.IlceAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Ilce");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");

            // Relationships
            this.HasRequired(t => t.Il)
                .WithMany(t => t.Ilces)
                .HasForeignKey(d => new { d.IlKodu, d.UlkeKodu });
            this.HasRequired(t => t.Ulke)
                .WithMany(t => t.Ilces)
                .HasForeignKey(d => d.UlkeKodu);

        }
    }
}
