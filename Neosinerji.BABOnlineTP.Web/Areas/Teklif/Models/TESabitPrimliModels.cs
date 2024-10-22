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
    public class TESabitPrimliModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }

        public TESabitPrimliGenelBilgiler GenelBilgiler { get; set; }
        public TESabitPrimliAnaTeminatlar AnaTeminatlar { get; set; }
        public TESabitPrimliEkTeminatlar EkTeminatlar { get; set; }


        // CONST
        public string AEGON_HTED_USD_KUR_PARAM { get; set; }
        public string AEGON_HTED_EUR_KUR_PARAM { get; set; }
        public string AEGON_KHYT_USD_KUR_PARAM { get; set; }
        public string AEGON_KHYT_EUR_KUR_PARAM { get; set; }
        public string AEGON_KSVT_TTAKSV_ANAT_TOPLAM_EUR { get; set; }
        public string AEGON_KSVT_TTAKSV_ANAT_TOPLAM_USD { get; set; }
        public string AEGON_TE_AnaTeminatLimiti_Dolar { get; set; }
        public string AEGON_TE_AnaTeminatLimiti_Euro { get; set; }
        public string AEGON_TE_YillikPrimLimiti_Dolar { get; set; }
        public string AEGON_TE_YillikPrimLimiti_Euro { get; set; }
    }

    public class TEDetayPartialModel
    {
        public TEDetayPartialModel()
        {
            this.AnaTeminatTutari = "0";
            this.KritikHastalikTutar = "0";
            this.KazaSonucuVefat = "0";
            this.TopluTasimaAraclarindaVefat = "0";
            this.TamVeDaimiMaluliyet = "0";
            this.MaluliyetYillikDestek = "0";
            this.KazaSonucu_TedaviMasraflariBedeli = "0";
            this.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli = "0";
        }

        public string AdSoyad { get; set; }
        public string SigortaBaslangicTarihi { get; set; }
        public string SigortaSuresi { get; set; }
        public string PrimOdemeDonemi { get; set; }
        public string HesaplamaSecenegi { get; set; }
        public string HesaplamaSecenegiText { get; set; }
        public string YillikPrim { get; set; }

        //ANA TEMINAT
        public string AnaTeminatAdi { get; set; }
        public string AnaTeminatTutari { get; set; }

        //EK TEMINATLAR
        public string KritikHastalikTutar { get; set; }
        public string KazaSonucuVefat { get; set; }
        public string TopluTasimaAraclarindaVefat { get; set; }
        public string TamVeDaimiMaluliyet { get; set; }
        public string MaluliyetYillikDestek { get; set; }

        public string KazaSonucu_TedaviMasraflariBedeli { get; set; }
        public string KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli { get; set; }
    }

    public class DetayTESabitPrimliModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }

        public TESabitPrimliGenelBilgiler GenelBilgiler { get; set; }
        public TESabitPrimliAnaTeminatlar AnaTeminatlar { get; set; }
        public TESabitPrimliEkTeminatlar EkTeminatlar { get; set; }
    }

    public class OdemeTESabitPrimliModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class TESabitPrimliTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class TESabitPrimliGenelBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "InsuranceSince")]
        public DateTime SigortaBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Select_Currency")]
        public byte? ParaBirimi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectInsuranceTime")]
        public byte? SigortaSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "PremiumPaymentPeriod")]
        public byte? PrimOdemeDonemi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectCalculationMethod")]
        public int? HesaplamaSecenegi { get; set; }

        public SelectList ParaBirimleri { get; set; }
        public SelectList PrimDonemleri { get; set; }
        public SelectList HesaplamaSecenekleri { get; set; }

        //FOR DETAIL
        public string ParaBirimiText { get; set; }
        public string PrimOdemeDonemiText { get; set; }
        public string HesaplamaSecenegiText { get; set; }
    }

    public class TESabitPrimliAnaTeminatlar
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte? AnaTeminat { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? AnaTeminatSigortaBedeli { get; set; }

        public SelectList AnaTeminatlar { get; set; }

        public string AnaTeminatText { get; set; }
    }

    public class TESabitPrimliEkTeminatlar
    {
        public bool KritikHastaliklar { get; set; }
        public bool TamVeDaimiMaluliyet { get; set; }
        public bool KazaSonucuVefat { get; set; }
        public bool TopluTasimaAraclariKSV { get; set; }
        public bool MaluliyetYillikDestek { get; set; }

        //Yeni ek Teminatlar
        public bool KazaSonucu_TedaviMasraflari { get; set; }
        public bool KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme { get; set; }



        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? KritikHastaliklarSigortaBedeli { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? TamVeDaimiMaluliyetSigortaBedeli { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? KazaSonucuVefatSigortaBedeli { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? TopluTasimaAraclariKSVSigortaBedeli { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? MaluliyetYillikDestekSigortaBedeli { get; set; }


        //Yeni ek Teminatlar
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? KazaSonucu_TedaviMasraflariBedeli { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli { get; set; }
    }
}