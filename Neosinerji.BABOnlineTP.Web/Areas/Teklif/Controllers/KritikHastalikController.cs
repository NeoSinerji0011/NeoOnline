using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using nsbusiness = Neosinerji.BABOnlineTP.Business;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Newtonsoft.Json;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    public class KritikHastalikController : TeklifController
    {
        public KritikHastalikController(ITVMService tvmService,
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


        public ActionResult Ekle(int? csid)
        {
            KritikHastalikModel model = EkleModel(csid);
            return View(model);
        }

        //[HttpPost]
        //public ActionResult Ekle(int? id)
        //{
        //    KritikHastalikModel model = EkleModel(id);
        //    return View(model);
        //}

        public KritikHastalikModel EkleModel(int? csid)
        {
            KritikHastalikModel model = new KritikHastalikModel();
            try
            {
                #region Teklif Genel


                ILogService log = DependencyResolver.Current.GetService<ILogService>();
                log.Visit();
                //ITeklif teklif = null;

                #endregion

                #region Teklif Hazırlayan Müşteri / Sigorta ettiren
                int? sigortaliMusteriKodu = null;

                //Teklifi hazırlayan
                model.Hazirlayan = base.EkleHazirlayanModel();

                //Sigorta Ettiren / Sigortalı
                model.Musteri = new SigortaliModel();
                model.Musteri.SigortaliAyni = true;

                List<SelectListItem> ulkeler = new List<SelectListItem>();
                ulkeler.Add(new SelectListItem() { Selected = true, Value = "TUR", Text = "TÜRKİYE" });

                if (csid.HasValue)
                {
                    model.TekrarTeklif = true;
                    model.Musteri.SigortaEttiren = base.EkleMusteriModel(7980);

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

                #region Kritik Hastalık
                model.GenelBilgiler = KritikHastalikGenelBilgiler(csid);
                model.Teminatlar = KritikHastalikTeminatlar(csid);
                model.Lehtar = LehtarBilgileri(csid);
                #endregion

                #region TUM IMAGES

                model.TeklifUM = new TeklifUMListeModel();
                List<TVMUrunYetkileriOzelModel> urunyetkileri = _TVMService.GetTVMUrunYetki(_AktifKullaniciService.TVMKodu, UrunKodlari.KritikHastalik);
                foreach (var item in urunyetkileri)
                    model.TeklifUM.Add(item.TumKodu, item.TumUnvani, item.IMGUrl);

                #endregion

                #region Odeme

                model.Odeme = new KritikHastalikTeklifOdemeModel();
                model.Odeme.OdemeSekli = true;
                model.Odeme.OdemeTipi = 1;
                model.Odeme.Yenilensin = false;
                model.Odeme.TaksitSayilari = new List<SelectListItem>();
                model.Odeme.OdemeTipleri = new SelectList(TeklifProvider.KritikHastalikOdemeTipleri(), "Value", "Text", "2").ToList();

                for (int i = 2; i < 13; i++)
                {
                    model.Odeme.TaksitSayilari.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                }

                model.KrediKarti = new KrediKartiOdemeModel();
                model.KrediKarti.KK_TeklifId = 0;
                model.KrediKarti.Tutar = 0;
                model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
                model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
                model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
                model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

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
        public ActionResult Hesapla(KritikHastalikModel model)
        {
            #region Teklif kaydı ve hesaplamanın başlatılması
            if (ModelState.IsValid)
            {
                try
                {
                    nsbusiness.ITeklif teklif = nsbusiness.Teklif.Create(UrunKodlari.KritikHastalik, model.Hazirlayan.TVMKodu,
                                                                         model.Hazirlayan.TVMKullaniciKodu,
                                                                         model.Musteri.SigortaEttiren.MusteriKodu.Value, _AktifKullaniciService.TVMKodu, _AktifKullaniciService.KullaniciKodu);

                    #region Sigortali

                    //TeklifHesaplaMusteriKaydet(teklif, model.Musteri);

                    #endregion

                    #region Genel Bilgiler

                    //if (model.GenelBilgiler.paraBirimi.HasValue)
                    //    teklif.AddSoru(KritikHastalikSorular.ParaBirimi, model.GenelBilgiler.paraBirimi.Value.ToString());

                    //if (model.GenelBilgiler.meslek.HasValue)
                    //    teklif.AddSoru(KritikHastalikSorular.Meslek, model.GenelBilgiler.meslek.Value.ToString());

                    //if (model.GenelBilgiler.sigortaSuresi.HasValue)
                    //    teklif.AddSoru(KritikHastalikSorular.SigortaSuresi, model.GenelBilgiler.sigortaSuresi.Value.ToString());

                    #endregion

                    #region KritikHastalık
                    //Teminat Sorular
                    //if (model.Teminatlar.teminatTutari != 4)
                    //{
                    //    teklif.AddSoru(KritikHastalikSorular.TeminatTutari, model.Teminatlar.teminatTutari);
                    //}
                    //else { teklif.AddSoru(KritikHastalikSorular.TeminatTutariDiger, model.Teminatlar.teminatTutariDiger); }

                    //teklif.AddSoru(KritikHastalikSorular.VefatTeminati, model.Teminatlar.vefatTeminati);
                    //teklif.AddSoru(KritikHastalikSorular.KazaSonucuMaluliyet, model.Teminatlar.kazaSonucuMaluliyet);
                    //teklif.AddSoru(KritikHastalikSorular.HastalikSonucuMaluliyet, model.Teminatlar.hastalikSonucuMaluliyet);
                    //teklif.AddSoru(KritikHastalikSorular.TehlikeliHastalik, model.Teminatlar.tehlikeliHastalik);


                    //// TEMINATLAR
                    //if (model.Teminatlar.vefatTeminati && model.Teminatlar.VefatBedeli.HasValue)
                    //    teklif.AddTeminat(KritikHastalikTeminatlari.VefatTeminati, model.Teminatlar.VefatBedeli.Value, 0, 0, 0, 0);

                    //if (model.Teminatlar.kazaSonucuMaluliyet && model.Teminatlar.kazaSonucuMaluliyetBedeli.HasValue)
                    //    teklif.AddTeminat(KritikHastalikTeminatlari.KazaSonucuMaluliyet, model.Teminatlar.kazaSonucuMaluliyetBedeli.Value, 0, 0, 0, 0);

                    //if (model.Teminatlar.hastalikSonucuMaluliyet && model.Teminatlar.hastalikSonucuMaluliyetBedeli.HasValue)
                    //    teklif.AddTeminat(KritikHastalikTeminatlari.HastalikSonucuMaluliyet, model.Teminatlar.hastalikSonucuMaluliyetBedeli.Value, 0, 0, 0, 0);

                    //if (model.Teminatlar.tehlikeliHastalik && model.Teminatlar.tehlikeliHastalikBedeli.HasValue)
                    //    teklif.AddTeminat(KritikHastalikTeminatlari.TehlikeliHastalik, model.Teminatlar.tehlikeliHastalikBedeli.Value, 0, 0, 0, 0);

                    #endregion

                    #region Teklif return

                    IKritikHastalikTeklif kritikHastalikTeklif = new KritikHastalikTeklif();

                    kritikHastalikTeklif.AddUretimMerkezi(TeklifUretimMerkezleri.METLIFE);

                    // ==== Teklif ödeme şekli ve tipi ==== //
                    teklif.GenelBilgiler.OdemeSekli = (byte)(model.Odeme.OdemeSekli == true ? OdemeSekilleri.Pesin : OdemeSekilleri.Vadeli);
                    teklif.GenelBilgiler.OdemeTipi = model.Odeme.OdemeTipi;

                    if (!model.Odeme.OdemeSekli)
                    {
                        teklif.GenelBilgiler.TaksitSayisi = model.Odeme.TaksitSayisi;
                        kritikHastalikTeklif.AddOdemePlani(model.Odeme.TaksitSayisi);
                    }
                    else
                    {
                        teklif.GenelBilgiler.TaksitSayisi = 1;
                        kritikHastalikTeklif.AddOdemePlani(OdemePlaniAlternatifKodlari.Pesin);
                    }

                    IsDurum isDurum = kritikHastalikTeklif.Hesapla(teklif);
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
            DetayKritikHastalikModel model = new DetayKritikHastalikModel();

            #region Teklif Genel


            KritikHastalikTeklif kritikHastalikTeklif = new KritikHastalikTeklif(id);
            model.TeklifId = id;
            model.TeklifNo = kritikHastalikTeklif.Teklif.TeklifNo.ToString();

            #endregion

            #region Teklif Hazırlayan

            //Teklifi hazırlayan
            model.Hazirlayan = base.DetayHazirlayanModel(kritikHastalikTeklif.Teklif);

            //Sigorta Ettiren
            model.SigortaEttiren = base.DetayMusteriModel(kritikHastalikTeklif.Teklif.SigortaEttiren.MusteriKodu);

            // ====Sigortali varsa Ekleniyor ==== // 
            if (kritikHastalikTeklif.Teklif.Sigortalilar.Count > 0 &&
               (kritikHastalikTeklif.Teklif.SigortaEttiren.MusteriKodu != kritikHastalikTeklif.Teklif.Sigortalilar.First().MusteriKodu))
                model.Sigortali = base.DetayMusteriModel(kritikHastalikTeklif.Teklif.Sigortalilar.First().MusteriKodu);

            #endregion

            #region Kritik Hastalık

            #endregion

            #region Teklif Fiyat

            model.Fiyat = KritikHastalikTeklif(kritikHastalikTeklif);
            model.KrediKarti = new KrediKartiOdemeModel();
            model.KrediKarti.KK_TeklifId = 0;
            model.KrediKarti.Tutar = 0;
            model.KrediKarti.Aylar = TeklifProvider.KrediKartiAylar();
            model.KrediKarti.SonKullanmaAy = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
            model.KrediKarti.Yillar = TeklifProvider.KrediKartiYillar();
            model.KrediKarti.SonKullanmaYil = TurkeyDateTime.Today.Year.ToString();

            #endregion


            return View(model);
        }

        //---EKLE
        private KritikHastalikGenelBilgilerModel KritikHastalikGenelBilgiler(int? csid)
        {
            KritikHastalikGenelBilgilerModel model = new KritikHastalikGenelBilgilerModel();

            try
            {
                model.paraBirimleri = new SelectList(TeklifProvider.ParaBirimleriTipleriAEGON(), "Value", "Text", "");
                model.meslekler = new SelectList(TeklifProvider.MeslekTipleriMetlife(), "Value", "Text", "");
                model.sigortaSureleri = new SelectList(TeklifProvider.KritikHastalikSureSecenegiTipleri(), "Value", "Text", "");

                if (csid.HasValue)
                {
                    //Para Brimi 
                    model.paraBirimi = 1;

                    //Meslek 
                    model.meslek = 1;

                    //Sigorta süresi (yıl)
                    model.sigortaSuresi = 1;
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private KritikHastalikTeminatlarModel KritikHastalikTeminatlar(int? csid)
        {
            KritikHastalikTeminatlarModel model = new KritikHastalikTeminatlarModel();

            try
            {
                model.teminatTutarlari = new SelectList(TeklifProvider.KritikHastalikTeminatTutarlari(), "Value", "Text", "");
                if (csid.HasValue)
                {
                    //Teminat Tutari
                    model.teminatTutari = 1;

                    if (csid.HasValue)
                    {
                        model.vefatTeminati = true;
                        model.VefatBedeli = 1500;


                        model.kazaSonucuMaluliyet = true;
                        model.kazaSonucuMaluliyetBedeli = 1500;

                        model.hastalikSonucuMaluliyet = true;
                        model.hastalikSonucuMaluliyetBedeli = 1500;


                        model.tehlikeliHastalik = true;
                        model.tehlikeliHastalikBedeli = 1500;
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return model;
        }

        private LehtarBilgileriModel LehtarBilgileri(int? csid)
        {
            LehtarBilgileriModel model = new LehtarBilgileriModel();
            model.LehterList = new List<Models.LehtarBilgileri>();
            model.kisiSayilari = new List<SelectListItem>();

            for (int i = 1; i < 6; i++)
            {
                model.kisiSayilari.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }

            for (int i = 0; i < 5; i++)
            {
                LehtarBilgileri sigortali = new LehtarBilgileri();
                model.LehterList.Add(sigortali);
            }

            if (csid.HasValue)
            {
                model.LehterList[0].Adi = "Ahmet";
                model.LehterList[0].Soyadi = "ATEŞ";
                model.LehterList[0].DogumTarihi = Convert.ToDateTime("01.12.1980");
                model.LehterList[0].Oran = 5;
                model.LehterList.Add(model.LehterList[0]);
            }

            return model;
        }

        ////--DETAY
        //private KritikHastalikGenelBilgilerModel KritikHastalikGenelBilgilerDetay(ITeklif teklif)
        //{
        //    KritikHastalikGenelBilgilerModel model = new KritikHastalikGenelBilgilerModel();

        //    try
        //    {
        //        if (teklif != null)
        //        {
        //            model.paraBirimiText = KritikHastalikParaBirimi.ParaBirimiText(teklif.ReadSoru(KritikHastalikSorular.ParaBirimi, String.Empty));
        //            model.meslekText = teklif.ReadSoru(KritikHastalikSorular.Meslek, String.Empty);
        //            model.sigortaSuresiText = teklif.ReadSoru(KritikHastalikSorular.SigortaSuresi, String.Empty);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _LogService.Error(ex);
        //    }

        //    return model;
        //}
        //private KritikHastalikTeminatlarModel KritikHastalikTeminatlarDetay(ITeklif teklif)
        //{
        //    KritikHastalikTeminatlarModel model = new KritikHastalikTeminatlarModel();

        //    if (teklif != null)
        //    {

        //        try
        //        {
        //            if (teklif != null)
        //            {
        //                //Teminat Tutari
        //                string TeminatTutari = teklif.ReadSoru(KritikHastalikSorular.TeminatTutari, String.Empty);
        //                if (!String.IsNullOrEmpty(TeminatTutari) && Convert.ToInt32(TeminatTutari) == 4)
        //                {
        //                    string teminatTutariDiger = teklif.ReadSoru(KritikHastalikSorular.TeminatTutariDiger, String.Empty);
        //                    if (!String.IsNullOrEmpty(teminatTutariDiger))
        //                        model.teminatTutariText = teminatTutariDiger;

        //                }
        //                else
        //                {
        //                    model.teminatTutariText = TeminatTutari;
        //                }


        //                if (teklif.ReadSoru(KritikHastalikSorular.VefatTeminati, false))
        //                {
        //                    TeklifTeminat VefatTeminati = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KritikHastalikTeminatlari.VefatTeminati);
        //                    if (VefatTeminati != null)
        //                    {
        //                        model.vefatTeminati = true;
        //                        if (VefatTeminati.TeminatBedeli.HasValue)
        //                            model.VefatBedeli = (int)VefatTeminati.TeminatBedeli;
        //                    }
        //                }


        //                if (teklif.ReadSoru(KritikHastalikSorular.KazaSonucuMaluliyet, false))
        //                {
        //                    TeklifTeminat KazaSonucuMaluliyet = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KritikHastalikTeminatlari.KazaSonucuMaluliyet);
        //                    if (KazaSonucuMaluliyet != null)
        //                    {
        //                        model.kazaSonucuMaluliyet = true;
        //                        if (KazaSonucuMaluliyet.TeminatBedeli.HasValue)
        //                            model.kazaSonucuMaluliyetBedeli = (int)KazaSonucuMaluliyet.TeminatBedeli;
        //                    }
        //                }
        //                if (teklif.ReadSoru(KritikHastalikSorular.HastalikSonucuMaluliyet, false))
        //                {
        //                    TeklifTeminat HastalikSonucuMaluliyet = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KritikHastalikTeminatlari.HastalikSonucuMaluliyet);
        //                    if (HastalikSonucuMaluliyet != null)
        //                    {
        //                        model.hastalikSonucuMaluliyet = true;
        //                        if (HastalikSonucuMaluliyet.TeminatBedeli.HasValue)
        //                            model.hastalikSonucuMaluliyetBedeli = (int)HastalikSonucuMaluliyet.TeminatBedeli;
        //                    }
        //                }
        //                if (teklif.ReadSoru(KritikHastalikSorular.TehlikeliHastalik, false))
        //                {
        //                    TeklifTeminat TehlikeliHastalik = teklif.Teminatlar.FirstOrDefault(s => s.TeminatKodu == KritikHastalikTeminatlari.TehlikeliHastalik);
        //                    if (TehlikeliHastalik != null)
        //                    {
        //                        model.tehlikeliHastalik = true;
        //                        if (TehlikeliHastalik.TeminatBedeli.HasValue)
        //                            model.tehlikeliHastalikBedeli = (int)TehlikeliHastalik.TeminatBedeli;
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _LogService.Error(ex);
        //        }
        //    }
        //    return model;
        //}

        private TeklifFiyatModel KritikHastalikTeklif(KritikHastalikTeklif kritikHastalikTeklif)
        {
            TeklifFiyatModel model = new TeklifFiyatModel();
            model.Fiyatlar = new List<TeklifFiyatDetayModel>();

            model.TeklifId = kritikHastalikTeklif.Teklif.GenelBilgiler.TeklifId;

            MusteriGenelBilgiler musteri = kritikHastalikTeklif.Teklif.SigortaEttiren.MusteriGenelBilgiler;
            model.MusteriKodu = musteri.MusteriKodu;
            model.AdSoyadUnvan = String.Format("{0} {1}", musteri.AdiUnvan, musteri.SoyadiUnvan);

            model.PoliceBaslangicTarihi = kritikHastalikTeklif.Teklif.GenelBilgiler.BaslamaTarihi.ToString("dd.MM.yyyy");
            model.PDFDosyasi = kritikHastalikTeklif.Teklif.GenelBilgiler.PDFDosyasi;

            var tumListe = kritikHastalikTeklif.TUMTeklifler.GroupBy(g => g.GenelBilgiler.TUMKodu).Select(s => s.Key).ToList();

            IsDurum durum = kritikHastalikTeklif.GetIsDurum();
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
        public ActionResult TeklifEPostaTest()
        {
            TeklifMailGonderModel model = new TeklifMailGonderModel();
            model.SigortaEttirenMail = "Test";
            model.SigortaEttirenAdSoyad = "Kullanicisi";
            model.SigortaEttirenMailGonder = true;
            model.DigerMailGonder = false;

            return PartialView("_MailGonderPartial", model);
        }

        public ActionResult IsDetay(int? adim)
        {
            KritikHastalikAdimModel Model = new KritikHastalikAdimModel();

            Model.Adim = 2;

            switch (adim)
            {
                case 2: Model.Adim = 2; break;
                case 3: Model.Adim = 3; break;
                case 4: Model.Adim = 4; break;
                case 5: Model.Adim = 5; break;
            }

            return View(Model);
        }

        public ActionResult IsTakipSistemi()
        {
            return View();

        }

    }


}
