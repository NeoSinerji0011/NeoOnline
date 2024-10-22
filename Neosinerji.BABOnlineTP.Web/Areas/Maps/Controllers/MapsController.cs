using Neosinerji.BABOnlineTP.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Maps.Controllers
{
    public class MapsController : Controller
    {      
        ITVMService _TVMService;
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _AktifKullanici;      
        ILogService _Log;
        public MapsController(   ITVMService tvm,
                                 IKullaniciService kullanici,
                                 IAktifKullaniciService aktifKullanici,
                                 ITeklifService teklifService
                                 )
        {
            
            _TVMService = tvm;
            _KullaniciService = kullanici;
            _AktifKullanici = aktifKullanici;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Index()
        {
            return View();
        }

    }
}
