using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Tools;
using reCAPTCHA.MVC;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Neosinerji.BABOnlineTP.Business.aksigorta.kasko;
using Neosinerji.BABOnlineTP.Business.aksigorta.basim;
using Neosinerji.BABOnlineTP.Business.Pdf;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;

namespace Neosinerji.BABOnlineTP.Web.Controllers
{
    public abstract class AES
    {
        public static String doEncryptAES(String plainText, String key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, getRijndaelManaged(key)));
        }

        public static String doDecryptAES(String encryptedText, String key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, getRijndaelManaged(key)));
        }

        private static RijndaelManaged getRijndaelManaged(String secretKey)
        {
            byte[] hashBytes = Encoding.UTF8.GetBytes(secretKey);
            SHA1 sha1 = SHA1Managed.Create();
            byte[] cryptPassword = sha1.ComputeHash(hashBytes);
            //byte[] test1 = new byte[16];
            //for (int i = 0; i < cryptPassword.Length - 4; i++)
            //{
            //    test1[i] = cryptPassword[i];
            //}

            var keyBytes = new byte[16];
            //var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(cryptPassword, keyBytes, 16);
            return new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

    }

    [Authorize]
    public class AccountController : Controller
    {
        IMembershipService _MemberShipService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullaniciService;
        IKullaniciService _KullaniciService;
        ILogService _Log;
        ITVMService _TVMService;



        public AccountController(IMembershipService membershipService,
                                 IFormsAuthenticationService formsAuthenticationService,
                                 IAktifKullaniciService aktifkullaniciService,
                                         ITVMService tvmService,
                                 IKullaniciService kullanici)
        {
            _MemberShipService = membershipService;
            _FormsAuthenticationService = formsAuthenticationService;
            _AktifKullaniciService = aktifkullaniciService;
            _KullaniciService = kullanici;
            _TVMService = tvmService;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

       
        [AllowAnonymous]
        public ActionResult TimeOut()
        {
            return View();
        }
         
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        { 
            ViewBag.ReturnUrl = returnUrl;
            return View(); 
        }
         
        [HttpPost]
        [AllowAnonymous]
        //[CaptchaValidator]
        public ActionResult Login(LoginModel model, string ReturnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool validated = _MemberShipService.ValidateUser(model.Email, model.Password);

                    if (validated)
                    {
                        _AktifKullaniciService.SetUser(model.Email);
                        _FormsAuthenticationService.SignIn(model.Email, false);

                        if (String.IsNullOrEmpty(ReturnUrl))
                        {
                            //return RedirectToAction("OfflineUretimPerformans", "TVM", new { area = "TVM" });
                            //tvm de sms kontrolu yapılacak yer
                            var tvm = _AktifKullaniciService.TVMKodu;
                            var getTVMDetay = _TVMService.GetDetay(tvm);
                            if (getTVMDetay.MobilDogrulama == true && getTVMDetay.BagliOlduguTVMKodu == -9999 && _AktifKullaniciService.KullaniciKodu != 11191)
                            {
                                return RedirectToAction("MobilOnayKodu", "TVM", new { area = "TVM" });
                            }
                            else
                            {
                                return RedirectToAction("Index", "TVM", new { area = "TVM" });
                            }
                        }
                        else
                        {
                            return RedirectToLocal(ReturnUrl);
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }

            model.Password = "";

            return View(model);
        }


        [AllowAnonymous]
        public ActionResult SigortaliGiris(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        #region LİLYUM
        [AllowAnonymous]
        public ActionResult LilyumKart(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LilyumKart(LilyumLoginModel model, string ReturnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool validated = _MemberShipService.ValidateUser(model.Email, model.Password);

                    if (validated)
                    {
                        _AktifKullaniciService.SetUser(model.Email);
                        _FormsAuthenticationService.SignIn(model.Email, false);

                        if (String.IsNullOrEmpty(ReturnUrl))
                        {
                            //return RedirectToAction("OfflineUretimPerformans", "TVM", new { area = "TVM" });
                            //tvm de sms kontrolu yapılacak yer
                            //var tvm = _AktifKullaniciService.TVMKodu;
                            //var getTVMDetay = _TVMService.GetDetay(tvm);
                            //if (getTVMDetay.MobilDogrulama == true && getTVMDetay.BagliOlduguTVMKodu == -9999 && _AktifKullaniciService.KullaniciKodu != 11191)
                            //{
                            //    return RedirectToAction("MobilOnayKodu", "TVM", new { area = "TVM" });
                            //}
                            //else
                            //{
                            if (_AktifKullaniciService.TVMKodu == NeosinerjiTVM.LilyumTVMKodu) // (Lilyum Acente Yönetici Yetki Grup Kodu)
                            {
                                return RedirectToAction("Index", "TVM", new { area = "TVM" });
                            }
                            else
                            {
                                return RedirectToAction("SatinAl", "LilyumKart", new { area = "Teklif" });
                            }

                            //}
                        }
                        else
                        {
                            return RedirectToLocal(ReturnUrl);
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }

            model.Password = "";
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LilyumSifremiUnuttum()
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
        public ActionResult LilyumSifremiUnuttum(RecoverPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool sonuc = false;
                    TVMKullanicilar kullanici = _KullaniciService.GetKullaniciByEmail(model.Email);
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
                                ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
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
        public ActionResult LilyumSifremiUnuttumEski()
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
        [ValidateAntiForgeryToken]
        public ActionResult LilyumSifremiUnuttumEski(RecoverPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool sonuc = false;
                    TVMKullanicilar kullanici = _KullaniciService.GetKullaniciByEmail(model.Email);

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
                                ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
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



        #endregion

        #region AEGON

        [AllowAnonymous]
        public ActionResult Aegon(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Aegon(LoginModelAEGON model, string returnUrl)
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
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }
            catch (AccountLockedException)
            {
                ModelState.AddModelError("", babonline.Message_LockedUser);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", babonline.Message_InvalidUser);
            }

            return View(model);
        }

        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorization]
        public ActionResult LogOff()
        {
            //Aegon için ayrıca bir redireck işlemi yapılıyor.
            //if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Aegon)
            //{
            //    return RedirectToAction("LogOff", "Aegon", new { area = "" });
            //}

            _AktifKullaniciService.Logout();
            _FormsAuthenticationService.SignOut();

            switch (_AktifKullaniciService.ProjeKodu)
            {
                case TVMProjeKodlari.Aegon: return RedirectToAction("Login", "Aegon");
                case TVMProjeKodlari.Mapfre: return RedirectToAction("Login", "Mapfre");
                case TVMProjeKodlari.Lilyum: return RedirectToAction("LilyumKart", "Account");
            }

            return RedirectToAction("Index", "Home");
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
        public ActionResult SifremiUnuttum(RecoverPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool sonuc = false;
                    TVMKullanicilar kullanici = _KullaniciService.GetKullaniciByEmail(model.Email);
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
                                ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
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
        public ActionResult SifremiUnuttumEski()
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
        [ValidateAntiForgeryToken]
        public ActionResult SifremiUnuttumEski(RecoverPassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool sonuc = false;
                    TVMKullanicilar kullanici = _KullaniciService.GetKullaniciByEmail(model.Email);

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
                                ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
                                return View();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", babonline.Not_Found_User_ThisEposta);
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
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion

        #region Arda Sigorta İnternetten Poliçe Satış Ekranı

        [AllowAnonymous]
        public ActionResult InternetSatis(string returnUrl)
        {
            InternetSatisModel model = new InternetSatisModel();
            model.TVMKodu = MerkezTVMKodlari.ArdaSigorta;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        #endregion
    }
}
