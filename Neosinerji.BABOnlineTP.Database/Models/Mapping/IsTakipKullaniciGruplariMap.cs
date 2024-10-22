using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipKullaniciGruplariMap : EntityTypeConfiguration<IsTakipKullaniciGruplari>
    {
        public IsTakipKullaniciGruplariMap()
        {
            // Primary Key
            this.HasKey(t => t.KullaniciGrupId);

            // Properties
            this.Property(t => t.GrupAdi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Aciklama)
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("IsTakipKullaniciGruplari");
            this.Property(t => t.KullaniciGrupId).HasColumnName("KullaniciGrupId");
            this.Property(t => t.GrupAdi).HasColumnName("GrupAdi");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
