using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.ergoturkiye.police.uretim;
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

namespace Neosinerji.BABOnlineTP.Business.ERGO.Trafik
{
    public class ERGOTrafik : Teklif, IERGOTrafik
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
        public ERGOTrafik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
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
            ExternalPolicyService clnt = new ExternalPolicyService();
            try
            {
                #region Veri Hazırlama

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleERGOService);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ERGO });
                clnt.Url = konfig[Konfig.ERGO_KaskoServiceURL]; //Trafik geldiğinde değişecek
                clnt.Timeout = 150000;
                clnt.Credentials = new System.Net.NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre);//yenisini bu şekilde yolluyorum soapta da header 
                string teklifPDFURL = String.Empty;
                ////WEB Service e username token ekleniyor
                UsernameToken token = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
                clnt.RequestSoapContext.Security.Tokens.Add(token);
                clnt.RequestSoapContext.Security.MustUnderstand = false;
                clnt.RequestSoapContext.Security.Actor = "";

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

                #endregion

                #region REQUEST

                proposalWsDto request = new proposalWsDto();
                List<benefitWsDto> blist = new List<benefitWsDto>();
                List<customerWsDto> clist = new List<customerWsDto>();
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                #region Acente Bilgileri

                request.agencyNumber = Convert.ToInt32(servisKullanici.PartajNo_);
                request.agencySellerNumber = Convert.ToInt32(servisKullanici.SubAgencyCode);
                request.agencySubNumber = Convert.ToInt32(servisKullanici.SubAgencyCode);
                //request.authorizationCode = "";
                #endregion

                #region Property Value Reading

                string MarkaModel = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                var ErgoAracTipValue = clnt.retrievePropertyValue("@aracTip", MarkaModel);
                int ErgoAracTipValueId = 0;
                if (ErgoAracTipValue != null)
                {
                    ErgoAracTipValueId = ErgoAracTipValue.propertyValueId;
                }
                clnt.Dispose();

                var ErgoModelYiliValue = clnt.retrievePropertyValue("@aracModelYili", teklif.Arac.Model.Value.ToString());
                int ErgoModelYiliValueId = 0;
                if (ErgoModelYiliValue != null)
                {
                    ErgoModelYiliValueId = ErgoModelYiliValue.propertyValueId;
                }
                clnt.Dispose();

                var ErgoTescilYeriValue = clnt.retrievePropertyValue("@tescilYeri", "34");//Ekrandan seçilen gönderilecek
                int ErgoTescilYeriValueId = 0;
                if (ErgoTescilYeriValue != null)
                {
                    ErgoTescilYeriValueId = ErgoTescilYeriValue.propertyValueId;
                }
                clnt.Dispose();

                var ErgoPlakaKoduValue = clnt.retrievePropertyValue("@plaka1", teklif.Arac.PlakaKodu);
                int ErgoPlakaKoduValueId = 0;
                if (ErgoPlakaKoduValue != null)
                {
                    ErgoPlakaKoduValueId = ErgoPlakaKoduValue.propertyValueId;
                }
                clnt.Dispose();

                #endregion

                #region Tarih Bilgileri

                request.beginDate = polBaslangic;
                request.endDate = polBaslangic.AddYears(1);
                request.issueDate = polBaslangic;

                #endregion

                #region Menfaat Bilgileri

                benefitWsDto benefit = new benefitWsDto
                {
                    benefitCode = ERGOTrafikSorular.HukuksalKorumaSurucu,
                    suminsured = 0

                };
                blist.Add(benefit);

                benefit = new benefitWsDto
                {
                    benefitCode = ERGOTrafikSorular.HukuksalKorumaArac,
                    suminsured = 0

                };
                blist.Add(benefit);

                request.benefitList = blist.ToArray();

                #endregion

                #region Müşteri Bilgileri

                customerWsDto customer = new customerWsDto
                {
                    addressText="",
                    customerName = sigortali.AdiUnvan,
                    customerSurname = sigortali.SoyadiUnvan,
                    customerTypeId = ERGOMusteriTipleri.Sigortali,
                    identityNo = sigortali.KimlikNo,
                    identityTypeId = sigortaliKimlikType,
                    isLocal=true,
                    isPerson=true
                };
                clist.Add(customer);

                customer = new customerWsDto
                {
                    addressText = "",
                    customerName = MusteriAyniMi ? sigortali.AdiUnvan : sigEttirenGenelBilgi.AdiUnvan,
                    customerSurname = MusteriAyniMi ? sigortali.SoyadiUnvan : sigEttirenGenelBilgi.SoyadiUnvan,
                    customerTypeId = ERGOMusteriTipleri.SigortaEttiren,
                    identityNo = MusteriAyniMi ? sigortali.KimlikNo : sigEttirenGenelBilgi.KimlikNo,
                    identityTypeId = sigortaEttirenKimlikType,
                    isLocal = true,
                    isPerson = true

                };
                clist.Add(customer);

                request.customerList = clist.ToArray();

                #endregion

                #region Dain-i Murtein Bilgileri

                //request.lossPayee = new lossPayeeWsDto();

                #endregion

                #region Sorular

                string rakam = "";
                string harf = "";
                foreach (char ch in teklif.Arac.PlakaNo)
                {
                    if (Char.IsDigit(ch))
                        rakam += ch;
                    if (Char.IsLetter(ch))
                        harf += ch;
                }

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

                List<pPropertyWsDto> plist = new List<pPropertyWsDto>();
                pPropertyWsDto prop = new pPropertyWsDto
                {                    
                    propertyCode = ERGOTrafikSorular.AracKullanimTarzi,
                    propertyValueId = Convert.ToInt32(AnaKullanimTarzi)

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracAltKullanimTarzi,
                    propertyValueId = Convert.ToInt32(AltKullanimTarzi)

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracTip,
                    propertyValueId = ErgoAracTipValueId,
                    propertyValueStr = ""

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracModelYili,
                    propertyValueId = ErgoModelYiliValueId,
                    propertyValueStr = ""

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracBirlikKod,
                    propertyValueStr = MarkaModel

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracKoltukAdet,
                    propertyValueStr = teklif.Arac.KoltukSayisi.HasValue ? (teklif.Arac.KoltukSayisi.Value - 1).ToString() : "4"

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.Plaka1,
                    propertyValueId = ErgoPlakaKoduValueId
                    //propertyValueStr = teklif.Arac.PlakaKodu.PadLeft(3, '0')

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.Plaka2,
                    propertyValueStr = harf

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.Plaka3,
                    propertyValueStr = rakam

                };
                plist.Add(prop);

                //prop = new pPropertyWsDto
                //{
                //    propertyCode = ERGOKaskoPropertyList.aracTescilBelgeSeriNo1,
                //    propertyValueStr = !String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) ? teklif.Arac.TescilSeriKod : ""

                //};
                //plist.Add(prop);

                //prop = new pPropertyWsDto
                //{
                //    propertyCode = ERGOKaskoPropertyList.aracTescilBelgeSeriNo2,
                //    propertyValueStr = !String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) ? teklif.Arac.TescilSeriNo : teklif.Arac.AsbisNo

                //};
                //plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracMotorNo,
                    propertyValueStr = teklif.Arac.MotorNo

                };
                plist.Add(prop);             

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracSasiNo,
                    propertyValueStr = teklif.Arac.SasiNo

                };
                plist.Add(prop);               

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.AracTescilTarihi,
                    propertyValueStr = teklif.Arac.TrafikTescilTarihi.Value.ToString("dd/MM/yyyy").Replace('.', '/')

                };
                plist.Add(prop);

                prop = new pPropertyWsDto
                {
                    propertyCode = ERGOTrafikSorular.TescilYeri,
                    propertyValueId = ErgoTescilYeriValueId,
                    propertyValueStr = "ADALAR"//teklif.Arac.TescilIlceKodu

                };
                plist.Add(prop);

                #region Önceki Poliçe
                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                string oncekiAcenteNo = String.Empty;
                string oncekiPoliceNo = String.Empty;
                string oncekiSirketKodu = String.Empty;
                string oncekiYenilemeNo = String.Empty;

                if (eskiPoliceVar)
                {
                    oncekiAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                    oncekiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                    oncekiSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    oncekiYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);

                    prop = new pPropertyWsDto
                    {
                        propertyCode = ERGOTrafikSorular.TrafikReferansAcenteNo,
                        propertyValueStr = oncekiAcenteNo

                    };
                    plist.Add(prop);

                    prop = new pPropertyWsDto
                    {
                        propertyCode = ERGOTrafikSorular.TrafikReferansPoliceNo,
                        propertyValueStr = oncekiPoliceNo

                    };
                    plist.Add(prop);

                    prop = new pPropertyWsDto
                    {
                        propertyCode = ERGOTrafikSorular.TrafikReferansPoliceYenilemeNo,
                        propertyValueStr = oncekiYenilemeNo

                    };
                    plist.Add(prop);

                    var ErgoOncekiSigortaSirketiValue = clnt.retrievePropertyValue("@trafikReferansSirketKodu", oncekiSirketKodu);
                    int ErgoOncekiSigortaSirketiValueId = 0;
                    if (ErgoOncekiSigortaSirketiValue != null)
                    {
                        ErgoOncekiSigortaSirketiValueId = ErgoOncekiSigortaSirketiValue.propertyValueId;
                    }
                    clnt.Dispose();

                    prop = new pPropertyWsDto
                    {
                        propertyCode = ERGOTrafikSorular.TrafikReferansSirketKodu,
                        propertyValueId = ErgoOncekiSigortaSirketiValueId,

                    };
                    plist.Add(prop);
                }
                
                #endregion                
                #endregion

                #region Genel Bilgileri

                request.currencyCode = "TL";
                request.pPropertyList = plist.ToArray();
               request.paymentPlanId = 1;
               request.paymentTypeId = 1;
                request.productNumber = ERGOUrunKodlari.Trafik;
                //request.profileIdList = null;

                #endregion

                #endregion

                #region Service Call
                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);

                var response = clnt.saveProposal(request);
                clnt.Dispose();
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

                    if (item.policyNo > 0 || item.policyNo != null)
                    {
                        this.EndLog(response, true, response.GetType());
                        UsernameToken tokenPDF = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
                        clnt.RequestSoapContext.Security.Tokens.Add(tokenPDF);
                        clnt.RequestSoapContext.Security.MustUnderstand = false;
                        clnt.RequestSoapContext.Security.Actor = "";

                        endorsementKeyWsDto requestPDF = new endorsementKeyWsDto();
                        requestPDF.endorsementNo = item.endorsementNo;
                        requestPDF.policyNo = item.policyNo;
                        requestPDF.renewalNo = item.renewalNo;

                        printPolicyWsDto[] pdfResponse = clnt.printPolicy(requestPDF);
                        clnt.Dispose();
                        if (pdfResponse != null)
                        {
                            foreach (var itemPDF in pdfResponse)
                            {
                                byte[] data = itemPDF.bytes;
                                if (itemPDF.printType == printTypeWsDto.PRINT)
                                {
                                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                                    string fileName = String.Format("ERGO_Trafik_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                                    teklifPDFURL = storage.UploadFile("trafik", fileName, data);
                                }
                            }
                        }
                    }
                    this.GenelBilgiler.PDFDosyasi = teklifPDFURL;
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

                    #region ERGO Teminatlar

                    var teminatlar = item.benefitCoverageList;
                    if (teminatlar != null)
                    {
                        decimal sakatlanmaOlumKisiBasiPrim = teminatlar.Where(s => s.benefit.benefitId == 2050).Select(s => s.premium).FirstOrDefault();
                        decimal sakatlanmaOlumKisiBasiToplamTutar = teminatlar.Where(s => s.benefit.benefitId == 1032).Select(s => s.suminsured).FirstOrDefault();

                        decimal sakatlanmaOlumKazaBasiPrim = teminatlar.Where(s => s.benefit.benefitId == 2051).Select(s => s.premium).FirstOrDefault();
                        decimal sakatlanmaOlumKazaBasiToplamTutar = teminatlar.Where(s => s.benefit.benefitId == 2051).Select(s => s.suminsured).FirstOrDefault();

                        decimal maddiKazaBasiPrim = teminatlar.Where(s => s.benefit.benefitId == 2052).Select(s => s.premium).FirstOrDefault();
                        decimal maddiKazaBasiToplamTutar = teminatlar.Where(s => s.benefit.benefitId == 2052).Select(s => s.suminsured).FirstOrDefault();

                        decimal maddiAracBasiPrim = teminatlar.Where(s => s.benefit.benefitId == 2053).Select(s => s.premium).FirstOrDefault();
                        decimal maddiAracBasiToplamTutar = teminatlar.Where(s => s.benefit.benefitId == 2053).Select(s => s.suminsured).FirstOrDefault();

                        decimal saglikKazaBasiPrim = teminatlar.Where(s => s.benefit.benefitId == 2055).Select(s => s.premium).FirstOrDefault();
                        decimal saglikKazaBasiToplamTutar = teminatlar.Where(s => s.benefit.benefitId == 2055).Select(s => s.suminsured).FirstOrDefault();

                        decimal saglikKisiBasiPrim = teminatlar.Where(s => s.benefit.benefitId == 2054).Select(s => s.premium).FirstOrDefault();
                        decimal saglikKisiBasiToplamTutar = teminatlar.Where(s => s.benefit.benefitId == 2054).Select(s => s.suminsured).FirstOrDefault();

                        this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, sakatlanmaOlumKisiBasiToplamTutar, 0, 0, sakatlanmaOlumKisiBasiPrim, 0);
                        this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, sakatlanmaOlumKazaBasiToplamTutar, 0, 0, sakatlanmaOlumKazaBasiPrim, 0);
                        this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, maddiKazaBasiToplamTutar, 0, 0, maddiKazaBasiPrim, 0);
                        this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, maddiAracBasiToplamTutar, 0, 0, maddiAracBasiPrim, 0);
                        this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, saglikKazaBasiToplamTutar, 0, 0, saglikKazaBasiPrim, 0);
                        this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, saglikKisiBasiToplamTutar, 0, 0, saglikKisiBasiPrim, 0);

                    }
                    #endregion

                    this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                    this.GenelBilgiler.TaksitSayisi = Convert.ToByte(item.paymentPlan.installmentCount);
                    this.GenelBilgiler.ZKYTMSYüzdesi = 0;

                    ////// ==== Güncellenicek. ==== //
                    ////this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                    ////this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                    ////Odeme Bilgileri
                    //this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                    //this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;

                    //this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                    // this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                    // this.GenelBilgiler.HasarSurprimYuzdesi = 60;

                    #region Ödeme Planı
                    if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                    {
                        this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                    }
                    #endregion
                }

                #endregion

                #region Hasarsılık indirim oranı set ediliyor

                //#region Teminatlar

                //var TedaviKisiBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KisiBasinaTedaviMasraflari);
                //decimal TeminatBedeli = 0;
                //decimal BrutPrim = 0;
                //if (TedaviKisiBasina != null)
                //{
                //    TeminatBedeli = TedaviKisiBasina.COVER_AMOUNT.HasValue ? TedaviKisiBasina.COVER_AMOUNT.Value : 0;
                //    BrutPrim = TedaviKisiBasina.GROSS_PREMIUM.HasValue ? TedaviKisiBasina.GROSS_PREMIUM.Value : 0;
                // this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                //}

                //var TedaviKazaBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KazaBasinaTedaviMasraflari);
                //TeminatBedeli = 0;
                //BrutPrim = 0;
                //if (TedaviKazaBasina != null)
                //{
                //    TeminatBedeli = TedaviKazaBasina.COVER_AMOUNT.HasValue ? TedaviKazaBasina.COVER_AMOUNT.Value : 0;
                //    BrutPrim = TedaviKazaBasina.GROSS_PREMIUM.HasValue ? TedaviKazaBasina.GROSS_PREMIUM.Value : 0;
                //    this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                //}

                //var OlumSakatlikKisiBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KisiBasiSakatOlum);
                //TeminatBedeli = 0;
                //BrutPrim = 0;
                //if (OlumSakatlikKisiBasina != null)
                //{
                //    TeminatBedeli = OlumSakatlikKisiBasina.COVER_AMOUNT.HasValue ? OlumSakatlikKisiBasina.COVER_AMOUNT.Value : 0;
                //    BrutPrim = OlumSakatlikKisiBasina.GROSS_PREMIUM.HasValue ? OlumSakatlikKisiBasina.GROSS_PREMIUM.Value : 0;
                //    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                //}

                //var OlumSakatlikKazaBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KazaBasiSakatOlum);
                //TeminatBedeli = 0;
                //BrutPrim = 0;
                //if (OlumSakatlikKazaBasina != null)
                //{
                //    TeminatBedeli = OlumSakatlikKazaBasina.COVER_AMOUNT.HasValue ? OlumSakatlikKazaBasina.COVER_AMOUNT.Value : 0;
                //    BrutPrim = OlumSakatlikKazaBasina.GROSS_PREMIUM.HasValue ? OlumSakatlikKazaBasina.GROSS_PREMIUM.Value : 0;
                //    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                //}

                //var MaddiAracBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.AracBasinaMaddi);
                //TeminatBedeli = 0;
                //BrutPrim = 0;
                //if (MaddiAracBasina != null)
                //{
                //    TeminatBedeli = MaddiAracBasina.COVER_AMOUNT.HasValue ? MaddiAracBasina.COVER_AMOUNT.Value : 0;
                //    BrutPrim = MaddiAracBasina.GROSS_PREMIUM.HasValue ? MaddiAracBasina.GROSS_PREMIUM.Value : 0;
                //    this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                //}

                //this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kisi_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kaza_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kaza_Basina, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.Asistans, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Tedavi, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Ölüm_Sakatlık, 0, 0, 0, 0, 0);
                //this.AddTeminat(TrafikTeminatlar.Trafik, 0, 0, 0, 0, 0);
                //#endregion

                //#region Web servis cevapları
                //this.AddWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Teklif_Police_No, result.PROPOSAL_NO.ToString());
                //this.AddWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Trafik_Session_No, result.RESPONSE_MESSAGE_NO.ToString());
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
            ExternalPolicyService clnt = new ExternalPolicyService();
            try
            {
                #region Veri Hazırlama GENEL
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.ERGO });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                #endregion

                #region Policelestirme Request

                clnt.Url = konfig[Konfig.ERGO_KaskoServiceURL];
                clnt.Timeout = 150000;
                clnt.Credentials = new System.Net.NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre);
                //WEB Service e username token ekleniyor
                UsernameToken token = new UsernameToken(servisKullanici.KullaniciAdi, servisKullanici.Sifre, PasswordOption.SendPlainText);
                clnt.RequestSoapContext.Security.Tokens.Add(token);
                clnt.RequestSoapContext.Security.MustUnderstand = false;
                clnt.RequestSoapContext.Security.Actor = "";

                int proposalNo = Convert.ToInt32(teklif.GenelBilgiler.TUMTeklifNo);

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
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
                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
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

                policyWsDto response = clnt.savePolicy(proposalNo, krediKarti);
                clnt.Dispose();

                this.EndLog(response, true, response.GetType());

                if (response.premium.totalPremium > 0)
                {
                    endorsementKeyWsDto printRequest = new endorsementKeyWsDto()
                    {
                        endorsementNo = response.endorsementNo,
                        policyNo = response.policyNo,
                        renewalNo = response.renewalNo
                    };

                    this.BeginLog(printRequest, printRequest.GetType(), WebServisIstekTipleri.Police);

                    var responsePDF = clnt.printPolicy(printRequest);
                    clnt.Dispose();
                    this.EndLog(responsePDF, true, responsePDF.GetType());
                    if (responsePDF != null)
                    {
                        foreach (var item in responsePDF)
                        {
                            byte[] data = item.bytes;
                            if (item.printType == printTypeWsDto.PRINT)
                            {
                                IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                                string fileName = String.Format("ERGO_Trafik_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                                string url = storage.UploadFile("trafik", fileName, data);
                                this.GenelBilgiler.PDFPolice = url;
                                _Log.Info("Police_PDF url: {0}", url);
                            }
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
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }
    }
}
