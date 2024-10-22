using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TUMUrunleriMap : EntityTypeConfiguration<TUMUrunleri>
    {
        public TUMUrunleriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.TUMUrunKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TUMUrunKodu)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.TUMUrunAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TUMBransKodu)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.TUMBransAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("TUMUrunleri");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.TUMUrunKodu).HasColumnName("TUMUrunKodu");
            this.Property(t => t.TUMUrunAdi).HasColumnName("TUMUrunAdi");
            this.Property(t => t.TUMBransKodu).HasColumnName("TUMBransKodu");
            this.Property(t => t.TUMBransAdi).HasColumnName("TUMBransAdi");
            this.Property(t => t.BABOnlineUrunKodu).HasColumnName("BABOnlineUrunKodu");

            // Relationships
            this.HasRequired(t => t.TUMDetay)
                .WithMany(t => t.TUMUrunleris)
                .HasForeignKey(d => d.TUMKodu);
            this.HasRequired(t => t.Urun)
                .WithMany(t => t.TUMUrunleris)
                .HasForeignKey(d => d.BABOnlineUrunKodu);

        }
    }
}
