using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class RaporMapfreBolgeMap : EntityTypeConfiguration<RaporMapfreBolge>
    {
        public RaporMapfreBolgeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.Gun });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("RaporMapfreBolge");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.Gun).HasColumnName("Gun");
            this.Property(t => t.TrafikTeklif).HasColumnName("TrafikTeklif");
            this.Property(t => t.TrafikPolice).HasColumnName("TrafikPolice");
            this.Property(t => t.TrafikBrutPrim).HasColumnName("TrafikBrutPrim");
            this.Property(t => t.TrafikKomisyon).HasColumnName("TrafikKomisyon");
            this.Property(t => t.KaskoTeklif).HasColumnName("KaskoTeklif");
            this.Property(t => t.KaskoPolice).HasColumnName("KaskoPolice");
            this.Property(t => t.KaskoBrutPrim).HasColumnName("KaskoBrutPrim");
            this.Property(t => t.KaskoKomisyon).HasColumnName("KaskoKomisyon");
        }
    }
}
