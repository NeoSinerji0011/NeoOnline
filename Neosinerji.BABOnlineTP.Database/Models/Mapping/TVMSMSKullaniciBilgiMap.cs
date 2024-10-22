using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMSMSKullaniciBilgiMap : EntityTypeConfiguration<TVMSMSKullaniciBilgi>
    {
        public TVMSMSKullaniciBilgiMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.KullaniciAdi)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.Sifre)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Gonderen)
                .IsRequired()
                .HasMaxLength(11);

            // Table & Column Mappings
            this.ToTable("TVMSMSKullaniciBilgi");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.KullaniciAdi).HasColumnName("KullaniciAdi");
            this.Property(t => t.Sifre).HasColumnName("Sifre");
            this.Property(t => t.Gonderen).HasColumnName("Gonderen");
            this.Property(t => t.SmsSuresiDK).HasColumnName("SmsSuresiDK");
        }
    }
}
