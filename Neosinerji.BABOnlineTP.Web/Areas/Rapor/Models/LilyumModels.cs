using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models
{
    public class LilyumKartKullanimModel
    {
        public MultiSelectList TVMLerItems { get; set; }
        public List<LilyumItemModel> list { get; set; }
        public string ReferansNo { get; set; }
        public string BrutPrim { get; set; }
        public byte? TaksitSayisi { get; set; }
        public bool? iptal { get; set; }
        public string LilyumKartNo { get; set; }
        public string MusteriAdiSoyadi { get; set; }
        public string KartLogo { get; set; }
        public string KonutAdres { get; set; }
        public string KonutIlIlce { get; set; }
        public string Kart { get; set; }
        public string KimlikNo { get; set; }
        public List<SelectListItem> Kartlar { get; set; }

        public bool InternetAcentesiMi { get; set; }
        public int KullaniciKodu { get; set; }
        public List<SelectListItem> Kullanicilar { get; set; }

        public string[] TVMLerSelectList { get; set; }
        public string kullaniciAdiSoyadi { get; set; }
        public string adsoyad { get; set; }
    }
    public class KartKullanimGuncelleModel
    {
        public int TvmKodu { get; set; }
        public int KullaniciKodu { get; set; }
        public string ReferansNo { get; set; }
        public string LilyumKartNo { get; set; }
        public List<SelectListItem> TVMler { get; set; }
        public List<SelectListItem> Kullanicilar { get; set; }
        public List<SelectListItem> Referanslar { get; set; }
        public List<LilyumItemModel> list { get; set; }
        public string kullaniciAdiSoyadi { get; set; }
        public string adsoyad { get; set; }
    }

    public class KartKullanimGuncelleArrayModel
    {
        public int teminatId { get; set; }
        public byte teminatKullanimAdet { get; set; }
        public string teminatSonKullanilanTarihi { get; set; }
        public string lilyumKartNo { get; set; }
        public string lilyumReferansNo { get; set; }

    }
    public class LilyumItemModel
    {
        public int TeminatId { get; set; }
        public string GrupAdi { get; set; }
        public string TeminatAdi { get; set; }
        public string TeminatAciklama { get; set; }
        public int KullanimHakkiAdet { get; set; }
        public int ToplamKullanimAdet { get; set; }
        public int KalanKullanimAdet { get; set; }
        public string TeminatSonKullanilanTarih { get; set; }

        public string LilyumKartNo { get; set; }
        public string LilyumReferansNo { get; set; }
        public int kullaniciKodu { get; set; }
    }
}