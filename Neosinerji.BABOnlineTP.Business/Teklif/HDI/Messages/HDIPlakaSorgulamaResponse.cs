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
    public class HDIPlakaSorgulamaResponse : HDIMessage
    {
        public string ReferansNo { get; set; }
        public string Durum { get; set; }
        public string DurumMesaj { get; set; }

        public HDIPlakaSorgulamaResponseDetails HDISIGORTA { get; set; }
    }

    [Serializable]
    public class HDIPlakaSorgulamaResponseDetails
    {
        public string Durum { get; set; }
        public string Mesaj { get; set; }
        public string EskiPoliceSigortaSirkedKodu { get; set; }
        public string EskiPoliceAcenteKod { get; set; }
        public string EskiPoliceNo { get; set; }
        public string EskiPoliceYenilemeNo { get; set; }
        public string TanzimTarih { get; set; }
        public string PoliceBaslangicTarih { get; set; }
        public string PoliceBitisTarih { get; set; }
        public string EkBaslangicTarih { get; set; }
        public string AracTescilTarih { get; set; }
        public string TramerTarifeGrup { get; set; }
        public string AracTarifeGrupKodu { get; set; }
        public string AracMarkaKodu { get; set; }
        public string AracTipKodu { get; set; }
        public string AracMarka { get; set; }
        public string AracTipModel { get; set; }
        public string AracModelYili { get; set; }
        public string AracMotorNo { get; set; }
        public string AracSasiNo { get; set; }
        public string AracKullanimSekli { get; set; }
        public string AracKoltukSayisi { get; set; }
        public string AracUygulananKademe { get; set; }
        public string SigortaliAdUnvan { get; set; }
        public string SigortaliSoyAd { get; set; }
        public string TramerBelgeNo { get; set; }
        public string TramerBelgeTarih { get; set; }
        public string ZeylTuru { get; set; }
        public string AracRenk { get; set; }
        public string AracSilindir { get; set; }
        public string AracMotorGucu { get; set; }
        public string AracImalatYeri { get; set; }
        public string IndirimYuzde { get; set; }
        public string SuprimYuzde { get; set; }
        public string IlPlakaIndirimYuzde { get; set; }
        public string TasiyiciIndirimYuzde { get; set; }
        public string TasiyiciSigSirkerKod { get; set; }
        public string TasiyiciSigAcenteNo { get; set; }
        public string TasiyiciSigPoliceNo { get; set; }
        public string TasiyiciSigYenilemeNoField;
    }
}
