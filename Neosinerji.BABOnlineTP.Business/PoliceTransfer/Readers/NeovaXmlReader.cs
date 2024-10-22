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
    public class NeovaXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public NeovaXmlReader()
        {
        }

        public NeovaXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            decimal dovizKuru = 1;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            try
            {
                #region Poliçe Reader

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
                                            if (gnlnode.Name == "DOVIZ_KUR")
                                            {
                                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                                {
                                                    police.GenelBilgiler.DovizKur = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                                }
                                            }
                                            //if (police.GenelBilgiler.ParaBirimi != "YTL")
                                            //{
                                            //    if (polNode.Name == "DOVIZ_KUR") dovizKuru = Util.ToDecimal(polNode.InnerText);
                                            //}  
                                            if (gnlnode.Name == "TARIFE_ADI") tumUrunAdi = gnlnode.InnerText;
                                            if (gnlnode.Name == "TARIFE_KOD") tumUrunKodu = gnlnode.InnerText;
                                            if (gnlnode.Name == "TECDIT_NO") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);

                                            if (gnlnode.Name == "TOPLAM_NET_PRIM") police.GenelBilgiler.NetPrim = Util.ToDecimal(gnlnode.InnerText);
                                            if (gnlnode.Name == "BRUT_PRIM") police.GenelBilgiler.BrutPrim = Util.ToDecimal(gnlnode.InnerText);
                                            if (gnlnode.Name == "TOPLAM_KOMISYON_TL") police.GenelBilgiler.Komisyon = Util.ToDecimal(gnlnode.InnerText);
                                            if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                            {
                                                if (gnlnode.Name == "TOPLAM_NET_PRIM_DOVIZ") police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(gnlnode.InnerText);
                                                if (gnlnode.Name == "BRUT_PRIM_DOVIZ") police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(gnlnode.InnerText);
                                                if (gnlnode.Name == "TOPLAM_KOMISYON_DOVIZ") police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(gnlnode.InnerText);
                                            }
                                            //if (dovizKuru != 0 && dovizKuru != 1)
                                            //{
                                            //    police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * dovizKuru;
                                            //    police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * dovizKuru;
                                            //    police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * dovizKuru;
                                            //}
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
                                                        gp.VergiTutari += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }

                                                    if (elm["VERGI_KODU"].InnerText.Trim() == "GV")
                                                    {
                                                        gv.VergiTutari += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }

                                                    if (elm["VERGI_KODU"].InnerText.Trim() == "TF")
                                                    {
                                                        thgf.VergiTutari += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }

                                                    if (elm["VERGI_KODU"].InnerText.Trim() == "YSV")
                                                    {
                                                        yv.VergiTutari += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        topVergi += Util.ToDecimal(elm["DVERGI"].InnerText);
                                                        continue;
                                                    }
                                                }
                                                police.GenelBilgiler.PoliceVergis.Add(gp);
                                                police.GenelBilgiler.PoliceVergis.Add(gv);
                                                police.GenelBilgiler.PoliceVergis.Add(thgf);
                                                police.GenelBilgiler.PoliceVergis.Add(yv);
                                                police.GenelBilgiler.ToplamVergi = topVergi;

                                            }
                                            #endregion

                                            #region Sigorta Ettiren Bilgileri

                                            if (gnlnode.Name == "POLICE_SIGORTA_ETTIREN")
                                            {
                                                XmlNodeList sigortaettnode = gnlnode.ChildNodes;

                                                for (int idx = 0; idx < sigortaettnode.Count; idx++)
                                                {
                                                    XmlNode sigortalınode = sigortaettnode.Item(idx);
                                                    if (sigortalınode.Name == "AD1") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "AD2") police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "VERGI_NO") police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "TC_KIMLIK_NO") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "MAH_KOY") police.GenelBilgiler.PoliceSigortaEttiren.Mahalle = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "CADDE") police.GenelBilgiler.PoliceSigortaEttiren.Cadde = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "SEMT") police.GenelBilgiler.PoliceSigortaEttiren.Semt = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "ULKE_KODU") police.GenelBilgiler.PoliceSigortaEttiren.UlkeKodu = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "IL") police.GenelBilgiler.PoliceSigortaEttiren.IlKodu = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "IL_ADI") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "ILCE") police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu = Util.toInt(sigortalınode.InnerText);
                                                    if (sigortalınode.Name == "ILCE_ADI") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = sigortalınode.InnerText;
                                                    sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                                                }
                                            }
                                            #endregion

                                            #region Sigortalı Bilgileri

                                            if (gnlnode.Name == "POLICE_SIGORTALI")
                                            {
                                                XmlNodeList sigortalılarnode = gnlnode.ChildNodes;

                                                for (int idx = 0; idx < sigortalılarnode.Count; idx++)
                                                {
                                                    XmlNode sigortalınode = sigortalılarnode.Item(idx);
                                                    if (sigortalınode.Name == "AD1") police.GenelBilgiler.PoliceSigortali.AdiUnvan = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "AD2") police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "VERGI_NO") police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "TC_KIMLIK_NO") police.GenelBilgiler.PoliceSigortali.KimlikNo = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "MAH_KOY") police.GenelBilgiler.PoliceSigortali.Mahalle = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "CADDE") police.GenelBilgiler.PoliceSigortali.Cadde = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "SEMT") police.GenelBilgiler.PoliceSigortali.Semt = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "ULKE_KODU") police.GenelBilgiler.PoliceSigortali.UlkeKodu = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "IL") police.GenelBilgiler.PoliceSigortali.IlKodu = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "IL_ADI") police.GenelBilgiler.PoliceSigortali.IlAdi = sigortalınode.InnerText;
                                                    if (sigortalınode.Name == "ILCE") police.GenelBilgiler.PoliceSigortali.IlceKodu = Util.toInt(sigortalınode.InnerText);
                                                    if (sigortalınode.Name == "ILCE_ADI") police.GenelBilgiler.PoliceSigortali.IlceAdi = sigortalınode.InnerText;
                                                    sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                                }
                                            }
                                            #endregion

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
                                                        odm.TaksitTutari = Util.ToDecimal(odemenodep["TUTAR"].InnerText) *carpan;
                                                        odm.DovizliTaksitTutari = Math.Round(Util.ToDecimal(odemenodep["TUTAR"].InnerText) / police.GenelBilgiler.DovizKur.Value, 2) * carpan;
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
                                                    else if (odm.TaksitTutari == 0 && odm.TaksitNo == 1 && police.GenelBilgiler.BrutPrim != 0)
                                                    {
                                                        odm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                                        if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                        {
                                                            odm.DovizliTaksitTutari = police.GenelBilgiler.BrutPrim.Value;
                                                        }
                                                        if (odm.VadeTarihi == null)
                                                        {
                                                            odm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                                        }

                                                        odm.TaksitNo = 1;
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
                                                        police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                                    }
                                                    #region Tahsilat işlemi
                                                    PoliceGenelBrans PoliceBransEslestir2 = new PoliceGenelBrans();
                                                    PoliceBransEslestir2 = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                                                    police.GenelBilgiler.BransAdi = PoliceBransEslestir2.BransAdi;
                                                    police.GenelBilgiler.BransKodu = PoliceBransEslestir2.BransKodu;
                                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.NEOVASIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                                        if (odemenodep["ODEME_CINSI"].InnerText == "K")
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
                                            }
                                            #endregion

                                            #region Teminatlar

                                            //teminatlar-> brütprim,netprim,komisyon için

                                            //if (gnlnode.Name == "TEMINATLAR")
                                            //{
                                            //    XmlNodeList teminatlarnode = gnlnode.ChildNodes;
                                            //    for (int idx = 0; idx < teminatlarnode.Count; idx++)
                                            //    {
                                            //        XmlNode polteminat = teminatlarnode[idx];
                                            //        netprim += Util.ToDecimal(polteminat["NET_PRIM"].InnerText);
                                            //        komisyon += Util.ToDecimal(polteminat["KOM_TUTARI"].InnerText);
                                            //    }
                                            //    police.GenelBilgiler.NetPrim = carpan * netprim;
                                            //    police.GenelBilgiler.Komisyon = carpan * komisyon;
                                            //}
                                            //police.GenelBilgiler.BrutPrim = police.GenelBilgiler.NetPrim + police.GenelBilgiler.ToplamVergi;

                                            #endregion

                                            #region Araç Bilgileri

                                            if (gnlnode.Name == "BILGILER")
                                            {
                                                XmlNodeList policeBilgiNodeList = gnlnode.ChildNodes; //poliçe bilgi
                                                for (int idxSaha = 0; idxSaha < policeBilgiNodeList.Count; idxSaha++)
                                                {
                                                    // XmlNodeList policeBilgiElemanNodeList = policeBilgiNodeList[idxSaha].ChildNodes;

                                                    XmlNode saha = policeBilgiNodeList[idxSaha];

                                                    if (saha["BILGI_ADI"].InnerText == "MARKA")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.MarkaAciklama = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "MODEL")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.Model = Util.toInt(saha["ACIKLAMA"].InnerText);
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "MOTOR NO")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.MotorNo = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "TESCIL SERI")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TescilSeriKod = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "TESCİL SIRA NO")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TescilSeriNo = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }

                                                    if (saha["BILGI_ADI"].InnerText == "TESCİL TARİHİ")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(saha["ACIKLAMA"].InnerText, Util.DateFormat1);
                                                        }

                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "TİP")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.AracinTipiAciklama = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "ŞASİ NO")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.SasiNo = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "PLAKA NO" || saha["BILGI_ADI"].InnerText == "PLAKA")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.PlakaNo = saha["ACIKLAMA"].InnerText != "" && saha["ACIKLAMA"].InnerText.Length >= 2 ? saha["ACIKLAMA"].InnerText.Substring(2, saha["ACIKLAMA"].InnerText.Length - 2) : "";
                                                            police.GenelBilgiler.PoliceArac.PlakaKodu = saha["ACIKLAMA"].InnerText != "" && saha["ACIKLAMA"].InnerText.Length >= 2 ? saha["ACIKLAMA"].InnerText.Substring(0, 2) : "";
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "ARAÇ RENGİ")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.Renk = saha["ACIKLAMA"].InnerText;
                                                        }
                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "SİLİNDİR HACMİ")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.SilindirHacmi = saha["ACIKLAMA"].InnerText;
                                                        }

                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "TRAMER BELGE NO")
                                                    {
                                                        var aciklama = FindNode(saha.ChildNodes, "ACIKLAMA");

                                                        if (aciklama != null)
                                                        {
                                                            police.GenelBilgiler.PoliceArac.TramerBelgeNo = saha["ACIKLAMA"].InnerText;
                                                        }

                                                    }
                                                    if (saha["BILGI_ADI"].InnerText == "KULLANIM TARZI")
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
                                        //  police.GenelBilgiler.YenilemeNo = 0;
                                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.NEOVASIGORTA;

                                        //odemesekli
                                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                                        if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;
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
