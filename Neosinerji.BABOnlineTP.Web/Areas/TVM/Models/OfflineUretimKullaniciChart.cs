using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Areas.TVM.Models
{
    public class OfflineUretimKullaniciChart
    {
        public OfflineUretimKullaniciChart(OfflineUretimPerformansKullanici Performans)
        {
            if (Performans != null)
            {
                this.policeAdediJSciprt = GetJScriptForPoliceAdedi(Performans);
                this.policePrimiJSciprt = GetJScriptForPolicePrimi(Performans);
                this.polKomisyonJSciprt = GetJScriptForPoliceKomisyon(Performans);
                this.polVerilenKomisyonJSciprt = GetJScriptForPoliceVerilenKomisyon(Performans);
                this.list.Add(aylarToplam);
                this.polGenelToplamJScript = GetJScriptGenelToplam(this.list);
                this.ALLJScript = this.policeAdediJSciprt + this.policePrimiJSciprt + this.polKomisyonJSciprt + this.polVerilenKomisyonJSciprt + this.polGenelToplamJScript;
            }
        }

        public string tvmKodu { get; set; }
        public string tvmKoduTali { get; set; }
        public string donemYil { get; set; }
        public string bransKod { get; set; }

        public string policeAdediJSciprt { get; set; }
        public string policePrimiJSciprt { get; set; }
        public string polKomisyonJSciprt { get; set; }
        public string polVerilenKomisyonJSciprt { get; set; }
        public string polGenelToplamJScript { get; set; }
        public string ALLJScript { get; set; }
        string tl = " ₺";
        public List<PoliceAylar> list = new List<PoliceAylar>();
        PoliceAylar aylarToplam = new PoliceAylar();

        //Aylık police başı üretimleri hesaplamak için kullanılıyor
        private int ocakPoliceAdetToplam = 0;
        private int subatPoliceAdetToplam = 0;
        private int martPoliceAdetToplam = 0;
        private int nisanPoliceAdetToplam = 0;
        private int mayisPoliceAdetToplam = 0;
        private int haziranPoliceAdetToplam = 0;
        private int temmuzPoliceAdetToplam = 0;
        private int agustosPoliceAdetToplam = 0;
        private int eylulPoliceAdetToplam = 0;
        private int ekimPoliceAdetToplam = 0;
        private int kasimPoliceAdetToplam = 0;
        private int aralikPoliceAdetToplam = 0;
        IBransService _BransService = DependencyResolver.Current.GetService<IBransService>();
        IAktifKullaniciService _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
        private string GetJScriptForPoliceAdedi(OfflineUretimPerformansKullanici model)
        {
            string result = String.Empty;

            #region Array Lsit
            List<OfflineUretimPerformansKullaniciProcedureModel> performansList = new List<OfflineUretimPerformansKullaniciProcedureModel>();
            var uretimList = model.performansList;
            var bransSayisi = model.performansList.Select(s => s.branskodu).Distinct();
            var branslar = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            var SiraliBranslar = branslar.OrderByDescending(w => w.BransKodu).ToList();
            var BranskoduMax = SiraliBranslar.First().BransKodu;
            int[,] array = new int[BranskoduMax + 1, 13]; //Brans tablosuna brans eklendiğinde dizi boyutu artırılacak
            if (uretimList != null)
            {
                for (int i = 0; i < uretimList.Count; i++)
                {
                    //if (uretimList[i].branskodu == -9999)
                    //{
                    //    uretimList[i].branskodu = 18; //Tanımsız Brans Kodu =18
                    //}
                    if (bransSayisi.Contains(uretimList[i].branskodu))
                    {
                        if (uretimList[i].OcakPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 1] += Convert.ToInt32(uretimList[i].OcakPoliceSayisi.Value);
                        }
                        if (uretimList[i].SubatPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 2] += Convert.ToInt32(uretimList[i].SubatPoliceSayisi.Value);
                        }
                        if (uretimList[i].MartPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 3] += Convert.ToInt32(uretimList[i].MartPoliceSayisi.Value);
                        }
                        if (uretimList[i].NisanPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 4] += Convert.ToInt32(uretimList[i].NisanPoliceSayisi.Value);
                        }
                        if (uretimList[i].MayisPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 5] += Convert.ToInt32(uretimList[i].MayisPoliceSayisi.Value);
                        }
                        if (uretimList[i].HaziranPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 6] += Convert.ToInt32(uretimList[i].HaziranPoliceSayisi.Value);
                        }
                        if (uretimList[i].TemmuzPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 7] += Convert.ToInt32(uretimList[i].TemmuzPoliceSayisi.Value);
                        }
                        if (uretimList[i].AgustosPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 8] += Convert.ToInt32(uretimList[i].AgustosPoliceSayisi.Value);
                        }
                        if (uretimList[i].EylulPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 9] += Convert.ToInt32(uretimList[i].EylulPoliceSayisi.Value);
                        }
                        if (uretimList[i].EkimPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 10] += Convert.ToInt32(uretimList[i].EkimPoliceSayisi.Value);
                        }
                        if (uretimList[i].KasimPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 11] += Convert.ToInt32(uretimList[i].KasimPoliceSayisi.Value);
                        }
                        if (uretimList[i].AralikPoliceSayisi != null)
                        {
                            array[uretimList[i].branskodu, 12] += Convert.ToInt32(uretimList[i].AralikPoliceSayisi.Value);
                        }
                    }

                }
            }


            OfflineUretimPerformansKullaniciProcedureModel item = new OfflineUretimPerformansKullaniciProcedureModel();
            if (array != null)
            {
                for (int i = 1; i < BranskoduMax + 1; i++)
                {
                    item = new OfflineUretimPerformansKullaniciProcedureModel();
                    for (int j = 1; j < 13; j++)
                    {
                        if (array[i, j] != 0 && array[i, j] != null)
                        {
                            if (j == 1) //Ocak
                            {
                                item.OcakPoliceSayisi = array[i, j];
                            }
                            if (j == 2) //Şubat
                            {
                                item.SubatPoliceSayisi = array[i, j];
                            }
                            if (j == 3) //Mart
                            {
                                item.MartPoliceSayisi = array[i, j];
                            }
                            if (j == 4) //Nisan
                            {
                                item.NisanPoliceSayisi = array[i, j];
                            }
                            if (j == 5) //Mayıs
                            {
                                item.MayisPoliceSayisi = array[i, j];
                            }
                            if (j == 6) //Haziran
                            {
                                item.HaziranPoliceSayisi = array[i, j];
                            }
                            if (j == 7) //Temmuz
                            {
                                item.TemmuzPoliceSayisi = array[i, j];
                            }
                            if (j == 8) //Agustos
                            {
                                item.AgustosPoliceSayisi = array[i, j];
                            }
                            if (j == 9) //Eylul
                            {
                                item.EylulPoliceSayisi = array[i, j];
                            }
                            if (j == 10) //Ekim
                            {
                                item.EkimPoliceSayisi = array[i, j];
                            }
                            if (j == 11) //Kasım
                            {
                                item.KasimPoliceSayisi = array[i, j];
                            }
                            if (j == 12) //Aralık
                            {
                                item.AralikPoliceSayisi = array[i, j];
                            }
                        }
                    }
                    item.branskodu = i;
                    performansList.Add(item);
                }
            }

            #endregion

            if (model != null)
            {
                #region Tanımlamalar

                StringBuilder teklifHelper = new StringBuilder();
                StringBuilder scriptGenel = new StringBuilder();
                int sayac = 0;
                int ocakSayac = 0;
                int subatSayac = 0;
                int martSayac = 0;
                int nisanSayac = 0;
                int mayisSayac = 0;
                int haziranSayac = 0;
                int temmuzSayac = 0;
                int agustosSayac = 0;
                int eylulSayac = 0;
                int ekimSayac = 0;
                int kasimSayac = 0;
                int aralikSayac = 0;

                var yKonumOcak = 500;
                var yKonumSubat = 500;
                var yKonumMart = 500;
                var yKonumNisan = 500;
                var yKonumMayis = 500;
                var yKonumHaziran = 500;
                var yKonumTemmuz = 500;
                var yKonumAgustos = 500;
                var yKonumEylul = 500;
                var yKonumEkim = 500;
                var yKonumKasim = 500;
                var yKonumAralik = 500;

                var xKonumOcak = 0;
                var xKonumSubat = 0;
                var xKonumMart = 0;
                var xKonumNisan = 0;
                var xKonumMayis = 0;
                var xKonumHaziran = 0;
                var xKonumTemmuz = 0;
                var xKonumAgustos = 0;
                var xKonumEylul = 0;
                var xKonumEkim = 0;
                var xKonumKasim = 0;
                var xKonumAralik = 0;

                int TeklifSayisi = 0;

                TeklifSayisi = model.performansList.Count();

                string BransAdi = String.Empty;
                var bransListesi = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                StringBuilder scriptOcak = new StringBuilder();
                StringBuilder scriptSubat = new StringBuilder();
                StringBuilder scriptMart = new StringBuilder();
                StringBuilder scriptNisan = new StringBuilder();
                StringBuilder scriptMayis = new StringBuilder();
                StringBuilder scriptHaziran = new StringBuilder();
                StringBuilder scriptTemmuz = new StringBuilder();
                StringBuilder scriptAgustos = new StringBuilder();
                StringBuilder scriptEylul = new StringBuilder();
                StringBuilder scriptEkim = new StringBuilder();
                StringBuilder scriptKasim = new StringBuilder();
                StringBuilder scriptAralik = new StringBuilder();

                string labelOcak = String.Empty;
                string labelSubat = String.Empty;
                string labelMart = String.Empty;
                string labelNisan = String.Empty;
                string labelMayis = String.Empty;
                string labelHaziran = String.Empty;
                string labelTemmuz = String.Empty;
                string labelAgustos = String.Empty;
                string labelEylul = String.Empty;
                string labelEkim = String.Empty;
                string labelKasim = String.Empty;
                string labelAralik = String.Empty;

                scriptGenel.AppendLine("");
                //Grafik üzerindeki label ları 3 sütunlu yazdırmak icin kullanılıyor
                int satirSayacOcak = 0;
                int satirSayacSubat = 0;
                int satirSayacMart = 0;
                int satirSayacNisan = 0;
                int satirSayacMayis = 0;
                int satirSayacHaziran = 0;
                int satirSayacTemmuz = 0;
                int satirSayacAgustos = 0;
                int satirSayacEylul = 0;
                int satirSayacEkim = 0;
                int satirSayacKasim = 0;
                int satirSayacAralik = 0;

                //Aylık Toplam üretimleri göstermek için kullanılıyor
                int ocakToplam = 0;
                int subatToplam = 0;
                int martToplam = 0;
                int nisanToplam = 0;
                int mayisToplam = 0;
                int haziranToplam = 0;
                int temmuzToplam = 0;
                int agustosToplam = 0;
                int eylulToplam = 0;
                int ekimToplam = 0;
                int kasimToplam = 0;
                int aralikToplam = 0;

                #endregion

                foreach (var performans in performansList)
                {
                    #region Label text konum kontrol

                    if (performans.OcakPoliceSayisi != null)
                    {
                        if (satirSayacOcak == 3)
                        {
                            satirSayacOcak = 1;
                            xKonumOcak = 30;
                            yKonumOcak += 20;
                        }
                        else
                        {
                            if (xKonumOcak == 0)
                            {
                                xKonumOcak += 30;
                            }
                            else
                            {
                                xKonumOcak += 150;
                            }
                            satirSayacOcak++;
                        }
                    }

                    if (performans.SubatPoliceSayisi != null)
                    {
                        if (satirSayacSubat == 3)
                        {
                            satirSayacSubat = 1;
                            xKonumSubat = 30;
                            yKonumSubat += 20;
                        }
                        else
                        {
                            if (xKonumSubat == 0)
                            {
                                xKonumSubat += 30;
                            }
                            else
                            {
                                xKonumSubat += 150;
                            }
                            satirSayacSubat++;
                        }
                    }

                    if (performans.MartPoliceSayisi != null)
                    {
                        if (satirSayacMart == 3)
                        {
                            satirSayacMart = 1;
                            xKonumMart = 30;
                            yKonumMart += 20;
                        }
                        else
                        {
                            if (xKonumMart == 0)
                            {
                                xKonumMart += 30;
                            }
                            else
                            {
                                xKonumMart += 150;
                            }
                            satirSayacMart++;
                        }
                    }
                    if (performans.NisanPoliceSayisi != null)
                    {
                        if (satirSayacNisan == 3)
                        {
                            satirSayacNisan = 1;
                            xKonumNisan = 30;
                            yKonumNisan += 20;
                        }
                        else
                        {
                            if (xKonumNisan == 0)
                            {
                                xKonumNisan += 30;
                            }
                            else
                            {
                                xKonumNisan += 150;
                            }
                            satirSayacNisan++;
                        }
                    }

                    if (performans.MayisPoliceSayisi != null)
                    {
                        if (satirSayacMayis == 3)
                        {
                            satirSayacMayis = 1;
                            xKonumMayis = 30;
                            yKonumMayis += 20;
                        }
                        else
                        {
                            if (xKonumMayis == 0)
                            {
                                xKonumMayis += 30;
                            }
                            else
                            {
                                xKonumMayis += 150;
                            }
                            satirSayacMayis++;
                        }
                    }

                    if (performans.HaziranPoliceSayisi != null)
                    {
                        if (satirSayacHaziran == 3)
                        {
                            satirSayacHaziran = 1;
                            xKonumHaziran = 30;
                            yKonumHaziran += 20;
                        }
                        else
                        {
                            if (xKonumHaziran == 0)
                            {
                                xKonumHaziran += 30;
                            }
                            else
                            {
                                xKonumHaziran += 150;
                            }
                            satirSayacHaziran++;
                        }
                    }

                    if (performans.TemmuzPoliceSayisi != null)
                    {
                        if (satirSayacTemmuz == 3)
                        {
                            satirSayacTemmuz = 1;
                            xKonumTemmuz = 30;
                            yKonumTemmuz += 20;
                        }
                        else
                        {
                            if (xKonumTemmuz == 0)
                            {
                                xKonumTemmuz += 30;
                            }
                            else
                            {
                                xKonumTemmuz += 150;
                            }
                            satirSayacTemmuz++;
                        }
                    }

                    if (performans.AgustosPoliceSayisi != null)
                    {
                        if (satirSayacAgustos == 3)
                        {
                            satirSayacAgustos = 1;
                            xKonumAgustos = 30;
                            yKonumAgustos += 20;
                        }
                        else
                        {
                            if (xKonumAgustos == 0)
                            {
                                xKonumAgustos += 30;
                            }
                            else
                            {
                                xKonumAgustos += 150;
                            }
                            satirSayacAgustos++;
                        }
                    }


                    if (performans.EylulPoliceSayisi != null)
                    {
                        if (satirSayacEylul == 3)
                        {
                            satirSayacEylul = 1;
                            xKonumEylul = 30;
                            yKonumEylul += 20;
                        }
                        else
                        {
                            if (xKonumEylul == 0)
                            {
                                xKonumEylul += 30;
                            }
                            else
                            {
                                xKonumEylul += 150;
                            }
                            satirSayacEylul++;
                        }
                    }

                    if (performans.EkimPoliceSayisi != null)
                    {
                        if (satirSayacEkim == 3)
                        {
                            satirSayacEkim = 1;
                            xKonumEkim = 30;
                            yKonumEkim += 20;
                        }
                        else
                        {
                            if (xKonumEkim == 0)
                            {
                                xKonumEkim += 30;
                            }
                            else
                            {
                                xKonumEkim += 150;
                            }
                            satirSayacEkim++;
                        }
                    }

                    if (performans.KasimPoliceSayisi != null)
                    {
                        if (satirSayacKasim == 3)
                        {
                            satirSayacKasim = 1;
                            xKonumKasim = 30;
                            yKonumKasim += 20;
                        }
                        else
                        {
                            if (xKonumKasim == 0)
                            {
                                xKonumKasim += 30;
                            }
                            else
                            {
                                xKonumKasim += 150;
                            }
                            satirSayacKasim++;
                        }
                    }

                    if (performans.AralikPoliceSayisi != null)
                    {
                        if (satirSayacAralik == 3)
                        {
                            satirSayacAralik = 1;
                            xKonumAralik = 30;
                            yKonumAralik += 20;
                        }
                        else
                        {
                            if (xKonumAralik == 0)
                            {
                                xKonumAralik += 30;
                            }
                            else
                            {
                                xKonumAralik += 150;
                            }
                            satirSayacAralik++;
                        }
                    }


                    #endregion

                    if (performans.OcakPoliceSayisi != null)
                    {
                        #region Ocak Poliçe sayıları
                        ocakSayac++;
                        if (ocakSayac == 1)
                        {
                            scriptOcak.AppendLine("var chartOcakPolice = AmCharts.makeChart('ocak-chartdiv-police', {");
                            scriptOcak.AppendLine("type: 'pie' ,");
                            scriptOcak.AppendLine("theme: 'light',");
                            scriptOcak.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }
                        if (performans.OcakPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.OcakPoliceSayisi.Value).ToString("N").Split(',');
                            labelOcak += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumOcak + ", y :" + yKonumOcak + "},";
                        }
                        else
                        {

                            labelOcak += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumOcak + " y :" + yKonumOcak + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value:     " + Convert.ToInt32(performans.OcakPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value:     " + Convert.ToInt32(performans.OcakPoliceSayisi) + "},");
                        }
                        ocakToplam += Convert.ToInt32(performans.OcakPoliceSayisi);
                        #endregion
                    }

                    if (performans.SubatPoliceSayisi != null)
                    {
                        #region Şubat Poliçe sayıları

                        subatSayac++;
                        if (subatSayac == 1)
                        {
                            scriptSubat.AppendLine("var chartSubatPolice = AmCharts.makeChart('subat-chartdiv-police', {");
                            scriptSubat.AppendLine("type: 'pie' ,");
                            scriptSubat.AppendLine("theme: 'light',");
                            scriptSubat.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.SubatPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.SubatPoliceSayisi.Value).ToString("N").Split(',');
                            labelSubat += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x:  " + xKonumSubat + ", y :" + yKonumSubat + "},";
                        }
                        else
                        {
                            labelSubat += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumSubat + ", y :" + yKonumSubat + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPoliceSayisi) + "},");
                        }

                        subatToplam += Convert.ToInt32(performans.SubatPoliceSayisi);

                        #endregion
                    }

                    if (performans.MartPoliceSayisi != null)
                    {
                        #region Mart Poliçe sayıları

                        martSayac++;
                        if (martSayac == 1)
                        {
                            scriptMart.AppendLine("var chartMartPolice = AmCharts.makeChart('mart-chartdiv-police', {");
                            scriptMart.AppendLine("type: 'pie' ,");
                            scriptMart.AppendLine("theme: 'light',");
                            scriptMart.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MartPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.MartPoliceSayisi.Value).ToString("N").Split(',');
                            labelMart += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumMart + ", y :" + yKonumMart + "},";
                        }
                        else
                        {
                            labelMart += "{ text: '" + BransAdi + " 0 ', bold: true, x:  " + xKonumMart + ", y :" + yKonumMart + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPoliceSayisi) + "},");
                        }

                        martToplam += Convert.ToInt32(performans.MartPoliceSayisi);
                        #endregion
                    }

                    if (performans.NisanPoliceSayisi != null)
                    {
                        #region Nisan Poliçe sayıları

                        nisanSayac++;
                        if (nisanSayac == 1)
                        {
                            scriptNisan.AppendLine("var chartNisanPolice = AmCharts.makeChart('nisan-chartdiv-police', {");
                            scriptNisan.AppendLine("type: 'pie' ,");
                            scriptNisan.AppendLine("theme: 'light',");
                            scriptNisan.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.NisanPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.NisanPoliceSayisi.Value).ToString("N").Split(',');
                            labelNisan += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x:  " + xKonumNisan + ", y :" + yKonumNisan + "},";
                        }
                        else
                        {
                            labelNisan += "{ text: '" + BransAdi + " 0 ', bold: true, x:  " + xKonumNisan + ", y :" + yKonumNisan + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPoliceSayisi) + "},");
                        }
                        nisanToplam += Convert.ToInt32(performans.NisanPoliceSayisi);
                        #endregion
                    }

                    if (performans.MayisPoliceSayisi != null)
                    {
                        #region Mayis Poliçe sayıları

                        mayisSayac++;
                        if (mayisSayac == 1)
                        {
                            scriptMayis.AppendLine("var chartMayisPolice = AmCharts.makeChart('mayis-chartdiv-police', {");
                            scriptMayis.AppendLine("type: 'pie' ,");
                            scriptMayis.AppendLine("theme: 'light',");
                            scriptMayis.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MayisPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.MayisPoliceSayisi.Value).ToString("N").Split(',');
                            labelMayis += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x:  " + xKonumMayis + ", y :" + yKonumMayis + "},";
                        }
                        else
                        {
                            labelMayis += "{ text: '" + BransAdi + " 0 ', bold: true, x:  " + xKonumMayis + ", y :" + yKonumMayis + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPoliceSayisi) + "},");
                        }
                        mayisToplam += Convert.ToInt32(performans.MayisPoliceSayisi);
                        #endregion
                    }

                    if (performans.HaziranPoliceSayisi != null)
                    {
                        #region Haziran Poliçe sayıları

                        haziranSayac++;
                        if (haziranSayac == 1)
                        {
                            scriptHaziran.AppendLine("var chartHaziranPolice = AmCharts.makeChart('haziran-chartdiv-police', {");
                            scriptHaziran.AppendLine("type: 'pie' ,");
                            scriptHaziran.AppendLine("theme: 'light',");
                            scriptHaziran.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.HaziranPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.HaziranPoliceSayisi.Value).ToString("N").Split(',');
                            labelHaziran += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x:  " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                        }
                        else
                        {
                            labelHaziran += "{ text: '" + BransAdi + " 0 ', bold: true, x:  " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPoliceSayisi) + "},");
                        }
                        haziranToplam += Convert.ToInt32(performans.HaziranPoliceSayisi);
                        #endregion
                    }

                    if (performans.TemmuzPoliceSayisi != null)
                    {
                        #region Temmuz Poliçe sayıları

                        temmuzSayac++;
                        if (temmuzSayac == 1)
                        {
                            scriptTemmuz.AppendLine("var chartTemmuzPolice = AmCharts.makeChart('temmuz-chartdiv-police', {");
                            scriptTemmuz.AppendLine("type: 'pie' ,");
                            scriptTemmuz.AppendLine("theme: 'light',");
                            scriptTemmuz.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.TemmuzPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.TemmuzPoliceSayisi.Value).ToString("N").Split(',');
                            labelTemmuz += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x:  " + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                        }
                        else
                        {
                            labelTemmuz += "{ text: '" + BransAdi + " 0 ', bold: true, x:  " + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPoliceSayisi) + "},");
                        }
                        temmuzToplam += Convert.ToInt32(performans.TemmuzPoliceSayisi);
                        #endregion
                    }

                    if (performans.AgustosPoliceSayisi != null)
                    {
                        #region Ağustos Poliçe sayıları
                        agustosSayac++;
                        if (agustosSayac == 1)
                        {
                            scriptAgustos.AppendLine("var chartAgustosPolice = AmCharts.makeChart('agustos-chartdiv-police', {");
                            scriptAgustos.AppendLine("type: 'pie' ,");
                            scriptAgustos.AppendLine("theme: 'light',");
                            scriptAgustos.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AgustosPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.AgustosPoliceSayisi.Value).ToString("N").Split(',');
                            labelAgustos += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAgustos + ", y :" + yKonumAgustos + "},";
                        }
                        else
                        {
                            labelAgustos += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + (xKonumAgustos - 150) + ", y :" + yKonumAgustos + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value:" + Convert.ToInt32(performans.AgustosPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AgustosPoliceSayisi) + "},");
                        }
                        agustosToplam += Convert.ToInt32(performans.AgustosPoliceSayisi);

                        #endregion
                    }

                    if (performans.EylulPoliceSayisi != null)
                    {
                        #region Eylül Poliçe sayıları

                        eylulSayac++;
                        if (eylulSayac == 1)
                        {
                            scriptEylul.AppendLine("var chartEylulPolice = AmCharts.makeChart('eylul-chartdiv-police', {");
                            scriptEylul.AppendLine("type: 'pie' ,");
                            scriptEylul.AppendLine("theme: 'light',");
                            scriptEylul.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EylulPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.EylulPoliceSayisi.Value).ToString("N").Split(',');
                            labelEylul += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEylul + ", y :" + yKonumEylul + "},";
                        }
                        else
                        {
                            labelEylul += "{ text: '" + BransAdi + " 0 ', bold: true, x:" + xKonumEylul + ", y :" + yKonumEylul + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPoliceSayisi) + "},");
                        }
                        eylulToplam += Convert.ToInt32(performans.EylulPoliceSayisi);

                        #endregion
                    }

                    if (performans.EkimPoliceSayisi != null)
                    {
                        #region Ekim Poliçe sayıları

                        ekimSayac++;
                        if (ekimSayac == 1)
                        {
                            scriptEkim.AppendLine("var chartEkimPolice = AmCharts.makeChart('ekim-chartdiv-police', {");
                            scriptEkim.AppendLine("type: 'pie' ,");
                            scriptEkim.AppendLine("theme: 'light',");
                            scriptEkim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EkimPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.EkimPoliceSayisi.Value).ToString("N").Split(',');
                            labelEkim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                        }
                        else
                        {
                            labelEkim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPoliceSayisi) + "},");
                        }
                        ekimToplam += Convert.ToInt32(performans.EkimPoliceSayisi);

                        #endregion
                    }

                    if (performans.KasimPoliceSayisi != null)
                    {
                        #region kasım Poliçe sayıları

                        kasimSayac++;
                        if (kasimSayac == 1)
                        {
                            scriptKasim.AppendLine("var chartKasimPolice = AmCharts.makeChart('kasim-chartdiv-police', {");
                            scriptKasim.AppendLine("type: 'pie' ,");
                            scriptKasim.AppendLine("theme: 'light',");
                            scriptKasim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.KasimPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.KasimPoliceSayisi.Value).ToString("N").Split(',');
                            labelKasim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                        }
                        else
                        {
                            labelKasim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPoliceSayisi) + "},");
                        }
                        kasimToplam += Convert.ToInt32(performans.KasimPoliceSayisi);

                        #endregion
                    }

                    if (performans.AralikPoliceSayisi != null)
                    {
                        #region Aralik Poliçe sayıları

                        aralikSayac++;
                        if (aralikSayac == 1)
                        {
                            scriptAralik.AppendLine("var chartAralikPolice = AmCharts.makeChart('aralik-chartdiv-police', {");
                            scriptAralik.AppendLine("type: 'pie' ,");
                            scriptAralik.AppendLine("theme: 'light',");
                            scriptAralik.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AralikPoliceSayisi != null)
                        {
                            string[] parts = Convert.ToInt32(performans.AralikPoliceSayisi.Value).ToString("N").Split(',');
                            labelAralik += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                        }
                        else
                        {
                            labelAralik += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                        }
                        if (sayac != TeklifSayisi)
                        {
                            scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPoliceSayisi) + "},");
                        }
                        else
                        {
                            scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPoliceSayisi) + "},");
                        }
                        aralikToplam += Convert.ToInt32(performans.AralikPoliceSayisi);

                        #endregion
                    }

                }

                aylarToplam.policeAdetOcak = ocakToplam;
                aylarToplam.policeAdetSubat = subatToplam;
                aylarToplam.policeAdetMart = martToplam;
                aylarToplam.policeAdetNisan = nisanToplam;
                aylarToplam.policeAdetMayis = mayisToplam;
                aylarToplam.policeAdetHaziran = haziranToplam;
                aylarToplam.policeAdetTemmuz = temmuzToplam;
                aylarToplam.policeAdetAgustos = agustosToplam;
                aylarToplam.policeAdetEylul = eylulToplam;
                aylarToplam.policeAdetEkim = ekimToplam;
                aylarToplam.policeAdetKasim = kasimToplam;
                aylarToplam.policeAdetAralik = aralikToplam;

                ocakPoliceAdetToplam = ocakToplam;
                subatPoliceAdetToplam = subatToplam;
                martPoliceAdetToplam = martToplam;
                nisanPoliceAdetToplam = nisanToplam;
                mayisPoliceAdetToplam = mayisToplam;
                haziranPoliceAdetToplam = haziranToplam;
                temmuzPoliceAdetToplam = temmuzToplam;
                agustosPoliceAdetToplam = agustosToplam;
                eylulPoliceAdetToplam = eylulToplam;
                ekimPoliceAdetToplam = ekimToplam;
                kasimPoliceAdetToplam = kasimToplam;
                aralikPoliceAdetToplam = aralikToplam;

                #region Script Sonu

                if (ocakSayac > 0)
                {
                    string[] parts = ocakToplam.ToString("N").Split(',');
                    labelOcak += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumOcak + 35) + "},";
                    scriptOcak.AppendLine("],");
                    scriptOcak.AppendLine("valueField: 'value' ,");
                    scriptOcak.AppendLine("titleField:'brans',");
                    scriptOcak.AppendLine("outlineAlpha: 0.4,");
                    scriptOcak.AppendLine("depth3D: 15 ,");
                    scriptOcak.AppendLine("angle:30 ,");
                    scriptOcak.AppendLine("allLabels:[" + labelOcak + " ],");
                    scriptOcak.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptOcak.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptOcak;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptOcak.AppendLine("$('.ocak-police-img').show();");
                    scriptGenel = scriptOcak;

                    result += scriptGenel.ToString();
                }
                if (subatSayac > 0)
                {
                    string[] parts = subatToplam.ToString("N").Split(',');
                    labelSubat += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumSubat + 35) + "},";

                    scriptSubat.AppendLine("],");
                    scriptSubat.AppendLine("valueField: 'value' ,");
                    scriptSubat.AppendLine("titleField:'brans',");
                    scriptSubat.AppendLine("outlineAlpha: 0.4,");
                    scriptSubat.AppendLine("depth3D: 15 ,");
                    scriptSubat.AppendLine("angle:30 ,");
                    scriptSubat.AppendLine("allLabels:[" + labelSubat + " ],");
                    scriptSubat.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptSubat.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptSubat;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptSubat.AppendLine("$('.subat-police-img').show();");
                    scriptGenel = scriptSubat;

                    result += scriptGenel.ToString();
                }
                if (martSayac > 0)
                {
                    string[] parts = martToplam.ToString("N").Split(',');
                    labelMart += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumMart + 35) + "},";
                    scriptMart.AppendLine("],");
                    scriptMart.AppendLine("valueField: 'value' ,");
                    scriptMart.AppendLine("titleField:'brans',");
                    scriptMart.AppendLine("outlineAlpha: 0.4,");
                    scriptMart.AppendLine("depth3D: 15 ,");
                    scriptMart.AppendLine("angle:30 ,");
                    scriptMart.AppendLine("allLabels:[" + labelMart + " ],");
                    scriptMart.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptMart.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMart.AppendLine("$('.mart-police-img').show();");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                if (nisanSayac > 0)
                {
                    string[] parts = nisanToplam.ToString("N").Split(',');
                    labelNisan += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumNisan + 35) + "},";

                    scriptNisan.AppendLine("],");
                    scriptNisan.AppendLine("valueField: 'value' ,");
                    scriptNisan.AppendLine("titleField:'brans',");
                    scriptNisan.AppendLine("outlineAlpha: 0.4,");
                    scriptNisan.AppendLine("depth3D: 15 ,");
                    scriptNisan.AppendLine("angle:30 ,");
                    scriptNisan.AppendLine("allLabels:[" + labelNisan + " ],");
                    scriptNisan.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptNisan.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptNisan.AppendLine("$('.nisan-police-img').show();");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                if (mayisSayac > 0)
                {
                    string[] parts = mayisToplam.ToString("N").Split(',');
                    labelMayis += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumMayis + 35) + "},";

                    scriptMayis.AppendLine("],");
                    scriptMayis.AppendLine("valueField: 'value' ,");
                    scriptMayis.AppendLine("titleField:'brans',");
                    scriptMayis.AppendLine("outlineAlpha: 0.4,");
                    scriptMayis.AppendLine("depth3D: 15 ,");
                    scriptMayis.AppendLine("angle:30 ,");
                    scriptMayis.AppendLine("allLabels:[" + labelMayis + " ],");
                    scriptMayis.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptMayis.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMayis.AppendLine("$('.mayis-police-img').show();");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                if (haziranSayac > 0)
                {
                    string[] parts = haziranToplam.ToString("N").Split(',');
                    labelHaziran += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumHaziran + 35) + "},";

                    scriptHaziran.AppendLine("],");
                    scriptHaziran.AppendLine("valueField: 'value' ,");
                    scriptHaziran.AppendLine("titleField:'brans',");
                    scriptHaziran.AppendLine("outlineAlpha: 0.4,");
                    scriptHaziran.AppendLine("depth3D: 15 ,");
                    scriptHaziran.AppendLine("angle:30 ,");
                    scriptHaziran.AppendLine("allLabels:[" + labelHaziran + " ],");
                    scriptHaziran.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptHaziran.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptHaziran.AppendLine("$('.haziran-police-img').show();");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                if (temmuzSayac > 0)
                {
                    string[] parts = temmuzToplam.ToString("N").Split(',');
                    labelTemmuz += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumTemmuz + 35) + "},";

                    scriptTemmuz.AppendLine("],");
                    scriptTemmuz.AppendLine("valueField: 'value' ,");
                    scriptTemmuz.AppendLine("titleField:'brans',");
                    scriptTemmuz.AppendLine("outlineAlpha: 0.4,");
                    scriptTemmuz.AppendLine("depth3D: 15 ,");
                    scriptTemmuz.AppendLine("angle:30 ,");
                    scriptTemmuz.AppendLine("allLabels:[" + labelTemmuz + " ],");
                    scriptTemmuz.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptTemmuz.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptTemmuz;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptTemmuz.AppendLine("$('.temmuz-police-img').show();");
                    scriptGenel = scriptTemmuz;

                    result += scriptGenel.ToString();
                }
                if (agustosSayac > 0)
                {
                    string[] parts = agustosToplam.ToString("N").Split(',');
                    labelAgustos += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumAgustos + 35) + "},";

                    scriptAgustos.AppendLine("],");
                    scriptAgustos.AppendLine("valueField: 'value' ,");
                    scriptAgustos.AppendLine("titleField:'brans',");
                    scriptAgustos.AppendLine("outlineAlpha: 0.4,");
                    scriptAgustos.AppendLine("depth3D: 15 ,");
                    scriptAgustos.AppendLine("angle:30 ,");
                    scriptAgustos.AppendLine("allLabels:[" + labelAgustos + " ],");
                    scriptAgustos.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptAgustos.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAgustos.AppendLine("$('.agustos-police-img').show();");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                if (eylulSayac > 0)
                {
                    string[] parts = eylulToplam.ToString("N").Split(',');
                    labelEylul += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumEylul + 35) + "},";

                    scriptEylul.AppendLine("],");
                    scriptEylul.AppendLine("valueField: 'value' ,");
                    scriptEylul.AppendLine("titleField:'brans',");
                    scriptEylul.AppendLine("outlineAlpha: 0.4,");
                    scriptEylul.AppendLine("depth3D: 15 ,");
                    scriptEylul.AppendLine("angle:30 ,");
                    scriptEylul.AppendLine("allLabels:[" + labelEylul + " ],");
                    scriptEylul.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptEylul.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEylul.AppendLine("$('.eylul-police-img').show();");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                if (ekimSayac > 0)
                {
                    string[] parts = ekimToplam.ToString("N").Split(',');
                    labelEkim += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumEkim + 35) + "},";

                    scriptEkim.AppendLine("],");
                    scriptEkim.AppendLine("valueField: 'value' ,");
                    scriptEkim.AppendLine("titleField:'brans',");
                    scriptEkim.AppendLine("outlineAlpha: 0.4,");
                    scriptEkim.AppendLine("depth3D: 15 ,");
                    scriptEkim.AppendLine("angle:30 ,");
                    scriptEkim.AppendLine("allLabels:[" + labelEkim + " ],");
                    scriptEkim.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptEkim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEkim.AppendLine("$('.ekim-police-img').show();");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                if (kasimSayac > 0)
                {
                    string[] parts = kasimToplam.ToString("N").Split(',');
                    labelKasim += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumKasim + 35) + "},";

                    scriptKasim.AppendLine("],");
                    scriptKasim.AppendLine("valueField: 'value' ,");
                    scriptKasim.AppendLine("titleField:'brans',");
                    scriptKasim.AppendLine("outlineAlpha: 0.4,");
                    scriptKasim.AppendLine("depth3D: 15 ,");
                    scriptKasim.AppendLine("angle:30 ,");
                    scriptKasim.AppendLine("allLabels:[" + labelKasim + " ],");
                    scriptKasim.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptKasim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptKasim.AppendLine("$('.kasim-police-img').show();");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                if (aralikSayac > 0)
                {
                    string[] parts = aralikToplam.ToString("N").Split(',');
                    labelAralik += "{ text: 'Toplam Poliçe Adedi: " + parts[0] + " ', bold: true, x: " + 30 + ", y :" + (yKonumAralik + 35) + "},";

                    scriptAralik.AppendLine("],");
                    scriptAralik.AppendLine("valueField: 'value' ,");
                    scriptAralik.AppendLine("titleField:'brans',");
                    scriptAralik.AppendLine("outlineAlpha: 0.4,");
                    scriptAralik.AppendLine("depth3D: 15 ,");
                    scriptAralik.AppendLine("angle:30 ,");
                    scriptAralik.AppendLine("allLabels:[" + labelAralik + " ],");
                    scriptAralik.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                    scriptAralik.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAralik.AppendLine("$('.aralik-police-img').show();");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }

                #endregion
            }

            return result;
        }

        private string GetJScriptForPolicePrimi(OfflineUretimPerformansKullanici model)
        {
            string result = String.Empty;
            #region Array Lsit
            List<OfflineUretimPerformansKullaniciProcedureModel> performansList = new List<OfflineUretimPerformansKullaniciProcedureModel>();

            var uretimList = model.performansList;
            var bransSayisi = model.performansList.Select(s => s.branskodu).Distinct();
            var branslar = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            var SiraliBranslar = branslar.OrderByDescending(w => w.BransKodu).ToList();
            var BranskoduMax = SiraliBranslar.First().BransKodu;
            int[,] array = new int[BranskoduMax + 1, 13];
            if (uretimList != null)
            {
                for (int i = 0; i < uretimList.Count; i++)
                {
                    if (bransSayisi.Contains(uretimList[i].branskodu))
                    {
                        if (uretimList[i].OcakPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 1] += Convert.ToInt32(uretimList[i].OcakPolicePrimi.Value);
                        }
                        if (uretimList[i].SubatPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 2] += Convert.ToInt32(uretimList[i].SubatPolicePrimi.Value);
                        }
                        if (uretimList[i].MartPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 3] += Convert.ToInt32(uretimList[i].MartPolicePrimi.Value);
                        }
                        if (uretimList[i].NisanPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 4] += Convert.ToInt32(uretimList[i].NisanPolicePrimi.Value);
                        }
                        if (uretimList[i].MayisPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 5] += Convert.ToInt32(uretimList[i].MayisPolicePrimi.Value);
                        }
                        if (uretimList[i].HaziranPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 6] += Convert.ToInt32(uretimList[i].HaziranPolicePrimi.Value);
                        }
                        if (uretimList[i].TemmuzPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 7] += Convert.ToInt32(uretimList[i].TemmuzPolicePrimi.Value);
                        }
                        if (uretimList[i].AgustosPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 8] += Convert.ToInt32(uretimList[i].AgustosPolicePrimi.Value);
                        }
                        if (uretimList[i].EylulPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 9] += Convert.ToInt32(uretimList[i].EylulPolicePrimi.Value);
                        }
                        if (uretimList[i].EkimPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 10] += Convert.ToInt32(uretimList[i].EkimPolicePrimi.Value);
                        }
                        if (uretimList[i].KasimPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 11] += Convert.ToInt32(uretimList[i].KasimPolicePrimi.Value);
                        }
                        if (uretimList[i].AralikPolicePrimi != null)
                        {
                            array[uretimList[i].branskodu, 12] += Convert.ToInt32(uretimList[i].AralikPolicePrimi.Value);
                        }
                    }

                }
            }

            OfflineUretimPerformansKullaniciProcedureModel item = new OfflineUretimPerformansKullaniciProcedureModel();
            if (array != null)
            {
                for (int i = 1; i < BranskoduMax + 1; i++)
                {
                    item = new OfflineUretimPerformansKullaniciProcedureModel();
                    for (int j = 1; j < 13; j++)
                    {
                        if (array[i, j] != 0 && array[i, j] != null)
                        {
                            if (j == 1) //Ocak
                            {
                                item.OcakPolicePrimi = array[i, j];
                            }
                            if (j == 2) //Şubat
                            {
                                item.SubatPolicePrimi = array[i, j];
                            }
                            if (j == 3) //Mart
                            {
                                item.MartPolicePrimi = array[i, j];
                            }
                            if (j == 4) //Nisan
                            {
                                item.NisanPolicePrimi = array[i, j];
                            }
                            if (j == 5) //Mayıs
                            {
                                item.MayisPolicePrimi = array[i, j];
                            }
                            if (j == 6) //Haziran
                            {
                                item.HaziranPolicePrimi = array[i, j];
                            }
                            if (j == 7) //Temmuz
                            {
                                item.TemmuzPolicePrimi = array[i, j];
                            }
                            if (j == 8) //Agustos
                            {
                                item.AgustosPolicePrimi = array[i, j];
                            }
                            if (j == 9) //Eylul
                            {
                                item.EylulPolicePrimi = array[i, j];
                            }
                            if (j == 10) //Ekim
                            {
                                item.EkimPolicePrimi = array[i, j];
                            }
                            if (j == 11) //Kasım
                            {
                                item.KasimPolicePrimi = array[i, j];
                            }
                            if (j == 12) //Aralık
                            {
                                item.AralikPolicePrimi = array[i, j];
                            }
                        }
                    }
                    item.branskodu = i;
                    performansList.Add(item);
                }
            }

            #endregion
            if (model != null)
            {
                #region Tanımlamalar

                StringBuilder teklifHelper = new StringBuilder();
                StringBuilder scriptGenel = new StringBuilder();
                int sayac = 0;
                int ocakSayac = 0;
                int subatSayac = 0;
                int martSayac = 0;
                int nisanSayac = 0;
                int mayisSayac = 0;
                int haziranSayac = 0;
                int temmuzSayac = 0;
                int agustosSayac = 0;
                int eylulSayac = 0;
                int ekimSayac = 0;
                int kasimSayac = 0;
                int aralikSayac = 0;

                var yKonumOcak = 500;
                var yKonumSubat = 500;
                var yKonumMart = 500;
                var yKonumNisan = 500;
                var yKonumMayis = 500;
                var yKonumHaziran = 500;
                var yKonumTemmuz = 500;
                var yKonumAgustos = 500;
                var yKonumEylul = 500;
                var yKonumEkim = 500;
                var yKonumKasim = 500;
                var yKonumAralik = 500;

                var xKonumOcak = 0;
                var xKonumSubat = 0;
                var xKonumMart = 0;
                var xKonumNisan = 0;
                var xKonumMayis = 0;
                var xKonumHaziran = 0;
                var xKonumTemmuz = 0;
                var xKonumAgustos = 0;
                var xKonumEylul = 0;
                var xKonumEkim = 0;
                var xKonumKasim = 0;
                var xKonumAralik = 0;

                int satirSayacOcak = 0;
                int satirSayacSubat = 0;
                int satirSayacMart = 0;
                int satirSayacNisan = 0;
                int satirSayacMayis = 0;
                int satirSayacHaziran = 0;
                int satirSayacTemmuz = 0;
                int satirSayacAgustos = 0;
                int satirSayacEylul = 0;
                int satirSayacEkim = 0;
                int satirSayacKasim = 0;
                int satirSayacAralik = 0;

                int TeklifSayisi = 0;

                TeklifSayisi = model.performansList.Count();

                string BransAdi = String.Empty;
                var bransListesi = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                StringBuilder scriptOcak = new StringBuilder();
                StringBuilder scriptSubat = new StringBuilder();
                StringBuilder scriptMart = new StringBuilder();
                StringBuilder scriptNisan = new StringBuilder();
                StringBuilder scriptMayis = new StringBuilder();
                StringBuilder scriptHaziran = new StringBuilder();
                StringBuilder scriptTemmuz = new StringBuilder();
                StringBuilder scriptAgustos = new StringBuilder();
                StringBuilder scriptEylul = new StringBuilder();
                StringBuilder scriptEkim = new StringBuilder();
                StringBuilder scriptKasim = new StringBuilder();
                StringBuilder scriptAralik = new StringBuilder();

                string labelOcak = String.Empty;
                string labelSubat = String.Empty;
                string labelMart = String.Empty;
                string labelNisan = String.Empty;
                string labelMayis = String.Empty;
                string labelHaziran = String.Empty;
                string labelTemmuz = String.Empty;
                string labelAgustos = String.Empty;
                string labelEylul = String.Empty;
                string labelEkim = String.Empty;
                string labelKasim = String.Empty;
                string labelAralik = String.Empty;

                //Aylık Toplam üretimleri göstermek için kullanılıyor
                decimal ocakToplam = 0;
                decimal subatToplam = 0;
                decimal martToplam = 0;
                decimal nisanToplam = 0;
                decimal mayisToplam = 0;
                decimal haziranToplam = 0;
                decimal temmuzToplam = 0;
                decimal agustosToplam = 0;
                decimal eylulToplam = 0;
                decimal ekimToplam = 0;
                decimal kasimToplam = 0;
                decimal aralikToplam = 0;

                scriptGenel.AppendLine("");
                #endregion

                foreach (var performans in performansList)
                {
                    #region Label text konum kontrol

                    if (performans.OcakPolicePrimi != null)
                    {
                        if (satirSayacOcak == 3)
                        {
                            satirSayacOcak = 1;
                            xKonumOcak = 30;
                            yKonumOcak += 20;
                        }
                        else
                        {
                            if (xKonumOcak == 0)
                            {
                                xKonumOcak += 30;
                            }
                            else
                            {
                                xKonumOcak += 150;
                            }
                            satirSayacOcak++;
                        }
                    }

                    if (performans.SubatPolicePrimi != null)
                    {
                        if (satirSayacSubat == 3)
                        {
                            satirSayacSubat = 1;
                            xKonumSubat = 30;
                            yKonumSubat += 20;
                        }
                        else
                        {
                            if (xKonumSubat == 0)
                            {
                                xKonumSubat += 30;
                            }
                            else
                            {
                                xKonumSubat += 150;
                            }
                            satirSayacSubat++;
                        }
                    }

                    if (performans.MartPolicePrimi != null)
                    {
                        if (satirSayacMart == 3)
                        {
                            satirSayacMart = 1;
                            xKonumMart = 30;
                            yKonumMart += 20;
                        }
                        else
                        {
                            if (xKonumMart == 0)
                            {
                                xKonumMart += 30;
                            }
                            else
                            {
                                xKonumMart += 150;
                            }
                            satirSayacMart++;
                        }
                    }
                    if (performans.NisanPolicePrimi != null)
                    {
                        if (satirSayacNisan == 3)
                        {
                            satirSayacNisan = 1;
                            xKonumNisan = 30;
                            yKonumNisan += 20;
                        }
                        else
                        {
                            if (xKonumNisan == 0)
                            {
                                xKonumNisan += 30;
                            }
                            else
                            {
                                xKonumNisan += 150;
                            }
                            satirSayacNisan++;
                        }
                    }

                    if (performans.MayisPolicePrimi != null)
                    {
                        if (satirSayacMayis == 3)
                        {
                            satirSayacMayis = 1;
                            xKonumMayis = 30;
                            yKonumMayis += 20;
                        }
                        else
                        {
                            if (xKonumMayis == 0)
                            {
                                xKonumMayis += 30;
                            }
                            else
                            {
                                xKonumMayis += 150;
                            }
                            satirSayacMayis++;
                        }
                    }

                    if (performans.HaziranPolicePrimi != null)
                    {
                        if (satirSayacHaziran == 3)
                        {
                            satirSayacHaziran = 1;
                            xKonumHaziran = 30;
                            yKonumHaziran += 20;
                        }
                        else
                        {
                            if (xKonumHaziran == 0)
                            {
                                xKonumHaziran += 30;
                            }
                            else
                            {
                                xKonumHaziran += 150;
                            }
                            satirSayacHaziran++;
                        }
                    }

                    if (performans.TemmuzPolicePrimi != null)
                    {
                        if (satirSayacTemmuz == 3)
                        {
                            satirSayacTemmuz = 1;
                            xKonumTemmuz = 30;
                            yKonumTemmuz += 20;
                        }
                        else
                        {
                            if (xKonumTemmuz == 0)
                            {
                                xKonumTemmuz += 30;
                            }
                            else
                            {
                                xKonumTemmuz += 150;
                            }
                            satirSayacTemmuz++;
                        }
                    }

                    if (performans.AgustosPolicePrimi != null)
                    {
                        if (satirSayacAgustos == 3)
                        {
                            satirSayacAgustos = 1;
                            xKonumAgustos = 30;
                            yKonumAgustos += 20;
                        }
                        else
                        {
                            if (xKonumAgustos == 0)
                            {
                                xKonumAgustos += 30;
                            }
                            else
                            {
                                xKonumAgustos += 150;
                            }
                            satirSayacAgustos++;
                        }
                    }


                    if (performans.EylulPolicePrimi != null)
                    {
                        if (satirSayacEylul == 3)
                        {
                            satirSayacEylul = 1;
                            xKonumEylul = 30;
                            yKonumEylul += 20;
                        }
                        else
                        {
                            if (xKonumEylul == 0)
                            {
                                xKonumEylul += 30;
                            }
                            else
                            {
                                xKonumEylul += 150;
                            }
                            satirSayacEylul++;
                        }
                    }

                    if (performans.EkimPolicePrimi != null)
                    {
                        if (satirSayacEkim == 3)
                        {
                            satirSayacEkim = 1;
                            xKonumEkim = 30;
                            yKonumEkim += 20;
                        }
                        else
                        {
                            if (xKonumEkim == 0)
                            {
                                xKonumEkim += 30;
                            }
                            else
                            {
                                xKonumEkim += 150;
                            }
                            satirSayacEkim++;
                        }
                    }

                    if (performans.KasimPolicePrimi != null)
                    {
                        if (satirSayacKasim == 3)
                        {
                            satirSayacKasim = 1;
                            xKonumKasim = 30;
                            yKonumKasim += 20;
                        }
                        else
                        {
                            if (xKonumKasim == 0)
                            {
                                xKonumKasim += 30;
                            }
                            else
                            {
                                xKonumKasim += 150;
                            }
                            satirSayacKasim++;
                        }
                    }

                    if (performans.AralikPolicePrimi != null)
                    {
                        if (satirSayacAralik == 3)
                        {
                            satirSayacAralik = 1;
                            xKonumAralik = 30;
                            yKonumAralik += 20;
                        }
                        else
                        {
                            if (xKonumAralik == 0)
                            {
                                xKonumAralik += 30;
                            }
                            else
                            {
                                xKonumAralik += 150;
                            }
                            satirSayacAralik++;
                        }
                    }


                    #endregion

                    if (performans.OcakPolicePrimi != null)
                    {
                        #region Ocak Poliçe Primleri
                        ocakSayac++;
                        if (ocakSayac == 1)
                        {
                            scriptOcak.AppendLine("var chartOcakPrim = AmCharts.makeChart('ocak-chartdiv-prim', {");
                            scriptOcak.AppendLine("type: 'pie' ,");
                            scriptOcak.AppendLine("theme: 'light',");
                            scriptOcak.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.OcakPolicePrimi.HasValue)
                        {
                            if (performans.OcakPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.OcakPolicePrimi.Value).ToString("N").Split(',');
                                labelOcak += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumOcak + ", y :" + yKonumOcak + "},";
                            }
                            else
                            {
                                labelOcak += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumOcak + ", y :" + yKonumOcak + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.OcakPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.OcakPolicePrimi) + "},");
                            }
                        }

                        ocakToplam += performans.OcakPolicePrimi.Value;
                        #endregion
                    }

                    if (performans.SubatPolicePrimi != null)
                    {
                        #region Şubat Poliçe Primleri

                        subatSayac++;
                        if (subatSayac == 1)
                        {
                            scriptSubat.AppendLine("var chartSubatPrim = AmCharts.makeChart('subat-chartdiv-prim', {");
                            scriptSubat.AppendLine("type: 'pie' ,");
                            scriptSubat.AppendLine("theme: 'light',");
                            scriptSubat.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.SubatPolicePrimi.HasValue)
                        {
                            if (performans.SubatPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.SubatPolicePrimi.Value).ToString("N").Split(',');
                                labelSubat += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumSubat + ", y :" + yKonumSubat + "},";
                            }
                            else
                            {
                                labelSubat += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumSubat + ", y :" + yKonumSubat + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPolicePrimi) + "},");
                            }
                        }

                        subatToplam += performans.SubatPolicePrimi.Value;
                        #endregion
                    }

                    if (performans.MartPolicePrimi != null)
                    {
                        #region Mart Poliçe Primleri

                        martSayac++;
                        if (martSayac == 1)
                        {
                            scriptMart.AppendLine("var chartMartPrim = AmCharts.makeChart('mart-chartdiv-prim', {");
                            scriptMart.AppendLine("type: 'pie' ,");
                            scriptMart.AppendLine("theme: 'light',");
                            scriptMart.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MartPolicePrimi.HasValue)
                        {
                            if (performans.MartPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.MartPolicePrimi.Value).ToString("N").Split(',');
                                labelMart += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumMart + ", y :" + yKonumMart + "},";
                            }
                            else
                            {
                                labelMart += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumMart + ", y :" + yKonumMart + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPolicePrimi) + "},");
                            }
                        }
                        martToplam += performans.MartPolicePrimi.Value;

                        #endregion
                    }

                    if (performans.NisanPolicePrimi != null)
                    {
                        #region Nisan Poliçe Primleri

                        nisanSayac++;
                        if (nisanSayac == 1)
                        {
                            scriptNisan.AppendLine("var chartNisanPrim = AmCharts.makeChart('nisan-chartdiv-prim', {");
                            scriptNisan.AppendLine("type: 'pie' ,");
                            scriptNisan.AppendLine("theme: 'light',");
                            scriptNisan.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.NisanPolicePrimi.HasValue)
                        {
                            if (performans.NisanPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.NisanPolicePrimi.Value).ToString("N").Split(',');
                                labelNisan += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumNisan + ", y :" + yKonumNisan + "},";
                            }
                            else
                            {
                                labelNisan += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumNisan + ", y :" + yKonumNisan + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPolicePrimi) + "},");
                            }
                        }
                        nisanToplam += performans.NisanPolicePrimi.Value;

                        #endregion
                    }

                    if (performans.MayisPolicePrimi != null)
                    {
                        #region Mayis Poliçe Primleri

                        mayisSayac++;
                        if (mayisSayac == 1)
                        {
                            scriptMayis.AppendLine("var chartMayisPrim = AmCharts.makeChart('mayis-chartdiv-prim', {");
                            scriptMayis.AppendLine("type: 'pie' ,");
                            scriptMayis.AppendLine("theme: 'light',");
                            scriptMayis.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MayisPolicePrimi.HasValue)
                        {
                            if (performans.MayisPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.MayisPolicePrimi.Value).ToString("N").Split(',');
                                labelMayis += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumMayis + ", y :" + yKonumMayis + "},";
                            }
                            else
                            {
                                labelMayis += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumMayis + ", y :" + yKonumMayis + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPolicePrimi) + "},");
                            }
                        }
                        mayisToplam += performans.MayisPolicePrimi.Value;

                        #endregion
                    }

                    if (performans.HaziranPolicePrimi != null)
                    {
                        #region Haziran Poliçe Primleri

                        haziranSayac++;
                        if (haziranSayac == 1)
                        {
                            scriptHaziran.AppendLine("var chartHaziranPrim = AmCharts.makeChart('haziran-chartdiv-prim', {");
                            scriptHaziran.AppendLine("type: 'pie' ,");
                            scriptHaziran.AppendLine("theme: 'light',");
                            scriptHaziran.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.HaziranPolicePrimi.HasValue)
                        {
                            if (performans.HaziranPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.HaziranPolicePrimi.Value).ToString("N").Split(',');
                                labelHaziran += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                            }
                            else
                            {
                                labelHaziran += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPolicePrimi) + "},");
                            }
                        }
                        haziranToplam += performans.HaziranPolicePrimi.Value;

                        #endregion
                    }

                    if (performans.TemmuzPolicePrimi != null)
                    {
                        #region Temmuz Poliçe Primleri

                        temmuzSayac++;
                        if (temmuzSayac == 1)
                        {
                            scriptTemmuz.AppendLine("var chartTemmuzPrim = AmCharts.makeChart('temmuz-chartdiv-prim', {");
                            scriptTemmuz.AppendLine("type: 'pie' ,");
                            scriptTemmuz.AppendLine("theme: 'light',");
                            scriptTemmuz.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.TemmuzPolicePrimi.HasValue)
                        {
                            if (performans.TemmuzPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.TemmuzPolicePrimi.Value).ToString("N").Split(',');
                                labelTemmuz += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                            }
                            else
                            {
                                labelTemmuz += "{ text: '" + BransAdi + " 0 ', bold: true, x:" + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPolicePrimi) + "},");
                            }
                        }

                        temmuzToplam += performans.TemmuzPolicePrimi.Value;
                        #endregion
                    }

                    if (performans.AgustosPolicePrimi != null)
                    {
                        #region Ağustos Poliçe Primleri
                        agustosSayac++;
                        if (agustosSayac == 1)
                        {
                            scriptAgustos.AppendLine("var chartAgustosPrim = AmCharts.makeChart('agustos-chartdiv-prim', {");
                            scriptAgustos.AppendLine("type: 'pie' ,");
                            scriptAgustos.AppendLine("theme: 'light',");
                            scriptAgustos.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AgustosPolicePrimi.HasValue)
                        {
                            if (performans.AgustosPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.AgustosPolicePrimi.Value).ToString("N").Split(',');
                                labelAgustos += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAgustos + ", y :" + yKonumAgustos + "},";
                            }
                            else
                            {
                                labelAgustos += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumAgustos + ", y :" + yKonumAgustos + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AgustosPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AgustosPolicePrimi) + "},");
                            }
                        }
                        agustosToplam += performans.AgustosPolicePrimi.Value;
                        #endregion
                    }

                    if (performans.EylulPolicePrimi != null)
                    {
                        #region Eylul Poliçe Primleri
                        eylulSayac++;
                        if (eylulSayac == 1)
                        {
                            scriptEylul.AppendLine("var chartEylulPrim = AmCharts.makeChart('eylul-chartdiv-prim', {");
                            scriptEylul.AppendLine("type: 'pie' ,");
                            scriptEylul.AppendLine("theme: 'light',");
                            scriptEylul.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EylulPolicePrimi.HasValue)
                        {
                            if (performans.EylulPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.EylulPolicePrimi.Value).ToString("N").Split(',');
                                labelEylul += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEylul + ", y :" + yKonumEylul + "},";
                            }
                            else
                            {
                                labelEylul += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumEylul + ", y :" + yKonumEylul + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPolicePrimi) + "},");
                            }
                        }
                        eylulToplam += performans.EylulPolicePrimi.Value;
                        #endregion
                    }

                    if (performans.EkimPolicePrimi != null)
                    {
                        #region Ekim Poliçe Primleri
                        ekimSayac++;
                        if (ekimSayac == 1)
                        {
                            scriptEkim.AppendLine("var chartEkimPrim = AmCharts.makeChart('ekim-chartdiv-prim', {");
                            scriptEkim.AppendLine("type: 'pie' ,");
                            scriptEkim.AppendLine("theme: 'light',");
                            scriptEkim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EkimPolicePrimi.HasValue)
                        {
                            if (performans.EkimPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.EkimPolicePrimi.Value).ToString("N").Split(',');
                                labelEkim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                            }
                            else
                            {
                                labelEkim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPolicePrimi) + "},");
                            }
                        }
                        ekimToplam += performans.EkimPolicePrimi.Value;
                        #endregion
                    }

                    if (performans.KasimPolicePrimi != null)
                    {
                        #region Kasim Poliçe Primleri
                        kasimSayac++;
                        if (kasimSayac == 1)
                        {
                            scriptKasim.AppendLine("var chartKasimPrim = AmCharts.makeChart('kasim-chartdiv-prim', {");
                            scriptKasim.AppendLine("type: 'pie' ,");
                            scriptKasim.AppendLine("theme: 'light',");
                            scriptKasim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.KasimPolicePrimi.HasValue)
                        {
                            if (performans.KasimPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.KasimPolicePrimi.Value).ToString("N").Split(',');
                                labelKasim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                            }
                            else
                            {
                                labelKasim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPolicePrimi) + "},");
                            }
                        }
                        kasimToplam += performans.KasimPolicePrimi.Value;
                        #endregion
                    }

                    if (performans.AralikPolicePrimi != null)
                    {
                        #region Aralik Poliçe Primleri
                        aralikSayac++;
                        if (aralikSayac == 1)
                        {
                            scriptAralik.AppendLine("var chartAralikPrim = AmCharts.makeChart('aralik-chartdiv-prim', {");
                            scriptAralik.AppendLine("type: 'pie' ,");
                            scriptAralik.AppendLine("theme: 'light',");
                            scriptAralik.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AralikPolicePrimi.HasValue)
                        {
                            if (performans.AralikPolicePrimi != null)
                            {
                                string[] parts = Convert.ToInt32(performans.AralikPolicePrimi.Value).ToString("N").Split(',');
                                labelAralik += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                            }
                            else
                            {
                                labelAralik += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPolicePrimi) + "},");
                            }
                            else
                            {
                                scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPolicePrimi) + "},");
                            }
                        }
                        aralikToplam += performans.AralikPolicePrimi.Value;
                        #endregion
                    }

                }
                aylarToplam.policePrimOcak = ocakToplam;
                aylarToplam.policePrimSubat = subatToplam;
                aylarToplam.policePrimMart = martToplam;
                aylarToplam.policePrimNisan = nisanToplam;
                aylarToplam.policePrimMayis = mayisToplam;
                aylarToplam.policePrimHaziran = haziranToplam;
                aylarToplam.policePrimTemmuz = temmuzToplam;
                aylarToplam.policePrimAgustos = agustosToplam;
                aylarToplam.policePrimEylul = eylulToplam;
                aylarToplam.policePrimEkim = ekimToplam;
                aylarToplam.policePrimKasim = kasimToplam;
                aylarToplam.policePrimAralik = aralikToplam;


                #region Script Sonu

                if (ocakSayac > 0)
                {
                    string[] parts = ocakToplam.ToString("N").Split(',');
                    labelOcak += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumOcak + 35) + "},";

                    if (ocakToplam != 0 && ocakPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((ocakToplam / ocakPoliceAdetToplam), 2);
                        labelOcak += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumOcak + 55) + "},";
                    }

                    scriptOcak.AppendLine("],");
                    scriptOcak.AppendLine("valueField: 'value' ,");
                    scriptOcak.AppendLine("titleField:'brans',");
                    scriptOcak.AppendLine("outlineAlpha: 0.4,");
                    scriptOcak.AppendLine("depth3D: 15 ,");
                    scriptOcak.AppendLine("angle:30 ,");
                    scriptOcak.AppendLine("allLabels:[" + labelOcak + " ],");
                    scriptOcak.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                    scriptOcak.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptOcak;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptOcak.AppendLine("$('.ocak-prim-img').show();");
                    scriptGenel = scriptOcak;

                    result += scriptGenel.ToString();
                }
                if (subatSayac > 0)
                {
                    string[] parts = subatToplam.ToString("N").Split(',');
                    labelSubat += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumSubat + 35) + "},";

                    if (subatToplam != 0 && subatPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((subatToplam / subatPoliceAdetToplam), 2);
                        labelSubat += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumSubat + 55) + "},";
                    }

                    scriptSubat.AppendLine("],");
                    scriptSubat.AppendLine("valueField: 'value' ,");
                    scriptSubat.AppendLine("titleField:'brans',");
                    scriptSubat.AppendLine("outlineAlpha: 0.4,");
                    scriptSubat.AppendLine("depth3D: 15 ,");
                    scriptSubat.AppendLine("angle:30 ,");
                    scriptSubat.AppendLine("allLabels:[" + labelSubat + " ],");
                    scriptSubat.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptSubat.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptSubat;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptSubat.AppendLine("$('.subat-prim-img').show();");
                    scriptGenel = scriptSubat;

                    result += scriptGenel.ToString();
                }
                if (martSayac > 0)
                {
                    string[] parts = martToplam.ToString("N").Split(',');
                    labelMart += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMart + 35) + "},";

                    if (martToplam != 0 && martPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((martToplam / martPoliceAdetToplam), 2);
                        labelMart += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMart + 55) + "},";
                    }

                    scriptMart.AppendLine("],");
                    scriptMart.AppendLine("valueField: 'value' ,");
                    scriptMart.AppendLine("titleField:'brans',");
                    scriptMart.AppendLine("outlineAlpha: 0.4,");
                    scriptMart.AppendLine("depth3D: 15 ,");
                    scriptMart.AppendLine("angle:30 ,");
                    scriptMart.AppendLine("allLabels:[" + labelMart + " ],");
                    scriptMart.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptMart.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMart.AppendLine("$('.mart-prim-img').show();");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                if (nisanSayac > 0)
                {
                    string[] parts = nisanToplam.ToString("N").Split(',');
                    labelNisan += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumNisan + 35) + "},";

                    if (nisanToplam != 0 && nisanPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((nisanToplam / nisanPoliceAdetToplam), 2);
                        labelNisan += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumNisan + 55) + "},";
                    }

                    scriptNisan.AppendLine("],");
                    scriptNisan.AppendLine("valueField: 'value' ,");
                    scriptNisan.AppendLine("titleField:'brans',");
                    scriptNisan.AppendLine("outlineAlpha: 0.4,");
                    scriptNisan.AppendLine("depth3D: 15 ,");
                    scriptNisan.AppendLine("angle:30 ,");
                    scriptNisan.AppendLine("allLabels:[" + labelNisan + " ],");
                    scriptNisan.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                    scriptNisan.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptNisan.AppendLine("$('.nisan-prim-img').show();");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                if (mayisSayac > 0)
                {
                    string[] parts = mayisToplam.ToString("N").Split(',');
                    labelMayis += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMayis + 35) + "},";

                    if (mayisToplam != 0 && mayisPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((mayisToplam / mayisPoliceAdetToplam), 2);
                        labelMayis += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMayis + 55) + "},";
                    }

                    scriptMayis.AppendLine("],");
                    scriptMayis.AppendLine("valueField: 'value' ,");
                    scriptMayis.AppendLine("titleField:'brans',");
                    scriptMayis.AppendLine("outlineAlpha: 0.4,");
                    scriptMayis.AppendLine("depth3D: 15 ,");
                    scriptMayis.AppendLine("angle:30 ,");
                    scriptMayis.AppendLine("allLabels:[" + labelMayis + " ],");
                    scriptMayis.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptMayis.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMayis.AppendLine("$('.mayis-prim-img').show();");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                if (haziranSayac > 0)
                {
                    string[] parts = haziranToplam.ToString("N").Split(',');
                    labelHaziran += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumHaziran + 35) + "},";

                    if (haziranToplam != 0 && haziranPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((haziranToplam / haziranPoliceAdetToplam), 2);
                        labelHaziran += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumHaziran + 55) + "},";
                    }

                    scriptHaziran.AppendLine("],");
                    scriptHaziran.AppendLine("valueField: 'value' ,");
                    scriptHaziran.AppendLine("titleField:'brans',");
                    scriptHaziran.AppendLine("outlineAlpha: 0.4,");
                    scriptHaziran.AppendLine("depth3D: 15 ,");
                    scriptHaziran.AppendLine("angle:30 ,");
                    scriptHaziran.AppendLine("allLabels:[" + labelHaziran + " ],");
                    scriptHaziran.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptHaziran.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptHaziran.AppendLine("$('.haziran-prim-img').show();");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                if (temmuzSayac > 0)
                {
                    string[] parts = temmuzToplam.ToString("N").Split(',');
                    labelTemmuz += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumTemmuz + 35) + "},";

                    if (temmuzToplam != 0 && temmuzPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((temmuzToplam / temmuzPoliceAdetToplam), 2);
                        labelTemmuz += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumTemmuz + 55) + "},";
                    }

                    scriptTemmuz.AppendLine("],");
                    scriptTemmuz.AppendLine("valueField: 'value' ,");
                    scriptTemmuz.AppendLine("titleField:'brans',");
                    scriptTemmuz.AppendLine("outlineAlpha: 0.4,");
                    scriptTemmuz.AppendLine("depth3D: 15 ,");
                    scriptTemmuz.AppendLine("angle:30 ,");
                    scriptTemmuz.AppendLine("allLabels:[" + labelTemmuz + " ],");
                    scriptTemmuz.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptTemmuz.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptTemmuz;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptTemmuz.AppendLine("$('.temmuz-prim-img').show();");
                    scriptGenel = scriptTemmuz;

                    result += scriptGenel.ToString();
                }
                if (agustosSayac > 0)
                {
                    string[] parts = agustosToplam.ToString("N").Split(',');
                    labelAgustos += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAgustos + 35) + "},";

                    if (agustosToplam != 0 && agustosPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((agustosToplam / agustosPoliceAdetToplam), 2);
                        labelAgustos += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAgustos + 55) + "},";
                    }

                    scriptAgustos.AppendLine("],");
                    scriptAgustos.AppendLine("valueField: 'value' ,");
                    scriptAgustos.AppendLine("titleField:'brans',");
                    scriptAgustos.AppendLine("outlineAlpha: 0.4,");
                    scriptAgustos.AppendLine("depth3D: 15 ,");
                    scriptAgustos.AppendLine("angle:30 ,");
                    scriptAgustos.AppendLine("allLabels:[" + labelAgustos + " ],");
                    scriptAgustos.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptAgustos.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAgustos.AppendLine("$('.agustos-prim-img').show();");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                if (eylulSayac > 0)
                {
                    string[] parts = eylulToplam.ToString("N").Split(',');
                    labelEylul += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEylul + 35) + "},";

                    if (eylulToplam != 0 && eylulPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((eylulToplam / eylulPoliceAdetToplam), 2);
                        labelEylul += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEylul + 55) + "},";
                    }

                    scriptEylul.AppendLine("],");
                    scriptEylul.AppendLine("valueField: 'value' ,");
                    scriptEylul.AppendLine("titleField:'brans',");
                    scriptEylul.AppendLine("outlineAlpha: 0.4,");
                    scriptEylul.AppendLine("depth3D: 15 ,");
                    scriptEylul.AppendLine("angle:30 ,");
                    scriptEylul.AppendLine("allLabels:[" + labelEylul + " ],");
                    scriptEylul.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptEylul.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEylul.AppendLine("$('.eylul-prim-img').show();");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                if (ekimSayac > 0)
                {
                    string[] parts = ekimToplam.ToString("N").Split(',');
                    labelEkim += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEkim + 35) + "},";

                    if (eylulToplam != 0 && ekimPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((ekimToplam / ekimPoliceAdetToplam), 2);
                        labelEkim += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEkim + 55) + "},";
                    }

                    scriptEkim.AppendLine("],");
                    scriptEkim.AppendLine("valueField: 'value' ,");
                    scriptEkim.AppendLine("titleField:'brans',");
                    scriptEkim.AppendLine("outlineAlpha: 0.4,");
                    scriptEkim.AppendLine("depth3D: 15 ,");
                    scriptEkim.AppendLine("angle:30 ,");
                    scriptEkim.AppendLine("allLabels:[" + labelEkim + " ],");
                    scriptEkim.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptEkim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEkim.AppendLine("$('.ekim-prim-img').show();");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                if (kasimSayac > 0)
                {
                    string[] parts = kasimToplam.ToString("N").Split(',');
                    labelKasim += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumKasim + 35) + "},";

                    if (kasimToplam != 0 && kasimPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((kasimToplam / kasimPoliceAdetToplam), 2);
                        labelKasim += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumKasim + 55) + "},";
                    }

                    scriptKasim.AppendLine("],");
                    scriptKasim.AppendLine("valueField: 'value' ,");
                    scriptKasim.AppendLine("titleField:'brans',");
                    scriptKasim.AppendLine("outlineAlpha: 0.4,");
                    scriptKasim.AppendLine("depth3D: 15 ,");
                    scriptKasim.AppendLine("angle:30 ,");
                    scriptKasim.AppendLine("allLabels:[" + labelKasim + " ],");
                    scriptKasim.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptKasim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptKasim.AppendLine("$('.kasim-prim-img').show();");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                if (aralikSayac > 0)
                {
                    string[] parts = aralikToplam.ToString("N").Split(',');
                    labelAralik += "{ text: ' Toplam Poliçe Primi: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAralik + 35) + "},";

                    if (aralikToplam != 0 && aralikPoliceAdetToplam != 0)
                    {
                        decimal policeBasiPrim = Math.Round((aralikToplam / aralikPoliceAdetToplam), 2);
                        labelAralik += "{ text: 'Poliçe Başı Prim: " + policeBasiPrim + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAralik + 55) + "},";
                    }

                    scriptAralik.AppendLine("],");
                    scriptAralik.AppendLine("valueField: 'value' ,");
                    scriptAralik.AppendLine("titleField:'brans',");
                    scriptAralik.AppendLine("outlineAlpha: 0.4,");
                    scriptAralik.AppendLine("depth3D: 15 ,");
                    scriptAralik.AppendLine("angle:30 ,");
                    scriptAralik.AppendLine("allLabels:[" + labelAralik + " ],");
                    scriptAralik.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptAralik.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAralik.AppendLine("$('.aralik-prim-img').show();");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }
                #endregion

            }

            return result;
        }

        private string GetJScriptForPoliceKomisyon(OfflineUretimPerformansKullanici model)
        {
            string result = String.Empty;
            #region Array Lsit
            List<OfflineUretimPerformansKullaniciProcedureModel> performansList = new List<OfflineUretimPerformansKullaniciProcedureModel>();
            var uretimList = model.performansList;
            var bransSayisi = model.performansList.Select(s => s.branskodu).Distinct();
            var branslar = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            var SiraliBranslar = branslar.OrderByDescending(w => w.BransKodu).ToList();
            var BranskoduMax = SiraliBranslar.First().BransKodu;
            int[,] array = new int[BranskoduMax + 1, 13];
            if (uretimList != null)
            {
                for (int i = 0; i < uretimList.Count; i++)
                {
                    if (bransSayisi.Contains(uretimList[i].branskodu))
                    {
                        if (uretimList[i].OcakPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 1] += Convert.ToInt32(uretimList[i].OcakPoliceKomisyon.Value);
                        }
                        if (uretimList[i].SubatPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 2] += Convert.ToInt32(uretimList[i].SubatPoliceKomisyon.Value);
                        }
                        if (uretimList[i].MartPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 3] += Convert.ToInt32(uretimList[i].MartPoliceKomisyon.Value);
                        }
                        if (uretimList[i].NisanPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 4] += Convert.ToInt32(uretimList[i].NisanPoliceKomisyon.Value);
                        }
                        if (uretimList[i].MayisPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 5] += Convert.ToInt32(uretimList[i].MayisPoliceKomisyon.Value);
                        }
                        if (uretimList[i].HaziranPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 6] += Convert.ToInt32(uretimList[i].HaziranPoliceKomisyon.Value);
                        }
                        if (uretimList[i].TemmuzPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 7] += Convert.ToInt32(uretimList[i].TemmuzPoliceKomisyon.Value);
                        }
                        if (uretimList[i].AgustosPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 8] += Convert.ToInt32(uretimList[i].AgustosPoliceKomisyon.Value);
                        }
                        if (uretimList[i].EylulPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 9] += Convert.ToInt32(uretimList[i].EylulPoliceKomisyon.Value);
                        }
                        if (uretimList[i].EkimPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 10] += Convert.ToInt32(uretimList[i].EkimPoliceKomisyon.Value);
                        }
                        if (uretimList[i].KasimPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 11] += Convert.ToInt32(uretimList[i].KasimPoliceKomisyon.Value);
                        }
                        if (uretimList[i].AralikPoliceKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 12] += Convert.ToInt32(uretimList[i].AralikPoliceKomisyon.Value);
                        }
                    }

                }
            }

            OfflineUretimPerformansKullaniciProcedureModel item = new OfflineUretimPerformansKullaniciProcedureModel();
            if (array != null)
            {
                for (int i = 1; i < BranskoduMax + 1; i++)
                {
                    item = new OfflineUretimPerformansKullaniciProcedureModel();
                    for (int j = 1; j < 13; j++)
                    {
                        if (array[i, j] != 0 && array[i, j] != null)
                        {
                            if (j == 1) //Ocak
                            {
                                item.OcakPoliceKomisyon = array[i, j];
                            }
                            if (j == 2) //Şubat
                            {
                                item.SubatPoliceKomisyon = array[i, j];
                            }
                            if (j == 3) //Mart
                            {
                                item.MartPoliceKomisyon = array[i, j];
                            }
                            if (j == 4) //Nisan
                            {
                                item.NisanPoliceKomisyon = array[i, j];
                            }
                            if (j == 5) //Mayıs
                            {
                                item.MayisPoliceKomisyon = array[i, j];
                            }
                            if (j == 6) //Haziran
                            {
                                item.HaziranPoliceKomisyon = array[i, j];
                            }
                            if (j == 7) //Temmuz
                            {
                                item.TemmuzPoliceKomisyon = array[i, j];
                            }
                            if (j == 8) //Agustos
                            {
                                item.AgustosPoliceKomisyon = array[i, j];
                            }
                            if (j == 9) //Eylul
                            {
                                item.EylulPoliceKomisyon = array[i, j];
                            }
                            if (j == 10) //Ekim
                            {
                                item.EkimPoliceKomisyon = array[i, j];
                            }
                            if (j == 11) //Kasım
                            {
                                item.KasimPoliceKomisyon = array[i, j];
                            }
                            if (j == 12) //Aralık
                            {
                                item.AralikPoliceKomisyon = array[i, j];
                            }
                        }
                    }
                    item.branskodu = i;
                    performansList.Add(item);
                }
            }

            #endregion
            if (model != null)
            {
                #region Tanımlamalar

                StringBuilder teklifHelper = new StringBuilder();
                StringBuilder scriptGenel = new StringBuilder();
                int sayac = 0;
                int ocakSayac = 0;
                int subatSayac = 0;
                int martSayac = 0;
                int nisanSayac = 0;
                int mayisSayac = 0;
                int haziranSayac = 0;
                int temmuzSayac = 0;
                int agustosSayac = 0;
                int eylulSayac = 0;
                int ekimSayac = 0;
                int kasimSayac = 0;
                int aralikSayac = 0;

                var yKonumOcak = 500;
                var yKonumSubat = 500;
                var yKonumMart = 500;
                var yKonumNisan = 500;
                var yKonumMayis = 500;
                var yKonumHaziran = 500;
                var yKonumTemmuz = 500;
                var yKonumAgustos = 500;
                var yKonumEylul = 500;
                var yKonumEkim = 500;
                var yKonumKasim = 500;
                var yKonumAralik = 500;

                var xKonumOcak = 0;
                var xKonumSubat = 0;
                var xKonumMart = 0;
                var xKonumNisan = 0;
                var xKonumMayis = 0;
                var xKonumHaziran = 0;
                var xKonumTemmuz = 0;
                var xKonumAgustos = 0;
                var xKonumEylul = 0;
                var xKonumEkim = 0;
                var xKonumKasim = 0;
                var xKonumAralik = 0;

                int satirSayacOcak = 0;
                int satirSayacSubat = 0;
                int satirSayacMart = 0;
                int satirSayacNisan = 0;
                int satirSayacMayis = 0;
                int satirSayacHaziran = 0;
                int satirSayacTemmuz = 0;
                int satirSayacAgustos = 0;
                int satirSayacEylul = 0;
                int satirSayacEkim = 0;
                int satirSayacKasim = 0;
                int satirSayacAralik = 0;

                int TeklifSayisi = 0;
                TeklifSayisi = model.performansList.Count();

                string BransAdi = String.Empty;
                var bransListesi = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                StringBuilder scriptOcak = new StringBuilder();
                StringBuilder scriptSubat = new StringBuilder();
                StringBuilder scriptMart = new StringBuilder();
                StringBuilder scriptNisan = new StringBuilder();
                StringBuilder scriptMayis = new StringBuilder();
                StringBuilder scriptHaziran = new StringBuilder();
                StringBuilder scriptTemmuz = new StringBuilder();
                StringBuilder scriptAgustos = new StringBuilder();
                StringBuilder scriptEylul = new StringBuilder();
                StringBuilder scriptEkim = new StringBuilder();
                StringBuilder scriptKasim = new StringBuilder();
                StringBuilder scriptAralik = new StringBuilder();

                string labelOcak = String.Empty;
                string labelSubat = String.Empty;
                string labelMart = String.Empty;
                string labelNisan = String.Empty;
                string labelMayis = String.Empty;
                string labelHaziran = String.Empty;
                string labelTemmuz = String.Empty;
                string labelAgustos = String.Empty;
                string labelEylul = String.Empty;
                string labelEkim = String.Empty;
                string labelKasim = String.Empty;
                string labelAralik = String.Empty;

                scriptGenel.AppendLine("");

                //Aylık Toplam üretimleri göstermek için kullanılıyor
                decimal ocakToplam = 0;
                decimal subatToplam = 0;
                decimal martToplam = 0;
                decimal nisanToplam = 0;
                decimal mayisToplam = 0;
                decimal haziranToplam = 0;
                decimal temmuzToplam = 0;
                decimal agustosToplam = 0;
                decimal eylulToplam = 0;
                decimal ekimToplam = 0;
                decimal kasimToplam = 0;
                decimal aralikToplam = 0;
                #endregion

                foreach (var performans in performansList)
                {
                    #region Label text konum kontrol

                    if (performans.OcakPoliceKomisyon != null)
                    {
                        if (satirSayacOcak == 3)
                        {
                            satirSayacOcak = 1;
                            xKonumOcak = 30;
                            yKonumOcak += 20;
                        }
                        else
                        {
                            if (xKonumOcak == 0)
                            {
                                xKonumOcak += 30;
                            }
                            else
                            {
                                xKonumOcak += 150;
                            }
                            satirSayacOcak++;
                        }
                    }

                    if (performans.SubatPoliceKomisyon != null)
                    {
                        if (satirSayacSubat == 3)
                        {
                            satirSayacSubat = 1;
                            xKonumSubat = 30;
                            yKonumSubat += 20;
                        }
                        else
                        {
                            if (xKonumSubat == 0)
                            {
                                xKonumSubat += 30;
                            }
                            else
                            {
                                xKonumSubat += 150;
                            }
                            satirSayacSubat++;
                        }
                    }

                    if (performans.MartPoliceKomisyon != null)
                    {
                        if (satirSayacMart == 3)
                        {
                            satirSayacMart = 1;
                            xKonumMart = 30;
                            yKonumMart += 20;
                        }
                        else
                        {
                            if (xKonumMart == 0)
                            {
                                xKonumMart += 30;
                            }
                            else
                            {
                                xKonumMart += 150;
                            }
                            satirSayacMart++;
                        }
                    }
                    if (performans.NisanPoliceKomisyon != null)
                    {
                        if (satirSayacNisan == 3)
                        {
                            satirSayacNisan = 1;
                            xKonumNisan = 30;
                            yKonumNisan += 20;
                        }
                        else
                        {
                            if (xKonumNisan == 0)
                            {
                                xKonumNisan += 30;
                            }
                            else
                            {
                                xKonumNisan += 150;
                            }
                            satirSayacNisan++;
                        }
                    }

                    if (performans.MayisPoliceKomisyon != null)
                    {
                        if (satirSayacMayis == 3)
                        {
                            satirSayacMayis = 1;
                            xKonumMayis = 30;
                            yKonumMayis += 20;
                        }
                        else
                        {
                            if (xKonumMayis == 0)
                            {
                                xKonumMayis += 30;
                            }
                            else
                            {
                                xKonumMayis += 150;
                            }
                            satirSayacMayis++;
                        }
                    }

                    if (performans.HaziranPoliceKomisyon != null)
                    {
                        if (satirSayacHaziran == 3)
                        {
                            satirSayacHaziran = 1;
                            xKonumHaziran = 30;
                            yKonumHaziran += 20;
                        }
                        else
                        {
                            if (xKonumHaziran == 0)
                            {
                                xKonumHaziran += 30;
                            }
                            else
                            {
                                xKonumHaziran += 150;
                            }
                            satirSayacHaziran++;
                        }
                    }

                    if (performans.TemmuzPoliceKomisyon != null)
                    {
                        if (satirSayacTemmuz == 3)
                        {
                            satirSayacTemmuz = 1;
                            xKonumTemmuz = 30;
                            yKonumTemmuz += 20;
                        }
                        else
                        {
                            if (xKonumTemmuz == 0)
                            {
                                xKonumTemmuz += 30;
                            }
                            else
                            {
                                xKonumTemmuz += 150;
                            }
                            satirSayacTemmuz++;
                        }
                    }

                    if (performans.AgustosPoliceKomisyon != null)
                    {
                        if (satirSayacAgustos == 3)
                        {
                            satirSayacAgustos = 1;
                            xKonumAgustos = 30;
                            yKonumAgustos += 20;
                        }
                        else
                        {
                            if (xKonumAgustos == 0)
                            {
                                xKonumAgustos += 30;
                            }
                            else
                            {
                                xKonumAgustos += 150;
                            }
                            satirSayacAgustos++;
                        }
                    }


                    if (performans.EylulPoliceKomisyon != null)
                    {
                        if (satirSayacEylul == 3)
                        {
                            satirSayacEylul = 1;
                            xKonumEylul = 30;
                            yKonumEylul += 20;
                        }
                        else
                        {
                            if (xKonumEylul == 0)
                            {
                                xKonumEylul += 30;
                            }
                            else
                            {
                                xKonumEylul += 150;
                            }
                            satirSayacEylul++;
                        }
                    }

                    if (performans.EkimPoliceKomisyon != null)
                    {
                        if (satirSayacEkim == 3)
                        {
                            satirSayacEkim = 1;
                            xKonumEkim = 30;
                            yKonumEkim += 20;
                        }
                        else
                        {
                            if (xKonumEkim == 0)
                            {
                                xKonumEkim += 30;
                            }
                            else
                            {
                                xKonumEkim += 150;
                            }
                            satirSayacEkim++;
                        }
                    }

                    if (performans.KasimPoliceKomisyon != null)
                    {
                        if (satirSayacKasim == 3)
                        {
                            satirSayacKasim = 1;
                            xKonumKasim = 30;
                            yKonumKasim += 20;
                        }
                        else
                        {
                            if (xKonumKasim == 0)
                            {
                                xKonumKasim += 30;
                            }
                            else
                            {
                                xKonumKasim += 150;
                            }
                            satirSayacKasim++;
                        }
                    }

                    if (performans.AralikPoliceKomisyon != null)
                    {
                        if (satirSayacAralik == 3)
                        {
                            satirSayacAralik = 1;
                            xKonumAralik = 30;
                            yKonumAralik += 20;
                        }
                        else
                        {
                            if (xKonumAralik == 0)
                            {
                                xKonumAralik += 30;
                            }
                            else
                            {
                                xKonumAralik += 150;
                            }
                            satirSayacAralik++;
                        }
                    }


                    #endregion

                    if (performans.OcakPoliceKomisyon != null)
                    {
                        #region Ocak Poliçe Komisyonları
                        ocakSayac++;
                        if (ocakSayac == 1)
                        {
                            scriptOcak.AppendLine("var chartOcakKomisyon = AmCharts.makeChart('ocak-chartdiv-komisyon', {");
                            scriptOcak.AppendLine("type: 'pie' ,");
                            scriptOcak.AppendLine("theme: 'light',");
                            scriptOcak.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.OcakPoliceKomisyon.HasValue)
                        {
                            if (performans.OcakPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.OcakPoliceKomisyon.Value).ToString("N").Split(',');
                                labelOcak += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumOcak + ", y :" + yKonumOcak + "},";
                            }
                            else
                            {
                                labelOcak += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumOcak + ", y :" + yKonumOcak + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.OcakPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.OcakPoliceKomisyon) + "},");
                            }
                        }
                        ocakToplam += performans.OcakPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.SubatPoliceKomisyon != null)
                    {
                        #region Şubat Poliçe Komisyonları

                        subatSayac++;
                        if (subatSayac == 1)
                        {
                            scriptSubat.AppendLine("var chartSubatKomisyon = AmCharts.makeChart('subat-chartdiv-komisyon', {");
                            scriptSubat.AppendLine("type: 'pie' ,");
                            scriptSubat.AppendLine("theme: 'light',");
                            scriptSubat.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.SubatPoliceKomisyon.HasValue)
                        {
                            if (performans.SubatPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.SubatPoliceKomisyon.Value).ToString("N").Split(',');
                                labelSubat += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumSubat + ", y :" + yKonumSubat + "},";
                            }
                            else
                            {
                                labelSubat += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumSubat + ", y :" + yKonumSubat + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPoliceKomisyon) + "},");
                            }
                        }

                        subatToplam += performans.SubatPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.MartPoliceKomisyon != null)
                    {
                        #region Mart Poliçe Komisyonları
                        martSayac++;
                        if (martSayac == 1)
                        {
                            scriptMart.AppendLine("var chartMartKomisyon = AmCharts.makeChart('mart-chartdiv-komisyon', {");
                            scriptMart.AppendLine("type: 'pie' ,");
                            scriptMart.AppendLine("theme: 'light',");
                            scriptMart.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MartPoliceKomisyon.HasValue)
                        {
                            if (performans.MartPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.MartPoliceKomisyon.Value).ToString("N").Split(',');
                                labelMart += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumMart + ", y :" + yKonumMart + "},";
                            }
                            else
                            {
                                labelMart += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumMart + ", y :" + yKonumMart + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPoliceKomisyon) + "},");
                            }
                        }
                        martToplam += performans.MartPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.NisanPoliceKomisyon != null)
                    {
                        #region Nisan Poliçe Komisyonları
                        nisanSayac++;
                        if (nisanSayac == 1)
                        {
                            scriptNisan.AppendLine("var chartNisanKomisyon = AmCharts.makeChart('nisan-chartdiv-komisyon', {");
                            scriptNisan.AppendLine("type: 'pie' ,");
                            scriptNisan.AppendLine("theme: 'light',");
                            scriptNisan.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.NisanPoliceKomisyon.HasValue)
                        {
                            if (performans.NisanPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.NisanPoliceKomisyon.Value).ToString("N").Split(',');
                                labelNisan += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumNisan + ", y :" + yKonumNisan + "},";
                            }
                            else
                            {
                                labelNisan += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumNisan + ", y :" + yKonumNisan + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPoliceKomisyon) + "},");
                            }
                        }
                        nisanToplam += performans.NisanPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.MayisPoliceKomisyon != null)
                    {
                        #region Mayis Poliçe Komisyonları
                        mayisSayac++;
                        if (mayisSayac == 1)
                        {
                            scriptMayis.AppendLine("var chartMayisKomisyon = AmCharts.makeChart('mayis-chartdiv-komisyon', {");
                            scriptMayis.AppendLine("type: 'pie' ,");
                            scriptMayis.AppendLine("theme: 'light',");
                            scriptMayis.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MayisPoliceKomisyon.HasValue)
                        {
                            if (performans.MayisPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.MayisPoliceKomisyon.Value).ToString("N").Split(',');
                                labelMayis += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumMayis + ", y :" + yKonumMayis + "},";
                            }
                            else
                            {
                                labelMayis += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumMayis + ", y :" + yKonumMayis + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPoliceKomisyon) + "},");
                            }
                        }
                        mayisToplam += performans.MayisPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.HaziranPoliceKomisyon != null)
                    {
                        #region Haziran Poliçe Komisyonları

                        haziranSayac++;
                        if (haziranSayac == 1)
                        {
                            scriptHaziran.AppendLine("var chartHaziranKomisyon = AmCharts.makeChart('haziran-chartdiv-komisyon', {");
                            scriptHaziran.AppendLine("type: 'pie' ,");
                            scriptHaziran.AppendLine("theme: 'light',");
                            scriptHaziran.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.HaziranPoliceKomisyon.HasValue)
                        {
                            if (performans.HaziranPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.HaziranPoliceKomisyon.Value).ToString("N").Split(',');
                                labelHaziran += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                            }
                            else
                            {
                                labelHaziran += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPoliceKomisyon) + "},");
                            }
                        }
                        haziranToplam += performans.HaziranPoliceKomisyon.Value;

                        #endregion
                    }

                    if (performans.TemmuzPoliceKomisyon != null)
                    {
                        #region Temmuz Poliçe Komisyonları
                        temmuzSayac++;
                        if (temmuzSayac == 1)
                        {
                            scriptTemmuz.AppendLine("var chartTemmuzKomisyon = AmCharts.makeChart('temmuz-chartdiv-komisyon', {");
                            scriptTemmuz.AppendLine("type: 'pie' ,");
                            scriptTemmuz.AppendLine("theme: 'light',");
                            scriptTemmuz.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.TemmuzPoliceKomisyon.HasValue)
                        {
                            if (performans.TemmuzPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.TemmuzPoliceKomisyon.Value).ToString("N").Split(',');
                                labelTemmuz += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                            }
                            else
                            {
                                labelTemmuz += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPoliceKomisyon) + "},");
                            }
                        }
                        temmuzToplam += performans.TemmuzPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.AgustosPoliceKomisyon != null)
                    {
                        #region Ağustos Poliçe Komisyonları
                        agustosSayac++;
                        if (agustosSayac == 1)
                        {
                            scriptAgustos.AppendLine("var chartAgustosKomisyon = AmCharts.makeChart('agustos-chartdiv-komisyon', {");
                            scriptAgustos.AppendLine("type: 'pie' ,");
                            scriptAgustos.AppendLine("theme: 'light',");
                            scriptAgustos.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AgustosPoliceKomisyon.HasValue)
                        {
                            if (performans.AgustosPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.AgustosPoliceKomisyon.Value).ToString("N").Split(',');
                                labelAgustos += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAgustos + ", y :" + yKonumAgustos + "},";
                            }
                            else
                            {
                                labelAgustos += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumAgustos + ", y :" + yKonumAgustos + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AgustosPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AgustosPoliceKomisyon) + "},");
                            }
                        }
                        agustosToplam += performans.AgustosPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.EylulPoliceKomisyon != null)
                    {
                        #region Eylul Poliçe Komisyonları
                        eylulSayac++;
                        if (eylulSayac == 1)
                        {
                            scriptEylul.AppendLine("var chartEylulKomisyon = AmCharts.makeChart('eylul-chartdiv-komisyon', {");
                            scriptEylul.AppendLine("type: 'pie' ,");
                            scriptEylul.AppendLine("theme: 'light',");
                            scriptEylul.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EylulPoliceKomisyon.HasValue)
                        {
                            if (performans.EylulPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.EylulPoliceKomisyon.Value).ToString("N").Split(',');
                                labelEylul += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEylul + ", y :" + yKonumEylul + "},";
                            }
                            else
                            {
                                labelEylul += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumEylul + ", y :" + yKonumEylul + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPoliceKomisyon) + "},");
                            }
                        }
                        eylulToplam += performans.EylulPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.EkimPoliceKomisyon != null)
                    {
                        #region Ekim Poliçe Komisyonları
                        ekimSayac++;
                        if (ekimSayac == 1)
                        {
                            scriptEkim.AppendLine("var chartEkimKomisyon = AmCharts.makeChart('ekim-chartdiv-komisyon', {");
                            scriptEkim.AppendLine("type: 'pie' ,");
                            scriptEkim.AppendLine("theme: 'light',");
                            scriptEkim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EkimPoliceKomisyon.HasValue)
                        {
                            if (performans.EkimPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.EkimPoliceKomisyon.Value).ToString("N").Split(',');
                                labelEkim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                            }
                            else
                            {
                                labelEkim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPoliceKomisyon) + "},");
                            }
                        }
                        ekimToplam += performans.EkimPoliceKomisyon.Value;
                        #endregion
                    }

                    if (performans.KasimPoliceKomisyon != null)
                    {
                        #region Kasım Poliçe Komisyonları

                        kasimSayac++;
                        if (kasimSayac == 1)
                        {
                            scriptKasim.AppendLine("var chartKasimKomisyon = AmCharts.makeChart('kasim-chartdiv-komisyon', {");
                            scriptKasim.AppendLine("type: 'pie' ,");
                            scriptKasim.AppendLine("theme: 'light',");
                            scriptKasim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.KasimPoliceKomisyon.HasValue)
                        {
                            if (performans.KasimPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.KasimPoliceKomisyon.Value).ToString("N").Split(',');
                                labelKasim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                            }
                            else
                            {
                                labelKasim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPoliceKomisyon) + "},");
                            }
                        }
                        kasimToplam += performans.KasimPoliceKomisyon.Value;

                        #endregion
                    }

                    if (performans.AralikPoliceKomisyon != null)
                    {
                        #region Aralik Poliçe Komisyonları
                        aralikSayac++;
                        if (aralikSayac == 1)
                        {
                            scriptAralik.AppendLine("var chartAralikKomisyon = AmCharts.makeChart('aralik-chartdiv-komisyon', {");
                            scriptAralik.AppendLine("type: 'pie' ,");
                            scriptAralik.AppendLine("theme: 'light',");
                            scriptAralik.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AralikPoliceKomisyon.HasValue)
                        {
                            if (performans.AralikPoliceKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.AralikPoliceKomisyon.Value).ToString("N").Split(',');
                                labelAralik += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                            }
                            else
                            {
                                labelAralik += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPoliceKomisyon) + "},");
                            }
                            else
                            {
                                scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPoliceKomisyon) + "},");
                            }
                        }
                        aralikToplam += performans.AralikPoliceKomisyon.Value;
                        #endregion
                    }
                }
                aylarToplam.policeKomisyonOcak = ocakToplam;
                aylarToplam.policeKomisyonSubat = subatToplam;
                aylarToplam.policeKomisyonMart = martToplam;
                aylarToplam.policeKomisyonNisan = nisanToplam;
                aylarToplam.policeKomisyonMayis = mayisToplam;
                aylarToplam.policeKomisyonHaziran = haziranToplam;
                aylarToplam.policeKomisyonTemmuz = temmuzToplam;
                aylarToplam.policeKomisyonAgustos = agustosToplam;
                aylarToplam.policeKomisyonEylul = eylulToplam;
                aylarToplam.policeKomisyonEkim = ekimToplam;
                aylarToplam.policeKomisyonKasim = kasimToplam;
                aylarToplam.policeKomisyonAralik = aralikToplam;
                #region Script Sonu

                if (ocakSayac > 0)
                {
                    string[] parts = ocakToplam.ToString("N").Split(',');
                    labelOcak += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumOcak + 35) + "},";

                    if (ocakToplam != 0 && ocakPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((ocakToplam / ocakPoliceAdetToplam), 2);
                        labelOcak += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumOcak + 55) + "},";
                    }

                    scriptOcak.AppendLine("],");
                    scriptOcak.AppendLine("valueField: 'value' ,");
                    scriptOcak.AppendLine("titleField:'brans',");
                    scriptOcak.AppendLine("outlineAlpha: 0.4,");
                    scriptOcak.AppendLine("depth3D: 15 ,");
                    scriptOcak.AppendLine("angle:30 ,");
                    scriptOcak.AppendLine("allLabels:[" + labelOcak + " ],");
                    scriptOcak.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                    scriptOcak.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptOcak;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptOcak.AppendLine("$('.ocak-komisyon-img').show();");
                    scriptGenel = scriptOcak;

                    result += scriptGenel.ToString();
                }
                if (subatSayac > 0)
                {
                    string[] parts = subatToplam.ToString("N").Split(',');
                    labelSubat += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumSubat + 35) + "},";

                    if (subatToplam != 0 && subatPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((subatToplam / subatPoliceAdetToplam), 2);
                        labelSubat += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumSubat + 55) + "},";
                    }

                    scriptSubat.AppendLine("],");
                    scriptSubat.AppendLine("valueField: 'value' ,");
                    scriptSubat.AppendLine("titleField:'brans',");
                    scriptSubat.AppendLine("outlineAlpha: 0.4,");
                    scriptSubat.AppendLine("depth3D: 15 ,");
                    scriptSubat.AppendLine("angle:30 ,");
                    scriptSubat.AppendLine("allLabels:[" + labelSubat + " ],");
                    scriptSubat.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptSubat.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptSubat;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptSubat.AppendLine("$('.subat-komisyon-img').show();");
                    scriptGenel = scriptSubat;

                    result += scriptGenel.ToString();
                }
                if (martSayac > 0)
                {
                    string[] parts = martToplam.ToString("N").Split(',');
                    labelMart += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMart + 35) + "},";

                    if (martToplam != 0 && martPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((martToplam / martPoliceAdetToplam), 2);
                        labelMart += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMart + 55) + "},";
                    }

                    scriptMart.AppendLine("],");
                    scriptMart.AppendLine("valueField: 'value' ,");
                    scriptMart.AppendLine("titleField:'brans',");
                    scriptMart.AppendLine("outlineAlpha: 0.4,");
                    scriptMart.AppendLine("depth3D: 15 ,");
                    scriptMart.AppendLine("angle:30 ,");
                    scriptMart.AppendLine("allLabels:[" + labelMart + " ],");
                    scriptMart.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptMart.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMart.AppendLine("$('.mart-komisyon-img').show();");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                if (nisanSayac > 0)
                {
                    string[] parts = nisanToplam.ToString("N").Split(',');
                    labelNisan += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumNisan + 35) + "},";

                    if (nisanToplam != 0 && nisanPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((nisanToplam / nisanPoliceAdetToplam), 2);
                        labelNisan += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumNisan + 55) + "},";
                    }

                    scriptNisan.AppendLine("],");
                    scriptNisan.AppendLine("valueField: 'value' ,");
                    scriptNisan.AppendLine("titleField:'brans',");
                    scriptNisan.AppendLine("outlineAlpha: 0.4,");
                    scriptNisan.AppendLine("depth3D: 15 ,");
                    scriptNisan.AppendLine("angle:30 ,");
                    scriptNisan.AppendLine("allLabels:[" + labelNisan + " ],");
                    scriptNisan.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                    scriptNisan.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptNisan.AppendLine("$('.nisan-komisyon-img').show();");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                if (mayisSayac > 0)
                {
                    string[] parts = mayisToplam.ToString("N").Split(',');
                    labelMayis += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMayis + 35) + "},";

                    if (mayisToplam != 0 && mayisPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((mayisToplam / mayisPoliceAdetToplam), 2);
                        labelMayis += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMayis + 55) + "},";
                    }

                    scriptMayis.AppendLine("],");
                    scriptMayis.AppendLine("valueField: 'value' ,");
                    scriptMayis.AppendLine("titleField:'brans',");
                    scriptMayis.AppendLine("outlineAlpha: 0.4,");
                    scriptMayis.AppendLine("depth3D: 15 ,");
                    scriptMayis.AppendLine("angle:30 ,");
                    scriptMayis.AppendLine("allLabels:[" + labelMayis + " ],");
                    scriptMayis.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptMayis.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMayis.AppendLine("$('.mayis-komisyon-img').show();");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                if (haziranSayac > 0)
                {
                    string[] parts = haziranToplam.ToString("N").Split(',');
                    labelHaziran += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumHaziran + 35) + "},";

                    if (haziranToplam != 0 && haziranPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((haziranToplam / haziranPoliceAdetToplam), 2);
                        labelHaziran += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumHaziran + 55) + "},";
                    }

                    scriptHaziran.AppendLine("],");
                    scriptHaziran.AppendLine("valueField: 'value' ,");
                    scriptHaziran.AppendLine("titleField:'brans',");
                    scriptHaziran.AppendLine("outlineAlpha: 0.4,");
                    scriptHaziran.AppendLine("depth3D: 15 ,");
                    scriptHaziran.AppendLine("angle:30 ,");
                    scriptHaziran.AppendLine("allLabels:[" + labelHaziran + " ],");
                    scriptHaziran.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptHaziran.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptHaziran.AppendLine("$('.haziran-komisyon-img').show();");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                if (temmuzSayac > 0)
                {
                    string[] parts = temmuzToplam.ToString("N").Split(',');
                    labelTemmuz += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumTemmuz + 35) + "},";

                    if (temmuzToplam != 0 && temmuzPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((temmuzToplam / temmuzPoliceAdetToplam), 2);
                        labelTemmuz += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumTemmuz + 55) + "},";
                    }

                    scriptTemmuz.AppendLine("],");
                    scriptTemmuz.AppendLine("valueField: 'value' ,");
                    scriptTemmuz.AppendLine("titleField:'brans',");
                    scriptTemmuz.AppendLine("outlineAlpha: 0.4,");
                    scriptTemmuz.AppendLine("depth3D: 15 ,");
                    scriptTemmuz.AppendLine("angle:30 ,");
                    scriptTemmuz.AppendLine("allLabels:[" + labelTemmuz + " ],");
                    scriptTemmuz.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptTemmuz.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptTemmuz;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptTemmuz.AppendLine("$('.temmuz-komisyon-img').show();");
                    scriptGenel = scriptTemmuz;

                    result += scriptGenel.ToString();
                }
                if (agustosSayac > 0)
                {
                    string[] parts = agustosToplam.ToString("N").Split(',');
                    labelAgustos += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAgustos + 35) + "},";

                    if (agustosToplam != 0 && agustosPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((agustosToplam / agustosPoliceAdetToplam), 2);
                        labelAgustos += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAgustos + 55) + "},";
                    }

                    scriptAgustos.AppendLine("],");
                    scriptAgustos.AppendLine("valueField: 'value' ,");
                    scriptAgustos.AppendLine("titleField:'brans',");
                    scriptAgustos.AppendLine("outlineAlpha: 0.4,");
                    scriptAgustos.AppendLine("depth3D: 15 ,");
                    scriptAgustos.AppendLine("angle:30 ,");
                    scriptAgustos.AppendLine("allLabels:[" + labelAgustos + " ],");
                    scriptAgustos.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptAgustos.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAgustos.AppendLine("$('.agustos-komisyon-img').show();");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                if (eylulSayac > 0)
                {
                    string[] parts = eylulToplam.ToString("N").Split(',');
                    labelEylul += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEylul + 35) + "},";

                    if (eylulToplam != 0 && eylulPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((eylulToplam / eylulPoliceAdetToplam), 2);
                        labelEylul += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEylul + 55) + "},";
                    }

                    scriptEylul.AppendLine("],");
                    scriptEylul.AppendLine("valueField: 'value' ,");
                    scriptEylul.AppendLine("titleField:'brans',");
                    scriptEylul.AppendLine("outlineAlpha: 0.4,");
                    scriptEylul.AppendLine("depth3D: 15 ,");
                    scriptEylul.AppendLine("angle:30 ,");
                    scriptEylul.AppendLine("allLabels:[" + labelEylul + " ],");
                    scriptEylul.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptEylul.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEylul.AppendLine("$('.eylul-komisyon-img').show();");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                if (ekimSayac > 0)
                {
                    string[] parts = ekimToplam.ToString("N").Split(',');
                    labelEkim += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEkim + 35) + "},";

                    if (ekimToplam != 0 && ekimPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((ekimToplam / ekimPoliceAdetToplam), 2);
                        labelEkim += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEkim + 55) + "},";
                    }

                    scriptEkim.AppendLine("],");
                    scriptEkim.AppendLine("valueField: 'value' ,");
                    scriptEkim.AppendLine("titleField:'brans',");
                    scriptEkim.AppendLine("outlineAlpha: 0.4,");
                    scriptEkim.AppendLine("depth3D: 15 ,");
                    scriptEkim.AppendLine("angle:30 ,");
                    scriptEkim.AppendLine("allLabels:[" + labelEkim + " ],");
                    scriptEkim.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptEkim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEkim.AppendLine("$('.ekim-komisyon-img').show();");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                if (kasimSayac > 0)
                {
                    string[] parts = kasimToplam.ToString("N").Split(',');
                    labelKasim += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumKasim + 35) + "},";

                    if (kasimToplam != 0 && kasimPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((kasimToplam / kasimPoliceAdetToplam), 2);
                        labelKasim += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumKasim + 55) + "},";
                    }

                    scriptKasim.AppendLine("],");
                    scriptKasim.AppendLine("valueField: 'value' ,");
                    scriptKasim.AppendLine("titleField:'brans',");
                    scriptKasim.AppendLine("outlineAlpha: 0.4,");
                    scriptKasim.AppendLine("depth3D: 15 ,");
                    scriptKasim.AppendLine("angle:30 ,");
                    scriptKasim.AppendLine("allLabels:[" + labelKasim + " ],");
                    scriptKasim.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptKasim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptKasim.AppendLine("$('.kasim-komisyon-img').show();");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                if (aralikSayac > 0)
                {
                    string[] parts = aralikToplam.ToString("N").Split(',');
                    labelAralik += "{ text: ' Toplam Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAralik + 35) + "},";

                    if (aralikToplam != 0 && aralikPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((aralikToplam / aralikPoliceAdetToplam), 2);
                        labelAralik += "{ text: 'Poliçe Başı Komisyon: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAralik + 55) + "},";
                    }

                    scriptAralik.AppendLine("],");
                    scriptAralik.AppendLine("valueField: 'value' ,");
                    scriptAralik.AppendLine("titleField:'brans',");
                    scriptAralik.AppendLine("outlineAlpha: 0.4,");
                    scriptAralik.AppendLine("depth3D: 15 ,");
                    scriptAralik.AppendLine("angle:30 ,");
                    scriptAralik.AppendLine("allLabels:[" + labelAralik + " ],");
                    scriptAralik.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptAralik.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAralik.AppendLine("$('.aralik-komisyon-img').show();");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }

                #endregion

            }

            return result;
        }

        private string GetJScriptForPoliceVerilenKomisyon(OfflineUretimPerformansKullanici model)
        {
            string result = String.Empty;
            #region Array Lsit

            List<OfflineUretimPerformansKullaniciProcedureModel> performansList = new List<OfflineUretimPerformansKullaniciProcedureModel>();
            var uretimList = model.performansList;
            var bransSayisi = model.performansList.Select(s => s.branskodu).Distinct();
            var branslar = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
            var SiraliBranslar = branslar.OrderByDescending(w => w.BransKodu).ToList();
            var BranskoduMax = SiraliBranslar.First().BransKodu;
            int[,] array = new int[BranskoduMax + 1, 13];
            if (uretimList != null)
            {
                for (int i = 0; i < uretimList.Count; i++)
                {
                    if (bransSayisi.Contains(uretimList[i].branskodu))
                    {
                        if (uretimList[i].OcakPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 1] += Convert.ToInt32(uretimList[i].OcakPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].SubatPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 2] += Convert.ToInt32(uretimList[i].SubatPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].MartPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 3] += Convert.ToInt32(uretimList[i].MartPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].NisanPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 4] += Convert.ToInt32(uretimList[i].NisanPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].MayisPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 5] += Convert.ToInt32(uretimList[i].MayisPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].HaziranPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 6] += Convert.ToInt32(uretimList[i].HaziranPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].TemmuzPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 7] += Convert.ToInt32(uretimList[i].TemmuzPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].AgustosPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 8] += Convert.ToInt32(uretimList[i].AgustosPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].EylulPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 9] += Convert.ToInt32(uretimList[i].EylulPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].EkimPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 10] += Convert.ToInt32(uretimList[i].EkimPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].KasimPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 11] += Convert.ToInt32(uretimList[i].KasimPoliceVerilenKomisyon.Value);
                        }
                        if (uretimList[i].AralikPoliceVerilenKomisyon != null)
                        {
                            array[uretimList[i].branskodu, 12] += Convert.ToInt32(uretimList[i].AralikPoliceVerilenKomisyon.Value);
                        }
                    }

                }
            }


            OfflineUretimPerformansKullaniciProcedureModel item = new OfflineUretimPerformansKullaniciProcedureModel();
            if (array != null)
            {
                for (int i = 1; i < BranskoduMax + 1; i++)
                {
                    item = new OfflineUretimPerformansKullaniciProcedureModel();
                    for (int j = 1; j < 13; j++)
                    {
                        if (array[i, j] != 0 && array[i, j] != null)
                        {
                            if (j == 1) //Ocak
                            {
                                item.OcakPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 2) //Şubat
                            {
                                item.SubatPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 3) //Mart
                            {
                                item.MartPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 4) //Nisan
                            {
                                item.NisanPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 5) //Mayıs
                            {
                                item.MayisPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 6) //Haziran
                            {
                                item.HaziranPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 7) //Temmuz
                            {
                                item.TemmuzPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 8) //Agustos
                            {
                                item.AgustosPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 9) //Eylul
                            {
                                item.EylulPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 10) //Ekim
                            {
                                item.EkimPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 11) //Kasım
                            {
                                item.KasimPoliceVerilenKomisyon = array[i, j];
                            }
                            if (j == 12) //Aralık
                            {
                                item.AralikPoliceVerilenKomisyon = array[i, j];
                            }
                        }
                    }
                    item.branskodu = i;
                    performansList.Add(item);
                }
            }

            #endregion
            if (model != null)
            {
                #region Tanımlamalar

                StringBuilder teklifHelper = new StringBuilder();
                StringBuilder scriptGenel = new StringBuilder();
                int sayac = 0;
                int ocakSayac = 0;
                int subatSayac = 0;
                int martSayac = 0;
                int nisanSayac = 0;
                int mayisSayac = 0;
                int haziranSayac = 0;
                int temmuzSayac = 0;
                int agustosSayac = 0;
                int eylulSayac = 0;
                int ekimSayac = 0;
                int kasimSayac = 0;
                int aralikSayac = 0;

                var yKonumOcak = 500;
                var yKonumSubat = 500;
                var yKonumMart = 500;
                var yKonumNisan = 500;
                var yKonumMayis = 500;
                var yKonumHaziran = 500;
                var yKonumTemmuz = 500;
                var yKonumAgustos = 500;
                var yKonumEylul = 500;
                var yKonumEkim = 500;
                var yKonumKasim = 500;
                var yKonumAralik = 500;

                var xKonumOcak = 0;
                var xKonumSubat = 0;
                var xKonumMart = 0;
                var xKonumNisan = 0;
                var xKonumMayis = 0;
                var xKonumHaziran = 0;
                var xKonumTemmuz = 0;
                var xKonumAgustos = 0;
                var xKonumEylul = 0;
                var xKonumEkim = 0;
                var xKonumKasim = 0;
                var xKonumAralik = 0;

                int satirSayacOcak = 0;
                int satirSayacSubat = 0;
                int satirSayacMart = 0;
                int satirSayacNisan = 0;
                int satirSayacMayis = 0;
                int satirSayacHaziran = 0;
                int satirSayacTemmuz = 0;
                int satirSayacAgustos = 0;
                int satirSayacEylul = 0;
                int satirSayacEkim = 0;
                int satirSayacKasim = 0;
                int satirSayacAralik = 0;

                int TeklifSayisi = 0;
                TeklifSayisi = model.performansList.Count();

                string BransAdi = String.Empty;
                var bransListesi = _BransService.GetList(_AktifKullanici.TvmTipi.ToString());
                StringBuilder scriptOcak = new StringBuilder();
                StringBuilder scriptSubat = new StringBuilder();
                StringBuilder scriptMart = new StringBuilder();
                StringBuilder scriptNisan = new StringBuilder();
                StringBuilder scriptMayis = new StringBuilder();
                StringBuilder scriptHaziran = new StringBuilder();
                StringBuilder scriptTemmuz = new StringBuilder();
                StringBuilder scriptAgustos = new StringBuilder();
                StringBuilder scriptEylul = new StringBuilder();
                StringBuilder scriptEkim = new StringBuilder();
                StringBuilder scriptKasim = new StringBuilder();
                StringBuilder scriptAralik = new StringBuilder();

                string labelOcak = String.Empty;
                string labelSubat = String.Empty;
                string labelMart = String.Empty;
                string labelNisan = String.Empty;
                string labelMayis = String.Empty;
                string labelHaziran = String.Empty;
                string labelTemmuz = String.Empty;
                string labelAgustos = String.Empty;
                string labelEylul = String.Empty;
                string labelEkim = String.Empty;
                string labelKasim = String.Empty;
                string labelAralik = String.Empty;

                scriptGenel.AppendLine("");

                //Aylık Toplam üretimleri göstermek için kullanılıyor
                decimal ocakToplam = 0;
                decimal subatToplam = 0;
                decimal martToplam = 0;
                decimal nisanToplam = 0;
                decimal mayisToplam = 0;
                decimal haziranToplam = 0;
                decimal temmuzToplam = 0;
                decimal agustosToplam = 0;
                decimal eylulToplam = 0;
                decimal ekimToplam = 0;
                decimal kasimToplam = 0;
                decimal aralikToplam = 0;

                #endregion
                foreach (var performans in performansList)
                {
                    #region Label text konum kontrol

                    if (performans.OcakPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacOcak == 3)
                        {
                            satirSayacOcak = 1;
                            xKonumOcak = 30;
                            yKonumOcak += 20;
                        }
                        else
                        {
                            if (xKonumOcak == 0)
                            {
                                xKonumOcak += 30;
                            }
                            else
                            {
                                xKonumOcak += 150;
                            }
                            satirSayacOcak++;
                        }
                    }

                    if (performans.SubatPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacSubat == 3)
                        {
                            satirSayacSubat = 1;
                            xKonumSubat = 30;
                            yKonumSubat += 20;
                        }
                        else
                        {
                            if (xKonumSubat == 0)
                            {
                                xKonumSubat += 30;
                            }
                            else
                            {
                                xKonumSubat += 150;
                            }
                            satirSayacSubat++;
                        }
                    }

                    if (performans.MartPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacMart == 3)
                        {
                            satirSayacMart = 1;
                            xKonumMart = 30;
                            yKonumMart += 20;
                        }
                        else
                        {
                            if (xKonumMart == 0)
                            {
                                xKonumMart += 30;
                            }
                            else
                            {
                                xKonumMart += 150;
                            }
                            satirSayacMart++;
                        }
                    }
                    if (performans.NisanPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacNisan == 3)
                        {
                            satirSayacNisan = 1;
                            xKonumNisan = 30;
                            yKonumNisan += 20;
                        }
                        else
                        {
                            if (xKonumNisan == 0)
                            {
                                xKonumNisan += 30;
                            }
                            else
                            {
                                xKonumNisan += 150;
                            }
                            satirSayacNisan++;
                        }
                    }

                    if (performans.MayisPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacMayis == 3)
                        {
                            satirSayacMayis = 1;
                            xKonumMayis = 30;
                            yKonumMayis += 20;
                        }
                        else
                        {
                            if (xKonumMayis == 0)
                            {
                                xKonumMayis += 30;
                            }
                            else
                            {
                                xKonumMayis += 150;
                            }
                            satirSayacMayis++;
                        }
                    }

                    if (performans.HaziranPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacHaziran == 3)
                        {
                            satirSayacHaziran = 1;
                            xKonumHaziran = 30;
                            yKonumHaziran += 20;
                        }
                        else
                        {
                            if (xKonumHaziran == 0)
                            {
                                xKonumHaziran += 30;
                            }
                            else
                            {
                                xKonumHaziran += 150;
                            }
                            satirSayacHaziran++;
                        }
                    }

                    if (performans.TemmuzPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacTemmuz == 3)
                        {
                            satirSayacTemmuz = 1;
                            xKonumTemmuz = 30;
                            yKonumTemmuz += 20;
                        }
                        else
                        {
                            if (xKonumTemmuz == 0)
                            {
                                xKonumTemmuz += 30;
                            }
                            else
                            {
                                xKonumTemmuz += 150;
                            }
                            satirSayacTemmuz++;
                        }
                    }

                    if (performans.AgustosPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacAgustos == 3)
                        {
                            satirSayacAgustos = 1;
                            xKonumAgustos = 30;
                            yKonumAgustos += 20;
                        }
                        else
                        {
                            if (xKonumAgustos == 0)
                            {
                                xKonumAgustos += 30;
                            }
                            else
                            {
                                xKonumAgustos += 150;
                            }
                            satirSayacAgustos++;
                        }
                    }


                    if (performans.EylulPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacEylul == 3)
                        {
                            satirSayacEylul = 1;
                            xKonumEylul = 30;
                            yKonumEylul += 20;
                        }
                        else
                        {
                            if (xKonumEylul == 0)
                            {
                                xKonumEylul += 30;
                            }
                            else
                            {
                                xKonumEylul += 150;
                            }
                            satirSayacEylul++;
                        }
                    }

                    if (performans.EkimPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacEkim == 3)
                        {
                            satirSayacEkim = 1;
                            xKonumEkim = 30;
                            yKonumEkim += 20;
                        }
                        else
                        {
                            if (xKonumEkim == 0)
                            {
                                xKonumEkim += 30;
                            }
                            else
                            {
                                xKonumEkim += 150;
                            }
                            satirSayacEkim++;
                        }
                    }

                    if (performans.KasimPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacKasim == 3)
                        {
                            satirSayacKasim = 1;
                            xKonumKasim = 30;
                            yKonumKasim += 20;
                        }
                        else
                        {
                            if (xKonumKasim == 0)
                            {
                                xKonumKasim += 30;
                            }
                            else
                            {
                                xKonumKasim += 150;
                            }
                            satirSayacKasim++;
                        }
                    }

                    if (performans.AralikPoliceVerilenKomisyon != null)
                    {
                        if (satirSayacAralik == 3)
                        {
                            satirSayacAralik = 1;
                            xKonumAralik = 30;
                            yKonumAralik += 20;
                        }
                        else
                        {
                            if (xKonumAralik == 0)
                            {
                                xKonumAralik += 30;
                            }
                            else
                            {
                                xKonumAralik += 150;
                            }
                            satirSayacAralik++;
                        }
                    }


                    #endregion

                    if (performans.OcakPoliceVerilenKomisyon != null)
                    {
                        #region Ocak Poliçe Verilen Komisyonları
                        ocakSayac++;
                        if (ocakSayac == 1)
                        {
                            scriptOcak.AppendLine("var chartOcakVerilenKomisyon = AmCharts.makeChart('ocak-chartdiv-verilenkomisyon', {");
                            scriptOcak.AppendLine("type: 'pie' ,");
                            scriptOcak.AppendLine("theme: 'light',");
                            scriptOcak.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.OcakPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.OcakPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.OcakPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelOcak += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumOcak + ", y :" + yKonumOcak + "},";
                            }
                            else
                            {
                                labelOcak += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumOcak + ", y :" + yKonumOcak + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.OcakPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptOcak.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.OcakPoliceVerilenKomisyon) + "},");
                            }
                        }
                        ocakToplam += performans.OcakPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.SubatPoliceVerilenKomisyon != null)
                    {
                        #region Şubat Poliçe Verilen Komisyonları

                        subatSayac++;
                        if (subatSayac == 1)
                        {
                            scriptSubat.AppendLine("var chartSubatVerilenKomisyon = AmCharts.makeChart('subat-chartdiv-verilenkomisyon', {");
                            scriptSubat.AppendLine("type: 'pie' ,");
                            scriptSubat.AppendLine("theme: 'light',");
                            scriptSubat.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.SubatPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.SubatPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.SubatPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelSubat += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumSubat + ", y :" + yKonumSubat + "},";
                            }
                            else
                            {
                                labelSubat += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumSubat + ", y :" + yKonumSubat + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptSubat.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.SubatPoliceVerilenKomisyon) + "},");
                            }
                        }

                        subatToplam += performans.SubatPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.MartPoliceVerilenKomisyon != null)
                    {
                        #region Mart Poliçe Verilen Komisyonları
                        martSayac++;
                        if (martSayac == 1)
                        {
                            scriptMart.AppendLine("var chartMartVerilenKomisyon = AmCharts.makeChart('mart-chartdiv-verilenkomisyon', {");
                            scriptMart.AppendLine("type: 'pie' ,");
                            scriptMart.AppendLine("theme: 'light',");
                            scriptMart.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MartPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.MartPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.MartPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelMart += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumMart + ", y :" + yKonumMart + "},";
                            }
                            else
                            {
                                labelMart += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumMart + ", y :" + yKonumMart + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptMart.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MartPoliceVerilenKomisyon) + "},");
                            }
                        }
                        martToplam += performans.MartPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.NisanPoliceVerilenKomisyon != null)
                    {
                        #region Nisan Poliçe Verilen Komisyonları
                        nisanSayac++;
                        if (nisanSayac == 1)
                        {
                            scriptNisan.AppendLine("var chartNisanVerilenKomisyon = AmCharts.makeChart('nisan-chartdiv-verilenkomisyon', {");
                            scriptNisan.AppendLine("type: 'pie' ,");
                            scriptNisan.AppendLine("theme: 'light',");
                            scriptNisan.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.NisanPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.NisanPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.NisanPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelNisan += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumNisan + ", y :" + yKonumNisan + "},";
                            }
                            else
                            {
                                labelNisan += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumNisan + ", y :" + yKonumNisan + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptNisan.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.NisanPoliceVerilenKomisyon) + "},");
                            }
                        }
                        nisanToplam += performans.NisanPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.MayisPoliceVerilenKomisyon != null)
                    {
                        #region Mayis Poliçe Verilen Komisyonları
                        mayisSayac++;
                        if (mayisSayac == 1)
                        {
                            scriptMayis.AppendLine("var chartMayisVerilenKomisyon = AmCharts.makeChart('mayis-chartdiv-verilenkomisyon', {");
                            scriptMayis.AppendLine("type: 'pie' ,");
                            scriptMayis.AppendLine("theme: 'light',");
                            scriptMayis.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.MayisPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.MayisPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.MayisPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelMayis += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumMayis + ", y :" + yKonumMayis + "},";
                            }
                            else
                            {
                                labelMayis += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumMayis + ", y :" + yKonumMayis + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptMayis.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.MayisPoliceVerilenKomisyon) + "},");
                            }
                        }
                        mayisToplam += performans.MayisPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.HaziranPoliceVerilenKomisyon != null)
                    {
                        #region Haziran Poliçe Verilen Komisyonları

                        haziranSayac++;
                        if (haziranSayac == 1)
                        {
                            scriptHaziran.AppendLine("var chartHaziranVerilenKomisyon = AmCharts.makeChart('haziran-chartdiv-verilenkomisyon', {");
                            scriptHaziran.AppendLine("type: 'pie' ,");
                            scriptHaziran.AppendLine("theme: 'light',");
                            scriptHaziran.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.HaziranPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.HaziranPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.HaziranPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelHaziran += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                            }
                            else
                            {
                                labelHaziran += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumHaziran + ", y :" + yKonumHaziran + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptHaziran.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.HaziranPoliceVerilenKomisyon) + "},");
                            }
                        }
                        haziranToplam += performans.HaziranPoliceVerilenKomisyon.Value;

                        #endregion
                    }

                    if (performans.TemmuzPoliceVerilenKomisyon != null)
                    {
                        #region Temmuz Poliçe Verilen Komisyonları
                        temmuzSayac++;
                        if (temmuzSayac == 1)
                        {
                            scriptTemmuz.AppendLine("var chartTemmuzVerilenKomisyon = AmCharts.makeChart('temmuz-chartdiv-verilenkomisyon', {");
                            scriptTemmuz.AppendLine("type: 'pie' ,");
                            scriptTemmuz.AppendLine("theme: 'light',");
                            scriptTemmuz.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.TemmuzPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.TemmuzPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.TemmuzPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelTemmuz += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                            }
                            else
                            {
                                labelTemmuz += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumTemmuz + ", y :" + yKonumTemmuz + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptTemmuz.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.TemmuzPoliceVerilenKomisyon) + "},");
                            }
                        }
                        temmuzToplam += performans.TemmuzPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.AgustosPoliceVerilenKomisyon != null)
                    {
                        #region Ağustos Poliçe Verilen Komisyonları
                        agustosSayac++;
                        if (agustosSayac == 1)
                        {
                            scriptAgustos.AppendLine("var chartAgustosVerilenKomisyon = AmCharts.makeChart('agustos-chartdiv-verilenkomisyon', {");
                            scriptAgustos.AppendLine("type: 'pie' ,");
                            scriptAgustos.AppendLine("theme: 'light',");
                            scriptAgustos.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AgustosPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.AgustosPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.AgustosPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelAgustos += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAgustos + ", y :" + yKonumAgustos + "},";
                            }
                            else
                            {
                                labelAgustos += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumAgustos + ", y :" + yKonumAgustos + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AgustosPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptAgustos.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AgustosPoliceVerilenKomisyon) + "},");
                            }
                        }
                        agustosToplam += performans.AgustosPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.EylulPoliceVerilenKomisyon != null)
                    {
                        #region Eylul Poliçe Verilen Komisyonları
                        eylulSayac++;
                        if (eylulSayac == 1)
                        {
                            scriptEylul.AppendLine("var chartEylulVerilenKomisyon = AmCharts.makeChart('eylul-chartdiv-verilenkomisyon', {");
                            scriptEylul.AppendLine("type: 'pie' ,");
                            scriptEylul.AppendLine("theme: 'light',");
                            scriptEylul.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EylulPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.EylulPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.EylulPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelEylul += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEylul + ", y :" + yKonumEylul + "},";
                            }
                            else
                            {
                                labelEylul += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumEylul + ", y :" + yKonumEylul + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptEylul.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EylulPoliceVerilenKomisyon) + "},");
                            }
                        }
                        eylulToplam += performans.EylulPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.EkimPoliceVerilenKomisyon != null)
                    {
                        #region Ekim Poliçe Verilen Komisyonları

                        ekimSayac++;
                        if (ekimSayac == 1)
                        {
                            scriptEkim.AppendLine("var chartEkimVerilenKomisyon = AmCharts.makeChart('ekim-chartdiv-verilenkomisyon', {");
                            scriptEkim.AppendLine("type: 'pie' ,");
                            scriptEkim.AppendLine("theme: 'light',");
                            scriptEkim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.EkimPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.EkimPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.EkimPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelEkim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                            }
                            else
                            {
                                labelEkim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumEkim + ", y :" + yKonumEkim + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptEkim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.EkimPoliceVerilenKomisyon) + "},");
                            }
                        }
                        ekimToplam += performans.EkimPoliceVerilenKomisyon.Value;

                        #endregion
                    }

                    if (performans.KasimPoliceVerilenKomisyon != null)
                    {
                        #region Kasım Poliçe Verilen Komisyonları
                        kasimSayac++;
                        if (kasimSayac == 1)
                        {
                            scriptKasim.AppendLine("var chartKasimVerilenKomisyon = AmCharts.makeChart('kasim-chartdiv-verilenkomisyon', {");
                            scriptKasim.AppendLine("type: 'pie' ,");
                            scriptKasim.AppendLine("theme: 'light',");
                            scriptKasim.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.KasimPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.KasimPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.KasimPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelKasim += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                            }
                            else
                            {
                                labelKasim += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumKasim + ", y :" + yKonumKasim + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptKasim.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.KasimPoliceVerilenKomisyon) + "},");
                            }
                        }
                        kasimToplam += performans.KasimPoliceVerilenKomisyon.Value;
                        #endregion
                    }

                    if (performans.AralikPoliceVerilenKomisyon != null)
                    {
                        #region Aralik Poliçe Verilen Komisyonları
                        aralikSayac++;
                        if (aralikSayac == 1)
                        {
                            scriptAralik.AppendLine("var chartAralikVerilenKomisyon = AmCharts.makeChart('aralik-chartdiv-verilenkomisyon', {");
                            scriptAralik.AppendLine("type: 'pie' ,");
                            scriptAralik.AppendLine("theme: 'light',");
                            scriptAralik.AppendLine("dataProvider:[");
                        }

                        sayac++;
                        if (bransListesi != null)
                        {
                            BransAdi = bransListesi.Where(s => s.BransKodu == performans.branskodu).Select(s => s.BransAdi).First();
                        }

                        if (performans.AralikPoliceVerilenKomisyon.HasValue)
                        {
                            if (performans.AralikPoliceVerilenKomisyon != null)
                            {
                                string[] parts = Convert.ToInt32(performans.AralikPoliceVerilenKomisyon.Value).ToString("N").Split(',');
                                labelAralik += "{ text: '" + BransAdi + " " + parts[0] + "', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                            }
                            else
                            {
                                labelAralik += "{ text: '" + BransAdi + " 0 ', bold: true, x: " + xKonumAralik + ", y :" + yKonumAralik + "},";
                            }
                            if (sayac != TeklifSayisi)
                            {
                                scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPoliceVerilenKomisyon) + "},");
                            }
                            else
                            {
                                scriptAralik.AppendLine("{brans:'" + BransAdi + "' , value: " + Convert.ToInt32(performans.AralikPoliceVerilenKomisyon) + "},");
                            }
                        }
                        aralikToplam += performans.AralikPoliceVerilenKomisyon.Value;
                        #endregion
                    }
                }
                aylarToplam.policeVerilenKomisyonOcak = ocakToplam;
                aylarToplam.policeVerilenKomisyonSubat = subatToplam;
                aylarToplam.policeVerilenKomisyonMart = martToplam;
                aylarToplam.policeVerilenKomisyonNisan = nisanToplam;
                aylarToplam.policeVerilenKomisyonMayis = mayisToplam;
                aylarToplam.policeVerilenKomisyonHaziran = haziranToplam;
                aylarToplam.policeVerilenKomisyonTemmuz = temmuzToplam;
                aylarToplam.policeVerilenKomisyonAgustos = agustosToplam;
                aylarToplam.policeVerilenKomisyonEylul = eylulToplam;
                aylarToplam.policeVerilenKomisyonEkim = ekimToplam;
                aylarToplam.policeVerilenKomisyonKasim = kasimToplam;
                aylarToplam.policeVerilenKomisyonAralik = aralikToplam;
                #region Script Sonu

                if (ocakSayac > 0)
                {
                    string[] parts = ocakToplam.ToString("N").Split(',');
                    labelOcak += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumOcak + 35) + "},";

                    if (ocakToplam != 0 && ocakPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((ocakToplam / ocakPoliceAdetToplam), 2);
                        labelOcak += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumOcak + 55) + "},";
                    }

                    scriptOcak.AppendLine("],");
                    scriptOcak.AppendLine("valueField: 'value' ,");
                    scriptOcak.AppendLine("titleField:'brans',");
                    scriptOcak.AppendLine("outlineAlpha: 0.4,");
                    scriptOcak.AppendLine("depth3D: 15 ,");
                    scriptOcak.AppendLine("angle:30 ,");
                    scriptOcak.AppendLine("allLabels:[" + labelOcak + " ],");
                    scriptOcak.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                    scriptOcak.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptOcak;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptOcak.AppendLine("$('.ocak-verilenKomisyon-img').show();");
                    scriptGenel = scriptOcak;

                    result += scriptGenel.ToString();
                }
                if (subatSayac > 0)
                {
                    string[] parts = subatToplam.ToString("N").Split(',');
                    labelSubat += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumSubat + 35) + "},";

                    if (subatToplam != 0 && subatPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((subatToplam / subatPoliceAdetToplam), 2);
                        labelSubat += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumSubat + 55) + "},";
                    }

                    scriptSubat.AppendLine("],");
                    scriptSubat.AppendLine("valueField: 'value' ,");
                    scriptSubat.AppendLine("titleField:'brans',");
                    scriptSubat.AppendLine("outlineAlpha: 0.4,");
                    scriptSubat.AppendLine("depth3D: 15 ,");
                    scriptSubat.AppendLine("angle:30 ,");
                    scriptSubat.AppendLine("allLabels:[" + labelSubat + " ],");
                    scriptSubat.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptSubat.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptSubat;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptSubat.AppendLine("$('.subat-verilenKomisyon-img').show();");
                    scriptGenel = scriptSubat;

                    result += scriptGenel.ToString();
                }
                if (martSayac > 0)
                {
                    string[] parts = martToplam.ToString("N").Split(',');
                    labelMart += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMart + 35) + "},";

                    if (martToplam != 0 && martPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((martToplam / martPoliceAdetToplam), 2);
                        labelMart += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMart + 55) + "},";
                    }

                    scriptMart.AppendLine("],");
                    scriptMart.AppendLine("valueField: 'value' ,");
                    scriptMart.AppendLine("titleField:'brans',");
                    scriptMart.AppendLine("outlineAlpha: 0.4,");
                    scriptMart.AppendLine("depth3D: 15 ,");
                    scriptMart.AppendLine("angle:30 ,");
                    scriptMart.AppendLine("allLabels:[" + labelMart + " ],");
                    scriptMart.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptMart.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMart.AppendLine("$('.mart-verilenKomisyon-img').show();");
                    scriptGenel = scriptMart;

                    result += scriptGenel.ToString();
                }
                if (nisanSayac > 0)
                {
                    string[] parts = nisanToplam.ToString("N").Split(',');
                    labelNisan += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumNisan + 35) + "},";

                    if (nisanToplam != 0 && nisanPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((nisanToplam / nisanPoliceAdetToplam), 2);
                        labelNisan += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumNisan + 55) + "},";
                    }

                    scriptNisan.AppendLine("],");
                    scriptNisan.AppendLine("valueField: 'value' ,");
                    scriptNisan.AppendLine("titleField:'brans',");
                    scriptNisan.AppendLine("outlineAlpha: 0.4,");
                    scriptNisan.AppendLine("depth3D: 15 ,");
                    scriptNisan.AppendLine("angle:30 ,");
                    scriptNisan.AppendLine("allLabels:[" + labelNisan + " ],");
                    scriptNisan.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                    scriptNisan.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptNisan.AppendLine("$('.nisan-verilenKomisyon-img').show();");
                    scriptGenel = scriptNisan;

                    result += scriptGenel.ToString();
                }
                if (mayisSayac > 0)
                {
                    string[] parts = mayisToplam.ToString("N").Split(',');
                    labelMayis += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMayis + 35) + "},";

                    if (mayisToplam != 0 && mayisPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((mayisToplam / mayisPoliceAdetToplam), 2);
                        labelMayis += "{ text: 'Poliçe Verilen Başı Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumMayis + 55) + "},";
                    }

                    scriptMayis.AppendLine("],");
                    scriptMayis.AppendLine("valueField: 'value' ,");
                    scriptMayis.AppendLine("titleField:'brans',");
                    scriptMayis.AppendLine("outlineAlpha: 0.4,");
                    scriptMayis.AppendLine("depth3D: 15 ,");
                    scriptMayis.AppendLine("angle:30 ,");
                    scriptMayis.AppendLine("allLabels:[" + labelMayis + " ],");
                    scriptMayis.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptMayis.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptMayis.AppendLine("$('.mayis-verilenKomisyon-img').show();");
                    scriptGenel = scriptMayis;

                    result += scriptGenel.ToString();
                }
                if (haziranSayac > 0)
                {
                    string[] parts = haziranToplam.ToString("N").Split(',');
                    labelHaziran += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumHaziran + 35) + "},";

                    if (haziranToplam != 0 && haziranPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((haziranToplam / haziranPoliceAdetToplam), 2);
                        labelHaziran += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumHaziran + 55) + "},";
                    }

                    scriptHaziran.AppendLine("],");
                    scriptHaziran.AppendLine("valueField: 'value' ,");
                    scriptHaziran.AppendLine("titleField:'brans',");
                    scriptHaziran.AppendLine("outlineAlpha: 0.4,");
                    scriptHaziran.AppendLine("depth3D: 15 ,");
                    scriptHaziran.AppendLine("angle:30 ,");
                    scriptHaziran.AppendLine("allLabels:[" + labelHaziran + " ],");
                    scriptHaziran.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptHaziran.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptHaziran.AppendLine("$('.haziran-verilenKomisyon-img').show();");
                    scriptGenel = scriptHaziran;

                    result += scriptGenel.ToString();
                }
                if (temmuzSayac > 0)
                {
                    string[] parts = temmuzToplam.ToString("N").Split(',');
                    labelTemmuz += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumTemmuz + 35) + "},";

                    if (temmuzToplam != 0 && temmuzPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((temmuzToplam / temmuzPoliceAdetToplam), 2);
                        labelTemmuz += "{ text: 'Poliçe Verilen Başı Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumTemmuz + 55) + "},";
                    }

                    scriptTemmuz.AppendLine("],");
                    scriptTemmuz.AppendLine("valueField: 'value' ,");
                    scriptTemmuz.AppendLine("titleField:'brans',");
                    scriptTemmuz.AppendLine("outlineAlpha: 0.4,");
                    scriptTemmuz.AppendLine("depth3D: 15 ,");
                    scriptTemmuz.AppendLine("angle:30 ,");
                    scriptTemmuz.AppendLine("allLabels:[" + labelTemmuz + " ],");
                    scriptTemmuz.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptTemmuz.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptTemmuz;
                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptTemmuz.AppendLine("$('.temmuz-verilenKomisyon-img').show();");
                    scriptGenel = scriptTemmuz;

                    result += scriptGenel.ToString();
                }
                if (agustosSayac > 0)
                {
                    string[] parts = agustosToplam.ToString("N").Split(',');
                    labelAgustos += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAgustos + 35) + "},";

                    if (agustosToplam != 0 && agustosPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((agustosToplam / agustosPoliceAdetToplam), 2);
                        labelAgustos += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAgustos + 55) + "},";
                    }

                    scriptAgustos.AppendLine("],");
                    scriptAgustos.AppendLine("valueField: 'value' ,");
                    scriptAgustos.AppendLine("titleField:'brans',");
                    scriptAgustos.AppendLine("outlineAlpha: 0.4,");
                    scriptAgustos.AppendLine("depth3D: 15 ,");
                    scriptAgustos.AppendLine("angle:30 ,");
                    scriptAgustos.AppendLine("allLabels:[" + labelAgustos + " ],");
                    scriptAgustos.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptAgustos.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAgustos.AppendLine("$('.agustos-verilenKomisyon-img').show();");
                    scriptGenel = scriptAgustos;

                    result += scriptGenel.ToString();
                }
                if (eylulSayac > 0)
                {
                    string[] parts = eylulToplam.ToString("N").Split(',');
                    labelEylul += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEylul + 35) + "},";

                    if (eylulToplam != 0 && eylulPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((eylulToplam / eylulPoliceAdetToplam), 2);
                        labelEylul += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEylul + 55) + "},";
                    }

                    scriptEylul.AppendLine("],");
                    scriptEylul.AppendLine("valueField: 'value' ,");
                    scriptEylul.AppendLine("titleField:'brans',");
                    scriptEylul.AppendLine("outlineAlpha: 0.4,");
                    scriptEylul.AppendLine("depth3D: 15 ,");
                    scriptEylul.AppendLine("angle:30 ,");
                    scriptEylul.AppendLine("allLabels:[" + labelEylul + " ],");
                    scriptEylul.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptEylul.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEylul.AppendLine("$('.eylul-verilenKomisyon-img').show();");
                    scriptGenel = scriptEylul;

                    result += scriptGenel.ToString();
                }
                if (ekimSayac > 0)
                {
                    string[] parts = ekimToplam.ToString("N").Split(',');
                    labelEkim += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEkim + 35) + "},";

                    if (ekimToplam != 0 && ekimPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((ekimToplam / ekimPoliceAdetToplam), 2);
                        labelEkim += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumEkim + 55) + "},";
                    }

                    scriptEkim.AppendLine("],");
                    scriptEkim.AppendLine("valueField: 'value' ,");
                    scriptEkim.AppendLine("titleField:'brans',");
                    scriptEkim.AppendLine("outlineAlpha: 0.4,");
                    scriptEkim.AppendLine("depth3D: 15 ,");
                    scriptEkim.AppendLine("angle:30 ,");
                    scriptEkim.AppendLine("allLabels:[" + labelEkim + " ],");
                    scriptEkim.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptEkim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptEkim.AppendLine("$('.ekim-verilenKomisyon-img').show();");
                    scriptGenel = scriptEkim;

                    result += scriptGenel.ToString();
                }
                if (kasimSayac > 0)
                {
                    string[] parts = kasimToplam.ToString("N").Split(',');
                    labelKasim += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumKasim + 35) + "},";

                    if (kasimToplam != 0 && kasimPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((kasimToplam / kasimPoliceAdetToplam), 2);
                        labelKasim += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumKasim + 55) + "},";
                    }

                    scriptKasim.AppendLine("],");
                    scriptKasim.AppendLine("valueField: 'value' ,");
                    scriptKasim.AppendLine("titleField:'brans',");
                    scriptKasim.AppendLine("outlineAlpha: 0.4,");
                    scriptKasim.AppendLine("depth3D: 15 ,");
                    scriptKasim.AppendLine("angle:30 ,");
                    scriptKasim.AppendLine("allLabels:[" + labelKasim + " ],");
                    scriptKasim.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptKasim.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptKasim.AppendLine("$('.kasim-verilenKomisyon-img').show();");
                    scriptGenel = scriptKasim;

                    result += scriptGenel.ToString();
                }
                if (aralikSayac > 0)
                {
                    string[] parts = aralikToplam.ToString("N").Split(',');
                    labelAralik += "{ text: ' Toplam Verilen Komisyon: " + parts[0] + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAralik + 35) + "},";

                    if (aralikToplam != 0 && aralikPoliceAdetToplam != 0)
                    {
                        decimal policeBasiKomisyon = Math.Round((aralikToplam / aralikPoliceAdetToplam), 2);
                        labelAralik += "{ text: 'Poliçe Başı Verilen Kom.: " + policeBasiKomisyon + tl + "', bold: true, x: " + 30 + ", y :" + (yKonumAralik + 55) + "},";
                    }

                    scriptAralik.AppendLine("],");
                    scriptAralik.AppendLine("valueField: 'value' ,");
                    scriptAralik.AppendLine("titleField:'brans',");
                    scriptAralik.AppendLine("outlineAlpha: 0.4,");
                    scriptAralik.AppendLine("depth3D: 15 ,");
                    scriptAralik.AppendLine("angle:30 ,");
                    scriptAralik.AppendLine("allLabels:[" + labelAralik + " ],");
                    scriptAralik.AppendLine("balloonText: '[[title]]" + " ([[value]]  " + tl + ") [[percents]]%' ,");
                    scriptAralik.AppendLine("export: '{ enabled:true}' });");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }
                else
                {
                    scriptAralik.AppendLine("$('.aralik-verilenKomisyon-img').show();");
                    scriptGenel = scriptAralik;

                    result += scriptGenel.ToString();
                }
                #endregion

            }

            return result;
        }

        private string GetJScriptGenelToplam(List<PoliceAylar> list)
        {
            string result = String.Empty;

            if (list != null)
            {
                StringBuilder scriptPoliceAdet = new StringBuilder();
                StringBuilder scriptPolicePrim = new StringBuilder();
                StringBuilder scriptPoliceKomisyon = new StringBuilder();
                StringBuilder scriptPoliceVerilenKomisyon = new StringBuilder();
                StringBuilder scriptGenel = new StringBuilder();

                string labelGenelToplamPolice = String.Empty;
                string labelGenelToplamPrim = String.Empty;
                string labelGenelToplamKomisyon = String.Empty;
                string labelGenelToplamVerilenKomisyon = String.Empty;

                int genelToplamPoliceAdet = 0;
                decimal genelToplamPolicePrim = 0;
                decimal genelToplamPoliceKomisyon = 0;
                decimal genelToplamPoliceVerilenKomisyon = 0;

                #region Poliçe Adetleri

                scriptPoliceAdet.AppendLine("var chartGenelToplamPolice = AmCharts.makeChart('geneltoplam-chartdiv-police', {");
                scriptPoliceAdet.AppendLine("type: 'pie' ,");
                scriptPoliceAdet.AppendLine("theme: 'light',");
                scriptPoliceAdet.AppendLine("dataProvider:[");

                foreach (var item in list)
                {
                    if (item.policeAdetOcak != null)
                    {
                        string[] parts = item.policeAdetOcak.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Ocak: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 500 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Ocak' , value:" + item.policeAdetOcak + "},");
                        genelToplamPoliceAdet += item.policeAdetOcak;
                    }
                    if (item.policeAdetSubat != null)
                    {
                        string[] parts = item.policeAdetSubat.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Şubat: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 500 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Şubat' , value:" + item.policeAdetSubat + "},");
                        genelToplamPoliceAdet += item.policeAdetSubat;
                    }
                    if (item.policeAdetMart != null)
                    {
                        string[] parts = item.policeAdetMart.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Mart: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 500 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Mart' , value:" + item.policeAdetMart + "},");
                        genelToplamPoliceAdet += item.policeAdetMart;
                    }
                    if (item.policeAdetNisan != null)
                    {
                        string[] parts = item.policeAdetNisan.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Nisan: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 520 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Nisan' , value:" + item.policeAdetNisan + "},");
                        genelToplamPoliceAdet += item.policeAdetNisan;
                    }
                    if (item.policeAdetMayis != null)
                    {
                        string[] parts = item.policeAdetMayis.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Mayıs: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 520 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Mayıs' , value:" + item.policeAdetMayis + "},");
                        genelToplamPoliceAdet += item.policeAdetMayis;
                    }
                    if (item.policeAdetHaziran != null)
                    {
                        string[] parts = item.policeAdetHaziran.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Haziran: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 520 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Haziran' , value:" + item.policeAdetHaziran + "},");
                        genelToplamPoliceAdet += item.policeAdetHaziran;
                    }
                    if (item.policeAdetTemmuz != null)
                    {
                        string[] parts = item.policeAdetTemmuz.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Temmuz: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 540 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Temmuz' , value:" + item.policeAdetTemmuz + "},");
                        genelToplamPoliceAdet += item.policeAdetTemmuz;
                    }
                    if (item.policeAdetAgustos != null)
                    {
                        string[] parts = item.policeAdetAgustos.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: ' Ağustos: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 540 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Ağustos' , value:" + item.policeAdetAgustos + "},");
                        genelToplamPoliceAdet += item.policeAdetAgustos;
                    }
                    if (item.policeAdetEylul != null)
                    {
                        string[] parts = item.policeAdetEylul.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: 'Eylül: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 540 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Eylül' , value:" + item.policeAdetEylul + "},");
                        genelToplamPoliceAdet += item.policeAdetEylul;
                    }
                    if (item.policeAdetEkim != null)
                    {
                        string[] parts = item.policeAdetEkim.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: 'Ekim: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 560 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Ekim' , value:" + item.policeAdetEkim + "},");
                        genelToplamPoliceAdet += item.policeAdetEkim;
                    }
                    if (item.policeAdetKasim != null)
                    {
                        string[] parts = item.policeAdetKasim.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: 'Kasım: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 560 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Kasım' , value:" + item.policeAdetKasim + "},");
                        genelToplamPoliceAdet += item.policeAdetKasim;
                    }
                    if (item.policeAdetAralik != null)
                    {
                        string[] parts = item.policeAdetAralik.ToString("N").Split(',');
                        labelGenelToplamPolice += "{ text: 'Aralık: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 560 + "},";
                        scriptPoliceAdet.AppendLine("{ay:'Aralık' , value:" + item.policeAdetAralik + "},");
                        genelToplamPoliceAdet += item.policeAdetAralik;
                    }
                }

                string[] partsAdet = genelToplamPoliceAdet.ToString("N").Split(',');
                labelGenelToplamPolice += "{ text: 'Genel Toplam: " + partsAdet[0] + "', bold: true, x: " + 30 + ", y :" + 590 + "},";
                scriptPoliceAdet.AppendLine("],");
                scriptPoliceAdet.AppendLine("valueField: 'value' ,");
                scriptPoliceAdet.AppendLine("titleField:'ay',");
                scriptPoliceAdet.AppendLine("outlineAlpha: 0.4,");
                scriptPoliceAdet.AppendLine("depth3D: 15 ,");
                scriptPoliceAdet.AppendLine("angle:30 ,");
                scriptPoliceAdet.AppendLine("allLabels:[" + labelGenelToplamPolice + " ],");
                scriptPoliceAdet.AppendLine("balloonText: '[[title]]" + " ([[value]] Adet) [[percents]]%' ,");
                scriptPoliceAdet.AppendLine("export: '{ enabled:true}' });");

                scriptGenel = scriptPoliceAdet;
                result += scriptGenel;

                #endregion

                #region Poliçe Primleri

                scriptPolicePrim.AppendLine("var chartGenelToplamPrim = AmCharts.makeChart('geneltoplam-chartdiv-prim', {");
                scriptPolicePrim.AppendLine("type: 'pie' ,");
                scriptPolicePrim.AppendLine("theme: 'light',");
                scriptPolicePrim.AppendLine("dataProvider:[");

                foreach (var item in list)
                {
                    if (item.policePrimOcak != null)
                    {
                        string[] parts = item.policePrimOcak.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Ocak: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 500 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Ocak' , value:" + item.policePrimOcak + "},");
                        genelToplamPolicePrim += item.policePrimOcak;
                    }
                    if (item.policePrimSubat != null)
                    {
                        string[] parts = item.policePrimSubat.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Şubat: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 500 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Şubat' , value:" + item.policePrimSubat + "},");
                        genelToplamPolicePrim += item.policePrimSubat;
                    }
                    if (item.policePrimMart != null)
                    {
                        string[] parts = item.policePrimMart.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Mart: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 500 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Mart' , value:" + item.policePrimMart + "},");
                        genelToplamPolicePrim += item.policePrimMart;
                    }
                    if (item.policePrimNisan != null)
                    {
                        string[] parts = item.policePrimNisan.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Nisan: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 520 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Nisan' , value:" + item.policePrimNisan + "},");
                        genelToplamPolicePrim += item.policePrimNisan;
                    }
                    if (item.policePrimMayis != null)
                    {
                        string[] parts = item.policePrimMayis.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Mayıs: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 520 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Mayıs' , value:" + item.policePrimMayis + "},");
                        genelToplamPolicePrim += item.policePrimMayis;
                    }
                    if (item.policePrimHaziran != null)
                    {
                        string[] parts = item.policePrimHaziran.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Haziran: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 520 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Haziran' , value:" + item.policePrimHaziran + "},");
                        genelToplamPolicePrim += item.policePrimHaziran;
                    }
                    if (item.policePrimTemmuz != null)
                    {
                        string[] parts = item.policePrimTemmuz.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Temmuz: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 540 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Temmuz' , value:" + item.policePrimTemmuz + "},");
                        genelToplamPolicePrim += item.policePrimTemmuz;
                    }
                    if (item.policePrimAgustos != null)
                    {
                        string[] parts = item.policePrimAgustos.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: ' Ağustos: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 540 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Ağustos' , value:" + item.policePrimAgustos + "},");
                        genelToplamPolicePrim += item.policePrimAgustos;
                    }
                    if (item.policePrimEylul != null)
                    {
                        string[] parts = item.policePrimEylul.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: 'Eylül: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 540 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Eylül' , value:" + item.policePrimEylul + "},");
                        genelToplamPolicePrim += item.policePrimEylul;
                    }
                    if (item.policePrimEkim != null)
                    {
                        string[] parts = item.policePrimEkim.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: 'Ekim: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 560 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Ekim' , value:" + item.policePrimEkim + "},");
                        genelToplamPolicePrim += item.policePrimEkim;
                    }
                    if (item.policePrimKasim != null)
                    {
                        string[] parts = item.policePrimKasim.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: 'Kasım: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 560 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Kasım' , value:" + item.policePrimKasim + "},");
                        genelToplamPolicePrim += item.policePrimKasim;
                    }
                    if (item.policePrimAralik != null)
                    {
                        string[] parts = item.policePrimAralik.ToString("N").Split(',');
                        labelGenelToplamPrim += "{ text: 'Aralık: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 560 + "},";
                        scriptPolicePrim.AppendLine("{ay:'Aralık' , value:" + item.policePrimAralik + "},");
                        genelToplamPolicePrim += item.policePrimAralik;
                    }
                }
                string[] partsPrim = genelToplamPolicePrim.ToString("N").Split(',');
                labelGenelToplamPrim += "{ text: 'Genel Toplam: " + partsPrim[0] + tl + "', bold: true, x: " + 30 + ", y :" + 590 + "},";

                if (genelToplamPolicePrim != 0 && genelToplamPoliceAdet != 0)
                {
                    var policePrimOran = Math.Round((genelToplamPolicePrim / genelToplamPoliceAdet), 2);
                    labelGenelToplamPrim += "{ text: 'Poliçe Başı Prim: " + policePrimOran + tl + "', bold: true, x: " + 30 + ", y :" + 610 + "},";
                }

                scriptPolicePrim.AppendLine("],");
                scriptPolicePrim.AppendLine("valueField: 'value' ,");
                scriptPolicePrim.AppendLine("titleField:'ay',");
                scriptPolicePrim.AppendLine("outlineAlpha: 0.4,");
                scriptPolicePrim.AppendLine("depth3D: 15 ,");
                scriptPolicePrim.AppendLine("angle:30 ,");
                scriptPolicePrim.AppendLine("allLabels:[" + labelGenelToplamPrim + " ],");
                scriptPolicePrim.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                scriptPolicePrim.AppendLine("export: '{ enabled:true}' });");

                scriptGenel = scriptPolicePrim;
                result += scriptGenel;

                #endregion

                #region Komisyonlar

                scriptPoliceKomisyon.AppendLine("var chartGenelToplamKomisyon = AmCharts.makeChart('geneltoplam-chartdiv-komisyon', {");
                scriptPoliceKomisyon.AppendLine("type: 'pie' ,");
                scriptPoliceKomisyon.AppendLine("theme: 'light',");
                scriptPoliceKomisyon.AppendLine("dataProvider:[");

                foreach (var item in list)
                {
                    if (item.policeKomisyonOcak != null)
                    {
                        string[] parts = item.policeKomisyonOcak.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: 'Ocak: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 500 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Ocak' , value:" + item.policeKomisyonOcak + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonOcak;
                    }
                    if (item.policeKomisyonSubat != null)
                    {
                        string[] parts = item.policeKomisyonSubat.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: ' Şubat: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 500 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Şubat' , value:" + item.policeKomisyonSubat + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonSubat;
                    }
                    if (item.policeKomisyonMart != null)
                    {
                        string[] parts = item.policeKomisyonMart.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: ' Mart: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 500 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Mart' , value:" + item.policeKomisyonMart + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonMart;
                    }
                    if (item.policeKomisyonNisan != null)
                    {
                        string[] parts = item.policeKomisyonNisan.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: ' Nisan: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 520 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Nisan' , value:" + item.policeKomisyonNisan + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonNisan;
                    }
                    if (item.policeKomisyonMayis != null)
                    {
                        string[] parts = item.policeKomisyonMayis.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: ' Mayıs: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 520 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Mayıs' , value:" + item.policeKomisyonMayis + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonMayis;
                    }
                    if (item.policeKomisyonHaziran != null)
                    {
                        string[] parts = item.policeKomisyonHaziran.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: ' Haziran: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 520 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Haziran' , value:" + item.policeKomisyonHaziran + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonHaziran;
                    }
                    if (item.policeKomisyonTemmuz != null)
                    {
                        string[] parts = item.policeKomisyonTemmuz.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: ' Temmuz: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 540 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Temmuz' , value:" + item.policeKomisyonTemmuz + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonTemmuz;
                    }
                    if (item.policeKomisyonAgustos != null)
                    {
                        string[] parts = item.policeKomisyonAgustos.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: ' Ağustos: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 540 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Ağustos' , value:" + item.policeKomisyonAgustos + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonAgustos;
                    }
                    if (item.policeKomisyonEylul != null)
                    {
                        string[] parts = item.policeKomisyonEylul.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: 'Eylül: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 540 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Eylül' , value:" + item.policeKomisyonEylul + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonEylul;
                    }
                    if (item.policeKomisyonEkim != null)
                    {
                        string[] parts = item.policeKomisyonEkim.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: 'Ekim: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 560 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Ekim' , value:" + item.policeKomisyonEkim + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonEkim;
                    }
                    if (item.policeKomisyonKasim != null)
                    {
                        string[] parts = item.policeKomisyonKasim.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: 'Kasım: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 560 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Kasım' , value:" + item.policeKomisyonKasim + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonKasim;
                    }
                    if (item.policeKomisyonAralik != null)
                    {
                        string[] parts = item.policeKomisyonAralik.ToString("N").Split(',');
                        labelGenelToplamKomisyon += "{ text: 'Aralık: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 560 + "},";
                        scriptPoliceKomisyon.AppendLine("{ay:'Aralık' , value:" + item.policeKomisyonAralik + "},");
                        genelToplamPoliceKomisyon += item.policeKomisyonAralik;
                    }
                }

                string[] partsKom = genelToplamPoliceKomisyon.ToString("N").Split(',');
                labelGenelToplamKomisyon += "{ text: 'Genel Toplam: " + partsKom[0] + tl + "', bold: true, x: " + 30 + ", y :" + 590 + "},";

                if (genelToplamPoliceKomisyon != 0 && genelToplamPoliceAdet != 0)
                {
                    var policeKomisyonOran = Math.Round((genelToplamPoliceKomisyon / genelToplamPoliceAdet), 2);
                    labelGenelToplamKomisyon += "{ text: 'Poliçe Başı Komisyon: " + policeKomisyonOran + tl + "', bold: true, x: " + 30 + ", y :" + 610 + "},";
                }

                scriptPoliceKomisyon.AppendLine("],");
                scriptPoliceKomisyon.AppendLine("valueField: 'value' ,");
                scriptPoliceKomisyon.AppendLine("titleField:'ay',");
                scriptPoliceKomisyon.AppendLine("outlineAlpha: 0.4,");
                scriptPoliceKomisyon.AppendLine("depth3D: 15 ,");
                scriptPoliceKomisyon.AppendLine("angle:30 ,");
                scriptPoliceKomisyon.AppendLine("allLabels:[" + labelGenelToplamKomisyon + " ],");
                scriptPoliceKomisyon.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                scriptPoliceKomisyon.AppendLine("export: '{ enabled:true}' });");

                scriptGenel = scriptPoliceKomisyon;
                result += scriptGenel;

                #endregion

                #region Poliçe Verilen Komisyonlar

                scriptPoliceVerilenKomisyon.AppendLine("var chartGenelToplamVerilenKomisyon = AmCharts.makeChart('geneltoplam-chartdiv-verilenkomisyon', {");
                scriptPoliceVerilenKomisyon.AppendLine("type: 'pie' ,");
                scriptPoliceVerilenKomisyon.AppendLine("theme: 'light',");
                scriptPoliceVerilenKomisyon.AppendLine("dataProvider:[");

                foreach (var item in list)
                {
                    if (item.policeVerilenKomisyonOcak != null)
                    {
                        string[] parts = item.policeVerilenKomisyonOcak.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: 'Ocak: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 500 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Ocak' , value:" + item.policeVerilenKomisyonOcak + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonOcak;
                    }
                    if (item.policeVerilenKomisyonSubat != null)
                    {
                        string[] parts = item.policeVerilenKomisyonSubat.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: ' Şubat:" + parts[0] + "', bold: true, x: " + 180 + ", y :" + 500 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Şubat' , value:" + item.policeVerilenKomisyonSubat + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonSubat;
                    }
                    if (item.policeVerilenKomisyonMart != null)
                    {
                        string[] parts = item.policeVerilenKomisyonMart.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: ' Mart: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 500 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Mart' , value:" + item.policeVerilenKomisyonMart + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonMart;
                    }
                    if (item.policeVerilenKomisyonNisan != null)
                    {
                        string[] parts = item.policeVerilenKomisyonNisan.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: ' Nisan: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 520 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Nisan' , value:" + item.policeVerilenKomisyonNisan + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonNisan;
                    }
                    if (item.policeVerilenKomisyonMayis != null)
                    {
                        string[] parts = item.policeVerilenKomisyonMayis.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: ' Mayıs: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 520 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Mayıs' , value:" + item.policeVerilenKomisyonMayis + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonMayis;
                    }
                    if (item.policeVerilenKomisyonHaziran != null)
                    {
                        string[] parts = item.policeVerilenKomisyonHaziran.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: ' Haziran: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 520 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Haziran' , value:" + item.policeVerilenKomisyonHaziran + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonHaziran;
                    }
                    if (item.policeVerilenKomisyonTemmuz != null)
                    {
                        string[] parts = item.policeVerilenKomisyonTemmuz.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: ' Temmuz: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 540 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Temmuz' , value:" + item.policeVerilenKomisyonTemmuz + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonTemmuz;
                    }
                    if (item.policeVerilenKomisyonAgustos != null)
                    {
                        string[] parts = item.policeVerilenKomisyonAgustos.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: ' Ağustos: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 540 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Ağustos' , value:" + item.policeVerilenKomisyonAgustos + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonAgustos;
                    }
                    if (item.policeVerilenKomisyonEylul != null)
                    {
                        string[] parts = item.policeVerilenKomisyonEylul.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: 'Eylül: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 540 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Eylül' , value:" + item.policeVerilenKomisyonEylul + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonEylul;
                    }
                    if (item.policeVerilenKomisyonEkim != null)
                    {
                        string[] parts = item.policeVerilenKomisyonEkim.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: 'Ekim: " + parts[0] + "', bold: true, x: " + 30 + ", y :" + 560 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Ekim' , value:" + item.policeVerilenKomisyonEkim + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonEkim;
                    }
                    if (item.policeVerilenKomisyonKasim != null)
                    {
                        string[] parts = item.policeVerilenKomisyonKasim.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: 'Kasım: " + parts[0] + "', bold: true, x: " + 180 + ", y :" + 560 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Kasım' , value:" + item.policeVerilenKomisyonKasim + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonKasim;
                    }
                    if (item.policeVerilenKomisyonAralik != null)
                    {
                        string[] parts = item.policeVerilenKomisyonAralik.ToString("N").Split(',');
                        labelGenelToplamVerilenKomisyon += "{ text: 'Aralık: " + parts[0] + "', bold: true, x: " + 330 + ", y :" + 560 + "},";
                        scriptPoliceVerilenKomisyon.AppendLine("{ay:'Aralık' , value:" + item.policeVerilenKomisyonAralik + "},");
                        genelToplamPoliceVerilenKomisyon += item.policeVerilenKomisyonAralik;
                    }
                }

                string[] partsVerKom = genelToplamPoliceVerilenKomisyon.ToString("N").Split(',');
                labelGenelToplamVerilenKomisyon += "{ text: 'Genel Toplam Verilen Komisyon: " + partsVerKom[0] + tl + "', bold: true, x: " + 30 + ", y :" + 590 + "},";

                if (genelToplamPoliceVerilenKomisyon != 0 && genelToplamPoliceAdet != 0)
                {
                    var policeVerilenKomisyonOran = Math.Round((genelToplamPoliceVerilenKomisyon / genelToplamPoliceAdet), 2);
                    labelGenelToplamVerilenKomisyon += "{ text: 'Poliçe Başı Verilen Kom.: " + policeVerilenKomisyonOran + tl + "', bold: true, x: " + 30 + ", y :" + 610 + "},";
                }

                scriptPoliceVerilenKomisyon.AppendLine("],");
                scriptPoliceVerilenKomisyon.AppendLine("valueField: 'value' ,");
                scriptPoliceVerilenKomisyon.AppendLine("titleField:'ay',");
                scriptPoliceVerilenKomisyon.AppendLine("outlineAlpha: 0.4,");
                scriptPoliceVerilenKomisyon.AppendLine("depth3D: 15 ,");
                scriptPoliceVerilenKomisyon.AppendLine("angle:30 ,");
                scriptPoliceVerilenKomisyon.AppendLine("allLabels:[" + labelGenelToplamVerilenKomisyon + " ],");
                scriptPoliceVerilenKomisyon.AppendLine("balloonText: '[[title]]" + " ([[value]] " + tl + ") [[percents]]%' ,");
                scriptPoliceVerilenKomisyon.AppendLine("export: '{ enabled:true}' });");

                scriptGenel = scriptPoliceVerilenKomisyon;
                result += scriptGenel;

                #endregion
            }
            return result;
        }

    }

  
}