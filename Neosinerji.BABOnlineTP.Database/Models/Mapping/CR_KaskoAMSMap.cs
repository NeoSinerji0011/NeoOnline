using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_KaskoAMSMap : EntityTypeConfiguration<CR_KaskoAMS>
    {
        public CR_KaskoAMSMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.TVMKodu, t.AMSKodu, t.KullanimTarziKodu, t.GecerliOlduguTarih });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AMSKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.KullanimTarziKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Aciklama)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CR_KaskoAMS");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.AMSKodu).HasColumnName("AMSKodu");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.GecerliOlduguTarih).HasColumnName("GecerliOlduguTarih");
            this.Property(t => t.BedeniSahis).HasColumnName("BedeniSahis");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
