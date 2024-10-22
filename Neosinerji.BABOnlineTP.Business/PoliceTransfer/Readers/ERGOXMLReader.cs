using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class ERGOXMLReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;

        public ERGOXMLReader()
        { }

        public ERGOXMLReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
                decimal dovizKuru = 1;
                decimal dovizKuruVegiler = 1;
                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        Police police = new Police();
                        XmlNode polNode = s.ChildNodes[i];

                        #region Genel Bilgiler

                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        police.GenelBilgiler.TUMKodu = 0;
                        police.GenelBilgiler.UrunKodu = 0;
                        tumUrunKodu = polNode["brans-kod"].InnerText;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ERGOISVICRESIGORTA;
                        police.GenelBilgiler.PoliceNumarasi = polNode["police-no"].InnerText;
                        police.GenelBilgiler.EkNo = Util.toInt(polNode["zeyil-no"].InnerText);
                        police.GenelBilgiler.YenilemeNo = Util.toInt(polNode["yenileme-no"].InnerText);
                        police.GenelBilgiler.TanzimTarihi = Util.toDate(polNode["tanzim-tarihi"].InnerText);
                        police.GenelBilgiler.BaslangicTarihi = Util.toDate(polNode["baslangic-tarihi"].InnerText);
                        police.GenelBilgiler.BitisTarihi = Util.toDate(polNode["bitis-tarihi"].InnerText);
                        police.GenelBilgiler.ParaBirimi = polNode["kur-kod"].InnerText;                    
                        if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL")
                        {
                            if (!String.IsNullOrEmpty(polNode["kur"].InnerText))
                            {
                                police.GenelBilgiler.DovizKur = Util.ToDecimal(polNode["kur"].InnerText.Replace(".", ","));
                            }
                            dovizKuru = Util.ToDecimal(polNode["kur"].InnerText.Replace(".", ","));
                            dovizKuruVegiler = Util.ToDecimal(polNode["kur"].InnerText.Replace(".", ","));
                        }

                        decimal brutprim = Util.ToDecimal(polNode["toplam-vergi"].InnerText) + Util.ToDecimal(polNode["net-prim"].InnerText);
                        police.GenelBilgiler.BrutPrim = brutprim;
                        police.GenelBilgiler.NetPrim = Util.ToDecimal(polNode["net-prim"].InnerText);
                        police.GenelBilgiler.Komisyon = Util.ToDecimal(polNode["toplam-komisyon"].InnerText);
                        police.GenelBilgiler.ToplamVergi = Util.ToDecimal(polNode["toplam-vergi"].InnerText);
                        polNet = police.GenelBilgiler.NetPrim;
                        polBrutprimim = police.GenelBilgiler.BrutPrim;
                        polKomisyon = police.GenelBilgiler.Komisyon;

                        if (dovizKuru != 0 && dovizKuru != 1)
                        {
                            police.GenelBilgiler.DovizliBrutPrim = police.GenelBilgiler.BrutPrim.Value;
                            police.GenelBilgiler.DovizliNetPrim = police.GenelBilgiler.NetPrim.Value;
                            police.GenelBilgiler.DovizliKomisyon = police.GenelBilgiler.Komisyon.Value;

                            police.GenelBilgiler.BrutPrim = brutprim * dovizKuru;
                            police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value,2);
                            police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru,2);
                            police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru,2);
                            police.GenelBilgiler.ToplamVergi = Math.Round(police.GenelBilgiler.ToplamVergi.Value * dovizKuru, 2);
                            dovizKuru = 0;
                        }
                        police.GenelBilgiler.Durum = 0;

                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                        #endregion

                        #region Sigortalı ve Sigorta Ettiren Bilgileri
                        XmlNode adresler = polNode["adresler"];
                        XmlNodeList adres = adresler.ChildNodes;
                        string adresTuru = String.Empty;

                        PoliceSigortali sigortali = null;
                        PoliceSigortaEttiren sigortaEttiren = null;

                        for (int idx = 0; idx < adres.Count; idx++)
                        {
                            XmlNode elm = adres.Item(idx);

                            adresTuru = elm["adres-turu"].InnerText;

                            if (adresTuru == "S") //Sigortalı
                            {
                                sigortali = new PoliceSigortali();
                                sigortali.AdiUnvan = elm["musteri-ad"].InnerText;
                                sigortali.SoyadiUnvan = elm["musteri-soyad"].InnerText;
                                sigortali.KimlikNo = elm["tckimlik-no"].InnerText;

                                if (elm["vergi-no"].InnerText.Length == 10)
                                {
                                    sigortali.VergiKimlikNo = elm["vergi-no"].InnerText;
                                }
                                sLiKimlikNo = !String.IsNullOrEmpty(sigortali.KimlikNo) ? sigortali.KimlikNo : sigortali.VergiKimlikNo;
                                sigortali.IlKodu = elm["il-kod"].InnerText;
                                sigortali.IlAdi = elm["il-adi"].InnerText;
                                sigortali.IlceKodu = Util.toInt(elm["ilce-kod"].InnerText);
                                sigortali.IlceAdi = elm["ilce-adi"].InnerText;
                                sigortali.Semt = elm["semt-adi"].InnerText;
                                sigortali.Adres = elm["cadde-adi"].InnerText;
                                sigortali.HanAptFab = elm["apartman-adi"].InnerText + elm["han-adi"].InnerText;
                                sigortali.DaireNo = elm["kapi-no"].InnerText;
                                sigortali.Mahalle = elm["mahalle"].InnerText;
                                sigortali.PostaKodu = Util.toInt(elm["posta-kod"].InnerText);
                                sigortali.MobilTelefonNo = elm["cep-tel"].InnerText;
                                sigortali.TelefonNo = elm["telefon"].InnerText;
                                sigortali.DogumTarihi = Util.toDate(elm["musteri-dogtar"].InnerText);
                                sigortali.Cinsiyet = elm["musteri-cinsiyet"].InnerText;
                                police.GenelBilgiler.PoliceSigortali = sigortali;
                            }
                            if (adresTuru == "E") //Sigorta Ettiren 
                            {
                                sigortaEttiren = new PoliceSigortaEttiren();
                                sigortaEttiren.AdiUnvan = elm["musteri-ad"].InnerText;
                                sigortaEttiren.SoyadiUnvan = elm["musteri-soyad"].InnerText;
                                sigortaEttiren.KimlikNo = elm["tckimlik-no"].InnerText;

                                if (elm["vergi-no"].InnerText.Length == 10)
                                {
                                    sigortaEttiren.VergiKimlikNo = elm["vergi-no"].InnerText;
                                }
                                sEttirenKimlikNo = !String.IsNullOrEmpty(sigortaEttiren.KimlikNo) ? sigortaEttiren.KimlikNo : sigortaEttiren.VergiKimlikNo;
                                sigortaEttiren.IlKodu = elm["il-kod"].InnerText;
                                sigortaEttiren.IlAdi = elm["il-adi"].InnerText;
                                sigortaEttiren.IlceKodu = Util.toInt(elm["ilce-kod"].InnerText);
                                sigortaEttiren.IlceAdi = elm["ilce-adi"].InnerText;
                                sigortaEttiren.Semt = elm["semt-adi"].InnerText;
                                sigortaEttiren.Adres = elm["cadde-adi"].InnerText;
                                sigortaEttiren.HanAptFab = elm["apartman-adi"].InnerText + elm["han-adi"].InnerText;
                                sigortaEttiren.DaireNo = elm["kapi-no"].InnerText;
                                sigortaEttiren.Mahalle = elm["mahalle"].InnerText;
                                sigortaEttiren.PostaKodu = Util.toInt(elm["posta-kod"].InnerText);
                                sigortaEttiren.MobilTelefonNo = elm["cep-tel"].InnerText;
                                sigortaEttiren.TelefonNo = elm["telefon"].InnerText;
                                sigortaEttiren.DogumTarihi = Util.toDate(elm["musteri-dogtar"].InnerText);
                                sigortaEttiren.Cinsiyet = elm["musteri-cinsiyet"].InnerText;
                                police.GenelBilgiler.PoliceSigortaEttiren = sigortaEttiren;
                            }
                        }
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

                        XmlNode tk = polNode["taksitler"];
                        XmlNodeList tks = tk.ChildNodes;

                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            string odemeTipi = elm["taksit-turu"].InnerText;
                            //if (odemeTipi == "KRK") odm.OdemeTipi = OdemeTipleri.KrediKarti;  // Kredi karti 
                            //else if (odemeTipi == "BLK") odm.OdemeTipi = OdemeTipleri.KrediKarti;
                            //else odm.OdemeTipi = OdemeTipleri.Nakit; //Nakit

                            odm.TaksitNo = indx + 1;
                            odm.VadeTarihi = Util.toDate(elm["taksit-tarihi"].InnerText);
                            odm.TaksitTutari = Util.ToDecimal(elm["taksit-tutari"].InnerText);
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["taksit-tutari"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                odm.DovizliTaksitTutari = Util.ToDecimal(elm["taksit-tutari"].InnerText);
                            }
                            string taksitTuru = elm["taksit-turu"].InnerText;
                            if (taksitTuru == "NAK")
                            {
                                odm.OdemeTipi = OdemeTipleri.Nakit;
                            }
                            else if (taksitTuru == "KRK")
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
                                if (taksitTuru == "NAK")
                                {
                                    odm.OdemeTipi = OdemeTipleri.Nakit;
                                }
                                else if (taksitTuru == "KRK")
                                { odm.OdemeTipi = OdemeTipleri.KrediKarti; }
                                else if (taksitTuru == null) { odm.OdemeTipi = OdemeTipleri.Havale; }
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            #region Tahsilat işlemi
                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ERGOISVICRESIGORTA, police.GenelBilgiler.BransKodu.Value);
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

                                    if (taksitTuru == "NAK")
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
                                    else if (taksitTuru == "KRK")
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

                        #region Vergiler

                        //Trafik hizmetleri gelilstirme fonu- THGF
                        PoliceVergi thgf = new PoliceVergi();
                        thgf.VergiKodu = 1;
                        thgf.VergiTutari = Util.ToDecimal(polNode["gelistirme-fonu-bedel"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(thgf);

                        //GiderVergisi - BSMV
                        PoliceVergi gv = new PoliceVergi();
                        gv.VergiKodu = 2;
                        gv.VergiTutari = Util.ToDecimal(polNode["gider-vergisi"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(gv);

                        //Garanti Fonu- GF
                        PoliceVergi gf = new PoliceVergi();
                        gf.VergiKodu = 3;
                        gf.VergiTutari = Util.ToDecimal(polNode["garanti-fonu-bedel"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(gf);

                        //YSV
                        PoliceVergi ysv = new PoliceVergi();
                        ysv.VergiKodu = 4;
                        ysv.VergiTutari = Util.ToDecimal(polNode["ysv-bedel"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(ysv);



                        if (dovizKuruVegiler != 0 && dovizKuruVegiler != 1)
                        {
                            if (thgf.VergiTutari.HasValue)
                            {
                                thgf.VergiTutari = Math.Round(thgf.VergiTutari.Value * dovizKuruVegiler, 2);
                            }
                            if (gv.VergiTutari.HasValue)
                            {
                                gv.VergiTutari = Math.Round(gv.VergiTutari.Value * dovizKuruVegiler, 2);
                            }
                            if (gf.VergiTutari.HasValue)
                            {
                                gf.VergiTutari = Math.Round(gf.VergiTutari.Value * dovizKuruVegiler, 2);
                            }
                            if (ysv.VergiTutari.HasValue)
                            {
                                ysv.VergiTutari = Math.Round(ysv.VergiTutari.Value * dovizKuruVegiler, 2);
                            }
                            dovizKuruVegiler = 0;
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

                        police.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                        police.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;
                        policeler.Add(police);

                        #endregion

                        #region Araç Bilgileri oto

                        //Kasko veya Trafik ürünü araç bilgileri
                        XmlNodeList gnlb = polNode["oto"].ChildNodes;
                        for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlb[gnlbidx];
                            if (gnlnode.Name == "marka-kod")
                            {
                                police.GenelBilgiler.PoliceArac.Marka = gnlnode.InnerText;
                            }

                            if (gnlnode.Name == "marka-adi")
                            {
                                police.GenelBilgiler.PoliceArac.MarkaAciklama = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "tip-kod")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiKodu = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "tip-adi")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiAciklama = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "alttip-kod")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiKodu2 = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "alttip-adi")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiAciklama2 = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "model")
                            {
                                police.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(gnlnode.InnerText) ? Convert.ToInt32(gnlnode.InnerText) : 0;
                            }
                            if (gnlnode.Name == "plaka" && !String.IsNullOrEmpty(gnlnode.InnerText))
                            {
                                police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                string plakaNo = gnlnode.InnerText;
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
                            }
                            if (gnlnode.Name == "yolcu-adedi")
                            {
                                police.GenelBilgiler.PoliceArac.KoltukSayisi = !String.IsNullOrEmpty(gnlnode.InnerText) ? Convert.ToInt32(gnlnode.InnerText) : 0;
                            }
                            if (gnlnode.Name == "tarz")
                            {
                                police.GenelBilgiler.PoliceArac.KullanimTarzi = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "motor-no")
                            {
                                police.GenelBilgiler.PoliceArac.MotorNo = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "sasi-no")
                            {
                                police.GenelBilgiler.PoliceArac.SasiNo = gnlnode.InnerText;
                            }

                            if (polNode.Name == "oto" && gnlnode.Name == "tescil-tarihi")
                            {
                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                {
                                    police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                }
                            }
                            if (polNode.Name == "oto" && gnlnode.Name == "tramer-belge-no")
                            {
                                police.GenelBilgiler.PoliceArac.TramerBelgeNo = gnlnode.InnerText;
                            }
                            if (polNode.Name == "oto" && gnlnode.Name == "tramer-belge-tarihi")
                            {
                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                {
                                    police.GenelBilgiler.PoliceArac.TramerBelgeTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                }
                            }
                        }

                        #endregion
                        #region Araç Bilgileri kasko

                        //Kasko veya Trafik ürünü araç bilgileri
                        //XmlNodeList gnlb = polNode["oto"].ChildNodes;
                        XmlNodeList gnlbKasko = polNode["kasko"].ChildNodes;
                        for (int gnlbidx = 0; gnlbidx < gnlbKasko.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlbKasko[gnlbidx];
                            if (gnlnode.Name == "marka-kod")
                            {
                                police.GenelBilgiler.PoliceArac.Marka = gnlnode.InnerText;
                            }

                            if (gnlnode.Name == "marka-adi")
                            {
                                police.GenelBilgiler.PoliceArac.MarkaAciklama = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "tip-kod")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiKodu = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "tip-adi")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiAciklama = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "alttip-kod")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiKodu2 = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "alttip-adi")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiAciklama2 = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "model")
                            {
                                police.GenelBilgiler.PoliceArac.Model = !String.IsNullOrEmpty(gnlnode.InnerText) ? Convert.ToInt32(gnlnode.InnerText) : 0;
                            }
                            if (gnlnode.Name == "plaka" && !String.IsNullOrEmpty(gnlnode.InnerText))
                            {
                                police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                string plakaNo = gnlnode.InnerText;
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
                            }
                            if (gnlnode.Name == "yolcu-adedi")
                            {
                                police.GenelBilgiler.PoliceArac.KoltukSayisi = !String.IsNullOrEmpty(gnlnode.InnerText) ? Convert.ToInt32(gnlnode.InnerText) : 0;
                            }
                            if (gnlnode.Name == "tarz")
                            {
                                police.GenelBilgiler.PoliceArac.KullanimTarzi = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "motor-no")
                            {
                                police.GenelBilgiler.PoliceArac.MotorNo = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "sasi-no")
                            {
                                police.GenelBilgiler.PoliceArac.SasiNo = gnlnode.InnerText;
                            }

                            if (polNode.Name == "oto" && gnlnode.Name == "tescil-tarihi")
                            {
                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                {
                                    police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                }
                            }
                            if (polNode.Name == "oto" && gnlnode.Name == "tramer-belge-no")
                            {
                                police.GenelBilgiler.PoliceArac.TramerBelgeNo = gnlnode.InnerText;
                            }
                            if (polNode.Name == "oto" && gnlnode.Name == "tramer-belge-tarihi")
                            {
                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                {
                                    police.GenelBilgiler.PoliceArac.TramerBelgeTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                }
                            }
                        }

                        #endregion
                        #region riziko yangın

                        //Kasko veya Trafik ürünü araç bilgileri
                        //XmlNodeList gnlb = polNode["oto"].ChildNodes;
                        XmlNodeList gnlbYangin = polNode["yangin"].ChildNodes;
                        for (int gnlbidx = 0; gnlbidx < gnlbYangin.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlbYangin[gnlbidx];
                            if (gnlnode.Name == "riziko-il-ad")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Il = gnlnode.InnerText;
                            }

                            if (gnlnode.Name == "riziko-ilce-ad")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Ilce = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "riziko-semt-ad")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.SemtBelde = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "riziko-sokak-ad")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Sokak = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "riziko-daire-no")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Daire = gnlnode.InnerText;
                            }


                        }

                        #endregion
                        #region riziko dask

                        //Kasko veya Trafik ürünü araç bilgileri
                        //XmlNodeList gnlb = polNode["oto"].ChildNodes;
                        XmlNodeList gnlbDask = polNode["dask"].ChildNodes;
                        for (int gnlbidx = 0; gnlbidx < gnlbDask.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlbDask[gnlbidx];
                            if (gnlnode.Name == "dask-riziko-ilad")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Il = gnlnode.InnerText;
                            }

                            if (gnlnode.Name == "dask-riziko-ilcead")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Ilce = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "dask-riziko-semtad")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.SemtBelde = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "dask-riziko-sokakad")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Sokak = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "dask-riziko-kapino")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Daire = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "dask-riziko-mahalle")
                            {
                                police.GenelBilgiler.PoliceRizikoAdresi.Mahalle = gnlnode.InnerText;
                            }


                        }

                        #endregion
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
