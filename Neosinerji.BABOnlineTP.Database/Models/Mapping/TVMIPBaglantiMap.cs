using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMIPBaglantiMap : EntityTypeConfiguration<TVMIPBaglanti>
    {
        public TVMIPBaglantiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.BaslangicIP)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.BitisIP)
                .IsRequired()
                .HasMaxLength(25);

            // Table & Column Mappings
            this.ToTable("TVMIPBaglanti");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.BaslangicIP).HasColumnName("BaslangicIP");
            this.Property(t => t.BitisIP).HasColumnName("BitisIP");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.Durum).HasColumnName("Durum");
        }
    }
}
