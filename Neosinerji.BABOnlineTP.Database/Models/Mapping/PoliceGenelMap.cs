using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceGenelMap : EntityTypeConfiguration<PoliceGenel>
    {
        public PoliceGenelMap()
        {
            // Primary Key
            this.HasKey(t => t.PoliceId);

            // Properties
            this.Property(t => t.UrunAdi)
                .HasMaxLength(30);

            this.Property(t => t.BransAdi)
                .HasMaxLength(30);

            this.Property(t => t.TUMUrunKodu)
                .HasMaxLength(50);

            this.Property(t => t.TUMUrunAdi)
                .HasMaxLength(200);

            this.Property(t => t.TUMBransKodu)
                .HasMaxLength(20);

            this.Property(t => t.TUMBransAdi)
                .HasMaxLength(150);

            this.Property(t => t.TUMBirlikKodu)
                .HasMaxLength(5);

            this.Property(t => t.PoliceNumarasi)
                .HasMaxLength(50);

            this.Property(t => t.ParaBirimi)
                .HasMaxLength(5);

            this.Property(t => t.HashCode)
                .IsFixedLength()
                .HasMaxLength(200);

            this.Property(t => t.ZeyilAdi)
                .HasMaxLength(50);

            this.Property(t => t.GrupZeyilNo)
                .HasMaxLength(25);

            this.Property(t => t.ZeyilKodu)
                .HasMaxLength(10);

            this.Property(t => t.SirketZeyilAdi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PoliceGenel");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.UrunAdi).HasColumnName("UrunAdi");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.BransAdi).HasColumnName("BransAdi");
            this.Property(t => t.TUMUrunKodu).HasColumnName("TUMUrunKodu");
            this.Property(t => t.TUMUrunAdi).HasColumnName("TUMUrunAdi");
            this.Property(t => t.TUMBransKodu).HasColumnName("TUMBransKodu");
            this.Property(t => t.TUMBransAdi).HasColumnName("TUMBransAdi");
            this.Property(t => t.TUMBirlikKodu).HasColumnName("TUMBirlikKodu");
            this.Property(t => t.PoliceNumarasi).HasColumnName("PoliceNumarasi");
            this.Property(t => t.EkNo).HasColumnName("EkNo");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.TanzimTarihi).HasColumnName("TanzimTarihi");
            this.Property(t => t.BaslangicTarihi).HasColumnName("BaslangicTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.BrutPrim).HasColumnName("BrutPrim");
            this.Property(t => t.NetPrim).HasColumnName("NetPrim");
            this.Property(t => t.ToplamVergi).HasColumnName("ToplamVergi");
            this.Property(t => t.Komisyon).HasColumnName("Komisyon");
            this.Property(t => t.DovizliBrutPrim).HasColumnName("DovizliBrutPrim");
            this.Property(t => t.DovizliNetPrim).HasColumnName("DovizliNetPrim");
            this.Property(t => t.DovizliKomisyon).HasColumnName("DovizliKomisyon");
            this.Property(t => t.OdemeSekli).HasColumnName("OdemeSekli");
            this.Property(t => t.ParaBirimi).HasColumnName("ParaBirimi");
            this.Property(t => t.TaliAcenteKodu).HasColumnName("TaliAcenteKodu");
            this.Property(t => t.UretimTaliAcenteKodu).HasColumnName("UretimTaliAcenteKodu");
            this.Property(t => t.HashCode).HasColumnName("HashCode");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.TaliKomisyonOrani).HasColumnName("TaliKomisyonOrani");
            this.Property(t => t.TaliKomisyonOran).HasColumnName("TaliKomisyonOran");
            this.Property(t => t.TaliKomisyon).HasColumnName("TaliKomisyon");
            this.Property(t => t.TaliKomisyonGuncelleyenKullanici).HasColumnName("TaliKomisyonGuncelleyenKullanici");
            this.Property(t => t.TaliKomisyonGuncellemeTarihi).HasColumnName("TaliKomisyonGuncellemeTarihi");
            this.Property(t => t.ZeyilAdi).HasColumnName("ZeyilAdi");
            this.Property(t => t.GrupZeyilNo).HasColumnName("GrupZeyilNo");
            this.Property(t => t.EkKomisyonTutari).HasColumnName("EkKomisyonTutari");
            this.Property(t => t.ZeyilKodu).HasColumnName("ZeyilKodu");
            this.Property(t => t.SirketZeyilAdi).HasColumnName("SirketZeyilAdi");
            this.Property(t => t.DovizKuru).HasColumnName("DovizKuru");
            this.Property(t => t.DovizKur).HasColumnName("DovizKur").HasPrecision(10, 4);
            this.Property(t => t.CariHareketKayitTarihi).HasColumnName("CariHareketKayitTarihi");
            this.Property(t => t.MuhasebeyeAktarildiMi).HasColumnName("MuhasebeyeAktarildiMi");
            this.Property(t => t.Yeni_is).HasColumnName("Yeni_is");
            this.Property(t => t.OnaylayanKullaniciKodu).HasColumnName("OnaylayanKullaniciKodu");



            // Relationships
            this.HasOptional(t => t.SigortaSirketleri)
                .WithMany(t => t.PoliceGenels)
                .HasForeignKey(d => d.TUMBirlikKodu);

            this.HasOptional(t => t.TVMDetay)
                .WithMany(t => t.PoliceGenels)
                .HasForeignKey(d => d.TaliAcenteKodu);

            this.HasOptional(t => t.TVMKullanicilar)
                .WithMany(t => t.PoliceGenels)
                .HasForeignKey(d => d.OnaylayanKullaniciKodu);

        }
    }
}
