using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.ComponentModel.DataAnnotations;
using Neosinerji.BABOnlineTP.Business.HDI;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class IkinciElGarantiModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public AracBilgiModel Arac { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public IkinciElGarantiTeklifOdemeModel Odeme { get; set; }

        public IkinciElGarantiGenelBilgiler GenelBilgiler { get; set; }
    }

    public class DetayIkinciElGarantiModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public int TeklifPoliceId { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public DetayAracBilgiModel Arac { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public IkinciElGarantiPoliceOdemeModel OdemeBilgileri { get; set; }

        public IkinciElGarantiGenelBilgiler GenelBilgiler { get; set; }
    }

    public class OdemeIkinciElGarantiModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class IkinciElGarantiTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class IkinciElGarantiPoliceOdemeModel
    {
        public int teklifId { get; set; }

        public decimal NetPrim { get; set; }
        public decimal BrutPrim { get; set; }
        public decimal Vergi { get; set; }
        public int TaksitSayisi { get; set; }

        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMLogoURL { get; set; }

        public string PoliceURL { get; set; }
        public string DokumanURL { get; set; }
        public string TUMPoliceNo { get; set; }
    }

    public class IkinciElGarantiGenelBilgiler
    {
        //public string PlakaKodu { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        //public string PlakaNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Model { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte TeminatTuru { get; set; }
        public string TeminatTuruText { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte PoliceSuresi { get; set; }
        public string PoliceSuresiText { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte SilindirHacmi { get; set; }
        public string SilindirHacmiText { get; set; }


        //public SelectList PlakaKoduListe { get; set; }

        public List<SelectListItem> TeminatTurleri { get; set; }
        public List<SelectListItem> PoliceSureleri { get; set; }
        public List<SelectListItem> SilindirHacimleri { get; set; }
        public List<SelectListItem> Modeller { get; set; }
    }


    public class PlakaSorguIkinciElModel
    {
        public PlakaSorguIkinciElModel(HDIPlakaSorgulamaResponseDetails detail)
        {
            this.AracModelYili = detail.AracModelYili;
            this.AracSilindir = detail.AracSilindir;
        }
        public string AracModelYili { get; set; }
        public string AracSilindir { get; set; }
    }

    public class IkinciElGarantiDokumanEkleModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int teklifId { get; set; }
        public string DokumanAdi { get; set; }
    }
}