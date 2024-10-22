using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class Z_Aegon_TeklifGenel
    {
        public int TeklifId { get; set; }
        public int TVMKodu { get; set; }
        public int TeklifNo { get; set; }
        public int TeklifRevizyonNo { get; set; }
        public string TUMPoliceNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public int TUMKodu { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public int UrunKodu { get; set; }
        public byte TeklifDurumKodu { get; set; }
        public int OdemePlaniAlternatifKodu { get; set; }
        public System.DateTime TanzimTarihi { get; set; }
        public System.DateTime BaslamaTarihi { get; set; }
        public System.DateTime BitisTarihi { get; set; }
        public System.DateTime GecerlilikBitisTarihi { get; set; }
        public Nullable<decimal> BrutPrim { get; set; }
        public Nullable<decimal> NetPrim { get; set; }
        public Nullable<decimal> ToplamVergi { get; set; }
        public Nullable<byte> OdemeSekli { get; set; }
        public Nullable<byte> OdemeTipi { get; set; }
        public Nullable<byte> TaksitSayisi { get; set; }
        public Nullable<byte> DovizTL { get; set; }
        public string DovizKodu { get; set; }
        public Nullable<decimal> DovizKurBedeli { get; set; }
        public Nullable<short> TarifeBasamakKodu { get; set; }
        public Nullable<decimal> GecikmeZammiYuzdesi { get; set; }
        public Nullable<decimal> HasarsizlikIndirimYuzdesi { get; set; }
        public Nullable<decimal> PlakaIndirimYuzdesi { get; set; }
        public Nullable<decimal> HasarSurprimYuzdesi { get; set; }
        public Nullable<decimal> ZKYTMSYüzdesi { get; set; }
        public Nullable<decimal> ToplamIndirimTutari { get; set; }
        public Nullable<decimal> ToplamSurprimTutari { get; set; }
        public Nullable<decimal> ToplamKomisyon { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public string PDFDosyasi { get; set; }
        public string PDFPolice { get; set; }
        public string PDFBilgilendirme { get; set; }
        public string PDFGenelSartlari { get; set; }
        public Nullable<bool> Basarili { get; set; }
        public Nullable<int> Otorizasyon { get; set; }
        public Nullable<int> IlgiliTeklifId { get; set; }
        public Nullable<int> IlgiliTeklifNo { get; set; }
        public Nullable<int> IlgiliTeklifUrunKodu { get; set; }
        public string PDFDekont { get; set; }
    }
}
