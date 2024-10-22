using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_Aegon_MusteriGenelBilgilerMap : EntityTypeConfiguration<Z_Aegon_MusteriGenelBilgiler>
    {
        public Z_Aegon_MusteriGenelBilgilerMap()
        {
            // Primary Key
            this.HasKey(t => t.MusteriKodu);

            // Properties
            this.Property(t => t.MusteriKodu)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TVMMusteriKodu)
                .HasMaxLength(20);

            this.Property(t => t.KimlikNo)
                .IsRequired()
                .HasMaxLength(11);

            this.Property(t => t.PasaportNo)
                .HasMaxLength(40);

            this.Property(t => t.VergiDairesi)
                .HasMaxLength(50);

            this.Property(t => t.AdiUnvan)
                .IsRequired()
                .HasMaxLength(150);

            this.Property(t => t.SoyadiUnvan)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Cinsiyet)
                .HasMaxLength(1);

            this.Property(t => t.EMail)
                .HasMaxLength(50);

            this.Property(t => t.WebUrl)
                .HasMaxLength(50);

            this.Property(t => t.FaaliyetOlcegi_)
                .HasMaxLength(50);

            this.Property(t => t.FaaliyetGosterdigiAnaSektor)
                .HasMaxLength(50);

            this.Property(t => t.FaaliyetGosterdigiAltSektor)
                .HasMaxLength(50);

            this.Property(t => t.SabitVarlikBilgisi)
                .HasMaxLength(50);

            this.Property(t => t.CiroBilgisi)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Z_Aegon_MusteriGenelBilgiler");
            this.Property(t => t.MusteriKodu).HasColumnName("MusteriKodu");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TVMKullaniciKodu).HasColumnName("TVMKullaniciKodu");
            this.Property(t => t.TVMMusteriKodu).HasColumnName("TVMMusteriKodu");
            this.Property(t => t.MusteriTipKodu).HasColumnName("MusteriTipKodu");
            this.Property(t => t.KimlikNo).HasColumnName("KimlikNo");
            this.Property(t => t.PasaportNo).HasColumnName("PasaportNo");
            this.Property(t => t.PasaportGecerlilikBitisTarihi).HasColumnName("PasaportGecerlilikBitisTarihi");
            this.Property(t => t.VergiDairesi).HasColumnName("VergiDairesi");
            this.Property(t => t.AdiUnvan).HasColumnName("AdiUnvan");
            this.Property(t => t.SoyadiUnvan).HasColumnName("SoyadiUnvan");
            this.Property(t => t.Cinsiyet).HasColumnName("Cinsiyet");
            this.Property(t => t.DogumTarihi).HasColumnName("DogumTarihi");
            this.Property(t => t.EMail).HasColumnName("EMail");
            this.Property(t => t.WebUrl).HasColumnName("WebUrl");
            this.Property(t => t.Uyruk).HasColumnName("Uyruk");
            this.Property(t => t.EgitimDurumu).HasColumnName("EgitimDurumu");
            this.Property(t => t.MeslekKodu).HasColumnName("MeslekKodu");
            this.Property(t => t.MedeniDurumu).HasColumnName("MedeniDurumu");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.FaaliyetOlcegi_).HasColumnName("FaaliyetOlcegi ");
            this.Property(t => t.FaaliyetGosterdigiAnaSektor).HasColumnName("FaaliyetGosterdigiAnaSektor");
            this.Property(t => t.FaaliyetGosterdigiAltSektor).HasColumnName("FaaliyetGosterdigiAltSektor");
            this.Property(t => t.SabitVarlikBilgisi).HasColumnName("SabitVarlikBilgisi");
            this.Property(t => t.CiroBilgisi).HasColumnName("CiroBilgisi");
        }
    }
}
