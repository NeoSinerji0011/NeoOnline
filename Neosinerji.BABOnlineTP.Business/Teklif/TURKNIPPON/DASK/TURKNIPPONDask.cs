using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Common.TURKNIPPON;
using Neosinerji.BABOnlineTP.Business.turknippon.dask;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK;
using Neosinerji.BABOnlineTP.Business.TURKNIPPON.Messages;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.DASK
{
    public class TURKNIPPONDask : Teklif, ITURKNIPPONDask
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
        public TURKNIPPONDask(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService,
            ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService, IAktifKullaniciService aktifKullaniciService)
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
                return TeklifUretimMerkezleri.TURKNIPPON;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;

                #region Ana Bilgiler
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);

                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon cepTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                MusteriTelefon evTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);

                TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
                MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
                MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
                MusteriTelefon cepTelSE = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                MusteriTelefon evTelSE = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
                #endregion
                DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                //Gönderilmediği taktirde UAVT’den gelen veriler kullanılmaktadır. 
                TitleDeedModel TitleDeed = new TitleDeedModel();
                TitleDeed.BB = ""; //Bağımsız bölüm numarası 
                TitleDeed.AD = teklif.ReadSoru(DASKSorular.Riziko_Ada, String.Empty); //Ada 
                TitleDeed.PF = teklif.ReadSoru(DASKSorular.Riziko_Pafta_No, String.Empty); //Pafta 
                TitleDeed.PR = teklif.ReadSoru(DASKSorular.Riziko_Parsel, String.Empty);  //Parsel 
                TitleDeed.SY = teklif.ReadSoru(DASKSorular.Riziko_Sayfa_No, String.Empty); //Sayfa no 


                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };

                DASKInput request = new DASKInput();

                request.Channel = Convert.ToInt32(servisKullanici.PartajNo_);
                request.Username = servisKullanici.KullaniciAdi;
                request.TrackingCode = "";
                string OncekiPolice = teklif.ReadSoru(DASKSorular.Yururlukte_dask_policesi_VarYok, "H");
                string OncekiPoliceNumarasi = teklif.ReadSoru(DASKSorular.Dask_Police_Numarasi, "0");

                request.PolicyNo = 0;
                request.UseCreditCard = false;

                if (OncekiPolice == "E")
                {
                    request.IsRenewal = true;
                    request.DASKPolicyNo = Convert.ToInt32(OncekiPoliceNumarasi);
                }
                else
                {
                    request.IsRenewal = false;
                    request.DASKPolicyNo = 0;



                    #region Sigortalı /Sigorta Ettiren Bilgileri

                    request.UnitNo = 0;  //CitizenshipNumber veya TaxNumber yoksa zorunlu  sigortalı müşteri no
                    request.ClientNo = 0; //sigorta ettiren müşteri no
                    if (sigortali.KimlikNo.Length == 11)
                    {
                        request.CitizenshipNumber = sigortali.KimlikNo;

                    }
                    else
                    {
                        request.TaxNumber = sigortali.KimlikNo; //sigortalı vergi no

                    }

                    if (evTel != null && evTel.Numara.Length > 10)
                    {
                        request.PhoneNumber = evTel.Numara.Substring(3, 3) + evTel.Numara.Substring(7, 7);//sigortalı tel no //gönderilmesi tavsiye edilir
                    }
                    if (cepTel != null && cepTel.Numara.Length > 10)
                    {
                        request.MobilePhoneNumber = cepTel.Numara.Substring(3, 3) + cepTel.Numara.Substring(7, 7); //sigortalı tel no //gönderilmesi tavsiye edilir
                    }

                    if (SEGenelBilgiler.KimlikNo.Length == 11)
                    {
                        request.ClientCitizenshipNumber = SEGenelBilgiler.KimlikNo;
                    }
                    else
                    {
                        request.ClientTaxNumber = SEGenelBilgiler.KimlikNo;
                    }
                    if (evTelSE != null && evTelSE.Numara.Length > 10)
                    {
                        request.ClientMobilePhoneNumber = evTelSE.Numara.Substring(3, 3) + evTelSE.Numara.Substring(7, 7);//gönderilmesi tavsiye edilir
                    }
                    if (cepTelSE != null && cepTelSE.Numara.Length > 10)
                    {
                        request.ClientMobilePhoneNumber = cepTelSE.Numara.Substring(3, 3) + cepTelSE.Numara.Substring(7, 7);//gönderilmesi tavsiye edilir
                    }

                    #endregion

                    var yapiTarzi = Convert.ToByte(teklif.ReadSoru(DASKSorular.Yapi_Tarzi, "0"));
                    string ToplamKatSayisi = teklif.ReadSoru(DASKSorular.Bina_Kat_sayisi, String.Empty);
                    string BinaInsaatYili = teklif.ReadSoru(DASKSorular.Bina_Insa_Yili, String.Empty);
                    byte DaireKullanimSekli = Convert.ToByte(teklif.ReadSoru(DASKSorular.Daire_KullanimSekli, "0"));
                    string DaireYuzOlcumu = teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                    string BinaHasar = teklif.ReadSoru(DASKSorular.Hasar_Durumu, String.Empty);
                    string SESifat = teklif.ReadSoru(DASKSorular.Sigorta_Ettiren_Sifati, "");


                    request.BeginDate = policeBaslangic;
                    if (SESifat != "")
                    {
                        request.InsurerType = Convert.ToInt32(SESifat);
                    }

                    request.RiskAddressCode = teklif.RizikoAdresi.UAVTKodu;
                    request.TitleDeed = TitleDeed;
                    if (DaireYuzOlcumu != "")
                    {
                        request.GrossAreaM2 = Convert.ToInt32(DaireYuzOlcumu);
                    }

                    if (DaireKullanimSekli == DaskBinaKullanimSekli.Mesken)
                    {
                        request.UsageType = TURKNIPPONUsageType.Mesken;
                    }
                    else if (DaireKullanimSekli == DaskBinaKullanimSekli.Buro)
                    {
                        request.UsageType = TURKNIPPONUsageType.Buro;
                    }
                    else if (DaireKullanimSekli == DaskBinaKullanimSekli.Ticarethane)
                    {
                        request.UsageType = TURKNIPPONUsageType.Ticarethane;
                    }
                    else
                    {
                        request.UsageType = TURKNIPPONUsageType.Diger;
                    }

                    if (yapiTarzi == KonutBinaYapiTazrlari.Celik_Betonarme_Karkas)
                    {
                        request.BuildType = TURKNIPPONBuildType.CelikBetonarmeKarkas;
                    }
                    else if (yapiTarzi == KonutBinaYapiTazrlari.Yigma_Kagir)
                    {
                        request.BuildType = TURKNIPPONBuildType.YigmaKagir;
                    }
                    else
                    {
                        request.BuildType = TURKNIPPONBuildType.Diger;
                    }
                    if (BinaInsaatYili != "")
                    {
                        request.BuildYear = Convert.ToInt32(BinaInsaatYili);
                    }
                    if (ToplamKatSayisi != "")
                    {
                        request.TotalFloor = Convert.ToInt32(ToplamKatSayisi);
                    }
                    if (BinaHasar != "")
                    {
                        request.AnteriorDamage = Convert.ToInt32(BinaHasar);
                    }


                    string RehinliAlacak = teklif.ReadSoru(DASKSorular.RA_Dain_i_Muhtehin_VarYok, String.Empty);
                    if (!String.IsNullOrEmpty(RehinliAlacak) && RehinliAlacak == "0")
                        request.LossPayeeType = TURKNIPPONLossPayeeType.Yok;

                    if (RehinliAlacak == "1")
                    {
                        string Tipi = teklif.ReadSoru(DASKSorular.RA_Tipi_Banka_Finansal_Kurum, String.Empty);
                        string KurumId = teklif.ReadSoru(DASKSorular.RA_Kurum_Banka, String.Empty);
                        if (Tipi == "1")
                        {
                            request.LossPayeeType = TURKNIPPONLossPayeeType.Banka;

                            request.Bank = KurumId.PadLeft(4, '0');
                            string SubeId = teklif.ReadSoru(DASKSorular.RA_Sube, String.Empty);
                            request.BankBranch = SubeId.PadLeft(5, '0'); ;
                        }
                        else
                        {
                            request.LossPayeeType = TURKNIPPONLossPayeeType.FinansKurumu;
                            request.FinancialInstitution = KurumId.PadLeft(3, '0');
                        }

                        string Kredi_Rfrns_No_Hsp_ = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, String.Empty);
                        DateTime KrediBitisTarihi = teklif.ReadSoru(DASKSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);
                        decimal KrediTutari = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, 0);
                        string DovizKodu = teklif.ReadSoru(DASKSorular.RA_Doviz_Kodu, "0");
                    }
                }
                #region Service Call
                DASKOuput proposalresult = new DASKOuput();
                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);
                var response = clnt.Proposal(request);
                clnt.Dispose();

                #endregion

                #region Varsa Hata Kaydı

                if (!response.IsSuccess)
                {
                    if (!String.IsNullOrEmpty(response.StatusDescription))
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.StatusDescription);
                    }
                }              
                else
                {
                    this.EndLog(response, true, response.GetType());
                    
                }
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

                if (!String.IsNullOrEmpty(response.UnitName))
                {
                    response.UnitName = response.UnitName.Trim();
                    var parts = response.UnitName.Split(' ');
                    if (parts.Length == 2)
                    {
                        sigortali.AdiUnvan = parts[0];
                        sigortali.SoyadiUnvan = parts[1];
                    }
                    else if (parts.Length == 3)
                    {
                        sigortali.AdiUnvan = parts[0] + ' ' + parts[1];
                        sigortali.SoyadiUnvan = parts[2];
                    }

                    _MusteriService.UpdateMusteriAdi(sigortali);
                }

                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = response.BeginDate;
                this.GenelBilgiler.BitisTarihi = response.EndDate;
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = response.Premium;
                this.GenelBilgiler.NetPrim = response.Premium;
                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                this.GenelBilgiler.TUMTeklifNo = response.PolicyNo.ToString();
                #endregion

                #region Web servis cevapları

                string musterino = response.UnitNo.ToString();
                string clientNo = response.ClientNo.ToString();
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, response.PolicyNo.ToString());
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, musterino);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Client_No, clientNo);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, response.TrackingCode);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_DaskUyariMesaji, response.DASKWarning);
                int[] basimTipi = new int[3];
                basimTipi[0] = TurkNippon_BaskiTipleri.TeklifPolice;
                this.PDfGetir(servisKullanici, clnt, response.TrackingCode, response.PolicyNo.ToString(), musterino, basimTipi, true);
                #endregion

                if (teklif.GenelBilgiler.TaksitSayisi.HasValue)
                {
                    if (teklif.GenelBilgiler.TaksitSayisi.Value < 4 & teklif.GenelBilgiler.TaksitSayisi.Value > 0)
                        this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi.Value;
                    else
                        this.GenelBilgiler.TaksitSayisi = 3;
                }
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

                #region Teminat

                //	<PoliceBedeli>148400.00</PoliceBedeli> Dairenin hasar durumunda alacağı teminatı donuyor.
                this.AddTeminat(DASKTeminatlar.DASK, response.DASKInsuranceAmount, 0, 0, 0, 0);

                #endregion

                //int[] basimTipi = new int[3];
                //basimTipi[0] = TurkNippon_BaskiTipleri.TeklifPolice;
                //this.PDfGetir(servisKullanici, clnt, response.TrackingCode, response.PolicyNo.ToString(), musterino, basimTipi, true);

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
            turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
            try
            {
                #region Veri Hazırlama

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);

                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                clnt.Timeout = 150000;
                TURKNIPPON_Proposal_Response result = new TURKNIPPON_Proposal_Response();
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon Telefon = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                DASKInput request = new DASKInput();
                DASKOuput response = new DASKOuput();

                KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                string testMi = konfigTestMi[Konfig.TestMi];
                #endregion

                #region Genel Bilgiler

                string IslemTakipKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, "0");
                string SigortaliMusteriNo = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, "0");
                string PoliceNumarasi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, "0");
                string ClientNumarasi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Client_No, "0");

                string adi = String.Empty;
                string soyadi = String.Empty;

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
                request.IsTestMode = false;
                if (Convert.ToBoolean(testMi))
                {
                    request.IsTestMode = true; // Zorunlu
                }
                request.Channel = Convert.ToInt32(servisKullanici.PartajNo_);
                request.Username = servisKullanici.KullaniciAdi;
                request.TrackingCode = IslemTakipKodu;
                request.UnitNo = Convert.ToInt64(SigortaliMusteriNo);             
                request.PolicyNo = Convert.ToInt64(PoliceNumarasi);
                request.ClientNo = Convert.ToInt64(ClientNumarasi);
                request.RiskAddressCode = teklif.RizikoAdresi.UAVTKodu;

                string OncekiPolice = teklif.ReadSoru(DASKSorular.Yururlukte_dask_policesi_VarYok, "H");
                string OncekiPoliceNumarasi = teklif.ReadSoru(DASKSorular.Dask_Police_Numarasi, "0");
                if (OncekiPolice == "E")
                {
                    request.IsRenewal = true;
                    request.DASKPolicyNo = Convert.ToInt64(OncekiPoliceNumarasi);
                }
                else
                {
                    request.IsRenewal = false;
                    DateTime policeBaslangic = teklif.ReadSoru(KaskoSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                    request.BeginDate = policeBaslangic;

                    var yapiTarzi = Convert.ToByte(teklif.ReadSoru(DASKSorular.Yapi_Tarzi, "0"));
                    string ToplamKatSayisi = teklif.ReadSoru(DASKSorular.Bina_Kat_sayisi, String.Empty);
                    string BinaInsaatYili = teklif.ReadSoru(DASKSorular.Bina_Insa_Yili, String.Empty);
                    byte DaireKullanimSekli = Convert.ToByte(teklif.ReadSoru(DASKSorular.Daire_KullanimSekli, "0"));
                    string DaireYuzOlcumu = teklif.ReadSoru(DASKSorular.Daire_Brut_Yuzolcumu_M2, String.Empty);
                    string BinaHasar = teklif.ReadSoru(DASKSorular.Hasar_Durumu, String.Empty);
                    string SESifat = teklif.ReadSoru(DASKSorular.Sigorta_Ettiren_Sifati, "");                   
                    if (SESifat != "")
                    {
                        request.InsurerType = Convert.ToInt32(SESifat);
                    }
                    if (DaireYuzOlcumu != "")
                    {
                        request.GrossAreaM2 = Convert.ToInt32(DaireYuzOlcumu);
                    }

                    if (DaireKullanimSekli == DaskBinaKullanimSekli.Mesken)
                    {
                        request.UsageType = TURKNIPPONUsageType.Mesken;
                    }
                    else if (DaireKullanimSekli == DaskBinaKullanimSekli.Buro)
                    {
                        request.UsageType = TURKNIPPONUsageType.Buro;
                    }
                    else if (DaireKullanimSekli == DaskBinaKullanimSekli.Ticarethane)
                    {
                        request.UsageType = TURKNIPPONUsageType.Ticarethane;
                    }
                    else
                    {
                        request.UsageType = TURKNIPPONUsageType.Diger;
                    }

                    if (yapiTarzi == KonutBinaYapiTazrlari.Celik_Betonarme_Karkas)
                    {
                        request.BuildType = TURKNIPPONBuildType.CelikBetonarmeKarkas;
                    }
                    else if (yapiTarzi == KonutBinaYapiTazrlari.Yigma_Kagir)
                    {
                        request.BuildType = TURKNIPPONBuildType.YigmaKagir;
                    }
                    else
                    {
                        request.BuildType = TURKNIPPONBuildType.Diger;
                    }
                    if (BinaInsaatYili != "")
                    {
                        request.BuildYear = Convert.ToInt32(BinaInsaatYili);
                    }
                    if (ToplamKatSayisi != "")
                    {
                        request.TotalFloor = Convert.ToInt32(ToplamKatSayisi);
                    }
                    if (BinaHasar != "")
                    {
                        request.AnteriorDamage = Convert.ToInt32(BinaHasar);
                    }
                   

                    string RehinliAlacak = teklif.ReadSoru(DASKSorular.RA_Dain_i_Muhtehin_VarYok, String.Empty);
                    if (!String.IsNullOrEmpty(RehinliAlacak) && RehinliAlacak == "0")
                        request.LossPayeeType = TURKNIPPONLossPayeeType.Yok;

                    if (RehinliAlacak == "1")
                    {
                        string Tipi = teklif.ReadSoru(DASKSorular.RA_Tipi_Banka_Finansal_Kurum, String.Empty);
                        string KurumId = teklif.ReadSoru(DASKSorular.RA_Kurum_Banka, String.Empty);
                        if (Tipi == "1")
                        {
                            request.LossPayeeType = TURKNIPPONLossPayeeType.Banka;

                            request.Bank = KurumId.PadLeft(4, '0');
                            string SubeId = teklif.ReadSoru(DASKSorular.RA_Sube, String.Empty);
                            request.BankBranch = SubeId.PadLeft(5, '0'); ;
                        }
                        else
                        {
                            request.LossPayeeType = TURKNIPPONLossPayeeType.FinansKurumu;
                            request.FinancialInstitution = KurumId.PadLeft(3, '0');
                        }

                        string Kredi_Rfrns_No_Hsp_ = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, String.Empty);
                        DateTime KrediBitisTarihi = teklif.ReadSoru(DASKSorular.RA_Kredi_Bitis_Tarihi, DateTime.MinValue);
                        decimal KrediTutari = teklif.ReadSoru(DASKSorular.RA_Kredi_Referans_No_Hesap_Sozlesme_No, 0);
                        string DovizKodu = teklif.ReadSoru(DASKSorular.RA_Doviz_Kodu, "0");
                    }
                }
                #endregion

                #region Ödeme Biglileri
                request.CreditCard = new CreditCardInput();
                request.UseCreditCard = false;
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

                #region Service Call

                this.BeginLog(request, typeof(DASKInput), WebServisIstekTipleri.Police);

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
                    this.GenelBilgiler.BrutPrim = response.Premium;
                    this.GenelBilgiler.NetPrim = response.Premium;
                    this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_DaskUyariMesaji, response.DASKWarning);

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

        public void PDfGetir(TVMWebServisKullanicilari servisKullanici, DASKServicev2 clnt, string islemTakipKodu, string policeNo, string musteriNo, int[] basimtipi, bool teklif)
        {
            KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
            string testMi = konfigTestMi[Konfig.TestMi];
            DASKInput request = new DASKInput();
            DASKOuput response = new DASKOuput();

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

                    this.BeginLog(request, typeof(DASKInput), WebServisIstekTipleri.Police);
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
                                    fileName = String.Format("TURKNIPPON_DASK_Teklif_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                    url = storage.UploadFile("dask", fileName, data);
                                    this.GenelBilgiler.PDFDosyasi = url;
                                    _Log.Info("Teklif_PDF url: {0}", url);
                                }
                                else
                                {
                                    fileName = String.Format("TURKNIPPON_DASK_Police_{0}.pdf", System.Guid.NewGuid().ToString("N"));
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

        public ReturnAdresModel GetAdresDetay(long UAVTKodu)
        {
            turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
            clnt.Timeout = 250000;
            int tvmkodu = 0;
            var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
            if (tvmDetay.BagliOlduguTVMKodu == -9999)
            {
                tvmkodu = tvmDetay.Kodu;
            }
            else
            {
                tvmkodu = tvmDetay.BagliOlduguTVMKodu;
            }
            TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
            clnt.AuthHeaderValue = new AuthHeader()
            {
                Username = servisKullanici.KullaniciAdi,
                Channel = servisKullanici.PartajNo_,
                Password = servisKullanici.Sifre
            };
            ReturnAdresModel model = new ReturnAdresModel();
            JetSatisAddressModel adresDetay = clnt.GetAddressDetail(UAVTKodu);
            if (adresDetay != null)
            {
                model.IlKodu = adresDetay.IL_CODE;
                model.IlAdi = adresDetay.IL;
                model.IlceKodu = adresDetay.IC_CODE;
                model.IlceAdi = adresDetay.IC;
                model.BeldeKodu = adresDetay.BE_CODE;
                model.BeldeAdi = adresDetay.BE;
                model.MahalleKodu = adresDetay.MH_CODE;
                model.MahalleAdi = adresDetay.MH;
                model.CaddeSokakKodu = adresDetay.CS_CODE;
                model.MahalleAdi = adresDetay.MH;
                model.CaddeAdi = adresDetay.CD;
                model.SokakAdi = adresDetay.SK;
                model.SiteAdi = adresDetay.ST;
                model.ApartmanAdi = adresDetay.AP;
                model.BinaKodu = adresDetay.BN_CODE;
                model.BinaNo = adresDetay.BN;
                model.DaireNo = adresDetay.DR;
                model.Kat = adresDetay.KT;
                model.Ada = adresDetay.AD;
                model.Pafta = adresDetay.PF;
                model.Parsel = adresDetay.PR;
                model.SayfaNo = adresDetay.SY;
                model.UAVTAdresKodu = adresDetay.AK;
                model.Hata = "";
            }
            else
            {
                model.Hata = "Hata! UAVT Koduna ait kayıt bulunamadı.";
            }
            return model;
        }

        public List<ListOutput> GetIlList()
        {
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                list = clnt.GetCityList().ToList();
                if (list != null)
                {
                    list = list.OrderBy(s => s.Description).ToList();
                }
            }
            catch (Exception ex)
            {

                _Log.Error(ex.Message);
            }
            return list;
        }

        public List<ListOutput> GetIlceList(int ilKodu)
        {
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                list = clnt.GetTownList(ilKodu).ToList();
                if (list != null)
                {
                    list = list.OrderBy(s => s.Description).ToList();
                }
            }
            catch (Exception ex)
            {

                _Log.Error(ex.Message);
            }
            return list;
        }

        public List<ListOutput> GetBeldeList(int ilceKodu)
        {
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                list = clnt.GetMunicipalityList(ilceKodu).ToList();
                if (list != null)
                {
                    list = list.OrderBy(s => s.Description).ToList();
                }
            }
            catch (Exception ex)
            {

                _Log.Error(ex.Message);
            }
            return list;
        }

        public List<ListOutput> GetMahalleList(int beldeKodu)
        {
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                list = clnt.GetQuarterList(beldeKodu).ToList();
                if (list != null)
                {
                    list = list.OrderBy(s => s.Description).ToList();
                }
            }
            catch (Exception ex)
            {

                _Log.Error(ex.Message);
            }
            return list;
        }

        public List<ListOutput> GetCaddeSokakList(int mahalleKodu)
        {
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                list = clnt.GetStreetList(mahalleKodu).ToList();
                if (list != null)
                {
                    list = list.OrderBy(s => s.Description).ToList();
                }
            }
            catch (Exception ex)
            {

                _Log.Error(ex.Message);
            }
            return list;
        }

        public List<ListOutput> GetBinaList(int caddeSokakKodu)
        {
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                list = clnt.GetBuildingList(caddeSokakKodu).ToList();
                if (list != null)
                {
                    list = list.OrderBy(s => s.Description).ToList();
                }
            }
            catch (Exception ex)
            {

                _Log.Error(ex.Message);
            }
            return list;
        }

        public List<ListOutput> GetBagimsizBolumList(int binaKodu)
        {
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];
                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                list = clnt.GetSectionList(binaKodu).ToList();
                if (list != null)
                {
                    list = list.OrderBy(s => s.Description).ToList();
                }
            }
            catch (Exception ex)
            {

                _Log.Error(ex.Message);
            }
            return list;
        }

        public PoliceBilgileri GetPoliceDetay(long PolNo, int? YenilemeNo, int? EkNo)
        {
            PoliceBilgileri policeBilgileri = new PoliceBilgileri();
            List<ListOutput> list = new List<ListOutput>();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONDASK);
                turknippon.dask.DASKServicev2 clnt = new turknippon.dask.DASKServicev2();
                clnt.Timeout = 250000;
                clnt.Url = konfig[Konfig.TURKNIPPON_DASK_ServisURL];

                KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                string testMi = konfigTestMi[Konfig.TestMi];

                int tvmkodu = 0;
                var tvmDetay = _TVMService.GetDetay(_AktifKullaniciService.TVMKodu);
                if (tvmDetay.BagliOlduguTVMKodu == -9999)
                {
                    tvmkodu = tvmDetay.Kodu;
                }
                else
                {
                    tvmkodu = tvmDetay.BagliOlduguTVMKodu;
                }
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmkodu, TeklifUretimMerkezleri.TURKNIPPON });
                clnt.AuthHeaderValue = new AuthHeader()
                {
                    Username = servisKullanici.KullaniciAdi,
                    Channel = servisKullanici.PartajNo_,
                    Password = servisKullanici.Sifre
                };
                string ResponseXml = "";
                if (testMi == "false")
                {
                    ResponseXml = clnt.GetPolicyDetailXML(PolNo, 0, 0, false);
                }
                else
                {
                    ResponseXml = clnt.GetPolicyDetailXML(PolNo, 0, 0, true);
                }

                XmlDocument doc = new XmlDocument();
                XmlNode root;
                XmlNode s;
                doc.LoadXml(ResponseXml);
                root = doc.FirstChild;
                s = root.Clone();
                string uavtKodu = "";
                if (s.Name != "POLICY")
                {
                    var policyNodes = doc.ChildNodes;
                    for (int i = 0; i < policyNodes.Count; i++)
                    {
                        var node = policyNodes[i];
                        if (node.Name == "POLICY")
                        {
                            var polmasNodes = node.ChildNodes;
                            for (int indx = 0; indx < polmasNodes.Count; indx++)
                            {
                                XmlNode elm = polmasNodes.Item(indx);
                                XmlNodeList polmasChild = elm.ChildNodes;
                                for (int j = 0; j < polmasChild.Count; j++)
                                {
                                    if (polmasChild[j].Name == "INSURED_NAME")
                                    {
                                        policeBilgileri.SigortaliAdi = polmasChild[j].InnerText;
                                    }
                                    else if (polmasChild[j].Name == "INSURED_SURNAME")
                                    {
                                        policeBilgileri.SigortaliSoyadi = polmasChild[j].InnerText;
                                    }
                                    else if (polmasChild[j].Name == "POLADC")
                                    {
                                        policeBilgileri.AcikAdres = polmasChild[j].FirstChild.InnerText;
                                        if (policeBilgileri.AcikAdres != null)
                                        {
                                            var parts = policeBilgileri.AcikAdres.Split(' ');
                                            if (parts.Count() > 0)
                                            {
                                                if (parts[0] == "AK")
                                                {
                                                    uavtKodu = parts[1];
                                                    if (!String.IsNullOrEmpty(uavtKodu))
                                                    {
                                                        policeBilgileri.adres = this.GetAdresDetay(Convert.ToInt64(uavtKodu));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (polmasChild[j].Name == "PSRCVP")
                                    {
                                        XmlNode plmsItem = polmasChild.Item(j);
                                        if (plmsItem["QUESTION_CODE"].InnerText == "4811")
                                            policeBilgileri.Aciklama = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "19910")
                                            policeBilgileri.DigerSirketYenilemesiMi = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1135")
                                            policeBilgileri.BinaninToplamKatSayisi = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1999")
                                            policeBilgileri.ReferansPoliceNumarasi = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1993")
                                            policeBilgileri.DainiMurteinKrediBitisTar = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1996")
                                            policeBilgileri.DainiMurteinBankaKodu = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1134")
                                            policeBilgileri.DaireM2 = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1132")
                                            policeBilgileri.BinaInsaTarzi = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1133")
                                            policeBilgileri.BinaInsaYili = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "13")
                                            policeBilgileri.DainiMurteinAdi = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1992")
                                            policeBilgileri.DainiMurteinKrediDovizCinsi = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1995")
                                            policeBilgileri.DainiMurteinFinansKurumu = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1990")
                                            policeBilgileri.DainiMurteinVarMi = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1991")
                                            policeBilgileri.DainiMurteinKrediTutari = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1997")
                                            policeBilgileri.DainiMurteinSubeKodu = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1131")
                                            policeBilgileri.DaireKullanimSekli = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1994")
                                            policeBilgileri.DainiMurteinKrediSozlesmeNo = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1118")
                                            policeBilgileri.SigortaEttirenSifati = plmsItem["ANSWER"].InnerText;

                                        if (plmsItem["QUESTION_CODE"].InnerText == "1136")
                                            policeBilgileri.HasarliMi = plmsItem["ANSWER"].InnerText;

                                    }
                                    else if (elm.Name == "Error")
                                    {
                                        policeBilgileri.Hata = elm["ErrDesc"].InnerText;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex.Message);
            }
            return policeBilgileri;
        }

    }
}
