using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Areas.TVM.Models;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, IslemId = "YONETIM")]
    public class YonetimController : Controller
    {
        IYetkiService _YetkiService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullanici;
        IMenuService _MenuService;
        ITVMService _TVMService;
        IKullaniciService _KullaniciService;
        IMusteriService _MusteriService;
        ITeklifService _TeklifService;
        IDuyuruService _DuyuruService;

        public YonetimController(IYetkiService yetkiService,
                                 IFormsAuthenticationService formsAuthenticationService,
                                 IAktifKullaniciService aktifkullanici,
                                 ITVMService tvmService,
                                 IMenuService menuService,
                                 IKullaniciService kullaniciService,
                                 IMusteriService musteriService,
                                 ITeklifService teklifService,
                                 IDuyuruService duyuruService)
        {
            _YetkiService = yetkiService;
            _FormsAuthenticationService = formsAuthenticationService;
            _AktifKullanici = aktifkullanici;
            _MenuService = menuService;
            _TVMService = tvmService;
            _KullaniciService = kullaniciService;
            _MusteriService = musteriService;
            _TeklifService = teklifService;
            _DuyuruService = duyuruService;

        }

        // GET: /Manage/Yonetim/
        public ActionResult Index()
        {
            YonetimModel model = new YonetimModel();


            if (_AktifKullanici != null)
            {
            }

            return View(model);
        }

        public void KullaniciNotlariGetir(List<KullaniciNotListeleModel> notModel)
        {
            //Kullanıcı Notları
            List<TVMKullaniciNotlar> notlar = _TVMService.GetListKullaniciNotlar();
            foreach (var item in notlar)
            {
                KullaniciNotListeleModel mdl = new KullaniciNotListeleModel();

                mdl.NotId = item.KullaniciNotId;
                mdl.Aciklama = item.Aciklama;
                mdl.Konu = item.Konu;
                mdl.Oncelik = TVMListProvider.GetNotOnceligiText(item.Oncelik);
                mdl.BitisTarihi = item.BitisTarihi;

                notModel.Add(mdl);
            }
        }
    }
}
