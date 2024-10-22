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
using Neosinerji.BABOnlineTP.Business.AEGON;
using System.Globalization;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.TESabitPrimli)]
    public class TESabitPrimliController : TeklifController
    {
        private IUrunParametreleriService _UrunParametreleriService;

        public TESabitPrimliController(ITVMService tvmService,
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
            _UrunParametreleriService = DependencyResolver.Current.GetService<IUrunParametreleriService>();
        }


        public ActionResult Ekle(int? id)
        {
            TESabitPrimliModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            TESabitPrimliModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public TESabitPrimliModel EkleModel(int? id, int? teklifId)
        {
            TESabitPrimliModel model = new TESabitPrimliModel();
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

                #endregion

                #region CONST

                if (_UrunParametreleriService != null)
                {
                    UrunParamsTable parametreler = _UrunParametreleriService.GetListUrunParametre(UrunParametreleriModel.Bundle_AEGON_TE);

                    if (parametreler != null && parametreler.Count > 0)
                    {
                        //model.AEGON_HTED_USD_KUR_PARAM = parametreler[UrunParametreleriModel.AEGON_HTED_USD_KUR_PARAM];
                        //model.AEGON_HTED_EUR_KUR_PARAM = parametreler[UrunParametreleriModel.AEGON_HTED_EUR_KUR_PARAM];
                        //model.AEGON_KHYT_USD_KUR_PARAM = parametreler[UrunParametreleriModel.AEGON_KHYT_USD_KUR_PARAM];
                        //model.AEGON_KHYT_EUR_KUR_PARAM = parametreler[UrunParametreleriModel.AEGON_KHYT_EUR_KUR_PARAM];
                        //model.AEGON_KSVT_TTAKSV_ANAT_TOPLAM_EUR = parametreler[UrunParametreleriModel.AEGON_KSVT_TTAKSV_ANAT_TOPLAM_EUR];
                        //model.AEGON_KSVT_TTAKSV_ANAT_TOPLAM_USD = parametreler[UrunParametreleriModel.AEGON_KSVT_TTAKSV_ANAT_TOPLAM_USD];
                        model.AEGON_TE_AnaTeminatLimiti_Dolar = parametreler[UrunParametreleriModel.AEGON_TE_AnaTeminatLimiti_Dolar];
                        model.AEGON_TE_AnaTeminatLimiti_Euro = parametreler[UrunParametreleriModel.AEGON_TE_AnaTeminatLimiti_Euro];
                        model.AEGON_TE_YillikPrimLimiti_Dolar = parametreler[UrunParametreleriModel.AEGON_TE_YillikPrimLimiti_Dolar];
                        model.AEGON_TE_YillikPrimLimiti_Euro = parametreler[UrunParametreleriModel.AEGON_TE_YillikPrimLimiti_Euro];
                    }
                }

                #endregion

                #region TESabitPirimli

                model.GenelBilgiler = TESabitPrimliGenelBilgiler(teklifId, teklif);
                model.AnaTeminatlar = TESabitPrimliAnaTeminatlar(teklifId, teklif);
                model.EkTeminatlar = TESabitPrimliEkTeminatlar(teklifId, teklif);

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
        public ActionResult Hesapla(TESabitPrimliModel model)
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


                    if (model.AnaTeminatlar != null && model.EkTeminatlar != null)
                    {
                        //EK TEMINATLAR
                        if (!model.EkTeminatlar.KritikHastaliklar)
                        {
                            if (ModelState["EkTeminatlar.KritikHastaliklarSigortaBedeli"] != null)
                                ModelState["EkTeminatlar.KritikHastaliklarSigortaBedeli"].Errors.Clear();
                        }

                        if (!model.EkTeminatlar.KazaSonucuVefat)
                        {
                            if (ModelState["EkTeminatlar.KazaSonucuVefatSigortaBedeli"] != null)
                                ModelState["EkTeminatlar.KazaSonucuVefatSigortaBedeli"].Errors.Clear();
                        }

                        if (!model.EkTeminatlar.MaluliyetYillikDestek)
                        {
                            if (ModelState["EkTeminatlar.MaluliyetYillikDestekSigortaBedeli"] != null)
                                ModelState["EkTeminatlar.MaluliyetYillikDestekSigortaBedeli"].Errors.Clear();
                        }

                        if (!model.EkTeminatlar.TamVeDaimiMaluliyet)
                        {
                            if (ModelState["EkTeminatlar.TamVeDaimiMaluliyetSigortaBedeli"] != null)
                                ModelState["EkTeminatlar.TamVeDaimiMaluliyetSigortaBedeli"].Errors.Clear();
                        }

                        if (!model.EkTeminatlar.TopluTasimaAraclariKSV)
                        {
                            if (ModelState["EkTeminatlar.TopluTasimaAraclariKSVSigortaBedeli"] != null)
                                ModelState["EkTeminatlar.TopluTasimaAraclariKSVSigortaBedeli"].Errors.Clear();
                        }

                        if (!model.EkTeminatlar.KazaSonucu_TedaviMasraflari)
                        {
                            if (ModelState["EkTeminatlar.KazaSonucu_TedaviMasraflariBedeli"] != null)
                                ModelState["EkTeminatlar.KazaSonucu_TedaviMasraflariBedeli"].Errors.Clear();
                        }

                        if (!model.EkTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme)
                        {
                            if (ModelState["EkTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli"] != null)
                                ModelState["EkTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli"].Errors.Clear();
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

            #region Teklif kaydı ve hesaplamanın başlatılması
            if (ModelState.IsValid)
            {
                try
                {
                    int musteriKodu = TE_MusteriKaydet(model);
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.TESabitPrimli, model.Hazirlayan.TVMKodu,
                                                                         _AktifKullaniciService.KullaniciKodu, musteriKodu, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Gerekli Girişler ve Hata Uyarıları

                    //if (model.AnaTeminatlar.Vefat && model.AnaTeminatlar.Vefat_KritikHastalik)
                    //    return Json(new { id = 0, hata = "Ana teminatlardan yanlızca 1 tanesini seçmelisiniz." });


                    #endregion

                    #region Sigortali

                    teklif.AddSigortali(musteriKodu);

                    #endregion

                    #region TESabitPrimli

                    #region Genel Bilgiler

                    teklif.AddSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, model.GenelBilgiler.SigortaBaslangicTarihi);

                    if (model.GenelBilgiler.ParaBirimi.HasValue)
                        teklif.AddSoru(TESabitPrimliSorular.ParaBirimi, model.GenelBilgiler.ParaBirimi.Value.ToString());

                    if (model.GenelBilgiler.PrimOdemeDonemi.HasValue)
                        teklif.AddSoru(TESabitPrimliSorular.PrimOdemeDonemi, model.GenelBilgiler.PrimOdemeDonemi.Value.ToString());

                    if (model.GenelBilgiler.SigortaSuresi.HasValue)
                        teklif.AddSoru(TESabitPrimliSorular.SigortaSuresi, model.GenelBilgiler.SigortaSuresi.Value.ToString());

                    if (model.GenelBilgiler.HesaplamaSecenegi.HasValue)
                        teklif.AddSoru(TESabitPrimliSorular.HesaplamaSecenegi, model.GenelBilgiler.HesaplamaSecenegi.Value.ToString());

                    #endregion

                    #region Teminatlar

                    //ANA TEMINAT SORULAR
                    if (model.AnaTeminatlar.AnaTeminat.HasValue)
                        teklif.AddSoru(TESabitPrimliSorular.AnaTeminat, model.AnaTeminatlar.AnaTeminat.Value.ToString());


                    //EK TEMINAT SORULAR
                    teklif.AddSoru(TESabitPrimliSorular.KritikHastaliklar, model.EkTeminatlar.KritikHastaliklar);
                    teklif.AddSoru(TESabitPrimliSorular.TamVeDaimiMaluliyet, model.EkTeminatlar.TamVeDaimiMaluliyet);
                    teklif.AddSoru(TESabitPrimliSorular.KazaSonucuVefat, model.EkTeminatlar.KazaSonucuVefat);
                    teklif.AddSoru(TESabitPrimliSorular.TopluTasimaAraclariKSV, model.EkTeminatlar.TopluTasimaAraclariKSV);
                    teklif.AddSoru(TESabitPrimliSorular.MaluliyetYillikDestek, model.EkTeminatlar.MaluliyetYillikDestek);
                    teklif.AddSoru(TESabitPrimliSorular.KazaSonucu_TedaviMasraflari, model.EkTeminatlar.KazaSonucu_TedaviMasraflari);
                    teklif.AddSoru(TESabitPrimliSorular.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme, model.EkTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme);


                    // YILLIK PRIM - ANA TEMINAT
                    if (model.AnaTeminatlar.AnaTeminat.HasValue)
                    {
                        if (model.GenelBilgiler.HesaplamaSecenegi == TESabitPrimliHesaplamaSecenegi.YillikPrim)
                        {
                            teklif.AddSoru(TESabitPrimliSorular.YillikPrimTutari, model.AnaTeminatlar.AnaTeminatSigortaBedeli.Value.ToString(CultureInfo.InvariantCulture));

                            //ANA TEMINATLAR  (yıllık hesaplama ile teklif alınacak olsa bile Ana teminat bilgisi 0 ile ekleniyor.)
                            if (model.AnaTeminatlar.AnaTeminat.Value == 1)
                                teklif.AddTeminat(TESabitPrimliTeminatlar.Vefat, 0, 0, 0, 0, 0);
                            else if (model.AnaTeminatlar.AnaTeminat.Value == 2)
                                teklif.AddTeminat(TESabitPrimliTeminatlar.Vefat_KritikHastalik, 0, 0, 0, 0, 0);
                        }
                        else if (model.GenelBilgiler.HesaplamaSecenegi == TESabitPrimliHesaplamaSecenegi.AnaTeminatTutari)
                        {
                            if (model.AnaTeminatlar.AnaTeminat.Value == 1)
                                teklif.AddTeminat(TESabitPrimliTeminatlar.Vefat, model.AnaTeminatlar.AnaTeminatSigortaBedeli.Value, 0, 0, 0, 0);
                            else if (model.AnaTeminatlar.AnaTeminat.Value == 2)
                                teklif.AddTeminat(TESabitPrimliTeminatlar.Vefat_KritikHastalik, model.AnaTeminatlar.AnaTeminatSigortaBedeli.Value, 0, 0, 0, 0);
                        }
                    }


                    //EK TEMINATLAR
                    if (model.EkTeminatlar.KritikHastaliklar && model.EkTeminatlar.KritikHastaliklarSigortaBedeli.HasValue)
                        teklif.AddTeminat(TESabitPrimliTeminatlar.KritikHastaliklar, model.EkTeminatlar.KritikHastaliklarSigortaBedeli.Value, 0, 0, 0, 0);

                    if (model.EkTeminatlar.TamVeDaimiMaluliyet && model.EkTeminatlar.TamVeDaimiMaluliyetSigortaBedeli.HasValue)
                        teklif.AddTeminat(TESabitPrimliTeminatlar.TamVeDaimiMaluliyet, model.EkTeminatlar.TamVeDaimiMaluliyetSigortaBedeli.Value, 0, 0, 0, 0);

                    if (model.EkTeminatlar.KazaSonucuVefat && model.EkTeminatlar.KazaSonucuVefatSigortaBedeli.HasValue)
                        teklif.AddTeminat(TESabitPrimliTeminatlar.KazaSonucuVefat, model.EkTeminatlar.KazaSonucuVefatSigortaBedeli.Value, 0, 0, 0, 0);

                    if (model.EkTeminatlar.TopluTasimaAraclariKSV && model.EkTeminatlar.TopluTasimaAraclariKSVSigortaBedeli.HasValue)
                        teklif.AddTeminat(TESabitPrimliTeminatlar.TopluTasimaAraclariKSV, model.EkTeminatlar.TopluTasimaAraclariKSVSigortaBedeli.Value, 0, 0, 0, 0);

                    if (model.EkTeminatlar.MaluliyetYillikDestek && model.EkTeminatlar.MaluliyetYillikDestekSigortaBedeli.HasValue)
                        teklif.AddTeminat(TESabitPrimliTeminatlar.MaluliyetYillikDestek, model.EkTeminatlar.MaluliyetYillikDestekSigortaBedeli.Value, 0, 0, 0, 0);

                    if (model.EkTeminatlar.KazaSonucu_TedaviMasraflari && model.EkTeminatlar.KazaSonucu_TedaviMasraflariBedeli.HasValue)
                        teklif.AddTeminat(TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati,
                                          model.EkTeminatlar.KazaSonucu_TedaviMasraflariBedeli.Value, 0, 0, 0, 0);

                    if (model.EkTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme && model.EkTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli.HasValue)
                        teklif.AddTeminat(TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme,
                                          model.EkTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli.Value, 0, 0, 0, 0);

                    #endregion

                    #endregion

                    #region Teklif return

                    ITESabitPrimliTeklif teSabitPrimliTeklif = new TESabitPrimliTeklif();

                    teSabitPrimliTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.AEGON);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                    teSabitPrimliTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    IsDurum isDurum = teSabitPrimliTeklif.Hesapla(teklif);

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
            DetayTESabitPrimliModel model = new DetayTESabitPrimliModel();

            #region Teklif Genel

            TESabitPrimliTeklif teSabitPirimliTeklif = new TESabitPrimliTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = teSabitPirimliTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(teSabitPirimliTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.AegonDetayMusteriModel(teSabitPirimliTeklif.Teklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region TESabitPirimli

            model.GenelBilgiler = TESabitPrimliGenelBilgilerDetay(teSabitPirimliTeklif.Teklif);
            model.AnaTeminatlar = TESabitPrimliAnaTeminatlarDetay(teSabitPirimliTeklif.Teklif);
            model.EkTeminatlar = TESabitPrimliEkTeminatlarDetay(teSabitPirimliTeklif.Teklif);

            #endregion

            #region Teklif Fiyat

            model.Fiyat = TESabitPrimliFiyat(teSabitPirimliTeklif);

            #endregion

            #region Provizyon

            model.OnProvizyon = _TeklifService.AegonOnProvizyonKontrol(id);

            #endregion

            return View(model);
        }

        private TeklifFiyatModel TESabitPrimliFiyat(TESabitPrimliTeklif teSabitPirimliTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = teSabitPirimliTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = teSabitPirimliTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = teSabitPirimliTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = teSabitPirimliTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = teSabitPirimliTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = teSabitPirimliTeklif.GetIsDurum();
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

        //------------EKLE
        private TESabitPrimliGenelBilgiler TESabitPrimliGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            TESabitPrimliGenelBilgiler model = new TESabitPrimliGenelBilgiler();

            try
            {
                model.ParaBirimleri = new SelectList(TeklifProvider.ParaBirimleriAEGON(), "Value", "Text", "");
                model.PrimDonemleri = new SelectList(TeklifProvider.PrimDonemleriAEGON(), "Value", "Text", "");
                model.HesaplamaSecenekleri = new SelectList(TeklifProvider.TuruncuElmaHesaplamaSecenegiTipleri(), "Value", "Text");

                if (teklifId.HasValue && teklif != null)
                {
                    string paraBirimi = teklif.ReadSoru(TESabitPrimliSorular.ParaBirimi, String.Empty);
                    if (!String.IsNullOrEmpty(paraBirimi))
                        model.ParaBirimi = Convert.ToByte(paraBirimi);

                    string hesaplamaSecenegi = teklif.ReadSoru(TESabitPrimliSorular.HesaplamaSecenegi, String.Empty);
                    if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                        model.HesaplamaSecenegi = Convert.ToInt32(hesaplamaSecenegi);

                    string primDonemi = teklif.ReadSoru(TESabitPrimliSorular.PrimOdemeDonemi, String.Empty);
                    if (!String.IsNullOrEmpty(primDonemi))
                        model.PrimOdemeDonemi = Convert.ToByte(primDonemi);

                    model.SigortaBaslangicTarihi = teklif.ReadSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    string sigortaSuresi = teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);
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

        private TESabitPrimliAnaTeminatlar TESabitPrimliAnaTeminatlar(int? teklifId, ITeklif teklif)
        {
            TESabitPrimliAnaTeminatlar model = new TESabitPrimliAnaTeminatlar();

            try
            {
                model.AnaTeminatlar = new SelectList(new List<SelectListItem>()
                                                {
                                                new SelectListItem() { Value = "", Text = "Lütfen Seçiniz" },
                                                new SelectListItem() { Value = "1", Text = "Vefat" },
                                                new SelectListItem() { Value = "2", Text = "Vefat veya Kritik Hastalık" }
                                                }, "Value", "Text");

                if (teklifId.HasValue && teklif != null)
                {

                    string teminatKodu = teklif.ReadSoru(TESabitPrimliSorular.AnaTeminat, String.Empty);
                    string hesaplamaSecenegi = teklif.ReadSoru(TESabitPrimliSorular.HesaplamaSecenegi, String.Empty);

                    if (!String.IsNullOrEmpty(teminatKodu))
                    {
                        model.AnaTeminat = Convert.ToByte(teminatKodu);

                        if (hesaplamaSecenegi == "1")
                        {
                            if (model.AnaTeminat.Value == 1)
                            {
                                TeklifTeminat vefat = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat).FirstOrDefault();
                                if (vefat != null)
                                    model.AnaTeminatSigortaBedeli = vefat.TeminatBedeli;
                            }
                            else if (model.AnaTeminat.Value == 2)
                            {
                                TeklifTeminat vefatKHastalik = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat_KritikHastalik).FirstOrDefault();
                                if (vefatKHastalik != null)
                                    model.AnaTeminatSigortaBedeli = vefatKHastalik.TeminatBedeli;
                            }
                        }
                        else
                        {
                            model.AnaTeminatSigortaBedeli = teklif.ReadSoru(TESabitPrimliSorular.YillikPrimTutari, decimal.Zero);
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

        private TESabitPrimliEkTeminatlar TESabitPrimliEkTeminatlar(int? teklifId, ITeklif teklif)
        {
            TESabitPrimliEkTeminatlar model = new TESabitPrimliEkTeminatlar();

            try
            {
                if (teklifId.HasValue && teklif != null)
                {
                    if (teklif.ReadSoru(TESabitPrimliSorular.KritikHastaliklar, false))
                    {
                        TeklifTeminat KritikHastaliklar = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.KritikHastaliklar);
                        if (KritikHastaliklar != null)
                        {
                            model.KritikHastaliklar = true;
                            if (KritikHastaliklar.TeminatBedeli.HasValue)
                                model.KritikHastaliklarSigortaBedeli = KritikHastaliklar.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.TamVeDaimiMaluliyet, false))
                    {
                        TeklifTeminat TamVeDaimiMaluliyet = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.TamVeDaimiMaluliyet);
                        if (TamVeDaimiMaluliyet != null)
                        {
                            model.TamVeDaimiMaluliyet = true;
                            if (TamVeDaimiMaluliyet.TeminatBedeli.HasValue)
                                model.TamVeDaimiMaluliyetSigortaBedeli = TamVeDaimiMaluliyet.TeminatBedeli;
                        }
                    }


                    if (teklif.ReadSoru(TESabitPrimliSorular.KazaSonucuVefat, false))
                    {
                        TeklifTeminat KazaSonucuVefat = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucuVefat);
                        if (KazaSonucuVefat != null)
                        {
                            model.KazaSonucuVefat = true;
                            if (KazaSonucuVefat.TeminatBedeli.HasValue)
                                model.KazaSonucuVefatSigortaBedeli = KazaSonucuVefat.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.TopluTasimaAraclariKSV, false))
                    {
                        TeklifTeminat TopluTasimaAraclariKSV = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.TopluTasimaAraclariKSV);
                        if (TopluTasimaAraclariKSV != null)
                        {
                            model.TopluTasimaAraclariKSV = true;
                            if (TopluTasimaAraclariKSV.TeminatBedeli.HasValue)
                                model.TopluTasimaAraclariKSVSigortaBedeli = TopluTasimaAraclariKSV.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.MaluliyetYillikDestek, false))
                    {
                        TeklifTeminat MaluliyetYillikDestek = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.MaluliyetYillikDestek);
                        if (MaluliyetYillikDestek != null)
                        {
                            model.MaluliyetYillikDestek = true;
                            if (MaluliyetYillikDestek.TeminatBedeli.HasValue)
                                model.MaluliyetYillikDestekSigortaBedeli = MaluliyetYillikDestek.TeminatBedeli;
                        }
                    }

                    //Yeni Ek Teminatlar
                    if (teklif.ReadSoru(TESabitPrimliSorular.KazaSonucu_TedaviMasraflari, false))
                    {
                        TeklifTeminat KazaSonucuTedaviMasraflari = teklif.Teminatlar
                                                                    .FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati);
                        if (KazaSonucuTedaviMasraflari != null)
                        {
                            model.KazaSonucu_TedaviMasraflari = true;
                            if (KazaSonucuTedaviMasraflari.TeminatBedeli.HasValue)
                                model.KazaSonucu_TedaviMasraflariBedeli = KazaSonucuTedaviMasraflari.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme, false))
                    {
                        TeklifTeminat KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu ==
                                                                                        TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme);
                        if (KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme != null)
                        {
                            model.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme = true;
                            if (KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme.TeminatBedeli.HasValue)
                                model.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli = KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme.TeminatBedeli;
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
        private TESabitPrimliGenelBilgiler TESabitPrimliGenelBilgilerDetay(ITeklif teklif)
        {
            TESabitPrimliGenelBilgiler model = new TESabitPrimliGenelBilgiler();

            try
            {
                if (teklif != null)
                {
                    model.ParaBirimiText = TESabitPrimliParaBirimi.ParaBirimiText(teklif.ReadSoru(TESabitPrimliSorular.ParaBirimi, String.Empty));
                    model.HesaplamaSecenegi = Convert.ToInt32(teklif.ReadSoru(TESabitPrimliSorular.HesaplamaSecenegi, String.Empty));
                    model.HesaplamaSecenegiText = TESabitPrimliHesaplamaSecenegi.GetText(model.HesaplamaSecenegi.Value.ToString());
                    model.PrimOdemeDonemiText = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(TESabitPrimliSorular.PrimOdemeDonemi, String.Empty));
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, DateTime.MinValue);


                    string sigortaSuresi = teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);
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

        private TESabitPrimliAnaTeminatlar TESabitPrimliAnaTeminatlarDetay(ITeklif teklif)
        {
            TESabitPrimliAnaTeminatlar model = new TESabitPrimliAnaTeminatlar();

            if (teklif != null)
            {
                string teminatKodu = teklif.ReadSoru(TESabitPrimliSorular.AnaTeminat, String.Empty);

                model.AnaTeminatText = TESPAnaTeminatlar.AnaTeminatText(teminatKodu);

                if (!String.IsNullOrEmpty(teminatKodu))
                {
                    if (teminatKodu == "1")
                    {
                        TeklifTeminat vefat = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat).FirstOrDefault();
                        if (vefat != null)
                            model.AnaTeminatSigortaBedeli = vefat.TeminatBedeli;
                    }
                    else if (teminatKodu == "2")
                    {
                        TeklifTeminat vefatKHastalik = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat_KritikHastalik).FirstOrDefault();
                        if (vefatKHastalik != null)
                            model.AnaTeminatSigortaBedeli = vefatKHastalik.TeminatBedeli;
                    }
                }
            }

            return model;
        }

        private TESabitPrimliEkTeminatlar TESabitPrimliEkTeminatlarDetay(ITeklif teklif)
        {
            TESabitPrimliEkTeminatlar model = new TESabitPrimliEkTeminatlar();

            try
            {
                if (teklif != null)
                {
                    if (teklif.ReadSoru(TESabitPrimliSorular.KritikHastaliklar, false))
                    {
                        TeklifTeminat KritikHastaliklar = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.KritikHastaliklar);
                        if (KritikHastaliklar != null)
                        {
                            model.KritikHastaliklar = true;
                            if (KritikHastaliklar.TeminatBedeli.HasValue)
                                model.KritikHastaliklarSigortaBedeli = KritikHastaliklar.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.TamVeDaimiMaluliyet, false))
                    {
                        TeklifTeminat TamVeDaimiMaluliyet = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.TamVeDaimiMaluliyet);
                        if (TamVeDaimiMaluliyet != null)
                        {
                            model.TamVeDaimiMaluliyet = true;
                            if (TamVeDaimiMaluliyet.TeminatBedeli.HasValue)
                                model.TamVeDaimiMaluliyetSigortaBedeli = TamVeDaimiMaluliyet.TeminatBedeli;
                        }
                    }


                    if (teklif.ReadSoru(TESabitPrimliSorular.KazaSonucuVefat, false))
                    {
                        TeklifTeminat KazaSonucuVefat = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucuVefat);
                        if (KazaSonucuVefat != null)
                        {
                            model.KazaSonucuVefat = true;
                            if (KazaSonucuVefat.TeminatBedeli.HasValue)
                                model.KazaSonucuVefatSigortaBedeli = KazaSonucuVefat.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.TopluTasimaAraclariKSV, false))
                    {
                        TeklifTeminat TopluTasimaAraclariKSV = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.TopluTasimaAraclariKSV);
                        if (TopluTasimaAraclariKSV != null)
                        {
                            model.TopluTasimaAraclariKSV = true;
                            if (TopluTasimaAraclariKSV.TeminatBedeli.HasValue)
                                model.TopluTasimaAraclariKSVSigortaBedeli = TopluTasimaAraclariKSV.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.MaluliyetYillikDestek, false))
                    {
                        TeklifTeminat MaluliyetYillikDestek = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.MaluliyetYillikDestek);
                        if (MaluliyetYillikDestek != null)
                        {
                            model.MaluliyetYillikDestek = true;
                            if (MaluliyetYillikDestek.TeminatBedeli.HasValue)
                                model.MaluliyetYillikDestekSigortaBedeli = MaluliyetYillikDestek.TeminatBedeli;
                        }
                    }

                    //Yeni Ek Teminatlar
                    if (teklif.ReadSoru(TESabitPrimliSorular.KazaSonucu_TedaviMasraflari, false))
                    {
                        TeklifTeminat KazaSonucu_TedaviMasraflari = teklif.Teminatlar
                                                            .FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati);
                        if (KazaSonucu_TedaviMasraflari != null)
                        {
                            model.KazaSonucu_TedaviMasraflari = true;
                            if (KazaSonucu_TedaviMasraflari.TeminatBedeli.HasValue)
                                model.KazaSonucu_TedaviMasraflariBedeli = KazaSonucu_TedaviMasraflari.TeminatBedeli;
                        }
                    }

                    if (teklif.ReadSoru(TESabitPrimliSorular.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme, false))
                    {
                        TeklifTeminat KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme = teklif.Teminatlar
                                                            .FirstOrDefault(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme);
                        if (KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme != null)
                        {
                            model.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme = true;
                            if (KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme.TeminatBedeli.HasValue)
                                model.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli = KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme.TeminatBedeli;
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

        private int TE_MusteriKaydet(TESabitPrimliModel model)
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
        public JsonResult GetAnaTeminatLimit()
        {
            try
            {
                if (_UrunParametreleriService != null)
                {
                    string dolar = _UrunParametreleriService.GetUrunParametreValue(UrunParametreleriModel.AEGON_TE_AnaTeminatLimiti_Dolar);
                    string euro = _UrunParametreleriService.GetUrunParametreValue(UrunParametreleriModel.AEGON_TE_AnaTeminatLimiti_Euro);
                    return Json(new { dolar = dolar, avro = euro }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return Json(new { dolar = 10000, avro = 7500 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxException]
        public JsonResult GetYillikPrimLimit()
        {
            try
            {
                if (_UrunParametreleriService != null)
                {
                    string dolar = _UrunParametreleriService.GetUrunParametreValue(UrunParametreleriModel.AEGON_TE_YillikPrimLimiti_Dolar);
                    string euro = _UrunParametreleriService.GetUrunParametreValue(UrunParametreleriModel.AEGON_TE_YillikPrimLimiti_Euro);
                    return Json(new { dolar = dolar, avro = euro }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return Json(new { dolar = 600, avro = 420 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxException]
        public JsonResult SigortaSuresiHesapla(DateTime dogumTarihi, DateTime sigortaBaslangic, int sure, int kademe)
        {
            if (sigortaBaslangic < DateTime.Today)
                return Json(new { Success = "false", Message = "Sigorta başlangıç tarihi olarak geçmiş bir tarih girilemez" });

            int yas = AEGONTESabitPrimli.AegonYasHesapla(dogumTarihi, sigortaBaslangic);

            if (yas > 17 && yas < 71)
            {
                if (kademe == 1)
                {
                    if (yas < 36)
                        if (sure < 5 || (sure > 5 && sure < 10) || (sure > 10 && sure < 15) || sure > 40)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5,10 veya 15 ile 40 arası olabilir." });

                    int makulYasSiniri = 75 - yas;

                    if (makulYasSiniri == sure)
                        return Json(new { Success = "true", Message = "" });
                    else if (makulYasSiniri < sure)
                    {
                        if (yas < 60)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5,10 veya 15 ile " + makulYasSiniri + " arası olabilir." });

                        if (yas < 65)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5,10," + makulYasSiniri + " olabilir." });

                        if (yas < 70)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5," + makulYasSiniri + " olabilir." });
                        else return Json(new { Success = "false", Message = "Sigorta süresi 5 olabilir." });
                    }
                    else
                    {
                        if (makulYasSiniri > 14)
                        {
                            if (sure < 5 || (sure > 5 && sure < 10) || (sure > 10 && sure < 15))
                                return Json(new { Success = "false", Message = "Sigorta süresi 5,10 veya 15 ile " + makulYasSiniri + " arası olabilir." });
                        }
                        else if (makulYasSiniri > 9)
                        {
                            if (sure < 5 || (sure > 5 && sure < 10))
                                return Json(new { Success = "false", Message = "Sigorta süresi 5,10," + makulYasSiniri + " olabilir." });
                        }
                        else if (makulYasSiniri > 4)
                        {
                            if (sure < 5 || (sure > 5 && sure < 10))
                                return Json(new { Success = "false", Message = "Sigorta süresi 5,10," + makulYasSiniri + " olabilir." });
                        }
                        else if (sure < 5)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5 olabilir." });
                    }
                }
                else if (kademe == 2)
                {
                    if (yas < 26)
                        if (sure < 5 || (sure > 5 && sure < 10) || (sure > 10 && sure < 15) || sure > 40)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5,10 veya 15 ile 40 arası olabilir." });

                    int makulYasSiniri = 65 - yas;

                    if (makulYasSiniri == sure)
                        return Json(new { Success = "true", Message = "" });
                    else if (makulYasSiniri < sure)
                    {
                        if (yas < 50)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5,10 veya 15 ile " + makulYasSiniri + " arası olabilir." });
                        if (yas < 55)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5,10," + makulYasSiniri + " olabilir." });
                        if (yas < 60)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5," + makulYasSiniri + " olabilir." });
                        if (yas == 60)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5 olabilir." });
                        if (yas > 60)
                            return Json(new { Success = "false", Message = "Seçilen teminatlar için minimum limit 60 yaş." });
                    }
                    else
                    {
                        if (makulYasSiniri > 14)
                        {
                            if (sure < 5 || (sure > 5 && sure < 10) || (sure > 10 && sure < 15))
                                return Json(new { Success = "false", Message = "Sigorta süresi 5,10 veya 15 ile " + makulYasSiniri + " arası olabilir." });
                        }
                        else if (makulYasSiniri > 9)
                        {
                            if (sure < 5 || (sure > 5 && sure < 10))
                                return Json(new { Success = "false", Message = "Sigorta süresi 5,10," + makulYasSiniri + " olabilir." });
                        }
                        else if (makulYasSiniri > 4)
                        {
                            if (sure < 5 || (sure > 5 && sure < 10))
                                return Json(new { Success = "false", Message = "Sigorta süresi 5," + makulYasSiniri + " olabilir." });
                        }
                        else if (sure < 5)
                            return Json(new { Success = "false", Message = "Sigorta süresi 5 olabilir." });
                    }
                }
                else return Json(new { Success = "false", Message = "Kademe bilgisi alınamadı." });
                return Json(new { Success = "true", Message = "" });
            }
            else
                return Json(new { Success = "false", Message = "Hesaplanan sigortalı yaşı 18 - 71 arası olmalıdır." });
        }

        [HttpPost]
        [AjaxException]
        public ActionResult _DetayPartial(int IsDurum_id)
        {
            TEDetayPartialModel model = new TEDetayPartialModel();

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

                        model.SigortaBaslangicTarihi = teklif.ReadSoru(TESabitPrimliSorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");
                        model.SigortaSuresi = teklif.ReadSoru(TESabitPrimliSorular.SigortaSuresi, String.Empty);
                        model.PrimOdemeDonemi = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(TESabitPrimliSorular.PrimOdemeDonemi, String.Empty));
                        model.HesaplamaSecenegi = teklif.ReadSoru(TESabitPrimliSorular.HesaplamaSecenegi, String.Empty);
                        model.HesaplamaSecenegiText = TESabitPrimliHesaplamaSecenegi.GetText(model.HesaplamaSecenegi);

                        string paraBirimi = "";

                        switch (teklif.ReadSoru(TESabitPrimliSorular.ParaBirimi, String.Empty))
                        {
                            case "1": paraBirimi = "€"; break;
                            case "2": paraBirimi = "$"; break;
                        }

                        switch (model.HesaplamaSecenegi)
                        {
                            case "1":
                                switch (teklif.ReadSoru(TESabitPrimliSorular.AnaTeminat, String.Empty))
                                {
                                    case "1":
                                        TeklifTeminat vefat = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat).FirstOrDefault();
                                        if (vefat != null && vefat.TeminatBedeli.HasValue)
                                        {
                                            model.AnaTeminatAdi = "Vefat";
                                            model.AnaTeminatTutari = vefat.TeminatBedeli.Value.ToString("N2") + " " + paraBirimi;
                                        }
                                        break;
                                    case "2":
                                        TeklifTeminat vefat_Krt = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.Vefat_KritikHastalik).FirstOrDefault();
                                        if (vefat_Krt != null && vefat_Krt.TeminatBedeli.HasValue)
                                        {
                                            model.AnaTeminatAdi = "Vefat veya Kritik Hastalık";
                                            model.AnaTeminatTutari = vefat_Krt.TeminatBedeli.Value.ToString("N2") + " " + paraBirimi;
                                        }
                                        break;
                                }
                                break;
                            case "2":
                                model.YillikPrim = teklif.ReadSoru(TESabitPrimliSorular.YillikPrimTutari, decimal.Zero).ToString("N2") + " " + paraBirimi;
                                break;
                        }

                        #region Ek Teminatlar

                        TeklifTeminat KritikHastalik = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KritikHastaliklar).FirstOrDefault();
                        if (KritikHastalik != null && KritikHastalik.TeminatBedeli.HasValue)
                        {
                            if (KritikHastalik.TeminatBedeli.Value > 0)
                                model.KritikHastalikTutar = KritikHastalik.TeminatBedeli.Value.ToString("N0") + " %";
                        }

                        TeklifTeminat KazaSonucuVefat = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucuVefat).FirstOrDefault();
                        if (KazaSonucuVefat != null && KazaSonucuVefat.TeminatBedeli.HasValue)
                        {
                            if (KazaSonucuVefat.TeminatBedeli.Value > 0)
                                model.KazaSonucuVefat = KazaSonucuVefat.TeminatBedeli.Value.ToString("N0") + " %";
                        }

                        TeklifTeminat TopluTasimaAKSV = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.TopluTasimaAraclariKSV).FirstOrDefault();
                        if (TopluTasimaAKSV != null && TopluTasimaAKSV.TeminatBedeli.HasValue)
                        {
                            if (TopluTasimaAKSV.TeminatBedeli.Value > 0)
                                model.TopluTasimaAraclarindaVefat = TopluTasimaAKSV.TeminatBedeli.Value.ToString("N0") + " %";
                        }

                        TeklifTeminat TamVeDaimiMaluliyet = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.TamVeDaimiMaluliyet).FirstOrDefault();
                        if (TamVeDaimiMaluliyet != null && TamVeDaimiMaluliyet.TeminatBedeli.HasValue)
                        {
                            if (TamVeDaimiMaluliyet.TeminatBedeli.Value > 0)
                                model.TamVeDaimiMaluliyet = TamVeDaimiMaluliyet.TeminatBedeli.Value.ToString("N0") + " %";
                        }

                        TeklifTeminat MaluliyetYillikD = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.MaluliyetYillikDestek).FirstOrDefault();
                        if (MaluliyetYillikD != null && MaluliyetYillikD.TeminatBedeli.HasValue)
                        {
                            if (MaluliyetYillikD.TeminatBedeli.Value > 0)
                                model.MaluliyetYillikDestek = MaluliyetYillikD.TeminatBedeli.Value.ToString("N0") + " %";
                        }

                        //YENİ EKLENEN TEMİNATLAR
                        TeklifTeminat KSTMEkTeminati = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_TedaviMasraflari_EkTeminati)
                                                                        .FirstOrDefault();
                        if (KSTMEkTeminati != null && KSTMEkTeminati.TeminatBedeli.HasValue)
                        {
                            if (KSTMEkTeminati.TeminatBedeli.Value > 0)
                                model.KazaSonucu_TedaviMasraflariBedeli = KSTMEkTeminati.TeminatBedeli.Value.ToString("N0") + " %";
                        }

                        TeklifTeminat KSHYTDkO = teklif.Teminatlar.Where(s => s.TeminatKodu == TESabitPrimliTeminatlar.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdeme)
                                                                        .FirstOrDefault();
                        if (KSHYTDkO != null && KSHYTDkO.TeminatBedeli.HasValue)
                        {
                            if (KSHYTDkO.TeminatBedeli.Value > 0)
                                model.KazaSonucu_HastanedeYatarakTD_HaftalikEkOdemeBedeli = KSHYTDkO.TeminatBedeli.Value.ToString("N0") + " %";
                        }

                        #endregion
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
