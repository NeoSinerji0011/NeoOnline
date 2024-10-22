using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class KaskoTasinanYukKademeleriMap : EntityTypeConfiguration<KaskoTasinanYukKademeleri>
    {
        public KaskoTasinanYukKademeleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.KullanimTarziKodu)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.KullanimTarziKodu2)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Kademe)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Aciklama)
                .IsRequired()
                .HasMaxLength(500);

            // Table & Column Mappings
            this.ToTable("KaskoTasinanYukKademeleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.KullanimTarziKodu2).HasColumnName("KullanimTarziKodu2");
            this.Property(t => t.Kademe).HasColumnName("Kademe");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
        }
    }
}
