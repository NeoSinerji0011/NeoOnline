using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers
{
    class MapfreDaskXmlReader : IPoliceTransferReader
    {
        private string filePath;
        private int tvmkodu;
        private string message;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        ITVMService _TVMService;
        public MapfreDaskXmlReader()
        { }

        public MapfreDaskXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
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
                        int sayac = 0;
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
                            if (gnlnode.Name == "police-prefix")
                            {
                                police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);
                            }
                            if (gnlnode.Name == "zeyil-no")
                            {
                                police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                zeyilNo = gnlnode.InnerText;
                            }
                            if (gnlnode.Name == "zeyil tipi") police.GenelBilgiler.ZeyilAdi = gnlnode.InnerText;

                            if (zeyilNo == "0")
                            {
                                if (gnlnode.Name == "police-tanzim-tarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat6);
                                if (gnlnode.Name == "police-baslama-tarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat6);
                                if (gnlnode.Name == "police-bitis-tarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat6);
                            }
                            else
                            {
                                if (gnlnode.Name == "police-tanzim-tarihi") police.GenelBilgiler.TanzimTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat6);
                                if (gnlnode.Name == "zeyil-baslama-tarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat6);
                                if (gnlnode.Name == "zeyil-bitis-tarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText, Util.DateFormat6);
                            }

                            //if (gnlnode.Name == "yenileme-no") police.GenelBilgiler.YenilemeNo = Util.toInt(gnlnode.InnerText);


                            if (gnlnode.Name == "doviz-kodu" || gnlnode.Name == "police-doviz-kodu")
                            {
                                police.GenelBilgiler.ParaBirimi = gnlnode.InnerText;
                            }

                            if (gnlnode.Name == "doviz-birim-kodu" || gnlnode.Name == "police-doviz-kuru")
                            {
                                if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                {
                                    police.GenelBilgiler.DovizKur = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                }
                            }

                            if (police.GenelBilgiler.ParaBirimi != "TL")
                            {
                                if (police.GenelBilgiler.ParaBirimi == "YTL")
                                {
                                    police.GenelBilgiler.ParaBirimi = "TL";
                                }
                                if (gnlnode.Name == "doviz-birim-kodu" || gnlnode.Name == "police-doviz-kuru")
                                {
                                    dovizKuru = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                }
                            }
                            if (gnlnode.Name == "net-prim" || gnlnode.Name == "police-net-prim-ytl")
                            {
                                police.GenelBilgiler.NetPrim = Util.ToDecimal(gnlnode.InnerText);
                            }
                            // dövizli olanların diğer adlarını bulunca yaz.
                            if (gnlnode.Name == "police-net-prim")
                            {
                                police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(gnlnode.InnerText);
                            }
                            if (gnlnode.Name == "police-brut-prim")
                            {
                                police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(gnlnode.InnerText);
                            }
                            if (gnlnode.Name == "police-acente-komisyonu")
                            {
                                police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(gnlnode.InnerText);
                            }
                            //....
                            if (gnlnode.Name == "brut-prim" || gnlnode.Name == "police-brut-prim-ytl")
                            {
                                police.GenelBilgiler.BrutPrim = Util.ToDecimal(gnlnode.InnerText);
                            }
                            if (gnlnode.Name == "komisyon-net" || gnlnode.Name == "police-acente-komisyonu-ytl")
                            {
                                police.GenelBilgiler.Komisyon = Util.ToDecimal(gnlnode.InnerText);
                            }

                            if (gnlnode.Name == "vergi-tutari-ytl") police.GenelBilgiler.ToplamVergi = Util.ToDecimal(gnlnode.InnerText);


                            // Sigorta ettiren - Dask için
                            if (gnlnode.Name == "sigorta-ettiren-tcno") police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-vergino") police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = gnlnode.InnerText;
                            sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                            if (gnlnode.Name == "sigorta-ettiren") police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-adresi") police.GenelBilgiler.PoliceSigortaEttiren.Adres = gnlnode.InnerText;
                            if (gnlnode.Name == "sigorta-ettiren-telefon") police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = gnlnode.InnerText;
                            // Sigortali - (dask kullanıyor)
                            if (gnlnode.Name == "sigortali-vergi-no") police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = gnlnode.InnerText;
                            if (gnlnode.Name == "sigortali-tc-kimik-no") police.GenelBilgiler.PoliceSigortali.KimlikNo = gnlnode.InnerText;
                            sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                            if (gnlnode.Name == "sigortali") police.GenelBilgiler.PoliceSigortali.AdiUnvan = gnlnode.InnerText;
                            if (gnlnode.Name == "sigortali-adres") police.GenelBilgiler.PoliceSigortali.Adres = gnlnode.InnerText;
                            if (gnlnode.Name == "sigortali-telefon") police.GenelBilgiler.PoliceSigortali.TelefonNo = gnlnode.InnerText;
                            if (police.GenelBilgiler.PoliceSigortali.KimlikNo == "0")
                            {
                                police.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                            }
                            if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "0")
                            {
                                police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                            }
                            if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "0")
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                            }
                            if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "0")
                            {
                                police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                            }
                            // Odeme Sekli
                            if (gnlnode.Name == "odeme-kodu" || gnlnode.Name == "musteri-odeme-tipi")
                            {
                                if (Util.toInt(gnlnode.InnerText) == 10)
                                {
                                    police.GenelBilgiler.OdemeSekli = 1; //Pesin
                                }
                                if (gnlnode.InnerText == "PEŞİN")
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

                        police.GenelBilgiler.Durum = 0;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.MAPFREDASK;

                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);
                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;
                        #endregion




                        #region Odeme Planı Dask için
                        XmlNodeList risk = polNode["acente-taksitleri"].ChildNodes;//riskler child nodes
                        for (int indx = 0; indx < risk.Count; indx++)
                        {
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            XmlNodeList rsk = risk[indx].ChildNodes; //risk child nodes
                            for (int idx = 0; idx < rsk.Count; idx++)
                            {
                                XmlNode risks = rsk[idx];

                                XmlNode sh = risks.FirstChild;
                                if (risks.Name == "vade-tarihi" && sayac < 2)
                                {
                                    odm.VadeTarihi = Util.toDate(risks.InnerText, Util.DateFormat6);
                                    sayac++;
                                }
                                //odm.TaksitTutari = Util.ToDecimal(risks["taksit-tutari-ytl"].InnerText);

                                if (risks.Name == "taksit-tutari-ytl" && sayac < 2)
                                {
                                    odm.TaksitTutari = Util.ToDecimal(risks.InnerText);
                                    sayac++;
                                    odm.TaksitNo = sayac;
                                }
                                if (risks.Name == "taksit-tutari")
                                {
                                    if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                    {
                                        odm.DovizliTaksitTutari = Util.ToDecimal(risks.InnerText);
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
                            }
                            if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                            {
                                #region Tahsilat işlemi

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
                                            tahsilat.YenilemeNo = police.GenelBilgiler.YenilemeNo;
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
                                        tahsilat.YenilemeNo = police.GenelBilgiler.YenilemeNo;
                                        tahsilat.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortali.KimlikNo;
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
                                        tahsilats.YenilemeNo = police.GenelBilgiler.YenilemeNo;
                                        tahsilats.KimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
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
                                #endregion
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            
                        }
                        if (risk.Count == 0)
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
                                                tahsilat.YenilemeNo = police.GenelBilgiler.YenilemeNo;
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
                                        tahsilat.YenilemeNo = police.GenelBilgiler.YenilemeNo;
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

                        #region riziko Bilgileri

                        XmlNode aracList = polNode["sahalar"];
                        XmlNodeList araclis = aracList.ChildNodes;
                        for (int idx = 0; idx < araclis.Count; idx++)
                        {
                            XmlNode elm = araclis.Item(idx);
                            if (elm.Name == "saha")
                            {
                                if (!String.IsNullOrEmpty(elm["saha-adi"].InnerText))
                                {
                                    if (elm["saha-adi"].InnerText == "RİZİKO DAİRE NO")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.Daire = elm["saha-degeri"].InnerText;

                                    }
                                    if (elm["saha-adi"].InnerText == "RİSK MAHALLE")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.Mahalle = elm["saha-degeri"].InnerText;

                                    }
                                    if (elm["saha-adi"].InnerText == "RİSK SOKAK")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.Sokak = elm["saha-degeri"].InnerText;

                                    }

                                    if (elm["saha-adi"].InnerText == "RİSK APARTMAN ADI")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.Apartman = elm["saha-degeri"].InnerText;

                                    }
                                    if (elm["saha-adi"].InnerText == "İLETİŞİM ADRESİ ........1")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.Adres = elm["saha-degeri"].InnerText;

                                    }
                                    if (elm["saha-adi"].InnerText == "RİSK POSTA KODU")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.PostaKodu = Util.toInt(elm["saha-degeri"].InnerText);

                                    }
                                    if (elm["saha-adi"].InnerText == "DASK İL")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.IlKodu = Util.toInt(elm["saha-degeri"].InnerText);

                                    }
                                    if (elm["saha-adi"].InnerText == "DASK İLÇE")
                                    {
                                        police.GenelBilgiler.PoliceRizikoAdresi.IlceKodu = Util.toInt(elm["saha-degeri"].InnerText);

                                    }
                                }
                            }
                        }

                        #endregion

                        #region Vergiler

                        //vergiler  mapfre yapısında vergi toplamını dönüyor.  ayrı ayrı vergileri dönmüyor. 
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
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA;

                        policeler.Add(police);
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
