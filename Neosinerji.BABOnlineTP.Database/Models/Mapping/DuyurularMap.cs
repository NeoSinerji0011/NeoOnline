using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DuyurularMap : EntityTypeConfiguration<Duyurular>
    {
        public DuyurularMap()
        {
            // Primary Key
            this.HasKey(t => t.DuyuruId);

            // Properties
            this.Property(t => t.Konu)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Aciklama)
                .HasMaxLength(2000);

            // Table & Column Mappings
            this.ToTable("Duyurular");
            this.Property(t => t.DuyuruId).HasColumnName("DuyuruId");
            this.Property(t => t.Konu).HasColumnName("Konu");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.EkleyenKullanici).HasColumnName("EkleyenKullanici");
            this.Property(t => t.EklemeTarihi).HasColumnName("EklemeTarihi");
            this.Property(t => t.DegistirenKullanici).HasColumnName("DegistirenKullanici");
            this.Property(t => t.DegistirmeTarihi).HasColumnName("DegistirmeTarihi");
            this.Property(t => t.Oncelik).HasColumnName("Oncelik");
            this.Property(t => t.BaslangisTarihi).HasColumnName("BaslangisTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");

            // Relationships
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.Duyurulars)
                .HasForeignKey(d => d.EkleyenKullanici);
            this.HasOptional(t => t.TVMKullanicilar1)
                .WithMany(t => t.Duyurulars1)
                .HasForeignKey(d => d.DegistirenKullanici);

        }
    }
}
