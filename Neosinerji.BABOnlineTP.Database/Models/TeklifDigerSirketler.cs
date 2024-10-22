using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifDigerSirketler
    {
        public int Id { get; set; }
        public int TeklifId { get; set; }
        public int SigortaSirketKodu { get; set; }
        public string SigortaSirketTeklifNo { get; set; }
        public string HasarsizlikIndirim { get; set; }
        public string HasarsizlikSurprim { get; set; }
        public decimal BrutPrim { get; set; }
        public Nullable<int> TaksitSayisi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public int KayitEdenKullaniciKodu { get; set; }
        public decimal KomisyonTutari { get; set; }
        //public string TeklifPDF { get; set; }
        public virtual TeklifGenel TeklifGenel { get; set; }
    }
}
