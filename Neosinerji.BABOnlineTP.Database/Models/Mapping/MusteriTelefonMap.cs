using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MusteriTelefonMap : EntityTypeConfiguration<MusteriTelefon>
    {
        public MusteriTelefonMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MusteriKodu, t.SiraNo });

            // Properties
            this.Property(t => t.MusteriKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Numara)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.NumaraSahibi)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("MusteriTelefon");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.IletisimNumaraTipi).HasColumnName("IletisimNumaraTipi");
            this.Property(t => t.Numara).HasColumnName("Numara");
            this.Property(t => t.NumaraSahibi).HasColumnName("NumaraSahibi");

            // Relationships
            this.HasRequired(t => t.MusteriGenelBilgiler)
                .WithMany(t => t.MusteriTelefons)
                .HasForeignKey(d => d.MusteriKodu);

        }
    }
}
