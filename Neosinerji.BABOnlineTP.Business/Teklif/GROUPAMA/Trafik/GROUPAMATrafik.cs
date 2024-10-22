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
    public class GROUPAMATrafik : Teklif, IGROUPAMATrafik
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
        IUlkeService _UlkeService;
        [InjectionConstructor]
        public GROUPAMATrafik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService, IUlkeService ulkeService)
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
            _UlkeService = ulkeService;
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
                groupama.service.BGS_WS clntTrafik = new groupama.service.BGS_WS();

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGROUPAMAService);
                clntTrafik.Url = konfig[Konfig.GROUPAMA_ServiceURL];
                clntTrafik.Timeout = 250000;
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.GROUPAMA });
                #endregion

                #region Request

                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
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

                TeklifCreateRequest trafikRequest = new TeklifCreateRequest()
                {
                    teklifId = 0,
                    teklifVersiyon = 0,
                    //ip = ClientIPNo,
                    ip="94.54.88.114",
                    bgsUrunKodu = GroupamaUrunKodlari.Trafik,
                    acente = new AcenteBilgi()
                    {
                        acenteKodu = Convert.ToInt64(servisKullanici.KullaniciAdi),
                        kullaniciKodu = Convert.ToInt64(servisKullanici.Sifre),
                        type = ACENTE_TYPE.OZEL
                    },

                    sigortali = this.SigortaliBilgileri(teklif, clntTrafik),
                    sigortaEttiren = this.SigortaEttirenBilgileri(teklif, clntTrafik),

                    teklifZamani = System.DateTime.Now,
                    teklifBaslangicTarihi = new DateTime(polBasYil, polBasAy, polBasGun),
                    teklifBitisTarihi = new DateTime(polBitisYil, polBitisAy, polBitisGun),
                    dovizKodu = 0, // 0 TL                   
                };
                trafikRequest.soruListesi = this.GroupamaTeklifSorular(teklif).ToArray();

                trafikRequest.odemeBilgileri = new OdemeBilgileri()
                {
                    odemeTipi = ODEMETYPE.KrediKarti,
                    odemePlanNo = 86,

                };
                trafikRequest.bankaHesapBilgileri = new BankaHesapBilgileri()
                {
                    bankaHesapNo = "0",
                    bankaKodu = 0,
                    krediReferansNo = "0",
                    personelSicilNo = 0,
                    subeKodu = 0
                };

                trafikRequest.adresTipi = AdresTipi.Tip0;

                this.BeginLog(trafikRequest, trafikRequest.GetType(), WebServisIstekTipleri.Teklif);

                #endregion

                #region Response
                var response = clntTrafik.createTeklif(trafikRequest);
                if (response != null)
                {
                    if (response.responseHeader.answerCode == 100) //100=Servis Başarılı Kodu
                    {
                        this.EndLog(response, true, response.GetType());
                    }
                    else
                    {
                        this.EndLog(response, false, response.GetType());
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
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                this.GenelBilgiler.DovizTL = Convert.ToByte(response.teklif.dovizKodu);
                this.GenelBilgiler.BaslamaTarihi = response.teklif.teklifBaslangicTarihi;
                this.GenelBilgiler.BitisTarihi = response.teklif.teklifBitisTarihi;
                this.GenelBilgiler.TanzimTarihi = response.teklif.teklifZamani;
                var tarifeBasamagiResponse = response.teklif.soruListesi.Where(s => s.Id == Groupama_TrafikSorular.TarifeBasamagi).FirstOrDefault();

                if (tarifeBasamagiResponse != null)
                {
                    var tarifeBasamakKodu = tarifeBasamagiResponse.DecimalDeger;
                    if (tarifeBasamakKodu == 4)
                    {
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;

                    }
                    else if (tarifeBasamakKodu == 1)
                    {
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                        this.GenelBilgiler.HasarSurprimYuzdesi = 40;
                    }
                    else if (tarifeBasamakKodu == 2)
                    {
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                        this.GenelBilgiler.HasarSurprimYuzdesi = 20;
                    }
                    else if (tarifeBasamakKodu == 3)
                    {
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                        this.GenelBilgiler.HasarSurprimYuzdesi = 10;
                    }
                    else if (tarifeBasamakKodu == 5)
                    {
                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 10;
                    }
                    else if (tarifeBasamakKodu == 6)
                    {
                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 15;
                    }
                    else if (tarifeBasamakKodu == 7)
                    {
                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 20;
                    }
                }
                string TRAMER_TARIFE_KODU = string.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ANADOLU &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        TRAMER_TARIFE_KODU = kullanimTarzi.TarifeKodu;
                    }
                }
                #region Temiant

                var maxBaslamaTarih = _AracContext.AracTrafikTeminatRepository.All().Max(f => f.GecerlilikBaslamaTarihi);
                var teminat = _AracContext.AracTrafikTeminatRepository.Filter(f => f.GecerlilikBaslamaTarihi <= this.GenelBilgiler.BaslamaTarihi &&
                                                                                    f.GecerlilikBaslamaTarihi >= maxBaslamaTarih &&
                                                                                    f.AracGrupKodu == TRAMER_TARIFE_KODU)
                                                                        .FirstOrDefault();

                if (teminat != null)
                {
                    this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, teminat.MaddiAracBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, teminat.MaddiKazaBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, teminat.TedaviKisiBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, teminat.TedaviKazaBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, teminat.SakatlikKisiBasina.Value, 0, 0, 0, 0);
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, teminat.SakatlikKazaBasina.Value, 0, 0, 0, 0);
                }
                #endregion
                #region Teklif PDF

                string pdfURL = this.TeklifPDF(clntTrafik, servisKullanici);
                if (!String.IsNullOrEmpty(pdfURL))
                {
                    this.GenelBilgiler.PDFDosyasi = pdfURL;
                }
                #endregion
                #endregion

                #region Web servis cevapları

                this.AddWebServisCevap(Common.WebServisCevaplar.GROUPAMA_TeklifVersiyonNo, response.teklif.teklifVersiyon);

                #endregion

                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }


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
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleGROUPAMAService);

            groupama.service.BGS_WS clntTrafik = new groupama.service.BGS_WS();
            clntTrafik.Url = konfig[Konfig.GROUPAMA_ServiceURL];
            clntTrafik.Timeout = 150000;

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

            if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
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

            var response = clntTrafik.createPolice(request);
            clntTrafik.Dispose();

            if (response != null)
            {
                if (response.responseHeader.answerCode == 100) //100=Servis Başarılı Kodu
                {
                    this.EndLog(response, true, response.GetType());

                    this.GenelBilgiler.TUMPoliceNo = response.policeNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    string policePDFURL = this.PolicePDF(clntTrafik, servisKullanici, this.GenelBilgiler.TUMPoliceNo, "police");
                    if (!String.IsNullOrEmpty(policePDFURL))
                    {
                        this.GenelBilgiler.PDFPolice = policePDFURL;
                    }
                    string policeBilgilendirmeURL = this.PolicePDF(clntTrafik, servisKullanici, this.GenelBilgiler.TUMPoliceNo, "policeBilgilendirme");
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

        public string TeklifPDF(groupama.service.BGS_WS clntTrafik, TVMWebServisKullanicilari servisKullanici)
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
                var response = clntTrafik.getPrintURL(request);
                clntTrafik.Dispose();
                if (response.responseHeader.answerCode == 100)
                {
                    WebClient myClient = new WebClient();
                    byte[] data = myClient.DownloadData(response.printURL);

                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Format("Groupama_Trafik_Teklif_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                    string url = storage.UploadFile("trafik", fileName, data);
                    return url;
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                _Log.Error("GroupamaTrafik.TeklifPDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                return String.Empty;
            }
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
                        fileName = String.Format("Groupama_Trafik_police_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                    }
                    else if (pdfType == "policeBilgilendirme")
                    {
                        fileName = String.Format("Groupama_Trafik_policeBilgilendirme_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                    }

                    string url = storage.UploadFile("trafik", fileName, data);
                    return url;
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                _Log.Error("GroupamaTrafik.PolicePDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                return String.Empty;
            }
        }

        public List<groupama.service.Soru> GroupamaTeklifSorular(ITeklif teklif)
        {
            List<groupama.service.Soru> soruList = new List<groupama.service.Soru>();

            int GroupamaKullanimTarziKodu = 1;
            string[] parts = teklif.Arac.KullanimTarzi.Split('-');
            if (parts.Length == 2)
            {
                string kullanimTarziKodu = parts[0];
                string kod2 = parts[1];
                CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.GROUPAMA &&
                                                                                                f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                f.Kod2 == kod2)
                                                                                                .SingleOrDefault<CR_KullanimTarzi>();

                if (kullanimTarzi != null)
                    GroupamaKullanimTarziKodu = Convert.ToInt32(kullanimTarzi.TarifeKodu);
            }

            groupama.service.Soru soru = null;
            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.KullanimTarzi; ;
            soru.DecimalDeger = Convert.ToDouble(GroupamaKullanimTarziKodu); // <KarakterDeger>kullanimTarzi.otomobil</KarakterDeger> 
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.MarkaTip; //MARKA & TİP      //YARIS 1.3 SOL (ESKI KASA)                        
            soru.DecimalDeger = Convert.ToDouble(teklif.Arac.Marka + teklif.Arac.AracinTipi.PadLeft(4,'0'));
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.Model;
            soru.DecimalDeger = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.YerAdedi;
            soru.DecimalDeger = teklif.Arac.KoltukSayisi.HasValue ? (teklif.Arac.KoltukSayisi.Value - 1) : 4;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.MotorNo;
            soru.KarakterDeger = teklif.Arac.MotorNo;
            soru.DecimalDeger = 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.SasiNo;
            soru.KarakterDeger = teklif.Arac.SasiNo;
            soru.DecimalDeger = 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.PlakaNumarasi;
            soru.KarakterDeger = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
            soru.DecimalDeger = 0;
            soruList.Add(soru);

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.PlakaIlKodu;
            soru.KarakterDeger = teklif.Arac.PlakaKodu;
            soru.DecimalDeger = 0;
            soruList.Add(soru);

            double trafikTecilTarihi = 0;
            if (teklif.Arac.TrafikTescilTarihi.HasValue)
            {
                string tarih = teklif.Arac.TrafikTescilTarihi.Value.ToString("ddMMyyyy");
                if (!String.IsNullOrEmpty(tarih))
                {
                    soru = new groupama.service.Soru();
                    soru.Id = Groupama_TrafikSorular.TrafigeCikisTarihi;
                    trafikTecilTarihi = Convert.ToDouble(tarih.Substring(0, 2) + tarih.Substring(2, 2) + tarih.Substring(4, 4));
                    soru.DecimalDeger = trafikTecilTarihi;
                    soruList.Add(soru);
                }
            }

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.RuhsatDuzeltmeTarihi;
            soru.DecimalDeger = trafikTecilTarihi;
            soruList.Add(soru);

            if (teklif.GenelBilgiler.TarifeBasamakKodu.HasValue)
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.TarifeBasamagi;
                soru.DecimalDeger = teklif.GenelBilgiler.TarifeBasamakKodu.Value;
                soruList.Add(soru);
            }
            if (!String.IsNullOrEmpty(teklif.Arac.TramerBelgeNo))
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.TramerBelgeNo;
                soru.DecimalDeger = Convert.ToDouble(teklif.Arac.TramerBelgeNo);
                soruList.Add(soru);
            }

            if (teklif.Arac.TramerBelgeTarihi.HasValue)
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.TramerBelgeTarihi;

                string tarih = teklif.Arac.TramerBelgeTarihi.Value.ToString("ddMMyyyy");
                var TramerBelgeTarihi = Convert.ToDouble(tarih.Substring(0, 2) + tarih.Substring(2, 2) + tarih.Substring(4, 4));

                soru.DecimalDeger = TramerBelgeTarihi;
                soruList.Add(soru);
            }

            if (teklif.Arac.TescilSeriKod != null)
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.TescilSeriKodu;
                if (teklif.Arac.TescilSeriKod != null)
                {
                    soru.KarakterDeger = teklif.Arac.TescilSeriKod;
                    soru.DecimalDeger = 0;
                }
                soruList.Add(soru);
            }

            if (teklif.Arac.TescilSeriNo != null || teklif.Arac.AsbisNo != null)
            {
                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.TescilSeriNo;
                soru.KarakterDeger = teklif.Arac.TescilSeriNo != null ? teklif.Arac.TescilSeriNo : teklif.Arac.AsbisNo;
                soru.DecimalDeger = 0;

                soruList.Add(soru);
            }

            #region Önceki Poliçe
            bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
            string oncekiAcenteNo = String.Empty;
            string oncekiPoliceNo = String.Empty;
            string oncekiSirketKodu = String.Empty;
            string oncekiYenilemeNo = String.Empty;
            string oncekiBasamakKodu = String.Empty;

            if (eskiPoliceVar)
            {
                oncekiAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                oncekiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                oncekiSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                oncekiYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                oncekiBasamakKodu = teklif.ReadSoru(TrafikSorular.UygulanmisTarifeBasamakKodu, String.Empty);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.OncekiAcenteKodu;
                soru.KarakterDeger = oncekiAcenteNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.OncekiPoliceNo;
                soru.KarakterDeger = oncekiPoliceNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.OncekiSirketKodu;
                soru.DecimalDeger = Convert.ToInt32(oncekiSirketKodu);
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.OncekiYenilemeNo;
                soru.KarakterDeger = oncekiYenilemeNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.OncekiTarifeBasamagi;
                soru.KarakterDeger = oncekiBasamakKodu;
                soruList.Add(soru);
            }

            #endregion

            #region Zorunlu Karayolu Tasimacilik MSS

            bool ZorunluKarayoluTasimacilikMSS = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_VarYok, false);

            string ZKTMSSAcenteNo = String.Empty;
            string ZKTMSSPoliceNo = String.Empty;
            string ZKTMSSSirketKodu = String.Empty;
            string ZKTMSSYenilemeNo = String.Empty;

            soru = new groupama.service.Soru();
            soru.Id = Groupama_TrafikSorular.ZorunluKarayoluTasimacilikMSSVar;
            soru.DecimalDeger = 0;
            soru.DefaultDecimalDeger = 2;
            soruList.Add(soru);

            if (ZorunluKarayoluTasimacilikMSS)
            {
                ZKTMSSAcenteNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Acente_No, String.Empty);
                ZKTMSSPoliceNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Police_No, String.Empty);
                ZKTMSSSirketKodu = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Sigorta_Sirketi, String.Empty);
                ZKTMSSYenilemeNo = teklif.ReadSoru(TrafikSorular.Tasiyici_Sorumluluk_Yenileme_No, String.Empty);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.ZKYTMSAcenteNo;
                soru.KarakterDeger = ZKTMSSAcenteNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.ZKYTMSPoliceNo;
                soru.KarakterDeger = ZKTMSSPoliceNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.ZKYTMSSirketkodu;
                soru.KarakterDeger = ZKTMSSSirketKodu;
                soru.DecimalDeger = 0;
                soruList.Add(soru);

                soru = new groupama.service.Soru();
                soru.Id = Groupama_TrafikSorular.ZKYTMSYenilemeNo;
                soru.KarakterDeger = ZKTMSSYenilemeNo;
                soru.DecimalDeger = 0;
                soruList.Add(soru);
            }

            #endregion

            return soruList;
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
            sigortali.adi = sigortaliBilgi.AdiUnvan;
            sigortali.soyadi = sigortaliBilgi.SoyadiUnvan;

            if (sigortaliBilgi.KimlikNo.Length == 11)
            {
                sigortali.tckNo = Convert.ToInt64(sigortaliBilgi.KimlikNo);
                sigortali.uyruk = UYRUK.YERLI;
                sigortali.vknNo = 0;
            }
            else if (sigortaliBilgi.KimlikNo.Length == 10)
            {
                sigortali.tckNo = 0;
                sigortali.vknNo = Convert.ToInt64(sigortaliBilgi.KimlikNo);
                sigortali.uyruk = UYRUK.YERLI;
            }
            else
            {
                sigortali.pasaportNo = sigortaliBilgi.KimlikNo;
                sigortali.uyruk = UYRUK.YABANCI;
            }

            sigortali.sifati = 1;

            if (cepTel != null)
            {
                sigortali.gsm = cepTel.Numara.Substring(3, 3);
                sigortali.gsm += cepTel.Numara.Substring(7, 7);
            }
            else
            {
                sigortali.gsm = "5559999999";
            }
            if (evTel != null)
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
            sigortaEttiren.adi = sigortaEttirenBilgi.AdiUnvan;
            sigortaEttiren.soyadi = sigortaEttirenBilgi.SoyadiUnvan;

            if (sigortaEttirenBilgi.KimlikNo.Length == 11)
            {
                sigortaEttiren.tckNo = Convert.ToInt64(sigortaEttirenBilgi.KimlikNo);
                sigortaEttiren.uyruk = UYRUK.YERLI;
                sigortaEttiren.vknNo = 0;
            }
            else if (sigortaEttirenBilgi.KimlikNo.Length == 10)
            {
                sigortaEttiren.tckNo = 0;
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
    }
}
