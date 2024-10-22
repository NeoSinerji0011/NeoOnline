using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class GunesXmlReader : IPoliceTransferReader
    {
        IPoliceTransferService _IPoliceTransferService;
        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public GunesXmlReader()
        {
        }

        public GunesXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
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
                decimal dovizKuru = 1;
                XmlNode root = doc.FirstChild;
                XmlNode s = root;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        if (s.ChildNodes[i].Name == "PoliceLer")
                        {
                            XmlNode polinode = s.ChildNodes[i];
                            XmlNodeList polNode = polinode.ChildNodes;
                            for (int polNodeindx = 0; polNodeindx < polNode.Count; polNodeindx++)
                            {
                                string polNo = null;
                                string readerKulKodu = null;
                                string odemeAraci = null;
                                XmlNode policenode = polNode[polNodeindx];
                                carpan = 1;
                                Police police = new Police();
                                if (policenode.Name == "Police")
                                {
                                    #region Genel Bilgiler

                                    XmlNodeList gnlb = policenode.ChildNodes;
                                    for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                                    {
                                        XmlNode gnlnode = gnlb[gnlbidx];
                                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                        else police.GenelBilgiler.TVMKodu = 0;
                                        if (gnlnode.Name == "UrunKodu") tumUrunKodu = gnlnode.InnerText;
                                        if (gnlnode.Name == "UrunAdi") tumUrunAdi = gnlnode.InnerText;
                                        if (gnlnode.Name == "PoliceNo")
                                        {
                                            police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                                            polNo = police.GenelBilgiler.PoliceNumarasi;
                                            foreach (var item in police.GenelBilgiler.PoliceTahsilats)
                                            {
                                                item.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                            }
                                        }
                                        if (gnlnode.Name == "ZeyilNo") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                        if (gnlnode.Name == "YenilemeNo") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                                        if (gnlnode.Name == "ZeyilTanimi") police.GenelBilgiler.ZeyilAdi = gnlnode.InnerText;
                                        if (gnlnode.Name == "IptalIstihsal")
                                        {
                                            if (gnlnode.InnerText == "IPTAL")
                                            {
                                                carpan = -1;
                                                police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * carpan;
                                                police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * carpan;
                                                police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * carpan;
                                                police.GenelBilgiler.ToplamVergi = police.GenelBilgiler.ToplamVergi * carpan;
                                                foreach (var item in police.GenelBilgiler.PoliceVergis)
                                                {
                                                    item.VergiTutari = item.VergiTutari * carpan;
                                                }
                                                foreach (var item in police.GenelBilgiler.PoliceOdemePlanis)
                                                {
                                                    item.TaksitTutari = item.TaksitTutari * carpan;
                                                }
                                            }
                                        }
                                        if (gnlnode.Name == "TanzimTarihi") police.GenelBilgiler.TanzimTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                        if (gnlnode.Name == "BaslangicTarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText);
                                        if (gnlnode.Name == "BitisTarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText);
                                        if (gnlnode.Name == "DovizCinsi") police.GenelBilgiler.ParaBirimi = gnlnode.InnerText;
                                        if (gnlnode.Name == "ToplamVergiTutari") police.GenelBilgiler.ToplamVergi = carpan * Util.ToDecimal(gnlnode.InnerText);
                                        if (gnlnode.Name == "DovizKuru")
                                        {
                                            if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                            {
                                                police.GenelBilgiler.DovizKur = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                            }
                                        }
                                        if (police.GenelBilgiler.ParaBirimi != "TL")
                                        {
                                            if (gnlnode.Name == "DovizKuru")
                                            {
                                                dovizKuru = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                            }
                                        }
                                        if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                        {
                                            if (gnlnode.Name == "NetPrim")
                                            {
                                                police.GenelBilgiler.DovizliNetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                //if (dovizKuru != 0 && dovizKuru != 1)
                                                //{
                                                //    police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru);
                                                //}
                                                if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                                {
                                                    police.GenelBilgiler.DovizliNetPrim = police.GenelBilgiler.DovizliNetPrim * 2;
                                                }
                                            }
                                            if (gnlnode.Name == "BrutPrim")
                                            {
                                                police.GenelBilgiler.DovizliBrutPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                //if (dovizKuru != 0 && dovizKuru != 1)
                                                //{
                                                //    police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                                //}
                                                if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                                {
                                                    police.GenelBilgiler.DovizliBrutPrim = police.GenelBilgiler.DovizliBrutPrim * 2;
                                                }
                                            }
                                            if (gnlnode.Name == "AcenteKomisyonTutari")
                                            {
                                                police.GenelBilgiler.DovizliKomisyon = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                if (dovizKuru != 0 && dovizKuru != 1)
                                                {
                                                    //police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru);
                                                    dovizKuru = 0;
                                                }
                                            }
                                        }
                                        if (gnlnode.Name == "TLNetPrim")
                                        {
                                            police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                            {
                                                police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * 2;
                                            }
                                        }
                                        if (gnlnode.Name == "TLBrutPrim")
                                        {
                                            police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                            {
                                                police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * 2;
                                            }
                                        }
                                        if (gnlnode.Name == "TLAcenteKomisyonTutari") police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(gnlnode.InnerText);

                                        if (gnlnode.Name == "teklifOlusturanKullanici") readerKulKodu = gnlnode.InnerText;
                                        if (readerKulKodu != null)
                                        {
                                            var getReaderKodu = _IPoliceTransferService.GetPoliceReaderKullanicilari(readerKulKodu);
                                            if (getReaderKodu != null)
                                            {
                                                police.GenelBilgiler.TaliAcenteKodu = Convert.ToInt32(getReaderKodu.AltTvmKodu);

                                            }
                                        }
                                        if (gnlnode.Name == "OdemeAraci")
                                        {
                                            odemeAraci = gnlnode.InnerText;
                                        }

                                        #region Vergiler

                                        if (gnlnode.Name == "Vergiler")
                                        {
                                            XmlNodeList Vergilernode = gnlnode.ChildNodes;
                                            for (int idx = 0; idx < Vergilernode.Count; idx++)
                                            {
                                                XmlNode vergi = Vergilernode.Item(idx);
                                                if (vergi.Name == "Vergi")
                                                {
                                                    PoliceVergi gv = new PoliceVergi();
                                                    bool kontrol = true;
                                                    XmlNodeList verginodes = vergi.ChildNodes;
                                                    for (int verginodesindx = 0; verginodesindx < verginodes.Count; verginodesindx++)
                                                    {
                                                        XmlNode vergin = verginodes[verginodesindx];
                                                        if (vergin.Name == "VergiAdi")
                                                        {
                                                            if (vergin.InnerText == "BSMV") gv.VergiKodu = 2;
                                                            if (vergin.InnerText == "YSV") gv.VergiKodu = 4;
                                                            //garanti Fonu
                                                            if (vergin.InnerText == "Güvence  Hesabı Fonu") gv.VergiKodu = 3;
                                                            if (vergin.InnerText == "Güven.Hsb.İşv.Payı") kontrol = false;
                                                            if (vergin.InnerText == "Trf.Hizm.Glş.Fonu") gv.VergiKodu = 1;
                                                        }
                                                        if (vergin.Name == "VergiTutari" && kontrol)
                                                        {
                                                            gv.VergiTutari = carpan * Util.ToDecimal(vergin.InnerText);

                                                        }
                                                    }
                                                    if (kontrol)
                                                    {
                                                        police.GenelBilgiler.PoliceVergis.Add(gv);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Sigortalı Bilgileri

                                        if (gnlnode.Name == "Sigortalilar")
                                        {

                                            XmlNodeList sigortalinode = gnlnode.ChildNodes;
                                            string pasaportno = "";
                                            for (int idx = 0; idx < sigortalinode.Count; idx++)
                                            {
                                                XmlNode sigortanode = sigortalinode.Item(idx);
                                                if (sigortanode.Name == "Sigortali")
                                                {

                                                    XmlNodeList sigortanodelist = sigortanode.ChildNodes;
                                                    for (int sigortanodelistindx = 0; sigortanodelistindx < sigortanodelist.Count; sigortanodelistindx++)
                                                    {
                                                        XmlNode nodes = sigortanodelist[sigortanodelistindx];
                                                        if (nodes.Name == "PasaportNo") pasaportno = nodes.InnerText;
                                                        if (nodes.Name == "AdSoyadUnvan") police.GenelBilgiler.PoliceSigortali.AdiUnvan = nodes.InnerText;

                                                        if (nodes.Name == "KimlikNo") police.GenelBilgiler.PoliceSigortali.KimlikNo = nodes.InnerText;
                                                        if (nodes.Name == "VergiNo")
                                                        {
                                                            police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = nodes.InnerText;
                                                            if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo.Length==9)
                                                            {
                                                                police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = "0" + police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                                            }

                                                        }
                                                        sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                                        if (nodes.Name == "Ulke") police.GenelBilgiler.PoliceSigortali.UlkeAdi = nodes.InnerText;
                                                        if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == null && police.GenelBilgiler.PoliceSigortali.KimlikNo == null && police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == null && police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == null && !String.IsNullOrEmpty(pasaportno))
                                                        {
                                                            police.GenelBilgiler.PoliceSigortali.KimlikNo = pasaportno;
                                                        }
                                                        if (nodes.Name == "Adresler")
                                                        {
                                                            XmlNodeList adresler = nodes.ChildNodes;

                                                            for (int adrindx = 0; adrindx < adresler.Count; adrindx++)
                                                            {
                                                                XmlNodeList adresItems = adresler[adrindx].ChildNodes;
                                                                for (var j = 0; j < adresItems.Count; j++)
                                                                {
                                                                    XmlNode adr = adresItems[j];
                                                                    if (adr.Name == "AdresinKisaAdi")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Adres = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Adres))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Adres = ".";
                                                                    }
                                                                    if (adr.Name == "Ulke")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.UlkeAdi = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.UlkeAdi))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.UlkeAdi = "Belirtilmedi";
                                                                    }
                                                                    if (adr.Name == "Sehir")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.IlAdi = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.IlAdi))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.IlAdi = ".";
                                                                    }
                                                                    if (adr.Name == "Ilce")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.IlceAdi = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.IlceAdi))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.IlceAdi = ".";
                                                                    }
                                                                    if (adr.Name == "Belde")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Semt = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Semt))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Semt = ".";
                                                                    }
                                                                    if (adr.Name == "Cadde")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Cadde = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Cadde))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Cadde = ".";
                                                                    }
                                                                    if (adr.Name == "Sokak")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Sokak = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.Sokak))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.Sokak = ".";
                                                                    }
                                                                    if (adr.Name == "PostaKodu")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.PostaKodu = Convert.ToInt32(adr.InnerText);

                                                                        if (police.GenelBilgiler.PoliceSigortali.PostaKodu == null)
                                                                        {
                                                                            police.GenelBilgiler.PoliceSigortali.PostaKodu = 0;
                                                                        }
                                                                    }

                                                                    if (adr.Name == "DaireNo")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.DaireNo = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.DaireNo))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.DaireNo = ".";
                                                                    }
                                                                    if (adr.Name == "KapiNo")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.BinaNo = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.BinaNo))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortali.BinaNo = ".";
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region Sigorta Ettiren Bilgileri

                                        if (gnlnode.Name == "SigortaEttirenler")
                                        {
                                            XmlNodeList sigortaettirennode = gnlnode.ChildNodes;
                                            string pasaportno = "";
                                            for (int idx = 0; idx < sigortaettirennode.Count; idx++)
                                            {
                                                XmlNode sigettirennode = sigortaettirennode.Item(idx);
                                                if (sigettirennode.Name == "SigortaEttiren")
                                                {

                                                    XmlNodeList sigortaet = sigettirennode.ChildNodes;
                                                    for (int sigortaetindx = 0; sigortaetindx < sigortaet.Count; sigortaetindx++)
                                                    {
                                                        XmlNode nodes = sigortaet[sigortaetindx];
                                                        if (nodes.Name == "PasaportNo") pasaportno = nodes.InnerText;
                                                        if (nodes.Name == "AdSoyadUnvan") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = nodes.InnerText;

                                                        if (nodes.Name == "KimlikNo") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = nodes.InnerText;
                                                        if (nodes.Name == "VergiNo")
                                                        {
                                                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = nodes.InnerText;
                                                            if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo.Length == 9)
                                                            {
                                                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = "0" + police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                                            }

                                                        }
                                                        if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == null && police.GenelBilgiler.PoliceSigortali.KimlikNo == null && police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == null && police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == null && !String.IsNullOrEmpty(pasaportno))
                                                        {
                                                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = pasaportno;
                                                        }
                                                        sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                                        if (nodes.Name == "Ulke") police.GenelBilgiler.PoliceSigortaEttiren.UlkeAdi = nodes.InnerText;
                                                        if (nodes.Name == "Adresler")
                                                        {
                                                            XmlNodeList adresler = nodes.ChildNodes;

                                                            for (int adrindx = 0; adrindx < adresler.Count; adrindx++)
                                                            {
                                                                XmlNodeList adresItems = adresler[adrindx].ChildNodes;
                                                                for (var j = 0; j < adresItems.Count; j++)
                                                                {
                                                                    XmlNode adr = adresItems[j];
                                                                    if (adr.Name == "AdresinKisaAdi")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Adres = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Adres))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Adres = ".";
                                                                    }
                                                                    if (adr.Name == "Ulke")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.UlkeAdi = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.UlkeAdi))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.UlkeAdi = "Belirtilmedi";
                                                                    }
                                                                    if (adr.Name == "Sehir")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.IlAdi))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = ".";
                                                                    }
                                                                    if (adr.Name == "Ilce")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = ".";
                                                                    }
                                                                    if (adr.Name == "Belde")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Semt = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Semt))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Semt = ".";
                                                                    }
                                                                    if (adr.Name == "Cadde")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Cadde = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Cadde))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Cadde = ".";
                                                                    }
                                                                    if (adr.Name == "Sokak")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Sokak = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.Sokak))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.Sokak = ".";
                                                                    }
                                                                    if (adr.Name == "PostaKodu")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu = Convert.ToInt32(adr.InnerText);

                                                                        if (police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu == null)
                                                                        {
                                                                            police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu = 0;
                                                                        }
                                                                    }

                                                                    if (adr.Name == "DaireNo")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.DaireNo = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.DaireNo))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.DaireNo = ".";
                                                                    }
                                                                    if (adr.Name == "KapiNo")
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.BinaNo = adr.InnerText;
                                                                    }
                                                                    if (String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.BinaNo))
                                                                    {
                                                                        police.GenelBilgiler.PoliceSigortaEttiren.BinaNo = ".";
                                                                    }
                                                                }
                                                            }
                                                        }
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
                                        decimal taksitTutari = 0;
                                        string OdemeBelgeNo = null;
                                        if (gnlnode.Name == "Taksitler")
                                        {

                                            XmlNodeList tks = gnlnode.ChildNodes;

                                            for (int indx = 0; indx < tks.Count; indx++)
                                            {
                                                XmlNode elm = tks.Item(indx);
                                                XmlNodeList taksitelm = elm.ChildNodes;
                                                PoliceOdemePlani odm = new PoliceOdemePlani();
                                                PoliceTahsilat tahsilats = new PoliceTahsilat();
                                                //PoliceTahsilat tahsilatss = new PoliceTahsilat();                                       
                                                for (int taksitelmindx = 0; taksitelmindx < taksitelm.Count; taksitelmindx++)
                                                {
                                                    XmlNode taksitnodes = taksitelm[taksitelmindx];
                                                    if (taksitnodes.Name == "TaksitNo") odm.TaksitNo = Util.toInt(taksitnodes.InnerText);
                                                    if (taksitnodes.Name == "VadeTarihi") odm.VadeTarihi = Util.toDate(taksitnodes.InnerText);
                                                    if (taksitnodes.Name == "TaksitTutari")
                                                    {
                                                        odm.TaksitTutari = carpan * Util.ToDecimal(taksitnodes.InnerText);
                                                        if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                        {
                                                            odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["TaksitTutari"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2) * carpan;
                                                            odm.DovizliTaksitTutari = carpan * Util.ToDecimal(elm["TaksitTutari"].InnerText);
                                                        }
                                                    }
                                                    taksitTutari = Convert.ToDecimal(odm.TaksitTutari);
                                                    if (taksitnodes.Name == "KartNumarasi")
                                                    {
                                                        OdemeBelgeNo = taksitnodes.InnerText;
                                                    }
                                                }
                                                if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                                                {
                                                    var ayniTaksitVarmi = police.GenelBilgiler.PoliceOdemePlanis.Where(y => y.TaksitNo == odm.TaksitNo).FirstOrDefault();
                                                    var ayniTaksitVarmii = police.GenelBilgiler.PoliceTahsilats.Where(y => y.TaksitNo == odm.TaksitNo).FirstOrDefault();

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
                                                    if (ayniTaksitVarmii == null)
                                                    {// odeme belge kk harıcı yazı alıyor acik gibi , pop da - olan poltah da + 
                                                        #region tahsilat
                                                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.TURKIYESIGORTA, police.GenelBilgiler.BransKodu.Value);
                                                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                                        {
                                                            int otoOdeSayac = 0;
                                                            foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                                            {
                                                                if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                                {
                                                                    otoOdeSayac++;
                                                                    //PoliceTahsilat tahsilats = new PoliceTahsilat();

                                                                    tahsilats.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                                    odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                                    if (tahsilats.OdemTipi == 1)
                                                                    {
                                                                        tahsilats.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                        tahsilats.KalanTaksitTutari = 0;
                                                                        tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                                        if (OdemeBelgeNo == null)
                                                                        {
                                                                            tahsilats.OdemeBelgeNo = "111111****1111";
                                                                        }
                                                                        tahsilats.OtomatikTahsilatiKkMi = 1;
                                                                    }
                                                                    else
                                                                    {
                                                                        tahsilats.OdenenTutar = 0;
                                                                        tahsilats.KalanTaksitTutari = odm.TaksitTutari;
                                                                    }
                                                                    tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                                    tahsilats.TaksitNo = odm.TaksitNo;
                                                                    tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                                    tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                    tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                                    tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                                    tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                                    tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                                    tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                                    tahsilats.KayitTarihi = DateTime.Today;
                                                                    tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            #region Tahsilat işlemi
                                                            // PoliceTahsilat tahsilatss = new PoliceTahsilat();
                                                            // tahsilat = new PoliceTahsilat();
                                                            if (odemeAraci == "Kredi Karti")
                                                            {
                                                                tahsilats.OdemTipi = OdemeTipleri.KrediKarti;
                                                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                                tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                                if (OdemeBelgeNo == null)
                                                                {
                                                                    tahsilats.OdemeBelgeNo = "111111****1111";
                                                                }
                                                                tahsilats.KalanTaksitTutari = 0;
                                                                tahsilats.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                tahsilats.OtomatikTahsilatiKkMi = 1;
                                                            }
                                                            else if (odemeAraci == "Nakit")
                                                            {
                                                                tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                                odm.OdemeTipi = OdemeTipleri.Nakit;
                                                                tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                tahsilats.OdenenTutar = 0;
                                                                tahsilats.OdemeBelgeNo = null;
                                                            }
                                                            else
                                                            {
                                                                tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                                odm.OdemeTipi = OdemeTipleri.Nakit;
                                                                tahsilats.OdemeBelgeNo = null;
                                                                tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                tahsilats.OdenenTutar = 0;
                                                            }
                                                            tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilats.TaksitNo = odm.TaksitNo;
                                                            tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                            tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                            tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                            tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                            tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                            tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                            tahsilats.KayitTarihi = DateTime.Today;
                                                            tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                            #endregion
                                                        }
                                                        #endregion
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilats);

                                                    }
                                                    else
                                                    {
                                                        odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                        tahsilats.TaksitTutari = odm.TaksitTutari.Value;
                                                        police.GenelBilgiler.PoliceTahsilats.Remove(ayniTaksitVarmii);
                                                        #region tahsilat
                                                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.TURKIYESIGORTA, police.GenelBilgiler.BransKodu.Value);
                                                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                                        {
                                                            int otoOdeSayac = 0;
                                                            foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                                            {
                                                                if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                                {
                                                                    otoOdeSayac++;
                                                                    //PoliceTahsilat tahsilats = new PoliceTahsilat();

                                                                    tahsilats.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                                    odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                                    if (tahsilats.OdemTipi == 1)
                                                                    {
                                                                        tahsilats.OdenenTutar = taksitTutari + ayniTaksitVarmi.TaksitTutari.Value;
                                                                        tahsilats.KalanTaksitTutari = 0;
                                                                        tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                                        if (OdemeBelgeNo == null)
                                                                        {
                                                                            tahsilats.OdemeBelgeNo = "111111****1111";
                                                                        }
                                                                        tahsilats.OtomatikTahsilatiKkMi = 1;
                                                                    }
                                                                    else
                                                                    {
                                                                        tahsilats.OdenenTutar = 0;
                                                                        tahsilats.KalanTaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                                    }
                                                                    tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                                    tahsilats.TaksitNo = odm.TaksitNo;
                                                                    tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                                    odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                                    tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                                    tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                                    tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                                    tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                                    tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                                    tahsilats.KayitTarihi = DateTime.Today;
                                                                    tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            #region Tahsilat işlemi
                                                            // PoliceTahsilat tahsilatss = new PoliceTahsilat();
                                                            // tahsilat = new PoliceTahsilat();
                                                            if (odemeAraci == "Kredi Karti")
                                                            {
                                                                tahsilats.OdemTipi = OdemeTipleri.KrediKarti;
                                                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                                tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                                tahsilats.KalanTaksitTutari = 0;
                                                                tahsilats.OdenenTutar = taksitTutari + ayniTaksitVarmi.TaksitTutari.Value;
                                                                tahsilats.OtomatikTahsilatiKkMi = 1;
                                                                if (OdemeBelgeNo == null)
                                                                {
                                                                    tahsilats.OdemeBelgeNo = "111111****1111";
                                                                }
                                                            }
                                                            else if (odemeAraci == "Nakit")
                                                            {
                                                                tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                                odm.OdemeTipi = OdemeTipleri.Nakit;
                                                                tahsilats.KalanTaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                                tahsilats.OdenenTutar = 0;
                                                                tahsilats.OdemeBelgeNo = null;
                                                            }
                                                            else
                                                            {
                                                                tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                                odm.OdemeTipi = OdemeTipleri.Nakit;
                                                                tahsilats.OdemeBelgeNo = null;
                                                                tahsilats.KalanTaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                                tahsilats.OdenenTutar = 0;
                                                            }
                                                            tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilats.TaksitNo = odm.TaksitNo;
                                                            tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                            tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                            tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                            tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                            tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                            tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                            tahsilats.KayitTarihi = DateTime.Today;
                                                            tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                            #endregion
                                                        }
                                                        #endregion
                                                        police.GenelBilgiler.PoliceTahsilats.Add(tahsilats);

                                                    }

                                                }
                                            }
                                            //oto odeme tipinden cekılecek
                                            if (tks.Count == 0)
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
                                                        PoliceTahsilat tahsilat = new PoliceTahsilat();

                                                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.TURKIYESIGORTA, police.GenelBilgiler.BransKodu.Value);
                                                        if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                                        {
                                                            int otoOdeSayac = 0;
                                                            foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                                            {
                                                                if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                                {
                                                                    otoOdeSayac++;
                                                                    //PoliceTahsilat tahsilat = new PoliceTahsilat();

                                                                    tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                                    odmm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                                    if (tahsilat.OdemTipi == 1)
                                                                    {
                                                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                                        tahsilat.KalanTaksitTutari = 0;
                                                                        tahsilat.OdemeBelgeNo = OdemeBelgeNo;
                                                                        if (OdemeBelgeNo == null)
                                                                        {
                                                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                                                        }
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
                                                            #region Tahsilat işlemi
                                                            // PoliceTahsilat tahsilatss = new PoliceTahsilat();
                                                            // tahsilat = new PoliceTahsilat();
                                                            if (odemeAraci == "Kredi Karti")
                                                            {
                                                                tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                                odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                                tahsilat.OdemeBelgeNo = OdemeBelgeNo;
                                                                tahsilat.KalanTaksitTutari = 0;
                                                                tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                                tahsilat.OtomatikTahsilatiKkMi = 1;
                                                                if (OdemeBelgeNo == null)
                                                                {
                                                                    tahsilat.OdemeBelgeNo = "111111****1111";
                                                                }
                                                            }
                                                            else if (odemeAraci == "Nakit")
                                                            {
                                                                tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                                odmm.OdemeTipi = OdemeTipleri.Nakit;
                                                                tahsilat.KalanTaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                                tahsilat.OdenenTutar = 0;
                                                                tahsilat.OdemeBelgeNo = null;
                                                            }
                                                            else
                                                            {
                                                                tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                                odmm.OdemeTipi = OdemeTipleri.Nakit;
                                                                tahsilat.OdemeBelgeNo = null;
                                                                tahsilat.KalanTaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                                tahsilat.OdenenTutar = 0;
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
                                                            #endregion

                                                        }
                                                    }
                                                }

                                            }

                                        }

                                        #endregion

                                        #region Araç Bilgileri

                                        if (gnlnode.Name == "TarifeSorulari")
                                        {
                                            XmlNodeList tarifeSoru = gnlnode.ChildNodes;

                                            for (int indx = 0; indx < tarifeSoru.Count; indx++)
                                            {
                                                XmlNode elm = tarifeSoru.Item(indx);
                                                XmlNodeList tarifeSoruElm = elm.ChildNodes;
                                                for (int taksitelmindx = 0; taksitelmindx < tarifeSoruElm.Count; taksitelmindx++)
                                                {
                                                    XmlNode taksitnodes = tarifeSoruElm[taksitelmindx];
                                                    if (taksitnodes.Name == "TarifeSorusuKodu")
                                                    {
                                                        if (taksitnodes.InnerText == "PLAKA_IL_KODU" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.PlakaKodu = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "CINSIYET" && tarifeSoruElm.Count == 4)
                                                        {
                                                            if (tarifeSoruElm[taksitelmindx + 2].InnerText == "KADIN")
                                                            {
                                                                police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = "K";
                                                                police.GenelBilgiler.PoliceSigortali.Cinsiyet = "K";
                                                            }
                                                            else if (tarifeSoruElm[taksitelmindx + 2].InnerText == "ERKEK")
                                                            {
                                                                police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = "E";
                                                                police.GenelBilgiler.PoliceSigortali.Cinsiyet = "E";
                                                            }
                                                            else
                                                            {
                                                                police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                                police.GenelBilgiler.PoliceSigortali.Cinsiyet = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                            }

                                                        }
                                                        if (taksitnodes.InnerText == "PLAKA_NO" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.PlakaNo = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "TESCIL_BELGE_SERI_KOD" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TescilSeriKod = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "TESCIL_BELGE_SERI_NO" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TescilSeriNo = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "ARAC_GRUP" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.KullanimTarzi = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "EGM_ARAC_KULLANIM_SEKLI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.KullanimSekli = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "ARAC_MARKASI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.MarkaAciklama = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "ARAC_TIPI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.AracinTipiAciklama = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "ARAC_KODU" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.Marka = tarifeSoruElm[taksitelmindx + 3].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "ARAC_MODELI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            if (!String.IsNullOrEmpty(tarifeSoruElm[taksitelmindx + 2].InnerText))
                                                            {
                                                                if (tarifeSoruElm[taksitelmindx + 2].InnerText != "DİĞER")
                                                                {
                                                                    police.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                                }
                                                            }
                                                        }
                                                        if (taksitnodes.InnerText == "ARAC_MODEL_DIGER" && tarifeSoruElm.Count == 4)
                                                        {
                                                            if (!String.IsNullOrEmpty(tarifeSoruElm[taksitelmindx + 2].InnerText))
                                                            {
                                                                police.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                            }
                                                        }
                                                        if (taksitnodes.InnerText == "MOTOR_NO" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.MotorNo = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "SASI_NO" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.SasiNo = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "TESCIL_TARIHI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            if (!String.IsNullOrEmpty(tarifeSoruElm[taksitelmindx + 2].InnerText))
                                                            {
                                                                police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                            }
                                                        }
                                                        if (taksitnodes.InnerText == "KOLTUK_SAYISI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.KoltukSayisi = Util.toInt(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_ILI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceRizikoAdresi.Il = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_ILCESI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceRizikoAdresi.Ilce = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_BELDESI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceRizikoAdresi.SemtBelde = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_KOY_MAHALLESI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceRizikoAdresi.Mahalle = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_CADDESI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceRizikoAdresi.Cadde = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_SOKAGI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceRizikoAdresi.Sokak = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_SITESI" && tarifeSoruElm.Count == 4)
                                                        {
                                                            if (!String.IsNullOrEmpty(tarifeSoruElm[taksitelmindx + 2].InnerText))
                                                                {
                                                                if (tarifeSoruElm[taksitelmindx + 2].InnerText.Length > 50)
                                                                {
                                                                    police.GenelBilgiler.PoliceRizikoAdresi.Bina = tarifeSoruElm[taksitelmindx + 2].InnerText.ToString().Substring(0, 50);
                                                                }
                                                                else
                                                                {
                                                                    police.GenelBilgiler.PoliceRizikoAdresi.Bina = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                                }


                                                            }

                                                           
                                                           
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_BLOK_NO" && tarifeSoruElm.Count == 4)
                                                        {
                                                            
                                                                police.GenelBilgiler.PoliceRizikoAdresi.BinaKodu = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                            
                                                        }
                                                        if (taksitnodes.InnerText == "RIZIKO_DAIRE_NO" && tarifeSoruElm.Count == 4)
                                                        {
                                                            police.GenelBilgiler.PoliceRizikoAdresi.Daire = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }

                                    police.GenelBilgiler.Durum = 0;
                                    police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.TURKIYESIGORTA;

                                    // Odeme Sekli
                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;
                                    foreach (var itemv in police.GenelBilgiler.PoliceTahsilats)
                                    {
                                        itemv.BrutPrim = police.GenelBilgiler.BrutPrim.Value;
                                        itemv.TaksitTutari = (itemv.TaksitTutari) * carpan;
                                        itemv.KalanTaksitTutari = (itemv.KalanTaksitTutari) * carpan;
                                        itemv.OdenenTutar = (itemv.OdenenTutar) * carpan;
                                    }

                                    policeler.Add(police);
                                    #endregion
                                }
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
    }
}
