using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.LilyumKoru;
using System.Text;
using Neosinerji.BABOnlineTP.Business.KORU.LilyumFerdiKaza;
using Neosinerji.BABOnlineTP.Business.Common.KORU;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]

    public class LilyumKartController : TeklifController
    {
        IMusteriDokumanStorage _Storage;
        ITVMContext _TVMContext;
        IFormsAuthenticationService _FormsAuthenticationService;
        IEMailService _EMailService;
        public LilyumKartController(ITVMService tvmService,
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
                              ITVMContext tvm,
                              IFormsAuthenticationService formsAuthenticationService,
                              IEMailService emailService)
            : base(tvmService, teklifService, musteriService, kullaniciService, aktifKullaniciService, tanimService,
            ulkeService, crService, aracService, urunService, tumService)
        {
            _FormsAuthenticationService = formsAuthenticationService;
            _Storage = storage;
            _TVMContext = tvm;
            _TeklifService = teklifService;
            _EMailService = emailService;
        }
        // GET: Teklif/LilyumFerdiKaza
        [HttpGet]
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]
        public ActionResult Ekle(int? id)
        {
            ViewBag.AnaAcenteMi = _AktifKullaniciService.BagliOlduguTvmKodu;
            if (_AktifKullaniciService.YetkiGrubu == TvmYetkiKodlari.LilyumYoneticiYetkiKodu)
            {
                ViewBag.TutarYetki = true;
            }
            else
            {
                ViewBag.TutarYetki = false;
            }
            LilyumFerdiKazaModel model = EkleModel(id, null);
            return View(model);
        }
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]
        public LilyumFerdiKazaModel EkleModel(int? id, int? teklifId)
        {
            ViewBag.AnaAcenteMi = _AktifKullaniciService.BagliOlduguTvmKodu;

            LilyumFerdiKazaModel model = new LilyumFerdiKazaModel();
            ListModels meslek = new ListModels();
            List<ListModels> meslekList = new List<ListModels>();
            IKoruFerdiKaza request = DependencyResolver.Current.GetService<IKoruFerdiKaza>();

            model.GenelBilgiler = new LilyumGenelBilgilerModel();
            model.UrunAdi = "LilyumKart";
            model.GenelBilgiler.PlakaIlKodu = "34";
            model.GenelBilgiler.PlakaNo = "";
            model.GenelBilgiler.MotorsikletKullaniyorMu = false;
            model.GenelBilgiler.SporlaUgrasiyorMu = false;
            //Teklifi hazırlayan
            model.Hazirlayan = base.EkleHazirlayanModel();

            //Sigorta Ettiren / Sigortalı
            model.Musteri = new SigortaliModel();
            model.Musteri.SigortaliAyni = false;
            model.Musteri.Sigortali = new Models.MusteriModel();
            model.Odeme = new LilyumTeklifOdemeModel();
            model.Odeme.OdemeSekli = true;
            model.Odeme.DigerOdemeTutari = null;
            model.AdresAyniMi = true;

            List<SelectListItem> ulkeler = new List<SelectListItem>();
            ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });
            model.GenelBilgiler.PoliceBaslangicTarihi = TurkeyDateTime.Today;
            model.GenelBilgiler.PoliceBitisTarihi = TurkeyDateTime.Today.AddYears(1);
            model.Odeme.OdemeTipi = OdemeTipleri.KrediKarti;

            #region TUM IMAGES

            model.TeklifUM = new TeklifUMListeModel();
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

        [HttpPost]
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]
        [AjaxException]
        public ActionResult Hesapla(LilyumFerdiKazaModel model)
        {
            try
            {
                string constDate = "01.01.1900";
                model.Musteri.Sigortali.DogumTarihi = System.Convert.ToDateTime(constDate);
                // model.Musteri.Sigortali.DogumTarihi= Convert.ToDateTime(model.Musteri.Sigortali.DogumTarihi.ToString("yyyy-MM-dd h:mm tt"));

                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                #endregion

                #region Teklif kaydı ve hesaplamanın başlatılması
                if (!ModelState.IsValid)
                {
                    #region Sigortali
                    model.Hazirlayan = new HazirlayanModel();
                    model.Hazirlayan.TVMKodu = _AktifKullaniciService.TVMKodu;
                    model.Hazirlayan.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;

                    //Sigorta Ettiren Değişmiyor.
                    //13932934- Gülru (Canay)
                    var siEttiren = _MusteriService.GetMusteri(13932934);
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.Lilyum,
                                                                              model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                               13932934, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    if (model.GenelBilgiler == null)
                    {
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruMotorsikletKullaniyorMu, false);
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSporlaUgrasiyorMu, false);
                    }
                    else
                    {
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruMotorsikletKullaniyorMu, model.GenelBilgiler.MotorsikletKullaniyorMu);
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSporlaUgrasiyorMu, model.GenelBilgiler.SporlaUgrasiyorMu);
                    }

                    //model.GenelBilgiler = new LilyumGenelBilgilerModel();  
                    if (_AktifKullaniciService.TVMKodu == NeosinerjiTVM.LilyumInternetSatisTVMKodu)
                    {
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliTCKimlikNo, _AktifKullaniciService.TCKN);
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruLilyumEmail, _AktifKullaniciService.Email);
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliAdi, _AktifKullaniciService.Adi);
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliSoyadi, _AktifKullaniciService.Soyadi);
                    }
                    else
                    {
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliTCKimlikNo, model.Musteri.Sigortali.KimlikNo);
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruLilyumEmail, model.Musteri.Sigortali.Email);
                        if (!String.IsNullOrEmpty(model.Musteri.Sigortali.AdiUnvan))
                        {
                            teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliAdi, model.Musteri.Sigortali.AdiUnvan);
                        }
                        if (!String.IsNullOrEmpty(model.Musteri.Sigortali.SoyadiUnvan))
                        {
                            teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliSoyadi, model.Musteri.Sigortali.SoyadiUnvan);
                        }
                    }
                    teklif.AddSoru(LilyumFerdiKazaSorular.KoruLilyumTelefon, model.Musteri.Sigortali.CepTelefonu);
                    teklif.AddSoru(LilyumFerdiKazaSorular.KoruLilyumTeslimatAdres, model.TeslimatAdresi);
                    teklif.AddSoru(LilyumFerdiKazaSorular.KoruLilyumIletisimAdres, model.IletisimAdresi);
                    teklif.AddSoru(LilyumFerdiKazaSorular.KorulilyumIletisimIlKodu, model.IletisimIlKodu);
                    teklif.AddSoru(LilyumFerdiKazaSorular.KorulilyumIletisimIlceKodu, model.IletisimIlceKodu);
                    teklif.AddSoru(LilyumFerdiKazaSorular.KorulilyumTeslimatIlKodu, model.TeslimatIlKodu);
                    teklif.AddSoru(LilyumFerdiKazaSorular.KorulilyumTeslimatIlceKodu, model.TeslimatIlceKodu);
                    teklif.AddSoru(LilyumFerdiKazaSorular.KorulilyumDigerOdemeTutari, model.Odeme.DigerOdemeTutari.ToString());

                    if (model.Musteri.Sigortali.DogumTarihi.HasValue)
                    {
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliDogumTarihi, model.Musteri.Sigortali.DogumTarihi.Value.ToString());
                    }

                    if (!String.IsNullOrEmpty(model.SigortaliMeslekKodu))
                    {
                        teklif.AddSoru(LilyumFerdiKazaSorular.KoruSigortaliMeslek, model.SigortaliMeslekKodu);
                    }
                    model.Musteri.SigortaEttiren = new Models.MusteriModel();
                    model.Musteri.SigortaEttiren.AdiUnvan = siEttiren.AdiUnvan;
                    model.Musteri.SigortaEttiren.SoyadiUnvan = siEttiren.SoyadiUnvan;
                    model.Musteri.SigortaEttiren.Uyruk = siEttiren.Uyruk;
                    model.Musteri.SigortaEttiren.KimlikNo = siEttiren.KimlikNo;
                    model.Musteri.SigortaEttiren.Cinsiyet = siEttiren.Cinsiyet;
                    model.Musteri.SigortaEttiren.Email = siEttiren.EMail;
                    model.Musteri.SigortaEttiren.MusteriKodu = siEttiren.MusteriKodu;
                    model.Musteri.SigortaEttiren.MusteriTipKodu = siEttiren.MusteriTipKodu;
                    model.Musteri.SigortaEttiren.TVMKodu = siEttiren.TVMKodu;
                    model.Musteri.SigortaEttiren.DogumTarihi = siEttiren.DogumTarihi;

                    LilyumTeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Teklif kaydı ve hesaplamanın başlatılması
                    //if (ModelState.IsValid)
                    //{
                    try
                    {
                        #region Teklif return

                        ILilyumKoruTeklif lilyumTeklif = new LilyumKoruTeklif();
                        lilyumTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.KORU);

                        // ==== Teklif ödeme şekli ve tipi ==== //
                        if (model.Odeme.OdemeSekli)
                        {
                            teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                            model.Odeme.TaksitSayisi = 1;
                        }
                        else
                        {
                            teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Vadeli;
                            model.Odeme.TaksitSayisi = 6;
                        }

                        if (model.KrediKartiMi)
                        {
                            teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;
                        }
                        else
                        {
                            teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.Nakit;
                        }

                        if (!model.Odeme.OdemeSekli)
                        {
                            teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                            lilyumTeklif.AddOdemePlani(model.Odeme.TaksitSayisi.Value);
                        }
                        else
                        {
                            teklif.GenelBilgiler.TaksitSayisi = 1;
                            lilyumTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                        }
                        if (!String.IsNullOrEmpty(model.GenelBilgiler.PlakaIlKodu) && !String.IsNullOrEmpty(model.GenelBilgiler.PlakaNo))
                        {
                            teklif.Arac.PlakaKodu = model.GenelBilgiler.PlakaIlKodu;
                            teklif.Arac.PlakaNo = model.GenelBilgiler.PlakaNo;
                        }

                        IsDurum isDurum = lilyumTeklif.Hesapla(teklif);
                        #endregion
                        return Json(new { id = isDurum.IsId, g = isDurum.Guid });
                    }

                    catch (Exception ex)
                    {
                        _LogService.Error(ex);
                    }
                    //}
                    #endregion
                }


                #endregion

                #region Hata Log
                StringBuilder sb = new StringBuilder();
                foreach (var key in ModelState.Keys)
                {
                    ModelState state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        if (sb.Length > 0) sb.Append(", ");
                        sb.AppendFormat("key {0}", key);
                    }
                }

                if (sb.Length > 0)
                {
                    return Json(new { id = 0, hata = "Validasyon başarısız. Hatalı alanlar : " + sb.ToString() });
                }
                #endregion

                //string token = lilyumt.ReadWebServisCevap(WebServisCevaplar.Koru3DParatikaToken, "0");

            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]

        public ActionResult Police(int id, string odemeTutari)
        {
            DetayLilyumModel model = new DetayLilyumModel();

            #region Teklif Genel

            ITeklif lilyumTeklif = _TeklifService.GetTeklif(id);
            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(lilyumTeklif.TeklifNo, lilyumTeklif.GenelBilgiler.TVMKodu);
            model.TeklifId = anaTeklif.GenelBilgiler.TeklifId;
            model.PoliceId = id;
            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(anaTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(anaTeklif.SigortaEttiren.MusteriKodu);
            var sigortali = anaTeklif.Sigortalilar.First();
            if (sigortali != null)
            {
                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(sigortali.MusteriKodu);
                if (anaTeklif.Sigortalilar.Count > 0 && musteri != null)
                {
                    model.Sigortali = new DetayLilyumMusteriModel();
                    model.Sigortali.MusteriKodu = anaTeklif.Sigortalilar.First().MusteriKodu;
                    model.Sigortali.MusteriTipText = nsmusteri.MusteriListProvider.GetMusteriTipiText(musteri.MusteriTipKodu);
                    model.Sigortali.KimlikNo = musteri.KimlikNo;
                    model.Sigortali.AdiUnvan = musteri.AdiUnvan;
                    model.Sigortali.SoyadiUnvan = musteri.SoyadiUnvan;
                    model.Sigortali.Email = musteri.EMail;
                    model.Sigortali.UlkeAdi = "TÜRKİYE";
                    MusteriAdre iletisimAdresi = musteri.MusteriAdres.FirstOrDefault(f => f.AdresTipi == AdresTipleri.Iletisim);
                    if (iletisimAdresi != null)
                    {
                        model.Sigortali.IlAdi = _UlkeService.GetIlAdi("TUR", iletisimAdresi.IlKodu);
                        model.Sigortali.IlceAdi = _UlkeService.GetIlceAdi(iletisimAdresi.IlceKodu.Value);
                        model.Sigortali.IletisimAcikAdres = iletisimAdresi.Adres;
                    }

                    MusteriAdre teslimatAdresi = musteri.MusteriAdres.FirstOrDefault(f => f.AdresTipi == AdresTipleri.Teslimat);
                    if (teslimatAdresi != null)
                    {
                        model.Sigortali.TeslimatIlAdi = _UlkeService.GetIlAdi("TUR", teslimatAdresi.IlKodu);
                        model.Sigortali.TeslimatIlceAdi = _UlkeService.GetIlceAdi(teslimatAdresi.IlceKodu.Value);
                        model.Sigortali.TeslimatAcikAdres = teslimatAdresi.Adres;
                    }

                    MusteriTelefon cepTelefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                    if (cepTelefon != null)
                    {
                        model.Sigortali.CepTelText = cepTelefon.Numara;
                    }
                }
            }

            #endregion

            #region Teklif Genel Bilgiler
            model.GenelBilgiler = new LilyumGenelBilgilerDetayModel();
            model.GenelBilgiler.PoliceBaslangicTarihi = anaTeklif.GenelBilgiler.BaslamaTarihi.ToString("dd/MM/yyyy");
            model.GenelBilgiler.PoliceBitisTarihi = anaTeklif.GenelBilgiler.BitisTarihi.ToString("dd/MM/yyyy");
            model.GenelBilgiler.OncekiPoliceBaslangicTarihi = anaTeklif.ReadSoru(TSSSorular.Eski_Police_Baslangic_Tarihi, "");
            model.GenelBilgiler.TeklifId = anaTeklif.GenelBilgiler.TeklifId;
            if (!String.IsNullOrEmpty(anaTeklif.Arac.PlakaKodu))
            {
                model.GenelBilgiler.Plaka = anaTeklif.Arac.PlakaKodu + anaTeklif.Arac.PlakaNo;
            }
            //var MotosikletKullaniyorMu = anaTeklif.ReadSoru(LilyumFerdiKazaSorular.KoruMotorsikletKullaniyorMu, false);
            //if (MotosikletKullaniyorMu)
            //{
            //    model.GenelBilgiler.MotorsikletKullaniyorMu = "Evet";
            //}
            //else
            //{
            //    model.GenelBilgiler.MotorsikletKullaniyorMu = "Hayır";
            //}
            //var SporlaUgrasiyorMu = anaTeklif.ReadSoru(LilyumFerdiKazaSorular.KoruSporlaUgrasiyorMu, false);
            //if (SporlaUgrasiyorMu)
            //{
            //    model.GenelBilgiler.SporlaUgrasiyorMu = "Evet";
            //}
            //else
            //{
            //    model.GenelBilgiler.SporlaUgrasiyorMu = "Hayır";
            //}

            #endregion

            IKoruFerdiKaza koruFerdiKaza = DependencyResolver.Current.GetService<IKoruFerdiKaza>();
            KoruPoliceResponseModel KoruPoliceModel = new KoruPoliceResponseModel();
            if (lilyumTeklif != null)
            {
                if (String.IsNullOrEmpty(lilyumTeklif.GenelBilgiler.TUMPoliceNo))
                {
                    IsDurumDetay durumDetay = _TeklifService.GetIsDurumDetay(lilyumTeklif.GenelBilgiler.TeklifId);
                    if (durumDetay != null)
                    {
                        if (!String.IsNullOrEmpty(durumDetay.HataMesaji))
                        {
                            KoruPoliceModel.HataMesaji = durumDetay.HataMesaji;
                        }
                        else
                        {
                            KoruPoliceModel = koruFerdiKaza.KoruPolicelestir(anaTeklif, lilyumTeklif.GenelBilgiler.TeklifId);
                        }
                    }
                    else
                    {
                        KoruPoliceModel = koruFerdiKaza.KoruPolicelestir(anaTeklif, lilyumTeklif.GenelBilgiler.TeklifId);
                    }

                }
            }

            var lilyumSonOdemeDurumu = lilyumTeklif.GenelBilgiler.TeklifWebServisCevaps.Where(s => s.CevapKodu == WebServisCevaplar.Koru3DParatikaSonOdemeDurumu).FirstOrDefault();

            if (lilyumSonOdemeDurumu != null)
            {
                model.Parartika3DLilyumSonOdemeDurumu = lilyumSonOdemeDurumu.Cevap;
            }
            else
            {
                var lilyumGuid = lilyumTeklif.GenelBilgiler.TeklifWebServisCevaps.Where(s => s.CevapKodu == WebServisCevaplar.KoruTokenGuidId).FirstOrDefault();
                if (lilyumGuid != null)
                {
                    var ParatikaOdeme = koruFerdiKaza.LilyumParatika3DSSonOdemeDurumu(lilyumGuid.Cevap);
                    if (ParatikaOdeme != null)
                    {
                        model.Parartika3DLilyumSonOdemeDurumu = ParatikaOdeme;
                    }
                }
            }
            if (!String.IsNullOrEmpty(lilyumTeklif.GenelBilgiler.TUMPoliceNo))
            {
                model.TUMPoliceNo = lilyumTeklif.GenelBilgiler.TUMPoliceNo;
            }
            else
            {
                model.KoruPoliceDurumu = KoruPoliceModel.HataMesaji;
            }
            model.BrutPrim = lilyumTeklif.GenelBilgiler.BrutPrim;
            model.TaksitSayisi = lilyumTeklif.GenelBilgiler.TaksitSayisi;
            ITUMService tumService = DependencyResolver.Current.GetService<ITUMService>();
            TUMDetay tum = tumService.GetDetay(lilyumTeklif.TUMKodu);
            model.TUMUnvani = tum.Unvani;
            model.TUMLogoURL = tum.Logo;
            // email gönder.
            string referansNo = "";
            if (!String.IsNullOrEmpty(model.TUMPoliceNo))
            {
                referansNo = model.TUMPoliceNo;
            }
            else
            {
                referansNo = model.KoruPoliceDurumu;
            }
            if (_AktifKullaniciService.TVMKodu == 153008)
            {
                _EMailService.SendLilyumBilgilendirme(model.Sigortali.AdiUnvan + " " + model.Sigortali.SoyadiUnvan, referansNo, model.BrutPrim.ToString(), model.Parartika3DLilyumSonOdemeDurumu, model.Sigortali.Email,odemeTutari.ToString());
                if (model.Parartika3DLilyumSonOdemeDurumu == "Başarılı ")
                {
                    return RedirectToAction("Bilgilendirme", "SatinAl", new
                    {
                        area = "Bireysel",
                        odemeDurum = true
                    });
                }
                else
                {
                    return RedirectToAction("Bilgilendirme", "SatinAl", new
                    {
                        area = "Bireysel",
                        odemeDurum = false
                    });
                }
            }
            else
            {
                _EMailService.SendLilyumBilgilendirme(model.Sigortali.AdiUnvan + " " + model.Sigortali.SoyadiUnvan, referansNo, model.BrutPrim.ToString(), model.Parartika3DLilyumSonOdemeDurumu, model.Sigortali.Email, odemeTutari.ToString());
            }
            model.OdemeTutari = odemeTutari.ToString();
            return View(model);
        }
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]

        public ActionResult Detay(int id)
        {
            DetayLilyumModel model = new DetayLilyumModel();
            IKoruFerdiKaza koruFerdiKaza = DependencyResolver.Current.GetService<IKoruFerdiKaza>();
            #region Teklif Genel

            LilyumKoruTeklif anaTeklif = new LilyumKoruTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = anaTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(anaTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(anaTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(anaTeklif.Teklif.Sigortalilar.First().MusteriKodu);
            if (anaTeklif.Teklif.Sigortalilar.Count > 0 && musteri != null)
            {
                model.Sigortali = new DetayLilyumMusteriModel();
                model.Sigortali.MusteriKodu = anaTeklif.Teklif.Sigortalilar.First().MusteriKodu;
                model.Sigortali.MusteriTipText = nsmusteri.MusteriListProvider.GetMusteriTipiText(musteri.MusteriTipKodu);
                model.Sigortali.KimlikNo = musteri.KimlikNo;
                model.Sigortali.AdiUnvan = musteri.AdiUnvan;
                model.Sigortali.SoyadiUnvan = musteri.SoyadiUnvan;
                model.Sigortali.Email = musteri.EMail;
                model.Sigortali.UlkeAdi = "TÜRKİYE";
                MusteriAdre iletisimAdresi = musteri.MusteriAdres.FirstOrDefault(f => f.AdresTipi == AdresTipleri.Iletisim);
                if (iletisimAdresi != null)
                {
                    model.Sigortali.IlAdi = _UlkeService.GetIlAdi("TUR", iletisimAdresi.IlKodu);
                    model.Sigortali.IlceAdi = _UlkeService.GetIlceAdi(iletisimAdresi.IlceKodu.Value);
                    model.Sigortali.IletisimAcikAdres = iletisimAdresi.Adres;
                }


                MusteriAdre teslimatAdresi = musteri.MusteriAdres.FirstOrDefault(f => f.AdresTipi == AdresTipleri.Teslimat);
                if (teslimatAdresi != null)
                {
                    model.Sigortali.TeslimatIlAdi = _UlkeService.GetIlAdi("TUR", teslimatAdresi.IlKodu);
                    model.Sigortali.TeslimatIlceAdi = _UlkeService.GetIlceAdi(teslimatAdresi.IlceKodu.Value);
                    model.Sigortali.TeslimatAcikAdres = teslimatAdresi.Adres;
                }


                MusteriTelefon cepTelefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                if (cepTelefon != null)
                {
                    model.Sigortali.CepTelText = cepTelefon.Numara;
                }
            }

            #endregion

            #region Teklif Genel Bilgiler
            model.GenelBilgiler = new LilyumGenelBilgilerDetayModel();
            model.GenelBilgiler.PoliceBaslangicTarihi = anaTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd/MM/yyyy");
            model.GenelBilgiler.PoliceBitisTarihi = anaTeklif.Teklif.GenelBilgiler.BitisTarihi.ToString("dd/MM/yyyy");

            #endregion

            #region Teklif Fiyat

            var tumTeklif = anaTeklif.TUMTeklifler.Where(w => w.GenelBilgiler.TUMKodu == TeklifUretimMerkezleri.KORU).FirstOrDefault();
            if (tumTeklif != null)
            {
                model.TaksitSayisi = tumTeklif.GenelBilgiler.TaksitSayisi.HasValue ? tumTeklif.GenelBilgiler.TaksitSayisi : 0;

                KoruPoliceResponseModel KoruPoliceModel = new KoruPoliceResponseModel();
                if (tumTeklif != null)
                {
                    if (String.IsNullOrEmpty(tumTeklif.GenelBilgiler.TUMPoliceNo))
                    {
                        IsDurumDetay durumDetay = _TeklifService.GetIsDurumDetay(tumTeklif.GenelBilgiler.TeklifId);
                        if (durumDetay != null)
                        {
                            if (!String.IsNullOrEmpty(durumDetay.HataMesaji))
                            {
                                KoruPoliceModel.HataMesaji = durumDetay.HataMesaji;
                                model.KoruPoliceDurumu = KoruPoliceModel.HataMesaji;
                            }
                        }
                    }
                }
                model.BrutPrim = tumTeklif.GenelBilgiler.BrutPrim;

                var lilyumSonOdemeDurumu = tumTeklif.GenelBilgiler.TeklifWebServisCevaps.Where(s => s.CevapKodu == WebServisCevaplar.Koru3DParatikaSonOdemeDurumu).FirstOrDefault();

                if (lilyumSonOdemeDurumu != null)
                {
                    var asa = lilyumSonOdemeDurumu.Cevap;
                    model.Parartika3DLilyumSonOdemeDurumu = asa;
                }
                else
                {
                    var lilyumGuid = tumTeklif.GenelBilgiler.TeklifWebServisCevaps.Where(s => s.CevapKodu == WebServisCevaplar.KoruTokenGuidId).FirstOrDefault();
                    if (lilyumGuid != null)
                    {
                        var aa = koruFerdiKaza.LilyumParatika3DSSonOdemeDurumu(lilyumGuid.Cevap);
                        if (aa != null)
                        {
                            model.Parartika3DLilyumSonOdemeDurumu = aa;
                        }
                    }
                }
                ITUMService tumService = DependencyResolver.Current.GetService<ITUMService>();
                TUMDetay tum = tumService.GetDetay(tumTeklif.TUMKodu);
                model.TUMUnvani = tum.Unvani;
                model.TUMLogoURL = tum.Logo;
            }

            return View(model);
            #endregion
        }
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]

        [HttpPost]
        public ActionResult OdemeAl(OdemeLilyumModel model)
        {
            TeklifOdemeCevapModel cevap = new TeklifOdemeCevapModel();
            if (model.KrediKarti.KK_OdemeTipi == OdemeTipleri.Nakit)
            {
                TryValidateModel(model);
                if (ModelState["KrediKarti.KartSahibi"] != null)
                    ModelState["KrediKarti.KartSahibi"].Errors.Clear();
                if (ModelState["KrediKarti.KartNumarasi"] != null)
                    ModelState["KrediKarti.KartNumarasi"].Errors.Clear();
                if (ModelState["KrediKarti.GuvenlikNumarasi"] != null)
                    ModelState["KrediKarti.GuvenlikNumarasi"].Errors.Clear();
                if (ModelState["KrediKarti.SonKullanmaAy"] != null)
                    ModelState["KrediKarti.SonKullanmaAy"].Errors.Clear();
                if (ModelState["KrediKarti.SonKullanmaYil"] != null)
                    ModelState["KrediKarti.SonKullanmaYil"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                nsbusiness.ITeklif teklif = _TeklifService.GetTeklif(model.KrediKarti.KK_TeklifId);

                if (!String.IsNullOrEmpty(teklif.GenelBilgiler.TUMPoliceNo))
                {
                    string msg = String.Format("Bu teklif daha önce poliçeleştirilmiş : {0}. Lütfen yeni bir teklif alarak poliçeleştirin.", teklif.GenelBilgiler.TUMPoliceNo);
                    cevap.Hatalar = new string[] { msg };
                    cevap.RedirectUrl = String.Empty;
                    return Json(cevap);
                }
                nsbusiness.Odeme odeme = null;
                if (model.KrediKarti.KK_OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    odeme = new nsbusiness.Odeme(model.KrediKarti.KartSahibi, model.KrediKarti.KartNumarasi.ToString(), model.KrediKarti.GuvenlikNumarasi, model.KrediKarti.SonKullanmaAy, model.KrediKarti.SonKullanmaYil, model.KrediKarti.KK_OdemeSekli);
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin)
                        odeme.TaksitSayisi = 1;
                    else
                        odeme.TaksitSayisi = model.KrediKarti.TaksitSayisi;
                }
                else
                {
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin || model.KrediKarti.KK_OdemeTipi == OdemeTipleri.Havale)
                    {
                        odeme = new Odeme(OdemeSekilleri.Pesin, 1);
                    }
                    else
                    {
                        odeme = new Odeme(model.KrediKarti.KK_OdemeSekli, model.KrediKarti.TaksitSayisi);
                    }
                }
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
                    cevap.RedirectUrl = TeklifSayfaAdresleri.PoliceAdres(UrunKodlari.Lilyum) + urun.GenelBilgiler.TeklifId;
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
        [Authorization(UrunKodu = UrunKodlari.Lilyum)]
        public ActionResult OdemeAlLilyum(OdemeLilyumModel model)
        {
            TeklifOdemeCevapModel cevap = new TeklifOdemeCevapModel();
            if (model.KrediKarti.KK_OdemeTipi == OdemeTipleri.Nakit)
            {
                TryValidateModel(model);
                if (ModelState["KrediKarti.KartSahibi"] != null)
                    ModelState["KrediKarti.KartSahibi"].Errors.Clear();
                if (ModelState["KrediKarti.KartNumarasi"] != null)
                    ModelState["KrediKarti.KartNumarasi"].Errors.Clear();
                if (ModelState["KrediKarti.GuvenlikNumarasi"] != null)
                    ModelState["KrediKarti.GuvenlikNumarasi"].Errors.Clear();
                if (ModelState["KrediKarti.SonKullanmaAy"] != null)
                    ModelState["KrediKarti.SonKullanmaAy"].Errors.Clear();
                if (ModelState["KrediKarti.SonKullanmaYil"] != null)
                    ModelState["KrediKarti.SonKullanmaYil"].Errors.Clear();
            }
            if (ModelState.IsValid)
            {
                nsbusiness.ITeklif teklif = _TeklifService.GetTeklif(model.KrediKarti.KK_TeklifId);

                if (!String.IsNullOrEmpty(teklif.GenelBilgiler.TUMPoliceNo))
                {
                    string msg = String.Format("Bu teklif daha önce poliçeleştirilmiş : {0}. Lütfen yeni bir teklif alarak poliçeleştirin.", teklif.GenelBilgiler.TUMPoliceNo);
                    cevap.Hatalar = new string[] { msg };
                    cevap.RedirectUrl = String.Empty;
                    return Json(cevap);
                }
                nsbusiness.Odeme odeme = null;
                if (model.KrediKarti.KK_OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    odeme = new nsbusiness.Odeme(model.KrediKarti.KartSahibi, model.KrediKarti.KartNumarasi.ToString(), model.KrediKarti.GuvenlikNumarasi, model.KrediKarti.SonKullanmaAy, model.KrediKarti.SonKullanmaYil, model.KrediKarti.KK_OdemeSekli);
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin)
                        odeme.TaksitSayisi = 1;
                    else
                        odeme.TaksitSayisi = model.KrediKarti.TaksitSayisi;
                }
                else
                {
                    if (model.KrediKarti.KK_OdemeSekli == OdemeSekilleri.Pesin || model.KrediKarti.KK_OdemeTipi == OdemeTipleri.Havale)
                    {
                        odeme = new Odeme(OdemeSekilleri.Pesin, 1);
                    }
                    else
                    {
                        odeme = new Odeme(model.KrediKarti.KK_OdemeSekli, model.KrediKarti.TaksitSayisi);
                    }
                }
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
                    cevap.RedirectUrl = TeklifSayfaAdresleri.PoliceAdres(UrunKodlari.Lilyum) + urun.GenelBilgiler.TeklifId;
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
        [AjaxException]
        public ActionResult DekontPDF(int id)
        {
            bool success = true;
            string url = String.Empty;
            ITeklif teklif = _TeklifService.GetTeklif(id);

            try
            {
                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFDekont))
                {
                    ITeklif urun = TeklifUrunFactory.AsUrunClass(teklif);
                    urun.DekontPDF();

                    teklif = _TeklifService.GetTeklif(id);
                }

                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFDekont))
                {
                    throw new Exception("Dekont pdf'i oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                _LogService.Error("LilyumFerdiKazaController.DekontPDF", ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFDekont;
            return Json(new { Success = success, PDFUrl = url });
        }

        private TeklifFiyatModel LilyumFiyat(LilyumKoruTeklif lilyumTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = lilyumTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = lilyumTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            var tumListe = lilyumTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = lilyumTeklif.GetIsDurum();
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

        [HttpGet]
        [AjaxException]
        public ActionResult GetBitisTarihi(DateTime baslamaTarihi)
        {
            PoliceBitisTarihiModel model = new PoliceBitisTarihiModel();

            var date = baslamaTarihi.AddYears(1);
            model.PoliceBitisTarihi = date.ToString("dd/MM/yyyy");

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public class ListModels
        {
            public string key { get; set; }
            public string value { get; set; }
        }

        [HttpPost]
        public ActionResult LilyumParaticaUrl(int isId, string guid, int[] gosterilenler)
        {
            ILogService _log = DependencyResolver.Current.GetService<ILogService>();
            TeklifDurumModel model = new TeklifDurumModel();
            try
            {
                IsDurum durum = _TeklifService.GetIsDurumu(isId);
                if (guid != durum.Guid)
                {
                    model.mesaj = "Geçersiz anahtar.";
                    return Json(new { model = model });
                }
                TimeSpan ts = TurkeyDateTime.Now.Subtract(durum.Baslangic.Value);
                if (ts.TotalMinutes > 20)
                {
                    model.mesaj = "Geçersiz istek.";
                    return Json(new { model = model });
                }

                model.id = isId;
                model.tamamlandi = durum.Durumu == IsDurumTipleri.Tamamlandi;
                model.teklifId = durum.ReferansId;

                var detaylar = durum.IsDurumDetays.ToList<IsDurumDetay>();
                var tamamlananlar = detaylar.Where(w => w.Durumu == IsDurumTipleri.Tamamlandi);

                IsDurumDetay tamamlananTeklif = new IsDurumDetay();

                string LilyumParaticaURL = "";
                StringBuilder hatalar = new StringBuilder();
                if (tamamlananlar.Count(c => c.TUMKodu == TeklifUretimMerkezleri.KORU) > 0)
                { tamamlananTeklif = tamamlananlar.Where(w => w.TUMKodu == TeklifUretimMerkezleri.KORU).FirstOrDefault(); }
                if (tamamlananTeklif != null)
                {
                    var getPolice = _TeklifService.GetTeklifGenel(tamamlananTeklif.ReferansId);
                    if (getPolice != null)
                    {
                        var paratikaUrl = getPolice.TeklifWebServisCevaps.Where(w => w.CevapKodu == WebServisCevaplar.Koru3DParatikaToken).FirstOrDefault();
                        string url = paratikaUrl != null ? paratikaUrl.Cevap : "";
                        if (!String.IsNullOrEmpty(url))
                        {
                            LilyumParaticaURL = "https://vpos.paratika.com.tr/payment/" + url;
                        }

                    }
                    if (LilyumParaticaURL == "")
                    {

                        List<string> hataListe = detaylar.Where(w => w.TUMKodu == TeklifUretimMerkezleri.KORU)
                                                         .GroupBy(g => g.HataMesaji)
                                                         .Select(s => s.Key)
                                                         .ToList<string>();


                        foreach (string hata in hataListe)
                        {
                            string[] parts = hata.Split('|');

                            foreach (string h in parts)
                            {
                                hatalar.AppendLine(h);
                            }
                        }
                    }

                    return Json(new { hata = hatalar.ToString(), LilyumParaticaURL = LilyumParaticaURL });
                }



            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return Json(new { model = model });
        }


        [HttpGet]
        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, UrunKodu = UrunKodlari.Lilyum)]
        public ActionResult SatinAl()
        {
            ViewBag.AnaAcenteMi = _AktifKullaniciService.BagliOlduguTvmKodu;

            if (_AktifKullaniciService.YetkiGrubu== TvmYetkiKodlari.LilyumYoneticiYetkiKodu)
            {
                ViewBag.TutarYetki = true;
            }
            else
            {
                ViewBag.TutarYetki = false;
            }


            LilyumFerdiKazaModel model = new LilyumFerdiKazaModel();
            model = EkleModel(null, null);
            model.Musteri.Sigortali.KimlikNo = _AktifKullaniciService.TCKN;
            model.Musteri.Sigortali.Email = _AktifKullaniciService.Email;
            model.Musteri.Sigortali.AdiUnvan = _AktifKullaniciService.Adi;
            model.Musteri.Sigortali.SoyadiUnvan = _AktifKullaniciService.Soyadi;
            var MeslekListesi = _TeklifService.GetMeslekList();
            if (MeslekListesi != null)
            {
                model.SigortaliMeslekListesi = new SelectList(MeslekListesi, "MeslekKodu", "Aciklama", "").ListWithOptionLabel();
            }
            return View(model);
        }
    }
}