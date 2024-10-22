using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracKullanimTarziMap : EntityTypeConfiguration<AracKullanimTarzi>
    {
        public AracKullanimTarziMap()
        {
            // Primary Key
            this.HasKey(t => new { t.KullanimTarziKodu, t.Kod2 });

            // Properties
            this.Property(t => t.KullanimTarziKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.Kod2)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.KullanimTarzi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("AracKullanimTarzi");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.Kod2).HasColumnName("Kod2");
            this.Property(t => t.KullanimSekliKodu).HasColumnName("KullanimSekliKodu");
            this.Property(t => t.KullanimTarzi).HasColumnName("KullanimTarzi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
