using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models;
using System.Linq.Expressions;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business.GorevTakip;
using static Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models.RaporListProvider;
using Neosinerji.BABOnlineTP.Web.Areas.GorevTakip.Models;
using AutoMapper;

namespace Neosinerji.BABOnlineTP.Web.Areas.GorevTakip.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = 0, SekmeKodu = 0)]
    public class GorevTakipController : Controller
    {
        // GET: GorevTakip/GorevTakip
        IBransService _BransService;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullaniciService;
        ISigortaSirketleriService _SigortaSirketleriService;
        IKullaniciService _KullaniciService;
        IGorevTakipService _GorevTakipService;
        IGorevTakipDokumanStorage _GorevTakipDokumanStorage;
        ICommonService _CommonService;
        IMusteriService _MusteriService;

        public GorevTakipController(IBransService bransService, ITVMService tvmService, IAktifKullaniciService aktifKullaniciService, ISigortaSirketleriService sigortaSirketleriService, IKullaniciService kullaniciService, IGorevTakipService gorevTakipService, IGorevTakipDokumanStorage gorevTakipDokumanStorage, ICommonService commonService, IMusteriService musteriService)
        {
            _BransService = bransService;
            _AktifKullaniciService = aktifKullaniciService;
            _KullaniciService = kullaniciService;
            _SigortaSirketleriService = sigortaSirketleriService;
            _TVMService = tvmService;
            _GorevTakipService = gorevTakipService;
            _GorevTakipDokumanStorage = gorevTakipDokumanStorage;
            _CommonService = commonService;
            _MusteriService = musteriService;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.GorevAtama, SekmeKodu = 0)]
        public ActionResult Ekle(int? id)
        {
            GorevTakipModel model = new GorevTakipModel();
            //id alanı müşteri kodu için kullanılıyor
            #region Müşteri Bilgileri Getirme
            if (id.HasValue)
            {
                #region Genel Müşteri
                var musteriDetay = _MusteriService.GetMusteri(id.Value);
                if (musteriDetay != null)
                {
                    model.GonderenAdiSoyadi = musteriDetay.AdiUnvan + " " + musteriDetay.SoyadiUnvan;
                    model.GonderenEmail = musteriDetay.EMail;
                    model.GonderenTCVKN = musteriDetay.KimlikNo;
                    if (musteriDetay.MusteriTelefons != null)
                    {
                        if (musteriDetay.MusteriTelefons.Count > 0)
                        {
                            var CepTel = musteriDetay.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
                            if (CepTel != null)
                            {
                                model.GonderenTelefon = CepTel.Numara;
                            }
                            else
                            {
                                var EvTel = musteriDetay.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Ev).FirstOrDefault();
                                if (EvTel != null)
                                {
                                    model.GonderenTelefon = EvTel.Numara;
                                }
                                else
                                {
                                    var IsTel = musteriDetay.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Is).FirstOrDefault();
                                    if (IsTel != null)
                                    {
                                        model.GonderenTelefon = IsTel.Numara;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Potansiyel Müşteri
                if (musteriDetay == null)
                {
                    var potMusDetay = _MusteriService.GetPotansiyelMusteri(id.Value);
                    if (potMusDetay != null)
                    {
                        model.GonderenAdiSoyadi = potMusDetay.AdiUnvan + " " + potMusDetay.SoyadiUnvan;
                        model.GonderenEmail = potMusDetay.EMail;
                        model.GonderenTCVKN = potMusDetay.KimlikNo;
                        if (potMusDetay.PotansiyelMusteriTelefons != null)
                        {
                            if (potMusDetay.PotansiyelMusteriTelefons.Count > 0)
                            {
                                var CepTel = potMusDetay.PotansiyelMusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
                                if (CepTel != null)
                                {
                                    model.GonderenTelefon = CepTel.Numara;
                                }
                                else
                                {
                                    var EvTel = potMusDetay.PotansiyelMusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Ev).FirstOrDefault();
                                    if (EvTel != null)
                                    {
                                        model.GonderenTelefon = EvTel.Numara;
                                    }
                                    else
                                    {
                                        var IsTel = potMusDetay.PotansiyelMusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Is).FirstOrDefault();
                                        if (IsTel != null)
                                        {
                                            model.GonderenTelefon = IsTel.Numara;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            #endregion

            model.DurumTipleri = new SelectList(RaporListProvider.GetIsDurumList(), "Value", "Text", "2").ToList();
            model.OncelikSeviyeleri = new SelectList(RaporListProvider.GetOncelikSeviyeleri(), "Value", "Text", "1").ToList();
            model.IsTipleri = new SelectList(_GorevTakipService.GetIsTipleri(), "TipKodu", "TipAciklama", "1").ToList();
            model.IsAlanTvmKodu = _AktifKullaniciService.TVMKodu;
            model.IsAlanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
            var kullanicilarlist = (_KullaniciService.GetListKullanicilarTeklifAra());
            List<SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
            model.SigortaSirketeri = new SelectList(SSirketler, "SirketKodu", "SirketAdi").ListWithOptionLabel();
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            model.BransKodlari = new SelectList(brans, "BransKodu", "BransAdi", "").ListWithOptionLabel();
            model.IsAlanTvmKodlari = new SelectList(_TVMService.GetTVMListeNeosinerji(0).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.IsAlanTvmKodu).ToList();
            model.IsAlanKullaniciKodlari = new SelectList(kullanicilarlist, "KullaniciKodu", "AdiSoyadi", model.IsAlanKullaniciKodu).ListWithOptionLabel();
            model.TalepKanallari = new SelectList(_GorevTakipService.GetTalepKanallari(), "Kodu", "Adi", "1").ToList();

            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.GorevAtama, SekmeKodu = 0)]
        public ActionResult Ekle(GorevTakipModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AtananIsler atananIs = new AtananIsler();
                    atananIs.Aciklama = model.Aciklama;
                    atananIs.AtamaTarihi = TurkeyDateTime.Now;
                    atananIs.BaslamaTarihi = model.BaslamaTarihi;
                    atananIs.TahminiBitisTarihi = model.TahminiBitisTarihi;
                    atananIs.TamamlanmaTarihi = model.TamamlamaTarihi;
                    atananIs.Baslik = model.Baslik;
                    atananIs.BransKodu = model.BransKodu;
                    atananIs.Durum = 1;//Beklemede
                    atananIs.EkNo = model.ZeylNumarasi;
                    atananIs.YenilemeNo = model.YenilemeNumarasi;
                    atananIs.PoliceNumarasi = model.PoliceNumarasi;
                    atananIs.SigortaSirketKodu = model.SigortaSirketiKodu;
                    atananIs.OncelikSeviyesi = model.OncelikSeviyesi;
                    atananIs.TalepYapanAcente = model.TalepYapanAcente;
                    atananIs.GonderenAdiSoyadi = model.GonderenAdiSoyadi;
                    atananIs.GonderenEmail = model.GonderenEmail;
                    atananIs.GonderenTel = model.GonderenTelefon;
                    atananIs.GonderenTCVKN = model.GonderenTCVKN;
                    atananIs.EvrakNo = model.EvrakNo;
                    atananIs.IsTipi = model.IsTipi;
                    atananIs.IsAlanTVMKodu = model.IsAlanTvmKodu;
                    atananIs.IsAlanKullaniciKodu = model.IsAlanKullaniciKodu;
                    atananIs.IsAtayanTVMKodu = _AktifKullaniciService.TVMKodu;
                    atananIs.IsAtayanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                    atananIs.TalepKanaliKodu = model.TalepKanalKodu;
                    var result = _GorevTakipService.AddAtananIs(atananIs);
                    if (result != null)
                    {
                        return RedirectToAction("Detay", "GorevTakip", new { id = result.IsId });
                    }
                }
            }
            catch (Exception)
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }
            ModelState.AddModelError("", babonline.Message_RequiredValues);
            model.DurumTipleri = new SelectList(RaporListProvider.GetIsDurumList(), "Value", "Text", model.Durum).ToList();
            model.OncelikSeviyeleri = new SelectList(RaporListProvider.GetOncelikSeviyeleri(), "Value", "Text", model.OncelikSeviyesi).ToList();
            model.IsTipleri = new SelectList(_GorevTakipService.GetIsTipleri(), "TipKodu", "TipAciklama", model.IsTipi).ToList();

            model.IsAlanTvmKodu = _AktifKullaniciService.TVMKodu;
            model.IsAlanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
            var kullanicilarlist = (_KullaniciService.GetListKullanicilarTeklifAra());
            List<SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
            model.SigortaSirketeri = new SelectList(SSirketler, "SirketKodu", "SirketAdi", model.SigortaSirketiKodu).ListWithOptionLabel();
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            model.BransKodlari = new SelectList(brans, "BransKodu", "BransAdi", model.BransKodu).ListWithOptionLabel();
            model.IsAlanTvmKodlari = new SelectList(_TVMService.GetTVMListeNeosinerji(0).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.IsAlanTvmKodu).ToList();
            model.IsAlanKullaniciKodlari = new SelectList(kullanicilarlist, "KullaniciKodu", "AdiSoyadi", model.IsAlanKullaniciKodu).ListWithOptionLabel();
            model.TalepKanallari = new SelectList(_GorevTakipService.GetTalepKanallari(), "Kodu", "Adi", model.TalepKanalKodu).ToList();
            return View(model);
        }
        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.GorevAtama, SekmeKodu = 0)]

        public ActionResult Detay(int id)
        {
            GorevTakipDetayModel model = new GorevTakipDetayModel();
            AtananIsler isDetay = _GorevTakipService.getIsDetay(id);
            if (isDetay != null)
            {
                var talepKanalList = _GorevTakipService.GetTalepKanallari();
                var istipList = _GorevTakipService.GetIsTipleri();
                List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());

                model.Aciklama = isDetay.Aciklama;
                model.AtamaTarihi = isDetay.AtamaTarihi;
                model.BaslamaTarihi = isDetay.BaslamaTarihi;
                model.BransKodu = isDetay.BransKodu;
                if (isDetay.BransKodu.HasValue)
                {
                    var bransDetay = brans.Where(s => s.BransKodu == isDetay.BransKodu).FirstOrDefault();
                    if (bransDetay != null)
                    {
                        model.BransAdi = bransDetay.BransAdi;
                    }
                }

                model.Baslik = isDetay.Baslik;
                model.Durum = isDetay.Durum;
                model.DurumAciklama = RaporListProvider.GetDurumAciklamaDetay(isDetay.Durum);
                model.GonderenTCVKN = isDetay.GonderenTCVKN;
                model.GonderenAdiSoyadi = isDetay.GonderenAdiSoyadi;
                model.TalepYapanAcente = isDetay.TalepYapanAcente;
                model.GonderenEmail = isDetay.GonderenEmail;
                model.GonderenTelefon = isDetay.GonderenTel;
                model.EvrakNo = isDetay.EvrakNo;
                model.IsAlanKullaniciKodu = isDetay.IsAlanKullaniciKodu;
                if (isDetay.IsAlanKullaniciKodu.HasValue)
                {
                    var kDetay = _KullaniciService.GetKullanici(isDetay.IsAlanKullaniciKodu.Value);
                    model.IsAlanKullaniciUnvani = kDetay.Adi + " " + kDetay.Soyadi;
                }

                model.IsAlanTvmKodu = isDetay.IsAlanTVMKodu;
                var tvmDetay = _TVMService.GetDetay(isDetay.IsAlanTVMKodu);
                model.IsAlanTvmUnvani = tvmDetay.Unvani;

                model.IsAtayanKullaniciKodu = isDetay.IsAtayanKullaniciKodu;
                var kulDetay = _KullaniciService.GetKullanici(isDetay.IsAtayanKullaniciKodu);
                model.IsAtayanKullaniciUnvani = kulDetay.Adi + " " + kulDetay.Soyadi;
                model.IsAtayanTvmKodu = isDetay.IsAtayanTVMKodu;
                var tDetay = _TVMService.GetDetay(isDetay.IsAtayanTVMKodu);
                model.IsAtayanTvmUnvani = tDetay.Unvani;


                model.IsId = isDetay.IsId;
                model.IsTipi = isDetay.IsTipi;

                if (istipList != null)
                {
                    var istipDetay = istipList.Where(w => w.TipKodu == isDetay.IsTipi).FirstOrDefault();
                    if (istipDetay != null)
                    {
                        model.IsTipiAciklama = istipDetay.TipAciklama;
                    }
                }

                model.OncelikSeviyesi = isDetay.OncelikSeviyesi;
                model.OncelikSeviyeAciklama = RaporListProvider.GetOncelikSeviyeDetay(isDetay.OncelikSeviyesi);
                model.PoliceNumarasi = isDetay.PoliceNumarasi;
                model.SigortaSirketiKodu = isDetay.SigortaSirketKodu;
                if (!String.IsNullOrEmpty(model.SigortaSirketiKodu))
                {
                    var sirketDetay = _SigortaSirketleriService.GetSirket(model.SigortaSirketiKodu.PadLeft(3, '0'));
                    model.SigortaSirketiUnvani = sirketDetay.SirketAdi;
                }

                model.TahminiBitisTarihi = isDetay.TahminiBitisTarihi;
                model.TalepKanaliKodu = isDetay.TalepKanaliKodu;
                if (talepKanalList != null)
                {
                    var talepDetay = talepKanalList.Where(w => w.Kodu == isDetay.TalepKanaliKodu).FirstOrDefault();
                    if (talepDetay != null)
                    {
                        model.TalepKanali = talepDetay.Adi;
                    }
                }
                model.TamamlamaTarihi = isDetay.TamamlanmaTarihi;
                model.YenilemeNumarasi = isDetay.YenilemeNo;
                model.ZeylNumarasi = isDetay.EkNo;
                model.DokumanlariList = DokumanList(id, "detay");
                model.NotlarList = NotList(model.IsId, "detay");
            }
            else
            {
                model.DokumanlariList = new AtananIsDokumanlarListModel();
            }

            return View(model);
        }
        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.GorevAtama, SekmeKodu = 0)]

        public ActionResult Guncelle(int id)
        {
            GorevTakipGuncelleModel model = new GorevTakipGuncelleModel();
            AtananIsler isDetay = _GorevTakipService.getIsDetay(id);
            try
            {
                model.Aciklama = isDetay.Aciklama;
                model.AtamaTarihi = isDetay.AtamaTarihi;
                model.BaslamaTarihi = isDetay.BaslamaTarihi;
                model.Baslik = isDetay.Baslik;
                model.BransKodu = isDetay.BransKodu;
                model.Durum = isDetay.Durum;
                model.TalepYapanAcente = isDetay.TalepYapanAcente;
                model.GonderenTCVKN = isDetay.GonderenTCVKN;
                model.GonderenAdiSoyadi = isDetay.GonderenAdiSoyadi;
                model.EvrakNo = isDetay.EvrakNo;
                model.GonderenEmail = isDetay.GonderenEmail;
                model.GonderenTelefon = isDetay.GonderenTel;
                model.IsAlanKullaniciKodu = isDetay.IsAlanKullaniciKodu.Value;
                model.IsAlanTvmKodu = isDetay.IsAlanTVMKodu;
                model.IsId = id;
                model.IsTipi = isDetay.IsTipi;
                model.OncelikSeviyesi = isDetay.OncelikSeviyesi;
                model.PoliceNumarasi = isDetay.PoliceNumarasi;
                model.SigortaSirketiKodu = isDetay.SigortaSirketKodu;
                model.TahminiBitisTarihi = isDetay.TahminiBitisTarihi;
                model.TalepKanalKodu = isDetay.TalepKanaliKodu;
                model.TamamlamaTarihi = isDetay.TamamlanmaTarihi;
                model.YenilemeNumarasi = isDetay.YenilemeNo;
                model.ZeylNumarasi = isDetay.EkNo;

                model.DurumTipleri = new SelectList(RaporListProvider.GetIsDurumList(), "Value", "Text", model.Durum).ToList();
                model.OncelikSeviyeleri = new SelectList(RaporListProvider.GetOncelikSeviyeleri(), "Value", "Text", model.OncelikSeviyesi).ToList();
                model.IsTipleri = new SelectList(_GorevTakipService.GetIsTipleri(), "TipKodu", "TipAciklama", model.IsTipi).ToList();

                model.IsAlanTvmKodu = _AktifKullaniciService.TVMKodu;
                var kullanicilarlist = (_KullaniciService.GetListKullanicilarTeklifAra());
                List<SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
                model.SigortaSirketeri = new SelectList(SSirketler, "SirketKodu", "SirketAdi", model.SigortaSirketiKodu).ListWithOptionLabel();
                List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
                model.BransKodlari = new SelectList(brans, "BransKodu", "BransAdi", model.BransKodu).ListWithOptionLabel();
                model.IsAlanTvmKodlari = new SelectList(_TVMService.GetTVMListeNeosinerji(0).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.IsAlanTvmKodu).ToList();
                model.IsAlanKullaniciKodlari = new SelectList(kullanicilarlist, "KullaniciKodu", "AdiSoyadi", model.IsAlanKullaniciKodu).ListWithOptionLabel();
                model.TalepKanallari = new SelectList(_GorevTakipService.GetTalepKanallari(), "Kodu", "Adi", model.TalepKanalKodu).ToList();
                model.DokumanlariList = DokumanList(id, "guncelle");
                model.NotlarList = NotList(model.IsId, "guncelle");
            }
            catch (Exception)
            { }
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.GorevAtama, SekmeKodu = 0)]
        public ActionResult Guncelle(GorevTakipGuncelleModel model)
        {
            try
            {
                var getIsDetay = _GorevTakipService.getIsDetay(model.IsId);
                getIsDetay.Aciklama = model.Aciklama;
                getIsDetay.AtamaTarihi = TurkeyDateTime.Now;
                getIsDetay.BaslamaTarihi = model.BaslamaTarihi;
                getIsDetay.Baslik = model.Baslik;
                getIsDetay.TahminiBitisTarihi = model.TahminiBitisTarihi;
                getIsDetay.TamamlanmaTarihi = model.TamamlamaTarihi;
                getIsDetay.BransKodu = model.BransKodu;
                getIsDetay.Durum = model.Durum;
                getIsDetay.EkNo = model.ZeylNumarasi;
                getIsDetay.YenilemeNo = model.YenilemeNumarasi;
                getIsDetay.PoliceNumarasi = model.PoliceNumarasi;
                getIsDetay.SigortaSirketKodu = model.SigortaSirketiKodu;
                getIsDetay.OncelikSeviyesi = model.OncelikSeviyesi;
                getIsDetay.IsTipi = model.IsTipi;
                getIsDetay.TalepYapanAcente = model.TalepYapanAcente;
                getIsDetay.GonderenTCVKN = model.GonderenTCVKN;
                getIsDetay.GonderenAdiSoyadi = model.GonderenAdiSoyadi;
                getIsDetay.EvrakNo = model.EvrakNo;
                getIsDetay.GonderenEmail = model.GonderenEmail;
                getIsDetay.GonderenTel = model.GonderenTelefon;
                getIsDetay.IsAlanTVMKodu = model.IsAlanTvmKodu;
                getIsDetay.IsAlanKullaniciKodu = model.IsAlanKullaniciKodu;
                getIsDetay.IsAtayanTVMKodu = _AktifKullaniciService.TVMKodu;
                getIsDetay.IsAtayanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                var result = _GorevTakipService.UpdateAtananIs(getIsDetay);
                if (result)
                {
                    return RedirectToAction("Detay", "GorevTakip", new { id = model.IsId });
                }
            }
            catch (Exception ex)
            {
                return new RedirectResult("~/Error/ErrorPage/500");
            }
            model.DurumTipleri = new SelectList(RaporListProvider.GetIsDurumList(), "Value", "Text", model.Durum).ToList();
            model.OncelikSeviyeleri = new SelectList(RaporListProvider.GetOncelikSeviyeleri(), "Value", "Text", model.OncelikSeviyesi).ToList();
            model.IsTipleri = new SelectList(_GorevTakipService.GetIsTipleri(), "TipKodu", "TipAciklama", model.IsTipi).ToList();

            model.IsAlanTvmKodu = _AktifKullaniciService.TVMKodu;
            var kullanicilarlist = (_KullaniciService.GetListKullanicilarTeklifAra());
            List<SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
            model.SigortaSirketeri = new SelectList(SSirketler, "SirketKodu", "SirketAdi", model.SigortaSirketiKodu).ListWithOptionLabel();
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            model.BransKodlari = new SelectList(brans, "BransKodu", "BransAdi", model.BransKodu).ListWithOptionLabel();
            model.IsAlanTvmKodlari = new SelectList(_TVMService.GetTVMListeNeosinerji(0).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.IsAlanTvmKodu).ToList();
            model.IsAlanKullaniciKodlari = new SelectList(kullanicilarlist, "KullaniciKodu", "AdiSoyadi", model.IsAlanKullaniciKodu).ListWithOptionLabel();
            model.TalepKanallari = new SelectList(_GorevTakipService.GetTalepKanallari(), "Kodu", "Adi", model.TalepKanalKodu).ToList();
            model.DokumanlariList = DokumanList(model.IsId, "guncelle");
            model.NotlarList = NotList(model.IsId, "guncelle");

            return View(model);
        }

        #region Dokuman Ekleme
        public ActionResult Dokuman(int id, int isAlanTvmKodu)
        {
            AtananIsDokumanlarModel model = new AtananIsDokumanlarModel();
            model.IsId = id;
            model.IsAlanTvmkodu = isAlanTvmKodu;
            return PartialView("_AtananIsDokumanEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult Dokuman(AtananIsDokumanlarModel model, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileName(file.FileName);

                if (_TVMService.CheckedFileName(fileName))
                {

                    var tvmDetay = _TVMService.GetDetay(model.IsAlanTvmkodu);
                    string folderName = "";
                    if (tvmDetay != null)
                    {
                        if (tvmDetay.BagliOlduguTVMKodu == -9999)
                        {
                            folderName = tvmDetay.Kodu.ToString();
                        }
                        else
                        {
                            folderName = tvmDetay.BagliOlduguTVMKodu.ToString() + "/" + tvmDetay.Kodu.ToString();
                        }
                    }
                    string url = _GorevTakipDokumanStorage.UploadFile(folderName, fileName, file.InputStream);
                    AtananIsDokumanlar dokuman = new AtananIsDokumanlar();
                    dokuman.IsId = model.IsId;
                    //Tvm ile ilgili bililer otomatik gelicek....
                    dokuman.EkleyenPersonelKodu = _AktifKullaniciService.KullaniciKodu;
                    dokuman.EklemeTarihi = TurkeyDateTime.Now;
                    dokuman.DokumanURL = url;
                    dokuman.SiraNo = 1;
                    var extensionParts = file.FileName.Split('.');
                    string extension = "";
                    if (extensionParts != null)
                    {
                        extension = extensionParts[extensionParts.Length - 1].ToString();
                    }
                    dokuman.DokumanAdi = model.DokumanAdi + "." + extension.ToLower();

                    if (_GorevTakipService.GetListDokumanlar(model.IsId).Count() != 0)
                        dokuman.SiraNo = _GorevTakipService.GetListDokumanlar(model.IsId).Select(s => s.SiraNo).Max() + 1;

                    _GorevTakipService.CreateDokuman(dokuman);
                    //Kayıt Başarılı ise detay sayfasına gönderiliyor...
                    return null;
                }
                else
                {
                    ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
                    return View(model);
                }
            }
            //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
            ModelState.AddModelError("", babonline.Message_DocumentSaveError);
            return View(model);
        }

        //public ActionResult DokumanGuncelle(int SiraNo, int isId)
        //{
        //    AtananIsDokumanlar dokuman = _GorevTakipService.GetTVMDokuman(SiraNo, isId);

        //    Mapper.CreateMap<AtananIsDokumanlar, AtananIsDokumanlarModel>();
        //    AtananIsDokumanlarModel model = Mapper.Map<AtananIsDokumanlar, AtananIsDokumanlarModel>(dokuman);

        //    return PartialView("_AtananIsDokumanEkle", model);
        //}

        //[HttpPost]
        //[AjaxException]
        //[ValidateAntiForgeryToken]
        //public ActionResult DokumanGuncelle(AtananIsDokumanlarModel model, HttpPostedFileBase file)
        //{
        //    if (ModelState.IsValid && file.ContentLength > 0)
        //    {
        //        string fileName = System.IO.Path.GetFileName(file.FileName);

        //        if (_TVMService.CheckedFileName(fileName))
        //        {
        //            string url = "";// _Storage.UploadFile(model.TVMKodu.ToString(), fileName, file.InputStream);
        //            AtananIsDokumanlar dokuman = new AtananIsDokumanlar();
        //            dokuman.IsId = model.IsId;
        //            //Tvm ile ilgili bililer otomatik gelicek....
        //            dokuman.EkleyenPersonelKodu = 1;
        //            dokuman.EklemeTarihi = TurkeyDateTime.Now;

        //            //dokuman.DokumanAdi = fileName;
        //            dokuman.DokumanAdi = model.DokumanAdi;
        //            dokuman.Dokuman = url;
        //            dokuman.SiraNo = 1;

        //            if (_GorevTakipService.GetListDokumanlar(model.IsId).Count() != 0)
        //                dokuman.SiraNo = _GorevTakipService.GetListDokumanlar(model.IsId).Select(s => s.SiraNo).Max() + 1;

        //            //Kayıt Başarılı ise detay sayfasına gönderiliyor...
        //            return null;
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("", babonline.Message_File_AlreadyExists);
        //            return View(model);
        //        }
        //    }
        //    //Kayıt Sırasında bilgiler eksikse hata bilgisi geri dondürülüyor..
        //    ModelState.AddModelError("", babonline.Message_DocumentSaveError);
        //    return View(model);
        //}

        [HttpPost]
        [AjaxException]
        public ActionResult DokumanSil(int dokumanKodu, int isId)
        {
            _GorevTakipService.DeleteDokuman(dokumanKodu, isId);
            AtananIsDokumanlarListModel model = DokumanList(isId, "detay");
            model.sayfaAdi = "guncelle";
            return PartialView("_AtananIsDokumanlar", model);
        }
        public AtananIsDokumanlarListModel DokumanList(int isId, string sayfaAdi)
        {
            List<AtananIsDokumanlar> dokumanlar = _GorevTakipService.GetListDokumanlar(isId).ToList();

            AtananIsDokumanlarListModel model = new AtananIsDokumanlarListModel();
            Mapper.CreateMap<AtananIsDokumanlar, AtananIsDokumanlarModel>();
            model.Items = Mapper.Map<List<AtananIsDokumanlar>, List<AtananIsDokumanlarModel>>(dokumanlar);
            if (model.Items.Count > 0)
            {
                var kullaniciListesi = _KullaniciService.GetTVMKullanicilari(_AktifKullaniciService.TVMKodu);
                foreach (var item in model.Items)
                {
                    var kullaniciDetay = kullaniciListesi.Where(w => w.KullaniciKodu == item.EkleyenPersonelKodu).FirstOrDefault();
                    if (kullaniciDetay != null)
                    {
                        item.EkleyenPersonelAdi = kullaniciDetay.AdiSoyadi;
                    }
                }
            }

            model.sayfaAdi = sayfaAdi;
            return model;
        }


        [AjaxException]
        public ActionResult DokumanView(int id, string sayfaAdi)
        {
            return PartialView("_AtananIsDokumanlar", DokumanList(id, sayfaAdi));
        }

        #endregion

        #region Not Ekleme
        public ActionResult NotEkle(int id)
        {
            AtananIsNotlarModel model = new AtananIsNotlarModel();
            model.IsId = id;
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMPersonelKodu = _AktifKullaniciService.KullaniciKodu;
            return PartialView("_NotEkle", model);
        }

        [HttpPost]
        [AjaxException]
        [ValidateAntiForgeryToken]
        public ActionResult NotEkle(AtananIsNotlarModel model)
        {
            if (ModelState.IsValid)
            {
                AtananIsNotlar not = new AtananIsNotlar();
                not.IsId = model.IsId;
                not.TVMKodu = model.TVMKodu;
                not.TVMPersonelKodu = model.TVMPersonelKodu;
                not.KayitTarihi = TurkeyDateTime.Now;
                not.Konu = model.Konu;
                not.NotAciklamasi = model.NotAciklamasi;
                _GorevTakipService.CreateNot(not);
                //Kayıt Başarılı ise detay sayfasına gönderiliyor...
                return null;
            }
            else
            {
                ModelState.AddModelError("", babonline.PleaseFillInTheRequiredFields);
                return View(model);
            }

        }
        private AtananIsNotlarListModel NotList(int isId, string sayfaAdi)
        {
            List<AtananIsNotlar> notlar = _GorevTakipService.GetListNotlar(isId).ToList();
            AtananIsNotlarListModel model = new AtananIsNotlarListModel();
            Mapper.CreateMap<AtananIsNotlar, AtananIsNotlarModel>();
            model.Items = Mapper.Map<List<AtananIsNotlar>, List<AtananIsNotlarModel>>(notlar);
            if (model.Items.Count > 0)
            {
                var kullaniciListesi = _KullaniciService.GetTVMKullanicilari(_AktifKullaniciService.TVMKodu);
                foreach (var item in model.Items)
                {
                    var kullaniciDetay = kullaniciListesi.Where(w => w.KullaniciKodu == item.TVMPersonelKodu).FirstOrDefault();
                    if (kullaniciDetay != null)
                    {
                        item.EkleyenPersonelAdi = kullaniciDetay.AdiSoyadi;
                    }
                }
            }
            if (_AktifKullaniciService.Gorevi == KullaniciGorevTipleri.Yonetici)
            {
                model.AdminKullanicimi = true;
            }
            else
            {
                model.AdminKullanicimi = false;
            }
            model.sayfaAdi = sayfaAdi;
            return model;
        }
        [AjaxException]
        public ActionResult NotView(int id, string sayfaAdi)
        {
            return PartialView("_Notlar", NotList(id, sayfaAdi));
        }
        [HttpPost]
        [AjaxException]
        public ActionResult NotSil(int notId, int isId)
        {
            _GorevTakipService.DeleteNot(notId);
            AtananIsNotlarListModel model = NotList(isId, "detay");
            model.sayfaAdi = "guncelle";
            return PartialView("_Notlar", model);
        }
        #endregion

        public ActionResult GetMusteriBilgi(string kimlikNo, int tvmKodu)
        {
            MusteriBilgi bilgi = new MusteriBilgi();
            var musteriDetay = _GorevTakipService.GetMusteriBilgi(kimlikNo, tvmKodu);
            if (musteriDetay != null)
            {
                bilgi.Adi = musteriDetay.Adi;
                bilgi.Soyadi = musteriDetay.Soyadi;
            }
            return Json(bilgi, JsonRequestBehavior.AllowGet);
        }
        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.Islerim, SekmeKodu = 0)]
        public ActionResult Islerim()
        {
            List<IslerimProcedureModel> list = _GorevTakipService.IslerimPagedList();
            IsModel isModel = new IsModel();
            IslerimDetayModel model = new IslerimDetayModel();
            if (list != null)
            {
                var GroupList = list.GroupBy(ac => new
                {
                    ac.OncelikSeviyesi,
                    ac.Durum
                }).Select(ac => new
                {
                    OncelikSeviyesi = ac.Key.OncelikSeviyesi,
                    Durum = ac.Key.Durum,
                    list = ac.ToList()
                }).OrderBy(s => s.OncelikSeviyesi).ToList();

                foreach (var item in GroupList)
                {
                    foreach (var items in item.list)
                    {
                        isModel = new IsModel();
                        isModel.OncelikSeviyeKodu = item.OncelikSeviyesi;
                        isModel.OncelikSeviyesi = RaporListProvider.GetOncelikSeviyeDetay(item.OncelikSeviyesi);
                        isModel.DurumKodu = item.Durum;
                        isModel.Durum = RaporListProvider.GetDurumAciklamaDetay(item.Durum);
                        isModel.IsNumarasi = items.IsNumarasi.ToString();
                        isModel.Baslik = items.Baslik;
                        isModel.IsTipi = items.TipAciklama;
                        isModel.TalepKanali = items.TalepAdi;
                        isModel.BaslamaTarihi = items.BaslamaTarihi.ToString("dd.MM.yyyy");
                        isModel.TahminiBitisTarihi = items.TahminiBitisTarihi.ToString("dd.MM.yyyy");
                        isModel.TalepYapanAcenteUnvani = items.TalepYapanAcente;
                        model.list.Add(isModel);
                    }
                    //isModel = new IsModel();
                    //isModel.IsNumarasi = item.IsNumarasi.ToString();
                    //isModel.Baslik = item.Baslik;
                    //isModel.Durum = RaporListProvider.GetDurumAciklamaDetay(item.Durum);
                    //isModel.OncelikSeviyesi = RaporListProvider.GetOncelikSeviyeDetay(item.OncelikSeviyesi);
                    //isModel.IsTipi = item.TipAciklama;
                    //isModel.TalepKanali = item.TalepAdi;
                    //isModel.AtamaTarihi = item.AtamaTarihi.ToString("dd.MM.yyyy");
                    //isModel.BaslamaTarihi = item.BaslamaTarihi.ToString("dd.MM.yyyy");
                    //isModel.TahminiBitisTarihi = item.TahminiBitisTarihi.ToString("dd.MM.yyyy");
                    //model.list.Add(isModel);
                }
            }
            return View(model);
        }
        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.GorevDagilimRaporu, SekmeKodu = 0)]
        public ActionResult GorevDagilimRaporu()
        {
            AtananIslerModel model = new AtananIslerModel();

            model.IsBaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.IsBasBitisTarihi = TurkeyDateTime.Now;
            model.RaporSonuc = new List<AtananIslerProcedureModel>();

            model.Durumlar = new SelectList(RaporListProvider.GetIsDurumList(), "Value", "Text", "").ListWithOptionLabel();
            model.OncelikSeviyeleri = new SelectList(RaporListProvider.GetOncelikSeviyeleri(), "Value", "Text", "").ListWithOptionLabel();
            model.IsTipleri = new SelectList(_GorevTakipService.GetIsTipleri(), "TipKodu", "TipAciklama", "").ListWithOptionLabel();

            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
            model.Tvmler = new SelectList(_TVMService.GetTVMListeNeosinerji(0).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.TVMKodu).ToList();
            var kullanicilarlist = (_KullaniciService.GetListKullanicilarTeklifAra());
            model.KullaniciList = new SelectList(kullanicilarlist, "KullaniciKodu", "AdiSoyadi", model.TVMKullaniciKodu).ListWithOptionLabel();
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.GorevAtamaSistemi, AltMenuKodu = AltMenuler.GorevDagilimRaporu, SekmeKodu = 0)]
        public ActionResult ListePagerGorevDagilimRaporu()
        {
            if (Request["sEcho"] != null)
            {
                AtananIslerimListe arama = new AtananIslerimListe(Request, new Expression<Func<AtananIslerProcedureModel, object>>[]
                                                                    {
                                                                        t=>t.IsNumarasi,
                                                                        t=>t.Baslik,
                                                                           t=>t.TalepYapanAcente,
                                                                        t => t.IsAlanTvmUnvani,
                                                                        t => t.IsAlanKullaniciUnvani,
                                                                        t => t.BransAdi,
                                                                        t => t.IsTipi,
                                                                        t => t.Durum,
                                                                        t => t.OncelikSeviyesi,
                                                                        t => t.BaslamaTarihi,
                                                                        t => t.TahminiBitisTarihi,
                                                                        t => t.TamamlanmaTarihi,
                                                                        t => t.IseGecBaslamaGunSayisi,
                                                                        t => t.IsiGecBitirmeGunSayisi,
                                                                        t => t.IsBitimineKalanGunSayisi,
                                                                        });

                arama.IsBaslangicTarihi = arama.TryParseParamDate("IsBaslangicTarihi");
                arama.IsBasBitisTarihi = arama.TryParseParamDate("IsBasBitisTarihi");
                arama.Durum = arama.TryParseParamByte("Durum");
                arama.IsTipi = arama.TryParseParamByte("IsTipi");
                arama.OncelikSeviyesi = arama.TryParseParamByte("OncelikSeviyesi");
                arama.TvmKodu = arama.TryParseParamInt("TVMKodu");
                arama.KullaniciList = arama.TryParseParamInt("TVMKullaniciKodu");

                int totalRowCount = 0;
                List<AtananIslerProcedureModel> list = _GorevTakipService.GorevDagilimRaporuPagedList(arama, out totalRowCount);
                var bransList = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
                //var sigortaSirketList = _SigortaSirketleriService.GetList();
                var isTipList = _GorevTakipService.GetIsTipleri();
                arama.AddFormatter(f => f.Baslik, f => String.Format("{0}", !String.IsNullOrEmpty(f.Baslik) && f.Baslik.Length > 9 ? "<span title ='" + f.Baslik + "'><b>" + f.Baslik.Substring(0, 8) + "...</b></span>" : f.Baslik));
                arama.AddFormatter(f => f.IsNumarasi, f => String.Format("{0}", " <a  href='/GorevTakip/GorevTakip/Detay/" + f.IsNumarasi + "'>" + f.IsNumarasi + "</a>"));
                arama.AddFormatter(f => f.TamamlanmaTarihi, f => String.Format("{0}", f.TamamlanmaTarihi.HasValue ? f.TamamlanmaTarihi.Value.ToString("dd.MM.yyyy") : ""));
                arama.AddFormatter(f => f.BaslamaTarihi, f => String.Format("{0}", f.BaslamaTarihi.ToString("dd.MM.yyyy")));
                arama.AddFormatter(f => f.TahminiBitisTarihi, f => String.Format("{0}", f.TahminiBitisTarihi.ToString("dd.MM.yyyy")));
                arama.AddFormatter(f => f.IsBitimineKalanGunSayisi, f => String.Format("{0}", f.IsBitimineKalanGunSayisi >= 0 ? f.IsBitimineKalanGunSayisi : 0));
                arama.AddFormatter(f => f.AtamaTarihi, f => String.Format("{0}", f.AtamaTarihi.ToString("dd.MM.yyyy")));
                arama.AddFormatter(f => f.Durum, f => String.Format("{0}", "<span style=' color:white; background-color: " + _GorevTakipService.DurumRenk(f.Durum) + "' ><b>" + RaporListProvider.GetDurumAciklama(f.Durum) + "</b></span>"));
                arama.AddFormatter(f => f.OncelikSeviyesi, f => String.Format("{0}", "<span style='color:white; background-color: " + _GorevTakipService.OncelikSeviyesiRenk(f.OncelikSeviyesi) + "' ><b>" + RaporListProvider.GetOncelikSeviye(f.OncelikSeviyesi) + "</b></span>"));
                arama.AddFormatter(f => f.IsTipi, f => String.Format("{0}", isTipList != null ? isTipList.Where(w => w.TipKodu == f.IsTipi).FirstOrDefault().TipAciklama : ""));
                arama.AddFormatter(f => f.BransAdi, f => String.Format("{0}", f.BransKodu != null ? this.getBransAdi(f.BransKodu.Value, bransList) : ""));
                //arama.AddFormatter(f => f.SigortaSirketAdi, f => String.Format("{0}", !String.IsNullOrEmpty(f.SigortaSirketKodu) ? this.getSirketAdi(f.SigortaSirketKodu, sigortaSirketList) : ""));

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }
        public ActionResult GetListTVMKullanicilari(int tvmKodu)
        {
            List<int> tvmKodlari = new List<int>();
            if (!String.IsNullOrEmpty(""))
            {
                //string[] parts = model.TvmlerSelectList.ToString().Split(',');
                //foreach (var item in parts)
                //{
                //    if (item != "multiselect-all")
                //    {
                //        tvmKodlari.Add(Convert.ToInt32(item));
                //    }
                //}
            }
            // var kullaniciList = new SelectList(_KullaniciService.GetListTVMKullanicilari(tvmKodu), "KullaniciKodu", "AdiSoyadi").ListWithOptionLabel();
            var kullaniciList = new SelectList(_KullaniciService.GetListTVMKullanicilari(tvmKodu), "KullaniciKodu", "AdiSoyadi").ListWithOptionLabel();
            return Json(new { list = kullaniciList }, JsonRequestBehavior.AllowGet);

            //return Json(new SelectList(_KullaniciService.GetListTVMKullanicilari(tvmKodu), "KullaniciKodu", "AdiSoyadi"), JsonRequestBehavior.AllowGet);
        }

        public string getBransAdi(int bransKodu, List<Bran> bransList)
        {
            var bransDetay = bransList.Where(w => w.BransKodu == bransKodu).FirstOrDefault();
            if (bransDetay != null)
            {
                return bransDetay.BransAdi;
            }
            return "";
        }
        //public string getSirketAdi(string sirketKodu, List<SigortaSirketleri> sirketList)
        //{
        //    var sirketDetay = sirketList.Where(w => w.SirketKodu == sirketKodu).FirstOrDefault();
        //    if (sirketDetay != null)
        //    {
        //        return sirketDetay.SirketAdi;
        //    }
        //    return "";
        //}

    }
}