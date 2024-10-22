using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class SaglikTeminatTipleriMap : EntityTypeConfiguration<SaglikTeminatTipleri>
    {
        public SaglikTeminatTipleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Kodu);

            // Properties
            this.Property(t => t.Kodu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TeminatTipi)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("SaglikTeminatTipleri");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.TeminatTipi).HasColumnName("TeminatTipi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
