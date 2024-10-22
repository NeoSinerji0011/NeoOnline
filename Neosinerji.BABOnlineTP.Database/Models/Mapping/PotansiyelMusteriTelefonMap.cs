using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PotansiyelMusteriTelefonMap : EntityTypeConfiguration<PotansiyelMusteriTelefon>
    {
        public PotansiyelMusteriTelefonMap()
        {
            // Primary Key
            this.HasKey(t => new { t.PotansiyelMusteriKodu, t.SiraNo });

            // Properties
            this.Property(t => t.PotansiyelMusteriKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Numara)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.NumaraSahibi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PotansiyelMusteriTelefon");
            this.Property(t => t.PotansiyelMusteriKodu).HasColumnName("PotansiyelMusteriKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.IletisimNumaraTipi).HasColumnName("IletisimNumaraTipi");
            this.Property(t => t.Numara).HasColumnName("Numara");
            this.Property(t => t.NumaraSahibi).HasColumnName("NumaraSahibi");

            // Relationships
            this.HasRequired(t => t.PotansiyelMusteriGenelBilgiler)
                .WithMany(t => t.PotansiyelMusteriTelefons)
                .HasForeignKey(d => d.PotansiyelMusteriKodu);

        }
    }
}
