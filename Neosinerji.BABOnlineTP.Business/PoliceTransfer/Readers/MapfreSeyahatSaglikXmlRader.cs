using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    public class MapfreSeyahatSaglikXmlRader : IPoliceTransferReader
    {
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        ITVMService _TVMService;

        public MapfreSeyahatSaglikXmlRader()
        { }
        public MapfreSeyahatSaglikXmlRader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();

        }
        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;

            string tumUrunAdi = null;
            string tumUrunKodu = null;
            decimal dovizKuru = 1;
            string odemeTipim = null;
            string statu;
            int carpan = 1;

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
                        if (s.ChildNodes[i].Name == "PoliceAktarimListesi")
                        {
                            XmlNode polNode = s.ChildNodes[i];
                            if (polNode.Name == "PoliceAktarimListesi")
                            {
                                XmlNodeList pols = polNode.ChildNodes;
                                for (int polsindx = 0; polsindx < pols.Count; polsindx++)
                                {
                                    int odesayac = 0;
                                    XmlNode polc = pols[polsindx];
                                    if (polc.Name == "PoliceAktarimModel")
                                    {
                                        Police police = new Police();
                                        //Poliçe İptal durumu belirleniyor.
                                        carpan = 1;

                                        #region Genel Bilgiler

                                        XmlNodeList gnlb = polc.ChildNodes;
                                        PoliceGenelBilgiler genelBilgiler = new PoliceGenelBilgiler();
                                        for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                                        {
                                            XmlNode gnlnode = gnlb[gnlbidx];
                                            if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                            if (gnlnode.Name == "GenelBilgi")
                                            {
                                                XmlNodeList genelBilgi = gnlnode.ChildNodes;
                                                for (int idx = 0; idx < genelBilgi.Count; idx++)
                                                {
                                                    XmlNode odemenodep = genelBilgi[idx];
                                                    //if (odemenodep.Name == "Polid") police.GenelBilgiler.PoliceNumarasi = odemenodep.InnerText;
                                                    // if (odemenodep.Name == "BransKod") police.GenelBilgiler.PoliceNumarasi = odemenodep.InnerText;
                                                    if (odemenodep.Name == "UrunKod") tumUrunKodu = odemenodep.InnerText;
                                                    if (odemenodep.Name == "PoliceNo") genelBilgiler.PoliceNumarasi = odemenodep.InnerText.Substring(0, Convert.ToInt32(odemenodep.InnerText.Length-1));
                                                    if (odemenodep.Name == "YenilemeNo") genelBilgiler.YenilemeNo = Util.toInt(odemenodep.InnerText);
                                                    if (odemenodep.Name == "PolBasTar") genelBilgiler.PoliceBasTarihi = Util.toDate(odemenodep.InnerText, Util.DateFormat4);
                                                    if (odemenodep.Name == "PolBitTar") genelBilgiler.PoliceBitTarihi = Util.toDate(odemenodep.InnerText, Util.DateFormat4);
                                                    if (odemenodep.Name == "PolTanTar") genelBilgiler.PoliceTanzimTarihi = Util.toDate(odemenodep.InnerText, Util.DateFormat4);
                                                    //if (odemenodep.Name == "SonZeylNo") genelBilgiler.EkNo = Util.toInt(odemenodep.InnerText);
                                                    if (odemenodep.Name == "Status") statu = odemenodep.InnerText;
                                                    if (odemenodep.Name == "DovKod") genelBilgiler.ParaBirimi = odemenodep.InnerText;
                                                    //if (odemenodep.Name == "GrupId") police.GenelBilgiler.PoliceNumarasi = odemenodep.InnerText;
                                                    if (odemenodep.Name == "SigEttirenBil") genelBilgiler.SEUnvan = odemenodep.InnerText;
                                                    if (odemenodep.Name == "AdresBil") genelBilgiler.SEAdres = odemenodep.InnerText;
                                                    if (odemenodep.Name == "GrupZeylNo") genelBilgiler.GrupZeylNo = odemenodep.InnerText;
                                                    if (odemenodep.Name == "VergiNo")
                                                        if (odemenodep.InnerText != null)
                                                        {
                                                            switch (odemenodep.InnerText.Length)
                                                            {
                                                                case 11:
                                                                    genelBilgiler.SETCNo = odemenodep.InnerText;
                                                                    break;
                                                                case 10:
                                                                    genelBilgiler.SEVergiNo = odemenodep.InnerText;
                                                                    break;
                                                            }
                                                        }
                                                    if (odemenodep.Name == "OdeTip") genelBilgiler.OdemeTipi = odemenodep.InnerText;

                                                    if (odemenodep.Name == "Kurlar")
                                                    {
                                                        XmlNode odemeList = gnlnode["Kurlar"];
                                                        XmlNodeList odemes = odemeList.ChildNodes;
                                                        for (int index = 0; index < odemes.Count; index++)
                                                        {
                                                            XmlNode elm = odemes.Item(index);
                                                            if (elm.Name == "Kur")
                                                            {
                                                                if (genelBilgiler.ParaBirimi != "TL")
                                                                {
                                                                    genelBilgiler.ParaBirimi = elm["DovKod"].InnerText;
                                                                    genelBilgiler.DovizKur = Util.ToDecimal(elm["DovKur"].InnerText.Replace('.', ','));
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            if (gnlnode.Name == "ZeylBilgi")
                                            {
                                                //XmlNode zeylList = gnlnode["Zeyl_Data"];
                                                var polZeylDataList = gnlnode.ChildNodes;
                                                for (int polZeylIndex = 0; polZeylIndex < polZeylDataList.Count; polZeylIndex++)
                                                {
                                                    police = new Police();
                                                    police.GenelBilgiler = new PoliceGenel();
                                                    police.GenelBilgiler.PoliceSigortaEttiren = new PoliceSigortaEttiren();
                                                    police.GenelBilgiler.PoliceSigortali = new PoliceSigortali();
                                                    police.GenelBilgiler.PoliceNumarasi = genelBilgiler.PoliceNumarasi;
                                                    police.GenelBilgiler.YenilemeNo = genelBilgiler.YenilemeNo;
                                                    police.GenelBilgiler.BaslangicTarihi = genelBilgiler.PoliceBasTarihi;
                                                    police.GenelBilgiler.BitisTarihi = genelBilgiler.PoliceBitTarihi;
                                                    police.GenelBilgiler.TanzimTarihi = genelBilgiler.PoliceTanzimTarihi;
                                                    police.GenelBilgiler.TUMUrunKodu = tumUrunKodu;
                                                    police.GenelBilgiler.ParaBirimi = genelBilgiler.ParaBirimi;
                                                    police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = genelBilgiler.SEUnvan;
                                                    police.GenelBilgiler.PoliceSigortaEttiren.Adres = genelBilgiler.SEAdres;
                                                    police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = genelBilgiler.SEVergiNo;
                                                    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = genelBilgiler.SETCNo;
                                                    police.GenelBilgiler.TVMKodu = tvmkodu;
                                                    police.GenelBilgiler.GrupZeyilNo = genelBilgiler.GrupZeylNo;
                                                    PoliceGenelBrans PoliceBransEslestirr = new PoliceGenelBrans();
                                                    PoliceBransEslestirr = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                                                    police.GenelBilgiler.BransAdi = PoliceBransEslestirr.BransAdi;
                                                    police.GenelBilgiler.BransKodu = PoliceBransEslestirr.BransKodu;
                                                    for (int c = 0; c < polZeylDataList[polZeylIndex].ChildNodes.Count; c++)
                                                    {
                                                        XmlNodeList polZeylItem = polZeylDataList[polZeylIndex].ChildNodes;
                                                        for (int index = 0; index < polZeylItem.Count; index++)
                                                        {
                                                            XmlNode elm = polZeylItem.Item(index);
                                                            if (elm.Name == "Zeyl")
                                                            {
                                                                police.GenelBilgiler.NetPrim = Util.ToDecimal(elm["NetPrim"].InnerText);
                                                                police.GenelBilgiler.Komisyon = Util.ToDecimal(elm["TlKomTutar"].InnerText);
                                                                police.GenelBilgiler.ToplamVergi = Util.ToDecimal(elm["Bsmv"].InnerText);
                                                                police.GenelBilgiler.BrutPrim = police.GenelBilgiler.NetPrim + police.GenelBilgiler.ToplamVergi;
                                                                if (dovizKuru != 0 && dovizKuru != 1)
                                                                {
                                                                    police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(elm["NetPrim"].InnerText);
                                                                    police.GenelBilgiler.DovizliNetPrim = police.GenelBilgiler.NetPrim;
                                                                    police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(elm["TlKomTutar"].InnerText);
                                                                }
                                                                police.GenelBilgiler.ZeyilAdi = elm["Aciklama"].InnerText;
                                                                var zeylNo = elm["ZeylNo"].InnerText;
                                                                police.GenelBilgiler.EkNo = Convert.ToInt32(zeylNo) - 1;
                                                            }
                                                            // mapfre sey. sağ.  odemeplanı tek taksit olarak alınıyor çunku komısyonu da odemeplanına atıyor
                                                            if (elm.Name == "OdePlan")
                                                            {
                                                                odesayac++;
                                                                PoliceOdemePlani odm = new PoliceOdemePlani();
                                                                XmlNodeList odePlanData = elm.ChildNodes;
                                                                for (int f = 0; f < odePlanData.Count; f++)
                                                                {
                                                                    XmlNodeList odemenodep = odePlanData[f].ChildNodes;
                                                                    if (odesayac == 1)
                                                                    {
                                                                        odesayac++;
                                                                        for (int k = 0; k < odemenodep.Count; k++)
                                                                        {
                                                                            XmlNode ode = odemenodep.Item(k);

                                                                            odm.VadeTarihi = Util.toDate(ode["VadeTar"].InnerText, Util.DateFormat4);
                                                                            odm.TaksitTutari = Util.ToDecimal(ode["Tutar"].InnerText);
                                                                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                                            {
                                                                                odm.TaksitTutari = Math.Round(Util.ToDecimal(ode["Tutar"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                                                                odm.DovizliTaksitTutari = Util.ToDecimal(ode["Tutar"].InnerText);
                                                                            }
                                                                            odm.TaksitNo = k + 1;
                                                                            if (odemeTipim != null)
                                                                            {
                                                                                switch (odemeTipim)
                                                                                {
                                                                                    case "K":
                                                                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                                                        break;
                                                                                    case "N":
                                                                                        odm.OdemeTipi = OdemeTipleri.Nakit;
                                                                                        break;
                                                                                    default:
                                                                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                                                                        break;
                                                                                }

                                                                            }
                                                                            if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                                                                            {
                                                                                #region Tahsilat işlemi

                                                                                if (police.GenelBilgiler.BrutPrim > 0)
                                                                                {
                                                                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.MAPFREDASK, police.GenelBilgiler.BransKodu.Value);
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
                                                                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
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
                                                                                        if (odemeTipim == "K")
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
                                                                                        else if (odemeTipim == "N")
                                                                                        {
                                                                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                                                            odm.OdemeTipi = OdemeTipleri.Nakit;
                                                                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                                                            tahsilat.TaksitNo = odm.TaksitNo;
                                                                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                                                            tahsilat.OdemeBelgeNo = "";
                                                                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                                            tahsilat.OdenenTutar = 0;
                                                                                            tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
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
                                                                                        else
                                                                                        {
                                                                                            PoliceTahsilat tahsilat = new PoliceTahsilat();
                                                                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                                                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                                                                            tahsilat.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                                                            tahsilat.TaksitNo = odm.TaksitNo;
                                                                                            tahsilat.OdemeBelgeTarihi = odm.VadeTarihi;
                                                                                            tahsilat.OdemeBelgeNo = "";
                                                                                            tahsilat.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                                            tahsilat.OdenenTutar = 0;
                                                                                            tahsilat.KalanTaksitTutari = tahsilat.TaksitTutari;
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

                                                                                #endregion
                                                                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                                                                odm = new PoliceOdemePlani();
                                                                            }

                                                                        }
                                                                        if (odemenodep.Count == 0)
                                                                        {
                                                                            PoliceOdemePlani odmm = new PoliceOdemePlani();
                                                                            if (odmm.TaksitTutari == null)
                                                                            {
                                                                                odmm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                                                                if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                                                {
                                                                                    odmm.DovizliTaksitTutari = police.GenelBilgiler.BrutPrim.Value;
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
                                                                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.MAPFREDASK, police.GenelBilgiler.BransKodu.Value);
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
                                                                                                tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
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
                                                                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
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
                                                                }
                                                            }
                                                            if (elm.Name == "SigortaliZeyl")
                                                            {
                                                                MusteriGenelBilgiler musteri = new MusteriGenelBilgiler();
                                                                MusteriAdre adres = new MusteriAdre();
                                                                XmlNodeList sLiZeyl = elm.ChildNodes;
                                                                for (int x = 0; x < sLiZeyl.Count; x++)
                                                                {
                                                                    XmlNode sLi = sLiZeyl.Item(x);
                                                                    police.GenelBilgiler.PoliceSigortali.AdiUnvan = sLi["Ad"].InnerText;
                                                                    police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = sLi["Soyad"].InnerText;
                                                                    police.GenelBilgiler.PoliceSigortali.Cinsiyet = sLi["Cins"].InnerText;
                                                                    police.GenelBilgiler.PoliceSigortali.DogumTarihi = Util.toDate(sLi["DogumTar"].InnerText, Util.DateFormat4);
                                                                    string kimlikNo = sLi["KimlikNo"].InnerText;
                                                                    if (kimlikNo != null)
                                                                    {
                                                                        switch (kimlikNo.Length)
                                                                        {
                                                                            case 11:
                                                                                police.GenelBilgiler.PoliceSigortali.KimlikNo = kimlikNo;
                                                                                break;
                                                                            case 10:
                                                                                police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = kimlikNo;
                                                                                break;
                                                                        }
                                                                    }
                                                                    police.GenelBilgiler.PoliceSigortali.Adres = sLi["Adres"].InnerText;

                                                                    // musteri genel bilgilere kaç tane sigortalı varsa ekliyoruz. 
                                                                    musteri = new MusteriGenelBilgiler();
                                                                    musteri.AdiUnvan = sLi["Ad"].InnerText;
                                                                    musteri.SoyadiUnvan= sLi["Soyad"].InnerText;
                                                                    musteri.Cinsiyet= sLi["Cins"].InnerText;
                                                                    musteri.DogumTarihi = Util.toDate(sLi["DogumTar"].InnerText, Util.DateFormat4);
                                                                    musteri.KimlikNo= sLi["KimlikNo"].InnerText;
                                                                    if(sLi["Uyruk"].InnerText.Trim()!= "TC")
                                                                    {
                                                                        musteri.Uyruk = 1;
                                                                    }
                                                                    else
                                                                    {
                                                                        musteri.Uyruk = 0;
                                                                    }
                                                                    if(police.MusteriBilgiler.Where(w=>w.KimlikNo == musteri.KimlikNo).ToList().Count()== 0)
                                                                    {
                                                                        police.MusteriBilgiler.Add(musteri);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    police.GenelBilgiler.Durum = 0;
                                                    police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.MAPFREGENELYASAM;
                                                    //odemesekli
                                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                                                    if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;

                                                    if (tumUrunAdi == null)
                                                    {
                                                        police.GenelBilgiler.TUMUrunAdi = PoliceBransEslestirr.TUMUrunAdi.Trim();
                                                    }
                                                    else
                                                    {
                                                        police.GenelBilgiler.TUMUrunAdi = tumUrunAdi.Trim();
                                                    }
                                                    if (tumUrunKodu == null)
                                                    {
                                                        police.GenelBilgiler.TUMUrunKodu = PoliceBransEslestirr.TUMUrunKodu;
                                                    }
                                                    else
                                                    {
                                                        police.GenelBilgiler.TUMUrunKodu = tumUrunKodu;
                                                    }
                                                    police.GenelBilgiler.TUMBransAdi = PoliceBransEslestirr.TUMBransAdi;
                                                    police.GenelBilgiler.TUMBransKodu = PoliceBransEslestirr.TUMBransKodu;
                                                    police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA;
                                                    policeler.Add(police);
                                                }
                                            }
                                        }

                                        #endregion
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
        public string getMessage()
        {
            return this.message;
        }
        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }

        public class PoliceGenelBilgiler
        {
            public string PoliceNumarasi { get; set; }
            public string UrunKodu { get; set; }
            public int YenilemeNo { get; set; }
            public DateTime? PoliceBasTarihi { get; set; }
            public DateTime? PoliceBitTarihi { get; set; }
            public DateTime? PoliceTanzimTarihi { get; set; }
            public string SEUnvan { get; set; }
            public string SEAdres { get; set; }
            public string SEVergiNo { get; set; }
            public string SETCNo { get; set; }
            public string OdemeTipi { get; set; }
            public string ParaBirimi { get; set; }
            public decimal DovizKur { get; set; }
            public string zeyilAciklama { get; set; }
            public string GrupZeylNo { get; set; }
        }
    }
}
