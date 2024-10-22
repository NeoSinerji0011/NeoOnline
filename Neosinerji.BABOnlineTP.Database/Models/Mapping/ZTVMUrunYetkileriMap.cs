using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class zTVMUrunYetkileriMap : EntityTypeConfiguration<zTVMUrunYetkileri>
    {
        public zTVMUrunYetkileriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.BABOnlineUrunKodu, t.TUMKodu, t.TUMUrunKodu, t.Teklif, t.Police, t.Rapor, t.ManuelHavale, t.HavaleEntegrasyon, t.KrediKartiTahsilat, t.AcikHesapTahsilatGercek, t.AcikHesapTahsilatTuzel });

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
            this.ToTable("zTVMUrunYetkileri");
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
        }
    }
}
