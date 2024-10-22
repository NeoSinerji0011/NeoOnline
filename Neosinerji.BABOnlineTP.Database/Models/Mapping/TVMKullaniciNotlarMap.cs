using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMKullaniciNotlarMap : EntityTypeConfiguration<TVMKullaniciNotlar>
    {
        public TVMKullaniciNotlarMap()
        {
            // Primary Key
            this.HasKey(t => t.KullaniciNotId);

            // Properties
            this.Property(t => t.Konu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Aciklama)
                .HasMaxLength(1000);

            // Table & Column Mappings
            this.ToTable("TVMKullaniciNotlar");
            this.Property(t => t.KullaniciNotId).HasColumnName("KullaniciNotId");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.EklemeTarihi).HasColumnName("EklemeTarihi");
            this.Property(t => t.DegistirmeTarihi).HasColumnName("DegistirmeTarihi");
            this.Property(t => t.Oncelik).HasColumnName("Oncelik");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");

            // Relationships
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.TVMKullaniciNotlars)
                .HasForeignKey(d => d.KullaniciKodu);

        }
    }
}
