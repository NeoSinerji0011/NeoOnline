using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TVMDetayMap : EntityTypeConfiguration<TVMDetay>
    {
        public TVMDetayMap()
        {
            // Primary Key
            this.HasKey(t => t.Kodu);

            // Properties
            this.Property(t => t.Kodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.Unvani)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.KayitNo)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.VergiDairesi)
                .IsRequired()
                .HasMaxLength(30);

            this.Property(t => t.VergiNumarasi)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.TCKN)
                .HasMaxLength(20);

            this.Property(t => t.Telefon)
                .HasMaxLength(20);

            this.Property(t => t.Fax)
                .HasMaxLength(20);

            this.Property(t => t.Email)
                .HasMaxLength(100);

            this.Property(t => t.WebAdresi)
                .HasMaxLength(100);

            this.Property(t => t.Banner)
                .HasMaxLength(300);

            this.Property(t => t.Logo)
                .HasMaxLength(300);

            this.Property(t => t.UlkeKodu)
                .HasMaxLength(3);

            this.Property(t => t.IlKodu)
                .HasMaxLength(5);

            this.Property(t => t.Semt)
                .HasMaxLength(50);

            this.Property(t => t.Adres)
                .HasMaxLength(200);

            this.Property(t => t.Notlar)
                .HasMaxLength(150);

            this.Property(t => t.Latitude)
                .HasMaxLength(15);

            this.Property(t => t.Longitude)
                .HasMaxLength(15);

            this.Property(t => t.ProjeKodu)
                .HasMaxLength(20);

            this.Property(t => t.IpmiMacmi)
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("TVMDetay");
            this.Property(t => t.Kodu).HasColumnName("Kodu");
            this.Property(t => t.Unvani).HasColumnName("Unvani");
            this.Property(t => t.Tipi).HasColumnName("Tipi");
            this.Property(t => t.KayitNo).HasColumnName("KayitNo");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.VergiNumarasi).HasColumnName("VergiNumarasi");
            this.Property(t => t.TCKN).HasColumnName("TCKN");
            this.Property(t => t.Profili).HasColumnName("Profili");
            this.Property(t => t.BagliOlduguTVMKodu).HasColumnName("BagliOlduguTVMKodu");
            this.Property(t => t.BolgeYetkilisiMi).HasColumnName("BolgeYetkilisiMi");
            this.Property(t => t.PoliceTransfer).HasColumnName("PoliceTransfer");
            this.Property(t => t.AcentSuvbeVar).HasColumnName("AcentSuvbeVar");
            this.Property(t => t.Durum).HasColumnName("Durum");
            this.Property(t => t.DurumGuncallemeTarihi).HasColumnName("DurumGuncallemeTarihi");
            this.Property(t => t.SozlesmeBaslamaTarihi).HasColumnName("SozlesmeBaslamaTarihi");
            this.Property(t => t.SozlesmeDondurmaTarihi).HasColumnName("SozlesmeDondurmaTarihi");
            this.Property(t => t.Telefon).HasColumnName("Telefon");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.WebAdresi).HasColumnName("WebAdresi");
            this.Property(t => t.Banner).HasColumnName("Banner");
            this.Property(t => t.Logo).HasColumnName("Logo");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.Semt).HasColumnName("Semt");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.Notlar).HasColumnName("Notlar");
            this.Property(t => t.BolgeKodu).HasColumnName("BolgeKodu");
            this.Property(t => t.SifreKontralSayisi).HasColumnName("SifreKontralSayisi");
            this.Property(t => t.SifreDegistirmeGunu).HasColumnName("SifreDegistirmeGunu");
            this.Property(t => t.SifreIkazGunu).HasColumnName("SifreIkazGunu");
            this.Property(t => t.GrupKodu).HasColumnName("GrupKodu");
            this.Property(t => t.BaglantiSiniri).HasColumnName("BaglantiSiniri");
            this.Property(t => t.UcretlendirmeKodu).HasColumnName("UcretlendirmeKodu");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.MuhasebeEntegrasyon).HasColumnName("MuhasebeEntegrasyon");
            this.Property(t => t.ProjeKodu).HasColumnName("ProjeKodu");
            this.Property(t => t.MobilDogrulama).HasColumnName("MobilDogrulama");
            this.Property(t => t.IpmiMacmi).HasColumnName("IpmiMacmi");
            this.Property(t => t.SonPoliceOnayTarihi).HasColumnName("SonPoliceOnayTarihi");
        }
    }
}
