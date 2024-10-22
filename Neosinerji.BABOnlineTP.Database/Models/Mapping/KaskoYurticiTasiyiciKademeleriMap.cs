using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class KaskoYurticiTasiyiciKademeleriMap : EntityTypeConfiguration<KaskoYurticiTasiyiciKademeleri>
    {
        public KaskoYurticiTasiyiciKademeleriMap()
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

            this.Property(t => t.KullanimTarziAciklama)
                .HasMaxLength(150);

            this.Property(t => t.KullanimTarziAciklama2)
                .HasMaxLength(150);

            // Table & Column Mappings
            this.ToTable("KaskoYurticiTasiyiciKademeleri");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.KullanimTarziKodu).HasColumnName("KullanimTarziKodu");
            this.Property(t => t.KullanimTarziKodu2).HasColumnName("KullanimTarziKodu2");
            this.Property(t => t.Kademe).HasColumnName("Kademe");
            this.Property(t => t.YillikLimit).HasColumnName("YillikLimit");
            this.Property(t => t.SeferLimiti).HasColumnName("SeferLimiti");
            this.Property(t => t.Prim).HasColumnName("Prim");
            this.Property(t => t.KullanimTarziAciklama).HasColumnName("KullanimTarziAciklama");
            this.Property(t => t.KullanimTarziAciklama2).HasColumnName("KullanimTarziAciklama2");
        }
    }
}
