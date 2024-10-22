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


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    //SFS programini kullanan ecente. Excel kolon sirasi ve isimleri farklilik gosteriyor !!!!
    // SFS excel tek bir sinif ile kullanilamadi!!!!!!!!
    // SFSExcel????? classlari bu nedenden dolayi hazirlandi.

    class SFSExcelAnkara
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
                                            "P Poliçe No",                  //0
                                            "P Zeyil No",              //1  
                                            "P Yenileme No",          //2      
                                            "P Tanzim Tarihi",         //3
                                            "P Onay Tarihi",                //4
                                            "P Baş.Tarih",              //5
                                            "P Bit. Tarihi",                //6  
                                            "P İptal Tarihi",                //7
                                            "U Sigortalı Adı",                 //8
                                            "U Sig. Adresi",                  //9
                                            "U Sig. İl",            //10    
                                            "U Sig. Vergi Numarası",           //11
                                            "U Sig. TC Kimlik No",               //12
                                            "U Müşteri Adı",          //13
                                            "U Müş. Adresi",              //14
                                            "U Müş. İl",                //15
                                            "U Müş. Vergi Numarası",           //16
                                            "U Müş. TC Kimlik No",           //17
                                            "P Riziko Adresi",              //18
                                            "P Ürün No",                   //19
                                            "P Ürün Adı",              //20    
                                            "P Plaka",             //21
                                            "P Brüt Prim",             //22
                                            "P Komisyon",                      //23
                                            "P GDV",                   //24
                                            "P GF",           //25
                                            "P THGF",               //26
                                            "P YSV",                 //27
                                            "P Döviz Cinsi",  //28
                                            "P Döviz Kuru",           //29
                                            "P Peşin (C) / Vadeli (I)",            //30
                                            "P Taksit Sayısı",     //31
                                            "P Taksit Tarihi",        //32
                                            "P Taksit Tutarı",         //33
                                            "T Taksit Ödeme Miktarı",      //34
                                            "T Taksit Ödeme Tarihi",                  //35
                                            "P Eski Poliçe No",               //36
                                            "P Net Prim",                 //37
                                            "U Sig. Vergi Dairesi",                  //38
                                            "SYS Kullanıcı Adı",            //39
                                            "MARKA",    //40
                                            "MARKA KODU",     //41
                                            "MOTOR NO",      //42
                                            "ARAÇ ŞASİ NO",      //43
                                            "TİPİ",                   //44
                                            "İMAL YILI (MODEL)",                       //45
                                            "KULLANIM TARZI",  //46
                                            "U Sig. Telefon1",             //47
                                            "U Sig. Telefon2",               //48
                                            "U Sig. Doğum Tarihi",           //49
                                            "U Sig. Doğum Yeri",           //50
                                            "P Zeyl Tipi",          //51
                                            "P Sanal POS?"             //52
                                                        
                                        };

        public SFSExcelAnkara(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization
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
            decimal dovizKuru = 1;

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
            string psliKimlikNo = null;
            decimal? polKomisyon = null;
            string psliVknNo = null;
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
            ISheet sheet = wb.GetSheet("Sheet1");

            // check sheet correct... 
            int startRow = checkSheetCorrect(sheet);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }
            int indx;
            string temprowvalue = "";
            // sheet correct. Start to get rows...
            try
            {
                for (indx = startRow; indx <= sheet.LastRowNum; indx++)
                {
                    decimal? polBrutprimim = null;
                    IRow row = sheet.GetRow(indx);
                    temprowvalue = "";
                    // null rowlar icin
                    if (row == null) continue;
                    try
                    {
                        if (row.FirstCellNum == 0)
                            temprowvalue = row.GetCell(0).ToString();
                    }
                    catch (Exception e)
                    {
                    }
                    // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                    if (row.FirstCellNum == 0 && temprowvalue == "P Brüt Prim") break;

                    // Police genel bilgileri icin. Police genel bilgiler aliniyor.
                    if (row.FirstCellNum == 0)
                    {
                        carpan = 1;
                        Police pol = new Police();
                        PoliceOdemePlani odm = new PoliceOdemePlani();
                        pol.GenelBilgiler.OdemeSekli = 1; // pesin

                        // tvm kodu
                        pol.GenelBilgiler.TVMKodu = tvmKodu;

                        // Birlik Kodu
                        pol.GenelBilgiler.TUMBirlikKodu = birlikKodu;

                        List<ICell> cels = row.Cells;

                        foreach (ICell cell in cels)
                        {

                            if (cell.ColumnIndex == 0) pol.GenelBilgiler.PoliceNumarasi = cell.ToString();
                            policemNo = pol.GenelBilgiler.PoliceNumarasi;

                            if (cell.ColumnIndex == 1) pol.GenelBilgiler.EkNo = Util.toInt(row.GetCell(1).StringCellValue);
                            ekNom = pol.GenelBilgiler.EkNo;

                            if (cell.ColumnIndex == 2) pol.GenelBilgiler.YenilemeNo = Util.toInt(cell.StringCellValue);
                            if (cell.ColumnIndex == 3) pol.GenelBilgiler.TanzimTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                            if (cell.ColumnIndex == 5) pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                            polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;

                            if (cell.ColumnIndex == 6) pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);

                            //  if (cell.ColumnIndex == 7) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                            if (cell.ColumnIndex == 8) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                            if (cell.ColumnIndex == 9) pol.GenelBilgiler.PoliceSigortali.Adres = cell.StringCellValue;
                            if (cell.ColumnIndex == 10) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.StringCellValue;
                            if (cell.ColumnIndex == 11) pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.StringCellValue;

                            if (cell.ColumnIndex == 12) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue;

                            if (cell.ColumnIndex == 13) pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                            if (cell.ColumnIndex == 14) pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.StringCellValue;
                            if (cell.ColumnIndex == 15) pol.GenelBilgiler.PoliceSigortaEttiren.IlAdi = cell.StringCellValue;
                            if (cell.ColumnIndex == 16) pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.StringCellValue;
                            if (cell.ColumnIndex == 17) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue;
                            psliKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            psliVknNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;


                            if (cell.ColumnIndex == 19) tumUrunKodu = cell.StringCellValue;

                            #region Araç Bilgileri
                            if (cell.ColumnIndex == 21)
                            {
                                pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(2, cell.StringCellValue.Length - 2) : "";
                                pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(0, 2) : "";

                            }
                            if (cell.ColumnIndex == 41)
                            {
                                pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue;
                            }
                            if (cell.ColumnIndex == 42)
                            {
                                pol.GenelBilgiler.PoliceArac.MotorNo = cell.StringCellValue;
                            }
                            if (cell.ColumnIndex == 43)
                            {
                                pol.GenelBilgiler.PoliceArac.SasiNo = cell.StringCellValue;
                            }
                            if (cell.ColumnIndex == 44)
                            {
                                pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = cell.StringCellValue;
                            }
                            if (cell.ColumnIndex == 45)
                            {
                                pol.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToInt32(cell.StringCellValue) : 0;
                            }
                            if (cell.ColumnIndex == 46)
                            {
                                pol.GenelBilgiler.PoliceArac.KullanimTarzi = cell.StringCellValue;
                            }
                            #endregion

                            if (cell.ColumnIndex == 22)
                            {
                                pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                                polBrutprimim = pol.GenelBilgiler.BrutPrim;

                                if (pol.GenelBilgiler.BrutPrim < 0)
                                {
                                    carpan = -1;


                                }
                            }

                            if (cell.ColumnIndex == 23)
                            {
                                pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.NumericCellValue.ToString());
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                                    pol.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
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
                            }
                            if (cell.ColumnIndex == 25)
                            {
                                // Garanti fonu
                                PoliceVergi gf = new PoliceVergi();
                                gf.VergiKodu = 3;
                                gf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                pol.GenelBilgiler.ToplamVergi += gf.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(gf);
                            }
                            if (cell.ColumnIndex == 26)
                            {
                                // THGF 
                                PoliceVergi thgf = new PoliceVergi();
                                thgf.VergiKodu = 1;
                                thgf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                pol.GenelBilgiler.ToplamVergi += thgf.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(thgf);
                            }
                            if (cell.ColumnIndex == 27)
                            {
                                // YSV 
                                PoliceVergi ysv = new PoliceVergi();
                                ysv.VergiKodu = 4;
                                ysv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(ysv);
                            }

                            if (cell.ColumnIndex == 28)
                            {
                                pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                                if (pol.GenelBilgiler.ParaBirimi == "YTL")
                                {
                                    pol.GenelBilgiler.ParaBirimi = "TL";
                                }
                            }
                            if (cell.ColumnIndex == 29)
                            {
                                if (!String.IsNullOrEmpty(cell.StringCellValue))
                                {
                                    pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.StringCellValue.Replace(".", ","));
                                    dovizKuru = Util.ToDecimal(cell.StringCellValue.Replace(".", ","));

                                }
                            }
                            // odeme plani
                            if (cell.ColumnIndex == 32 || cell.ColumnIndex == 33)
                            {
                                if (cell.ColumnIndex == 32)
                                {
                                    // Odeme Plani - ilk taksit
                                    taksitNo = 1;
                                    odm.TaksitNo = taksitNo;
                                    odm.VadeTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                                }
                                if (cell.ColumnIndex == 33)
                                {
                                    odm.TaksitTutari = carpan * Util.ToDecimal(cell.NumericCellValue.ToString()); // iptal ise tutuar eksi deger olmali
                                    if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                    {
                                        odm.TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString()) * carpan;
                                        odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(cell.NumericCellValue.ToString()) / pol.GenelBilgiler.DovizKur.Value * carpan, 2);
                                    }

                                    PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                    PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                    pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                    pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
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
                                            odm.DovizliTaksitTutari = pol.GenelBilgiler.BrutPrim.Value;
                                        }
                                        if (odm.VadeTarihi == null)
                                        {
                                            odm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                                        }

                                        odm.TaksitNo = 1;

                                        pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }
                                    #region Tahsilat işlemi

                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.ANKARASIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
                                            // tahsilat.OdemeBelgeNo = "111111";
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

                            if (cell.ColumnIndex == 37)
                            {
                                pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                                if (pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.BrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.NetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.Komisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    polBrutprimim = pol.GenelBilgiler.BrutPrim;
                                }
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliNetPrim = pol.GenelBilgiler.NetPrim.Value;
                                }
                            }

                            if (cell.ColumnIndex == 51) pol.GenelBilgiler.ZeyilAdi = cell.StringCellValue;
                        }
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
                        if (bransKod == 1 || bransKod == 2)
                        {
                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                        }
                        else
                        {
                            odm.OdemeTipi = OdemeTipleri.Havale;
                        }
                        odm.VadeTarihi = Util.toDate(row.GetCell(32).StringCellValue, Util.DateFormat1);
                        try
                        {
                            if (row.LastCellNum >= 34)
                                odm.TaksitTutari = carpan * Util.ToDecimal(row.GetCell(33).ToString());
                            else
                                odm.TaksitTutari = 0;
                        }
                        catch (Exception)
                        {
                            odm.TaksitTutari = 0;
                        }
                        //odm.TaksitTutari = carpan * Util.ToDecimal(row.GetCell(33).ToString()); // iptal ise tutuar eksi deger olmali
                        if (odemePol.GenelBilgiler.DovizKur != 1 && odemePol.GenelBilgiler.DovizKur != 0 && odemePol.GenelBilgiler.DovizKur != null)
                        {
                            odm.TaksitTutari = Util.ToDecimal(row.GetCell(33).NumericCellValue.ToString()) * carpan;
                            odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(row.GetCell(33).NumericCellValue.ToString()) / odemePol.GenelBilgiler.DovizKur.Value * carpan, 2);
                        }
                        odemePol.GenelBilgiler.OdemeSekli = 2; // Vadeli - taksit varsa vadeli

                        if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                        {
                            #region Tahsilat işlemi

                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.ANKARASIGORTA, odemePol.GenelBilgiler.BransKodu.Value);
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
                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : odemePol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.PoliceNo = odemePol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = odemePol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                        tahsilat.BrutPrim = odemePol.GenelBilgiler.BrutPrim.HasValue ? odemePol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = odemePol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = odemePol.GenelBilgiler.TVMKodu.Value;
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
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
                                    // tahsilat.OdemeBelgeNo = "111111";
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = 0;
                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.PoliceNo = policemNo;
                                    tahsilat.ZeyilNo = ekNom.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
            }
            catch (Exception e)
            {
            }

            return policeler;
        }

        public string getMessage()
        {
            return this.message;
        }

        private int checkSheetCorrect(ISheet sht)
        {
            for (int indx = 0; indx <= sht.LastRowNum; indx++)
            {
                IRow row = sht.GetRow(indx);
                if (row == null) continue;
                if (row.FirstCellNum != 0) continue;
                //check column sequenc is correct !!!
                if (row.GetCell(0).StringCellValue == columnNames[0])
                {
                    if (checkColumnSequence(row) == true)
                    {
                        return indx + 1;
                    }
                }
            }

            return -1;
        }

        private bool checkColumnSequence(IRow row)
        {
            bool tf = false;

            for (int indx = 0; indx < columnNames.Length; indx++)
            {
                string colName = row.GetCell(indx).StringCellValue;
                if (colName.Equals(columnNames[indx]))
                    tf = true;
                else
                {
                    tf = false;
                    break;
                }

            }
            return tf;
        }

    }
}
