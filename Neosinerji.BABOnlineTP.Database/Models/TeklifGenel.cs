using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class TeklifGenel
    {
        public TeklifGenel()
        {
            this.TeklifAracs = new List<TeklifArac>();
            this.TeklifAracEkSorus = new List<TeklifAracEkSoru>();
            this.TeklifDigerSirketlers = new List<TeklifDigerSirketler>();
            this.TeklifOdemePlanis = new List<TeklifOdemePlani>();
            this.TeklifProvizyons = new List<TeklifProvizyon>();
            this.TeklifRizikoAdresis = new List<TeklifRizikoAdresi>();
            this.TeklifSigortaEttirens = new List<TeklifSigortaEttiren>();
            this.TeklifSigortalis = new List<TeklifSigortali>();
            this.TeklifSorus = new List<TeklifSoru>();
            this.TeklifTeminats = new List<TeklifTeminat>();
            this.TeklifVergis = new List<TeklifVergi>();
            this.TeklifWebServisCevaps = new List<TeklifWebServisCevap>();
            this.WEBServisLogs = new List<WEBServisLog>();
            this.TeklifDokumans = new List<TeklifDokuman>();
        }

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
        public string PDFDekont { get; set; }
        public Nullable<bool> Basarili { get; set; }
        public Nullable<bool> iptal { get; set; }
        public Nullable<int> Otorizasyon { get; set; }
        public Nullable<int> IlgiliTeklifId { get; set; }
        public Nullable<int> IlgiliTeklifNo { get; set; }
        public Nullable<int> IlgiliTeklifUrunKodu { get; set; }
        public Nullable<int> KaydiEKleyenTVMKodu { get; set; }
        public Nullable<int> KaydiEKleyenTVMKullaniciKodu { get; set; }
        public virtual ICollection<TeklifArac> TeklifAracs { get; set; }
        public virtual ICollection<TeklifAracEkSoru> TeklifAracEkSorus { get; set; }
        public virtual ICollection<TeklifDigerSirketler> TeklifDigerSirketlers { get; set; }
        public virtual TVMDetay TVMDetay { get; set; }
        public virtual TVMKullanicilar TVMKullanicilar { get; set; }
        public virtual TeklifNot TeklifNot { get; set; }
        public virtual ICollection<TeklifOdemePlani> TeklifOdemePlanis { get; set; }
        public virtual ICollection<TeklifProvizyon> TeklifProvizyons { get; set; }
        public virtual ICollection<TeklifRizikoAdresi> TeklifRizikoAdresis { get; set; }
        public virtual ICollection<TeklifSigortaEttiren> TeklifSigortaEttirens { get; set; }
        public virtual ICollection<TeklifSigortali> TeklifSigortalis { get; set; }
        public virtual ICollection<TeklifSoru> TeklifSorus { get; set; }
        public virtual ICollection<TeklifTeminat> TeklifTeminats { get; set; }
        public virtual ICollection<TeklifVergi> TeklifVergis { get; set; }
        public virtual ICollection<TeklifWebServisCevap> TeklifWebServisCevaps { get; set; }
        public virtual ICollection<WEBServisLog> WEBServisLogs { get; set; }
        public virtual ICollection<TeklifDokuman> TeklifDokumans{ get; set; }
    }
}
