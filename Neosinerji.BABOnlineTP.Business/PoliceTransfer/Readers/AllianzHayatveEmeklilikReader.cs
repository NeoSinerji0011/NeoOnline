using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class AllianzHayatveEmeklilikReader : IPoliceTransferReader
    {
         private string filePath;
        private int tvmkodu;
        private string message;
        private bool TahsilatMi;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;

        public AllianzHayatveEmeklilikReader()
        { }

        public AllianzHayatveEmeklilikReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;

        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();
            List<PoliceTahsilat> tahsilatlar = new List<PoliceTahsilat>();
            XmlDocument doc = null;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            decimal dovizKuru = 1;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNodeList xnList1 = doc.SelectNodes("/ACENTE_MASTER_TABLE/KOC_SIRKET/ACENTE/URUN_LIST/URUN");
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
                                police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ALLIANZHAYATVEEMEKLILIK;
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
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {                                    
                                    police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * dovizKuru;
                                    police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * dovizKuru;
                                    police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * dovizKuru;
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

                                #region Sigorta Ettiren Bilgileri

                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = polNode["SIGORTA_ETTIREN_VERGI_NO"].InnerText;
                                police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polNode["SIGORTA_ETTIREN_ADI_SOYADI"].InnerText;
                              
                                police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = polNode["SIGORTA_ETTIREN_TC_KIMLIK_NO"].InnerText;
                                police.GenelBilgiler.PoliceSigortaEttiren.Adres = polNode["SIGORTA_ETTIREN_ADRESI"].InnerText;
                                #endregion

                                #region Sigortalı Bilgileri
                                XmlNode sigoraliList = polNode["SIGORTALI_LIST"];
                                XmlNodeList sigortalis = sigoraliList.ChildNodes;
                                for (int idx = 0; idx < sigortalis.Count; idx++)
                                {
                                    XmlNode elm = sigortalis.Item(idx);
                                    police.GenelBilgiler.PoliceSigortali.AdiUnvan = elm["SIGORTALI_ADI_SOYADI"].InnerText;
                                    //if (elm.Name == "SIGORTALI_ADI_SOYADI" || elm.Name == "SIGORTALI_AD_SOYAD")
                                    //{
                                    //    police.GenelBilgiler.PoliceSigortali.AdiUnvan = !String.IsNullOrEmpty(elm["SIGORTALI_ADI_SOYADI"].InnerText) ? elm["SIGORTALI_AD_SOYAD"].InnerText : "";
                                    //}
                                    police.GenelBilgiler.PoliceSigortali.KimlikNo = elm["TC_KIMLIK_NO"].InnerText;
                                    police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = elm["VERGI_NO"].InnerText;

                                }
                                #endregion

                                #region Odeme Plani

                                XmlNode odemeList = polNode["ODEME_LIST"];
                                XmlNodeList odemes = odemeList.ChildNodes;

                                for (int index = 0; index < odemes.Count; index++)
                                { XmlNode elm = odemes.Item(index);
                                    PoliceOdemePlani odm = new PoliceOdemePlani();
                                    //if (elm.Name=="ODEME")
                                    //{
                                         odm.TaksitNo = index + 1;
                                    odm.OdemeTipi = 2;  // Kredi karti !!!!!!
                                    odm.VadeTarihi = Util.toDate(elm["VADESI"].InnerText, Util.DateFormat2);
                                    odm.TaksitTutari = Util.ToDecimal(elm["TUTAR"].InnerText);

                                    if (odm.TaksitTutari != 0)
                                    {
                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                    }

                                    //}

                                }

                                #endregion

                                #region Vergiler

                                //XmlNode vergiList = polNode["VERGI_LIST"];
                                //XmlNodeList vergis = vergiList.ChildNodes;
                                
                                //for (int index = 0; index < vergis.Count; index++)
                                //{
                                //    XmlNode elm = vergis.Item(index);
                                //    if (elm["VERGI_KODU"].InnerText == "1")
                                //    {
                                //        PoliceVergi yv = new PoliceVergi();
                                //        yv.VergiKodu = 4;
                                //        yv.VergiTutari = Util.ToDecimal(elm["TUTARI"].InnerText);
                                //        //if (elm.Name == "TUTAR" || elm.Name == "TUTARI")
                                //        //{
                                //        //    yv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText) == null ? Util.ToDecimal(elm["TUTARI"].InnerText) : 0;
                                //        //}
                                //        police.GenelBilgiler.PoliceVergis.Add(yv);
                                //        continue;
                                //    }


                                //    if (elm["VERGI_KODU"].InnerText == "2")
                                //    {
                                //        PoliceVergi gv = new PoliceVergi();
                                //        gv.VergiKodu = 2;
                                //        gv.VergiTutari = Util.ToDecimal(elm["TUTARI"].InnerText);

                                //        //if (elm.Name == "TUTAR" || elm.Name == "TUTARI")
                                //        //{
                                //        //    gv.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText)==null ? Util.ToDecimal(elm["TUTARI"].InnerText): 0;
                                //        //}

                                //        police.GenelBilgiler.PoliceVergis.Add(gv);
                                //        continue;
                                //    }

                                //    if (elm["VERGI_KODU"].InnerText == "3")
                                //    {
                                //        PoliceVergi thgf = new PoliceVergi();
                                //        thgf.VergiKodu = 1;
                                //        thgf.VergiTutari = Util.ToDecimal(elm["TUTARI"].InnerText);
                                //        //if (elm.Name == "TUTAR" || elm.Name == "TUTARI")
                                //        //{
                                //        //    thgf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText) == null ? Util.ToDecimal(elm["TUTARI"].InnerText) : 0;
                                //        //}
                                //        police.GenelBilgiler.PoliceVergis.Add(thgf);
                                //        continue;
                                //    }

                                //    if (elm["VERGI_KODU"].InnerText == "4")
                                //    {
                                //        PoliceVergi gf = new PoliceVergi();
                                //        gf.VergiKodu = 3;
                                //        gf.VergiTutari = Util.ToDecimal(elm["TUTARI"].InnerText);
                                //        //if (elm.Name == "TUTAR" || elm.Name == "TUTARI")
                                //        //{
                                //        //    gf.VergiTutari = Util.ToDecimal(elm["TUTAR"].InnerText) == null ? Util.ToDecimal(elm["TUTARI"].InnerText) : 0;
                                //        //}
                                //        police.GenelBilgiler.PoliceVergis.Add(gf);
                                //        continue;
                                //    }


                                //}



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
                                polGenel = _PoliceService.getTahsilatPolice(SigortaSirketiBirlikKodlari.ALLIANZHAYATVEEMEKLILIK, policeNumarasi, ekNo.Value);

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
                                            odemeType = true;
                                        }
                                        else if (polNode.InnerText == "NAK")
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                            tahsilat.OdemeBelgeNo = "111111";
                                            odemeType = true;
                                        }
                                        else
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            tahsilat.OdemeBelgeNo = "111111";
                                            odemeType = true;
                                        }
                                    }

                                    #region Tahsilat işlemi
                                    if (odemeType)
                                    {
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
                                            else
                                            {
                                                varOlanKayitlar++;
                                            }
                                            break;
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
    }
}
