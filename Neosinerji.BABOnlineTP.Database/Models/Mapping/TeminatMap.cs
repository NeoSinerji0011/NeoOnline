using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeminatMap : EntityTypeConfiguration<Teminat>
    {
        public TeminatMap()
        {
            // Primary Key
            this.HasKey(t => t.TeminatKodu);

            // Properties
            this.Property(t => t.TeminatKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TeminatAdi)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("Teminat");
            this.Property(t => t.TeminatKodu).HasColumnName("TeminatKodu");
            this.Property(t => t.TeminatAdi).HasColumnName("TeminatAdi");
        }
    }
}
