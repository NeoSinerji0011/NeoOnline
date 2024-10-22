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
    public class HalkXmlReader : IPoliceTransferReader
    {
        IPoliceTransferService _IPoliceTransferService;
        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public HalkXmlReader()
        {
        }

        public HalkXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();

        }

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;
            int carpan = 1;
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
                #region  Police Reader

                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;
                XmlNode a = s.LastChild;

                if (a.HasChildNodes)
                {
                    string str = "Size:" + a.ChildNodes.Count;
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        if (s.ChildNodes[i].Name == "POLICELER")
                        {
                            XmlNode polNode = s.ChildNodes[i];
                            if (polNode.Name == "POLICELER")
                            {
                                string readerKulKodu = null;
                                XmlNodeList pols = polNode.ChildNodes;
                                for (int polsindx = 0; polsindx < pols.Count; polsindx++)
                                {
                                    XmlNode polc = pols[polsindx];
                                    if (polc.Name == "POLICE")
                                    {
                                        Police police = new Police();

                                        //Poliçe İptal durumu belirleniyor.
                                        carpan = 1;
                                        XmlNode iptal = polc.SelectSingleNode("TEMINATLAR/POLICE_TEMINAT/T_I");
                                        if (iptal != null)
                                        {
                                            if (iptal.InnerText == "I")
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
                                        }

                                        #region Genel Bilgiler

                                        XmlNodeList gnlb = polc.ChildNodes;

                                        for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                                        {
                                            XmlNode gnlnode = gnlb[gnlbidx];

                                            if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                            else police.GenelBilgiler.TVMKodu = 0;

                                            if (gnlnode.Name == "CARI_POL_NO") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                                            if (gnlnode.Name == "ZEYL_SIRA_NO") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                            if (gnlnode.Name == "TANZIM_TARIH") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText);
                                            if (gnlnode.Name == "BASLAMA_TARIH") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText);
                                            if (gnlnode.Name == "BITIS_TARIH") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText);
                                            if (gnlnode.Name == "DOVIZ_CINS")
                                            {
                                                police.GenelBilgiler.ParaBirimi = gnlnode.InnerText;
                                                if (police.GenelBilgiler.ParaBirimi == "YTL")
                                                {
                                                    police.GenelBilgiler.ParaBirimi = "TL";
                                                }
                                            }
                                            if (gnlnode.Name == "OP_ID") readerKulKodu = gnlnode.InnerText;
                                            if (readerKulKodu != null)
                                            {
                                                var getReaderKodu = _IPoliceTransferService.GetPoliceReaderKullanicilari(readerKulKodu);
                                                if (getReaderKodu != null)
                                                {
                                                    police.GenelBilgiler.TaliAcenteKodu = Convert.ToInt32(getReaderKodu.AltTvmKodu);

                                                }
                                            }
                                            if (gnlnode.Name == "DOVIZ_KUR")
                                            {
                                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                                {
                                                    police.GenelBilgiler.DovizKur = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                                    if (police.GenelBilgiler.ParaBirimi == "TL")
                                                    {
                                                        police.GenelBilgiler.DovizKur = 1;
                                                    }
                                                }
                                            }


                                            //if (gnlnode.Name == "DOVIZ_KUR")
                                            //{
                                            //    dovizKuru = Util.ToDecimal(polNode.InnerText.Replace(".", ","));
                                            //}

                                            if (gnlnode.Name == "TARIFE_ADI") tumUrunAdi = gnlnode.InnerText;
                                            if (gnlnode.Name == "TARIFE_KOD") tumUrunKodu = gnlnode.InnerText;

                                            if (gnlnode.Name == "YTL_NET_PRIM")
                                            {
                                                police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                polNet = police.GenelBilgiler.NetPrim;
                                                if (police.GenelBilgiler.DovizKur != null && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != 1)
                                                {
                                                    police.GenelBilgiler.DovizliNetPrim = polNet.Value;
                                                    police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * police.GenelBilgiler.DovizKur.Value, 2);
                                                    dovizKuru = 0;
                                                }
                                            }
                                            if (gnlnode.Name == "VERGI") police.GenelBilgiler.ToplamVergi = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            if (gnlnode.Name == "BRUT")
                                            {
                                                police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                polBrutprimim = police.GenelBilgiler.BrutPrim;
                                                if (police.GenelBilgiler.DovizKur != null && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != 1)
                                                {
                                                    police.GenelBilgiler.DovizliBrutPrim = polBrutprimim.Value;
                                                    police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * police.GenelBilgiler.DovizKur.Value, 2);
                                                }
                                            }
                                            if (gnlnode.Name == "DKOM_TUTARI")
                                            {
                                                police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                polKomisyon = police.GenelBilgiler.Komisyon;
                                                if (police.GenelBilgiler.DovizKur != null && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != 1)
                                                {
                                                    police.GenelBilgiler.DovizliKomisyon = polKomisyon.Value;
                                                    police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * police.GenelBilgiler.DovizKur.Value, 2);
                                                }
                                            }
                                            #region Vergiler

                                            decimal topVergi = 0;
                                            if (gnlnode.Name == "VERGILER")
                                            {
                                                PoliceVergi gp = new PoliceVergi();
                                                gp.VergiTutari = 0;
                                                gp.VergiKodu = 3;
                                                PoliceVergi gv = new PoliceVergi();
                                                gv.VergiTutari = 0;
                                                gv.VergiKodu = 2;
                                                PoliceVergi thgf = new PoliceVergi();
                                                thgf.VergiTutari = 0;
                                                thgf.VergiKodu = 1;
                                                PoliceVergi yv = new PoliceVergi();
                                                yv.VergiTutari = 0;
                                                yv.VergiKodu = 4;

                                                XmlNodeList primVergilernode = gnlnode.ChildNodes;

                                                for (int idx = 0; idx < primVergilernode.Count; idx++)
                                                {
                                                    XmlNode elm = primVergilernode.Item(idx);
                                                    if (elm["VERGI_KODU"].InnerText.Trim() == "GP")
                                                    {
                                                        gp.VergiTutari += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }

                                                    if (elm["VERGI_KODU"].InnerText.Trim() == "GV")
                                                    {
                                                        gv.VergiTutari += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }

                                                    if (elm["VERGI_KODU"].InnerText.Trim() == "TF")
                                                    {
                                                        thgf.VergiTutari += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }

                                                    if (elm["VERGI_KODU"].InnerText.Trim() == "YSV")
                                                    {
                                                        yv.VergiTutari += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += carpan * Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }
                                                }
                                                police.GenelBilgiler.PoliceVergis.Add(gp);
                                                police.GenelBilgiler.PoliceVergis.Add(gv);
                                                police.GenelBilgiler.PoliceVergis.Add(thgf);
                                                police.GenelBilgiler.PoliceVergis.Add(yv);
                                                if (police.GenelBilgiler.ToplamVergi == null) police.GenelBilgiler.ToplamVergi = topVergi;
                                            }

                                            #endregion

                                            #region Sigorta Ettiren Bilgileri

                                            if (gnlnode.Name == "POLICE_SIGORTA_ETTIREN")
                                            {
                                                XmlNodeList sigortaEttirenNodes = gnlnode.ChildNodes;

                                                for (int idx = 0; idx < sigortaEttirenNodes.Count; idx++)
                                                {
                                                    XmlNode sigortaEttirenNode = sigortaEttirenNodes.Item(idx);
                                                    if (sigortaEttirenNode.Name == "AD1") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "AD2") police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "VERGI_NO") police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "TC_KIMLIK_NO") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = sigortaEttirenNode.InnerText;
                                                    sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                                    if (sigortaEttirenNode.Name == "MAH_KOY") police.GenelBilgiler.PoliceSigortaEttiren.Mahalle = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "CADDE") police.GenelBilgiler.PoliceSigortaEttiren.Cadde = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "SOKAK") police.GenelBilgiler.PoliceSigortaEttiren.Cadde = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "NO") police.GenelBilgiler.PoliceSigortaEttiren.BinaNo = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "DAIRE") police.GenelBilgiler.PoliceSigortaEttiren.DaireNo = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "SEMT") police.GenelBilgiler.PoliceSigortaEttiren.Semt = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "ULKE_KODU") police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "IL") police.GenelBilgiler.PoliceSigortaEttiren.IlKodu = sigortaEttirenNode.InnerText;
                                                    if (sigortaEttirenNode.Name == "IL_ADI") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = sigortaEttirenNode.InnerText;
                                                    //if (sigortaEttirenNode.Name == "ILCE") police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu =null/* Util.toInt(sigortaEttirenNode.InnerText)*/;
                                                    if (sigortaEttirenNode.Name == "ILCE_ADI") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = sigortaEttirenNode.InnerText;
                                                }
                                            }
                                            #endregion

                                            #region Sigortali Bilgileri

                                            if (gnlnode.Name == "POLICE_SIGORTALI")
                                            {
                                                XmlNodeList sigortalilarNode = gnlnode.ChildNodes;

                                                for (int idx = 0; idx < sigortalilarNode.Count; idx++)
                                                {
                                                    XmlNode sigortaliNode = sigortalilarNode.Item(idx);
                                                    if (sigortaliNode.Name == "AD1") police.GenelBilgiler.PoliceSigortali.AdiUnvan = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "AD2") police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "VERGI_NO") police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "TC_KIMLIK_NO") police.GenelBilgiler.PoliceSigortali.KimlikNo = sigortaliNode.InnerText;
                                                    sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                                    if (sigortaliNode.Name == "MAH_KOY") police.GenelBilgiler.PoliceSigortali.Mahalle = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "CADDE") police.GenelBilgiler.PoliceSigortali.Cadde = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "SOKAK") police.GenelBilgiler.PoliceSigortali.Sokak = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "NO") police.GenelBilgiler.PoliceSigortali.BinaNo = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "SEMT") police.GenelBilgiler.PoliceSigortali.Semt = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "ULKE_KODU") police.GenelBilgiler.PoliceSigortali.UlkeKodu = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "IL") police.GenelBilgiler.PoliceSigortali.IlKodu = sigortaliNode.InnerText;
                                                    if (sigortaliNode.Name == "IL_ADI") police.GenelBilgiler.PoliceSigortali.IlAdi = sigortaliNode.InnerText;
                                                    //if (sigortaliNode.Name == "ILCE") police.GenelBilgiler.PoliceSigortali.IlceKodu = null/*Util.toInt(sigortaliNode.InnerText)*/;
                                                    if (sigortaliNode.Name == "ILCE_ADI") police.GenelBilgiler.PoliceSigortali.IlceAdi = sigortaliNode.InnerText;
                                                    //if (sigortaliNode.Name == "POSTA_KODU") police.GenelBilgiler.PoliceSigortali.PostaKodu = Convert.ToInt32(sigortaliNode.InnerText);
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
                                            if (gnlnode.Name == "TAKSITLER")
                                            {
                                                XmlNodeList taksitlerrnode = gnlnode.ChildNodes;
                                                for (int idx = 0; idx < taksitlerrnode.Count; idx++)
                                                {
                                                    PoliceOdemePlani odm = new PoliceOdemePlani();

                                                    XmlNode odemenodep = taksitlerrnode[idx];
                                                    odm.VadeTarihi = Util.toDate(odemenodep["VADE"].InnerText);
                                                    odm.TaksitTutari = carpan * Util.ToDecimal(odemenodep["TUTAR"].InnerText);
                                                    if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                    {
                                                        odm.TaksitTutari = Math.Round(carpan * Util.ToDecimal(odemenodep["TUTAR"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                                        odm.DovizliTaksitTutari = carpan * Util.ToDecimal(odemenodep["TUTAR"].InnerText);
                                                    }
                                                    odm.TaksitNo = idx + 1;
                                                    if (odemenodep["ODEME_CINSI"].InnerText == "K")
                                                    {
                                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                    }
                                                    else if (odemenodep["ODEME_CINSI"].InnerText == "N")
                                                    {
                                                        odm.OdemeTipi = OdemeTipleri.Nakit;
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
                                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.TURKIYESIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                                        if (odemenodep["ODEME_CINSI"].InnerText == "K")
                                                        {
                                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                            tahsilat.OtomatikTahsilatiKkMi = 1;
                                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilat.TaksitNo = odm.TaksitNo;
                                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
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
                                                        else if (odemenodep["ODEME_CINSI"].InnerText == "N")
                                                        {
                                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                            odm.OdemeTipi = OdemeTipleri.Nakit;
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
                                                }
                                                if (taksitlerrnode.Count == 0)
                                                {

                                                    PoliceOdemePlani odmm = new PoliceOdemePlani();

                                                    if (odmm.TaksitTutari == null)
                                                    {
                                                        odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                                        if (odmm.VadeTarihi == null)
                                                        {
                                                            odmm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                                        }
                                                        odmm.OdemeTipi = OdemeTipleri.Havale;
                                                        odmm.TaksitNo = 1;
                                                        if (odmm.TaksitTutari != 0 && odmm.TaksitTutari != null)
                                                        {
                                                            police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                                                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                            {
                                                                odmm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim.Value;
                                                            }
                                                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.TURKIYESIGORTA, police.GenelBilgiler.BransKodu.Value);
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

                                            #region Teminatlar

                                            if (police.GenelBilgiler.NetPrim == null && police.GenelBilgiler.BrutPrim == null)
                                            {//teminatlar->brütprim,netprim,komisyon için
                                                decimal netprim = 0, komisyon = 0;

                                                if (gnlnode.Name == "TEMINATLAR")
                                                {
                                                    XmlNodeList teminatlarnode = gnlnode.ChildNodes;
                                                    for (int idx = 0; idx < teminatlarnode.Count; idx++)
                                                    {
                                                        XmlNode polteminat = teminatlarnode[idx];
                                                        netprim += Util.ToDecimal(polteminat["NET_PRIM"].InnerText);
                                                        komisyon += Util.ToDecimal(polteminat["KOM_TUTARI"].InnerText);
                                                    }
                                                    police.GenelBilgiler.NetPrim = carpan * netprim;
                                                    if (police.GenelBilgiler.Komisyon == null || police.GenelBilgiler.Komisyon == 0)
                                                    {
                                                        police.GenelBilgiler.Komisyon = carpan * komisyon;
                                                    }
                                                }
                                                police.GenelBilgiler.BrutPrim = police.GenelBilgiler.NetPrim + police.GenelBilgiler.ToplamVergi;
                                            }
                                            else if (police.GenelBilgiler.NetPrim == null)
                                            {//teminatlar->netprim için
                                                decimal netprim = 0;

                                                if (gnlnode.Name == "TEMINATLAR")
                                                {
                                                    XmlNodeList teminatlarnode = gnlnode.ChildNodes;
                                                    for (int idx = 0; idx < teminatlarnode.Count; idx++)
                                                    {
                                                        XmlNode polteminat = teminatlarnode[idx];
                                                        netprim += Util.ToDecimal(polteminat["NET_PRIM"].InnerText);
                                                    }
                                                    police.GenelBilgiler.NetPrim = carpan * netprim;
                                                }
                                            }
                                            else if (police.GenelBilgiler.BrutPrim == null)
                                            {//teminatlar->brütprim için
                                                decimal netprim = 0;

                                                if (gnlnode.Name == "TEMINATLAR")
                                                {
                                                    XmlNodeList teminatlarnode = gnlnode.ChildNodes;
                                                    for (int idx = 0; idx < teminatlarnode.Count; idx++)
                                                    {
                                                        XmlNode polteminat = teminatlarnode[idx];
                                                        netprim += Util.ToDecimal(polteminat["NET_PRIM"].InnerText);
                                                    }
                                                }
                                                police.GenelBilgiler.BrutPrim = police.GenelBilgiler.NetPrim + police.GenelBilgiler.ToplamVergi;
                                            }
                                            if (police.GenelBilgiler.Komisyon == null || police.GenelBilgiler.Komisyon == 0)
                                            {//teminatlar->komisyon için
                                                decimal komisyon = 0;
                                                if (gnlnode.Name == "TEMINATLAR")
                                                {
                                                    XmlNodeList teminatlarnode = gnlnode.ChildNodes;
                                                    for (int idx = 0; idx < teminatlarnode.Count; idx++)
                                                    {
                                                        XmlNode polteminat = teminatlarnode[idx];
                                                        komisyon += Util.ToDecimal(polteminat["KOM_TUTARI"].InnerText);
                                                    }
                                                    police.GenelBilgiler.Komisyon = carpan * komisyon;
                                                }
                                            }
                                            #endregion
                                            #region Araç Bilgileri

                                            if (gnlnode.Name == "BILGILER")
                                            {
                                                XmlNodeList policeBilgiNodeList = gnlnode.ChildNodes; //poliçe bilgi
                                                for (int idxSaha = 0; idxSaha < policeBilgiNodeList.Count; idxSaha++)
                                                {
                                                    // XmlNodeList policeBilgiElemanNodeList = policeBilgiNodeList[idxSaha].ChildNodes;

                                                    XmlNode saha = policeBilgiNodeList[idxSaha];

                                                    if (saha["BILGI_ADI"].InnerText == "MARKA :" || saha["BILGI_ADI"].InnerText == "MARKASI")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.MarkaAciklama = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "MODEL :" || saha["BILGI_ADI"].InnerText == "MODELİ")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.Model = Util.toInt(saha["ACIKLAMA"].InnerText);
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "MOTOR NO :" || saha["BILGI_ADI"].InnerText == "MOTOR NO")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.MotorNo = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "TESCIL SERI :")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TescilSeriNo = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }

                                                    if (saha["BILGI_ADI"].InnerText == "TRAF.TESC.TAR :" || saha["BILGI_ADI"].InnerText == "TRAFİK TES.TRH")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(saha["ACIKLAMA"].InnerText, Util.DateFormat1);
                                                        }

                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "TRAF.CIKS.TAR :")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TrafikCikisTarihi = Util.toDate(saha["ACIKLAMA"].InnerText, Util.DateFormat1);
                                                        }

                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "ARAÇ TIPI :" || saha["BILGI_ADI"].InnerText == "TIP")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.AracinTipiAciklama = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "SASI NO :" || saha["BILGI_ADI"].InnerText == "ŞASİ NO")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.SasiNo = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "PLAKA :" || saha["BILGI_ADI"].InnerText == "PLAKA")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.PlakaNo = saha["ACIKLAMA"].InnerText != "" && saha["ACIKLAMA"].InnerText.Length >= 2 ? saha["ACIKLAMA"].InnerText.Substring(2, saha["ACIKLAMA"].InnerText.Length - 2) : "";
                                                            police.GenelBilgiler.PoliceArac.PlakaKodu = saha["ACIKLAMA"].InnerText != "" && saha["ACIKLAMA"].InnerText.Length >= 2 ? saha["ACIKLAMA"].InnerText.Substring(0, 2) : "";
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "ARAÇ RENGI :" || saha["BILGI_ADI"].InnerText == "RENK")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.Renk = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "SILINDIR HACM :" || saha["BILGI_ADI"].InnerText == "SILINDIR HACMI")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.SilindirHacmi = saha["ACIKLAMA"].InnerText;
                                                        }

                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "SBM TRAMER NO" || saha["BILGI_ADI"].InnerText == "SBMTRAMERNO")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TramerBelgeNo = saha["ACIKLAMA"].InnerText;
                                                        }

                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "KULLANIM TARZ :" || saha["BILGI_ADI"].InnerText == "KULLANIM TARZI")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.KullanimTarzi = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                }

                                            }

                                            #endregion
                                        }
                                        police.GenelBilgiler.Durum = 0;
                                        police.GenelBilgiler.YenilemeNo = 0;
                                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.TURKIYESIGORTA;

                                        //odemesekli
                                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                                        if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;

                                        #endregion

                                        policeler.Add(police);
                                    }
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
        private XmlNode FindNode(XmlNodeList list, string nodeName)
        {
            if (list.Count > 0)
            {
                foreach (XmlNode node in list)
                {
                    if (node.Name.Equals(nodeName)) return node;
                    if (node.HasChildNodes) FindNode(node.ChildNodes, nodeName);
                }
            }
            return null;
        }

        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
    }
}
