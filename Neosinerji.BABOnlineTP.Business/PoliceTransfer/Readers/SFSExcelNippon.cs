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
    // SFSExce????? classlari bu nedenden dolayi hazirlandi.

    class SFSExcelNippon
    {
        HSSFWorkbook wb, wb1;
        IAktifKullaniciService _AktifKullanici;
        ITVMService _TVMService;
        private string message = string.Empty;
        private string excelFileName;
        private string birlikKodu;
        private int tvmKodu;

        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;

        private string[] columnNames =  {
                                           "P Poliçe No", //0
                                            "P Yenileme No",//1
                                           "P Zeyil No",//2                                         
                                           "P Tanzim Tarihi",//3
                                           "P Onay Tarihi",//4
                                           "P Baş.Tarih",//5
                                           "P Bit. Tarihi",//6
                                          "P İptal Tarihi",//7
                                           "U Sigortalı Adı",//8
                                           "U Sig. Adresi",//9
                                           "U Sig. İl-1",//10
                                           "U Sig. Vergi Numarası",//11                                        
                                           "U Sig. TC Kimlik No",//12
                                           "U Müşteri Adı",//13
                                           "U Müş. Adresi",//14
                                           "U Müş. İl",//15
                                           "U Müş. Vergi Numarası",//16                                          
                                           "U Müş. TC Kimlik No",//17
                                           "P Riziko Adresi",//18
                                           "P Ürün No",//19
                                           "P Ürün Adı",//20
                                           "P Plaka",//21
                                           "P Brüt Prim",//22
                                           "P Komisyon",//23
                                           "P GDV",//24
                                           "P GF",//25
                                           "P THGF",//26
                                           "P YSV",//27
                                           "P Döviz Cinsi",//28
                                           "P Döviz Kuru",//29
                                           "P Peşin (C) / Vadeli (I)",//30
                                           "P Taksit Sayısı",//31
                                           "P Taksit Tarihi",//32
                                           "P Taksit Tutarı",//33
                                           "T Taksit Ödeme Miktarı",//34
                                           "T Taksit Ödeme Tarihi",//35
                                           "P Eski Poliçe No",//36
                                           "P Net Prim",//37
                                           "U Sig. Vergi Dairesi",//38
                                           "SYS Kullanıcı Adı",//39
                                           "MARKA",//40
                                           "MARKA KODU",//41
                                           "MOTOR NO",//42
                                           "ŞASİ NO",//43
                                            "TİP",//44
                                           "MODEL",//45                                          
                                           "KULLANIM TARZI",//46
                                           "U Sig. Telefon1",//47
                                           "U Sig. Telefon2",//48
                                           "U Sig. Doğum Tarihi",//49
                                           "U Sig. Doğum Yeri",//50
                                           "P Zeyl Tipi",//51
                                           "P Sanal POS?", //52
                                           "P Ödeme Şekli",//53
                                           "P Ödeme Tipi",//54
                                           "EGM CİNSİ",
                                           "EGM MARKA",
                                           "EGM TİPİ",
                                           "U Sig. İl-2"
                                        };

        #region
        public SFSExcelNippon(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
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
            FileStream excelFile = null, excelFile1 = null;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            int? bransKod = null;
            string policemNo = null;
            DateTime? polBasTarihi = null;
            int? ekNom = null;
            string psliKimlikNo = null;
            decimal? polKomisyon = null;
            string psliVknNo = null;
            string odemeTipi = null;
            decimal? odenenTaksitTutari = null;
            string bransAdi = null;
            int? bransKodu = null;
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


            ////////////
            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();
            string[] tempPath = excelFileName.Split('#');
            if (tempPath.Length > 1)
            {
                policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                excelFileName = excelFileName.Substring(0, excelFileName.IndexOf("#"));
                //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\NeoOnline_TahsilatKapatma.xls";
                //excelFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                //wb1 = new HSSFWorkbook(excelFile);
                //ISheet sheet1 = wb1.GetSheet("Sheet1");

                //for (int indx = sheet1.FirstRowNum + 2; indx <= sheet1.LastRowNum; indx++)
                //{
                //    IRow row = sheet1.GetRow(indx);
                //    var pno = row.GetCell(1).NumericCellValue.ToString();
                //    var eno = row.GetCell(2).NumericCellValue.ToString();
                //    var yno = row.GetCell(3).NumericCellValue.ToString();
                //    var kkno = row.GetCell(7).StringCellValue.Trim();
                //    neoOnline_TahsilatKapatma = new NeoOnline_TahsilatKapatma { Police_No = pno, Yenileme_No = yno, Ek_No = eno, Kart_No = kkno };
                //    policeTahsilatKapatma.Add(neoOnline_TahsilatKapatma);
                //}
            }
            /////////////


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
                    PoliceTahsilat tahsilat = new PoliceTahsilat();




                    // tvm kodu
                    pol.GenelBilgiler.TVMKodu = tvmKodu;

                    // Birlik Kodu
                    pol.GenelBilgiler.TUMBirlikKodu = birlikKodu;


                    List<ICell> cels = row.Cells;
                    #endregion
                    foreach (ICell cell in cels)
                    {

                        if (cell.ColumnIndex == 0) pol.GenelBilgiler.PoliceNumarasi = cell.StringCellValue;
                        policemNo = pol.GenelBilgiler.PoliceNumarasi;

                        if (cell.ColumnIndex == 2) pol.GenelBilgiler.EkNo = Util.toInt(row.GetCell(2).StringCellValue);
                        ekNom = pol.GenelBilgiler.EkNo;

                        if (cell.ColumnIndex == 1) pol.GenelBilgiler.YenilemeNo = Util.toInt(cell.StringCellValue);
                        if (cell.ColumnIndex == 5) pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;

                        if (cell.ColumnIndex == 6) pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        if (cell.ColumnIndex == 3) pol.GenelBilgiler.TanzimTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        if (cell.ColumnIndex == 8) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 9) pol.GenelBilgiler.PoliceSigortali.Adres = cell.StringCellValue;
                        if (cell.ColumnIndex == 10) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 11) pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.StringCellValue;

                        if (cell.ColumnIndex == 12) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue;

                        if (cell.ColumnIndex == 13) pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 14) pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.StringCellValue;
                        if (cell.ColumnIndex == 15) pol.GenelBilgiler.PoliceSigortaEttiren.IlAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 17) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 16) pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.StringCellValue;
                        if (pol.GenelBilgiler.PoliceSigortali.KimlikNo == "0")
                        {
                            pol.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                        }
                        if (pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "0")
                        {
                            pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                        }
                        if (pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "0")
                        {
                            pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                        }
                        if (pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "0")
                        {
                            pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                        }
                        psliKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        psliVknNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                        if (cell.ColumnIndex == 18) pol.GenelBilgiler.PoliceRizikoAdresi.Adres = cell.StringCellValue;
                        if (cell.ColumnIndex == 19) tumUrunKodu = cell.StringCellValue;
                        if (cell.ColumnIndex == 20) tumUrunAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 21)
                        {
                            pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(2, cell.StringCellValue.Length - 2) : "";
                            pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(0, 2) : "";
                        }
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
                            pol.GenelBilgiler.Komisyon = Util.ToDecimal(row.GetCell(23).NumericCellValue.ToString());
                            polKomisyon = pol.GenelBilgiler.Komisyon;
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

                        if (cell.ColumnIndex == 28) pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                        if (cell.ColumnIndex == 29)
                        {
                            if (!String.IsNullOrEmpty(cell.StringCellValue))
                            {
                                pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.StringCellValue.Replace(".", ","));
                            }
                        }
                        //if (pol.GenelBilgiler.ParaBirimi != "TL")
                        //{
                        //    if (cell.ColumnIndex == 29) dovizKuru = Util.ToDecimal(cell.StringCellValue);
                        //}
                        if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                        {
                            pol.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                            pol.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
                        }

                        // odeme plani

                        var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, pol.GenelBilgiler);
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
                                if (odemeTipi == "Kredi Kartı")
                                {
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                }
                                else if (odemeTipi == "Nakit")
                                {
                                    odm.OdemeTipi = OdemeTipleri.Nakit;
                                }
                                else
                                {
                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                }
                                if (odm.TaksitTutari != 0)
                                {
                                    pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                }
                                else if (odm.TaksitTutari == 0 && odm.TaksitNo == 1 && pol.GenelBilgiler.BrutPrim != 0)
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
                                    if (odemeTipi == "Kredi Kartı")
                                    {
                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    }
                                    else if (odemeTipi == "Nakit")
                                    {
                                        odm.OdemeTipi = OdemeTipleri.Nakit;
                                    }
                                    else
                                    {
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                    }
                                    odm.TaksitNo = 1;
                                    if (odm.TaksitTutari != 0)
                                    {
                                        pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }


                                }
                                if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                                {
                                    #region Tahsilat işlemi
                                    PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                    PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                    pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                    pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                                    bransAdi = PoliceBransEslestir2.BransAdi;
                                    bransKodu = PoliceBransEslestir2.BransKodu;
                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.TURKNIPPONSIGORTA, pol.GenelBilgiler.BransKodu.Value);



                                    if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                    {
                                        int otoOdeSayac = 0;
                                        foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                        {
                                            if (otoOdeSayac < 1 && pol.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                            {
                                                otoOdeSayac++;
                                                //PoliceTahsilat tahsilat = new PoliceTahsilat();

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
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
                                        if (odemeTipi == "Kredi Kartı")
                                        {
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
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                            tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                            tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                            //tahsilat.TahsilatId = odm.PoliceId;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                    pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                        else if (odemeTipi == "Nakit")
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                            odm.OdemeTipi = OdemeTipleri.Nakit;
                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                            tahsilat.TaksitNo = odm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                            //tahsilat.OdemeBelgeNo = "11111111";
                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = policemNo;
                                            tahsilat.ZeyilNo = ekNom.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                            tahsilat.BrutPrim = polBrutprimim.Value;
                                            tahsilat.PoliceId = odm.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                                            //tahsilat.TahsilatId = odm.PoliceId;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                    pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }
                                        else
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                            tahsilat.TaksitNo = odm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                            // tahsilat.OdemeBelgeNo = "11111111";
                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = policemNo;
                                            tahsilat.ZeyilNo = ekNom.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                            tahsilat.BrutPrim = polBrutprimim.Value;
                                            tahsilat.PoliceId = odm.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                                            //tahsilat.TahsilatId = odm.PoliceId;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                    pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }


                                        }
                                    }

                                    #endregion
                                }

                            }

                        }
                        if (cell.ColumnIndex == 34)
                        {
                            odenenTaksitTutari = carpan * Util.ToDecimal(cell.NumericCellValue.ToString());
                        }
                        if (cell.ColumnIndex == 37)
                        {
                            pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                            if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                            {
                                pol.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                            }
                            if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                            {
                                pol.GenelBilgiler.BrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                pol.GenelBilgiler.NetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                pol.GenelBilgiler.Komisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                polBrutprimim = pol.GenelBilgiler.BrutPrim;
                            }
                        }

                        if (cell.ColumnIndex == 40) pol.GenelBilgiler.PoliceArac.MarkaAciklama = cell.StringCellValue;
                        if (cell.ColumnIndex == 41) pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue;
                        if (cell.ColumnIndex == 42) pol.GenelBilgiler.PoliceArac.MotorNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 43) pol.GenelBilgiler.PoliceArac.SasiNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 44) pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = cell.StringCellValue;
                        if (cell.ColumnIndex == 45) pol.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToInt32(cell.StringCellValue) : 0;
                        if (cell.ColumnIndex == 46) pol.GenelBilgiler.PoliceArac.KullanimTarzi = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToString(cell.StringCellValue) : null;
                        if (cell.ColumnIndex == 47) pol.GenelBilgiler.PoliceSigortali.TelefonNo = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToString(cell.StringCellValue) : null;
                        if (cell.ColumnIndex == 48) pol.GenelBilgiler.PoliceSigortali.MobilTelefonNo = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToString(cell.StringCellValue) : null;
                        if (cell.ColumnIndex == 49 && !String.IsNullOrEmpty(cell.StringCellValue)) pol.GenelBilgiler.PoliceSigortali.DogumTarihi = Convert.ToDateTime(cell.StringCellValue);
                        if (cell.ColumnIndex == 51) pol.GenelBilgiler.ZeyilAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 54)
                        {
                            odemeTipi = cell.StringCellValue;

                        }

                        //if (cell.ColumnIndex == 58) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.StringCellValue;

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


                // Odeme planinin deiger taksitleri icin
                if (row.FirstCellNum == 32)
                {
                    // Odeme Plani - diger taksitler

                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    PoliceTahsilat tahsilats = new PoliceTahsilat();

                    var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, odemePol.GenelBilgiler);


                    taksitNo += 1;
                    odm.TaksitNo = taksitNo;
                    odm.VadeTarihi = Util.toDate(row.GetCell(32).StringCellValue, Util.DateFormat1);
                    if (row.GetCell(33) != null)
                    {
                        odm.TaksitTutari = Convert.ToDecimal(carpan * Util.ToDecimal(row.GetCell(33).NumericCellValue.ToString())); // iptal ise tutuar eksi deger olmali
                        if (odemePol.GenelBilgiler.DovizKur != 1 && odemePol.GenelBilgiler.DovizKur != 0 && odemePol.GenelBilgiler.DovizKur != null)
                        {
                            odm.TaksitTutari = Util.ToDecimal(row.GetCell(33).NumericCellValue.ToString()) * carpan;
                            odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(row.GetCell(33).NumericCellValue.ToString()) / odemePol.GenelBilgiler.DovizKur.Value * carpan, 2);
                        }
                    }
                    odemePol.GenelBilgiler.OdemeSekli = 2; // Vadeli - taksit varsa vadeli
                    if (odemeTipi == "Kredi Kartı")
                    {
                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                    }
                    else if (odemeTipi == "Nakit")
                    {
                        odm.OdemeTipi = OdemeTipleri.Nakit;
                    }
                    else
                    {
                        odm.OdemeTipi = OdemeTipleri.Havale;
                    }
                    if (odm.TaksitTutari != 0)
                    {
                        #region Tahsilat işlemi
                        PoliceGenelBrans PoliceBransEslestir3 = new PoliceGenelBrans();
                        PoliceBransEslestir3 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                        bransAdi = PoliceBransEslestir3.BransAdi;
                        bransKodu = PoliceBransEslestir3.BransKodu;
                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.TURKNIPPONSIGORTA, bransKodu.Value);


                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                        {
                            int otoOdeSayac = 0;
                            foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                            {
                                if (otoOdeSayac < 1 && bransKodu == itemOtoOdemeTipleri.BransKodu)
                                {
                                    otoOdeSayac++;
                                    //PoliceTahsilat tahsilat = new PoliceTahsilat();

                                    tahsilats.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                    odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                    if (tahsilats.OdemTipi == 1)
                                    {
                                        tahsilats.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilats.KalanTaksitTutari = 0;
                                        tahsilats.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                        tahsilats.OtomatikTahsilatiKkMi = 1;
                                    }
                                    else
                                    {
                                        tahsilats.OdenenTutar = 0;
                                        tahsilats.KalanTaksitTutari = odm.TaksitTutari;
                                    }
                                    tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                    tahsilats.TaksitNo = odm.TaksitNo;
                                    tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                    tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilats.PoliceNo = policemNo;
                                    tahsilats.ZeyilNo = ekNom.ToString();
                                    tahsilats.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                    tahsilats.BrutPrim = polBrutprimim.Value;
                                    tahsilats.PoliceId = odm.PoliceId;
                                    tahsilats.KayitTarihi = DateTime.Today;
                                    tahsilats.KaydiEkleyenKullaniciKodu = tvmKodu;
                                    if (tahsilats.TaksitTutari != 0)
                                    {
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilats);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (odemeTipi == "Kredi Kartı")
                            {
                                tahsilats.OdemTipi = OdemeTipleri.KrediKarti;
                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                tahsilats.OtomatikTahsilatiKkMi = 1;
                                tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                tahsilats.TaksitNo = odm.TaksitNo;
                                tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                tahsilats.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.KalanTaksitTutari = 0;
                                tahsilats.PoliceNo = policemNo;
                                tahsilats.ZeyilNo = ekNom.ToString();
                                tahsilats.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                tahsilats.BrutPrim = polBrutprimim.Value;
                                tahsilats.PoliceId = odm.PoliceId;
                                tahsilats.KayitTarihi = DateTime.Today;
                                tahsilats.KaydiEkleyenKullaniciKodu = tvmKodu;
                                //tahsilats.TahsilatId = odm.PoliceId;
                                if (tahsilats.TaksitTutari != 0)
                                {
                                    //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilats);
                                }
                            }
                            else if (odemeTipi == "Nakit")
                            {
                                tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                odm.OdemeTipi = OdemeTipleri.Nakit;
                                tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                tahsilats.TaksitNo = odm.TaksitNo;
                                tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                //  tahsilat.OdemeBelgeNo = "11111111";
                                tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.OdenenTutar = 0;
                                tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.PoliceNo = policemNo;
                                tahsilats.ZeyilNo = ekNom.ToString();
                                tahsilats.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                tahsilats.BrutPrim = polBrutprimim.Value;
                                tahsilats.PoliceId = odm.PoliceId;
                                tahsilats.KayitTarihi = DateTime.Today;
                                tahsilats.KaydiEkleyenKullaniciKodu = tvmKodu;
                                //tahsilats.TahsilatId = odm.PoliceId;
                                if (tahsilats.TaksitTutari != 0)
                                {
                                    //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilats);
                                }
                            }
                            else
                            {
                                tahsilats.OdemTipi = OdemeTipleri.Havale;
                                odm.OdemeTipi = OdemeTipleri.Havale;
                                tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : polBasTarihi.Value;
                                tahsilats.TaksitNo = odm.TaksitNo;
                                tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                //   tahsilat.OdemeBelgeNo = "11111111";
                                tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.OdenenTutar = 0;
                                tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.PoliceNo = policemNo;
                                tahsilats.ZeyilNo = ekNom.ToString();
                                tahsilats.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                tahsilats.BrutPrim = polBrutprimim.Value;
                                tahsilats.PoliceId = odm.PoliceId;
                                tahsilats.KayitTarihi = DateTime.Today;
                                tahsilats.KaydiEkleyenKullaniciKodu = tvmKodu;
                                //tahsilats.TahsilatId = odm.PoliceId;
                                if (tahsilats.TaksitTutari != 0)
                                {
                                    //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        odemePol.GenelBilgiler.PoliceTahsilats.Add(tahsilats);
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
