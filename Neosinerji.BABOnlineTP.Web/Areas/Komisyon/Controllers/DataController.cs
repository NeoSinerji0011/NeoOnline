using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Komisyon;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Komisyon.Controllers
{
    public class DataController : Controller
    {
        ITVMService _TVMService;
        IBransService _BransService;
        ISigortaSirketleriService _SigortaSirketleriService;
        IAktifKullaniciService _AktifKullaniciService;

        public DataController(ITVMService tvmService, IBransService bransService, ISigortaSirketleriService sigortaSirketleriService, IAktifKullaniciService aktifKullaniciService)
        {
            _TVMService = tvmService;
            _BransService = bransService;
            _SigortaSirketleriService = sigortaSirketleriService;
            _AktifKullaniciService = aktifKullaniciService;
        }

        [HttpPost]
        //[Authorization(AnaMenuKodu = 0)]
        public ActionResult TVMListe()
        {
            try
            {
                //var list = _TVMService.GetTVMListeKullaniciYetki(_AktifKullaniciService.TVMKodu);
                var list = _TVMService.GetTVMListeKullaniciYetki(0);
                var tvmList = new List<TVMModel>();
                foreach (var item in list)
                {
                    var tvm = new TVMModel();
                    tvm.Kodu = item.Kodu;
                    tvm.Unvani = item.Unvani;
                    tvmList.Add(tvm);
                }
                return Json(tvmList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        //[Authorization(AnaMenuKodu = 0)]
        public ActionResult BransListe()
        {
            try
            {
                var list = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
                var bmList = new List<BransModel>();
                foreach (var item in list)
                {
                    var bm = new BransModel();
                    bm.Kodu = item.BransKodu;
                    bm.Adi = item.BransAdi;
                    bmList.Add(bm);
                }
                return Json(bmList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        [HttpPost]
        //[Authorization(AnaMenuKodu = 0)]
        public ActionResult SigortaSirketiListe()
        {
            try
            {
                var list = _SigortaSirketleriService.GetList();
                var ssList = new List<SigortaSirketiModel>();
                foreach (var item in list)
                {
                    var ss = new SigortaSirketiModel();
                    ss.Kodu = item.SirketKodu;
                    ss.Adi = item.SirketAdi;
                    ssList.Add(ss);
                }
                return Json(ssList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        //[Authorization(AnaMenuKodu = 0)]
        public ActionResult DisKaynakListe()
        {
            try
            {
                //var list = _TVMService.GetTVMListeKullaniciYetki(_AktifKullaniciService.TVMKodu);
                var list = _TVMService.GetDisUretimTVMListeKullaniciYetki(0);
                var tvmList = new List<TVMModel>();
                foreach (var item in list)
                {
                    var tvm = new TVMModel();
                    tvm.Kodu = item.Kodu;
                    tvm.Unvani = item.Unvani;
                    tvmList.Add(tvm);
                }
                return Json(tvmList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
