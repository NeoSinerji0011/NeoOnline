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
    public class OdulluBirikimModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public OdulluBirikimGenelBilgiler GenelBilgiler { get; set; }
    }

    public class OdulluBirikimDetayPartialModel
    {
        public string AdSoyad { get; set; }
        public string SigortaBaslangicTarihi { get; set; }

        public string PrimOdemeDonemiText { get; set; }
        public string SigortaSuresiText { get; set; }
        public string SuprimOrani { get; set; }

        public string HesaplamaSecenegi { get; set; }
        public string Tutar { get; set; }
    }

    public class DetayOdulluBirikimModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }

        public OdulluBirikimGenelBilgiler GenelBilgiler { get; set; }
    }

    public class OdemeOdulluBirikimModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class OdulluBirikimGenelBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "InsuranceSince")]
        public DateTime SigortaBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectInsuranceTime")]
        public int? SigortaSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "PremiumPaymentPeriod")]
        public byte? PrimOdemeDonemi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectCalculationMethod")]
        public byte? HesaplamaSecenegi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? Ortak_PrimTutari { get; set; }

        public SelectList ParaBirimleri { get; set; }
        public SelectList PrimDonemleri { get; set; }
        public SelectList HesaplamaSecenegiTipleri { get; set; }


        public string PrimOdemeDonemiText { get; set; }
        public string HesaplamaSecenegiText { get; set; }
    }

    public class OdulluBirikimAnaTeminatlar
    {
        public bool Vefat { get; set; }
        public bool SureSonuPrimIadesi { get; set; }
        public int? VefatBedeli { get; set; }
        public int? SureSonuPrimIadesiBedeli { get; set; }
    }

    public class OdulluBirikimEkTeminatlar
    {
        public bool EkOdeme { get; set; }
        public int? EkOdemeBedeli { get; set; }
    }
}
