using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class PoliceTahsilat
    {
        public int TahsilatId { get; set; }
        public int PoliceId { get; set; }
        public string KimlikNo { get; set; }
        public string PoliceNo { get; set; }
        public string CariHesapKodu { get; set; }
        public string ZeyilNo { get; set; }
        public decimal BrutPrim { get; set; }
        public int OdemTipi { get; set; }
        public int TaksitNo { get; set; }
        public Nullable<int> YenilemeNo { get; set; }
        public System.DateTime TaksitVadeTarihi { get; set; }
        public decimal TaksitTutari { get; set; }
        public decimal OdenenTutar { get; set; }
        public string OdemeBelgeNo { get; set; }
        public string Dekont_EvrakNo { get; set; }
        public Nullable<byte> OtomatikTahsilatiKkMi { get; set; }
        public Nullable<System.DateTime> OdemeBelgeTarihi { get; set; }
        public System.DateTime KayitTarihi { get; set; }
        public Nullable<System.DateTime> GuncellemeTarihi { get; set; }
        public int KaydiEkleyenKullaniciKodu { get; set; }
        public Nullable<decimal> KalanTaksitTutari { get; set; }
        public virtual PoliceGenel PoliceGenel { get; set; }
    }
}
