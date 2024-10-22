using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Neosinerji.BABOnlineTP.Database.Models.Mapping
{
    public class ReasurorGenelMap : EntityTypeConfiguration<ReasurorGenel>
    {
        public ReasurorGenelMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.PdfPoliceCreditNote)
                .HasMaxLength(255);
            this.Property(t => t.PdfPoliceDebitNote)
                .HasMaxLength(255);
            this.Property(t => t.PdfPoliceDosyasi)
                .HasMaxLength(255);
            this.Property(t => t.PdfTeklifDosyasi)
                .HasMaxLength(255);
            this.Property(t => t.YurtdisiPoliceNo)
               .HasMaxLength(255);
            this.Property(t => t.Aciklama)
               .HasMaxLength(255);
            this.Property(t => t.PoliceBaslangicSaat)
                .HasMaxLength(10);
            this.Property(t => t.PoliceBitisSaat)
                .HasMaxLength(10);
            this.Property(t => t.DovizTuru)
                .HasMaxLength(10);

            // Table & Column Mappings
            this.ToTable("ReasurorGenel");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Teklif_ID).HasColumnName("Teklif_ID");
            this.Property(t => t.Police_ID).HasColumnName("Police_ID");
            this.Property(t => t.TVMKodu).HasColumnName("TVMKodu");
            this.Property(t => t.DisKaynakKodu).HasColumnName("DisKaynakKodu");
            this.Property(t => t.SigortaSirketiKodu).HasColumnName("SigortaSirketiKodu");
            this.Property(t => t.SatisKanaliKodu).HasColumnName("SatisKanaliKodu");
            this.Property(t => t.PoliceBaslangicTarihi).HasColumnName("PoliceBaslangicTarihi");
            this.Property(t => t.PoliceBitisTarihi).HasColumnName("PoliceBitisTarihi");
            this.Property(t => t.PoliceBaslangicSaat).HasColumnName("PoliceBaslangicSaat");
            this.Property(t => t.PoliceBitisSaat).HasColumnName("PoliceBitisSaat");
            this.Property(t => t.TeminatTutari).HasColumnName("TeminatTutari");
            this.Property(t => t.TeminatTutariTL).HasColumnName("TeminatTutariTL");
            this.Property(t => t.YurtdisiPrim).HasColumnName("YurtdisiPrim");
            this.Property(t => t.YurtdisiPrimTL).HasColumnName("YurtdisiPrimTL");
            this.Property(t => t.YurtdisiDisKaynakKomisyon).HasColumnName("YurtdisiDisKaynakKomisyon");
            this.Property(t => t.YurtdisiDisKaynakKomisyonTL).HasColumnName("YurtdisiDisKaynakKomisyonTL");
            this.Property(t => t.YurtdisiAlinanKomisyon).HasColumnName("YurtdisiAlinanKomisyon");
            this.Property(t => t.YurtdisiAlinanKomisyonTL).HasColumnName("YurtdisiAlinanKomisyonTL");
            this.Property(t => t.FrontingSigortaSirketiKomisyon).HasColumnName("FrontingSigortaSirketiKomisyon");
            this.Property(t => t.FrontingSigortaSirketiKomisyonTL).HasColumnName("FrontingSigortaSirketiKomisyonTL");
            this.Property(t => t.SatisKanaliKomisyon).HasColumnName("SatisKanaliKomisyon");
            this.Property(t => t.SatisKanaliKomisyonTL).HasColumnName("SatisKanaliKomisyonTL");
            this.Property(t => t.YurticiAlinanKomisyon).HasColumnName("YurticiAlinanKomisyon");
            this.Property(t => t.YurticiAlinanKomisyonTL).HasColumnName("YurticiAlinanKomisyonTL");
            this.Property(t => t.YurtdisiNetPrim).HasColumnName("YurtdisiNetPrim");
            this.Property(t => t.YurtdisiNetPrimTL).HasColumnName("YurtdisiNetPrimTL");
            this.Property(t => t.YurticiNetPrim).HasColumnName("YurticiNetPrim");
            this.Property(t => t.YurticiNetPrimTL).HasColumnName("YurticiNetPrimTL");
            this.Property(t => t.YurticiBrutPrim).HasColumnName("YurticiBrutPrim");
            this.Property(t => t.YurticiBrutPrimTL).HasColumnName("YurticiBrutPrimTL");
            this.Property(t => t.PdfTeklifDosyasi).HasColumnName("PdfTeklifDosyasi");
            this.Property(t => t.PdfPoliceDosyasi).HasColumnName("PdfPoliceDosyasi");
            this.Property(t => t.PdfPoliceDebitNote).HasColumnName("PdfPoliceDebitNote");
            this.Property(t => t.PdfPoliceCreditNote).HasColumnName("PdfPoliceCreditNote");
            this.Property(t => t.DovizKur).HasColumnName("DovizKur");
            this.Property(t => t.DovizTuru).HasColumnName("DovizTuru");
            this.Property(t => t.YurtdisiPoliceNo).HasColumnName("YurtdisiPoliceNo");
            this.Property(t => t.Aciklama).HasColumnName("Aciklama");
            this.Property(t => t.Bsmv).HasColumnName("Bsmv");
            this.Property(t => t.BsmvTL).HasColumnName("BsmvTL");
            this.Property(t => t.BransKodu).HasColumnName("BransKodu");

        }
    }
}
