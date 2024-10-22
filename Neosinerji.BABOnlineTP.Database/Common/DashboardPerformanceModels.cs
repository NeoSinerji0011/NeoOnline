using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public class DashboardChartModel
    {
        public int? ToplamMusteri { get; set; }
        public int? ToplamTeklif { get; set; }
        public int? ToplamPolice { get; set; }
        public decimal? PolicelestirmeOranim { get; set; }

        // === Charts === //

        #region Trafik

        public int? tekliftrafikadet { get; set; }
        public int? policetrafikadet { get; set; }
        public decimal? policetrafiktutar { get; set; }
        public decimal? policetrafikkomisyon { get; set; }

        #endregion

        #region Kasko

        public int? teklifkaskoadet { get; set; }
        public int? policekaskoadet { get; set; }
        public decimal? policekaskotutar { get; set; }
        public decimal? policekaskokomisyon { get; set; }

        #endregion

        #region Dask

        public int? teklifdaskadet { get; set; }
        public int? policedaskadet { get; set; }
        public decimal? policedasktutar { get; set; }
        public decimal? policedaskkomisyon { get; set; }

        #endregion

        #region Kredili Hayat

        public int? teklifkredilihayatadet { get; set; }
        public int? policekredilihayatadet { get; set; }
        public decimal? policekredilihayattutar { get; set; }
        public decimal? policekredilihayatkomisyon { get; set; }

        #endregion
    }

    [Serializable]
    public class Performans
    {
        public string ToplamMusteri { get; set; }
        public string ToplamTeklif { get; set; }
        public string ToplamPolice { get; set; }
        public string PolicelesmeOrani { get; set; }

        public List<URN> TeklifList { get; set; }
        public List<URN> PoliceList { get; set; }
    }

    public class URN
    {
        public int UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public string UrunAdiBASE { get; set; }
        public int Adet { get; set; }

        //For Policy
        public decimal BrutPrim { get; set; }
        public string ToplamKomisyon { get; set; }
    }
}

/* <UrunKodu>10</UrunKodu>
      <Adet>1</Adet>
      <BrutPrim>156.00</BrutPrim>
      <ToplamKomisyon>0.00</ToplamKomisyon>*/
