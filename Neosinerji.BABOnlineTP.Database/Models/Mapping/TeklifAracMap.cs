using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class TeklifAracMap : EntityTypeConfiguration<TeklifArac>
    {
        public TeklifAracMap()
        {
            // Primary Key
            this.HasKey(t => t.TeklifAracId);

            // Properties
            this.Property(t => t.PlakaKodu)
                .IsRequired()
                .HasMaxLength(3);

            this.Property(t => t.PlakaNo)
                .IsRequired()
                .HasMaxLength(25);

            this.Property(t => t.Marka)
                .HasMaxLength(25);

            this.Property(t => t.AracinTipi)
                .HasMaxLength(25);

            this.Property(t => t.MotorNo)
                .HasMaxLength(50);

            this.Property(t => t.SasiNo)
                .HasMaxLength(50);

            this.Property(t => t.Cinsi)
                .HasMaxLength(30);

            this.Property(t => t.TescilSeriKod)
                .HasMaxLength(50);

            this.Property(t => t.TescilSeriNo)
                .HasMaxLength(50);

            this.Property(t => t.AsbisNo)
                .HasMaxLength(50);

            this.Property(t => t.YakitCinsi)
                .HasMaxLength(30);

            this.Property(t => t.Renk)
                .HasMaxLength(30);

            this.Property(t => t.KullanimSekli)
                .HasMaxLength(50);

            this.Property(t => t.KullanimTarzi)
                .HasMaxLength(50);

            this.Property(t => t.AracId)
                .HasMaxLength(50);

            this.Property(t => t.SilindirHacmi)
                .HasMaxLength(10);

            this.Property(t => t.MotorGucu)
                .HasMaxLength(20);

            this.Property(t => t.TescilIlKodu)
                .HasMaxLength(5);

            this.Property(t => t.TescilIlceKodu)
                .HasMaxLength(10);

            this.Property(t => t.TramerBelgeNo)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("TeklifArac");
            this.Property(t => t.TeklifAracId).HasColumnName("TeklifAracId");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.SiraNo).HasColumnName("SiraNo");
            this.Property(t => t.PlakaKodu).HasColumnName("PlakaKodu");
            this.Property(t => t.PlakaNo).HasColumnName("PlakaNo");
            this.Property(t => t.Marka).HasColumnName("Marka");
            this.Property(t => t.AracinTipi).HasColumnName("AracinTipi");
            this.Property(t => t.Model).HasColumnName("Model");
            this.Property(t => t.MotorNo).HasColumnName("MotorNo");
            this.Property(t => t.SasiNo).HasColumnName("SasiNo");
            this.Property(t => t.Cinsi).HasColumnName("Cinsi");
            this.Property(t => t.TescilSeriKod).HasColumnName("TescilSeriKod");
            this.Property(t => t.TescilSeriNo).HasColumnName("TescilSeriNo");
            this.Property(t => t.AsbisNo).HasColumnName("AsbisNo");
            this.Property(t => t.YakitCinsi).HasColumnName("YakitCinsi");
            this.Property(t => t.Renk).HasColumnName("Renk");
            this.Property(t => t.KullanimSekli).HasColumnName("KullanimSekli");
            this.Property(t => t.KullanimTarzi).HasColumnName("KullanimTarzi");
            this.Property(t => t.AracId).HasColumnName("AracId");
            this.Property(t => t.SilindirHacmi).HasColumnName("SilindirHacmi");
            this.Property(t => t.MotorGucu).HasColumnName("MotorGucu");
            this.Property(t => t.KoltukSayisi).HasColumnName("KoltukSayisi");
            this.Property(t => t.YukKapasitesiKg).HasColumnName("YukKapasitesiKg");
            this.Property(t => t.ImalatYeri).HasColumnName("ImalatYeri");
            this.Property(t => t.TrafikCikisTarihi).HasColumnName("TrafikCikisTarihi");
            this.Property(t => t.TrafikTescilTarihi).HasColumnName("TrafikTescilTarihi");
            this.Property(t => t.Hurda).HasColumnName("Hurda");
            this.Property(t => t.TrafiktenCekilmis).HasColumnName("TrafiktenCekilmis");
            this.Property(t => t.PlakaYeniKayit).HasColumnName("PlakaYeniKayit");
            this.Property(t => t.TescilIlKodu).HasColumnName("TescilIlKodu");
            this.Property(t => t.TescilIlceKodu).HasColumnName("TescilIlceKodu");
            this.Property(t => t.TramerBelgeNo).HasColumnName("TramerBelgeNo");
            this.Property(t => t.TramerBelgeTarihi).HasColumnName("TramerBelgeTarihi");
            this.Property(t => t.TarifeBasamagi).HasColumnName("TarifeBasamagi");
            this.Property(t => t.TarifeGecikmeZammi).HasColumnName("TarifeGecikmeZammi");
            this.Property(t => t.GarajTipKodu).HasColumnName("GarajTipKodu");
            this.Property(t => t.AracDeger).HasColumnName("AracDeger");

            // Relationships
            this.HasRequired(t => t.TeklifGenel)
                .WithMany(t => t.TeklifAracs)
                .HasForeignKey(d => d.TeklifId);

        }
    }
}
