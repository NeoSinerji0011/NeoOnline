using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;


using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Web.Mvc;
using NPOI.XSSF.UserModel;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    class AkExcelReader : IPoliceTransferReader

    {
        HSSFWorkbook wb, wb1;

        IAktifKullaniciService _AktifKullanici;
        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        ITVMService _TVMService;
        private string message = string.Empty;
        private string filePath;
        private string[] columnNameseski1 =  {
                                            "Urun Paket Kodu",//0
                                            "Cari Pol No",//1
                                            "Zeyl Sira No",//2
                                            "Teklif/Onay",//3
                                            "Tali Acenta No",//4
                                            "Police Baslangic Tarihi",//5
                                            "Police Bitis Tarihi",//6
                                            "Tanzim Tarihi",//7
                                            "Artis Azalis Kodu",//8
                                            "Tarife Kodu",//9
                                            "Zeyil Hareket Kodu",//10
                                            "Sozlesme",//11
                                            "Musteri Adi",//12
                                            "Musteri Soyadi",//13
                                            "Vergi Numarasi",//14
                                            "Cadde",//15
                                            "Sokak",//16
                                            "Bina",//17
                                            "Ilce Kodu",//18
                                            "Semt",//19
                                            "Il Kodu",//20
                                            "Gercek / Tuzel Kodu",//21
                                            "Plaka Numarasi",//22
                                            "Gercek Sistem Tarihi",//23
                                            "Marka Kodu",//24
                                            "Motor Numarasi",//25
                                            "Sasi Numarasi",//26
                                            "Police Doviz Cinsi",//27
                                            "Police Doviz Kuru",//28
                                            "Daini Mürtehin Banka Kodu",//29
                                            "Daini Mürtehin Sube Kodu",//30
                                            "Daini Mürtehin Vergi Numarası",//31
                                            "Pesinat Tarihi",//32
                                            "Pesinat Tutari",//33
                                            "1.Vade Tarihi",//34
                                            "1.Vade Tutari",//35
                                            "2.Vade Tarihi",//36
                                            "2.Vade Tutari",//37
                                            "3.Vade Tarihi",//38
                                            "3.Vade Tutari",//39
                                            "4.Vade Tarihi",//40
                                            "4.Vade Tutari",//41
                                            "5.Vade Tarihi",//42
                                            "5.Vade Tutari",//43
                                            "6.Vade Tarihi",//44
                                            "6.Vade Tutari",//45
                                            "7.Vade Tarihi",//46
                                            "7.Vade Tutari",//47
                                            "8.Vade Tarihi",//48
                                            "8.Vade Tutari",//49
                                            "9.Vade Tarihi",//50
                                            "9.Vade Tutari",//51
                                            "10.Vade Tarihi",//52
                                            "10.Vade Tutari",//53
                                            "11.Vade Tarihi",//54
                                            "11.Vade Tutari",//55
                                            "12.Vade Tarihi",//56
                                            "12.Vade Tutari",//57
                                            "Net Prim",//58
                                            "Vergi",//59
                                            "YSV",//60
                                            "FON (GF+THGF)",//61
                                            "Komisyon",//62
                                            "Brut Prim",//63
                                            "Dogum Tarihi",//64
                                            "Telefon No",//65
                                            "Arac Modeli",//66
                                            "Trafik Tescil Tarihi",//67
                                            "Sigorta Bedeli",//68
                                            "AS400PaketKodu",//69
                                            "AS400PoliceNo",//70
                                            "AS400TecditNo",//71
                                            "AS400ZeyilNo",//72
                                            "Mail Adres,",//73
                                            "Kullanıcı",//74
                                            "Yenileme No",//75
                                            "Aracın Kullanım Tarzı",//76
                                            "Aracın Kullanım Tarzı Açıklaması",//77
                                            "Ödeme Tipi",//78
                                            "Hesap Numarası",//79
                                            "Önceki Poliçe Numarası",//80
                                            "GF",//81
                                            "THGF",//82
                                            "Ruhsat Numarası",//83
                                            "Ödeme Aracı",//84
                                            "Akreditif Numarası"//85

                                        };
        private string[] columnNames =  {
                                            "Sigorta Şirket Adı",//0
                                            "Poliçe Numarası",//1
                                            "Eski Poliçe Numarası",//2
                                            "İlgili Poliçe Yenileme Numarası",//3
                                            "Zeyl Sıra No",//4
                                            "Zeyl Tip Kodu",//5
                                            "Zeyl Tip Adı",//6
                                            "Teklif/Onay",//7
                                            "Daini Banka",//8
                                            "Daini Şube",//9
                                            "Daini VKN",//10
                                            "Tarife Kodu",//11
                                            "Poliçe Branş Ad",//12
                                            "Ana Ekip T",//13
                                            "Teklif Tarihi",//14
                                            "Tanzim Tarihi",//15
                                            "Tarih",//16
                                            "Başlama Tarihi",//17
                                            "Bitiş Tarihi",//18
                                            "Acente No",//19
                                            "Acente Adı",//20
                                            "Sigorta Ettiren No",//21
                                            "Sigorta Ettiren Adı",//22
                                            "Sigorta Ettiren Kimlik No",//23
                                            "Sigorta Ettiren Vergi No",//24
                                            "Sigortalı No",//25
                                            "Sigortalı Ad",//26
                                            "Sigortalı Tc Kimlik No",//27
                                            "Sigortalı Vergi No",//28
                                            "Riziko Adres",//29
                                            "Döviz Cins",//30
                                            "Döviz Kur",//31
                                            "aKomisyon TL Değer",//
                                            "aKomisyon Döviz Değer",
                                            "Net Prim TL Değer",
                                            "Net Prim Döviz Değer",
                                            "Gider Vergisi TL Değer",
                                            "Gider Vergisi Döviz Değer",
                                            "YS Vergisi TL Değer",
                                            "YS Vergisi Döviz Değer",
                                            "TRAHIZGELFON TL Değer",
                                            "TRHIZGELFON Döviz Değer",
                                            "Garanti Fonu TL Değer",
                                            "Garanti Fonu Döviz Değer",
                                            "Toplam Ücret TL Değer",
                                            "Toplam Ücret Döviz Değer",
                                            "Kullanıcı",
                                            "Akreditif Numarası",
                                            "Pesinat Tarihi",//32
                                            "Pesinat Tutari",//33
                                            "Peşinat Döviz Tutarı",
                                            "1. Vade Tutarı Ödeme Tarihi",//34
                                            "1. Vade Tutarı Ödeme TL Değer",//35
                                            "1. Vade Tutarı Ödeme Döviz Değer",
                                            "2. Vade Tutarı Ödeme Tarihi",//36
                                            "2. Vade Tutarı Ödeme TL Değer",//37
                                            "2. Vade Tutarı Ödeme Döviz Değer",
                                            "3. Vade Tutarı Ödeme Tarihi",//38
                                            "3. Vade Tutarı Ödeme TL Değer",//39
                                            "3. Vade Tutarı Ödeme Döviz Değer",
                                            "4. Vade Tutarı Ödeme Tarihi",//40
                                            "4. Vade Tutarı Ödeme TL Değer",//41
                                            "4. Vade Tutarı Ödeme Döviz Değer",
                                            "5. Vade Tutarı Ödeme Tarihi",//42
                                            "5. Vade Tutarı Ödeme TL Değer",//43
                                            "5. Vade Tutarı Ödeme Döviz Değer",
                                            "6. Vade Tutarı Ödeme Tarihi",//44
                                            "6. Vade Tutarı Ödeme TL Değer",//45
                                            "6. Vade Tutarı Ödeme Döviz Değer",
                                            "7. Vade Tutarı Ödeme Tarihi",//46
                                            "7. Vade Tutarı Ödeme TL Değer",//47
                                            "7. Vade Tutarı Ödeme Döviz Değer",
                                            "8. Vade Tutarı Ödeme Tarihi",//48
                                            "8. Vade Tutarı Ödeme TL Değer",//49
                                            "8. Vade Tutarı Ödeme Döviz Değer",
                                            "9. Vade Tutarı Ödeme Tarihi",//50
                                            "9. Vade Tutarı Ödeme TL Değer",//51
                                            "9. Vade Tutarı Ödeme Döviz Değer",
                                            "10. Vade Tutarı Ödeme Tarihi",//52
                                            "10. Vade Tutarı Ödeme TL Değer",//53
                                            "10. Vade Tutarı Ödeme Döviz Değer",
                                            "11. Vade Tutarı Ödeme Tarihi",//54
                                            "11. Vade Tutarı Ödeme TL Değer",//55
                                            "11. Vade Tutarı Ödeme Döviz Değer",
                                            "12. Vade Tutarı Ödeme Tarihi",//56
                                            "12. Vade Tutarı Ödeme TL Değer",//57
                                            "12. Vade Tutarı Ödeme Döviz Değer",
                                            "Aracin Kullanim Tarzi",//58
                                            "Aracin Kullanim Tarzi Aciklamasi",//59
                                            "Araç Kullanim Sekli Kodu",//61
                                            "Araç Kullanım Sekli",//60
                                            "DASK POL. NO",//62
                                            "FAALİYET AÇIKLAMA",//63
                                            "Kasko Tarife Tip Aciklamasi",//64
                                            "Kasko Tarife Tip Kodu",//65
                                            "M2",//66
                                            "Marka",//67
                                            "Marka Kodu",//68
                                            "Model Yili",//69
                                            "Motor Numarasi",//70
                                            "Plaka Numarasi",//71
                                            "Ruhsat Numarasi",//72
                                            "Sasi Numarasi",//73
                                            "Trafik Tescil Tarihi",//74
                                            "UAVT NO",//75
                                            "Yurtdışı Teminatı",//76
                                            "yurtDisiTemSur",//77
                                            "Urun Paket Kodu",
                                            "Ödeme Aracı",
                                            "Sigorta bedeli",
                                            "Sigortalı Doğum Tarihi"

                                        };

        public AkExcelReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            this.filePath = path;
            this.tvmKodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }

        public List<Police> getPolicelereski()
        {
            List<Police> policeler = new List<Police>();

            // get excel file...
            FileStream excelFile = null, excelFile1 = null;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string musteriEmaili = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            try
            {
                excelFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }

            ISheet sheet = null;
            if (filePath.Contains(".xlsx"))
            {
                XSSFWorkbook wb1 = new XSSFWorkbook(excelFile);
                sheet = wb1.GetSheet("veri");
            }
            else
            {
                wb = new HSSFWorkbook(excelFile);
                sheet = wb.GetSheet("veri");
            }


            // check sheet correct... 
            int startRow = Util.checkSheetCorrect(sheet, columnNames);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }

            ////////////
            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();
            string[] tempPath = filePath.Split('#');
            if (tempPath.Length > 1)
            {
                policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                filePath = filePath.Substring(0, filePath.IndexOf("#"));
                //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\NeoOnline_TahsilatKapatma.xls";
                //excelFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                //wb1 = new HSSFWorkbook(excelFile);
                //ISheet sheet1 = wb1.GetSheet("Sheet1");

                //for (int indx = sheet1.FirstRowNum + 2; indx <= sheet1.LastRowNum; indx++)
                //{
                //    IRow row = sheet1.GetRow(indx);
                //    var pno = row.GetCell(1).ToString();
                //    var eno = row.GetCell(2).ToString();
                //    var yno = row.GetCell(3).ToString();
                //    var kkno = row.GetCell(7).StringCellValue.Trim();
                //    neoOnline_TahsilatKapatma = new NeoOnline_TahsilatKapatma { Police_No = pno, Yenileme_No = yno, Ek_No = eno, Kart_No = kkno };
                //    policeTahsilatKapatma.Add(neoOnline_TahsilatKapatma);
                //}
            }
            /////////////
            // sheet correct. Start to get rows...
            try
            {
                for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
                {
                    IRow row = sheet.GetRow(indx);

                    // null rowlar icin
                    if (row == null) break;

                    Police pol = new Police();
                    PoliceOdemePlani[] odemeler = { new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani() };

                    // Birlik Kodu
                    pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AKSIGORTA;
                    // tvm kodu
                    pol.GenelBilgiler.TVMKodu = tvmKodu;

                    List<ICell> cels = row.Cells;
                    var tanimliOdemeTipleri = _TVMService.GetListTanımliBransOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.AKSIGORTA);

                    foreach (ICell cell in cels)
                    {
                        int colIndex = cell.ColumnIndex;

                        switch (colIndex)
                        {
                            case 1:
                                pol.GenelBilgiler.PoliceNumarasi = cell.ToString();
                                break;
                            case 2:
                                pol.GenelBilgiler.EkNo = Util.toInt(cell.ToString());
                                break;
                            case 5:
                                pol.GenelBilgiler.BaslangicTarihi = cell.DateCellValue;
                                break;
                            case 6:
                                pol.GenelBilgiler.BitisTarihi = cell.DateCellValue;
                                break;
                            case 7:
                                pol.GenelBilgiler.TanzimTarihi = cell.DateCellValue;
                                break;
                            case 9:
                                tumUrunKodu = cell.StringCellValue;
                                break;
                            case 12:
                                pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                                pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                                break;
                            case 13:
                                pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = cell.StringCellValue;
                                pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan = cell.StringCellValue;
                                break;
                            case 14:
                                string kimlikNo = cell.ToString();
                                if (kimlikNo != null && kimlikNo.Length == 11) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = kimlikNo;
                                if (kimlikNo != null && kimlikNo.Length == 10) pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = kimlikNo;
                                if (kimlikNo != null && kimlikNo.Length == 11) pol.GenelBilgiler.PoliceSigortali.KimlikNo = kimlikNo;
                                if (kimlikNo != null && kimlikNo.Length == 10) pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = kimlikNo;
                                sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                                break;
                            case 15:
                                if (!String.IsNullOrEmpty(cell.StringCellValue))
                                {
                                    pol.GenelBilgiler.PoliceSigortali.Cadde = cell.StringCellValue.Trim();
                                    pol.GenelBilgiler.PoliceSigortaEttiren.Cadde = cell.StringCellValue.Trim();
                                }
                                else
                                {
                                    pol.GenelBilgiler.PoliceSigortali.Cadde = null;
                                    pol.GenelBilgiler.PoliceSigortaEttiren.Cadde = null;
                                }
                                break;
                            case 16:
                                pol.GenelBilgiler.PoliceSigortali.Sokak = cell.StringCellValue;
                                pol.GenelBilgiler.PoliceSigortaEttiren.Sokak = cell.StringCellValue;
                                break;
                            case 17:
                                pol.GenelBilgiler.PoliceSigortali.Apartman = cell.StringCellValue;
                                pol.GenelBilgiler.PoliceSigortaEttiren.Apartman = cell.StringCellValue;
                                break;
                            case 18:
                                //pol.GenelBilgiler.PoliceSigortali.IlceKodu = !String.IsNullOrEmpty(cell.StringCellValue.Replace('I','0').Replace('A','0')) ? Convert.ToInt32(cell.StringCellValue.Replace('I', '0').Replace('A', '0')) : 0; 
                                pol.GenelBilgiler.PoliceSigortali.IlceKodu = int.TryParse(cell.StringCellValue.Replace('I', '0').Replace('A', '0'), out int res) ? res : 0;
                                //pol.GenelBilgiler.PoliceSigortaEttiren.IlceKodu = !String.IsNullOrEmpty(cell.StringCellValue.Replace('I', '0').Replace('A', '0')) ? Convert.ToInt32(cell.StringCellValue.Replace('I', '0').Replace('A', '0')) : 0;
                                pol.GenelBilgiler.PoliceSigortaEttiren.IlceKodu = int.TryParse(cell.StringCellValue.Replace('I', '0').Replace('A', '0'), out res) ? res : 0;
                                break;
                            case 19:
                                pol.GenelBilgiler.PoliceSigortali.Semt = cell.StringCellValue;
                                pol.GenelBilgiler.PoliceSigortaEttiren.Semt = cell.StringCellValue;
                                break;
                            case 20:
                                pol.GenelBilgiler.PoliceSigortali.IlKodu = cell.ToString();
                                pol.GenelBilgiler.PoliceSigortaEttiren.IlKodu = cell.ToString();
                                break;
                            case 22:
                                if (cell.ColumnIndex != null)
                                {
                                    pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(2, cell.StringCellValue.Length - 2) : "";
                                    pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(0, 2) : "";
                                }
                                break;
                            case 24:
                                pol.GenelBilgiler.PoliceArac.Marka = cell.ToString();
                                break;
                            case 25:
                                pol.GenelBilgiler.PoliceArac.MotorNo = cell.ToString();
                                break;
                            case 26:
                                pol.GenelBilgiler.PoliceArac.SasiNo = cell.ToString();
                                break;
                            case 27:
                                pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                                if (pol.GenelBilgiler.ParaBirimi == "YTL")
                                {
                                    pol.GenelBilgiler.ParaBirimi = "TL";
                                }
                                break;
                            case 28:
                                pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.ToString());

                                break;
                            case 32: // pesinat
                                odemeler[0].TaksitNo = 1;
                                odemeler[0].VadeTarihi = cell.DateCellValue;
                                break;
                            case 33:
                                odemeler[0].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[0].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[0].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 34:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[1].TaksitNo = 2;
                                    odemeler[1].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 35:
                                odemeler[1].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[1].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[1].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 36:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[2].TaksitNo = 3;
                                    odemeler[2].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 37:
                                odemeler[2].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[2].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[2].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 38:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[3].TaksitNo = 4;
                                    odemeler[3].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 39:
                                odemeler[3].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[3].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[3].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 40:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[4].TaksitNo = 5;
                                    odemeler[4].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 41:
                                odemeler[4].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[4].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[4].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 42:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[5].TaksitNo = 6;
                                    odemeler[5].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 43:
                                odemeler[5].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[5].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[5].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 44:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[6].TaksitNo = 7;
                                    odemeler[6].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 45:
                                odemeler[6].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[6].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[6].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 46:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[7].TaksitNo = 8;
                                    odemeler[7].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 47:
                                odemeler[7].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[7].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[7].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 48:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[8].TaksitNo = 9;
                                    odemeler[8].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 49:
                                odemeler[8].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[8].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[8].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 50:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[9].TaksitNo = 10;
                                    odemeler[9].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 51:
                                odemeler[9].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[9].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[9].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 52:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[10].TaksitNo = 11;
                                    odemeler[10].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 53:
                                odemeler[10].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[10].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[10].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 54:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[11].TaksitNo = 12;
                                    odemeler[11].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 55:
                                odemeler[11].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[11].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[11].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 56:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[12].TaksitNo = 13;
                                    odemeler[12].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 57:
                                odemeler[12].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[12].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    odemeler[12].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;

                            case 58:
                                pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 59: // Gider vergisi
                                PoliceVergi gv = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    gv.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi = Util.ToDecimal(cell.ToString());
                                    gv.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                gv.VergiKodu = 2;
                                pol.GenelBilgiler.PoliceVergis.Add(gv);
                                break;
                            case 60: // YSV
                                PoliceVergi ysv = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi += Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    ysv.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi += Util.ToDecimal(cell.ToString());
                                    ysv.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                ysv.VergiKodu = 4;
                                pol.GenelBilgiler.PoliceVergis.Add(ysv);
                                break;
                            //case 57: // GF + THGF
                            //    pol.GenelBilgiler.ToplamVergi += Util.toDecimal(cell.ToString());
                            //    PoliceVergi gfthgf = new PoliceVergi();
                            //    gfthgf.VergiKodu = 1;
                            //    gfthgf.VergiTutari = Util.toDecimal(cell.ToString());
                            //    pol.GenelBilgiler.PoliceVergis.Add(gfthgf);
                            //    break;
                            case 62:
                                pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 63:
                                pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.BrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.NetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.Komisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    // dovizKuru = 0;
                                }
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(cell.ToString());
                                }
                                break;

                            case 67:
                                pol.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                                break;
                            case 73:
                                musteriEmaili = cell.StringCellValue;
                                pol.GenelBilgiler.PoliceSigortali.EMail = musteriEmaili;
                                pol.GenelBilgiler.PoliceSigortaEttiren.EMail = musteriEmaili;

                                break;
                            case 77:
                                pol.GenelBilgiler.PoliceArac.KullanimTarzi = cell.StringCellValue;
                                break;
                            case 81:
                                PoliceVergi gf = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi += Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    gf.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi += Util.ToDecimal(cell.ToString());
                                    gf.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                gf.VergiKodu = 3;
                                pol.GenelBilgiler.PoliceVergis.Add(gf);
                                break;
                            case 82:
                                PoliceVergi thgf = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi += Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    thgf.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi += Util.ToDecimal(cell.ToString());
                                    thgf.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                thgf.VergiKodu = 1;
                                pol.GenelBilgiler.PoliceVergis.Add(thgf);
                                break;
                        }
                    }
                    // Odeme planini duzenle
                    PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                    PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                    pol.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                    pol.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;

                    var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, pol.GenelBilgiler);

                    for (int i = 0; i < odemeler.Length; i++)
                    {
                        PoliceOdemePlani odm = odemeler[i];
                        if (row.GetCell(84).StringCellValue == "SANAL POS TAHSILAT")
                        {
                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                        }
                        else if (row.GetCell(84).StringCellValue == "KREDİ KARTI")
                        {
                            odm.OdemeTipi = OdemeTipleri.KrediKarti;

                        }
                        else if (row.GetCell(84).StringCellValue == "NAKİT ÖDEME")
                        {
                            odm.OdemeTipi = OdemeTipleri.Nakit;
                        }
                        else if (row.GetCell(84).StringCellValue == "HAVALE")
                        {
                            odm.OdemeTipi = OdemeTipleri.Havale;
                        }
                        else
                        {
                            odm.OdemeTipi = OdemeTipleri.Havale;
                        }
                        if (odm.TaksitTutari != 0) // tutar pozitif ise odeme planina ekle
                        {
                            pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                        }

                        #region Tahsilat işlemi


                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.AKSIGORTA, pol.GenelBilgiler.BransKodu.Value);

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
                                        tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
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
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (!String.IsNullOrEmpty(row.GetCell(84).StringCellValue))
                            {
                                if (row.GetCell(84).StringCellValue == "SANAL POS TAHSILAT")
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.KalanTaksitTutari = 0;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                                else if (row.GetCell(84).StringCellValue == "KREDİ KARTI")
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.KalanTaksitTutari = 0;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                                else if (row.GetCell(84).StringCellValue == "NAKİT ÖDEME")
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
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
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                                else if (row.GetCell(84).StringCellValue == "HAVALE")
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
                                    tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
                                    tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }


                                }
                            }
                        }



                        #endregion
                    }
                    if (odemeler.Count() == 0 && pol.GenelBilgiler.BrutPrim.Value != 0)
                    {
                        PoliceOdemePlani odmm = new PoliceOdemePlani();
                        if (odmm.TaksitTutari == null)
                        {
                            odmm.TaksitTutari = pol.GenelBilgiler.BrutPrim;
                            if (odmm.VadeTarihi == null)
                            {
                                odmm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                            }
                            odmm.OdemeTipi = OdemeTipleri.Havale;
                            odmm.TaksitNo = 1;
                            if (row.GetCell(84).StringCellValue == "SANAL POS TAHSILAT")
                            {
                                odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                            }
                            else if (row.GetCell(84).StringCellValue == "KREDİ KARTI")
                            {
                                odmm.OdemeTipi = OdemeTipleri.KrediKarti;

                            }
                            else if (row.GetCell(84).StringCellValue == "NAKİT ÖDEME")
                            {
                                odmm.OdemeTipi = OdemeTipleri.Nakit;
                            }
                            else if (row.GetCell(84).StringCellValue == "HAVALE")
                            {
                                odmm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            else
                            {
                                odmm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            pol.GenelBilgiler.PoliceOdemePlanis.Add(odmm);

                            var restahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, pol.GenelBilgiler);


                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.AKSIGORTA, pol.GenelBilgiler.BransKodu.Value);

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
                                        odmm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                        if (tahsilat.OdemTipi == 1)
                                        {
                                            tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                            tahsilat.KalanTaksitTutari = 0;
                                            tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                            tahsilat.OtomatikTahsilatiKkMi = 1;
                                        }
                                        else
                                        {
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odmm.TaksitTutari;
                                        }
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(row.GetCell(84).StringCellValue))
                                {
                                    if (row.GetCell(84).StringCellValue == "SANAL POS TAHSILAT")
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                        odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.KalanTaksitTutari = 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else if (row.GetCell(84).StringCellValue == "KREDİ KARTI")
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                        odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.KalanTaksitTutari = 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else if (row.GetCell(84).StringCellValue == "NAKİT ÖDEME")
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                        odmm.OdemeTipi = OdemeTipleri.Nakit;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odmm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else if (row.GetCell(84).StringCellValue == "HAVALE")
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odmm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odmm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }
                            }



                        }
                    }

                    // add pol to list

                    pol.GenelBilgiler.Durum = 0;
                    pol.GenelBilgiler.YenilemeNo = 0;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 0) pol.GenelBilgiler.OdemeSekli = 0;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 1) pol.GenelBilgiler.OdemeSekli = 1;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count > 1) pol.GenelBilgiler.OdemeSekli = 2;



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
                }

            }
            catch (Exception ex)
            {
                this.message = ex.ToString();
                policeler = null;
            }
            return policeler;
        }
        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            // get excel file...
            FileStream excelFile = null, excelFile1 = null;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string musteriEmaili = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            try
            {
                excelFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }

            ISheet sheet = null;
            if (filePath.Contains(".xlsx"))
            {
                XSSFWorkbook wb1 = new XSSFWorkbook(excelFile);
                sheet = wb1.GetSheet("veri");
            }
            else
            {
                wb = new HSSFWorkbook(excelFile);
                sheet = wb.GetSheet("veri");
            }


            // check sheet correct... 
            int startRow = Util.checkSheetCorrect(sheet, columnNames);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }

            ////////////
            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();
            string[] tempPath = filePath.Split('#');
            if (tempPath.Length > 1)
            {
                policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                filePath = filePath.Substring(0, filePath.IndexOf("#"));
                //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\NeoOnline_TahsilatKapatma.xls";
                //excelFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                //wb1 = new HSSFWorkbook(excelFile);
                //ISheet sheet1 = wb1.GetSheet("Sheet1");

                //for (int indx = sheet1.FirstRowNum + 2; indx <= sheet1.LastRowNum; indx++)
                //{
                //    IRow row = sheet1.GetRow(indx);
                //    var pno = row.GetCell(1).ToString();
                //    var eno = row.GetCell(2).ToString();
                //    var yno = row.GetCell(3).ToString();
                //    var kkno = row.GetCell(7).StringCellValue.Trim();
                //    neoOnline_TahsilatKapatma = new NeoOnline_TahsilatKapatma { Police_No = pno, Yenileme_No = yno, Ek_No = eno, Kart_No = kkno };
                //    policeTahsilatKapatma.Add(neoOnline_TahsilatKapatma);
                //}
            }
            /////////////
            // sheet correct. Start to get rows...
            try
            {
                for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
                {
                    IRow row = sheet.GetRow(indx);

                    // null rowlar icin
                    if (row == null) break;

                    Police pol = new Police();
                    PoliceOdemePlani[] odemeler = { new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani(), new PoliceOdemePlani() };

                    // Birlik Kodu
                    pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AKSIGORTA;
                    // tvm kodu
                    pol.GenelBilgiler.TVMKodu = tvmKodu;

                    List<ICell> cels = row.Cells;
                    var tanimliOdemeTipleri = _TVMService.GetListTanımliBransOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.AKSIGORTA);

                    foreach (ICell cell in cels)
                    {
                        int colIndex = cell.ColumnIndex;

                        switch (colIndex)
                        {
                            case 1:
                                pol.GenelBilgiler.PoliceNumarasi = cell.ToString();
                                break;
                            case 4:
                                pol.GenelBilgiler.EkNo = Util.toInt(cell.ToString());
                                break;
                            case 17:
                                pol.GenelBilgiler.BaslangicTarihi = cell.DateCellValue;
                                break;
                            case 18:
                                pol.GenelBilgiler.BitisTarihi = cell.DateCellValue;
                                break;
                            case 15:
                                pol.GenelBilgiler.TanzimTarihi = cell.DateCellValue;
                                break;
                            case 11:
                                tumUrunKodu = cell.StringCellValue;
                                break;
                            case 22://sgr ettiren
                                //var temp = cell.ToString().Split(' ');
                                string tempAd = "", tempSoyad = "";
                                tempAd = cell.ToString();
                                tempAd = tempAd.Substring(0, tempAd.LastIndexOf(' ') > 0 ? tempAd.LastIndexOf(' ') : tempAd.Length).Trim();
                                tempSoyad = cell.ToString();
                                if (tempSoyad.LastIndexOf(' ') > 0)
                                    tempSoyad = tempSoyad.Substring(tempSoyad.LastIndexOf(' ')).Trim();
                                else
                                    tempSoyad = "";
                                pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = tempAd;
                                pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = tempSoyad;

                                break;
                            case 23:
                                string kimlikNo = cell.ToString();
                                kimlikNo = kimlikNo == "0" ? "" : kimlikNo;
                                pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = kimlikNo;

                                break;
                            case 24:
                                kimlikNo = cell.ToString();
                                kimlikNo = kimlikNo == "0" ? "" : kimlikNo;
                                pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = kimlikNo;

                                break;
                            case 26: // sigortalı 
                                tempAd = cell.ToString();
                                tempAd = tempAd.Substring(0, tempAd.LastIndexOf(' ') > 0 ? tempAd.LastIndexOf(' ') : tempAd.Length).Trim();
                                tempSoyad = cell.ToString();
                                if (tempSoyad.LastIndexOf(' ') > 0)
                                    tempSoyad = tempSoyad.Substring(tempSoyad.LastIndexOf(' ')).Trim();
                                else
                                    tempSoyad = "";

                                pol.GenelBilgiler.PoliceSigortali.AdiUnvan = tempAd;
                                pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan = tempSoyad;

                                pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan) ? pol.GenelBilgiler.PoliceSigortali.AdiUnvan : pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan;
                                pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan) ? pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan : pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan;

                                break;
                            case 27:// sigortalı
                                kimlikNo = cell.ToString();
                                kimlikNo = kimlikNo == "0" ? "" : kimlikNo;
                                pol.GenelBilgiler.PoliceSigortali.KimlikNo = kimlikNo;
                                sLiKimlikNo = kimlikNo;

                                pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                                sEttirenKimlikNo = string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;

                                break;
                            case 28:// sigortalı
                                kimlikNo = cell.ToString();
                                kimlikNo = kimlikNo == "0" ? "" : kimlikNo;
                                pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = kimlikNo;

                                sLiKimlikNo = string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                                pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo) ? pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                                sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                                break;
                            case 110:// sigortalı
                                var temp = cell.ToString();
                                if (!string.IsNullOrEmpty(temp))
                                    if (DateTime.TryParse(temp, out DateTime dateTime))
                                    {
                                        pol.GenelBilgiler.PoliceSigortali.DogumTarihi = dateTime;
                                        pol.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = dateTime;
                                    }
                                break;
                            case 100:
                                if (cell.ColumnIndex != null)
                                {
                                    pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(2, cell.StringCellValue.Length - 2) : "";
                                    pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(0, 2) : "";
                                }
                                break;
                            case 97:
                                pol.GenelBilgiler.PoliceArac.Marka = cell.ToString();
                                break;
                            case 99:
                                pol.GenelBilgiler.PoliceArac.MotorNo = cell.ToString();
                                break;
                            case 102:
                                pol.GenelBilgiler.PoliceArac.SasiNo = cell.ToString();
                                break;
                            case 30:
                                pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                                if (pol.GenelBilgiler.ParaBirimi == "YTL")
                                {
                                    pol.GenelBilgiler.ParaBirimi = "TL";
                                }
                                break;
                            case 31:
                                pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.ToString());

                                break;
                            case 48: // pesinat
                                odemeler[0].TaksitNo = 1;
                                odemeler[0].VadeTarihi = cell.DateCellValue;
                                break;
                            case 49:
                                odemeler[0].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[0].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[0].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 51:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[1].TaksitNo = 2;
                                    odemeler[1].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 52:
                                odemeler[1].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[1].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[1].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 54:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[2].TaksitNo = 3;
                                    odemeler[2].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 55:
                                odemeler[2].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[2].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[2].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 57:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[3].TaksitNo = 4;
                                    odemeler[3].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 58:
                                odemeler[3].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[3].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[3].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 60:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[4].TaksitNo = 5;
                                    odemeler[4].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 61:
                                odemeler[4].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[4].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[4].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 63:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[5].TaksitNo = 6;
                                    odemeler[5].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 64:
                                odemeler[5].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[5].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[5].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 66:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[6].TaksitNo = 7;
                                    odemeler[6].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 67:
                                odemeler[6].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[6].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[6].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 69:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[7].TaksitNo = 8;
                                    odemeler[7].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 70:
                                odemeler[7].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[7].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[7].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 72:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[8].TaksitNo = 9;
                                    odemeler[8].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 73:
                                odemeler[8].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[8].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[8].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 75:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[9].TaksitNo = 10;
                                    odemeler[9].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 76:
                                odemeler[9].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[9].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[9].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 78:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[10].TaksitNo = 11;
                                    odemeler[10].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 79:
                                odemeler[10].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[10].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[10].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 81:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[11].TaksitNo = 12;
                                    odemeler[11].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 82:
                                odemeler[11].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[11].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                if (pol.GenelBilgiler.ParaBirimi != "YTL" && pol.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    odemeler[11].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 84:
                                if (cell.DateCellValue != null)
                                {
                                    odemeler[12].TaksitNo = 13;
                                    odemeler[12].VadeTarihi = cell.DateCellValue;
                                }
                                break;
                            case 85:
                                odemeler[12].TaksitTutari = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odemeler[12].TaksitTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    odemeler[12].DovizliTaksitTutari = Util.ToDecimal(cell.ToString());
                                }
                                break;

                            case 34:
                                pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 36: // Gider vergisi
                                PoliceVergi gv = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    gv.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi = Util.ToDecimal(cell.ToString());
                                    gv.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                gv.VergiKodu = 2;
                                pol.GenelBilgiler.PoliceVergis.Add(gv);
                                break;
                            case 38: // YSV
                                PoliceVergi ysv = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi += Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    ysv.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi += Util.ToDecimal(cell.ToString());
                                    ysv.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                ysv.VergiKodu = 4;
                                pol.GenelBilgiler.PoliceVergis.Add(ysv);
                                break;
                            //case 57: // GF + THGF
                            //    pol.GenelBilgiler.ToplamVergi += Util.toDecimal(cell.ToString());
                            //    PoliceVergi gfthgf = new PoliceVergi();
                            //    gfthgf.VergiKodu = 1;
                            //    gfthgf.VergiTutari = Util.toDecimal(cell.ToString());
                            //    pol.GenelBilgiler.PoliceVergis.Add(gfthgf);
                            //    break;
                            case 32:
                                pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(cell.ToString());
                                }
                                break;
                            case 44:
                                pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.ToString());
                                if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.BrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.NetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.Komisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    // dovizKuru = 0;
                                }
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(cell.ToString());
                                }
                                break;

                            case 103:
                                pol.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                                break;
                            //case 73:
                            //    musteriEmaili = cell.StringCellValue;
                            //    pol.GenelBilgiler.PoliceSigortali.EMail = musteriEmaili;
                            //    pol.GenelBilgiler.PoliceSigortaEttiren.EMail = musteriEmaili;

                            //    break;
                            case 88:
                                pol.GenelBilgiler.PoliceArac.KullanimTarzi = cell.StringCellValue;
                                break;
                            case 42:
                                PoliceVergi gf = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi += Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    gf.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi += Util.ToDecimal(cell.ToString());
                                    gf.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                gf.VergiKodu = 3;
                                pol.GenelBilgiler.PoliceVergis.Add(gf);
                                break;
                            case 40:
                                PoliceVergi thgf = new PoliceVergi();
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    pol.GenelBilgiler.ToplamVergi += Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                    thgf.VergiTutari = Math.Round(Util.ToDecimal(cell.ToString()) * pol.GenelBilgiler.DovizKur.Value, 2);
                                }
                                else
                                {
                                    pol.GenelBilgiler.ToplamVergi += Util.ToDecimal(cell.ToString());
                                    thgf.VergiTutari = Util.ToDecimal(cell.ToString());
                                }
                                thgf.VergiKodu = 1;
                                pol.GenelBilgiler.PoliceVergis.Add(thgf);
                                break;
                        }
                    }
                    // Odeme planini duzenle
                    PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                    PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                    pol.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                    pol.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;

                    var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, pol.GenelBilgiler);

                    for (int i = 0; i < odemeler.Length; i++)
                    {
                        PoliceOdemePlani odm = odemeler[i];
                        odm.TaksitTutari = odm.TaksitTutari != null ? odm.TaksitTutari : 0;
                        var tempOdemetipi = row.GetCell(108).ToString();
                        if (tempOdemetipi == "SPS")
                        {
                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                        }
                        else if (tempOdemetipi == "NKT")
                        {
                            odm.OdemeTipi = OdemeTipleri.Nakit;
                        }
                        else if (tempOdemetipi == "HAVALE")
                        {
                            odm.OdemeTipi = OdemeTipleri.Havale;
                        }
                        else
                        {
                            odm.OdemeTipi = OdemeTipleri.Havale;
                        }
                        if (odm.TaksitTutari != 0) // tutar pozitif ise odeme planina ekle
                        {
                            pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                        }

                        #region Tahsilat işlemi


                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.AKSIGORTA, pol.GenelBilgiler.BransKodu.Value);

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
                                        tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
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
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                            }

                        }
                        else
                        {
                            tempOdemetipi = row.GetCell(108).ToString();
                            if (!String.IsNullOrEmpty(tempOdemetipi))
                            {
                                if (tempOdemetipi == "SPS")
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.KalanTaksitTutari = 0;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                                //else if (row.GetCell(84).StringCellValue == "KREDİ KARTI")
                                //{
                                //    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                //    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                //    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                //    tahsilat.OtomatikTahsilatiKkMi = 1;
                                //    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                //    tahsilat.TaksitNo = odm.TaksitNo;
                                //    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                //    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                //    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                //    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                //    tahsilat.KalanTaksitTutari = 0;
                                //    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                //    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                //    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                //    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                //    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                //    tahsilat.KayitTarihi = DateTime.Today;
                                //    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                //    tahsilat.TahsilatId = odm.PoliceId;
                                //    if (tahsilat.TaksitTutari != 0)
                                //    {
                                //        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                //            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                //    }
                                //}
                                else if (tempOdemetipi == "NKT")
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
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
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                                //else if (row.GetCell(84).StringCellValue == "HAVALE")
                                //{
                                //    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                //    tahsilat.OdemTipi = OdemeTipleri.Havale;
                                //    odm.OdemeTipi = OdemeTipleri.Havale;
                                //    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                //    tahsilat.TaksitNo = odm.TaksitNo;
                                //    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                //    //tahsilat.OdemeBelgeNo = "111111";
                                //    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                //    tahsilat.OdenenTutar = 0;
                                //    tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                //    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                //    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                //    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                //    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                //    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                //    tahsilat.KayitTarihi = DateTime.Today;
                                //    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                //    tahsilat.TahsilatId = odm.PoliceId;
                                //    if (tahsilat.TaksitTutari != 0)
                                //    {
                                //        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                //            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                //    }
                                //}
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
                                    tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }


                                }
                            }
                        }



                        #endregion
                    }

                    if (odemeler.Count() == 0 && pol.GenelBilgiler.BrutPrim.Value != 0)
                    {
                        PoliceOdemePlani odmm = new PoliceOdemePlani();
                        var tempOdemetipi = row.GetCell(108).ToString();
                        if (odmm.TaksitTutari == null)
                        {
                            odmm.TaksitTutari = pol.GenelBilgiler.BrutPrim;
                            if (odmm.VadeTarihi == null)
                            {
                                odmm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                            }
                            odmm.OdemeTipi = OdemeTipleri.Havale;
                            odmm.TaksitNo = 1;
                            if (tempOdemetipi == "SPS")
                            {
                                odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                            }
                            //else if (tempOdemetipi == "KREDİ KARTI")
                            //{
                            //    odmm.OdemeTipi = OdemeTipleri.KrediKarti;

                            //}
                            else if (tempOdemetipi == "NKT")
                            {
                                odmm.OdemeTipi = OdemeTipleri.Nakit;
                            }
                            else if (tempOdemetipi == "HAVALE")
                            {
                                odmm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            else
                            {
                                odmm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            pol.GenelBilgiler.PoliceOdemePlanis.Add(odmm);

                            var restahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, pol.GenelBilgiler);


                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.AKSIGORTA, pol.GenelBilgiler.BransKodu.Value);

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
                                        odmm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                        if (tahsilat.OdemTipi == 1)
                                        {
                                            tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                            tahsilat.KalanTaksitTutari = 0;
                                            tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                            tahsilat.OtomatikTahsilatiKkMi = 1;
                                        }
                                        else
                                        {
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odmm.TaksitTutari;
                                        }
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                tempOdemetipi = row.GetCell(108).ToString();
                                if (!String.IsNullOrEmpty(tempOdemetipi))
                                {
                                    if (tempOdemetipi == "SPS")
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                        odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.KalanTaksitTutari = 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    //else if (row.GetCell(84).StringCellValue == "KREDİ KARTI")
                                    //{
                                    //    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    //    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                    //    odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    //    tahsilat.OtomatikTahsilatiKkMi = 1;
                                    //    tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                    //    tahsilat.TaksitNo = odmm.TaksitNo;
                                    //    tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                    //    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                    //    tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                    //    tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                    //    tahsilat.KalanTaksitTutari = 0;
                                    //    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    //    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    //    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    //    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    //    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    //    tahsilat.KayitTarihi = DateTime.Today;
                                    //    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    //    if (tahsilat.TaksitTutari != 0)
                                    //    {
                                    //        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                    //            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    //    }
                                    //}
                                    else if (tempOdemetipi == "NKT")
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                        odmm.OdemeTipi = OdemeTipleri.Nakit;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odmm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else if (tempOdemetipi == "HAVALE")
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odmm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                    else
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odmm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }
                            }



                        }
                    }

                    // add pol to list

                    pol.GenelBilgiler.Durum = 0;
                    pol.GenelBilgiler.YenilemeNo = 0;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 0) pol.GenelBilgiler.OdemeSekli = 0;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 1) pol.GenelBilgiler.OdemeSekli = 1;
                    if (pol.GenelBilgiler.PoliceOdemePlanis.Count > 1) pol.GenelBilgiler.OdemeSekli = 2;



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

        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }

        string tahsilatKapatmaVarmi(List<NeoOnline_TahsilatKapatma> neoOnline_TahsilatKapatmas = null, PoliceGenel police = null)
        {
            foreach (var item in neoOnline_TahsilatKapatmas)
            {
                if (item.Police_No.Trim() == police.PoliceNumarasi.Trim() && item.Yenileme_No.Trim() == police.YenilemeNo.Value.ToString().Trim() && item.Ek_No.Trim() == police.EkNo.Value.ToString().Trim() && !_TVMService.CheckListTVMBankaCariHesaplari(_AktifKullanici.TVMKodu, 5, item.Kart_No.Trim()))
                {
                    return item.Kart_No.Trim();
                }
            }
            return "";
        }

    }
}
