using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Common
{
    [Serializable]
    public class AutoPoliceTransferProcedureModel
    {
        public int TvmKodu { get; set; }
        public string Unvani { get; set; }
        public string SirketKodu { get; set; }
        public string SirketAdi { get; set; }
        public DateTime TanzimBaslangicTarihi { get; set; }
        public DateTime TanzimBitisTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int KaydiEkleyenKullaniciKodu { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string PoliceTransferUrl { get; set; }
    }
}
