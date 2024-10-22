using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Controllers
{
    public class TestController : Controller
    {
        IMembershipService _MemberShipService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullaniciService;
        IKullaniciService _KullaniciService;

        public TestController(IMembershipService membershipService,
                                 IFormsAuthenticationService formsAuthenticationService,
                                 IAktifKullaniciService aktifkullaniciService,
                                 IKullaniciService kullanici)
        {
            _MemberShipService = membershipService;
            _FormsAuthenticationService = formsAuthenticationService;
            _AktifKullaniciService = aktifkullaniciService;
            _KullaniciService = kullanici;
        }

        //
        // GET: /TVM/Test/
        public ActionResult Index()
        {
#if DEBUG
            _AktifKullaniciService.SetUser("eser.yitmen@ideabilisim.com");
            _FormsAuthenticationService.SignIn("eser.yitmen@ideabilisim.com", false);

            return RedirectToAction("Liste", "Teklif", new { area = "Teklif" });
#else
            return RedirectToAction("Login", "Account");
#endif
        }
    }
}
