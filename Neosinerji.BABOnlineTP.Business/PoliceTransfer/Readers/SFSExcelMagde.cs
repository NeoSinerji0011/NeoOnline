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

    class SFSExcelMagde
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

                                            "Poliçe No",                 //0 
                                            "Ürün No",                   //1
                                            "P Ürün Adı",                  //2
                                            "P Tanzim Tarihi",             //3 
                                            "P Baş.Tarih",               //4
                                            "P Onay Tarihi",             //5 
                                            "P Bit. Tarihi",             //6 
                                            "P Brüt Prim",               //7 
                                            "P Net Prim",                //8 
                                            "P Plaka",                   //9
                                            "Zeyil No",                  //10 
                                            "Yenileme No",               //11
                                            "P Şirket",                  //12  
                                            "İptal Tarihi",              //13  
                                            "P Ödeme Şekli",             //14  
                                            "P Zeyil Adedi",             //15  
                                            "P YSV",                     //16
                                            "P THGF",                    //17 
                                            "P Döviz Cinsi",             //18
                                            "P Döviz Kuru",              //19
                                            "P Eski Poliçe No",            //20
                                            "P GDV",                     //21 
                                            "P GF",                      //22 
                                            "P Komisyon",                //23
                                            "P Peşin (C) / Vadeli (I)",   //24
                                            "P Riziko Adresi",           //25
                                            "P Sanal POS?",              //26
                                            "P Si/rta Bedeli",           //27  
                                            "P Taksit Sayısı",             //28
                                            "P Taksit Tarihi",           //29
                                            "P Taksit Tutarı",           //30
                                            "T Taksit Ödeme Miktarı",    //31
                                            "Tarim Devlet Desteği",      //32  
                                            "Tarim İdari Masraf",        //33
                                            "Hasarsızlık İndirimi",      //34
                                            "T Taksit Ödeme Tarihi",     //35
                                            "U Müş. Adresi",             //36
                                            "U Müş. Cinsiyet",           //37
                                            "U Müş. Doğum Tarihi",       //38
                                            "U Müş. Doğum Yeri",         //39
                                            "U Müş. E-posta Adresi",     //40
                                            "U Müş. Telefon1",           //41
                                            "U Müş. Telefon2",           //42
                                            "U Müş. TC Kimlik No",       //43
                                            "U Müş. Vergi Numarası",     //44
                                            "U Müş. İl",                 //45
                                            "U Müş. İlçe",               //46
                                            "U Müşteri Adı",             //47
                                            "U Müşteri No",              //48
                                            "U Si/rtalı No",             //49
                                            "U Sig. Adresi",             //50
                                            "U Sig. Cinsiyet",           //51
                                            "U Sig. Doğum Tarihi",       //52
                                            "U Sig. Doğum Yeri",         //53
                                            "U Sig. E-posta Adresi",     //54
                                            "U Sig. TC Kimlik No",       //55
                                            "U Sig. Telefon1",           //56
                                            "U Sig. Telefon2",           //57
                                            "U Sig. Vergi Numarası",     //58
                                            "U Sig. Uyruğu",             //59
                                            "U Sig. İl",                 //60
                                            "U Sig. İlçe",               //61
                                            "Sigortalı Adı",             //62
                                            "daini murtehin adı",        //63
                                            "MARKA",                     //64
                                            "MOTOR NO",                  //65
                                            "ŞASİ NO",                   //66





                                                
                                        };


        public SFSExcelMagde(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            string sEttirenKimlikNo = null;
            string odemeTipi = null;
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
                string tempValue = "";
                IRow row = sheet.GetRow(indx);

                // null rowlar icin
                if (row == null) continue;

                // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                if (row.FirstCellNum == 0 && row.GetCell(0).StringCellValue == "P Brüt Prim") break;

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
                        if (cell.ColumnIndex == 1) tumUrunKodu = cell.StringCellValue;
                        if (cell.ColumnIndex == 2) tumUrunAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 3) pol.GenelBilgiler.TanzimTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        if (cell.ColumnIndex == 4) pol.GenelBilgiler.BaslangicTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                        polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;
                        if (cell.ColumnIndex == 6) pol.GenelBilgiler.BitisTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);

                        if (cell.ColumnIndex == 7)
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
                        if (cell.ColumnIndex == 8)
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
                        if (cell.ColumnIndex == 9)
                        {
                            pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(2, cell.StringCellValue.Length - 2) : "";
                            pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue != "" && cell.StringCellValue.Length >= 2 ? cell.StringCellValue.Substring(0, 2) : "";
                        }

                        if (cell.ColumnIndex == 10) pol.GenelBilgiler.EkNo = Util.toInt(row.GetCell(10).StringCellValue);
                        ekNom = pol.GenelBilgiler.EkNo;

                        if (cell.ColumnIndex == 11) pol.GenelBilgiler.YenilemeNo = Util.toInt(cell.StringCellValue);

                        if (cell.ColumnIndex == 16)
                        {
                            // YSV 
                            PoliceVergi ysv = new PoliceVergi();
                            ysv.VergiKodu = 4;
                            ysv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(ysv);
                        }

                        if (cell.ColumnIndex == 17)
                        {
                            // THGF 
                            PoliceVergi thgf = new PoliceVergi();
                            thgf.VergiKodu = 1;
                            thgf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += thgf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(thgf);
                        }

                        if (cell.ColumnIndex == 18) pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                        if (cell.ColumnIndex == 19)
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


                        if (cell.ColumnIndex == 21)
                        {
                            // Gider Vergisi
                            pol.GenelBilgiler.ToplamVergi = 0;
                            PoliceVergi gv = new PoliceVergi();
                            gv.VergiKodu = 2;
                            gv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += gv.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gv);
                        }



                        if (cell.ColumnIndex == 22)
                        {
                            // Garanti fonu
                            PoliceVergi gf = new PoliceVergi();
                            gf.VergiKodu = 3;
                            gf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            pol.GenelBilgiler.ToplamVergi += gf.VergiTutari;
                            pol.GenelBilgiler.PoliceVergis.Add(gf);
                        }

                        if (cell.ColumnIndex == 23)
                        {
                            pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.NumericCellValue.ToString());
                            polKomisyon = pol.GenelBilgiler.Komisyon;
                        }

                        if (cell.ColumnIndex == 25) pol.GenelBilgiler.PoliceRizikoAdresi.Adres = cell.StringCellValue != null ? cell.StringCellValue : null;

                        if (cell.ColumnIndex == 29 || cell.ColumnIndex == 30)
                        {
                            if (cell.ColumnIndex == 29)
                            {
                                // Odeme Plani - ilk taksit
                                taksitNo = 1;
                                odm.TaksitNo = taksitNo;
                                odm.VadeTarihi = Util.toDate(cell.StringCellValue, Util.DateFormat1);
                            }
                            if (cell.ColumnIndex == 30)
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
                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.MAGDEBURGERSIGORTA, pol.GenelBilgiler.BransKodu.Value);
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


                        if (cell.ColumnIndex == 36) pol.GenelBilgiler.PoliceSigortaEttiren.Adres = cell.StringCellValue;

                        if (cell.ColumnIndex == 37)
                        {
                            pol.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = cell.StringCellValue;

                            if (!string.IsNullOrEmpty(cell.StringCellValue))
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.TipKodu = 1;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceSigortaEttiren.TipKodu = 2;
                            }
                        }
                            
                            

                        if (cell.ColumnIndex == 38)
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

                        if (cell.ColumnIndex == 40)
                        {
                            pol.GenelBilgiler.PoliceSigortaEttiren.EMail = row.GetCell(40).StringCellValue;
                        }


                        if (cell.ColumnIndex == 41) pol.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = cell.StringCellValue != null ? cell.StringCellValue : null;


                        if (cell.ColumnIndex == 43)
                        {
                            pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = row.GetCell(43).StringCellValue;
                        }

                        if (cell.ColumnIndex == 44)
                        {
                            pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = row.GetCell(44).StringCellValue;
                            psliKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        }


                        //if (cell.ColumnIndex == 43) pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue;

                        //if (cell.ColumnIndex == 44) pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.StringCellValue;


                        //if (pol.GenelBilgiler.PoliceSigortali.KimlikNo == "0")
                        //{
                        //    pol.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                        //}
                        //if (pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "0")
                        //{
                        //    pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                        //}
                        //if (pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "0")
                        //{
                        //    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                        //}
                        //if (pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "0")
                        //{
                        //    pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                        //}
                        //psliKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        //psliVknNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;


                        if (cell.ColumnIndex == 45) pol.GenelBilgiler.PoliceSigortaEttiren.IlAdi = cell.StringCellValue;
                        if (cell.ColumnIndex == 46) pol.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = cell.StringCellValue;

                        if (cell.ColumnIndex == 47)
                        {
                            pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                            if (!string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo))
                            {
                                var temp = cell.StringCellValue.Trim().LastIndexOf(" ");
                                string tempAd = temp > 0 ? cell.StringCellValue.Substring(0, temp) : cell.StringCellValue;
                                string tempSoyad = temp > 0 ? cell.StringCellValue.Substring(temp) : "";
                                pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = tempAd;
                                pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = tempSoyad;
                            }
                        }


                        if (cell.ColumnIndex == 50) pol.GenelBilgiler.PoliceSigortali.Adres = cell.StringCellValue;

                        if (cell.ColumnIndex == 51)
                        {
                            pol.GenelBilgiler.PoliceSigortali.Cinsiyet = cell.StringCellValue;

                            if (!string.IsNullOrEmpty(cell.StringCellValue))
                            {
                                pol.GenelBilgiler.PoliceSigortali.TipKodu = 1;
                            }
                            else
                            {
                                pol.GenelBilgiler.PoliceSigortali.TipKodu = 2;
                            }

                        }

                        if (cell.ColumnIndex == 52 && !String.IsNullOrEmpty(cell.StringCellValue)) pol.GenelBilgiler.PoliceSigortali.DogumTarihi = Convert.ToDateTime(cell.StringCellValue);

                        if (cell.ColumnIndex == 54)
                        {
                            pol.GenelBilgiler.PoliceSigortali.EMail = row.GetCell(54).StringCellValue;
                        }


                        if (cell.ColumnIndex == 55)
                        {
                            pol.GenelBilgiler.PoliceSigortali.KimlikNo = row.GetCell(55).StringCellValue;
                        }

                        if (cell.ColumnIndex == 56)
                        {
                            pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = row.GetCell(56).StringCellValue;
                            sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        }



                        //if (cell.ColumnIndex == 55) pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue;


                        //if (cell.ColumnIndex == 56) pol.GenelBilgiler.PoliceSigortali.TelefonNo = cell.StringCellValue != null ? cell.StringCellValue : null;


                        if (cell.ColumnIndex == 58) pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.StringCellValue;


                        if (cell.ColumnIndex == 60) pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.StringCellValue;

                        if (cell.ColumnIndex == 61) pol.GenelBilgiler.PoliceSigortali.IlceAdi = cell.StringCellValue;


                        if (cell.ColumnIndex == 62)
                        {
                            pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;
                            
                            if (!string.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo))
                            {
                                var temp = cell.StringCellValue.Trim().LastIndexOf(" ");
                                string tempAd = temp > 0 ? cell.StringCellValue.Substring(0, temp) : cell.StringCellValue;
                                string tempSoyad = temp > 0 ? cell.StringCellValue.Substring(temp) : "";
                                pol.GenelBilgiler.PoliceSigortali.AdiUnvan = tempAd;
                                pol.GenelBilgiler.PoliceSigortali.SoyadiUnvan = tempSoyad;
                            }

                        }
                        if (cell.ColumnIndex == 64) pol.GenelBilgiler.PoliceArac.MarkaAciklama = cell.StringCellValue;
                        //if (cell.ColumnIndex == 43) pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue;

                        if (cell.ColumnIndex == 65) pol.GenelBilgiler.PoliceArac.MotorNo = cell.StringCellValue;

                        if (cell.ColumnIndex == 66) pol.GenelBilgiler.PoliceArac.SasiNo = cell.StringCellValue;


                        //if (cell.ColumnIndex == 30) pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = cell.StringCellValue;

                        //if (cell.ColumnIndex == 37) pol.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(cell.StringCellValue) ? Convert.ToInt32(cell.StringCellValue) : 0;

                        //if (cell.ColumnIndex == 7) pol.GenelBilgiler.ZeyilAdi = cell.StringCellValue;

                        //if (cell.ColumnIndex == 11) pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.StringCellValue;

                        // odeme plani



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


                        foreach (var item in pol.GenelBilgiler.PoliceTahsilats)
                        {
                            item.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : psliKimlikNo;
                        }
                    }

                    policeler.Add(pol);
                    odemePol = pol;

                }


                // Odeme planinin deiger taksitleri icin
                List<ICell> celsa = row.Cells;

                if (row.FirstCellNum == 29)
                {
                    // Odeme Plani - diger taksitler

                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    PoliceTahsilat tahsilats = new PoliceTahsilat();

                    taksitNo += 1;
                    odm.TaksitNo = taksitNo;
                    odm.VadeTarihi = Util.toDate(row.GetCell(29).StringCellValue, Util.DateFormat1);
                    if (row.GetCell(30) != null)
                    {
                        odm.TaksitTutari = Convert.ToDecimal(carpan * Util.ToDecimal(row.GetCell(30).NumericCellValue.ToString())); // iptal ise tutuar eksi deger olmali
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
                                        tahsilats.OdemeBelgeNo = "111111****1111";
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
                                tahsilats.OdemeBelgeNo = "111111****1111";
                                tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                tahsilats.KalanTaksitTutari = 0;
                                tahsilats.PoliceNo = policemNo;
                                tahsilats.ZeyilNo = ekNom.ToString();
                                tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : psliKimlikNo;
                                tahsilats.BrutPrim = polBrutprimim.Value;
                                tahsilats.PoliceId = odm.PoliceId;
                                tahsilats.KayitTarihi = DateTime.Today;
                                tahsilats.KaydiEkleyenKullaniciKodu = tvmKodu;
                                //tahsilats.TahsilatId = odm.PoliceId;
                                if (tahsilats.TaksitTutari != 0)
                                {
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
                                tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : psliKimlikNo;
                                tahsilats.BrutPrim = polBrutprimim.Value;
                                tahsilats.PoliceId = odm.PoliceId;
                                tahsilats.KayitTarihi = DateTime.Today;
                                tahsilats.KaydiEkleyenKullaniciKodu = tvmKodu;
                                //tahsilats.TahsilatId = odm.PoliceId;
                                if (tahsilats.TaksitTutari != 0)
                                {
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
                                tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : psliKimlikNo;
                                tahsilats.BrutPrim = polBrutprimim.Value;
                                tahsilats.PoliceId = odm.PoliceId;
                                tahsilats.KayitTarihi = DateTime.Today;
                                tahsilats.KaydiEkleyenKullaniciKodu = tvmKodu;
                                //tahsilats.TahsilatId = odm.PoliceId;
                                if (tahsilats.TaksitTutari != 0)
                                {
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


    }
}