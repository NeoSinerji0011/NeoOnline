using System.Text;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Collections.Generic;
using System;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class SFSExcelQuick
    {

        HSSFWorkbook wb;

        private string message = string.Empty;
        private string excelFileName;
        private string birlikKodu;
        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        ITVMService _TVMService;
        IPoliceTransferService _IPoliceTransferService;
        private string[] columnNames =  {
                                          "Poliçe No",//0
                                          "Zeyil No",//1
                                          "P Baş.Tarih",//2
                                          "P Bit. Tarihi",//3
                                          "P Brüt Prim",//4
                                          "P Döviz Cinsi",//5
                                          "P Döviz Kuru",//6
                                          "P GDV",//7
                                          "P GF",//8
                                          "P Kaynak Adı",//9
                                          "Kaynak No",//10
                                          "P Komisyon",//11
                                          "P Komisyoner Adı",//12
                                          "P Net Prim",//13
                                          "P Onay Tarihi",//14
                                          "P Peşin (C) / Vadeli (I)",//15
                                          "P Plaka",//16
                                          "P Riziko Adresi",//17
                                          "P THGF",//18
                                          "P Taksit Sayısı",//19
                                          "P Taksit Tarihi",//20
                                          "P Taksit Tutarı",//21
                                          "P Tanzim Tarihi",//22
                                          "P Toplam Vergi",//23
                                          "P YSV",//24
                                          "Yenileme No",//25
                                          "P Zeyl Tipi",//26
                                          "P Ürün Adı",//27
                                          "Ürün No",//28
                                          "SYS Kullanıcı Adı",//29
                                          "U Kay. E-posta Adresi",//30
                                          "U Kay. Faks1",//31
                                          "U Kay. Telefon1",//32
                                          "U Kay. Vergi Dairesi",//33
                                          "U Kay. Vergi Numarası",//34
                                          "U Kay. İl",//35
                                          "U Müş. Adresi",//36
                                          "U Müş. Doğum Tarihi",//37
                                          "U Müş. E-posta Adresi",//38
                                          "U Müş. Kimlik No",//39
                                          "U Müş. Mesleği",//40
                                          "U Müş. TC Kimlik No",//41
                                          "U Müş. Telefon1",//42
                                          "U Müş. Vergi Dairesi",//43
                                          "U Müş. Vergi Numarası",//44
                                          "U Müş. İl",//45
                                          "U Müş. İlçe",//46
                                          "U Müşteri Adı",//47
                                          "U Sig. TC Kimlik No",//48
                                          "U Sig. Telefon1",//49
                                          "U Sig. Vergi Dairesi",//50
                                          "U Sig. Vergi Numarası",//51
                                          "U Sig. İl",//52
                                          "U Sig. İlçe",//53
                                          "Sigortalı Adı",//54
                                          "MODEL YILI",//55
                                          "MARKA",//56
                                          "RUHSAT BELGE SERİ NO",//57
                                          "MOTOR NO",//58
                                          "ŞASİ NO",//59
                                          "MARKA KODU",//60
                                          "TİP"//61
                                          
       };
        public SFSExcelQuick(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.excelFileName = fileName;
            this.birlikKodu = birlikKodu;
            this.tvmKodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
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
            int? taksitler = null;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            int? bransKod = null;
            string policemNo = null;
            DateTime? polBasTarihi = null;
            int? ekNom = null;
            decimal? polBrutprimim = null;
            string bransAdi = null;
            int? bransKodu = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            try
            {
                excelFile = new FileStream(excelFileName, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }

            try
            {


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
                    decimal? polKomisyon = null;
                    string readerKulKodu = null;
                    string tempValue = "";
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

                            if (cell.ColumnIndex == 0) pol.GenelBilgiler.PoliceNumarasi = cell.StringCellValue;
                            policemNo = pol.GenelBilgiler.PoliceNumarasi;

                            if (cell.ColumnIndex == 1) pol.GenelBilgiler.EkNo = Util.toInt(row.GetCell(1).StringCellValue);
                            ekNom = pol.GenelBilgiler.EkNo;
                            if (cell.ColumnIndex == 2)
                            {
                                if (cell.StringCellValue.Contains("'"))
                                {
                                    pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue.Substring(1, 10), Util.DateFormat1);
                                    polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;

                                }
                                else
                                {
                                    pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                                    polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;

                                }
                            }
                            if (cell.ColumnIndex == 3)
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
                            if (cell.ColumnIndex == 4)
                            {
                                tempValue = cell.ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                pol.GenelBilgiler.BrutPrim = Util.ToDecimal(tempValue);
                                polBrutprimim = pol.GenelBilgiler.BrutPrim;
                            }
                            if (cell.ColumnIndex == 5) pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                            if (cell.ColumnIndex == 6)
                            {
                                if (!String.IsNullOrEmpty(cell.StringCellValue))
                                {
                                    pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.StringCellValue.Replace(".", ","));
                                }
                            }
                            if (cell.ColumnIndex == 7)
                            {
                                // Gider Vergisi
                                tempValue = cell.ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                pol.GenelBilgiler.ToplamVergi = 0;
                                PoliceVergi gv = new PoliceVergi();
                                gv.VergiKodu = 2;
                                //gv.VergiTutari = Util.ToDecimal(row.GetCell(7).StringCellValue);
                                gv.VergiTutari = Util.ToDecimal(tempValue);
                                pol.GenelBilgiler.ToplamVergi += gv.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(gv);
                            }
                            if (cell.ColumnIndex == 8)
                            {
                                // Garanti fonu
                                tempValue = cell.ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                PoliceVergi gf = new PoliceVergi();
                                gf.VergiKodu = 3;
                                //gf.VergiTutari = Util.ToDecimal(row.GetCell(8).StringCellValue);
                                gf.VergiTutari = Util.ToDecimal(tempValue);
                                pol.GenelBilgiler.ToplamVergi += gf.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(gf);
                            }
                            if (cell.ColumnIndex == 11)
                            {
                                tempValue = cell.ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                //pol.GenelBilgiler.Komisyon = Util.ToDecimal(row.GetCell(11).StringCellValue);
                                pol.GenelBilgiler.Komisyon = Util.ToDecimal(tempValue);
                            }
                            if (cell.ColumnIndex == 13)
                            {
                                tempValue = cell.ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                //pol.GenelBilgiler.NetPrim = Util.ToDecimal(row.GetCell(13).StringCellValue);
                                pol.GenelBilgiler.NetPrim = Util.ToDecimal(tempValue);
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliNetPrim = pol.GenelBilgiler.NetPrim.Value;
                                    pol.GenelBilgiler.DovizliBrutPrim = pol.GenelBilgiler.BrutPrim.Value;
                                    pol.GenelBilgiler.DovizliKomisyon = pol.GenelBilgiler.Komisyon.Value;

                                    pol.GenelBilgiler.BrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.NetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    pol.GenelBilgiler.Komisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                                    polBrutprimim = pol.GenelBilgiler.BrutPrim;
                                }
                            }
                            if (cell.ColumnIndex == 16)
                            {
                                pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(2, cell.StringCellValue.Length - 2) : "";
                                pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(0, 2) : "";
                            }
                            if (cell.ColumnIndex == 17) pol.GenelBilgiler.PoliceRizikoAdresi.Adres = cell.StringCellValue != null ? cell.StringCellValue : null;
                            if (cell.ColumnIndex == 18)
                            {
                                // THGF 
                                tempValue = cell.ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                PoliceVergi thgf = new PoliceVergi();
                                thgf.VergiKodu = 1;
                                //thgf.VergiTutari = Util.ToDecimal(row.GetCell(18).StringCellValue);
                                thgf.VergiTutari = Util.ToDecimal(tempValue);
                                pol.GenelBilgiler.ToplamVergi += thgf.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(thgf);
                            }
                            if (cell.ColumnIndex == 19)
                            {
                                taksitler = Convert.ToInt32(cell.StringCellValue);
                            }
                            if (cell.ColumnIndex == 20 || cell.ColumnIndex == 21)
                            {
                                if (cell.ColumnIndex == 20)
                                {
                                    // Odeme Plani - ilk taksit
                                    taksitNo = 1;
                                    odm.TaksitNo = taksitNo;
                                    // odm.VadeTarihi = Util.toDate(row.GetCell(20).StringCellValue, Util.DateFormat1);
                                    if (cell.StringCellValue.Contains("'"))
                                    {
                                        odm.VadeTarihi = Util.toDate(cell.StringCellValue.Substring(1, 10), Util.DateFormat1);
                                    }
                                    else
                                    {
                                        odm.VadeTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);

                                    }
                                }
                                if (cell.ColumnIndex == 21)
                                {
                                    tempValue = cell.ToString();
                                    tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                    PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                    PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                    pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                    pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                                    if (pol.GenelBilgiler.BrutPrim < 0)
                                    {
                                        odm.TaksitTutari = carpan * Util.ToDecimal(tempValue); // iptal ise tutuar eksi deger olmali
                                        if (odm.TaksitTutari > 0)
                                        {
                                            odm.TaksitTutari = -1 * Util.ToDecimal(tempValue); // iptal ise tutuar eksi deger olmali
                                        }
                                        else
                                        {
                                            odm.TaksitTutari = carpan * Util.ToDecimal(tempValue); // iptal ise tutuar eksi deger olmali
                                        }
                                    }
                                    else
                                    {
                                        odm.TaksitTutari = carpan * Util.ToDecimal(tempValue); // iptal ise tutuar eksi deger olmali
                                    }

                                    if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                    {
                                        if (pol.GenelBilgiler.BrutPrim < 0 && odm.TaksitTutari > 0)
                                        {
                                            odm.TaksitTutari = Util.ToDecimal(tempValue) * -1;
                                            odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(tempValue) / pol.GenelBilgiler.DovizKur.Value * -1, 2);
                                        }
                                        else
                                        {
                                            odm.TaksitTutari = Util.ToDecimal(tempValue);
                                            odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(tempValue) / pol.GenelBilgiler.DovizKur.Value * carpan, 2);
                                        }
                                    }
                                    if (bransKod == 1 || bransKod == 2)
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
                                        if (bransKod == 1 || bransKod == 2)
                                        {
                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
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

                                        bransAdi = PoliceBransEslestir2.BransAdi;
                                        bransKodu = PoliceBransEslestir2.BransKodu;
                                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.QUICKSIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                                            if (bransKod == 1 || bransKod == 2)
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
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
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
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
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
                            if (cell.ColumnIndex == 22)
                            {
                                if (cell.StringCellValue.Contains("'"))
                                {
                                    pol.GenelBilgiler.TanzimTarihi = Util.toDate(cell.StringCellValue.Substring(1, 10), Util.DateFormat1);
                                }
                                else
                                {
                                    pol.GenelBilgiler.TanzimTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                                }
                            }
                            if (cell.ColumnIndex == 24)
                            {
                                // YSV 
                                tempValue = cell.ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                PoliceVergi ysv = new PoliceVergi();
                                ysv.VergiKodu = 4;
                                //ysv.VergiTutari = Util.ToDecimal(row.GetCell(24).StringCellValue);
                                ysv.VergiTutari = Util.ToDecimal(tempValue);
                                pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                                pol.GenelBilgiler.PoliceVergis.Add(ysv);
                            }
                            if (cell.ColumnIndex == 25) pol.GenelBilgiler.YenilemeNo = Util.toInt(cell.StringCellValue);
                            if (cell.ColumnIndex == 26) pol.GenelBilgiler.ZeyilAdi = cell.StringCellValue;
                            if (cell.ColumnIndex == 27) tumUrunAdi = cell.StringCellValue;
                            if (cell.ColumnIndex == 28) tumUrunKodu = cell.StringCellValue;
                            if (cell.ColumnIndex == 29) readerKulKodu = cell.StringCellValue;
                            if (readerKulKodu != null)
                            {
                                var getReaderKodu = _IPoliceTransferService.GetPoliceReaderKullanicilari(readerKulKodu);
                                if (getReaderKodu != null)
                                {
                                    pol.GenelBilgiler.TaliAcenteKodu = Convert.ToInt32(getReaderKodu.AltTvmKodu);

                                }
                            }
                            if (cell.ColumnIndex == 36)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.StringCellValue != null ? cell.StringCellValue : null;
                                pol.GenelBilgiler.PoliceSigortali.Adres = cell.StringCellValue != null ? cell.StringCellValue : null;
                            }
                            if (cell.ColumnIndex == 37)
                            {
                                if (cell.StringCellValue.Contains("'"))
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = Util.toDate(cell.StringCellValue.Substring(1, 10), Util.DateFormat1);
                                }
                                else
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);

                                }
                            }
                            if (cell.ColumnIndex == 38)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.EMail = row.GetCell(38).StringCellValue;
                            }
                            if (cell.ColumnIndex == 41)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = row.GetCell(41).StringCellValue;
                            }
                            if (cell.ColumnIndex == 42) pol.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = cell.StringCellValue != null ? cell.StringCellValue : null;
                            if (cell.ColumnIndex == 44)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = row.GetCell(44).StringCellValue;
                                sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                            }
                            if (cell.ColumnIndex == 45) pol.GenelBilgiler.PoliceSigortaEttiren.IlAdi = cell.StringCellValue != null ? cell.StringCellValue : null;
                            if (cell.ColumnIndex == 46) pol.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = cell.StringCellValue != null ? cell.StringCellValue : null;
                            if (cell.ColumnIndex == 47) pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                            if (cell.ColumnIndex == 48)
                            {
                                pol.GenelBilgiler.PoliceSigortali.KimlikNo = row.GetCell(48).StringCellValue;
                            }
                            if (cell.ColumnIndex == 49) pol.GenelBilgiler.PoliceSigortali.TelefonNo = cell.StringCellValue != null ? cell.StringCellValue : null;
                            if (cell.ColumnIndex == 51)
                            {
                                pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = row.GetCell(51).StringCellValue;
                                sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            }
                            if (cell.ColumnIndex == 52) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.StringCellValue;
                            if (cell.ColumnIndex == 53) pol.GenelBilgiler.PoliceSigortali.IlceAdi = cell.StringCellValue;
                            if (cell.ColumnIndex == 54) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                            if (cell.ColumnIndex == 55) pol.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToInt32(cell.StringCellValue) : 0;
                            if (cell.ColumnIndex == 56) pol.GenelBilgiler.PoliceArac.MarkaAciklama = cell.StringCellValue;
                            if (cell.ColumnIndex == 58) pol.GenelBilgiler.PoliceArac.MotorNo = cell.StringCellValue;
                            if (cell.ColumnIndex == 59) pol.GenelBilgiler.PoliceArac.SasiNo = cell.StringCellValue;

                            if (cell.ColumnIndex == 60) pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue;
                            if (cell.ColumnIndex == 61) pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = cell.StringCellValue;


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
                        foreach (var item in pol.GenelBilgiler.PoliceTahsilats)
                        {
                            item.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                        }
                        policeler.Add(pol);
                        odemePol = pol;

                    }

                    // Odeme planinin deiger taksitleri icin
                    List<ICell> celsa = row.Cells;

                    //taksitler = Convert.ToInt32(row.GetCell(19).StringCellValue);
                    if (row.FirstCellNum == 20)
                    {
                        // Odeme Plani - diger taksitler
                        PoliceOdemePlani odm = new PoliceOdemePlani();
                        taksitNo += 1;
                        odm.TaksitNo = taksitNo;
                        // int taksitler = null;

                        if (taksitler >= taksitNo)
                        {
                            var test = row.GetCell(20);
                            tempValue = row.GetCell(20).ToString();
                            tempValue = tempValue.Contains("'") ? tempValue.Substring(1, 10) : tempValue;
                            odm.VadeTarihi = Util.toDate(tempValue, Util.DateFormat1);

                            if (row.GetCell(21) != null)
                            {
                                tempValue = row.GetCell(21).ToString();
                                tempValue = tempValue.Contains(",") ? tempValue.Replace(".", "").Replace(",", ".") : tempValue;
                                if (!String.IsNullOrEmpty(tempValue))
                                {
                                    odm.TaksitTutari = carpan * Util.ToDecimal(tempValue); // iptal ise tutuar eksi deger olmali
                                    if (odemePol.GenelBilgiler.BrutPrim < 0 && odm.TaksitTutari > 0)
                                    {
                                        odm.TaksitTutari = -1 * Util.ToDecimal(tempValue); // iptal ise tutuar eksi deger olmali
                                    }
                                    else
                                    {
                                        odm.TaksitTutari = carpan * Util.ToDecimal(tempValue); // iptal ise tutuar eksi deger olmali
                                    }
                                    if (odemePol.GenelBilgiler.DovizKur != 1 && odemePol.GenelBilgiler.DovizKur != 0 && odemePol.GenelBilgiler.DovizKur != null)
                                    {
                                        if (odemePol.GenelBilgiler.BrutPrim < 0 && odm.TaksitTutari > 0)
                                        {
                                            odm.TaksitTutari = Util.ToDecimal(tempValue) * -1;
                                            odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(tempValue) / odemePol.GenelBilgiler.DovizKur.Value * -1, 2);
                                        }
                                        else
                                        {
                                            odm.TaksitTutari = odm.TaksitTutari;
                                            odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(tempValue) / odemePol.GenelBilgiler.DovizKur.Value * carpan, 2);
                                        }
                                    }
                                }
                                else
                                {
                                    odm.TaksitTutari = 0;
                                }

                            }
                            if (bransKod == 1 || bransKod == 2)
                            {
                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                            }
                            else
                            {
                                odm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            odemePol.GenelBilgiler.OdemeSekli = 2; // Vadeli - taksit varsa vadeli

                            if (odm.TaksitTutari != 0)
                            {
                                #region Tahsilat işlemi

                                PoliceGenelBrans PoliceBransEslestir3 = new PoliceGenelBrans();
                                PoliceBransEslestir3 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                bransAdi = PoliceBransEslestir3.BransAdi;
                                bransKodu = PoliceBransEslestir3.BransKodu;
                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.QUICKSIGORTA, bransKodu.Value);
                                if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                {
                                    int otoOdeSayac = 0;
                                    foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                    {
                                        if (otoOdeSayac < 1 && bransKodu == itemOtoOdemeTipleri.BransKodu)
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
                                            tahsilat.BrutPrim = polBrutprimim.Value;
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
                                        tahsilat.BrutPrim = polBrutprimim.Value;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                                        tahsilat.PoliceId = odm.PoliceId;
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
                                        tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.KalanTaksitTutari = 0;
                                        tahsilat.PoliceNo = policemNo;
                                        tahsilat.ZeyilNo = ekNom.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilat.BrutPrim = polBrutprimim.Value;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
                                        tahsilat.PoliceId = odm.PoliceId;
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
