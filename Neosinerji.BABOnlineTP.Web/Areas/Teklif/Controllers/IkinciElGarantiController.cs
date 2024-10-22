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

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.IkinciElGaranti)]
    public class IkinciElGarantiController : TeklifController
    {
        public IkinciElGarantiController(ITVMService tvmService,
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
            IkinciElGarantiModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            IkinciElGarantiModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public IkinciElGarantiModel EkleModel(int? id, int? teklifId)
        {
            IkinciElGarantiModel model = new IkinciElGarantiModel();
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

                #region IkinciElGaranti

                model.GenelBilgiler = IkinciElGarantiGenelBilgiler(teklifId, teklif);

                #endregion

                #region Arac

                //Araç bilgileri
                model.Arac = base.EkleAracModel();
                model.Arac.TescilIl = "34";
                model.Arac.TescilIller = new SelectList(_CRService.GetTescilIlList(), "Key", "Value", model.Arac.TescilIl).ListWithOptionLabel();
                model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value", "").ListWithOptionLabel(); ;
                model.Arac.PoliceBaslangicTarihi = TurkeyDateTime.Today;

                if (teklifId.HasValue)
                {
                    model.Arac.PlakaKodu = teklif.Arac.PlakaKodu;
                    model.Arac.PlakaNo = teklif.Arac.PlakaNo;
                    model.Arac.KullanimSekliKodu = teklif.Arac.KullanimSekli;
                    short kullanimSekliKodu = Convert.ToInt16(teklif.Arac.KullanimSekli);

                    string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                    string kullanimTarziKodu = String.Empty;
                    if (parts.Length == 2)
                    {
                        kullanimTarziKodu = parts[0];
                        model.Arac.KullanimTarziKodu = teklif.Arac.KullanimTarzi;

                        List<AracKullanimTarziServisModel> tarzlar = _AracService.GetAracKullanimTarziTeklif(kullanimSekliKodu);
                        model.Arac.KullanimTarzlari = new SelectList(tarzlar, "Kod", "KullanimTarzi").ListWithOptionLabel();
                    }

                    model.Arac.TescilIl = teklif.Arac.TescilIlKodu;
                    model.Arac.TescilIlce = teklif.Arac.TescilIlceKodu;
                    model.Arac.TescilIlceler = new SelectList(_CRService.GetTescilIlceList(model.Arac.TescilIl), "Key", "Value").ListWithOptionLabel();

                    model.Arac.MarkaKodu = teklif.Arac.Marka;
                    List<AracMarka> markalar = _AracService.GetAracMarkaList(kullanimTarziKodu);
                    model.Arac.Markalar = new SelectList(markalar, "MarkaKodu", "MarkaAdi").ListWithOptionLabel();

                    model.Arac.Model = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : 0;

                    model.Arac.TipKodu = teklif.Arac.AracinTipi;

                    List<AracTip> tipler = _AracService.GetAracTipList(kullanimTarziKodu, model.Arac.MarkaKodu, model.Arac.Model);
                    model.Arac.AracTipleri = new SelectList(tipler, "TipKodu", "TipAdi").ListWithOptionLabel();

                    model.Arac.MotorNo = teklif.Arac.MotorNo;
                    model.Arac.SaseNo = teklif.Arac.SasiNo;

                    if (teklif.Arac.TrafikTescilTarihi.HasValue)
                        model.Arac.TrafikTescilTarihi = teklif.Arac.TrafikTescilTarihi.Value;

                    if (teklif.Arac.TrafikCikisTarihi.HasValue)
                        model.Arac.TrafigeCikisTarihi = teklif.Arac.TrafikCikisTarihi.Value;

                    model.Arac.PoliceBaslangicTarihi = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, DateTime.MinValue);

                    model.Arac.TescilBelgeSeriKod = teklif.Arac.TescilSeriKod;
                    model.Arac.TescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
                    model.Arac.AsbisNo = teklif.Arac.AsbisNo;
                }

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.IkinciElGaranti);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme

                model.Odeme = new IkinciElGarantiTeklifOdemeModel();
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

        [HttpPost]
        public ActionResult Hesapla(IkinciElGarantiModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (model != null)
                {
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
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.IkinciElGaranti, model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Seyehat Sağlık Bilgileri

                    teklif.AddSoru(IkinciElGarantiSorular.ModelYili, model.GenelBilgiler.Model.ToString());
                    teklif.AddSoru(IkinciElGarantiSorular.PoliceSuresi, model.GenelBilgiler.PoliceSuresi.ToString());
                    teklif.AddSoru(IkinciElGarantiSorular.SilindirHacmi, model.GenelBilgiler.SilindirHacmi.ToString());
                    teklif.AddSoru(IkinciElGarantiSorular.TeminatTuru, model.GenelBilgiler.TeminatTuru.ToString());

                    //Araç Bilgileri
                    teklif.Arac.PlakaKodu = model.Arac.PlakaKodu;
                    teklif.Arac.PlakaNo = model.Arac.PlakaNo.ToUpperInvariant();
                    teklif.Arac.Marka = model.Arac.MarkaKodu;
                    teklif.Arac.AracinTipi = model.Arac.TipKodu;
                    teklif.Arac.Model = model.Arac.Model;
                    teklif.Arac.MotorNo = model.Arac.MotorNo;
                    teklif.Arac.SasiNo = model.Arac.SaseNo;
                    teklif.Arac.KullanimSekli = model.Arac.KullanimSekliKodu;
                    teklif.Arac.KullanimTarzi = model.Arac.KullanimTarziKodu;
                    teklif.Arac.TrafikTescilTarihi = model.Arac.TrafikTescilTarihi;
                    teklif.Arac.TrafikCikisTarihi = model.Arac.TrafigeCikisTarihi;
                    teklif.Arac.TescilSeriKod = model.Arac.TescilBelgeSeriKod;
                    teklif.Arac.TescilSeriNo = model.Arac.TescilBelgeSeriNo;
                    teklif.Arac.AsbisNo = model.Arac.AsbisNo;
                    teklif.Arac.TescilIlKodu = model.Arac.TescilIl;
                    teklif.Arac.TescilIlceKodu = model.Arac.TescilIlce;

                    #endregion

                    #region Teklif return

                    IIkinciElGarantiTeklif ikinciElGarantiTeklif = new IkinciElGarantiTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    foreach (var item in model.TeklifUM)
                    {
                        if (item.TeklifAl)
                            ikinciElGarantiTeklif.AddUretimMerkezi(item.TUMKodu);
                    }

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.Odeme.OdemeTipi;

                    if (!model.Odeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                        ikinciElGarantiTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        ikinciElGarantiTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = ikinciElGarantiTeklif.Hesapla(teklif);
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
            DetayIkinciElGarantiModel model = new DetayIkinciElGarantiModel();

            #region Teklif Genel


            IkinciElGarantiTeklif ikinciElGarantiTeklif = new IkinciElGarantiTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = ikinciElGarantiTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(ikinciElGarantiTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(ikinciElGarantiTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (ikinciElGarantiTeklif.Teklif.Sigortalilar.Count > 0 &&
               (ikinciElGarantiTeklif.Teklif.SigortaEttiren.MusteriKodu != ikinciElGarantiTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(ikinciElGarantiTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Ikinci El Uzatmali Garanti

            model.GenelBilgiler = IkinciElGarantiGenelBilgilerDetay(ikinciElGarantiTeklif.Teklif);

            #endregion

            #region Arac

            model.Arac = base.DetayAracModel(ikinciElGarantiTeklif.Teklif);

            #endregion

            #region Teklif Fiyat

            model.Fiyat = IkinciElGarantiFiyat(ikinciElGarantiTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            model.KrediKarti.KK_OdemeSekli = ikinciElGarantiTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            model.KrediKarti.KK_OdemeTipi = ikinciElGarantiTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = ikinciElGarantiTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
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
            DetayIkinciElGarantiModel model = new DetayIkinciElGarantiModel();

            #region Teklif Genel


            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif ikinciElGarantiTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = ikinciElGarantiTeklif.GenelBilgiler.TeklifId;
            model.TeklifPoliceId = id;

            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(ikinciElGarantiTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(ikinciElGarantiTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Ikinci El Uzatmali Garanti

            model.GenelBilgiler = IkinciElGarantiGenelBilgilerDetay(ikinciElGarantiTeklif);

            #endregion

            #region Arac

            model.Arac = base.DetayAracModel(ikinciElGarantiTeklif);

            #endregion

            #region Teklif Odeme

            model.OdemeBilgileri = IkinciElGarantiPoliceOdemeModel(teklif);

            TeklifTeminat ikinciElGaranti = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == IkinciElGarantiTeminatlar.IkinciElGarantiliUzatma);
            if (ikinciElGaranti != null)
                model.OdemeBilgileri.NetPrim = ikinciElGaranti.TeminatBedeli.HasValue ? ikinciElGaranti.TeminatBedeli.Value : 0;

            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult OdemeAl(OdemeIkinciElGarantiModel model)
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

        private TeklifFiyatModel IkinciElGarantiFiyat(IkinciElGarantiTeklif ikinciElGarantiTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = ikinciElGarantiTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = ikinciElGarantiTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = ikinciElGarantiTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = ikinciElGarantiTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = ikinciElGarantiTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = ikinciElGarantiTeklif.GetIsDurum();
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

        [AjaxException]
        public ActionResult PlakaSorgula(PlakaSorgulaModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(model.MusteriKodu);

                    if (musteri != null)
                    {
                        IHDITrafik HDITrafik = DependencyResolver.Current.GetService<IHDITrafik>();
                        model.PlakaNo = model.PlakaNo.ToUpperInvariant();
                        HDIPlakaSorgulamaResponse plakaSorguResponse = HDITrafik.PlakaSorgula(model.PlakaKodu, model.PlakaNo, musteri.MusteriTipKodu, musteri.KimlikNo);

                        if (!String.IsNullOrEmpty(plakaSorguResponse.Durum) && plakaSorguResponse.Durum != "0")
                        {
                            throw new Exception(String.Format("{0} - {1}", plakaSorguResponse.Durum, plakaSorguResponse.DurumMesaj));
                        }

                        HDIPlakaSorgulamaResponseDetails details = plakaSorguResponse.HDISIGORTA;
                        if (details != null && !String.IsNullOrEmpty(details.Durum) && details.Durum != "0" && !String.IsNullOrEmpty(details.Mesaj))
                        {
                            throw new Exception(String.Format("{0} - {1}", details.Durum, details.Mesaj));
                        }

                        PlakaSorguIkinciElModel plakaSorgu = new PlakaSorguIkinciElModel(details);

                        return Json(plakaSorgu, JsonRequestBehavior.AllowGet);
                    }
                }
                throw new Exception("Plaka sorgulama servisi çalıştırılırken hata oluştu.");
            }
            catch (Exception ex)
            {
                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Error(ex);
                throw;
            }
        }

        private IkinciElGarantiPoliceOdemeModel IkinciElGarantiPoliceOdemeModel(ITeklif teklif)
        {
            IkinciElGarantiPoliceOdemeModel model = new IkinciElGarantiPoliceOdemeModel();

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
                model.DokumanURL = teklif.GenelBilgiler.PDFDosyasi;
                model.teklifId = teklif.GenelBilgiler.TeklifId;
                model.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
            }
            return model;
        }

        IkinciElGarantiGenelBilgiler IkinciElGarantiGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            IkinciElGarantiGenelBilgiler model = new IkinciElGarantiGenelBilgiler();

            List<int> yillar = new List<int>();
            int today = TurkeyDateTime.Today.Year;

            for (int yil = today; yil >= (today - 7); yil--)
                yillar.Add(yil);

            model.Modeller = new SelectList(yillar).ListWithOptionLabel();
            // model.PlakaKoduListe = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlKodu", "34");
            model.PoliceSureleri = new SelectList(PoliceSureleri.PoliceSureleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.TeminatTurleri = new SelectList(TeminatTurleri.TeminatTurleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.SilindirHacimleri = new SelectList(SilindirHacimleri.SilindirHacimleriList(), "Value", "Text", "").ListWithOptionLabel();

            if (teklifId.HasValue & teklif != null)
            {
                model.Model = Convert.ToInt32(teklif.ReadSoru(IkinciElGarantiSorular.ModelYili, "0"));
                model.PoliceSuresi = Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.PoliceSuresi, "0"));
                model.SilindirHacmi = Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.SilindirHacmi, "0"));
                model.TeminatTuru = Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.TeminatTuru, "0"));
            }

            return model;
        }

        IkinciElGarantiGenelBilgiler IkinciElGarantiGenelBilgilerDetay(ITeklif teklif)
        {
            IkinciElGarantiGenelBilgiler model = new IkinciElGarantiGenelBilgiler();

            model.Model = teklif.Arac.Model ?? 0;
            model.PoliceSuresiText = PoliceSureleri.PoliceSuresi(Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.PoliceSuresi, "0")));
            model.SilindirHacmiText = SilindirHacimleri.SilindirHacmi(Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.SilindirHacmi, "0")));
            model.TeminatTuruText = TeminatTurleri.TeminatTuru(Convert.ToByte(teklif.ReadSoru(IkinciElGarantiSorular.TeminatTuru, "0")));

            return model;
        }

        public ActionResult DokumanEkle(int teklifId)
        {
            IkinciElGarantiDokumanEkleModel model = new IkinciElGarantiDokumanEkleModel();

            model.teklifId = teklifId;

            return PartialView("_DokumanEkle", model);
        }


        [HttpPost]
        public ActionResult DokumanEkle(IkinciElGarantiDokumanEkleModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && model.teklifId > 0 && file != null && file.ContentLength > 0)
            {
                ITeklif teklif = _TeklifService.GetTeklif(model.teklifId);
                if (teklif != null)
                {
                    try
                    {
                        string fileName = System.IO.Path.GetFileName(file.FileName);
                        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                        string url = storage.UploadFile("kasko", fileName + " Adi :" + model.DokumanAdi, file.InputStream);

                        teklif.GenelBilgiler.PDFDosyasi = url;

                        _TeklifService.UpdateGenelBilgiler(teklif.GenelBilgiler);

                        _LogService.Info("2.el garanti ürünü ek dosyası url: {0}", url);
                        return null;
                    }
                    catch (Exception ex)
                    {
                        _LogService.Error(ex);
                    }
                }
            }
            //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
            ModelState.AddModelError("", babonline.Message_DocumentSaveError);

            return PartialView("_DokumanEkle", model);
        }
    }
}
