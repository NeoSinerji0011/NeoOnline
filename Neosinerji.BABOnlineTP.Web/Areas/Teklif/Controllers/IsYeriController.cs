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
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.IsYeri)]
    public class IsYeriController : TeklifController
    {
        public IsYeriController(ITVMService tvmService,
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
            IsYeriModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            IsYeriModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public IsYeriModel EkleModel(int? id, int? teklifId)
        {
            IsYeriModel model = new IsYeriModel();
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

                #region İş yeri

                model.RizikoGenelBilgiler = IsYeriRizikoGenelBilgiler(teklifId, teklif);
                model.RizikoAdresBilgiler = IsYeriRizikoAdresModel(teklifId, teklif);
                model.RizikoDigerBilgiler = IsYeriRizikoDigerBilgiler(teklifId, teklif);
                model.IsYeriTeminatBedelBilgileri = IsYeriTeminatBedelBilgileri(teklifId, teklif);
                model.IsYeriTeminatBilgileri = IsYeriTeminatBilgileri(teklifId, teklif);

                model.IsYeriNotModel = new IsYeriNotModel();
                if (teklif != null & teklifId.HasValue)
                    if (teklif.GenelBilgiler.TeklifNot != null)
                        model.IsYeriNotModel.Not = teklif.GenelBilgiler.TeklifNot.Aciklama;

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.IsYeri);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme
                model.Odeme = new IsYeriTeklifOdemeModel();
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

            DetayIsYeriModel model = new DetayIsYeriModel();

            #region Teklif Genel


            IsYeriTeklif isYeriTeklif = new IsYeriTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = isYeriTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(isYeriTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(isYeriTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (isYeriTeklif.Teklif.Sigortalilar.Count > 0 &&
               (isYeriTeklif.Teklif.SigortaEttiren.MusteriKodu != isYeriTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(isYeriTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Riziko

            model.RizikoGenelBilgiler = IsYeriRizikoGenelBilgilerDetay(isYeriTeklif.Teklif);
            model.RizikoAdresBilgiler = IsYeriRizikoAdresModelDetay(isYeriTeklif.Teklif);
            model.RizikoDigerBilgiler = IsYeriRizikoDigerBilgilerDetay(isYeriTeklif.Teklif);
            model.IsYeriTeminatBilgileri = IsYeriTeminatBilgileriDetay(isYeriTeklif.Teklif);
            model.IsYeriTeminatBedelBilgileri = IsYeriTeminatBedelBilgileriDetay(isYeriTeklif.Teklif);

            model.IsYeriNotModel = new IsYeriNotModel();
            if (isYeriTeklif.Teklif.GenelBilgiler.TeklifNot != null)
                model.IsYeriNotModel.Not = isYeriTeklif.Teklif.GenelBilgiler.TeklifNot.Aciklama;

            #endregion

            #region Teklif Fiyat

            model.Fiyat = IsYeriFiyat(isYeriTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            model.KrediKarti.KK_OdemeSekli = isYeriTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            model.KrediKarti.KK_OdemeTipi = isYeriTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = isYeriTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
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
            DetayIsYeriModel model = new DetayIsYeriModel();

            #region Teklif Genel

            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif isYeriTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = isYeriTeklif.GenelBilgiler.TeklifId;

            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(isYeriTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(isYeriTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Riziko

            model.RizikoGenelBilgiler = IsYeriRizikoGenelBilgilerDetay(isYeriTeklif);
            model.RizikoAdresBilgiler = IsYeriRizikoAdresModelDetay(isYeriTeklif);
            model.RizikoDigerBilgiler = IsYeriRizikoDigerBilgilerDetay(isYeriTeklif);
            model.IsYeriTeminatBilgileri = IsYeriTeminatBilgileriDetay(isYeriTeklif);
            model.IsYeriTeminatBedelBilgileri = IsYeriTeminatBedelBilgileriDetay(isYeriTeklif);

            model.IsYeriNotModel = new IsYeriNotModel();
            if (isYeriTeklif.GenelBilgiler.TeklifNot != null)
                model.IsYeriNotModel.Not = isYeriTeklif.GenelBilgiler.TeklifNot.Aciklama;

            #endregion

            #region Teklif Odeme

            model.OdemeBilgileri = IsYeriPoliceOdemeModel(teklif);

            #endregion

            return View(model);
        }

        private TeklifFiyatModel IsYeriFiyat(IsYeriTeklif isYeriTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = isYeriTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = isYeriTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = isYeriTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = isYeriTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = isYeriTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = isYeriTeklif.GetIsDurum();
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
        public ActionResult OdemeAl(OdemeIsYeriModel model)
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
        public ActionResult Hesapla(IsYeriModel model)
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


                    if (model.IsYeriTeminatBilgileri != null)
                    {
                        //------------- SOL --------------//

                        //Makina Techizat	E / H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.MakinaTechizat)
                            if (ModelState["IsYeriTeminatBilgileri.MakinaTechizatBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.MakinaTechizatBedel"].Errors.Clear();

                        //3.Şahıs Malları Yangın	E / H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.SahisMallariYangin3)
                            if (ModelState["IsYeriTeminatBilgileri.SahisMallariYangin3Bedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.SahisMallariYangin3Bedel"].Errors.Clear();

                        //Makina Kırılması	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.MakinaKirilmasi)
                            if (ModelState["IsYeriTeminatBilgileri.MakinaKirilmasiBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.MakinaKirilmasiBedel"].Errors.Clear();

                        //Cam Kırılması	E/H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.CamKirilmasi)
                            if (ModelState["IsYeriTeminatBilgileri.CamKirilmasiBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.CamKirilmasiBedel"].Errors.Clear();

                        //Kira Kaybı	E/H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.KiraKaybi)
                            if (ModelState["IsYeriTeminatBilgileri.KiraKaybiBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.KiraKaybiBedel"].Errors.Clear();

                        //İşveren Mali Mesuliyet Kaza Başına Bedeni	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKazaBasinaBedeni)
                            if (ModelState["IsYeriTeminatBilgileri.IsverenMaliMesuliyetKazaBasinaBedeniBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.IsverenMaliMesuliyetKazaBasinaBedeniBedel"].Errors.Clear();

                        //3.Sahıs Mali Sorumluluk Kişi Başına Bedeni	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKisiBasinaBedeni3)
                            if (ModelState["IsYeriTeminatBilgileri.SahisMaliSorumlulukKisiBasinaBedeni3Bedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.SahisMaliSorumlulukKisiBasinaBedeni3Bedel"].Errors.Clear();

                        //3.Sahıs Mali Sorumluluk Kaza Başına Maddi	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaMaddi3)
                            if (ModelState["IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaMaddi3Bedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaMaddi3Bedel"].Errors.Clear();

                        //Komşuluk Mali Sorumluluk Yangın Dahıli Su Duman	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukYanginDahiliSuDuman)
                            if (ModelState["IsYeriTeminatBilgileri.KomsulukMaliSorumlulukYanginDahiliSuDumanBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.KomsulukMaliSorumlulukYanginDahiliSuDumanBedel"].Errors.Clear();

                        //Kiracı Mali Sorumluluk Yangın Dahıli Su Duman	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukYanginDahiliSuDuman)
                            if (ModelState["IsYeriTeminatBilgileri.KiraciMaliSorumlulukYanginDahiliSuDumanBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.KiraciMaliSorumlulukYanginDahiliSuDumanBedel"].Errors.Clear();

                        //------------- SAĞ --------------//

                        //Kasa Muhteviyat Yangın	E / H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.KasaMuhteviyatYangin)
                            if (ModelState["IsYeriTeminatBilgileri.KasaMuhteviyatYanginBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.KasaMuhteviyatYanginBedel"].Errors.Clear();

                        //Temeller Yangın	E / H - Default H	Bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.TemellerYangin)
                            if (ModelState["IsYeriTeminatBilgileri.TemellerYanginBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.TemellerYanginBedel"].Errors.Clear();

                        //Kasa Hırsızlık	E/H - default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.KasaHirsizlik)
                            if (ModelState["IsYeriTeminatBilgileri.KasaHirsizlikBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.KasaHirsizlikBedel"].Errors.Clear();

                        //Elektronik Cihaz	E/H - default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.ElektronikCihaz)
                            if (ModelState["IsYeriTeminatBilgileri.ElektronikCihazBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.ElektronikCihazBedel"].Errors.Clear();

                        //Yaz Durması	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.YazDurmasi)
                            if (ModelState["IsYeriTeminatBilgileri.YazDurmasiBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.YazDurmasiBedel"].Errors.Clear();

                        //İşveren Mali Mesuliyet Kişi Başına Bedeni	E/H - default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKisiBasinaBedeni)
                            if (ModelState["IsYeriTeminatBilgileri.IsverenMaliMesuliyetKisiBasinaBedeniBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.IsverenMaliMesuliyetKisiBasinaBedeniBedel"].Errors.Clear();

                        //3.Sahıs Mali Sorumluluk Kaza Başına Bedeni	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaBedeni3)
                            if (ModelState["IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaBedeni3Bedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaBedeni3Bedel"].Errors.Clear();

                        //Komşuluk Mali Sorumluluk Terör	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukTeror)
                            if (ModelState["IsYeriTeminatBilgileri.KomsulukMaliSorumlulukTerorBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.KomsulukMaliSorumlulukTerorBedel"].Errors.Clear();

                        //Kiracı Mali Sorumluluk Terör	E/H - Default H	bedel girilecek
                        if (!model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukTeror)
                            if (ModelState["IsYeriTeminatBilgileri.KiraciMaliSorumlulukTerorBedel"] != null)
                                ModelState["IsYeriTeminatBilgileri.KiraciMaliSorumlulukTerorBedel"].Errors.Clear();
                    }
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
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.IsYeri, model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);


                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region İş Yerine İlişkin Bilgiler

                    #region Riziko Genel Bilgiler

                    teklif.AddSoru(IsYeriSorular.Yururlukte_dask_policesi_VarYok, model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi);
                    if (model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi == true)
                    {
                        teklif.AddSoru(IsYeriSorular.Dask_Police_Sigorta_Sirketi, model.RizikoGenelBilgiler.SigortaSirketi);

                        if (model.RizikoGenelBilgiler.PoliceninVadeTarihi.HasValue)
                            teklif.AddSoru(IsYeriSorular.Dask_Police_Vade_Tarihi, model.RizikoGenelBilgiler.PoliceninVadeTarihi.Value);

                        if (!String.IsNullOrEmpty(model.RizikoGenelBilgiler.PoliceNumarasi))
                            teklif.AddSoru(IsYeriSorular.Dask_Police_Numarasi, model.RizikoGenelBilgiler.PoliceNumarasi);

                        teklif.AddSoru(IsYeriSorular.Dask_Sigorta_Bedeli, model.RizikoGenelBilgiler.DaskSigortaBedeli.ToString());
                    }


                    teklif.AddSoru(IsYeriSorular.RA_Dain_i_Muhtehin_VarYok, model.RizikoGenelBilgiler.RehinliAlacakliDainMurtehinVarmi);
                    if (model.RizikoGenelBilgiler.RehinliAlacakliDainMurtehinVarmi == true)
                    {
                        teklif.AddSoru(IsYeriSorular.RA_Tipi_Banka_Finansal_Kurum, model.RizikoGenelBilgiler.Tipi);
                        teklif.AddSoru(IsYeriSorular.RA_Kurum_Banka, model.RizikoGenelBilgiler.KurumBanka.ToString());
                        teklif.AddSoru(IsYeriSorular.RA_Sube, model.RizikoGenelBilgiler.Sube.ToString());
                        teklif.AddSoru(IsYeriSorular.RA_Doviz_Kodu, model.RizikoGenelBilgiler.DovizKodu.ToString());

                        if (model.RizikoGenelBilgiler.KrediTutari.HasValue)
                            teklif.AddSoru(IsYeriSorular.RA_Kredi_Tutari, model.RizikoGenelBilgiler.KrediTutari.Value.ToString());
                        teklif.AddSoru(IsYeriSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, model.RizikoGenelBilgiler.KrediReferansNo_HesapSozlesmeNo);
                        if (model.RizikoGenelBilgiler.KrediBitisTarihi.HasValue)
                            teklif.AddSoru(IsYeriSorular.RA_Kredi_Bitis_Tarihi, model.RizikoGenelBilgiler.KrediBitisTarihi.Value);
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

                    //IstigalKonusuKodu
                    if (model.RizikoDigerBilgiler.IstigalKonusuKodu.HasValue)
                        teklif.AddSoru(IsYeriSorular.IstigalKonusu, model.RizikoDigerBilgiler.IstigalKonusuKodu.Value.ToString());

                    //BelediyeKodu
                    if (model.RizikoDigerBilgiler.BelediyeKodu.HasValue)
                        teklif.AddSoru(IsYeriSorular.Belediye_Kodu, model.RizikoDigerBilgiler.BelediyeKodu.Value.ToString());

                    //EnflasyonOrani
                    if (model.RizikoDigerBilgiler.EnflasyonOrani.HasValue)
                        teklif.AddSoru(IsYeriSorular.EnflasyonOrani, model.RizikoDigerBilgiler.EnflasyonOrani.Value.ToString());

                    //Cati Tipi
                    if ((model.RizikoDigerBilgiler.CatiTipi).HasValue)
                        teklif.AddSoru(IsYeriSorular.CatiTipi, model.RizikoDigerBilgiler.CatiTipi.Value.ToString());

                    //KatTipi
                    if (model.RizikoDigerBilgiler.KatTipi.HasValue)
                        teklif.AddSoru(IsYeriSorular.KatTipi, model.RizikoDigerBilgiler.KatTipi.Value.ToString());

                    //BosKalmaSuresi
                    if (model.RizikoDigerBilgiler.BosKalmaSuresi.HasValue)
                        teklif.AddSoru(IsYeriSorular.BosKalmaSuresi, model.RizikoDigerBilgiler.BosKalmaSuresi.Value.ToString());

                    //DaireBrutYuzOlcumu
                    if (model.RizikoDigerBilgiler.DaireBrutYuzOlcumu.HasValue)
                        teklif.AddSoru(IsYeriSorular.Daire_Brut_Yuzolcumu_M2, model.RizikoDigerBilgiler.DaireBrutYuzOlcumu.Value.ToString());

                    //AsansorSayisi
                    if (!String.IsNullOrEmpty(model.RizikoDigerBilgiler.AsansorSayisi))
                        teklif.AddSoru(IsYeriSorular.AsansorSayisi, model.RizikoDigerBilgiler.AsansorSayisi);

                    //IsYerindeCalisanSayisi
                    if (model.RizikoDigerBilgiler.IsYerindeCalisanSayisi.HasValue)
                        teklif.AddSoru(IsYeriSorular.IsYerindeCalisanKisiSayisi, model.RizikoDigerBilgiler.IsYerindeCalisanSayisi.Value.ToString());

                    //Yapi Tarzi
                    if (model.RizikoDigerBilgiler.YapiTarzi.HasValue)
                        teklif.AddSoru(IsYeriSorular.Yapi_Tarzi, model.RizikoDigerBilgiler.YapiTarzi.Value.ToString());

                    //Bekci Varmı?
                    teklif.AddSoru(IsYeriSorular.Bekci, model.RizikoDigerBilgiler.BekciVarMi);
                    //Alarm
                    teklif.AddSoru(IsYeriSorular.OzelGuvenlik_Alarm_VarMi_EH, model.RizikoDigerBilgiler.AlarmVarMi);
                    //KepenkDemirVarMi
                    teklif.AddSoru(IsYeriSorular.KepenkVeVeyaDemir, model.RizikoDigerBilgiler.KepenkDemirVarMi);
                    //PasajIciUstKatMi
                    teklif.AddSoru(IsYeriSorular.PasajIciUstKatlar, model.RizikoDigerBilgiler.PasajIciUstKatMi);
                    //KameraVarMi
                    teklif.AddSoru(IsYeriSorular.Kamera, model.RizikoDigerBilgiler.KameraVarMi);
                    //TemperliCam
                    teklif.AddSoru(IsYeriSorular.TemperliCam, model.RizikoDigerBilgiler.TemperliCam);

                    #endregion

                    #region Teminat Bedeller

                    //Bina Bedeli

                    if (model.IsYeriTeminatBedelBilgileri.BinaBedeli.HasValue)
                        teklif.AddSoru(IsYeriSorular.BinaBedeli, model.IsYeriTeminatBedelBilgileri.BinaBedeli.Value.ToString());

                    //Dekorasyon Bedeli
                    if (model.IsYeriTeminatBedelBilgileri.DekorasyonBedeli.HasValue)
                        teklif.AddSoru(IsYeriSorular.DekorasyonBedeli, model.IsYeriTeminatBedelBilgileri.DekorasyonBedeli.Value.ToString());

                    //DemirbasBedeli
                    if (model.IsYeriTeminatBedelBilgileri.DemirbasBedeli.HasValue)
                        teklif.AddSoru(IsYeriSorular.DemirbasBedeli, model.IsYeriTeminatBedelBilgileri.DemirbasBedeli.Value.ToString());

                    //EmteaBedeli
                    if (model.IsYeriTeminatBedelBilgileri.EmteaBedeli.HasValue)
                        teklif.AddSoru(IsYeriSorular.EmteaBedeli, model.IsYeriTeminatBedelBilgileri.EmteaBedeli.Value.ToString());

                    #endregion

                    #region Sorular Ve Teminatlar

                    #region AnaTeminat Ve Sorular

                    int DemirbasBedeli = model.IsYeriTeminatBedelBilgileri.DemirbasBedeli ?? 0;
                    int DekorasyonBedeli = model.IsYeriTeminatBedelBilgileri.DekorasyonBedeli ?? 0;
                    int BinaBedeli = model.IsYeriTeminatBedelBilgileri.BinaBedeli ?? 0;
                    int EmteaBedeli = model.IsYeriTeminatBedelBilgileri.EmteaBedeli ?? 0;

                    //--------------------SOL -----------------//

                    //Demirbaş  Yangın	E  150.000 ₺
                    teklif.AddSoru(IsYeriSorular.DemirbasYangin, true);
                    if (DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DemirbasYangin, DemirbasBedeli, 0, 0, 0, 0);

                    //Demirbaş  Deprem	E / H - Default H	150.000 ₺
                    teklif.AddSoru(IsYeriSorular.DemirbasDeprem, model.IsYeriTeminatBilgileri.DemirbasDeprem);
                    if (model.IsYeriTeminatBilgileri.DemirbasDeprem && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DemirbasDeprem, DemirbasBedeli, 0, 0, 0, 0);

                    //Dekorasyon Yangın	E / H - Default H	10.000 ₺
                    teklif.AddSoru(IsYeriSorular.DekorasyonYangin, model.IsYeriTeminatBilgileri.DekorasyonYangin);
                    if (model.IsYeriTeminatBilgileri.DekorasyonYangin && DekorasyonBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DekorasyonYangin, DekorasyonBedeli, 0, 0, 0, 0);

                    //Dekorasyon Deprem	E / H - Default H	10.000 ₺
                    teklif.AddSoru(IsYeriSorular.DekorasyonDeprem, model.IsYeriTeminatBilgileri.DekorasyonDeprem);
                    if (model.IsYeriTeminatBilgileri.DekorasyonDeprem && DekorasyonBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DekorasyonDeprem, DekorasyonBedeli, 0, 0, 0, 0);

                    //Makina Techizat	E / H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.MakinaTechizat, model.IsYeriTeminatBilgileri.MakinaTechizat);
                    if (model.IsYeriTeminatBilgileri.MakinaTechizat && model.IsYeriTeminatBilgileri.MakinaTechizatBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.MakinaVeTechizat, model.IsYeriTeminatBilgileri.MakinaTechizatBedel.Value, 0, 0, 0, 0);

                    //3.Şahıs Malları Yangın	E / H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.SahisMallariYangin3, model.IsYeriTeminatBilgileri.SahisMallariYangin3);
                    if (model.IsYeriTeminatBilgileri.SahisMallariYangin3 && model.IsYeriTeminatBilgileri.SahisMallariYangin3Bedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.SahisMallariYangin, model.IsYeriTeminatBilgileri.SahisMallariYangin3Bedel.Value, 0, 0, 0, 0);


                    //--------------------SAĞ-----------------//

                    //Bina Yangın	E / H - Default H	350.000 ₺
                    teklif.AddSoru(IsYeriSorular.BinaYangin, model.IsYeriTeminatBilgileri.BinaYangin);
                    if (model.IsYeriTeminatBilgileri.BinaYangin && BinaBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.BinaYangin, BinaBedeli, 0, 0, 0, 0);

                    //Bina Deprem	E / H - Default H	250.000 ₺
                    teklif.AddSoru(IsYeriSorular.BinaDeprem, model.IsYeriTeminatBilgileri.BinaDeprem);
                    if (model.IsYeriTeminatBilgileri.BinaDeprem && (BinaBedeli > 0))
                    {
                        if (model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi && model.RizikoGenelBilgiler.DaskSigortaBedeli.HasValue)
                            teklif.AddTeminat(IsYeriTeminatlar.BinaDeprem, (BinaBedeli - model.RizikoGenelBilgiler.DaskSigortaBedeli.Value), 0, 0, 0, 0);
                        else
                            teklif.AddTeminat(IsYeriTeminatlar.BinaDeprem, BinaBedeli, 0, 0, 0, 0);
                    }


                    //Emtea Yangın	E / H - Default H	100.000 ₺
                    teklif.AddSoru(IsYeriSorular.EmteaYangin, model.IsYeriTeminatBilgileri.EmteaYangin);
                    if (model.IsYeriTeminatBilgileri.EmteaYangin && EmteaBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.EmteaYangin, EmteaBedeli, 0, 0, 0, 0);

                    //Emtea Deprem	E / H - Default H	100.000 ₺
                    teklif.AddSoru(IsYeriSorular.EmteaDeprem, model.IsYeriTeminatBilgileri.EmteaDeprem);
                    if (model.IsYeriTeminatBilgileri.EmteaDeprem && EmteaBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.EmteaDeprem, EmteaBedeli, 0, 0, 0, 0);

                    //Kasa Muhteviyat Yangın
                    teklif.AddSoru(IsYeriSorular.KasaMuhteviyatYangin, model.IsYeriTeminatBilgileri.KasaMuhteviyatYangin);
                    if (model.IsYeriTeminatBilgileri.KasaMuhteviyatYangin && model.IsYeriTeminatBilgileri.KasaMuhteviyatYanginBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.KasaMuhteviyatYangin, model.IsYeriTeminatBilgileri.KasaMuhteviyatYanginBedel.Value, 0, 0, 0, 0);

                    //Temeller Yangın
                    teklif.AddSoru(IsYeriSorular.TemellerYangin, model.IsYeriTeminatBilgileri.TemellerYangin);
                    if (model.IsYeriTeminatBilgileri.TemellerYangin && model.IsYeriTeminatBilgileri.TemellerYanginBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.TemellerYangin, model.IsYeriTeminatBilgileri.TemellerYanginBedel.Value, 0, 0, 0, 0);

                    #endregion

                    #region EkTeminat Ve Sorular

                    //--------------------SOL -----------------//

                    //Ek Teminat Muhteviyat	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.EkTeminatMuhteviyat, model.IsYeriTeminatBilgileri.EkTeminatMuhteviyat);
                    if (model.IsYeriTeminatBilgileri.EkTeminatMuhteviyat && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.EkTeminatMuhteviyat, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Hırsızlık	E/H	250.000 ₺
                    teklif.AddSoru(IsYeriSorular.Hirsizlik, model.IsYeriTeminatBilgileri.Hirsizlik);
                    if (model.IsYeriTeminatBilgileri.Hirsizlik && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.Hirsizlik, (DemirbasBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Makina Kırılması	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.MakinaKirilmasi, model.IsYeriTeminatBilgileri.MakinaKirilmasi);
                    if (model.IsYeriTeminatBilgileri.MakinaKirilmasi && model.IsYeriTeminatBilgileri.MakinaKirilmasiBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.MakinaKirilmasi, model.IsYeriTeminatBilgileri.MakinaKirilmasiBedel.Value, 0, 0, 0, 0);

                    //Fırtına	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.Firtina, model.IsYeriTeminatBilgileri.Firtina);
                    if (model.IsYeriTeminatBilgileri.Firtina && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.Firtina, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Deprem Yanardağ Püskürmesi Muhteviyat	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.DepremYanardagPuskurmesiMuhteviyat, model.IsYeriTeminatBilgileri.DepremYanardagPuskurmesiMuhteviyat);
                    if (model.IsYeriTeminatBilgileri.DepremYanardagPuskurmesiMuhteviyat && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DepremYanardagPuskurmesiMuhteviyat, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Sel ve Su baskını	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.SelVeSuBaskini, model.IsYeriTeminatBilgileri.SelVeSuBaskini);
                    if (model.IsYeriTeminatBilgileri.SelVeSuBaskini && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.SelVeSuBaskini, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Cam Kırılması	E/H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.CamKirilmasi, model.IsYeriTeminatBilgileri.CamKirilmasi);
                    if (model.IsYeriTeminatBilgileri.CamKirilmasi && model.IsYeriTeminatBilgileri.CamKirilmasiBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.CamKirilmasi, model.IsYeriTeminatBilgileri.CamKirilmasiBedel.Value, 0, 0, 0, 0);

                    //Dahili Su	E/H	610.000 ₺
                    teklif.AddSoru(IsYeriSorular.DahiliSu, model.IsYeriTeminatBilgileri.DahiliSu);
                    if (model.IsYeriTeminatBilgileri.DahiliSu && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DahiliSu, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli + BinaBedeli), 0, 0, 0, 0);

                    //Kara Taşıtları Çarpması	E/H	610.000 ₺
                    teklif.AddSoru(IsYeriSorular.KaraTasitlariCarpmasi, model.IsYeriTeminatBilgileri.KaraTasitlariCarpmasi);
                    if (model.IsYeriTeminatBilgileri.KaraTasitlariCarpmasi && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.KaraTasitlariCarpmasi, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli + BinaBedeli), 0, 0, 0, 0);

                    //Kar Ağırlığı	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.KarAgirligi, model.IsYeriTeminatBilgileri.KarAgirligi);
                    if (model.IsYeriTeminatBilgileri.KarAgirligi && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.KarAgirligi, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Ferdi Kaza	E/H
                    teklif.AddSoru(IsYeriSorular.FerdiKaza, model.IsYeriTeminatBilgileri.FerdiKaza);

                    //Ferdi Kaza  Ölüm	E/H
                    teklif.AddSoru(IsYeriSorular.FerdiKazaOlum, model.IsYeriTeminatBilgileri.FerdiKazaOlum);

                    //Mali Sorumluluk Yangin	E/H
                    teklif.AddSoru(IsYeriSorular.MaliSorumlulukYangin, model.IsYeriTeminatBilgileri.MaliSorumlulukYangin);

                    //Enkaz Kaldırma 	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.EnkazKaldirma, model.IsYeriTeminatBilgileri.EnkazKaldirma);
                    if (model.IsYeriTeminatBilgileri.EnkazKaldirma && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.EnkazKaldirma, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Kira Kaybı	E/H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.KiraKaybi, model.IsYeriTeminatBilgileri.KiraKaybi);
                    if (model.IsYeriTeminatBilgileri.KiraKaybi && model.IsYeriTeminatBilgileri.KiraKaybiBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.KiraKaybi, model.IsYeriTeminatBilgileri.KiraKaybiBedel.Value, 0, 0, 0, 0);

                    //İşveren Mali Mesuliyet Kaza Başına Bedeni	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.IsverenMaliMesuliyetKazaBasinaBedeni, model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKazaBasinaBedeni);
                    if (model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKazaBasinaBedeni &&
                        model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKazaBasinaBedeniBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.IsVerenMaliMesuliyetKazaBasinaBedeni,
                                                                 model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKazaBasinaBedeniBedel.Value, 0, 0, 0, 0);

                    //3.Sahıs Mali Sorumluluk Kişi Başına Bedeni	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.SahisMaliSorumlulukKisiBasinaBedeni3, model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKisiBasinaBedeni3);
                    if (model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKisiBasinaBedeni3 &&
                        model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKisiBasinaBedeni3Bedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.SahisMaliSorumlulukKisiBasinaBedeni3,
                                                                 model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKisiBasinaBedeni3Bedel.Value, 0, 0, 0, 0);

                    //3.Sahıs Mali Sorumluluk Kaza Başına Maddi	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.SahisMaliSorumlulukKazaBasinaMaddi3, model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaMaddi3);
                    if (model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaMaddi3 &&
                        model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaMaddi3Bedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaMaddi3,
                                                                 model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaMaddi3Bedel.Value, 0, 0, 0, 0);

                    //Komşuluk Mali Sorumluluk Yangın Dahıli Su Duman	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.KomsulukMaliSorumlulukYanginDahiliSuDuman, model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukYanginDahiliSuDuman);
                    if (model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukYanginDahiliSuDuman &&
                        model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukYanginDahiliSuDumanBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.KomsulukMaliSorumlulukYanginDahiliSuDuman,
                                                                 model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukYanginDahiliSuDumanBedel.Value, 0, 0, 0, 0);

                    //Kiracı Mali Sorumluluk Yangın Dahıli Su Duman	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.KiraciMaliSorumlulukYanginDahiliSuDuman, model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukYanginDahiliSuDuman);
                    if (model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukYanginDahiliSuDuman &&
                        model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukYanginDahiliSuDumanBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.KiraciMaliSorumlulukYanginDahiliSuDuman,
                                                                 model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukYanginDahiliSuDumanBedel.Value, 0, 0, 0, 0);



                    //-------------------- SAĞ -----------------//

                    //Ek Teminat Bina	E/H	350.000 ₺
                    teklif.AddSoru(IsYeriSorular.EkTeminatBina, model.IsYeriTeminatBilgileri.EkTeminatBina);
                    if (model.IsYeriTeminatBilgileri.EkTeminatBina && BinaBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.EkTeminatBina, BinaBedeli, 0, 0, 0, 0);

                    //Kasa Hırsızlık	E/H - default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.KasaHirsizlik, model.IsYeriTeminatBilgileri.KasaHirsizlik);
                    if (model.IsYeriTeminatBilgileri.KasaHirsizlik && model.IsYeriTeminatBilgileri.KasaHirsizlikBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.KasaHirsizlik, model.IsYeriTeminatBilgileri.KasaHirsizlikBedel.Value, 0, 0, 0, 0);

                    //Elektronik Cihaz	E/H - default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.ElektronikCihaz, model.IsYeriTeminatBilgileri.ElektronikCihaz);
                    if (model.IsYeriTeminatBilgileri.ElektronikCihaz && model.IsYeriTeminatBilgileri.ElektronikCihazBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.ElektronikCihazSigortasi, model.IsYeriTeminatBilgileri.ElektronikCihazBedel.Value, 0, 0, 0, 0);

                    //Deprem Yanardağ Püskürmesi 	E/H	610.000 ₺
                    teklif.AddSoru(IsYeriSorular.DepremYanardagPuskurmesi, model.IsYeriTeminatBilgileri.DepremYanardagPuskurmesi);
                    if (model.IsYeriTeminatBilgileri.DepremYanardagPuskurmesi && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DepremYanardagPuskurmesi, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli + BinaBedeli), 0, 0, 0, 0);

                    //Deprem Yanardağ Püskürmesi Bina	E/H	350.000 ₺
                    teklif.AddSoru(IsYeriSorular.DepremYanardagPuskurmesiBina, model.IsYeriTeminatBilgileri.DepremYanardagPuskurmesiBina);
                    if (model.IsYeriTeminatBilgileri.DepremYanardagPuskurmesiBina && BinaBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.DepremYanardagPuskurmesiBina, BinaBedeli, 0, 0, 0, 0);

                    //GLKHHKNH – Terör	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.GLKHHKNHTeror, model.IsYeriTeminatBilgileri.GLKHHKNHTeror);
                    if (model.IsYeriTeminatBilgileri.GLKHHKNHTeror && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.GLKHHKNHTeror, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //Asistan Hizmeti	E/H
                    teklif.AddSoru(IsYeriSorular.AsistanHizmeti, model.IsYeriTeminatBilgileri.AsistanHizmeti);

                    //Hukuksal Koruma	E/H
                    teklif.AddSoru(IsYeriSorular.HukuksalKoruma, model.IsYeriTeminatBilgileri.HukuksalKoruma);

                    //Duman	E/H	610.000 ₺
                    teklif.AddSoru(IsYeriSorular.Duman, model.IsYeriTeminatBilgileri.Duman);
                    if (model.IsYeriTeminatBilgileri.Duman && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.Duman, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli + BinaBedeli), 0, 0, 0, 0);

                    //Hava Taşıtları Çarpması	E/H	610.000 ₺
                    teklif.AddSoru(IsYeriSorular.HavaTasitlariCarpmasi, model.IsYeriTeminatBilgileri.HavaTasitlariCarpmasi);
                    if (model.IsYeriTeminatBilgileri.HavaTasitlariCarpmasi && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.HavaTasitlariCarpmasi, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli + BinaBedeli), 0, 0, 0, 0);

                    //Yaz Durması	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.YazDurmasi, model.IsYeriTeminatBilgileri.YazDurmasi);
                    if (model.IsYeriTeminatBilgileri.YazDurmasi && model.IsYeriTeminatBilgileri.YazDurmasiBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.YazDurmasi, model.IsYeriTeminatBilgileri.YazDurmasiBedel.Value, 0, 0, 0, 0);

                    //Ferdi Kaza  Sürekli Sakatlık	E/H
                    teklif.AddSoru(IsYeriSorular.FerdiKazaSurekliSakatlik, model.IsYeriTeminatBilgileri.FerdiKazaSurekliSakatlik);

                    //Mali Sorumluluk Ek Teminat	E/H
                    teklif.AddSoru(IsYeriSorular.MaliSorumlulukEkTeminat, model.IsYeriTeminatBilgileri.MaliSorumlulukEkTeminat);

                    //Enkaz Kaldırma Bina	E/H	350.000 ₺
                    teklif.AddSoru(IsYeriSorular.EnkazKaldirmaBina, model.IsYeriTeminatBilgileri.EnkazKaldirmaBina);
                    if (model.IsYeriTeminatBilgileri.EnkazKaldirmaBina && BinaBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.EnkazKaldirmaBina, BinaBedeli, 0, 0, 0, 0);

                    //Yer Kayması	E/H	260.000 ₺
                    teklif.AddSoru(IsYeriSorular.YerKaymasi, model.IsYeriTeminatBilgileri.YerKaymasi);
                    if (model.IsYeriTeminatBilgileri.YerKaymasi && DemirbasBedeli > 0)
                        teklif.AddTeminat(IsYeriTeminatlar.YerKaymasi, (DemirbasBedeli + DekorasyonBedeli + EmteaBedeli), 0, 0, 0, 0);

                    //İşveren Mali Mesuliyet Kişi Başına Bedeni	E/H - default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.IsverenMaliMesuliyetKisiBasinaBedeni, model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKisiBasinaBedeni);
                    if (model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKisiBasinaBedeni &&
                        model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKisiBasinaBedeniBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.IsVerenMaliMEsuliyetKisiBasinaBedeni,
                                     model.IsYeriTeminatBilgileri.IsverenMaliMesuliyetKisiBasinaBedeniBedel.Value, 0, 0, 0, 0);

                    //3.Sahıs Mali Sorumluluk Kaza Başına Bedeni	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.SahisMaliSorumlulukKazaBasinaBedeni3, model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaBedeni3);
                    if (model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaBedeni3 &&
                        model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaBedeni3Bedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaBedeni3,
                                     model.IsYeriTeminatBilgileri.SahisMaliSorumlulukKazaBasinaBedeni3Bedel.Value, 0, 0, 0, 0);

                    //Komşuluk Mali Sorumluluk Terör	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.KomsulukMaliSorumlulukTeror, model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukTeror);
                    if (model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukTeror && model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukTerorBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.KomsulukMaliSorumlulukTeror,
                                                                model.IsYeriTeminatBilgileri.KomsulukMaliSorumlulukTerorBedel.Value, 0, 0, 0, 0);

                    //Kiracı Mali Sorumluluk Terör	E/H - Default H	bedel girilecek
                    teklif.AddSoru(IsYeriSorular.KiraciMaliSorumlulukTeror, model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukTeror);
                    if (model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukTeror && model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukTerorBedel.HasValue)
                        teklif.AddTeminat(IsYeriTeminatlar.KiraciMaliSorumlulukTeror, model.IsYeriTeminatBilgileri.KiraciMaliSorumlulukTerorBedel.Value, 0, 0, 0, 0);


                    #endregion

                    #endregion

                    #region Not

                    if (!String.IsNullOrEmpty(model.IsYeriNotModel.Not))
                    {
                        teklif.GenelBilgiler.TeklifNot = new TeklifNot();
                        teklif.GenelBilgiler.TeklifNot.Aciklama = model.IsYeriNotModel.Not;
                    }

                    #endregion  

                    #endregion

                    #region Teklif return

                    IIsYeriTeklif isYeriTeklif = new IsYeriTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    foreach (var item in model.TeklifUM)
                    {
                        if (item.TeklifAl)
                            isYeriTeklif.AddUretimMerkezi(item.TUMKodu);
                    }

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.Odeme.OdemeTipi;

                    if (!model.Odeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                        isYeriTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        isYeriTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = isYeriTeklif.Hesapla(teklif);

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

        private IsYeriPoliceOdemeModel IsYeriPoliceOdemeModel(ITeklif teklif)
        {
            IsYeriPoliceOdemeModel model = new IsYeriPoliceOdemeModel();

            if (teklif != null && teklif.GenelBilgiler != null)
            {

                model.BrutPrim = teklif.GenelBilgiler.BrutPrim ?? 0;
                model.NetPrim = teklif.GenelBilgiler.NetPrim ?? 0;
                model.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi ?? 1;
                model.Vergi = teklif.GenelBilgiler.ToplamVergi ?? 0;

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

        private IsYeriRizikoGenelBilgiler IsYeriRizikoGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            IsYeriRizikoGenelBilgiler model = new IsYeriRizikoGenelBilgiler();

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
                model.YururlukteDaskPolicesiVarmi = teklif.ReadSoru(IsYeriSorular.Yururlukte_dask_policesi_VarYok, false); ;
                model.RehinliAlacakliDainMurtehinVarmi = teklif.ReadSoru(IsYeriSorular.RA_Dain_i_Muhtehin_VarYok, false);

                if (model.YururlukteDaskPolicesiVarmi)
                {
                    model.SigortaSirketi = teklif.ReadSoru(IsYeriSorular.Dask_Police_Sigorta_Sirketi, "0");
                    model.PoliceninVadeTarihi = teklif.ReadSoru(IsYeriSorular.Dask_Police_Vade_Tarihi, DateTime.MinValue);
                    model.PoliceNumarasi = teklif.ReadSoru(IsYeriSorular.Dask_Police_Numarasi, String.Empty);
                    model.DaskSigortaBedeli = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.Dask_Sigorta_Bedeli, "0"));
                }

                if (model.RehinliAlacakliDainMurtehinVarmi)
                {
                    model.Tipi = teklif.ReadSoru(IsYeriSorular.RA_Tipi_Banka_Finansal_Kurum, "0");
                    model.KurumBanka = (int)teklif.ReadSoru(IsYeriSorular.RA_Kurum_Banka, 0);
                    model.Subeler = new SelectList(_CRService.GetListDaskSubeler(model.KurumBanka), "SubeKodu", "SubeAdi", "").ListWithOptionLabel();
                    model.Sube = (int)teklif.ReadSoru(IsYeriSorular.RA_Sube, 0);
                    model.KrediReferansNo_HesapSozlesmeNo = teklif.ReadSoru(IsYeriSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, "0");
                    model.KrediBitisTarihi = teklif.ReadSoru(IsYeriSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);

                    string kredi = teklif.ReadSoru(IsYeriSorular.RA_Kredi_Tutari, String.Empty);
                    if (!String.IsNullOrEmpty(kredi))
                        model.KrediTutari = Convert.ToInt32(kredi);

                    model.DovizKodu = (byte)teklif.ReadSoru(IsYeriSorular.RA_Doviz_Kodu, 0);
                }
            }

            return model;
        }

        private IsYeriRizikoAdresModel IsYeriRizikoAdresModel(int? teklifId, ITeklif teklif)
        {
            IsYeriRizikoAdresModel model = new IsYeriRizikoAdresModel();

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

        private IsYeriRizikoDigerBilgiler IsYeriRizikoDigerBilgiler(int? teklifId, ITeklif teklif)
        {
            IsYeriRizikoDigerBilgiler model = new IsYeriRizikoDigerBilgiler();

            model.BosKalmaSureleri = new SelectList(TeklifProvider.BosKalmaSureleri(), "Value", "Text", "").ListWithOptionLabel();
            model.CatiTipleri = new SelectList(TeklifProvider.CatiTipleri(), "Value", "Text", "").ListWithOptionLabel();
            model.KatTipleri = new SelectList(TeklifProvider.KatTipleri(), "Value", "Text", "").ListWithOptionLabel();
            model.IstigalKonusu = new SelectList(_CRService.GetListIstigal(), "Kod", "Aciklama").ListWithOptionLabel();
            model.YapiTarzlari = new SelectList(TeklifProvider.KonutBinaYapiTazrlari(), "Value", "Text", "").ListWithOptionLabel();

            if (teklifId.HasValue & teklif != null)
            {
                string belediyeKod = teklif.ReadSoru(IsYeriSorular.Belediye_Kodu, String.Empty);
                if ((!String.IsNullOrEmpty(belediyeKod)) && teklif.RizikoAdresi.IlKodu.HasValue)
                {
                    model.BelediyeKodu = Convert.ToInt32(belediyeKod);
                    model.belediyeler = new SelectList(_CRService.GetListBelediye(teklif.RizikoAdresi.IlKodu.Value), "BelediyeKodu", "BelediyeAdi", "").
                                                                                                                                  ListWithOptionLabel();
                }

                model.BekciVarMi = teklif.ReadSoru(IsYeriSorular.Bekci, false);
                model.KameraVarMi = teklif.ReadSoru(IsYeriSorular.Kamera, false);
                model.TemperliCam = teklif.ReadSoru(IsYeriSorular.TemperliCam, false);
                model.KepenkDemirVarMi = teklif.ReadSoru(IsYeriSorular.KepenkVeVeyaDemir, false);
                model.PasajIciUstKatMi = teklif.ReadSoru(IsYeriSorular.PasajIciUstKatlar, false);
                model.AlarmVarMi = teklif.ReadSoru(IsYeriSorular.OzelGuvenlik_Alarm_VarMi_EH, false);

                string BosKalmaSuresi = teklif.ReadSoru(IsYeriSorular.BosKalmaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(BosKalmaSuresi) && BosKalmaSuresi != "0")
                    model.BosKalmaSuresi = Convert.ToInt32(BosKalmaSuresi);

                string CatiTipi = teklif.ReadSoru(IsYeriSorular.CatiTipi, String.Empty);
                if (!String.IsNullOrEmpty(CatiTipi) && CatiTipi != "0")
                    model.CatiTipi = Convert.ToInt32(CatiTipi);

                string KatTipi = teklif.ReadSoru(IsYeriSorular.KatTipi, String.Empty);
                if (!String.IsNullOrEmpty(KatTipi) && KatTipi != "0")
                    model.KatTipi = Convert.ToInt32(KatTipi);

                string yapiTarzi = teklif.ReadSoru(IsYeriSorular.Yapi_Tarzi, String.Empty);
                if (!String.IsNullOrEmpty(yapiTarzi) && yapiTarzi != "0")
                    model.YapiTarzi = Convert.ToInt32(yapiTarzi);

                string istigalKodu = teklif.ReadSoru(IsYeriSorular.IstigalKonusu, String.Empty);
                if (!String.IsNullOrEmpty(istigalKodu) && istigalKodu != "0")
                    model.IstigalKonusuKodu = Convert.ToInt32(istigalKodu);

                string daireM2 = teklif.ReadSoru(IsYeriSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                if (!String.IsNullOrEmpty(daireM2) && daireM2 != "0")
                    model.DaireBrutYuzOlcumu = Convert.ToInt32(daireM2);

                string enflasyonO = teklif.ReadSoru(IsYeriSorular.EnflasyonOrani, String.Empty);
                if (!String.IsNullOrEmpty(enflasyonO) & enflasyonO != "0")
                    model.EnflasyonOrani = Convert.ToInt32(enflasyonO);

                string calisansayisi = teklif.ReadSoru(IsYeriSorular.IsYerindeCalisanKisiSayisi, String.Empty);
                if (!String.IsNullOrEmpty(calisansayisi) & calisansayisi != "0")
                    model.IsYerindeCalisanSayisi = Convert.ToInt32(calisansayisi);


                string asansorSayisi = teklif.ReadSoru(IsYeriSorular.AsansorSayisi, String.Empty);
                if (!String.IsNullOrEmpty(asansorSayisi) & asansorSayisi != "0")
                    model.AsansorSayisi = asansorSayisi;
            }
            else
                model.belediyeler = new SelectList(_CRService.GetListBelediye(34), "BelediyeKodu", "BelediyeAdi", "").ListWithOptionLabel();

            return model;
        }

        private IsYeriTeminatBedelBilgileri IsYeriTeminatBedelBilgileri(int? teklifId, ITeklif teklif)
        {
            IsYeriTeminatBedelBilgileri model = new IsYeriTeminatBedelBilgileri();

            if (teklifId.HasValue && teklif != null)
            {
                int binabedel = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.BinaBedeli, "0"));
                if (binabedel > 0)
                    model.BinaBedeli = binabedel;

                int demirbasbedel = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.DemirbasBedeli, "0"));
                if (demirbasbedel > 0)
                    model.DemirbasBedeli = demirbasbedel;

                int dekorasyonbedel = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.DekorasyonBedeli, "0"));
                if (dekorasyonbedel > 0)
                    model.DekorasyonBedeli = dekorasyonbedel;

                int emteabedeli = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.EmteaBedeli, "0"));
                if (emteabedeli > 0)
                    model.EmteaBedeli = emteabedeli;
            }

            return model;
        }

        private IsYeriTeminatBilgileri IsYeriTeminatBilgileri(int? teklifId, ITeklif teklif)
        {
            IsYeriTeminatBilgileri model = new IsYeriTeminatBilgileri();

            if (teklifId.HasValue && teklif != null)
            {
                SetTeminatlar(model, teklif);
            }
            else
            {
                foreach (PropertyInfo prop in model.GetType().GetProperties())
                    if (prop.PropertyType == typeof(bool))
                        prop.SetValue(model, false, null);

                model.DemirbasYangin = true;
            }
            return model;
        }

        #endregion

        #region Detay Method

        private IsYeriRizikoGenelBilgiler IsYeriRizikoGenelBilgilerDetay(ITeklif teklif)
        {
            ISigortaSirketleriService _SigortaSirketiService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
            IsYeriRizikoGenelBilgiler model = new IsYeriRizikoGenelBilgiler();

            try
            {
                model.YururlukteDaskPolicesiVarmi = teklif.ReadSoru(IsYeriSorular.Yururlukte_dask_policesi_VarYok, false); ;
                model.RehinliAlacakliDainMurtehinVarmi = teklif.ReadSoru(IsYeriSorular.RA_Dain_i_Muhtehin_VarYok, false);

                if (model.YururlukteDaskPolicesiVarmi == true)
                {
                    SigortaSirketleri sirket = _SigortaSirketiService.GetSirket(teklif.ReadSoru(IsYeriSorular.Dask_Police_Sigorta_Sirketi, "0"));

                    if (sirket != null)
                        model.SigortaSirketi = sirket.SirketAdi;

                    model.PoliceninVadeTarihi = teklif.ReadSoru(IsYeriSorular.Dask_Police_Vade_Tarihi, DateTime.MinValue);
                    model.PoliceNumarasi = teklif.ReadSoru(IsYeriSorular.Dask_Police_Numarasi, String.Empty);
                    model.DaskSigortaBedeli = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.Dask_Sigorta_Bedeli, "0"));
                }


                if (model.RehinliAlacakliDainMurtehinVarmi == true)
                {
                    model.Tipi = teklif.ReadSoru(IsYeriSorular.RA_Tipi_Banka_Finansal_Kurum, "0") == "1" ? "Banka" : "Finansal Kurum";
                    model.KrediReferansNo_HesapSozlesmeNo = teklif.ReadSoru(IsYeriSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, String.Empty); ;
                    model.KrediBitisTarihi = teklif.ReadSoru(IsYeriSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);
                    model.DovizKoduText = DaskDovizTipleri.DovizTipi(Convert.ToByte(teklif.ReadSoru(IsYeriSorular.RA_Doviz_Kodu, "0")));

                    string kredi = teklif.ReadSoru(IsYeriSorular.RA_Kredi_Tutari, String.Empty);
                    if (!String.IsNullOrEmpty(kredi))
                        model.KrediTutari = Convert.ToInt32(kredi);

                    DaskKurumlar kurum = _CRService.GetDaskKurum(Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.RA_Kurum_Banka, "0")));
                    if (kurum != null)
                    {
                        model.KurumBankaAdi = kurum.KurumAdi;
                        DaskSubeler sube = _CRService.GetDaskSube(kurum.KurumKodu, Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.RA_Sube, "0")));
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

        private IsYeriRizikoAdresModel IsYeriRizikoAdresModelDetay(ITeklif teklif)
        {
            IsYeriRizikoAdresModel model = new IsYeriRizikoAdresModel();
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

        private IsYeriRizikoDigerBilgiler IsYeriRizikoDigerBilgilerDetay(ITeklif teklif)
        {
            IsYeriRizikoDigerBilgiler model = new IsYeriRizikoDigerBilgiler();
            try
            {
                string bosKalmaSuresi = teklif.ReadSoru(IsYeriSorular.BosKalmaSuresi, String.Empty);
                if (!String.IsNullOrEmpty(bosKalmaSuresi))
                    model.BosKalmaSuresiText = BosKalmaSuresi.Sure(Convert.ToByte(bosKalmaSuresi));

                string yuzOlcumu = teklif.ReadSoru(IsYeriSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                if (!String.IsNullOrEmpty(yuzOlcumu))
                    model.DaireBrutYuzOlcumu = Convert.ToInt32(yuzOlcumu);

                string enflasyon = teklif.ReadSoru(IsYeriSorular.EnflasyonOrani, String.Empty);
                if (!String.IsNullOrEmpty(enflasyon))
                    model.EnflasyonOrani = Convert.ToInt32(enflasyon);

                string cati = teklif.ReadSoru(IsYeriSorular.CatiTipi, String.Empty);
                if (!String.IsNullOrEmpty(cati))
                    model.CatiTipiText = CatiTipi.Tipi(Convert.ToByte(cati));

                string yapi = teklif.ReadSoru(IsYeriSorular.Yapi_Tarzi, String.Empty);
                if (!String.IsNullOrEmpty(yapi))
                    model.YapiTarziText = KonutYapiTarzlari.YapiTarzi(Convert.ToByte(yapi));

                string kat = teklif.ReadSoru(IsYeriSorular.KatTipi, String.Empty);
                if (!String.IsNullOrEmpty(kat))
                    model.KatTipiText = KatTipi.Tipi(Convert.ToByte(kat));

                string asansor = teklif.ReadSoru(IsYeriSorular.AsansorSayisi, String.Empty);
                if (!String.IsNullOrEmpty(asansor))
                    model.AsansorSayisi = asansor;

                string IsCalisan = teklif.ReadSoru(IsYeriSorular.IsYerindeCalisanKisiSayisi, String.Empty);
                if (!String.IsNullOrEmpty(IsCalisan))
                    model.IsYerindeCalisanSayisi = Convert.ToInt32(IsCalisan);

                string istigalKonusu = teklif.ReadSoru(IsYeriSorular.IstigalKonusu, String.Empty);
                if (!String.IsNullOrEmpty(istigalKonusu))
                {
                    int kodu = Convert.ToInt32(istigalKonusu);
                    Istigal istigal = _CRService.GetIstigal(kodu);
                    if (istigal != null) model.IstigalKonusuText = istigal.Aciklama;
                }


                string belediyeKod = teklif.ReadSoru(IsYeriSorular.Belediye_Kodu, String.Empty);
                if ((!String.IsNullOrEmpty(belediyeKod)) && teklif.RizikoAdresi.IlKodu.HasValue)
                {
                    Belediye belediye = _CRService.GetBelediye(teklif.RizikoAdresi.IlKodu.Value, Convert.ToInt32(belediyeKod));
                    if (belediye != null)
                        model.BelediyeAdi = belediye.BelediyeAdi;
                }

                model.BekciVarMi = teklif.ReadSoru(IsYeriSorular.Bekci, false);
                model.KameraVarMi = teklif.ReadSoru(IsYeriSorular.Kamera, false);
                model.TemperliCam = teklif.ReadSoru(IsYeriSorular.TemperliCam, false);
                model.KepenkDemirVarMi = teklif.ReadSoru(IsYeriSorular.KepenkVeVeyaDemir, false);
                model.PasajIciUstKatMi = teklif.ReadSoru(IsYeriSorular.PasajIciUstKatlar, false);
                model.AlarmVarMi = teklif.ReadSoru(IsYeriSorular.OzelGuvenlik_Alarm_VarMi_EH, false);
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }

            return model;
        }

        private IsYeriTeminatBedelBilgileri IsYeriTeminatBedelBilgileriDetay(ITeklif teklif)
        {
            IsYeriTeminatBedelBilgileri model = new IsYeriTeminatBedelBilgileri();

            model.DemirbasBedeli = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.DemirbasBedeli, "0"));
            model.BinaBedeli = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.BinaBedeli, "0"));
            model.EmteaBedeli = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.EmteaBedeli, "0"));
            model.DekorasyonBedeli = Convert.ToInt32(teklif.ReadSoru(IsYeriSorular.DekorasyonBedeli, "0"));

            return model;
        }

        private IsYeriTeminatBilgileri IsYeriTeminatBilgileriDetay(ITeklif teklif)
        {
            IsYeriTeminatBilgileri model = new IsYeriTeminatBilgileri();

            SetTeminatlar(model, teklif);

            return model;
        }

        #endregion

        private void SetTeminatlar(IsYeriTeminatBilgileri model, ITeklif teklif)
        {
            //Ana Teminatlar
            model.DemirbasYangin = teklif.ReadSoru(IsYeriSorular.DemirbasYangin, false);
            model.DemirbasDeprem = teklif.ReadSoru(IsYeriSorular.DemirbasDeprem, false);
            model.EmteaYangin = teklif.ReadSoru(IsYeriSorular.EmteaYangin, false);
            model.EmteaDeprem = teklif.ReadSoru(IsYeriSorular.EmteaDeprem, false);
            model.DekorasyonYangin = teklif.ReadSoru(IsYeriSorular.DekorasyonYangin, false);
            model.DekorasyonDeprem = teklif.ReadSoru(IsYeriSorular.DekorasyonDeprem, false);
            model.BinaYangin = teklif.ReadSoru(IsYeriSorular.BinaYangin, false);
            model.BinaDeprem = teklif.ReadSoru(IsYeriSorular.BinaDeprem, false);
            model.MakinaTechizat = teklif.ReadSoru(IsYeriSorular.MakinaTechizat, false);
            model.SahisMallariYangin3 = teklif.ReadSoru(IsYeriSorular.SahisMallariYangin3, false);
            model.KasaMuhteviyatYangin = teklif.ReadSoru(IsYeriSorular.KasaMuhteviyatYangin, false);
            model.TemellerYangin = teklif.ReadSoru(IsYeriSorular.TemellerYangin, false);

            //Ana Teminat Bedelleri TL

            //------SOL---------//

            TeklifTeminat demirbasyanginTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DemirbasYangin).FirstOrDefault();
            if (demirbasyanginTEM != null && demirbasyanginTEM.TeminatBedeli.HasValue)
                model.DemirbasYanginBedel = Convert.ToInt32(demirbasyanginTEM.TeminatBedeli.Value);

            TeklifTeminat demirbasdepremTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DemirbasDeprem).FirstOrDefault();
            if (demirbasdepremTEM != null && demirbasdepremTEM.TeminatBedeli.HasValue)
                model.DemirbasDepremBedel = Convert.ToInt32(demirbasdepremTEM.TeminatBedeli.Value);

            TeklifTeminat dekorasyonyanginTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DekorasyonYangin).FirstOrDefault();
            if (dekorasyonyanginTEM != null && dekorasyonyanginTEM.TeminatBedeli.HasValue)
                model.DekorasyonYanginBedel = Convert.ToInt32(dekorasyonyanginTEM.TeminatBedeli.Value);

            TeklifTeminat dekorasyondepremTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DekorasyonDeprem).FirstOrDefault();
            if (dekorasyondepremTEM != null && dekorasyondepremTEM.TeminatBedeli.HasValue)
                model.DekorasyonDepremBedel = Convert.ToInt32(dekorasyondepremTEM.TeminatBedeli.Value);

            TeklifTeminat makinatechizatTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.MakinaVeTechizat).FirstOrDefault();
            if (makinatechizatTEM != null && makinatechizatTEM.TeminatBedeli.HasValue)
                model.MakinaTechizatBedel = Convert.ToInt32(makinatechizatTEM.TeminatBedeli.Value);

            TeklifTeminat sahisMallariyanginTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMallariYangin).FirstOrDefault();
            if (sahisMallariyanginTEM != null && sahisMallariyanginTEM.TeminatBedeli.HasValue)
                model.SahisMallariYangin3Bedel = Convert.ToInt32(sahisMallariyanginTEM.TeminatBedeli.Value);

            //------SAĞ---------//

            TeklifTeminat binayanginTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.BinaYangin).FirstOrDefault();
            if (binayanginTEM != null && binayanginTEM.TeminatBedeli.HasValue)
                model.BinaYanginBedel = Convert.ToInt32(binayanginTEM.TeminatBedeli.Value);

            TeklifTeminat binadepremTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.BinaDeprem).FirstOrDefault();
            if (binadepremTEM != null && binadepremTEM.TeminatBedeli.HasValue)
                model.BinaDepremBedel = Convert.ToInt32(binadepremTEM.TeminatBedeli.Value);

            TeklifTeminat emteayanginTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EmteaYangin).FirstOrDefault();
            if (emteayanginTEM != null && emteayanginTEM.TeminatBedeli.HasValue)
                model.EmteaYanginBedel = Convert.ToInt32(emteayanginTEM.TeminatBedeli.Value);

            TeklifTeminat emteadepremTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EmteaDeprem).FirstOrDefault();
            if (emteadepremTEM != null && emteadepremTEM.TeminatBedeli.HasValue)
                model.EmteaDepremBedel = Convert.ToInt32(emteadepremTEM.TeminatBedeli.Value);

            TeklifTeminat kasamuhteviyatYanginTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KasaMuhteviyatYangin).FirstOrDefault();
            if (kasamuhteviyatYanginTEM != null && kasamuhteviyatYanginTEM.TeminatBedeli.HasValue)
                model.KasaMuhteviyatYanginBedel = Convert.ToInt32(kasamuhteviyatYanginTEM.TeminatBedeli.Value);

            TeklifTeminat temelleryanginTEM = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.TemellerYangin).FirstOrDefault();
            if (temelleryanginTEM != null && temelleryanginTEM.TeminatBedeli.HasValue)
                model.TemellerYanginBedel = Convert.ToInt32(temelleryanginTEM.TeminatBedeli.Value);

            //Ek Teminatlar

            //SOL

            model.EkTeminatMuhteviyat = teklif.ReadSoru(IsYeriSorular.EkTeminatMuhteviyat, false);
            model.Hirsizlik = teklif.ReadSoru(IsYeriSorular.Hirsizlik, false);
            model.MakinaKirilmasi = teklif.ReadSoru(IsYeriSorular.MakinaKirilmasi, false);
            model.Firtina = teklif.ReadSoru(IsYeriSorular.Firtina, false);
            model.DepremYanardagPuskurmesiMuhteviyat = teklif.ReadSoru(IsYeriSorular.DepremYanardagPuskurmesiMuhteviyat, false);
            model.SelVeSuBaskini = teklif.ReadSoru(IsYeriSorular.SelVeSuBaskini, false);
            model.CamKirilmasi = teklif.ReadSoru(IsYeriSorular.CamKirilmasi, false);
            model.DahiliSu = teklif.ReadSoru(IsYeriSorular.DahiliSu, false);
            model.KaraTasitlariCarpmasi = teklif.ReadSoru(IsYeriSorular.KaraTasitlariCarpmasi, false);
            model.KarAgirligi = teklif.ReadSoru(IsYeriSorular.KarAgirligi, false);
            model.FerdiKaza = teklif.ReadSoru(IsYeriSorular.FerdiKaza, false);
            model.FerdiKazaOlum = teklif.ReadSoru(IsYeriSorular.FerdiKazaOlum, false);
            model.MaliSorumlulukYangin = teklif.ReadSoru(IsYeriSorular.MaliSorumlulukYangin, false);
            model.EnkazKaldirma = teklif.ReadSoru(IsYeriSorular.EnkazKaldirma, false);
            model.KiraKaybi = teklif.ReadSoru(IsYeriSorular.KiraKaybi, false);

            model.IsverenMaliMesuliyetKazaBasinaBedeni = teklif.ReadSoru(IsYeriSorular.IsverenMaliMesuliyetKazaBasinaBedeni, false);
            model.SahisMaliSorumlulukKisiBasinaBedeni3 = teklif.ReadSoru(IsYeriSorular.SahisMaliSorumlulukKisiBasinaBedeni3, false);
            model.SahisMaliSorumlulukKazaBasinaBedeni3 = teklif.ReadSoru(IsYeriSorular.SahisMaliSorumlulukKazaBasinaBedeni3, false);
            model.KomsulukMaliSorumlulukYanginDahiliSuDuman = teklif.ReadSoru(IsYeriSorular.KomsulukMaliSorumlulukYanginDahiliSuDuman, false);
            model.KiraciMaliSorumlulukYanginDahiliSuDuman = teklif.ReadSoru(IsYeriSorular.KiraciMaliSorumlulukYanginDahiliSuDuman, false);

            //SOL Ek Teminat Bedelleri
            TeklifTeminat muhteviyatekteminat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EkTeminatMuhteviyat).FirstOrDefault();
            if (muhteviyatekteminat != null && muhteviyatekteminat.TeminatBedeli.HasValue)
                model.EkTeminatMuhteviyatBedel = Convert.ToInt32(muhteviyatekteminat.TeminatBedeli.Value);

            TeklifTeminat hirsizlik = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.Hirsizlik).FirstOrDefault();
            if (hirsizlik != null && hirsizlik.TeminatBedeli.HasValue)
                model.HirsizlikBedel = Convert.ToInt32(hirsizlik.TeminatBedeli.Value);

            TeklifTeminat makinakirilmasi = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.MakinaKirilmasi).FirstOrDefault();
            if (makinakirilmasi != null && makinakirilmasi.TeminatBedeli.HasValue)
                model.MakinaKirilmasiBedel = Convert.ToInt32(makinakirilmasi.TeminatBedeli.Value);

            TeklifTeminat firtina = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.Firtina).FirstOrDefault();
            if (firtina != null && firtina.TeminatBedeli.HasValue)
                model.FirtinaBedel = Convert.ToInt32(firtina.TeminatBedeli.Value);

            TeklifTeminat depremyanardagPMuhteviyat = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DepremYanardagPuskurmesiMuhteviyat).FirstOrDefault();
            if (depremyanardagPMuhteviyat != null && depremyanardagPMuhteviyat.TeminatBedeli.HasValue)
                model.DepremYanardagPuskurmesiMuhteviyatBedel = Convert.ToInt32(depremyanardagPMuhteviyat.TeminatBedeli.Value);

            TeklifTeminat selsubaskini = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SelVeSuBaskini).FirstOrDefault();
            if (selsubaskini != null && selsubaskini.TeminatBedeli.HasValue)
                model.SelVeSuBaskiniBedel = Convert.ToInt32(selsubaskini.TeminatBedeli.Value);

            TeklifTeminat camkirilmasi = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.CamKirilmasi).FirstOrDefault();
            if (camkirilmasi != null && camkirilmasi.TeminatBedeli.HasValue)
                model.CamKirilmasiBedel = Convert.ToInt32(camkirilmasi.TeminatBedeli.Value);

            TeklifTeminat dahilisu = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DahiliSu).FirstOrDefault();
            if (dahilisu != null && dahilisu.TeminatBedeli.HasValue)
                model.DahiliSuBedel = Convert.ToInt32(dahilisu.TeminatBedeli.Value);

            TeklifTeminat karatasitlaricarp = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KaraTasitlariCarpmasi).FirstOrDefault();
            if (karatasitlaricarp != null && karatasitlaricarp.TeminatBedeli.HasValue)
                model.KaraTasitlariCarpmasiBedel = Convert.ToInt32(karatasitlaricarp.TeminatBedeli.Value);

            TeklifTeminat karagirligi = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KarAgirligi).FirstOrDefault();
            if (karagirligi != null && karagirligi.TeminatBedeli.HasValue)
                model.KarAgirligiBedel = Convert.ToInt32(karagirligi.TeminatBedeli.Value);

            TeklifTeminat enkazkaldirma = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EnkazKaldirma).FirstOrDefault();
            if (enkazkaldirma != null && enkazkaldirma.TeminatBedeli.HasValue)
                model.EnkazKaldirmaBedel = Convert.ToInt32(enkazkaldirma.TeminatBedeli.Value);

            TeklifTeminat kirakaybi = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KiraKaybi).FirstOrDefault();
            if (kirakaybi != null && kirakaybi.TeminatBedeli.HasValue)
                model.KiraKaybiBedel = Convert.ToInt32(kirakaybi.TeminatBedeli.Value);

            TeklifTeminat IsVerenMMKazaBasinaBedeni = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.IsVerenMaliMesuliyetKazaBasinaBedeni).FirstOrDefault();
            if (IsVerenMMKazaBasinaBedeni != null && IsVerenMMKazaBasinaBedeni.TeminatBedeli.HasValue)
                model.IsverenMaliMesuliyetKazaBasinaBedeniBedel = Convert.ToInt32(IsVerenMMKazaBasinaBedeni.TeminatBedeli.Value);

            TeklifTeminat SahisMSKisiBasinaBedeni3 = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKisiBasinaBedeni3).FirstOrDefault();
            if (SahisMSKisiBasinaBedeni3 != null && IsVerenMMKazaBasinaBedeni.TeminatBedeli.HasValue)
                model.SahisMaliSorumlulukKisiBasinaBedeni3Bedel = Convert.ToInt32(SahisMSKisiBasinaBedeni3.TeminatBedeli.Value);

            TeklifTeminat SahisMSKazaBasinaBedeni3 = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaBedeni3).FirstOrDefault();
            if (SahisMSKazaBasinaBedeni3 != null && SahisMSKazaBasinaBedeni3.TeminatBedeli.HasValue)
                model.SahisMaliSorumlulukKazaBasinaBedeni3Bedel = Convert.ToInt32(SahisMSKazaBasinaBedeni3.TeminatBedeli.Value);

            TeklifTeminat komsulukyangin = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KomsulukMaliSorumlulukYanginDahiliSuDuman).FirstOrDefault();
            if (komsulukyangin != null && komsulukyangin.TeminatBedeli.HasValue)
                model.KomsulukMaliSorumlulukYanginDahiliSuDumanBedel = Convert.ToInt32(komsulukyangin.TeminatBedeli.Value);

            TeklifTeminat kiraciyangin = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KiraciMaliSorumlulukYanginDahiliSuDuman).FirstOrDefault();
            if (kiraciyangin != null && kiraciyangin.TeminatBedeli.HasValue)
                model.KiraciMaliSorumlulukYanginDahiliSuDumanBedel = Convert.ToInt32(kiraciyangin.TeminatBedeli.Value);


            //SAĞ
            model.EkTeminatBina = teklif.ReadSoru(IsYeriSorular.EkTeminatBina, false);
            model.KasaHirsizlik = teklif.ReadSoru(IsYeriSorular.KasaHirsizlik, false);
            model.ElektronikCihaz = teklif.ReadSoru(IsYeriSorular.ElektronikCihaz, false);
            model.DepremYanardagPuskurmesi = teklif.ReadSoru(IsYeriSorular.DepremYanardagPuskurmesi, false);
            model.DepremYanardagPuskurmesiBina = teklif.ReadSoru(IsYeriSorular.DepremYanardagPuskurmesiBina, false);
            model.GLKHHKNHTeror = teklif.ReadSoru(IsYeriSorular.GLKHHKNHTeror, false);
            model.AsistanHizmeti = teklif.ReadSoru(IsYeriSorular.AsistanHizmeti, false);
            model.HukuksalKoruma = teklif.ReadSoru(IsYeriSorular.HukuksalKoruma, false);
            model.Duman = teklif.ReadSoru(IsYeriSorular.Duman, false);
            model.HavaTasitlariCarpmasi = teklif.ReadSoru(IsYeriSorular.HavaTasitlariCarpmasi, false);
            model.YazDurmasi = teklif.ReadSoru(IsYeriSorular.YazDurmasi, false);
            model.FerdiKazaSurekliSakatlik = teklif.ReadSoru(IsYeriSorular.FerdiKazaSurekliSakatlik, false);
            model.MaliSorumlulukEkTeminat = teklif.ReadSoru(IsYeriSorular.MaliSorumlulukEkTeminat, false);
            model.EnkazKaldirmaBina = teklif.ReadSoru(IsYeriSorular.EnkazKaldirmaBina, false);
            model.YerKaymasi = teklif.ReadSoru(IsYeriSorular.YerKaymasi, false);

            model.IsverenMaliMesuliyetKisiBasinaBedeni = teklif.ReadSoru(IsYeriSorular.IsverenMaliMesuliyetKisiBasinaBedeni, false);
            model.SahisMaliSorumlulukKazaBasinaBedeni3 = teklif.ReadSoru(IsYeriSorular.SahisMaliSorumlulukKazaBasinaBedeni3, false);
            model.KomsulukMaliSorumlulukTeror = teklif.ReadSoru(IsYeriSorular.KomsulukMaliSorumlulukTeror, false);
            model.KiraciMaliSorumlulukTeror = teklif.ReadSoru(IsYeriSorular.KiraciMaliSorumlulukTeror, false);


            //SAĞ Ek Teminat Bedelleri
            TeklifTeminat ekteminatbina = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EkTeminatBina).FirstOrDefault();
            if (ekteminatbina != null && ekteminatbina.TeminatBedeli.HasValue)
                model.EkTeminatBinaBedel = Convert.ToInt32(ekteminatbina.TeminatBedeli.Value);

            TeklifTeminat kasahirsizlik = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KasaHirsizlik).FirstOrDefault();
            if (kasahirsizlik != null && kasahirsizlik.TeminatBedeli.HasValue)
                model.KasaHirsizlikBedel = Convert.ToInt32(kasahirsizlik.TeminatBedeli.Value);

            TeklifTeminat elektronikcihaz = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.ElektronikCihazSigortasi).FirstOrDefault();
            if (elektronikcihaz != null && elektronikcihaz.TeminatBedeli.HasValue)
                model.ElektronikCihazBedel = Convert.ToInt32(elektronikcihaz.TeminatBedeli.Value);

            TeklifTeminat depremyanardahP = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DepremYanardagPuskurmesi).FirstOrDefault();
            if (depremyanardahP != null && depremyanardahP.TeminatBedeli.HasValue)
                model.DepremYanardagPuskurmesiBedel = Convert.ToInt32(depremyanardahP.TeminatBedeli.Value);

            TeklifTeminat depremyanardahPbina = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.DepremYanardagPuskurmesiBina).FirstOrDefault();
            if (depremyanardahPbina != null && depremyanardahPbina.TeminatBedeli.HasValue)
                model.DepremYanardagPuskurmesiBinaBedel = Convert.ToInt32(depremyanardahPbina.TeminatBedeli.Value);

            TeklifTeminat grev = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.GLKHHKNHTeror).FirstOrDefault();
            if (grev != null && grev.TeminatBedeli.HasValue)
                model.GLKHHKNHTerorBedel = Convert.ToInt32(grev.TeminatBedeli.Value);

            TeklifTeminat duman = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.Duman).FirstOrDefault();
            if (duman != null && duman.TeminatBedeli.HasValue)
                model.DumanBedel = Convert.ToInt32(duman.TeminatBedeli.Value);

            TeklifTeminat havatasitlari = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.HavaTasitlariCarpmasi).FirstOrDefault();
            if (havatasitlari != null && havatasitlari.TeminatBedeli.HasValue)
                model.HavaTasitlariCarpmasiBedel = Convert.ToInt32(havatasitlari.TeminatBedeli.Value);

            TeklifTeminat yazdurmasi = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.YazDurmasi).FirstOrDefault();
            if (yazdurmasi != null && yazdurmasi.TeminatBedeli.HasValue)
                model.YazDurmasiBedel = Convert.ToInt32(yazdurmasi.TeminatBedeli.Value);

            TeklifTeminat enkazkaldirmabina = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.EnkazKaldirmaBina).FirstOrDefault();
            if (enkazkaldirmabina != null && enkazkaldirmabina.TeminatBedeli.HasValue)
                model.EnkazKaldirmaBinaBedel = Convert.ToInt32(enkazkaldirmabina.TeminatBedeli.Value);

            TeklifTeminat yerkaymasi = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.YerKaymasi).FirstOrDefault();
            if (yerkaymasi != null && yerkaymasi.TeminatBedeli.HasValue)
                model.YerKaymasiBedel = Convert.ToInt32(yerkaymasi.TeminatBedeli.Value);

            TeklifTeminat isverenMMKisiBasinaBedeni = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.IsVerenMaliMEsuliyetKisiBasinaBedeni).FirstOrDefault();
            if (isverenMMKisiBasinaBedeni != null && isverenMMKisiBasinaBedeni.TeminatBedeli.HasValue)
                model.IsverenMaliMesuliyetKisiBasinaBedeniBedel = Convert.ToInt32(isverenMMKisiBasinaBedeni.TeminatBedeli.Value);

            TeklifTeminat sahisMSKazaBasinaBedeni = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.SahisMaliSorumlulukKazaBasinaBedeni3).FirstOrDefault();
            if (sahisMSKazaBasinaBedeni != null && sahisMSKazaBasinaBedeni.TeminatBedeli.HasValue)
                model.SahisMaliSorumlulukKazaBasinaBedeni3Bedel = Convert.ToInt32(sahisMSKazaBasinaBedeni.TeminatBedeli.Value);

            TeklifTeminat komsulukmalisorumlulukteror = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KomsulukMaliSorumlulukTeror).FirstOrDefault();
            if (komsulukmalisorumlulukteror != null && komsulukmalisorumlulukteror.TeminatBedeli.HasValue)
                model.KomsulukMaliSorumlulukTerorBedel = Convert.ToInt32(komsulukmalisorumlulukteror.TeminatBedeli.Value);

            TeklifTeminat kiraciteror = teklif.Teminatlar.Where(s => s.TeminatKodu == IsYeriTeminatlar.KiraciMaliSorumlulukTeror).FirstOrDefault();
            if (kiraciteror != null && kiraciteror.TeminatBedeli.HasValue)
                model.KiraciMaliSorumlulukTerorBedel = Convert.ToInt32(kiraciteror.TeminatBedeli.Value);
        }
    }
}