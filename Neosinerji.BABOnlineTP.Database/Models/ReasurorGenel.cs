using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class ReasurorGenel
    {
        public int Id { get; set; }
        public int Teklif_ID { get; set; }
        public Nullable<int> Police_ID { get; set; }
        public int TVMKodu { get; set; }
        public Nullable<int> DisKaynakKodu { get; set; }
        public Nullable<int> SigortaSirketiKodu { get; set; }
        public Nullable<int> SatisKanaliKodu { get; set; }
        public Nullable<int> BransKodu { get; set; }
        public Nullable<System.DateTime> PoliceBaslangicTarihi { get; set; }
        public Nullable<System.DateTime> PoliceBitisTarihi { get; set; }
        public string PoliceBaslangicSaat { get; set; }
        public string PoliceBitisSaat { get; set; }
        public Nullable<decimal> TeminatTutari { get; set; }
        public Nullable<decimal> TeminatTutariTL { get; set; }
        public Nullable<decimal> YurtdisiPrim { get; set; }
        public Nullable<decimal> YurtdisiPrimTL { get; set; }
        public Nullable<decimal> YurtdisiDisKaynakKomisyon { get; set; }
        public Nullable<decimal> YurtdisiDisKaynakKomisyonTL { get; set; }
        public Nullable<decimal> YurtdisiAlinanKomisyon { get; set; }
        public Nullable<decimal> YurtdisiAlinanKomisyonTL { get; set; }
        public Nullable<decimal> FrontingSigortaSirketiKomisyon { get; set; }
        public Nullable<decimal> FrontingSigortaSirketiKomisyonTL { get; set; }
        public Nullable<decimal> SatisKanaliKomisyon { get; set; }
        public Nullable<decimal> SatisKanaliKomisyonTL { get; set; }
        public Nullable<decimal> YurticiAlinanKomisyon { get; set; }
        public Nullable<decimal> YurticiAlinanKomisyonTL { get; set; }
        public Nullable<decimal> YurtdisiNetPrim { get; set; }
        public Nullable<decimal> YurtdisiNetPrimTL { get; set; }
        public Nullable<decimal> YurtdisiBrokerNetPrim { get; set; }
        public Nullable<decimal> YurtdisiBrokerNetPrimTL { get; set; }
        public Nullable<decimal> YurticiNetPrim { get; set; }
        public Nullable<decimal> YurticiNetPrimTL { get; set; }
        public Nullable<decimal> YurticiBrutPrim { get; set; }
        public Nullable<decimal> YurticiBrutPrimTL { get; set; }
        public Nullable<decimal> Bsmv { get; set; }
        public Nullable<decimal> BsmvTL { get; set; }
        public string PdfTeklifDosyasi { get; set; }
        public string PdfPoliceDosyasi { get; set; }
        public string PdfPoliceDebitNote { get; set; }    
        public string PdfPoliceCreditNote { get; set; }
        public Nullable<decimal> DovizKur { get; set; }
        public string DovizTuru { get; set; }
        public string YurtdisiPoliceNo { get; set; }
        public string Aciklama { get; set; }
    }
}
