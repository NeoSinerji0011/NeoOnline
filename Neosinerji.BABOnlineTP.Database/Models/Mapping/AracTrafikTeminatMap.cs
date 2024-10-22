using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AracTrafikTeminatMap : EntityTypeConfiguration<AracTrafikTeminat>
    {
        public AracTrafikTeminatMap()
        {
            // Primary Key
            this.HasKey(t => new { t.AracGrupKodu, t.GecerlilikBaslamaTarihi });

            // Properties
            this.Property(t => t.AracGrupKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.AracGrubu)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("AracTrafikTeminat");
            this.Property(t => t.AracGrupKodu).HasColumnName("AracGrupKodu");
            this.Property(t => t.GecerlilikBaslamaTarihi).HasColumnName("GecerlilikBaslamaTarihi");
            this.Property(t => t.AracGrubu).HasColumnName("AracGrubu");
            this.Property(t => t.MaddiAracBasina).HasColumnName("MaddiAracBasina");
            this.Property(t => t.MaddiKazaBasina).HasColumnName("MaddiKazaBasina");
            this.Property(t => t.TedaviKisiBasina).HasColumnName("TedaviKisiBasina");
            this.Property(t => t.TedaviKazaBasina).HasColumnName("TedaviKazaBasina");
            this.Property(t => t.SakatlikKisiBasina).HasColumnName("SakatlikKisiBasina");
            this.Property(t => t.SakatlikKazaBasina).HasColumnName("SakatlikKazaBasina");
        }
    }
}
