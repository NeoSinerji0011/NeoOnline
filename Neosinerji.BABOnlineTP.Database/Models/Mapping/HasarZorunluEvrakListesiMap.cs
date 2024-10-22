using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarZorunluEvrakListesiMap : EntityTypeConfiguration<HasarZorunluEvrakListesi>
    {
        public HasarZorunluEvrakListesiMap()
        {
            // Primary Key
            this.HasKey(t => t.EvrakKodu);

            // Properties
            this.Property(t => t.EvrakAdi)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("HasarZorunluEvrakListesi");
            this.Property(t => t.EvrakKodu).HasColumnName("EvrakKodu");
            this.Property(t => t.EvrakAdi).HasColumnName("EvrakAdi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
