using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer;
using Neosinerji.BABOnlineTP.Business.Tools;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Database.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    public class HepIyiExcel
    {
        HSSFWorkbook wb;
        ITVMService _TVMService;
        private string message = string.Empty;
        private string excelFileName;
        private string birlikKodu;
        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;

        private string[] columnNames;

        public HepIyiExcel(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
                string namesheet = wb1.NumberOfSheets > 0 ? wb1.GetSheetName(0) : "veri";
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
            columnNames = Utils.GetColumnNameList(birlikKodu);
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

                        if (cell.ColumnIndex == 2) pol.GenelBilgiler.EkNo = Util.toInt(cell.ToString());
                        ekNom = pol.GenelBilgiler.EkNo;

                        if (cell.ColumnIndex == 1) pol.GenelBilgiler.YenilemeNo = Util.toInt(cell.ToString());
                        if (cell.ColumnIndex == 8)
                        {
                            pol.GenelBilgiler.TanzimTarihi = Convert.ToDateTime(cell.ToString());
                            taksitNo = 1;
                            odm.TaksitNo = taksitNo;
                            odm.VadeTarihi = Convert.ToDateTime(cell.ToString());
                        }

                        if (cell.ColumnIndex == 6) pol.GenelBilgiler.BaslangicTarihi = Convert.ToDateTime(cell.ToString());
                        polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;


                        if (cell.ColumnIndex == 7) pol.GenelBilgiler.BitisTarihi = Convert.ToDateTime(cell.ToString());  // 7. satır mevcut değil                      
                        if (cell.ColumnIndex == 4) pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.ToString();
                        if (cell.ColumnIndex == 5) pol.GenelBilgiler.PoliceSigortali.Adres = cell.ToString();
                        //if (cell.ColumnIndex == 10) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.ToString();
                        if (cell.ColumnIndex == 22)
                        {
                            if (cell.ToString().Length == 10)
                            {
                                pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.ToString();
                            }
                        }

                        if (cell.ColumnIndex == 23) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.ToString();

                        if (cell.ColumnIndex == 21) pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.ToString();
                        if (cell.ColumnIndex == 5) pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.ToString() != null ? cell.ToString() : null;
                        // if (cell.ColumnIndex == 15) pol.GenelBilgiler.PoliceSigortaEttiren.IlAdi = cell.ToString() != null ? cell.ToString() : null;
                        if (cell.ColumnIndex == 22)
                        {
                            if (cell.ToString().Length == 10)
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.ToString();
                            }
                        }
                        if (cell.ColumnIndex == 23) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.ToString();
                        if (cell.ColumnIndex == 5) pol.GenelBilgiler.PoliceRizikoAdresi.Adres = cell.ToString() != null ? cell.ToString() : null;
                        if (cell.ColumnIndex == 10) tumUrunKodu = cell.ToString();
                        if (cell.ColumnIndex == 9) tumUrunAdi = cell.ToString();
                        sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                        if (cell.ColumnIndex == 20)
                        {
                            pol.GenelBilgiler.PoliceArac.PlakaNo = cell.ToString() != "" && cell.ToString().Length >= 2 ? cell.ToString().Substring(2, cell.ToString().Length - 2) : "";
                            pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.ToString() != "" && cell.ToString().Length >= 2 ? cell.ToString().Substring(0, 2) : "";
                        }
                        if (cell.ColumnIndex == 15)
                        {
                            pol.GenelBilgiler.BrutPrim = Utils.decimalDuzenle(cell.ToString());
                            polBrutprimim = pol.GenelBilgiler.BrutPrim;

                            if (pol.GenelBilgiler.BrutPrim < 0)
                            {
                                carpan = -1;

                            }
                        }
                        if (cell.ColumnIndex == 18)
                        {
                            pol.GenelBilgiler.Komisyon = Utils.decimalDuzenle(cell.ToString());
                            polKomisyon = pol.GenelBilgiler.Komisyon;
                        }
                        #region Vergiler
                        if (cell.ColumnIndex == 16)
                        {
                            // Gider Vergisi
                            pol.GenelBilgiler.ToplamVergi = 0;
                            PoliceVergi gv = new PoliceVergi();
                            gv.VergiKodu = 2;
                            gv.VergiTutari = Utils.decimalDuzenle(cell.ToString());
                            pol.GenelBilgiler.ToplamVergi += gv.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gv);
                        }
                        if (cell.ColumnIndex == 17)
                        {
                            // Garanti fonu
                            PoliceVergi gf = new PoliceVergi();
                            gf.VergiKodu = 3;
                            gf.VergiTutari = Utils.decimalDuzenle(cell.ToString());
                            pol.GenelBilgiler.ToplamVergi += gf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gf);
                        }
                        if (cell.ColumnIndex == 19)
                        {
                            // THGF 
                            PoliceVergi thgf = new PoliceVergi();
                            thgf.VergiKodu = 1;
                            thgf.VergiTutari = Utils.decimalDuzenle(cell.ToString());
                            pol.GenelBilgiler.ToplamVergi += thgf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(thgf);
                        }
                        //if (cell.ColumnIndex == 27)
                        //{
                        //    // YSV 
                        //    PoliceVergi ysv = new PoliceVergi();
                        //    ysv.VergiKodu = 4;
                        //    ysv.VergiTutari = Util.ToDecimal(cell.ToString());
                        //    pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                        //    pol.GenelBilgiler.PoliceVergis.Add(ysv);
                        //}
                        #endregion
                        if (cell.ColumnIndex == 13)
                        {
                            pol.GenelBilgiler.ParaBirimi = cell.ToString();
                            if (pol.GenelBilgiler.ParaBirimi == "YTL")
                            {
                                pol.GenelBilgiler.ParaBirimi = "TL";
                            }
                        }
                        //if (cell.ColumnIndex == 29)
                        //{
                        //    if (!String.IsNullOrEmpty(cell.ToString()))
                        //    {
                        //        pol.GenelBilgiler.DovizKur = Util.ToDecimal(cell.ToString().Replace(".", ","));
                        //    }
                        //}
                        if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                        {
                            pol.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value / pol.GenelBilgiler.DovizKur;
                            pol.GenelBilgiler.DovizliKomisyon = polKomisyon.Value / pol.GenelBilgiler.DovizKur;
                        }


                        if (cell.ColumnIndex == 14)
                        {
                           // pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.ToString());
                            pol.GenelBilgiler.NetPrim = Utils.decimalDuzenle(cell.ToString());

                            if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != null)
                            {
                                pol.GenelBilgiler.DovizliBrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value / pol.GenelBilgiler.DovizKur.Value, 2);
                                pol.GenelBilgiler.DovizliNetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value / pol.GenelBilgiler.DovizKur.Value, 2);
                                pol.GenelBilgiler.DovizliKomisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value / pol.GenelBilgiler.DovizKur.Value, 2);
                                polBrutprimim = pol.GenelBilgiler.BrutPrim;
                            }
                        }
                        if (cell.ColumnIndex == 25) pol.GenelBilgiler.PoliceArac.Marka = cell.ToString();
                        if (cell.ColumnIndex == 30) pol.GenelBilgiler.PoliceArac.SasiNo = cell.ToString();
                        if (cell.ColumnIndex == 29) pol.GenelBilgiler.PoliceArac.MotorNo = cell.ToString();
                        if (cell.ColumnIndex == 27) pol.GenelBilgiler.PoliceArac.Model = Util.toInt(cell.ToString());
                        if (cell.ColumnIndex == 26) pol.GenelBilgiler.PoliceArac.MarkaAciklama = cell.ToString();
                        if (cell.ColumnIndex == 28) pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = cell.ToString();
                        //if (cell.ColumnIndex == 47) pol.GenelBilgiler.PoliceArac.Marka = cell.ToString() + row.GetCell(46).ToString();

                    }
                    // odeme plani
                    if (pol.GenelBilgiler.BrutPrim != 0)
                    {
                        PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                        PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                        pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                        pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                        odm.TaksitTutari = carpan * pol.GenelBilgiler.BrutPrim; // iptal ise tutuar eksi deger olmali
                        if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                        {
                            odm.TaksitTutari = pol.GenelBilgiler.BrutPrim * carpan;
                            odm.DovizliTaksitTutari = Math.Round(pol.GenelBilgiler.BrutPrim.Value / pol.GenelBilgiler.DovizKur.Value * carpan, 2);
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

                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.HEPIYISIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                                    //tahsilat.OdemeBelgeNo = "111111";
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


            }

            return policeler;
        }

        public string getMessage()
        {
            return this.message;
        }
    }
}