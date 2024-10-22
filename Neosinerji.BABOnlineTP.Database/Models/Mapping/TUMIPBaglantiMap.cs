using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TUMIPBaglantiMap : EntityTypeConfiguration<TUMIPBaglanti>
    {
        public TUMIPBaglantiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TUMKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BaslangicIP)
                .IsRequired()
                .HasMaxLength(15);

            this.Property(t => t.BitisIP)
                .IsRequired()
                .HasMaxLength(15);

            // Table & Column Mappings
            this.ToTable("TUMIPBaglanti");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.BaslangicIP).HasColumnName("BaslangicIP");
            this.Property(t => t.BitisIP).HasColumnName("BitisIP");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
