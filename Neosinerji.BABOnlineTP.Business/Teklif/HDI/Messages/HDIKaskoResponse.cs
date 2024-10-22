using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    [Serializable]
    [XmlRoot(ElementName = "HDISIGORTA", Namespace = "")]
    public class HDIKaskoResponse
    {
        public string ReferansNo { get; set; }
        public string Durum { get; set; }
        public string DurumMesaj { get; set; }
        public string PERTARAC { get; set; }
        public string HDIPLBSTR { get; set; }
        public string UFY { get; set; }
        public string HamKaskoNetPrimi { get; set; }
        public string KaskoNetPrimiOdemeSekli { get; set; }
        public string ToplamKomisyon { get; set; }
        public string KomisyonIndirimTutari { get; set; }
        public string SonKaskoNetPrimi { get; set; }
        public string IhtiyariMMPrimi { get; set; }
        public string KoltukFerdiKazaPrimi { get; set; }
        public string HukuksalKorumaPrimi { get; set; }
        public string YurticiNakliyeciSorPrimi { get; set; }
        public string SaglikPrimi { get; set; }
        public string Medline { get; set; }
        public string AsistanHizmeti { get; set; }
        public string MiniOnarimHizmeti { get; set; }
        public string PoliceNetPrim { get; set; }
        public string GiderVergisi { get; set; }
        public string OdenecekPrim { get; set; }
        public string HasarKademesi { get; set; }
        public string TramerBelgeNo { get; set; }
        public string TramerBelgeTarih { get; set; }
        public string TramerBelgeKademe { get; set; }
        public string EskiPoliceBitisTarihi { get; set; }
        public string EskiPoliceIptalDurum { get; set; }
        public string EskiPoliceIptalTarihi { get; set; }
        public string PrsIndVar { get; set; }
        public string tcKmAd { get; set; }
        public string tcKmSyAd { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }


        public string OTOROZASYON { get; set; }
        public string PoliceNo { get; set; }
        public string YenilemeNo { get; set; }
        public string BIMReferans { get; set; }
        public string SeriNo { get; set; }


        public List<OdemeVadeTutar> Taksitler { get; set; }
        public List<Teminat> Teminatlar { get; set; }

        public string TaksitVade { get; set; }
        public string TaksitTutar { get; set; }
        public string Kloz { get; set; }
        public string Cizgi { get; set; }
        public string BosSatir { get; set; }
        public string Paragraf { get; set; }
        public string Text { get; set; }
        public string Enter { get; set; }
        public string KoyuText { get; set; }
    }

    public class OdemeVadeTutar
    {
        public string TaksitVade { get; set; }
        public string TaksitTutar { get; set; }
    }

    public class Teminat
    {
        public string TeminatAd { get; set; }
        public string TeminatTutar { get; set; }
    }

}



