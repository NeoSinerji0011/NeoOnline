using System.Collections.Generic;
namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Models
{
    public class TaliKomisyonOranListeModel
    {
        public TaliKomisyonOranListeModel()
        {
            this.KademeListesi = new List<KademeliOran>();
        }
        public int KomisyonOranId { get; set; }
        public string TaliUnvani { get; set; }
        public string TaliDisKaynakUnvani { get; set; }
        public string BransAdi { get; set; }
        public string SigortaSirketiAdi { get; set; }
        public string BaslangicTarihi { get; set; }
        public decimal Oran { get; set; }
        public int GecerliYil { get; set; }
        public List<KademeliOran> KademeListesi { get; set; }
    }

    public class KademeliOran
    {
        public int Sira { get; set; }
        public decimal MinUretim { get; set; }
        public decimal MaxUretim { get; set; }
        public decimal Oran { get; set; }
        public bool CikarShow { get { return Sira != 0; } }
    }
}