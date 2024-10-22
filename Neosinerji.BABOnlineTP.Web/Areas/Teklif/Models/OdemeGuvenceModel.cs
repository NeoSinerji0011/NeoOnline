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
    public class OdemeGuvenceModel
    {
        public bool TekrarTeklif { get; set; }
        public bool IliskiliTeklifVarmi { get; set; }
        public int? IliskiliTeklifId { get; set; }

        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }

        public OdemeGuvenceGenelBilgiler GenelBilgiler { get; set; }
        public OdemeGuvenceKAP OdemeGuvenceKAP { get; set; }
    }

    public class DetayOdemeGuvenceModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public bool OnProvizyon { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public AegonDetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }

        public OdemeGuvenceGenelBilgiler GenelBilgiler { get; set; }
        public OdemeGuvenceKAP OdemeGuvenceKAP { get; set; }
    }

    public class OdemeOdemeGuvenceModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class OdemeGuvenceGenelBilgiler
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "PremiumPaymentPeriod")]
        public byte primOdemeDonemi { get; set; }
        public string PrimOdemeDonemiText { get; set; }

        public string ekTeminatSecenegi { get; set; }
        public DateTime SigortaBaslangicTarihi { get; set; }

        //Ek Teminat Seçenekleri
        public bool KritikHastalikDPM { get; set; }
        public bool TamVeTaimiMaluliyetDPM { get; set; }
        public bool IssizlikDPM { get; set; }
        public bool KazaSonucuHastanedeyatarakTDPM { get; set; }

        public bool IssizlikDPM_FaydalanabilecekDurumdami { get; set; }

        public SelectList primOdemeDonemiList { get; set; }
    }

    public class OdemeGuvenceKAP
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "SelectInsuranceTime")]
        public byte? sigortaSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? aylikPrimTutari { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Select_Currency")]
        public byte paraBirimi { get; set; }

        public string paraBirimiText { get; set; }
        public string KapPoliceNo { get; set; }

        public DateTime PoliceBaslangicTarihi { get; set; }
        public SelectList paraBirimiList { get; set; }
    }

    public class OdemeGuvenceDetayPartialModel
    {
        public string AdSoyad { get; set; }
        public string SigortaBaslangicTarihi { get; set; }
        public string PrimOdemeDonemi { get; set; }

        //EK Teminatlar
        public bool KritikHastalikDPM { get; set; }
        public bool TamVeTaimiMaluliyetDPM { get; set; }
        public bool IssizlikDPM { get; set; }
        public bool KazaSonucuHastanedeyatarakTDPM { get; set; }

        //KAP
        public string PoliceBaslangicTarihi { get; set; }
        public string ParaBirimi { get; set; }
        public string SigortaSuresi { get; set; }
        public string AylikPrimTutari { get; set; }
        public string KapPoliceNo { get; set; }
        public int? IlgiliTeklifId { get; set; }
        public int? IlgiliTeklifNo { get; set; }
    }
}