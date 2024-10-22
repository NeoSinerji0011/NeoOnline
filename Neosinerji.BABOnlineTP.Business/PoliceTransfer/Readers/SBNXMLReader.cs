using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class SBNXMLReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        int dovizKuru = 0;
        public SBNXMLReader()
        { }

        public SBNXMLReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;

        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            decimal? polBrutprimim = null;
            decimal? polKomisyon = null;
            decimal? polNet = null;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;
                int carpan = 1;
                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        string odemeAraci = null;

                        Police police = new Police();
                        XmlNode polNode = s.ChildNodes[i];
                        XmlNodeList polNodeList = polNode.ChildNodes;

                        #region Genel Bilgiler

                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        tumUrunKodu = polNode["UrunKodu"].InnerText;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.SBNSIGORTA;
                        police.GenelBilgiler.PoliceNumarasi = polNode["PoliceNo"].InnerText;
                        police.GenelBilgiler.EkNo = Util.toInt(polNode["ZeyilNo"].InnerText);
                        police.GenelBilgiler.YenilemeNo = Util.toInt(polNode["TecditNo"].InnerText);
                        police.GenelBilgiler.TanzimTarihi = Util.toDate(polNode["TanzimTarihi"].InnerText);
                        police.GenelBilgiler.BaslangicTarihi = Util.toDate(polNode["BaslangicTarihi"].InnerText);
                        police.GenelBilgiler.BitisTarihi = Util.toDate(polNode["BitisTarihi"].InnerText);

                        // Product - Iptal/Iade drumuna gore -1 ile carpilacaklar net ve brut primler !!!!!!
                        string SBNKayitTipi = polNode["KayitTipi"].InnerText;
                        if (SBNKayitTipi.Trim() == "Iptal")
                        {
                            carpan = -1;
                            //police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * carpan;
                            //police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * carpan;
                            //police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * carpan;
                            //police.GenelBilgiler.ToplamVergi = police.GenelBilgiler.ToplamVergi * carpan;
                            //foreach (var item in police.GenelBilgiler.PoliceVergis)
                            //{
                            //    item.VergiTutari = item.VergiTutari * carpan;
                            //}
                            //foreach (var item in police.GenelBilgiler.PoliceOdemePlanis)
                            //{
                            //    item.TaksitTutari = item.TaksitTutari * carpan;
                            //}
                        }
                        else
                        {
                            carpan = 1;

                        }
                        police.GenelBilgiler.BrutPrim = Util.ToDecimal(polNode["BrutPrim"].InnerText) * carpan;
                        police.GenelBilgiler.NetPrim = Util.ToDecimal(polNode["NetPrim"].InnerText) * carpan;
                        police.GenelBilgiler.Komisyon = Util.ToDecimal(polNode["Komisyon"].InnerText) * carpan;
                        police.GenelBilgiler.ParaBirimi = polNode["DovizTipi"].InnerText;
                        polBrutprimim = police.GenelBilgiler.BrutPrim;
                        polNet = police.GenelBilgiler.NetPrim;
                        polKomisyon = police.GenelBilgiler.Komisyon;
                        if (!String.IsNullOrEmpty(polNode["Kur"].InnerText))
                        {
                            police.GenelBilgiler.DovizKur = Convert.ToDecimal(polNode["Kur"].InnerText.Replace(".", ","));
                            //police.GenelBilgiler.DovizKur = Convert.ToInt32(polNode["Kur"].InnerText.Replace(".", ","));
                        }

                        if (!String.IsNullOrEmpty(police.GenelBilgiler.ParaBirimi))
                        {
                            if (police.GenelBilgiler.ParaBirimi != "TL")
                            {
                               // dovizKuru = Convert.ToDecimal(polNode["Kur"].InnerText.Replace("."))
                                dovizKuru = Convert.ToInt32(polNode["Kur"].InnerText.Replace(".", ","));

                                if (dovizKuru > 1)
                                {
                                    police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                    police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                                    police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                                    dovizKuru = 0;
                                }
                            }
                        }
                        if (police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != null)
                        {
                            police.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                            police.GenelBilgiler.DovizliNetPrim = polNet.Value;
                            police.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
                        }

                        //police.GenelBilgiler.TaliAcenteKodu = polNode["TaliKod"].InnerText;
                        police.GenelBilgiler.Durum = 0;

                        //Ödeme Şeklinden Ödeme Tipi Belirleniyor.
                        byte SBNOdemeTipi = 0;
                        for (int indx = 0; indx < polNodeList.Count; indx++)
                        {
                            XmlNode policeNode = polNodeList[indx];
                            if (policeNode.Name == "OdemeSekli")
                            {
                                odemeAraci= polNode["OdemeSekli"].InnerText;
                                // Odeme Sekli
                                if (polNode["OdemeSekli"].InnerText == "N")
                                {
                                    SBNOdemeTipi = OdemeTipleri.Nakit; //Nakit ödeme
                                }
                                else if (polNode["OdemeSekli"].InnerText == "K")
                                {
                                    SBNOdemeTipi = OdemeTipleri.KrediKarti; //Kredi Kartı Peşin ödeme
                                }
                                else if (polNode["OdemeSekli"].InnerText == "A")
                                {
                                    SBNOdemeTipi = OdemeTipleri.KrediKarti; //Anlaşmalı banka taksitli kk ödeme
                                }
                                else if (polNode["OdemeSekli"].InnerText == "M")
                                {
                                    SBNOdemeTipi = OdemeTipleri.KrediKarti; // taksitli mail kk ödeme
                                }
                                else if (polNode["OdemeSekli"].InnerText == "I")
                                {
                                    SBNOdemeTipi = OdemeTipleri.Yok; // taksitli mail kk ödeme
                                }
                                else
                                {
                                    SBNOdemeTipi = OdemeTipleri.Havale; //berlirsiz ödeme
                                }
                            }
                        }
                        #endregion

                        #region Sigorta Ettiren Bilgileri
                    
                        for (int indx = 0; indx < polNodeList.Count; indx++)
                        {
                            XmlNode policeNode = polNodeList[indx];
                            if (policeNode.Name == "SigortaEttirenAdi")
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = policeNode.InnerText;
                            }
                            if (policeNode.Name == "SigortaEttirenSoyadi")
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = policeNode.InnerText;
                            }
                            if (policeNode.Name == "SigortaEttirenTcKimlikNo")
                                police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = policeNode.InnerText;
                            if (policeNode.Name == "SigortaEttirenVergiNo")
                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = policeNode.InnerText;
                            sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                        }
                        #endregion

                        #region Sigortalı Bilgileri
                       

                        for (int indx = 0; indx < polNodeList.Count; indx++)
                        {
                            XmlNode policeNode = polNodeList[indx];
                            if (policeNode.Name == "SigortaliAdi")
                            {
                                police.GenelBilgiler.PoliceSigortali.AdiUnvan = policeNode.InnerText;
                            }
                            if (policeNode.Name == "SigortaliSoyadi")
                            {
                                police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = policeNode.InnerText;
                            }
                            if (policeNode.Name == "SigortaliAdres")
                            {
                                police.GenelBilgiler.PoliceSigortali.Adres = policeNode.InnerText;
                            }
                            if (policeNode.Name == "SigortaliIl")
                            {
                                police.GenelBilgiler.PoliceSigortali.IlAdi = policeNode.InnerText;
                            }
                            if (policeNode.Name == "SigortaliIlce")
                            {
                                police.GenelBilgiler.PoliceSigortali.IlceAdi = policeNode.InnerText;
                            }
                            if (policeNode.Name == "SigortaliTcKimlikNo")
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = policeNode.InnerText;
                            if (policeNode.Name == "SigortaliVergiNo")
                                police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = policeNode.InnerText;
                            sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;

                        }

                        #endregion
                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;

                        #region Araç Bilgileri
                        //if (polNode.Name == "Arac")
                        //{
                        XmlNode aracs = polNode["Arac"];
                        if (aracs!=null)
                        {
                            XmlNodeList arac = aracs.ChildNodes;

                            for (int j = 0; j < arac.Count; j++)
                            {
                                XmlNode elm = arac.Item(j);

                                if (elm.Name == "KullanimTarzi")
                                {
                                    police.GenelBilgiler.PoliceArac.KullanimTarzi = elm.InnerText;
                                }

                                if (elm.Name == "Plaka")
                                {
                                    police.GenelBilgiler.PoliceArac.PlakaNo = elm.InnerText != "" && elm.InnerText.Length >= 3 ? elm.InnerText.Substring(3, elm.InnerText.Length - 3) : "";
                                    police.GenelBilgiler.PoliceArac.PlakaKodu = elm.InnerText != "" && elm.InnerText.Length >= 3 ? elm.InnerText.Substring(0, 3) : "";
                                }

                                // if (elm.Name == "Plaka") 
                                // {
                                //     police.GenelBilgiler.PoliceArac.PlakaKodu = "";          
                                //     police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                //     for (int z = 0; z < elm.InnerText.Length; z++)
                                //     {
                                //         if (z <= 2)
                                //        {
                                //             police.GenelBilgiler.PoliceArac.PlakaKodu += elm.InnerText[z].ToString();
                                //         }
                                //         if (z > 2)
                                //         {
                                //             police.GenelBilgiler.PoliceArac.PlakaNo += elm.InnerText[z].ToString();
                                //         }
                                //     }
                                //  police.GenelBilgiler.PoliceArac.PlakaNo = elm.InnerText;
                                // }

                                if (elm.Name == "MotorNo")
                                {
                                    police.GenelBilgiler.PoliceArac.MotorNo = elm.InnerText;
                                }
                                if (elm.Name == "SasiNo")
                                {
                                    police.GenelBilgiler.PoliceArac.SasiNo = elm.InnerText;
                                }
                                if (elm.Name == "Markakodu")
                                {
                                    police.GenelBilgiler.PoliceArac.Marka = elm.InnerText;
                                }
                                if (elm.Name == "Model")
                                {
                                    police.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(elm.InnerText);
                                }
                                if (elm.Name == "Tip")
                                {
                                    police.GenelBilgiler.PoliceArac.AracinTipiAciklama = elm.InnerText;
                                }
                                if (elm.Name == "TipKodu")
                                {
                                    police.GenelBilgiler.PoliceArac.AracinTipiKodu = elm.InnerText;
                                }
                            }
                        }                   


                        //}

                        #endregion

                        #region Odeme Plani

                        XmlNode tkSbn = polNode["Taksitler"];
                        XmlNodeList tksSbn = tkSbn.ChildNodes;
                        PoliceOdemePlani odm = new PoliceOdemePlani();


                        for (int indx = 0; indx < tksSbn.Count; indx++)
                        {
                            XmlNode elm = tksSbn.Item(indx);
                            if (tksSbn.Count == 1)
                                police.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;//Peşin
                            else
                                police.GenelBilgiler.OdemeSekli = OdemeSekilleri.Vadeli;//Vadeli

                            odm.TaksitNo = indx + 1;
                            odm.OdemeTipi = SBNOdemeTipi;
                            odm.VadeTarihi = Util.toDate(elm["Tarih"].InnerText);
                            odm.TaksitTutari = Util.ToDecimal(elm["MusteriTutar"].InnerText) * carpan;
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["MusteriTutar"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2) * carpan;
                                odm.DovizliTaksitTutari = Util.ToDecimal(elm["MusteriTutar"].InnerText) * carpan;
                            }


                            if (odm.TaksitTutari != 0)
                            {
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            #region Tahsilat işlemi

                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.SBNSIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (odm.TaksitTutari != 0)
                                {
                               
                                    //if (polNode.Name== "OdemeSekli")
                                    //{
                                    //    if (polNode["OdemeSekli"].InnerText != null)
                                    //    {
                                            string OdemeTiplerim = odemeAraci;


                                            //if (OdemeTiplerim != null)
                                            //{

                                                if (OdemeTiplerim == "A" || OdemeTiplerim == "K" || OdemeTiplerim == "M")
                                                {
                                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilat.TaksitNo = odm.TaksitNo;
                                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                    tahsilat.OdemeBelgeNo = "111111****1111";
                                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.KalanTaksitTutari = 0;
                                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilat.KayitTarihi = DateTime.Today;
                                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                    if (tahsilat.TaksitTutari != 0)
                                                    {
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                    }
                                                }
                                                if (OdemeTiplerim == "KK")
                                                {
                                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilat.TaksitNo = odm.TaksitNo;
                                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                    tahsilat.OdemeBelgeNo = "111111****1111";
                                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.KalanTaksitTutari = 0;
                                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilat.KayitTarihi = DateTime.Today;
                                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                    if (tahsilat.TaksitTutari != 0)
                                                    {
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                    }
                                                }
                                                else if (OdemeTiplerim == "N")
                                                {
                                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                    tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                    odm.OdemeTipi = OdemeTipleri.Nakit;
                                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilat.TaksitNo = odm.TaksitNo;
                                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                    //tahsilat.OdemeBelgeNo = "1111111";
                                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.OdenenTutar = 0;
                                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilat.KayitTarihi = DateTime.Today;
                                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                    if (tahsilat.TaksitTutari != 0)
                                                    {
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                    }
                                                }
                                                else
                                                {
                                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                    tahsilat.OdemTipi = OdemeTipleri.Havale;
                                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilat.TaksitNo = odm.TaksitNo;
                                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                    //tahsilat.OdemeBelgeNo = "1111111";
                                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.OdenenTutar = 0;
                                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilat.KayitTarihi = DateTime.Today;
                                                    tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                    if (tahsilat.TaksitTutari != 0)
                                                    {
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                    }
                                                }

                                            //}
                                    //    }

                                    //}
                                }
                            }

                            #endregion
                        }
                        if (tksSbn.Count == 0)
                        {

                            PoliceOdemePlani odmm = new PoliceOdemePlani();

                            if (odmm.TaksitTutari == null)
                            {
                                odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                {
                                    odm.DovizliTaksitTutari = police.GenelBilgiler.BrutPrim.Value;
                                }
                                if (odmm.VadeTarihi == null)
                                {
                                    odmm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                }
                                odmm.OdemeTipi = OdemeTipleri.Havale;
                                odmm.TaksitNo = 1;
                                if (police.GenelBilgiler.BrutPrim != null && police.GenelBilgiler.BrutPrim != 0)
                                {
                                    police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.SBNSIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                                if(tahsilat.OdemTipi ==1)
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
                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odmm.OdemeTipi = OdemeTipleri.Havale;
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
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }
                            }

                        }
                        #endregion

                        #region Vergiler

                        decimal topVergi = 0;
                        //Trafik hizmetleri gelilstirme fonu- THGF
                        PoliceVergi thgf = new PoliceVergi();
                        thgf.VergiKodu = 1;
                        thgf.VergiTutari = carpan * Util.ToDecimal(polNode["TrafikGelistirmeFonu"].InnerText);
                        topVergi += Util.ToDecimal(polNode["TrafikGelistirmeFonu"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(thgf);

                        //GiderVergisi - BSMV
                        PoliceVergi gv = new PoliceVergi();
                        gv.VergiKodu = 2;
                        gv.VergiTutari = carpan * Util.ToDecimal(polNode["GiderVergisi"].InnerText);
                        topVergi += Util.ToDecimal(polNode["GiderVergisi"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(gv);

                        //Garanti Fonu- GF
                        PoliceVergi gf = new PoliceVergi();
                        gf.VergiKodu = 3;
                        gf.VergiTutari = carpan * Util.ToDecimal(polNode["GarantiFonu"].InnerText);
                        topVergi += Util.ToDecimal(polNode["GarantiFonu"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(gf);

                        //YSV
                        PoliceVergi ysv = new PoliceVergi();
                        ysv.VergiKodu = 4;
                        ysv.VergiTutari = carpan * Util.ToDecimal(polNode["YSV"].InnerText);
                        topVergi += Util.ToDecimal(polNode["YSV"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(ysv);
                        police.GenelBilgiler.ToplamVergi = carpan * topVergi;



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
                        policeler.Add(police);

                        #endregion


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

    }
}
