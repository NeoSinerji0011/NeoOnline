using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.MAPFRE;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class MapfreXmlReader : IPoliceTransferReader
    {
        ITVMService _TVMService;
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public MapfreXmlReader()
        { }

        public MapfreXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            decimal dovizKuru = 1;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);

                XmlNode root = doc.FirstChild;
                XmlNode s = root.NextSibling;
                if (s.Name == "policeler")
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        Police police = new Police();
                        XmlNode polNode = s.ChildNodes[i];

                        #region Genel Bilgiler

                        XmlNodeList gnlb = polNode["genel-bilgiler"].ChildNodes;

                        string kimlikTipi = null;
                        string kimlikNo = null;
                        string zeyilNo = null;

                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                        else police.GenelBilgiler.TVMKodu = 0;

                        for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                        {
                            XmlNode gnlnode = gnlb[gnlbidx];

                            if (gnlnode.Name == "police-no") police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                            if (gnlnode.Name == "brans") tumUrunKodu = gnlnode.InnerText;

                            if (gnlnode.Name == "zeyil-no")
                            {
                                police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                zeyilNo = gnlnode.InnerText;
                            }

                            if (zeyilNo == "0")
                            {
                                if (gnlnode.Name == "police-tanzim-tarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                                if (gnlnode.Name == "police-baslama-tarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                                if (gnlnode.Name == "police-bitis-tarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                            }
                            else
                            {
                                if (gnlnode.Name == "zeyil-tanzim-tarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                                if (gnlnode.Name == "zeyil-baslama-tarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                                if (gnlnode.Name == "zeyil-bitis-tarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat1);
                            }

                            if (gnlnode.Name == "yenileme-no") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);


                            if (gnlnode.Name == "doviz-kodu") police.GenelBilgiler.ParaBirimi = gnlnode.InnerText;

                            if (gnlnode.Name == "zeyil-kur")
                            {
                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                {
                                    if (gnlnode.InnerText !="1" && gnlnode.InnerText !="0")
                                    {
                                        police.GenelBilgiler.DovizKur = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                    }
                                }
                            }

                            if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != null)
                            {
                                if (gnlnode.Name == "zeyil-kur")
                                {
                                    dovizKuru = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                }
                            }
                            if (gnlnode.Name == "net-prim")
                            {
                                police.GenelBilgiler.NetPrim = Util.ToDecimal(gnlnode.InnerText);
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * dovizKuru;
                                    police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(gnlnode.InnerText);
                                }
                            }

                            if (gnlnode.Name == "brut-prim")
                            {
                                police.GenelBilgiler.BrutPrim = Util.ToDecimal(gnlnode.InnerText);
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * dovizKuru;
                                    police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(gnlnode.InnerText);
                                }
                            }
                            if (gnlnode.Name == "komisyon-net")
                            {
                                police.GenelBilgiler.Komisyon = Util.ToDecimal(gnlnode.InnerText);
                                if (dovizKuru != 0 && dovizKuru != 1)
                                {
                                    police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * dovizKuru;
                                    police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(gnlnode.InnerText);
                                    dovizKuru = 0;
                                }
                            }
                            if (gnlnode.Name == "vergi") police.GenelBilgiler.ToplamVergi = Util.ToDecimal(gnlnode.InnerText);

                            // Sigorta ettiren - Musteri
                            if (gnlnode.Name == "sigorta-ettiren-belge-no") kimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-belge-tip") kimlikTipi = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-ad-unvan") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-soyad-1") police.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-adres") police.GenelBilgiler.PoliceSigortaEttiren.Adres = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-telefon") police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = gnlnode.InnerText;

                            // Odeme Sekli
                            if (gnlnode.Name == "odeme-kodu")
                            {
                                if (Util.toInt(gnlnode.InnerText) == 10)
                                {
                                    police.GenelBilgiler.OdemeSekli = 1; //Pesin
                                }
                                else
                                {
                                    police.GenelBilgiler.OdemeSekli = 2; //vadeli
                                }
                            }
                        }

                        // sigorta ettiren icon belge tipinden belge no belirle

                        if (kimlikTipi == "TCK")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = kimlikNo;
                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                        }
                        if (kimlikTipi == "VRG")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = kimlikNo;
                        }
                        if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "0")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                        }
                        if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "0")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                        }
                        if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                        }
                        if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                        }
                        sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        police.GenelBilgiler.Durum = 0;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA;

                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                        #endregion

                        #region Sigortali Bilgileri

                        XmlNodeList risk = polNode["riskler"].ChildNodes;//riskler child nodes
                        for (int indx = 0; indx < risk.Count; indx++)
                        {
                            XmlNodeList rsk = risk[indx].ChildNodes; //risk child nodes
                            for (int idx = 0; idx < rsk.Count; idx++)
                            {
                                XmlNode risks = rsk[idx];
                                if (risks.Name == "risk-sahislar")
                                {
                                    XmlNode sh = risks.FirstChild; //sahis node
                                    police.GenelBilgiler.PoliceSigortali.AdiUnvan = sh["ad-unvan"].InnerText;
                                    police.GenelBilgiler.PoliceSigortali.SoyadiUnvan = sh["soyad-1"].InnerText;
                                    police.GenelBilgiler.PoliceSigortali.SoyadiUnvan += " " + sh["soyad-2"].InnerText;
                                    police.GenelBilgiler.PoliceSigortali.Adres = sh["adres"].InnerText;
                                    if (sh["belge-tip"].InnerText == "TCK")
                                    {
                                        if (sh["belge-no"].InnerText.Length == 11)
                                        {
                                            police.GenelBilgiler.PoliceSigortali.KimlikNo = sh["belge-no"].InnerText;

                                        }
                                    }
                                    if (sh["belge-tip"].InnerText == "VRG")
                                    {
                                        if (sh["belge-no"].InnerText.Length == 10)
                                        {
                                            police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = sh["belge-no"].InnerText;

                                        }
                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.KimlikNo == "0")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "0")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.KimlikNo == "")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                                    }
                                    if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "")
                                    {
                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                                    }
                                    sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                                }
                            }
                        }
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
                        #region Odeme Planı

                        XmlNode tk = polNode["odeme-plani"];
                        XmlNodeList tks = tk.ChildNodes;

                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            odm.TaksitNo = Util.toInt(elm["sira"].InnerText);
                            odm.VadeTarihi = Util.toDate(elm["son-odeme-tarihi"].InnerText, Util.DateFormat1);
                            odm.TaksitTutari = Util.ToDecimal(elm["tutar"].InnerText);
                            if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                            {
                                odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["tutar"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2);
                                odm.DovizliTaksitTutari = Util.ToDecimal(elm["tutar"].InnerText);
                            }
                            if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                            {
                                odm.OdemeTipi = OdemeTipleri.KrediKarti;
                            }
                            else
                            {
                                odm.OdemeTipi = OdemeTipleri.Havale;
                            }
                            if (odm.TaksitTutari != 0 && odm.TaksitTutari!=null)
                            {
                                #region Tahsilat işlemi                     
                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                    if (tvmkodu == 139)
                                    {

                                        PoliceTahsilat tahsilats = new PoliceTahsilat();
                                        tahsilats.OdemTipi = OdemeTipleri.Havale;
                                        odm.OdemeTipi = OdemeTipleri.Havale;
                                        tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                        tahsilats.TaksitNo = odm.TaksitNo;
                                        tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                        //tahsilats.OdemeBelgeNo = "111111";
                                        tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilats.OdenenTutar = 0;
                                        tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                        tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                        tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                        tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                        tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                        tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                        tahsilats.KayitTarihi = DateTime.Today;
                                        tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                        tahsilats.TahsilatId = odm.PoliceId;
                                        if (tahsilats.TaksitTutari != 0)
                                        {
                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilats);
                                        }

                                    }
                                    else
                                    {

                                        if (police.GenelBilgiler.BransKodu == 1 || police.GenelBilgiler.BransKodu == 2)
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
                                            PoliceTahsilat tahsilats = new PoliceTahsilat();
                                            tahsilats.OdemTipi = OdemeTipleri.Havale;
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                            tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                            tahsilats.TaksitNo = odm.TaksitNo;
                                            tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                            //tahsilats.OdemeBelgeNo = "111111";
                                            tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilats.OdenenTutar = 0;
                                            tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                            tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                            tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                            tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                            tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                            tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                            tahsilats.KayitTarihi = DateTime.Today;
                                            tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;
                                            tahsilats.TahsilatId = odm.PoliceId;
                                            if (tahsilats.TaksitTutari != 0)
                                            {
                                                police.GenelBilgiler.PoliceTahsilats.Add(tahsilats);
                                            }
                                        }

                                    }
                                }

                                #endregion
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            

                        }
                        if (tks.Count == 0)
                        {
                            PoliceOdemePlani odmm = new PoliceOdemePlani();
                            if (odmm.TaksitTutari == null && police.GenelBilgiler.BrutPrim != 0)
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
                                    var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA, police.GenelBilgiler.BransKodu.Value);
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


                        #endregion

                        #region Vergiler

                        //vergiler  mapfre yapısında vergi toplamını dönüyor.  ayrı ayrı vergileri dönmüyor. 


                        policeler.Add(police);

                        #endregion

                        #region Araç Bilgileri
                        for (int indx = 0; indx < risk.Count; indx++)
                        {
                            XmlNodeList rsk = risk[indx].ChildNodes; //risk child nodes
                            for (int idx = 0; idx < rsk.Count; idx++)
                            {
                                XmlNode risks = rsk[idx];

                                if (risks.Name == "risk-sahalari")
                                {
                                    XmlNodeList sahaNodeList = risks.ChildNodes; //saha node list
                                    for (int idxSaha = 0; idxSaha < sahaNodeList.Count; idxSaha++)
                                    {
                                        XmlNode saha = sahaNodeList[idxSaha];
                                        if (saha["kod"].InnerText == "COD_PLAKA_IL_KODU")
                                        {
                                            police.GenelBilgiler.PoliceArac.PlakaKodu = saha["deger"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "TXT_PLAKA_NO")
                                        {
                                            police.GenelBilgiler.PoliceArac.PlakaNo = saha["deger"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "CINSIYET")
                                        {
                                            police.GenelBilgiler.PoliceSigortali.Cinsiyet = saha["deger"].InnerText;
                                            police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = saha["deger"].InnerText;

                                        }
                                        if (saha["kod"].InnerText == "COD_ARAC_RUHSAT_SERI")
                                        {
                                            police.GenelBilgiler.PoliceArac.TescilSeriKod = saha["deger"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "COD_ARAC_RUHSAT_SERI_NO")
                                        {
                                            police.GenelBilgiler.PoliceArac.TescilSeriNo = saha["deger"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "TXT_SASI_NO")
                                        {
                                            police.GenelBilgiler.PoliceArac.SasiNo = saha["deger"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "TXT_MOTOR_NO")
                                        {
                                            police.GenelBilgiler.PoliceArac.MotorNo = saha["deger"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "COD_MARKA")
                                        {
                                            police.GenelBilgiler.PoliceArac.Marka = saha["deger"].InnerText;
                                            police.GenelBilgiler.PoliceArac.MarkaAciklama = saha["txt"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "COD_MARKA_TIPI")
                                        {
                                            police.GenelBilgiler.PoliceArac.AracinTipiKodu = saha["deger"].InnerText;
                                            police.GenelBilgiler.PoliceArac.AracinTipiAciklama = saha["txt"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "COD_MODELI")
                                        {
                                            police.GenelBilgiler.PoliceArac.AracinTipiKodu2 = saha["deger"].InnerText;
                                            police.GenelBilgiler.PoliceArac.AracinTipiAciklama2 = saha["txt"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "NUM_MODEL_YILI")
                                        {
                                            police.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(saha["deger"].InnerText);
                                        }
                                        if (saha["kod"].InnerText == "NUM_YER_ADEDI")
                                        {
                                            police.GenelBilgiler.PoliceArac.KoltukSayisi = Convert.ToInt32(saha["deger"].InnerText);
                                        }
                                        if (saha["kod"].InnerText == "COD_KULLANIM_SEKLI")
                                        {
                                            police.GenelBilgiler.PoliceArac.KullanimSekli = saha["deger"].InnerText;
                                        }
                                        if (saha["kod"].InnerText == "FEC_TESCIL_TARIHI")
                                        {
                                            police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = MapfreSorguResponse.ToDateTime3(saha["deger"].InnerText);
                                        }
                                        if (saha["kod"].InnerText == "TXT_SBM_NO")
                                        {
                                            police.GenelBilgiler.PoliceArac.TramerBelgeNo = saha["deger"].InnerText;
                                        }
                                    }

                                }
                            }
                        }
                        #endregion

                    }
                }
                else if (s.Name == "tahsilat-odeme-list")
                {
                    #region tahsilat
                    IPoliceService _PoliceService = DependencyResolver.Current.GetService<IPoliceService>();
                    IPoliceTransferService _PoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
                    IPoliceContext _PoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
                    string policeNumarasi = "0";
                    int? ekNo = null;
                    int polSayac = 0;
                    int varOlanKayitlar = 0;
                    int policesiOlmayanSayac = 0;
                    foreach (XmlNode xn in s)
                    {
                        //Police polices = new Police();
                        XmlNodeList polices = xn.ChildNodes;
                        PoliceTahsilat tahsilat = new PoliceTahsilat();
                        PoliceOdemePlani odm = new PoliceOdemePlani();
                        var polGenel = new PoliceGenel();
                        bool odemeType = false;
                        bool odemeType2 = false;
                        tahsilat = new PoliceTahsilat();
                        for (int indx = 0; indx < polices.Count; indx++)
                        {
                            XmlNode polNode = polices[indx];

                            if (polNode.Name == "police-no") policeNumarasi = polNode.InnerText;
                            if (polNode.Name == "zeyil-no") ekNo = Util.toInt(polNode.InnerText);
                            if (policeNumarasi != "0" && ekNo != null)
                            {
                                polGenel = _PoliceService.getTahsilatPolice(SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA, policeNumarasi, ekNo.Value);

                                #region tahsilat kapatma
                                if (polGenel != null)
                                {
                                    #region Tek Çekim
                                    if (polNode.Name == "tahsilat-odeme-tip")
                                    {
                                        if (polNode.InnerText == "SANALPOS_ODEME")
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                            odemeType2 = true;
                                        }
                                        else if (polNode.InnerText == "SENET_ODEME")
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.CekSenet;
                                            odm.OdemeTipi = OdemeTipleri.CekSenet;
                                            tahsilat.OdemeBelgeNo = "111111";
                                            odemeType2 = true;
                                        }
                                        else if (polNode.InnerText == "NAKIT_ODEME")
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                            odm.OdemeTipi = OdemeTipleri.Nakit;
                                            tahsilat.OdemeBelgeNo = "111111";
                                            odemeType2 = true;
                                        }
                                        else
                                        {
                                            tahsilat.OdemTipi = OdemeTipleri.Havale;
                                            odm.OdemeTipi = OdemeTipleri.Havale;
                                            tahsilat.OdemeBelgeNo = "111111";
                                            odemeType2 = true;
                                        }
                                    }
                                    if (polNode.Name == "tahsilat-odeme-tutar")
                                    {
                                        tahsilat.OdenenTutar = Util.ToDecimal(polNode.InnerText);
                                        tahsilat.TaksitTutari = Util.ToDecimal(polNode.InnerText);
                                    }
                                    if (polNode.Name == "tahsilat-odeme-islem-tarihi")
                                    {
                                        tahsilat.TaksitVadeTarihi = Util.toDate(polNode.InnerText, Util.DateFormat1).HasValue ?
                                                                   Util.toDate(polNode.InnerText, Util.DateFormat1).Value : Convert.ToDateTime("01.01.0001");
                                        tahsilat.OdemeBelgeTarihi = Util.toDate(polNode.InnerText, Util.DateFormat1).HasValue ?
                                                   Util.toDate(polNode.InnerText, Util.DateFormat1).Value : Convert.ToDateTime("01.01.0001");
                                    }
                                    if (polNode.Name == "tahsilat-odeme-taksit-sayisi") tahsilat.TaksitNo = Util.toInt(polNode.InnerText);
                                    #endregion

                                    #region Taksitli Çekim

                                    if (polNode.Name == "tahsilat-odeme-taksit-list")
                                    {
                                        XmlNodeList polTahsilatTaksitler = polNode.ChildNodes;

                                        if (polTahsilatTaksitler.Count > 0)
                                        {
                                            for (int idx = 0; idx < polTahsilatTaksitler.Count; idx++)
                                            {
                                                XmlNode elm = polTahsilatTaksitler.Item(idx);
                                                XmlNodeList taksitList = elm.ChildNodes;

                                                for (int i = 0; i < taksitList.Count; i++)
                                                {
                                                    XmlNode elmn = taksitList.Item(i);
                                                    if (elmn.Name == "tahsilat-taksit-tutar")
                                                    {
                                                        tahsilat.OdenenTutar = Util.ToDecimal(elmn.InnerText);
                                                        tahsilat.TaksitTutari = Util.ToDecimal(elmn.InnerText);
                                                    }

                                                    if (elmn.Name == "tahsilat-taksit-vade") tahsilat.TaksitVadeTarihi = Util.toDate(elmn.InnerText, Util.DateFormat1).HasValue ?
                                                        Util.toDate(elmn.InnerText, Util.DateFormat1).Value : Convert.ToDateTime("01.01.0001");
                                                    if (elmn.Name == "tahsilat-taksit-vade") tahsilat.OdemeBelgeTarihi = Util.toDate(elmn.InnerText, Util.DateFormat1).HasValue ?
                                            Util.toDate(elmn.InnerText, Util.DateFormat1).Value : Convert.ToDateTime("01.01.0001");
                                                    if (elmn.Name == "taksit-tahsilat-odeme-tip")
                                                    {
                                                        if (elmn.InnerText == "SANALPOS_TAHSILAT")
                                                        {
                                                            tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                            odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                            tahsilat.OdemeBelgeNo = "111111****1111";
                                                            odemeType = true;
                                                        }
                                                        else if (elmn.InnerText == "SENET_TAHSILAT")
                                                        {
                                                            tahsilat.OdemTipi = OdemeTipleri.CekSenet;
                                                            odm.OdemeTipi = OdemeTipleri.CekSenet;
                                                            tahsilat.OdemeBelgeNo = "111111";
                                                            odemeType = true;
                                                        }
                                                        else if (elmn.InnerText == "NAKIT_TAHSILAT")
                                                        {
                                                            tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                            odm.OdemeTipi = OdemeTipleri.Nakit;
                                                            tahsilat.OdemeBelgeNo = "111111";
                                                            odemeType = true;
                                                        }
                                                    }
                                                    if (odemeType)
                                                    {

                                                        tahsilat.TaksitNo = idx + 1;
                                                        tahsilat.KalanTaksitTutari = 0;
                                                        tahsilat.PoliceNo = policeNumarasi;
                                                        tahsilat.ZeyilNo = ekNo.ToString();
                                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.PoliceSigortaEttiren.KimlikNo) ? polGenel.PoliceSigortaEttiren.KimlikNo : polGenel.PoliceSigortaEttiren.VergiKimlikNo;
                                                        tahsilat.BrutPrim = polGenel.BrutPrim.HasValue ? polGenel.BrutPrim.Value : 0;
                                                        tahsilat.KayitTarihi = DateTime.Today;
                                                        tahsilat.KaydiEkleyenKullaniciKodu = tvmkodu;
                                                        var resultAyniKayitmi = polGenel.PoliceTahsilats.Where(a => a.PoliceNo == policeNumarasi
                                                                       && a.ZeyilNo == ekNo.ToString()
                                                                       && a.OdemTipi == tahsilat.OdemTipi
                                                                       && a.TaksitNo == tahsilat.TaksitNo
                                                                       && a.OdenenTutar == tahsilat.OdenenTutar
                                                                       ).FirstOrDefault();
                                                        if (resultAyniKayitmi == null)
                                                        {
                                                            polGenel.PoliceTahsilats.Add(tahsilat);
                                                            polGenel.PoliceOdemePlanis.Add(odm);
                                                            _PoliceContext.PoliceGenelRepository.Update(polGenel);
                                                            _PoliceContext.Commit();
                                                            odemeType = false;
                                                            polSayac++;
                                                        }
                                                        else
                                                        {
                                                            varOlanKayitlar++;
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        if (odemeType2)
                                        {
                                            tahsilat.KalanTaksitTutari = 0;
                                            tahsilat.PoliceNo = policeNumarasi;
                                            tahsilat.ZeyilNo = ekNo.ToString();
                                            tahsilat.KimlikNo = !String.IsNullOrEmpty(polGenel.PoliceSigortaEttiren.KimlikNo) ? polGenel.PoliceSigortaEttiren.KimlikNo : polGenel.PoliceSigortaEttiren.VergiKimlikNo; tahsilat.KayitTarihi = DateTime.Today;
                                            tahsilat.BrutPrim = polGenel.BrutPrim.HasValue ? polGenel.BrutPrim.Value : 0;
                                            tahsilat.KaydiEkleyenKullaniciKodu = tvmkodu;
                                            var resultAyniKayitmi = polGenel.PoliceTahsilats.Where(a => a.PoliceNo == policeNumarasi
                                                           && a.ZeyilNo == ekNo.ToString()
                                                           && a.OdemTipi == tahsilat.OdemTipi
                                                           && a.TaksitNo == tahsilat.TaksitNo
                                                           && a.OdenenTutar == tahsilat.OdenenTutar
                                                           ).FirstOrDefault();
                                            if (resultAyniKayitmi == null)
                                            {
                                                polGenel.PoliceTahsilats.Add(tahsilat);
                                                polGenel.PoliceOdemePlanis.Add(odm);
                                                _PoliceContext.PoliceGenelRepository.Update(polGenel);
                                                _PoliceContext.Commit();
                                                odemeType2 = false;
                                                polSayac++;
                                            }
                                            else
                                            {
                                                varOlanKayitlar++;
                                            }
                                            break;
                                        }

                                    }
                                    #endregion
                                }
                                else
                                {
                                    policesiOlmayanSayac++;
                                    break;
                                }
                                #endregion

                            }
                        }
                    }

                    this.message = "Tahsilatı kapatılan kayıt sayısı: " + polSayac + " Daha önceden tahsilatı kapatılan kayıt sayısı:  " + varOlanKayitlar +
                         " Poliçesi transfer edilmediği için tahsilatı kapatılamayan kayıt sayısı:  " + policesiOlmayanSayac;
                    _PoliceTransferService.setMessage(this.message);
                    this.TahsilatMi = true;
                    return null;
                    #endregion
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
