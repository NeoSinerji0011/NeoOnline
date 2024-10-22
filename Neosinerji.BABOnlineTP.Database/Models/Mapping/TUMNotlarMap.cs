using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TUMNotlarMap : EntityTypeConfiguration<TUMNotlar>
    {
        public TUMNotlarMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KonuAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.NotAciklamasi)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("TUMNotlar");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.KonuAdi).HasColumnName("KonuAdi");
            this.Property(t => t.EklemeTarihi).HasColumnName("EklemeTarihi");
            this.Property(t => t.EkleyenPersonelKodu).HasColumnName("EkleyenPersonelKodu");
            this.Property(t => t.NotAciklamasi).HasColumnName("NotAciklamasi");
        }
    }
}
