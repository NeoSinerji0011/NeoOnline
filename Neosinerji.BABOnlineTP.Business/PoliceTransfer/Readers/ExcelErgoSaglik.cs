using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using System.IO;


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class ExcelErgoSaglik
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
                                           "", //0
                                           "Acente Adı",//1
                                           "Acente Kodu",//2
                                           "Tali Acente No",//3
                                           "MT",//4
                                           "MT Kodu",//5
                                           "Poliçe No",//6 *
                                           "Sıra",//7*
                                           "Harici Poliçe No",//8
                                           "Poliçe Baş.Tar.",//9*
                                           "Poliçe Bit.Tar.",//10*
                                           "Poliçe Durumu",//11
                                           "Poliçe Tipi",//12*
                                           "Ürün",//13*
                                           "Sigorta Ettiren Adı",//14         *
                                           "Sigorta Ettiren Soyadı",//15      *
                                           "Sig. Ettiren TCKN",//16           *
                                           "Sig. Ettiren Pasaport No",//17    *
                                           "Sig. Ettiren Vergi No",//18       *
                                           "Grubu",//19
                                           "Ödeme Planı",//20**
                                           "Zeyil No",//21**
                                           "Tanzim Tarihi",//22*
                                           "Geçerlilik Tarihi",//23*
                                           "T.C. Kimlik No",//24*
                                           "Pasaport No",//25*
                                           "Ad",//26*
                                           "Soyad",//27*
                                           "Doğum Tarihi",//28*
                                           "Cinsiyet",//29*
                                           "VIP",//30*
                                           "Yakınlık",//31
                                           "Plan",//32
                                           "Prim(TL)",//33*
                                           "Komisyon(TL)",//34*
                                           "Vergi(TL)",//35**
                                           "Poliçe Döviz Tipi",//36*
                                           "Dönüşüm Döviz Tipi",//37*
                                           "Kur"//38*


    };

      
        public ExcelErgoSaglik(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization
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
            string psliKimlikNo = null;
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
            ISheet sheet = wb.GetSheet("Sheet1");

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

                    // tvm kodu
                    pol.GenelBilgiler.TVMKodu = tvmKodu;

                    // Birlik Kodu
                    pol.GenelBilgiler.TUMBirlikKodu = birlikKodu;

                    List<ICell> cels = row.Cells;

                    foreach (ICell cell in cels)
                    {

                        if (cell.ColumnIndex == 6) pol.GenelBilgiler.PoliceNumarasi = cell.StringCellValue;
                        if (cell.ColumnIndex == 7) pol.GenelBilgiler.YenilemeNo = Convert.ToInt32(cell.StringCellValue);
                        if (cell.ColumnIndex == 9)
                            if (cell.StringCellValue.Contains("'"))
                            {
                                pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue.Substring(1, 10), Util.DateFormat1);
                            }
                            else
                            {
                                pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                            }
                        if (cell.ColumnIndex == 10)
                        {
                            if (cell.StringCellValue.Contains("'"))
                            {
                                pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.StringCellValue.Substring(1, 10), Util.DateFormat1);
                            }
                            else
                            {
                                pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                            }
                        }
                        if (cell.ColumnIndex == 14) pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 15) pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 16) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 17) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue != null ? cell.StringCellValue : null;
                        if (cell.ColumnIndex == 18) pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 21) pol.GenelBilgiler.ZeyilKodu = cell.StringCellValue;
                        if (cell.ColumnIndex == 22) pol.GenelBilgiler.TanzimTarihi = Convert.ToDateTime(cell.StringCellValue);
                        if (cell.ColumnIndex == 24) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 25) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue != null ? cell.StringCellValue : null;
                        if (cell.ColumnIndex == 26) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 27) pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 28) pol.GenelBilgiler.PoliceSigortali.DogumTarihi = Convert.ToDateTime(cell.StringCellValue);
                        if (cell.ColumnIndex == 29) pol.GenelBilgiler.PoliceSigortali.Cinsiyet = cell.StringCellValue;

                        if (cell.ColumnIndex == 33) pol.GenelBilgiler.NetPrim = Convert.ToInt32(cell.StringCellValue);
                        if (cell.ColumnIndex == 33) pol.GenelBilgiler.BrutPrim = Convert.ToInt32(cell.StringCellValue);

                        if (cell.ColumnIndex == 34) pol.GenelBilgiler.Komisyon = Convert.ToInt32(cell.StringCellValue);

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
                List<ICell> celsa = row.Cells;

                foreach (ICell cell in celsa)
                {
                    if (cell.ColumnIndex == 6) policemNo = cell.StringCellValue;
                    if (cell.ColumnIndex == 9) polBasTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                    if (cell.ColumnIndex == 21) ekNom = Util.toInt(row.GetCell(2).StringCellValue);
                    if (cell.ColumnIndex == 24) psliKimlikNo = cell.StringCellValue;
                    if (cell.ColumnIndex == 33)
                    {
                        polBrutprimim = Util.ToDecimal(cell.NumericCellValue.ToString());
                        if (polBrutprimim < 0) carpan = -1;
                    }

                }
                // Odeme planinin deiger taksitleri icin
                if (row.FirstCellNum == 32)
                {
                    // Odeme Plani - diger taksitler

                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    taksitNo += 1;
                    odm.TaksitNo = taksitNo;
                    odm.VadeTarihi = Util.toDate(row.GetCell(32).StringCellValue, Util.DateFormat1);
                    odm.TaksitTutari = carpan * Util.ToDecimal(row.GetCell(33).NumericCellValue.ToString()); // iptal ise tutuar eksi deger olmali
                    odemePol.GenelBilgiler.OdemeSekli = 2; // Vadeli - taksit varsa vadeli

                    #region Tahsilat işlemi
                    var getDetay = _TVMService.GetDetay(tvmKodu);
                    if (getDetay != null)
                    {
                        if (getDetay.MuhasebeEntegrasyon.HasValue)
                        {
                            if (!getDetay.MuhasebeEntegrasyon.Value)
                            {
                                if (bransKod == 1 || bransKod == 2)
                    {
                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                        tahsilat.TaksitNo = odm.TaksitNo;
                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                        tahsilat.OdemeBelgeNo = "111111****1111";
                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                        tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                        tahsilat.KalanTaksitTutari = 0;
                        tahsilat.PoliceNo = policemNo;
                        tahsilat.ZeyilNo = ekNom.ToString();
                        tahsilat.KimlikNo = psliKimlikNo;
                        tahsilat.BrutPrim = polBrutprimim.Value;
                        // tahsilat.PoliceId = odm.PoliceGenel.PoliceId;
                        tahsilat.KayitTarihi = DateTime.Today;
                        tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                        tahsilat.TahsilatId = odm.PoliceId;
                        if (tahsilat.TaksitTutari != 0)
                        {
                            odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                        }
                    }
                            }
                        }
                    }
                    #endregion
                    if (odm.TaksitTutari != 0)
                    {
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

     

