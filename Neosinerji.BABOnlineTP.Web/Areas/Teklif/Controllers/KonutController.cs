using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.KonutSigortasi_Paket)]
    public class KonutController : TeklifController
    {
        public KonutController(ITVMService tvmService,
                                ITeklifService teklifService,
                                IMusteriService musteriService,
                                IKullaniciService kullaniciService,
                                IAktifKullaniciService aktifKullaniciService,
                                ITanimService tanimService,
                                IUlkeService ulkeService,
                                ICRService crService,
                                IAracService aracService,
                                IUrunService urunService,
                                ITUMService tumService)
            : base(tvmService, teklifService, musteriService, kullaniciService, aktifKullaniciService, tanimService, ulkeService, crService, aracService, urunService, tumService)
        {

        }


        public ActionResult Ekle(int? id)
        {
            KonutModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            KonutModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public KonutModel EkleModel(int? id, int? teklifId)
        {
            KonutModel model = new KonutModel();
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
                List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                model.Musteri.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text").ListWithOptionLabel();


                model.Musteri.MusteriTipleri = nsmusteri.MusteriListProvider.MusteriTipleri();
                model.Musteri.UyrukTipleri = new SelectList(nsmusteri.MusteriListProvider.UyrukTipleri(), "Value", "Text", "0");
                model.Musteri.CinsiyetTipleri = new SelectList(nsmusteri.MusteriListProvider.CinsiyetTipleri(), "Value", "Text");
                model.Musteri.CinsiyetTipleri.First().Selected = true;
                #endregion

                #region Konut

                model.RizikoGenelBilgiler = KonutRizikoGenelBilgiler(teklifId, teklif);
                model.RizikoAdresBilgiler = KonutRizikoAdresModel(teklifId, teklif);
                model.RizikoDigerBilgiler = KonutRizikoDigerBilgiler(teklifId, teklif);
                model.KonutTeminatBedelBilgileri = KonutTeminatBedelBilgileri(teklifId, teklif);
                model.KonutTeminatBilgileri = KonutTeminatBilgileri(teklifId, teklif);

                model.KonutNotModel = new KonutNotModel();
                if (teklif != null & teklifId.HasValue)
                    if (teklif.GenelBilgiler.TeklifNot != null)
                        model.KonutNotModel.Not = teklif.GenelBilgiler.TeklifNot.Aciklama;

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.KonutSigortasi_Paket);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme
                model.Odeme = new KonutTeklifOdemeModel();
                model.Odeme.OdemeSekli = true;
                model.Odeme.OdemeTipi = 1;
                model.Odeme.TaksitSayilari = new List<SelectListItem>();
                model.Odeme.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", "2").ToList();

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
            DetayKonutModel model = new DetayKonutModel();

            #region Teklif Genel


            KonutTeklif konutTeklif = new KonutTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = konutTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(konutTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(konutTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (konutTeklif.Teklif.Sigortalilar.Count > 0 &&
               (konutTeklif.Teklif.SigortaEttiren.MusteriKodu != konutTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(konutTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Riziko

            model.RizikoGenelBilgiler = KonutRizikoGenelBilgilerDetay(konutTeklif.Teklif);
            model.RizikoAdresBilgiler = KonutRizikoAdresModelDetay(konutTeklif.Teklif);
            model.RizikoDigerBilgiler = KonutRizikoDigerBilgilerDetay(konutTeklif.Teklif);
            model.KonutTeminatBilgileri = KonutTeminatBilgileriDetay(konutTeklif.Teklif);
            model.KonutTeminatBedelBilgileri = KonutTeminatBedelBilgileriDetay(konutTeklif.Teklif);

            model.KonutNotModel = new KonutNotModel();
            if (konutTeklif.Teklif.GenelBilgiler.TeklifNot != null)
                model.KonutNotModel.Not = konutTeklif.Teklif.GenelBilgiler.TeklifNot.Aciklama;

            #endregion

            #region Teklif Fiyat

            model.Fiyat = KonutFiyat(konutTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            model.KrediKarti.KK_OdemeSekli = konutTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            model.KrediKarti.KK_OdemeTipi = konutTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = konutTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
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

            return View(model);
        }

        public ActionResult Police(int id)
        {
            DetayKonutModel model = new DetayKonutModel();

            #region Teklif Genel


            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif konutTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = konutTeklif.GenelBilgiler.TeklifId;

            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(konutTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(konutTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Riziko

            model.RizikoGenelBilgiler = KonutRizikoGenelBilgilerDetay(konutTeklif);
            model.RizikoAdresBilgiler = KonutRizikoAdresModelDetay(konutTeklif);
            model.RizikoDigerBilgiler = KonutRizikoDigerBilgilerDetay(konutTeklif);
            model.KonutTeminatBilgileri = KonutTeminatBilgileriDetay(konutTeklif);
            model.KonutTeminatBedelBilgileri = KonutTeminatBedelBilgileriDetay(konutTeklif);

            model.KonutNotModel = new KonutNotModel();
            if (konutTeklif.GenelBilgiler.TeklifNot != null)
                model.KonutNotModel.Not = konutTeklif.GenelBilgiler.TeklifNot.Aciklama;

            #endregion

            #region Teklif Odeme

            model.OdemeBilgileri = KonutPoliceOdemeModel(teklif);

            #endregion

            return View(model);
        }

        private TeklifFiyatModel KonutFiyat(KonutTeklif konutTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = konutTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = konutTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = konutTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = konutTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = konutTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = konutTeklif.GetIsDurum();
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

        [HttpPost]
        public ActionResult OdemeAl(OdemeKonutModel model)
        {
            TeklifOdemeCevapModel cevap = new TeklifOdemeCevapModel();

            if (ModelState.IsValid)
            {
                nsbusiness.ITeklif teklif = _TeklifService.GetTeklif(model.KrediKarti.KK_TeklifId);

                nsbusiness.Odeme odeme = new nsbusiness.Odeme(model.KrediKarti.KartSahibi, model.KrediKarti.KartNumarasi.ToString(), model.KrediKarti.GuvenlikNumarasi, model.KrediKarti.SonKullanmaAy, model.KrediKarti.SonKullanmaYil);

                ITeklif urun = TeklifUrunFactory.AsUrunClass(teklif);
                urun.Policelestir(odeme);

                if (urun.Hatalar.Count == 0)
                {
                    try
                    {
                        urun.PolicePDF();
                    }
                    catch (PolicePDFException ex)
                    {
                        _LogService.Error(ex);
                    }

                    cevap.Success = true;
                    cevap.RedirectUrl = TeklifSayfaAdresleri.PoliceAdres(urun.GenelBilgiler.UrunKodu) + urun.GenelBilgiler.TeklifId;
                    return Json(cevap);
                }

                cevap.Hatalar = urun.Hatalar.ToArray();
                cevap.RedirectUrl = String.Empty;
                return Json(cevap);
            }

            cevap.Hatalar = new string[] { babonline.Message_RequiredValues };

            return Json(cevap);
        }

        [HttpPost]
        public ActionResult Hesapla(KonutModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (model.RizikoGenelBilgiler != null)
                {
                    if (!String.IsNullOrEmpty(model.RizikoAdresBilgiler.Cadde) && String.IsNullOrEmpty(model.RizikoAdresBilgiler.Sokak))
                    {
                        if (ModelState["RizikoAdresBilgiler.Sokak"] != null)
                            ModelState["RizikoAdresBilgiler.Sokak"].Errors.Clear();
                    }

                    if (String.IsNullOrEmpty(model.RizikoAdresBilgiler.Cadde) && !String.IsNullOrEmpty(model.RizikoAdresBilgiler.Sokak))
                    {
                        if (ModelState["RizikoAdresBilgiler.Cadde"] != null)
                            ModelState["RizikoAdresBilgiler.Cadde"].Errors.Clear();
                    }


                    if (!model.RizikoGenelBilgiler.RehinliAlacakliDainMurtehinVarmi)
                    {
                        if (ModelState["RizikoGenelBilgiler.Tipi"] != null)
                            ModelState["RizikoGenelBilgiler.Tipi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.KurumBanka"] != null)
                            ModelState["RizikoGenelBilgiler.KurumBanka"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.Sube"] != null)
                            ModelState["RizikoGenelBilgiler.Sube"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.KrediReferansNo_HesapSozlesmeNo"] != null)
                            ModelState["RizikoGenelBilgiler.KrediReferansNo_HesapSozlesmeNo"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.KrediBitisTarihi"] != null)
                            ModelState["RizikoGenelBilgiler.KrediBitisTarihi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.KrediTutari"] != null)
                            ModelState["RizikoGenelBilgiler.KrediTutari"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.DovizKodu"] != null)
                            ModelState["RizikoGenelBilgiler.DovizKodu"].Errors.Clear();
                    }

                    if (!model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi)
                    {
                        if (ModelState["RizikoGenelBilgiler.SigortaSirketi"] != null)
                            ModelState["RizikoGenelBilgiler.SigortaSirketi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.PoliceninVadeTarihi"] != null)
                            ModelState["RizikoGenelBilgiler.PoliceninVadeTarihi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.PoliceNumarasi"] != null)
                            ModelState["RizikoGenelBilgiler.PoliceNumarasi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.PoliceNumarasi"] != null)
                            ModelState["RizikoGenelBilgiler.DaskSigortaBedeli"].Errors.Clear();
                    }

                    ModelStateMusteriClear(ModelState, model.Musteri);
                }

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            #endregion

            #region Teklif kaydı ve hesaplamanın başlatılması

            if (ModelState.IsValid)
            {
                try
                {
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.KonutSigortasi_Paket, model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);


                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Konuta İlişkin Bilgiler

                    #region Riziko Genel Bilgiler

                    teklif.AddSoru(KonutSorular.Yururlukte_dask_policesi_VarYok, model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi);
                    if (model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi == true)
                    {
                        teklif.AddSoru(KonutSorular.Dask_Police_Sigorta_Sirketi, model.RizikoGenelBilgiler.SigortaSirketi);

                        if (model.RizikoGenelBilgiler.PoliceninVadeTarihi.HasValue)
                            teklif.AddSoru(KonutSorular.Dask_Police_Vade_Tarihi, model.RizikoGenelBilgiler.PoliceninVadeTarihi.Value);

                        if (!String.IsNullOrEmpty(model.RizikoGenelBilgiler.PoliceNumarasi))
                            teklif.AddSoru(KonutSorular.Dask_Police_Numarasi, model.RizikoGenelBilgiler.PoliceNumarasi);

                        teklif.AddSoru(KonutSorular.Dask_Sigorta_Bedeli, model.RizikoGenelBilgiler.DaskSigortaBedeli.ToString());
                    }


                    teklif.AddSoru(KonutSorular.RA_Dain_i_Muhtehin_VarYok, model.RizikoGenelBilgiler.RehinliAlacakliDainMurtehinVarmi);
                    if (model.RizikoGenelBilgiler.RehinliAlacakliDainMurtehinVarmi == true)
                    {
                        teklif.AddSoru(KonutSorular.RA_Tipi_Banka_Finansal_Kurum, model.RizikoGenelBilgiler.Tipi);
                        teklif.AddSoru(KonutSorular.RA_Kurum_Banka, model.RizikoGenelBilgiler.KurumBanka.ToString());
                        teklif.AddSoru(KonutSorular.RA_Sube, model.RizikoGenelBilgiler.Sube.ToString());
                        teklif.AddSoru(KonutSorular.RA_Doviz_Kodu, model.RizikoGenelBilgiler.DovizKodu.ToString());

                        if (model.RizikoGenelBilgiler.KrediTutari.HasValue)
                            teklif.AddSoru(KonutSorular.RA_Kredi_Tutari, model.RizikoGenelBilgiler.KrediTutari.Value.ToString());

                        teklif.AddSoru(KonutSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, model.RizikoGenelBilgiler.KrediReferansNo_HesapSozlesmeNo);
                        if (model.RizikoGenelBilgiler.KrediBitisTarihi.HasValue)
                            teklif.AddSoru(KonutSorular.RA_Kredi_Bitis_Tarihi, model.RizikoGenelBilgiler.KrediBitisTarihi.Value);
                    }

                    #endregion

                    #region Riziko Adres Bilgileri

                    teklif.RizikoAdresi.IlKodu = model.RizikoAdresBilgiler.Il;
                    teklif.RizikoAdresi.IlceKodu = model.RizikoAdresBilgiler.Ilce;
                    teklif.RizikoAdresi.SemtBelde = model.RizikoAdresBilgiler.SemtBelde;
                    teklif.RizikoAdresi.Apartman = model.RizikoAdresBilgiler.Apartman;
                    teklif.RizikoAdresi.Mahalle = model.RizikoAdresBilgiler.Mahalle;
                    teklif.RizikoAdresi.Cadde = model.RizikoAdresBilgiler.Cadde;
                    teklif.RizikoAdresi.Sokak = model.RizikoAdresBilgiler.Sokak;
                    teklif.RizikoAdresi.PostaKodu = model.RizikoAdresBilgiler.PostaKodu;
                    teklif.RizikoAdresi.Daire = model.RizikoAdresBilgiler.Daire;
                    teklif.RizikoAdresi.Bina = model.RizikoAdresBilgiler.Bina;
                    teklif.RizikoAdresi.HanAptFab = model.RizikoAdresBilgiler.Han_Aprt_Fab;

                    #endregion

                    #region Riziko Diğer Bilgileri

                    teklif.AddSoru(KonutSorular.Belediye_Kodu, model.RizikoDigerBilgiler.BelediyeKodu.ToString());
                    teklif.AddSoru(KonutSorular.KislikMi, model.RizikoDigerBilgiler.KislikMi);
                    teklif.AddSoru(KonutSorular.Celik_Kapı_VarMi_EH, model.RizikoDigerBilgiler.CelikKapiVarMi);
                    teklif.AddSoru(KonutSorular.DemirPArmaklik_VarMi_EH, model.RizikoDigerBilgiler.DemirParmaklikVarMi);
                    teklif.AddSoru(KonutSorular.OzelGuvenlik_Alarm_VarMi_EH, model.RizikoDigerBilgiler.OzelGuvenlikAlarmVarMi);
                    teklif.AddSoru(KonutSorular.SigortalanacakYer_Kacinci_Katta, model.RizikoDigerBilgiler.SigortalanacakYerKacinciKatta.ToString());
                    teklif.AddSoru(KonutSorular.BosKalmaSuresi, model.RizikoDigerBilgiler.BosKalmaSuresi);
                    teklif.AddSoru(KonutSorular.Yapi_Tarzi, model.RizikoDigerBilgiler.YapiTarzi);
                    teklif.AddSoru(KonutSorular.Daire_Brut_Yuzolcumu_M2, model.RizikoDigerBilgiler.DaireBrutYuzOlcumu.ToString());

                    if (model.RizikoDigerBilgiler.EnflasyonOrani.HasValue)
                        teklif.AddSoru(KonutSorular.EnflasyonOrani, model.RizikoDigerBilgiler.EnflasyonOrani.Value.ToString());

                    if (model.RizikoDigerBilgiler.YillikEnflasyonKorumaOrani.HasValue)
                        teklif.AddSoru(KonutSorular.YillikEnflasyonundan_Koruma_Orani, model.RizikoDigerBilgiler.YillikEnflasyonKorumaOrani.Value.ToString());

                    if (model.RizikoDigerBilgiler.HasarsizlikIndirimOrani.HasValue)
                        teklif.AddSoru(KonutSorular.HasarsizlikIndirimOrani, model.RizikoDigerBilgiler.HasarsizlikIndirimOrani.Value.ToString());

                    #endregion

                    #region Teminat Bedeller

                    if (model.KonutTeminatBedelBilgileri.EsyaBedeli.HasValue)
                        teklif.AddSoru(KonutSorular.EsyaBedeli, model.KonutTeminatBedelBilgileri.EsyaBedeli.Value.ToString());

                    if (model.KonutTeminatBedelBilgileri.BinaBedeli.HasValue)
                        teklif.AddSoru(KonutSorular.BinaBedeli, model.KonutTeminatBedelBilgileri.BinaBedeli.Value.ToString());

                    #endregion

                    #region Sorular

                    //Eşya Bedeli Zorunlu olduğundan Bu teminatlar TRUE
                    teklif.AddSoru(KonutSorular.EsyaYangin, true);
                    teklif.AddSoru(KonutSorular.EsyaDeprem, true);
                    teklif.AddSoru(KonutSorular.EkTeminatEsya, true);
                    teklif.AddSoru(KonutSorular.Hirsizlik, true);
                    teklif.AddSoru(KonutSorular.Firtina, true);
                    teklif.AddSoru(KonutSorular.DepremYanardagPuskurmesiEsya, true);
                    teklif.AddSoru(KonutSorular.SelVeSuBaskini, true);
                    teklif.AddSoru(KonutSorular.DahiliSu, true);
                    teklif.AddSoru(KonutSorular.KaraTasitlariCarpmasi, true);
                    teklif.AddSoru(KonutSorular.KarAgirligi, true);
                    teklif.AddSoru(KonutSorular.EnkazKaldirmaEsya, true);
                    teklif.AddSoru(KonutSorular.YerKaymasi, true);
                    teklif.AddSoru(KonutSorular.DepremYanardagPuskurmesi, true);
                    teklif.AddSoru(KonutSorular.GLKHHKNHTeror, true);
                    teklif.AddSoru(KonutSorular.Duman, true);
                    teklif.AddSoru(KonutSorular.HavaTasitlariCarpmasi, true);


                    //Bina beleli girilirse aşağıdakiler zorunlu TRUE
                    if (model.KonutTeminatBedelBilgileri.BinaBedeli.HasValue)
                    {
                        teklif.AddSoru(KonutSorular.BinaYangin, true);
                        teklif.AddSoru(KonutSorular.BinaDeprem, true);
                        teklif.AddSoru(KonutSorular.EkTeminatBina, true);
                        teklif.AddSoru(KonutSorular.TemellerYangin, true);
                        teklif.AddSoru(KonutSorular.DepremYanardagPuskurmesiBina, true);
                        teklif.AddSoru(KonutSorular.EnkazKaldirmaBina, true);
                    }
                    else
                    {
                        teklif.AddSoru(KonutSorular.BinaYangin, model.KonutTeminatBilgileri.BinaYangin);
                        teklif.AddSoru(KonutSorular.BinaDeprem, model.KonutTeminatBilgileri.BinaDeprem);
                        teklif.AddSoru(KonutSorular.EkTeminatBina, model.KonutTeminatBilgileri.EkTeminatBina);
                        teklif.AddSoru(KonutSorular.TemellerYangin, model.KonutTeminatBilgileri.TemellerYangin);
                        teklif.AddSoru(KonutSorular.DepremYanardagPuskurmesiBina, model.KonutTeminatBilgileri.DepremYanardagPuskurmesiBina);
                        teklif.AddSoru(KonutSorular.EnkazKaldirmaBina, model.KonutTeminatBilgileri.EnkazKaldirmaBina);
                    }


                    //Isteğe bağlı teminatlar
                    teklif.AddSoru(KonutSorular.FerdiKaza, model.KonutTeminatBilgileri.FerdiKaza);
                    teklif.AddSoru(KonutSorular.FerdiKazaOlum, model.KonutTeminatBilgileri.FerdiKazaOlum);
                    teklif.AddSoru(KonutSorular.AsistanHizmeti, model.KonutTeminatBilgileri.AsistanHizmeti);
                    teklif.AddSoru(KonutSorular.HukuksalKoruma, model.KonutTeminatBilgileri.HukuksalKoruma);
                    teklif.AddSoru(KonutSorular.FerdiKazaSurekliSakatlik, model.KonutTeminatBilgileri.FerdiKazaSurekliSakatlik);
                    teklif.AddSoru(KonutSorular.Medline, model.KonutTeminatBilgileri.Medline);
                    teklif.AddSoru(KonutSorular.AcilTibbiHastaneFerdiKaza, model.KonutTeminatBilgileri.AcilTibbiHastaneFerdiKaza);


                    //Değer girilecek teminatlar (Kullanıcıdan değer alınıyor.)
                    teklif.AddSoru(KonutSorular.CamKirilmasi, model.KonutTeminatBilgileri.CamKirilmasi);

                    teklif.AddSoru(KonutSorular.MaliMesuliyetYangin, model.KonutTeminatBilgileri.MaliMesuliyetYangin);
                    teklif.AddSoru(KonutSorular.Kapkac, model.KonutTeminatBilgileri.Kapkac);


                    //    teklif.AddSoru(KonutSorular.DegerliEsyaYangin, model.KonutTeminatBilgileri.DegerliEsyaYangin);
                    teklif.AddSoru(KonutSorular.KiraKaybi, model.KonutTeminatBilgileri.KiraKaybi);
                    teklif.AddSoru(KonutSorular.MaliMesuliyetEkTeminat, model.KonutTeminatBilgileri.MaliSorumlulukEkTeminat);
                    teklif.AddSoru(KonutSorular.IzolasOlayBsYil, model.KonutTeminatBilgileri.IzolasOlayBsYil);

                    //Muafiyet soruları kullanıcıdan ALINMIYOR.
                    //1332 numaralı DEP.YANARD.PÜSK.(EŞYA) teminatı seçilirse WSMUMD alanının değerini default 5,
                    teklif.AddSoru(KonutSorular.MuafiyetEsya, "5");

                    //1331 numaralı DEP.YANARD.PÜSK.(BİNA) teminatı seçilirse WSMUBD alanının değerini default 2
                    if (model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi)
                        if (model.KonutTeminatBedelBilgileri.BinaBedeli.HasValue)
                            teklif.AddSoru(KonutSorular.Muafiyet_Bina, "2");

                    #endregion

                    #region Teminatlar

                    int? EsyaBedeli = model.KonutTeminatBedelBilgileri.EsyaBedeli;
                    int? BinaBedeli = model.KonutTeminatBedelBilgileri.BinaBedeli;


                    if (EsyaBedeli.HasValue)
                    {
                        //Eşya ile set edilen teminatlar
                        teklif.AddTeminat(KonutTeminatlar.EsyaYangin, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.EsyaDeprem, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.EkTeminatEsya, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.Hirsizlik, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.DepremYanardagPuskurmesiEsya, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.EnkazKaldirmaEsya, EsyaBedeli.Value, 0, 0, 0, 0);

                    }


                    if (EsyaBedeli.HasValue & BinaBedeli.HasValue)
                    {
                        //Bina ile set edilen teminatlar
                        teklif.AddTeminat(KonutTeminatlar.BinaYangin, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.EkTeminatBina, BinaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.DepremYanardagPuskurmesiBina, BinaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.TemellerYangin, BinaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.EnkazKaldirmaBina, BinaBedeli.Value, 0, 0, 0, 0);

                        //Ortak Set edilen teminatlar
                        teklif.AddTeminat(KonutTeminatlar.Firtina, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.SelVeSuBaskini, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.DahiliSu, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.KaraTasitlariCarpmasi, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.KarAgirligi, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.YerKaymasi, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.DepremYanardagPuskurmesi, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.GLKHHKNHTeror, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.Duman, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.HavaTasitlariCarpmasi, (EsyaBedeli.Value + BinaBedeli.Value), 0, 0, 0, 0);

                        //Dask bedeline bağlı teminat
                        if (model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi)
                        {
                            if (model.RizikoGenelBilgiler.DaskSigortaBedeli.HasValue)
                            {
                                int daskbedel = model.RizikoGenelBilgiler.DaskSigortaBedeli.Value;
                                teklif.AddTeminat(KonutTeminatlar.BinaDeprem, BinaBedeli.Value - daskbedel, 0, 0, 0, 0);
                            }
                        }
                        else
                            teklif.AddTeminat(KonutTeminatlar.BinaDeprem, BinaBedeli.Value, 0, 0, 0, 0);
                    }
                    else
                    {
                        teklif.AddTeminat(KonutTeminatlar.Firtina, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.SelVeSuBaskini, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.DahiliSu, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.KaraTasitlariCarpmasi, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.KarAgirligi, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.YerKaymasi, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.DepremYanardagPuskurmesi, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.GLKHHKNHTeror, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.Duman, EsyaBedeli.Value, 0, 0, 0, 0);
                        teklif.AddTeminat(KonutTeminatlar.HavaTasitlariCarpmasi, EsyaBedeli.Value, 0, 0, 0, 0);
                    }


                    //Değer girilecek teminatlar (Kullanıcıdan değer alınıyor.)
                    if (model.KonutTeminatBilgileri.CamKirilmasi & model.KonutTeminatBilgileri.CamKirilmasiBedel.HasValue)
                        teklif.AddTeminat(KonutTeminatlar.CamKirilmasi, model.KonutTeminatBilgileri.CamKirilmasiBedel.Value, 0, 0, 0, 0);

                    if (model.KonutTeminatBilgileri.Kapkac & model.KonutTeminatBilgileri.KapkacBedel.HasValue)
                        teklif.AddTeminat(KonutTeminatlar.Kapkac, model.KonutTeminatBilgileri.KapkacBedel.Value, 0, 0, 0, 0);

                    if (model.KonutTeminatBilgileri.MaliMesuliyetYangin & model.KonutTeminatBilgileri.MaliMesuliyetYanginBedel.HasValue)
                        teklif.AddTeminat(KonutTeminatlar.MaliMesuliyetYangin, model.KonutTeminatBilgileri.MaliMesuliyetYanginBedel.Value, 0, 0, 0, 0);

                    //if (model.KonutTeminatBilgileri.DegerliEsyaYangin)
                    //    teklif.AddTeminat(KonutSorular.DegerliEsyaYangin, model.KonutTeminatBilgileri.DegerliEsyaYanginBedel.ToString());
                    if (model.KonutTeminatBilgileri.KiraKaybi & model.KonutTeminatBilgileri.KiraKaybiBedel.HasValue)
                        teklif.AddTeminat(KonutTeminatlar.KiraKaybi, model.KonutTeminatBilgileri.KiraKaybiBedel.Value, 0, 0, 0, 0);
                    if (model.KonutTeminatBilgileri.MaliSorumlulukEkTeminat & model.KonutTeminatBilgileri.MaliSorumlulukEkTeminatBedel.HasValue)
                        teklif.AddTeminat(KonutTeminatlar.MaliMesuliyetEkTeminat, model.KonutTeminatBilgileri.MaliSorumlulukEkTeminatBedel.Value, 0, 0, 0, 0);
                    if (model.KonutTeminatBilgileri.IzolasOlayBsYil & model.KonutTeminatBilgileri.IzolasOlayBsYilBedel.HasValue)
                        teklif.AddTeminat(KonutTeminatlar.IzolasOlayBsYil, model.KonutTeminatBilgileri.IzolasOlayBsYilBedel.Value, 0, 0, 0, 0);

                    #endregion

                    #region Not

                    if (!String.IsNullOrEmpty(model.KonutNotModel.Not))
                    {
                        teklif.GenelBilgiler.TeklifNot = new TeklifNot();
                        teklif.GenelBilgiler.TeklifNot.Aciklama = model.KonutNotModel.Not;
                    }

                    #endregion

                    #endregion

                    #region Teklif return

                    IKonutTeklif konutTeklif = new KonutTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    foreach (var item in model.TeklifUM)
                    {
                        if (item.TeklifAl)
                            konutTeklif.AddUretimMerkezi(item.TUMKodu);
                    }

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.Odeme.OdemeTipi;

                    if (!model.Odeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                        konutTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        konutTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = konutTeklif.Hesapla(teklif);

                    return Json(new { id = isDurum.IsId, g = isDurum.Guid });

                    #endregion
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

            #endregion

            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }

        private KonutPoliceOdemeModel KonutPoliceOdemeModel(ITeklif teklif)
        {
            KonutPoliceOdemeModel model = new KonutPoliceOdemeModel();

            if (teklif != null && teklif.GenelBilgiler != null)
            {

                model.BrutPrim = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value : 0;
                model.NetPrim = teklif.GenelBilgiler.NetPrim.HasValue ? teklif.GenelBilgiler.NetPrim.Value : 0;
                model.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : 0;
                model.Vergi = teklif.GenelBilgiler.ToplamVergi.HasValue ? teklif.GenelBilgiler.ToplamVergi.Value : 0;

                model.TUMKodu = teklif.GenelBilgiler.TUMKodu;

                TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);

                model.TUMUnvani = tum.Unvani;
                model.TUMLogoURL = tum.Logo;
                model.PoliceURL = teklif.GenelBilgiler.PDFPolice;

                model.teklifId = teklif.GenelBilgiler.TeklifId;
                model.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
            }
            return model;
        }

        #region Ekle Method

        private KonutRizikoGenelBilgiler KonutRizikoGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            KonutRizikoGenelBilgiler model = new KonutRizikoGenelBilgiler();

            //Evet ise sigorta şirketi ve poliçe vade tarihi bilgileri alınacak.
            model.YururlukteDaskPolicesiVarmi = false;
            model.SigortaSirketleri = base.SigortaSirketleri;
            model.RehinliAlacakliDainMurtehinVarmi = false;
            model.Tipler = new SelectList(TeklifProvider.DaskKurumTipleri(), "Value", "Text", "").ToList();
            model.Kurum_Bankalar = new SelectList(_CRService.GetListDaskKurumlar(), "KurumKodu", "KurumAdi", "").ListWithOptionLabel();
            model.Subeler = new List<SelectListItem>();
            model.DovizKodlari = new SelectList(TeklifProvider.DaskDovizKodlari(), "Value", "Text", "").ListWithOptionLabel();


            if (teklifId.HasValue & teklif != null)
            {
                model.YururlukteDaskPolicesiVarmi = teklif.ReadSoru(KonutSorular.Yururlukte_dask_policesi_VarYok, false); ;
                model.RehinliAlacakliDainMurtehinVarmi = teklif.ReadSoru(KonutSorular.RA_Dain_i_Muhtehin_VarYok, false);

                if (model.YururlukteDaskPolicesiVarmi)
                {
                    model.SigortaSirketi = teklif.ReadSoru(KonutSorular.Dask_Police_Sigorta_Sirketi, "0");
                    model.PoliceninVadeTarihi = teklif.ReadSoru(KonutSorular.Dask_Police_Vade_Tarihi, DateTime.MinValue);
                    model.PoliceNumarasi = teklif.ReadSoru(KonutSorular.Dask_Police_Numarasi, String.Empty);
                    model.DaskSigortaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.Dask_Sigorta_Bedeli, "0"));
                }

                if (model.RehinliAlacakliDainMurtehinVarmi)
                {
                    model.Tipi = teklif.ReadSoru(KonutSorular.RA_Tipi_Banka_Finansal_Kurum, "0");
                    model.KurumBanka = (int)teklif.ReadSoru(KonutSorular.RA_Kurum_Banka, 0);
                    model.Subeler = new SelectList(_CRService.GetListDaskSubeler(model.KurumBanka), "SubeKodu", "SubeAdi", "").ListWithOptionLabel();
                    model.Sube = (int)teklif.ReadSoru(KonutSorular.RA_Sube, 0);
                    model.KrediReferansNo_HesapSozlesmeNo = teklif.ReadSoru(KonutSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, "0");
                    model.KrediBitisTarihi = teklif.ReadSoru(KonutSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);

                    string kreditutari = teklif.ReadSoru(KonutSorular.RA_Kredi_Tutari, "0");
                    if (kreditutari != "0")
                        model.KrediTutari = Convert.ToInt32(kreditutari);

                    model.DovizKodu = (byte)teklif.ReadSoru(KonutSorular.RA_Doviz_Kodu, 0);
                }
            }

            return model;
        }

        private KonutRizikoAdresModel KonutRizikoAdresModel(int? teklifId, ITeklif teklif)
        {
            KonutRizikoAdresModel model = new KonutRizikoAdresModel();

            model.HanAptFabList = new SelectList(TeklifProvider.HanAptFabTipleri(), "Value", "Text", "").ListWithOptionLabel();

            if (teklifId.HasValue && teklif != null)
            {
                model.Iller = new SelectList(_UlkeService.GetIlList(), "IlKodu", "IlAdi", "").ListWithOptionLabelIller();
                if (teklif.RizikoAdresi != null)
                {
                    TeklifRizikoAdresi adres = teklif.RizikoAdresi;
                    int ilkodu = adres.IlKodu.HasValue ? adres.IlKodu.Value : 999;
                    int ilcekodu = adres.IlceKodu.HasValue ? adres.IlceKodu.Value : 977;

                    model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", ilkodu.ToString()), "IlceKodu", "IlceAdi", "").ListWithOptionLabelIller();

                    model.Il = ilkodu;
                    model.Ilce = ilcekodu;
                    model.Apartman = adres.Apartman;
                    model.Bina = adres.Bina;
                    model.Cadde = adres.Cadde;
                    model.Daire = adres.Daire;
                    model.Han_Aprt_Fab = adres.HanAptFab;
                    model.Mahalle = adres.Mahalle;
                    model.PostaKodu = adres.PostaKodu;
                    model.SemtBelde = adres.SemtBelde;
                    model.Sokak = adres.Sokak;
                }
            }
            else
            {
                model.Il = 34;
                model.Iller = new SelectList(_UlkeService.GetIlList(), "IlKodu", "IlAdi", "").ListWithOptionLabelIller();
                model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", "34"), "IlceKodu", "IlceAdi", "").ListWithOptionLabelIller();
            }

            return model;
        }

        private KonutRizikoDigerBilgiler KonutRizikoDigerBilgiler(int? teklifId, ITeklif teklif)
        {
            KonutRizikoDigerBilgiler model = new KonutRizikoDigerBilgiler();

            model.YapiTarzlari = new SelectList(TeklifProvider.KonutBinaYapiTazrlari(), "Value", "Text", "").ListWithOptionLabel();
            model.BosKalmaSureleri = new SelectList(TeklifProvider.BosKalmaSureleri(), "Value", "Text", "").ListWithOptionLabel();

            if (teklifId.HasValue & teklif != null)
            {
                string belediyeKod = teklif.ReadSoru(KonutSorular.Belediye_Kodu, String.Empty);
                if (!String.IsNullOrEmpty(belediyeKod))
                {
                    if (teklif.RizikoAdresi.IlKodu.HasValue)
                        model.Belediyeler = new SelectList(_CRService.GetListBelediye(teklif.RizikoAdresi.IlKodu.Value), "BelediyeKodu", "BelediyeAdi", "").ListWithOptionLabel();
                    model.BelediyeKodu = Convert.ToInt32(belediyeKod);
                }

                model.YapiTarzi = teklif.ReadSoru(KonutSorular.Yapi_Tarzi, String.Empty);
                model.BosKalmaSuresi = teklif.ReadSoru(KonutSorular.BosKalmaSuresi, String.Empty);

                model.CelikKapiVarMi = teklif.ReadSoru(KonutSorular.Celik_Kapı_VarMi_EH, false);
                model.KislikMi = teklif.ReadSoru(KonutSorular.KislikMi, false);
                model.DemirParmaklikVarMi = teklif.ReadSoru(KonutSorular.DemirPArmaklik_VarMi_EH, false);
                model.OzelGuvenlikAlarmVarMi = teklif.ReadSoru(KonutSorular.OzelGuvenlik_Alarm_VarMi_EH, false);
                model.DaireBrutYuzOlcumu = Convert.ToInt32(teklif.ReadSoru(KonutSorular.Daire_Brut_Yuzolcumu_M2, "0"));
                model.SigortalanacakYerKacinciKatta = Convert.ToByte(teklif.ReadSoru(KonutSorular.SigortalanacakYer_Kacinci_Katta, String.Empty));

                string enflasyonO = teklif.ReadSoru(KonutSorular.EnflasyonOrani, String.Empty);
                if (!String.IsNullOrEmpty(enflasyonO) & enflasyonO != "0")
                    model.EnflasyonOrani = Convert.ToInt32(enflasyonO);

                string hasarsizlik = teklif.ReadSoru(KonutSorular.HasarsizlikIndirimOrani, String.Empty);
                if (!String.IsNullOrEmpty(hasarsizlik) & hasarsizlik != "0")
                    model.HasarsizlikIndirimOrani = Convert.ToInt32(hasarsizlik);

                string yillikenflasyonO = teklif.ReadSoru(KonutSorular.YillikEnflasyonundan_Koruma_Orani, String.Empty);
                if (!String.IsNullOrEmpty(yillikenflasyonO) & yillikenflasyonO != "0")
                    model.YillikEnflasyonKorumaOrani = Convert.ToInt32(yillikenflasyonO);
            }
            else
            {
                model.Belediyeler = new SelectList(_CRService.GetListBelediye(34), "BelediyeKodu", "BelediyeAdi", "").ListWithOptionLabel();
                model.KislikMi = true;
            }

            return model;
        }

        private KonutTeminatBedelBilgileri KonutTeminatBedelBilgileri(int? teklifId, ITeklif teklif)
        {
            KonutTeminatBedelBilgileri model = new KonutTeminatBedelBilgileri();

            if (teklifId.HasValue && teklif != null)
            {
                int esyabedel = Convert.ToInt32(teklif.ReadSoru(KonutSorular.EsyaBedeli, "0"));
                if (esyabedel > 0)
                    model.EsyaBedeli = esyabedel;
                int binabedel = Convert.ToInt32(teklif.ReadSoru(KonutSorular.BinaBedeli, "0"));
                if (binabedel > 0)
                    model.BinaBedeli = binabedel;
            }

            return model;
        }

        private KonutTeminatBilgileri KonutTeminatBilgileri(int? teklifId, ITeklif teklif)
        {
            KonutTeminatBilgileri model = new KonutTeminatBilgileri();

            if (teklifId.HasValue && teklif != null)
            {
                int EsyaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.EsyaBedeli, "0"));
                int BinaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.BinaBedeli, "0"));

                //Zorunlu   teminatlar
                model.EsyaYangin = true;
                model.EsyaDeprem = true;
                model.EkTeminatEsya = true;
                model.Hirsizlik = true;
                model.Firtina = true;
                model.DepremYanardagPuskurmesiEsya = true;
                model.SelVeSuBaskini = true;
                model.DahiliSu = true;
                model.KaraTasitlariCarpmasi = true;
                model.KarAgirligi = true;
                model.EnkazKaldirmaEsya = true;
                model.YerKaymasi = true;
                model.DepremYanardagPuskurmesi = true;
                model.GLKHHKNHTeror = true;
                model.Duman = true;
                model.HavaTasitlariCarpmasi = true;

                //Esya bedeline Bağlı TEminatlar
                if (BinaBedeli > 0)
                {
                    model.BinaYangin = true;
                    model.BinaDeprem = true;
                    model.EkTeminatBina = true;
                    model.DepremYanardagPuskurmesiBina = true;
                    model.TemellerYangin = true;
                    model.EnkazKaldirmaBina = true;

                    if (teklif.ReadSoru(KonutSorular.Yururlukte_dask_policesi_VarYok, false))
                    {
                        int daskBedel = Convert.ToInt32(teklif.ReadSoru(KonutSorular.Dask_Sigorta_Bedeli, "0"));
                        model.BinaDepremBedel = BinaBedeli - daskBedel;
                    }
                    else
                        model.BinaDepremBedel = BinaBedeli;
                }

                model.AsistanHizmeti = teklif.ReadSoru(KonutSorular.AsistanHizmeti, false);
                model.HukuksalKoruma = teklif.ReadSoru(KonutSorular.HukuksalKoruma, false);
                model.FerdiKaza = teklif.ReadSoru(KonutSorular.FerdiKaza, false);
                model.FerdiKazaOlum = teklif.ReadSoru(KonutSorular.FerdiKazaOlum, false);
                model.FerdiKazaSurekliSakatlik = teklif.ReadSoru(KonutSorular.FerdiKazaSurekliSakatlik, false);
                model.Medline = teklif.ReadSoru(KonutSorular.Medline, false);
                model.AcilTibbiHastaneFerdiKaza = teklif.ReadSoru(KonutSorular.AcilTibbiHastaneFerdiKaza, false);

                //Cam Kırılması
                if (teklif.ReadSoru(KonutSorular.CamKirilmasi, false))
                {
                    model.CamKirilmasi = true;
                    TeklifTeminat camKirilmasi = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.CamKirilmasi).FirstOrDefault();
                    if (camKirilmasi != null)
                        model.CamKirilmasiBedel = Convert.ToInt32(camKirilmasi.TeminatBedeli ?? null);
                }

                //Kapkac
                if (teklif.ReadSoru(KonutSorular.CamKirilmasi, false))
                {
                    model.Kapkac = true;
                    TeklifTeminat kapkacBedeli = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.Kapkac).FirstOrDefault();
                    if (kapkacBedeli != null)
                        model.KapkacBedel = Convert.ToInt32(kapkacBedeli.TeminatBedeli ?? null);
                }

                //Mali mesuliyet Yangın
                if (teklif.ReadSoru(KonutSorular.MaliMesuliyetYangin, false))
                {
                    model.MaliMesuliyetYangin = true;
                    TeklifTeminat maliMesuliyetYanginBedeli = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetYangin).FirstOrDefault();
                    if (maliMesuliyetYanginBedeli != null)
                        model.MaliMesuliyetYanginBedel = Convert.ToInt32(maliMesuliyetYanginBedeli.TeminatBedeli ?? null);
                }
                //Değerli Eşya Yangın
                //if (teklif.ReadSoru(KonutSorular.DegerliEsyaYangin, false))
                //{
                //    model.DegerliEsyaYangin = true;
                //    TeklifTeminat degerliesya = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.DegerliEsyaYangin).FirstOrDefault();
                //    if (degerliesya != null)
                //        model.DegerliEsyaYanginBedel = Convert.ToInt32(degerliesya.TeminatBedeli ?? null);
                //}

                //KiraKaybiBedel
                if (teklif.ReadSoru(KonutSorular.KiraKaybi, false))
                {
                    model.KiraKaybi = true;
                    TeklifTeminat kirakaybi = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.KiraKaybi).FirstOrDefault();
                    if (kirakaybi != null)
                        model.KiraKaybiBedel = Convert.ToInt32(kirakaybi.TeminatBedeli ?? null);
                }

                //MaliSorumlulukEkTeminatBedel
                if (teklif.ReadSoru(KonutSorular.MaliMesuliyetEkTeminat, false))
                {
                    model.MaliSorumlulukEkTeminat = true;
                    TeklifTeminat maliSorumluluk = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetEkTeminat).FirstOrDefault();
                    if (maliSorumluluk != null)
                        model.MaliSorumlulukEkTeminatBedel = Convert.ToInt32(maliSorumluluk.TeminatBedeli ?? null);
                }

                //zolas./Olay Bş/Yıl
                if (teklif.ReadSoru(KonutSorular.IzolasOlayBsYil, false))
                {
                    model.IzolasOlayBsYil = true;
                    TeklifTeminat izolasyonolay = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.IzolasOlayBsYil).FirstOrDefault();
                    if (izolasyonolay != null)
                        model.IzolasOlayBsYilBedel = Convert.ToInt32(izolasyonolay.TeminatBedeli ?? null);
                }
            }
            else
                foreach (PropertyInfo prop in model.GetType().GetProperties())
                    if (prop.PropertyType == typeof(bool))
                        prop.SetValue(model, true, null);

            return model;
        }

        #endregion

        #region Detay Method

        private KonutRizikoGenelBilgiler KonutRizikoGenelBilgilerDetay(ITeklif teklif)
        {
            ISigortaSirketleriService _SigortaSirketiService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
            KonutRizikoGenelBilgiler model = new KonutRizikoGenelBilgiler();

            try
            {
                model.YururlukteDaskPolicesiVarmi = teklif.ReadSoru(KonutSorular.Yururlukte_dask_policesi_VarYok, false); ;
                model.RehinliAlacakliDainMurtehinVarmi = teklif.ReadSoru(KonutSorular.RA_Dain_i_Muhtehin_VarYok, false);

                if (model.YururlukteDaskPolicesiVarmi == true)
                {
                    SigortaSirketleri sirket = _SigortaSirketiService.GetSirket(teklif.ReadSoru(KonutSorular.Dask_Police_Sigorta_Sirketi, "0"));

                    if (sirket != null)
                        model.SigortaSirketi = sirket.SirketAdi;

                    model.PoliceninVadeTarihi = teklif.ReadSoru(KonutSorular.Dask_Police_Vade_Tarihi, DateTime.MinValue);
                    model.PoliceNumarasi = teklif.ReadSoru(KonutSorular.Dask_Police_Numarasi, String.Empty);
                    model.DaskSigortaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.Dask_Sigorta_Bedeli, "0"));
                }


                if (model.RehinliAlacakliDainMurtehinVarmi == true)
                {
                    model.Tipi = teklif.ReadSoru(KonutSorular.RA_Tipi_Banka_Finansal_Kurum, "0") == "1" ? "Banka" : "Finansal Kurum";
                    model.KrediReferansNo_HesapSozlesmeNo = teklif.ReadSoru(KonutSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, String.Empty); ;
                    model.KrediBitisTarihi = teklif.ReadSoru(KonutSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);
                    model.DovizKoduText = DaskDovizTipleri.DovizTipi(Convert.ToByte(teklif.ReadSoru(KonutSorular.RA_Doviz_Kodu, "0")));

                    string kreditutari = teklif.ReadSoru(KonutSorular.RA_Kredi_Tutari, "0");
                    if (kreditutari != "0")
                        model.KrediTutari = Convert.ToInt32(kreditutari);

                    DaskKurumlar kurum = _CRService.GetDaskKurum(Convert.ToInt32(teklif.ReadSoru(KonutSorular.RA_Kurum_Banka, "0")));
                    if (kurum != null)
                    {
                        model.KurumBankaAdi = kurum.KurumAdi;
                        DaskSubeler sube = _CRService.GetDaskSube(kurum.KurumKodu, Convert.ToInt32(teklif.ReadSoru(KonutSorular.RA_Sube, "0")));
                        if (sube != null)
                            model.SubeAdi = sube.SubeAdi;
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }

            return model;
        }

        private KonutRizikoAdresModel KonutRizikoAdresModelDetay(ITeklif teklif)
        {
            KonutRizikoAdresModel model = new KonutRizikoAdresModel();
            try
            {
                TeklifRizikoAdresi adres = teklif.RizikoAdresi;
                if (adres != null)
                {
                    Il il = _UlkeService.GetIl("TUR", adres.IlKodu.HasValue ? adres.IlKodu.Value.ToString() : "");
                    if (il != null)
                        model.IlAdi = il.IlAdi;
                    Ilce ilce = _UlkeService.GetIlce(adres.IlceKodu.HasValue ? adres.IlceKodu.Value : 999);
                    if (ilce != null)
                        model.IlceAdi = ilce.IlceAdi;


                    model.SemtBelde = adres.SemtBelde;
                    model.Mahalle = adres.Mahalle;
                    model.Cadde = adres.Cadde;
                    model.Bina = adres.Bina;
                    model.Daire = adres.Daire;
                    model.PostaKodu = adres.PostaKodu;
                    model.Apartman = adres.Apartman;
                    model.Sokak = adres.Sokak;
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }

            return model;
        }

        private KonutRizikoDigerBilgiler KonutRizikoDigerBilgilerDetay(ITeklif teklif)
        {
            KonutRizikoDigerBilgiler model = new KonutRizikoDigerBilgiler();

            try
            {
                model.CelikKapiVarMi = teklif.ReadSoru(KonutSorular.Celik_Kapı_VarMi_EH, false);
                model.DemirParmaklikVarMi = teklif.ReadSoru(KonutSorular.DemirPArmaklik_VarMi_EH, false);
                model.OzelGuvenlikAlarmVarMi = teklif.ReadSoru(KonutSorular.OzelGuvenlik_Alarm_VarMi_EH, false);
                model.KislikMi = teklif.ReadSoru(KonutSorular.KislikMi, false);

                string yapiTarzi = teklif.ReadSoru(KonutSorular.Yapi_Tarzi, String.Empty);
                if (!String.IsNullOrEmpty(yapiTarzi))
                    model.YapiTarzi = KonutYapiTarzlari.YapiTarzi(Convert.ToByte(yapiTarzi));

                string bosKalmaSuresi = teklif.ReadSoru(KonutSorular.BosKalmaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(bosKalmaSuresi))
                    model.BosKalmaSuresi = BosKalmaSuresi.Sure(Convert.ToByte(bosKalmaSuresi));

                string yuzOlcumu = teklif.ReadSoru(KonutSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                if (!String.IsNullOrEmpty(yuzOlcumu))
                    model.DaireBrutYuzOlcumu = Convert.ToInt32(yuzOlcumu);

                string kat = teklif.ReadSoru(KonutSorular.SigortalanacakYer_Kacinci_Katta, String.Empty);
                if (!String.IsNullOrEmpty(kat))
                    model.SigortalanacakYerKacinciKatta = Convert.ToByte(kat);

                string enflasyon = teklif.ReadSoru(KonutSorular.EnflasyonOrani, String.Empty);
                if (!String.IsNullOrEmpty(enflasyon))
                    model.EnflasyonOrani = Convert.ToInt32(enflasyon);

                string hasarsizLik = teklif.ReadSoru(KonutSorular.HasarsizlikIndirimOrani, String.Empty);
                if (!String.IsNullOrEmpty(hasarsizLik))
                    model.HasarsizlikIndirimOrani = Convert.ToInt32(hasarsizLik);

                string yillikEnflasyonKorumaO = teklif.ReadSoru(KonutSorular.YillikEnflasyonundan_Koruma_Orani, String.Empty);
                if (!String.IsNullOrEmpty(yillikEnflasyonKorumaO))
                    model.YillikEnflasyonKorumaOrani = Convert.ToInt32(yillikEnflasyonKorumaO);

                string belediyeKod = teklif.ReadSoru(KonutSorular.Belediye_Kodu, String.Empty);
                if (!String.IsNullOrEmpty(belediyeKod) && teklif.RizikoAdresi.IlKodu.HasValue)
                {
                    int belediyeKodu = Convert.ToInt32(belediyeKod);
                    Belediye belediye = _CRService.GetBelediye(teklif.RizikoAdresi.IlKodu.Value, belediyeKodu);
                    if (belediye != null)
                        model.BelediyeAdi = belediye.BelediyeAdi;
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }

            return model;
        }

        private KonutTeminatBedelBilgileri KonutTeminatBedelBilgileriDetay(ITeklif teklif)
        {
            KonutTeminatBedelBilgileri model = new KonutTeminatBedelBilgileri();

            model.EsyaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.EsyaBedeli, "0"));
            model.BinaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.BinaBedeli, "0"));

            return model;
        }

        private KonutTeminatBilgileri KonutTeminatBilgileriDetay(ITeklif teklif)
        {
            KonutTeminatBilgileri model = new KonutTeminatBilgileri();

            int EsyaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.EsyaBedeli, "0"));
            int BinaBedeli = Convert.ToInt32(teklif.ReadSoru(KonutSorular.BinaBedeli, "0"));

            //Zorunlu   teminatlar
            model.EsyaYangin = true;
            model.EsyaDeprem = true;
            model.EkTeminatEsya = true;
            model.Hirsizlik = true;
            model.Firtina = true;
            model.DepremYanardagPuskurmesiEsya = true;
            model.SelVeSuBaskini = true;
            model.DahiliSu = true;
            model.KaraTasitlariCarpmasi = true;
            model.KarAgirligi = true;
            model.EnkazKaldirmaEsya = true;
            model.YerKaymasi = true;
            model.DepremYanardagPuskurmesi = true;
            model.GLKHHKNHTeror = true;
            model.Duman = true;
            model.HavaTasitlariCarpmasi = true;

            //Bina bedeline Bağlı TEminatlar
            if (BinaBedeli > 0)
            {
                model.BinaYangin = true;
                model.BinaDeprem = true;
                model.EkTeminatBina = true;
                model.DepremYanardagPuskurmesiBina = true;
                model.TemellerYangin = true;
                model.EnkazKaldirmaBina = true;

                if (teklif.ReadSoru(KonutSorular.Yururlukte_dask_policesi_VarYok, false))
                {
                    int daskBedel = Convert.ToInt32(teklif.ReadSoru(KonutSorular.Dask_Sigorta_Bedeli, "0"));
                    model.BinaDepremBedel = BinaBedeli - daskBedel;
                }
                else
                    model.BinaDepremBedel = BinaBedeli;
            }

            //Değer almayan teminatlar
            model.AsistanHizmeti = teklif.ReadSoru(KonutSorular.AsistanHizmeti, false);
            model.HukuksalKoruma = teklif.ReadSoru(KonutSorular.HukuksalKoruma, false);
            model.FerdiKaza = teklif.ReadSoru(KonutSorular.FerdiKaza, false);
            model.FerdiKazaOlum = teklif.ReadSoru(KonutSorular.FerdiKazaOlum, false);
            model.FerdiKazaSurekliSakatlik = teklif.ReadSoru(KonutSorular.FerdiKazaSurekliSakatlik, false);
            model.Medline = teklif.ReadSoru(KonutSorular.Medline, false);
            model.AcilTibbiHastaneFerdiKaza = teklif.ReadSoru(KonutSorular.AcilTibbiHastaneFerdiKaza, false);

            //Cam Kırılması
            if (teklif.ReadSoru(KonutSorular.CamKirilmasi, false))
            {
                model.CamKirilmasi = true;
                TeklifTeminat camKirilmasi = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.CamKirilmasi).FirstOrDefault();
                if (camKirilmasi != null)
                    model.CamKirilmasiBedel = Convert.ToInt32(camKirilmasi.TeminatBedeli ?? null);
            }
            //Kapkac
            if (teklif.ReadSoru(KonutSorular.CamKirilmasi, false))
            {
                model.Kapkac = true;
                TeklifTeminat kapkacBedeli = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.Kapkac).FirstOrDefault();
                if (kapkacBedeli != null)
                    model.KapkacBedel = Convert.ToInt32(kapkacBedeli.TeminatBedeli ?? null);
            }

            //Mali mesuliyet Yangın
            if (teklif.ReadSoru(KonutSorular.MaliMesuliyetYangin, false))
            {
                model.MaliMesuliyetYangin = true;
                TeklifTeminat maliMesuliyetYanginBedeli = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetYangin).FirstOrDefault();
                if (maliMesuliyetYanginBedeli != null)
                    model.MaliMesuliyetYanginBedel = Convert.ToInt32(maliMesuliyetYanginBedeli.TeminatBedeli ?? null);
            }
            //Değerli Eşya Yangın
            //if (teklif.ReadSoru(KonutSorular.DegerliEsyaYangin, false))
            //{
            //    model.DegerliEsyaYangin = true;
            //    TeklifTeminat degerliesya = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.DegerliEsyaYangin).FirstOrDefault();
            //    if (degerliesya != null)
            //        model.DegerliEsyaYanginBedel = Convert.ToInt32(degerliesya.TeminatBedeli ?? null);
            //}

            //KiraKaybiBedel
            if (teklif.ReadSoru(KonutSorular.KiraKaybi, false))
            {
                model.KiraKaybi = true;
                TeklifTeminat kirakaybi = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.KiraKaybi).FirstOrDefault();
                if (kirakaybi != null)
                    model.KiraKaybiBedel = Convert.ToInt32(kirakaybi.TeminatBedeli ?? null);
            }

            //MaliSorumlulukEkTeminatBedel
            if (teklif.ReadSoru(KonutSorular.MaliMesuliyetEkTeminat, false))
            {
                model.MaliSorumlulukEkTeminat = true;
                TeklifTeminat maliSorumluluk = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.MaliMesuliyetEkTeminat).FirstOrDefault();
                if (maliSorumluluk != null)
                    model.MaliSorumlulukEkTeminatBedel = Convert.ToInt32(maliSorumluluk.TeminatBedeli ?? null);
            }

            //zolas./Olay Bş/Yıl
            if (teklif.ReadSoru(KonutSorular.IzolasOlayBsYil, false))
            {
                model.IzolasOlayBsYil = true;
                TeklifTeminat izolasyonolay = teklif.Teminatlar.Where(s => s.TeminatKodu == KonutTeminatlar.IzolasOlayBsYil).FirstOrDefault();
                if (izolasyonolay != null)
                    model.IzolasOlayBsYilBedel = Convert.ToInt32(izolasyonolay.TeminatBedeli ?? null);
            }

            return model;
        }

        #endregion

        #region Out Method

        public ActionResult GetListBelediye(int IlKodu)
        {
            return Json(new SelectList(_CRService.GetListBelediye(IlKodu), "BelediyeKodu", "BelediyeAdi", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        private List<SelectListItem> FillMuafiyet(List<DepremMuafiyet> muafiyetler, int kodu)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in muafiyetler.Where(s => s.TeminatKodu == kodu))
            {
                SelectListItem listItem = new SelectListItem();
                listItem.Value = item.YazlikKislik + "-" + item.Kademe;
                listItem.Text = (item.YazlikKislik == 2 ? babonline.Summery : babonline.WinterWeight) + " " + item.Kademe;
                list.Add(listItem);
            }

            return list;
        }

    }
}
