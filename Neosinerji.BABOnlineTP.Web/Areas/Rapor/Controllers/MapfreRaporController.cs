using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Web.Areas.Rapor.Models;
using Neosinerji.BABOnlineTP.Web.Tools;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Web.Tools.Helpers;
using ClosedXML.Excel;

namespace Neosinerji.BABOnlineTP.Web.Areas.Rapor.Controllers
{
    [Authorization(AnaMenuKodu = AnaMenuler.Rapor, AltMenuKodu = 0, SekmeKodu = 0)]
    public class MapfreRaporController : Controller
    {
        IRaporService _RaporService;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullanici;

        public MapfreRaporController(IRaporService raporService, ITVMService tvmService, IAktifKullaniciService aktifKullanici)
        {
            _RaporService = raporService;
            _TVMService = tvmService;
            _AktifKullanici = aktifKullanici;
        }

        //
        // GET: /Rapor/Mapfre/BolgeUretim
        public ActionResult BolgeUretim()
        {
            //Merkez tvm ve bölge müdürlükleri hariç bu sayfaya giremez
            if (_AktifKullanici.TVMKodu != 107 && !_AktifKullanici.MapfreBolge)
            {
                return RedirectToAction("ErrorPage", "Error", new { id = 403 });
            }

            MapfreBolgeUretimRaporModel model = new MapfreBolgeUretimRaporModel();

            model.BaslangicTarih = DateTime.Today;
            model.BitisTarih = DateTime.Today;
            model.BolgeKodu = null;
            model.Acenteler = false;

            model.Bolgeler = new SelectList(_TVMService.GetListBolgeler(107), "TVMBolgeKodu", "BolgeAdi");

            //Bölge seçeneği gösterilmeyecek
            model.BolgeSecebilir = _AktifKullanici.TVMKodu == 107;
            if (!model.BolgeSecebilir)
                model.Acenteler = true;

            return View(model);
        }

        //
        // POST: /Rapor/Mapfre/BolgeUretim
        [HttpPost]
        public ActionResult BolgeUretim(MapfreBolgeUretimRaporModel model)
        {
            //Merkez tvm ve bölge müdürlükleri hariç bu sayfaya giremez
            if (_AktifKullanici.TVMKodu != 107 && !_AktifKullanici.MapfreBolge)
            {
                return RedirectToAction("ErrorPage", "Error", new { id = 403 });
            }

            model.Bolgeler = new SelectList(_TVMService.GetListBolgeler(107), "TVMBolgeKodu", "BolgeAdi");

            //Bolgeler müdürlükleri bölge seçemiyor default kendi bölgeleri listeleniyor.
            if (_AktifKullanici.MapfreBolge)
            {
                TVMDetay tvm = _TVMService.GetDetay(_AktifKullanici.TVMKodu);
                model.BolgeKodu = tvm.BolgeKodu;
            }

            try
            {
                model.Rapor = _RaporService.MapfreBolgeUretimRapor(model.BaslangicTarih, model.BitisTarih, model.BolgeKodu, model.Acenteler);

                this.BolgeUretimRaporHazirla(model);
            }
            catch (Exception ex)
            {
                model.ErrorReport = true;
                model.ExceptionMessage = ex.Message;
            }

            return View(model);
        }

        private void BolgeUretimRaporHazirla(MapfreBolgeUretimRaporModel model)
        {
            if (model.Acenteler)
            {
                model.RaporBolgeler = (from row in model.Rapor
                                       group row by new { row.BolgeKodu, row.BolgeAdi } into g
                                       select new MapfreBolgeUretimModel()
                                       {
                                           BolgeKodu = g.Key.BolgeKodu,
                                           BolgeAdi = g.Key.BolgeAdi,
                                           TVMKodu = 0,
                                           TVMUnvani = String.Empty,
                                           KaskoTeklif = g.Sum(s => s.KaskoTeklif),
                                           KaskoPolice = g.Sum(s => s.KaskoPolice),
                                           KaskoBrutPrim = g.Sum(s => s.KaskoBrutPrim),
                                           KaskoKomisyon = g.Sum(s => s.KaskoKomisyon),
                                           KaskoPoliceOran = 0,
                                           TrafikTeklif = g.Sum(s => s.TrafikTeklif),
                                           TrafikPolice = g.Sum(s => s.TrafikPolice),
                                           TrafikBrutPrim = g.Sum(s => s.TrafikBrutPrim),
                                           TrafikKomisyon = g.Sum(s => s.TrafikKomisyon),
                                           TrafikPoliceOran = 0
                                       }).ToList<MapfreBolgeUretimModel>();

                foreach (var bolge in model.RaporBolgeler)
                {
                    bolge.KaskoPoliceOran = bolge.KaskoTeklif > 0 ? (decimal)bolge.KaskoPolice / bolge.KaskoTeklif * 100 : 0;
                    bolge.TrafikPoliceOran = bolge.TrafikTeklif > 0 ? (decimal)bolge.TrafikPolice / bolge.TrafikTeklif * 100 : 0;
                }
            }
            else
            {
                model.RaporBolgeler = model.Rapor;
            }

            model.BolgeSecebilir = _AktifKullanici.TVMKodu == 107;

            if (model.RaporBolgeler != null && model.RaporBolgeler.Count > 0)
            {
                MapfreBolgeUretimModel toplam = new MapfreBolgeUretimModel();
                toplam.BolgeKodu = -1;
                toplam.BolgeAdi = String.Empty;
                toplam.TVMKodu = 0;
                toplam.TVMUnvani = String.Empty;
                toplam.KaskoTeklif = model.RaporBolgeler.Sum(s => s.KaskoTeklif);
                toplam.KaskoPolice = model.RaporBolgeler.Sum(s => s.KaskoPolice);
                toplam.KaskoBrutPrim = model.RaporBolgeler.Sum(s => s.KaskoBrutPrim);
                toplam.KaskoKomisyon = model.RaporBolgeler.Sum(s => s.KaskoKomisyon);
                toplam.KaskoPoliceOran = toplam.KaskoTeklif > 0 ? (decimal)toplam.KaskoPolice / toplam.KaskoTeklif * 100 : 0; ;
                toplam.TrafikTeklif = model.RaporBolgeler.Sum(s => s.TrafikTeklif);
                toplam.TrafikPolice = model.RaporBolgeler.Sum(s => s.TrafikPolice);
                toplam.TrafikBrutPrim = model.RaporBolgeler.Sum(s => s.TrafikBrutPrim);
                toplam.TrafikKomisyon = model.RaporBolgeler.Sum(s => s.TrafikKomisyon);
                toplam.TrafikPoliceOran = toplam.TrafikTeklif > 0 ? (decimal)toplam.TrafikPolice / toplam.TrafikTeklif * 100 : 0; ;
                model.RaporBolgeler.Add(toplam);
            }
        }

        public ExcelResult BolgeUretimExcel(MapfreBolgeUretimRaporModel model)
        {
            try
            {
                //Merkez tvm ve bölge müdürlükleri hariç bu sayfaya giremez
                if (_AktifKullanici.TVMKodu != 107 && !_AktifKullanici.MapfreBolge)
                {
                    return new ExcelResult();
                }

                model.Rapor = _RaporService.MapfreBolgeUretimRapor(model.BaslangicTarih, model.BitisTarih, model.BolgeKodu, model.Acenteler);

                this.BolgeUretimRaporHazirla(model);

                #region Prop Set

                CultureInfo turkey = new CultureInfo("tr-TR");

                string fileName = String.Format("bolge-uretim-{0:yyyy-MM-dd-HH:mm:ss}", TurkeyDateTime.Now);

                var workbook = new XLWorkbook(XLEventTracking.Disabled);
                var ws = workbook.Worksheets.Add("BolgeUretim");
                ws.Range("A1:L2").Style.Fill.BackgroundColor = XLColor.LightGray;
                ws.Range("A1:L2").Style.Font.SetBold(true);
                ws.Range("A1:L2").Style.Border.InsideBorderColor = XLColor.Black;
                ws.Range("A1:L2").Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                ws.Range("A1:L2").Style.Border.OutsideBorderColor = XLColor.Black;
                ws.Range("A1:L2").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                #endregion

                #region Column Set

                ws.Range("A1:B1").Merge();
                ws.Range("C1:G1").Merge().Value = "TRAFİK";
                ws.Range("C1:G1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range("H1:L1").Merge().Value = "KASKO";
                ws.Range("H1:L1").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                ws.Cell(2, 1).Value = "Kod";
                ws.Cell(2, 2).Value = "Bölge / Acente";
                ws.Cell(2, 3).Value = "Teklif Sayısı";
                ws.Cell(2, 4).Value = "Poliçe Sayısı";
                ws.Cell(2, 5).Value = "Brüt Prim";
                ws.Cell(2, 6).Value = "Komisyon";
                ws.Cell(2, 7).Value = "Poliçeleşme Oranı";
                ws.Cell(2, 8).Value = "Teklif Sayısı";
                ws.Cell(2, 9).Value = "Poliçe Sayısı";
                ws.Cell(2, 10).Value = "Brüt Prim";
                ws.Cell(2, 11).Value = "Komisyon";
                ws.Cell(2, 12).Value = "Poliçeleşme Oranı";
                ws.Range(ws.Cell(2, 3), ws.Cell(2, 12)).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

                ws.Column(1).Width = 12;
                ws.Column(2).Width = 40;
                ws.Column(3).Width = 12;
                ws.Column(4).Width = 12;
                ws.Column(5).Width = 12;
                ws.Column(6).Width = 12;
                ws.Column(7).Width = 15;
                ws.Column(8).Width = 12;
                ws.Column(9).Width = 12;
                ws.Column(10).Width = 12;
                ws.Column(11).Width = 12;
                ws.Column(12).Width = 15;
                #endregion

                #region Value Set

                int row = 3;
                foreach (MapfreBolgeUretimModel item in model.RaporBolgeler)
                {
                    if (item.BolgeKodu > -1)
                    {
                        ws.Cell(row, 1).Value = item.BolgeKodu;
                        ws.Cell(row, 2).Value = item.BolgeAdi;

                        BolgeUretimExcelCell(ws, model, item, row);
                        row++;

                        if (model.Acenteler)
                        {
                            foreach (MapfreBolgeUretimModel acente in model.Rapor.Where(w => w.BolgeKodu == item.BolgeKodu))
                            {
                                ws.Cell(row, 1).Value = acente.TVMKodu;
                                ws.Cell(row, 2).Value = acente.TVMUnvani;

                                BolgeUretimExcelCell(ws, model, acente, row);
                                row++;
                            }
                        }
                    }
                    else
                    {
                        IXLRange range = ws.Range(ws.Cell(row, 1), ws.Cell(row, 2)).Merge();
                        range.Value = "TOPLAM";
                        range.Style.Font.SetBold(true);
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorderColor = XLColor.Black;
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.OutsideBorderColor = XLColor.Black;
                        range.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                        BolgeUretimExcelCell(ws, model, item, row);

                        range = ws.Range(ws.Cell(row, 3), ws.Cell(row, 12));
                        range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.InsideBorderColor = XLColor.Black;
                        range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        range.Style.Border.OutsideBorderColor = XLColor.Black;
                        range.Style.Fill.BackgroundColor = XLColor.LightGray;
                        range.Style.Font.SetBold(true);
                    }
                }

                #endregion

                return new ExcelResult(workbook, fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void BolgeUretimExcelCell(IXLWorksheet ws, MapfreBolgeUretimRaporModel model, MapfreBolgeUretimModel item, int row)
        {
            ws.Cell(row, 3).Value = model.IntToString(item.TrafikTeklif);
            ws.Cell(row, 4).Value = model.IntToString(item.TrafikPolice);
            ws.Cell(row, 5).Value = model.DecimalToString(item.TrafikBrutPrim);
            ws.Cell(row, 6).Value = model.DecimalToString(item.TrafikKomisyon);
            ws.Cell(row, 7).Value = model.DecimalToString(item.TrafikPoliceOran);
            ws.Cell(row, 8).Value = model.IntToString(item.KaskoTeklif);
            ws.Cell(row, 9).Value = model.IntToString(item.KaskoPolice);
            ws.Cell(row, 10).Value = model.DecimalToString(item.KaskoBrutPrim);
            ws.Cell(row, 11).Value = model.DecimalToString(item.KaskoKomisyon);
            ws.Cell(row, 12).Value = model.DecimalToString(item.KaskoPoliceOran);
        }
    }
}
