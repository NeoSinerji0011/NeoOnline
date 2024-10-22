using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsDurumMap : EntityTypeConfiguration<IsDurum>
    {
        public IsDurumMap()
        {
            // Primary Key
            this.HasKey(t => t.IsId);

            // Properties
            this.Property(t => t.Guid)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("IsDurum");
            this.Property(t => t.IsId).HasColumnName("IsId");
            this.Property(t => t.Guid).HasColumnName("Guid");
            this.Property(t => t.IsTipi).HasColumnName("IsTipi");
            this.Property(t => t.ReferansId).HasColumnName("ReferansId");
            this.Property(t => t.IsSayi).HasColumnName("IsSayi");
            this.Property(t => t.Tamamlanan).HasColumnName("Tamamlanan");
            this.Property(t => t.Durumu).HasColumnName("Durumu");
            this.Property(t => t.Baslangic).HasColumnName("Baslangic");
            this.Property(t => t.Bitis).HasColumnName("Bitis");
        }
    }
}
