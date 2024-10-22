using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Hasar.Models
{
    public class HasarModel
    {
        public string PoliceNo { get; set; }
        public int PoliceId { get; set; }
        public int HasarId { get; set; }
        public string SigortaliTCVKN { get; set; }
        public string SigortaliUnvani { get; set; }
        public int? YenilemeNo { get; set; }
        public int? EkNo { get; set; }
        public int? UrunKodu { get; set; }
        public string UrunAdi { get; set; }
        public int? BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string PlakaNo { get; set; }
        public int? TaliAcenteKodu { get; set; }
        public string TaliAcenteAdi { get; set; }
        public string SigortaSirketKodu { get; set; }
        public string SigortaSirketi { get; set; }
        public int? AcenteKodu { get; set; }
        public string AcenteUnvani { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HasarDosyaNo { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime IhbarTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime HasarTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HasarSaati { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HasarMevki { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HasarTuruNedeni { get; set; }
        public string HasarDosyaDurumu { get; set; }
        public string HasarDosyaRedNedeni { get; set; }
        public string DosyayaAtananMTKodu { get; set; }
        public bool AnlasmaliServisVarMi { get; set; }
        public int? AnlasmaliServisKodu { get; set; }

        public List<SelectListItem> AnlasmaliServisler { get; set; }
        public SelectList HasarDosyaDurumlari { get; set; }
        public List<SelectListItem> MTKodlari { get; set; }

        public HasarEksperIslemleriListModel EksperList { get; set; }
        public HasarNotlarListModel NotlarList { get; set; }
        public HasarZorunluEvrakListModel ZorunluEvrakList { get; set; }
        public HasarBankaHesaplariListModel BankaHesaplariList { get; set; }
        public HasarIletisimYetkilileriListModel IletisimYetkilileriList { get; set; }
    }
    public class HasarEksperIslemleriListModel
    {
        public List<HasarEksperModel> Items { get; set; }
    }
    public class HasarNotlarListModel
    {
        public List<HasarNotlarModel> Items { get; set; }
    }
    public class HasarZorunluEvrakListModel
    {
        public List<ZorunluEvrakModel> Items { get; set; }
    }

    public class HasarNotlarModel
    {
        public int HasarId { get; set; }
        public int NotId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KonuAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public System.DateTime EklemeTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string NotAciklamasi { get; set; }
    }

    public class ZorunluEvrakModel
    {
        public int HasarId { get; set; }
        public int EvrakId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string EvrakKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime EvrakKayitTarihi { get; set; }

        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Evrak { get; set; }
        public string EvrakAdi { get; set; }
        public List<SelectListItem> EvrakList { get; set; }
    }

    public class HasarEksperModel
    {
        public int? HasarId { get; set; }
        public int EksperId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EksperSorumlusuKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TahminiHasarBedeli { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? TahminiHasarParaBirimi { get; set; }

        public string AnlasmaliServisBedeli { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? AnlasmaliServisParaBirimi { get; set; }

        public string TahakkukBedeli { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? TahakkukParaBirimi { get; set; }

        public string RucuBedeli { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? RucuBedeliParaBirimi { get; set; }

        public string RedBedeli { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? RedParaBirimi { get; set; }

        public string AsistansFirma { get; set; }
        public string AsistansFirmaBedeli { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public short? AsistansFirmaParaBirimi { get; set; }

        public string OdemeTarihi { get; set; }
        public string BildirimTarihi { get; set; }
        public string GelisSaati { get; set; }
        public string BeklemeSuresi { get; set; }


        public string TahakkukParaBirimiText { get; set; }
        public string RucuParaBirimiText { get; set; }
        public string RedParaBirimiText { get; set; }
        public string AsistansFirmaParaBirimiText { get; set; }
        public string AnlasmaliServisParaBirimiText { get; set; }
        public string TahminiHasarParaBirimiText { get; set; }
        public string EksperAdSoyad { get; set; }

        public SelectList TahminiHasarParaBirimleri { get; set; }
        public SelectList AnlasmaliServisParaBirimleri { get; set; }
        public SelectList TahakkukParaBirimleri { get; set; }
        public SelectList RucuParaBirimleri { get; set; }
        public SelectList RedParaBirimleri { get; set; }
        public SelectList AsistansFirmaParaBirimleri { get; set; }
        public List<SelectListItem> EksperList { get; set; }
        public List<SelectListItem> AsistansFirmalari { get; set; }

    }

    public class HasarBankaHesaplariListModel
    {
        public List<HasarBankaHesaplariModel> Items { get; set; }
    }

    public class HasarBankaHesaplariModel
    {
        public int HasarId { get; set; }
        public int BankaHesapId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BankaAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SubeAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HesapNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IBAN { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HesapAdi { get; set; }


        public List<SelectListItem> Bankalar { get; set; }
        public List<SelectListItem> Subeler { get; set; }
    }


    public class HasarIletisimYetkilileriListModel
    {
        public List<HasarIletisimYetkilileriModel> Items { get; set; }
    }
    public class HasarIletisimYetkilileriModel
    {

        public int IletisimYetkiliId { get; set; }
        public int HasarId { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string GorusulenKisi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Gorevi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte TelefonTipi { get; set; }
        public string TelefonTipiText { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TelefonNo { get; set; }
        public string Email { get; set; }
        public SelectList TelefonTipleri { get; set; }

    }
}
