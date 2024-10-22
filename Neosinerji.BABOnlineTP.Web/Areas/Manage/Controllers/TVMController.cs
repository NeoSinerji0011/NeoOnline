using AutoMapper;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System.Text.RegularExpressions;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.TVMYonetimi, SekmeKodu = AltMenuSekmeler.TeklifVermeMerkeziTVM)]
    [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
    public class TVMController : Controller
    {
        ITVMService _TVMService;
        ISigortaSirketleriService _SigortaSirketleriService;
        ITVMDokumanStorage _Storage;
        IUlkeService _UlkeService;
        IUrunService _UrunService;
        ITUMService _TUMService;
        IYetkiService _YetkiService;
        IKullaniciFotografStorage _LogoStorage;
        IMuhasebe_CariHesapService _Muhasebe_CariHesapService;
        ILogService _Log;
        IAktifKullaniciService _AktifKullanici;
        IBankaSubeleriService _BankaSubeleri;
        IKomisyonContext _KomisyonContext;
        INeoConnectService _NeoConnectService;
        IKullaniciService _KullaniciService;
        IBransService _BransService;

        #region TVM Genel Metodlar

        public TVMController(ITVMService tvmService,
                             ISigortaSirketleriService sigortaSirketleriService,
                             ITVMDokumanStorage storage,
                             IUlkeService ulkeService,
                             IMuhasebe_CariHesapService muhasebe_CariHesapService,
                             IYetkiService yetkiService,
                             ITUMService tumService,
                             IKomisyonContext komisyonContext,
                             IUrunService urunService,
                             IKullaniciFotografStorage logoStorage, INeoConnectService neoConnectService,
                             IBransService bransService,
                             IKullaniciService kullaniciService)
        {
            _TVMService = tvmService;
            _SigortaSirketleriService = sigortaSirketleriService;
            _Storage = storage;
            _UlkeService = ulkeService;
            _UrunService = urunService;
            _TUMService = tumService;
            _KomisyonContext = komisyonContext;
            _YetkiService = yetkiService;
            _Muhasebe_CariHesapService = muhasebe_CariHesapService;
            _LogoStorage = logoStorage;
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _BankaSubeleri = DependencyResolver.Current.GetService<IBankaSubeleriService>();
            _NeoConnectService = neoConnectService;
            _KullaniciService = kullaniciService;
            _BransService = bransService;
        }


        public ActionResult Liste()
        {
            try
            {
                TVMListeModel model = new TVMListeModel();
                model.Kodu = _AktifKullanici.TVMKodu;
                model.TVMTipleri = new SelectList(TVMListProvider.TVMTipleriFull(), "Value", "Text", model.Tipi);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [AjaxException]
        public ActionResult ListePager()
        {
            try
            {
                if (Request["sEcho"] != null)
                {
                    TVMListe tvmListe = new TVMListe(Request, new Expression<Func<TVMDetay, object>>[]
                                                {
                                                    t => t.Kodu,
                                                    t => t.Unvani
                                                },
                                                    t => t.Kodu,
                                                    t => t.Unvani,
                                                    "/Manage/TVM/Detay/",
                                                    "/Manage/TVM/Guncelle/");

                    tvmListe.Kodu = tvmListe.TryParseParamInt("Kodu");
                    tvmListe.BagliOlduguTVMKodu = tvmListe.TryParseParamInt("BagliOlduguTVMKodu");
                    tvmListe.Unvani = tvmListe.TryParseParamString("Unvani");
                    tvmListe.Tipi = tvmListe.TryParseParamShort("Tipi");

                    DataTableList result = _TVMService.PagedList(tvmListe);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }


        public ActionResult Detay(int id)
        {
            try
            {
                TVMDetayModel model = new TVMDetayModel();
                TVMDetay tvm = _TVMService.GetDetayYetkili(id);

                if (tvm != null && tvm.Kodu > 0)
                {
                    Mapper.CreateMap<TVMDetay, TVMDetayModel>();
                    model = Mapper.Map<TVMDetayModel>(tvm);

                    model.TipiText = TVMListProvider.GetTVMTipiText(model.Tipi);
                    if (tvm.BolgeYetkilisiMi == 1)
                    {
                        model.BolgeYetkilisiMiText = "Evet";
                    }
                    else
                    {
                        model.BolgeYetkilisiMiText = "Hayır";
                    }
                    if (tvm.PoliceTransfer == 1)
                    {
                        model.PoliceTransferiYapilacakmiText = "Evet";
                    }
                    else
                    {
                        model.PoliceTransferiYapilacakmiText = "Hayır";
                    }
                    model.AcentSuvbeVarText = TVMListProvider.GetVarYok(model.AcentSuvbeVar);
                    model.BaglantiSiniriText = TVMListProvider.GetVarYok(tvm.BaglantiSiniri);
                    model.SonPoliceOnayTarihi = tvm.SonPoliceOnayTarihi;
                    model.ProjeKodu = tvm.ProjeKodu;
                    if (tvm.MuhasebeEntegrasyon.HasValue)
                        model.MuhasebeEntegrasyonVarText = tvm.MuhasebeEntegrasyon.Value ? babonline.Exists : babonline.Absent;

                    Ulke ulke = _UlkeService.GetUlke(tvm.UlkeKodu);
                    if (ulke != null)
                        model.UlkeAdi = ulke.UlkeAdi;

                    Il il = _UlkeService.GetIl(tvm.UlkeKodu, tvm.IlKodu);
                    if (il != null)
                        model.IlAdi = il.IlAdi;

                    if (tvm.IlceKodu != null)
                    {
                        Ilce ilce = _UlkeService.GetIlce(tvm.IlceKodu.Value);
                        model.IlceAdi = (ilce == null ? string.Empty : ilce.IlceAdi);
                    }
                    else
                    {
                        model.IlceAdi = string.Empty;
                    }

                    TVMDetay anaTVM = _TVMService.GetDetay(model.BagliOlduguTVMKodu);
                    if (anaTVM != null)
                        model.BagliOlduguTVMAdi = anaTVM.Unvani;

                    if (model.GrupKodu.HasValue)
                    {
                        TVMDetay detay = _TVMService.GetDetay(model.GrupKodu.Value);
                        model.BolgeYetkilisiUnvani = detay.Unvani;
                    }

                    model.IPBaglantilariList = BaglantiList(id);
                    model.BolgeleriList = BolgeList(id);
                    model.DepartmanlarList = DepartmanList(id);
                    model.NotlarList = NotlarList(id);
                    model.DokumanlariList = DokumanList(id);

                    TVMLogoModel logomodel = new TVMLogoModel();
                    logomodel.Kodu = tvm.Kodu;
                    logomodel.Alt = tvm.Unvani;
                    logomodel.Src = tvm.Logo;
                    model.LogoModel = logomodel;
                }
                else return new RedirectResult("~/Error/ErrorPage/403");

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.TVMYonetimi,
                       SekmeKodu = AltMenuSekmeler.TeklifVermeMerkeziTVM,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle()
        {
            try
            {
                TVMEkleModel model = new TVMEkleModel();

                if (_AktifKullanici.TVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu)
                {
                    model.Profili = TVMProfilleri.Merkez;
                    model.BagliOlduguTVMKodu = _AktifKullanici.BagliOlduguTvmKodu;
                }
                else
                {
                    model.Profili = TVMProfilleri.Sube;
                    model.BagliOlduguTVMKodu = _AktifKullanici.TVMKodu;
                }

                if (model.BagliOlduguTVMKodu != -9999)
                {
                    var tvmDetay = _TVMService.GetDetay(model.BagliOlduguTVMKodu.Value);
                    model.TVMUnvani = tvmDetay.Unvani;
                }
                model.MerkezTipi = TVMTipleri.Yok;
                model.SubeAcenteTipi = TVMTipleri.Yok;
                model.Durum = TVMDurumlari.Aktif;

                model.SozlesmeBaslamaTarihi = TurkeyDateTime.Today;

                model.TCKN = "XXXXXXXXXXX";
                model.UlkeKodu = "TUR";
                model.IlKodu = "34";
                model.SifreKontralSayisi = 6;
                model.SifreDegistirmeGunu = 90;
                model.SifreIkazGunu = 10;
                model.BaglantiSiniri = TVMBaglantiSiniri.Yok;
                model.AcentSuvbeVar = TVMAcenteSubeVar.Yok;
                model.MuhasebeEntegrasyonu = 0;
                model.ProjeKodu = _AktifKullanici.ProjeKodu;

                model.PoliceTransferiYapilacakMi = 0;

                model.Kodu = _TVMService.GetMerkezAcenteSonSatisKanaliKodu(model.BagliOlduguTVMKodu.Value);

                model.TVMMerkezTipleri = new SelectList(TVMListProvider.TVMTipleri(), "Value", "Text", model.MerkezTipi);
                model.TVMSubeAcenteTipleri = new SelectList(TVMListProvider.TVMTipleriSubeAcente(), "Value", "Text", model.SubeAcenteTipi);
                model.ProfilTipleri = new SelectList(TVMListProvider.ProfilTipleri(), "Value", "Text", model.Profili);
                model.BolgeYetkiliTipleri = new SelectList(TVMListProvider.BolgeYetkiliTipleri(), "Value", "Text", "0");
                model.AcenteSubeVarTipleri = new SelectList(TVMListProvider.VarYokTipleri(), "Value", "Text", model.AcentSuvbeVar);
                model.BaglantiSiniriVarYok = new SelectList(TVMListProvider.VarYokTipleri(), "Value", "Text", model.BaglantiSiniri);
                model.ProjeTipleri = new SelectList(TVMListProvider.ProjeTipleri(), "Value", "Text", model.ProjeKodu);
                model.PoliceTransferAcentesi = new SelectList(TVMListProvider.PoliceTransferAcentesi(), "Value", "Text", model.PoliceTransferiYapilacakMi);
                List<TVMDetay> tvmlist = _TVMService.GetListBolgeYetkilisi();
                model.BolgeYetkilileri = new SelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel();

                List<Ulke> ulkeler = _UlkeService.GetUlkeList();
                List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu);
                List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu);
                model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
                model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
                model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", "0").ListWithOptionLabel();
                model.Bolgeler = new List<SelectListItem>();

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.TVMYonetimi,
                       SekmeKodu = AltMenuSekmeler.TeklifVermeMerkeziTVM,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle(TVMEkleModel model)
        {
            try
            {
                if (ModelState["MerkezTipi"] != null)
                    ModelState["MerkezTipi"].Errors.Clear();

                if (ModelState["SubeAcenteTipi"] != null)
                    ModelState["SubeAcenteTipi"].Errors.Clear();

                if (ModelState["BagliOlduguTVMKodu"] != null)
                    ModelState["BagliOlduguTVMKodu"].Errors.Clear();

                if (ModelState.IsValid)
                {
                    TVMDetay tvm = new TVMDetay();

                    tvm.AcentSuvbeVar = model.AcentSuvbeVar;
                    tvm.Adres = model.Adres;
                    tvm.BaglantiSiniri = model.BaglantiSiniri;
                    tvm.BagliOlduguTVMKodu = model.BagliOlduguTVMKodu ?? -9999;
                    if (model.bolgeYetkilisi.HasValue)
                    {
                        tvm.GrupKodu = model.bolgeYetkilisi;
                    }
                    tvm.MuhasebeEntegrasyon = model.MuhasebeEntegrasyonu == 1 ? true : false;
                    tvm.BolgeKodu = model.BolgeKodu ?? 0;
                    tvm.Email = model.Email;
                    tvm.Fax = model.Fax;
                    tvm.IlKodu = model.IlKodu;
                    tvm.IlceKodu = model.IlceKodu;
                    tvm.KayitNo = model.KayitNo;
                    //1 evet 0 hayır
                    tvm.BolgeYetkilisiMi = model.BolgeYetkilisiMi;
                    //1 evet 0 hayır
                    tvm.PoliceTransfer = model.PoliceTransferiYapilacakMi;
                    tvm.SonPoliceOnayTarihi = model.SonPoliceOnayTarihi;
                    string tvmkodu = String.Empty;
                    if (model.Kodu.HasValue)
                    {
                        tvm.Kodu = model.Kodu.Value;
                    }

                    tvm.Profili = model.Profili;
                    tvm.Semt = model.Semt;
                    tvm.UlkeKodu = model.UlkeKodu;

                    if (model.SifreDegistirmeGunu.HasValue)
                        tvm.SifreDegistirmeGunu = model.SifreDegistirmeGunu.Value;

                    if (model.SifreIkazGunu.HasValue)
                        tvm.SifreIkazGunu = model.SifreIkazGunu.Value;

                    if (model.SifreKontralSayisi.HasValue)
                        tvm.SifreKontralSayisi = model.SifreKontralSayisi.Value;

                    tvm.SozlesmeBaslamaTarihi = model.SozlesmeBaslamaTarihi;
                    tvm.SozlesmeDondurmaTarihi = model.SozlesmeDondurmaTarihi;

                    tvm.Telefon = model.Telefon;
                    tvm.Tipi = model.MerkezTipi > 0 ? model.MerkezTipi : model.SubeAcenteTipi;
                    tvm.Unvani = model.Unvani;
                    tvm.VergiDairesi = model.VergiDairesi;
                    tvm.VergiNumarasi = model.VergiNumarasi;
                    tvm.WebAdresi = model.WebAdresi;
                    tvm.Durum = TVMDurumlari.Aktif;
                    tvm.ProjeKodu = model.ProjeKodu;

                    tvm.TCKN = String.Empty;

                    if (tvm.Profili == 0)
                    {
                        tvm.AcentSuvbeVar = 0;
                    }

                    tvm = _TVMService.CreateDetay(tvm);

                    return RedirectToAction("Detay", "TVM", new { id = tvm.Kodu });
                }

                model.Bolgeler = new List<SelectListItem>();
                model.Durum = TVMDurumlari.Aktif;
                model.TVMMerkezTipleri = new SelectList(TVMListProvider.TVMTipleri(), "Value", "Text", model.MerkezTipi);
                model.TVMSubeAcenteTipleri = new SelectList(TVMListProvider.TVMTipleri(), "Value", "Text", model.SubeAcenteTipi);
                model.ProfilTipleri = new SelectList(TVMListProvider.ProfilTipleri(), "Value", "Text", model.Profili);
                model.AcenteSubeVarTipleri = new SelectList(TVMListProvider.VarYokTipleri(), "Value", "Text", model.AcentSuvbeVar);
                model.BaglantiSiniriVarYok = new SelectList(TVMListProvider.VarYokTipleri(), "Value", "Text", model.BaglantiSiniri);
                model.BolgeYetkiliTipleri = new SelectList(TVMListProvider.BolgeYetkiliTipleri(), "Value", "Text", "0");
                model.ProjeTipleri = new SelectList(TVMListProvider.ProjeTipleri(), "Value", "Text", model.ProjeKodu);
                model.PoliceTransferAcentesi = new SelectList(TVMListProvider.PoliceTransferAcentesi(), "Value", "Text", model.PoliceTransferiYapilacakMi);
                List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
                model.BolgeYetkilileri = new SelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel();
                List<Ulke> ulkeler = _UlkeService.GetUlkeList().ToList<Ulke>();
                List<Il> iller;
                List<Ilce> ilceler;

                if (String.IsNullOrEmpty(model.UlkeKodu) || model.UlkeKodu == "0")
                    iller = new List<Il>();
                else
                    iller = _UlkeService.GetIlList(model.UlkeKodu).OrderBy(o => o.IlAdi).ToList<Il>();

                if (String.IsNullOrEmpty(model.IlKodu) || model.IlKodu == "0")
                    ilceler = new List<Ilce>();
                else
                    ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu).OrderBy(o => o.IlceAdi).ToList<Ilce>();

                model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
                model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
                model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel(false);

                ModelState.AddModelError("", babonline.Message_TVMSaveError);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.TVMYonetimi,
                       SekmeKodu = AltMenuSekmeler.TeklifVermeMerkeziTVM,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Guncelle(int id)
        {
            try
            {
                TVMGuncelleModel model = new TVMGuncelleModel();
                TVMDetay tvm = _TVMService.GetDetayYetkili(id);

                if (tvm != null)
                {
                    model = ModelDoldur(tvm);
                    return View(model);
                }
                else return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
               AltMenuKodu = AltMenuler.TVMYonetimi,
               SekmeKodu = AltMenuSekmeler.TeklifVermeMerkeziTVM,
               menuPermission = MenuPermission.Guncelleme,
               menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Guncelle(TVMGuncelleModel model)
        {
            try
            {
                TryValidateModel(model);
                if (ModelState["BagliOlduguTVMKodu"] != null)
                    ModelState["BagliOlduguTVMKodu"].Errors.Clear();

                if (ModelState.IsValid && model != null && model.Kodu.HasValue)
                {
                    model.TCKN = (model.TCKN == "XXXXXXXXXXX" ? null : model.TCKN);

                    TVMDetay tvm = _TVMService.GetDetayYetkili(model.Kodu.Value);
                    if (tvm != null)
                    {
                        tvm.Adres = model.Adres;
                        tvm.BaglantiSiniri = model.BaglantiSiniri;

                        //if (model.BagliOlduguTVMKodu.HasValue)
                        //    tvm.BagliOlduguTVMKodu = model.BagliOlduguTVMKodu.Value;

                        if (tvm.Profili == TVMProfilleri.Merkez)
                            tvm.AcentSuvbeVar = model.AcentSuvbeVar;

                        tvm.Durum = model.Durum;
                        tvm.Email = model.Email;
                        tvm.Fax = model.Fax;
                        tvm.IlceKodu = model.IlceKodu;
                        tvm.IlKodu = model.IlKodu;
                        tvm.KayitNo = model.KayitNo;
                        tvm.Semt = model.Semt;
                        tvm.SozlesmeBaslamaTarihi = model.SozlesmeBaslamaTarihi;
                        tvm.SozlesmeDondurmaTarihi = model.SozlesmeDondurmaTarihi;
                        tvm.SonPoliceOnayTarihi = model.SonPoliceOnayTarihi;
                        tvm.TCKN = model.TCKN;
                        tvm.Telefon = model.Telefon;
                        tvm.Tipi = model.MerkezTipi > 0 ? model.MerkezTipi : model.SubeAcenteTipi;
                        tvm.UlkeKodu = model.UlkeKodu;
                        tvm.Unvani = model.Unvani;
                        tvm.VergiDairesi = model.VergiDairesi;
                        tvm.VergiNumarasi = model.VergiNumarasi;
                        tvm.WebAdresi = model.WebAdresi;
                        tvm.MuhasebeEntegrasyon = model.MuhasebeEntegrasyonu == 1 ? true : false;
                        tvm.ProjeKodu = model.ProjeKodu;
                        tvm.GrupKodu = model.bolgeYetkilisi;
                        tvm.BolgeYetkilisiMi = model.BolgeYetkilisiMi;
                        tvm.PoliceTransfer = model.PoliceTransferiYapilacakMi;
                        _TVMService.UpdateDetay(tvm);

                        return RedirectToAction("Detay", "TVM", new { id = tvm.Kodu });
                    }
                    else return new RedirectResult("~/Error/ErrorPage/403");
                }
                else
                {
                    ModelState.AddModelError("", babonline.Message_TVMSaveError);
                    TVMDetay TVM = _TVMService.GetDetayYetkili(model.id);
                    if (TVM != null)
                        model = ModelDoldur(TVM);

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        private TVMGuncelleModel ModelDoldur(TVMDetay tvm)
        {
            TVMGuncelleModel model = new TVMGuncelleModel();

            if (tvm != null)
            {
                Mapper.CreateMap<TVMDetay, TVMGuncelleModel>();
                model = Mapper.Map<TVMDetay, TVMGuncelleModel>(tvm);

                model.Telefon = tvm.Telefon;
                model.Fax = tvm.Fax;

                if (tvm.MuhasebeEntegrasyon.HasValue)
                    model.MuhasebeEntegrasyonu = (byte)(tvm.MuhasebeEntegrasyon.Value ? 1 : 0);

                model.IPBaglantilariList = BaglantiList(tvm.Kodu);
                model.BolgeleriList = BolgeList(tvm.Kodu);
                model.DepartmanlarList = DepartmanList(tvm.Kodu);
                model.NotlarList = NotlarList(tvm.Kodu);
                model.DokumanlariList = DokumanList(tvm.Kodu);
                model.AcentelikleriList = AcenteList(tvm.Kodu);
                model.BankaHesaplariList = BankaHesaplariList(tvm.Kodu);
                model.IletisimYetkilileriList = IletisimYetkilileriList(tvm.Kodu);
                model.WebServisKullanicilari = WebServisKullanicilariList(tvm.Kodu);
                model.NeoConnectKullanicilari = NeoConnectKullanicilariList(tvm.Kodu);
                model.NeoConnectTvmSigortaSirketiKullanicilari = NeoConnectTvmSigortaSirketiKullanicilariList(tvm.Kodu);
                model.NeoConnectYasakliUrlModelsKullanicilari = NeoConnectYasakliUrlKullanicilariList(tvm.Kodu);
                model.ProjeKodu = tvm.ProjeKodu;
                model.Profili = tvm.Profili;
                model.PoliceTransferiYapilacakMi = Convert.ToByte(tvm.PoliceTransfer);
                model.MerkezTVMKodu = _AktifKullanici.TVMKodu;
                if (model.Profili == TVMProfilleri.Merkez)
                {
                    model.MerkezTipi = tvm.Tipi;
                }
                else
                {
                    model.SubeAcenteTipi = tvm.Tipi;
                }
                model.Bolgeleri = new TVMBolgeleriModel();
                model.Bolgeleri.TVMKodu = tvm.Kodu;
                model.Bolgeleri.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

                model.Departmanlari = new TVMDepartmanlarModel();
                model.Departmanlari.TVMKodu = tvm.Kodu;
                model.Departmanlari.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.Departmanlari.MerkezYetkileri = new SelectList(MerkezYetkisiListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Departmanlari.MerkezYetkisi);
                model.Departmanlari.Bolgeler = new SelectList(BolgeList(tvm.Kodu).Items, "Value", "Text", model.Departmanlari.BolgeKodu);

                model.TVMMerkezTipleri = new SelectList(TVMListProvider.TVMTipleri(), "Value", "Text", model.MerkezTipi);
                model.TVMSubeAcenteTipleri = new SelectList(TVMListProvider.TVMTipleriSubeAcente(), "Value", "Text", model.SubeAcenteTipi);
                // model.ProfilTipleri = new SelectList(TVMListProvider.ProfilTipleri(), "Value", "Text", model.Profili);
                model.AcenteSubeVarTipleri = new SelectList(TVMListProvider.VarYokTipleri(), "Value", "Text", model.AcentSuvbeVar);
                model.BaglantiSiniriVarYok = new SelectList(TVMListProvider.VarYokTipleri(), "Value", "Text", model.BaglantiSiniri);
                model.ProjeTipleri = new SelectList(TVMListProvider.ProjeTipleri(), "Value", "Text", model.ProjeKodu);
                model.BolgeYetkiliTipleri = new SelectList(TVMListProvider.BolgeYetkiliTipleri(), "Value", "Text", model.BolgeYetkilisiMi);
                model.PoliceTransferAcentesi = new SelectList(TVMListProvider.PoliceTransferAcentesi(), "Value", "Text", model.PoliceTransferiYapilacakMi);
                model.AcentSuvbeVarText = TVMListProvider.GetVarYok(model.AcentSuvbeVar);
                model.TVMProfiliText = TVMListProvider.ProfilTipiText(model.Profili);
                if (tvm.BagliOlduguTVMKodu != -9999)
                {
                    TVMDetay tvmdetay = _TVMService.GetDetay(tvm.BagliOlduguTVMKodu);
                    if (tvmdetay != null)
                        model.BagliOlduguTVMText = tvmdetay.Unvani;

                }

                List<Ulke> ulkeler = _UlkeService.GetUlkeList().ToList<Ulke>();
                List<Il> iller = _UlkeService.GetIlList(model.UlkeKodu).OrderBy(o => o.IlAdi).ToList<Il>();
                List<Ilce> ilceler = _UlkeService.GetIlceList(model.UlkeKodu, model.IlKodu).OrderBy(o => o.IlceAdi).ToList<Ilce>();
                model.Ulkeler = new SelectList(ulkeler, "UlkeKodu", "UlkeAdi", model.UlkeKodu).ListWithOptionLabel(false);
                model.Iller = new SelectList(iller, "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller(false);
                model.IlceLer = new SelectList(ilceler, "IlceKodu", "IlceAdi", "0").ListWithOptionLabel();
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

                if (tvm.GrupKodu.HasValue)
                {
                    TVMDetay detay = _TVMService.GetDetay(tvm.GrupKodu.Value);
                    model.BolgeYetkilisiText = detay != null ? detay.Unvani : "";
                    model.bolgeYetkilisi = tvm.GrupKodu;
                }

                List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
                model.BolgeYetkilileri = new SelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani", model.bolgeYetkilisi).ListWithOptionLabel();

                TVMLogoModel logomodel = new TVMLogoModel();
                logomodel.Kodu = tvm.Kodu;
                logomodel.Alt = tvm.Unvani;
                logomodel.Src = tvm.Logo;
                model.Logo = logomodel;
                model.id = tvm.Kodu;
            }

            return model;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectSirketAra)]
        public ActionResult NeoConnectSifreYetkisiListe()
        {
            try
            {
                NeoConnectListeModel model = new NeoConnectListeModel();
                List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
                model.TVMKodlari = new SelectList(alttvmler, "Kodu", "Unvani", "").ListWithOptionLabel();

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectSirketAra)]
        public ActionResult NeoConnectSifreYetkisiListe(NeoConnectListeModel model)
        {
            try
            {
                List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
                model.TVMKodlari = new SelectList(alttvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
                var TVMUnvan = alttvmler.Where(s => s.Kodu == model.TVMKodu).FirstOrDefault();
                var List = _TVMService.NeoConnectSirketListesi(model.TVMKodu);
                NeoConnectSifreListModel sifreModel = new NeoConnectSifreListModel();
                foreach (var item in List)
                {
                    sifreModel = new NeoConnectSifreListModel();
                    sifreModel.TVMKodu = item.TVMKodu;
                    sifreModel.AltTVMKodu = item.AltTVMKodu;
                    sifreModel.AltTVMUnvani = TVMUnvan != null ? TVMUnvan.Unvani : "";
                    sifreModel.SirketAdi = item.SigortaSirketAdi;
                    if (item.GrupKodu.HasValue)
                    {
                        sifreModel.GrupAdi = _TVMService.GetGrupAdi(item.GrupKodu.Value);
                    }
                    else
                    {
                        sifreModel.GrupAdi = "";
                    }
                    sifreModel.KullaniciAdi = item.KullaniciAdi;
                    sifreModel.Sifre = item.Sifre;
                    sifreModel.AcenteKodu = item.AcenteKodu;
                    sifreModel.ProxyIpPort = item.ProxyIpPort;
                    model.tableList.Add(sifreModel);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        #endregion

        #region TVM Urun Yetkileri

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.YetkiYonetimi, SekmeKodu = AltMenuSekmeler.TVMUrunYetkileri)]
        public ActionResult UrunYetkileriListe()
        {
            try
            {
                List<TVMDetay> TvmList = _TVMService.GetListTVMDetayYetkili();
                List<TVMUrunYetkileriModel> model = new List<TVMUrunYetkileriModel>();

                foreach (var tvm in TvmList)
                {
                    TVMUrunYetkileriModel yetkimod = new TVMUrunYetkileriModel();
                    yetkimod.TVMKodu = tvm.Kodu;
                    yetkimod.TVMAdi = tvm.Unvani;
                    model.Add(yetkimod);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.YetkiYonetimi, SekmeKodu = AltMenuSekmeler.TVMUrunYetkileri)]
        public ActionResult UrunYetkileriEkle()
        {
            return RedirectToAction("UrunYetkileriListe", "TVM");
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.YetkiYonetimi, SekmeKodu = AltMenuSekmeler.TVMUrunYetkileri)]
        public ActionResult UrunYetkileriDetay(int id)
        {
            bool merkezAcenteYetkisiMi = false;
            if (_TVMService.KullaniciTvmyiGormeyeYetkiliMi(id))
            {
                TVMDetay tvm = _TVMService.GetDetayYetkili(id);
                if (tvm != null)
                {
                    List<TUMDetay> tumList = _TUMService.GetListTUMDetay();
                    List<TUMUrunleri> tumUrunList = _TUMService.GetListUrunler();
                    TVMUrunYetkileriModel_Detay model = new TVMUrunYetkileriModel_Detay();
                    model.TVMKodu = id;
                    model.TVMAdi = tvm.Unvani;

                    List<TVMUrunYetkileri> listYetkiler = _TVMService.GetListTVMUrunYetkileri(id);
                    if (listYetkiler.Count == 0)
                    {
                        listYetkiler = _TVMService.GetListTVMUrunYetkileri(_AktifKullanici.TVMKodu);
                        merkezAcenteYetkisiMi = true;
                    }

                    List<UrunServiceModel> babOnlineUrunler = _UrunService.GetList();
                    babOnlineUrunler = (from l in listYetkiler
                                        join b in babOnlineUrunler on l.BABOnlineUrunKodu equals b.UrunKodu
                                        select b).Distinct().ToList<UrunServiceModel>();

                    model.BabOnlineUrunListesi = new SelectList(babOnlineUrunler, "UrunKodu", "UrunAdi");
                    model.TUMUrunList = new List<TVMUrunYetkileriModel_Urun>();


                    foreach (TVMUrunYetkileri item in listYetkiler)
                    {
                        TVMUrunYetkileriModel_Urun urun = new TVMUrunYetkileriModel_Urun();

                        if (merkezAcenteYetkisiMi)
                        {
                            urun.AcikHesapTahsilatGercek = false;
                            urun.AcikHesapTahsilatTuzel = false;
                            urun.BabOnlineUrunKodu = item.BABOnlineUrunKodu;
                            urun.HavaleEntegrasyon = false;
                            urun.KrediKartiTahsilat = false;
                            urun.ManuelHavale = false;
                            urun.Police = false;
                            urun.Rapor = false;
                            urun.Teklif = false;
                            urun.TUMUrunKodu = item.TUMUrunKodu;
                            TUMUrunleri urunyetki = tumUrunList.Where(w => w.TUMUrunKodu == urun.TUMUrunKodu).FirstOrDefault();
                            if (urunyetki != null)
                                urun.TUMUrunAdi = urunyetki.TUMUrunAdi;

                            TUMDetay tumdetay = tumList.Where(s => s.Kodu == item.TUMKodu).FirstOrDefault();
                            if (tumdetay != null)
                                urun.TUMUnvani = tumdetay.Unvani;

                            model.TUMUrunList.Add(urun);
                        }
                        else
                        {
                            urun.AcikHesapTahsilatGercek = (item.AcikHesapTahsilatGercek == 1 ? true : false);
                            urun.AcikHesapTahsilatTuzel = (item.AcikHesapTahsilatTuzel == 1 ? true : false);
                            urun.BabOnlineUrunKodu = item.BABOnlineUrunKodu;
                            urun.HavaleEntegrasyon = (item.HavaleEntegrasyon == 1 ? true : false);
                            urun.KrediKartiTahsilat = (item.KrediKartiTahsilat == 1 ? true : false);
                            urun.ManuelHavale = (item.ManuelHavale == 1 ? true : false);
                            urun.Police = (item.Police == 1 ? true : false);
                            urun.Rapor = (item.Rapor == 1 ? true : false);
                            urun.Teklif = (item.Teklif == 1 ? true : false);
                            urun.TUMUrunKodu = item.TUMUrunKodu;
                            TUMUrunleri urunyetki = tumUrunList.Where(w => w.TUMUrunKodu == urun.TUMUrunKodu).FirstOrDefault();
                            if (urunyetki != null)
                                urun.TUMUrunAdi = urunyetki.TUMUrunAdi;

                            TUMDetay tumdetay = tumList.Where(s => s.Kodu == item.TUMKodu).FirstOrDefault();
                            if (tumdetay != null)
                                urun.TUMUnvani = tumdetay.Unvani;

                            model.TUMUrunList.Add(urun);
                        }

                    }
                    return View(model);
                }
                else return RedirectToAction("UrunYetkileriListe", "TVM");
            }
            else
                return new RedirectResult("~/Error/ErrorPage/403");
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                      AltMenuKodu = AltMenuler.YetkiYonetimi,
                      SekmeKodu = AltMenuSekmeler.TVMUrunYetkileri,
                      menuPermission = MenuPermission.Guncelleme,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult UrunYetkileriGuncelle(int id)
        {
            if (_TVMService.KullaniciTvmyiGormeyeYetkiliMi(id))
            {
                TVMDetay tvm = _TVMService.GetDetayYetkili(id);

                if (tvm != null)
                {
                    TVMUrunYetkileriEkleModel model = new TVMUrunYetkileriEkleModel();

                    model.TVMKodu = tvm.Kodu;
                    model.TVMUnvani = tvm.Unvani;

                    GetSelectListItems(model);

                    return View(model);
                }

                return RedirectToAction("UrunYetkileriListe", "TVM");
            }
            else
                return new RedirectResult("~/Error/ErrorPage/403");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                      AltMenuKodu = AltMenuler.YetkiYonetimi,
                      SekmeKodu = AltMenuSekmeler.TVMUrunYetkileri,
                      menuPermission = MenuPermission.Guncelleme,
                      menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult UrunYetkileriGuncelle(TVMUrunYetkileriEkleModel model)
        {
            if (ModelState.IsValid)
            {
                if (_TVMService.KullaniciTvmyiGormeyeYetkiliMi(model.TVMKodu))
                {
                    foreach (BabOnlineListe babOnline in model.BabOnlineUrunListesi)
                    {
                        foreach (TVMUrunYetkileriModel_Urun urun in babOnline.TUMUrunList)
                        {
                            TVMUrunYetkileri yetki = _TVMService.GetTVMUrunYetkisi(model.TVMKodu, babOnline.BabOnlineUrunKodu, urun.TUMKodu, urun.TUMUrunKodu);
                            if (yetki == null)
                            {
                                yetki = new TVMUrunYetkileri();
                                yetki.TVMKodu = model.TVMKodu;
                                yetki.TUMKodu = urun.TUMKodu;
                                yetki.BABOnlineUrunKodu = babOnline.BabOnlineUrunKodu;
                                yetki.TUMUrunKodu = urun.TUMUrunKodu;
                                yetki.AcikHesapTahsilatGercek = Convert.ToByte(urun.AcikHesapTahsilatGercek);
                                yetki.AcikHesapTahsilatTuzel = Convert.ToByte(urun.AcikHesapTahsilatTuzel);
                                yetki.HavaleEntegrasyon = Convert.ToByte(urun.HavaleEntegrasyon);
                                yetki.KrediKartiTahsilat = Convert.ToByte(urun.KrediKartiTahsilat);
                                yetki.ManuelHavale = Convert.ToByte(urun.ManuelHavale);
                                yetki.Police = Convert.ToByte(urun.Police);
                                yetki.Rapor = Convert.ToByte(urun.Rapor);
                                yetki.Teklif = Convert.ToByte(urun.Teklif);

                                _TVMService.CreateUrunYetki(yetki);
                            }
                            else
                            {
                                yetki.TVMKodu = model.TVMKodu;
                                yetki.TUMUrunKodu = urun.TUMUrunKodu;
                                yetki.AcikHesapTahsilatGercek = Convert.ToByte(urun.AcikHesapTahsilatGercek);
                                yetki.AcikHesapTahsilatTuzel = Convert.ToByte(urun.AcikHesapTahsilatTuzel);
                                yetki.HavaleEntegrasyon = Convert.ToByte(urun.HavaleEntegrasyon);
                                yetki.KrediKartiTahsilat = Convert.ToByte(urun.KrediKartiTahsilat);
                                yetki.ManuelHavale = Convert.ToByte(urun.ManuelHavale);
                                yetki.Police = Convert.ToByte(urun.Police);
                                yetki.Rapor = Convert.ToByte(urun.Rapor);
                                yetki.Teklif = Convert.ToByte(urun.Teklif);

                                _TVMService.UpdateUrunYetkileri(yetki);
                            }
                        }
                    }

                    return RedirectToAction("UrunYetkileriDetay", "TVM", new { id = model.TVMKodu });
                }
                else return new RedirectResult("~/Error/ErrorPage/403");
            }

            GetSelectListItems(model);

            return View(model);
        }

        private void GetSelectListItems(TVMUrunYetkileriEkleModel model)
        {
            model.TVMListesi = new SelectList(_TVMService.GetListTVMDetay(), "Kodu", "Unvani", model.TVMKodu);
            model.BabOnlineUrunListesi = new List<BabOnlineListe>();
            List<UrunServiceModel> babItems = _UrunService.GetList();
            List<TUMUrunleri> urunList = _TUMService.GetListUrunler();
            List<TVMUrunYetkileri> UrunYetkileri = _TVMService.GetListTVMUrunYetkileri(model.TVMKodu);
            bool merkezacenteYetkisiMi = false;
            if (UrunYetkileri.Count == 0)
            {
                UrunYetkileri = _TVMService.GetListTVMUrunYetkileri(_AktifKullanici.TVMKodu);
                merkezacenteYetkisiMi = true;
            }

            babItems = (from l in UrunYetkileri
                        join b in babItems on l.BABOnlineUrunKodu equals b.UrunKodu
                        select b).Distinct().ToList<UrunServiceModel>();

            List<TUMDetay> tumList = _TUMService.GetListTUMDetay();

            foreach (UrunServiceModel item in babItems)
            {
                List<TUMUrunleri> urunItems = _TUMService.GetListUrunler().Where(w => w.BABOnlineUrunKodu == item.UrunKodu).ToList();
                if (urunItems.Count > 0)
                {
                    BabOnlineListe listItem = new BabOnlineListe();
                    listItem.BabOnlineUrunAdi = item.UrunAdi;
                    listItem.BabOnlineUrunKodu = item.UrunKodu;
                    listItem.TUMUrunList = new List<TVMUrunYetkileriModel_Urun>();

                    foreach (TUMUrunleri urun in urunItems)
                    {
                        TVMUrunYetkileriModel_Urun urundetay = new TVMUrunYetkileriModel_Urun();
                        urundetay.TUMKodu = urun.TUMKodu;
                        urundetay.TUMUrunKodu = urun.TUMUrunKodu;
                        urundetay.TUMUrunAdi = urun.TUMUrunAdi;
                        urundetay.BabOnlineUrunKodu = item.UrunKodu;

                        TVMUrunYetkileri urunYetkisi = UrunYetkileri.Where(s => s.TUMUrunKodu == urun.TUMUrunKodu &&
                                                                           s.BABOnlineUrunKodu == urun.BABOnlineUrunKodu &&
                                                                           s.TUMKodu == urundetay.TUMKodu).FirstOrDefault();

                        if (urunYetkisi != null && !merkezacenteYetkisiMi)
                        {
                            urundetay.AcikHesapTahsilatGercek = Convert.ToBoolean(urunYetkisi.AcikHesapTahsilatGercek);
                            urundetay.AcikHesapTahsilatTuzel = Convert.ToBoolean(urunYetkisi.AcikHesapTahsilatTuzel);
                            urundetay.HavaleEntegrasyon = Convert.ToBoolean(urunYetkisi.HavaleEntegrasyon);
                            urundetay.KrediKartiTahsilat = Convert.ToBoolean(urunYetkisi.KrediKartiTahsilat);
                            urundetay.ManuelHavale = Convert.ToBoolean(urunYetkisi.ManuelHavale);
                            urundetay.Police = Convert.ToBoolean(urunYetkisi.Police);
                            urundetay.Rapor = Convert.ToBoolean(urunYetkisi.Rapor);
                            urundetay.Teklif = Convert.ToBoolean(urunYetkisi.Teklif);
                        }
                        else
                        {
                            urundetay.AcikHesapTahsilatGercek = false;
                            urundetay.AcikHesapTahsilatTuzel = false;
                            urundetay.HavaleEntegrasyon = false;
                            urundetay.KrediKartiTahsilat = false;
                            urundetay.ManuelHavale = false;
                            urundetay.Police = false;
                            urundetay.Rapor = false;
                            urundetay.Teklif = false;
                        }

                        TUMDetay tumdetay = tumList.Where(s => s.Kodu == urundetay.TUMKodu).FirstOrDefault();
                        if (tumdetay != null)
                            urundetay.TUMUnvani = tumdetay.Unvani;

                        listItem.TUMUrunList.Add(urundetay);
                    }
                    model.BabOnlineUrunListesi.Add(listItem);
                }
            }
        }
        #endregion

        #region Bağlantı Ekleme
        public ActionResult BaglantiEkle(int TVMKodu)
        {
            TVMIPBaglantiModel model = new TVMIPBaglantiModel();
            model.TVMKodu = TVMKodu;
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_BaglantiEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BaglantiEkle(TVMIPBaglanti model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMIPBaglantiModel, TVMIPBaglanti>();
                TVMIPBaglanti baglanti = Mapper.Map<TVMIPBaglanti>(model);

                baglanti.KayitTarihi = TurkeyDateTime.Now;

                if (_TVMService.GetListIPBaglanti(model.TVMKodu).Count() != 0)
                    baglanti.SiraNo = _TVMService.GetListIPBaglanti(model.TVMKodu).Select(s => s.SiraNo).Max() + 1;

                _TVMService.CreateIPBaglanti(baglanti);
                return null;
            }


            return PartialView("_BaglantiEkle", model);
        }

        public ActionResult BaglantiGuncelle(int SiraNo, int tvmKodu)
        {
            TVMIPBaglanti baglanti = _TVMService.GetTVMIPBaglanti(SiraNo, tvmKodu);

            Mapper.CreateMap<TVMIPBaglanti, TVMIPBaglantiModel>();
            TVMIPBaglantiModel model = Mapper.Map<TVMIPBaglanti, TVMIPBaglantiModel>(baglanti);

            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_BaglantiEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BaglantiGuncelle(TVMIPBaglanti model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMIPBaglantiModel, TVMIPBaglanti>();
                TVMIPBaglanti baglanti = Mapper.Map<TVMIPBaglanti>(model);

                baglanti.KayitTarihi = TurkeyDateTime.Now;

                _TVMService.UpdateItem(baglanti);
                return null;
            }


            return PartialView("_BaglantiEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult BaglantiSil(int baglantiKodu, int tvmKodu)
        {
            _TVMService.DeleteBaglanti(baglantiKodu, tvmKodu);
            TVMIPBaglantiListModel model = BaglantiList(tvmKodu);

            return PartialView("_Baglantilar", model);
        }
        #endregion

        #region Bölge Ekleme
        public ActionResult BolgeEkle(int TVMKodu)
        {
            TVMBolgeleriModel model = new TVMBolgeleriModel();
            model.TVMKodu = TVMKodu;
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_BolgeEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BolgeEkle(TVMBolgeleriModel model)
        {
            if (ModelState.IsValid)
            {
                //TVMBolgeleri bolge = new TVMBolgeleri();
                //bolge.Aciklama = model.Aciklama;
                //bolge.BolgeAdi = model.BolgeAdi;
                //bolge.Durum = model.Durum;
                //bolge.TVMBolgeKodu = model.TVMBolgeKodu;
                //bolge.TVMKodu = model.TVMKodu;

                //_TVMService.CreateBolge(bolge);
                //return null;

                Mapper.CreateMap<TVMBolgeleriModel, TVMBolgeleri>();
                TVMBolgeleri bolge = Mapper.Map<TVMBolgeleri>(model);

                if (_TVMService.GetListBolgeler(model.TVMKodu).Count() != 0)
                    bolge.TVMBolgeKodu = _TVMService.GetListBolgeler(model.TVMKodu).Select(s => s.TVMBolgeKodu).Max() + 1;

                _TVMService.CreateBolge(bolge);
                return null;
            }


            return PartialView("_BolgeEkle", model);
        }

        public ActionResult BolgeGuncelle(int BolgeNo, int tvmKodu)
        {
            TVMBolgeleri bolge = _TVMService.GetTVMBolge(BolgeNo, tvmKodu);

            Mapper.CreateMap<TVMBolgeleri, TVMBolgeleriModel>();
            TVMBolgeleriModel model = Mapper.Map<TVMBolgeleri, TVMBolgeleriModel>(bolge);

            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_BolgeEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BolgeGuncelle(TVMBolgeleriModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMBolgeleriModel, TVMBolgeleri>();
                TVMBolgeleri bolge = Mapper.Map<TVMBolgeleriModel, TVMBolgeleri>(model);

                _TVMService.UpdateItem(bolge);
                return null;
            }


            return PartialView("_BolgeEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult BolgeSil(int bolgeKodu, int tvmKodu)
        {
            _TVMService.DeleteBolge(bolgeKodu, tvmKodu);
            TVMBolgeleriListModel model = BolgeList(tvmKodu);

            return PartialView("_Bolgeler", model);
        }

        #endregion

        #region Departman Ekleme
        public ActionResult DepartmanEkle(int TVMKodu)
        {
            TVMDepartmanlarModel model = new TVMDepartmanlarModel();
            model.TVMKodu = TVMKodu;
            model.MerkezYetkileri = new SelectList(MerkezYetkisiListesiAktifPasif.DurumTipleri(), "Value", "Text", model.MerkezYetkisi);
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            model.Bolgeler = new SelectList(BolgeList(TVMKodu).Items, "TVMBolgeKodu", "BolgeAdi", model.BolgeKodu);

            return PartialView("_DepartmanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmanEkle(TVMDepartmanlar model)
        {
            if (ModelState.IsValid)
            {
                //TVMDepartmanlar departman = new TVMDepartmanlar();
                //departman.Adi = model.Adi;
                //departman.BolgeKodu = model.BolgeKodu;
                //departman.DepartmanKodu = model.DepartmanKodu;
                //departman.Durum = model.Durum;
                //departman.MerkezYetkisi = model.MerkezYetkisi;
                //departman.TVMKodu = model.TVMKodu;

                //_TVMService.CreateDepartman(departman);
                //return null;

                Mapper.CreateMap<TVMDepartmanlarModel, TVMDepartmanlar>();
                TVMDepartmanlar departman = Mapper.Map<TVMDepartmanlar>(model);

                if (_TVMService.GetListDepartmanlar(model.TVMKodu).Count() != 0)
                    departman.DepartmanKodu = _TVMService.GetListDepartmanlar(model.TVMKodu).Select(s => s.DepartmanKodu).Max() + 1;

                _TVMService.CreateDepartman(departman);
                return null;
            }


            return PartialView("_DepartmanEkle", model);
        }

        public ActionResult DepartmanGuncelle(int departmanNo, int tvmKodu)
        {
            TVMDepartmanlar departman = _TVMService.GetTVMDepartman(departmanNo, tvmKodu);

            Mapper.CreateMap<TVMDepartmanlar, TVMDepartmanlarModel>();
            TVMDepartmanlarModel model = Mapper.Map<TVMDepartmanlar, TVMDepartmanlarModel>(departman);

            model.MerkezYetkileri = new SelectList(MerkezYetkisiListesiAktifPasif.DurumTipleri(), "Value", "Text", model.MerkezYetkisi);
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            model.Bolgeler = new SelectList(BolgeList(departman.TVMKodu).Items, "TVMBolgeKodu", "BolgeAdi", model.BolgeKodu);

            return PartialView("_DepartmanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult DepartmanGuncelle(TVMDepartmanlar model)
        {
            if (ModelState.IsValid)
            {
                //TVMDepartmanlar departman = new TVMDepartmanlar();
                //departman.Adi = model.Adi;
                //departman.BolgeKodu = model.BolgeKodu;
                //departman.DepartmanKodu = model.DepartmanKodu;
                //departman.Durum = model.Durum;
                //departman.MerkezYetkisi = model.MerkezYetkisi;
                //departman.TVMKodu = model.TVMKodu;

                Mapper.CreateMap<TVMDepartmanlarModel, TVMDepartmanlar>();
                TVMDepartmanlar departman = Mapper.Map<TVMDepartmanlar>(model);

                _TVMService.UpdateItem(departman);
                return null;
            }


            return PartialView("_DepartmanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult DepartmanSil(int departmanKodu, int tvmKodu)
        {
            _TVMService.DeleteDepartman(departmanKodu, tvmKodu);
            TVMDepartmanlarListModel model = DepartmanList(tvmKodu);

            return PartialView("_Departmanlar", model);
        }
        #endregion

        #region Acente Sşirketi/ürün bazında ödeme tipi Tanımlama/Ekleme/Sime
        public ActionResult AcenteEkle(int TVMKodu)
        {
            TVMAcentelikleriModel model = new TVMAcentelikleriModel();
            model.TVMKodu = TVMKodu;
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            model.SigortaSirketleriList = new SelectList(SigortaSirketleriList().Items, "SirketKodu", "SirketAdi", model.SigortaSirketKodu);

            //var odemeTipleri = _Muhasebe_CariHesapService.getCariOdemeTipleriList();
            //model.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", "1");

            List<SelectListItem> odemeTip = new List<SelectListItem>();
            odemeTip.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Açık" },
                new SelectListItem() { Value = "1", Text = "Kapalı" }
            });
            model.OdemeTipleri = new SelectList(odemeTip, "Value", "Text", model.OdemeTipi);


            List<Bran> brans = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            model.Branslar = new MultiSelectList(brans, "BransKodu", "BransAdi");
            return PartialView("_AcentelikEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult AcenteEkle(TVMAcentelikleriModel model)
        {
            //0 açık. 1 kapalı
            if (ModelState.IsValid)
            {
                if (model.BranslarSelectList != null)
                {
                    List<string> liste = new List<string>();
                    foreach (var item in model.BranslarSelectList)
                    {
                        if (item != "multiselect-all")
                        {
                            liste.Add(item);
                        }
                    }
                    //model.SatisKanali = String.Empty;
                    for (int i = 0; i < liste.Count; i++)
                    {
                        TVMAcentelikleri acente = new TVMAcentelikleri();

                        acente.TVMKodu = _AktifKullanici.TVMKodu;
                        acente.SigortaSirketKodu = model.SigortaSirketKodu;
                        acente.BransKodu = Convert.ToInt32(liste[i]);
                        acente.OdemeTipi = model.OdemeTipi;

                        var varMi = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(acente.TVMKodu, acente.SigortaSirketKodu, acente.BransKodu);
                        if (varMi.Count == 0)
                        {
                            _TVMService.CreateAcente(acente);
                        }
                        else
                        {
                            ViewBag.Mesaj = "Böyle bir kayıt zaten var";
                        }
                    }
                }
                return null;
            }


            return PartialView("_AcentelikEkle", model);
        }
        public ActionResult AcenteGuncelle(int acenteKodu, int tvmKodu)
        {
            TVMAcentelikleri acentelik = _TVMService.GetTVMAcente(acenteKodu, tvmKodu);

            Mapper.CreateMap<TVMAcentelikleri, TVMAcentelikleriModel>();
            TVMAcentelikleriModel model = Mapper.Map<TVMAcentelikleri, TVMAcentelikleriModel>(acentelik);

            //var odemeTipleri = _Muhasebe_CariHesapService.getCariOdemeTipleriList();
            //model.OdemeTipleri = new SelectList(odemeTipleri, "Kodu", "Aciklama", "1");
            List<SelectListItem> odemeTip = new List<SelectListItem>();
            odemeTip.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Açık" },
                new SelectListItem() { Value = "1", Text = "Kapalı" }
            });
            model.OdemeTipleri = new SelectList(odemeTip, "Value", "Text", model.OdemeTipi);
            //model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            model.SigortaSirketleriList = new SelectList(SigortaSirketleriList().Items, "SirketKodu", "SirketAdi", model.SigortaSirketKodu);
            List<Bran> brans = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            model.Branslar = new MultiSelectList(brans, "BransKodu", "BransAdi");

            return PartialView("_AcentelikEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult AcenteGuncelle(TVMAcentelikleriModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMAcentelikleriModel, TVMAcentelikleri>();
                TVMAcentelikleri acente = Mapper.Map<TVMAcentelikleri>(model);
                if (model.BranslarSelectList != null)
                {
                    List<string> liste = new List<string>();
                    foreach (var item in model.BranslarSelectList)
                    {
                        if (item != "multiselect-all")
                        {
                            liste.Add(item);
                        }
                    }
                    for (int i = 0; i < liste.Count; i++)
                    {
                        acente.BransKodu = Convert.ToInt32(liste[i]);
                    }
                }
                _TVMService.UpdateItem(acente);
                return null;
            }


            return PartialView("_AcentelikEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult AcenteSil(int acenteKodu, int tvmKodu)
        {
            _TVMService.DeleteAcente(acenteKodu, tvmKodu);
            TVMAcentelikleriListModel model = AcenteList(tvmKodu);

            return PartialView("_Acentelikler", model);
        }

        #endregion

        #region Not Ekleme
        public ActionResult NotEkle(int TVMKodu)
        {
            TVMNotlarModel model = new TVMNotlarModel();
            model.TVMKodu = TVMKodu;

            return PartialView("_NotEkle", model);
        }

        public ActionResult NotOku(int SiraNo, int tvmKodu)
        {
            TVMNotlar not = _TVMService.GetTVMNot(SiraNo, tvmKodu);

            Mapper.CreateMap<TVMNotlar, TVMNotlarModel>();
            TVMNotlarModel model = Mapper.Map<TVMNotlar, TVMNotlarModel>(not);

            return PartialView("_NotEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NotEkle(TVMNotlarModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMNotlarModel, TVMNotlar>();
                TVMNotlar not = Mapper.Map<TVMNotlar>(model);

                not.EkleyenPersonelKodu = 1;
                not.EklemeTarihi = TurkeyDateTime.Now;

                if (_TVMService.GetListNotlar(model.TVMKodu).Count() != 0)
                    not.SiraNo = _TVMService.GetListNotlar(model.TVMKodu).Select(s => s.SiraNo).Max() + 1;

                _TVMService.CreateNot(not);
                return null;
            }


            return PartialView("_NotEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult NotSil(int notKodu, int tvmKodu)
        {
            _TVMService.DeleteNot(notKodu, tvmKodu);
            TVMNotlarListModel model = NotlarList(tvmKodu);

            return PartialView("_Notlar", model);
        }
        #endregion

        #region Dokuman Ekleme
        public ActionResult Dokuman(int TVMKodu)
        {
            TVMDokumanlarModel model = new TVMDokumanlarModel();
            model.TVMKodu = TVMKodu;
            List<DokumanTurleri> dokumanTurleri = _TVMService.GetListDokumanTurleri().ToList<DokumanTurleri>();
            model.DokumanTurleri = new SelectList(dokumanTurleri, "DokumanTurKodu", "DokumanTurAciklama").ListWithOptionLabel();
            return PartialView("_DokumanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult Dokuman(TVMDokumanlarModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileName(file.FileName);

                if (_TVMService.CheckedFileName(fileName))
                {
                    string url = "";// _Storage.UploadFile(model.TVMKodu.ToString(), fileName, file.InputStream);
                    TVMDokumanlar dokuman = new TVMDokumanlar();
                    dokuman.TVMKodu = model.TVMKodu;
                    //Tvm ile ilgili bililer otomatik gelicek....
                    dokuman.EkleyenPersonelKodu = _AktifKullanici.KullaniciKodu;
                    dokuman.EklemeTarihi = TurkeyDateTime.Now;

                    //dokuman.DokumanAdi = fileName;
                    dokuman.DokumanTuru = model.DokumanTuru;
                    dokuman.Dokuman = url;
                    dokuman.SiraNo = 1;

                    if (_TVMService.GetListDokumanlar(model.TVMKodu).Count() != 0)
                        dokuman.SiraNo = _TVMService.GetListDokumanlar(model.TVMKodu).Select(s => s.SiraNo).Max() + 1;

                    _TVMService.CreateDokuman(dokuman);
                    //Kayıt Başarılı ise detay sayfasına gönderiliyor...
                    return null;
                }
                else
                {
                    ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
                    return View(model);
                }
            }
            //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
            ModelState.AddModelError("", babonline.Message_DocumentSaveError);
            return View(model);
        }

        public ActionResult DokumanGuncelle(int SiraNo, int tvmKodu)
        {
            TVMDokumanlar dokuman = _TVMService.GetTVMDokuman(SiraNo, tvmKodu);

            Mapper.CreateMap<TVMDokumanlar, TVMDokumanlarModel>();
            TVMDokumanlarModel model = Mapper.Map<TVMDokumanlar, TVMDokumanlarModel>(dokuman);
            List<DokumanTurleri> dokumanTurleri = _TVMService.GetListDokumanTurleri().ToList<DokumanTurleri>();
            model.DokumanTurleri = new SelectList(dokumanTurleri, "DokumanTurKodu", "DokumanTurAciklama").ListWithOptionLabel();
            return PartialView("_DokumanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult DokumanGuncelle(TVMDokumanlarModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileName(file.FileName);

                if (_TVMService.CheckedFileName(fileName))
                {
                    string url = "";// _Storage.UploadFile(model.TVMKodu.ToString(), fileName, file.InputStream);
                    TVMDokumanlar dokuman = new TVMDokumanlar();
                    dokuman.TVMKodu = model.TVMKodu;
                    //Tvm ile ilgili bililer otomatik gelicek....
                    dokuman.EkleyenPersonelKodu = 1;
                    dokuman.EklemeTarihi = TurkeyDateTime.Now;

                    //dokuman.DokumanAdi = fileName;
                    dokuman.DokumanTuru = model.DokumanTuru;
                    dokuman.Dokuman = url;
                    dokuman.SiraNo = 1;

                    if (_TVMService.GetListDokumanlar(model.TVMKodu).Count() != 0)
                        dokuman.SiraNo = _TVMService.GetListDokumanlar(model.TVMKodu).Select(s => s.SiraNo).Max() + 1;

                    _TVMService.UpdateItem(dokuman);
                    //Kayıt Başarılı ise detay sayfasına gönderiliyor...
                    return null;
                }
                else
                {
                    ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
                    return View(model);
                }
            }
            //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
            ModelState.AddModelError("", babonline.Message_DocumentSaveError);
            return View(model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult DokumanSil(int dokumanKodu, int tvmKodu)
        {
            _TVMService.DeleteDokuman(dokumanKodu, tvmKodu);
            TVMDokumanlarListModel model = DokumanList(tvmKodu);

            return PartialView("_Dokumanlar", model);
        }
        #endregion

        #region BankaHesaplari Ekleme
        public ActionResult BankaHesaplariEkle(int TVMKodu)
        {
            TVMBankaHesaplariModel model = new TVMBankaHesaplariModel();
            model.TVMKodu = TVMKodu;

            List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
            List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

            model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
            model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();
            List<SelectListItem> hesaptips = new List<SelectListItem>();
            hesaptips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "8", Text = "Banka Hesabı" },
                new SelectListItem() { Value = "5", Text = "Acente Kredi Kartı" },
                new SelectListItem() { Value = "6", Text = "Acente Pos hesabı" },
                new SelectListItem() { Value = "1", Text = "Acente Kasa hesabı" },
                new SelectListItem() { Value = "4", Text = "Alınan Çekler" },
                new SelectListItem() { Value = "7", Text = "Alacak Senetleri" },
                new SelectListItem() { Value = "9", Text = "Acente Bireysel K. Kartı" }

            });
            model.HesapTipi = "8";
            model.HesapTipleri = new SelectList(hesaptips, "Value", "Text", model.HesapTipi);
            ViewBag.carihesapyok = false;
            ViewBag.ekle = true;
            return PartialView("_BankaHesaplariEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BankaHesaplariEkle(TVMBankaHesaplariModel model)
        {
            var carihesap = _Muhasebe_CariHesapService.GetCariHesapAdi(model.CariHesapNo);

            if (carihesap.Item2 != null && carihesap.Item2 != "")
            {

                // database modele hesaptipi ve cari hesap kodu girilecek.
                if (ModelState.IsValid)
                {
                    TVMBankaHesaplari bankahesap = new TVMBankaHesaplari();

                    bankahesap.TVMKodu = model.TVMKodu;
                    bankahesap.BankaAdi = model.BankaAdi;
                    bankahesap.HesapAdi = model.HesapAdi;
                    bankahesap.HesapNo = model.HesapNo;
                    bankahesap.IBAN = model.IBAN;
                    bankahesap.AcenteKrediKartiNo = model.AcenteKrediKartiNo;
                    bankahesap.SubeAdi = model.SubeAdi;
                    bankahesap.CariHesapNo = model.CariHesapNo;
                    bankahesap.HesapTipi = Convert.ToInt32(model.HesapTipi);
                    if (_TVMService.GetListTVMBankaHesaplari(model.TVMKodu).Count() != 0)
                        bankahesap.SiraNo = _TVMService.GetListTVMBankaHesaplari(model.TVMKodu).Select(s => s.SiraNo).Max() + 1;

                    _TVMService.CreateTVMBankaHesap(bankahesap);
                    ViewBag.carihesapyok = false;
                    ViewBag.ekle = false;
                    return null;

                }
            }
            else
            {
                List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
                List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

                model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
                model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();
                List<SelectListItem> hesaptips = new List<SelectListItem>();
                hesaptips.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = "8", Text = "Banka Hesabı" },
                new SelectListItem() { Value = "5", Text = "Acente Kredi Kartı" },
                new SelectListItem() { Value = "6", Text = "Acente Pos hesabı" },
                new SelectListItem() { Value = "1", Text = "Acente Kasa hesabı" },
                new SelectListItem() { Value = "4", Text = "Alınan Çekler" },
                new SelectListItem() { Value = "7", Text = "Alacak Senetleri" },
                new SelectListItem() { Value = "9", Text = "Acente Bireysel K. Kartı" }
                });
                model.HesapTipleri = new SelectList(hesaptips, "Value", "Text", model.HesapTipi);
                ViewBag.carihesapyok = true;
                ViewBag.ekle = true;
                ModelState.AddModelError("Uyarı", "Cari hesap kodu bulunamadı.");

            }
            return PartialView("_BankaHesaplariEkle", model);
        }

        public ActionResult BankaHesaplariGuncelle(int SiraNo, int tvmKodu)
        {
            TVMBankaHesaplari bankahesap = _TVMService.GetTVMBankaHesap(SiraNo, tvmKodu);

            Mapper.CreateMap<TVMBankaHesaplari, TVMBankaHesaplariModel>();
            TVMBankaHesaplariModel model = Mapper.Map<TVMBankaHesaplari, TVMBankaHesaplariModel>(bankahesap);
            model.TVMKodu = tvmKodu;
            List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
            List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

            model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
            model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();
            List<SelectListItem> hesaptips = new List<SelectListItem>();
            hesaptips.AddRange(new SelectListItem[] {
                 new SelectListItem() { Value = "8", Text = "Banka Hesabı" },
                new SelectListItem() { Value = "5", Text = "Acente Kredi Kartı" },
                new SelectListItem() { Value = "6", Text = "Acente Pos hesabı" },
                new SelectListItem() { Value = "1", Text = "Acente Kasa hesabı" },
                new SelectListItem() { Value = "4", Text = "Alınan Çekler" },
                new SelectListItem() { Value = "7", Text = "Alacak Senetleri" },
                new SelectListItem() { Value = "9", Text = "Acente Bireysel K. Kartı" }
            });
            model.HesapTipi = bankahesap.HesapTipi.ToString();
            model.HesapTipleri = new SelectList(hesaptips, "Value", "Text", model.HesapTipi);
            model.CariHesapNo = bankahesap.CariHesapNo;
            ViewBag.carihesapyok = false;
            ViewBag.ekle = false;
            return PartialView("_BankaHesaplariEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BankaHesaplariGuncelle(TVMBankaHesaplariModel model)
        {
            var carihesap = _Muhasebe_CariHesapService.GetCariHesapAdi(model.CariHesapNo);

            if (carihesap.Item2 != null && carihesap.Item2 != "")
            {

                if (ModelState.IsValid)
                {
                    Mapper.CreateMap<TVMBankaHesaplariModel, TVMBankaHesaplari>();
                    TVMBankaHesaplari bankahesap = Mapper.Map<TVMBankaHesaplari>(model);
                    bankahesap.CariHesapNo = model.CariHesapNo;
                    _TVMService.UpdateBankaHesap(bankahesap);
                    ViewBag.carihesapyok = false;
                    ViewBag.ekle = false;
                    return null;
                }
            }
            else
            {
                List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
                List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

                model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
                model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();
                List<SelectListItem> hesaptips = new List<SelectListItem>();
                hesaptips.AddRange(new SelectListItem[] {
                    new SelectListItem() { Value = "8", Text = "Banka Hesabı" },
                new SelectListItem() { Value = "5", Text = "Acente Kredi Kartı" },
                new SelectListItem() { Value = "6", Text = "Acente Pos hesabı" },
                new SelectListItem() { Value = "1", Text = "Acente Kasa hesabı" },
                new SelectListItem() { Value = "4", Text = "Alınan Çekler" },
                new SelectListItem() { Value = "7", Text = "Alacak Senetleri" },
                new SelectListItem() { Value = "9", Text = "Acente Bireysel K. Kartı" }
                });
                model.HesapTipleri = new SelectList(hesaptips, "Value", "Text", model.HesapTipi);
                ViewBag.carihesapyok = true;
                ViewBag.ekle = false;
                ModelState.AddModelError("Uyarı", "Cari hesap kodu bulunamadı.");
            }

            return PartialView("_BankaHesaplariEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult BankaHesaplariSil(int SiraNo, int tvmKodu)
        {
            _TVMService.DeleteTVMBankaHesap(SiraNo, tvmKodu);
            TVMBankaHesaplariListModel model = BankaHesaplariList(tvmKodu);

            return PartialView("_BankaHesaplari", model);
        }
        #endregion

        #region IletisimYetkilileri Ekleme
        public ActionResult IletisimYetkilileriEkle(int TVMKodu)
        {
            TVMIletisimYetkilileriModel model = new TVMIletisimYetkilileriModel();
            model.TVMKodu = TVMKodu;
            model.TelefonTipleri = new SelectList(TelefonTipler.TelefonTipleri(), "Value", "Text", model.TelefonTipi);

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult IletisimYetkilileriEkle(TVMIletisimYetkilileriModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMIletisimYetkilileriModel, TVMIletisimYetkilileri>();
                TVMIletisimYetkilileri iletisimYetkili = Mapper.Map<TVMIletisimYetkilileri>(model);

                if (_TVMService.GetListTVMIletisimYetkilileri(model.TVMKodu).Count() != 0)
                    iletisimYetkili.SiraNo = _TVMService.GetListTVMIletisimYetkilileri(model.TVMKodu).Select(s => s.SiraNo).Max() + 1;

                _TVMService.CreateTVMIletisimYetkili(iletisimYetkili);
                return null;
            }


            return PartialView("_IletisimYetkilileriEkle", model);
        }

        public ActionResult IletisimYetkilileriGuncelle(int SiraNo, int tvmKodu)
        {
            TVMIletisimYetkilileri iletisimYetkili = _TVMService.GetTVMIletisimYetkili(SiraNo, tvmKodu);

            Mapper.CreateMap<TVMIletisimYetkilileri, TVMIletisimYetkilileriModel>();
            TVMIletisimYetkilileriModel model = Mapper.Map<TVMIletisimYetkilileri, TVMIletisimYetkilileriModel>(iletisimYetkili);

            model.TelefonTipleri = new SelectList(TelefonTipler.TelefonTipleri(), "Value", "Text", model.TelefonTipi);

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult IletisimYetkilileriGuncelle(TVMIletisimYetkilileriModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMIletisimYetkilileriModel, TVMIletisimYetkilileri>();
                TVMIletisimYetkilileri iletisimYetkili = Mapper.Map<TVMIletisimYetkilileri>(model);

                _TVMService.UpdateIletisimYetkili(iletisimYetkili);
                return null;
            }


            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult IletisimYetkilileriSil(int SiraNo, int tvmKodu)
        {
            _TVMService.DeleteTVMIletisimYetkili(SiraNo, tvmKodu);
            TVMIletisimYetkilileriListModel model = IletisimYetkilileriList(tvmKodu);

            return PartialView("_IletisimYetkilileri", model);
        }
        #endregion

        #region Web servis kullanici bilgileri
        public ActionResult WebServisKullaniciEkle(int TVMKodu)
        {

            TVMWebServisKullanicilariModel model = new TVMWebServisKullanicilariModel();
            model.TVMKodu = TVMKodu;
            model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();

            return PartialView("_WebServisKullaniciEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult WebServisKullaniciEkle(TVMWebServisKullanicilariModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMWebServisKullanicilariModel, TVMWebServisKullanicilari>();
                TVMWebServisKullanicilari webServisKullanici = Mapper.Map<TVMWebServisKullanicilari>(model);

                _TVMService.CreateTVMWebServisKullanicilari(webServisKullanici);
                return null;
            }

            model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();

            return PartialView("_WebServisKullaniciEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult WebServisKullaniciSil(int tvmKodu, int tumKodu)
        {
            _TVMService.DeleteTVMWebServisKullanicilari(tvmKodu, tumKodu);
            List<TVMWebServisKullanicilariModel> model = WebServisKullanicilariList(tvmKodu);

            return PartialView("_WebServisKullanicilari", model);
        }

        public ActionResult WebServisKullaniciGuncelle(int tvmKodu, int tumKodu)
        {
            TVMWebServisKullanicilari kullanici = _TVMService.GetTVMWebServisKullanicilari(tvmKodu, tumKodu);

            Mapper.CreateMap<TVMWebServisKullanicilari, TVMWebServisKullanicilariModel>();
            TVMWebServisKullanicilariModel model = Mapper.Map<TVMWebServisKullanicilari, TVMWebServisKullanicilariModel>(kullanici);

            model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();

            return PartialView("_WebServisKullaniciEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult WebServisKullaniciGuncelle(TVMWebServisKullanicilariModel model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMWebServisKullanicilariModel, TVMWebServisKullanicilari>();
                TVMWebServisKullanicilari kullanici = Mapper.Map<TVMWebServisKullanicilari>(model);

                _TVMService.UpdateTVMWebServisKullanicilari(kullanici);

                return null;
            }
            model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();

            return PartialView("_WebServisKullaniciEkle", model);
        }
        #endregion

        #region NeoConnect kullanici bilgileri

        public ActionResult NeoConnectKullaniciEkle(int TVMKodu)
        {
            SirketWebEkranModel model = new SirketWebEkranModel();
            model.TVMKodu = TVMKodu;

            List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);

            if (TVMKodu == model.AltTVMKodu)
            {
                model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            }
            else
            {
                model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.AltTVMKodu).ListWithOptionLabel();
            }

            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            model.SirketGrupKullaniciListesi = new List<SelectListItem>();
            return PartialView("_NeoConnectKullaniciEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NeoConnectKullaniciEkle(SirketWebEkranModel model)
        {
            if (ModelState.IsValid)
            {
                OtoLoginSigortaSirketKullanicilar AutoLoginKullanici = new OtoLoginSigortaSirketKullanicilar();
                AutoLoginKullanici.TVMKodu = model.TVMKodu;
                AutoLoginKullanici.AltTVMKodu = model.AltTVMKodu;
                AutoLoginKullanici.TUMKodu = model.TUMKodu;
                AutoLoginKullanici.SigortaSirketAdi = _TUMService.GetTumUnvan(model.TUMKodu);
                AutoLoginKullanici.KullaniciAdi = model.KullaniciAdi;
                AutoLoginKullanici.AcenteKodu = model.AcenteKodu;
                AutoLoginKullanici.Sifre = model.Sifre;
                AutoLoginKullanici.InputTextGirisId = model.InputTextGirisId;
                AutoLoginKullanici.InputTextSifreId = model.InputTextSifreId;
                AutoLoginKullanici.InputTextKullaniciId = model.InputTextKullaniciId;
                AutoLoginKullanici.InputTextAcenteKoduId = model.InputTextAcenteKoduId;
                AutoLoginKullanici.LoginUrl = model.LoginUrl;
                AutoLoginKullanici.ProxyIpPort = model.ProxyIpPort;
                AutoLoginKullanici.ProxyKullaniciAdi = model.ProxyKullaniciAdi;
                AutoLoginKullanici.ProxySifre = model.ProxySifre;
                if (model.GrupKodu.HasValue)
                {
                    AutoLoginKullanici.GrupKodu = model.GrupKodu.Value;
                }

                _TVMService.CreateNeoConnectKullanicilari(AutoLoginKullanici);
                return null;
            }
            List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);

            if (model.TVMKodu == model.AltTVMKodu)
            {
                model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            }
            else
            {
                model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.AltTVMKodu).ListWithOptionLabel();
            }
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            model.SirketGrupKullaniciListesi = new List<SelectListItem>();
            return PartialView("_NeoConnectKullaniciEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult NeoConnectKullaniciSil(int id, int tvmKodu)
        {
            _TVMService.DeleteNeoConnectKullanicilari(id);
            List<SirketWebEkranModel> model = NeoConnectKullanicilariList(tvmKodu);

            return PartialView("_NeoConnectKullanicilari", model);
        }

        public ActionResult NeoConnectKullaniciGuncelle(int tvmKodu, int tumKodu, int id)
        {
            OtoLoginSigortaSirketKullanicilar kullanici = _TVMService.GetNeoConnectKullanicilari(tvmKodu, id);

            Mapper.CreateMap<OtoLoginSigortaSirketKullanicilar, SirketWebEkranModel>();
            SirketWebEkranModel model = Mapper.Map<OtoLoginSigortaSirketKullanicilar, SirketWebEkranModel>(kullanici);
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            if (tvmKodu == model.AltTVMKodu)
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            }
            else
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.AltTVMKodu).ListWithOptionLabel();
            }
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", model.TUMKodu).ListWithOptionLabel();
            model.SirketGrupKullaniciListesi = new List<SelectListItem>();

            var grupListesi = _TVMService.GetNeoConnectSirketGrupKullaniciList(tvmKodu, tumKodu.ToString());
            model.SirketGrupKullaniciListesi = new SelectList(grupListesi, "GrupKodu", "GrupAdi", model.GrupKodu).ListWithOptionLabel();

            return PartialView("_NeoConnectKullaniciEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NeoConnectKullaniciGuncelle(SirketWebEkranModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = _TVMService.GetAutoLoginKullanici(model.Id);
                if (kullanici != null)
                {
                    kullanici.KullaniciAdi = model.KullaniciAdi;
                    kullanici.AltTVMKodu = model.AltTVMKodu;
                    kullanici.AcenteKodu = model.AcenteKodu;
                    kullanici.Sifre = model.Sifre;
                    kullanici.ProxyIpPort = model.ProxyIpPort;
                    kullanici.TUMKodu = model.TUMKodu;
                    kullanici.GrupKodu = model.GrupKodu;
                    _TVMService.UpdateNeoConnectKullanicilari(kullanici);
                }
                return null;
            }
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            if (model.TVMKodu == model.AltTVMKodu)
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            }
            else
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.AltTVMKodu).ListWithOptionLabel();
            }
            var grupListesi = _TVMService.GetNeoConnectSirketGrupKullaniciList(model.TVMKodu, model.TUMKodu.ToString());
            model.SirketGrupKullaniciListesi = new SelectList(grupListesi, "GrupKodu", "GrupAdi", "").ListWithOptionLabel();
            return PartialView("_NeoConnectKullaniciEkle", model);
        }
        #endregion

        #region NeoConnect Yasakli Urller

        public ActionResult NeoConnectYasakliUrlEkle(int TVMKodu)
        {

            NeoConnectYasakliUrlModels model = new NeoConnectYasakliUrlModels();
            model.TVMKodu = TVMKodu;
            model.SatisKanallari = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();

            return PartialView("_NeoConnectYasakliUrlEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NeoConnectYasakliUrlEkle(NeoConnectYasakliUrlModels model)
        {

            if (ModelState.IsValid)
            {
                if (model.SatisKanallariSelectList != null)
                {
                    List<string> liste = new List<string>();
                    foreach (var item in model.SatisKanallariSelectList)
                    {
                        if (item != "multiselect-all")
                        {
                            liste.Add(item);
                        }
                    }
                    model.SatisKanali = String.Empty;
                    for (int i = 0; i < liste.Count; i++)
                    {
                        NeoConnectYasakliUrller yasakliUrller = new NeoConnectYasakliUrller();

                        yasakliUrller.TvmKodu = _AktifKullanici.TVMKodu;
                        yasakliUrller.SigortaSirketKodu = Convert.ToInt32(model.SigortaSirketKodu);
                        yasakliUrller.AltTvmKodu = Convert.ToInt32(liste[i]);
                        //      yasakliUrller.id = model.Id;

                        yasakliUrller.YasaklanacakUrl = model.YasaklanacakUrl;

                        _TVMService.CreateNeoConnectYasakliUrlKullanicilari(yasakliUrller);
                    }
                }
                return null;
            }
            model.SatisKanallari = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();

            return PartialView("_NeoConnectYasakliUrlEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult NeoConnectYasakliUrlSil(int tvmKodu, int tumKodu)
        {
            _TVMService.DeleteNeoConnectYasakliUrlKullanicilari(tvmKodu, tumKodu);
            List<NeoConnectYasakliUrlModels> model = NeoConnectYasakliUrlKullanicilariList(tvmKodu);

            return PartialView("_NeoConnectYasakliUrl", model);
        }

        public ActionResult NeoConnectYasakliUrlGuncelle(int tvmKodu, int tumKodu)
        {
            NeoConnectYasakliUrller kullanici = _TVMService.GetNeoConnectYasakliUrlKullanicilari(tvmKodu, tumKodu);

            Mapper.CreateMap<NeoConnectYasakliUrller, NeoConnectYasakliUrlModels>();
            NeoConnectYasakliUrlModels model = Mapper.Map<NeoConnectYasakliUrller, NeoConnectYasakliUrlModels>(kullanici);
            model.SatisKanallari = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.AltTvmKodu.ToString());

            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();

            return PartialView("_NeoConnectYasakliUrlEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NeoConnectYasakliUrlGuncelle(NeoConnectYasakliUrlModels model)
        {
            if (ModelState.IsValid)
            {
                if (model.SatisKanallariSelectList != null)
                {
                    List<string> liste = new List<string>();
                    foreach (var item in model.SatisKanallariSelectList)
                    {
                        if (item != "multiselect-all")
                        {
                            liste.Add(item);
                        }
                    }
                    // model.SatisKanali = String.Empty;
                    for (int i = 0; i < liste.Count; i++)
                    {
                        var kullanici = _TVMService.GetNeoConnectYasakliUrllerKullanici(model.Id);
                        if (kullanici != null)
                        {
                            kullanici.TvmKodu = model.TVMKodu;
                            kullanici.SigortaSirketKodu = Convert.ToInt32(model.SigortaSirketKodu);
                            kullanici.AltTvmKodu = Convert.ToInt32(liste[i]);
                            kullanici.YasaklanacakUrl = model.YasaklanacakUrl;


                            _TVMService.UpdateNeoConnectYasakliUrlKullanicilari(kullanici);
                        }
                    }
                }
                return null;
            }
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            model.SatisKanallari = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            return PartialView("_NeoConnectYasakliUrlEkle", model);
        }


        #endregion

        #region NeoConnect sigorta şirket atama/kısıtlama bilgileri

        public ActionResult NeoConnectSirketYetkiListe()
        {
            NeoConnectSirketYetkileriListModel model = new NeoConnectSirketYetkileriListModel();
            model.TVMKodu = _AktifKullanici.TVMKodu;
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();

            var sirketYetkiListesi = _NeoConnectService.getNeoConnectSirketYetkileri(_AktifKullanici.TVMKodu);
            if (sirketYetkiListesi.Count > 0)
            {
                NeoConnectTvmSirketYetkileriModel listItem = new NeoConnectTvmSirketYetkileriModel();
                foreach (var item in sirketYetkiListesi)
                {
                    listItem = new NeoConnectTvmSirketYetkileriModel();
                    listItem.Id = item.Id;
                    listItem.TVMKodu = item.TvmKodu;
                    var tvmDetay = tvmler.Where(s => s.Kodu == item.TvmKodu).FirstOrDefault();
                    if (tvmDetay != null)
                    {
                        listItem.TVMUnvan = tvmDetay.Unvani;
                    }
                    else
                    {
                        listItem.TVMUnvan = "";
                    }
                    if (item.Durum == 1)
                    {
                        listItem.Durum = "Aktif";
                    }
                    else
                    {
                        listItem.Durum = "Pasif";
                    }

                    var sirketDetay = SSirketler.Where(s => s.SirketKodu == item.TumKodu).FirstOrDefault();
                    if (sirketDetay != null)
                    {
                        listItem.TUMUnvan = sirketDetay.SirketAdi;
                    }
                    else
                    {
                        listItem.TUMUnvan = "";
                    }
                    model.sirketYetkliList.Add(listItem);
                }
            }

            return View(model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]

        public ActionResult NeoConnectSirketYetkiListe(NeoConnectSirketYetkileriListModel model)
        {
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();
            if (ModelState.IsValid)
            {
                NeoConnectTvmSirketYetkileri NeoConnectSirketKisitla = new NeoConnectTvmSirketYetkileri();

                var sirketYetkiListesi = _NeoConnectService.getNeoConnectSirketYetkileri(model.TVMKodu);
                if (sirketYetkiListesi.Count > 0)
                {
                    NeoConnectTvmSirketYetkileriModel listItem = new NeoConnectTvmSirketYetkileriModel();
                    foreach (var item in sirketYetkiListesi)
                    {
                        listItem = new NeoConnectTvmSirketYetkileriModel();
                        listItem.Id = item.Id;
                        listItem.TVMKodu = item.TvmKodu;
                        var tvmDetay = tvmler.Where(s => s.Kodu == item.TvmKodu).FirstOrDefault();
                        if (tvmDetay != null)
                        {
                            listItem.TVMUnvan = tvmDetay.Unvani;
                        }
                        else
                        {
                            listItem.TVMUnvan = "";
                        }
                        if (item.Durum == 1)
                        {
                            listItem.Durum = "Aktif";
                        }
                        else
                        {
                            listItem.Durum = "Pasif";
                        }

                        var sirketDetay = SSirketler.Where(s => s.SirketKodu == item.TumKodu).FirstOrDefault();
                        if (sirketDetay != null)
                        {
                            listItem.TUMUnvan = sirketDetay.SirketAdi;
                        }
                        else
                        {
                            listItem.TUMUnvan = "";
                        }
                        model.sirketYetkliList.Add(listItem);
                    }
                }
            }
            model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();

            return View(model);
        }


        public ActionResult NeoConnectSirketYetkisiEkle()
        {
            NeoConnectTvmSirketYetkileriModels model = new NeoConnectTvmSirketYetkileriModels();
            model.TVMKodu = _AktifKullanici.TVMKodu;
            model.Durum = 1;
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            //model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();
            model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            var sirketYetkiListesi = _NeoConnectService.getNeoConnectSirketYetkileri(_AktifKullanici.TVMKodu);
            if (sirketYetkiListesi.Count > 0)
            {
                NeoConnectTvmSirketYetkileriModel listItem = new NeoConnectTvmSirketYetkileriModel();
                foreach (var item in sirketYetkiListesi)
                {
                    listItem = new NeoConnectTvmSirketYetkileriModel();
                    listItem.Id = item.Id;
                    listItem.TVMKodu = item.TvmKodu;
                    var tvmDetay = tvmler.Where(s => s.Kodu == item.TvmKodu).FirstOrDefault();
                    if (tvmDetay != null)
                    {
                        listItem.TVMUnvan = tvmDetay.Unvani;
                    }
                    else
                    {
                        listItem.TVMUnvan = "";
                    }
                    if (item.Durum == 1)
                    {
                        listItem.Durum = "Aktif";
                    }
                    else
                    {
                        listItem.Durum = "Pasif";
                    }

                    var sirketDetay = SSirketler.Where(s => s.SirketKodu == item.TumKodu).FirstOrDefault();
                    if (sirketDetay != null)
                    {
                        listItem.TUMUnvan = sirketDetay.SirketAdi;
                    }
                    else
                    {
                        listItem.TUMUnvan = "";
                    }
                    model.sirketYetkliList.Add(listItem);
                }
            }

            return View(model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]

        public ActionResult NeoConnectSirketYetkisiEkle(NeoConnectTvmSirketYetkileriModels model)
        {
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();
            if (ModelState.IsValid)
            {

                NeoConnectTvmSirketYetkileri NeoConnectSirketKisitla = new NeoConnectTvmSirketYetkileri();

                if (model.SigortaSirketleriSelectList != null)
                {
                    List<string> liste = new List<string>();
                    foreach (var item in model.SigortaSirketleriSelectList)
                    {
                        if (item != "multiselect-all")
                        {
                            liste.Add(item);
                        }
                    }
                    model.SigortaSirket = String.Empty;
                    for (int i = 0; i < liste.Count; i++)
                    {
                        NeoConnectSirketKisitla = new NeoConnectTvmSirketYetkileri();
                        NeoConnectSirketKisitla.TvmKodu = model.TVMKodu;
                        NeoConnectSirketKisitla.Durum = model.Durum;
                        NeoConnectSirketKisitla.TumKodu = liste[i];
                        _TVMService.CreateNeoConnectTvmSirketKullanicilari(NeoConnectSirketKisitla);
                    }
                }
                var sirketYetkiListesi = _NeoConnectService.getNeoConnectSirketYetkileri(model.TVMKodu);
                if (sirketYetkiListesi.Count > 0)
                {
                    NeoConnectTvmSirketYetkileriModel listItem = new NeoConnectTvmSirketYetkileriModel();
                    foreach (var item in sirketYetkiListesi)
                    {
                        listItem = new NeoConnectTvmSirketYetkileriModel();
                        listItem.Id = item.Id;
                        listItem.TVMKodu = item.TvmKodu;
                        var tvmDetay = tvmler.Where(s => s.Kodu == item.TvmKodu).FirstOrDefault();
                        if (tvmDetay != null)
                        {
                            listItem.TVMUnvan = tvmDetay.Unvani;
                        }
                        else
                        {
                            listItem.TVMUnvan = "";
                        }
                        if (item.Durum == 1)
                        {
                            listItem.Durum = "Aktif";
                        }
                        else
                        {
                            listItem.Durum = "Pasif";
                        }

                        var sirketDetay = SSirketler.Where(s => s.SirketKodu == item.TumKodu).FirstOrDefault();
                        if (sirketDetay != null)
                        {
                            listItem.TUMUnvan = sirketDetay.SirketAdi;
                        }
                        else
                        {
                            listItem.TUMUnvan = "";
                        }
                        model.sirketYetkliList.Add(listItem);
                    }
                }
            }
            model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani").ListWithOptionLabel();
            model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return View(model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult NeoConnectDeleteSirketSifreYetki(int id, int tvmKodu)
        {
            NeoConnectSirketYetkileriListModel model = new NeoConnectSirketYetkileriListModel();
            try
            {
                _TVMService.NeoConnectDeleteSirketSifreYetki(id);
                model.TVMKodu = _AktifKullanici.TVMKodu;
                List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
                List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();

                var sirketYetkiListesi = _NeoConnectService.getNeoConnectSirketYetkileri(id);
                if (sirketYetkiListesi.Count > 0)
                {
                    NeoConnectTvmSirketYetkileriModel listItem = new NeoConnectTvmSirketYetkileriModel();
                    foreach (var item in sirketYetkiListesi)
                    {
                        listItem = new NeoConnectTvmSirketYetkileriModel();
                        listItem.Id = item.Id;
                        listItem.TVMKodu = item.TvmKodu;
                        var tvmDetay = tvmler.Where(s => s.Kodu == item.TvmKodu).FirstOrDefault();
                        if (tvmDetay != null)
                        {
                            listItem.TVMUnvan = tvmDetay.Unvani;
                        }
                        else
                        {
                            listItem.TVMUnvan = "";
                        }
                        if (item.Durum == 1)
                        {
                            listItem.Durum = "Aktif";
                        }
                        else
                        {
                            listItem.Durum = "Pasif";
                        }

                        var sirketDetay = SSirketler.Where(s => s.SirketKodu == item.TumKodu).FirstOrDefault();
                        if (sirketDetay != null)
                        {
                            listItem.TUMUnvan = sirketDetay.SirketAdi;
                        }
                        else
                        {
                            listItem.TUMUnvan = "";
                        }
                        model.sirketYetkliList.Add(listItem);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return RedirectToAction("NeoConnectSirketYetkiListe", "TVM", new { model = model });
        }
        public ActionResult NeoConnectTvmSigortaSirketEkle(int TVMKodu)
        {

            NeoConnectTvmSirketYetkileriModels model = new NeoConnectTvmSirketYetkileriModels();
            model.TVMKodu = TVMKodu;
            model.Durum = 1;
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
            model.TVMMultiSelectList = new MultiSelectList(tvmler, "Kodu", "Unvani");
            //model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();
            model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_NeoConnectTvmSigortaSirketEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NeoConnectTvmSigortaSirketEkle(NeoConnectTvmSirketYetkileriModels model)
        {
            List<string> TVMKoduListe = new List<string>();
            foreach (var item in model.TVMKoduSelectList)
            {
                if (item != "multiselect-all")
                {
                    TVMKoduListe.Add(item.ToString());
                }
            }
            var temptumlist = _TUMService.GetNeoConnectTUMList();
            for (int k = 0; k < TVMKoduListe.Count(); k++)
            {
                if (ModelState.IsValid)
                {
                    NeoConnectTvmSirketYetkileri NeoConnectSirketKisitla = new NeoConnectTvmSirketYetkileri();
                    if (model.SigortaSirketleriSelectList != null)
                    {
                        List<string> liste = new List<string>();
                        foreach (var item in model.SigortaSirketleriSelectList)
                        {
                            if (item != "multiselect-all")
                            {
                                liste.Add(item);
                            }
                        }
                        model.SigortaSirket = String.Empty;
                        for (int i = 0; i < liste.Count; i++)
                        {
                            NeoConnectSirketKisitla = new NeoConnectTvmSirketYetkileri();
                            NeoConnectSirketKisitla.TvmKodu = Convert.ToInt32(TVMKoduListe[k]);
                            NeoConnectSirketKisitla.Durum = model.Durum;
                            var temptum = temptumlist.Where(x => x.BirlikKodu == liste[i]).FirstOrDefault();
                            if (temptum != null)
                            {
                                NeoConnectSirketKisitla.TumKodu2 = temptum.Kodu; 
                            }
                            NeoConnectSirketKisitla.TumKodu = liste[i];
                            _TVMService.CreateNeoConnectTvmSirketKullanicilari(NeoConnectSirketKisitla);
                        }
                    }


                }
            }

            return null;
        }

        [HttpPost]
        [AjaxException]
        public ActionResult NeoConnectTvmSigortaSirketSil(int tvmKodu, string tumKodu)
        {
            _TVMService.DeleteNeoConnectTvmSirketKullanicilari(tvmKodu, tumKodu);
            List<NeoConnectTvmSirketYetkileriModels> model = NeoConnectTvmSigortaSirketiKullanicilariList(tvmKodu);

            return PartialView("_NeoConnectTvmSigortaSirketi", model);
        }

        public ActionResult NeoConnectTvmSigortaSirketGuncelle(int Id)
        {
            NeoConnectTvmSirketYetkileri sirketYetki = _TVMService.GetNeoConnectTvmSirketYetkileriKullanici(Id);

            Mapper.CreateMap<NeoConnectTvmSirketYetkileri, NeoConnectTvmSirketYetkileriModels>();
            NeoConnectTvmSirketYetkileriModels model = Mapper.Map<NeoConnectTvmSirketYetkileri, NeoConnectTvmSirketYetkileriModels>(sirketYetki);
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
            model.TVMMultiSelectList = new MultiSelectList(tvmler, "Kodu", "Unvani", model.TVMKodu.ToString());
            //model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani",model.TUMKodu).ListWithOptionLabel();
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();
            //  model.SigortaSirketleriSelectList = sirketYetki.TumKodu.t;
            model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi", sirketYetki.TumKodu.ToString());
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            return PartialView("_NeoConnectTvmSigortaSirketEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NeoConnectTvmSigortaSirketGuncelle(NeoConnectTvmSirketYetkileriModels model)
        {

            if (ModelState.IsValid)
            {
                var sirketYetki = _TVMService.GetNeoConnectTvmSirketYetkileriKullanici(model.Id);
                if (sirketYetki != null)
                {
                    //sirketYetki.TvmKodu = model.TVMKodu;
                    //sirketYetki.TumKodu = model.TUMKodu;
                    sirketYetki.Durum = model.Durum;
                    _TVMService.UpdateNeoConnectTvmSirketKullanicilari(sirketYetki);
                }
                return null;
            }
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
            //model.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani", model.TUMKodu).ListWithOptionLabel();
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetNeoConnectSirketListesi();
            model.TVMMultiSelectList = new MultiSelectList(tvmler, "Kodu", "Unvani");
            model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return PartialView("_NeoConnectTvmSigortaSirketEkle", model);
        }
        #endregion

        #region NeoConnect Grup Şirket Yetkisi Güncelleme

        public ActionResult NeoConnectGrupSirketYetkisiGuncelle()
        {
            NeoConnectListModel listModel = new NeoConnectListModel();
            listModel.listGrup = new List<NeoConnectGrupKullaniciModels>();
            listModel.listMerkez = new List<NeoConnectMerkezKullaniciModels>();
            listModel.AktifTvmKodu = _AktifKullanici.TVMKodu;
            listModel.SigortaSirketleri = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", "").ListWithOptionLabel();
            listModel.IslemTipi = 1;
            listModel.IslemTipleri = new SelectList(TVMListProvider.GetIslemTipleri(), "Value", "Text", listModel.IslemTipi);
            return View(listModel);
        }

        #endregion

        #region NeoConnect Grup Kullanıcı Bilgileri Güncelleme

        public ActionResult NeoConnectGrupKullaniciGuncelle()
        {
            NeoConnectListModel listModel = new NeoConnectListModel();
            listModel.listGrup = new List<NeoConnectGrupKullaniciModels>();
            listModel.listMerkez = new List<NeoConnectMerkezKullaniciModels>();
            listModel.AktifTvmKodu = _AktifKullanici.TVMKodu;

            listModel.SigortaSirketleri = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", "").ListWithOptionLabel();
            listModel.IslemTipi = 1;
            listModel.IslemTipleri = new SelectList(TVMListProvider.GetIslemTipleri(), "Value", "Text", listModel.IslemTipi);
            return View(listModel);
        }

        [HttpPost]
        public ActionResult NeoConnectGrupKullaniciGuncelle(NeoConnectListModel model)
        {
            NeoConnectGrupKullaniciModels listItem = new NeoConnectGrupKullaniciModels();
            var tvmDetay = _TVMService.GetDetay(Convert.ToInt32(_AktifKullanici.TVMKodu));
            TUMDetay tumDetay = new TUMDetay();
            model.listGrup = new List<NeoConnectGrupKullaniciModels>();
            model.listMerkez = new List<NeoConnectMerkezKullaniciModels>();


            //if (ModelState.IsValid && file != null && file.ContentLength > 0)
            //{
            //    string _FileName = Path.GetFileName(file.FileName);
            //    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
            //    //path = Path.Combine(Server.MapPath("~/Files"), "_" + Guid.NewGuid().ToString("N") + "_" + file.FileName);
            //    file.SaveAs(_path);
            //}


            if (model.SirketKodu > 0)
            {
                tumDetay = _TUMService.GetDetay(Convert.ToInt32(model.SirketKodu));
            }
            if (model.IslemTipi == 1)
            {
                var list = _TVMService.GetNeoConnectGrupKullanicilist(_AktifKullanici.TVMKodu, model.SirketKodu.ToString());
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        listItem = new NeoConnectGrupKullaniciModels();
                        listItem.TvmKodu = item.TvmKodu;
                        listItem.GrupKodu = item.GrupKodu;
                        listItem.GrupAdi = item.GrupAdi;
                        listItem.KullaniciAdi = item.KullaniciAdi;
                        listItem.Sifre = item.Sifre;
                        listItem.SirketKodu = model.SirketKodu.ToString();
                        listItem.SirketUnvani = tumDetay != null ? tumDetay.Unvani : "";
                        listItem.TvmUnvani = tvmDetay.Unvani;
                        model.listGrup.Add(listItem);
                    }
                }
            }
            else
            {
                var listMerkez = _TVMService.NeoConnectSirketListesi();
                var grupList = _TVMService.GetNeoconnectGruplist();

                NeoConnectMerkezKullaniciModels listMerkezItem = new NeoConnectMerkezKullaniciModels();
                if (listMerkez != null)
                {
                    foreach (var item in listMerkez)
                    {
                        listMerkezItem = new NeoConnectMerkezKullaniciModels();
                        listMerkezItem.TvmKodu = item.TVMKodu;
                        listMerkezItem.GrupKodu = item.GrupKodu;
                        if (item.GrupKodu != null)
                        {
                            var grupAdi = grupList.Where(s => s.GrupKodu == item.GrupKodu).FirstOrDefault();
                            listMerkezItem.GrupAdi = grupAdi != null ? grupAdi.GrupAdi : "";
                        }
                        else
                        {
                            listMerkezItem.GrupAdi = "";
                        }
                        listMerkezItem.KullaniciAdi = item.KullaniciAdi;
                        listMerkezItem.Sifre = item.Sifre;
                        listMerkezItem.TUMKodu = item.TUMKodu;
                        listMerkezItem.SirketUnvani = item.SigortaSirketAdi;
                        listMerkezItem.TvmUnvani = tvmDetay.Unvani;
                        listMerkezItem.ProxyIpPort = item.ProxyIpPort;
                        listMerkezItem.SmsKodTelNo = item.SmsKodTelNo;
                        listMerkezItem.SmsKodSecretKey1 = item.SmsKodSecretKey1;
                        listMerkezItem.SmsKodSecretKey2 = item.SmsKodSecretKey2;
                        model.listMerkez.Add(listMerkezItem);
                    }
                }
            }
            model.SigortaSirketleri = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", model.SirketKodu).ListWithOptionLabel();
            model.IslemTipleri = new SelectList(TVMListProvider.GetIslemTipleri(), "Value", "Text", model.IslemTipi);
            return View(model);

        }

        public ActionResult GrupKullaniciGuncelle(NeoConnectListModel model)
        {
            List<NeoConnectSirketGrupKullaniciDetay> listKayit = new List<NeoConnectSirketGrupKullaniciDetay>();
            NeoConnectSirketGrupKullaniciDetay grupKullaniciItem = new NeoConnectSirketGrupKullaniciDetay();
            List<OtoLoginSigortaSirketKullanicilar> listMerkezKayit = new List<OtoLoginSigortaSirketKullanicilar>();
            OtoLoginSigortaSirketKullanicilar merkezKullaniciItem = new OtoLoginSigortaSirketKullanicilar();
            var tvmDetay = _TVMService.GetDetay(Convert.ToInt32(_AktifKullanici.TVMKodu));

            string mesaj = "";
            try
            {
                if (model.listGrup != null)
                {
                    if (model.listGrup.Count > 0)
                    {
                        NeoConnectGrupKullaniciModels listItem = new NeoConnectGrupKullaniciModels();
                        if (model.listGrup != null)
                        {
                            foreach (var item in model.listGrup)
                            {
                                if (!String.IsNullOrEmpty(item.KullaniciAdi) && !String.IsNullOrEmpty(item.Sifre))
                                {
                                    grupKullaniciItem = new NeoConnectSirketGrupKullaniciDetay();
                                    grupKullaniciItem.TvmKodu = item.TvmKodu;
                                    grupKullaniciItem.GrupKodu = item.GrupKodu;
                                    grupKullaniciItem.GrupAdi = item.GrupAdi;
                                    grupKullaniciItem.KullaniciAdi = item.KullaniciAdi;
                                    grupKullaniciItem.Sifre = item.Sifre;
                                    grupKullaniciItem.SirketKodu = model.SirketKodu.ToString();
                                    listKayit.Add(grupKullaniciItem);
                                }
                                else
                                {
                                    mesaj = "Kullanıcı adı veya şifre boş olamaz. Lütfen girdiğiniz bilgileri kontrol ediniz.";

                                    model.AktifTvmKodu = _AktifKullanici.TVMKodu;
                                    model.SigortaSirketleri = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", model.SirketKodu).ListWithOptionLabel();

                                    return Json(new { sum = mesaj });

                                }
                            }
                            var guncelList = _TVMService.NeoConnectGrupKullanicilistGuncelle(_AktifKullanici.TVMKodu, listKayit.ToList());
                            mesaj = "Kayıt Güncellendi";
                        }
                    }
                }
                else if (model.listMerkez != null)
                {
                    if (model.listMerkez.Count > 0)
                    {
                        NeoConnectMerkezKullaniciModels listItem = new NeoConnectMerkezKullaniciModels();
                        if (model.listMerkez != null)
                        {
                            foreach (var item in model.listMerkez)
                            {
                                if (!String.IsNullOrEmpty(item.KullaniciAdi) && !String.IsNullOrEmpty(item.Sifre))
                                {
                                    merkezKullaniciItem = new OtoLoginSigortaSirketKullanicilar();
                                    merkezKullaniciItem.TVMKodu = item.TvmKodu;
                                    merkezKullaniciItem.GrupKodu = item.GrupKodu;
                                    merkezKullaniciItem.KullaniciAdi = item.KullaniciAdi;
                                    merkezKullaniciItem.Sifre = item.Sifre;
                                    merkezKullaniciItem.TUMKodu = item.TUMKodu;
                                    merkezKullaniciItem.ProxyIpPort = item.ProxyIpPort;
                                    merkezKullaniciItem.SmsKodTelNo = item.SmsKodTelNo;
                                    merkezKullaniciItem.SmsKodSecretKey1 = item.SmsKodSecretKey1;
                                    merkezKullaniciItem.SmsKodSecretKey2 = item.SmsKodSecretKey2;
                                    listMerkezKayit.Add(merkezKullaniciItem);
                                }
                                else
                                {
                                    mesaj = "Kullanıcı adı veya şifre  boş olamaz. Lütfen girdiğiniz bilgileri kontrol ediniz.";

                                    model.AktifTvmKodu = _AktifKullanici.TVMKodu;
                                    model.SigortaSirketleri = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", model.SirketKodu).ListWithOptionLabel();
                                    return Json(new { sum = mesaj });
                                }
                            }
                            var guncelList = _TVMService.NeoConnectMerkezKullanicilistGuncelle(_AktifKullanici.TVMKodu, listMerkezKayit.ToList());
                            mesaj = "Kayıt/Kayıtlar Güncellendi";
                        }
                    }
                }

            }
            catch (Exception)
            {
                mesaj = "İşlem sırasında bir hata oluştu.";
                throw;
            }
            model.AktifTvmKodu = _AktifKullanici.TVMKodu;
            model.SigortaSirketleri = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", model.SirketKodu).ListWithOptionLabel();
            return Json(new { sum = mesaj });
        }


        public ActionResult GrupKullaniciListe()
        {
            //try
            //{
            //    NeoConnectListModel model = new NeoConnectListModel();
            //    List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            //    model.EkranOtologin.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", "").ListWithOptionLabel();

            //    return View(model);
            //}
            //catch (Exception ex)
            //{
            //    _Log.Error(ex);
            return View("_NeoConnectGrupKullaniciGuncelle");
            //}
        }

        [HttpPost]
        public ActionResult GrupKullaniciListe(NeoConnectListModel model)
        {
            try
            {
                //  if (ModelState.IsValid)
                //  {
                //      model.otoLogin.TVMKodu = model.EkranOtologin.TVMKodu;
                //      model.otoLogin.AltTVMKodu = model.EkranOtologin.AltTVMKodu;
                //      model.otoLogin.TUMKodu = model.EkranOtologin.TUMKodu;
                //      model.otoLogin.SigortaSirketAdi = _TUMService.GetTumUnvan(model.EkranOtologin.TUMKodu);
                //      model.otoLogin.KullaniciAdi = model.EkranOtologin.KullaniciAdi;
                //      model.otoLogin.AcenteKodu = model.EkranOtologin.AcenteKodu;
                //      model.otoLogin.Sifre = model.EkranOtologin.Sifre;
                //      model.otoLogin.InputTextGirisId = model.EkranOtologin.InputTextGirisId;
                //      model.otoLogin.InputTextSifreId = model.EkranOtologin.InputTextSifreId;
                //      model.otoLogin.InputTextKullaniciId = model.EkranOtologin.InputTextKullaniciId;
                //      model.otoLogin.InputTextAcenteKoduId = model.EkranOtologin.InputTextAcenteKoduId;
                //      model.otoLogin.LoginUrl = model.EkranOtologin.LoginUrl;
                //      model.otoLogin.ProxyIpPort = model.EkranOtologin.ProxyIpPort;
                //      if (model.EkranOtologin.GrupKodu.HasValue)
                //      {
                //          model.otoLogin.GrupKodu = model.EkranOtologin.GrupKodu.Value;
                //      }

                //      _TVMService.CreateNeoConnectKullanicilari(model.otoLogin);
                //      return null;
                //  }
                //  List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);

                //  if (model.EkranOtologin.TVMKodu == model.EkranOtologin.AltTVMKodu)
                //  {
                //      model.EkranOtologin.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
                //  }
                //  else
                //  {
                //      model.EkranOtologin.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.EkranOtologin.AltTVMKodu).ListWithOptionLabel();
                //  }
                //  model.EkranOtologin.TUMListesi = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani").ListWithOptionLabel();
                //  model.EkranOtologin.SirketGrupKullaniciListesi = new List<SelectListItem>();
                ////  return PartialView("_NeoConnectKullaniciEkle", model);
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }
        #endregion

        #region Logo Yukleme

        [HttpGet]
        [AjaxException]
        public ActionResult LogoEkle(int TVMKodu)
        {
            if (TVMKodu > 0)
            {
                TVMLogoModel model = new TVMLogoModel();
                model.Kodu = TVMKodu;
                return PartialView("_LogoEkle", model);
            }
            return null;
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult LogoEkle(TVMLogoModel model, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && ModelState.IsValid && model.Kodu > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileExtension = Path.GetExtension(fileName);
                if ((fileExtension == ".jpg") || (fileExtension == ".gif") || (fileExtension == ".png"))
                {
                    string guid = Guid.NewGuid().ToString();
                    string url = _LogoStorage.UploadFile(guid + "." + fileExtension, file.InputStream);

                    TVMDetay tvm = _TVMService.GetDetay(model.Kodu);
                    tvm.Logo = url;
                    _TVMService.UpdateDetay(tvm);

                    return null;
                }
                ModelState.AddModelError("", "Yanlızca (jpg,gif,png) türündeki dosyaları ekleyebilirsiniz.");
                return PartialView("_LogoEkle", model);
            }
            ModelState.AddModelError("", "Lütfen uygun formatta bir dosya giriniz.");
            return null;
        }

        public ActionResult LogoView(int TVMKodu)
        {
            if (TVMKodu > 0)
            {
                TVMLogoModel model = new TVMLogoModel();
                TVMDetay tvm = _TVMService.GetDetay(TVMKodu);
                if (tvm != null)
                {
                    model.Kodu = TVMKodu;
                    model.Alt = tvm.Unvani;
                    model.Src = tvm.Logo;
                    return PartialView("_Logo", model);
                }
                return null;
            }
            return null;
        }

        #endregion

        #region List Metodları
        [AjaxException]
        public ActionResult BaglantiView(int TVMKodu)
        {
            return PartialView("_Baglantilar", BaglantiList(TVMKodu));
        }

        public TVMIPBaglantiListModel BaglantiList(int TVMKodu)
        {
            List<TVMIPBaglanti> baglantilar = _TVMService.GetListIPBaglanti(TVMKodu).ToList<TVMIPBaglanti>();

            TVMIPBaglantiListModel model = new TVMIPBaglantiListModel();
            Mapper.CreateMap<TVMIPBaglanti, TVMIPBaglantiModel>();
            model.Items = Mapper.Map<List<TVMIPBaglanti>, List<TVMIPBaglantiModel>>(baglantilar);

            return model;

        }

        [AjaxException]
        public ActionResult BolgeView(int TVMKodu)
        {
            return PartialView("_Bolgeler", BolgeList(TVMKodu));
        }

        public TVMBolgeleriListModel BolgeList(int TVMKodu)
        {
            List<TVMBolgeleri> bolgeler = _TVMService.GetListBolgeler(TVMKodu).ToList<TVMBolgeleri>();

            TVMBolgeleriListModel model = new TVMBolgeleriListModel();
            Mapper.CreateMap<TVMBolgeleri, TVMBolgeleriModel>();
            model.Items = Mapper.Map<List<TVMBolgeleri>, List<TVMBolgeleriModel>>(bolgeler);

            return model;
        }

        [AjaxException]
        public ActionResult DepartmanView(int TVMKodu)
        {
            return PartialView("_Departmanlar", DepartmanList(TVMKodu));
        }

        public TVMDepartmanlarListModel DepartmanList(int TVMKodu)
        {
            List<TVMDepartmanlar> departmanlar = _TVMService.GetListDepartmanlar(TVMKodu).ToList();

            TVMDepartmanlarListModel model = new TVMDepartmanlarListModel();
            Mapper.CreateMap<TVMDepartmanlar, TVMDepartmanlarModel>();
            model.Items = Mapper.Map<List<TVMDepartmanlar>, List<TVMDepartmanlarModel>>(departmanlar);

            return model;
        }

        [AjaxException]
        public ActionResult NotView(int TVMKodu)
        {
            return PartialView("_Notlar", NotlarList(TVMKodu));
        }

        public TVMNotlarListModel NotlarList(int TVMKodu)
        {
            List<TVMNotlar> notlar = _TVMService.GetListNotlar(TVMKodu).ToList();

            TVMNotlarListModel model = new TVMNotlarListModel();
            Mapper.CreateMap<TVMNotlar, TVMNotlarModel>();
            model.Items = Mapper.Map<List<TVMNotlar>, List<TVMNotlarModel>>(notlar);

            return model;
        }

        [AjaxException]
        public ActionResult DokumanView(int TVMKodu)
        {
            return PartialView("_Dokumanlar", DokumanList(TVMKodu));
        }

        public TVMDokumanlarListModel DokumanList(int TVMKodu)
        {
            List<TVMDokumanlar> dokumanlar = _TVMService.GetListDokumanlar(TVMKodu).ToList<TVMDokumanlar>();

            var dokumanTurleri = _TVMService.GetListDokumanTurleri();
            foreach (var item in dokumanlar)
            {
                var dokumanDetay = dokumanTurleri.Where(s => s.DokumanTurKodu == Convert.ToInt32(item.DokumanTuru)).FirstOrDefault();
                item.DokumanTuru = dokumanDetay.DokumanTurAciklama;
            }

            TVMDokumanlarListModel model = new TVMDokumanlarListModel();
            Mapper.CreateMap<TVMDokumanlar, TVMDokumanlarModel>();
            model.Items = Mapper.Map<List<TVMDokumanlar>, List<TVMDokumanlarModel>>(dokumanlar);
            foreach (var item in model.Items)
            {
                var kullaniciDetay = _KullaniciService.GetKullanici(item.EkleyenPersonelKodu);
                if (kullaniciDetay != null)
                {
                    item.EkleyenPersonelAdi = kullaniciDetay.Adi + kullaniciDetay.Soyadi;
                }
            }
            return model;

        }

        [AjaxException]
        public ActionResult AcenteView(int TVMKodu)
        {
            return PartialView("_Acentelikler", AcenteList(TVMKodu));
        }

        public TVMAcentelikleriListModel AcenteList(int TVMKodu)
        {
            List<TVMAcentelikleri> acentelikler = _TVMService.GetListAcenteler(TVMKodu).ToList();

            TVMAcentelikleriListModel model = new TVMAcentelikleriListModel();
            Mapper.CreateMap<TVMAcentelikleri, TVMAcentelikleriModel>();
            model.Items = Mapper.Map<List<TVMAcentelikleri>, List<TVMAcentelikleriModel>>(acentelikler);
            foreach (var item in model.Items)
            {
                item.TVMUnvani = _TVMService.GetDetay(item.TVMKodu).Unvani;
                item.SigortaSirketUnvani = _SigortaSirketleriService.GetSigortaSirketleriUnvan(item.SigortaSirketKodu.ToString());
                item.OdemeTipiAdi = _Muhasebe_CariHesapService.getCariOdemeTip(item.OdemeTipi);
                item.BransUnvani = _BransService.GetBransUnvani(item.BransKodu).ToString();
            }
            return model;
        }

        public SigortaSirketleriListModel SigortaSirketleriList()
        {
            List<SigortaSirketleri> sigorta = _SigortaSirketleriService.GetList();

            SigortaSirketleriListModel model = new SigortaSirketleriListModel();
            Mapper.CreateMap<SigortaSirketleri, SigortaSirketleriModel>();
            model.Items = Mapper.Map<List<SigortaSirketleri>, List<SigortaSirketleriModel>>(sigorta);

            return model;

        }

        [AjaxException]
        public ActionResult BankaHesapView(int TVMKodu)
        {
            return PartialView("_BankaHesaplari", BankaHesaplariList(TVMKodu));
        }

        public TVMBankaHesaplariListModel BankaHesaplariList(int TVMKodu)
        {
            List<TVMBankaHesaplari> bankahesaplari = _TVMService.GetListTVMBankaHesaplari(TVMKodu);

            TVMBankaHesaplariListModel model = new TVMBankaHesaplariListModel();
            Mapper.CreateMap<TVMBankaHesaplari, TVMBankaHesaplariModel>();
            var bankaBilgisi = Mapper.Map<List<TVMBankaHesaplari>, List<TVMBankaHesaplariModel>>(bankahesaplari);
            List<SelectListItem> hesaptips = new List<SelectListItem>();
            hesaptips.AddRange(new SelectListItem[] {
                     new SelectListItem() { Value = "8", Text = "Banka Hesabı" },
                new SelectListItem() { Value = "5", Text = "Acente Kredi Kartı" },
                new SelectListItem() { Value = "6", Text = "Acente Pos hesabı" },
                new SelectListItem() { Value = "1", Text = "Acente Kasa hesabı" },
                new SelectListItem() { Value = "4", Text = "Alınan Çekler" },
                new SelectListItem() { Value = "7", Text = "Alacak Senetleri" },
                new SelectListItem() { Value = "9", Text = "Acente Bireysel K. Kartı" }
                });
            for (int i = 0; i < bankaBilgisi.Count; i++)
            {

                bankaBilgisi[i].HesapTipiAdi = hesaptips.Where(p => p.Value == bankaBilgisi[i].HesapTipi).First().Text; ;
            }

            model.Items = bankaBilgisi;

            return model;
        }

        [AjaxException]
        public ActionResult IletisimYetkiView(int TVMKodu)
        {
            return PartialView("_IletisimYetkilileri", IletisimYetkilileriList(TVMKodu));
        }

        [AjaxException]
        public ActionResult WebservisKullaniciView(int TVMKodu)
        {
            return PartialView("_WebServisKullanicilari", WebServisKullanicilariList(TVMKodu));
        }

        [AjaxException]
        public ActionResult NeoConnectKullaniciView(int TVMKodu)
        {
            return PartialView("_NeoConnectKullanicilari", NeoConnectKullanicilariList(TVMKodu));
        }

        [AjaxException]
        public ActionResult NeoConnectTvmSirketView(int TVMKodu)
        {
            return PartialView("_NeoConnectTvmSigortaSirketi", NeoConnectTvmSigortaSirketiKullanicilariList(TVMKodu));
        }

        [AjaxException]
        public ActionResult NeoconnectYasakliUrlKullaniciView(int TVMKodu)
        {
            return PartialView("_NeoConnectYasakliUrl", NeoConnectYasakliUrlKullanicilariList(TVMKodu));
        }

        public TVMIletisimYetkilileriListModel IletisimYetkilileriList(int TVMKodu)
        {
            List<TVMIletisimYetkilileri> iletisimYetkilileri = _TVMService.GetListTVMIletisimYetkilileri(TVMKodu);

            TVMIletisimYetkilileriListModel model = new TVMIletisimYetkilileriListModel();
            Mapper.CreateMap<TVMIletisimYetkilileri, TVMIletisimYetkilileriModel>();
            model.Items = Mapper.Map<List<TVMIletisimYetkilileri>, List<TVMIletisimYetkilileriModel>>(iletisimYetkilileri);

            return model;
        }

        public List<TVMWebServisKullanicilariModel> WebServisKullanicilariList(int TVMKodu)
        {
            List<TVMWebServisKullanicilari> webServisKullanicilari = _TVMService.GetListTVMWebServisKullanicilari(TVMKodu);

            Mapper.CreateMap<TVMWebServisKullanicilari, TVMWebServisKullanicilariModel>();
            List<TVMWebServisKullanicilariModel> model = Mapper.Map<List<TVMWebServisKullanicilari>, List<TVMWebServisKullanicilariModel>>(webServisKullanicilari);

            foreach (var item in model)
            {
                item.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
            }

            return model;
        }

        public List<SirketWebEkranModel> NeoConnectKullanicilariList(int TVMKodu)
        {
            List<OtoLoginSigortaSirketKullanicilar> webServisKullanicilari = _TVMService.GetListNeoConnectKullanicilari(TVMKodu);

            Mapper.CreateMap<OtoLoginSigortaSirketKullanicilar, SirketWebEkranModel>();
            List<SirketWebEkranModel> model = Mapper.Map<List<OtoLoginSigortaSirketKullanicilar>, List<SirketWebEkranModel>>(webServisKullanicilari);


            foreach (var item in model)
            {
                item.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);

                item.TVMUnvan = _TVMService.GetDetay(item.TVMKodu).Unvani;
                if (item.AltTVMKodu != 0 && item.AltTVMKodu != null)
                {
                    var altTvmKoduBosmu = _TVMService.GetDetay(item.AltTVMKodu.Value);

                    if (altTvmKoduBosmu != null)
                    {
                        item.AltTvmUnvan = altTvmKoduBosmu.Unvani;
                    }

                }
                if (item.GrupKodu != null)
                {
                    var grupKoduDetaay = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(item.GrupKodu.Value);
                    if (grupKoduDetaay != null)
                    {
                        item.GrupAdi = grupKoduDetaay.GrupAdi;

                    }

                }

            }

            return model;
        }

        public List<NeoConnectTvmSirketYetkileriModels> NeoConnectTvmSigortaSirketiKullanicilariList(int TVMKodu)
        {
            List<NeoConnectTvmSirketYetkileri> webServisKullanicilari = _TVMService.GetListNeoConnectTvmSirketKullanicilari(TVMKodu);

            Mapper.CreateMap<NeoConnectTvmSirketYetkileri, NeoConnectTvmSirketYetkileriModels>();
            List<NeoConnectTvmSirketYetkileriModels> model = Mapper.Map<List<NeoConnectTvmSirketYetkileri>, List<NeoConnectTvmSirketYetkileriModels>>(webServisKullanicilari);

            foreach (var item in model)
            {
                //  item.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
                item.TVMUnvan = _TVMService.GetDetay(item.TVMKodu).Unvani;
                item.TUMUnvan = _SigortaSirketleriService.GetSigortaSirketleriUnvan(item.TUMKodu.ToString());
            }

            return model;
        }
        //public List<NeoConnectTvmSirketYetkileriModel> NeoConnectTvmSigortaSirketiKullanicilariList(int TVMKodu)
        //{
        //    List<NeoConnectTvmSirketYetkileri> webServisKullanicilari = _TVMService.GetListNeoConnectTvmSirketKullanicilari(TVMKodu);

        //    Mapper.CreateMap<NeoConnectTvmSirketYetkileri, NeoConnectTvmSirketYetkileriModels>();
        //    List<NeoConnectTvmSirketYetkileriModels> model = Mapper.Map<List<NeoConnectTvmSirketYetkileri>, List<NeoConnectTvmSirketYetkileriModels>>(webServisKullanicilari);

        //    foreach (var item in model)
        //    {
        //        //  item.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
        //        item.TVMUnvan = _TVMService.GetDetay(item.TVMKodu).Unvani;
        //        item.TUMUnvan = _SigortaSirketleriService.GetSigortaSirketleriUnvan(item.TUMKodu.ToString());
        //    }

        //    return model;
        //}

        public List<NeoConnectYasakliUrlModels> NeoConnectYasakliUrlKullanicilariList(int TVMKodu)
        {
            List<NeoConnectYasakliUrller> webServisKullanicilari = _TVMService.GetListNeoConnectYasakliUrlKullanicilari(TVMKodu);

            Mapper.CreateMap<NeoConnectYasakliUrller, NeoConnectYasakliUrlModels>();
            List<NeoConnectYasakliUrlModels> model = Mapper.Map<List<NeoConnectYasakliUrller>, List<NeoConnectYasakliUrlModels>>(webServisKullanicilari);

            foreach (var item in model)
            {
                item.TVMUnvan = _TVMService.GetDetay(item.TVMKodu).Unvani;
                item.TUMUnvan = _TUMService.GetTumUnvan(Convert.ToInt32(item.SigortaSirketKodu));
                item.AltTvmUnvan = _TVMService.GetDetay(item.AltTvmKodu).Unvani;
            }

            return model;
        }

        #endregion

        #region ListePager Metodları
        public ActionResult ListePagerNot(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<TVMNotlar> notList = new DataTableParameters<TVMNotlar>(Request, new Expression<Func<TVMNotlar, object>>[]
                                                                                        {
                                                                                            t => t.SiraNo,
                                                                                            t => t.KonuAdi,
                                                                                            t => t.NotAciklamasi,
                                                                                            t => t.EklemeTarihi
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _TVMService.PagedListNot(notList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerDokuman(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<TVMDokumanlar> dokumanList = new DataTableParameters<TVMDokumanlar>(Request, new Expression<Func<TVMDokumanlar, object>>[]
                                                                                        {
                                                                                            t => t.SiraNo,
                                                                                            t => t.Dokuman,
                                                                                            t => t.DokumanTuru,
                                                                                            t => t.EklemeTarihi,
                                                                                            t => t.EkleyenPersonelKodu
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _TVMService.PagedListDokuman(dokumanList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerIPBaglanti(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<TVMIPBaglanti> baglantiList = new DataTableParameters<TVMIPBaglanti>(Request, new Expression<Func<TVMIPBaglanti, object>>[]
                                                                                        {
                                                                                            t => t.BaslangicIP,
                                                                                            t => t.BitisIP,
                                                                                            t => t.Durum
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _TVMService.PagedListIPBaglanti(baglantiList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerBolge(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<TVMBolgeleri> bolgeList = new DataTableParameters<TVMBolgeleri>(Request, new Expression<Func<TVMBolgeleri, object>>[]
                                                                                        {
                                                                                            t => t.TVMBolgeKodu,
                                                                                            t => t.BolgeAdi,
                                                                                            t => t.Aciklama,
                                                                                            t => t.Durum
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _TVMService.PagedListBolge(bolgeList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePagerDepartman(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<TVMDepartmanlar> departmanList = new DataTableParameters<TVMDepartmanlar>(Request, new Expression<Func<TVMDepartmanlar, object>>[]
                                                                                        {
                                                                                            t => t.DepartmanKodu,
                                                                                            t => t.Adi,
                                                                                            t => t.BolgeKodu,
                                                                                            t => t.Durum
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _TVMService.PagedListDepartman(departmanList, id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        #endregion

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim)]
        public ActionResult TVMDepartmanlar(int tvmKodu)
        {
            List<TVMDepartmanlar> departmanlar = _TVMService.GetListDepartmanlar(tvmKodu);

            List<SelectListItem> departmanlarList = new SelectList(departmanlar, "DepartmanKodu", "Adi", "").ListWithOptionLabel();

            return Json(departmanlarList, JsonRequestBehavior.AllowGet);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim)]
        public ActionResult TVMYetkiler(int tvmKodu)
        {
            List<TVMYetkiGruplari> yetkiler = _YetkiService.GetListYetkiGrup(tvmKodu);

            List<SelectListItem> yetkilerList = new SelectList(yetkiler, "YetkiGrupKodu", "YetkiGrupAdi", "").ListWithOptionLabel();

            return Json(yetkilerList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = 0)]
        public ActionResult TVMListe()
        {
            try
            {
                List<TVMOzetModel> model = _TVMService.GetTVMListeKullaniciYetki(0);

                return PartialView("_TVMListe", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim)]
        public ActionResult GetBolgeler(int TVMKodu)
        {
            List<TVMBolgeleri> bolgeler = new List<TVMBolgeleri>();

            if (TVMKodu > 0)
            {
                bolgeler = _TVMService.GetListBolgeler(TVMKodu);
            }

            return Json(new SelectList(bolgeler, "TVMBolgeKodu", "BolgeAdi", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubeler(string Banka)
        {
            List<BankaSubeleri> subeler = new List<BankaSubeleri>();

            if (Banka != null) subeler = _BankaSubeleri.GetListBankaSubeleri(Banka);

            return Json(new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSirketGrupKullaniciListesi(int TVMKodu, int SirketKodu)
        {
            List<NeoConnectSirketGrupKullaniciDetay> kullaniciGruplari = new List<NeoConnectSirketGrupKullaniciDetay>();

            if (TVMKodu != null && SirketKodu != null) kullaniciGruplari = _TVMService.GetNeoConnectSirketGrupKullaniciList(TVMKodu, SirketKodu.ToString());

            return Json(new SelectList(kullaniciGruplari, "GrupKodu", "GrupAdi", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSirketGrupKullaniciDetay(int grupKodu)
        {
            NeoConnectSirketGrupKullaniciDetay kullaniciGrupDetay = new NeoConnectSirketGrupKullaniciDetay();

            if (grupKodu != null) kullaniciGrupDetay = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(grupKodu);

            return Json(kullaniciGrupDetay, JsonRequestBehavior.AllowGet);
        }

        #region IP Ekleme

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        public ActionResult TVMIPEkle()
        {
            TVMIPBaglantiModel model = new TVMIPBaglantiModel();
            model.Durum = 1;
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            model.TVMKodu = _AktifKullanici.TVMKodu;

            model.IpList = new List<IPListModel>();
            IPListModel ipItem = new IPListModel();
            IQueryable<TVMIPBaglanti> baglantilar;
            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullanici.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullanici.TVMKodu).BagliOlduguTVMKodu;
            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                baglantilar = _TVMService.GetListIPBaglantiAnaTvm(_AktifKullanici.TVMKodu);
            }
            else
            {
                baglantilar = _TVMService.GetListIPBaglanti(_AktifKullanici.TVMKodu);
            }

            if (baglantilar != null)
            {
                foreach (var item in baglantilar)
                {
                    var tvmDetay = _TVMService.GetDetay(item.TVMKodu);
                    ipItem = new IPListModel();
                    if (tvmDetay != null)
                    {
                        ipItem.TVMUnvani = tvmDetay.Unvani;
                    }
                    ipItem.TVMKodu = item.TVMKodu;
                    ipItem.BaslangicIP = item.BaslangicIP;
                    ipItem.BitisIP = item.BitisIP;
                    ipItem.Durum = item.Durum;
                    ipItem.SiraNo = item.SiraNo;
                    model.IpList.Add(ipItem);
                }
            }
            ViewBag.tvmKodu = _AktifKullanici.TVMKodu;
            return View(model);
        }

        //[HttpPost]
        //public ActionResult TVMIPEkle(TVMIPBaglantiModel model)
        //{
        //    model.Durum = 1;
        //    model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
        //    model.TVMKodu = _AktifKullanici.TVMKodu;

        //    model.IpList = new List<IPListModel>();
        //    IPListModel ipItem = new IPListModel();

        //    var baglantilar = _TVMService.GetListIPBaglanti(Convert.ToInt32(model.TVMKoduAra));
        //    if (baglantilar != null)
        //    {
        //        foreach (var item in baglantilar)
        //        {
        //            var tvmDetay = _TVMService.GetDetay(item.TVMKodu);
        //            ipItem = new IPListModel();
        //            if (tvmDetay != null)
        //            {
        //                ipItem.TVMUnvani = tvmDetay.Unvani;
        //            }
        //            ipItem.TVMKodu = item.TVMKodu;
        //            ipItem.BaslangicIP = item.BaslangicIP;
        //            ipItem.BitisIP = item.BitisIP;
        //            ipItem.Durum = item.Durum;
        //            ipItem.SiraNo = item.SiraNo;
        //            model.IpList.Add(ipItem);
        //        }
        //    }
        //    return View(model);
        //}

        #region IP ve TVM Kodu bazlı arama
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        public ActionResult TVMIPAra(TVMIPBaglantiModel model, string postButton, string AramaIpMac, string BaslangicIP, string kaydetButonu, string TVMKoduAra)
        {
            TVMIPBaglantiModel modelIP = new TVMIPBaglantiModel();

            if (!String.IsNullOrEmpty(AramaIpMac) && !String.IsNullOrEmpty(TVMKoduAra))
            {
                int _TVMKodu = Convert.ToInt32(TVMKoduAra);
                modelIP.Durum = 1;
                modelIP.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", modelIP.Durum);

                modelIP.TVMKodu = _AktifKullanici.TVMKodu;

                modelIP.IpList = new List<IPListModel>();
                IPListModel ipItem = new IPListModel();

                var baglantilar = _TVMService.GetListIPBaglanti(_TVMKodu, AramaIpMac);
                if (baglantilar != null)
                {
                    foreach (var item in baglantilar)
                    {
                        var tvmDetay = _TVMService.GetDetay(item.TVMKodu);
                        ipItem = new IPListModel();
                        if (tvmDetay != null)
                        {
                            ipItem.TVMUnvani = tvmDetay.Unvani;
                        }
                        ipItem.TVMKodu = item.TVMKodu;
                        ipItem.BaslangicIP = item.BaslangicIP;
                        ipItem.BitisIP = item.BitisIP;
                        ipItem.Durum = item.Durum;
                        ipItem.SiraNo = item.SiraNo;
                        modelIP.IpList.Add(ipItem);
                    }
                }
            }
            else if ((String.IsNullOrEmpty(model.AramaIpMac) && TVMKoduAra != ""))
            {
                int _TVMKodu = Convert.ToInt32(TVMKoduAra);
                modelIP.Durum = 1;
                modelIP.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", modelIP.Durum);

                modelIP.TVMKodu = _AktifKullanici.TVMKodu;

                modelIP.IpList = new List<IPListModel>();
                IPListModel ipItem = new IPListModel();

                var baglantilar = _TVMService.GetListIPBaglanti(_TVMKodu);
                if (baglantilar != null)
                {
                    foreach (var item in baglantilar)
                    {
                        var tvmDetay = _TVMService.GetDetay(item.TVMKodu);
                        ipItem = new IPListModel();
                        if (tvmDetay != null)
                        {
                            ipItem.TVMUnvani = tvmDetay.Unvani;
                        }
                        ipItem.TVMKodu = item.TVMKodu;
                        ipItem.BaslangicIP = item.BaslangicIP;
                        ipItem.BitisIP = item.BitisIP;
                        ipItem.Durum = item.Durum;
                        ipItem.SiraNo = item.SiraNo;
                        modelIP.IpList.Add(ipItem);
                    }
                }
            }
            else
            {
                modelIP.IpList = new List<IPListModel>();
                IPListModel ipItem = new IPListModel();
                var tvmIp = _TVMService.GetTVMIPAra(AramaIpMac);
                if (tvmIp.Count > 0)
                {
                    foreach (var item in tvmIp)
                    {
                        modelIP.Durum = 1;
                        modelIP.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", modelIP.Durum);
                        modelIP.TVMKodu = item.TVMKodu;
                        ipItem = new IPListModel();
                        ipItem.TVMKodu = item.TVMKodu;
                        ipItem.BaslangicIP = item.BaslangicIP;
                        ipItem.BitisIP = item.BitisIP;
                        ipItem.Durum = item.Durum;
                        var tvmDetay = _TVMService.GetDetay(item.TVMKodu);
                        if (tvmDetay != null)
                        {
                            ipItem.TVMUnvani = tvmDetay.Unvani;
                        }
                        ipItem.SiraNo = item.SiraNo;
                        modelIP.IpList.Add(ipItem);
                    }

                }
                else
                {
                    modelIP.Durum = 1;
                    modelIP.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", modelIP.Durum);
                    modelIP.TVMKodu = _AktifKullanici.TVMKodu;
                    var tvmDetay = _TVMService.GetDetay(modelIP.TVMKodu);
                    if (tvmDetay != null)
                    {
                        ipItem.TVMUnvani = tvmDetay.Unvani;
                    }
                    modelIP.IpList = new List<IPListModel>();
                }
            }



            return View("TVMIPEkle", modelIP);
        }
        #endregion

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        public ActionResult TVMIPGuncelle(int SiraNo, int tvmKodu)
        {
            TVMIPBaglanti baglanti = _TVMService.GetTVMIPBaglanti(SiraNo, tvmKodu);

            Mapper.CreateMap<TVMIPBaglanti, TVMIPBaglantiModel>();
            TVMIPBaglantiModel model = Mapper.Map<TVMIPBaglanti, TVMIPBaglantiModel>(baglanti);

            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

            return View(model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        public ActionResult TVMIPGuncelle(TVMIPBaglanti model)
        {
            if (ModelState.IsValid)
            {
                Mapper.CreateMap<TVMIPBaglantiModel, TVMIPBaglanti>();
                TVMIPBaglanti baglanti = Mapper.Map<TVMIPBaglanti>(model);

                baglanti.KayitTarihi = TurkeyDateTime.Now;

                _TVMService.UpdateItem(baglanti);
                return null;
            }

            return View(model);
        }

        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        public ActionResult TVMIPSil(int siraNo, int tvmkodu)
        {
            try
            {
                _TVMService.DeleteBaglanti(siraNo, tvmkodu);
                return Json(new { sonuc = "Kayıt silindi." });
            }
            catch (Exception)
            {
                return Json(new { sonuc = "İşlem sırasında bir hata oluştu." });
            }

        }
        [HttpPost]
        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        public ActionResult TVMIPEkleAjax(string tvmkodu, string durum, string baslangicIP, string bitisIp)
        {
            string ModelTvmKodu = tvmkodu.Trim();
            string tvmKodu = Convert.ToString(_AktifKullanici.TVMKodu).Trim();
            if (tvmKodu.Length <= ModelTvmKodu.Length)
            {
                ModelTvmKodu = ModelTvmKodu.Substring(0, tvmKodu.Length);
            }
            if ((_AktifKullanici.TVMKodu == 100 || ModelTvmKodu == tvmKodu) && _TVMService.GetTVMIPVarMi(baslangicIP, Convert.ToInt32(tvmkodu.Trim())))
            {
                try
                {
                    TVMIPBaglanti baglanti = new TVMIPBaglanti();
                    baglanti.TVMKodu = Convert.ToInt32(tvmkodu.Trim());
                    baglanti.Durum = Convert.ToByte(1);
                    baglanti.BaslangicIP = baslangicIP.Trim();
                    baglanti.BitisIP = bitisIp.Trim();
                    baglanti.BaslangicIP = Regex.Replace(baglanti.BaslangicIP, @"\s+", "");
                    baglanti.KayitTarihi = TurkeyDateTime.Now;
                    IPListModel ipItem = new IPListModel();
                    var ipList = _TVMService.GetListIPBaglanti();
                    if (ipList != null)
                    {
                        if (ipList.Count() != 0)
                            baglanti.SiraNo = ipList.Select(s => s.SiraNo).Max() + 1;
                        else
                        {
                            baglanti.SiraNo = 1;
                        }
                    }
                    else
                    {
                        baglanti.SiraNo = 1;
                    }
                    _TVMService.CreateIPBaglanti(baglanti);

                    return Json(new { kayitOlduMu = true, sonuc = "IP başarılı bir şekilde eklendi." });
                }
                catch (Exception ex)
                {
                    return Json(new { kayitOlduMu = false, sonuc = "İşlem sırasında bir hata oluştu." });
                }
            }
            else if (ModelTvmKodu != tvmKodu)
            {
                return Json(new { kayitOlduMu = false, sonuc = "Girdiğiniz Satış Kanalı Kodu Hatalı!" });
            }
            else if (!_TVMService.GetTVMIPVarMi(baslangicIP, Convert.ToInt32(tvmkodu.Trim())))
            {
                return Json(new { kayitOlduMu = false, sonuc = "Aynı Başlangıç Ip Sistemde Kayıtlıdır." });
            }
            else
            {
                return Json(new { kayitOlduMu = false, sonuc = "İşlem sırasında bir hata oluştu." });
            }
        }

        #endregion
        #region tvm ip listeleme
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        public ActionResult TVMIPList()
        {
            IQueryable<TVMIPBaglanti> baglantilar;
            TVMIPBaglantiModel model = new TVMIPBaglantiModel();
            model.TVMKodu = _AktifKullanici.TVMKodu;
            model.IpList = new List<IPListModel>();
            IPListModel ipItem = new IPListModel();
            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullanici.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullanici.TVMKodu).BagliOlduguTVMKodu;
            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                baglantilar = _TVMService.GetListIPBaglantiAnaTvm(_AktifKullanici.TVMKodu);
            }
            else
            {
                baglantilar = _TVMService.GetListIPBaglanti(_AktifKullanici.TVMKodu);
            }

            if (baglantilar != null)
            {
                foreach (var item in baglantilar)
                {
                    var tvmDetay = _TVMService.GetDetay(item.TVMKodu);
                    ipItem = new IPListModel();
                    if (tvmDetay != null)
                    {
                        ipItem.TVMUnvani = tvmDetay.Unvani;
                    }
                    ipItem.TVMKodu = item.TVMKodu;
                    ipItem.BaslangicIP = item.BaslangicIP;
                    ipItem.BitisIP = item.BitisIP;
                    ipItem.Durum = item.Durum;
                    ipItem.SiraNo = item.SiraNo;
                    model.IpList.Add(ipItem);
                }
            }
            return View(model);
        }
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.SatısKanalıIpAraEkle)]
        [HttpPost]
        public ActionResult TVMIPList(TVMIPBaglantiModel model)
        {
            #region ip ara
            TVMIPBaglantiModel modelIP = new TVMIPBaglantiModel();
            modelIP.IpList = new List<IPListModel>();
            IPListModel ipItem = new IPListModel();
            var tvmIp = _TVMService.GetTVMIPAraByTvmKodu(model.TVMKodu);
            if (tvmIp.Count > 0)
            {
                foreach (var item in tvmIp)
                {
                    modelIP.TVMKodu = item.TVMKodu;
                    ipItem = new IPListModel();
                    ipItem.TVMKodu = item.TVMKodu;
                    ipItem.BaslangicIP = item.BaslangicIP;
                    ipItem.BitisIP = item.BitisIP;
                    ipItem.Durum = item.Durum;
                    var tvmDetay = _TVMService.GetDetay(item.TVMKodu);
                    if (tvmDetay != null)
                    {
                        ipItem.TVMUnvani = tvmDetay.Unvani;
                    }
                    ipItem.SiraNo = item.SiraNo;
                    modelIP.IpList.Add(ipItem);
                }

            }
            else
            {
                modelIP.TVMKodu = _AktifKullanici.TVMKodu;
                var tvmDetay = _TVMService.GetDetay(modelIP.TVMKodu);
                if (tvmDetay != null)
                {
                    ipItem.TVMUnvani = tvmDetay.Unvani;
                }
                modelIP.IpList = new List<IPListModel>();
            }

            return View(modelIP);
            #endregion
        }

        #endregion
        #region Neo Connect Log Raporlama
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectLogRapor)]
        public ActionResult NeoConnectLogRapor()
        {
            NeoConnectLogModels model = new NeoConnectLogModels();

            //model.KullaniciGirisTarihi = DateTime.Now.ToLongTimeString();
            //model.KullaniciCikisTarihi = DateTime.Now.ToLongDateString();
            // var tvmlistesi = (_TVMService.GetTVMListeKullaniciYetki(0));
            model.TvmKodu = _AktifKullanici.TVMKodu;

            List<TVMOzetModel> tvmlistesi = _TVMService.GetTVMListeKullaniciYetki(0);
            model.tvmler = new SelectList(tvmlistesi, "Kodu", "Unvani");

            //model.TvmKodu = _AktifKullaniciService.TVMKodu;
            //model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            //List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            //model.tvmList = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            //model.tvmler = new SelectList(tvmler, "Kodu", "Unvani");


            // model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            model.KullaniciKodu = _AktifKullanici.KullaniciKodu;
            List<Database.Models.TVMKullanicilar> Kullanici = _KullaniciService.GetListKullanicilar();
            List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.KullanicilarList = new SelectList(alttvmler, "Kodu", "Unvani", model.TvmKodu).ListWithOptionLabel();

            List<Database.Models.TUMDetay> SSirketler = _TUMService.GetNeoConnectTUMList();
            model.SigortaSirketleri = new SelectList(SSirketler, "Kodu", "Unvani");



            model.raporList = new List<NeoConnectLogListModel>();

            NeoConnectLogListModel raporItem = new NeoConnectLogListModel();
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectLogRapor)]
        public ActionResult NeoConnectLogRapor(NeoConnectLogModels model)
        {
            if (model.tvmList != null)
            {
                List<int> liste = new List<int>();
                foreach (var item in model.tvmList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(Convert.ToInt32(item));
                    }
                }
                model.TVMListe = liste;
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.TVMListe = new List<int>();
            }
            if (model.SigortaSirketleriSelectList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.SigortaSirketleriSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.SigortaSirketleriListe = liste;
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.SigortaSirketleriListe = new List<string>();
            }
            List<TVMOzetModel> tvmlistesi = _TVMService.GetTVMListeKullaniciYetki(0);
            model.tvmler = new SelectList(tvmlistesi, "Kodu", "Unvani");

            List<Database.Models.TUMDetay> SSirketler = _TUMService.GetNeoConnectTUMList();
            model.SigortaSirketleri = new SelectList(SSirketler, "Kodu", "Unvani");

            var raporlar = _TVMService.GetListNeoConnectLog(model.KullaniciGirisTarihi.Value, model.KullaniciCikisTarihi.Value, model.TvmKodu, model.SigortaSirketleriListe, model.TVMListe);

            model.raporList = new List<NeoConnectLogListModel>();
            NeoConnectLogListModel raporItem = new NeoConnectLogListModel();

            if (raporlar != null)
            {
                foreach (var item in raporlar)
                {
                    raporItem = new NeoConnectLogListModel();
                    raporItem.Kullanici = item.Kullanici;
                    if (!String.IsNullOrEmpty(item.SigortaSirketKodu))
                    {
                        raporItem.SigortaSirketKodu = _TUMService.GetTumUnvan(Convert.ToInt32(item.SigortaSirketKodu));

                    }
                    raporItem.IPAdresi = item.IPAdresi;
                    raporItem.MACAdresi = item.MACAdresi;
                    raporItem.KullaniciGirisTarihi = item.KullaniciGirisTarihi;
                    raporItem.KullaniciCikisTarihi = item.KullaniciCikisTarihi;
                    raporItem.SirketKullaniciAdi = item.SirketKullaniciAdi;//poliçe no alanını sirket grup adı içib kullanıyorum
                                                                           //  raporItem.SirketKullaniciSifre = item.SirketKullaniciSifresi;
                    model.raporList.Add(raporItem);
                }

            }

            return View(model);
        }
        #endregion

        #region neoconnect k.adı şifre listele/Ekle/Sil/Güncelle

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.KullaniciYonetimi)]
        public ActionResult NeoConnectSifreIslemleri()
        {
            SirketWebEkranModel model = new SirketWebEkranModel();

            model.TVMKodu = _AktifKullanici.TVMKodu;

            List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);


            model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();

            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            model.SirketGrupKullaniciListesi = new List<SelectListItem>();
            var robotTumList = new List<string> { "36", "56", "32", "2", "1", "14", "13", "95", "15", "67", "5", "26", "100", "73", "93", "55", "9", "10", "92", "91", "11", "17" };
            foreach (var item in model.TUMListesi)
            {
                if (robotTumList.Contains(item.Value))
                {
                    item.Text = item.Text + " (ROBOT)";
                }
            }
            model.sifreList = new List<NeoConnectSifreIslemleriListModel>();
            NeoConnectSifreIslemleriListModel sifreItem = new NeoConnectSifreIslemleriListModel();

            var baglantilar = _TVMService.GetListNeoConnectKullanicilari(model.TVMKodu);
            if (baglantilar != null)
            {
                foreach (var item in baglantilar)
                {
                    sifreItem = new NeoConnectSifreIslemleriListModel();
                    sifreItem.TVMKodu = item.TVMKodu;
                    sifreItem.Id = item.Id;
                    sifreItem.TUMKodu = item.TUMKodu;
                    sifreItem.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
                    // sifreItem.TVMUnvani = _TVMService.GetDetay(item.TVMKodu).Unvani;
                    if (item.AltTVMKodu != 0 && item.AltTVMKodu != null)
                    {
                        var altTvmKoduBosmu = _TVMService.GetDetay(item.AltTVMKodu.Value);

                        if (altTvmKoduBosmu != null)
                        {
                            sifreItem.AltTVMUnvani = _TVMService.GetDetay(item.AltTVMKodu.Value).Unvani;

                        }
                    }
                    if (item.GrupKodu != null)
                    {
                        var grupVarmi = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(item.GrupKodu.Value);
                        if (grupVarmi != null)
                        {
                            sifreItem.GrupAdi = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(item.GrupKodu.Value).GrupAdi;
                        }
                    }
                    sifreItem.KullaniciAdi = item.KullaniciAdi;
                    sifreItem.AcenteKodu = item.AcenteKodu;
                    sifreItem.Sifre = item.Sifre;
                    sifreItem.ProxyIpPort = item.ProxyIpPort;
                    sifreItem.ProxyKullaniciAdi = item.ProxyKullaniciAdi;
                    sifreItem.ProxySifre = item.ProxySifre;
                    model.sifreList.Add(sifreItem);
                }
            }
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.KullaniciYonetimi)]
        public ActionResult NeoConnectSifreIslemleri(SirketWebEkranModel model, string postButton)
        {
            //if (postButton == "neoEkle")
            //{
            if (ModelState.IsValid)
            {
                List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);

                if (model.TVMKodu == model.AltTVMKodu)
                {
                    model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
                }
                else
                {
                    model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.AltTVMKodu).ListWithOptionLabel();
                }
                model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
                model.SirketGrupKullaniciListesi = new List<SelectListItem>();
                if (ModelState.IsValid)
                {
                    OtoLoginSigortaSirketKullanicilar AutoLoginKullanici = new OtoLoginSigortaSirketKullanicilar();
                    AutoLoginKullanici.TVMKodu = model.TVMKodu;
                    AutoLoginKullanici.AltTVMKodu = model.AltTVMKodu;
                    AutoLoginKullanici.TUMKodu = model.TUMKodu;
                    AutoLoginKullanici.SigortaSirketAdi = _TUMService.GetTumUnvan(model.TUMKodu);
                    AutoLoginKullanici.KullaniciAdi = model.KullaniciAdi;
                    AutoLoginKullanici.AcenteKodu = model.AcenteKodu;
                    AutoLoginKullanici.Sifre = model.Sifre;
                    AutoLoginKullanici.ProxyIpPort = model.ProxyIpPort;
                    AutoLoginKullanici.ProxyKullaniciAdi = model.ProxyKullaniciAdi;
                    AutoLoginKullanici.ProxySifre = model.ProxySifre;
                    if (model.GrupKodu.HasValue)
                    {
                        AutoLoginKullanici.GrupKodu = model.GrupKodu.Value;
                    }

                    _TVMService.CreateNeoConnectKullanicilari(AutoLoginKullanici);
                    model.IslemMesaji = "İşleminiz başarlı bir şekilde kaydedilmiştir.";
                    //  return View(model);
                }
            }
            else
            {
                model.IslemMesaji = "Lütfen zorunlu alanları doldurunuz.";

            }
            //}
            #region listeyi ekrana getir
            //model.sifreList = new List<NeoConnectSifreIslemleriListModel>();
            //NeoConnectSifreIslemleriListModel sifreItem = new NeoConnectSifreIslemleriListModel();

            //var baglantilar = _TVMService.GetListNeoConnectKullanicilari(model.TVMKodu);
            //if (baglantilar != null)
            //{
            //    foreach (var item in baglantilar)
            //    {
            //        sifreItem = new NeoConnectSifreIslemleriListModel();
            //        sifreItem.TVMKodu = item.TVMKodu;
            //        sifreItem.Id = item.Id;
            //        sifreItem.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
            //        // sifreItem.TVMUnvani = _TVMService.GetDetay(item.TVMKodu).Unvani;
            //        if (item.AltTVMKodu != 0 && item.AltTVMKodu != null)
            //        {
            //            var altTvmKoduBosmu = _TVMService.GetDetay(item.AltTVMKodu.Value);

            //            if (altTvmKoduBosmu != null)
            //            {
            //                sifreItem.AltTVMUnvani = _TVMService.GetDetay(item.AltTVMKodu.Value).Unvani;

            //            }
            //        }
            //        if (item.GrupKodu != null)
            //        {
            //            sifreItem.GrupAdi = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(item.GrupKodu.Value).GrupAdi;
            //        }
            //        sifreItem.KullaniciAdi = item.KullaniciAdi;
            //        sifreItem.AcenteKodu = item.AcenteKodu;
            //        sifreItem.Sifre = item.Sifre;
            //        sifreItem.ProxyIpPort = item.ProxyIpPort;
            //        model.sifreList.Add(sifreItem);
            //    }
            //}
            #endregion

            // return View(model);
            return Json(new { HataMesaji = model.IslemMesaji });

        }

        [HttpPost]
        [AjaxException]
        public ActionResult NeoConnectSifreIslemleriKullaniciSil(int id, int tvmKodu)
        {
            try
            {

                _TVMService.DeleteNeoConnectKullanicilari(id);
                List<SirketWebEkranModel> model = NeoConnectKullanicilariList(tvmKodu);
                return Json(new { sonuc = "Kayıt silindi." });
            }
            catch (Exception)
            {
                _TVMService.DeleteNeoConnectKullanicilari(id);
                List<SirketWebEkranModel> model = NeoConnectKullanicilariList(tvmKodu);
                return Json(new { sonuc = "İşlem sırasında bir hata oluştu." });

                throw;
            }

        }

        public ActionResult NeoConnectSifreIslemleriKullaniciGuncelle(int tvmKodu, int tumKodu, int id)
        {
            OtoLoginSigortaSirketKullanicilar kullanici = _TVMService.GetNeoConnectKullanicilari(tvmKodu, id);
            List<OtoLoginSigortaSirketKullanicilar> kullanicii = _TVMService.GetListNeoConnectKullanicilari(tvmKodu).ToList();

            Mapper.CreateMap<OtoLoginSigortaSirketKullanicilar, SirketWebEkranModel>();
            SirketWebEkranModel model = Mapper.Map<OtoLoginSigortaSirketKullanicilar, SirketWebEkranModel>(kullanici);
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            if (tvmKodu == model.AltTVMKodu)
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            }
            else
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.AltTVMKodu).ListWithOptionLabel();
            }
            if (model.GrupKodu != null)
            {
                model.GrupAdi = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(model.GrupKodu.Value).GrupAdi;
            }
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani", model.TUMKodu).ListWithOptionLabel();
            //model.SirketGrupKullaniciListesi = new List<SelectListItem>();

            var grupListesi = _TVMService.GetNeoConnectSirketGrupKullaniciList(tvmKodu, tumKodu.ToString());
            model.SirketGrupKullaniciListesi = new SelectList(grupListesi, "GrupKodu", "GrupAdi", model.GrupKodu).ListWithOptionLabel();

            return PartialView("_NeoConnectKullaniciEkle", model);
        }


        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NeoConnectSifreIslemleriKullaniciGuncelle(SirketWebEkranModel model)
        {
            if (ModelState.IsValid)
            {
                var kullanici = _TVMService.GetAutoLoginKullanici(model.Id);
                if (kullanici != null)
                {
                    kullanici.KullaniciAdi = model.KullaniciAdi;
                    kullanici.AltTVMKodu = model.AltTVMKodu;
                    kullanici.AcenteKodu = model.AcenteKodu;
                    kullanici.Sifre = model.Sifre;
                    kullanici.ProxyIpPort = model.ProxyIpPort;
                    kullanici.ProxyKullaniciAdi = model.ProxyKullaniciAdi;
                    kullanici.ProxySifre = model.ProxySifre;
                    kullanici.TUMKodu = model.TUMKodu;
                    kullanici.GrupKodu = model.GrupKodu;
                    _TVMService.UpdateNeoConnectKullanicilari(kullanici);
                }
                return null;
            }
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            List<TVMOzetModel> tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            if (model.TVMKodu == model.AltTVMKodu)
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
            }
            else
            {
                model.TVMListesi = new SelectList(tvmler, "Kodu", "Unvani", model.AltTVMKodu).ListWithOptionLabel();
            }
            var grupListesi = _TVMService.GetNeoConnectSirketGrupKullaniciList(model.TVMKodu, model.TUMKodu.ToString());
            model.SirketGrupKullaniciListesi = new SelectList(grupListesi, "GrupKodu", "GrupAdi", "").ListWithOptionLabel();
            return PartialView("_NeoConnectKullaniciEkle", model);
        }


        #endregion
        #region neoconnect grup listele/ekle/sil/güncelle

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectSigortaSirketGrupTanimlama)]
        public ActionResult NeoConnectGrupTanimlama()
        {
            NeoConnectGrupTanımlama model = new NeoConnectGrupTanımlama();
            NeoConnectListGrupTanımlama listItem = new NeoConnectListGrupTanımlama();
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            var grupListesi = _TVMService.GetNeoConnectSirketGrupKullaniciDetayByTVMKodu();
            foreach (var item in grupListesi)
            {
                listItem = new NeoConnectListGrupTanımlama();
                listItem.GrupAdi = item.GrupAdi;
                listItem.KullaniciAdi = item.KullaniciAdi;
                listItem.Sifre = item.Sifre;
                listItem.SirketKodu = item.SirketKodu;
                listItem.id = item.GrupKodu;
                model.grupListesi.Add(listItem);
            }
            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectSigortaSirketGrupTanimlama)]
        public ActionResult NeoConnectGrupKullaniciEkle(string sirketKodu, string GrupAdi, string KullaniciAdi, string sifre)
        {
            string mesaj = "";
            try
            {
                NeoConnectSirketGrupKullaniciDetay model = new NeoConnectSirketGrupKullaniciDetay();
                model.GrupAdi = GrupAdi;
                model.KullaniciAdi = KullaniciAdi;
                model.Sifre = sifre;
                model.SirketKodu = sirketKodu;
                model.TvmKodu = _AktifKullanici.TVMKodu;
                _TVMService.CreateNeoConnectSirketGrupKullaniciDetayByTVMKodu(model);
                mesaj = "true";
            }
            catch (Exception ex)
            {
                mesaj = "false";
            }
            return Json(new { success = mesaj });
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectSigortaSirketGrupTanimlama)]
        public ActionResult NeoConnectGrupKullaniciGuncelleByGrupKodu(string sirketKodu, string GrupAdi, string KullaniciAdi, string sifre, int id)
        {
            string mesaj = "";
            try
            {
                NeoConnectSirketGrupKullaniciDetay model = new NeoConnectSirketGrupKullaniciDetay();
                model.GrupAdi = GrupAdi;
                model.KullaniciAdi = KullaniciAdi;
                model.Sifre = sifre;
                model.SirketKodu = sirketKodu;
                model.GrupKodu = id;
                model.TvmKodu = _AktifKullanici.TVMKodu;
                _TVMService.UpdateNeoConnectSirketGrupKullaniciDetayByTVMKodu(model);
                mesaj = "true";
            }
            catch (Exception ex)
            {
                mesaj = "false";
            }
            return Json(new { success = mesaj });
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.NeoConnectSigortaSirketGrupTanimlama)]
        public ActionResult NeoConnectGrupKullaniciSil(int id)
        {
            string mesaj = "";
            try
            {
                _TVMService.DeleteNeoConnectSirketGrupKullaniciDetayByTVMKodu(id);
                mesaj = "true";

            }
            catch (Exception ex)
            {
                mesaj = "false";
            }
            return Json(new { success = mesaj });
        }



        #endregion

        #region neoconnect k.adı şifre Ara
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.KullaniciAraGuncelleSil)]
        public ActionResult NeoConnectKullaniciAra()
        {
            SirketWebEkranModel model = new SirketWebEkranModel();
            model.TVMKodu = _AktifKullanici.TVMKodu;

            List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();

            List<NeoConnectSirketGrupKullaniciDetay> sirketGruplari = _TVMService.GetListNeoConnectGrupSirketleri(model.TUMKodu, model.TVMKodu).ToList();
            model.GrupListesi = new SelectList(sirketGruplari, "GrupKodu", "GrupAdi").ListWithOptionLabel();

            model.SirketGrupKullaniciListesi = new List<SelectListItem>();
            model.IslemTipi = 0;
            model.IslemTipleri = new SelectList(TVMListProvider.GetIslemTipleri(), "Value", "Text", model.IslemTipi);
            return View(model);
        }
        public ActionResult NeoConnectKullaniciSirketGrupAra(int? TUMKodu)
        {
            try
            {
                SirketWebEkranModel model = new SirketWebEkranModel();
                //IANADOLUKasko _kasko = DependencyResolver.Current.GetService<IANADOLUKasko>();

                //var result = _kasko.getAnadoluTeminatlar(aracKodu, kullanimSekliKodu, kullanimTipKodu, _AktifKullaniciService.TVMKodu);
                //model.list = new SelectList(result.ikameList, "key", "value").ListWithOptionLabel();

                List<NeoConnectSirketGrupKullaniciDetay> sirketGruplari = _TVMService.GetListNeoConnectGrupSirketleri(TUMKodu, _AktifKullanici.TVMKodu).ToList();
                model.GrupListesi = new SelectList(sirketGruplari, "GrupKodu", "GrupAdi").ListWithOptionLabel();

                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.NeoConnectYonetim, SekmeKodu = AltMenuSekmeler.KullaniciAraGuncelleSil)]
        public ActionResult NeoConnectKullaniciAra(SirketWebEkranModel model)
        {
            model.TVMKodu = _AktifKullanici.TVMKodu;

            List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMListesi = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
            model.SirketGrupKullaniciListesi = new List<SelectListItem>();

            List<NeoConnectSirketGrupKullaniciDetay> sirketGruplari = _TVMService.GetListNeoConnectGrupSirketleri(model.TUMKodu, model.TVMKodu).ToList();
            model.GrupListesi = new SelectList(sirketGruplari, "GrupKodu", "GrupAdi").ListWithOptionLabel();

            model.sifreList = new List<NeoConnectSifreIslemleriListModel>();
            NeoConnectSifreIslemleriListModel sifreItem = new NeoConnectSifreIslemleriListModel();
            if (model.IslemTipi == 0)
            {
                var neoKullAra = _TVMService.GetNeoConnectSirketAra(model.TUMKodu, model.TVMKodu);
                if (neoKullAra != null)
                {
                    foreach (var item in neoKullAra)
                    {
                        sifreItem = new NeoConnectSifreIslemleriListModel();
                        sifreItem.TVMKodu = item.TVMKodu;
                        sifreItem.TUMKodu = item.TUMKodu;
                        sifreItem.Id = item.Id;
                        sifreItem.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
                        // sifreItem.TVMUnvani = _TVMService.GetDetay(item.TVMKodu).Unvani;
                        if (item.AltTVMKodu != 0 && item.AltTVMKodu != null)
                        {
                            var altTvmKoduBosmu = _TVMService.GetDetay(item.AltTVMKodu.Value);

                            if (altTvmKoduBosmu != null)
                            {
                                sifreItem.AltTVMUnvani = _TVMService.GetDetay(item.AltTVMKodu.Value).Unvani;

                            }
                        }
                        if (item.GrupKodu != null)
                        {
                            sifreItem.GrupAdi = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(item.GrupKodu.Value).GrupAdi;
                        }
                        sifreItem.GrupKodu = item.GrupKodu;
                        sifreItem.KullaniciAdi = item.KullaniciAdi;
                        sifreItem.AcenteKodu = item.AcenteKodu;
                        sifreItem.Sifre = item.Sifre;
                        sifreItem.ProxyIpPort = item.ProxyIpPort;
                        model.sifreList.Add(sifreItem);
                    }
                }
            }
            else
            {
                var neoKullAra = _TVMService.GetNeoConnectAra(model.TUMKodu, model.GrupKodu, model.TVMKodu);
                if (neoKullAra != null)
                {
                    foreach (var item in neoKullAra)
                    {
                        sifreItem = new NeoConnectSifreIslemleriListModel();
                        sifreItem.TVMKodu = item.TVMKodu;
                        sifreItem.TUMKodu = item.TUMKodu;
                        sifreItem.Id = item.Id;
                        sifreItem.TUMUnvan = _TUMService.GetTumUnvan(item.TUMKodu);
                        // sifreItem.TVMUnvani = _TVMService.GetDetay(item.TVMKodu).Unvani;
                        if (item.AltTVMKodu != 0 && item.AltTVMKodu != null)
                        {
                            var altTvmKoduBosmu = _TVMService.GetDetay(item.AltTVMKodu.Value);

                            if (altTvmKoduBosmu != null)
                            {
                                sifreItem.AltTVMUnvani = _TVMService.GetDetay(item.AltTVMKodu.Value).Unvani;

                            }
                        }
                        if (item.GrupKodu != null)
                        {
                            sifreItem.GrupAdi = _TVMService.GetNeoConnectSirketGrupKullaniciDetay(item.GrupKodu.Value).GrupAdi;
                        }
                        sifreItem.GrupKodu = item.GrupKodu;
                        sifreItem.KullaniciAdi = item.KullaniciAdi;
                        sifreItem.AcenteKodu = item.AcenteKodu;
                        sifreItem.Sifre = item.Sifre;
                        sifreItem.ProxyIpPort = item.ProxyIpPort;
                        model.sifreList.Add(sifreItem);
                    }
                }
            }
            model.TUMListesi = new SelectList(_TUMService.GetNeoConnectTUMList(), "Kodu", "Unvani").ListWithOptionLabel();
            model.IslemTipleri = new SelectList(TVMListProvider.GetIslemTipleri(), "Value", "Text", model.IslemTipi);
            return View(model);


        }
        #endregion

    }
}
