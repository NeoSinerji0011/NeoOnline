using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DuyuruDokumanMap : EntityTypeConfiguration<DuyuruDokuman>
    {
        public DuyuruDokumanMap()
        {
            // Primary Key
            this.HasKey(t => t.DuyuruDokumanId);

            // Properties
            this.Property(t => t.DosyaAdi)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.DokumanURL)
                .IsRequired()
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("DuyuruDokuman");
            this.Property(t => t.DuyuruDokumanId).HasColumnName("DuyuruDokumanId");
            this.Property(t => t.DuyuruId).HasColumnName("DuyuruId");
            this.Property(t => t.DosyaAdi).HasColumnName("DosyaAdi");
            this.Property(t => t.DokumanURL).HasColumnName("DokumanURL");
            this.Property(t => t.EkleyenKullanici).HasColumnName("EkleyenKullanici");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

            // Relationships
            this.HasRequired(t => t.Duyurular)
                .WithMany(t => t.DuyuruDokumen)
                .HasForeignKey(d => d.DuyuruId);
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.DuyuruDokumen)
                .HasForeignKey(d => d.EkleyenKullanici);

        }
    }
}
