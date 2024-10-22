using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using AutoMapper;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.YetkiYonetimi, SekmeKodu = AltMenuSekmeler.YetkiAyarlari)]
    public class YetkiController : Controller
    {
        IYetkiService _YetkiService;
        IMenuService _MenuService;
        IAktifKullaniciService _AktifKullanici;
        ITVMService _TvmService;
        ILogService _Log;

        public YetkiController(IYetkiService yetki, IMenuService menuService, IAktifKullaniciService aktifKullanici, ITVMService tvm)
        {
            _MenuService = menuService;
            _AktifKullanici = aktifKullanici;
            _YetkiService = yetki;
            _TvmService = tvm;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                YetkiModel model = new YetkiModel();

                model.TVMKodu = _AktifKullanici.TVMKodu;
                model.TVMtable = _YetkiService.GetListYetkiGrupByTVMKodu(model.TVMKodu);
                model.TVMSelectList = new SelectList(_TvmService.GetListTVMDetayYetkili(), "Kodu", "Unvani", "").ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Liste(YetkiModel model)
        {
            try
            {
                model.TVMSelectList = new SelectList(_TvmService.GetListTVMDetayYetkili(), "Kodu", "Unvani", "").ToList();
                model.TVMtable = _YetkiService.GetListYetkiGrupByTVMKodu(model.TVMKodu);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult Detay(int Id)
        {
            try
            {
                if (Id > 0 && _YetkiService.CheckAuthorityYetkiGrup(Id))
                {
                    YetkiEklemeModel model = ModelDoldurGuncelleDetay(Id);
                    return View(model);
                }
                return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.YetkiYonetimi,
                       SekmeKodu = AltMenuSekmeler.YetkiAyarlari,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle()
        {
            try
            {
                YetkiEklemeModel model = new YetkiEklemeModel();
                model.AnaMenuler = new List<AnaMenuYetkiModel>();

                List<AnaMenu> anaMenuler = _MenuService.GetAnaMenuListYetkili();
                List<AltMenu> altMenuler = _MenuService.GetAltMenuListYetkili();
                List<AltMenuSekme> sekmeler = _MenuService.GetALtMenuSekmeListYetkili();

                foreach (var item in anaMenuler)
                {
                    AnaMenuYetkiModel anamenu = new AnaMenuYetkiModel();
                    anamenu.MenuAdi = item.Aciklama;
                    anamenu.AnaMenuKodu = (short)item.AnaMenuKodu;
                    foreach (var item2 in altMenuler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu))
                    {
                        AltMenuYetkiModel altmenu = new AltMenuYetkiModel();
                        altmenu.MenuAdi = item2.Aciklama;
                        altmenu.AltMenuKodu = (short)item2.AltMenuKodu;
                        altmenu.AnaMenuKodu = (short)item2.AnaMenuKodu;
                        foreach (var item3 in sekmeler.Where(s => s.AltMenuKodu == item2.AltMenuKodu && s.AnaMenuKodu == item.AnaMenuKodu))
                        {
                            SekmeYetkiModel sekme = new SekmeYetkiModel();
                            sekme.MenuAdi = item3.Aciklama;
                            sekme.AnaMenuKodu = (short)item3.AnaMenuKodu;
                            sekme.AltMenuKodu = (short)item3.AltMenuKodu;
                            sekme.SekmeKodu = (short)item3.SekmeKodu;
                            altmenu.Sekmeler.Add(sekme);
                        }
                        anamenu.AltMenuler.Add(altmenu);
                    }
                    model.AnaMenuler.Add(anamenu);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.YetkiYonetimi,
                       SekmeKodu = AltMenuSekmeler.YetkiAyarlari,
                       menuPermission = MenuPermission.Ekleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Ekle(YetkiEklemeModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TVMYetkiGruplari yetkigrubu = new TVMYetkiGruplari();
                    yetkigrubu.TVMKodu = model.TVMKodu.Value;
                    yetkigrubu.YetkiGrupAdi = model.GrupAdi;
                    yetkigrubu.YetkiSeviyesi = model.YetkiSeviyesi;
                    yetkigrubu = _YetkiService.CreateYetkiGrubu(yetkigrubu);

                    //TUM Ana menüler ekleniyor yetkileri 0 olarak veriliyor.
                    List<AnaMenu> tumAnaMenuler = _MenuService.GetAnaMenuList();
                    foreach (var item in tumAnaMenuler)
                    {
                        TVMYetkiGrupYetkileri yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                        yetkiGrupYetki.YetkiGrupKodu = yetkigrubu.YetkiGrupKodu;
                        yetkiGrupYetki.AnaMenuKodu = item.AnaMenuKodu;
                        yetkiGrupYetki.AltMenuKodu = 0;
                        yetkiGrupYetki.SekmeKodu = 0;

                        yetkiGrupYetki.Gorme = 0;
                        yetkiGrupYetki.YeniKayit = 0;
                        yetkiGrupYetki.Degistirme = 0;
                        yetkiGrupYetki.Silme = 0;
                        yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);
                    }

                    //TUM Alt menüler ekleniyor yetkileri 0 olarak veriliyor.
                    List<AltMenu> tumAltMenuler = _MenuService.GetAltMenuList();
                    foreach (var item in tumAltMenuler)
                    {
                        TVMYetkiGrupYetkileri yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                        yetkiGrupYetki.YetkiGrupKodu = yetkigrubu.YetkiGrupKodu;
                        yetkiGrupYetki.AnaMenuKodu = item.AnaMenuKodu;
                        yetkiGrupYetki.AltMenuKodu = item.AltMenuKodu;
                        yetkiGrupYetki.SekmeKodu = 0;

                        yetkiGrupYetki.Gorme = 0;
                        yetkiGrupYetki.YeniKayit = 0;
                        yetkiGrupYetki.Degistirme = 0;
                        yetkiGrupYetki.Silme = 0;
                        yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);
                    }

                    //TUM Sekmeler ekleniyor yetkileri 0 olarak veriliyor.
                    List<AltMenuSekme> tumSekmeler = _MenuService.GetALtMenuSekmeList();
                    foreach (var item in tumSekmeler)
                    {
                        TVMYetkiGrupYetkileri yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                        yetkiGrupYetki.YetkiGrupKodu = yetkigrubu.YetkiGrupKodu;
                        yetkiGrupYetki.AnaMenuKodu = item.AnaMenuKodu;
                        yetkiGrupYetki.AltMenuKodu = item.AltMenuKodu;
                        yetkiGrupYetki.SekmeKodu = item.SekmeKodu;

                        yetkiGrupYetki.Gorme = 0;
                        yetkiGrupYetki.YeniKayit = 0;
                        yetkiGrupYetki.Degistirme = 0;
                        yetkiGrupYetki.Silme = 0;
                        yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);
                    }

                    YetkiGuncelle(model, yetkigrubu);

                    return RedirectToAction("Detay", "Yetki", new { Id = yetkigrubu.YetkiGrupKodu });
                }
                YetkiModelDoldurEkle(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.YetkiYonetimi,
                       SekmeKodu = AltMenuSekmeler.YetkiAyarlari,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Guncelle(int Id)
        {
            try
            {
                if (Id > 0 && _YetkiService.CheckAuthorityYetkiGrup(Id))
                {
                    YetkiEklemeModel model = ModelDoldurGuncelleDetay(Id);

                    return View(model);
                }
                return new RedirectResult("~/Error/ErrorPage/403");
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                       AltMenuKodu = AltMenuler.YetkiYonetimi,
                       SekmeKodu = AltMenuSekmeler.YetkiAyarlari,
                       menuPermission = MenuPermission.Guncelleme,
                       menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult Guncelle(YetkiEklemeModel model)
        {
            try
            {
                if (ModelState.IsValid && model.YetkiGrupKodu > 0)
                {
                    if (_YetkiService.CheckAuthorityYetkiGrup(model.YetkiGrupKodu))
                    {
                        if (_AktifKullanici.TVMKodu != model.TVMKodu)
                        {
                            //var yetkiGrupKoduKontrolu = model.YetkiGrupKodu;
                            var yetkiGrubuVarmi = _YetkiService.GetListYetkiGrupByTVMKodu(model.TVMKodu.Value);
                            if (yetkiGrubuVarmi.Count > 0)
                            {
                                foreach (var item in yetkiGrubuVarmi)
                                {

                                    if (model.TVMKodu == item.TVMKodu && model.GrupAdi == item.YetkiGrupAdi)
                                    {
                                        TVMYetkiGruplari yetkiGrup = _YetkiService.GetYetkiGrup(item.YetkiGrupKodu);
                                        yetkiGrup.TVMKodu = model.TVMKodu.Value;
                                        yetkiGrup.YetkiGrupAdi = model.GrupAdi;
                                        yetkiGrup.YetkiSeviyesi = model.YetkiSeviyesi;
                                        _YetkiService.UpdateYetkiGrubu(yetkiGrup);
                                        model.YetkiGrupKodu = item.YetkiGrupKodu;
                                        YetkiGuncelle(model, yetkiGrup);

                                        return RedirectToAction("Detay", "Yetki", new { Id = yetkiGrup.YetkiGrupKodu });
                                       
                                    }
                                    var varMi = _YetkiService.GetYetkiGrupKontrolu(model.YetkiGrupKodu, model.TVMKodu.Value);
                                    if (varMi==null)
                                    {                                                                     
                                            Ekle(model);
                                            return RedirectToAction("Liste", "Yetki");                                       
                                    }
                                 
                                }
                            }
                            else
                            {
                                Ekle(model);
                                return RedirectToAction("Liste", "Yetki");
                            }
                        }
                        else
                        {
                            TVMYetkiGruplari yetkiGrup = _YetkiService.GetYetkiGrup(model.YetkiGrupKodu);
                            yetkiGrup.TVMKodu = model.TVMKodu.Value;
                            yetkiGrup.YetkiGrupAdi = model.GrupAdi;
                            yetkiGrup.YetkiSeviyesi = model.YetkiSeviyesi;
                            _YetkiService.UpdateYetkiGrubu(yetkiGrup);

                            YetkiGuncelle(model, yetkiGrup);

                            return RedirectToAction("Detay", "Yetki", new { Id = yetkiGrup.YetkiGrupKodu });
                        }

                    }
                    return new RedirectResult("~/Error/ErrorPage/403");
                }
                ModelState.AddModelError("", babonline.Message_RequiredValues);
                YetkiModelDoldurEkle(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        private YetkiEklemeModel ModelDoldurGuncelleDetay(int YetkiId)
        {
            TVMYetkiGruplari yetkiGrubu = _YetkiService.GetYetkiGrup(YetkiId);

            List<TVMYetkiGrupYetkileri> YetkiGrupYetkileri = _YetkiService.GetListYetkiGrupYetki(YetkiId);
            List<AnaMenu> anaMenuler = _MenuService.GetAnaMenuListYetkili();
            List<AltMenu> altMenuler = _MenuService.GetAltMenuListYetkili();
            List<AltMenuSekme> sekmeler = _MenuService.GetALtMenuSekmeListYetkili();

            YetkiEklemeModel model = new YetkiEklemeModel();
            model.AnaMenuler = new List<AnaMenuYetkiModel>();

            model.YetkiGrupKodu = yetkiGrubu.YetkiGrupKodu;
            model.GrupAdi = yetkiGrubu.YetkiGrupAdi;
            model.TVMKodu = yetkiGrubu.TVMKodu;
            model.YetkiSeviyesi = yetkiGrubu.YetkiSeviyesi.HasValue ? yetkiGrubu.YetkiSeviyesi.Value : false;

            model.TVMUnvani = _TvmService.GetTvmUnvan(yetkiGrubu.TVMKodu);

            foreach (var item in YetkiGrupYetkileri)
            {
                AnaMenuYetkiModel anamenu = new AnaMenuYetkiModel();
                AltMenuYetkiModel altmenu = new AltMenuYetkiModel();

                if (item.AnaMenuKodu != 0 && item.AltMenuKodu == 0 && item.SekmeKodu == 0)
                {
                    AnaMenu ANAMENU = anaMenuler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu).FirstOrDefault();
                    if (ANAMENU != null)
                    {
                        anamenu.AnaMenuKodu = (short)ANAMENU.AnaMenuKodu;
                        anamenu.MenuAdi = ANAMENU.Aciklama;
                        anamenu.Gorme = item.Gorme == 1 ? true : false;
                        anamenu.YeniKayit = item.YeniKayit == 1 ? true : false;
                        anamenu.Degistirme = item.Degistirme == 1 ? true : false;
                        anamenu.Silme = item.Silme == 1 ? true : false;
                        model.AnaMenuler.Add(anamenu);
                    }
                }
                if (item.SekmeKodu == 0 && item.AltMenuKodu != 0)
                {
                    AltMenu ALTMENU = altMenuler.Where(s => s.AltMenuKodu == item.AltMenuKodu && s.AnaMenuKodu == item.AnaMenuKodu).FirstOrDefault();
                    if (ALTMENU != null)
                    {
                        altmenu.AnaMenuKodu = (short)ALTMENU.AnaMenuKodu;
                        altmenu.AltMenuKodu = (short)ALTMENU.AltMenuKodu;
                        altmenu.MenuAdi = ALTMENU.Aciklama;
                        altmenu.Gorme = item.Gorme == 1 ? true : false;
                        altmenu.YeniKayit = item.YeniKayit == 1 ? true : false;
                        altmenu.Degistirme = item.Degistirme == 1 ? true : false;
                        altmenu.Silme = item.Silme == 1 ? true : false;

                        //Alt Menuleri ne Ekleniyor...
                        AnaMenuYetkiModel _anamenu = model.AnaMenuler.Where(s => s.AnaMenuKodu == altmenu.AnaMenuKodu).FirstOrDefault();
                        if (_anamenu != null)
                            _anamenu.AltMenuler.Add(altmenu);
                    }
                }

                if (item.SekmeKodu != 0)
                {
                    SekmeYetkiModel sekme = new SekmeYetkiModel();

                    AltMenuSekme SEKME = sekmeler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu && s.AltMenuKodu == item.AltMenuKodu &&
                                                             s.SekmeKodu == item.SekmeKodu).FirstOrDefault();

                    if (SEKME != null)
                    {
                        sekme.AnaMenuKodu = (short)SEKME.AnaMenuKodu;
                        sekme.AltMenuKodu = (short)SEKME.AltMenuKodu;
                        sekme.SekmeKodu = (short)SEKME.SekmeKodu;
                        sekme.MenuAdi = SEKME.Aciklama;
                        sekme.Gorme = item.Gorme == 1 ? true : false;
                        sekme.YeniKayit = item.YeniKayit == 1 ? true : false;
                        sekme.Degistirme = item.Degistirme == 1 ? true : false;
                        sekme.Silme = item.Silme == 1 ? true : false;

                        //Alt Menuye Sekme Ekleniyor..
                        AnaMenuYetkiModel _anamenu = model.AnaMenuler.Where(s => s.AnaMenuKodu == sekme.AnaMenuKodu).FirstOrDefault();
                        if (_anamenu != null)
                        {
                            AltMenuYetkiModel _altmenu = _anamenu.AltMenuler.Where(s => s.AltMenuKodu == sekme.AltMenuKodu).FirstOrDefault();
                            if (_altmenu != null)
                                _altmenu.Sekmeler.Add(sekme);
                        }
                    }
                }
            }
            return model;
        }

        private void YetkiModelDoldurEkle(YetkiEklemeModel model)
        {
            List<AnaMenu> anaMenuler = _MenuService.GetAnaMenuListYetkili();
            List<AltMenu> altMenuler = _MenuService.GetAltMenuListYetkili();
            List<AltMenuSekme> sekmeler = _MenuService.GetALtMenuSekmeListYetkili();

            for (int i = 0; i < model.AnaMenuler.Count; i++)
            {
                model.AnaMenuler[i].MenuAdi = anaMenuler.Where(s => s.AnaMenuKodu == model.AnaMenuler[i].AnaMenuKodu).First().Aciklama;
                for (int p = 0; p < model.AnaMenuler[i].AltMenuler.Count; p++)
                {
                    model.AnaMenuler[i].AltMenuler[p].MenuAdi = altMenuler.Where(s => s.AnaMenuKodu == model.AnaMenuler[i].AltMenuler[p].AnaMenuKodu &&
                                                                                 s.AltMenuKodu == model.AnaMenuler[i].AltMenuler[p].AltMenuKodu).
                                                                                 First().Aciklama;
                    for (int c = 0; c < model.AnaMenuler[i].AltMenuler[p].Sekmeler.Count; c++)
                    {
                        int anakodu = model.AnaMenuler[i].AltMenuler[p].Sekmeler[c].AnaMenuKodu;
                        int altkodu = model.AnaMenuler[i].AltMenuler[p].Sekmeler[c].AltMenuKodu;
                        int sekme = model.AnaMenuler[i].AltMenuler[p].Sekmeler[c].SekmeKodu;
                        model.AnaMenuler[i].AltMenuler[p].Sekmeler[c].MenuAdi = sekmeler.Where(s => s.AnaMenuKodu == anakodu &&
                                                                                               s.AltMenuKodu == altkodu &&
                                                                                               s.SekmeKodu == sekme).
                                                                                               First().Aciklama;
                    }
                }
            }
        }

        private void YetkiGuncelle(YetkiEklemeModel model, TVMYetkiGruplari yetkigrubu)
        {
            List<TVMYetkiGrupYetkileri> yetkiler = _YetkiService.GetListYetkiGrupYetki(yetkigrubu.YetkiGrupKodu);

            foreach (var item in yetkiler)
            {
                //Ana Menüler Güncelleniyor
                if (item.AnaMenuKodu != 0 && item.AltMenuKodu == 0 && item.SekmeKodu == 0)
                {
                    AnaMenuYetkiModel anaMenu = model.AnaMenuler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu).FirstOrDefault();
                    if (anaMenu != null)
                    {
                        item.Gorme = (byte)(anaMenu.Gorme ? 1 : 0);
                        item.YeniKayit = (byte)(anaMenu.YeniKayit ? 1 : 0);
                        item.Degistirme = (byte)(anaMenu.Degistirme ? 1 : 0);
                        item.Silme = (byte)(anaMenu.Silme ? 1 : 0);
                    }
                }
                else if (item.AnaMenuKodu != 0 && item.AltMenuKodu != 0 && item.SekmeKodu == 0)
                {
                    //AltMenuler Güncelleniyor.
                    AnaMenuYetkiModel anaMenu = model.AnaMenuler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu).FirstOrDefault();
                    if (anaMenu != null && anaMenu.AltMenuler != null && anaMenu.AltMenuler.Count() > 0)
                    {
                        AltMenuYetkiModel altMenu = anaMenu.AltMenuler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu &&
                                                                                  s.AltMenuKodu == item.AltMenuKodu).FirstOrDefault();
                        if (altMenu != null)
                        {
                            item.Gorme = (byte)(altMenu.Gorme ? 1 : 0);
                            item.YeniKayit = (byte)(altMenu.YeniKayit ? 1 : 0);
                            item.Degistirme = (byte)(altMenu.Degistirme ? 1 : 0);
                            item.Silme = (byte)(altMenu.Silme ? 1 : 0);
                        }
                    }
                }
                else if (item.SekmeKodu != 0)
                {
                    //Sekmeler güncelleniyor
                    AnaMenuYetkiModel anaMenu = model.AnaMenuler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu).FirstOrDefault();
                    if (anaMenu != null && anaMenu.AltMenuler != null && anaMenu.AltMenuler.Count() > 0)
                    {
                        AltMenuYetkiModel altMenu = anaMenu.AltMenuler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu &&
                                                                                  s.AltMenuKodu == item.AltMenuKodu).FirstOrDefault();
                        if (altMenu != null && altMenu.Sekmeler != null && altMenu.Sekmeler.Count() > 0)
                        {
                            SekmeYetkiModel sekme = altMenu.Sekmeler.Where(s => s.AnaMenuKodu == item.AnaMenuKodu &&
                                                                                s.AltMenuKodu == item.AltMenuKodu && s.SekmeKodu == item.SekmeKodu).FirstOrDefault();
                            if (sekme != null)
                            {
                                item.Gorme = (byte)(sekme.Gorme ? 1 : 0);
                                item.YeniKayit = (byte)(sekme.YeniKayit ? 1 : 0);
                                item.Degistirme = (byte)(sekme.Degistirme ? 1 : 0);
                                item.Silme = (byte)(sekme.Silme ? 1 : 0);
                            }
                        }
                    }
                }

                _YetkiService.UpdateYetkiGrupYetkileri(item);
            }
        }


        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
                  AltMenuKodu = AltMenuler.YetkiYonetimi,
                  SekmeKodu = AltMenuSekmeler.YetkiAyarlari,
                  menuPermission = MenuPermission.Ekleme,
                  menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult YetkiSablonuEkle()
        {
            try
            {
                YetkiEklemeModel model = new YetkiEklemeModel();

                model.AnaMenuler = new List<AnaMenuYetkiModel>();
                //if (model.GrupAdi=="Yönetim")
                //{
                //    model.YetkiGrupKodu = 1;
                //}
                //var yetkiSablonu = _YetkiService.GetListTVMYetkiGrupSablonu(model.YetkiGrupKodu);




                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Yonetim,
          AltMenuKodu = AltMenuler.YetkiYonetimi,
          SekmeKodu = AltMenuSekmeler.YetkiAyarlari,
          menuPermission = MenuPermission.Ekleme,
          menuPermissionType = MenuPermissionType.ALtMenuSekme)]
        public ActionResult YetkiSablonuEkle(YetkiEklemeModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TVMYetkiGruplari yetkigrubu = new TVMYetkiGruplari();
                    yetkigrubu.TVMKodu = model.TVMKodu.Value;
                    yetkigrubu.YetkiGrupAdi = model.GrupAdi;
                    yetkigrubu.YetkiSeviyesi = model.YetkiSeviyesi;
                    yetkigrubu = _YetkiService.CreateYetkiGrubu(yetkigrubu);


                    List<AnaMenu> tumAnaMenuler = _MenuService.GetAnaMenuList();
                    List<AltMenu> tumAltMenuler = _MenuService.GetAltMenuList();
                    List<AltMenuSekme> tumSekmeler = _MenuService.GetALtMenuSekmeList();

                    model.AnaMenuler = new List<AnaMenuYetkiModel>();
                    if (model.GrupAdi == "YÖNETİM")
                    {
                        model.YetkiGrupKodu = 1;
                    }
                    var yetkiSablonu = _YetkiService.GetListTVMYetkiGrupSablonu(model.YetkiGrupKodu);
                    List<TVMYetkiGrupYetkileri> YetkiGrupYetkileri = _YetkiService.GetListYetkiGrupYetki(1622);

                    var ss = YetkiGrupYetkileri.GroupBy(x => new { x.AnaMenuKodu });
                    var anaMenuSablonu = yetkiSablonu.GroupBy(x => new { x.AnaMenuKodu });

                    //foreach (var sablon in yetkiSablonu)
                    //{
                    int sayac = tumAnaMenuler.Count;

                    foreach (var sablon in ss)
                    {

                        foreach (var item in tumAnaMenuler)
                        {
                            if (sayac > 0)
                            {
                                TVMYetkiGrupYetkileri yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                                //yetkigrubu.YetkiGrupKodu = sablon.YetkiGrupKodu;
                                yetkiGrupYetki.YetkiGrupKodu = yetkigrubu.YetkiGrupKodu;
                                //yetkiGrupYetki.AnaMenuKodu = sablon.AnaMenuKodu;



                                yetkiGrupYetki.AnaMenuKodu = sablon.Key.AnaMenuKodu;


                                //if (sablon.Key.AltMenuKodu != null)
                                //{
                                //    yetkiGrupYetki.AltMenuKodu = sablon.Key.AltMenuKodu.Value;
                                //}
                                //else
                                //{
                                yetkiGrupYetki.AltMenuKodu = 0;
                                //}
                                //if (sablon.Key.SekmeKodu != null)
                                //{
                                //    yetkiGrupYetki.SekmeKodu = sablon.Key.SekmeKodu.Value;
                                //}
                                //else
                                //{
                                yetkiGrupYetki.SekmeKodu = 0;
                                //}

                                yetkiGrupYetki.Gorme = 1;
                                yetkiGrupYetki.YeniKayit = 1;
                                yetkiGrupYetki.Degistirme = 1;
                                yetkiGrupYetki.Silme = 1;
                                yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);
                                sayac--;
                            }

                        }
                    }
                    //foreach (var item in tumAltMenuler)
                    //{

                    //    TVMYetkiGrupYetkileri yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                    //    yetkiGrupYetki.YetkiGrupKodu = yetkigrubu.YetkiGrupKodu;
                    //    yetkiGrupYetki.AnaMenuKodu = sablon.AnaMenuKodu;
                    //    if (sablon.AltMenuKodu != null)
                    //    {
                    //        yetkiGrupYetki.AltMenuKodu = sablon.AltMenuKodu.Value;
                    //    }
                    //    else
                    //    {
                    //        yetkiGrupYetki.AltMenuKodu = 0;
                    //    }
                    //    if (sablon.SekmeKodu != null)
                    //    {
                    //        yetkiGrupYetki.SekmeKodu = sablon.SekmeKodu.Value;
                    //    }
                    //    else
                    //    {
                    //        yetkiGrupYetki.SekmeKodu = 0;
                    //    }
                    //    yetkiGrupYetki.Gorme = sablon.Gorme;
                    //    yetkiGrupYetki.YeniKayit = sablon.YeniKayit;
                    //    yetkiGrupYetki.Degistirme = sablon.Degistirme;
                    //    yetkiGrupYetki.Silme = sablon.Silme;
                    //    yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);

                    //}
                    //foreach (var item in tumSekmeler)
                    //{

                    //    TVMYetkiGrupYetkileri yetkiGrupYetki = new TVMYetkiGrupYetkileri();
                    //    yetkiGrupYetki.YetkiGrupKodu = yetkigrubu.YetkiGrupKodu;
                    //    yetkiGrupYetki.AnaMenuKodu = sablon.AnaMenuKodu;
                    //    if (sablon.AltMenuKodu != null)
                    //    {
                    //        yetkiGrupYetki.AltMenuKodu = sablon.AltMenuKodu.Value;
                    //    }
                    //    else
                    //    {
                    //        yetkiGrupYetki.AltMenuKodu = 0;
                    //    }
                    //    if (sablon.SekmeKodu != null)
                    //    {
                    //        yetkiGrupYetki.SekmeKodu = sablon.SekmeKodu.Value;
                    //    }
                    //    else
                    //    {
                    //        yetkiGrupYetki.SekmeKodu = 0;
                    //    }

                    //    yetkiGrupYetki.Gorme = sablon.Gorme;
                    //    yetkiGrupYetki.YeniKayit = sablon.YeniKayit;
                    //    yetkiGrupYetki.Degistirme = sablon.Degistirme;
                    //    yetkiGrupYetki.Silme = sablon.Silme;
                    //    yetkiGrupYetki = _YetkiService.CreateYetkiGrupYetkileri(yetkiGrupYetki);

                    //}
                    //}

                    YetkiGuncelle(model, yetkigrubu);

                    return RedirectToAction("Detay", "Yetki", new { Id = yetkigrubu.YetkiGrupKodu });
                }
                YetkiModelDoldurEkle(model);
                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

    }
}
