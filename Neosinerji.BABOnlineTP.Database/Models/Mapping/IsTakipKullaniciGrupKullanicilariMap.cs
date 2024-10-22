using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipKullaniciGrupKullanicilariMap : EntityTypeConfiguration<IsTakipKullaniciGrupKullanicilari>
    {
        public IsTakipKullaniciGrupKullanicilariMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            // Table & Column Mappings
            this.ToTable("IsTakipKullaniciGrupKullanicilari");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.IsTakipKullaniciGrupId).HasColumnName("IsTakipKullaniciGrupId");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");

            // Relationships
            this.HasRequired(t => t.IsTakipKullaniciGruplari)
                .WithMany(t => t.IsTakipKullaniciGrupKullanicilaris)
                .HasForeignKey(d => d.IsTakipKullaniciGrupId);
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.IsTakipKullaniciGrupKullanicilaris)
                .HasForeignKey(d => d.KullaniciKodu);

        }
    }
}
