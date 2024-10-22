using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class VizyoneksExcelBereket
    {
        HSSFWorkbook wb;
        ITVMService _TVMService;
        IPoliceTransferService _IPoliceTransferService;
        private string message = string.Empty;
        private string excelFileName;
        private string birlikKodu;
        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;

        private string[] columnNames =  {

                                            "ACENTA_NO",	//	0
                                            "ACENTA_POL_NO",	//	1
                                            "CF_ARAC_MARKA_ADI",	//	2
                                            "CF_ARAC_MARKA_KODU",	//	3
                                            "CF_ARAC_TIP",	//	4
                                            "BASLAMA_TARIH",	//	5
                                            "BITIS_TARIH",	//	6
                                            "CF_BRUT_PRIM_TOTAL",	//	7
                                            "CARI_POL_NO",	//	8
                                            "DOVIZ_CINS",	//	9
                                            "DOVIZ_KUR",	//	10
                                            "CF_IMAL_YILI",	//	11
                                            "CF_KULLANIM_SEKLI",	//	12
                                            "CF_MODEL_YILI",	//	13
                                            "CF_MOTOR_NO",	//	14
                                            "CF_MUSTERI_ADRES",	//	15
                                            "CF_ONAY_TARIHI",	//	16
                                            "CF_TOTAL_PRIM",	//	17
                                            "OP_ID",	//	18
                                            "CF_PLAKA",	//	19
                                            "CF_SANAL_POS",	//	20
                                            "CF_RIZIKO_ADRESI",	//	21
                                            "CF_SASI_NO",	//	22
                                            "CF_SIGORTALI_DOGUM_YERI",	//	23
                                            "CF_SIGORTALI_FAX",	//	24
                                            "CF_SIGORTALI_IL",	//	25
                                            "CF_SIGORTALI_TC_KIMLIK",	//	26
                                            "CF_SIGORTALI_TEL_NO",	//	27
                                            "CF_SIGORTALI_VERGI_NO",	//	28
                                            "CF_SIGORTA_ETTIREN_TC_KIMLIK",	//	29
                                            "CF_SIGORTA_ETTIREN_VERGI_NO",	//	30
                                            "SON_DURUM",	//	31
                                            "TANZIM_TARIH",	//	32
                                            "TARIFE_KOD",	//	33
                                            "TECDIT_NO",	//	34
                                            "CF_TESCIL_TARIH",	//	35
                                            "CF_TRAFIGE_CIKIS_TARIHI",	//	36
                                            "CF_TRAFIK_TESCIL_SERI",	//	37
                                            "CF_TRAFIK_TESCIL_SIRA",	//	38
                                            "CF_YOLCU_SAYISI",	//	39
                                            "ZEYL_SIRA_NO",	//	40
                                            "CF_MUSTERI_UNVAN",	//	41
                                            "VERGI_DAIRESI",	//	42
                                            "VERGI_NO",	//	43
                                            "CF_MUST_EMAIL",	//	44
                                            "CF_SIGORTALI_UNVAN",	//	45
                                            "CF_TUTAR",	//	46
                                            "CF_VADE_SAYISI",	//	47
                                            "VADE",	//	48
                                            "CF_SUM_GARANTI_FONU",	//	49
                                            "CF_SUM_GIDER_VERGISI",	//	50
                                            "CF_SUM_KOM_TUTARI",	//	51
                                            "CF_SUM_TRAFIK_GARANTI_FONU",	//	52
                                            "CF_SUM_YANGIN_SIGORTA_VERGISI",	//	53
                                        };

        public VizyoneksExcelBereket(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.excelFileName = fileName;
            this.birlikKodu = birlikKodu;
            this.tvmKodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }
        decimal dovizKuru = 1;
        string tumUrunAdi;
        string tumUrunKodu;
        string sLiKimlikNo;
        string sEttirenKimlikNo;
        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            // get excel file...
            FileStream excelFile = null;

            tumUrunAdi = null;
            tumUrunKodu = null;
            sLiKimlikNo = null;
            sEttirenKimlikNo = null;
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
            ISheet sheet = wb.GetSheet("Table1");

            // check sheet correct... 
            int startRow = Util.checkSheetCorrect(sheet, columnNames);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }
            // sheet correct. Start to get rows...
            Police pol = new Police();
            List<PoliceSatirlari> policeSatirList = new List<PoliceSatirlari>();
            PoliceSatirlari policeSatir = new PoliceSatirlari();
            DataFormatter formatter = new DataFormatter();
            //Excel deki saıtrlar listeye ekleniyor
            for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
            {
                IRow row = sheet.GetRow(indx);
                try
                {
                    policeSatir = new PoliceSatirlari()
                    {

                        ACENTA_NO = formatter.FormatCellValue(row.GetCell(0)),
                        ACENTA_POL_NO = formatter.FormatCellValue(row.GetCell(1)),
                        CF_ARAC_MARKA_ADI = formatter.FormatCellValue(row.GetCell(2)),
                        CF_ARAC_MARKA_KODU = formatter.FormatCellValue(row.GetCell(3)),
                        CF_ARAC_TIP = formatter.FormatCellValue(row.GetCell(4)),
                        BASLAMA_TARIH = formatter.FormatCellValue(row.GetCell(5)),
                        BITIS_TARIH = formatter.FormatCellValue(row.GetCell(6)),
                        CF_BRUT_PRIM_TOTAL = formatter.FormatCellValue(row.GetCell(7)),
                        CARI_POL_NO = formatter.FormatCellValue(row.GetCell(8)),
                        DOVIZ_CINS = formatter.FormatCellValue(row.GetCell(9)),
                        DOVIZ_KUR = formatter.FormatCellValue(row.GetCell(10)),
                        CF_IMAL_YILI = formatter.FormatCellValue(row.GetCell(11)),
                        CF_KULLANIM_SEKLI = formatter.FormatCellValue(row.GetCell(12)),
                        CF_MODEL_YILI = formatter.FormatCellValue(row.GetCell(13)),
                        CF_MOTOR_NO = formatter.FormatCellValue(row.GetCell(14)),
                        CF_MUSTERI_ADRES = formatter.FormatCellValue(row.GetCell(15)),
                        CF_ONAY_TARIHI = formatter.FormatCellValue(row.GetCell(16)),
                        CF_TOTAL_PRIM = formatter.FormatCellValue(row.GetCell(17)),
                        OP_ID = formatter.FormatCellValue(row.GetCell(18)),
                        CF_PLAKA = formatter.FormatCellValue(row.GetCell(19)),
                        CF_SANAL_POS = formatter.FormatCellValue(row.GetCell(20)),
                        CF_RIZIKO_ADRESI = formatter.FormatCellValue(row.GetCell(21)),
                        CF_SASI_NO = formatter.FormatCellValue(row.GetCell(22)),
                        CF_SIGORTALI_DOGUM_YERI = formatter.FormatCellValue(row.GetCell(23)),
                        CF_SIGORTALI_FAX = formatter.FormatCellValue(row.GetCell(24)),
                        CF_SIGORTALI_IL = formatter.FormatCellValue(row.GetCell(25)),
                        CF_SIGORTALI_TC_KIMLIK = formatter.FormatCellValue(row.GetCell(26)),
                        CF_SIGORTALI_TEL_NO = formatter.FormatCellValue(row.GetCell(27)),
                        CF_SIGORTALI_VERGI_NO = formatter.FormatCellValue(row.GetCell(28)),
                        CF_SIGORTA_ETTIREN_TC_KIMLIK = formatter.FormatCellValue(row.GetCell(29)),
                        CF_SIGORTA_ETTIREN_VERGI_NO = formatter.FormatCellValue(row.GetCell(30)),
                        SON_DURUM = formatter.FormatCellValue(row.GetCell(31)),
                        TANZIM_TARIH = formatter.FormatCellValue(row.GetCell(32)),
                        TARIFE_KOD = formatter.FormatCellValue(row.GetCell(33)),
                        TECDIT_NO = formatter.FormatCellValue(row.GetCell(34)),
                        CF_TESCIL_TARIH = formatter.FormatCellValue(row.GetCell(35)),
                        CF_TRAFIGE_CIKIS_TARIHI = formatter.FormatCellValue(row.GetCell(36)),
                        CF_TRAFIK_TESCIL_SERI = formatter.FormatCellValue(row.GetCell(37)),
                        CF_TRAFIK_TESCIL_SIRA = formatter.FormatCellValue(row.GetCell(38)),
                        CF_YOLCU_SAYISI = formatter.FormatCellValue(row.GetCell(39)),
                        ZEYL_SIRA_NO = formatter.FormatCellValue(row.GetCell(40)),
                        CF_MUSTERI_UNVAN = formatter.FormatCellValue(row.GetCell(41)),
                        VERGI_DAIRESI = formatter.FormatCellValue(row.GetCell(42)),
                        VERGI_NO = formatter.FormatCellValue(row.GetCell(43)),
                        CF_MUST_EMAIL = formatter.FormatCellValue(row.GetCell(44)),
                        CF_SIGORTALI_UNVAN = formatter.FormatCellValue(row.GetCell(45)),
                        CF_TUTAR = formatter.FormatCellValue(row.GetCell(46)),
                        CF_VADE_SAYISI = formatter.FormatCellValue(row.GetCell(47)),
                        VADE = formatter.FormatCellValue(row.GetCell(48)),
                        CF_SUM_GARANTI_FONU = formatter.FormatCellValue(row.GetCell(49)),
                        CF_SUM_GIDER_VERGISI = formatter.FormatCellValue(row.GetCell(50)),
                        CF_SUM_KOM_TUTARI = formatter.FormatCellValue(row.GetCell(51)),
                        CF_SUM_TRAFIK_GARANTI_FONU = formatter.FormatCellValue(row.GetCell(52)),
                        CF_SUM_YANGIN_SIGORTA_VERGISI = formatter.FormatCellValue(row.GetCell(53)),
                    };

                    policeSatirList.Add(policeSatir);
                }
                catch (Exception ex)
                {

                    String meesage = ex.Message;
                }

            }
            //Listedeki poliçeler poliçeno, ekno ve yenileme noya göre gruplanıyor 

            var GroupList = policeSatirList.GroupBy(ac => new
            {
                ac.CARI_POL_NO,
                ac.ZEYL_SIRA_NO,
                ac.TECDIT_NO
            }).Select(ac => new
            {
                PoliceNo = ac.Key.CARI_POL_NO,
                ZeylNo = ac.Key.ZEYL_SIRA_NO,
                YenilemeNo=ac.Key.TECDIT_NO,
                list = ac.OrderBy(s => s.VADE).ToList()
            }).ToList();

            //Gruplanan poliçeleri tespit etmek için döngü yapılıyor.
            foreach (var itemGrup in GroupList)
            {
                pol = new Police();

                for (int i = 0; i < itemGrup.list.Count; i++)
                {
                    string readerKulKodu = null;
                    var polDetay = itemGrup.list[i];
                    if (i == 0)
                    {
                        #region Genel Bilgiler

                        pol.GenelBilgiler.Durum = 0;
                        pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.BEREKET;
                        pol.GenelBilgiler.TVMKodu = tvmKodu;
                        pol.GenelBilgiler.PoliceNumarasi = itemGrup.PoliceNo;
                        pol.GenelBilgiler.EkNo = Convert.ToInt32(itemGrup.ZeylNo);
                        pol.GenelBilgiler.YenilemeNo = Convert.ToInt32(itemGrup.YenilemeNo);
                        pol.GenelBilgiler.BaslangicTarihi = DateTime.Parse(polDetay.BASLAMA_TARIH);
                        pol.GenelBilgiler.BitisTarihi = DateTime.Parse(polDetay.BITIS_TARIH);
                        pol.GenelBilgiler.TanzimTarihi = DateTime.Parse(polDetay.TANZIM_TARIH);
                        pol.GenelBilgiler.ParaBirimi = polDetay.DOVIZ_CINS;
                        //tumUrunKodu = polDetay.PTARIFE_KOD;
                        pol.GenelBilgiler.BrutPrim = Util.ToDecimal(polDetay.CF_BRUT_PRIM_TOTAL);
                        pol.GenelBilgiler.Komisyon = Util.ToDecimal(polDetay.CF_SUM_KOM_TUTARI);
                        pol.GenelBilgiler.NetPrim = Util.ToDecimal(polDetay.CF_TOTAL_PRIM);

                        if (!String.IsNullOrEmpty(polDetay.DOVIZ_KUR))
                        {
                            pol.GenelBilgiler.DovizKur = Util.ToDecimal(polDetay.DOVIZ_KUR.Replace(".", ","));
                        }
                        if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                        {
                            pol.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(polDetay.CF_BRUT_PRIM_TOTAL);
                            pol.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(polDetay.CF_SUM_KOM_TUTARI);
                            pol.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(polDetay.CF_TOTAL_PRIM);

                            pol.GenelBilgiler.BrutPrim = Math.Round(pol.GenelBilgiler.BrutPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                            pol.GenelBilgiler.NetPrim = Math.Round(pol.GenelBilgiler.NetPrim.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                            pol.GenelBilgiler.Komisyon = Math.Round(pol.GenelBilgiler.Komisyon.Value * pol.GenelBilgiler.DovizKur.Value, 2);
                        }
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

                        readerKulKodu = polDetay.OP_ID;
                        if (!String.IsNullOrEmpty(readerKulKodu))
                        {
                            var getReaderKodu = _IPoliceTransferService.GetPoliceReaderKullanicilari(readerKulKodu);
                            if (getReaderKodu != null)
                            {
                                pol.GenelBilgiler.TaliAcenteKodu = Convert.ToInt32(getReaderKodu.AltTvmKodu);
                            }
                        }
                        if (itemGrup.list.Count == 1)
                        {
                            pol.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                        }
                        else if (itemGrup.list.Count > 1)
                        {
                            pol.GenelBilgiler.OdemeSekli = OdemeSekilleri.Vadeli;
                        }

                        #endregion

                        #region Araç Bilgileri

                        if (!String.IsNullOrEmpty(polDetay.CF_PLAKA))
                        {
                            pol.GenelBilgiler.PoliceArac.PlakaNo = polDetay.CF_PLAKA.Length >= 2 ? polDetay.CF_PLAKA.Substring(2, polDetay.CF_PLAKA.Length - 2) : "";
                            pol.GenelBilgiler.PoliceArac.PlakaKodu = polDetay.CF_PLAKA.Length >= 2 ? polDetay.CF_PLAKA.Substring(0, 2) : "";
                        }
                        pol.GenelBilgiler.PoliceArac.SasiNo = polDetay.CF_SASI_NO;
                        //pol.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(polDetay.CF_TESCIL_TARIH, Util.DateFormat1);
                        if (polDetay.CF_TRAFIGE_CIKIS_TARIHI != "")
                        {
                            pol.GenelBilgiler.PoliceArac.TrafikCikisTarihi = DateTime.Parse(polDetay.CF_TRAFIGE_CIKIS_TARIHI);
                        }
                        else
                        {
                            pol.GenelBilgiler.PoliceArac.TrafikCikisTarihi = null;
                        }
                        pol.GenelBilgiler.PoliceArac.TescilSeriNo = polDetay.CF_TRAFIK_TESCIL_SERI + polDetay.CF_TRAFIK_TESCIL_SIRA;
                        pol.GenelBilgiler.PoliceArac.KoltukSayisi = Util.toInt(polDetay.CF_YOLCU_SAYISI);
                        pol.GenelBilgiler.PoliceArac.MarkaAciklama = polDetay.CF_ARAC_MARKA_ADI;
                        pol.GenelBilgiler.PoliceArac.Marka = polDetay.CF_ARAC_MARKA_KODU;
                        pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = polDetay.CF_ARAC_TIP;
                        pol.GenelBilgiler.PoliceArac.Model = Util.toInt(polDetay.CF_IMAL_YILI);
                        pol.GenelBilgiler.PoliceArac.MotorNo = polDetay.CF_MOTOR_NO;
                        pol.GenelBilgiler.PoliceRizikoAdresi.Adres = polDetay.CF_RIZIKO_ADRESI;

                        #endregion

                        #region Poliçe Sigortalı/Sigorta Ettiren

                        pol.GenelBilgiler.PoliceSigortaEttiren.Adres = polDetay.CF_MUSTERI_ADRES;
                        pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polDetay.CF_MUSTERI_UNVAN;
                        pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTA_ETTIREN_TC_KIMLIK) ? polDetay.CF_SIGORTA_ETTIREN_TC_KIMLIK : null;
                        pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTA_ETTIREN_VERGI_NO) ? polDetay.CF_SIGORTA_ETTIREN_VERGI_NO : null;
                        pol.GenelBilgiler.PoliceSigortali.KimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTALI_TC_KIMLIK) ? polDetay.CF_SIGORTALI_TC_KIMLIK : null;
                        pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTALI_VERGI_NO) ? polDetay.CF_SIGORTALI_VERGI_NO : null;
                        pol.GenelBilgiler.PoliceSigortali.AdiUnvan = polDetay.CF_SIGORTALI_UNVAN;
                        pol.GenelBilgiler.PoliceSigortali.DogumTarihi = TurkeyDateTime.Now;
                        pol.GenelBilgiler.PoliceSigortali.TelefonNo = polDetay.CF_SIGORTALI_TEL_NO;
                        pol.GenelBilgiler.PoliceSigortali.IlAdi = polDetay.CF_SIGORTALI_IL;
                        sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                        #endregion

                        #region Poliçe Vergiler
                        pol.GenelBilgiler.ToplamVergi = 0;

                        if (!String.IsNullOrEmpty(polDetay.CF_SUM_GARANTI_FONU))
                        {
                            PoliceVergi gf = new PoliceVergi();
                            gf.VergiKodu = TrafikVergiler.GarantiFonu;
                            gf.VergiTutari = Util.ToDecimal(polDetay.CF_SUM_GARANTI_FONU);
                            pol.GenelBilgiler.PoliceVergis.Add(gf);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gf.VergiTutari;
                        }

                        if (!String.IsNullOrEmpty(polDetay.CF_SUM_GIDER_VERGISI.ToString()))
                        {
                            PoliceVergi gv = new PoliceVergi();
                            gv.VergiKodu = TrafikVergiler.GiderVergisi;
                            gv.VergiTutari = Util.ToDecimal(polDetay.CF_SUM_GIDER_VERGISI);
                            pol.GenelBilgiler.PoliceVergis.Add(gv);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gv.VergiTutari;
                        }

                        if (!String.IsNullOrEmpty(polDetay.CF_SUM_TRAFIK_GARANTI_FONU))
                        {
                            //trafih hizmetleri vergisi
                            PoliceVergi tghf = new PoliceVergi();
                            tghf.VergiKodu = TrafikVergiler.THGFonu;
                            tghf.VergiTutari = Util.ToDecimal(polDetay.CF_SUM_TRAFIK_GARANTI_FONU);
                            pol.GenelBilgiler.PoliceVergis.Add(tghf);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + tghf.VergiTutari;
                        }
                        if (!String.IsNullOrEmpty(polDetay.CF_SUM_YANGIN_SIGORTA_VERGISI.ToString()))
                        {

                            PoliceVergi ysv = new PoliceVergi();
                            ysv.VergiKodu = 4;
                            ysv.VergiTutari = Util.ToDecimal(polDetay.CF_SUM_YANGIN_SIGORTA_VERGISI);
                            pol.GenelBilgiler.PoliceVergis.Add(ysv);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + ysv.VergiTutari;
                        }
                        #endregion

                    }

                    #region Poliçe Ödeme Planı ve Tahsilat

                    byte odemeTipi = 0;
                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    if (!String.IsNullOrEmpty(itemGrup.list[i].CF_TUTAR.ToString()))
                    {
                        odm = new PoliceOdemePlani();
                        odm.TaksitNo = (i + 1);
                        odm.TaksitTutari = Util.ToDecimal(itemGrup.list[i].CF_TUTAR);
                        if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                        {
                            odm.TaksitTutari = Math.Round(Util.ToDecimal(itemGrup.list[i].CF_TUTAR) * pol.GenelBilgiler.DovizKur.Value, 2);
                            odm.DovizliTaksitTutari = Util.ToDecimal(itemGrup.list[i].CF_TUTAR);
                        }
                        //odm.VadeTarihi = Util.toDate(itemGrup.list[i].VADE, Util.DateFormat1);
                        if ((itemGrup.list[i].VADE) != "")
                        {
                            odm.VadeTarihi = DateTime.Parse(itemGrup.list[i].VADE);
                        }
                        else
                        {
                            odm.VadeTarihi = null;
                        }
                        if (itemGrup.list[i].CF_SANAL_POS == "KRD" || itemGrup.list[i].CF_SANAL_POS == "ORD")
                        {
                            odemeTipi = OdemeTipleri.KrediKarti;
                            odm.OdemeTipi = odemeTipi;
                        }
                        else if (itemGrup.list[i].CF_SANAL_POS == "NKT" || itemGrup.list[i].CF_SANAL_POS == "MAK")
                        {
                            odemeTipi = OdemeTipleri.Nakit;
                            odm.OdemeTipi = odemeTipi;
                        }
                        else if (itemGrup.list[i].CF_SANAL_POS == "HVL")
                        {
                            odemeTipi = OdemeTipleri.Havale;
                            odm.OdemeTipi = odemeTipi;
                        }
                        else if (itemGrup.list[i].CF_SANAL_POS == "SNT" || itemGrup.list[i].CF_SANAL_POS == "CEK")
                        {
                            odemeTipi = OdemeTipleri.CekSenet;
                            odm.OdemeTipi = odemeTipi;
                        }
                        else if (!String.IsNullOrEmpty(itemGrup.list[i].CF_SANAL_POS))
                        {
                            odemeTipi = OdemeTipleri.Havale;
                            odm.OdemeTipi = odemeTipi;
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
                    }
                    if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                    {
                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.BEREKET, pol.GenelBilgiler.BransKodu.Value);
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
                                    tahsilat.YenilemeNo = pol.GenelBilgiler.YenilemeNo;
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
                            if (!String.IsNullOrEmpty(itemGrup.list[i].CF_TUTAR.ToString()))
                            {
                                PoliceTahsilat tahsilat = new PoliceTahsilat();
                                tahsilat.OdemTipi = odemeTipi;
                                tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                tahsilat.TaksitNo = odm.TaksitNo;
                                tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                //tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                //tahsilat.KalanTaksitTutari = 0;
                                tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                tahsilat.YenilemeNo = pol.GenelBilgiler.YenilemeNo;
                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                tahsilat.KayitTarihi = DateTime.Today;
                                tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                //tahsilat.TahsilatId = odm.PoliceId;
                                if (odemeTipi == OdemeTipleri.KrediKarti)
                                {
                                    tahsilat.OdemeBelgeNo = "111111****1111";
                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.KalanTaksitTutari = 0;
                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                }
                                else if (odemeTipi == OdemeTipleri.Havale || odemeTipi == OdemeTipleri.Nakit || odemeTipi == OdemeTipleri.CekSenet)
                                {
                                    tahsilat.OdenenTutar = 0;
                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                }
                                else
                                {
                                    tahsilat.OdenenTutar = 0;
                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                }
                                if (tahsilat.TaksitTutari != 0)
                                {
                                    pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                }
                            }
                        }
                    }
                    #endregion

                }
                if (String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo) && String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo) && String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo))
                {
                    string ras = "";
                    int sayi = 0;
                    Random rastgele = new Random();
                    for (int i = 0; i < 3; i++)
                    {
                        ras += rastgele.Next(0, 9);
                    }
                    sayi = Convert.ToInt32(ras);
                    var yeniTcVkn = 8000000001 + sayi;
                    var varMi = _TVMService.GetPoliceByVergiKimlikNo(yeniTcVkn.ToString(), tvmKodu);
                    if (!String.IsNullOrEmpty(varMi))
                    {
                        //Int64 Kimlik = 0;
                        //var kimlik = varMi;
                        //Kimlik = Convert.ToInt64(kimlik);
                        pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = (yeniTcVkn).ToString();
                        pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = (yeniTcVkn).ToString();
                        foreach (var item in pol.GenelBilgiler.PoliceTahsilats)
                        {
                            item.KimlikNo = (yeniTcVkn).ToString();
                        }
                    }
                    else
                    {
                        pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = yeniTcVkn.ToString();
                        pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = yeniTcVkn.ToString();
                        foreach (var item in pol.GenelBilgiler.PoliceTahsilats)
                        {
                            item.KimlikNo = yeniTcVkn.ToString();
                        }
                    }
                }

                policeler.Add(pol);

            }

            
            return policeler;
        }

        public string getMessage()
        {
            return this.message;
        }

        public class PoliceSatirlari
        {
            public string ACENTA_NO { get; set; }
            public string ACENTA_POL_NO { get; set; }
            public string CF_ARAC_MARKA_ADI { get; set; }
            public string CF_ARAC_MARKA_KODU { get; set; }
            public string CF_ARAC_TIP { get; set; }
            public string BASLAMA_TARIH { get; set; }
            public string BITIS_TARIH { get; set; }
            public string CF_BRUT_PRIM_TOTAL { get; set; }
            public string CARI_POL_NO { get; set; }
            public string DOVIZ_CINS { get; set; }
            public string DOVIZ_KUR { get; set; }
            public string CF_IMAL_YILI { get; set; }
            public string CF_KULLANIM_SEKLI { get; set; }
            public string CF_MODEL_YILI { get; set; }
            public string CF_MOTOR_NO { get; set; }
            public string CF_MUSTERI_ADRES { get; set; }
            public string CF_ONAY_TARIHI { get; set; }
            public string CF_TOTAL_PRIM { get; set; }
            public string OP_ID { get; set; }
            public string CF_PLAKA { get; set; }
            public string CF_SANAL_POS { get; set; }
            public string CF_RIZIKO_ADRESI { get; set; }
            public string CF_SASI_NO { get; set; }
            public string CF_SIGORTALI_DOGUM_YERI { get; set; }
            public string CF_SIGORTALI_FAX { get; set; }
            public string CF_SIGORTALI_IL { get; set; }
            public string CF_SIGORTALI_TC_KIMLIK { get; set; }
            public string CF_SIGORTALI_TEL_NO { get; set; }
            public string CF_SIGORTALI_VERGI_NO { get; set; }
            public string CF_SIGORTA_ETTIREN_TC_KIMLIK { get; set; }
            public string CF_SIGORTA_ETTIREN_VERGI_NO { get; set; }
            public string SON_DURUM { get; set; }
            public string TANZIM_TARIH { get; set; }
            public string TARIFE_KOD { get; set; }
            public string TECDIT_NO { get; set; }
            public string CF_TESCIL_TARIH { get; set; }
            public string CF_TRAFIGE_CIKIS_TARIHI { get; set; }
            public string CF_TRAFIK_TESCIL_SERI { get; set; }
            public string CF_TRAFIK_TESCIL_SIRA { get; set; }
            public string CF_YOLCU_SAYISI { get; set; }
            public string ZEYL_SIRA_NO { get; set; }
            public string CF_MUSTERI_UNVAN { get; set; }
            public string VERGI_DAIRESI { get; set; }
            public string VERGI_NO { get; set; }
            public string CF_MUST_EMAIL { get; set; }
            public string CF_SIGORTALI_UNVAN { get; set; }
            public string CF_TUTAR { get; set; }
            public string CF_VADE_SAYISI { get; set; }
            public string VADE { get; set; }
            public string CF_SUM_GARANTI_FONU { get; set; }
            public string CF_SUM_GIDER_VERGISI { get; set; }
            public string CF_SUM_KOM_TUTARI { get; set; }
            public string CF_SUM_TRAFIK_GARANTI_FONU { get; set; }
            public string CF_SUM_YANGIN_SIGORTA_VERGISI { get; set; }



        }
    }
}
