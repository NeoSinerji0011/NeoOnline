using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_TescilIlIlceMap : EntityTypeConfiguration<CR_TescilIlIlce>
    {
        public CR_TescilIlIlceMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.IlKodu, t.IlceKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.IlKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.IlceKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.TescilIlAdi)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TescilIlceAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CR_TescilIlIlce");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.TescilIlAdi).HasColumnName("TescilIlAdi");
            this.Property(t => t.TescilIlceAdi).HasColumnName("TescilIlceAdi");
        }
    }
}
