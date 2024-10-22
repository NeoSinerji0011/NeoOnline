using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.KORU.LilyumFerdiKaza;
using Neosinerji.BABOnlineTP.Database.Models;
using teklifMus = Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Bireysel.Controllers
{
    public class SatinAlController : Controller
    {
        public ITVMService _TVMService;
        public ITeklifService _TeklifService;
        public IMusteriService _MusteriService;
        public ITanimService _TanimService;
        public IKullaniciService _KullaniciService;
        public IAktifKullaniciService _AktifKullaniciService;
        public IUlkeService _UlkeService;
        public ICRService _CRService;
        public IUrunService _UrunService;
        public IAracService _AracService;
        public ITUMService _TUMService;
        public ILogService _LogService;
        public IPoliceToXML _PoliceToXML;
        IFormsAuthenticationService _FormsAuthenticationService;
        public SatinAlController(ITVMService tvmService,
                               ITeklifService teklifService,
                               IMusteriService musteriService,
                               IKullaniciService kullaniciService,
                               IAktifKullaniciService aktifKullaniciService,
                               ITanimService tanimService,
                               IUlkeService ulkeService,
                               ICRService crService,
                               IAracService aracService,
                               IUrunService urunService,
                               ITUMService tumService,
                              IFormsAuthenticationService formsAuthenticationService)
        {
            _FormsAuthenticationService = formsAuthenticationService;
            _TVMService = tvmService;
            _TeklifService = teklifService;
            _MusteriService = musteriService;
            _TanimService = tanimService;
            _KullaniciService = kullaniciService;
            _AktifKullaniciService = aktifKullaniciService;
            _UlkeService = ulkeService;
            _CRService = crService;
            _UrunService = urunService;
            _AracService = aracService;
            _TUMService = tumService;
        }

        // GET: Bireysel/LilyumKart
        [HttpGet]
        public ActionResult LilyumKart(int? tvmkodu)
        {
            try
            {
                ViewBag.Logo = _TVMService.GetDetay(Convert.ToInt32(tvmkodu)).Logo;
            }
            catch (Exception)
            {
            }
            List<MeslekIndirimiKasko> MeslekListesi = new List<MeslekIndirimiKasko>();
            if (tvmkodu != null)
            {
                ViewBag.AnaAcenteMi = tvmkodu;
                ViewBag.checkTvm = true;
                _AktifKullaniciService.SetUser(tvmkodu);
                _FormsAuthenticationService.SignIn(_AktifKullaniciService.Email, false);
            }
            else
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }
            if (_AktifKullaniciService.YetkiGrubu == TvmYetkiKodlari.LilyumYoneticiYetkiKodu)
            {
                ViewBag.TutarYetki = true;
            }
            else
            {
                ViewBag.TutarYetki = false;
            }

            teklifMus.LilyumFerdiKazaModel models = EkleModel(null, null);
            MeslekListesi = _TeklifService.GetMeslekList();
            if (MeslekListesi != null)
            {
                models.SigortaliMeslekListesi = new SelectList(MeslekListesi, "MeslekKodu", "Aciklama", "").ListWithOptionLabel();
            }
            return View(models);
        }
        public class ListModels
        {
            public string key { get; set; }
            public string value { get; set; }
        }
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]
        public teklifMus.LilyumFerdiKazaModel EkleModel(int? id, int? teklifId)
        {
            ViewBag.AnaAcenteMi = _AktifKullaniciService.BagliOlduguTvmKodu;

            teklifMus.LilyumFerdiKazaModel model = new teklifMus.LilyumFerdiKazaModel();
            ListModels meslek = new ListModels();
            List<ListModels> meslekList = new List<ListModels>();
            IKoruFerdiKaza request = DependencyResolver.Current.GetService<IKoruFerdiKaza>();

            model.GenelBilgiler = new teklifMus.LilyumGenelBilgilerModel();
            model.UrunAdi = "LilyumKart";
            model.GenelBilgiler.PlakaIlKodu = "34";
            model.GenelBilgiler.PlakaNo = "";
            model.GenelBilgiler.MotorsikletKullaniyorMu = false;
            model.GenelBilgiler.SporlaUgrasiyorMu = false;
            //Teklifi hazırlayan
            teklifMus.HazirlayanModel hazirlayan = new teklifMus.HazirlayanModel();
            hazirlayan.KendiAdima = 1;
            hazirlayan.KendiAdimaList = new SelectList(teklifMus.TeklifListeleri.TeklifHazirlayanTipleri(), "Value", "Text", hazirlayan.KendiAdima);
            hazirlayan.TVMKodu = _AktifKullaniciService.TVMKodu;
            hazirlayan.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
            hazirlayan.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            hazirlayan.TVMKullaniciAdi = _AktifKullaniciService.AdiSoyadi;
            hazirlayan.YeniIsMi = false;
            model.Hazirlayan = hazirlayan;

          //Sigorta Ettiren / Sigortalı
          model.Musteri = new teklifMus.SigortaliModel();
            model.Musteri.SigortaliAyni = false;
            model.Musteri.Sigortali = new teklifMus.MusteriModel();
            model.Odeme = new teklifMus.LilyumTeklifOdemeModel();
            model.Odeme.OdemeSekli = true;
            model.Odeme.DigerOdemeTutari = null;
            model.AdresAyniMi = true;

            List<SelectListItem> ulkeler = new List<SelectListItem>();
            ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });
            model.GenelBilgiler.PoliceBaslangicTarihi = TurkeyDateTime.Today;
            model.GenelBilgiler.PoliceBitisTarihi = TurkeyDateTime.Today.AddYears(1);
            model.Odeme.OdemeTipi = OdemeTipleri.KrediKarti;

            #region TUM IMAGES

            model.TeklifUM = new teklifMus.TeklifUMListeModel();
            List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.Lilyum);
            foreach (var item in urunyetkileri)
                model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);
            #endregion
            model.Odeme.OdemeTipleri = new List<SelectListItem>();
            model.Odeme.OdemeTipleri.Add(new SelectListItem() { Text = "Kredi Kartı", Value = "2", Selected = true });
            model.GenelBilgiler.PlakaKoduListe = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlKodu", model.GenelBilgiler.PlakaIlKodu).ListWithOptionLabel();

            List<Il> iller = _UlkeService.GetIlList("TUR").OrderBy(o => o.IlAdi).ToList<Il>();
            List<Ilce> ilceler = _UlkeService.GetIlceList("TUR", "34").OrderBy(o => o.IlceAdi).ToList<Ilce>();

            model.TeslimatIller = new SelectList(iller, "IlKodu", "IlAdi", "34").ListWithOptionLabel();
            model.TeslimatIlceler = new SelectList(ilceler, "IlceKodu", "IlceAdi", "").ListWithOptionLabel();
            model.IletisimIller = new SelectList(iller, "IlKodu", "IlAdi", "34").ListWithOptionLabel();
            model.IletisimIlceler = new SelectList(ilceler, "IlceKodu", "IlceAdi", "").ListWithOptionLabel();
            return model;
        }
        public ActionResult Bilgilendirme(Boolean odemeDurum)
        {
            teklifMus.LilyumKartBilgilendirme model = new teklifMus.LilyumKartBilgilendirme();
            model.odemeDurumu = odemeDurum.ToString();
            return View(model);
        }
    }
}