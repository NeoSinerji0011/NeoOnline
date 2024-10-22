using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Police.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Lilyum)]

    public class LilyumController : Controller
    {
        IUlkeService _UlkeService;
        IAktifKullaniciService _AktifKullaniciService;
        ITVMService _TVMService;
        IRaporService _RaporService;
        ITeklifService _TeklifService;
        IKullaniciService _KullaniciService;

        public LilyumController()
        {
            _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            _AktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _RaporService = DependencyResolver.Current.GetService<IRaporService>();
            _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
            _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();
        }
        // GET: Rapor/LilyumKart

        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = AltMenuler.LilyumKartKullanim, SekmeKodu = 0)]
        public ActionResult KartKullanim(string tckn)
        {
            if (_AktifKullaniciService.TVMKodu == 153008)
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }
            LilyumKartKullanimModel model = new LilyumKartKullanimModel();
            model.list = new List<LilyumItemModel>();

            List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani");

            var kullaniciListesi = _KullaniciService.GetListTVMKullanicilari(_AktifKullaniciService.TVMKodu);
            model.KullaniciKodu = _AktifKullaniciService.KullaniciKodu;

            model.InternetAcentesiMi = false;
            if ((_AktifKullaniciService.TVMKodu == NeosinerjiTVM.LilyumInternetSatisTVMKodu))
            {
                model.InternetAcentesiMi = true;
            }
            if (model.InternetAcentesiMi)
            {
                var kartListesi = _TeklifService.GetMusteriLilyumKartlari(_AktifKullaniciService.TCKN);
                if (kartListesi.Count > 0)
                {
                    model.Kart = kartListesi[0].ReferansNo;
                    model.KartLogo = "https://neoonlinestrg.blob.core.windows.net/musteri-dokuman/LilyumKart/lilyumKart.png";
                    model.MusteriAdiSoyadi = kartListesi[0].MusteriAdiSoyadi;
                    model.KonutAdres = kartListesi[0].MusteriKonutAdresi;
                    model.KonutIlIlce = kartListesi[0].MusteriKonutIlIlce;
                    model.BrutPrim = kartListesi[0].BrutPrim.HasValue ? kartListesi[0].BrutPrim.Value.ToString("N2") + "₺" : "";
                    model.TaksitSayisi = kartListesi[0].TaksitSayisi;
                    model.LilyumKartNo = kartListesi[0].KartNo;
                    model.KimlikNo = kartListesi[0].KimlikNo;
                    model.list = KartKullanimiGetir(_AktifKullaniciService.TVMKodu, kartListesi[0].ReferansNo);
                }

            }
            else if (!String.IsNullOrEmpty(tckn) && (_AktifKullaniciService.YetkiGrubu == 1188 || _AktifKullaniciService.YetkiGrubu == 1168))
            {
                var kartListesi = _TeklifService.GetMusteriLilyumKartlari(tckn);

                if (kartListesi.Count > 0)
                {
                    model.Kart = kartListesi[0].ReferansNo;
                    model.KartLogo = "https://neoonlinestrg.blob.core.windows.net/musteri-dokuman/LilyumKart/lilyumKart.png";
                    model.MusteriAdiSoyadi = kartListesi[0].MusteriAdiSoyadi;
                    model.KonutAdres = kartListesi[0].MusteriKonutAdresi;
                    model.KonutIlIlce = kartListesi[0].MusteriKonutIlIlce;
                    model.BrutPrim = kartListesi[0].BrutPrim.HasValue ? kartListesi[0].BrutPrim.Value.ToString("N2") + "₺" : "";
                    model.TaksitSayisi = kartListesi[0].TaksitSayisi;
                    model.LilyumKartNo = kartListesi[0].KartNo;
                    model.KimlikNo = kartListesi[0].KimlikNo;
                    model.list = KartKullanimiGetir(_AktifKullaniciService.TVMKodu, kartListesi[0].ReferansNo);
                }
            }
            else
            {

                model.list = new List<LilyumItemModel>();
            }
            if (model.Kart == null)
            {
                model.Kart = "";
            }
            List<LilyumMusteriKartlari> kartlar = new List<LilyumMusteriKartlari>();

            if (!String.IsNullOrEmpty(tckn))
            {
                kartlar = _TeklifService.GetAcenteKullaniciLilyumKartlariByTCKN(_AktifKullaniciService.TVMKodu, tckn);
            }
            else
            {
                kartlar = _TeklifService.GetAcenteKullaniciLilyumKartlari(_AktifKullaniciService.TVMKodu, model.adsoyad);
            }
            model.Kartlar = new SelectList(kartlar, "ReferansNo", "KartValue", model.Kart).ListWithOptionLabel();

            model.Kullanicilar = new SelectList(kullaniciListesi, "KullaniciKodu", "AdiSoyadi", model.KullaniciKodu).ListWithOptionLabel();

            return View(model);
        }

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = 0, SekmeKodu = 0)]
        public ActionResult GetLilyumKartKullanimDetayi(string referansNo,string tvmkodu, string kullanicikodu)
        {
            LilyumKartKullanimModel model = new LilyumKartKullanimModel();
            var KartDetay = _TeklifService.GetMusteriLilyumKartDetay(_AktifKullaniciService.TCKN, referansNo,tvmkodu,kullanicikodu);
            model.iptal = KartDetay.iptal;
            model.BrutPrim = KartDetay.BrutPrim.HasValue ? KartDetay.BrutPrim.Value.ToString("N2") + "₺" : "";
            model.KonutAdres = KartDetay.MusteriKonutAdresi;
            model.KonutIlIlce = KartDetay.MusteriKonutIlIlce;
            model.LilyumKartNo = KartDetay.KartNo;
            model.MusteriAdiSoyadi = KartDetay.MusteriAdiSoyadi;
            model.TaksitSayisi = KartDetay.TaksitSayisi;
            model.list = KartKullanimiGetir(Convert.ToInt32(tvmkodu), referansNo);
            model.Kart = referansNo;
            model.KartLogo = "https://neoonlinestrg.blob.core.windows.net/musteri-dokuman/LilyumKart/lilyumKart.png";

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = 0, SekmeKodu = 0)]
        public ActionResult lilyumKartIptalEt(string referansNo)
        {
            bool result = _TeklifService.lilyumKartIptalEt(referansNo);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [AjaxException]
        //[Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = AltMenuler.LilyumKartKullanim, SekmeKodu = 0)]
        //Satici Lilyum Kart Teminat Detay Görüntüleme
        public ActionResult GetLilyumKartKullanimDetay(string referansNo, int[] tvmKodu)
        {
            LilyumKartKullanimModel model = new LilyumKartKullanimModel();
            LilyumMusteriKartlari KartDetay = new LilyumMusteriKartlari();
            for (int i = 0; i < tvmKodu.Count(); i++)
            {
                KartDetay = _TeklifService.GetMusteriLilyumKartDetay(tvmKodu[i], referansNo);
                if (KartDetay.ReferansNo != null)
                {
                    model.BrutPrim = KartDetay.BrutPrim.HasValue ? KartDetay.BrutPrim.Value.ToString("N2") + "₺" : "";
                    model.KonutAdres = KartDetay.MusteriKonutAdresi;
                    model.KonutIlIlce = KartDetay.MusteriKonutIlIlce;
                    model.LilyumKartNo = KartDetay.KartNo;
                    model.MusteriAdiSoyadi = KartDetay.MusteriAdiSoyadi;
                    model.TaksitSayisi = KartDetay.TaksitSayisi;
                    model.iptal = KartDetay.iptal;
                    model.list = KartKullanimiGetir(tvmKodu[i], referansNo);
                    model.Kart = referansNo;
                    model.KartLogo = "https://neoonlinestrg.blob.core.windows.net/musteri-dokuman/LilyumKart/lilyumKart.png";
                    break;
                }
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public List<LilyumItemModel> KartKullanimiGetir(int tvmKodu, string referansNo)
        {
            List<LilyumItemModel> list = new List<LilyumItemModel>();
            LilyumItemModel modelListItem = null;
            var kartKullanimDetayListesi = _TeklifService.GetLilyumKullaniciTeminatKullanim(tvmKodu, referansNo);

            var teminatListesi = _TeklifService.GetLilyumTeminatlar();
            StringBuilder grupAdi = new StringBuilder();
            StringBuilder teminatAdi = new StringBuilder();
            StringBuilder teminatAciklama = new StringBuilder();
            for (int i = kartKullanimDetayListesi.Count() - 1; i >= 0; i--)
            {
                grupAdi = new StringBuilder();
                teminatAdi = new StringBuilder();
                teminatAciklama = new StringBuilder();
                var teminatDetay = teminatListesi.Where(w => w.TeminatId == kartKullanimDetayListesi[i].TeminatId).FirstOrDefault();
                if (teminatDetay != null)
                {
                    grupAdi.Append(teminatDetay.GrupAdi);
                    teminatAdi.Append(teminatDetay.TeminatAdi);
                    teminatAciklama.Append(teminatDetay.Aciklama);
                }
                modelListItem = new LilyumItemModel()
                {
                    GrupAdi = grupAdi.ToString(),
                    TeminatAdi = teminatAdi.ToString(),
                    TeminatAciklama = teminatAciklama.ToString(),
                    TeminatId = kartKullanimDetayListesi[i].TeminatId,
                    LilyumKartNo = kartKullanimDetayListesi[i].LilyumKartNo,
                    LilyumReferansNo = kartKullanimDetayListesi[i].ReferansNo
                };
                if (kartKullanimDetayListesi[i].ToplamKullanimHakkiAdet != 0)
                {
                    modelListItem.ToplamKullanimAdet = kartKullanimDetayListesi[i].ToplamKullanilanAdet;
                    modelListItem.KullanimHakkiAdet = kartKullanimDetayListesi[i].ToplamKullanimHakkiAdet;
                    modelListItem.KalanKullanimAdet = kartKullanimDetayListesi[i].ToplamKullanimHakkiAdet - kartKullanimDetayListesi[i].ToplamKullanilanAdet;
                    if (kartKullanimDetayListesi[i].TeminatSonKullanilanTarihi.HasValue)
                    {
                        modelListItem.TeminatSonKullanilanTarih = kartKullanimDetayListesi[i].TeminatSonKullanilanTarihi.Value.ToString("dd.MM.yyyy");
                    }
                }

                list.Add(modelListItem);
            }
            if (list.Count > 0)
            {
                list = list.OrderByDescending(s => s.GrupAdi).ThenByDescending(w => w.TeminatAdi).ToList();
            }
            return list;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = AltMenuler.LilyumKartKullanimGuncelle, SekmeKodu = 0)]
        public ActionResult KartKullanimGuncelle()
        {
            KartKullanimGuncelleModel model = new KartKullanimGuncelleModel();
            List<TVMOzetModel> tvmlist = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMler = new SelectList(tvmlist, "Kodu", "Unvani", "").ListWithOptionLabel();
            model.Kullanicilar = new List<SelectListItem>();
            model.Kullanicilar.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
            model.Referanslar = new List<SelectListItem>();
            model.Referanslar.Insert(0, new SelectListItem() { Value = "", Text = babonline.PleaseSelect, Selected = true });
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = AltMenuler.LilyumKartKullanimGuncelle, SekmeKodu = 0)]
        public ActionResult KartKullanimGuncelle(List<KartKullanimGuncelleArrayModel> model)
        {
            List<LilyumKartTeminatKullanimGuncelleModel> guncelleModelList = new List<LilyumKartTeminatKullanimGuncelleModel>();
            LilyumKartTeminatKullanimGuncelleModel guncelleItem = new LilyumKartTeminatKullanimGuncelleModel();
            bool islem = false;
            try
            {
                string LilyumKartNo = "";
                for (int i = model.Count - 1; i >= 0; i--)
                {
                    guncelleItem = new LilyumKartTeminatKullanimGuncelleModel();
                    guncelleItem.Id = model[i].teminatId;
                    guncelleItem.LilyumReferansNo = model[i].lilyumReferansNo;
                    guncelleItem.ToplamKullanilanAdet = model[i].teminatKullanimAdet;
                    if (!String.IsNullOrEmpty(model[i].teminatSonKullanilanTarihi))
                    {
                        guncelleItem.TeminatSonKullanilanTarihi = Convert.ToDateTime(model[i].teminatSonKullanilanTarihi);
                    }

                    if (!String.IsNullOrEmpty(model[i].lilyumKartNo))
                    {
                        guncelleItem.LilyumKartNo = model[i].lilyumKartNo;
                    }

                    guncelleModelList.Add(guncelleItem);
                }

                _TeklifService.UpdateLilyumKartTeminatKullanimlari(guncelleModelList);
                islem = true;
            }
            catch (Exception ex)
            {

            }
            return Json(new { islem });
        }

        public ActionResult GetAcenteKullanici(int acenteKodu)
        {
            var kullaniciListesi = _KullaniciService.GetListTVMKullanicilari(acenteKodu);
            List<SelectListItem> list = new SelectList(kullaniciListesi, "KullaniciKodu", "AdiSoyadi").ListWithOptionLabel();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getKullaniciReferanslari(int[] acenteKodu, string adsoyad)
        {
            List<LilyumMusteriKartlari> kullaniciListesi = new List<LilyumMusteriKartlari>();
            for (int i = 0; i < acenteKodu.Count(); i++)
            {
                kullaniciListesi.InsertRange(kullaniciListesi.Count(), _TeklifService.GetAcenteKullaniciLilyumKartlari(acenteKodu[i], adsoyad));
            }
            List<SelectListItem> list = new SelectList(kullaniciListesi, "ReferansNo", "KartValue").ListWithOptionLabel();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #region KartListesi
        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = AltMenuler.LilyumKartKartListesi, SekmeKodu = 0)]
        public ActionResult KartListesi()
        {
            KartListesi model = new KartListesi();

            try
            {
                List<SelectListItem> aramaTips = new List<SelectListItem>();
                aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Referans Oluşanlar" },
                new SelectListItem() { Value = "1", Text = "Referans Oluşmayanlar" }
            });
                model.AramaTip = 0;
                model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);
                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMUnvani = _AktifKullaniciService.TVMUnvani;

                model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                model.BitisTarihi = TurkeyDateTime.Now;

                List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
                model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani");

                model.kartList = new List<KartListesiProcedureModel>();
                model.OdemeSekilleri = new SelectList(OdemeSekilleriRaporModel.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();

                ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
                var getTVMDetay = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (getTVMDetay != null)
                {
                    int KontrolTVMKod = getTVMDetay.BagliOlduguTVMKodu;
                    if (tvmTaliVar || KontrolTVMKod == -9999)
                    {
                        ViewBag.AnaTVM = true;
                    }
                    else
                    {
                        ViewBag.AnaTVM = false;
                    }

                    if (getTVMDetay.BolgeYetkilisiMi == 1)
                    {
                        model.BolgeYetkilisiMi = true;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = AltMenuler.LilyumKartKartListesi, SekmeKodu = 0)]
        public ActionResult KartListesi(KartListesi model)
        {
            #region multiselects / branslar - tvmler - sirketler

            if (model.TVMLerSelectList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.TVMLerSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.TVMListe = String.Empty;
                if (liste.Count > 0)
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        if (i != liste.Count - 1)
                            model.TVMListe = model.TVMListe + liste[i] + ",";
                        else model.TVMListe = model.TVMListe + liste[i];
                    }
                }
                else
                {
                    model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                }

            }
            else
            {
                model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
            }
            #endregion

            List<SelectListItem> aramaTips = new List<SelectListItem>();
            aramaTips.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Referans Oluşanlar" },
                new SelectListItem() { Value = "1", Text = "Referans Oluşmayanlar" }
            });
            model.AramaTipTipleri = new SelectList(aramaTips, "Value", "Text", model.AramaTip);
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            int anaTVMKodu = _AktifKullaniciService.TVMKodu;

            //model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(model.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.TVMLerItems = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.OdemeSekilleri = new SelectList(OdemeSekilleriRaporModel.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();


            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
            TVMDetay tvmDetay = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu);


            if (tvmDetay.BolgeYetkilisiMi == 1)
            {
                model.BolgeYetkilisiMi = true;
            }

            anaTVMKodu = _AktifKullaniciService.TVMKodu;
            var iller = _UlkeService.GetIlList();
            var ilceler = _UlkeService.GetIlceList();
            model.kartList = new List<KartListesiProcedureModel>();
            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                #region   talisi olan anaacente ya da talisi olmayan anaacanete
                //anaTVMKodu = _AktifKullaniciService.TVMKodu;
                if (model.TVMListe != "" || model.ad != null || model.soyad != null || model.tckn != null)
                {
                    model.kartList = _RaporService.KartListesiRaporGetir(model.BaslangicTarihi, model.BitisTarihi, model.TVMListe,
                                                    model.OdemeSekli, anaTVMKodu, model.ad, model.soyad, model.tckn, model.AramaTip);

                    if (model.kartList.Count > 0)
                    {
                        for (int i = model.kartList.Count - 1; i >= 0; i--)
                        {
                            if (!String.IsNullOrEmpty(model.kartList[i].ilKodu))
                            {
                                model.kartList[i].ilVeIlce = iller.Where(w => w.IlKodu == model.kartList[i].ilKodu).FirstOrDefault() != null ? iller.Where(w => w.IlKodu == model.kartList[i].ilKodu).FirstOrDefault().IlAdi : "";
                            }
                            if (!String.IsNullOrEmpty(model.kartList[i].ilceKodu))
                            {
                                model.kartList[i].ilVeIlce += ilceler.Where(w => w.IlceKodu == Convert.ToInt32(model.kartList[i].ilceKodu)).FirstOrDefault() != null ? " / " + ilceler.Where(w => w.IlceKodu == Convert.ToInt32(model.kartList[i].ilceKodu)).FirstOrDefault().IlceAdi : "";
                            }
                        }
                    }
                    if (model.AramaTip == 1)
                    {
                        foreach (var item in model.kartList)
                        {
                            if (item.TeklifDurumKodu == 2 && item.TUMKodu == 36)
                            {
                                var teklifData = model.kartList.Where(w => w.TeklifNo == item.TeklifNo).FirstOrDefault();
                                item.taksitSayisi = teklifData.anaTeklifTaksitSayisi;
                                item.odemeSekli = teklifData.anaTeklifOdemeSekli;
                                item.Brut = teklifData.anaTeklifBrut;
                                item.ilVeIlce = teklifData.ilVeIlce;
                            }
                        }
                    }
                    model.kartList = model.kartList.Where(x => x.TUMKodu == 36 && x.TeklifDurumKodu == 2).ToList();
                }

                ViewBag.AnaTVM = true;

                #endregion
            }

            return View(model);
        }

        #endregion
        #region referans no güncelle 

        public ActionResult lilyumReferansNoEdit(string adSoyad, string teklifId, string brut, string odemeSekli, string taksitSayisi)
        {
            referansNoEdit model = new referansNoEdit();
            model.adSoyad = adSoyad;
            model.TeklifId = teklifId;
            model.brut = brut;
            model.odemeSekli = odemeSekli;
            model.taksitSayisi = taksitSayisi;
            return PartialView("_lilyumReferansNoEdit", model);
        }

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Lilyum, AltMenuKodu = 0, SekmeKodu = 0)]
        public ActionResult lilyumReferansGuncelle(string teklifId, string brut, string odemeSekli, string taksitSayisi, string referansNo)
        {
            bool result=false;
            string hataMesaji = "";
            bool basarili = false;
            // teklifgenel'de tumpoliceno alanını güncelle(update) ve lilyumTeminatKullanım tablosunda verileri oluştur.(create)
            //aynı referansNo var mı kontrol et varsa güncelleme hata verdir.
            var kontrol = _TeklifService.getLilyumReferans(referansNo);
            if (kontrol == null)
            {
                var lilyumTeminatKullanim = _TeklifService.getLilyumTeminatKullanim(referansNo);
                if (lilyumTeminatKullanim == null)
                {
                    LilyumTeminatKaydetModel kaydetModel = new LilyumTeminatKaydetModel();
                    kaydetModel.TvmKodu = _AktifKullaniciService.TVMKodu;
                    kaydetModel.KullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                    kaydetModel.ReferansNo = referansNo;
                    kaydetModel.KaydedenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                    _TeklifService.LilyumKartTeminatKullanimCreate(kaydetModel, ref basarili, ref hataMesaji);
                }
                result = _TeklifService.lilyumKartReferansGuncelle(teklifId, brut, odemeSekli, taksitSayisi, referansNo);
            }
            else
            {
                result = false;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}