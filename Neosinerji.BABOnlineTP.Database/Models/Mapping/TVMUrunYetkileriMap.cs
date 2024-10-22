using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMUrunYetkileriMap : EntityTypeConfiguration<TVMUrunYetkileri>
    {
        public TVMUrunYetkileriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.BABOnlineUrunKodu, t.TUMKodu, t.TUMUrunKodu });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BABOnlineUrunKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TUMUrunKodu)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("TVMUrunYetkileri");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.BABOnlineUrunKodu).HasColumnName("BABOnlineUrunKodu");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.TUMUrunKodu).HasColumnName("TUMUrunKodu");
            this.Property(t => t.Teklif).HasColumnName("Teklif");
            this.Property(t => t.Police).HasColumnName("Police");
            this.Property(t => t.Rapor).HasColumnName("Rapor");
            this.Property(t => t.ManuelHavale).HasColumnName("ManuelHavale");
            this.Property(t => t.HavaleEntegrasyon).HasColumnName("HavaleEntegrasyon");
            this.Property(t => t.KrediKartiTahsilat).HasColumnName("KrediKartiTahsilat");
            this.Property(t => t.AcikHesapTahsilatGercek).HasColumnName("AcikHesapTahsilatGercek");
            this.Property(t => t.AcikHesapTahsilatTuzel).HasColumnName("AcikHesapTahsilatTuzel");

            // Relationships
            this.HasRequired(t => t.TUMDetay)
                .WithMany(t => t.TVMUrunYetkileris)
                .HasForeignKey(d => d.TUMKodu);
            this.HasRequired(t => t.TVMDetay)
                .WithMany(t => t.TVMUrunYetkileris)
                .HasForeignKey(d => d.TVMKodu);
            this.HasRequired(t => t.Urun)
                .WithMany(t => t.TVMUrunYetkileris)
                .HasForeignKey(d => d.BABOnlineUrunKodu);

        }
    }
}
