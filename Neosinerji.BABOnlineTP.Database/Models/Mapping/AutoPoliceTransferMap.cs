using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class AutoPoliceTransferMap : EntityTypeConfiguration<AutoPoliceTransfer>
    {
        public AutoPoliceTransferMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SirketKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.PoliceTransferUrl)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("AutoPoliceTransfer");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.SirketKodu).HasColumnName("SirketKodu");
            this.Property(t => t.PoliceTransferUrl).HasColumnName("PoliceTransferUrl");
            this.Property(t => t.TanzimBaslangicTarihi).HasColumnName("TanzimBaslangicTarihi");
            this.Property(t => t.TanzimBitisTarihi).HasColumnName("TanzimBitisTarihi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.KaydiEkleyenKullaniciKodu).HasColumnName("KaydiEkleyenKullaniciKodu");
        }
    }
}
