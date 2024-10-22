using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PotansiyelMusteriNotMap : EntityTypeConfiguration<PotansiyelMusteriNot>
    {
        public PotansiyelMusteriNotMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PotansiyelMusteriKodu, t.SiraNo });

            // Properties
            this.Property(t => t.PotansiyelMusteriKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Konu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NotAciklamasi)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("PotansiyelMusteriNot");
            this.Property(t => t.PotansiyelMusteriKodu).HasColumnName("PotansiyelMusteriKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.NotAciklamasi).HasColumnName("NotAciklamasi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMPersonelKodu).HasColumnName("TVMPersonelKodu");

            // Relationships
            this.HasRequired(t => t.PotansiyelMusteriGenelBilgiler)
                .WithMany(t => t.PotansiyelMusteriNots)
                .HasForeignKey(d => d.PotansiyelMusteriKodu);

        }
    }
}
