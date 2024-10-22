using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Models
{
    public class PerformansChart
    {
        public PerformansChart(Performans Performans)
        {
            if (Performans != null)
            {
                this.ToplamMusteri = Performans.ToplamMusteri;
                this.ToplamTeklif = Performans.ToplamTeklif;
                this.ToplamPolice = Performans.ToplamPolice;
                this.PolicelesmeOrani = Performans.PolicelesmeOrani;

                this.TeklifJSciprt = GetJScriptForTeklif(Performans);
                this.PoliceJSciprt = GetJScriptForPolice(Performans);
                this.BrutPrimJSciprt = GetJScriptForBrutPrim(Performans);
                this.KomisyonJSciprt = GetJScriptForKomisyon(Performans);

                this.ALLJScript = this.TeklifJSciprt + this.PoliceJSciprt + this.BrutPrimJSciprt + this.KomisyonJSciprt;
            }
        }

        public string ToplamMusteri { get; set; }
        public string ToplamTeklif { get; set; }
        public string ToplamPolice { get; set; }
        public string PolicelesmeOrani { get; set; }

        public string TeklifJSciprt { get; set; }
        public string PoliceJSciprt { get; set; }
        public string BrutPrimJSciprt { get; set; }
        public string KomisyonJSciprt { get; set; }
        public string ALLJScript { get; set; }

        private string GetJScriptForTeklif(Performans model)
        {
            string result = String.Empty;

            if (model != null)
            {
                int TeklifSayisi = model.TeklifList.Count();
                int Teklifmax = 0;
                if (TeklifSayisi > 0)
                    Teklifmax = model.TeklifList.Max(s => s.Adet);

                StringBuilder script = new StringBuilder();
                StringBuilder teklifHelper = new StringBuilder();
                string label = String.Empty;
                int sayac = 0;

                script.AppendLine("var chart = AmCharts.makeChart('chartdiv', {");
                script.AppendLine("type: 'pie' ,");
                script.AppendLine("theme: 'light',");
                script.AppendLine("dataProvider:[");

                var yKonum = 20;

                foreach (var urun in model.TeklifList)
                {
                    sayac++;
                    urun.UrunAdi = UrunKodlari.GetUrunAdi(urun.UrunKodu);
                    urun.UrunAdiBASE = UrunKodlari.GetUrunAdiBASE(urun.UrunKodu);

                    label += "{ text: '" + urun.UrunAdiBASE + " " + urun.Adet.ToString() + "', bold: true, x: 20, y :" + yKonum + "},";
                    yKonum = yKonum + 20;


                    if (sayac != TeklifSayisi)
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value: " + urun.Adet + "},");
                    }
                    else
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value: " + urun.Adet + "}");
                    }
                }

                script.AppendLine("],");
                script.AppendLine("valueField: 'value' ,");
                script.AppendLine("titleField:'urun',");
                script.AppendLine("outlineAlpha: 0.4,");
                script.AppendLine("depth3D: 15 ,");
                script.AppendLine("angle:30 ,");
                script.AppendLine("allLabels:[" + label + " ],");
                script.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                script.AppendLine("export: '{ enabled:true}' });");
                script.AppendLine("$('#toplam-teklif-sayisi').val(" + ToplamTeklif + ");");

                result = script.ToString();

            }

            return result;
        }

        private string GetJScriptForPolice(Performans model)
        {
            string result = String.Empty;

            if (model != null)
            {
                int PoliceSayisi = model.PoliceList.Count();

                int Policemax = 0;
                if (PoliceSayisi > 0)
                    Policemax = model.PoliceList.Max(s => s.Adet);


                StringBuilder script = new StringBuilder();
                string label = String.Empty;
                int sayac = 0;
                script.AppendLine(" var chartPolice = AmCharts.makeChart('chartdiv-police', {");
                script.AppendLine("type: 'pie' ,");
                script.AppendLine("theme: 'light',");
                script.AppendLine("dataProvider:[");
                var yKonum = 20;

                foreach (var urun in model.PoliceList)
                {
                    sayac++;
                    urun.UrunAdi = UrunKodlari.GetUrunAdi(urun.UrunKodu);
                    urun.UrunAdiBASE = UrunKodlari.GetUrunAdiBASE(urun.UrunKodu);
                    label += "{ text: '" + urun.UrunAdiBASE + " " + urun.Adet.ToString() + "', bold: true, x: 20, y :" + yKonum + "},";
                    yKonum = yKonum + 20;

                    if (sayac != PoliceSayisi)
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value: " + urun.Adet + "},");
                    }
                    else
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value: " + urun.Adet + "}");
                    }
                }
                script.AppendLine("],");
                script.AppendLine("valueField: 'value' ,");
                script.AppendLine("titleField:'urun',");
                script.AppendLine("outlineAlpha: 0.4,");
                script.AppendLine("depth3D: 15 ,");
                script.AppendLine("angle:30 ,");
                script.AppendLine("allLabels:[" + label + " ],");
                script.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                script.AppendLine("export: { enabled:true} });");

                result = script.ToString();

            }
            return result;
        }

        private string GetJScriptForBrutPrim(Performans model)
        {
            string result = String.Empty;

            if (model != null)
            {
                int PoliceSayisi = model.PoliceList.Count();
                int Policemax = 1;
                if (PoliceSayisi > 0)
                {
                    Policemax = (int)model.PoliceList.Max(s => s.BrutPrim);
                }


                StringBuilder script = new StringBuilder();
                StringBuilder policeHelper = new StringBuilder();
                string label = String.Empty;
                int sayac = 0;
                var yKonum = 20;

                script.AppendLine("var chartPrim = AmCharts.makeChart('chartdiv-prim', {");
                script.AppendLine("type: 'pie' ,");
                script.AppendLine("theme: 'light',");
                script.AppendLine("dataProvider:[");

                foreach (var urun in model.PoliceList)
                {
                    sayac++;
                    urun.UrunAdi = UrunKodlari.GetUrunAdi(urun.UrunKodu);
                    urun.UrunAdiBASE = UrunKodlari.GetUrunAdiBASE(urun.UrunKodu);

                    label += "{ text: '" + urun.UrunAdiBASE + " " + urun.BrutPrim.ToString() + "', bold: true, x: 20, y :" + yKonum + "},";
                    yKonum = yKonum + 20;

                    if (sayac != PoliceSayisi)
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value:" + Convert.ToInt32(urun.BrutPrim) + "},");
                    }
                    else
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value:" + Convert.ToInt32(urun.BrutPrim) + "},");
                    }

                }
                script.AppendLine("],");
                script.AppendLine("valueField: 'value' ,");
                script.AppendLine("titleField:'urun',");
                script.AppendLine("outlineAlpha: 0.4,");
                script.AppendLine("depth3D: 15 ,");
                script.AppendLine("angle:30 ,");
                script.AppendLine("allLabels:[" + label + " ],");
                script.AppendLine("balloonText: '[[title]]" + " ([[value]] TL) [[percents]]%' ,");
                script.AppendLine("export: { enabled:true} });");
                result = script.ToString();

            }

            return result;
        }

        private string GetJScriptForKomisyon(Performans model)
        {
            string result = String.Empty;

            if (model != null)
            {
                int PoliceSayisi = model.PoliceList.Count();
                int Policemax = 1;
                if (PoliceSayisi > 0)
                {
                    Policemax = (int)model.PoliceList.Max(s => s.BrutPrim);
                }


                StringBuilder script = new StringBuilder();
                StringBuilder policeHelper = new StringBuilder();
                var yKonum = 20;
                string label = String.Empty;

                int sayac = 0;

                script.AppendLine("var chartKomisyon = AmCharts.makeChart('chartdiv-komisyon', {");
                script.AppendLine("type: 'pie' ,");
                script.AppendLine("theme: 'light',");
                script.AppendLine("dataProvider:[");

                foreach (var urun in model.PoliceList)
                {
                    sayac++;
                    urun.UrunAdi = UrunKodlari.GetUrunAdi(urun.UrunKodu);
                    urun.UrunAdiBASE = UrunKodlari.GetUrunAdiBASE(urun.UrunKodu);

                    label += "{ text: '" + urun.UrunAdiBASE + " " + urun.ToplamKomisyon + "', bold: true, x: 20, y :" + yKonum + "},";
                    yKonum = yKonum + 20;

                    var komisyon = Convert.ToDecimal(urun.ToplamKomisyon);
                    if (sayac != PoliceSayisi)
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value:" + Convert.ToInt32(komisyon) + "},");
                    }
                    else
                    {
                        script.AppendLine("{urun:'" + urun.UrunAdiBASE + "' , value:" + Convert.ToInt32(komisyon) + "},");
                    }

                }

                script.AppendLine("],");
                script.AppendLine("valueField: 'value' ,");
                script.AppendLine("titleField:'urun',");
                script.AppendLine("outlineAlpha: 0.4,");
                script.AppendLine("depth3D: 15 ,");
                script.AppendLine("allLabels:[" + label + " ],");
                script.AppendLine("angle:30 ,");
                script.AppendLine("balloonText: '[[title]]" + " ([[value]] TL) [[percents]]%' ,");
                script.AppendLine("export: { enabled:true} });");
                result = script.ToString();

            }

            return result;
        }

        //script.AppendLine("var previousPoint2 = null;");
        //script.AppendLine("$('#site_teklif_loading').hide();");
        //script.AppendLine("$('#site_teklif_content').show();");

        //script.AppendLine("var plot_activities = $.plot($('#site_teklif'), [");
        //script.AppendLine(teklifHelper.ToString());
        //script.AppendLine("], {");
        //script.AppendLine("series: {bars: {show: true,barWidth: 0.9}},");
        //script.AppendLine("grid: {show: true,hoverable: true,clickable: false,autoHighlight: true,borderWidth: 0},");
        //script.AppendLine("yaxis: {min: 0, max: " + Teklifmax + "},");
        //script.AppendLine("xaxis: {show: false}});");

        //if (TeklifSayisi > 0)
        //    script.AppendLine("$('#site_teklif').bind('plothover', function (event, pos, item) {plothover(event, pos, item);});");



        //private string GetJScriptForTeklif(Performans model)
        //{
        //    string result = String.Empty;

        //    if (model != null)
        //    {
        //        int TeklifSayisi = model.TeklifList.Count();
        //        int Teklifmax = 0;
        //        if (TeklifSayisi > 0)
        //            Teklifmax = model.TeklifList.Max(s => s.Adet);

        //        StringBuilder script = new StringBuilder();
        //        StringBuilder teklifHelper = new StringBuilder();

        //        int sayac = 0;
        //        int rgb1 = 90;
        //        int rgb2 = 200;
        //        int rgb3 = 255;


        //        //Teklif
        //        foreach (var urun in model.TeklifList)
        //        {
        //            sayac++;
        //            urun.UrunAdi = UrunKodlari.GetUrunAdi(urun.UrunKodu);
        //            urun.UrunAdiBASE = UrunKodlari.GetUrunAdiBASE(urun.UrunKodu);

        //            script.AppendLine("var " + urun.UrunAdiBASE + " = [[" + sayac + ", " + urun.Adet + "]];");

        //            if (rgb1 > 10) rgb1 -= 10; else rgb1 += 10;
        //            if (rgb2 > 20) rgb2 -= 20; else rgb2 += 20;
        //            if (rgb3 > 10) rgb3 -= 10;


        //            string label = String.Format("{0}{1} {2}{0}", '"', urun.UrunAdi, urun.Adet);
        //            string color = String.Format("{0}{1}{0}", '"', "rgba(" + rgb1 + "," + rgb2 + "," + rgb3 + "," + " 0.7)");


        //            teklifHelper.AppendLine("{");
        //            teklifHelper.AppendLine("data:" + urun.UrunAdiBASE + ",");
        //            teklifHelper.AppendLine("label: " + label + ",");
        //            teklifHelper.AppendLine("color: " + color + ",");
        //            teklifHelper.AppendLine("shadowSize: 0,");
        //            teklifHelper.AppendLine(" bars: {show: true,lineWidth: 0,fill: true,fillColor: {colors: [{opacity: 1}, {opacity: 1}]}}}");
        //            if (sayac < TeklifSayisi)
        //                teklifHelper.Append(",");
        //        }

        //        script.AppendLine("var previousPoint2 = null;");
        //        script.AppendLine("$('#site_teklif_loading').hide();");
        //        script.AppendLine("$('#site_teklif_content').show();");

        //        script.AppendLine("var plot_activities = $.plot($('#site_teklif'), [");
        //        script.AppendLine(teklifHelper.ToString());
        //        script.AppendLine("], {");
        //        script.AppendLine("series: {bars: {show: true,barWidth: 0.9}},");
        //        script.AppendLine("grid: {show: true,hoverable: true,clickable: false,autoHighlight: true,borderWidth: 0},");
        //        script.AppendLine("yaxis: {min: 0, max: " + Teklifmax + "},");
        //        script.AppendLine("xaxis: {show: false}});");

        //        if (TeklifSayisi > 0)
        //            script.AppendLine("$('#site_teklif').bind('plothover', function (event, pos, item) {plothover(event, pos, item);});");

        //        result = script.ToString();
        //    }

        //    return result;
        //}

        //private string GetJScriptForPolice(Performans model)
        //{
        //    string result = String.Empty;

        //    if (model != null)
        //    {
        //        int PoliceSayisi = model.PoliceList.Count();

        //        int Policemax = 0;
        //        if (PoliceSayisi > 0)
        //            Policemax = model.PoliceList.Max(s => s.Adet);


        //        StringBuilder script = new StringBuilder();
        //        StringBuilder policeHelper = new StringBuilder();

        //        int sayac = 0;
        //        int rgb1 = 90;
        //        int rgb2 = 200;
        //        int rgb3 = 255;

        //        //Teklif
        //        foreach (var urun in model.PoliceList)
        //        {
        //            sayac++;
        //            urun.UrunAdi = UrunKodlari.GetUrunAdi(urun.UrunKodu);
        //            urun.UrunAdiBASE = UrunKodlari.GetUrunAdiBASE(urun.UrunKodu);

        //            script.AppendLine("var " + urun.UrunAdiBASE + " = [[" + sayac + ", " + urun.Adet + "]];");

        //            if (rgb1 > 10) rgb1 -= 10; else rgb1 += 10;
        //            if (rgb2 > 20) rgb2 -= 20; else rgb2 += 20;
        //            if (rgb3 > 10) rgb3 -= 10;

        //            string label = String.Format("{0}{1} {2}{0}", '"', urun.UrunAdi, urun.Adet);
        //            string color = String.Format("{0}{1}{0}", '"', "rgba(" + rgb1 + "," + rgb2 + "," + rgb3 + "," + " 0.7)");


        //            policeHelper.AppendLine("{");
        //            policeHelper.AppendLine("data:" + urun.UrunAdiBASE + ",");
        //            policeHelper.AppendLine("label: " + label + ",");
        //            policeHelper.AppendLine("color: " + color + ",");
        //            policeHelper.AppendLine("shadowSize: 0,");
        //            policeHelper.AppendLine("bars: {show: true,lineWidth: 0,fill: true,fillColor: {colors: [{opacity: 1}, {opacity: 1}]}}}");

        //            if (sayac < PoliceSayisi)
        //                policeHelper.Append(",");
        //        }

        //        script.AppendLine("var previousPoint2 = null;");
        //        script.AppendLine("$('#site_police_loading').hide();");
        //        script.AppendLine("$('#site_police_content').show();");

        //        script.AppendLine("var plot_activities = $.plot($('#site_police'), [");
        //        script.AppendLine(policeHelper.ToString());
        //        script.AppendLine("], {");
        //        script.AppendLine("series: {bars: {show: true,barWidth: 0.9}},");
        //        script.AppendLine("grid: {show: true,hoverable: true,clickable: false,autoHighlight: true,borderWidth: 0},");
        //        script.AppendLine("yaxis: {min: 0, max: " + Policemax + "},");
        //        script.AppendLine("xaxis: {show: false}});");

        //        if (PoliceSayisi > 0)
        //            script.AppendLine("$('#site_police').bind('plothover', function (event, pos, item) {plothover(event, pos, item);});");

        //        result = script.ToString();
        //    }

        //    return result;
        //}

        //private string GetJScriptForBrutPrim(Performans model)
        //{
        //    string result = String.Empty;

        //    if (model != null)
        //    {
        //        int PoliceSayisi = model.PoliceList.Count();
        //        int Policemax = 1;
        //        if (PoliceSayisi > 0)
        //        {
        //            Policemax = (int)model.PoliceList.Max(s => s.BrutPrim);
        //        }


        //        StringBuilder script = new StringBuilder();
        //        StringBuilder policeHelper = new StringBuilder();

        //        int sayac = 0;
        //        int rgb1 = 90;
        //        int rgb2 = 200;
        //        int rgb3 = 255;


        //        //Teklif
        //        foreach (var urun in model.PoliceList)
        //        {
        //            sayac++;
        //            urun.UrunAdi = UrunKodlari.GetUrunAdi(urun.UrunKodu);
        //            urun.UrunAdiBASE = UrunKodlari.GetUrunAdiBASE(urun.UrunKodu);

        //            script.AppendLine("var " + urun.UrunAdiBASE + " = [[" + sayac + ", " + urun.BrutPrim + "]];");

        //            if (rgb1 > 10) rgb1 -= 10; else rgb1 += 10;
        //            if (rgb2 > 20) rgb2 -= 20; else rgb2 += 20;
        //            if (rgb3 > 10) rgb3 -= 10;

        //            string label = String.Format("{0}{1} {2} Kom :{3} {0}", '"', urun.UrunAdi, urun.BrutPrim, urun.ToplamKomisyon);
        //            string color = String.Format("{0}{1}{0}", '"', "rgba(" + rgb1 + "," + rgb2 + "," + rgb3 + "," + " 0.7)");


        //            policeHelper.AppendLine("{");
        //            policeHelper.AppendLine("data:" + urun.UrunAdiBASE + ",");
        //            policeHelper.AppendLine("label: " + label + ",");
        //            policeHelper.AppendLine("color: " + color + ",");
        //            policeHelper.AppendLine("shadowSize: 0,");
        //            policeHelper.AppendLine("bars: {show: true,lineWidth: 0,fill: true,fillColor: {colors: [{opacity: 1}, {opacity: 1}]}}}");

        //            if (sayac < PoliceSayisi)
        //                policeHelper.Append(",");
        //        }

        //        script.AppendLine("var previousPoint2 = null;");
        //        script.AppendLine("$('#site_policetutar_loading').hide();");
        //        script.AppendLine("$('#site_policetutar_content').show();");

        //        script.AppendLine("var plot_activities = $.plot($('#site_policetutar'), [");
        //        script.AppendLine(policeHelper.ToString());
        //        script.AppendLine("], {");
        //        script.AppendLine("series: {bars: {show: true,barWidth: 0.9}},");
        //        script.AppendLine("grid: {show: true,hoverable: true,clickable: false,autoHighlight: true,borderWidth: 0},");
        //        script.AppendLine("yaxis: {min: 0, max: " + Policemax + "},");
        //        script.AppendLine("xaxis: {show: false}});");

        //        if (PoliceSayisi > 0)
        //            script.AppendLine("$('#site_policetutar').bind('plothover', function (event, pos, item) {plothover(event, pos, item);});");

        //        IAktifKullaniciService _aktif = DependencyResolver.Current.GetService<IAktifKullaniciService>();
        //        if (_aktif.ProjeKodu != TVMProjeKodlari.Aegon)
        //            result = script.ToString();
        //    }

        //    return result;
        //}
    }
}