using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Common.GROUPAMA;
using Neosinerji.BABOnlineTP.Business.groupama.service;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.GROUPAMA
{
    public class GROUPAMAKasko : Teklif, IGROUPAMAKasko
    {
        ICRService _CRService;
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IUlkeService _UlkeService;
        ITVMService _TVMService;
        IAracService _AracService;
        [InjectionConstructor]
        public GROUPAMAKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService, IAracService AracService)
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

            _UlkeService = DependencyResolver.Current.GetService<IUlkeService>();
            _TVMService = TVMService;
            _AracService = AracService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.GROUPAMA;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region Veri Hazırlama
                groupama.service.BGS_WS clntKasko = new groupama.service.BGS_WS();

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGROUPAMAService);
                clntKasko.Url = konfig[Konfig.GROUPAMA_ServiceURL];
                clntKasko.Timeout = 250000;
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.GROUPAMA });
                #endregion

                #region  Request

                DateTime polBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                DateTime polBitis = polBaslangic.AddYears(1);
                int polBasYil = 9999;
                int polBasAy = 99;
                int polBasGun = 99;

                int polBitisYil = 9999;
                int polBitisAy = 99;
                int polBitisGun = 99;

                if (polBaslangic != null)
                {
                    polBasYil = polBaslangic.Year;
                    polBasAy = polBaslangic.Month;
                    polBasGun = polBaslangic.Day;

                }
                if (polBitis != null)
                {
                    polBitisYil = polBitis.Year;
                    polBitisAy = polBitis.Month;
                    polBitisGun = polBitis.Day;
                }

                TeklifCreateRequest kaskoRequest = new TeklifCreateRequest();

                kaskoRequest.teklifId = 0;
                kaskoRequest.teklifVersiyon = 0;
                //ip = ClientIPNo,                    
                kaskoRequest.acente = new AcenteBilgi();

                kaskoRequest.acente.acenteKodu = Convert.ToInt64(servisKullanici.PartajNo_);
                kaskoRequest.acente.kullaniciKodu = Convert.ToInt64(servisKullanici.KullaniciAdi);
                kaskoRequest.acente.type = ACENTE_TYPE.OZEL;
                
                kaskoRequest.sigortali = this.SigortaliBilgileri(teklif, clntKasko);
                kaskoRequest.sigortaEttiren = this.SigortaEttirenBilgileri(teklif, clntKasko);

                kaskoRequest.teklifZamani = System.DateTime.Now;
                kaskoRequest.teklifBaslangicTarihi = new DateTime(polBasYil, polBasAy, polBasGun);
                kaskoRequest.teklifBitisTarihi = new DateTime(polBitisYil, polBitisAy, polBitisGun);
                kaskoRequest.dovizKodu = 0; // 0 - TL                   


                if (teklif.Arac.KullanimTarzi == "111-10" || teklif.Arac.KullanimTarzi == "111-12" || teklif.Arac.KullanimTarzi == "111-11")
                {

                    kaskoRequest.bgsUrunKodu = GroupamaUrunKodlari.HususiOtoKasko;
                }
                else
                {
                    kaskoRequest.bgsUrunKodu = GroupamaUrunKodlari.KlasikKasko;
                }


                bool ElitKasko = teklif.ReadSoru(KaskoSorular.GroupamaElitKaskomu, false);
                if (ElitKasko)
                {
                    kaskoRequest.bgsUrunKodu = GroupamaUrunKodlari.ElitKasko;
                }


                kaskoRequest.soruListesi = this.GroupamaTeklifSorular(teklif, kaskoRequest.bgsUrunKodu).ToArray();
                kaskoRequest.sigortaKonusuList = this.GroupamaSigortaKonusu(teklif, kaskoRequest.bgsUrunKodu).ToArray();
                kaskoRequest.teminatList = this.GroupamaTeminat(teklif, kaskoRequest.bgsUrunKodu).ToArray();
                this.BeginLog(kaskoRequest, kaskoRequest.GetType(), WebServisIstekTipleri.Teklif);

                #endregion

                #region Response
                var response = clntKasko.createTeklif(kaskoRequest);
                if (response != null)
                {
                    if (response.responseHeader.answerCode == 100)
                    {
                        this.EndLog(response, true, response.GetType());

                    }
                    else
                    {
                        this.EndLog(response, true, response.GetType());
                        this.AddHata(response.responseHeader.answerCode + response.responseHeader.answer);
                    }
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

                #region Teklif kaydı

                #region Genel bilgiler
                this.Import(teklif);

                this.GenelBilgiler.TUMTeklifNo = response.teklif.teklifId.ToString();
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BrutPrim = Convert.ToDecimal(response.teklif.brutPrim);
                this.GenelBilgiler.NetPrim = Convert.ToDecimal(response.teklif.netPrim);
                this.GenelBilgiler.ToplamVergi = Convert.ToDecimal(response.teklif.vergiFonTutari);
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = Convert.ToByte(response.teklif.dovizKodu);
                this.GenelBilgiler.BaslamaTarihi = response.teklif.teklifBaslangicTarihi;
                this.GenelBilgiler.BitisTarihi = response.teklif.teklifBitisTarihi;
                this.GenelBilgiler.TanzimTarihi = response.teklif.teklifZamani;

                if (teklif.GenelBilgiler.TarifeBasamakKodu.HasValue && teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi.HasValue)
                {
                    if (teklif.GenelBilgiler.TarifeBasamakKodu > 0 && teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi > 0)
                    {
                        if (teklif.GenelBilgiler.TarifeBasamakKodu < 4)
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = teklif.GenelBilgiler.HasarsizlikIndirimYuzdesi;
                        }
                        else if (teklif.GenelBilgiler.TarifeBasamakKodu > 4)
                        {
                            this.GenelBilgiler.HasarSurprimYuzdesi = teklif.GenelBilgiler.HasarSurprimYuzdesi;
                        }
                        else
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        }
                    }
                }
                #endregion
                #region Teklif PDF

                string pdfURL = this.TeklifPDF(clntKasko, servisKullanici);
                if (!String.IsNullOrEmpty(pdfURL))
                {
                    this.GenelBilgiler.PDFDosyasi = pdfURL;
                }
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
            #region Veri Hazırlama

            ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
            groupama.service.BGS_WS clntKasko = new groupama.service.BGS_WS();

            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGROUPAMAService);
            clntKasko.Url = konfig[Konfig.GROUPAMA_ServiceURL];
            clntKasko.Timeout = 250000;
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.GROUPAMA });
            string GroupamaTeklifVersionNo = this.ReadWebServisCevap(Common.WebServisCevaplar.GROUPAMA_TeklifVersiyonNo, "0");

            #endregion

            #region Request

            PoliceCreateRequest request = new PoliceCreateRequest();
            request.teklifId = Convert.ToInt64(this.GenelBilgiler.TUMTeklifNo);
            request.teklifVersiyon = Convert.ToInt32(GroupamaTeklifVersionNo);

            request.acente = new AcenteBilgi()
            {
                acenteKodu = Convert.ToInt64(servisKullanici.KullaniciAdi),
                kullaniciKodu = Convert.ToInt64(servisKullanici.Sifre),
                type = ACENTE_TYPE.OZEL,
            };

            this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);

            if (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
            {
                bool KartTypeMasterMi = true;
                if (odeme.KrediKarti.KartNo.Substring(0, 1) == "5")
                {
                    KartTypeMasterMi = true;
                }
                else
                {
                    KartTypeMasterMi = false;
                }
                request.odemeBilgileri = new OdemeBilgileri()
                {
                    odemeTipi = ODEMETYPE.KrediKarti,
                    odemePlanNo = Groupoma_Odeme_Plani.KrediKartiBlokeli,
                    krediKarti = new groupama.service.KrediKarti()
                    {
                        gecerlilikAy = Convert.ToInt32(odeme.KrediKarti.SKA),
                        gecerlilikYil = Convert.ToInt32(odeme.KrediKarti.SKY),
                        kartGuvenlikNo = Convert.ToInt32(odeme.KrediKarti.CVC),
                        kartNo = Convert.ToInt64(odeme.KrediKarti.KartNo),
                        kartTip = KartTypeMasterMi ? KARTTIP.Master : KARTTIP.Visa,
                        taksitSayisi = odeme.TaksitSayisi
                    },
                };
            }
            else
            {
                request.odemeBilgileri = new OdemeBilgileri()
                {
                    odemeTipi = ODEMETYPE.KrediKarti,
                    odemePlanNo = Groupoma_Odeme_Plani.KrediKartiBlokeli,
                    krediKarti = new groupama.service.KrediKarti()
                    {
                        gecerlilikAy = 0,
                        gecerlilikYil = 0,
                        kartGuvenlikNo = 0,
                        kartNo = 0,
                        kartTip = 0,
                        taksitSayisi = 1
                    },
                };
            }

            #endregion

            #region Response

            var response = clntKasko.createPolice(request);
            clntKasko.Dispose();

            if (response != null)
            {
                if (response.responseHeader.answerCode == 100) //100=Servis Başarılı Kodu
                {
                    this.EndLog(response, true, response.GetType());

                    this.GenelBilgiler.TUMPoliceNo = response.policeNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    string policePDFURL = this.PolicePDF(clntKasko, servisKullanici, this.GenelBilgiler.TUMPoliceNo, "police");
                    if (!String.IsNullOrEmpty(policePDFURL))
                    {
                        this.GenelBilgiler.PDFPolice = policePDFURL;
                    }
                    string policeBilgilendirmeURL = this.PolicePDF(clntKasko, servisKullanici, this.GenelBilgiler.TUMPoliceNo, "policeBilgilendirme");
                    if (!String.IsNullOrEmpty(policeBilgilendirmeURL))
                    {
                        this.GenelBilgiler.PDFBilgilendirme = policeBilgilendirmeURL;
                    }

                    this.GenelBilgiler.WEBServisLogs = this.Log;
                    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());
                    this.AddHata(response.responseHeader.answerCode + response.responseHeader.answer);
                }
            }

            #endregion
        }

        public string TeklifPDF(groupama.service.BGS_WS clntKasko, TVMWebServisKullanicilari servisKullanici)
        {
            try
            {
                PrintRequest request = new PrintRequest();
                request.acente = new AcenteBilgi()
                {
                    acenteKodu = Convert.ToInt64(servisKullanici.KullaniciAdi),
                    kullaniciKodu = Convert.ToInt64(servisKullanici.Sifre),
                    type = ACENTE_TYPE.OZEL
                };

                request.type = PRINT_TYPE.TEKLIF;
                request.teklifId = Convert.ToInt64(this.GenelBilgiler.TUMTeklifNo);

                string GroupamaTeklifVersiyon = this.ReadWebServisCevap(Common.WebServisCevaplar.GROUPAMA_TeklifVersiyonNo, "0");
                request.teklifVersiyon = Convert.ToInt32(GroupamaTeklifVersiyon);

                this.BeginLog(request, typeof(PrintRequest), WebServisIstekTipleri.Teklif);
                var response = clntKasko.getPrintURL(request);
                clntKasko.Dispose();
                if (response.responseHeader.answerCode == 100)
                {
                    WebClient myClient = new WebClient();
                    byte[] data = myClient.DownloadData(response.printURL);

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("Groupama_Kasko_Police_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                    string url = storage.UploadFile("kasko", fileName, data);
                    return url;
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                _Log.Error("GroupamaKasko.TeklifPDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                return String.Empty;
            }
        }

        public List<groupama.service.Soru> GroupamaTeklifSorular(ITeklif teklif, int kaskoTuru)
        {
            List<groupama.service.Soru> soruList = new List<groupama.service.Soru>();
            int GroupamaKullanimTarziKodu = 1;
            string kullanimtarzi = teklif.Arac.KullanimTarzi;
            groupama.service.Soru soru = null;

            #region kullanim tarzı soruları


            if (!String.IsNullOrEmpty(kullanimtarzi))
            {
                switch (kullanimtarzi)
                {
                    case "111-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.OTOMOBİL; break;
                    case "111-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.TAKSİ; break;
                    case "111-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.RENT_A_CAR_10; break;
                    case "311-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİNİBÜS_HATLI_DOLMUS_10_17_YOLCU_dankucuk; break;
                    case "311-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİNİBÜS_ÖZEL_SERVİS_ŞEHİRİÇİ_10_17_YOLCU; break;
                    case "311-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİNİBÜS_ÖZEL_SERVİS_ŞEHİRİÇİ_10_17_YOLCU; break;
                    case "411-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİDİBÜS_ÖZEL_SERVİS_ŞEHİR_İÇİ_18_30_YOLCU; break;
                    case "411-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİDİBÜS_ÖZEL_SERVİS_ŞEHİR_İÇİ_18_30_YOLCU; break;
                    case "411-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİDİBÜS_ÖZEL_SERVİS_ŞEHİR_İÇİ_18_30_YOLCU; break;
                    case "411-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİDİBÜS_HATLI_ŞEHİRLER_ARASI_18_30_YOLCU; break;
                    case "421-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.OTOBÜS_TİCARİ_ŞEHİRLER_ARASI_31_VE_ÜST; break;
                    case "421-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.OTOBÜS_TİCARİ_ŞEHİR_İÇİ_ÖZEL_31_VE_ÜST; break;
                    case "421-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.OTOBÜS_TİCARİ_ŞEHİR_İÇİ_ÖZEL_31_VE_ÜST; break;
                    case "421-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.OTOBÜS_TİCARİ_ŞEHİR_İÇİ_ÖZEL_31_VE_ÜST; break;
                    case "511-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "511-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "511-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "511-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "511-14": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "511-15": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "511-16": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "521-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYONET; break;
                    case "521-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYON; break;
                    case "521-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYON; break;
                    case "521-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ÇÖP_İTFAİYE_KAMYONU; break;
                    case "521-14": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ÇÖP_İTFAİYE_KAMYONU; break;
                    case "521-15": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYON; break;
                    case "521-16": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYON; break;
                    case "521-17": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİKSER_BETON_POMPALI_KAYA_KAMYONU; break;
                    case "521-18": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.SİLOLU_KAMYON; break;
                    case "521-19": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİKSER_BETON_POMPALI_KAYA_KAMYONU; break;
                    case "521-20": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MİKSER_BETON_POMPALI_KAYA_KAMYONU; break;
                    case "521-21": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYON; break;
                    case "521-22": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.KAMYON; break;
                    case "526-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ÇEKİCİ_TIR; break;
                    case "526-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ÇEKİCİ_TIR; break;
                    case "523-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.TANKER; break;
                    case "523-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.TANKER_ASİT_TAŞIYAN; break;
                    case "523-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.TANKER_ASİT_PETROL_VB_TAŞIMAYAN; break;
                    case "523-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.TANKER_ASİT_PETROL_VB_TAŞIMAYAN; break;
                    case "111-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ZIRHLI_ARAÇ; break;
                    case "911-17": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-20": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-21": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_SABİT; break;
                    case "711-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.MOTOSİKLET_TRİPORTÖR; break;
                    case "611-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.BİÇER_DÖVER; break;
                    case "611-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ZIRAİ_TRAKTÖR; break;
                    case "532-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.RÖMORK; break;
                    case "311-14": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.AMBULANS; break;
                    case "121-10": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ARAZİ_TAŞITI_JEEP; break;
                    case "121-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ARAZİ_TAŞITI_JEEP; break;
                    case "121-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ARAZİ_TAŞITI_JEEP; break;
                    case "121-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ARAZİ_TAŞITI_JEEP; break;
                    case "121-14": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.ARAZİ_TAŞITI_JEEP; break;
                    case "911-11": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-12": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-13": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-14": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-15": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-16": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.İŞ_MAKİNASI_HAREKETLİ; break;
                    case "911-18": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.LOKOMOTİF_VAGON; break;
                    case "911-19": GroupamaKullanimTarziKodu = Groupoma_HususiOtoKaskoSorular_KullanimTarziSorular.LOKOMOTİF_VAGON; break;
                }
            }

            #endregion

            if (kaskoTuru == GroupamaUrunKodlari.HususiOtoKasko)
            {
                #region sorular

                bool asistansPlusPaket = teklif.ReadSoru(KaskoSorular.AsistansPlusPaketi, false);
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.ASISTANS_PLUS_PAKETI;
                if (asistansPlusPaket) soru.DecimalDeger = 1;
                else soru.DecimalDeger = 2;
                soruList.Add(soru);

                bool alarm = teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false);
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.ALARM_VAR_MI;
                if (alarm) soru.DecimalDeger = 1;
                else soru.DecimalDeger = 2;
                soruList.Add(soru);

                //Araç sürücü bilgisi eklenecek mi
                //Araç gce park yeri eklenecek mi
                //ehliyet tarihi eklenecek mi
                //radyo teyp markası eklenecek mi
                //kolonlar markası eklenecek mi
                //alarm markası eklenecek mi
                //diğer ek cihazlar eklenecek mi

                //====Ferdi Kaza Teminatı   ==== //
                string kullanimT = String.Empty;
                string kod2 = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    kullanimT = parts[0];
                    kod2 = parts[1];
                }
                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                bool fkVarmi = false;
                string Grupoma_FKKademe = "1";

                // koltuk teminat
                if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                {
                    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                    if (FKBedel != null)
                    {
                        fkVarmi = true;
                        CRKademeNo = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.GROUPAMA, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, parts[0], parts[1]);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KOLTUK_SAHIS_BASINA_OLUM_TAZMINATI;
                        soru.DecimalDeger = Convert.ToDouble(FKBedel.Vefat.Value);
                        soruList.Add(soru);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KOLTUK_SAHIS_BASINA_TEDAVI_TAZMINATI;
                        soru.DecimalDeger = Convert.ToDouble(FKBedel.Tedavi.Value);
                        soruList.Add(soru);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KOLTUK_SBDS_TAZ;
                        soru.DecimalDeger = Convert.ToDouble(FKBedel.Sakatlik.Value);
                        soruList.Add(soru);
                    }
                    else
                    {
                        fkVarmi = false;
                    }
                    if (CRKademeNo != null)
                    {
                        Grupoma_FKKademe = CRKademeNo.Kademe.ToString();
                    }
                }
                if (!fkVarmi)
                {
                    //FK Yoksa default değerler gönderiliyor
                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KOLTUK_SAHIS_BASINA_OLUM_TAZMINATI;
                    soru.DecimalDeger = 10000;
                    soruList.Add(soru);

                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KOLTUK_SBDS_TAZ;
                    soru.DecimalDeger = 10000;
                    soruList.Add(soru);

                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KOLTUK_SAHIS_BASINA_TEDAVI_TAZMINATI;
                    soru.DecimalDeger = 1000;
                    soruList.Add(soru);
                }

                //Tramer Belge Sıra No Gönderilecek Mi
                //Hasarsızlık Var Mı? eklenecek mi
                //Has. Ten. kor Var Mı? eklenecek mi
                //Yakınlık Derecesi eklenecek mi
                //Riziko fiyatı eklenecek mi

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                if (sigortali != null)
                {
                    if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                    {
                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.OZEL_TUZEL_SIGORTALI;
                        soru.DecimalDeger = 1;//ÖZEL SİGORTALI
                        soruList.Add(soru);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.SIGORTALI_CINSIYETI;
                        if (sigortali.Cinsiyet == "K")
                        {
                            soru.DecimalDeger = 1;//Bayan
                        }
                        else
                        {
                            soru.DecimalDeger = 2;//Erkek
                        }
                        soruList.Add(soru);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.SIGORTALI_DOGUM_TARIHI;
                        soru.KarakterDeger = Convert.ToString(sigortali.DogumTarihi);
                        soru.DecimalDeger = 0;
                        soruList.Add(soru);
                    }
                    else
                    {
                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.OZEL_TUZEL_SIGORTALI;
                        soru.DecimalDeger = 2;//TÜZEL SİGORTALI
                        soruList.Add(soru);
                    }
                }

                string acenteOzelIndirim = teklif.ReadSoru(KaskoSorular.AcenteOzelIndirimi, String.Empty);
                if (!String.IsNullOrEmpty(acenteOzelIndirim))
                {
                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.ACENTE_OZEL_INDIRIMI;
                    soru.DecimalDeger = Convert.ToDouble(acenteOzelIndirim);
                    soruList.Add(soru);
                }

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.TESCILNO_ASBISNO;
                soru.DecimalDeger = 0;
                if (teklif.Arac.TescilSeriKod != null) soru.KarakterDeger = teklif.Arac.TescilSeriNo;
                else soru.KarakterDeger = teklif.Arac.AsbisNo;

                //Kontrol Parametre 4 eklenecek mi

                bool kazaDestekVarMi = teklif.ReadSoru(KaskoSorular.KazaDestekVarmi, false);
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KAZA_DESTEK_VARMI;
                if (kazaDestekVarMi) soru.DecimalDeger = 1;
                else soru.DecimalDeger = 2;
                soruList.Add(soru);

                if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                {
                    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                    if (FKBedel != null)
                    {
                        CRKademeNo = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.GROUPAMA, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, parts[0], parts[1]);
                    }
                    if (CRKademeNo != null)
                    {
                        Grupoma_FKKademe = CRKademeNo.Kademe.ToString();
                    }
                }

                string GroupamaMeslekKodu = teklif.ReadSoru(KaskoSorular.GroupamaMeslekKodu, "");
                if (!String.IsNullOrEmpty(GroupamaMeslekKodu))
                {
                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.MESLEK_INDIRIMI;
                    soru.DecimalDeger = Convert.ToDouble(GroupamaMeslekKodu);
                    soruList.Add(soru);
                }

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.DIGER_EK_CIHAZLAR;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                string kolonMarka = teklif.ReadSoru(KaskoSorular.KolonMarka, String.Empty);
                if (!String.IsNullOrEmpty(kolonMarka))
                {
                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.KOLONLAR_MARKA;
                    soru.KarakterDeger = kolonMarka;
                    soruList.Add(soru);
                }

                if (teklif.AracEkSorular.Count > 0)
                {

                    List<TeklifAracEkSoru> elektronikCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                                                                   .ToList<TeklifAracEkSoru>();
                    string Aciklama = "";
                    if (elektronikCihazlar.Count > 0)
                    {
                        foreach (TeklifAracEkSoru item in elektronikCihazlar)
                        {
                            if (item.SoruKodu == MapfreElektronikCihazTipleri.RT)
                            {
                                Aciklama = !String.IsNullOrEmpty(item.Aciklama) ? item.Aciklama : "";

                                soru = new groupama.service.Soru();
                                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.RADYO_TEYP_MARKASI;
                                soru.KarakterDeger = Aciklama;
                                soruList.Add(soru);
                            }
                        }
                    }
                }

                string YHIMSVarMi = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSKodu, String.Empty);
                string YHIMSBasamakKodu = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSBasamakKodu, String.Empty);
                string YHIMSSerbestLimit = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSSerbestLimit, String.Empty);

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.YHIMS_VARMI;
                if (!String.IsNullOrEmpty(YHIMSVarMi))
                {
                    soru.DecimalDeger = Convert.ToDouble(YHIMSVarMi);
                }
                else
                {
                    soru.DecimalDeger = Groupoma_KlasikKaskoSorular_IMMTipi.YHIMS_YOK;
                }

                soruList.Add(soru);

                if (!String.IsNullOrEmpty(YHIMSBasamakKodu))
                {
                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.YHIMS_BASAMAK_NO;
                    soru.DecimalDeger = Convert.ToDouble(YHIMSBasamakKodu);
                    soruList.Add(soru);
                }

                soru = new groupama.service.Soru();
                soru.Id = 51;
                soru.DecimalDeger = 1;
                soruList.Add(soru);

                #endregion

            }
            else if (kaskoTuru == GroupamaUrunKodlari.KlasikKasko)
            {
                #region sorular

                //Radyo- Teyp markası eklenecek mi
                //Kolonlar markası eklenecek mi
                //DİĞER EK CİHAZLAR eklenecek mi
                //DIŞ KASKO BİTİŞ TARİHİ eklenecek mi 

                //TRAMER BELGE SIRA NO
                //HASARSIZLIK VAR MI ? 
                //HAS.TEN.KOR VAR MI ?
                //YAKINLIK DERECESİ
                //YHIMS BASAMAK NO

                string kullanimT = String.Empty;
                string kod2 = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    kullanimT = parts[0];
                    kod2 = parts[1];
                }

                string YHIMSVarMi = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSKodu, String.Empty);
                string YHIMSBasamakKodu = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSBasamakKodu, String.Empty);
                string YHIMSSerbestLimit = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSSerbestLimit, String.Empty);

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.YHIMS_VARMI;
                if (!String.IsNullOrEmpty(YHIMSVarMi))
                {
                    soru.DecimalDeger = Convert.ToDouble(YHIMSVarMi);
                }
                else
                {
                    soru.DecimalDeger = Groupoma_KlasikKaskoSorular_IMMTipi.YHIMS_YOK;
                }

                soruList.Add(soru);

                if (!String.IsNullOrEmpty(YHIMSBasamakKodu))
                {
                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.YHIMS_BASAMAK_NO;
                    soru.DecimalDeger = Convert.ToDouble(YHIMSBasamakKodu);
                    soruList.Add(soru);
                }


                MusteriGenelBilgiler sigortaliBilgim = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.SIGORTALI_DOGUM_TARIHI;
                soru.KarakterDeger = Convert.ToString(sigortaliBilgim.DogumTarihi);
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                //====Ferdi Kaza Teminatı   ==== //

                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                string Grupoma_FKKademe = "1";

                if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                {

                    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.KOLTUK_FK_VARMI;
                    if (!String.IsNullOrEmpty(fkKademe)) soru.DecimalDeger = 1;
                    else soru.DecimalDeger = 2;
                    soruList.Add(soru);

                    if (FKBedel != null)
                    {
                        CRKademeNo = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.GROUPAMA, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, parts[0], parts[1]);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.KOLTUK_SAHIS_BASINA_OLUM_TAZMINATI;
                        soru.DecimalDeger = Convert.ToDouble(FKBedel.Vefat.Value);
                        soruList.Add(soru);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.KOLTUK_SAHIS_BASINA_TEDAVI_TAZMINATI;
                        soru.DecimalDeger = Convert.ToDouble(FKBedel.Tedavi.Value);
                        soruList.Add(soru);

                        soru = new groupama.service.Soru();
                        soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.KOLTUK_SBDS_TAZ;
                        soru.DecimalDeger = Convert.ToDouble(FKBedel.Sakatlik.Value);
                        soruList.Add(soru);
                    }
                    if (CRKademeNo != null)
                    {
                        Grupoma_FKKademe = CRKademeNo.Kademe.ToString();
                    }
                }

                #endregion
            }

            #region Ortak Sorular

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.KULLANIM_TARZI;
            soru.DecimalDeger = Convert.ToDouble(GroupamaKullanimTarziKodu); // <KarakterDeger>kullanimTarzi.otomobil</KarakterDeger> 
            soruList.Add(soru);

            if (teklif.GenelBilgiler.TarifeBasamakKodu > 0)
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.HASARSIZLIK_VARMI;
                soru.DecimalDeger = Convert.ToDouble(teklif.GenelBilgiler.TarifeBasamakKodu);
                soruList.Add(soru);
            }

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.MARKA_TIP; //MARKA & TİP  Tip kodu 4 hane zorunlu                      
            soru.DecimalDeger = Convert.ToDouble(teklif.Arac.Marka + teklif.Arac.AracinTipi.PadLeft(4, '0'));
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.MODEL;
            soru.DecimalDeger = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.YER_ADEDI;
            soru.DecimalDeger = teklif.Arac.KoltukSayisi.HasValue ? (teklif.Arac.KoltukSayisi.Value - 1) : 4;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.MOTOR_NUMARASI;
            soru.KarakterDeger = teklif.Arac.MotorNo;
            soru.DecimalDeger = 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.SASI_NUMARASI;
            soru.KarakterDeger = teklif.Arac.SasiNo;
            soru.DecimalDeger = 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.PLAKA_IL_KODU;
            soru.KarakterDeger = teklif.Arac.PlakaKodu;
            soru.DecimalDeger = 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.PLAKA_NUMARASI;

            if (teklif.Arac.PlakaNo == "YK")
            {
                soru.KarakterDeger = teklif.Arac.PlakaKodu + "G 111";
            }
            else
            {
                soru.KarakterDeger = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
            }

            soru.DecimalDeger = 0;
            soruList.Add(soru);

            bool pesinIndirimi = teklif.ReadSoru(KaskoSorular.PesinIndirimi, false);
            if (pesinIndirimi)
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.PESIN_INDIRIMI;
                soru.DecimalDeger = Groupoma_CevapTipi.Evet;
                soruList.Add(soru);
            }
            else
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.PESIN_INDIRIMI;
                soru.DecimalDeger = Groupoma_CevapTipi.Hayir;
                soruList.Add(soru);
            }

            #region Önceki Poliçe

            bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
            string oncekiAcenteNo = String.Empty;
            string oncekiPoliceNo = String.Empty;
            string oncekiSirketKodu = String.Empty;
            string oncekiYenilemeNo = String.Empty;

            if (eskiPoliceVar)
            {
                oncekiAcenteNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Acente_No, String.Empty);
                oncekiPoliceNo = teklif.ReadSoru(KaskoSorular.Eski_Police_No, String.Empty);
                oncekiSirketKodu = teklif.ReadSoru(KaskoSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                oncekiYenilemeNo = teklif.ReadSoru(KaskoSorular.Eski_Police_Yenileme_No, String.Empty);

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.ONCEKI_ACENTE;
                soru.KarakterDeger = oncekiAcenteNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.ONCEKI_POLICE;
                soru.KarakterDeger = oncekiPoliceNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.ONCEKI_SIRKET_KODU;
                soru.DecimalDeger = Convert.ToDouble(oncekiSirketKodu); // oncekiSirketKodu;;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.ONCEKI_YENILEME_NO;
                soru.KarakterDeger = oncekiYenilemeNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);
            }

            #endregion

            if (!String.IsNullOrEmpty(teklif.Arac.TramerBelgeNo))
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.TRAMER_BELGE_NO;
                soru.DecimalDeger = Convert.ToDouble(teklif.Arac.TramerBelgeNo);
                soruList.Add(soru);
            }

            if (teklif.Arac.TramerBelgeTarihi.HasValue)
            {
                if (teklif.Arac.TramerBelgeTarihi != null && teklif.Arac.TramerBelgeTarihi.ToString() != "")
                {
                    soru = new groupama.service.Soru();
                    soru.Id = Groupoma_HususiOtoKaskoSorular_SoruListesi.TRAMER_BELGE_TARIHI;

                    string tarih = teklif.Arac.TramerBelgeTarihi.Value.ToString("ddMMyyyy");
                    var TramerBelgeTarihi = Convert.ToDouble(tarih.Substring(0, 2) + tarih.Substring(2, 2) + tarih.Substring(4, 4));

                    soru.DecimalDeger = TramerBelgeTarihi;
                    soruList.Add(soru);
                }
            }

            string Grupoma_KazaDestekTeminatlimiti = teklif.ReadSoru(KaskoSorular.TeminatLimiti, String.Empty);
            if (!String.IsNullOrEmpty(Grupoma_KazaDestekTeminatlimiti))
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupoma_KlasikKaskoSorular_SoruListesi.TEMINAT_LIMITLERI;
                soru.DecimalDeger = Convert.ToDouble(Grupoma_KazaDestekTeminatlimiti);
                soruList.Add(soru);
            }

            #endregion

            return soruList;
        }

        public List<groupama.service.SigortaKonusu> GroupamaSigortaKonusu(ITeklif teklif, int kaskoTuru)
        {
            List<groupama.service.SigortaKonusu> SigortaKonusuList = new List<groupama.service.SigortaKonusu>();
            SigortaKonusu sigortasoru = new groupama.service.SigortaKonusu();

            #region Husiso Oto kasko
            if (kaskoTuru == GroupamaUrunKodlari.HususiOtoKasko)
            {
                //bool alarm = teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false);
                //if (alarm)
                //{
                //    sigortasoru = new groupama.service.SigortaKonusu();
                //    sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.ALARM;
                //    sigortasoru.Bedel = 0;
                //    SigortaKonusuList.Add(sigortasoru);
                //}
                if (teklif.ReadSoru(KaskoSorular.LPG_VarYok, false))
                {
                    if (!teklif.ReadSoru(KaskoSorular.LPG_Arac_Orjinalmi, true))
                    {
                        sigortasoru = new groupama.service.SigortaKonusu();
                        sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.LPG;
                        sigortasoru.Bedel = Convert.ToDouble(teklif.ReadSoru(KaskoSorular.LPG_Bedel, 0));
                        SigortaKonusuList.Add(sigortasoru);
                    }
                }

                #region Araç aksesuarlar

                if (teklif.AracEkSorular.Count > 0)
                {
                    List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR)
                                                             .ToList<TeklifAracEkSoru>();

                    if (aksesuarlar.Count > 0)
                    {
                        sigortasoru = new groupama.service.SigortaKonusu();
                        foreach (TeklifAracEkSoru item in aksesuarlar)
                        {
                            if (item.SoruKodu == MapfreAksesuarTipleri.LPG)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.LPG;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Jant)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.CELIK_JANT;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Diger)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.DIGER_EK_CIHAZLAR;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                        }

                    }
                }

                #endregion

                bool primKoruma = teklif.ReadSoru(KaskoSorular.PrimKoruma, false);
                if (primKoruma)
                {
                    sigortasoru = new groupama.service.SigortaKonusu();
                    sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.PRIM_KORUMA;
                    sigortasoru.Bedel = 0;
                    SigortaKonusuList.Add(sigortasoru);
                }

                var GroupamaAracHukuksalKoruma = teklif.ReadSoru(KaskoSorular.GroupamaAracHukuksalKoruma, 0);
                var GroupamaSurucuHukuksalKoruma = teklif.ReadSoru(KaskoSorular.GroupamaSurucuHukuksalKoruma, 0);
                var GroupamaSerbestLimit = teklif.ReadSoru(KaskoSorular.GroupamaYHIMSSerbestLimit, 0);
                var GroupamaManeviTazminatBedeli = teklif.ReadSoru(KaskoSorular.GroupamaManeviDahilBedeli, 0);

                sigortasoru = new groupama.service.SigortaKonusu();
                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.GroupamaAracHukuksalKoruma;
                sigortasoru.Bedel = Convert.ToDouble(GroupamaAracHukuksalKoruma);
                SigortaKonusuList.Add(sigortasoru);

                sigortasoru = new groupama.service.SigortaKonusu();
                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.GroupamaSurucuHukuksalKoruma;
                sigortasoru.Bedel = Convert.ToDouble(GroupamaSurucuHukuksalKoruma);
                SigortaKonusuList.Add(sigortasoru);

                if (GroupamaManeviTazminatBedeli > 0)
                {
                    sigortasoru = new groupama.service.SigortaKonusu();
                    sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.YHİMS_KB_MANEVİ_TAZ;
                    sigortasoru.Bedel = Convert.ToDouble(GroupamaManeviTazminatBedeli); //Max. Manevi Tazminat
                    SigortaKonusuList.Add(sigortasoru);
                }

                sigortasoru = new groupama.service.SigortaKonusu();
                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.YHIMS_MADDİ_BEDENI;
                sigortasoru.Bedel = Convert.ToDouble(GroupamaSerbestLimit);
                SigortaKonusuList.Add(sigortasoru);
            }
            #endregion

            #region Klasik Kasko

            else if (kaskoTuru == GroupamaUrunKodlari.KlasikKasko)
            {
                //string[] parts = teklif.Arac.KullanimTarzi.Split('-');

                //string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                //if (!String.IsNullOrEmpty(immKademe))
                //{
                //    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                //    if (IMMBedel != null)
                //    {
                //        sigortasoru = new groupama.service.SigortaKonusu();
                //        sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.YHIMS_SBBEDENI;
                //        sigortasoru.Bedel = Convert.ToDouble(IMMBedel.BedeniSahis);
                //        SigortaKonusuList.Add(sigortasoru);

                //        sigortasoru = new groupama.service.SigortaKonusu();
                //        sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.YHIMS_KBBEDENİ;
                //        sigortasoru.Bedel = Convert.ToDouble(IMMBedel.BedeniKaza);
                //        SigortaKonusuList.Add(sigortasoru);

                //        sigortasoru = new groupama.service.SigortaKonusu();
                //        sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.YHIMS_MADDİ;
                //        sigortasoru.Bedel = Convert.ToDouble(IMMBedel.Maddi);
                //        SigortaKonusuList.Add(sigortasoru);

                //        sigortasoru = new groupama.service.SigortaKonusu();
                //        sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.YHIMS_MADDİ_BEDENI;
                //        sigortasoru.Bedel = Convert.ToDouble(IMMBedel.Kombine); //???
                //        SigortaKonusuList.Add(sigortasoru);
                //    }
                //}

                #region Araç aksesuarlar

                if (teklif.AracEkSorular.Count > 0)
                {
                    List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR)
                                                             .ToList<TeklifAracEkSoru>();

                    if (aksesuarlar.Count > 0)
                    {
                        sigortasoru = new groupama.service.SigortaKonusu();
                        foreach (TeklifAracEkSoru item in aksesuarlar)
                        {
                            if (item.SoruKodu == MapfreAksesuarTipleri.Kasa)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.KASA_TANKER;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Diger)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.DIGER_EK_CİHAZLAR;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                        }
                    }
                }

                #endregion

                //-------------------- Ekranda Yok Web Servise Gönderilecek mi
                //RADYO/TEYP/CD PLAYER  +
                //KOLONLAR              +
                //EMTEA
                //DİĞER EK CİHAZLAR     +
                //KASA / TANKER         +
                //TRANSMİKSER BEDELİ
                //ARAÇ HUKUKSAL KORUMA  +
                //SÜRÜCÜ HUKUKSAL KOR.  +
                //T.M.M.
                //YHİMS KB.MANEVİ TAZ.

                //sigortasoru = new groupama.service.SigortaKonusu();
                //sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.EMTEA; 
                //sigortasoru.Bedel = 0;
                //SigortaKonusuList.Add(sigortasoru);

                //sigortasoru = new groupama.service.SigortaKonusu();
                //sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.TMM; 
                //sigortasoru.Bedel = 0;
                //SigortaKonusuList.Add(sigortasoru);

                //sigortasoru = new groupama.service.SigortaKonusu();
                //sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.YHİMS_KB_MANEVİ_TAZ;
                //sigortasoru.Bedel = 0;
                //SigortaKonusuList.Add(sigortasoru);

                //sigortasoru = new groupama.service.SigortaKonusu();
                //sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.TRANSMIKSER_BEDELI; 
                //sigortasoru.Bedel = 0;
                //SigortaKonusuList.Add(sigortasoru);

                sigortasoru = new groupama.service.SigortaKonusu();
                sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.ARAC_HUKUKSAL_KORUMA; //Bedel ekrandan girilecek mi
                sigortasoru.Bedel = 0;
                SigortaKonusuList.Add(sigortasoru);

                sigortasoru = new groupama.service.SigortaKonusu();
                sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.SURUCU_HUKUKSAL_KOR;//Bedel ekrandan girilecek mi
                sigortasoru.Bedel = 0;
                SigortaKonusuList.Add(sigortasoru);

                //PRİM KORUMA
                bool primKoruma = teklif.ReadSoru(KaskoSorular.PrimKoruma, false);
                if (primKoruma)
                {
                    sigortasoru = new groupama.service.SigortaKonusu();
                    sigortasoru.Id = Groupoma_KlasikKaskoSorular_SigortaKonusuListesi.PRIM_KORUMA;
                    sigortasoru.Bedel = 0;
                    SigortaKonusuList.Add(sigortasoru);
                }
            }
            #endregion

            #region Elit kasko

            else if (kaskoTuru == GroupamaUrunKodlari.ElitKasko)
            {
                bool alarm = teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false);
                if (alarm)
                {
                    sigortasoru = new groupama.service.SigortaKonusu();
                    sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.ALARM;
                    sigortasoru.Bedel = 0;
                    SigortaKonusuList.Add(sigortasoru);
                }
                if (teklif.ReadSoru(KaskoSorular.LPG_VarYok, false))
                {
                    if (!teklif.ReadSoru(KaskoSorular.LPG_Arac_Orjinalmi, true))
                    {
                        sigortasoru = new groupama.service.SigortaKonusu();
                        sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.LPG;
                        sigortasoru.Bedel = Convert.ToDouble(teklif.ReadSoru(KaskoSorular.LPG_Bedel, 0));
                        SigortaKonusuList.Add(sigortasoru);
                    }
                }


                #region Araç aksesuarlar

                if (teklif.AracEkSorular.Count > 0)
                {
                    List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR)
                                                             .ToList<TeklifAracEkSoru>();

                    if (aksesuarlar.Count > 0)
                    {
                        sigortasoru = new groupama.service.SigortaKonusu();
                        foreach (TeklifAracEkSoru item in aksesuarlar)
                        {
                            if (item.SoruKodu == MapfreAksesuarTipleri.LPG)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.LPG;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Jant)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.CELIK_JANT;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                            if (item.SoruKodu == MapfreAksesuarTipleri.Diger)
                            {
                                sigortasoru = new groupama.service.SigortaKonusu();
                                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.DIGER_EK_CIHAZLAR;
                                sigortasoru.Bedel = Convert.ToDouble(item.Bedel);
                                SigortaKonusuList.Add(sigortasoru);
                            }
                        }

                    }
                }

                #endregion

                bool primKoruma = teklif.ReadSoru(KaskoSorular.PrimKoruma, false);
                if (primKoruma)
                {
                    sigortasoru = new groupama.service.SigortaKonusu();
                    sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.PRIM_KORUMA;
                    sigortasoru.Bedel = 0;
                    SigortaKonusuList.Add(sigortasoru);
                }

            }
            #endregion

            #region Ortak Ürün Sigorta Konusu Soruları
            if (teklif.AracEkSorular.Count > 0)
            {

                List<TeklifAracEkSoru> elektronikCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                                                               .ToList<TeklifAracEkSoru>();
                int CihazBedel = 0;
                if (elektronikCihazlar.Count > 0)
                {
                    foreach (TeklifAracEkSoru item in elektronikCihazlar)
                    {
                        if (item.SoruKodu == MapfreElektronikCihazTipleri.RT)
                        {
                            CihazBedel = item.Bedel.HasValue ? Convert.ToInt32(item.Bedel.Value) : 0;

                            sigortasoru = new groupama.service.SigortaKonusu();
                            sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.RADYO_TEYP_CD_PLAYER;
                            sigortasoru.Bedel = Convert.ToDouble(CihazBedel);
                            SigortaKonusuList.Add(sigortasoru);
                        }
                    }
                }
            }

            bool kolonlar = teklif.ReadSoru(KaskoSorular.Kolonlar, false);
            if (kolonlar)
            {
                string kolonBedel = teklif.ReadSoru(KaskoSorular.KolonBedel, "0");

                sigortasoru = new groupama.service.SigortaKonusu();
                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.KOLONLAR;
                sigortasoru.Bedel = Convert.ToDouble(kolonBedel);
                SigortaKonusuList.Add(sigortasoru);
            }



            //Ürün sigorta konusu listesinden kaldırılmış. //17/04/2017
            int modelYili = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : 0;
            //AracModel aracModel = _AracService.GetAracModel(teklif.Arac.Marka, teklif.Arac.AracinTipi, modelYili);

            if (teklif.Arac.AracDeger.HasValue)
            {
                sigortasoru = new groupama.service.SigortaKonusu();
                sigortasoru.Id = Groupoma_HususiOtoKaskoSigortaKonusuListesi.ARAC;
                sigortasoru.Bedel = Convert.ToDouble(teklif.Arac.AracDeger.Value);
                SigortaKonusuList.Add(sigortasoru);
            }


            #endregion

            return SigortaKonusuList;
        }

        public List<groupama.service.Teminat> GroupamaTeminat(ITeklif teklif, int kaskoTuru)
        {
            List<groupama.service.Teminat> SigortaTeminatList = new List<groupama.service.Teminat>();

            groupama.service.Teminat teminat = new groupama.service.Teminat();

            if (kaskoTuru == GroupamaUrunKodlari.HususiOtoKasko)
            {
                teminat = new groupama.service.Teminat();
                teminat.Id = 12;

                SigortaTeminatList.Add(teminat);

                teminat = new groupama.service.Teminat();
                teminat.Id = 419;

                SigortaTeminatList.Add(teminat);

                teminat = new groupama.service.Teminat();
                teminat.Id = 4;

                SigortaTeminatList.Add(teminat);

                teminat = new groupama.service.Teminat();
                teminat.Id = 5;

                SigortaTeminatList.Add(teminat);

                teminat = new groupama.service.Teminat();
                teminat.Id = 6;

                SigortaTeminatList.Add(teminat);

                teminat = new groupama.service.Teminat();
                teminat.Id = 11;

                bool primKoruma = teklif.ReadSoru(KaskoSorular.PrimKoruma, false);
                if (primKoruma)
                {
                    SigortaTeminatList.Add(teminat);
                    teminat = new groupama.service.Teminat();
                    teminat.Id = Groupoma_HususiOtoKaskoSorular_TeminatListesi.PRIM_KORUMA;

                    SigortaTeminatList.Add(teminat);
                }

                bool yurtDisiKasko = teklif.ReadSoru(KaskoSorular.YurtdisiKasko, false);
                if (yurtDisiKasko)
                {
                    SigortaTeminatList.Add(teminat);
                    teminat = new groupama.service.Teminat();
                    teminat.Id = Groupoma_HususiOtoKaskoSorular_TeminatListesi.YURT_DISI_KASKO;

                    SigortaTeminatList.Add(teminat);
                }
            }
            else
            {
                //Amortisan eklenecek mi
                teminat = new groupama.service.Teminat();
                teminat.Id = Groupoma_KlasikKaskoSorular_TeminatListesi.AMORTISMAN;
                teminat.Aciklama = "AMORTİSMAN";
                SigortaTeminatList.Add(teminat);

                //ANAHTAR KAYBI ZARARLARI eklenecek mi
                teminat = new groupama.service.Teminat();
                teminat.Id = Groupoma_KlasikKaskoSorular_TeminatListesi.ANAHTAR_KAYBI_ZARARLARI;
                teminat.Aciklama = "ANAHTAR KAYBI ZARARLARI";
                SigortaTeminatList.Add(teminat);

                //GREV-LOKAVT-KARGAŞALIK-HALK HAREKETLERİ eklenecek mi
                teminat = new groupama.service.Teminat();
                teminat.Id = Groupoma_KlasikKaskoSorular_TeminatListesi.GREV_LOKAVT_KARGASALIK_HALK_HAREKETLERI;
                teminat.Aciklama = "GREV-LOKAVT-KARGAŞALIK-HALK HAREKETLERİ";
                SigortaTeminatList.Add(teminat);

                //TERÖR-SABOTAJ eklenecek mi 
                teminat = new groupama.service.Teminat();
                teminat.Id = Groupoma_KlasikKaskoSorular_TeminatListesi.TEROR_SABOTAJ;
                teminat.Aciklama = "TERÖR-SABOTAJ";
                SigortaTeminatList.Add(teminat);

                //SEL - SU eklenecek mi
                teminat = new groupama.service.Teminat();
                teminat.Id = Groupoma_KlasikKaskoSorular_TeminatListesi.SEL_SU;
                teminat.Aciklama = "SEL - SU";
                SigortaTeminatList.Add(teminat);

                //DEPREM eklenecek mi
                teminat = new groupama.service.Teminat();
                teminat.Id = Groupoma_KlasikKaskoSorular_TeminatListesi.DEPREM;
                teminat.Aciklama = "DEPREM";
                SigortaTeminatList.Add(teminat);

                //KULLANIM VE GELİR KAYBI EK TEMİNATI eklenecek mi
                teminat = new groupama.service.Teminat();
                teminat.Id = Groupoma_KlasikKaskoSorular_TeminatListesi.KULLANIM_VE_GELIR_KAYBI_EK_TEMINATI;
                teminat.Aciklama = "KULLANIM VE GELİR KAYBI EK TEMİNATI";
                SigortaTeminatList.Add(teminat);

                //PATLAYICI MADDDELER eklenecek mi
            }

            //Ortak Teminatlar
            //bool primKoruma = teklif.ReadSoru(KaskoSorular.PrimKoruma, false);
            //if (primKoruma)
            //{
            //    teminat = new groupama.service.Teminat();
            //    teminat.Id = Groupoma_HususiOtoKaskoSorular_TeminatListesi.PRIM_KORUMA;
            //    teminat.Aciklama = "PRİM KORUMA";
            //    SigortaTeminatList.Add(teminat);
            //}


            //bool yurtDisiKasko = teklif.ReadSoru(KaskoSorular.YurtdisiKasko, false);
            //if (yurtDisiKasko)
            //{
            //    teminat = new groupama.service.Teminat();
            //    teminat.Id = Groupoma_HususiOtoKaskoSorular_TeminatListesi.YURT_DISI_KASKO;
            //    teminat.Aciklama = "YURT DIŞI KASKO";
            //    SigortaTeminatList.Add(teminat);
            //}
            return SigortaTeminatList;
        }

        public groupama.service.Musteri SigortaliBilgileri(ITeklif teklif, groupama.service.BGS_WS client)
        {
            MusteriGenelBilgiler sigortaliBilgi = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriAdre adres = _MusteriService.GetDefaultAdres(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriTelefon cepTel = null;
            MusteriTelefon evTel = null;
            if (sigortaliBilgi.MusteriTelefons != null)
            {
                cepTel = sigortaliBilgi.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                evTel = sigortaliBilgi.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            }

            groupama.service.Musteri sigortali = new groupama.service.Musteri();

            sigortali.kodu = 0;
            //sigortali.adi = sigortaliBilgi.AdiUnvan;
            //sigortali.soyadi = sigortaliBilgi.SoyadiUnvan;

            if (sigortaliBilgi.KimlikNo.Length == 11)
            {
                sigortali.tckNo = Convert.ToInt64(sigortaliBilgi.KimlikNo);
                sigortali.uyruk = UYRUK.YERLI;
                //sigortali.vknNo = 0;
            }
            else if (sigortaliBilgi.KimlikNo.Length == 10)
            {
                //sigortali.tckNo = 0;
                sigortali.vknNo = Convert.ToInt64(sigortaliBilgi.KimlikNo);
                sigortali.uyruk = UYRUK.YERLI;
            }
            else
            {
                sigortali.pasaportNo = sigortaliBilgi.KimlikNo;
                sigortali.uyruk = UYRUK.YABANCI;
            }

            sigortali.sifati = 1;

            if (cepTel != null && cepTel.Numara.Length > 10)
            {
                sigortali.gsm = cepTel.Numara.Substring(3, 3);
                sigortali.gsm += cepTel.Numara.Substring(7, 7);
            }
            else
            {
                sigortali.gsm = "5559999999";
            }
            if (evTel != null && evTel.Numara.Length>10)
            {
                sigortali.telefon = evTel.Numara.Substring(3, 3);
                sigortali.telefon += evTel.Numara.Substring(7, 7);
            }
            //else
            //{

            //    sigortali.telefon = "5559999999";

            //}
            if (sigortaliBilgi.MusteriTipKodu == MusteriTipleri.TCMusteri)
            {
                sigortali.tip = TIP.OZEL;
            }
            else if (sigortaliBilgi.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
            {
                sigortali.tip = TIP.TUZEL;
            }
            sigortali.email = sigortaliBilgi.EMail;

            Adres_Tip1 adr1 = new Adres_Tip1();
            adr1.adresTipi = AdresTipi.Tip1;

            var ilResponse = client.GetIlIlceKoyListesi(TYPE.IL, 0, 0);
            int musteriIlKodu = !String.IsNullOrEmpty(adres.IlKodu) ? Convert.ToInt32(adres.IlKodu) : 34;
            string GroupamaIlAdi = "İSTANBUL";
            if (ilResponse != null)
            {
                var iller = ilResponse.Where(s => s.ilKodu == musteriIlKodu).FirstOrDefault();
                if (iller != null)
                {
                    GroupamaIlAdi = iller.adi;
                }
            }
            adr1.il = GroupamaIlAdi;
            var ilceResponse = client.GetIlIlceKoyListesi(TYPE.ILCE, musteriIlKodu, 0);
            int musteriIlceKodu = !String.IsNullOrEmpty(adres.IlKodu) ? Convert.ToInt32(adres.IlKodu) : 34;
            string GroupamaIlceAdi = "MERKEZ";
            if (ilceResponse != null)
            {
                string musteriIlceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(adres.IlceKodu));

                var ilceler = ilceResponse.Where(s => s.adi == musteriIlceAdi).FirstOrDefault();
                if (ilceler != null)
                {
                    GroupamaIlceAdi = ilceler.adi;
                }
            }

            adr1.ilce = GroupamaIlceAdi;
            sigortali.adres = adr1;

            return sigortali;

        }

        public groupama.service.Musteri SigortaEttirenBilgileri(ITeklif teklif, groupama.service.BGS_WS client)
        {
            MusteriGenelBilgiler sigortaEttirenBilgi = _MusteriService.GetMusteri(teklif.SigortaEttiren.MusteriKodu);
            MusteriAdre adres = _MusteriService.GetDefaultAdres(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriTelefon cepTel = null;
            MusteriTelefon evTel = null;
            if (sigortaEttirenBilgi.MusteriTelefons != null)
            {
                cepTel = sigortaEttirenBilgi.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                evTel = sigortaEttirenBilgi.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);

            }
            groupama.service.Musteri sigortaEttiren = new groupama.service.Musteri();

            sigortaEttiren.kodu = 0;
            //sigortaEttiren.adi = sigortaEttirenBilgi.AdiUnvan;
            //sigortaEttiren.soyadi = sigortaEttirenBilgi.SoyadiUnvan;

            if (sigortaEttirenBilgi.KimlikNo.Length == 11)
            {
                sigortaEttiren.tckNo = Convert.ToInt64(sigortaEttirenBilgi.KimlikNo);
                sigortaEttiren.uyruk = UYRUK.YERLI;
                // sigortaEttiren.vknNo = 0;
            }
            else if (sigortaEttirenBilgi.KimlikNo.Length == 10)
            {
                //  sigortaEttiren.tckNo = 0;
                sigortaEttiren.vknNo = Convert.ToInt64(sigortaEttirenBilgi.KimlikNo);
                sigortaEttiren.uyruk = UYRUK.YERLI;
            }
            else
            {
                sigortaEttiren.pasaportNo = sigortaEttirenBilgi.KimlikNo;
                sigortaEttiren.uyruk = UYRUK.YABANCI;
            }
            if (sigortaEttirenBilgi.MusteriTipKodu == MusteriTipleri.TCMusteri)
            {
                sigortaEttiren.tip = TIP.OZEL;
            }
            else if (sigortaEttirenBilgi.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
            {
                sigortaEttiren.tip = TIP.TUZEL;
            }
            sigortaEttiren.sifati = 1;
            if (cepTel != null)
            {
                sigortaEttiren.gsm = cepTel.Numara.Substring(3, 3);
                sigortaEttiren.gsm += cepTel.Numara.Substring(7, 7);
            }
            if (evTel != null)
            {
                sigortaEttiren.telefon = evTel.Numara.Substring(3, 3);
                sigortaEttiren.telefon += evTel.Numara.Substring(7, 7);
            }
            //else
            //{
            //    sigortaEttiren.telefon = "5559999999";
            //}

            sigortaEttiren.email = sigortaEttirenBilgi.EMail;

            Adres_Tip1 adr1 = new Adres_Tip1();
            adr1.adresTipi = AdresTipi.Tip1;

            var ilResponse = client.GetIlIlceKoyListesi(TYPE.IL, 0, 0);
            int musteriIlKodu = !String.IsNullOrEmpty(adres.IlKodu) ? Convert.ToInt32(adres.IlKodu) : 34;
            string GroupamaIlAdi = "İSTANBUL";
            if (ilResponse != null)
            {
                var iller = ilResponse.Where(s => s.ilKodu == musteriIlKodu).FirstOrDefault();
                if (iller != null)
                {
                    GroupamaIlAdi = iller.adi;
                }
            }
            adr1.il = GroupamaIlAdi;
            var ilceResponse = client.GetIlIlceKoyListesi(TYPE.ILCE, musteriIlKodu, 0);
            int musteriIlceKodu = !String.IsNullOrEmpty(adres.IlKodu) ? Convert.ToInt32(adres.IlKodu) : 34;
            string GroupamaIlceAdi = "MERKEZ";
            if (ilceResponse != null)
            {
                string musteriIlceAdi = _UlkeService.GetIlceAdi(Convert.ToInt32(adres.IlceKodu));

                var ilceler = ilResponse.Where(s => s.adi == musteriIlceAdi).FirstOrDefault();
                if (ilceler != null)
                {
                    GroupamaIlceAdi = ilceler.adi;
                }
            }

            adr1.ilce = GroupamaIlceAdi;
            sigortaEttiren.adres = adr1;

            return sigortaEttiren;
        }

        public List<GroupamaResponse> KazaDestekTeminatLimitleri()
        {
            groupama.service.BGS_WS clntKasko = new groupama.service.BGS_WS();
            List<GroupamaResponse> list = new List<GroupamaResponse>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGROUPAMAService);
                clntKasko.Url = konfig[Konfig.GROUPAMA_ServiceURL];
                clntKasko.Timeout = 250000;
                GroupamaResponse teminat = new GroupamaResponse();

                var response = clntKasko.GetCevapDegerSeti(3428, 3, 151);
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        teminat = new GroupamaResponse();
                        teminat.kodu = item.CevapKodu.ToString();
                        teminat.aciklama = item.Aciklama;
                        list.Add(teminat);
                    }
                }
            }
            catch (Exception)
            {
                return list;
                throw;
            }

            return list;
        }

        public List<GroupamaResponse> YHIMSBasamakLimitleri()
        {
            groupama.service.BGS_WS clntKasko = new groupama.service.BGS_WS();
            List<GroupamaResponse> list = new List<GroupamaResponse>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGROUPAMAService);
                clntKasko.Url = konfig[Konfig.GROUPAMA_ServiceURL];
                clntKasko.Timeout = 250000;


                GroupamaResponse YHIMSBasamak = new GroupamaResponse();

                var response = clntKasko.GetCevapDegerSeti(3283, 3, 47);
                if (response != null)
                {
                    foreach (var item in response)
                    {
                        YHIMSBasamak = new GroupamaResponse();
                        YHIMSBasamak.kodu = item.CevapKodu.ToString();
                        YHIMSBasamak.aciklama = item.Aciklama;
                        list.Add(YHIMSBasamak);
                    }
                }
            }
            catch (Exception)
            {
                return list;
                throw;
            }

            return list;
        }

        public string PolicePDF(groupama.service.BGS_WS clntPolice, TVMWebServisKullanicilari servisKullanici, string TUMPoliceNo, string pdfType)
        {
            try
            {
                PrintRequest request = new PrintRequest();
                request.acente = new AcenteBilgi()
                {
                    acenteKodu = Convert.ToInt64(servisKullanici.KullaniciAdi),
                    kullaniciKodu = Convert.ToInt64(servisKullanici.Sifre),
                    type = ACENTE_TYPE.OZEL
                };
                if (pdfType == "police")
                {
                    request.type = PRINT_TYPE.POLICE;
                }
                else if (pdfType == "policeBilgilendirme")
                {
                    request.type = PRINT_TYPE.POLICE_BF;
                }
                request.teklifId = 0;
                request.policeNo = Convert.ToInt64(TUMPoliceNo);

                string GroupamaTeklifVersiyon = this.ReadWebServisCevap(Common.WebServisCevaplar.GROUPAMA_TeklifVersiyonNo, "0");
                request.teklifVersiyon = Convert.ToInt32(GroupamaTeklifVersiyon);

                this.BeginLog(request, typeof(PrintRequest), WebServisIstekTipleri.Police);
                var response = clntPolice.getPrintURL(request);
                clntPolice.Dispose();
                if (response.responseHeader.answerCode == 100)
                {
                    WebClient myClient = new WebClient();
                    byte[] data = myClient.DownloadData(response.printURL);

                    IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                    string fileName = "";
                    if (pdfType == "police")
                    {
                        fileName = String.Format("Groupama_Kasko_police_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                    }
                    else if (pdfType == "policeBilgilendirme")
                    {
                        fileName = String.Format("Groupama_Kasko_policeBilgilendirme_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                    }

                    string url = storage.UploadFile("kasko", fileName, data);
                    return url;
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                _Log.Error("GroupamaKasko.PolicePDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                return String.Empty;
            }
        }

    }
}
