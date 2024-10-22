using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Business;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Service;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using System.Xml;
using Neosinerji.BABOnlineTP.Web.Areas.TaliAcente.Models;
using Neosinerji.BABOnlineTP.Business.TaliPolice;
using System.Globalization;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Neosinerji.BABOnlineTP.Web.Areas.TaliAcente.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = 0, SekmeKodu = 0)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class TaliAcenteController : Controller
    {
        //
        // GET: /TaliAcente/TaliAcente/
        public IMusteriService _MusteriService;
        public IAktifKullaniciService _AktifKullaniciService;
        public ITaliPoliceService _TaliPoliceService;
        public ITVMKullanicilarService _TVMKullanicilar;
        ITUMService _TUMService;
        ITVMService _TVMService;
        IBransService _BransService;
        IKomisyonService _KomisyonService;
        IPoliceService _PoliceService;
        ISigortaSirketleriService _SigortaSirketleriService;
        public TaliAcenteController(
                                IAktifKullaniciService aktifKullaniciService,
                                ITaliPoliceService taliPoliceService,
                                ITUMService TUMService,
                                IBransService bransService,
                                IKomisyonService komisyonService,
                                ITVMService tvmService,
                                IPoliceService policeService,
                                ISigortaSirketleriService sigortaSirketServis)
        {

            _AktifKullaniciService = aktifKullaniciService;
            _TaliPoliceService = taliPoliceService;
            _TUMService = TUMService;
            _BransService = bransService;
            _KomisyonService = komisyonService;
            _TVMService = tvmService;
            _PoliceService = policeService;
            _SigortaSirketleriService = sigortaSirketServis;

        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.UretimListesi, SekmeKodu = 0)]
        public ActionResult TaliAcenteIslem(string bordroTarihi)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();

            TaliAcenteModel model = new TaliAcenteModel();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            TaliPoliceListe tali;
            List<PoliceTaliAcenteler> taliPoliceler = new List<PoliceTaliAcenteler>();
            if (!String.IsNullOrEmpty(bordroTarihi))
            {
                taliPoliceler = _TaliPoliceService.GetPoliceTaliAcenteIslemTarih(_AktifKullaniciService.TVMKodu, Convert.ToDateTime(bordroTarihi));
            }
            else
            {
                taliPoliceler = _TaliPoliceService.GetPoliceTaliAcenteGunlukListe(_AktifKullaniciService.TVMKodu);
            }

            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();
            model.taliPoliceListe = new List<TaliPoliceListe>();

            foreach (var item in taliPoliceler)
            {
                tali = new TaliPoliceListe();
                tali.AdUnvan_ = item.AdUnvan_;
                tali.EkNo = item.EkNo;
                tali.GuncellemeTarihi = item.GuncellemeTarihi.HasValue ? item.GuncellemeTarihi.Value.ToString("dd/MM/yyyy") : "";
                tali.Id = item.Id;
                tali.KayitTarihi_ = item.KayitTarihi_.Value.ToString("dd/MM/yyyy");
                tali.KimlikNo = item.KimlikNo;
                tali.PoliceNo = item.PoliceNo;
                tali.SigortaSirketAdi = _SigortaSirketleriService.GetSirket(item.SigortaSirketNo_).SirketAdi;
                tali.SoyadUnvan = item.SoyadUnvan;
                tali.TVMKodu = item.TVMKodu;
                tali.TVMUnvan = item.TVMKodu.HasValue ? _TVMService.GetDetay(item.TVMKodu.Value).Unvani : "";

                model.taliPoliceListe.Add(tali);
            }

            model.BaslangicTarihi = TurkeyDateTime.Today;
            if (bordroTarihi != null)
            {
                model.BaslangicTarihi = Convert.ToDateTime(bordroTarihi);
            }

            var aktifKullaniciBagliOlduguTvmKodu = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
            if (aktifKullaniciBagliOlduguTvmKodu != null && aktifKullaniciBagliOlduguTvmKodu.BagliOlduguTVMKodu == -9999)
            {
                model.AnaTVMmi = true;
            }
            else
            {
                model.AnaTVMmi = false;
            }
            model.PoliceTVMKodu = _AktifKullaniciService.TVMKodu;
            model.PoliceTVMUnvani = _AktifKullaniciService.TVMUnvani;


            //int lang = 1;
            //string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            //if (!String.IsNullOrEmpty(language))
            //    switch (language)
            //    {
            //        case "tr": lang = 1; break;
            //        case "en": lang = 2; break;
            //        case "it": lang = 3; break;
            //        case "fr": lang = 4; break;
            //        case "es": lang = 5; break;
            //    }


            //ViewBag.dil = lang;

            return View(model);
        }

        [HttpPost]
        public ActionResult KimlikSorgula(string kimlikNo)
        {

            IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
            KimlikSorguResponse kimlik = mapfreSorgu.KimlikSorgu(NeosinerjiTVM.NeosinerjiTVMKodu, kimlikNo);
            TaliKimlikModel model = new TaliKimlikModel();

            if (kimlik != null && kimlikNo.Length == 11)
            {
                model.Ad = kimlik.vrgAd;
                model.Soyad = kimlik.vrgSoyAd;
                model.SorgulamaSonuc = true;
            }
            if (kimlik != null && kimlikNo.Length == 10)
            {
                model.Ad = kimlik.vrgnvan;
                model.Soyad = ".";
                model.SorgulamaSonuc = true;
            }
            if (kimlik == null)
            {

                model.SorgulamaHata("Kimlik bilgileri alınamadı.");
                return Json(model);
            }
            else if (kimlik != null && !String.IsNullOrEmpty(kimlik.hata))
            {
                model.SorgulamaHata(kimlik.hata);
                return Json(model);
            }
            else if (kimlik != null)
            {
                if (!String.IsNullOrEmpty(kimlik.vrgTCKimlikNo) && kimlik.vrgTCKimlikNo.Length == 11)
                {
                    if (!String.IsNullOrEmpty(kimlik.vrgAd) && kimlik.vrgAd.Length > 150)
                    {
                        model.Ad = kimlik.vrgAd.Substring(0, 150);
                    }
                    else
                    {
                        model.Ad = kimlik.vrgAd;
                    }
                    if (!String.IsNullOrEmpty(kimlik.vrgSoyAd) && kimlik.vrgSoyAd.Length > 50)
                    {
                        model.Soyad = kimlik.vrgSoyAd.Substring(0, 50);
                    }
                    else
                    {
                        model.Soyad = kimlik.vrgSoyAd;
                    }
                }
                else if (!String.IsNullOrEmpty(kimlik.vrgVergiNo) && kimlikNo.Length == 10)
                {
                    if (!String.IsNullOrEmpty(kimlik.vrgnvan) && kimlik.vrgnvan.Length > 150)
                    {
                        model.Ad = kimlik.vrgnvan.Substring(0, 150);
                    }
                    else
                    {
                        model.Ad = kimlik.vrgnvan;
                    }
                    model.Soyad = ".";
                }
            }
            return Json(model);
        }

        [HttpPost]
        public ActionResult TaliPoliceEkle(TaliAcenteModel model)
        {
            PoliceTaliAcenteler policeTali = new PoliceTaliAcenteler();
            PoliceTaliAcenteRapor policeTaliRapor = new PoliceTaliAcenteRapor();
            PoliceTaliAcenteRapor yeniTaliPoliceTaliRapor = new PoliceTaliAcenteRapor();
            int _TVMKodu = _AktifKullaniciService.TVMKodu;

            policeTali.TVMKodu = _TVMKodu;
            bool response = false;
            bool responseRapor = false;
            bool responseGuncelle = false;
            bool responseRaporGuncelle = false;
            string mesaj = string.Empty;
            DateTime kayitTarihi = Convert.ToDateTime(model.PoliceBordroKayitTarihi);

            #region Merkez acente alt acentesi adına işlem yapıyorsa
            if (model != null && model.AnaTVMmi && model.PoliceTVMKodu > 0)
            {
                policeTali.TVMKodu = model.PoliceTVMKodu;

                #region Guncelleme
                if (model.KayitId > 0)
                {
                    PoliceTaliAcenteler guncellenenTalininPolicesi = new PoliceTaliAcenteler();
                    guncellenenTalininPolicesi = _TaliPoliceService.GetPoliceTaliAcente(model.KayitId);

                    int guncellenenTalininTvmKodu = guncellenenTalininPolicesi.TVMKodu.Value;

                    guncellenenTalininPolicesi.AdUnvan_ = model.Ad;
                    guncellenenTalininPolicesi.EkNo = Convert.ToInt32(model.EkNo);
                    guncellenenTalininPolicesi.SoyadUnvan = model.Soyad;
                    guncellenenTalininPolicesi.PoliceNo = model.PoliceNo;
                    guncellenenTalininPolicesi.SigortaSirketNo_ = model.SigortaSirketiKodu;
                    guncellenenTalininPolicesi.KayitTarihi_ = Convert.ToDateTime(model.KayitTarihi);
                    guncellenenTalininPolicesi.KimlikNo = model.tcVkn;
                    guncellenenTalininPolicesi.Id = model.KayitId;
                    guncellenenTalininPolicesi.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                    guncellenenTalininPolicesi.TVMKodu = model.PoliceTVMKodu;
                    responseGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteler(guncellenenTalininPolicesi);
                    mesaj = "Poliçe Güncellendi";

                    policeTaliRapor = _TaliPoliceService.GetPoliceTaliAcenteRaporByDate(guncellenenTalininTvmKodu, Convert.ToDateTime(model.KayitTarihi));
                    if (policeTaliRapor != null)
                    {
                        policeTaliRapor.GuncellemeTarihi = TurkeyDateTime.Today;
                        if (policeTaliRapor.Police_EkAdedi == 0)
                        {
                            policeTaliRapor.UretimVAR_YOK = 0;
                        }
                        else
                        {
                            policeTaliRapor.UretimVAR_YOK = 1;
                        }
                        responseRaporGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteRapor(policeTaliRapor);
                    }

                }
                #endregion

                #region Yeni Kayıt

                else
                {
                    var kayitVarmi = _TaliPoliceService.TaliPoliceVarmi(model.PoliceNo, Convert.ToInt32(model.EkNo), model.SigortaSirketiKodu);
                    if (kayitVarmi != null)
                    {
                        mesaj = model.PoliceNo + " nolu poliçe daha önceden eklenmiştir.";
                        return Json(new { Success = "False", sum = mesaj });
                    }
                    else
                    {
                        policeTali.PoliceTransferEslestimi = 0;
                        policeTali.AdUnvan_ = model.Ad;
                        policeTali.EkNo = Convert.ToInt32(model.EkNo);
                        policeTali.SoyadUnvan = model.Soyad;
                        policeTali.PoliceNo = model.PoliceNo;
                        policeTali.SigortaSirketNo_ = model.SigortaSirketiKodu;
                        policeTali.KayitTarihi_ = kayitTarihi;
                        policeTali.GuncellemeTarihi = kayitTarihi;
                        policeTali.KimlikNo = model.tcVkn;
                        response = _TaliPoliceService.CreatePoliceTaliAcenteler(policeTali);
                        mesaj = model.PoliceNo + " nolu poliçe eklendi";

                        policeTaliRapor = _TaliPoliceService.GetPoliceTaliAcenteRapor(policeTali.TVMKodu.Value, kayitTarihi);
                        if (policeTaliRapor != null)
                        {
                            policeTaliRapor.UretimVAR_YOK = 1;
                            policeTaliRapor.GuncellemeTarihi = TurkeyDateTime.Today;
                            policeTaliRapor.Police_EkAdedi = policeTaliRapor.Police_EkAdedi + 1;
                            responseRaporGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteRapor(policeTaliRapor);
                        }
                        else
                        {
                            policeTaliRapor = new PoliceTaliAcenteRapor();
                            policeTaliRapor.TVMKodu = policeTali.TVMKodu;
                            policeTaliRapor.UretimVAR_YOK = 1;
                            policeTaliRapor.KayitTarihi = kayitTarihi;
                            policeTaliRapor.GuncellemeTarihi = kayitTarihi;
                            policeTaliRapor.Police_EkAdedi = 1;
                            responseRapor = _TaliPoliceService.CreatePoliceTaliAcenteRapor(policeTaliRapor);
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region Alt acentesi olmayan merkez acente veya işlem yapan alt acente ise
            else if (model != null)
            {
                //Güncelleme
                if (model.KayitId > 0)
                {
                    policeTali = _TaliPoliceService.GetPoliceTaliAcente(model.KayitId);
                    policeTali.AdUnvan_ = model.Ad;
                    policeTali.EkNo = Convert.ToInt32(model.EkNo);
                    policeTali.SoyadUnvan = model.Soyad;
                    policeTali.PoliceNo = model.PoliceNo;
                    policeTali.SigortaSirketNo_ = model.SigortaSirketiKodu;
                    policeTali.KayitTarihi_ = Convert.ToDateTime(model.KayitTarihi);
                    policeTali.KimlikNo = model.tcVkn;
                    policeTali.Id = model.KayitId;
                    policeTali.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                    responseGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteler(policeTali);
                    mesaj = "Poliçe Güncellendi";

                    policeTaliRapor = _TaliPoliceService.GetPoliceTaliAcenteRapor(_TVMKodu, kayitTarihi);
                    if (policeTaliRapor != null)
                    {
                        policeTaliRapor.UretimVAR_YOK = 1;
                        policeTaliRapor.GuncellemeTarihi = TurkeyDateTime.Today;
                        policeTaliRapor.TVMKodu = _TVMKodu;
                        responseRaporGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteRapor(policeTaliRapor);
                    }
                }
                //yeni kayıt ekleme
                else
                {
                    var kayitVarmi = _TaliPoliceService.TaliPoliceVarmi(model.PoliceNo, Convert.ToInt32(model.EkNo), model.SigortaSirketiKodu);
                    if (kayitVarmi != null)
                    {
                        mesaj = model.PoliceNo + " nolu poliçe daha önceden eklenmiştir.";
                        return Json(new { Success = "False", sum = mesaj });
                    }
                    else
                    {
                        policeTali.PoliceTransferEslestimi = 0;
                        policeTali.AdUnvan_ = model.Ad;
                        policeTali.EkNo = Convert.ToInt32(model.EkNo);
                        policeTali.SoyadUnvan = model.Soyad;
                        policeTali.PoliceNo = model.PoliceNo;
                        policeTali.SigortaSirketNo_ = model.SigortaSirketiKodu;
                        policeTali.KayitTarihi_ = kayitTarihi;
                        policeTali.GuncellemeTarihi = kayitTarihi;
                        policeTali.KimlikNo = model.tcVkn;
                        response = _TaliPoliceService.CreatePoliceTaliAcenteler(policeTali);
                        mesaj = "Poliçe Eklendi";

                        policeTaliRapor = _TaliPoliceService.GetPoliceTaliAcenteRapor(_TVMKodu, kayitTarihi);
                        if (policeTaliRapor != null)
                        {
                            policeTaliRapor.UretimVAR_YOK = 1;
                            policeTaliRapor.GuncellemeTarihi = TurkeyDateTime.Today;
                            policeTaliRapor.Police_EkAdedi = policeTaliRapor.Police_EkAdedi + 1;
                            policeTaliRapor.TVMKodu = _TVMKodu;
                            responseRaporGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteRapor(policeTaliRapor);
                        }
                        else
                        {
                            policeTaliRapor = new PoliceTaliAcenteRapor();
                            policeTaliRapor.TVMKodu = _TVMKodu;
                            policeTaliRapor.UretimVAR_YOK = 1;
                            policeTaliRapor.KayitTarihi = kayitTarihi;
                            policeTaliRapor.GuncellemeTarihi = kayitTarihi;
                            policeTaliRapor.Police_EkAdedi = 1;

                            responseRapor = _TaliPoliceService.CreatePoliceTaliAcenteRapor(policeTaliRapor);
                        }
                    }
                }
            }
            #endregion

            return Json(new { Success = "True", sum = mesaj });
        }

        [HttpGet]
        public ActionResult TaliPoliceDurum(DateTime bordroTarihi)
        {

            PoliceTaliAcenteRapor taliR = new PoliceTaliAcenteRapor();

            taliR = _TaliPoliceService.GetPoliceTaliAcenteRapor(_AktifKullaniciService.TVMKodu, bordroTarihi);

            if (taliR != null)
            {
                if (taliR.UretimVAR_YOK == 1)
                {
                    return Json(new { durum = 1 }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { durum = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { durum = 2 }, JsonRequestBehavior.AllowGet);
            }



        }

        [HttpPost]
        [AjaxException]
        public ActionResult TaliPoliceGuncelle(int taliPoliceId, int TVMKodu)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            TaliPoliceListe tali;

            PoliceTaliAcenteler talipolice = new PoliceTaliAcenteler();
            talipolice = _TaliPoliceService.GetPoliceTaliAcente(taliPoliceId);
            TaliAcenteModel model = new TaliAcenteModel();
            model.Ad = talipolice.AdUnvan_;
            model.EkNo = talipolice.EkNo.ToString();
            model.PoliceNo = talipolice.PoliceNo;
            model.Soyad = talipolice.SoyadUnvan;
            model.tcVkn = talipolice.KimlikNo;
            model.KayitId = talipolice.Id;
            model.KayitTarihi = talipolice.KayitTarihi_.ToString();
            model.PoliceTVMKodu = talipolice.TVMKodu.Value;
            model.SigortaSirketiKodu = talipolice.SigortaSirketNo_;
            model.PoliceTVMUnvani = _TVMService.GetDetay(talipolice.TVMKodu.Value).Unvani;

            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();
            model.taliPoliceListe = new List<TaliPoliceListe>();
            var taliPoliceler = _TaliPoliceService.GetPoliceTaliAcenteIslemTarih(_AktifKullaniciService.TVMKodu, talipolice.KayitTarihi_.Value);
            foreach (var item in taliPoliceler)
            {
                tali = new TaliPoliceListe();
                tali.AdUnvan_ = item.AdUnvan_;
                tali.EkNo = item.EkNo;
                tali.GuncellemeTarihi = item.GuncellemeTarihi.HasValue ? item.GuncellemeTarihi.Value.ToString("dd/MM/yyyy") : "";
                tali.Id = item.Id;
                tali.KayitTarihi_ = item.KayitTarihi_.Value.ToString("dd/MM/yyyy");
                tali.KimlikNo = item.KimlikNo;
                tali.PoliceNo = item.PoliceNo;
                tali.SigortaSirketAdi = _SigortaSirketleriService.GetSirket(item.SigortaSirketNo_).SirketAdi;
                tali.SoyadUnvan = item.SoyadUnvan;
                tali.TVMKodu = item.TVMKodu;
                model.taliPoliceListe.Add(tali);
            }

            return Json(model);
        }

        [HttpPost]
        public ActionResult TaliPoliceSil(int id)
        {
            var result = _TaliPoliceService.DeletePoliceTaliAcenteler(id);
            return Json(new { success = result });
        }

        [HttpPost]
        public ActionResult GunKapa()
        {
            bool responseGunKapaRaporGuncelle;
            bool responseGunKapaRaporCreate;
            PoliceTaliAcenteRapor taliRaporGunKapa = new PoliceTaliAcenteRapor();

            taliRaporGunKapa = _TaliPoliceService.GetPoliceTaliAcenteRapor(_AktifKullaniciService.TVMKodu);
            if (taliRaporGunKapa != null)
            {
                taliRaporGunKapa.GuncellemeTarihi = TurkeyDateTime.Today;
                responseGunKapaRaporGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteRapor(taliRaporGunKapa);

                return Json(new { success = responseGunKapaRaporGuncelle });
            }
            else
            {
                taliRaporGunKapa = new PoliceTaliAcenteRapor();
                taliRaporGunKapa.TVMKodu = _AktifKullaniciService.TVMKodu;
                taliRaporGunKapa.UretimVAR_YOK = 0;
                taliRaporGunKapa.KayitTarihi = TurkeyDateTime.Today.Date;
                taliRaporGunKapa.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                taliRaporGunKapa.Police_EkAdedi = 0;
                responseGunKapaRaporCreate = _TaliPoliceService.CreatePoliceTaliAcenteRapor(taliRaporGunKapa);

                return Json(new { success = responseGunKapaRaporCreate });
            }

        }

        [HttpPost]
        public ActionResult GunKapamaDurumum(DateTime bordroTarihi)
        {
            string Message = String.Empty;
            try
            {
                //PoliceTaliAcenteler model = new PoliceTaliAcenteler();
                bool responseGünKapamaDurumumGuncelle;
                bool responseGünKapamaDurumumCreate;
                PoliceTaliAcenteRapor taliGünKapamaDurumum = new PoliceTaliAcenteRapor();
                taliGünKapamaDurumum = _TaliPoliceService.GetPoliceTaliAcenteRapor(_AktifKullaniciService.TVMKodu, bordroTarihi);


                if (taliGünKapamaDurumum != null)
                {
                    taliGünKapamaDurumum.GuncellemeTarihi = TurkeyDateTime.Today.Date;
                    taliGünKapamaDurumum.GunKapamaDurumu = 1;
                    responseGünKapamaDurumumGuncelle = _TaliPoliceService.UpdatePoliceTaliAcenteRapor(taliGünKapamaDurumum);
                }
                else
                {
                    taliGünKapamaDurumum = new PoliceTaliAcenteRapor();
                    taliGünKapamaDurumum.TVMKodu = _AktifKullaniciService.TVMKodu;
                    taliGünKapamaDurumum.GunKapamaDurumu = 1;
                    taliGünKapamaDurumum.KayitTarihi = bordroTarihi;
                    taliGünKapamaDurumum.GuncellemeTarihi = bordroTarihi;
                    responseGünKapamaDurumumCreate = _TaliPoliceService.CreatePoliceTaliAcenteRapor(taliGünKapamaDurumum);
                }
                Message = "Gün kapatma işleminiz başarıyla gerçekleşmiştir.";

                return Json(new { Success = "True", Message = Message });
            }
            catch (Exception ex)
            {
                return Json(new { Success = "False", Message = ex.Message });
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.UretimKontrol, SekmeKodu = 0)]
        public ActionResult TaliAcenteRaporEkrani()
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            TaliAcenteModel model = new TaliAcenteModel();
            //model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            List<SelectListItem> raporTip = new List<SelectListItem>();
            raporTip.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Günlük Liste" },
                new SelectListItem() { Value = "1", Text ="İki Tarih Aralığı"}
            });
            model.RaporTipleri = new SelectList(raporTip, "Value", "Text", "0");


            var taliPolicelerRaporEkran = _TaliPoliceService.GetPoliceTaliAcenteRaporTarih(Convert.ToInt32(_AktifKullaniciService.TVMKodu), Convert.ToDateTime(TurkeyDateTime.Today));
            model.taliAcenteRaporEkranListe = new List<TaliAcenteRaporEkran>();

            //List<TVMDetay> yetkiliTVMler = _TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu);
            List<TVMOzetModel> yetkiliTVMler = _TVMService.GetTVMListeKullaniciYetki(0);
            int sayacBelirsizler = 0;
            foreach (var itemTVM in yetkiliTVMler)
            {
                TaliAcenteRaporEkran taliEkranYeni;
                int sayac = 0;
                foreach (var itemTaliPolice in taliPolicelerRaporEkran)
                {

                    if (itemTVM.Kodu == itemTaliPolice.TVMKodu)
                    {
                        taliEkranYeni = new TaliAcenteRaporEkran();
                        taliEkranYeni.TVMKodu = itemTaliPolice.TVMKodu;
                        taliEkranYeni.TVMAdi = itemTVM.Unvani;
                        taliEkranYeni.UretimVAR_YOK = itemTaliPolice.UretimVAR_YOK;
                        taliEkranYeni.Police_EkAdedi = itemTaliPolice.Police_EkAdedi;
                        taliEkranYeni.GuncellemeTarihi = itemTaliPolice.GuncellemeTarihi;
                        taliEkranYeni.KayitTarihi = itemTaliPolice.KayitTarihi;
                        taliEkranYeni.GunKapandimi = itemTaliPolice.GunKapamaDurumu;
                        model.taliAcenteRaporEkranListe.Add(taliEkranYeni);
                        sayac++;
                        break;
                    }
                    else if (taliPolicelerRaporEkran.Count() == sayac + 1)
                    {
                        taliEkranYeni = new TaliAcenteRaporEkran();
                        taliEkranYeni.TVMKodu = itemTVM.Kodu;
                        taliEkranYeni.TVMAdi = itemTVM.Unvani;
                        taliEkranYeni.UretimVAR_YOK = 2;
                        taliEkranYeni.Police_EkAdedi = 0;
                        taliEkranYeni.GunKapandimi = 0;
                        //taliEkranYeni.GuncellemeTarihi = TurkeyDateTime.Today;
                        //taliEkranYeni.KayitTarihi = TurkeyDateTime.Today;
                        model.taliAcenteRaporEkranListe.Add(taliEkranYeni);
                        sayacBelirsizler++;
                        break;
                    }
                    sayac++;
                }
            }

            model.BaslangicTarihi = TurkeyDateTime.Today;

            ViewBag.durumMesaj = babonline.Total + " " + yetkiliTVMler.Count() + " " + babonline.Agency + " " + sayacBelirsizler + " " + babonline.Unclear + "!!!";
            return View(model);

        }

        // Günlük Rapor listeleme tarih kısmı için
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.UretimKontrol, SekmeKodu = 0)]
        public ActionResult TaliAcenteRaporEkrani(TaliAcenteModel aramaModel)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            TaliAcenteModel model = new TaliAcenteModel();
            List<PoliceTaliAcenteRapor> taliPolicelerRaporListe = new List<PoliceTaliAcenteRapor>();
            if (aramaModel.RaporTipi == 0)
            {
                taliPolicelerRaporListe = _TaliPoliceService.GetPoliceTaliAcenteRaporTarih(Convert.ToInt32(_AktifKullaniciService.TVMKodu), Convert.ToDateTime(aramaModel.BaslangicTarihi));

                if (ModelState["tvmList"] != null)
                    ModelState["tvmList"].Errors.Clear();

                if (ModelState["KayitBaslangicTarihi"] != null)
                    ModelState["KayitBaslangicTarihi"].Errors.Clear();

                if (ModelState["KayitBitisTarihi"] != null)
                    ModelState["KayitBitisTarihi"].Errors.Clear();
            }
            else
            {
                List<string> liste = new List<string>();
                foreach (var item in aramaModel.tvmList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                taliPolicelerRaporListe = _TaliPoliceService.GetPoliceTaliAcenteRaporTarihAraligi(liste.ToArray(), aramaModel.KayitBaslangicTarihi, aramaModel.KayitBitisTarihi);
            }

            //dil ing olunca baslangıc tarihi null geliyor 
            if (aramaModel.BaslangicTarihi == null)
            {
                taliPolicelerRaporListe = _TaliPoliceService.GetPoliceTaliAcenteRaporStringTarih(Convert.ToInt32(_AktifKullaniciService.TVMKodu), aramaModel.tarih);
            }
            model.taliAcenteRaporEkranListe = new List<TaliAcenteRaporEkran>();

            //List<TVMDetay> yetkiliTVMler = _TaliPoliceService.GetYetkiliTVM(Convert.ToInt32(_AktifKullaniciService.TVMKodu));
            List<TVMOzetModel> yetkiliTVMler = _TVMService.GetTVMListeKullaniciYetki(0);
            int sayacBelirsizler = 0;
            foreach (var itemTVM in yetkiliTVMler)
            {
                TaliAcenteRaporEkran taliEkranYeni;
                int sayac = 0;
                foreach (var itemTaliPolice in taliPolicelerRaporListe)
                {
                    if (itemTVM.Kodu == itemTaliPolice.TVMKodu)
                    {
                        taliEkranYeni = new TaliAcenteRaporEkran();
                        taliEkranYeni.TVMKodu = itemTaliPolice.TVMKodu;
                        taliEkranYeni.TVMAdi = itemTVM.Unvani;
                        taliEkranYeni.UretimVAR_YOK = itemTaliPolice.UretimVAR_YOK;
                        taliEkranYeni.Police_EkAdedi = itemTaliPolice.Police_EkAdedi;
                        taliEkranYeni.GuncellemeTarihi = itemTaliPolice.GuncellemeTarihi;
                        taliEkranYeni.KayitTarihi = itemTaliPolice.KayitTarihi;
                        taliEkranYeni.GunKapandimi = itemTaliPolice.GunKapamaDurumu;
                        model.taliAcenteRaporEkranListe.Add(taliEkranYeni);
                        sayac++;
                        break;
                    }
                    else if (taliPolicelerRaporListe.Count() == sayac + 1)
                    {
                        taliEkranYeni = new TaliAcenteRaporEkran();
                        taliEkranYeni.TVMKodu = itemTVM.Kodu;
                        taliEkranYeni.TVMAdi = itemTVM.Unvani;
                        taliEkranYeni.UretimVAR_YOK = 2;
                        taliEkranYeni.Police_EkAdedi = 0;
                        taliEkranYeni.GunKapandimi = 0;
                        model.taliAcenteRaporEkranListe.Add(taliEkranYeni);
                        sayacBelirsizler++;
                        break;
                    }
                    sayac++;
                }
            }

            model.BaslangicTarihi = aramaModel.BaslangicTarihi;

            // ViewBag.durumMesaj = "Toplam " + yetkiliTVMler.Count() + " acenteden " + sayacBelirsizler + " tanesinin durumu belirsiz !!!";
            ViewBag.durumMesaj = babonline.Total + " " + yetkiliTVMler.Count() + " " + babonline.Agency + " " + sayacBelirsizler + " " + babonline.Unclear + "!!!";

            model.KayitBaslangicTarihi = aramaModel.KayitBaslangicTarihi;
            model.KayitBitisTarihi = aramaModel.KayitBitisTarihi;
            model.tvmList = aramaModel.tvmList;
            // model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.tvmList);
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani", model.tvmList);

            List<SelectListItem> raporTip = new List<SelectListItem>();
            raporTip.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Günlük Liste" },
                new SelectListItem() { Value = "1", Text ="İki Tarih Aralığı"}
            });
            model.RaporTipleri = new SelectList(raporTip, "Value", "Text", aramaModel.RaporTipi);

            model.RaporTipi = aramaModel.RaporTipi;
            return View(model);
        }
        //  Rapor listeleme tarih kısmı için

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.UretimListesi, SekmeKodu = 0)]
        public ActionResult TaliAcenteIslem(TaliPoliceListe aramaModel)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();

            TaliAcenteModel model = new TaliAcenteModel();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            TaliPoliceListe tali;

            var taliPolicelerAcenteIslemTariheGore = _TaliPoliceService.GetPoliceTaliAcenteIslemTarih(Convert.ToInt32(_AktifKullaniciService.TVMKodu), Convert.ToDateTime(aramaModel.BaslangicTarihi));

            if (aramaModel.BaslangicTarihi == null)
            {
                //string str = aramaModel.tarih;
                //string[] dateString = str.Split('/');
                //DateTime enter_date = Convert.ToDateTime(dateString[0] + "." + dateString[1] + "." + dateString[2]);

                taliPolicelerAcenteIslemTariheGore = _TaliPoliceService.GetPoliceTaliAcenteIslemStringTarih(Convert.ToInt32(_AktifKullaniciService.TVMKodu), aramaModel.tarih);
            }

            var taliPoliceler = _TaliPoliceService.GetPoliceTaliAcenteGunlukListe(_AktifKullaniciService.TVMKodu);

            model.taliPoliceListe = new List<TaliPoliceListe>();
            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();

            foreach (var item in taliPolicelerAcenteIslemTariheGore)
            {
                tali = new TaliPoliceListe();
                tali.AdUnvan_ = item.AdUnvan_;
                tali.EkNo = item.EkNo;
                tali.GuncellemeTarihi = item.GuncellemeTarihi.HasValue ? item.GuncellemeTarihi.Value.ToString("dd/MM/yyyy") : "";
                tali.Id = item.Id;
                tali.KayitTarihi_ = item.KayitTarihi_.Value.ToString("dd/MM/yyyy");
                tali.KimlikNo = item.KimlikNo;
                tali.PoliceNo = item.PoliceNo;
                tali.SigortaSirketAdi = _SigortaSirketleriService.GetSirket(item.SigortaSirketNo_).SirketAdi;
                tali.SoyadUnvan = item.SoyadUnvan;
                tali.TVMKodu = item.TVMKodu;
                tali.TVMUnvan = item.TVMKodu.HasValue ? _TVMService.GetDetay(item.TVMKodu.Value).Unvani : "";

                model.taliPoliceListe.Add(tali);
            }

            model.BaslangicTarihi = aramaModel.BaslangicTarihi;
            model.BitisTarihi = aramaModel.BitisTarihi;
            model.AnaTVMmi = _TVMService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            model.PoliceTVMKodu = _AktifKullaniciService.TVMKodu;
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.PoliceEslesmeyenListesi, SekmeKodu = 0)]
        public ActionResult PoliceTransferEslemeListesi()
        {
            PoliceTrasferTaliAcenteKoduEslemeModel model = new PoliceTrasferTaliAcenteKoduEslemeModel();
            // model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            model.SigortaSirketleri = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");

            List<SelectListItem> raporTip = new List<SelectListItem>();
            raporTip.AddRange(new SelectListItem[] {
                new SelectListItem() { Value = "0", Text = "Transfer ile Eşleşmeyen Bordro Kayıtları" },
                new SelectListItem() { Value = "1", Text ="Transferde Açıkta Kalan Poliçeler"}
            });
            model.RaporTipleri = new SelectList(raporTip, "Value", "Text", "0");

            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.PoliceEslesmeyenListesi, SekmeKodu = 0)]
        public ActionResult PoliceTransferEslemeListesi(PoliceTrasferTaliAcenteKoduEslemeModel model)
        {
            List<PoliceTaliAcenteler> eslesmeyenPoliceBordroListesi = new List<PoliceTaliAcenteler>();
            List<PoliceGenel> eslesmeyenPoliceGenelListesi = new List<PoliceGenel>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            try
            {
                if (model != null)
                {
                    List<string> TVMListe = new List<string>();
                    foreach (var item in model.tvmList)
                    {
                        if (item != "multiselect-all")
                        {
                            TVMListe.Add(item);
                        }
                    }

                    List<string> SirketListe = new List<string>();
                    if (model.SigortaSirketleriSelectList != null)
                    {

                        foreach (var item in model.SigortaSirketleriSelectList)
                        {
                            if (item != "multiselect-all")
                            {
                                SirketListe.Add(item);
                            }
                        }
                    }
                    eslesmeyenPoliceBordroListesi = _TaliPoliceService.GetPoliceTaliAcenteler(TVMListe.ToArray(), SirketListe.ToArray(), model.KayitBaslangicTarihi, model.KayitBitisTarihi).ToList();
                    eslesmeyenPoliceGenelListesi = _TaliPoliceService.GetPoliceGenelEslesmeyen(_AktifKullaniciService.TVMKodu, model.KayitBaslangicTarihi, model.KayitBitisTarihi).ToList();
                    if (model.RaporTipi == 0)
                    {
                        // var tvmler = _TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu);
                        var tvmler = _TVMService.GetTVMListeKullaniciYetki(0);

                        model.PoliceListe = new List<PoliceTaliAcentelerModel>();
                        PoliceTaliAcentelerModel police = new PoliceTaliAcentelerModel();
                        if (eslesmeyenPoliceBordroListesi != null)
                        {
                            foreach (var item in eslesmeyenPoliceBordroListesi)
                            {
                                police = new PoliceTaliAcentelerModel();
                                police.TVMKodu = item.TVMKodu;
                                police.TVMUnvan = "";
                                if (tvmler != null)
                                {
                                    var tvmUnvan = tvmler.Where(s => s.Kodu == item.TVMKodu).FirstOrDefault();
                                    police.TVMUnvan = tvmUnvan.Unvani;
                                }
                                police.KimlikNo = item.KimlikNo;
                                police.AdUnvan_ = item.AdUnvan_;
                                police.SoyadUnvan = item.SoyadUnvan;
                                police.PoliceNo = item.PoliceNo;
                                police.EkNo = item.EkNo;
                                police.KayitTarihi = item.KayitTarihi_;
                                police.GuncellemeTarihi = item.GuncellemeTarihi;
                                police.SigortaSirketNo = item.SigortaSirketNo_;

                                if (sigortaSirketleri != null)
                                {
                                    var sirketUnvan = sigortaSirketleri.Where(s => s.SirketKodu == item.SigortaSirketNo_).FirstOrDefault();
                                    police.SigortaSirketAdi = sirketUnvan.SirketAdi;
                                }
                                else
                                {
                                    police.SigortaSirketAdi = "";
                                }
                                model.PoliceListe.Add(police);
                            }
                            var list = (from m in model.PoliceListe
                                        select m.TVMKodu).Distinct();
                            PoliceTaliAcentelerTVMModel listItem = new PoliceTaliAcentelerTVMModel();
                            model.taliPoliceGrupListe = new List<PoliceTaliAcentelerTVMModel>();
                            foreach (var item in list)
                            {
                                listItem = new PoliceTaliAcentelerTVMModel();
                                listItem.TVMKodu = item.Value;
                                model.taliPoliceGrupListe.Add(listItem);
                            }
                        }
                    }
                    else
                    {
                        if (eslesmeyenPoliceGenelListesi != null)
                        {
                            //var tvmler = _TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu);
                            var tvmler = _TVMService.GetTVMListeKullaniciYetki(0);
                            PoliceGenelModel police = new PoliceGenelModel();

                            model.PoliceGenelListe = new List<PoliceGenelModel>();
                            foreach (var item in eslesmeyenPoliceGenelListesi)
                            {
                                var policeBordroListesindeVarMi = eslesmeyenPoliceBordroListesi.Find(s => s.PoliceNo == item.PoliceNumarasi && s.EkNo == item.EkNo && s.SigortaSirketNo_ == item.TUMBirlikKodu);
                                if (policeBordroListesindeVarMi == null)
                                {
                                    police = new PoliceGenelModel();
                                    police.TVMKodu = item.TVMKodu;
                                    police.TVMUnvan = "";
                                    if (tvmler != null)
                                    {
                                        var tvmUnvan = tvmler.Where(s => s.Kodu == item.TVMKodu).FirstOrDefault();
                                        police.TVMUnvan = tvmUnvan.Unvani;
                                    }
                                    police.SigortaliKimlikNo = !String.IsNullOrEmpty(item.PoliceSigortali.KimlikNo) ? item.PoliceSigortali.KimlikNo : !String.IsNullOrEmpty(item.PoliceSigortali.VergiKimlikNo) ? item.PoliceSigortali.VergiKimlikNo : "";
                                    police.SigortaliAdiSoyAdi = item.PoliceSigortali.AdiUnvan + " " + item.PoliceSigortali.SoyadiUnvan;
                                    police.SigortaEttirenKimlikNo = !String.IsNullOrEmpty(item.PoliceSigortaEttiren.KimlikNo) ? item.PoliceSigortaEttiren.KimlikNo : !String.IsNullOrEmpty(item.PoliceSigortaEttiren.VergiKimlikNo) ? item.PoliceSigortaEttiren.VergiKimlikNo : "";
                                    police.SigortaEttirenAdiSoyAdi = item.PoliceSigortaEttiren.AdiUnvan + " " + item.PoliceSigortaEttiren.SoyadiUnvan;
                                    police.PoliceNo = item.PoliceNumarasi;
                                    police.EkNo = item.EkNo;
                                    police.YenilemeNo = item.YenilemeNo;
                                    police.TanzimTarihi = item.TanzimTarihi;
                                    police.BaslangicTarihi = item.BaslangicTarihi;
                                    police.BitisTarihi = item.BitisTarihi;
                                    police.TumBirlikAciklama = "";
                                    if (sigortaSirketleri != null)
                                    {
                                        var sirketUnvan = sigortaSirketleri.Where(s => s.SirketKodu == item.TUMBirlikKodu).FirstOrDefault();
                                        police.TumBirlikAciklama = sirketUnvan.SirketAdi;
                                    }
                                    model.PoliceGenelListe.Add(police);
                                }
                            }
                        }
                    }
                    //model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");
                    model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
                    model.SigortaSirketleri = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");

                    List<SelectListItem> raporTip = new List<SelectListItem>();
                    raporTip.AddRange(new SelectListItem[] {
                       new SelectListItem() { Value = "0", Text = "Transfer ile Eşleşmeyen Bordro Kayıtları" },
                       new SelectListItem() { Value = "1", Text ="Transferde Açıkta Kalan Poliçeler"}
                                        });
                    model.RaporTipleri = new SelectList(raporTip, "Value", "Text", model.RaporTipi);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        public ActionResult PaylasimliPoliceUretimGirisi()
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            PaylasimliPoliceUretimModel model = new PaylasimliPoliceUretimModel();
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvan = _AktifKullaniciService.TVMUnvani;
            model.TaliTVMList = new SelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel();
            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();
            model.Branslar = new SelectList(brans, "BransKodu", "BransAdi").ListWithOptionLabel();

            return View(model);
        }

        public ActionResult PaylasimliPoliceUretimGirisiEkle(PaylasimliPoliceUretimModel model)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            model.TVMKodu = _AktifKullaniciService.TVMKodu;

            PaylasimliPoliceUretim police = new PaylasimliPoliceUretim();
            List<TaliAcenteKomisyonOrani> KomisyonOranlari;
            TaliAcenteKomisyonOrani merkezAcenteKomisyonOrani;

            var kayitVarMi = _TaliPoliceService.PaylasimliPoliceUretimVarMi(model.TVMKodu, model.TaliTVMKodu, model.PoliceNo, model.YenilemeNo, model.ZeylNo, model.SigortaSirketiKodu);
            if (kayitVarMi == null)
            {
                police.PoliceNo = model.PoliceNo;
                police.YenilemeNo = model.YenilemeNo;
                police.ZeylNo = model.ZeylNo;
                police.NetPrim = model.NetPrim;
                police.BrutPrim = model.BrutPrim;
                police.BransKodu = model.BransKodu;
                police.SigortaSirketNo = model.SigortaSirketiKodu;
                police.TVMKodu = model.TVMKodu;
                police.TaliTVMKodu = model.TaliTVMKodu == null ? model.TVMKodu : model.TaliTVMKodu;
                police.TanzimTarihi = model.TanzimTarihi;
                police.KaydiEkleyenKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                police.KayitTarihi = model.KayitTarihi;
                police.GuncellemeTarihi = model.GuncellemeTarihi;

                KomisyonOranlari = new List<TaliAcenteKomisyonOrani>();
                KomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(_AktifKullaniciService.TVMKodu, model.TaliTVMKodu, model.DisKaynakKodu, model.SigortaSirketiKodu, Convert.ToInt32(model.BransKodu));
                if (KomisyonOranlari != null)
                {
                    merkezAcenteKomisyonOrani = new TaliAcenteKomisyonOrani();
                    //kademeli çalışmıyorsa
                    if (KomisyonOranlari.Count > 0)
                    {
                        if (KomisyonOranlari[0].MinUretim == null && KomisyonOranlari[0].MaxUretim == null)
                        {
                            merkezAcenteKomisyonOrani = KomisyonOranlari[0];
                        }
                    }

                    police.PoliceKomisyonTutari = (model.NetPrim * merkezAcenteKomisyonOrani.KomisyonOran.Value) / 100;
                }
            }

            model.TaliTVMList = new SelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel();
            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();
            model.Branslar = new SelectList(brans, "BransKodu", "BransAdi").ListWithOptionLabel();

            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.UretimBordrosuGoruntuleme, SekmeKodu = 0)]
        public ActionResult UretimBordrosuGoruntule()
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            TaliPoliceListe tali;
            List<PoliceTaliAcenteler> taliPoliceler = new List<PoliceTaliAcenteler>();
            PoliceBordroModel model = new PoliceBordroModel();
            model.PoliceTVMKodu = _AktifKullaniciService.TVMKodu;
            model.PoliceTVMUnvani = _AktifKullaniciService.TVMUnvani;
            // model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.SigortaSirketleri = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");
            model.KayitBaslangicTarihi = TurkeyDateTime.Today.AddDays(-1);
            model.KayitBitisTarihi = TurkeyDateTime.Today;

            taliPoliceler = _TaliPoliceService.GetPoliceTaliAcenteIslemTarih(_AktifKullaniciService.TVMKodu, model.KayitBaslangicTarihi);
            model.taliPoliceListe = new List<TaliPoliceListe>();

            foreach (var item in taliPoliceler)
            {
                tali = new TaliPoliceListe();
                tali.AdUnvan_ = item.AdUnvan_;
                tali.EkNo = item.EkNo;
                tali.GuncellemeTarihi = item.GuncellemeTarihi.HasValue ? item.GuncellemeTarihi.Value.ToString("dd/MM/yyyy") : "";
                tali.Id = item.Id;
                tali.KayitTarihi_ = item.KayitTarihi_.Value.ToString("dd/MM/yyyy");
                tali.KimlikNo = item.KimlikNo;
                tali.PoliceNo = item.PoliceNo;
                tali.SigortaSirketAdi = _SigortaSirketleriService.GetSirket(item.SigortaSirketNo_).SirketAdi;
                tali.SoyadUnvan = item.SoyadUnvan;
                tali.TVMKodu = item.TVMKodu;
                tali.TVMUnvan = item.TVMKodu.HasValue ? _TVMService.GetDetay(item.TVMKodu.Value).Unvani : "";

                model.taliPoliceListe.Add(tali);
            }
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.UretimBordrosuGoruntuleme, SekmeKodu = 0)]
        public ActionResult UretimBordrosuGoruntule(PoliceBordroModel model)
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            TaliPoliceListe tali;
            List<PoliceTaliAcenteler> taliPoliceler = new List<PoliceTaliAcenteler>();

            // model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.tvmler = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.SigortaSirketleri = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");

            if (model != null)
            {
                List<string> TVMListe = new List<string>();
                foreach (var item in model.tvmList)
                {
                    if (item != "multiselect-all")
                    {
                        TVMListe.Add(item);
                    }
                }

                List<string> SirketListe = new List<string>();
                if (model.SigortaSirketleriSelectList != null)
                {

                    foreach (var item in model.SigortaSirketleriSelectList)
                    {
                        if (item != "multiselect-all")
                        {
                            SirketListe.Add(item);
                        }
                    }
                }
                taliPoliceler = _TaliPoliceService.GetPoliceBordroList(TVMListe.ToArray(), SirketListe.ToArray(), model.KayitBaslangicTarihi, model.KayitBitisTarihi);
                model.taliPoliceListe = new List<TaliPoliceListe>();
            }

            foreach (var item in taliPoliceler)
            {
                tali = new TaliPoliceListe();
                tali.AdUnvan_ = item.AdUnvan_;
                tali.EkNo = item.EkNo;
                tali.GuncellemeTarihi = item.GuncellemeTarihi.HasValue ? item.GuncellemeTarihi.Value.ToString("dd/MM/yyyy") : "";
                tali.Id = item.Id;
                tali.KayitTarihi_ = item.KayitTarihi_.Value.ToString("dd/MM/yyyy");
                tali.KimlikNo = item.KimlikNo;
                tali.PoliceNo = item.PoliceNo;
                tali.SigortaSirketAdi = _SigortaSirketleriService.GetSirket(item.SigortaSirketNo_).SirketAdi;
                tali.SoyadUnvan = item.SoyadUnvan;
                tali.TVMKodu = item.TVMKodu;
                tali.TVMUnvan = item.TVMKodu.HasValue ? _TVMService.GetDetay(item.TVMKodu.Value).Unvani : "";

                model.taliPoliceListe.Add(tali);
            }
            var list = (from m in taliPoliceler
                        select m.TVMKodu).Distinct();
            PoliceTaliAcentelerTVMModel listItem = new PoliceTaliAcentelerTVMModel();
            model.taliPoliceGrupListe = new List<PoliceTaliAcentelerTVMModel>();
            foreach (var item in list)
            {
                listItem = new PoliceTaliAcentelerTVMModel();
                listItem.TVMKodu = item.Value;
                model.taliPoliceGrupListe.Add(listItem);
            }
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.PoliceOnaylama, SekmeKodu = 0)]
        public ActionResult PoliceOnaylama()
        {
            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            PoliceOnaylamaModel model = new PoliceOnaylamaModel();

            model.MerkezAcentemi = false;
            model.MerkezAcenKodu = 0;
            TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                model.MerkezAcentemi = true;
                model.MerkezAcenKodu = tvmDetay.Kodu;
            }
            // model.Tvmler = new SelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel();
            model.Tvmler = new SelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel();
            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi").ListWithOptionLabel();

            // model.tvmMultiList = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu), "Kodu", "Unvani");
            model.tvmMultiList = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.sirketMultiList = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");

            model.Branslar = new SelectList(brans, "BransKodu", "BransAdi").ListWithOptionLabel();
            model.IslemTipi = 0;
            model.Islemler = new SelectList(Islemler.PoliceOnaylamaIslemTipleri(), "Value", "Text");
            model.DisKaynakList = this.PoliceTransferTaliAcenteler;
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.PoliceOnaylama, SekmeKodu = 0)]
        public ActionResult PoliceOnaylama(PoliceOnaylamaModel model)
        {
            #region Zorunluluğu Kontrol Edilmeyecek Alanlar

            if (!model.TVMKodu.HasValue)
            {
                if (ModelState["tvmKodu"] != null)
                    ModelState["tvmKodu"].Errors.Clear();
            }
            if (model.IslemTipi == 0)
            {
                if (ModelState["tvmMultiLists"] != null)
                    ModelState["tvmMultiLists"].Errors.Clear();

                if (ModelState["sirketMultiLists"] != null)
                    ModelState["sirketMultiLists"].Errors.Clear();

                if (ModelState["BaslangicTarihi"] != null)
                    ModelState["BaslangicTarihi"].Errors.Clear();

                if (ModelState["BitisTarihi"] != null)
                    ModelState["BitisTarihi"].Errors.Clear();
            }
            else
            {
                if (ModelState["tvmKodu"] != null)
                    ModelState["tvmKodu"].Errors.Clear();

                if (ModelState["SigortaSirketiKodu"] != null)
                    ModelState["SigortaSirketiKodu"].Errors.Clear();

                if (ModelState["PoliceNo"] != null)
                    ModelState["PoliceNo"].Errors.Clear();

                if (ModelState["tcVkn"] != null)
                    ModelState["tcVkn"].Errors.Clear();
            }

            #endregion

            ITVMService _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketleriService.GetList();

            //var tvmler = _TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu).OrderBy(s => s.Unvani);
            var tvmler = _TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani);
            var disKaynaklar = _TVMService.GetDisUretimTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani);
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());

            model.policeListesi = new List<PoliceOnaylamaListModel>();
            PoliceOnaylamaListModel pol = new PoliceOnaylamaListModel();
            List<TaliAcenteKomisyonOrani> taliKomisyonOranlari;
            decimal GerceklesenTaliUretim = 0;
            TaliAcenteKomisyonOrani taliKomisyonOrani;
            int MerkezTVMkodu = 0;
            int taliTVMkodu = 0;
            model.MerkezAcentemi = false;
            TVMDetay tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                MerkezTVMkodu = _AktifKullaniciService.TVMKodu;
                taliTVMkodu = model.TVMKodu.HasValue ? model.TVMKodu.Value : _AktifKullaniciService.TVMKodu;
                model.MerkezAcentemi = true;
            }
            else
            {
                MerkezTVMkodu = tvmDetay.BagliOlduguTVMKodu;
                taliTVMkodu = _AktifKullaniciService.TVMKodu;
                TVMDetay merkezDetay = _TVMService.GetDetay(MerkezTVMkodu);
                model.SonPoliceOnayTarihi = merkezDetay.SonPoliceOnayTarihi;
            }

            List<PoliceGenel> polList = new List<PoliceGenel>();
            if (model.IslemTipi == 0) //Poliçe Onaylama
            {
              
                polList = _TaliPoliceService.GetPoliceGenelListesi(MerkezTVMkodu, taliTVMkodu, model.DisKaynakKodu, model.SigortaSirketiKodu, model.PoliceNo, model.tcVkn, model.plaka);
            }
            else  //Hesaplanmışlar Lsitesi
            {
                polList = _TaliPoliceService.GetHesaplanmisPoliceGenelListesi(MerkezTVMkodu, model.tvmMultiLists, model.sirketMultiLists, model.BaslangicTarihi, model.BitisTarihi);
            }

            if (polList != null)
            {
                if (model.IslemTipi == 0)
                {
                    #region Poliçe Onaylama
                    int policeTaliAcenteKodu = 0;
                    bool policemi = true;
                    foreach (var item in polList)
                    {
                        // sayac++;
                        if ((item.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && item.EkNo == 0) || (item.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && item.EkNo == 1))
                        {
                            policemi = true;
                        }
                        else if (item.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && item.BransKodu == BransListeCeviri.Dask && item.EkNo.ToString().Length > 4)
                        {
                            if (item.EkNo.ToString().Substring(4, 1) == "1")
                            {
                                policemi = true;
                            }
                            else
                            {
                                policemi = false;
                            }
                        }
                        else
                        {
                            policemi = false;
                        }

                        #region Poliçe Komisyon Oranı Atanıyor
                        if (policemi)
                        {
                            if (item.TaliAcenteKodu != null)
                            {
                                policeTaliAcenteKodu = item.TaliAcenteKodu.Value;
                            }
                            else
                            {
                                policeTaliAcenteKodu = taliTVMkodu;
                            }
                            if ((taliTVMkodu == policeTaliAcenteKodu && !item.TaliAcenteKodu.HasValue) || (taliTVMkodu == policeTaliAcenteKodu && item.TaliAcenteKodu.HasValue && item.TaliAcenteKodu == taliTVMkodu))
                            {
                                pol = new PoliceOnaylamaListModel();
                                pol.PoliceId = item.PoliceId;
                                pol.PoliceNo = item.PoliceNumarasi;
                                pol.tcVkn = item.PoliceSigortali != null ? item.PoliceSigortali.KimlikNo : "";
                                pol.Ad = item.PoliceSigortali != null ? item.PoliceSigortali.AdiUnvan : "";
                                pol.Soyad = item.PoliceSigortali != null ? item.PoliceSigortali.SoyadiUnvan : "";
                                pol.TanzimTarihi = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.ToString("dd/MM/yyyy") : null;
                                pol.BaslangicTarihi = item.BaslangicTarihi.HasValue ? item.BaslangicTarihi.Value.ToString("dd/MM/yyyy") : null;
                                pol.BitisTarihi = item.BitisTarihi.HasValue ? item.BitisTarihi.Value.ToString("dd/MM/yyyy") : null;
                                pol.EkNo = item.EkNo.ToString();
                                pol.NetPrim = item.NetPrim;
                                pol.DovizliBrütPrim = item.DovizliBrutPrim;
                                pol.DovizliNetPrim = item.DovizliNetPrim;
                                if (item.TaliKomisyonOran.HasValue)
                                {
                                    pol.KomisyonOrani = item.TaliKomisyonOran;
                                    pol.KomisyonTutari = item.TaliKomisyon;
                                }
                                else
                                {
                                    taliKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(MerkezTVMkodu, policeTaliAcenteKodu, model.DisKaynakKodu, item.TUMBirlikKodu, item.BransKodu.Value);
                                    GerceklesenTaliUretim = _KomisyonService.PoliceTVMGerceklesen(policeTaliAcenteKodu, item.TanzimTarihi.Value.Year, item.BransKodu.Value);
                                    if (GerceklesenTaliUretim == null)
                                    {
                                        GerceklesenTaliUretim = 0;
                                    }
                                    taliKomisyonOrani = new TaliAcenteKomisyonOrani();

                                    if (item.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (item.EkNo > 1 || item.BransKodu == BransListeCeviri.Dask)
                                        {
                                            if (item.EkNo.ToString().Length > 4)
                                            {
                                                if (item.EkNo.ToString().Substring(4, 1) != "1")
                                                {
                                                    var ZeylinPolicesi = _PoliceService.getPolice(item.TaliAcenteKodu.HasValue ? item.TaliAcenteKodu.Value : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 1);
                                                    if (ZeylinPolicesi != null)
                                                    {
                                                        taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var ZeylinPolicesi = _PoliceService.getPolice(policeTaliAcenteKodu > 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 1);
                                                if (ZeylinPolicesi != null)
                                                {
                                                    taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (item.EkNo > 0)
                                        {
                                            var ZeylinPolicesi = _PoliceService.getPolice(policeTaliAcenteKodu > 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 0);
                                            if (ZeylinPolicesi != null)
                                            {
                                                taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                            }
                                        }
                                    }
                                    if (taliKomisyonOranlari != null)
                                    {
                                        if (!taliKomisyonOrani.KomisyonOran.HasValue)
                                        {
                                            taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                            if (taliKomisyonOranlari.Count() > 1)
                                            {
                                                //kademeli komisyon varsa kademeler içinden en yükseğini belirliyor.
                                                decimal maxUertim = 0;
                                                foreach (var itemOran in taliKomisyonOranlari)
                                                {
                                                    if (itemOran.MaxUretim == null && itemOran.MinUretim == null)
                                                    {
                                                        taliKomisyonOrani = taliKomisyonOranlari[0];
                                                    }
                                                    else if (maxUertim < itemOran.MaxUretim)
                                                    {
                                                        maxUertim = itemOran.MaxUretim.Value;
                                                    }
                                                }

                                                taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MinUretim <= GerceklesenTaliUretim && GerceklesenTaliUretim <= s.MaxUretim);

                                                // gerçekleşen üretim kademelerden büyükse null gelecek en yüksek kademe atanacak
                                                if (taliKomisyonOrani == null && maxUertim != null)
                                                {
                                                    taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MaxUretim == maxUertim);
                                                }
                                                //else if (taliKomisyonOrani != null && maxUertim != 0)
                                                //{
                                                //    taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MaxUretim == maxUertim);
                                                //}
                                            }
                                            //kademeli çalışmıyorsa
                                            else if (taliKomisyonOranlari.Count > 0)
                                            {
                                                if (taliKomisyonOranlari[0].MinUretim == null && taliKomisyonOranlari[0].MaxUretim == null)
                                                {
                                                    taliKomisyonOrani = taliKomisyonOranlari[0];
                                                }
                                            }
                                        }
                                        if (taliKomisyonOrani.KomisyonOran != null && item.Komisyon != null)
                                        {
                                            pol.KomisyonOrani = taliKomisyonOrani.KomisyonOran;
                                            pol.KomisyonTutari = (item.Komisyon * pol.KomisyonOrani) / 100;
                                        }
                                    }
                                }
                                if (item.TaliAcenteKodu != null)
                                {
                                    pol.PoliceDurumu = "Hesaplanmış";
                                }
                                else
                                {
                                    pol.PoliceDurumu = "Hesaplanmamış";
                                }
                                if (tvmler != null && pol.KomisyonOrani.HasValue)
                                {
                                    pol.TVMKodu = policeTaliAcenteKodu != 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu;
                                    var tvmUnvan = tvmler.Where(s => s.Kodu == pol.TVMKodu).FirstOrDefault();
                                    pol.TVMUnvani = tvmUnvan.Unvani;
                                }
                                else
                                {
                                    if (tvmler != null)
                                    {
                                        pol.TVMKodu = MerkezTVMkodu;
                                        pol.TVMUnvani = _AktifKullaniciService.TVMUnvani;

                                    }
                                }
                                if (sigortaSirketleri != null)
                                {
                                    var sirketUnvan = sigortaSirketleri.Where(s => s.SirketKodu == item.TUMBirlikKodu).FirstOrDefault();
                                    pol.SigortaSirketiUnvani = sirketUnvan.SirketAdi;
                                }
                                model.policeListesi.Add(pol);
                            }
                            else if (!item.TaliAcenteKodu.HasValue && !item.UretimTaliAcenteKodu.HasValue)
                            {
                                pol = new PoliceOnaylamaListModel();
                                pol.PoliceId = item.PoliceId;
                                pol.PoliceNo = item.PoliceNumarasi;
                                pol.tcVkn = item.PoliceSigortali != null ? item.PoliceSigortali.KimlikNo : "";
                                pol.Ad = item.PoliceSigortali != null ? item.PoliceSigortali.AdiUnvan : "";
                                pol.Soyad = item.PoliceSigortali != null ? item.PoliceSigortali.SoyadiUnvan : "";
                                pol.TanzimTarihi = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.ToString("dd/MM/yyyy") : null;
                                pol.BaslangicTarihi = item.BaslangicTarihi.HasValue ? item.BaslangicTarihi.Value.ToString("dd/MM/yyyy") : null;
                                pol.BitisTarihi = item.BitisTarihi.HasValue ? item.BitisTarihi.Value.ToString("dd/MM/yyyy") : null;
                                pol.EkNo = item.EkNo.ToString();
                                pol.NetPrim = item.NetPrim;
                                pol.DovizliBrütPrim = item.DovizliBrutPrim;
                                pol.DovizliNetPrim = item.DovizliNetPrim;
                                if (item.TaliKomisyonOran.HasValue)
                                {
                                    pol.KomisyonOrani = item.TaliKomisyonOran;
                                    pol.KomisyonTutari = item.TaliKomisyon;
                                }
                                else
                                {
                                    taliKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(MerkezTVMkodu, policeTaliAcenteKodu, model.DisKaynakKodu, item.TUMBirlikKodu, item.BransKodu.Value);
                                    GerceklesenTaliUretim = _KomisyonService.PoliceTVMGerceklesen(policeTaliAcenteKodu, item.TanzimTarihi.Value.Year, item.BransKodu.Value);
                                    if (GerceklesenTaliUretim == null)
                                    {
                                        GerceklesenTaliUretim = 0;
                                    }
                                    taliKomisyonOrani = new TaliAcenteKomisyonOrani();

                                    if (item.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                    {
                                        if (item.EkNo > 1 || item.BransKodu == BransListeCeviri.Dask)
                                        {
                                            if (item.EkNo.ToString().Length > 4)
                                            {
                                                if (item.EkNo.ToString().Substring(4, 1) != "1")
                                                {
                                                    var ZeylinPolicesi = _PoliceService.getPolice(policeTaliAcenteKodu > 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 1);
                                                    if (ZeylinPolicesi != null)
                                                    {
                                                        taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var ZeylinPolicesi = _PoliceService.getPolice(policeTaliAcenteKodu > 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 1);
                                                if (ZeylinPolicesi != null)
                                                {
                                                    taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (item.EkNo > 0)
                                        {
                                            var ZeylinPolicesi = _PoliceService.getPolice(policeTaliAcenteKodu > 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 0);
                                            if (ZeylinPolicesi != null)
                                            {
                                                taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                            }
                                        }
                                    }
                                    if (taliKomisyonOranlari != null)
                                    {
                                        if (!taliKomisyonOrani.KomisyonOran.HasValue)
                                        {
                                            taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                            if (taliKomisyonOranlari.Count() > 1)
                                            {
                                                //kademeli komisyon varsa kademeler içinden en yükseğini belirliyor.
                                                decimal maxUertim = 0;
                                                foreach (var itemOran in taliKomisyonOranlari)
                                                {
                                                    if (maxUertim < itemOran.MaxUretim)
                                                    {
                                                        maxUertim = itemOran.MaxUretim.Value;
                                                    }
                                                }
                                                taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MinUretim <= GerceklesenTaliUretim && GerceklesenTaliUretim <= s.MaxUretim);
                                                // gerçekleşen üretim kademelerden büyükse null gelecek en yüksek kademe atanacak
                                                if (taliKomisyonOrani == null)
                                                {
                                                    taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MaxUretim == maxUertim);
                                                }
                                            }
                                            //kademeli çalışmıyorsa
                                            else if (taliKomisyonOranlari.Count > 0)
                                            {
                                                if (taliKomisyonOranlari[0].MinUretim == null && taliKomisyonOranlari[0].MaxUretim == null)
                                                {
                                                    taliKomisyonOrani = taliKomisyonOranlari[0];
                                                }
                                            }

                                        }
                                        if (taliKomisyonOrani.KomisyonOran != null && item.Komisyon != null)
                                        {
                                            pol.KomisyonOrani = taliKomisyonOrani.KomisyonOran;
                                            pol.KomisyonTutari = (item.Komisyon * pol.KomisyonOrani) / 100;
                                        }
                                    }
                                }
                                if (item.TaliAcenteKodu != null)
                                {
                                    pol.PoliceDurumu = "Hesaplanmış";
                                }
                                else
                                {
                                    pol.PoliceDurumu = "Hesaplanmamış";
                                }
                                if (tvmler != null && pol.KomisyonOrani.HasValue)
                                {
                                    pol.TVMKodu = item.TaliAcenteKodu.HasValue ? item.TaliAcenteKodu.Value : taliTVMkodu;
                                    var tvmUnvan = tvmler.Where(s => s.Kodu == pol.TVMKodu).FirstOrDefault();
                                    pol.TVMUnvani = tvmUnvan != null ? tvmUnvan.Unvani : "";
                                }
                                else
                                {
                                    if (tvmler != null)
                                    {
                                        pol.TVMKodu = MerkezTVMkodu;
                                        pol.TVMUnvani = _AktifKullaniciService.TVMUnvani;
                                    }
                                }
                                if (sigortaSirketleri != null)
                                {
                                    var sirketUnvan = sigortaSirketleri.Where(s => s.SirketKodu == item.TUMBirlikKodu).FirstOrDefault();
                                    pol.SigortaSirketiUnvani = sirketUnvan.SirketAdi;
                                }
                                model.policeListesi.Add(pol);
                            }
                        }
                        #endregion

                        #region Zeyil Komisyon Oranı Atanıyor

                        if (polList.Count > 1 && !policemi)
                        {
                            if ((item.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && item.EkNo != 1) || (item.TUMBirlikKodu != SigortaSirketiBirlikKodlari.ALLIANZSIGORTA && item.EkNo != 0))
                            {
                                if ((taliTVMkodu == policeTaliAcenteKodu && !item.UretimTaliAcenteKodu.HasValue) || (taliTVMkodu == policeTaliAcenteKodu && item.UretimTaliAcenteKodu.HasValue && item.UretimTaliAcenteKodu == taliTVMkodu))
                                {
                                    pol = new PoliceOnaylamaListModel();
                                    pol.PoliceId = item.PoliceId;
                                    pol.PoliceNo = item.PoliceNumarasi;
                                    pol.tcVkn = item.PoliceSigortali != null ? item.PoliceSigortali.KimlikNo : "";
                                    pol.Ad = item.PoliceSigortali != null ? item.PoliceSigortali.AdiUnvan : "";
                                    pol.Soyad = item.PoliceSigortali != null ? item.PoliceSigortali.SoyadiUnvan : "";
                                    pol.TanzimTarihi = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.ToString("dd/MM/yyyy") : null;
                                    pol.BaslangicTarihi = item.BaslangicTarihi.HasValue ? item.BaslangicTarihi.Value.ToString("dd/MM/yyyy") : null;
                                    pol.BitisTarihi = item.BitisTarihi.HasValue ? item.BitisTarihi.Value.ToString("dd/MM/yyyy") : null;
                                    pol.EkNo = item.EkNo.ToString();
                                    pol.NetPrim = item.NetPrim;
                                    pol.DovizliBrütPrim = item.DovizliBrutPrim;
                                    pol.DovizliNetPrim = item.DovizliNetPrim;
                                    if (item.TaliKomisyonOran.HasValue)
                                    {
                                        pol.KomisyonOrani = item.TaliKomisyonOran;
                                        pol.KomisyonTutari = item.TaliKomisyon;
                                    }
                                    else
                                    {
                                        taliKomisyonOranlari = _KomisyonService.GetTaliAcenteKomisyon(MerkezTVMkodu, policeTaliAcenteKodu, model.DisKaynakKodu, item.TUMBirlikKodu, item.BransKodu.Value);
                                        GerceklesenTaliUretim = _KomisyonService.PoliceTVMGerceklesen(policeTaliAcenteKodu, item.TanzimTarihi.Value.Year, item.BransKodu.Value);
                                        if (GerceklesenTaliUretim == null)
                                        {
                                            GerceklesenTaliUretim = 0;
                                        }
                                        taliKomisyonOrani = new TaliAcenteKomisyonOrani();

                                        if (item.TUMBirlikKodu == SigortaSirketiBirlikKodlari.ALLIANZSIGORTA)
                                        {
                                            if (item.EkNo > 1 || item.BransKodu == BransListeCeviri.Dask)
                                            {
                                                if (item.EkNo.ToString().Length > 4)
                                                {
                                                    if (item.EkNo.ToString().Substring(4, 1) != "1")
                                                    {
                                                        var ZeylinPolicesi = _PoliceService.getPolice(policeTaliAcenteKodu > 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 1);
                                                        if (ZeylinPolicesi != null)
                                                        {
                                                            taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (item.EkNo > 0)
                                            {
                                                var ZeylinPolicesi = _PoliceService.getPolice(policeTaliAcenteKodu > 0 ? policeTaliAcenteKodu : _AktifKullaniciService.TVMKodu, item.TUMBirlikKodu, item.PoliceNumarasi, 0);
                                                if (ZeylinPolicesi != null)
                                                {
                                                    taliKomisyonOrani.KomisyonOran = ZeylinPolicesi.TaliKomisyonOran;
                                                }
                                            }
                                        }
                                        if (taliKomisyonOranlari != null)
                                        {
                                            if (!taliKomisyonOrani.KomisyonOran.HasValue)
                                            {
                                                taliKomisyonOrani = new TaliAcenteKomisyonOrani();
                                                if (taliKomisyonOranlari.Count() > 1)
                                                {
                                                    //kademeli komisyon varsa kademeler içinden en yükseğini belirliyor.
                                                    decimal maxUertim = 0;
                                                    foreach (var itemOran in taliKomisyonOranlari)
                                                    {
                                                        if (maxUertim < itemOran.MaxUretim)
                                                        {
                                                            maxUertim = itemOran.MaxUretim.Value;
                                                        }
                                                    }
                                                    taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MinUretim <= GerceklesenTaliUretim && GerceklesenTaliUretim <= s.MaxUretim);
                                                    // gerçekleşen üretim kademelerden büyükse null gelecek en yüksek kademe atanacak
                                                    if (taliKomisyonOrani == null)
                                                    {
                                                        taliKomisyonOrani = taliKomisyonOranlari.Find(s => s.MaxUretim == maxUertim);
                                                    }
                                                }
                                                //kademeli çalışmıyorsa
                                                else if (taliKomisyonOranlari.Count > 0)
                                                {
                                                    if (taliKomisyonOranlari[0].MinUretim == null && taliKomisyonOranlari[0].MaxUretim == null)
                                                    {
                                                        taliKomisyonOrani = taliKomisyonOranlari[0];
                                                    }
                                                }

                                            }
                                            if (taliKomisyonOrani.KomisyonOran != null && item.Komisyon != null)
                                            {
                                                pol.KomisyonOrani = taliKomisyonOrani.KomisyonOran;
                                                pol.KomisyonTutari = (item.Komisyon * pol.KomisyonOrani) / 100;
                                            }
                                        }
                                    }
                                    if (item.TaliAcenteKodu != null && item.TaliKomisyon != null)
                                    {
                                        pol.PoliceDurumu = "Hesaplanmış";
                                    }
                                    else
                                    {
                                        pol.PoliceDurumu = "Hesaplanmamış";
                                    }
                                    if (tvmler != null && pol.KomisyonOrani.HasValue)
                                    {
                                        if (item.TaliAcenteKodu.HasValue && item.TaliKomisyon != null)
                                        {
                                            pol.TVMKodu = item.TaliAcenteKodu.Value;
                                        }
                                        else
                                        {
                                            pol.TVMKodu = taliTVMkodu;
                                        }

                                        var tvmUnvan = tvmler.Where(s => s.Kodu == pol.TVMKodu).FirstOrDefault();
                                        if (tvmUnvan != null)
                                        {
                                            pol.TVMUnvani = tvmUnvan.Unvani;

                                        }
                                    }
                                    else
                                    {
                                        if (tvmler != null)
                                        {
                                            pol.TVMKodu = MerkezTVMkodu;
                                            pol.TVMUnvani = _AktifKullaniciService.TVMUnvani;

                                        }
                                    }
                                    if (sigortaSirketleri != null)
                                    {
                                        var sirketUnvan = sigortaSirketleri.Where(s => s.SirketKodu == item.TUMBirlikKodu).FirstOrDefault();
                                        pol.SigortaSirketiUnvani = sirketUnvan.SirketAdi;
                                    }
                                    model.policeListesi.Add(pol);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region Hesaplanmışlar Listesi

                    foreach (var item in polList)
                    {
                        pol = new PoliceOnaylamaListModel();
                        pol.PoliceId = item.PoliceId;
                        pol.PoliceNo = item.PoliceNumarasi;
                        pol.tcVkn = item.PoliceSigortali != null ? item.PoliceSigortali.KimlikNo : "";
                        pol.Ad = item.PoliceSigortali != null ? item.PoliceSigortali.AdiUnvan : "";
                        pol.Soyad = item.PoliceSigortali != null ? item.PoliceSigortali.SoyadiUnvan : "";
                        pol.TanzimTarihi = item.TanzimTarihi.HasValue ? item.TanzimTarihi.Value.ToString("dd/MM/yyyy") : null;
                        pol.BaslangicTarihi = item.BaslangicTarihi.HasValue ? item.BaslangicTarihi.Value.ToString("dd/MM/yyyy") : null;
                        pol.BitisTarihi = item.BitisTarihi.HasValue ? item.BitisTarihi.Value.ToString("dd/MM/yyyy") : null;
                        pol.EkNo = item.EkNo.ToString();
                        pol.NetPrim = item.NetPrim;
                        pol.KomisyonOrani = item.TaliKomisyonOran;
                        pol.KomisyonTutari = item.TaliKomisyon;
                        pol.PoliceDurumu = "Hesaplanmış";
                        pol.DovizliBrütPrim = item.DovizliBrutPrim;
                        pol.DovizliNetPrim = item.DovizliNetPrim;
                        if (tvmler != null)
                        {
                            pol.TVMKodu = item.TaliAcenteKodu.HasValue ? item.TaliAcenteKodu.Value : _AktifKullaniciService.TVMKodu;
                            var tvmUnvan = tvmler.Where(s => s.Kodu == pol.TVMKodu).FirstOrDefault();
                            pol.TVMUnvani = tvmUnvan.Unvani;


                        }
                        if (disKaynaklar != null)
                        {
                            if (item.UretimTaliAcenteKodu.HasValue)
                            {
                                pol.DisKaynakKodu = item.UretimTaliAcenteKodu.Value;
                                var disKaynakUnvan = disKaynaklar.Where(s => s.Kodu == pol.DisKaynakKodu).FirstOrDefault();
                                pol.DisKaynakUnvani = disKaynakUnvan.Unvani;
                            }
                        }

                        if (sigortaSirketleri != null)
                        {
                            var sirketUnvan = sigortaSirketleri.Where(s => s.SirketKodu == item.TUMBirlikKodu).FirstOrDefault();
                            pol.SigortaSirketiUnvani = sirketUnvan.SirketAdi;
                        }
                        model.policeListesi.Add(pol);
                    }
                    #endregion
                }
                var list = (from m in model.policeListesi
                            select m.TVMKodu).Distinct();
                PoliceTaliAcentelerTVMModel listItem = new PoliceTaliAcentelerTVMModel();
                model.taliPoliceGrupListe = new List<PoliceTaliAcentelerTVMModel>();
                foreach (var item in list)
                {
                    listItem = new PoliceTaliAcentelerTVMModel();
                    listItem.TVMKodu = item.Value;
                    model.taliPoliceGrupListe.Add(listItem);
                }
            }

            // model.Tvmler = new SelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu), "Kodu", "Unvani").ListWithOptionLabel();
            model.Tvmler = new SelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani").ListWithOptionLabel();
            model.SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi").ListWithOptionLabel();
            // model.tvmMultiList = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(_AktifKullaniciService.TVMKodu), "Kodu", "Unvani");
            model.tvmMultiList = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.sirketMultiList = new MultiSelectList(sigortaSirketleri, "SirketKodu", "SirketAdi");
            model.Branslar = new SelectList(brans, "BransKodu", "BransAdi").ListWithOptionLabel();
            model.Islemler = new SelectList(Islemler.PoliceOnaylamaIslemTipleri(), "Value", "Text", model.IslemTipi);
            model.DisKaynakList = this.PoliceTransferTaliAcenteler;
            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.SigortaUzmanlari, AltMenuKodu = AltMenuler.PoliceOnaylama, SekmeKodu = 0)]
        public ActionResult PoliceOnayGuncelleme(int policeId, decimal? taliAcenteKomisyonOrani, decimal? komisyonTutari, int? taliAcenteKodu)
        {
            if (policeId > 0 && taliAcenteKomisyonOrani != null && komisyonTutari != null && taliAcenteKodu != null)
            {
                try
                {
                    var hedefGuncellendiMi = false;
                    PoliceGenel police = _PoliceService.GetPolice(policeId);
                    police.OnaylayanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                    var polices = _PoliceService.GetGuncellenecekPoliceZeyl(police.TVMKodu.Value, police.TUMBirlikKodu, police.PoliceNumarasi);
                    foreach (var item in polices)
                    {
                        item.OnaylayanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                        if (taliAcenteKomisyonOrani > 100)
                        {
                            taliAcenteKomisyonOrani = taliAcenteKomisyonOrani / 10;
                        }
                        item.TaliKomisyonOran = taliAcenteKomisyonOrani;
                        item.TaliKomisyon = (item.Komisyon * taliAcenteKomisyonOrani) / 100;
                        if (taliAcenteKodu != null)
                        {
                            item.TaliAcenteKodu = taliAcenteKodu;
                        }

                        bool guncellendiMi;
                        var guncellenenPolice = _PoliceService.TaliAcnteKomisyonGuncelle(item, out guncellendiMi);

                        if (guncellendiMi)
                        {
                            hedefGuncellendiMi = _PoliceService.GetPoliceUretimHedefGerceklesen(guncellenenPolice);
                        }
                    }
                    if (hedefGuncellendiMi)
                    {
                        return Json(new { Success = "True" });
                    }
                    else
                    {
                        return Json(new { Success = "False", mesaj = "Hedef Tablosu güncellenirken bir hata oluştu." });
                    }
                    //police.TaliKomisyonOran = taliAcenteKomisyonOrani;
                    //police.TaliKomisyon = komisyonTutari.HasValue ? komisyonTutari.Value : 0;
                    //if (taliAcenteKodu != null)
                    //{
                    //    police.TaliAcenteKodu = taliAcenteKodu;
                    //}

                    //bool guncellendiMi;
                    //var guncellenenPolice = _PoliceService.TaliAcnteKomisyonGuncelle(police, out guncellendiMi);
                    //if (guncellendiMi)
                    //{
                    //    var hedefGuncellendiMi = _PoliceService.GetPoliceUretimHedefGerceklesen(guncellenenPolice);
                    //    if (hedefGuncellendiMi)
                    //    {
                    //        return Json(new { Success = "True" });
                    //    }
                    //    else
                    //    {
                    //        return Json(new { Success = "False", mesaj = "Hedef Tablosu güncellenirken bir hata oluştu." });
                    //    }

                    //}
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                                    validationError.PropertyName,
                                                    validationError.ErrorMessage);
                        }
                    }
                }
            }
            return View();
        }

        private List<SelectListItem> _PoliceTransferTaliAcenteler;
        protected List<SelectListItem> PoliceTransferTaliAcenteler
        {
            get
            {
                if (_PoliceTransferTaliAcenteler == null)
                {
                    List<TVMDetay> taliTvmler = _TVMService.GetListTVMDetayPoliceTransferTali();

                    _PoliceTransferTaliAcenteler = new SelectList(taliTvmler, "Kodu", "Unvani", "").ListWithOptionLabel();
                }

                return _PoliceTransferTaliAcenteler;
            }
        }
    }
}
