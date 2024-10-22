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
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.OdulluBirikim)]
    public class OdulluBirikimController : TeklifController
    {
        IKonfigurasyonService _KonfigurasyonService;

        public OdulluBirikimController(ITVMService tvmService,
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
            _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
        }

        public ActionResult Ekle(int? id)
        {
            OdulluBirikimModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            OdulluBirikimModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public OdulluBirikimModel EkleModel(int? id, int? teklifId)
        {
            OdulluBirikimModel model = new OdulluBirikimModel();
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
                    model.Musteri.SigortaEttiren.Cinsiyet = "";
                    model.Musteri.SigortaEttiren.CepTelefonu = "";
                    model.Musteri.Ulkeler = ulkeler;
                    model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi").ListWithOptionLabelIller();
                    model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", "34"), "IlceKodu", "IlceAdi").ListWithOptionLabel();
                }


                model.Musteri.MusteriTipleri = nsmusteri.MusteriListProvider.MusteriTipleri();
                model.Musteri.UyrukTipleri = new SelectList(nsmusteri.MusteriListProvider.UyrukTipleri(), "Value", "Text", "0");
                model.Musteri.CinsiyetTipleri = new SelectList(nsmusteri.MusteriListProvider.CinsiyetTipleri(), "Value", "Text");
                model.Musteri.GelirVergisiTipleri = new SelectList(TeklifProvider.GelirVergisiTipleriAEGON(), "Value", "Text", "");
                model.Musteri.CinsiyetTipleri.First().Selected = true;
                #endregion

                #region Odullu Birkim

                model.GenelBilgiler = OdulluBirikimGenelBilgiler(teklifId, teklif);

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                TUMDetay aegon = _TUMService.GetDetay(TeklifUretimMerkezleri.AEGON);
                model.TeklifUM.Add(aegon.Kodu, aegon.Unvani, aegon.Logo);

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
        public ActionResult Hesapla(OdulluBirikimModel model)
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
                    int musteriKodu = OB_MusteriKaydet(model);
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.OdulluBirikim, model.Hazirlayan.TVMKodu,
                                                                         _AktifKullaniciService.KullaniciKodu, musteriKodu, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Gerekli Girişler ve Hata Uyarıları

                    #endregion

                    #region Sigortali

                    teklif.AddSigortali(musteriKodu);

                    #endregion

                    #region Ödüllü Birikim

                    #region Genel Bilgiler

                    teklif.AddSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, model.GenelBilgiler.SigortaBaslangicTarihi);

                    teklif.AddSoru(OdulluBirikimSorular.ParaBirimi, AegonParaBirimleri.TL.ToString()); //Default TL Ekleniyor.

                    if (model.GenelBilgiler.SigortaSuresi.HasValue)
                        teklif.AddSoru(OdulluBirikimSorular.SigortaSuresi, model.GenelBilgiler.SigortaSuresi.Value.ToString());

                    if (model.GenelBilgiler.PrimOdemeDonemi.HasValue)
                        teklif.AddSoru(OdulluBirikimSorular.PrimOdemeDonemi, model.GenelBilgiler.PrimOdemeDonemi.Value.ToString());

                    if (model.GenelBilgiler.HesaplamaSecenegi.HasValue)
                    {
                        teklif.AddSoru(OdulluBirikimSorular.HesaplamaSecenegi, model.GenelBilgiler.HesaplamaSecenegi.Value.ToString());

                        switch (model.GenelBilgiler.HesaplamaSecenegi.Value)
                        {
                            case ROL_HesaplamaSecenekleri.YillikPrim:
                                if (model.GenelBilgiler.Ortak_PrimTutari.HasValue)
                                    teklif.AddSoru(OdulluBirikimSorular.YillikPrimTutari, model.GenelBilgiler.Ortak_PrimTutari.Value.ToString());
                                break;
                            case ROL_HesaplamaSecenekleri.SureSonuBirikim:
                                if (model.GenelBilgiler.Ortak_PrimTutari.HasValue)
                                    teklif.AddSoru(OdulluBirikimSorular.SureSonuPrimIadesiTeminati, model.GenelBilgiler.Ortak_PrimTutari.Value.ToString());
                                break;
                        }
                    }

                    #endregion

                    #endregion

                    #region Teklif return

                    IOdulluBirikimTeklif odulluBirikimTeklif = new OdulluBirikimTeklif();

                    odulluBirikimTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.AEGON);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                    odulluBirikimTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    IsDurum isDurum = odulluBirikimTeklif.Hesapla(teklif);
                    #endregion

                    return Json(new { id = isDurum.IsId, g = isDurum.Guid, tid = teklif.GenelBilgiler.TeklifId });
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
            DetayOdulluBirikimModel model = new DetayOdulluBirikimModel();

            #region Teklif Genel


            OdulluBirikimTeklif odulluBirikimTeklif = new OdulluBirikimTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = odulluBirikimTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(odulluBirikimTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.AegonDetayMusteriModel(odulluBirikimTeklif.Teklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Ödüllü Birikim

            model.GenelBilgiler = OdulluBirikimGenelBilgilerDetay(odulluBirikimTeklif.Teklif);

            #endregion

            #region Teklif Fiyat

            model.Fiyat = OdulluBirikimFiyat(odulluBirikimTeklif);

            #endregion

            #region Provizyon

            model.OnProvizyon = _TeklifService.AegonOnProvizyonKontrol(id);

            #endregion

            return View(model);
        }

        private TeklifFiyatModel OdulluBirikimFiyat(OdulluBirikimTeklif odulluBirikimTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = odulluBirikimTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = odulluBirikimTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = odulluBirikimTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = odulluBirikimTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = odulluBirikimTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = odulluBirikimTeklif.GetIsDurum();
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
        public ActionResult OdemeAl(OdemeOdulluBirikimModel model)
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
                        this.SendMuhasebe(urun);
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

        //------------EKLE
        private OdulluBirikimGenelBilgiler OdulluBirikimGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            OdulluBirikimGenelBilgiler model = new OdulluBirikimGenelBilgiler();

            try
            {
                model.ParaBirimleri = new SelectList(TeklifProvider.ParaBirimleriAEGON(), "Value", "Text", "");
                model.PrimDonemleri = new SelectList(TeklifProvider.PrimDonemleriAEGON(), "Value", "Text", "");
                model.HesaplamaSecenegiTipleri = new SelectList(TeklifProvider.HesaplamaSecenegiTipleriAEGON(), "Value", "Text", "");

                if (teklifId.HasValue && teklif != null)
                {
                    //Sigorta Başlangıç Tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Sigorta süresi (yıl)
                    string sigortaSuresi = teklif.ReadSoru(OdulluBirikimSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.SigortaSuresi = Convert.ToByte(sigortaSuresi);

                    //Prim Ödeme Dönemi 
                    string primDonemi = teklif.ReadSoru(OdulluBirikimSorular.PrimOdemeDonemi, String.Empty);
                    if (!String.IsNullOrEmpty(primDonemi))
                        model.PrimOdemeDonemi = Convert.ToByte(primDonemi);

                    //Hesaplama Seçeneği 
                    string hesaplamaSecenegi = teklif.ReadSoru(OdulluBirikimSorular.HesaplamaSecenegi, String.Empty);
                    if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                    {
                        model.HesaplamaSecenegi = Convert.ToByte(hesaplamaSecenegi);
                        switch (model.HesaplamaSecenegi)
                        {
                            case ROL_HesaplamaSecenekleri.YillikPrim:
                                model.Ortak_PrimTutari = teklif.ReadSoru(OdulluBirikimSorular.YillikPrimTutari, decimal.Zero);
                                break;
                            case ROL_HesaplamaSecenekleri.SureSonuBirikim:
                                model.Ortak_PrimTutari = teklif.ReadSoru(OdulluBirikimSorular.SureSonuPrimIadesiTeminati, decimal.Zero);
                                break;
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

        //----------------DETAY 
        private OdulluBirikimGenelBilgiler OdulluBirikimGenelBilgilerDetay(ITeklif teklif)
        {
            OdulluBirikimGenelBilgiler model = new OdulluBirikimGenelBilgiler();

            try
            {
                if (teklif != null)
                {
                    //Sigorta Başlangıç Tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue);
                    model.PrimOdemeDonemiText = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(OdulluBirikimSorular.PrimOdemeDonemi, String.Empty));

                    //Sigorta süresi (yıl)
                    string sigortaSuresi = teklif.ReadSoru(OdulluBirikimSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.SigortaSuresi = Convert.ToByte(sigortaSuresi);

                    //Hesaplama Seçeneği 
                    string hesaplamaSecenegi = teklif.ReadSoru(OdulluBirikimSorular.HesaplamaSecenegi, String.Empty);
                    if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                    {
                        model.HesaplamaSecenegi = Convert.ToByte(hesaplamaSecenegi);
                        model.HesaplamaSecenegiText = ROL_HesaplamaSecenekleri.GetHesaplamaSecenegi(Convert.ToInt32(hesaplamaSecenegi));
                        switch (model.HesaplamaSecenegi)
                        {
                            case ROL_HesaplamaSecenekleri.YillikPrim:
                                model.Ortak_PrimTutari = teklif.ReadSoru(OdulluBirikimSorular.YillikPrimTutari, decimal.Zero);
                                break;
                            case ROL_HesaplamaSecenekleri.SureSonuBirikim:
                                model.Ortak_PrimTutari = teklif.ReadSoru(OdulluBirikimSorular.SureSonuPrimIadesiTeminati, decimal.Zero);
                                break;
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

        private int OB_MusteriKaydet(OdulluBirikimModel model)
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
        public JsonResult GetMinimumYillikPrimTutari(decimal? primTutari)
        {
            try
            {
                int minYillikPrimLimiti = 2400;
                if (!primTutari.HasValue || primTutari < minYillikPrimLimiti)
                {
                    return Json(new
                    {
                        Success = "false",
                        Message = "Yıllık prim, minimum yıllık prim limiti olan " + minYillikPrimLimiti + " TL’nin altındadır"
                    }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Success = "true", Message = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                return Json(new { Success = "false", Message = "Prim limiti hesaplanırken bir sorun oluştu lütfen tekrar deneyiniz." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AjaxException]
        public JsonResult SigortaSuresiHesapla(DateTime dogumTarihi, DateTime sigortaBaslangic, int sure)
        {
            try
            {
                if (sigortaBaslangic < DateTime.Today)
                    return Json(new { Success = "false", Message = "Sigorta başlangıç tarihi olarak geçmiş bir tarih girilemez" });

                int sigortaSuresiMin = 10;
                int sigortaSuresiMax = 20;

                if (sure < sigortaSuresiMin || sure > sigortaSuresiMax)
                    return Json(new { Success = "false", Message = "Sigorta süresi " + sigortaSuresiMin + " – " + sigortaSuresiMax + " sene arasında olmalıdır" });
                else
                    return Json(new { Success = "true", Message = "" });
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                return Json(new { Success = "true", Message = "Sigorta süresi hesaplanırken bir hata oluştu lütfen tekrar deneyiniz." });
            }
        }

        [HttpPost]
        [AjaxException]
        public ActionResult _DetayPartial(int IsDurum_id)
        {
            OdulluBirikimDetayPartialModel model = new OdulluBirikimDetayPartialModel();

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

                        //Sigorta Başlangıç Tarihi
                        model.SigortaBaslangicTarihi = teklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");

                        //Prim Ödeme Dönemi
                        model.PrimOdemeDonemiText = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(OdulluBirikimSorular.PrimOdemeDonemi, String.Empty));

                        //Sigorta Suresi
                        model.SigortaSuresiText = teklif.ReadSoru(OdulluBirikimSorular.SigortaSuresi, String.Empty);

                        //Hesaplama Seçeneği 
                        string hesaplamaSecenegi = teklif.ReadSoru(OdulluBirikimSorular.HesaplamaSecenegi, String.Empty);
                        if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                        {
                            model.HesaplamaSecenegi = ROL_HesaplamaSecenekleri.GetHesaplamaSecenegi(Convert.ToInt32(hesaplamaSecenegi));
                            switch (Convert.ToInt32(hesaplamaSecenegi))
                            {
                                case ROL_HesaplamaSecenekleri.YillikPrim:
                                    model.Tutar = teklif.ReadSoru(OdulluBirikimSorular.YillikPrimTutari, decimal.Zero).ToString("N2") + " TL";
                                    break;
                                case ROL_HesaplamaSecenekleri.SureSonuBirikim:
                                    model.Tutar = teklif.ReadSoru(OdulluBirikimSorular.SureSonuPrimIadesiTeminati, decimal.Zero).ToString("N2") + " TL";
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return PartialView(model);
        }
    }
}
