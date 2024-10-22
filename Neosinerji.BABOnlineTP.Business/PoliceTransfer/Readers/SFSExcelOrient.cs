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

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    class SFSExcelOrient
    {
        HSSFWorkbook wb, wb1;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullanici;
        IPoliceTransferService _IPoliceTransferService;
        private string message = string.Empty;
        private string excelFileName;
        private string birlikKodu;

        private int tvmKodu;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;

        private string[] columnNames =  {
                                            "POL_ACENTA_NO",                  //0
                                            "POL_ACENTA_POL_NO",              //1  
                                            "CF_ARAC_MARKA_ADI",          //2      
                                            "CF_ARAC_MARKA_KODU",         //3
                                            "CF_ARAC_TIP",                //4
                                            "POL_BASLAMA_TARIH",              //5
                                            "POL_BITIS_TARIH",                //6  
                                            "POL_CARI_POL_NO",                //7
                                            "POL_DOVIZ_CINS",                 //8
                                            "POL_DOVIZ_KUR",                  //9
                                            "CF_GARANTI_FONU",            //10    
                                            "CF_GIDER_VERGISI",           //11
                                            "CF_IMAL_YILI",               //12
                                            "CF_KULLANIM_SEKLI",          //13
                                            "CF_MODEL_YILI",              //14
                                            "CF_MOTOR_NO",                //15
                                            "CF_MUSTERI_ADRES",           //16
                                            "CF_MUSTERI_UNVAN",           //17
                                            "PSETT_VERGI_DAIRESI",              //18
                                            "PSETT_VERGI_NO",                   //19
                                            "CF_MUST_EMAIL",              //20    
                                            "CF_ONAY_TARIHI",             //21
                                            "POL_OP_ID",                      //22
                                            "CF_PLAKA",                   //23
                                            "CF_RIZIKO_ADRESI",           //24
                                            "CF_SANAL_POS",               //25
                                            "CF_SASI_NO",                 //26
                                            "CF_SIGORTALI_DOGUM_YERI",  //27
                                            "CF_SIGORTALI_FAX",           //28
                                            "CF_SIGORTALI_IL",            //29
                                            "CF_SIGORTALI_TC_KIMLIK",     //30
                                            "CF_SIGORTALI_TEL_NO",        //31
                                            "CF_SIGORTALI_UNVAN",         //32
                                            "CF_SIGORTALI_VERGI_NO",      //33
                                            "CF_SIGORTA_ETTIREN_TC_KIMLIK",//34
                                            "CF_SIGORTA_ETTIREN_VERGI_NO", //35
                                            "POL_SON_DURUM",                  //36
                                            "POL_TANZIM_TARIH",               //37
                                            "POL_TARIFE_KOD",                 //38
                                            "POL_TECDIT_NO",                  //39
                                            "CF_TESCIL_TARIH",            //40
                                            "CF_TRAFIGE_CIKIS_TARIHI",    //41
                                            "CF_TRAFIK_GARANTI_FONU",     //42
                                            "CF_TRAFIK_TESCIL_SERI",      //43
                                            "CF_TRAFIK_TESCIL_SIRA",      //44
                                            "CF_TUTAR",                   //45
                                            "PTAKO_VADE",                 //46
                                            "CF_YANGIN_SIGORTA_VERGISI",  //47
                                            "CF_YOLCU_SAYISI",             //48
                                            "POL_ZEYL_SIRA_NO",               //49
                                            "CF_SUM_BRUT_PRIM",           //50
                                            "CF_SUM_KOM_TUTARI",          //51
                                            "CF_SUM_NET_PRIM"             //52
                                                        
                                        };

        public SFSExcelOrient(string fileName, int tvmKodu, string birlikKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
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
            FileStream excelFile = null, excelFile1 = null;

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
            //Excel deki saıtrlar listeye ekleniyor
            for (int indx = startRow; indx <= sheet.LastRowNum; indx++)
            {

                IRow row = sheet.GetRow(indx);
                policeSatir = new PoliceSatirlari()
                {
                    POL_ACENTA_NO = row.GetCell(0).StringCellValue,
                    POL_ACENTA_POL_NO = row.GetCell(1).StringCellValue,
                    CF_ARAC_MARKA_ADI = row.GetCell(2).StringCellValue,
                    CF_ARAC_MARKA_KODU = row.GetCell(3).StringCellValue,
                    CF_ARAC_TIP = row.GetCell(4).StringCellValue,
                    POL_BASLAMA_TARIH = row.GetCell(5).StringCellValue,
                    POL_BITIS_TARIH = row.GetCell(6).StringCellValue,
                    POL_CARI_POL_NO = row.GetCell(7).StringCellValue,
                    POL_DOVIZ_CINS = row.GetCell(8).StringCellValue,
                    POL_DOVIZ_KUR = row.GetCell(9).StringCellValue,
                    CF_GARANTI_FONU = row.GetCell(10).StringCellValue,
                    CF_GIDER_VERGISI = row.GetCell(11).StringCellValue,
                    CF_IMAL_YILI = row.GetCell(12).StringCellValue,
                    CF_KULLANIM_SEKLI = row.GetCell(13).StringCellValue,
                    CF_MODEL_YILI = row.GetCell(14).StringCellValue,
                    CF_MOTOR_NO = row.GetCell(15).StringCellValue,
                    CF_MUSTERI_ADRES = row.GetCell(16).StringCellValue,
                    CF_MUSTERI_UNVAN = row.GetCell(17).StringCellValue,
                    PSETT_VERGI_DAIRESI = row.GetCell(18).StringCellValue,
                    PSETT_VERGI_NO = row.GetCell(19).StringCellValue,
                    CF_MUST_EMAIL = row.GetCell(20).StringCellValue,
                    CF_ONAY_TARIHI = row.GetCell(21).StringCellValue,
                    POL_OP_ID = row.GetCell(22).StringCellValue,
                    CF_PLAKA = row.GetCell(23).StringCellValue,
                    CF_RIZIKO_ADRESI = row.GetCell(24).StringCellValue,
                    CF_SANAL_POS = row.GetCell(25).StringCellValue,
                    CF_SASI_NO = row.GetCell(26).StringCellValue,
                    CF_SIGORTALI_DOGUM_YERI = row.GetCell(27).StringCellValue,
                    CF_SIGORTALI_FAX = row.GetCell(28).StringCellValue,
                    CF_SIGORTALI_IL = row.GetCell(29).StringCellValue,
                    CF_SIGORTALI_TC_KIMLIK = row.GetCell(30).StringCellValue,
                    CF_SIGORTALI_TEL_NO = row.GetCell(31).StringCellValue,
                    CF_SIGORTALI_UNVAN = row.GetCell(32).StringCellValue,
                    CF_SIGORTALI_VERGI_NO = row.GetCell(33).StringCellValue,
                    CF_SIGORTA_ETTIREN_TC_KIMLIK = row.GetCell(34).StringCellValue,
                    CF_SIGORTA_ETTIREN_VERGI_NO = row.GetCell(35).StringCellValue,
                    POL_SON_DURUM = row.GetCell(36).StringCellValue,
                    POL_TANZIM_TARIH = row.GetCell(37).StringCellValue,
                    POL_TARIFE_KOD = row.GetCell(38).StringCellValue,
                    POL_TECDIT_NO = row.GetCell(39).StringCellValue,
                    CF_TESCIL_TARIH = row.GetCell(40).StringCellValue,
                    CF_TRAFIGE_CIKIS_TARIHI = row.GetCell(41).StringCellValue,
                    CF_TRAFIK_GARANTI_FONU = row.GetCell(42).StringCellValue,
                    CF_TRAFIK_TESCIL_SERI = row.GetCell(43).StringCellValue,
                    CF_TRAFIK_TESCIL_SIRA = row.GetCell(44).StringCellValue,
                    CF_TUTAR = row.GetCell(45).StringCellValue,
                    PTAKO_VADE = row.GetCell(46).StringCellValue,
                    CF_YANGIN_SIGORTA_VERGISI = row.GetCell(47).StringCellValue,
                    CF_YOLCU_SAYISI = row.GetCell(48).StringCellValue,
                    POL_ZEYL_SIRA_NO = row.GetCell(49).StringCellValue,
                    CF_SUM_BRUT_PRIM = row.GetCell(50).StringCellValue,
                    CF_SUM_KOM_TUTARI = row.GetCell(51).StringCellValue,
                    CF_SUM_NET_PRIM = row.GetCell(52).StringCellValue,
                };
                policeSatirList.Add(policeSatir);
            }
            //Listedeki poliçeler poliçeno, ekno ve yenileme noya göre gruplanıyor
            var GroupList = policeSatirList.GroupBy(ac => new
            {
                ac.POL_CARI_POL_NO,
                ac.POL_ZEYL_SIRA_NO,
                ac.POL_TECDIT_NO
            }).Select(ac => new
            {
                PoliceNo = ac.Key.POL_CARI_POL_NO,
                ZeylNo = ac.Key.POL_ZEYL_SIRA_NO,
                YenilemeNo = ac.Key.POL_TECDIT_NO,
                list = ac.OrderBy(s => s.PTAKO_VADE).ToList()
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
                        pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ORIENTSIGORTA;
                        pol.GenelBilgiler.TVMKodu = tvmKodu;
                        pol.GenelBilgiler.PoliceNumarasi = itemGrup.PoliceNo;
                        pol.GenelBilgiler.EkNo = Convert.ToInt32(itemGrup.ZeylNo);
                        pol.GenelBilgiler.YenilemeNo = Convert.ToInt32(itemGrup.YenilemeNo);
                        pol.GenelBilgiler.BaslangicTarihi = Util.toDate(polDetay.POL_BASLAMA_TARIH, Util.DateFormat1);
                        pol.GenelBilgiler.BitisTarihi = Util.toDate(polDetay.POL_BITIS_TARIH, Util.DateFormat1);
                        pol.GenelBilgiler.TanzimTarihi = Util.toDate(polDetay.POL_TANZIM_TARIH, Util.DateFormat1);
                        pol.GenelBilgiler.ParaBirimi = polDetay.POL_DOVIZ_CINS;
                        tumUrunKodu = polDetay.POL_TARIFE_KOD;
                        pol.GenelBilgiler.BrutPrim = Util.ToDecimal(polDetay.CF_SUM_BRUT_PRIM);
                        pol.GenelBilgiler.Komisyon = Util.ToDecimal(polDetay.CF_SUM_KOM_TUTARI);
                        pol.GenelBilgiler.NetPrim = Util.ToDecimal(polDetay.CF_SUM_NET_PRIM);

                        if (!String.IsNullOrEmpty(polDetay.POL_DOVIZ_KUR))
                        {
                            pol.GenelBilgiler.DovizKur = Util.ToDecimal(polDetay.POL_DOVIZ_KUR.Replace(".", ","));
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

                        readerKulKodu = polDetay.POL_OP_ID;
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
                        pol.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(polDetay.CF_TESCIL_TARIH, Util.DateFormat1);
                        pol.GenelBilgiler.PoliceArac.TrafikCikisTarihi = Util.toDate(polDetay.CF_TRAFIGE_CIKIS_TARIHI, Util.DateFormat1);
                        pol.GenelBilgiler.PoliceArac.TescilSeriKod = polDetay.CF_TRAFIK_TESCIL_SERI;
                        pol.GenelBilgiler.PoliceArac.TescilSeriNo = polDetay.CF_TRAFIK_TESCIL_SIRA;
                        pol.GenelBilgiler.PoliceArac.KoltukSayisi = Util.toInt(polDetay.CF_YOLCU_SAYISI);
                        pol.GenelBilgiler.PoliceArac.MarkaAciklama = polDetay.CF_ARAC_MARKA_ADI;
                        pol.GenelBilgiler.PoliceArac.Marka = polDetay.CF_ARAC_MARKA_KODU;
                        pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = polDetay.CF_ARAC_TIP;
                        pol.GenelBilgiler.PoliceArac.Model = Util.toInt(polDetay.CF_MODEL_YILI);
                        pol.GenelBilgiler.PoliceArac.MotorNo = polDetay.CF_MOTOR_NO;
                        pol.GenelBilgiler.PoliceRizikoAdresi.Adres = polDetay.CF_RIZIKO_ADRESI;

                        #endregion

                        #region Poliçe Sigortalı/Sigorta Ettiren

                        pol.GenelBilgiler.PoliceSigortaEttiren.Adres = polDetay.CF_MUSTERI_ADRES;
                        pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polDetay.CF_MUSTERI_UNVAN;
                        // pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = itemPolSatir.PSETT_VERGI_NO;
                        pol.GenelBilgiler.PoliceSigortali.KimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTALI_TC_KIMLIK) ? polDetay.CF_SIGORTALI_TC_KIMLIK : null;
                        pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTALI_VERGI_NO) ? polDetay.CF_SIGORTALI_VERGI_NO : null;
                        pol.GenelBilgiler.PoliceSigortali.AdiUnvan = polDetay.CF_SIGORTALI_UNVAN;
                        pol.GenelBilgiler.PoliceSigortali.IlKodu = polDetay.CF_SIGORTALI_IL;
                        pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTA_ETTIREN_TC_KIMLIK) ? polDetay.CF_SIGORTA_ETTIREN_TC_KIMLIK : null;
                        pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = !String.IsNullOrEmpty(polDetay.CF_SIGORTA_ETTIREN_VERGI_NO) ? polDetay.CF_SIGORTA_ETTIREN_VERGI_NO : null;
                        sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

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

                    var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, pol.GenelBilgiler);

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
                        odm.VadeTarihi = Util.toDate(itemGrup.list[i].PTAKO_VADE, Util.DateFormat1);
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
                    }

                    if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                    {
                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmKodu, SigortaSirketiBirlikKodlari.ORIENTSIGORTA, pol.GenelBilgiler.BransKodu.Value);


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
                                    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
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
                                    //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
            public string POL_ACENTA_NO { get; set; }
            public string POL_ACENTA_POL_NO { get; set; }
            public string CF_ARAC_MARKA_ADI { get; set; }
            public string CF_ARAC_MARKA_KODU { get; set; }
            public string CF_ARAC_TIP { get; set; }
            public string POL_BASLAMA_TARIH { get; set; }
            public string POL_BITIS_TARIH { get; set; }
            public string POL_CARI_POL_NO { get; set; }
            public string POL_DOVIZ_CINS { get; set; }
            public string POL_DOVIZ_KUR { get; set; }
            public string CF_GARANTI_FONU { get; set; }
            public string CF_GIDER_VERGISI { get; set; }
            public string CF_IMAL_YILI { get; set; }
            public string CF_KULLANIM_SEKLI { get; set; }
            public string CF_MODEL_YILI { get; set; }
            public string CF_MOTOR_NO { get; set; }
            public string CF_MUSTERI_ADRES { get; set; }
            public string CF_MUSTERI_UNVAN { get; set; }
            public string PSETT_VERGI_DAIRESI { get; set; }
            public string PSETT_VERGI_NO { get; set; }
            public string CF_MUST_EMAIL { get; set; }
            public string CF_ONAY_TARIHI { get; set; }
            public string POL_OP_ID { get; set; }
            public string CF_PLAKA { get; set; }
            public string CF_RIZIKO_ADRESI { get; set; }
            public string CF_SANAL_POS { get; set; }
            public string CF_SASI_NO { get; set; }
            public string CF_SIGORTALI_DOGUM_YERI { get; set; }
            public string CF_SIGORTALI_FAX { get; set; }
            public string CF_SIGORTALI_IL { get; set; }
            public string CF_SIGORTALI_TC_KIMLIK { get; set; }
            public string CF_SIGORTALI_TEL_NO { get; set; }
            public string CF_SIGORTALI_UNVAN { get; set; }
            public string CF_SIGORTALI_VERGI_NO { get; set; }
            public string CF_SIGORTA_ETTIREN_TC_KIMLIK { get; set; }
            public string CF_SIGORTA_ETTIREN_VERGI_NO { get; set; }
            public string POL_SON_DURUM { get; set; }
            public string POL_TANZIM_TARIH { get; set; }
            public string POL_TARIFE_KOD { get; set; }
            public string POL_TECDIT_NO { get; set; }
            public string CF_TESCIL_TARIH { get; set; }
            public string CF_TRAFIGE_CIKIS_TARIHI { get; set; }
            public string CF_TRAFIK_GARANTI_FONU { get; set; }
            public string CF_TRAFIK_TESCIL_SERI { get; set; }
            public string CF_TRAFIK_TESCIL_SIRA { get; set; }
            public string CF_TUTAR { get; set; }
            public string PTAKO_VADE { get; set; }
            public string CF_YANGIN_SIGORTA_VERGISI { get; set; }
            public string CF_YOLCU_SAYISI { get; set; }
            public string POL_ZEYL_SIRA_NO { get; set; }
            public string CF_SUM_BRUT_PRIM { get; set; }
            public string CF_SUM_KOM_TUTARI { get; set; }
            public string CF_SUM_NET_PRIM { get; set; }
        }
        //public class NeoOnline_TahsilatKapatma
        //{
        //    public string Sirket_Ismi { get; set; }
        //    public string Police_No { get; set; }
        //    public string Yenileme_No { get; set; }
        //    public string Ek_No { get; set; }
        //    public string Brut_Prim { get; set; }
        //    public string Taksit_Sayisi { get; set; }
        //    public string Kart_Sahibi { get; set; }
        //    public string Kimlik_No { get; set; }
        //    public string Kart_No { get; set; }
        //    public string Doviz_Kodu { get; set; }
        //    public string Tahsilat_Tutari { get; set; }
        //    public string Tahsilat_Tarihi { get; set; }



        //}

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
