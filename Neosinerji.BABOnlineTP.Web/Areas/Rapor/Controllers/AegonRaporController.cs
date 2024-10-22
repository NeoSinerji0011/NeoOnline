using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using Neosinerji.BABOnlineTP.Web.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;
using ClosedXML.Excel;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = 0, SekmeKodu = 0)]
    public class AegonRaporController : Controller
    {
        IAktifKullaniciService _AktifKullaniciService;
        ILogService _log;
        ITVMService _TVMService;
        IRaporService _RaporService;
        ITeklifService _TeklifService;

        public AegonRaporController()
        {
            _AktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _log = DependencyResolver.Current.GetService<ILogService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _RaporService = DependencyResolver.Current.GetService<IRaporService>();
            _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = 0)]
        public ActionResult TeklifRaporu()
        {
            AegonTeklifRaporuModel model = new AegonTeklifRaporuModel();
            model.RaporSonuc = new List<TeklifRaporuProcedureModel>();

            try
            {
                model.BaslangicTarihi = TurkeyDateTime.Now.AddDays(-2);
                model.BitisTarihi = TurkeyDateTime.Now;
                model.TeklifTarihi = 2;


                model.TeklifTarihiTipleri = new SelectList(AegonRaporListProvider.TarihTipleri(), "Value", "Text", "2").ToList();
                model.ParaBirimiList = new SelectList(AegonRaporListProvider.ParaBirimleri(), "Value", "Text", "0").ToList();
                model.UrunlerItems = new MultiSelectList(_AktifKullaniciService.UrunYetkileri, "UrunKodu", "Aciklama");
                model.BolgeList = new SelectList(_TVMService.GetListBolgeler(NeosinerjiTVM.AegonTVMKodu), "TVMBolgeKodu", "BolgeAdi", "").ToList();
                model.BolgeList.Insert(0, new SelectListItem() { Text = "Tümü", Value = "", Selected = true });


                List<TVMDetay> tvmlist = _TVMService.GetListTVMDetayYetkili();
                model.TVMLerItems = new SelectList(tvmlist, "Kodu", "Unvani", "").ToList();

                model.TVMLerItems.Insert(0, new SelectListItem() { Text = "Tümü", Value = "", Selected = true });

                ViewBag.TVMKodu = _AktifKullaniciService.TVMKodu;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View(model);
        }

        [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = AltMenuler.OnlineRaporlar, SekmeKodu = 0)]
        public ActionResult ListePagerTeklifRaporu()
        {
            try
            {
                if (Request["sEcho"] != null)
                {
                    CultureInfo turkey = new CultureInfo("tr-TR");

                    AegonTeklifRaporuListe arama = new AegonTeklifRaporuListe(Request, new Expression<Func<AegonTeklifRaporuProcedureModel, object>>[]
                                                                    {
                                                                        t => t.TeklifNo,
                                                                        t => t.IlgiliTeklifNo,
                                                                        t => t.MusteriAdiSoyadi,
                                                                        t => t.UrunAdi,
                                                                        t => t.TeklifTarihi,
                                                                        t => t.BitisTarihi,
                                                                        t => t.BaslangicTarihi,
                                                                        t => t.RiskDegerlendirmeSonucu,
                                                                        t => t.TVMUnvani,
                                                                        t => t.EkleyenKullanici,
                                                                        t => t.SigortaSuresi,
                                                                        t => t.PrimOdemeDonemi,
                                                                        t => t.DonemselPrim,
                                                                        t => t.YillikPrim,
                                                                        t => t.ToplamPrim,
                                                                        t => t.ParaBirimi,
                                                                        t => t.OnProvizyon,
                                                                    });


                    //SOL
                    arama.TeklifTarihi = arama.TryParseParamInt("TeklifTarihi") ?? 0;
                    arama.Subeler = arama.TryParseParamString("TVMLerSelectList");
                    arama.Urunler = arama.TryParseParamString("UrunSelectList");
                    arama.YillikMin = arama.TryParseParamInt("YillikPrimMin") ?? 0;
                    arama.YillikMax = arama.TryParseParamInt("YillikPrimMax") ?? 0;
                    arama.BolgeKodu = arama.TryParseParamString("BolgeKodu");

                    //SAĞ
                    arama.BaslangicTarihi = arama.TryParseParamDate("BaslangicTarihi");
                    arama.BitisTarihi = arama.TryParseParamDate("BitisTarihi");
                    arama.ParaBirimi = arama.TryParseParamInt("ParaBirimi") ?? 0;
                    arama.TeklifNo = arama.TryParseParamInt("TeklifNo");


                    int totalRowCount = 0;
                    List<AegonTeklifRaporuProcedureModel> list = _RaporService.AegonTeklifRaporuPagedList(arama, out totalRowCount);


                    arama.AddFormatter(f => f.TeklifNo, f => String.Format("<a href='{0}{1}'>{2}</a> {3}", TeklifSayfaAdresleri.DetayAdres(f.UrunKodu),
                                                         f.TeklifId, f.TeklifNo, f.PDFDosyasi == null ? "" :
                                                         "<a href=" + f.PDFDosyasi + " title='Teklif PDF' target='_blank' class='pull-right'>" +
                                                         "<img src='/content/img/pdf_icon.png' /></a>"));

                    arama.AddFormatter(f => f.IlgiliTeklifNo, f => f.IlgiliTeklifNo.HasValue ?
                                                                 String.Format("<a href='{0}{1}'>{2}</a>", TeklifSayfaAdresleri.DetayAdres(f.IlgiliTeklifUrunKodu ?? 0),
                                                                 f.IlgiliTeklifId, f.IlgiliTeklifNo) : "");


                    arama.AddFormatter(f => f.TeklifTarihi, f => String.Format("{0}", f.TeklifTarihi.HasValue ? f.TeklifTarihi.Value.ToString("dd.MM.yyyy") : ""));
                    arama.AddFormatter(f => f.BaslangicTarihi, f => String.Format("{0}", f.BaslangicTarihi.HasValue ? f.BaslangicTarihi.Value.ToString("dd.MM.yyyy") : ""));
                    arama.AddFormatter(f => f.BitisTarihi, f => String.Format("{0}", f.BitisTarihi.HasValue ? f.BitisTarihi.Value.ToString("dd.MM.yyyy") : ""));

                    arama.AddFormatter(f => f.DonemselPrim, f => String.Format("{0}", f.DonemselPrim.HasValue ? f.DonemselPrim.Value.ToString("N2", turkey) : ""));
                    arama.AddFormatter(f => f.YillikPrim, f => String.Format("{0}", f.YillikPrim.HasValue ? f.YillikPrim.Value.ToString("N2", turkey) : ""));
                    arama.AddFormatter(f => f.ToplamPrim, f => String.Format("{0}", f.ToplamPrim.HasValue ? f.ToplamPrim.Value.ToString("N2", turkey) : ""));

                    arama.AddFormatter(f => f.OnProvizyon, f => String.Format("{0}", f.OnProvizyon ?
                                                                "<a href='javascript:;' class='on-provizyon-detay' teklif-id='" + f.TeklifId + "'> " +
                                                                "<div class='label label-success'><i class='icon-check'></i></div></a>" :
                                                                String.Format("<a href='{0}{1}'><span class='label label-info'>Ön Provizyon Al</span></a>",
                                                                TeklifSayfaAdresleri.DetayAdres(f.UrunKodu), f.TeklifId)));


                    DataTableList result = arama.Prepare(list, totalRowCount);

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetTVMByBolgeKodu(string BolgeKodu)
        {
            List<SelectListItem> selectList = new List<SelectListItem>();

            try
            {
                List<TVMDetay> list = new List<TVMDetay>();

                list = _TVMService.GetListTvmByBolgeKodu(BolgeKodu);

                selectList = new SelectList(list, "Kodu", "Unvani", "").ToList();

                selectList.Insert(0, new SelectListItem() { Text = "Tümü", Value = "", Selected = true });
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return Json(selectList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public PartialViewResult GetOnProvizyonDetay(int teklifId)
        {
            AegonOnProvizyonModelDetay Model = new AegonOnProvizyonModelDetay();

            try
            {
                if (teklifId > 0)
                {
                    Model = _TeklifService.AegonOnProvizyonDetay(teklifId);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return PartialView("_OnProvizyonDetay", Model);
        }

        public ExcelResult GetAegonRaporAll()
        {
            try
            {
                #region Prop Set

                CultureInfo turkey = new CultureInfo("tr-TR");

                AegonTeklifRaporuExcelAktar arama = new AegonTeklifRaporuExcelAktar();


                //SOL
                if (Request["TeklifTarihi"] != null && !String.IsNullOrEmpty(Request["TeklifTarihi"]))
                    arama.TeklifTarihi = Convert.ToInt32(Request["TeklifTarihi"]);

                if (Request["TVMLerSelectList"] != null && !String.IsNullOrEmpty(Request["TVMLerSelectList"]))
                    arama.Subeler = Request["TVMLerSelectList"];

                if (Request["UrunSelectList"] != null && !String.IsNullOrEmpty(Request["UrunSelectList"].ToString()) && Request["UrunSelectList"].ToString() != "null")
                    arama.Urunler = Request["UrunSelectList"];

                if (Request["YillikPrimMin"] != null && !String.IsNullOrEmpty(Request["YillikPrimMin"]))
                    arama.YillikMin = Convert.ToInt32(Request["YillikPrimMin"]);

                if (Request["YillikPrimMax"] != null && !String.IsNullOrEmpty(Request["YillikPrimMax"]))
                    arama.YillikMax = Convert.ToInt32(Request["YillikPrimMax"]);

                if (Request["BolgeKodu"] != null && !String.IsNullOrEmpty(Request["BolgeKodu"]))
                    arama.BolgeKodu = Request["BolgeKodu"];


                //SAĞ
                if (Request["BaslangicTarihi"] != null && !String.IsNullOrEmpty(Request["BaslangicTarihi"]))
                    arama.BaslangicTarihi = Convert.ToDateTime(Request["BaslangicTarihi"]);

                if (Request["BitisTarihi"] != null && !String.IsNullOrEmpty(Request["BitisTarihi"]))
                    arama.BitisTarihi = Convert.ToDateTime(Request["BitisTarihi"]);

                if (Request["ParaBirimi"] != null && !String.IsNullOrEmpty(Request["ParaBirimi"]))
                    arama.ParaBirimi = Convert.ToInt32(Request["ParaBirimi"]);

                if (Request["TeklifNo"] != null && !String.IsNullOrEmpty(Request["TeklifNo"]))
                    arama.TeklifNo = Convert.ToInt32(Request["TeklifNo"]);

                List<AegonTeklifRaporuProcedureModel> list = _RaporService.AegonTeklifRaporuExcelAktar(arama);

                string fileName = String.Format("aegon-teklif-raporu-{0:yyyy-MM-dd-HH:mm:ss}", TurkeyDateTime.Now);

                var workbook = new XLWorkbook(XLEventTracking.Disabled);
                var ws = workbook.Worksheets.Add("Teklifler");
                ws.Range("A1:Q1").Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Range("A1:Q1").Style.Font.SetBold(true);

                #endregion

                #region Column Set

                ws.Cell(1, 1).Value = "Teklif No";
                ws.Cell(1, 2).Value = "Müşteri";
                ws.Cell(1, 3).Value = "Ürün";
                ws.Cell(1, 4).Value = "Teklif Tarihi";
                ws.Cell(1, 5).Value = "Başlangıç Tarihi";
                ws.Cell(1, 6).Value = "Bitiş Tarihi";
                ws.Cell(1, 7).Value = "TVM Unvani";
                ws.Cell(1, 8).Value = "Ekleyen Kullanıcı";
                ws.Cell(1, 9).Value = "Para Birimi";
                ws.Cell(1, 10).Value = "Prim Ödeme Dönemi";
                ws.Cell(1, 11).Value = "Sigorta Süresi (Yıl)";
                ws.Cell(1, 12).Value = "Dönemsel Prim";
                ws.Cell(1, 13).Value = "Yıllık Prim";
                ws.Cell(1, 14).Value = "Toplam Prim";
                ws.Cell(1, 15).Value = "Risk Değerlendirme Sonucu";
                ws.Cell(1, 16).Value = "Ön Provizyon";
                ws.Cell(1, 17).Value = "İlgili Teklif No";

                ws.Column(1).Width = 10;
                ws.Column(2).Width = 20;
                ws.Column(3).Width = 15;
                ws.Column(4).Width = 15;
                ws.Column(5).Width = 15;
                ws.Column(6).Width = 15;
                ws.Column(7).Width = 40;
                ws.Column(8).Width = 20;
                ws.Column(9).Width = 10;
                ws.Column(10).Width = 10;
                ws.Column(11).Width = 15;
                ws.Column(12).Width = 15;
                ws.Column(13).Width = 15;
                ws.Column(14).Width = 15;
                ws.Column(15).Width = 40;
                ws.Column(16).Width = 15;
                ws.Column(17).Width = 15;

                #endregion

                #region Value Set

                int row = 2;
                foreach (var item in list)
                {
                    ws.Cell(row, 1).Value = item.TeklifNo;
                    ws.Cell(row, 2).Value = item.MusteriAdiSoyadi;
                    ws.Cell(row, 3).Value = item.UrunAdi;

                    ws.Cell(row, 4).Style.DateFormat.Format = "dd.MM.yyyy";
                    ws.Cell(row, 4).Value = item.TeklifTarihi;

                    ws.Cell(row, 5).Style.DateFormat.Format = "dd.MM.yyyy";
                    ws.Cell(row, 5).Value = item.BaslangicTarihi;

                    ws.Cell(row, 6).Style.DateFormat.Format = "dd.MM.yyyy";
                    ws.Cell(row, 6).Value = item.BitisTarihi;

                    ws.Cell(row, 7).Value = item.TVMUnvani;
                    ws.Cell(row, 8).Value = item.EkleyenKullanici;
                    ws.Cell(row, 9).Value = item.ParaBirimi;
                    ws.Cell(row, 10).Value = item.PrimOdemeDonemi;
                    ws.Cell(row, 11).Value = item.SigortaSuresi;
                    ws.Cell(row, 12).Value = String.Format("{0}", item.DonemselPrim.HasValue ? item.DonemselPrim.Value.ToString("N2", turkey) : "");
                    ws.Cell(row, 13).Value = String.Format("{0}", item.YillikPrim.HasValue ? item.YillikPrim.Value.ToString("N2", turkey) : "");
                    ws.Cell(row, 14).Value = String.Format("{0}", item.ToplamPrim.HasValue ? item.ToplamPrim.Value.ToString("N2", turkey) : "");
                    ws.Cell(row, 15).Value = item.RiskDegerlendirmeSonucu;

                    if (item.OnProvizyon)
                    {
                        ws.Cell(row, 16).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        ws.Cell(row, 16).Value = "Yapıldı";
                    }
                    else
                    {
                        ws.Cell(row, 16).Value = "Yapılmadı";
                    }

                    ws.Cell(row, 17).Value = item.IlgiliTeklifNo;

                    row++;
                }

                #endregion

                return new ExcelResult(workbook, fileName);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }
    }
}
