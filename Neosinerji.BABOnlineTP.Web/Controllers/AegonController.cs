using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Controllers
{
    [LifeLogin]
    public class AegonController : Controller
    {
        IMembershipService _MemberShipService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullaniciService;
        IKullaniciService _KullaniciService;
        ILogService _Log;
        IKonfigurasyonService _Konfig;

        public AegonController()
        {
            _MemberShipService = DependencyResolver.Current.GetService<IMembershipService>();
            _FormsAuthenticationService = DependencyResolver.Current.GetService<IFormsAuthenticationService>();
            _AktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _Konfig = DependencyResolver.Current.GetService<IKonfigurasyonService>();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModelAEGON model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool validated = _MemberShipService.ValidateUserAEGON(model.AcenteNo, model.Password);

                    if (validated)
                    {
                        _AktifKullaniciService.SetUserAEGON(model.AcenteNo);
                        _FormsAuthenticationService.SignIn(model.AcenteNo, false);

                        if (String.IsNullOrEmpty(returnUrl))
                        {
                            return RedirectToAction("Index", "TVM", new { area = "TVM" });
                        }
                        else
                        {
                            return RedirectToLocal(returnUrl);
                        }
                    }
                }

                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }
            catch (AccountPasiveException)
            {
                ModelState.AddModelError("", "Kullanıcı pasif yada dondurulmuş.");
            }
            catch (AccountLockedException)
            {
                ModelState.AddModelError("", babonline.Message_LockedUser);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }

            model.Password = "";

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Login2(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            string loginUrl = "https://sso.aegon.com.tr/ssoaegon/giris.php";

            try
            {
                loginUrl = _Konfig.GetKonfigDeger(Konfig.AEGON_SSO_LOGIN_Login);
            }
            catch (Exception)
            { }

            return Redirect(loginUrl);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login2(LoginModel_SSO_AEGON model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool validated = _MemberShipService.ValidateUserAEGON_SSO(model.USER_NAME, model.SESSION_TOKEN, model.CHECKSUM);

                    if (validated)
                    {
                        _AktifKullaniciService.SetUserAEGON_SSO(model.USER_NAME, model.SESSION_TOKEN);
                        _FormsAuthenticationService.SignIn(model.USER_NAME, false);

                        return RedirectToAction("Index", "TVM", new { area = "TVM" });
                    }
                }

                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }
            catch (AccountPasiveException)
            {
                ModelState.AddModelError("", "Kullanıcı pasif yada dondurulmuş.");
            }
            catch (AccountLockedException)
            {
                ModelState.AddModelError("", babonline.Message_LockedUser);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }

            string loginUrl = "https://sso.aegon.com.tr/ssoaegon/giris.php";

            try
            {
                loginUrl = _Konfig.GetKonfigDeger(Konfig.AEGON_SSO_LOGIN_Login);
            }
            catch (Exception)
            { }

            return Redirect(loginUrl);
        }

        [AllowAnonymous]
        public ActionResult SifremiUnuttum()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SifremiUnuttum(AegonSifremiUnuttum model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool sonuc = false;
                    TVMKullanicilar kullanici = _KullaniciService.GetKullaniciByPartajNO(model.PartajNo);

                    if (kullanici != null)
                    {
                        TVMKullaniciSifremiUnuttum sifremiUnuttum = kullanici.TVMKullaniciSifremiUnuttums.OrderByDescending(s => s.Id).FirstOrDefault();

                        if (sifremiUnuttum != null)
                        {
                            switch (sifremiUnuttum.Status)
                            {
                                case KullaniciSifremiUnuttumTipleri.LinkGonderildi:
                                    if (sifremiUnuttum.SendDate > TurkeyDateTime.Now.AddMinutes(-20))
                                    {
                                        ModelState.AddModelError("", babonline.RecoveryPassword_Success_AlreadySendLink);
                                        return View();
                                    }
                                    else
                                    {
                                        sonuc = _KullaniciService.RecoverPassword(kullanici);
                                        ViewBag.Message = babonline.RecoveryPassword_Success_SendLink;
                                        return View();
                                    }
                                case KullaniciSifremiUnuttumTipleri.SifreResetlendi:
                                    sonuc = _KullaniciService.RecoverPassword(kullanici);
                                    ViewBag.Message = babonline.RecoveryPassword_Success_SendLink;
                                    return View();
                            }
                        }
                        else
                        {
                            sonuc = _KullaniciService.RecoverPassword(kullanici);
                            if (sonuc)
                            {
                                ViewBag.Message = babonline.RecoveryPassword_Success_SendLink;

                                foreach (var item in ModelState.Values)
                                {
                                    item.Errors.Clear();
                                }

                                return View();
                            }
                            else
                            {
                                ModelState.AddModelError("", "Bu partaj numarası bir hesap ile ilişkili değil");
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Bu partaj numarası bir hesap ile ilişkili değil");
                        return View();
                    }
                }

                ModelState.AddModelError("", babonline.Message_RequiredValues);
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [AllowAnonymous]
        public ActionResult SifreSifirla(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                bool sonuc = _KullaniciService.ResetPassword(id);
                if (sonuc)
                {
                    ViewBag.Basarili = "Şifreniz başarıyla sıfırlanmıştır.Birkaç dakika içerisinde yeni şifrenizi içeren bir mail alacaksınız. Lütfen bekleyiniz giriş sayfasına Yönlendiriliyorsunuz ";
                    ViewBag.Yonlendir = 1;
                    return View();
                }
            }
            ViewBag.Basarili = "Hatalı Anahtar.";
            ViewBag.Yonlendir = 0;
            return View();
        }

        [Authorization]
        public ActionResult LogOff()
        {
            string url = "https://sso.aegon.com.tr/ssoaegon/giris.php";

            try
            {
                string appId = _Konfig.GetKonfigDeger(Konfig.AEGON_SSO_LOGIN_AppId);
                string logOffUrl = _Konfig.GetKonfigDeger(Konfig.AEGON_SSO_LOGIN_LogOff);
                string sesion = _AktifKullaniciService.AegonSession;

                url = String.Format(logOffUrl, sesion, appId);
            }
            catch (Exception)
            { }

            _AktifKullaniciService.Logout();
            _FormsAuthenticationService.SignOut();

            return Redirect(url);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
