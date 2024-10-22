using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMKullaniciSifreTarihcesiMap : EntityTypeConfiguration<TVMKullaniciSifreTarihcesi>
    {
        public TVMKullaniciSifreTarihcesiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKodu, t.TVMKullaniciKodu, t.SifreDegistirmeNo });

            // Properties
            this.Property(t => t.TVMKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TVMKullaniciKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SifreDegistirmeNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.OncekiSifre)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.YeniSifre)
                .IsRequired()
                .HasMaxLength(30);

            // Table & Column Mappings
            this.ToTable("TVMKullaniciSifreTarihcesi");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMKullaniciKodu).HasColumnName("TVMKullaniciKodu");
            this.Property(t => t.SifreDegistirmeNo).HasColumnName("SifreDegistirmeNo");
            this.Property(t => t.DegistirmeTarihi).HasColumnName("DegistirmeTarihi");
            this.Property(t => t.OncekiSifre).HasColumnName("OncekiSifre");
            this.Property(t => t.YeniSifre).HasColumnName("YeniSifre");
        }
    }
}
