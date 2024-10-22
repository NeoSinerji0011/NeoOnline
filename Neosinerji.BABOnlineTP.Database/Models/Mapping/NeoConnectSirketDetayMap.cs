using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class NeoConnectSirketDetayMap : EntityTypeConfiguration<NeoConnectSirketDetay>
    {
        public NeoConnectSirketDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SigortaSirketAdi)
                .HasMaxLength(150);

            this.Property(t => t.InputTextKullaniciId)
                .HasMaxLength(50);

            this.Property(t => t.InputTextAcenteKoduId)
                .HasMaxLength(50);

            this.Property(t => t.InputTextSifreId)
                .HasMaxLength(50);

            this.Property(t => t.InputTextGirisId)
                .HasMaxLength(50);

            this.Property(t => t.LoginUrl)
                .HasMaxLength(400);

            // Table & Column Mappings
            this.ToTable("NeoConnectSirketDetay");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.SigortaSirketAdi).HasColumnName("SigortaSirketAdi");
            this.Property(t => t.InputTextKullaniciId).HasColumnName("InputTextKullaniciId");
            this.Property(t => t.InputTextAcenteKoduId).HasColumnName("InputTextAcenteKoduId");
            this.Property(t => t.InputTextSifreId).HasColumnName("InputTextSifreId");
            this.Property(t => t.InputTextGirisId).HasColumnName("InputTextGirisId");
            this.Property(t => t.LoginUrl).HasColumnName("LoginUrl");
        }
    }
}
