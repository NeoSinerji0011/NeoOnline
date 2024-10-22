using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Web.Tools
{
    /// <summary>
    /// Kullanıcı giriş yapmadı ise kullanıcıyı /Account/Login sayfasına yönlendirir
    /// </summary>
    /// 
    public class TestAuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
#if !DEBUG
            throw new Exception("TestAuthorization release versiyonunda kullanılamaz.");
#else
            IAktifKullaniciService aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            IFormsAuthenticationService forms = DependencyResolver.Current.GetService<IFormsAuthenticationService>();
            ITVMService tvmService = DependencyResolver.Current.GetService<ITVMService>();
            IKullaniciService _kullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();

            Controller controller = filterContext.Controller as Controller;

            if (!aktif.IsAuthenticated)
            {
                aktif.SetUserMAPFRE("A1100001", "", "110000");
                forms.SignIn("A1100001", false);
            }

            // ==== Urun yetkileri url bilgisi ekleniyor==== //
            List<TVMUrunYetkileriProcedureModel> Urunyetkileri = aktif.UrunYetkileri;
            foreach (var item in Urunyetkileri)
            {
                item.Active = false;
                item.UrunURL = TeklifSayfaAdresleri.EkleAdres(item.UrunKodu);
            }
            // ==== Kullanıcının tvm sinin logosu getiriliyor ==== //
            string logoUrl = tvmService.GetDetay(aktif.TVMKodu).Logo;

            #region Menü Ve Yetkiler

            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllername = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;


            List<KullaniciYetkiModel> anaMenuler = aktif.Yetkiler.Where(s => s.AnaMenu == 0).ToList<KullaniciYetkiModel>();
            List<KullaniciYetkiModel> altMenuler = aktif.Yetkiler.Where(s => s.AnaMenu != 0 && s.SekmeKodu == 0).ToList<KullaniciYetkiModel>();
            List<KullaniciYetkiModel> sekmeler = aktif.Yetkiler.Where(s => s.SekmeKodu != 0 && s.AnaMenu != 0).ToList<KullaniciYetkiModel>();

            //Sayfa yeri bilgisi temizleniyor tekrar set edilecek
            foreach (var item in aktif.Yetkiler) { item.Active = false; }

            //Alt Menülerin sekme kontrolu yapılıyor
            foreach (var item in altMenuler) item.HasChild = false;
            foreach (var item in altMenuler) if (sekmeler.Where(s => s.MenuKodu == item.MenuKodu).Count() > 0) item.HasChild = true;


            //string areaName = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"].ToString();

            int sayac = 0;

            //ANA menü yetki kontrolu
            if (this.AnaMenuKodu != 0)
            {
                foreach (var item in anaMenuler)
                {
                    if (item.MenuKodu == this.AnaMenuKodu)
                    {
                        sayac++;
                        item.Active = true;
                    }
                }
                if (sayac == 0)
                {
                    filterContext.Result = new RedirectResult("/Error/ErrorPage/403");
                    return;
                }
            }
            sayac = 0;

            //Alt Menü kontrolu
            if (this.AltMenuKodu != 0)
            {

                foreach (var item in altMenuler)
                {
                    if (item.AnaMenu == this.AnaMenuKodu && item.MenuKodu == this.AltMenuKodu)
                    {
                        sayac++;
                        item.Active = true;
                    }
                }
                if (sayac == 0)
            {
                    filterContext.Result = new RedirectResult("/Error/ErrorPage/403");
                    return;
                }
            }

            sayac = 0;

            //Sekme Kontrolu
            if (this.SekmeKodu != 0)
            {
                foreach (var item in sekmeler)
                {
                    if (item.AnaMenu == this.AnaMenuKodu && item.MenuKodu == this.AltMenuKodu && item.SekmeKodu == this.SekmeKodu)
                    {
                        sayac++;
                        item.Active = true;
                    }
                }
                if (sayac == 0)
                {
                    filterContext.Result = new RedirectResult("/Error/ErrorPage/403");
                    return;
                }
            }

                controller.ViewBag.IslemId = this.IslemId;
                controller.ViewBag.Kullanici = aktif;
                controller.ViewBag.Menuler = aktif.Yetkiler.Where(w => w.AnaMenu == this.AnaMenuKodu);

            sayac = 0;
            if (this.UrunKodu != 0)
            {
                foreach (var item in Urunyetkileri)
                {
                    if (item.UrunKodu == this.UrunKodu)
                    {
                        sayac++;
                        item.Active = true;
            }
                }
                if (sayac == 0)
            {
                    filterContext.Result = new RedirectResult("/Error/ErrorPage/403");
                    return;
                }
            }

            sayac = 0;
            if (this.menuPermission > 0 && this.menuPermissionType > 0)
            {
                switch (this.menuPermissionType)
                {
                    case MenuPermissionType.AnaMenu: break;
                    case MenuPermissionType.AltMenu:
                        foreach (var item in altMenuler)
                        {
                            if (item.AnaMenu == this.AnaMenuKodu && item.MenuKodu == this.AltMenuKodu)
                            {
                                switch (this.menuPermission)
                                {
                                    case MenuPermission.Gorme: if (item.Gorme == 1) sayac++; break;
                                    case MenuPermission.Ekleme: if (item.YeniKayit == 1) sayac++; break;
                                    case MenuPermission.Guncelleme: if (item.Degistirme == 1) sayac++; break;
                                    case MenuPermission.Silme: if (item.Silme == 1) sayac++; break;
                                }
                            }
                        }
                        break;
                    case MenuPermissionType.ALtMenuSekme:
                        foreach (var item in sekmeler)
                        {
                            if (item.AnaMenu == this.AnaMenuKodu && item.SekmeKodu == this.SekmeKodu && item.MenuKodu == this.AltMenuKodu)
                            {
                                switch (this.menuPermission)
                                {
                                    case MenuPermission.Gorme: if (item.Gorme == 1) sayac++; break;
                                    case MenuPermission.Ekleme: if (item.YeniKayit == 1) sayac++; break;
                                    case MenuPermission.Guncelleme: if (item.Degistirme == 1) sayac++; break;
                                    case MenuPermission.Silme: if (item.Silme == 1) sayac++; break;
                                }
                            }
                        }
                        break;
                }

                if (sayac == 0)
                {
                    filterContext.Result = new RedirectResult("/Error/ErrorPage/403");
                    return;
                }
            }

            #endregion

            controller.ViewBag.IslemId = this.IslemId;
            controller.ViewBag.Kullanici = aktif;
            controller.ViewBag.AnaMenuler = anaMenuler;
            controller.ViewBag.AltMenuler = altMenuler;
            controller.ViewBag.Sekmeler = sekmeler;
            controller.ViewBag.SonGirisTarihi = aktif.SonGirisTarihi;
            controller.ViewBag.Logo = logoUrl;
            controller.ViewBag.UrunYetkileri = Urunyetkileri;
            controller.ViewBag.TvmMuhasebeEntg = aktif.MuhasebeEntg ? "1" : "";
            controller.ViewBag.AcenteKullanicilari = _kullaniciService.GetListAcenteKullanicilari();
            controller.ViewBag.ProjeKodu = aktif.ProjeKodu;

            #region Urun Tipleri Ayrılıyor


            controller.ViewBag.VarlikSigortalari = Urunyetkileri.Where(s => s.UrunKodu == UrunKodlari.DogalAfetSigortasi_Deprem ||
                                                                            s.UrunKodu == UrunKodlari.KonutSigortasi_Paket ||
                                                                            s.UrunKodu == UrunKodlari.IsYeri).ToList<TVMUrunYetkileriProcedureModel>();


            controller.ViewBag.BireyselGuvence = Urunyetkileri.Where(s => s.UrunKodu == UrunKodlari.TrafikSigortasi ||
                                                                        s.UrunKodu == UrunKodlari.KaskoSigortasi ||
                                                                        s.UrunKodu == UrunKodlari.FerdiKazaSigortasi ||
                                                                        s.UrunKodu == UrunKodlari.KrediHayat ||
                                                                        s.UrunKodu == UrunKodlari.IkinciElGaranti ||
                                                                        s.UrunKodu == UrunKodlari.MapfreKasko ||
                                                                        s.UrunKodu == UrunKodlari.MapfreTrafik).ToList<TVMUrunYetkileriProcedureModel>();

            controller.ViewBag.CanSigortalari = Urunyetkileri.Where(s => s.UrunKodu == UrunKodlari.YurtDisiSeyehatSaglik ||
                                                                            s.UrunKodu == UrunKodlari.AcilSaglik ||
                                                                            s.UrunKodu == UrunKodlari.TamamlayiciSaglik).ToList<TVMUrunYetkileriProcedureModel>();

            //AEGON
            controller.ViewBag.BirikimliSigortalari = Urunyetkileri.Where(s => s.UrunKodu == UrunKodlari.OdulluBirikim ||
                                                                                s.UrunKodu == UrunKodlari.Egitim).ToList<TVMUrunYetkileriProcedureModel>();

            controller.ViewBag.RiskSigortalari = Urunyetkileri.Where(s => s.UrunKodu == UrunKodlari.TESabitPrimli ||
                                                                            s.UrunKodu == UrunKodlari.PrimIadeli ||
                                                                            s.UrunKodu == UrunKodlari.OdemeGuvence ||
                                                                            s.UrunKodu == UrunKodlari.KorunanGelecek ||
                                                                            s.UrunKodu == UrunKodlari.PrimIadeli2
                                                                        ).ToList<TVMUrunYetkileriProcedureModel>();



                #endregion           
#endif
            //filterContext.Result = new RedirectResult("~/Account/Login");
        }

        public string IslemId { get; set; }
        public int AnaMenuKodu { get; set; }
        public int AltMenuKodu { get; set; }
        public int SekmeKodu { get; set; }
        public int UrunKodu { get; set; }
        public int menuPermission { get; set; }
        public int menuPermissionType { get; set; }
    }
}
