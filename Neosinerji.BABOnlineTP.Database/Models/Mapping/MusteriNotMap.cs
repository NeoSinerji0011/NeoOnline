using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MusteriNotMap : EntityTypeConfiguration<MusteriNot>
    {
        public MusteriNotMap()
        {
            // Primary Key
            this.HasKey(t => new { t.MusteriKodu, t.SiraNo });

            // Properties
            this.Property(t => t.MusteriKodu)
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
            this.ToTable("MusteriNot");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.NotAciklamasi).HasColumnName("NotAciklamasi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMPersonelKodu).HasColumnName("TVMPersonelKodu");

            // Relationships
            this.HasRequired(t => t.MusteriGenelBilgiler)
                .WithMany(t => t.MusteriNots)
                .HasForeignKey(d => d.MusteriKodu);

        }
    }
}
