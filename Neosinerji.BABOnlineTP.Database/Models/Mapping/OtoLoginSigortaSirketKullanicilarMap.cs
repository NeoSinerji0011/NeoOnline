using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class OtoLoginSigortaSirketKullanicilarMap : EntityTypeConfiguration<OtoLoginSigortaSirketKullanicilar>
    {
        public OtoLoginSigortaSirketKullanicilarMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SigortaSirketAdi)
                .HasMaxLength(150);

            this.Property(t => t.KullaniciAdi)
                .HasMaxLength(50);

            this.Property(t => t.AcenteKodu)
                .HasMaxLength(50);

            this.Property(t => t.Sifre)
                .HasMaxLength(50);

            this.Property(t => t.InputTextKullaniciId)
                .HasMaxLength(400);

            this.Property(t => t.InputTextAcenteKoduId)
                .HasMaxLength(400);

            this.Property(t => t.InputTextSifreId)
                .HasMaxLength(400);

            this.Property(t => t.InputTextGirisId)
                .HasMaxLength(400);

            this.Property(t => t.LoginUrl)
                .HasMaxLength(400);

            this.Property(t => t.ProxyIpPort)
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("OtoLoginSigortaSirketKullanicilar");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.AltTVMKodu).HasColumnName("AltTVMKodu");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.SigortaSirketAdi).HasColumnName("SigortaSirketAdi");
            this.Property(t => t.KullaniciAdi).HasColumnName("KullaniciAdi");
            this.Property(t => t.AcenteKodu).HasColumnName("AcenteKodu");
            this.Property(t => t.Sifre).HasColumnName("Sifre");
            this.Property(t => t.InputTextKullaniciId).HasColumnName("InputTextKullaniciId");
            this.Property(t => t.InputTextAcenteKoduId).HasColumnName("InputTextAcenteKoduId");
            this.Property(t => t.InputTextSifreId).HasColumnName("InputTextSifreId");
            this.Property(t => t.InputTextGirisId).HasColumnName("InputTextGirisId");
            this.Property(t => t.LoginUrl).HasColumnName("LoginUrl");
            this.Property(t => t.ProxyIpPort).HasColumnName("ProxyIpPort");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.GrupKodu).HasColumnName("GrupKodu");
        }
    }
}
