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
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.YurtDisiSeyehatSaglik)]
    public class SeyehatSaglikController : TeklifController
    {

        public SeyehatSaglikController(ITVMService tvmService,
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
            SeyehatSaglikModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            SeyehatSaglikModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public SeyehatSaglikModel EkleModel(int? id, int? teklifId)
        {
            SeyehatSaglikModel model = new SeyehatSaglikModel();
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

                #region Seyehat Sağlık

                model.GenelBilgiler = SeyehatSaglikGenelBilgiler(teklifId, teklif);

                model.Sigortalilar = SeyehatSaglikSigortalilarList(teklifId, teklif);

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.YurtDisiSeyehatSaglik);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme

                model.Odeme = new SeyehatSaglikTeklifOdemeModel();
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
        public ActionResult Hesapla(SeyehatSaglikModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                TryValidateModel(model);

                if (model != null)
                {
                    ModelStateMusteriClear(ModelState, model.Musteri);

                    if (model.GenelBilgiler != null)
                    {
                        if (model.GenelBilgiler.UlkeTipi != 2)
                        {
                            if (ModelState["GenelBilgiler.Plan"] != null)
                                ModelState["GenelBilgiler.Plan"].Errors.Clear();
                        }

                        if (model.GenelBilgiler.KisiSayisi > 0 & model.GenelBilgiler.KisiSayisi < 6)
                        {
                            if (model.GenelBilgiler.SigortaEttirenSigortalilardanBirimi)
                            {
                                if (ModelState["Sigortalilar.SigortaliList[0].Adi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[0].Adi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[0].Soyadi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[0].Soyadi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[0].KimlikNo"] != null)
                                    ModelState["Sigortalilar.SigortaliList[0].KimlikNo"].Errors.Clear();
                            }

                            if (model.GenelBilgiler.KisiSayisi == 1)
                            {
                                if (ModelState["Sigortalilar.SigortaliList[0].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[0].BireyTipi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[1].Adi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].Adi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[1].Soyadi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].Soyadi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[1].DogumTarihi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].DogumTarihi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[1].Uyruk"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].Uyruk"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[1].KimlikTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].KimlikTipi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[1].KimlikNo"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].KimlikNo"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[1].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].BireyTipi"].Errors.Clear();
                            }


                            if (model.GenelBilgiler.KisiSayisi == 1 || model.GenelBilgiler.KisiSayisi == 2)
                            {
                                if (ModelState["Sigortalilar.SigortaliList[0].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[0].BireyTipi"].Errors.Clear();
                                if (ModelState["Sigortalilar.SigortaliList[1].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].BireyTipi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[2].Adi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].Adi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[2].Soyadi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].Soyadi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[2].DogumTarihi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].DogumTarihi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[2].Uyruk"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].Uyruk"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[2].KimlikTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].KimlikTipi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[2].KimlikNo"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].KimlikNo"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[2].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].BireyTipi"].Errors.Clear();
                            }

                            if (model.GenelBilgiler.KisiSayisi == 1 || model.GenelBilgiler.KisiSayisi == 2 || model.GenelBilgiler.KisiSayisi == 3)
                            {
                                if (ModelState["Sigortalilar.SigortaliList[3].Adi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].Adi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[3].Soyadi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].Soyadi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[3].DogumTarihi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].DogumTarihi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[3].Uyruk"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].Uyruk"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[3].KimlikTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].KimlikTipi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[3].KimlikNo"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].KimlikNo"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[3].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].BireyTipi"].Errors.Clear();
                            }

                            if (model.GenelBilgiler.KisiSayisi != 5)
                            {
                                if (ModelState["Sigortalilar.SigortaliList[4].Adi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].Adi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[4].Soyadi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].Soyadi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[4].DogumTarihi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].DogumTarihi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[4].Uyruk"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].Uyruk"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[4].KimlikTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].KimlikTipi"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[4].KimlikNo"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].KimlikNo"].Errors.Clear();

                                if (ModelState["Sigortalilar.SigortaliList[4].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].BireyTipi"].Errors.Clear();
                            }

                            if (!model.GenelBilgiler.SigortalilarAilemi)
                            {
                                if (ModelState["Sigortalilar.SigortaliList[0].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[0].BireyTipi"].Errors.Clear();
                                if (ModelState["Sigortalilar.SigortaliList[1].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[1].BireyTipi"].Errors.Clear();
                                if (ModelState["Sigortalilar.SigortaliList[2].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[2].BireyTipi"].Errors.Clear();
                                if (ModelState["Sigortalilar.SigortaliList[3].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[3].BireyTipi"].Errors.Clear();
                                if (ModelState["Sigortalilar.SigortaliList[4].BireyTipi"] != null)
                                    ModelState["Sigortalilar.SigortaliList[4].BireyTipi"].Errors.Clear();
                            }
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

                    if (model.GenelBilgiler.SeyehatBaslangicTarihi.Date <= TurkeyDateTime.Now.Date)
                    {
                        return Json(new { id = 0, hata = "Seyehat başlangıç tarihi en erken yarın olabilir" });
                    }

                    if (model.GenelBilgiler.SeyehatBaslangicTarihi > TurkeyDateTime.Now.AddMonths(6))
                    {
                        return Json(new { id = 0, hata = "Seyehat başlangıç tarihi 6 ay sonrası olamaz." });
                    }

                    if (model.GenelBilgiler.SeyehatBitisTarihi > model.GenelBilgiler.SeyehatBaslangicTarihi.AddYears(1))
                    {
                        return Json(new { id = 0, hata = "Seyehat süreniz 1 yılı aşamaz." });
                    }

                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.YurtDisiSeyehatSaglik, model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Seyehat Sağlık Bilgileri

                    teklif.AddSoru(SeyehatSaglikSorular.Sigorta_Ettiren_Sigortalilardan_Birimi, model.GenelBilgiler.SigortaEttirenSigortalilardanBirimi);
                    teklif.AddSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, model.GenelBilgiler.UlkeTipi.ToString());
                    teklif.AddSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, model.GenelBilgiler.KayakTeminati);
                    teklif.AddSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, model.GenelBilgiler.SeyehatBaslangicTarihi);
                    teklif.AddSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, model.GenelBilgiler.SeyehatBitisTarihi);
                    teklif.AddSoru(SeyehatSaglikSorular.Gidilecek_Ulke, model.GenelBilgiler.GidilecekUlke);
                    teklif.AddSoru(SeyehatSaglikSorular.Kisi_Sayisi, model.GenelBilgiler.KisiSayisi.ToString());
                    teklif.AddSoru(SeyehatSaglikSorular.Sigortalilar_Ailemi, model.GenelBilgiler.SigortalilarAilemi);

                    if (model.GenelBilgiler.SigortaEttirenSigortalilardanBirimi)
                    {
                        SeyehatSaglikSigortalilar sigortali1 = new SeyehatSaglikSigortalilar();
                        sigortali1.Adi = model.Musteri.SigortaEttiren.AdiUnvan;
                        sigortali1.Soyadi = model.Musteri.SigortaEttiren.SoyadiUnvan;
                        if (model.Musteri.SigortaEttiren.DogumTarihi.HasValue)
                            sigortali1.DogumTarihi = model.Musteri.SigortaEttiren.DogumTarihi.Value;
                        sigortali1.KimlikNo = model.Musteri.SigortaEttiren.KimlikNo;
                        sigortali1.KimlikTipi = 1;
                        sigortali1.Uyruk = Convert.ToByte(model.Musteri.SigortaEttiren.Uyruk == 0 ? 1 : 2);
                        sigortali1.BireyTipi = model.Sigortalilar.SigortaliList[0].BireyTipi;

                        var sigortali1Json = JsonConvert.SerializeObject(sigortali1);
                        teklif.AddSoru(SeyehatSaglikSorular.Sigortali_1, sigortali1Json);
                    }
                    else
                    {
                        var sigortali1 = JsonConvert.SerializeObject(model.Sigortalilar.SigortaliList[0]);
                        teklif.AddSoru(SeyehatSaglikSorular.Sigortali_1, sigortali1);
                    }

                    if (model.GenelBilgiler.UlkeTipi == 2)
                    {
                        teklif.AddSoru(SeyehatSaglikSorular.PlanTipi, model.GenelBilgiler.Plan.ToString());
                    }

                    if (model.GenelBilgiler.KisiSayisi > 1 & model.GenelBilgiler.KisiSayisi < 6)
                    {
                        var sigortali2 = JsonConvert.SerializeObject(model.Sigortalilar.SigortaliList[1]);
                        teklif.AddSoru(SeyehatSaglikSorular.Sigortali_2, sigortali2);
                    }

                    if (model.GenelBilgiler.KisiSayisi > 2 & model.GenelBilgiler.KisiSayisi < 6)
                    {
                        var sigortali3 = JsonConvert.SerializeObject(model.Sigortalilar.SigortaliList[2]);
                        teklif.AddSoru(SeyehatSaglikSorular.Sigortali_3, sigortali3);
                    }

                    if (model.GenelBilgiler.KisiSayisi == 4 || model.GenelBilgiler.KisiSayisi == 5)
                    {
                        var sigortali4 = JsonConvert.SerializeObject(model.Sigortalilar.SigortaliList[3]);
                        teklif.AddSoru(SeyehatSaglikSorular.Sigortali_4, sigortali4);
                    }

                    if (model.GenelBilgiler.KisiSayisi == 5)
                    {
                        var sigortali5 = JsonConvert.SerializeObject(model.Sigortalilar.SigortaliList[4]);
                        teklif.AddSoru(SeyehatSaglikSorular.Sigortali_5, sigortali5);
                    }

                    #endregion

                    #region Teklif return

                    ISeyahatSaglikTeklif seyehatSaglikTeklif = new SeyahatSaglikTeklif();

                    // ==== Teklif alınacak şirketler ==== //
                    foreach (var item in model.TeklifUM)
                    {
                        if (item.TeklifAl)
                            seyehatSaglikTeklif.AddUretimMerkezi(item.TUMKodu);
                    }

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.Odeme.OdemeTipi;

                    if (!model.Odeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                        seyehatSaglikTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        seyehatSaglikTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = seyehatSaglikTeklif.Hesapla(teklif);
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
            DetaySeyehatSaglikModel model = new DetaySeyehatSaglikModel();

            #region Teklif Genel


            SeyahatSaglikTeklif seyehatSagliTeklif = new SeyahatSaglikTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = seyehatSagliTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(seyehatSagliTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(seyehatSagliTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (seyehatSagliTeklif.Teklif.Sigortalilar.Count > 0 &&
               (seyehatSagliTeklif.Teklif.SigortaEttiren.MusteriKodu != seyehatSagliTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(seyehatSagliTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Seyehat Saglik

            model.GenelBilgiler = SeyehatSaglikGenelBilgiler(seyehatSagliTeklif.Teklif);

            model.Sigortalilar = SeyehatSaglikSigortalilarList(seyehatSagliTeklif.Teklif);

            #endregion

            #region Teklif Fiyat

            model.Fiyat = SeyehatSaglikFiyat(seyehatSagliTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
            model.KrediKarti.KK_OdemeSekli = seyehatSagliTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
            model.KrediKarti.KK_OdemeTipi = seyehatSagliTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

            List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
            odemeSekilleri.AddRange(new SelectListItem[]{
                new SelectListItem(){Text=babonline.Cash , Value="1"},
                new SelectListItem(){Text=babonline.Forward , Value="2"}
            });
            model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

            model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

            model.KrediKarti.TaksitSayisi = seyehatSagliTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
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
            DetaySeyehatSaglikModel model = new DetaySeyehatSaglikModel();

            #region Teklif Genel


            ITeklif teklif = _TeklifService.GetTeklif(id);
            ITeklif seyehatSaglikTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            model.TeklifId = seyehatSaglikTeklif.GenelBilgiler.TeklifId;

            #endregion

            #region Teklif hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(seyehatSaglikTeklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(seyehatSaglikTeklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Seyehat Sağlık

            model.GenelBilgiler = SeyehatSaglikGenelBilgiler(seyehatSaglikTeklif);

            model.Sigortalilar = SeyehatSaglikSigortalilarList(seyehatSaglikTeklif);

            #endregion

            #region Teklif Odeme

            model.OdemeBilgileri = SeyehatSaglikPoliceOdemeModel(teklif);

            TeklifTeminat seyehatSaglik = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == SeyehatSaglikTeminatlar.SeyehatSaglik);
            if (seyehatSaglik != null)
                model.OdemeBilgileri.NetPrim = seyehatSaglik.TeminatBedeli.HasValue ? seyehatSaglik.TeminatBedeli.Value : 0;

            #endregion

            return View(model);
        }

        private TeklifFiyatModel SeyehatSaglikFiyat(SeyahatSaglikTeklif seyehatSaglikTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = seyehatSaglikTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = seyehatSaglikTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = seyehatSaglikTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = seyehatSaglikTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = seyehatSaglikTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = seyehatSaglikTeklif.GetIsDurum();
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
        public ActionResult OdemeAl(OdemeSeyehatSaglikModel model)
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

        private NipponSeyehatSaglikGenelBilgiler SeyehatSaglikGenelBilgiler(ITeklif teklif)
        {
            NipponSeyehatSaglikGenelBilgiler model = new NipponSeyehatSaglikGenelBilgiler();

            model.SigortaEttirenSigortalilardanBirimi = teklif.ReadSoru(SeyehatSaglikSorular.Sigorta_Ettiren_Sigortalilardan_Birimi, false);
            model.KayakTeminati = teklif.ReadSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, false);
            model.SeyehatBaslangicTarihi = teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, DateTime.MinValue);
            model.SeyehatBitisTarihi = teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, DateTime.MinValue);

            if (model.UlkeTipi == UlkeTipleri.Diger)
                model.PlanTipiText = SeyehatPlanlariList.SeyehatPlani(Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, "0")));


            model.UlkeTipi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, "0"));
            model.KisiSayisi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1"));

            model.SigortalilarAilemi = teklif.ReadSoru(SeyehatSaglikSorular.Sigortalilar_Ailemi, false);

            string ulkeKodu = teklif.ReadSoru(SeyehatSaglikSorular.Gidilecek_Ulke, String.Empty);
            if (!String.IsNullOrEmpty(ulkeKodu))
            {
                UlkeKodlari ulke = _CRService.GetSeyehatUlkesi(ulkeKodu);
                if (ulke != null)
                    model.GidilecekUlke = ulke.UlkeAdi;
            }

            return model;
        }

        private NipponSeyehatSaglikSigortalilarList SeyehatSaglikSigortalilarList(ITeklif teklif)
        {
            NipponSeyehatSaglikSigortalilarList model = new NipponSeyehatSaglikSigortalilarList();

            model.SigortaliList = new List<NipponSeyehatSaglikSigortalilar>();

            byte kisiSayisi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1"));

            if (kisiSayisi > 0 & kisiSayisi < 6)
            {
                int sayac = 103;
                for (int i = 0; i < kisiSayisi; i++)
                {
                    var sigortali = teklif.ReadSoru(sayac, string.Empty);
                    if (!String.IsNullOrEmpty(sigortali))
                        model.SigortaliList.Add(JsonConvert.DeserializeObject<NipponSeyehatSaglikSigortalilar>(sigortali));

                    sayac++;
                }
            }

            return model;
        }

        private SeyehatSaglikGenelBilgiler SeyehatSaglikGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            SeyehatSaglikGenelBilgiler model = new SeyehatSaglikGenelBilgiler();

            model.UlkeTipleri = new SelectList(TeklifProvider.UlkeTipleri(), "Value", "Text", "").ListWithOptionLabel();
            model.Planlar = new SelectList(TeklifProvider.SeyehatPlanlari(), "Value", "Text", "").ListWithOptionLabel();
            model.Ulkeler = new SelectList(_CRService.GetSeyehatUlkeleri(true), "UlkeKodu", "UlkeAdi", "").ListWithOptionLabel();

            model.KisiSayilari = new List<SelectListItem>();
            model.KayakTeminati = false;
            model.SigortaEttirenSigortalilardanBirimi = true;
            model.UlkeTipi = 1;

            for (int i = 1; i < 6; i++)
            {
                model.KisiSayilari.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            if (teklifId.HasValue)
            {
                model.KayakTeminati = teklif.ReadSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, false);
                model.SeyehatBaslangicTarihi = teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, DateTime.MinValue);
                model.SeyehatBitisTarihi = teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, DateTime.MinValue);
                model.KisiSayisi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1"));
                model.GidilecekUlke = teklif.ReadSoru(SeyehatSaglikSorular.Gidilecek_Ulke, "0");
                model.SigortaEttirenSigortalilardanBirimi = teklif.ReadSoru(SeyehatSaglikSorular.Sigorta_Ettiren_Sigortalilardan_Birimi, true);
                model.UlkeTipi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, "0"));
            }
            else
            {
                model.SeyehatBaslangicTarihi = TurkeyDateTime.Now;
                model.SeyehatBitisTarihi = TurkeyDateTime.Now.AddDays(1);
            }


            return model;
        }

        private SeyehatSaglikSigortalilarList SeyehatSaglikSigortalilarList(int? teklifId, ITeklif teklif)
        {
            SeyehatSaglikSigortalilarList model = new SeyehatSaglikSigortalilarList();
            model.BireyTipleri = new SelectList(TeklifProvider.BireyTipleri(), "Value", "Text", 0).ListWithOptionLabel();
            model.SigortaliList = new List<SeyehatSaglikSigortalilar>();

            if (teklifId.HasValue)
            {
                byte kisiSayisi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "0"));
                int sayac = 103;
                for (int i = 0; i < kisiSayisi; i++)
                {
                    var sigortaliJson = teklif.ReadSoru(sayac, String.Empty);
                    if (sigortaliJson != null && !String.IsNullOrEmpty(sigortaliJson))
                    {
                        SeyehatSaglikSigortalilar sigortali = JsonConvert.DeserializeObject<SeyehatSaglikSigortalilar>(sigortaliJson);
                        sigortali.KimlikTipleri = new SelectList(TeklifProvider.KimlikTipleri(), "Value", "Text", sigortali.KimlikTipi).ListWithOptionLabel();
                        sigortali.Uyruklar = new SelectList(TeklifProvider.Uyruklar(), "Value", "Text", sigortali.Uyruk).ListWithOptionLabel();
                        model.SigortaliList.Add(sigortali);
                    }
                    sayac++;
                }
                for (int i = model.SigortaliList.Count; i < 5; i++)
                {
                    SeyehatSaglikSigortalilar sigortali = new SeyehatSaglikSigortalilar();
                    sigortali.Uyruk = 0;
                    sigortali.KimlikTipi = 1;
                    sigortali.KimlikTipleri = new SelectList(TeklifProvider.KimlikTipleri(), "Value", "Text", sigortali.KimlikTipi).ListWithOptionLabel();
                    sigortali.Uyruklar = new SelectList(TeklifProvider.Uyruklar(), "Value", "Text", sigortali.Uyruk).ListWithOptionLabel();
                    model.SigortaliList.Add(sigortali);
                }
            }
            else
                for (int i = 0; i < 5; i++)
                {
                    SeyehatSaglikSigortalilar sigortali = new SeyehatSaglikSigortalilar();
                    sigortali.Uyruk = 0;
                    sigortali.KimlikTipi = 1;
                    sigortali.KimlikTipleri = new SelectList(TeklifProvider.KimlikTipleri(), "Value", "Text", sigortali.KimlikTipi).ListWithOptionLabel();
                    sigortali.Uyruklar = new SelectList(TeklifProvider.Uyruklar(), "Value", "Text", sigortali.Uyruk).ListWithOptionLabel();
                    model.SigortaliList.Add(sigortali);
                }
            return model;
        }

        private SeyehatSaglikPoliceOdemeModel SeyehatSaglikPoliceOdemeModel(ITeklif teklif)
        {
            SeyehatSaglikPoliceOdemeModel model = new SeyehatSaglikPoliceOdemeModel();

            if (teklif != null && teklif.GenelBilgiler != null)
            {
                model.BrutPrim = teklif.GenelBilgiler.BrutPrim.HasValue ? (teklif.GenelBilgiler.BrutPrim.Value * KurBilgileri.Euro) : 0;
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

        [HttpPost]
        [AjaxException]
        public ActionResult KimlikNoSorgulaSeyehat(string kimlikNo)
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
                    model.AdiUnvan = musteri.AdiUnvan;
                    model.SoyadiUnvan = musteri.SoyadiUnvan;
                    model.DogumTarihiText = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : "";
                    model.Uyruk = musteri.Uyruk;
                    model.KimlikNo = musteri.KimlikNo;
                    model.MusteriTipKodu = musteri.MusteriTipKodu;
                    model.SorgulamaSonuc = true;
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

        public ActionResult SeyehatUlkeleri(bool schengenMi)
        {
            return Json(new SelectList(_CRService.GetSeyehatUlkeleri(schengenMi), "UlkeKodu", "UlkeAdi", "").ListWithOptionLabel(), JsonRequestBehavior.AllowGet);
        }
    }
}
