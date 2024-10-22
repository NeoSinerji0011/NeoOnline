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

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.PrimIadeli2)]
    public class PrimIadeli2Controller : TeklifController
    {
        public PrimIadeli2Controller(ITVMService tvmService,
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
            PrimIadeli2Model model = EkleModel(id, null);
            return View(model);
        }

        public PrimIadeli2Model EkleModel(int? id, int? teklifId)
        {
            PrimIadeli2Model model = new PrimIadeli2Model();
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

                    string gelirVergisi = teklif.ReadSoru(TESabitPrimliSorular.MusteriGelirVergisiOrani, String.Empty);
                    if (!String.IsNullOrEmpty(gelirVergisi))
                        model.Musteri.SigortaEttiren.GelirVergisiOrani = Convert.ToByte(gelirVergisi);
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

                #region PrimIadeli2

                model.GenelBilgiler = PrimIadeli2GenelBilgiler(teklifId, teklif);



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
        public ActionResult Ekle(int? id, int? teklifId)
        {
            PrimIadeli2Model model = EkleModel(id, teklifId);
            return View(model);
        }

        [HttpPost]
        public ActionResult Hesapla(PrimIadeli2Model model)
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

                    if (model.GenelBilgiler.HesaplamaSecenegi == 1)
                    {
                        if (ModelState["GenelBilgiler.VefatTutari"] != null)
                            ModelState["GenelBilgiler.VefatTutari"].Errors.Clear();
                    }
                    else if (model.GenelBilgiler.HesaplamaSecenegi == 2)
                    {
                        if (ModelState["GenelBilgiler.YillikPrimTutari"] != null)
                            ModelState["GenelBilgiler.YillikPrimTutari"].Errors.Clear();
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
                    int musteriKodu = PI2_MusteriKaydet(model);
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.PrimIadeli2, model.Hazirlayan.TVMKodu,
                                                                         _AktifKullaniciService.KullaniciKodu, musteriKodu, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Gerekli Girişler ve Hata Uyarıları

                    //if (model.AnaTeminatlar.Vefat && model.AnaTeminatlar.Vefat_KritikHastalik)
                    //    return Json(new { id = 0, hata = "Ana teminatlardan yanlızca 1 tanesini seçmelisiniz." });


                    #endregion

                    #region Sigortali

                    teklif.AddSigortali(musteriKodu);

                    #endregion

                    #region PrimIadeli2

                    //Sigorta Başlangıç Tarihi
                    teklif.AddSoru(PrimIadeli2Sorular.SigortaBaslangicTarihi, model.GenelBilgiler.SigortaBaslangicTarihi);

                    //Para Birimi
                    teklif.AddSoru(PrimIadeliSorular.ParaBirimi, AegonParaBirimleri.USD.ToString()); //Default USD Ekleniyor.

                    //Sigorta Süresi
                    teklif.AddSoru(PrimIadeliSorular.SigortaSuresi, "5");

                    ////Surprim Orani
                    //if (model.GenelBilgiler.SurPrimOrani.HasValue)
                    //    teklif.AddSoru(PrimIadeli2Sorular.SurprimOrani, model.GenelBilgiler.SurPrimOrani.Value.ToString());

                    //Prim Ödeme Dönemi
                    if (model.GenelBilgiler.PrimOdemeDonemi.HasValue)
                        teklif.AddSoru(PrimIadeli2Sorular.PrimOdemeDonemi, model.GenelBilgiler.PrimOdemeDonemi.Value.ToString());

                    //Hesaplama Seçeneği
                    if (model.GenelBilgiler.HesaplamaSecenegi.HasValue)
                    {
                        teklif.AddSoru(PrimIadeli2Sorular.HesaplamaSecenegi, model.GenelBilgiler.HesaplamaSecenegi.Value.ToString());

                        switch (model.GenelBilgiler.HesaplamaSecenegi.Value)
                        {
                            case Prim_HesaplamaSecenekleri.YillikPrim:
                                if (model.GenelBilgiler.YillikPrimTutari.HasValue)
                                    teklif.AddSoru(PrimIadeli2Sorular.YillikPrimTutari, model.GenelBilgiler.YillikPrimTutari.Value.ToString());
                                break;
                            case Prim_HesaplamaSecenekleri.VefatTeminati:
                                if (model.GenelBilgiler.VefatTutari.HasValue)
                                    teklif.AddSoru(PrimIadeli2Sorular.VefatTeminatTutari, model.GenelBilgiler.VefatTutari.Value.ToString());
                                break;
                        }
                    }

                    #endregion

                    #region Teklif return

                    IPrimIadeli2Teklif primIadeliTeklif = new PrimIadeli2Teklif();

                    primIadeliTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.AEGON);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    teklif.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                    primIadeliTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);

                    IsDurum isDurum = primIadeliTeklif.Hesapla(teklif);
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
            DetayPrimIadeli2Model model = new DetayPrimIadeli2Model();

            #region Teklif Genel

            PrimIadeli2Teklif primIadeli2Teklif = new PrimIadeli2Teklif(id);
            model.TeklifId = id;
            model.TeklifNo = primIadeli2Teklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(primIadeli2Teklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.AegonDetayMusteriModel(primIadeli2Teklif.Teklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region PrimIadeli2

            model.GenelBilgiler = PrimIadeli2GenelBilgilerDetay(primIadeli2Teklif.Teklif);

            #endregion

            #region Teklif Fiyat

            model.Fiyat = PrimIadeli2Fiyat(primIadeli2Teklif);

            #endregion

            #region Provizyon

            model.OnProvizyon = _TeklifService.AegonOnProvizyonKontrol(id);

            #endregion

            return View(model);
        }

        private TeklifFiyatModel PrimIadeli2Fiyat(PrimIadeli2Teklif primIadeli2Teklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = primIadeli2Teklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = primIadeli2Teklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = primIadeli2Teklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = primIadeli2Teklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = primIadeli2Teklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = primIadeli2Teklif.GetIsDurum();
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

        //----EKLE
        private PrimIadeli2GenelBilgiler PrimIadeli2GenelBilgiler(int? teklifId, ITeklif teklif)
        {
            PrimIadeli2GenelBilgiler model = new PrimIadeli2GenelBilgiler();

            try
            {
                model.PrimDonemleri = new SelectList(TeklifProvider.PrimDonemleriAEGON(), "Value", "Text", "");
                model.HesaplamaSecenekleri = new SelectList(TeklifProvider.PrimIadeliHesaplamaSecenegiTipleriAEGON(), "Value", "Text", "");
                model.YillikPrimTutarlari = new SelectList(TeklifProvider.YillikPrimTutarlariAEGON(), "Value", "Text", "");

                if (teklifId.HasValue && teklif != null)
                {
                    //Sigorta başlangıç tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(PrimIadeli2Sorular.SigortaBaslangicTarihi, DateTime.MinValue);


                    //Prim Ödeme Dönemi
                    string primDonemi = teklif.ReadSoru(PrimIadeli2Sorular.PrimOdemeDonemi, String.Empty);
                    if (!String.IsNullOrEmpty(primDonemi))
                        model.PrimOdemeDonemi = Convert.ToByte(primDonemi);


                    //Hesaplama Seçeneği 
                    string hesaplamaSecenegi = teklif.ReadSoru(PrimIadeli2Sorular.HesaplamaSecenegi, String.Empty);
                    if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                    {
                        model.HesaplamaSecenegi = Convert.ToByte(hesaplamaSecenegi);
                        switch (model.HesaplamaSecenegi)
                        {
                            case Prim_HesaplamaSecenekleri.YillikPrim:
                                string yillikPrimTutari = teklif.ReadSoru(PrimIadeli2Sorular.YillikPrimTutari, String.Empty);
                                if (!String.IsNullOrEmpty(yillikPrimTutari))
                                    model.YillikPrimTutari = Convert.ToByte(yillikPrimTutari);
                                break;
                            case Prim_HesaplamaSecenekleri.VefatTeminati:
                                model.VefatTutari = teklif.ReadSoru(PrimIadeli2Sorular.VefatTeminatTutari, decimal.Zero);
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

        //---DETAY
        private PrimIadeli2GenelBilgiler PrimIadeli2GenelBilgilerDetay(ITeklif teklif)
        {
            PrimIadeli2GenelBilgiler model = new PrimIadeli2GenelBilgiler();

            try
            {
                if (teklif != null)
                {
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(PrimIadeli2Sorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Prim Ödeme Dönemi 
                    switch (teklif.ReadSoru(PrimIadeli2Sorular.PrimOdemeDonemi, String.Empty))
                    {
                        case "1": model.PrimOdemeDonemiText = "Aylık"; break;
                        case "2": model.PrimOdemeDonemiText = "3 Aylık"; break;
                        case "3": model.PrimOdemeDonemiText = "6 Aylık"; break;
                        case "4": model.PrimOdemeDonemiText = "Yıllık"; break;
                    }

                    //Hesaplama Seçeneği 
                    string hesaplamaSecenegi = teklif.ReadSoru(PrimIadeli2Sorular.HesaplamaSecenegi, String.Empty);
                    if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                    {
                        model.HesaplamaSecenegi = Convert.ToByte(hesaplamaSecenegi);
                        model.HesaplamaSecenegiText = Prim_HesaplamaSecenekleri.GetHesaplamaSecenegi(Convert.ToInt32(hesaplamaSecenegi));
                        switch (model.HesaplamaSecenegi)
                        {
                            case Prim_HesaplamaSecenekleri.YillikPrim:
                                string yillikPrimTutari = teklif.ReadSoru(PrimIadeli2Sorular.YillikPrimTutari, String.Empty);
                                if (!String.IsNullOrEmpty(yillikPrimTutari))
                                {
                                    model.YillikPrimTutarText = Prim_YillikTutarSecenekleri.GetYillikPrimSecenegi(Convert.ToInt32(yillikPrimTutari));
                                }
                                break;
                            case Prim_HesaplamaSecenekleri.VefatTeminati:
                                model.VefatTutari = teklif.ReadSoru(PrimIadeli2Sorular.VefatTeminatTutari, decimal.Zero);
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

        private int PI2_MusteriKaydet(PrimIadeli2Model model)
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
            PrimIadeli2DetayPartialModel model = new PrimIadeli2DetayPartialModel();

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
                        model.SigortaBaslangicTarihi = teklif.ReadSoru(PrimIadeli2Sorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");

                        //Prim Ödeme Dönemi
                        model.PrimOdemeDonemiText = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(PrimIadeli2Sorular.PrimOdemeDonemi, String.Empty));

                        //Hesaplama Seçeneği 
                        string hesaplamaSecenegi = teklif.ReadSoru(PrimIadeli2Sorular.HesaplamaSecenegi, String.Empty);
                        if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                        {
                            model.HesaplamaSecenegiText = Prim_HesaplamaSecenekleri.GetHesaplamaSecenegi(Convert.ToInt32(hesaplamaSecenegi));

                            switch (Convert.ToInt32(hesaplamaSecenegi))
                            {
                                case Prim_HesaplamaSecenekleri.YillikPrim:
                                    string yillikprim = teklif.ReadSoru(PrimIadeli2Sorular.YillikPrimTutari, String.Empty);
                                    if (!String.IsNullOrEmpty(yillikprim))
                                    {
                                        model.Tutar = Convert.ToInt32(Prim_YillikTutarSecenekleri.GetYillikPrimSecenegi(Convert.ToInt32(yillikprim))).ToString("N2") + " $";
                                    }
                                    break;
                                case Prim_HesaplamaSecenekleri.VefatTeminati:
                                    model.Tutar = teklif.ReadSoru(PrimIadeli2Sorular.VefatTeminatTutari, decimal.Zero).ToString("N2") + " $";
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
