using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracKullanimSekliMap : EntityTypeConfiguration<AracKullanimSekli>
    {
        public AracKullanimSekliMap()
        {
            // Primary Key
            this.HasKey(t => t.KullanimSekliKodu);

            // Properties
            this.Property(t => t.KullanimSekliKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KullanimSekli)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AracKullanimSekli");
            this.Property(t => t.KullanimSekliKodu).HasColumnName("KullanimSekliKodu");
            this.Property(t => t.KullanimSekli).HasColumnName("KullanimSekli");
        }
    }
}
