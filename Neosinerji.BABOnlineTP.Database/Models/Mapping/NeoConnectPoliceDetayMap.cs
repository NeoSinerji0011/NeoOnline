using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class NeoConnectPoliceDetayMap : EntityTypeConfiguration<NeoConnectPoliceDetay>
    {
        public NeoConnectPoliceDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.PoliceNo)
                .HasMaxLength(50);

            this.Property(t => t.SirketNo)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("NeoConnectPoliceDetay");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.AltTvmKodu).HasColumnName("AltTvmKodu");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");
            this.Property(t => t.PoliceNo).HasColumnName("PoliceNo");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.EkNo).HasColumnName("EkNo");
            this.Property(t => t.SirketNo).HasColumnName("SirketNo");
        }
    }
}
