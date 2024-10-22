using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class OfflinePoliceNumaraMap : EntityTypeConfiguration<OfflinePoliceNumara>
    {
        public OfflinePoliceNumaraMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.UrunKodu });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.UrunKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("OfflinePoliceNumara");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.Baslangic).HasColumnName("Baslangic");
            this.Property(t => t.Bitis).HasColumnName("Bitis");
            this.Property(t => t.Numara).HasColumnName("Numara");
        }
    }
}
