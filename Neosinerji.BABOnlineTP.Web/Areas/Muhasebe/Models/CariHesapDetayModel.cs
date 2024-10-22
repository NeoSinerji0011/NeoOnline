using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Models
{
    public class CariHesapDetayModel
    {
        public int Id { get; set; }
        public int TVMKodu { get; set; }
        public string TVMUnvani { get; set; }
        public string CariHesapKodu { get; set; }
        public string CariHesapTipi { get; set; }
        public string CariHesapTipiAdi { get; set; }
        public string SatisIadeleriMuhasebeKodu { get; set; }
        public SelectList CariHesapTipleri { get; set; }
        public string MusteriGrupKodu { get; set; }
        public string KimlikNo { get; set; }
        public string Unvan { get; set; }
        public string VergiDairesi { get; set; }
        public string Telefon1 { get; set; }
        public string Telefon2 { get; set; }
        public string CepTel { get; set; }
        public string Email { get; set; }
        public string DisaktarimMuhasebeKodu { get; set; }
        public string DisaktarimCariKodu { get; set; }
        public string KomisyonGelirleriMuhasebeKodu { get; set; }
        public string WebAdresi { get; set; }
        public string UlkeKodu { get; set; }
        public string IlKodu { get; set; }
        public int IlceKodu { get; set; }
        public List<SelectListItem> Ulkeler { get; set; }
        public List<SelectListItem> Iller { get; set; }
        public List<SelectListItem> IlceLer { get; set; }
        public string UlkeAdi { get; set; }
        public string IlAdi { get; set; }
        public string IlceAdi { get; set; }
        public int PostaKodu { get; set; }
        public string Adres { get; set; }
        public string UyariNotu { get; set; }
        public string BilgiNotu { get; set; }
        public DateTime KayitTarihi { get; set; }
        public DateTime GuncellemeTarihi { get; set; }
    }
}