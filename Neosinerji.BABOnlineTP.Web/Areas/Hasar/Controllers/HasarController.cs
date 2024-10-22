using AutoMapper;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Business.TaliPolice;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Hasar.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Hasar.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = 0, SekmeKodu = 0)]
    public class HasarController : Controller
    {
        //
        // GET: /Hasar/Hasar/
        IAktifKullaniciService _AktifKullaniciService;
        ITUMService _TUMService;
        IUrunService _UrunService;
        IKullaniciService _KullaniciService;
        IHasarStorageService _HasarStorageService;
        IRaporService _RaporService;
        IBransService _BransService;
        ITVMService _TVMService;
        IPoliceService _PoliceService;
        IHasarService _HasarService;
        ISigortaSirketleriService _SigortaSirketleriService;
        IBankaSubeleriService _BankaSubeleri;
        public HasarController(IAktifKullaniciService aktifKullaniciService,
                                ITUMService tumService,
                                IUrunService urunService,
                                IKullaniciService kullaniciService,
                                IHasarStorageService hasarStorageService,
                                IBransService bransService,

                                IPoliceService policeService,
                                IHasarService HasarService, ISigortaSirketleriService SigortaSirketleriService, ITVMService TVMService, IBankaSubeleriService bankaSubeleriService)
        {
            _AktifKullaniciService = aktifKullaniciService;
            _TUMService = tumService;
            _UrunService = urunService;
            _KullaniciService = kullaniciService;
            _HasarStorageService = hasarStorageService;
            _RaporService = DependencyResolver.Current.GetService<IRaporService>();
            _BransService = bransService;
            _PoliceService = policeService;
            _HasarService = HasarService;
            _SigortaSirketleriService = SigortaSirketleriService;
            _TVMService = TVMService;
            _BankaSubeleri = bankaSubeleriService;
        }


        public ActionResult HasarGirisi(int? id)
        {
            HasarModel model = new HasarModel();
            model.AnlasmaliServisVarMi = false;
            if (id.HasValue)
            {
                model = HasarGirisiDoldur(id.Value);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult HasarGirisi(HasarModel model)
        {
            try
            {
                if (!model.AnlasmaliServisVarMi)
                {
                    if (ModelState["AnlasmaliServisVarMi"] != null)
                        ModelState["AnlasmaliServisVarMi"].Errors.Clear();
                }
                if (ModelState.IsValid)
                {
                    HasarGenelBilgiler hasar = new HasarGenelBilgiler()
                    {
                        PoliceId = model.PoliceId,
                        SigortaSirketNo = model.SigortaSirketKodu,
                        PoliceNo = model.PoliceNo,
                        YenilemeNo = model.YenilemeNo.HasValue ? model.YenilemeNo.Value.ToString() : "",
                        ZeyilNo = model.EkNo.HasValue ? model.EkNo.Value.ToString() : "",
                        KullaniciKodu = _AktifKullaniciService.KullaniciKodu,
                        KayitTarihi = TurkeyDateTime.Now,
                        AltTVMKodu = model.TaliAcenteKodu,
                        HasarDosyaNo = model.HasarDosyaNo,
                        HasarMevki = model.HasarMevki,
                        HasarSaati = model.HasarSaati,
                        HasarTarihi = model.HasarTarihi,
                        HasarTuruNedeni = model.HasarTuruNedeni,
                        IhbarTarihi = model.IhbarTarihi,
                        KayitTipi = 0,
                        PlakaNo = model.PlakaNo,
                        RedNedeni = model.HasarDosyaRedNedeni,
                        SigortaliTCVKN = model.SigortaliTCVKN,
                        BransKodu = model.BransKodu.HasValue ? model.BransKodu.Value.ToString() : "",
                        UrunKodu = model.UrunKodu.HasValue ? model.UrunKodu.Value.ToString() : "",
                        TVMKodu = _AktifKullaniciService.TVMKodu,
                        DosyayaAtananMTKodu = model.TaliAcenteKodu,
                        HasarDosyaDurumu = Convert.ToInt16(model.HasarDosyaDurumu),
                        AnlasmasiServisKodu = model.AnlasmaliServisKodu,
                    };

                    _HasarService.CreateHasarGenelBilgiler(hasar);
                    model = HasarGirisiDoldur(model.PoliceId);
                }
                else
                {
                    var anlasmaliServisList = _HasarService.GetListAnlasmaliServisler();
                    model.AnlasmaliServisler = new SelectList(anlasmaliServisList, "Kodu", "Unvani", "").ListWithOptionLabel();
                    model.HasarDosyaDurumlari = new SelectList(HasarCommon.HasarDurumlari(), "Value", "Text", model.HasarDosyaDurumu);

                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult HasarGirisiGuncelle(HasarModel model)
        {
            try
            {
                if (model.AnlasmaliServisVarMi != null)
                {
                    if (ModelState["AnlasmaliServisVarMi"] != null)
                        ModelState["AnlasmaliServisVarMi"].Errors.Clear();
                }
                if (ModelState.IsValid)
                {
                    var hasar = _HasarService.GetHasarDetay(model.PoliceId);
                    hasar.KullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                    hasar.KayitTarihi = TurkeyDateTime.Now;
                    hasar.AltTVMKodu = model.TaliAcenteKodu;
                    hasar.HasarDosyaNo = model.HasarDosyaNo;
                    hasar.HasarMevki = model.HasarMevki;
                    hasar.HasarSaati = model.HasarSaati;
                    hasar.HasarTarihi = model.HasarTarihi;
                    hasar.HasarTuruNedeni = model.HasarTuruNedeni;
                    hasar.IhbarTarihi = model.IhbarTarihi;
                    hasar.KayitTipi = 0;
                    hasar.RedNedeni = model.HasarDosyaRedNedeni;
                    hasar.TVMKodu = _AktifKullaniciService.TVMKodu;
                    hasar.DosyayaAtananMTKodu = model.TaliAcenteKodu;
                    hasar.HasarDosyaDurumu = Convert.ToInt16(model.HasarDosyaDurumu);
                    hasar.AnlasmasiServisKodu = Convert.ToInt32(model.HasarDosyaDurumu);

                    _HasarService.UpdateHasarGenelBilgiler(hasar);
                    model = HasarGirisiDoldur(model.PoliceId);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return View(model);
        }

        #region Not Ekleme
        public ActionResult NotEkle(int? id)
        {
            HasarNotlarModel model = new HasarNotlarModel();
            model.HasarId = id.Value;
            return PartialView("_NotEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NotEkle(HasarNotlarModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    HasarNotlari not = new HasarNotlari();

                    not.NotKonu = model.KonuAdi;
                    not.NotAciklama = model.NotAciklamasi;
                    not.HasarId = model.HasarId;
                    not.NotKayitTarihi = TurkeyDateTime.Now;

                    _HasarService.CreateHasarNotlar(not);

                    return null;
                }

                ModelState.AddModelError("", babonline.Message_RequiredValues);
                return PartialView("_NotEkle", model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", babonline.ThereWas_anError);
                return PartialView("_NotEkle", model);
            }
        }

        [AjaxException]
        public ActionResult NotView(int HasarId)
        {
            return PartialView("_Notlar", NotlarList(HasarId));
        }

        [HttpPost]
        [AjaxException]
        public ActionResult NotSil(int notId)
        {
            try
            {
                _HasarService.DeleteNot(notId);
                return Json(new { Basarili = "true", Message = babonline.TheOperationWasSuccessful }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Basarili = "false", Yetkili = "true", Message = babonline.ThereWas_anError }, JsonRequestBehavior.AllowGet);
            }
        }

        public HasarNotlarListModel NotlarList(int HasarId)
        {
            List<HasarNotlari> eksperler = _HasarService.GetListNotlar(HasarId).ToList();
            HasarNotlarListModel model = new HasarNotlarListModel();
            model.Items = new List<HasarNotlarModel>();
            HasarNotlarModel notModel = new HasarNotlarModel();
            if (eksperler != null)
            {
                foreach (var item in eksperler)
                {
                    notModel = new HasarNotlarModel();
                    notModel.HasarId = item.HasarId;
                    notModel.NotId = item.NotId;
                    notModel.KonuAdi = item.NotKonu;
                    notModel.NotAciklamasi = item.NotAciklama;
                    notModel.EklemeTarihi = item.NotKayitTarihi;
                    model.Items.Add(notModel);
                }
            }

            return model;
        }

        #endregion

        #region Hasar Zorunlu Evrak Ekleme
        public ActionResult EvrakEkle(int? id)
        {
            ZorunluEvrakModel model = new ZorunluEvrakModel();

            model.EvrakList = new List<SelectListItem>();
            model.HasarId = id.Value;
            var zorunluEvrakListesi = _HasarService.GetListEvraklar();
            model.EvrakList = new SelectList(zorunluEvrakListesi, "EvrakKodu", "EvrakAdi", "").ToList();

            return PartialView("_EvrakEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult EvrakEkle(ZorunluEvrakModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file.ContentLength > 0)
            {

                //if (_TVMService.CheckedFileName(fileName))
                //{
                string fileName = System.IO.Path.GetFileName(file.FileName);
                string url = _HasarStorageService.UploadFile(model.EvrakKodu.ToString(), fileName, file.InputStream);
                HasarZorunluEvraklari evrak = new HasarZorunluEvraklari();
                evrak.HasarId = model.HasarId;
                evrak.EvrakKodu = Convert.ToInt32(model.EvrakKodu);
                evrak.EvrakKayitTarihi = TurkeyDateTime.Now;
                evrak.EvrakURL = url;

                _HasarService.CreateHasarEvrak(evrak);
                //Kayıt Başarılı ise detay sayfasına gönderiliyor...
                return null;
                //}
                //else
                //{
                //    ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
                //    return View(model);
                //}
            }
            //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
            ModelState.AddModelError("", babonline.Message_DocumentSaveError);
            return View(model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult EvrakSil(int evrakId, int hasarId)
        {
            _HasarService.DeleteHasarEvrak(evrakId);
            HasarZorunluEvrakListModel model = new HasarZorunluEvrakListModel();
            model.Items = new List<ZorunluEvrakModel>();
            model.Items = HasarEvraklariGetir(hasarId);

            return PartialView("_Evraklar", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult EvraklariDoldur(int hasarId)
        {
            try
            {
                HasarZorunluEvrakListModel model = new HasarZorunluEvrakListModel();
                model.Items = HasarEvraklariGetir(hasarId);
                return PartialView("_Evraklar", model);
            }
            catch (Exception ex)
            {

                return null;
            }
        }
        private List<ZorunluEvrakModel> HasarEvraklariGetir(int hasarId)
        {
            List<ZorunluEvrakModel> evrakModelList = new List<ZorunluEvrakModel>();
            ZorunluEvrakModel evrakItem = new ZorunluEvrakModel();
            var evraklar = _HasarService.GetHasarEvraklari(hasarId);

            if (evraklar != null)
            {
                foreach (var item in evraklar)
                {
                    evrakItem = new ZorunluEvrakModel();
                    evrakItem.HasarId = item.HasarId;
                    evrakItem.EvrakId = item.EvrakId;
                    evrakItem.EvrakKodu = item.EvrakKodu.ToString();
                    evrakItem.EvrakAdi = _HasarService.GetEvrakAdi(item.EvrakKodu);
                    evrakItem.Evrak = item.EvrakURL;
                    evrakItem.EvrakKayitTarihi = item.EvrakKayitTarihi;
                    evrakModelList.Add(evrakItem);
                }
            }
            return evrakModelList;
        }

        #endregion

        #region Hasar Eksper İşlemleri

        public ActionResult EksperEkle(int? id)
        {
            HasarEksperModel model = new HasarEksperModel();
            model.HasarId = id.Value;

            var eksperList = _HasarService.GetListEksperler();
            model.EksperList = new SelectList(eksperList, "EksperKodu", "EksperAdSoyadUnvan", "").ListWithOptionLabel();

            var asistansFirmaList = _HasarService.GetListAsistansFirmalari();
            model.AsistansFirmalari = new SelectList(asistansFirmaList, "AsistansKodu", "AsistansAdUnvan", model.AsistansFirma).ListWithOptionLabel();

            model.TahminiHasarParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.AnlasmaliServisParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.TahakkukParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.RucuParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.RedParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.AsistansFirmaParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            return PartialView("_EksperEkle", model);
        }

        [HttpPost]
        public ActionResult EksperEkle(HasarEksperModel model)
        {
            var eksperList = _HasarService.GetListEksperler();
            var asistansFirmaList = _HasarService.GetListAsistansFirmalari();

            if (String.IsNullOrEmpty(model.AnlasmaliServisBedeli))
            {
                if (ModelState["AnlasmaliServisParaBirimi"] != null)
                    ModelState["AnlasmaliServisParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.AsistansFirmaBedeli))
            {
                if (ModelState["AsistansFirmaParaBirimi"] != null)
                    ModelState["AsistansFirmaParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.RedBedeli))
            {
                if (ModelState["RedParaBirimi"] != null)
                    ModelState["RedParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.RucuBedeli))
            {
                if (ModelState["RucuBedeliParaBirimi"] != null)
                    ModelState["RucuBedeliParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.TahakkukBedeli))
            {
                if (ModelState["TahakkukParaBirimi"] != null)
                    ModelState["TahakkukParaBirimi"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                HasarEksperIslemleri eksper = new HasarEksperIslemleri();
                eksper.HasarId = model.HasarId.Value;
                eksper.EksperSorumlusuKodu = model.EksperSorumlusuKodu;

                if (!String.IsNullOrEmpty(model.AnlasmaliServisBedeli))
                {
                    eksper.AnlasmaliServisBedeli = Convert.ToDecimal(model.AnlasmaliServisBedeli);
                    eksper.AnlasmaliServisParaBirimi = model.AnlasmaliServisParaBirimi;
                }

                eksper.AsistansFirma = model.AsistansFirma;
                if (!String.IsNullOrEmpty(model.AsistansFirmaBedeli))
                {
                    eksper.AsistansFirmaBedeli = Convert.ToDecimal(model.AsistansFirmaBedeli);
                    eksper.AsistansFirmaParaBirimi = model.AsistansFirmaParaBirimi;
                }

                if (!String.IsNullOrEmpty(model.RedBedeli))
                {
                    eksper.RedBedeli = Convert.ToDecimal(model.RedBedeli);
                    eksper.RedParaBirimi = model.RedParaBirimi;
                }
                if (!String.IsNullOrEmpty(model.RucuBedeli))
                {
                    eksper.RucuBedeli = Convert.ToDecimal(model.RucuBedeli);
                    eksper.RucuBedeliParaBirimi = model.RucuBedeliParaBirimi;
                }
                if (!String.IsNullOrEmpty(model.TahakkukBedeli))
                {
                    eksper.TahakkukBedeli = Convert.ToDecimal(model.TahakkukBedeli);
                    eksper.TahakkukParaBirimi = model.TahakkukParaBirimi;
                }

                eksper.BeklemeSuresi = model.BeklemeSuresi;
                eksper.BildirimTarihi = !String.IsNullOrEmpty(model.BildirimTarihi) ? Convert.ToDateTime(model.BildirimTarihi) : TurkeyDateTime.Now;
                eksper.GelisSaati = model.GelisSaati;
                eksper.OdemeTarihi = !String.IsNullOrEmpty(model.OdemeTarihi) ? Convert.ToDateTime(model.OdemeTarihi) : TurkeyDateTime.Now;

                eksper.TahminiHasarBedeli = Convert.ToDecimal(model.TahminiHasarBedeli);
                eksper.TahminiHasarParaBirimi = model.TahminiHasarParaBirimi;

                model.EksperList = new SelectList(eksperList, "EksperKodu", "EksperAdSoyadUnvan", model.EksperSorumlusuKodu).ListWithOptionLabel();
                model.AsistansFirmalari = new SelectList(asistansFirmaList, "AsistansKodu", "AsistansAdUnvan", model.AsistansFirma).ListWithOptionLabel();

                _HasarService.CreateHasarEksper(eksper);
                return null;
            }

            model.EksperList = new SelectList(eksperList, "EksperKodu", "EksperAdSoyadUnvan", "").ListWithOptionLabel();
            model.AsistansFirmalari = new SelectList(asistansFirmaList, "AsistansKodu", "AsistansAdUnvan", "").ListWithOptionLabel();

            model.TahminiHasarParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.AnlasmaliServisParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.TahakkukParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.RucuParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.RedParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            model.AsistansFirmaParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "01");
            return PartialView("_EksperEkle", model);
        }

        [AjaxException]
        public ActionResult EksperView(int HasarId)
        {
            return PartialView("_EksperList", EksperlerList(HasarId));
        }
        public HasarEksperIslemleriListModel EksperlerList(int HasarId)
        {
            List<HasarEksperIslemleri> eksperler = _HasarService.GetListEksperIslemleri(HasarId).ToList();
            HasarEksperIslemleriListModel model = new HasarEksperIslemleriListModel();
            model.Items = new List<HasarEksperModel>();
            HasarEksperModel eksperModel = new HasarEksperModel();
            if (eksperler != null)
            {
                foreach (var item in eksperler)
                {
                    eksperModel = new HasarEksperModel();
                    eksperModel.HasarId = item.HasarId;
                    eksperModel.EksperId = item.EksperId;
                    eksperModel.EksperAdSoyad = _HasarService.GetEksperAdi(item.EksperSorumlusuKodu.Value);
                    eksperModel.AnlasmaliServisBedeli = item.AnlasmaliServisBedeli.HasValue ? item.AnlasmaliServisBedeli.Value.ToString("N2") : "";
                    eksperModel.AnlasmaliServisParaBirimi = item.AnlasmaliServisParaBirimi;
                    eksperModel.AnlasmaliServisParaBirimiText = item.AnlasmaliServisParaBirimi.HasValue ? HasarCommon.ParaBirimiText(item.AnlasmaliServisParaBirimi.Value) : "";
                    eksperModel.GelisSaati = item.GelisSaati;
                    eksperModel.OdemeTarihi = item.OdemeTarihi.HasValue ? item.OdemeTarihi.Value.ToString("dd.MM.yyyy") : TurkeyDateTime.Now.ToString("dd.MM.yyyy");
                    eksperModel.AsistansFirma = item.AsistansFirma;
                    eksperModel.AsistansFirmaBedeli = item.AsistansFirmaBedeli.HasValue ? item.AsistansFirmaBedeli.Value.ToString("N2") : "";
                    eksperModel.AsistansFirmaParaBirimi = item.AsistansFirmaParaBirimi;
                    eksperModel.AsistansFirmaParaBirimiText = item.AsistansFirmaParaBirimi.HasValue ? HasarCommon.ParaBirimiText(item.AsistansFirmaParaBirimi.Value) : "";
                    eksperModel.BeklemeSuresi = item.BeklemeSuresi;
                    eksperModel.BildirimTarihi = item.BildirimTarihi.HasValue ? item.BildirimTarihi.Value.ToString("dd.MM.yyyy") : TurkeyDateTime.Now.ToString("dd.MM.yyyy");
                    eksperModel.RedBedeli = item.RedBedeli.HasValue ? item.RedBedeli.Value.ToString("N2") : "";
                    eksperModel.RedParaBirimi = item.RedParaBirimi;
                    eksperModel.RedParaBirimiText = item.AsistansFirmaParaBirimi.HasValue ? HasarCommon.ParaBirimiText(item.RedParaBirimi.Value) : "";
                    eksperModel.RucuBedeli = item.RucuBedeli.HasValue ? item.RucuBedeli.Value.ToString("N2") : "";
                    eksperModel.TahakkukBedeli = item.TahakkukBedeli.HasValue ? item.TahakkukBedeli.Value.ToString("N2") : "";
                    eksperModel.TahakkukParaBirimi = item.TahakkukParaBirimi;
                    eksperModel.TahakkukParaBirimiText = item.AsistansFirmaParaBirimi.HasValue ? HasarCommon.ParaBirimiText(item.TahakkukParaBirimi.Value) : "";
                    eksperModel.TahminiHasarBedeli = item.TahminiHasarBedeli.HasValue ? item.TahminiHasarBedeli.Value.ToString("N2") : "";
                    eksperModel.TahminiHasarParaBirimi = item.TahminiHasarParaBirimi;
                    eksperModel.TahminiHasarParaBirimiText = item.AsistansFirmaParaBirimi.HasValue ? HasarCommon.ParaBirimiText(item.TahminiHasarParaBirimi.Value) : "";

                    model.Items.Add(eksperModel);
                }
            }

            return model;
        }

        public ActionResult EksperGuncelle(int eksperId)
        {
            if (eksperId > 0)
            {
                HasarEksperIslemleri eksperIslemleri = _HasarService.GetHasarEksperIslem(eksperId);

                Mapper.CreateMap<HasarEksperIslemleri, HasarEksperModel>();
                HasarEksperModel model = Mapper.Map<HasarEksperIslemleri, HasarEksperModel>(eksperIslemleri);
                model.TahminiHasarBedeli = eksperIslemleri.TahminiHasarBedeli.ToString();
                model.BildirimTarihi = model.BildirimTarihi.Substring(0, 10);
                model.TahminiHasarParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", model.TahminiHasarParaBirimi);
                model.AnlasmaliServisParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", model.AnlasmaliServisParaBirimi);
                model.TahakkukParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", model.TahakkukParaBirimi);
                model.RucuParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", model.RucuBedeliParaBirimi);
                model.RedParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", model.RedParaBirimi);
                model.AsistansFirmaParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", model.AsistansFirmaParaBirimi);
                var eksperList = _HasarService.GetListEksperler();
                model.EksperList = new SelectList(eksperList, "EksperKodu", "EksperAdSoyadUnvan", "").ListWithOptionLabel();
                var asistansFirmaList = _HasarService.GetListAsistansFirmalari();
                model.AsistansFirmalari = new SelectList(asistansFirmaList, "AsistansKodu", "AsistansAdUnvan", model.AsistansFirma).ListWithOptionLabel();
                return PartialView("_EksperEkle", model);
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult EksperGuncelle(HasarEksperModel model)
        {
            if (String.IsNullOrEmpty(model.AnlasmaliServisBedeli))
            {
                if (ModelState["AnlasmaliServisParaBirimi"] != null)
                    ModelState["AnlasmaliServisParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.AsistansFirmaBedeli))
            {
                if (ModelState["AsistansFirmaParaBirimi"] != null)
                    ModelState["AsistansFirmaParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.RedBedeli))
            {
                if (ModelState["RedParaBirimi"] != null)
                    ModelState["RedParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.RucuBedeli))
            {
                if (ModelState["RucuBedeliParaBirimi"] != null)
                    ModelState["RucuBedeliParaBirimi"].Errors.Clear();
            }

            if (String.IsNullOrEmpty(model.TahakkukBedeli))
            {
                if (ModelState["TahakkukParaBirimi"] != null)
                    ModelState["TahakkukParaBirimi"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                HasarEksperIslemleri eksper = _HasarService.GetHasarEksperIslem(model.EksperId);
                if (eksper != null)
                {
                    eksper.EksperSorumlusuKodu = model.EksperSorumlusuKodu;

                    eksper.AsistansFirma = model.AsistansFirma;

                    eksper.BeklemeSuresi = model.BeklemeSuresi;
                    eksper.BildirimTarihi = !String.IsNullOrEmpty(model.BildirimTarihi) ? Convert.ToDateTime(model.BildirimTarihi) : TurkeyDateTime.Now;
                    eksper.GelisSaati = model.GelisSaati;
                    eksper.OdemeTarihi = !String.IsNullOrEmpty(model.OdemeTarihi) ? Convert.ToDateTime(model.OdemeTarihi) : TurkeyDateTime.Now;

                    eksper.TahminiHasarBedeli = !String.IsNullOrEmpty(model.TahminiHasarBedeli) ? Convert.ToDecimal(model.TahminiHasarBedeli) : 0;
                    eksper.TahminiHasarParaBirimi = model.TahminiHasarParaBirimi;

                    if (!String.IsNullOrEmpty(model.AnlasmaliServisBedeli))
                    {
                        eksper.AnlasmaliServisBedeli = Convert.ToDecimal(model.AnlasmaliServisBedeli);
                        eksper.AnlasmaliServisParaBirimi = model.AnlasmaliServisParaBirimi;
                    }

                    eksper.AsistansFirma = model.AsistansFirma;
                    if (!String.IsNullOrEmpty(model.AsistansFirmaBedeli))
                    {
                        eksper.AsistansFirmaBedeli = Convert.ToDecimal(model.AsistansFirmaBedeli);
                        eksper.AsistansFirmaParaBirimi = model.AsistansFirmaParaBirimi;
                    }

                    if (!String.IsNullOrEmpty(model.RedBedeli))
                    {
                        eksper.RedBedeli = Convert.ToDecimal(model.RedBedeli);
                        eksper.RedParaBirimi = model.RedParaBirimi;
                    }
                    if (!String.IsNullOrEmpty(model.RucuBedeli))
                    {
                        eksper.RucuBedeli = Convert.ToDecimal(model.RucuBedeli);
                        eksper.RucuBedeliParaBirimi = model.RucuBedeliParaBirimi;
                    }
                    if (!String.IsNullOrEmpty(model.TahakkukBedeli))
                    {
                        eksper.TahakkukBedeli = Convert.ToDecimal(model.TahakkukBedeli);
                        eksper.TahakkukParaBirimi = model.TahakkukParaBirimi;
                    }

                    _HasarService.UpdateHasarEksper(eksper);
                    return null;
                }
            }

            var eksperList = _HasarService.GetListEksperler();
            model.EksperList = new SelectList(eksperList, "EksperKodu", "EksperAdSoyadUnvan", "").ListWithOptionLabel();
            var asistansFirmaList = _HasarService.GetListAsistansFirmalari();
            model.AsistansFirmalari = new SelectList(asistansFirmaList, "AsistansKodu", "AsistansAdUnvan").ListWithOptionLabel();
            model.TahminiHasarParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "");
            model.AnlasmaliServisParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "");
            model.TahakkukParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "");
            model.RucuParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "");
            model.RedParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "");
            model.AsistansFirmaParaBirimleri = new SelectList(HasarCommon.HasarParaBirimleri(), "Value", "Text", "");
            return PartialView("_EksperEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult EksperSil(int eksperId, int hasarId)
        {
            _HasarService.DeleteHasarIletisimYetkili(eksperId);
            HasarEksperIslemleriListModel model = EksperlerList(hasarId);

            return PartialView("_IletisimYetkilileri", model);
        }

        #endregion

        #region Banka Hesap İşlemleri

        public ActionResult BankaHesapEkle(int? id)
        {
            HasarBankaHesaplariModel model = new HasarBankaHesaplariModel();
            model.HasarId = id.Value;
            List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
            List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

            model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
            model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();
            return PartialView("_BankaHesaplariEkle", model);
        }


        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BankaHesapEkle(HasarBankaHesaplariModel model)
        {
            if (ModelState.IsValid)
            {
                HasarBankaHesaplari bankahesap = new HasarBankaHesaplari();

                bankahesap.HasarId = model.HasarId;
                bankahesap.BankaAdi = model.BankaAdi;
                bankahesap.HesapAdi = model.HesapAdi;
                bankahesap.HesapNo = model.HesapNo;
                bankahesap.IBAN = model.IBAN;
                bankahesap.SubeAdi = model.SubeAdi;

                _HasarService.CreateHasarBankaHesap(bankahesap);
                return null;
            }


            return PartialView("_BankaHesaplariEkle", model);
        }


        [AjaxException]
        public ActionResult BankaHesapView(int id)
        {
            return PartialView("_BankaHesaplari", BankaHesaplariList(id));
        }
        public HasarBankaHesaplariListModel BankaHesaplariList(int HasarId)
        {
            List<HasarBankaHesaplari> bankaHesapList = _HasarService.GetListBankaHesaplari(HasarId).ToList();
            HasarBankaHesaplariListModel model = new HasarBankaHesaplariListModel();
            model.Items = new List<HasarBankaHesaplariModel>();
            HasarBankaHesaplariModel bankaHesapModel = new HasarBankaHesaplariModel();
            if (bankaHesapList != null)
            {
                foreach (var item in bankaHesapList)
                {
                    bankaHesapModel = new HasarBankaHesaplariModel();
                    bankaHesapModel.HasarId = HasarId;
                    bankaHesapModel.BankaHesapId = item.BankaHesapId;
                    bankaHesapModel.HesapAdi = item.HesapAdi;
                    bankaHesapModel.HesapNo = item.HesapNo;
                    bankaHesapModel.IBAN = item.IBAN;
                    bankaHesapModel.SubeAdi = item.SubeAdi;
                    bankaHesapModel.BankaAdi = item.BankaAdi;
                    model.Items.Add(bankaHesapModel);
                }
            }
            return model;
        }

        public ActionResult BankaHesaplariGuncelle(int hesapId)
        {
            HasarBankaHesaplari bankahesap = _HasarService.GetBankaHesap(hesapId);
            if (bankahesap != null)
            {
                Mapper.CreateMap<HasarBankaHesaplari, HasarBankaHesaplariModel>();
                HasarBankaHesaplariModel model = Mapper.Map<HasarBankaHesaplari, HasarBankaHesaplariModel>(bankahesap);

                List<Bankalar> bankalar = _BankaSubeleri.GetListBanka();
                List<BankaSubeleri> subeler = _BankaSubeleri.GetListBankaSubeleri(model.BankaAdi);

                model.Bankalar = new SelectList(bankalar, "BankaKodu", "BankaAdi", "").ListWithOptionLabel();
                model.Subeler = new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel();
                return PartialView("_BankaHesaplariEkle", model);
            }
            return null;
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult BankaHesaplariGuncelle(HasarBankaHesaplariModel model)
        {
            if (ModelState.IsValid)
            {
                var bankaHesap = _HasarService.GetBankaHesap(model.BankaHesapId);
                bankaHesap.BankaAdi = model.BankaAdi;
                bankaHesap.HesapNo = model.HesapNo;
                bankaHesap.HesapAdi = model.HesapAdi;
                bankaHesap.IBAN = model.IBAN;
                bankaHesap.SubeAdi = model.SubeAdi;
                _HasarService.UpdateBankaHesap(bankaHesap);
                return null;
            }

            return PartialView("_BankaHesaplariEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult BankaHesaplariSil(int bankaHesapId, int hasarId)
        {
            _HasarService.DeleteHasarBankaHesap(bankaHesapId);
            HasarBankaHesaplariListModel model = BankaHesaplariList(hasarId);

            return PartialView("_BankaHesaplari", model);
        }

        public ActionResult GetSubeler(string Banka)
        {
            List<BankaSubeleri> subeler = new List<BankaSubeleri>();

            if (Banka != null) subeler = _BankaSubeleri.GetListBankaSubeleri(Banka);

            return Json(new SelectList(subeler, "Sube", "Sube", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        public ActionResult IletisimYetkilileriEkle(int? id)
        {
            HasarIletisimYetkilileriModel model = new HasarIletisimYetkilileriModel();
            model.HasarId = id.Value;
            model.TelefonTipleri = new SelectList(TelefonTipler.TelefonTipleri(), "Value", "Text", model.TelefonTipi);

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult IletisimYetkilileriEkle(HasarIletisimYetkilileriModel model)
        {
            if (ModelState.IsValid)
            {
                HasarIletisimYetkilileri iletisimYetkili = new HasarIletisimYetkilileri();

                iletisimYetkili.HasarId = model.HasarId;
                iletisimYetkili.Email = model.Email;
                iletisimYetkili.Gorevi = model.Gorevi;
                iletisimYetkili.TelefonNo = model.TelefonNo;
                iletisimYetkili.TelefonTipi = model.TelefonTipi;
                iletisimYetkili.GorusulenKisi = model.GorusulenKisi;

                _HasarService.CreateHasarIletisimYetkilileri(iletisimYetkili);
                return null;
            }

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        public ActionResult IletisimYetkilileriGuncelle(int iletisimId)
        {
            HasarIletisimYetkilileri iletisimYetkili = _HasarService.GetTVMIletisimYetkili(iletisimId);

            Mapper.CreateMap<HasarIletisimYetkilileri, HasarIletisimYetkilileriModel>();
            HasarIletisimYetkilileriModel model = Mapper.Map<HasarIletisimYetkilileri, HasarIletisimYetkilileriModel>(iletisimYetkili);

            model.TelefonTipleri = new SelectList(TelefonTipler.TelefonTipleri(), "Value", "Text", model.TelefonTipi);

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult IletisimYetkilileriGuncelle(HasarIletisimYetkilileriModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var iletisimYetkili = _HasarService.GetIletisimYetkilileriDetay(model.IletisimYetkiliId);
                    iletisimYetkili.Email = model.Email;
                    iletisimYetkili.Gorevi = model.Gorevi;
                    iletisimYetkili.GorusulenKisi = model.GorusulenKisi;
                    iletisimYetkili.TelefonNo = model.TelefonNo;
                    iletisimYetkili.TelefonTipi = model.TelefonTipi;

                    _HasarService.UpdateHasarIletisimYetkili(iletisimYetkili);
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return PartialView("_IletisimYetkilileriEkle", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult IletisimYetkilileriSil(int iletisimId, int HasarId)
        {
            _HasarService.DeleteHasarIletisimYetkili(iletisimId);
            HasarIletisimYetkilileriListModel model = HasarIletisimYetkilileriList(HasarId);

            return PartialView("_IletisimYetkilileri", model);
        }

        [AjaxException]
        public ActionResult IletisimYetkiView(int id)
        {
            return PartialView("_IletisimYetkilileri", HasarIletisimYetkilileriList(id));
        }

        public HasarIletisimYetkilileriListModel HasarIletisimYetkilileriList(int HasarId)
        {
            List<HasarIletisimYetkilileri> iletisimYetkilileriList = _HasarService.GetListIletisimYetkilileri(HasarId).ToList();
            HasarIletisimYetkilileriListModel model = new HasarIletisimYetkilileriListModel();
            model.Items = new List<HasarIletisimYetkilileriModel>();
            HasarIletisimYetkilileriModel iletisimYetkilileriModel = new HasarIletisimYetkilileriModel();
            if (iletisimYetkilileriList != null)
            {
                foreach (var item in iletisimYetkilileriList)
                {
                    iletisimYetkilileriModel = new HasarIletisimYetkilileriModel();
                    iletisimYetkilileriModel.IletisimYetkiliId = item.IletisimYetkiliId;
                    iletisimYetkilileriModel.HasarId = item.HasarId;
                    iletisimYetkilileriModel.GorusulenKisi = item.GorusulenKisi;
                    iletisimYetkilileriModel.Email = item.Email;
                    iletisimYetkilileriModel.Gorevi = item.Gorevi;
                    iletisimYetkilileriModel.TelefonNo = item.TelefonNo;
                    iletisimYetkilileriModel.TelefonTipi = item.TelefonTipi;
                    model.Items.Add(iletisimYetkilileriModel);
                }
            }
            return model;
        }

        public HasarModel HasarGirisiDoldur(int policeId)
        {
            HasarModel model = new HasarModel();
            var hasarPoliceBilgi = _PoliceService.GetPoliceById(policeId);
            var anlasmaliServisList = _HasarService.GetListAnlasmaliServisler();
            model.NotlarList = new HasarNotlarListModel();
            model.NotlarList.Items = new List<HasarNotlarModel>();
            model.EksperList = new HasarEksperIslemleriListModel();
            model.EksperList.Items = new List<HasarEksperModel>();
            model.ZorunluEvrakList = new HasarZorunluEvrakListModel();
            model.ZorunluEvrakList.Items = new List<ZorunluEvrakModel>();
            model.BankaHesaplariList = new HasarBankaHesaplariListModel();
            model.BankaHesaplariList.Items = new List<HasarBankaHesaplariModel>();
            model.IletisimYetkilileriList = new HasarIletisimYetkilileriListModel();
            model.IletisimYetkilileriList.Items = new List<HasarIletisimYetkilileriModel>();
            model.HasarDosyaDurumlari = new SelectList(HasarCommon.HasarDurumlari(), "Value", "Text", model.HasarDosyaDurumu);
            model.AnlasmaliServisler = new SelectList(anlasmaliServisList, "Kodu", "Unvani", "").ListWithOptionLabel();

            if (hasarPoliceBilgi != null)
            {
                model.PoliceId = hasarPoliceBilgi.GenelBilgiler.PoliceId;
                model.PoliceNo = hasarPoliceBilgi.GenelBilgiler.PoliceNumarasi;
                model.PlakaNo = hasarPoliceBilgi.GenelBilgiler.PoliceArac.PlakaKodu + hasarPoliceBilgi.GenelBilgiler.PoliceArac.PlakaNo;
                model.UrunKodu = hasarPoliceBilgi.GenelBilgiler.UrunKodu;
                model.UrunAdi = hasarPoliceBilgi.GenelBilgiler.UrunAdi;
                model.BransKodu = hasarPoliceBilgi.GenelBilgiler.BransKodu;
                model.BransAdi = hasarPoliceBilgi.GenelBilgiler.BransAdi;
                model.YenilemeNo = hasarPoliceBilgi.GenelBilgiler.YenilemeNo;
                model.EkNo = hasarPoliceBilgi.GenelBilgiler.EkNo;
                model.SigortaSirketKodu = hasarPoliceBilgi.GenelBilgiler.TUMBirlikKodu;
                model.SigortaSirketi = hasarPoliceBilgi.GenelBilgiler.SigortaSirketleri.SirketAdi;
                model.AcenteKodu = hasarPoliceBilgi.GenelBilgiler.TVMKodu;
                if (hasarPoliceBilgi.GenelBilgiler.TVMKodu.HasValue)
                {
                    var tvmDetay = _TVMService.GetDetay(hasarPoliceBilgi.GenelBilgiler.TVMKodu.Value);
                    if (tvmDetay != null)
                    {
                        model.AcenteUnvani = tvmDetay.Unvani;
                    }
                }

                model.SigortaliUnvani = hasarPoliceBilgi.GenelBilgiler.PoliceSigortali.AdiUnvan;
                model.SigortaliTCVKN = hasarPoliceBilgi.GenelBilgiler.PoliceSigortali.KimlikNo;
                model.TaliAcenteKodu = hasarPoliceBilgi.GenelBilgiler.TaliAcenteKodu;

                if (hasarPoliceBilgi.GenelBilgiler.TaliAcenteKodu.HasValue)
                {
                    var tvmDetay = _TVMService.GetDetay(hasarPoliceBilgi.GenelBilgiler.TaliAcenteKodu.Value);
                    if (tvmDetay != null)
                    {
                        model.TaliAcenteAdi = tvmDetay.Unvani;
                    }
                }

                var policeHasar = _HasarService.GetHasarDetay(policeId);
                if (policeHasar != null)
                {
                    HasarEksperModel hasarEksper = new HasarEksperModel();
                    model.HasarDosyaNo = policeHasar.HasarDosyaNo;
                    model.HasarDosyaDurumu = policeHasar.HasarDosyaDurumu.HasValue ? policeHasar.HasarDosyaDurumu.Value.ToString() : "1";
                    model.HasarDosyaRedNedeni = policeHasar.RedNedeni;
                    model.HasarMevki = policeHasar.HasarMevki;
                    model.HasarSaati = policeHasar.HasarSaati;
                    model.HasarTarihi = policeHasar.HasarTarihi;
                    model.HasarTuruNedeni = policeHasar.HasarTuruNedeni;
                    model.IhbarTarihi = policeHasar.IhbarTarihi;
                    model.HasarId = policeHasar.HasarId;

                    if (policeHasar.AnlasmasiServisKodu != null)
                    {
                        model.AnlasmaliServisVarMi = true;
                        model.AnlasmaliServisKodu = policeHasar.AnlasmasiServisKodu;
                        model.AnlasmaliServisler = new SelectList(anlasmaliServisList, "Kodu", "Unvani", model.AnlasmaliServisKodu).ListWithOptionLabel();
                    }

                    if (policeHasar.HasarEksperIslemleris.Count > 0)
                    {
                        model.EksperList = EksperlerList(policeHasar.HasarId);
                    }
                    if (policeHasar.HasarNotlaris.Count > 0)
                    {
                        model.NotlarList = NotlarList(policeHasar.HasarId);
                    }
                    if (policeHasar.HasarZorunluEvraklaris.Count > 0)
                    {
                        model.ZorunluEvrakList.Items = HasarEvraklariGetir(policeHasar.HasarId);
                    }
                    if (policeHasar.HasarBankaHesaplaris.Count > 0)
                    {
                        model.BankaHesaplariList = BankaHesaplariList(policeHasar.HasarId);
                    }
                    if (policeHasar.HasarIletisimYetkilileris.Count > 0)
                    {
                        model.IletisimYetkilileriList = HasarIletisimYetkilileriList(policeHasar.HasarId);
                    }

                }
            }



            return model;
        }


    }
}
