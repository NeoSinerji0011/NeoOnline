using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DaskKurumlarMap : EntityTypeConfiguration<DaskKurumlar>
    {
        public DaskKurumlarMap()
        {
            // Primary Key
            this.HasKey(t => t.KurumKodu);

            // Properties
            this.Property(t => t.KurumKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KurumAdi)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(60);

            // Table & Column Mappings
            this.ToTable("DaskKurumlar");
            this.Property(t => t.KurumKodu).HasColumnName("KurumKodu");
            this.Property(t => t.KurumAdi).HasColumnName("KurumAdi");
        }
    }
}
