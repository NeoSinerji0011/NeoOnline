using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MAPFREKasko : Teklif, IMAPFREKasko
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
        public MAPFREKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
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
                return TeklifUretimMerkezleri.MAPFRE;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            mapfre.ZeyilWS response = null;
            mapfre.TanzimWSService ws = null;
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

            try
            {
                #region Veri Hazırlama
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleMAPFREKasko);
                mapfre.ZeyilWS request = this.TeklifRequest(teklif, konfig);
                #endregion

                #region Servis call
                string serviceURL = konfig[Konfig.MAPFRE_ServiceURL];
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

                #region Basarılı kontrolu
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
                this.GenelBilgiler.BitisTarihi = MAPFREHelper.ToDateTime(response.polBitisTarihi);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = response.primBilgileriWS.burutPrim;
                this.GenelBilgiler.NetPrim = response.primBilgileriWS.netPrim;
                this.GenelBilgiler.ToplamKomisyon = response.primBilgileriWS.komisyon;
                this.GenelBilgiler.TUMTeklifNo = response.polPoliceNo.ToString();
                #region TEKLİF PDF
                MapfrePrintRequest requestPDF = new MapfrePrintRequest();
                requestPDF.userName = servisKullanici.KullaniciAdi;
                requestPDF.passWord = servisKullanici.Sifre;
                requestPDF.p_num_poliza = response.polPoliceNo;
                requestPDF.p_num_spto = response.polZeyilNo.Value;
                requestPDF.p_num_apli = 0;
                requestPDF.p_num_spto_apli = 0;
                requestPDF.p_num_riesgos = 1;
                requestPDF.p_tip_emision = "C";

                this.BeginLog(requestPDF, typeof(MapfrePrintRequest), WebServisIstekTipleri.Police);
                mapfre.ZeyilWS responsePDF = null;
                ws = new mapfre.TanzimWSService();
                ws.Url = serviceURL;
                responsePDF = ws.print(requestPDF.userName, requestPDF.passWord, requestPDF.p_num_poliza, requestPDF.p_num_spto, requestPDF.p_num_apli, requestPDF.p_num_spto_apli, requestPDF.p_num_riesgos, requestPDF.p_tip_emision);
                ws.Dispose();
                if (responsePDF.hataMesajlari != null && responsePDF.hataMesajlari.Length > 0)
                {
                    foreach (var item in responsePDF.hataMesajlari)
                    {
                        this.AddHata(item);
                    }
                    this.EndLog(responsePDF, true, typeof(mapfre.ZeyilWS));
                }

                #region Hata Kontrol Ve Kayıt
                if (this.Hatalar.Count == 0 && responsePDF.reports != null)
                {
                    ITeklifPDFStorage pdfStorage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                    string pdfUrl = pdfStorage.UploadFile("kasko", fileName, responsePDF.reports[0].report);
                    this.GenelBilgiler.PDFDosyasi = pdfUrl;
                }
                #endregion
                #endregion

                decimal? GiderVergisi = response.primDetayBilgileri.Where(w => w.primDetayKodu == 800).Sum(s => s.primDetayTutar);
                this.GenelBilgiler.ToplamVergi = GiderVergisi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;

                // ==== Güncellenecek ==== //
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                var hasarKademe = response.sahalarRisk.FirstOrDefault(f => f.p_cod_campo == "NUM_UYGULANACAK_HASARKADEMENO");
                if (hasarKademe != null)
                {
                    int kademe = 0;
                    if (int.TryParse(hasarKademe.p_val_campo, out kademe))
                    {
                        teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi = kademe;
                    }
                }
                #endregion

                #region Vergiler
                this.AddVergi(KaskoVergiler.GiderVergisi, GiderVergisi.Value);
                #endregion

                #region Teminatlar
                // ==== 420	 A	 KASKO KASKO   	 ARÇ ==== //
                var kasko = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4201);
                if (kasko != null)
                    this.AddTeminat(KaskoTeminatlar.Kasko, kasko.p_capital.Value, 0, 0, kasko.p_primium.Value, 0);

                // ==== 420	 E	 ARAÇ ASİSTANS  	 ASSISTK ==== //
                var assistk = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4231);
                if (assistk != null)
                    this.AddTeminat(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, assistk.p_price.Value, 0, 0, assistk.p_primium.Value, 0);

                // ==== 420	 A	 KISMİ KASKO     KASKO2 	 ARÇ ==== //
                var kismikasko = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4202);
                if (kismikasko != null && kismikasko.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.Kasko_Kismi, kismikasko.p_capital.Value, 0, 0, 0, 0);

                // ==== 420	 E	 RADYO-TEYP     RT     ARÇ==== //
                var radyoteyp = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4203);
                if (radyoteyp != null && radyoteyp.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.Radyo_Teyp, radyoteyp.p_capital.Value, 0, 0, 0, 0);

                // ==== 420	 E	 TAŞINAN YÜK    	 TAŞYÜK 	 ARÇ==== //
                var tasinanyuk = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4204);
                if (tasinanyuk != null && tasinanyuk.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.TasinanYuk, tasinanyuk.p_capital.Value, 0, 0, 0, 0);

                // ====420	 E	 ARAÇ ÇALINMASI 	 A.ÇALIN	 ARÇ==== //
                var araccalinmasa = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4220);
                if (araccalinmasa != null && araccalinmasa.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.Arac_Calinmasi, araccalinmasa.p_capital.Value, 0, 0, araccalinmasa.p_primium.Value, 0);

                // ====420	 E	 MİNİ ONARIM    	 MONARIM	 ARÇ==== //
                var minionarim = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4253);
                if (minionarim != null && minionarim.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.Mini_Onrarim_Hizmeti, minionarim.p_capital.Value, 0, 0, 0, 0);

                // ==== 420	 A	 ARTAN MALİ MES 	 AMS     ARÇ ==== //
                var artanmalimesuliyet = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4205);
                if (artanmalimesuliyet != null && artanmalimesuliyet.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, artanmalimesuliyet.p_price.Value, 0, 0, artanmalimesuliyet.p_primium.Value, 0);

                // ==== 420	 A	    KAZA BAŞINA 	 AMS-KB 	 ARÇ ==== //
                var kazabasina = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4206);
                if (kazabasina != null && kazabasina.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, kazabasina.p_price.Value, 0, 0, kazabasina.p_primium.Value, 0);

                // ==== 420	 A	    ŞAHIS BAŞINA	 AMS-ŞB 	 ARÇ ==== //
                var sahisbasina = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4207);
                if (sahisbasina != null && sahisbasina.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, sahisbasina.p_price.Value, 0, 0, sahisbasina.p_primium.Value, 0);

                // ==== 420	 A	    MADDİ HASAR 	 AMS-MH 	 ARÇ ==== //
                var maddihasar = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4208);
                if (maddihasar != null && maddihasar.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, maddihasar.p_price.Value, 0, 0, maddihasar.p_primium.Value, 0);

                // ==== 420	 E	 ŞÖFÖR YOLCU    	 ŞK     ARÇ ==== //
                var soforyolcu = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4210);
                if (soforyolcu != null)
                    this.AddTeminat(KaskoTeminatlar.Koltuk_Ferdi_Kaza_Surucu_Yolcu, soforyolcu.p_price.Value, 0, 0, soforyolcu.p_primium.Value, 0);

                // ==== 420	 E	 ÖLÜM    	 ŞK-ÖLÜM	 ARÇ ==== //
                var olum = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4211);
                if (olum != null && olum.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.KFK_Olum, olum.p_price.Value, 0, 0, olum.p_primium.Value, 0);

                // ==== 420	 E	 SAKATLIK	 ŞK-SS  	 ARÇ ==== //
                var sakatlik = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4212);
                if (sakatlik != null && sakatlik.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, sakatlik.p_price.Value, 0, 0, sakatlik.p_primium.Value, 0);

                // ==== 420	 E	  TEDAVİ  	 ŞK-TED 	 ARÇ ==== //
                var tedavi = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4213);
                if (tedavi != null && tedavi.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, tedavi.p_price.Value, 0, 0, tedavi.p_primium.Value, 0);

                // ====420	 E	 DEPREM         	 DEPR420	 ARÇ==== //
                var deprem = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4214);
                if (deprem != null && deprem.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.Deprem, deprem.p_price.Value, 0, 0, deprem.p_primium.Value, 0);

                // ====420	 A	 HUKUKSAL KORUMA	 HUK.KOR ==== //
                var hukuksalkoruma = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4250);
                if (hukuksalkoruma != null && hukuksalkoruma.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.Hukuksal_Koruma, hukuksalkoruma.p_price.Value, 0, 0, 0, 0);

                // ====420	 A	 HK-M.ARACA BĞL.	 HK-MAB  ==== //
                var hkaracabagli = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4251);
                if (hkaracabagli != null && hkaracabagli.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, hkaracabagli.p_price.Value, 0, 0, hkaracabagli.p_primium.Value, 0);

                // ====420	 A	 HK-SÜRÜCÜYE BĞL	 HK-SB   ==== //
                var hksurucuyebagli = response.teminatlar.FirstOrDefault(f => f.p_cod_cob == 4252);
                if (hksurucuyebagli != null && hksurucuyebagli.p_mca_seleccion == "S")
                    this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, hksurucuyebagli.p_price.Value, 0, 0, hksurucuyebagli.p_primium.Value, 0);

                #endregion

                #region Ödeme Planı
                if (response.taksitler == null || response.taksitler.Length == 0)
                {
                    this.GenelBilgiler.TaksitSayisi = 1;
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                else
                {
                    this.GenelBilgiler.TaksitSayisi = (byte)response.taksitler.Length;
                    int index = 0;
                    foreach (mapfre.TaksitWS taksit in response.taksitler)
                    {
                        this.AddOdemePlani(index, taksit.p_fec_efec_recibo.Value, taksit.p_imp_recibo.Value, OdemeTipleri.KrediKarti);
                        index++;
                    }
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
            mapfre.ZeyilWS response = null;
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

                request.sahalarPolice = null;
                request.sahalarRisk = null;
                request.teminatlar = null;

                if (odeme.KrediKarti != null &&( odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti))
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

                this.BeginLog(request, typeof(mapfre.ZeyilWS), WebServisIstekTipleri.Police);

                //Kart Bilgileri log dosyasına eklenmiyor.
                if (odeme.KrediKarti != null && (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti))
                {
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

                #region Hata Kontrol ve Kayıt
                if (this.Hatalar.Count == 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.polPoliceNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    //Muhasebe aktarımı
                    //this.SendMuhasebe();

                    TeklifWebServisCevap c = new TeklifWebServisCevap();
                    c.CevapKodu = Common.WebServisCevaplar.MAPFRE_Police_No;
                    c.CevapTipi = SoruCevapTipleri.Metin;
                    c.Cevap = response.polPoliceNo.ToString();
                    this.GenelBilgiler.TeklifWebServisCevaps.Add(c);
                    this.WebServisCevaplar.Add(c);

                    c = new TeklifWebServisCevap();
                    c.CevapKodu = Common.WebServisCevaplar.MAPFRE_Zeyil_Id;
                    c.CevapTipi = SoruCevapTipleri.Metin;
                    c.Cevap = response.polZeyilNo.ToString();
                    this.GenelBilgiler.TeklifWebServisCevaps.Add(c);
                    this.WebServisCevaplar.Add(c);


                    if (response.reports != null && response.reports.Length > 0)
                    {
                        IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        //var police = response.reports.FirstOrDefault(w => w.tip_report == "basim");
                        //if (police != null && police.report.Length > 0)
                        //{
                        //    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        //    string pdfUrl = pdfStorage.UploadFile("kasko", fileName, police.report);
                        //    this.GenelBilgiler.PDFPolice = pdfUrl;
                        //}

                        var dekont = response.reports.FirstOrDefault(w => w.tip_report == "sp");
                        if (dekont != null && dekont.report.Length > 0)
                        {
                            string fileName = String.Format("mapfre_dekont_krsltml_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                            string pdfUrl = pdfStorage.UploadFile("kasko", fileName, dekont.report);
                            this.GenelBilgiler.PDFGenelSartlari = pdfUrl;
                        }
                    }
                }

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

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

        public string TeklifPDF()
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
                if (response.reports == null || response.reports.Length == 0)
                {
                    this.AddHata("PDF dosyası alınamadı.");
                }
                #endregion

                #region Hata Kontrol Ve Kayıt
                if (this.Hatalar.Count == 0)
                {
                    ITeklifPDFStorage pdfStorage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                    string pdfUrl = pdfStorage.UploadFile("kasko", fileName, response.reports[0].report);
                    this.GenelBilgiler.PDFDosyasi = pdfUrl;
                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

                    return pdfUrl;
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

            return String.Empty;
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

                #region Hata Kontrol ve Kayıt
                if (this.Hatalar.Count == 0)
                {
                    IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                    string pdfUrl = pdfStorage.UploadFile("kasko", fileName, response.reports[0].report);
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

                    string pdfUrl = pdfStorage.UploadFile("mapfrekasko", fileName, response.reports[0].report);
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
                response = ws.printBilgi(request.userName, request.passWord, request.p_num_poliza, request.p_num_spto, request.p_num_apli, request.p_num_spto_apli, request.p_num_riesgos, request.p_tip_emision, "tr");
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
                    string fileName = String.Format("mapfre_{0}.pdf", System.Guid.NewGuid().ToString());

                    string pdfUrl = pdfStorage.UploadFile("kasko", fileName, response.reports[0].report);
                    //this.GenelBilgiler.PDFGenelSartlari = pdfUrl;
                    this.GenelBilgiler.PDFDekont = pdfUrl;

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

        private mapfre.ZeyilWS TeklifRequest(ITeklif teklif, KonfigTable konfig)
        {
            #region Genel Bilgiler
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.MAPFRE });

            mapfre.ZeyilWS request = new mapfre.ZeyilWS();
            request.userName = servisKullanici.KullaniciAdi;
            request.passWord = servisKullanici.Sifre;
            request.brbBransKodu = "420";

            DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            request.polBaslangicTarihi = MAPFREHelper.ToMAPFREDateLong(policeBaslangic);
            request.polBitisTarihi = MAPFREHelper.ToMAPFREDateLong(policeBaslangic.AddYears(1));
            request.policeTanzimTarihi = MAPFREHelper.ToMAPFREDateLong(policeBaslangic);
            request.polSource = "C";
            request.tanzimTipi = "C";

            request.tramerBilgileri = new mapfre.TramerBilgileriWS();
            request.tramerBilgileri.islem = "TramerPolice";
            request.tramerBilgileri.zkymsVar = "0";
            mapfre.SirketAnahtariWS tramer = new mapfre.SirketAnahtariWS();

            bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
            if (eskiPoliceVar)
            {
                request.tramerBilgileri.islem = "TramerPolice";
                tramer.sirketNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                tramer.acenteNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Acente_No, String.Empty);
                tramer.policeNo = teklif.ReadSoru(KaskoSorular.Eski_Police_No, String.Empty);
                tramer.yenilemeNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Yenileme_No, String.Empty);
            }
            else
            {
                request.tramerBilgileri.islem = "Surprim";
            }

            request.tramerBilgileri.tramer = tramer;
            request.tramerBilgileri.zkytms = new mapfre.SirketAnahtariWS();


            decimal cod_agt = 0;
            decimal.TryParse(servisKullanici.PartajNo_, out cod_agt);
            request.genelBilgiler = new mapfre.GenelBilgilerWS();
            request.genelBilgiler.p_cod_agt = cod_agt;
            request.genelBilgiler.p_cod_gestor = cod_agt.ToString();
            request.genelBilgiler.p_cod_mon = MapfreParaBirimi.TL;
            int taksitSayisi = (byte)(teklif.GenelBilgiler.TaksitSayisi ?? 1);
            request.genelBilgiler.p_cod_fracc_pago = Business.Common.MapfeOdemeKodlari.OdemeKodu(taksitSayisi);
            request.genelBilgiler.p_cod_ramo = 420;
            #endregion

            #region Sigortali
            List<mapfre.SahisWS> sahislar = new List<mapfre.SahisWS>();

            TeklifSigortali teklifSigortali = teklif.Sigortalilar.FirstOrDefault();
            string sigortaliIl = String.Empty;
            int sigortaliIlce = 0;
            string sigortaliTuru = String.Empty;
            string mapfreSigortaliUyruk = "0";

            if (teklifSigortali != null)
            {
                mapfre.SahisWS s1 = new mapfre.SahisWS();
                s1.p_tip_benef = MapfreSahisTipi.Sigortali;

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklifSigortali.MusteriKodu);

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
                    sigortaliIl = sigortaliAdres.IlKodu;
                    sigortaliIlce = sigortaliAdres.IlceKodu.HasValue ? sigortaliAdres.IlceKodu.Value : 0;
                }

                MusteriTelefon sigortaliTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                if (sigortaliTelefon != null)
                {
                    if (!String.IsNullOrEmpty(sigortaliTelefon.Numara) && sigortaliTelefon.Numara.Trim() != "90" && sigortaliTelefon.Numara.Length >= 10)
                    {
                        string telefon = sigortaliTelefon.Numara;
                        if (telefon.Length == 14)
                        {
                            telefon = telefon.Substring(3, 11);
                        }
                        telefon = telefon.Replace("-", " ");
                        s1.p_telefon = telefon;
                    }
                }
                sahislar.Add(s1);

            }

            #region Dain-i Mürtehin
            bool dainiMurteinVar = teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
            string dainiMurteinKimlikNo = teklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, String.Empty);
            if (dainiMurteinVar && !String.IsNullOrEmpty(dainiMurteinKimlikNo))
            {
                mapfre.SahisWS s2 = new mapfre.SahisWS();
                s2.p_tip_benef = MapfreSahisTipi.DainiMurtein;
                s2.p_tip_docum = "VRG";
                s2.p_cod_docum = dainiMurteinKimlikNo;
                s2.p_address = "";
                sahislar.Add(s2);
            }
            #endregion

            request.sahislar = sahislar.ToArray();
            #endregion

            #region Araç Bilgileri
            string mapfreTescilIlKodu = String.Empty;
            string mapfreTescilIlceKodu = String.Empty;
            string mapfreSigortaliIlKodu = String.Empty;
            string mapfreSigortaliIlceKodu = String.Empty;
            string tarifeGrupKodu = String.Empty;
            string aracKullanimSekli = String.Empty;

            if (!String.IsNullOrEmpty(teklif.Arac.TescilIlKodu) && !String.IsNullOrEmpty(teklif.Arac.TescilIlceKodu))
            {
                int tescilIlceKodu = int.Parse(teklif.Arac.TescilIlceKodu);
                CR_IlIlce ilIlce = _CRContext.CR_IlIlceRepository.Filter(s => s.IlKodu == teklif.Arac.TescilIlKodu &&
                                                                         s.IlceKodu == tescilIlceKodu &&
                                                                         s.TUMKodu == TeklifUretimMerkezleri.MAPFRE).FirstOrDefault();
                if (ilIlce != null)
                {
                    mapfreTescilIlKodu = ilIlce.CRIlKodu;
                    mapfreTescilIlceKodu = ilIlce.CRIlceKodu;
                }
            }

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

            string kullanimT = String.Empty;
            string kod2 = String.Empty;
            string[] parts = teklif.Arac.KullanimTarzi.Split('-');
            if (parts.Length == 2)
            {
                kullanimT = parts[0];
                kod2 = parts[1];
                CR_KullanimTarzi kTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.MAPFRE &&
                                                                                            f.KullanimTarziKodu == kullanimT &&
                                                                                            f.Kod2 == kod2).SingleOrDefault<CR_KullanimTarzi>();
                if (kTarzi != null)
                {
                    tarifeGrupKodu = kTarzi.TarifeKodu;
                }
            }

            // Mapfre Araç Kullanım Şekilleri
            // 0 - Özel
            // 1 - Ticari
            // 2 - Resmi
            if (teklif.Arac.KullanimSekli == "0")
                aracKullanimSekli = "1";
            else if (teklif.Arac.KullanimSekli == "1")
                aracKullanimSekli = "2";
            else
                aracKullanimSekli = "0";

            bool asistansVar = teklif.ReadSoru(TrafikSorular.Teminat_Asistans, false);

            List<mapfre.SahaWS> sahalarPolice = new List<mapfre.SahaWS>();
            sahalarPolice.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ULKE", p_val_campo = "TR" });
            sahalarPolice.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_ARAC_RENT_A_CAR_MI", p_val_campo = "H" });
            sahalarPolice.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_FILO_POLICE", p_val_campo = "H" });
            sahalarPolice.Add(new mapfre.SahaWS() { p_cod_campo = "VAL_CAMBIO_POLIZA", p_val_campo = "1" });
            request.sahalarPolice = sahalarPolice.ToArray();

            string Plaka = teklif.Arac.PlakaNo;

            if (Plaka == "YK")
                request.tramerBilgileri.islem = "Surprim";

            List<mapfre.SahaWS> sahalarRisk = new List<mapfre.SahaWS>();
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SIGORTALI_IL", p_val_campo = mapfreSigortaliIlKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SIGORTALI_ILCE", p_val_campo = mapfreSigortaliIlceKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SIGORTALI_UYRUK", p_val_campo = mapfreSigortaliUyruk });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_GOREV_ILI", p_val_campo = mapfreSigortaliIlKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_RIZIKO_ILI", p_val_campo = mapfreSigortaliIlKodu });
            // sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_DAINI_MURTEIN", p_val_campo = "H" });

            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_DAINI_MURTEIN", p_val_campo = dainiMurteinVar ? "E" : "H" });
            string kurumTipi = teklif.ReadSoru(KaskoSorular.DainiMurtein_KurumTipi, String.Empty);
            if (dainiMurteinVar && !String.IsNullOrEmpty(kurumTipi))
            {
                string kurumKodu = teklif.ReadSoru(KaskoSorular.DainiMurtein_KurumKodu, String.Empty);
                string subeKodu = teklif.ReadSoru(KaskoSorular.DainiMurtein_SubeKodu, String.Empty);
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_SBM_DM_KURUM_TIPI", p_val_campo = kurumTipi });
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_SBM_DM_KURUM_KODU", p_val_campo = kurumKodu });
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_SBM_DM_SUBE_KODU", p_val_campo = subeKodu });
            }

            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_PLAKA_IL_KODU", p_val_campo = "0" + teklif.Arac.PlakaKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_PLAKA_NO", p_val_campo = Plaka });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_MARKA", p_val_campo = teklif.Arac.Marka });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_MARKA_TIPI", p_val_campo = teklif.Arac.AracinTipi });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_SASI_NO", p_val_campo = teklif.Arac.SasiNo });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "TXT_MOTOR_NO", p_val_campo = teklif.Arac.MotorNo });
            if (teklif.Arac.Model.HasValue)
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "NUM_MODEL_YILI", p_val_campo = teklif.Arac.Model.Value.ToString() });
            if (teklif.Arac.KoltukSayisi.HasValue)
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "NUM_YER_ADEDI", p_val_campo = teklif.Arac.KoltukSayisi.Value.ToString() });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_KULLANIM_SEKLI", p_val_campo = aracKullanimSekli });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_KULLANIM_TARZI", p_val_campo = tarifeGrupKodu });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "FEC_TESCIL_TARIHI", p_val_campo = teklif.Arac.TrafikTescilTarihi.Value.ToString("ddMMyyyy") });

            bool hukuksalKorumaVarYok = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_VarYok, false);
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_HUKUKSAL_KORUMA", p_val_campo = hukuksalKorumaVarYok ? "E" : "H" });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_KEMIRGEN_HAYVAN", p_val_campo = "E" });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_ONARIM_YERI", p_val_campo = "T" });
            sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "MCA_YAGSIZLIK_SUSUZLUK_ZARAR", p_val_campo = "H" });
            //if (tarifeGrupKodu == "40")
            //{
            //    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_IKAME_TURU", p_val_campo = "ABC07" });
            //}
            //else
            //{
            //    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_IKAME_TURU", p_val_campo = "0" });
            //}

            string ikameTuru = teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
            if (!String.IsNullOrEmpty(ikameTuru))
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_IKAME_TURU", p_val_campo = ikameTuru });
            }
            else
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_IKAME_TURU", p_val_campo = "0" });
            }


            string amsKodu = teklif.ReadSoru(KaskoSorular.Teminat_AMS_Kodu, String.Empty);
            if (!String.IsNullOrEmpty(amsKodu))
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_GRP_AMS", p_val_campo = amsKodu });
                int olumSakatlik = (int)teklif.ReadSoru(KaskoSorular.Teminat_Olum_Sakatlik, decimal.Zero);
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "VAL_SK_OLUM_SAKATLIK", p_val_campo = Convert.ToString(olumSakatlik) });
            }
            else
            {
                string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                decimal bedeniSahis = 0;
                decimal kombine = 0;
                amsKodu = String.Empty;
                if (!String.IsNullOrEmpty(immKademe) && !String.IsNullOrEmpty(kullanimT))
                {
                    //var iMM = _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &&
                    //                                                       s.Kademe == immKademe && s.Kod2 == kod2 &&
                    //                                                       s.KullanimTarziKodu == kullanimT).FirstOrDefault();

                    //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                    if (IMMBedel != null)
                    {
                        bedeniSahis = IMMBedel.BedeniSahis.Value;
                        if (bedeniSahis == 0)
                        {
                            kombine = IMMBedel.Kombine.Value;
                        }
                    }
                }
                CR_KaskoAMS ams = new CR_KaskoAMS();
                if (bedeniSahis > 0)
                {
                    ams = _CRContext.CR_KaskoAMSRepository.All()
                                                              .Where(w => (w.TVMKodu == 999999 || w.TVMKodu == teklif.GenelBilgiler.TVMKodu) &&
                                                                          w.KullanimTarziKodu == tarifeGrupKodu &&
                                                                          w.BedeniSahis >= bedeniSahis)
                                                              .OrderBy(o => o.BedeniSahis)
                                                              .FirstOrDefault();
                }
                else
                {
                    ams = _CRContext.CR_KaskoAMSRepository.All()
                                                          .Where(w => (w.TVMKodu == 999999 || w.TVMKodu == teklif.GenelBilgiler.TVMKodu) &&
                                                                      w.KullanimTarziKodu == tarifeGrupKodu &&
                                                                      w.BedeniSahis >= kombine)
                                                          .OrderBy(o => o.BedeniSahis)
                                                          .FirstOrDefault();
                }
                if (ams != null)
                {
                    amsKodu = ams.AMSKodu;
                }
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_GRP_AMS", p_val_campo = amsKodu });

                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                {

                    //var FK = _CRContext.CR_KaskoFKRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &&
                    //                                                s.Kademe == fkKademe &&
                    //                                                s.Kod2 == kod2 &&
                    //                                                s.KullanimTarziKodu == kullanimT).FirstOrDefault();

                    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                    if (FKBedel != null)
                    {
                        int olumSakatlik = 5000;
                        if (FKBedel.Vefat.HasValue)
                        {
                            if (FKBedel.Vefat.Value >= 5000 && FKBedel.Vefat.Value <= 50000)
                                olumSakatlik = (int)FKBedel.Vefat.Value;
                            else if (FKBedel.Vefat.Value > 50000)
                                olumSakatlik = 50000;
                        }

                        sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "VAL_SK_OLUM_SAKATLIK", p_val_campo = Convert.ToString(olumSakatlik) });
                    }
                }
                else
                {
                    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "VAL_SK_OLUM_SAKATLIK", p_val_campo = "5000" });
                }
            }

            //Tescil seri kod, seri no, asbis no
            if (teklif.Arac.PlakaKodu != "YK")
            {
                sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ARAC_RUHSAT_SERI", p_val_campo = teklif.Arac.TescilSeriKod });
                if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriNo))
                {
                    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ARAC_RUHSAT_SERI_NO", p_val_campo = teklif.Arac.TescilSeriNo });
                }
                else if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                {
                    sahalarRisk.Add(new mapfre.SahaWS() { p_cod_campo = "COD_ASBIS_REFERANS_NO", p_val_campo = teklif.Arac.AsbisNo });
                }
            }
            MusteriTelefon sigortaliCepTel = teklifSigortali.MusteriGenelBilgiler.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
            if (sigortaliCepTel == null)
            {
                sigortaliCepTel = teklifSigortali.MusteriGenelBilgiler.MusteriTelefons.FirstOrDefault();
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

            #region Teminatlar
            // ==== TEminatlar ==== //
            List<mapfre.TeminatWS> teminatlar = new List<mapfre.TeminatWS>();
            // ==== KASKO ==== //
            teminatlar.Add(new mapfre.TeminatWS() { p_cod_cob = 4201, p_mca_seleccion = "S", p_capital = teklif.Arac.AracDeger });

            // ====  Ek teminatlar ==== //
            // ==== Araç Çalınması ==== //
            string calinmaVarYok = teklif.ReadSoru(KaskoSorular.Calinma_VarYok, false) ? "S" : "N";
            teminatlar.Add(new mapfre.TeminatWS() { p_cod_cob = 4220, p_mca_seleccion = calinmaVarYok });

            // ==== DEPREM ==== //
            string depremVarYok = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false) ? "S" : "N";
            teminatlar.Add(new mapfre.TeminatWS() { p_cod_cob = 4214, p_mca_seleccion = depremVarYok });

            bool ozelEsyaYok = teklif.ReadSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, false);
            if (ozelEsyaYok == false)
            {
                teminatlar.Add(new mapfre.TeminatWS() { p_cod_cob = 4224, p_mca_seleccion = "N" });
            }
            bool anahtarlaCalinmaYok = teklif.ReadSoru(KaskoSorular.Teminat_Anahtarla_Calinma_VarYok, false);
            if (anahtarlaCalinmaYok == false)
            {
                teminatlar.Add(new mapfre.TeminatWS() { p_cod_cob = 4219, p_mca_seleccion = "N" });
            }
            bool anahtarKaybiYok = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
            if (anahtarKaybiYok == false)
            {
                teminatlar.Add(new mapfre.TeminatWS() { p_cod_cob = 4223, p_mca_seleccion = "N" });
            }

            // ==== HUKUKSAL KORUMA ==== //   
            string hukuksalKoruma = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_VarYok, false) ? "S" : "N";
            teminatlar.Add(new mapfre.TeminatWS() { p_cod_cob = 4250, p_mca_seleccion = hukuksalKoruma });

            request.teminatlar = teminatlar.ToArray();
            #endregion

            #region Araç aksesuarlar
            if (teklif.AracEkSorular.Count > 0)
            {
                List<mapfre.ListWS> listeler = new List<mapfre.ListWS>();
                #region Aksesuarlar
                List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR)
                                                         .ToList<TeklifAracEkSoru>();

                if (aksesuarlar.Count > 0)
                {
                    mapfre.ListWS aksesuarList = new mapfre.ListWS();
                    aksesuarList.listCode = 902;
                    aksesuarList.listHolderCodCampo = "NUM_EKSTRA_AKSESUAR";

                    List<mapfre.ListElementWS> listElements = new List<mapfre.ListElementWS>();
                    foreach (TeklifAracEkSoru item in aksesuarlar)
                    {
                        mapfre.ListElementWS element = new mapfre.ListElementWS();
                        element.listElements = new mapfre.SahaWS[3];
                        element.listElements[0] = new mapfre.SahaWS() { p_cod_campo = "COD_EKSTRA_AKSESUAR", p_val_campo = item.SoruKodu };
                        element.listElements[1] = new mapfre.SahaWS() { p_cod_campo = "TXT_EKSTRA_AKSESUAR", p_val_campo = item.Aciklama };
                        element.listElements[2] = new mapfre.SahaWS() { p_cod_campo = "VAL_EKSTRA_AKSESUAR", p_val_campo = Convert.ToString(item.Bedel) };
                        listElements.Add(element);
                    }
                    aksesuarList.listElements = listElements.ToArray();
                    listeler.Add(aksesuarList);
                }
                #endregion

                #region Elektronik Cihaz Listesi
                List<TeklifAracEkSoru> elekCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                                                                          .ToList<TeklifAracEkSoru>();
                if (elekCihazlar.Count > 0)
                {
                    mapfre.ListWS elekCihazList = new mapfre.ListWS();
                    elekCihazList.listCode = 903;
                    elekCihazList.listHolderCodCampo = "VAL_ELEKTRONIK_CIHAZ_ADEDI";

                    List<mapfre.ListElementWS> listElements = new List<mapfre.ListElementWS>();
                    foreach (TeklifAracEkSoru item in elekCihazlar)
                    {
                        mapfre.ListElementWS element = new mapfre.ListElementWS();
                        element.listElements = new mapfre.SahaWS[3];
                        element.listElements[0] = new mapfre.SahaWS() { p_cod_campo = "COD_ELEKTRONIK_CIHAZ", p_val_campo = item.SoruKodu };
                        element.listElements[1] = new mapfre.SahaWS() { p_cod_campo = "TXT_ELEKTRONIK_CIHAZ_ACIKLAMA", p_val_campo = item.Aciklama };
                        element.listElements[2] = new mapfre.SahaWS() { p_cod_campo = "VAL_ELEKTRONIK_CIHAZ_BEDEL", p_val_campo = Convert.ToString(item.Bedel) };
                        listElements.Add(element);
                    }
                    elekCihazList.listElements = listElements.ToArray();
                    listeler.Add(elekCihazList);
                }
                #endregion


                request.listeler = listeler.ToArray();
            }
            #endregion

            request.bilgiMesajlari = new string[1];
            request.bilgiMesajlari[0] = "<![CDATA[<ApiCode>0I4eod77GFrsrHNPUeF4wA==</ApiCode>]]>";

            return request;
        }
    }
}
