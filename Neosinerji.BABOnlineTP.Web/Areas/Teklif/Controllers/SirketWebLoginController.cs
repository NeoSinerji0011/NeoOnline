using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Business.TaliPolice;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Teklif, AltMenuKodu = 0, SekmeKodu = 0)]
    public class SirketWebLoginController : Controller
    {
        IAktifKullaniciService _AktifKullaniciService;
        ITUMService _TUMService;
        IUrunService _UrunService;
        IKullaniciService _KullaniciService;
        ITeklifService _TeklifService;
        ILogService _Log;
        IRaporService _RaporService;
        IBransService _BransService;
        ITaliPoliceService _TaliPoliceService;
        ITUMContext _TumContext;
        ITVMService _TVMService;
        IPoliceService _PoliceService;
        ISigortaSirketleriService _SigortaSirketleriService;
        public SirketWebLoginController(IAktifKullaniciService aktifKullaniciService,
                                ITUMService tumService,
                                IUrunService urunService,
                                IKullaniciService kullaniciService,
                                ITeklifService teklifService,
                                IBransService bransService,
                                ITaliPoliceService taliPoliceService,
                                IPoliceService policeService,
                                ITUMContext tumContext, ISigortaSirketleriService SigortaSirketleriService, ITVMService TVMService)
        {
            _AktifKullaniciService = aktifKullaniciService;
            _TUMService = tumService;
            _UrunService = urunService;
            _KullaniciService = kullaniciService;
            _TeklifService = teklifService;
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _RaporService = DependencyResolver.Current.GetService<IRaporService>();
            _BransService = bransService;
            _TaliPoliceService = taliPoliceService;
            _PoliceService = policeService;
            _TumContext = tumContext;
            _SigortaSirketleriService = SigortaSirketleriService;
            _TVMService = TVMService;
        }

        public ActionResult SirketWeb()
        {
            SirketWebEkranModel model = new SirketWebEkranModel();
            int tvmkodu = _AktifKullaniciService.TVMKodu;
            int tumkodu = _AktifKullaniciService.TVMKodu;
            List<OtoLoginSigortaSirketKullanicilar> OtoLogin = _PoliceService.getOtoLoginSigortaSirketKullanicilar(tvmkodu).ToList();

            foreach (var item in OtoLogin)
            {


            }


            return View(model);
        }
        [HttpPost]
        public ActionResult SirketWeb(SirketWebEkranModel model)
        {
            int _TVMKodu = _AktifKullaniciService.TVMKodu;


            return View(model);
        }

    }
}
