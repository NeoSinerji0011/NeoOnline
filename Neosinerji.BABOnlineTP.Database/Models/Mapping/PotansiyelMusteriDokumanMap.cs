using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PotansiyelMusteriDokumanMap : EntityTypeConfiguration<PotansiyelMusteriDokuman>
    {
        public PotansiyelMusteriDokumanMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PotansiyelMusteriKodu, t.SiraNo });

            // Properties
            this.Property(t => t.PotansiyelMusteriKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.DokumanTuru)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.DokumanURL)
                .IsRequired()
                .HasMaxLength(500);

            this.Property(t => t.DosyaAdi)
                .IsRequired()
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("PotansiyelMusteriDokuman");
            this.Property(t => t.PotansiyelMusteriKodu).HasColumnName("PotansiyelMusteriKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.DokumanTuru).HasColumnName("DokumanTuru");
            this.Property(t => t.DokumanURL).HasColumnName("DokumanURL");
            this.Property(t => t.DosyaAdi).HasColumnName("DosyaAdi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMPersonelKodu).HasColumnName("TVMPersonelKodu");

            // Relationships
            this.HasRequired(t => t.PotansiyelMusteriGenelBilgiler)
                .WithMany(t => t.PotansiyelMusteriDokumen)
                .HasForeignKey(d => d.PotansiyelMusteriKodu);

        }
    }
}
