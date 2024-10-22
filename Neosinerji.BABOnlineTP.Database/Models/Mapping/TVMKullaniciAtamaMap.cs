using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMKullaniciAtamaMap : EntityTypeConfiguration<TVMKullaniciAtama>
    {
        public TVMKullaniciAtamaMap()
        {
            // Primary Key
            this.HasKey(t => new { t.TVMKullaniciKodu, t.AtamaNo });

            // Properties
            this.Property(t => t.TVMKullaniciKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.AtamaNo)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            this.ToTable("TVMKullaniciAtama");
            this.Property(t => t.TVMKullaniciKodu).HasColumnName("TVMKullaniciKodu");
            this.Property(t => t.AtamaNo).HasColumnName("AtamaNo");
            this.Property(t => t.AtandigiDepartmanKodu).HasColumnName("AtandigiDepartmanKodu");
            this.Property(t => t.OncekiDepartmanKodu).HasColumnName("OncekiDepartmanKodu");
            this.Property(t => t.BaslamaTarihi).HasColumnName("BaslamaTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.AtamaTarihi).HasColumnName("AtamaTarihi");

            // Relationships
            this.HasRequired(t => t.TVMKullanicilar)
                .WithMany(t => t.TVMKullaniciAtamas)
                .HasForeignKey(d => d.TVMKullaniciKodu);

        }
    }
}
