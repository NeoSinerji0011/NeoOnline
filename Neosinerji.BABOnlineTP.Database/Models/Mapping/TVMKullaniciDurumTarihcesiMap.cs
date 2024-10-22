using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMKullaniciDurumTarihcesiMap : EntityTypeConfiguration<TVMKullaniciDurumTarihcesi>
    {
        public TVMKullaniciDurumTarihcesiMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKullaniciKodu, t.SiraNo });

            // Properties
            this.Property(t => t.TVMKullaniciKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.SiraNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TVMKullaniciDurumTarihcesi");
            this.Property(t => t.TVMKullaniciKodu).HasColumnName("TVMKullaniciKodu");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.BaslamaTarihi).HasColumnName("BaslamaTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

            // Relationships
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.TVMKullaniciDurumTarihcesis)
                .HasForeignKey(d => d.TVMKullaniciKodu);

        }
    }
}
