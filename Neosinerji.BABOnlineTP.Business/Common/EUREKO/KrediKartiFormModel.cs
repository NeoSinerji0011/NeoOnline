using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EurekoSigorta_Business.Common.EUREKO
{
    public class KrediKartiFormModel
    {
        public string IslemTarihi { get; set; }
        public string KrediKartiTuru { get; set; }
        public string KrediKartiNo { get; set; }
        public string SonKullanmaTarihi { get; set; }
        public string TaksitSayisi { get; set; }
        public string CVV2 { get; set; }
        public string AdiSoyadi { get; set; }
        public string KimlikNo { get; set; }
        public string AcenteNo { get; set; }
        public string UrunAdi { get; set; }
        public string PoliceNo { get; set; }

        public string ToplamTutar { get; set; }
        public string KartSahibininAdiSoyadi { get; set; }
        public string Tarih { get; set; }
        public Odemeler[] OdemePlani { get; set; }
        public string AcenteAdi { get; set; }

    }

    public class Odemeler
    {
        public string Taksitler { get; set; }
        public string Tarihi { get; set; }
        public string Tutari { get; set; }
    }
}
