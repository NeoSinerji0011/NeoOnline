using Neosinerji.BABOnlineTP.Business.turknippon.seyahat;
using Newtonsoft.Json.Linq;
//using Neosinerji.BABOnlineTP.Business.turknippon.seyahat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.Seyahat
{
    public interface ITURKNIPPONSeyahat : ITeklif
    {
        ScopeOutput[] GetScopeList(bool isDomestic);
        AlternativeOutput[] GetAlternativeList(bool isDomestic, int ScopeOrPocket);
        CountryOutput[] GetCountryList(int Alternative);
        TravelOutput Print(JObject insuredJson);
        TravelOutput Compute(JObject insuredJson);
        TravelOutput Approve(JObject insuredJson);
        JObject CreateOfferRecord(JObject insuredsJson);
        JObject CreatePolicyRecord(JObject insuredsJson);
        JObject MergePDFFiles(JObject pdfFilesArray);

    }
    public class PoliceBilgileri
    {
        public string SigortaliAdi { get; set; }
        public string SigortaliSoyadi { get; set; }
        public string AcikAdres { get; set; }
        public string Aciklama { get; set; }
        public string Hata { get; set; }
        public string DigerSirketYenilemesiMi { get; set; }
        public string ReferansPoliceNumarasi { get; set; }
        public string DainiMurteinKrediBitisTar { get; set; }
        public string DainiMurteinBankaKodu { get; set; }
        public string DainiMurteinAdi { get; set; }
        public string DainiMurteinKrediDovizCinsi { get; set; }
        public string DainiMurteinFinansKurumu { get; set; }
        public string DainiMurteinVarMi { get; set; }
        public string DainiMurteinKrediTutari { get; set; }
        public string DainiMurteinSubeKodu { get; set; }
        public string DainiMurteinKrediSozlesmeNo { get; set; }
        public string DainiMurteinAdiUnvani { get; set; }
        public string BagimsizBolumNo { get; set; }
        public string OncekiSirketPolNo { get; set; }
        public string OncekiSirketKodu { get; set; }
        public string OncekiZeylNo { get; set; }
        public string DaireM2 { get; set; }
        public string BinaInsaTarzi { get; set; }
        public string BinaInsaYili { get; set; }
        public string DaireKullanimSekli { get; set; }
        public string SigortaEttirenSifati { get; set; }
        public string HasarliMi { get; set; }

    }

}
