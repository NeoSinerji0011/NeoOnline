using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;


namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class ZurichXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public ZurichXmlReader()
        {
        }

        public ZurichXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            int carpan = 1;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        Police pol = new Police();
                        XmlNode polNode = s.ChildNodes[i];
                        carpan = 1;
                        string iptalNode = polNode.SelectSingleNode("POLICE-BILGI/TAH-IPTAL").InnerText;
                        if (iptalNode == "I")
                        {
                            carpan = -1;
                            //pol.GenelBilgiler.BrutPrim = pol.GenelBilgiler.BrutPrim * carpan;
                            //pol.GenelBilgiler.NetPrim = pol.GenelBilgiler.NetPrim * carpan;
                            //pol.GenelBilgiler.Komisyon = pol.GenelBilgiler.Komisyon * carpan;
                            //pol.GenelBilgiler.ToplamVergi = pol.GenelBilgiler.ToplamVergi * carpan;
                            //foreach (var item in pol.GenelBilgiler.PoliceVergis)
                            //{
                            //    item.VergiTutari = item.VergiTutari * carpan;
                            //}
                            //foreach (var item in pol.GenelBilgiler.PoliceOdemePlanis)
                            //{
                            //    item.TaksitTutari = item.TaksitTutari * carpan;
                            //}
                        }

                        #region Genel Bilgileri

                        if (tvmkodu > 0) pol.GenelBilgiler.TVMKodu = tvmkodu;
                        else pol.GenelBilgiler.TVMKodu = 0;

                        decimal topVergi = 0;
                        XmlNodeList gnlb = polNode["POLICE-BILGI"].ChildNodes;
                        for (int polBilgindx = 0; polBilgindx < gnlb.Count; polBilgindx++)
                        {
                            XmlNode polbilgi = gnlb[polBilgindx];

                            if (polbilgi.Name == "TARIFE-AD") tumUrunAdi = polbilgi.InnerText;
                            if (polbilgi.Name == "TARIFE-KOD") tumUrunKodu = polbilgi.InnerText;
                            if (polbilgi.Name == "POLICE-NO") pol.GenelBilgiler.PoliceNumarasi = polbilgi.InnerText;
                            if (polbilgi.Name == "ZEYL-NO") pol.GenelBilgiler.EkNo = Util.toInt(polbilgi.InnerText);
                            if (polbilgi.Name == "TANZIM-TAR") pol.GenelBilgiler.TanzimTarihi = Util.toDate(polbilgi.InnerText, Util.DateFormat1);
                            if (polbilgi.Name == "BASLANGIC-TAR") pol.GenelBilgiler.BaslangicTarihi = Util.toDate(polbilgi.InnerText, Util.DateFormat1);
                            if (polbilgi.Name == "BITIS-TAR") pol.GenelBilgiler.BitisTarihi = Util.toDate(polbilgi.InnerText, Util.DateFormat1);
                            if (polbilgi.Name == "POLICE-NET-PRIM") pol.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(polbilgi.InnerText);
                            if (polbilgi.Name == "POLICE-KOMISYON-TUTAR")
                            {
                                pol.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(polbilgi.InnerText.Trim());
                                if (pol.GenelBilgiler.DovizKur != null && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != 1)
                                {
                                    pol.GenelBilgiler.DovizliKomisyon = carpan * Util.ToDecimal(polbilgi.InnerText.Trim());
                                }
                            }
                            if (polbilgi.Name == "POLICE-YTL-NET-PRIM") pol.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(polbilgi.InnerText) * carpan;
                            #region Vergiler

                            if (polbilgi.Name == "POLICE-GV")
                            {
                                PoliceVergi gv = new PoliceVergi();
                                gv.VergiKodu = 2;
                                gv.VergiTutari = carpan * Util.ToDecimal(polbilgi.InnerText);
                                topVergi += carpan * Util.ToDecimal(polbilgi.InnerText);
                                pol.GenelBilgiler.PoliceVergis.Add(gv);
                            }

                            if (polbilgi.Name == "POLICE-TF")
                            {
                                PoliceVergi tghf = new PoliceVergi();
                                tghf.VergiKodu = 1;
                                tghf.VergiTutari = carpan * Util.ToDecimal(polbilgi.InnerText);
                                topVergi += carpan * Util.ToDecimal(polbilgi.InnerText);
                                pol.GenelBilgiler.PoliceVergis.Add(tghf);
                            }

                            if (polbilgi.Name == "POLICE-GF")
                            {
                                PoliceVergi gf = new PoliceVergi();
                                gf.VergiKodu = 3;
                                gf.VergiTutari = carpan * Util.ToDecimal(polbilgi.InnerText);
                                topVergi += carpan * Util.ToDecimal(polbilgi.InnerText);
                                pol.GenelBilgiler.PoliceVergis.Add(gf);
                            }

                            if (polbilgi.Name == "POLICE-YSV")
                            {
                                PoliceVergi ysv = new PoliceVergi();
                                ysv.VergiKodu = 4;
                                ysv.VergiTutari = carpan * Util.ToDecimal(polbilgi.InnerText);
                                topVergi += carpan * Util.ToDecimal(polbilgi.InnerText);
                                pol.GenelBilgiler.PoliceVergis.Add(ysv);
                            }
                            if (polbilgi.Name == "NAKLIYAT")
                            {
                                XmlNodeList doviznodes = polbilgi.ChildNodes;
                                for (int dovidx = 0; dovidx < doviznodes.Count; dovidx++)
                                {
                                    XmlNode node = doviznodes[dovidx];
                                    if (node.Name == "DOVIZ-CINS") pol.GenelBilgiler.ParaBirimi = node.InnerText;
                                }
                            }
                            pol.GenelBilgiler.ToplamVergi = topVergi;
                            #endregion
                        }
                        #endregion

                        XmlNodeList tmnt = polNode["POLICE-TEMINATLARI"].ChildNodes;
                        for (int polBilgindx = 0; polBilgindx < tmnt.Count; polBilgindx++)
                        {
                            XmlNodeList polbilgi = tmnt[polBilgindx].ChildNodes;
                            for (int j = 0; j < polbilgi.Count; j++)
                            {
                                XmlNode node = polbilgi[j];

                                if (node.Name == "DOVIZ-CINSI") pol.GenelBilgiler.ParaBirimi = polbilgi[j].InnerText;
                                if (node.Name == "DOVIZ-KURU") pol.GenelBilgiler.DovizKur = Util.ToDecimal(polbilgi[j].InnerText);
                            }

                        }

                        #region Araç Bilgileri
                        XmlNodeList matbular = polNode["MATBU-BILGI"].ChildNodes;

                        for (int aracBilgindx = 0; aracBilgindx < matbular.Count; aracBilgindx++)
                        {
                            XmlNodeList aracBilgiNode = matbular[aracBilgindx].ChildNodes;
                            for (int j = 0; j < aracBilgiNode.Count; j++)
                            {
                                XmlNode node = aracBilgiNode[j];

                                if (node.InnerText == "ARAÇ MARKASI  :")
                                {
                                    pol.GenelBilgiler.PoliceArac.MarkaAciklama = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "TİPİ          :")
                                {
                                    pol.GenelBilgiler.PoliceArac.AracinTipiKodu = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "MODEL         :")
                                {
                                    if (!String.IsNullOrEmpty(aracBilgiNode[j + 1].InnerText))
                                    {
                                        pol.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(aracBilgiNode[j + 1].InnerText);
                                    }
                                }
                                if (node.InnerText == "ARAÇ RENGİ :")
                                {
                                    pol.GenelBilgiler.PoliceArac.Renk = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "MOTOR NO      :")
                                {
                                    pol.GenelBilgiler.PoliceArac.MotorNo = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "ŞASİ NO       :")
                                {
                                    pol.GenelBilgiler.PoliceArac.SasiNo = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "PLAKA         :")
                                {
                                    if (!String.IsNullOrEmpty(aracBilgiNode[j + 1].InnerText))
                                    {
                                        pol.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                        pol.GenelBilgiler.PoliceArac.PlakaNo = "";
                                        string plakaNo = aracBilgiNode[j + 1].InnerText;
                                        if (!String.IsNullOrEmpty(plakaNo))
                                        {
                                            pol.GenelBilgiler.PoliceArac.PlakaNo = plakaNo != "" && plakaNo.Length >= 2 ? plakaNo.Substring(2, plakaNo.Length - 2) : "";
                                            pol.GenelBilgiler.PoliceArac.PlakaKodu = plakaNo != "" && plakaNo.Length >= 2 ? plakaNo.Substring(0, 2) : "";
                                        }
                                    }
                                }
                                if (node.InnerText == "KULLANIM ŞEKLİ:")
                                {
                                    pol.GenelBilgiler.PoliceArac.KullanimSekli = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "TESCİL TARİHİ :")
                                {
                                    if (!String.IsNullOrEmpty(aracBilgiNode[j + 1].InnerText))
                                    {
                                        pol.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(aracBilgiNode[j + 1].InnerText);
                                    }
                                }
                                if (node.InnerText == "MOTOR GÜCÜ    :")
                                {
                                    pol.GenelBilgiler.PoliceArac.MotorGucu = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "SİLİNDİR HACMİ:")
                                {
                                    pol.GenelBilgiler.PoliceArac.SilindirHacmi = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "MARKASI       :")
                                {
                                    pol.GenelBilgiler.PoliceArac.AracinTipiAciklama = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "TRAMER_SBM_NO:")
                                {
                                    pol.GenelBilgiler.PoliceArac.TramerBelgeNo = aracBilgiNode[j + 1].InnerText;
                                }
                                if (node.InnerText == "YAKIT TİPİ:")
                                {
                                    pol.GenelBilgiler.PoliceArac.YakitCinsi = aracBilgiNode[j + 1].InnerText;
                                }
                            }
                        }
                        #endregion

                        #region Sigorta Ettiren Bilgileri

                        XmlNodeList mustNode = polNode["SIGORTA-ETTIREN-BILGILERI"].ChildNodes;
                        for (int mnidx = 0; mnidx < mustNode.Count; mnidx++)
                        {
                            XmlNode mcnode = mustNode[mnidx];
                            if (mcnode.Name == "AD-UNVAN") pol.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = mcnode.InnerText;
                            if (mcnode.Name == "SOYAD-UNVAN2") pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = mcnode.InnerText;
                            if (mcnode.Name == "VERGI-NO") pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = mcnode.InnerText;
                            if (mcnode.Name == "TC-KIMLIK-NO") pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = mcnode.InnerText;
                            sEttirenKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                            if (mcnode.Name == "ADRES")
                            {
                                XmlNodeList mustAdrs = mcnode.ChildNodes;
                                for (int mustAdrsindx = 0; mustAdrsindx < mustAdrs.Count; mustAdrsindx++)
                                {
                                    XmlNode nodes = mustAdrs[mustAdrsindx];
                                    if (nodes.Name == "IL") pol.GenelBilgiler.PoliceSigortaEttiren.IlAdi = nodes.InnerText;
                                    if (nodes.Name == "ILCE") pol.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = nodes.InnerText;
                                    if (nodes.Name == "ULKE") pol.GenelBilgiler.PoliceSigortaEttiren.UlkeAdi = nodes.InnerText;
                                }
                            }
                        }
                        #endregion

                        #region Sigortalı Bilgileri

                        XmlNodeList SigortalıNode = polNode["SIGORTALI-BILGILERI"].ChildNodes;
                        for (int snidx = 0; snidx < SigortalıNode.Count; snidx++)
                        {
                            XmlNode mcnode = SigortalıNode[snidx];
                            if (mcnode.Name == "AD-UNVAN") pol.GenelBilgiler.PoliceSigortali.AdiUnvan = mcnode.InnerText;
                            if (mcnode.Name == "SOYAD-UNVAN2") pol.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = mcnode.InnerText;
                            if (mcnode.Name == "VERGI-NO") pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo = mcnode.InnerText;
                            if (mcnode.Name == "TC-KIMLIK-NO") pol.GenelBilgiler.PoliceSigortali.KimlikNo = mcnode.InnerText;
                            sLiKimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortali.KimlikNo) ? pol.GenelBilgiler.PoliceSigortali.KimlikNo : pol.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                            if (mcnode.Name == "ADRES")
                            {
                                XmlNodeList SAdrs = mcnode.ChildNodes;
                                for (int SAdrsindx = 0; SAdrsindx < SAdrs.Count; SAdrsindx++)
                                {
                                    XmlNode nodes = SAdrs[SAdrsindx];
                                    if (nodes.Name == "IL") pol.GenelBilgiler.PoliceSigortali.IlAdi = nodes.InnerText;
                                    if (nodes.Name == "ILCE") pol.GenelBilgiler.PoliceSigortali.IlceAdi = nodes.InnerText;
                                    if (nodes.Name == "ULKE") pol.GenelBilgiler.PoliceSigortali.UlkeAdi = nodes.InnerText;

                                }
                            }
                        }
                        #endregion

                        pol.GenelBilgiler.Durum = 0;
                        pol.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ZURICHSIGORTA;
                        pol.GenelBilgiler.YenilemeNo = 0;
                        pol.GenelBilgiler.BrutPrim = topVergi + pol.GenelBilgiler.NetPrim;
                        pol.GenelBilgiler.DovizliBrutPrim = topVergi + pol.GenelBilgiler.DovizliNetPrim;

                        #region Ödem Planı

                        XmlNode tk = polNode["TAKSIT-BILGILERI"];
                        XmlNodeList tks = tk.ChildNodes;
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
                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            odm.TaksitNo = indx + 1;
                            // odeme Tipi ?
                            odm.VadeTarihi = Util.toDate(elm["TAKSIT-VADESI"].InnerText, Util.DateFormat1);
                            odm.TaksitTutari = carpan * Util.ToDecimal(elm["TAKSIT-TUTARI"].InnerText);
                            if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                            {
                                odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["TAKSIT-TUTARI"].InnerText) * pol.GenelBilgiler.DovizKur.Value, 2) * carpan;
                                odm.DovizliTaksitTutari = Util.ToDecimal(elm["TAKSIT-TUTARI"].InnerText) * carpan;
                            }
                            if (pol.GenelBilgiler.BransKodu.Value == 1 || pol.GenelBilgiler.BransKodu.Value == 2)
                            {
                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                            }
                            else
                            {
                                odm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            if (odm.TaksitTutari != 0)
                            {
                                pol.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            #region Tahsilat işlemi
                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ZURICHSIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                                if (pol.GenelBilgiler.BransKodu.Value == 1 || pol.GenelBilgiler.BransKodu.Value == 2)
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    tahsilat.OdemeBelgeNo = "111111****1111";
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.KalanTaksitTutari = 0;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                                else
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.Havale;
                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                    tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                    //tahsilat.OdemeBelgeNo = "111111";
                                    tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = 0;
                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                    tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                    tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                    tahsilat.KimlikNo = !String.IsNullOrEmpty(pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? pol.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : pol.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                    tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.HasValue ? pol.GenelBilgiler.BrutPrim.Value : 0;
                                    tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                    tahsilat.KayitTarihi = DateTime.Today;
                                    tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                    tahsilat.TahsilatId = odm.PoliceId;
                                    if (tahsilat.TaksitTutari != 0)
                                    {
                                        pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                    }
                                }
                            }
                            #endregion
                        }
                        if (tks.Count == 0)
                        {

                            PoliceOdemePlani odmm = new PoliceOdemePlani();

                            if (odmm.TaksitTutari == null)
                            {
                                odmm.TaksitTutari = pol.GenelBilgiler.BrutPrim;
                                if (pol.GenelBilgiler.DovizKur != 1 && pol.GenelBilgiler.DovizKur != 0 && pol.GenelBilgiler.DovizKur != null)
                                {
                                    odmm.DovizliTaksitTutari = pol.GenelBilgiler.BrutPrim.Value;
                                }
                                if (odmm.VadeTarihi == null)
                                {
                                    odmm.VadeTarihi = pol.GenelBilgiler.BaslangicTarihi;
                                }
                                odmm.OdemeTipi = OdemeTipleri.Havale;
                                odmm.TaksitNo = 1;
                                if (odmm.TaksitTutari == null && pol.GenelBilgiler.BrutPrim != 0)
                                {
                                    pol.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ZURICHSIGORTA, pol.GenelBilgiler.BransKodu.Value);
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
                                                tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                                tahsilat.TaksitNo = odmm.TaksitNo;
                                                tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                                tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
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
                                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                                        tahsilat.OdemTipi = OdemeTipleri.Havale;
                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : pol.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilat.TaksitNo = odmm.TaksitNo;
                                        tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                        //tahsilat.OdemeBelgeNo = "111111";
                                        tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.OdenenTutar = 0;
                                        tahsilat.KalanTaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                        tahsilat.PoliceNo = pol.GenelBilgiler.PoliceNumarasi;
                                        tahsilat.ZeyilNo = pol.GenelBilgiler.EkNo.ToString();
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilat.BrutPrim = pol.GenelBilgiler.BrutPrim.Value;
                                        tahsilat.PoliceId = pol.GenelBilgiler.PoliceId;
                                        tahsilat.KayitTarihi = DateTime.Today;
                                        tahsilat.KaydiEkleyenKullaniciKodu = pol.GenelBilgiler.TVMKodu.Value;
                                        tahsilat.TahsilatId = odmm.PoliceId;
                                        if (tahsilat.TaksitTutari != 0)
                                        {
                                            pol.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 0) pol.GenelBilgiler.OdemeSekli = 0;
                        if (pol.GenelBilgiler.PoliceOdemePlanis.Count == 1) pol.GenelBilgiler.OdemeSekli = 1;
                        if (pol.GenelBilgiler.PoliceOdemePlanis.Count > 1) pol.GenelBilgiler.OdemeSekli = 2;

                        policeler.Add(pol);
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
