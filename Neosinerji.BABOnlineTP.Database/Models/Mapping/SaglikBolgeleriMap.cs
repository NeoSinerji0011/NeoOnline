using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class SaglikBolgeleriMap : EntityTypeConfiguration<SaglikBolgeleri>
    {
        public SaglikBolgeleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Kodu);

            // Properties
            this.Property(t => t.Kodu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.BolgeAciklamasi)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("SaglikBolgeleri");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.BolgeAciklamasi).HasColumnName("BolgeAciklamasi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
