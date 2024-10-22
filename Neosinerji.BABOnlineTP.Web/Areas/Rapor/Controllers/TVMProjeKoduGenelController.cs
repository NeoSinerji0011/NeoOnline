using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Business;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Database.Repository;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Service;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models;
using System.Globalization;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar)]
    public class TVMProjeKoduGenelController : Controller
    {
        IYetkiService _YetkiService;
        IFormsAuthenticationService _FormsAuthenticationService;
        IAktifKullaniciService _AktifKullanici;
        ITVMContext _TVMContext;
        ILogService _LogService;
        IRaporService _RaporService;
        ITUMContext _TUMContext;
        public TVMProjeKoduGenelController(IYetkiService yetkiService,
                                           IFormsAuthenticationService formsAuthenticationService,
                                           IAktifKullaniciService aktifkullanici,
                                           ITVMContext tvmContext,
                                           ILogService logService,
                                           IRaporService raporService,
                                           ITUMContext tumcontext)
        {
            _YetkiService = yetkiService;
            _FormsAuthenticationService = formsAuthenticationService;
            _AktifKullanici = aktifkullanici;
            _TVMContext = tvmContext;
            _LogService = logService;
            _RaporService = raporService;
            _TUMContext = tumcontext;
        }

        public ActionResult TVMProjeKoduGenel()
        {
            TVMProjeKoduGenelModel model = new TVMProjeKoduGenelModel();
            try
            {
                int TvmKodu = _AktifKullanici.TVMKodu;
                string ProjeKodu = _AktifKullanici.ProjeKodu;
                model.RaporSonuc = _TUMContext.GenelRapor_Getir(TvmKodu, ProjeKodu);
                model.ToplamAcenteSayisi = model.RaporSonuc.AktifAcenteSayisi + model.RaporSonuc.PasifAcenteSayisi;
                model.ToplamKullaniciSayisi = model.RaporSonuc.AktifKullaniciSayisi + model.RaporSonuc.PasifKullaniciSayisi;
                //model.TeklifSayisi = DuzenlenenTeklifSayisi();
                //model.PoliceSayisi = DuzenlenenPoliceSayisi();

            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }
            return View(model);
        }

        private DuzenlenenTeklifSayisi DuzenlenenTeklifSayisi()
        {
            DuzenlenenTeklifSayisi model = new DuzenlenenTeklifSayisi();
            model.trafik = "12";
            model.kasko = "12";
            return model;
        }

        public DuzelenenPoliceSayisi DuzenlenenPoliceSayisi()
        {
            DuzelenenPoliceSayisi model = new DuzelenenPoliceSayisi();
            model.trafik = "12";
            model.kasko = "12";
            return model;
        }
    }
}
