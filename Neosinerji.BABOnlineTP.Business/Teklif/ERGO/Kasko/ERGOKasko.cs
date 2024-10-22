using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
//using Neosinerji.BABOnlineTP.Business.ergoturkiye.police.uretim;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Security;
using Microsoft.Web.Services3.Security.Tokens;
using System.Net;
using Neosinerji.BABOnlineTP.Business.ergoturkiye.kasko;
using Neosinerji.BABOnlineTP.Business.Ergo.Info;

namespace Neosinerji.BABOnlineTP.Business.ERGO.Kasko
{
    public class ERGOKasko : Teklif, IERGOKasko
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

        [InjectionConstructor]
        public ERGOKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
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
        }
        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.ERGO;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region Teklif Bilgilerine Ulaşım
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleERGOService);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ERGO });
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR ||
                                                                                       w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                                                           .ToList<TeklifAracEkSoru>();

                #region Sigortali / Sigorta Ettiren Bilgileri Hazırlama

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                var sigEttiren = teklif.SigortaEttiren;
                var sigEttirenGenelBilgi = sigEttiren.MusteriGenelBilgiler;
                short sigortaliKimlikType = 1;
                bool MusteriAyniMi = false;

                if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                {
                    sigortaliKimlikType = ERGOKimlikTipleri.TC;
                }
                else if (sigortali.MusteriTipKodu == MusteriTipleri.SahisFirmasi || sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                {
                    sigortaliKimlikType = ERGOKimlikTipleri.Vergi;
                }
                else if (sigortali.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                {
                    sigortaliKimlikType = ERGOKimlikTipleri.Yabanci;
                }

                short sigortaEttirenKimlikType = sigortaliKimlikType;
                if (sigEttiren.MusteriKodu == sigortali.MusteriKodu)
                {
                    MusteriAyniMi = true;
                }
                if (!MusteriAyniMi)
                {
                    if (sigEttirenGenelBilgi.MusteriTipKodu == MusteriTipleri.TCMusteri)
                    {
                        sigortaEttirenKimlikType = ERGOKimlikTipleri.TC;
                    }
                    else if (sigEttirenGenelBilgi.MusteriTipKodu == MusteriTipleri.SahisFirmasi || sigEttirenGenelBilgi.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                    {
                        sigortaEttirenKimlikType = ERGOKimlikTipleri.Vergi;
                    }
                    else if (sigEttirenGenelBilgi.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                    {
                        sigortaEttirenKimlikType = ERGOKimlikTipleri.Yabanci;
                    }
                }
                #endregion

                #region Araç Bilgileri


                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                string AnaKullanimTarzi = "";
                string AltKullanimTarzi = "";
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.ERGO &&
                                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        var ErgoKullanimTarzi = kullanimTarzi.TarifeKodu.Split('-');
                        if (ErgoKullanimTarzi.Length == 2)
                        {
                            AnaKullanimTarzi = ErgoKullanimTarzi[0];
                            AltKullanimTarzi = ErgoKullanimTarzi[1];
                        }
                    }
                }
                #endregion

                #endregion

                #region Info Servise 

                InfoService clntInfo = new InfoService();
                clntInfo.Url = konfig[Konfig.ERGO_InfoServiceURL]; ;
                UsernameToken token = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
                clntInfo.RequestSoapContext.Security.Tokens.Add(token);
                clntInfo.RequestSoapContext.Security.MustUnderstand = false;
                clntInfo.RequestSoapContext.Security.Actor = "";

                #endregion

                #region Web Servis Access

                CascoService client = new CascoService();
                client.Url = konfig[Konfig.ERGO_KaskoServiceURL];
                client.Timeout = 150000;
                ////WEB Service e username token ekleniyor
                UsernameToken tokenKasko = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
                client.RequestSoapContext.Security.Tokens.Add(tokenKasko);
                client.RequestSoapContext.Security.MustUnderstand = false;
                client.RequestSoapContext.Security.Actor = "";

                #endregion

                #region Web Servis Requesti Oluşturma
                proposalWsDto request = new proposalWsDto()
                {
                    agencyNumber = Convert.ToInt32(servisKullanici.PartajNo_),
                    agencySellerNumber = 0,
                    agencySubNumber = Convert.ToInt32(servisKullanici.SubAgencyCode),
                    authorizationCode = "0",
                    beginDate = polBaslangic,
                    endDate = polBaslangic.AddYears(1),
                    issueDate = polBaslangic,
                    currencyCode = "TL",
                    paymentPlanId = teklif.GenelBilgiler.TaksitSayisi.Value,
                    productNumber = ERGOUrunKodlari.Kasko,

                };

                #region Odeme Tipi
                if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    request.paymentTypeId = ERGOOdemeTipleri.KrediKarti;
                }
                else if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    request.paymentTypeId = ERGOOdemeTipleri.BlokeKart;
                }
                else if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.Nakit)
                {
                    request.paymentTypeId = ERGOOdemeTipleri.Nakit;
                }
                else if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.Havale)
                {
                    request.paymentTypeId = ERGOOdemeTipleri.Havale;
                }
                else if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.CekSenet)
                {
                    request.paymentTypeId = ERGOOdemeTipleri.Cek;
                }

                #endregion

                #region Menfaat Listesi
                List<ergoturkiye.kasko.benefitWsDto> blist = new List<ergoturkiye.kasko.benefitWsDto>();

                //benefitId
                //benefitCode (Menfaat kodu)
                //benefitName (Menfaat adı)
                //suminsured (Menfaat bedeli)
                #region Zorunlu Menfaatlar
                ergoturkiye.kasko.benefitWsDto benefit = new ergoturkiye.kasko.benefitWsDto
                {
                    benefitCode = ERGOKaskoBenefitList.arac,
                    suminsured = teklif.Arac.AracDeger.HasValue ? Convert.ToInt32(teklif.Arac.AracDeger.Value) : 0,

                }; blist.Add(benefit);
                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
                {
                    CR_KaskoFK CRKademeFk = new CR_KaskoFK();
                    //Sigorta Şirketinin Ferdi Kaza Limit listesinde kullanım tarzı fark etmiyorsa 111-10 limitler ekleniyor
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), "111", "10");

                    if (FKBedel != null)
                    {
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.olumSurekliSakatlikYolcu,
                            suminsured = FKBedel.Sakatlik.HasValue ? FKBedel.Sakatlik.Value : 0
                        };
                        blist.Add(benefit);
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.olumSurekliSakatlikSurucu,
                            suminsured = FKBedel.Sakatlik.HasValue ? FKBedel.Sakatlik.Value : 0

                        };
                        blist.Add(benefit);
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.tedaviMasraflariSurucu,
                            suminsured = FKBedel.Tedavi.HasValue ? FKBedel.Tedavi.Value : 0

                        };
                        blist.Add(benefit);
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.tedaviMasraflariYolcu,
                            suminsured = FKBedel.Tedavi.HasValue ? FKBedel.Tedavi.Value : 0

                        };
                        blist.Add(benefit);
                    }
                }

                string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();
                if (!String.IsNullOrEmpty(immKademe))
                {
                    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);
                    if (IMMBedel != null)
                    {
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.tefriksizBedeniMaddiKazaBasi,
                            suminsured = IMMBedel.Kombine.HasValue ? IMMBedel.Kombine.Value : 0

                        };
                        blist.Add(benefit);
                    }
                }
                //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                string hukuksalKoruma = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, "");
                if (!String.IsNullOrEmpty(hukuksalKoruma))
                {
                    var hkBedel = _TeklifService.getHkKademesi(Convert.ToInt32(hukuksalKoruma));
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.hukuksalKorumaSurucu,
                        suminsured = hkBedel

                    };
                    blist.Add(benefit);

                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.hukuksalKorumaArac,
                        suminsured = hkBedel

                    };
                    blist.Add(benefit);
                }

                #endregion

                #region Kullanım Tarzına Göre Ayrımlı Olanlar


                if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Taksi)
                {

                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.taksimetre,
                        suminsured = 0

                    };
                    blist.Add(benefit);
                }
                if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Dolmus9Koltuk)
                {

                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.isDurmasi,
                        suminsured = 0

                    };
                    blist.Add(benefit);
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.gap,
                        suminsured = 0

                    };
                    blist.Add(benefit);
                }
                if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Otomobil9Koltuk)
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.ozelEsya,
                        suminsured = 1000

                    };
                    blist.Add(benefit);

                }
                if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Otomobil9Koltuk || AltKullanimTarzi == ErgoAltKullanimTarzlari.Kamyonet)
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.anahtar,
                        suminsured = 2000

                    };
                    blist.Add(benefit);
                    //benefit = new ergoturkiye.kasko.benefitWsDto
                    //{
                    //    benefitCode = ERGOKaskoBenefitList.bireyselSorumluluk,
                    //    suminsured = 0

                    //};
                    //blist.Add(benefit);
                }
                if (AltKullanimTarzi == ErgoAltKullanimTarzlari.OzelKullanim1830 || AltKullanimTarzi == ErgoAltKullanimTarzlari.Kamyonet)
                {
                    if (teklif.ReadSoru(KaskoSorular.Teminat_TasinanYuk_VarYok, false))
                    {
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.yuk,
                            suminsured = 0

                        };
                        blist.Add(benefit);
                    }
                }
                if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Kamyonet)
                {
                    if (teklif.ReadSoru(KaskoSorular.Teminat_TasinanYuk_VarYok, false))
                    {
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.yuk3kisi,
                            suminsured = 0

                        };
                        blist.Add(benefit);
                    }
                    decimal? KasaBedeli = 0;
                    if (aksesuarlar.Count > 0)
                    {
                        var kasaVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreAksesuarTipleri.Kasa).FirstOrDefault();
                        if (kasaVarMi != null)
                        {
                            KasaBedeli = kasaVarMi.Bedel;
                        }
                    }
                    if (KasaBedeli != 0)
                    {
                        benefit = new ergoturkiye.kasko.benefitWsDto
                        {
                            benefitCode = ERGOKaskoBenefitList.kasa,
                            suminsured = KasaBedeli.Value,
                        };
                        blist.Add(benefit);
                    }
                }
                //if (AltKullanimTarzi != ErgoAltKullanimTarzlari.DolmusHatli1017)
                //{
                //    benefit = new ergoturkiye.kasko.benefitWsDto
                //    {
                //        benefitCode = ERGOKaskoBenefitList.aracTelefonu,
                //        suminsured = 0
                //    };
                //    blist.Add(benefit);
                //}

                #endregion

                #region Diğer Menfaatlar
                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.acilSaglik,
                //    suminsured = 0

                //};
                //blist.Add(benefit);

                if (teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false))
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.alarm,
                        suminsured = 0

                    };
                    blist.Add(benefit);
                }

                decimal? JantBedeli = 0;
                if (aksesuarlar.Count > 0)
                {
                    var jantVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreAksesuarTipleri.Jant).FirstOrDefault();
                    if (jantVarMi != null)
                    {
                        JantBedeli = jantVarMi.Bedel;
                    }
                }
                if (JantBedeli != 0)
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.celikJant,
                        suminsured = JantBedeli.Value

                    };
                    blist.Add(benefit);
                }

                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.deriDoseme,
                //    suminsured = 0

                //};
                //blist.Add(benefit);

                decimal? DVDBedeli = 0;
                if (aksesuarlar.Count > 0)
                {
                    var DVDVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreElektronikCihazTipleri.DVD).FirstOrDefault();
                    if (DVDVarMi != null)
                    {
                        DVDBedeli = DVDVarMi.Bedel;
                    }
                }
                if (DVDBedeli != 0)
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.dvdVcdOynatici,
                        suminsured = DVDBedeli.Value

                    };
                    blist.Add(benefit);
                }
                decimal? RadyoTeypBedeli = 0;
                if (aksesuarlar.Count > 0)
                {
                    var RadyoTeypVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreElektronikCihazTipleri.RT).FirstOrDefault();
                    if (RadyoTeypVarMi != null)
                    {
                        RadyoTeypBedeli = RadyoTeypVarMi.Bedel;
                    }
                }
                if (RadyoTeypBedeli != 0)
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.radyoTeyp,
                        suminsured = RadyoTeypBedeli.Value

                    };
                    blist.Add(benefit);
                }
                decimal? LPGBedeli = 0;
                if (aksesuarlar.Count > 0)
                {
                    var LPGVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreAksesuarTipleri.LPG).FirstOrDefault();
                    if (LPGVarMi != null)
                    {
                        LPGBedeli = LPGVarMi.Bedel;
                    }
                }
                if (LPGBedeli != 0)
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.lpgTanki,
                        suminsured = LPGBedeli.Value

                    };
                    blist.Add(benefit);
                }
                decimal? DigerAksesuarBedeli = 0;
                if (aksesuarlar.Count > 0)
                {
                    var DigerAksesuarVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreAksesuarTipleri.Diger).FirstOrDefault();
                    if (DigerAksesuarVarMi != null)
                    {
                        DigerAksesuarBedeli = DigerAksesuarVarMi.Bedel;
                    }
                }
                if (DigerAksesuarBedeli != 0)
                {
                    benefit = new ergoturkiye.kasko.benefitWsDto
                    {
                        benefitCode = ERGOKaskoBenefitList.digerAksesuar,
                        suminsured = DigerAksesuarBedeli.Value

                    };
                    blist.Add(benefit);
                }

                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.klima,
                //    suminsured = 0

                //};
                //blist.Add(benefit);
                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.kolon,
                //    suminsured = 0

                //};
                //blist.Add(benefit);
                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.ozelLastik,
                //    suminsured = 0

                //};
                //blist.Add(benefit);
                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.parkSensoru,
                //    suminsured = 0

                //};
                //blist.Add(benefit);

                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.televizyon,
                //    suminsured = 0

                //};
                //blist.Add(benefit);
                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.telsiz,
                //    suminsured = 0

                //};
                //blist.Add(benefit);
                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.xenonFar,
                //    suminsured = 0

                //};
                //blist.Add(benefit);             

                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.ucuncuSahisBedeniZararKisiBasi,
                //    suminsured = 0

                //};
                //blist.Add(benefit);

                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.ucuncuSahisBedeniZararKazaBasi,
                //    suminsured = 0

                //};
                //blist.Add(benefit);

                //benefit = new benefitWsDto
                //{
                //    benefitCode = ERGOKaskoBenefitList.ucuncuSahisMaddiZararKazaBasi,
                //    suminsured = 0

                //};
                //blist.Add(benefit);


                #endregion

                request.benefitList = blist.ToArray();

                #endregion

                #region Teminat Listesi

                List<ergoturkiye.kasko.coverageWsDto> coverageLists = new List<ergoturkiye.kasko.coverageWsDto>();
                ergoturkiye.kasko.coverageWsDto coverageWsDto = new ergoturkiye.kasko.coverageWsDto();

                #region Zorunlu Teminatlar //Zorunlu teminatlar gönderilmese de otomatik Ergo sisteminde işleniyor
                //#region Ana Teminat

                //coverageWsDto.coverageCode = ERGOCoverageList.kasko;
                //coverageWsDto.coverageId = 1006;
                //coverageWsDto.coverageName = "KASKO(YANMA,ÇALINMA,ÇARPISMA)";
                //coverageList.Add(coverageWsDto);

                //#endregion



                //coverageWsDto.coverageCode = ERGOCoverageList.acilSaglik;
                //coverageWsDto.coverageId = 1016;
                //coverageWsDto.coverageName = "ACIL SAGLIK";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.GLKHHKNHT;
                //coverageWsDto.coverageId = 1002;
                //coverageWsDto.coverageName = "GLKHHKNHT";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.selSuBaskini;
                //coverageWsDto.coverageId = 1003;
                //coverageWsDto.coverageName = "SEL VE SU BASKINI";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.tefriksiz;
                //coverageWsDto.coverageId = 1021;
                //coverageWsDto.coverageName = "TEFRİKSİZ(BEDENİ-MADDİ)";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.ucuncuSahisMaddiZarar;
                //coverageWsDto.coverageId = 1019;
                //coverageWsDto.coverageName = "3.ŞAHIS MADDİ ZARAR";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.ucuncuSahisBedeniZarar;
                //coverageWsDto.coverageId = 1020;
                //coverageWsDto.coverageName = "3.ŞAHIS BEDENİ ZARAR";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.tedaviMasraflari;
                //coverageWsDto.coverageId = 1018;
                //coverageWsDto.coverageName = "TEDAVİ MASRAFLARI";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.olumsurekliSakatlik;
                //coverageWsDto.coverageId = 1017;
                //coverageWsDto.coverageName = "ÖLÜM SÜREKLİ SAKATLIK";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.depremVeYanardag;
                //coverageWsDto.coverageId = 1001;
                //coverageWsDto.coverageName = "DEPREM VE YANARDAG P.";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.hukuksalKoruma;
                //coverageWsDto.coverageId = 1009;
                //coverageWsDto.coverageName = "HUKUKSAL KORUMA";
                //coverageList.Add(coverageWsDto);

                //coverageWsDto.coverageCode = ERGOCoverageList.hukuksalKoruma;
                //coverageWsDto.coverageId = 1009;
                //coverageWsDto.coverageName = "HUKUKSAL KORUMA";
                //coverageList.Add(coverageWsDto);

                //if (AltKullanimTarzi!=ErgoAltKullanimTarzlari.OtobusDolmusHatli)
                //{
                //    coverageWsDto.coverageCode = ERGOCoverageList.kucukHasarOnarim;
                //    coverageWsDto.coverageId = 1004;
                //    coverageWsDto.coverageName = "KÜÇÜK HASAR ONARIM";
                //    coverageList.Add(coverageWsDto);
                //}
                //if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Kamyonet || AltKullanimTarzi == ErgoAltKullanimTarzlari.OzelKullanim)
                //{
                //    coverageWsDto.coverageCode = ERGOCoverageList.yukKasko;
                //    coverageWsDto.coverageId = 1008;
                //    coverageWsDto.coverageName = "YÜK KASKO";
                //    coverageList.Add(coverageWsDto);
                //}
                //if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Kamyonet || AltKullanimTarzi == ErgoAltKullanimTarzlari.Otomobil9Koltuk)
                //{
                //    coverageWsDto.coverageCode = ERGOCoverageList.bireyselSorumluluk;
                //    coverageWsDto.coverageId = 1014;
                //    coverageWsDto.coverageName = "BİREYSEL SORUMLULUK";
                //    coverageList.Add(coverageWsDto);

                //    coverageWsDto.coverageCode = ERGOCoverageList.anahtarKaybi;
                //    coverageWsDto.coverageId = 1010;
                //    coverageWsDto.coverageName = "ANAHTAR KAYBI";
                //    coverageList.Add(coverageWsDto);

                //    coverageWsDto.coverageCode = ERGOCoverageList.asistans;
                //    coverageWsDto.coverageId = 1030;
                //    coverageWsDto.coverageName = "ASİSTANS";
                //    coverageList.Add(coverageWsDto);
                //}

                //if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Otomobil9Koltuk)
                //{
                //    coverageWsDto.coverageCode = ERGOCoverageList.ozelEsya;
                //    coverageWsDto.coverageId = 1011;
                //    coverageWsDto.coverageName = "ÖZEL EŞYA";
                //    coverageList.Add(coverageWsDto);
                //}
                //if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Kamyonet)
                //{
                //    coverageWsDto.coverageCode = ERGOCoverageList.yuk3Kisi;
                //    coverageWsDto.coverageId = 1039;
                //    coverageWsDto.coverageName = "YÜK 3. KİŞİ";
                //    coverageList.Add(coverageWsDto);
                //}
                //request.coverageList = coverageList.ToArray();
                #endregion

                #region Seçimli Teminatlar
                string ErgoIkameTuru = "";

                string ikame = teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                switch (ikame)
                {
                    case "ABC07":
                        {

                            coverageWsDto.coverageCode = ERGOCoverageList.asistansIkameAracYilda7Gun3Kez;
                            coverageWsDto.coverageId = 1022;
                            coverageWsDto.coverageName = "ASİSTANS-İKAME ARAÇ (YILDA 7 GÜN/3 KEZ)";
                            coverageLists.Add(coverageWsDto);
                        }
                        break;
                    case "ABC14":
                        {
                            coverageWsDto.coverageCode = ERGOCoverageList.asistansIkameAracYilda15Gun2Kez;
                            coverageWsDto.coverageId = 1023;
                            coverageWsDto.coverageName = "ASİSTANS-İKAME ARAÇ (YILDA 15 GÜN/2 KEZ)";
                            coverageLists.Add(coverageWsDto);
                        }
                        break;
                }
                request.coverageList = coverageLists.ToArray();

                #endregion

                #endregion

                #region Customer List
                List<customerWsDto> customerList = new List<customerWsDto>();
                customerWsDto customerWsDto = new customerWsDto
                {
                    cityId = 0,
                    customerTypeId = ERGOMusteriTipleri.Sigortali,
                    identityNo = sigortali.KimlikNo,
                    identityTypeId = sigortaliKimlikType,
                    customerName = sigortali.AdiUnvan,
                    customerSurname = sigortali.SoyadiUnvan,

                };

                if (sigortaliKimlikType == 2)
                {
                    MusteriAdre sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                    customerWsDto.cityId = 34;
                    customerWsDto.districtId = 34029;
                    customerWsDto.addressText = "İSTANBUL";
                    if (!String.IsNullOrEmpty(sigortaliAdres.IlKodu) && sigortaliAdres.IlceKodu.HasValue)
                    {
                        var neoonlineIl = _UlkeService.GetIlAdi("TUR", sigortaliAdres.IlKodu);
                        var neoonlineIlce = _UlkeService.GetIlceAdi(sigortaliAdres.IlceKodu.Value);
                        var ErgoIlIlce = _UlkeService.GetErgoIlIlce(neoonlineIl, neoonlineIlce);
                        if (ErgoIlIlce != null)
                        {
                            customerWsDto.cityId = ErgoIlIlce.IlKodu;
                            customerWsDto.districtId = ErgoIlIlce.TramerDistrictId;
                            customerWsDto.addressText = ErgoIlIlce.IlAdi + " " + ErgoIlIlce.IlceAdi;
                        }
                    }

                }
                customerList.Add(customerWsDto);
                customerWsDto = new customerWsDto
                {
                    cityId = 0,
                    customerTypeId = ERGOMusteriTipleri.SigortaEttiren,
                    identityNo = MusteriAyniMi ? sigortali.KimlikNo : sigEttirenGenelBilgi.KimlikNo,
                    identityTypeId = sigortaEttirenKimlikType,
                    customerName = MusteriAyniMi ? sigortali.AdiUnvan : sigEttirenGenelBilgi.AdiUnvan,
                    customerSurname = MusteriAyniMi ? sigortali.SoyadiUnvan : sigEttirenGenelBilgi.SoyadiUnvan

                };
                if (sigortaliKimlikType == 2)
                {
                    MusteriAdre sigortaEttirenAdres = sigEttirenGenelBilgi.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                    customerWsDto.cityId = 34;
                    customerWsDto.districtId = 34029;
                    customerWsDto.addressText = "İSTANBUL";
                    if (!String.IsNullOrEmpty(sigortaEttirenAdres.IlKodu) && sigortaEttirenAdres.IlceKodu.HasValue)
                    {
                        var neoonlineIl = _UlkeService.GetIlAdi("TUR", sigortaEttirenAdres.IlKodu);
                        var neoonlineIlce = _UlkeService.GetIlceAdi(sigortaEttirenAdres.IlceKodu.Value);
                        var ErgoIlIlce = _UlkeService.GetErgoIlIlce(neoonlineIl, neoonlineIlce);
                        if (ErgoIlIlce != null)
                        {
                            customerWsDto.cityId = ErgoIlIlce.IlKodu;
                            customerWsDto.districtId = ErgoIlIlce.TramerDistrictId;
                            customerWsDto.addressText = ErgoIlIlce.IlAdi + " " + ErgoIlIlce.IlceAdi;
                        }
                    }
                }
                customerList.Add(customerWsDto);
                request.customerList = customerList.ToArray();
                #endregion

                #region Property List

                string plakaRakam = "";
                string plakaHarf = "";
                foreach (char ch in teklif.Arac.PlakaNo)
                {
                    if (Char.IsDigit(ch))
                        plakaRakam += ch;
                    if (Char.IsLetter(ch))
                        plakaHarf += ch;
                }


                string MarkaModel = teklif.Arac.Marka + teklif.Arac.AracinTipi.PadLeft(3, '0');
                List<pPropertyWsDto> propertyList = new List<pPropertyWsDto>();

                pPropertyWsDto pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracKullanimTarzi;
                pPropertyWsDto.propertyValueId = !String.IsNullOrEmpty(AnaKullanimTarzi) ? Convert.ToInt32(AnaKullanimTarzi) : 3;
                propertyList.Add(pPropertyWsDto);

                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracAltKullanimTarzi;
                pPropertyWsDto.propertyValueId = !String.IsNullOrEmpty(AltKullanimTarzi) ? Convert.ToInt32(AltKullanimTarzi) : 24;
                propertyList.Add(pPropertyWsDto);

                var plakaValueId = clntInfo.retrievePropertyValue("@plaka1", teklif.Arac.PlakaKodu);
                clntInfo.Dispose();
                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.plaka1;
                pPropertyWsDto.propertyValueId = plakaValueId.propertyValueId;
                pPropertyWsDto.propertyValueStr = teklif.Arac.PlakaKodu.PadLeft(3, '0');
                propertyList.Add(pPropertyWsDto);

                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.plaka2;
                pPropertyWsDto.propertyValueStr = plakaHarf;
                propertyList.Add(pPropertyWsDto);

                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.plaka3;
                pPropertyWsDto.propertyValueStr = plakaRakam;
                propertyList.Add(pPropertyWsDto);
                if (teklif.Arac.Model.HasValue)
                {
                    var ModelValueId = clntInfo.retrievePropertyValue(ERGOKaskoPropertyList.aracModelYili, teklif.Arac.Model.Value.ToString());
                    clntInfo.Dispose();
                    pPropertyWsDto = new pPropertyWsDto();
                    pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracModelYili;
                    pPropertyWsDto.propertyValueId = ModelValueId.propertyValueId;
                    pPropertyWsDto.propertyValueStr = teklif.Arac.Model.Value.ToString();
                    propertyList.Add(pPropertyWsDto);
                }


                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracBirlikKod;
                pPropertyWsDto.propertyValueStr = MarkaModel;
                propertyList.Add(pPropertyWsDto);

                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracSasiNo;
                pPropertyWsDto.propertyValueStr = teklif.Arac.SasiNo;
                propertyList.Add(pPropertyWsDto);

                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracMotorNo;
                pPropertyWsDto.propertyValueStr = teklif.Arac.MotorNo;
                propertyList.Add(pPropertyWsDto);

                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracTescilTarihi;
                pPropertyWsDto.propertyValueStr = teklif.Arac.TrafikTescilTarihi.Value.ToString("dd/MM/yyyy").Replace('.', '/');
                propertyList.Add(pPropertyWsDto);

                pPropertyWsDto = new pPropertyWsDto();
                pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.aracKoltukAdet;
                pPropertyWsDto.propertyValueStr = teklif.Arac.KoltukSayisi.HasValue ? (teklif.Arac.KoltukSayisi.Value - 1).ToString() : "4";
                propertyList.Add(pPropertyWsDto);

                if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriNo))
                {
                    if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod))
                    {
                        pPropertyWsDto = new pPropertyWsDto();
                        pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.tescilBelgeSeriNo1;
                        pPropertyWsDto.propertyValueStr = teklif.Arac.TescilSeriKod;
                        propertyList.Add(pPropertyWsDto);
                    }
                    pPropertyWsDto = new pPropertyWsDto();
                    pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.tescilBelgeSeriNo2;
                    pPropertyWsDto.propertyValueStr = teklif.Arac.TescilSeriNo;
                    propertyList.Add(pPropertyWsDto);

                }
                else
                {
                    pPropertyWsDto = new pPropertyWsDto();
                    pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.tescilBelgeSeriNo2;
                    pPropertyWsDto.propertyValueStr = teklif.Arac.AsbisNo;
                    propertyList.Add(pPropertyWsDto);
                }
                var meslekId = teklif.ReadSoru(KaskoSorular.ErgoMeslekKodu, null);
                if (meslekId != null)
                {
                    pPropertyWsDto = new pPropertyWsDto();
                    pPropertyWsDto.propertyCode = ERGOKaskoPropertyList.meslekBilgisi;
                    pPropertyWsDto.propertyValueId = Convert.ToInt32(meslekId);
                    propertyList.Add(pPropertyWsDto);
                }

                request.pPropertyList = propertyList.ToArray();
                #endregion

                #region Discount Sur List

                List<ergoturkiye.kasko.discountSurchargeWsDto> discountSurList = new List<ergoturkiye.kasko.discountSurchargeWsDto>();
                ergoturkiye.kasko.discountSurchargeWsDto discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();

                string kaskoServisi = teklif.ReadSoru(KaskoSorular.ErgoServisSecenegi, "0");
                switch (kaskoServisi)
                {
                    case "1":
                        { //Tüm servisler ise
                            discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                            discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.tumServisler;
                            discountSurchargeWsDto.disSurId = 3188;
                            discountSurchargeWsDto.disSurName = "SB5 - TÜM SERVİSLER";
                            discountSurList.Add(discountSurchargeWsDto);
                        }
                        break;
                    case "2":
                        {
                            discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                            discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.tumAnlasmaliServisler;
                            discountSurchargeWsDto.disSurId = 3213;
                            discountSurchargeWsDto.disSurName = "SB4-TÜM ANLAŞMALI SERVİSLER";
                            discountSurList.Add(discountSurchargeWsDto);
                        }
                        break;
                    case "3":
                        {
                            discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                            discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.sadeceAnlasmaliOzelServisler;
                            discountSurchargeWsDto.disSurId = 3040;
                            discountSurchargeWsDto.disSurName = "SB3 - Sadece Anlaşmalı Özel Servis";
                            discountSurList.Add(discountSurchargeWsDto);
                        }
                        break;
                    case "4":
                        {
                            discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                            discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.tumAnlasmaliServislerMuafiyet;
                            discountSurchargeWsDto.disSurId = 3091;
                            discountSurchargeWsDto.disSurName = "D26- (%2 Muafiyet) TÜM ANLAŞMALI SERVİSLER";
                            discountSurList.Add(discountSurchargeWsDto);
                        }
                        break;
                }
                if (AltKullanimTarzi == ErgoAltKullanimTarzlari.Kamyonet || AltKullanimTarzi == ErgoAltKullanimTarzlari.Otomobil9Koltuk)
                {
                    if (teklif.ReadSoru(KaskoSorular.Hasarsizlik_Koruma_VarYok, false))
                    {
                        discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                        discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.hasarsizlikKoruma;
                        discountSurchargeWsDto.disSurId = 3065;
                        discountSurchargeWsDto.disSurName = "S20-HASARSIZLIK KORUMA";
                        discountSurList.Add(discountSurchargeWsDto);
                    }
                }

                var deprem = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);
                if (deprem)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.depremMuafiyetsiz;
                    discountSurchargeWsDto.disSurId = 3052;
                    discountSurchargeWsDto.disSurName = "S56-DEPREM MUAFİYETSİZ";
                    discountSurList.Add(discountSurchargeWsDto);
                }

                var camMuafiyeti = teklif.ReadSoru(KaskoSorular.CamMuafiyetiKaldirilsinMi, true);
                if (camMuafiyeti)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.muafiyetsizCam;
                    discountSurList.Add(discountSurchargeWsDto);
                }
                var yurtDisi = teklif.ReadSoru(KaskoSorular.YurtdisiKasko, false);
                if (yurtDisi)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.yurtDisi;
                    discountSurchargeWsDto.disSurId = 3130;
                    discountSurchargeWsDto.disSurName = "S34-YURT DISI";
                    discountSurList.Add(discountSurchargeWsDto);
                }
                var YetkiliOlmayanCekilme = teklif.ReadSoru(KaskoSorular.YetkiliOlmayanCekilme, false);
                if (YetkiliOlmayanCekilme)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.yetkiliOlmayanCekme;
                    discountSurchargeWsDto.disSurId = 3224;
                    discountSurchargeWsDto.disSurName = "S33-YETKILI OLMAYAN ÇEKME";
                    discountSurList.Add(discountSurchargeWsDto);
                }
                var SigaraYanigi = teklif.ReadSoru(KaskoSorular.SigaraYanigi, false);
                if (SigaraYanigi)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.sigaraYanigi;
                    discountSurchargeWsDto.disSurId = 3139;
                    discountSurchargeWsDto.disSurName = "S31-SIGARA YANIGI";
                    discountSurList.Add(discountSurchargeWsDto);
                }
                var KiymetKazanma = teklif.ReadSoru(KaskoSorular.KiymetKazanma, false);
                if (KiymetKazanma)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.kiymetKazanma2;
                    discountSurchargeWsDto.disSurId = 3171;
                    discountSurchargeWsDto.disSurName = "S15-KIYMET KAZANMA";
                    discountSurList.Add(discountSurchargeWsDto);
                }
                var AnahtarliCalinma = teklif.ReadSoru(KaskoSorular.AnahtarliCalinma, false);
                if (AnahtarliCalinma)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.anahtarlaCalinma;
                    discountSurchargeWsDto.disSurId = 3157;
                    discountSurchargeWsDto.disSurName = "S61-ANAHTARLI ÇALINMA";
                    discountSurList.Add(discountSurchargeWsDto);
                }
                var seylap = teklif.ReadSoru(KaskoSorular.Seylap, false);
                if (seylap)
                {
                    discountSurchargeWsDto = new ergoturkiye.kasko.discountSurchargeWsDto();
                    discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.seylapMuafiyetsiz;
                    discountSurchargeWsDto.disSurId = 3173;
                    discountSurchargeWsDto.disSurName = "S55-SEYLAP MUAFİYETSİZ";
                    discountSurList.Add(discountSurchargeWsDto);
                }

                //discountSurchargeWsDto = new discountSurchargeWsDto();
                //discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.yeniDeger1Yil;
                //discountSurList.Add(discountSurchargeWsDto);

                //discountSurchargeWsDto = new discountSurchargeWsDto();
                //discountSurchargeWsDto.disSurCode = ERGOKaskoDiscountList.patlayiciParlayiciMadde;
                //discountSurList.Add(discountSurchargeWsDto);

                request.discountSurchargeList = discountSurList.ToArray();
                #endregion

                #region Dain-i Murtein Listesi

                bool dainiMurteinVar = teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
                string dainiMurteinKimlikNo = teklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, String.Empty);
                var kurutipi = teklif.ReadSoru(KaskoSorular.DainiMurtein_KurumTipi, String.Empty);

                if (dainiMurteinVar && !String.IsNullOrEmpty(dainiMurteinKimlikNo))
                {
                    ergoturkiye.kasko.lossPayeeTypeWsDto ergoKurumTipi = ergoturkiye.kasko.lossPayeeTypeWsDto.BANK;
                    if (kurutipi == "1")
                    {
                        ergoKurumTipi = ergoturkiye.kasko.lossPayeeTypeWsDto.BANK;
                    }
                    else if (kurutipi == "2")
                    {
                        ergoKurumTipi = ergoturkiye.kasko.lossPayeeTypeWsDto.FINANCE;
                    }
                    else
                    {
                        ergoKurumTipi = ergoturkiye.kasko.lossPayeeTypeWsDto.OTHER;
                    }

                    var responseDainiMurtein = clntInfo.retrieveLossPayee(dainiMurteinKimlikNo, sigortaEttirenKimlikType);
                    clntInfo.Dispose();
                    request.lossPayee = new lossPayeeWsDto()
                    {
                        bankCode = responseDainiMurtein.bank.bankCode,
                        branchCode = responseDainiMurtein.bankBranch.branchCode,
                        financeBankCode = responseDainiMurtein.financeBank.financeBankCode,
                        identityNo = responseDainiMurtein.identityNo,
                        identityTypeId = responseDainiMurtein.identityTypeId,
                    };
                }

                #endregion

                #endregion

                #region Web Servisi Çağırma
                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);
                var response = client.saveCascoProposal(request);
                client.Dispose();
                this.EndLog(response, true, response.GetType());

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

                foreach (var item in response)
                {
                    this.GenelBilgiler.TUMTeklifNo = item.policyNo.ToString();
                    this.GenelBilgiler.Basarili = true;
                    this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                    this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                    this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                    this.GenelBilgiler.BrutPrim = item.premium.totalGrossPremium;
                    this.GenelBilgiler.NetPrim = item.premium.totalPremium;
                    this.GenelBilgiler.ToplamKomisyon = item.premium.totalComm;
                    this.GenelBilgiler.ToplamVergi = item.premium.totalTax;

                    #region Teklif PDF
                    try
                    {
                        string teklifPDFURL = String.Empty;
                        if (item.policyNo > 0)
                        {
                            this.EndLog(response, true, response.GetType());
                            //UsernameToken tokenPDF = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
                            //client.RequestSoapContext.Security.Tokens.Add(tokenPDF);
                            //client.RequestSoapContext.Security.MustUnderstand = false;
                            //client.RequestSoapContext.Security.Actor = "";                            

                            printPolicyWsDto[] pdfResponse = client.printProposal(item.policyNo, item.proposalVerNo);
                            client.Dispose();
                            if (pdfResponse != null)
                            {
                                foreach (var itemPDF in pdfResponse)
                                {
                                    byte[] data = itemPDF.bytes;
                                    if (itemPDF.printType == printTypeWsDto.PRINT)
                                    {
                                        ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                                        string fileName = String.Format("ERGO_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                                        teklifPDFURL = storage.UploadFile("kasko", fileName, data);
                                    }
                                }
                            }
                        }
                        this.GenelBilgiler.PDFDosyasi = teklifPDFURL;
                    }
                    catch (Exception)
                    {

                        this.GenelBilgiler.PDFDosyasi = "";
                    }

                    #endregion

                    #region ERGO Vergiler

                    var vergiler = item.premium.taxWsDtoList;
                    if (vergiler != null)
                    {
                        this.AddVergi(TrafikVergiler.GiderVergisi, vergiler.Where(s => s.taxType.taxTypeId == ERGOVergiTipleri.GiderVerigisi).Select(s => s.taxAmount).FirstOrDefault());
                        this.AddVergi(TrafikVergiler.THGFonu, vergiler.Where(s => s.taxType.taxTypeId == ERGOVergiTipleri.GelistirmeFonu).Select(s => s.taxAmount).FirstOrDefault());
                        this.AddVergi(TrafikVergiler.GarantiFonu, vergiler.Where(s => s.taxType.taxTypeId == ERGOVergiTipleri.GarantiFonu).Select(s => s.taxAmount).FirstOrDefault());
                    }
                    #endregion

                    #region Teminatlar

                    if (camMuafiyeti)
                    {
                        this.AddTeminat(KaskoTeminatlar.CamMuafiyeti, 0, 0, 0, 0, 0);
                    }


                    decimal suminsured = 0;
                    decimal premium = 0;
                    var benefitCoverageList = item.benefitCoverageList;

                    if (benefitCoverageList != null)
                    {
                        foreach (var itemBenCov in benefitCoverageList)
                        {
                            if (itemBenCov.benefit != null)
                            {
                                if (itemBenCov.benefit.benefitCode == ERGOKaskoBenefitList.arac && itemBenCov.coverage.coverageCode == ERGOCoverageList.depremVeYanardag)
                                {
                                    suminsured = itemBenCov.suminsured;
                                    premium = itemBenCov.premium;
                                    this.AddTeminat(KaskoTeminatlar.Deprem, suminsured, 0, premium, 0, 0); continue;
                                }
                                if (itemBenCov.benefit.benefitCode == ERGOKaskoBenefitList.arac && itemBenCov.coverage.coverageCode == ERGOCoverageList.GLKHHKNHT)
                                {
                                    suminsured = itemBenCov.suminsured;
                                    premium = itemBenCov.premium;
                                    this.AddTeminat(KaskoTeminatlar.GLKHHT, suminsured, 0, premium, 0, 0); continue;
                                }
                                if (itemBenCov.benefit.benefitCode == ERGOKaskoBenefitList.arac && itemBenCov.coverage.coverageCode == ERGOCoverageList.selSuBaskini)
                                {
                                    suminsured = itemBenCov.suminsured;
                                    premium = itemBenCov.premium;
                                    this.AddTeminat(KaskoTeminatlar.SelSuBaskini, suminsured, 0, premium, 0, 0); continue;
                                }
                                if (itemBenCov.benefit.benefitCode == ERGOKaskoBenefitList.arac && itemBenCov.coverage.coverageCode == ERGOCoverageList.kasko)
                                {
                                    suminsured = itemBenCov.suminsured;
                                    premium = itemBenCov.premium;
                                    this.AddTeminat(KaskoTeminatlar.Kasko, suminsured, 0, premium, 0, 0); continue;
                                }
                            }
                        }
                        var kucukHasarOnarim = benefitCoverageList.Where(w => w.coverage.coverageCode == ERGOCoverageList.kucukHasarOnarim).FirstOrDefault();
                        if (kucukHasarOnarim != null)
                        {
                            suminsured = kucukHasarOnarim.suminsured;
                            premium = kucukHasarOnarim.premium;
                            this.AddTeminat(KaskoTeminatlar.Mini_Onrarim_Hizmeti, suminsured, 0, premium, 0, 0);
                        }
                        var hukuksalKorumaSurucu = benefitCoverageList.Where(w => w.coverage.coverageCode == ERGOCoverageList.hukuksalKoruma && w.benefit.benefitCode == ERGOKaskoBenefitList.hukuksalKorumaSurucu).FirstOrDefault();
                        if (hukuksalKorumaSurucu != null)
                        {
                            suminsured = hukuksalKorumaSurucu.suminsured;
                            premium = hukuksalKorumaSurucu.premium;
                            this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, suminsured, 0, premium, 0, 0);
                        }
                        var hukuksalKorumaArac = benefitCoverageList.Where(w => w.coverage.coverageCode == ERGOCoverageList.hukuksalKoruma && w.benefit.benefitCode == ERGOKaskoBenefitList.hukuksalKorumaArac).FirstOrDefault();
                        if (hukuksalKorumaArac != null)
                        {
                            suminsured = hukuksalKorumaArac.suminsured;
                            premium = hukuksalKorumaArac.premium;
                            this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, suminsured, 0, premium, 0, 0);
                        }
                        var ozelEsya = benefitCoverageList.Where(w => w.coverage.coverageCode == ERGOKaskoBenefitList.ozelEsya).FirstOrDefault();
                        if (ozelEsya != null)
                        {
                            suminsured = ozelEsya.suminsured;
                            premium = ozelEsya.premium;
                            this.AddTeminat(KaskoTeminatlar.Ozel_Esya, suminsured, 0, premium, 0, 0);
                        }
                        var anahtar = benefitCoverageList.Where(w => w.coverage.coverageCode == ERGOKaskoBenefitList.anahtar).FirstOrDefault();
                        if (anahtar != null)
                        {
                            suminsured = anahtar.suminsured;
                            premium = anahtar.premium;
                            this.AddTeminat(KaskoTeminatlar.Anahtar_Kaybi, suminsured, 0, premium, 0, 0);
                        }

                        var olumsurekliSakatlik = benefitCoverageList.Where(w => w.coverage.coverageId == ERGOKaskoCoverageList.olumsurekliSakatlik && w.benefit.benefitId == 2017).FirstOrDefault();
                        if (olumsurekliSakatlik != null)
                        {
                            suminsured = olumsurekliSakatlik.suminsured;
                            premium = olumsurekliSakatlik.premium;
                            this.AddTeminat(KaskoTeminatlar.Yolcu_Olum_Surekli_Sakatlik, suminsured, 0, premium, 0, 0);
                        }
                        var surucuOlumsurekliSakatlik = benefitCoverageList.Where(w => w.coverage.coverageId == ERGOKaskoCoverageList.olumsurekliSakatlik && w.benefit.benefitId == 2018).FirstOrDefault();
                        if (surucuOlumsurekliSakatlik != null)
                        {
                            suminsured = surucuOlumsurekliSakatlik.suminsured;
                            premium = surucuOlumsurekliSakatlik.premium;
                            this.AddTeminat(KaskoTeminatlar.Surucu_Olum_Surekli_Sakatlik, suminsured, 0, premium, 0, 0);
                            this.AddTeminat(KaskoTeminatlar.KFK_Olum, suminsured, 0, premium, 0, 0);
                            this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, suminsured, 0, premium, 0, 0);
                        }
                        var surucuTedavi = benefitCoverageList.Where(w => w.coverage.coverageId == ERGOKaskoCoverageList.tedaviMasraflari && w.benefit.benefitId == 2020).FirstOrDefault();
                        if (surucuTedavi != null)
                        {
                            suminsured = surucuTedavi.suminsured;
                            premium = surucuTedavi.premium;
                            this.AddTeminat(KaskoTeminatlar.Surucu_Tedavi, suminsured, 0, premium, 0, 0);
                            this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, suminsured, 0, premium, 0, 0);
                        }
                        var yolcuTedavi = benefitCoverageList.Where(w => w.coverage.coverageId == ERGOKaskoCoverageList.tedaviMasraflari && w.benefit.benefitId == 2019).FirstOrDefault();
                        if (yolcuTedavi != null)
                        {
                            suminsured = yolcuTedavi.suminsured;
                            premium = yolcuTedavi.premium;
                            this.AddTeminat(KaskoTeminatlar.Yolcu_Tedavi, suminsured, 0, premium, 0, 0);
                        }
                        var tefriksiz = benefitCoverageList.Where(w => w.coverage.coverageId == ERGOKaskoCoverageList.tefriksiz).FirstOrDefault();
                        if (tefriksiz != null)
                        {
                            suminsured = tefriksiz.suminsured;
                            premium = tefriksiz.premium;
                            this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, suminsured, 0, premium, 0, 0);
                        }
                        var ergoIkame = benefitCoverageList.Where(w => w.coverage.coverageId == ERGOKaskoCoverageList.asistansIkameAracYilda15Gun2Kez ||
                                                                    w.coverage.coverageId == ERGOKaskoCoverageList.asistansIkameAracYilda7Gun3Kez).FirstOrDefault();
                        if (ergoIkame != null)
                        {
                            suminsured = ergoIkame.suminsured;
                            premium = ergoIkame.premium;
                            this.AddTeminat(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, suminsured, 0, premium, 0, 0);
                        }

                    }

                    var pDiscountSurchargeList = item.pDiscountSurchargeList;

                    if (pDiscountSurchargeList != null)
                    {
                        foreach (var itemDis in pDiscountSurchargeList)
                        {
                            if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.tumAnlasmaliServisler)
                            {
                                this.AddTeminat(KaskoTeminatlar.ServisTuru, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.sadeceAnlasmaliOzelServisler)
                            {
                                this.AddTeminat(KaskoTeminatlar.ServisTuru, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.tumServisler)
                            {
                                this.AddTeminat(KaskoTeminatlar.ServisTuru, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.hasarsizlikKoruma)
                            {
                                this.AddTeminat(KaskoTeminatlar.HasarsizlikKoruma, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.patlayiciParlayiciMadde)
                            {
                                this.AddTeminat(KaskoTeminatlar.Arac_Yanmasi, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.yurtDisi)
                            {
                                this.AddTeminat(KaskoTeminatlar.YurtDisi, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.sigaraYanigi)
                            {
                                this.AddTeminat(KaskoTeminatlar.Sigara_Ve_Benzeri_Madde_Zararlari, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.anahtarlaCalinma)
                            {
                                this.AddTeminat(KaskoTeminatlar.Arac_Calinmasi, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.kiymetKazanma2)
                            {
                                this.AddTeminat(KaskoTeminatlar.KiymetKazanmaTenzili, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.seylapMuafiyetsiz)
                            {
                                this.AddTeminat(KaskoTeminatlar.Seylap, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.yetkiliOlmayanCekme)
                            {
                                this.AddTeminat(KaskoTeminatlar.YetkisizCekilme, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.meslekIndirimi)
                            {
                                this.AddTeminat(KaskoTeminatlar.MeslekIndirimi, 0, 0, 0, 0, 0);
                            }
                            else if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoDiscountList.pesinOdeme)
                            {
                                this.AddTeminat(KaskoTeminatlar.ErgoPesinodeme, 0, 0, 0, 0, 0);
                            }
                            if (itemDis.isRider && itemDis.discountSurcharge.disSurCode == ERGOKaskoiscountSurchargeList.hasarsizlik)
                            {
                                var hasarOrani = itemDis.rate * 100;
                                if (hasarOrani <= 60)
                                {
                                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = hasarOrani;
                                }
                                else if (hasarOrani > 60)
                                {
                                    this.GenelBilgiler.HasarSurprimYuzdesi = hasarOrani;
                                }
                                else
                                {
                                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                                    this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                                }
                            }
                        }

                    }
                    #endregion

                    this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                    this.GenelBilgiler.TaksitSayisi = Convert.ToByte(item.paymentPlan.installmentCount);
                    this.GenelBilgiler.ZKYTMSYüzdesi = 0;

                    ////// ==== Güncellenicek. ==== //
                    if (this.GenelBilgiler.TaksitSayisi == 1)
                    {
                        this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    }
                    else
                    {
                        this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Vadeli;
                    }
                    ////Odeme Bilgileri
                    this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;

                    #region Ödeme Planı
                    if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                    {
                        this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                    }
                    #endregion
                }

                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                //clnt.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            CascoService client = new CascoService();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleERGOService);
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ERGO });


                client.Url = konfig[Konfig.ERGO_KaskoServiceURL];
                client.Timeout = 150000;
                ////WEB Service e username token ekleniyor
                UsernameToken tokenKasko = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
                client.RequestSoapContext.Security.Tokens.Add(tokenKasko);
                client.RequestSoapContext.Security.MustUnderstand = false;
                client.RequestSoapContext.Security.Actor = "";
                int proposalNo = Convert.ToInt32(teklif.GenelBilgiler.TUMTeklifNo);

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    creditCardWsDto krediKart = new creditCardWsDto()
                    {
                        cardHolder = odeme.KrediKarti.KartSahibi,
                        creditCardNumber = odeme.KrediKarti.KartNo,
                        cvvNumber = odeme.KrediKarti.CVC,
                        expireMonth = odeme.KrediKarti.SKA,
                        expireYear = odeme.KrediKarti.SKY
                    };
                    this.BeginLog(krediKart, krediKart.GetType(), WebServisIstekTipleri.Police);
                }
                creditCardWsDto krediKarti = new creditCardWsDto();
                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    krediKarti = new creditCardWsDto()
                    {
                        cardHolder = odeme.KrediKarti.KartSahibi,
                        creditCardNumber = odeme.KrediKarti.KartNo,
                        cvvNumber = odeme.KrediKarti.CVC,
                        expireMonth = odeme.KrediKarti.SKA,
                        expireYear = odeme.KrediKarti.SKY
                    };
                }

                policyWsDto response = client.saveCascoPolicy(proposalNo, odeme.TaksitSayisi, krediKarti);
                client.Dispose(); this.EndLog(response, true, response.GetType());

                if (response.premium.totalPremium > 0)
                {
                    this.GenelBilgiler.TUMPoliceNo = response.policyNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    endorsementKeyWsDto printRequest = new endorsementKeyWsDto()
                    {
                        endorsementNo = response.endorsementNo,
                        policyNo = response.policyNo,
                        renewalNo = response.renewalNo
                    };

                    this.BeginLog(printRequest, printRequest.GetType(), WebServisIstekTipleri.Police);

                    var responsePDF = client.printPolicy(printRequest);
                    client.Dispose();
                    this.EndLog(responsePDF, true, responsePDF.GetType());
                    if (responsePDF != null)
                    {
                        WebClient myClient = new WebClient();
                        foreach (var item in responsePDF)
                        {
                            byte[] data = myClient.DownloadData(item.bytes.ToString());

                            IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                            string fileName = String.Format("ERGO_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                            string url = storage.UploadFile("KASKO", fileName, data);
                            this.GenelBilgiler.PDFPolice = url;
                            _Log.Info("Police_PDF url: {0}", url);
                        }
                    }
                }

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
        //public override void Policelestir(Odeme odeme)
        //{
        //    ExternalPolicyService clnt = new ExternalPolicyService();
        //    try
        //    {
        //        #region Veri Hazırlama GENEL
        //        ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
        //        KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);
        //        var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
        //        TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ERGO });
        //        MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
        //        #endregion

        //        #region Policelestirme Request

        //        clnt.Url = konfig[Konfig.ERGO_ServiceURL];
        //        clnt.Timeout = 150000;
        //        clnt.Credentials = new System.Net.NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre);
        //        //WEB Service e username token ekleniyor
        //        UsernameToken token = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
        //        clnt.RequestSoapContext.Security.Tokens.Add(token);
        //        clnt.RequestSoapContext.Security.MustUnderstand = false;
        //        clnt.RequestSoapContext.Security.Actor = "";

        //        int proposalNo = Convert.ToInt32(teklif.GenelBilgiler.TUMTeklifNo);

        //        if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
        //        {
        //            creditCardWsDto krediKart = new creditCardWsDto()
        //            {
        //                cardHolder = odeme.KrediKarti.KartSahibi,
        //                creditCardNumber = odeme.KrediKarti.KartNo,
        //                cvvNumber = odeme.KrediKarti.CVC,
        //                expireMonth = odeme.KrediKarti.SKA,
        //                expireYear = odeme.KrediKarti.SKY
        //            };
        //            this.BeginLog(krediKart, krediKart.GetType(), WebServisIstekTipleri.Police);
        //        }

        //        creditCardWsDto krediKarti = new creditCardWsDto();
        //        if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
        //        {
        //            krediKarti = new creditCardWsDto()
        //            {
        //                cardHolder = odeme.KrediKarti.KartSahibi,
        //                creditCardNumber = odeme.KrediKarti.KartNo,
        //                cvvNumber = odeme.KrediKarti.CVC,
        //                expireMonth = odeme.KrediKarti.SKA,
        //                expireYear = odeme.KrediKarti.SKY
        //            };
        //        }

        //        policyWsDto response = clnt.savePolicy(proposalNo, krediKarti);
        //        clnt.Dispose();

        //        this.EndLog(response, true, response.GetType());

        //        if (response.premium.totalPremium > 0)
        //        {
        //            this.GenelBilgiler.TUMPoliceNo = response.policyNo.ToString();
        //            this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

        //            endorsementKeyWsDto printRequest = new endorsementKeyWsDto()
        //            {
        //                endorsementNo = response.endorsementNo,
        //                policyNo = response.policyNo,
        //                renewalNo = response.renewalNo
        //            };

        //            this.BeginLog(printRequest, printRequest.GetType(), WebServisIstekTipleri.Police);

        //            var responsePDF = clnt.printPolicy(printRequest);
        //            clnt.Dispose();
        //            this.EndLog(responsePDF, true, responsePDF.GetType());
        //            if (responsePDF != null)
        //            {
        //                WebClient myClient = new WebClient();
        //                foreach (var item in responsePDF)
        //                {
        //                    byte[] data = myClient.DownloadData(item.bytes.ToString());

        //                    IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
        //                    string fileName = String.Format("ERGO_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString());
        //                    string url = storage.UploadFile("KASKO", fileName, data);
        //                    this.GenelBilgiler.PDFPolice = url;
        //                    _Log.Info("Police_PDF url: {0}", url);
        //                }
        //            }
        //        }

        //        this.GenelBilgiler.WEBServisLogs = this.Log;
        //        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Hata Log
        //        clnt.Abort();
        //        this.EndLog(ex.Message, false);
        //        this.AddHata(ex.Message);

        //        this.GenelBilgiler.WEBServisLogs = this.Log;
        //        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
        //        #endregion
        //    }
        //}
    }
}
