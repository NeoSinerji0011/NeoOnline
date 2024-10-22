using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class SFSExcelTurkland
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
                                     "P Poliçe No",                       //0
                                     "P Zeyil No",                        //1 
                                     "P Baş.Tarih",                       //2 
                                     "P Bit. Tarihi",                     //3
                                     "P Tanzim Tarihi",                   //4
                                     "P Brüt Prim",                       //5
                                     "P Net Prim",                        //6 
                                     "P GDV",                             //7
                                     "P GF",                              //8
                                     "P THGF",                            //9
                                     "P YSV",                             //10
                                     "U Müşteri No",                      //11
                                     "U Müşteri Adı",                    //12
                                     "P Plaka",                           //13
                                     "P Döviz Cinsi",                     //14
                                     "U Sigortalı Adı",                   //15
                                     "P Taksit Tarihi",                   //16
                                     "P Taksit Tutarı",                   //17
                                     "P Ürün No",                 //18
                                     "P Yenileme No",                     //19
                                     "P Komisyon",                        //20
                                     "P Riziko Adresi",                   //21
                                     "SYS Kullanıcı Adı",                 //22
                                     "P Döviz Kuru",                      //23
                                //     " ",                                  //24
                                     "U Müş. Adresi",                     //24
                                     "U Müş. TC Kimlik No",               //25
                                     "U Müş. Vergi Numarası",             //26
                                     "U Müş. Vergi Dairesi",              //27
                                     "U Müş. Doğum Tarihi",               //28
                                     "U Müş. E-posta Adresi"             //29
                                                                             
                                        };

        public SFSExcelTurkland(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.excelFileName = fileName;
            this.birlikKodu = birlikKodu;
            this.tvmKodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }

        string tumUrunAdi;
        string tumUrunKodu;

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();
            // get excel file...
            FileStream excelFile = null;
            string policemNo = null;
            tumUrunAdi = null;
            tumUrunKodu = null;
            string sEttirenKimlikNo = null;
            string sLiKimlikNo = null;
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

            int recordNo = 0;
            string cariPolNo = null;
            string polNo = null;
            Police pol = new Police();
            for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
            {
                IRow row = sheet.GetRow(indx);
                if (polNo == null)
                {
                    recordNo = 1;
                    polNo = cariPolNo;
                }
                //string cariPolNo = null;
                if (row.Cells.Count > 3)
                {
                    cariPolNo = row.GetCell(0).NumericCellValue.ToString();
                    policemNo = cariPolNo;


                }
                else
                {
                    if (polNo == cariPolNo)
                    {
                        if (recordNo > 1) // odeme plani - diger taksitler aliniyor
                        {
                            // add odeme plani - taksitler
                            PoliceOdemePlani odm = new PoliceOdemePlani();
                            odm.TaksitNo = recordNo;
                            PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                            PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                            pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                            pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                            odm.VadeTarihi = row.GetCell(16).DateCellValue;

                            if (row.GetCell(17) != null)
                            {
                                odm.TaksitTutari = Util.ToDecimal(row.GetCell(17).NumericCellValue.ToString());
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odm.TaksitTutari = Math.Round(Util.ToDecimal(row.GetCell(17).NumericCellValue.ToString()) * pol.GenelBilgiler.DovizKur.Value , 2);
                                    odm.DovizliTaksitTutari = Util.ToDecimal(row.GetCell(17).NumericCellValue.ToString());
                                }
                            }
                            if (row.GetCell(0) != null)
                            {
                                odm.VadeTarihi = row.GetCell(0).DateCellValue;
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
                                #region Tahsilat işlemi
                                sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.TURKLANDSIGORTA, pol.GenelBilgiler.BransKodu.Value);
                                if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                {
                                    int otoOdeSayac = 0;
                                    foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                    {
                                        if (otoOdeSayac < 1 && pol.GenelBilgiler.BransKodu.Value == itemOtoOdemeTipleri.BransKodu)
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
                                            tahsilat.PoliceNo = policemNo;
                                            tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                            tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.Value;
                                            tahsilat.PoliceId = odm.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = tvmKodu;
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
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
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
                                        // tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
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
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }

                                #endregion
                                pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                pol.GenelBilgiler.OdemeSekli = 2;
                            }
                        }
                    }
                }



                if (polNo == cariPolNo)
                {

                    if (recordNo == 1)
                    {

                        pol.GenelBilgiler.PoliceNumarasi = cariPolNo;

                        transferCellDataToPolice(pol, row);

                        pol.GenelBilgiler.Durum = 0;

                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);


                        if (PoliceBransEslestir != null)
                        {
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

                            pol.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                            pol.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;

                        }

                        policeler.Add(pol);
                    }

                    recordNo += 1;

                }
                else
                {
                    recordNo = 1;
                    polNo = cariPolNo;
                    pol = new Police();

                    pol.GenelBilgiler.PoliceNumarasi = cariPolNo;

                    transferCellDataToPolice(pol, row);

                    pol.GenelBilgiler.Durum = 0;


                    PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                    PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);


                    if (PoliceBransEslestir != null)
                    {
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

                        pol.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                        pol.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;

                    }

                    policeler.Add(pol);

                    recordNo += 1;
                }
            }

            return policeler;

        }

        private void transferCellDataToPolice(Police pol, IRow row)
        {
            DateTime? polBasTarihi = null;
            int? ekNom = null;
            string sEttirenKimlikNo = null;
            string sLiKimlikNo = null;
            decimal? polBrutprimim = null;
            try
            {
                pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.TURKLANDSIGORTA;
                // tvm kodu
                pol.GenelBilgiler.TVMKodu = tvmKodu;
                if (row.GetCell(1) != null)
                {
                    pol.GenelBilgiler.EkNo = Util.toInt(row.GetCell(1).NumericCellValue.ToString());
                    ekNom = pol.GenelBilgiler.EkNo;

                }
                if (row.GetCell(2) != null)
                {
                    pol.GenelBilgiler.BaslangicTarihi = row.GetCell(2).DateCellValue;
                    polBasTarihi = pol.GenelBilgiler.BaslangicTarihi;

                }
                if (row.GetCell(3) != null)
                {
                    pol.GenelBilgiler.BitisTarihi = row.GetCell(3).DateCellValue;
                }
                if (row.GetCell(4) != null)
                {
                    pol.GenelBilgiler.TanzimTarihi = row.GetCell(4).DateCellValue;
                }
                if (row.GetCell(5) != null)
                {
                    pol.GenelBilgiler.BrutPrim = Util.ToDecimal(row.GetCell(5).NumericCellValue.ToString());
                    polBrutprimim = pol.GenelBilgiler.BrutPrim;

                }
                if (row.GetCell(6) != null)
                {
                    pol.GenelBilgiler.NetPrim = Util.ToDecimal(row.GetCell(6).NumericCellValue.ToString());
                }
                // Gider vergisi
                PoliceVergi gv = new PoliceVergi();
                gv.VergiKodu = 2;
                if (row.GetCell(7) != null)
                {
                    gv.VergiTutari = Util.ToDecimal(row.GetCell(7).NumericCellValue.ToString());
                }
                pol.GenelBilgiler.PoliceVergis.Add(gv);
                pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gv.VergiTutari;

                // Garanti fonu
                pol.GenelBilgiler.ToplamVergi = 0;

                PoliceVergi gf = new PoliceVergi();
                gf.VergiKodu = 3;
                if (row.GetCell(8) != null)
                {
                    gf.VergiTutari = Util.ToDecimal(row.GetCell(8).NumericCellValue.ToString());
                }
                pol.GenelBilgiler.PoliceVergis.Add(gf);
                pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gf.VergiTutari;

                //trafih hizmetleri vergisi
                PoliceVergi tghf = new PoliceVergi();
                tghf.VergiKodu = 1;
                if (row.GetCell(9) != null)
                {
                    tghf.VergiTutari = Util.ToDecimal(row.GetCell(9).NumericCellValue.ToString());
                }
                pol.GenelBilgiler.PoliceVergis.Add(tghf);
                pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + tghf.VergiTutari;

                // ysv
                PoliceVergi ysv = new PoliceVergi();

                ysv.VergiKodu = 4;
                if (row.GetCell(10) != null)
                {
                    ysv.VergiTutari = Util.ToDecimal(row.GetCell(10).NumericCellValue.ToString());
                }
                pol.GenelBilgiler.PoliceVergis.Add(ysv);
                pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + ysv.VergiTutari;

                if (row.GetCell(12) != null)
                {
                    pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = row.GetCell(12).StringCellValue;
                }
                if (row.GetCell(13) != null)
                {
                    pol.GenelBilgiler.PoliceArac.PlakaNo = row.GetCell(13).StringCellValue != "" && row.GetCell(13).StringCellValue.Length >= 2 ? row.GetCell(13).StringCellValue.Substring(2, row.GetCell(13).StringCellValue.Length - 2) : "";
                    pol.GenelBilgiler.PoliceArac.PlakaKodu = row.GetCell(13).StringCellValue != "" && row.GetCell(13).StringCellValue.Length >= 2 ? row.GetCell(13).StringCellValue.Substring(0, 2) : "";
                }
                if (row.GetCell(14) != null)
                {
                    pol.GenelBilgiler.ParaBirimi = row.GetCell(14).StringCellValue;
                }

                if (row.GetCell(15) != null)
                {
                    pol.GenelBilgiler.PoliceSigortali.AdiUnvan = row.GetCell(15).StringCellValue;
                }
                // PoliceOdemePlan - Ilk taksit 
                PoliceOdemePlani odm = new PoliceOdemePlani();
                odm.TaksitNo = 1;
                if (row.GetCell(17) != null)
                {
                    odm.TaksitTutari = Util.ToDecimal(row.GetCell(17).NumericCellValue.ToString());
                    if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                    {
                        odm.TaksitTutari = Math.Round(Util.ToDecimal(row.GetCell(17).NumericCellValue.ToString()) * pol.GenelBilgiler.DovizKur.Value , 2);
                        odm.DovizliTaksitTutari = Util.ToDecimal(row.GetCell(17).NumericCellValue.ToString());
                    }
                }
                if (row.GetCell(16) != null)
                {
                    odm.VadeTarihi = row.GetCell(16).DateCellValue;
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
                if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                {
                    pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                    pol.GenelBilgiler.OdemeSekli = 1;
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
                    if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                    {
                        pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                    }
                }
                if (row.GetCell(18) != null)
                {
                    tumUrunKodu = row.GetCell(18).StringCellValue;
                }
                if (row.GetCell(19) != null)
                {
                    pol.GenelBilgiler.YenilemeNo = Util.toInt(row.GetCell(19).NumericCellValue.ToString());
                }
                if (row.GetCell(20) != null)
                {
                    pol.GenelBilgiler.Komisyon = Util.ToDecimal(row.GetCell(20).NumericCellValue.ToString());
                }
                if (row.GetCell(21) != null)
                {
                    pol.GenelBilgiler.PoliceRizikoAdresi.Adres = row.GetCell(21).StringCellValue;
                }
                if (row.GetCell(23) != null)
                {
                    pol.GenelBilgiler.DovizKur = Convert.ToDecimal(row.GetCell(23).NumericCellValue.ToString());
                }
                if (row.GetCell(24) != null)
                {
                    pol.GenelBilgiler.PoliceSigortaEttiren.Adres = row.GetCell(24).StringCellValue;
                }
                if (row.GetCell(25) != null)
                {
                    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = row.GetCell(25).StringCellValue;
                    pol.GenelBilgiler.PoliceSigortali.KimlikNo = row.GetCell(25).StringCellValue;
                    sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                }
                if (row.GetCell(26) != null)
                {
                    pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = row.GetCell(26).StringCellValue;
                    pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = row.GetCell(26).StringCellValue;
                    sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                }
                if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                {
                    #region Tahsilat işlemi
                    sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                    sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.TURKLANDSIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                            tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
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

                    #endregion
                }

                if (row.GetCell(28) != null)
                {
                    pol.GenelBilgiler.PoliceSigortali.DogumTarihi = row.GetCell(28).DateCellValue;

                }
                if (row.GetCell(29) != null)
                {
                    pol.GenelBilgiler.PoliceSigortali.EMail = row.GetCell(29).StringCellValue;
                }
                //  tumUrunAdi = row.GetCell(31).StringCellValue;            
                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                {
                    pol.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(row.GetCell(5).NumericCellValue.ToString());
                    pol.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(row.GetCell(6).NumericCellValue.ToString());
                    pol.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(row.GetCell(20).NumericCellValue.ToString());
                }

            }
            catch (Exception ex)
            {
                this.message = ex.ToString();
                //  policeler = null;
            }

        }
        public string getMessage()
        {
            return this.message;
        }
    }
}
