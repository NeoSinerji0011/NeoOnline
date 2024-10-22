using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DepremMuafiyetMap : EntityTypeConfiguration<DepremMuafiyet>
    {
        public DepremMuafiyetMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TeminatKodu, t.YazlikKislik, t.Kademe });

            // Properties
            this.Property(t => t.TeminatKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("DepremMuafiyet");
            this.Property(t => t.TeminatKodu).HasColumnName("TeminatKodu");
            this.Property(t => t.YazlikKislik).HasColumnName("YazlikKislik");
            this.Property(t => t.Kademe).HasColumnName("Kademe");
            this.Property(t => t.Fiyat).HasColumnName("Fiyat");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
