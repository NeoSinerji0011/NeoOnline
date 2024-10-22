using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class FibaExcelReader
    {
        HSSFWorkbook wb;
        ITVMService _TVMService;
        private string message = string.Empty;
        private string excelFileName;
        private string birlikKodu;
        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;

        private string[] columnNames = {
                                            "TIP",//0
                                            "SIRKET",//1
                                            "BRANSKOD",//2
                                            "POLID",//3
                                            "ZEYLTIP_ACIKLAMA",//4
                                            "TARIFENO",//5
                                            "UZUNAD",//6
                                            "POLICENO",//7
                                            "ZEYLNO",//8
                                            "GRUPID",//9
                                            "GRUPZEYLNO",//10
                                            "ZEYL_DURUM",//11
                                            "TANTAR",//12
                                            "POLBASTAR",//13
                                            "POLBITTAR",//14
                                            "YILPRIM",//15
                                            "ODETIP",//16
                                            "ODEME_PERIYODU",//17
                                            "THKTAR",//18
                                            "POLTANTAR", //19
                                            "DOVKUR", //20
                                            "DOVKOD",//21
                                            "DOVNET",//22
                                            "TLNET",//23
                                            "TLVERGI",//24
                                            "TLKOMTUTAR",//25
                                            "VADETAR",//26
                                            "TAHSILAT",//27
                                            "MIN_TAHSTAR",//28
                                            "ODEME_TUR",//29
                                            "ODEYEN",//30
                                            "ODEYEN_TCKN",//31
                                            "ODEYEN_VKN",//32
                                            "SIGORTALI",//33
                                            "SIGOR_TCKN",//34
                                            "SIGOR_VERGI",//35
                                   
                                       };

        public FibaExcelReader(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.excelFileName = fileName;
            this.birlikKodu = birlikKodu;
            this.tvmKodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }

        public List<Police> getPoliceler()
        {
            // Odeme plani icin kullanilacaklar
            int taksitNo = 0;
            int carpan = 1;
            Police odemePol = null;

            // excelden alinan policeler
            List<Police> policeler = new List<Police>();

            // get excel file...
            FileStream excelFile = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            int? bransKod = null;
            decimal? polKomisyon = null;
            decimal? polNet = null;
            decimal? polBrutprimim = null;
            try
            {
                excelFile = new FileStream(excelFileName, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }



            wb = new HSSFWorkbook(excelFile);
            ISheet sheet = wb.GetSheet("Sayfa1");

            // check sheet correct... 
            int startRow = Util.checkSheetCorrect(sheet, columnNames);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }

            // sheet correct. Start to get rows...
            try
            {
                for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
                {
                    IRow row = sheet.GetRow(indx);

                    // null rowlar icin
                    if (row == null) continue;

                    // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                    if (row.FirstCellNum == 0 && row.GetCell(0).StringCellValue == "P Brüt Prim") break;

                    // Police genel bilgileri icin. Police genel bilgiler aliniyor.
                    if (row.FirstCellNum == 0)
                    {

                        Police pol = new Police();
                        PoliceOdemePlani odm = new PoliceOdemePlani();
                        pol.GenelBilgiler.OdemeSekli = 1; // pesin
                        carpan = 1;
                        MusteriGenelBilgiler musGenel = new MusteriGenelBilgiler();

                        // tvm kodu
                        pol.GenelBilgiler.TVMKodu = tvmKodu;

                        // Birlik Kodu
                        pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.FIBAEMEKLILIK;

                        List<ICell> cels = row.Cells;
                        var Tip = "";

                        foreach (ICell cell in cels)
                        {
                            pol.GenelBilgiler.YenilemeNo = 0;

                            if (cell.ColumnIndex == 0)
                            {
                                Tip = cell.StringCellValue;
                            }

                            if (cell.ColumnIndex == 4) pol.GenelBilgiler.SirketZeyilAdi = cell.StringCellValue;
                            if (cell.ColumnIndex == 5)
                            {
                                if (cell.CellType == CellType.Numeric)
                                {
                                    tumUrunKodu = cell.NumericCellValue.ToString();
                                }
                                else if (cell.CellType == CellType.String)
                                {
                                    tumUrunKodu = cell.StringCellValue;
                                }
                            }
                            if (cell.ColumnIndex == 7)
                            {
                                if (!tumUrunKodu.Contains("GHS"))
                                {
                                    pol.GenelBilgiler.PoliceNumarasi = cell.NumericCellValue.ToString();
                                }
                            }

                            if (cell.ColumnIndex == 8) pol.GenelBilgiler.EkNo = Convert.ToInt32(cell.NumericCellValue);

                            if (cell.ColumnIndex == 9)
                            {
                                if (Tip == "ANA_SOZLESME" && tumUrunKodu.Contains("GHS"))
                                {
                                    pol.GenelBilgiler.PoliceNumarasi = cell.NumericCellValue.ToString();
                                }
                            }

                            if (cell.ColumnIndex == 10) pol.GenelBilgiler.EkNo = Convert.ToInt32(cell.NumericCellValue);

                            if (cell.ColumnIndex == 11)
                            {
                                if (cell.ColumnIndex != null)
                                {
                                    pol.GenelBilgiler.ZeyilKodu = cell.StringCellValue.Substring(0, 10);
                                }
                            }

                            if (cell.ColumnIndex == 12)
                            {
                                if (pol.GenelBilgiler.EkNo != 0)
                                {
                                    pol.GenelBilgiler.TanzimTarihi = cell.DateCellValue;
                                }
                            }
                            if (cell.ColumnIndex == 13) pol.GenelBilgiler.BaslangicTarihi = cell.DateCellValue;
                            if (cell.ColumnIndex == 14) pol.GenelBilgiler.BitisTarihi = cell.DateCellValue;





                            if (cell.ColumnIndex == 19)
                            {
                                if (pol.GenelBilgiler.EkNo == 0)
                                {
                                    pol.GenelBilgiler.TanzimTarihi = cell.DateCellValue;
                                }
                            }
                            if (cell.ColumnIndex == 20)
                            {
                                if (!String.IsNullOrEmpty(cell.NumericCellValue.ToString()))
                                {
                                    pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.NumericCellValue.ToString());
                                }
                            }

                            if (cell.ColumnIndex == 21) pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;

                            //if (cell.ColumnIndex == 22)
                            //{
                            //    pol.GenelBilgiler.NetPrim = Convert.ToInt32(cell.NumericCellValue);
                            //    taksitNo = 1;
                            //    odm.TaksitNo = taksitNo;
                            //    odm.VadeTarihi = pol.GenelBilgiler.TanzimTarihi;

                            //    odm.TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString()); // iptal ise tutuar eksi deger olmali

                            //    pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            //}
                            if (cell.ColumnIndex == 23) //TLNET
                            {
                                pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                                polNet = pol.GenelBilgiler.NetPrim;
                                taksitNo = 1;
                                odm.TaksitNo = taksitNo;
                                odm.VadeTarihi = pol.GenelBilgiler.TanzimTarihi;

                                odm.TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString()); // iptal ise tutuar eksi deger olmali
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odm.TaksitTutari = Math.Round(Util.ToDecimal(cell.NumericCellValue.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    odm.DovizliTaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                }
                                if (odm.TaksitTutari != 0)
                                {
                                    pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                }

                            }

                            if (cell.ColumnIndex == 24)
                            {
                                // Gider Vergisi
                                pol.GenelBilgiler.ToplamVergi = 0;
                                PoliceVergi gv = new PoliceVergi();
                                gv.VergiKodu = 2;
                                gv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                pol.GenelBilgiler.ToplamVergi += gv.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(gv);


                                pol.GenelBilgiler.BrutPrim = pol.GenelBilgiler.ToplamVergi + pol.GenelBilgiler.NetPrim;
                                polBrutprimim = pol.GenelBilgiler.BrutPrim;
                                if (pol.GenelBilgiler.BrutPrim < 0)
                                {
                                    carpan = -1;

                                }
                                if (odm.TaksitTutari == 0 && pol.GenelBilgiler.BrutPrim != 0 && odm.TaksitNo == 1)
                                {
                                    odm.TaksitTutari = pol.GenelBilgiler.BrutPrim;
                                    if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                    {
                                        odm.DovizliTaksitTutari = pol.GenelBilgiler.BrutPrim.Value;
                                    }
                                    if (odm.VadeTarihi == null)
                                    {
                                        odm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                                    }

                                    odm.TaksitNo = 1;
                                    if (pol.GenelBilgiler.BrutPrim <= 0)
                                    {
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                    }
                                    pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                }

                            }

                            if (cell.ColumnIndex == 25)
                            {
                                pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.NumericCellValue.ToString());
                                polKomisyon = pol.GenelBilgiler.Komisyon;
                            }

                            //if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                            //{
                            // pol.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                            //pol.GenelBilgiler.DovizliNetPrim = polKomisyon.Value;
                            //pol.GenelBilgiler.DovizliKomisyon = polNet.Value;
                            // }

                            if (cell.ColumnIndex == 29)
                            {
                                var odeme_tur = cell.StringCellValue;
                                #region Tahsilat işlemi                  

                                PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                                PoliceTahsilat tahsilat;

                                var tcknCell = cels.Where(x => x.ColumnIndex == 34).FirstOrDefault();
                                var vknCell = cels.Where(x => x.ColumnIndex == 35).FirstOrDefault();

                                var tckn = tcknCell?.NumericCellValue.ToString();
                                var vkn = vknCell?.NumericCellValue.ToString();

                                string kimlikNo = !String.IsNullOrEmpty(tckn) ? tckn : vkn;
                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.FIBAEMEKLILIK, pol.GenelBilgiler.BransKodu.Value);
                                if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                {
                                    int otoOdeSayac = 0;
                                    foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                    {
                                        if (otoOdeSayac < 1 && pol.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                        {
                                            otoOdeSayac++;
                                            tahsilat = new PoliceTahsilat();
                                            tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                            odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                            if (tahsilat.OdemTipi == 1)
                                            {
                                                tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                tahsilat.KalanTaksitTutari = 0;
                                                tahsilat.OdemeBelgeNo = "111111****1111";
                                                tahsilat.OtomatikTahsilatiKkMi = 1;
                                            }
                                            else
                                            {
                                                tahsilat.OdenenTutar = 0;
                                                tahsilat.KalanTaksitTutari = odm.TaksitTutari;
                                            }
                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = odm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = kimlikNo;
                                            tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.Value;
                                            tahsilat.PoliceId = odm.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (odeme_tur == "Havale")
                                    {
                                        tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = kimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else if (odeme_tur == "Kredi Kartı")
                                    {
                                        tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = "111111****1111";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.KalanTaksitTutari = 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = kimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else if (odeme_tur == "Nakit")
                                    {
                                        tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                        odm.OdemeTipi = OdemeTipleri.Nakit;

                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = kimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else if (odeme_tur == "Hesaptan Otomatik")
                                    {
                                        tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odm.OdemeTipi = OdemeTipleri.Havale;

                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.KalanTaksitTutari = 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = kimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else
                                    {
                                        tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        // tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = kimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    if (tahsilat.KimlikNo == null)
                                    {

                                    }
                                }


                                #endregion
                            }

                            if (cell.ColumnIndex == 30)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                            }

                            if (cell.ColumnIndex == 31)
                            {
                                if (pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo != null)
                                {

                                    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = row.GetCell(31).NumericCellValue.ToString();
                                    //pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.NumericCellValue.ToString();

                                    if (!tumUrunKodu.Contains("GHS"))
                                    {
                                        pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = pol.GenelBilgiler.PoliceSigortali.KimlikNo;
                                    }
                                }
                                else
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = row.GetCell(31).NumericCellValue.ToString();
                                    //pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.NumericCellValue.ToString();

                                }
                            }


                            if (cell.ColumnIndex == 32)
                            {
                                if (pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo != null)
                                {

                                    pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = row.GetCell(32).NumericCellValue.ToString();
                                    //pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.NumericCellValue.ToString();

                                    if (!tumUrunKodu.Contains("GHS"))
                                    {
                                        pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    }
                                }
                                else
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = row.GetCell(32).NumericCellValue.ToString();

                                }
                                sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                if (tumUrunKodu.Contains("GHS"))
                                {
                                    sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo) ? pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                }

                                if(sLiKimlikNo == null)
                                {

                                }
                            }


                            if (cell.ColumnIndex == 33)
                            {
                                pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                                if (!tumUrunKodu.Contains("GHS"))
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = pol.GenelBilgiler.PoliceSigortali.AdiUnvan;
                                }
                            }



                            if (cell.ColumnIndex == 34)
                            {

                                pol.GenelBilgiler.PoliceSigortali.KimlikNo = row.GetCell(34).NumericCellValue.ToString();
                                //pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.NumericCellValue.ToString();
                                if (!tumUrunKodu.Contains("GHS"))
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = pol.GenelBilgiler.PoliceSigortali.KimlikNo;
                                }
                            }

                            if (cell.ColumnIndex == 35)
                            {


                                pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = row.GetCell(35).NumericCellValue.ToString();
                                //pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.NumericCellValue.ToString();
                                if (!tumUrunKodu.Contains("GHS"))
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                }
                                sEttirenKimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                if (tumUrunKodu.Contains("GHS"))
                                {
                                    sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo) ? pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                }

                                if(sEttirenKimlikNo == null)
                                {

                                }
                            }


                        }

                        //sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        //sEttirenKimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;

                        pol.GenelBilgiler.Durum = 0;
                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                        if (PoliceBransEslestir != null)
                        {
                            pol.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                            pol.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                            bransKod = PoliceBransEslestir.BransKodu;
                            if (tumUrunAdi == null)
                            {
                                pol.GenelBilgiler.TUMUrunAdi = PoliceBransEslestir.TUMUrunAdi;
                            }
                            else
                            {
                                pol.GenelBilgiler.TUMUrunAdi = tumUrunAdi;
                            }

                            if (tumUrunKodu == null)
                            {
                                pol.GenelBilgiler.TUMUrunKodu = PoliceBransEslestir.TUMUrunKodu;
                            }
                            else
                            {
                                pol.GenelBilgiler.TUMUrunKodu = tumUrunKodu;
                            }

                            pol.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                            pol.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;

                        }
                        if (pol.GenelBilgiler.PoliceNumarasi == null)
                        {
                            continue;
                        }

                        policeler.Add(pol);

                        odemePol = pol;
                    }


                }
            }
            catch (Exception ex)
            {
                this.message = ex.ToString();
                policeler = null;
            }
            return policeler;

        }
        public string getMessage()
        {
            return this.message;
        }
    }
}

