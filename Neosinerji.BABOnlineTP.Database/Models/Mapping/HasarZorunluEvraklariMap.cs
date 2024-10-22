using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarZorunluEvraklariMap : EntityTypeConfiguration<HasarZorunluEvraklari>
    {
        public HasarZorunluEvraklariMap()
        {
            // Primary Key
            this.HasKey(t => t.EvrakId);

            // Properties
            this.Property(t => t.EvrakURL)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("HasarZorunluEvraklari");
            this.Property(t => t.EvrakId).HasColumnName("EvrakId");
            this.Property(t => t.HasarId).HasColumnName("HasarId");
            this.Property(t => t.EvrakKodu).HasColumnName("EvrakKodu");
            this.Property(t => t.EvrakKayitTarihi).HasColumnName("EvrakKayitTarihi");
            this.Property(t => t.EvrakURL).HasColumnName("EvrakURL");

            // Relationships
            this.HasRequired(t => t.HasarGenelBilgiler)
                .WithMany(t => t.HasarZorunluEvraklaris)
                .HasForeignKey(d => d.HasarId);

        }
    }
}
