using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using nsmusteri = Neosinerji.BABOnlineTP.Web.Areas.Musteri.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Content.Lang;
using System.Linq.Expressions;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using System.Net;
using System.Text;
using Neosinerji.BABOnlineTP.Business.Common.AEGON;
using Neosinerji.BABOnlineTP.Business.AEGON;
using Neosinerji.BABOnlineTP.Database.Common;
using System.Globalization;
using Neosinerji.BABOnlineTP.Business.GULF;
using Neosinerji.BABOnlineTP.Business.LilyumKoru;

using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Seyahat;

namespace Neosinerji.BABOnlineTP.Web.Areas.Teklif.Controllers
{
    [IDAuthority(Type = "Teklif")]
    [Authorization(AnaMenuKodu = 0, IslemId = "")]
    public class TeklifController : Controller
    {
        public ITVMService _TVMService;
        public ITeklifService _TeklifService;
        public IMusteriService _MusteriService;
        public ITanimService _TanimService;
        public IKullaniciService _KullaniciService;
        public IAktifKullaniciService _AktifKullaniciService;
        public IUlkeService _UlkeService;
        public ICRService _CRService;
        public IUrunService _UrunService;
        public IAracService _AracService;
        public ITUMService _TUMService;
        public ILogService _LogService;
        public IPoliceToXML _PoliceToXML;
        public ITURKNIPPONSeyahat _TurkNIPPONService;
        public TeklifGenel TUMTeklif = new TeklifGenel();

        public TeklifController(ITVMService tvmService,
                                ITeklifService teklifService,
                                IMusteriService musteriService,
                                IKullaniciService kullaniciService,
                                IAktifKullaniciService aktifKullaniciService,
                                ITanimService tanimService,
                                IUlkeService ulkeService,
                                ICRService crService,
                                IAracService aracService,
                                IUrunService urunService,
                                ITUMService tumService
/*                                ITURKNIPPONSeyahat turkNIPPONService*/)
        {
            _TVMService = tvmService;
            _TeklifService = teklifService;
            _MusteriService = musteriService;
            _TanimService = tanimService;
            _KullaniciService = kullaniciService;
            _AktifKullaniciService = aktifKullaniciService;
            _UlkeService = ulkeService;
            _CRService = crService;
            _UrunService = urunService;
            _AracService = aracService;
            _TUMService = tumService;
            //_TurkNIPPONService = turkNIPPONService;
            _LogService = DependencyResolver.Current.GetService<ILogService>();
        }

        public ActionResult TeklifEkleTest()
        {
            TrafikModel model = new TrafikModel();


            return View(model);
        }
        //Teklif Arama Ekranı..........
        public ActionResult Liste()
        {

            TeklifAramaModel model = new TeklifAramaModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;

            //if (_AktifKullaniciService.TVMKodu != NeosinerjiTVM.AegonTVMKodu)
            //{
            model.TVMKodu = _AktifKullaniciService.TVMKodu;
            model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            //}


            model.Durumlar = new SelectList(TeklifListeleri.TeklifDurumTipleri(), "Value", "Text", "1");
            model.TUMler = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani", "").ListWithOptionLabel();
            model.AktifProjeKodu = _AktifKullaniciService.ProjeKodu;
            model.Urunler = new SelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama", "").ListWithOptionLabel();
            model.Kullanicilar = new SelectList(_KullaniciService.GetListKullanicilarTeklifAra(), "KullaniciKodu", "AdiSoyadi", "").ListWithOptionLabel();

            return View(model);
        }

        public ActionResult Liste1()
        {

            TeklifAramaModel model = new TeklifAramaModel();

            model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
            model.BitisTarihi = TurkeyDateTime.Now;

            if (_AktifKullaniciService.TVMKodu != NeosinerjiTVM.AegonTVMKodu)
            {
                model.TVMKodu = _AktifKullaniciService.TVMKodu;
                model.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            }


            model.Durumlar = new SelectList(TeklifListeleri.TeklifDurumTipleri(), "Value", "Text", "1");
            model.TUMler = new SelectList(_TUMService.GetListTUMDetay(), "Kodu", "Unvani", "").ListWithOptionLabel();

            model.Urunler = new SelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama", "").ListWithOptionLabel();
            model.Kullanicilar = new SelectList(_KullaniciService.GetListKullanicilarTeklifAra(), "KullaniciKodu", "AdiSoyadi", "").ListWithOptionLabel();

            return View(model);
        }

        public ActionResult ListePager()
        {
            if (Request["sEcho"] != null)
            {
                List<Expression<Func<TeklifAramaTableModel, object>>> selectColumns = new List<Expression<Func<TeklifAramaTableModel, object>>>();

                selectColumns.Add(t => t.TeklifNo);
                if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                {
                    selectColumns.Add(t => t.TUMTeklifNo);
                }
                selectColumns.Add(t => t.TVMUnvani);
                selectColumns.Add(t => t.TanzimTarihi);
                selectColumns.Add(t => t.TVMKullaniciAdSoyad);
                selectColumns.Add(t => t.UrunAdi);
                selectColumns.Add(t => t.MusteriAdSoyad);
                selectColumns.Add(t => t.OzelAlan);
                selectColumns.Add(t => t.DetailIcon);

                TeklifListe arama = new TeklifListe(Request, selectColumns.ToArray());

                arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                arama.TUMKodu = arama.TryParseParamInt("TUMKodu");
                arama.UrunKodu = arama.TryParseParamInt("UrunKodu");
                arama.HazirlayanKodu = arama.TryParseParamInt("HazirlayanKodu");

                arama.TeklifNo = arama.TryParseParamString("TeklifNo");
                arama.BaslangisTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                arama.TeklifDurumu = arama.TryParseParamInt("TeklifDurumu");
                arama.MusteriKodu = arama.TryParseParamInt("MusteriKodu");


                arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.KayitTarihi.ToString()));
                arama.AddFormatter(f => f.MusteriAdSoyad, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.MusteriAdSoyad));
                arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a> {3}{4}", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu), f.TeklifId, f.TeklifNo,
                                                       f.PdfURL == null ? "" :
                                                       "<a href=" + f.PdfURL + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                                                       "<img src='/content/img/pdf_icon.png' /></a>",
                                                       f.Otorizasyon ? String.Format("<button class='btn pull-right otorizasyon-check' type='button' data-teklif-id='{0}' title='Otorizasyon'><i class='icon icon-warning-sign pull-right' style='color:#b94a48;'></i></button>", f.TeklifId) : ""));

                if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                {
                    arama.AddFormatter(f => f.TUMTeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a>", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu), f.TeklifId, f.TUMTeklifNo));
                }

                arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", RaporController.GetOzelAlan(s.TeklifId)));
                arama.AddFormatter(s => s.DetailIcon, s => String.Format("<a href='javascript:;' class='see-teklif-detail' teklif-id='{0}'>" +
                                                                         "<img src='/Content/img/icon-details.png'/></a>", s.TeklifId));


                int totalRowCount = 0;
                List<TeklifAramaTableModel> list = _TeklifService.PagedList(arama, out totalRowCount, true);

                Neosinerji.BABOnlineTP.Business.DataTableList result = arama.Prepare(list, totalRowCount);


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult ListePager1()
        {
            if (Request["sEcho"] != null)
            {
                List<Expression<Func<TeklifAramaTableModel1, object>>> selectColumns = new List<Expression<Func<TeklifAramaTableModel1, object>>>();

                selectColumns.Add(t => t.TeklifNo);
                if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                {
                    selectColumns.Add(t => t.TUMTeklifNo);
                }
                selectColumns.Add(t => t.TVMUnvani);
                selectColumns.Add(t => t.TanzimTarihi);
                selectColumns.Add(t => t.TVMKullaniciAdSoyad);
                selectColumns.Add(t => t.UrunAdi);
                selectColumns.Add(t => t.MusteriAdSoyad);
                selectColumns.Add(t => t.OzelAlan);
                selectColumns.Add(t => t.DetailIcon);

                TeklifListe1 arama = new TeklifListe1(Request, selectColumns.ToArray());

                arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                arama.TUMKodu = arama.TryParseParamInt("TUMKodu");
                arama.UrunKodu = arama.TryParseParamInt("UrunKodu");
                arama.HazirlayanKodu = arama.TryParseParamInt("HazirlayanKodu");

                arama.TeklifNo = arama.TryParseParamString("TeklifNo");
                arama.BaslangisTarihi = arama.TryParseParamDate("BaslangicTarihi");
                arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                arama.TeklifDurumu = arama.TryParseParamInt("TeklifDurumu");
                arama.MusteriKodu = arama.TryParseParamInt("MusteriKodu");


                arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.KayitTarihi.ToString()));
                arama.AddFormatter(f => f.MusteriAdSoyad, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.MusteriAdSoyad));
                arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a> {3}{4}", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu), f.TeklifId, f.TeklifNo,
                                                       f.PdfURL == null ? "" :
                                                       "<a href=" + f.PdfURL + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                                                       "<img src='/content/img/pdf_icon.png' /></a>",
                                                       f.Otorizasyon == 1 ? String.Format("<button class='btn pull-right otorizasyon-check' type='button' data-teklif-id='{0}' title='Otorizasyon'><i class='icon icon-warning-sign pull-right' style='color:#b94a48;'></i></button>", f.TeklifId) : ""));

                if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                {
                    arama.AddFormatter(f => f.TUMTeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a>", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu), f.TeklifId, f.TUMTeklifNo));
                }

                arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", RaporController.GetOzelAlan(s.TeklifId)));
                arama.AddFormatter(s => s.DetailIcon, s => String.Format("<a href='javascript:;' class='see-teklif-detail' teklif-id='{0}'>" +
                                                                         "<img src='/Content/img/icon-details.png'/></a>", s.TeklifId));


                int totalRowCount = 0;
                //List<TeklifAramaTableModel> list = _TeklifService.PagedList(arama, out totalRowCount, true);
                List<TeklifAramaTableModel1> list = _TeklifService.PagedList1(arama, out totalRowCount, true);

                Neosinerji.BABOnlineTP.Database.Common.DataTableList result = arama.Prepare(list, totalRowCount);


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public ActionResult TeklifAraListePager()
        {
            try
            {

                if (Request["sEcho"] != null)
                {
                    CultureInfo turkey = new CultureInfo("tr-TR");

                    List<Expression<Func<TeklifAraProcedureModel, object>>> selectColumns = new List<Expression<Func<TeklifAraProcedureModel, object>>>();

                    selectColumns.Add(t => t.TeklifNo);
                    selectColumns.Add(t => t.MusteriAdiSoyadi);
                    selectColumns.Add(t => t.UrunAdi);
                    selectColumns.Add(t => t.TanzimTarihi);
                    selectColumns.Add(t => t.OzelAlan);
                    selectColumns.Add(t => t.TVMUnvan);
                    selectColumns.Add(t => t.EkleyenKullaniciAdi);
                    selectColumns.Add(t => t.DetailIcon);

                    TeklifAraListe arama = new TeklifAraListe(Request, selectColumns.ToArray());

                    //SOL
                    arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                    arama.MusteriKodu = arama.TryParseParamInt("MusteriKodu") ?? 0;
                    arama.UrunKodu = arama.TryParseParamInt("UrunKodu") ?? 0;
                    arama.HazirlayanKodu = arama.TryParseParamInt("HazirlayanKodu") ?? 0;
                    arama.TUMKodu = arama.TryParseParamInt("TUMKodu") ?? 0;

                    //SAĞ                   
                    arama.TeklifNo = arama.TryParseParamInt("TeklifNo");
                    arama.BaslangisTarihi = arama.TryParseParamDate("BaslangicTarihi");
                    arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                    arama.TeklifDurumu = arama.TryParseParamInt("TeklifDurumu") ?? 0;

                    int totalRowCount = 0;
                    List<TeklifAraProcedureModel> list = _TeklifService.TeklifAraPageList(arama, out totalRowCount);

                    arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a> {3}", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu),
                                                            f.TeklifId, f.TeklifNo, f.PDFDosyasi == null ? "" :
                                                            "<a href=" + f.PDFDosyasi + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                                                            "<img src='/content/img/pdf_icon.png' /></a>"));


                    arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.HasValue ? f.TanzimTarihi.Value.ToString("dd.MM.yyyy") : ""));
                    arama.AddFormatter(f => f.MusteriAdiSoyadi, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.MusteriAdiSoyadi));

                    if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                    {
                        arama.AddFormatter(f => f.TUMTeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a>", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu), f.TeklifId, f.TUMTeklifNo));
                    }

                    arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", RaporController.GetOzelAlan(s.TeklifId)));
                    arama.AddFormatter(s => s.DetailIcon, s => String.Format("<a href='javascript:;' class='see-teklif-detail' teklif-id='{0}'>" +
                                                                             "<img src='/Content/img/icon-details.png'/></a>", s.TeklifId));

                    Neosinerji.BABOnlineTP.Business.DataTableList result = arama.Prepare(list, totalRowCount);


                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return View();
        }

        public ActionResult Aegon_TeklifAraListePager()
        {
            try
            {

                if (Request["sEcho"] != null)
                {
                    CultureInfo turkey = new CultureInfo("tr-TR");

                    List<Expression<Func<AegonTeklifAraProcedureModel, object>>> selectColumns = new List<Expression<Func<AegonTeklifAraProcedureModel, object>>>();

                    selectColumns.Add(t => t.TeklifNo);
                    selectColumns.Add(t => t.MusteriAdiSoyadi);
                    selectColumns.Add(t => t.UrunAdi);
                    selectColumns.Add(t => t.TanzimTarihi);
                    selectColumns.Add(t => t.TVMUnvan);
                    selectColumns.Add(t => t.EkleyenKullaniciAdi);
                    selectColumns.Add(t => t.DetailIcon);

                    AegonTeklifAraListe arama = new AegonTeklifAraListe(Request, selectColumns.ToArray());

                    //SOL
                    arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                    arama.MusteriKodu = arama.TryParseParamInt("MusteriKodu") ?? 0;
                    arama.UrunKodu = arama.TryParseParamInt("UrunKodu") ?? 0;
                    arama.HazirlayanKodu = arama.TryParseParamInt("HazirlayanKodu") ?? 0;

                    //SAĞ
                    arama.TeklifNo = arama.TryParseParamInt("TeklifNo");
                    arama.BaslangisTarihi = arama.TryParseParamDate("BaslangicTarihi");
                    arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");

                    int totalRowCount = 0;
                    List<AegonTeklifAraProcedureModel> list = _TeklifService.Aegon_TeklifAraPageList(arama, out totalRowCount);

                    arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a> {3}", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu),
                                                            f.TeklifId, f.TeklifNo, f.PDFDosyasi == null ? "" :
                                                            "<a href=" + f.PDFDosyasi + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                                                            "<img src='/content/img/pdf_icon.png' /></a>"));


                    arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.HasValue ? f.TanzimTarihi.Value.ToString("dd.MM.yyyy") : ""));
                    arama.AddFormatter(f => f.MusteriAdiSoyadi, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.MusteriAdiSoyadi));
                    arama.AddFormatter(s => s.DetailIcon, s => String.Format("<a href='javascript:;' class='see-teklif-detail' teklif-id='{0}'>" +
                                                                                                 "<img src='/Content/img/icon-details.png'/></a>", s.TeklifId));

                    Neosinerji.BABOnlineTP.Business.DataTableList result = arama.Prepare(list, totalRowCount);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return View();
        }

        public ActionResult Mapfre_TeklifAraListePager()
        {
            try
            {

                if (Request["sEcho"] != null)
                {
                    CultureInfo turkey = new CultureInfo("tr-TR");

                    List<Expression<Func<MapfreTeklifAraProcedureModel, object>>> selectColumns = new List<Expression<Func<MapfreTeklifAraProcedureModel, object>>>();

                    selectColumns.Add(t => t.TeklifNo);
                    selectColumns.Add(t => t.TUMTeklifNo);
                    selectColumns.Add(t => t.MusteriAdiSoyadi);
                    selectColumns.Add(t => t.UrunAdi);
                    selectColumns.Add(t => t.TanzimTarihi);
                    selectColumns.Add(t => t.OzelAlan);
                    selectColumns.Add(t => t.TVMUnvan);
                    selectColumns.Add(t => t.EkleyenKullaniciAdi);
                    selectColumns.Add(t => t.DetailIcon);

                    MapfreTeklifAraListe arama = new MapfreTeklifAraListe(Request, selectColumns.ToArray());


                    //SOL
                    arama.TVMKodu = arama.TryParseParamInt("TVMKodu");
                    arama.MusteriKodu = arama.TryParseParamInt("MusteriKodu") ?? 0;
                    arama.UrunKodu = arama.TryParseParamInt("UrunKodu") ?? 0;
                    arama.HazirlayanKodu = arama.TryParseParamInt("HazirlayanKodu") ?? 0;

                    //SAĞ
                    string TeklifNo = arama.TryParseParamString("TeklifNo");

                    if (!String.IsNullOrEmpty(TeklifNo) && TeklifNo.Substring(0, 1) == "T")
                    {
                        arama.TeklifNo = 0;
                        arama.TUMTeklifNo = arama.TryParseParamString("TeklifNo");
                    }
                    else
                    {
                        arama.TeklifNo = arama.TryParseParamInt("TeklifNo");
                        arama.TUMTeklifNo = "0";
                    }

                    arama.BaslangisTarihi = arama.TryParseParamDate("BaslangicTarihi");
                    arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                    arama.TeklifDurumu = arama.TryParseParamInt("TeklifDurumu") ?? 0;

                    int totalRowCount = 0;
                    List<MapfreTeklifAraProcedureModel> list = _TeklifService.Mapfre_TeklifAraPageList(arama, out totalRowCount);

                    arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a> {3}", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu),
                                                            f.TeklifId, f.TeklifNo, f.PDFDosyasi == null ? "" :
                                                            "<a href=" + f.PDFDosyasi + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                                                            "<img src='/content/img/pdf_icon.png' /></a>"));

                    arama.AddFormatter(f => f.TUMTeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a>", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu), f.TeklifId, f.TUMTeklifNo));
                    arama.AddFormatter(f => f.TanzimTarihi, f => String.Format("{0}", f.TanzimTarihi.HasValue ? f.TanzimTarihi.Value.ToString("dd.MM.yyyy") : ""));
                    arama.AddFormatter(f => f.MusteriAdiSoyadi, f => String.Format("<a href='/Musteri/Musteri/Detay/{0}'>{1}</a>", f.MusteriKodu, f.MusteriAdiSoyadi));

                    arama.AddFormatter(s => s.OzelAlan, s => String.Format("{0}", RaporController.GetOzelAlan(s.TeklifId)));
                    arama.AddFormatter(s => s.DetailIcon, s => String.Format("<a href='javascript:;' class='see-teklif-detail' teklif-id='{0}'>" +
                                                                             "<img src='/Content/img/icon-details.png'/></a>", s.TeklifId));

                    Neosinerji.BABOnlineTP.Business.DataTableList result = arama.Prepare(list, totalRowCount);


                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetAllTeklifDetail(int? teklifId)
        {
            if (teklifId.HasValue && teklifId > 0)
            {
                TeklifGenel teklif = _TeklifService.GetTeklifGenel(teklifId.Value);
                if (teklif == null)
                    return null;

                List<TeklifTUMDetayPartialModel> teklifs = _TeklifService.GetAllListTeklif(teklifId.Value);

                return PartialView("TeklifTUMDetayPartial", teklifs);
            }
            return null;
        }

        [HttpPost]
        public ActionResult TeklifDurumu(int isId, string guid, int[] gosterilenler)
        {
            ILogService _log = DependencyResolver.Current.GetService<ILogService>();
            TeklifDurumModel model = new TeklifDurumModel();
            try
            {
                IsDurum durum = _TeklifService.GetIsDurumu(isId);
                if (guid != durum.Guid)
                {
                    model.mesaj = "Geçersiz anahtar.";
                    return Json(new { model = model });
                }
                TimeSpan ts = TurkeyDateTime.Now.Subtract(durum.Baslangic.Value);
                if (ts.TotalMinutes > 20)
                {
                    model.mesaj = "Geçersiz istek.";
                    return Json(new { model = model });
                }

                model.id = isId;
                model.tamamlandi = durum.Durumu == IsDurumTipleri.Tamamlandi;
                model.teklifId = durum.ReferansId;

                var detaylar = durum.IsDurumDetays.ToList<IsDurumDetay>();
                var tamamlananlar = detaylar.Where(w => w.Durumu == IsDurumTipleri.Tamamlandi);
                var baslayanlar = detaylar.Where(w => w.Durumu == IsDurumTipleri.Basladi);

                List<IsDurumDetay> tamamlananTeklifler = new List<IsDurumDetay>();

                foreach (var item in tamamlananlar)
                {
                    if (baslayanlar.Count(c => c.TUMKodu == item.TUMKodu) > 0)
                        continue;

                    tamamlananTeklifler.Add(item);
                }

                var tumListe = tamamlananTeklifler.GroupBy(g => g.TUMKodu).Select(s => new { TUMKodu = s.Key });

                ITUMService tumService = DependencyResolver.Current.GetService<ITUMService>();

                model.teklifler = new List<TeklifFiyatDetayModel>();
                foreach (var item in tumListe)
                {
                    if (gosterilenler != null && gosterilenler.Contains(item.TUMKodu))
                        continue;

                    TeklifFiyatDetayModel fiyatModel = this.TeklifFiyat(item.TUMKodu, tamamlananTeklifler);

                    model.teklifler.Add(fiyatModel);
                }

                if (model.tamamlandi)
                {
                    ITeklif teklif = _TeklifService.GetTeklif(durum.ReferansId);

                    if (teklif != null)
                    {
                        model.pdf = teklif.GenelBilgiler.PDFDosyasi;
                        model.teklifNo = teklif.GenelBilgiler.TeklifNo.ToString();
                    }
                }


            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return Json(new { model = model });
        }

        [HttpPost]
        [AjaxException]
        public ActionResult TeklifPDF(int id)
        {
            bool success = true;
            string url = String.Empty;
            ITeklif teklif;
            try
            {
                teklif = _TeklifService.GetTeklif(id);
                //if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFDosyasi))
                //{
                if (teklif.UrunKodu == UrunKodlari.TrafikSigortasi)
                {
                    TrafikTeklif trafik = new TrafikTeklif(id);
                    trafik.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.KaskoSigortasi)
                {
                    KaskoTeklif kasko = new KaskoTeklif(id);
                    kasko.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.MapfreKasko)
                {
                    MapfreKaskoTeklif kasko = new MapfreKaskoTeklif(id);
                    kasko.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.MapfreTrafik)
                {
                    MapfreTrafikTeklif trafik = new MapfreTrafikTeklif(id);
                    trafik.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.DogalAfetSigortasi_Deprem)
                {
                    DaskTeklif dask = new DaskTeklif(id);
                    dask.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.YurtDisiSeyehatSaglik)
                {
                    SeyahatSaglikTeklif seyehat = new SeyahatSaglikTeklif(id);
                    seyehat.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.TamamlayiciSaglik)
                {
                    TSSTeklif tss = new TSSTeklif(id);
                    tss.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.IkinciElGaranti)
                {
                    IkinciElGarantiTeklif ikinciElGaranti = new IkinciElGarantiTeklif(id);
                    ikinciElGaranti.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.KonutSigortasi_Paket)
                {
                    KonutTeklif Konut = new KonutTeklif(id);
                    Konut.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.IsYeri)
                {
                    IsYeriTeklif isyeri = new IsYeriTeklif(id);
                    isyeri.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.TESabitPrimli)
                {
                    TESabitPrimliTeklif teSabitPrimli = new TESabitPrimliTeklif(id);
                    teSabitPrimli.CreatePDF();
                }
                else if (teklif.UrunKodu == UrunKodlari.Lilyum)
                {
                    LilyumKoruTeklif lilyum = new LilyumKoruTeklif(id);
                    lilyum.CreatePDF();
                }

                //  }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFDosyasi;
            if (String.IsNullOrEmpty(url))
            {
                throw new Exception("PDF dosyası oluşturulamadı.");
            }

            return Json(new { Success = success, PDFUrl = url });
        }

        [HttpGet]
        [AjaxException]
        public ActionResult TeklifEPosta(int id)
        {
            ITeklif teklif = _TeklifService.GetTeklif(id);
            string pdfDosyasi = "";
            if (teklif.GenelBilgiler.UrunKodu == UrunKodlari.TamamlayiciSaglik)
            {
                var sirketTeklifi = _TeklifService.GetTeklifGenel(teklif.GenelBilgiler.TeklifNo, teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.TURKNIPPON);
                if (sirketTeklifi != null)
                {
                    pdfDosyasi = sirketTeklifi.PDFDosyasi;
                }
            }
            else
            {
                pdfDosyasi = teklif.GenelBilgiler.PDFDosyasi;
            }
            // var TUMTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);
            TeklifMailGonderModel model = new TeklifMailGonderModel();
            model.TeklifId = id;
            model.SigortaEttirenMail = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;
            model.SigortaEttirenAdSoyad = teklif.SigortaEttiren.MusteriGenelBilgiler.AdiUnvan + " " + teklif.SigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan;
            model.SigortaEttirenMailGonder = true;
            model.DigerMailGonder = false;
            model.TeklifPDF = pdfDosyasi;
            return PartialView("_MailGonderPartial", model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult TeklifEPosta(TeklifMailGonderModel model)
        {
            ITeklif teklif = _TeklifService.GetTeklif(model.TeklifId);

            string epostaAdresi = String.Empty;
            string adSoyad = String.Empty;
            bool success = true;
            string message = String.Empty;
            TryValidateModel(model);

            if (!model.DigerMailGonder)
            {
                if (ModelState["DigerAdSoyad"] != null)
                    ModelState["DigerAdSoyad"].Errors.Clear();

                if (ModelState["DigerEmail"] != null)
                    ModelState["DigerEmail"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                if (model.SigortaEttirenMailGonder || model.DigerMailGonder)
                {
                    try
                    {
                        if (model.SigortaEttirenMailGonder)
                        {
                            EmailHelper(teklif, model.TeklifPDF, String.Empty, String.Empty);
                        }

                        if (model.DigerMailGonder)
                        {
                            epostaAdresi = model.DigerEmail;
                            adSoyad = model.DigerAdSoyad;

                            EmailHelper(teklif, model.TeklifPDF, adSoyad, epostaAdresi);
                        }

                        bool acenteMailSendMi = false;

                        //AEGON ACENTE MAIL GONDER
                        if (teklif.UrunKodu == UrunKodlari.Egitim || teklif.UrunKodu == UrunKodlari.TESabitPrimli ||
                            teklif.UrunKodu == UrunKodlari.PrimIadeli || teklif.UrunKodu == UrunKodlari.OdulluBirikim ||
                            teklif.UrunKodu == UrunKodlari.OdemeGuvence || teklif.UrunKodu == UrunKodlari.KorunanGelecek ||
                            teklif.UrunKodu == UrunKodlari.PrimIadeli2
                            )
                        {
                            IEMailService _email = DependencyResolver.Current.GetService<IEMailService>();
                            acenteMailSendMi = _email.SendAegonEMailTeklif(teklif, null, null, true);
                        }


                        string yetkiliAdSoyad = "";
                        TVMKullanicilar yetkili = teklif.GenelBilgiler.TVMKullanicilar;
                        if (yetkili != null)
                            yetkiliAdSoyad = yetkili.Adi + " " + yetkili.Soyadi;

                        string email = teklif.SigortaEttiren.MusteriGenelBilgiler.EMail;

                        // ==== Bilgi mesajı donduruluyor ==== //
                        if (model.SigortaEttirenMailGonder && model.DigerMailGonder)
                        {
                            if (!String.IsNullOrEmpty(email))
                                message = String.Format("{0} ve {1} adreslerine e-posta başarıyla gönderildi.", email, model.DigerEmail);
                            else
                                message = String.Format("{0} adreslerine e-posta başarıyla gönderildi.", model.DigerEmail);

                            if (acenteMailSendMi)
                                message += " Acente yetkilisi " + yetkiliAdSoyad + "'e bilgi mail'i gönderildi";

                            return Json(new { Success = success, Message = message });
                        }
                        else if (model.SigortaEttirenMailGonder && !model.DigerMailGonder)
                        {
                            if (!String.IsNullOrEmpty(email))
                                message = String.Format("{0} adresine e-posta başarıyla gönderildi.", email);
                            else
                                message = String.Format("Mail adresi bulunamadı.");

                            if (acenteMailSendMi)
                                message += " Acente yetkilisi " + yetkiliAdSoyad + "'e bilgi mail'i gönderildi";

                            return Json(new { Success = success, Message = message });
                        }
                        else if (!model.SigortaEttirenMailGonder && model.DigerMailGonder)
                        {
                            message = String.Format("{0} adresine e-posta başarıyla gönderildi.", model.DigerEmail);

                            if (acenteMailSendMi)
                                message += " Acente yetkilisi " + yetkiliAdSoyad + "'e bilgi mail'i gönderildi";

                            return Json(new { Success = success, Message = message });
                        }
                    }
                    catch (Exception ex)
                    {
                        _LogService.Error(ex);
                        throw new Exception(String.Format("{0} adresine e-posta gönderilemedi : {1}", epostaAdresi, ex.Message));
                    }
                }
                else
                {
                    message = "Lütfen en az 1 adres seciniz";
                    return Json(new { Success = success, Message = message });
                }
            }
            message = "Eksik bilgi girdiniz. Lütfen zorunlu alanları doldurunuz.";
            return Json(new { Success = success, Message = message });
        }

        public void EmailHelper(ITeklif teklif, string pdfUrl, string adSoyad, string email)
        {
            TeklifBase teklifBase;

            switch (teklif.UrunKodu)
            {
                case UrunKodlari.TrafikSigortasi: teklifBase = new TrafikTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.KaskoSigortasi: teklifBase = new KaskoTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.MapfreKasko: teklifBase = new MapfreKaskoTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.MapfreTrafik: teklifBase = new MapfreTrafikTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.DogalAfetSigortasi_Deprem: teklifBase = new DaskTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.KrediHayat: teklifBase = new KrediliHayatTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.YurtDisiSeyehatSaglik: teklifBase = new SeyahatSaglikTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.TamamlayiciSaglik: teklifBase = new TSSTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.TSSEPostaGonder(pdfUrl, adSoyad, email); break;
                case UrunKodlari.KonutSigortasi_Paket: teklifBase = new KonutTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.IsYeri: teklifBase = new IsYeriTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.IkinciElGaranti: teklifBase = new IkinciElGarantiTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.TESabitPrimli: teklifBase = new TESabitPrimliTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.OdulluBirikim: teklifBase = new OdulluBirikimTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.PrimIadeli: teklifBase = new PrimIadeliTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.Egitim: teklifBase = new EgitimTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.OdemeGuvence: teklifBase = new OdemeGuvenceTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.KorunanGelecek: teklifBase = new KorunanGelecekTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.PrimIadeli2: teklifBase = new PrimIadeli2Teklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;
                case UrunKodlari.Lilyum: teklifBase = new LilyumKoruTeklif(teklif.GenelBilgiler.TeklifId); teklifBase.EPostaGonder(adSoyad, email); break;

            }
        }

        [HttpPost]
        [AjaxException]
        public ActionResult PolicePDF(int id)
        {
            bool success = true;
            string url = String.Empty;
            ITeklif teklif = _TeklifService.GetTeklif(id);

            try
            {
                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFPolice))
                {
                    ITeklif urun = TeklifUrunFactory.AsUrunClass(teklif);
                    urun.PolicePDF();

                    teklif = _TeklifService.GetTeklif(id);
                }

                if (String.IsNullOrEmpty(teklif.GenelBilgiler.PDFPolice))
                {
                    throw new Exception("Poliçe pdf'i oluşturulamadı.");
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                throw;
            }

            url = teklif.GenelBilgiler.PDFPolice;
            return Json(new { Success = success, PDFUrl = url });
        }

        [HttpPost]
        [AjaxException]
        public ActionResult KimlikNoSorgula(string kimlikNo, int? TVMKodu)
        {
            MusteriModel model = new MusteriModel();

            //Mapfre sigortada sorgulamada hata çıkarsa elle giriş yapılamayacak.
            if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
            {
                model.DisableManualGiris = true;
                model.DisableControls = true;
            }

            if (String.IsNullOrEmpty(kimlikNo))
            {
                model.SorgulamaHata("Kimlik numarası boş bırakılamaz");
            }
            else if (kimlikNo.Length == 11 || kimlikNo.Length == 10)
            {
                int tvm = _AktifKullaniciService.TVMKodu;
                if (TVMKodu.HasValue && TVMKodu.Value > 0)
                    tvm = TVMKodu.Value;

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, tvm);

                if (musteri != null)
                {
                    model.TVMKodu = musteri.TVMKodu;
                    model.MusteriKodu = musteri.MusteriKodu;
                    model.MusteriTipKodu = musteri.MusteriTipKodu;
                    model.KimlikNo = musteri.KimlikNo;
                    model.PasaportNo = musteri.PasaportNo;
                    model.VergiDairesi = musteri.VergiDairesi;
                    model.AdiUnvan = musteri.AdiUnvan;
                    model.SoyadiUnvan = musteri.SoyadiUnvan;
                    if (MusteriTipleri.Ozel(musteri.MusteriTipKodu) && musteri.DogumTarihi.HasValue)
                    {
                        model.DogumTarihi = musteri.DogumTarihi.Value;
                        model.DogumTarihiText = musteri.DogumTarihi.Value.ToString("dd.MM.yyyy");
                        model.Cinsiyet = musteri.Cinsiyet;
                    }
                    model.Email = musteri.EMail;

                    //AEGON sigorta müşterisinn gelir vergisi oranı getiriliyor.
                    TVMDetay tvmdetay = _TVMService.GetDetay(musteri.TVMKodu);
                    if (tvmdetay != null)
                    {
                        if (!String.IsNullOrEmpty(musteri.CiroBilgisi) && tvmdetay.ProjeKodu == TVMProjeKodlari.Aegon)
                        {
                            model.GelirVergisiOrani = Convert.ToByte(musteri.CiroBilgisi);
                        }
                    }

                    MusteriAdre adres = musteri.MusteriAdres.FirstOrDefault(m => m.Varsayilan == true);

                    if (adres != null)
                    {
                        model.IlKodu = adres.IlKodu;
                        model.IlceKodu = adres.IlceKodu;

                        model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                        model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.IlKodu), "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();

                        model.AdresTipi = adres.AdresTipi.HasValue ? adres.AdresTipi.Value : 0;
                        model.Semt = adres.Semt;
                        model.Mahalle = adres.Mahalle;
                        model.Cadde = adres.Cadde;
                        model.Sokak = adres.Sokak;
                        model.Apartman = adres.Apartman;
                        model.BinaNo = adres.BinaNo;
                        model.DaireNo = adres.DaireNo;
                        model.PostaKodu = adres.PostaKodu;
                        model.AcikAdres = adres.Adres;
                    }

                    // MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                    MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault();
                    if (telefon != null)
                    {
                        List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                        model.CepTelefonu = telefon.Numara;
                        model.MusteriTelTipKodu = telefon.IletisimNumaraTipi;
                        model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text", model.MusteriTelTipKodu).ListWithOptionLabel();
                    }
                    else
                    {
                        List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                        model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text").ListWithOptionLabel();

                    }

                    //Mapfre sigortada her seferinde adres sorgulanacak.
                    if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre.ToString())
                    {
                        IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                        KimliktenAdresSorguResponse mapadr = mapfreSorgu.AdresSorgu(_AktifKullaniciService.TVMKodu, kimlikNo);
                        if (mapadr != null && String.IsNullOrEmpty(mapadr.hata) && mapadr.KisiAcikAdresBilgisiType != null)
                        {
                            string mapfreIlceKodu = mapadr.KisiAcikAdresBilgisiType.ilceKodu;
                            if (mapfreIlceKodu.Length == 6)
                            {
                                mapfreIlceKodu = mapfreIlceKodu.Substring(3, 3);
                            }
                            CR_IlIlce ilIlce = _CRService.GetIlIlceByCr(TeklifUretimMerkezleri.MAPFRE, mapadr.KisiAcikAdresBilgisiType.ilKodu, mapfreIlceKodu);

                            if (ilIlce != null)
                            {
                                model.IlKodu = ilIlce.IlKodu;
                                model.IlceKodu = ilIlce.IlceKodu;
                            }

                            model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                            model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.IlKodu), "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();

                            model.AdresTipi = MusteriTipleri.Ozel(model.MusteriTipKodu.Value) ? AdresTipleri.Ev : AdresTipleri.Is;
                            model.Semt = "";
                            model.Mahalle = mapadr.KisiAcikAdresBilgisiType.mahalle;
                            model.Cadde = mapadr.KisiAcikAdresBilgisiType.csbm;
                            model.Sokak = "";
                            model.Apartman = "";
                            model.BinaNo = mapadr.KisiAcikAdresBilgisiType.disKapiNo;
                            model.DaireNo = mapadr.KisiAcikAdresBilgisiType.icKapiNo;
                            model.PostaKodu = 0;
                            model.AcikAdres = mapadr.KisiAcikAdresBilgisiType.acikAdres;

                            model.DisableControls = true;
                        }
                    }


                    return Json(model);
                }


                //if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre.ToString() || _AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre_DisAcente)
                //{
                //    IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                //    KimlikSorguResponse kimlik = mapfreSorgu.KimlikSorgu(_AktifKullaniciService.TVMKodu, kimlikNo);

                //    if (kimlik == null)
                //    {
                //        model.SorgulamaHata("Kimlik bilgileri alınamadı.");
                //        return Json(model);
                //    }
                //    else if (kimlik != null && !String.IsNullOrEmpty(kimlik.hata))
                //    {
                //        model.SorgulamaHata(kimlik.hata);
                //        return Json(model);
                //    }
                //    else if (kimlik != null)
                //    {
                //        model.MusteriKodu = 0;
                //        model.Email = String.Empty;

                //        if (!String.IsNullOrEmpty(kimlik.vrgTCKimlikNo) && kimlik.vrgTCKimlikNo.Length == 11)
                //        {
                //            model.MusteriTipKodu = MusteriTipleri.TCMusteri;
                //            model.Uyruk = UyrukTipleri.TC;
                //            if (kimlik.vrgTCKimlikNo[0] == '9')
                //            {
                //                model.MusteriTipKodu = MusteriTipleri.YabanciMusteri;
                //                model.Uyruk = UyrukTipleri.Yabanci;
                //            }

                //            model.KimlikNo = kimlik.vrgTCKimlikNo;
                //            model.PasaportNo = String.Empty;
                //            model.VergiDairesi = String.Empty;
                //            if (!String.IsNullOrEmpty(kimlik.vrgAd) && kimlik.vrgAd.Length > 150)
                //            {
                //                model.AdiUnvan = kimlik.vrgAd.Substring(0, 150);
                //            }
                //            else
                //            {
                //                model.AdiUnvan = kimlik.vrgAd;
                //            }
                //            if (!String.IsNullOrEmpty(kimlik.vrgSoyAd) && kimlik.vrgSoyAd.Length > 50)
                //            {
                //                model.SoyadiUnvan = kimlik.vrgSoyAd.Substring(0, 50);
                //            }
                //            else
                //            {
                //                model.SoyadiUnvan = kimlik.vrgSoyAd;
                //            }
                //            if (kimlik.DogumTarihiAsDate() > DateTime.MinValue)
                //            {
                //                model.DogumTarihi = kimlik.DogumTarihiAsDate();
                //                model.DogumTarihiText = model.DogumTarihi.Value.ToString("dd.MM.yyyy");
                //            }
                //            if (!String.IsNullOrEmpty(kimlik.vrgCinsiyet))
                //            {
                //                model.Cinsiyet = kimlik.vrgCinsiyet;
                //            }
                //        }
                //        else if (!String.IsNullOrEmpty(kimlik.vrgVergiNo) && kimlikNo.Length == 10)
                //        {
                //            model.MusteriTipKodu = MusteriTipleri.TuzelMusteri;
                //            model.KimlikNo = kimlik.vrgVergiNo;
                //            model.PasaportNo = String.Empty;
                //            model.VergiDairesi = String.Empty;
                //            if (!String.IsNullOrEmpty(kimlik.vrgnvan) && kimlik.vrgnvan.Length > 150)
                //            {
                //                model.AdiUnvan = kimlik.vrgnvan.Substring(0, 150);
                //            }
                //            else
                //            {
                //                model.AdiUnvan = kimlik.vrgnvan;
                //            }
                //            model.SoyadiUnvan = ".";
                //        }

                //        KimliktenAdresSorguResponse adres = mapfreSorgu.AdresSorgu(_AktifKullaniciService.TVMKodu, kimlikNo);
                //        if (adres != null && String.IsNullOrEmpty(adres.hata) && adres.KisiAcikAdresBilgisiType != null)
                //        {
                //            string mapfreIlceKodu = adres.KisiAcikAdresBilgisiType.ilceKodu;
                //            if (mapfreIlceKodu.Length == 6)
                //            {
                //                mapfreIlceKodu = mapfreIlceKodu.Substring(3, 3);
                //            }
                //            CR_IlIlce ilIlce = _CRService.GetIlIlceByCr(TeklifUretimMerkezleri.MAPFRE, adres.KisiAcikAdresBilgisiType.ilKodu, mapfreIlceKodu);

                //            if (ilIlce != null)
                //            {
                //                model.IlKodu = ilIlce.IlKodu.Length == 1 ? ilIlce.IlKodu.PadLeft(2, '0') : ilIlce.IlKodu;
                //                model.IlceKodu = ilIlce.IlceKodu;
                //            }

                //            model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                //            model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.IlKodu), "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();

                //            model.AdresTipi = MusteriTipleri.Ozel(model.MusteriTipKodu.Value) ? AdresTipleri.Ev : AdresTipleri.Is;
                //            model.Semt = "";
                //            model.Mahalle = adres.KisiAcikAdresBilgisiType.mahalle;
                //            model.Cadde = adres.KisiAcikAdresBilgisiType.csbm;
                //            model.Sokak = "";
                //            model.Apartman = "";
                //            model.BinaNo = adres.KisiAcikAdresBilgisiType.disKapiNo;
                //            model.DaireNo = adres.KisiAcikAdresBilgisiType.icKapiNo;
                //            model.PostaKodu = 0;
                //            model.AcikAdres = adres.KisiAcikAdresBilgisiType.acikAdres;
                //        }
                //        else
                //        {
                //            model.IlKodu = "";
                //            model.IlceKodu = 0;
                //            model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", "34").ListWithOptionLabelIller();
                //            model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.IlKodu), "IlceKodu", "IlceAdi").ListWithOptionLabel();
                //        }
                //        List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                //        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                //        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                //        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                //        model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text").ListWithOptionLabel();

                //        model.DisableControls = true;
                //        model.CepTelefonu = "90-";
                //        model.TVMKodu = TVMKodu;
                //        return Json(model);
                //    }
                //}
                model.KimlikNo = kimlikNo;
                model.SorgulamaHata("Girilen kimlik numarasına ait bilgi bulunamadı.");
                return Json(model);
            }

            model.SorgulamaHata("Kimlik numarası tüzel müşteriler için 10, şahıslar için 11 rakamdan oluşmalıdır");

            return Json(model);
        }

        public ActionResult TSSNipponKimlikSorgula(string kimlikNo, int? TVMKodu)
        {
            MusteriModel model = new MusteriModel();
            if (String.IsNullOrEmpty(kimlikNo))
            {
                model.SorgulamaHata("Kimlik numarası boş bırakılamaz");
            }
            else if (kimlikNo.Length == 11 || kimlikNo.Length == 10)
            {
                int tvm = _AktifKullaniciService.TVMKodu;
                if (TVMKodu.HasValue && TVMKodu.Value > 0)
                    tvm = TVMKodu.Value;

                MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, tvm);

                if (musteri != null)
                {
                    model.TVMKodu = musteri.TVMKodu;
                    model.MusteriKodu = musteri.MusteriKodu;
                    model.MusteriTipKodu = musteri.MusteriTipKodu;
                    model.KimlikNo = musteri.KimlikNo;
                    model.PasaportNo = musteri.PasaportNo;
                    model.VergiDairesi = musteri.VergiDairesi;
                    model.AdiUnvan = musteri.AdiUnvan;
                    model.SoyadiUnvan = musteri.SoyadiUnvan;
                    if (MusteriTipleri.Ozel(musteri.MusteriTipKodu) && musteri.DogumTarihi.HasValue)
                    {
                        model.DogumTarihi = musteri.DogumTarihi.Value;
                        model.DogumTarihiText = musteri.DogumTarihi.Value.ToString("dd.MM.yyyy");
                        model.Cinsiyet = musteri.Cinsiyet;
                    }
                    model.Email = musteri.EMail;
                    MusteriAdre adres = musteri.MusteriAdres.FirstOrDefault(m => m.Varsayilan == true);

                    if (adres != null)
                    {
                        model.IlKodu = adres.IlKodu;
                        model.IlceKodu = adres.IlceKodu;

                        model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                        model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.IlKodu), "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();

                        model.AdresTipi = adres.AdresTipi.HasValue ? adres.AdresTipi.Value : 0;
                        model.Semt = adres.Semt;
                        model.Mahalle = adres.Mahalle;
                        model.Cadde = adres.Cadde;
                        model.Sokak = adres.Sokak;
                        model.Apartman = adres.Apartman;
                        model.BinaNo = adres.BinaNo;
                        model.DaireNo = adres.DaireNo;
                        model.PostaKodu = adres.PostaKodu;
                        model.AcikAdres = adres.Adres;
                    }

                    // MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                    MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault();
                    if (telefon != null)
                    {
                        List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                        model.CepTelefonu = telefon.Numara;
                        model.MusteriTelTipKodu = telefon.IletisimNumaraTipi;
                        model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text", model.MusteriTelTipKodu).ListWithOptionLabel();
                    }
                    else
                    {
                        List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                        numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                        model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text").ListWithOptionLabel();

                    }
                    //Mapfre sigortada her seferinde adres sorgulanacak.
                    if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre.ToString())
                    {
                        IMAPFRESorguService mapfreSorgu = DependencyResolver.Current.GetService<IMAPFRESorguService>();
                        KimliktenAdresSorguResponse mapadr = mapfreSorgu.AdresSorgu(_AktifKullaniciService.TVMKodu, kimlikNo);
                        if (mapadr != null && String.IsNullOrEmpty(mapadr.hata) && mapadr.KisiAcikAdresBilgisiType != null)
                        {
                            string mapfreIlceKodu = mapadr.KisiAcikAdresBilgisiType.ilceKodu;
                            if (mapfreIlceKodu.Length == 6)
                            {
                                mapfreIlceKodu = mapfreIlceKodu.Substring(3, 3);
                            }
                            CR_IlIlce ilIlce = _CRService.GetIlIlceByCr(TeklifUretimMerkezleri.MAPFRE, mapadr.KisiAcikAdresBilgisiType.ilKodu, mapfreIlceKodu);

                            if (ilIlce != null)
                            {
                                model.IlKodu = ilIlce.IlKodu;
                                model.IlceKodu = ilIlce.IlceKodu;
                            }

                            model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                            model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.IlKodu), "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();

                            model.AdresTipi = MusteriTipleri.Ozel(model.MusteriTipKodu.Value) ? AdresTipleri.Ev : AdresTipleri.Is;
                            model.Semt = "";
                            model.Mahalle = mapadr.KisiAcikAdresBilgisiType.mahalle;
                            model.Cadde = mapadr.KisiAcikAdresBilgisiType.csbm;
                            model.Sokak = "";
                            model.Apartman = "";
                            model.BinaNo = mapadr.KisiAcikAdresBilgisiType.disKapiNo;
                            model.DaireNo = mapadr.KisiAcikAdresBilgisiType.icKapiNo;
                            model.PostaKodu = 0;
                            model.AcikAdres = mapadr.KisiAcikAdresBilgisiType.acikAdres;

                            model.DisableControls = true;
                        }
                    }

                    return Json(model);
                }
                else
                {
                    model.SorgulamaHata("");
                }
            }
            return Json(model);
        }

        public ActionResult KimlikNoSorgulaGulf(string kimlikNo, int? TVMKodu)
        {
            MusteriModel model = new MusteriModel();

            if (String.IsNullOrEmpty(kimlikNo))
            {
                model.SorgulamaHata("Kimlik numarası boş bırakılamaz");
            }
            else if (kimlikNo.Length == 11 || kimlikNo.Length == 10)
            {
                try
                {

                    IGULFKasko GulfKasko = DependencyResolver.Current.GetService<IGULFKasko>();
                    GULFMusteriBigileri kimlikSorguGulf = GulfKasko.GetGULFMusteriNo(kimlikNo);
                    if (kimlikSorguGulf != null && String.IsNullOrEmpty(kimlikSorguGulf.HataMesaji))
                    {
                        model.MusteriKodu = 0;
                        model.Email = String.Empty;
                        int tvm = _AktifKullaniciService.TVMKodu;
                        if (TVMKodu.HasValue && TVMKodu.Value > 0)
                            tvm = TVMKodu.Value;
                        MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(kimlikNo, tvm);
                        if (kimlikNo.Length == 11)
                        {
                            if (musteri != null)
                            {
                                model.TVMKodu = musteri.TVMKodu;
                                model.MusteriKodu = musteri.MusteriKodu;
                            }


                            model.MusteriTipKodu = MusteriTipleri.TCMusteri;
                            model.Uyruk = UyrukTipleri.TC;
                            if (kimlikNo[0] == '9')
                            {
                                model.MusteriTipKodu = MusteriTipleri.YabanciMusteri;
                                model.Uyruk = UyrukTipleri.Yabanci;
                            }

                            model.KimlikNo = kimlikNo;
                            model.PasaportNo = String.Empty;
                            model.VergiDairesi = String.Empty;
                            if (!String.IsNullOrEmpty(kimlikSorguGulf.Adi))
                            {
                                if (kimlikSorguGulf.Adi.Length > 150)
                                {
                                    model.AdiUnvan = kimlikSorguGulf.Adi.Substring(0, 150);
                                }
                                else
                                {
                                    model.AdiUnvan = kimlikSorguGulf.Adi;
                                }
                            }

                            if (!String.IsNullOrEmpty(kimlikSorguGulf.Soyadi))
                            {
                                if (kimlikSorguGulf.Soyadi.Length > 50)
                                {
                                    model.SoyadiUnvan = kimlikSorguGulf.Soyadi.Substring(0, 50);
                                }
                                else
                                {
                                    model.SoyadiUnvan = kimlikSorguGulf.Soyadi;
                                }
                            }

                            if (!String.IsNullOrEmpty(kimlikSorguGulf.DogumTarihi))
                            {
                                model.DogumTarihi = Convert.ToDateTime(kimlikSorguGulf.DogumTarihi);
                                model.DogumTarihiText = model.DogumTarihi.Value.ToString("dd.MM.yyyy");
                            }
                            if (!String.IsNullOrEmpty(kimlikSorguGulf.Cinsiyeti))
                            {
                                model.Cinsiyet = kimlikSorguGulf.Cinsiyeti;
                            }
                        }
                        else if (kimlikNo.Length == 10)
                        {
                            model.KimlikNo = kimlikNo;
                            model.MusteriTipKodu = MusteriTipleri.TuzelMusteri;
                            if (!String.IsNullOrEmpty(kimlikSorguGulf.FirmaAdi))
                                model.AdiUnvan = kimlikSorguGulf.FirmaAdi;
                            model.PasaportNo = String.Empty;
                            model.VergiDairesi = String.Empty;
                            model.SoyadiUnvan = ".";

                        }
                        if (!String.IsNullOrEmpty(kimlikSorguGulf.IlKodu))
                        {
                            //CR_IlIlce ilIlce = _CRService.GetIlIlceByCr(TeklifUretimMerkezleri.GULF, kimlikSorguGulf.IlKodu, kimlikSorguGulf.IlceKodu);
                            model.IlKodu = kimlikSorguGulf.IlKodu.PadLeft(2, '0');
                        }
                        else
                        {
                            model.IlKodu = "";
                        }
                        if (musteri != null)
                        {
                            MusteriAdre adres = musteri.MusteriAdres.FirstOrDefault(m => m.Varsayilan == true);

                            if (adres != null)
                            {
                                if (String.IsNullOrEmpty(model.IlKodu))
                                {
                                    model.IlKodu = adres.IlKodu;
                                }
                            }
                        }

                        model.IlceKodu = 0;
                        if (!String.IsNullOrEmpty(kimlikSorguGulf.MusteriNo))
                        {
                            model.GulfKimlikNo = kimlikSorguGulf.MusteriNo;
                        }

                        model.Iller = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlAdi", model.IlKodu).ListWithOptionLabelIller();
                        model.Ilceler = new SelectList(_UlkeService.GetIlceList("TUR", model.IlKodu), "IlceKodu", "IlceAdi", model.IlceKodu).ListWithOptionLabel();


                        //else if (!String.IsNullOrEmpty(kimlikNo) && kimlikNo.Length == 10)
                        //{
                        //    model.MusteriTipKodu = MusteriTipleri.TuzelMusteri;
                        //    model.KimlikNo = kimlikNo;
                        //    model.PasaportNo = String.Empty;
                        //    model.VergiDairesi = String.Empty;
                        //    if (!String.IsNullOrEmpty(kimlik.vrgnvan) && kimlik.vrgnvan.Length > 150)
                        //    {
                        //        model.AdiUnvan = kimlik.vrgnvan.Substring(0, 150);
                        //    }
                        //    else
                        //    {
                        //        model.AdiUnvan = kimlik.vrgnvan;
                        //    }
                        //    model.SoyadiUnvan = ".";
                        //}

                        // MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                        if (musteri != null)
                        {

                            MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                            if (telefon != null)
                            {
                                List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });
                                if (!String.IsNullOrEmpty(telefon.Numara))
                                {
                                    if (telefon.Numara.Length > 7)
                                    {
                                        model.CepTelefonu = telefon.Numara;
                                    }
                                }

                                model.MusteriTelTipKodu = telefon.IletisimNumaraTipi;
                                model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text", model.MusteriTelTipKodu).ListWithOptionLabel();
                            }
                            else
                            {
                                List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                                numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                                model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text").ListWithOptionLabel();
                                model.CepTelefonu = "90-";
                            }
                        }
                        else
                        {
                            List<SelectListItem> numaraTipleri = new List<SelectListItem>();
                            numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Ev.ToString(), Text = "Ev" });
                            numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Cep.ToString(), Text = "Cep" });
                            numaraTipleri.Add(new SelectListItem() { Value = IletisimNumaraTipleri.Is.ToString(), Text = "İş" });

                            model.MusteriTelTipleri = new SelectList(numaraTipleri, "Value", "Text").ListWithOptionLabel();
                            model.CepTelefonu = "90-";
                        }

                        model.DisableControls = true;

                        model.TVMKodu = TVMKodu;
                        return Json(model);

                    }
                    else
                    {
                        model.SorgulamaHata(kimlikSorguGulf.HataMesaji);
                        return Json(model);
                    }

                }
                catch (Exception)
                {
                    model.SorgulamaHata("Kimlik bilgileri sorgularken bir hata oluştu.");
                    return Json(model);
                }
            }

            model.SorgulamaHata("Girilen kimlik numarasına ait bilgi bulunamadı.");
            return Json(model);

        }

        [HttpPost]
        [AjaxException]
        public ActionResult MusteriKaydet(MusteriKaydetModel model)
        {
            SigortaliModel sigortaliModel = model.Musteri;
            sigortaliModel.EMailRequired = false;

            #region Sigorta ettiren kaydet
            if (!sigortaliModel.SigortaEttiren.MusteriKodu.HasValue ||
                sigortaliModel.SigortaEttiren.MusteriKodu == 0)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, model.Musteri.TVMKodu))
                {
                    return Json(sigortaliModel);
                }
            }

            if (!sigortaliModel.SigortaliAyni)
            {
                if (!this.MusteriKaydet(sigortaliModel.Sigortali, model.Musteri.TVMKodu))
                {
                    return Json(sigortaliModel);
                }
            }

            if (sigortaliModel.SigortaEttiren.TVMKodu != _AktifKullaniciService.TVMKodu)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, _AktifKullaniciService.TVMKodu))
                {
                    return Json(sigortaliModel);
                }
            }
            #endregion

            return Json(sigortaliModel);
        }
        [HttpPost]
        [AjaxException]
        public ActionResult MusteriKaydetDask(DaskMusteriKaydetModel model)
        {
            SigortaliModel sigortaliModel = new SigortaliModel();
            //if (model==null && !String.IsNullOrEmpty(kimlikNo))
            //{
            //    sigortaliModel.SigortaEttiren = new MusteriModel();
            //    sigortaliModel.SigortaEttiren.KimlikNo = kimlikNo;
            //    sigortaliModel.SigortaEttiren.MusteriTelTipKodu = 1;
            //}
            //else
            //{
            //     sigortaliModel = model.Musteri;
            //}

            sigortaliModel.EMailRequired = false;

            #region Sigorta ettiren kaydet
            if (!sigortaliModel.SigortaEttiren.MusteriKodu.HasValue ||
                sigortaliModel.SigortaEttiren.MusteriKodu == 0)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, model.Musteri.TVMKodu))
                {
                    return Json(sigortaliModel);
                }
            }

            if (!sigortaliModel.SigortaliAyni)
            {
                if (!this.MusteriKaydet(sigortaliModel.Sigortali, model.Musteri.TVMKodu))
                {
                    return Json(sigortaliModel);
                }
            }

            if (sigortaliModel.SigortaEttiren.TVMKodu != _AktifKullaniciService.TVMKodu)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, _AktifKullaniciService.TVMKodu))
                {
                    return Json(sigortaliModel);
                }
            }
            #endregion

            return Json(sigortaliModel);
        }

        [HttpGet]
        public ActionResult BilgilendirmeFormu(string fn, int id)
        {
            BilgilendirmeFormuModel model = new BilgilendirmeFormuModel();

            try
            {
                ITeklif teklif = _TeklifService.GetTeklif(id);
                ITeklif urunTeklif = TeklifUrunFactory.AsUrunClass(teklif);

                Hashtable ht = urunTeklif.BilgilendirmeFormu(fn);

                IEPostaService eposta = DependencyResolver.Current.GetService<IEPostaService>();
                EPostaFormatlari format = eposta.GetEPosta(fn);

                if (format == null)
                    throw new Exception(String.Format("Formatı bulunamadı: {0}", fn));

                model.SayfaBasligi = format.Konu;
                model.HtmlContent = format.Icerik;

                foreach (DictionaryEntry item in ht)
                {
                    model.HtmlContent = model.HtmlContent.Replace(item.Key.ToString(), item.Value.ToString());
                }
            }
            catch (Exception ex)
            {
                model.SayfaBasligi = "Bilgilendirme formu oluşturulamadı";
                model.HtmlContent = "<span style='color:red'>Bilgilendirme formu oluşturulamadı</span><br/><spanstyle='color:red'>" + ex.Message + "</span>";
                _LogService.Error(ex);
            }

            return View(model);
        }

        [HttpPost]
        [AjaxException]
        public ActionResult AracMarkaTip(string kullanimTarzi, string markaKodu, string tipKodu)
        {
            string[] parts = kullanimTarzi.Split('-');
            string kullanimTarziKodu = parts[0];
            tipKodu = tipKodu.TrimStart('0');
            IAracService _AracService = DependencyResolver.Current.GetService<IAracService>();
            List<AracModel> modeller = _AracService.GetAracModelList(markaKodu, tipKodu);
            if (modeller != null && modeller.Count > 0)
            {
                int modelYili = modeller.Max(m => m.Model);
                List<SelectListItem> markalar = new SelectList(_AracService.GetAracMarkaList(kullanimTarziKodu), "MarkaKodu", "MarkaAdi").ListWithOptionLabel();
                List<SelectListItem> tipler = new SelectList(_AracService.GetAracTipList(kullanimTarziKodu, markaKodu, modelYili), "TipKodu", "TipAdi").ListWithOptionLabel();

                return Json(new
                {
                    success = true,
                    markalar = markalar,
                    tipler = tipler,
                    model = modelYili
                });
            }

            return Json(new
            {
                success = false
            });
        }

        private bool MusteriKaydet(MusteriModel model, int? tvmKodu = null)
        {
            int _TVMKodu = _AktifKullaniciService.TVMKodu;
            if (tvmKodu.HasValue && tvmKodu.Value > 0)
                _TVMKodu = tvmKodu.Value;

            //Kimlik Numarası Kaydedilmişse hata dondürülüyor...
            MusteriGenelBilgiler genelBilgiler = _MusteriService.GetMusteriTeklifFor(model.KimlikNo, _TVMKodu);
            if (genelBilgiler != null)
            {
                model.SorgulamaHata("Bilgiler daha önce kaydedilmiş.");
                if (genelBilgiler.MusteriKodu > 0)
                {
                    model.MusteriKodu = genelBilgiler.MusteriKodu;
                    return true;
                }
                return false;
            }

            try
            {
                MusteriGenelBilgiler sigortaEttiren = new MusteriGenelBilgiler();
                if (String.IsNullOrEmpty(model.AdiUnvan))
                {
                    model.AdiUnvan = ".";
                }
                sigortaEttiren.AdiUnvan = model.AdiUnvan;
                sigortaEttiren.SoyadiUnvan = model.SoyadiUnvan;
                if (String.IsNullOrEmpty(sigortaEttiren.SoyadiUnvan))
                    sigortaEttiren.SoyadiUnvan = ".";

                sigortaEttiren.KimlikNo = model.KimlikNo;
                sigortaEttiren.MusteriTipKodu = model.MusteriTipKodu.HasValue ? model.MusteriTipKodu.Value : (short)0;
                sigortaEttiren.WebUrl = "";
                sigortaEttiren.EMail = model.Email;
                sigortaEttiren.Uyruk = model.Uyruk;

                sigortaEttiren.TVMMusteriKodu = "";
                sigortaEttiren.TVMKodu = _TVMKodu;
                sigortaEttiren.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
                sigortaEttiren.DogumTarihi = model.DogumTarihi;
                sigortaEttiren.Cinsiyet = model.Cinsiyet;

                MusteriAdre sigortaEttirenAdres = new MusteriAdre();
                sigortaEttirenAdres.SiraNo = 1;
                sigortaEttirenAdres.AdresTipi = AdresTipleri.Diger;
                sigortaEttirenAdres.UlkeKodu = "TUR";
                sigortaEttirenAdres.IlKodu = model.IlKodu;
                sigortaEttirenAdres.IlceKodu = model.IlceKodu;

                string ilAdi = _UlkeService.GetIlAdi(sigortaEttirenAdres.UlkeKodu, sigortaEttirenAdres.IlKodu);
                string ilceAdi = String.Empty;
                if (sigortaEttirenAdres.IlceKodu.HasValue)
                {
                    ilceAdi = _UlkeService.GetIlceAdi(sigortaEttirenAdres.IlceKodu.Value);
                }

                sigortaEttirenAdres.Adres = String.Format("{0} {1}", ilceAdi, ilAdi);
                sigortaEttirenAdres.Mahalle = "";
                sigortaEttirenAdres.Cadde = "";
                sigortaEttirenAdres.Sokak = "";
                sigortaEttirenAdres.Apartman = "";
                sigortaEttirenAdres.BinaNo = "";
                sigortaEttirenAdres.DaireNo = "";
                sigortaEttirenAdres.PostaKodu = 0;

                if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre)
                {
                    if (!String.IsNullOrEmpty(model.AcikAdres))
                        sigortaEttirenAdres.Adres = model.AcikAdres;
                    if (!String.IsNullOrEmpty(model.Mahalle))
                        sigortaEttirenAdres.Mahalle = model.Mahalle;
                    if (!String.IsNullOrEmpty(model.Cadde))
                        sigortaEttirenAdres.Cadde = model.Cadde;
                    if (!String.IsNullOrEmpty(model.BinaNo))
                        sigortaEttirenAdres.BinaNo = model.BinaNo;
                    if (!String.IsNullOrEmpty(model.DaireNo))
                        sigortaEttirenAdres.DaireNo = model.DaireNo;
                }

                MusteriTelefon sigortaEttirenTelefon = null;
                if (!String.IsNullOrEmpty(model.CepTelefonu))
                {
                    sigortaEttirenTelefon = new MusteriTelefon();
                    sigortaEttirenTelefon.SiraNo = 1;
                    sigortaEttirenTelefon.IletisimNumaraTipi = model.MusteriTelTipKodu;
                    // sigortaEttirenTelefon.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                    sigortaEttirenTelefon.Numara = model.CepTelefonu;
                }

                sigortaEttiren = _MusteriService.CreateMusteri(sigortaEttiren, sigortaEttirenAdres, sigortaEttirenTelefon);
                model.MusteriKodu = sigortaEttiren.MusteriKodu;
                model.TVMKodu = sigortaEttiren.TVMKodu;

            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
                model.SorgulamaHata("Müşteri bilgileri kaydedilemedi.");
                return false;
            }

            return true;
        }

        //Müşteri bilgilerinde değişiklik olmussa kaydediliyor.
        protected void MusteriGuncelle(MusteriModel model)
        {
            MusteriGenelBilgiler Musteri = _MusteriService.GetMusteri(model.MusteriKodu.Value);
            if (Musteri != null)
            {
                if (!String.IsNullOrEmpty(model.AdiUnvan))
                {
                    Musteri.AdiUnvan = model.AdiUnvan;
                }
                else
                {
                    Musteri.AdiUnvan = ".";
                }
                if (!String.IsNullOrEmpty(model.SoyadiUnvan))
                {
                    Musteri.SoyadiUnvan = model.SoyadiUnvan;
                }
                else
                {
                    Musteri.SoyadiUnvan = ".";
                }

                Musteri.DogumTarihi = model.DogumTarihi;
                Musteri.Cinsiyet = model.Cinsiyet;
                Musteri.EMail = model.Email;

                if ((Musteri.MusteriTipKodu == MusteriTipleri.SahisFirmasi || Musteri.MusteriTipKodu == MusteriTipleri.TuzelMusteri) &&
                    !String.IsNullOrEmpty(model.VergiDairesi))
                {
                    Musteri.VergiDairesi = model.VergiDairesi;
                }

                if (!String.IsNullOrEmpty(model.CepTelefonu))
                {
                    MusteriTelefon telefon = Musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == model.MusteriTelTipKodu);
                    if (telefon != null)
                    {
                        telefon.Numara = model.CepTelefonu;
                    }
                    else
                    {
                        MusteriTelefon cepTel = new MusteriTelefon();
                        //cepTel.IletisimNumaraTipi = IletisimNumaraTipleri.Cep;
                        cepTel.IletisimNumaraTipi = model.MusteriTelTipKodu;

                        cepTel.Numara = model.CepTelefonu;
                        cepTel.NumaraSahibi = Musteri.AdiUnvan + " " + Musteri.SoyadiUnvan;

                        if (!String.IsNullOrEmpty(cepTel.NumaraSahibi) && cepTel.NumaraSahibi.Length > 50)
                        {
                            cepTel.NumaraSahibi = cepTel.NumaraSahibi.Substring(0, 50);
                        }

                        if (Musteri.MusteriTelefons != null)
                        {
                            int sirano = Musteri.MusteriTelefons.Count > 0 ? Musteri.MusteriTelefons.Max(s => s.SiraNo) : 0;
                            cepTel.SiraNo = sirano + 1;
                            Musteri.MusteriTelefons.Add(cepTel);
                        }
                    }
                }

                // Mapfre sigorta musteri güncelleme aşamasında adres bilgileri de güncelleniyor.
                if (!String.IsNullOrEmpty(model.IlKodu) && model.IlceKodu.HasValue && model.IlceKodu.Value > 0)
                {
                    MusteriAdre adres = Musteri.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                    if (adres != null)
                    {
                        adres.IlKodu = model.IlKodu;
                        adres.IlceKodu = model.IlceKodu;
                        //adres.Adres = model.AcikAdres;
                    }
                }

                //Aynı kod iki defa yazıldığı için kaldırıldı.
                // Mapfre sigorta musteri güncelleme aşamasında adres bilgileri de güncelleniyor.
                //if (_AktifKullaniciService.ProjeKodu == TVMProjeKodlari.Mapfre &&
                //    !String.IsNullOrEmpty(model.IlKodu) && model.IlceKodu.HasValue && model.IlceKodu.Value > 0)
                //{
                //    MusteriAdre adres = Musteri.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                //    if (adres != null)
                //    {
                //        adres.IlKodu = model.IlKodu;
                //        adres.IlceKodu = model.IlceKodu;
                //        adres.Adres = model.AcikAdres;
                //    }
                //}

                _MusteriService.UpdateMusteri(Musteri);
            }
        }

        protected void TeklifHesaplaMusteriKaydet(ITeklif teklif, SigortaliModel Musteri)
        {
            if (Musteri != null)
            {
                if (Musteri.SigortaliAyni && Musteri.SigortaEttiren.MusteriKodu.HasValue)
                {
                    teklif.AddSigortali(Musteri.SigortaEttiren.MusteriKodu.Value);
                    MusteriGuncelle(Musteri.SigortaEttiren);
                }
                else if (Musteri.Sigortali.MusteriKodu.HasValue)
                {
                    teklif.AddSigortali(Musteri.Sigortali.MusteriKodu.Value);
                    MusteriGuncelle(Musteri.SigortaEttiren);
                    MusteriGuncelle(Musteri.Sigortali);
                }
            }
        }
        protected void LilyumTeklifHesaplaMusteriKaydet(ITeklif teklif, SigortaliModel Musteri)
        {
            if (Musteri != null)
            {
                if (Musteri.SigortaliAyni && Musteri.SigortaEttiren.MusteriKodu.HasValue)
                {
                    teklif.AddSigortali(Musteri.SigortaEttiren.MusteriKodu.Value);
                    MusteriGuncelle(Musteri.SigortaEttiren);
                }
                else if (Musteri.SigortaEttiren.MusteriKodu.HasValue)
                {
                    //teklif.AddSigortali(Musteri.SigortaEttiren.MusteriKodu.Value);
                    // MusteriGuncelle(Musteri.SigortaEttiren);
                    //Musteri.Sigortali = Musteri.SigortaEttiren;
                    //var sigortaliKayit = MusteriKaydet(Musteri.Sigortali);
                   
                }
            }
        }
        protected TeklifFiyatDetayModel TeklifFiyat(int tumKodu, List<IsDurumDetay> detaylar)
        {
            ITUMService tumService = DependencyResolver.Current.GetService<ITUMService>();
            TUMDetay tum = tumService.GetDetay(tumKodu);

            TeklifFiyatDetayModel fiyatModel = new TeklifFiyatDetayModel();
            fiyatModel.TUMKodu = tum.Kodu;
            fiyatModel.TUMUnvani = tum.Unvani;
            fiyatModel.TUMLogoUrl = tum.Logo;
            fiyatModel.Surprimler = new List<TeklifSurprimModel>();

            int[] teklifIdList = detaylar.Where(w => w.TUMKodu == tumKodu).Select(s => s.ReferansId).ToArray<int>();
            List<ITeklif> teklifler = _TeklifService.GetTeklifler(teklifIdList).OrderBy(o => o.OdemePlaniAlternatifKodu).ToList<ITeklif>();

            int teklifIndex = 0;
            foreach (var teklif in teklifler)
            {
                if (teklifIndex == 0)
                {
                    switch (teklif.GenelBilgiler.UrunKodu)
                    {
                        case UrunKodlari.YurtDisiSeyehatSaglik: YurtIciSeyehatParser(teklif, fiyatModel); break;
                        case UrunKodlari.TESabitPrimli: TuruncuElmaParser(teklif, fiyatModel); break;
                        case UrunKodlari.OdulluBirikim: OdulluBirikimParser(teklif, fiyatModel); break;
                        case UrunKodlari.PrimIadeli: PrimIadeliParser(teklif, fiyatModel); break;
                        case UrunKodlari.OdemeGuvence: OdemeGuvenceParser(teklif, fiyatModel); break;
                        case UrunKodlari.KorunanGelecek: KorunanGelecekParser(teklif, fiyatModel); break;
                        case UrunKodlari.PrimIadeli2: PrimIadeli2Parser(teklif, fiyatModel); break;
                        case UrunKodlari.Egitim: EgitimParser(teklif, fiyatModel); break;
                        default: DefaultFiyatParser(teklif, fiyatModel); break;
                    }
                }
                else if (teklifIndex == 1)
                {
                    fiyatModel.Fiyat2 = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " TL" : "";
                    fiyatModel.Fiyat2_TeklifId = teklif.GenelBilgiler.TeklifId;
                }
                else if (teklifIndex == 2)
                {
                    fiyatModel.Fiyat3 = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " TL" : "";
                    fiyatModel.Fiyat3_TeklifId = teklif.GenelBilgiler.TeklifId;
                }
                fiyatModel.TUMTeklifNo = teklif.GenelBilgiler.TUMTeklifNo;
                fiyatModel.TUMPDF = "";
                var uyariMesaji = teklif.GenelBilgiler.TeklifWebServisCevaps.Where(w => w.CevapKodu == WebServisCevaplar.TeklifUyariMesaji).FirstOrDefault();
                fiyatModel.TUMTeklifUyariMesaji = uyariMesaji != null ? uyariMesaji.Cevap : "";
                var bilgiMesaji = teklif.GenelBilgiler.TeklifWebServisCevaps.Where(w => w.CevapKodu == WebServisCevaplar.TeklifBilgiMesaji).FirstOrDefault();
                fiyatModel.TUMTeklifBilgiMesaji = bilgiMesaji != null ? bilgiMesaji.Cevap : "";
                if (!String.IsNullOrEmpty(teklif.GenelBilgiler.PDFDosyasi))
                {
                    fiyatModel.TUMPDF = String.Format("<a href=" + teklif.GenelBilgiler.PDFDosyasi + " title='Teklif PDF' target='_blank' class='pull-center'>" +
                                                                   "<img src='/content/img/pdf_icon.png' /></a>");
                }
                fiyatModel.TUMTeklifPDF = teklif.GenelBilgiler.PDFDosyasi;// Detay için kullanılıyor
                if (teklif.GenelBilgiler.UrunKodu == UrunKodlari.DogalAfetSigortasi_Deprem)
                {
                    fiyatModel.DaskUyariMesaji = teklif.ReadWebServisCevap(Business.Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, "");
                }
                var paratikaUrl = teklif.GenelBilgiler.TeklifWebServisCevaps.Where(w => w.CevapKodu == WebServisCevaplar.Koru3DParatikaToken).FirstOrDefault();
                string url = paratikaUrl != null ? paratikaUrl.Cevap : "";
                fiyatModel.LilyumParaticaURL = "https://entegrasyon.paratika.com.tr/payment/" + url;

                teklifIndex++;
            }

            if (String.IsNullOrEmpty(fiyatModel.Fiyat1) && String.IsNullOrEmpty(fiyatModel.Fiyat2) && String.IsNullOrEmpty(fiyatModel.Fiyat3))
            {
                List<string> hataListe = detaylar.Where(w => w.TUMKodu == tumKodu)
                                                 .GroupBy(g => g.HataMesaji)
                                                 .Select(s => s.Key)
                                                 .ToList<string>();

                fiyatModel.Hatalar = new List<string>();
                foreach (string hata in hataListe)
                {
                    string[] parts = hata.Split('|');

                    foreach (string h in parts)
                    {
                        fiyatModel.Hatalar.Add(h);
                    }
                }
            }


            return fiyatModel;
        }

        protected HazirlayanModel EkleHazirlayanModel()
        {
            HazirlayanModel hazirlayan = new HazirlayanModel();
            hazirlayan.KendiAdima = 1;
            hazirlayan.KendiAdimaList = new SelectList(TeklifListeleri.TeklifHazirlayanTipleri(), "Value", "Text", hazirlayan.KendiAdima);
            hazirlayan.TVMKodu = _AktifKullaniciService.TVMKodu;
            hazirlayan.TVMKullaniciKodu = _AktifKullaniciService.KullaniciKodu;
            hazirlayan.TVMUnvani = _AktifKullaniciService.TVMUnvani;
            hazirlayan.TVMKullaniciAdi = _AktifKullaniciService.AdiSoyadi;
            hazirlayan.YeniIsMi = false;

            return hazirlayan;
        }

        protected DetayHazirlayanModel DetayHazirlayanModel(ITeklif teklif)
        {
            DetayHazirlayanModel hazirlayan = new DetayHazirlayanModel();

            TVMDetay tvm = _TVMService.GetDetay(teklif.GenelBilgiler.TVMKodu);
            if (tvm != null)
                hazirlayan.TVMUnvani = tvm.Unvani;

            TVMKullanicilar kullanici = _KullaniciService.GetKullanici(teklif.GenelBilgiler.TVMKullaniciKodu);
            if (kullanici != null)
                hazirlayan.TVMKullaniciAdi = String.Format("{0} {1}", kullanici.Adi, kullanici.Soyadi);

            return hazirlayan;
        }

        protected MusteriModel EkleMusteriModel(int musteriKodu)
        {
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

            MusteriModel model = new MusteriModel();
            model.MusteriKodu = musteriKodu;
            model.MusteriTipKodu = musteri.MusteriTipKodu;
            model.KimlikNo = musteri.KimlikNo == "1AEGONAEGON" ? "" : musteri.KimlikNo;
            model.AdiUnvan = musteri.AdiUnvan.ToUpper(new System.Globalization.CultureInfo("tr-TR", false));
            model.SoyadiUnvan = musteri.SoyadiUnvan.ToUpper(new System.Globalization.CultureInfo("tr-TR", false));
            model.Email = musteri.EMail;
            model.Cinsiyet = musteri.Cinsiyet;
            model.VergiDairesi = musteri.VergiDairesi;
            model.DogumTarihi = musteri.DogumTarihi;
            model.DogumTarihiText = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : "";

            //AEGON sigorta müşterisinn gelir vergisi oranı getiriliyor.
            TVMDetay tvmdetay = _TVMService.GetDetay(musteri.TVMKodu);
            if (tvmdetay != null)
                if (!String.IsNullOrEmpty(musteri.CiroBilgisi) && tvmdetay.ProjeKodu == TVMProjeKodlari.Aegon)
                    model.GelirVergisiOrani = Convert.ToByte(musteri.CiroBilgisi);

            MusteriAdre adres = musteri.MusteriAdres.FirstOrDefault(f => f.Varsayilan.Value);
            if (adres != null)
            {
                model.AdresTipi = adres.AdresTipi.HasValue ? adres.AdresTipi.Value : 0;
                model.UlkeKodu = adres.UlkeKodu;
                model.IlKodu = adres.IlKodu;
                model.IlceKodu = adres.IlceKodu;
                model.Semt = adres.Semt;
                model.Mahalle = adres.Mahalle;
                model.Cadde = adres.Cadde;
                model.Sokak = adres.Sokak;
                model.Apartman = adres.Apartman;
                model.BinaNo = adres.BinaNo;
                model.DaireNo = adres.DaireNo;
                model.PostaKodu = adres.PostaKodu;
                model.AcikAdres = adres.Adres;
            }

            //MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon telefon = musteri.MusteriTelefons.FirstOrDefault();
            if (telefon != null)
            {
                model.CepTelefonu = telefon.Numara;
                model.MusteriTelTipKodu = telefon.IletisimNumaraTipi;
            }

            return model;
        }

        protected DetayMusteriModel DetayMusteriModel(int musteriKodu)
        {
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

            DetayMusteriModel model = new DetayMusteriModel();
            model.MusteriKodu = musteriKodu;
            model.MusteriTipText = nsmusteri.MusteriListProvider.GetMusteriTipiText(musteri.MusteriTipKodu);
            model.KimlikNo = musteri.KimlikNo;
            model.AdiUnvan = musteri.AdiUnvan;
            model.SoyadiUnvan = musteri.SoyadiUnvan;
            model.Email = musteri.EMail;

            MusteriAdre adres = _MusteriService.GetDefaultAdres(musteriKodu);
            if (adres != null)
            {
                IUlkeService ulkeService = DependencyResolver.Current.GetService<IUlkeService>();
                Ulke ulke = ulkeService.GetUlke(adres.UlkeKodu);
                if (ulke != null)
                {
                    model.UlkeAdi = ulke.UlkeAdi;

                    Il il = ulke.Ils.FirstOrDefault(f => f.IlKodu == adres.IlKodu);
                    if (il != null)
                    {
                        model.IlAdi = il.IlAdi;

                        Ilce ilce = il.Ilces.FirstOrDefault(f => f.IlceKodu == adres.IlceKodu);
                        if (ilce != null)
                            model.IlceAdi = ilce.IlceAdi;
                    }
                }
            }

            if (MusteriTipleri.Ozel(musteri.MusteriTipKodu))
            {
                ITanimService tanimService = DependencyResolver.Current.GetService<ITanimService>();

                if (musteri.EgitimDurumu.HasValue)
                {
                    GenelTanimlar egitimDurumu = tanimService.GetTanim("Egitim", musteri.EgitimDurumu.ToString());

                    if (egitimDurumu != null)
                    {
                        model.EgitimDurumu = egitimDurumu.Aciklama;
                    }
                }

                if (musteri.MeslekKodu.HasValue)
                {
                    Meslek meslek = tanimService.GetMeslek(musteri.MeslekKodu.Value);

                    if (meslek != null)
                    {
                        model.MeslekAdi = meslek.MeslekAdi;
                    }
                }

                MusteriTelefon cepTelefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                if (cepTelefon != null)
                {
                    model.CepTelefonu = new Web.Models.TelefonModel(cepTelefon.Numara);
                }
                else
                {
                    model.CepTelefonu = new Web.Models.TelefonModel();
                }
            }
            else
            {
                model.CepTelefonu = new Web.Models.TelefonModel();
            }

            return model;
        }

        protected AegonDetayMusteriModel AegonDetayMusteriModel(int musteriKodu)
        {
            MusteriGenelBilgiler musteri = _MusteriService.GetMusteri(musteriKodu);

            AegonDetayMusteriModel model = new AegonDetayMusteriModel();

            model.MusteriKodu = musteriKodu;
            model.KimlikNo = musteri.KimlikNo == "1AEGONAEGON" ? "" : musteri.KimlikNo;
            model.AdiUnvan = musteri.AdiUnvan;
            model.SoyadiUnvan = musteri.SoyadiUnvan;
            model.DogumTarihi = musteri.DogumTarihi.HasValue ? musteri.DogumTarihi.Value.ToString("dd.MM.yyyy") : "";
            model.Uyruk = musteri.Uyruk == 0 ? "TC" : "Yabancı";

            switch (musteri.CiroBilgisi)
            {
                case "1": model.GelirVergisiOrani = "15.00%"; break;
                case "2": model.GelirVergisiOrani = "20.00%"; break;
                case "3": model.GelirVergisiOrani = "27.00%"; break;
                case "4": model.GelirVergisiOrani = "35.00%"; break;
                case "5": model.GelirVergisiOrani = "Beyan Edilmemiştir."; break;
            }

            model.Cinsiyet = musteri.Cinsiyet == "E" ? "Erkek" : "Kadın";
            model.Email = musteri.EMail;

            MusteriAdre adres = _MusteriService.GetDefaultAdres(musteriKodu);
            if (adres != null)
            {
                IUlkeService ulkeService = DependencyResolver.Current.GetService<IUlkeService>();
                Ulke ulke = ulkeService.GetUlke(adres.UlkeKodu);
                if (ulke != null)
                {
                    Il il = ulke.Ils.FirstOrDefault(f => f.IlKodu == adres.IlKodu);
                    if (il != null)
                    {
                        model.IlAdi = il.IlAdi;

                        Ilce ilce = il.Ilces.FirstOrDefault(f => f.IlceKodu == adres.IlceKodu);
                        if (ilce != null)
                            model.IlceAdi = ilce.IlceAdi;
                    }
                }
            }

            MusteriTelefon cepTelefon = musteri.MusteriTelefons.FirstOrDefault(f => f.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            if (cepTelefon != null)
            {
                model.CepTelefonu = cepTelefon.Numara;
            }

            return model;
        }

        protected AracBilgiModel EkleAracModel()
        {
            AracBilgiModel model = new AracBilgiModel();
            model.PlakaKodu = "34";

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();

            model.PlakaKoduListe = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlKodu", "34");
            model.KullanimSekilleri = new SelectList(aracService.GetAracKullanimSekliList(), "KullanimSekliKodu", "KullanimSekli", "").ListWithOptionLabel();
            model.KullanimTarzlari = EmptySelectList.EmptyList();
            model.Markalar = EmptySelectList.EmptyList();

            List<int> yillar = new List<int>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 1966; yil--)
                yillar.Add(yil);

            model.Modeller = new SelectList(yillar).ListWithOptionLabel();
            model.AracTipleri = EmptySelectList.EmptyList();

            return model;
        }

        protected KaskoAracBilgiModel KaskoEkleAracModel()
        {
            KaskoAracBilgiModel model = new KaskoAracBilgiModel();
            model.PlakaKodu = "34";

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();

            model.PlakaKoduListe = new SelectList(_UlkeService.GetIlList("TUR"), "IlKodu", "IlKodu", "34");
            model.KullanimSekilleri = new SelectList(aracService.GetAracKullanimSekliList(), "KullanimSekliKodu", "KullanimSekli", "").ListWithOptionLabel();
            model.KullanimTarzlari = EmptySelectList.EmptyList();
            model.Markalar = EmptySelectList.EmptyList();

            List<int> yillar = new List<int>();
            for (int yil = TurkeyDateTime.Today.Year; yil >= 1966; yil--)
                yillar.Add(yil);

            List<short> kisiler = new List<short>();
            for (short sayi = 1; sayi < 30; sayi++)
                kisiler.Add(sayi);

            model.Modeller = new SelectList(yillar).ListWithOptionLabel();
            model.AracTipleri = EmptySelectList.EmptyList();
            model.KisiSayisiListe = new SelectList(kisiler).ListWithOptionLabel();

            return model;
        }

        protected DetayAracBilgiModel DetayAracModel(ITeklif teklif)
        {
            TeklifArac arac = teklif.Arac;

            DetayAracBilgiModel model = new DetayAracBilgiModel();
            model.PlakaKodu = arac.PlakaKodu;
            model.PlakaNo = arac.PlakaNo;

            IAracService aracService = DependencyResolver.Current.GetService<IAracService>();

            short kullanimSekliKodu = Convert.ToInt16(arac.KullanimSekli);
            AracKullanimSekli kullanimSekli = aracService.GetAracKullanimSekli(kullanimSekliKodu);

            model.KullanimSekli = kullanimSekli != null ? kullanimSekli.KullanimSekli : String.Empty;

            if(arac.KullanimTarzi == null)
            {
                arac.KullanimTarzi = "111-10";
            }
            string[] parts = arac.KullanimTarzi.Split('-');
            if (parts.Length == 2)
            {
                AracKullanimTarzi kullanimTarzi = aracService.GetAracKullanimTarzi(parts[0], parts[1]);
                model.KullanimTarzi = kullanimTarzi != null ? kullanimTarzi.KullanimTarzi : String.Empty;
            }

            AracMarka marka = aracService.GetAracMarka(arac.Marka);
            model.Marka = marka != null ? marka.MarkaAdi : String.Empty;

            model.Model = arac.Model.HasValue ? arac.Model.Value : 0;

            AracTip tip = aracService.GetAracTip(arac.Marka, arac.AracinTipi);
            model.Tip = tip != null ? tip.TipAdi : String.Empty;

            model.MotorNo = arac.MotorNo;
            model.SaseNo = arac.SasiNo;

            string tescilIlKodu = teklif.Arac.TescilIlKodu;
            string tescilIlceKodu = teklif.Arac.TescilIlceKodu;

            if (!String.IsNullOrEmpty(tescilIlKodu))
            {
                CR_TescilIlIlce ilIlce = _CRService.GetTescilIlIlce(TeklifUretimMerkezleri.HDI, tescilIlKodu, tescilIlceKodu);

                if (ilIlce != null)
                {
                    model.TescilIl = ilIlce.TescilIlAdi;
                    model.TescilIlce = ilIlce.TescilIlceAdi;
                }
            }

            if (arac.TrafikTescilTarihi.HasValue)
            {
                model.TrafikTescilTarihi = arac.TrafikTescilTarihi.Value.ToString("dd.MM.yyyy");
            }
            if (arac.TrafikCikisTarihi.HasValue)
            {
                model.TrafigeCikisTarihi = arac.TrafikCikisTarihi.Value.ToString("dd.MM.yyyy");
            }

            DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, DateTime.MinValue);
            if (polBaslangic != DateTime.MinValue)
            {
                model.PoliceBaslangicTarihi = polBaslangic.ToString("dd.MM.yyyy");
            }

            model.TescilBelgeSeriKod = teklif.Arac.TescilSeriKod;
            model.TescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
            model.AsbisNo = teklif.Arac.AsbisNo;
            model.Deger = teklif.Arac.AracDeger;


            //model.MotorGucu = teklif.Arac.MotorGucu;
            //model.SilindirHacmi = teklif.Arac.SilindirHacmi;
            //model.Renk = teklif.Arac.Renk;
            //model.TramerBelgeNo = teklif.Arac.TramerBelgeNo;
            //model.TramerBelgeTarihi = teklif.Arac.TramerBelgeTarihi;
            //model.ImalatYeri = teklif.Arac.ImalatYeri.HasValue? teklif.Arac.ImalatYeri.Value : (byte) 0;


            return model;
        }

        protected EskiPoliceModel EkleEskiPoliceModel()
        {
            EskiPoliceModel model = new EskiPoliceModel();

            model.EskiPoliceVar = false;
            model.SigortaSirketleri = this.SigortaSirketleri;

            return model;
        }

        protected DetayEskiPoliceModel DetayEskiPoliceModel(ITeklif teklif)
        {
            DetayEskiPoliceModel model = new DetayEskiPoliceModel();

            model.EskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);

            if (model.EskiPoliceVar)
            {
                string sigortaSirketiKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);

                ISigortaSirketleriService _SigortaSirketService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
                SigortaSirketleri sigortaSirketi = _SigortaSirketService.GetSirket(sigortaSirketiKodu);

                if (sigortaSirketi != null)
                    model.SigortaSirketiAdi = sigortaSirketi.SirketAdi;

                model.AcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                model.PoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                model.YenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
            }

            return model;
        }

        protected TasiyiciSorumlulukModel EkleTasiyiciSorumlulukModel()
        {
            TasiyiciSorumlulukModel model = new TasiyiciSorumlulukModel();
            model.YetkiBelgesi = false;
            model.Sorumluluk = false;
            model.SigortaSirketleri = this.SigortaSirketleri;

            return model;
        }

        protected KaskoTeminatModel KaskoTeminatModel()
        {
            KaskoTeminatModel model = new KaskoTeminatModel();
            model.KaskoTurleri = TeklifListeleri.KaskoTurleri();
            model.KaskoServisleri = TeklifListeleri.KaskoServisTurleri();
            model.KaskoYedekParcalari = TeklifListeleri.KaskoYedekParcaTuru();
            model.Yurtdisi_Teminat_Sureleri = TeklifListeleri.KaskoYurtDisiTeminatSureleri();
            model.Hukuksal_Koruma_Teminati = true;

            model.Saglik_Kisi_Sayilari = new List<SelectListItem>();
            for (int sayi = 1; sayi < 8; sayi++)
                model.Saglik_Kisi_Sayilari.Add(new SelectListItem() { Value = sayi.ToString(), Text = sayi.ToString() });

            model.LPGAracModel = new LPGAracModel();
            model.LPG_Arac_Orjinalmi = true;

            return model;
        }

        protected DetayTasiyiciSorumlulukModel DetayTasiyiciSorumlulukModel(ITeklif teklif)
        {
            DetayTasiyiciSorumlulukModel model = new DetayTasiyiciSorumlulukModel();
            model.YetkiBelgesi = teklif.ReadSoru(TrafikSorular.Tasima_Yetki_Belgesi_VarYok, false);
            model.Sorumluluk = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, false);

            if (model.Sorumluluk)
            {
                string sigortaSirketiKodu = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);

                ISigortaSirketleriService _SigortaSirketService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
                SigortaSirketleri sigortaSirketi = _SigortaSirketService.GetSirket(sigortaSirketiKodu);

                if (sigortaSirketi != null)
                    model.SigortaSirketiAdi = sigortaSirketi.SirketAdi;

                model.AcenteNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                model.PoliceNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                model.YenilemeNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
            }

            return model;
        }

        protected KaskoPoliceOdemeModel KaskoPoliceOdemeModel(ITeklif teklif)
        {
            KaskoPoliceOdemeModel model = new KaskoPoliceOdemeModel();

            if (teklif != null && teklif.GenelBilgiler != null)
            {

                model.BrutPrim = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value : 0;
                model.NetPrim = teklif.GenelBilgiler.NetPrim.HasValue ? teklif.GenelBilgiler.NetPrim.Value : 0;
                model.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : 0;
                model.Vergi = teklif.GenelBilgiler.ToplamVergi.HasValue ? teklif.GenelBilgiler.ToplamVergi.Value : 0;

                model.TUMKodu = teklif.GenelBilgiler.TUMKodu;

                TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);

                model.TUMUnvani = tum.Unvani;
                model.TUMLogoURL = tum.Logo;
                model.PoliceURL = teklif.GenelBilgiler.PDFPolice;

                model.teklifId = teklif.GenelBilgiler.TeklifId;
                model.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
            }
            return model;
        }

        protected TSSPoliceOdemeModel TSSPoliceOdemeModel(ITeklif teklif)
        {
            TSSPoliceOdemeModel model = new TSSPoliceOdemeModel();

            if (teklif != null && teklif.GenelBilgiler != null)
            {

                model.BrutPrim = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value : 0;
                model.NetPrim = teklif.GenelBilgiler.NetPrim.HasValue ? teklif.GenelBilgiler.NetPrim.Value : 0;
                model.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : 0;
                model.Vergi = teklif.GenelBilgiler.ToplamVergi.HasValue ? teklif.GenelBilgiler.ToplamVergi.Value : 0;

                model.TUMKodu = teklif.GenelBilgiler.TUMKodu;

                TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);

                model.TUMUnvani = tum.Unvani;
                model.TUMLogoURL = tum.Logo;
                model.PoliceURL = teklif.GenelBilgiler.PDFPolice;

                model.teklifId = teklif.GenelBilgiler.TeklifId;
                model.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
            }
            return model;
        }

        protected LilyumPoliceOdemeModel LilyumPoliceOdemeModel(ITeklif teklif)
        {
            LilyumPoliceOdemeModel model = new LilyumPoliceOdemeModel();

            if (teklif != null && teklif.GenelBilgiler != null)
            {

                model.BrutPrim = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value : 0;
                model.NetPrim = teklif.GenelBilgiler.NetPrim.HasValue ? teklif.GenelBilgiler.NetPrim.Value : 0;
                model.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.HasValue ? teklif.GenelBilgiler.TaksitSayisi.Value : 0;
                model.Vergi = teklif.GenelBilgiler.ToplamVergi.HasValue ? teklif.GenelBilgiler.ToplamVergi.Value : 0;

                model.TUMKodu = teklif.GenelBilgiler.TUMKodu;

                TUMDetay tum = _TUMService.GetDetay(teklif.GenelBilgiler.TUMKodu);

                model.TUMUnvani = tum.Unvani;
                model.TUMLogoURL = tum.Logo;
                model.PoliceURL = teklif.GenelBilgiler.PDFPolice;

                model.teklifId = teklif.GenelBilgiler.TeklifId;
                model.TUMPoliceNo = teklif.GenelBilgiler.TUMPoliceNo;
            }
            return model;
        }
        private List<SelectListItem> _SigortaSirketleri;
        protected List<SelectListItem> SigortaSirketleri
        {
            get
            {
                if (_SigortaSirketleri == null)
                {
                    ISigortaSirketleriService _SigortaSirketService = DependencyResolver.Current.GetService<ISigortaSirketleriService>();
                    List<SigortaSirketleri> sigortaSirketleri = _SigortaSirketService.GetList();

                    _SigortaSirketleri = new SelectList(sigortaSirketleri, "SirketKodu", "SirketAdi", "").ListWithOptionLabel();
                }

                return _SigortaSirketleri;
            }
        }

        protected void ModelStateMusteriClear(ModelStateDictionary ModalState, SigortaliModel Musteri)
        {

            if (ModelState["Musteri.SigortaEttiren.AdiUnvan"] != null)
                ModelState["Musteri.SigortaEttiren.AdiUnvan"].Errors.Clear();

            if (ModelState["Musteri.SigortaEttiren.SoyadiUnvan"] != null)
                ModelState["Musteri.SigortaEttiren.SoyadiUnvan"].Errors.Clear();
            if (ModelState["Musteri.SigortaEttiren.MusteriTelTipKodu"] != null)
                ModelState["Musteri.SigortaEttiren.MusteriTelTipKodu"].Errors.Clear();

            if (ModelState["Musteri.Sigortali.AdiUnvan"] != null)
                ModelState["Musteri.Sigortali.AdiUnvan"].Errors.Clear();

            if (ModelState["Musteri.Sigortali.SoyadiUnvan"] != null)
                ModelState["Musteri.Sigortali.SoyadiUnvan"].Errors.Clear();
            if (ModelState["Musteri.Sigortali.MusteriTelTipKodu"] != null)
                ModelState["Musteri.Sigortali.MusteriTelTipKodu"].Errors.Clear();
            if (ModelState["Musteri.Sigortali.CepTelefonu"] != null)
                ModelState["Musteri.Sigortali.CepTelefonu"].Errors.Clear();
            if (Musteri.SigortaliAyni)
            {
                if (ModelState["Musteri.Sigortali.MusteriTipKodu"] != null)
                    ModelState["Musteri.Sigortali.MusteriTipKodu"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.KimlikNo"] != null)
                    ModelState["Musteri.Sigortali.KimlikNo"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.AdiUnvan"] != null)
                    ModelState["Musteri.Sigortali.AdiUnvan"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.SoyadiUnvan"] != null)
                    ModelState["Musteri.Sigortali.SoyadiUnvan"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.UlkeKodu"] != null)
                    ModelState["Musteri.Sigortali.UlkeKodu"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.IlKodu"] != null)
                    ModelState["Musteri.Sigortali.IlKodu"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.IlceKodu"] != null)
                    ModelState["Musteri.Sigortali.IlceKodu"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.DogumTarihi"] != null)
                    ModelState["Musteri.Sigortali.DogumTarihi"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.Email"] != null)
                    ModelState["Musteri.Sigortali.Email"].Errors.Clear();

                if (ModelState["Musteri.Sigortali.VergiDairesi"] != null)
                    ModelState["Musteri.Sigortali.VergiDairesi"].Errors.Clear();


                if (ModelState["Musteri.Sigortali.AcikAdres"] != null)
                    ModelState["Musteri.Sigortali.AcikAdres"].Errors.Clear();
            }
            else
            {
                if (!Musteri.Sigortali.MusteriTipKodu.HasValue && Musteri.Sigortali.MusteriKodu.HasValue)
                {
                    MusteriGenelBilgiler sigortalimusteri = _MusteriService.GetMusteri(Musteri.Sigortali.MusteriKodu.Value);
                    if (sigortalimusteri != null)
                        Musteri.Sigortali.MusteriTipKodu = sigortalimusteri.MusteriTipKodu;

                    if (ModelState["Musteri.Sigortali.MusteriTipKodu"] != null)
                        ModelState["Musteri.Sigortali.MusteriTipKodu"].Errors.Clear();
                }

                short sigortalitipkodu = Musteri.Sigortali.MusteriTipKodu ?? 1;

                if (sigortalitipkodu == MusteriTipleri.TCMusteri || sigortalitipkodu == MusteriTipleri.YabanciMusteri)
                {
                    if (ModelState["Musteri.Sigortali.VergiDairesi"] != null)
                        ModelState["Musteri.Sigortali.VergiDairesi"].Errors.Clear();
                }
            }

            if (!Musteri.SigortaEttiren.MusteriTipKodu.HasValue && Musteri.SigortaEttiren.MusteriKodu.HasValue)
            {
                MusteriGenelBilgiler sigortaliettirenmusteri = _MusteriService.GetMusteri(Musteri.SigortaEttiren.MusteriKodu.Value);
                if (sigortaliettirenmusteri != null)
                    Musteri.SigortaEttiren.MusteriTipKodu = sigortaliettirenmusteri.MusteriTipKodu;

                if (ModelState["Musteri.SigortaEttiren.MusteriTipKodu"] != null)
                    ModelState["Musteri.SigortaEttiren.MusteriTipKodu"].Errors.Clear();
            }

            short sigortaettirenlitipkodu = Musteri.SigortaEttiren.MusteriTipKodu ?? 1;

            if (sigortaettirenlitipkodu == MusteriTipleri.TCMusteri || sigortaettirenlitipkodu == MusteriTipleri.YabanciMusteri)
            {
                if (ModelState["Musteri.SigortaEttiren.VergiDairesi"] != null)
                    ModelState["Musteri.SigortaEttiren.VergiDairesi"].Errors.Clear();
            }

            if (ModelState["Musteri.Sigortali.IlKodu"] != null)
                ModelState["Musteri.Sigortali.IlKodu"].Errors.Clear();

            if (ModelState["Musteri.Sigortali.IlceKodu"] != null)
                ModelState["Musteri.Sigortali.IlceKodu"].Errors.Clear();

            if (ModelState["Musteri.Sigortali.AcikAdres"] != null)
                ModelState["Musteri.Sigortali.AcikAdres"].Errors.Clear();

            if (ModelState["Musteri.SigortaEttiren.IlKodu"] != null)
                ModelState["Musteri.SigortaEttiren.IlKodu"].Errors.Clear();

            if (ModelState["Musteri.SigortaEttiren.IlceKodu"] != null)
                ModelState["Musteri.SigortaEttiren.IlceKodu"].Errors.Clear();

            if (ModelState["Musteri.SigortaEttiren.AcikAdres"] != null)
                ModelState["Musteri.SigortaEttiren.AcikAdres"].Errors.Clear();

            if (ModelState["Musteri.SigortaEttiren.CepTelefonu"] != null)
                ModelState["Musteri.SigortaEttiren.CepTelefonu"].Errors.Clear();
        }
        protected void ModelStateMusteriClearDask(ModelStateDictionary ModalState, SigortaliModel Musteri)
        {
            if (Musteri.SigortaliAyni)
            {
                if (ModelState["DaskMusteri.Sigortali.MusteriTipKodu"] != null)
                    ModelState["DaskMusteri.Sigortali.MusteriTipKodu"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.KimlikNo"] != null)
                    ModelState["DaskMusteri.Sigortali.KimlikNo"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.AdiUnvan"] != null)
                    ModelState["DaskMusteri.Sigortali.AdiUnvan"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.SoyadiUnvan"] != null)
                    ModelState["DaskMusteri.Sigortali.SoyadiUnvan"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.UlkeKodu"] != null)
                    ModelState["DaskMusteri.Sigortali.UlkeKodu"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.IlKodu"] != null)
                    ModelState["DaskMusteri.Sigortali.IlKodu"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.IlceKodu"] != null)
                    ModelState["DaskMusteri.Sigortali.IlceKodu"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.DogumTarihi"] != null)
                    ModelState["DaskMusteri.Sigortali.DogumTarihi"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.Email"] != null)
                    ModelState["DaskMusteri.Sigortali.Email"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.VergiDairesi"] != null)
                    ModelState["DaskMusteri.Sigortali.VergiDairesi"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.CepTelefonu"] != null)
                    ModelState["DaskMusteri.Sigortali.CepTelefonu"].Errors.Clear();

                if (ModelState["DaskMusteri.Sigortali.AcikAdres"] != null)
                    ModelState["DaskMusteri.Sigortali.AcikAdres"].Errors.Clear();
            }
            else
            {
                if (!Musteri.Sigortali.MusteriTipKodu.HasValue && Musteri.Sigortali.MusteriKodu.HasValue)
                {
                    MusteriGenelBilgiler sigortalimusteri = _MusteriService.GetMusteri(Musteri.Sigortali.MusteriKodu.Value);
                    if (sigortalimusteri != null)
                        Musteri.Sigortali.MusteriTipKodu = sigortalimusteri.MusteriTipKodu;

                    if (ModelState["DaskMusteri.Sigortali.MusteriTipKodu"] != null)
                        ModelState["DaskMusteri.Sigortali.MusteriTipKodu"].Errors.Clear();
                }

                short sigortalitipkodu = Musteri.Sigortali.MusteriTipKodu ?? 1;

                if (sigortalitipkodu == MusteriTipleri.TCMusteri || sigortalitipkodu == MusteriTipleri.YabanciMusteri)
                {
                    if (ModelState["DaskMusteri.Sigortali.VergiDairesi"] != null)
                        ModelState["DaskMusteri.Sigortali.VergiDairesi"].Errors.Clear();
                }
            }

            if (!Musteri.SigortaEttiren.MusteriTipKodu.HasValue && Musteri.SigortaEttiren.MusteriKodu.HasValue)
            {
                MusteriGenelBilgiler sigortaliettirenmusteri = _MusteriService.GetMusteri(Musteri.SigortaEttiren.MusteriKodu.Value);
                if (sigortaliettirenmusteri != null)
                    Musteri.SigortaEttiren.MusteriTipKodu = sigortaliettirenmusteri.MusteriTipKodu;

                if (ModelState["DaskMusteri.SigortaEttiren.MusteriTipKodu"] != null)
                    ModelState["DaskMusteri.SigortaEttiren.MusteriTipKodu"].Errors.Clear();
            }

            short sigortaettirenlitipkodu = Musteri.SigortaEttiren.MusteriTipKodu ?? 1;

            if (sigortaettirenlitipkodu == MusteriTipleri.TCMusteri || sigortaettirenlitipkodu == MusteriTipleri.YabanciMusteri)
            {
                if (ModelState["DaskMusteri.SigortaEttiren.VergiDairesi"] != null)
                    ModelState["DaskMusteri.SigortaEttiren.VergiDairesi"].Errors.Clear();
            }

            if (ModelState["DaskMusteri.Sigortali.IlKodu"] != null)
                ModelState["DaskMusteri.Sigortali.IlKodu"].Errors.Clear();

            if (ModelState["DaskMusteri.Sigortali.IlceKodu"] != null)
                ModelState["DaskMusteri.Sigortali.IlceKodu"].Errors.Clear();

            if (ModelState["DaskMusteri.Sigortali.AcikAdres"] != null)
                ModelState["DaskMusteri.Sigortali.AcikAdres"].Errors.Clear();

            if (ModelState["DaskMusteri.SigortaEttiren.IlKodu"] != null)
                ModelState["DaskMusteri.SigortaEttiren.IlKodu"].Errors.Clear();

            if (ModelState["DaskMusteri.SigortaEttiren.IlceKodu"] != null)
                ModelState["DaskMusteri.SigortaEttiren.IlceKodu"].Errors.Clear();

            if (ModelState["DaskMusteri.SigortaEttiren.AcikAdres"] != null)
                ModelState["DaskMusteri.SigortaEttiren.AcikAdres"].Errors.Clear();

        }

        private void YurtIciSeyehatParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            
            if (teklif.GenelBilgiler.DovizKurBedeli == 1)
                fiyatModel.Fiyat1 = (teklif.GenelBilgiler.BrutPrim.Value).ToString("N2") + " TL";
            else
            {
                decimal? dovizKuru = teklif.GenelBilgiler.DovizKurBedeli;
                decimal prim = teklif.GenelBilgiler.BrutPrim.Value;
                fiyatModel.Fiyat1 = ((decimal)(prim * dovizKuru)).ToString("N2") + " TL" + " | " + prim.ToString("N2") + " EURO | Euro Kuru = " + ((decimal)dovizKuru).ToString("N2");
            }

            fiyatModel.Fiyat1_TeklifId = teklif.GenelBilgiler.TeklifId;

            ITeklif anaTeklif = _TeklifService.GetAnaTeklif(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu);

            if (anaTeklif.ReadSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, false))
            {
                TeklifSurprimModel surprim = new TeklifSurprimModel();
                surprim.SurprimAciklama = babonline.SkiDeposit;
                surprim.Surprim = "% 25";
                surprim.SurprimIS = "S";
                fiyatModel.Surprimler.Add(surprim);
            }
        }

        private void DefaultFiyatParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            fiyatModel.Fiyat1 = teklif.GenelBilgiler.BrutPrim.HasValue ? teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " TL" : "";
            fiyatModel.Fiyat1_TeklifId = teklif.GenelBilgiler.TeklifId;

            fiyatModel.KomisyonTutari = teklif.GenelBilgiler.ToplamKomisyon.HasValue ? teklif.GenelBilgiler.ToplamKomisyon.Value.ToString("N2") + " TL" : "";

            fiyatModel.merkezAcenteMi = false;

            var MerkezAcentemi = _TVMService.MerkezAcenteMi(_AktifKullaniciService.TVMKodu);

            if (MerkezAcentemi)
            {
                fiyatModel.merkezAcenteMi = true;
            }

            decimal komisyonOrani = 0;
            if (teklif.GenelBilgiler.ToplamKomisyon.HasValue && teklif.GenelBilgiler.NetPrim.HasValue)
            {
                if (teklif.GenelBilgiler.ToplamKomisyon.Value > 0 && teklif.GenelBilgiler.NetPrim.Value > 0)
                {
                    komisyonOrani = Math.Round((teklif.GenelBilgiler.ToplamKomisyon.Value / teklif.GenelBilgiler.NetPrim.Value) * 100, 2);
                }
                else
                {
                    komisyonOrani = 0;
                }
                fiyatModel.KomisyonOrani = " %" + komisyonOrani.ToString();
            }

            if (teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.HasValue && teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi > 0)
            {
                fiyatModel.Hasarsizlik = "- %" + teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.Value.ToString("N2");
                fiyatModel.HasarIndirimSurprim = "I";
            }
            if (teklif.GenelBilgiler.HasarSurprimYuzdesi.HasValue && teklif.GenelBilgiler.HasarSurprimYuzdesi > 0)
            {
                fiyatModel.Hasarsizlik = "+ %" + teklif.GenelBilgiler.HasarSurprimYuzdesi.Value.ToString("N2");
                fiyatModel.HasarIndirimSurprim = "S";
            }
            if (teklif.GenelBilgiler.GecikmeZammiYuzdesi.HasValue && teklif.GenelBilgiler.GecikmeZammiYuzdesi > 0)
            {
                TeklifSurprimModel surprim = new TeklifSurprimModel();
                surprim.SurprimAciklama = babonline.DelayHike;
                surprim.Surprim = "%" + teklif.GenelBilgiler.GecikmeZammiYuzdesi.Value.ToString("N2");
                surprim.SurprimIS = "S";
                fiyatModel.Surprimler.Add(surprim);
            }
            if (teklif.GenelBilgiler.PlakaIndirimYuzdesi.HasValue && teklif.GenelBilgiler.PlakaIndirimYuzdesi > 0)
            {
                TeklifSurprimModel surprim = new TeklifSurprimModel();
                surprim.SurprimAciklama = babonline.PlateSale;
                surprim.Surprim = "%" + teklif.GenelBilgiler.PlakaIndirimYuzdesi.Value.ToString("N2");
                surprim.SurprimIS = "I";
                fiyatModel.Surprimler.Add(surprim);
            }
        }

        protected void SetIlgiliTeklif(ITeklif teklif)
        {
            if (teklif.GenelBilgiler.IlgiliTeklifId.HasValue)
            {
                _TeklifService.SetIlgiliTeklifId(teklif);
            }
        }

        #region Aegon Method

        private void TuruncuElmaParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            string brutPrim = String.Empty;
            string paraBirimi = String.Empty;

            TeklifSoru ParaBirimi = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, TESabitPrimliSorular.ParaBirimi);
            if (ParaBirimi != null)
            {
                switch (ParaBirimi.Cevap)
                {
                    case "1": paraBirimi = "€"; break;
                    case "2": paraBirimi = "$"; break;
                }
            }

            #region Brut Prim

            TeklifSoru hesaplamaSecenegi = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, TESabitPrimliSorular.HesaplamaSecenegi);
            if (hesaplamaSecenegi != null)
            {
                switch (hesaplamaSecenegi.Cevap)
                {
                    case "1":
                        {
                            if (teklif.GenelBilgiler.BrutPrim.HasValue && teklif.GenelBilgiler.NetPrim.HasValue)
                                brutPrim = teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " " + paraBirimi + " / " +
                                           teklif.GenelBilgiler.NetPrim.Value.ToString("N2") + " " + paraBirimi;
                        }
                        break;
                    case "2":
                        TeklifSoru anateminat = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, TESabitPrimliSorular.AnaTeminat);
                        switch (anateminat.Cevap)
                        {
                            case "1":
                                TeklifTeminat vefat = _TeklifService.GetTeklifTeminat(teklif.GenelBilgiler.TeklifId, TESabitPrimliTeminatlar.Vefat);
                                if (vefat != null && vefat.TeminatBedeli.HasValue)
                                    brutPrim = vefat.TeminatBedeli.Value.ToString("N2") + " " + paraBirimi;
                                break;
                            case "2":
                                TeklifTeminat vefatKritik = _TeklifService.GetTeklifTeminat(teklif.GenelBilgiler.TeklifId, TESabitPrimliTeminatlar.Vefat_KritikHastalik);
                                if (vefatKritik != null && vefatKritik.TeminatBedeli.HasValue)
                                    brutPrim = vefatKritik.TeminatBedeli.Value.ToString("N2") + " " + paraBirimi;
                                break;
                        }
                        break;
                }
            }

            #endregion

            //Tetkik Parse Ediliyor
            TetkikParse(fiyatModel, _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.TibbiTetkikSonucu));

            fiyatModel.Fiyat1 = brutPrim;
        }

        private void OdulluBirikimParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            string brutPrim = String.Empty;

            TeklifSoru HesaplamaSecenegi = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, OdulluBirikimSorular.HesaplamaSecenegi);
            if (HesaplamaSecenegi != null)
            {
                switch (HesaplamaSecenegi.Cevap)
                {
                    case "1":
                        TeklifTeminat vefat = _TeklifService.GetTeklifTeminat(teklif.GenelBilgiler.TeklifId, OdulluBirikimTeminatlar.Vefat);
                        if (vefat != null && vefat.TeminatBedeli.HasValue)
                            brutPrim = vefat.TeminatBedeli.Value.ToString("N2") + " TL";
                        break;
                    case "2":
                        if (teklif.GenelBilgiler.BrutPrim.HasValue && teklif.GenelBilgiler.NetPrim.HasValue)
                            brutPrim = teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " TL / " + teklif.GenelBilgiler.NetPrim.Value.ToString("N2") + " TL";
                        break;
                }
            }

            //Tetkik Parse Ediliyor
            TetkikParse(fiyatModel, _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.TibbiTetkikSonucu));

            fiyatModel.Fiyat1 = brutPrim;
        }

        private void PrimIadeliParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            string brutPrim = String.Empty;

            TeklifSoru hesaplamaSecenegi = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, PrimIadeliSorular.HesaplamaSecenegi);
            if (hesaplamaSecenegi != null)
            {
                switch (hesaplamaSecenegi.Cevap)
                {
                    case "1":
                        TeklifTeminat vefat = _TeklifService.GetTeklifTeminat(teklif.GenelBilgiler.TeklifId, PrimIadeliTeminatlar.VefatTeminati);
                        if (vefat != null && vefat.TeminatBedeli.HasValue)
                            brutPrim = vefat.TeminatBedeli.Value.ToString("N2") + " $";
                        break;
                    case "2":
                        if (teklif.GenelBilgiler.BrutPrim.HasValue && teklif.GenelBilgiler.NetPrim.HasValue)
                            brutPrim = teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " $ / " + teklif.GenelBilgiler.NetPrim.Value.ToString("N2") + " $";
                        break;
                }
            }

            //Tetkik Parse Ediliyor
            TetkikParse(fiyatModel, _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.TibbiTetkikSonucu));

            fiyatModel.Fiyat1 = brutPrim;
        }

        private void OdemeGuvenceParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            string brutPrim = String.Empty;
            string paraBirimi = String.Empty;

            TeklifSoru KalanSure = _TeklifService.GetTeklifSoru(teklif.GenelBilgiler.TeklifId, OdemeGuvenceSorular.KapYildonumuKalanSure);
            if (KalanSure != null)
            {
                brutPrim = KalanSure.Cevap;
                if (!String.IsNullOrEmpty(brutPrim) && brutPrim != "0")
                    brutPrim = brutPrim + " Ay";
            }

            TeklifSoru ParaBirimi = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, OdemeGuvenceSorular.ParaBirimi);
            if (ParaBirimi != null)
            {
                paraBirimi = OdemeGuvenceParaBirimi.ParaBirimiText(ParaBirimi.Cevap);
            }

            if (teklif.GenelBilgiler.BrutPrim.HasValue)
                brutPrim = brutPrim + " / " + teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " " + paraBirimi;

            //Tetkik Parse Ediliyor
            TetkikParse(fiyatModel, _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.TibbiTetkikSonucu));

            //Ödeme güvence ürünü için uyarı mevcut
            fiyatModel.Hasarsizlik = _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.Uyari);

            fiyatModel.Fiyat1 = brutPrim;
        }

        private void EgitimParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            string prim = String.Empty;

            if (teklif.GenelBilgiler.NetPrim.HasValue)
            {
                prim = teklif.GenelBilgiler.NetPrim.Value.ToString("N2") + " $ ";

                TeklifTeminat vefat = _TeklifService.GetTeklifTeminat(teklif.GenelBilgiler.TeklifId, EgitimSigortasiTeminatlar.VefatTeminati);
                if (vefat != null && vefat.TeminatBedeli.HasValue)
                    prim += " / " + vefat.TeminatBedeli.Value.ToString("N2") + " $";
            }

            //Tetkik Parse Ediliyor
            TetkikParse(fiyatModel, _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.TibbiTetkikSonucu));

            fiyatModel.Fiyat1 = prim;
        }

        private void KorunanGelecekParser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            string brutPrim = String.Empty;
            string paraBirimi = String.Empty;

            TeklifSoru ParaBirimi = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, TESabitPrimliSorular.ParaBirimi);
            if (ParaBirimi != null)
            {
                switch (ParaBirimi.Cevap)
                {
                    case "1": paraBirimi = "€"; break;
                    case "2": paraBirimi = "$"; break;
                }
            }

            if (teklif.GenelBilgiler.BrutPrim.HasValue)
            {
                brutPrim = teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " " + paraBirimi + " / ";
                brutPrim += teklif.GenelBilgiler.NetPrim.Value.ToString("N2") + " " + paraBirimi;
            }
            //Tetkik Parse Ediliyor
            TetkikParse(fiyatModel, _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.TibbiTetkikSonucu));

            fiyatModel.Fiyat1 = brutPrim;
        }

        private void PrimIadeli2Parser(ITeklif teklif, TeklifFiyatDetayModel fiyatModel)
        {
            string brutPrim = String.Empty;

            TeklifSoru hesaplamaSecenegi = _TeklifService.GetAnaTeklifSoru(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu, PrimIadeli2Sorular.HesaplamaSecenegi);
            if (hesaplamaSecenegi != null)
            {
                switch (hesaplamaSecenegi.Cevap)
                {
                    case "1":
                        TeklifTeminat vefat = _TeklifService.GetTeklifTeminat(teklif.GenelBilgiler.TeklifId, PrimIadeli2Teminatlar.VefatTeminati);
                        if (vefat != null && vefat.TeminatBedeli.HasValue)
                            brutPrim = vefat.TeminatBedeli.Value.ToString("N2") + " $";
                        break;
                    case "2":
                        if (teklif.GenelBilgiler.BrutPrim.HasValue && teklif.GenelBilgiler.NetPrim.HasValue)
                            brutPrim = teklif.GenelBilgiler.BrutPrim.Value.ToString("N2") + " $ / " + teklif.GenelBilgiler.NetPrim.Value.ToString("N2") + " $";
                        break;

                }
            }

            //Tetkik Parse Ediliyor
            TetkikParse(fiyatModel, _TeklifService.GetTeklifWebServisCevap(teklif.GenelBilgiler.TeklifId, WebServisCevaplar.TibbiTetkikSonucu));

            fiyatModel.Fiyat1 = brutPrim;
        }

        private void TetkikParse(TeklifFiyatDetayModel fiyatModel, string tetkik)
        {
            if (!String.IsNullOrEmpty(tetkik))
            {
                int index = tetkik.IndexOf(".");
                if (index != -1 && tetkik.Length > (index + 3))
                {
                    string ilk = tetkik.Substring(0, index + 1);
                    string son = tetkik.Substring(index + 3);

                    if (!String.IsNullOrEmpty(ilk))
                    {
                        TeklifSurprimModel surprim = new TeklifSurprimModel();
                        surprim.Surprim = ilk;
                        fiyatModel.Surprimler.Add(surprim);
                    }

                    if (!String.IsNullOrEmpty(son))
                    {
                        TeklifSurprimModel surprim = new TeklifSurprimModel();
                        surprim.Surprim = son;
                        fiyatModel.Surprimler.Add(surprim);
                    }
                }
                else
                {
                    TeklifSurprimModel surprim = new TeklifSurprimModel();
                    surprim.Surprim = tetkik;
                    fiyatModel.Surprimler.Add(surprim);
                }
            }
        }

        [HttpGet]
        [AjaxException]
        public PartialViewResult AegonOnProvizyon(int teklifId)
        {
            AegonOnProvizyonModel Model = new AegonOnProvizyonModel();

            TeklifGenel teklif = _TeklifService.GetTeklifGenel(teklifId);

            if (teklif != null)
            {
                Model.teklifid = teklif.TeklifId;

                TVMKullanicilar kullanici = _KullaniciService.GetKullanici(_AktifKullaniciService.KullaniciKodu);

                if (kullanici != null)
                {
                    Model.pPartajNox = Convert.ToInt32(kullanici.TeknikPersonelKodu);
                }

                Model.pProvTarx = TurkeyDateTime.Today;

                Model.pKKNox = new KrediKartiModel();
                Model.Aylar = TeklifProvider.KrediKartiAylar();
                Model.pSKT_Ayx = TurkeyDateTime.Today.Month < 10 ? "0" + TurkeyDateTime.Today.Month.ToString() : TurkeyDateTime.Today.Month.ToString();
                Model.Yillar = TeklifProvider.KrediKartiYillar();
                Model.pSKT_Yilx = TurkeyDateTime.Today.Year.ToString();

                Model.pParaBirimix = _TeklifService.AegonOnProvizyonParaBirimi(teklif);

                int gercekTeklifId = 0;
                Model.pTutarx = _TeklifService.AegonOnProvizyonTutar(teklif, out gercekTeklifId);

                if (Model.pTutarx > 0)
                    return PartialView("_OnProvizyonPartial", Model);
            }

            return null;
        }

        [HttpPost]
        public JsonResult AegonOnProvizyon(AegonOnProvizyonModel Model)
        {
            bool result = false;
            string message = String.Empty;
            string html = String.Empty;

            try
            {
                #region Kredi Kartı

                string KrediKartiNo = String.Empty;

                if (Request.Params["pKKNox.KK1"] != null)
                    KrediKartiNo += Request.Params["pKKNox.KK1"].ToString();

                if (Request.Params["pKKNox.KK2"] != null)
                    KrediKartiNo += Request.Params["pKKNox.KK2"].ToString();

                if (Request.Params["pKKNox.KK3"] != null)
                    KrediKartiNo += Request.Params["pKKNox.KK3"].ToString();

                if (Request.Params["pKKNox.KK4"] != null)
                    KrediKartiNo += Request.Params["pKKNox.KK4"].ToString();

                if (KrediKartiNo.Length == 16)
                    ModelState["pKKNox"].Errors.Clear();

                #endregion

                if (ModelState.IsValid)
                {
                    TeklifGenel teklif = _TeklifService.GetTeklifGenel(Model.teklifid);

                    if (teklif != null &&
                        teklif.TeklifDurumKodu == TeklifDurumlari.Teklif &&
                        Model.pProvTarx < TurkeyDateTime.Today.AddDays(14) &&
                        Model.pProvTarx > TurkeyDateTime.Today.AddDays(-1))
                    {
                        #region SET REQUEST

                        AegonOnProvizyonRequest request = new AegonOnProvizyonRequest();

                        request.pTeklifNox = teklif.TeklifNo;
                        request.pPartajNox = Model.pPartajNox.ToString();
                        request.pBasvuruNox = Model.pBasvuruNox;
                        request.pOdemeTurux = 3;    //--hep ‘3’ olmalıdır. 
                        request.pTCKx = Model.pTCKx;
                        request.pKKNox = KrediKartiNo;
                        request.pSKT_Ayx = Model.pSKT_Ayx;
                        request.pSKT_Yilx = Model.pSKT_Yilx;
                        request.pCVVx = Model.pCVVx;
                        request.pPartajNox = Model.pPartajNox.ToString();
                        request.pProvTarx = Model.pProvTarx.ToString("dd.MM.yyyy");

                        #region UrunKodu Set

                        string urunKodu = String.Empty;

                        switch (teklif.UrunKodu)
                        {
                            case UrunKodlari.TESabitPrimli: urunKodu = "TE02"; break;
                            case UrunKodlari.KorunanGelecek: urunKodu = "KG01"; break;
                            case UrunKodlari.OdemeGuvence: urunKodu = "OGS01"; break;
                            case UrunKodlari.KritikHastalik: urunKodu = ""; break;
                            case UrunKodlari.OdulluBirikim: urunKodu = "OB01"; break;
                            case UrunKodlari.PrimIadeli: urunKodu = "PI01"; break;
                            case UrunKodlari.Egitim: urunKodu = "ES01"; break;
                            case UrunKodlari.PrimIadeli2: urunKodu = "PI02"; break;
                        }

                        request.pUrunHaymerKodux = urunKodu;

                        #endregion

                        #region DovizKodu - ParaBirimi

                        request.pDovKodx = _TeklifService.AegonOnProvizyonParaBirimi(teklif);
                        request.pParaBirimix = request.pDovKodx;

                        #endregion

                        result = _TeklifService.AegonOnProvizyon(request, teklif, out message);

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                _LogService.Error(ex);
            }

            if (!result)
            {
                #region  ModelDolduruluyor.

                Model.Aylar = TeklifProvider.KrediKartiAylar();
                Model.pSKT_Ayx = Model.pSKT_Ayx;
                Model.Yillar = TeklifProvider.KrediKartiYillar();
                Model.pSKT_Yilx = Model.pSKT_Yilx;

                TeklifGenel teklif1 = _TeklifService.GetTeklifGenel(Model.teklifid);

                if (teklif1 != null)
                {
                    Model.pParaBirimix = _TeklifService.AegonOnProvizyonParaBirimi(teklif1);

                    int gercekTeklifId = 0;
                    Model.pTutarx = _TeklifService.AegonOnProvizyonTutar(teklif1, out gercekTeklifId);
                }

                ModelState.AddModelError("", message);
                html = RenderViewToString(this.ControllerContext, "_OnProvizyonPartial", Model);

                #endregion
            }

            return Json(new { Success = result, Message = message, html = html });
        }

        #endregion

        #region Muhasebe

        protected void SendMuhasebe(ITeklif teklif)
        {
            if (teklif.GenelBilgiler.TeklifDurumKodu == TeklifDurumlari.Police)
            {
                TVMDetay tvm = teklif.GenelBilgiler.TVMDetay;

                //Muhasebe kullanıyorsa poliçe bilgisi muhasebeye gönderiliyor.
                if (tvm != null && tvm.MuhasebeEntegrasyon.HasValue && tvm.MuhasebeEntegrasyon.Value)
                {
                    _PoliceToXML = DependencyResolver.Current.GetService<IPoliceToXML>();
                    _PoliceToXML.SendPoliceToMuhasebe(teklif);
                }
            }
        }

        #endregion

        public SigortaliModel TSSMusteriKaydet(TamamlayiciSaglikModel model)
        {
            SigortaliModel sigortaliModel = model.Musteri;
            sigortaliModel.EMailRequired = true;

            #region Sigorta ettiren kaydet
            if (!sigortaliModel.SigortaEttiren.MusteriKodu.HasValue ||
                sigortaliModel.SigortaEttiren.MusteriKodu == 0)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, model.Musteri.TVMKodu))
                {
                    return sigortaliModel;
                }
            }

            if (!sigortaliModel.SigortaliAyni)
            {
                if (!this.MusteriKaydet(sigortaliModel.Sigortali, model.Musteri.TVMKodu))
                {
                    return sigortaliModel;
                }
            }

            if (sigortaliModel.SigortaEttiren.TVMKodu != _AktifKullaniciService.TVMKodu)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, _AktifKullaniciService.TVMKodu))
                {
                    return sigortaliModel;
                }
            }
            #endregion

            return sigortaliModel;
        }
        public SigortaliModel LilyumMusteriKaydet(LilyumFerdiKazaModel model)
        {
            SigortaliModel sigortaliModel = model.Musteri;
            sigortaliModel.EMailRequired = true;

            #region Sigorta ettiren kaydet
            if (!sigortaliModel.SigortaEttiren.MusteriKodu.HasValue ||
                sigortaliModel.SigortaEttiren.MusteriKodu == 0)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, model.Musteri.TVMKodu))
                {
                    return sigortaliModel;
                }
            }

            if (!sigortaliModel.SigortaliAyni)
            {
                if (!this.MusteriKaydet(sigortaliModel.Sigortali, model.Musteri.TVMKodu))
                {
                    return sigortaliModel;
                }
            }

            if (sigortaliModel.SigortaEttiren.TVMKodu != _AktifKullaniciService.TVMKodu)
            {
                if (!this.MusteriKaydet(sigortaliModel.SigortaEttiren, _AktifKullaniciService.TVMKodu))
                {
                    return sigortaliModel;
                }
            }
            #endregion

            return sigortaliModel;
        }

        protected string RenderViewToString(ControllerContext context,
                        string viewName, object viewData)
        {
            //Create memory writer 
            var sb = new StringBuilder();
            var memWriter = new System.IO.StringWriter(sb);

            //Create fake http context to render the view 
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request, fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                context.RouteData, context.Controller);

            var oldContext = System.Web.HttpContext.Current;
            System.Web.HttpContext.Current = fakeContext;

            //Use HtmlHelper to render partial view to fake context 
            var html = new HtmlHelper(new ViewContext(fakeControllerContext,
                new FakeView(), new ViewDataDictionary(), new TempDataDictionary(), memWriter),
                new ViewPage());
            html.RenderPartial(viewName, viewData);

            //Restore context 
            System.Web.HttpContext.Current = oldContext;

            //Flush memory and return output 
            memWriter.Flush();
            return sb.ToString();
        }


    }
}

/// <summary>
/// Fake IView implementation, only used to instantiate an HtmlHelper.
/// </summary> 
public class FakeView : IView
{
    #region IView Members
    public void Render(ViewContext viewContext, System.IO.TextWriter writer)
    {
        throw new NotImplementedException();
    }
    #endregion
}