using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{

    public class PrimIadeli2Model
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public PrimIadeliTeklifOdemeModel Odeme { get; set; }

        public PrimIadeli2GenelBilgiler GenelBilgiler { get; set; }

    }

    public class PrimIadeli2DetayPartialModel
    {
        public string AdSoyad { get; set; }
        public string SigortaBaslangicTarihi { get; set; }
        public string PrimOdemeDonemiText { get; set; }
        public string HesaplamaSecenegiText { get; set; }
        public string Tutar { get; set; }
    }

    public class DetayPrimIadeli2Model
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }

        public PrimIadeli2GenelBilgiler GenelBilgiler { get; set; }
    }

    public class OdemePrimIadeli2Model
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class PrimIadeli2TeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class PrimIadeli2GenelBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "InsuranceSince")]
        public DateTime SigortaBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "PremiumPaymentPeriod")]
        public byte? PrimOdemeDonemi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectCalculationMethod")]
        public byte? HesaplamaSecenegi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? VefatTutari { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte? YillikPrimTutari { get; set; }

        public SelectList PrimDonemleri { get; set; }
        public SelectList HesaplamaSecenekleri { get; set; }
        public SelectList YillikPrimTutarlari { get; set; }

        //FOR DETAIL
        public string PrimOdemeDonemiText { get; set; }
        public string HesaplamaSecenegiText { get; set; }
        public string YillikPrimTutarText { get; set; }
    }


}