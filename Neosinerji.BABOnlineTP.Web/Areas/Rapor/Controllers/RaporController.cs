using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.GorevTakip;
using Neosinerji.BABOnlineTP.Business.Pdf;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Manage.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Police.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = 0, SekmeKodu = 0)]
    public class RaporController : Controller
    {
        ITeklifService _TeklifService;
        IUlkeService _UlkeService;
        IMusteriService _MusteriService;
        IBransService _BransService;
        IUrunService _UrunService;
        IRaporService _RaporService;
        ITVMService _TVMService;
        ILogService _log;
        IKullaniciService _KullaniciService;
        IAktifKullaniciService _AktifKullaniciService;
        ITVMContext _TVMContext;
        ISigortaSirketleriService _SigortaSirketleriService;
        IGorevTakipService _GorevTakipService;
        Neosinerji.BABOnlineTP.Business.PoliceTransfer.IPoliceService _PoliceService;
        public RaporController(ITeklifService teklifService, IUlkeService ulke, IMusteriService musteri, IBransService bransService, IUrunService urunService, ISigortaSirketleriService SigortaSirketleriService, IRaporService raporService, ITVMService tvmService, ILogService log, IAktifKullaniciService aktifKullaniciService, ITVMContext tvmContext, Neosinerji.BABOnlineTP.Business.PoliceTransfer.IPoliceService policeService, IKullaniciService kullaniciService, IGorevTakipService gorevTakipService)
        {
            _TeklifService = teklifService;
            _UlkeService = ulke;
            _MusteriService = musteri;
            _BransService = bransService;
            _UrunService = urunService;
            _RaporService = raporService;
            _TVMService = tvmService;
            _log = log;
            _AktifKullaniciService = aktifKullaniciService;
            _TVMContext = tvmContext;
            _PoliceService = policeService;
            _KullaniciService = kullaniciService;
            _SigortaSirketleriService = SigortaSirketleriService;
            _GorevTakipService = gorevTakipService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.SubeSatisRaporu)]
        public ActionResult SubeSatisRaporu()
        {

            SubeSatisRaporModel model = new SubeSatisRaporModel();

            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();

            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TVMKodu);
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi", model.BransSelectList);
            model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");

            //model.RaporSonuc = _RaporService.GetSubeSatisRaporSonuc();

            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.SubeSatisRaporu)]
        public ActionResult ListePagerSubeSatis()
        {
            if (Request["sEcho"] != null)
            {
                SubeSatisListe arama = new SubeSatisListe(Request, new Expression<Func<SubeSatisRaporProcedureModel, object>>[]
                                                                    {
                                                                        t =>t.TVMKodu,
                                                                        t =>t.Unvani,
                                                                        t =>t.ToplamPolice,
                                                                        t =>t.BrutTutar,
                                                                        t =>t.ToplamKomisyon
                                                                    });


                //arama.PoliceTarihi = arama.TryParseParamInt("PoliceTarihi");
                //tanzim=1;
                arama.PoliceTarihi = 1;


                //Hepsi=0;
                //arama.DovizTL = arama.TryParseParamInt("DovizTL");
                arama.DovizTL = 0;
                arama.BransList = arama.TryParseParamString("BransSelectList");
                arama.UrunList = arama.TryParseParamString("UrunSelectList");

                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                arama.TahIpt = arama.TryParseParamInt("TahsIptal");

                //arama.AddFormatter(f => f.TanzimTarihi, f => f.TanzimTarihi.ToString("dd.MM.yyyy"));
                //arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='/Teklif/Trafik/Detay/{0}'>{1}</a>", f.TeklifId, f.TeklifNo));

                int totalRowCount = 0;
                List<SubeSatisRaporProcedureModel> list = _RaporService.SubeSatisPagedList(arama, out totalRowCount);


                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.MTSatisRaporu)]
        public ActionResult MTSatisRaporu()
        {

            MTSatisRaporModel model = new MTSatisRaporModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;


            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();

            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TVMKodu);
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");
            model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");

            //List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            List<TVMOzetModel> tvmlist = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMLerItems = new MultiSelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.OdemeTipleri = new SelectList(OdemeTipleri.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.OdemeSekilleri = new SelectList(OdemeSekilleri.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();


            //model.TVMUnvani = _AktifKullaniciService.TVMUnvani;

            //model.RaporSonuc = _RaporService.GetMTSatisRaporSonuc();
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.MTSatisRaporu)]
        public ActionResult ListePagerMTSatis()
        {
            if (Request["sEcho"] != null)
            {
                MtSatisListe arama = new MtSatisListe(Request, new Expression<Func<MTSatisRaporProcedureModel, object>>[]
                                                                    {
                                                                        t => t.TVMKodu,
                                                                        t => t.TVMKullaniciKodu,
                                                                        t => t.ToplamPolice,
                                                                        t => t.BrutTutar,
                                                                        t => t.ToplamKomisyon,
                                                                    });


                arama.PoliceTarihi = arama.TryParseParamInt("PoliceTarihi");
                arama.DovizTL = arama.TryParseParamInt("DovizTL");
                arama.BransList = arama.TryParseParamString("BransSelectList");
                arama.UrunList = arama.TryParseParamString("UrunSelectList");
                arama.Subeler = arama.TryParseParamString("TVMLerSelectList");

                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                arama.TahIpt = arama.TryParseParamInt("TahsIptal");

                arama.OdemeSekli = arama.TryParseParamByte("OdemeSekli");
                arama.OdemeTipi = arama.TryParseParamByte("OdemeTipi");

                arama.AddFormatter(s => s.TVMKodu, s => String.Format("{0} {1}", s.TVMKodu, s.Unvani));
                arama.AddFormatter(s => s.TVMKullaniciKodu, s => String.Format("{0} {1}", s.TVMKullaniciKodu, s.AdiSoyadi));

                arama.AddFormatter(s => s.BrutTutar, s => String.Format("{0:N2}", s.BrutTutar));
                arama.AddFormatter(s => s.ToplamKomisyon, s => String.Format("{0:N2}", s.ToplamKomisyon));

                int totalRowCount = 0;
                List<MTSatisRaporProcedureModel> list = _RaporService.MtSatisPagedList(arama, out totalRowCount);

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        #region PoliceListesi Online

        [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = AltMenuler.PoliceRaporu, SekmeKodu = 0)]
        public ActionResult PoliceRaporu()
        {

            PoliceRaporuModel model = new PoliceRaporuModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;
            model.RaporSonuc = new List<PoliceRaporProcedureModel>();

            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            // model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();

            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TVMKodu);
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");
          //  model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");

            List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani");


            List<Urun> urunlist = _UrunService.GetListUrun();
            model.UrunlerItems = new MultiSelectList(urunlist, "UrunKodu", "UrunAdi");


            model.OdemeTipleri = new SelectList(OdemeTipleriRaporModel.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.OdemeSekilleri = new SelectList(OdemeSekilleriRaporModel.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();

            // model.Durumlari = new SelectList(PoliceDurumlari.PoliceDurumlariList(), "Value", "Text", "").ToList();

            return View(model);
        }
        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = AltMenuler.PoliceRaporu, SekmeKodu = 0)]
        public ActionResult ListePagerPoliceRapor()
        {
            if (Request["sEcho"] != null)
            {
                PoliceRaporListe arama = new PoliceRaporListe(Request, new Expression<Func<PoliceRaporProcedureModel, object>>[]
                                                                    {
                                                                        t => t.BaslamaTarihi,
                                                                        t => t.BitisTarihi,
                                                                        t => t.BrutPrim,
                                                                        t => t.ToplamKomisyon,
                                                                        //t => t.TarifeBasamakKodu,
                                                                        //t => t.MT,
                                                                        t => t.SigortaEttiren,
                                                                        t => t.Sigortali,
                                                                        t => t.TanzimTarihi,
                                                                        t => t.TeklifNo,
                                                                        t => t.TUM,
                                                                        t => t.TUMPoliceNo,
                                                                        t =>t.TUMTeklifNo,
                                                                        t => t.TVM,
                                                                        t => t.UrunAdi,
                                                                        t => t.OzelAlan,
                                                                        t => t.Kullanici,
                                                                        t => t.OdemeTipi,
                                                                        t => t.OdemeSekli,
                                                                       // t => t.Durumu,
                                                                        t => t.ZeyilNo,
                                                                        t => t.YenilemeNo,
                                                                        t => t.PoliceSuresi,
                                                                       // t => t.BankaAdi,
                                                                        t => t.TaksitSayisi

                                                                    });


                arama.PoliceTarihi = arama.TryParseParamByte("PoliceTarihi");
                //arama.DovizTL = arama.TryParseParamInt("DovizTL");
                arama.BransList = arama.TryParseParamString("BransSelectList");
                arama.UrunList = arama.TryParseParamString("UrunSelectList");

                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                //  arama.TahIpt = arama.TryParseParamByte("TahsIptal");
                arama.Subeler = arama.TryParseParamString("TVMLerSelectList");

                arama.PoliceNo = arama.TryParseParamString("PoliceNo");
                arama.OdemeSekli = arama.TryParseParamByte("OdemeSekli");
                arama.OdemeTipi = arama.TryParseParamByte("OdemeTipi");
                //arama.Durumu = arama.TryParseParamByte("Durumu");

                int totalRowCount = 0;
                List<PoliceRaporProcedureModel> list = _RaporService.PoliceRaporPagedList(arama, out totalRowCount);

                //TempData["PDFTable"] =list;     

                //TempData["PDFTable1"] = list;   

                arama.AddFormatter(f => f.BaslamaTarihi, f => String.Format("{0}", f.BaslamaTarihi.HasValue ? f.BaslamaTarihi.Value.ToString("dd.MM.yyyy") : ""));
                arama.AddFormatter(f => f.BitisTarihi, f => String.Format("{0}", f.BitisTarihi.HasValue ? f.BitisTarihi.Value.ToString("dd.MM.yyyy") : ""));
                arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.HasValue ? f.TanzimTarihi.Value.ToString("dd.MM.yyyy") : ""));
                arama.AddFormatter(f => f.OdemeTipi, f => String.Format("{0}", f.OdemeTipi.HasValue ? TeklifOdemeTipleri.OdemeTipi(Convert.ToByte(f.OdemeTipi.Value)) : ""));
                arama.AddFormatter(f => f.OdemeSekli, f => String.Format("{0}", f.OdemeSekli.HasValue ? OdemeSekilleri.OdemeSekli(Convert.ToByte(f.OdemeSekli.Value)) : ""));

                //arama.AddFormatter(f => f.TeklifNo, f => String.Format("{0}", (f.Durumu == "Offline") ? "" : String.Format("<a href='{0}{1}'>{2}</a> {3}",
                //                                     TeklifSayfaAdresleri.DetayAdres(f.Urunkodu), f.AnaTeklifId, ((f.TeklifNo == 0) ? "" : f.TeklifNo.ToString()),
                //                                     (String.IsNullOrEmpty(f.AnaTeklifPDF)) ? "" :
                //                                     "<a href=" + f.AnaTeklifPDF + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                //                                     "<img src='/content/img/pdf_icon.png' /></a>")));

                //arama.AddFormatter(f => f.TUMPoliceNo, f => String.Format("{0}", (f.Durumu == "Offline") ? f.TUMPoliceNo : String.Format("<a href='{0}{1}'>{2}</a> {3}",
                //                                                            TeklifSayfaAdresleri.PoliceAdres(f.Urunkodu), f.TeklifId, f.TUMPoliceNo,
                //                                                            (String.IsNullOrEmpty(f.PDFDosyasi) ? "" :
                //                                                            "<a href=" + f.PDFDosyasi + " title='Poliçe PDF' target='_blank' class='pull-right'>" +
                //                                                            "<img src='/content/img/pdf_icon.png' /></a>"))));

                arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", GetOzelAlan(s.TeklifId)));

                arama.AddFormatter(s => s.SatisTuru, s => String.Format("{0}", SatisTurleri.SatisTuru(s.SatisTuru)));
                
                arama.AddFormatter(s => s.BrutPrim, s => String.Format("{0:N2}", s.BrutPrim));
                

                DataTableList result = arama.Prepare(list, totalRowCount);


                return Json(result, JsonRequestBehavior.AllowGet);

            }
            return View();
        }

        #endregion

        #region PoliceListesi Offline
        [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = AltMenuler.PoliceListesiOffline, SekmeKodu = 0)]
        public ActionResult PoliceListesi()
        {
            PoliceListesiOfflineModelim model = new PoliceListesiOfflineModelim();

            try
            {

                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
                
                model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                model.BitisTarihi = TurkeyDateTime.Now;
                //model.RaporSonuc = new List<PoliceListesiOfflineRaporProcedureModel>();

                model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();

                List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
                model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");

                model.TVMLerItems = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
                model.uretimTvmler = new MultiSelectList(_TVMService.GetDisUretimTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

                model.procedurePoliceOfflineList = new List<PoliceListesiOfflineRaporProcedureModel>();

                List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
                model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");

                model.OdemeTipleri = new SelectList(OdemeTipleriRaporModel.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
                model.OdemeSekilleri = new SelectList(OdemeSekilleriRaporModel.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();

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
        [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = AltMenuler.PoliceListesiOffline, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult PoliceListesi(PoliceListesiOfflineModelim model)
        {
            #region multiselects / branslar - tvmler - sirketler

            if (model.TVMLerSelectList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.TVMLerSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.TVMListe = String.Empty;
                if (liste.Count > 0)
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        if (i != liste.Count - 1)
                            model.TVMListe = model.TVMListe + liste[i] + ",";
                        else model.TVMListe = model.TVMListe + liste[i];
                    }
                }
                else
                {
                    model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                }

            }
            else
            {
                model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
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
                model.SigortaSirket = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.SigortaSirket = model.SigortaSirket + liste[i] + ",";
                    else model.SigortaSirket = model.SigortaSirket + liste[i];
                }
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
            if (model.uretimTvmList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.uretimTvmList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.UretimTVMListe = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.UretimTVMListe = model.UretimTVMListe + liste[i] + ",";
                    else model.UretimTVMListe = model.UretimTVMListe + liste[i];
                }
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.UretimTVMListe = String.Empty;
            }
            #endregion

            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            int anaTVMKodu = _AktifKullaniciService.TVMKodu;
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");
            
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
            model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");

            //model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(model.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.TVMLerItems = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.uretimTvmler = new MultiSelectList(_TVMService.GetDisUretimTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.OdemeTipleri = new SelectList(OdemeTipleriRaporModel.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.OdemeSekilleri = new SelectList(OdemeSekilleriRaporModel.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();

            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
            TVMDetay tvmDetay = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu);


            if (tvmDetay.BolgeYetkilisiMi == 1)
            {
                model.BolgeYetkilisiMi = true;
            }
            byte Merkez = 0;
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                Merkez = 1;

            }

            anaTVMKodu = _AktifKullaniciService.TVMKodu;

            if (tvmTaliVar || KontrolTVMKod == -9999)
            {
                #region   talisi olan anaacente ya da talisi olmayan anaacanete
                //anaTVMKodu = _AktifKullaniciService.TVMKodu;
                if (model.TVMListe != "" || model.PoliceNo != null)
                {
                    model.procedurePoliceOfflineList = new List<PoliceListesiOfflineRaporProcedureModel>();
                    model.procedurePoliceOfflineList = _RaporService.PoliceListesiOfflineRaporGetir(model.TVMKodu, model.PoliceTarihTipi, model.BaslangicTarihi, model.BitisTarihi, model.TVMListe,
                                                   model.SigortaSirket, model.BransList,
                                                   model.PoliceNo, model.OdemeSekli, model.OdemeTipi, anaTVMKodu, model.UretimTVMListe);

                    var pdfUrl = CreateDonemselRaporPDF(model);


                    if (!String.IsNullOrEmpty(pdfUrl))
                    {
                        model.PDFURL = pdfUrl;
                        model.pdfVar = true;
                    }
                    else
                    {
                        model.pdfVar = false;
                    }

                    ViewBag.AnaTVM = true;
                }
                #endregion
                else
                {
                    #region tali tvm
                    anaTVMKodu = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
                    if (anaTVMKodu == -9999)
                    {
                        anaTVMKodu = _AktifKullaniciService.TVMKodu;
                    }
                    if (model.TVMListe != "" || model.PoliceNo != null)
                    {
                        model.procedurePoliceOfflineList = new List<PoliceListesiOfflineRaporProcedureModel>();
                        model.procedurePoliceOfflineList = _RaporService.PoliceListesiOfflineRaporGetir(model.TVMKodu, model.PoliceTarihTipi, model.BaslangicTarihi, model.BitisTarihi, model.TVMListe,
                                                       model.SigortaSirket, model.BransList,
                                                       model.PoliceNo, model.OdemeSekli, model.OdemeTipi, anaTVMKodu, model.UretimTVMListe);

                    }
                    //if (model.procedurePoliceOfflineList.Count > 0)
                    //{
                    //    for (int i = model.procedurePoliceOfflineList.Count - 1; i >= 0; i--)
                    //    {
                    //        var telefonDetay = _MusteriService.GetMusteriTelefon(model.procedurePoliceOfflineList[i].mGrupKodu).FirstOrDefault();
                    //        if (telefonDetay != null)
                    //        {
                    //            string musTelefon = telefonDetay.Numara;
                    //            model.procedurePoliceOfflineList[i].cepTel = musTelefon == null ? " " : musTelefon;
                    //        }
                    //        else
                    //        {
                    //            model.procedurePoliceOfflineList[i].cepTel = "";
                    //        }
                    //    }
                    //}
                    ViewBag.AnaTVM = false;

                }

                #endregion
            }
            return View(model);
        }
        #endregion

        #region police teklif listesi

        [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = AltMenuler.PoliceTeklifListesi, SekmeKodu = 0)]
        public ActionResult PoliceTeklifListesi()
        {
            TeklifPoliceListesi model = new TeklifPoliceListesi();

            try
            {

                model.durumlar = new SelectList(DurumListesiAktifPasif.TeklifPoliceListesi(), "Value", "Text", "0");
                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
                ViewBag.AcenteTipi = _AktifKullaniciService.TvmTipi;
                model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                model.BitisTarihi = TurkeyDateTime.Now;
                //model.RaporSonuc = new List<PoliceListesiOfflineRaporProcedureModel>();

                model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();

                List<Bran> brans = _BransService.GetListBroker();
                model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");

                model.TVMLerItems = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
                model.uretimTvmler = new MultiSelectList(_TVMService.GetDisUretimTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

                model.procedurePoliceOfflineList = new List<ReasurorPoliceListesiProcedureModel>();
                model.procedureTeklifOfflineList = new List<ReasurorTeklifListesiProcedureModel>();

                List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
                List<SigortaSirketleri> tempbrokerItem = new List<SigortaSirketleri>();
                foreach (var item in SSirketler)
                {
                    if (int.TryParse(item.SirketKodu.ToString(), out int a) && int.TryParse(item.SirketKodu.ToString(), out int b))
                    {
                        if (a >= 500 && b <= 530)
                        {
                            tempbrokerItem.Add(item);
                        }
                    }
                }
                foreach (var item in tempbrokerItem)
                {
                    SSirketler.Remove(item);
                    SSirketler.Insert(0, item);
                }

                model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");
                model.OdemeTipleri = new SelectList(OdemeTipleriRaporModel.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
                model.OdemeSekilleri = new SelectList(OdemeSekilleriRaporModel.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();

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


            if (_AktifKullaniciService.Gorevi == 4)
            {
                ViewBag.AnaTVM = false;
            }
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = AltMenuler.PoliceTeklifListesi, SekmeKodu = 0)]
        [HttpPost]
        public ActionResult PoliceTeklifListesi(TeklifPoliceListesi model)
        {
            #region multiselects / branslar - tvmler - sirketler

            if (model.TVMLerSelectList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.TVMLerSelectList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.TVMListe = String.Empty;
                if (liste.Count > 0)
                {
                    for (int i = 0; i < liste.Count; i++)
                    {
                        if (i != liste.Count - 1)
                            model.TVMListe = model.TVMListe + liste[i] + ",";
                        else model.TVMListe = model.TVMListe + liste[i];
                    }
                }
                else
                {
                    model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                }

            }
            else
            {
                model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
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
                model.SigortaSirket = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.SigortaSirket = model.SigortaSirket + liste[i] + ",";
                    else model.SigortaSirket = model.SigortaSirket + liste[i];
                }
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
            if (model.uretimTvmList != null)
            {
                List<string> liste = new List<string>();
                foreach (var item in model.uretimTvmList)
                {
                    if (item != "multiselect-all")
                    {
                        liste.Add(item);
                    }
                }
                model.UretimTVMListe = String.Empty;
                for (int i = 0; i < liste.Count; i++)
                {
                    if (i != liste.Count - 1)
                        model.UretimTVMListe = model.UretimTVMListe + liste[i] + ",";
                    else model.UretimTVMListe = model.UretimTVMListe + liste[i];
                }
            }
            else
            {
                // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
                model.UretimTVMListe = String.Empty;
            }
            //else
            //{
            //    // model.TVMListe = _AktifKullaniciService.TVMKodu.ToString();
            //    model.TVMListe = String.Empty;
            //}

            #endregion

            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            int anaTVMKodu = _AktifKullaniciService.TVMKodu;
            ViewBag.AcenteTipi = _AktifKullaniciService.TvmTipi;
            List<Bran> brans = _BransService.GetListBroker();
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");

            model.durumlar = new SelectList(DurumListesiAktifPasif.TeklifPoliceListesi(), "Value", "Text", model.durum);
            List<Database.Models.SigortaSirketleri> SSirketler = _SigortaSirketleriService.GetList();
            List<SigortaSirketleri> tempbrokerItem = new List<SigortaSirketleri>();
            foreach (var item in SSirketler)
            {
                if (int.TryParse(item.SirketKodu.ToString(), out int a) && int.TryParse(item.SirketKodu.ToString(), out int b))
                {
                    if (a >= 500 && b <= 530)
                    {
                        tempbrokerItem.Add(item);
                    }
                }
            }
            foreach (var item in tempbrokerItem)
            {
                SSirketler.Remove(item);
                SSirketler.Insert(0, item);
            }
            model.SigortaSirketleri = new MultiSelectList(SSirketler, "SirketKodu", "SirketAdi");

            //model.tvmler = new MultiSelectList(_TaliPoliceService.GetYetkiliTVM(model.TVMKodu).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.TVMLerItems = new MultiSelectList(_TVMService.GetTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");
            model.uretimTvmler = new MultiSelectList(_TVMService.GetDisUretimTVMListeKullaniciYetki(0).OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.OdemeTipleri = new SelectList(OdemeTipleriRaporModel.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.OdemeSekilleri = new SelectList(OdemeSekilleriRaporModel.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();

            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();

            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
            TVMDetay tvmDetay = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu);


            if (tvmDetay.BolgeYetkilisiMi == 1)
            {
                model.BolgeYetkilisiMi = true;
            }
            byte Merkez = 0;
            if (tvmDetay != null && tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                Merkez = 1;
            }

            anaTVMKodu = _AktifKullaniciService.TVMKodu;

            // teklif Liste Arama
            if (model.durum == 1)
            {

                if (tvmTaliVar || KontrolTVMKod == -9999)
                {
                    #region   talisi olan anaacente ya da talisi olmayan anaacanete
                    if (model.TVMListe != "" || model.PoliceNo != null)
                    {
                        model.procedureTeklifOfflineList = new List<ReasurorTeklifListesiProcedureModel>();
                        model.procedureTeklifOfflineList = _RaporService.ReasurorTeklifListesi(model.TVMKodu, model.PoliceTarihTipi, model.BaslangicTarihi, model.BitisTarihi, model.TVMListe,
                                                       model.SigortaSirket, model.BransList,
                                                       model.PoliceNo, model.OdemeSekli, model.OdemeTipi, anaTVMKodu, model.UretimTVMListe);
                    }
                    #endregion
                }
                else
                {
                    #region tali tvm
                    anaTVMKodu = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
                    if (anaTVMKodu == -9999)
                    {
                        anaTVMKodu = _AktifKullaniciService.TVMKodu;
                    }
                    if (model.TVMListe != "" || model.PoliceNo != null)
                    {
                        model.procedureTeklifOfflineList = new List<ReasurorTeklifListesiProcedureModel>();
                        model.procedureTeklifOfflineList = _RaporService.ReasurorTeklifListesi(model.TVMKodu, model.PoliceTarihTipi, model.BaslangicTarihi, model.BitisTarihi, model.TVMListe,
                                                       model.SigortaSirket, model.BransList,
                                                       model.PoliceNo, model.OdemeSekli, model.OdemeTipi, anaTVMKodu, model.UretimTVMListe);

                    }
                    ViewBag.AnaTVM = false;
                    #endregion
                }
            }
            //poliçe Liste Arama
            else
            {
                if (tvmTaliVar || KontrolTVMKod == -9999)
                {
                    #region   talisi olan anaacente ya da talisi olmayan anaacanete
                    if (model.TVMListe != "" || model.PoliceNo != null)
                    {
                        model.procedurePoliceOfflineList = new List<ReasurorPoliceListesiProcedureModel>();
                        model.procedurePoliceOfflineList = _RaporService.ReasurorPoliceListesi(model.TVMKodu, model.PoliceTarihTipi, model.BaslangicTarihi, model.BitisTarihi, model.TVMListe,
                                                       model.SigortaSirket, model.BransList,
                                                       model.PoliceNo, model.OdemeSekli, model.OdemeTipi, anaTVMKodu, model.UretimTVMListe);
                    }
                    ViewBag.AnaTVM = true;

                    #endregion
                }
                else
                {
                    #region tali tvm
                    anaTVMKodu = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;
                    if (anaTVMKodu == -9999)
                    {
                        anaTVMKodu = _AktifKullaniciService.TVMKodu;
                    }
                    if (model.TVMListe != "" || model.PoliceNo != null)
                    {
                        model.procedurePoliceOfflineList = new List<ReasurorPoliceListesiProcedureModel>();
                        model.procedurePoliceOfflineList = _RaporService.ReasurorPoliceListesi(model.TVMKodu, model.PoliceTarihTipi, model.BaslangicTarihi, model.BitisTarihi, model.TVMListe,
                                                       model.SigortaSirket, model.BransList,
                                                       model.PoliceNo, model.OdemeSekli, model.OdemeTipi, anaTVMKodu, model.UretimTVMListe);

                    }
                    ViewBag.AnaTVM = false;
                    #endregion
                }
            }



            #region değerleri sıfırlama
            model.GenelPolToplamSayac = 0;
            model.GenelZeylToplamSayac = 0;

            //tahakkuk toplam
            model.ToplamTeminatTutariTLTahakkuk = 0;
            model.ToplamYurtdisiPrimTLTahakkuk = 0;
            model.ToplamYurtdisiDisKaynakKomisyonTLTahakkuk = 0;
            model.ToplamYurtdisiAlinanKomisyonTLTahakkuk = 0;
            model.ToplamFrontingSigortaSirketiKomisyonTLTahakkuk = 0;
            model.ToplamSatisKanaliKomisyonTLTahakkuk = 0;
            model.ToplamYurticiAlinanKomisyonTLTahakkuk = 0;
            model.ToplamYurtdisiNetPrimTLTahakkuk = 0;
            model.ToplamYurtdisiBrokerNetPrimTLTahakkuk = 0;
            model.ToplamYurticiNetPrimTLTahakkuk = 0;
            model.ToplamYurticiBrutPrimTLTahakkuk = 0;
            model.ToplamBsmvTLTahakkuk = 0;

            //iptal toplam 
            model.ToplamTeminatTutariTLIptal = 0;
            model.ToplamYurtdisiPrimTLIptal = 0;
            model.ToplamYurtdisiDisKaynakKomisyonTLIptal = 0;
            model.ToplamYurtdisiAlinanKomisyonTLIptal = 0;
            model.ToplamFrontingSigortaSirketiKomisyonTLIptal = 0;
            model.ToplamSatisKanaliKomisyonTLIptal = 0;
            model.ToplamYurticiAlinanKomisyonTLIptal = 0;
            model.ToplamYurtdisiNetPrimTLIptal = 0;
            model.ToplamYurtdisiBrokerNetPrimTLIptal = 0;
            model.ToplamYurticiNetPrimTLIptal = 0;
            model.ToplamYurticiBrutPrimTLIptal = 0;
            model.ToplamBsmvTLIptal = 0;

            // genel toplam (tahakkuk + iptal)
            model.ToplamTeminatTutariTL = 0;
            model.ToplamYurtdisiPrimTL = 0;
            model.ToplamYurtdisiDisKaynakKomisyonTL = 0;
            model.ToplamYurtdisiAlinanKomisyonTL = 0;
            model.ToplamFrontingSigortaSirketiKomisyonTL = 0;
            model.ToplamSatisKanaliKomisyonTL = 0;
            model.ToplamYurticiAlinanKomisyonTL = 0;
            model.ToplamYurtdisiNetPrimTL = 0;
            model.ToplamYurtdisiBrokerNetPrimTL = 0;
            model.ToplamYurticiNetPrimTL = 0;
            model.ToplamYurticiBrutPrimTL = 0;
            model.ToplamBsmvTL = 0;

            // dolar
            model.ToplamDolarTeminatTutari = 0;
            model.ToplamDolarYurtdisiPrim = 0;
            model.ToplamDolarYurtdisiDisKaynakKomisyon = 0;
            model.ToplamDolarYurtdisiAlinanKomisyon = 0;
            model.ToplamDolarFrontingSigortaSirketiKomisyon = 0;
            model.ToplamDolarSatisKanaliKomisyon = 0;
            model.ToplamDolarYurticiAlinanKomisyon = 0;
            model.ToplamDolarYurtdisiNetPrim = 0;
            model.ToplamDolarYurtdisiBrokerNetPrim = 0;
            model.ToplamDolarYurticiNetPrim = 0;
            model.ToplamDolarYurticiBrutPrim = 0;
            model.ToplamDolarBsmv = 0;

            //euro

            model.ToplamEuroTeminatTutari = 0;
            model.ToplamEuroYurtdisiPrim = 0;
            model.ToplamEuroYurtdisiDisKaynakKomisyon = 0;
            model.ToplamEuroYurtdisiAlinanKomisyon = 0;
            model.ToplamEuroFrontingSigortaSirketiKomisyon = 0;
            model.ToplamEuroSatisKanaliKomisyon = 0;
            model.ToplamEuroYurticiAlinanKomisyon = 0;
            model.ToplamEuroYurtdisiNetPrim = 0;
            model.ToplamEuroYurtdisiBrokerNetPrim = 0;
            model.ToplamEuroYurticiNetPrim = 0;
            model.ToplamEuroYurticiBrutPrim = 0;
            model.ToplamEuroBsmv = 0;

            //aed

            model.ToplamAedTeminatTutari = 0;
            model.ToplamAedYurtdisiPrim = 0;
            model.ToplamAedYurtdisiDisKaynakKomisyon = 0;
            model.ToplamAedYurtdisiAlinanKomisyon = 0;
            model.ToplamAedFrontingSigortaSirketiKomisyon = 0;
            model.ToplamAedSatisKanaliKomisyon = 0;
            model.ToplamAedYurticiAlinanKomisyon = 0;
            model.ToplamAedYurtdisiNetPrim = 0;
            model.ToplamAedYurtdisiBrokerNetPrim = 0;
            model.ToplamAedYurticiNetPrim = 0;
            model.ToplamAedYurticiBrutPrim = 0;
            model.ToplamAedBsmv = 0;

            #endregion
            if (model.procedurePoliceOfflineList != null)
            {
                if (model.procedurePoliceOfflineList.Count > 0)
                {
                    for (int i = model.procedurePoliceOfflineList.Count - 1; i >= 0; i--)
                    {

                        var topPoliceSayac = 0;
                        var topZeylSayac = 0;
                        if (model.procedurePoliceOfflineList[i].SirketAdi == "ALLIANZ SİGORTA A.Ş." && model.procedurePoliceOfflineList[i].EkNo == 1)
                        {
                            topPoliceSayac++;
                        }
                        else if (model.procedurePoliceOfflineList[i].EkNo == 0 && model.procedurePoliceOfflineList[i].SirketAdi != "ALLIANZ SİGORTA A.Ş.")
                        {
                            topPoliceSayac++;
                        }
                        else if (model.procedurePoliceOfflineList[i].EkNo != 0 && model.procedurePoliceOfflineList[i].SirketAdi != "ALLIANZ SİGORTA A.Ş.")
                        {
                            topZeylSayac++;
                        }
                        else if (model.procedurePoliceOfflineList[i].SirketAdi == "ALLIANZ SİGORTA A.Ş." && model.procedurePoliceOfflineList[i].BransAdi == "DASK" && model.procedurePoliceOfflineList[i].EkNo.ToString().Length > 3)
                        {
                            if (model.procedurePoliceOfflineList[i].EkNo.ToString().Substring(4, 1) == "1")
                            {
                                topPoliceSayac++;
                            }
                            else
                            {
                                topZeylSayac++;
                            }
                        }
                        else
                        {
                            topZeylSayac++;
                        }
                        model.GenelPolToplamSayac += topPoliceSayac;
                        model.GenelZeylToplamSayac += topZeylSayac;


                        if (model.procedurePoliceOfflineList[i].YurticiBrutPrimTL > 0)
                        {
                            //tahakkuk toplam
                            model.ToplamTeminatTutariTLTahakkuk += model.procedurePoliceOfflineList[i].TeminatTutariTL;
                            model.ToplamYurtdisiPrimTLTahakkuk += model.procedurePoliceOfflineList[i].YurtdisiPrimTL;
                            model.ToplamYurtdisiDisKaynakKomisyonTLTahakkuk += model.procedurePoliceOfflineList[i].YurtdisiDisKaynakKomisyonTL;
                            model.ToplamYurtdisiAlinanKomisyonTLTahakkuk += model.procedurePoliceOfflineList[i].YurtdisiAlinanKomisyonTL;
                            model.ToplamFrontingSigortaSirketiKomisyonTLTahakkuk += model.procedurePoliceOfflineList[i].FrontingSigortaSirketiKomisyonTL;
                            model.ToplamSatisKanaliKomisyonTLTahakkuk += model.procedurePoliceOfflineList[i].SatisKanaliKomisyonTL;
                            model.ToplamYurticiAlinanKomisyonTLTahakkuk += model.procedurePoliceOfflineList[i].YurticiAlinanKomisyonTL;
                            model.ToplamYurtdisiNetPrimTLTahakkuk += model.procedurePoliceOfflineList[i].YurtdisiNetPrimTL;
                            model.ToplamYurtdisiBrokerNetPrimTLTahakkuk += model.procedurePoliceOfflineList[i].YurtdisiBrokerNetPrimTL;
                            model.ToplamYurticiNetPrimTLTahakkuk += model.procedurePoliceOfflineList[i].YurticiNetPrimTL;
                            model.ToplamYurticiBrutPrimTLTahakkuk += model.procedurePoliceOfflineList[i].YurticiBrutPrimTL;
                            model.ToplamBsmvTLTahakkuk += model.procedurePoliceOfflineList[i].BsmvTL;
                        }
                        else
                        {
                            //iptal toplam 
                            model.ToplamTeminatTutariTLIptal += model.procedurePoliceOfflineList[i].TeminatTutariTL;
                            model.ToplamYurtdisiPrimTLIptal += model.procedurePoliceOfflineList[i].YurtdisiPrimTL;
                            model.ToplamYurtdisiDisKaynakKomisyonTLIptal += model.procedurePoliceOfflineList[i].YurtdisiDisKaynakKomisyonTL;
                            model.ToplamYurtdisiAlinanKomisyonTLIptal += model.procedurePoliceOfflineList[i].YurtdisiAlinanKomisyonTL;
                            model.ToplamFrontingSigortaSirketiKomisyonTLIptal += model.procedurePoliceOfflineList[i].FrontingSigortaSirketiKomisyonTL;
                            model.ToplamSatisKanaliKomisyonTLIptal += model.procedurePoliceOfflineList[i].SatisKanaliKomisyonTL;
                            model.ToplamYurticiAlinanKomisyonTLIptal += model.procedurePoliceOfflineList[i].YurticiAlinanKomisyonTL;
                            model.ToplamYurtdisiNetPrimTLIptal += model.procedurePoliceOfflineList[i].YurtdisiNetPrimTL;
                            model.ToplamYurtdisiBrokerNetPrimTLIptal += model.procedurePoliceOfflineList[i].YurtdisiBrokerNetPrimTL;
                            model.ToplamYurticiNetPrimTLIptal += model.procedurePoliceOfflineList[i].YurticiNetPrimTL;
                            model.ToplamYurticiBrutPrimTLIptal += model.procedurePoliceOfflineList[i].YurticiBrutPrimTL;
                            model.ToplamBsmvTLIptal += model.procedurePoliceOfflineList[i].BsmvTL;

                        }

                        // dolar
                        if (model.procedurePoliceOfflineList[i].ParaBirimi == "USD")
                        {
                            model.ToplamDolarTeminatTutari += model.procedurePoliceOfflineList[i].TeminatTutari;
                            model.ToplamDolarYurtdisiPrim += model.procedurePoliceOfflineList[i].YurtdisiPrim;
                            model.ToplamDolarYurtdisiDisKaynakKomisyon += model.procedurePoliceOfflineList[i].YurtdisiDisKaynakKomisyon;
                            model.ToplamDolarYurtdisiAlinanKomisyon += model.procedurePoliceOfflineList[i].YurtdisiAlinanKomisyon;
                            model.ToplamDolarFrontingSigortaSirketiKomisyon += model.procedurePoliceOfflineList[i].FrontingSigortaSirketiKomisyon;
                            model.ToplamDolarSatisKanaliKomisyon += model.procedurePoliceOfflineList[i].SatisKanaliKomisyon;
                            model.ToplamDolarYurticiAlinanKomisyon += model.procedurePoliceOfflineList[i].YurticiAlinanKomisyon;
                            model.ToplamDolarYurtdisiNetPrim += model.procedurePoliceOfflineList[i].YurtdisiNetPrim;
                            model.ToplamDolarYurtdisiBrokerNetPrim += model.procedurePoliceOfflineList[i].YurtdisiBrokerNetPrim;
                            model.ToplamDolarYurticiNetPrim += model.procedurePoliceOfflineList[i].YurticiNetPrim;
                            model.ToplamDolarYurticiBrutPrim += model.procedurePoliceOfflineList[i].YurticiBrutPrim;
                            model.ToplamDolarBsmv += model.procedurePoliceOfflineList[i].Bsmv;

                        }
                        //euro
                        if (model.procedurePoliceOfflineList[i].ParaBirimi == "EUR")
                        {

                            model.ToplamEuroTeminatTutari += model.procedurePoliceOfflineList[i].TeminatTutari;
                            model.ToplamEuroYurtdisiPrim += model.procedurePoliceOfflineList[i].YurtdisiPrim;
                            model.ToplamEuroYurtdisiDisKaynakKomisyon += model.procedurePoliceOfflineList[i].YurtdisiDisKaynakKomisyon;
                            model.ToplamEuroYurtdisiAlinanKomisyon += model.procedurePoliceOfflineList[i].YurtdisiAlinanKomisyon;
                            model.ToplamEuroFrontingSigortaSirketiKomisyon += model.procedurePoliceOfflineList[i].FrontingSigortaSirketiKomisyon;
                            model.ToplamEuroSatisKanaliKomisyon += model.procedurePoliceOfflineList[i].SatisKanaliKomisyon;
                            model.ToplamEuroYurticiAlinanKomisyon += model.procedurePoliceOfflineList[i].YurticiAlinanKomisyon;
                            model.ToplamEuroYurtdisiNetPrim += model.procedurePoliceOfflineList[i].YurtdisiNetPrim;
                            model.ToplamEuroYurtdisiBrokerNetPrim += model.procedurePoliceOfflineList[i].YurtdisiBrokerNetPrim;
                            model.ToplamEuroYurticiNetPrim += model.procedurePoliceOfflineList[i].YurticiNetPrim;
                            model.ToplamEuroYurticiBrutPrim += model.procedurePoliceOfflineList[i].YurticiBrutPrim;
                            model.ToplamEuroBsmv += model.procedurePoliceOfflineList[i].Bsmv;


                        }
                        //aed
                        if (model.procedurePoliceOfflineList[i].ParaBirimi == "AED")
                        {

                            model.ToplamAedTeminatTutari += model.procedurePoliceOfflineList[i].TeminatTutari;
                            model.ToplamAedYurtdisiPrim += model.procedurePoliceOfflineList[i].YurtdisiPrim;
                            model.ToplamAedYurtdisiDisKaynakKomisyon += model.procedurePoliceOfflineList[i].YurtdisiDisKaynakKomisyon;
                            model.ToplamAedYurtdisiAlinanKomisyon += model.procedurePoliceOfflineList[i].YurtdisiAlinanKomisyon;
                            model.ToplamAedFrontingSigortaSirketiKomisyon += model.procedurePoliceOfflineList[i].FrontingSigortaSirketiKomisyon;
                            model.ToplamAedSatisKanaliKomisyon += model.procedurePoliceOfflineList[i].SatisKanaliKomisyon;
                            model.ToplamAedYurticiAlinanKomisyon += model.procedurePoliceOfflineList[i].YurticiAlinanKomisyon;
                            model.ToplamAedYurtdisiNetPrim += model.procedurePoliceOfflineList[i].YurtdisiNetPrim;
                            model.ToplamAedYurtdisiBrokerNetPrim += model.procedurePoliceOfflineList[i].YurtdisiBrokerNetPrim;
                            model.ToplamAedYurticiNetPrim += model.procedurePoliceOfflineList[i].YurticiNetPrim;
                            model.ToplamAedYurticiBrutPrim += model.procedurePoliceOfflineList[i].YurticiBrutPrim;
                            model.ToplamAedBsmv += model.procedurePoliceOfflineList[i].Bsmv;

                        }

                    }
                }

            }
            if (model.procedureTeklifOfflineList != null)
            {
                if (model.procedureTeklifOfflineList.Count > 0)
                {
                    for (int i = model.procedureTeklifOfflineList.Count - 1; i >= 0; i--)
                    {

                        model.GenelPolToplamSayac += 1;


                        if (model.procedureTeklifOfflineList[i].YurticiBrutPrimTL > 0)
                        {
                            //tahakkuk toplam
                            model.ToplamTeminatTutariTLTahakkuk += model.procedureTeklifOfflineList[i].TeminatTutariTL;
                            model.ToplamYurtdisiPrimTLTahakkuk += model.procedureTeklifOfflineList[i].YurtdisiPrimTL;
                            model.ToplamYurtdisiDisKaynakKomisyonTLTahakkuk += model.procedureTeklifOfflineList[i].YurtdisiDisKaynakKomisyonTL;
                            model.ToplamYurtdisiAlinanKomisyonTLTahakkuk += model.procedureTeklifOfflineList[i].YurtdisiAlinanKomisyonTL;
                            model.ToplamFrontingSigortaSirketiKomisyonTLTahakkuk += model.procedureTeklifOfflineList[i].FrontingSigortaSirketiKomisyonTL;
                            model.ToplamSatisKanaliKomisyonTLTahakkuk += model.procedureTeklifOfflineList[i].SatisKanaliKomisyonTL;
                            model.ToplamYurticiAlinanKomisyonTLTahakkuk += model.procedureTeklifOfflineList[i].YurticiAlinanKomisyonTL;
                            model.ToplamYurtdisiNetPrimTLTahakkuk += model.procedureTeklifOfflineList[i].YurtdisiNetPrimTL;
                            model.ToplamYurtdisiBrokerNetPrimTLTahakkuk += model.procedureTeklifOfflineList[i].YurtdisiBrokerNetPrimTL;
                            model.ToplamYurticiNetPrimTLTahakkuk += model.procedureTeklifOfflineList[i].YurticiNetPrimTL;
                            model.ToplamYurticiBrutPrimTLTahakkuk += model.procedureTeklifOfflineList[i].YurticiBrutPrimTL;
                            model.ToplamBsmvTLTahakkuk += model.procedureTeklifOfflineList[i].BsmvTL;
                        }
                        else
                        {
                            //iptal toplam 
                            model.ToplamTeminatTutariTLIptal += model.procedureTeklifOfflineList[i].TeminatTutariTL;
                            model.ToplamYurtdisiPrimTLIptal += model.procedureTeklifOfflineList[i].YurtdisiPrimTL;
                            model.ToplamYurtdisiDisKaynakKomisyonTLIptal += model.procedureTeklifOfflineList[i].YurtdisiDisKaynakKomisyonTL;
                            model.ToplamYurtdisiAlinanKomisyonTLIptal += model.procedureTeklifOfflineList[i].YurtdisiAlinanKomisyonTL;
                            model.ToplamFrontingSigortaSirketiKomisyonTLIptal += model.procedureTeklifOfflineList[i].FrontingSigortaSirketiKomisyonTL;
                            model.ToplamSatisKanaliKomisyonTLIptal += model.procedureTeklifOfflineList[i].SatisKanaliKomisyonTL;
                            model.ToplamYurticiAlinanKomisyonTLIptal += model.procedureTeklifOfflineList[i].YurticiAlinanKomisyonTL;
                            model.ToplamYurtdisiNetPrimTLIptal += model.procedureTeklifOfflineList[i].YurtdisiNetPrimTL;
                            model.ToplamYurtdisiBrokerNetPrimTLIptal += model.procedureTeklifOfflineList[i].YurtdisiBrokerNetPrimTL;
                            model.ToplamYurticiNetPrimTLIptal += model.procedureTeklifOfflineList[i].YurticiNetPrimTL;
                            model.ToplamYurticiBrutPrimTLIptal += model.procedureTeklifOfflineList[i].YurticiBrutPrimTL;
                            model.ToplamBsmvTLIptal += model.procedureTeklifOfflineList[i].BsmvTL;

                        }

                        // dolar
                        if (model.procedureTeklifOfflineList[i].DovizTuru == "USD")
                        {
                            model.ToplamDolarTeminatTutari += model.procedureTeklifOfflineList[i].TeminatTutari;
                            model.ToplamDolarYurtdisiPrim += model.procedureTeklifOfflineList[i].YurtdisiPrim;
                            model.ToplamDolarYurtdisiDisKaynakKomisyon += model.procedureTeklifOfflineList[i].YurtdisiDisKaynakKomisyon;
                            model.ToplamDolarYurtdisiAlinanKomisyon += model.procedureTeklifOfflineList[i].YurtdisiAlinanKomisyon;
                            model.ToplamDolarFrontingSigortaSirketiKomisyon += model.procedureTeklifOfflineList[i].FrontingSigortaSirketiKomisyon;
                            model.ToplamDolarSatisKanaliKomisyon += model.procedureTeklifOfflineList[i].SatisKanaliKomisyon;
                            model.ToplamDolarYurticiAlinanKomisyon += model.procedureTeklifOfflineList[i].YurticiAlinanKomisyon;
                            model.ToplamDolarYurtdisiNetPrim += model.procedureTeklifOfflineList[i].YurtdisiNetPrim;
                            model.ToplamDolarYurtdisiBrokerNetPrim += model.procedureTeklifOfflineList[i].YurtdisiBrokerNetPrim;
                            model.ToplamDolarYurticiNetPrim += model.procedureTeklifOfflineList[i].YurticiNetPrim;
                            model.ToplamDolarYurticiBrutPrim += model.procedureTeklifOfflineList[i].YurticiBrutPrim;
                            model.ToplamDolarBsmv += model.procedureTeklifOfflineList[i].Bsmv;

                        }
                        //euro
                        if (model.procedureTeklifOfflineList[i].DovizTuru == "EUR")
                        {

                            model.ToplamEuroTeminatTutari += model.procedureTeklifOfflineList[i].TeminatTutari;
                            model.ToplamEuroYurtdisiPrim += model.procedureTeklifOfflineList[i].YurtdisiPrim;
                            model.ToplamEuroYurtdisiDisKaynakKomisyon += model.procedureTeklifOfflineList[i].YurtdisiDisKaynakKomisyon;
                            model.ToplamEuroYurtdisiAlinanKomisyon += model.procedureTeklifOfflineList[i].YurtdisiAlinanKomisyon;
                            model.ToplamEuroFrontingSigortaSirketiKomisyon += model.procedureTeklifOfflineList[i].FrontingSigortaSirketiKomisyon;
                            model.ToplamEuroSatisKanaliKomisyon += model.procedureTeklifOfflineList[i].SatisKanaliKomisyon;
                            model.ToplamEuroYurticiAlinanKomisyon += model.procedureTeklifOfflineList[i].YurticiAlinanKomisyon;
                            model.ToplamEuroYurtdisiNetPrim += model.procedureTeklifOfflineList[i].YurtdisiNetPrim;
                            model.ToplamEuroYurtdisiBrokerNetPrim += model.procedureTeklifOfflineList[i].YurtdisiBrokerNetPrim;
                            model.ToplamEuroYurticiNetPrim += model.procedureTeklifOfflineList[i].YurticiNetPrim;
                            model.ToplamEuroYurticiBrutPrim += model.procedureTeklifOfflineList[i].YurticiBrutPrim;
                            model.ToplamEuroBsmv += model.procedureTeklifOfflineList[i].Bsmv;


                        }
                        //aed
                        if (model.procedureTeklifOfflineList[i].DovizTuru == "AED")
                        {

                            model.ToplamAedTeminatTutari += model.procedureTeklifOfflineList[i].TeminatTutari;
                            model.ToplamAedYurtdisiPrim += model.procedureTeklifOfflineList[i].YurtdisiPrim;
                            model.ToplamAedYurtdisiDisKaynakKomisyon += model.procedureTeklifOfflineList[i].YurtdisiDisKaynakKomisyon;
                            model.ToplamAedYurtdisiAlinanKomisyon += model.procedureTeklifOfflineList[i].YurtdisiAlinanKomisyon;
                            model.ToplamAedFrontingSigortaSirketiKomisyon += model.procedureTeklifOfflineList[i].FrontingSigortaSirketiKomisyon;
                            model.ToplamAedSatisKanaliKomisyon += model.procedureTeklifOfflineList[i].SatisKanaliKomisyon;
                            model.ToplamAedYurticiAlinanKomisyon += model.procedureTeklifOfflineList[i].YurticiAlinanKomisyon;
                            model.ToplamAedYurtdisiNetPrim += model.procedureTeklifOfflineList[i].YurtdisiNetPrim;
                            model.ToplamAedYurtdisiBrokerNetPrim += model.procedureTeklifOfflineList[i].YurtdisiBrokerNetPrim;
                            model.ToplamAedYurticiNetPrim += model.procedureTeklifOfflineList[i].YurticiNetPrim;
                            model.ToplamAedYurticiBrutPrim += model.procedureTeklifOfflineList[i].YurticiBrutPrim;
                            model.ToplamAedBsmv += model.procedureTeklifOfflineList[i].Bsmv;
                             
                        }

                    }
                }
            }

            // genel toplam (tahakkuk + iptal)

            model.ToplamTeminatTutariTL = model.ToplamTeminatTutariTLTahakkuk + model.ToplamTeminatTutariTLIptal;
            model.ToplamYurtdisiPrimTL = model.ToplamYurtdisiPrimTLTahakkuk + model.ToplamYurtdisiPrimTLIptal;
            model.ToplamYurtdisiDisKaynakKomisyonTL = model.ToplamYurtdisiDisKaynakKomisyonTLTahakkuk + model.ToplamYurtdisiDisKaynakKomisyonTLIptal;
            model.ToplamYurtdisiAlinanKomisyonTL = model.ToplamYurtdisiAlinanKomisyonTLTahakkuk + model.ToplamYurtdisiAlinanKomisyonTLIptal;
            model.ToplamFrontingSigortaSirketiKomisyonTL = model.ToplamFrontingSigortaSirketiKomisyonTLTahakkuk + model.ToplamFrontingSigortaSirketiKomisyonTLIptal;
            model.ToplamSatisKanaliKomisyonTL = model.ToplamSatisKanaliKomisyonTLTahakkuk + model.ToplamSatisKanaliKomisyonTLIptal;
            model.ToplamYurticiAlinanKomisyonTL = model.ToplamYurticiAlinanKomisyonTLTahakkuk + model.ToplamYurticiAlinanKomisyonTLIptal;
            model.ToplamYurtdisiNetPrimTL = model.ToplamYurtdisiNetPrimTLTahakkuk + model.ToplamYurtdisiNetPrimTLIptal;
            model.ToplamYurtdisiBrokerNetPrimTL = model.ToplamYurtdisiBrokerNetPrimTLTahakkuk + model.ToplamYurtdisiBrokerNetPrimTLIptal;
            model.ToplamYurticiNetPrimTL = model.ToplamYurticiNetPrimTLTahakkuk + model.ToplamYurticiNetPrimTLIptal;
            model.ToplamYurticiBrutPrimTL = model.ToplamYurticiBrutPrimTLTahakkuk + model.ToplamYurticiBrutPrimTLIptal;
            model.ToplamBsmvTL = model.ToplamBsmvTLTahakkuk + model.ToplamBsmvTLIptal;

            if (_AktifKullaniciService.Gorevi == 4)
            {
                ViewBag.AnaTVM = false;
            }
            else
            {
                ViewBag.AnaTVM = true;
            }
            return View(model);
        }

        [AjaxException]
        [Authorization(AnaMenuKodu = AnaMenuler.Police, AltMenuKodu = AltMenuler.PoliceTeklifListesi, SekmeKodu = 0)]
        public ActionResult GetUnderwritersPartial(string teklifOrPoliceID, string durum)
        {
            UnderwritersPartialView policeModel = new UnderwritersPartialView();
            try
            {
                if (durum == "police")
                {
                    var reasurorGenel = _TeklifService.getReasurorGenel(teklifOrPoliceID, "");
                    if (reasurorGenel != null)
                    {
                        policeModel.Aciklama = reasurorGenel.Aciklama;
                    }
                    var uws = _TeklifService.getUnderwriters(teklifOrPoliceID, "");
                    if (uws.Count > 0)
                    {
                        policeModel.UnderwriterSatirlar = new List<UnderwriterModel>();
                        UnderwriterModel uwm = new UnderwriterModel();
                        for (int i = 0; i < uws.Count; i++)
                        {
                            uwm = new UnderwriterModel();
                            uwm.UnderwriterAdi = uws[i].UnderwriterAdi;
                            uwm.UnderwriterPayOrani = uws[i].UnderwriterPayOrani;
                            policeModel.UnderwriterSatirlar.Add(uwm);
                        }

                    }

                }
                else if (durum == "teklif")
                {
                    var reasurorGenel = _TeklifService.getReasurorGenel("", teklifOrPoliceID);
                    if (reasurorGenel != null)
                    {
                        policeModel.Aciklama = reasurorGenel.Aciklama;
                    }
                    var uws = _TeklifService.getUnderwriters("", teklifOrPoliceID);
                    if (uws.Count > 0)
                    {
                        policeModel.UnderwriterSatirlar = new List<UnderwriterModel>();
                        UnderwriterModel uwm = new UnderwriterModel();
                        for (int i = 0; i < uws.Count; i++)
                        {
                            uwm = new UnderwriterModel();
                            uwm.UnderwriterAdi = uws[i].UnderwriterAdi;
                            uwm.UnderwriterPayOrani = uws[i].UnderwriterPayOrani;
                            policeModel.UnderwriterSatirlar.Add(uwm);
                        }

                    }
                }
                else if (durum == "policeeki")
                {
                    var reasurorGenel = _TeklifService.getReasurorGenel(teklifOrPoliceID, "");
                    if (reasurorGenel != null)
                    {
                        policeModel.Aciklama = reasurorGenel.Aciklama;
                    }
                    var uws = _TeklifService.getUnderwriters(teklifOrPoliceID, "");
                    if (uws.Count > 0)
                    {
                        policeModel.UnderwriterSatirlar = new List<UnderwriterModel>();
                        UnderwriterModel uwm = new UnderwriterModel();
                        for (int i = 0; i < uws.Count; i++)
                        {
                            uwm = new UnderwriterModel();
                            uwm.UnderwriterAdi = uws[i].UnderwriterAdi;
                            uwm.UnderwriterPayOrani = uws[i].UnderwriterPayOrani;
                            policeModel.UnderwriterSatirlar.Add(uwm);
                        }

                    }

                }


            }
            catch (Exception ex)
            {
                return PartialView("_underwriters", policeModel);
            }
            return PartialView("_underwriters", policeModel);
        }

        #endregion
        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OfflineRaporlar, SekmeKodu = AltMenuSekmeler.VadeTakipRaporu)]
        public ActionResult VadeTakipRaporu()
        {
            VadeTakipRaporuModel model = new VadeTakipRaporuModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;

            model.RaporSonuc = new List<VadeTakipRaporuProcedureModel>();

            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();

            // List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TVMKodu);
            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TvmTipi.ToString());
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");
            // model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");

            // List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            List<TVMOzetModel> tvmlist = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani");
            model.TarihTipleri = new SelectList(TarihAraliklari.TarihAraliklariList(), "Value", "Text", "").ListWithOptionLabel();

            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OfflineRaporlar, SekmeKodu = AltMenuSekmeler.VadeTakipRaporu)]
        [HttpPost]
        public ActionResult ListePagerVadeTakipRaporu()
        {
            if (Request["sEcho"] != null)
            {
                VadeTakipRaporuListe arama = new VadeTakipRaporuListe(Request, new Expression<Func<VadeTakipRaporuProcedureModel, object>>[]
                                                                    {
                                                                        t => t.PoliceId,
                                                                        t => t.PoliceNo,
                                                                        t => t.BransAdi,
                                                                        t => t.TUM,
                                                                        t => t.YenilemeNo,
                                                                        t => t.EkNo,
                                                                        t => t.TanzimTarihi,
                                                                        t => t.BaslamaTarihi,
                                                                        t => t.BitisTarihi,
                                                                        t => t.OzelAlan,
                                                                        t => t.SigortaliAdi,
                                                                        t => t.SigortaEttirenAdi,
                                                                        t => t.TaksitSayisi,
                                                                        t => t.PoliceSuresi,
                                                                        t => t.TVM,
                                                                        t => t.TcknVkn,
                                                                        t => t.OdemeSekli,
                                                                        t => t.OdemeTipi,
                                                                        t => t.MuhasebeKodu,
                                                                        t => t.Yeni_is,
                                                                        });


              
                arama.DovizTL = arama.TryParseParamInt("DovizTL"); 
                //arama.BransList = arama.TryParseParamString("BransSelectList");
                arama.UrunList = arama.TryParseParamString("UrunSelectList");
                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                arama.TahIpt = arama.TryParseParamByte("TahsIptal");
                //arama.Subeler = arama.TryParseParamString("TVMLerSelectList");


                //POST metod parameters 
                arama.BransList = Request.Form["BransSelectList"];
                arama.Subeler = Request.Form["TVMLerSelectList"]; //post metodu

                arama.TarihTipi = arama.TryParseParamByte("TarihTipi");

                if (arama.TarihTipi.HasValue)
                    switch (arama.TarihTipi)
                    {
                        case 1:
                            arama.BaslangicTarihi = TurkeyDateTime.Now;
                            arama.BitisTarihi = TurkeyDateTime.Now.AddDays(7);
                            break;

                        case 2:
                            arama.BaslangicTarihi = TurkeyDateTime.Now;
                            arama.BitisTarihi = TurkeyDateTime.Now.AddDays(15);
                            break;

                        case 3:
                            arama.BaslangicTarihi = TurkeyDateTime.Now;
                            arama.BitisTarihi = TurkeyDateTime.Now.AddDays(30);
                            break;

                        case 4:
                            arama.BaslangicTarihi = TurkeyDateTime.Now;
                            arama.BitisTarihi = TurkeyDateTime.Now.AddDays(45);
                            break;
                    }
                else
                {
                    arama.BaslangicTarihi = TurkeyDateTime.Now;
                    arama.BitisTarihi = TurkeyDateTime.Now.AddYears(20);
                }


                int totalRowCount = 0;
                List<VadeTakipRaporuProcedureModel> list = _RaporService.VadeTakipRaporuPagedList(arama, out totalRowCount);
                List<int> polIsList = new List<int>();
                for (int i = 0; i < list.Count; i++)
                {
                    polIsList.Add(list[i].PoliceId);
                }
                var atananPoliceListesi = _RaporService.AtananIsListesi(polIsList);


                arama.AddFormatter(f => f.SigortaliAdi, f => String.Format("{0}{1}", f.SigortaliAdi, f.SigortaliSoyadi));
                arama.AddFormatter(f => f.SigortaEttirenAdi, f => String.Format("{0}{1}", f.SigortaEttirenAdi, f.SigortaEttirenSoyadi));
                arama.AddFormatter(f => f.BaslamaTarihi, f => String.Format("{0}", f.BaslamaTarihi.HasValue ? f.BaslamaTarihi.Value.ToString("dd.MM.yyyy") : ""));
                arama.AddFormatter(f => f.BitisTarihi, f => f.BitisTarihi.Value == DateTime.Today ? String.Format("<span style='font-weight:bold; background-color:red; color:white;'>{0}</span>", f.BitisTarihi.Value.ToString("dd.MM.yyyy")) : f.BitisTarihi.Value.ToString("dd.MM.yyyy"));

                arama.AddFormatter(f => f.Yeni_is, f => f.Yeni_is == 1 ? String.Format("<span style='text-align:center'> <label><img src='~/Content/img/yesiltik.png'  style='width: 10% '/></label> </span>") : "");

                arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.HasValue ? f.TanzimTarihi.Value.ToString("dd.MM.yyyy") : ""));

                arama.AddFormatter(f => f.PoliceId, f => (atananPoliceListesi.Contains(f.PoliceId) ? String.Format("<span style='color:red'>Atanmış</span>") : String.Format("<input type='checkbox' class='policeListesi' id='{0}' policeNo='{1}' yenilemeNo='{2}' ekNo='{3}' bransKodu='{4}' tvmKodu='{5}'  policeBitisTarihi='{6}' sigortaSirketKodu='{7}'/>",
                                    f.PoliceId, f.PoliceNo, f.YenilemeNo, f.EkNo, f.BransKodu, f.TcknVkn, f.BitisTarihi, f.SigortaSirketKodu)));


                arama.AddFormatter(f => f.PoliceNo, f => String.Format("<a href='/Rapor/Rapor/GetPoliceDetail/{0}' target ='_blank'>{1}</a>", f.PoliceId, f.PoliceNo));
                arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", GetOzelAlan(s.PoliceId)));

                //arama.AddFormatter(s => s.SatisTuru, s => String.Format("{0}", SatisTurleri.SatisTuru(s.SatisTuru)));

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult GetPoliceDetail(int? id)
        {
            VadeTakipRaporuModel model = new VadeTakipRaporuModel();

            model.RaporSonuc = new List<VadeTakipRaporuProcedureModel>();
            //model.TVMKodu = _AktifKullaniciService.TVMKodu;
            //model.TvmUnvani = _AktifKullaniciService.TVMUnvani;
            ITVMService _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            _TvmService = DependencyResolver.Current.GetService<ITVMService>();
            bool tvmTaliVar = _TvmService.TvmTaliVarMi(_AktifKullaniciService.TVMKodu);
            int KontrolTVMKod = _TvmService.GetDetay(_AktifKullaniciService.TVMKodu).BagliOlduguTVMKodu;


            if (id.HasValue && id > 0 && model.RaporSonuc != null)
            {
                Neosinerji.BABOnlineTP.Business.Police police = _PoliceService.GetPoliceById(id.Value);

                if (police == null)
                    return null;

                // List<TeklifTUMDetayPartialModel> teklifs = _TeklifService.GetAllListTeklif(policeId.Value);
                string tcK = police.GenelBilgiler.PoliceSigortali.KimlikNo;
                //if (!String.IsNullOrEmpty(tcK))
                //{
                //    police.GenelBilgiler.PoliceSigortali.KimlikNo = tcK.Substring(0, 2) + "*******" + tcK.Substring(9, 2);
                //}

                string vkn = police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                //if (!String.IsNullOrEmpty(vkn))
                //{
                //    police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = vkn.Substring(0, 2) + "*******" + vkn.Substring(8, 2);
                //}

                string tcKSEttiren = police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                //if (!String.IsNullOrEmpty(tcKSEttiren))
                //{
                //    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = tcKSEttiren.Substring(0, 2) + "*******" + tcKSEttiren.Substring(9, 2);
                //}

                string vknSEttiren = police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                //if (!String.IsNullOrEmpty(vknSEttiren))
                //{
                //    police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = vknSEttiren.Substring(0, 2) + "*******" + vknSEttiren.Substring(8, 2);
                //}


                string sasi = police.GenelBilgiler.PoliceArac.SasiNo;
                //if (!String.IsNullOrEmpty(sasi))
                //{
                //    police.GenelBilgiler.PoliceArac.SasiNo = sasi.Substring(0, 3) + "******";
                //}

                string motono = police.GenelBilgiler.PoliceArac.MotorNo;
                //if (!string.IsNullOrEmpty(motono))
                //{
                //    police.GenelBilgiler.PoliceArac.MotorNo = motono.Substring(0, 3) + "******";
                //}
                return View(police);

            }
            return null;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.KrediliHayatPoliceAktar, SekmeKodu = 0)]
        public ActionResult KrediliHayatPoliceRaporu()
        {
            KrediliHayatPoliceRaporuModel model = new KrediliHayatPoliceRaporuModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;


            model.RaporSonuc = new List<PoliceRaporProcedureModel>();

            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();


            model.OdemeTipleri = new SelectList(OdemeTipleri.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.OdemeSekilleri = new SelectList(OdemeSekilleri.OdemeSekilleriList(), "Value", "Text", "").ListWithOptionLabel();

            List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            model.TVMLerItems = new MultiSelectList(tvmlist, "Kodu", "Unvani");


            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.KrediliHayatPoliceAktar, SekmeKodu = 0)]
        public ActionResult ListePagerKrediliHayatPoliceRapor()
        {
            if (Request["sEcho"] != null)
            {
                KrediliHayatPoliceRaporListe arama = new KrediliHayatPoliceRaporListe(Request, new Expression<Func<KrediliHayatPoliceRaporProcedureModel, object>>[]
                                                                    {
                                                                        t =>t.TUMPoliceNo,
                                                                        t =>t.TVM,
                                                                        t =>t.SigortaliKimlikNo,
                                                                        t =>t.SogrataliAdSoyad,
                                                                        t =>t.DogumTarihi,
                                                                        t =>t.Cinsiyet,
                                                                        t =>t.BabaAdi,
                                                                        t =>t.Adres,
                                                                        t =>t.PostaKodu,
                                                                        t =>t.Numara,
                                                                        t =>t.KrediBaslangicTarihi,
                                                                        t =>t.KrediBitisTarihi,
                                                                        t =>t.KrediVade,
                                                                        t =>t.KrediTutari,
                                                                        t =>t.ParaBirimi,
                                                                        t =>t.KrediMiktarKarsiligi,
                                                                        t =>t.TeminatMiktari,
                                                                        t =>t.Primi,
                                                                        //t =>t.VergiDairesi,
                                                                        //t =>t.VerdiNumarasi,
                                                                        t =>t.Uyruk,
                                                                        t =>t.Meslek,
                                                                        t =>t.TanzimTarihi,
                                                                         t =>t.PoliceNo,
                                                                          t =>t.OdemeSekli,
                                                                          t =>t.OdemeTipi

                                                                    });


                arama.PoliceTarihi = arama.TryParseParamByte("PoliceTarihi");
                arama.DovizTL = arama.TryParseParamInt("DovizTL");
                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                arama.TahIpt = arama.TryParseParamByte("TahsIptal");
                arama.Subeler = arama.TryParseParamString("TVMLerSelectList");

                arama.PoliceNo = arama.TryParseParamString("PoliceNo");
                arama.OdemeSekli = arama.TryParseParamByte("OdemeSekli");
                arama.OdemeTipi = arama.TryParseParamByte("OdemeTipi");

                int totalRowCount = 0;
                List<KrediliHayatPoliceRaporProcedureModel> list = _RaporService.KrediliHayatPoliceRaporPagedList(arama, out totalRowCount);


                arama.AddFormatter(f => f.Cinsiyet, f => String.Format("{0}", CinsiyetTipleri.CinsiyetTipi(f.Cinsiyet)));
                arama.AddFormatter(f => f.KrediVade, f => String.Format("{0} {1}", f.KrediVade, babonline.Year));
                arama.AddFormatter(f => f.DogumTarihi, f => String.Format("{0}", f.DogumTarihi.HasValue ? f.DogumTarihi.Value.ToString("dd.MM.yyyy") : String.Empty));
                arama.AddFormatter(f => f.KrediBaslangicTarihi, f => String.Format("{0}", f.KrediBaslangicTarihi.HasValue ?
                                                                 f.KrediBaslangicTarihi.Value.ToString("dd.MM.yyyy") : String.Empty));
                arama.AddFormatter(f => f.KrediBitisTarihi, f => String.Format("{0}", f.KrediBitisTarihi.HasValue ?
                                                            f.KrediBitisTarihi.Value.ToString("dd.MM.yyyy") : String.Empty));

                arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.HasValue ?
                                                        f.TanzimTarihi.Value.ToString("dd.MM.yyyy") : String.Empty));

                arama.AddFormatter(f => f.Uyruk, f => String.Format("{0}", UyrukTipleri.UyrukTipi(f.Uyruk)));
                arama.AddFormatter(f => f.ParaBirimi, f => String.Format("{0}", DovizTLTipleri.DovizTLTip(f.ParaBirimi)));

                arama.AddFormatter(f => f.TUMPoliceNo, f => String.Format("<a href='/Teklif/KrediliHayat/Police/{0}'>{1}</a> {2}",
                                                                            f.TeklifId, f.TUMPoliceNo, f.PDFDosyasi == null ? "" :
                                                                            "<a href=" + f.PDFDosyasi + " title='Poliçe PDF' target='_blank' class='pull-right'>" +
                                                                            "<img src='/content/img/pdf_icon.png' /></a>"));


                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.TeklifRaporu)]
        public ActionResult TeklifRaporu()
        {
            if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Aegon)
            {
                return RedirectToAction("TeklifRaporu", "AegonRapor");
            }

            TeklifRaporuModel model = new TeklifRaporuModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;

            model.RaporSonuc = new List<TeklifRaporuProcedureModel>();

            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();

            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TVMKodu);
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");
            model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");

            // List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            List<TVMOzetModel> tvmlist = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMLerItems = new MultiSelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani");


            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.TeklifRaporu)]
        public ActionResult ListePagerTeklifRaporu()
        {
            if (Request["sEcho"] != null)
            {
                TeklifRaporuListe arama = new TeklifRaporuListe(Request, new Expression<Func<TeklifRaporuProcedureModel, object>>[]
                                                                    {
                                                                        t =>t.TeklifNo,
                                                                        t =>t.AdiSoyadi,
                                                                        t =>t.UrunAdi,
                                                                        t =>t.TanzimTarihi,
                                                                        t =>t.BitisTarihi,
                                                                        t =>t.OzelAlan,
                                                                        t =>t.Unvani,
                                                                        t =>t.EkleyenKullanici,
                                                                        t =>t.DetailIcon,

                                                                    });


                //arama.PoliceTarihi = arama.TryParseParamByte("PoliceTarihi");
                //tanzim tarihi=1 
                arama.PoliceTarihi = 1;

                // arama.DovizTL = arama.TryParseParamInt("DovizTL");
                //hepsi=0
                arama.DovizTL = 0;
                arama.BransList = arama.TryParseParamString("BransSelectList");
                arama.UrunList = arama.TryParseParamString("UrunSelectList");

                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");

                // arama.TahIpt = arama.TryParseParamByte("TahsIptal");
                //hepsi=0
                arama.TahIpt = 0;
                arama.Subeler = arama.TryParseParamString("TVMLerSelectList");

                arama.TeklifNo = arama.TryParseParamInt("TeklifNo");

                int totalRowCount = 0;
                List<TeklifRaporuProcedureModel> list = _RaporService.TeklifRaporuPagedList(arama, out totalRowCount);


                arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a> {3}", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu),
                                                     f.TeklifId, f.TeklifNo, f.PDFDosyasi == null ? "" :
                                                     "<a href=" + f.PDFDosyasi + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                                                     "<img src='/content/img/pdf_icon.png' /></a>"));
                arama.AddFormatter(s => s.DetailIcon, s => String.Format("<a href='javascript:;' class='see-teklif-detail' teklif-id='" + s.TeklifId + "'>" +
                                                                        "<img src='/Content/img/icon-details.png'/></a>"));

                arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", GetOzelAlan(s.TeklifId)));
                arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.HasValue ? f.TanzimTarihi.Value.ToString("dd.MM.yyyy") : ""));

                arama.AddFormatter(f => f.BitisTarihi, f => String.Format("{0}", f.BitisTarihi.HasValue ? f.BitisTarihi.Value.ToString("dd.MM.yyyy") : ""));

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.OzetRapor)]
        public ActionResult OzetRapor()
        {
            OzetRaporModel model = new OzetRaporModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;

            model.OdemeTipleri = new SelectList(OdemeTipleri.OdemeTipleriList(), "Value", "Text", "").ListWithOptionLabel();
            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();

            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TVMKodu);
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi");
            model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");

            //List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            List<TVMOzetModel> tvmlist = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMLerItems = new MultiSelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.RaporSonuc = new List<OzetRaporProcedureModel>();
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.OzetRapor)]
        public ActionResult ListePagerOzetRaporu()
        {
            if (Request["sEcho"] != null)
            {
                OzetRaporListe arama = new OzetRaporListe(Request, new Expression<Func<OzetRaporProcedureModel, object>>[]
                                                                    {
                                                                        t => t.Kodu,
                                                                        t => t.Adi,
                                                                        t => t.BrutPrim,
                                                                        t => t.NetKomisyon,
                                                                        t => t.NetPrim,
                                                                        t => t.PoliceAdedi,
                                                                        t => t.ToplamKomisyon,
                                                                        t => t.TaliKomisyon,
                                                                    });


                arama.UrunList = arama.TryParseParamString("UrunSelectList");
                arama.BransList = arama.TryParseParamString("BransSelectList");
                arama.PoliceTarihi = arama.TryParseParamInt("PoliceTarihi");
                arama.OdemeTipi = arama.TryParseParamInt("OdemeTipi");

                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");

                //hepsi=0;
                //arama.TahIpt = arama.TryParseParamInt("TahsIptal");
                //arama.DovizTL = arama.TryParseParamInt("DovizTL");
                arama.TahIpt = 0;
                arama.DovizTL = 0;

                arama.aramakriteri = arama.TryParseParamInt("aramakriteri");
                arama.asildeger = arama.TryParseParamInt("asildeger");

                arama.data2 = arama.TryParseParamInt("data2");
                arama.data3 = arama.TryParseParamString("data3");

                //Binlik ayraç konuyor.
                arama.AddFormatter(s => s.BrutPrim, s => String.Format("{0}", BinlikAyracEkle(s.BrutPrim ?? 0)));
                arama.AddFormatter(s => s.NetPrim, s => String.Format("{0}", BinlikAyracEkle(s.NetPrim ?? 0)));
                arama.AddFormatter(s => s.ToplamKomisyon, s => String.Format("{0}", BinlikAyracEkle(s.ToplamKomisyon ?? 0)));
                arama.AddFormatter(s => s.TaliKomisyon, s => String.Format("{0}", BinlikAyracEkle(s.TaliKomisyon ?? 0)));
                arama.AddFormatter(s => s.NetKomisyon, s => String.Format("{0}", BinlikAyracEkle(s.NetKomisyon ?? 0)));

                int totalRowCount = 0;
                List<OzetRaporProcedureModel> list = _RaporService.OzetRaporPagedList(arama, out totalRowCount);

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.AracPoliceIstatistik)]
        public ActionResult AracSigortalariIstatistikRaporu()
        {

            AracSigortalariIstatistikRaporModel model = new AracSigortalariIstatistikRaporModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;

            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;

            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", "0").ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", "0").ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", "0").ToList();

            //model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");
            if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
            {
                model.UrunlerItems = new SelectList(RaporListProvider.GetMapfreUrunList(), "Value", "Text", "0").ToList();
            }
            else
            {
                model.UrunlerItems = new SelectList(RaporListProvider.GetUrunList(), "Value", "Text", "0").ToList();
            }

            // List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            List<TVMOzetModel> tvmlist = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMLerItems = new MultiSelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani");

            model.RaporSonuc = new List<AracSigortalariIstatistikRaporuProcedureModel>();
            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.AracPoliceIstatistik)]
        public ActionResult ListePagerAracSigortalariIstatistikRaporu()
        {

            if (Request["sEcho"] != null)
            {
                AracSigortalariIstatistikRaporuListe arama = new AracSigortalariIstatistikRaporuListe(Request, new Expression<Func<AracSigortalariIstatistikRaporuProcedureModel, object>>[]
                                                                    {
                                                                        t => t.Kodu,
                                                                        t => t.Adi,
                                                                        t => t.BrutPrim,
                                                                        t => t.NetKomisyon,
                                                                        t => t.NetPrim,
                                                                        t => t.PoliceAdedi,
                                                                        t => t.ToplamKomisyon,
                                                                        t => t.TaliKomisyon,
                                                                    });


                arama.PoliceTarihi = arama.TryParseParamInt("PoliceTarihi");
                arama.DovizTL = arama.TryParseParamInt("DovizTL");
                arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                arama.Urun = arama.TryParseParamInt("Urun");
                arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                arama.TahIpt = arama.TryParseParamInt("TahsIptal");
                arama.customvalue = arama.TryParseParamInt("customvalue");
                arama.kodu = arama.TryParseParamInt("kodu");
                arama.data2 = arama.TryParseParamInt("data2");
                arama.sorguturu = arama.TryParseParamString("sorguturu");

                //Binlik ayraç konuyor.
                arama.AddFormatter(s => s.BrutPrim, s => String.Format("{0}", BinlikAyracEkle(s.BrutPrim ?? 0)));
                arama.AddFormatter(s => s.NetPrim, s => String.Format("{0}", BinlikAyracEkle(s.NetPrim ?? 0)));
                arama.AddFormatter(s => s.ToplamKomisyon, s => String.Format("{0}", BinlikAyracEkle(s.ToplamKomisyon ?? 0)));
                arama.AddFormatter(s => s.TaliKomisyon, s => String.Format("{0}", BinlikAyracEkle(s.TaliKomisyon ?? 0)));
                arama.AddFormatter(s => s.NetKomisyon, s => String.Format("{0}", BinlikAyracEkle(s.NetKomisyon ?? 0)));

                int totalRowCount = 0;
                List<AracSigortalariIstatistikRaporuProcedureModel> list = _RaporService.AracSigortaliIstatistikRaporPagedList(arama, out totalRowCount);

                DataTableList result = arama.Prepare(list, totalRowCount);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [HttpPost]
        public ActionResult PerformanMusteriSayilari(DateTime tarih1, DateTime tarih2)
        {
            PerformansSayilariModel model = new PerformansSayilariModel();
            return Json(model);
        }

        public static string GetOzelAlan(int teklifid)
        {
            string result = String.Empty;
            try
            {
                ITeklifService _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
                TeklifOzelAlan ozelAlan = _TeklifService.TeklifOzelAlan(teklifid);

                if (ozelAlan != null)
                {
                    switch (ozelAlan.UrunKodu)
                    {
                        case UrunKodlari.TrafikSigortasi:
                        case UrunKodlari.KaskoSigortasi:
                        case UrunKodlari.MapfreKasko:
                        case UrunKodlari.MapfreTrafik:
                        case UrunKodlari.IkinciElGaranti: result = babonline.LicenceNumber + " : " + ozelAlan.OzelAlan; break;
                        case UrunKodlari.DogalAfetSigortasi_Deprem: result = babonline.City + " : " + ozelAlan.OzelAlan; break;
                        case UrunKodlari.KrediHayat: result = babonline.CreditType + " : " + ozelAlan.OzelAlan; break;
                        //case UrunKodlari.YurtDisiSeyehatSaglik: result = babonline.Country + " : " + ozelAlan.OzelAlan; break;
                        case UrunKodlari.YurtDisiSeyehatSaglik: result = ozelAlan.OzelAlan.ToString(); break;
                        case UrunKodlari.KonutSigortasi_Paket:
                        case UrunKodlari.IsYeri: result = babonline.City + " : " + ozelAlan.OzelAlan; break;
                    }
                }
            }
            catch (Exception e)
            {

            }
            return result;
        }

        private static string BinlikAyracEkle(decimal tutar)
        {
            string eskiTutar = tutar.ToString();
            int length = eskiTutar.Length;
            string result = "";
            int sayac = 0;

            if (length > 6)
            {
                for (int i = 0; i < length; i++)
                {
                    sayac++;
                    result += eskiTutar[i];
                    if (sayac == (length - 6) || sayac == (length - 9) || sayac == (length - 12))
                        result += ",";
                }

                return result;
            }
            else return eskiTutar;
        }

        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.OfflinePoliceAktar, SekmeKodu = 0)]
        public ActionResult OfflinePoliceAktar()
        {

            return View();
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.OfflinePoliceAktar, SekmeKodu = 0)]
        public ActionResult OfflinePoliceAktar(HttpPostedFileBase file)
        {

            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    var path = Path.Combine(Server.MapPath("~/Files"), file.FileName);
                    file.SaveAs(path);

                    return RedirectToAction("OfflinePoliceKontrol", "Rapor", new { path = path });
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            ModelState.AddModelError("", "Lütfen uygun formatta bir dosya seçiniz");
            return View();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.OfflinePoliceAktar, SekmeKodu = 0)]
        public ActionResult OfflinePoliceKontrol(string path)
        {

            OfflinePoliceADL adl = new OfflinePoliceADL();
            ExcelOfflinePoliceListModel model = adl.ProcessFile(path);
            model.DosyaAdresi = path;

            return View(model);
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.VeriTransferMerkezi, AltMenuKodu = AltMenuler.OfflinePoliceAktar, SekmeKodu = 0)]
        public ActionResult OfflinePoliceKontrol(ExcelOfflinePoliceListModel model)
        {

            if (!String.IsNullOrEmpty(model.DosyaAdresi))
            {
                OfflinePoliceADL adl = new OfflinePoliceADL();
                bool sonuc = adl.OfflinePoliceKaydet(model.DosyaAdresi);
                if (sonuc)
                    return RedirectToAction("PoliceRaporu", "Rapor");
                else
                {
                    ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Dosya adresi bulunamadı..Lütfen tekrar deneyin.");
                return View(model);
            }
        }

        //Test

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.OzetRapor)]
        public ActionResult OzetRaporTest()
        {
            try
            {
                OzetRaporTestModel model = new OzetRaporTestModel();

                OzetRaporModelDoldur(model);

                model.RaporSonuc = new List<OzetRaporProcedureModel>();
                return View(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        [HttpPost]
        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = AltMenuSekmeler.OzetRapor)]
        public ActionResult OzetRaporTest(OzetRaporTestModel model)
        {
            try
            {
                OzetRaporProcedureRequestModel aramaModel = new OzetRaporProcedureRequestModel();
                model.RaporSonuc = new List<OzetRaporProcedureModel>();


                if (model.XMLModel == null)
                    model.XMLModel = new List<OzetRaporXMLModel>();


                OzetRaporXMLModel mdl = new OzetRaporXMLModel();
                switch (model.AramaKriteri)
                {
                    case OzetRaporAramaKriteri.AnaTali:
                        mdl.Tip = "AT";
                        mdl.Value = "0";
                        model.XMLModel.Add(mdl);
                        break;

                    case OzetRaporAramaKriteri.Sube:
                        mdl.Tip = "SB";
                        mdl.Value = model.BirincilData.ToString();
                        model.XMLModel.Add(mdl);
                        break;

                    case OzetRaporAramaKriteri.Urun:
                        mdl.Tip = "UR";
                        mdl.Value = model.BirincilData.ToString();
                        model.XMLModel.Add(mdl);
                        break;

                    case OzetRaporAramaKriteri.SigortaSirketi:
                        mdl.Tip = "SS";
                        mdl.Value = model.BirincilData.ToString();
                        model.XMLModel.Add(mdl);
                        break;

                    case OzetRaporAramaKriteri.OdemeSecenegi:
                        mdl.Tip = "OS";
                        mdl.Value = model.BirincilData.ToString();
                        model.XMLModel.Add(mdl);
                        break;

                    case OzetRaporAramaKriteri.SatisTuru:
                        mdl.Tip = "ST";
                        mdl.Value = model.BirincilData.ToString();
                        model.XMLModel.Add(mdl);
                        break;

                    case OzetRaporAramaKriteri.MT:
                        mdl.Tip = "MT";
                        mdl.Value = model.BirincilData.ToString();
                        model.XMLModel.Add(mdl);
                        break;

                    case OzetRaporAramaKriteri.Bolge:
                        mdl.Tip = "BG";
                        mdl.Value = model.BirincilData.ToString();
                        model.XMLModel.Add(mdl);
                        break;
                }

                OzetRaporModelDoldur(model);

                model.RaporSonuc = _RaporService.OzetRaporPagedListTest(aramaModel);

                return View(model);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return new RedirectResult("~/Error/ErrorPage/500");
            }
        }

        private void OzetRaporModelDoldur(OzetRaporTestModel model)
        {
            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;

            model.OdemeTipleri = new SelectList(OdemeTipleri.OdemeTipleriList(), "Value", "Text", model.OdemeTipi).ListWithOptionLabel();
            model.TahsilatIptalList = new SelectList(RaporListProvider.GetTahsilatIptalList(), "Value", "Text", model.TahsIptal).ToList();
            model.PoliceTarihiTipleri = new SelectList(RaporListProvider.GetPoliceTarihiTipleri(), "Value", "Text", model.PoliceTarihiTipleri).ToList();
            model.DovizTlList = new SelectList(RaporListProvider.GetDovizTLList(), "Value", "Text", model.DovizTL).ToList();

            List<Bran> brans = _BransService.GetList(_AktifKullaniciService.TVMKodu);
            model.BranslarItems = new MultiSelectList(brans, "BransKodu", "BransAdi", model.BransSelectList);
            model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama", model.UrunSelectList);

            //List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
            List<TVMOzetModel> tvmlist = _TVMService.GetTVMListeKullaniciYetki(0);
            model.TVMLerItems = new MultiSelectList(tvmlist.OrderBy(s => s.Unvani), "Kodu", "Unvani", model.TVMLerSelectList);
        }

        #region İslem Atama
        [AjaxException]
        public ActionResult IsAtama()
        {
            IslemAtamaModel model = new IslemAtamaModel();
            try
            {
                var tvmler = (_TVMService.GetTVMListeKullaniciYetki(0));
                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMSelectList = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
                model.KullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                model.KullanicilarList = new List<SelectListItem>();

                if (tvmler.Count > 0)
                {
                    var kullanicilarlist = (_KullaniciService.GetListKullanicilarTeklifAra());
                    model.KullanicilarList = new SelectList(kullanicilarlist, "KullaniciKodu", "AdiSoyadi", model.KullaniciKodu).ListWithOptionLabel();
                }
                model.OncelikSeviyeleri = new SelectList(RaporListProvider.GetOncelikSeviyeleri(), "Value", "Text", "").ListWithOptionLabel();
            }
            catch (Exception ex)
            {
                model.IslemMesaji = "İşlem sırasında bir hata oluştu." + ex.ToString();
            }

            return PartialView("_IslemAtama", model);
        }

        [HttpPost]
        public ActionResult IsAta(List<IslemPoliceListesi> polList)
        {
            if (polList.Count > 0)
            {
                List<AtananIsler> isList = new List<AtananIsler>();
                AtananIsler isItem = new AtananIsler();
                foreach (var item in polList)
                {
                    isItem = new AtananIsler();
                    isItem.Aciklama = item.Aciklama;
                    isItem.AtamaTarihi = TurkeyDateTime.Now;
                    isItem.BaslamaTarihi = TurkeyDateTime.Now;
                    isItem.TahminiBitisTarihi = Convert.ToDateTime(item.PoliceBitisTarihi);
                    isItem.SigortaSirketKodu = item.SigortaSirketiKodu;
                    isItem.PoliceId = item.PoliceId;
                    isItem.TalepKanaliKodu = 8;//Sistem
                    isItem.PoliceNumarasi = item.PoliceNumarasi;
                    isItem.YenilemeNo = item.YenilemeNo;
                    isItem.EkNo = item.ZeyilNo;
                    isItem.BransKodu = item.BransKodu;
                    isItem.OncelikSeviyesi = item.OncelikSeviyesi;
                    isItem.IsTipi = 1; //Poliçe Yenileme
                    isItem.IsAlanTVMKodu = item.AcenteTVMKodu;
                    isItem.IsAlanKullaniciKodu = item.TVMKullaniciKodu;
                    isItem.IsAtayanTVMKodu = _AktifKullaniciService.TVMKodu;
                    isItem.IsAtayanKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                    isItem.Durum = 1;
                    isList.Add(isItem);
                }
                var isAtandimi = _RaporService.AtananIslerCreate(isList);
                IsAtamaModel returnModel = new IsAtamaModel();
                returnModel.basariliKayitlar = isAtandimi.basariliKayitSayisi;
                returnModel.basarisizKayitlar = isAtandimi.hataliKayitlar.Count;
                return Json(returnModel);
            }

            return View();
        }
        public ActionResult GetTVMKullanici(int tvmKodu)
        {
            var kullaniciList = new SelectList(_KullaniciService.GetTVMKullanicilari(tvmKodu), "KullaniciKodu", "AdiSoyadi").ListWithOptionLabel();
            return Json(new { list = kullaniciList }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        //#region İslem Atama
        //        [AjaxException]
        //        public ActionResult IslemAtama(List<IslemPoliceListesi> policeListesi, int TVMKodu)
        //        {
        //            IslemAtamaModel model = new IslemAtamaModel();
        //            try
        //            {
        //                var tvmler = (_TVMService.GetTVMListeKullaniciYetki(0));
        //                model.TVMKodu = model.TVMKodu;
        //                model.TVMSelectList = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ToList();

        //                model.KullaniciKodu = _AktifKullaniciService.KullaniciKodu;
        //                //List<Database.Models.TVMKullanicilar> Kullanici = _KullaniciService.GetListKullanicilar();
        //                List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
        //                model.KullanicilarList = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();

        //                if (alttvmler.Count > 0)
        //                {
        //                    var kullanicilarlist = (_KullaniciService.GetKullaniciOzet(_AktifKullaniciService.TVMKodu));
        //                    model.KullanicilarList = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();
        //                }
        //                else
        //                    model.TVMSelectList = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ToList();


        //                model.seciliPoliceListesi = policeListesi;


        //            }
        //            catch (Exception ex)
        //            {
        //                model.IslemMesaji = "İşlem sırasında bir hata oluştu." + ex.ToString();
        //            }

        //            return PartialView("_IslemAtama", model);
        //        }


        //        [AjaxException]
        //        public ActionResult IslemAtama(IslemAtamaModel model)
        //        {
        //            IslemPoliceListesi islemItem = new IslemPoliceListesi();

        //            var tvmler = (_TVMService.GetTVMListeKullaniciYetki(0));
        //            model.TVMKodu = model.TVMKodu;
        //            model.TVMSelectList = new SelectList(tvmler, "Kodu", "Unvani", model.TVMKodu).ToList();

        //            model.KullaniciKodu = _AktifKullaniciService.KullaniciKodu;
        //            List<TVMOzetModel> alttvmler = _TVMService.GetTVMListeKullaniciYetki(0);
        //            model.KullanicilarList = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ListWithOptionLabel();

        //            if (alttvmler.Count > 0)
        //            {
        //                var kullanicilarlist = (_KullaniciService.GetKullaniciOzet(_AktifKullaniciService.TVMKodu));
        //                //model.KullanicilarList = new SelectList(
        //                //    ).ListWithOptionLabel();
        //            }
        //            else
        //                model.TVMSelectList = new SelectList(alttvmler, "Kodu", "Unvani", model.TVMKodu).ToList();


        //            //var islemler = _RaporService.VadeTakipRaporuPagedList(VadeTakipRaporuListe arama, out int totalRowCount);

        //            //if (islemler != null)
        //            //{
        //            //    foreach (var item in islemler)
        //            //    {
        //            //        islemItem = new IslemPoliceListesi();
        //            //        islemItem.PoliceNumarasi = islemItem.PoliceNumarasi;
        //            //        islemItem.YenilemeNo = islemItem.YenilemeNo;
        //            //        islemItem.ZeyilNo = islemItem.ZeyilNo;
        //            //        islemItem.UrunKodu = islemItem.UrunKodu;
        //            //        islemItem.TVMKodu = islemItem.TVMKodu;

        //            //        model.seciliPoliceListesi.Add(islemItem);
        //            //    }

        //            //}

        //            return PartialView("_IslemAtama");
        //        }
        //#endregion

        public string CreateDonemselRaporPDF(PoliceListesiOfflineModelim model)
        {
            PDFHelper pdf = null;
            try
            {
                if (model.procedurePoliceOfflineList != null && model.procedurePoliceOfflineList.Count > 0)
                {

                    string rootPath = System.Web.HttpContext.Current.Server.MapPath("/");
                    string template = PdfTemplates.GetTemplate(rootPath + "Content/templates/", PdfTemplates.PoliceListesiDonemselRapor);

                    pdf = new PDFHelper("NeoOnline", "DONEMSEL URETIM RAPORU", "DONEMSEL URETIM RAPORU", 8, rootPath + "Content/fonts/");

                    // ==== Bu method her bir pdf sayfasının footer ekliyor. Poliçelerde Kullanılmaz ==== //
                    pdf.SetPageEventHelper(new PDFCustomEventHelper());
                    pdf.Rotate();
                    PDFParser parser = new PDFParser(template, pdf);
                    string tarihAraligi = model.BaslangicTarihi.Value.ToString("dd.MM.yyyy") + "-" + model.BitisTarihi.Value.ToString("dd.MM.yyyy");
                    parser.SetVariable("$raporTarihAraligi$", tarihAraligi);
                    parser.SetVariable("$tvmUnvani$", model.TVMUnvani);

                    #region Toplam Satırları Police Zeyl 
                    int policeSayac = 0;
                    int zeylSayac = 0;
                    policeSayac += model.procedurePoliceOfflineList.Where(w => (w.SirketAdi == "ALLIANZ SİGORTA A.Ş." && w.EkNo == 1)
                    || (w.SirketAdi != "ALLIANZ SİGORTA A.Ş." && w.EkNo == 0 )).Count();
                    zeylSayac += model.procedurePoliceOfflineList.Where(w => w.SirketAdi != "ALLIANZ SİGORTA A.Ş." && w.EkNo != 0).Count();
                    var allianzDaskListesi = model.procedurePoliceOfflineList.Where(w => w.SirketAdi == "ALLIANZ SİGORTA A.Ş." && w.BransAdi == "DASK" && w.EkNo > 3).Select(s => new
                    {
                        Ekno = s.EkNo
                    }).ToList();

                    foreach (var item in allianzDaskListesi)
                    {
                        if (item.Ekno.ToString().Length > 3)
                        {
                            if (item.Ekno.ToString().Substring(4, 1) == "1")
                            {
                                policeSayac++;
                            }
                            else
                            {
                                zeylSayac++;
                            }
                        }
                    }
                    //Alliazn Dask için eklenecek

                    parser.SetVariable("$PoliceZeylAdedi$", policeSayac.ToString() + "/" + zeylSayac.ToString());
                    #endregion

                    #region Toplam Satırları Tahakkuk

                    decimal? toplamTahakkuk = 0;
                    toplamTahakkuk = model.procedurePoliceOfflineList.Where(w => w.BrutPrim.Value > 0).Sum(s => s.BrutPrim);
                    parser.SetVariable("$ToplamTahakkukBrut$", toplamTahakkuk.Value.ToString("N2"));
                    toplamTahakkuk = model.procedurePoliceOfflineList.Where(w => w.NetPrim.Value > 0).Sum(s => s.NetPrim);
                    parser.SetVariable("$ToplamTahakkukNet$", toplamTahakkuk.Value.ToString("N2"));
                    toplamTahakkuk = model.procedurePoliceOfflineList.Where(w => w.Komisyon.Value > 0).Sum(s => s.Komisyon);
                    parser.SetVariable("$ToplamTahakkukKomisyon$", toplamTahakkuk.Value.ToString("N2"));

                    //tahakkuk KMS%
                    decimal ? tahakkukKMSToplam = 0;
                    decimal? tahakkukNetToplam = 0;
                    var toplamKomisyonYuzdeListe = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "TL").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        NetPrim = s.NetPrim
                    });
                    foreach (var item in toplamKomisyonYuzdeListe)
                    {
                        if (item.NetPrim.HasValue && item.NetPrim.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            tahakkukKMSToplam += item.Komisyon.Value;
                            tahakkukNetToplam += item.NetPrim.Value;
                        }
                    }

                    if (tahakkukNetToplam != 0)
                    {
                        var kmsToplamYuzde = (tahakkukKMSToplam / tahakkukNetToplam) * 100;
                        parser.SetVariable("$ToplamTahakkukKMS%$", kmsToplamYuzde.Value.ToString("N2"));
                    }
                    else
                    {
                        var kmsToplamYuzde = 0;
                        parser.SetVariable("$ToplamTahakkukKMS%$", kmsToplamYuzde.ToString("N2"));
                    }

                    toplamTahakkuk = model.procedurePoliceOfflineList.Where(w => w.TaliKomisyon.Value > 0).Sum(s => s.TaliKomisyon);
                    parser.SetVariable("$ToplamTahakkukTaliKomisyon$", toplamTahakkuk.Value.ToString("N2"));
                    toplamTahakkuk = model.procedurePoliceOfflineList.Where(w => w.Komisyon.Value > 0).Sum(s => s.Komisyon - s.TaliKomisyon);
                    parser.SetVariable("$ToplamTahakkukNetKomisyon$", toplamTahakkuk.Value.ToString("N2"));
                    toplamTahakkuk = model.procedurePoliceOfflineList.Where(w => w.BrutPrim.Value > 0).Sum(s => s.BrutPrim - s.Komisyon);
                    parser.SetVariable("$ToplamTahakkukBorç$", toplamTahakkuk.Value.ToString("N2"));
                    
                    #endregion

                    #region Toplam Satırları Iptal
                    decimal? toplamIptal = 0;
                    toplamIptal = model.procedurePoliceOfflineList.Where(w => w.BrutPrim.Value < 0).Sum(s => s.BrutPrim);
                    parser.SetVariable("$ToplamIptalBrut$", toplamIptal.Value.ToString("N2"));
                    toplamIptal = model.procedurePoliceOfflineList.Where(w => w.NetPrim.Value < 0).Sum(s => s.NetPrim);
                    parser.SetVariable("$ToplamIptalNet$", toplamIptal.Value.ToString("N2"));
                    toplamIptal = model.procedurePoliceOfflineList.Where(w => w.Komisyon.Value < 0).Sum(s => s.Komisyon);
                    parser.SetVariable("$ToplamIptalKomisyon$", toplamIptal.Value.ToString("N2"));

                    //iptalKMS%
                    decimal? iptalKMSToplam = 0;
                    decimal? iptalNetToplam = 0;
                    var toplamiptalKomisyonYuzdeListe = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "TL").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        NetPrim = s.NetPrim
                    });
                    foreach (var item in toplamiptalKomisyonYuzdeListe)
                    {
                        if (item.NetPrim.HasValue && item.NetPrim.Value < 0 && item.Komisyon.HasValue && item.Komisyon.Value < 0)
                        {
                            iptalKMSToplam += item.Komisyon.Value;
                            iptalNetToplam += item.NetPrim.Value;
                        }
                    }

                    if (iptalNetToplam != 0)
                    {
                        var kmsToplamYuzde = (iptalKMSToplam / iptalNetToplam) * 100;
                        parser.SetVariable("$ToplamIptalKMS%$", kmsToplamYuzde.Value.ToString("N2"));
                    }
                    else
                    {
                        var kmsToplamYuzde = 0;
                        parser.SetVariable("$ToplamIptalKMS%$", kmsToplamYuzde.ToString("N2"));
                    }
                    
                    toplamIptal = model.procedurePoliceOfflineList.Where(w => w.TaliKomisyon.Value < 0).Sum(s => s.TaliKomisyon);
                    parser.SetVariable("$ToplamIptalTaliKomisyon$", toplamIptal.Value.ToString("N2"));
                    toplamIptal = model.procedurePoliceOfflineList.Where(w => w.Komisyon.Value < 0).Sum(s => s.Komisyon - s.TaliKomisyon);
                    parser.SetVariable("$ToplamIptalNetKomisyon$", toplamIptal.Value.ToString("N2"));
                    toplamIptal = model.procedurePoliceOfflineList.Where(w => w.BrutPrim.Value < 0).Sum(s => s.BrutPrim - s.Komisyon);
                    parser.SetVariable("$ToplamIptalBorç$", toplamIptal.Value.ToString("N2"));
                    //Alliazn Dask için eklenecek
                    #endregion

                    #region Toplam Satırları Uretim
                    decimal? toplamUretim = 0;
                    toplamUretim = model.procedurePoliceOfflineList.Where(w => w.BrutPrim.HasValue).Sum(s => s.BrutPrim);
                    parser.SetVariable("$ToplamUretimBrut$", toplamUretim.Value.ToString("N2"));
                    toplamUretim = model.procedurePoliceOfflineList.Where(w => w.NetPrim.HasValue).Sum(s => s.NetPrim);
                    parser.SetVariable("$ToplamUretimNet$", toplamUretim.Value.ToString("N2"));
                    toplamUretim = model.procedurePoliceOfflineList.Where(w => w.Komisyon.HasValue).Sum(s => s.Komisyon);
                    parser.SetVariable("$ToplamUretimKomisyon$", toplamUretim.Value.ToString("N2"));

                    //üretim KMS%
                    decimal? uretimKMSToplam = 0;
                    decimal? uretimNetToplam = 0;
                    var toplamUretimKomisyonYuzdeListe = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "TL").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        NetPrim = s.NetPrim
                    });
                    foreach (var item in toplamUretimKomisyonYuzdeListe)
                    {
                        if (item.NetPrim.HasValue && item.Komisyon.HasValue )
                        {
                            uretimKMSToplam += item.Komisyon.Value;
                            uretimNetToplam += item.NetPrim.Value;
                        }
                    }

                    if (uretimNetToplam != 0)
                    {
                        var kmsToplamYuzde = (uretimKMSToplam / uretimNetToplam) * 100;
                        parser.SetVariable("$ToplamUretimKMS%$", kmsToplamYuzde.Value.ToString("N2"));
                    }
                    else
                    {
                        var kmsToplamYuzde = 0;
                        parser.SetVariable("$ToplamUretimKMS%$", kmsToplamYuzde.ToString("N2"));
                    }

                    toplamUretim = model.procedurePoliceOfflineList.Where(w => w.TaliKomisyon.HasValue).Sum(s => s.TaliKomisyon);
                    parser.SetVariable("$ToplamUretimTaliKomisyon$", toplamUretim.Value.ToString("N2"));
                    toplamUretim = model.procedurePoliceOfflineList.Where(w => w.Komisyon.HasValue).Sum(s => s.Komisyon - s.TaliKomisyon);
                    parser.SetVariable("$ToplamUretimNetKomisyon$", toplamUretim.Value.ToString("N2"));
                    toplamUretim = model.procedurePoliceOfflineList.Where(w => w.BrutPrim.HasValue).Sum(s => s.BrutPrim - s.Komisyon);
                    parser.SetVariable("$ToplamUretimBorç$", toplamUretim.Value.ToString("N2"));
                    //Alliazn Dask için eklenecek
                    #endregion

                    #region Toplam Satırları USD Toplam

                    #region usd brüt
                    decimal? usdBrutPrim = 0;
                    var toplamDovizBrutListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "USD").Select(s => new
                    {
                        brutPrim = s.BrutPrim,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamDovizBrutListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.brutPrim.HasValue && item.brutPrim.Value > 0)
                        {
                            usdBrutPrim += item.brutPrim.Value / item.dovizKur.Value;
                        }
                    }
                    //Alliazn Dask için eklenecek
                    parser.SetVariable("$USDBrutToplam$", usdBrutPrim.Value.ToString("N2"));
                    #endregion

                    #region usd net
                    decimal? usdNetPrim = 0;
                    var toplamNetListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "USD").Select(s => new
                    {
                        netPrim = s.NetPrim,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamNetListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.netPrim.HasValue && item.netPrim.Value > 0)
                        {
                            usdNetPrim += item.netPrim.Value / item.dovizKur.Value;
                        }
                    }
                    //Alliazn Dask için eklenecek
                    parser.SetVariable("$USDNetToplam$", usdNetPrim.Value.ToString("N2"));
                    #endregion
                    #region usd komisyon
                    decimal? usdKomisyon = 0;
                    var toplamKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "USD").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamKomisyonListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            usdKomisyon += item.Komisyon.Value / item.dovizKur.Value;
                        }
                    }
                    //Alliazn Dask için eklenecek
                    parser.SetVariable("$USDKomisyonToplam$", usdKomisyon.Value.ToString("N2"));
                    #endregion

                    #region usd KMS%
                    decimal? usdKMSToplam = 0;
                    decimal? usdNetToplam = 0;
                    var toplamKomisyonYuzdeListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "USD").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        NetPrim = s.NetPrim
                    });
                    foreach (var item in toplamKomisyonYuzdeListesi)
                    {
                        if (item.NetPrim.HasValue && item.NetPrim.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            usdKMSToplam += item.Komisyon.Value;
                            usdNetToplam += item.NetPrim.Value;
                        }
                    }

                    if (usdNetToplam != 0)
                    {
                        var kmsToplamYuzde = (usdKMSToplam / usdNetToplam) * 100;
                        parser.SetVariable("$USDKmsToplam$", kmsToplamYuzde.Value.ToString("N2"));
                    }
                    else
                    {
                        var kmsToplamYuzde = 0;
                        parser.SetVariable("$USDKmsToplam$", kmsToplamYuzde.ToString("N2"));
                    }
                   
                    #endregion
                    #region usd talikomisyon
                    decimal? usdTaliKomisyon = 0;
                    var toplamTaliKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "USD").Select(s => new
                    {
                        taliKomisyon = s.TaliKomisyon,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamTaliKomisyonListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.taliKomisyon.HasValue && item.taliKomisyon.Value > 0)
                        {
                            usdTaliKomisyon += item.taliKomisyon.Value / item.dovizKur.Value;
                        }
                    }
                    //Alliazn Dask için eklenecek
                    parser.SetVariable("$USDTaliKomisyonToplam$", usdTaliKomisyon.Value.ToString("N2"));
                    #endregion
                    #region usd netKomisyon
                    decimal? usdKomisyonToplam = 0;
                    decimal? usdTaliKomisyonToplam = 0;

                    var toplamNetKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "USD").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        TaliKomisyon = s.TaliKomisyon,
                    });
                    foreach (var item in toplamNetKomisyonListesi)
                    {
                        if (item.Komisyon.HasValue && item.Komisyon.Value > 0 && item.TaliKomisyon.HasValue && item.TaliKomisyon.Value > 0)
                        {
                            usdKomisyonToplam += item.Komisyon.Value;
                            usdTaliKomisyonToplam += item.TaliKomisyon.Value;
                        }
                    }
                    var netKomisyon = (usdKomisyonToplam - usdTaliKomisyonToplam);
                    
                    parser.SetVariable("$USDNetKomisyonToplam$", netKomisyon.Value.ToString("N2"));
                    #endregion
                    #region usdborç
                    decimal? usdBrutToplam = 0;
                    decimal? USDKomisyonToplam = 0;

                    var toplamBorçListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "USD").Select(s => new
                    {
                        BrutPrim = s.BrutPrim,
                        Komisyon = s.Komisyon,
                    });
                    foreach (var item in toplamBorçListesi)
                    {
                        if (item.BrutPrim.HasValue && item.BrutPrim.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            usdBrutToplam += item.BrutPrim.Value;
                            USDKomisyonToplam += item.Komisyon.Value;
                        }
                    }
                    var usdBorç = (usdBrutToplam - USDKomisyonToplam);

                    parser.SetVariable("$USDBorçToplam$", usdBorç.Value.ToString("N2"));
                    #endregion
                    #endregion

                    #region Toplam Satırları EUR Toplam

                    #region eur brüt
                    decimal? eurBrutPrim = 0;
                    var toplamEURDovizBrutListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "EUR").Select(s => new
                    {
                        brutPrim = s.BrutPrim,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamEURDovizBrutListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.brutPrim.HasValue && item.brutPrim.Value > 0)
                        {
                            eurBrutPrim += item.brutPrim.Value / item.dovizKur.Value;
                        }
                    }
                    
                    parser.SetVariable("$EURBrutToplam$", eurBrutPrim.Value.ToString("N2"));
                    #endregion

                    #region eur net
                    decimal? eurNetPrim = 0;
                    var toplamEURNetListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "EUR").Select(s => new
                    {
                        netPrim = s.NetPrim,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamEURNetListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.netPrim.HasValue && item.netPrim.Value > 0)
                        {
                            eurNetPrim += item.netPrim.Value / item.dovizKur.Value;
                        }
                    }
                    
                    parser.SetVariable("$EURNetToplam$", eurNetPrim.Value.ToString("N2"));
                    #endregion
                    #region eur komisyon
                    decimal? eurKomisyon = 0;
                    var toplamEURKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "EUR").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamEURKomisyonListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            eurKomisyon += item.Komisyon.Value / item.dovizKur.Value;
                        }
                    }
                    
                    parser.SetVariable("$EURKomisyonToplam$", eurKomisyon.Value.ToString("N2"));
                    #endregion
                    #region eur KMS%
                    decimal? eurKMSToplam = 0;
                    decimal? eurNetToplam = 0;
                    var toplamEURKomisyonYuzdeListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "EUR").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        NetPrim = s.NetPrim
                    });
                    foreach (var item in toplamEURKomisyonYuzdeListesi)
                    {
                        if (item.NetPrim.HasValue && item.NetPrim.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            eurKMSToplam += item.Komisyon.Value;
                            eurNetToplam += item.NetPrim.Value;
                        }
                    }
                    if (eurNetToplam != 0)
                    {
                    var kmsEURToplamYuzde = (eurKMSToplam / eurNetToplam) * 100;

                    parser.SetVariable("$KMSEURToplam$", kmsEURToplamYuzde.Value.ToString("N2"));
                    }
                    else
                    {
                        var kmsEURToplamYuzde = 0;
                        parser.SetVariable("$KMSEURToplam$", kmsEURToplamYuzde.ToString("N2"));
                    }
                    

                    
                    #endregion
                    #region eur talikomisyon
                    decimal? eurTaliKomisyon = 0;
                    var toplamEURTaliKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "EUR").Select(s => new
                    {
                        taliKomisyon = s.TaliKomisyon,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamEURTaliKomisyonListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.taliKomisyon.HasValue && item.taliKomisyon.Value > 0)
                        {
                            eurTaliKomisyon += item.taliKomisyon.Value / item.dovizKur.Value;
                        }
                    }
                    
                    parser.SetVariable("$EURTaliKomisyonToplam$", eurTaliKomisyon.Value.ToString("N2"));
                    #endregion
                    #region eur netKomisyon
                    decimal? eurKomisyonToplam = 0;
                    decimal? eurTaliKomisyonToplam = 0;

                    var toplamEURNetKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "EUR").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        TaliKomisyon = s.TaliKomisyon,
                    });
                    foreach (var item in toplamEURNetKomisyonListesi)
                    {
                        if (item.Komisyon.HasValue && item.Komisyon.Value > 0 && item.TaliKomisyon.HasValue && item.TaliKomisyon.Value > 0)
                        {
                            eurKomisyonToplam += item.Komisyon.Value;
                            eurTaliKomisyonToplam += item.TaliKomisyon.Value;
                        }
                    }
                    var eurnetKomisyon = (eurKomisyonToplam - eurTaliKomisyonToplam);

                    parser.SetVariable("$EURNetKomisyonToplam$", eurnetKomisyon.Value.ToString("N2"));
                    #endregion
                    #region eurborç
                    decimal? eurBrutToplam = 0;
                    decimal? EURKomisyonToplam = 0;

                    var toplamEURBorçListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "EUR").Select(s => new
                    {
                        BrutPrim = s.BrutPrim,
                        Komisyon = s.Komisyon,
                    });
                    foreach (var item in toplamEURBorçListesi)
                    {
                        if (item.BrutPrim.HasValue && item.BrutPrim.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            eurBrutToplam += item.BrutPrim.Value;
                            EURKomisyonToplam += item.Komisyon.Value;
                        }
                    }
                    var EURBorç = (eurBrutToplam - EURKomisyonToplam);

                    parser.SetVariable("$EURBorçToplam$", EURBorç.Value.ToString("N2"));
                    #endregion
                    #endregion

                    #region Toplam Satırları GBP Toplam

                    #region gbp brüt
                    decimal? gbpBrutPrim = 0;
                    var toplamGBPDovizBrutListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "GBP").Select(s => new
                    {
                        brutPrim = s.BrutPrim,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamGBPDovizBrutListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.brutPrim.HasValue && item.brutPrim.Value > 0)
                        {
                            gbpBrutPrim += item.brutPrim.Value / item.dovizKur.Value;
                        }
                    }

                    parser.SetVariable("$GBPBrutToplam$", gbpBrutPrim.Value.ToString("N2"));
                    #endregion

                    #region gbp net
                    decimal? gbpNetPrim = 0;
                    var toplamGBPNetListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "GBP").Select(s => new
                    {
                        netPrim = s.NetPrim,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamGBPNetListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.netPrim.HasValue && item.netPrim.Value > 0)
                        {
                            gbpNetPrim += item.netPrim.Value / item.dovizKur.Value;
                        }
                    }

                    parser.SetVariable("$GBPNetToplam$", gbpNetPrim.Value.ToString("N2"));
                    #endregion
                    #region gbp komisyon
                    decimal? gbpKomisyon = 0;
                    var toplamGBPKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "GBP").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamGBPKomisyonListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            gbpKomisyon += item.Komisyon.Value / item.dovizKur.Value;
                        }
                    }

                    parser.SetVariable("$GBPKomisyonToplam$", gbpKomisyon.Value.ToString("N2"));
                    #endregion
                    #region gbp KMS%
                    decimal? gbpKMSToplam = 0;
                    decimal? gbpNetToplam = 0;
                    var toplamGBPKomisyonYuzdeListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "GBP").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        NetPrim = s.NetPrim
                    });
                    foreach (var item in toplamGBPKomisyonYuzdeListesi)
                    {
                        if (item.NetPrim.HasValue && item.NetPrim.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            gbpKMSToplam += item.Komisyon.Value;
                            gbpNetToplam += item.NetPrim.Value;
                        }
                    }

                    if (gbpNetToplam != 0)
                    {
                    var kmsGBPToplamYuzde = (gbpKMSToplam / gbpNetToplam) * 100;

                    parser.SetVariable("$KMSGBPToplam$", kmsGBPToplamYuzde.Value.ToString("N2"));
                    }
                    else
                    {
                    var kmsGBPToplamYuzde = 0;
                    parser.SetVariable("$KMSGBPToplam$", kmsGBPToplamYuzde.ToString("N2"));

                    }
                    #endregion
                    #region gbp talikomisyon
                    decimal? gbpTaliKomisyon = 0;
                    var toplamGBPTaliKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "GBP").Select(s => new
                    {
                        taliKomisyon = s.TaliKomisyon,
                        dovizKur = s.DovizKur
                    });
                    foreach (var item in toplamGBPTaliKomisyonListesi)
                    {
                        if (item.dovizKur.HasValue && item.dovizKur.Value > 0 && item.taliKomisyon.HasValue && item.taliKomisyon.Value > 0)
                        {
                            gbpTaliKomisyon += item.taliKomisyon.Value / item.dovizKur.Value;
                        }
                    }

                    parser.SetVariable("$GBPTaliKomisyonToplam$", gbpTaliKomisyon.Value.ToString("N2"));
                    #endregion
                    #region gbp netKomisyon
                    decimal? gbpKomisyonToplam = 0;
                    decimal? gbpTaliKomisyonToplam = 0;

                    var toplamGBPNetKomisyonListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "GBP").Select(s => new
                    {
                        Komisyon = s.Komisyon,
                        TaliKomisyon = s.TaliKomisyon,
                    });
                    foreach (var item in toplamGBPNetKomisyonListesi)
                    {
                        if (item.Komisyon.HasValue && item.Komisyon.Value > 0 && item.TaliKomisyon.HasValue && item.TaliKomisyon.Value > 0)
                        {
                            gbpKomisyonToplam += item.Komisyon.Value;
                            gbpTaliKomisyonToplam += item.TaliKomisyon.Value;
                        }
                    }
                    var gbpnetKomisyon = (gbpKomisyonToplam - gbpTaliKomisyonToplam);

                    parser.SetVariable("$GBPNetKomisyonToplam$", gbpnetKomisyon.Value.ToString("N2"));
                    #endregion
                    #region gbpborç
                    decimal? gbpBrutToplam = 0;
                    decimal? GBPKomisyonToplam = 0;

                    var toplamGBPBorçListesi = model.procedurePoliceOfflineList.Where(w => w.ParaBirimi == "GBP").Select(s => new
                    {
                        BrutPrim = s.BrutPrim,
                        Komisyon = s.Komisyon,
                    });
                    foreach (var item in toplamGBPBorçListesi)
                    {
                        if (item.BrutPrim.HasValue && item.BrutPrim.Value > 0 && item.Komisyon.HasValue && item.Komisyon.Value > 0)
                        {
                            gbpBrutToplam += item.BrutPrim.Value;
                            GBPKomisyonToplam += item.Komisyon.Value;
                        }
                    }
                    var GBPBorç = (gbpBrutToplam - GBPKomisyonToplam);

                    parser.SetVariable("$GBPBorçToplam$", GBPBorç.Value.ToString("N2"));
                #endregion
                #endregion
                parser.Parse();
                pdf.Close();

                byte[] fileData = pdf.GetFileBytes();
                    
                    IPoliceListesiPDFStorage storage = DependencyResolver.Current.GetService<IPoliceListesiPDFStorage>();
                    string fileName = String.Format(model.TVMUnvani +  " - " + tarihAraligi + "-DonemselUretimRaporu_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile(fileName, fileData);
                    Console.Write(url);
                    if (!String.IsNullOrEmpty(url))
                    {
                        return url;
                    }
                }
                return "";
            }
            catch (Exception ex)
            {

                return "";
            }
            finally
            {
                if (pdf != null)
                    pdf.Dispose();
            }
        }
    }
     
}

