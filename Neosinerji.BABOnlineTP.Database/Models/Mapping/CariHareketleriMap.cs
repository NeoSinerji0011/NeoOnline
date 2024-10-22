using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class CariHareketleriMap : EntityTypeConfiguration<CariHareketleri>
    {
        public CariHareketleriMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            this.Property(t => t.CariHesapKodu)
            .HasMaxLength(20);

            // Properties
            this.Property(t => t.EvrakNo).HasMaxLength(50);

            this.Property(t => t.MusteriGrupKodu)
                .HasMaxLength(60);

            this.Property(t => t.DovizTipi)
                .HasMaxLength(10);

            this.Property(t => t.Aciklama)
                .HasMaxLength(700);

            // Table & Column Mappings
            this.ToTable("CariHareketleri");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.CariHesapKodu).HasColumnName("CariHesapKodu");
            this.Property(t => t.CariHareketTarihi).HasColumnName("CariHareketTarihi");
            this.Property(t => t.OdemeTarihi).HasColumnName("OdemeTarihi");
            this.Property(t => t.EvrakTipi).HasColumnName("EvrakTipi");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");
            this.Property(t => t.EvrakNo).HasColumnName("EvrakNo");
            this.Property(t => t.Tutar).HasColumnName("Tutar");
            this.Property(t => t.MasrafMerkezi).HasColumnName("MasrafMerkezi");
            this.Property(t => t.MusteriGrupKodu).HasColumnName("MusteriGrupKodu");
            this.Property(t => t.DovizTipi).HasColumnName("DovizTipi");
            this.Property(t => t.DovizKuru).HasColumnName("DovizKuru").HasPrecision(10, 4);
            this.Property(t => t.DovizTutari).HasColumnName("DovizTutari").HasPrecision(12, 2);
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");

        }
    }
}
