using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class WebServisCevapMap : EntityTypeConfiguration<WebServisCevap>
    {
        public WebServisCevapMap()
        {
            // Primary Key
            this.HasKey(t => t.CevapKodu);

            // Properties
            this.Property(t => t.CevapKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.CevapAdi)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("WebServisCevap");
            this.Property(t => t.CevapKodu).HasColumnName("CevapKodu");
            this.Property(t => t.CevapAdi).HasColumnName("CevapAdi");
        }
    }
}
