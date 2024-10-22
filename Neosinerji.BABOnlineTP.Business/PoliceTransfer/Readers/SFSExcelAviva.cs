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
    class SFSExcelAviva
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
                                            "CF_ARACIN_TIPI",                  //0
                                            "CF_ARAC_MARKA_ADI",              //1  
                                            "CF_ARAC_MARKA_KODU",          //2      
                                            "BASLAMA_TARIH",         //3
                                            "BITIS_TARIH",                //4
                                            "CARI_POL_NO",              //5
                                            "DOVIZ_CINS",                //6  
                                            "DOVIZ_KUR",                //7
                                            "ESKI_POL_NO",                 //8
                                            "CF_GARANTI_FONU",                  //9
                                            "CF_GIDER_VERGISI",            //10    
                                            "CF_IMAL_YILI",           //11
                                            "CF_IPTAL_TARIHI",               //12
                                            "CF_MOTOR_NO",          //13
                                            "CF_MUSTERI_ADRES",              //14
                                            "CF_ONAY_TARIHI",                //15
                                            "CF_ONCEKI_ACENTE_NO",           //16
                                            "CF_ONCEKI_POLICE_NO",           //17
                                            "CF_ONCEKI_SIRKET_KODU",              //18
                                            "OP_ID",                   //19
                                            "CF_PLAKA",              //20    
                                            "CF_SANAL_POS",             //21
                                            "CF_SASI_NO",             //22
                                            "CF_SIGORTALI_ADRESI",                      //23
                                            "CF_SIGORTALI_DOGUM_TARIHI",                   //24
                                            "CF_SIGORTALI_IL",           //25
                                            "CF_SIGORTALI_TC_KIMLIK",               //26
                                            "CF_SIGORTALI_TEL_NO",                 //27
                                            "CF_SIGORTALI_VERGI_DAIRESI",  //28
                                            "CF_SIGORTALI_VERGI_NO",           //29
                                            "CF_SIGORTA_ETTIREN_TCK",     //30
                                            "CF_SIGORTA_ETTIREN_VKN",    //31
                                            "TANZIM_TARIH",            //32
                                            "CF_TARIFE_ADI",     //33
                                            "TARIFE_KOD",        //34
                                            "TECDIT_NO",         //35
                                            "CF_TRAFIK_GARANTI_FONU",      //36
                                            "CF_YANGIN_SIGORTA_VERGISI",                  //37
                                            "ZEYL_SIRA_NO",               //38
                                            "CF_MUSTERI_UNVAN",                 //39
                                            "VERGI_DAIRESI",                  //40
                                            "VERGI_NO",            //41                                           
                                            "SIG_ETTIREN_NO",     //42
                                            "CF_SIGORTALI_UNVAN",      //43
                                            "CF_MUST_CEP_TEL" ,     //44
                                            "CF_MUST_SABIT_TEL",    //45
                                            "CF_TUTAR",     //46
                                            "VADE",      //47
                                            "CF_SUM_BRUT_PRIM",      //48
                                            "CF_SUM_KOM_TUTARI",      //49
                                            "CF_SUM_NET_PRIM"      //50
                                                        
                                        };

        public SFSExcelAviva(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            Police pol = new Police();
            List<PoliceSatirlari> policeSatirList = new List<PoliceSatirlari>();
            PoliceSatirlari policeSatir = new PoliceSatirlari();

            //Excel deki saıtrlar listeye ekleniyor
            for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
            {

                IRow row = sheet.GetRow(indx);
                policeSatir = new PoliceSatirlari()
                {
                    CF_ARACIN_TIPI = row.GetCell(0).StringCellValue,
                    CF_ARAC_MARKA_ADI = row.GetCell(1).StringCellValue,
                    CF_ARAC_MARKA_KODU = row.GetCell(2).StringCellValue,
                    BASLAMA_TARIH = row.GetCell(3).StringCellValue,
                    BITIS_TARIH = row.GetCell(4).StringCellValue,
                    CARI_POL_NO = row.GetCell(5).StringCellValue,
                    DOVIZ_CINS = row.GetCell(6).StringCellValue,
                    DOVIZ_KUR = row.GetCell(7).StringCellValue,
                    ESKI_POL_NO = row.GetCell(8).StringCellValue,
                    CF_GARANTI_FONU = row.GetCell(9).StringCellValue,
                    CF_GIDER_VERGISI = row.GetCell(10).StringCellValue,
                    CF_IMAL_YILI = row.GetCell(11).StringCellValue,
                    CF_IPTAL_TARIHI = row.GetCell(12).StringCellValue,
                    CF_MOTOR_NO = row.GetCell(13).StringCellValue,
                    CF_MUSTERI_ADRES = row.GetCell(14).StringCellValue,
                    CF_ONAY_TARIHI = row.GetCell(15).StringCellValue,
                    CF_ONCEKI_ACENTE_NO = row.GetCell(16).StringCellValue,
                    CF_ONCEKI_POLICE_NO = row.GetCell(17).StringCellValue,
                    CF_ONCEKI_SIRKET_KODU = row.GetCell(18).StringCellValue,
                    OP_ID = row.GetCell(19).StringCellValue,
                    CF_PLAKA = row.GetCell(20).StringCellValue,
                    CF_SANAL_POS = row.GetCell(21).StringCellValue,
                    CF_SASI_NO = row.GetCell(22).StringCellValue,
                    CF_SIGORTALI_ADRESI = row.GetCell(23).StringCellValue,
                    CF_SIGORTALI_DOGUM_TARIHI = row.GetCell(24).StringCellValue,
                    CF_SIGORTALI_IL = row.GetCell(25).StringCellValue,
                    CF_SIGORTALI_TC_KIMLIK = row.GetCell(26).StringCellValue,
                    CF_SIGORTALI_TEL_NO = row.GetCell(27).StringCellValue,
                    CF_SIGORTALI_VERGI_DAIRESI = row.GetCell(28).StringCellValue,
                    CF_SIGORTALI_VERGI_NO = row.GetCell(29).StringCellValue,
                    CF_SIGORTA_ETTIREN_TCK = row.GetCell(30).StringCellValue,
                    CF_SIGORTA_ETTIREN_VKN = row.GetCell(31).StringCellValue,
                    TANZIM_TARIH = row.GetCell(32).StringCellValue,
                    CF_TARIFE_ADI = row.GetCell(33).StringCellValue,
                    TARIFE_KOD = row.GetCell(34).StringCellValue,
                    TECDIT_NO = row.GetCell(35).StringCellValue,
                    CF_TRAFIK_GARANTI_FONU = row.GetCell(36).StringCellValue,
                    CF_YANGIN_SIGORTA_VERGISI = row.GetCell(37).StringCellValue,
                    ZEYL_SIRA_NO = row.GetCell(38).StringCellValue,
                    CF_MUSTERI_UNVAN = row.GetCell(39).StringCellValue,
                    VERGI_DAIRESI = row.GetCell(40).StringCellValue,
                    VERGI_NO = row.GetCell(41).StringCellValue,
                    SIG_ETTIREN_NO = row.GetCell(42).StringCellValue,
                    CF_SIGORTALI_UNVAN = row.GetCell(43).StringCellValue,
                    CF_MUST_CEP_TEL = row.GetCell(44).StringCellValue,
                    CF_MUST_SABIT_TEL = row.GetCell(45).StringCellValue,
                    CF_TUTAR = row.GetCell(46).StringCellValue,
                    VADE = row.GetCell(47).StringCellValue,
                    CF_SUM_BRUT_PRIM = row.GetCell(48).StringCellValue,
                    CF_SUM_KOM_TUTARI = row.GetCell(49).StringCellValue,
                    CF_SUM_NET_PRIM = row.GetCell(50).StringCellValue,
                };
                policeSatirList.Add(policeSatir);
            }
            //Listedeki poliçeler poliçeno, ekno ve yenileme noya göre gruplanıyor
            var GroupList = policeSatirList.GroupBy(ac => new
            {
                ac.CARI_POL_NO,
                ac.TECDIT_NO,
                ac.ZEYL_SIRA_NO,
            }).Select(ac => new
            {
                PoliceNo = ac.Key.CARI_POL_NO,
                YenilemeNo = ac.Key.TECDIT_NO,
                ZeylNo = ac.Key.ZEYL_SIRA_NO,
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
                        pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AVIVASIGORTA;
                        pol.GenelBilgiler.TVMKodu = tvmKodu;
                        pol.GenelBilgiler.PoliceNumarasi = itemGrup.PoliceNo;
                        pol.GenelBilgiler.EkNo = Convert.ToInt32(itemGrup.ZeylNo);
                        pol.GenelBilgiler.YenilemeNo = Convert.ToInt32(itemGrup.YenilemeNo);
                        pol.GenelBilgiler.BaslangicTarihi = Util.toDate(polDetay.BASLAMA_TARIH, Util.DateFormat3);
                        pol.GenelBilgiler.BitisTarihi = Util.toDate(polDetay.BITIS_TARIH, Util.DateFormat3);
                        pol.GenelBilgiler.TanzimTarihi = Util.toDate(polDetay.TANZIM_TARIH, Util.DateFormat3);
                        pol.GenelBilgiler.ParaBirimi = polDetay.DOVIZ_CINS;
                        tumUrunKodu = polDetay.TARIFE_KOD;
                        pol.GenelBilgiler.BrutPrim = Util.ToDecimal(polDetay.CF_SUM_BRUT_PRIM);
                        pol.GenelBilgiler.Komisyon = Util.ToDecimal(polDetay.CF_SUM_KOM_TUTARI);
                        pol.GenelBilgiler.NetPrim = Util.ToDecimal(polDetay.CF_SUM_NET_PRIM);

                        if (!String.IsNullOrEmpty(polDetay.DOVIZ_KUR))
                        {
                            pol.GenelBilgiler.DovizKur = Util.ToDecimal(polDetay.DOVIZ_KUR.Replace(".", ","));
                        }
                        if (pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                        {
                            pol.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(polDetay.CF_SUM_BRUT_PRIM);
                            pol.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(polDetay.CF_SUM_KOM_TUTARI);
                            pol.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(polDetay.CF_SUM_NET_PRIM);

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
                        pol.GenelBilgiler.PoliceArac.MarkaAciklama = polDetay.CF_ARAC_MARKA_ADI;
                        pol.GenelBilgiler.PoliceArac.Marka = polDetay.CF_ARAC_MARKA_KODU;
                        pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = polDetay.CF_ARACIN_TIPI;
                        pol.GenelBilgiler.PoliceArac.MotorNo = polDetay.CF_MOTOR_NO;

                        #endregion

                        #region Poliçe Sigortalı/Sigorta Ettiren

                        pol.GenelBilgiler.PoliceSigortaEttiren.Adres = polDetay.CF_MUSTERI_ADRES;
                        pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polDetay.CF_MUSTERI_UNVAN;
                        // pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = itemPolSatir.PSETT_VERGI_NO;
                        pol.GenelBilgiler.PoliceSigortali.KimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTALI_TC_KIMLIK) && (polDetay.CF_SIGORTALI_TC_KIMLIK.Length == 11)? polDetay.CF_SIGORTALI_TC_KIMLIK : null;
                        pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTALI_VERGI_NO) && polDetay.CF_SIGORTALI_VERGI_NO.Length == 10 ? polDetay.CF_SIGORTALI_VERGI_NO : null;

                        pol.GenelBilgiler.PoliceSigortali.AdiUnvan = polDetay.CF_SIGORTALI_UNVAN;
                        pol.GenelBilgiler.PoliceSigortali.IlKodu = polDetay.CF_SIGORTALI_IL;
                        pol.GenelBilgiler.PoliceSigortali.Adres = polDetay.CF_SIGORTALI_ADRESI;
                        if (!String.IsNullOrEmpty(polDetay.CF_SIGORTALI_DOGUM_TARIHI))
                        {
                            pol.GenelBilgiler.PoliceSigortali.DogumTarihi =Convert.ToDateTime(polDetay.CF_SIGORTALI_DOGUM_TARIHI);
                        }

                        pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTA_ETTIREN_TCK) && (polDetay.CF_SIGORTA_ETTIREN_TCK.Length == 11) ? polDetay.CF_SIGORTA_ETTIREN_TCK : null;
                        pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTA_ETTIREN_VKN) && (polDetay.CF_SIGORTA_ETTIREN_VKN.Length == 10) ? polDetay.CF_SIGORTA_ETTIREN_VKN : null;                                                 

                        //if(String.IsNullOrEmpty(polDetay.CF_SIGORTALI_TC_KIMLIK) && String.IsNullOrEmpty(polDetay.CF_SIGORTALI_VERGI_NO)) continue;
                        
                        if (!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) && !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo)) {

                            sLiKimlikNo = pol.GenelBilgiler.PoliceSigortali.KimlikNo;                           
                        }
                        else
                        {
                            sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        }

                        if (String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && (String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))) 
                        {
                            sEttirenKimlikNo = pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                            pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo =!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo: null;
                            pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo =!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo) ? pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo: null;
                        }
                        else if (!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && (!String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo))) {

                            sEttirenKimlikNo = pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                            pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                        }
                        else
                        {
                            sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        }

                        #endregion

                        #region Poliçe Vergiler
                        pol.GenelBilgiler.ToplamVergi = 0;

                        if (!String.IsNullOrEmpty(polDetay.CF_GARANTI_FONU))
                        {
                            PoliceVergi gf = new PoliceVergi();
                            gf.VergiKodu = TrafikVergiler.GarantiFonu;
                            gf.VergiTutari = Util.ToDecimal(polDetay.CF_GARANTI_FONU);
                            pol.GenelBilgiler.PoliceVergis.Add(gf);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gf.VergiTutari;
                        }

                        if (!String.IsNullOrEmpty(polDetay.CF_GIDER_VERGISI))
                        {
                            PoliceVergi gv = new PoliceVergi();
                            gv.VergiKodu = TrafikVergiler.GiderVergisi;
                            gv.VergiTutari = Util.ToDecimal(polDetay.CF_GIDER_VERGISI);
                            pol.GenelBilgiler.PoliceVergis.Add(gv);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gv.VergiTutari;
                        }

                        if (!String.IsNullOrEmpty(polDetay.CF_TRAFIK_GARANTI_FONU))
                        {
                            //trafih hizmetleri vergisi
                            PoliceVergi tghf = new PoliceVergi();
                            tghf.VergiKodu = TrafikVergiler.THGFonu;
                            tghf.VergiTutari = Util.ToDecimal(polDetay.CF_TRAFIK_GARANTI_FONU);
                            pol.GenelBilgiler.PoliceVergis.Add(tghf);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + tghf.VergiTutari;
                        }
                        if (!String.IsNullOrEmpty(polDetay.CF_YANGIN_SIGORTA_VERGISI))
                        {

                            PoliceVergi ysv = new PoliceVergi();
                            ysv.VergiKodu = 4;
                            ysv.VergiTutari = Util.ToDecimal(polDetay.CF_YANGIN_SIGORTA_VERGISI);
                            pol.GenelBilgiler.PoliceVergis.Add(ysv);
                            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + ysv.VergiTutari;
                        }
                        #endregion

                    }

                    #region Poliçe Ödeme Planı ve Tahsilat

                    byte odemeTipi = 0;
                    PoliceOdemePlani odm = new PoliceOdemePlani();
                    if (!String.IsNullOrEmpty(itemGrup.list[i].CF_TUTAR))
                    {
                        odm = new PoliceOdemePlani();
                        odm.TaksitNo = (i + 1);
                        odm.TaksitTutari = Util.ToDecimal(itemGrup.list[i].CF_TUTAR);
                        if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                        {
                            odm.TaksitTutari = Math.Round(Util.ToDecimal(itemGrup.list[i].CF_TUTAR) * pol.GenelBilgiler.DovizKur.Value, 2);
                            odm.DovizliTaksitTutari = Util.ToDecimal(itemGrup.list[i].CF_TUTAR);
                        }
                        odm.VadeTarihi = Util.toDate(itemGrup.list[i].VADE, Util.DateFormat3);

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
                        #region Taksitleri Eksik Poliçeler

                        #endregion
                    }
                    if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                    {
                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.AVIVASIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                            if (!String.IsNullOrEmpty(itemGrup.list[i].CF_TUTAR))
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
            public string CF_ARACIN_TIPI { get; set; }
            public string CF_ARAC_MARKA_ADI { get; set; }
            public string CF_ARAC_MARKA_KODU { get; set; }
            public string BASLAMA_TARIH { get; set; }
            public string BITIS_TARIH { get; set; }
            public string CARI_POL_NO { get; set; }
            public string DOVIZ_CINS { get; set; }
            public string DOVIZ_KUR { get; set; }
            public string ESKI_POL_NO { get; set; }
            public string CF_GARANTI_FONU { get; set; }
            public string CF_GIDER_VERGISI { get; set; }
            public string CF_IMAL_YILI { get; set; }
            public string CF_IPTAL_TARIHI { get; set; }
            public string CF_MOTOR_NO { get; set; }
            public string CF_MUSTERI_ADRES { get; set; }
            public string CF_ONAY_TARIHI { get; set; }
            public string CF_ONCEKI_ACENTE_NO { get; set; }
            public string CF_ONCEKI_POLICE_NO { get; set; }
            public string CF_ONCEKI_SIRKET_KODU { get; set; }
            public string OP_ID { get; set; }
            public string CF_PLAKA { get; set; }
            public string CF_SANAL_POS { get; set; }
            public string CF_SASI_NO { get; set; }
            public string CF_SIGORTALI_ADRESI { get; set; }
            public string CF_SIGORTALI_DOGUM_TARIHI { get; set; }
            public string CF_SIGORTALI_IL { get; set; }
            public string CF_SIGORTALI_TC_KIMLIK { get; set; }
            public string CF_SIGORTALI_TEL_NO { get; set; }
            public string CF_SIGORTALI_VERGI_DAIRESI { get; set; }
            public string CF_SIGORTALI_VERGI_NO { get; set; }
            public string TANZIM_TARIH { get; set; }
            public string CF_TARIFE_ADI { get; set; }
            public string TARIFE_KOD { get; set; }
            public string TECDIT_NO { get; set; }
            public string CF_TRAFIK_GARANTI_FONU { get; set; }
            public string CF_YANGIN_SIGORTA_VERGISI { get; set; }
            public string ZEYL_SIRA_NO { get; set; }
            public string CF_MUSTERI_UNVAN { get; set; }
            public string VERGI_DAIRESI { get; set; }
            public string VERGI_NO { get; set; }
            public string SIG_ETTIREN_NO { get; set; }
            public string CF_SIGORTALI_UNVAN { get; set; }
            public string CF_MUST_CEP_TEL { get; set; }
            public string CF_MUST_SABIT_TEL { get; set; }
            public string CF_TUTAR { get; set; }
            public string VADE { get; set; }
            public string CF_SUM_BRUT_PRIM { get; set; }
            public string CF_SUM_KOM_TUTARI { get; set; }
            public string CF_SUM_NET_PRIM { get; set; }
            public string CF_SIGORTA_ETTIREN_TCK { get; set; }
            public string CF_SIGORTA_ETTIREN_VKN { get; set; }
        }

    }
}
