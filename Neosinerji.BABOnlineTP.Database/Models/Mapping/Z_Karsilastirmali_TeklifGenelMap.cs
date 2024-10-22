using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class Z_Karsilastirmali_TeklifGenelMap : EntityTypeConfiguration<Z_Karsilastirmali_TeklifGenel>
    {
        public Z_Karsilastirmali_TeklifGenelMap()
        {
            // Primary Key
            this.HasKey(t => t.TeklifId);

            // Properties
            this.Property(t => t.TeklifId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            this.Property(t => t.TUMPoliceNo)
                .HasMaxLength(20);

            this.Property(t => t.TUMTeklifNo)
                .HasMaxLength(20);

            this.Property(t => t.DovizKodu)
                .HasMaxLength(5);

            this.Property(t => t.PDFDosyasi)
                .HasMaxLength(255);

            this.Property(t => t.PDFPolice)
                .HasMaxLength(255);

            this.Property(t => t.PDFBilgilendirme)
                .HasMaxLength(255);

            this.Property(t => t.PDFGenelSartlari)
                .HasMaxLength(255);

            this.Property(t => t.PDFDekont)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Z_Karsilastirmali_TeklifGenel");
            this.Property(t => t.TeklifId).HasColumnName("TeklifId");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.TeklifNo).HasColumnName("TeklifNo");
            this.Property(t => t.TeklifRevizyonNo).HasColumnName("TeklifRevizyonNo");
            this.Property(t => t.TUMPoliceNo).HasColumnName("TUMPoliceNo");
            this.Property(t => t.TUMTeklifNo).HasColumnName("TUMTeklifNo");
            this.Property(t => t.TUMKodu).HasColumnName("TUMKodu");
            this.Property(t => t.TVMKullaniciKodu).HasColumnName("TVMKullaniciKodu");
            this.Property(t => t.UrunKodu).HasColumnName("UrunKodu");
            this.Property(t => t.TeklifDurumKodu).HasColumnName("TeklifDurumKodu");
            this.Property(t => t.OdemePlaniAlternatifKodu).HasColumnName("OdemePlaniAlternatifKodu");
            this.Property(t => t.TanzimTarihi).HasColumnName("TanzimTarihi");
            this.Property(t => t.BaslamaTarihi).HasColumnName("BaslamaTarihi");
            this.Property(t => t.BitisTarihi).HasColumnName("BitisTarihi");
            this.Property(t => t.GecerlilikBitisTarihi).HasColumnName("GecerlilikBitisTarihi");
            this.Property(t => t.BrutPrim).HasColumnName("BrutPrim");
            this.Property(t => t.NetPrim).HasColumnName("NetPrim");
            this.Property(t => t.ToplamVergi).HasColumnName("ToplamVergi");
            this.Property(t => t.OdemeSekli).HasColumnName("OdemeSekli");
            this.Property(t => t.OdemeTipi).HasColumnName("OdemeTipi");
            this.Property(t => t.TaksitSayisi).HasColumnName("TaksitSayisi");
            this.Property(t => t.DovizTL).HasColumnName("DovizTL");
            this.Property(t => t.DovizKodu).HasColumnName("DovizKodu");
            this.Property(t => t.DovizKurBedeli).HasColumnName("DovizKurBedeli");
            this.Property(t => t.TarifeBasamakKodu).HasColumnName("TarifeBasamakKodu");
            this.Property(t => t.GecikmeZammiYuzdesi).HasColumnName("GecikmeZammiYuzdesi");
            this.Property(t => t.HasarsizlikIndirimYuzdesi).HasColumnName("HasarsizlikIndirimYuzdesi");
            this.Property(t => t.PlakaIndirimYuzdesi).HasColumnName("PlakaIndirimYuzdesi");
            this.Property(t => t.HasarSurprimYuzdesi).HasColumnName("HasarSurprimYuzdesi");
            this.Property(t => t.ZKYTMSYüzdesi).HasColumnName("ZKYTMSYüzdesi");
            this.Property(t => t.ToplamIndirimTutari).HasColumnName("ToplamIndirimTutari");
            this.Property(t => t.ToplamSurprimTutari).HasColumnName("ToplamSurprimTutari");
            this.Property(t => t.ToplamKomisyon).HasColumnName("ToplamKomisyon");
            this.Property(t => t.KayitTarihi).HasColumnName("KayitTarihi");
            this.Property(t => t.PDFDosyasi).HasColumnName("PDFDosyasi");
            this.Property(t => t.PDFPolice).HasColumnName("PDFPolice");
            this.Property(t => t.PDFBilgilendirme).HasColumnName("PDFBilgilendirme");
            this.Property(t => t.PDFGenelSartlari).HasColumnName("PDFGenelSartlari");
            this.Property(t => t.Basarili).HasColumnName("Basarili");
            this.Property(t => t.Otorizasyon).HasColumnName("Otorizasyon");
            this.Property(t => t.IlgiliTeklifId).HasColumnName("IlgiliTeklifId");
            this.Property(t => t.IlgiliTeklifNo).HasColumnName("IlgiliTeklifNo");
            this.Property(t => t.IlgiliTeklifUrunKodu).HasColumnName("IlgiliTeklifUrunKodu");
            this.Property(t => t.PDFDekont).HasColumnName("PDFDekont");
        }
    }
}
