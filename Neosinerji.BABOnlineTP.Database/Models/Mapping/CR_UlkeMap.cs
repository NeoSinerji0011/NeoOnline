using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CR_UlkeMap : EntityTypeConfiguration<CR_Ulke>
    {
        public CR_UlkeMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.UlkeKodu });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UlkeKodu)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.UlkeAdi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CR_Ulke");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.UlkeAdi).HasColumnName("UlkeAdi");
        }
    }
}
