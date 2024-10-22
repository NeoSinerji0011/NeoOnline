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

    class SFSExcelSompo
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
                                          "Poliçe No",     //0
                                                 "Zeyl No",     //1
                                                 "Yenileme No",     //2
                                                 "Tanzim Tarihi",     //3
                                                 "Onay Tarihi",     //4
                                                 "Başlangıç Tarihi",     //5
                                                 "Bitiş Tarihi",     //6
                                                 "Zeyl Tipi",     //7
                                                 "Sigortalı Ünvanı",     //8
                                                 "Sig. Adresi",     //9
                                                 "Sig. İl",     //10
                                                 "Sig. Vergi Numarası",     //11
                                                 "Sig. Vergi Dairesi",     //12
                                                 "Sig. TC Kimlik No",     //13
                                                 "Müşteri Ünvanı",     //14
                                                 "Müş. Adresi",     //15
                                                 "Müş. İl",     //16
                                                 "Müş. Vergi Numarası",     //17
                                                 "Müş. Vergi Dairesi",     //18
                                                 "Müş. TC Kimlik No",     //19
                                                 "Riziko Adresi",     //20
                                                 "Ürün No",     //21
                                                 "Ürün Adı",     //22
                                                 "Plaka",     //23
                                                 "Brüt Prim",     //24
                                                 "Komisyon",     //25
                                                 "GDV",     //26
                                                 "GF",     //27
                                                 "THGF",     //28
                                                 "YSV",     //29
                                                 "Döviz Cinsi",     //30
                                                 "Döviz Kuru",     //31
                                                 "Peşin (C) GO Vadeli (I)",     //32
                                                 "Taksit Sayısı",     //33
                                                 "Taksit Tarihi",     //34
                                                 "Taksit Tutarı",     //35
                                                 "T Taksit Ödeme Miktarı",     //36
                                                 "T Taksit Ödeme Tarihi",     //37
                                                 "Eski Poliçe No",     //38
                                                 "Net Prim",     //39
                                                 "Kaynak No",     //40
                                                 "SYS Kullanıcı Adı",     //41
                                                 "ARAÇ MARKASI",     //42
                                                 "MARKA KODU",     //43
                                                 "MOTOR NO",     //44
                                                 "ŞASİ NO",     //45
                                                 "TİPİ",     //46
                                                 "İmal Yılı (Model)",     //47
                                                 "Kullanım Kodu",//48
                                                 "Sig. Doğum Tarihi",     //49
                                                 "Sig. Doğum Yeri",     //40
                                                 "Sanal POS?"      //51
                                        };


        public SFSExcelSompo(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            // TODO: Complete member initialization
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.excelFileName = fileName;
            this.birlikKodu = birlikKodu;
            this.tvmKodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }
        #region
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
            string psliVknNo = null;
            decimal? polKomisyon = null;
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

            // sheet correct. Start to get rows...
            for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
            {
                IRow row = sheet.GetRow(indx);

                // null rowlar icin
                if (row == null) continue;

                // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                if (row.FirstCellNum == 0 && row.GetCell(0).StringCellValue == "Brüt Prim") break;

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
                    #endregion
                    foreach (ICell cell in cels)
                    {
                        
                        if (cell.ColumnIndex == 0) pol.GenelBilgiler.PoliceNumarasi = cell.StringCellValue;
                        policemNo = pol.GenelBilgiler.PoliceNumarasi;
                        if (cell.ColumnIndex == 1) pol.GenelBilgiler.EkNo = Util.toInt(row.GetCell(1).StringCellValue);
                        ekNom = pol.GenelBilgiler.EkNo;
                        if (cell.ColumnIndex == 2) pol.GenelBilgiler.YenilemeNo = Util.toInt(cell.StringCellValue);
                        if (cell.ColumnIndex == 4) pol.GenelBilgiler.TanzimTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        if (cell.ColumnIndex == 5) pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;
                        if (cell.ColumnIndex == 6) pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        if (cell.ColumnIndex == 7) pol.GenelBilgiler.ZeyilAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 8) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 9) pol.GenelBilgiler.PoliceSigortali.Adres = cell.StringCellValue;
                        if (cell.ColumnIndex == 10) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 11) pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 13) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue;

                        if (cell.ColumnIndex == 14) pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                        if (cell.ColumnIndex == 15) pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.StringCellValue;
                        if (cell.ColumnIndex == 19) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 17) pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.StringCellValue;
                        psliKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        psliVknNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;


                        if (cell.ColumnIndex == 20) pol.GenelBilgiler.PoliceRizikoAdresi.Adres = cell.StringCellValue != null ? cell.StringCellValue : null;
                        if (cell.ColumnIndex == 21) tumUrunKodu = cell.StringCellValue;
                        if (cell.ColumnIndex == 23)
                        {
                            pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(2, cell.StringCellValue.Length - 2) : "";
                            pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(0, 2) : "";
                        }
                        if (cell.ColumnIndex == 24)
                        {
                            pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
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
                        if (cell.ColumnIndex == 25) {
                            pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.NumericCellValue.ToString());
                            polKomisyon = pol.GenelBilgiler.Komisyon;
                        }
                        if (cell.ColumnIndex == 26)
                        {
                            // Gider Vergisi
                            pol.GenelBilgiler.ToplamVergi = 0;
                            PoliceVergi gv = new PoliceVergi();
                            gv.VergiKodu = 2;
                            gv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += gv.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gv);
                        }
                        if (cell.ColumnIndex == 27)
                        {
                            // Garanti fonu
                            PoliceVergi gf = new PoliceVergi();
                            gf.VergiKodu = 3;
                            gf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += gf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gf);
                        }
                        if (cell.ColumnIndex == 28)
                        {
                            // THGF 
                            PoliceVergi thgf = new PoliceVergi();
                            thgf.VergiKodu = 1;
                            thgf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += thgf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(thgf);
                        }
                        if (cell.ColumnIndex == 29)
                        {
                            // YSV 
                            PoliceVergi ysv = new PoliceVergi();
                            ysv.VergiKodu = 4;
                            ysv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(ysv);
                        }
                        if (cell.ColumnIndex == 30)
                        {
                            pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                            if (pol.GenelBilgiler.ParaBirimi == "YTL")
                            {
                                pol.GenelBilgiler.ParaBirimi = "TL";
                            }
                        }
                        if (cell.ColumnIndex == 31)
                        {
                            if (!String.IsNullOrEmpty(cell.StringCellValue))
                            {
                                pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.StringCellValue.Replace(".", ","));
                            }
                        }

                        if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                        {
                            pol.GenelBilgiler.DovizliBrutPrim = Math.Round(Util.ToDecimal(row.GetCell(24).NumericCellValue.ToString()) / pol.GenelBilgiler.DovizKur.Value,2);
                            pol.GenelBilgiler.DovizliKomisyon = Math.Round(Util.ToDecimal(row.GetCell(25).NumericCellValue.ToString()) / pol.GenelBilgiler.DovizKur.Value,2);
                        }
                        // odeme plani
                        if (cell.ColumnIndex == 34 || cell.ColumnIndex == 35)
                        {
                            if (cell.ColumnIndex == 34)
                            {
                                // Odeme Plani - ilk taksit
                                taksitNo = 1;
                                odm.TaksitNo = taksitNo;
                                odm.VadeTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                            }
                            if (cell.ColumnIndex == 35)
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
                                        odm.DovizliTaksitTutari = pol.GenelBilgiler.BrutPrim.Value / pol.GenelBilgiler.DovizKur;
                                    }
                                    if (odm.VadeTarihi == null)
                                    {
                                        odm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                                    }
                                    if (pol.GenelBilgiler.BransKodu.Value == 1 || pol.GenelBilgiler.BransKodu.Value == 2)
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
                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.TURKNIPPONSIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                                                if(tahsilat.OdemTipi ==1)
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
                                            tahsilat.KimlikNo = tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = odm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                            //   tahsilat.OdemeBelgeNo = "111111";
                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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

                                    #endregion
                                }
                            }

                        }

                        if (cell.ColumnIndex == 39)
                        {
                            pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.NumericCellValue.ToString());
                            if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                            {
                                pol.GenelBilgiler.DovizliNetPrim = Math.Round(Util.ToDecimal(cell.NumericCellValue.ToString()) / pol.GenelBilgiler.DovizKur.Value, 2);
        }
                        
                        if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != null)
                        {
                            //pol.GenelBilgiler.BrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                            //pol.GenelBilgiler.NetPrim = Math.Round( pol.GenelBilgiler.NetPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                            //pol.GenelBilgiler.Komisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                            polBrutprimim = pol.GenelBilgiler.BrutPrim;
                        }
                        }
                        if (cell.ColumnIndex == 42) pol.GenelBilgiler.PoliceArac.MarkaAciklama = cell.StringCellValue;
                        if (cell.ColumnIndex == 43) pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue;
                        if (cell.ColumnIndex == 44) pol.GenelBilgiler.PoliceArac.MotorNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 45) pol.GenelBilgiler.PoliceArac.SasiNo = cell.StringCellValue;
                        if (cell.ColumnIndex == 46) pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = cell.StringCellValue;
                        if (cell.ColumnIndex == 47) pol.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToInt32(cell.StringCellValue) : 0;
                        if (cell.ColumnIndex == 49 && !String.IsNullOrEmpty(cell.StringCellValue)) pol.GenelBilgiler.PoliceSigortali.DogumTarihi = Convert.ToDateTime(cell.StringCellValue);
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


                if (row.FirstCellNum == 34)
                {
                    // Odeme Plani - diger taksitler
                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    taksitNo += 1;
                    odm.TaksitNo = taksitNo;
                    if (!String.IsNullOrEmpty(row.GetCell(34).StringCellValue))
                    {
                        odm.VadeTarihi = Util.toDate(row.GetCell(34).StringCellValue, Util.DateFormat1);
                    }
                    if (bransKod == 1 || bransKod == 2)
                    {
                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                    }
                    else
                    {
                        odm.OdemeTipi = OdemeTipleri.Havale;
                    }
                    odm.TaksitTutari = carpan * Util.ToDecimal(row.GetCell(35).NumericCellValue.ToString()); // iptal ise tutuar eksi deger olmali
                    if (odemePol.GenelBilgiler.DovizKur != 1 && odemePol.GenelBilgiler.DovizKur != 0 && odemePol.GenelBilgiler.DovizKur != null)
                    {
                        odm.TaksitTutari = Util.ToDecimal(row.GetCell(35).NumericCellValue.ToString()) * carpan;
                        odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(row.GetCell(35).NumericCellValue.ToString()) / odemePol.GenelBilgiler.DovizKur.Value * carpan, 2);
                    }
                    odemePol.GenelBilgiler.OdemeSekli = 2; // Vadeli - taksit varsa vadeli
                    
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
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();

                                    tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                    odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                    if(tahsilat.OdemTipi ==1)
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
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
                                tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
                                tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
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
