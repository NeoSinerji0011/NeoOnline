using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database.Models; 
using Neosinerji.BABOnlineTP.Business.SOMPOJAPAN.Messages;
using Neosinerji.BABOnlineTP.Business.Common.SOMPOJAPANCommon;
using OutSourceLib;

namespace Neosinerji.BABOnlineTP.Business.SOMPOJAPAN
{
    public class SOMPOJAPANKasko : Teklif, ISOMPOJAPANKasko
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
        IAktifKullaniciService _AktifKullaniciService;
        [InjectionConstructor]
        public SOMPOJAPANKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService, IAktifKullaniciService aktifKullaniciService)
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
            _AktifKullaniciService = aktifKullaniciService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.SOMPOJAPAN;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            sompojapan.kasko.Casco clntKasko = new sompojapan.kasko.Casco();
            try
            {
                #region yeni ws
                #region Veri Hazırlama
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.SOMPOJAPAN });
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                clntKasko.IdentityHeaderValue = new sompojapan.kasko.IdentityHeader()
                {
                    KullaniciAdi = servisKullanici.KullaniciAdi,
                    KullaniciParola = servisKullanici.Sifre,
                    KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    KullaniciTipi = Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ClientType.ACENTE
                };

                KonfigTable konfigKasko = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKaskoV2);
                clntKasko.Url = konfigKasko[Konfig.SOMPOJAPAN_CascoServiceURLV2];
                clntKasko.Timeout = 150000;
                #endregion

                #region Genel Bigiler

                sompojapan.kasko.ProposalParameters cascoparameters = new sompojapan.kasko.ProposalParameters();

                //Plaka Bilgileri
                string rakam = "";
                string harf = "";
                foreach (char ch in teklif.Arac.PlakaNo)
                {
                    if (Char.IsDigit(ch))
                        rakam += ch;
                    if (Char.IsLetter(ch))
                        harf += ch;
                }
                cascoparameters.PlateState = teklif.Arac.PlakaKodu;
                cascoparameters.PlateChar = harf;
                cascoparameters.PlateNumber = rakam;
                cascoparameters.TypeCode = 2;// Muafiyetsiz=1  Muafiyetli=2

                #endregion

                #region Sorular

                cascoparameters.Parameters = this.TeklifSorular(teklif, clntKasko, servisKullanici);

                #endregion

                #region Sigortali / Sigorta Ettiren Bilgileri

                cascoparameters.Insured = this.SigortaEttirenBilgileri(teklif, clntKasko);
                cascoparameters.Customer = this.SigortaliBilgileri(teklif, clntKasko);

                #endregion

                #region Service call

                this.BeginLog(cascoparameters, cascoparameters.GetType(), WebServisIstekTipleri.Teklif);

                sompojapan.kasko.MethodResultOfProposalOutput teklifResult = clntKasko.GetProposal(cascoparameters);
                clntKasko.Dispose();
                if (teklifResult.Result == null && teklifResult.ResultCode == sompojapan.kasko.ProposalResultCodes.Fail)
                {
                    if (!String.IsNullOrEmpty(teklifResult.Description))
                    {
                        this.EndLog(teklifResult, true, teklifResult.GetType());
                        this.AddHata(teklifResult.Description);
                    }
                    else
                    {
                        this.EndLog(teklifResult, true, teklifResult.GetType());
                        this.AddHata("Bu ip işlem yapmaya yetkili değildir.");
                    }
                }
                else if (teklifResult.ResultCode == sompojapan.kasko.ProposalResultCodes.Success && teklifResult.Result == null)
                {
                    this.EndLog(teklifResult, true, teklifResult.GetType());
                    this.AddHata("Web Servisten yanıt alınamadı.");
                }
                else if (teklifResult.ResultCode == sompojapan.kasko.ProposalResultCodes.Success && teklifResult.Result != null && teklifResult.Result.RESULT != null)
                {
                    if (teklifResult.Result.RESULT.ERROR != null)
                    {
                        this.EndLog(teklifResult, true, teklifResult.GetType());

                        string hata = String.Empty;
                        var list = teklifResult.Result.RESULT.NOTIFICATION_LIST;
                        if (list != null)
                        {
                            foreach (var item in list)
                            {
                                hata += item.NOTIFICATION_DESCRIPTION;
                            }
                        }
                        this.AddHata(teklifResult.Result.RESULT.ERROR.ERROR_DESCRIPTION + hata);
                    }
                    else
                    {
                        this.EndLog(teklifResult, true, teklifResult.GetType());
                    }
                }
                else
                {
                    this.EndLog(teklifResult, true, teklifResult.GetType());
                }
                #endregion
                #endregion

                #region Eski ws

                //#region Veri Hazırlama

                //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKasko);

                //sompojapankasko.Casco clnt = new sompojapankasko.Casco();
                //clnt.Url = konfig[Konfig.SOMPOJAPAN_CascoServiceURL];
                //clnt.Timeout = 150000;

                //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.SOMPOJAPAN });

                //#endregion

                //#region Genel Bigiler

                //CascoParameters cascoparameters = new CascoParameters();

                //cascoparameters.Username = servisKullanici.KullaniciAdi;
                //cascoparameters.Password = servisKullanici.Sifre;
                //cascoparameters.PlateState = teklif.Arac.PlakaKodu;

                //string rakam = "";
                //string harf = "";
                //foreach (char ch in teklif.Arac.PlakaNo)
                //{
                //    if (Char.IsDigit(ch))
                //        rakam += ch;
                //    if (Char.IsLetter(ch))
                //        harf += ch;
                //}

                //cascoparameters.PlateChar = harf;
                //cascoparameters.PlateNumber = rakam;
                //cascoparameters.TypeCode = 1;// Muafiyetsiz=1  Muafiyetli=2

                //if (String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) && String.IsNullOrEmpty(teklif.Arac.TescilSeriNo) && !String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                //{
                //    cascoparameters.EgmTescilBelgeSeriKod = "";
                //    cascoparameters.EgmTescilBelgeSeriNo = teklif.Arac.AsbisNo;
                //}
                //else
                //{
                //    cascoparameters.EgmTescilBelgeSeriKod = teklif.Arac.TescilSeriKod;
                //    cascoparameters.EgmTescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
                //}

                //#endregion

                //#region Sorular

                //cascoparameters.Parameters = this.TeklifSorular(teklif, servisKullanici, clnt);

                //#endregion

                //#region Sigortali Bilgileri

                //cascoparameters.Unit = this.SigortaliBilgileri(teklif, servisKullanici);

                //#endregion

                //#region Service call

                //this.BeginLog(cascoparameters, cascoparameters.GetType(), WebServisIstekTipleri.Teklif);

                //MethodResultOfCascoResult teklifResult = clnt.GetProposal3(cascoparameters);
                //string nipponHata = teklifResult.Result.Result.resultcode.ToString();
                //if (nipponHata == "Error" || nipponHata == "None")
                //{
                //    this.EndLog(teklifResult, true, teklifResult.GetType());
                //    this.AddHata(teklifResult.Result.Result.description);
                //}
                //else
                //{
                //    this.EndLog(teklifResult, true, teklifResult.GetType());

                //}
                //#endregion
                #endregion

                #region Başarı kontrolu
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
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = Convert.ToDecimal(teklifResult.Result.PAYMENT.GROSS_PREMIUM);
                this.GenelBilgiler.TUMTeklifNo = teklifResult.Result.PROPOSAL_NO.HasValue ? teklifResult.Result.PROPOSAL_NO.Value.ToString() : "";

                #region Teklif PDF
                sompojapan.common.ExtendConfirmParameters confirmRequest = new sompojapan.common.ExtendConfirmParameters();
                sompojapan.common.Common client = new sompojapan.common.Common();
                KonfigTable konfigCommon = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANCommonV2);
                client.Url = konfigCommon[Konfig.SOMPOJAPAN_CommonServiceURLV2]; ;
                client.Timeout = 150000;

                client.IdentityHeaderValue = new sompojapan.common.IdentityHeader()
                {
                    KullaniciAdi = servisKullanici.KullaniciAdi,
                    KullaniciParola = servisKullanici.Sifre,
                    KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    KullaniciTipi = sompojapan.common.ClientType.ACENTE,
                };
                confirmRequest.Policy = new sompojapan.common.Policy()
                {
                    PolicyNumber = teklifResult.Result.PROPOSAL_NO.Value,
                    ProductName = sompojapan.common.ProductName.BireyselKasko,
                    CompanyCode = 0,
                    EndorsNr = 0,
                    FirmCode = 0,
                    RenewalNr = 0,

                };
                string TeklifPDFUrl = this.GetPDFURL(client, confirmRequest, sompojapan.common.PrintoutType.Policy);
                if (!String.IsNullOrEmpty(TeklifPDFUrl))
                {
                    if (TeklifPDFUrl != null)
                    {
                        WebClient myClient = new WebClient();
                        byte[] data = myClient.DownloadData(TeklifPDFUrl);

                        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                        string fileName = String.Empty;
                        string url = String.Empty;
                        fileName = String.Format("SOMPOJAPAN_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                        url = storage.UploadFile("kasko", fileName, data);
                        this.GenelBilgiler.PDFDosyasi = url;
                        _Log.Info("Teklif_PDF url: {0}", url);
                    }
                    else
                    {
                        this.AddHata("PDF dosyası alınamadı.");
                    }
                }
                #endregion


                var SompoVergiKomisyon = teklifResult.Result.PAYMENT.TAXES;
                decimal giderVergi = 0;
                decimal komisyon = 0;

                foreach (var item in SompoVergiKomisyon)
                {
                    if (item.DEDUCTION_DESCRIPTION.Trim() == "Vergiler")
                        giderVergi = item.DEDUCTION_AMOUNT;

                    if (item.DEDUCTION_DESCRIPTION.Trim() == "Komisyonyonlar")
                        komisyon = item.DEDUCTION_AMOUNT;
                }

                this.GenelBilgiler.ToplamVergi = giderVergi;
                this.GenelBilgiler.NetPrim = Convert.ToDecimal(teklifResult.Result.PAYMENT.NET_PREMIUM);
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;

                if (teklifResult.Result.QUESTIONS != null && teklifResult.Result.QUESTIONS.Length > 0)
                {
                    var hasarsizlikKademe = teklifResult.Result.QUESTIONS.FirstOrDefault(s => s.QUESTION_CODE == 224);
                    if (hasarsizlikKademe != null)
                    {
                        string HasarsizlikIndirim = hasarsizlikKademe.ANSWER.Trim();

                        if (!String.IsNullOrEmpty(HasarsizlikIndirim))
                        { this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToInt32(HasarsizlikIndirim); }

                        if (!String.IsNullOrEmpty(HasarsizlikIndirim))
                        {
                            if (HasarsizlikIndirim == "1")
                            {
                                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 30;
                            }
                            else if (HasarsizlikIndirim == "2")
                            {
                                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 40;
                            }
                            else if (HasarsizlikIndirim == "3")
                            {
                                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 50;
                            }
                            else if (HasarsizlikIndirim == "4")
                            {
                                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 60;
                            }
                            else
                            {
                                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                            }
                        }
                    }
                }
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = komisyon;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                #endregion

                #region Vergiler
                this.AddVergi(KaskoVergiler.GiderVergisi, giderVergi);
                #endregion

                #region Teminatlar

                var SompoJapanTeminatlar = teklifResult.Result.COVERS;
                decimal teminatBedel = 0;
                decimal teminatBrutPrim = 0;

                if (SompoJapanTeminatlar != null)
                {
                    for (int i = 0; i < SompoJapanTeminatlar.Count(); i++)
                    {
                        switch (SompoJapanTeminatlar[i].COVER_CODE)
                        {
                            case SOMPOJAPAN_KaskoTeminatlar.Arac724Hizmet:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.CarpmaCarpilma:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.CarpmaCarpilma, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.Yanma:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Arac_Yanmasi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.Calinma:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Arac_Calinmasi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.GLKHHT:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.GLKHHT, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.SelBaskini:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Seylap, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.Deprem:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Deprem, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.AnahtarKaybiDiger:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Anahtar_Kaybi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.HayvanZarari:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Hayvanlarin_Verecegi_Zarar, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.SigaraBenzeri:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Sigara_Ve_Benzeri_Madde_Zararlari, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.CekmeCekilme:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.YetkisizCekilme, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.FK_Vefat:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.KFK_Olum, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.FK_SurekliSakat:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.HK_MotorluArac:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.HK_SurucuyeBagli:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.IMM_Sahis:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.IMM_Kombine:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.IMM_Kaza:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.IMM_Maddi:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                            case SOMPOJAPAN_KaskoTeminatlar.HasarsizlikKoruma:
                                {
                                    teminatBedel = SompoJapanTeminatlar[i].COVER_AMOUNT.Value;
                                    teminatBrutPrim = SompoJapanTeminatlar[i].GROSS_PREMIUM.Value;
                                    this.AddTeminat(KaskoTeminatlar.HasarsizlikKoruma, teminatBedel, 0, 0, teminatBrutPrim, 0);
                                }
                                break;
                        }

                    }

                    //foreach (var item in SompoJapanTeminatlar)
                    //{
                    //}
                }

                #endregion

                //#region Ödeme Planı (Sompo Japan Sigortada Teklifler Peşin olarak hesaplanıyor. Poliçeleştirmede Taksit gönderiliyor.)

                //var sompoTaksitler = teklifResult.Result.PAYMENT.INSTALLMENTS;
                //if (sompoTaksitler == null || sompoTaksitler.Length == 0)
                //{
                //    this.GenelBilgiler.TaksitSayisi = 1;
                //    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                //}
                //else
                //{
                //    this.GenelBilgiler.TaksitSayisi = (byte)sompoTaksitler.Length;

                //    foreach (var taksit in sompoTaksitler)
                //    {
                //        this.AddOdemePlani(taksit.INSTALLMENT_ORDER_NO, taksit.INSTALLMENT_DATE, taksit.INSTALLMENT_AMOUNT, OdemeTipleri.KrediKarti);
                //    }
                //}
                //#endregion

                #region WebServis Cevap
                this.AddWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Teklif_Police_No, teklifResult.Result.PROPOSAL_NO.ToString());
                string SompoBilgiMesaji = "";
                if (!String.IsNullOrEmpty(teklifResult.Description))
                {
                    SompoBilgiMesaji = teklifResult.Description;
                    if (SompoBilgiMesaji.Length <= 1000)
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, SompoBilgiMesaji);
                    }
                    else
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, SompoBilgiMesaji.Substring(0, 999));
                    }
                }
                //this.AddWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Kasko_Session_No, teklifResult.Result..ToString());
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                #region Hata Log
                clntKasko.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            sompojapan.common.Common client = new sompojapan.common.Common();
            try
            {
                #region Yeni WS

                #region Veri Hazırlama GENEL
                KonfigTable konfigCommon = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANCommonV2);
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);           
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.SOMPOJAPAN });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();
                // Cryptography sifrele = new Cryptography();
                Crypto crypto = new Crypto();

                client.Url = konfigCommon[Konfig.SOMPOJAPAN_CommonServiceURLV2]; ;
                client.Timeout = 150000;

                sompojapan.common.ExtendConfirmParameters confirmRequest = new sompojapan.common.ExtendConfirmParameters();

                client.IdentityHeaderValue = new sompojapan.common.IdentityHeader()
                {
                    KullaniciAdi = servisKullanici.KullaniciAdi,
                    KullaniciParola = servisKullanici.Sifre,
                    KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    KullaniciTipi = sompojapan.common.ClientType.ACENTE,
                };

                #endregion

                #region Genel Bilgiler

                confirmRequest.Policy = this.GetPolicy(teklif);
                confirmRequest.Unit = this.SigortaEttirenBilgileriCommon(teklif, servisKullanici);

                List<sompojapan.common.CustomParameter> parameters = new List<sompojapan.common.CustomParameter>();
                sompojapan.common.CustomParameter parametre = new sompojapan.common.CustomParameter();
                parametre.code = "GSM";
                parametre.value = telefon.Numara.Substring(3, 3) + telefon.Numara.Substring(7, 7);
                parameters.Add(parametre);

                confirmRequest.Parameters = parameters.ToArray();

                this.BeginLog(confirmRequest, confirmRequest.GetType(), WebServisIstekTipleri.Police);

                //Kredi kartı bilgileri Loglanmıyor. Şifrelenecek alanlar
                string cardHolderName = String.Empty;
                string cardNumber = String.Empty;
                string month = String.Empty;
                string year = String.Empty;
                string cvv = String.Empty;
                int taksitSayisi = odeme.TaksitSayisi;
                if (odeme.KrediKarti != null && (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti))
                {
                    cardHolderName = crypto.Encript(odeme.KrediKarti.KartSahibi);
                    cardNumber = crypto.Encript(odeme.KrediKarti.KartNo);
                    month = crypto.Encript(odeme.KrediKarti.SKA);
                    year = crypto.Encript(odeme.KrediKarti.SKY);
                    cvv = crypto.Encript(odeme.KrediKarti.CVC);
                }

                confirmRequest.PaymentInput = GetPaymentInfo(odeme);
                #endregion

                #region Service Call

                sompojapan.common.ConfirmOutput response = client.Onay(confirmRequest);
                client.Dispose();
                #endregion

                #region Hata Kontrol ve Kayıt

                if (!String.IsNullOrEmpty(response.RESULT.ERROR.ERROR_DESCRIPTION))
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.RESULT.ERROR.ERROR_DESCRIPTION);
                }
                else
                {
                    long? sompoPoliceNo = response.POLICY_NUMBER.Value;
                    long sompoMessageNo = Convert.ToInt64(ReadSoru(Common.WebServisCevaplar.SOMPOJAPAN_Trafik_Session_No, "0"));
                    this.GenelBilgiler.TUMPoliceNo = sompoPoliceNo.Value.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    confirmRequest.Policy = new sompojapan.common.Policy();
                    confirmRequest.Policy = this.GetPolicyPDF(teklif);

                    #region Poliçe PDF
                    string PolicePDFUrl = this.GetPDFURL(client, confirmRequest, sompojapan.common.PrintoutType.Policy);
                    if (!String.IsNullOrEmpty(PolicePDFUrl))
                    {
                        if (PolicePDFUrl != null)
                        {
                            WebClient myClient = new WebClient();
                            byte[] data = myClient.DownloadData(PolicePDFUrl);

                            IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                            string fileName = String.Empty;
                            string url = String.Empty;
                            fileName = String.Format("SOMPOJAPAN_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                            url = storage.UploadFile("kasko", fileName, data);
                            this.GenelBilgiler.PDFPolice = url;
                            _Log.Info("Police_PDF url: {0}", url);
                        }
                        else
                        {
                            this.AddHata("PDF dosyası alınamadı.");
                        }
                    }
                    #endregion

                    #region Bilgilendirme PDF
                    string BilgilendirmeURL = this.GetPDFURL(client, confirmRequest, sompojapan.common.PrintoutType.CustomerInformationForm);
                    if (!String.IsNullOrEmpty(BilgilendirmeURL))
                    {
                        if (BilgilendirmeURL != null)
                        {
                            WebClient myClient = new WebClient();
                            byte[] data = myClient.DownloadData(BilgilendirmeURL);

                            IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                            string fileName = String.Empty;
                            string url = String.Empty;
                            fileName = String.Format("SOMPOJAPAN_Kasko_Bilgilendirme_Formu_{0}.pdf", System.Guid.NewGuid().ToString());
                            url = storage.UploadFile("kasko", fileName, data);
                            this.GenelBilgiler.PDFBilgilendirme = url;
                            _Log.Info("Police_Bilgilendirme_Formu url: {0}", url);
                        }
                        else
                        {
                            this.AddHata("Police Bilgilendirme Formu alınamadı.");
                        }
                    }
                    #endregion
                }
                #endregion

                #endregion

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
            }
            catch (Exception ex)
            {
                #region Hata Log
                client.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }

        private sompojapan.kasko.Unit SigortaliBilgileri(ITeklif teklif, sompojapan.kasko.Casco clnt)
        {
            MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriAdre adress = _MusteriService.GetDefaultAdres(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriTelefon cepTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon evTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
            sompojapan.kasko.Unit unit = new sompojapan.kasko.Unit();

            sompojapan.kasko.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
            clnt.Dispose();
            sompojapan.kasko.MethodResultOfListOfAddressItem ilceler = new sompojapan.kasko.MethodResultOfListOfAddressItem();

            try
            {
                List<sompojapan.kasko.Address> addressList = new List<sompojapan.kasko.Address>();
                sompojapan.kasko.Address address = new sompojapan.kasko.Address();

                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Il;
                address.ADDRESS_DATA = "İSTANBUL";
                //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                if (il != null)
                {
                    var sompoIlAdi = il.Result.Where(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).FirstOrDefault();
                    if (sompoIlAdi != null)
                    {
                        address.ADDRESS_DATA = sompoIlAdi.ItemName;
                    }
                }
                addressList.Add(address);

                ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
                clnt.Dispose();
                address = new sompojapan.kasko.Address();
                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Ilce;
                address.ADDRESS_DATA = "MERKEZ";
                //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                if (ilceler != null && ilceler.Result != null)
                {
                    var sompoIlceAdi = ilceler.Result.Where(S => S.ItemName == ilce.IlceAdi).FirstOrDefault();
                    if (sompoIlceAdi != null)
                    {
                        address.ADDRESS_DATA = sompoIlceAdi.ItemName;
                    }
                }
                addressList.Add(address);

                if (!String.IsNullOrEmpty(adress.Mahalle))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = adress.Mahalle;
                    addressList.Add(address);

                }
                else if (!String.IsNullOrEmpty(adress.Semt))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Semt;
                    address.ADDRESS_DATA = adress.Semt;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = "Mah.";
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.Cadde))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = adress.Cadde;
                    addressList.Add(address);
                }
                else if (!String.IsNullOrEmpty(adress.Sokak))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Sokak;
                    address.ADDRESS_DATA = adress.Sokak;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = "Cad.";
                    addressList.Add(address);
                }
                if (!String.IsNullOrEmpty(adress.Apartman))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Apartmani;
                    address.ADDRESS_DATA = adress.Apartman;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.BinaNo))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.BinaNo;
                    address.ADDRESS_DATA = adress.BinaNo;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.DaireNo))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.DaireNo;
                    address.ADDRESS_DATA = adress.DaireNo;
                    addressList.Add(address);
                }

                unit.ADDRESS_LIST = addressList.ToArray();

                if (sigortali.KimlikNo.Length == 11)
                {
                    unit.IDENTITY_NO = Convert.ToInt64(sigortali.KimlikNo);
                    unit.PERSONAL_COMMERCIAL = "O";
                }
                else
                {
                    unit.TAX_NO = sigortali.KimlikNo;
                    unit.PERSONAL_COMMERCIAL = "T";
                }

                //unit.PASSPORT_NO = 0;
                unit.NAME = sigortali.AdiUnvan;
                unit.SURNAME = sigortali.SoyadiUnvan;
                if (sigortali.DogumTarihi.HasValue)
                {
                    unit.BIRTHDATE = sigortali.DogumTarihi.Value;
                }
                unit.FIRM_NAME = "";
                unit.FATHER_NAME = "";
                unit.MARITAL_STATUS = "";
                unit.BIRTH_PLACE = "";
                unit.OCCUPATION = SompoJapan_KaskoSoruCevapTipleri.Diger;
                unit.NATIONALITY = 1;
                unit.PERSONAL_COMMERCIAL = "";

                if (evTel != null)
                {
                    if (evTel.Numara.Length > 10)
                    {

                        unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                        unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                        unit.PHONE_NUMBER = Convert.ToInt64(evTel.Numara.Substring(7, 7));
                    }
                }
                if (cepTel != null)
                {
                    if (cepTel.Numara.Length > 10)
                    {

                        unit.GSM_COUNTRY_CODE = cepTel.Numara.Substring(0, 2);
                        unit.GSM_CODE = cepTel.Numara.Substring(3, 3);
                        unit.GSM_NUMBER = cepTel.Numara.Substring(7, 7);
                    }
                }

                unit.EMAIL_ADDRESS = sigortali.EMail;
                unit.GENDER = sigortali.Cinsiyet;

                return unit;
            }
            catch (Exception ex)
            {
                clnt.Abort();
                string hataMesaji = "";
                if (!string.IsNullOrEmpty(il.Description))
                {
                    if (il.Description != "Success")
                    {
                        hataMesaji += il.Description;
                    }
                }
                else
                {
                    hataMesaji += il.ResultCode.ToString();
                }
                if (!string.IsNullOrEmpty(ilceler.Description))
                {
                    hataMesaji += ilceler.ResultCode.ToString() + ilceler.Description;
                }
                hataMesaji += ex.Message;
                throw new Exception(hataMesaji);
            }
        }

        private sompojapan.kasko.Unit SigortaEttirenBilgileri(ITeklif teklif, sompojapan.kasko.Casco clnt)
        {
            TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
            MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
            MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
            MusteriTelefon cepTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon evTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
            sompojapan.kasko.Unit unit = new sompojapan.kasko.Unit();
            sompojapan.kasko.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
            clnt.Dispose();
            sompojapan.kasko.MethodResultOfListOfAddressItem ilceler = new sompojapan.kasko.MethodResultOfListOfAddressItem();
            try
            {
                List<sompojapan.kasko.Address> addressList = new List<sompojapan.kasko.Address>();
                sompojapan.kasko.Address address = new sompojapan.kasko.Address();
                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Il;
                address.ADDRESS_DATA = "İSTANBUL";
                //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                if (il != null)
                {
                    var sompoIlAdi = il.Result.Where(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).FirstOrDefault();
                    if (sompoIlAdi != null)
                    {
                        address.ADDRESS_DATA = sompoIlAdi.ItemName;
                    }
                }
                addressList.Add(address);

                ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
                clnt.Dispose();
                address = new sompojapan.kasko.Address();
                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Ilce;
                address.ADDRESS_DATA = "MERKEZ";
                //  address.ADDRESS_DATA = (ilceler != null && ilceler.ResultCode != Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ProposalResultCodes.Fail) ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                if (ilceler != null && ilceler.Result != null)
                {
                    var sompoIlceAdi = ilceler.Result.Where(S => S.ItemName == ilce.IlceAdi).FirstOrDefault();
                    if (sompoIlceAdi != null)
                    {
                        address.ADDRESS_DATA = sompoIlceAdi.ItemName;
                    }
                }

                addressList.Add(address);

                if (!String.IsNullOrEmpty(adress.Mahalle))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = adress.Mahalle;
                    addressList.Add(address);

                }
                else if (!String.IsNullOrEmpty(adress.Semt))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Semt;
                    address.ADDRESS_DATA = adress.Semt;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = "Mah.";
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.Cadde))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = adress.Cadde;
                    addressList.Add(address);
                }
                else if (!String.IsNullOrEmpty(adress.Sokak))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Sokak;
                    address.ADDRESS_DATA = adress.Sokak;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = "Cad.";
                    addressList.Add(address);
                }
                if (!String.IsNullOrEmpty(adress.Apartman))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Apartmani;
                    address.ADDRESS_DATA = adress.Apartman;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.BinaNo))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.BinaNo;
                    address.ADDRESS_DATA = adress.BinaNo;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.DaireNo))
                {
                    address = new sompojapan.kasko.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.DaireNo;
                    address.ADDRESS_DATA = adress.DaireNo;
                    addressList.Add(address);
                }

                unit.ADDRESS_LIST = addressList.ToArray();

                if (SEGenelBilgiler.KimlikNo.Length == 11)
                {
                    unit.IDENTITY_NO = Convert.ToInt64(SEGenelBilgiler.KimlikNo);
                    unit.PERSONAL_COMMERCIAL = "O";
                }
                else
                {
                    unit.TAX_NO = SEGenelBilgiler.KimlikNo;
                    unit.PERSONAL_COMMERCIAL = "T";
                }

                //unit.PASSPORT_NO = 0;
                unit.NAME = SEGenelBilgiler.AdiUnvan;
                unit.SURNAME = SEGenelBilgiler.SoyadiUnvan;
                if (SEGenelBilgiler.DogumTarihi.HasValue)
                {
                    unit.BIRTHDATE = SEGenelBilgiler.DogumTarihi.Value;
                }
                unit.FIRM_NAME = "";
                unit.FATHER_NAME = "";
                unit.MARITAL_STATUS = "";
                unit.BIRTH_PLACE = "";
                unit.OCCUPATION = SompoJapan_KaskoSoruCevapTipleri.Diger;
                unit.NATIONALITY = 1;

                if (evTel != null)
                {
                    if (evTel.Numara.Length > 10)
                    {

                        unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                        unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                        unit.PHONE_NUMBER = Convert.ToInt64(evTel.Numara.Substring(7, 7));
                    }
                }
                if (cepTel != null)
                {
                    if (cepTel.Numara.Length > 10)
                    {

                        unit.GSM_COUNTRY_CODE = cepTel.Numara.Substring(0, 2);
                        unit.GSM_CODE = cepTel.Numara.Substring(3, 3);
                        unit.GSM_NUMBER = cepTel.Numara.Substring(7, 7);
                    }
                }
                unit.EMAIL_ADDRESS = SEGenelBilgiler.EMail;
                unit.GENDER = SEGenelBilgiler.Cinsiyet;

                return unit;
            }
            catch (Exception)
            {
                clnt.Abort();
                string hataMesaji = "";
                if (!string.IsNullOrEmpty(il.Description))
                    hataMesaji += il.Description;
                else
                {
                    hataMesaji += il.ResultCode.ToString();
                }
                if (!string.IsNullOrEmpty(ilceler.Description))
                    hataMesaji += ilceler.ResultCode.ToString() + ilceler.Description;

                throw new Exception(hataMesaji);
            }
        }

        private sompojapan.kasko.CustomParameter[] TeklifSorular(ITeklif teklif, sompojapan.kasko.Casco clntKasko, TVMWebServisKullanicilari serviskullanici)
        {
            #region İp li ws
            List<sompojapan.kasko.CustomParameter> customParameter = new List<sompojapan.kasko.CustomParameter>();
            sompojapan.kasko.CustomParameter parametre = new sompojapan.kasko.CustomParameter();
            sompojapan.kasko.CascoParameters cascoparameters = new sompojapan.kasko.CascoParameters();


            //Tescil veya asbis no gönderiliyor
            string TescilSeriNo = teklif.Arac.TescilSeriNo;
            string TescilSeriKod = teklif.Arac.TescilSeriKod;
            if (!String.IsNullOrEmpty(TescilSeriNo) && !String.IsNullOrEmpty(TescilSeriKod))
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_TrafikSoruTipleri.TescilBelgeKod;
                parametre.value = TescilSeriKod;
                customParameter.Add(parametre);

                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_TrafikSoruTipleri.AsbisTescilSeriNo;
                parametre.value = TescilSeriNo;
                customParameter.Add(parametre);
            }
            else if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_TrafikSoruTipleri.AsbisTescilSeriNo;
                parametre.value = teklif.Arac.AsbisNo;
                customParameter.Add(parametre);
            }

            string kullanimT = String.Empty;
            string kod2 = String.Empty;
            string[] parts = teklif.Arac.KullanimTarzi.Split('-');
            if (parts.Length == 2)
            {
                kullanimT = parts[0];
                kod2 = parts[1];
            }

            #region IMM ve FK

            string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
            decimal bedeniSahis = 0;
            decimal Kombine = 0;
            if (!String.IsNullOrEmpty(immKademe))
            {
                //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                if (IMMBedel != null)
                {
                    bedeniSahis = IMMBedel.BedeniSahis.Value;
                    Kombine = IMMBedel.Kombine.Value;

                    sompojapan.kasko.MethodResultOfListOfImmValues SompoIMMList = clntKasko.GetIMMListExt();
                    clntKasko.Dispose();

                    // Sompo Japan IMM Limit listesi wsinden, ekrandan girilen bedelin değer karşılığı alınıyor
                    if (SompoIMMList.Result != null)
                    {
                        if (bedeniSahis > 0)
                        {
                            var immBedel = SompoIMMList.Result.Where(s => s.KisiBasiBedeni == bedeniSahis).FirstOrDefault();
                            if (immBedel == null)
                            {
                                immBedel = SompoIMMList.Result.Where(s => s.KisiBasiBedeni <= bedeniSahis).OrderByDescending(s => s.KisiBasiBedeni).FirstOrDefault();
                            }
                            if (immBedel != null)
                            {
                                parametre = new sompojapan.kasko.CustomParameter();
                                parametre.code = SompoJapan_KaskoSoruTipleri.KombineKisiKazaBasinaMi;
                                parametre.value = SompoJapan_KaskoSoruCevapTipleri.KisiKazaBasina;
                                customParameter.Add(parametre);

                                parametre = new sompojapan.kasko.CustomParameter();
                                parametre.code = SompoJapan_KaskoSoruTipleri.IMMBedelSecimi;
                                parametre.value = immBedel.Kod.ToString();
                                customParameter.Add(parametre);
                            }
                        }
                        else
                        {
                            var kombineBedel = SompoIMMList.Result.Where(s => s.Kombine == Kombine).FirstOrDefault();
                            if (kombineBedel == null)
                            {
                                kombineBedel = SompoIMMList.Result.Where(s => s.Kombine <= Kombine).OrderByDescending(s => s.Kombine).FirstOrDefault();
                            }
                            if (kombineBedel != null)
                            {
                                parametre = new sompojapan.kasko.CustomParameter();
                                parametre.code = SompoJapan_KaskoSoruTipleri.KombineKisiKazaBasinaMi;
                                parametre.value = SompoJapan_KaskoSoruCevapTipleri.Kombine;
                                customParameter.Add(parametre);

                                parametre = new sompojapan.kasko.CustomParameter();
                                parametre.code = SompoJapan_KaskoSoruTipleri.IMMBedelSecimi;
                                parametre.value = kombineBedel.Kod.ToString();
                                customParameter.Add(parametre);
                            }
                        }
                    }
                }
            }

            string ferdiKaza = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
            decimal FKVefat = 0;
            decimal FKMasraf = 0;
            if (!String.IsNullOrEmpty(ferdiKaza))
            {
                CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                var FK = _CRService.GetKaskoFKBedel(Convert.ToInt32(ferdiKaza), parts[0], parts[1]);
                if (FK != null)
                {
                    FKVefat = FK.Vefat.Value;
                    FKMasraf = FK.Tedavi.Value;
                }
                var SompoFKTedaviMasraflari = clntKasko.PersonalAccidentCoverLimitsExt(SompoJapan_KaskoTeminatKodlari.FerdiKazaTedaviMasraflari).Result;
                clntKasko.Dispose();
                if (SompoFKTedaviMasraflari != null && FKMasraf > 0)
                {
                    parametre = new sompojapan.kasko.CustomParameter();
                    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;

                    foreach (var item in SompoFKTedaviMasraflari)
                    {
                        if ((item.ItemText == "Min" && FKMasraf > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKMasraf < Convert.ToDecimal(item.ItemValue)))
                            parametre.value = FKMasraf.ToString();
                        else
                            parametre.value = SompoFKTedaviMasraflari.FirstOrDefault().ItemValue;
                    }
                    customParameter.Add(parametre);
                }
            }
            //else
            //{
            //    parametre = new sompojapan.kasko.CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;
            //    parametre.value = "500";
            //    customParameter.Add(parametre);
            //}
            var SompoFKVefatTeminati = clntKasko.PersonalAccidentCoverLimitsExt(SompoJapan_KaskoTeminatKodlari.FerdiKazaVefatTeminati).Result;
            clntKasko.Dispose();
            if (SompoFKVefatTeminati != null && FKVefat > 0)
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaVefatTeminati;
                foreach (var item in SompoFKVefatTeminati)
                {
                    if ((item.ItemText == "Min" && FKVefat > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKVefat < Convert.ToDecimal(item.ItemValue)))
                        parametre.value = FKVefat.ToString();
                    else
                        parametre.value = SompoFKVefatTeminati.FirstOrDefault().ItemValue;
                }

                customParameter.Add(parametre);
            }
            //else
            //{
            //    parametre = new sompojapan.kasko.CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaVefatTeminati;
            //    parametre.value = "500";
            //    customParameter.Add(parametre);
            //}

            #endregion

            #region Marka Model

            var sompoJapanMarkaModel = "";
            var sompoJapanMarkaList = clntKasko.BrandsWithCodeExt();
            if (sompoJapanMarkaList.Result != null)
            {
                // sompojapan.trafik.Traffic clntTrafik = new sompojapan.trafik.Traffic();

                clntKasko.IdentityHeaderValue = new sompojapan.kasko.IdentityHeader()
                {
                    KullaniciAdi = serviskullanici.KullaniciAdi,
                    KullaniciParola = serviskullanici.Sifre,
                    KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    KullaniciTipi = Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ClientType.ACENTE
                };

                var sompoJapanMarka = sompoJapanMarkaList.Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue;
                if (sompoJapanMarka != null)
                {
                    string aracTipi = teklif.Arac.AracinTipi.Trim(); //babonline arac tipi

                    if (aracTipi.Length < 3)
                    {
                        aracTipi = "0" + aracTipi;
                    }
                    string markaModel = sompoJapanMarka.Trim() + aracTipi;
                    var sompoJapanMarkaModelList = clntKasko.ModelsViaBrandCodeExt(sompoJapanMarka.ToString()).Result;
                    clntKasko.Dispose();

                    if (sompoJapanMarkaModelList != null)
                    {
                        sompoJapanMarkaModel = sompoJapanMarkaModelList.FirstOrDefault(k => k.ItemValue.Trim() == markaModel).ItemValue.Trim();
                        if (sompoJapanMarkaModel != null)
                        {
                            parametre = new sompojapan.kasko.CustomParameter();
                            parametre.code = SompoJapan_KaskoSoruTipleri.MarkaAdi;
                            parametre.value = sompoJapanMarkaModel;
                            customParameter.Add(parametre);
                        }
                    }
                }
            }

            #endregion

            #region Diğer Araç Bilgileri

            parametre = new sompojapan.kasko.CustomParameter();
            parametre.code = SompoJapan_KaskoSoruTipleri.TrafigeCikisTarihi;
            parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
            customParameter.Add(parametre);

            parametre = new sompojapan.kasko.CustomParameter();
            parametre.code = SompoJapan_KaskoSoruTipleri.KullanimKodu;

            string kullanimtarzi = teklif.Arac.KullanimTarzi;
            int sompoKullanimTarzi = 1;
            if (!String.IsNullOrEmpty(kullanimtarzi))
            {
                switch (kullanimtarzi)
                {
                    case "111-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Otomobil; break;
                    case "111-14": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                    case "311-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                    case "311-12": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                    case "411-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                    case "411-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                    case "411-12": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                    case "311-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                    case "511-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                    case "511-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                    case "511-12": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                    case "511-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                    case "611-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Traktor; break;
                    case "711-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Motorsiklet; break;
                    case "521-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
                    case "521-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
                    case "421-10": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
                    case "421-11": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
                    case "121-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Jeep; break;
                    case "532-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Romork; break;
                    case "526-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Cekici; break;
                    case "911-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                    case "911-11": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                    case "911-12": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                    case "523-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                    case "611-11": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                }
            }
            else parametre.value = "1";

            sompoKullanimTarzi = Convert.ToInt32(parametre.value);

            customParameter.Add(parametre);

            if (teklif.Arac.KoltukSayisi.HasValue)
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.Koltukadedi;
                parametre.value = teklif.Arac.KoltukSayisi.Value.ToString();
                customParameter.Add(parametre);
            }
            bool anahtarKaybi = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
            if (anahtarKaybi)
            {
                var sompoJapanAnahtarKaybi = clntKasko.LostIgnitionKeyCoversExt();
                clntKasko.Dispose();
                string sompoJapanAnahtarKaybiBedel = "";
                if (sompoJapanAnahtarKaybi.Result != null)
                {
                    sompoJapanAnahtarKaybiBedel = sompoJapanAnahtarKaybi.Result.FirstOrDefault(s => s.ItemValue == "1").ItemValue;
                }
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.AnahtarKaybiTeminatLimiti;
                parametre.value = sompoJapanAnahtarKaybiBedel;
                customParameter.Add(parametre);
            }

            bool artiTeminatPlani = teklif.ReadSoru(KaskoSorular.SompoArtiTeminatPlani, false);
            if (artiTeminatPlani)
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.ArtiTeminatIstiyorMu;
                parametre.value = "E";
                customParameter.Add(parametre);

                string artiTeminatPlanDegeri = teklif.ReadSoru(KaskoSorular.SompoArtiTeminatPlanDegeri, "");
                if (!String.IsNullOrEmpty(artiTeminatPlanDegeri))
                {
                    parametre = new sompojapan.kasko.CustomParameter();
                    parametre.code = SompoJapan_KaskoSoruTipleri.ArtiTeminatDegeri;
                    parametre.value = artiTeminatPlanDegeri;
                    customParameter.Add(parametre);

                }
            }
            else
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.ArtiTeminatIstiyorMu;
                parametre.value = "H";
                customParameter.Add(parametre);
            }

            #endregion

            #region Eski Poliçesi Olmayan Araç için Sorular

            bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
            if (!eskiPoliceVar)
            {
                //parametre = new sompojapan.kasko.CustomParameter();

                //if (sompoJapanMarkaModel != null)
                //{
                //    parametre.code = SompoJapan_KaskoSoruTipleri.MarkaAdi;
                //    parametre.value = sompoJapanMarkaModel;
                //    customParameter.Add(parametre);
                //}
                //parametre = new sompojapan.kasko.CustomParameter();
                //parametre.code = SompoJapan_KaskoSoruTipleri.TrafigeCikisTarihi;
                //parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
                //customParameter.Add(parametre);

                //parametre = new sompojapan.kasko.CustomParameter();
                //parametre.code = SompoJapan_KaskoSoruTipleri.KullanimKodu;

                //if (!String.IsNullOrEmpty(kullanimtarzi))
                //{
                //    switch (kullanimtarzi)
                //    {
                //        case "111-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Otomobil; break;
                //        case "311-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
                //        case "311-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                //        case "511-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
                //        case "611-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Traktor; break;
                //        case "711-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Motorsiklet; break;
                //        case "521-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
                //        case "421-10": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
                //        case "121-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Jeep; break;
                //        case "532-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Romork; break;
                //        case "526-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Cekici; break;
                //        case "911-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
                //        case "911-12": parametre.value = SompoJapan_KaskoTarifeKodlari.DigerAraclar; break;
                //    }
                //}
                //else parametre.value = "1";
                //customParameter.Add(parametre);

                //parametre = new sompojapan.kasko.CustomParameter();
                //parametre.code = SompoJapan_KaskoSoruTipleri.Koltukadedi;
                //parametre.value = teklif.Arac.KoltukSayisi.ToString();
                //customParameter.Add(parametre);             
            }
            #endregion

            #region Kasa Tipi

            //var sompoJapanKasaTipiList = clntKasko.VehicleCaseTypesExt(false, sompoKullanimTarzi);
            //clntKasko.Dispose();
            //if (sompoJapanKasaTipiList != null && sompoJapanKasaTipiList.Result != null && sompoJapanKasaTipiList.Description == "Success")
            //{
            //    parametre = new sompojapan.kasko.CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.KasaTipi;
            //    parametre.value = sompoJapanKasaTipiList.Result;
            //    customParameter.Add(parametre);
            //}

            if (sompoKullanimTarzi == 3 || sompoKullanimTarzi == 4 || sompoKullanimTarzi == 7 || sompoKullanimTarzi == 12 || sompoKullanimTarzi == 13)
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.KasaTipi;
                parametre.value = "3";
                customParameter.Add(parametre);
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.KasaTeminati;
                parametre.value = "H";
                customParameter.Add(parametre);
            }

            #endregion

            #region Kasko Türü
            bool kaskoTuru = teklif.ReadSoru(KaskoSorular.SompoJapanKaskoTuru, true);
            string faaliyetKodu = teklif.ReadSoru(KaskoSorular.SompoJapanFaaliyetKodu, String.Empty);
            if (!kaskoTuru)
            {
                parametre = new sompojapan.kasko.CustomParameter();
                parametre.code = SompoJapan_KaskoSoruTipleri.FaaliyetKodu;
                parametre.value = faaliyetKodu;
                customParameter.Add(parametre);
            }
            else
            {
                string MeslekKodu = teklif.ReadSoru(KaskoSorular.Meslek, "99");
                var SompoMeslekKodu = _TeklifService.GetTUMMeslekKod(MeslekKodu, TeklifUretimMerkezleri.SOMPOJAPAN);

                if (SompoMeslekKodu != null)
                {
                    parametre = new sompojapan.kasko.CustomParameter();
                    parametre.code = SompoJapan_KaskoSoruTipleri.MeslekKodu;
                    parametre.value = SompoMeslekKodu.CR_MeslekKodu;
                    customParameter.Add(parametre);
                }
            }
            #endregion
            cascoparameters.Parameters = customParameter.ToArray();
            #endregion

            #region Kullanıcı Adı, Şifre ws
            //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKasko);

            //clnt = new sompojapankasko.Casco();
            //clnt.Url = konfig[Konfig.SOMPOJAPAN_CascoServiceURL];
            //clnt.Timeout = 150000;

            //List<CustomParameter> customParameter = new List<CustomParameter>();
            //CustomParameter parametre = new CustomParameter();
            //CascoParameters cascoparameters = new CascoParameters();

            //parametre.code = SompoJapan_KaskoSoruTipleri.MeslekKodu;
            //parametre.value = "99";
            //customParameter.Add(parametre);

            //string kullanimT = String.Empty;
            //string kod2 = String.Empty;
            //string[] parts = teklif.Arac.KullanimTarzi.Split('-');
            //if (parts.Length == 2)
            //{
            //    kullanimT = parts[0];
            //    kod2 = parts[1];
            //}

            //string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
            //decimal bedeniSahis = 0;
            //decimal Kombine = 0;
            //if (!String.IsNullOrEmpty(immKademe))
            //{
            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.KombineKisiKazaBasinaMi;
            //    parametre.value = SompoJapan_KaskoSoruCevapTipleri.KisiKazaBasina;
            //    customParameter.Add(parametre);

            //    //var iMM = _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &&
            //    //                                                       s.Kademe == immKademe && s.Kod2 == kod2 &&
            //    //                                                       s.KullanimTarziKodu == kullanimT).FirstOrDefault();

            //    //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
            //    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

            //    if (IMMBedel != null)
            //    {
            //        bedeniSahis = IMMBedel.BedeniSahis.Value;
            //        Kombine = IMMBedel.Kombine.Value;

            //        MethodResultOfListOfImmValues SompoIMMList = clnt.GetIMMList(servisKullanici.KullaniciAdi, servisKullanici.Sifre);

            //        // Sompo Japan IMM Limit listesi wsinden, ekrandan girilen bedelin değer karşılığı alınıyor

            //        if (SompoIMMList.Result != null)
            //        {
            //            parametre = new CustomParameter();
            //            foreach (var immlist in SompoIMMList.Result)
            //            {
            //                if (bedeniSahis > 0)
            //                {
            //                    if (Convert.ToDecimal(immlist.KisiBasiBedeni) == bedeniSahis || Convert.ToDecimal(immlist.KisiBasiBedeni) < bedeniSahis)
            //                    {
            //                        parametre.code = SompoJapan_KaskoSoruTipleri.IMMBedelSecimi;
            //                        parametre.value = immlist.Kod.ToString();

            //                    }
            //                }
            //                else
            //                {
            //                    if (Convert.ToDecimal(immlist.Kombine) == Kombine || Convert.ToDecimal(immlist.Kombine) < Kombine)
            //                    {
            //                        parametre.code = SompoJapan_KaskoSoruTipleri.IMMBedelSecimi;
            //                        parametre.value = immlist.Kod.ToString();

            //                    }
            //                }
            //            }
            //            customParameter.Add(parametre);
            //        }
            //    }
            //}
            ////sompojapan.kasko.Casco clntKasko = new sompojapan.kasko.Casco();
            ////clntKasko.IdentityHeaderValue = new sompojapan.kasko.IdentityHeader()
            ////{
            ////    KullaniciAdi = servisKullanici.KullaniciAdi,
            ////    KullaniciParola = servisKullanici.Sifre,
            ////    KullaniciIP = "85.105.78.56",
            ////    KullaniciTipi = Neosinerji.BABOnlineTP.Business.sompojapan.kasko.ClientType.DEVELOP
            ////};

            ////KonfigTable konfigKasko = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKaskoV2);
            ////clntKasko.Url = konfigKasko[Konfig.SOMPOJAPAN_CascoServiceURLV2];

            //////var sompoJapanMarka2 = clntKasko.BrandsWithCodeExt().Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue; 
            ////var sompojapanMarkaModel = clntKasko.ModelsViaBrandCodeExt(sompoJapanMarka2);

            //string sompoJapanMarka = clnt.BrandsWithCode(servisKullanici.KullaniciAdi, servisKullanici.Sifre).Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue;
            //string aracTipi = teklif.Arac.AracinTipi.Trim(); //babonline arac tipi

            //if (aracTipi.Length < 3)
            //{
            //    aracTipi = "0" + aracTipi;
            //}
            //string markaModel = sompoJapanMarka.Trim() + aracTipi;
            //var sompoJapanMarkaModel = clnt.ModelsViaBrandCode(servisKullanici.KullaniciAdi, servisKullanici.Sifre, sompoJapanMarka).Result.FirstOrDefault(k => k.ItemValue.Trim() == markaModel).ItemValue.Trim();

            //if (sompoJapanMarkaModel != null)
            //{
            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.MarkaAdi;
            //    parametre.value = sompoJapanMarkaModel;
            //    customParameter.Add(parametre);
            //}

            //parametre = new CustomParameter();
            //parametre.code = SompoJapan_KaskoSoruTipleri.TrafigeCikisTarihi;
            //parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
            //customParameter.Add(parametre);

            //parametre = new CustomParameter();
            //parametre.code = SompoJapan_KaskoSoruTipleri.KullanimKodu;

            //string kullanimtarzi = teklif.Arac.KullanimTarzi;
            //if (!String.IsNullOrEmpty(kullanimtarzi))
            //{
            //    switch (kullanimtarzi)
            //    {
            //        case "111-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Otomobil; break;
            //        case "311-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
            //        case "311-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
            //        case "611-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Traktor; break;
            //        case "711-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Motorsiklet; break;
            //        case "521-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
            //        case "421-10": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
            //        case "121-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Jeep; break;
            //        case "532-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Romork; break;
            //        case "526-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Cekici; break;
            //        case "911-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
            //        case "911-12": parametre.value = SompoJapan_KaskoTarifeKodlari.DigerAraclar; break;
            //    }
            //}
            //else parametre.value = "1";

            //customParameter.Add(parametre);

            //if (teklif.Arac.KoltukSayisi.HasValue)
            //{
            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.Koltukadedi;
            //    parametre.value = teklif.Arac.KoltukSayisi.Value.ToString();
            //    customParameter.Add(parametre);
            //}
            //bool anahtarKaybi = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
            //if (anahtarKaybi)
            //{
            //    string sompoJapanAnahtarKaybiBedel = clnt.LostIgnitionKeyCovers(servisKullanici.KullaniciAdi, servisKullanici.Sifre).Result.FirstOrDefault(s => s.ItemValue == "1").ItemValue;

            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.AnahtarKaybiTeminatLimiti;
            //    parametre.value = sompoJapanAnahtarKaybiBedel;
            //    customParameter.Add(parametre);
            //}

            //string ferdiKaza = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
            //decimal FKVefat = 0;
            //decimal FKMasraf = 0;
            //if (!String.IsNullOrEmpty(ferdiKaza))
            //{
            //    //var FK = _CRContext.CR_KaskoFKRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.HDI &&
            //    //                                                      s.Kademe == ferdiKaza && s.Kod2 == kod2 &&
            //    //                                                      s.KullanimTarziKodu == kullanimT).FirstOrDefault();
            //    CR_KaskoFK CRKademeNo = new CR_KaskoFK();
            //    var FK = _CRService.GetKaskoFKBedel(Convert.ToInt32(ferdiKaza), parts[0], parts[1]);
            //    if (FK != null)
            //    {
            //        FKVefat = FK.Vefat.Value;
            //        FKMasraf = FK.Tedavi.Value;
            //    }
            //    var SompoFKTedaviMasraflari = clnt.PersonalAccidentCoverLimits(servisKullanici.KullaniciAdi, servisKullanici.Sifre, SompoJapan_KaskoTeminatKodlari.FerdiKazaTedaviMasraflari).Result;
            //    if (SompoFKTedaviMasraflari != null && FKMasraf > 0)
            //    {
            //        parametre = new CustomParameter();
            //        parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;

            //        foreach (var item in SompoFKTedaviMasraflari)
            //        {
            //            if ((item.ItemText == "Min" && FKMasraf > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKMasraf < Convert.ToDecimal(item.ItemValue)))
            //                parametre.value = FKMasraf.ToString();
            //            else
            //                parametre.value = SompoFKTedaviMasraflari.FirstOrDefault().ItemValue;
            //        }
            //        customParameter.Add(parametre);
            //    }
            //}
            //else
            //{
            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaTedaviMasraflari;
            //    parametre.value = "500";
            //    customParameter.Add(parametre);
            //}
            //var SompoFKVefatTeminati = clnt.PersonalAccidentCoverLimits(servisKullanici.KullaniciAdi, servisKullanici.Sifre, SompoJapan_KaskoTeminatKodlari.FerdiKazaVefatTeminati).Result;
            //if (SompoFKVefatTeminati != null && FKVefat > 0)
            //{
            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaVefatTeminati;
            //    foreach (var item in SompoFKVefatTeminati)
            //    {
            //        if ((item.ItemText == "Min" && FKVefat > Convert.ToDecimal(item.ItemValue)) || (item.ItemText == "Max" && FKVefat < Convert.ToDecimal(item.ItemValue)))
            //            parametre.value = FKMasraf.ToString();
            //        else
            //            parametre.value = SompoFKVefatTeminati.FirstOrDefault().ItemValue;
            //    }

            //    customParameter.Add(parametre);
            //}
            //else
            //{
            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.FerdiKazaVefatTeminati;
            //    parametre.value = "500";
            //    customParameter.Add(parametre);
            //}
            //cascoparameters.Parameters = customParameter.ToArray();

            //#region Yeni Arac

            //bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
            //if (!eskiPoliceVar)
            //{
            //    parametre = new CustomParameter();

            //    if (sompoJapanMarkaModel != null)
            //    {
            //        parametre.code = SompoJapan_KaskoSoruTipleri.MarkaAdi;
            //        parametre.value = sompoJapanMarkaModel;
            //        customParameter.Add(parametre);
            //    }
            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.TrafigeCikisTarihi;
            //    parametre.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
            //    customParameter.Add(parametre);

            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.KullanimKodu;

            //    if (!String.IsNullOrEmpty(kullanimtarzi))
            //    {
            //        switch (kullanimtarzi)
            //        {
            //            case "111-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Otomobil; break;
            //            case "311-11": parametre.value = SompoJapan_KaskoTarifeKodlari.Minibus; break;
            //            case "311-15": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyonet; break;
            //            case "611-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Traktor; break;
            //            case "711-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Motorsiklet; break;
            //            case "521-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Kamyon; break;
            //            case "421-10": parametre.value = SompoJapan_KaskoTarifeKodlari.BOtobus; break;
            //            case "121-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Jeep; break;
            //            case "532-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Romork; break;
            //            case "526-10": parametre.value = SompoJapan_KaskoTarifeKodlari.Cekici; break;
            //            case "911-10": parametre.value = SompoJapan_KaskoTarifeKodlari.IsMakinesi; break;
            //            case "911-12": parametre.value = SompoJapan_KaskoTarifeKodlari.DigerAraclar; break;
            //        }
            //    }
            //    else parametre.value = "1";
            //    customParameter.Add(parametre);

            //    parametre = new CustomParameter();
            //    parametre.code = SompoJapan_KaskoSoruTipleri.Koltukadedi;
            //    parametre.value = teklif.Arac.KoltukSayisi.ToString();
            //    customParameter.Add(parametre);

            //    cascoparameters.Parameters = customParameter.ToArray();
            //}
            //#endregion
            #endregion

            return cascoparameters.Parameters;
        }

        private sompojapan.common.Unit SigortaEttirenBilgileriCommon(ITeklif teklif, TVMWebServisKullanicilari servisKullanici)
        {
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANTrafikV2);
      
            sompojapan.trafik.Traffic clnt = new sompojapan.trafik.Traffic();
            clnt.Url = konfig[Konfig.SOMPOJAPAN_TrafikServiceURLV2];
            clnt.Timeout = 150000;

            clnt.IdentityHeaderValue = new sompojapan.trafik.IdentityHeader()
            {
                KullaniciAdi = servisKullanici.KullaniciAdi,
                KullaniciParola = servisKullanici.Sifre,
                KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                KullaniciTipi = sompojapan.trafik.ClientType.ACENTE,
            };


            TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;

            MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
            MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
            MusteriTelefon cepTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon evTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
            sompojapan.common.Unit unit = new sompojapan.common.Unit();

            sompojapan.trafik.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
            clnt.Dispose();
            sompojapan.trafik.MethodResultOfListOfAddressItem ilceler = new sompojapan.trafik.MethodResultOfListOfAddressItem();

            try
            {
                List<sompojapan.common.Address> addressList = new List<sompojapan.common.Address>();
                sompojapan.common.Address address = new sompojapan.common.Address();

                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Il;
                address.ADDRESS_DATA = il.Result.FirstOrDefault(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).ItemName;

                addressList.Add(address);

                ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
                clnt.Dispose();
                address = new sompojapan.common.Address();
                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Ilce;
                address.ADDRESS_DATA = ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName;
                addressList.Add(address);

                if (!String.IsNullOrEmpty(adress.Mahalle))
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = adress.Mahalle;
                    addressList.Add(address);

                }
                else if (!String.IsNullOrEmpty(adress.Semt))
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Semt;
                    address.ADDRESS_DATA = adress.Semt;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = "Mah.";
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.Cadde))
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = adress.Cadde;
                    addressList.Add(address);
                }
                else if (!String.IsNullOrEmpty(adress.Sokak))
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Sokak;
                    address.ADDRESS_DATA = adress.Sokak;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = "Cad.";
                    addressList.Add(address);
                }
                if (!String.IsNullOrEmpty(adress.Apartman))
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Apartmani;
                    address.ADDRESS_DATA = adress.Apartman;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.BinaNo))
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.BinaNo;
                    address.ADDRESS_DATA = adress.BinaNo;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.DaireNo))
                {
                    address = new sompojapan.common.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.DaireNo;
                    address.ADDRESS_DATA = adress.DaireNo;
                    addressList.Add(address);
                }

                unit.ADDRESS_LIST = addressList.ToArray();

                if (SEGenelBilgiler.KimlikNo.Length == 11)
                {
                    unit.IDENTITY_NO = Convert.ToInt64(SEGenelBilgiler.KimlikNo);
                    unit.PERSONAL_COMMERCIAL = "O";
                }
                else
                {
                    unit.TAX_NO = SEGenelBilgiler.KimlikNo;
                    unit.PERSONAL_COMMERCIAL = "T";
                }

                //unit.PASSPORT_NO = 0;
                unit.NAME = SEGenelBilgiler.AdiUnvan;
                unit.SURNAME = SEGenelBilgiler.SoyadiUnvan;
                if (SEGenelBilgiler.DogumTarihi.HasValue)
                {
                    unit.BIRTHDATE = SEGenelBilgiler.DogumTarihi.Value;
                }
                unit.FIRM_NAME = "";
                unit.FATHER_NAME = "";
                unit.MARITAL_STATUS = "";
                unit.BIRTH_PLACE = "";
                // unit.OCCUPATION = SompoJapan_KaskoSoruCevapTipleri.Diger;
                unit.NATIONALITY = 1;

                if (evTel != null)
                {
                    unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                    unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                    unit.PHONE_NUMBER = Convert.ToInt64(evTel.Numara.Substring(7, 7));
                }
                if (cepTel != null)
                {
                    unit.GSM_COUNTRY_CODE = cepTel.Numara.Substring(0, 2);
                    unit.GSM_CODE = cepTel.Numara.Substring(3, 3);
                    unit.GSM_NUMBER = cepTel.Numara.Substring(7, 7);
                }
                unit.EMAIL_ADDRESS = SEGenelBilgiler.EMail;
                unit.GENDER = SEGenelBilgiler.Cinsiyet;

                return unit;
            }
            catch (Exception)
            {
                string hataMesaji = "";
                if (!string.IsNullOrEmpty(il.Description))
                    hataMesaji += il.Description;
                if (!string.IsNullOrEmpty(ilceler.Description))
                    hataMesaji += ilceler.Description;
                if (String.IsNullOrEmpty(hataMesaji))
                    hataMesaji = "Bir hata oluştu.";
                throw new Exception(hataMesaji);
            }
        }

        private sompojapan.common.Policy GetPolicy(ITeklif teklif)
        {
            sompojapan.common.Policy policy = new sompojapan.common.Policy()
            {
                ProductName = sompojapan.common.ProductName.BireyselKasko,
                CompanyCode = 0,
                EndorsNr = 0,
                FirmCode = 0,
                RenewalNr = 0
            };
            string TUMTeklifNo = this.ReadWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Teklif_Police_No, "0");
            policy.PolicyNumber = Convert.ToInt32(TUMTeklifNo);
            return policy;
        }

        private sompojapan.common.PaymentInfo GetPaymentInfo(Odeme odeme)
        {
            sompojapan.common.PaymentInfo paymentInfo = new sompojapan.common.PaymentInfo()
            {
                AccountBankNo = 0,
                AccountBranchCode = 0,
                AccountNo = 0,
                AccountOwnerName = "",
                CreditCardCvv = odeme.KrediKarti.CVC,
                CreditCardEndMonth = odeme.KrediKarti.SKA,
                CreditCardEndYear = odeme.KrediKarti.SKY,
                CreditCardNameSurname = odeme.KrediKarti.KartSahibi,
                CreditCardNumber = odeme.KrediKarti.KartNo,
                Installment = odeme.TaksitSayisi,
                PaymentType = sompojapan.common.PaymentMethod.WithCreditCard,

            };

            return paymentInfo;
        }

        private sompojapan.common.Policy GetPolicyPDF(ITeklif teklif)
        {
            sompojapan.common.Policy policy = new sompojapan.common.Policy()
            {
                PolicyNumber = Convert.ToInt32(teklif.GenelBilgiler.TUMPoliceNo),
                ProductName = sompojapan.common.ProductName.BireyselKasko,
                CompanyCode = 0,
                EndorsNr = 0,
                FirmCode = 0,
                RenewalNr = 0
            };

            return policy;
        }

        private string GetPDFURL(sompojapan.common.Common client, sompojapan.common.ExtendConfirmParameters confirmRequest, sompojapan.common.PrintoutType type)
        {
            string url = String.Empty;
            sompojapan.common.PrintResponse response = client.Basim(confirmRequest.Policy, type);
            client.Dispose();
            if (!response.success)
            {
                this.AddHata(response.description);
            }
            else
            {
                url = response.downloadurl;
            }

            return url;
        }

        public override void DekontPDF()
        {
            SompoDekontRequest request = new SompoDekontRequest();
            try
            {

                #region Yeni ws
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                KonfigTable konfigCommon = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANCommonV2);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.SOMPOJAPAN });
                sompojapan.common.ExtendConfirmParameters confirmRequest = new sompojapan.common.ExtendConfirmParameters();
                sompojapan.common.Common client = new sompojapan.common.Common();
                client.Url = konfigCommon[Konfig.SOMPOJAPAN_CommonServiceURLV2]; ;
                client.Timeout = 150000;

                client.IdentityHeaderValue = new sompojapan.common.IdentityHeader()
                {
                    KullaniciAdi = servisKullanici.KullaniciAdi,
                    KullaniciParola = servisKullanici.Sifre,
                    KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    KullaniciTipi = sompojapan.common.ClientType.ACENTE,
                };

                #region Service call
                sompojapan.common.PrintResponse response = client.Basim(confirmRequest.Policy, sompojapan.common.PrintoutType.Slip);

                #endregion

                #endregion              

                #region Hata Kontrol ve Kayıt
                if (!response.success)
                {
                    this.AddHata(response.description);
                }
                else
                {
                    var policeKrediKartiSlipPDF = response.downloadurl;
                    if (policeKrediKartiSlipPDF != null)
                    {
                        WebClient myClient = new WebClient();
                        byte[] data = myClient.DownloadData(policeKrediKartiSlipPDF);

                        IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Empty;
                        string url = String.Empty;

                        fileName = String.Format("sompo_dekont_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        url = storage.UploadFile("kasko", fileName, data);
                        this.GenelBilgiler.PDFDekont = url;

                        _Log.Info("Dekont_PDF url: {0}", url);
                        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                    }
                    else
                    {
                        this.AddHata("PDF dosyası alınamadı.");
                        return;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                _Log.Error("SompoJapanKasko.DekontPDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        public string IpGetir(int tvmKodu)
        {
            //string cagriIP = "82.222.165.62";
            string sigortaShopIp = " 94.54.67.159";

            string gonderilenIp = String.Empty;
            gonderilenIp = ClientIPNo; // "88.249.209.253"; Birebir ip 
            var tvmDetay = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == tvmKodu);
            if (tvmDetay != null)
            {
                var bagliOlduguTVMKodu = tvmDetay.BagliOlduguTVMKodu;
                if (bagliOlduguTVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu)
                {
                    gonderilenIp = sigortaShopIp;
                }
            }
            //gonderilenIp= "81.214.50.9";//MB Grup Sigorta
            //gonderilenIp = "88.249.80.115";//Tuğra Sigorta
            return gonderilenIp;
        

        }
    }
}
