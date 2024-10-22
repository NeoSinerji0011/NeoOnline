using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MapfreBolgeRaporuIcmalMap : EntityTypeConfiguration<MapfreBolgeRaporuIcmal>
    {
        public MapfreBolgeRaporuIcmalMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.BolgeAdi)
                .HasMaxLength(256);

            this.Property(t => t.TVMUnvani)
                .HasMaxLength(256);

            // Table & Column Mappings
            this.ToTable("MapfreBolgeRaporuIcmal");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.BolgeKodu).HasColumnName("BolgeKodu");
            this.Property(t => t.BolgeAdi).HasColumnName("BolgeAdi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMUnvani).HasColumnName("TVMUnvani");
            this.Property(t => t.Profili).HasColumnName("Profili");
            this.Property(t => t.KaskoTeklif).HasColumnName("KaskoTeklif");
            this.Property(t => t.TrafikTeklif).HasColumnName("TrafikTeklif");
            this.Property(t => t.KaskoPolice).HasColumnName("KaskoPolice");
            this.Property(t => t.TrafikPolice).HasColumnName("TrafikPolice");
            this.Property(t => t.KaskoBrutPrim).HasColumnName("KaskoBrutPrim");
            this.Property(t => t.KaskoKomisyon).HasColumnName("KaskoKomisyon");
            this.Property(t => t.TrafikBrutPrim).HasColumnName("TrafikBrutPrim");
            this.Property(t => t.TrafikKomisyon).HasColumnName("TrafikKomisyon");
            this.Property(t => t.Id).HasColumnName("Id");
        }
    }
}
