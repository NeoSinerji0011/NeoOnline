using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class PoliceSigortaliMap : EntityTypeConfiguration<PoliceSigortali>
    {
        public PoliceSigortaliMap()
        {
            // Primary Key
            this.HasKey(t => t.PoliceId);

            // Properties
            this.Property(t => t.PoliceId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.KimlikNo)
                .HasMaxLength(11);

            this.Property(t => t.VergiKimlikNo)
                .HasMaxLength(10);

            this.Property(t => t.AdiUnvan)
                .HasMaxLength(150);

            this.Property(t => t.SoyadiUnvan)
                .HasMaxLength(50);

            this.Property(t => t.Cinsiyet)
                .HasMaxLength(10);

            this.Property(t => t.MobilTelefonNo)
                .HasMaxLength(20);

            this.Property(t => t.TelefonNo)
                .HasMaxLength(20);

            this.Property(t => t.UlkeKodu)
                .HasMaxLength(10);

            this.Property(t => t.UlkeAdi)
                .HasMaxLength(250);

            this.Property(t => t.IlKodu)
                .HasMaxLength(10);

            this.Property(t => t.IlAdi)
                .HasMaxLength(100);

            this.Property(t => t.IlceAdi)
                .HasMaxLength(100);

            this.Property(t => t.Semt)
                .HasMaxLength(50);

            this.Property(t => t.Mahalle)
                .HasMaxLength(100);

            this.Property(t => t.Cadde)
                .HasMaxLength(100);

            this.Property(t => t.Sokak)
                .HasMaxLength(100);

            this.Property(t => t.Apartman)
                .HasMaxLength(50);

            this.Property(t => t.BinaNo)
                .HasMaxLength(20);

            this.Property(t => t.DaireNo)
                .HasMaxLength(20);

            this.Property(t => t.HanAptFab)
                .HasMaxLength(50);

            this.Property(t => t.Adres)
                .HasMaxLength(500);

            this.Property(t => t.MusteriTemsilciKodu)
                .HasMaxLength(20);

            this.Property(t => t.TahsilatSorumlusuKodu)
                .HasMaxLength(20);

            this.Property(t => t.MusteriGrupKodu)
                .HasMaxLength(20);

            this.Property(t => t.MuhasebeEntegrasyonHesapNo)
                .HasMaxLength(25);

            this.Property(t => t.Latitude)
                .HasMaxLength(20);

            this.Property(t => t.Longitude)
                .HasMaxLength(20);

            this.Property(t => t.EMail)
               .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("PoliceSigortali");
            this.Property(t => t.PoliceId).HasColumnName("PoliceId");
            this.Property(t => t.KimlikNo).HasColumnName("KimlikNo");
            this.Property(t => t.VergiKimlikNo).HasColumnName("VergiKimlikNo");
            this.Property(t => t.AdiUnvan).HasColumnName("AdiUnvan");
            this.Property(t => t.SoyadiUnvan).HasColumnName("SoyadiUnvan");
            this.Property(t => t.TipKodu).HasColumnName("TipKodu");
            this.Property(t => t.Cinsiyet).HasColumnName("Cinsiyet");
            this.Property(t => t.DogumTarihi).HasColumnName("DogumTarihi");
            this.Property(t => t.MobilTelefonNo).HasColumnName("MobilTelefonNo");
            this.Property(t => t.TelefonNo).HasColumnName("TelefonNo");
            this.Property(t => t.UlkeKodu).HasColumnName("UlkeKodu");
            this.Property(t => t.UlkeAdi).HasColumnName("UlkeAdi");
            this.Property(t => t.IlKodu).HasColumnName("IlKodu");
            this.Property(t => t.IlAdi).HasColumnName("IlAdi");
            this.Property(t => t.IlceKodu).HasColumnName("IlceKodu");
            this.Property(t => t.IlceAdi).HasColumnName("IlceAdi");
            this.Property(t => t.Semt).HasColumnName("Semt");
            this.Property(t => t.Mahalle).HasColumnName("Mahalle");
            this.Property(t => t.Cadde).HasColumnName("Cadde");
            this.Property(t => t.Sokak).HasColumnName("Sokak");
            this.Property(t => t.Apartman).HasColumnName("Apartman");
            this.Property(t => t.BinaNo).HasColumnName("BinaNo");
            this.Property(t => t.DaireNo).HasColumnName("DaireNo");
            this.Property(t => t.HanAptFab).HasColumnName("HanAptFab");
            this.Property(t => t.PostaKodu).HasColumnName("PostaKodu");
            this.Property(t => t.Adres).HasColumnName("Adres");
            this.Property(t => t.MusteriTemsilciKodu).HasColumnName("MusteriTemsilciKodu");
            this.Property(t => t.TahsilatSorumlusuKodu).HasColumnName("TahsilatSorumlusuKodu");
            this.Property(t => t.MusteriGrupKodu).HasColumnName("MusteriGrupKodu");
            this.Property(t => t.MuhasebeEntegrasyonHesapNo).HasColumnName("MuhasebeEntegrasyonHesapNo");
            this.Property(t => t.DainiMurtehin).HasColumnName("DainiMurtehin");
            this.Property(t => t.Latitude).HasColumnName("Latitude");
            this.Property(t => t.Longitude).HasColumnName("Longitude");
            this.Property(t => t.EMail).HasColumnName("EMail");

            // Relationships
            this.HasRequired(t => t.PoliceGenel)
                .WithOptional(t => t.PoliceSigortali);

        }
    }
}
