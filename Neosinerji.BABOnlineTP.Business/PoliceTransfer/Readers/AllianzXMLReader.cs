using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using static Neosinerji.BABOnlineTP.Business.PoliceTransfer.SFSExcelOrient;


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class AllianzXMLReader : IPoliceTransferReader
    {
        private string filePath;
        private int tvmkodu;
        private string message;
        private bool TahsilatMi;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        IAktifKullaniciService _AktifKullanici;
        ITVMService _TVMService;
        public AllianzXMLReader(IAktifKullaniciService aktifKullanici)
        { }

        public AllianzXMLReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();
            List<PoliceTahsilat> tahsilatlar = new List<PoliceTahsilat>();
            XmlDocument doc = null;

            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();
            string[] tempPath = filePath.Split('#');
            if (tempPath.Length > 1)
            {
                policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                filePath = filePath.Substring(0, filePath.IndexOf("#"));
                //NeoOnline_TahsilatKapatma neoOnline_TahsilatKapatma;
                //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\NeoOnline_TahsilatKapatma.xls";
                //FileStream excelFile = new FileStream(path, FileMode.Open, FileAccess.Read);
                //HSSFWorkbook wb1 = new HSSFWorkbook(excelFile);
                //ISheet sheet1 = wb1.GetSheet("Sheet1");
                //int startRow = sheet1.FirstRowNum;
                //for (int indx = startRow + 2; indx <= sheet1.LastRowNum; indx++)
                //{
                //    IRow row = sheet1.GetRow(indx);
                //    var pno = row.GetCell(1).NumericCellValue.ToString();
                //    var yno = row.GetCell(2).NumericCellValue.ToString();
                //    var eno = row.GetCell(3).NumericCellValue.ToString();
                //    var kkno = row.GetCell(7).StringCellValue.Trim();
                //    neoOnline_TahsilatKapatma = new NeoOnline_TahsilatKapatma { Police_No = pno, Yenileme_No = yno, Ek_No = eno, Kart_No = kkno };
                //    policeTahsilatKapatma.Add(neoOnline_TahsilatKapatma);
                //}
            }

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            decimal dovizKuru = 1;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;

            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.XmlResolver = null;
                doc.Load(filePath);
                var tanimliOdemeTipleri = _TVMService.GetListTanımliBransOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ALLIANZSIGORTA);

                XmlNodeList xnList1 = doc.SelectNodes("/ACENTE_MASTER_TABLE/KOC_SIRKET/ACENTE/URUN_LIST/URUN");
                XmlNodeList xnListAcente = doc.SelectNodes("/ACENTE_MASTER_TABLE/KOC_SIRKET/ACENTE/ALT_ACENTE_LIST/ACENTE/URUN_LIST/URUN");
                XmlNodeList xnList2 = doc.SelectNodes("/ACENTE_TAHSILAT/KOC_SIRKET/ACENTE/TAHSILAT_LIST/TAHSILAT");
                if (xnList1.Count > 0)
                {

                    foreach (XmlNode xn in xnList1)
                    {

                        XmlNodeList polices = xn.ChildNodes;
                        for (int indx = 0; indx < polices.Count; indx++)
                        {

                            XmlNode polNodes = polices[indx];
                            XmlNodeList polChilds = polNodes.ChildNodes;
                            for (int zindx = 0; zindx < polChilds.Count; zindx++)
                            {
                                Police police = new Police();
                                XmlNode polNode = polChilds[zindx];

                                #region Genel Bilgiler

                                if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                else police.GenelBilgiler.TVMKodu = 0;

                                tumUrunKodu = polNode["URUN_KODU"].InnerText;
                                police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ALLIANZSIGORTA;
                                police.GenelBilgiler.PoliceNumarasi = polNode["POLICE_NO"].InnerText;
                                police.GenelBilgiler.EkNo = Util.toInt(polNode["ZEYIL_NO"].InnerText);
                                police.GenelBilgiler.ZeyilKodu = polNode["ZEYIL_TIPI"].InnerText;
                                //yenilemeNo yok.
                                police.GenelBilgiler.YenilemeNo = 0;
                                police.GenelBilgiler.TanzimTarihi = Util.toDate(polNode["TANZIM_TARIHI"].InnerText, Util.DateFormat2);
                                police.GenelBilgiler.BaslangicTarihi = Util.toDate(polNode["BASLANGIC_TARIHI"].InnerText, Util.DateFormat2);
                                police.GenelBilgiler.BitisTarihi = Util.toDate(polNode["BITIS_TARIHI"].InnerText, Util.DateFormat2);
                                police.GenelBilgiler.ToplamVergi = Util.ToDecimal(polNode["TOPLAM_VERGI"].InnerText);
                                police.GenelBilgiler.ParaBirimi = polNode["PRIM_DOVIZ_TIPI"].InnerText;
                                police.GenelBilgiler.DovizKur = Util.ToDecimal(polNode["PRIM_DOVIZ_KURU"].InnerText.Replace(".", ","));

                                if (police.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    dovizKuru = police.GenelBilgiler.DovizKur.Value;
                                }
                                police.GenelBilgiler.NetPrim = Util.ToDecimal(polNode["NET_PRIM"].InnerText);
                                police.GenelBilgiler.BrutPrim = Util.ToDecimal(polNode["BRUT_PRIM"].InnerText);
                                police.GenelBilgiler.Komisyon = Util.ToDecimal(polNode["KOMISYON"].InnerText);
                                if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                {
                                    police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(polNode["NET_PRIM_DOVIZ"].InnerText);
                                    police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(polNode["BRUT_PRIM_DOVIZ"].InnerText);
                                    police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(polNode["KOMISYON_DOVIZ"].InnerText);
                                }
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    //police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                                    //police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                    //police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                                    dovizKuru = 0;
                                }
                                // Odeme Sekli Belirleniyor...
                                if (polNode["ODEME_SEKLI"].InnerText.Trim() == "TAKSITLI")
                                {
                                    police.GenelBilgiler.OdemeSekli = 2; //Vadeli
                                }
                                else if (polNode["ODEME_SEKLI"].InnerText.Trim() == "PESIN")
                                {
                                    police.GenelBilgiler.OdemeSekli = 1; //Peşin
                                }
                                PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                                PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                                police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                                police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                                #endregion

                                #region Police Araç
                                //string plakaNo = polNode["PLAKA_NO"].InnerText;
                                //if (!String.IsNullOrEmpty(plakaNo))
                                //{
                                //    police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                //    police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                //    for (int i = 0; i < plakaNo.Length; i++)
                                //    {
                                //        if (i <= 2)
                                //        {
                                //            police.GenelBilgiler.PoliceArac.PlakaKodu += plakaNo[i].ToString();
                                //        }
                                //        if (i > 2)
                                //        {
                                //            police.GenelBilgiler.PoliceArac.PlakaNo += plakaNo[i].ToString();
                                //        }
                                //    }
                                //}

                                //if (polNode["BILGI_ADI"].InnerText == "PLAKA_NO :" || saha["BILGI_ADI"].InnerText == "PLAKA")
                                //{
                                //    var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                //    if (aciklama != null)
                                //    {
                                //        police.GenelBilgiler.PoliceArac.PlakaNo = saha["ACIKLAMA"].InnerText != "" && saha["ACIKLAMA"].InnerText.Length >= 2 ? saha["ACIKLAMA"].InnerText.Substring(2, saha["ACIKLAMA"].InnerText.Length - 2) : "";
                                //        police.GenelBilgiler.PoliceArac.PlakaKodu = saha["ACIKLAMA"].InnerText != "" && saha["ACIKLAMA"].InnerText.Length >= 2 ? saha["ACIKLAMA"].InnerText.Substring(0, 2) : "";
                                //    }
                                //}

                                #endregion

                                #region Sigorta Ettiren Bilgileri

                                police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polNode["SIGORTA_ETTIREN_AD_SOYAD"].InnerText;
                                //if (polNode.Name == "SIGORTA_ETTIREN_AD_SOYAD" || polNode.Name == "SIGORTA_ETTIREN_ADI_SOYADI")
                                //{
                                //    police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polNode["SIGORTA_ETTIREN_AD_SOYAD"].InnerText;
                                //}

                                if (polNode["SIGORTA_ETTIREN_TC_KIMLIK_NO"].InnerText.Length == 11)
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = polNode["SIGORTA_ETTIREN_TC_KIMLIK_NO"].InnerText;
                                }
                                if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo))
                                {
                                    if (polNode["SIGORTA_ETTIREN_VERGI_NO"].InnerText.Length == 10)
                                    {
                                        police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = polNode["SIGORTA_ETTIREN_VERGI_NO"].InnerText;
                                    }
                                }

                                sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                                police.GenelBilgiler.PoliceSigortaEttiren.Adres = !String.IsNullOrEmpty(polNode["SIGORTA_ETTIREN_ADRESI"].InnerText) ? polNode["SIGORTA_ETTIREN_ADRESI"].InnerText : ".";


                                #endregion

                                #region Sigortalı Bilgileri
                                XmlNode sigoraliList = polNode["SIGORTALI_LIST"];
                                XmlNodeList sigortalis = sigoraliList.ChildNodes;
                                for (int idx = 0; idx < sigortalis.Count; idx++)
                                {
                                    XmlNode elm = sigortalis.Item(idx);
                                    police.GenelBilgiler.PoliceSigortali.AdiUnvan = elm["SIGORTALI_ADI_SOYADI"].InnerText;
                                    sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    if (elm["TC_KIMLIK_NO"].InnerText.Length == 11)
                                    {
                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = elm["TC_KIMLIK_NO"].InnerText;

                                    }
                                    if (elm["VERGI_NO"].InnerText.Length == 10)
                                    {
                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = elm["VERGI_NO"].InnerText;

                                    }
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                    {
                                        if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                        {
                                            police.GenelBilgiler.PoliceSigortali.KimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                                        }
                                        if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == police.GenelBilgiler.PoliceSigortali.KimlikNo)
                                        {
                                            police.GenelBilgiler.PoliceSigortali.Adres = police.GenelBilgiler.PoliceSigortaEttiren.Adres;
                                        }
                                    }
                                    else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                    {
                                        if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                        {
                                            police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        }
                                        if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == police.GenelBilgiler.PoliceSigortali.VergiKimlikNo)
                                        {
                                            police.GenelBilgiler.PoliceSigortali.Adres = police.GenelBilgiler.PoliceSigortaEttiren.Adres;
                                        }
                                    }
                                    else
                                    {
                                        police.GenelBilgiler.PoliceSigortali.Adres = "";
                                    }

                                }
                                #endregion

                                #region Araç Bilgileri
                                XmlNode aracList = polNode["DETAIL_TABLE_LIST"];
                                XmlNodeList araclis = aracList.ChildNodes;
                                for (int idx = 0; idx < araclis.Count; idx++)
                                {
                                    XmlNode elm = araclis.Item(idx);
                                    if (!String.IsNullOrEmpty(elm["MODEL_KODU"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.AracinTipiKodu = (elm["MODEL_KODU"].InnerText);
                                    }
                                    if (!String.IsNullOrEmpty(elm["MODEL_YILI"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.Model = Util.toInt(elm["MODEL_YILI"].InnerText);
                                    }
                                    if (!String.IsNullOrEmpty(elm["MARKA_KODU"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.Marka = elm["MARKA_KODU"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["MOTOR_NO"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.MotorNo = elm["MOTOR_NO"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["SASI_NO"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.SasiNo = elm["SASI_NO"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["TRAFIGE_CIKIS_YILI"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.TrafikCikisTarihi = Util.toDate(elm["TRAFIGE_CIKIS_YILI"].InnerText, Util.DateFormat5);
                                    }
                                    if (!String.IsNullOrEmpty(elm["KULLANIM_TARZI"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.KullanimTarzi = elm["KULLANIM_TARZI"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["PLAKA_NO"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                        police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                        for (int i = 0; i < elm["PLAKA_NO"].InnerText.Length; i++)
                                        {
                                            if (i <= 2)
                                            {
                                                police.GenelBilgiler.PoliceArac.PlakaKodu += elm["PLAKA_NO"].InnerText[i].ToString().Trim().Replace(" ", "");

                                            }
                                            if (i > 2)
                                            {
                                                police.GenelBilgiler.PoliceArac.PlakaNo += elm["PLAKA_NO"].InnerText[i].ToString().Trim().Replace(" ", "");
                                            }


                                        }
                                    }

                                }
                                #endregion

                                #region Odeme Plani
                                var checkOdemeTipi = polNode.SelectSingleNode("ODEME_KREDI_KARTIYLA") == null ? false : true;
                                decimal taksitTutari = 0;
                                XmlNode odemeList = polNode["ODEME_LIST"];
                                XmlNodeList odemes = odemeList.ChildNodes;

                                var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);

                                for (int index = 0; index < odemes.Count; index++)
                                {
                                    XmlNode elm = odemes.Item(index);
                                    PoliceOdemePlani odm = new PoliceOdemePlani();

                                    odm.TaksitNo = index + 1;
                                    //odm.OdemeTipi = 2;  // Kredi karti !!!!!!
                                    odm.VadeTarihi = Util.toDate(elm["VADESI"].InnerText, Util.DateFormat2);
                                    odm.TaksitTutari = Util.ToDecimal(elm["TUTARI"].InnerText);
                                    if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                    {
                                        odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["TUTARI"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                        odm.DovizliTaksitTutari = Util.ToDecimal(elm["TUTARI"].InnerText);
                                    }
                                    if (checkOdemeTipi)
                                    {
                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    }
                                    else
                                    {
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                    }


                                    var ayniTaksitVarmi = police.GenelBilgiler.PoliceOdemePlanis.Where(y => y.TaksitNo == odm.TaksitNo).FirstOrDefault();
                                    if (ayniTaksitVarmi == null)
                                    {
                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }
                                    else
                                    {
                                        odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                        police.GenelBilgiler.PoliceOdemePlanis.Remove(ayniTaksitVarmi);
                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }
                                    #region Tahsilat işlemi



                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ALLIANZSIGORTA, police.GenelBilgiler.BransKodu.Value);

                                    if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                    {
                                        int otoOdeSayac = 0;
                                        foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                        {
                                            if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                            {
                                                otoOdeSayac++;
                                                PoliceTahsilat tahsilat = new PoliceTahsilat();

                                                tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                if (tahsilat.OdemTipi == 2 || tahsilat.OdemTipi == 5 || tahsilat.OdemTipi == 6 || tahsilat.OdemTipi == 9)
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
                                                tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                tahsilat.TaksitNo = odm.TaksitNo;
                                                tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                tahsilat.KayitTarihi = DateTime.Today;
                                                tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                if (tahsilat.TaksitTutari != 0)
                                                {
                                                    //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        if (checkOdemeTipi)
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        }
                                        else
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                        }
                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = "";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }


                                    }



                                    #endregion
                                }
                                if (odemes.Count == 0 && police.GenelBilgiler.BrutPrim.Value != 0)
                                {

                                    PoliceOdemePlani odmm = new PoliceOdemePlani();

                                    if (odmm.TaksitTutari == null)
                                    {
                                        odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                        if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                        {
                                            odmm.TaksitTutari = police.GenelBilgiler.BrutPrim.Value;
                                            odmm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim.Value;
                                        }
                                        if (odmm.VadeTarihi == null)
                                        {
                                            odmm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                        }
                                        if (checkOdemeTipi)
                                        {
                                            odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        }
                                        else
                                        {
                                            odmm.OdemeTipi = OdemeTipleri.Havale;
                                        }

                                        odmm.TaksitNo = 1;

                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);



                                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ANADOLUSIGORTA, police.GenelBilgiler.BransKodu.Value);

                                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                        {
                                            int otoOdeSayac = 0;
                                            foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                            {
                                                if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                {
                                                    otoOdeSayac++;
                                                    PoliceTahsilat tahsilat = new PoliceTahsilat();

                                                    tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                    odmm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                    if (tahsilat.OdemTipi == 1)
                                                    {
                                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                        tahsilat.KalanTaksitTutari = 0;
                                                        tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                                    }
                                                    else
                                                    {
                                                        tahsilat.OdenenTutar = 0;
                                                        tahsilat.KalanTaksitTutari = odmm.TaksitTutari;
                                                    }
                                                    tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilat.TaksitNo = odmm.TaksitNo;
                                                    tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                                    tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilat.KayitTarihi = DateTime.Today;
                                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                    if (tahsilat.TaksitTutari != 0)
                                                    {
                                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                            if (checkOdemeTipi)
                                            {
                                                tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                            }
                                            else
                                            {
                                                tahsilat.OdemTipi = OdemeTipleri.Havale;
                                                odmm.OdemeTipi = OdemeTipleri.Havale;
                                            }
                                            tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = odmm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                            //tahsilat.OdemeBelgeNo = "111111";
                                            tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                            tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                            tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.Value;
                                            tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }

                                        }

                                    }

                                }
                                #endregion

                                #region Vergiler

                                XmlNode vergiList = polNode["VERGI_LIST"];
                                XmlNodeList vergis = vergiList.ChildNodes;
                                for (int index = 0; index < vergis.Count; index++)
                                {
                                    XmlNode elm = vergis.Item(index);

                                    if (elm["VERGI_KODU"].InnerText == "1")
                                    {

                                        PoliceVergi yv = new PoliceVergi();
                                        yv.VergiKodu = 4;
                                        yv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);

                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 4).FirstOrDefault();
                                        if (varmi == null)
                                        {

                                            police.GenelBilgiler.PoliceVergis.Add(yv);

                                        }
                                        else
                                        {
                                            yv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText) + varmi.VergiTutari;
                                            police.GenelBilgiler.PoliceVergis.Remove(varmi);
                                            police.GenelBilgiler.PoliceVergis.Add(yv);

                                        }

                                        continue;

                                    }

                                    if (elm["VERGI_KODU"].InnerText == "2")
                                    {
                                        PoliceVergi gv = new PoliceVergi();
                                        gv.VergiKodu = 2;
                                        gv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);

                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 2).FirstOrDefault();
                                        if (varmi == null)
                                        {
                                            police.GenelBilgiler.PoliceVergis.Add(gv);

                                        }
                                        else
                                        {
                                            gv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText) + varmi.VergiTutari;
                                            police.GenelBilgiler.PoliceVergis.Remove(varmi);
                                            police.GenelBilgiler.PoliceVergis.Add(gv);

                                        }


                                        continue;
                                    }

                                    if (elm["VERGI_KODU"].InnerText == "3")
                                    {
                                        PoliceVergi thgf = new PoliceVergi();
                                        thgf.VergiKodu = 1;
                                        thgf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);

                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 3).FirstOrDefault();
                                        if (varmi == null)
                                        {
                                            police.GenelBilgiler.PoliceVergis.Add(thgf);
                                        }
                                        else
                                        {
                                            thgf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText) + varmi.VergiTutari;
                                            police.GenelBilgiler.PoliceVergis.Remove(varmi);
                                            police.GenelBilgiler.PoliceVergis.Add(thgf);

                                        }
                                        continue;
                                    }

                                    if (elm["VERGI_KODU"].InnerText == "4")
                                    {
                                        PoliceVergi gf = new PoliceVergi();
                                        gf.VergiKodu = 3;
                                        gf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);
                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 3).FirstOrDefault();
                                        if (varmi == null)
                                        {
                                            police.GenelBilgiler.PoliceVergis.Add(gf);
                                        }
                                        else
                                        {
                                            gf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText) + varmi.VergiTutari;
                                            police.GenelBilgiler.PoliceVergis.Remove(varmi);
                                            police.GenelBilgiler.PoliceVergis.Add(gf);

                                        }
                                        continue;
                                    }

                                }



                                if (tumUrunAdi == null)
                                {
                                    police.GenelBilgiler.TUMUrunAdi = PoliceBransEslestir.TUMUrunAdi;
                                }
                                else
                                {
                                    police.GenelBilgiler.TUMUrunAdi = tumUrunAdi;
                                }

                                if (tumUrunKodu == null)
                                {
                                    police.GenelBilgiler.TUMUrunKodu = PoliceBransEslestir.TUMUrunKodu;
                                }
                                else
                                {
                                    police.GenelBilgiler.TUMUrunKodu = tumUrunKodu;
                                }

                                police.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                                police.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;

                                police.GenelBilgiler.Durum = 0;
                                policeler.Add(police);

                                #endregion
                            }
                        }
                    }
                }
                else if (xnList2.Count > 0)
                {
                    #region tahsilat
                    IPoliceService _PoliceService = DependencyResolver.Current.GetService<IPoliceService>();
                    IPoliceTransferService _PoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
                    IPoliceContext _PoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
                    string policeNumarasi = "0";
                    int? ekNo = null;
                    int polSayac = 0;
                    int varOlanKayitlar = 0;
                    int policesiOlmayanSayac = 0;
                    foreach (XmlNode xn in xnList2)
                    {
                        XmlNodeList polices = xn.ChildNodes;
                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                        //  PoliceOdemePlani odm = new PoliceOdemePlani();
                        var polGenel = new PoliceGenel();
                        bool odemeType = false;
                        tahsilat = new PoliceTahsilat();
                        for (int indx = 0; indx < polices.Count; indx++)
                        {
                            XmlNode polNode = polices[indx];

                            if (polNode.Name == "POLICE_NO") policeNumarasi = polNode.InnerText;
                            if (polNode.Name == "ZEYIL_NO") ekNo = Util.toInt(polNode.InnerText);

                            if (policeNumarasi != "0" && ekNo != null)
                            {
                                polGenel = _PoliceService.getTahsilatPolice(SigortaSirketiBirlikKodlari.ALLIANZSIGORTA, policeNumarasi, ekNo.Value);

                                #region Tahsilat
                                if (polGenel != null)
                                {
                                    if (polNode.Name == "TAKSIT_VADESI") tahsilat.TaksitVadeTarihi = Util.toDate(polNode.InnerText, Util.DateFormat2).HasValue ?
                                        Util.toDate(polNode.InnerText, Util.DateFormat2).Value : Convert.ToDateTime("01.01.0001");

                                    if (polNode.Name == "TAKSIT_SIRA_NO") tahsilat.TaksitNo = Util.toInt(polNode.InnerText);

                                    if (polNode.Name == "TAHSILAT_TARIHI") tahsilat.OdemeBelgeTarihi = Util.toDate(polNode.InnerText, Util.DateFormat2).HasValue ?
                                        Util.toDate(polNode.InnerText, Util.DateFormat2).Value : Convert.ToDateTime("01.01.0001");

                                    if (polNode.Name == "ODEME_TIPI")
                                    {
                                        if (polNode.InnerText == "KK")
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                            //       odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                            odemeType = true;
                                        }
                                        else if (polNode.InnerText == "NAK")
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                            //          odm.OdemeTipi = OdemeTipleri.Nakit;
                                            tahsilat.OdemeBelgeNo = "111111";
                                            odemeType = true;
                                        }
                                        else
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            //          odm.OdemeTipi = OdemeTipleri.Nakit;
                                            tahsilat.OdemeBelgeNo = "111111";
                                            odemeType = true;
                                        }
                                    }

                                    #region Tahsilat işlemi
                                    if (odemeType)
                                    {
                                        var getDetay = _TVMService.GetDetay(tvmkodu);
                                        if (getDetay != null)
                                        {
                                            //if (getDetay.MuhasebeEntegrasyon.HasValue)
                                            //{
                                            //    if (!getDetay.MuhasebeEntegrasyon.Value)
                                            //    {

                                            if (odemeType && polNode.Name == "TUTAR")
                                            {
                                                tahsilat.TaksitTutari = Util.ToDecimal(polNode.InnerText);
                                                tahsilat.OdenenTutar = Util.ToDecimal(polNode.InnerText);
                                                tahsilat.KalanTaksitTutari = 0;
                                                tahsilat.PoliceNo = policeNumarasi;
                                                tahsilat.ZeyilNo = ekNo.ToString();
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.PoliceSigortaEttiren.KimlikNo) ? polGenel.PoliceSigortaEttiren.KimlikNo : polGenel.PoliceSigortaEttiren.VergiKimlikNo;
                                                tahsilat.BrutPrim = polGenel.BrutPrim.HasValue ? polGenel.BrutPrim.Value : 0;
                                                tahsilat.KayitTarihi = DateTime.Today;
                                                tahsilat.KaydiEkleyenKullaniciKodu = tvmkodu;
                                                var resultAyniKayitmi = polGenel.PoliceTahsilats.Where(s => s.PoliceNo == policeNumarasi
                                                       && s.ZeyilNo == ekNo.ToString()
                                                       && s.OdemTipi == tahsilat.OdemTipi
                                                       && s.TaksitNo == tahsilat.TaksitNo
                                                       && s.OdenenTutar == tahsilat.OdenenTutar
                                                       ).FirstOrDefault();
                                                if (resultAyniKayitmi == null)
                                                {
                                                    polGenel.PoliceTahsilats.Add(tahsilat);
                                                    _PoliceContext.PoliceGenelRepository.Update(polGenel);
                                                    _PoliceContext.Commit();
                                                    odemeType = false;
                                                    polSayac++;
                                                }
                                                List<PoliceOdemePlani> polOdemePlaniList = new List<PoliceOdemePlani>();
                                                polOdemePlaniList = polGenel.PoliceOdemePlanis.Where(s => s.PoliceId == polGenel.PoliceId).ToList<PoliceOdemePlani>();
                                                if (polOdemePlaniList != null)
                                                {
                                                    foreach (var item in polOdemePlaniList)
                                                    {
                                                        item.OdemeTipi = Convert.ToByte(tahsilat.OdemTipi);
                                                        polGenel.PoliceOdemePlanis.Add(item);
                                                        _PoliceContext.PoliceGenelRepository.Update(polGenel);
                                                        _PoliceContext.Commit();
                                                    }


                                                    odemeType = false;
                                                }


                                                else
                                                {
                                                    varOlanKayitlar++;
                                                }
                                                break;
                                                //}
                                                //}
                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    policesiOlmayanSayac++;
                                }
                                #endregion
                            }
                        }
                    }
                    this.message = "Tahsilatı kapatılan kayıt sayısı: " + polSayac + " Daha önceden tahsilatı kapatılan kayıt sayısı:  " + varOlanKayitlar +
                        " Poliçesi transfer edilmediği için tahsilatı kapatılamayan kayıt sayısı:  " + policesiOlmayanSayac;
                    this.TahsilatMi = true;
                    return null;
                    #endregion
                }
                else if (xnListAcente.Count > 0)
                {
                    foreach (XmlNode xn in xnListAcente)
                    {
                        XmlNodeList polices = xn.ChildNodes;
                        for (int indx = 0; indx < polices.Count; indx++)
                        {
                            XmlNode polNodes = polices[indx];
                            XmlNodeList polChilds = polNodes.ChildNodes;
                            for (int zindx = 0; zindx < polChilds.Count; zindx++)
                            {
                                Police police = new Police();
                                XmlNode polNode = polChilds[zindx];

                                #region Genel Bilgiler

                                if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                else police.GenelBilgiler.TVMKodu = 0;

                                tumUrunKodu = polNode["URUN_KODU"].InnerText;
                                police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ALLIANZSIGORTA;
                                police.GenelBilgiler.PoliceNumarasi = polNode["POLICE_NO"].InnerText;
                                police.GenelBilgiler.EkNo = Util.toInt(polNode["ZEYIL_NO"].InnerText);
                                police.GenelBilgiler.ZeyilKodu = polNode["ZEYIL_TIPI"].InnerText;
                                //yenilemeNo yok.
                                police.GenelBilgiler.YenilemeNo = 0;
                                police.GenelBilgiler.TanzimTarihi = Util.toDate(polNode["TANZIM_TARIHI"].InnerText, Util.DateFormat2);
                                police.GenelBilgiler.BaslangicTarihi = Util.toDate(polNode["BASLANGIC_TARIHI"].InnerText, Util.DateFormat2);
                                police.GenelBilgiler.BitisTarihi = Util.toDate(polNode["BITIS_TARIHI"].InnerText, Util.DateFormat2);
                                police.GenelBilgiler.ToplamVergi = Util.ToDecimal(polNode["TOPLAM_VERGI"].InnerText);
                                police.GenelBilgiler.ParaBirimi = polNode["PRIM_DOVIZ_TIPI"].InnerText;
                                if (polNode.Name == "PRIM_DOVIZ_KURU")
                                {
                                    if (!String.IsNullOrEmpty(polNode.InnerText))
                                    {
                                        police.GenelBilgiler.DovizKur = Util.ToDecimal(polNode.InnerText.Replace(".", ","));
                                    }
                                }

                                if (police.GenelBilgiler.ParaBirimi != "TL")
                                {
                                    if (polNode.Name == "PRIM_DOVIZ_KURU") dovizKuru = Util.ToDecimal(polNode.InnerText.Replace(".", ","));
                                }
                                police.GenelBilgiler.NetPrim = Util.ToDecimal(polNode["NET_PRIM"].InnerText);
                                police.GenelBilgiler.BrutPrim = Util.ToDecimal(polNode["BRUT_PRIM"].InnerText);
                                police.GenelBilgiler.Komisyon = Util.ToDecimal(polNode["KOMISYON"].InnerText);
                                if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                {
                                    police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(polNode["KOMISYON_DOVIZ"].InnerText);
                                    police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(polNode["NET_PRIM_DOVIZ"].InnerText);
                                    police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(polNode["BRUT_PRIM_DOVIZ"].InnerText);
                                }
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    //police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                                    //police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                    //police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                                    dovizKuru = 0;
                                }
                                // Odeme Sekli Belirleniyor...
                                if (polNode["ODEME_SEKLI"].InnerText.Trim() == "TAKSITLI")
                                {
                                    police.GenelBilgiler.OdemeSekli = 2; //Vadeli
                                }
                                else if (polNode["ODEME_SEKLI"].InnerText.Trim() == "PESIN")
                                {
                                    police.GenelBilgiler.OdemeSekli = 1; //Peşin
                                }
                                PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                                PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                                police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                                police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                                #endregion

                                #region Police Araçc
                                //string plakaNo = polNode["PLAKA_NO"].InnerText;
                                //if (!String.IsNullOrEmpty(plakaNo))
                                //{
                                //    police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                //    police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                //    for (int i = 0; i < plakaNo.Length; i++)
                                //    {
                                //        if (i <= 2)
                                //        {
                                //            police.GenelBilgiler.PoliceArac.PlakaKodu += plakaNo[i].ToString();
                                //        }
                                //        if (i > 2)
                                //        {
                                //            police.GenelBilgiler.PoliceArac.PlakaNo += plakaNo[i].ToString();
                                //        }
                                //    }
                                //}

                                #endregion

                                #region Sigorta Ettiren Bilgileri

                                police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polNode["SIGORTA_ETTIREN_AD_SOYAD"].InnerText;
                                //if (polNode.Name == "SIGORTA_ETTIREN_AD_SOYAD" || polNode.Name == "SIGORTA_ETTIREN_ADI_SOYADI")
                                //{
                                //    police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polNode["SIGORTA_ETTIREN_AD_SOYAD"].InnerText;
                                //}
                                if (polNode["SIGORTA_ETTIREN_TC_KIMLIK_NO"].InnerText.Length == 11)
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = polNode["SIGORTA_ETTIREN_TC_KIMLIK_NO"].InnerText;

                                }
                                if (polNode["SIGORTA_ETTIREN_VERGI_NO"].InnerText.Length == 10)
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = polNode["SIGORTA_ETTIREN_VERGI_NO"].InnerText;

                                }
                                if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "0")
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                                }
                                if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "0")
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                                }
                                if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "")
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                                }
                                if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "")
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                                }
                                sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                                //if (polNode.Name=="SIGORTA_ETTIREN_ADRESI")
                                //{
                                police.GenelBilgiler.PoliceSigortaEttiren.Adres = !String.IsNullOrEmpty(polNode["SIGORTA_ETTIREN_ADRESI"].InnerText) ? polNode["SIGORTA_ETTIREN_ADRESI"].InnerText : ".";
                                //}

                                #endregion

                                #region Sigortalı Bilgileri
                                XmlNode sigoraliList = polNode["SIGORTALI_LIST"];
                                XmlNodeList sigortalis = sigoraliList.ChildNodes;
                                for (int idx = 0; idx < sigortalis.Count; idx++)
                                {
                                    XmlNode elm = sigortalis.Item(idx);
                                    police.GenelBilgiler.PoliceSigortali.AdiUnvan = elm["SIGORTALI_ADI_SOYADI"].InnerText;
                                    if (elm["TC_KIMLIK_NO"].InnerText.Length == 11)
                                    {
                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = elm["TC_KIMLIK_NO"].InnerText;

                                    }
                                    if (elm["VERGI_NO"].InnerText.Length == 10)
                                    {
                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = elm["VERGI_NO"].InnerText;

                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.KimlikNo == "0")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "0")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.KimlikNo == "")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                                    }
                                    sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                    if (sLiKimlikNo == null && sLiKimlikNo == "")
                                    {

                                    }
                                    if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                    {
                                        if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo))
                                        {
                                            police.GenelBilgiler.PoliceSigortali.KimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                                        }
                                        if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == police.GenelBilgiler.PoliceSigortali.KimlikNo)
                                        {
                                            police.GenelBilgiler.PoliceSigortali.Adres = police.GenelBilgiler.PoliceSigortaEttiren.Adres;
                                        }
                                    }
                                    else if (!String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo) && !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                    {
                                        if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.VergiKimlikNo))
                                        {
                                            police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                        }
                                        if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == police.GenelBilgiler.PoliceSigortali.VergiKimlikNo)
                                        {
                                            police.GenelBilgiler.PoliceSigortali.Adres = police.GenelBilgiler.PoliceSigortaEttiren.Adres;
                                        }
                                    }
                                    else
                                    {
                                        police.GenelBilgiler.PoliceSigortali.Adres = "";
                                    }

                                }
                                #endregion

                                #region Araç Bilgileri
                                XmlNode aracList = polNode["DETAIL_TABLE_LIST"];
                                XmlNodeList araclis = aracList.ChildNodes;
                                for (int idx = 0; idx < araclis.Count; idx++)
                                {
                                    XmlNode elm = araclis.Item(idx);
                                    if (!String.IsNullOrEmpty(elm["MODEL_KODU"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.AracinTipiKodu = (elm["MODEL_KODU"].InnerText);
                                    }
                                    if (!String.IsNullOrEmpty(elm["MODEL_YILI"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.Model = Util.toInt(elm["MODEL_YILI"].InnerText);
                                    }
                                    if (!String.IsNullOrEmpty(elm["MARKA_KODU"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.Marka = elm["MARKA_KODU"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["MOTOR_NO"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.MotorNo = elm["MOTOR_NO"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["SASI_NO"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.SasiNo = elm["SASI_NO"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["TRAFIGE_CIKIS_YILI"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.TrafikCikisTarihi = Util.toDate(elm["TRAFIGE_CIKIS_YILI"].InnerText);
                                    }
                                    if (!String.IsNullOrEmpty(elm["KULLANIM_TARZI"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.KullanimTarzi = elm["KULLANIM_TARZI"].InnerText;
                                    }
                                    if (!String.IsNullOrEmpty(elm["PLAKA_NO"].InnerText))
                                    {
                                        police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                        police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                        for (int i = 0; i < elm["PLAKA_NO"].InnerText.Length; i++)
                                        {
                                            if (i <= 2)
                                            {
                                                police.GenelBilgiler.PoliceArac.PlakaKodu += elm["PLAKA_NO"].InnerText[i].ToString();
                                            }
                                            if (i > 2)
                                            {
                                                police.GenelBilgiler.PoliceArac.PlakaNo += elm["PLAKA_NO"].InnerText[i].ToString();
                                            }
                                        }
                                    }

                                }
                                #endregion
                                #region Odeme Plani

                                var checkOdemeTipi = polNode.SelectSingleNode("ODEME_KREDI_KARTIYLA") == null ? false : true;
                                decimal taksitTutari = 0;
                                XmlNode odemeList = polNode["ODEME_LIST"];
                                XmlNodeList odemes = odemeList.ChildNodes;

                                var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);

                                for (int index = 0; index < odemes.Count; index++)
                                {
                                    XmlNode elm = odemes.Item(index);
                                    PoliceOdemePlani odm = new PoliceOdemePlani();

                                    odm.TaksitNo = index + 1;
                                    //  odm.OdemeTipi = 2;  // Kredi karti !!!!!!
                                    odm.VadeTarihi = Util.toDate(elm["VADESI"].InnerText, Util.DateFormat2);
                                    odm.TaksitTutari = Util.ToDecimal(elm["TUTARI"].InnerText);
                                    if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                    {
                                        odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["TUTARI"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                        odm.DovizliTaksitTutari = Util.ToDecimal(elm["TUTARI"].InnerText);
                                    }
                                    if (checkOdemeTipi)
                                    {
                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    }
                                    else
                                    {
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                    }


                                    var ayniTaksitVarmi = police.GenelBilgiler.PoliceOdemePlanis.Where(y => y.TaksitNo == odm.TaksitNo).FirstOrDefault();
                                    if (ayniTaksitVarmi == null)
                                    {
                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }
                                    else
                                    {
                                        odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                        police.GenelBilgiler.PoliceOdemePlanis.Remove(ayniTaksitVarmi);
                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }
                                    #region Tahsilat işlemi



                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ALLIANZSIGORTA, police.GenelBilgiler.BransKodu.Value);

                                    if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                    {
                                        int otoOdeSayac = 0;
                                        foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                        {
                                            if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
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
                                                tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                tahsilat.TaksitNo = odm.TaksitNo;
                                                tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                tahsilat.KayitTarihi = DateTime.Today;
                                                tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                if (tahsilat.TaksitTutari != 0)
                                                {
                                                    //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        if (checkOdemeTipi)
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        }
                                        else
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                        }
                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                        tahsilat.OdemeBelgeNo = "";
                                        tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                        tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }



                                    #endregion
                                }
                                if (odemes.Count == 0)
                                {

                                    PoliceOdemePlani odmm = new PoliceOdemePlani();

                                    if (odmm.TaksitTutari == null)
                                    {
                                        odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                        if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                        {
                                            odmm.TaksitTutari = police.GenelBilgiler.BrutPrim.Value;
                                            odmm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim.Value;
                                        }
                                        if (odmm.VadeTarihi == null)
                                        {
                                            odmm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                        }
                                        if (checkOdemeTipi)
                                        {
                                            odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        }
                                        else
                                        {
                                            odmm.OdemeTipi = OdemeTipleri.Havale;
                                        }
                                        odmm.TaksitNo = 1;

                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);

                                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ALLIANZSIGORTA, police.GenelBilgiler.BransKodu.Value);

                                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                        {
                                            int otoOdeSayac = 0;
                                            foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                            {
                                                if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                {
                                                    otoOdeSayac++;
                                                    PoliceTahsilat tahsilat = new PoliceTahsilat();

                                                    tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                    odmm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                    if (tahsilat.OdemTipi == 1)
                                                    {
                                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                        tahsilat.KalanTaksitTutari = 0;
                                                        tahsilat.OdemeBelgeNo = "111111****1111";
                                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                                    }
                                                    else
                                                    {
                                                        tahsilat.OdenenTutar = 0;
                                                        tahsilat.KalanTaksitTutari = odmm.TaksitTutari;
                                                    }
                                                    tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilat.TaksitNo = odmm.TaksitNo;
                                                    tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                                    tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilat.KayitTarihi = DateTime.Today;
                                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                    if (tahsilat.TaksitTutari != 0)
                                                    {
                                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                    }
                                                }
                                            }

                                        }
                                        else
                                        {
                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                            if (checkOdemeTipi)
                                            {
                                                tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                            }
                                            else
                                            {
                                                tahsilat.OdemTipi = OdemeTipleri.Havale;
                                                odmm.OdemeTipi = OdemeTipleri.Havale;
                                            }
                                            tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilat.TaksitNo = odmm.TaksitNo;
                                            tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                            tahsilat.OdemeBelgeNo = "";
                                            tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                            tahsilat.OdenenTutar = 0;
                                            tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                            tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                            tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                            tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                            tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                            tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                            if (tahsilat.TaksitTutari != 0)
                                            {
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                            }
                                        }

                                    }

                                }
                                #endregion

                                #region Vergiler

                                XmlNode vergiList = polNode["VERGI_LIST"];
                                XmlNodeList vergis = vergiList.ChildNodes;

                                for (int index = 0; index < vergis.Count; index++)
                                {
                                    XmlNode elm = vergis.Item(index);
                                    if (elm["VERGI_KODU"].InnerText == "1")
                                    {
                                        PoliceVergi yv = new PoliceVergi();
                                        yv.VergiKodu = 4;
                                        yv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);

                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 1).FirstOrDefault();
                                        if (varmi == null)
                                        {
                                            police.GenelBilgiler.PoliceVergis.Add(yv);
                                        }

                                        continue;
                                    }


                                    if (elm["VERGI_KODU"].InnerText == "2")
                                    {
                                        PoliceVergi gv = new PoliceVergi();
                                        gv.VergiKodu = 2;
                                        gv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);

                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 2).FirstOrDefault();
                                        if (varmi == null)
                                        {
                                            police.GenelBilgiler.PoliceVergis.Add(gv);
                                        }


                                        continue;
                                    }

                                    if (elm["VERGI_KODU"].InnerText == "3")
                                    {
                                        PoliceVergi thgf = new PoliceVergi();
                                        thgf.VergiKodu = 1;
                                        thgf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);

                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 3).FirstOrDefault();
                                        if (varmi == null)
                                        {
                                            police.GenelBilgiler.PoliceVergis.Add(thgf);
                                        }

                                        continue;
                                    }

                                    if (elm["VERGI_KODU"].InnerText == "4")
                                    {
                                        PoliceVergi gf = new PoliceVergi();
                                        gf.VergiKodu = 3;
                                        gf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText);
                                        var varmi = police.GenelBilgiler.PoliceVergis.Where(s => s.VergiKodu == 4).FirstOrDefault();
                                        if (varmi == null)
                                        {
                                            police.GenelBilgiler.PoliceVergis.Add(gf);
                                        }

                                        continue;
                                    }


                                }



                                if (tumUrunAdi == null)
                                {
                                    police.GenelBilgiler.TUMUrunAdi = PoliceBransEslestir.TUMUrunAdi;
                                }
                                else
                                {
                                    police.GenelBilgiler.TUMUrunAdi = tumUrunAdi;
                                }

                                if (tumUrunKodu == null)
                                {
                                    police.GenelBilgiler.TUMUrunKodu = PoliceBransEslestir.TUMUrunKodu;
                                }
                                else
                                {
                                    police.GenelBilgiler.TUMUrunKodu = tumUrunKodu;
                                }

                                police.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                                police.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;

                                police.GenelBilgiler.Durum = 0;

                                policeler.Add(police);
                                #endregion
                            }
                        }
                    }
                }
                #endregion
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

        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
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
