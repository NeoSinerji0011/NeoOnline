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

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    public class HDIKasko : Teklif, IHDIKasko
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
        public HDIKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService,
                        ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
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
            HDIKaskoResponse response = null;
            try
            {
                #region Teklif Hazırlama
                HDIKaskoRequest request = this.TeklifRequest(teklif);
                #endregion

                #region Servis call
                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);

                string requestXML = request.ToString();
                string responseXML = String.Empty;

                this.BeginLog(requestXML, WebServisIstekTipleri.Teklif);

                response = request.HttpRequest<HDIKaskoResponse>(serviceURL, requestXML, out responseXML);

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
                this.GenelBilgiler.BaslamaTarihi = teklif.GenelBilgiler.BaslamaTarihi;// TurkeyDateTime.Now;//HDIMessage.ToDateTime(response.PolBasTar);
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = HDIMessage.ToDecimal(response.OdenecekPrim);
                this.GenelBilgiler.NetPrim = HDIMessage.ToDecimal(response.PoliceNetPrim);
                decimal GiderVergisi = HDIMessage.ToDecimal(response.GiderVergisi);
                this.GenelBilgiler.ToplamVergi = GiderVergisi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ToplamKomisyon = HDIMessage.ToDecimal(response.ToplamKomisyon);
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;

                if (response.Taksitler.Count() > 0)
                    this.GenelBilgiler.TaksitSayisi = (byte)response.Taksitler.Count();
                else
                    this.GenelBilgiler.TaksitSayisi = 1;

                if (response.HasarKademesi != null)
                {
                    string HasarsizlikIndirim = response.HasarKademesi;

                    if (!String.IsNullOrEmpty(HasarsizlikIndirim))
                    {
                        if (HasarsizlikIndirim == "1")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 60;
                        }
                        else if (HasarsizlikIndirim == "2")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 55;
                        }
                        else if (HasarsizlikIndirim == "3")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 50;
                        }
                        else if (HasarsizlikIndirim == "4")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 40;
                        }
                        else if (HasarsizlikIndirim == "5")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 30;
                        }
                        else if (HasarsizlikIndirim == "6")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                        }
                        else if (HasarsizlikIndirim == "7")
                        {
                            this.GenelBilgiler.HasarSurprimYuzdesi = 10;
                        }
                        else if (HasarsizlikIndirim == "8")
                        {
                            this.GenelBilgiler.HasarSurprimYuzdesi = 15;
                        }
                        else if (HasarsizlikIndirim == "9")
                        {
                            this.GenelBilgiler.HasarSurprimYuzdesi = 20;
                        }
                    }
                }

                #endregion

                #region Vergiler
                this.AddVergi(KaskoVergiler.GiderVergisi, GiderVergisi);
                #endregion

                #region Teminatlar
                //this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, HDIMessage.ToDecimal(response.IhtiyariMMPrimi), 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.Koltuk_Ferdi_Kaza_Surucu_Yolcu, HDIMessage.ToDecimal(response.KoltukFerdiKazaPrimi), 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.Hukuksal_Koruma, HDIMessage.ToDecimal(response.HukuksalKorumaPrimi), 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.Yurtici_Nakliyat_Sorumluluk, HDIMessage.ToDecimal(response.YurticiNakliyeciSorPrimi), 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.Saglik, HDIMessage.ToDecimal(response.SaglikPrimi), 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.Medline, HDIMessage.ToDecimal(response.Medline), 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, HDIMessage.ToDecimal(response.AsistanHizmeti), 0, 0, 0, 0);
                this.AddTeminat(KaskoTeminatlar.Mini_Onrarim_Hizmeti, HDIMessage.ToDecimal(response.MiniOnarimHizmeti), 0, 0, 0, 0);

                // ==== KASKO ==== //
                var kasko = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "KASKO");
                if (kasko != null)
                    this.AddTeminat(KaskoTeminatlar.Kasko, HDIMessage.ToDecimal(kasko.TeminatTutar), 0, 0, 0, 0);

                // ==== GLKHH VE TERÖR ==== //
                var GLKHHT = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "GLKHH VE TERÖR");
                if (GLKHHT != null)
                    this.AddTeminat(KaskoTeminatlar.GLKHHT, HDIMessage.ToDecimal(GLKHHT.TeminatTutar), 0, 0, 0, 0);

                // ==== DEPREM ==== //
                var Deprem = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "DEPREM (MUAFİYETSİZ)");
                if (Deprem != null)
                    this.AddTeminat(KaskoTeminatlar.Deprem, HDIMessage.ToDecimal(Deprem.TeminatTutar), 0, 0, 0, 0);

                // ==== SEL SU BASKINI ==== //
                var selsubaskini = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "SEL SU BASKINI (MUAFİYETSİZ)");
                if (selsubaskini != null)
                    this.AddTeminat(KaskoTeminatlar.Seylap, HDIMessage.ToDecimal(selsubaskini.TeminatTutar), 0, 0, 0, 0);

                // ==== SİGARA VE BENZERİ MADDE ZARARLARI ==== //
                var sigarazararlari = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "SİGARA VE BENZERİ MADDE ZARARLARI");
                if (sigarazararlari != null)
                    this.AddTeminat(KaskoTeminatlar.Sigara_Ve_Benzeri_Madde_Zararlari, HDIMessage.ToDecimal(sigarazararlari.TeminatTutar), 0, 0, 0, 0);

                // ==== YETKİLİ OLMAYAN KİŞİLERCE ÇEKİLME ==== //
                var yetkisizcekilme = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "YETKİLİ OLMAYAN KİŞİLERCE ÇEKİLME");
                if (yetkisizcekilme != null)
                    this.AddTeminat(KaskoTeminatlar.YetkisizCekilme, HDIMessage.ToDecimal(yetkisizcekilme.TeminatTutar), 0, 0, 0, 0);

                // ==== KIYMET KAZANMA TENZİLİ ==== //
                var kiymetkazanmatenzili = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "KIYMET KAZANMA TENZİLİ");
                if (kiymetkazanmatenzili != null)
                    this.AddTeminat(KaskoTeminatlar.KiymetKazanmaTenzili, HDIMessage.ToDecimal(kiymetkazanmatenzili.TeminatTutar), 0, 0, 0, 0);

                // ==== KEMİRGEN TEMİNATI ==== //
                var kemirgenteminati = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "KEMİRGEN TEMİNATI");
                if (kemirgenteminati != null)
                    this.AddTeminat(KaskoTeminatlar.Hayvanlarin_Verecegi_Zarar, HDIMessage.ToDecimal(kemirgenteminati.TeminatTutar), 0, 0, 0, 0);

                // ==== İHTİYARİ MALİ MESULİYET Bedeni Şahıs Başına ==== //
                var kombine = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "İHTİYARİ MALİ MESULİYET Bedeni ve Maddi Ayırımı Yapılmaksızın Yıllık Azami");
                if (kombine != null)
                    this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, HDIMessage.ToDecimal(kombine.TeminatTutar), 0, 0, 0, 0);


                // ==== İHTİYARİ MALİ MESULİYET Bedeni Şahıs Başına ==== //
                var imm_bedenisahisbasina = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "İHTİYARİ MALİ MESULİYET Bedeni Şahıs Başına");
                if (imm_bedenisahisbasina != null)
                    this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, HDIMessage.ToDecimal(imm_bedenisahisbasina.TeminatTutar), 0, 0, 0, 0);

                // ==== İHTİYARİ MALİ MESULİYET Bedeni Kaza  Başına ==== //
                var imm_bedenikazabasina = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "İHTİYARİ MALİ MESULİYET Bedeni Kaza  Başına");
                if (imm_bedenikazabasina != null)
                    this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, HDIMessage.ToDecimal(imm_bedenikazabasina.TeminatTutar), 0, 0, 0, 0);

                // ==== İHTİYARİ MALİ MESULİYET Maddi  Kaza  Başına ==== //
                var imm_maddihasar = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "İHTİYARİ MALİ MESULİYET Maddi  Kaza  Başına");
                if (imm_maddihasar != null)
                    this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, HDIMessage.ToDecimal(imm_maddihasar.TeminatTutar), 0, 0, 0, 0);

                // ==== KOLTUK FERDİ KAZA Ölüm/Sürekli Sakatlık ==== //
                var fk_olumvesakatlik = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "KOLTUK FERDİ KAZA Ölüm/Sürekli Sakatlık");
                if (fk_olumvesakatlik != null)
                {
                    this.AddTeminat(KaskoTeminatlar.KFK_Olum, HDIMessage.ToDecimal(fk_olumvesakatlik.TeminatTutar), 0, 0, 0, 0);
                    this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, HDIMessage.ToDecimal(fk_olumvesakatlik.TeminatTutar), 0, 0, 0, 0);
                }
                // ==== KOLTUK FERDİ KAZA Tedavi Masrafları ==== //
                var fk_tedavi = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "KOLTUK FERDİ KAZA Tedavi Masrafları");
                if (fk_tedavi != null)
                    this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, HDIMessage.ToDecimal(fk_tedavi.TeminatTutar), 0, 0, 0, 0);

                // ==== HUKUKSAL KORUMA Motorlu Araca Bağlı ==== //
                var HK_Motorluaracabagli = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "HUKUKSAL KORUMA Motorlu Araca Bağlı");
                if (HK_Motorluaracabagli != null)
                    this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, HDIMessage.ToDecimal(HK_Motorluaracabagli.TeminatTutar), 0, 0, 0, 0);

                // ==== HUKUKSAL KORUMA Sürücüye Bağlı ==== //
                var HK_Surucuyebagli = response.Teminatlar.FirstOrDefault(s => s.TeminatAd == "HUKUKSAL KORUMA Sürücüye Bağlı");
                if (HK_Surucuyebagli != null)
                    this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, HDIMessage.ToDecimal(HK_Surucuyebagli.TeminatTutar), 0, 0, 0, 0);


                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                else
                {
                    // ==== HDI Response taksit sayısı ve miktarlarını donuyor. ==== //
                    if (response.Taksitler != null && response.Taksitler.Count > 1)
                    {
                        int sayac = 1;
                        foreach (var item in response.Taksitler)
                        {
                            this.AddOdemePlani(sayac, HDIMessage.ToDateTimeForKasko(item.TaksitVade), HDIMessage.ToDecimal(item.TaksitTutar),
                                                                                                    (teklif.GenelBilgiler.OdemeTipi ?? 0));
                            sayac++;
                        }
                    }
                    else
                        this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
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
            HDIKaskoResponse response = null;
            try
            {
                #region Teklif Hazırlama

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                HDIKaskoRequest request = this.TeklifRequest(teklif);
                string referansNo = this.ReadWebServisCevap(Common.WebServisCevaplar.HDI_Referans_No, String.Empty);

                request.OrjinalRefNo = referansNo;
                request.IstekTip = "O";

                //Ödeme tipi      1	KREDİ KARTI (BLOKELİ)   2 KREDİ KARTI   3 NAKİT    4 ÇEK-SENET

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    request.OdemeTipi = "2";
                }
                else if (odeme.OdemeTipi == OdemeTipleri.Nakit || odeme.OdemeTipi == OdemeTipleri.Havale)
                {
                    request.OdemeTipi = "3";
                }
                if (odeme.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    request.OdemeTipi = "1";
                }

                #endregion

                #region Servis call

                string serviceURL = _KonfigurasyonService.GetKonfigDeger(Konfig.HDI_ServiceURL);
                string requestXML = request.ToString();
                string responseXML = String.Empty;

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);
                if (odeme.KrediKarti != null && (odeme.OdemeTipi == OdemeTipleri.KrediKarti  || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti ))
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
                    request.TaksitSekli = odeme.TaksitSayisi.ToString();
                }
                else
                    request.TckR = "";
                response = request.HttpRequest<HDIKaskoResponse>(serviceURL, requestXML, out responseXML);

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

                #region Poliçe Kaydı

                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.PoliceNo;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    //Muhasebe aktarımı
                    //this.SendMuhasebe();

                    if (response.Taksitler != null && response.Taksitler.Count() > 0)
                        if (this.GenelBilgiler.TaksitSayisi != response.Taksitler.Count())
                        {
                            //Yeni taksit miktarı güncelleniyor.
                            this.GenelBilgiler.TaksitSayisi = (byte)response.Taksitler.Count();
                            this.ResetOdemePlani();

                            if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                            {

                                this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                            }
                            else
                            {
                                // ==== HDI Response taksit sayısı ve miktarlarını donuyor. ==== //
                                int sayac = 1;
                                foreach (var item in response.Taksitler)
                                {
                                    this.AddOdemePlani(sayac, HDIMessage.ToDateTimeForKasko(item.TaksitVade), HDIMessage.ToDecimal(item.TaksitTutar),
                                                                                                            (teklif.GenelBilgiler.OdemeTipi ?? 0));
                                    sayac++;
                                }
                            }
                        }

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

        public override void DekontPDF()
        {
            try
            {
                #region Request hazırlama

                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] {tvmKodu ,
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
                    string fileName = String.Format("kasko_HDI_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("kasko", fileName, data);
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
                request.Uygulama = "218";//_KonfigurasyonService.GetKonfigDeger(Konfig.HDI_UygulamaKasko);
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
                    string fileName = String.Format("kasko_HDI_{0}.pdf", System.Guid.NewGuid().ToString());
                    string url = storage.UploadFile("kasko", fileName, data);
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

        private HDIKaskoRequest TeklifRequest(ITeklif teklif)
        {
            #region Ana Bilgiler
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.HDI });
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDIKasko);

            HDIKaskoRequest request = new HDIKaskoRequest();
            request.user = servisKullanici.KullaniciAdi;
            request.pwd = servisKullanici.Sifre;
            request.Uygulama = konfig[Konfig.HDI_UygulamaKasko];
            request.IstekTip = "P";
            request.Refno = System.Guid.NewGuid().ToString();
            request.OrjinalRefNo = String.Empty;
            request.HDIPoliceNumara = String.Empty;
            request.HDIPoliceYenilemeNo = String.Empty;

            DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            request.Tarih = HDIMessage.ToHDIDate(policeBaslangic);
            #endregion

            #region Müşteri bilgileri
            MusteriGenelBilgiler sigortaEttiren = _MusteriService.GetMusteri(teklif.SigortaEttiren.MusteriKodu);

            // request.LMUSNO = sigortaEttiren.MusteriKodu.ToString();

            if (MusteriTipleri.Ozel(sigortaEttiren.MusteriTipKodu))
            {
                request.OzelTuzel = "O";
                request.MSCnsTp = sigortaEttiren.Cinsiyet;
                request.MSDogYL = sigortaEttiren.DogumTarihi.HasValue ? sigortaEttiren.DogumTarihi.Value.Year.ToString() : String.Empty;

                string MeslekKodu = teklif.ReadSoru(KaskoSorular.Meslek, "99");
                var HDIMeslekKodu = _TeklifService.GetTUMMeslekKod(MeslekKodu, TeklifUretimMerkezleri.HDI);
                if (HDIMeslekKodu != null)
                {
                    request.MSMesKd = HDIMeslekKodu.CR_MeslekKodu;
                }
                else
                {
                    request.MSMesKd = "10";
                }
                request.MSEgKd = "1";

                //TODO: Meslek Kodu
                request.MSMedKd = "1";
                request.MSCocKd = "1";

                //TODO: Ehliyet Yılı
                request.MSEhlYL = "";

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
                request.MSSekKd = "101";
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
                AdresDoldur(request, adres);
            #endregion

            #region Eski Poliçe
            HDIEskiPoliceBilgileri eskiPolice = new HDIEskiPoliceBilgileri();
            bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
            if (eskiPoliceVar)
            {
                request.EskiPoliceSirket = teklif.ReadSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                request.EskiPoliceAcente = teklif.ReadSoru(KaskoSorular.Eski_Police_Acente_No, String.Empty);
                request.EskiPoliceNo = teklif.ReadSoru(KaskoSorular.Eski_Police_No, String.Empty);
                request.EskiPoliceYenilemeNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Yenileme_No, String.Empty);
            }
            #endregion

            #region Taşıyıcı sorumluluk
            // ==== HDI taşıyıcı sorumluluk ile ilgili parametre istemiyor. ==== //

            //bool tasiyiciSorumluluk = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_VarYok, false);
            //if (tasiyiciSorumluluk)
            //{
            //    request.TasiyiciSorumluluk = "E";
            //    request.TasSorSigSirKod = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
            //    request.TasSorAcenteKod = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
            //    request.TasSorPoliceNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
            //    request.TasSorYenilemeNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
            //}
            //else
            //{
            //    //request.TasiyiciSorumluluk = "H";
            //}
            #endregion

            #region Araç Bilgileri
            string[] parts = teklif.Arac.KullanimTarzi.Split('-');

            if (parts.Length == 2)
            {
                request.SinifTarifeKod = parts[0];
                request.SinifTarifeKdEk = parts[1];
            }
            //request.TescilTarihi = HDIMessage.ToHDIDate(teklif.Arac.TrafikTescilTarihi.Value);

            request.ModelYil = teklif.Arac.Model.Value.ToString();

            string aracTipi = "";
            if (teklif.Arac.AracinTipi.Length == 1) aracTipi = "00" + teklif.Arac.AracinTipi;
            else if (teklif.Arac.AracinTipi.Length == 2) aracTipi = "0" + teklif.Arac.AracinTipi;
            else aracTipi = teklif.Arac.AracinTipi;
            request.AracListeKod = teklif.Arac.Marka + aracTipi;

            if (teklif.Arac.TrafikCikisTarihi.HasValue)
                request.TrafigeCikisTarihi = teklif.Arac.TrafikCikisTarihi.Value.ToString("ddMMyyyy");
            else
                request.TrafigeCikisTarihi = TurkeyDateTime.Now.AddDays(-30).ToString("ddMMyyyy");

            request.MotorNo = teklif.Arac.MotorNo;
            request.SasiNo = teklif.Arac.SasiNo;

            AracModel aracModel = _AracContext.AracModelRepository.Filter(s => s.MarkaKodu == teklif.Arac.Marka &&
                                                                               s.Model == teklif.Arac.Model &&
                                                                               s.TipKodu == teklif.Arac.AracinTipi).FirstOrDefault();
            if (aracModel != null && aracModel.Fiyat.HasValue)
                request.AracDegeri = aracModel.Fiyat.Value.ToString();

            AracMarka marka = _AracContext.AracMarkaRepository.FindById(teklif.Arac.Marka);
            if (marka != null)
                request.Marka = marka.MarkaAdi;

            request.AracTipiModeli = String.Empty;

            request.PlakailKod = teklif.Arac.PlakaKodu;
            string Plaka = teklif.Arac.PlakaNo;

            if (Plaka == "YK")
            {
                Plaka = Plaka + "1234";
            }
            request.PlakaKod2 = Plaka;


            if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) && !String.IsNullOrEmpty(teklif.Arac.TescilSeriNo))
            {
                request.TBSKd = teklif.Arac.TescilSeriKod;
                request.TBSNo = teklif.Arac.TescilSeriNo;
            }

            bool dainiMurteinVar = teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
            string dainiMurteinUnvan = teklif.ReadSoru(KaskoSorular.DainiMurtein_Unvan, "");
            if (dainiMurteinVar)
            {
                request.DainiMurtein = "E";
                request.DainMurteinTanim = dainiMurteinUnvan;
            }


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

            // ==== ihtiyari kademe kontrol ediliyor ==== //
            string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
            if (!String.IsNullOrEmpty(immKademe))
            {
                request.ihtiyariKdVarmi = "E";
                CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();
                var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                if (IMMBedel != null)
                {
                    CRKademeNo = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.HDI, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                }
                if (CRKademeNo != null)
                {
                    request.IhtiyariKademe = CRKademeNo.Kademe.ToString();
                }
            }
            else
                request.ihtiyariKdVarmi = "H";


            // ==== koltuk ferdi kademe kontrol ediliyor ==== //
            string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
            if (!String.IsNullOrEmpty(fkKademe))
            {
                request.koltukferdiKdVarmi = "E";
                CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                if (FKBedel != null)
                {
                    CRKademeNo = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.HDI, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, parts[0], parts[1]);
                }
                if (CRKademeNo != null)
                {
                    request.KoltukFerdiKademe = CRKademeNo.Kademe.ToString();
                }
            }
            else
                request.koltukferdiKdVarmi = "H";


            // ==== Koltuk Sayısı ==== //
            AracTip aracTip = _AracContext.AracTipRepository.FindById(new object[] { teklif.Arac.Marka, teklif.Arac.AracinTipi });
            if (aracTip != null)
                request.KoltukSayisi = aracTip.KisiSayisi.ToString();

            var aracParts = teklif.Arac.KullanimTarzi.Split('-');
            // ==== Hukuksal Teminat zorunlu Kademeside DEFAULT Değiştirilebilir ==== //
            request.HukuksalKorKdVarmi = "E";
            request.HkkslKorumaKademe = "4"; //DEfault parametre
            string hukuksalKoruma = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, "");
            if (!String.IsNullOrEmpty(hukuksalKoruma))
            {
                var hkBedel = _TeklifService.getHkKademesi(Convert.ToInt32(hukuksalKoruma));
                string HDIHukuksalKorumaKademe = _TeklifService.HDIHukuksalKorumaKademesi(aracParts[0], aracParts[1], hkBedel);
                if (!String.IsNullOrEmpty(HDIHukuksalKorumaKademe))
                {
                    request.HkkslKorumaKademe = HDIHukuksalKorumaKademe;
                }                
            }

                // ==== Yurt içi taşıyıcı varmı ==== //
                string yurticiTasiyici = teklif.ReadSoru(KaskoSorular.Yurt_ici_Tasiyici_VarYok, String.Empty);
            if (!String.IsNullOrEmpty(yurticiTasiyici))
            {
                request.YurtIciTasiyiciVarmi = "E";
                request.YutIciTasiyiciKademe = "1"; //DEfault parametre
            }
            else
                request.YurtIciTasiyiciVarmi = "H";


            // ==== İsteğe bağlı teminatlar ==== //

            // === Deprem === //
            bool deprem = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
            if (deprem)
            {
                request.Deprem = "E";
                request.T2301Kdm = "0";
            }
            else
                request.Deprem = "H";


            // === Sel SU === //
            bool selsu = teklif.ReadSoru(KaskoSorular.Sel_Su_VarYok, false);
            if (selsu)
            {
                request.SelSu = "E";
                request.T2303Kdm = "0";
            }
            else
                request.SelSu = "H";

            request.AnahtarKaybi = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false) ? "E" : "H";
            request.KullanimKaybi = teklif.ReadSoru(KaskoSorular.KullanimGelirKaybiVarMi, false) ? "E" : "H";
            //request.Alarm = teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false) ? "E" : "H";
            request.Saglik = teklif.ReadSoru(KaskoSorular.Saglik_VarYok, false) ? "E" : "H";

            if (request.Saglik == "E")
                request.KisiSayi = teklif.ReadSoru(KaskoSorular.Saglik_Kisi_Sayisi, String.Empty);

            request.WS31IA = teklif.ReadSoru(KaskoSorular.Ikame_Arac_Teminati_VarYok, false) ? "E" : "H";
            request.T2345 = teklif.ReadSoru(KaskoSorular.Hasarsizlik_Koruma_VarYok, false) ? "E" : "H";

            //Kasa Bedeli Varsa Gönderiliyor..
          
            if (aracParts != null)
            {
                string tarz1 = aracParts[0].ToString();
                if (tarz1 == "523" || tarz1 == "521")
                {
                    List<mapfre.ListWS> listeler = new List<mapfre.ListWS>();

                    List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR)
                                                             .ToList<TeklifAracEkSoru>();

                    if (aksesuarlar.Count > 0)
                    {
                        foreach (TeklifAracEkSoru item in aksesuarlar)
                        {
                            if (item.SoruKodu == MapfreAksesuarTipleri.Kasa)
                            {
                                request.T2180 = item.Bedel.ToString(); //Kasa Bedeli
                                break;
                            }
                        }
                    }
                }
            }
            //--------------------

            var meslek = teklif.ReadSoru(KaskoSorular.Meslek, "");
            if (!String.IsNullOrEmpty(meslek))
            {
                request.T2451 = "E";//Meslek İndirimi 
            }
            else
            {
                request.T2451 = "H";//Meslek İndirimi
            }

            TeklifAracEkSoru tasinanYuk = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.TASINAN_YUK).FirstOrDefault();
            if (tasinanYuk != null)
            {
                request.TasinanYukKademe = tasinanYuk.SoruKodu;
                request.TasinanYukTeminatDegeri = tasinanYuk.Bedel.HasValue ? tasinanYuk.Bedel.Value.ToString() : "";
                request.WS1MRKTSYACK = tasinanYuk.Aciklama;
            }

            #endregion

            #region Kasko Servis yedekParca
            string kaskoServis = teklif.ReadSoru(KaskoSorular.Kasko_Turu, "1");
            string servisKodu = teklif.ReadSoru(KaskoSorular.Servis_Turu, "1");

            request.DainiMurtein = "H";
            request.WSHSST = kaskoServis;
            request.WSHSPT = servisKodu == "1" ? "O" : "E";
            #endregion
            string kisiselEsya=teklif.ReadSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, true)? "E":"H";
            string patlayiciParlayici=teklif.ReadSoru(KaskoSorular.HDIPatlayiciParlayici, true)? "E":"H";
            string rayicDeger=teklif.ReadSoru(KaskoSorular.HDIRayicBedelKoruma, true)? "E":"H";
            string hataliAkaryakit = teklif.ReadSoru(KaskoSorular.HataliAkaryakitAlimi, true)? "E":"H";
            request.KisiselEsya = kisiselEsya;
           
            //RAYİÇ DEĞER KORUMA TEMİNATI (E/H)
            request.T2355 = rayicDeger;

            //PATLAYICI PARLAYICI VE YANICI MADDE TASIMA E/H
            request.T2322 = patlayiciParlayici;

            //Yanlış Akaryakıt Alımı Teminatı (E/H)
            request.T2354 = hataliAkaryakit;
            
            #region Ödeme Seçenekleri

            // ==== Odeme tipi ve Taksit Sayısı ==== //
            byte? odemeSekli = teklif.GenelBilgiler.OdemeSekli;
            byte? odemeTipi = teklif.GenelBilgiler.OdemeTipi;

            if (odemeSekli.HasValue && odemeSekli == 2)
            {
                byte? taksit = teklif.GenelBilgiler.TaksitSayisi;
                if (taksit.HasValue)
                    request.TaksitSekli = taksit.ToString();
                else
                    request.TaksitSekli = "1";

                switch (odemeTipi)
                {
                    case 1: if (taksit.HasValue && taksit > 6) request.TaksitSekli = "6"; break;
                    case 3: if (taksit.HasValue && taksit > 6) request.TaksitSekli = "6"; break;
                }
            }
            else
                request.TaksitSekli = "1";


            switch (odemeTipi)
            {
                // ==== 3 Nakit ==== //
                case 1: request.OdemeTipi = "3"; break;
                // ==== 1 KREDİ KARTI (BLOKELİ) ==== //
                case 2: request.OdemeTipi = "1"; break;
                // ==== Odeme Tipi havale olduğunda hdi bunu kabul etmiyor.  3 Nakit ==== //
                case 3: request.OdemeTipi = "3"; break;
            }


            #endregion

            return request;
        }

        public void AdresDoldur(HDIKaskoRequest request, MusteriAdre adres)
        {
            if (adres.UlkeKodu != "TUR")
            {
                request.IlKod = "999";
                request.Ilce = "YURTDIŞI";
            }
            else
            {
                CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.HDI &&
                                                                              f.IlKodu == adres.IlKodu &&
                                                                              f.IlceKodu == adres.IlceKodu)
                                                                                                         .Single<CR_IlIlce>();

                if (ililce != null)
                {
                    request.Ilce = ililce.CRIlceAdi;
                    request.IlKod = ililce.CRIlKodu;
                }
            }
            // HDI canlı ortamda il ve ilçeden farklı diğer iki adres zorunlu isteniyor. Sigorta Ettiren ekranından bu bilgiler alınmıyor.
            //Cadde ve mahalle boş ise "..." gönderiliyor.
            if (!String.IsNullOrEmpty(adres.Cadde))
                request.Cadde = adres.Cadde;
            else
                request.Cadde = "...";

            request.Sokak = adres.Sokak;
            request.Semt = adres.Semt;

            if (!String.IsNullOrEmpty(adres.Mahalle))
                request.KoyMahalle = adres.Mahalle;
            else request.KoyMahalle = "...";

            request.BinaNo = adres.BinaNo;
            request.Daire = adres.DaireNo;
            request.HanApartmanAd = adres.Apartman;
            request.PostaKod = adres.PostaKodu.ToString();
        }

        public HDIPlakaSorgulamaResponse PlakaSorgula(string plakaKodu, string plakaNo, short musteriTipKodu, string kimlikNo)
        {
            HDIPlakaSorgulamaResponse response = null;
            try
            {
                IAktifKullaniciService aktifKullanici = DependencyResolver.Current.GetService<IAktifKullaniciService>();
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(aktifKullanici.TVMKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.HDI });
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleHDIPlaka);

                HDIPlakaSorgulamaRequest request = new HDIPlakaSorgulamaRequest();
                request.user = servisKullanici.KullaniciAdi;
                request.pwd = servisKullanici.Sifre;
                request.Uygulama = konfig[Konfig.HDI_UygulamaPlakaSorKasko];
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
