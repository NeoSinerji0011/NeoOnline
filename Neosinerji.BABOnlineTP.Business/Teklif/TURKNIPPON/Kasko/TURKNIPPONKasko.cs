using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.turknipponkasko;
using Neosinerji.BABOnlineTP.Business.Common.TURKNIPPON;
using System.Xml.Serialization;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON;
using System.IO;
using System.Net;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.Kasko
{
    public class TURKNIPPONKasko : Teklif, ITURKNIPPONKasko
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
        public TURKNIPPONKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
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
                return TeklifUretimMerkezleri.TURKNIPPON;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            turknipponkasko.MODService clnt = new turknipponkasko.MODService();
            try
            {
                #region Veri Hazırlama

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONKasko);

                clnt.Url = konfig[Konfig.TURKNIPPON_KaskoServiceURL];
                clnt.Timeout = 150000;

                CreditCardInput kredikarti = new CreditCardInput();

                TURKNIPPON_Proposal_Response proposalresult = new TURKNIPPON_Proposal_Response();
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon cepTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                MusteriTelefon evTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);

                KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                string testMi = konfigTestMi[Konfig.TestMi];
                #endregion

                #region Genel Bilgiler
                turknipponkasko.MODInput modInput = new MODInput();
                modInput.IsTestMode = false;
                if (Convert.ToBoolean(testMi))
                {
                    modInput.IsTestMode = true; // Zorunlu
                }
                modInput.Channel = Convert.ToInt32(servisKullanici.PartajNo_);
                modInput.Username = servisKullanici.KullaniciAdi;
                modInput.TrackingCode = ""; // boş olabilir işlem takip kodu

                #endregion

                #region Sigortalı Bilgileri

                modInput.UnitNo = 0;  //CitizenshipNumber veya TaxNumber yoksa zorunlu  sigortalı müşteri no
                modInput.ClientNo = 0; //sigorta ettiren müşteri no

                if (sigortali.KimlikNo.Length == 11)
                {
                    modInput.CitizenshipNumber = sigortali.KimlikNo;
                    modInput.ClientCitizenshipNumber = sigortali.KimlikNo;
                }
                else
                {
                    modInput.TaxNumber = sigortali.KimlikNo; //sigortalı vergi no
                    modInput.ClientTaxNumber = sigortali.KimlikNo;
                }
                if (evTel != null && evTel.Numara.Length > 10)
                {
                    modInput.PhoneNumber = evTel.Numara.Substring(3, 3) + evTel.Numara.Substring(7, 7);//sigortalı tel no //gönderilmesi tavsiye edilir
                    modInput.ClientPhoneNumber = evTel.Numara.Substring(3, 3) + evTel.Numara.Substring(7, 7);//gönderilmesi tavsiye edilir
                }
                if (cepTel != null && cepTel.Numara.Length > 10)
                {
                    modInput.PhoneNumber = cepTel.Numara.Substring(3, 3) + cepTel.Numara.Substring(7, 7); //sigortalı tel no //gönderilmesi tavsiye edilir
                    modInput.ClientPhoneNumber = cepTel.Numara.Substring(3, 3) + cepTel.Numara.Substring(7, 7);//gönderilmesi tavsiye edilir
                }

                #endregion

                #region Credit Card
                modInput.UseCreditCard = false; //Kredi kartı kullanılacak mı? dikkate alınmaz
                #endregion

                #region Arac Bilgileri ve Teminatlar

                modInput = this.TeklifRequest(teklif, servisKullanici, clnt, modInput);
                var yetkiliIndirimi = teklif.ReadSoru(KaskoSorular.NipponYetkiliInidirimi, 0);
                modInput.AuthorizedDiscount = Convert.ToInt32(yetkiliIndirimi);
                #endregion

                #region Service call

                this.BeginLog(modInput, modInput.GetType(), WebServisIstekTipleri.Teklif);

                MODOutput teklifresult = clnt.Proposal(modInput);
                clnt.Dispose();
                #endregion

                #region Varsa Hata Kaydı

                if (!teklifresult.IsSuccess)
                {
                    this.EndLog(teklifresult, false, teklifresult.GetType());
                    this.AddHata(teklifresult.StatusDescription);
                }
                else
                {
                    this.EndLog(teklifresult, true, teklifresult.GetType());
                    XmlSerializer _Serialize = new XmlSerializer(typeof(TURKNIPPON_Proposal_Response));
                    using (TextReader reader = new StringReader(teklifresult.ExtraXmlString))
                    {

                        proposalresult = (TURKNIPPON_Proposal_Response)_Serialize.Deserialize(reader);
                        reader.ReadToEnd();
                        this.EndLog(proposalresult, true, proposalresult.GetType());

                    }
                }
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
                this.GenelBilgiler.BaslamaTarihi = teklifresult.BeginDate;
                this.GenelBilgiler.BitisTarihi = teklifresult.EndDate;
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = teklifresult.Premium;
                decimal GiderVergisi = 0;
                this.GenelBilgiler.ToplamVergi = GiderVergisi;
                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                if (proposalresult.POLDID != null)
                {

                    if (proposalresult != null) this.GenelBilgiler.ToplamKomisyon = TURKNIPPONMessage.ToDecimal(proposalresult.POLDID.DEDUCTION_AMOUNT);
                    else this.GenelBilgiler.ToplamKomisyon = 0;

                }
                string policeno = teklifresult.PolicyNo.ToString();
                this.GenelBilgiler.TUMTeklifNo = policeno;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                #endregion

                #region Teminatlar

                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                string kullanimTarziKodu = String.Empty;
                string kod2 = String.Empty;
                if (parts.Length == 2)
                {
                    kullanimTarziKodu = parts[0];
                    kod2 = parts[1];
                }

                // ==== 420	 A	 KASKO KASKO   	 ARÇ ==== //
                var kasko = teklifresult.VehicleMarketValue;
                if (kasko != 0)
                    this.AddTeminat(KaskoTeminatlar.Kasko, kasko, 0, 0, teklifresult.Premium, 0);

                // ====420	 A	 HUKUKSAL KORUMA	 HUK.KOR ==== //
                var hukuksalkoruma = teklifresult.Input;
                int nipponHukuksalKorumaBedel = 0;

                if (hukuksalkoruma != null)
                {
                    switch (hukuksalkoruma.LegalProtection)
                    {
                        case 0: nipponHukuksalKorumaBedel = TurkNippon_KaskoHukuksalKorumaLimitleri.Yok; break;
                        case 1: nipponHukuksalKorumaBedel = TurkNippon_KaskoHukuksalKorumaLimitleri.BesBin; break;
                        case 2: nipponHukuksalKorumaBedel = TurkNippon_KaskoHukuksalKorumaLimitleri.OnBin; break;
                        case 3: nipponHukuksalKorumaBedel = TurkNippon_KaskoHukuksalKorumaLimitleri.OnBesBin; break;
                        case 4: nipponHukuksalKorumaBedel = TurkNippon_KaskoHukuksalKorumaLimitleri.YirmiBin; break;
                        case 5: nipponHukuksalKorumaBedel = TurkNippon_KaskoHukuksalKorumaLimitleri.YirmiBesBin; break;
                        case 6: nipponHukuksalKorumaBedel = TurkNippon_KaskoHukuksalKorumaLimitleri.OtuzBin; break;
                        default: nipponHukuksalKorumaBedel = 5000; break;
                    }

                    this.AddTeminat(KaskoTeminatlar.Hukuksal_Koruma, nipponHukuksalKorumaBedel, 0, 0, 0, 0);
                }


                var nipponInput = teklifresult.Input;
                string nipponimmkademe = teklifresult.Input.DiscretionaryIndemnity.HasValue ? teklifresult.Input.DiscretionaryIndemnity.Value.ToString() : "";
                string nipponImmKombineKademe = teklifresult.Input.DiscretionaryIndemnityFixed.HasValue ? (teklifresult.Input.DiscretionaryIndemnityFixed.Value * 100).ToString() : "";
                if (nipponInput != null)
                {
                    if (!String.IsNullOrEmpty(nipponimmkademe))
                    {
                        CR_KaskoIMM kaskoIMM = _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.TURKNIPPON &&
                                                                              s.Kademe == nipponimmkademe && s.Kod2 == kod2 &&
                                                                              s.KullanimTarziKodu == kullanimTarziKodu).FirstOrDefault();
                        if (kaskoIMM != null)
                        {// ==== 420	 A	    KAZA BAŞINA 	 AMS-KB 	 ARÇ ==== //
                            this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, kaskoIMM.BedeniKaza.Value, 0, 0, 0, 0);

                            // ==== 420	 A	    ŞAHIS BAŞINA	 AMS-ŞB 	 ARÇ ==== //
                            this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, kaskoIMM.BedeniSahis.Value, 0, 0, 0, 0);

                            // ==== 420	 A	    MADDİ HASAR 	 AMS-MH 	 ARÇ ==== //
                            this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, kaskoIMM.Maddi.Value, 0, 0, 0, 0);
                        }
                    }
                    else if (!String.IsNullOrEmpty(nipponImmKombineKademe))
                    {
                        CR_KaskoIMM kaskoIMM = new CR_KaskoIMM();
                        kaskoIMM = _CRContext.CR_KaskoIMMRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.TURKNIPPON &&
                                                                         s.Kademe == nipponImmKombineKademe && s.Kod2 == "10" &&
                                                                         s.KullanimTarziKodu == "111").FirstOrDefault();
                        if (kaskoIMM != null)
                        {
                            this.AddTeminat(KaskoTeminatlar.Ihtiyari_Mali_Mesuliyet_Kombine_Bedeni_Maddi, kaskoIMM.Kombine.Value, 0, 0, 0, 0);
                        }


                    }
                    string nipponfkkademe = teklifresult.Input.PersonalAccident.ToString();
                    var kaskoFK = _CRContext.CR_KaskoFKRepository.Filter(s => s.TUMKodu == TeklifUretimMerkezleri.TURKNIPPON &&
                                                                  s.Kademe == nipponfkkademe && s.Kod2 == "10" &&
                                                                  s.KullanimTarziKodu == "111").FirstOrDefault();
                    if (kaskoFK != null)
                    {
                        // ==== 420	 E	 ÖLÜM    	 ŞK-ÖLÜM	 ARÇ ==== //
                        this.AddTeminat(KaskoTeminatlar.KFK_Olum, kaskoFK.Vefat.Value, 0, 0, 0, 0);

                        // ==== 420	 E	 SAKATLIK	 ŞK-SS  	 ARÇ ==== //
                        this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, kaskoFK.Sakatlik.Value, 0, 0, 0, 0);

                        // ==== 420	 E	  TEDAVİ  	 ŞK-TED 	 ARÇ ==== //
                        this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, kaskoFK.Tedavi.Value, 0, 0, 0, 0);
                    }

                }

                #endregion

                #region Web servis cevapları

                string musterino = teklifresult.UnitNo.ToString();
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, policeno);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, musterino);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, teklifresult.TrackingCode);

                int[] basimTipi = new int[3];
                basimTipi[0] = TurkNippon_BaskiTipleri.TeklifPolice;
                this.PDfGetir(servisKullanici, clnt, teklifresult.TrackingCode, policeno, musterino, basimTipi, true);
                #endregion

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                clnt.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            turknipponkasko.MODService clnt = new turknipponkasko.MODService();
            try
            {
                #region Veri Hazırlama

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONKasko);

                clnt.Url = konfig[Konfig.TURKNIPPON_KaskoServiceURL];
                clnt.Timeout = 150000;
                TURKNIPPON_Proposal_Response result = new TURKNIPPON_Proposal_Response();
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon Telefon = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                MODInput request = new MODInput();
                MODOutput response = new MODOutput();

                KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                string testMi = konfigTestMi[Konfig.TestMi];
                #endregion

                #region Genel Bilgiler

                string IslemTakipKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, "0");
                string SigortaliMusteriNo = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, "0");
                string PoliceNumarasi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, "0");

                string adi = String.Empty;
                string soyadi = String.Empty;

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    string[] parts = odeme.KrediKarti.KartSahibi.Split(' ');
                    if (parts.Length == 2)
                    {
                        adi = parts[0];
                        soyadi = parts[1];

                    }
                    else if (parts.Length == 3)
                    {
                        adi = parts[0] + " " + parts[1];
                        soyadi = parts[2];
                    }
                }
                request.IsTestMode = false;
                if (Convert.ToBoolean(testMi))
                {
                    request.IsTestMode = true; // Zorunlu
                }
                request.Channel = Convert.ToInt32(servisKullanici.PartajNo_);
                request.Username = servisKullanici.KullaniciAdi;
                request.TrackingCode = IslemTakipKodu;
                request.CitizenshipNumber = "";
                request.TaxNumber = "";
                request.UnitNo = Convert.ToInt64(SigortaliMusteriNo);
                request.PolicyNo = Convert.ToInt64(PoliceNumarasi);

                #endregion

                #region Sigortali Bilgileri

                request.ClientNo = Convert.ToInt64(SigortaliMusteriNo);
                request.ClientCitizenshipNumber = "";
                request.ClientTaxNumber = "";

                if (Telefon != null)
                {
                    if (Telefon.Numara.Length > 10)
                    {
                        request.PhoneNumber = Telefon.Numara.Substring(3, 3) + Telefon.Numara.Substring(7, 7);
                        request.ClientPhoneNumber = Telefon.Numara.Substring(3, 3) + Telefon.Numara.Substring(7, 7);
                    }
                }

                #endregion

                #region Arac Bilgileri

                request = this.TeklifRequest(teklif, servisKullanici, clnt, request);

                #endregion

                #region Ödeme Biglileri
                request.CreditCard = new CreditCardInput();
                request.UseCreditCard = false;
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    request.UseCreditCard = true;
                    request.CreditCard.CardHolderFirstname = adi;
                    request.CreditCard.CardHolderLastname = soyadi;
                    request.CreditCard.CardNumber = odeme.KrediKarti.KartNo.Substring(0, 6);
                    request.CreditCard.CVV = "XXX";
                    request.CreditCard.Month = "99";
                    request.CreditCard.Year = "9999";
                    request.CreditCard.Installment = (int)odeme.TaksitSayisi;
                }

                #endregion

                #region Service Call

                this.BeginLog(request, typeof(MODInput), WebServisIstekTipleri.Police);

                //KrediKarti bilgileri loglanmıyor..
                #region KrediKartı Bilgileri
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    request.UseCreditCard = true;
                    if (odeme.KrediKarti.KartNo.Substring(0, 1) == "5")
                        request.CreditCard.CardType = 2;
                    else request.CreditCard.CardType = 1;

                    request.CreditCard.CardNumber = odeme.KrediKarti.KartNo;
                    request.CreditCard.Month = odeme.KrediKarti.SKA;
                    request.CreditCard.Year = odeme.KrediKarti.SKY;
                    request.CreditCard.CVV = odeme.KrediKarti.CVC;
                    request.CreditCard.CardHolderFirstname = adi;
                    request.CreditCard.CardHolderLastname = soyadi;
                    request.CreditCard.Installment = odeme.TaksitSayisi;

                }
                else
                {
                    request.CreditCard.CardNumber = "";
                    request.CreditCard.Month = "";
                    request.CreditCard.Year = "";
                    request.CreditCard.CVV = "";
                    request.CreditCard.Installment = odeme.TaksitSayisi;
                    request.CreditCard.CardHolderFirstname = "";
                    request.CreditCard.CardHolderLastname = "";
                    request.CreditCard.CardType = 0;
                }
                #endregion

                response = clnt.Approve(request);
                clnt.Dispose();
                #endregion

                #region Hata Kontrol ve Kayıt

                if (!response.IsSuccess)
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.StatusDescription);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());

                    this.GenelBilgiler.TUMPoliceNo = response.PolicyNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                    this.GenelBilgiler.OdemeSekli = odeme.OdemeSekli;
                    this.GenelBilgiler.OdemeTipi = odeme.OdemeTipi;
                    this.GenelBilgiler.TaksitSayisi = odeme.TaksitSayisi;

                    int[] basimTipi = new int[3];
                    basimTipi[0] = TurkNippon_BaskiTipleri.TeklifPolice;
                    basimTipi[1] = TurkNippon_BaskiTipleri.BilgilendirmeFormu;
                    this.PDfGetir(servisKullanici, clnt, IslemTakipKodu, PoliceNumarasi, SigortaliMusteriNo, basimTipi, false);

                }
                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
            catch (Exception ex)
            {
                _Log.Error("TurkNipponKasko.Policelestir", ex);
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        public MODInput TeklifRequest(ITeklif teklif, TVMWebServisKullanicilari servisKullanici, MODService clnt, MODInput modInput)
        {
            CoverageModel teminatlar = new CoverageModel();
            List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR ||
                                                                                      w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                                                          .ToList<TeklifAracEkSoru>();
            #region Araç Bilgileri
            modInput.CashPaymentType = 0; //Nakit Ödeme Planı //Dikkate alınmaz

            bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
            if (!eskiPoliceVar)
            {
                modInput.IsRenewal = false; // Yenileme mi //Print işlemi haricinde zorunludur
            }
            else
            {
                modInput.IsRenewal = true;
            }
            modInput.PlateCityCode = Convert.ToInt32(teklif.Arac.PlakaKodu); //Plaka il kodu
            //if (teklif.Arac.PlakaNo == "YK") // G plaka ile teklif alınamadı değiştirilecek
            //    modInput.PlateNumber = "G 9998";
            //else modInput.PlateNumber = teklif.Arac.PlakaNo;
            modInput.PlateNumber = teklif.Arac.PlakaNo;
            DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            modInput.BeginDate = policeBaslangic; //Print işlemi haricinde zorunludur
            modInput.ModelYear = teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value : DateTime.Now.Year; //Araç Model Yılı  IsRenewal false ise zorunlu

            // ====GetBrandList metodundan verileri alabilmek için header da gönderilen parametreler gönderiliyor   
            AuthHeader header = new AuthHeader();
            header.Channel = servisKullanici.PartajNo_;
            header.Username = servisKullanici.KullaniciAdi;
            header.Password = servisKullanici.Sifre;

            clnt.AuthHeaderValue = header;
            // ==== //

            var NipponMarkaList = clnt.GetBrandList(modInput.ModelYear,"");
            clnt.Dispose();
            string nipponMarka = String.Empty;
            foreach (var item in NipponMarkaList)
            {
                if (item.Code.Trim() == teklif.Arac.Marka.Trim())
                {
                    nipponMarka = item.Code;
                }
            }

            if (teklif.Arac.AracinTipi.Length == 2)
            {
                teklif.Arac.AracinTipi = "0" + teklif.Arac.AracinTipi;
            }

            string aracmarkatip = teklif.Arac.Marka + teklif.Arac.AracinTipi;
            var nipponAracTipleri = clnt.GetModelList(modInput.ModelYear, nipponMarka,"").ToList();
            string nipponAracTipi = "";
            if (nipponAracTipleri != null)
            {
                nipponAracTipi = nipponAracTipleri.FirstOrDefault(s => s.Code.Trim() == aracmarkatip).Code.Trim();
            }
            clnt.Dispose();
            modInput.VehicleModel = nipponAracTipi; // Araç Model Kodu IsRenewal false ise zorunlu

            modInput.EngineNo = teklif.Arac.MotorNo; //Motor No IsRenewal false ise zorunlu
            modInput.ChassisNo = teklif.Arac.SasiNo; //Şasi No IsRenewal false ise zorunlu

            modInput.RegistrationDate = teklif.Arac.TrafikTescilTarihi.Value;//Aracın son tescil no
            if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo) || !String.IsNullOrEmpty(teklif.Arac.TescilSeriKod))
            {
                modInput.TescilASBIS = !String.IsNullOrEmpty(teklif.Arac.AsbisNo) ? teklif.Arac.AsbisNo : teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo;
            }
            else
            {
                modInput.TescilASBIS = "";
            }
            //VehicleType set ediliyor..
            switch (teklif.Arac.KullanimTarzi)
            {
                case "111-10": modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Binek; break;
                case "121-10": modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Binek; break;
                case "111-13": modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Kamyonet; break;
                case "511-10": modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Kamyonet; break;
                case "511-11": modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Kamyonet; break;
                case "511-12": modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Kamyonet; break;
                case "515-15": modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Kamyonet; break;
                default: modInput.VehicleType = TurkNippon_KaskoKullanimTarzi.Binek; break; //Araç Tipi
            }

            string[] parts = teklif.Arac.KullanimTarzi.Split('-');
            string kullanimTarziKodu = String.Empty;
            string kod2 = String.Empty;
            if (parts.Length == 2)
            {
                kullanimTarziKodu = parts[0];
                kod2 = parts[1];
                CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.TURKNIPPON &&
                                                                                              f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                              f.Kod2 == kod2)
                                                                                              .SingleOrDefault<CR_KullanimTarzi>();
                if (kullanimTarzi != null)
                {
                    modInput.TariffClass = Convert.ToInt32(kullanimTarzi.TarifeKodu); //Tarife sınıfı
                }
                else modInput.TariffClass = 1;
            }
            if (teklif.Arac.KullanimSekli == "0")
            {
                modInput.UsageType = TurkNippon_KaskoKullanimSekilleri.Ticari;
            }
            else if (teklif.Arac.KullanimSekli == "1")
            {
                modInput.UsageType = TurkNippon_KaskoKullanimSekilleri.Resmi;
            }
            else
            {
                modInput.UsageType = TurkNippon_KaskoKullanimSekilleri.Ozel;
            }

            modInput.PassengerCount = teklif.Arac.KoltukSayisi.Value - 1; //Yolcu sayısı (sürücü hariç)
            modInput.EmployeeCount = 0; // Görevli sayısı

            string yakitTipi = teklif.ReadSoru(KaskoSorular.LPG_VarYok, String.Empty);
            if (String.IsNullOrEmpty(yakitTipi))
                modInput.FuelType = TurkNippon_KaskoAracYakitTipleri.BenzinLPG; // Yakıt tipi
            else modInput.FuelType = TurkNippon_KaskoAracYakitTipleri.Benzin;

            modInput.VehicleColor = TurkNippon_KaskoAracRenkTipleri.Beyaz; //araç ana rengi
            modInput.LossPayeeType = TurkNippon_KaskoDainiMurteinDurumlari.Yok; //daini murtain durumu
            modInput.Bank = ""; //daini murtain banka   LossPayeeType b ise zorunlu
            modInput.BankBranch = "";// daini murtain banka şube LossPayeeType b ise zorunlu
            modInput.FinancialInstitution = "";//daini murtain finansal kurumu LossPayeeType f ise zorunlu

            string servisTuru = teklif.ReadSoru(KaskoSorular.TurkNipponServisTuru, "");
            if (servisTuru != "")
            {
                modInput.RepairServiceType = Convert.ToInt32(servisTuru); // onarım servis tipi
                modInput.SparePartType = TurkNippon_KaskoYedekParcaTipleri.orjinal;
            }
            
            modInput.GarageType = TurkNippon_KaskoAracBagajTipleri.Acik;//Garaj tipi

            #endregion

            #region Teminatlar

            bool alarm = teklif.ReadSoru(KaskoSorular.Alarm_VarYok, false);
            if (alarm) modInput.AudibleAlarm = true;//alarm var mı?
            else modInput.AudibleAlarm = false;

            modInput.Immobilizer = false; //immobilizer var mı

            // ====IMM Teminatı   ==== //
            string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
            int TurkNipponIMMKademe = 1;
            int TurkNipponKombineKademe = 1;

            if (!String.IsNullOrEmpty(immKademe) && !String.IsNullOrEmpty(kullanimTarziKodu))
            {
                CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();
                //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                if (IMMBedel != null)
                {
                    //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                    CRKademeNo = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.TURKNIPPON, IMMBedel.BedeniSahis, IMMBedel.Kombine, "111", "10");
                }
                if (CRKademeNo != null)
                {
                    if (IMMBedel.BedeniSahis == 0)
                    {
                        TurkNipponKombineKademe = Convert.ToInt32(CRKademeNo.Kademe) / 100;// imm kademe kodları ile kombine kademe kodları aynı olduğu için kombine kademe kodları db ye 100 ile çarpılarak eklendi. okurken 100 bölerek okuyoruz.
                        modInput.DiscretionaryIndemnityFixed = TurkNipponKombineKademe;
                    }
                    else
                    {
                        TurkNipponIMMKademe = Convert.ToInt32(CRKademeNo.Kademe);
                        modInput.DiscretionaryIndemnity = TurkNipponIMMKademe; //IMM maddi / bedeni ayrımlı zorunlu
                    }
                }
            }
            else
            {
                modInput.DiscretionaryIndemnityFixed = TurkNipponKombineKademe;
            }

            // ====Ferdi Kaza Teminatı   ==== //
            string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
            int nipponFKKademe = 1;
            int nipponFKTedaviMasrafi = 100;
            if (!String.IsNullOrEmpty(fkKademe) && fkKademe != "0")
            {
                CR_KaskoFK CRKademeNo = new CR_KaskoFK();
                var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                if (FKBedel != null)
                {
                    CRKademeNo = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.TURKNIPPON, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, "111", "10");
                }
                if (CRKademeNo != null)
                {
                    nipponFKKademe = Convert.ToInt32(CRKademeNo.Kademe);
                    nipponFKTedaviMasrafi = Convert.ToInt32(CRKademeNo.Tedavi);
                }
            }
            modInput.PersonalAccident = nipponFKKademe;//Ferdi kaza limit //Print işlemi haricinde zorunludur
            modInput.TreatmentExpenses = nipponFKTedaviMasrafi;//ferdi kaza tedavi masrafları isteğe bağlı belirli limitler arasından

            modInput.LegalProtection = 1;// alternatif huk. koruma isteğe bağlı belirli limitler arasından
            modInput.AuthorizedDiscount = 0; //yetkili indirim oranı isteğe bağlı belirli limitler arasından
            string muafiyetTutari = teklif.ReadSoru(KaskoSorular.TurkNipponMuafiyetTutari, "0");
            modInput.Exemption = Convert.ToInt32(muafiyetTutari);//Muafiyet limiti isteğe bağlı belirli limitler arasından
            modInput.Surcharge = 0;//surprim oranı isteğe bağlı belirli limitler arasından

            #endregion

            #region İsteğe Bağlı Teminatlar

            teminatlar.GPRS = 0;
            teminatlar.Kolon = 0;
            teminatlar.TelevizyonVideo = 0;
            teminatlar.Alarm = 0;
            teminatlar.GSMAracKiti = 0;
            teminatlar.Klima = 0;
            teminatlar.CocukKoltugu = false;

            teminatlar.KisiselEsya = teklif.ReadSoru(KaskoSorular.Teminat_Ozel_Esya_VarYok, true);
            teminatlar.GelirKaybi = teklif.ReadSoru(KaskoSorular.KullanimGelirKaybiVarMi, false);

            decimal? RadyoTeypBedeli = 0;
            if (aksesuarlar.Count > 0)
            {
                var RadyoTeypVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreElektronikCihazTipleri.RT).FirstOrDefault();
                if (RadyoTeypVarMi != null)
                {
                    RadyoTeypBedeli = RadyoTeypVarMi.Bedel;
                }
            }
            teminatlar.RadyoTeypCDCalar = Convert.ToInt32(RadyoTeypBedeli.Value);


            decimal? LPGBedeli = 0;
            if (aksesuarlar.Count > 0)
            {
                var LPGVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreAksesuarTipleri.LPG).FirstOrDefault();
                if (LPGVarMi != null)
                {
                    LPGBedeli = LPGVarMi.Bedel;
                }
            }
            teminatlar.LPGSistemi = Convert.ToInt32(LPGBedeli.Value);

            decimal? JantBedeli = 0;
            if (aksesuarlar.Count > 0)
            {
                var jantVarMi = aksesuarlar.Where(s => s.SoruKodu == MapfreAksesuarTipleri.Jant).FirstOrDefault();
                if (jantVarMi != null)
                {
                    JantBedeli = jantVarMi.Bedel;
                }
            }
            teminatlar.CelikJant = Convert.ToInt32(JantBedeli.Value);



            modInput.Coverage = teminatlar; //isteğe bağlı belirli limitler arasından
            #endregion

            return modInput;
        }

        public void PDfGetir(TVMWebServisKullanicilari servisKullanici, MODService clnt, string islemTakipKodu, string policeNo, string musteriNo, int[] basimtipi, bool teklif)
        {
            KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
            string testMi = konfigTestMi[Konfig.TestMi];
            MODInput request = new MODInput();
            MODOutput response = new MODOutput();

            request.IsTestMode = false;
            if (Convert.ToBoolean(testMi))
            {
                request.IsTestMode = true; // Zorunlu
            }
            request.Channel = Convert.ToInt32(servisKullanici.PartajNo_);
            request.Username = servisKullanici.KullaniciAdi;
            request.TrackingCode = islemTakipKodu;
            request.UnitNo = Convert.ToInt64(musteriNo);
            request.PolicyNo = Convert.ToInt64(policeNo);
            request.ClientNo = Convert.ToInt64(musteriNo);

            foreach (var item in basimtipi)
            {
                if (item > 0)
                {
                    request.PrintType = item;

                    this.BeginLog(request, typeof(MODInput), WebServisIstekTipleri.Police);
                    response = clnt.Print(request);
                    clnt.Dispose();
                    if (!response.IsSuccess)
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.StatusDescription);
                    }
                    else
                    {
                        this.EndLog(response, true, response.GetType());
                        IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();

                        var policePDF = response.PrintDownloadUrl;
                        if (policePDF != null)
                        {
                            WebClient myClient = new WebClient();
                            byte[] data = myClient.DownloadData(policePDF);

                            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                            string fileName = String.Empty;
                            string url = String.Empty;

                            if (item == TurkNippon_BaskiTipleri.TeklifPolice)
                            {
                                if (teklif)
                                {
                                    fileName = String.Format("TURKNIPPON_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                    url = storage.UploadFile("kasko", fileName, data);
                                    this.GenelBilgiler.PDFDosyasi = url;
                                    _Log.Info("Teklif_PDF url: {0}", url);
                                }
                                else
                                {
                                    fileName = String.Format("TURKNIPPON_Kasko_Police_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                    url = pdfStorage.UploadFile("kasko", fileName, data);
                                    this.GenelBilgiler.PDFPolice = url;
                                    _Log.Info("Teklif_PDF url: {0}", url);
                                }
                            }
                            if (item == TurkNippon_BaskiTipleri.BilgilendirmeFormu)
                            {
                                fileName = String.Format("TURKNIPPON_Kasko_Police_Bilgilendirme_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                url = pdfStorage.UploadFile("kasko", fileName, data);
                                this.GenelBilgiler.PDFBilgilendirme = url;
                            }
                        }
                        else
                        {
                            this.AddHata("PDF dosyası alınamadı.");
                            return;
                        }
                    }
                }
            }
        }

        public override void DekontPDF()
        {
            turknipponkasko.MODService clnt = new turknipponkasko.MODService();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONKasko);

                clnt.Url = konfig[Konfig.TURKNIPPON_KaskoServiceURL];
                clnt.Timeout = 150000;

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });

                #region Servis call

                string nipponPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, "0");
                string nipponUnitNo = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, "0");
                string nipponIslemTakipNo = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, "0");

                MODInput request = new MODInput();
                MODOutput response = new MODOutput();

                KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                string testMi = konfigTestMi[Konfig.TestMi];

                request.IsTestMode = false;
                if (Convert.ToBoolean(testMi))
                {
                    request.IsTestMode = true; // Zorunlu
                }
                request.Channel = Convert.ToInt32(servisKullanici.PartajNo_);
                request.Username = servisKullanici.KullaniciAdi;
                request.TrackingCode = nipponIslemTakipNo;
                request.UnitNo = Convert.ToInt64(nipponUnitNo);
                request.PolicyNo = Convert.ToInt64(nipponPoliceNo);
                request.ClientNo = Convert.ToInt64(nipponUnitNo);
                request.PrintType = TurkNippon_BaskiTipleri.KrediKartiOdemeFormu;

                this.BeginLog(request, typeof(MODInput), WebServisIstekTipleri.DekontPDF);
                response = clnt.Print(request);
                clnt.Dispose();
                if (!response.IsSuccess)
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.StatusDescription);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());

                    var policePDF = response.PrintDownloadUrl;
                    if (policePDF != null)
                    {
                        WebClient myClient = new WebClient();
                        byte[] data = myClient.DownloadData(policePDF);

                        IPolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Format("TURKNIPPON_Kasko_Police_KrediKartiOdemeFormu_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string url = pdfStorage.UploadFile("kasko", fileName, data);
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
                _Log.Error("TurkNipponKasko.DekontPDF", ex);
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        //public int GetKaskoIndirimi(ITeklif teklif, MusteriGenelBilgiler sigortali,TVMWebServisKullanicilari servisKullanici)
        //{
        //    turknipponkasko.MODService clnt = new turknipponkasko.MODService();
        //    KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONKasko);
        //    clnt.Url = konfig[Konfig.TURKNIPPON_KaskoServiceURL];
        //    clnt.Timeout = 150000;

        //    AuthHeader header = new AuthHeader();
        //    header.Username = servisKullanici.KullaniciAdi;
        //    header.Password = servisKullanici.Sifre;
        //    header.Channel = servisKullanici.PartajNo_;

        //    clnt.AuthHeaderValue = header;

        //    PlateDetail dt=new PlateDetail();
        //    dt.ilkodu=teklif.Arac.PlakaKodu.PadLeft(3,'0');
        //    dt.no=teklif.Arac.PlakaNo;

        //    DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
        //    IdentityDetail id=new IdentityDetail();
        //    if (teklif.SigortaEttiren.MusteriGenelBilgiler.KimlikNo.Length==10)
        //    {
        //        id.sigortaettirentckimliknumarasi =  String.Empty;
        //        id.sigortaettirenverginumarasi = teklif.SigortaEttiren.MusteriGenelBilgiler.KimlikNo;
        //    }
        //    else
        //    {
        //        id.sigortaettirentckimliknumarasi = teklif.SigortaEttiren.MusteriGenelBilgiler.KimlikNo;
        //        id.sigortaettirenverginumarasi = String.Empty;
        //    }            
        //    if (sigortali.KimlikNo.Length==10)
        //    {
        //        id.verginumarasi = sigortali.KimlikNo;
        //        id.tckimliknumarasi = String.Empty;
        //    }
        //    else
        //    {
        //        id.tckimliknumarasi = teklif.SigortaEttiren.MusteriGenelBilgiler.KimlikNo;
        //        id.verginumarasi = String.Empty;
        //    }          

        //    string msj = "";

        //    var mesaj = clnt.GetMaxAuthorizedDiscount(id, dt, policeBaslangic, out msj);

        //    clnt.Dispose();
        //    return mesaj;
        //}

        public string GetKaskoIndirimi(string sigortaliKimlikNo, string sigortaEttirenKimlikNo, string plakaKodu, string plakaNo, DateTime policeBaslangicTarihi, int tvmKodu)
        {
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
            turknipponkasko.MODService clnt = new turknipponkasko.MODService();
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONKasko);
            clnt.Url = konfig[Konfig.TURKNIPPON_KaskoServiceURL];
            clnt.Timeout = 150000;

            AuthHeader header = new AuthHeader();
            header.Username = servisKullanici.KullaniciAdi;
            header.Password = servisKullanici.Sifre;
            header.Channel = servisKullanici.PartajNo_;
            clnt.AuthHeaderValue = header;

            PlateDetail dt = new PlateDetail();
            dt.ilkodu = plakaKodu.PadLeft(3, '0');
            dt.no = plakaNo;

            IdentityDetail id = new IdentityDetail();
            if (sigortaEttirenKimlikNo.Length == 10)
            {
                id.sigortaettirentckimliknumarasi = String.Empty;
                id.sigortaettirenverginumarasi = sigortaEttirenKimlikNo;
            }
            else
            {
                id.sigortaettirentckimliknumarasi = sigortaEttirenKimlikNo;
                id.sigortaettirenverginumarasi = String.Empty;
            }
            if (sigortaliKimlikNo.Length == 10)
            {
                id.verginumarasi = sigortaliKimlikNo;
                id.tckimliknumarasi = String.Empty;
            }
            else
            {
                id.tckimliknumarasi = sigortaliKimlikNo;
                id.verginumarasi = String.Empty;
            }

            string msj = "";

            var mesaj = clnt.GetMaxAuthorizedDiscount(id, dt, policeBaslangicTarihi, out msj);
            clnt.Dispose();
            if (!String.IsNullOrEmpty(msj))
            {
                return msj;
            }
            else
            {
                return mesaj.ToString();
            }

        }
    }
}