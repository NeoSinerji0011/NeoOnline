using AutoMapper;
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
using Neosinerji.BABOnlineTP.Business.HDI;
using Neosinerji.BABOnlineTP.Database.Models;
using Newtonsoft.Json;
using Neosinerji.BABOnlineTP.Business.Common.AEGON;
using Neosinerji.BABOnlineTP.Business.AEGON;


namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.KorunanGelecek)]
    public class KorunanGelecekController : TeklifController
    {
        public KorunanGelecekController(ITVMService tvmService,
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
            KorunanGelecekModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            KorunanGelecekModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public KorunanGelecekModel EkleModel(int? id, int? teklifId)
        {
            KorunanGelecekModel model = new KorunanGelecekModel();
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

                #region Korunan Gelecek

                model.GenelBilgiler = KorunanGelecekGenelBilgiler(teklifId, teklif);
                model.AnaTeminatlar = KorunanGelecekAnaTeminatlar(teklifId, teklif);
                model.EkTeminatlar = KorunanGelecekEkTeminatlar(teklifId, teklif);

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.KorunanGelecek);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme

                model.Odeme = new KorunanGelecekTeklifOdemeModel();
                model.Odeme.OdemeSekli = true;
                model.Odeme.OdemeTipi = 1;
                model.Odeme.TaksitSayilari = new List<SelectListItem>();
                model.Odeme.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", "2").ToList();

                for (int i = 2; i < 13; i++)
                {
                    model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                }

                model.KrediKarti = new KrediKartiOdemeModel();
                model.KrediKarti.KK_TeklifId = 0;
                model.KrediKarti.Tutar = 0;
                model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
                model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
                model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
                model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }
            return model;
        }
        [HttpPost]
        public ActionResult Hesapla(KorunanGelecekModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (model != null)
                {
                    //Sigortali
                    if (ModelState["Musteri.Sigortali.MusteriTipKodu"] != null)
                        ModelState["Musteri.Sigortali.MusteriTipKodu"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.KimlikNo"] != null)
                        ModelState["Musteri.Sigortali.KimlikNo"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.AdiUnvan"] != null)
                        ModelState["Musteri.Sigortali.AdiUnvan"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.SoyadiUnvan"] != null)
                        ModelState["Musteri.Sigortali.SoyadiUnvan"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.UlkeKodu"] != null)
                        ModelState["Musteri.Sigortali.UlkeKodu"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.IlKodu"] != null)
                        ModelState["Musteri.Sigortali.IlKodu"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.IlceKodu"] != null)
                        ModelState["Musteri.Sigortali.IlceKodu"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.DogumTarihi"] != null)
                        ModelState["Musteri.Sigortali.DogumTarihi"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.Email"] != null)
                        ModelState["Musteri.Sigortali.Email"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.VergiDairesi"] != null)
                        ModelState["Musteri.Sigortali.VergiDairesi"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.CepTelefonu"] != null)
                        ModelState["Musteri.Sigortali.CepTelefonu"].Errors.Clear();

                    if (ModelState["Musteri.Sigortali.AcikAdres"] != null)
                        ModelState["Musteri.Sigortali.AcikAdres"].Errors.Clear();


                    //Sİgorta ettiren

                    if (ModelState["Musteri.SigortaEttiren.MusteriTipKodu"] != null)
                        ModelState["Musteri.SigortaEttiren.MusteriTipKodu"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.KimlikNo"] != null)
                        ModelState["Musteri.SigortaEttiren.KimlikNo"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.UlkeKodu"] != null)
                        ModelState["Musteri.SigortaEttiren.UlkeKodu"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.IlKodu"] != null)
                        ModelState["Musteri.SigortaEttiren.IlKodu"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.IlceKodu"] != null)
                        ModelState["Musteri.SigortaEttiren.IlceKodu"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.Email"] != null)
                        ModelState["Musteri.SigortaEttiren.Email"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.VergiDairesi"] != null)
                        ModelState["Musteri.SigortaEttiren.VergiDairesi"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.CepTelefonu"] != null)
                        ModelState["Musteri.SigortaEttiren.CepTelefonu"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.GelirVergisiOrani"] != null)
                        ModelState["Musteri.SigortaEttiren.GelirVergisiOrani"].Errors.Clear();

                    if (ModelState["Musteri.SigortaEttiren.AcikAdres"] != null)
                        ModelState["Musteri.SigortaEttiren.AcikAdres"].Errors.Clear();

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
                    int musteriKodu = KG_MusteriKaydet(model);
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.KorunanGelecek,
                                                model.Hazirlayan.TVMKodu, _AktifKullaniciService.KullaniciKodu, musteriKodu, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Gerekli Girişler ve Hata Uyarıları


                    #endregion

                    #region Sigortali

                    teklif.AddSigortali(musteriKodu);

                    #endregion

                    #region Korunan Gelecek

                    #region Genel Bilgiler

                    teklif.AddSoru(KorunanGelecekSorular.SigortaBaslangicTarihi, model.GenelBilgiler.SigortaBaslangicTarihi);
                    teklif.AddSoru(KorunanGelecekSorular.PrimOdemeDonemi, model.GenelBilgiler.PrimOdemeDonemi.Value.ToString());
                    teklif.AddSoru(KorunanGelecekSorular.ParaBirimi, model.GenelBilgiler.ParaBirimi.Value.ToString());

                    if (model.GenelBilgiler.SigortaSuresi.HasValue)
                        teklif.AddSoru(KorunanGelecekSorular.SigortaSuresi, model.GenelBilgiler.SigortaSuresi.Value.ToString());

                    #endregion

                    #region Teminatlar

                    //ANA TEMINAT SORULAR
                    model.AnaTeminatlar.Vefat = true;
                    teklif.AddSoru(KorunanGelecekSorular.VefatTeminati, model.AnaTeminatlar.Vefat);

                    //EK TEMINAT SORULAR
                    if (model.EkTeminatlar != null && model.EkTeminatlar.MaluliyetYillikDestekTeminati)
                        teklif.AddSoru(KorunanGelecekSorular.MaluliyetYillikDestek, model.EkTeminatlar.MaluliyetYillikDestekTeminati);

                    //ANA TEMINATLAR
                    if (model.AnaTeminatlar.Vefat && model.AnaTeminatlar.VefatBedeli.HasValue)
                        teklif.AddTeminat(KorunanGelecekTeminatlar.Vefat, model.AnaTeminatlar.VefatBedeli.Value, 0, 0, 0, 0);

                    //EK TEMINATLAR
                    if (model.EkTeminatlar != null && model.EkTeminatlar.MaluliyetYillikDestekTeminati)
                    {
                        var MaluliyetYillikDestekBedel = Convert.ToInt32(model.AnaTeminatlar.VefatBedeli.Value) / Convert.ToInt32(model.GenelBilgiler.SigortaSuresi);
                        teklif.AddTeminat(KorunanGelecekTeminatlar.MaluliyetYillikDestek, Math.Round(Convert.ToDecimal(MaluliyetYillikDestekBedel), 2), 0, 0, 0, 0);
                    }

                    #endregion

                    #endregion

                    #region Teklif return

                    IKorunanGelecekTeklif korunanGelecek = new KorunanGelecekTeklif();

                    korunanGelecek.AddUretimMerkezi(TeklifUretimMerkezleri.AEGON);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                    korunanGelecek.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    IsDurum isDurum = korunanGelecek.Hesapla(teklif);

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

        public ActionResult Detay(int id)
        {
            DetayKorunanGelecekModel model = new DetayKorunanGelecekModel();

            #region Teklif Genel

            KorunanGelecekTeklif korunanGelecekTeklif = new KorunanGelecekTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = korunanGelecekTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(korunanGelecekTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.AegonDetayMusteriModel(korunanGelecekTeklif.Teklif.SigortaEttiren.MusteriKodu);

            //// ====Sigortali varsa Ekleniyor ==== // 
            //if (korunanGelecekTeklif.Teklif.Sigortalilar.Count > 0 &&
            //   (korunanGelecekTeklif.Teklif.SigortaEttiren.MusteriKodu != korunanGelecekTeklif.Teklif.Sigortalilar.First().MusteriKodu))
            //    model.Sigortali = base.DetayMusteriModel(korunanGelecekTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region KorunanGelecek
            model.GenelBilgiler = KorunanGelecekGenelBilgilerDetay(korunanGelecekTeklif.Teklif);
            model.AnaTeminatlar = KorunanGelecekAnaTeminatlarDetay(korunanGelecekTeklif.Teklif);
            model.EkTeminatlar = KorunanGelecekEkTeminatlarDetay(korunanGelecekTeklif.Teklif);
            #endregion

            #region Teklif Fiyat

            model.Fiyat = KorunanGelecekFiyat(korunanGelecekTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

            #endregion

            #region Provizyon

            model.OnProvizyon = _TeklifService.AegonOnProvizyonKontrol(id);

            #endregion

            return View(model);
        }

        private TeklifFiyatModel KorunanGelecekFiyat(KorunanGelecekTeklif korunanGelecekTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = korunanGelecekTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = korunanGelecekTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = korunanGelecekTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = korunanGelecekTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = korunanGelecekTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = korunanGelecekTeklif.GetIsDurum();
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
        public ActionResult OdemeAl(OdemeKorunanGelecekModel model)
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

        //EKLE 
        private KorunanGelecekGenelBilgiler KorunanGelecekGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            KorunanGelecekGenelBilgiler model = new KorunanGelecekGenelBilgiler();

            try
            {
                model.ParaBirimleri = new SelectList(TeklifProvider.ParaBirimleriAEGON(), "Value", "Text", "");
                model.PrimDonemleri = new SelectList(TeklifProvider.PrimDonemleriAEGON(), "Value", "Text", "");


                if (teklifId.HasValue && teklif != null)
                {
                    //Sigorta Başlangıç Tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(KorunanGelecekSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Sigorta süresi (yıl)
                    string sigortaSuresi = teklif.ReadSoru(KorunanGelecekSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.SigortaSuresi = Convert.ToByte(sigortaSuresi);

                    //Prim Ödeme Dönemi 
                    string primDonemi = teklif.ReadSoru(KorunanGelecekSorular.PrimOdemeDonemi, String.Empty);
                    if (!String.IsNullOrEmpty(primDonemi))
                        model.PrimOdemeDonemi = Convert.ToByte(primDonemi);

                    //Para Brimi 
                    string paraBirimi = teklif.ReadSoru(KorunanGelecekSorular.ParaBirimi, String.Empty);
                    if (!String.IsNullOrEmpty(paraBirimi))
                        model.ParaBirimi = Convert.ToByte(paraBirimi);




                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private KorunanGelecekAnaTeminatlar KorunanGelecekAnaTeminatlar(int? teklifId, ITeklif teklif)
        {
            KorunanGelecekAnaTeminatlar model = new KorunanGelecekAnaTeminatlar();

            try
            {
                model.Vefat = true;

                if (teklifId.HasValue && teklif != null)
                {
                    //Zorunlu Teminat
                    model.Vefat = true;
                    if (teklif.ReadSoru(KorunanGelecekSorular.VefatTeminati, false))
                    {
                        TeklifTeminat VefatTeminati = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KorunanGelecekTeminatlar.Vefat);
                        if (VefatTeminati != null)
                        {
                            model.Vefat = true;
                            if (VefatTeminati.TeminatBedeli.HasValue)
                                model.VefatBedeli = VefatTeminati.TeminatBedeli;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private KorunanGelecekEkTeminatlar KorunanGelecekEkTeminatlar(int? teklifId, ITeklif teklif)
        {
            KorunanGelecekEkTeminatlar model = new KorunanGelecekEkTeminatlar();

            try
            {
                if (teklifId.HasValue && teklif != null)
                {
                    if (teklif.ReadSoru(KorunanGelecekTeminatlar.MaluliyetYillikDestek, false))
                    {
                        TeklifTeminat MaluliyetYilliDestekTeminati = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KorunanGelecekTeminatlar.MaluliyetYillikDestek);
                        if (MaluliyetYilliDestekTeminati != null)
                        {
                            model.MaluliyetYillikDestekTeminati = true;
                            if (MaluliyetYilliDestekTeminati.TeminatBedeli.HasValue)
                                model.MaluliyetYillikDestekTeminatiBedeli = (int)MaluliyetYilliDestekTeminati.TeminatBedeli;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        //DETAY

        private KorunanGelecekGenelBilgiler KorunanGelecekGenelBilgilerDetay(ITeklif teklif)
        {
            KorunanGelecekGenelBilgiler model = new KorunanGelecekGenelBilgiler();

            try
            {
                if (teklif != null)
                {
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(OdemeGuvenceSorular.SigortaBaslangicTarihi, DateTime.MinValue);
                    model.ParaBirimiText = TESabitPrimliParaBirimi.ParaBirimiText(teklif.ReadSoru(TESabitPrimliSorular.ParaBirimi, String.Empty));

                    string sigortaSuresi = teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.SigortaSuresi = Convert.ToByte(sigortaSuresi);

                    //Prim Ödeme Dönemi 
                    switch (teklif.ReadSoru(OdemeGuvenceSorular.PrimOdemeDonemi, String.Empty))
                    {
                        case "1": model.PrimOdemeDonemiText = "Aylık"; break;
                        case "2": model.PrimOdemeDonemiText = "3 Aylık"; break;
                        case "3": model.PrimOdemeDonemiText = "6 Aylık"; break;
                        case "4": model.PrimOdemeDonemiText = "Yıllık"; break;
                    }


                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private KorunanGelecekAnaTeminatlar KorunanGelecekAnaTeminatlarDetay(ITeklif teklif)
        {
            KorunanGelecekAnaTeminatlar model = new KorunanGelecekAnaTeminatlar();
            try
            {
                model.Vefat = true;

                if (teklif != null)
                {
                    //Zorunlu   teminatlar
                    model.Vefat = true;

                    if (teklif.ReadSoru(KorunanGelecekSorular.VefatTeminati, false))
                    {
                        TeklifTeminat VefatTeminati = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KorunanGelecekTeminatlar.Vefat);
                        if (VefatTeminati != null)
                        {
                            model.Vefat = true;
                            if (VefatTeminati.TeminatBedeli.HasValue)
                                model.VefatBedeli = VefatTeminati.TeminatBedeli;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private KorunanGelecekEkTeminatlar KorunanGelecekEkTeminatlarDetay(ITeklif teklif)
        {
            KorunanGelecekEkTeminatlar model = new KorunanGelecekEkTeminatlar();
            AegonKGResponse response = new AegonKGResponse();
            try
            {
                if (teklif != null)
                {
                    if (teklif.ReadSoru(KorunanGelecekSorular.MaluliyetYillikDestek, false))
                    {
                        TeklifTeminat MaluliyetYillikDestekTeminati = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KorunanGelecekTeminatlar.MaluliyetYillikDestek);
                        if (MaluliyetYillikDestekTeminati != null)
                        {
                            model.MaluliyetYillikDestekTeminati = true;
                            if (MaluliyetYillikDestekTeminati.TeminatBedeli.HasValue)
                                model.MaluliyetYillikDestekTeminatiBedeli = (int)MaluliyetYillikDestekTeminati.TeminatBedeli.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private int KG_MusteriKaydet(KorunanGelecekModel model)
        {
            if (model != null && model.Musteri != null)
            {
                MusteriModel musteri = model.Musteri.SigortaEttiren;

                MusteriGenelBilgiler KayitliMusteri = _MusteriService.GetMusteri(musteri.KimlikNo, _AktifKullaniciService.TVMKodu);

                if (KayitliMusteri == null)
                {
                    //Müşteri Yeni Ekleniyor.
                    KayitliMusteri = new MusteriGenelBilgiler();
                    KayitliMusteri.MusteriTipKodu = MusteriTipleri.TCMusteri;
                    KayitliMusteri.TVMKodu = _AktifKullaniciService.TVMKodu;
                    KayitliMusteri.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;

                    if (!String.IsNullOrEmpty(musteri.KimlikNo))
                        KayitliMusteri.KimlikNo = musteri.KimlikNo;
                    else
                        KayitliMusteri.KimlikNo = "1AEGONAEGON";

                    KayitliMusteri.AdiUnvan = musteri.AdiUnvan;
                    KayitliMusteri.SoyadiUnvan = musteri.SoyadiUnvan;
                    KayitliMusteri.DogumTarihi = musteri.DogumTarihi;
                    KayitliMusteri.EMail = musteri.Email;
                    KayitliMusteri.Uyruk = musteri.Uyruk;
                    KayitliMusteri.Cinsiyet = musteri.Cinsiyet;

                    //GElir vergisi oranı musteride tutuluyor
                    KayitliMusteri.CiroBilgisi = musteri.GelirVergisiOrani.HasValue ? musteri.GelirVergisiOrani.Value.ToString() : "5";

                    if (!String.IsNullOrEmpty(musteri.CepTelefonu))
                    {
                        MusteriTelefon cep = new MusteriTelefon();
                        cep.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                        cep.Numara = musteri.CepTelefonu;
                        cep.NumaraSahibi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;
                        cep.SiraNo = 1;
                        KayitliMusteri.MusteriTelefons.Add(cep);
                    }

                    if (!String.IsNullOrEmpty(musteri.IlKodu))
                    {
                        MusteriAdre adres = new MusteriAdre();
                        adres.AdresTipi = AdresTipleri.Ev;
                        adres.UlkeKodu = "TUR";
                        adres.SiraNo = 1;
                        adres.IlKodu = musteri.IlKodu;
                        adres.IlceKodu = musteri.IlceKodu;
                        adres.Varsayilan = true;
                        adres.Mahalle = "";
                        adres.Cadde = "";
                        adres.Sokak = "";
                        adres.Apartman = "";
                        adres.Mahalle = "";
                        adres.PostaKodu = 0;
                        adres.BinaNo = "";
                        adres.DaireNo = "";

                        if (musteri.IlceKodu.HasValue)
                        {
                            string ilceAdi = _UlkeService.GetIlceAdi(musteri.IlceKodu.Value);
                            if (!String.IsNullOrEmpty(ilceAdi))
                                adres.Adres = ilceAdi + " ";
                        }

                        string ilAdi = _UlkeService.GetIlAdi("TUR", musteri.IlKodu);
                        if (!String.IsNullOrEmpty(ilAdi))
                            adres.Adres += ilAdi;

                        KayitliMusteri.MusteriAdres.Add(adres);
                    }

                    KayitliMusteri = _MusteriService.CreateMusteri(KayitliMusteri);
                }
                else
                {
                    KayitliMusteri.AdiUnvan = musteri.AdiUnvan;
                    KayitliMusteri.SoyadiUnvan = musteri.SoyadiUnvan;
                    KayitliMusteri.DogumTarihi = musteri.DogumTarihi;
                    KayitliMusteri.Cinsiyet = musteri.Cinsiyet;
                    KayitliMusteri.EMail = musteri.Email;

                    //GElir vergisi oranı musteride tutuluyor
                    KayitliMusteri.CiroBilgisi = musteri.GelirVergisiOrani.HasValue ? musteri.GelirVergisiOrani.Value.ToString() : "5";

                    if (!String.IsNullOrEmpty(musteri.CepTelefonu))
                    {
                        MusteriTelefon telefon = KayitliMusteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                        if (telefon != null)
                        {
                            telefon.Numara = musteri.CepTelefonu;
                        }
                        else
                        {
                            MusteriTelefon cepTel = new MusteriTelefon();
                            cepTel.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                            cepTel.Numara = musteri.CepTelefonu;
                            cepTel.NumaraSahibi = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                            if (KayitliMusteri.MusteriTelefons != null)
                            {
                                int sirano = KayitliMusteri.MusteriTelefons.Max(s => s.SiraNo);
                                cepTel.SiraNo = sirano + 1;
                                KayitliMusteri.MusteriTelefons.Add(cepTel);
                            }
                        }
                    }

                    _MusteriService.UpdateMusteri(KayitliMusteri);
                }
                return KayitliMusteri.MusteriKodu;
            }
            return 0;
        }

        [HttpPost]
        [AjaxException]
        public ActionResult _DetayPartial(int IsDurum_id)
        {
            KGDetayPartialModel model = new KGDetayPartialModel();
            AegonKGResponse response = new AegonKGResponse();
            try
            {
                IsDurum isDurum = _TeklifService.GetIsDurumu(IsDurum_id);
                if (isDurum != null)
                {
                    ITeklif teklif = _TeklifService.GetTeklif(isDurum.ReferansId);
                    if (teklif != null)
                    {
                        MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(teklif.SigortaEttiren.MusteriKodu);
                        if (musteri != null)
                            model.AdSoyad = musteri.AdiUnvan + " " + musteri.SoyadiUnvan;

                        model.SigortaBaslangicTarihi = teklif.ReadSoru(KorunanGelecekSorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");
                        model.SigortaSuresi = teklif.ReadSoru(KorunanGelecekSorular.SigortaSuresi, String.Empty);
                        model.PrimOdemeDonemi = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(KorunanGelecekSorular.PrimOdemeDonemi, String.Empty));

                        string paraBirimi = "";

                        switch (teklif.ReadSoru(KorunanGelecekSorular.ParaBirimi, String.Empty))
                        {
                            case "1": paraBirimi = "€"; break;
                            case "2": paraBirimi = "$"; break;
                        }

                        model.ParaBirimi = TESabitPrimliParaBirimi.ParaBirimiText(teklif.ReadSoru(KorunanGelecekSorular.ParaBirimi, String.Empty));
                        TeklifTeminat Vefat = teklif.Teminatlar.Where(s => s.TeminatKodu == KorunanGelecekTeminatlar.Vefat).FirstOrDefault();
                        model.AnaTeminat = true;
                        if (Vefat != null && Vefat.TeminatBedeli.HasValue)
                            model.AnaTeminatTutar = Vefat.TeminatBedeli.Value.ToString("N2").Replace(",", " ").Replace(".", ",").Replace(" ", ".") + " " + paraBirimi;

                        TeklifTeminat Maluliyet = teklif.Teminatlar.Where(s => s.TeminatKodu == KorunanGelecekTeminatlar.MaluliyetYillikDestek).FirstOrDefault();
                        if (Maluliyet != null && Maluliyet.TeminatBedeli.HasValue)
                            model.MaluliyetYillikDestekTutar = Maluliyet.TeminatBedeli.Value.ToString("N2").Replace(",", " ").Replace(".", ",").Replace(" ", ".") + " " + paraBirimi;

                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return PartialView(model);
        }

        [HttpPost]
        [AjaxException]
        public JsonResult SigortaSuresiHesapla(DateTime dogumTarihi, DateTime sigortaBaslangic, int sure)
        {
            if (sigortaBaslangic < DateTime.Today)
                return Json(new { Success = "false", Message = "Sigorta başlangıç tarihi olarak geçmiş bir tarih girilemez" });

            int yas = AEGONTESabitPrimli.AegonYasHesapla(dogumTarihi, sigortaBaslangic);

            if (sure < 10 || sure > 38)
                return Json(new { Success = "false", Message = "Sigorta süresi 10 – 38 sene arasında olmalıdır" });
            else
                return Json(new { Success = "true", Message = "" });
        }

        public string SigortaliYasHesapla(DateTime DogTar, DateTime SigBas, int SigSure, bool AnaTeminat, bool Maluliyet)
        {
            string message = String.Empty;

            int yas = 0;
            int sigortalanabilirYas = 0;

            if (DogTar != null && SigBas != null)
            {
                yas = AEGONTESabitPrimli.AegonYasHesapla(DogTar, SigBas);
                if (SigSure > 0)
                {
                    sigortalanabilirYas = yas + SigSure;
                }
            }

            if (yas > 17)
            {
                if (AnaTeminat && sigortalanabilirYas > KorunanGelecekSabitleri.VefatMaxSigortalanabilirYas)
                {
                    message += "<p>Vefat Teminatı için,Sigortalı yaşı + Sigorta süresi " + KorunanGelecekSabitleri.VefatMaxSigortalanabilirYas + "' ten büyük olamaz. </p>";
                }

                if (Maluliyet && yas > KorunanGelecekSabitleri.MaluliyetMaxGirisYasi && sigortalanabilirYas > KorunanGelecekSabitleri.MaluliyetMaxSigortalanabilirYas)
                {
                    message += "<p>Maluliyet Yıllık Destek Teminatı için, Sigortalı yaşı + Sigorta süresi  " + KorunanGelecekSabitleri.MaluliyetMaxSigortalanabilirYas + " ' ten büyük olamaz.</p>";
                }
            }
            else
            {
                message = "<p>Sigortalı yaşı 18 den küçük olamaz</p>";
            }

            return message;
        }

        [HttpPost]
        [AjaxException]
        public JsonResult GetVefatTeminatTutariLimit()
        {
            return Json(new { dolar = 10000, avro = 7500 }, JsonRequestBehavior.AllowGet);
        }
    }
}
