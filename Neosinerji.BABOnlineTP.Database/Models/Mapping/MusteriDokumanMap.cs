using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MusteriDokumanMap : EntityTypeConfiguration<MusteriDokuman>
    {
        public MusteriDokumanMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MusteriKodu, t.SiraNo });

            // Properties
            this.Property(t => t.MusteriKodu)
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
            this.ToTable("MusteriDokuman");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.DokumanTuru).HasColumnName("DokumanTuru");
            this.Property(t => t.DokumanURL).HasColumnName("DokumanURL");
            this.Property(t => t.DosyaAdi).HasColumnName("DosyaAdi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMPersonelKodu).HasColumnName("TVMPersonelKodu");

            // Relationships
            this.HasRequired(t => t.MusteriGenelBilgiler)
                .WithMany(t => t.MusteriDokumen)
                .HasForeignKey(d => d.MusteriKodu);

        }
    }
}
