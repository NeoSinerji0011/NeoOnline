using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.TaliAcenteTransfer;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Web.Controllers
{
    public class HizliTeklifController : Controller
    {
        //
        // GET: /HizliTeklif/
        ITVMService _TVMService;
        ITaliAcenteTransferService _TaliAcenteTransferService;
        IYetkiService _YetkiService;
        IMenuService _MenuService;
        public HizliTeklifController(ITVMService tvmService, ITaliAcenteTransferService taliAcenteTransferService, IYetkiService yetkiService, IMenuService menuService)
        {
            _TVMService = tvmService;
            _TaliAcenteTransferService = taliAcenteTransferService;
            _YetkiService = yetkiService;
            _MenuService = menuService;
        }

        public ActionResult Login()
        {
            HizliTeklifModel model = new HizliTeklifModel();
            model.TVMKodu = 100;
            model.TVMUnvani = "NEOSİNERJİ";

            List<TVMDetay> tvmListesi = _TVMService.GetListTVMDetay().Where(s => s.BagliOlduguTVMKodu == -9999).ToList<TVMDetay>();
            model.TVMList = new SelectList(tvmListesi, "Kodu", "Unvani", "100").ListWithOptionLabel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Kaydet(HizliTeklifModel model)
        {
            #region TVM Deneme detay

            TVMParametreModel parametreModel = new TVMParametreModel();
            parametreModel.tvmDetay = _TaliAcenteTransferService.GetSonTvm(model.TVMKodu);

            int altTvmKoduSayac = parametreModel.tvmDetay.Kodu;
            do
            {
                altTvmKoduSayac++;
            }
            while (_TaliAcenteTransferService.TVMKoduVarMi(altTvmKoduSayac));

            TVMDetay tvmDetay = new TVMDetay();
            tvmDetay.BagliOlduguTVMKodu = model.TVMKodu;
            tvmDetay.Kodu = altTvmKoduSayac;
            tvmDetay.Unvani = model.TVMUnvani + "" + model.adi + "" + model.soyadi;
            tvmDetay.Tipi = TVMTipleri.Acente;
            tvmDetay.KayitNo = "-99999";
            tvmDetay.VergiDairesi = "";
            tvmDetay.VergiDairesi = "-999999999";
            tvmDetay.Profili = TVMProfilleri.Sube;
            tvmDetay.AcentSuvbeVar = 1;
            tvmDetay.Durum = TVMDurumlari.Aktif;
            tvmDetay.SozlesmeBaslamaTarihi = DateTime.Now;
            tvmDetay.SozlesmeDondurmaTarihi = DateTime.Now.AddYears(10);
            tvmDetay.Email = model.email;
            tvmDetay.Telefon = "90-212-9999999";
            tvmDetay.Fax = "90-212-9999999";
            tvmDetay.Logo = parametreModel.tvmDetay.Logo;
            tvmDetay.UlkeKodu = parametreModel.tvmDetay.UlkeKodu;
            tvmDetay.IlKodu = parametreModel.tvmDetay.IlKodu;
            tvmDetay.IlceKodu = parametreModel.tvmDetay.IlceKodu;
            tvmDetay.Adres = parametreModel.tvmDetay.Adres;
            tvmDetay.BolgeKodu = parametreModel.tvmDetay.BolgeKodu;
            tvmDetay.SifreKontralSayisi = parametreModel.tvmDetay.SifreKontralSayisi;
            tvmDetay.SifreDegistirmeGunu = parametreModel.tvmDetay.SifreDegistirmeGunu;
            tvmDetay.SifreIkazGunu = parametreModel.tvmDetay.SifreIkazGunu;
            tvmDetay.BaglantiSiniri = parametreModel.tvmDetay.BaglantiSiniri;
            tvmDetay.MuhasebeEntegrasyon = parametreModel.tvmDetay.MuhasebeEntegrasyon;
            tvmDetay.ProjeKodu = parametreModel.tvmDetay.ProjeKodu;

            #endregion

            #region TVM Yetki Gruplari Add

            TVMYetkiGruplari yetkiGrup = new TVMYetkiGruplari();
            yetkiGrup.TVMKodu = tvmDetay.Kodu;
            yetkiGrup.YetkiGrupAdi = "Sigortali Web";
            yetkiGrup.YetkiSeviyesi = false;
            tvmDetay.TVMYetkiGruplaris.Add(yetkiGrup);

            #endregion

            #region TVM Urun Yetkileri Add

            TVMUrunYetkileri urunYetki;
            foreach (var urunYetkileri in parametreModel.tvmDetay.TVMUrunYetkileris)
            {
                urunYetki = new TVMUrunYetkileri();
                if (urunYetki.BABOnlineUrunKodu == UrunKodlari.TrafikSigortasi || urunYetki.BABOnlineUrunKodu == UrunKodlari.KaskoSigortasi)
                {
                    urunYetki.AcikHesapTahsilatGercek = 1;
                    urunYetki.AcikHesapTahsilatTuzel = 1;
                    urunYetki.BABOnlineUrunKodu = urunYetkileri.BABOnlineUrunKodu;
                    urunYetki.HavaleEntegrasyon = 1;
                    urunYetki.KrediKartiTahsilat = 1;
                    urunYetki.ManuelHavale = 1;
                    urunYetki.Police = 1;
                    urunYetki.Rapor = 1;
                    urunYetki.Teklif = 1;
                    urunYetki.TUMKodu = urunYetkileri.TUMKodu;
                    urunYetki.TUMUrunKodu = urunYetkileri.TUMUrunKodu;
                    urunYetki.TVMKodu = altTvmKoduSayac;
                    tvmDetay.TVMUrunYetkileris.Add(urunYetki);
                }
            }

            #endregion


            bool kayit = _TVMService.AddTaliTVM(tvmDetay);
            if (kayit == false)
            {
                ViewBag.mesaj = "Kayit Eklenemedi Lütfen Bilgilerinizi Kontrol Ediniz.";
                return View(model);
            }

            int yetkiGrupKodu = _TVMService.GetYetkiGrupKodu(altTvmKoduSayac);

            List<TVMYetkiGrupYetkileri> TVMYetkiList = new List<TVMYetkiGrupYetkileri>();
            TVMYetkiGrupYetkileri yetkiGrupYetki = new TVMYetkiGrupYetkileri();
            List<AnaMenu> tumAnaMenuler = _MenuService.GetAnaMenuList();
            foreach (var item in tumAnaMenuler)
            {
                if (item.AnaMenuKodu == AnaMenuler.Teklif || item.AnaMenuKodu == AnaMenuler.Police)
                {
                    yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                    yetkiGrupYetki.YetkiGrupKodu = yetkiGrupKodu;
                    yetkiGrupYetki.AnaMenuKodu = item.AnaMenuKodu;
                    yetkiGrupYetki.AltMenuKodu = 0;
                    yetkiGrupYetki.SekmeKodu = 0;

                    yetkiGrupYetki.Gorme = 0;
                    yetkiGrupYetki.YeniKayit = 0;
                    yetkiGrupYetki.Degistirme = 0;
                    yetkiGrupYetki.Silme = 0;
                    yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);
                }


            }

            //TUM Alt menüler ekleniyor yetkileri 0 olarak veriliyor.
            List<AltMenu> tumAltMenuler = _MenuService.GetAltMenuList();
            foreach (var item in tumAltMenuler)
            {
                yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                yetkiGrupYetki.YetkiGrupKodu = yetkiGrupKodu;
                yetkiGrupYetki.AnaMenuKodu = item.AnaMenuKodu;
                yetkiGrupYetki.AltMenuKodu = item.AltMenuKodu;
                yetkiGrupYetki.SekmeKodu = 0;

                yetkiGrupYetki.Gorme = 0;
                yetkiGrupYetki.YeniKayit = 0;
                yetkiGrupYetki.Degistirme = 0;
                yetkiGrupYetki.Silme = 0;
                yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);
            }

            //TUM Sekmeler ekleniyor yetkileri 0 olarak veriliyor.
            List<AltMenuSekme> tumSekmeler = _MenuService.GetALtMenuSekmeList();
            foreach (var item in tumSekmeler)
            {
                yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                yetkiGrupYetki.YetkiGrupKodu = yetkiGrupKodu;
                yetkiGrupYetki.AnaMenuKodu = item.AnaMenuKodu;
                yetkiGrupYetki.AltMenuKodu = item.AltMenuKodu;
                yetkiGrupYetki.SekmeKodu = item.SekmeKodu;

                yetkiGrupYetki.Gorme = 0;
                yetkiGrupYetki.YeniKayit = 0;
                yetkiGrupYetki.Degistirme = 0;
                yetkiGrupYetki.Silme = 0;
                yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);
            }

            #region servisKullanıcıları Ekle
            List<TVMWebServisKullanicilari> tvmWebServisKullanicilari = new List<TVMWebServisKullanicilari>();
            tvmWebServisKullanicilari = _TaliAcenteTransferService.GetListTVMWebServisKullanicilari(model.TVMKodu);
            foreach (var servisKullanici in tvmWebServisKullanicilari)
            {
                TVMWebServisKullanicilari webservisKullancilari = new TVMWebServisKullanicilari();
                webservisKullancilari.CompanyId = servisKullanici.CompanyId;
                webservisKullancilari.KullaniciAdi = servisKullanici.KullaniciAdi;
                webservisKullancilari.KullaniciAdi2 = servisKullanici.KullaniciAdi2;
                webservisKullancilari.PartajNo_ = servisKullanici.PartajNo_;
                webservisKullancilari.Sifre = servisKullanici.Sifre;
                webservisKullancilari.Sifre2 = servisKullanici.Sifre2;
                webservisKullancilari.SourceId = servisKullanici.SourceId;
                webservisKullancilari.SubAgencyCode = servisKullanici.SubAgencyCode;
                webservisKullancilari.TUMKodu = servisKullanici.TUMKodu;
                webservisKullancilari.TVMKodu = altTvmKoduSayac;
                webservisKullancilari.Sifre = servisKullanici.Sifre;

                tvmWebServisKullanicilari.Add(webservisKullancilari);
            }
            _TaliAcenteTransferService.taliWebServisKullaniciAdd(tvmWebServisKullanicilari);

            #endregion


            return View();
        }
    }
}
