using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Database.Models;
using System.Xml;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.Readers;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.Message;
using System.Net;
using Neosinerji.BABOnlineTP.Database.UnitOfWork;
using Neosinerji.BABOnlineTP.Database.Common;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.Factory;
using Neosinerji.BABOnlineTP.Business.PoliceTransfer.OtomatikPoliceTransferler;
using Neosinerji.BABOnlineTP.Business.dogapolicetransfer;

namespace Neosinerji.BABOnlineTP.Business.PoliceTransfer
{
    public class PoliceTransferService : IPoliceTransferService
    {
        IBransUrunService _BransUrunService;
        IPoliceTransferStorage _IPoliceTransferStorage;
        IAktifKullaniciService _AktifKullanici;
        IPoliceContext _IPoliceContext;
        public string _message;
        public bool _TahsilatMi;

        public PoliceTransferService(IBransUrunService bransUrunService, IPoliceTransferStorage policeTransferStorage, IAktifKullaniciService aktifKullaniciService, IPoliceContext policeContext)
        {
            _BransUrunService = bransUrunService;
            _IPoliceTransferStorage = policeTransferStorage;
            _AktifKullanici = aktifKullaniciService;
            _IPoliceContext = policeContext;
        }

        //Dosyadan Police Transfer 
        public List<Police> getPoliceler(string sigortaSirketiKodu, string path, int tvmKodu)
        {
            List<Police> policeler = new List<Police>();

            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
            SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList(sigortaSirketiKodu);

            List<Bran> branslar = new List<Bran>();
            branslar = _BransUrunService.getBranslar();

            IPoliceTransferReader reader = PoliceTransferReaderFactory.createReader(sigortaSirketiKodu, path, tvmKodu, SigortaSirketiBransList, branslar);
            if (reader == null)
            {
                _message = "reader factory error";
                return null;
            }

            policeler = reader.getPoliceler();
            _message = reader.getMessage();
            _TahsilatMi = reader.getTahsilatMi();
            return policeler;
        }

        public GencListeler gencPoliceler(string path)
        {
            GencListeler gencPoliceler = new GencListeler();
            //GencExcelReader read = new GencExcelReader(path);

            List<Police> policeler = new List<Police>();

            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
            SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList("054");

            List<Bran> branslar = new List<Bran>();
            branslar = _BransUrunService.getBranslar();

            IGencPoliceTransferReader reader = PoliceTransferReaderFactory.createReaderGenc("000", path, _AktifKullanici.TVMKodu, SigortaSirketiBransList, branslar);
            if (reader == null)
            {
                _message = "reader factory error";
                return null;
            }
            gencPoliceler = reader.getPoliceler();

            return gencPoliceler;
        }

        public string getMessage()
        {
            return _message;
        }

        public void setMessage(string mesaj)
        {
            this._message = mesaj;
        }
        public bool getTahsilatMi()
        {
            return _TahsilatMi;
        }

        //Otomatik Police Transfer 
        public List<Police> getAutoPoliceler(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi, string TahsilatDosyaYolu = "", string partajNo = "")
        {
            List<Police> policeler = new List<Police>();
            if (sirketKodu == SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA)
            {
                IMAPFREOtomatikPoliceTransferReader reader = AutoPoliceTransferReaderFactory.createMapfreReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                policeler = reader.GetMAPFREAutoPoliceTransfer();

            }
            if (sirketKodu == SigortaSirketiBirlikKodlari.MAPFREDASK)
            {
                IMAPFREDASKOtomtikPoliceTranfer reader = AutoPoliceTransferReaderFactory.createMapfreDaskReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                policeler = reader.GetMAPFREDASKAutoPoliceTransfer();

            }
            else if (sirketKodu == SigortaSirketiBirlikKodlari.HDISIGORTA)
            {
                IHDIOtomatikPoliceTransferReader reader = AutoPoliceTransferReaderFactory.createHDIReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                policeler = reader.GetHDIAutoPoliceTransfer();
            }
            else if (sirketKodu == SigortaSirketiBirlikKodlari.AXASIGORTA)
            {
                IAXAOtomatikPoliceTransfer reader = AutoPoliceTransferReaderFactory.createAXAReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                policeler = reader.GetAXAAutoPoliceTransfer();
            }

            else if (sirketKodu == SigortaSirketiBirlikKodlari.ETHICA)
            {
                IETHICAOtomatikPoliceTransfer reader = AutoPoliceTransferReaderFactory.createETHICAReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                policeler = reader.GetETHICAAutoPoliceTransfer();
            }

            else if (sirketKodu == SigortaSirketiBirlikKodlari.SSDOGASİGORTAKOOPERATİF)
            {
                IDOGAOtomatikPoliceTransfer reader = AutoPoliceTransferReaderFactory.createDOGAReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                policeler = reader.GetDOGAAutoPoliceTransfer();
            }
            else if (sirketKodu == SigortaSirketiBirlikKodlari.RAYSIGORTA)
            {
                IRaySigortaOtomatikPoliceTransfer reader = AutoPoliceTransferReaderFactory.createRAYReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi, TahsilatDosyaYolu, partajNo);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                policeler = reader.GetRAYHayatAutoPoliceTransfer();
            }
            return policeler;
        }

        public List<Police> getAxaPoliceler(string sigortaSirketiKodu, XmlNode policeXml, int tvmKodu)
        {
            List<Police> policeler = new List<Police>();

            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
            SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList(sigortaSirketiKodu);

            List<Bran> branslar = new List<Bran>();
            branslar = _BransUrunService.getBranslar();

            // IPoliceTransferReader reader = PoliceTransferReaderFactory.createReader(sigortaSirketiKodu, path, tvmKodu, SigortaSirketiBransList, branslar);
            AxaXmlReader reader = new AxaXmlReader(policeXml, tvmKodu, SigortaSirketiBransList, branslar);
            if (reader == null)
            {
                _message = "reader factory error";
                return null;
            }

            policeler = reader.getPoliceler();
            _message = reader.getMessage();
            _TahsilatMi = reader.getTahsilatMi();
            return policeler;
        }


        public List<Police> getDogaPoliceler(string sigortaSirketiKodu, GeriyePoliceTransferCevap policeListesi, int tvmKodu)
        {
            List<Police> policeler = new List<Police>();
            string[] tempPath = sigortaSirketiKodu.Split('#');
            if (tempPath.Length > 1)
            {
                sigortaSirketiKodu = sigortaSirketiKodu.Substring(0, sigortaSirketiKodu.IndexOf("#"));
            }
            List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
            SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList(sigortaSirketiKodu);

            List<Bran> branslar = new List<Bran>();
            branslar = _BransUrunService.getBranslar();

            // IPoliceTransferReader reader = PoliceTransferReaderFactory.createReader(sigortaSirketiKodu, path, tvmKodu, SigortaSirketiBransList, branslar);
            DogaXmlReader reader = new DogaXmlReader(policeListesi.ToString() + (tempPath.Length > 1 ? "#" + tempPath[1] : ""), tvmKodu, SigortaSirketiBransList, branslar);
            if (reader == null)
            {
                _message = "reader factory error";
                return null;
            }

            policeler = reader.getPoliceler();
            _message = reader.getMessage();
            _TahsilatMi = reader.getTahsilatMi();
            return policeler;
        }
        //public List<Police> getAveonGlobalPoliceler(string sigortaSirketiKodu, GeriyePoliceTransferCevap policeListesi, int tvmKodu)
        //{
        //    List<Police> policeler = new List<Police>();
        //    string[] tempPath = sigortaSirketiKodu.Split('#');
        //    if (tempPath.Length > 1)
        //    {
        //        sigortaSirketiKodu = sigortaSirketiKodu.Substring(0, sigortaSirketiKodu.IndexOf("#"));
        //    }
        //    List<BransUrun> SigortaSirketiBransList = new List<BransUrun>();
        //    SigortaSirketiBransList = _BransUrunService.getSigortaSirketiBransList(sigortaSirketiKodu);

        //    List<Bran> branslar = new List<Bran>();
        //    branslar = _BransUrunService.getBranslar();

        //    // IPoliceTransferReader reader = PoliceTransferReaderFactory.createReader(sigortaSirketiKodu, path, tvmKodu, SigortaSirketiBransList, branslar);
        //    AveonGlobalXmlReader reader = new AveonGlobalXmlReader(policeListesi.ToString() + (tempPath.Length > 1 ? "#" + tempPath[1] : ""), tvmKodu, SigortaSirketiBransList, branslar);
        //    if (reader == null)
        //    {
        //        _message = "reader factory error";
        //        return null;
        //    }

        //    policeler = reader.getPoliceler();
        //    _message = reader.getMessage();
        //    _TahsilatMi = reader.getTahsilatMi();
        //    return policeler;
        //}
        public List<AutoPoliceTransferProcedureModel> AutoPoliceTransferGetir(DateTime TanzimBaslangicTarih, DateTime TanzimBitisTarih, string SigortaSirketleriListe, int tvmKodu)
        {
            return _IPoliceContext.AutoPoliceTransferGetir(TanzimBaslangicTarih, TanzimBitisTarih, SigortaSirketleriListe, tvmKodu);
        }

        public AutoPoliceTransfer AutoPoliceTransferGetir(DateTime TanzimBaslangicTarih, string SigortaSirketKodu, int tvmKodu)
        {
            return _IPoliceContext.AutoPoliceTransferRepository.All().Where(s => s.TanzimBaslangicTarihi >= TanzimBaslangicTarih && s.TanzimBaslangicTarihi <= TanzimBaslangicTarih && s.SirketKodu == SigortaSirketKodu && s.TvmKodu == tvmKodu).FirstOrDefault();
        }

        //Otomatik Police Transfer Tahsilat Kapatma
        public string getAutoTahsilatPoliceKapatma(int tvmKodu, string sirketKodu, string serviceURL, string KullaniciAdi, string Sifre, DateTime TanzimBaslangicTarihi, DateTime TanzimBitisTarihi)
        {
            string resultMessage = "";
            if (sirketKodu == SigortaSirketiBirlikKodlari.MAPFREGENELSIGORTA)
            {
                IMAPFREOtomatikTahsilatPoliceTransferReader reader = AutoPoliceTransferReaderFactory.createTahsilatMapfreReader(tvmKodu, sirketKodu, serviceURL, KullaniciAdi, Sifre, TanzimBaslangicTarihi, TanzimBitisTarihi);
                if (reader == null)
                {
                    _message = "reader factory error";
                    return null;
                }

                resultMessage = reader.GetMAPFREAutoTahsilatPoliceTransfer();
            }

            return resultMessage;
        }

        public PoliceModel getOtoOnayPoliceler(string policeNumarasi, string sigortaSirketNumarasi)
        {
            PoliceModel result = new PoliceModel();
            result.policeler = _IPoliceContext.PoliceGenelRepository.All().Where(s => s.TVMKodu == _AktifKullanici.TVMKodu && s.PoliceNumarasi == policeNumarasi && s.TUMBirlikKodu == sigortaSirketNumarasi).ToList();
            if (result.policeler.Count == 0)
            {
                result.BilgiMesaji = policeNumarasi + " nolu Poliçe sistemde bulunamadı.";
            }
            else
            {
                result.policeler = result.policeler.Where(s => s.TVMKodu == _AktifKullanici.TVMKodu && s.PoliceNumarasi == policeNumarasi && s.TUMBirlikKodu == sigortaSirketNumarasi && s.TaliAcenteKodu == null).ToList();
                if (result.policeler.Count == 0)
                {
                    result.BilgiMesaji = policeNumarasi + " nolu Poliçe daha önceden onaylanmış.";
                }
            }
            return result;
        }

        public PoliceTransferReaderKullanicilari GetPoliceReaderKullanicilari(string readerKullanciKodu)
        {
            PoliceTransferReaderKullanicilari readerKullanicisi = new PoliceTransferReaderKullanicilari();
            readerKullanicisi = _IPoliceContext.PoliceTransferReaderKullanicilariRepository.Find(s => s.ReaderKodu == readerKullanciKodu);

            return readerKullanicisi;
        }
    }
}
