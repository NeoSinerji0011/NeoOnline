using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Common;


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    class AvivaXMLSpreadSheetReader : IPoliceTransferReader
    {


        private string filePath;
        private int tvmKodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;

        private string[] columnNames =  {
                                            "ACENTA_NO",                        // 0
                                            "ACENTA_POL_NO",                    // 1
                                            "CF_ARACIN_TIPI",                   // 2
                                            "CF_ARAC_MARKA_ADI",                // 3
                                            "BASLAMA_TARIH",                    // 4
                                            "BITIS_TARIH",                      // 5
                                            "CARI_POL_NO",                      // 6
                                            "DOVIZ_CINS",                       // 7
                                            "DOVIZ_KUR",                        // 8
                                            "CF_GARANTI_FONU",                  // 9
                                            "CF_GIDER_VERGISI",                 // 10
                                            "CF_IMAL_YILI",                     // 11
                                            "CF_KULLANIM_SEKLI",                // 12
                                            "CF_MOTOR_NO",                      // 13
                                            "CF_MUSTERI_ADRES",                 // 14
                                            "CF_ONAY_TARIHI",                   // 15
                                            "OP_ID",                            // 16
                                            "CF_PLAKA",                         // 17
                                            "CF_RIZIKO_ADRESI",                 // 18
                                            "CF_SANAL_POS",                     // 19
                                            "CF_SASI_NO",                       // 20
                                            "CF_SIGORTALI_DOGUM_TARIHI",        // 21
                                            "CF_SIGORTALI_FAX",                 // 22
                                            "CF_SIGORTALI_TC_KIMLIK",           // 23
                                            "CF_SIGORTALI_TEL_NO",              // 24
                                            "SON_DURUM",                        // 25
                                            "TARIFE_KOD",                       // 26
                                            "TECDIT_NO",                        // 27
                                            "CF_TESCIL_TARIH",                  // 28
                                            "CF_TRAFIK_GARANTI_FONU",           // 29
                                            "CF_YANGIN_SIGORTA_VERGISI",        // 30
                                            "CF_YOLCU_ADEDI",                   // 31
                                            "ZEYL_SIRA_NO",                     // 32
                                            "CF_MUSTERI_UNVAN",                 // 33
                                            "VERGI_DAIRESI",                    // 34
                                            "VERGI_NO",                         // 35
                                            "CF_MUST_EMAIL",                    // 36
                                            "SIG_ETTIREN_NO",                   // 37
                                            "CF_SIGORTALI_UNVAN",               // 38
                                            "CF_TUTAR",                         // 39
                                            "VADE",                             // 40
                                            "CF_SUM_BRUT_PRIM",                 // 41
                                            "CF_SUM_KOM_TUTARI",                // 42
                                            "CF_SUM_NET_PRIM"                   // 43

                                        };

        public AvivaXMLSpreadSheetReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmKodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }

        string tumUrunAdi ;
        string tumUrunKodu ;

        public List<Police> getPoliceler()
        {
            XmlDocument doc = null;

            List<Police> policeler = new List<Police>();

            try
            {
                #region Poliçe Reader

                #endregion
                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode workbook = doc.LastChild;
                XmlNode worksheat = workbook.LastChild;
                XmlNode table = worksheat.FirstChild;
                XmlNodeList rows = table.ChildNodes;

                // get first row of table and check excel column sequence (column names)
                XmlNode firstRow = rows[0];
                XmlNodeList cells = firstRow.ChildNodes;
                bool tf = true;
                for (int indx = 0; indx < cells.Count; indx++)
                {
                    XmlNode cn = cells[indx];
                    string columnName = cn.InnerText;
                    if (columnName != columnNames[indx])
                    {
                        tf = false;
                        break;
                    }
                }
                if (tf == false)
                {
                    message = "excel format error!!!!";
                    return null;
                }

                // column sequence is correct
                // start to read rows.

                int recordNo = 0;

                string polNo = null;
                tumUrunAdi = null;
                tumUrunKodu = null;

                Police pol = new Police();

                for (int indx = 1; indx < rows.Count; indx++)
                {
                    XmlNode row = rows[indx];
                    XmlNodeList rcell = row.ChildNodes;

                    // start to get cells of row

                    string cariPolNo = rcell[6].InnerText;

                    if (polNo == null)
                    {
                        recordNo = 1;
                        polNo = cariPolNo;
                    }

                    if (polNo == cariPolNo)
                    {
                        if (recordNo > 1) // odeme plani - diger taksitler aliniyor
                        {
                            // add odeme plani - taksitler
                            PoliceOdemePlani odm = new PoliceOdemePlani();
                            odm.TaksitNo = recordNo;
                            odm.TaksitTutari = Util.ToDecimal(rcell[39].InnerText);
                            odm.VadeTarihi = Util.toDate(rcell[40].InnerText, Util.DateFormat3);

                            if (odm.TaksitTutari != 0)
                            {
                                pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                pol.GenelBilgiler.OdemeSekli = 2; // Vadeli
                            }
                        }
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
            }
            catch (Exception ex)
            {
                this.message = ex.ToString();
                policeler = null;

            }

            return policeler;
        }

        private void transferCellDataToPolice(Police pol, XmlNode row)
        {
            XmlNodeList rcell = row.ChildNodes;

            // Birlik Kodu
            pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AVIVASIGORTA;
            // tvm kodu
            pol.GenelBilgiler.TVMKodu = tvmKodu;

            pol.GenelBilgiler.BaslangicTarihi = Util.toDate(rcell[4].InnerText, Util.DateFormat3);
            pol.GenelBilgiler.BitisTarihi = Util.toDate(rcell[5].InnerText, Util.DateFormat3);
            pol.GenelBilgiler.ParaBirimi = rcell[7].InnerText;
            if (!String.IsNullOrEmpty(rcell[8].InnerText))
            {
                pol.GenelBilgiler.DovizKur = Util.ToDecimal(rcell[8].InnerText.Replace(".", ","));
            }           

            // Garanti fonu
            pol.GenelBilgiler.ToplamVergi = 0;

            PoliceVergi gf = new PoliceVergi();
            gf.VergiKodu = 3;
            gf.VergiTutari = Util.ToDecimal(rcell[9].InnerText);
            pol.GenelBilgiler.PoliceVergis.Add(gf);
            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gf.VergiTutari;

            // Gider vergisi
            PoliceVergi gv = new PoliceVergi();
            gv.VergiKodu = 2;
            gv.VergiTutari = Util.ToDecimal(rcell[10].InnerText);
            pol.GenelBilgiler.PoliceVergis.Add(gv);
            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + gv.VergiTutari;

            pol.GenelBilgiler.PoliceSigortaEttiren.Adres = rcell[14].InnerText;
            pol.GenelBilgiler.TanzimTarihi = Util.toDate(rcell[15].InnerText, Util.DateFormat1);
            pol.GenelBilgiler.PoliceSigortali.KimlikNo = rcell[23].InnerText;
            tumUrunKodu = rcell[26].InnerText;
            pol.GenelBilgiler.YenilemeNo = Util.toInt(rcell[27].InnerText);

            // Trafik hizmetleri vergisi
            PoliceVergi tghf = new PoliceVergi();
            tghf.VergiKodu = 1;
            tghf.VergiTutari = Util.ToDecimal(rcell[29].InnerText);
            pol.GenelBilgiler.PoliceVergis.Add(tghf);
            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + tghf.VergiTutari;

            // ysv
            PoliceVergi ysv = new PoliceVergi();
            ysv.VergiKodu = 4;
            ysv.VergiTutari = Util.ToDecimal(rcell[30].InnerText);
            pol.GenelBilgiler.PoliceVergis.Add(ysv);
            pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi + ysv.VergiTutari;

            pol.GenelBilgiler.EkNo = Util.toInt(rcell[32].InnerText);
            pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = rcell[33].InnerText;
            pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = rcell[35].InnerText;
            pol.GenelBilgiler.PoliceSigortali.AdiUnvan = rcell[38].InnerText;

            // Odeme - Ilk taksit 
            PoliceOdemePlani odm = new PoliceOdemePlani();
            odm.TaksitNo = 1;
            odm.TaksitTutari = Util.ToDecimal(rcell[39].InnerText);
            odm.VadeTarihi = Util.toDate(rcell[40].InnerText, Util.DateFormat3);

            if (odm.TaksitTutari != 0)
            {
                pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                pol.GenelBilgiler.OdemeSekli = 1; // pesin
            }

            pol.GenelBilgiler.BrutPrim = Util.ToDecimal(rcell[41].InnerText);
            pol.GenelBilgiler.Komisyon = Util.ToDecimal(rcell[42].InnerText);
            pol.GenelBilgiler.NetPrim = Util.ToDecimal(rcell[43].InnerText);
        }

        public string getMessage()
        {
            return this.message;
        }
        
        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }

    }
}
