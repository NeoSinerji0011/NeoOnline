using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Models
{
    public class KullaniciNotEkleModel
    {
        public int KullaniciKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Konu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Aciklama { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public byte Oncelik { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", NullDisplayText = "", ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true)]
        //public DateTime? BitisTarihi { get; set; }

        public SelectList Oncelikler { get; set; }
    }

    public class KullaniciNotListeleModel
    {
        public int NotId { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public string Oncelik { get; set; }
        public DateTime? BitisTarihi { get; set; }
    }

    public class KullaniciNotDetayModel
    {
        public int NotId { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public string EklemeTarihi { get; set; }
    }

    public class KullaniciDuyuruDetayModel
    {
        public int DuyuruId { get; set; }
        public string Konu { get; set; }
        public string Aciklama { get; set; }
        public string BitisTarihi { get; set; }
        public string BaslamaTarihi { get; set; }
    }

    public class OfflinePerformansModel
    {
        public int tvmKodu { get; set; }
        public int taliTvmKodu { get; set; }
        public int donemYil { get; set; }
        public int bransKodu { get; set; }
        public bool MerkezAcenteMi { get; set; }
        public bool BolgeYetkilisiMi { get; set; }

        public string TVMListe { get; set; }
        public MultiSelectList tvmler { get; set; }
        public string[] tvmList { get; set; }

        public SelectList donemler { get; set; }

        public List<PoliceAylar> list = new List<PoliceAylar>();

        public bool uretimSatir1 { get; set; }

        public List<OfflineUretimPerformansModel> uretimList = new List<OfflineUretimPerformansModel>();
    }
    public class OfflinePerformansModelKullanici
    {
        public int tvmKodu { get; set; }
        public int kullaniciKodu { get; set; }
        public int donemYil { get; set; }
        public int bransKodu { get; set; }
        public bool MerkezAcenteMi { get; set; }
        public bool BolgeYetkilisiMi { get; set; }

        public string KullaniciListe { get; set; }
        public MultiSelectList kullanicilar { get; set; }
        public string[] kullaniciList { get; set; }

        public SelectList donemler { get; set; }

        public List<PoliceAylar> list = new List<PoliceAylar>();

        public bool uretimSatir1 { get; set; }

        public List<OfflineUretimPerformansModel> uretimList = new List<OfflineUretimPerformansModel>();
    }
    public class OfflineUretimPerformansModel
    {
        public int tvmKodu { get; set; }
        public int taliTvmKodu { get; set; }
        public int donemYil { get; set; }
        public int bransKodu { get; set; }

        public decimal? policeAdetOcak { get; set; }
        public decimal? netPrimOcak { get; set; }
        public decimal? policeKomisyonOcak { get; set; }
        public decimal? policeVerilenKomisyonOcak { get; set; }

        public decimal? policeAdetSubat { get; set; }
        public decimal? netPrimSubat { get; set; }
        public decimal? policeKomisyonSubat { get; set; }
        public decimal? policeVerilenKomisyonSubat { get; set; }

        public decimal? policeAdetMart { get; set; }
        public decimal? netPrimMart { get; set; }
        public decimal? policeKomisyonMart { get; set; }
        public decimal? policeVerilenKomisyonMart { get; set; }

        public decimal? policeAdetNisan { get; set; }
        public decimal? netPrimNisan { get; set; }
        public decimal? policeKomisyonNisan { get; set; }
        public decimal? policeVerilenKomisyonNisan { get; set; }

        public decimal? policeAdetMayis { get; set; }
        public decimal? netPrimMayis { get; set; }
        public decimal? policeKomisyonMayis { get; set; }
        public decimal? policeVerilenKomisyonMayis { get; set; }

        public decimal? policeAdetHaziran { get; set; }
        public decimal? netPrimHaziran { get; set; }
        public decimal? policeKomisyonHaziran { get; set; }
        public decimal? policeVerilenKomisyonHaziran { get; set; }

        public decimal? policeAdetTemmuz { get; set; }
        public decimal? netPrimTemmuz { get; set; }
        public decimal? policeKomisyonTemmuz { get; set; }
        public decimal? policeVerilenKomisyonTemmuz { get; set; }

        public decimal? policeAdetAgustos { get; set; }
        public decimal? netPrimAgustos { get; set; }
        public decimal? policeKomisyonAgustos { get; set; }
        public decimal? policeVerilenKomisyonAgustos { get; set; }

        public decimal? policeAdetEylul { get; set; }
        public decimal? netPrimEylul { get; set; }
        public decimal? policeKomisyonEylul { get; set; }
        public decimal? policeVerilenKomisyonEylul { get; set; }

        public decimal? policeAdetEkim { get; set; }
        public decimal? netPrimEkim { get; set; }
        public decimal? policeKomisyonEkim { get; set; }
        public decimal? policeVerilenKomisyonEkim { get; set; }

        public decimal? policeAdetKasim { get; set; }
        public decimal? netPrimKasim { get; set; }
        public decimal? policeKomisyonKasim { get; set; }
        public decimal? policeVerilenKomisyonKasim { get; set; }

        public decimal? policeAdetAralik { get; set; }
        public decimal? netPrimAralik { get; set; }
        public decimal? policeKomisyonAralik { get; set; }
        public decimal? policeVerilenKomisyonAralik { get; set; }

        public MultiSelectList tvmler { get; set; }
        public string[] tvmList { get; set; }

        public SelectList donemler { get; set; }

    }

    public class MobilOnayKoduModel
    {
        public string smsKodu { get; set; }
        public string cepTelefonu { get; set; }
        public int TvmKodu { get; set; }
        public int KullaniciKodu { get; set; }
    }

}