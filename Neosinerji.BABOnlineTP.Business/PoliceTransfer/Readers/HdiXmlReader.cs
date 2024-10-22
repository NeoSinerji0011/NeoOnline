using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using static Neosinerji.BABOnlineTP.Business.PoliceTransfer.SFSExcelOrient;


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class HdiXMLReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullanici;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;

        public HdiXMLReader()
        {
        }

        public HdiXMLReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;
            int carpan = 1;

            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = null; string resTahsilatKapatmaVarmi = "";
            string[] tempPath = filePath.Split('#');
            if (tempPath.Length > 1)
            {
                policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                filePath = filePath.Substring(0, filePath.IndexOf("#"));

            }

            decimal dovizKuru = 1;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            decimal? polKomisyon = null;
            decimal? polNet = null;
            decimal? polBrutprimim = null;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;
                bool veriVarmi = true;
                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;
                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        if (s.ChildNodes[i].Name == "ToplamListelenPolice")
                        {
                            XmlNode polNode = s.ChildNodes[i];
                            if (polNode.InnerText == "0")
                            {
                                veriVarmi = false;
                                break;
                            }
                        }
                    }
                    if (veriVarmi)
                    {
                        for (int i = 0; i < s.ChildNodes.Count; i++)
                        {
                            Police police = new Police();
                            if (s.ChildNodes[i].Name == "POLİÇE")
                            {
                                XmlNode polNode = s.ChildNodes[i];

                                #region Genel Bilgiler

                                XmlNodeList gnlb = polNode.ChildNodes;
                                for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                                {
                                    XmlNode gnlnode = gnlb[gnlbidx];

                                    if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                    else police.GenelBilgiler.TVMKodu = 0;

                                    if (gnlnode.Name == "UrunKodu") tumUrunKodu = gnlnode.InnerText;
                                    if (gnlnode.Name == "PoliceNumarası") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                                    if (gnlnode.Name == "ZeylNo") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                    if (gnlnode.Name == "YenilemeNumarası") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                                    if (gnlnode.Name == "TanzimTarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                    if (gnlnode.Name == "BaşlangıçTarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                    if (gnlnode.Name == "BitişTarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);

                                    if (gnlnode.Name == "iptal")
                                    {
                                        //if (gnlnode.InnerText == "E")
                                        //{
                                        //    carpan = -1;
                                        //    //police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * carpan;
                                        //    //police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * carpan;
                                        //    //police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * carpan;
                                        //    //police.GenelBilgiler.ToplamVergi = police.GenelBilgiler.ToplamVergi * carpan;
                                        //    //foreach (var item in police.GenelBilgiler.PoliceVergis)
                                        //    //{
                                        //    //    item.VergiTutari = item.VergiTutari * carpan;
                                        //    //}
                                        //    //foreach (var item in police.GenelBilgiler.PoliceOdemePlanis)
                                        //    //{
                                        //    //    item.TaksitTutari = item.TaksitTutari * carpan;
                                        //    //}
                                        //}
                                        //else
                                        //{
                                        //    carpan = 1;
                                        //}
                                    }

                                    #region Vergiler

                                    // brut prim, net prim
                                    decimal topVergi = 0;
                                    if (gnlnode.Name == "PRİMVEVERGİLER")
                                    {
                                        XmlNodeList primVergilernode = gnlnode.ChildNodes;

                                        for (int idx = 0; idx < primVergilernode.Count; idx++)
                                        {
                                            XmlNode vergi = primVergilernode[idx];
                                            if (vergi.Name == "NetPrim")
                                            {
                                                police.GenelBilgiler.NetPrim = Util.ToDecimal(vergi.InnerText);
                                                polNet = police.GenelBilgiler.NetPrim;
                                            }
                                            if (vergi.Name == "BrütPrim")
                                            {
                                                police.GenelBilgiler.BrutPrim = Util.ToDecimal(vergi.InnerText);
                                                polBrutprimim = police.GenelBilgiler.BrutPrim;
                                            }
                                            if (vergi.Name == "AcenteKomisyonu")
                                            {
                                                police.GenelBilgiler.Komisyon = Util.ToDecimal(vergi.InnerText);
                                                polKomisyon = police.GenelBilgiler.Komisyon;
                                            }

                                            if (vergi.Name == "YangınSigortaVergisi")
                                            {
                                                PoliceVergi yv = new PoliceVergi();
                                                yv.VergiKodu = 4;
                                                yv.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                                topVergi += Util.ToDecimal(vergi.InnerText);
                                                police.GenelBilgiler.PoliceVergis.Add(yv);
                                            }
                                            if (vergi.Name == "GarantiFonu")
                                            {
                                                PoliceVergi gf = new PoliceVergi();
                                                gf.VergiKodu = 3;
                                                gf.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                                topVergi += Util.ToDecimal(vergi.InnerText);
                                                police.GenelBilgiler.PoliceVergis.Add(gf);
                                            }

                                            if (vergi.Name == "GiderVergisi")
                                            {
                                                PoliceVergi gv = new PoliceVergi();
                                                gv.VergiKodu = 2;
                                                gv.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                                topVergi += Util.ToDecimal(vergi.InnerText);
                                                police.GenelBilgiler.PoliceVergis.Add(gv);
                                            }

                                            if (vergi.Name == "TrafikHizmetleriGeliştirmeFonu")
                                            {
                                                PoliceVergi thgf = new PoliceVergi();
                                                thgf.VergiKodu = 1;
                                                thgf.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                                topVergi += Util.ToDecimal(vergi.InnerText);
                                                police.GenelBilgiler.PoliceVergis.Add(thgf);
                                            }
                                        }

                                        police.GenelBilgiler.ToplamVergi = topVergi;
                                    }
                                    // dovizli brut-net kom
                                    if (gnlnode.Name == "DÖVİZ")
                                    {
                                        XmlNodeList primDoviznode = gnlnode.ChildNodes;

                                        for (int idx = 0; idx < primDoviznode.Count; idx++)
                                        {
                                            XmlNode dovizz = primDoviznode[idx];

                                            if (dovizz.Name == "DövizNetPrim")
                                            {
                                                police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(dovizz.InnerText);
                                                polNet = police.GenelBilgiler.DovizliNetPrim;
                                            }
                                            if (dovizz.Name == "DövizBrutPrim")
                                            {
                                                police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(dovizz.InnerText);
                                                polBrutprimim = police.GenelBilgiler.DovizliBrutPrim;
                                            }
                                            if (dovizz.Name == "DövizAcenteKomisyonu")
                                            {
                                                police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(dovizz.InnerText);
                                                polKomisyon = police.GenelBilgiler.DovizliKomisyon;
                                            }
                                        }

                                    }
                                    #endregion

                                    #region Döviz Cinsi

                                    if (gnlnode.Name == "DÖVİZ")
                                    {
                                        XmlNodeList doviznode = gnlnode.ChildNodes;

                                        for (int idx = 0; idx < doviznode.Count; idx++)
                                        {
                                            XmlNode dovizcinsnode = doviznode[idx];
                                            if (dovizcinsnode.Name == "PoliçeTürü") police.GenelBilgiler.ParaBirimi = dovizcinsnode.InnerText;
                                            if (police.GenelBilgiler.ParaBirimi == "TRL")
                                            {
                                                police.GenelBilgiler.ParaBirimi = "TL";
                                            }
                                            if (dovizcinsnode.Name == "DövizKuru")
                                            {
                                                if (!String.IsNullOrEmpty(dovizcinsnode.InnerText))
                                                {
                                                    police.GenelBilgiler.DovizKur = Util.ToDecimal(dovizcinsnode.InnerText.Replace(".", ","));
                                                }

                                            }
                                            if (police.GenelBilgiler.ParaBirimi != "TRL")
                                            {
                                                if (dovizcinsnode.Name == "DövizKuru")
                                                {
                                                    dovizKuru = Util.ToDecimal(dovizcinsnode.InnerText.Replace(".", ","));
                                                    if (dovizKuru != 0 && dovizKuru != 1)
                                                    {
                                                        //police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                                        //police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                                                        //police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);                                                  
                                                        dovizKuru = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    #endregion

                                    #region Sigorta Ettiren Bilgileri

                                    if (gnlnode.Name == "MÜŞTERİ")
                                    {
                                        XmlNodeList sigettirenrnode = gnlnode.ChildNodes;

                                        for (int idx = 0; idx < sigettirenrnode.Count; idx++)
                                        {
                                            XmlNode musterinode = sigettirenrnode[idx];
                                            if (musterinode.Name == "AdıSoyadı-Ünvanı") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = musterinode.InnerText;
                                            if (musterinode.Name == "VergiKimlikNumarası") police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = musterinode.InnerText;
                                            if (musterinode.Name == "TCKimlikNumarası") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = musterinode.InnerText;
                                            if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "0")
                                            {
                                                police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                                            }
                                            if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "0")
                                            {
                                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                                            }
                                            sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                            if (musterinode.Name == "Adres1") police.GenelBilgiler.PoliceSigortaEttiren.Adres += musterinode.InnerText;
                                            if (musterinode.Name == "Adres2") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + musterinode.InnerText;
                                            if (musterinode.Name == "Adres3") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + musterinode.InnerText;
                                            if (musterinode.Name == "Adres4") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + musterinode.InnerText;
                                            if (musterinode.Name == "Adres5") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + musterinode.InnerText;
                                            if (musterinode.Name == "İlçe") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = musterinode.InnerText;
                                            if (musterinode.Name == "İl") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = musterinode.InnerText;
                                        }
                                    }
                                    #endregion

                                    #region Sigortalı Bilgileri

                                    if (gnlnode.Name == "SİGORTALILAR")
                                    {
                                        XmlNodeList sigortalılarnode = gnlnode.ChildNodes;

                                        for (int idx = 0; idx < sigortalılarnode.Count; idx++)
                                        {
                                            XmlNode sigortalınode = sigortalılarnode.Item(idx);
                                            if (sigortalınode.Name == "SİGORTALI")
                                            {
                                                XmlNodeList sigortalı = sigortalınode.ChildNodes;
                                                for (int sigortalıindx = 0; sigortalıindx < sigortalı.Count; sigortalıindx++)
                                                {
                                                    XmlNode nodes = sigortalı[sigortalıindx];
                                                    if (nodes.Name == "AdıSoyadı-Ünvanı") police.GenelBilgiler.PoliceSigortali.AdiUnvan = nodes.InnerText;
                                                    if (nodes.Name == "VergiKimlikNumarası") police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = nodes.InnerText;
                                                    if (nodes.Name == "TCKimlikNumarası") police.GenelBilgiler.PoliceSigortali.KimlikNo = nodes.InnerText;
                                                    if (police.GenelBilgiler.PoliceSigortali.KimlikNo == "0")
                                                    {
                                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                                                    }
                                                    if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "0")
                                                    {
                                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                                                    }
                                                    sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                                    if (nodes.Name == "Adres1") police.GenelBilgiler.PoliceSigortali.Adres += nodes.InnerText;
                                                    if (nodes.Name == "Adres2") police.GenelBilgiler.PoliceSigortali.Adres += " " + nodes.InnerText;
                                                    if (nodes.Name == "Adres3") police.GenelBilgiler.PoliceSigortali.Adres += " " + nodes.InnerText;
                                                    if (nodes.Name == "Adres4") police.GenelBilgiler.PoliceSigortali.Adres += " " + nodes.InnerText;
                                                    if (nodes.Name == "Adres5") police.GenelBilgiler.PoliceSigortali.Adres += " " + nodes.InnerText;
                                                    if (nodes.Name == "İlçe") police.GenelBilgiler.PoliceSigortali.IlceAdi = nodes.InnerText;
                                                    if (nodes.Name == "İl") police.GenelBilgiler.PoliceSigortali.IlAdi = nodes.InnerText;
                                                }
                                            }
                                        }
                                    }

                                    #endregion
                                    PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                                    PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                                    police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                                    police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;

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
                                    #region Ödeme Planı

                                   

                                    if (gnlnode.Name == "TAKSİTLER")
                                    {
                                        XmlNodeList taksitlerrnode = gnlnode.ChildNodes;
                                        int taksitno = 0;



                                        for (int idx = 0; idx < taksitlerrnode.Count; idx++)
                                        {
                                            PoliceOdemePlani odm = new PoliceOdemePlani();



                                            XmlNode odemenode = taksitlerrnode[idx];

                                            if (odemenode.Name == "OdemeTuru")
                                            {
                                                if (odemenode.InnerText == "1" || odemenode.InnerText == "2")
                                                {
                                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                }
                                                else if (odemenode.InnerText == "3")
                                                {
                                                    odm.OdemeTipi = OdemeTipleri.Nakit;
                                                }
                                                else if (odemenode.InnerText == "4")
                                                {
                                                    odm.OdemeTipi = OdemeTipleri.CekSenet;
                                                }
                                                else
                                                {
                                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                                }
                                                idx++;
                                            }

                                            XmlNode odemenodeTsekli = taksitlerrnode[idx];

                                            if (odemenodeTsekli.Name == "TaksitSekli")
                                            {
                                                idx++;
                                            }

                                            XmlNode odemenodep = taksitlerrnode[idx];
                                            if (odemenodep.Name == "TAKSİT")
                                            {
                                                taksitno++;
                                                XmlNodeList taksitnode = odemenodep.ChildNodes;
                                                for (int tskindex = 0; tskindex < taksitnode.Count; tskindex++)
                                                {
                                                    XmlNode taksit = taksitnode[tskindex];

                                                    if (taksit.Name == "VadeTarihi") odm.VadeTarihi = Util.toDate(taksit.InnerText, Util.DateFormat2);
                                                    if (taksit.Name == "Tutar")
                                                    {
                                                        odm.TaksitTutari = Util.ToDecimal(odemenodep["Tutar"].InnerText) * carpan;
                                                        odm.DovizliTaksitTutari = Util.ToDecimal(odemenodep["DovizTutar"].InnerText) * carpan;
                                                    }
                                                }
                                            }
                                            odm.TaksitNo = taksitno;

                                            if (odm.TaksitTutari != 0)
                                            {
                                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                            }
                                            #region Tahsilat işlemi
                                             resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);


                                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.HDISIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                                if (tvmkodu == 139)
                                                {

                                                    if (odemenode.InnerText == "1" || odemenode.InnerText == "2")
                                                    {
                                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                        tahsilat.TaksitNo = odm.TaksitNo;
                                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                        // tahsilat.OdemeBelgeNo = "11111111";
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
                                                        //tahsilat.TahsilatId = odm.PoliceId;
                                                        if (tahsilat.TaksitTutari != 0)
                                                        {
                                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                        }
                                                    }
                                                    else if (odemenode.InnerText == "4")
                                                    {
                                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                        tahsilat.OdemTipi = OdemeTipleri.CekSenet;
                                                        odm.OdemeTipi = OdemeTipleri.CekSenet;
                                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                        tahsilat.TaksitNo = odm.TaksitNo;
                                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                        // tahsilat.OdemeBelgeNo = "11111111";
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
                                                        //tahsilat.TahsilatId = odm.PoliceId;
                                                        if (tahsilat.TaksitTutari != 0)
                                                        {
                                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                        }
                                                    }
                                                    else if (odemenode.InnerText == "3")
                                                    {
                                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                        tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                        odm.OdemeTipi = OdemeTipleri.Nakit;
                                                        tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                        tahsilat.TaksitNo = odm.TaksitNo;
                                                        tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                        //tahsilat.OdemeBelgeNo = "11111111";
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
                                                        //tahsilat.TahsilatId = odm.PoliceId;
                                                        if (tahsilat.TaksitTutari != 0)
                                                        {
                                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
                                                        //tahsilat.OdemeBelgeNo = "11111111";
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
                                                        //tahsilat.TahsilatId = odm.PoliceId;
                                                        if (tahsilat.TaksitTutari != 0)
                                                        {
                                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                        }
                                                    }

                                                }
                                                else
                                                {

                                                    if (odemenode.Name == "OdemeTuru")
                                                    {
                                                        if (odemenode.InnerText == "1" || odemenode.InnerText == "2")
                                                        {
                                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                            tahsilat.OtomatikTahsilatiKkMi = 1;
                                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilat.TaksitNo = odm.TaksitNo;
                                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
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
                                                            //tahsilat.TahsilatId = odm.PoliceId;

                                                            if (tahsilat.TaksitTutari != 0)
                                                            {
                                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                            }
                                                        }
                                                        else if (odemenode.InnerText == "4")
                                                        {
                                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                            tahsilat.OdemTipi = OdemeTipleri.CekSenet;
                                                            odm.OdemeTipi = OdemeTipleri.CekSenet;
                                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilat.TaksitNo = odm.TaksitNo;
                                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            // tahsilat.OdemeBelgeNo = "11111111";
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
                                                            //tahsilat.TahsilatId = odm.PoliceId;

                                                            if (tahsilat.TaksitTutari != 0)
                                                            {
                                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                            }
                                                        }
                                                        else if (odemenode.InnerText == "3")
                                                        {
                                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                            odm.OdemeTipi = OdemeTipleri.Nakit;
                                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilat.TaksitNo = odm.TaksitNo;
                                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            //tahsilat.OdemeBelgeNo = "11111111";
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
                                                            //tahsilat.TahsilatId = odm.PoliceId;
                                                            if (tahsilat.TaksitTutari != 0)
                                                            {
                                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
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
                                                            //tahsilat.OdemeBelgeNo = "11111111";
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
                                                            //tahsilat.TahsilatId = odm.PoliceId;
                                                            if (tahsilat.TaksitTutari != 0)
                                                            {
                                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                            }



                                                        }
                                                    }

                                                }
                                            }

                                            #endregion
                                        }
                                        if (taksitlerrnode.Count == 0)
                                        {

                                            PoliceOdemePlani odmm = new PoliceOdemePlani();

                                            if (odmm.TaksitTutari == null)
                                            {
                                                odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                                if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                {
                                                    odmm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim.Value;
                                                }
                                                if (odmm.VadeTarihi == null)
                                                {
                                                    odmm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                                }
                                                odmm.OdemeTipi = OdemeTipleri.Havale;
                                                odmm.TaksitNo = 1;
                                                if (odmm.TaksitTutari != 0 && odmm.TaksitTutari != null)
                                                {
                                                    police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);

                                                    resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);



                                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.HDISIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                                        tahsilat.TahsilatId = odmm.PoliceId;
                                                        if (tahsilat.TaksitTutari != 0)
                                                        {
                                                            //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                        }
                                                    }

                                                }
                                            }


                                        }

                                    }
                                    #endregion

                                    #region Poliçe Araç Bilgileri

                                    if (gnlnode.Name == "ARAC")
                                    {
                                        XmlNodeList polaracnode = gnlnode.ChildNodes;

                                        for (int idx = 0; idx < polaracnode.Count; idx++)
                                        {
                                            XmlNode aracnode = polaracnode[idx];
                                            if (aracnode.Name == "Marka") police.GenelBilgiler.PoliceArac.MarkaAciklama = aracnode.InnerText;
                                            if (aracnode.Name == "Model") police.GenelBilgiler.PoliceArac.AracinTipiAciklama = aracnode.InnerText;
                                            if (aracnode.Name == "ModelYili") police.GenelBilgiler.PoliceArac.Model = Util.toInt(aracnode.InnerText);
                                            if (aracnode.Name == "SasiNo") police.GenelBilgiler.PoliceArac.SasiNo = aracnode.InnerText;
                                            if (aracnode.Name == "MotorNo") police.GenelBilgiler.PoliceArac.MotorNo = aracnode.InnerText;
                                            if (aracnode.Name == "TescilNo") police.GenelBilgiler.PoliceArac.TescilSeriNo = aracnode.InnerText;
                                            if (aracnode.Name == "Plaka")
                                            {
                                                police.GenelBilgiler.PoliceArac.PlakaNo = aracnode.InnerText != "" && aracnode.InnerText.Length >= 2 ? aracnode.InnerText.Substring(2, aracnode.InnerText.Length - 2) : "";
                                                police.GenelBilgiler.PoliceArac.PlakaKodu = aracnode.InnerText != "" && aracnode.InnerText.Length >= 2 ? aracnode.InnerText.Substring(0, 2) : "";
                                            }
                                            if (aracnode.Name == "TescilTarihi") police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(aracnode.InnerText, Util.DateFormat0);
                                            if (aracnode.Name == "AracDegerKodu") police.GenelBilgiler.PoliceArac.AracDeger = !String.IsNullOrEmpty(aracnode.InnerText) ? Util.ToDecimal(aracnode.InnerText) : 0;
                                        }
                                    }
                                    #endregion
                                }
                                police.GenelBilgiler.Durum = 0;
                                police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.HDISIGORTA;

                                // Odeme Sekli Belirleniyor...
                                if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                                if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                                if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;

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
            if (neoOnline_TahsilatKapatmas != null)
            {


                foreach (var item in neoOnline_TahsilatKapatmas)
                {
                    if (item.Police_No.Trim() == police.PoliceNumarasi.Trim() && item.Yenileme_No.Trim() == police.YenilemeNo.Value.ToString().Trim() && item.Ek_No.Trim() == police.EkNo.Value.ToString().Trim() && !_TVMService.CheckListTVMBankaCariHesaplari(_AktifKullanici.TVMKodu, 5, item.Kart_No.Trim()))
                    {
                        return item.Kart_No.Trim();
                    }
                }
            }
            else
            {
                foreach (var item in police.PoliceOdemePlanis)
                {
                    if (item.OdemeTipi == 2 || item.OdemeTipi == 1)
                    {
                        return "111111****1111";
                    }
                }

            }
            return "";
        }
    }
}
