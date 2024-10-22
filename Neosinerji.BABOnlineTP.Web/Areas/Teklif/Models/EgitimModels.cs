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
    public class EgitimModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public EgitimGenelBilgiler GenelBilgiler { get; set; }
    }

    public class DetayEgitimModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }

        public EgitimGenelBilgiler GenelBilgiler { get; set; }
    }

    public class EgitimDetayPartialModel
    {
        public string AdSoyad { get; set; }
        public string SigortaBaslangicTarihi { get; set; }
        public string SigortaSuresi { get; set; }
        public string SigortaGeriOdemeSuresi { get; set; }
        public string GeriOdemelerdeAlnckYT { get; set; }
        public string OdemeDonemiText { get; set; }
    }

    public class OdemeEgitimModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class EgitimGenelBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "InsuranceSince")]
        public DateTime SigortaBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectInsuranceTime")]
        public int? SigortaSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "EnterInsuranceRepaymentPeriod")]
        public int? SigortaGeriOdemeSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "EnterAnnualAmountReceivedRepayment")]
        public decimal? GeriOdemelerdeAlinacakYillikTutar { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "PremiumPaymentPeriod")]
        public byte? OdemeDonemi { get; set; }

        public SelectList OdemeDonemeleri { get; set; }

        //FOR DETAIL
        public string OdemeDonemiText { get; set; }
    }

    public class EgitimAnaTeminatlar
    {
        public bool Vefat { get; set; }
        public bool SureSonuPrimIadesi { get; set; }

        public int? VefatBedeli { get; set; }
        public int? SureSonuPrimIadesiBedeli { get; set; }
    }
}