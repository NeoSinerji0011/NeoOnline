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
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Seyahat;
using Neosinerji.BABOnlineTP.Business.turknippon.seyahat;
using Neosinerji.BABOnlineTP.Business.Common.SBM;
using HtmlAgilityPack;
using RestSharp;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.YurtDisiSeyehatSaglik)]
    public class NipponSeyahatController : TeklifController
    {

        public NipponSeyahatController(ITVMService tvmService,
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

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public ActionResult Ekle()
        {
            NipponSeyahatModel model = EkleModel();
            ViewBag.TurknipponSeyahatSaglik = true;
            return View(model);
        }

        public NipponSeyahatModel EkleModel()
        {


            NipponSeyahatModel model = new NipponSeyahatModel();
            try
            {
                string DovizKuru = "";

                if (Session["ASPXAUTHCookie"] == null)
                {
                    #region TCKNo-Api Cookies Step 1
                    List<TVMWebServisKullanicilari> webServisKullanicilari = _TVMService.GetListTVMWebServisKullanicilari(_AktifKullaniciService.TVMKodu);
                    string username = webServisKullanicilari.FirstOrDefault().KullaniciAdi;
                    string password = webServisKullanicilari.FirstOrDefault().Sifre;
                    string proxyIP = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[0];
                    string proxyPort = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[1];


                    RestClient client = new RestClient("https://galaksi.turknippon.com/acente-giris");
                    client.Proxy = new WebProxy(proxyIP, Convert.ToInt32(proxyPort));
                    RestRequest nipponLoginGetRequest = new RestRequest(Method.GET);
                    IRestResponse loginPageResponse = client.Execute(nipponLoginGetRequest);

                    HtmlNode.ElementsFlags["form"] = HtmlElementFlag.Closed;
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(loginPageResponse.Content);
                    var form = doc.DocumentNode.SelectSingleNode("//form");
                    HtmlNode input = form.Element("input");
                    HtmlAttribute valueAttribute = input.Attributes["value"];

                    RestResponseCookie sessionIdCookie = (from cookie in loginPageResponse.Cookies where cookie.Name == "ASP.NET_SessionId" select cookie).FirstOrDefault();
                    RestResponseCookie requestVerificationTokenCookie = (from cookie in loginPageResponse.Cookies where cookie.Name == "__RequestVerificationToken_Lw__" select cookie).FirstOrDefault();
                    string requestVerificationToken = valueAttribute.Value;

                    #endregion

                    #region TCKNo-Api Cookies Step 2

                    RestRequest request2 = new RestRequest(Method.POST);
                    request2.AddHeader("content-type", "application/x-www-form-urlencoded");
                    request2.AddCookie(sessionIdCookie.Name, sessionIdCookie.Value);
                    request2.AddCookie(requestVerificationTokenCookie.Name, requestVerificationTokenCookie.Value);
                    request2.AddParameter("__RequestVerificationToken", requestVerificationToken);
                    request2.AddParameter("Captcha", "");
                    request2.AddParameter("LogOnType", "Standart");
                    request2.AddParameter("Password", password);
                    request2.AddParameter("Username", username);
                    IRestResponse response2 = client.Execute(request2);
                    JObject json = JObject.Parse(response2.Content);

                    RestResponseCookie ASPXAUTHCookie = (from cookie in response2.Cookies where cookie.Name == ".ASPXAUTH" select cookie).FirstOrDefault();
                    if ((bool)json["Status"])
                    {
                        Session["requestVerificationTokenCookie"] = requestVerificationTokenCookie;
                        Session["ASPXAUTHCookie"] = ASPXAUTHCookie;
                        Session["sessionIdCookie"] = sessionIdCookie;
                    }
                    else
                    {
                        Session["requestVerificationTokenCookie"] = requestVerificationTokenCookie;
                        Session["ASPXAUTHCookie"] = null;
                        Session["sessionIdCookie"] = sessionIdCookie;
                    }
                    Session["proxyIP"] = proxyIP;
                    Session["proxyPort"] = proxyPort;
                    #endregion

                    #region TCKNo-Api Cookies Step 4
                    if (ASPXAUTHCookie != null)
                    {
                        client.BaseUrl = new Uri("http://galaksi.turknippon.com/jet-satis/jet-seyahat/");
                        RestRequest request4 = new RestRequest(Method.POST);
                        request4.AddCookie(requestVerificationTokenCookie.Name, requestVerificationTokenCookie.Value);
                        request4.AddCookie(ASPXAUTHCookie.Name, ASPXAUTHCookie.Value);
                        request4.AddCookie(sessionIdCookie.Name, sessionIdCookie.Value);
                        IRestResponse response4 = client.Execute(request4);

                        doc = new HtmlDocument();
                        doc.LoadHtml(response4.Content);
                        HtmlNode dovizKuru = doc.GetElementbyId("DovizKuru");
                        string value = dovizKuru.InnerHtml.Trim();
                        DovizKuru = value;

                    }
                    #endregion
                }

                #region Teklif Genel

                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Visit();
                ITeklif teklif = null;

                #endregion

                #region Teklif Hazırlayan Müşteri / Sigorta ettiren
                int? sigortaliMusteriKodu = null;

                //Teklifi hazırlayan
                NipponSeyahatHazirlayanModel hazirlayan = new NipponSeyahatHazirlayanModel();
                hazirlayan.KendiAdima = 1;
                hazirlayan.KendiAdimaList = new SelectList(TeklifListeleri.TeklifHazirlayanTipleri(), "Value", "Text", hazirlayan.KendiAdima);
                hazirlayan.TVMKodu = _AktifKullaniciService.TVMKodu;
                hazirlayan.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                hazirlayan.TVMUnvani = _AktifKullaniciService.TVMUnvani;
                hazirlayan.TVMKullaniciAdi = _AktifKullaniciService.AdiSoyadi;
                hazirlayan.YeniIsMi = false;
                model.Hazirlayan = hazirlayan;

                //Sigorta Ettiren / Sigortalı
                model.Musteri = new NipponSeyahatSigortaliModel();
                model.Musteri.SigortaliAyni = false;

                List<SelectListItem> ulkeler = new List<SelectListItem>();
                ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });

                model.Musteri.SigortaEttiren = new MusteriModel();
                model.Musteri.SigortaEttiren.UlkeKodu = "TUR";
                model.Musteri.SigortaEttiren.Cinsiyet = "E";
                model.Musteri.SigortaEttiren.CepTelefonu = "90";
                model.Musteri.Ulkeler = ulkeler;
                model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", "34").ListWithOptionLabelIller();
                model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", "34"), "IlceKodu", "IlceAdi").ListWithOptionLabel();

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

                model.GenelBilgiler = SeyehatSaglikGenelBilgiler(teklif);
                model.GenelBilgiler.DovizKuru = DovizKuru;
                model.GenelBilgiler.IsDomesticList = new List<SelectListItem>();
                model.GenelBilgiler.IsDomesticList.Insert(0, new SelectListItem() { Text = "Yurtiçi", Value = "true" });
                model.GenelBilgiler.IsDomesticList.Insert(1, new SelectListItem() { Text = "Yurtdışı", Value = "false" });

                model.GenelBilgiler.PlanCodeList = new List<SelectListItem>();
                model.GenelBilgiler.PlanCodeList.Insert(0, new SelectListItem() { Text = "PLAN 1", Value = "1" });
                model.GenelBilgiler.PlanCodeList.Insert(1, new SelectListItem() { Text = "PLAN 2", Value = "2" });

                model.GenelBilgiler.ScopeList = new List<SelectListItem>();
                model.GenelBilgiler.AlternativeList = new List<SelectListItem>();
                model.GenelBilgiler.CountryList = new List<SelectListItem>();


                model.Sigortalilar = SeyehatSaglikSigortalilarList(teklif);

                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.YurtDisiSeyehatSaglik);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme

                model.Odeme = new NipponSeyehatSaglikTeklifOdemeModel();
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
                //throw;
            }
            return model;
        }




        [HttpPost]
        public ActionResult Hesapla(NipponSeyahatModel model)
        {
            #region Teklif kontrol (Valid)

            try
            {
                #region Geçerliliği kontrol edilmeyecek alanlar
                #endregion
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            #endregion

            #region Teklif kaydı ve hesaplamanın başlatılması
            if (!ModelState.IsValid)
            {
                try
                {

                    ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.YurtDisiSeyehatSaglik, model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);
                    #region Sigortali

                    TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Seyehat Sağlık Bilgileri

                    teklif.AddSoru(SeyehatSaglikSorular.Sigorta_Ettiren_Sigortalilardan_Birimi, model.GenelBilgiler.SigortaEttirenSigortalilardanBirimi);
                    teklif.AddSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, model.GenelBilgiler.KayakTeminati);
                    teklif.AddSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, model.GenelBilgiler.SeyehatBaslangicTarihi);
                    teklif.AddSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, model.GenelBilgiler.SeyehatBitisTarihi);
                    teklif.AddSoru(SeyehatSaglikSorular.Kisi_Sayisi, model.GenelBilgiler.KisiSayisi.ToString());
                    teklif.AddSoru(SeyehatSaglikSorular.Sigortalilar_Ailemi, model.GenelBilgiler.SigortalilarAilemi);
                    teklif.AddSoru(SeyehatSaglikSorular.NipponIsDomestic, model.GenelBilgiler.SelectedIsDomestic == "true" ? "E" : "H");

                    bool isDomestic = model.GenelBilgiler.SelectedIsDomestic == "true" ? true : false;
                    teklif.AddSoru(SeyehatSaglikSorular.NipponScope, isDomestic ? "1" : model.GenelBilgiler.SelectedScope);
                    teklif.AddSoru(SeyehatSaglikSorular.NipponTravelPocket, isDomestic ? "1" : model.GenelBilgiler.SelectedScope);
                    teklif.AddSoru(SeyehatSaglikSorular.NipponAlternative, isDomestic ? "1" : model.GenelBilgiler.SelectedAlternative);
                    teklif.AddSoru(SeyehatSaglikSorular.NipponCountry, isDomestic ? "9999" : model.GenelBilgiler.SelectedCountry);

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

        //public ActionResult Detay(int id)
        //{
        //    DetaySeyehatSaglikModel model = new DetaySeyehatSaglikModel();

        //    #region Teklif Genel


        //    SeyahatSaglikTeklif seyehatSagliTeklif = new SeyahatSaglikTeklif(id);
        //    model.TeklifId = id;
        //    model.TeklifNo = seyehatSagliTeklif.Teklif.TeklifNo.ToString();

        //    #endregion

        //    #region Teklif Hazırlayan

        //    //Teklifi hazırlayan
        //    model.Hazirlayan = base.DetayHazirlayanModel(seyehatSagliTeklif.Teklif);

        //    //Sigorta Ettiren
        //    model.SigortaEttiren = base.DetayMusteriModel(seyehatSagliTeklif.Teklif.SigortaEttiren.MusteriKodu);

        //    // ====Sigortali varsa Ekleniyor ==== // 
        //    if (seyehatSagliTeklif.Teklif.Sigortalilar.Count > 0 &&
        //       (seyehatSagliTeklif.Teklif.SigortaEttiren.MusteriKodu != seyehatSagliTeklif.Teklif.Sigortalilar.First().MusteriKodu))
        //        model.Sigortali = base.DetayMusteriModel(seyehatSagliTeklif.Teklif.Sigortalilar.First().MusteriKodu);

        //    #endregion

        //    #region Seyehat Saglik

        //    model.GenelBilgiler = SeyehatSaglikGenelBilgiler(seyehatSagliTeklif.Teklif);

        //    model.Sigortalilar = SeyehatSaglikSigortalilarList(seyehatSagliTeklif.Teklif);

        //    #endregion

        //    #region Teklif Fiyat

        //    model.Fiyat = SeyehatSaglikFiyat(seyehatSagliTeklif);
        //    model.KrediKarti = new KrediKartiOdemeModel();
        //    model.KrediKarti.KK_TeklifId = 0;
        //    model.KrediKarti.Tutar = 0;
        //    model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
        //    model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
        //    model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
        //    model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();
        //    model.KrediKarti.KK_OdemeSekli = seyehatSagliTeklif.Teklif.GenelBilgiler.OdemeSekli.Value;
        //    model.KrediKarti.KK_OdemeTipi = seyehatSagliTeklif.Teklif.GenelBilgiler.OdemeTipi.Value;

        //    List<SelectListItem> odemeSekilleri = new List<SelectListItem>();
        //    odemeSekilleri.AddRange(new SelectListItem[]{
        //        new SelectListItem(){Text=babonline.Cash , Value="1"},
        //        new SelectListItem(){Text=babonline.Forward , Value="2"}
        //    });
        //    model.KrediKarti.OdemeSekilleri = new SelectList(odemeSekilleri, "Value", "Text", model.KrediKarti.KK_OdemeSekli).ToList();

        //    model.KrediKarti.OdemeTipleri = new SelectList(TeklifProvider.OdemeTipleri(), "Value", "Text", model.KrediKarti.KK_OdemeTipi).ToList();

        //    model.KrediKarti.TaksitSayisi = seyehatSagliTeklif.Teklif.GenelBilgiler.TaksitSayisi.Value;
        //    model.KrediKarti.TaksitSayilari = new List<SelectListItem>();
        //    List<SelectListItem> taksitSeceneleri = new List<SelectListItem>();
        //    taksitSeceneleri.AddRange(
        //        new SelectListItem[]{
        //        new SelectListItem() { Text = "2", Value = "2" },
        //        new SelectListItem() { Text = "3", Value = "3" },
        //        new SelectListItem() { Text = "4", Value = "4" },
        //        new SelectListItem() { Text = "5", Value = "5" },
        //        new SelectListItem() { Text = "6", Value = "6" },
        //        new SelectListItem() { Text = "7", Value = "7" },
        //        new SelectListItem() { Text = "8", Value = "8" },
        //        new SelectListItem() { Text = "9", Value = "9" }});
        //    model.KrediKarti.TaksitSayilari = new SelectList(taksitSeceneleri, "Value", "Text", model.KrediKarti.TaksitSayisi).ToList();
        //    #endregion

        //    return View(model);
        //}

        //public ActionResult Police(int id)
        //{
        //    DetayNipponSeyahatModel model = new DetayNipponSeyahatModel();

        //    #region Teklif Genel


        //    ITeklif teklif = _TeklifService.GetTeklif(id);
        //    ITeklif seyehatSaglikTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
        //    model.TeklifId = seyehatSaglikTeklif.GenelBilgiler.TeklifId;

        //    #endregion

        //    #region Teklif hazırlayan

        //    //Teklifi hazırlayan
        //    model.Hazirlayan = base.DetayHazirlayanModel(seyehatSaglikTeklif);

        //    //Sigorta Ettiren
        //    model.SigortaEttiren = base.DetayMusteriModel(seyehatSaglikTeklif.SigortaEttiren.MusteriKodu);

        //    #endregion

        //    #region Seyehat Sağlık

        //    model.GenelBilgiler = SeyehatSaglikGenelBilgiler(seyehatSaglikTeklif);

        //    model.Sigortalilar = SeyehatSaglikSigortalilarList(seyehatSaglikTeklif);

        //    #endregion

        //    #region Teklif Odeme

        //    model.OdemeBilgileri = SeyehatSaglikPoliceOdemeModel(teklif);

        //    TeklifTeminat seyehatSaglik = teklif.Teminatlar.FirstOrDefault(w => w.TeminatKodu == SeyehatSaglikTeminatlar.SeyehatSaglik);
        //    if (seyehatSaglik != null)
        //        model.OdemeBilgileri.NetPrim = seyehatSaglik.TeminatBedeli.HasValue ? seyehatSaglik.TeminatBedeli.Value : 0;

        //    #endregion

        //    return View(model);
        //}

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



        private NipponSeyehatSaglikGenelBilgiler SeyehatSaglikGenelBilgiler(ITeklif teklif)
        {
            NipponSeyehatSaglikGenelBilgiler model = new NipponSeyehatSaglikGenelBilgiler();

            model.SigortaEttirenSigortalilardanBirimi = false;//teklif.ReadSoru(SeyehatSaglikSorular.Sigorta_Ettiren_Sigortalilardan_Birimi, false);
            model.KayakTeminati = false;//teklif.ReadSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, false);
            model.SeyehatBaslangicTarihi = DateTime.MinValue;//teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi, DateTime.MinValue);
            model.SeyehatBitisTarihi = DateTime.MinValue;//teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, DateTime.MinValue);

            if (model.UlkeTipi == UlkeTipleri.Diger)
                model.PlanTipiText = SeyehatPlanlariList.SeyehatPlani(Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi, "0")));


            model.UlkeTipi = Convert.ToByte("0");//Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, "0"));
            model.KisiSayisi = Convert.ToByte("1");//Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1"));

            model.SigortalilarAilemi = false;//teklif.ReadSoru(SeyehatSaglikSorular.Sigortalilar_Ailemi, false);

            string ulkeKodu = String.Empty;//teklif.ReadSoru(SeyehatSaglikSorular.Gidilecek_Ulke, String.Empty);
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

            byte kisiSayisi = Convert.ToByte("1");//Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "1"));

            if (kisiSayisi > 0 & kisiSayisi < 6)
            {
                int sayac = 103;
                for (int i = 0; i < kisiSayisi; i++)
                {
                    var sigortali = string.Empty;//teklif.ReadSoru(sayac, string.Empty);
                    if (!String.IsNullOrEmpty(sigortali))
                        model.SigortaliList.Add(JsonConvert.DeserializeObject<NipponSeyehatSaglikSigortalilar>(sigortali));

                    sayac++;
                }
            }

            return model;
        }

        private NipponSeyehatSaglikGenelBilgiler SeyehatSaglikGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            NipponSeyehatSaglikGenelBilgiler model = new NipponSeyehatSaglikGenelBilgiler();

            model.UlkeTipleri = new SelectList(TeklifProvider.UlkeTipleri(), "Value", "Text", "").ListWithOptionLabel();
            model.Planlar = new SelectList(TeklifProvider.SeyehatPlanlari(), "Value", "Text", "").ListWithOptionLabel();
            model.Ulkeler = new SelectList(_CRService.GetSeyehatUlkeleri(true), "UlkeKodu", "UlkeAdi", "").ListWithOptionLabel();

            model.KisiSayilari = new List<SelectListItem>();
            model.KayakTeminati = false;
            model.SigortaEttirenSigortalilardanBirimi = false;
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
                model.SigortaEttirenSigortalilardanBirimi = teklif.ReadSoru(SeyehatSaglikSorular.Sigorta_Ettiren_Sigortalilardan_Birimi, false);
                model.UlkeTipi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Ulke_Tipi_Schenge_Diger, "0"));
            }
            else
            {
                model.SeyehatBaslangicTarihi = TurkeyDateTime.Now;
                model.SeyehatBitisTarihi = TurkeyDateTime.Now.AddDays(1);
            }


            return model;
        }

        private NipponSeyehatSaglikSigortalilarList SeyehatSaglikSigortalilarList(int? teklifId, ITeklif teklif)
        {
            NipponSeyehatSaglikSigortalilarList model = new NipponSeyehatSaglikSigortalilarList();
            model.BireyTipleri = new SelectList(TeklifProvider.BireyTipleri(), "Value", "Text", 0).ListWithOptionLabel();
            model.SigortaliList = new List<NipponSeyehatSaglikSigortalilar>();

            if (teklifId.HasValue)
            {
                byte kisiSayisi = Convert.ToByte(teklif.ReadSoru(SeyehatSaglikSorular.Kisi_Sayisi, "0"));
                int sayac = 103;
                for (int i = 0; i < kisiSayisi; i++)
                {
                    var sigortaliJson = teklif.ReadSoru(sayac, String.Empty);
                    if (sigortaliJson != null && !String.IsNullOrEmpty(sigortaliJson))
                    {
                        NipponSeyehatSaglikSigortalilar sigortali = JsonConvert.DeserializeObject<NipponSeyehatSaglikSigortalilar>(sigortaliJson);
                        sigortali.KimlikTipleri = new SelectList(TeklifProvider.KimlikTipleri(), "Value", "Text", sigortali.KimlikTipi).ListWithOptionLabel();
                        sigortali.Uyruklar = new SelectList(TeklifProvider.Uyruklar(), "Value", "Text", sigortali.Uyruk).ListWithOptionLabel();
                        model.SigortaliList.Add(sigortali);
                    }
                    sayac++;
                }
                for (int i = model.SigortaliList.Count; i < 5; i++)
                {
                    NipponSeyehatSaglikSigortalilar sigortali = new NipponSeyehatSaglikSigortalilar();
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
                    NipponSeyehatSaglikSigortalilar sigortali = new NipponSeyehatSaglikSigortalilar();
                    sigortali.Uyruk = 0;
                    sigortali.KimlikTipi = 1;
                    sigortali.KimlikTipleri = new SelectList(TeklifProvider.KimlikTipleri(), "Value", "Text", sigortali.KimlikTipi).ListWithOptionLabel();
                    sigortali.Uyruklar = new SelectList(TeklifProvider.Uyruklar(), "Value", "Text", sigortali.Uyruk).ListWithOptionLabel();
                    model.SigortaliList.Add(sigortali);
                }
            return model;
        }

        //private SeyehatSaglikPoliceOdemeModel SeyehatSaglikPoliceOdemeModel(ITeklif teklif)
        //{
        //    SeyehatSaglikPoliceOdemeModel model = new SeyehatSaglikPoliceOdemeModel();

        //    if (teklif != null && teklif.GenelBilgiler != null)
        //    {
        //        model.BrutPrim = teklif.GenelBilgiler.BrutPrim.HasValue ? (teklif.GenelBilgiler.BrutPrim.Value * KurBilgileri.Euro) : 0;
        //        model.NetPrim = teklif.GenelBilgiler.NetPrim.HasValue ? teklif.GenelBilgiler.NetPrim.Value : 0;
        //        model.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : 0;
        //        model.Vergi = teklif.GenelBilgiler.ToplamVergi.HasValue ? teklif.GenelBilgiler.ToplamVergi.Value : 0;
        //        model.TUMKodu = teklif.GenelBilgiler.TUMKodu;

        //        TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);

        //        model.TUMUnvani = tum.Unvani;
        //        model.TUMLogoURL = tum.Logo;
        //        model.PoliceURL = teklif.GenelBilgiler.PDFPolice;
        //        model.teklifId = teklif.GenelBilgiler.TeklifId;
        //        model.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
        //    }
        //    return model;
        //}

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

        [HttpGet]
        public JsonResult GetScopeList(bool isDomestic)
        {
            List<ScopeOutput> items = new List<ScopeOutput>();
            try
            {
                ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
                items = nipponSeyahat.GetScopeList(isDomestic).ToList();
            }
            catch (Exception ex)
            {
                items.Add(new ScopeOutput() { Description = ex.Message });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAlternativeList(bool isDomestic, int scopeOrPocket)
        {
            List<AlternativeOutput> items = new List<AlternativeOutput>();
            try
            {
                ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
                items = nipponSeyahat.GetAlternativeList(isDomestic, scopeOrPocket).ToList();
            }
            catch (Exception ex)
            {
                items.Add(new AlternativeOutput() { Description = ex.Message });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCountryList(int alternative)
        {
            List<CountryOutput> items = new List<CountryOutput>();
            try
            {
                ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
                items = nipponSeyahat.GetCountryList(alternative).ToList();
            }
            catch (Exception ex)
            {
                items.Add(new CountryOutput() { CountryName = ex.Message });
            }
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JObject QueryTCKNo()
        {
            RestResponseCookie requestVerificationTokenCookie = (RestResponseCookie)Session["requestVerificationTokenCookie"];
            RestResponseCookie ASPXAUTHCookie = (RestResponseCookie)Session["ASPXAUTHCookie"];
            RestResponseCookie sessionIdCookie = (RestResponseCookie)Session["sessionIdCookie"];
            string proxyIP = (string)Session["proxyIP"];
            int proxyPort = Convert.ToInt32((string)Session["proxyPort"]);

            string TCKNo = Request.Form["IdentityNo"];

            if (requestVerificationTokenCookie == null || ASPXAUTHCookie == null || sessionIdCookie == null)
            {
                JObject jObject = new JObject();
                jObject["Status"] = false;
                jObject["ErrorType"] = "WebServiceUserError";
                jObject["Data"] = "Web Servis Kullanıcı Hatası";
                return jObject;
            }

            RestClient client = new RestClient("https://galaksi.turknippon.com/jet-satis/jet-seyahat/ajx-kimlikno");
            client.Proxy = new WebProxy(proxyIP, proxyPort);
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("ClientType", "O");
            request.AddParameter("IdentityNo", TCKNo);
            request.AddCookie(sessionIdCookie.Name, sessionIdCookie.Value);
            request.AddCookie(requestVerificationTokenCookie.Name, requestVerificationTokenCookie.Value);
            request.AddCookie(ASPXAUTHCookie.Name, ASPXAUTHCookie.Value);
            IRestResponse response = client.Execute(request);
            JObject json = JObject.Parse(response.Content);
            return json;
        }

        [HttpPost]
        public ContentResult Compute()
        {
            JObject insuredJson = JObject.Parse(Request.Form["insuredJson"]);
            ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
            TravelOutput result = nipponSeyahat.Compute(insuredJson);
            string json = JsonConvert.SerializeObject(result);
            return Content(json, "application/json");
        }

        [HttpPost]
        public ContentResult Approve()
        {
            JObject insuredJson = JObject.Parse(Request.Form["insuredJson"]);
            ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
            TravelOutput result = nipponSeyahat.Approve(insuredJson);
            string json = JsonConvert.SerializeObject(result);
            return Content(json, "application/json");
        }

        [HttpPost]
        public JObject SaveOffer()
        {
            JObject insuredJson = JObject.Parse(Request.Form["insuredJson"]);
            ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
            JObject result = nipponSeyahat.CreateOfferRecord(insuredJson);
            return result;
        }

        [HttpPost]
        public JObject SavePolicy()
        {
            JObject insuredsJson = JObject.Parse(Request.Form["insuredsJson"]);
            ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
            JObject result = nipponSeyahat.CreatePolicyRecord(insuredsJson);
            return result;
        }

        [HttpPost]
        public ContentResult GetPolicyPDFFile()
        {
            JObject insuredJson = JObject.Parse(Request.Form["insuredJson"]);
            ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
            TravelOutput result = nipponSeyahat.Print(insuredJson);
            string json = JsonConvert.SerializeObject(result);
            return Content(json, "application/json");
        }

        [HttpPost]
        public JObject MergePDFFiles()
        {
            JObject jsonObject = JObject.Parse(Request.Form["jsonObject"]);
            ITURKNIPPONSeyahat nipponSeyahat = DependencyResolver.Current.GetService<ITURKNIPPONSeyahat>();
            JObject result = nipponSeyahat.MergePDFFiles(jsonObject);
            return result;
        }

        [HttpPost]
        public int GetCustomerId()
        {
            JObject customerJson = JObject.Parse(Request.Form["customerJson"]);

            string customerIdentityNo = (string)customerJson["CustomerIdentityNo"];
            int tvmCode = _AktifKullaniciService.TVMKodu;

            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(customerIdentityNo, tvmCode);
            if (musteri == null)
            {
                musteri = new MusteriGenelBilgiler();
                string fullName = (string)customerJson["CustomerName"];
                var firstSpaceIndex = fullName.IndexOf(" ");
                string firstName = fullName.Substring(0, firstSpaceIndex);
                string lastName = fullName.Substring(firstSpaceIndex + 1);

                musteri.AdiUnvan = firstName;
                musteri.SoyadiUnvan = lastName;
                musteri.KimlikNo = customerIdentityNo;
                musteri.TVMKodu = tvmCode;

                System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("tr-TR");
                musteri.DogumTarihi = DateTime.Parse((string)customerJson["CustomerBirthDate"], cultureinfo);
                musteri = _MusteriService.CreateMusteri(musteri);
            }

            return musteri.MusteriKodu;
        }
    }

}
