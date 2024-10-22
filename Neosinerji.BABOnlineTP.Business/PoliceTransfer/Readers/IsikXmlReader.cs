using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class IsikXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public IsikXmlReader()
        {
        }

        public IsikXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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

            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();

                // read the xml file as a string and put the xml code read from file in a variable of type string
                //string xml = System.IO.File.ReadAllText(filePath, System.Text.Encoding.Default);
                //string pattern = "[&]+";
                //string replacement = string.Empty;
                //Regex rgx = new Regex(pattern);
                //xml = rgx.Replace(xml, replacement);
                doc = new XmlDocument();
                doc.Load(filePath);
                decimal topVergi = 0;

                #region Ürünler

                XmlNode root = doc.LastChild;
                XmlNodeList urunler = root.SelectNodes("ÜRÜNLER/ÜRÜN");

                Dictionary<string, string> uruns = new Dictionary<string, string>();
                foreach (XmlNode urun in urunler)
                {
                    XmlNodeList urunNode = urun.ChildNodes;
                    for (int urnindx = 0; urnindx < urunNode.Count; urnindx++)
                    {
                        XmlNode urunKodu = urunNode[0];
                        XmlNode urunAdi = urunNode[2];
                        uruns[urunKodu.InnerText] = urunAdi.InnerText;
                    }
                }
                #endregion

                XmlNode s = root.LastChild;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        if (s.ChildNodes[i].Name == "POLİÇE")
                        {
                            XmlNode polinode = s.ChildNodes[i];
                            XmlNodeList polNode = polinode.ChildNodes;
                            topVergi = 0;
                            Police police = new Police();

                            #region Genel Bilgiler

                            if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                            else police.GenelBilgiler.TVMKodu = 0;

                            for (int gnlbidx = 0; gnlbidx < polNode.Count; gnlbidx++)
                            {
                                XmlNode gnlnode = polNode[gnlbidx];

                                if (gnlnode.Name == "ÜrünKodu")
                                {
                                    police.GenelBilgiler.UrunKodu = Util.toInt(gnlnode.InnerText);
                                    foreach (var item in uruns)
                                    {
                                        if (gnlnode.InnerText == item.Key)
                                        {
                                            tumUrunAdi = item.Value;
                                        }
                                    }
                                }

                                if (gnlnode.Name == "PoliceNumarası") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                                if (gnlnode.Name == "ZeylNo") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                if (gnlnode.Name == "YenilemeNumarası") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                                if (gnlnode.Name == "TanzimTarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                if (gnlnode.Name == "BaşlangıçTarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                if (gnlnode.Name == "BitişTarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                if (gnlnode.Name == "Plaka" && !String.IsNullOrEmpty(gnlnode.InnerText))
                                {
                                    string plakaNo = gnlnode.InnerText;
                                    if (!String.IsNullOrEmpty(plakaNo))
                                    {
                                        police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                        police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                        for (int j = 0; j < plakaNo.Length; j++)
                                        {
                                            if (j < 2)
                                            {
                                                police.GenelBilgiler.PoliceArac.PlakaKodu += plakaNo[j].ToString();
                                            }
                                            if (j > 2)
                                            {
                                                police.GenelBilgiler.PoliceArac.PlakaNo += plakaNo[j].ToString();
                                            }
                                        }
                                    }
                                }
                                #region Vergiler
                                decimal tahBrut = 0;
                                if (gnlnode.Name == "PRİMVEVERGİLER")
                                {
                                    topVergi = 0;
                                    XmlNodeList vergiler = gnlnode.ChildNodes;
                                    for (int dovidx = 0; dovidx < vergiler.Count; dovidx++)
                                    {
                                        XmlNode node = vergiler[dovidx];

                                        if (node.Name == "NetPrim") police.GenelBilgiler.NetPrim = Util.ToDecimal(node.InnerText);
                                        if (node.Name == "BrütPrim") police.GenelBilgiler.BrutPrim = Util.ToDecimal(node.InnerText);
                                        //tahBrut = police.GenelBilgiler.BrutPrim.Value;
                                        if (node.Name == "AcenteKomisyonu") police.GenelBilgiler.Komisyon = Util.ToDecimal(node.InnerText);
                                        if (node.Name == "BSMV")
                                        {
                                            PoliceVergi gv = new PoliceVergi();
                                            gv.VergiKodu = 2;
                                            gv.VergiTutari = Util.ToDecimal(node.InnerText);
                                            topVergi += Util.ToDecimal(node.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(gv);
                                        }

                                        if (node.Name == "YSV")
                                        {
                                            PoliceVergi yv = new PoliceVergi();
                                            yv.VergiKodu = 4;
                                            yv.VergiTutari = Util.ToDecimal(node.InnerText);
                                            topVergi += Util.ToDecimal(node.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(yv);
                                        }

                                        if (node.Name == "GARANTIFONU")
                                        {
                                            PoliceVergi gf = new PoliceVergi();
                                            gf.VergiKodu = 3;
                                            gf.VergiTutari = Util.ToDecimal(node.InnerText);
                                            topVergi += Util.ToDecimal(node.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(gf);
                                        }

                                        if (node.Name == "T.H.G.FONU")
                                        {
                                            PoliceVergi thgf = new PoliceVergi();
                                            thgf.VergiKodu = 1;
                                            thgf.VergiTutari = Util.ToDecimal(node.InnerText);
                                            topVergi += Util.ToDecimal(node.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(thgf);
                                        }

                                        police.GenelBilgiler.ToplamVergi = topVergi;
                                    }
                                }
                                #endregion

                                if (gnlnode.Name == "DÖVİZ")
                                {
                                    XmlNodeList doviznodes = gnlnode.ChildNodes;
                                    for (int dovidx = 0; dovidx < doviznodes.Count; dovidx++)
                                    {
                                        XmlNode node = doviznodes[dovidx];
                                        if (node.Name == "DövizCinsi") police.GenelBilgiler.ParaBirimi = node.InnerText;
                                        if (node.Name == "DövizKuru") police.GenelBilgiler.DovizKuru = Util.ToDecimal(node.InnerText.Replace(".", ","));
                                        if (police.GenelBilgiler.DovizKuru != null && police.GenelBilgiler.DovizKuru != 0)
                                        {
                                            police.GenelBilgiler.NetPrim =  Math.Round(police.GenelBilgiler.NetPrim.Value  * police.GenelBilgiler.DovizKuru.Value,2);
                                            police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * police.GenelBilgiler.DovizKuru.Value,2);
                                            police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * police.GenelBilgiler.DovizKuru.Value,2);
                                        }

                                    }
                                }

                                #region Sigorta Ettiren

                                if (gnlnode.Name == "MÜŞTERİ")
                                {
                                    XmlNodeList musteri = gnlnode.ChildNodes;
                                    for (int dovidx = 0; dovidx < musteri.Count; dovidx++)
                                    {
                                        XmlNode node = musteri[dovidx];

                                        if (node.Name == "AdıSoyadı-Ünvanı") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = node.InnerText;
                                        if (node.Name == "DoğumTarihi") police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = Util.toDate(node.InnerText, Util.DateFormat2);
                                        if (node.Name == "İşTelefonu1") police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = node.InnerText;
                                        if (node.Name == "CepNumarası") police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo = node.InnerText;
                                        if (node.Name == "VergiKimlikNumarası") police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = node.InnerText;
                                        if (node.Name == "TCKimlikNumarası") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = node.InnerText;
                                        if (node.Name == "İlçe") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = node.InnerText;
                                        if (node.Name == "İl") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = node.InnerText;
                                        if (node.Name == "AdıSoyadı-Ünvanı") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = node.InnerText;
                                        if (node.Name == "Adres1") police.GenelBilgiler.PoliceSigortaEttiren.Adres += node.InnerText;
                                        if (node.Name == "Adres2") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + node.InnerText;
                                        if (node.Name == "Adres3") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + node.InnerText;
                                        if (node.Name == "Adres4") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + node.InnerText;
                                        if (node.Name == "Adres5") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + node.InnerText;
                                    }
                                }
                                #endregion

                                #region Sigortali
                                if (gnlnode.Name == "SİGORTALILAR")
                                {
                                    XmlNodeList sigortali = gnlnode.ChildNodes;
                                    for (int idx = 0; idx < sigortali.Count; idx++)
                                    {
                                        XmlNode sgrtllar = sigortali[idx];
                                        XmlNodeList sigortliNode = sgrtllar.ChildNodes;
                                        for (int index = 0; index < sigortliNode.Count; index++)
                                        {
                                            XmlNode sigortaliNode = sigortliNode[idx];
                                            if (sigortaliNode.Name == "AdıSoyadı-Ünvanı") police.GenelBilgiler.PoliceSigortali.AdiUnvan = sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "DoğumTarihi") police.GenelBilgiler.PoliceSigortali.DogumTarihi = Util.toDate(sigortaliNode.InnerText, Util.DateFormat2);
                                            if (sigortaliNode.Name == "İşTelefonu1") police.GenelBilgiler.PoliceSigortali.TelefonNo = sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "CepNumarası") police.GenelBilgiler.PoliceSigortali.MobilTelefonNo = sigortaliNode.InnerText;

                                            if (sigortaliNode.Name == "VergiKimlikNumarası") police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "TCKimlikNumarası") police.GenelBilgiler.PoliceSigortali.KimlikNo = sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "İlçe") police.GenelBilgiler.PoliceSigortali.IlceAdi = sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "İl") police.GenelBilgiler.PoliceSigortali.IlAdi = sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "AdıSoyadı-Ünvanı") police.GenelBilgiler.PoliceSigortali.AdiUnvan = sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "Adres1") police.GenelBilgiler.PoliceSigortali.Adres += sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "Adres2") police.GenelBilgiler.PoliceSigortali.Adres += " " + sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "Adres3") police.GenelBilgiler.PoliceSigortali.Adres += " " + sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "Adres4") police.GenelBilgiler.PoliceSigortali.Adres += " " + sigortaliNode.InnerText;
                                            if (sigortaliNode.Name == "Adres5") police.GenelBilgiler.PoliceSigortali.Adres += " " + sigortaliNode.InnerText;
                                            idx++;
                                        }
                                    }
                                }
                                #endregion

                                #region Ödeme Plani
                                PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                                police.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                police.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                                if (gnlnode.Name == "TAKSİTLER")
                                {
                                    XmlNodeList taksitlerrnode = gnlnode.ChildNodes;
                                    for (int idx = 0; idx < taksitlerrnode.Count; idx++)
                                    {
                                        PoliceOdemePlani odm = new PoliceOdemePlani();

                                        XmlNode odemenodep = taksitlerrnode[idx];
                                        odm.VadeTarihi = Util.toDate(odemenodep["VadeTarihi"].InnerText, Util.DateFormat2);
                                        odm.TaksitTutari = Util.ToDecimal(odemenodep["Tutar"].InnerText);

                                        odm.TaksitNo = idx + 1;

                                        if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                                        {
                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                        }
                                        else
                                        {
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                        }

                                        if (odm.TaksitTutari != 0)
                                        {
                                            police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                        }
                                        else if (odm.TaksitTutari == 0  && odm.TaksitNo == 1)
                                        {
                                            odm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                            if (odm.VadeTarihi == null)
                                            {
                                                odm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                            }
                                           
                                                odm.TaksitNo = 1;
                                            if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                                            {
                                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                            }
                                            else
                                            {
                                                odm.OdemeTipi = OdemeTipleri.Havale;
                                            }
                                            police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                        }
                                        #region Tahsilat işlemi
                                        var getDetay = _TVMService.GetDetay(tvmkodu);
                                        if (getDetay != null)
                                        {
                                            if (getDetay.MuhasebeEntegrasyon.HasValue)
                                            {
                                                if (!getDetay.MuhasebeEntegrasyon.Value)
                                                {
                                                    
                                                        if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
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
                                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                                            tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                            //   tahsilat.BrutPrim = tahBrut;

                                                            tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                            tahsilat.KayitTarihi = DateTime.Today;
                                                            tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                            tahsilat.TahsilatId = odm.PoliceId;
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
                                                            // tahsilat.OdemeBelgeNo = "111111";
                                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                            tahsilat.OdenenTutar = 0;
                                                            tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                            tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                            tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                                            tahsilat.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                            tahsilat.PoliceId = police.GenelBilgiler.PoliceId;
                                                            tahsilat.KayitTarihi = DateTime.Today;
                                                            tahsilat.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                                            tahsilat.TahsilatId = odm.PoliceId;
                                                            if (tahsilat.TaksitTutari != 0)
                                                            {
                                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                            }
                                                        }
                                                    
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                            police.GenelBilgiler.Durum = 0;
                            police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.BEREKET;


                            //  Odeme Sekli
                            if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                            if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                            if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;

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
                            policeler.Add(police);
                            #endregion
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
    }
}

