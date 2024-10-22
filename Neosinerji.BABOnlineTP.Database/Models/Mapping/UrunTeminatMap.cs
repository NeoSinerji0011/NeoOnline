using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class UrunTeminatMap : EntityTypeConfiguration<UrunTeminat>
    {
        public UrunTeminatMap()
        {
            // Primary Key
            this.HasKey(t => new { t.UrunKodu, t.TeminatKodu });

            // Properties
            this.Property(t => t.UrunKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TeminatKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("UrunTeminat");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.TeminatKodu).HasColumnName("TeminatKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");

            // Relationships
            this.HasRequired(t => t.Teminat)
                .WithMany(t => t.UrunTeminats)
                .HasForeignKey(d => d.TeminatKodu);
            this.HasRequired(t => t.Urun)
                .WithMany(t => t.UrunTeminats)
                .HasForeignKey(d => d.UrunKodu);

        }
    }
}
