using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;


using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    class GencExcelReader : IGencPoliceTransferReader
    {
        HSSFWorkbook wb;
        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        IAktifKullaniciService _AktifKullaniciService;


        private string message = string.Empty;
        private string filePath;


        #region colon adları
        private string[] columnNames =  {
                                          //  "Sirket_kodu",               
                                          "sirket_adi",                     // 0      //0    	
                                          "Neo_Urun_adi",                   // 1      //1 brnk
                                          "tanzim",                         // 2      //3    
                                          "policeno",                       // 3      //4    
                                          "zeyilno",                        // 4      //5    
                                          "yenileme_no",                    // 5      //6    
                                          "Sigortalı_adı",                  // 6      //8 ***
                                          "Sigortalı_soyadı",			    // 7	  //9
                                          "sigortalivergino",               // 8      //10 ***
                                          "sigortaliadres",                 // 9      //11 **
                                          "Sig_Ettiren_adı",                // 10     //12 **
                                          "Sig_Ettiren_soyadı",			    // 11	  //13
                                          "sig_ettireneadres",              // 12     //14 **
                                          "sig_Ett_vergino",                // 13     //15 **
                                          "plaka_ili",                      // 14     //16   
                                          "plaka_no",                       // 15     //17  
                                          "marka",                          // 16     //18   
                                          "tip",                            // 17     //19
                                          "model",                          // 18     //20
                                          "rizikoadr",                      // 19     //21
                                          "baslangic",                      // 20     //22
                                          "bitis",                          // 21     //23  
                                          "doviztipi",                      // 22     //25  
                                          "tckimlik_Sigortali",             // 23     //27 
                                          "tckimlik_SigortaEtt",            // 24     //28 	
                                          "netprim",                        // 25     //41
                                          "brutprim",                       // 26     //43  
                                          "policekomisyonu",                // 27     //48  
                                          "motorno",                        // 28     //59  
                                          "sasino",                         // 29     //60  
                                          "vadetar1",                       // 30     //66  
                                          "vadetut1",                       // 31     //67
                                          "vadetar2",                       // 32     //68
                                          "vadetut2",                       // 33     //69
                                          "vadetar3",                       // 34     //70
                                          "vadetut3",                       // 35     //71
                                          "vadetar4",                       // 36     //72
                                          "vadetut4",                       // 37     //73
                                          "vadetar5",                       // 38     //74
                                          "vadetut5",                       // 39     //75
                                          "vadetar6",                       // 40     //76
                                          "vadetut6",                       // 41     //77
                                          "vadetar7",                       // 42     //78
                                          "vadetut7",                       // 43     //79
                                          "vadetar8",                       // 44     //80
                                          "vadetut8",                       // 45     //81
                                          "vadetar9",                       // 46     //82
                                          "vadetut9",                       // 47     //83
                                          "vadetar10",                      // 48     //84
                                          "vadetut10",                      // 49     //85
                                          "vadetar11",                      // 50     //86
                                          "vadetut11",                      // 51     //87
                                          "vadetar12",                      // 52     //88
                                          "vadetut12" ,                      // 53     //89
                                          
                                          //  "rizikoadr"                            //54   //88
                                        };

        #endregion

        public GencExcelReader(string path, int tvmKodu, List<Bran> branslar)
        {

            this.filePath = path;
            this.tvmKodu = tvmKodu;
            this._branslar = branslar;
        }

        public GencListeler getPoliceler()
        {
            List<Police> policeler = new List<Police>();
            List<MusteriGenelBilgiler> musteriler = new List<MusteriGenelBilgiler>();

            GencListeler listeler = new GencListeler();
            listeler.policeListesi = new List<Police>();
            listeler.musteriListesi = new List<MusteriGenelBilgiler>();

            ITVMKullanicilarService _TVMKullanicilarService = DependencyResolver.Current.GetService<TVMKullanicilarService>();

            // get excel file...
            FileStream excelFile = null;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string musteriAdSoyad = null;
            string musteriVergiNo = null;
            string musteriTcNo = null;

            try
            {
                excelFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }

            XSSFWorkbook wb = (XSSFWorkbook)WorkbookFactory.Create(excelFile);
            //wb = new HSSFWorkbook(excelFile);
            ISheet sheet = wb.GetSheet("NeoOnline");

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
                IRow row = sheet.GetRow(indx);

                // null rowlar icin
                if (row == null) break;
                if (row.FirstCellNum == 0 && row.GetCell(0).StringCellValue == "Sigorta Şirket İsmlendirmeleri") break;

                if (row.FirstCellNum == 0)
                {
                    MusteriGenelBilgiler mus = new MusteriGenelBilgiler();

                    Police pol = new Police();
                    PoliceOdemePlani[] odemeler = { new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani() };

                    // Birlik Kodu
                    // pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AKSIGORTA;
                    // tvm kodu neosinerji için 100 genc test 116 canli 118
                    pol.GenelBilgiler.TVMKodu = tvmKodu;
                    pol.GenelBilgiler.Durum = 3;

                    List<ICell> cels = row.Cells;
                    pol.GenelBilgiler.YenilemeNo = 0;
                    pol.GenelBilgiler.EkNo = 0;
                    foreach (ICell cell in cels)
                    {
                        int colIndex = cell.ColumnIndex;
                        if (cell.ColumnIndex == 0) pol.GenelBilgiler.TUMBirlikKodu = Util.ConvertToTUMBirlikKod(cell.StringCellValue);
                        if (cell.ColumnIndex == 1) tumUrunAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 2)
                        {
                            try
                            {
                                pol.GenelBilgiler.TanzimTarihi = Convert.ToDateTime(cell.DateCellValue.ToString());
                            }

                            catch (Exception)
                            {
                                pol.GenelBilgiler.TanzimTarihi = Convert.ToDateTime(cell.StringCellValue.ToString());
                            }

                        }


                        if (cell.ColumnIndex == 3)
                        {
                            try
                            {
                                if (cell.StringCellValue != null)
                                    pol.GenelBilgiler.PoliceNumarasi = cell.StringCellValue.Trim();
                            }
                            catch (Exception)
                            {
                                pol.GenelBilgiler.PoliceNumarasi = cell.NumericCellValue.ToString();
                            }

                        }
                        if (cell.ColumnIndex == 4)
                            try
                            {

                                if (cell.StringCellValue != null && cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.EkNo = Convert.ToInt32(cell.StringCellValue.Trim());
                                }
                                else
                                {
                                    pol.GenelBilgiler.EkNo = 0;
                                }
                            }
                            catch (Exception)
                            {
                                pol.GenelBilgiler.EkNo = Convert.ToInt32(cell.NumericCellValue);
                            }
                        if (cell.ColumnIndex == 5)
                            try
                            {
                                if (cell.StringCellValue != null && cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.YenilemeNo = Convert.ToInt32(cell.StringCellValue.Trim());
                                }
                                else
                                {
                                    pol.GenelBilgiler.YenilemeNo = 0;
                                }
                            }
                            catch (Exception)
                            {
                                pol.GenelBilgiler.YenilemeNo = Convert.ToInt32(cell.NumericCellValue);
                            }

                        if (cell.ColumnIndex == 6)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;

                            }

                        }

                        if (cell.ColumnIndex == 7)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan = cell.StringCellValue;
                                if (pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan.Length > 49)
                                    pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan = pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan.Substring(0, 49);
                            }

                        }


                        if (cell.ColumnIndex == 8)
                        {
                            try
                            {
                                if (cell.StringCellValue != null && cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.StringCellValue;
                                }
                            }
                            catch (Exception)
                            {
                                pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.NumericCellValue.ToString();
                            }
                        }
                        if (cell.ColumnIndex == 9)
                        {
                            pol.GenelBilgiler.PoliceSigortali.Adres = cell.StringCellValue != null ? cell.StringCellValue : ".";
                        }
                        if (cell.ColumnIndex == 10)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;

                            }

                        }
                        if (cell.ColumnIndex == 11)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = cell.StringCellValue;
                                if (pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan.Length > 49)
                                    pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan.Substring(0, 49);

                            }

                        }
                        if (cell.ColumnIndex == 12)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.StringCellValue;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.Adres = ".";

                            }
                        }
                        if (cell.ColumnIndex == 13)
                        {
                            try
                            {
                                if (cell.StringCellValue != null && cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.StringCellValue;
                                }
                            }
                            catch (Exception)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.NumericCellValue.ToString();

                            }
                        }
                        if (cell.ColumnIndex == 14)
                        {

                            try
                            {
                                if (cell.StringCellValue != null && cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue;
                                }
                            }
                            catch (Exception)
                            {
                                pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.NumericCellValue.ToString();

                            }

                        }
                        if (cell.ColumnIndex == 15)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue;
                            }

                        }


                        if (cell.ColumnIndex == 16)
                        {
                            if (cell.StringCellValue.Length > 25)
                            {
                                pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue.Substring(0, 25);
                            }
                            else if (cell.StringCellValue.Length < 25 && cell.StringCellValue.Length > 2)
                            {
                                pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceArac.Marka = ".";
                            }

                        }
                        if (cell.ColumnIndex == 17)
                        {
                            if (cell.StringCellValue.Length > 20)
                            {
                                pol.GenelBilgiler.PoliceArac.AracinTipiKodu = cell.StringCellValue.Substring(0, 20);
                            }
                            else if (cell.StringCellValue.Length < 20 && cell.StringCellValue.Length > 0)
                            {
                                pol.GenelBilgiler.PoliceArac.AracinTipiKodu = cell.StringCellValue;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceArac.AracinTipiKodu = ".";
                            }

                        }
                        if (cell.ColumnIndex == 18)
                        {
                            try
                            {
                                if (cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.PoliceArac.Model = Util.toInt(cell.StringCellValue);
                                }
                                else
                                {
                                    pol.GenelBilgiler.PoliceArac.Model = 0000;

                                }
                            }
                            catch (Exception)
                            {
                                pol.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(cell.NumericCellValue);
                            }
                        }
                        if (cell.ColumnIndex == 19)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceRizikoAdresi.Adres = cell.StringCellValue;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceRizikoAdresi.Adres = "Belirtilmedi";
                            }
                        }
                        if (cell.ColumnIndex == 20)
                        {
                            try
                            {
                                pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.DateCellValue.ToString());
                            }

                            catch (Exception)
                            {
                                pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue.ToString());
                            }
                        }
                        if (cell.ColumnIndex == 21)
                        {
                            try
                            {
                                pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.DateCellValue.ToString());
                            }

                            catch (Exception)
                            {
                                pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.StringCellValue.ToString());
                            }

                        }
                        if (cell.ColumnIndex == 22)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                            }
                            else
                            {
                                pol.GenelBilgiler.ParaBirimi = "TL";
                            }
                        }

                        if (cell.ColumnIndex == 23)
                        {
                            try
                            {
                                if (cell.StringCellValue != null && cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue;
                                }
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.NumericCellValue.ToString();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }


                        if (cell.ColumnIndex == 24)
                        {
                            try
                            {
                                if (cell.StringCellValue != null && cell.StringCellValue != "")
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue;
                                }
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.NumericCellValue.ToString();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        try
                        {
                            if (cell.ColumnIndex == 25) pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                        }
                        catch (Exception)
                        {
                            if (cell.ColumnIndex == 25) pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.StringCellValue.ToString());

                        }
                        try
                        {
                            if (cell.ColumnIndex == 26) pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                        }
                        catch (Exception)
                        {
                            if (cell.ColumnIndex == 26) pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.StringCellValue.ToString());
                        }
                        try
                        {
                            if (cell.ColumnIndex == 27) pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.NumericCellValue.ToString());
                        }
                        catch (Exception)
                        {
                            if (cell.ColumnIndex == 27) pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.StringCellValue.ToString());
                        }
                        if (cell.ColumnIndex == 28)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceArac.MotorNo = cell.StringCellValue;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceArac.MotorNo = ".";
                            }
                        }
                        if (cell.ColumnIndex == 29)
                        {
                            if (cell.StringCellValue != null && cell.StringCellValue != "")
                            {
                                pol.GenelBilgiler.PoliceArac.SasiNo = cell.StringCellValue;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceArac.SasiNo = ".";
                            }
                        }

                        #region vade taksitleri
                        if (cell.ColumnIndex == 30)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[0].TaksitNo = 1;
                                odemeler[0].VadeTarihi = temp;
                            }
                            else
                            {
                                odemeler[0].VadeTarihi = null;
                            }

                        }
                        if (cell.ColumnIndex == 31)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[0].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[0].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 32)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[1].TaksitNo = 2;
                                odemeler[1].VadeTarihi = temp;
                            }
                            else
                            {
                                odemeler[1].VadeTarihi = null;
                            }
                        }

                        if (cell.ColumnIndex == 33)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[1].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[1].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 34)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[2].TaksitNo = 3;
                                odemeler[2].VadeTarihi = temp;
                            }
                            else
                            {
                                odemeler[2].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 35)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[2].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[2].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 36)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[3].TaksitNo = 4;
                                odemeler[3].VadeTarihi = temp;
                            }
                            else
                            {
                                odemeler[3].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 37)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[3].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[3].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 38)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[4].TaksitNo = 5;
                                odemeler[4].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[4].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 39)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[4].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[4].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 40)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[5].TaksitNo = 6;
                                odemeler[5].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[5].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 41)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[5].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[5].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 42)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[6].TaksitNo = 7;
                                odemeler[6].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[5].VadeTarihi = null;
                            }

                        }
                        if (cell.ColumnIndex == 43)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[6].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[6].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 44)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[7].TaksitNo = 8;
                                odemeler[7].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[7].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 45)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[7].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[7].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 46)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[8].TaksitNo = 9;
                                odemeler[8].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[8].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 47)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[8].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[8].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 48)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[9].TaksitNo = 10;
                                odemeler[9].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[9].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 49)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[9].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[9].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 50)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[10].TaksitNo = 11;
                                odemeler[10].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[10].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 51)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[10].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[10].TaksitTutari = 0;
                            }

                        }
                        if (cell.ColumnIndex == 52)
                        {
                            DateTime temp;
                            if (DateTime.TryParse(cell.StringCellValue, out temp))
                            {
                                odemeler[11].TaksitNo = 12;
                                odemeler[11].VadeTarihi = Convert.ToDateTime(cell.DateCellValue);
                            }
                            else
                            {
                                odemeler[11].VadeTarihi = null;
                            }
                        }
                        if (cell.ColumnIndex == 53)
                        {
                            if (Util.ToDecimal(cell.NumericCellValue.ToString()).ToString() != "0")
                            {
                                odemeler[11].TaksitTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            else
                            {
                                odemeler[11].TaksitTutari = 0;
                            }

                        }
                        #endregion

                    }

                    #region ödeme planı
                    // Odeme planini duzenle
                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                    for (int i = 0; i < odemeler.Length; i++)
                    {
                        odm = new PoliceOdemePlani();
                        odm = odemeler[i];
                        if (odm.TaksitTutari != 0 && odm.TaksitTutari != null) // tutar pozitif ise odeme planina ekle
                        {
                            pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);

                            tahsilat = new PoliceTahsilat();
                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                            tahsilat.TaksitNo = odm.TaksitNo;
                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                            tahsilat.OdemTipi = 2;
                            tahsilat.OdemeBelgeNo = "111111****1111";
                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                            tahsilat.OdenenTutar = tahsilat.TaksitTutari;
                            //tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                            tahsilat.KalanTaksitTutari = 0;
                            tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                            tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                            tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            tahsilat.CariHesapKodu = "120.01." + tahsilat.KimlikNo;
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
                    }
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 0)
                    {
                        odm = new PoliceOdemePlani();
                        odm.TaksitTutari = pol.GenelBilgiler.BrutPrim;
                        odm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                        odm.TaksitNo = 1;
                        pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);

                        tahsilat = new PoliceTahsilat();
                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                        tahsilat.TaksitNo = odm.TaksitNo;
                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                        tahsilat.OdemTipi = 2;
                        tahsilat.OdemeBelgeNo = "111111****1111";

                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                        tahsilat.OdenenTutar = tahsilat.TaksitTutari;
                        //tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                        tahsilat.KalanTaksitTutari = 0;
                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        tahsilat.CariHesapKodu = "120.01." + tahsilat.KimlikNo;
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

                    #endregion

                    #region tahsilat

                    //PoliceTahsilat tahsilat = new PoliceTahsilat();
                    //tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                    //tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                    //tahsilat.TaksitNo = odm.TaksitNo;
                    //tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                    //// tahsilat.OdemeBelgeNo = "111111";
                    //tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                    //tahsilat.OdenenTutar = 0;
                    //tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                    //tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                    //tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                    //tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                    //tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                    //tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                    //tahsilat.KayitTarihi = DateTime.Today;
                    //tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                    //tahsilat.TahsilatId = odm.PoliceId;
                    //if (tahsilat.TaksitTutari != 0)
                    //{
                    //    pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                    //}

                    #endregion

                    #region  branş eşleştir

                    // add pol to list
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 0) pol.GenelBilgiler.OdemeSekli = 0;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 1) pol.GenelBilgiler.OdemeSekli = 1;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count > 1) pol.GenelBilgiler.OdemeSekli = 2;

                    PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                    PoliceBransEslestir = Util.PoliceBransAdiEslestirWithBirlikKod(pol.GenelBilgiler.TUMBirlikKodu, _branslar, tumUrunAdi, tumUrunKodu);

                    pol.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                    pol.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;

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

                    pol.GenelBilgiler.TUMBransAdi = tumUrunAdi;
                    pol.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;

                    if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Trafik".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 1;
                        pol.GenelBilgiler.BransAdi = "Trafik";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Kasko".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 2;
                        pol.GenelBilgiler.BransAdi = "Kasko";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Kaza-OtoDışı".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 3;
                        pol.GenelBilgiler.BransAdi = "Kaza-OtoDışı";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Sağlık".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 4;
                        pol.GenelBilgiler.BransAdi = "Sağlık";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Hayat".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 5;
                        pol.GenelBilgiler.BransAdi = "Hayat";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Yangın".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 6;
                        pol.GenelBilgiler.BransAdi = "Yangın";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Nakliyat".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 7;
                        pol.GenelBilgiler.BransAdi = "Nakliyat";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Tam. Sağlık".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 8;
                        pol.GenelBilgiler.BransAdi = "Tam. Sağlık";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Mühendislik".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 9;
                        pol.GenelBilgiler.BransAdi = "Mühendislik";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Sorumluluk".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 10;
                        pol.GenelBilgiler.BransAdi = "Sorumluluk";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "DASK".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 11;
                        pol.GenelBilgiler.BransAdi = "DASK";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Tarım".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 12;
                        pol.GenelBilgiler.BransAdi = "Tarım";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Tekne".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 13;
                        pol.GenelBilgiler.BransAdi = "Tekne";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Ferdi Kaza".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 14;
                        pol.GenelBilgiler.BransAdi = "Ferdi Kaza";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Hukuksal Koruma".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 15;
                        pol.GenelBilgiler.BransAdi = "Hukuksal Koruma";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "Kredi".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 16;
                        pol.GenelBilgiler.BransAdi = "Kredi";
                    }
                    else if (tumUrunAdi.ToLower().Replace('ı', 'i') == "BES".ToLower().Replace('ı', 'i'))
                    {
                        pol.GenelBilgiler.BransKodu = 17;
                        pol.GenelBilgiler.BransAdi = "BES";
                    }
                    #endregion

                    policeler.Add(pol);
                }
            }

            listeler.policeListesi = policeler;

            return listeler;
        }

        public string getMessage()
        {
            return this.message;
        }



    }
}
