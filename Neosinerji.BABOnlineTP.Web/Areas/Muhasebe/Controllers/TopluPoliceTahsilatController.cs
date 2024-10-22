using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Muhasebe_CariHesap;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Muhasebe.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = 0, SekmeKodu = 0)]
    public class TopluPoliceTahsilatController : Controller
    {
        IAktifKullaniciService _AktifKullaniciService;
        ITVMService _TVMService;
        IPoliceService _PoliceService;
        IMuhasebe_CariHesapService _Muhasebe_CariHesapService;
        IUlkeService _UlkeService;
        ISigortaSirketleriService _SigortaSirketleriService;
        IBransService _BransService;
        IUrunService _UrunService;
        ITUMService _TUMService;

        public TopluPoliceTahsilatController(IAktifKullaniciService aktifKullaniciService,
            ITVMService tvmService,
            IPoliceService policeService,
            ISigortaSirketleriService sigortaSirketleriService,
            IMuhasebe_CariHesapService muhasebe_CariHesapService,
            IBransService bransService,
            IUrunService urunService,
            IUlkeService ulkeService,
            ITUMService tumService
)
        {
            _AktifKullaniciService = aktifKullaniciService;
            _TVMService = tvmService;
            _SigortaSirketleriService = sigortaSirketleriService;
            _PoliceService = policeService;
            _Muhasebe_CariHesapService = muhasebe_CariHesapService;
            _UlkeService = ulkeService;
            _BransService = bransService;
            _UrunService = urunService;
            _TUMService =tumService;
        }
        [Authorization(AnaMenuKodu = AnaMenuler.OnMuhasebe, AltMenuKodu = 0, SekmeKodu = 0)]
        // GET: Muhasebe/TopluPoliceTahsilat
        public ActionResult TopluPoliceTahsilat()
        {
            TopluPoliceTahsilatModel model = new TopluPoliceTahsilatModel();
            try
            {

                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMUnvani = _AktifKullaniciService.TVMUnvani;

                model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                model.BitisTarihi = TurkeyDateTime.Now;
                //model.RaporSonuc = new List<PoliceListesiOfflineRaporProcedureModel>();


                List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
                model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");

                List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
                model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani");

                //  model.procedurePoliceOfflineList = new List<PoliceListesiOfflineRaporProcedureModel>();

                List<Database.Models.TUMDetay> SSirketler = _TUMService.GetListTUMDetay();
                model.SigortaSirketleri = new SelectList(SSirketler, "Kodu", "Unvani");


                ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                _TvmService = DependencyResolver.Current.GetService<ITVMService>();
                bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
                var getTVMDetay = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (getTVMDetay != null)
                {
                    int KontrolTVMKod = getTVMDetay.BagliOlduguTVMKodu;
                    if (tvmTaliVar || KontrolTVMKod == -9999)
                    {
                        ViewBag.AnaTVM = true;
                    }
                    else
                    {
                        ViewBag.AnaTVM = false;
                    }

                    if (getTVMDetay.BolgeYetkilisiMi == 1)
                    {
                        model.BolgeYetkilisiMi = true;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult TopluPoliceTahsilat(TopluPoliceTahsilatModel model)
        {

            #region multiselects / branslar - tvmler - sirketler

            if (model.tvmList != null)
            {
                List<int> liste = new List<int>();
                foreach (var item in model.tvmList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(Convert.ToInt32(item));
                    }
                }
                model.TVMListe = liste;
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.TVMListe = new List<int>();
            }
            if (model.SigortaSirketleriSelectList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.SigortaSirketleriSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.SigortaSirketleriListe = liste;
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.SigortaSirketleriListe = new List<string>();
            }
       

            if (model.BransSelectList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.BransSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.BransList = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.BransList = model.BransList + liste[i] + ",";
                    else model.BransList = model.BransList + liste[i];
                }
            }


            #endregion
            List<TVMOzetModel> tvmlistesi = _TVMService.GetTVMListeKullaniciYetki(0);
            model.tvmler = new SelectList(tvmlistesi, "Kodu", "Unvani");
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            List<Database.Models.TUMDetay> SSirketler = _TUMService.GetListTUMDetay();
            model.SigortaSirketleri = new SelectList(SSirketler, "Kodu", "Unvani");


            var list = _Muhasebe_CariHesapService.GetTopluPoliceTahsilatList(model.BaslangicTarihi.Value, model.BitisTarihi.Value, model.TVMKodu, model.SigortaSirketleriListe, model.TVMListe);
            if (list!=null)
            {

            }
            return View();
        }
    }
}