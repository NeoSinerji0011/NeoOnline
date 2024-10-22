using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class GeneraliXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public GeneraliXmlReader()
        {
        }

        public GeneraliXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            decimal topVergi = 0;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            int dovizKuru = 0;
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

                XmlNode root = doc.LastChild;
                XmlNode s = root;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {

                        if (s.ChildNodes[i].Name == "police-list")
                        {
                            XmlNode polinode = s.ChildNodes[i];
                            XmlNodeList polNode = polinode.ChildNodes;

                            for (int polNodeindx = 0; polNodeindx < polNode.Count; polNodeindx++)
                            {
                                XmlNode policenode = polNode[polNodeindx];

                                carpan = 1;
                                topVergi = 0;
                                Police police = new Police();
                                if (policenode.Name == "police")
                                {
                                    PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                                    PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                                    police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                                    police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                                    #region Genel Bilgiler

                                    if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                    else police.GenelBilgiler.TVMKodu = 0;

                                    XmlNodeList gnlb = policenode.ChildNodes;

                                    string iptalNode = policenode.SelectSingleNode("zeyil-istihsal-iptal").InnerText;
                                    if (iptalNode == "2")
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
                                    for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                                    {

                                        XmlNode gnlnode = gnlb[gnlbidx];

                                        if (gnlnode.Name == "urun-kodu") tumUrunKodu = gnlnode.InnerText;
                                        if (gnlnode.Name == "police-no") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                                        if (gnlnode.Name == "zeyil-no") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                        if (gnlnode.Name == "yenileme-no") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                                        if (gnlnode.Name == "tanzim-tarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                        if (gnlnode.Name == "police-baslama-tarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                        if (gnlnode.Name == "police-bitis-tarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat2);
                                        if (gnlnode.Name == "para-birimi") police.GenelBilgiler.ParaBirimi = gnlnode.InnerText;

                                        if (gnlnode.Name == "doviz-kuru")
                                        {
                                            if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                            {
                                                police.GenelBilgiler.DovizKur = decimal.Parse(gnlnode.InnerText, new NumberFormatInfo() { NumberDecimalSeparator = "." });

                                                if (!String.IsNullOrEmpty(police.GenelBilgiler.ParaBirimi) && police.GenelBilgiler.ParaBirimi != "TL")
                                                {
                                                    decimal generaliDovizKuru = decimal.Parse(gnlnode.InnerText, new NumberFormatInfo() { NumberDecimalSeparator = "." });
                                                    if (generaliDovizKuru > 1)
                                                    {
                                                        police.GenelBilgiler.DovizKur = generaliDovizKuru;
                                                    }
                                                }
                                            }
                                        }

                                        if (gnlnode.Name == "brut-prim")
                                        {
                                            police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            polBrutprimim = police.GenelBilgiler.BrutPrim;
                                            police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * police.GenelBilgiler.DovizKur.Value, 2);

                                        }
                                        if (gnlnode.Name == "net-prim")
                                        {
                                            police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            polNet = police.GenelBilgiler.NetPrim;
                                            police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * police.GenelBilgiler.DovizKur.Value, 2);
                                        }
                                        if (gnlnode.Name == "acente-komisyonu")
                                        {
                                            police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            polKomisyon = police.GenelBilgiler.Komisyon;
                                            police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * police.GenelBilgiler.DovizKur.Value, 2);
                                            dovizKuru = 0;
                                        }
                                        if (police.GenelBilgiler.DovizKur != null && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != 1)
                                        {
                                            police.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                                            police.GenelBilgiler.DovizliNetPrim = polNet.Value;
                                            police.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
                                        }
                                        if (gnlnode.Name == "riziko-adresi") police.GenelBilgiler.PoliceRizikoAdresi.Adres = gnlnode.InnerText;
                                        if (gnlnode.Name == "riziko-ili") police.GenelBilgiler.PoliceRizikoAdresi.Il = gnlnode.InnerText;
                                        if (gnlnode.Name == "riziko-ilce") police.GenelBilgiler.PoliceRizikoAdresi.Ilce = gnlnode.InnerText;
                                        if (gnlnode.Name == "musteri-unvani") police.GenelBilgiler.PoliceSigortali.AdiUnvan = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigorta-ettiren-unvani") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigorta-ettiren-adresi") police.GenelBilgiler.PoliceSigortaEttiren.Adres = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigorta-ettiren-ili") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigorta-ettiren-ilce") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigorta-ettiren-vergi-numarasi") police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigorta-ettiren-tckimlik-numarasi") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigortali-tckimlik-numarasi") police.GenelBilgiler.PoliceSigortali.KimlikNo = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigorta-ettiren-vergi-numarasi") police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigortali-ili") police.GenelBilgiler.PoliceSigortali.IlAdi = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigortali-ilce") police.GenelBilgiler.PoliceSigortali.IlceAdi = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigortali-cep-telefonu") police.GenelBilgiler.PoliceSigortali.MobilTelefonNo = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigortali-is-telefonu") police.GenelBilgiler.PoliceSigortali.TelefonNo = gnlnode.InnerText;
                                        if (gnlnode.Name == "sigortali-adresi") police.GenelBilgiler.PoliceSigortali.Adres = gnlnode.InnerText;
                                        sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                        sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                                        #region Vergiler

                                        if (gnlnode.Name == "gider-vergisi")
                                        {
                                            PoliceVergi gv = new PoliceVergi();
                                            gv.VergiKodu = 2;
                                            gv.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            topVergi += carpan * Util.ToDecimal(gnlnode.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(gv);
                                        }

                                        if (gnlnode.Name == "trafik-hizmetleri-fonu")
                                        {
                                            PoliceVergi tfg = new PoliceVergi();
                                            tfg.VergiKodu = 1;
                                            tfg.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            topVergi += carpan * Util.ToDecimal(gnlnode.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(tfg);
                                        }
                                        if (gnlnode.Name == "ysv")
                                        {
                                            PoliceVergi yv = new PoliceVergi();
                                            yv.VergiKodu = 4;
                                            yv.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            topVergi += carpan * Util.ToDecimal(gnlnode.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(yv);
                                        }
                                        if (gnlnode.Name == "garanti-fonu")
                                        {
                                            PoliceVergi gf = new PoliceVergi();
                                            gf.VergiKodu = 3;
                                            gf.VergiTutari = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            topVergi += carpan * Util.ToDecimal(gnlnode.InnerText);
                                            police.GenelBilgiler.PoliceVergis.Add(gf);

                                        }
                                        #endregion

                                        #region Ödeme Planı

                                        if (gnlnode.Name == "tahakkuk")
                                        {
                                            XmlNodeList tks = gnlnode.ChildNodes;

                                            for (int indx = 0; indx < tks.Count; indx++)
                                            {
                                                XmlNode elm = tks.Item(indx);
                                                XmlNodeList taksitelm = elm.ChildNodes;
                                                PoliceOdemePlani odm = new PoliceOdemePlani();
                                                for (int taksitelmindx = 0; taksitelmindx < taksitelm.Count; taksitelmindx++)
                                                {
                                                    XmlNode taksitnodes = taksitelm[taksitelmindx];
                                                    odm.TaksitNo = indx + 1;
                                                    if (taksitnodes.Name == "vade") odm.VadeTarihi = Util.toDate(taksitnodes.InnerText);
                                                    if (taksitnodes.Name == "tutar") odm.TaksitTutari = carpan * Util.ToDecimal(taksitnodes.InnerText);
                                                    if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                    {
                                                        odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["tutar"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                                        odm.DovizliTaksitTutari = Util.ToDecimal(elm["tutar"].InnerText);
                                                    }
                                                    #region Tahsilat işlemi

                                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.GENERALISIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                                            tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
                                                            tahsilat.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                            tahsilat.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
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

                                                    #endregion
                                                    //ödeme tipi ??
                                                    if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                                                    {
                                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                    }
                                                    else
                                                    {
                                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                                    }
                                                }
                                                if (odm.TaksitTutari != 0)
                                                {
                                                    police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                                }
                                            }
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
                                                    if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                                                    {
                                                        odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                    }
                                                    else
                                                    {
                                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                                    }
                                                    odmm.TaksitNo = 1;
                                                    if (odmm.TaksitTutari == null && police.GenelBilgiler.BrutPrim != 0)
                                                    {
                                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                                                        var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.GENERALISIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                            }
                                                        }
                                                    }
                                                }

                                            }
                                        }
                                        #endregion

                                        #region Araç Bilgileri
                                        if (gnlnode.Name == "arac-plakasi")
                                        {
                                            string plaka =  gnlnode.InnerText;
                                            string plakaKodu = "";
                                            string plakaNo = "";
                                            bool plakaKodMu = true;
                                            if (!String.IsNullOrEmpty(plaka))
                                            {
                                                plaka = plaka.Trim();
                                            }
                                            for (int k = 0; k < plaka.Length; k++)
                                            {
                                                if (char.IsDigit(plaka[k]) && plakaKodMu)
                                                {
                                                    plakaKodu += plaka[k];
                                                }
                                                else
                                                {
                                                    plakaKodMu = false;
                                                    plakaNo += plaka[k];
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
                                            police.GenelBilgiler.PoliceArac.PlakaNo = plakaNo;
                                            police.GenelBilgiler.PoliceArac.PlakaKodu = plakaKodu;
                                        }
                                        if (gnlnode.Name == "arac-modeli")
                                        {
                                            police.GenelBilgiler.PoliceArac.AracinTipiAciklama = gnlnode.InnerText;
                                        }
                                        if (gnlnode.Name == "arac-markasi")
                                        {
                                            police.GenelBilgiler.PoliceArac.MarkaAciklama = gnlnode.InnerText;
                                        }
                                        if (gnlnode.Name == "arac-tescil-tarihi" && !String.IsNullOrEmpty(gnlnode.InnerText))
                                        {
                                            police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                        }
                                        if (gnlnode.Name == "trafige-cikis-tarihi" && !String.IsNullOrEmpty(gnlnode.InnerText))
                                        {
                                            police.GenelBilgiler.PoliceArac.TrafikCikisTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                        }
                                        if (gnlnode.Name == "arac-motor-no")
                                        {
                                            police.GenelBilgiler.PoliceArac.MotorNo = gnlnode.InnerText;
                                        }
                                        if (gnlnode.Name == "arac-sasi-no")
                                        {
                                            police.GenelBilgiler.PoliceArac.SasiNo = gnlnode.InnerText;
                                        }
                                        if (gnlnode.Name == "arac-kullanim-tarzi")
                                        {
                                            police.GenelBilgiler.PoliceArac.KullanimTarzi = gnlnode.InnerText;
                                        }
                                        #endregion
                                    }
                                    police.GenelBilgiler.Durum = 0;
                                    police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.GENERALISIGORTA;

                                    // Odeme Sekli
                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;

                                    police.GenelBilgiler.ToplamVergi = topVergi;
                                    #endregion



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
