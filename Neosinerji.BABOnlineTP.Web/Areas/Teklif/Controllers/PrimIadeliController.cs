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
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, UrunKodu = UrunKodlari.PrimIadeli)]
    public class PrimIadeliController : TeklifController
    {

        public PrimIadeliController(ITVMService tvmService,
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
            PrimIadeliModel model = EkleModel(id, null);
            return View(model);
        }

        [HttpPost]
        public ActionResult Ekle(int? id, int? teklifId)
        {
            PrimIadeliModel model = EkleModel(id, teklifId);
            return View(model);
        }

        public PrimIadeliModel EkleModel(int? id, int? teklifId)
        {
            PrimIadeliModel model = new PrimIadeliModel();
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

                #region PrimIadeli

                model.GenelBilgiler = PrimIadeliGenelBilgiler(teklifId, teklif);



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
        public ActionResult Hesapla(PrimIadeliModel model)
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
                    int musteriKodu = PI_MusteriKaydet(model);
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.PrimIadeli, model.Hazirlayan.TVMKodu,
                                                                         _AktifKullaniciService.KullaniciKodu, musteriKodu, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Gerekli Girişler ve Hata Uyarıları

                    //if (model.AnaTeminatlar.Vefat && model.AnaTeminatlar.Vefat_KritikHastalik)
                    //    return Json(new { id = 0, hata = "Ana teminatlardan yanlızca 1 tanesini seçmelisiniz." });


                    #endregion

                    #region Sigortali

                    teklif.AddSigortali(musteriKodu);

                    #endregion

                    #region PrimIadeli

                    #region Genel Bilgiler

                    //Sigorta Başlangıç Tarihi
                    teklif.AddSoru(PrimIadeliSorular.SigortaBaslangicTarihi, model.GenelBilgiler.SigortaBaslangicTarihi);

                    //Para Birimi
                    teklif.AddSoru(PrimIadeliSorular.ParaBirimi, AegonParaBirimleri.USD.ToString()); //Default USD Ekleniyor.

                    //Sigorta Süresi
                    if (model.GenelBilgiler.SigortaSuresi.HasValue)
                        teklif.AddSoru(PrimIadeliSorular.SigortaSuresi, model.GenelBilgiler.SigortaSuresi.Value.ToString());

                    //Prim Ödeme Dönemi
                    if (model.GenelBilgiler.PrimOdemeDonemi.HasValue)
                        teklif.AddSoru(PrimIadeliSorular.PrimOdemeDonemi, model.GenelBilgiler.PrimOdemeDonemi.Value.ToString());

                    //Hesaplama Seçeneği
                    if (model.GenelBilgiler.HesaplamaSecenegi.HasValue)
                    {
                        teklif.AddSoru(PrimIadeliSorular.HesaplamaSecenegi, model.GenelBilgiler.HesaplamaSecenegi.Value.ToString());

                        switch (model.GenelBilgiler.HesaplamaSecenegi.Value)
                        {
                            case ROP_HesaplamaSecenekleri.YillikPrim:
                                if (model.GenelBilgiler.Ortak_PrimTutari.HasValue)
                                    teklif.AddSoru(PrimIadeliSorular.YillikPrimTutari, model.GenelBilgiler.Ortak_PrimTutari.Value.ToString());
                                break;
                            case ROP_HesaplamaSecenekleri.VefatTeminati:
                                if (model.GenelBilgiler.Ortak_PrimTutari.HasValue)
                                    teklif.AddSoru(PrimIadeliSorular.VefatTeminatTutari, model.GenelBilgiler.Ortak_PrimTutari.Value.ToString());
                                break;
                        }
                    }

                    #endregion

                    #endregion

                    #region Teklif return

                    IPrimIadeliTeklif primIadeliTeklif = new PrimIadeliTeklif();

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
            DetayPrimIadeliModel model = new DetayPrimIadeliModel();

            #region Teklif Genel

            PrimIadeliTeklif primIadeliTeklif = new PrimIadeliTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = primIadeliTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(primIadeliTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.AegonDetayMusteriModel(primIadeliTeklif.Teklif.SigortaEttiren.MusteriKodu);

            #endregion

            #region PrimIadeli

            model.GenelBilgiler = PrimIadeliGenelBilgilerDetay(primIadeliTeklif.Teklif);



            #endregion

            #region Teklif Fiyat

            model.Fiyat = PrimIadeliFiyat(primIadeliTeklif);

            #endregion

            #region Provizyon

            model.OnProvizyon = _TeklifService.AegonOnProvizyonKontrol(id);

            #endregion

            return View(model);
        }

        private TeklifFiyatModel PrimIadeliFiyat(PrimIadeliTeklif primIadeliTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = primIadeliTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = primIadeliTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = primIadeliTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = primIadeliTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = primIadeliTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = primIadeliTeklif.GetIsDurum();
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
        private PrimIadeliGenelBilgiler PrimIadeliGenelBilgiler(int? teklifId, ITeklif teklif)
        {
            PrimIadeliGenelBilgiler model = new PrimIadeliGenelBilgiler();

            try
            {
                model.ParaBirimleri = new SelectList(TeklifProvider.ParaBirimleriAEGON(), "Value", "Text", "");
                model.PrimDonemleri = new SelectList(TeklifProvider.PrimDonemleriAEGON(), "Value", "Text", "");
                model.HesaplamaSecenekleri = new SelectList(TeklifProvider.PrimIadeliHesaplamaSecenegiTipleriAEGON(), "Value", "Text", "");
                model.SigortaSureleri = new SelectList(TeklifProvider.PrimIadeliSureSecenegiTipleriAEGON(), "Value", "Text", "");


                if (teklifId.HasValue && teklif != null)
                {
                    //Sigorta başlangıç tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(PrimIadeliSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Sigorta Süresi
                    string sigortaSuresi = teklif.ReadSoru(PrimIadeliSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.SigortaSuresi = Convert.ToByte(sigortaSuresi);

                    //Prim Ödeme Dönemi
                    string primDonemi = teklif.ReadSoru(PrimIadeliSorular.PrimOdemeDonemi, String.Empty);
                    if (!String.IsNullOrEmpty(primDonemi))
                        model.PrimOdemeDonemi = Convert.ToByte(primDonemi);

                    //Hesaplama Seçeneği 
                    string hesaplamaSecenegi = teklif.ReadSoru(PrimIadeliSorular.HesaplamaSecenegi, String.Empty);
                    if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                    {
                        model.HesaplamaSecenegi = Convert.ToByte(hesaplamaSecenegi);
                        switch (model.HesaplamaSecenegi)
                        {
                            case ROP_HesaplamaSecenekleri.YillikPrim:
                                model.Ortak_PrimTutari = teklif.ReadSoru(PrimIadeliSorular.YillikPrimTutari, decimal.Zero);
                                break;
                            case ROP_HesaplamaSecenekleri.VefatTeminati:
                                model.Ortak_PrimTutari = teklif.ReadSoru(PrimIadeliSorular.VefatTeminatTutari, decimal.Zero);
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

        //------------DETAY
        private PrimIadeliGenelBilgiler PrimIadeliGenelBilgilerDetay(ITeklif teklif)
        {
            PrimIadeliGenelBilgiler model = new PrimIadeliGenelBilgiler();

            try
            {
                if (teklif != null)
                {
                    //Sigorta Başlangıç Tarihi
                    model.SigortaBaslangicTarihi = teklif.ReadSoru(PrimIadeliSorular.SigortaBaslangicTarihi, DateTime.MinValue);

                    //Prim Ödeme Dönemi
                    model.PrimOdemeDonemiText = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(PrimIadeliSorular.PrimOdemeDonemi, String.Empty));

                    //Sigorta Suresi
                    string sigortaSuresi = teklif.ReadSoru(PrimIadeliSorular.SigortaSuresi, String.Empty);
                    if (!String.IsNullOrEmpty(sigortaSuresi))
                        model.SigortaSuresiText = ROP_SigortaSureleri.GetSigortaSuresi(Convert.ToInt32(sigortaSuresi));

                    //Hesaplama Seçeneği 
                    string hesaplamaSecenegi = teklif.ReadSoru(PrimIadeliSorular.HesaplamaSecenegi, String.Empty);
                    if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                    {
                        model.HesaplamaSecenegi = Convert.ToByte(hesaplamaSecenegi);
                        model.HesaplamaSecenegiText = ROP_HesaplamaSecenekleri.GetHesaplamaSecenegi(Convert.ToInt32(hesaplamaSecenegi));
                        switch (model.HesaplamaSecenegi)
                        {
                            case ROP_HesaplamaSecenekleri.YillikPrim:
                                model.Ortak_PrimTutari = teklif.ReadSoru(PrimIadeliSorular.YillikPrimTutari, decimal.Zero);
                                break;
                            case ROP_HesaplamaSecenekleri.VefatTeminati:
                                model.Ortak_PrimTutari = teklif.ReadSoru(PrimIadeliSorular.VefatTeminatTutari, decimal.Zero);
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

        private int PI_MusteriKaydet(PrimIadeliModel model)
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
        public JsonResult GetYillikPrimtutarLimit()
        {
            return Json(new { dolar = 720 }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult _DetayPartial(int IsDurum_id)
        {
            PrimIadeliDetayPartialModel model = new PrimIadeliDetayPartialModel();

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
                        model.SigortaBaslangicTarihi = teklif.ReadSoru(PrimIadeliSorular.SigortaBaslangicTarihi, DateTime.MinValue).ToString("dd.MM.yyyy");

                        //Prim Ödeme Dönemi
                        model.PrimOdemeDonemiText = AegonPrimDonemleri.PrimDonemleriText(teklif.ReadSoru(PrimIadeliSorular.PrimOdemeDonemi, String.Empty));

                        //Hesaplama Seçeneği 
                        string hesaplamaSecenegi = teklif.ReadSoru(PrimIadeliSorular.HesaplamaSecenegi, String.Empty);
                        if (!String.IsNullOrEmpty(hesaplamaSecenegi))
                        {
                            model.HesaplamaSecenegiText = ROP_HesaplamaSecenekleri.GetHesaplamaSecenegi(Convert.ToInt32(hesaplamaSecenegi));
                            switch (Convert.ToInt32(hesaplamaSecenegi))
                            {
                                case ROP_HesaplamaSecenekleri.YillikPrim:
                                    model.Tutar = teklif.ReadSoru(PrimIadeliSorular.YillikPrimTutari, decimal.Zero).ToString("N2") + " $";
                                    break;
                                case ROP_HesaplamaSecenekleri.VefatTeminati:
                                    model.Tutar = teklif.ReadSoru(PrimIadeliSorular.VefatTeminatTutari, decimal.Zero).ToString("N2") + " $";
                                    break;
                            }
                        }

                        //Sigorta Suresi
                        string sigortaSuresi = teklif.ReadSoru(PrimIadeliSorular.SigortaSuresi, String.Empty);
                        if (!String.IsNullOrEmpty(sigortaSuresi))
                            model.SigortaSuresiText = ROP_SigortaSureleri.GetSigortaSuresi(Convert.ToInt32(sigortaSuresi));
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
