using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class IsTakipMap : EntityTypeConfiguration<IsTakip>
    {
        public IsTakipMap()
        {
            // Primary Key
            this.HasKey(t => t.IsTakipId);

            // Properties
            // Table & Column Mappings
            this.ToTable("IsTakip");
            this.Property(t => t.IsTakipId).HasColumnName("IsTakipId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.Asama).HasColumnName("Asama");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.TVMKullaniciId).HasColumnName("TVMKullaniciId");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
        }
    }
}
