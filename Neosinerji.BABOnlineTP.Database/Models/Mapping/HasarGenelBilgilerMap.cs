using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class HasarGenelBilgilerMap : EntityTypeConfiguration<HasarGenelBilgiler>
    {
        public HasarGenelBilgilerMap()
        {
            // Primary Key
            this.HasKey(t => t.HasarId);

            // Properties
            this.Property(t => t.BransKodu)
                .HasMaxLength(10);

            this.Property(t => t.UrunKodu)
                .HasMaxLength(10);

            this.Property(t => t.SigortaSirketNo)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.PoliceNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.YenilemeNo)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.ZeyilNo)
                .IsRequired()
                .HasMaxLength(5);

            this.Property(t => t.SigortaliTCVKN)
                .HasMaxLength(11);

            this.Property(t => t.PlakaNo)
                .HasMaxLength(25);

            this.Property(t => t.HasarDosyaNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.HasarSaati)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.HasarMevki)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.HasarMevkiEnlemKodu)
                .HasMaxLength(20);

            this.Property(t => t.HasarMevkiBoylamKodu)
                .HasMaxLength(20);

            this.Property(t => t.HasarTuruNedeni)
                .IsRequired()
                .HasMaxLength(250);

            this.Property(t => t.RedNedeni)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("HasarGenelBilgiler");
            this.Property(t => t.HasarId).HasColumnName("HasarId");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.SigortaSirketNo).HasColumnName("SigortaSirketNo");
            this.Property(t => t.PoliceNo).HasColumnName("PoliceNo");
            this.Property(t => t.YenilemeNo).HasColumnName("YenilemeNo");
            this.Property(t => t.ZeyilNo).HasColumnName("ZeyilNo");
            this.Property(t => t.SigortaliTCVKN).HasColumnName("SigortaliTCVKN");
            this.Property(t => t.PlakaNo).HasColumnName("PlakaNo");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.AltTVMKodu).HasColumnName("AltTVMKodu");
            this.Property(t => t.KullaniciKodu).HasColumnName("KullaniciKodu");
            this.Property(t => t.HasarDosyaNo).HasColumnName("HasarDosyaNo");
            this.Property(t => t.IhbarTarihi).HasColumnName("IhbarTarihi");
            this.Property(t => t.HasarTarihi).HasColumnName("HasarTarihi");
            this.Property(t => t.HasarSaati).HasColumnName("HasarSaati");
            this.Property(t => t.HasarMevki).HasColumnName("HasarMevki");
            this.Property(t => t.HasarMevkiEnlemKodu).HasColumnName("HasarMevkiEnlemKodu");
            this.Property(t => t.HasarMevkiBoylamKodu).HasColumnName("HasarMevkiBoylamKodu");
            this.Property(t => t.HasarTuruNedeni).HasColumnName("HasarTuruNedeni");
            this.Property(t => t.HasarDosyaDurumu).HasColumnName("HasarDosyaDurumu");
            this.Property(t => t.RedNedeni).HasColumnName("RedNedeni");
            this.Property(t => t.DosyayaAtananMTKodu).HasColumnName("DosyayaAtananMTKodu");
            this.Property(t => t.AnlasmasiServisKodu).HasColumnName("AnlasmasiServisKodu");
            this.Property(t => t.KayitTipi).HasColumnName("KayitTipi");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");

            // Relationships
            this.HasRequired(t => t.PoliceGenel)
                .WithMany(t => t.HasarGenelBilgilers)
                .HasForeignKey(d => d.PoliceId);

        }
    }
}
