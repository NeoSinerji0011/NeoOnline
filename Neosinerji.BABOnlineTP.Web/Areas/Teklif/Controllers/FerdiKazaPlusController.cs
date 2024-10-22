using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Business.Common.Metlife;
using Neosinerji.BABOnlineTP.Business.METLIFE;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Database;
using System.Threading;
using Newtonsoft.Json;
using System.Linq.Expressions;
using AutoMapper;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.FerdiKazaPlus)]
    public class FerdiKazaPlusController : TeklifController
    {
        IMusteriDokumanStorage _Storage;
        ITVMContext _TVMContext;
        public FerdiKazaPlusController(ITVMService tvmService,
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
                              IMusteriDokumanStorage storage,
                              ITVMContext tvm)
            : base(tvmService, teklifService, musteriService, kullaniciService, aktifKullaniciService, tanimService,
            ulkeService, crService, aracService, urunService, tumService)
        {
            _Storage = storage;
            _TVMContext = tvm;
            _TeklifService = teklifService;
        }

        public ActionResult IsAra()
        {
            try
            {
                IsTakipListeEkranModel model = new IsTakipListeEkranModel();
                model.HareketTipleri = new SelectList(FerdiKazaPlus.HareketTipleri(), "Value", "Text", "").ToList();
                model.IsTipleri = new SelectList(FerdiKazaPlus.IsTipleri(), "Value", "Text", "").ToList();
                return View(model);

            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult IsAraPager()
        {
            try
            {
                if (Request["sEcho"] != null)
                {
                    IsTakipDetayArama arama = new IsTakipDetayArama(Request, new Expression<Func<IsTakipDetayListeModel, object>>[]
                                                                    {
                                                                        t => t.IsNo,
                                                                        t => t.IsTipi,
                                                                        t => t.KullaniciAdiSoyadi,
                                                                        t => t.HareketTipi,
                                                                        t => t.KayitTarihi,

                                                                    });
                    arama.IsNo = arama.TryParseParamString("IsNo");
                    arama.IsTipi = arama.TryParseParamString("IsTipi");
                    arama.HareketTipi = arama.TryParseParamString("HareketTipi");
                    arama.BaslangicTarihi = arama.TryParseParamString("BaslangicTarihi");
                    arama.BitisTarihi = arama.TryParseParamString("BitisTarihi");


                    int totalRowCount = 0;
                    List<IsTakipDetayListeModel> list = _TeklifService.IsTakipDetayPagedList(arama, out totalRowCount);


                    DataTableList result = arama.Prepare(list, totalRowCount);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return null;
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }


        }

        public ActionResult Islerim()
        {
            IslerimListeModel model = _TeklifService.GetIslerim(_AktifKullaniciService.KullaniciKodu);

            return View(model);
        }

        public ActionResult Onayladiklarim()
        {
            OnayladiklarimListeModel model = _TeklifService.GetOnayladiklarim(_AktifKullaniciService.KullaniciKodu);
            return View(model);
        }

        public ActionResult Ekle(int? id)
        {
            FerdiKazaPlusModel model = EkleModel(id, null);

            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            FerdiKazaPlusModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public FerdiKazaPlusModel EkleModel(int? id, int? teklifId)
        {
            FerdiKazaPlusModel model = new FerdiKazaPlusModel();
            try
            {
                #region Teklif Genel


                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Visit();
                ITeklif teklif = null;

                #endregion

                #region Teklif Hazırlayan Müşteri / Sigorta ettiren
                int? sigortaliMusteriKodu = null;

                //Teklifi hazırlayan
                model.Hazirlayan = base.EkleHazirlayanModel();

                //Sigorta Ettiren / Sigortalı
                model.Musteri = new SigortaliModel();
                model.Musteri.SigortaliAyni = true;

                if (teklifId.HasValue)
                {
                    model.TekrarTeklif = true;
                    teklif = _TeklifService.GetTeklif(teklifId.Value);
                    id = teklif.SigortaEttiren.MusteriKodu;

                    TeklifSigortali sigortali = teklif.Sigortalilar.FirstOrDefault();
                    if (sigortali != null)
                    {
                        if (id != sigortali.MusteriKodu)
                        {
                            model.Musteri.SigortaliAyni = false;
                            sigortaliMusteriKodu = sigortali.MusteriKodu;
                        }
                    }
                }

                List<SelectListItem> ulkeler = new List<SelectListItem>();
                ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });

                if (id.HasValue)
                {
                    model.Musteri.SigortaEttiren = base.EkleMusteriModel(id.Value);
                    model.Musteri.Ulkeler = ulkeler;
                    model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.Musteri.SigortaEttiren.IlKodu).ListWithOptionLabelIller();
                    model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.Musteri.SigortaEttiren.IlKodu), "IlceKodu", "IlceAdi", model.Musteri.SigortaEttiren.IlceKodu).ListWithOptionLabel();
                }
                else
                {
                    model.Musteri.SigortaEttiren = new MusteriModel();
                    model.Musteri.SigortaEttiren.UlkeKodu = "TUR";
                    model.Musteri.SigortaEttiren.Cinsiyet = "E";
                    model.Musteri.SigortaEttiren.CepTelefonu = "90";
                    model.Musteri.Ulkeler = ulkeler;
                    model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", "34").ListWithOptionLabelIller();
                    model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", "34"), "IlceKodu", "IlceAdi").ListWithOptionLabel();
                }

                if (sigortaliMusteriKodu.HasValue)
                {
                    model.Musteri.Sigortali = base.EkleMusteriModel(sigortaliMusteriKodu.Value);

                    model.Musteri.Ulkeler = ulkeler;
                    model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.Musteri.SigortaEttiren.IlKodu).ListWithOptionLabelIller();
                    model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.Musteri.SigortaEttiren.IlKodu), "IlceKodu", "IlceAdi", model.Musteri.SigortaEttiren.IlceKodu).ListWithOptionLabel();

                }
                else
                {
                    model.Musteri.Sigortali = new MusteriModel();
                    model.Musteri.Sigortali.UlkeKodu = "TUR";
                    model.Musteri.Sigortali.Cinsiyet = "E";
                }
                model.Musteri.MusteriTipleri = nsmusteri.MusteriListProvider.MusteriTipleri();
                model.Musteri.UyrukTipleri = new SelectList(nsmusteri.MusteriListProvider.UyrukTipleri(), "Value", "Text", "0");
                model.Musteri.CinsiyetTipleri = new SelectList(nsmusteri.MusteriListProvider.CinsiyetTipleri(), "Value", "Text");
                model.Musteri.GelirVergisiTipleri = new SelectList(TeklifProvider.GelirVergisiTipleriAEGON(), "Value", "Text", "");

                model.Musteri.CinsiyetTipleri.First().Selected = true;
                #endregion

                #region Ferdi Kaza Plus
                model.Sigortali = FerdiKazaPlusSigortaliBilgileri(teklifId, teklif);
                model.Teminatlar = FerdiKazaPlusTeminatlar(teklifId, teklif);
                model.Lehtar = LehtarBilgileri(teklifId, teklif);
                model.Iletisim = IletisimBilgieri(teklifId, teklif);
                model.PrimOdeme = PrimOdeyenBilgileri(teklifId, teklif);
                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.FerdiKazaPlus);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme

                model.Odeme = new FerdiKazaPlusTeklifOdemeModel();
                model.Odeme.OdemeSekli = true;
                model.Odeme.OdemeTipi = 1;
                model.Odeme.Yenilensin = false;
                model.Odeme.TaksitSayilari = new List<SelectListItem>();
                model.Odeme.OdemeTipleri = new SelectList(TeklifProvider.KritikHastalikOdemeTipleri(), "Value", "Text", "2").ToList();

                model.Odeme.TaksitSayilari.AddRange(
                                 new SelectListItem[]{
                                     new SelectListItem() { Text = "2", Value = "2" },
                                    new SelectListItem() { Text = "3", Value = "3" },
                                    new SelectListItem() { Text = "4", Value = "4" },
                                    new SelectListItem() { Text = "5", Value = "5" },
                                    new SelectListItem() { Text = "6", Value = "6" },
                                    new SelectListItem() { Text = "7", Value = "7" },
                                    new SelectListItem() { Text = "8", Value = "8" },
                                    new SelectListItem() { Text = "9", Value = "9" }});

                model.KrediKarti = new KrediKartiOdemeModel();
                model.KrediKarti.KK_TeklifId = 0;
                model.KrediKarti.Tutar = 0;
                model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
                model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
                model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
                model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

                List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
                odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash,Value="1"},
                new SelectListItem(){Text=babonline.Forward,Value="2"}
                    });
                model.KrediKarti.OdemeSekilleri = odemeSekilleri;

                model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text").ToList();

                model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
                List<SelectListItem> taksitSeceneleri = new List<SelectListItem>();
                taksitSeceneleri.AddRange(
                    new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});
                model.KrediKarti.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.KrediKarti.TaksitSayisi).ToList();

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }
            return model;
        }

        public ActionResult Detay(int id)
        {
            DetayFerdiKazaPlusModel model = new DetayFerdiKazaPlusModel();
            try
            {

                #region Teklif Genel

                FerdiKazaPlusTeklif ferdiKazaPlusTeklif = new FerdiKazaPlusTeklif(id);
                model.TeklifId = id;
                model.TeklifNo = ferdiKazaPlusTeklif.Teklif.TeklifNo.ToString();

                #endregion

                #region Teklif Hazırlayan

                //Teklifi hazırlayan
                model.Hazirlayan = base.DetayHazirlayanModel(ferdiKazaPlusTeklif.Teklif);
                //Sigorta Ettiren
                model.SigortaEttiren = base.AegonDetayMusteriModel(ferdiKazaPlusTeklif.Teklif.SigortaEttiren.MusteriKodu);

                #endregion

                #region Ferdi Kaza Plus
                model.Sigortalilar = FerdiKazaPlusSigortaliBilgileriDetay(ferdiKazaPlusTeklif.Teklif, model.SigortaEttiren.KimlikNo);
                model.Teminatlar = FerdiKazaPlusTeminatlarDetay(ferdiKazaPlusTeklif.Teklif);
                model.PrimOdeme = PrimOdeyenBilgileriDetay(ferdiKazaPlusTeklif.Teklif);
                model.Iletisim = IletisimBilgieriDetay(ferdiKazaPlusTeklif.Teklif);
                #endregion

                #region Teklif Fiyat

                model.Fiyat = FerdiKazaPlusFiyat(ferdiKazaPlusTeklif);
                model.KrediKarti = new KrediKartiOdemeModel();
                model.KrediKarti.KK_TeklifId = 0;
                model.KrediKarti.Tutar = 0;
                model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
                model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
                model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
                model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
                model.KrediKarti.KK_OdemeSekli = ferdiKazaPlusTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
                model.KrediKarti.KK_OdemeTipi = ferdiKazaPlusTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

                List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
                odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="PEŞİN" , Value="1"},
                new SelectListItem(){Text="VADELİ" , Value="2"}
            });
                model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

                model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

                model.KrediKarti.TaksitSayisi = ferdiKazaPlusTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
                model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
                List<SelectListItem> taksitSeceneleri = new List<SelectListItem>();
                taksitSeceneleri.AddRange(
                    new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});
                model.KrediKarti.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.KrediKarti.TaksitSayisi).ToList();

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return View(model);
        }

        public ActionResult IsDetay(int? adim, int isTakipId)
        {
            try
            {
                FerdiKazaPlusIsDetayModel Model = new FerdiKazaPlusIsDetayModel();
                MusteriGenelBilgiler musteri = null;
                IsTakip isTakip = _TeklifService.IsTakipGet(isTakipId);

                string kgrup = String.Empty;

                Model.Adim = 2;

                switch (adim)
                {
                    case 2:
                        {
                            Model.Adim = 2;
                            kgrup = MetlifeKullaniciGruplari.MetlifeKullaniciGruplariText(MetlifeKullaniciGruplari.MetlifeOperasyon.ToString());
                            break;
                        }
                    case 3:
                        {
                            Model.Adim = 3;
                            kgrup = MetlifeKullaniciGruplari.MetlifeKullaniciGruplariText(MetlifeKullaniciGruplari.SatisEkibi.ToString());
                            break;
                        }
                    case 4:
                        {
                            Model.Adim = 4;
                            kgrup = MetlifeKullaniciGruplari.MetlifeKullaniciGruplariText(MetlifeKullaniciGruplari.MetlifeOperasyon.ToString());
                            break;
                        }
                    case 5: Model.Adim = 5; break;
                }
                if (isTakip != null)
                {
                    musteri = _MusteriService.GetMusteri(isTakip.MusteriKodu);

                    Model.Tarih = isTakip.KayitTarihi;
                    Model.IsNo = isTakipId;
                    Model.AdiSoyadi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                    Model.IliskiliBelge = isTakip.IsTakipId.ToString() + " Numaralı başvuru";
                    Model.TeklifId = isTakip.TeklifId;

                    if (!String.IsNullOrEmpty(_AktifKullaniciService.AdiSoyadi))
                        Model.TVMKullaniciAdiSoyadi = _AktifKullaniciService.AdiSoyadi;

                    if (kgrup != null)
                        Model.KullaniciGrubu = kgrup;

                    Model.IsTipi = "Sigorta Başvuru Süreci";
                    switch (adim)
                    {
                        case 2: { Model.IsTipiDetay = "Başvuru Süreci 2. adım"; break; }
                        case 3: { Model.IsTipiDetay = "Başvuru Süreci 3. adım"; break; }
                        case 4: { Model.IsTipiDetay = "Başvuru Süreci 4. adım"; break; }

                    }

                }
                Model.IsDetayDokuman = DokumanList(isTakip.IsTakipId);

                Model.IsTakipTarihce = new List<IsTakipTarihce>();
                var tarihceler = _TeklifService.GetListTarihceler(Model.TeklifId);

                foreach (var item in tarihceler)
                {
                    IsTakipTarihce tarihce = new IsTakipTarihce();
                    tarihce.IsNo = item.IsTakipDetayId;
                    tarihce.IsTipi = item.IsTipiDetayId;
                    TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(s => s.KullaniciKodu == item.TvmKullaniciId).FirstOrDefault();
                    tarihce.TVMKullaniciAdiSoyadi = kullanici.Adi + " " + kullanici.Soyadi;
                    tarihce.HareketTipi = item.HareketTipi.ToString(); ;
                    tarihce.KayitTarihi = item.KayitTarihi;

                    Model.IsTakipTarihce.Add(tarihce);

                }

                return View(Model);
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        public ActionResult Hesapla(FerdiKazaPlusModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (model.Lehtar != null)
                {
                    if (model.Lehtar.Lehtar == 1 || model.Lehtar.Lehtar == 0)
                    {
                        if (model.Lehtar.kisiSayisi > 0 & model.Lehtar.kisiSayisi < 6)
                        {
                            if (model.Lehtar.kisiSayisi == 1)
                            {
                                if (ModelState["Lehtar.LehterList[0].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[0].DogumTarihi"].Errors.Clear();
                            }

                            if (model.Lehtar.kisiSayisi == 1 || model.Lehtar.kisiSayisi == 2)
                            {

                                if (ModelState["Lehtar.LehterList[0].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[0].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[1].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[1].DogumTarihi"].Errors.Clear();

                            }

                            if (model.Lehtar.kisiSayisi == 1 || model.Lehtar.kisiSayisi == 2 || model.Lehtar.kisiSayisi == 3)
                            {
                                if (ModelState["Lehtar.LehterList[0].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[0].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[1].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[1].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[2].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[2].DogumTarihi"].Errors.Clear();

                            }
                            if (model.Lehtar.kisiSayisi == 1 || model.Lehtar.kisiSayisi == 2 || model.Lehtar.kisiSayisi == 3 || model.Lehtar.kisiSayisi == 4)
                            {
                                if (ModelState["Lehtar.LehterList[0].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[0].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[1].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[1].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[2].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[2].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[3].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[3].DogumTarihi"].Errors.Clear();

                            }

                            if (model.Lehtar.kisiSayisi == 1 || model.Lehtar.kisiSayisi == 2 || model.Lehtar.kisiSayisi == 3 || model.Lehtar.kisiSayisi == 4)
                            {
                                if (ModelState["Lehtar.LehterList[0].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[0].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[1].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[1].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[2].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[2].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[3].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[3].DogumTarihi"].Errors.Clear();

                                if (ModelState["Lehtar.LehterList[4].DogumTarihi"] != null)
                                    ModelState["Lehtar.LehterList[4].DogumTarihi"].Errors.Clear();
                            }
                        }
                        if (model.PrimOdeme != null)
                        {
                            if (ModelState["PrimOdeme.KartNo.KK1"] != null)
                                ModelState["PrimOdeme.KartNo.KK1"].Errors.Clear();

                            if (ModelState["PrimOdeme.KartNo.KK2"] != null)
                                ModelState["PrimOdeme.KartNo.KK2"].Errors.Clear();

                            if (ModelState["PrimOdeme.KartNo.KK3"] != null)
                                ModelState["PrimOdeme.KartNo.KK3"].Errors.Clear();

                            if (ModelState["PrimOdeme.KartNo.KK4"] != null)
                                ModelState["PrimOdeme.KartNo.KK4"].Errors.Clear();

                            if (ModelState["PrimOdeme.KartNo"] != null)
                                ModelState["PrimOdeme.KartNo"].Errors.Clear();

                            if (ModelState["PrimOdeme.SigortaBaslangicTarihi"] != null)
                                ModelState["PrimOdeme.SigortaBaslangicTarihi"].Errors.Clear();
                        }

                        if (model.Sigortali != null)
                        {
                            if (ModelState["Sigortali.DogumTarihi"] != null)
                                ModelState["Sigortali.DogumTarihi"].Errors.Clear();

                        }
                    }

                }

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            #endregion

            #region Kredi Kartı

            string KrediKartiNo = String.Empty;

            if (Request.Params["PrimOdeme.KartNo.KK1"] != null)
                KrediKartiNo += Request.Params["PrimOdeme.KartNo.KK1"].ToString();

            if (Request.Params["PrimOdeme.KartNo.KK2"] != null)
                KrediKartiNo += Request.Params["PrimOdeme.KartNo.KK2"].ToString();

            if (Request.Params["PrimOdeme.KartNo.KK3"] != null)
                KrediKartiNo += Request.Params["PrimOdeme.KartNo.KK3"].ToString();

            if (Request.Params["PrimOdeme.KartNo.KK4"] != null)
                KrediKartiNo += Request.Params["PrimOdeme.KartNo.KK4"].ToString();


            #endregion

            #region Teklif kaydı ve hesaplamanın başlatılması
            if (ModelState.IsValid)
            {
                try
                {
                    string KimlikNo = model.Sigortali.TCKimlikNo.ToString();
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(KimlikNo, _AktifKullaniciService.TVMKodu);
                    List<SelectListItem> ulkeler = new List<SelectListItem>();
                    ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });
                    model.Hazirlayan = base.EkleHazirlayanModel();
                    int musteriKodu = musteri.MusteriKodu;
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.FerdiKazaPlus,
                                                model.Hazirlayan.TVMKodu, model.Hazirlayan.TVMKullaniciKodu, musteriKodu, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Sigortali

                    teklif.AddSigortali(musteriKodu);

                    #endregion

                    #region Ferdi Kaza Plus
                    #region Sigortalı Bilgileri

                    teklif.AddSoru(FerdiKazaPlusSorular.Urun_Adi, model.Sigortali.UrunAd.ToString());
                    teklif.AddSoru(FerdiKazaPlusSorular.Meslek_Grubu, model.Sigortali.MeslekGrubu.ToString());
                    teklif.AddSoru(FerdiKazaPlusSorular.Sigorta_Suresi, model.Sigortali.SigortaSuresi.ToString());

                    #endregion

                    #region Teminat Kapsamı
                    if (model.Teminatlar.teminatTutari != null)
                        teklif.AddSoru(FerdiKazaPlusSorular.Teminat_Tutari, model.Teminatlar.teminatTutari.ToString());

                    if (model.Teminatlar.OdemeSecenegi != null)
                        teklif.AddSoru(FerdiKazaPlusSorular.Odeme_Secenegi, model.Teminatlar.OdemeSecenegi.ToString());

                    if (model.Teminatlar.KazaSonucuVefatBedeli.HasValue)
                        teklif.AddSoru(FerdiKazaPlusSorular.Kaza_Sonucu_Vefat, model.Teminatlar.KazaSonucuVefatBedeli.ToString());

                    if (model.Teminatlar.kazaSonucuMaluliyetBedeli.HasValue)
                        teklif.AddSoru(FerdiKazaPlusSorular.KazaSonucuMaluliyet, model.Teminatlar.kazaSonucuMaluliyetBedeli.ToString());

                    teklif.AddSoru(FerdiKazaPlusSorular.Asistans_Hizmeti, model.Teminatlar.asistansHizmeti);

                    if (model.Teminatlar.SigortaPrimTutari.HasValue)
                        teklif.AddSoru(FerdiKazaPlusSorular.Sigorta_Prim_Tutari, model.Teminatlar.SigortaPrimTutari.ToString());

                    if (model.Teminatlar.taksitSayisi.HasValue)
                        teklif.AddSoru(FerdiKazaPlusSorular.Taksit_Sayisi, model.Teminatlar.taksitSayisi.ToString());

                    #endregion

                    #region Lehtar

                    if (model.Lehtar.Lehtar == 1) teklif.AddSoru(FerdiKazaPlusSorular.Lehtar_Tipi, model.Lehtar.Lehtar.ToString());
                    else
                    {
                        teklif.AddSoru(FerdiKazaPlusSorular.Lehtar_Tipi, model.Lehtar.Lehtar);

                        if (model.Lehtar.kisiSayisi > 0)
                            teklif.AddSoru(FerdiKazaPlusSorular.Kisi_Sayisi, model.Lehtar.kisiSayisi.ToString());

                        if (model.Lehtar.kisiSayisi > 0 & model.Lehtar.kisiSayisi < 6)
                        {
                            var sigortali1 = JsonConvert.SerializeObject(model.Lehtar.LehterList[0]);
                            teklif.AddSoru(FerdiKazaPlusSorular.Sigortali_1, sigortali1);
                        }

                        if (model.Lehtar.kisiSayisi > 1 & model.Lehtar.kisiSayisi < 6)
                        {
                            var sigortali2 = JsonConvert.SerializeObject(model.Lehtar.LehterList[1]);
                            teklif.AddSoru(FerdiKazaPlusSorular.Sigortali_3, sigortali2);
                        }

                        if (model.Lehtar.kisiSayisi > 2 & model.Lehtar.kisiSayisi < 6)
                        {
                            var sigortali3 = JsonConvert.SerializeObject(model.Lehtar.LehterList[2]);
                            teklif.AddSoru(FerdiKazaPlusSorular.Sigortali_3, sigortali3);
                        }
                        if (model.Lehtar.kisiSayisi > 3 & model.Lehtar.kisiSayisi < 6)
                        {
                            var sigortali4 = JsonConvert.SerializeObject(model.Lehtar.LehterList[3]);
                            teklif.AddSoru(FerdiKazaPlusSorular.Sigortali_4, sigortali4);
                        }
                        if (model.Lehtar.kisiSayisi == 5)
                        {
                            var sigortali5 = JsonConvert.SerializeObject(model.Lehtar.LehterList[4]);
                            teklif.AddSoru(FerdiKazaPlusSorular.Sigortali_5, sigortali5);
                        }
                    }
                    #endregion

                    #region Prim Ödeyen Bilgileri

                    if (model.PrimOdeme.KartNoMevcut)
                    {
                        teklif.AddSoru(FerdiKazaPlusSorular.SigortaBaslangicTarihi, model.PrimOdeme.SigortaBaslangicTarihi);

                        teklif.AddSoru(FerdiKazaPlusSorular.KartNo, "1234123412341234");
                        teklif.AddSoru(FerdiKazaPlusSorular.SKT_ay, "12");
                        teklif.AddSoru(FerdiKazaPlusSorular.SKT_yil, "2017");
                    }
                    else
                    {
                        teklif.AddSoru(FerdiKazaPlusSorular.KartNo, KrediKartiNo);
                        teklif.AddSoru(FerdiKazaPlusSorular.SKT_ay, model.PrimOdeme.ay);
                        teklif.AddSoru(FerdiKazaPlusSorular.SKT_yil, model.PrimOdeme.yil);
                    }

                    #endregion

                    #region İletişim

                    SigortaliBilgileriModel sigortali = model.Sigortali;
                    IletisimBilgieriModel iletisim = model.Iletisim;
                    MusteriGenelBilgiler KayitliMusteri = _MusteriService.GetMusteri(sigortali.TCKimlikNo, _AktifKullaniciService.TVMKodu);

                    if (!model.Iletisim.AdresMevcut)
                    {
                        if (!String.IsNullOrEmpty(model.Iletisim.Adres))
                            teklif.AddSoru(FerdiKazaPlusSorular.Adres, model.Iletisim.Adres);

                        teklif.AddSoru(FerdiKazaPlusSorular.AdresTipi, model.Iletisim.AdresTipi.ToString());
                    }
                    else
                    {

                        if (KayitliMusteri != null)
                        {
                            MusteriAdre adres = KayitliMusteri.MusteriAdres.FirstOrDefault(s => s.MusteriKodu == sigortali.MusteriNo);
                            if (adres != null)
                            {
                                if (!String.IsNullOrEmpty(adres.Cadde))
                                    iletisim.Adres = adres.Cadde + " Cad.";
                                if (!String.IsNullOrEmpty(adres.Mahalle))
                                    iletisim.Adres += adres.Mahalle + " Mah.";
                                if (!String.IsNullOrEmpty(adres.Sokak))
                                    iletisim.Adres += adres.Sokak + " Sk.";
                                if (!String.IsNullOrEmpty(adres.HanAptFab))
                                    iletisim.Adres += adres.HanAptFab + " Apt.";
                                if (!String.IsNullOrEmpty(adres.BinaNo))
                                    iletisim.Adres += " No." + adres.BinaNo;
                                if (!String.IsNullOrEmpty(adres.DaireNo))
                                    iletisim.Adres += " D." + adres.DaireNo;

                                if (adres.PostaKodu > 0)
                                    iletisim.Adres += adres.PostaKodu;

                                if (!String.IsNullOrEmpty(adres.Semt))
                                    iletisim.Adres += adres.Semt + " /";

                            }
                            teklif.AddSoru(FerdiKazaPlusSorular.Adres, iletisim.Adres);
                            if (adres.AdresTipi != null)
                                teklif.AddSoru(FerdiKazaPlusSorular.AdresTipi, adres.AdresTipi.ToString());

                        }
                    }
                    if (!model.Iletisim.EmailMevcut)
                    {
                        if (!String.IsNullOrEmpty(model.Iletisim.Email))
                            teklif.AddSoru(FerdiKazaPlusSorular.Email, model.Iletisim.Email);
                    }
                    else
                    {
                        if (KayitliMusteri != null)
                        {
                            teklif.AddSoru(FerdiKazaPlusSorular.Email, KayitliMusteri.EMail);
                        }
                    }
                    if (!model.Iletisim.Tel1Mevcut)
                    {
                        if (!String.IsNullOrEmpty(model.Iletisim.Tel1))
                            teklif.AddSoru(FerdiKazaPlusSorular.Telefon1, model.Iletisim.Tel1);
                    }
                    else
                    {
                        if (KayitliMusteri != null)
                        {
                            MusteriTelefon telefon = KayitliMusteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                            if (telefon != null)
                            {
                                teklif.AddSoru(FerdiKazaPlusSorular.Telefon1, telefon.Numara);
                            }
                        }
                    }
                    if (!String.IsNullOrEmpty(model.Iletisim.Tel2))
                        teklif.AddSoru(FerdiKazaPlusSorular.Telefon2, model.Iletisim.Tel2);

                    #endregion

                    #region Ferdi Kaza Plus Tmeinatlar

                    if (model.Teminatlar.KazaSonucuVefatBedeli.HasValue)
                        teklif.AddTeminat(FerdiKazaPlusTeminat.KazaSonucuVefatBedeli, model.Teminatlar.KazaSonucuVefatBedeli.Value, 0, 0, 0, 0);

                    if (model.Teminatlar.kazaSonucuMaluliyetBedeli.HasValue)
                        teklif.AddTeminat(FerdiKazaPlusTeminat.KazaSonucuMalliyetBedeli, model.Teminatlar.kazaSonucuMaluliyetBedeli.Value, 0, 0, 0, 0);

                    if (model.Teminatlar.asistansHizmeti)
                        teklif.AddTeminat(FerdiKazaPlusTeminat.AsistansHizmeti, 0, 0, 0, 0, 0);

                    #endregion

                    #endregion

                    #region Teklif return

                    IFerdiKazaPlusTeklif ferdiKazaTeklif = new FerdiKazaPlusTeklif();

                    ferdiKazaTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.METLIFE);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = model.Teminatlar.OdemeSecenegi == 1 ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                    if (model.Teminatlar != null)
                    {
                        if (model.Teminatlar.taksitSayisi != null)
                        {
                            teklif.GenelBilgiler.TaksitSayisi = (byte)model.Teminatlar.taksitSayisi;
                            ferdiKazaTeklif.AddOdemePlani((int)model.Teminatlar.taksitSayisi);
                        }
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        ferdiKazaTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = ferdiKazaTeklif.Hesapla(teklif);
                    #endregion

                    #region İş Takip Oluştur

                    if (teklif.GenelBilgiler.TeklifId > 0)
                    {
                        //İş Takip ve İş Takip Detay Tablosuna İş ekle
                        IsTakip isTakip = new IsTakip();
                        isTakip.TeklifId = teklif.GenelBilgiler.TeklifId; //Teklif Hesaplandıktan sonraki id
                        isTakip.Asama = 1;
                        isTakip.KayitTarihi = System.DateTime.Now;
                        isTakip.TVMKullaniciId = model.Hazirlayan.TVMKullaniciKodu; //Kullanıcı Kodu
                        isTakip.MusteriKodu = musteri.MusteriKodu;

                        IsTakipDetay isTakipDetay = new IsTakipDetay();
                        isTakipDetay.IsTipiDetayId = MetlifeIsTipleri.IsOlusturldu;
                        isTakipDetay.TvmKullaniciId = _AktifKullaniciService.KullaniciKodu;
                        isTakipDetay.HareketTipi = MetlifeHareketTipleri.Baslangic;
                        isTakipDetay.KayitTarihi = System.DateTime.Now;
                        isTakip.IsTakipDetays.Add(isTakipDetay);

                        _TeklifService.CreateIsTakip(isTakip);
                    }
                    #endregion

                    return Json(new { id = isDurum.IsId, g = isDurum.Guid });
                }

                catch (Exception ex)
                {
                    _LogService.Error(ex);
                }
            }
            else
            {
                _LogService.Info("ModelState is not valid");
            }
            #endregion

            #region Hata Log

            foreach (var key in ModelState.Keys)
            {
                ModelState state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    _LogService.Warning("Validation key : {0}, error : {1}", key, error.ErrorMessage);
                }
            }

            ModelState.TraceErros();

            #endregion

            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }

        private TeklifFiyatModel FerdiKazaPlusFiyat(FerdiKazaPlusTeklif ferdiKazaPlusTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = ferdiKazaPlusTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = ferdiKazaPlusTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = ferdiKazaPlusTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = ferdiKazaPlusTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = ferdiKazaPlusTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = ferdiKazaPlusTeklif.GetIsDurum();
            List<IsDurumDetay> detaylar = null;
            if (durum != null)
            {
                detaylar = durum.IsDurumDetays.ToList<IsDurumDetay>();
            }

            foreach (var item in tumListe)
            {
                TeklifFiyatDetayModel fiyatModel = base.TeklifFiyat(item, detaylar);
                model.Fiyatlar.Add(fiyatModel);
            }

            return model;
        }

        //EKLE
        private SigortaliBilgileriModel FerdiKazaPlusSigortaliBilgileri(int? teklifId, ITeklif teklif)
        {
            SigortaliBilgileriModel model = new SigortaliBilgileriModel();
            try
            {
                model.CinsiyetTipler = new SelectList(FerdiKazaPlus.CinsiyetTipleri(), "Value", "Text", "");
                model.CinsiyetTipler.First().Selected = true;
                model.MeslekGruplari = new SelectList(FerdiKazaPlus.MeslekTipleriMetlife(), "Value", "Text", "");
                model.Urunler = new SelectList(FerdiKazaPlus.UrunTipleri(), "Value", "Text", "");
                model.SigortaSureler = new SelectList(FerdiKazaPlus.SigortaSureleri(), "Value", "Text", "1");
                if (teklifId.HasValue && teklif != null)
                {
                    //Meslek Grubu
                    string meslekGrubu = teklif.ReadSoru(FerdiKazaPlusSorular.Meslek_Grubu, "0");
                    if (!String.IsNullOrEmpty(meslekGrubu))
                        model.MeslekGrubu = Convert.ToByte(meslekGrubu);

                    //Ürün Adı
                    string urunAdi = teklif.ReadSoru(FerdiKazaPlusSorular.Urun_Adi, "0");
                    if (!String.IsNullOrEmpty(urunAdi))
                        model.UrunAd = Convert.ToByte(urunAdi);

                    //Sigorta süresi (yıl)
                    string sigortaSuresi = teklif.ReadSoru(FerdiKazaPlusSorular.Sigorta_Suresi, "0");
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.SigortaSuresi = Convert.ToByte(sigortaSuresi);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private FerdiKazaPlusTeminatlarModel FerdiKazaPlusTeminatlar(int? teklifId, ITeklif teklif)
        {
            FerdiKazaPlusTeminatlarModel model = new FerdiKazaPlusTeminatlarModel();

            try
            {
                model.teminatTutarlari = new SelectList(FerdiKazaPlus.FerdiKazaPlusTeminatTutarlari(), "Value", "Text", "");
                model.odemeSecenekleri = new SelectList(FerdiKazaPlus.FerdiKazaPlusOdemeSecenekleri(), "Value", "Text", "");
                if (teklifId.HasValue)
                {

                    //Teminat Tutari
                    string teminatTutari = teklif.ReadSoru(FerdiKazaPlusSorular.Teminat_Tutari, "0");
                    if (!String.IsNullOrEmpty(teminatTutari))
                        model.teminatTutari = Convert.ToInt32(teminatTutari);

                    string odemeSecenegi = teklif.ReadSoru(FerdiKazaPlusSorular.Odeme_Secenegi, "0");
                    if (!String.IsNullOrEmpty(odemeSecenegi))
                        model.OdemeSecenegi = Convert.ToByte(odemeSecenegi);

                    TeklifTeminat KazaSonucuVefat = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == FerdiKazaPlusTeminat.KazaSonucuVefatBedeli);
                    if (KazaSonucuVefat != null)
                    {
                        if (KazaSonucuVefat.TeminatBedeli.HasValue)
                            model.KazaSonucuVefatBedeli = (int)KazaSonucuVefat.TeminatBedeli;
                    }
                    TeklifTeminat KazaSonucuMaluliyet = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == FerdiKazaPlusTeminat.KazaSonucuMalliyetBedeli);
                    if (KazaSonucuMaluliyet != null)
                    {
                        if (KazaSonucuMaluliyet.TeminatBedeli.HasValue)
                            model.kazaSonucuMaluliyetBedeli = (int)KazaSonucuMaluliyet.TeminatBedeli;
                    }
                    model.asistansHizmeti = teklif.ReadSoru(FerdiKazaPlusTeminat.AsistansHizmeti, false);

                    string sigortaPrimT = teklif.ReadSoru(FerdiKazaPlusSorular.Sigorta_Prim_Tutari, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaPrimT))
                        model.SigortaPrimTutari = Convert.ToInt32(sigortaPrimT);

                    string taksitSayisi = teklif.ReadSoru(FerdiKazaPlusSorular.Taksit_Sayisi, String.Empty);
                    if (!String.IsNullOrEmpty(taksitSayisi))
                        model.taksitSayisi = Convert.ToInt32(taksitSayisi);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private FerdiKazaPlusLehtarBilgileriModel LehtarBilgileri(int? teklifId, ITeklif teklif)
        {
            FerdiKazaPlusLehtarBilgileriModel model = new FerdiKazaPlusLehtarBilgileriModel();
            model.LehterList = new List<Models.FerdiKazaPlusLehtarBilgileri>();
            model.kisiSayilari = new List<SelectListItem>();
            model.Lehtar = 1;
            model.LehtarList = new SelectList(FerdiKazaPlus.LehtarTipleri(), "Value", "Text", model.Lehtar);
            for (int i = 1; i < 6; i++)
            {
                //model.kisiSayilari.Add(new SelectListItem() { Text = "Lütfen Seçiniz", Value = "0" });
                model.kisiSayilari.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            for (int i = 0; i < 5; i++)
            {
                FerdiKazaPlusLehtarBilgileri sigortali = new FerdiKazaPlusLehtarBilgileri();
                model.LehterList.Add(sigortali);
            }

            if (teklifId.HasValue)
            {
                model.LehterList[0].AdiSoyadi = "Ahmet Ateş";
                model.LehterList[0].DogumTarihi = Convert.ToDateTime("01.12.1980");
                model.LehterList[0].Oran = 5;
                model.LehterList.Add(model.LehterList[0]);
            }

            return model;
        }

        private PrimOdeyenBilgileriModel PrimOdeyenBilgileri(int? teklifId, ITeklif teklif)
        {
            PrimOdeyenBilgileriModel model = new PrimOdeyenBilgileriModel();
            try
            {
                model.Aylar = TeklifProvider.KrediKartiAylar();
                model.Yillar = TeklifProvider.KrediKartiYillar();

                if (teklifId.HasValue && teklif != null)
                {
                    //Sigorta Başlangıç Tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(FerdiKazaPlusSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    string kartno = teklif.ReadSoru(FerdiKazaPlusSorular.KartNo, String.Empty);
                    if (!String.IsNullOrEmpty(kartno))
                    {
                        model.KartNo.KK1 = kartno.Substring(0, 3);
                        model.KartNo.KK2 = kartno.Substring(4, 7);
                        model.KartNo.KK3 = kartno.Substring(8, 11);
                        model.KartNo.KK4 = kartno.Substring(12, 15);

                    }

                    string sktay = teklif.ReadSoru(FerdiKazaPlusSorular.SKT_ay, String.Empty);
                    if (!String.IsNullOrEmpty(sktay))
                    {
                        model.ay = sktay;

                    }
                    string sktyil = teklif.ReadSoru(FerdiKazaPlusSorular.SKT_yil, String.Empty);
                    if (!String.IsNullOrEmpty(sktyil))
                    {
                        model.yil = sktyil;

                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return model;
        }

        private IletisimBilgieriModel IletisimBilgieri(int? teklifId, ITeklif teklif)
        {
            IletisimBilgieriModel model = new IletisimBilgieriModel();
            try
            {
                model.AdresTipleri = new SelectList(FerdiKazaPlus.AdresTipleri(), "Value", "Text", "1");
                if (teklifId.HasValue && teklif != null)
                {
                    model.AdresTipi = Convert.ToInt32(teklif.ReadSoru(FerdiKazaPlusSorular.AdresTipi, String.Empty));
                    model.Adres = teklif.ReadSoru(FerdiKazaPlusSorular.Adres, String.Empty);
                    model.Email = teklif.ReadSoru(FerdiKazaPlusSorular.Email, String.Empty);
                    model.Tel1 = teklif.ReadSoru(FerdiKazaPlusSorular.Telefon1, String.Empty);
                    model.Tel2 = teklif.ReadSoru(FerdiKazaPlusSorular.Telefon2, String.Empty);

                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return model;

        }

        //DETAY
        private SigortaliBilgileriModel FerdiKazaPlusSigortaliBilgileriDetay(ITeklif teklif, string kimlikNo)
        {
            SigortaliBilgileriModel model = new SigortaliBilgileriModel();
            try
            {

                if (teklif != null)
                {
                    string dogumTarihi = "";
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, _AktifKullaniciService.TVMKodu);
                    if (musteri != null)
                    {
                        model.TCKimlikNo = musteri.KimlikNo;
                        model.Ad = musteri.AdiUnvan;
                        dogumTarihi = musteri.DogumTarihi.ToString();
                        model.Cinsiyet = musteri.Cinsiyet;
                        model.MusteriNo = musteri.MusteriKodu;
                        model.SoyAd = musteri.SoyadiUnvan;
                    }
                    model.DogumYeri = "İstanbul";
                    model.BabaAdi = "Ali";
                    model.DogumTarihiText = dogumTarihi.Substring(0, 10);
                    //Meslek Grubu
                    switch (teklif.ReadSoru(FerdiKazaPlusSorular.Meslek_Grubu, "0"))
                    {
                        case "1": model.MeslekGrubuText = "Fikren ya da elle çalışanlar hiç bir mesleki faliyette bulunmayanlar"; break;
                        case "2": model.MeslekGrubuText = "Elle ve aynı zamanda bedenen çalışanlar"; break;
                        case "3": model.MeslekGrubuText = "150,000"; break;
                        default: model.MeslekGrubuText = ""; break;

                    }

                    //Ürün Adı
                    switch (teklif.ReadSoru(FerdiKazaPlusSorular.Urun_Adi, "0"))
                    {
                        case "1": model.UrunAdi = "Ferdi Kaza Plus"; break;
                        case "2": model.UrunAdi = "Yıllık Hayat Sigortası"; break;
                        default: model.UrunAdi = ""; break;
                    }

                    //Sigorta süresi (yıl)
                    model.SigortaSure = "1";

                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private FerdiKazaPlusTeminatlarModel FerdiKazaPlusTeminatlarDetay(ITeklif teklif)
        {
            FerdiKazaPlusTeminatlarModel model = new FerdiKazaPlusTeminatlarModel();

            try
            {
                //Teminat Tutari

                switch (teklif.ReadSoru(FerdiKazaPlusSorular.Teminat_Tutari, "0"))
                {
                    case "1": model.teminatTutariText = "75,000"; break;
                    case "2": model.teminatTutariText = "100,000"; break;
                    case "3": model.teminatTutariText = "150,000"; break;

                }
                switch (teklif.ReadSoru(FerdiKazaPlusSorular.Odeme_Secenegi, "0"))
                {
                    case "2": model.odemeSecenegiText = babonline.Installment; break;
                    case "1": model.odemeSecenegiText = babonline.Cash; break;
                }

                TeklifTeminat KazaSonucuVefat = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == FerdiKazaPlusTeminat.KazaSonucuVefatBedeli);
                if (KazaSonucuVefat != null)
                {
                    if (KazaSonucuVefat.TeminatBedeli.HasValue)
                        model.KazaSonucuVefatBedeli = (int)KazaSonucuVefat.TeminatBedeli;
                }
                TeklifTeminat KazaSonucuMaluliyet = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == FerdiKazaPlusTeminat.KazaSonucuMalliyetBedeli);
                if (KazaSonucuMaluliyet != null)
                {
                    if (KazaSonucuMaluliyet.TeminatBedeli.HasValue)
                        model.kazaSonucuMaluliyetBedeli = (int)KazaSonucuMaluliyet.TeminatBedeli;
                }
                model.asistansHizmeti = teklif.ReadSoru(FerdiKazaPlusSorular.Asistans_Hizmeti, false);

                string sigortaPrimT = teklif.ReadSoru(FerdiKazaPlusSorular.Sigorta_Prim_Tutari, String.Empty);
                if (!String.IsNullOrEmpty(sigortaPrimT))
                    model.SigortaPrimTutari = Convert.ToInt32(sigortaPrimT);

                string taksitSayisi = teklif.ReadSoru(FerdiKazaPlusSorular.Taksit_Sayisi, String.Empty);
                if (!String.IsNullOrEmpty(taksitSayisi))
                    model.taksitSayisi = Convert.ToInt32(taksitSayisi);

            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private PrimOdeyenBilgileriModel PrimOdeyenBilgileriDetay(ITeklif teklif)
        {
            PrimOdeyenBilgileriModel model = new PrimOdeyenBilgileriModel();
            try
            {
                if (teklif != null)
                {
                    //Sigorta Başlangıç Tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(FerdiKazaPlusSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    string kartno = teklif.ReadSoru(FerdiKazaPlusSorular.KartNo, String.Empty);
                    if (!String.IsNullOrEmpty(kartno))
                    {
                        model.KartNumarasi = kartno.Substring(0, 4);
                        model.KartNumarasi += "********";
                        model.KartNumarasi += kartno.Substring(12, 4);


                    }

                    string sktay = teklif.ReadSoru(FerdiKazaPlusSorular.SKT_ay, String.Empty);
                    if (!String.IsNullOrEmpty(sktay))
                    {
                        model.SonKullanmaTarihi = sktay;

                    }
                    string sktyil = teklif.ReadSoru(FerdiKazaPlusSorular.SKT_yil, String.Empty);
                    if (!String.IsNullOrEmpty(sktyil))
                    {
                        model.SonKullanmaTarihi += " / " + sktyil;

                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return model;
        }

        private IletisimBilgieriModel IletisimBilgieriDetay(ITeklif teklif)
        {
            IletisimBilgieriModel model = new IletisimBilgieriModel();
            try
            {

                if (teklif != null)
                {
                    string adresTipi = teklif.ReadSoru(FerdiKazaPlusSorular.AdresTipi, String.Empty);
                    if (adresTipi != null)
                    {
                        switch (adresTipi)
                        {
                            case "8": model.AdresTipiText = "Ev"; break;
                            case "9": model.AdresTipiText = "İş"; break;
                            default: model.AdresTipiText = "Ev"; break;
                        }
                    }

                    model.Adres = teklif.ReadSoru(FerdiKazaPlusSorular.Adres, String.Empty);
                    model.Email = teklif.ReadSoru(FerdiKazaPlusSorular.Email, String.Empty);
                    model.Tel1 = teklif.ReadSoru(FerdiKazaPlusSorular.Telefon1, String.Empty);
                    model.Tel2 = teklif.ReadSoru(FerdiKazaPlusSorular.Telefon2, String.Empty);

                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return model;

        }

        #region Sigortalı Sorgula

        [HttpPost]
        [AjaxException]
        public ActionResult SigortaliKimlikSorgula(string kimlikNo)
        {
            FerdiKazaPlusModel model = new FerdiKazaPlusModel();

            if (String.IsNullOrEmpty(kimlikNo))
            {
                model.SorgulamaHata("Kimlik numarası boş bırakılamaz");
            }
            else if (kimlikNo.Length == 11 || kimlikNo.Length == 10)
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, _AktifKullaniciService.TVMKodu);

                if (musteri != null)
                {
                    model.Sigortali = SigortaliBilgileriDoldur(kimlikNo);
                    model.Iletisim = AdresBilgileriDoldur(kimlikNo);
                    model.PrimOdeme = PrimOdeyenBilgileriDoldur();
                    return Json(model);
                }

                model.SorgulamaHata("Girilen kimlik numarasına ait bilgi bulunamadı.");
                return Json(model);
            }

            model.SorgulamaHata("Kimlik numarası tüzel müşteriler için 10, şahıslar için 11 rakamdan oluşmalıdır");

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private SigortaliBilgileriModel SigortaliBilgileriDoldur(string kimlikNu)
        {
            SigortaliBilgileriModel model = new SigortaliBilgileriModel();
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNu, _AktifKullaniciService.TVMKodu);

            if (musteri != null)
            {
                model.Ad = musteri.AdiUnvan;
                model.MeslekGrubu = 1;
                model.UrunAd = 1;
                model.SigortaSuresi = 1;
                model.MusteriNo = musteri.MusteriKodu;
                model.SoyAd = musteri.SoyadiUnvan;
                model.DogumYeri = "İstanbul";
                model.BabaAdi = "Ali";
                model.MeslekGruplari = new SelectList(FerdiKazaPlus.MeslekTipleriMetlife(), "Value", "Text", model.MeslekGrubu);
                model.Urunler = new SelectList(FerdiKazaPlus.UrunTipleri(), "Value", "Text", model.UrunAd);
                model.SigortaSureler = new SelectList(FerdiKazaPlus.SigortaSureleri(), "Value", "Text", model.MeslekGrubu);
                model.Cinsiyet = musteri.Cinsiyet;

                if (musteri.DogumTarihi.HasValue)
                {
                    model.DogumTarihi = musteri.DogumTarihi.Value;
                    model.DogumTarihiText = musteri.DogumTarihi.Value.ToString("dd.MM.yyyy");
                }
            }

            return model;
        }

        private IletisimBilgieriModel AdresBilgileriDoldur(string kimlikNu)
        {
            IletisimBilgieriModel model = new IletisimBilgieriModel();
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNu, _AktifKullaniciService.TVMKodu);

            if (musteri != null)
            {

                MusteriAdre adres = musteri.MusteriAdres.FirstOrDefault(m => m.Varsayilan == true);

                if (adres != null)
                {
                    if (!String.IsNullOrEmpty(adres.Cadde))
                        model.Adres = adres.Cadde + " Cad.";
                    if (!String.IsNullOrEmpty(adres.Mahalle))
                        model.Adres += adres.Mahalle + " Mah.";
                    if (!String.IsNullOrEmpty(adres.Sokak))
                        model.Adres += adres.Sokak + " Sk.";
                    if (!String.IsNullOrEmpty(adres.HanAptFab))
                        model.Adres += adres.HanAptFab + " Apt.";
                    if (!String.IsNullOrEmpty(adres.BinaNo))
                        model.Adres += " No." + adres.BinaNo;
                    if (!String.IsNullOrEmpty(adres.DaireNo))
                        model.Adres += " D." + adres.DaireNo;

                    if (adres.PostaKodu > 0)
                        model.Adres += adres.PostaKodu;

                    if (!String.IsNullOrEmpty(adres.Semt))
                        model.Adres += adres.Semt + " /";

                    model.Adres += adres.Adres;
                    model.AdresTipi = adres.AdresTipi.HasValue ? adres.AdresTipi.Value : 0;
                    model.AdresMevcut = true;
                    model.EmailMevcut = true;
                    model.Tel1Mevcut = true;
                    model.AdresTipleri = new SelectList(FerdiKazaPlus.AdresTipleri(), "Value", "Text", model.AdresTipi);
                    model.Email = musteri.EMail;
                }

                MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                if (telefon != null)
                {
                    model.Tel1 = telefon.Numara;
                }


            }

            return model;
        }

        private PrimOdeyenBilgileriModel PrimOdeyenBilgileriDoldur()
        {
            PrimOdeyenBilgileriModel model = new PrimOdeyenBilgileriModel();
            model.KartNo = KrediKartiDoldur();
            model.KartNoMevcut = true;
            model.ay = "12";
            model.yil = "2017";
            return model;
        }

        private KrediKartiModel KrediKartiDoldur()
        {
            KrediKartiModel model = new KrediKartiModel();

            model.KK1 = "1234";
            model.KK2 = "****";
            model.KK3 = "****";
            model.KK4 = "1234";
            return model;
        }
        #endregion

        #region Dokuman Ekle/Sil
        public ActionResult Dokuman(int teklifId)
        {
            MetlifeDokumanModel model = new MetlifeDokumanModel();
            model.TeklifId = teklifId;

            return PartialView("_DokumanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult Dokuman(MetlifeDokumanModel model, HttpPostedFileBase file)
        {
            try
            {
                IsTakip isTakip = _TeklifService.GetIsTakip(model.TeklifId);
                if (ModelState.IsValid && file != null && file.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);

                    if (_TVMService.CheckedFileName(fileName))
                    {
                        string url = _Storage.UploadFile(model.TeklifId.ToString(), fileName, file.InputStream);
                        IsTakipDokuman dokuman = new IsTakipDokuman();
                        dokuman.IsTakipId = isTakip.IsTakipId;
                        dokuman.DokumanTipi = 0;
                        dokuman.DokumanURL = url;
                        dokuman.KaydedenKullanici = _AktifKullaniciService.KullaniciKodu;
                        dokuman.KayitTarihi = System.DateTime.Now;

                        _TeklifService.CreateIsTakipDokuman(dokuman);
                        return null;
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
                        return PartialView("_DokumanEkle", model);
                    }
                }
                //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
                return PartialView("_DokumanEkle", model);
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                ModelState.AddModelError("", babonline.Message_DocumentSaveError);
                return PartialView("_DokumanEkle", model);
            }

        }

        [HttpPost]
        [AjaxException]
        public ActionResult DokumanSil(int IsTakipId, int IsTakipDokumanId)
        {
            IsTakipDokumanlarListModel model = new IsTakipDokumanlarListModel();
            if (IsTakipDokumanId > 0)
            {
                _TeklifService.DeleteIsTakipDokuman(IsTakipDokumanId);
            }
            if (IsTakipId > 0)
            {
                model = DokumanList(IsTakipId);
            }

            return PartialView("_Dokumanlar", model);


        }

        [AjaxException]
        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult DokumanView(int IsTakipId)
        {
            return PartialView("_Dokumanlar", DokumanList(IsTakipId));
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public IsTakipDokumanlarListModel DokumanList(int IsTakipId)
        {
            IsTakipDokumanlarListModel model = new IsTakipDokumanlarListModel();
            model.Items = new List<IsTakipDokumanlar>();
            var list = _TeklifService.GetListDokumanlar(IsTakipId);
            foreach (var item in list)
            {
                TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.Filter(s => s.KullaniciKodu == item.KaydedenKullanici).FirstOrDefault();
                IsTakipDokumanlar dokumanlar = new IsTakipDokumanlar();
                dokumanlar.DokumanTuru = "Başvuru Formu";
                dokumanlar.Tarihi = item.KayitTarihi;
                dokumanlar.KaydiEkleyen = kullanici.Adi + " " + kullanici.Soyadi;
                dokumanlar.DosyaAdi = "Ferdi Kaza Plus Başvuru Sureci PDF";
                dokumanlar.DokumanURL = item.DokumanURL;
                dokumanlar.IsTakipId = IsTakipId;
                dokumanlar.IsTakipDokumanId = item.IsTakipDokumanId;
                model.Items.Add(dokumanlar);

            }

            return model;

        }
        #endregion

        #region İş Takip İlerlet
        [HttpPost]
        [AjaxException]
        public JsonResult Ilerlet(int id, string aciklama)
        {

            IsTakip isTakip = _TeklifService.GetIsTakip(id);
            IsTakipDokuman istakipDokuman = _TeklifService.GetIsTakipDokuman(isTakip.IsTakipId);
            if (isTakip != null && istakipDokuman != null)
            {
                switch (isTakip.Asama)
                {
                    case 1: IsTakipAsama2(isTakip); break;
                    case 2: IsTakipAsama3(isTakip); break;
                    case 3: IsTakipAsama4(isTakip); break;
                    case 4: IsTakipAsama5(isTakip, aciklama); break;
                }

                return Json(new { Success = "true" });
            }
            else
                return Json(new { Success = "false" });



        }

        private void IsTakipAsama2(IsTakip isTakip)
        {
            isTakip.Asama = 2;
            isTakip.TVMKullaniciId = _AktifKullaniciService.KullaniciKodu;
            IsTakipDetay detay = new IsTakipDetay();

            detay.HareketTipi = MetlifeHareketTipleri.Ileri;
            detay.KayitTarihi = DateTime.Now;
            detay.IsTipiDetayId = MetlifeIsTipleri.BasvuruSureci2Adim;
            detay.TvmKullaniciId = _AktifKullaniciService.KullaniciKodu;

            isTakip.IsTakipDetays.Add(detay);



            _TeklifService.UpdateIsTakip(isTakip);

            IEMailService _email = DependencyResolver.Current.GetService<IEMailService>();
            _email.SendMetlifeEmail(MetlifeKullaniciGruplari.MetlifeOperasyon, isTakip.TeklifId, isTakip.Asama);


        }
        private void IsTakipAsama3(IsTakip isTakip)
        {
            isTakip.Asama = 3;
            isTakip.TVMKullaniciId = _AktifKullaniciService.KullaniciKodu;
            IsTakipDetay detay = new IsTakipDetay();

            detay.HareketTipi = MetlifeHareketTipleri.Ileri;
            detay.KayitTarihi = DateTime.Now;
            detay.IsTipiDetayId = MetlifeIsTipleri.BasvuruSureci3Adim;
            detay.TvmKullaniciId = _AktifKullaniciService.KullaniciKodu;

            isTakip.IsTakipDetays.Add(detay);

            IsTakipSoru soru = new IsTakipSoru();
            soru.TeklifId = isTakip.TeklifId;
            soru.SoruKodu = MetlifeSoruTipleri.EksikBelge;
            soru.CevapTipi = MetlifeCevapTipleri.Metin;
            soru.Cevap = "0";
            soru.KayitTarihi = DateTime.Now;

            detay.IsTakipSorus.Add(soru);

            soru = new IsTakipSoru();
            soru.TeklifId = isTakip.TeklifId;
            soru.SoruKodu = MetlifeSoruTipleri.EksikImza;
            soru.CevapTipi = MetlifeCevapTipleri.Metin;
            soru.Cevap = "0";
            soru.KayitTarihi = DateTime.Now;
            detay.IsTakipSorus.Add(soru);

            soru = new IsTakipSoru();
            soru.TeklifId = isTakip.TeklifId;
            soru.SoruKodu = MetlifeSoruTipleri.EksikBelgeAciklama;
            soru.CevapTipi = MetlifeCevapTipleri.Metin;
            soru.Cevap = "0";
            soru.KayitTarihi = DateTime.Now;
            detay.IsTakipSorus.Add(soru);

            soru = new IsTakipSoru();
            soru.TeklifId = isTakip.TeklifId;
            soru.SoruKodu = MetlifeSoruTipleri.EksikImzaAciklama;
            soru.CevapTipi = MetlifeCevapTipleri.Metin;
            soru.Cevap = "0";
            soru.KayitTarihi = DateTime.Now;

            detay.IsTakipSorus.Add(soru);

            _TeklifService.UpdateIsTakip(isTakip);

            IEMailService _email = DependencyResolver.Current.GetService<IEMailService>();
            _email.SendMetlifeEmail(MetlifeKullaniciGruplari.SatisEkibi, isTakip.TeklifId, isTakip.Asama);

        }
        private void IsTakipAsama4(IsTakip isTakip)
        {
            isTakip.Asama = 4;
            isTakip.TVMKullaniciId = _AktifKullaniciService.KullaniciKodu;
            IsTakipDetay detay = new IsTakipDetay();

            detay.HareketTipi = MetlifeHareketTipleri.Ileri;
            detay.KayitTarihi = DateTime.Now;
            detay.IsTipiDetayId = MetlifeIsTipleri.BasvuruSureci4Adim;
            detay.TvmKullaniciId = _AktifKullaniciService.KullaniciKodu;

            isTakip.IsTakipDetays.Add(detay);

            IsTakipSoru soru = new IsTakipSoru();
            soru.TeklifId = isTakip.TeklifId;
            soru.SoruKodu = MetlifeSoruTipleri.KuryeyeTeslimEdildi;
            soru.CevapTipi = MetlifeCevapTipleri.Metin;
            soru.Cevap = "1";
            soru.KayitTarihi = DateTime.Now;

            detay.IsTakipSorus.Add(soru);

            _TeklifService.UpdateIsTakip(isTakip);

            IEMailService _email = DependencyResolver.Current.GetService<IEMailService>();
            _email.SendMetlifeEmail(MetlifeKullaniciGruplari.MetlifeOperasyon, isTakip.TeklifId, isTakip.Asama);

        }
        private void IsTakipAsama5(IsTakip isTakip, string aciklama)
        {
            isTakip.Asama = 5;
            isTakip.TVMKullaniciId = _AktifKullaniciService.KullaniciKodu;

            IsTakipDetay detay = new IsTakipDetay();

            detay.HareketTipi = MetlifeHareketTipleri.Sonlandir;
            detay.KayitTarihi = DateTime.Now;
            detay.IsTipiDetayId = MetlifeIsTipleri.SurecSonu;
            detay.TvmKullaniciId = _AktifKullaniciService.KullaniciKodu;

            isTakip.IsTakipDetays.Add(detay);

            IsTakipSoru soru = new IsTakipSoru();
            soru.TeklifId = isTakip.TeklifId;
            soru.SoruKodu = MetlifeSoruTipleri.FormAsliGeldi;
            soru.CevapTipi = MetlifeCevapTipleri.Metin;
            soru.Cevap = "1";
            soru.KayitTarihi = DateTime.Now;

            detay.IsTakipSorus.Add(soru);

            soru = new IsTakipSoru();
            soru.TeklifId = isTakip.TeklifId;
            soru.SoruKodu = MetlifeSoruTipleri.FormBelgeNumarasi;
            soru.CevapTipi = MetlifeCevapTipleri.Metin;
            soru.Cevap = aciklama;
            soru.KayitTarihi = DateTime.Now;

            detay.IsTakipSorus.Add(soru);

            _TeklifService.UpdateIsTakip(isTakip);

        }

        #endregion

        #region Beklet
        [HttpPost]
        [AjaxException]
        public JsonResult Beklet(int id)
        {
            IsTakip isTakip = _TeklifService.GetIsTakip(id);
            if (isTakip != null)
            {
                IsTakipDetay detay = new IsTakipDetay();

                detay.HareketTipi = MetlifeHareketTipleri.Beklet;
                detay.KayitTarihi = DateTime.Now;
                if (isTakip.Asama == 2) detay.IsTipiDetayId = MetlifeIsTipleri.BasvuruSureci2Adim;
                else if (isTakip.Asama == 3) detay.IsTipiDetayId = MetlifeIsTipleri.BasvuruSureci3Adim;

                detay.TvmKullaniciId = _AktifKullaniciService.KullaniciKodu;

                isTakip.IsTakipDetays.Add(detay);

                _TeklifService.UpdateIsTakip(isTakip);

                return Json(new { Success = "true", Message = "" });
            }
            else
                return Json(new { Success = "false", Message = "" });

        }
        #endregion

        #region İade
        [HttpPost]
        [AjaxException]
        public JsonResult Iade(int isTakipId, string eksikBelge, string eksikImza, string eksikBelgeAciklama, string eksikImzaAciklama)
        {
            IsTakip isTakip = _TeklifService.IsTakipGet(isTakipId);
            if (isTakip != null)
            {
                isTakip.Asama = 1;
                isTakip.KayitTarihi = DateTime.Now;
                isTakip.TVMKullaniciId = _AktifKullaniciService.KullaniciKodu;

                IsTakipDetay detay = new IsTakipDetay();

                detay.IsTipiDetayId = MetlifeIsTipleri.BasvuruSureci2Adim;
                detay.TvmKullaniciId = _AktifKullaniciService.KullaniciKodu;
                detay.HareketTipi = MetlifeHareketTipleri.Iade;
                detay.KayitTarihi = DateTime.Now;

                isTakip.IsTakipDetays.Add(detay);

                IsTakipSoru soru = new IsTakipSoru();
                soru.IsTakipDetayId = detay.IsTakipDetayId;
                soru.TeklifId = isTakipId;
                soru.SoruKodu = MetlifeSoruTipleri.EksikBelge;
                soru.CevapTipi = MetlifeCevapTipleri.Metin;
                soru.Cevap = eksikBelge;
                soru.KayitTarihi = DateTime.Now;
                detay.IsTakipSorus.Add(soru);

                soru = new IsTakipSoru();
                soru.IsTakipDetayId = detay.IsTakipDetayId;
                soru.TeklifId = isTakipId;
                soru.SoruKodu = MetlifeSoruTipleri.EksikImza;
                soru.CevapTipi = MetlifeCevapTipleri.Metin;
                soru.Cevap = eksikImza;
                soru.KayitTarihi = DateTime.Now;
                detay.IsTakipSorus.Add(soru);

                soru = new IsTakipSoru();
                soru.IsTakipDetayId = detay.IsTakipDetayId;
                soru.TeklifId = isTakipId;
                soru.SoruKodu = MetlifeSoruTipleri.EksikBelgeAciklama;
                soru.CevapTipi = MetlifeCevapTipleri.Metin;
                soru.Cevap = eksikBelgeAciklama;
                soru.KayitTarihi = DateTime.Now;
                detay.IsTakipSorus.Add(soru);

                soru = new IsTakipSoru();
                soru.IsTakipDetayId = detay.IsTakipDetayId;
                soru.TeklifId = isTakipId;
                soru.SoruKodu = MetlifeSoruTipleri.EksikImzaAciklama;
                soru.CevapTipi = MetlifeCevapTipleri.Metin;
                soru.Cevap = eksikImzaAciklama;
                soru.KayitTarihi = DateTime.Now;
                detay.IsTakipSorus.Add(soru);


                _TeklifService.UpdateIsTakip(isTakip);

                //IEMailService _email = DependencyResolver.Current.GetService<IEMailService>();
                //_email.SendMetlifeIadeEmail(isTakipId, isTakip.TeklifId, isTakip.Asama);

                return Json(new { Success = "true" });
            }
            else
                return Json(new { Success = "false" });
        }
        #endregion

    }
}
