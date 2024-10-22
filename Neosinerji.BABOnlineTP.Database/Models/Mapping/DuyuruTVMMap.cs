using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class DuyuruTVMMap : EntityTypeConfiguration<DuyuruTVM>
    {
        public DuyuruTVMMap()
        {
            // Primary Key
            this.HasKey(t => t.DuyuruTvmId);

            // Properties
            // Table & Column Mappings
            this.ToTable("DuyuruTVM");
            this.Property(t => t.DuyuruTvmId).HasColumnName("DuyuruTvmId");
            this.Property(t => t.DuyuruId).HasColumnName("DuyuruId");
            this.Property(t => t.TVMId).HasColumnName("TVMId");
            this.Property(t => t.EkleyenKullanici).HasColumnName("EkleyenKullanici");
            this.Property(t => t.EklemeTarihi).HasColumnName("EklemeTarihi");

            // Relationships
            this.HasRequired(t => t.Duyurular)
                .WithMany(t => t.DuyuruTVMs)
                .HasForeignKey(d => d.DuyuruId);
            this.HasRequired(t => t.TVMDetay)
                .WithMany(t => t.DuyuruTVMs)
                .HasForeignKey(d => d.TVMId);
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.DuyuruTVMs)
                .HasForeignKey(d => d.EkleyenKullanici);

        }
    }
}
