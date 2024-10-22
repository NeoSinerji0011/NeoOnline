using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.GorevTakip.Models
{   
    public class GorevTakipModel
    {
        public int IsAlanTvmKodu { get; set; }
        public int IsAlanKullaniciKodu { get; set; }
        public int TalepKanalKodu { get; set; }
        public string SigortaSirketiKodu { get; set; }
        public int? BransKodu { get; set; }
        public string PoliceNumarasi { get; set; }
        public int? YenilemeNumarasi { get; set; }
        public int? ZeylNumarasi { get; set; }
        public byte Durum { get; set; }
        public byte IsTipi { get; set; }
        public string EvrakNo { get; set; }
        public string Aciklama { get; set; }
        public byte OncelikSeviyesi { get; set; }
        public string TalepYapanAcente { get; set; }
        public string Baslik { get; set; }
        public string GonderenTCVKN { get; set; }
        public string GonderenAdiSoyadi { get; set; }
        public string GonderenEmail { get; set; }
        public string GonderenTelefon { get; set; }
        public DateTime AtamaTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime BaslamaTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TahminiBitisTarihi { get; set; }
        public DateTime? TamamlamaTarihi { get; set; }
        public List<SelectListItem> DurumTipleri { get; set; }
        public List<SelectListItem> IsTipleri { get; set; }
        public List<SelectListItem> OncelikSeviyeleri { get; set; }
        public List<SelectListItem> BransKodlari { get; set; }
        public List<SelectListItem> SigortaSirketeri { get; set; }
        public List<SelectListItem> IsAlanTvmKodlari { get; set; }
        public List<SelectListItem> IsAlanKullaniciKodlari { get; set; }
        public List<SelectListItem> TalepKanallari { get; set; }


    }
    public class GorevTakipGuncelleModel : GorevTakipModel
    {
        public int IsId { get; set; }

        public AtananIsDokumanlarListModel DokumanlariList { get; set; }
        public AtananIsNotlarListModel NotlarList { get; set; }

    }
    public class GorevTakipDetayModel
    {
        public int IsId { get; set; }
        public int IsAlanTvmKodu { get; set; }
        public string IsAlanTvmUnvani { get; set; }
        public int? IsAlanKullaniciKodu { get; set; }
        public string IsAlanKullaniciUnvani { get; set; }

        public int IsAtayanTvmKodu { get; set; }
        public string IsAtayanTvmUnvani { get; set; }
        public int IsAtayanKullaniciKodu { get; set; }
        public string IsAtayanKullaniciUnvani { get; set; }
        public int TalepKanaliKodu { get; set; }
        public string TalepKanali { get; set; }
        public string SigortaSirketiKodu { get; set; }
        public string SigortaSirketiUnvani { get; set; }
        public string Baslik { get; set; }
        public int? BransKodu { get; set; }
        public string BransAdi { get; set; }
        public string PoliceNumarasi { get; set; }
        public int? YenilemeNumarasi { get; set; }
        public int? ZeylNumarasi { get; set; }
        public byte Durum { get; set; }
        public string DurumAciklama { get; set; }
        public byte IsTipi { get; set; }
        public string IsTipiAciklama { get; set; }
        public string Aciklama { get; set; }
        public byte OncelikSeviyesi { get; set; }
        public string OncelikSeviyeAciklama { get; set; }
        public string TalepYapanAcente { get; set; }
        public string GonderenTCVKN { get; set; }
        public string GonderenAdiSoyadi { get; set; }
        public string GonderenEmail { get; set; }
        public string GonderenTelefon { get; set; }
        public DateTime AtamaTarihi { get; set; }
        public DateTime BaslamaTarihi { get; set; }
        public DateTime TahminiBitisTarihi { get; set; }
        public DateTime? TamamlamaTarihi { get; set; }

        public string EvrakNo { get; set; }
        public AtananIsDokumanlarListModel DokumanlariList { get; set; }
        public AtananIsNotlarListModel NotlarList { get; set; }
    }

    public class AtananIsDokumanlarListModel
    {
        public List<AtananIsDokumanlarModel> Items { get; set; }
        public string sayfaAdi { get; set; }
    }
    public class AtananIsNotlarListModel
    {
        public List<AtananIsNotlarModel> Items { get; set; }
        public string sayfaAdi { get; set; }
        public bool AdminKullanicimi { get; set; }
    }
    public class AtananIsDokumanlarModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int IsId { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int SiraNo { get; set; }
        public int IsAlanTvmkodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DokumanAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime EklemeTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int EkleyenPersonelKodu { get; set; }
        public string EkleyenPersonelAdi { get; set; }
        //[Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DokumanURL { get; set; }
    }

    public class AtananIsNotlarModel
    {
        public int NotId { get; set; }
        public int IsId { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string NotAciklamasi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Konu { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime KayitTarihi { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMKodu { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int TVMPersonelKodu { get; set; }
        public string EkleyenPersonelAdi { get; set; }
    }
    public class IslerimDetayModel
    {
        public List<IsModel> list = new List<IsModel>();
    }
    public class IsModel
    {
        public string IsNumarasi { get; set; }
        public string Baslik { get; set; }
        public string IsTipi { get; set; }
        public string TalepKanali { get; set; }
        public byte DurumKodu { get; set; }
        public string Durum { get; set; }
        public byte OncelikSeviyeKodu { get; set; }
        public string OncelikSeviyesi { get; set; }
        public string AtamaTarihi { get; set; }
        public string BaslamaTarihi { get; set; }
        public string TahminiBitisTarihi { get; set; }
        public string TalepYapanAcenteUnvani { get; set; }
    }

    public class AtananIslerModel
    {
        public Nullable<byte> Durum { get; set; }

        public Nullable<byte> IsTipi { get; set; }

        public Nullable<byte> OncelikSeviyesi { get; set; }
        public int TVMKodu { get; set; }
        public List<SelectListItem> Tvmler { get; set; }
        public int TVMKullaniciKodu { get; set; }
        public List<SelectListItem> KullaniciList { get; set; }
        public string TVMKullaniciAdi { get; set; }

        //public MultiSelectList TvmlerItems { get; set; }
        //public int[] TvmlerSelectList { get; set; }

        //public MultiSelectList KullanicilarItems { get; set; }
        //public int[] KullanicilarSelectList { get; set; }

        public DateTime IsBaslangicTarihi { get; set; }
        public DateTime IsBasBitisTarihi { get; set; }

        public List<SelectListItem> Durumlar { get; set; }

        public List<SelectListItem> IsTipleri { get; set; }

        public List<SelectListItem> OncelikSeviyeleri { get; set; }

        public List<AtananIslerProcedureModel> RaporSonuc { get; set; }
    }
}