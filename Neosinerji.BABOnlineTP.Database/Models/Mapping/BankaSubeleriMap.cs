using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class BankaSubeleriMap : EntityTypeConfiguration<BankaSubeleri>
    {
        public BankaSubeleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Banka)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Sube)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.Adres)
                .HasMaxLength(300);

            this.Property(t => t.Ilce)
                .HasMaxLength(100);

            this.Property(t => t.Sehir)
                .HasMaxLength(100);

            this.Property(t => t.Telefon)
                .HasMaxLength(20);

            this.Property(t => t.Fax)
                .HasMaxLength(20);

            this.Property(t => t.Acilis)
                .HasMaxLength(20);

            this.Property(t => t.Kapanis)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("BankaSubeleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Banka).HasColumnName("Banka");
            this.Property(t => t.Sube).HasColumnName("Sube");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Ilce).HasColumnName("Ilce");
            this.Property(t => t.Sehir).HasColumnName("Sehir");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.Acilis).HasColumnName("Acilis");
            this.Property(t => t.Kapanis).HasColumnName("Kapanis");
        }
    }
}
