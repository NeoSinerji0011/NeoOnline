
namespace Neosinerji.BABOnlineTP.Business.Komisyon
{
    public class BransModel
    {
        public int Kodu { get; set; }
        public string Adi { get; set; }
    }

    public class SigortaSirketiModel
    {
        public string Kodu { get; set; }
        public string Adi { get; set; }
    }

    public class TVMModel
    {
        public int Kodu { get; set; }
        public string Unvani { get; set; }
    }

    public enum TaliKomisyonHesaplamaPoliceDurumu
    {
        Hesaplanmamis = 0,
        Hesaplanmis = 1,
        Hepsi = 2
    }

    public enum PrimTipleri
    {
        IptalZeyilleri = 0,
        Tahakkuk = 1
    }
    public class KomisyonKademeModel
    {
        public decimal MinUretim { get; set; }
        public decimal MaxUretim { get; set; }
        public decimal Oran { get; set; }
        public bool CikarShow { get; set; }
        public int Sira { get; set; }
    }
}
