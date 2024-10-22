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
    public class EurekoXmlReader : IPoliceTransferReader
    {

        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        public EurekoXmlReader()
        {
        }

        public EurekoXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
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
            try
            {
                #region Poliçe reader

                doc = new XmlDocument();
                doc.Load(filePath);
                decimal dovizKuru = 1;
                //doc.Normalize();

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

                        tumUrunKodu = polNode["POLBRANS"].InnerText;
                        tumUrunAdi = polNode["POLBRANSAD"].InnerText;
                        police.GenelBilgiler.PoliceNumarasi = polNode["POLICENUMARASI"].InnerText;
                        police.GenelBilgiler.EkNo = Util.toInt(polNode["ZEYLNO"].InnerText);
                        police.GenelBilgiler.YenilemeNo = Util.toInt(polNode["YENILEMENUMARASI"].InnerText);
                        police.GenelBilgiler.TanzimTarihi = Util.toDate(polNode["TANZIMTAR"].InnerText, Util.DateFormat2);
                        police.GenelBilgiler.BaslangicTarihi = Util.toDate(polNode["BASTAR"].InnerText, Util.DateFormat2);
                        police.GenelBilgiler.BitisTarihi = Util.toDate(polNode["BITTAR"].InnerText, Util.DateFormat2);

                        PoliceGenelBrans PoliceBransEslestir = new PoliceGenelBrans();
                        PoliceBransEslestir = Util.PoliceBransAdiEslestir(_SigortaSirketiBransList, _branslar, tumUrunAdi, tumUrunKodu);

                        police.GenelBilgiler.BransAdi = PoliceBransEslestir.BransAdi;
                        police.GenelBilgiler.BransKodu = PoliceBransEslestir.BransKodu;

                        #region Sigorta Ettiren Detay

                        police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = polNode["SIGETTIRENADI"].InnerText;
                        //police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = polNode["SIGETTIRENTCKIMLIKNO"].InnerText;
                        police.GenelBilgiler.PoliceSigortaEttiren.Adres = polNode["SIGETTIRENILETISIMADRES"].InnerText;
                        if (polNode["SIGETTIRENTCKIMLIKNO"].InnerText.Length == 11)
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = polNode["SIGETTIRENTCKIMLIKNO"].InnerText;
                        }
                        if (polNode["SIGETTIRENVERGINO"].InnerText.Length == 10)
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = polNode["SIGETTIRENVERGINO"].InnerText;

                        }
                        if (police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo == "0")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = null;
                        }
                        if (police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo == "0")
                        {
                            police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = null;
                        }
                        sEttirenKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo) ? police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo : police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo;
                        // Sigorta ettiren - Detay
                        XmlNodeList mustNode = polNode["SIGETTIRENDETAY"].ChildNodes;
                        for (int mnidx = 0; mnidx < mustNode.Count; mnidx++)
                        {
                            XmlNode mcnode = mustNode[mnidx];
                            if (mcnode.Name == "IL_KOD") police.GenelBilgiler.PoliceSigortaEttiren.IlKodu = mcnode.InnerText;
                            if (mcnode.Name == "IL_AD") police.GenelBilgiler.PoliceSigortaEttiren.IlAdi = mcnode.InnerText;

                        }

                        #endregion

                        #region Sigortali

                        police.GenelBilgiler.PoliceSigortali.AdiUnvan = polNode["SIGORTALIAD"].InnerText;
                        police.GenelBilgiler.PoliceSigortali.KimlikNo = polNode["SIGORTALIKIMLIKNO"].InnerText;
                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = polNode["SIGORTALIVNO"].InnerText;
                        police.GenelBilgiler.PoliceSigortali.Adres = polNode["SIGORTALIILETISIMADRES"].InnerText;
                        if (polNode["SIGORTALIKIMLIKNO"].InnerText.Length == 11)
                        {
                            police.GenelBilgiler.PoliceSigortali.KimlikNo = polNode["SIGORTALIKIMLIKNO"].InnerText;
                        }
                        if (polNode["SIGORTALIVNO"].InnerText.Length == 10)
                        {
                            police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = polNode["SIGORTALIVNO"].InnerText;

                        }
                        if (police.GenelBilgiler.PoliceSigortali.KimlikNo == "0")
                        {
                            police.GenelBilgiler.PoliceSigortali.KimlikNo = null;
                        }
                        if (police.GenelBilgiler.PoliceSigortali.VergiKimlikNo == "0")
                        {
                            police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = null;
                        }
                        sLiKimlikNo = !String.IsNullOrEmpty(police.GenelBilgiler.PoliceSigortali.KimlikNo) ? police.GenelBilgiler.PoliceSigortali.KimlikNo : police.GenelBilgiler.PoliceSigortali.VergiKimlikNo;
                        #endregion


                        #region Vergiler

                        // brut prim, net prim,komisyon ,vergiler
                        decimal topVergi = 0;
                        XmlNode primvergi = polNode["PRIMVEVERGILER"];
                        XmlNodeList primVergilernode = primvergi.ChildNodes;

                        for (int idx = 0; idx < primVergilernode.Count; idx++)
                        {
                            XmlNode vergi = primVergilernode[idx];
                            if (vergi.Name == "NETPRIM") police.GenelBilgiler.NetPrim = Util.ToDecimal(vergi.InnerText);
                            if (vergi.Name == "BRUTPRIM") police.GenelBilgiler.BrutPrim = Util.ToDecimal(vergi.InnerText);
                            if (vergi.Name == "AKOMISYON") police.GenelBilgiler.Komisyon = Util.ToDecimal(vergi.InnerText);
                            if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                            {
                                if (vergi.Name == "NETPRIMYTL") police.GenelBilgiler.DovizliNetPrim = Util.ToDecimal(vergi.InnerText);
                                if (vergi.Name == "BRUTPRIMYTL") police.GenelBilgiler.DovizliBrutPrim = Util.ToDecimal(vergi.InnerText);
                                if (vergi.Name == "AKOMISYONYTL") police.GenelBilgiler.DovizliKomisyon = Util.ToDecimal(vergi.InnerText);
                            }
                            if (vergi.Name == "YSVERGISI")
                            {
                                PoliceVergi yv = new PoliceVergi();
                                yv.VergiKodu = 4;
                                yv.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                topVergi += Util.ToDecimal(vergi.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(yv);
                            }
                            if (vergi.Name == "GARANTIFONU")
                            {
                                PoliceVergi gf = new PoliceVergi();
                                gf.VergiKodu = 3;
                                gf.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                topVergi += Util.ToDecimal(vergi.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(gf);
                            }

                            if (vergi.Name == "GIDERVERGISI")
                            {
                                PoliceVergi gv = new PoliceVergi();
                                gv.VergiKodu = 2;
                                gv.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                topVergi += Util.ToDecimal(vergi.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(gv);
                            }

                            if (vergi.Name == "TRAHIZGELFON")
                            {
                                PoliceVergi thgf = new PoliceVergi();
                                thgf.VergiKodu = 1;
                                thgf.VergiTutari = Util.ToDecimal(vergi.InnerText);
                                topVergi += Util.ToDecimal(vergi.InnerText);
                                police.GenelBilgiler.PoliceVergis.Add(thgf);
                            }
                            police.GenelBilgiler.ToplamVergi = topVergi;

                        }
                        #endregion

                        #region Döviz Cinsi
                        XmlNodeList doviznodes = polNode["DOVIZ"].ChildNodes;
                        for (int dovidx = 0; dovidx < doviznodes.Count; dovidx++)
                        {
                            XmlNode node = doviznodes[dovidx];
                            if (node.Name == "DOVIZCINSI") police.GenelBilgiler.ParaBirimi = node.InnerText;
                            if (node.Name == "DOVIZKURU")
                            {
                                if (!String.IsNullOrEmpty(node.InnerText))
                                {
                                    police.GenelBilgiler.DovizKur = Util.ToDecimal(node.InnerText.Replace(".", ","));
                                }
                            }

                            if (police.GenelBilgiler.ParaBirimi != "TL")
                            {
                                if (node.Name == "DOVIZKURU")
                                {
                                    dovizKuru = Util.ToDecimal(node.InnerText.Replace(".", ","));
                                    if (dovizKuru != 0 && dovizKuru != 1)
                                    {
                                        police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                        police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru, 2);
                                        police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru, 2);
                                        dovizKuru = 0;
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Ödeme Planı

                        XmlNode tk = polNode["TAKSITLER"];
                        XmlNodeList tks = tk.ChildNodes;

                        for (int indx = 0; indx < tks.Count; indx++)
                        {
                            XmlNode elm = tks.Item(indx);
                            PoliceOdemePlani odm = new PoliceOdemePlani();

                            odm.TaksitNo = indx + 1;
                            // odeme Tipi ?
                            odm.VadeTarihi = Util.toDate(elm["VADETARIHI"].InnerText, Util.DateFormat2);
                            if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                            {
                                odm.DovizliTaksitTutari = Util.ToDecimal(elm["TUTAR"].InnerText);
                            }
                            odm.TaksitTutari = Util.ToDecimal(elm["TUTARYTL"].InnerText);

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
                            else if (odm.TaksitTutari == 0 && odm.TaksitNo == 1)
                            {
                                odm.TaksitTutari = police.GenelBilgiler.BrutPrim;
                                if (odm.VadeTarihi == null)
                                {
                                    odm.VadeTarihi = police.GenelBilgiler.BaslangicTarihi;
                                }
                                odm.TaksitNo = 1;
                                if (police.GenelBilgiler.BransKodu.Value == 1 || police.GenelBilgiler.BransKodu.Value == 2)
                                {
                                    odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                }
                                else
                                {
                                    odm.OdemeTipi = OdemeTipleri.Havale;
                                }
                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                            }
                            #region Tahsilat işlemi

                            var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.EUREKOSIGORTA, police.GenelBilgiler.BransKodu.Value);
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
                                    tahsilat.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
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

                        #endregion

                        #region Araç Bilgileri

                        XmlNodeList riskSorular = polNode["RISK_SORULARI"].ChildNodes;
                        for (int riskidx = 0; riskidx < riskSorular.Count; riskidx++)
                        {
                            XmlNode aracBilgiNode = riskSorular[riskidx];
                            if (aracBilgiNode.Name == "ARACBEDELI" && !String.IsNullOrEmpty(aracBilgiNode.InnerText))
                            {
                                aracBilgiNode.InnerText = aracBilgiNode.InnerText.Replace(',', ' ').Replace('.', ',').Replace(' ', '.');
                                police.GenelBilgiler.PoliceArac.AracDeger = Convert.ToDecimal(aracBilgiNode.InnerText);

                            }
                            if (aracBilgiNode.Name == "PLAKA" && !String.IsNullOrEmpty(aracBilgiNode.InnerText))
                            {
                                string plakaNo = aracBilgiNode.InnerText;
                                if (!String.IsNullOrEmpty(plakaNo))
                                {
                                    police.GenelBilgiler.PoliceArac.PlakaKodu = "";
                                    police.GenelBilgiler.PoliceArac.PlakaNo = "";
                                    for (int j = 0; j < plakaNo.Length; j++)
                                    {
                                        if (j < 2)
                                        {
                                            police.GenelBilgiler.PoliceArac.PlakaKodu += plakaNo[j].ToString();
                                        }
                                        if (j > 2)
                                        {
                                            police.GenelBilgiler.PoliceArac.PlakaNo += plakaNo[j].ToString();
                                        }
                                    }
                                }
                            }
                            if (aracBilgiNode.Name == "MOTORNO")
                            {
                                police.GenelBilgiler.PoliceArac.MotorNo = aracBilgiNode.InnerText;
                            }
                            if (aracBilgiNode.Name == "SASINO")
                            {
                                police.GenelBilgiler.PoliceArac.SasiNo = aracBilgiNode.InnerText;
                            }
                            if (aracBilgiNode.Name == "ARACTIPI")
                            {
                                police.GenelBilgiler.PoliceArac.AracinTipiKodu = aracBilgiNode.InnerText;
                            }
                            if (aracBilgiNode.Name == "ARACKULLANIMSEKLI")
                            {
                                police.GenelBilgiler.PoliceArac.KullanimSekli = aracBilgiNode.InnerText;
                            }
                            if (aracBilgiNode.Name == "KOLTUKADEDI" && !String.IsNullOrEmpty(aracBilgiNode.InnerText))
                            {
                                police.GenelBilgiler.PoliceArac.KoltukSayisi = Convert.ToInt32(aracBilgiNode.InnerText);
                            }
                            if (aracBilgiNode.Name == "URETIMYILI" && !String.IsNullOrEmpty(aracBilgiNode.InnerText))
                            {
                                police.GenelBilgiler.PoliceArac.Model = Convert.ToInt32(aracBilgiNode.InnerText);
                            }
                            if (aracBilgiNode.Name == "ARACDEGERKODU")
                            {
                                police.GenelBilgiler.PoliceArac.Marka = aracBilgiNode.InnerText;
                            }
                        }

                        #endregion

                        police.GenelBilgiler.Durum = 0;
                        police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.EUREKOSIGORTA;

                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                        if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;

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
