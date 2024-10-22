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
using Neosinerji.BABOnlineTP.Business.turknippontss;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Kasko;


namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.YabanciSaglik
{
    public class TURKNIPPONYabanciSaglik : Teklif, ITURKNIPPONYabanciSaglik
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
        public TURKNIPPONYabanciSaglik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
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
            turknippontss.TssService clnt = new turknippontss.TssService();
            clnt.Timeout = 150000;
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONTSS);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                var getTVMDetay = _TVMService.GetDetay(tvmKodu);
                if (getTVMDetay != null)
                {
                    if (getTVMDetay.BagliOlduguTVMKodu != -9999)
                    {
                        tvmKodu = getTVMDetay.BagliOlduguTVMKodu;
                    }
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
                KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                string testMi = konfigTestMi[Konfig.TestMi];
                DateTime policeBaslangic = teklif.ReadSoru(TSSSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                DateTime policeBitis = teklif.ReadSoru(TSSSorular.Police_Bitis_Tarihi, TurkeyDateTime.Today);
                DateTime oncekiPoliceBaslangic = teklif.ReadSoru(TSSSorular.Eski_Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                bool yenilemeMi = teklif.ReadSoru(TSSSorular.YeniIsMi, false);

                bool kronikHastalikCerrahisiMi = teklif.ReadSoru(TSSSorular.KronikHastalikCerrahisi, false);
                string kronikHastalikAciklama = "";
                if (kronikHastalikCerrahisiMi)
                {
                    kronikHastalikAciklama = teklif.ReadSoru(TSSSorular.KronikHastalikAciklama, "");
                }

                string oncekiSirketKodu = teklif.ReadSoru(TSSSorular.Eski_Police_Sigorta_Sirketi, "");
                string oncekiPoliceNo = teklif.ReadSoru(TSSSorular.Eski_Police_No, "");
                string meslekKodu = teklif.ReadSoru(TSSSorular.Meslek, "");
                string tarifeGrupKodu = teklif.ReadSoru(TSSSorular.TarifeGrupKodu, "");

                decimal tedaviTipi = teklif.ReadSoru(TSSSorular.TedaviTipi, 1);
                int tedaviTip = Convert.ToInt32(tedaviTipi);

                bool YatarakTedaviMi = false;

                if (tedaviTip == 0)
                {
                    YatarakTedaviMi = false;
                }
                else
                {
                    YatarakTedaviMi = true;
                }

                var Boy = teklif.ReadSoru(TSSSorular.Boy, 0);
                var Kilo = teklif.ReadSoru(TSSSorular.Kilo, 0);

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriAdre adres = sigortali.MusteriAdres.FirstOrDefault(s => s.Varsayilan == true);
                MusteriTelefon SCepTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                MusteriTelefon SEvTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
                string musteriTel = "";
                if (SCepTel != null)
                {
                    musteriTel = SCepTel.Numara.Substring(3, 3) + SCepTel.Numara.Substring(7, 7);
                }
                if (SEvTel != null)
                {
                    musteriTel = SEvTel.Numara.Substring(3, 3) + SEvTel.Numara.Substring(7, 7);
                }
                TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
                MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
                MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
                MusteriTelefon SECepTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                MusteriTelefon SEEvTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
                string ilAdi = _UlkeService.GetIlAdi("TUR", adres.IlKodu);
                string ilceAdi = _UlkeService.GetIlceAdi(adres.IlceKodu.Value);

                bool sigortaliAyniMi = false;
                if (sigortali.KimlikNo == SEGenelBilgiler.KimlikNo)
                {
                    sigortaliAyniMi = true;
                }
                int odemeSekli = 0;
                if (teklif.GenelBilgiler.TaksitSayisi.HasValue && teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    odemeSekli = teklif.GenelBilgiler.TaksitSayisi.Value;
                    if (odemeSekli == 1)
                    {
                        odemeSekli = 2;
                    }
                    else if (odemeSekli == 2)
                    {
                        odemeSekli = 3;
                    }
                    else if (odemeSekli == 3)
                    {
                        odemeSekli = 4;
                    }
                    else if (odemeSekli == 4)
                    {
                        odemeSekli = 5;
                    }
                    else if (odemeSekli == 5)
                    {
                        odemeSekli = 6;
                    }
                }
                TssInput request = new TssInput()
                {
                    BeginDate = policeBaslangic,
                    CashPaymentType = odemeSekli,
                    Channel = Convert.ToInt32(servisKullanici.PartajNo_),
                    CitizenshipNumber = sigortali.KimlikNo.Length == 11 ? sigortali.KimlikNo : "",
                    IsTestMode = Convert.ToBoolean(testMi) ? true : false,
                    PhoneNumber = musteriTel,
                    PolicyNo = 0,
                    TaxNumber = sigortali.KimlikNo.Length == 10 ? sigortali.KimlikNo : "",
                    TrackingCode = "",
                    UnitNo = 0,
                    Username = servisKullanici.KullaniciAdi,
                    ChronicIllnessorSurgery = kronikHastalikCerrahisiMi,
                    ClientNo = 0,
                    CreditCard = new turknippontss.CreditCardInput(),
                    EndDate = policeBitis,
                    IdentityNo = "",
                    IsRenewal = yenilemeMi,
                    Job = meslekKodu,
                    PrintType = 0,
                    Size = Convert.ToInt32(Boy),
                    UseCreditCard = false,
                    Weight = Convert.ToInt32(Kilo),
                    TariffGroup = tarifeGrupKodu,
                    ClientIL = ilAdi,
                    ClientIC = ilceAdi,
                    ClientSM = "",
                    ClientSK = "",
                    ClientMH = "",
                    ClientCD = "",
                    ClientBD = "MERKEZ",
                    ClientAP = "",
                    PersonalOrGroup = "B", //Bireysel
                    PersonCount = 1,
                    outPatient = YatarakTedaviMi,
                };
                if (kronikHastalikCerrahisiMi)
                {
                   request.ChronicIllnessorSurgExplanation = kronikHastalikAciklama;
                }

                if (yenilemeMi)
                {
                    request.FirstPolicyBeginDate = oncekiPoliceBaslangic;
                    request.OldInsuranceCompany = oncekiSirketKodu;
                    request.OldPolicyNo = oncekiPoliceNo;
                }
                if (!sigortaliAyniMi)
                {
                    if (SEGenelBilgiler.KimlikNo.Length == 11)
                    {
                        request.ClientCitizenshipNumber = SEGenelBilgiler.KimlikNo;
                    }
                    else
                    {
                        request.ClientTaxNumber = SEGenelBilgiler.KimlikNo;
                    }
                    if (SECepTel != null)
                    {
                        request.ClientPhoneNumber = SECepTel.Numara.Substring(3, 3) + SECepTel.Numara.Substring(7, 7);
                    }
                    else if (SEEvTel != null)
                    {
                        request.ClientPhoneNumber = SEEvTel.Numara.Substring(3, 3) + SEEvTel.Numara.Substring(7, 7);
                    }
                }
                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);
                var response = clnt.Proposal(request);
                clnt.Dispose();
                teklif.Arac = null;
                if (!response.IsSuccess)
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.StatusDescription);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());
                }
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
                var tvmkod = teklif.GenelBilgiler.TVMKodu;
                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = response.BeginDate;
                this.GenelBilgiler.BitisTarihi = response.EndDate;
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = response.Premium;

                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                if (response != null) this.GenelBilgiler.ToplamKomisyon = response.ExchangeRate;
                else this.GenelBilgiler.ToplamKomisyon = 0;
                string policeno = response.PolicyNo.ToString();
                this.GenelBilgiler.TUMTeklifNo = policeno;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                this.GenelBilgiler.PDFDosyasi = response.PrintDownloadUrl;

                #endregion

                #region Web servis cevapları

                string musterino = response.UnitNo.ToString();
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, policeno);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, musterino);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, response.TrackingCode);


                #endregion

                int[] basimTipi = new int[3];
                basimTipi[0] = TurkNippon_BaskiTipleri.TeklifPolice;
                this.PDfGetir(servisKullanici, clnt, response.TrackingCode, policeno, musterino, basimTipi, true);

                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                clnt.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion

            }
        }

        public override void Policelestir(Odeme odeme)
        {
            turknippontss.TssService clnt = new turknippontss.TssService();
            clnt.Timeout = 150000;
            ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONTSS);
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            var getTVMDetay = _TVMService.GetDetay(tvmKodu);
            if (getTVMDetay != null)
            {
                if (getTVMDetay.BagliOlduguTVMKodu != -9999)
                {
                    tvmKodu = getTVMDetay.BagliOlduguTVMKodu;
                }
            }
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });

            KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
            string testMi = konfigTestMi[Konfig.TestMi];
            DateTime policeBaslangic = teklif.ReadSoru(TSSSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            DateTime policeBitis = teklif.ReadSoru(TSSSorular.Police_Bitis_Tarihi, TurkeyDateTime.Today);
            DateTime oncekiPoliceBaslangic = teklif.ReadSoru(TSSSorular.Eski_Police_Baslangic_Tarihi, TurkeyDateTime.Today);
            bool yenilemeMi = teklif.ReadSoru(TSSSorular.YeniIsMi, false);

            bool kronikHastalikCerrahisiMi = teklif.ReadSoru(TSSSorular.KronikHastalikCerrahisi, false);
            string oncekiSirketKodu = teklif.ReadSoru(TSSSorular.Eski_Police_Sigorta_Sirketi, "");
            string oncekiPoliceNo = teklif.ReadSoru(TSSSorular.Eski_Police_No, "");
            string meslekKodu = teklif.ReadSoru(TSSSorular.Meslek, "");
            string tarifeGrupKodu = teklif.ReadSoru(TSSSorular.TarifeGrupKodu, "");

            string tedaviTipi = teklif.ReadSoru(TSSSorular.TedaviTipi, "");
            bool YatarakTedaviMi = false;

            if (tedaviTipi == "0")
            {
                YatarakTedaviMi = false;
            }
            else
            {
                YatarakTedaviMi = true;
            }
            int odemeSekli = 0;
            if (teklif.GenelBilgiler.TaksitSayisi.HasValue && teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
            {
                odemeSekli = teklif.GenelBilgiler.TaksitSayisi.Value;
                if (odemeSekli == 1)
                {
                    odemeSekli = 2;
                }
                else if (odemeSekli == 2)
                {
                    odemeSekli = 3;
                }
                else if (odemeSekli == 3)
                {
                    odemeSekli = 4;
                }
                else if (odemeSekli == 4)
                {
                    odemeSekli = 5;
                }
                else if (odemeSekli == 5)
                {
                    odemeSekli = 6;
                }
            }
            var Boy = teklif.ReadSoru(TSSSorular.Boy, 0);
            var Kilo = teklif.ReadSoru(TSSSorular.Kilo, 0);

            MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriAdre adres = sigortali.MusteriAdres.FirstOrDefault(s => s.Varsayilan == true);
            MusteriTelefon SCepTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon SEvTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            string musteriTel = "";
            if (SCepTel != null)
            {
                musteriTel = SCepTel.Numara.Substring(3, 3) + SCepTel.Numara.Substring(7, 7);
            }
            if (SEvTel != null)
            {
                musteriTel = SEvTel.Numara.Substring(3, 3) + SEvTel.Numara.Substring(7, 7);
            }
            TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
            MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
            MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
            MusteriTelefon SECepTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon SEEvTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            string ilAdi = _UlkeService.GetIlAdi("TUR", adres.IlKodu);
            string ilceAdi = _UlkeService.GetIlceAdi(adres.IlceKodu.Value);

            bool sigortaliAyniMi = false;
            if (sigortali.KimlikNo == SEGenelBilgiler.KimlikNo)
            {
                sigortaliAyniMi = true;
            }


            string IslemTakipKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, "0");
            string SigortaliMusteriNo = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, "0");
            string PoliceNumarasi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, "0");


            string adi = String.Empty;
            string soyadi = String.Empty;
            try
            {
                TssInput request = new TssInput()
                {
                    BeginDate = policeBaslangic,
                    CashPaymentType = odemeSekli,
                    Channel = Convert.ToInt32(servisKullanici.PartajNo_),
                    CitizenshipNumber = sigortali.KimlikNo.Length == 11 ? sigortali.KimlikNo : "",
                    IsTestMode = Convert.ToBoolean(testMi) ? true : false,
                    PhoneNumber = musteriTel,
                    PolicyNo = Convert.ToInt32(PoliceNumarasi),
                    TaxNumber = sigortali.KimlikNo.Length == 10 ? sigortali.KimlikNo : "",
                    TrackingCode = IslemTakipKodu,
                    UnitNo = Convert.ToInt32(SigortaliMusteriNo),
                    Username = servisKullanici.KullaniciAdi,
                    ChronicIllnessorSurgery = kronikHastalikCerrahisiMi,
                    ClientNo = Convert.ToInt32(SigortaliMusteriNo),
                    CreditCard = new turknippontss.CreditCardInput(),
                    EndDate = policeBitis,
                    IdentityNo = "",
                    IsRenewal = yenilemeMi,
                    Job = meslekKodu,
                    PrintType = 0,
                    Size = Convert.ToInt32(Boy),
                    UseCreditCard = false,
                    Weight = Convert.ToInt32(Kilo),
                    TariffGroup = tarifeGrupKodu,
                    ClientIL = ilAdi,
                    ClientIC = ilceAdi,
                    ClientSM = "",
                    ClientSK = "",
                    ClientMH = "",
                    ClientCD = "",
                    ClientBD = "MERKEZ",
                    ClientAP = "",
                    PersonalOrGroup = "B", //Bireysel
                    PersonCount = 1,
                    outPatient = YatarakTedaviMi,
                };
                if (yenilemeMi)
                {
                    request.FirstPolicyBeginDate = oncekiPoliceBaslangic;
                    request.OldInsuranceCompany = oncekiSirketKodu;
                    request.OldPolicyNo = oncekiPoliceNo;
                }
                if (!sigortaliAyniMi)
                {
                    if (SEGenelBilgiler.KimlikNo.Length == 11)
                    {
                        request.ClientCitizenshipNumber = SEGenelBilgiler.KimlikNo;
                    }
                    else
                    {
                        request.ClientTaxNumber = SEGenelBilgiler.KimlikNo;
                    }
                    if (SECepTel != null)
                    {
                        request.ClientPhoneNumber = SECepTel.Numara.Substring(3, 3) + SECepTel.Numara.Substring(7, 7);
                    }
                    else if (SEEvTel != null)
                    {
                        request.ClientPhoneNumber = SEEvTel.Numara.Substring(3, 3) + SEEvTel.Numara.Substring(7, 7);
                    }
                }
                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
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
                #region Ödeme Biglileri
                request.CreditCard = new turknippontss.CreditCardInput();
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
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

                this.BeginLog(request, typeof(TssInput), WebServisIstekTipleri.Police);

                //KrediKarti bilgileri loglanmıyor..
                #region KrediKartı Bilgileri
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
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
                var response = clnt.Approve(request);
                clnt.Dispose();

                #region Hata Kontrol ve Kayıt

                if (!response.IsSuccess)
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.StatusDescription);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());

                    this.GenelBilgiler.TUMPoliceNo = response.Input.PolicyNo.ToString();
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
                    this.GenelBilgiler.OdemeSekli = odeme.OdemeSekli;
                    this.GenelBilgiler.OdemeTipi = odeme.OdemeTipi;
                    this.GenelBilgiler.TaksitSayisi = odeme.TaksitSayisi;

                    int[] basimTipi = new int[3];
                    basimTipi[0] = TurkNippon_BaskiTipleri.TeklifPolice;
                    basimTipi[1] = TurkNippon_BaskiTipleri.BilgilendirmeFormu;
                    basimTipi[2] = TurkNippon_BaskiTipleri.KrediKartiOdemeFormu;
                    this.PDfGetir(servisKullanici, clnt, IslemTakipKodu, PoliceNumarasi, SigortaliMusteriNo, basimTipi, false);

                }
                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
            catch (Exception ex)
            {

                _Log.Error("TurkNipponSagliginizBizdeHMO.Policelestir", ex);
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        public List<ListModel> GetMeslekListesi()
        {
            turknippontss.TssService clnt = new turknippontss.TssService();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONTSS);
            List<ListModel> list = new List<ListModel>();
            ListModel model = new ListModel();

            var response = clnt.GetJobList();
            if (response != null)
            {
                foreach (var item in response)
                {
                    model = new ListModel();
                    model.kodu = item.JobCode.ToString();
                    model.aciklama = item.JobDescription;
                    list.Add(model);
                }
            }
            return list;
        }

        public List<ListModel> GetTarifeGrupListesi()
        {
            turknippontss.TssService clnt = new turknippontss.TssService();

            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONTSS);

            List<ListModel> list = new List<ListModel>();
            ListModel model = new ListModel();

            var response = clnt.GetTariffGroupList();
            if (response != null)
            {
                foreach (var item in response)
                {
                    model = new ListModel();
                    model.kodu = item.TariffGroupCode.ToString();
                    model.aciklama = item.TariffGroupDescription;
                    list.Add(model);
                }
            }
            return list;
        }

        public void PDfGetir(TVMWebServisKullanicilari servisKullanici, TssService clnt, string islemTakipKodu, string policeNo, string musteriNo, int[] basimtipi, bool teklif)
        {
            KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
            string testMi = konfigTestMi[Konfig.TestMi];
            TssInput request = new TssInput();
            TssOutput response = new TssOutput();

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
            request.UseCreditCard = false;
            request.CreditCard = new turknippontss.CreditCardInput();
            foreach (var item in basimtipi)
            {
                if (item > 0)
                {
                    request.PrintType = item;

                    this.BeginLog(request, typeof(TssInput), WebServisIstekTipleri.Police);
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
                                    fileName = String.Format("TURKNIPPON_TSS_Teklif_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                    url = storage.UploadFile("tss", fileName, data);
                                    this.GenelBilgiler.PDFDosyasi = url;
                                    _Log.Info("Teklif_PDF url: {0}", url);
                                }
                                else
                                {
                                    fileName = String.Format("TURKNIPPON_TSS_Police_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                    url = pdfStorage.UploadFile("tss", fileName, data);
                                    this.GenelBilgiler.PDFPolice = url;
                                    _Log.Info("Teklif_PDF url: {0}", url);
                                }
                            }
                            if (item == TurkNippon_BaskiTipleri.BilgilendirmeFormu)
                            {
                                fileName = String.Format("TURKNIPPON_TSS_Police_Bilgilendirme_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                url = pdfStorage.UploadFile("tss", fileName, data);
                                this.GenelBilgiler.PDFBilgilendirme = url;
                            }
                            if (item == TurkNippon_BaskiTipleri.KrediKartiOdemeFormu)
                            {
                                fileName = String.Format("TURKNIPPON_TSS_KK_Dekont_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                url = pdfStorage.UploadFile("tss", fileName, data);
                                this.GenelBilgiler.PDFDekont = url;
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
    }
}
