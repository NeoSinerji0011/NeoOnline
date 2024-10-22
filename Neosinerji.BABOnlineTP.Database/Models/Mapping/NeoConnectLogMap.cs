using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class NeoConnectLogMap : EntityTypeConfiguration<NeoConnectLog>
    {
        public NeoConnectLogMap()
        {
            // Primary Key
            this.HasKey(t => t.LogId);

            // Properties
            this.Property(t => t.Kullanici)
                .HasMaxLength(200);

            this.Property(t => t.SigortaSirketKodu)
                .HasMaxLength(10);

            this.Property(t => t.IPAdresi)
                .HasMaxLength(200);

            this.Property(t => t.MACAdresi)
                .HasMaxLength(200);

            this.Property(t => t.SirketKullaniciAdi)
                .HasMaxLength(350);

            this.Property(t => t.SirketKullaniciSifresi)
                .HasMaxLength(350);

            // Table & Column Mappings
            this.ToTable("NeoConnectLog");
            this.Property(t => t.LogId).HasColumnName("LogId");
            this.Property(t => t.TvmKodu).HasColumnName("TvmKodu");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");
            this.Property(t => t.Kullanici).HasColumnName("Kullanici");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.IPAdresi).HasColumnName("IPAdresi");
            this.Property(t => t.MACAdresi).HasColumnName("MACAdresi");
            this.Property(t => t.KullaniciGirisTarihi).HasColumnName("KullaniciGirisTarihi");
            this.Property(t => t.KullaniciCikisTarihi).HasColumnName("KullaniciCikisTarihi");
            this.Property(t => t.SirketKullaniciAdi).HasColumnName("SirketKullaniciAdi");
            this.Property(t => t.SirketKullaniciSifresi).HasColumnName("SirketKullaniciSifresi");
            this.Property(t => t.GrupKodu).HasColumnName("GrupKodu");
        }
    }
}
