using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDITrafikResponse
    {
        public string ReferansNo { get; set; }
        public string Durum { get; set; }
        public string DurumMesaj { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string HDIMusteriNo { get; set; }
        public string UygulananTaksitSayisi { get; set; }
        public string ToplamKomisyon { get; set; }
        public string MADDIARACBASINA { get; set; }
        public string MADDIKAZABASINA { get; set; }
        public string TEDAVIKISIBASINA { get; set; }
        public string TEDAVIKAZABASINA { get; set; }
        public string TEDAVIDISIKISIBASINA { get; set; }
        public string TEDAVIDISIKAZABASINA { get; set; }
        public string TrafikNetPrimi { get; set; }
        public string Prim { get; set; }
        public string NetPrim { get; set; }
        public string GarantiFonu { get; set; }
        public string THGFonu { get; set; }
        public string IMMPrimi { get; set; }
        public string KoltukPrimi { get; set; }
        public string AsistanPrimi { get; set; }
        public string GiderVergisi { get; set; }
        public string OdenenTutar { get; set; }
        public string QPHSOR { get; set; }
        public string QPHIOR { get; set; }
        public string QPTSOR { get; set; }
        public string XWEP5 { get; set; }
        public string XWEP6 { get; set; }
        public string QPHKDM { get; set; }
        public string QP30CP { get; set; }
        public string OncekiPoliceSirket { get; set; }
        public string OncekiPoliceAcente { get; set; }
        public string OncekiPoliceNo { get; set; }
        public string OncekiPoliceYenilemeNo { get; set; }
        public string SeriNo { get; set; }
        public string PolBasTar { get; set; }
        public string TcKmAd { get; set; }
        public string TcKmSyAd { get; set; }
        public string AcenteLevhaNo { get; set; }
    }
}
