using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;


namespace Neosinerji.BABOnlineTP.Web.Areas.PoliceTransfer.Models
{
    public class PoliceTransferModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AutoPoliceTransferSirketiKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TanzimBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime TanzimBitisTarihi { get; set; }

        public byte IslemTipi { get; set; }
        public byte TahsilatDosyasiVarmi { get; set; } 
        public byte TahsilatDosyasiVarmi2 { get; set; } 
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]

        public string TaliAcenteKodu { get; set; }
        public string TaliAcenteKoduOtomatik { get; set; }
       // public int? disKaynakKodu { get; set; }
        public string TransferTipi { get; set; }
        public string message { get; set; }
        public int policeCount { get; set; }
        public int basariliKayitlar { get; set; }
        public int updateKayitlar { get; set; }
        public int varOlanKayitlar { get; set; }
        public int hataliEklenmeyenKayitlar { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> AutoPoliceTransferSirketleri { get; set; }
        public SelectList Islemler { get; set; }
        public List<SelectListItem> TransferTipleri { get; set; }
        public List<SelectListItem> TaliAcenteler { get; set; }

        public SelectList AxaPoliceTipleri { get; set; }
        public byte AxaPoliceTipi { get; set; }

    }

    public class PoliceTranferKayitModel
    {
        public int policeCount { get; set; }
        public int basariliKayitlar { get; set; }
        public int basarisizKayitlar { get; set; }
      
        public string Path { get; set; }
        public string SigortaSirketiKodu { get; set; }
        public bool TahsilatMi { get; set; }
        public string TahsilatMesaj { get; set; }
        public string taliTvmKodu { get; set; }
        public int? disKaynakKodu { get; set; }

        public int HataliEklenmeyenPoliceSayisi { get; set; }
        public List<PoliceKontrolModel> HataliEklenmeyenPoliceLsitesi { get; set; }
    }

    public class GencPoliceTransferModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? BagliOlduguTVMKodu { get; set; }

        public int? BolgeKodu { get; set; }

        public List<SelectListItem> Bolgeler { get; set; }

        public string TVMUnvani { get; set; }

    }

    public class GencTransferKayitModel
    {

        public string TaliAcenteKodu { get; set; }
        public List<SelectListItem> TaliAcenteler { get; set; }
        public int taliCount { get; set; }
        public int basariliKayitlar { get; set; }
        public int basarisizKayitlar { get; set; }
        public string Path { get; set; }
        public string tvmKodu { get; set; }
        public int ToplamPoliceSayisi { get; set; }

        public List<Neosinerji.BABOnlineTP.Business.PoliceTransfer.PoliceService.PoliceKontrol> EklenmeyenPoliceler { get; set; }
    }

    public class PoliceTransferGoruntulemeModel
    {
        public string SigortaSirketNo { get; set; }
        public string SigortaSirket { get; set; }

        public MultiSelectList SigortaSirketleri { get; set; }
        public string[] SigortaSirketleriSelectList { get; set; }

        public DateTime TanzimBaslangicTarihi { get; set; }
        public DateTime TanzimBitisTarihi { get; set; }
        public List<AutoPoliceTransferModel> list { get; set; }
    }
    public class AutoPoliceTransferModel
    {
        public int TVMKodu { get; set; }
        public string TVMUnvan { get; set; }
        public string SirketKodu { get; set; }
        public string SirketUnvani { get; set; }
        public string PoliceTransferUrl { get; set; }
        public DateTime TanzimBaslangicTarihi { get; set; }
        public DateTime TanzimBitisTarihi { get; set; }
        public DateTime KayitTarihi { get; set; }
        public int KaydiEkleyenKullaniciKodu { get; set; }
        public string KaydiEkleyenKullaniciUnvan { get; set; }
    }

    public class PoliceKontrolModel
    {
        public string PoliceNo;
        public int EkNo;
        public int YenilemeNo;
        public string Hatatip;
    }

    public class OtomatikPoliceOnayModel
    {
        public List<PoliceOnaySonucModels> HataliPoliceListesi = new List<PoliceOnaySonucModels>();
        public int basariliKayit { get; set; }
        public int hataliKayit { get; set; }
        public int toplamOkunanKayit { get; set; }
        public string genelHataMesaji { get; set; }
    }

    public class PoliceOnaySonucModels
    {
        public string SigortaSirketKodu { get; set; }
        public string SigortaSirketUnvani { get; set; }
        public string PoliceNumarasi { get; set; }
        public int? EkNumarasi { get; set; }
        public int? YenilemeNumarasi { get; set; }
        public int TaliAcenteKodu { get; set; }
        public string TaliAcenteUnvani { get; set; }
        public bool GuncellemeBasarili { get; set; }
        public string BilgiMesaji { get; set; }
    }
}