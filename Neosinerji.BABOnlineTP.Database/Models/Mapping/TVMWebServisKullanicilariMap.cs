using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMWebServisKullanicilariMap : EntityTypeConfiguration<TVMWebServisKullanicilari>
    {
        public TVMWebServisKullanicilariMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.TUMKodu });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TUMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KullaniciAdi)
                .HasMaxLength(50);

            this.Property(t => t.Sifre)
                .HasMaxLength(50);

            this.Property(t => t.KullaniciAdi2)
                .HasMaxLength(50);

            this.Property(t => t.Sifre2)
                .HasMaxLength(50);

            this.Property(t => t.PartajNo_)
                .HasMaxLength(20);

            this.Property(t => t.SubAgencyCode)
                .HasMaxLength(20);

            this.Property(t => t.SourceId)
                .HasMaxLength(20);

            this.Property(t => t.CompanyId)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TVMWebServisKullanicilari");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.KullaniciAdi).HasColumnName("KullaniciAdi");
            this.Property(t => t.Sifre).HasColumnName("Sifre");
            this.Property(t => t.KullaniciAdi2).HasColumnName("KullaniciAdi2");
            this.Property(t => t.Sifre2).HasColumnName("Sifre2");
            this.Property(t => t.PartajNo_).HasColumnName("PartajNo ");
            this.Property(t => t.SubAgencyCode).HasColumnName("SubAgencyCode");
            this.Property(t => t.SourceId).HasColumnName("SourceId");
            this.Property(t => t.CompanyId).HasColumnName("CompanyId");
        }
    }
}
