using AutoMapper;
using AutoMapper.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.HDI.DASK;
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{

    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu =9 , UrunKodu = UrunKodlari.DogalAfetSigortasi_Deprem)]
    public class DaskController : TeklifController
    {
        public DaskController(ITVMService tvmService,
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
            DaskModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            DaskModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public DaskModel EkleModel(int? id, int? teklifId)
        {
            DaskModel model = new DaskModel();
            try
            {
                #region Teklif Genel

                model.UrunAdi = "dask";
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
                model.Musteri.CepTelefonuRequired = true;
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

                #region Dask

                model.RizikoGenelBilgiler = DaskRizikoGenelBilgiler(teklifId, teklif);

                model.RizikoAdresBilgiler = DaskRizikoAdresModel(teklifId, teklif);

                model.RizikoDigerBilgiler = DaskRizikoDigerBilgiler(teklifId, teklif);

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.DogalAfetSigortasi_Deprem);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme
                model.Odeme = new DaskTeklifOdemeModel();
                model.KrediKarti = new KrediKartiOdemeModel();
                model.Odeme.OdemeSekli = true;
                model.Odeme.TaksitSayisi = 1;
                model.KrediKarti.TaksitSayisi = 1;
                model.Odeme.OdemeTipi = OdemeTipleri.KrediKarti;
                if (id.HasValue)
                {
                    if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                    {
                        model.Odeme.OdemeSekli = false;
                        model.Odeme.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : (byte)1;
                        model.KrediKarti.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : (byte)1;
                    }
                    model.Odeme.OdemeTipi = teklif.GenelBilgiler.OdemeTipi.HasValue ? teklif.GenelBilgiler.OdemeTipi.Value : (byte)OdemeTipleri.KrediKarti;
                }


                model.Odeme.TaksitSayilari = new List<SelectListItem>();
                model.Odeme.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.Odeme.OdemeTipi).ToList();



                model.KrediKarti.KK_TeklifId = 0;
                model.KrediKarti.Tutar = 0;

                model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
                model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
                model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
                model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

                List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
                odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash,Value="1"},
                new SelectListItem(){Text=babonline.Forward,Value="2"}});
                model.KrediKarti.OdemeSekilleri = odemeSekilleri;

                model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text").ToList();

                model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
                List<SelectListItem> taksitSeceneleri = new List<SelectListItem>();
                taksitSeceneleri.AddRange(
                    new SelectListItem[]{
                new SelectListItem() { Text = "1", Value = "1" },
                 new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() {Selected=true, Text = "3", Value = "3" }});
                model.KrediKarti.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.KrediKarti.TaksitSayisi).ToList();
                model.Odeme.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.Odeme.TaksitSayisi).ToList();
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
            DetayDaskModel model = new DetayDaskModel();

            #region Teklif Genel


            DaskTeklif daskTeklif = new DaskTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = daskTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(daskTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(daskTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (daskTeklif.Teklif.Sigortalilar.Count > 0 &&
               (daskTeklif.Teklif.SigortaEttiren.MusteriKodu != daskTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(daskTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Riziko

            model.RizikoGenelBilgiler = DaskRizikoGenelBilgilerDetay(daskTeklif.Teklif);
            model.RizikoAdresBilgiler = DaskRizikoAdresModelDetay(daskTeklif.Teklif);
            model.RizikoDigerBilgiler = DaskRizikoDigerBilgilerDetay(daskTeklif.Teklif);

            #endregion

            #region Teklif Fiyat

            model.Fiyat = DaskFiyat(daskTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();


            model.KrediKarti.KK_OdemeSekli = daskTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            model.KrediKarti.KK_OdemeTipi = daskTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = daskTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            List<SelectListItem> taksitSeceneleri = new List<SelectListItem>();
            taksitSeceneleri.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "1", Value = "1" },
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" }});
            model.KrediKarti.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.KrediKarti.TaksitSayisi).ToList();
            #endregion

            return View(model);
        }

        public ActionResult Police(int id)
        {
            DetayDaskModel model = new DetayDaskModel();

            #region Teklif Genel


            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif daskTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = daskTeklif.GenelBilgiler.TeklifId;

            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(daskTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(daskTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Riziko

            model.RizikoGenelBilgiler = DaskRizikoGenelBilgilerDetay(daskTeklif);
            model.RizikoAdresBilgiler = DaskRizikoAdresModelDetay(daskTeklif);
            model.RizikoDigerBilgiler = DaskRizikoDigerBilgilerDetay(daskTeklif);

            #endregion

            #region Teklif Odeme

            model.OdemeBilgileri = DaskPoliceOdemeModel(teklif);

            TeklifTeminat sigortaBedeli = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == DASKTeminatlar.DASK);
            if (sigortaBedeli != null)
                model.OdemeBilgileri.NetPrim = sigortaBedeli.TeminatBedeli.HasValue ? sigortaBedeli.TeminatBedeli.Value : 0;


            #endregion

            return View(model);
        }

        private TeklifFiyatModel DaskFiyat(DaskTeklif daskTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = daskTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = daskTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = daskTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = daskTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = daskTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = daskTeklif.GetIsDurum();
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
        public ActionResult OdemeAl(OdemeDaskModel model)
        {
            TeklifOdemeCevapModel cevap = new TeklifOdemeCevapModel();

            if (ModelState.IsValid)
            {
                nsbusiness.ITeklif teklif = _TeklifService.GetTeklif(model.KrediKarti.KK_TeklifId);

                nsbusiness.Odeme odeme = new nsbusiness.Odeme(model.KrediKarti.KartSahibi, model.KrediKarti.KartNumarasi.ToString(), model.KrediKarti.GuvenlikNumarasi, model.KrediKarti.SonKullanmaAy, model.KrediKarti.SonKullanmaYil);
                odeme.TaksitSayisi = model.KrediKarti.TaksitSayisi;

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
        public ActionResult Hesapla(DaskModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (model.RizikoGenelBilgiler != null)
                {
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

                    if (!model.RizikoGenelBilgiler.YanginPolicesiVarmi)
                    {
                        if (ModelState["RizikoGenelBilgiler.YanginSigortaSirketi"] != null)
                            ModelState["RizikoGenelBilgiler.YanginSigortaSirketi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.YanginPoliceNumarasi"] != null)
                            ModelState["RizikoGenelBilgiler.YanginPoliceNumarasi"].Errors.Clear();
                    }

                    if (!model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi)
                    {
                        if (ModelState["RizikoGenelBilgiler.DaskSigortaSirketi"] != null)
                            ModelState["RizikoGenelBilgiler.DaskSigortaSirketi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.DaskPoliceninVadeTarihi"] != null)
                            ModelState["RizikoGenelBilgiler.DaskPoliceninVadeTarihi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.DaskPoliceNo"] != null)
                            ModelState["RizikoGenelBilgiler.DaskPoliceNo"].Errors.Clear();
                    }
                    if (!String.IsNullOrEmpty(model.RizikoAdresBilgiler.UATVKodu) && model.RizikoAdresBilgiler.UATVKodu.Length == 10)
                    {
                        if (ModelState["RizikoAdresBilgiler.SemtBeldeKodu"] != null)
                            ModelState["RizikoAdresBilgiler.SemtBeldeKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.MahalleKodu"] != null)
                            ModelState["RizikoAdresBilgiler.MahalleKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.CaddeKodu"] != null)
                            ModelState["RizikoAdresBilgiler.CaddeKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.BinaKodu"] != null)
                            ModelState["RizikoAdresBilgiler.BinaKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.DaireKodu"] != null)
                            ModelState["RizikoAdresBilgiler.DaireKodu"].Errors.Clear();
                    }
                    if (model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi && !String.IsNullOrEmpty(model.RizikoGenelBilgiler.DaskPoliceNo))
                    {
                        if (ModelState["RizikoAdresBilgiler.IlKodu"] != null)
                            ModelState["RizikoAdresBilgiler.IlKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.IlceKodu"] != null)
                            ModelState["RizikoAdresBilgiler.IlceKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.SemtBeldeKodu"] != null)
                            ModelState["RizikoAdresBilgiler.SemtBeldeKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.MahalleKodu"] != null)
                            ModelState["RizikoAdresBilgiler.MahalleKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.CaddeKodu"] != null)
                            ModelState["RizikoAdresBilgiler.CaddeKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.BinaKodu"] != null)
                            ModelState["RizikoAdresBilgiler.BinaKodu"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.DaireKodu"] != null)
                            ModelState["RizikoAdresBilgiler.DaireKodu"].Errors.Clear();
                        if (ModelState["RizikoAdresBilgiler.DaireKodu"] != null)
                            ModelState["RizikoAdresBilgiler.UATVKodu"].Errors.Clear();


                        if (ModelState["RizikoDigerBilgiler.Ada"] != null)
                            ModelState["RizikoDigerBilgiler.Ada"].Errors.Clear();
                        if (ModelState["RizikoDigerBilgiler.YapiTarzi"] != null)
                            ModelState["RizikoDigerBilgiler.YapiTarzi"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.BinaInsaYili"] != null)
                            ModelState["RizikoDigerBilgiler.BinaInsaYili"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.HasarDurumu"] != null)
                            ModelState["RizikoDigerBilgiler.HasarDurumu"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.Parsel"] != null)
                            ModelState["RizikoDigerBilgiler.Parsel"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.PaftaNo"] != null)
                            ModelState["RizikoDigerBilgiler.PaftaNo"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.BinaKatSayisi"] != null)
                            ModelState["RizikoDigerBilgiler.BinaKatSayisi"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.DaireKullanimSekli"] != null)
                            ModelState["RizikoDigerBilgiler.DaireKullanimSekli"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.SigortaEttirenSifati"] != null)
                            ModelState["RizikoDigerBilgiler.SigortaEttirenSifati"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.DaireBrutYuzolcumu"] != null)
                            ModelState["RizikoDigerBilgiler.DaireBrutYuzolcumu"].Errors.Clear();

                        if (ModelState["RizikoDigerBilgiler.BinaKatSayisi"] != null)
                            ModelState["RizikoDigerBilgiler.BinaKatSayisi"].Errors.Clear();
                        if (ModelState["RizikoGenelBilgiler.DaskSigortaSirketi"] != null)
                            ModelState["RizikoGenelBilgiler.DaskSigortaSirketi"].Errors.Clear();

                        if (ModelState["RizikoGenelBilgiler.DaskPoliceninVadeTarihi"] != null)
                            ModelState["RizikoGenelBilgiler.DaskPoliceninVadeTarihi"].Errors.Clear();

                        if (ModelState["RizikoAdresBilgiler.PostaKodu"] != null)
                            ModelState["RizikoAdresBilgiler.PostaKodu"].Errors.Clear();
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
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.DogalAfetSigortasi_Deprem, model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu, model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Riziko Bilgileri

                    #region Riziko Genel Bilgiler

                    teklif.AddSoru(DASKSorular.Yangin_Police_VarYok, model.RizikoGenelBilgiler.YanginPolicesiVarmi);
                    if (model.RizikoGenelBilgiler.YanginPolicesiVarmi == true)
                    {
                        teklif.AddSoru(DASKSorular.Yangin_Police_Sigorta_Sirketi, model.RizikoGenelBilgiler.YanginSigortaSirketi);
                        teklif.AddSoru(DASKSorular.Yangin_Police_Numarasi, model.RizikoGenelBilgiler.YanginPoliceNumarasi);
                    }

                    teklif.AddSoru(DASKSorular.Yururlukte_dask_policesi_VarYok, model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi);
                    if (model.RizikoGenelBilgiler.YururlukteDaskPolicesiVarmi == true)
                    {
                        teklif.AddSoru(DASKSorular.Dask_Police_Sigorta_Sirketi, model.RizikoGenelBilgiler.DaskSigortaSirketi);

                        if (!String.IsNullOrEmpty(model.RizikoGenelBilgiler.DaskPoliceninVadeTarihi))
                            teklif.AddSoru(DASKSorular.Dask_Police_Vade_Tarihi, model.RizikoGenelBilgiler.DaskPoliceninVadeTarihi);

                        if (!String.IsNullOrEmpty(model.RizikoGenelBilgiler.DaskPoliceNo))
                            teklif.AddSoru(DASKSorular.Dask_Police_Numarasi, model.RizikoGenelBilgiler.DaskPoliceNo);
                    }

                    teklif.AddSoru(DASKSorular.RA_Dain_i_Muhtehin_VarYok, model.RizikoGenelBilgiler.RehinliAlacakliDainMurtehinVarmi);
                    if (model.RizikoGenelBilgiler.RehinliAlacakliDainMurtehinVarmi == true)
                    {
                        teklif.AddSoru(DASKSorular.RA_Tipi_Banka_Finansal_Kurum, model.RizikoGenelBilgiler.Tipi);
                        teklif.AddSoru(DASKSorular.RA_Kurum_Banka, model.RizikoGenelBilgiler.KurumBanka.ToNullSafeString());
                        teklif.AddSoru(DASKSorular.RA_Sube, model.RizikoGenelBilgiler.Sube.ToString());
                        teklif.AddSoru(DASKSorular.RA_Doviz_Kodu, model.RizikoGenelBilgiler.DovizKodu.ToNullSafeString());

                        teklif.AddSoru(DASKSorular.RA_Kredi_Tutari, model.RizikoGenelBilgiler.KrediTutari);
                        teklif.AddSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, model.RizikoGenelBilgiler.KrediReferansNo_HesapSozlesmeNo);
                        if (model.RizikoGenelBilgiler.KrediBitisTarihi.HasValue)
                            teklif.AddSoru(DASKSorular.RA_Kredi_Bitis_Tarihi, model.RizikoGenelBilgiler.KrediBitisTarihi.Value);
                    }

                    #endregion

                    #region Riziko Adres Bilgileri

                    //KOD Alanlar
                    teklif.RizikoAdresi.IlKodu = model.RizikoAdresBilgiler.IlKodu;
                    teklif.RizikoAdresi.IlceKodu = model.RizikoAdresBilgiler.IlceKodu;
                    teklif.RizikoAdresi.SemtBeldeKodu = model.RizikoAdresBilgiler.SemtBeldeKodu;
                    teklif.RizikoAdresi.MahalleKodu = model.RizikoAdresBilgiler.MahalleKodu;
                    teklif.RizikoAdresi.CaddeKodu = model.RizikoAdresBilgiler.CaddeKodu;
                    teklif.RizikoAdresi.PostaKodu = model.RizikoAdresBilgiler.PostaKodu;
                    teklif.RizikoAdresi.DaireKodu = model.RizikoAdresBilgiler.DaireKodu;
                    teklif.RizikoAdresi.BinaKodu = model.RizikoAdresBilgiler.BinaKodu;

                    //Adres doğrulama textbox değeri
                    if (!String.IsNullOrEmpty(model.RizikoAdresBilgiler.ParitusAdresDogrulama))
                        teklif.RizikoAdresi.Adres = model.RizikoAdresBilgiler.ParitusAdresDogrulama;

                    //EXTRA
                    teklif.RizikoAdresi.UAVTKodu = model.RizikoAdresBilgiler.UATVKodu;
                    teklif.RizikoAdresi.Latitude = model.RizikoAdresBilgiler.Latitude;
                    teklif.RizikoAdresi.Longitude = model.RizikoAdresBilgiler.Longitude;


                    //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
                    //HDIUAVTAdresResponse adresdetay = _HDIDask.GetUAVTAdres(teklif.RizikoAdresi.UAVTKodu);
                    //HDIUAVTAdresResponse.KAYIT adres = adresdetay.KAYITLAR.FirstOrDefault();
                    ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
                    if (!String.IsNullOrEmpty(teklif.RizikoAdresi.UAVTKodu))
                    {
                        var adres = _TURKNIPPONDask.GetAdresDetay(Convert.ToInt64(teklif.RizikoAdresi.UAVTKodu));
                        if (adres != null)
                        {
                            //TEXT Alanlar
                            teklif.RizikoAdresi.Il = adres.IlAdi;
                            teklif.RizikoAdresi.Ilce = adres.IlceAdi;
                            teklif.RizikoAdresi.SemtBelde = adres.BeldeAdi;
                            teklif.RizikoAdresi.Mahalle = adres.MahalleAdi;
                            teklif.RizikoAdresi.Cadde = adres.CaddeAdi + " " + adres.SokakAdi;
                            teklif.RizikoAdresi.Daire = adres.DaireNo;
                            teklif.RizikoAdresi.Bina = adres.BinaNo;
                        }
                    }

                    ////Teklif girişi manuel yapılmışsa adres enlem boylam bilgisi alınıyor
                    //if (String.IsNullOrEmpty(teklif.RizikoAdresi.Longitude) || String.IsNullOrEmpty(teklif.RizikoAdresi.Longitude)
                    //    || teklif.RizikoAdresi.Longitude == "0.0" || teklif.RizikoAdresi.Longitude == "0.0")
                    //{
                    //    ParitusAdresDogrulaLocal(teklif);
                    //}



                    #endregion

                    #region Riziko Diğer Bilgileri

                    teklif.AddSoru(DASKSorular.Yapi_Tarzi, model.RizikoDigerBilgiler.YapiTarzi);
                    teklif.AddSoru(DASKSorular.Bina_Kat_sayisi, model.RizikoDigerBilgiler.BinaKatSayisi);
                    teklif.AddSoru(DASKSorular.Bina_Insa_Yili, model.RizikoDigerBilgiler.BinaInsaYili);
                    teklif.AddSoru(DASKSorular.Daire_KullanimSekli, model.RizikoDigerBilgiler.DaireKullanimSekli);
                    teklif.AddSoru(DASKSorular.Hasar_Durumu, model.RizikoDigerBilgiler.HasarDurumu);
                    teklif.AddSoru(DASKSorular.Sigorta_Ettiren_Sifati, model.RizikoDigerBilgiler.SigortaEttirenSifati);
                    teklif.AddSoru(DASKSorular.Riziko_Parsel, model.RizikoDigerBilgiler.Parsel);
                    teklif.AddSoru(DASKSorular.Riziko_Kat_No, model.RizikoDigerBilgiler.KatNo);
                    teklif.AddSoru(DASKSorular.Riziko_Pafta_No, model.RizikoDigerBilgiler.PaftaNo);
                    teklif.AddSoru(DASKSorular.Riziko_Sayfa_No, model.RizikoDigerBilgiler.SayfaNo);
                    teklif.AddSoru(DASKSorular.Riziko_Ada, model.RizikoDigerBilgiler.Ada);
                    teklif.AddSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, model.RizikoDigerBilgiler.DaireBrutYuzolcumu);


                    teklif.AddSoru(DASKSorular.Tapuda_Birden_Fazla_Sigortali_VarYok, model.RizikoDigerBilgiler.TapudaBirdenFazlaSigortaliVarmi);
                    if (model.RizikoDigerBilgiler.TapudaBirdenFazlaSigortaliVarmi)
                    {
                        int sayac = 84;
                        for (int i = 0; i < model.RizikoDigerBilgiler.SigortaliList.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(model.RizikoDigerBilgiler.SigortaliList[i]))
                            {
                                teklif.AddSoru(sayac, model.RizikoDigerBilgiler.SigortaliList[i]);
                                sayac++;
                            }
                        }
                    }
                    #endregion

                    #endregion

                    #region Teklif return

                    IDaskTeklif daskTeklif = new DaskTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    foreach (var item in model.TeklifUM)
                    {
                        if (item.TeklifAl)
                            daskTeklif.AddUretimMerkezi(item.TUMKodu);
                    }

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.Odeme.OdemeTipi;

                    if (!model.Odeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                        daskTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        daskTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = daskTeklif.Hesapla(teklif);
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

        public ActionResult GetListSube(int KurumKodu)
        {
            return Json(new SelectList(_CRService.GetListDaskSubeler(KurumKodu), "SubeKodu", "SubeAdi").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }

        #region Adres servisleri

        public ActionResult GetListIlce(int IlKodu)
        {
            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            //HDIIlcelerResponse model = _HDIDask.GetUAVTIlcelerList(IlKodu);
            //return Json(new SelectList(model.KAYITLAR, "Kod", "Aciklama", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);

            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            return Json(new SelectList(_TURKNIPPONDask.GetIlceList(IlKodu), "Code", "Description", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetListBelde(int IlceKodu)
        {
            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            //HDIBeldelerResponse model = _HDIDask.GetUAVTBeldelerList(IlceKodu);
            //return Json(new SelectList(model.KAYITLAR, "Kod", "Aciklama", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);

            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            return Json(new SelectList(_TURKNIPPONDask.GetBeldeList(IlceKodu), "Code", "Description", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetListMahalle(int BeldeKodu)
        {
            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            //HDIMahallelerResponse model = _HDIDask.GetUAVTMahallelerList(BeldeKodu);
            //return Json(new SelectList(model.KAYITLAR, "Kod", "Aciklama", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);

            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            return Json(new SelectList(_TURKNIPPONDask.GetMahalleList(BeldeKodu), "Code", "Description", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetListCadSkBulMeydan(int MahalleKodu)
        {
            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();

            //HDICaddeSokakBulvarMeydanResponse model = _HDIDask.GetUAVTCadSkBlvMeydanList(MahalleKodu, Aciklama);

            //return Json(new SelectList(model.KAYITLAR, "Kod", "Aciklama", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);

            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            return Json(new SelectList(_TURKNIPPONDask.GetCaddeSokakList(MahalleKodu), "Code", "Description", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetListCadSkBulMeydan_BinaAd(int CadSkBulMeyKodu)
        {
            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            //HDICaddeSokakBulvarMeydanBinaAdResponse model = _HDIDask.GetUAVTCadSkBlvMeydan_BinaAdList(CadSkBulMeyKodu, Aciklama);
            //return Json(new SelectList(model.KAYITLAR, "Kod", "BinaNo", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);

            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            return Json(new SelectList(_TURKNIPPONDask.GetBinaList(CadSkBulMeyKodu), "Code", "Description", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetListDaireler(int BinaNo)
        {
            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            //HDIDairelerResponse model = _HDIDask.GetUAVTDairelerList(BinaNo);
            //return Json(new SelectList(model.KAYITLAR, "UAVT", "DaireNo", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);

            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            return Json(new SelectList(_TURKNIPPONDask.GetBagimsizBolumList(BinaNo), "Code", "Description", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetUAVTAdres(string UavtKodu)
        {
            // IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            // HDIUAVTAdresResponse model = _HDIDask.GetUAVTAdres(UavtKodu);
            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            if (!String.IsNullOrEmpty(UavtKodu))
            {
                ReturnAdresModel adresModel = _TURKNIPPONDask.GetAdresDetay(Convert.ToInt64(UavtKodu));
                return Json(adresModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }




        }

        [HttpPost]
        [AjaxException]
        public ActionResult ParitusAdresDogrulama(string paritusAdres)
        {
            ParitusAdresModel model = new ParitusAdresModel();
            model.Durum = ParitusAdresSorgulamaDurum.Basarisiz;

            try
            {
                if (!String.IsNullOrEmpty(paritusAdres))
                {
                    IParitusService _ParitusService = DependencyResolver.Current.GetService<IParitusService>();

                    ParitusAdresSorgulamaRequest adresModel = new ParitusAdresSorgulamaRequest();
                    adresModel.address = paritusAdres;

                    model = _ParitusService.GetParitusAdres(adresModel);


                    if (!String.IsNullOrEmpty(model.IlKodu))
                    {
                        IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();

                        int ilkodu = Convert.ToInt32(model.IlKodu);

                        //ILCELER
                        HDIIlcelerResponse ilceList = new HDIIlcelerResponse();

                        Thread thr1 = new Thread(() =>
                        {
                            ilceList = _HDIDask.GetUAVTIlcelerList(ilkodu);
                            model.IlceLer = new SelectList(ilceList.KAYITLAR, "Kod", "Aciklama", model.IlceKodu).ListWithOptionLabel();
                        });

                        thr1.Start();


                        Thread thr2 = new Thread(() =>
                        {
                            //BELDELER
                            if (model.IlceKodu > 0)
                            {
                                HDIBeldelerResponse beldeList = new HDIBeldelerResponse();

                                beldeList = _HDIDask.GetUAVTBeldelerList(model.IlceKodu);
                                model.Beldeler = new SelectList(beldeList.KAYITLAR, "Kod", "Aciklama", "").ListWithOptionLabel();
                            }
                        });

                        thr2.Start();

                        Thread thr3 = new Thread(() =>
                        {
                            if (!String.IsNullOrEmpty(model.uavtAddressCode))
                            {
                                HDIUAVTAdresResponse uatvAdres = new HDIUAVTAdresResponse();

                                uatvAdres = _HDIDask.GetUAVTAdres(model.uavtAddressCode);
                                if (uatvAdres != null && uatvAdres.KAYITLAR.Count() > 0)
                                {
                                    Neosinerji.BABOnlineTP.Business.HDI.HDIUAVTAdresResponse.KAYIT kayit = uatvAdres.KAYITLAR.FirstOrDefault();
                                    if (kayit != null)
                                    {
                                        model.BeldeKodu = kayit.BeldeKod;
                                        model.Pafta = kayit.Pafta;
                                        model.Parsel = kayit.Parsel;
                                        model.Ada = kayit.Ada;

                                        int beldekodu = Convert.ToInt32(model.BeldeKodu);

                                        HDIMahallelerResponse mahalle = _HDIDask.GetUAVTMahallelerList(beldekodu);

                                        model.Mahalleler = new SelectList(mahalle.KAYITLAR, "Kod", "Aciklama", beldekodu).ListWithOptionLabel();
                                    }
                                }
                            }
                        });

                        thr3.Start();

                        thr1.Join();
                        thr2.Join();
                        thr3.Join();
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        private void ParitusAdresDogrulaLocal(ITeklif teklif)
        {
            IParitusService _ParitusService = DependencyResolver.Current.GetService<IParitusService>();
            TeklifRizikoAdresi adres = teklif.RizikoAdresi;

            if (adres != null)
            {
                ParitusAdresSorgulamaRequest request = new ParitusAdresSorgulamaRequest();

                if (!String.IsNullOrEmpty(adres.Cadde))
                    request.address += adres.Cadde + " ";

                if (!String.IsNullOrEmpty(adres.Mahalle))
                    request.address += adres.Mahalle + " ";

                if (!String.IsNullOrEmpty(adres.Sokak))
                    request.address += adres.Sokak + " ";

                if (!String.IsNullOrEmpty(adres.Bina))
                    request.address += adres.Bina + " ";

                if (!String.IsNullOrEmpty(adres.Daire))
                    request.address += " Daire : " + adres.Daire + " ";

                if (!String.IsNullOrEmpty(adres.SemtBelde))
                    request.address += adres.SemtBelde + " ";

                if (!String.IsNullOrEmpty(adres.Ilce))
                    request.address += adres.Ilce + " ";

                if (!String.IsNullOrEmpty(adres.Il))
                    request.address += adres.Il + " ";

                if (adres.PostaKodu.HasValue)
                    request.address += adres.PostaKodu.Value.ToString();


                ParitusAdresModel paritusModel = _ParitusService.GetParitusAdres(request);
                if (paritusModel != null && !String.IsNullOrEmpty(paritusModel.Latitude) && !String.IsNullOrEmpty(paritusModel.Longitude))
                {
                    adres.Latitude = paritusModel.Latitude;
                    adres.Longitude = paritusModel.Longitude;

                    if (String.IsNullOrEmpty(adres.Adres))
                        adres.Adres = paritusModel.FullAdres;
                }
            }
        }

        #endregion

        private DaskRizikoGenelBilgiler DaskRizikoGenelBilgilerDetay(ITeklif teklif)
        {
            ISigortaSirketleriService _SigortaSirketiService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
            DaskRizikoGenelBilgiler model = new DaskRizikoGenelBilgiler();

            try
            {
                model.YururlukteDaskPolicesiVarmi = teklif.ReadSoru(DASKSorular.Yururlukte_dask_policesi_VarYok, false); ;
                model.YanginPolicesiVarmi = teklif.ReadSoru(DASKSorular.Yangin_Police_VarYok, false);
                model.RehinliAlacakliDainMurtehinVarmi = teklif.ReadSoru(DASKSorular.RA_Dain_i_Muhtehin_VarYok, false);

                if (model.YururlukteDaskPolicesiVarmi == true)
                {
                    model.DaskSigortaSirketi = teklif.ReadSoru(DASKSorular.Dask_Police_Sigorta_Sirketi, String.Empty);
                    model.DaskPoliceninVadeTarihi = teklif.ReadSoru(DASKSorular.Dask_Police_Vade_Tarihi, String.Empty);
                    model.DaskPoliceNo = teklif.ReadSoru(DASKSorular.Dask_Police_Numarasi, String.Empty);
                }

                if (model.YanginPolicesiVarmi == true)
                {
                    SigortaSirketleri sirket = _SigortaSirketiService.GetSirket(teklif.ReadSoru(DASKSorular.Yangin_Police_Sigorta_Sirketi, "0"));

                    if (sirket != null)
                        model.YanginSigortaSirketi = sirket.SirketAdi;
                    model.YanginPoliceNumarasi = teklif.ReadSoru(DASKSorular.Yangin_Police_Numarasi, "0");
                }

                if (model.RehinliAlacakliDainMurtehinVarmi == true)
                {
                    model.Tipi = teklif.ReadSoru(DASKSorular.RA_Tipi_Banka_Finansal_Kurum, "0") == "1" ? "Banka" : "Finansal Kurum";
                    model.KrediReferansNo_HesapSozlesmeNo = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, String.Empty); ;
                    model.KrediBitisTarihi = teklif.ReadSoru(DASKSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);
                    model.DovizKoduText = DaskDovizTipleri.DovizTipi(Convert.ToByte(teklif.ReadSoru(DASKSorular.RA_Doviz_Kodu, "0")));
                    model.KrediTutari = teklif.ReadSoru(DASKSorular.RA_Kredi_Tutari, String.Empty);

                    DaskKurumlar kurum = _CRService.GetDaskKurum(Convert.ToInt32(teklif.ReadSoru(DASKSorular.RA_Kurum_Banka, "0")));
                    if (kurum != null)
                    {
                        model.KurumBankaAdi = kurum.KurumAdi;
                        DaskSubeler sube = _CRService.GetDaskSube(kurum.KurumKodu, Convert.ToInt32(teklif.ReadSoru(DASKSorular.RA_Sube, "0")));
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

        private DaskRizikoAdresModel DaskRizikoAdresModelDetay(ITeklif teklif)
        {
            DaskRizikoAdresModel model = new DaskRizikoAdresModel();

            TeklifRizikoAdresi adres = teklif.RizikoAdresi;
            if (adres != null)
            {
                model.Il = adres.Il;
                model.Ilce = adres.Ilce;
                model.SemtBelde = adres.SemtBelde;
                model.Mahalle = adres.Mahalle;
                model.Cadde = adres.Cadde;
                model.Bina = adres.Bina;
                model.Daire = adres.Daire;
                model.PostaKodu = adres.PostaKodu;
            }

            return model;
        }

        private DaskRizikoDigerBilgiler DaskRizikoDigerBilgilerDetay(ITeklif teklif)
        {
            DaskRizikoDigerBilgiler model = new DaskRizikoDigerBilgiler();

            try
            {
                string yapiTarzi = teklif.ReadSoru(DASKSorular.Yapi_Tarzi, String.Empty);
                string katSayisi = teklif.ReadSoru(DASKSorular.Bina_Kat_sayisi, String.Empty);
                string binaInsaYili = teklif.ReadSoru(DASKSorular.Bina_Insa_Yili, String.Empty);
                string daireKullanimSekli = teklif.ReadSoru(DASKSorular.Daire_KullanimSekli, String.Empty);
                string yuzOlcumM2 = teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                string hasarDurumu = teklif.ReadSoru(DASKSorular.Hasar_Durumu, String.Empty);
                string SE_Sifati = teklif.ReadSoru(DASKSorular.Sigorta_Ettiren_Sifati, String.Empty);
                string katNo = teklif.ReadSoru(DASKSorular.Riziko_Kat_No, String.Empty);
                string paftaNo = teklif.ReadSoru(DASKSorular.Riziko_Pafta_No, String.Empty);
                string sayfaNo = teklif.ReadSoru(DASKSorular.Riziko_Sayfa_No, String.Empty);
                string ada = teklif.ReadSoru(DASKSorular.Riziko_Ada, String.Empty);
                string parsel = teklif.ReadSoru(DASKSorular.Riziko_Parsel, String.Empty);


                if (!String.IsNullOrEmpty(yapiTarzi))
                    model.YapiTarzi = DaskYapiTarzlari.YapiTarzi(Convert.ToByte(yapiTarzi));

                if (!String.IsNullOrEmpty(katSayisi))
                    model.BinaKatSayisi = DaskBinaKatSayilari.BinaKatSayisi(Convert.ToByte(katSayisi));

                if (!String.IsNullOrEmpty(binaInsaYili))
                    model.BinaInsaYili = DaskBinaInsaYillari.BinaInsaYili(Convert.ToByte(binaInsaYili));

                if (!String.IsNullOrEmpty(daireKullanimSekli))
                    model.DaireKullanimSekli = DaskBinaKullanimSeklilleri.KullanimSekli(Convert.ToByte(daireKullanimSekli));

                if (!String.IsNullOrEmpty(hasarDurumu))
                    model.HasarDurumu = DaskBinaHasarDurumlari.HasarDurumu(Convert.ToByte(hasarDurumu));

                if (!String.IsNullOrEmpty(SE_Sifati))
                    model.SigortaEttirenSifati = Dask_S_EttirenSifatlari.Sifati(Convert.ToByte(SE_Sifati));

                model.DaireBrutYuzolcumu = yuzOlcumM2;
                model.KatNo = katNo;
                model.PaftaNo = paftaNo;
                model.SayfaNo = sayfaNo;
                model.Ada = ada;
                model.Parsel = parsel;

                model.TapudaBirdenFazlaSigortaliVarmi = teklif.ReadSoru(DASKSorular.Tapuda_Birden_Fazla_Sigortali_VarYok, false);
                if (model.TapudaBirdenFazlaSigortaliVarmi)
                {
                    model.SigortaliList = new List<string>();
                    int sayac = 84;
                    for (int i = 0; i < 10; i++)
                    {
                        string sigortaliAdi = teklif.ReadSoru(sayac, "");
                        if (!String.IsNullOrEmpty(sigortaliAdi))
                            model.SigortaliList.Add(sigortaliAdi);

                        sayac++;
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

        private DaskPoliceOdemeModel DaskPoliceOdemeModel(ITeklif teklif)
        {
            DaskPoliceOdemeModel model = new DaskPoliceOdemeModel();

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

        private DaskRizikoGenelBilgiler DaskRizikoGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            DaskRizikoGenelBilgiler model = new DaskRizikoGenelBilgiler();

            //Evet ise sigorta şirketi ve poliçe vade tarihi bilgileri alınacak.
            model.YururlukteDaskPolicesiVarmi = false;
            model.SigortaSirketleri = base.SigortaSirketleri;
            model.YanginPolicesiVarmi = false;
            model.RehinliAlacakliDainMurtehinVarmi = false;
            model.Tipler = new SelectList(TeklifProvider.DaskKurumTipleri(), "Value", "Text", "").ToList();
            model.Kurum_Bankalar = new SelectList(_CRService.GetListDaskKurumlar(), "KurumKodu", "KurumAdi", "").ListWithOptionLabel();
            model.Subeler = new List<SelectListItem>();
            model.DovizKodlari = new SelectList(TeklifProvider.DaskDovizKodlari(), "Value", "Text", "").ListWithOptionLabel();

            if (teklifId.HasValue & teklif != null)
            {
                model.YururlukteDaskPolicesiVarmi = teklif.ReadSoru(DASKSorular.Yururlukte_dask_policesi_VarYok, false); ;
                model.YanginPolicesiVarmi = teklif.ReadSoru(DASKSorular.Yangin_Police_VarYok, false);
                model.RehinliAlacakliDainMurtehinVarmi = teklif.ReadSoru(DASKSorular.RA_Dain_i_Muhtehin_VarYok, false);

                if (model.YururlukteDaskPolicesiVarmi)
                {
                    model.DaskSigortaSirketi = teklif.ReadSoru(DASKSorular.Dask_Police_Sigorta_Sirketi, "0");
                    model.DaskPoliceninVadeTarihi = teklif.ReadSoru(DASKSorular.Dask_Police_Vade_Tarihi, String.Empty);
                    model.DaskPoliceNo = teklif.ReadSoru(DASKSorular.Dask_Police_Numarasi, String.Empty);
                }

                if (model.YanginPolicesiVarmi)
                {
                    model.YanginSigortaSirketi = teklif.ReadSoru(DASKSorular.Yangin_Police_Sigorta_Sirketi, "0");
                    model.YanginPoliceNumarasi = teklif.ReadSoru(DASKSorular.Yangin_Police_Numarasi, "0");
                }

                if (model.RehinliAlacakliDainMurtehinVarmi)
                {
                    model.Tipi = teklif.ReadSoru(DASKSorular.RA_Tipi_Banka_Finansal_Kurum, "0");
                    model.KurumBanka = (int)teklif.ReadSoru(DASKSorular.RA_Kurum_Banka, 0);
                    model.Subeler = new SelectList(_CRService.GetListDaskSubeler(model.KurumBanka), "SubeKodu", "SubeAdi", "").ListWithOptionLabel();
                    model.Sube = (int)teklif.ReadSoru(DASKSorular.RA_Sube, 0);
                    model.KrediReferansNo_HesapSozlesmeNo = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, "0");
                    model.KrediBitisTarihi = teklif.ReadSoru(DASKSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);
                    model.KrediTutari = teklif.ReadSoru(DASKSorular.RA_Kredi_Tutari, "0");
                    model.DovizKodu = (byte)teklif.ReadSoru(DASKSorular.RA_Doviz_Kodu, 0);
                }
            }

            return model;
        }

        private DaskRizikoAdresModel DaskRizikoAdresModel(int? teklifId, ITeklif teklif)
        {
            DaskRizikoAdresModel model = new DaskRizikoAdresModel();

            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            //model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", "34").ListWithOptionLabelIller();

            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            model.Iller = new SelectList(_TURKNIPPONDask.GetIlList(), "Code", "Description", "").ListWithOptionLabelIller();

            model.Ilceler = new List<SelectListItem>();
            model.Beldeler = new List<SelectListItem>();
            model.Caddeler = new List<SelectListItem>();
            model.Mahalleler = new List<SelectListItem>();
            model.Binalar = new List<SelectListItem>();
            model.Daireler = new List<SelectListItem>();


            if (teklifId.HasValue && teklif != null)
            {
                TeklifRizikoAdresi adres = teklif.RizikoAdresi;
                if (adres != null && adres.IlKodu.HasValue)
                {
                    model.IlKodu = adres.IlKodu.Value;
                    model.ParitusAdresDogrulama = adres.Adres;
                    model.UATVKodu = adres.UAVTKodu;
                    model.Latitude = adres.Latitude;
                    model.Longitude = adres.Longitude;

                    if (adres.IlceKodu.HasValue)
                    {
                        model.Ilceler = new SelectList(_TURKNIPPONDask.GetIlceList(adres.IlKodu.Value), "Code", "Description", "").ListWithOptionLabel();
                        model.IlceKodu = adres.IlceKodu.Value;

                        if (!String.IsNullOrEmpty(adres.SemtBeldeKodu))
                        {
                            model.Beldeler = new SelectList(_TURKNIPPONDask.GetBeldeList(adres.IlceKodu.Value),
                                                                                         "Code", "Description", "").ListWithOptionLabel();
                            model.SemtBeldeKodu = adres.SemtBeldeKodu;

                            if (!String.IsNullOrEmpty(adres.MahalleKodu))
                            {
                                model.Mahalleler = new SelectList(_TURKNIPPONDask.GetMahalleList(Convert.ToInt32(adres.SemtBeldeKodu)),
                                                                                             "Code", "Description", "").ListWithOptionLabel();
                                model.MahalleKodu = adres.MahalleKodu;

                                if (!String.IsNullOrEmpty(adres.CaddeKodu))
                                {
                                    model.Caddeler = new SelectList(_TURKNIPPONDask.GetCaddeSokakList(Convert.ToInt32(adres.MahalleKodu)),
                                                                                                       "Code", "Description", "").ListWithOptionLabel();
                                    model.CaddeKodu = adres.CaddeKodu;

                                    if (!String.IsNullOrEmpty(adres.BinaKodu))
                                    {
                                        model.Binalar = new SelectList(_TURKNIPPONDask.GetBinaList(Convert.ToInt32(adres.CaddeKodu)),
                                                                                                            "Code", "Description", "").ListWithOptionLabel();
                                        model.BinaKodu = adres.BinaKodu;

                                        if (!String.IsNullOrEmpty(adres.DaireKodu))
                                        {
                                            model.Daireler = new SelectList(_TURKNIPPONDask.GetBagimsizBolumList(Convert.ToInt32(adres.BinaKodu)),
                                                                                                                "Code", "Description", "").ListWithOptionLabel();
                                            model.DaireKodu = adres.DaireKodu;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                model.PostaKodu = teklif.RizikoAdresi.PostaKodu;
            }

            return model;
        }

        private DaskRizikoDigerBilgiler DaskRizikoDigerBilgiler(int? teklifId, ITeklif teklif)
        {
            DaskRizikoDigerBilgiler model = new DaskRizikoDigerBilgiler();

            model.YapiTarzlari = new SelectList(TeklifProvider.DaskBinaYapiTazrlari(), "Value", "Text", "").ListWithOptionLabel();
            model.BinaKatSayilari = new SelectList(TeklifProvider.DaskBinaKatSayisi(), "Value", "Text", "").ListWithOptionLabel();
            model.BinaInsaYillari = new SelectList(TeklifProvider.DaskBinaInsaYili(), "Value", "Text", "").ListWithOptionLabel();
            model.DaireKullanımSekilleri = new SelectList(TeklifProvider.DaskBinaKullanimSekli(), "Value", "Text", "").ListWithOptionLabel();
            model.HasarDurumlari = new SelectList(TeklifProvider.DaskBinaHasarDurumu(), "Value", "Text", "").ListWithOptionLabel();
            model.S_EttirenSifatlari = new SelectList(TeklifProvider.Dask_S_EttirenSifati(), "Value", "Text", "").ListWithOptionLabel();
            model.SigortaliList = new List<string>();


            if (teklifId.HasValue & teklif != null)
            {
                model.YapiTarzi = teklif.ReadSoru(DASKSorular.Yapi_Tarzi, String.Empty);
                model.BinaKatSayisi = teklif.ReadSoru(DASKSorular.Bina_Kat_sayisi, String.Empty);
                model.BinaInsaYili = teklif.ReadSoru(DASKSorular.Bina_Insa_Yili, String.Empty);
                model.DaireKullanimSekli = teklif.ReadSoru(DASKSorular.Daire_KullanimSekli, String.Empty);
                model.DaireBrutYuzolcumu = teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                model.HasarDurumu = teklif.ReadSoru(DASKSorular.Hasar_Durumu, String.Empty);
                model.SigortaEttirenSifati = teklif.ReadSoru(DASKSorular.Sigorta_Ettiren_Sifati, String.Empty);
                model.KatNo = teklif.ReadSoru(DASKSorular.Riziko_Kat_No, String.Empty);
                model.PaftaNo = teklif.ReadSoru(DASKSorular.Riziko_Pafta_No, String.Empty);
                model.SayfaNo = teklif.ReadSoru(DASKSorular.Riziko_Sayfa_No, String.Empty);
                model.Ada = teklif.ReadSoru(DASKSorular.Riziko_Ada, String.Empty);
                model.Parsel = teklif.ReadSoru(DASKSorular.Riziko_Parsel, String.Empty);

                model.TapudaBirdenFazlaSigortaliVarmi = teklif.ReadSoru(DASKSorular.Tapuda_Birden_Fazla_Sigortali_VarYok, false);
                if (model.TapudaBirdenFazlaSigortaliVarmi)
                {
                    int sayac = 84;
                    for (int i = 0; i < 10; i++)
                    {
                        model.SigortaliList.Add(teklif.ReadSoru(sayac, ""));
                        sayac++;
                    }
                }
                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        model.SigortaliList.Add(string.Empty);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    model.SigortaliList.Add(string.Empty);
                }
            }

            return model;
        }

        [HttpPost]
        [AjaxException]
        public ActionResult KimlikNoSorgulaRiziko(string kimlikNo)
        {
            MusteriModel model = new MusteriModel();

            if (String.IsNullOrEmpty(kimlikNo))
            {
                model.SorgulamaHata("Kimlik numarası boş bırakılamaz");
                model.SorgulamaSonuc = false;
            }
            else if (kimlikNo.Length == 11 || kimlikNo.Length == 10)
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, _AktifKullaniciService.TVMKodu);
                if (musteri != null)
                {
                    MusteriAdre adres = musteri.MusteriAdres.FirstOrDefault(m => m.Varsayilan == true);
                    if (adres != null)
                    {
                        model.AdresTipi = adres.AdresTipi.HasValue ? adres.AdresTipi.Value : 0;
                        model.Mahalle = adres.Mahalle;
                        model.Cadde = adres.Cadde;
                        model.Sokak = adres.Sokak;
                        model.Apartman = adres.Apartman;
                        model.BinaNo = adres.BinaNo;
                        model.DaireNo = adres.DaireNo;
                        model.PostaKodu = adres.PostaKodu;

                        model.SorgulamaSonuc = true;
                        return Json(model);
                    }
                    model.SorgulamaHata("Girilen kimlik numarasına ait bilgi bulunamadı.");
                    model.SorgulamaSonuc = false;
                    return Json(model);
                }
                model.SorgulamaHata("Girilen kimlik numarasına ait bilgi bulunamadı.");
                model.SorgulamaSonuc = false;
                return Json(model);
            }
            model.SorgulamaHata("Kimlik numarası tüzel müşteriler için 10, şahıslar için 11 rakamdan oluşmalıdır");
            model.SorgulamaSonuc = false;
            return Json(model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult EskiPoliceSorgulama(string EskiPoliceNo)
        {
            //    HDIDASKEskiPoliceResponse model = new HDIDASKEskiPoliceResponse();
            //    model.Durum = "1";
            //    model.DurumAciklama = "Bir hata oluştu";
            ITURKNIPPONDask _TURKNIPPONDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            var policeDetay = _TURKNIPPONDask.GetPoliceDetay(Convert.ToInt64(EskiPoliceNo), 0, 0);
            UavtAdresDEtayModel model = new UavtAdresDEtayModel();
            if (String.IsNullOrEmpty(policeDetay.Hata))
            {
                var UavtAdresDetay = this.AdresParts(policeDetay.adres);

                if (UavtAdresDetay != null)
                {
                    model = UavtAdresDetay;
                }
                model.policeBilgi = policeDetay;
            }
            else
            {
                model.HataMesaji = policeDetay.Hata;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
            //try
            //{
            //    if (!String.IsNullOrEmpty(EskiPoliceNo))
            //    {
            //        model = _TeklifService.EskiPoliceSorgula(EskiPoliceNo);

            //        if (model.Durum == "0" && model.DurumAciklama == "Başarılı")
            //        {
            //            if (model.PoliceBilgileri != null)
            //            {
            //                Poliçe Bitiş Tarihi
            //                if (!String.IsNullOrEmpty(model.PoliceBilgileri.PoliceBitTarihi))
            //                {
            //                    DateTime dt;
            //                    if (DateTime.TryParseExact(model.PoliceBilgileri.PoliceBitTarihi, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
            //                        model.PoliceBilgileri.PoliceBitTarihi = dt.ToString("dd.MM.yyyy");
            //                }

            //                Dask Riziko Bilgileri
            //                if (model.PoliceBilgileri.RizikoBilgileri != null && !String.IsNullOrEmpty(model.PoliceBilgileri.RizikoBilgileri.IlKod))
            //                {
            //                    IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();

            //                    int? ilkodu = Convert.ToInt32(model.PoliceBilgileri.RizikoBilgileri.IlKod);

            //                    if (ilkodu.HasValue)
            //                    {
            //                        HDIIlcelerResponse ilceler = _HDIDask.GetUAVTIlcelerList(ilkodu.Value);
            //                        model.PoliceBilgileri.RizikoBilgileri.Ilceler = new SelectList(ilceler.KAYITLAR, "Kod", "Aciklama", "2039").ListWithOptionLabel();


            //                        if (!String.IsNullOrEmpty(model.PoliceBilgileri.RizikoBilgileri.Ilce))
            //                        {
            //                            int? ilcekodu = Convert.ToInt32(model.PoliceBilgileri.RizikoBilgileri.Ilce);
            //                            if (ilcekodu.HasValue)
            //                            {
            //                                HDIBeldelerResponse beldeler = _HDIDask.GetUAVTBeldelerList(ilcekodu.Value);


            //                                model.PoliceBilgileri.RizikoBilgileri.Beldeler = new SelectList(beldeler.KAYITLAR, "Kod", "Aciklama",
            //                                                                                model.PoliceBilgileri.RizikoBilgileri.Belde).ListWithOptionLabel();
            //                            }
            //                        }
            //                    }
            //                }


            //                Rehinli Alacaklı bilgileri
            //                if (model.PoliceBilgileri.RehinAlacakBilgileri != null && model.PoliceBilgileri.RehinAlacakBilgileri.RehinAlacak == "E")
            //                {
            //                    if (!String.IsNullOrEmpty(model.PoliceBilgileri.RehinAlacakBilgileri.KurumID))
            //                    {
            //                        int? kurumId = Convert.ToInt32(model.PoliceBilgileri.RehinAlacakBilgileri.KurumID);
            //                        if (kurumId.HasValue)
            //                            model.PoliceBilgileri.RehinAlacakBilgileri.Subeler = new SelectList(_CRService.GetListDaskSubeler(kurumId.Value),
            //                                                                                                 "SubeKodu", "SubeAdi", "1004").ListWithOptionLabel();
            //                    }

            //                    if (!String.IsNullOrEmpty(model.PoliceBilgileri.RehinAlacakBilgileri.KrediBitisTarih))
            //                    {
            //                        DateTime krediDT;
            //                        if (DateTime.TryParseExact(model.PoliceBilgileri.RehinAlacakBilgileri.KrediBitisTarih, "yyyyMMdd",
            //                                                           CultureInfo.InvariantCulture, DateTimeStyles.None, out krediDT))
            //                            model.PoliceBilgileri.RehinAlacakBilgileri.KrediBitisTarih = krediDT.ToString("dd.MM.yyyy");
            //                    }
            //                }

            //                if (model.PoliceBilgileri.Sigortalilar != null)
            //                    model.PoliceBilgileri.SigortaliSayisi = model.PoliceBilgileri.Sigortalilar.Count();
            //            }
            //        }
            //    }

            //    return Json(model, JsonRequestBehavior.AllowGet);
            //}
            //catch (Exception ex)
            //{
            //    _LogService.Error(ex);
            //    return Json(model, JsonRequestBehavior.AllowGet);
            //}
            //return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult GetUAVTAdresDetay(string UAVTAdresKodu)
        {
            ITURKNIPPONDask _TurkNipponDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            var UavtAdresDetay = _TurkNipponDask.GetAdresDetay(Convert.ToInt64(UAVTAdresKodu));
            //IHDIDask _HDIDask = DependencyResolver.Current.GetService<IHDIDask>();
            UavtAdresDEtayModel model = new UavtAdresDEtayModel();
            if (UavtAdresDetay != null)
            {
                model = AdresParts(UavtAdresDetay);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public UavtAdresDEtayModel AdresParts(ReturnAdresModel UavtAdresDetay)
        {
            ITURKNIPPONDask _TurkNipponDask = DependencyResolver.Current.GetService<ITURKNIPPONDask>();
            UavtAdresDEtayModel model = new UavtAdresDEtayModel();
            model.IlKodu = UavtAdresDetay.IlKodu;

            model.IlceKodu = UavtAdresDetay.IlceKodu;
            var ilceler = _TurkNipponDask.GetIlceList(UavtAdresDetay.IlKodu);
            model.Ilceler = new SelectList(ilceler, "Code", "Description", UavtAdresDetay.IlceKodu).ListWithOptionLabel();

            model.SemtBeldeKodu = UavtAdresDetay.BeldeKodu.ToString();
            var beldeler = _TurkNipponDask.GetBeldeList(UavtAdresDetay.IlceKodu);
            model.Beldeler = new SelectList(beldeler, "Code", "Description", UavtAdresDetay.BeldeKodu).ListWithOptionLabel();

            model.MahalleKodu = UavtAdresDetay.MahalleKodu.ToString();
            var mahalleler = _TurkNipponDask.GetMahalleList(Convert.ToInt32(UavtAdresDetay.BeldeKodu));
            model.Mahalleler = new SelectList(mahalleler, "Code", "Description", UavtAdresDetay.MahalleKodu).ListWithOptionLabel();

            model.CaddeKodu = UavtAdresDetay.CaddeSokakKodu.ToString();
            var caddeSokaklar = _TurkNipponDask.GetCaddeSokakList(UavtAdresDetay.MahalleKodu);
            model.Caddeler = new SelectList(caddeSokaklar, "Code", "Description", UavtAdresDetay.CaddeSokakKodu).ListWithOptionLabel();

            model.BinaKodu = UavtAdresDetay.BinaKodu.ToString();
            var BinaNolar = _TurkNipponDask.GetBinaList(UavtAdresDetay.CaddeSokakKodu);
            model.Binalar = new SelectList(BinaNolar, "Code", "Description", UavtAdresDetay.BinaKodu).ListWithOptionLabel();

            model.DaireKodu = UavtAdresDetay.UAVTAdresKodu;
            var Daireler = _TurkNipponDask.GetBagimsizBolumList(Convert.ToInt32(UavtAdresDetay.BinaKodu));
            model.Daireler = new SelectList(Daireler, "Code", "Description", UavtAdresDetay.DaireNo).ListWithOptionLabel();
            model.Pafta = UavtAdresDetay.Pafta;
            model.Ada = UavtAdresDetay.Ada;
            model.Parsel = UavtAdresDetay.Parsel;
            model.SayfaNo = UavtAdresDetay.SayfaNo;

            model.MahalleAdi = UavtAdresDetay.MahalleAdi;
            model.BeldeAdi = UavtAdresDetay.BeldeAdi;
            model.CaddeAdi = UavtAdresDetay.CaddeAdi + " " + UavtAdresDetay.SokakAdi;
            model.IlAdi = UavtAdresDetay.IlAdi;
            model.IlceAdi = UavtAdresDetay.IlceAdi;
            model.DaireNo = UavtAdresDetay.DaireNo;
            model.BinaNo = UavtAdresDetay.BinaNo;
            model.HataMesaji = UavtAdresDetay.Hata;
            model.UATVKodu = UavtAdresDetay.UAVTAdresKodu;
            return model;
        }
        public class UavtAdresDEtayModel
        {
            public int IlKodu { get; set; }
            public int IlceKodu { get; set; }
            public string SemtBeldeKodu { get; set; }
            public string MahalleKodu { get; set; }
            public string CaddeKodu { get; set; }
            public string BinaKodu { get; set; }
            public string DaireKodu { get; set; }
            public string UATVKodu { get; set; }

            public string Ada { get; set; }
            public string Pafta { get; set; }
            public string Parsel { get; set; }
            public string SayfaNo { get; set; }
            public string HataMesaji { get; set; }

            public string IlAdi { get; set; }
            public string IlceAdi { get; set; }
            public string BeldeAdi { get; set; }
            public string MahalleAdi { get; set; }
            public string CaddeAdi { get; set; }
            public string SokakAdi { get; set; }
            public string SiteAdi { get; set; }
            public string DaireNo { get; set; }
            public string BinaNo { get; set; }
            public string ApartmanAdi { get; set; }

            public List<SelectListItem> Iller { get; set; }
            public List<SelectListItem> Ilceler { get; set; }
            public List<SelectListItem> Beldeler { get; set; }
            public List<SelectListItem> Mahalleler { get; set; }
            public List<SelectListItem> Caddeler { get; set; }
            public List<SelectListItem> Binalar { get; set; }
            public List<SelectListItem> Daireler { get; set; }

            public nsbusiness.TURKNIPPON.DASK.PoliceBilgileri policeBilgi = new nsbusiness.TURKNIPPON.DASK.PoliceBilgileri();
        }
    }
}
