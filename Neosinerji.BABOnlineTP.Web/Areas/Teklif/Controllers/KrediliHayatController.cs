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
using Neosinerji.BABOnlineTP.Business.DEMIR;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.KrediHayat)]
    public class KrediliHayatController : TeklifController
    {
        public KrediliHayatController(ITVMService tvmService,
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
            KrediliHayatModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            KrediliHayatModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public KrediliHayatModel EkleModel(int? id, int? teklifId)
        {

            ILogService log = DependencyResolver.Current.GetService<ILogService>();
            log.Visit();

            ITeklif teklif = null;
            KrediliHayatModel model = new KrediliHayatModel();

            #region Sigortalı

            //Teklifi hazırlayan
            model.Hazirlayan = base.EkleHazirlayanModel();

            //Sigorta Ettiren / Sigortalı
            model.Musteri = new SigortaliModel();
            model.Musteri.SadeceSigortaliGoster = true;
            model.Musteri.SigortaliAyni = true;

            if (teklifId.HasValue)
            {
                model.TekrarTeklif = true;
                teklif = _TeklifService.GetTeklif(teklifId.Value);
                id = teklif.SigortaEttiren.MusteriKodu;
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

            #region Kredi Bilgileri

            model.Kredi = new KrediBilgileriModel();
            model.Kredi.Sureler = KrediliHayatProvider.GetKrediSureleri();
            model.Kredi.KrediTurleri = KrediliHayatProvider.GetKrediliTurleri();

            #endregion

            #region Kredi Adres Bilgileri
            model.Adres = new KrediAlanAdresModel();
            model.Adres.UlkeKodu = "TUR";
            model.Adres.AdresTipleri = nsmusteri.MusteriListProvider.AdresTipleri();

            if (id.HasValue)
            {
                model.Adres.AdresTipi = model.Musteri.SigortaEttiren.AdresTipi;
                model.Adres.UlkeKodu = model.Musteri.SigortaEttiren.UlkeKodu;
                model.Adres.IlKodu = model.Musteri.SigortaEttiren.IlKodu;
                model.Adres.IlceKodu = model.Musteri.SigortaEttiren.IlceKodu;
                model.Adres.Semt = model.Musteri.SigortaEttiren.Semt;
                model.Adres.Mahalle = model.Musteri.SigortaEttiren.Mahalle;
                model.Adres.Cadde = model.Musteri.SigortaEttiren.Cadde;
                model.Adres.Sokak = model.Musteri.SigortaEttiren.Sokak;
                model.Adres.Apartman = model.Musteri.SigortaEttiren.Apartman;
                model.Adres.BinaNo = model.Musteri.SigortaEttiren.BinaNo;
                model.Adres.DaireNo = model.Musteri.SigortaEttiren.DaireNo;
                model.Adres.PostaKodu = model.Musteri.SigortaEttiren.PostaKodu;

                model.Adres.Iller = new SelectList(_UlkeService.GetIlList(model.Adres.UlkeKodu), "IlKodu", "IlAdi", model.Adres.IlKodu).ListWithOptionLabelIller();
                model.Adres.Ilceler = new SelectList(_UlkeService.GetIlceList(model.Adres.UlkeKodu, model.Adres.IlKodu), "IlceKodu", "IlceAdi").ListWithOptionLabel();
            }
            else
            {
                model.Adres.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", "34").ListWithOptionLabelIller();
                model.Adres.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", "34"), "IlceKodu", "IlceAdi").ListWithOptionLabel();
            }
            #endregion

            #region Teklif Üretim Merkezleri
            model.TeklifUM = new TeklifUMListeModel();
            List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.KrediHayat);
            foreach (var item in urunyetkileri)
                model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);
            #endregion

            #region Kredi Kartı Ödeme
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

            return model;
        }

        [HttpPost]
        public ActionResult Hesapla(KrediliHayatModel model)
        {
            #region Geçerliliği kontrol edilmeyecek alanlar
            TryValidateModel(model);

            if (model != null)
            {
                ModelStateMusteriClear(ModelState, model.Musteri);
            }
            #endregion

            #region Teklif kaydı ve hesaplamanın başlatılması
            if (ModelState.IsValid)
            {
                try
                {
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.KrediHayat,
                                                                            model.Hazirlayan.TVMKodu,
                                                                            model.Hazirlayan.TVMKullaniciKodu,
                                                                            model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Sigortali
                    teklif.AddSigortali(model.Musteri.SigortaEttiren.MusteriKodu.Value);
                    MusteriGuncelle(model.Musteri.SigortaEttiren);
                    #endregion

                    #region Sorular
                    teklif.AddSoru(KrediliHayatSorular.Baba_Adi, model.Kredi.BabaAdi);

                    if (model.Kredi.BaslangicTarihi.HasValue)
                        teklif.AddSoru(KrediliHayatSorular.Kredi_Baslangic_Tarihi, model.Kredi.BaslangicTarihi.Value);

                    if (model.Kredi.Tutar.HasValue)
                        teklif.AddSoru(KrediliHayatSorular.Kredi_Tutari, model.Kredi.Tutar.Value);

                    teklif.AddSoru(KrediliHayatSorular.Kredi_Turu, model.Kredi.KrediTuru.ToString());
                    teklif.AddSoru(KrediliHayatSorular.Kredi_Suresi, model.Kredi.KrediSuresi.ToString());

                    #endregion

                    #region Sigortalı Adres

                    MusteriAdre adres = _MusteriService.GetMusteriAdresleri(model.Musteri.SigortaEttiren.MusteriKodu.Value).FirstOrDefault(f => f.Varsayilan == true);

                    if (adres != null)
                    {
                        adres.AdresTipi = model.Adres.AdresTipi;
                        adres.IlKodu = model.Adres.IlKodu;
                        adres.IlceKodu = model.Adres.IlceKodu;
                        adres.Semt = model.Adres.Semt;
                        adres.Mahalle = model.Adres.Mahalle;
                        adres.Cadde = model.Adres.Cadde;
                        adres.Sokak = model.Adres.Sokak;
                        adres.Apartman = model.Adres.Apartman;
                        adres.PostaKodu = model.Adres.PostaKodu;
                        adres.BinaNo = model.Adres.BinaNo;
                        adres.DaireNo = model.Adres.DaireNo;

                        _MusteriService.UpdateAdres(adres);
                    }

                    #endregion

                    IKrediliHayatTeklif krediHayatTeklif = new KrediliHayatTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    krediHayatTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.DEMIR);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;
                    teklif.GenelBilgiler.TaksitSayisi = 1;
                    krediHayatTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    IsDurum isDurum = krediHayatTeklif.Hesapla(teklif);

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

            foreach (var key in ModelState.Keys)
            {
                ModelState state = ModelState[key];
                foreach (var error in state.Errors)
                {
                    _LogService.Warning("Validation key : {0}, error : {1}", key, error.ErrorMessage);
                }
            }

            ModelState.TraceErros();

            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }

        public ActionResult Detay(int id)
        {

            DetayKrediliHayatModel model = new DetayKrediliHayatModel();
            KrediliHayatTeklif teklif = new KrediliHayatTeklif(id);

            model.TeklifId = id;
            model.TeklifNo = teklif.Teklif.TeklifNo.ToString();

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(teklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(teklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali Ekleniyor ==== //
            model.Sigortali = base.DetayMusteriModel(teklif.Teklif.Sigortalilar.First().MusteriKodu);

            //Kredi Bilgileri
            model.Kredi = new DetayKrediBilgileriModel();
            model.Kredi.BabaAdi = teklif.Teklif.ReadSoru(KrediliHayatSorular.Baba_Adi, String.Empty);
            model.Kredi.BaslangicTarihi = teklif.Teklif.ReadSoru(KrediliHayatSorular.Kredi_Baslangic_Tarihi, DateTime.MinValue);
            model.Kredi.Tutar = teklif.Teklif.ReadSoru(KrediliHayatSorular.Kredi_Tutari, decimal.Zero);

            string krediTuru = teklif.Teklif.ReadSoru(KrediliHayatSorular.Kredi_Turu, String.Empty);
            string krediSuresi = teklif.Teklif.ReadSoru(KrediliHayatSorular.Kredi_Suresi, String.Empty);

            model.Kredi.KrediSuresi = Convert.ToInt32(krediSuresi);
            model.Kredi.KrediTuruText = KrediliHayatProvider.GetKrediTuruText(Convert.ToInt32(krediTuru));

            //Adres bilgileri
            model.Adres = new DetayKrediAlanAdresModel();
            MusteriAdre adres = _MusteriService.GetMusteriAdresleri(teklif.Teklif.SigortaEttiren.MusteriKodu).FirstOrDefault(f => f.Varsayilan == true);

            if (adres != null)
            {
                model.Adres.IlAdi = _UlkeService.GetIlAdi("TUR", adres.IlKodu);
                if (adres.IlceKodu.HasValue)
                    model.Adres.IlceAdi = _UlkeService.GetIlceAdi(adres.IlceKodu.Value);
                model.Adres.Semt = adres.Semt;
                model.Adres.Mahalle = adres.Mahalle;
                model.Adres.Cadde = adres.Cadde;
                model.Adres.Sokak = adres.Sokak;
                model.Adres.Apartman = adres.Apartman;
                model.Adres.PostaKodu = adres.PostaKodu;
                model.Adres.BinaNo = adres.BinaNo;
                model.Adres.DaireNo = adres.DaireNo;
            }

            //Teklif fiyat bilgisi
            model.Fiyat = KrediliHayatFiyat(teklif);

            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            model.KrediKarti.KK_OdemeSekli = teklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            model.KrediKarti.KK_OdemeTipi = teklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = teklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
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


            return View(model);
        }

        [HttpPost]
        public ActionResult OdemeAl(OdemeTrafikModel model)
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

        public ActionResult Police(int id)
        {
            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            DetayKrediliHayatModel model = new DetayKrediliHayatModel();

            model.TeklifId = id;
            model.TeklifNo = teklif.TeklifNo.ToString();

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali Ekleniyor ==== //
            model.Sigortali = base.DetayMusteriModel(teklif.Sigortalilar.First().MusteriKodu);

            //Kredi Bilgileri
            model.Kredi = new DetayKrediBilgileriModel();
            model.Kredi.BabaAdi = anaTeklif.ReadSoru(KrediliHayatSorular.Baba_Adi, String.Empty);
            model.Kredi.BaslangicTarihi = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Baslangic_Tarihi, DateTime.MinValue);
            model.Kredi.Tutar = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Tutari, decimal.Zero);

            string krediTuru = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Turu, String.Empty);
            string krediSuresi = anaTeklif.ReadSoru(KrediliHayatSorular.Kredi_Suresi, String.Empty);

            model.Kredi.KrediSuresi = Convert.ToInt32(krediSuresi);
            model.Kredi.KrediTuruText = KrediliHayatProvider.GetKrediTuruText(Convert.ToInt32(krediTuru));

            //Adres bilgileri
            model.Adres = new DetayKrediAlanAdresModel();
            MusteriAdre adres = _MusteriService.GetMusteriAdresleri(teklif.SigortaEttiren.MusteriKodu).FirstOrDefault(f => f.Varsayilan == true);

            if (adres != null)
            {
                model.Adres.IlAdi = _UlkeService.GetIlAdi("TUR", adres.IlKodu);
                if (adres.IlceKodu.HasValue)
                    model.Adres.IlceAdi = _UlkeService.GetIlceAdi(adres.IlceKodu.Value);
                model.Adres.Semt = adres.Semt;
                model.Adres.Mahalle = adres.Mahalle;
                model.Adres.Cadde = adres.Cadde;
                model.Adres.Sokak = adres.Sokak;
                model.Adres.Apartman = adres.Apartman;
                model.Adres.PostaKodu = adres.PostaKodu;
                model.Adres.BinaNo = adres.BinaNo;
                model.Adres.DaireNo = adres.DaireNo;
            }

            //Teklif fiyat bilgisi
            KrediliHayatTeklif krediHayatTeklif = new KrediliHayatTeklif(id);
            model.Fiyat = KrediliHayatFiyat(krediHayatTeklif);

            model.OdemeBilgileri = new KrediliHayatOdemeBilgileriModel();
            model.OdemeBilgileri.teklifId = id;
            model.OdemeBilgileri.NetPrim = teklif.GenelBilgiler.NetPrim;
            model.OdemeBilgileri.BrutPrim = teklif.GenelBilgiler.BrutPrim;

            TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);

            model.OdemeBilgileri.TUMKodu = tum.Kodu;
            model.OdemeBilgileri.TUMUnvani = tum.Unvani;
            model.OdemeBilgileri.TUMLogoURL = tum.Logo;
            model.OdemeBilgileri.PoliceURL = teklif.GenelBilgiler.PDFPolice;
            model.OdemeBilgileri.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;

            return View(model);
        }

        private TeklifFiyatModel KrediliHayatFiyat(KrediliHayatTeklif krediHayatTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = krediHayatTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = krediHayatTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = krediHayatTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = krediHayatTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = krediHayatTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(krediHayatTeklif.Teklif.TeklifNo, krediHayatTeklif.Teklif.GenelBilgiler.TVMKodu);

            KrediliHayatTeklif teklif = new KrediliHayatTeklif(anaTeklif.GenelBilgiler.TeklifId);
            IsDurum durum = teklif.GetIsDurum();
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
    }
}
