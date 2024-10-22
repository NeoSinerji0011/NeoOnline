using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.YabanciSaglik;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.TamamlayiciSaglik)]
    public class TamamlayiciSaglikController : TeklifController
    {
        public TamamlayiciSaglikController(ITVMService tvmService,
                                ITeklifService teklifService,
                                IMusteriService musteriService,
                                IKullaniciService kullaniciService,
                                IAktifKullaniciService aktifKullaniciService,
                                ITanimService tanimService,
                                IUlkeService ulkeService,
                                ICRService crService,
                                IAracService aracService,
                                IUrunService urunService,
                                ITUMService tumService
                                )
            : base(tvmService, teklifService, musteriService, kullaniciService, aktifKullaniciService, tanimService, ulkeService, crService, aracService, urunService, tumService)
        {

        }

        public ActionResult Ekle(int? id)
        {
            TamamlayiciSaglikModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            TamamlayiciSaglikModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public TamamlayiciSaglikModel EkleModel(int? id, int? teklifId)
        {
            TamamlayiciSaglikModel model = new TamamlayiciSaglikModel();
            ListModels meslek = new ListModels();
            List<ListModels> meslekList = new List<ListModels>();

            ListModels tarifeGruplari = new ListModels();
            List<ListModels> tarifeGrupList = new List<ListModels>();

            ITURKNIPPONYabanciSaglik request = DependencyResolver.Current.GetService<ITURKNIPPONYabanciSaglik>();

            model.GenelBilgiler = new TSSGenelBilgilerModel();
            model.Musteri = new SigortaliModel();
            model.UrunAdi = "TSS";
            var NipponMeslekList = request.GetMeslekListesi();
            var NipponTarifeGrupList = request.GetTarifeGrupListesi();

            ITeklif teklif = null;
            int? sigortaliMusteriKodu = null;
            bool OdemeSekliPesinMi = true;
            //Teklifi hazırlayan
            model.Hazirlayan = base.EkleHazirlayanModel();

            //Sigorta Ettiren / Sigortalı
            model.Musteri = new SigortaliModel();
            model.Musteri.SigortaliAyni = true;
            model.Musteri.EMailRequired = true;
            if (teklifId.HasValue)
            {
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
            model.GenelBilgiler.TedaviTipi = 1; //Yatarak + Ayak default seçim
            model.GenelBilgiler.PoliceBaslangicTarihi = TurkeyDateTime.Today;
            model.GenelBilgiler.PoliceBitisTarihi = TurkeyDateTime.Today.AddYears(1);

            if (id.HasValue)
            {
                model.TekrarTeklif = true;
                model.Musteri.SigortaEttiren = base.EkleMusteriModel(id.Value);
                model.Musteri.SigortaEttiren.CepTelefonu = model.Musteri.SigortaEttiren.CepTelefonu;
                model.Musteri.Ulkeler = ulkeler;
                model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.Musteri.SigortaEttiren.IlKodu).ListWithOptionLabelIller();
                model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.Musteri.SigortaEttiren.IlKodu), "IlceKodu", "IlceAdi", model.Musteri.SigortaEttiren.IlceKodu).ListWithOptionLabel();

                model.GenelBilgiler.MeslekKodu = teklif.ReadSoru(TSSSorular.Meslek, "");
                model.GenelBilgiler.TarifeKodu = teklif.ReadSoru(TSSSorular.TarifeGrupKodu, "");

                model.GenelBilgiler.Boy = Convert.ToInt32(teklif.ReadSoru(TSSSorular.Boy, 0));
                model.GenelBilgiler.Kilo = Convert.ToInt32(teklif.ReadSoru(TSSSorular.Kilo, 0));
                model.GenelBilgiler.YenilemeMi = teklif.ReadSoru(TSSSorular.YeniIsMi, false);
                model.GenelBilgiler.KronikHastalikCerrahisi = teklif.ReadSoru(TSSSorular.KronikHastalikCerrahisi, false);
                model.GenelBilgiler.TedaviTipi = Convert.ToByte(teklif.ReadSoru(TSSSorular.TedaviTipi, 1));

                var policeBaslangicTarihi = teklif.ReadSoru(TSSSorular.Police_Baslangic_Tarihi, null);
                if (!String.IsNullOrEmpty(policeBaslangicTarihi))
                {
                    model.GenelBilgiler.PoliceBaslangicTarihi = Convert.ToDateTime(policeBaslangicTarihi);
                }
                var policeBitisTarihi = teklif.ReadSoru(TSSSorular.Police_Bitis_Tarihi, null);
                if (!String.IsNullOrEmpty(policeBitisTarihi))
                {
                    model.GenelBilgiler.PoliceBitisTarihi = Convert.ToDateTime(policeBitisTarihi);
                }

                var oncekiPoliceBaslangicTarihi = teklif.ReadSoru(TSSSorular.Eski_Police_Baslangic_Tarihi, null);
                if (!String.IsNullOrEmpty(oncekiPoliceBaslangicTarihi))
                {
                    model.GenelBilgiler.OncekiPoliceBaslangicTarihi = Convert.ToDateTime(oncekiPoliceBaslangicTarihi);
                }
                model.GenelBilgiler.OncekiPoliceNo = teklif.ReadSoru(TSSSorular.Eski_Police_No, "");
                model.GenelBilgiler.OncekiSigortaSirketi = teklif.ReadSoru(TSSSorular.Eski_Police_Sigorta_Sirketi, "");
                model.GenelBilgiler.KronikHastalikAciklama = teklif.ReadSoru(TSSSorular.KronikHastalikAciklama, "");

                var odemeSekli = teklif.GenelBilgiler.OdemeSekli;
                if (odemeSekli == OdemeSekilleri.Pesin)
                {
                    OdemeSekliPesinMi = true;
                }
                else
                {
                    OdemeSekliPesinMi = false;
                }
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

            List<SelectListItem> tedaviTip = new List<SelectListItem>();

            tedaviTip.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Yatarak" }  ,
                new SelectListItem() { Value = "1", Text = "Yatarak + Ayakta" }
            });

            if (NipponMeslekList != null)
            {
                foreach (var item in NipponMeslekList)
                {
                    meslek = new ListModels();
                    meslek.key = item.kodu;
                    meslek.value = item.aciklama;
                    meslekList.Add(meslek);
                }
                model.GenelBilgiler.MeslekKodlari = new SelectList(meslekList, "key", "value", model.GenelBilgiler.MeslekKodu).ListWithOptionLabel();
            }
            else
            {
                model.GenelBilgiler.MeslekKodlari = new List<SelectListItem>();
            }
            model.GenelBilgiler.TarifeKodu = "7";
            if (NipponTarifeGrupList != null)
            {
                foreach (var item in NipponTarifeGrupList)
                {
                    if (item.kodu == "7" || item.kodu == "99")
                    {
                        tarifeGruplari = new ListModels();
                        tarifeGruplari.key = item.kodu;
                        if (item.kodu == "99")
                        {
                            tarifeGruplari.value = item.aciklama;
                        }
                        else
                        {
                            tarifeGruplari.value = item.aciklama;
                        }

                        tarifeGrupList.Add(tarifeGruplari);
                    }
                }
                model.GenelBilgiler.TarifeKodlari = new SelectList(tarifeGrupList, "key", "value", model.GenelBilgiler.TarifeKodu).ListWithOptionLabel();
            }
            else
            {
                model.GenelBilgiler.TarifeKodlari = new List<SelectListItem>();
            }

            model.GenelBilgiler.TedaviTipleri = new SelectList(tedaviTip, "Value", "Text", model.GenelBilgiler.TedaviTipi);

            #region Odeme
            model.Odeme = new TSSTeklifOdemeModel();
            model.Odeme.OdemeSekli = OdemeSekliPesinMi;
            model.Odeme.OdemeTipi = 2;
            model.Odeme.TaksitSayilari = new List<SelectListItem>();

            model.Odeme.OdemeTipleri = new SelectList(TeklifProvider.TSSOdemeTipleri(), "Value", "Text", model.Odeme.OdemeTipi).ToList();

            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "1", Value = "1" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "2", Value = "2" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "3", Value = "3" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "4", Value = "4" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "5", Value = "5" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "6", Value = "6" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "7", Value = "7" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "8", Value = "8" });
            model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = "9", Value = "9" });

            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_OdemeSekli = 1;
            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text="Tek Çekim",Value="1"},
                new SelectListItem(){Text="Taksitli",Value="2"}
            });

            model.KrediKarti.OdemeSekilleri = odemeSekilleri;
            model.KrediKarti.KK_OdemeTipi = 2;
            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.TSSOdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();
            model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
            model.KrediKarti.TaksitSayilari.AddRange(
                new SelectListItem[]{
                new SelectListItem() { Text = "2", Value = "2" },
                new SelectListItem() { Text = "3", Value = "3" },
                new SelectListItem() { Text = "4", Value = "4" },
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "6", Value = "6" },
                new SelectListItem() { Text = "7", Value = "7" },
                new SelectListItem() { Text = "8", Value = "8" },
                new SelectListItem() { Text = "9", Value = "9" }});
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            model.KrediKartiMi = true;
            #endregion

            #region TUM IMAGES

            model.TeklifUM = new TeklifUMListeModel();
            List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.TamamlayiciSaglik);
            foreach (var item in urunyetkileri)
                model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

            #endregion

            return model;
        }

        [HttpPost]
        public ActionResult Hesapla(TamamlayiciSaglikModel model)
        {
            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                ModelStateMusteriClear(ModelState, model.Musteri);

                var Musteri = TSSMusteriKaydet(model);

                #endregion

                #region Teklif kaydı ve hesaplamanın başlatılması
                if (ModelState.IsValid)
                {
                    #region Sigortali
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.TamamlayiciSaglik,
                                                                              model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                               Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);
                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Genel Bilgiler

                    teklif.AddSoru(TSSSorular.Police_Baslangic_Tarihi, model.GenelBilgiler.PoliceBaslangicTarihi);
                    teklif.AddSoru(TSSSorular.Police_Bitis_Tarihi, model.GenelBilgiler.PoliceBitisTarihi);
                    teklif.AddSoru(TSSSorular.Boy, model.GenelBilgiler.Boy);
                    teklif.AddSoru(TSSSorular.Kilo, model.GenelBilgiler.Kilo);

                    teklif.AddSoru(TSSSorular.Meslek, model.GenelBilgiler.MeslekKodu);
                    teklif.AddSoru(TSSSorular.TarifeGrupKodu, model.GenelBilgiler.TarifeKodu);
                    teklif.AddSoru(TSSSorular.YeniIsMi, model.GenelBilgiler.YenilemeMi);
                    teklif.AddSoru(TSSSorular.KronikHastalikCerrahisi, model.GenelBilgiler.KronikHastalikCerrahisi);
                    if (model.GenelBilgiler.KronikHastalikCerrahisi)
                    {
                        teklif.AddSoru(TSSSorular.KronikHastalikAciklama, model.GenelBilgiler.KronikHastalikAciklama);
                    }
                   
                    teklif.AddSoru(TSSSorular.TedaviTipi, model.GenelBilgiler.TedaviTipi);
                    if (model.GenelBilgiler.SigortaliSayisi.HasValue)
                    {
                        teklif.AddSoru(TSSSorular.SigortaliSayisi, model.GenelBilgiler.SigortaliSayisi.Value);
                    }

                    teklif.AddSoru(TSSSorular.KrediKartiMi, model.KrediKartiMi);

                    #endregion

                    #region EskiPoliçe
                    // ==== Eski Poliçe ==== //
                    if (model.GenelBilgiler.YenilemeMi)
                    {
                        teklif.AddSoru(TSSSorular.Eski_Police_Sigorta_Sirketi, model.GenelBilgiler.OncekiSigortaSirketi);
                        teklif.AddSoru(TSSSorular.Eski_Police_No, model.GenelBilgiler.OncekiPoliceNo);
                        if (model.GenelBilgiler.OncekiPoliceBaslangicTarihi.HasValue)
                        {
                            teklif.AddSoru(TSSSorular.Eski_Police_Baslangic_Tarihi, model.GenelBilgiler.OncekiPoliceBaslangicTarihi.Value);
                        }
                    }

                    #endregion

                    #region Teklif kaydı ve hesaplamanın başlatılması
                    //if (ModelState.IsValid)
                    //{
                    try
                    {
                        #region Teklif return

                        ITSSTeklif tssTeklif = new TSSTeklif();

                        // ==== Teklif alınacak şirketler ==== //
                        foreach (var item in model.TeklifUM)
                        {
                            if (item.TeklifAl)
                                tssTeklif.AddUretimMerkezi(item.TUMKodu);
                        }

                        // ==== Teklif ödeme şekli ve tipi ==== //
                        teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
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
                            tssTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                        }
                        else
                        {
                            teklif.GenelBilgiler.TaksitSayisi = 1;
                            tssTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                        }


                        IsDurum isDurum = tssTeklif.Hesapla(teklif);
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
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return Json(new { id = 0, hata = "Teklif hesaplaması başlatılamadı." });
        }

        public ActionResult Police(int id)
        {
            DetayTSSModel model = new DetayTSSModel();

            #region Teklif Genel

            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif tssTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = tssTeklif.GenelBilgiler.TeklifId;
            model.PoliceId = id;
            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(tssTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(tssTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Teklif Genel Bilgiler
            model.GenelBilgiler = new TSSGenelBilgilerDetayModel();
            model.GenelBilgiler.PoliceBaslangicTarihi = tssTeklif.GenelBilgiler.BaslamaTarihi.ToString("dd/MM/yyyy");
            model.GenelBilgiler.PoliceBitisTarihi = tssTeklif.GenelBilgiler.BitisTarihi.ToString("dd/MM/yyyy");
            model.GenelBilgiler.OncekiPoliceBaslangicTarihi = tssTeklif.ReadSoru(TSSSorular.Eski_Police_Baslangic_Tarihi, "");
            model.GenelBilgiler.OncekiPoliceNo = tssTeklif.ReadSoru(TSSSorular.Eski_Police_No, "");
            model.GenelBilgiler.OncekiSigortaSirketi = tssTeklif.ReadSoru(TSSSorular.Eski_Police_Sigorta_Sirketi, "");
            model.GenelBilgiler.TeklifId = tssTeklif.GenelBilgiler.TeklifId;
            model.GenelBilgiler.ToplamTutar = tssTeklif.GenelBilgiler.BrutPrim;

            var TedaviTipi = tssTeklif.ReadSoru(TSSSorular.TedaviTipi, "");
            if (TedaviTipi == "0")
            {
                TedaviTipi = "Yatarak";
            }
            else
            {
                TedaviTipi = "Yatarak + Ayakta";
            }

            var YenilemeMi = tssTeklif.ReadSoru(TSSSorular.YeniIsMi, false);
            if (YenilemeMi)
            {
                model.GenelBilgiler.YenilemeMi = "Evet";
            }
            else
            {
                model.GenelBilgiler.YenilemeMi = "Hayır";
            }
            model.GenelBilgiler.Meslek = "";

            var kronikHastalikCerrahisiMi = tssTeklif.ReadSoru(TSSSorular.KronikHastalikCerrahisi, false);
            if (kronikHastalikCerrahisiMi)
            {
                model.GenelBilgiler.KronikHastalikCerrahisi = "Evet";
            }
            else
            {
                model.GenelBilgiler.KronikHastalikCerrahisi = "Hayır";
            }

            model.GenelBilgiler.Kilo = tssTeklif.ReadSoru(TSSSorular.Kilo, "");
            model.GenelBilgiler.Boy = tssTeklif.ReadSoru(TSSSorular.Boy, "");

            #endregion

            #region Teklif Odeme

            model.Odeme = base.TSSPoliceOdemeModel(teklif);
            model.Odeme.DekontPDF = teklif.GenelBilgiler.PDFGenelSartlari;
            model.Odeme.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;

            if (teklif.GenelBilgiler.TUMKodu == TeklifUretimMerkezleri.MAPFRE)
            {
                model.Odeme.DekontPDFGoster = teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti;
            }
            else
            {
                model.Odeme.DekontPDFGoster = true;
            }

            if (teklif.GenelBilgiler.PDFDekont != null)
            {
                model.Odeme.DekontPDFGoster = true;
                model.Odeme.DekontPDF = teklif.GenelBilgiler.PDFDekont;
            }
            else
            {
                model.Odeme.DekontPDFGoster = false;
            }
            if (teklif.GenelBilgiler.PDFPolice != null)
            {
                model.Odeme.PoliceURL = teklif.GenelBilgiler.PDFPolice;
            }
            if (teklif.GenelBilgiler.PDFBilgilendirme != null)
            {
                model.Odeme.BilgilendirmePDF = teklif.GenelBilgiler.PDFBilgilendirme;
            }

            #endregion
            return View(model);
        }

        public ActionResult Detay(int id)
        {
            DetayTSSModel model = new DetayTSSModel();

            #region Teklif Genel

            TSSTeklif tssTeklif = new TSSTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = tssTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(tssTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(tssTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (tssTeklif.Teklif.Sigortalilar.Count > 0 &&
               (tssTeklif.Teklif.SigortaEttiren.MusteriKodu != tssTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(tssTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Teklif Genel Bilgiler
            model.GenelBilgiler = new TSSGenelBilgilerDetayModel();
            model.GenelBilgiler.PoliceBaslangicTarihi = tssTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd/MM/yyyy");
            model.GenelBilgiler.PoliceBitisTarihi = tssTeklif.Teklif.GenelBilgiler.BitisTarihi.ToString("dd/MM/yyyy");

            model.GenelBilgiler.OncekiPoliceBaslangicTarihi = tssTeklif.Teklif.ReadSoru(TSSSorular.Eski_Police_Baslangic_Tarihi, "");
            model.GenelBilgiler.OncekiPoliceNo = tssTeklif.Teklif.ReadSoru(TSSSorular.Eski_Police_No, "");
            model.GenelBilgiler.OncekiSigortaSirketi = tssTeklif.Teklif.ReadSoru(TSSSorular.Eski_Police_Sigorta_Sirketi, "");

            model.GenelBilgiler.Tarife = "Çoklu Acente 3 ay ( 90 gün )"; //Default bu tarife kullanılıyor

            var TedaviTipi = tssTeklif.Teklif.ReadSoru(TSSSorular.TedaviTipi, "");
            if (TedaviTipi == "0")
            {
                TedaviTipi = "Yatarak";
            }
            else
            {
                TedaviTipi = "Yatarak + Ayakta";
            }

            var YenilemeMi = tssTeklif.Teklif.ReadSoru(TSSSorular.YeniIsMi, false);
            if (YenilemeMi)
            {
                model.GenelBilgiler.YenilemeMi = "Evet";
            }
            else
            {
                model.GenelBilgiler.YenilemeMi = "Hayır";
            }
            model.GenelBilgiler.Meslek = "";

            var kronikHastalikCerrahisiMi = tssTeklif.Teklif.ReadSoru(TSSSorular.KronikHastalikCerrahisi, false);
            if (kronikHastalikCerrahisiMi)
            {
                model.GenelBilgiler.KronikHastalikCerrahisi = "Evet";
                model.GenelBilgiler.KronikHastalikAciklama = tssTeklif.Teklif.ReadSoru(TSSSorular.KronikHastalikAciklama, "");
            }
            else
            {
                model.GenelBilgiler.KronikHastalikCerrahisi = "Hayır";
                model.GenelBilgiler.KronikHastalikAciklama = "";
            }

            model.GenelBilgiler.Kilo = Convert.ToInt32(tssTeklif.Teklif.ReadSoru(TSSSorular.Kilo, 0)).ToString();
            model.GenelBilgiler.Boy = Convert.ToInt32(tssTeklif.Teklif.ReadSoru(TSSSorular.Boy, 0)).ToString();


            #endregion

            #region Teklif Fiyat

            model.Fiyat = TSSFiyat(tssTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.Odeme = new Models.TSSPoliceOdemeModel();
            model.Odeme.TaksitSayisi = Convert.ToInt32(tssTeklif.Teklif.GenelBilgiler.TaksitSayisi);
            model.KrediKarti.KK_OdemeSekli = tssTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.KK_OdemeTipi = tssTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = tssTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
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

            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = tssTeklif.Teklif.GenelBilgiler.BrutPrim;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            return View(model);
            #endregion
        }

        [HttpPost]
        public ActionResult OdemeAl(OdemeTSSModel model)
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
                _LogService.Error("TamamlayiciSaglikController.DekontPDF", ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFDekont;
            return Json(new { Success = success, PDFUrl = url });
        }

        private TeklifFiyatModel TSSFiyat(TSSTeklif tssTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = tssTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = tssTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            var tumListe = tssTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = tssTeklif.GetIsDurum();
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

    }
}