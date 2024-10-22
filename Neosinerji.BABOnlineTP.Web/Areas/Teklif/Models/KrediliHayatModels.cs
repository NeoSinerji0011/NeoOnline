using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class KrediliHayatProvider
    {
        public static List<SelectListItem> GetKrediSureleri()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
            for (int i = 1; i <= 10; i++)
            {
                items.Add(new SelectListItem() { Value = i.ToString(), Text = i.ToString(), Selected = true });
            }
            return items;
        }

        public static List<SelectListItem> GetKrediliTurleri()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
            items.Add(new SelectListItem() { Value = KrediTurleri.Araba.ToString(), Text = babonline.Car, Selected = true });
            items.Add(new SelectListItem() { Value = KrediTurleri.Konut.ToString(), Text = babonline.Home2, Selected = true });
            items.Add(new SelectListItem() { Value = KrediTurleri.KrediKarti.ToString(), Text = babonline.Credit_Card, Selected = true });
            items.Add(new SelectListItem() { Value = KrediTurleri.KrediMevduat.ToString(), Text = babonline.LoanDeposit, Selected = true });
            items.Add(new SelectListItem() { Value = KrediTurleri.Tuketici.ToString(), Text = babonline.Consumer, Selected = true });
            items.Add(new SelectListItem() { Value = KrediTurleri.CekKarnesi.ToString(), Text = babonline.Checkbook, Selected = true });

            return items;
        }

        public static string GetKrediTuruText(int krediTuru)
        {
            switch (krediTuru)
            {
                case KrediTurleri.Araba: return babonline.Car;
                case KrediTurleri.Konut: return babonline.Home2;
                case KrediTurleri.KrediKarti: return babonline.Credit_Card;
                case KrediTurleri.KrediMevduat: return babonline.LoanDeposit;
                case KrediTurleri.Tuketici: return babonline.Consumer;
                case KrediTurleri.CekKarnesi: return babonline.Checkbook;
                default: return String.Empty;
            }
        }
    }

    public class KrediliHayatModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public KrediBilgileriModel Kredi { get; set; }
        public KrediAlanAdresModel Adres { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
    }

    public class DetayKrediliHayatModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public DetayKrediBilgileriModel Kredi { get; set; }
        public DetayKrediAlanAdresModel Adres { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public KrediliHayatOdemeBilgileriModel OdemeBilgileri { get; set; }
    }

    public class KrediliHayatOdemeBilgileriModel
    {
        public int teklifId { get; set; }

        public decimal? NetPrim { get; set; }
        public decimal? BrutPrim { get; set; }

        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMLogoURL { get; set; }

        public string PoliceURL { get; set; }
        public string TUMPoliceNo { get; set; }
    }

    public class KrediBilgileriModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BabaAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime? BaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public decimal? Tutar { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KrediSuresi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KrediTuru { get; set; }


        public List<SelectListItem> Sureler { get; set; }
        public List<SelectListItem> KrediTurleri { get; set; }
    }

    public class DetayKrediBilgileriModel
    {
        public string BabaAdi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public decimal Tutar { get; set; }
        public int KrediSuresi { get; set; }
        public string KrediTuruText { get; set; }
    }

    public class KrediAlanAdresModel
    {
        public int AdresTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UlkeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IlKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? IlceKodu { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Neighborhood_Length")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Semt { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_ParishLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Mahalle { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_AvenueLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Cadde { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_StreetLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Sokak { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_ApartmentLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Apartman { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int PostaKodu { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Building_NoLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BinaNo { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Building_NoLength")]
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaireNo { get; set; }

        public List<SelectListItem> UlkeLer { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> Ilceler { get; set; }
        public List<SelectListItem> AdresTipleri { get; set; }
    }

    public class DetayKrediAlanAdresModel
    {
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public string Semt { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Sokak { get; set; }
        public string Apartman { get; set; }
        public int? PostaKodu { get; set; }
        public string BinaNo { get; set; }
        public string DaireNo { get; set; }
        public string Kat { get; set; }
    }
}