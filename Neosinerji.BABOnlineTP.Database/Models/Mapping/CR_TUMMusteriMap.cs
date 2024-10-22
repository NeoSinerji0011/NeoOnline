using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_TUMMusteriMap : EntityTypeConfiguration<CR_TUMMusteri>
    {
        public CR_TUMMusteriMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.MusteriKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.MusteriKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TUMMusteriKodu)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("CR_TUMMusteri");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
            this.Property(t => t.TUMMusteriKodu).HasColumnName("TUMMusteriKodu");
        }
    }
}
