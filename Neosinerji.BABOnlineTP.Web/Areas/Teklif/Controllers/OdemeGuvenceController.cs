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
using Neosinerji.BABOnlineTP.Business.Common.AEGON;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.OdemeGuvence)]
    public class OdemeGuvenceController : TeklifController
    {
        public OdemeGuvenceController(ITVMService tvmService,
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
            OdemeGuvenceModel model = EkleModel(id, null);
            return View(model);
        }

        public ActionResult EkleIlgili(int id)
        {
            OdemeGuvenceModel model = EkleModelIlgiliTeklif(id);
            return View("Ekle", model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? TeklifId)
        {
            OdemeGuvenceModel model = EkleModel(id, TeklifId);
            return View(model);
        }

        public OdemeGuvenceModel EkleModel(int? id, int? TeklifId)
        {
            OdemeGuvenceModel model = new OdemeGuvenceModel();
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

                if (TeklifId.HasValue)
                {
                    model.TekrarTeklif = true;
                    teklif = _TeklifService.GetTeklif(TeklifId.Value);
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

                #region Odeme Güvence

                model.GenelBilgiler = OdemeGuvenceGenelBilgiler(teklif);
                model.OdemeGuvenceKAP = OdemeGuvenceKAP(teklif);

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

        //Yeni Eklendi
        public OdemeGuvenceModel EkleModelIlgiliTeklif(int iliskiliTeklifId)
        {
            OdemeGuvenceModel model = new OdemeGuvenceModel();
            try
            {
                #region Teklif Genel

                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Visit();
                ITeklif iliskiliTeklif = null;
                int id = 0;
                model.IliskiliTeklifVarmi = true;
                model.IliskiliTeklifId = iliskiliTeklifId;

                #endregion

                #region Teklif Hazırlayan Müşteri / Sigorta ettiren

                iliskiliTeklif = _TeklifService.GetTeklif(iliskiliTeklifId);

                int? sigortaliMusteriKodu = null;

                //Teklifi hazırlayan
                model.Hazirlayan = base.EkleHazirlayanModel();

                //Sigorta Ettiren / Sigortalı
                model.Musteri = new SigortaliModel();
                model.Musteri.SigortaliAyni = true;

                id = iliskiliTeklif.SigortaEttiren.MusteriKodu;
                TeklifSigortali sigortali = iliskiliTeklif.Sigortalilar.FirstOrDefault();
                if (sigortali != null)
                {
                    if (id != sigortali.MusteriKodu)
                    {
                        model.Musteri.SigortaliAyni = false;
                        sigortaliMusteriKodu = sigortali.MusteriKodu;
                    }
                }

                List<SelectListItem> ulkeler = new List<SelectListItem>();
                ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });

                model.Musteri.SigortaEttiren = base.EkleMusteriModel(id);
                model.Musteri.MusteriTipleri = nsmusteri.MusteriListProvider.MusteriTipleri();
                model.Musteri.UyrukTipleri = new SelectList(nsmusteri.MusteriListProvider.UyrukTipleri(), "Value", "Text", "0");
                model.Musteri.CinsiyetTipleri = new SelectList(nsmusteri.MusteriListProvider.CinsiyetTipleri(), "Value", "Text");
                model.Musteri.GelirVergisiTipleri = new SelectList(TeklifProvider.GelirVergisiTipleriAEGON(), "Value", "Text", "");

                model.Musteri.Ulkeler = ulkeler;
                model.Musteri.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.Musteri.SigortaEttiren.IlKodu).ListWithOptionLabelIller();
                model.Musteri.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.Musteri.SigortaEttiren.IlKodu),
                                                        "IlceKodu", "IlceAdi", model.Musteri.SigortaEttiren.IlceKodu).ListWithOptionLabel();

                #endregion

                #region Odeme Güvence

                model.GenelBilgiler = OdemeGuvenceGenelBilgilerIliskiliTeklif(iliskiliTeklif);
                model.OdemeGuvenceKAP = OdemeGuvenceKAPIlgiliTeklif(iliskiliTeklif);

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

        public ActionResult Detay(int id)
        {
            DetayOdemeGuvenceModel model = new DetayOdemeGuvenceModel();

            #region Teklif Genel


            OdemeGuvenceTeklif odemeGuvenceTeklif = new OdemeGuvenceTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = odemeGuvenceTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(odemeGuvenceTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.AegonDetayMusteriModel(odemeGuvenceTeklif.Teklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region Ödeme Güvence

            model.GenelBilgiler = OdemeGuvenceGenelBilgilerDetay(odemeGuvenceTeklif.Teklif);
            model.OdemeGuvenceKAP = OdemeGuvenceKAPDetay(odemeGuvenceTeklif.Teklif);

            #endregion

            #region Teklif Fiyat

            model.Fiyat = OdemeGuvenceFiyat(odemeGuvenceTeklif);

            #endregion

            #region Provizyon

            model.OnProvizyon = _TeklifService.AegonOnProvizyonKontrol(id);

            #endregion

            return View(model);
        }

        [HttpPost]
        public ActionResult Hesapla(OdemeGuvenceModel model)
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
                    int musteriKodu = OG_MusteriKaydet(model);
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.OdemeGuvence, model.Hazirlayan.TVMKodu, _AktifKullaniciService.KullaniciKodu, musteriKodu, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Sigortali

                    teklif.AddSigortali(musteriKodu);

                    #endregion

                    #region Ödeme Güvence

                    #region Genel Bilgiler

                    //Ilgili Teklif Var
                    if (model.IliskiliTeklifVarmi && model.IliskiliTeklifId.HasValue)
                    {
                        TeklifGenel iliskiliTeklif = _TeklifService.GetTeklifGenel(model.IliskiliTeklifId.Value);
                        if (iliskiliTeklif != null)
                        {
                            teklif.GenelBilgiler.IlgiliTeklifId = model.IliskiliTeklifId;
                            teklif.GenelBilgiler.IlgiliTeklifNo = iliskiliTeklif.TeklifNo;
                            teklif.GenelBilgiler.IlgiliTeklifUrunKodu = iliskiliTeklif.UrunKodu;
                        }
                    }

                    //GENEL BILGILER
                    teklif.AddSoru(OdemeGuvenceSorular.SigortaBaslangicTarihi, model.GenelBilgiler.SigortaBaslangicTarihi);
                    teklif.AddSoru(OdemeGuvenceSorular.PrimOdemeDonemi, model.GenelBilgiler.primOdemeDonemi.ToString());

                    //KAP
                    teklif.AddSoru(OdemeGuvenceSorular.ParaBirimi, model.OdemeGuvenceKAP.paraBirimi.ToString());
                    teklif.AddSoru(OdemeGuvenceSorular.PoliceBaslangicTarihi, model.OdemeGuvenceKAP.PoliceBaslangicTarihi);
                    teklif.AddSoru(OdemeGuvenceSorular.PoliceNumarasi, model.OdemeGuvenceKAP.KapPoliceNo);

                    if (model.OdemeGuvenceKAP.aylikPrimTutari.HasValue)
                        teklif.AddSoru(OdemeGuvenceSorular.aylikPrimTutari, model.OdemeGuvenceKAP.aylikPrimTutari.Value);

                    if (model.OdemeGuvenceKAP.sigortaSuresi.HasValue)
                        teklif.AddSoru(OdemeGuvenceSorular.SigortaSuresi, model.OdemeGuvenceKAP.sigortaSuresi.Value.ToString());

                    #endregion

                    #region Teminatlar

                    teklif.AddSoru(OdemeGuvenceSorular.KritikHastalikDPM, model.GenelBilgiler.KritikHastalikDPM);
                    teklif.AddSoru(OdemeGuvenceSorular.TamVeTaimiMaluliyetDPM, model.GenelBilgiler.TamVeTaimiMaluliyetDPM);
                    teklif.AddSoru(OdemeGuvenceSorular.IssizlikDPM, model.GenelBilgiler.IssizlikDPM);
                    teklif.AddSoru(OdemeGuvenceSorular.KazaSonucuHastanedeyatarakTDPM, model.GenelBilgiler.KazaSonucuHastanedeyatarakTDPM);
                    teklif.AddSoru(OdemeGuvenceSorular.IssizlikDPM_Faydalanabilirmi, model.GenelBilgiler.IssizlikDPM_FaydalanabilecekDurumdami);

                    #endregion

                    #endregion

                    #region Teklif return

                    IOdemeGuvenceTeklif odemeGuvenceTeklif = new OdemeGuvenceTeklif();

                    odemeGuvenceTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.AEGON);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                    odemeGuvenceTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    IsDurum isDurum = odemeGuvenceTeklif.Hesapla(teklif);

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

        private int OG_MusteriKaydet(OdemeGuvenceModel model)
        {
            if (model != null && model.Musteri != null)
            {
                MusteriModel musteri = model.Musteri.SigortaEttiren;

                if (model.IliskiliTeklifVarmi && model.IliskiliTeklifId.HasValue && musteri.MusteriKodu.HasValue)
                {
                    return musteri.MusteriKodu.Value;
                }
                else
                {
                    MusteriGenelBilgiler KayitliMusteri = _MusteriService.GetMusteri(musteri.KimlikNo, _AktifKullaniciService.TVMKodu);

                    if (KayitliMusteri == null)
                    {
                        #region Yeni Müşteri Oluştur

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

                        #endregion
                    }
                    else
                    {
                        #region Musteri Güncelle

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

                        #endregion
                    }

                    if (KayitliMusteri != null)
                        return KayitliMusteri.MusteriKodu;
                }
            }

            return 0;
        }

        private TeklifFiyatModel OdemeGuvenceFiyat(OdemeGuvenceTeklif odemeGuvenceTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = odemeGuvenceTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = odemeGuvenceTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = odemeGuvenceTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = odemeGuvenceTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = odemeGuvenceTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = odemeGuvenceTeklif.GetIsDurum();
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
        private OdemeGuvenceGenelBilgiler OdemeGuvenceGenelBilgiler(ITeklif teklif)
        {
            OdemeGuvenceGenelBilgiler model = new OdemeGuvenceGenelBilgiler();

            model.KazaSonucuHastanedeyatarakTDPM = false;
            model.TamVeTaimiMaluliyetDPM = false;
            model.IssizlikDPM = false;
            model.KritikHastalikDPM = false;
            model.IssizlikDPM_FaydalanabilecekDurumdami = true;

            try
            {
                model.primOdemeDonemiList = new SelectList(TeklifProvider.PrimDonemleriAEGON(), "Value", "Text", "");

                if (teklif != null)
                {
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(OdemeGuvenceSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Prim Ödeme Dönemi 
                    string primDonemi = teklif.ReadSoru(OdemeGuvenceSorular.PrimOdemeDonemi, String.Empty);
                    if (!String.IsNullOrEmpty(primDonemi))
                        model.primOdemeDonemi = Convert.ToByte(primDonemi);

                    //Kritik Hastalıklar Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.KritikHastalikDPM, false))
                        model.KritikHastalikDPM = true;

                    //Tam ve Daimi Maluliyet Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.TamVeTaimiMaluliyetDPM, false))
                        model.TamVeTaimiMaluliyetDPM = true;

                    //İşsizlik Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.IssizlikDPM, false))
                        model.IssizlikDPM = true;

                    //Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.KazaSonucuHastanedeyatarakTDPM, false))
                        model.KazaSonucuHastanedeyatarakTDPM = true;

                    //İşsizlik durumunda prim muafiyeti sorusu sorulur ve cevabı hayırsa buraya kaydedilir.
                    if (!teklif.ReadSoru(OdemeGuvenceSorular.IssizlikDPM_Faydalanabilirmi, true))
                        model.IssizlikDPM_FaydalanabilecekDurumdami = false;
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }
        private OdemeGuvenceGenelBilgiler OdemeGuvenceGenelBilgilerIliskiliTeklif(ITeklif iliskiliTeklif)
        {
            OdemeGuvenceGenelBilgiler model = new OdemeGuvenceGenelBilgiler();
            try
            {
                model.primOdemeDonemiList = new SelectList(TeklifProvider.PrimDonemleriAEGON(), "Value", "Text", "");
                model.IssizlikDPM_FaydalanabilecekDurumdami = true;

                if (iliskiliTeklif != null)
                {
                    model.SigortaBaslangicTarihi = iliskiliTeklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Prim Ödeme Dönemi 
                    string primDonemi = iliskiliTeklif.ReadSoru(OdulluBirikimSorular.PrimOdemeDonemi, String.Empty);
                    if (!String.IsNullOrEmpty(primDonemi))
                        model.primOdemeDonemi = Convert.ToByte(primDonemi);

                    //İşsizlik durumunda prim muafiyeti sorusu sorulur ve cevabı hayırsa buraya kaydedilir.
                    if (!iliskiliTeklif.ReadSoru(OdemeGuvenceSorular.IssizlikDPM_Faydalanabilirmi, true))
                        model.IssizlikDPM_FaydalanabilecekDurumdami = false;
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private OdemeGuvenceKAP OdemeGuvenceKAP(ITeklif teklif)
        {
            OdemeGuvenceKAP model = new OdemeGuvenceKAP();
            try
            {
                model.paraBirimiList = new SelectList(TeklifProvider.ParaBirimleriTipleriAEGON(), "Value", "Text", "");
                if (teklif != null)
                {
                    CultureInfo turkey = new CultureInfo("tr-TR");

                    //Sigorta Başlangıç Tarihi KAP
                    model.PoliceBaslangicTarihi = teklif.ReadSoru(OdemeGuvenceSorular.PoliceBaslangicTarihi, DateTime.MinValue);
                    model.KapPoliceNo = teklif.ReadSoru(OdemeGuvenceSorular.PoliceNumarasi, String.Empty);

                    //Sigorta süresi (yıl)
                    string sigortaSuresi = teklif.ReadSoru(OdemeGuvenceSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.sigortaSuresi = Convert.ToByte(sigortaSuresi);

                    //Para Birimi 
                    string paraBirimi = teklif.ReadSoru(OdemeGuvenceSorular.ParaBirimi, String.Empty);
                    if (!String.IsNullOrEmpty(paraBirimi))
                        model.paraBirimi = Convert.ToByte(paraBirimi);

                    //Aylık Prim Tutarı 
                    string aylikPrimTutar = teklif.ReadSoru(OdemeGuvenceSorular.aylikPrimTutari, String.Empty);
                    if (!String.IsNullOrEmpty(aylikPrimTutar))
                        model.aylikPrimTutari = Convert.ToDecimal(aylikPrimTutar, turkey);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }
            return model;
        }

        private OdemeGuvenceKAP OdemeGuvenceKAPIlgiliTeklif(ITeklif iliskiliTeklif)
        {
            OdemeGuvenceKAP model = new OdemeGuvenceKAP();

            try
            {
                model.paraBirimiList = new SelectList(TeklifProvider.ParaBirimleriTipleriAEGON(), "Value", "Text", "");
                model.paraBirimi = AegonParaBirimleri.TL;

                if (iliskiliTeklif != null)
                {
                    CultureInfo turkey = new CultureInfo("tr-TR");

                    //Sigorta Başlangıç Tarihi KAP
                    model.PoliceBaslangicTarihi = iliskiliTeklif.ReadSoru(OdulluBirikimSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Sigorta süresi (yıl)
                    string sigortaSuresi = iliskiliTeklif.ReadSoru(OdulluBirikimSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.sigortaSuresi = Convert.ToByte(sigortaSuresi);


                    TeklifGenel aegonTeklif = _TeklifService.GetTeklifGenel(iliskiliTeklif.TeklifNo,
                                                                            iliskiliTeklif.GenelBilgiler.TVMKodu,
                                                                            TeklifUretimMerkezleri.AEGON);

                    if (aegonTeklif != null && aegonTeklif.TeklifNo == iliskiliTeklif.TeklifNo)
                    {
                        //Prim Ödeme Dönemi 
                        string primDonemi = iliskiliTeklif.ReadSoru(OdulluBirikimSorular.PrimOdemeDonemi, String.Empty);
                        if (!String.IsNullOrEmpty(primDonemi))
                        {
                            switch (Convert.ToByte(primDonemi))
                            {
                                case AegonPrimDonemleri.Aylik: model.aylikPrimTutari = aegonTeklif.NetPrim; break;
                                case AegonPrimDonemleri.Aylik_3: model.aylikPrimTutari = aegonTeklif.NetPrim / 3; break;
                                case AegonPrimDonemleri.Aylik_6: model.aylikPrimTutari = aegonTeklif.NetPrim / 6; break;
                                case AegonPrimDonemleri.Yillik: model.aylikPrimTutari = aegonTeklif.NetPrim / 12; break;
                            }
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
        private OdemeGuvenceGenelBilgiler OdemeGuvenceGenelBilgilerDetay(ITeklif teklif)
        {
            OdemeGuvenceGenelBilgiler model = new OdemeGuvenceGenelBilgiler();

            try
            {
                if (teklif != null)
                {
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(OdemeGuvenceSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Prim Ödeme Dönemi 
                    switch (teklif.ReadSoru(OdemeGuvenceSorular.PrimOdemeDonemi, String.Empty))
                    {
                        case "1": model.PrimOdemeDonemiText = "Aylık"; break;
                        case "2": model.PrimOdemeDonemiText = "3 Aylık"; break;
                        case "3": model.PrimOdemeDonemiText = "6 Aylık"; break;
                        case "4": model.PrimOdemeDonemiText = "Yıllık"; break;
                    }

                    //Kritik Hastalıklar Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.KritikHastalikDPM, false))
                        model.KritikHastalikDPM = true;

                    //Tam ve Daimi Maluliyet Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.TamVeTaimiMaluliyetDPM, false))
                        model.TamVeTaimiMaluliyetDPM = true;

                    //İşsizlik Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.IssizlikDPM, false))
                        model.IssizlikDPM = true;

                    //Kaza Sonucu Hastanede Yatarak Tedavi Durumunda Prim Muafiyeti Teminatı
                    if (teklif.ReadSoru(OdemeGuvenceSorular.KazaSonucuHastanedeyatarakTDPM, false))
                        model.KazaSonucuHastanedeyatarakTDPM = true;
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private OdemeGuvenceKAP OdemeGuvenceKAPDetay(ITeklif teklif)
        {
            OdemeGuvenceKAP model = new OdemeGuvenceKAP();
            try
            {
                if (teklif != null)
                {
                    CultureInfo turkey = new CultureInfo("tr-TR");

                    //Sigorta Başlangıç Tarihi KAP
                    model.PoliceBaslangicTarihi = teklif.ReadSoru(OdemeGuvenceSorular.PoliceBaslangicTarihi, DateTime.MinValue);

                    model.KapPoliceNo = teklif.ReadSoru(OdemeGuvenceSorular.PoliceNumarasi, String.Empty);

                    //Sigorta süresi (yıl)
                    string sigortaSuresi = teklif.ReadSoru(OdemeGuvenceSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.sigortaSuresi = Convert.ToByte(sigortaSuresi);

                    //Para Birimi 
                    model.paraBirimiText = OdemeGuvenceParaBirimi.ParaBirimiText(teklif.ReadSoru(OdemeGuvenceSorular.ParaBirimi, String.Empty));

                    //Aylık Prim Tutarı 
                    string aylikPrimTutar = teklif.ReadSoru(OdemeGuvenceSorular.aylikPrimTutari, String.Empty);
                    if (!String.IsNullOrEmpty(aylikPrimTutar))
                        model.aylikPrimTutari = Convert.ToDecimal(aylikPrimTutar, turkey);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        [HttpPost]
        [AjaxException]
        public ActionResult _DetayPartial(int IsDurum_id)
        {
            OdemeGuvenceDetayPartialModel model = new OdemeGuvenceDetayPartialModel();

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
                        model.SigortaBaslangicTarihi = teklif.ReadSoru(OdemeGuvenceSorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");

                        //Prim Ödeme Dönemi
                        model.PrimOdemeDonemi = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(OdemeGuvenceSorular.PrimOdemeDonemi, String.Empty));


                        //Teminatlar
                        model.KritikHastalikDPM = teklif.ReadSoru(OdemeGuvenceSorular.KritikHastalikDPM, false);
                        model.TamVeTaimiMaluliyetDPM = teklif.ReadSoru(OdemeGuvenceSorular.TamVeTaimiMaluliyetDPM, false);
                        model.IssizlikDPM = teklif.ReadSoru(OdemeGuvenceSorular.IssizlikDPM, false);
                        model.KazaSonucuHastanedeyatarakTDPM = teklif.ReadSoru(OdemeGuvenceSorular.KazaSonucuHastanedeyatarakTDPM, false);

                        ///KAP
                        model.PoliceBaslangicTarihi = teklif.ReadSoru(OdemeGuvenceSorular.PoliceBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");
                        model.SigortaSuresi = teklif.ReadSoru(OdemeGuvenceSorular.SigortaSuresi, String.Empty);
                        model.AylikPrimTutari = teklif.ReadSoru(OdemeGuvenceSorular.aylikPrimTutari, String.Empty);

                        //KAP Poliçe No
                        model.KapPoliceNo = teklif.ReadSoru(OdemeGuvenceSorular.PoliceNumarasi, String.Empty);

                        //Para Birimi 
                        model.ParaBirimi = OdemeGuvenceParaBirimi.ParaBirimiText(teklif.ReadSoru(OdemeGuvenceSorular.ParaBirimi, String.Empty));

                        model.IlgiliTeklifId = teklif.GenelBilgiler.IlgiliTeklifId;
                        model.IlgiliTeklifNo = _TeklifService.SetIlgiliTeklifId(teklif);
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return PartialView(model);
        }

        public void KapYildonumuTarihi(DateTime SigBas, DateTime KapBas)
        {
            //KapBas
            int kapBasGun = KapBas.Day;
            int kapBasAy = KapBas.Month;
            int kapBasYil = KapBas.Year;


            //SigBas
            int sigBasGun = SigBas.Day;
            int sigBasAy = SigBas.Month;
            int sigBasYil = SigBas.Year;
        }

        public string SigortaliYasHesapla(DateTime DogTar, DateTime SigBas, int SigSure, bool KritikHastalik, bool Maluliyet, bool Issizlik, bool Hastane)
        {
            string message = String.Empty;

            int yas = 0;
            int sigortalanabilirYas = 0;
            //int VEFPMYasi = 0;
            if (DogTar != null && SigBas != null)
            {
                yas = AEGONTESabitPrimli.AegonYasHesapla(DogTar, SigBas);
                if (SigSure > 0)
                {
                    sigortalanabilirYas = yas + SigSure;
                }
            }

            if (SigBas < DateTime.Today)
                message += "<p>Sigorta başlangıç tarihi olarak geçmiş bir tarih girilemez </p>";

            if (yas > 17)
            {
                if (KritikHastalik && yas > OdemeGUvenceSabitleri.KHPMMaxGirisYasi)
                {
                    message += "<p>Sigortalı poliçe başlangıç tarihinde " + OdemeGUvenceSabitleri.KHPMMaxGirisYasi + " yaşından büyük olduğu için <b>Kritik" +
                               " Hastalıklar Durumunda Prim Muafiyeti Teminatı teminatı</b> alınamaz.</p>";
                }

                if (Maluliyet && yas > OdemeGUvenceSabitleri.MDPMMaxGirisYasi)
                {
                    message += "<p>Sigortalı poliçe başlangıç tarihinde " + OdemeGUvenceSabitleri.MDPMMaxGirisYasi + " yaşından büyük olduğu için <b>Tam ve Daimi" +
                               " Maluliyet Durumunda Prim Muafiyeti Teminatı</b> alınamaz.</p>";
                }

                if (Issizlik && yas > OdemeGUvenceSabitleri.IDPMMaxGirisYasi)
                {
                    message += "<p>Sigortalı poliçe başlangıç tarihinde " + OdemeGUvenceSabitleri.IDPMMaxGirisYasi + " yaşından büyük olduğu için<b> İşsizlik Durumunda" +
                               " Prim Muafiyeti Teminatı</b> alınamaz.</p>";
                }

                if (Hastane && yas > OdemeGUvenceSabitleri.KHYPMMaxGirisYasi)
                {
                    message += "<p>Sigortalı poliçe başlangıç tarihinde " + OdemeGUvenceSabitleri.KHYPMMaxGirisYasi + " yaşından büyük olduğu için<b> Kaza Sonucu" +
                               " Hastanede Yatarak Tedavi Durumunda Prim Muafiyeti Teminatı </b> alınamaz.</p>";
                }
            }
            else
            {
                message = "<p>Sigortalı yaşı 18 den küçük olamaz</p>";
            }

            return message;
        }
    }
}
