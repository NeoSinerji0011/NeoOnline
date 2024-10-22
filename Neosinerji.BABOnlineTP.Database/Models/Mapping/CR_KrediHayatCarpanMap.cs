using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_KrediHayatCarpanMap : EntityTypeConfiguration<CR_KrediHayatCarpan>
    {
        public CR_KrediHayatCarpanMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.TipKodu, t.MusteriYas, t.KrediVade });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TipKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.MusteriYas)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KrediVade)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("CR_KrediHayatCarpan");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.MusteriYas).HasColumnName("MusteriYas");
            this.Property(t => t.KrediVade).HasColumnName("KrediVade");
            this.Property(t => t.Carpan).HasColumnName("Carpan");
        }
    }
}
