using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class MapfreKullaniciMap : EntityTypeConfiguration<MapfreKullanici>
    {
        public MapfreKullaniciMap()
        {
            // Primary Key
            this.HasKey(t => t.MapfeKullaniciId);

            // Properties
            this.Property(t => t.Bolge)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.TVMUnvan)
                .IsRequired()
                .HasMaxLength(255);

            this.Property(t => t.AnaPartaj)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.TaliPartaj)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.KullaniciAdi)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.EMail)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("MapfreKullanici");
            this.Property(t => t.MapfeKullaniciId).HasColumnName("MapfeKullaniciId");
            this.Property(t => t.Bolge).HasColumnName("Bolge");
            this.Property(t => t.TVMUnvan).HasColumnName("TVMUnvan");
            this.Property(t => t.AnaPartaj).HasColumnName("AnaPartaj");
            this.Property(t => t.TaliPartaj).HasColumnName("TaliPartaj");
            this.Property(t => t.KullaniciAdi).HasColumnName("KullaniciAdi");
            this.Property(t => t.EMail).HasColumnName("EMail");
            this.Property(t => t.Olusturuldu).HasColumnName("Olusturuldu");
        }
    }
}
