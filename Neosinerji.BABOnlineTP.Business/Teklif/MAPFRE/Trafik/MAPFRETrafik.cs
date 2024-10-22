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

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MAPFRETrafik : Teklif, IMAPFRETrafik
    {
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        ITVMService _TVMService;
        public MAPFRETrafik(TeklifGenel teklifGenel)
            : base(teklifGenel)
        {
            _CRContext = DependencyResolver.Current.GetService<ICRContext>();
            _MusteriService = DependencyResolver.Current.GetService<IMusteriService>();
            _AracContext = DependencyResolver.Current.GetService<IAracContext>();
            _KonfigurasyonService = DependencyResolver.Current.GetService<IKonfigurasyonService>();
            _TVMContext = DependencyResolver.Current.GetService<ITVMContext>();
            _Log = DependencyResolver.Current.GetService<ILogService>();
            _TeklifService = DependencyResolver.Current.GetService<ITeklifService>();
        }

        [InjectionConstructor]
        public MAPFRETrafik(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService,  ITVMService TVMService)
            : base()
        {
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
             _TVMService=TVMService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.MAPFRE;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            mapfre.TanzimWSService ws = null;
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });
            try
            {
                #region Veri Hazırlama
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                mapfre.ZeyilWS request = this.TeklifRequest(teklif, konfig);
                request.isBasim = true;
                #endregion

                #region Servis call
                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                mapfre.ZeyilWS response = null;

                this.BeginLog(request, typeof(mapfre.ZeyilWS), WebServisIstekTipleri.Teklif);

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                ws.Timeout = 150000;
                response = ws.issueTeklif(request);
                ws.Dispose();

                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                    this.EndLog(response, false, typeof(mapfre.ZeyilWS));
                }
                else
                {
                    this.EndLog(response, true, typeof(mapfre.ZeyilWS));
                }
                #endregion

                #region Basarı Kontrol
                if (!this.Basarili)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    return;
                }
                #endregion

                #region Teklif Kaydı
                #region Genel Bilgiler
                this.Import(teklif);

                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = MAPFREHelper.ToDateTime(response.polBaslangicTarihi);
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = response.primBilgileriWS.burutPrim.Value;
                this.GenelBilgiler.NetPrim = response.primBilgileriWS.netPrim.Value;
                this.GenelBilgiler.ToplamKomisyon = (decimal)response.primBilgileriWS.komisyon;
                this.GenelBilgiler.TUMTeklifNo = response.polPoliceNo.ToString();

                // MApfre Genel Sigorta Trafik Branşında teklif pdf i vermemektedir.
                #region TEKLİF PDF 
                //MapfrePrintRequest requestPDF = new MapfrePrintRequest();
                //requestPDF.userName = servisKullanici.KullaniciAdi;
                //requestPDF.passWord = servisKullanici.Sifre;
                //requestPDF.p_num_poliza = response.polPoliceNo;
                //requestPDF.p_num_spto = response.polZeyilNo.Value;
                //requestPDF.p_num_apli = 0;
                //requestPDF.p_num_spto_apli = 0;
                //requestPDF.p_num_riesgos = 1;
                //requestPDF.p_tip_emision = "C";

                //this.BeginLog(requestPDF, typeof(MapfrePrintRequest), WebServisIstekTipleri.Police);
                //mapfre.ZeyilWS responsePDF = null;
                //ws = new mapfre.TanzimWSService();
                //ws.Url = serviceURL;
                //responsePDF = ws.print(requestPDF.userName, requestPDF.passWord, requestPDF.p_num_poliza, requestPDF.p_num_spto, requestPDF.p_num_apli, requestPDF.p_num_spto_apli, requestPDF.p_num_riesgos, requestPDF.p_tip_emision);
                //ws.Dispose();
                //if (responsePDF.hataMesajlari != null && responsePDF.hataMesajlari.Length > 0)
                //{
                //    foreach (var item in responsePDF.hataMesajlari)
                //    {
                //        this.AddHata(item);
                //    }
                //    this.EndLog(responsePDF, true, typeof(mapfre.ZeyilWS));
                //}
                //if (responsePDF.reports == null && responsePDF.reports.Length == 0)
                //{
                //    this.AddHata("PDF dosyası alınamadı.");
                //}

                //#region Hata Kontrol Ve Kayıt
                //if (this.Hatalar.Count == 0)
                //{
                //    ITeklifPDFStorage pdfStorage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                //    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                //    string pdfUrl = pdfStorage.UploadFile("trafik", fileName, responsePDF.reports[0].report);
                //    this.GenelBilgiler.PDFDosyasi = pdfUrl;
                //}
                //#endregion
                #endregion              

                decimal THGFonu = (decimal)response.primDetayBilgileri.Where(w => w.primDetayKodu == 997).Sum(s => s.primDetayTutar);
                decimal GiderVergisi = (decimal)response.primDetayBilgileri.Where(w => w.primDetayKodu == 800).Sum(s => s.primDetayTutar);
                decimal GarantiFonu = (decimal)response.primDetayBilgileri.Where(w => w.primDetayKodu == 998 || w.primDetayKodu == 992).Sum(s => s.primDetayTutar);

                this.GenelBilgiler.ToplamVergi = THGFonu + GiderVergisi + GarantiFonu;
                this.GenelBilgiler.TaksitSayisi = response.taksitler == null ? (byte)0 : Convert.ToByte(response.taksitler.Length);
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;

                var gecikmeZammi = response.sahalarRisk.FirstOrDefault(f => f.p_cod_campo == "NUM_UYGMIS_GECIKME_SRP_YUZDE");
                if (gecikmeZammi != null && gecikmeZammi.p_val_campo != null)
                    this.GenelBilgiler.GecikmeZammiYuzdesi = decimal.Parse(gecikmeZammi.p_val_campo, CultureInfo.InvariantCulture);


                // Yeni Eklendi ****
                var hasarInd = response.sahalarRisk.FirstOrDefault(f => f.p_cod_campo == "NUM_UYG_GRKN_IND_YUZDE");
                if (hasarInd != null && hasarInd.p_val_campo != null)
                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = decimal.Parse(hasarInd.p_val_campo, CultureInfo.InvariantCulture);

                var hasarSurp = response.sahalarRisk.FirstOrDefault(f => f.p_cod_campo == "NUM_UYG_GRKN_SRP_YUZDE");
                if (hasarSurp != null && hasarSurp.p_val_campo != null)
                    this.GenelBilgiler.HasarSurprimYuzdesi = decimal.Parse(hasarSurp.p_val_campo, CultureInfo.InvariantCulture);

                //var hasarsizlik = response.sahalarRisk.FirstOrDefault(f => f.p_cod_campo == "VAL_KTT_HASAR_ORAN");
                //if (hasarsizlik != null && hasarsizlik.p_val_campo != null)
                //{
                //    decimal hasarYuzde = decimal.Parse(hasarsizlik.p_val_campo, CultureInfo.InvariantCulture);

                //    if(hasarYuzde < 0)
                //        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = hasarYuzde;
                //    else
                //        this.GenelBilgiler.HasarSurprimYuzdesi
                //}

                //var hasarsSurprim = response.zeyilSahalari.FirstOrDefault(f => f.shtSahaKodu == "00320");
                //if (hasarsSurprim != null)
                //    this.GenelBilgiler.HasarSurprimYuzdesi = decimal.Parse(hasarsSurprim.pshSahaDeger, CultureInfo.InvariantCulture);

                var tarifeBasamakKodu = response.sahalarRisk.FirstOrDefault(f => f.p_cod_campo == "COD_ARAC_TARIFE_GRUP_KODU");
                if (tarifeBasamakKodu != null && tarifeBasamakKodu.p_val_campo != null)
                    this.GenelBilgiler.TarifeBasamakKodu = Convert.ToInt16(tarifeBasamakKodu.p_val_campo);

                // ==== Güncellenecek. ==== //
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
                var maddiAracBasina = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4002);
                if (maddiAracBasina != null)
                    this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, (decimal)maddiAracBasina.p_capital, 0, 0, 0, 0);

                var maddiKazaBasina = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4003);
                if (maddiKazaBasina != null)
                    this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, (decimal)maddiKazaBasina.p_capital, 0, 0, 0, 0);

                var tedaviKisiBasina = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4009);
                if (tedaviKisiBasina != null)
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, (decimal)tedaviKisiBasina.p_capital, 0, 0, 0, 0);

                var tedaviKazaBasina = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4010);
                if (tedaviKazaBasina != null)
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, (decimal)tedaviKazaBasina.p_capital, 0, 0, 0, 0);

                var olumSakatlikKisiBasi = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4004);
                if (olumSakatlikKisiBasi != null)
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, (decimal)olumSakatlikKisiBasi.p_capital, 0, 0, 0, 0);

                var olumSakatlikKazaBasi = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4005);
                if (olumSakatlikKazaBasi != null)
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, (decimal)olumSakatlikKazaBasi.p_capital, 0, 0, 0, 0);

                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kaza_Basina, 0, 0, 0, 0, 0);

                var asistansTeminat = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4231);
                if (asistansTeminat != null)
                    this.AddTeminat(TrafikTeminatlar.Asistans, 0, 0, (decimal)asistansTeminat.p_capital, 0, 0);

                var ferdiKazaTedavi = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4008);
                if (ferdiKazaTedavi != null)
                    this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Tedavi, (decimal)ferdiKazaTedavi.p_capital, 0, (decimal)ferdiKazaTedavi.p_price, 0, 0);

                var trafik = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4001);
                if (trafik != null)
                    this.AddTeminat(TrafikTeminatlar.Trafik, 0, 0, (decimal)trafik.p_primium, 0, 0);
                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                else
                {
                    //foreach (var item in response.taksitler)
                    //{
                    //    this.AddOdemePlani(1, MAPFREHelper.ToDateTime(item.vadeTarihiLong), (decimal)item.tutarYTL, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                    //}
                }
                #endregion

                #region Web servis cevapları
                this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Police_No, response.polPoliceNo.ToString());
                this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Zeyil_Id, response.polZeyilNo.ToString());
                this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBaslama_Tarih, response.polBaslangicTarihi.ToString());
                this.AddWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBitis_Tarih, response.polBitisTarihi.ToString());
                #endregion
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                ws.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            mapfre.TanzimWSService ws = null;
            try
            {
                #region Veri Hazırlama
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);

                mapfre.ZeyilWS request = this.TeklifRequest(teklif, konfig);

                string mapPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Police_No, "0");
                string mapZeyilID = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Zeyil_Id, "0");
                string mapPolBaslamaTarih = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBaslama_Tarih, "0");
                string mapPolBitisTarih = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_PolBitis_Tarih, "0");

                request.polPoliceNo = mapPoliceNo;
                request.polZeyilNo = Convert.ToInt64(mapZeyilID);
                request.polBaslangicTarihi = Convert.ToInt64(mapPolBaslamaTarih);
                request.polBitisTarihi = Convert.ToInt64(mapPolBitisTarih);
                request.polSource = "P";
                request.tanzimTipi = "P";
                request.isBasim = true;

                request.sahalarPolice = null;
                request.sahalarRisk = null;
                request.teminatlar = null;
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    request.vpos = new mapfre.PaymentRequestWS();
                    request.vpos.kartHamili = odeme.KrediKarti.KartSahibi;
                    request.vpos.kartNo = odeme.KrediKarti.KartNo.Substring(0, 6);
                    request.vpos.cvc = "XXX";
                    request.vpos.expMonth = 99;
                    request.vpos.expYear = 9999;
                    request.vpos.taksitSayisi = (int)odeme.TaksitSayisi;
                }

                #endregion

                #region Servis call
                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                mapfre.ZeyilWS response = null;

                this.BeginLog(request, typeof(mapfre.ZeyilWS), WebServisIstekTipleri.Police);
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    //Kart Bilgileri log dosyasına eklenmiyor.
                    request.vpos.kartNo = odeme.KrediKarti.KartNo;
                    request.vpos.cvc = odeme.KrediKarti.CVC;
                    request.vpos.expMonth = Convert.ToInt32(odeme.KrediKarti.SKA);
                    request.vpos.expYear = Convert.ToInt32(odeme.KrediKarti.SKY);
                    request.vpos.taksitSayisi = (int)odeme.TaksitSayisi;
                }
                else
                {
                    request.vpos.kartHamili = "";
                    request.vpos.kartNo = "";
                    request.vpos.cvc = "";
                    request.vpos.expMonth = 0;
                    request.vpos.expYear = 0;
                    request.vpos.taksitSayisi = (int)odeme.TaksitSayisi;
                }

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.issueTekliftoPolice(request);
                ws.Dispose();
                this.EndLog(response, true, typeof(mapfre.ZeyilWS));

                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                }
                #endregion

                #region Hata Kontrol Ve kayıt
                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.polPoliceNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    if (response.reports != null && response.reports.Count(co => co.tip_report == "basim") > 0)
                    {
                        IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();

                        mapfre.ReportWS report = response.reports.FirstOrDefault(f => f.tip_report == "basim");
                        if (report != null)
                        {
                            string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());
                            string pdfUrl = pdfStorage.UploadFile("trafik", fileName, report.report);
                            this.GenelBilgiler.PDFPolice = pdfUrl;
                        }

                        var dekont = response.reports.FirstOrDefault(w => w.tip_report == "sp");
                        if (dekont != null && dekont.report.Length > 0)
                        {
                            string fileName = String.Format("mapfre_dekont_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                            string pdfUrl = pdfStorage.UploadFile("trafik", fileName, dekont.report);
                            this.GenelBilgiler.PDFGenelSartlari = pdfUrl;
                        }
                    }

                    TeklifWebServisCevap c = new TeklifWebServisCevap();
                    c.CevapKodu = Common.WebServisCevaplar.MAPFRE_Police_No;
                    c.CevapTipi = SoruCevapTipleri.Metin;
                    c.Cevap = response.polPoliceNo.ToString();
                    this.GenelBilgiler.TeklifWebServisCevaps.Add(c);

                    this.WebServisCevaplar.Add(c);
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                #endregion
            }
            catch (Exception ex)
            {
                #region hata log
                ws.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public void TeklifPDF()
        {
            mapfre.TanzimWSService ws = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] {tvmKodu, TeklifUretimMerkezleri.MAPFRE });

                #region Servis call
                MapfrePrintRequest request = new MapfrePrintRequest();
                request.userName = servisKullanici.KullaniciAdi;
                request.passWord = servisKullanici.Sifre;
                request.p_num_poliza = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Police_No, "0");
                request.p_num_spto = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Teklif_Zeyil_Id, decimal.Zero);
                request.p_num_apli = 0;
                request.p_num_spto_apli = 0;
                request.p_num_riesgos = 1;
                request.p_tip_emision = "C";

                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                mapfre.ZeyilWS response = null;

                this.BeginLog(request, typeof(MapfrePrintRequest), WebServisIstekTipleri.Police);

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.print(request.userName, request.passWord, request.p_num_poliza, request.p_num_spto, request.p_num_apli, request.p_num_spto_apli, request.p_num_riesgos, request.p_tip_emision);
                ws.Dispose();
                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                    this.EndLog(response, true, typeof(mapfre.ZeyilWS));
                }
                if (response.reports == null && response.reports.Length == 0)
                {
                    this.AddHata("PDF dosyası alınamadı.");
                }
                #endregion

                #region Hata Kontrol Ve Kayıt
                if (this.Hatalar.Count == 0)
                {
                    IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                    string pdfUrl = pdfStorage.UploadFile("trafik", fileName, response.reports[0].report);
                    this.GenelBilgiler.PDFDosyasi = pdfUrl;
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                ws.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
            finally
            {
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
            }
        }

        public override void PolicePDF()
        {
            mapfre.TanzimWSService ws = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

                #region Servis call
                MapfrePrintRequest request = new MapfrePrintRequest();
                request.userName = servisKullanici.KullaniciAdi;
                request.passWord = servisKullanici.Sifre;
                request.p_num_poliza = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Police_No, "0");
                request.p_num_spto = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Zeyil_Id, decimal.Zero);
                request.p_num_apli = 0;
                request.p_num_spto_apli = 0;
                request.p_num_riesgos = 1;
                request.p_tip_emision = "P";

                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                mapfre.ZeyilWS response = null;

                this.BeginLog(request, typeof(MapfrePrintRequest), WebServisIstekTipleri.Police);

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.print(request.userName, request.passWord, request.p_num_poliza, request.p_num_spto, request.p_num_apli, request.p_num_spto_apli, request.p_num_riesgos, request.p_tip_emision);
                ws.Dispose();
                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                    this.EndLog(response, true, typeof(mapfre.ZeyilWS));
                }

                if (response.reports == null && response.reports.Length == 0)
                {
                    this.AddHata("PDF dosyası alınamadı.");
                }
                #endregion

                #region Hata Kontrol Ve Kayıt
                if (this.Hatalar.Count == 0)
                {
                    IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                    string pdfUrl = pdfStorage.UploadFile("trafik", fileName, response.reports[0].report);
                    this.GenelBilgiler.PDFPolice = pdfUrl;

                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                ws.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public void BilgilendirmePDF()
        {
            mapfre.TanzimWSService ws = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

                #region Servis call
                MapfrePrintRequest request = new MapfrePrintRequest();
                request.userName = servisKullanici.KullaniciAdi;
                request.passWord = servisKullanici.Sifre;
                request.p_num_poliza = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Police_No, "0");
                request.p_num_spto = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Zeyil_Id, decimal.Zero);
                request.p_num_apli = 0;
                request.p_num_spto_apli = 0;
                request.p_num_riesgos = 1;
                request.p_tip_emision = "P";

                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                mapfre.ZeyilWS response = null;

                this.BeginLog(request, typeof(MapfrePrintRequest), WebServisIstekTipleri.Police);

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.printBilgi(request.userName, request.passWord, request.p_num_poliza, request.p_num_spto, request.p_num_apli, request.p_num_spto_apli, request.p_num_riesgos, request.p_tip_emision, "tr");
                ws.Dispose();
                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                    this.EndLog(response, true, typeof(mapfre.ZeyilWS));
                }
                if (response.reports == null && response.reports.Length == 0)
                {
                    this.AddHata("PDF dosyası alınamadı.");
                }
                #endregion

                #region Hata Kontrol ve Kayıt
                if (this.Hatalar.Count == 0)
                {
                    IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                    string pdfUrl = pdfStorage.UploadFile("trafik", fileName, response.reports[0].report);
                    this.GenelBilgiler.PDFBilgilendirme = pdfUrl;

                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                }
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log
                ws.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void DekontPDF()
        {
            mapfre.TanzimWSService ws = null;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFRETrafik);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(this.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

                #region Servis call
                MapfrePrintRequest request = new MapfrePrintRequest();
                request.userName = servisKullanici.KullaniciAdi;
                request.passWord = servisKullanici.Sifre;
                request.p_num_poliza = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Police_No, "0");
                request.p_num_spto = this.ReadWebServisCevap(Common.WebServisCevaplar.MAPFRE_Zeyil_Id, decimal.Zero);
                request.p_num_apli = 0;
                request.p_num_spto_apli = 0;
                request.p_num_riesgos = 1;
                request.p_tip_emision = "P";

                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
                mapfre.ZeyilWS response = null;

                this.BeginLog(request, typeof(MapfrePrintRequest), WebServisIstekTipleri.DekontPDF);

                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                response = ws.simulateSpDekont(request.userName,
                               request.passWord,
                               request.p_num_poliza,
                               request.p_num_spto,
                               request.p_num_apli,
                               request.p_num_spto_apli,
                               "0I4eod77GFrsrHNPUeF4wA==");
                ws.Dispose();
                this.EndLog(response, true, typeof(mapfre.ZeyilWS));
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                if (response.hataMesajlari != null && response.hataMesajlari.Length > 0)
                {
                    foreach (var item in response.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                }
                if (response.reports == null && response.reports.Length == 0)
                {
                    this.AddHata("PDF dosyası alınamadı.");
                }
                #endregion

                #region Hata Kontrol ve Kayıt
                if (this.Hatalar.Count == 0)
                {
                    IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                    var dekont = response.reports.FirstOrDefault(w => w.tip_report == "sp");
                    if (dekont != null && dekont.report.Length > 0)
                    {
                        string fileName = String.Format("mapfre_dekont_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string pdfUrl = pdfStorage.UploadFile("trafik", fileName, dekont.report);
                        //this.GenelBilgiler.PDFGenelSartlari = pdfUrl;
                        this.GenelBilgiler.PDFDekont = pdfUrl;
                    }

                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ws.Abort();
                _Log.Error("MapfreProjeTrafik.DekontPDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        private mapfre.ZeyilWS TeklifRequest(ITeklif teklif, KonfigTable konfig)
        {
            #region Genel Bilgiler
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

            mapfre.ZeyilWS request = new mapfre.ZeyilWS();
            request.userName = servisKullanici.KullaniciAdi;
            request.passWord = servisKullanici.Sifre;
            request.brbBransKodu = "410";

            DateTime policeBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            request.polBaslangicTarihi = MAPFREHelper.ToMAPFREDateLong(policeBaslangic);
            request.polBitisTarihi = MAPFREHelper.ToMAPFREDateLong(policeBaslangic.AddYears(1));
            request.policeTanzimTarihi = MAPFREHelper.ToMAPFREDateLong(policeBaslangic);
            request.polSource = "C";
            request.tanzimTipi = "C";

            request.tramerBilgileri = new mapfre.TramerBilgileriWS();
            request.tramerBilgileri.islem = "Surprim";
            request.tramerBilgileri.zkymsVar = "0";

            mapfre.SirketAnahtariWS tramer = new mapfre.SirketAnahtariWS();

            bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
            if (eskiPoliceVar)
            {
                request.tramerBilgileri.islem = "TramerPolice";
                string tramerIslemTipi = teklif.ReadSoru(TrafikSorular.TramerIslemTipi, String.Empty);
                if (tramerIslemTipi == "11")
                    request.tramerBilgileri.islem = "TramerReferansPolice";
                tramer.sirketNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                tramer.acenteNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Acente_No, String.Empty);
                tramer.policeNo = teklif.ReadSoru(KaskoSorular.Eski_Police_No, String.Empty);
                tramer.yenilemeNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Yenileme_No, String.Empty);
            }
            bool tasiyiciSorumluluk = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_VarYok, false);
            if (tasiyiciSorumluluk)
            {
                request.tramerBilgileri.zkymsVar = "1";
                mapfre.SirketAnahtariWS zkytms = new mapfre.SirketAnahtariWS();
                zkytms.sirketNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
                zkytms.acenteNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                zkytms.policeNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                zkytms.yenilemeNo = teklif.ReadSoru(KaskoSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);
                request.tramerBilgileri.zkytms = zkytms;
            }
            request.tramerBilgileri.tramer = tramer;

            decimal cod_agt = 0;
            decimal.TryParse(servisKullanici.PartajNo_, out cod_agt);
            request.genelBilgiler = new mapfre.GenelBilgilerWS();
            request.genelBilgiler.p_cod_agt = cod_agt;
            request.genelBilgiler.p_cod_gestor = cod_agt.ToString();
            request.genelBilgiler.p_cod_mon = MapfreParaBirimi.TL;
            request.genelBilgiler.p_cod_fracc_pago = 10;
            request.genelBilgiler.p_cod_ramo = 410;
            #endregion

            #region Sigortali
            List<mapfre.SahisWS> sahislar = new List<mapfre.SahisWS>();
            MusteriGenelBilgiler sigortali = new MusteriGenelBilgiler();
            TeklifSigortali teklifSigortali = teklif.Sigortalilar.FirstOrDefault();
            string sigortaliIl = String.Empty;
            int sigortaliIlce = 0;
            string sigortaliTuru = String.Empty;
            string mapfreSigortaliUyruk = "0";

            if (teklifSigortali != null)
            {
                mapfre.SahisWS s1 = new mapfre.SahisWS();
                s1.p_tip_benef = MapfreSahisTipi.Sigortali;

                sigortali = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);

                if (sigortali.Uyruk == UyrukTipleri.TC)
                    mapfreSigortaliUyruk = MapfreUyrukTipleri.Turk;
                else
                    mapfreSigortaliUyruk = MapfreUyrukTipleri.Yabanci;

                if (MusteriTipleri.Ozel(sigortali.MusteriTipKodu))
                {
                    request.genelBilgiler.p_cod_docum = sigortali.KimlikNo;
                    s1.p_cod_docum = sigortali.KimlikNo;
                    sigortaliTuru = "O";

                    if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                    {
                        request.genelBilgiler.p_tip_docum = MapfreKimlikTipleri.TCKimlikNo;
                        s1.p_tip_docum = MapfreKimlikTipleri.TCKimlikNo;
                    }
                    else if (sigortali.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                    {
                        request.genelBilgiler.p_tip_docum = MapfreKimlikTipleri.YabanciKimlikNo;
                        s1.p_tip_docum = MapfreKimlikTipleri.YabanciKimlikNo;
                    }
                }
                else
                {
                    request.genelBilgiler.p_cod_docum = sigortali.KimlikNo;
                    request.genelBilgiler.p_tip_docum = MapfreKimlikTipleri.VergiNo;

                    s1.p_tip_docum = MapfreKimlikTipleri.VergiNo;
                    s1.p_cod_docum = sigortali.KimlikNo;
                    sigortaliTuru = "T";
                }

                MusteriAdre sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                if (sigortaliAdres != null)
                {
                    s1.p_address = sigortaliAdres.Adres;

                    int sIlKodu = Convert.ToInt32(sigortaliAdres.IlKodu);
                    if (sIlKodu < 10)
                    {
                        sigortaliIl = sigortaliAdres.IlKodu.Substring(1, 1);
                    }
                    else
                    {
                        sigortaliIl = sigortaliAdres.IlKodu;
                    }
                    sigortaliIlce = sigortaliAdres.IlceKodu.HasValue ? sigortaliAdres.IlceKodu.Value : 0;
                }

                MusteriTelefon sigortaliTelefon = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Ev || s.IletisimNumaraTipi==IletisimNumaraTipleri.Is).FirstOrDefault();
                if (sigortaliTelefon == null)
                {
                    sigortaliTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                }
                if (sigortaliTelefon != null)
                {
                    if (!String.IsNullOrEmpty(sigortaliTelefon.Numara) && sigortaliTelefon.Numara.Trim() != "90" && sigortaliTelefon.Numara.Length >= 10)
                    {
                        string telefon = sigortaliTelefon.Numara;
                        if (telefon.Length == 14)
                        {
                            telefon = sigortaliTelefon.Numara.Substring(3, 3);
                            telefon += sigortaliTelefon.Numara.Substring(7, 7);
                        }
                        telefon = telefon.Replace("-", " ");
                        s1.p_telefon = telefon;
                    }
                }
                sahislar.Add(s1);
            }
            request.sahislar = sahislar.ToArray();
            #endregion

            #region Araç Bilgileri
            string mapfreSigortaliIlKodu = String.Empty;
            string mapfreSigortaliIlceKodu = String.Empty;
            string kullanimTarzi = String.Empty;
            string tarifeGrupKodu = String.Empty;
            string aracKullanimSekli = String.Empty;

            //Sigortali il ve ilçe bilgileri alınıyor.
            if (!String.IsNullOrEmpty(sigortaliIl) && sigortaliIlce > 0)
            {
                CR_IlIlce ilIlce = _CRContext.CR_IlIlceRepository.Filter(s => s.IlKodu == sigortaliIl &&
                                                                         s.IlceKodu == sigortaliIlce &&
                                                                         s.TUMKodu == TeklifUretimMerkezleri.MAPFRE).FirstOrDefault();
                if (ilIlce != null)
                {
                    mapfreSigortaliIlKodu = ilIlce.CRIlKodu;
                    mapfreSigortaliIlceKodu = ilIlce.CRIlceKodu;
                }
            }

            string[] parts = teklif.Arac.KullanimTarzi.Split('-');
            if (parts.Length == 2)
            {
                string kullnimT = parts[0];
                string kod2 = parts[1];

                CR_AracGrup grup = _CRContext.CR_AracGrupRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE &&
                                                                                  f.KullanimTarziKodu == kullnimT &&
                                                                                  f.Kod2 == kod2).SingleOrDefault<CR_AracGrup>();
                if (grup != null)
                {
                    tarifeGrupKodu = grup.TarifeKodu;
                }

                CR_KullanimTarzi kTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE &&
                                                                                               f.KullanimTarziKodu == kullnimT &&
                                                                                               f.Kod2 == kod2).SingleOrDefault<CR_KullanimTarzi>();
                if (kTarzi != null)
                {
                    kullanimTarzi = kTarzi.TarifeKodu;
                }
            }

            if (teklif.Arac.KullanimSekli == "0")
                aracKullanimSekli = "1";
            else if (teklif.Arac.KullanimSekli == "1")
                aracKullanimSekli = "2";
            else
                aracKullanimSekli = "0";

            bool asistansVar = teklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);
            //bool belediyeHalkOtobus = teklif.ReadSoru(TrafikSorular.Teminat_BelediyeHalkOtobusu, false);

            List<mapfre.SahaWS> sahalarRisk = new List<mapfre.SahaWS>();
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SIGORTALI_ULKE", p_val_campo = "792" });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SIGORTALI_IL", p_val_campo = mapfreSigortaliIlKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SIGORTALI_ILCE", p_val_campo = mapfreSigortaliIlceKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SIGORTALI_UYRUK", p_val_campo = mapfreSigortaliUyruk });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_PLAKA_IL_KODU", p_val_campo = "0" + teklif.Arac.PlakaKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_PLAKA_NO", p_val_campo = teklif.Arac.PlakaNo });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ARAC_RUHSAT_SERI", p_val_campo = teklif.Arac.TescilSeriKod });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ARAC_RUHSAT_SERI_NO", p_val_campo = teklif.Arac.TescilSeriNo });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ASBIS_REFERANS_NO", p_val_campo = teklif.Arac.AsbisNo });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ARAC_TARIFE_GRUP_KODU", p_val_campo = tarifeGrupKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_KULLANIM_TARZI", p_val_campo = kullanimTarzi });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_MARKA", p_val_campo = teklif.Arac.Marka });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_MARKA_TIPI", p_val_campo = teklif.Arac.AracinTipi });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_SASI_NO", p_val_campo = teklif.Arac.SasiNo });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_MOTOR_NO", p_val_campo = teklif.Arac.MotorNo });
            if (teklif.Arac.Model.HasValue)
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "NUM_MODEL_YILI", p_val_campo = teklif.Arac.Model.Value.ToString() });
            if (teklif.Arac.KoltukSayisi.HasValue)
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "NUM_YER_ADEDI", p_val_campo = teklif.Arac.KoltukSayisi.Value.ToString() });

            else
            {
                AracTip arac = _AracContext.AracTipRepository.Find(s => s.MarkaKodu == teklif.Arac.Marka && s.TipKodu == teklif.Arac.AracinTipi);
                if (arac != null)
                    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "NUM_YER_ADEDI", p_val_campo = arac.KisiSayisi.Value.ToString() });
            }
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_KULLANIM_SEKLI", p_val_campo = aracKullanimSekli });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "FEC_TESCIL_TARIHI", p_val_campo = teklif.Arac.TrafikTescilTarihi.Value.ToString("ddMMyyyy") });
            if (asistansVar)
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_ASISTANS_VERILSIN", p_val_campo = "E" });
            else
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_ASISTANS_VERILSIN", p_val_campo = "H" });

            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_DURAK_TAKSISI", p_val_campo = "H" });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_TASIMA_YETKI_BELGESI", p_val_campo = "H" });

            // 85 - OTOBÜS (Sür.Dah. 31+ Kol.) kullanım tarzı için ekrandaki bilgi. 
            // diğer kullanım tarzları için her zaman H
            //if (kullanimTarzi == "85")
            //{
            //    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_AYAKTA_YOLCU", p_val_campo = belediyeHalkOtobus ? "E" : "H" });
            //}

            if (request.tramerBilgileri.islem == "TramerReferansPolice")
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ISLEM_TIPI", p_val_campo = "11" });
            }
            else if (request.tramerBilgileri.islem == "TramerPolice")
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ISLEM_TIPI", p_val_campo = "1" });
            }
            else
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ISLEM_TIPI", p_val_campo = "0" });
            }

            MusteriTelefon sigortaliCepTel = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
            if (sigortaliCepTel == null)
            {
                sigortaliCepTel = sigortali.MusteriTelefons.FirstOrDefault();
            }
            if (sigortaliCepTel != null)
            {
                if (!String.IsNullOrEmpty(sigortaliCepTel.Numara) && sigortaliCepTel.Numara.Trim() != "90" && sigortaliCepTel.Numara.Length >= 10)
                {
                    string telefon = sigortaliCepTel.Numara;
                    if (telefon.Length == 14)
                    {
                        telefon = sigortaliCepTel.Numara.Substring(3, 3);
                        telefon += sigortaliCepTel.Numara.Substring(7, 7);
                    }
                    telefon = telefon.Replace("-", " ");

                    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_SIGORTALI_CEP_TEL", p_val_campo = telefon });
                }
            }
            else
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_SIGORTALI_CEP_TEL", p_val_campo = "5555555555" });
            }


            request.sahalarRisk = sahalarRisk.ToArray();
            #endregion

            request.bilgiMesajlari = new string[1];
            request.bilgiMesajlari[0] = "<![CDATA[<ApiCode>0I4eod77GFrsrHNPUeF4wA==</ApiCode>]]>";

            return request;
        }
    }
}
