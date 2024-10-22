using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMBolgeleriMap : EntityTypeConfiguration<TVMBolgeleri>
    {
        public TVMBolgeleriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.TVMBolgeKodu });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TVMBolgeKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BolgeAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Aciklama)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("TVMBolgeleri");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMBolgeKodu).HasColumnName("TVMBolgeKodu");
            this.Property(t => t.BolgeAdi).HasColumnName("BolgeAdi");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
