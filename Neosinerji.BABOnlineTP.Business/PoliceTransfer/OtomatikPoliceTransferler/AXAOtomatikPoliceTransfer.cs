using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using DataSharetest.DataShare;
using System.IO;
using Neosinerji.BABOnlineTP.Business.axasigorta.hayat;
using Neosinerji.BABOnlineTP.Business.Common;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler
{
    public class AXAOtomatikPoliceTransfer : IAXAOtomatikPoliceTransfer
    {
        private int _tvmKodu;
        private string _sirketKodu;
        private string _serviceURL;
        private string _KullaniciAdi;
        private string _Sifre;
        private DateTime _TanzimBaslangicTarihi;
        private DateTime _TanzimBitisTarihi;
        IBransUrunService _BransUrunService;

        public AXAOtomatikPoliceTransfer(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi)
        {
            this._tvmKodu = tvmKodu;
            this._sirketKodu = sirketKodu;
            this._serviceURL = serviceURL;
            this._KullaniciAdi = KullaniciAdi;
            this._Sifre = Sifre;
            this._TanzimBaslangicTarihi = TanzimBaslangicTarihi;
            this._TanzimBitisTarihi = TanzimBitisTarihi;
        }
        public List<Police> GetAXAAutoPoliceTransfer()
        {
            #region Service DependencyResolver

            IPoliceTransferService _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();

            IPoliceContext _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();
            _IPoliceContext = DependencyResolver.Current.GetService<IPoliceContext>();

            IPoliceTransferStorage _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();
            _IPoliceTransferStorage = DependencyResolver.Current.GetService<IPoliceTransferStorage>();

            IAktifKullaniciService _IAktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _IAktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();

            ITVMService _ITVMService = DependencyResolver.Current.GetService<ITVMService>();


            #endregion

            try
            {
                List<Police> policeler = new List<Police>();
                List<Police> returnPoliceler = new List<Police>();
                Police policeItem = new Police();
                using (AO_DataShare_Service01 v_Service = new AO_DataShare_Service01())
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                    //v_Service.Url = @"https://datashare.axasigorta.com.tr/AO_DataShareWS/AO_DataShare_Service01.asmx";
                    v_Service.Url = _serviceURL;
                    v_Service.Credentials = new NetworkCredential(_KullaniciAdi, _Sifre, "AXA");

                    #region Proxy
                    int anaTVMKodu = Convert.ToInt32(_IAktifKullaniciService.TVMKodu.ToString().Substring(0, 3));
                    var neoConnectKullanici = _ITVMService.GetNeoConnectKullanici(anaTVMKodu, 2); // 2 - AXA TUM Kodu
                    var proxyCredentialsArray = neoConnectKullanici.ProxyIpPort.Split(':');

                    string proxyIp = proxyCredentialsArray[0];
                    int proxyPort = Convert.ToInt32(proxyCredentialsArray[1]);
                    ICredentials credentials = new NetworkCredential(neoConnectKullanici.ProxyKullaniciAdi, neoConnectKullanici.ProxySifre);

                    v_Service.Proxy = new WebProxy(proxyIp + ":" + proxyPort, false, null, credentials);
                    #endregion

                    XmlNode v_Ret = v_Service.GetPoliciesList(1, _TanzimBaslangicTarihi.ToString().Replace(".", "/"), _TanzimBitisTarihi.ToString().Replace(".", "/"), 1);

                    AxaPoliceListModel polItemModel = new AxaPoliceListModel();
                    List<AxaPoliceListModel> polListModel = new List<AxaPoliceListModel>();
                    XmlNodeList responsePoliceList = v_Ret.ChildNodes;
                    for (int i = 0; i < responsePoliceList.Count; i++)
                    {
                        var polices = responsePoliceList[i].ChildNodes;

                        for (int j = 0; j < polices.Count; j++)
                        {
                            if (polices[j].Name == "CARI_POL_NO")
                            {
                                polItemModel = new AxaPoliceListModel();
                                polItemModel.PoliceNo = Convert.ToInt32(polices[j].InnerText);
                            }
                            if (polices[j].Name == "ZEYL_SIRA_NO")
                            {
                                polItemModel.ZeylSiraNo = Convert.ToInt32(polices[j].InnerText);
                            }

                        }
                        polListModel.Add(polItemModel);

                    }
                    for (int i = 0; i < polListModel.Count; i++)
                    {

                        XmlNode responseXML = v_Service.GetPolicyDetails(1, polListModel[i].PoliceNo, polListModel[i].ZeylSiraNo);

                        policeler = _IPoliceTransferService.getAxaPoliceler(_sirketKodu, responseXML, _tvmKodu);
                        if (policeler != null)
                        {
                            //string fileName = String.Format("Axa_{0}.xml", _TanzimBaslangicTarihi.ToString("dd.MM.yyyy") + "_" + _TanzimBitisTarihi.ToString("dd.MM.yyyy") + "_" + System.Guid.NewGuid().ToString("N"));
                            //url = _IPoliceTransferStorage.UploadXml(fileName, responseXML.OuterXml);

                            //AutoPoliceTransfer item = new AutoPoliceTransfer();
                            //item.TvmKodu = _tvmKodu;
                            //item.SirketKodu = _sirketKodu;
                            //item.PoliceTransferUrl = url;
                            //item.TanzimBaslangicTarihi = _TanzimBaslangicTarihi;
                            //item.TanzimBitisTarihi = _TanzimBitisTarihi;
                            //item.KayitTarihi = TurkeyDateTime.Now;
                            //item.KaydiEkleyenKullaniciKodu = _IAktifKullaniciService.KullaniciKodu;

                            //_IPoliceContext.AutoPoliceTransferRepository.Create(item);
                            //_IPoliceContext.Commit();

                            for (int j = 0; j < policeler.Count; j++)
                            {
                                policeItem = new Police();

                                policeItem = policeler[j];

                            }
                            string[] tempTescil;
                            if (policeItem.GenelBilgiler.PoliceArac.TescilSeriNo != null)
                            {
                                tempTescil = TescilPart(policeItem.GenelBilgiler.PoliceArac.TescilSeriNo);
                                policeItem.GenelBilgiler.PoliceArac.TescilSeriKod = tempTescil[0];
                                policeItem.GenelBilgiler.PoliceArac.TescilSeriNo = tempTescil[1];
                            }
                            if (policeItem.GenelBilgiler.BransKodu != 1 && policeItem.GenelBilgiler.BransKodu != 2)
                            {
                                policeItem.GenelBilgiler.PoliceArac = null;
                            }
                            returnPoliceler.Add(policeItem);
                        }
                        else
                        {
                            _IPoliceTransferService.setMessage("Otomatik poliçe transfer edilirken bir sorun oluştu.");
                        }
                    }
                }

                return returnPoliceler;
            }
            catch (Exception ex)
            {

                _IPoliceTransferService.setMessage("Otomatik poliçe transfer edilirken bir sorun oluştu. Detay" + ex.Message.ToString());
                return null;
                throw;
            }
        }

        public List<Police> GetAXAHayatAutoPoliceTransfer()
        {
            #region Service DependencyResolver
            _BransUrunService = DependencyResolver.Current.GetService<IBransUrunService>();
            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
            SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList(_sirketKodu);

            List<Bran> branslar = new List<Bran>();
            branslar = _BransUrunService.getBranslar();

            IPoliceTransferService _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            _IPoliceTransferService = DependencyResolver.Current.GetService<IPoliceTransferService>();
            #endregion

            axasigorta.hayat.AOHayatDataShare hayatService = new axasigorta.hayat.AOHayatDataShare();
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            hayatService.Url = _serviceURL;
            //hayatService.Url = @"https://datashare.axasigorta.com.tr/AOPublicServices_v5/AOHayatDataShare.asmx";
            hayatService.Credentials = new NetworkCredential(_KullaniciAdi, _Sifre, "AXA");
            //IWebProxy proxy = new WebProxy("178.18.206.24", 20103);
            //hayatService.Proxy = proxy;
            PoliceListesi policeListesi = hayatService.UretimListesi(_TanzimBaslangicTarihi.ToString().Replace(".", "/"), _TanzimBitisTarihi.ToString().Replace(".", "/"));
            List<Police> returnPoliceler = new List<Police>();
            Police policeItem = new Police();
            List<int> aynipolice = new List<int>();
            try
            {
                if (policeListesi != null)
                {
                    if (String.IsNullOrEmpty(policeListesi.Hata) && policeListesi.Police != null)
                    {
                        for (int i = 0; i < policeListesi.Police.ToList().Count; i++)
                        {
                            policeItem = new Police();
                            var police = policeListesi.Police[i];

                            #region Poliçe Genel Bilgiler
                            if (_tvmKodu > 0) policeItem.GenelBilgiler.TVMKodu = _tvmKodu;
                            else policeItem.GenelBilgiler.TVMKodu = 0;
                            policeItem.GenelBilgiler.PoliceNumarasi = police.PoliceNo.ToString();
                            policeItem.GenelBilgiler.YenilemeNo = (int)police.YenilemeNo;
                            policeItem.GenelBilgiler.EkNo = (int)police.EkbelgeNo;
                            policeItem.GenelBilgiler.ZeyilAdi = police.EkbelgeAdi;
                            policeItem.GenelBilgiler.ZeyilKodu = police.EkbelgeKodu.ToString();
                            policeItem.GenelBilgiler.TUMBransAdi = police.Urun.BransAdi;
                            policeItem.GenelBilgiler.BaslangicTarihi = police.BaslamaTarihi;
                            policeItem.GenelBilgiler.BitisTarihi = police.BitisTarihi;
                            policeItem.GenelBilgiler.TanzimTarihi = police.TanzimTarihi;
                            policeItem.GenelBilgiler.NetPrim = police.NetPrimTL;
                            policeItem.GenelBilgiler.BrutPrim = police.BrutPrimTL;
                            policeItem.GenelBilgiler.ToplamVergi = police.VergiTL;
                            policeItem.GenelBilgiler.Komisyon = police.KomisyonTL;
                            policeItem.GenelBilgiler.DovizKur = police.Kur;
                            policeItem.GenelBilgiler.ParaBirimi = police.ParaBirimi;
                            policeItem.GenelBilgiler.Durum = 0;
                            policeItem.GenelBilgiler.TUMBirlikKodu = SigortaSirketiBirlikKodlari.AXASIGORTA;

                            // var PoliceBransEslestir = Util.PoliceBransAdiEslestir(SigortaSirketiBransList, branslar, police.Urun.UrunAdi, police.Urun.UrunKodu.ToString());
                            var neoBransDetay = getNeoOnlineBranskodu(Convert.ToInt32(police.Urun.BransKodu));
                            policeItem.GenelBilgiler.BransAdi = neoBransDetay.BransAdi;
                            policeItem.GenelBilgiler.BransKodu = neoBransDetay.BransKodu;
                            policeItem.GenelBilgiler.TUMUrunAdi = police.Urun.UrunAdi;
                            policeItem.GenelBilgiler.TUMUrunKodu = police.Urun.UrunKodu.ToString();
                            policeItem.GenelBilgiler.TUMBransKodu = police.Urun.BransKodu.ToString();
                            policeItem.GenelBilgiler.TUMBransAdi = police.Urun.BransAdi.ToString();
                            #endregion

                            #region Poliçe Sigortalı
                            if (police.Sigortali != null)
                            {
                                policeItem.GenelBilgiler.PoliceSigortali.AdiUnvan = police.Sigortali.Adi;
                                policeItem.GenelBilgiler.PoliceSigortali.SoyadiUnvan = police.Sigortali.Soyadi;
                                policeItem.GenelBilgiler.PoliceSigortali.Cinsiyet = police.Sigortali.Cinsiyet;
                                policeItem.GenelBilgiler.PoliceSigortali.DogumTarihi = police.Sigortali.DogumTarihi;
                                //policeItem.GenelBilgiler.PoliceSigortali.EMail = police.GrupBilgi.SigortaliEMail;
                                policeItem.GenelBilgiler.PoliceSigortali.Adres = police.Sigortali.Adres;

                                if (!String.IsNullOrEmpty(police.Sigortali.TcKimlikNo))
                                {
                                    policeItem.GenelBilgiler.PoliceSigortali.KimlikNo = police.Sigortali.TcKimlikNo;
                                }
                                else if (!String.IsNullOrEmpty(police.Sigortali.VergiKimlikNo))
                                {
                                    policeItem.GenelBilgiler.PoliceSigortali.VergiKimlikNo = police.Sigortali.VergiKimlikNo;
                                }
                            }

                            #endregion

                            #region Poliçe Sigorta Ettiren
                            if (police.SigortaEttiren != null)
                            {

                                policeItem.GenelBilgiler.PoliceSigortaEttiren.AdiUnvan = police.SigortaEttiren.Adi;
                                policeItem.GenelBilgiler.PoliceSigortaEttiren.SoyadiUnvan = police.SigortaEttiren.Soyadi;
                                policeItem.GenelBilgiler.PoliceSigortaEttiren.Cinsiyet = police.SigortaEttiren.Cinsiyet;
                                policeItem.GenelBilgiler.PoliceSigortaEttiren.DogumTarihi = police.SigortaEttiren.DogumTarihi;
                                policeItem.GenelBilgiler.PoliceSigortaEttiren.Adres = police.SigortaEttiren.Adres;

                                if (!String.IsNullOrEmpty(police.SigortaEttiren.TcKimlikNo))
                                {
                                    policeItem.GenelBilgiler.PoliceSigortaEttiren.KimlikNo = police.SigortaEttiren.TcKimlikNo;
                                }
                                else if (!String.IsNullOrEmpty(police.SigortaEttiren.VergiKimlikNo))
                                {
                                    policeItem.GenelBilgiler.PoliceSigortaEttiren.VergiKimlikNo = police.SigortaEttiren.VergiKimlikNo;
                                }

                            }
                            #endregion

                            #region Poliçe Ödeme Planı
                            PoliceOdemePlani polOdeme = new PoliceOdemePlani();
                            if (police.OdemePlani != null)
                            {

                                for (int j = 0; j < police.OdemePlani.ToList().Count; j++)
                                {
                                    polOdeme = new PoliceOdemePlani();
                                    var policeOdeme = police.OdemePlani[j];
                                    polOdeme.TaksitNo = j + 1; ;
                                    polOdeme.TaksitTutari = policeOdeme.VadeTutari;
                                    polOdeme.VadeTarihi = policeOdeme.VadeTarihi;
                                    // polOdeme.OdemeTipi = policeOdeme.OdemeAraci;
                                    if (policeOdeme.OdemeAraci == "BANKA HAVALESİ")
                                    {
                                        polOdeme.OdemeTipi = OdemeTipleri.Havale;
                                    }
                                    policeItem.GenelBilgiler.PoliceOdemePlanis.Add(polOdeme);
                                }

                            }
                            //Ödeme Şekli
                            if (policeItem.GenelBilgiler.PoliceOdemePlanis != null)
                            {
                                if (policeItem.GenelBilgiler.PoliceOdemePlanis.Count == 0) policeItem.GenelBilgiler.OdemeSekli = 0;
                                if (policeItem.GenelBilgiler.PoliceOdemePlanis.Count == 1) policeItem.GenelBilgiler.OdemeSekli = 1;
                                if (policeItem.GenelBilgiler.PoliceOdemePlanis.Count > 1) policeItem.GenelBilgiler.OdemeSekli = 2;
                            }

                            #endregion
                            if (policeItem.GenelBilgiler.PoliceArac.TescilSeriNo != null)
                            {
                                policeItem.GenelBilgiler.PoliceArac.TescilSeriKod = policeItem.GenelBilgiler.PoliceArac.TescilSeriNo.Substring(0, 2);
                                policeItem.GenelBilgiler.PoliceArac.TescilSeriNo = policeItem.GenelBilgiler.PoliceArac.TescilSeriNo.Substring(2);
                            }

                            returnPoliceler.Add(policeItem);
                        }
                        ayniPoliceAyrim(returnPoliceler);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(policeListesi.Hata))
                        {
                            _IPoliceTransferService.setMessage(policeListesi.Hata);
                            return null;
                        }
                        else
                        {
                            return returnPoliceler;
                        }
                    }
                }
                if (returnPoliceler == null)
                {
                    _IPoliceTransferService.setMessage("Otomatik poliçe transfer edilirken bir sorun oluştu.");
                }
                return returnPoliceler;
            }
            catch (Exception ex)
            {
                _IPoliceTransferService.setMessage("Otomatik poliçe transfer edilirken bir sorun oluştu. Detay" + ex.Message.ToString());
                return null;
                throw;
            }
        }
        List<Police> ayniPoliceAyrim(List<Police> returnPoliceler)
        {
            List<Police> tempPoliceler = new List<Police>();
            foreach (var police in returnPoliceler)
            {
                if (police.GenelBilgiler.BransKodu != 4) continue;
                var checkpolice = returnPoliceler.Where(x => x.GenelBilgiler.PoliceNumarasi == police.GenelBilgiler.PoliceNumarasi && x.GenelBilgiler.EkNo == police.GenelBilgiler.EkNo && x.GenelBilgiler.YenilemeNo == police.GenelBilgiler.YenilemeNo).ToList();
                if (checkpolice.Count > 1)
                {
                    int yeniekno = checkpolice.FirstOrDefault().GenelBilgiler.EkNo.Value;
                     
                    foreach (var item in checkpolice)
                    {
                        item.GenelBilgiler.EkNo = yeniekno;
                        yeniekno++;
                    }
                    //if (tempPoliceler.Where(x => x.GenelBilgiler.PoliceNumarasi == police.GenelBilgiler.PoliceNumarasi && x.GenelBilgiler.EkNo == police.GenelBilgiler.EkNo && x.GenelBilgiler.YenilemeNo == police.GenelBilgiler.YenilemeNo).FirstOrDefault() == null)
                    //{

                    //    var tempdata = checkpolice.FirstOrDefault();
                    //    int yeniekno = tempdata.GenelBilgiler.EkNo.Value;
                    //    foreach (var item in checkpolice)
                    //    {
                    //        item.GenelBilgiler.EkNo = yeniekno;
                    //        tempPoliceler.Add(item);
                    //        yeniekno++;
                    //    }
                    //}
                }
            }
            return returnPoliceler;
        }
        string[] TescilPart(string val)
        {
            string temp = "", temp2 = "";
            for (int i = 0; i < val.Length; i++)
            {
                if (int.TryParse(val[i].ToString(), out int res))
                {
                    temp2 = val.Substring(i);
                    break;
                }
                temp += val[i].ToString();
            }
            temp = temp.Replace(" ", "");
            temp2 = temp2.Replace(" ", "");


            return new string[] { temp, temp2 };
        }
        public class AxaPoliceListModel
        {
            public int PoliceNo { get; set; }
            public int ZeylSiraNo { get; set; }
        }
        public class AxaPoliceDetayListModel
        {

        }
        public NeoOnlineBransResult getNeoOnlineBranskodu(int axaBransKodu)
        {
            var NeoOnlineBranslar = _BransUrunService.getBranslar();
            NeoOnlineBransResult neoBrans = new NeoOnlineBransResult();
            switch (axaBransKodu)
            {
                case 1: neoBrans.BransKodu = BransKodlari.YillikHayat; break;
                case 2: neoBrans.BransKodu = BransKodlari.Saglik; break;
                case 3: neoBrans.BransKodu = BransKodlari.BES; break;
                case 4: neoBrans.BransKodu = BransKodlari.FerdiKaza; break;
                case 5: neoBrans.BransKodu = BransKodlari.Yangin; break;
                case 6: neoBrans.BransKodu = BransKodlari.Nakliyat; break;
                case 7: neoBrans.BransKodu = BransKodlari.Trafik; break;
                case 11: neoBrans.BransKodu = BransKodlari.Saglik; break;
                case 12: neoBrans.BransKodu = BransKodlari.YillikHayat; break;
                case 14: neoBrans.BransKodu = BransKodlari.Saglik; break;
                default: neoBrans.BransKodu = BransKodlari.TANIMSIZ; break;
            }
            if (NeoOnlineBranslar.Count > 0)
            {
                var neoBransDetay = NeoOnlineBranslar.Where(s => s.BransKodu == neoBrans.BransKodu).FirstOrDefault();
                if (neoBransDetay != null)
                {
                    neoBrans.BransAdi = neoBransDetay.BransAdi;
                }
            }
            return neoBrans;
        }
        public class NeoOnlineBransResult
        {
            public int BransKodu { get; set; }
            public string BransAdi { get; set; }
        }
    }
}
