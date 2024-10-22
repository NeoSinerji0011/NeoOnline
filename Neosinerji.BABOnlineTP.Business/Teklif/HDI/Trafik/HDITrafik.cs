using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    public class HDITrafik : Teklif, IHDITrafik
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        ITVMService _TVMService;
        [InjectionConstructor]
        public HDITrafik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
            : base()
        {
            _CRService = crService;
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
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
            try
            {
                #region Veri Hazırlama
                HDITrafikRequest request = this.TeklifRequest(teklif);
                #endregion

                #region Servis call
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);
                HDITrafikResponse response = null;

                string requestXML = request.ToString();
                string responseXML = String.Empty;

                this.BeginLog(requestXML, WebServisIstekTipleri.Teklif);

                response = request.HttpRequest<HDITrafikResponse>(serviceURL, requestXML, out responseXML);

                if (response == null)
                {
                    this.AddHata("Teklif bilgileri alınamadı.");
                }
                else if (response.Durum != "0")
                {
                    this.EndLog(responseXML, false);
                    this.AddHata(response.DurumMesaj);
                }
                else
                    this.EndLog(responseXML, true);
                #endregion

                #region Basarılı Kontrol
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
                this.GenelBilgiler.BaslamaTarihi = HDIMessage.ToDateTime(response.PolBasTar);
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = HDIMessage.ToDecimal(response.OdenenTutar);
                this.GenelBilgiler.NetPrim = HDIMessage.ToDecimal(response.NetPrim);

                decimal THGFonu = HDIMessage.ToDecimal(response.THGFonu);
                decimal GiderVergisi = HDIMessage.ToDecimal(response.GiderVergisi);
                decimal GarantiFonu = HDIMessage.ToDecimal(response.GarantiFonu);

                this.GenelBilgiler.ToplamVergi = THGFonu + GiderVergisi + GarantiFonu;
                this.GenelBilgiler.TaksitSayisi = Convert.ToByte(response.UygulananTaksitSayisi);
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.GecikmeZammiYuzdesi = HDIMessage.ToDecimal(response.QP30CP);
                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = HDIMessage.ToDecimal(response.QPHIOR);
                this.GenelBilgiler.HasarSurprimYuzdesi = HDIMessage.ToDecimal(response.QPHSOR);
                this.GenelBilgiler.ZKYTMSYüzdesi = HDIMessage.ToDecimal(response.QPTSOR);
                this.GenelBilgiler.ToplamKomisyon = HDIMessage.ToDecimal(response.ToplamKomisyon);

                // ==== Güncellenicek. ==== //
                //this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                //this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                #endregion

                #region Vergiler
                this.AddVergi(TrafikVergiler.THGFonu, THGFonu);
                this.AddVergi(TrafikVergiler.GiderVergisi, GiderVergisi);
                this.AddVergi(TrafikVergiler.GarantiFonu, GarantiFonu);
                #endregion

                #region Teminatlar
                this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, HDIMessage.ToDecimal(response.MADDIARACBASINA), 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, HDIMessage.ToDecimal(response.MADDIKAZABASINA), 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, HDIMessage.ToDecimal(response.TEDAVIKISIBASINA), 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, HDIMessage.ToDecimal(response.TEDAVIKAZABASINA), 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, HDIMessage.ToDecimal(response.TEDAVIDISIKISIBASINA), 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, HDIMessage.ToDecimal(response.TEDAVIDISIKAZABASINA), 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kisi_Basina, 0, 0, HDIMessage.ToDecimal(response.IMMPrimi), 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Asistans, 0, 0, HDIMessage.ToDecimal(response.AsistanPrimi), 0, 0);
                this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Tedavi, 0, 0, HDIMessage.ToDecimal(response.KoltukPrimi), 0, 0);
                this.AddTeminat(TrafikTeminatlar.Trafik, 0, 0, HDIMessage.ToDecimal(response.TrafikNetPrimi), HDIMessage.ToDecimal(response.Prim), 0);
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

                #region Web servis cevapları
                this.AddWebServisCevap(Common.WebServisCevaplar.HDI_Referans_No, response.ReferansNo);
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
            try
            {
                #region Ver Hazırlama GENEL

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                HDITrafikRequest request = this.TeklifRequest(teklif);
                string referansNo = this.ReadWebServisCevap(Common.WebServisCevaplar.HDI_Referans_No, String.Empty);

                request.OrjinalRefNo = referansNo;
                request.IstekTip = "O";

                if (odeme.OdemeSekli == OdemeSekilleri.Pesin)
                    request.OdemeSekli = "P";
                else
                {
                    request.OdemeSekli = "V";
                }

                #endregion

                #region Servis call

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);
                HDITrafikResponse response = null;

                string requestXML = request.ToString();
                string responseXML = String.Empty;

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);

                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    //Kart Bilgileri log dosyasına eklenmiyor.
                    string kk = String.Format("{0}{1}", odeme.KrediKarti.KartNo, odeme.KrediKarti.CVC);
                    if (!String.IsNullOrEmpty(kk))
                    {
                        char[] kkVal = new char[] { kk[7], kk[12], kk[10], kk[2], kk[14], kk[17], kk[5], kk[0], kk[15], kk[9], kk[4], kk[18], kk[1], kk[13], kk[8], kk[16], kk[11], kk[3], kk[6] };
                        request.TckR = String.Format("{0}{1}{2}", new string(kkVal), odeme.KrediKarti.SKA, odeme.KrediKarti.SKY);
                    }
                    else
                    {
                        request.TckR = "";
                    }
                }
                else request.TckR = "";
                request.TaksitSayisi = odeme.TaksitSayisi.ToString();               
                response = request.HttpRequest<HDITrafikResponse>(serviceURL, requestXML, out responseXML);

                if (response == null)
                {
                    this.AddHata("Poliçe bilgileri alınamadı.");
                }
                else if (response.Durum != "0")
                {
                    this.EndLog(responseXML, false);
                    this.AddHata(response.DurumMesaj);
                }
                else
                    this.EndLog(responseXML, true);

                #endregion

                #region Hata kontrol ve kayıt

                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.PoliceNo;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    //Muhasebe aktarımı
                    //this.SendMuhasebe();

                    this.PolicePDF();
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
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

                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu,
                                                                                                                    TeklifUretimMerkezleri.HDI });
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_PDFServiceURL);

                HDIPDFRequest request = new HDIPDFRequest();
                request.PoliceNumarasi = this.GenelBilgiler.TUMPoliceNo;
                request.user = servisKullanici.KullaniciAdi;
                request.pwd = servisKullanici.Sifre;

                // ====Uygulama kodunu KASKOV3  olarak gonderdiğimde uygulama hatası alıyoruz. ==== //
                request.Uygulama = "231";//Sadece ilgili rakamlar gönderilmeli (231:Trafik) //_KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UygulamaTrafik);

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
                    string fileName = String.Format("trafik_HDI_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("trafik", fileName, data);
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

        public override void DekontPDF()
        {
            try
            {
                #region Request hazırlama

                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);

                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu,
                                                                                                                    TeklifUretimMerkezleri.HDI });
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_PDFServiceURL);

                HDIPDFRequest request = new HDIPDFRequest();
                request.PoliceNumarasi = this.GenelBilgiler.TUMPoliceNo;
                request.user = servisKullanici.KullaniciAdi;
                request.pwd = servisKullanici.Sifre;

                // ==== poliçe basım servisinde uygulama alanının değerini MAKBUZ olarak göndermeniz durumunda dekont pdf'ini alabilirsiniz. ==== //
                request.Uygulama = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UygulamaDekont);

                this.BeginLog(request, typeof(HDIPDFRequest), WebServisIstekTipleri.DekontPDF);

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
                    string fileName = String.Format("trafik_HDI_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("trafik", fileName, data);
                    //this.GenelBilgiler.PDFGenelSartlari = url;
                    this.GenelBilgiler.PDFDekont = url;

                    this.EndLog("PDFDekontURL :" + url, true);

                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                    _Log.Info("PDF dokumanı oluşturuldu : {0}", url);
                }


                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                _Log.Error(ex);
                this.EndLog(ex.Message, false);
                throw;
                #endregion
            }

        }

        private HDITrafikRequest TeklifRequest(ITeklif teklif)
        {
            #region Ana Bilgiler
            //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.HDI });

            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);

            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.HDI });
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDITrafik);

            HDITrafikRequest request = new HDITrafikRequest();
            request.user = servisKullanici.KullaniciAdi;
            request.pwd = servisKullanici.Sifre;
            request.Uygulama = konfig[Konfig.HDI_UygulamaTrafik];
            request.IstekTip = "P";
            request.Refno = System.Guid.NewGuid().ToString();
            request.OrjinalRefNo = String.Empty;
            request.HDIPoliceNumara = String.Empty;
            request.HDIPoliceYNo = String.Empty;

            DateTime policeBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            request.Tarih = HDIMessage.ToHDIDate(policeBaslangic);
            #endregion

            #region Müşteri bilgileri
            MusteriGenelBilgiler sigortaEttiren = _MusteriService.GetMusteri(teklif.SigortaEttiren.MusteriKodu);

            request.LMUSNO = sigortaEttiren.MusteriKodu.ToString();

            if (MusteriTipleri.Ozel(sigortaEttiren.MusteriTipKodu))
            {
                request.OzelTuzel = "O";
                request.MSCnsTp = sigortaEttiren.Cinsiyet;
                request.MSDogYL = sigortaEttiren.DogumTarihi.HasValue ? sigortaEttiren.DogumTarihi.Value.Year.ToString() : String.Empty;
                request.MSEgKd = "1";
                request.MSMesKd = "10";

                //TODO: Meslek Kodu
                request.MSMedKd = "0";
                request.MSCocKd = "0";

                //TODO: Ehliyet Yılı
                request.MSEhlYL = "0";

                if (sigortaEttiren.Uyruk == UyrukTipleri.TC)
                {
                    request.TcKimlikNo = sigortaEttiren.KimlikNo;
                    request.VergiKimlikNo = sigortaEttiren.KimlikNo;
                    request.YabanciKimlikNo = String.Empty;
                    request.PasaportNo = String.Empty;
                    request.Uyruk = "0";
                }
                else
                {
                    request.TcKimlikNo = String.Empty;
                    request.VergiKimlikNo = String.Empty;
                    request.YabanciKimlikNo = sigortaEttiren.KimlikNo;
                    request.PasaportNo = sigortaEttiren.PasaportNo;
                    request.Uyruk = "1";
                }
            }
            else
            {
                request.OzelTuzel = "T";
                request.TcKimlikNo = String.Empty;
                request.VergiKimlikNo = sigortaEttiren.KimlikNo;
                //TODO: İş Kodu
                request.MSSekKd = "";
                request.Uyruk = "0";
            }
            #endregion

            #region Telefonlar
            List<MusteriTelefon> telefonlar = sigortaEttiren.MusteriTelefons.ToList<MusteriTelefon>();
            List<MusteriTelefon> isTelefonlari = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Is)
                                                           .Take(2)
                                                           .ToList<MusteriTelefon>();

            if (isTelefonlari.Count > 0)
                request.IsTel1 = HDIMessage.ToHDITelefon(isTelefonlari[0].Numara);
            if (isTelefonlari.Count > 1)
                request.IsTel1 = HDIMessage.ToHDITelefon(isTelefonlari[1].Numara);

            List<MusteriTelefon> evTelefonlari = telefonlar.Where(w => w.IletisimNumaraTipi == IletisimNumaraTipleri.Ev)
                                                 .Take(2)
                                                 .ToList<MusteriTelefon>();

            if (evTelefonlari.Count > 0)
                request.EvTel1 = HDIMessage.ToHDITelefon(evTelefonlari[0].Numara);
            if (evTelefonlari.Count > 1)
                request.EvTel2 = HDIMessage.ToHDITelefon(evTelefonlari[1].Numara);
            #endregion

            #region Adres bilgileri
            MusteriAdre adres = sigortaEttiren.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
            if (adres != null)
            {
                HDIAdresBilgi adresBilgi = new HDIAdresBilgi();

                if (adres.UlkeKodu != "TUR")
                {
                    adresBilgi.IlKod = "999";
                    adresBilgi.Ilce = "YURTDIŞI";
                    adresBilgi.IKIlKd = "999";
                    adresBilgi.IKIlc = "YURTDIŞI";
                }
                else
                {
                    CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.HDI &&
                                                                                  f.IlKodu == adres.IlKodu &&
                                                                                  f.IlceKodu == adres.IlceKodu)
                                                                     .Single<CR_IlIlce>();

                    if (ililce != null)
                    {
                        adresBilgi.Ilce = ililce.CRIlceAdi;
                        adresBilgi.IlKod = ililce.CRIlKodu;
                        adresBilgi.IKIlKd = ililce.CRIlKodu;
                        adresBilgi.IKIlc = ililce.CRIlceAdi;
                    }
                }
                // HDI canlı ortamda il ve ilçeden farklı diğer iki adres zorunlu isteniyor. Sigorta Ettiren ekranından bu bilgiler alınmıyor.
                //Cadde ve mahalle boş ise "..." gönderiliyor.
                if (!String.IsNullOrEmpty(adres.Cadde))
                    adresBilgi.Cadde = adres.Cadde;
                else
                    adresBilgi.Cadde = "...";

                adresBilgi.Sokak = adres.Sokak;
                adresBilgi.Semt = adres.Semt;

                if (!String.IsNullOrEmpty(adres.Mahalle))
                    adresBilgi.KoyMahalle = adres.Mahalle;
                else adresBilgi.KoyMahalle = "...";

                adresBilgi.BinaNo = adres.BinaNo;
                adresBilgi.Daire = adres.DaireNo;
                adresBilgi.HanApartmanAd = adres.Apartman;
                adresBilgi.PostaKod = adres.PostaKodu.ToString();

                //Tescil il kodu ve ilçe adı
                string tescilIlKodu = teklif.Arac.TescilIlKodu;
                string tescilIlceKodu = teklif.Arac.TescilIlceKodu;

                if (!String.IsNullOrEmpty(tescilIlceKodu) && !String.IsNullOrEmpty(tescilIlceKodu))
                {
                    CR_TescilIlIlce tescilIl = _CRContext.CR_TescilIlIlceRepository.Filter(f => f.IlKodu == tescilIlKodu && f.IlceKodu == tescilIlceKodu)
                                                                                   .Single<CR_TescilIlIlce>();

                    adresBilgi.TSIlKd = tescilIlKodu;

                    if (tescilIl != null)
                        adresBilgi.TSIlc = tescilIl.TescilIlceAdi;
                }
                request.AdresBilgi = adresBilgi;
            }
            #endregion

            #region Eski Poliçe
            HDIEskiPoliceBilgileri eskiPolice = new HDIEskiPoliceBilgileri();
            bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
            if (eskiPoliceVar)
            {
                eskiPolice.EskiPoliceSirket = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                eskiPolice.EskiPoliceAcente = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                eskiPolice.EskiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                eskiPolice.EskiPoliceYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
            }
            request.EskiPoliceBilgileri = eskiPolice;
            #endregion

            #region Taşıyıcı sorumluluk
            bool tasiyiciSorumluluk = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, false);
            if (tasiyiciSorumluluk)
            {
                request.TasiyiciSorumluluk = "E";
                request.TasSorSigSirKod = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
                request.TasSorAcenteKod = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                request.TasSorPoliceNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                request.TasSorYenilemeNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
            }
            else
            {
                request.TasiyiciSorumluluk = "H";
            }
            #endregion

            #region Araç Bilgileri
            string[] parts = teklif.Arac.KullanimTarzi.Split('-');

            if (parts.Length == 2)
            {
                request.SinifTarifeKod = parts[0];
                request.STRFKd2 = parts[1];
            }
            request.TescilTarihi = HDIMessage.ToHDIDate(teklif.Arac.TrafikTescilTarihi.Value);
            request.ModelYili = teklif.Arac.Model.Value.ToString();

            string aracTipi = "";
            if (teklif.Arac.AracinTipi.Length == 1) aracTipi = "00" + teklif.Arac.AracinTipi;
            else if (teklif.Arac.AracinTipi.Length == 2) aracTipi = "0" + teklif.Arac.AracinTipi;
            else aracTipi = teklif.Arac.AracinTipi;
            request.AracListeKod = teklif.Arac.Marka + aracTipi;

            AracMarka marka = _AracContext.AracMarkaRepository.FindById(teklif.Arac.Marka);
            if (marka != null)
                request.Marka = marka.MarkaAdi;

            request.AracTipiModeli = String.Empty;

            request.PlakailKod = teklif.Arac.PlakaKodu;
            request.PlakaKod2 = teklif.Arac.PlakaNo;

            if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) && !String.IsNullOrEmpty(teklif.Arac.TescilSeriNo))
            {
                request.TBSKd = teklif.Arac.TescilSeriKod;
                request.TBSNo = teklif.Arac.TescilSeriNo;
            }

            request.MotorNo = teklif.Arac.MotorNo;
            request.SasiNo = teklif.Arac.SasiNo;
            #endregion

            #region Acente ve kullanıcı bilgileri
            //request.SubeKod = teklif.GenelBilgiler.TVMKodu.ToString();

            TVMKullanicilar kullanici = _TVMContext.TVMKullanicilarRepository.FindById(teklif.GenelBilgiler.TVMKullaniciKodu);

            if (kullanici != null)
            {
                request.Reseller_user_ID = kullanici.TCKN;
                request.Reseller_user_firstname = kullanici.Adi;
                request.Reseller_user_lastname = kullanici.Soyadi;
            }
            #endregion

            #region Teminat seçenekleri
            string immKademe = teklif.ReadSoru(TrafikSorular.Teminat_IMM_Kademe, String.Empty);
            if (!String.IsNullOrEmpty(immKademe))
            {
                request.IMMTeminati = "E";

                CR_TrafikIMM CRKademeNo = new CR_TrafikIMM();
                var IMMBedel = _CRService.GetTrafikIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                if (IMMBedel != null)
                {
                    CRKademeNo = _CRService.GetCRTrafikIMMBedel(TeklifUretimMerkezleri.HDI, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                }
                if (CRKademeNo != null)
                {
                    request.WSIMMK = CRKademeNo.Kademe.ToString();
                }
            }
            else
            {
                request.IMMTeminati = "H";
            }

            string fkKademe = teklif.ReadSoru(TrafikSorular.Teminat_FK_Kademe, String.Empty);
            if (!String.IsNullOrEmpty(fkKademe))
            {
                request.WSKOLS = "E";
                CR_TrafikFK CRKademeNo = new CR_TrafikFK();
                var FKBedel = _CRService.GetTrafikFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                if (FKBedel != null)
                {
                    CRKademeNo = _CRService.GetCRTrafikFKBedel(TeklifUretimMerkezleri.HDI, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, parts[0], parts[1]);
                }
                if (CRKademeNo != null)
                {
                    request.WSKOLK = CRKademeNo.Kademe.ToString();
                }
                else request.WSKOLS = "H";
            }
            else
            {
                request.WSKOLS = "H";
            }

            request.WSHUKS = "H";

            bool asistans = teklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);
            request.AsistanHizmeti = asistans ? "E" : "H";

            //Koltuk sayısıs
            AracTip aracTip = _AracContext.AracTipRepository.FindById(new object[] { teklif.Arac.Marka, teklif.Arac.AracinTipi });
            if (aracTip != null)
            {
                request.WSKOSY = aracTip.KisiSayisi.ToString();
            }
            #endregion

            #region Ödeme Seçenekleri
            if (this.OdemePlaniAlternatifKodu == OdemePlaniAlternatifKodlari.Pesin)
                request.OdemeSekli = "P";
            else
            {
                request.OdemeSekli = "V";
                request.TaksitSayisi = OdemePlaniAlternatifKodlari.TaksitSayisi(this.OdemePlaniAlternatifKodu).ToString();
            }
            #endregion

            return request;
        }

        public HDIPlakaSorgulamaResponse PlakaSorgula(string plakaKodu, string plakaNo, short musteriTipKodu, string kimlikNo)
        {
            HDIPlakaSorgulamaResponse response = null;
            try
            {
                IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { 100, TeklifUretimMerkezleri.HDI }); // aktifKullanici.TVMKodu 100 yerine yazılacak
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDIPlaka);

                HDIPlakaSorgulamaRequest request = new HDIPlakaSorgulamaRequest();
                request.user = servisKullanici.KullaniciAdi;
                request.pwd = servisKullanici.Sifre;
                request.Uygulama = konfig[Konfig.HDI_UygulamaPlakaSor];
                request.Refno = System.Guid.NewGuid().ToString();
                request.PlakaIlKd = plakaKodu.Length == 2 ? "0" + plakaKodu : plakaKodu;
                request.PlakaNo = plakaNo;

                switch (musteriTipKodu)
                {
                    case MusteriTipleri.TCMusteri: request.KimlikTipi = "TC"; break;
                    case MusteriTipleri.SahisFirmasi: request.KimlikTipi = "TC"; break;
                    case MusteriTipleri.TuzelMusteri: request.KimlikTipi = "VR"; break;
                    default: request.KimlikTipi = ""; break;
                }

                request.KimlikNo = kimlikNo;

                string serviceURL = konfig[Konfig.HDI_ServiceURL];
                string requestXML = request.ToString();
                string responseXML = String.Empty;

                response = request.HttpRequest<HDIPlakaSorgulamaResponse>(serviceURL, requestXML, out responseXML);

                if (response != null && response.Durum != null && response.Durum != "0")
                {
                    _Log.Error("HDI SİGORTA Plaka Sorgulama : {0}", response.DurumMesaj);
                    throw new Exception(response.DurumMesaj);
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
            }

            return response;
        }
    }
}
