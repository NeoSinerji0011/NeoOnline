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
using NPOI.HSSF.Record.PivotTable;
using static iTextSharp.text.pdf.AcroFields;
using Neosinerji.BABOnlineTP.Business.groupama.service;
using System.Web.UI;
using NPOI.SS.Formula.Functions;
using Neosinerji.BABOnlineTP.Business.axa.tahsilat;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    class ExcelAcnTurk
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
                                          "POLICE_HAREKET_NO",//0
                                             "POLICE_NO",//1
                                            "YENILEME_NO",//2
                                            "SIRA_NO",//3
                                            "EKBELGE_NO",//4
                                            "EKBELGE_KOD",
                                            "EKBELGE_AD",//5
                                            "URUN_KOD",//7
                                            "URUN_VERSIYON",//8
                                            "MODELLEME_URUN_AD",//9
                                            "DURUM",//10
                                            "ACENTE_KOD",//11
                                            "ACENTE_AD",//12
                                            "BOLGE_KOD",//13
                                            "BOLGE_AD",//13
                                            "SATIS_SORUMLUSU_AD",//14
                                            "SATIS_KANALI_AD",//15
                                            "SIGORTALI_ID",//16
                                            "SIGORTALI_AD",//17
                                            "SIGORTALI_TC",//18
                                            "SIGORTALI_VK",//19
                                            "SIGORTALI_ULKE",//20
                                            "SIGORTALI_IL",//21
                                            "SIGORTA_ETTIREN_ID",//22
                                            "SIGORTA_ETTIREN_AD",//23
                                            "SIGORTA_ETTIREN_TC",//24
                                            "SIGORTA_ETTIREN_VK",//25
                                            "RISK_ADRES_IL",//26
                                            "RISK_ADRES_ILCE",//27
                                            "RISK_ADRES_MAHALLE",//28
                                            "POLICE_PRIM_ID",//29
                                            "KAYIT_IPTAL",//30
                                            "KAYIT_TARIH",//31
                                            "POLICE_TANZIM_TARIH",//
                                            "EKBELGE_TANZIM_TARIH",
                                            "POLICE_BASLAMA_TARIH",
                                            "POLICE_BITIS_TARIH",
                                            "EKBELGE_BASLAMA_TARIH",
                                            "EKBELGE_BITIS_TARIH",
                                            "ODEME_ARAC_AD",
                                            "ODEME_PLAN_AD",
                                            "ODEME_SEKLI",
                                            "PARA_BIRIM_KOD",
                                            "MODELLEME_URUN_AD1",
                                            "SIGORTALI_YAS",
                                            "BINA_KOASURANS_ORAN",
                                            "BINA_TENZIL_MUAF_ORAN",
                                            "MUHTEVIYAT_KOAS_ORAN",
                                            "MUHTEVIYAT_TENZIL_MUAF_ORAN",
                                            "KOAS_ORAN",//32
                                            "TENZ_MUAF_ORAN",//33
                                            "GEMI_INSA_TARIH",
                                            "TOPLAM_KOLTUK_SAYISI",//34
                                            "FK_GRUP_KISI_SAYISI",//35
                                            "GEMI_ADI",
                                            "INSAA_YIL",//36
                                            "KSK_345_FILO_KODU",//37
                                            "SAGLIK_IPTAL_NEDEN",
                                            "YENILEME_MI",//38
                                            "YUKLEME_TARIH",//39
                                            "BINA_INSAA_YIL",
                                            "DASK_POLICE_NO",//40
                                            "DASK_SERI_NO",//41
                                            "CEK_BORDRO_NO",
                                            "ISARET_NUMARASI",//42
                                            "LEHDAR",//43
                                            "ALT_KULLANIM_TARZI",
                                            "FAALIYET_TURU",//44
                                            "FILO_KASKO_TURU",//45
                                            "FILO_SORU",
                                            "FK_RISK_GRUBU",//46
                                            "GUMRUK_ICIN_MI",//47
                                            "IKAME_YARDIM",
                                            "KASKO_HASARSIZLIK",//48
                                            "KASKO_PROJE_ADI",//49
                                            "KEFALET_SURELI_MI",
                                            "KULLANIM_TARZI",//50
                                            "PLAKA_IL_KOD",//51
                                            "PLAKA_NO",
                                            "MARKA",//52
                                            "MODEL",//53
                                            "MODEL_YILI",
                                            "MOTOR_NO",//54
                                            "SASI_NO",//55
                                            "POLICE_TIP",
                                            "RIZIKO_CINS",//56
                                            "SEVKIYAT_BAS_ULKE",//57
                                            "SEVKIYAT_BITIS_YER",
                                            "SEYAHAT_SURE",//58
                                            "SURPRIM_VAR_MI",//59
                                            "TEKLIF_URETIM_KAYNAGI",//61
                                            "URUN_GERI_CAGIRMA_VAR",//60
                                            "VASITA_TURU",//62
                                            "YABANCI_SAGLIK_SURPRIM",//63
                                            "YNGN_RISK_GRUBU",//64
                                            "BANKA_SUBE_KODU",//65
                                            "BANKA_SUBE_ADI",//66
                                            "PLATFORM",//67
                                            "POLICE_TIPI",//68
                                            "SON_GUNCELLEYEN",//69
                                            "SON_GUNCELLEMETARIHI",//70
                                            "SON_GUNCELLEYEN_DEPARTMAN",//71
                                            "SON_GUNCELLEYEN_AD",//72
                                            "TEKLIFI_GIREN",//73
                                            "TEKLIF_TARIHI",//74
                                            "TEKLIFI_GIREN_DEPARTMAN",//75
                                            "TEKLIFI_GIREN_AD",//76
                                            "TEKLIF_REF_ID",//77
                                            "BEDEL",//78
                                            "BEDEL_TL",//79
                                            "NET_PRIM",//80
                                            "NET_PRIM_TL" ,//81
                                            "PRIM",//82
                                            "PRIM_TL",//83
                                            "KOMISYON",//84
                                            "KOMISYON_TL",//85
                                            "GF",//86
                                            "GF_TL",//87
                                            "THGF",//88
                                            "THGF_TL",//89
                                            "YSV",//90
                                            "YSV_TL",//91
                                            "BSMV",//92
                                            "BSMV_TL",//93
                                            "GH",//94
                                            "GH_TL" //95
                                                        
                                        };

        public ExcelAcnTurk(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
        public class TarihTaksitBilgileri
        {
            public DateTime Tarih { get; set; }
            public int TaksitSayisi { get; set; }
            public byte OdemeTipi { get; set; }
            public decimal TaksitTutari { get; set; }

        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();
            int taksitNo = 0;
            int carpan = 1;
            Police odemePol = null;
            string bransAdi = null;
            int? bransKodu = null;
            // get excel file...
            FileStream excelFile = null;
            var tempBaslangisTarihi = "";
            string sLiKimlikNo = null;
            string kayitiptal = null;
            string sEttirenKimlikNo = null;
            string sEttirenvergino = null;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            int? bransKod = null;
            decimal? polKomisyon = null;
            decimal? polNet = null;
            decimal? polBrutprimim = null;
            tumUrunAdi = null;
            tumUrunKodu = null;
            string psliKimlikNo = null;
            string psliVknNo = null;
            decimal? brutPrim = 0;
            sLiKimlikNo = null;
            sEttirenKimlikNo = null;
            int temyakalanandeger = 0;
            try
            {
                excelFile = new FileStream(excelFileName, FileMode.Open, FileAccess.Read);
            }
            catch (IOException ioe)
            {
                message = ioe.ToString();
                return null;
            }

            //wb = new HSSFWorkbook(excelFile);
            //ISheet sheet = wb.GetSheet("Table1");
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
            // check sheet correct... 
            int startRow = Util.checkSheetCorrect(sheet, columnNames);
            if (startRow == -1) // error
            {
                message = "Sheet format error ....";
                return null;
            }
            // sheet correct. Start to get rows...
            // Police pol = new Police();
            List<PoliceSatirlari> policeSatirList = new List<PoliceSatirlari>();
            PoliceSatirlari policeSatir = new PoliceSatirlari();

            try
            {
                for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
                {
                    IRow row = sheet.GetRow(indx);

                    // null rowlar icin
                    if (row == null) continue;

                    // excel dosyasi okumayi sonlandirmak icin.  Police bilgileri bitmis oluyor
                    if (row.FirstCellNum == 0 && row.GetCell(0).NumericCellValue == 0) break;

                    // Police genel bilgileri icin. Police genel bilgiler aliniyor.
                    if (row.FirstCellNum == 0)
                    {

                        Police pol = new Police();
                        //PoliceOdemePlani odm = new PoliceOdemePlani();
                        pol.GenelBilgiler.OdemeSekli = 1; // pesin
                        carpan = 1;
                        MusteriGenelBilgiler musGenel = new MusteriGenelBilgiler();

                        // tvm kodu
                        pol.GenelBilgiler.TVMKodu = tvmKodu;

                        // Birlik Kodu
                        pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ACNTURKSIGORTA;

                        List<ICell> cels = row.Cells;
                        var Tip = "";

                        foreach (ICell cell in cels)
                        {
                            //pol.GenelBilgiler.YenilemeNo = 0;

                            if (cell.ColumnIndex == 0)
                            {

                            }
                            if (cell.ColumnIndex == 1)
                            {
                                try
                                {

                                    pol.GenelBilgiler.PoliceNumarasi = cell.NumericCellValue.ToString();
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 2)
                            {
                                try
                                {
                                    pol.GenelBilgiler.YenilemeNo = Convert.ToInt32(cell.NumericCellValue);
                                }
                                catch (Exception ex)
                                {

                                }

                            }

                            if (cell.ColumnIndex == 4)
                            {
                                try
                                {
                                    pol.GenelBilgiler.EkNo = Convert.ToInt32(cell.NumericCellValue);
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            if (cell.ColumnIndex == 7)
                            {
                                try
                                {
                                    tumUrunKodu = cell.StringCellValue;
                                    PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                    PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                    pol.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                    pol.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                                }
                                catch (Exception ex)
                                {


                                }

                            }


                            if (cell.ColumnIndex == 18)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortali.AdiUnvan = cell.StringCellValue;

                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 19)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortali.KimlikNo = cell.StringCellValue;
                                    sEttirenKimlikNo = pol.GenelBilgiler.PoliceSigortali.KimlikNo;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 20)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = cell.StringCellValue;
                                    sEttirenvergino = pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 21)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortali.UlkeAdi = cell.StringCellValue;

                                }
                                catch (Exception ex)
                                {


                                }
                            }
                            if (cell.ColumnIndex == 22)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortali.IlAdi = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 24)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            if (cell.ColumnIndex == 25)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 26)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }

                            if (cell.ColumnIndex == 27)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceRizikoAdresi.Il = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 28)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceRizikoAdresi.Ilce = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 29)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceRizikoAdresi.Mahalle = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 31)
                            {
                                try
                                {
                                    kayitiptal = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            // if (cell.ColumnIndex == 32) pol.GenelBilgiler.pol = cell.StringCellValue;///kayıt tarihi bakılacak

                            if (cell.ColumnIndex == 33)
                            {
                                try
                                {
                                    pol.GenelBilgiler.TanzimTarihi = cell.DateCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 35)
                            {
                                try
                                {
                                    pol.GenelBilgiler.BaslangicTarihi = cell.DateCellValue;
                                    tempBaslangisTarihi = pol.GenelBilgiler.BaslangicTarihi.ToString().Split(' ')[0];

                                }
                                catch (Exception ex)
                                {


                                }



                            }
                            if (cell.ColumnIndex == 36)
                            {
                                try
                                {
                                    pol.GenelBilgiler.BitisTarihi = cell.DateCellValue;
                                }
                                catch (Exception ex)
                                {

                                }

                            }

                            ///taksit bilgileri gelecek

                            int taksitsayisi = 0;
                            if (cell.ColumnIndex == 40)
                            {
                                try
                                {
                                    //TarihTaksitBilgileri tarih=new TarihTaksitBilgileri();

                                    //DateTime tempdate = Convert.ToDateTime(tempBaslangisTarihi);
                                    var gelentaksit = cell.StringCellValue;
                                    var tempgelen = gelentaksit.Trim();
                                    temyakalanandeger = int.Parse(tempgelen.Substring(0, 2));


                                    List<ICell> celsa = row.Cells;
                                    //int i;
                                    //List<TarihTaksitBilgileri> tarih = new List<TarihTaksitBilgileri>();
                                    //PoliceOdemePlani odm = new PoliceOdemePlani();
                                    //for (i = 0; i < temyakalanandeger; i++)
                                    //{
                                    //    tarih = new List<TarihTaksitBilgileri>();

                                    //    //var date2 = tempdate.AddMonths(i);
                                    //    string tempvade = tempdate.AddMonths(i).ToString();

                                    //    int temptaks = Convert.ToInt32(i + 1);
                                    //    //odm.VadeTarihi = Convert.ToDateTime(tarih);

                                    //    tarih.Add(new TarihTaksitBilgileri { Tarih = Convert.ToDateTime(tempvade), TaksitSayisi = temptaks, OdemeTipi = OdemeTipleri.KrediKarti });

                                    //    foreach (var item in tarih)
                                    //    {
                                    //        //var temptarih = tarih[j];

                                    //        //odm.VadeTarihi = item.Tarih;
                                    //        //odm.TaksitNo = item.TaksitSayisi;
                                    //        // odm.OdemeTipi = item.OdemeTipi;

                                    //        pol.GenelBilgiler.PoliceOdemePlanis.Add(new PoliceOdemePlani { VadeTarihi = item.Tarih, TaksitNo = item.TaksitSayisi, OdemeTipi = item.OdemeTipi });
                                    //    }



                                    //    //foreach (var item in tarih)
                                    //    //{

                                    //    //}

                                    //    //odm.VadeTarihi =  
                                    //    //    odm.TaksitNo =
                                    //    //    odm.OdemeTipi =
                                    //    //  tarih.Add(Convert.ToInt32(temptaks));
                                    //    //tarih.Add(Convert.ToByte(OdemeTipleri.KrediKarti)); 


                                    //}




                                    ////foreach (var item in tarih)
                                    ////{
                                    ////    //var temptarih = tarih[j];

                                    ////    odm.VadeTarihi = item.Tarih;
                                    ////    odm.TaksitNo = item.TaksitSayisi;
                                    ////    odm.OdemeTipi = item.OdemeTipi;

                                    ////}

                                    //pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);



                                }
                                catch (Exception ex)
                                {


                                }





                            }
                            if (cell.ColumnIndex == 41)
                            {
                                try
                                {
                                    var gelendata = cell.StringCellValue;


                                }
                                catch (Exception ex)
                                {


                                }

                            }

                            if (cell.ColumnIndex == 42)
                            {
                                try
                                {
                                    pol.GenelBilgiler.ParaBirimi = cell.StringCellValue;
                                    if (pol.GenelBilgiler.ParaBirimi == "TRY")
                                    {
                                        pol.GenelBilgiler.ParaBirimi = "TL";
                                    }

                                }
                                catch (Exception ex)
                                {


                                }

                            }

                            //if (cell.ColumnIndex == 31) pol.GenelBilgiler.kay= Convert.ToByte(cell.StringCellValue);
                            if (cell.ColumnIndex == 52)
                            {
                                try
                                {
                                    var tempkoltuk = 0;
                                    if (cell.StringCellValue == "")
                                        pol.GenelBilgiler.PoliceArac.KoltukSayisi = tempkoltuk;
                                    else
                                    {
                                        tempkoltuk = Convert.ToByte(cell.StringCellValue);


                                        pol.GenelBilgiler.PoliceArac.KoltukSayisi = tempkoltuk;

                                    }

                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            //if (cell.ColumnIndex == 57)
                            //{
                            //    try
                            //    {
                            //        pol.GenelBilgiler.PoliceArac.KoltukSayisi = 2;
                            //    }
                            //    catch (Exception ex)
                            //    {


                            //    }

                            //}
                            if (cell.ColumnIndex == 58)

                            {
                                try
                                {
                                    byte h = 0;
                                    byte e = 1;
                                    var tempyeniis = cell.StringCellValue;
                                    if (tempyeniis == 'H'.ToString())
                                    {
                                        pol.GenelBilgiler.Yeni_is = h;
                                    }
                                    else
                                    {
                                        pol.GenelBilgiler.Yeni_is = e;
                                    }
                                }
                                catch (Exception ex)
                                {


                                }


                            }


                            if (cell.ColumnIndex == 66)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.KullanimSekli = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }


                            if (cell.ColumnIndex == 76)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.KullanimTarzi = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }

                            if (cell.ColumnIndex == 77)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.PlakaKodu = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 78)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.PlakaNo = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {

                                }

                            }
                            if (cell.ColumnIndex == 79)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.Marka = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 80)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.MarkaAciklama = cell.StringCellValue;

                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 81)
                            {
                                try
                                {

                                    if ((cell.StringCellValue) == "")
                                    {
                                        pol.GenelBilgiler.PoliceArac.Model = 0;
                                    }
                                    else
                                    {
                                        pol.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(cell.StringCellValue);
                                    }
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 82)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.MotorNo = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 83)
                            {
                                try
                                {
                                    pol.GenelBilgiler.PoliceArac.SasiNo = cell.StringCellValue;
                                }
                                catch (Exception ex)
                                {


                                }

                            }

                            if (cell.ColumnIndex == 111)
                            {
                                try
                                {
                                    pol.GenelBilgiler.NetPrim = Util.ToDecimal(cell.ToString());
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 113)
                            {
                                try
                                {
                                    PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                    // PoliceOdemePlani odm = new PoliceOdemePlani();
                                    pol.GenelBilgiler.BrutPrim = Util.ToDecimal(cell.ToString());
                                    brutPrim = Convert.ToDecimal(pol.GenelBilgiler.BrutPrim);
                                    DateTime tempdate = Convert.ToDateTime(tempBaslangisTarihi);
                                    if (brutPrim != 0 && temyakalanandeger != 0)
                                    {

                                        double tamKisim = Math.Floor(Convert.ToDouble(brutPrim));
                                        //var tempbrutprimkurus = brutPrim.ToString().Split(',');
                                        double tempbrut = Convert.ToDouble(brutPrim / temyakalanandeger);
                                        //var yenibrut = tempbrut.ToString();
                                        //var yybrut = yenibrut.Split(',');
                                        //double brutTamKisim =  Convert.ToDouble(yybrut[0]);
                                        //double kusuratliKisim =Convert.ToDouble(yybrut[1].ToString()); // Küsüratlı kısmı alma
                                        //if (yybrut[1]==kusuratliKisim.ToString())
                                        //{
                                        //    double kusuratSonuc =Convert.ToDouble( kusuratliKisim * temyakalanandeger);
                                        //}
                                        //else
                                        //{
                                        //    double temppkusuratSıfırEk = Convert.ToDouble("0" + kusuratliKisim);
                                        //    double kusuratSonuc = Convert.ToDouble(temppkusuratSıfırEk * temyakalanandeger);
                                        //}

                                        //double kesilmisSayi = Math.Truncate(kusuratliKisim * 10000) / 10000;
                                        //double kusuratliKisim2 = tempbrut % 1; // Küsüratlı kısmı alır ve sıfırı da dahil eder
                                        //double yuvarlanmisSayi = Convert.ToDouble(Math.Round(Convert.ToDouble(yybrut[1]), 4));
                                        int i;
                                        List<TarihTaksitBilgileri> tarih = new List<TarihTaksitBilgileri>();

                                        for (i = 0; i < temyakalanandeger; i++)
                                        {
                                            tarih = new List<TarihTaksitBilgileri>();

                                            //var date2 = tempdate.AddMonths(i);
                                            string tempvade = tempdate.AddMonths(i).ToString();

                                            int temptaks = Convert.ToInt32(i + 1);
                                            //odm.VadeTarihi = Convert.ToDateTime(tarih);

                                            tarih.Add(new TarihTaksitBilgileri { Tarih = Convert.ToDateTime(tempvade), TaksitTutari = Convert.ToDecimal(tempbrut), TaksitSayisi = temptaks, OdemeTipi = OdemeTipleri.KrediKarti });

                                            foreach (var item in tarih)
                                            {
                                                //var temptarih = tarih[j];

                                                //odm.VadeTarihi = item.Tarih;
                                                //odm.TaksitNo = item.TaksitSayisi;
                                                // odm.OdemeTipi = item.OdemeTipi;

                                                pol.GenelBilgiler.PoliceOdemePlanis.Add(new PoliceOdemePlani { VadeTarihi = item.Tarih, TaksitTutari = Convert.ToDecimal(tempbrut), TaksitNo = item.TaksitSayisi, OdemeTipi = item.OdemeTipi });

                                                if (pol.GenelBilgiler.PoliceOdemePlanis.Count > 0)
                                                {
                                                    #region Tahsilat işlemi

                                                    bransAdi = pol.GenelBilgiler.BransAdi;
                                                    bransKodu = pol.GenelBilgiler.BransKodu;
                                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.ACNTURKSIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                                                                //pol.GenelBilgiler.pol.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                                if (tahsilat.OdemTipi == 1)
                                                                {
                                                                    if (item.TaksitTutari > 0)
                                                                    {
                                                                        tahsilat.OdenenTutar = item.TaksitTutari;
                                                                    }
                                                                    else
                                                                    {
                                                                        tahsilat.OdenenTutar = 0;
                                                                    }

                                                                    tahsilat.KalanTaksitTutari = 0;
                                                                    tahsilat.OdemeBelgeNo = "111111****1111";
                                                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                                                }
                                                                else
                                                                {
                                                                    tahsilat.OdenenTutar = 0;
                                                                    tahsilat.KalanTaksitTutari = item.TaksitTutari;
                                                                }
                                                                if (item.Tarih != null)
                                                                {
                                                                    tahsilat.TaksitVadeTarihi = item.Tarih;
                                                                }
                                                                else
                                                                {
                                                                    tahsilat.TaksitVadeTarihi = (DateTime)pol.GenelBilgiler.BaslangicTarihi;
                                                                }
                                                                tahsilat.TaksitNo = item.TaksitSayisi;
                                                                tahsilat.OdemeBelgeTarihi = item.Tarih;
                                                                if (item.TaksitTutari > 0)
                                                                {
                                                                    tahsilat.TaksitTutari = item.TaksitTutari;
                                                                }
                                                                else
                                                                {
                                                                    tahsilat.TaksitTutari = 0;
                                                                }

                                                                tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                                                tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                                                if (sEttirenKimlikNo !=null)
                                                                {
                                                                    tahsilat.KimlikNo = sEttirenKimlikNo;
                                                                }
                                                                else
                                                                {
                                                                    tahsilat.KimlikNo = sEttirenvergino;
                                                                }
                                                                 
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
                                                            //      odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                            tahsilat.OtomatikTahsilatiKkMi = 1;
                                                            if (item.Tarih != null)
                                                            {
                                                                tahsilat.TaksitVadeTarihi = item.Tarih;
                                                            }
                                                            else
                                                            {
                                                                tahsilat.TaksitVadeTarihi = (DateTime)pol.GenelBilgiler.BaslangicTarihi;
                                                            }

                                                            tahsilat.TaksitNo = item.TaksitSayisi;
                                                            tahsilat.OdemeBelgeTarihi = item.Tarih;
                                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                                            if (item.TaksitTutari > 0)
                                                            {
                                                                tahsilat.TaksitTutari = item.TaksitTutari;
                                                            }
                                                            else
                                                            {
                                                                tahsilat.TaksitTutari = 0;
                                                            }
                                                            if (item.TaksitTutari > 0)
                                                            {
                                                                tahsilat.OdenenTutar = item.TaksitTutari;
                                                            }
                                                            else
                                                            {
                                                                tahsilat.OdenenTutar = 0;
                                                            }
                                                             
                                                            tahsilat.KalanTaksitTutari = 0;
                                                            tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                                            tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                                            if (sEttirenKimlikNo != null)
                                                            {
                                                                tahsilat.KimlikNo = sEttirenKimlikNo;
                                                            }
                                                            else
                                                            {
                                                                tahsilat.KimlikNo = sEttirenvergino;
                                                            }

                                                            tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                                            tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                                            tahsilat.KayitTarihi = DateTime.Today;
                                                            tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                                            //tahsilat.TahsilatId = odm.PoliceId;
                                                            if (tahsilat.TaksitTutari != 0)
                                                            {
                                                                pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                            }
                                                        }
                                                        //else
                                                        //{
                                                        //    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                        //    tahsilat.OdemTipi = OdemeTipleri.Havale;
                                                            
                                                        //    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                                        //    tahsilat.TaksitNo = odm.TaksitNo;
                                                        //    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                        //    //   tahsilat.OdemeBelgeNo = "111111";
                                                        //    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                        //    tahsilat.OdenenTutar = 0;
                                                        //    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                        //    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                                        //    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                                        //    tahsilat.KimlikNo = tahsilat.KimlikNo = !String.IsNullOrEmpty(psliKimlikNo) ? psliKimlikNo : psliVknNo;
                                                        //    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                                        //    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                                        //    tahsilat.KayitTarihi = DateTime.Today;
                                                        //    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                                        //    tahsilat.TahsilatId = odm.PoliceId;
                                                        //    if (tahsilat.TaksitTutari != 0)
                                                        //    {
                                                        //        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                        //    }
                                                        //}

                                                    }

                                                    #endregion
                                                }

                                            }






                                        }






                                        //foreach (var item in tarih)
                                        //{
                                        //    //var temptarih = tarih[j];

                                        //    odm.VadeTarihi = item.Tarih;
                                        //    odm.TaksitNo = item.TaksitSayisi;
                                        //    odm.OdemeTipi = item.OdemeTipi;

                                        //}

                                        // pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);

                                    }



                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 115)
                            {
                                try
                                {
                                    pol.GenelBilgiler.Komisyon = Util.ToDecimal(cell.ToString());
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            // if (cell.ColumnIndex == 117) pol.GenelBilgiler. = Util.ToDecimal(cell.ToString());
                            if (cell.ColumnIndex == 117)
                            {

                                try
                                {
                                    // Garanti fonu
                                    PoliceVergi gf = new PoliceVergi();
                                    gf.VergiKodu = 3;
                                    gf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                    pol.GenelBilgiler.ToplamVergi += gf.VergiTutari;
                                    pol.GenelBilgiler.PoliceVergis.Add(gf);
                                }
                                catch (Exception ex)
                                {


                                }
                            }
                            if (cell.ColumnIndex == 119)
                            {
                                try
                                {
                                    // THGF 
                                    PoliceVergi thgf = new PoliceVergi();
                                    thgf.VergiKodu = 1;
                                    thgf.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                    pol.GenelBilgiler.ToplamVergi += thgf.VergiTutari;
                                    pol.GenelBilgiler.PoliceVergis.Add(thgf);
                                }
                                catch (Exception ex)
                                {


                                }


                            }

                            if (cell.ColumnIndex == 121)
                            {
                                try
                                {
                                    // YSV 
                                    PoliceVergi ysv = new PoliceVergi();
                                    ysv.VergiKodu = 4;
                                    ysv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                    pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                                    pol.GenelBilgiler.PoliceVergis.Add(ysv);
                                }
                                catch (Exception ex)
                                {


                                }

                            }
                            if (cell.ColumnIndex == 123)
                            {
                                try
                                {
                                    // GV
                                    PoliceVergi ysv = new PoliceVergi();
                                    ysv.VergiKodu = 2;
                                    ysv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                                    pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                                    pol.GenelBilgiler.PoliceVergis.Add(ysv);
                                }
                                catch (Exception ex)
                                {


                                }

                            }

                            //if (cell.ColumnIndex == 125)
                            //{
                            //    // gh
                            //    try
                            //    {
                            //        PoliceVergi ysv = new PoliceVergi();
                            //        ysv.VergiKodu = 3;
                            //        ysv.VergiTutari = Util.ToDecimal(cell.NumericCellValue.ToString());
                            //        pol.GenelBilgiler.ToplamVergi += ysv.VergiTutari;
                            //        pol.GenelBilgiler.PoliceVergis.Add(ysv);
                            //    }
                            //    catch (Exception ex)
                            //    {


                            //    }
                            //}



                        }

                        //sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        //sEttirenKimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;

                        try
                        {
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
                            if (pol.GenelBilgiler.PoliceNumarasi == null)
                            {
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {


                        }

                        policeler.Add(pol);

                        odemePol = pol;
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

        public class PoliceSatirlari
        {
            public string POLICE_HAREKET_NO { get; set; }
            public string POLICE_NO { get; set; }
            public string YENILEME_NO { get; set; }
            public string SIRA_NO { get; set; }
            public string EKBELGE_NO { get; set; }
            public string EKBELGE_KOD { get; set; }
            public string EKBELGE_AD { get; set; }
            public string URUN_KOD { get; set; }
            public string URUN_VERSIYON { get; set; }
            public string MODELLEME_URUN_AD { get; set; }
            public string DURUM { get; set; }
            public string ACENTE_KOD { get; set; }
            public string ACENTE_AD { get; set; }
            public string BOLGE_KOD { get; set; }
            public string BOLGE_AD { get; set; }
            public string SATIS_SORUMLUSU_AD { get; set; }
            public string SATIS_KANALI_AD { get; set; }
            public string SIGORTALI_ID { get; set; }
            public string SIGORTALI_AD { get; set; }
            public string SIGORTALI_TC { get; set; }
            public string SIGORTALI_VK { get; set; }
            public string SIGORTALI_ULKE { get; set; }
            public string SIGORTALI_IL { get; set; }
            public string SIGORTA_ETTIREN_ID { get; set; }
            public string SIGORTA_ETTIREN_AD { get; set; }
            public string SIGORTA_ETTIREN_TC { get; set; }
            public string SIGORTA_ETTIREN_VK { get; set; }
            public string RISK_ADRES_IL { get; set; }
            public string RISK_ADRES_ILCE { get; set; }
            public string RISK_ADRES_MAHALLE { get; set; }
            public string POLICE_PRIM_ID { get; set; }
            public string KAYIT_IPTAL { get; set; }
            public string KAYIT_TARIH { get; set; }
            public string POLICE_TANZIM_TARIH { get; set; }
            public string EKBELGE_TANZIM_TARIH { get; set; }
            public string POLICE_BASLAMA_TARIH { get; set; }
            public string POLICE_BITIS_TARIH { get; set; }
            public string EKBELGE_BASLAMA_TARIH { get; set; }
            public string EKBELGE_BITIS_TARIH { get; set; }
            public string ODEME_ARAC_AD { get; set; }
            public string ODEME_PLAN_AD { get; set; }
            public string ODEME_SEKLI { get; set; }
            public string PARA_BIRIM_KOD { get; set; }
            public string MODELLEME_URUN_AD1 { get; set; }
            public string SIGORTALI_YAS { get; set; }
            public string BINA_KOASURANS_ORAN { get; set; }
            public string BINA_TENZIL_MUAF_ORAN { get; set; }
            public string MUHTEVIYAT_KOAS_ORAN { get; set; }
            public string MUHTEVIYAT_TENZIL_MUAF_ORAN { get; set; }
            public string KOAS_ORAN { get; set; }
            public string TENZ_MUAF_ORAN { get; set; }
            public string GEMI_INSA_TARIH { get; set; }
            public string TOPLAM_KOLTUK_SAYISI { get; set; }
            public string FK_GRUP_KISI_SAYISI { get; set; }
            public string GEMI_ADI { get; set; }
            public string INSAA_YIL { get; set; }
            public string KSK_345_FILO_KODU { get; set; }
            public string SAGLIK_IPTAL_NEDEN { get; set; }
            public string YENILEME_MI { get; set; }
            public string YUKLEME_TARIH { get; set; }
            public string BINA_INSAA_YIL { get; set; }
            public string DASK_POLICE_NO { get; set; }
            public string DASK_SERI_NO { get; set; }
            public string CEK_BORDRO_NO { get; set; }
            public string ISARET_NUMARASI { get; set; }
            public string LEHDAR { get; set; }
            public string ALT_KULLANIM_TARZI { get; set; }
            public string FAALIYET_TURU { get; set; }
            public string FILO_KASKO_TURU { get; set; }
            public string FILO_SORU { get; set; }
            public string FK_RISK_GRUBU { get; set; }
            public string GUMRUK_ICIN_MI { get; set; }
            public string IKAME_YARDIM { get; set; }
            public string KASKO_HASARSIZLIK { get; set; }
            public string KASKO_PROJE_ADI { get; set; }
            public string KEFALET_SURELI_MI { get; set; }
            public string KULLANIM_TARZI { get; set; }
            public string PLAKA_IL_KOD { get; set; }
            public string PLAKA_NO { get; set; }
            public string MARKA { get; set; }
            public string MODEL { get; set; }
            public string MODEL_YILI { get; set; }
            public string MOTOR_NO { get; set; }
            public string SASI_NO { get; set; }
            public string POLICE_TIP { get; set; }
            public string RIZIKO_CINS { get; set; }
            public string SEVKIYAT_BAS_ULKE { get; set; }
            public string SEVKIYAT_BITIS_YER { get; set; }
            public string SEYAHAT_SURE { get; set; }
            public string SURPRIM_VAR_MI { get; set; }
            public string TEKLIF_URETIM_KAYNAGI { get; set; }
            public string URUN_GERI_CAGIRMA_VAR { get; set; }
            public string VASITA_TURU { get; set; }
            public string YABANCI_SAGLIK_SURPRIM { get; set; }
            public string YNGN_RISK_GRUBU { get; set; }
            public string BANKA_SUBE_KODU { get; set; }
            public string BANKA_SUBE_ADI { get; set; }
            public string PLATFORM { get; set; }
            public string POLICE_TIPI { get; set; }
            public string SON_GUNCELLEYEN { get; set; }
            public string SON_GUNCELLEMETARIHI { get; set; }
            public string SON_GUNCELLEYEN_DEPARTMAN { get; set; }
            public string SON_GUNCELLEYEN_AD { get; set; }
            public string TEKLIFI_GIREN { get; set; }
            public string TEKLIF_TARIHI { get; set; }
            public string TEKLIFI_GIREN_DEPARTMAN { get; set; }
            public string TEKLIFI_GIREN_AD { get; set; }
            public string TEKLIF_REF_ID { get; set; }
            public string BEDEL { get; set; }
            public string BEDEL_TL { get; set; }
            public string NET_PRIM { get; set; }
            public string NET_PRIM_TL { get; set; }
            public string PRIM { get; set; }
            public string PRIM_TL { get; set; }
            public string KOMISYON { get; set; }
            public string KOMISYON_TL { get; set; }
            public string GF { get; set; }
            public string GF_TL { get; set; }
            public string THGF { get; set; }
            public string THGF_TL { get; set; }
            public string YSV { get; set; }
            public string YSV_TL { get; set; }
            public string BSMV { get; set; }
            public string BSMV_TL { get; set; }
            public string GH { get; set; }
            public string GH_TL { get; set; }

        }

    }
}
