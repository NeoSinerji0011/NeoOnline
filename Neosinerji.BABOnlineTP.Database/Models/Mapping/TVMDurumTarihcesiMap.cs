using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMDurumTarihcesiMap : EntityTypeConfiguration<TVMDurumTarihcesi>
    {
        public TVMDurumTarihcesiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TVMDurumTarihcesi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.DurumKodu).HasColumnName("DurumKodu");
            this.Property(t => t.BaslamaTarihi).HasColumnName("BaslamaTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
        }
    }
}
