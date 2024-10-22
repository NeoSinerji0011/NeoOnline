using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Validations;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models
{
    public class MapfreKaskoModel
    {
        public bool TekrarTeklif { get; set; }
        public MapfreHazirlayanModel Hazirlayan { get; set; }
        public SigortaliModel Musteri { get; set; }
        public MapfreKaskoAracBilgiModel Arac { get; set; }
        public MapfreKaskoDainiMurtein DainiMurtein { get; set; }
        public EskiPoliceModel EskiPolice { get; set; }
        public TasiyiciSorumlulukModel Tasiyici { get; set; }
        public MapfreKaskoTeminatModel Teminat { get; set; }
        public TeklifUMListeModel TeklifUM { get; set; }
        public MapfreOdemeModel KrediKarti { get; set; }
        public KaskoTeklifOdemeModel Odeme { get; set; }
        public KaskoIndirimSurprimModel IndirimSurprim { get; set; }
        public string Aciklama { get; set; }
    }

    public class MapfreHazirlayanModel
    {
        public bool FarkliAcenteSecebilir { get; set; }
        public int? TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
    }

    public class DetayMapfreKaskoModel
    {
        public int TeklifId { get; set; }
        public string TeklifNo { get; set; }
        public string TUMTeklifNo { get; set; }
        public string ProjeKodu { get; set; }
        public DetayHazirlayanModel Hazirlayan { get; set; }
        public DetayMusteriModel Sigortali { get; set; }
        public DetayMusteriModel SigortaEttiren { get; set; }
        public DetayAracBilgiModel Arac { get; set; }
        public DetayEskiPoliceModel EskiPolice { get; set; }
        public DetayTasiyiciSorumlulukModel Tasiyici { get; set; }
        public MapfreKaskoDainiMurtein DainiMurtein { get; set; }
        public DetayMapfreKaskoTeminatModel Teminat { get; set; }
        public MapfreTeklifFiyatDetayModel Fiyat { get; set; }
        public MapfreOdemeModel KrediKarti { get; set; }
        public KaskoPoliceOdemeModel OdemeBilgileri { get; set; }
        public DetayMapfreIndirimSurprimModel IndirimSurprim { get; set; }
        public string Aciklama { get; set; }
        public bool Satinalinabilir { get; set; }
    }

    public class MapfreKaskoDainiMurtein
    {
        public bool DainiMurtein { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [MinLength(10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength")]
        [StringLength(10, MinimumLength = 10, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_TaxNumberLength")]
        public string KimlikNo { get; set; }

        [Required(ErrorMessage="Lütfen önce Daini Murtehin vergi kimlik numarasını sorgulayınız.")]        
        public string Unvan { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        [Range(1, 3, ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KurumTipi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KurumKodu { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KurumKodu1 { get; set; }

        public string SubeKodu { get; set; }

        public string KurumTipiAdi { get; set; }
        public string KurumAdi { get; set; }

        public SelectList KurumTipleri { get; set; }
        public SelectList Kurumlar { get; set; }
    }

    //public class MapfreKaskoDMKurumModel
    //{
    //    public bool DainiMurtein { get; set; }

    //    [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
    //    public int KurumTipi { get; set; }

    //    [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
    //    public string KurumKodu { get; set; }

    //    public string SubeKodu { get; set; }

    //    public string KurumTipiAdi { get; set; }
    //    public string KurumAdi { get; set; }

    //    public SelectList KurumTipleri { get; set; }
    //    public SelectList Kurumlar { get; set; }

    //    //Eski teklif / poliçelerde görünmesi için
    //    public string KimlikNo { get; set; }
    //    public string Unvan { get; set; }
    //}

    public class MapfreKaskoTeminatModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AMSKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int OlumSakatlikTeminat { get; set; }
        
        public bool Tedavi { get; set; }

        [MapfreTedaviTeminat(ErrorMessage = "Tedavi teminat tutarı, Ölüm sakatlık teminatının %10'unu geçemez.")]
        public int TedaviTeminat { get; set; }

        public bool Deprem { get; set; }
        public string DepremMuafiyetKodu { get; set; }
        public bool Yutr_Disi_Teminat { get; set; }
        public int Yurt_Disi_Teminati_Sure { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Yurt_Disi_Teminat_Ulke { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string IkameTuru { get; set; }

        public bool Hukuksal_Koruma_Teminati { get; set; }
        public bool Eskime_Payi_Teminati { get; set; }
        public bool Anahtar_Kaybi_Teminati { get; set; }
        public bool Ozel_Esya_Teminati { get; set; }
        public bool Anahtarla_Calinma_Teminati { get; set; }
        public bool FiloPolice_Teminati { get; set; }

        public bool Kullanici_Teminat { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Kullanici_TCKN { get; set; }
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string Kullanici_Adi { get; set; }

        public string OnarimYeri { get; set; }
        public List<SelectListItem> OnarimYerleri { get; set; }

        public bool Aksesuar_Teminati { get; set; }
        public List<MapfreAksesuarModel> Aksesuarlar { get; set; }
        public bool ElektronikCihaz_Teminati { get; set; }
        public List<MapfreAksesuarModel> Cihazlar { get; set; }
        public bool TasinanYuk_Teminati { get; set; }
        public List<MapfreTasinanYukModel> TasinanYukler { get; set; }
        public List<SelectListItem> AMS { get; set; }
        public List<SelectListItem> Yurtdisi_Teminat_Sureleri { get; set; }
        public List<SelectListItem> Yurtdisi_Teminat_Ulkeler { get; set; }
        public SelectList DepremMuafiyetKodlari { get; set; }
        public List<SelectListItem> AksesuarTipleri { get; set; }
        public List<SelectListItem> ElektronikCihazTipleri { get; set; }
        public List<SelectListItem> TasinanYukTipleri { get; set; }
        public List<SelectListItem> IkameTurleri { get; set; }
    }

    public class MapfreAksesuarModel
    {
        public string AksesuarTip { get; set; }
        [Required]
        public string Aciklama { get; set; }
        [Required]
        public int Bedel { get; set; }
    }

    public class MapfreTasinanYukModel
    {
        public string TasinanYukTip { get; set; }
        [Required]
        public string Aciklama { get; set; }
        [Required]
        public int Bedel { get; set; }
        [Required]
        public int Fiyat { get; set; }
    }

    public class DetayMapfreKaskoTeminatModel
    {
        public string AMS { get; set; }
        public string OlumSakatlik { get; set; }
        public bool Tedavi { get; set; }
        public string TedaviTeminat { get; set; }

        public bool Deprem { get; set; }
        public string DepremMuafiyetKodu { get; set; }
        public bool Yutr_Disi_Teminat { get; set; }
        public string Yurt_Disi_Teminati_Sure { get; set; }
        public string Yurt_Disi_Teminati_Ulke { get; set; }
        public bool Hukuksal_Koruma_Teminati { get; set; }
        public bool Eskime_Payi_Teminati { get; set; }
        public bool Anahtar_Kaybi_Teminati { get; set; }
        public bool Ozel_Esya_Teminati { get; set; }
        public bool Anahtarla_Calinma_Teminati { get; set; }
        public bool FiloPolice_Teminati { get; set; }
        public bool BelediyeHalkOtobusu { get; set; }

        public string IkameTuru { get; set; }
        public bool Kullanici_Teminat { get; set; }
        public string Kullanici_TCKN { get; set; }
        public string Kullanici_Adi { get; set; }

        public string OnarimYeri { get; set; }

        public bool Aksesuar_Teminati { get; set; }
        public List<MapfreAksesuarModel> Aksesuarlar { get; set; }
        public bool ElektronikCihaz_Teminati { get; set; }
        public List<MapfreAksesuarModel> Cihazlar { get; set; }
        public bool TasinanYuk_Teminati { get; set; }
        public List<MapfreTasinanYukModel> TasinanYukler { get; set; }
    }

    public class KaskoIndirimSurprimModel
    {
        public int IndirimTipi { get; set; }
        public int? IndirimOrani { get; set; }

        public List<SelectListItem> IndirimTipleri { get; set; }
    }

    public class DetayMapfreIndirimSurprimModel
    {
        public int IndirimTipi { get; set; }
        public string IndirimTipAdi { get; set; }
        public int IndirimOrani { get; set; }
    }

    public class MapfreKaskoAracBilgiModel
    {
        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PlakaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string PlakaNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullanimSekliKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KullanimTarziKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MarkaKodu { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int Model { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TipKodu { get; set; }

        public DateTime? TrafikTescilTarihi { get; set; }
        public DateTime? TrafigeCikisTarihi { get; set; }
        public string TescilIl { get; set; }
        public string TescilIlce { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public DateTime PoliceBaslangicTarihi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public int KisiSayisi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string AracDeger { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string MotorNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SaseNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilBelgeSeriKod { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string TescilBelgeSeriNo { get; set; }
        public string AsbisNo { get; set; }

        public string HasarsizlikIndirim { get; set; }
        public string HasarSurprim { get; set; }
        public string UygulananKademe { get; set; }

        public List<SelectListItem> SigortaSirketleri { get; set; }
        public List<SelectListItem> PlakaKoduListe { get; set; }

        public List<SelectListItem> KisiSayisiListe { get; set; }
        public List<SelectListItem> KullanimTarzlari { get; set; }
        public List<SelectListItem> KullanimSekilleri { get; set; }
        public List<SelectListItem> Markalar { get; set; }
        public List<SelectListItem> AracTipleri { get; set; }
        public List<SelectListItem> Modeller { get; set; }
        public List<SelectListItem> TescilIller { get; set; }
        public List<SelectListItem> TescilIlceler { get; set; }
    }

    public class MapfreOdemeAlModel
    {
        public MapfreOdemeModel KrediKarti { get; set; }
    }

    public class MapfreOdemeModel
    {
        public int KK_TeklifId { get; set; }

        public decimal? Tutar { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KartSahibi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string KartNumarasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string GuvenlikNumarasi { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SonKullanmaAy { get; set; }

        [Required(ErrorMessageResourceType = typeof(babonline), ErrorMessageResourceName = "Message_Required")]
        public string SonKullanmaYil { get; set; }

        public List<SelectListItem> Aylar { get; set; }
        public List<SelectListItem> Yillar { get; set; }

        public byte KK_OdemeSekli { get; set; }
        public byte KK_OdemeTipi { get; set; }
        public byte TaksitSayisi { get; set; }
        public List<SelectListItem> TaksitSayilari { get; set; }
        public List<SelectListItem> OdemeTipleri { get; set; }
        public List<SelectListItem> OdemeSekilleri { get; set; }
    }

    public class MapfreTeklifDurumModel
    {
        public int id { get; set; }
        public string teklifNo { get; set; }
        public string mapfreTeklifNo { get; set; }
        public bool tamamlandi { get; set; }
        public int teklifId { get; set; }
        public string pdf { get; set; }
        public string mesaj { get; set; }
        public bool hata { get; set; }
        public int SigortaEttirenKodu { get; set; }
        public MapfreTeklifFiyatDetayModel teklif { get; set; }
    }

    public class MapfreTeklifFiyatDetayModel : TeklifFiyatDetayModel
    {
        public MapfreTeklifFiyatDetayModel()
        {
            this.Otorizasyon = false;
        }

        public string PDFDosyasi { get; set; }
        public bool Otorizasyon { get; set; }
        public string OtorizasyonMesaj { get; set; }
        public List<string> OtorizasyonMesajlari { get; set; }
        public string MapfreTeklifNo { get; set; }
    }

    public class MapfreOtorizasyonModel
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string TeklifNo { get; set; }
        public string MapfreTeklifNo { get; set; }
    }
}