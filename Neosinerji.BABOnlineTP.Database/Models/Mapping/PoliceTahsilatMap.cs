using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceTahsilatMap : EntityTypeConfiguration<PoliceTahsilat>
    {
        public PoliceTahsilatMap()
        {
            // Primary Key
            this.HasKey(t => t.TahsilatId);

            // Properties
            this.Property(t => t.KimlikNo)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(11);

            this.Property(t => t.PoliceNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ZeyilNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.OdemeBelgeNo)
                .HasMaxLength(50);

            this.Property(t => t.Dekont_EvrakNo)
               .HasMaxLength(75);

            this.Property(t => t.CariHesapKodu)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("PoliceTahsilat");
            this.Property(t => t.TahsilatId).HasColumnName("TahsilatId");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.KimlikNo).HasColumnName("KimlikNo");
            this.Property(t => t.PoliceNo).HasColumnName("PoliceNo");
            this.Property(t => t.ZeyilNo).HasColumnName("ZeyilNo");
            this.Property(t => t.BrutPrim).HasColumnName("BrutPrim");
            this.Property(t => t.OdemTipi).HasColumnName("OdemTipi");
            this.Property(t => t.TaksitNo).HasColumnName("TaksitNo");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.TaksitVadeTarihi).HasColumnName("TaksitVadeTarihi");
            this.Property(t => t.TaksitTutari).HasColumnName("TaksitTutari");
            this.Property(t => t.OdenenTutar).HasColumnName("OdenenTutar");
            this.Property(t => t.OdemeBelgeNo).HasColumnName("OdemeBelgeNo");
            this.Property(t => t.Dekont_EvrakNo).HasColumnName("Dekont_EvrakNo");
            this.Property(t => t.OdemeBelgeTarihi).HasColumnName("OdemeBelgeTarihi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            this.Property(t => t.KaydiEkleyenKullaniciKodu).HasColumnName("KaydiEkleyenKullaniciKodu");
            this.Property(t => t.KalanTaksitTutari).HasColumnName("KalanTaksitTutari");
            this.Property(t => t.OtomatikTahsilatiKkMi).HasColumnName("OtomatikTahsilatiKkMi");
            this.Property(t => t.CariHesapKodu).HasColumnName("CariHesapKodu");

            // Relationships
            this.HasRequired(t => t.PoliceGenel)
                .WithMany(t => t.PoliceTahsilats)
                .HasForeignKey(d => d.PoliceId);

        }
    }
}
