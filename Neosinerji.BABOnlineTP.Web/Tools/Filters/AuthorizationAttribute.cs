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
using System.Web.SessionState;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;

namespace Neosinerji.BABOnlineTP.Web.Tools
{
    /// <summary>
    /// Kullanıcı giriş yapmadı ise kullanıcıyı /Account/Login sayfasına yönlendirir
    /// </summary>
    public class AuthorizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            IAktifKullaniciService aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            ITVMService tvmService = DependencyResolver.Current.GetService<ITVMService>();
            IKullaniciService _kullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();

            if (aktif.IsAuthenticated)
            {
                //if (!aktif.SifreTarihi.HasValue && aktif.ProjeKodu != TVMProjeKodlari.Aegon)
                if (!aktif.SifreTarihi.HasValue)
                {
                    filterContext.Result = new RedirectResult("~/TVM/Profil/SifreDegistir");
                    return;
                }

                Controller controller = filterContext.Controller as Controller;

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

                #region AEGON

                if (aktif.ProjeKodu == TVMProjeKodlari.Aegon)
                {
                    KullaniciYetkiModel teklifRaporu = altMenuler.Where(s => s.AnaMenu == AnaMenuler.Rapor && s.MenuKodu == AltMenuler.OnlineRaporlar).FirstOrDefault();
                    if (teklifRaporu != null)
                    {
                        teklifRaporu.URL = "/Rapor/AegonRapor/TeklifRaporu";
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
                controller.ViewBag.SifreSuresiMessage = _kullaniciService.KullaniciSifreSuresiKontrol(aktif.TVMKodu, aktif.KullaniciKodu);
                controller.ViewBag.BagliOlduguTvmKodu = aktif.TVMKodu;
                //var tvmdetay = tvmService.GetDetay(aktif.TVMKodu);
                //if (tvmdetay!=null)
                //{
                //    if (tvmdetay.Tipi==TVMTipleri.Internet)
                //    {
                //        controller.ViewBag.InternetMenu = true;
                //    } 
                //}

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
                                                                            s.UrunKodu == UrunKodlari.Lilyum ||
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

                return;
            }


            #region Yönlendirme

            string projeKodu = aktif.ProjeKodu;

            if (String.IsNullOrEmpty(projeKodu))
            {
                if (HttpContext.Current != null && HttpContext.Current.Request.Cookies["proc_kod"] != null)
                {
                    projeKodu = HttpContext.Current.Request.Cookies["proc_kod"].Value;
                    //string sayfaAdresi = HttpContext.Current.Request.FilePath;
                    //var parts = sayfaAdresi.Split('/');
                    //var acenteKodu =Convert.ToInt32(parts[4]);
                    //var tvmDetay=tvmService.GetDetay(acenteKodu);
                    //if (tvmDetay!=null)
                    //{
                    //    if (tvmDetay.Tipi == TVMTipleri.Internet)
                    //    {
                    //        var internetKullanici = _kullaniciService.GetListTVMKullanicilari(tvmDetay.Kodu);
                    //        filterContext.Result = new RedirectResult("~/Teklif/LilyumKart/Ekle/");
                    //    }
                    //}
                }
            }
            switch (projeKodu)
            {
                case TVMProjeKodlari.Aegon: filterContext.Result = new RedirectResult("~/Aegon/Login"); break;
                case TVMProjeKodlari.Mapfre: filterContext.Result = new RedirectResult("~/Mapfre/Login"); break;
                case TVMProjeKodlari.Lilyum: filterContext.Result = new RedirectResult("~/Account/LilyumKart"); break;
                default: filterContext.Result = new RedirectResult("~/Account/Login"); break;
            }

            #endregion
        }



        public string IslemId { get; set; }
        public int AnaMenuKodu { get; set; }
        public int AltMenuKodu { get; set; }
        public int SekmeKodu { get; set; }
        public int UrunKodu { get; set; }
        public int menuPermission { get; set; }
        public int menuPermissionType { get; set; }
    }
    public class PolicelestirmeYetkiAttribute : FilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            if (!_AktifKullanici.Policelestirme)
            {
                var model = new TeklifOdemeCevapModel();
                model.Success = false;
                model.Hatalar = new string[1] { "Poliçeleştirme Yetkiniz Bulunmamaktadır. Lüften Yetkili Kullanıcı ile İletişime Geçiniz." };
                filterContext.Controller.ViewBag.PolicelestirmeKontrol = model;
            }
        }
        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }

}