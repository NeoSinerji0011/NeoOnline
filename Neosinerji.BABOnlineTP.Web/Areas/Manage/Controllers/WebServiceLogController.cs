using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Tools;

namespace Neosinerji.BABOnlineTP.Web.Areas.Manage.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Yonetim, AltMenuKodu = AltMenuler.SistemYonetimi, SekmeKodu = AltMenuSekmeler.WebServisLoglari)]
    public class WebServiceLogController : Controller
    {
        IWebServiceLogListService _IWebServiceLogListService;
        ITVMService _TVMService;
        ILogService _Log;

        public WebServiceLogController(IWebServiceLogListService webServiceLogListService, ITVMService tvmService)
        {
            _IWebServiceLogListService = webServiceLogListService;
            _TVMService = tvmService;
            _Log = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult Index()
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

        public ActionResult Liste()
        {
            try
            {
                WebServiceLogListModel model = new WebServiceLogListModel();

                model.IstekTipleri = new SelectList(WebServiceLogListProvider.IstekTipleri(), "Value", "Text", model.IstekTipi);
                model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                model.BitisTarihi = TurkeyDateTime.Now;

                model.TvmHQKodu = 0;
                model.TvmKodu = 0;

                model.TvmHQList = new SelectList(_TVMService.GetTVMListeKullaniciYetki(null), "Kodu", "Unvani", "").ListWithOptionLabel();
                model.TvmList = new List<SelectListItem>();

                return View(model);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [OutputCache(Duration = 0, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult ListePager()
        {
            DataTableList result = new DataTableList();

            try
            {
                if (ModelState.IsValid)
                {
                    if (Request["sEcho"] != null)
                    {
                        WebServiceLogListe webloglist = new WebServiceLogListe(Request, new Expression<Func<WebServiceLogModelOzel, object>>[]
                                                                        {
                                                                            t => t.TeklifId,
                                                                            t => t.BasariliBasarisiz,
                                                                            t => t.IstekTipi,
                                                                            t => t.IstekTarihi,
                                                                            t => t.IstekUrl,
                                                                            t => t.Sure,
                                                                            t => t.CevapTarihi,
                                                                            t => t.CevapUrl,
                                                                            t => t.Unvani,
                                                                            t => t.UrunAdi

                                                                        });

                        webloglist.TeklifId = webloglist.TryParseParamInt("TeklifId");
                        webloglist.IstekTipi = webloglist.TryParseParamByte("IstekTipi");
                        webloglist.BaslangisTarihi = webloglist.TryParseParamDate("BaslangicTarihi");
                        webloglist.BitisTarihi = webloglist.TryParseParamDate("BitisTarihi");
                        webloglist.TvmHQKodu = webloglist.TryParseParamInt("TvmHQKodu");
                        webloglist.TvmKodu = webloglist.TryParseParamInt("TvmKodu");

                        if (webloglist.BaslangisTarihi.HasValue)
                            webloglist.BaslangisTarihi = webloglist.BaslangisTarihi.Value.AddDays(-1);

                        webloglist.AddFormatter(f => f.BasariliBasarisiz, f => String.Format("<span class='label label-" + (f.BasariliBasarisiz == 1 ? "success" : "important") + "'>" + (f.BasariliBasarisiz == 1 ? "Başarılı" : "Başarısız") + "</span>"));
                        webloglist.AddFormatter(f => f.IstekTipi, f => String.Format("{0}", GetIstekTipi(f.IstekTipi)));

                        webloglist.AddFormatter(f => f.IstekUrl, f => String.Format("<a href='{0}' class='btn mini btn-success' target='_blank'>Giden</a>", f.IstekUrl));
                        webloglist.AddFormatter(f => f.CevapUrl, f => String.Format("<a href='{0}' class='btn mini btn-success' target='_blank'>Gelen</a>", f.CevapUrl));
                        webloglist.AddFormatter(f => f.Sure, f => String.Format("{0}", f.Sure.HasValue ? f.Sure.Value.ToString() : ""));

                        List<WebServiceLogModelOzel> listServiceLog = _IWebServiceLogListService.PagedList(webloglist);

                        result = webloglist.Prepare(listServiceLog, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetBagliTVMList(int tvmKodu)
        {
            var tvmler = _TVMService.GetTVMListeKullaniciYetki(tvmKodu);

            return Json(new SelectList(tvmler, "Kodu", "Unvani").ListWithOptionLabel());
        }

        private string GetIstekTipi(byte tip)
        {
            string result = "";

            switch (tip)
            {
                case 1: result = babonline.Proposal; break;
                case 2: result = babonline.Policy; break;
                case 3: result = babonline.Customer_Save; break;
                case 4: result = babonline.Accounting; break;
                case 5: result = "Kimlik Sorgu"; break;
                case 6: result = "Plaka Sorgu"; break;
                case 7: result = "EGM Sorgu"; break;
                case 8: result = "Otorizasyon Sorgu"; break;
            }

            return result;
        }
    }
}
