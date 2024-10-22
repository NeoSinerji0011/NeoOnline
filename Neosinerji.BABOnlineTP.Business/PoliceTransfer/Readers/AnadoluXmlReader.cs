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
    public class AnadoluXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public AnadoluXmlReader()
        { }

        public AnadoluXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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

            int carpan = 1; //tahakkuk - iptal
            decimal dovizKuru = 1;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);
                var tanimliOdemeTipleri = _TVMService.GetListTanımliBransOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ANADOLUSIGORTA);
                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        carpan = 1;
                        Police police = new Police();
                        XmlNode polNode = s.ChildNodes[i];

                        #region Genel Bilgiler

                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        tumUrunAdi = polNode["PoliçeAçıklaması"].InnerText;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ANADOLUSIGORTA;
                        police.GenelBilgiler.PoliceNumarasi = polNode["PoliçeNumarası"].InnerText;
                        police.GenelBilgiler.EkNo = Util.toInt(polNode["EkNumarası"].InnerText);
                        police.GenelBilgiler.YenilemeNo = Util.toInt(polNode["YenilemeNumarası"].InnerText);
                        police.GenelBilgiler.TanzimTarihi = Util.toDate(polNode["TanzimTarihi"].InnerText, Util.DateFormat0);
                        police.GenelBilgiler.BaslangicTarihi = Util.toDate(polNode["BaşlangıçTarihi"].InnerText, Util.DateFormat0);
                        police.GenelBilgiler.BitisTarihi = Util.toDate(polNode["BitişTarihi"].InnerText, Util.DateFormat0);
                        police.GenelBilgiler.BrutPrim = Util.ToDecimal(polNode["ToplamBrütPrim"].InnerText);
                        police.GenelBilgiler.NetPrim = Util.ToDecimal(polNode["ToplamNetPrim"].InnerText);

                        if (polNode["ProdİptalKodu"].InnerText == "İptal/iade")
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

                        police.GenelBilgiler.ToplamVergi = Util.ToDecimal(polNode["ToplamBSMV"].InnerText);
                        police.GenelBilgiler.Komisyon = Util.ToDecimal(polNode["AcenteKomisyonu"].InnerText);
                        if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                        {
                            police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(polNode["DövizBrütPrim"].InnerText);
                            //police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(polNode[""].InnerText);
                            //police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(polNode[""].InnerText);
                        }
                        police.GenelBilgiler.ParaBirimi = polNode["DövizCinsi"].InnerText;
                        if (!String.IsNullOrEmpty(polNode["DövizKuru"].InnerText))
                        {
                            police.GenelBilgiler.DovizKur = Util.ToDecimal(polNode["DövizKuru"].InnerText.Replace(".", ","));
                        }

                        if (police.GenelBilgiler.ParaBirimi != "TL")
                        {
                            dovizKuru = Util.ToDecimal(polNode["DövizKuru"].InnerText.Replace(".", ","));
                        }
                        if (dovizKuru != 0 && dovizKuru != 1)
                        {
                            police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                            police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                            police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                            police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(polNode["ToplamNetPrim"].InnerText);
                            police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(polNode["AcenteKomisyonu"].InnerText);


                            dovizKuru = 0;
                        }
                        // Odeme Sekli
                        if (polNode["PeşinPoliçeKodu"].InnerText == "Evet")
                        {
                            police.GenelBilgiler.OdemeSekli = 1; //Pesin
                        }
                        else if (polNode["PeşinPoliçeKodu"].InnerText == "Hayır")
                        {
                            police.GenelBilgiler.OdemeSekli = 2; //Vadeli
                        }
                        else
                        {
                            police.GenelBilgiler.OdemeSekli = 0; //berlirsiz
                        }

                        police.GenelBilgiler.Durum = 0;

                        #endregion

                        #region Sigorta Ettiren Bilgileri

                        // Sigorta ettiren - Musteri
                        XmlNodeList mustNode = polNode["MÜŞTERİ"].ChildNodes;

                        for (int mnidx = 0; mnidx < mustNode.Count; mnidx++)
                        {
                            XmlNode mcnode = mustNode[mnidx];
                            if (mcnode.Name == "AdıSoyadı-Ünvanı") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = mcnode.InnerText;
                            if (mcnode.Name == "TCKimlikNumarası")
                            {
                                if (mcnode.InnerText.Length == 11)
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = mcnode.InnerText;
                                }
                            }
                            if (mcnode.Name == "VergiKimlikNumarası")
                            {
                                if (mcnode.InnerText.Length == 10)
                                {
                                    police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = mcnode.InnerText;
                                }
                            }
                            if (mcnode.Name == "E-Mail") police.GenelBilgiler.PoliceSigortaEttiren.EMail = mcnode.InnerText;
                            sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;

                            if (mcnode.Name == "ADRESİ")
                            {
                                XmlNodeList adrnode = mcnode.ChildNodes;
                                for (int adridx = 0; adridx < adrnode.Count; adridx++)
                                {
                                    XmlNode sanode = adrnode[adridx];
                                    if (sanode.Name == "Adres1") police.GenelBilgiler.PoliceSigortaEttiren.Adres += sanode.InnerText;
                                    if (sanode.Name == "Adres2") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + sanode.InnerText;
                                    if (sanode.Name == "Adres3") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + sanode.InnerText;
                                    if (sanode.Name == "Adres4") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + sanode.InnerText;
                                    if (sanode.Name == "Adres5") police.GenelBilgiler.PoliceSigortaEttiren.Adres += " " + sanode.InnerText;

                                    if (sanode.Name == "İl") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = sanode.InnerText;
                                    if (sanode.Name == "İlçe") police.GenelBilgiler.PoliceSigortaEttiren.IlceAdi = sanode.InnerText;
                                }
                            }
                        }
                        #endregion

                        #region Sigortali Bilgileri

                        XmlNodeList sigNode = polNode["SİGORTALI"].ChildNodes;

                        for (int snidx = 0; snidx < sigNode.Count; snidx++)
                        {
                            XmlNode snode = sigNode[snidx];
                            if (snode.Name == "AdıSoyadı") police.GenelBilgiler.PoliceSigortali.AdiUnvan = snode.InnerText;
                            if (police.GenelBilgiler.PoliceSigortali.AdiUnvan == null || police.GenelBilgiler.PoliceSigortali.AdiUnvan == "")
                            {
                                police.GenelBilgiler.PoliceSigortali.AdiUnvan = police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan;
                            }
                            if (snode.Name == "ADRESİ")
                            {
                                XmlNodeList adrnode = snode.ChildNodes;
                                for (int adridx = 0; adridx < adrnode.Count; adridx++)
                                {
                                    XmlNode sanode = adrnode[adridx];
                                    if (sanode.Name == "Adres1") police.GenelBilgiler.PoliceSigortali.Adres += sanode.InnerText;
                                    if (sanode.Name == "Adres2") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;
                                    if (sanode.Name == "Adres3") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;
                                    if (sanode.Name == "Adres4") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;
                                    if (sanode.Name == "Adres5") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;

                                    if (sanode.Name == "İl") police.GenelBilgiler.PoliceSigortali.IlAdi = sanode.InnerText;
                                    if (sanode.Name == "İlçe") police.GenelBilgiler.PoliceSigortali.IlceAdi = sanode.InnerText;
                                }
                            }
                            if (police.GenelBilgiler.PoliceSigortali.KimlikNo == null || police.GenelBilgiler.PoliceSigortali.KimlikNo == "")
                            {
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo;
                            }
                            if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == null || police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "")
                            {
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            }
                        }

                        #endregion
                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestirAnadolu(_SigortaSirketiBransList, _branslar, tumUrunAdi);

                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                        #region Odeme Planı

                        XmlNode tk = polNode["TAKSİTLER"];
                        XmlNodeList tks = tk.ChildNodes;

                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            odm.TaksitNo = indx + 1;
                            odm.VadeTarihi = Util.toDate(elm["VadeTarihi"].InnerText, Util.DateFormat0);

                            //BorcAlacak = 9 ise tutar -1 ile carpilacak !!!!!!!
                            odm.TaksitTutari = Util.ToDecimal(elm["Tutar"].InnerText);
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["Tutar"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                odm.DovizliTaksitTutari = Util.ToDecimal(elm["Tutar"].InnerText);
                            }
                            XmlNode ba = elm["BorçAlacak"]; // !!!!!  zorunlu depremde null oluyor
                            if (ba != null)
                            {
                                if (Util.toInt(elm["BorçAlacak"].InnerText) == 9)
                                {
                                    odm.TaksitTutari *= -1;
                                    if (odm.DovizliTaksitTutari != null)
                                    {
                                        odm.DovizliTaksitTutari *= -1;
                                    }
                                }
                            }
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
                            if (odm.TaksitTutari == null || odm.TaksitTutari == 0 && police.GenelBilgiler.BrutPrim.Value != 0)
                            {
                                odm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                {
                                    odm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                    odm.DovizliTaksitTutari = police.GenelBilgiler.DovizliBrutPrim;
                                }
                                odm.TaksitNo = 1;
                                odm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);

                            }
                            #region Tahsilat işlemi
                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ANADOLUSIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                    //  tahsilat.OdemeBelgeNo = "111111";
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
                            #endregion
                        }

                        #endregion

                        #region Vergiler

                        decimal topVergi = 0;

                        //GiderVergisi - BSMV
                        PoliceVergi gv = new PoliceVergi();
                        gv.VergiKodu = 2;
                        gv.VergiTutari = Util.ToDecimal(polNode["ToplamBSMV"].InnerText);
                        topVergi += Util.ToDecimal(polNode["ToplamBSMV"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(gv);

                        //Garanti Fonu- GF
                        PoliceVergi gf = new PoliceVergi();
                        gf.VergiKodu = 3;
                        gf.VergiTutari = Util.ToDecimal(polNode["GarantiFonu"].InnerText);
                        topVergi += Util.ToDecimal(polNode["GarantiFonu"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(gf);

                        //Trafik hizmetleri gelilstirme fonu- THGF
                        PoliceVergi thgf = new PoliceVergi();
                        thgf.VergiKodu = 1;
                        thgf.VergiTutari = Util.ToDecimal(polNode["TrafikGeliştirmeFonu"].InnerText);
                        topVergi += Util.ToDecimal(polNode["TrafikGeliştirmeFonu"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(thgf);

                        //YSV
                        PoliceVergi ysv = new PoliceVergi();
                        ysv.VergiKodu = 4;
                        ysv.VergiTutari = Util.ToDecimal(polNode["ToplamYSV"].InnerText);
                        topVergi += Util.ToDecimal(polNode["ToplamYSV"].InnerText);
                        police.GenelBilgiler.PoliceVergis.Add(ysv);
                        police.GenelBilgiler.ToplamVergi = topVergi;

                        // Product - Iptal/Iade drumuna gore -1 ile carpilacaklar net ve brut primler !!!!!!

                        police.GenelBilgiler.BrutPrim *= carpan;
                        police.GenelBilgiler.NetPrim *= carpan;


                        #endregion

                        #region Araç Bilgileri
                        XmlNode aracNode = polNode["ARAÇLİSTESİ"];
                        if (aracNode != null)
                        {
                            XmlNodeList araclist = aracNode["ARAÇ"].ChildNodes;
                            for (int aracindex = 0; aracindex < araclist.Count; aracindex++)
                            {
                                XmlNode anode = araclist[aracindex];
                                if (anode.Name == "Marka") police.GenelBilgiler.PoliceArac.MarkaAciklama = anode.InnerText;
                                if (anode.Name == "Modeli") police.GenelBilgiler.PoliceArac.AracinTipiAciklama = anode.InnerText;
                                if (anode.Name == "ModelYılı") police.GenelBilgiler.PoliceArac.Model = Util.toInt(anode.InnerText);
                                if (anode.Name == "AracınRengi") police.GenelBilgiler.PoliceArac.Renk = anode.InnerText;
                                if (anode.Name == "MotorNumarası") police.GenelBilgiler.PoliceArac.MotorNo = anode.InnerText;
                                if (anode.Name == "ŞasiNumarası") police.GenelBilgiler.PoliceArac.SasiNo = anode.InnerText;
                                if (anode.Name == "MotorGücü") police.GenelBilgiler.PoliceArac.MotorGucu = anode.InnerText;

                                if (anode.Name == "PlakaNumarası")
                                {
                                    police.GenelBilgiler.PoliceArac.PlakaNo = anode.InnerText != "" && anode.InnerText.Length >= 2 ? anode.InnerText.Substring(2, anode.InnerText.Length - 2) : "";
                                    police.GenelBilgiler.PoliceArac.PlakaKodu = anode.InnerText != "" && anode.InnerText.Length >= 2 ? anode.InnerText.Substring(0, 2) : "";
                                }
                                if (anode.Name == "RuhsatTescilTarihi") police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Util.toDate(anode.InnerText, Util.DateFormat0);
                                if (anode.Name == "AraçKodu") police.GenelBilgiler.PoliceArac.AracinTipiKodu = anode.InnerText;
                                police.GenelBilgiler.PoliceArac.AracDeger = 0;

                                if (police.GenelBilgiler.PoliceArac.AracinTipiKodu != null)
                                {
                                    string markakod = "", tipkod = "";

                                    if (police.GenelBilgiler.PoliceArac.AracinTipiKodu.Length == 6)
                                    {
                                        markakod = police.GenelBilgiler.PoliceArac.AracinTipiKodu.Substring(0, 3);
                                        tipkod = police.GenelBilgiler.PoliceArac.AracinTipiKodu.Substring(3);
                                    }
                                    else if (police.GenelBilgiler.PoliceArac.AracinTipiKodu.Length == 7)
                                    {
                                        markakod = police.GenelBilgiler.PoliceArac.AracinTipiKodu.Substring(0, 3);
                                        tipkod = police.GenelBilgiler.PoliceArac.AracinTipiKodu.Substring(3);
                                    }
                                    else if (police.GenelBilgiler.PoliceArac.AracinTipiKodu.Length == 8)
                                    {
                                        markakod = police.GenelBilgiler.PoliceArac.AracinTipiKodu.Substring(1, 3);
                                        tipkod = police.GenelBilgiler.PoliceArac.AracinTipiKodu.Substring(4);
                                    }
                                    if (markakod != "")
                                        police.GenelBilgiler.PoliceArac.Marka = markakod;
                                    if (tipkod != "")
                                        police.GenelBilgiler.PoliceArac.AracinTipiKodu = tipkod;
                                }
                                #endregion

                                #region Riziko Adres Bilgileri

                                if (polNode["BİNA"] != null)
                                {
                                    XmlNodeList rizNode = polNode["BİNA"].ChildNodes;
                                    for (int rnidx = 0; rnidx < rizNode.Count; rnidx++)
                                    {
                                        XmlNode rnode = rizNode[rnidx];

                                        if (rnode.Name == "ADRESİ")
                                        {
                                            XmlNodeList adrnode = rnode.ChildNodes;
                                            for (int adridx = 0; adridx < adrnode.Count; adridx++)
                                            {
                                                XmlNode sanode = adrnode[adridx];
                                                if (sanode.Name == "Adres1") police.GenelBilgiler.PoliceSigortali.Adres += sanode.InnerText;
                                                if (sanode.Name == "Adres2") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;
                                                if (sanode.Name == "Adres3") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;
                                                if (sanode.Name == "Adres4") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;
                                                if (sanode.Name == "Adres5") police.GenelBilgiler.PoliceSigortali.Adres += " " + sanode.InnerText;
                                                if (sanode.Name == "İlçe") police.GenelBilgiler.PoliceSigortali.IlceAdi = sanode.InnerText;
                                                if (sanode.Name == "İl") police.GenelBilgiler.PoliceSigortali.IlAdi = sanode.InnerText;

                                            }
                                        }
                                    }
                                }
                                #endregion

                                if (tumUrunAdi == null)
                                {
                                    police.GenelBilgiler.TUMUrunAdi = PoliceBransEslestir.TUMUrunAdi;
                                    police.GenelBilgiler.TUMUrunKodu = PoliceBransEslestir.TUMUrunKodu;
                                }
                                else
                                {
                                    police.GenelBilgiler.TUMUrunAdi = tumUrunAdi;
                                    police.GenelBilgiler.TUMUrunKodu = tumUrunKodu;
                                }

                                police.GenelBilgiler.TUMBransAdi = PoliceBransEslestir.TUMBransAdi;
                                police.GenelBilgiler.TUMBransKodu = PoliceBransEslestir.TUMBransKodu;
                                policeler.Add(police);

                            }
                        }
                        #endregion
                    }
                }
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
