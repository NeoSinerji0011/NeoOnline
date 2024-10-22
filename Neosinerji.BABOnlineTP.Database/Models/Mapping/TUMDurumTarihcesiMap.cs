using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TUMDurumTarihcesiMap : EntityTypeConfiguration<TUMDurumTarihcesi>
    {
        public TUMDurumTarihcesiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TUMDurumTarihcesi");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.DurumKodu).HasColumnName("DurumKodu");
            this.Property(t => t.BaslamaTarihi).HasColumnName("BaslamaTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
        }
    }
}
