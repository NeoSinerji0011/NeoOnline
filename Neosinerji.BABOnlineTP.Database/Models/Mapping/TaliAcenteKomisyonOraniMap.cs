using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TaliAcenteKomisyonOraniMap : EntityTypeConfiguration<TaliAcenteKomisyonOrani>
    {
        public TaliAcenteKomisyonOraniMap()
        {
            // Primary Key
            this.HasKey(t => t.KomisyonOranId);

            // Properties
            this.Property(t => t.SigortaSirketKodu)
                .HasMaxLength(5);

            // Table & Column Mappings
            this.ToTable("TaliAcenteKomisyonOrani");
            this.Property(t => t.KomisyonOranId).HasColumnName("KomisyonOranId");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TaliTVMKodu).HasColumnName("TaliTVMKodu");
            this.Property(t => t.SigortaSirketKodu).HasColumnName("SigortaSirketKodu");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.GecirlilikBaslangicTarihi).HasColumnName("GecirlilikBaslangicTarihi");
            this.Property(t => t.KomisyonOrani).HasColumnName("KomisyonOrani");
            this.Property(t => t.KomisyonOran).HasColumnName("KomisyonOran");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.KayitKullaniciKodu).HasColumnName("KayitKullaniciKodu");
            this.Property(t => t.GuncellemeTarihi).HasColumnName("GuncellemeTarihi");
            this.Property(t => t.GuncellemeKullaniciKodu).HasColumnName("GuncellemeKullaniciKodu");
            this.Property(t => t.MinUretim).HasColumnName("MinUretim");
            this.Property(t => t.MaxUretim).HasColumnName("MaxUretim");
            this.Property(t => t.DisKaynakKodu).HasColumnName("DisKaynakKodu");
        }
    }
}
