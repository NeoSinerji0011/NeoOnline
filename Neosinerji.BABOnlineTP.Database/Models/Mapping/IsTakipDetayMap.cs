using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipDetayMap : EntityTypeConfiguration<IsTakipDetay>
    {
        public IsTakipDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.IsTakipDetayId);

            // Properties
            // Table & Column Mappings
            this.ToTable("IsTakipDetay");
            this.Property(t => t.IsTakipDetayId).HasColumnName("IsTakipDetayId");
            this.Property(t => t.IsTakipId).HasColumnName("IsTakipId");
            this.Property(t => t.IsTipiDetayId).HasColumnName("IsTipiDetayId");
            this.Property(t => t.TvmKullaniciId).HasColumnName("TvmKullaniciId");
            this.Property(t => t.HareketTipi).HasColumnName("HareketTipi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

            // Relationships
            this.HasRequired(t => t.IsTakip)
                .WithMany(t => t.IsTakipDetays)
                .HasForeignKey(d => d.IsTakipId);

        }
    }
}
