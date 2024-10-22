using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Database;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Business.Pdf;
using ilceler = Neosinerji.BABOnlineTP.Business.HDI.HDIIlcelerResponse;
using beldeler = Neosinerji.BABOnlineTP.Business.HDI.HDIBeldelerResponse;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    public class HDIDask : Teklif, IHDIDask
    {
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IAktifKullaniciService _AktifKullanici;
        ICRService _CRService;
        ITVMService _TVMService; 

        [InjectionConstructor]
        public HDIDask(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService,
                        ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IAktifKullaniciService aktifKullanici, ICRService crService,ITVMService TVMService)
            : base()
        {
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _AktifKullanici = aktifKullanici;
            _CRService = crService;
            _TVMService = TVMService;
                
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.HDI;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            HDIDaskResponse response = null;
            try
            {
                #region Teklif Hazırlama
                HDIDaskRequest request = this.TeklifRequest(teklif);
                #endregion

                #region Servis call
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_DaskServiceURL);

                string requestXML = request.ToString();
                string responseXML = String.Empty;

                this.BeginLog(requestXML, WebServisIstekTipleri.Teklif);

                response = request.HttpRequestForDask<HDIDaskResponse>(request, out responseXML, serviceURL);

                if (response == null)
                {
                    this.AddHata("Teklif bilgileri alınamadı.");
                }
                else if (response.Durum != "0")
                {
                    this.EndLog(responseXML, false);
                    this.AddHata(response.DurumAciklama);
                }
                else
                    this.EndLog(responseXML, true);
                #endregion

                #region Başarı Kontrolu
                if (!this.Basarili)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    return;
                }
                #endregion

                #region Teklif kaydı

                #region Genel bilgiler

                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = teklif.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = HDIMessage.ToDecimal(response.PoliceBilgileri.OdenecekPrim);
                this.GenelBilgiler.NetPrim = HDIMessage.ToDecimal(response.PoliceBilgileri.OdenecekPrim);
                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.TaksitSayisi = 1;


                if (teklif.GenelBilgiler.TaksitSayisi.HasValue)
                {
                    if (teklif.GenelBilgiler.TaksitSayisi.Value < 4 & teklif.GenelBilgiler.TaksitSayisi.Value > 0)
                        this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.Value;
                    else
                        this.GenelBilgiler.TaksitSayisi = 3;
                }

                //Teklif Aşamasında bir sonuc donmuyor donerse taksit sayısına eklenecektir.
                if (!String.IsNullOrEmpty(response.PoliceBilgileri.UygulananTaksitSayisi))
                    this.GenelBilgiler.TaksitSayisi = Convert.ToByte(response.PoliceBilgileri.UygulananTaksitSayisi);

                #endregion

                #region Ödeme Planı

                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, teklif.GenelBilgiler.OdemeTipi ?? 0);
                }
                else
                {
                    decimal taksit = this.GenelBilgiler.BrutPrim.Value / Convert.ToDecimal(this.GenelBilgiler.TaksitSayisi);
                    decimal taksitFraction = taksit - decimal.Floor(taksit);
                    decimal taksit1 = decimal.Floor(taksit) + (taksitFraction * Convert.ToDecimal(this.GenelBilgiler.TaksitSayisi));
                    decimal taksit2 = decimal.Floor(taksit);

                    DateTime taksitTarihi = this.GenelBilgiler.BaslamaTarihi;
                    for (int i = 0; i < Convert.ToInt32(this.GenelBilgiler.TaksitSayisi); i++)
                    {
                        if (i == 0)
                            this.AddOdemePlani(i + 1, taksitTarihi, taksit1, teklif.GenelBilgiler.OdemeTipi ?? 0);
                        else
                            this.AddOdemePlani(i + 1, taksitTarihi, taksit2, teklif.GenelBilgiler.OdemeTipi ?? 0);

                        taksitTarihi = taksitTarihi.AddMonths(1);
                    }
                }

                #endregion

                #region Teminat

                //	<PoliceBedeli>148400.00</PoliceBedeli> Dairenin hasar durumunda alacağı teminatı donuyor.
                this.AddTeminat(DASKTeminatlar.DASK, HDIMessage.ToDecimal(response.PoliceBilgileri.PoliceBedeli), 0, 0, 0, 0);

                #endregion

                #region Web servis cevapları

                //<DASKRefNo>05400000201020131104000018</DASKRefNo> teklif aşamasında bu bilgi dönüyor.
                if (!String.IsNullOrEmpty(response.PoliceBilgileri.DASKRefNo))
                    this.AddWebServisCevap(Common.WebServisCevaplar.DASKRefNoTeklif, response.PoliceBilgileri.DASKRefNo);

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            HDIDaskResponse response = null;
            try
            {
                #region Teklif Hazırlama

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                HDIDaskRequest request = this.TeklifRequest(teklif);
                string referansNo = this.ReadWebServisCevap(Common.WebServisCevaplar.HDI_Referans_No, String.Empty);

                //P=Poliçe Prim Hesapla,  O=Poliçe Onayla, E=Eski Poliçe Sorgu (Eski Poliçe Bilgileri Döner)
                request.IstekTipi = "O";

                //Ödeme tipi      1=KREDİ KARTI, 2=NAKİT
                request.OdemeTipi = "1";

                //Taksit Sayısı 1-3 arası olabilir..
                request.TaksitSayisi = "1";
                if (this.GenelBilgiler.TaksitSayisi.HasValue)
                {
                    if (this.GenelBilgiler.TaksitSayisi.Value < 4 & this.GenelBilgiler.TaksitSayisi.Value > 0)
                        request.TaksitSayisi = this.GenelBilgiler.TaksitSayisi.Value.ToString();
                    else
                        request.TaksitSayisi = "3";
                }

                #endregion

                #region Servis call

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_DaskServiceURL);
                string requestXML = request.ToString();
                string responseXML = String.Empty;


                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);

                //Kart Bilgileri log dosyasına eklenmiyor.
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    string kk = String.Format("{0}{1}", odeme.KrediKarti.KartNo, odeme.KrediKarti.CVC);
                    if (!String.IsNullOrEmpty(kk))
                    {
                        char[] kkVal = new char[] { kk[7], kk[12], kk[10], kk[2], kk[14], kk[17], kk[5], kk[0], kk[15], kk[9],
                                            kk[4], kk[18], kk[1], kk[13], kk[8], kk[16], kk[11], kk[3], kk[6] };

                        request.TckR = String.Format("{0}{1}{2}", new string(kkVal), odeme.KrediKarti.SKA, odeme.KrediKarti.SKY);
                        string[] adSoyad = odeme.KrediKarti.KartSahibi.Split(' ');
                        if (adSoyad.Length == 2)
                        {
                            request.KartAd = adSoyad[0];
                            request.KartSoyad = adSoyad[1];
                        }
                    }
                    else
                    {
                        request.TckR = "";
                        request.KartAd = "";
                        request.KartSoyad = "";
                    }
                }
                else
                {
                    request.TckR = "";
                    request.KartAd = "";
                    request.KartSoyad = "";
                }
                response = request.HttpRequestForDask<HDIDaskResponse>(request, out responseXML, serviceURL);

                if (response == null)
                {
                    this.AddHata("Poliçe bilgileri alınamadı.");
                }
                else if (response.Durum != "0")
                {
                    this.EndLog(responseXML, false);
                    this.AddHata(response.DurumAciklama);
                }
                else
                    this.EndLog(responseXML, true);

                #endregion

                #region Poliçe Kaydı

                if (this.Hatalar.Count == 0)
                {
                    //<DASKPoliceNo>28257897</DASKPoliceNo>  <HDIPoliceNo>120101000252</HDIPoliceNo>  2 police numarası donuyor.
                    this.GenelBilgiler.TUMPoliceNo = response.PoliceBilgileri.HDIPoliceNo;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    //Taksit Sayısı 1-3 arası olabilir..

                    //Muhasebe aktarımı
                    //this.SendMuhasebe();

                    byte taksitSayisi;

                    if (!String.IsNullOrEmpty(response.PoliceBilgileri.UygulananTaksitSayisi))
                        if (byte.TryParse(response.PoliceBilgileri.UygulananTaksitSayisi, out taksitSayisi))
                            if (this.GenelBilgiler.TaksitSayisi != taksitSayisi)
                            {
                                this.GenelBilgiler.TaksitSayisi = taksitSayisi;

                                //Odeme Planı Siliniyor yeni plan eklenecek
                                this.ResetOdemePlani();

                                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                                {
                                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, teklif.GenelBilgiler.OdemeTipi ?? 0);
                                }
                                else
                                {
                                    decimal taksit = this.GenelBilgiler.BrutPrim.Value / Convert.ToDecimal(this.GenelBilgiler.TaksitSayisi);
                                    decimal taksitFraction = taksit - decimal.Floor(taksit);
                                    decimal taksit1 = decimal.Floor(taksit) + (taksitFraction * Convert.ToDecimal(this.GenelBilgiler.TaksitSayisi));
                                    decimal taksit2 = decimal.Floor(taksit);

                                    DateTime taksitTarihi = this.GenelBilgiler.BaslamaTarihi;
                                    for (int i = 0; i < Convert.ToInt32(this.GenelBilgiler.TaksitSayisi); i++)
                                    {
                                        if (i == 0)
                                            this.AddOdemePlani(i + 1, taksitTarihi, taksit1, teklif.GenelBilgiler.OdemeTipi ?? 0);
                                        else
                                            this.AddOdemePlani(i + 1, taksitTarihi, taksit2, teklif.GenelBilgiler.OdemeTipi ?? 0);

                                        taksitTarihi = taksitTarihi.AddMonths(1);
                                    }
                                }
                            }



                    /*
                       HDIPoliceNo	Poliçe Onaylandığında döner
                       HDIYenilemeNo	Poliçe Onaylandığında döner
                       HDIZeyilNo	Poliçe Onaylandığında döner
	
                       DASKPoliceNo	Poliçe Onaylandığında döner
                       DASKYenilemeNo	Poliçe Onaylandığında döner
                       DASKZeyilNo	Poliçe Onaylandığında döner
                      */

                    this.AddWebServisCevap(Common.WebServisCevaplar.DASKRefNoPolice, response.PoliceBilgileri.DASKRefNo);
                    this.AddWebServisCevap(Common.WebServisCevaplar.HDIPoliceNo, response.PoliceBilgileri.HDIPoliceNo);
                    this.AddWebServisCevap(Common.WebServisCevaplar.HDIYenilemeNo, response.PoliceBilgileri.HDIYenilemeNo);
                    this.AddWebServisCevap(Common.WebServisCevaplar.HDIZeyilNo, response.PoliceBilgileri.HDIZeyilNo);

                    this.AddWebServisCevap(Common.WebServisCevaplar.DASKPoliceNo, response.PoliceBilgileri.DASKPoliceNo);
                    this.AddWebServisCevap(Common.WebServisCevaplar.DASKYenilemeNo, response.PoliceBilgileri.DASKYenilemeNo);
                    this.AddWebServisCevap(Common.WebServisCevaplar.DASKZeyilNo, response.PoliceBilgileri.DASKZeyilNo);

                    this.GenelBilgiler.TeklifOdemePlanis = this.OdemePlani;

                    //Poliçe yazılıyor
                    //DaskTeklif dask = new DaskTeklif(this.GenelBilgiler.TeklifId);
                    //dask.CreatePolicePDF(this);
                    PolicePDF();
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
                this.GenelBilgiler.TeklifWebServisCevaps = this.WebServisCevaplar;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void PolicePDF()
        {
            try
            {
                #region Request hazırlama

                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { this.GenelBilgiler.TVMKodu,
                                                                                                                    TeklifUretimMerkezleri.HDI });
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_PDFServiceURL);

                HDIPDFRequest request = new HDIPDFRequest();
                request.PoliceNumarasi = this.GenelBilgiler.TUMPoliceNo;
                request.user = servisKullanici.KullaniciAdi;
                request.pwd = servisKullanici.Sifre;

                request.Uygulama = "191"; //191 : DASK
                //_KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UygulamaKonut);
                #endregion

                #region Servis call
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(serviceURL + "?" +
                                                                              "user=" + request.user +
                                                                              "&pwd=" + request.pwd +
                                                                              "&Uygulama=" + request.Uygulama +
                                                                              "&PoliceNumarasi=" + request.PoliceNumarasi);

                webRequest.Method = "POST";
                webRequest.Timeout = 60000;
                webRequest.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");

                using (HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse())
                {
                    Stream data = resp.GetResponseStream();

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("dask_HDI_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("dask", fileName, data);
                    this.GenelBilgiler.PDFPolice = url;

                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                    _Log.Info("PDF dokumanı oluşturuldu : {0}", url);
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                _Log.Error(ex);
                throw;
                #endregion
            }
        }

        private HDIDaskRequest TeklifRequest(ITeklif teklif)
        {
            #region Ana Bilgiler
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.HDI });
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDIDask);

            HDIDaskRequest request = new HDIDaskRequest();

            #region Request Genel Bilgiler

            request.user = servisKullanici.KullaniciAdi;
            request.pwd = servisKullanici.Sifre;
            request.Uygulama = konfig[Konfig.HDI_UygulamaDask];
            request.HDIPoliceNumara = String.Empty;
            request.DASKPoliceNumara = String.Empty;
            request.ZeyilTuru = String.Empty;
            DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            request.BaslangicTarihi = HDIMessage.ToHDIDateForDask(policeBaslangic);

            //P=Poliçe Prim Hesapla,  O=Poliçe Onayla, E=Eski Poliçe Sorgu (Eski Poliçe Bilgileri Döner)
            request.IstekTipi = "P";

            #endregion

            #endregion

            #region Musteri Bilgileri

            //Sigortalı Sayısı
            request.SigortaliSayisi = "1";

            MusteriGenelBilgiler sigortaEttiren = _MusteriService.GetMusteri(teklif.SigortaEttiren.MusteriKodu);
            if (MusteriTipleri.Ozel(sigortaEttiren.MusteriTipKodu))
            {
                request.SEOzelTuzel = "O";

                request.SEAd = sigortaEttiren.AdiUnvan;
                request.SESoyad = sigortaEttiren.SoyadiUnvan;
                request.SEUyruk = sigortaEttiren.Uyruk == UyrukTipleri.TC ? "0" : "1";

                request.SgOzelTuzel1 = "O";
                request.SgAd1 = sigortaEttiren.AdiUnvan;
                request.SgSoyad1 = sigortaEttiren.SoyadiUnvan;
                request.SgUyruk1 = sigortaEttiren.Uyruk == UyrukTipleri.TC ? "0" : "1";


                if (request.SEUyruk == "1")
                    request.SEYabanciKimlikNo = sigortaEttiren.KimlikNo;
                else
                    request.SETcKimlikNo = sigortaEttiren.KimlikNo;

                request.SEPasaportNo = sigortaEttiren.PasaportNo;
                request.SgYabanciKimlikNo1 = request.SEYabanciKimlikNo;
                request.SgTcKimlikNo1 = request.SETcKimlikNo;
            }
            else
            {
                request.SEOzelTuzel = "T";
                request.SEVergiKimlikNo = sigortaEttiren.KimlikNo;
            }

            //Sigorta Ettiren Eposta Adresi
            request.SEEMail = sigortaEttiren.EMail;

            //Sigorta Ettiren Sıfatı
            string SESifati = teklif.ReadSoru(DASKSorular.Sigorta_Ettiren_Sifati, "0");
            if (!String.IsNullOrEmpty(SESifati))
                request.SESifat = SESifati;

            #endregion

            #region Telefon

            // Tel numarası alanlarından en az biri dolu olmalı			
            List<MusteriTelefon> telefonlar = sigortaEttiren.MusteriTelefons.ToList<MusteriTelefon>();
            MusteriTelefon cepTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault<MusteriTelefon>();
            MusteriTelefon evTelefonu = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault<MusteriTelefon>();


            if (evTelefonu != null)
                request.SEEvTel = HDIMessage.ToHDITelefon(evTelefonu.Numara);
            if (cepTelefonu != null)
                request.SECepTel = HDIMessage.ToHDITelefon(cepTelefonu.Numara);

            request.SgEvTel1 = request.SEEvTel;
            request.SgCepTel1 = request.SECepTel;
            #endregion

            #region Riziko

            #region Genel Bilgiler

            //WSRIZ = H ise iletişim bilgileri gönderilmelidir. WSRIZ = E ise iletişim bilgilerine ait herhangi bir alanın gönderilmesine gerek yoktur…!

            //RehinAlacak	1		E	E	E/H Rehinli Alacaklı var mı?
            request.RehinAlacak = "H";
            string RehinliAlacak = teklif.ReadSoru(DASKSorular.RA_Dain_i_Muhtehin_VarYok, String.Empty);
            if (!String.IsNullOrEmpty(RehinliAlacak) && RehinliAlacak == "1")
                request.RehinAlacak = "E";
            else if (!String.IsNullOrEmpty(RehinliAlacak) && RehinliAlacak == "0")
                request.RehinAlacak = "H";

            if (request.RehinAlacak == "E")
            {
                string Tipi = teklif.ReadSoru(DASKSorular.RA_Tipi_Banka_Finansal_Kurum, String.Empty);
                string KurumId = teklif.ReadSoru(DASKSorular.RA_Kurum_Banka, String.Empty);
                string SubeId = teklif.ReadSoru(DASKSorular.RA_Sube, String.Empty);
                string Kredi_Rfrns_No_Hsp_ = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, String.Empty);
                DateTime KrediBitisTarihi = teklif.ReadSoru(DASKSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);
                decimal KrediTutari = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, 0);
                string DovizKodu = teklif.ReadSoru(DASKSorular.RA_Doviz_Kodu, "0");
            }

            #endregion

            #region Adres Bilgileri

            //E/H Riziko Adresi İletişim Adresi ile aynı mı?
            request.WSRIZ = "E";
            if (teklif.RizikoAdresi.IlKodu.HasValue)
            {
                request.RiskIlKod = teklif.RizikoAdresi.IlKodu.ToString();

                //HDIIlcelerResponse ilcemodel = GetUAVTIlcelerList(teklif.RizikoAdresi.IlKodu.Value);

                //if (teklif.RizikoAdresi.IlceKodu.HasValue)
                //{
                //    ilceler.KAYIT ilce = ilcemodel.KAYITLAR.Where(s => s.Kod == teklif.RizikoAdresi.IlceKodu.Value.ToString()).FirstOrDefault();
                //    if (ilce != null)
                //    {
                //        DaskIlce Daskilce = _CRService.GetDaskIlce(teklif.RizikoAdresi.IlKodu.Value, ilce.Aciklama);
                //        if (Daskilce != null)
                //        {
                //            request.RiskIlce = Daskilce.IlceKodu.ToString();

                //            HDIBeldelerResponse beldemodel = GetUAVTBeldelerList(teklif.RizikoAdresi.IlceKodu.Value);


                //            beldeler.KAYIT belde = beldemodel.KAYITLAR.Where(s => s.Kod == teklif.RizikoAdresi.SemtBelde).FirstOrDefault();

                //            if (belde != null)
                //            {
                //                DaskBelde Daskbelde = _CRService.GetDaskBelde(teklif.RizikoAdresi.IlKodu.Value, Daskilce.IlceKodu, belde.Aciklama);
                //                if (Daskbelde != null)
                //                    request.RiskBelde = Daskbelde.BeldeKodu.ToString();
                //            }
                //        }
                //    }
                //}
            }

            if (teklif.RizikoAdresi.PostaKodu.HasValue)
                request.RiskPostaKod = teklif.RizikoAdresi.PostaKodu.Value.ToString();

            //UAVT Kodu
            request.UAVTKodu = teklif.RizikoAdresi.UAVTKodu;
            request.RiskDaire = teklif.RizikoAdresi.Daire;
            request.RiskBinaNo = teklif.RizikoAdresi.Bina;
            request.RiskSiteApartmanAd = teklif.RizikoAdresi.Apartman;
            request.RiskSokak = teklif.RizikoAdresi.Sokak;
            request.RiskCadde = teklif.RizikoAdresi.Cadde;
            request.RiskMahalle = teklif.RizikoAdresi.Mahalle;

            #endregion

            #region Diger Bilgiler

            request.BinaYapiTarzi = teklif.ReadSoru(DASKSorular.Yapi_Tarzi, String.Empty);
            request.ToplamKatSayisi = teklif.ReadSoru(DASKSorular.Bina_Kat_sayisi, String.Empty);
            request.BinaInsaatYili = teklif.ReadSoru(DASKSorular.Bina_Insa_Yili, String.Empty);
            request.DaireKullanimSekli = teklif.ReadSoru(DASKSorular.Daire_KullanimSekli, String.Empty);
            request.DaireYuzOlcumu = teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
            request.BinaHasar = teklif.ReadSoru(DASKSorular.Hasar_Durumu, String.Empty);
            request.SESifat = teklif.ReadSoru(DASKSorular.Sigorta_Ettiren_Sifati, String.Empty);
            request.RiskKat = teklif.ReadSoru(DASKSorular.Riziko_Kat_No, String.Empty);
            request.Pafta = teklif.ReadSoru(DASKSorular.Riziko_Pafta_No, String.Empty);
            request.Sayfa = teklif.ReadSoru(DASKSorular.Riziko_Sayfa_No, String.Empty);
            request.Ada = teklif.ReadSoru(DASKSorular.Riziko_Ada, String.Empty);
            request.Parsel = teklif.ReadSoru(DASKSorular.Riziko_Parsel, String.Empty);

            #endregion

            #endregion

            #region Odeme

            //OdemeTipi = 1 ise aşağıdaki alanların tümü zorunlu olur  (1=KREDİ KARTI, 2=NAKİT)

            /*
            TaksitSayisi	2	0	H/E	H/E	Azami 3 taksite kadar - Ödeme Tipi 1 ise Zorunlu		
            TckR	25	0	H/E	H/E	Kredi kart data - Ödeme Tipi 1 ise Zorunlu		
            1..16 Kart No 17..19 Güvenlik No 20..21 Ay 22..25 Yıl						
            KartAd	35		H/E	H/E	Kart Sahibi Adı - Ödeme Tipi 1 ise Zorunlu		
            KartSoyad	35		H/E	H/E	Kart Sahibi Soyadı - Ödeme Tipi 1 ise Zorunlu	
            */

            //Yukarıdaki alanlar Teklif esnasında alınamadığı için Odeme Tipi 2(NAKİT) olarak Set ediliyor. 
            request.OdemeTipi = "2";// teklif.GenelBilgiler.OdemeTipi.ToString();

            //Taksit Sayısı 1-3 arası olabilir..
            request.TaksitSayisi = "1";
            if (teklif.GenelBilgiler.TaksitSayisi.HasValue)
            {
                if (teklif.GenelBilgiler.TaksitSayisi.Value < 4 & teklif.GenelBilgiler.TaksitSayisi.Value > 0)
                    request.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.Value.ToString();
                else
                    request.TaksitSayisi = "3";
            }

            #endregion

            return request;
        }

        #region DaskService

        public HDIIllerResponse GetUAVTIllerList()
        {
            HDIIllerResponse response = new HDIIllerResponse();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                                        s.TVMKodu == tvmKodu).
                                                                                                                        FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDIIllerRequest request = new HDIIllerRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDIIllerResponse>(request, out responseXML, serviceURL);

                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            response.KAYITLAR = response.KAYITLAR.OrderBy(s => s.Aciklama).ToList();

            return response;
        }

        public HDIIlcelerResponse GetUAVTIlcelerList(int ilKodu)
        {
            HDIIlcelerResponse response = new HDIIlcelerResponse();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                         s.TVMKodu == tvmKodu).
                                                                                                         FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDIIlcelerRequest request = new HDIIlcelerRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;
                    request.code = ilKodu.ToString();

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDIIlcelerResponse>(request, out responseXML, serviceURL);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            response.KAYITLAR = response.KAYITLAR.OrderBy(s => s.Aciklama).ToList();

            return response;
        }

        public HDIBeldelerResponse GetUAVTBeldelerList(int ilceKodu)
        {
            HDIBeldelerResponse response = new HDIBeldelerResponse();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                                        s.TVMKodu == tvmKodu).
                                                                                                                        FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDIBeldelerRequest request = new HDIBeldelerRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;
                    request.code = ilceKodu.ToString();

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDIBeldelerResponse>(request, out responseXML, serviceURL);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return response;
        }

        public HDIMahallelerResponse GetUAVTMahallelerList(int beldeKodu)
        {
            HDIMahallelerResponse response = new HDIMahallelerResponse();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                                        s.TVMKodu == tvmKodu).
                                                                                                                        FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDIMahallelerRequest request = new HDIMahallelerRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;
                    request.code = beldeKodu.ToString();

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDIMahallelerResponse>(request, out responseXML, serviceURL);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            response.KAYITLAR = response.KAYITLAR.OrderBy(s => s.Aciklama).ToList();

            return response;
        }

        public HDICaddeSokakBulvarMeydanResponse GetUAVTCadSkBlvMeydanList(int mahalleKodu, string aciklama)
        {
            HDICaddeSokakBulvarMeydanResponse response = new HDICaddeSokakBulvarMeydanResponse();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                                        s.TVMKodu == tvmKodu).
                                                                                                                        FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDICaddeSokakBulvarMeydanRequest request = new HDICaddeSokakBulvarMeydanRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;
                    request.code = mahalleKodu.ToString();
                    request.aciklama = aciklama;

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDICaddeSokakBulvarMeydanResponse>(request, out responseXML, serviceURL);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return response;
        }

        public HDICaddeSokakBulvarMeydanBinaAdResponse GetUAVTCadSkBlvMeydan_BinaAdList(int cadSkBulMeyKodu, string aciklama)
        {
            HDICaddeSokakBulvarMeydanBinaAdResponse response = new HDICaddeSokakBulvarMeydanBinaAdResponse();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                                        s.TVMKodu == tvmKodu).
                                                                                                                        FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDICaddeSokakBulvarMeydanBinaAdRequest request = new HDICaddeSokakBulvarMeydanBinaAdRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;
                    request.code = cadSkBulMeyKodu.ToString();
                    request.aciklama = aciklama;

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDICaddeSokakBulvarMeydanBinaAdResponse>(request, out responseXML, serviceURL);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }


            return response;
        }

        public HDIDairelerResponse GetUAVTDairelerList(int binaKodu)
        {
            HDIDairelerResponse response = new HDIDairelerResponse();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                                        s.TVMKodu == tvmKodu).
                                                                                                                        FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDIDairelerRequest request = new HDIDairelerRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;
                    request.code = binaKodu.ToString();

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDIDairelerResponse>(request, out responseXML, serviceURL);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return response;
        }

        public HDIUAVTAdresResponse GetUAVTAdres(string uavtKodu)
        {
            HDIUAVTAdresResponse response = new HDIUAVTAdresResponse();
            response.KAYITLAR = new List<HDIUAVTAdresResponse.KAYIT>();

            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(_AktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &
                                                                                                                        s.TVMKodu == tvmKodu).
                                                                                                                        FirstOrDefault();

                if (servisKullanici != null)
                {
                    HDIUAVTAdresRequest request = new HDIUAVTAdresRequest();
                    request.user = servisKullanici.KullaniciAdi;
                    request.pwd = servisKullanici.Sifre;
                    request.code = uavtKodu;

                    string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UAVTServiceURL);
                    string requestXML = request.ToString();
                    string responseXML = String.Empty;

                    response = request.HttpRequestForDask<HDIUAVTAdresResponse>(request, out responseXML, serviceURL);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }


            return response;
        }

        #endregion
    }
}
