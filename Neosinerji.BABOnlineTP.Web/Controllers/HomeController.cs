using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Web.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.Threading;
using System.Globalization;
using reCAPTCHA.MVC;

namespace Neosinerji.BABOnlineTP.Web.Controllers
{
    public class HomeController : Controller
    {

        IMembershipService _MemberShipService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullaniciService;
        IKullaniciService _KullaniciService;


        public HomeController(IMembershipService membershipService,
                              IFormsAuthenticationService formsAuthenticationService,
                              IAktifKullaniciService aktifkullaniciService,
                              IKullaniciService kullanici
                             )
        {
            _MemberShipService = membershipService;
            _FormsAuthenticationService = formsAuthenticationService;
            _AktifKullaniciService = aktifkullaniciService;
            _KullaniciService = kullanici;
        }
        //[CaptchaValidator]
         
        public ActionResult Index(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Index", "TVM", new { area = "TVM" });
            }


            if (id == null)
                return View();
            IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            if (aktifKullanici != null && aktifKullanici.IsAuthenticated)
            {
                ViewBag.KullaniciKodu = aktifKullanici.KullaniciKodu;
                ViewBag.KullaniciAdi = aktifKullanici.AdiSoyadi;
            }
            else if (User.Identity.IsAuthenticated)
            {
                IKullaniciService kullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();
                TVMKullanicilar kullanici = kullaniciService.GetKullaniciByEmail(User.Identity.Name);

                if (kullanici != null)
                {
                    ViewBag.KullaniciKodu = kullanici.KullaniciKodu;
                    ViewBag.KullaniciAdi = kullanici.Adi + " " + kullanici.Soyadi;

                    aktifKullanici.SetUser(kullanici);
                }
            }

            return View("index2");
            //return View("OfflineUretimPerformans");
        }

        public ActionResult IndexTest()
        {
            IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            if (aktifKullanici != null && aktifKullanici.IsAuthenticated)
            {
                ViewBag.KullaniciKodu = aktifKullanici.KullaniciKodu;
                ViewBag.KullaniciAdi = aktifKullanici.AdiSoyadi;
            }
            else if (User.Identity.IsAuthenticated)
            {
                IKullaniciService kullaniciService = DependencyResolver.Current.GetService<IKullaniciService>();
                TVMKullanicilar kullanici = kullaniciService.GetKullaniciByEmail(User.Identity.Name);

                if (kullanici != null)
                {
                    ViewBag.KullaniciKodu = kullanici.KullaniciKodu;
                    ViewBag.KullaniciAdi = kullanici.Adi + " " + kullanici.Soyadi;

                    aktifKullanici.SetUser(kullanici);
                }
            }

            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult IndexTest(LoginModel model)
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
                        return RedirectToAction("Index", "TVM", new { area = "TVM" });
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

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        // ==== Sitenin dilini değiştirir ==== //
        public ActionResult ChangeCulture(Culture lang, string returnUrl)
        {
            if (!String.IsNullOrEmpty(lang.ToString()))
            {
                string culture = "";
                switch (lang.ToString())
                {
                    case "en": culture = "en-US"; break;
                    case "tr": culture = "tr-TR"; break;
                    case "it": culture = "it-IT"; break;
                    case "es": culture = "es-ES"; break;
                    case "fr": culture = "fr-FR"; break;
                }

                HttpCookie language = new HttpCookie("lang", culture);
                language.Expires = TurkeyDateTime.Now.AddDays(10);
                Response.Cookies.Add(language);

                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(culture);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

                if (!String.IsNullOrEmpty(_AktifKullaniciService.Email))
                    _AktifKullaniciService.ChangeLang(_AktifKullaniciService.Email);

            }
            return Redirect(returnUrl);
        }

        public ActionResult Harita()
        {
            return View();
        }
    }
}
