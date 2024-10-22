using System;
using System.Linq;
using System.Web;
using AutoMapper;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.SistemYonetimi, SekmeKodu = AltMenuSekmeler.MenuYonetimi)]
    [OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
    public class MenuController : Controller
    {
        IMenuService _MenuService;
        IAktifKullaniciService _AktifKullanici;
        ILogService _Log;

        public MenuController(IMenuService menuService, IAktifKullaniciService aktifKullanici)
        {
            _MenuService = menuService;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Liste()
        {
            try
            {
                MenulerModel model = new MenulerModel();

                List<AnaMenu> AnaMenuler = _MenuService.GetAnaMenuListYetkili();
                Mapper.CreateMap<AnaMenu, AnaMenuModel>();
                model.AnaMenuler = Mapper.Map<List<AnaMenu>, List<AnaMenuModel>>(AnaMenuler);


                List<AltMenu> AltMenuler = _MenuService.GetAltMenuListYetkili();
                Mapper.CreateMap<AltMenu, AltMenuModel>();
                model.AltMenuler = Mapper.Map<List<AltMenu>, List<AltMenuModel>>(AltMenuler);


                List<AltMenuSekme> AltMenuSekmeler = _MenuService.GetALtMenuSekmeListYetkili();
                Mapper.CreateMap<AltMenuSekme, AltMenuSekmeModel>();
                model.AltMenuSekmeler = Mapper.Map<List<AltMenuSekme>, List<AltMenuSekmeModel>>(AltMenuSekmeler);

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult AnaEkran()
        {
            try
            {
                MenulerModel model = new MenulerModel();

                List<AnaMenu> AnaMenuler = _MenuService.GetAnaMenuListYetkili();
                Mapper.CreateMap<AnaMenu, AnaMenuModel>();
                model.AnaMenuler = Mapper.Map<List<AnaMenu>, List<AnaMenuModel>>(AnaMenuler);

                List<AltMenu> AltMenuler = _MenuService.GetAltMenuListYetkili();
                Mapper.CreateMap<AltMenu, AltMenuModel>();
                model.AltMenuler = Mapper.Map<List<AltMenu>, List<AltMenuModel>>(AltMenuler);

                List<AltMenuSekme> AltMenuSekmeler = _MenuService.GetALtMenuSekmeListYetkili();
                Mapper.CreateMap<AltMenuSekme, AltMenuSekmeModel>();
                model.AltMenuSekmeler = Mapper.Map<List<AltMenuSekme>, List<AltMenuSekmeModel>>(AltMenuSekmeler);

                return PartialView("_AnaEkran", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        #region AnaMenu İşlemleri

        public ActionResult AnaMenuDetay(int AnaMenuKodu)
        {
            try
            {
                AnaMenu anamenu = _MenuService.GetAnaMenu(AnaMenuKodu);
                Mapper.CreateMap<AnaMenu, AnaMenuModel>();
                AnaMenuModel model = Mapper.Map<AnaMenu, AnaMenuModel>(anamenu);

                model.URL = _MenuService.GetMenuIslem(anamenu.IslemKodu).URL;
                return PartialView("_AnaMenuDetay", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult AnaMenuGuncelle(int AnaMenuKodu)
        {
            try
            {
                AnaMenu anamenu = _MenuService.GetAnaMenu(AnaMenuKodu);
                Mapper.CreateMap<AnaMenu, AnaMenuModel>();
                AnaMenuModel model = Mapper.Map<AnaMenu, AnaMenuModel>(anamenu);

                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);

                return PartialView("_AnaMenuGuncelle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        public ActionResult AnaMenuGuncelle(AnaMenuModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AnaMenu anamenu = _MenuService.GetAnaMenu(model.AnaMenuKodu.Value);
                    anamenu.Aciklama = model.Aciklama;
                    anamenu.YardimAciklama = model.YardimAciklama;
                    anamenu.Durum = model.Durum;
                    anamenu.IslemKodu = model.IslemKodu.Value;

                    _MenuService.UpdateAnaMenu(anamenu);
                    return null;
                }

                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                return PartialView("_AnaMenuGuncelle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public ActionResult AnaMenuEkle()
        {
            try
            {
                AnaMenuModel model = new AnaMenuModel();

                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                return PartialView("_AnaMenuEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        public ActionResult AnaMenuEkle(AnaMenuModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AnaMenu anaMenu = new AnaMenu();

                    anaMenu.Aciklama = model.Aciklama;
                    anaMenu.YardimAciklama = model.YardimAciklama;
                    anaMenu.IslemKodu = model.IslemKodu.Value;
                    anaMenu.Durum = model.Durum;

                    _MenuService.CreateAnaMenu(anaMenu);
                    return null;
                }

                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                return PartialView("_AnaMenuEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        public void AnaMenuSil(int AnaMenuKodu)
        {
            try
            {
                _MenuService.DeleteAnaMenu(AnaMenuKodu);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
        }

        public bool AnaMenuSilOnay(int AnaMenuKodu)
        {
            try
            {
                return _MenuService.CheckAltMenu(AnaMenuKodu);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return false;
            }
        }

        #endregion

        #region AltMenu İşlemleri
        public ActionResult AltMenuEkle(int AnaMenuKodu)
        {
            try
            {
                AltMenuModel model = new AltMenuModel();
                model.AnaMenuKodu = AnaMenuKodu;
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                return PartialView("_AltMenuEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        public ActionResult AltMenuEkle(AltMenuModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AltMenu altMenu = new AltMenu();

                    altMenu.AnaMenuKodu = model.AnaMenuKodu.Value;
                    altMenu.IslemKodu = model.IslemKodu.Value;
                    altMenu.Aciklama = model.Aciklama;
                    altMenu.YardimAciklama = model.YardimAciklama;
                    altMenu.Durum = model.Durum;

                    _MenuService.CreateAltMenu(altMenu);
                    return null;
                }

                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                return PartialView("_AltMenuEkle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        public ActionResult AltMenuDetay(int AltMenuKodu, int AnaMenuKodu)
        {
            try
            {
                AltMenu altmenu = _MenuService.GetAltMenu(AltMenuKodu, AnaMenuKodu);
                Mapper.CreateMap<AltMenu, AltMenuModel>();
                AltMenuModel model = Mapper.Map<AltMenu, AltMenuModel>(altmenu);
                model.URL = _MenuService.GetMenuIslem(altmenu.IslemKodu).URL;

                return PartialView("_AltMenuDetay", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        public ActionResult AltMenuGuncelle(int AltMenuKodu, int AnaMenuKodu)
        {
            try
            {
                AltMenu altmenu = _MenuService.GetAltMenu(AltMenuKodu, AnaMenuKodu);
                Mapper.CreateMap<AltMenu, AltMenuModel>();
                AltMenuModel model = Mapper.Map<AltMenu, AltMenuModel>(altmenu);
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                return PartialView("_AltMenuGuncelle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        [HttpPost]
        public ActionResult AltMenuGuncelle(AltMenuModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    AltMenu altmenu = _MenuService.GetAltMenu(model.AltMenuKodu, model.AnaMenuKodu.Value);
                    altmenu.Aciklama = model.Aciklama;
                    altmenu.YardimAciklama = model.YardimAciklama;
                    altmenu.Durum = model.Durum;
                    altmenu.IslemKodu = model.IslemKodu.Value;

                    _MenuService.UpdateAltMenu(altmenu);
                    return null;
                }

                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
                return PartialView("_AltMenuGuncelle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        public void AltMenuSil(int AnaMenuKodu, int AltMenuKodu)
        {
            try
            {
                _MenuService.DeleteAltMenu(AnaMenuKodu, AltMenuKodu);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }
        }

        public bool AltMenuSilOnay(int AltMenuKodu)
        {
            try
            {
                return _MenuService.CheckAltMenuSekme(AltMenuKodu);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return false;
            }
        }

        #endregion

        #region AltMenuSekmeEkleme işlemleri
        public ActionResult AltMenuSekmeEkle(int AnaMenuKodu, int AltMenuKodu)
        {
            try
            {
                AltMenuSekmeModel model = new AltMenuSekmeModel();

                model.AnaMenuKodu = AnaMenuKodu;
                model.AltMenuKodu = AltMenuKodu;
                model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
                model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();

                return PartialView("_AltMenuSekme-Ekle", model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return null;
            }
        }

        [HttpPost]
        public ActionResult AltMenuSekmeEkle(AltMenuSekmeModel model)
        {
            if (ModelState.IsValid)
            {
                AltMenuSekme altmenusekme = new AltMenuSekme();

                altmenusekme.AnaMenuKodu = model.AnaMenuKodu.Value;
                altmenusekme.AltMenuKodu = model.AltMenuKodu;
                altmenusekme.SiraNumarasi = 1;
                altmenusekme.SekmeKodu = 1;


                altmenusekme.IslemKodu = model.IslemKodu.Value;
                altmenusekme.Aciklama = model.Aciklama;
                altmenusekme.YardimAciklama = model.YardimAciklama;
                altmenusekme.Durum = model.Durum;

                _MenuService.CreateALtMenuSekme(altmenusekme);
                return null;
            }

            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();

            return PartialView("_AltMenuSekme-Ekle", model);
        }

        public ActionResult AltMenuSekmeDetay(int SekmeKodu, int altMenuKodu)
        {
            AltMenuSekme altMenuSekme = _MenuService.GetAltMenuSekme(altMenuKodu, SekmeKodu);
            Mapper.CreateMap<AltMenuSekme, AltMenuSekmeModel>();
            AltMenuSekmeModel model = Mapper.Map<AltMenuSekme, AltMenuSekmeModel>(altMenuSekme);

            model.URL = _MenuService.GetMenuIslem(altMenuSekme.IslemKodu).URL;

            return PartialView("_AltMenuSekme-Detay", model);
        }

        public ActionResult AltMenuSekmeGuncelle(int altMenuKodu, int SekmeKodu)
        {
            AltMenuSekme altMenuSekme = _MenuService.GetAltMenuSekme(altMenuKodu, SekmeKodu);
            Mapper.CreateMap<AltMenuSekme, AltMenuSekmeModel>();
            AltMenuSekmeModel model = Mapper.Map<AltMenuSekme, AltMenuSekmeModel>(altMenuSekme);
            model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            return PartialView("_AltMenuSekme-Guncelle", model);
        }

        [HttpPost]
        public ActionResult AltMenuSekmeGuncelle(AltMenuSekmeModel model)
        {
            if (ModelState.IsValid)
            {
                AltMenuSekme altMenuSekme = _MenuService.GetAltMenuSekme(model.AltMenuKodu, model.SekmeKodu);

                altMenuSekme.Aciklama = model.Aciklama;
                altMenuSekme.YardimAciklama = model.YardimAciklama;
                altMenuSekme.Durum = model.Durum;
                altMenuSekme.IslemKodu = model.IslemKodu.Value;

                _MenuService.UpdateAltMenuSekme(altMenuSekme);
                return null;
            }
            model.MenuIslemleri = new SelectList(MenuIslemYukle(), "IslemKodu", "URL").ListWithOptionLabel();
            model.Durumlar = new SelectList(DurumListesiAktifPasif.DurumTipleri(), "Value", "Text", model.Durum);
            return PartialView("_AltMenuSekme-Guncelle", model);
        }

        public void AltMenuSekmeSil(int SekmeKodu)
        {
            _MenuService.DeleteAltMenuSekme(SekmeKodu);
        }

        #endregion

        #region MenuIslem İşlemleri

        public ActionResult MenuIslemListe()
        {
            MenuIslemListModel model = new MenuIslemListModel();
            List<MenuIslem> menuIslems = _MenuService.GetListMenuIslem();
            Mapper.CreateMap<MenuIslem, MenuIslemModel>();
            model.Items = Mapper.Map<List<MenuIslem>, List<MenuIslemModel>>(menuIslems);

            return View(model);
        }

        public ActionResult MenuIslemDetay(int Id)
        {
            MenuIslem menuislem = _MenuService.GetMenuIslem(Id);
            Mapper.CreateMap<MenuIslem, MenuIslemModel>();
            MenuIslemModel model = Mapper.Map<MenuIslem, MenuIslemModel>(menuislem);

            return View(model);
        }

        public ActionResult MenuIslemEkle()
        {
            MenuIslemModel model = new MenuIslemModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult MenuIslemEkle(MenuIslemModel model)
        {
            if (ModelState.IsValid)
            {
                MenuIslem menuislem = new MenuIslem();
                menuislem.IslemKodu = 1;
                //Menu işlem kodu ilk kayıt için 1 sonraki kayıtlar için bir önceki kaydın 1 fazlası
                if (_MenuService.GetListMenuIslem().ToList<MenuIslem>().Count > 0)
                    menuislem.IslemKodu = _MenuService.GetListMenuIslem().Select(s => s.IslemKodu).Max() + 1;

                menuislem.URL = model.URL;
                menuislem.Icon = model.Icon;
                menuislem.IslemId = model.IslemId;

                menuislem = _MenuService.CreateMenuIslem(menuislem);
                return RedirectToAction("MenuIslemDetay", "Menu", new { Id = menuislem.IslemKodu });
            }
            return View(model);
        }

        public ActionResult MenuIslemGuncelle(int Id)
        {
            MenuIslem menuislem = _MenuService.GetMenuIslem(Id);
            Mapper.CreateMap<MenuIslem, MenuIslemModel>();
            MenuIslemModel model = Mapper.Map<MenuIslem, MenuIslemModel>(menuislem);

            return View(model);
        }

        [HttpPost]
        public ActionResult MenuIslemGuncelle(MenuIslemModel model)
        {
            if (ModelState.IsValid)
            {
                MenuIslem menuislem = _MenuService.GetMenuIslem(model.IslemKodu);

                menuislem.URL = model.URL;
                menuislem.Icon = model.Icon;
                menuislem.IslemId = model.IslemId;

                _MenuService.UpdateMenuIslem(menuislem);
                return RedirectToAction("MenuIslemDetay", "Menu", new { Id = menuislem.IslemKodu });
            }
            return View();
        }

        #endregion

        public ActionResult ListePagerAltMenu(int id)
        {
            if (Request["sEcho"] != null)
            {
                DataTableParameters<AltMenu> altMenuList = new DataTableParameters<AltMenu>(Request, new Expression<Func<AltMenu, object>>[]
                                                                                        {
                                                                                            t => t.Aciklama,
                                                                                            t => t.YardimAciklama,
                                                                                            t => t.Durum
                                                                                        });

                Neosinerji.BABOnlineTP.Business.DataTableList result = _MenuService.PagedListAltMenuList(altMenuList, id);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        private List<MenuIslemModel> MenuIslemYukle()
        {
            List<MenuIslem> menuislemler = _MenuService.GetListMenuIslem();
            Mapper.CreateMap<MenuIslem, MenuIslemModel>();
            MenuIslemListModel menuislemmodel = new MenuIslemListModel();
            menuislemmodel.Items = Mapper.Map<List<MenuIslem>, List<MenuIslemModel>>(menuislemler);
            return menuislemmodel.Items;
        }

    }
}
