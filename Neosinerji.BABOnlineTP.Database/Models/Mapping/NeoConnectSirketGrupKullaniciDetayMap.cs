using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class NeoConnectSirketGrupKullaniciDetayMap : EntityTypeConfiguration<NeoConnectSirketGrupKullaniciDetay>
    {
        public NeoConnectSirketGrupKullaniciDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.GrupKodu);

            // Properties
            this.Property(t => t.GrupAdi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.SirketKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.KullaniciAdi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Sifre)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("NeoConnectSirketGrupKullaniciDetay");
            this.Property(t => t.GrupKodu).HasColumnName("GrupKodu");
            this.Property(t => t.GrupAdi).HasColumnName("GrupAdi");
            this.Property(t => t.SirketKodu).HasColumnName("SirketKodu");
            this.Property(t => t.KullaniciAdi).HasColumnName("KullaniciAdi");
            this.Property(t => t.Sifre).HasColumnName("Sifre");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
        }
    }
}
