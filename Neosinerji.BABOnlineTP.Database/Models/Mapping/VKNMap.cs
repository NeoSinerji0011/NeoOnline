using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class VKNMap : EntityTypeConfiguration<VKN>
    {
        public VKNMap()
        {
            // Primary Key
            this.HasKey(t => t.VergiNo);

            // Properties
            this.Property(t => t.BabaAdi)
                .HasMaxLength(50);

            this.Property(t => t.Durum)
                .HasMaxLength(50);

            this.Property(t => t.Adi)
                .HasMaxLength(50);

            this.Property(t => t.Soyadi)
                .HasMaxLength(50);

            this.Property(t => t.VergiNo)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.SirketTuru)
                .HasMaxLength(50);

            this.Property(t => t.DogumYeri)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("VKN");
            this.Property(t => t.DogumTarihi).HasColumnName("DogumTarihi");
            this.Property(t => t.BabaAdi).HasColumnName("BabaAdi");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.Adi).HasColumnName("Adi");
            this.Property(t => t.Soyadi).HasColumnName("Soyadi");
            this.Property(t => t.VergiNo).HasColumnName("VergiNo");
            this.Property(t => t.SirketTuru).HasColumnName("SirketTuru");
            this.Property(t => t.DogumYeri).HasColumnName("DogumYeri");
        }
    }
}
