using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Controllers
{
    public class MapfreController : Controller
    {
        IMembershipService _MemberShipService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullaniciService;
        IKullaniciService _KullaniciService;
        ILogService _Log;

        public MapfreController()
        {
            _MemberShipService = DependencyResolver.Current.GetService<IMembershipService>(); ;
            _FormsAuthenticationService = DependencyResolver.Current.GetService<IFormsAuthenticationService>(); ;
            _AktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>(); ;
            _KullaniciService = DependencyResolver.Current.GetService<IKullaniciService>(); ;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return Redirect("https://www.babonline.com");
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModelMAPFRE model, string returnUrl)
        {
            return Redirect("https://www.babonline.com");

            try
            {
                if (ModelState.IsValid)
                {
                    //bool validated = _MemberShipService.ValidateUserMAPFRE(model.PartajNo, model.Password);
                    //string partajNo = _KullaniciService.GetMapfreKullanici(model.PartajNo).MTKodu;

                    IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                    string partajNo = String.Empty;
                    bool validated;

                    if (model.PartajNo == "babuser")
                    {
                        validated = _MemberShipService.ValidateUser(model.PartajNo, model.Password);
                        partajNo = "";
                    }
                    else
                    {
                        validated = mapfreSorgu.ValidateUserWithIP(model.PartajNo, model.Password, out partajNo);
                    }

                    if (validated)
                    {
                        _AktifKullaniciService.SetUserMAPFRE(model.PartajNo, model.Password, partajNo);
                        _FormsAuthenticationService.SignIn(model.PartajNo, false);

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
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }
            catch (AccountLockedException)
            {
                ModelState.AddModelError("", babonline.Message_LockedUser);
            }
            catch (MapfreValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }

            model.Password = String.Empty;
            return View(model);
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
        public ActionResult SifremiUnuttum(MapfreRecoverPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool sonuc = false;
                    TVMKullanicilar kullanici = _KullaniciService.GetMapfreKullanici(model.KullaniciKodu);

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
                                ModelState.AddModelError("", babonline.Not_Found_MapfreUserCode);
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Not_Found_MapfreUserCode);
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

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login", "Mapfre");
            }
        }
        #endregion

    }
}
