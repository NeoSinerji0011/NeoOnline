using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PaylasimliPoliceUretimMap : EntityTypeConfiguration<PaylasimliPoliceUretim>
    {
        public PaylasimliPoliceUretimMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.SigortaSirketNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.BransKodu)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.PoliceNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.YenilemeNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ZeylNo)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PaylasimliPoliceUretim");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TaliTVMKodu).HasColumnName("TaliTVMKodu");
            this.Property(t => t.SigortaSirketNo).HasColumnName("SigortaSirketNo");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.PoliceNo).HasColumnName("PoliceNo");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.ZeylNo).HasColumnName("ZeylNo");
            this.Property(t => t.TanzimTarihi).HasColumnName("TanzimTarihi");
            this.Property(t => t.BrutPrim).HasColumnName("BrutPrim");
            this.Property(t => t.NetPrim).HasColumnName("NetPrim");
            this.Property(t => t.PoliceKomisyonTutari).HasColumnName("PoliceKomisyonTutari");
            this.Property(t => t.TvmKomisyonTutari).HasColumnName("TvmKomisyonTutari");
            this.Property(t => t.TvmKomisyonOrani).HasColumnName("TvmKomisyonOrani");
            this.Property(t => t.TaliTvmKomisyonTutari).HasColumnName("TaliTvmKomisyonTutari");
            this.Property(t => t.TaliTvmKomisyonOrani).HasColumnName("TaliTvmKomisyonOrani");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            this.Property(t => t.KaydiEkleyenKullaniciKodu).HasColumnName("KaydiEkleyenKullaniciKodu");
        }
    }
}
