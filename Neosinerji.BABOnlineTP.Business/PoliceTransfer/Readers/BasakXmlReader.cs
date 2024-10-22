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
    public class BasakXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;

        public BasakXmlReader()
        {
        }

        public BasakXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            decimal? polKomisyon = null;
            decimal? polNet = null;
            decimal? polBrutprimim = null;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);
                int carpan = 1;
                decimal dovizKuru = 1;
                decimal dovizKuruVegiler = 1;
                XmlNode root = doc.FirstChild;
                XmlNode t = root.NextSibling;
                XmlNode s = t["POLICELER"];


                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        Police police = new Police();
                        XmlNode polNode = s.ChildNodes[i];

                        carpan = 1;
                        XmlNode iptal = polNode.SelectSingleNode("TEMINATLAR/POLICE_TEMINAT/T_I");
                        if (iptal != null)
                        {
                            if (iptal.InnerText == "I")
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

                        #region Genel Bilgiler

                        

                            if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        police.GenelBilgiler.TUMKodu = 0;
                        police.GenelBilgiler.UrunKodu = 0;
                        if (polNode.SelectSingleNode("TARIFE_ADI") != null)
                            tumUrunAdi = polNode["TARIFE_ADI"].InnerText;
                        if (polNode.SelectSingleNode("TARIFE_KOD") != null)
                            tumUrunKodu = polNode["TARIFE_KOD"].InnerText;

                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.BASAKGROUPAMASIGORTA;
                        police.GenelBilgiler.PoliceNumarasi = polNode["CARI_POL_NO"].InnerText;
                        police.GenelBilgiler.EkNo = Util.toInt(polNode["ZEYL_SIRA_NO"].InnerText);
                        police.GenelBilgiler.YenilemeNo = Util.toInt(polNode["ESKI_POL_NO"].InnerText);
                        police.GenelBilgiler.TanzimTarihi = Util.toDate(polNode["TANZIM_TARIH"].InnerText);
                        police.GenelBilgiler.BaslangicTarihi = Util.toDate(polNode["BASLAMA_TARIH"].InnerText);
                        police.GenelBilgiler.BitisTarihi = Util.toDate(polNode["BITIS_TARIH"].InnerText);
                        police.GenelBilgiler.ParaBirimi = polNode["DOVIZ_CINS"].InnerText;
                        if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL")
                        {
                            if (!String.IsNullOrEmpty(polNode["DOVIZ_KUR"].InnerText))
                            {
                                police.GenelBilgiler.DovizKur = Util.ToDecimal(polNode["DOVIZ_KUR"].InnerText.Replace(".", ","));
                            }
                            dovizKuru = Util.ToDecimal(polNode["DOVIZ_KUR"].InnerText.Replace(".", ","));
                            dovizKuruVegiler = Util.ToDecimal(polNode["DOVIZ_KUR"].InnerText.Replace(".", ","));
                        }
                        police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(polNode["BRUT"].InnerText);
                        police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(polNode["TL_NET_PRIM"].InnerText);
                        police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(polNode["KOM_TUTARI"].InnerText);
                        police.GenelBilgiler.ToplamVergi = carpan * Util.ToDecimal(polNode["VERGI"].InnerText);
                        polNet = police.GenelBilgiler.NetPrim;
                        polBrutprimim = police.GenelBilgiler.BrutPrim;
                        polKomisyon = police.GenelBilgiler.Komisyon;

                        if (dovizKuru != 0 && dovizKuru != 1)
                        {
                            police.GenelBilgiler.DovizliBrutPrim = police.GenelBilgiler.BrutPrim.Value;
                            police.GenelBilgiler.DovizliNetPrim = police.GenelBilgiler.NetPrim.Value;
                            police.GenelBilgiler.DovizliKomisyon = police.GenelBilgiler.Komisyon.Value;

                            police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                            police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                            police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                            police.GenelBilgiler.ToplamVergi = Math.Round(police.GenelBilgiler.ToplamVergi.Value * dovizKuru, 2);
                            dovizKuru = 0;
                        }
                        police.GenelBilgiler.Durum = 0;
                        // burayı kontrol et
                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                        //
                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                        #endregion

                        #region Sigortalı ve Sigorta Ettiren Bilgileri
                        XmlNode polSigortalilar = polNode["POLICE_SIGORTALI"];

                        XmlNode polSigortaEttirens = polNode["POLICE_SIGORTA_ETTIREN"];
                        XmlNodeList polSigortaEttiren = polSigortaEttirens.ChildNodes;

                        PoliceSigortali sigortali = null;
                        PoliceSigortaEttiren sigortaEttiren = null;

                        XmlNode elm = polSigortalilar;


                        sigortali = new PoliceSigortali();
                        sigortali.AdiUnvan = elm["AD1"].InnerText;
                        if (elm.SelectSingleNode("AD2") != null)
                            sigortali.SoyadiUnvan = elm["AD2"].InnerText;

                        if (elm.SelectSingleNode("TC_KIMLIK_NO") != null)
                        {
                            sigortali.KimlikNo = elm["TC_KIMLIK_NO"].InnerText;
                        }

                        if (elm.SelectSingleNode("VERGI_NO") != null)
                        {
                            if (elm["VERGI_NO"].InnerText.Length == 10)
                            {
                                sigortali.VergiKimlikNo = elm["VERGI_NO"].InnerText;
                            }
                        }
                        sLiKimlikNo = !String.IsNullOrEmpty(sigortali.KimlikNo) ? sigortali.KimlikNo : sigortali.VergiKimlikNo;

                        sigortali.IlKodu = elm["IL"].InnerText;
                        sigortali.IlAdi = elm["IL_ADI"].InnerText;
                        sigortali.IlceAdi = elm["ILCE_ADI"].InnerText;
                        if (elm.SelectSingleNode("SIFATI") != null)
                            sigortali.Cinsiyet = elm["SIFATI"].InnerText;
                        if (elm.SelectSingleNode("HAN_APT_FAB") != null)
                            sigortali.HanAptFab = elm["HAN_APT_FAB"].InnerText;

                        string SigortaliAdres = "";
                        if (elm.SelectSingleNode("MAH_KOY") != null)
                        {
                            sigortali.Mahalle = elm["MAH_KOY"].InnerText;
                            SigortaliAdres += sigortali.Mahalle;
                        }
                        if (elm.SelectSingleNode("SOKAK") != null)
                            SigortaliAdres += " " + sigortali.Adres;
                        if (elm.SelectSingleNode("NO") != null)
                            SigortaliAdres += " No:" + elm["NO"].InnerText;
                        if (elm.SelectSingleNode("DAIRE") != null)
                        {
                            sigortali.DaireNo = elm["DAIRE"].InnerText;
                            SigortaliAdres += " Daire:" + sigortali.DaireNo;
                        }
                        sigortali.Adres = SigortaliAdres + " " + sigortali.IlAdi + " / " + sigortali.IlceAdi;
                        police.GenelBilgiler.PoliceSigortali = sigortali;

                        XmlNode elms = polSigortaEttirens;


                        sigortaEttiren = new PoliceSigortaEttiren();
                        sigortaEttiren.AdiUnvan = elms["AD1"].InnerText;
                        if (elms.SelectSingleNode("AD2") != null)
                            sigortaEttiren.SoyadiUnvan = elms["AD2"].InnerText;
                        if (elms.SelectSingleNode("TC_KIMLIK_NO") != null)
                            sigortaEttiren.KimlikNo = elms["TC_KIMLIK_NO"].InnerText;
                        if (elms.SelectSingleNode("VERGI_NO") != null)
                        {
                            if (elms["VERGI_NO"].InnerText.Length == 10)
                            {
                                sigortaEttiren.VergiKimlikNo = elms["VERGI_NO"].InnerText;
                            }
                        }
                        sEttirenKimlikNo = !String.IsNullOrEmpty(sigortaEttiren.KimlikNo) ? sigortaEttiren.KimlikNo : sigortaEttiren.VergiKimlikNo;
                        sigortaEttiren.IlKodu = elms["IL"].InnerText;
                        sigortaEttiren.IlAdi = elms["IL_ADI"].InnerText;
                        sigortaEttiren.IlceAdi = elms["ILCE_ADI"].InnerText;
                        if (elms.SelectSingleNode("HAN_APT_FAB") != null)
                            sigortaEttiren.HanAptFab = elms["HAN_APT_FAB"].InnerText;
                        if (elms.SelectSingleNode("SIFATI") != null)
                            sigortaEttiren.Cinsiyet = elms["SIFATI"].InnerText;
                        string SigortaEttirenAdres = "";
                        if (elms.SelectSingleNode("MAH_KOY") != null)
                        {
                            sigortaEttiren.Mahalle = elms["MAH_KOY"].InnerText;
                            SigortaEttirenAdres += elms["MAH_KOY"].InnerText;
                        }
                        if (elms.SelectSingleNode("SOKAK") != null)
                            SigortaEttirenAdres += " " + elms["SOKAK"].InnerText;
                        if (elms.SelectSingleNode("NO") != null)
                            SigortaEttirenAdres += " No:" + elms["NO"].InnerText;

                        if (elms.SelectSingleNode("DAIRE") != null)
                        {
                            sigortaEttiren.DaireNo = elms["DAIRE"].InnerText;
                            SigortaEttirenAdres += " Daire:" + elms["DAIRE"].InnerText;
                        }
                        sigortaEttiren.Adres = SigortaEttirenAdres + " " + sigortaEttiren.IlAdi + " / " + sigortaEttiren.IlceAdi;
                        police.GenelBilgiler.PoliceSigortaEttiren = sigortaEttiren;


                        if (sigortali == null && sigortaEttiren != null)
                        {
                            police.GenelBilgiler.PoliceSigortali.AdiUnvan = police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan;
                            police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan;
                            police.GenelBilgiler.PoliceSigortali.KimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                            police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            police.GenelBilgiler.PoliceSigortali.IlKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlKodu;
                            police.GenelBilgiler.PoliceSigortali.IlAdi = police.GenelBilgiler.PoliceSigortaEttiren.IlAdi;
                            police.GenelBilgiler.PoliceSigortali.IlceKodu = police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu;
                            police.GenelBilgiler.PoliceSigortali.IlceAdi = police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi;
                            police.GenelBilgiler.PoliceSigortali.Semt = police.GenelBilgiler.PoliceSigortaEttiren.Semt;
                            police.GenelBilgiler.PoliceSigortali.Adres = police.GenelBilgiler.PoliceSigortaEttiren.Cadde;
                            police.GenelBilgiler.PoliceSigortali.HanAptFab = police.GenelBilgiler.PoliceSigortaEttiren.HanAptFab;
                            police.GenelBilgiler.PoliceSigortali.DaireNo = police.GenelBilgiler.PoliceSigortaEttiren.DaireNo;
                            police.GenelBilgiler.PoliceSigortali.Mahalle = police.GenelBilgiler.PoliceSigortaEttiren.Mahalle;
                            police.GenelBilgiler.PoliceSigortali.PostaKodu = police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu;
                            police.GenelBilgiler.PoliceSigortali.MobilTelefonNo = police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo;
                            police.GenelBilgiler.PoliceSigortali.TelefonNo = police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo;
                            police.GenelBilgiler.PoliceSigortali.DogumTarihi = police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi;
                            police.GenelBilgiler.PoliceSigortali.Cinsiyet = police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet;
                        }
                        else if (sigortaEttiren == null && sigortali != null)
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = police.GenelBilgiler.PoliceSigortali.AdiUnvan;
                            police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = police.GenelBilgiler.PoliceSigortali.SoyadiUnvan;
                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = police.GenelBilgiler.PoliceSigortali.KimlikNo;
                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                            police.GenelBilgiler.PoliceSigortaEttiren.IlKodu = police.GenelBilgiler.PoliceSigortali.IlKodu;
                            police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = police.GenelBilgiler.PoliceSigortali.IlAdi;
                            police.GenelBilgiler.PoliceSigortaEttiren.IlceKodu = police.GenelBilgiler.PoliceSigortali.IlceKodu;
                            police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = police.GenelBilgiler.PoliceSigortali.IlceAdi;
                            police.GenelBilgiler.PoliceSigortaEttiren.Semt = police.GenelBilgiler.PoliceSigortali.Semt;
                            police.GenelBilgiler.PoliceSigortaEttiren.Adres = police.GenelBilgiler.PoliceSigortali.Cadde;
                            police.GenelBilgiler.PoliceSigortaEttiren.HanAptFab = police.GenelBilgiler.PoliceSigortali.HanAptFab;
                            police.GenelBilgiler.PoliceSigortaEttiren.DaireNo = police.GenelBilgiler.PoliceSigortali.DaireNo;
                            police.GenelBilgiler.PoliceSigortaEttiren.Mahalle = police.GenelBilgiler.PoliceSigortali.Mahalle;
                            police.GenelBilgiler.PoliceSigortaEttiren.PostaKodu = police.GenelBilgiler.PoliceSigortali.PostaKodu;
                            police.GenelBilgiler.PoliceSigortaEttiren.MobilTelefonNo = police.GenelBilgiler.PoliceSigortali.MobilTelefonNo;
                            police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = police.GenelBilgiler.PoliceSigortali.TelefonNo;
                            police.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = police.GenelBilgiler.PoliceSigortali.DogumTarihi;
                            police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = police.GenelBilgiler.PoliceSigortali.Cinsiyet;
                        }

                        #endregion

                        #region Odeme Plani

                        XmlNode tk = polNode["TAKSITLER"];
                        XmlNodeList tks = tk.ChildNodes;

                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elmm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            //if (odemeTipi == "KRK") odm.OdemeTipi = OdemeTipleri.KrediKarti;  // Kredi karti 
                            //else if (odemeTipi == "BLK") odm.OdemeTipi = OdemeTipleri.KrediKarti;
                            //else odm.OdemeTipi = OdemeTipleri.Nakit; //Nakit

                            odm.TaksitNo = indx + 1;
                            odm.VadeTarihi = Util.toDate(elmm["VADE"].InnerText);
                            odm.TaksitTutari = carpan * Util.ToDecimal(elmm["TUTAR"].InnerText);
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                odm.TaksitTutari = Math.Round(carpan *  Util.ToDecimal(elmm["TUTAR"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                odm.DovizliTaksitTutari = carpan *  Util.ToDecimal(elmm["TUTAR"].InnerText);
                            }
                            string taksitTuru = elmm["ODEME_CINSI"].InnerText;
                            if (taksitTuru == "N")
                            {
                                odm.OdemeTipi = OdemeTipleri.Nakit;
                            }
                            else if (taksitTuru == "K")
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
                            else if (odm.TaksitTutari == 0 && police.GenelBilgiler.BrutPrim != 0 && odm.TaksitNo == 1)
                            {
                                odm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                {
                                    odm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim.Value;
                                }
                                if (odm.VadeTarihi == null)
                                {
                                    odm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                }

                                odm.TaksitNo = 1;
                                if (taksitTuru == "N")
                                {
                                    odm.OdemeTipi = OdemeTipleri.Nakit;
                                }
                                else if (taksitTuru == "K")
                                {
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                }
                                else if (taksitTuru == null)
                                {
                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                }
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            #region Tahsilat işlemi
                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.BASAKGROUPAMASIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                if (tvmkodu == 136)
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

                                    if (taksitTuru == "N")
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
                                    else if (taksitTuru == "K")
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
                                        // tahsilat.OdemeBelgeNo = "1111111";
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
                            }


                            #endregion

                        }
                        if (tks.Count == 1)
                            police.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;//Peşin
                        else
                            police.GenelBilgiler.OdemeSekli = OdemeSekilleri.Vadeli;//Vadeli


                        #endregion

                        #region Teminatlar

                        if (police.GenelBilgiler.NetPrim == null && police.GenelBilgiler.BrutPrim == null)
                        {//teminatlar->brütprim,netprim,komisyon için
                            decimal netprim = 0, komisyon = 0;

                            if (polNode.Name == "TEMINATLAR")
                            {
                                XmlNodeList teminatlarnode = polNode.ChildNodes;
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

                            if (polNode.Name == "TEMINATLAR")
                            {
                                XmlNodeList teminatlarnode = polNode.ChildNodes;
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

                            if (polNode.Name == "TEMINATLAR")
                            {
                                XmlNodeList teminatlarnode = polNode.ChildNodes;
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
                            if (polNode.Name == "TEMINATLAR")
                            {
                                XmlNodeList teminatlarnode = polNode.ChildNodes;
                                for (int idx = 0; idx < teminatlarnode.Count; idx++)
                                {
                                    XmlNode polteminat = teminatlarnode[idx];
                                    komisyon += Util.ToDecimal(polteminat["KOM_TUTARI"].InnerText);
                                }
                                police.GenelBilgiler.Komisyon = carpan * komisyon;
                            }
                        }
                        #endregion

                        #region Araç Bilgileri oto

                        //Kasko veya Trafik ürünü araç bilgileri
                        XmlNode gnlbilgiler = polNode["BILGILER"];
                        XmlNodeList gnlb = gnlbilgiler.ChildNodes;
                        for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlb[gnlbidx];
                            switch (gnlnode["BILGI_ADI"].InnerText)
                            {
                                case "PLAKA":
                                    police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                    police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                    string plakaNo = gnlnode["ACIKLAMA"].InnerText;
                                    for (int k = 0; k < plakaNo.Length; k++)
                                    {
                                        if (k < 2)
                                        {
                                            police.GenelBilgiler.PoliceArac.PlakaKodu += plakaNo[k].ToString();
                                        }
                                        if (k > 2)
                                        {
                                            police.GenelBilgiler.PoliceArac.PlakaNo += plakaNo[k].ToString();
                                        }
                                    }
                                    break;
                                case "ARAÇ MARKASI":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.MarkaAciklama = gnlnode["ACIKLAMA"].InnerText;
                                    if (gnlnode.SelectSingleNode("IST_KOD") != null)
                                        police.GenelBilgiler.PoliceArac.Marka = gnlnode["IST_KOD"].InnerText;
                                    break;
                                case "ARAÇ TİPİ":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.AracinTipiAciklama = gnlnode["ACIKLAMA"].InnerText;
                                    if (gnlnode.SelectSingleNode("IST_KOD") != null)
                                        police.GenelBilgiler.PoliceArac.AracinTipiKodu = gnlnode["IST_KOD"].InnerText;
                                    break;
                                case "MODEL YILI":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(gnlnode["ACIKLAMA"].InnerText) ? Convert.ToInt32(gnlnode["ACIKLAMA"].InnerText) : 0;
                                    break;
                                case "SÜRÜCÜ+YOLCU":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.KoltukSayisi = !String.IsNullOrEmpty(gnlnode["ACIKLAMA"].InnerText) ? Convert.ToInt32(gnlnode["ACIKLAMA"].InnerText) : 0;
                                    break;
                                case "KULLANIM TARZI":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.KullanimTarzi = gnlnode["ACIKLAMA"].InnerText;
                                    break;
                                case "MOTOR NO  ":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.MotorNo = gnlnode["ACIKLAMA"].InnerText;
                                    break;
                                case "ŞASİ NO":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.SasiNo = gnlnode["ACIKLAMA"].InnerText;
                                    break;
                                case "TESCİL TARİHİ":
                                    if (gnlnode.SelectSingleNode("ACIKLAMA") != null)
                                        police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(gnlnode["ACIKLAMA"].InnerText);
                                    break;
                                default:
                                    break;
                            }
                        }

                        #endregion

                        #region Vergiler



                        decimal topVergi = 0;
                        if (polNode.Name == "VERGILER")
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

                            XmlNodeList primVergilernode = polNode.ChildNodes;

                            for (int idx = 0; idx < primVergilernode.Count; idx++)
                            {
                                XmlNode elmm = primVergilernode.Item(idx);
                                if (elm["VERGI_KODU"].InnerText.Trim() == "GP")
                                {
                                    gp.VergiTutari += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    topVergi += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    continue;
                                }

                                if (elm["VERGI_KODU"].InnerText.Trim() == "GV")
                                {
                                    gv.VergiTutari += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    topVergi += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    continue;
                                }

                                if (elm["VERGI_KODU"].InnerText.Trim() == "TF")
                                {
                                    thgf.VergiTutari += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    topVergi += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    continue;
                                }

                                if (elm["VERGI_KODU"].InnerText.Trim() == "YSV")
                                {
                                    yv.VergiTutari += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    topVergi += carpan * Util.ToDecimal(elmm["DVERGI"].InnerText);
                                    continue;
                                }
                            }
                            police.GenelBilgiler.PoliceVergis.Add(gp);
                            police.GenelBilgiler.PoliceVergis.Add(gv);
                            police.GenelBilgiler.PoliceVergis.Add(thgf);
                            police.GenelBilgiler.PoliceVergis.Add(yv);
                            if (police.GenelBilgiler.ToplamVergi == null) police.GenelBilgiler.ToplamVergi = topVergi;
                        }


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
                        #endregion



                        police.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                        police.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;
                        policeler.Add(police);
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
