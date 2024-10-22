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
    public class DaskModel
    {
        public bool TekrarTeklif { get; set; }
        public HazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public DaskTeklifOdemeModel Odeme { get; set; }
        public DaskRizikoAdresModel RizikoAdresBilgiler { get; set; }
        public DaskRizikoDigerBilgiler RizikoDigerBilgiler { get; set; }
        public DaskRizikoGenelBilgiler RizikoGenelBilgiler { get; set; }
        public string UrunAdi { get; set; }
    }

    public class DetayDaskModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public TeklifFiyatModel Fiyat { get; set; }
        public KrediKartiOdemeModel KrediKarti { get; set; }
        public DaskPoliceOdemeModel OdemeBilgileri { get; set; }

        public DaskRizikoGenelBilgiler RizikoGenelBilgiler { get; set; }
        public DaskRizikoAdresModel RizikoAdresBilgiler { get; set; }
        public DaskRizikoDigerBilgiler RizikoDigerBilgiler { get; set; }
    }

    public class OdemeDaskModel
    {
        public KrediKartiOdemeModel KrediKarti { get; set; }
    }

    public class DaskTeklifOdemeModel
    {
        public bool OdemeSekli { get; set; }
        public byte OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
    }

    public class DaskPoliceOdemeModel
    {
        public int teklifId { get; set; }

        public decimal NetPrim { get; set; }
        public decimal BrutPrim { get; set; }
        public decimal Vergi { get; set; }
        public int TaksitSayisi { get; set; }

        public int TUMKodu { get; set; }
        public string TUMUnvani { get; set; }
        public string TUMLogoURL { get; set; }

        public string PoliceURL { get; set; }
        public string TUMPoliceNo { get; set; }
    }

    public class DaskRizikoGenelBilgiler
    {
        //Yürürlükte Dask Poliçesi Var mı?
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool YururlukteDaskPolicesiVarmi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaskSigortaSirketi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaskPoliceninVadeTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaskPoliceNo { get; set; }



        //Yangın Poliçesi Var mı?
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool YanginPolicesiVarmi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YanginSigortaSirketi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YanginPoliceNumarasi { get; set; }



        //Rehinli Alacaklı (Dain-i Mürtehin) Var mı?
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public bool RehinliAlacakliDainMurtehinVarmi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Tipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KurumBanka { get; set; }
        public string KurumBankaAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Sube { get; set; }
        public string SubeAdi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KrediReferansNo_HesapSozlesmeNo { get; set; }

        public DateTime? KrediBitisTarihi { get; set; }
        public string KrediTutari { get; set; }
        public byte DovizKodu { get; set; }
        public string DovizKoduText { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> Tipler { get; set; }
        public List<SelectListItem> DovizKodlari { get; set; }
        public List<SelectListItem> Kurum_Bankalar { get; set; }
        public List<SelectListItem> Subeler { get; set; }
    }

    public class DaskRizikoAdresModel
    {
        //Adres Bilgileri
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int IlKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int IlceKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SemtBeldeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MahalleKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string CaddeKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BinaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaireKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int? PostaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string UATVKodu { get; set; }

        public string Il { get; set; }
        public string Ilce { get; set; }
        public string SemtBelde { get; set; }
        public string Mahalle { get; set; }
        public string Cadde { get; set; }
        public string Bina { get; set; }
        public string Daire { get; set; }

        public string ParitusAdresDogrulama { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }


        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> Ilceler { get; set; }
        public List<SelectListItem> Beldeler { get; set; }
        public List<SelectListItem> Mahalleler { get; set; }
        public List<SelectListItem> Caddeler { get; set; }
        public List<SelectListItem> Binalar { get; set; }
        public List<SelectListItem> Daireler { get; set; }
    }

    public class DaskRizikoDigerBilgiler
    {
        //Diğer bilgiler
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string YapiTarzi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BinaKatSayisi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string BinaInsaYili { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaireKullanimSekli { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string DaireBrutYuzolcumu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string HasarDurumu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SigortaEttirenSifati { get; set; }

        public string KatNo { get; set; }
        public string PaftaNo { get; set; }
        public string SayfaNo { get; set; }
        public string Ada { get; set; }
        public string Parsel { get; set; }

        public bool TapudaBirdenFazlaSigortaliVarmi { get; set; }
        public List<string> SigortaliList { get; set; }


        //1. Çelik - Betonarme - Karkas / 2. Diğer / 3. Yığma Kagir 
        public List<SelectListItem> YapiTarzlari { get; set; }

        //1. 01-04 Arası Kat / 2. 05-07 Arası Kat / 3. 08-19 Arası Kat / 4. 20 - Üzeri Katlar
        public List<SelectListItem> BinaKatSayilari { get; set; }

        //1. 1975 - Öncesi / 2. 1976 - 1996 / 3. 1997 - 1999 / 4. 2000 - 2006 / 5. 2007 ve Sonrası
        public List<SelectListItem> BinaInsaYillari { get; set; }

        //1. Mesken / 2. Büro / 3. Ticarethane / Diğer
        public List<SelectListItem> DaireKullanımSekilleri { get; set; }

        //1. Hasarsız / 2. Az Hasarlı / 3. Orta Hasarlı
        public List<SelectListItem> HasarDurumlari { get; set; }

        //1. Mal Sahibi / 2. Kiracı / 3. İnfifa Hakkı Sahibi / 4. Yönetici / 5. Akraba / 6. Daini Mürtehin / 7. Diğer
        public List<SelectListItem> S_EttirenSifatlari { get; set; }
    }
}