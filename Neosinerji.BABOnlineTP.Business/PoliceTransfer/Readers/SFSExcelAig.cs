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
using NPOI.XSSF.UserModel;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    //SFS programini kullanan ecente. Excel kolon sirasi ve isimleri farklilik gosteriyor !!!!
    // SFS excel tek bir sinif ile kullanilamadi!!!!!!!!
    // SFSExcel????? classlari bu nedenden dolayi hazirlandi.

    class SFSExcelAig
    {
        HSSFWorkbook wb;
        ITVMService _TVMService;
        private string message = string.Empty;
        private string excelFileName;
        private string birlikKodu;
        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;

        private string[] columnNames =  {
                            "P Poliçe No", //0    " Poliçe No,      
                            "P Zeyil No", //1    "P Zeyil No",
                            "P Yenileme No", //2    "P Baş.Tarih",
                            "P Tanzim Tarihi", //3    "P Bit. Tarihi",
                            "P Onay Tarihi", //4    "P Tanzim Tarihi",
                            "P Baş.Tarih", //5    "P Brüt Prim",
                            "P Bit. Tarihi", //6    "P Net Prim",
                            "P İptal Tarihi", //7    "P GDV",
                            "U Si/rtalı Adı", //8    "P GF",
                            "U Sig. Adresi", //9    "P THGF",
                            "U Sig. İl", //10	"P YSV",
                            "U Sig. Vergi Numarası", //11	"U Müşteri No",
                            "U Sig. TC Kimlik No", //12	"U Müşteri Adı",
                            "U Müşteri Adı", //13	"P Plaka",
                            "U Müş. Adresi", //14	"P Döviz Cinsi",
                            "U Müş. İl", //15	"U Si/rtalı Adı",
                            "U Müş. Vergi Numarası", //16	"P Taksit Tarihi",
                            "U Müş. TC Kimlik No", //17	"P Taksit Tutarı",
                            "P Riziko Adresi", //18	"P Ürün No",
                            "P Ürün No", //19	"P Yenileme No",
                            "P Ürün Adı", //20	"P Komisyon",
                            "P Plaka", //21	"P Riziko Adresi",
                            "P Brüt Prim", //22	"SON Kullanıcı Adı",
                            "P Komisyon", //23	"P Döviz Kuru",
                            "P GDV", //24	"U Müş. Adresi",
                            "P GF", //25	"U Müş. TC Kimlik No",
                            "P THGF", //26	"U Müş. Vergi Numarası",
                            "P YSV", //27	"U Müş. Doğum Tarihi",
                            "P Döviz Cinsi", //28	"U Müş. E-posta Adresi",
                            "P Döviz Kuru", //29	"P Ürün Adı",
                            "P Peşin (C) / Vadeli (I)", //30	"Tipi",
                            "P Taksit Sayısı", //31	"ARAÇ-1 MARKASI",
                            "P Taksit Tarihi", //32	"MOTOR NO",
                            "P Taksit Tutarı", //33	"ŞASİ NO",
                            "T Taksit Ödeme Miktarı", //34	"TESCİL BELGESİ TARİHİ",
                            "T Taksit Ödeme Tarihi", //35	"ARAÇ-1 KULLANIM SEKLI"
                            "P Eski Poliçe No", //36	
                            "P Net Prim", //37	
                            "U Sig. Vergi Dairesi", //38	
                            "SON Kullanıcı Adı", //39	
                            "MARKA KODU", //40	
                            "ŞASİ NO", //41	
                            "MOTOR NO", //42
                            "MODEL YILI",//43
                            "ARAÇ ADI",//44
                            "ARAÇ TİPİ", //45	
                            "MARKA KOD1", //46
                            "MARKA KOD2", //47
                            "P Sanal POS?"  //48

                                            
                                        };


        public SFSExcelAig(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            int? bransKod = null;
            string policemNo = null;
            DateTime? polBasTarihi = null;
            int? ekNom = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            decimal? polKomisyon = null;

            try
            {
                excelFile = new FileStream(excelFileName, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }
            ISheet sheet = null;
            if (excelFileName.Contains(".xlsx"))
            {
                XSSFWorkbook wb1 = new XSSFWorkbook(excelFile);
                string namesheet = wb1.NumberOfSheets > 0 ? wb1.GetSheetName(0):"veri";
                sheet = wb1.GetSheet(namesheet);
            }
            else
            {
                wb = new HSSFWorkbook(excelFile);
                string namesheet = wb.NumberOfSheets > 0 ? wb.GetSheetName(0) : "veri";
                sheet = wb.GetSheet(namesheet);
            }

            //wb = new HSSFWorkbook(excelFile);
            //ISheet sheet = wb.GetSheet("Sheet1");

            // check sheet correct... 
            int startRow = Util.checkSheetCorrect(sheet, columnNames);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }

            // sheet correct. Start to get rows...
            for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
            {
                decimal? polBrutprimim = null;
                IRow row = sheet.GetRow(indx);

                // null rowlar icin
                if (row == null) continue;

                // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                if (row.FirstCellNum == 0 && row.GetCell(0).ToString() == "P Brüt Prim") break;

                // Police genel bilgileri icin. Police genel bilgiler aliniyor.
                if (row.FirstCellNum == 0)
                {

                    Police pol = new Police();
                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    pol.GenelBilgiler.OdemeSekli = 1; // pesin
                    carpan = 1;

                    // tvm kodu
                    pol.GenelBilgiler.TVMKodu = tvmKodu;

                    // Birlik Kodu
                    pol.GenelBilgiler.TUMBirlikKodu = birlikKodu;

                    List<ICell> cels = row.Cells;

                    foreach (ICell cell in cels)
                    {

                        if (cell.ColumnIndex == 0) pol.GenelBilgiler.PoliceNumarasi = cell.ToString();
                        policemNo = pol.GenelBilgiler.PoliceNumarasi;

                        if (cell.ColumnIndex == 1) pol.GenelBilgiler.EkNo = Util.toInt(row.GetCell(1).ToString());
                        ekNom = pol.GenelBilgiler.EkNo;

                        if (cell.ColumnIndex == 2) pol.GenelBilgiler.YenilemeNo = Util.toInt(cell.ToString());
                        //if (cell.ColumnIndex == 3) pol.GenelBilgiler.TanzimTarihi = Util.toDate(cell.ToString(), Util.DateFormat1);
                        if (cell.ColumnIndex == 3) pol.GenelBilgiler.TanzimTarihi = Convert.ToDateTime(cell.DateCellValue.ToString());
                        //   if (cell.ColumnIndex == 4) pol.GenelBilgiler.onaytarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        //if (cell.ColumnIndex == 5) pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.ToString(), Util.DateFormat1);
                        if (cell.ColumnIndex == 5) pol.GenelBilgiler.BaslangicTarihi = Convert.ToDateTime(cell.DateCellValue.ToString());
                        polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;

                        //if (cell.ColumnIndex == 6) pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.ToString(), Util.DateFormat1);  // 7. satır mevcut değil                      
                        if (cell.ColumnIndex == 6) pol.GenelBilgiler.BitisTarihi = Convert.ToDateTime(cell.DateCellValue.ToString());  // 7. satır mevcut değil                      
                        if (cell.ColumnIndex == 8) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.ToString();
                        if (cell.ColumnIndex == 9) pol.GenelBilgiler.PoliceSigortali.Adres = cell.ToString();
                        if (cell.ColumnIndex == 10) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.ToString();
                        if (cell.ColumnIndex == 11)
                        {
                            if (cell.ToString().Length==10)
                            {
                                pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.ToString();
                            }
                        }

                        if (cell.ColumnIndex == 12) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.ToString();

                        if (cell.ColumnIndex == 13) pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.ToString();
                        if (cell.ColumnIndex == 14) pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.ToString() != null ? cell.ToString() : null;
                        if (cell.ColumnIndex == 15) pol.GenelBilgiler.PoliceSigortaEttiren.IlAdi = cell.ToString() != null ? cell.ToString() : null;
                        if (cell.ColumnIndex == 16)
                        {
                            if (cell.ToString().Length == 10)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.ToString();
                            }
                        }
                        if (cell.ColumnIndex == 17) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.ToString();
                        if (cell.ColumnIndex == 18) pol.GenelBilgiler.PoliceRizikoAdresi.Adres = cell.ToString() != null ? cell.ToString() : null;
                        if (cell.ColumnIndex == 19) tumUrunKodu = cell.ToString();
                        if (cell.ColumnIndex == 20) tumUrunAdi = cell.ToString();
                        sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                        if (cell.ColumnIndex == 21)
                        {
                            pol.GenelBilgiler.PoliceArac.PlakaNo = cell.ToString() != "" && cell.ToString().Length >= 2 ? cell.ToString().Substring(2, cell.ToString().Length - 2) : "";
                            pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.ToString() != "" && cell.ToString().Length >= 2 ? cell.ToString().Substring(0, 2) : "";
                        }
                        if (cell.ColumnIndex == 22)
                        {
                            pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.ToString());
                            polBrutprimim = pol.GenelBilgiler.BrutPrim;

                            if (pol.GenelBilgiler.BrutPrim < 0)
                            {
                                carpan = -1;
                                //pol.GenelBilgiler.BrutPrim = pol.GenelBilgiler.BrutPrim * carpan;
                                //pol.GenelBilgiler.NetPrim = pol.GenelBilgiler.NetPrim * carpan;
                                //pol.GenelBilgiler.Komisyon = pol.GenelBilgiler.Komisyon * carpan;
                                //pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi * carpan;
                                //foreach (var item in pol.GenelBilgiler.PoliceVergis)
                                //{
                                //    item.VergiTutari = item.VergiTutari * carpan;
                                //}
                                //foreach (var item in pol.GenelBilgiler.PoliceOdemePlanis)
                                //{
                                //    item.TaksitTutari = item.TaksitTutari * carpan;
                                //}
                            }
                        }
                        if (cell.ColumnIndex == 23)
                        {
                            pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.ToString());
                            polKomisyon = pol.GenelBilgiler.Komisyon;
                        }
                        #region Vergiler
                        if (cell.ColumnIndex == 24)
                        {
                            // Gider Vergisi
                            pol.GenelBilgiler.ToplamVergi = 0;
                            PoliceVergi gv = new PoliceVergi();
                            gv.VergiKodu = 2;
                            gv.VergiTutari = Util.ToDecimal(cell.ToString());
                            pol.GenelBilgiler.ToplamVergi += gv.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gv);
                        }
                        if (cell.ColumnIndex == 25)
                        {
                            // Garanti fonu
                            PoliceVergi gf = new PoliceVergi();
                            gf.VergiKodu = 3;
                            gf.VergiTutari = Util.ToDecimal(cell.ToString());
                            pol.GenelBilgiler.ToplamVergi += gf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gf);
                        }
                        if (cell.ColumnIndex == 26)
                        {
                            // THGF 
                            PoliceVergi thgf = new PoliceVergi();
                            thgf.VergiKodu = 1;
                            thgf.VergiTutari = Util.ToDecimal(cell.ToString());
                            pol.GenelBilgiler.ToplamVergi += thgf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(thgf);
                        }
                        if (cell.ColumnIndex == 27)
                        {
                            // YSV 
                            PoliceVergi ysv = new PoliceVergi();
                            ysv.VergiKodu = 4;
                            ysv.VergiTutari = Util.ToDecimal(cell.ToString());
                            pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(ysv);
                        }
                        #endregion
                        if (cell.ColumnIndex == 28)
                        {
                            pol.GenelBilgiler.ParaBirimi = cell.ToString();
                            if (pol.GenelBilgiler.ParaBirimi == "YTL")
                            {
                                pol.GenelBilgiler.ParaBirimi = "TL";
                            }
                        }
                        if (cell.ColumnIndex == 29)
                        {
                            if (!String.IsNullOrEmpty(cell.ToString()))
                            {
                                pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.ToString().Replace(".", ","));
                            }
                        }
                        if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                        {
                            pol.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value / pol.GenelBilgiler.DovizKur;
                            pol.GenelBilgiler.DovizliKomisyon = polKomisyon.Value / pol.GenelBilgiler.DovizKur;
                        }

                        //if (cell.ColumnIndex == 31) odm.TaksitNo = cell.StringCellValue;
                        // odeme plani
                        if (cell.ColumnIndex == 32 || cell.ColumnIndex == 33)
                        {
                            if (cell.ColumnIndex == 32)
                            {
                                // Odeme Plani - ilk taksit
                                taksitNo = 1;
                                odm.TaksitNo = taksitNo;
                               // odm.VadeTarihi = Util.toDate(cell.DateCellValue.ToString(), Util.DateFormat1);
                                odm.VadeTarihi = Convert.ToDateTime(cell.DateCellValue.ToString());
                            }
                            if (cell.ColumnIndex == 33)
                            {
                                PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                                odm.TaksitTutari = carpan * Util.ToDecimal(cell.ToString()); // iptal ise tutuar eksi deger olmali
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odm.TaksitTutari = Util.ToDecimal(cell.ToString()) * carpan;
                                    odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) / pol.GenelBilgiler.DovizKur.Value * carpan, 2);
                                }
                                if (pol.GenelBilgiler.BransKodu.Value == 1 || pol.GenelBilgiler.BransKodu.Value == 2)
                                {
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                }
                                else
                                {
                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                }
                                if (odm.TaksitTutari != 0)
                                {
                                    pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                }
                                else if (odm.TaksitTutari == 0 && odm.TaksitNo == 1)
                                {
                                    odm.TaksitTutari = pol.GenelBilgiler.BrutPrim;
                                    if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                    {
                                        odm.DovizliTaksitTutari = pol.GenelBilgiler.DovizliBrutPrim.Value;
                                    }
                                    if (odm.VadeTarihi == null)
                                    {
                                        odm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                                    }

                                    odm.TaksitNo = 1;
                                    if (pol.GenelBilgiler.BransKodu.Value == 1 || pol.GenelBilgiler.BransKodu.Value == 2)
                                    {
                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    }
                                    else
                                    {
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                    }
                                    if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                                    {
                                        pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }
                                }
                                if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                                {
                                    #region Tahsilat işlemi
                                    sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.GULFSIGORTA, pol.GenelBilgiler.BransKodu.Value);
                                    if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                    {
                                        int otoOdeSayac = 0;
                                        foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                        {
                                            if (otoOdeSayac < 1 && pol.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                            {
                                                otoOdeSayac++;
                                                PoliceTahsilat tahsilat = new PoliceTahsilat();

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
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                                tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                                tahsilat.KayitTarihi = DateTime.Today;
                                                tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                                if (tahsilat.TaksitTutari != 0)
                                                {
                                                    pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        if (pol.GenelBilgiler.BransKodu.Value == 1 || pol.GenelBilgiler.BransKodu.Value == 2)
                                        {
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
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
                                            tahsilat.KimlikNo = pol.GenelBilgiler.PoliceSigortali.KimlikNo;
                                            tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.Value;
                                            tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                        else
                                        {
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = odm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                            //tahsilat.OdemeBelgeNo = "111111";
                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = pol.GenelBilgiler.PoliceSigortali.KimlikNo;
                                            tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.Value;
                                            tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                    }


                                    #endregion
                                }
                            }
                        }

                        if (cell.ColumnIndex == 37)
                        {
                            pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.ToString());

                            if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != null)
                            {
                                pol.GenelBilgiler.DovizliBrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value / pol.GenelBilgiler.DovizKur.Value, 2);
                                pol.GenelBilgiler.DovizliNetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value / pol.GenelBilgiler.DovizKur.Value, 2);
                                pol.GenelBilgiler.DovizliKomisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value / pol.GenelBilgiler.DovizKur.Value, 2);
                                polBrutprimim = pol.GenelBilgiler.BrutPrim;
                            }
                        }
                        if (cell.ColumnIndex == 40) pol.GenelBilgiler.PoliceArac.Marka = cell.ToString();
                        if (cell.ColumnIndex == 41) pol.GenelBilgiler.PoliceArac.SasiNo = cell.ToString();
                        if (cell.ColumnIndex == 42) pol.GenelBilgiler.PoliceArac.MotorNo = cell.ToString();
                        if (cell.ColumnIndex == 43) pol.GenelBilgiler.PoliceArac.Model = Util.toInt(cell.ToString());
                        if (cell.ColumnIndex == 44) pol.GenelBilgiler.PoliceArac.MarkaAciklama = cell.ToString();
                        if (cell.ColumnIndex == 45) pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = cell.ToString();
                        if (cell.ColumnIndex == 47) pol.GenelBilgiler.PoliceArac.Marka = cell.ToString() + row.GetCell(46).ToString();

                    }
                    pol.GenelBilgiler.Durum = 0;

                    PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                    PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

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

                    policeler.Add(pol);
                    odemePol = pol;

                }

                // Odeme planinin deiger taksitleri icin
                List<ICell> celsa = row.Cells;


                if (row.FirstCellNum == 32)
                {
                    // Odeme Plani - diger taksitler
                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    taksitNo += 1;
                    odm.TaksitNo = taksitNo;
                    //odm.VadeTarihi = Util.toDate(row.GetCell(32).ToString(), Util.DateFormat1);
                    odm.VadeTarihi = Convert.ToDateTime(row.GetCell(32).DateCellValue.ToString());
                    odm.TaksitTutari = carpan * Util.ToDecimal(row.GetCell(33).ToString()); // iptal ise tutuar eksi deger olmali
                    if (odemePol.GenelBilgiler.DovizKur != 1 && odemePol.GenelBilgiler.DovizKur != 0 && odemePol.GenelBilgiler.DovizKur != null)
                    {
                        odm.TaksitTutari = Util.ToDecimal(row.GetCell(34).ToString()) * carpan;
                        odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(row.GetCell(34).ToString()) / odemePol.GenelBilgiler.DovizKur.Value * carpan, 2);
                    }
                    odemePol.GenelBilgiler.OdemeSekli = 2; // Vadeli - taksit varsa vadeli
                    if (bransKod == 1 || bransKod == 2)
                    {
                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                    }
                    else
                    {
                        odm.OdemeTipi = OdemeTipleri.Havale;
                    }

                    if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                    {
                        #region Tahsilat işlemi

                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.GULFSIGORTA, bransKod.Value);
                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                        {
                            int otoOdeSayac = 0;
                            foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                            {
                                if (otoOdeSayac < 1 && bransKod == itemOtoOdemeTipleri.BransKodu)
                                {
                                    otoOdeSayac++;
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();

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
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.PoliceNo = policemNo;
                                    tahsilat.ZeyilNo = ekNom.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                    tahsilat.BrutPrim = odemePol.GenelBilgiler.BrutPrim.Value;
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

                            if (bransKod == 1 || bransKod == 2)
                            {
                                PoliceTahsilat tahsilat = new PoliceTahsilat();
                                tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                tahsilat.OtomatikTahsilatiKkMi = 1;
                                tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                tahsilat.TaksitNo = odm.TaksitNo;
                                tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                tahsilat.OdemeBelgeNo = "111111****1111";
                                tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilat.KalanTaksitTutari = 0;
                                tahsilat.PoliceNo = policemNo;
                                tahsilat.ZeyilNo = ekNom.ToString();
                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                tahsilat.BrutPrim = odemePol.GenelBilgiler.BrutPrim.Value;
                                tahsilat.PoliceId = odm.PoliceId;
                                tahsilat.KayitTarihi = DateTime.Today;
                                tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                                if (tahsilat.TaksitTutari != 0)
                                {
                                    odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                }
                            }
                            else
                            {
                                PoliceTahsilat tahsilat = new PoliceTahsilat();
                                tahsilat.OdemTipi = OdemeTipleri.Havale;
                                odm.OdemeTipi = OdemeTipleri.Havale;
                                tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                tahsilat.TaksitNo = odm.TaksitNo;
                                tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                //tahsilat.OdemeBelgeNo = "111111";
                                tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilat.OdenenTutar = 0;
                                tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilat.PoliceNo = policemNo;
                                tahsilat.ZeyilNo = ekNom.ToString();
                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                tahsilat.BrutPrim = odemePol.GenelBilgiler.BrutPrim.Value;
                                tahsilat.PoliceId = odm.PoliceId;
                                tahsilat.KayitTarihi = DateTime.Today;
                                tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                                if (tahsilat.TaksitTutari != 0)
                                {
                                    odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                }
                            }
                        }

                        #endregion
                        odemePol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                    }

                }

            }

            return policeler;
        }

        public string getMessage()
        {
            return this.message;
        }
    }
}
