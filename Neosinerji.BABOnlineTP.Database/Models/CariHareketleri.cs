using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class CariHareketleri
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public string CariHesapKodu { get; set; }
        public int CariHesapId { get; set; }
        public Nullable<DateTime> CariHareketTarihi { get; set; }
        public Nullable<DateTime> OdemeTarihi { get; set; }
        public byte EvrakTipi { get; set; }
        public Nullable<int> OdemeTipi { get; set; }
        public string BorcAlacakTipi { get; set; }      
        public string EvrakNo { get; set; }
        public decimal Tutar { get; set; }
        public Nullable<int> MasrafMerkezi { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string DovizTipi { get; set; }
        public Nullable<decimal> DovizKuru { get; set; }
        public Nullable<decimal> DovizTutari { get; set; }
        public string Aciklama { get; set; }
        public DateTime KayitTarihi { get; set; }
        public Nullable<DateTime> GuncellemeTarihi { get; set; }
        

    }
}
