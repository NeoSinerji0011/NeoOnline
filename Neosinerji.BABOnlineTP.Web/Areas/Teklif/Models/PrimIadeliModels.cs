using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.ComponentModel.DataAnnotations;


namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class PrimIadeliModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public PrimIadeliTeklifOdemeModel Odeme { get; set; }

        public PrimIadeliGenelBilgiler GenelBilgiler { get; set; }
    }

    public class PrimIadeliDetayPartialModel
    {
        public string AdSoyad { get; set; }
        public string SigortaBaslangicTarihi { get; set; }

        public string PrimOdemeDonemiText { get; set; }
        public string SigortaSuresiText { get; set; }
        public string HesaplamaSecenegiText { get; set; }
        public string Tutar { get; set; }
    }

    public class DetayPrimIadeliModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }

        public PrimIadeliGenelBilgiler GenelBilgiler { get; set; }
    }

    public class OdemePrimIadeliModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class PrimIadeliTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class PrimIadeliGenelBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "InsuranceSince")]
        public DateTime SigortaBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectInsuranceTime")]
        public byte? SigortaSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "PremiumPaymentPeriod")]
        public byte? PrimOdemeDonemi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectCalculationMethod")]
        public byte? HesaplamaSecenegi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? Ortak_PrimTutari { get; set; }


        public SelectList ParaBirimleri { get; set; }
        public SelectList PrimDonemleri { get; set; }
        public SelectList SigortaSureleri { get; set; }
        public SelectList HesaplamaSecenekleri { get; set; }

        //FOR DETAIL
        public string PrimOdemeDonemiText { get; set; }
        public string SigortaSuresiText { get; set; }
        public string HesaplamaSecenegiText { get; set; }
    }
}