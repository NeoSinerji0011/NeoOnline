using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using static Neosinerji.BABOnlineTP.Business.PoliceTransfer.SFSExcelOrient;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class EthicaXmlReader : IPoliceTransferReader
    {
        IPoliceTransferService _IPoliceTransferService;
        IAktifKullaniciService _AktifKullanici;
        ITVMService _TVMService;
        private string message;
        private int tvmkodu;
        private string filePath;
        private List<BransUrun> _SigortaSirketiBransList;
        private List<Bran> _branslar;
        private bool TahsilatMi;
        //IBransUrunService _BransUrunService;

        public EthicaXmlReader()
        {
        }

        public EthicaXmlReader(string path, int tvmKodu, List<BransUrun> SigortaSirketiBransList, List<Bran> branslar)
        {
            this.filePath = path;
            this.tvmkodu = tvmKodu;
            this._SigortaSirketiBransList = SigortaSirketiBransList;
            this._branslar = branslar;
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();
            //_BransUrunService = DependencyResolver.Current.GetService<IBransUrunService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _AktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();

        }

        //public void download()
        //{
        //    string remoteIp = "88.247.127.91";
        //    string userName = "CEM ÇOPUR";
        //    string password = "EsOfu4105";
        //    string beginDate = "17.08.2021";
        //    string endDate = "30.08.2021";
        //    string url = "http://services.ethicasigorta.com:8080/Agency/AgencyService.asmx/GetAgencyProductions?remoteIp=" + remoteIp + "&userName=" + userName + "&password=" + password + "&beginDate=" + beginDate + "&endDate=" + endDate;
        //    downloadXmlAsync(url);

        //}

        //async System.Threading.Tasks.Task<bool> downloadXmlAsync(string url)
        //{
        //    // Download zip file
        //    var fileContent = new System.Net.WebClient().DownloadData(url); //byte[]
        //    var guid = Guid.NewGuid().ToString();
        //    var path = Directory.GetCurrentDirectory() + "/" + guid + ".zip";
        //    //System.IO.File.WriteAllBytesAsync(path, fileContent);
        //    FileStream fs = File.OpenWrite(path);


        //    fs.Write(fileContent, 0, fileContent.Length);
        //    // Extract from zip archive
        //    var extractPath = Directory.GetCurrentDirectory() + "/" + guid + "/";
        //    System.IO.Compression.ZipFile.ExtractToDirectory(path, extractPath);

        //    // Get files
        //    string[] xmlFiles = Directory.GetFiles(@extractPath, "*.xml", SearchOption.AllDirectories); // we need xmlFiles[0]

        //    string xmlContent = System.IO.File.ReadAllText(xmlFiles[0]);

        //    // @todo: delete template files

        //    return true;
        //    //return File(fileContent, "application/zip", Guid.NewGuid().ToString() + ".zip");
        //}

        public List<Police> getPoliceler()
        {
            List<Police> policeler = new List<Police>();

            XmlDocument doc = null;
            int carpan = 1;
            List<NeoOnline_TahsilatKapatma> policeTahsilatKapatma = new List<NeoOnline_TahsilatKapatma>();
            string[] tempPath = filePath.Split('#');
            if (tempPath.Length > 1)
            {
                policeTahsilatKapatma = Util.tahsilatDosayasiOkur(tempPath[1]);
                filePath = filePath.Substring(0, filePath.IndexOf("#"));

            }
            //_SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList("109");

            //_branslar = _BransUrunService.getBranslar();
            //tvmkodu = 100;
            string tumUrunAdi = null;
            string tumUrunKodu = null;
            string sLiKimlikNo = null;
            string sEttirenKimlikNo = null;
            try
            {
                #region Poliçe Reader

                doc = new XmlDocument();
                doc.Load(filePath);
                decimal dovizKuru = 1;
                XmlNode root = doc.FirstChild;
                XmlNode s = root;
                string odemeTipi = "Peşin";

                if (s.HasChildNodes)
                {
                    message = "Size:" + s.ChildNodes.Count;

                    for (int i = 0; i < s.ChildNodes.Count; i++)
                    {
                        if (s.ChildNodes[i].Name == "police")
                        {
                            XmlNode polinode = s.ChildNodes[i];
                            XmlNodeList polNode = polinode.ChildNodes;
                            Police police = new Police();
                            var tempVergiKimlikNo = "900" + new Random().Next(1000000, 9999999);
                            sEttirenKimlikNo = tempVergiKimlikNo;
                            for (int polNodeindx = 0; polNodeindx < polNode.Count; polNodeindx++)
                            {
                                string polNo = null;
                                string readerKulKodu = null;
                                string odemeAraci = null;
                                XmlNode policenode = polNode[polNodeindx];
                                carpan = 1;
                                odemeTipi = "Peşin";
                                foreach (XmlNode item in polNode)
                                {
                                    if (item.Name == "kart_bilgileri")
                                    {
                                        XmlNodeList res1 = item.ChildNodes;
                                        foreach (XmlNode item1 in res1)
                                        {
                                            XmlNodeList res2 = item1.ChildNodes;

                                            foreach (XmlNode item2 in res2)
                                            {
                                                if (item2.Name == "ad_soyad")
                                                {
                                                    if (!string.IsNullOrEmpty(item2.InnerText.ToString().Trim()))
                                                    {
                                                        odemeTipi = "Kredi Karti";
                                                    }
                                                }

                                            }
                                        }
                                    }
                                }
                                #region Genel Bilgiler
                                if (policenode.Name == "police_bilgileri")
                                {


                                    XmlNodeList gnlb = policenode.ChildNodes;
                                    for (int gnlbidx = 0; gnlbidx < gnlb.Count; gnlbidx++)
                                    {
                                        XmlNode gnlnode = gnlb[gnlbidx];
                                        if (tvmkodu > 0) police.GenelBilgiler.TVMKodu = tvmkodu;
                                        else police.GenelBilgiler.TVMKodu = 0;
                                        if (gnlnode.Name == "urun_kodu") tumUrunKodu = gnlnode.InnerText;
                                        if (gnlnode.Name == "urun_adi") tumUrunAdi = gnlnode.InnerText;
                                        if (gnlnode.Name == "police_no")
                                        {
                                            police.GenelBilgiler.PoliceNumarasi = gnlnode.InnerText;
                                            polNo = police.GenelBilgiler.PoliceNumarasi;
                                            foreach (var item in police.GenelBilgiler.PoliceTahsilats)
                                            {
                                                item.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                            }
                                        }
                                        if (gnlnode.Name == "ek_belge_no") police.GenelBilgiler.EkNo = Util.toInt(gnlnode.InnerText);
                                        police.GenelBilgiler.YenilemeNo = 0;
                                        police.GenelBilgiler.ZeyilAdi = "";
                                        //if (gnlnode.Name == "IptalIstihsal")
                                        //{
                                        //    if (gnlnode.InnerText == "IPTAL")
                                        //    {
                                        //        carpan = -1;
                                        //        police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * carpan;
                                        //        police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * carpan;
                                        //        police.GenelBilgiler.Komisyon = police.GenelBilgiler.Komisyon * carpan;
                                        //        police.GenelBilgiler.ToplamVergi = police.GenelBilgiler.ToplamVergi * carpan;
                                        //        foreach (var item in police.GenelBilgiler.PoliceVergis)
                                        //        {
                                        //            item.VergiTutari = item.VergiTutari * carpan;
                                        //        }
                                        //        foreach (var item in police.GenelBilgiler.PoliceOdemePlanis)
                                        //        {
                                        //            item.TaksitTutari = item.TaksitTutari * carpan;
                                        //        }
                                        //    }
                                        //}
                                        if (gnlnode.Name == "tanzim_tarihi") police.GenelBilgiler.TanzimTarihi = Convert.ToDateTime(gnlnode.InnerText);
                                        if (gnlnode.Name == "baslama_tarihi") police.GenelBilgiler.BaslangicTarihi = Util.toDate(gnlnode.InnerText);
                                        if (gnlnode.Name == "bitis_tarihi") police.GenelBilgiler.BitisTarihi = Util.toDate(gnlnode.InnerText);
                                        if (gnlnode.Name == "doviz_kod") police.GenelBilgiler.ParaBirimi = gnlnode.InnerText;
                                        if (gnlnode.Name == "ToplamVergiTutari") police.GenelBilgiler.ToplamVergi = carpan * Util.ToDecimal(gnlnode.InnerText);
                                        //if (gnlnode.Name == "DovizKuru")
                                        //{
                                        //    if (!String.IsNullOrEmpty(gnlnode.InnerText))
                                        //    {
                                        //        police.GenelBilgiler.DovizKur = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                        //    }
                                        //}
                                        //if (police.GenelBilgiler.ParaBirimi != "TL")
                                        //{
                                        //    if (gnlnode.Name == "DovizKuru")
                                        //    {
                                        //        dovizKuru = Util.ToDecimal(gnlnode.InnerText.Replace(".", ","));
                                        //    }
                                        //}
                                        if (police.GenelBilgiler.ParaBirimi != "TL" && police.GenelBilgiler.ParaBirimi != "YTL" && police.GenelBilgiler.ParaBirimi != null)
                                        {
                                            if (gnlnode.Name == "toplam_net_prim")
                                            {
                                                police.GenelBilgiler.DovizliNetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                //if (dovizKuru != 0 && dovizKuru != 1)
                                                //{
                                                //    police.GenelBilgiler.NetPrim = Math.Round(police.GenelBilgiler.NetPrim.Value * dovizKuru);
                                                //}
                                                if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                                {
                                                    police.GenelBilgiler.DovizliNetPrim = police.GenelBilgiler.DovizliNetPrim * 2;
                                                }
                                            }
                                            if (gnlnode.Name == "toplam_brut_prim")
                                            {
                                                police.GenelBilgiler.DovizliBrutPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                //if (dovizKuru != 0 && dovizKuru != 1)
                                                //{
                                                //    police.GenelBilgiler.BrutPrim = Math.Round(police.GenelBilgiler.BrutPrim.Value * dovizKuru, 2);
                                                //}
                                                if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                                {
                                                    police.GenelBilgiler.DovizliBrutPrim = police.GenelBilgiler.DovizliBrutPrim * 2;
                                                }
                                            }
                                            if (gnlnode.Name == "toplam_komisyon")
                                            {
                                                police.GenelBilgiler.DovizliKomisyon = carpan * Util.ToDecimal(gnlnode.InnerText);
                                                if (dovizKuru != 0 && dovizKuru != 1)
                                                {
                                                    //police.GenelBilgiler.Komisyon = Math.Round(police.GenelBilgiler.Komisyon.Value * dovizKuru);
                                                    dovizKuru = 0;
                                                }
                                            }
                                        }
                                        if (gnlnode.Name == "toplam_tl_net_prim")
                                        {
                                            police.GenelBilgiler.NetPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                            {
                                                police.GenelBilgiler.NetPrim = police.GenelBilgiler.NetPrim * 2;
                                            }
                                        }
                                        if (gnlnode.Name == "toplam_tl_brut_prim")
                                        {
                                            police.GenelBilgiler.BrutPrim = carpan * Util.ToDecimal(gnlnode.InnerText);
                                            if (police.GenelBilgiler.BransKodu == 12) //Tarım ise
                                            {
                                                police.GenelBilgiler.BrutPrim = police.GenelBilgiler.BrutPrim * 2;
                                            }
                                        }
                                        if (gnlnode.Name == "toplam_tl_komisyon") police.GenelBilgiler.Komisyon = carpan * Util.ToDecimal(gnlnode.InnerText);

                                        if (gnlnode.Name == "kullanici_id") readerKulKodu = gnlnode.InnerText;
                                        if (readerKulKodu != null)
                                        {
                                            var getReaderKodu = _IPoliceTransferService.GetPoliceReaderKullanicilari(readerKulKodu);
                                            if (getReaderKodu != null)
                                            {
                                                police.GenelBilgiler.TaliAcenteKodu = Convert.ToInt32(getReaderKodu.AltTvmKodu);

                                            }
                                        }
                                        //if (gnlnode.Name == "OdemeAraci")
                                        //{
                                        //    odemeAraci = gnlnode.InnerText;
                                        //}








                                    }
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



                                }
                                #endregion


                                #region Ödeme Planı


                                if (policenode.Name == "odeme_plani")
                                {

                                    decimal taksitTutari = 0;
                                    string OdemeBelgeNo = null;
                                    //sadece odeme tipi kontrolü için yazılmıştır.
                                    odemeAraci = odemeTipi;
                                    XmlNodeList tks = policenode.ChildNodes;
                                    var resTahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);

                                    for (int indx = 0; indx < tks.Count; indx++)
                                    {
                                        XmlNode elm = tks.Item(indx);
                                        XmlNodeList taksitelm = elm.ChildNodes;
                                        PoliceOdemePlani odm = new PoliceOdemePlani();
                                        PoliceTahsilat tahsilats = new PoliceTahsilat();
                                        //PoliceTahsilat tahsilatss = new PoliceTahsilat();                                       
                                        for (int taksitelmindx = 0; taksitelmindx < taksitelm.Count; taksitelmindx++)
                                        {
                                            XmlNode taksitnodes = taksitelm[taksitelmindx];
                                            odm.TaksitNo = indx + 1;
                                            if (taksitnodes.Name == "taksit_tarihi") odm.VadeTarihi = Util.toDate(taksitnodes.InnerText);
                                            if (taksitnodes.Name == "brut_tutar")
                                            {
                                                odm.TaksitTutari = carpan * Util.ToDecimal(taksitnodes.InnerText);
                                                if (police.GenelBilgiler.DovizKur != 1 && police.GenelBilgiler.DovizKur != 0 && police.GenelBilgiler.DovizKur != null)
                                                {
                                                    odm.TaksitTutari = Math.Round(Util.ToDecimal(elm["brut_tutar"].InnerText) * police.GenelBilgiler.DovizKur.Value, 2) * carpan;
                                                    odm.DovizliTaksitTutari = carpan * Util.ToDecimal(elm["brut_tutar"].InnerText);
                                                }
                                            }
                                            taksitTutari = Convert.ToDecimal(odm.TaksitTutari);

                                        }
                                        if (odm.TaksitTutari != 0 && odm.TaksitTutari != null)
                                        {
                                            var ayniTaksitVarmi = police.GenelBilgiler.PoliceOdemePlanis.Where(y => y.TaksitNo == odm.TaksitNo).FirstOrDefault();
                                            var ayniTaksitVarmii = police.GenelBilgiler.PoliceTahsilats.Where(y => y.TaksitNo == odm.TaksitNo).FirstOrDefault();

                                            if (ayniTaksitVarmi == null)
                                            {
                                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);

                                            }
                                            else
                                            {
                                                odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                police.GenelBilgiler.PoliceOdemePlanis.Remove(ayniTaksitVarmi);
                                                police.GenelBilgiler.PoliceOdemePlanis.Add(odm);
                                            }

                                            if (ayniTaksitVarmii == null)
                                            {// odeme belge kk harıcı yazı alıyor acik gibi , pop da - olan poltah da + 
                                                #region tahsilat
                                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ETHICA, police.GenelBilgiler.BransKodu.Value);

                                                if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                                {
                                                    int otoOdeSayac = 0;
                                                    foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                                    {
                                                        if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                        {
                                                            otoOdeSayac++;
                                                            //PoliceTahsilat tahsilats = new PoliceTahsilat();

                                                            tahsilats.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                            odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                            if (tahsilats.OdemTipi == 1)
                                                            {
                                                                tahsilats.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                                tahsilats.KalanTaksitTutari = 0;
                                                                tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                                if (OdemeBelgeNo == null)
                                                                {
                                                                    tahsilats.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                                                }
                                                                tahsilats.OtomatikTahsilatiKkMi = 1;
                                                            }
                                                            else
                                                            {
                                                                tahsilats.OdenenTutar = 0;
                                                                tahsilats.KalanTaksitTutari = odm.TaksitTutari;
                                                            }
                                                            tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilats.TaksitNo = odm.TaksitNo;
                                                            tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                            tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                            tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                            tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                            tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                            tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                            tahsilats.KayitTarihi = DateTime.Today;
                                                            tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    #region Tahsilat işlemi

                                                    // PoliceTahsilat tahsilatss = new PoliceTahsilat();
                                                    // tahsilat = new PoliceTahsilat();
                                                    if (odemeAraci == "Kredi Karti")
                                                    {
                                                        tahsilats.OdemTipi = OdemeTipleri.KrediKarti;
                                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                        tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                        if (OdemeBelgeNo == null)
                                                        {
                                                            tahsilats.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                                        }
                                                        tahsilats.KalanTaksitTutari = 0;
                                                        tahsilats.OdenenTutar = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                        tahsilats.OtomatikTahsilatiKkMi = 1;
                                                    }
                                                    else if (odemeAraci == "Peşin")
                                                    {
                                                        tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                        odm.OdemeTipi = OdemeTipleri.Nakit;
                                                        tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                        tahsilats.OdenenTutar = 0;
                                                        tahsilats.OdemeBelgeNo = null;
                                                    }
                                                    else
                                                    {
                                                        tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                        odm.OdemeTipi = OdemeTipleri.Nakit;
                                                        tahsilats.OdemeBelgeNo = null;
                                                        tahsilats.KalanTaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                        tahsilats.OdenenTutar = 0;
                                                    }
                                                    tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilats.TaksitNo = odm.TaksitNo;
                                                    tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                    tahsilats.TaksitTutari = odm.TaksitTutari.HasValue ? odm.TaksitTutari.Value : 0;
                                                    tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilats.KayitTarihi = DateTime.Today;
                                                    tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                    #endregion
                                                }
                                                #endregion
                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilats);

                                            }
                                            else
                                            {
                                                odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                tahsilats.TaksitTutari = odm.TaksitTutari.Value;
                                                police.GenelBilgiler.PoliceTahsilats.Remove(ayniTaksitVarmii);
                                                #region tahsilat
                                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ETHICA, police.GenelBilgiler.BransKodu.Value);
                                                if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                                {
                                                    int otoOdeSayac = 0;
                                                    foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                                    {
                                                        if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                        {
                                                            otoOdeSayac++;
                                                            //PoliceTahsilat tahsilats = new PoliceTahsilat();

                                                            tahsilats.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                            odm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                            if (tahsilats.OdemTipi == 1)
                                                            {
                                                                tahsilats.OdenenTutar = taksitTutari + ayniTaksitVarmi.TaksitTutari.Value;
                                                                tahsilats.KalanTaksitTutari = 0;
                                                                tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                                if (OdemeBelgeNo == null)
                                                                {
                                                                    tahsilats.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                                                }
                                                                tahsilats.OtomatikTahsilatiKkMi = 1;
                                                            }
                                                            else
                                                            {
                                                                tahsilats.OdenenTutar = 0;
                                                                tahsilats.KalanTaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                            }
                                                            tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                            tahsilats.TaksitNo = odm.TaksitNo;
                                                            tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                            odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                            tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                            tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                            tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                            tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                            tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                            tahsilats.KayitTarihi = DateTime.Today;
                                                            tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    #region Tahsilat işlemi
                                                    // PoliceTahsilat tahsilatss = new PoliceTahsilat();
                                                    // tahsilat = new PoliceTahsilat();
                                                    if (odemeAraci == "Kredi Karti")
                                                    {
                                                        tahsilats.OdemTipi = OdemeTipleri.KrediKarti;
                                                        odm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                        tahsilats.OdemeBelgeNo = OdemeBelgeNo;
                                                        tahsilats.KalanTaksitTutari = 0;
                                                        tahsilats.OdenenTutar = taksitTutari + ayniTaksitVarmi.TaksitTutari.Value;
                                                        tahsilats.OtomatikTahsilatiKkMi = 1;
                                                        if (OdemeBelgeNo == null)
                                                        {
                                                            tahsilats.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                                        }
                                                    }
                                                    else if (odemeAraci == "Peşin")
                                                    {
                                                        tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                        odm.OdemeTipi = OdemeTipleri.Nakit;
                                                        tahsilats.KalanTaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                        tahsilats.OdenenTutar = 0;
                                                        tahsilats.OdemeBelgeNo = null;
                                                    }
                                                    else
                                                    {
                                                        tahsilats.OdemTipi = OdemeTipleri.Nakit;
                                                        odm.OdemeTipi = OdemeTipleri.Nakit;
                                                        tahsilats.OdemeBelgeNo = null;
                                                        tahsilats.KalanTaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                        tahsilats.OdenenTutar = 0;
                                                    }
                                                    tahsilats.TaksitVadeTarihi = odm.VadeTarihi.HasValue ? odm.VadeTarihi.Value : police.GenelBilgiler.BaslangicTarihi.Value;
                                                    tahsilats.TaksitNo = odm.TaksitNo;
                                                    tahsilats.OdemeBelgeTarihi = odm.VadeTarihi;
                                                    odm.TaksitTutari = taksitTutari + ayniTaksitVarmi.TaksitTutari;
                                                    tahsilats.PoliceNo = police.GenelBilgiler.PoliceNumarasi;
                                                    tahsilats.ZeyilNo = police.GenelBilgiler.EkNo.ToString();
                                                    tahsilats.KimlikNo = !String.IsNullOrEmpty(sEttirenKimlikNo) ? sEttirenKimlikNo : sLiKimlikNo;
                                                    tahsilats.BrutPrim = police.GenelBilgiler.BrutPrim.HasValue ? police.GenelBilgiler.BrutPrim.Value : 0;
                                                    tahsilats.PoliceId = police.GenelBilgiler.PoliceId;
                                                    tahsilats.KayitTarihi = DateTime.Today;
                                                    tahsilats.KaydiEkleyenKullaniciKodu = police.GenelBilgiler.TVMKodu.Value;

                                                    #endregion
                                                }
                                                #endregion

                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilats);
                                            }


                                        }
                                    }
                                    //oto odeme tipinden cekılecek
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
                                            odmm.OdemeTipi = OdemeTipleri.Havale;
                                            odmm.TaksitNo = 1;
                                            if (odmm.TaksitTutari != 0 && odmm.TaksitTutari != null)
                                            {
                                                police.GenelBilgiler.PoliceOdemePlanis.Add(odmm);
                                                PoliceTahsilat tahsilat = new PoliceTahsilat();

                                                var tanimliBransOdemeTipleri = _TVMService.GetListTanımliBransTvmSirketOdemeTipleri(tvmkodu, SigortaSirketiBirlikKodlari.ETHICA, police.GenelBilgiler.BransKodu.Value);
                                                var restahsilatKapatmaVarmi = tahsilatKapatmaVarmi(policeTahsilatKapatma, police.GenelBilgiler);

                                                if (tanimliBransOdemeTipleri != null && tanimliBransOdemeTipleri.Count > 0)
                                                {
                                                    int otoOdeSayac = 0;
                                                    foreach (var itemOtoOdemeTipleri in tanimliBransOdemeTipleri)
                                                    {
                                                        if (otoOdeSayac < 1 && police.GenelBilgiler.BransKodu == itemOtoOdemeTipleri.BransKodu)
                                                        {
                                                            otoOdeSayac++;
                                                            //PoliceTahsilat tahsilat = new PoliceTahsilat();

                                                            tahsilat.OdemTipi = itemOtoOdemeTipleri.OdemeTipi;
                                                            odmm.OdemeTipi = Convert.ToByte(itemOtoOdemeTipleri.OdemeTipi);
                                                            if (tahsilat.OdemTipi == 1)
                                                            {
                                                                tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                                tahsilat.KalanTaksitTutari = 0;
                                                                tahsilat.OdemeBelgeNo = OdemeBelgeNo;
                                                                if (OdemeBelgeNo == null)
                                                                {
                                                                    tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                                                }
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
                                                                //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                                    police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    #region Tahsilat işlemi
                                                    // PoliceTahsilat tahsilatss = new PoliceTahsilat();
                                                    // tahsilat = new PoliceTahsilat();
                                                    if (odemeAraci == "Kredi Karti")
                                                    {
                                                        tahsilat.OdemTipi = OdemeTipleri.KrediKarti;
                                                        odmm.OdemeTipi = OdemeTipleri.KrediKarti;
                                                        tahsilat.OdemeBelgeNo = OdemeBelgeNo;
                                                        tahsilat.KalanTaksitTutari = 0;
                                                        tahsilat.OdenenTutar = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                        tahsilat.OtomatikTahsilatiKkMi = 1;
                                                        if (OdemeBelgeNo == null)
                                                        {
                                                            tahsilat.OdemeBelgeNo = string.IsNullOrEmpty(resTahsilatKapatmaVarmi) ? "111111****1111" : resTahsilatKapatmaVarmi;
                                                        }
                                                    }
                                                    else if (odemeAraci == "Peşin")
                                                    {
                                                        tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                        odmm.OdemeTipi = OdemeTipleri.Nakit;
                                                        tahsilat.KalanTaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                        tahsilat.OdenenTutar = 0;
                                                        tahsilat.OdemeBelgeNo = null;
                                                    }
                                                    else
                                                    {
                                                        tahsilat.OdemTipi = OdemeTipleri.Nakit;
                                                        odmm.OdemeTipi = OdemeTipleri.Nakit;
                                                        tahsilat.OdemeBelgeNo = null;
                                                        tahsilat.KalanTaksitTutari = odmm.TaksitTutari.HasValue ? odmm.TaksitTutari.Value : 0;
                                                        tahsilat.OdenenTutar = 0;
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
                                                        //if (!string.IsNullOrEmpty(resTahsilatKapatmaVarmi))
                                                            police.GenelBilgiler.PoliceTahsilats.Add(tahsilat);
                                                    }
                                                    #endregion

                                                }

                                            }
                                        }

                                    }

                                }
                                #endregion


                                #region Araç Bilgileri ( Soru cevap )

                                else if (policenode.Name == "soru_cevap_listesi")
                                {
                                    XmlNodeList tarifeSoru = policenode.ChildNodes;

                                    for (int indx = 0; indx < tarifeSoru.Count; indx++)
                                    {
                                        XmlNode elm = tarifeSoru.Item(indx);
                                        XmlNodeList tarifeSoruElm = elm.ChildNodes;
                                        for (int taksitelmindx = 0; taksitelmindx < tarifeSoruElm.Count; taksitelmindx++)
                                        {
                                            XmlNode taksitnodes = tarifeSoruElm[taksitelmindx];

                                            if (taksitnodes.InnerText == "MRK")
                                            {

                                                police.GenelBilgiler.PoliceArac.Marka = tarifeSoruElm[taksitelmindx + 1].InnerText;
                                                police.GenelBilgiler.PoliceArac.MarkaAciklama = tarifeSoruElm[taksitelmindx + 3].InnerText;
                                            }

                                            if (taksitnodes.InnerText == "MDL")
                                            {
                                                if (int.TryParse(tarifeSoruElm[taksitelmindx + 3].InnerText, out int model))
                                                {
                                                    police.GenelBilgiler.PoliceArac.Model = model;
                                                }
                                            }

                                            if (taksitnodes.InnerText == "KŞK")
                                            {
                                                police.GenelBilgiler.PoliceArac.KullanimSekli = tarifeSoruElm[taksitelmindx + 3].InnerText;
                                            }

                                            if (taksitnodes.InnerText == "TRL")
                                            {
                                                police.GenelBilgiler.PoliceArac.KullanimTarzi = tarifeSoruElm[taksitelmindx + 3].InnerText;
                                            }



                                            if (taksitnodes.InnerText == "CİN")
                                            {
                                                police.GenelBilgiler.PoliceSigortali.Cinsiyet = tarifeSoruElm[taksitelmindx + 1].InnerText;
                                                police.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = tarifeSoruElm[taksitelmindx + 1].InnerText;


                                            }


                                        }
                                    }
                                }
                                if (policenode.Name == "matbu_listesi")
                                {
                                    XmlNodeList tarifeSoru = policenode.ChildNodes;

                                    for (int indx = 0; indx < tarifeSoru.Count; indx++)
                                    {
                                        XmlNode elm = tarifeSoru.Item(indx);
                                        XmlNodeList tarifeSoruElm = elm.ChildNodes;
                                        for (int taksitelmindx = 0; taksitelmindx < tarifeSoruElm.Count; taksitelmindx++)
                                        {
                                            XmlNode taksitnodes = tarifeSoruElm[taksitelmindx];

                                            //if (taksitnodes.InnerText == "EAC")
                                            //{
                                            //    police.GenelBilgiler.TaliAcenteKodu = Convert.ToInt32(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                            //}
                                            if (taksitnodes.InnerText == "PLK")
                                            {
                                                var temp = plakaAyrac(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                police.GenelBilgiler.PoliceArac.PlakaKodu = temp[0];
                                                police.GenelBilgiler.PoliceArac.PlakaNo = temp[1];
                                            }
                                            if (taksitnodes.InnerText == "EGM")
                                            {
                                                var temp = tescilAyrac(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                police.GenelBilgiler.PoliceArac.TescilSeriKod = temp[0];
                                                police.GenelBilgiler.PoliceArac.TescilSeriNo = temp[1];
                                            }

                                            if (taksitnodes.InnerText == "MOT")
                                            {
                                                police.GenelBilgiler.PoliceArac.MotorNo = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                            }
                                            if (taksitnodes.InnerText == "ŞAS")
                                            {
                                                police.GenelBilgiler.PoliceArac.SasiNo = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                            }
                                            if (taksitnodes.InnerText == "TTT")
                                            {
                                                if (!String.IsNullOrEmpty(tarifeSoruElm[taksitelmindx + 2].InnerText))
                                                {
                                                    police.GenelBilgiler.PoliceArac.TrafikTescilTarihi = Convert.ToDateTime(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                }
                                            }
                                            if (taksitnodes.InnerText == "YEN")
                                            {
                                                if (!String.IsNullOrEmpty(tarifeSoruElm[taksitelmindx + 2].InnerText))
                                                {
                                                    police.GenelBilgiler.YenilemeNo = Convert.ToInt32(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                }
                                            }

                                            if (taksitnodes.InnerText == "REN")
                                            {
                                                police.GenelBilgiler.PoliceArac.Renk = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                            }

                                            if (taksitnodes.InnerText == "HDN")
                                            {
                                                police.GenelBilgiler.PoliceArac.TramerBelgeNo = tarifeSoruElm[taksitelmindx + 2].InnerText;
                                            }

                                            if (taksitnodes.InnerText == "HDT")
                                            {
                                                if (!string.IsNullOrEmpty(tarifeSoruElm[taksitelmindx + 2].InnerText))
                                                {
                                                    police.GenelBilgiler.PoliceArac.TramerBelgeTarihi = Convert.ToDateTime(tarifeSoruElm[taksitelmindx + 2].InnerText);
                                                }
                                            }




                                        }
                                    }
                                }

                                #endregion

                                #region Vergiler

                                if (policenode.Name == "vergi_listesi")
                                {
                                    XmlNodeList Vergilernode = policenode.ChildNodes;
                                    PoliceVergi gv = new PoliceVergi();
                                    decimal bsmv = 0, thgf = 0, gh = 0;
                                    for (int idx = 0; idx < Vergilernode.Count; idx++)
                                    {
                                        XmlNode vergi = Vergilernode.Item(idx);

                                        bool kontrol = true;
                                        XmlNodeList verginodes = vergi.ChildNodes;
                                        for (int verginodesindx = 0; verginodesindx < verginodes.Count; verginodesindx++)
                                        {
                                            XmlNode vergin = verginodes[verginodesindx];
                                            if (vergin.InnerText == "BSMV")
                                            {
                                                bsmv += carpan * Util.ToDecimal(verginodes[verginodesindx + 3].InnerText);

                                            }
                                            if (vergin.InnerText == "GH")
                                            {
                                                gh += carpan * Util.ToDecimal(verginodes[verginodesindx + 3].InnerText);

                                            }
                                            if (vergin.InnerText == "THGF")
                                            {
                                                thgf += carpan * Util.ToDecimal(verginodes[verginodesindx + 3].InnerText);

                                            }

                                        }
                                    }
                                    gv = new PoliceVergi();
                                    gv.VergiKodu = 2;
                                    gv.VergiTutari = bsmv;
                                    police.GenelBilgiler.PoliceVergis.Add(gv);
                                    gv = new PoliceVergi();
                                    gv.VergiKodu = 1;
                                    gv.VergiTutari = gh;
                                    police.GenelBilgiler.PoliceVergis.Add(gv);
                                    gv = new PoliceVergi();
                                    gv.VergiKodu = 3;
                                    gv.VergiTutari = thgf;
                                    police.GenelBilgiler.PoliceVergis.Add(gv);


                                }
                                #endregion




                                #region Teminatlar
                                if (policenode.Name == "teminat_listesi")
                                {
                                    XmlNodeList Vergilernode = policenode.ChildNodes;
                                    PoliceVergi gv = new PoliceVergi();
                                    decimal netprim = 0, komisyon = 0;
                                    for (int idx = 0; idx < Vergilernode.Count; idx++)
                                    {
                                        XmlNode vergi = Vergilernode.Item(idx);

                                        bool kontrol = true;
                                        XmlNodeList verginodes = vergi.ChildNodes;
                                        for (int verginodesindx = 0; verginodesindx < verginodes.Count; verginodesindx++)
                                        {
                                            XmlNode vergin = verginodes[verginodesindx];


                                            if (vergin.Name == "tl_net_prim")
                                            {
                                                netprim += Util.ToDecimal(vergin.InnerText);
                                                police.GenelBilgiler.NetPrim = carpan * netprim;

                                            }
                                            if (vergin.Name == "tl_komisyon")
                                            {
                                                komisyon += Util.ToDecimal(vergin.InnerText);
                                                police.GenelBilgiler.Komisyon = carpan * komisyon;

                                            }
                                            //police.GenelBilgiler.BrutPrim = police.GenelBilgiler.NetPrim + police.GenelBilgiler.ToplamVergi;


                                        }
                                    }


                                }
                                #endregion

                                #region Müşteri Bilgileri

                                if (policenode.Name == "musteri_listesi")
                                {

                                    string tempFirmaAd = "";

                                    XmlNodeList musterinode = policenode.ChildNodes;
                                    string pasaportno = "";
                                    for (int idx = 0; idx < musterinode.Count; idx++)
                                    {
                                        XmlNode sigortanode = musterinode.Item(idx);

                                        XmlNodeList sigortanodelist = sigortanode.ChildNodes;
                                        for (int sigortanodelistindx = 0; sigortanodelistindx < sigortanodelist.Count; sigortanodelistindx++)
                                        {
                                            XmlNode nodes = sigortanodelist[sigortanodelistindx];


                                            if (nodes.InnerText == "Sigorta Ettiren")
                                            {
                                                police.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = sigortanodelist[sigortanodelistindx + 1].InnerText;

                                                police.GenelBilgiler.PoliceSigortaEttiren.TelefonNo = sigortanodelist[sigortanodelistindx + 4].InnerText;

                                                police.GenelBilgiler.PoliceSigortaEttiren.Adres = sigortanodelist[sigortanodelistindx + 2].InnerText;
                                                if (sigortanodelist[0].InnerText == "Firma")
                                                {
                                                    //if (tempFirmaAd != sigortanodelist[sigortanodelistindx + 1].InnerText)
                                                    //    tempVergiKimlikNo = "900" + new Random().Next(1000000, 9999999);
                                                    //police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = tempVergiKimlikNo;

                                                    if (!string.IsNullOrEmpty(sigortanodelist[sigortanodelistindx + 3].InnerText.Trim()))
                                                    {
                                                        police.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = sigortanodelist[sigortanodelistindx + 3].InnerText.Trim();
                                                    }
                                                    else
                                                    {
                                                        police.GenelBilgiler.YenilemeNo = null;
                                                        break;
                                                    }

                                                    //foreach (var item in police.GenelBilgiler.PoliceTahsilats)
                                                    //{
                                                    //    item.KimlikNo = "";
                                                    //}
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(sigortanodelist[sigortanodelistindx + 3].InnerText.Trim()))
                                                    {
                                                        police.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = sigortanodelist[sigortanodelistindx + 3].InnerText.Trim();
                                                    }
                                                    else
                                                    {
                                                        police.GenelBilgiler.YenilemeNo = null;
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (nodes.InnerText == "Sigortalı")
                                            {
                                                police.GenelBilgiler.PoliceSigortali.AdiUnvan = sigortanodelist[sigortanodelistindx + 1].InnerText;

                                                police.GenelBilgiler.PoliceSigortali.TelefonNo = sigortanodelist[sigortanodelistindx + 4].InnerText;

                                                police.GenelBilgiler.PoliceSigortali.Adres = sigortanodelist[sigortanodelistindx + 2].InnerText;
                                                if (sigortanodelist[0].InnerText == "Firma")
                                                {
                                                    if (!string.IsNullOrEmpty(sigortanodelist[sigortanodelistindx + 3].InnerText.Trim()))
                                                    {
                                                        police.GenelBilgiler.PoliceSigortali.VergiKimlikNo = sigortanodelist[sigortanodelistindx + 3].InnerText.Trim();
                                                    }
                                                    else
                                                    {
                                                        police.GenelBilgiler.YenilemeNo = null;
                                                        break;
                                                    }
                                                    //foreach (var item in police.GenelBilgiler.PoliceTahsilats)
                                                    //{
                                                    //    item.KimlikNo = "";
                                                    //}
                                                }
                                                else
                                                {
                                                    if (!string.IsNullOrEmpty(sigortanodelist[sigortanodelistindx + 3].InnerText.Trim()))
                                                    {
                                                        police.GenelBilgiler.PoliceSigortali.KimlikNo = sigortanodelist[sigortanodelistindx + 3].InnerText.Trim();
                                                    }
                                                    else
                                                    {
                                                        police.GenelBilgiler.YenilemeNo = null;
                                                        break;
                                                    }
                                                }
                                                tempFirmaAd = sigortanodelist[sigortanodelistindx + 1].InnerText;
                                            }

                                        }

                                    }
                                }
                                #endregion

                                #region kart bilgileri

                                if (policenode.Name == "kart_bilgileri")
                                {
                                    XmlNodeList tarifeSoru = policenode.ChildNodes;
                                    string kartNumarasi = "";
                                    for (int indx = 0; indx < tarifeSoru.Count; indx++)
                                    {
                                        XmlNode elm = tarifeSoru.Item(indx);
                                        XmlNodeList tarifeSoruElm = elm.ChildNodes;
                                        for (int taksitelmindx = 0; taksitelmindx < tarifeSoruElm.Count; taksitelmindx++)
                                        {
                                            XmlNode taksitnodes = tarifeSoruElm[taksitelmindx];
                                            if (odemeTipi == "Kredi Kartı")
                                            {
                                                if (taksitnodes.Name == "kart_ilk6")
                                                {
                                                    kartNumarasi = taksitnodes.InnerText;
                                                }
                                                if (taksitnodes.Name == "kart_son4")
                                                {
                                                    kartNumarasi += "******" + taksitnodes.InnerText;
                                                }

                                            }

                                        }
                                    }

                                }
                                #endregion
                            }
                            police.GenelBilgiler.Durum = 0;
                            police.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.ETHICA;

                            //// Odeme Sekli
                            if (police.GenelBilgiler.PoliceOdemePlanis.Count == 0) police.GenelBilgiler.OdemeSekli = 0;
                            if (police.GenelBilgiler.PoliceOdemePlanis.Count == 1) police.GenelBilgiler.OdemeSekli = 1;
                            if (police.GenelBilgiler.PoliceOdemePlanis.Count > 1) police.GenelBilgiler.OdemeSekli = 2;
                            foreach (var itemv in police.GenelBilgiler.PoliceTahsilats)
                            {
                                itemv.BrutPrim = police.GenelBilgiler.BrutPrim.Value;
                                itemv.TaksitTutari = (itemv.TaksitTutari) * carpan;
                                itemv.KalanTaksitTutari = (itemv.KalanTaksitTutari) * carpan;
                                itemv.OdenenTutar = (itemv.OdenenTutar) * carpan;
                            }


                            policeler.Add(police);
                        }
                    }
                    #endregion
                }
            }

            catch (Exception ex)
            {
                this.message = ex.ToString();
                policeler = null;
            }
            return policeler;
        }
        string[] plakaAyrac(string val)
        {
            string temp = "", temp1 = "";
            for (int i = 0; i < val.Length; i++)
            {
                if (int.TryParse(val[i].ToString(), out int a))
                {
                    temp += val[i].ToString();
                }
                else
                {
                    temp1 = val.Substring(i);
                    break;

                }
            }
            return new string[] { temp, temp1 };
        }
        string[] tescilAyrac(string val)
        {
            string temp = "", temp1 = "";
            for (int i = 0; i < val.Length; i++)
            {
                if (int.TryParse(val[i].ToString(), out int a))
                {
                    temp1 += val[i].ToString();
                }
                else
                {
                    temp += val[i].ToString();
                }
            }
            return new string[] { temp, temp1 };
        }
        public string getMessage()
        {
            return this.message;
        }

        public bool getTahsilatMi()
        {
            return this.TahsilatMi;
        }
        string tahsilatKapatmaVarmi(List<NeoOnline_TahsilatKapatma> neoOnline_TahsilatKapatmas = null, PoliceGenel police = null)
        {
            foreach (var item in neoOnline_TahsilatKapatmas)
            {
                if (item.Police_No.Trim() == police.PoliceNumarasi.Trim() && item.Yenileme_No.Trim() == police.YenilemeNo.Value.ToString().Trim() && item.Ek_No.Trim() == police.EkNo.Value.ToString().Trim() && !_TVMService.CheckListTVMBankaCariHesaplari(_AktifKullanici.TVMKodu, 5, item.Kart_No.Trim()))
                {
                    return item.Kart_No.Trim();
                }
            }
            return "";
        }


    }
}
