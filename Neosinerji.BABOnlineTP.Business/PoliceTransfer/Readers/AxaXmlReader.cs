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
    public class AxaXmlReader : IPoliceTransferReader
    {
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        private XmlNode PoliceXML;
        IPoliceTransferService _IPoliceTransferService;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullanici;

        public AxaXmlReader()
        { }

        public AxaXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();

        }
        public AxaXmlReader(XmlNode policeXML, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            this.PoliceXML = policeXML;
        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;
            int carpan = 1;

            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();


            string readerKulKodu = null;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            decimal? polKomisyon = null;
            decimal? polNet = null;
            decimal? polBrutprimim = null;
            XmlNode root;
            XmlNode s;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                if (this.PoliceXML == null)
                {
                    string[] tempPath = filePath.Split('#');
                    if (tempPath.Length > 1)
                    {
                        policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                        filePath = filePath.Substring(0, filePath.IndexOf("#"));

                    }
                    doc.Load(filePath);
                    root = doc.FirstChild;
                    s = root.NextSibling;
                }
                else
                {
                    doc.LoadXml(this.PoliceXML.OuterXml);
                    root = doc.FirstChild;
                    s = root.Clone();
                }
                Police police = new Police();
                XmlNode polNode = null;
                // xml dosyasında tek pplice varsa policelere node u olmuyor o yüzden...
                if (s.Name == "POLICE")
                {
                    polNode = s;

                    #region GenelBilgiler

                    XmlNodeList gnlb = polNode["GENEL"].ChildNodes;
                    for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                    {
                        XmlNode gnlnode = gnlb[gnlbidx];

                        // (T)ahakkuk / (I)ptal
                        if (gnlnode.Name == "T_I")
                        {
                            string ti = gnlnode.InnerText;
                            if (ti == "I")
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
                            else carpan = 1;
                        }

                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        if (gnlnode.Name == "TARIFE_KOD") tumUrunKodu = gnlnode.InnerText;
                        if (gnlnode.Name == "TARIFE_ADI") tumUrunAdi = gnlnode.InnerText;
                        if (gnlnode.Name == "CARI_POL_NO") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                        if (gnlnode.Name == "ZEYL_SIRA_NO") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                        if (gnlnode.Name == "YENILEME") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                        if (gnlnode.Name == "TANZIM_TARIHI") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                        if (gnlnode.Name == "BASLAMA_TARIH") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                        if (gnlnode.Name == "BITIS_TARIH") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                        if (gnlnode.Name == "DOVIZ_KOD") police.GenelBilgiler.ParaBirimi = gnlnode.InnerText;
                        if (gnlnode.Name == "DOVIZ_KUR")
                        {
                            if (!String.IsNullOrEmpty(gnlnode.InnerText))
                            {
                                police.GenelBilgiler.DovizKur = Convert.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                            }
                        }

                        #region Vergiler

                        // brut prim, net prim, toplem vergi ve vergiler
                        if (gnlnode.Name == "ICMAL")
                        {
                            XmlNodeList icmalnode = gnlnode.ChildNodes;
                            police.GenelBilgiler.ToplamVergi = 0;
                            for (int idx = 0; idx < icmalnode.Count; idx++)
                            {
                                XmlNode toplam = icmalnode[idx];

                                if (toplam["TOPLAM_BILGI"].InnerText == "NET_PRIM")
                                { // net prim
                                    police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                    {
                                        police.GenelBilgiler.DovizliNetPrim = carpan * Util.ToDecimal(toplam["TOPLAM_TUTAR"].InnerText);
                                    }
                                    polNet = police.GenelBilgiler.NetPrim;
                                }
                                if (toplam["TOPLAM_BILGI"].InnerText == "BRUT_PRIM") // brut prim
                                {
                                    police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                    {
                                        police.GenelBilgiler.DovizliBrutPrim = carpan * Util.ToDecimal(toplam["TOPLAM_TUTAR"].InnerText);
                                    }
                                    polBrutprimim = police.GenelBilgiler.BrutPrim;
                                }
                                if (toplam["TOPLAM_BILGI"].InnerText == "KOMISYON") //komisyon
                                {
                                    police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                    {
                                        police.GenelBilgiler.DovizliKomisyon = carpan * Util.ToDecimal(toplam["TOPLAM_TUTAR"].InnerText);
                                    }
                                    polKomisyon = police.GenelBilgiler.Komisyon;
                                }
                                //vergiler ve toplam vergi
                                if (toplam["TOPLAM_BILGI"].InnerText == "BSV") //vergi
                                {
                                    police.GenelBilgiler.ToplamVergi += carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    PoliceVergi bsv = new PoliceVergi();
                                    bsv.VergiKodu = 2;
                                    bsv.VergiTutari = carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    police.GenelBilgiler.PoliceVergis.Add(bsv);
                                }
                                if (toplam["TOPLAM_BILGI"].InnerText == "GH") //vergi
                                {
                                    police.GenelBilgiler.ToplamVergi += carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    PoliceVergi gh = new PoliceVergi();
                                    gh.VergiKodu = 3;
                                    gh.VergiTutari = carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    police.GenelBilgiler.PoliceVergis.Add(gh);
                                }
                                if (toplam["TOPLAM_BILGI"].InnerText == "THG") //vergi
                                {
                                    police.GenelBilgiler.ToplamVergi += carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    PoliceVergi thg = new PoliceVergi();
                                    thg.VergiKodu = 1;
                                    thg.VergiTutari = carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    police.GenelBilgiler.PoliceVergis.Add(thg);
                                }
                                if (toplam["TOPLAM_BILGI"].InnerText == "YSV") //vergi
                                {
                                    police.GenelBilgiler.ToplamVergi += carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    PoliceVergi ysv = new PoliceVergi();
                                    ysv.VergiKodu = 4;
                                    ysv.VergiTutari = carpan * Util.ToDecimal(toplam["TOPLAM_TL_TUTAR"].InnerText);
                                    police.GenelBilgiler.PoliceVergis.Add(ysv);
                                }
                            }
                        }
                        #endregion
                    }

                    //if (police.GenelBilgiler.DovizKur != null && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != 1)
                    //{
                    //    police.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                    //    police.GenelBilgiler.DovizliNetPrim = polNet.Value;
                    //    police.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
                    //}
                    police.GenelBilgiler.Durum = 0;
                    police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AXASIGORTA;
                    #endregion

                    #region Matbular

                    XmlNodeList matbuNode = polNode["MATBULAR"].ChildNodes;
                    police.GenelBilgiler.PoliceArac = new PoliceArac();
                    for (int mnidx = 0; mnidx < matbuNode.Count; mnidx++)
                    {
                        XmlNodeList matbuListNode = matbuNode[mnidx].ChildNodes;
                        for (int mlidx = 0; mlidx < matbuListNode.Count; mlidx++)
                        {
                            XmlNode snode = matbuListNode[mlidx];
                            XmlNode nodeItem = matbuNode.Item(mnidx);
                            if (snode.Name == "MATBU_KOD")
                            {
                                switch (snode.InnerText)
                                {
                                    case "MOT": police.GenelBilgiler.PoliceArac.MotorNo = nodeItem["ACIKLAMA"].InnerText; break;
                                    case "ŞAS": police.GenelBilgiler.PoliceArac.SasiNo = nodeItem["ACIKLAMA"].InnerText; break;
                                    case "PLK":
                                        {
                                            if (!String.IsNullOrEmpty(nodeItem["ACIKLAMA"].InnerText))
                                            {
                                                string plaka = nodeItem["ACIKLAMA"].InnerText;
                                                string plakaKodu = "";
                                                string plakaNo = "";
                                                bool plakaKodMu = true;
                                                if (!String.IsNullOrEmpty(plaka))
                                                {
                                                    plaka = plaka.Trim();
                                                }
                                                for (int i = 0; i < plaka.Length; i++)
                                                {
                                                    if (char.IsDigit(plaka[i]) && plakaKodMu)
                                                    {
                                                        plakaKodu += plaka[i];
                                                    }
                                                    else
                                                    {
                                                        plakaKodMu = false;
                                                        plakaNo += plaka[i];
                                                    }
                                                }
                                                if (String.IsNullOrEmpty(plakaNo))
                                                {
                                                    if (!String.IsNullOrEmpty(plakaKodu) && plakaKodu.Substring(0, 1) == "0")
                                                    {
                                                        plakaNo = plakaKodu.Substring(3, (plakaKodu.Length - 4));
                                                        plakaKodu = plakaKodu.Substring(0, 3);
                                                    }
                                                    else if (!String.IsNullOrEmpty(plakaKodu) && plakaKodu.Substring(0, 1) != "0")
                                                    {
                                                        plakaNo = plakaKodu.Substring(2, (plakaKodu.Length - 3));
                                                        plakaKodu = plakaKodu.Substring(0, 2);
                                                    }
                                                }
                                                police.GenelBilgiler.PoliceArac.PlakaKodu = plakaKodu;
                                                police.GenelBilgiler.PoliceArac.PlakaNo = plakaNo;
                                            }
                                        }
                                        break;
                                    case "REN": police.GenelBilgiler.PoliceArac.Renk = nodeItem["ACIKLAMA"].InnerText; break;
                                    case "YOL":
                                        {
                                            if (!String.IsNullOrEmpty(nodeItem["ACIKLAMA"].InnerText))
                                            {
                                                police.GenelBilgiler.PoliceArac.KoltukSayisi = Convert.ToInt32(nodeItem["ACIKLAMA"].InnerText);
                                            }

                                        }
                                        break;
                                    case "TCT":
                                        {
                                            if (!String.IsNullOrEmpty(nodeItem["ACIKLAMA"].InnerText))
                                            {
                                                police.GenelBilgiler.PoliceArac.TrafikCikisTarihi = Convert.ToDateTime(nodeItem["ACIKLAMA"].InnerText);
                                            }

                                        }
                                        break;
                                    case "TTT":
                                        {
                                            if (!String.IsNullOrEmpty(nodeItem["ACIKLAMA"].InnerText))
                                            {
                                                police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(nodeItem["ACIKLAMA"].InnerText);
                                            }
                                        }
                                        break;
                                    case "SHC": police.GenelBilgiler.PoliceArac.SilindirHacmi = nodeItem["ACIKLAMA"].InnerText; break;
                                    case "MTH": police.GenelBilgiler.PoliceArac.MotorGucu = nodeItem["ACIKLAMA"].InnerText; break;
                                    case "HDN": police.GenelBilgiler.PoliceArac.TramerBelgeNo = nodeItem["ACIKLAMA"].InnerText; break;
                                    case "HDT":
                                        {
                                            if (!String.IsNullOrEmpty(nodeItem["ACIKLAMA"].InnerText))
                                            {
                                                police.GenelBilgiler.PoliceArac.TramerBelgeTarihi = Convert.ToDateTime(nodeItem["ACIKLAMA"].InnerText);
                                            }
                                        }
                                        break;

                                    case "EGM": police.GenelBilgiler.PoliceArac.TescilSeriNo = nodeItem["ACIKLAMA"].InnerText; break;
                                    default:
                                        break;
                                }
                                break;
                            }
                        }
                    }

                    #endregion

                    #region brans eslestir
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
                    #endregion

                    #region Sigorta Ettiren Bilgileri

                    XmlNodeList mustNode = polNode["SIGORTA_ETTIREN"].ChildNodes;
                    for (int mnidx = 0; mnidx < mustNode.Count; mnidx++)
                    {
                        XmlNode snode = mustNode[mnidx];
                        if (snode.Name == "AD_UNVAN1") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = snode.InnerText;
                        if (snode.Name == "AD_UNVAN2")
                        {
                            if (!String.IsNullOrEmpty(snode.InnerText))
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan += " " + snode.InnerText;
                            }
                        }
                        if (snode.Name == "AD_UNVAN3") police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = snode.InnerText;
                        if (snode.Name == "KIMLIK_NO") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = snode.InnerText;
                        if (snode.Name == "VERGI_NO") police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = snode.InnerText;
                        sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        if (snode.Name == "ULKE_KOD") police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu = snode.InnerText;
                        if (snode.Name == "ULKE_ADI") police.GenelBilgiler.PoliceSigortaEttiren.UlkeAdi = snode.InnerText;
                        if (snode.Name == "IL_KOD") police.GenelBilgiler.PoliceSigortaEttiren.IlKodu = snode.InnerText;
                        if (snode.Name == "IL_ADI") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = snode.InnerText;
                        if (snode.Name == "ILCE_ADI") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = snode.InnerText;
                        if (snode.Name == "MAHALLE_ADI") police.GenelBilgiler.PoliceSigortaEttiren.Mahalle = snode.InnerText;
                        if (snode.Name == "CADDE") police.GenelBilgiler.PoliceSigortaEttiren.Cadde = snode.InnerText;
                        if (snode.Name == "SOKAK") police.GenelBilgiler.PoliceSigortaEttiren.Sokak = snode.InnerText;
                        if (snode.Name == "BINA_NO") police.GenelBilgiler.PoliceSigortaEttiren.BinaNo = snode.InnerText;
                        if (snode.Name == "CINSIYET") police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = snode.InnerText;
                        if (snode.Name == "POSTA_KOD") police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu = Util.toInt(snode.InnerText);
                        if (snode.Name == "DAIRE_NO") police.GenelBilgiler.PoliceSigortaEttiren.DaireNo = snode.InnerText;
                        if (snode.Name == "SEMT_KOY") police.GenelBilgiler.PoliceSigortaEttiren.Semt = snode.InnerText;
                        //if (snode.Name == "POSTA_KOD") police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu = Util.toInt(snode.InnerText); !!! nokta oldugunda hata veriyor.

                    }

                    #endregion

                    #region Sigortalı Bilgileri

                    XmlNodeList sigNode = polNode["SIGORTALI"].ChildNodes;
                    for (int snidx = 0; snidx < sigNode.Count; snidx++)
                    {
                        XmlNode snode = sigNode[snidx];
                        if (snode.Name == "AD_UNVAN1") police.GenelBilgiler.PoliceSigortali.AdiUnvan = snode.InnerText;
                        if (snode.Name == "AD_UNVAN2")
                        {
                            if (!String.IsNullOrEmpty(snode.InnerText))
                            {
                                police.GenelBilgiler.PoliceSigortali.AdiUnvan += " " + snode.InnerText;
                            }
                        }
                        if (snode.Name == "AD_UNVAN3") police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = snode.InnerText;
                        if (snode.Name == "KIMLIK_NO") police.GenelBilgiler.PoliceSigortali.KimlikNo = snode.InnerText;
                        if (snode.Name == "VERGI_NO") police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = snode.InnerText;
                        sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        if (snode.Name == "ULKE_KOD") police.GenelBilgiler.PoliceSigortali.UlkeKodu = snode.InnerText;
                        if (snode.Name == "ULKE_ADI") police.GenelBilgiler.PoliceSigortali.UlkeAdi = snode.InnerText;
                        if (snode.Name == "IL_KOD") police.GenelBilgiler.PoliceSigortali.IlKodu = snode.InnerText;
                        if (snode.Name == "IL_ADI") police.GenelBilgiler.PoliceSigortali.IlAdi = snode.InnerText;
                        if (snode.Name == "ILCE_ADI") police.GenelBilgiler.PoliceSigortali.IlceAdi = snode.InnerText;
                        if (snode.Name == "MAHALLE_ADI") police.GenelBilgiler.PoliceSigortali.Mahalle = snode.InnerText;
                        if (snode.Name == "CADDE") police.GenelBilgiler.PoliceSigortali.Cadde = snode.InnerText;
                        if (snode.Name == "SOKAK") police.GenelBilgiler.PoliceSigortali.Sokak = snode.InnerText;
                        if (snode.Name == "BINA_NO") police.GenelBilgiler.PoliceSigortali.BinaNo = snode.InnerText;
                        if (snode.Name == "DAIRE_NO") police.GenelBilgiler.PoliceSigortali.DaireNo = snode.InnerText;
                        if (snode.Name == "CINSIYET") police.GenelBilgiler.PoliceSigortali.Cinsiyet = snode.InnerText;
                        if (snode.Name == "POSTA_KOD") police.GenelBilgiler.PoliceSigortali.PostaKodu = Util.toInt(snode.InnerText);
                        if (snode.Name == "DAIRE_NO") police.GenelBilgiler.PoliceSigortali.DaireNo = snode.InnerText;
                        if (snode.Name == "SEMT_KOY") police.GenelBilgiler.PoliceSigortali.Semt = snode.InnerText;
                        //if (snode.Name == "POSTA_KOD") police.GenelBilgiler.PoliceSigortali.PostaKodu = Util.toInt(snode.InnerText);!!! nokta oldugunda hata veriyor.
                    }

                    #endregion

                    #region Ödem Planı

                    XmlNode tk = polNode["ODEME_PLANI"];
                    XmlNodeList tks = tk.ChildNodes;
                    var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);


                    for (int indx = 0; indx < tks.Count; indx++)
                    {
                        XmlNode elm = tks.Item(indx);
                        PoliceOdemePlani odm = new PoliceOdemePlani();

                        odm.TaksitNo = indx + 1;
                        odm.VadeTarihi = Util.toDate(elm["TAKSIT_TARIHI"].InnerText, Util.DateFormat1);
                        odm.TaksitTutari = carpan * Util.ToDecimal(elm["TOPLAM_BRUT_TL"].InnerText);
                        if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                        {
                            odm.DovizliTaksitTutari = Util.ToDecimal(elm["TOPLAM_BRUT"].InnerText) * carpan;
                        }
                        // odeme tipi null gelebılıyor tekrar bak acil !!!!!!!!!
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
                        #region Tahsilat işlemi



                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.AXASIGORTA, police.GenelBilgiler.BransKodu.Value);
                        int otoOdeSayac = 0;
                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                        {

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
                            if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
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
                                //tahsilat.OdemeBelgeNo = "111111";
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
                                    //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                }

                            }
                        }



                        #endregion
                    }
                    if (tks.Count == 0 && police.GenelBilgiler.BrutPrim.Value != 0)
                    {
                        PoliceOdemePlani odmm = new PoliceOdemePlani();
                        if (odmm.TaksitTutari == null)
                        {
                            odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                odmm.TaksitTutari = Math.Round(police.GenelBilgiler.BrutPrim.Value * police.GenelBilgiler.DovizKur.Value, 2);
                            }
                            if (police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != "TL")
                            {
                                odmm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim.Value;
                            }
                            if (odmm.VadeTarihi == null)
                            {
                                odmm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                            }
                            odmm.OdemeTipi = OdemeTipleri.Havale;
                            odmm.TaksitNo = 1;
                            if (odmm.TaksitTutari != 0)
                            {

                                police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                            }



                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.AXASIGORTA, police.GenelBilgiler.BransKodu.Value);

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
                                if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                                {
                                    PoliceTahsilat tahsilat = new PoliceTahsilat();
                                    tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                    odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                    tahsilat.OtomatikTahsilatiKkMi = 1;
                                    tahsilat.TaksitVadeTarihi = odmm.VadeTarihi.HasValue ? odmm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                    tahsilat.TaksitNo = odmm.TaksitNo;
                                    tahsilat.OdemeBelgeTarihi = odmm.VadeTarihi;
                                    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                    tahsilat.TaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                    tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
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
                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
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

                    }

                    //Ödeme Şekli
                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                    if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;

                    #endregion

                    policeler.Add(police);
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

