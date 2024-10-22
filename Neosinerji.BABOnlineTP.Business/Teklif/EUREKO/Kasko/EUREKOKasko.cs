using EurekoSigorta_Business.EUREKO;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.eurekosigorta.musteriV2;
using Neosinerji.BABOnlineTP.Business.eurekosigorta.kaskoV3;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using EurekoSigorta_Business.Common.EUREKO;

namespace Neosinerji.BABOnlineTP.Business.EUREKO.Kasko
{
    public class EUREKOKasko : Teklif, IEUREKOKasko
    {
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        ICRService _CRService;
        IAktifKullaniciService _AktifkullaniciService;
        ITVMService _TVMService; 

        [InjectionConstructor]
        public EUREKOKasko(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ICRService cRService, IAktifKullaniciService aktifkullaniciService,  ITVMService TVMService )
            : base()
        {
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _CRService = cRService;
            _AktifkullaniciService = aktifkullaniciService;
            _TVMService = TVMService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.EUREKO;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            eurekosigorta.kaskoV3.AutoInsPolicyCreateV3Service kaskoClient = new eurekosigorta.kaskoV3.AutoInsPolicyCreateV3Service();

            try
            {
                #region Veri Hazırlama GENEL

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEurekoKasko);
                KonfigTable konfigPlatformType = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPlatformType);
                KonfigTable konfigTeklifPoliceBilgi = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOTeklifPoliceBilgi);
                KonfigTable konfigTeminat = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPolieTeminat);
                                
                kaskoClient.Url = konfig[Konfig.EUREKO_KaskoServiceURL];
                kaskoClient.Timeout = 150000;

                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.EUREKO });

                eurekosigorta.kaskoV3.ExecuteRequest request = new eurekosigorta.kaskoV3.ExecuteRequest();
                eurekosigorta.kaskoV3.ExecuteResponseResponse response = new eurekosigorta.kaskoV3.ExecuteResponseResponse();

                #endregion

                #region InHeader

                var header = new eurekosigorta.kaskoV3.ExecuteRequestInHeader();
                header.CompanyId = servisKullanici.CompanyId;
                header.UserId = servisKullanici.KullaniciAdi;
                header.Password = servisKullanici.Sifre;
                header.PlatformType = (eurekosigorta.kaskoV3.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eurekosigorta.kaskoV3.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                header.MessageId = "100";
                request.InHeader = header;

                #endregion

                #region InGlobalInfo

                var inGlobalInfo = new eurekosigorta.kaskoV3.ExecuteRequestInGlobalInfo();

                //Teklif için "PROPOSAL", poliçe için "POLICY" girilmelidir
                inGlobalInfo.OperationCode = "PROPOSAL";
                inGlobalInfo.RaAndAgencySameFlag = "H";
                bool dainiMurtein = teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
                if (dainiMurtein)
                {
                    string kimlikNo = teklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, string.Empty);
                    if (!string.IsNullOrEmpty(kimlikNo))
                    {
                        var policyLossPayee = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyLossPayee();
                        policyLossPayee.LossPayeeAmount = 10;
                        policyLossPayee.LossPayeeContractNum = "1";
                        policyLossPayee.LossPayeeCurrencyCode = "TL";
                        policyLossPayee.LossPayeeEndDate = teklif.GenelBilgiler.BitisTarihi.ToString("yyyyMMdd");
                        policyLossPayee.LossPayeeId = kimlikNo;
                        policyLossPayee.LossPayeeRate = 1;
                        policyLossPayee.LossPayeeStartDate = teklif.GenelBilgiler.BaslamaTarihi.ToString("yyyyMMdd");
                        request.InPolicyLossPayee = policyLossPayee;
                    }
                }

                request.InGlobalInfo = inGlobalInfo;
                #endregion

                #region InPolicyMaster

                var inPolicyMaster = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyMaster();
                inPolicyMaster.CompanyCode = "GS";
                inPolicyMaster.AgencyCode = servisKullanici.PartajNo_;
                inPolicyMaster.SubAgencyCode = servisKullanici.SubAgencyCode;
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                inPolicyMaster.PolicyStartDate = polBaslangic.ToString("yyyyMMdd");
                inPolicyMaster.PolicyEndDate = polBaslangic.AddYears(1).ToString("yyyyMMdd");
                inPolicyMaster.ProcessingDate = DateTime.Now.ToString("yyyyMMdd");
                inPolicyMaster.CurrencyCode = "TL";
                inPolicyMaster.ProductCode = EUREKO_Urunkodlari.KASKO;
                inPolicyMaster.TariffCode = "10";
                inPolicyMaster.PaymentType = "";
                inPolicyMaster.SourceId = servisKullanici.SourceId;
                inPolicyMaster.PolicyGroupNum = "0";
                inPolicyMaster.PolicyNum = "0";
                inPolicyMaster.RenewalNum = "0";
                inPolicyMaster.EndorsementNum = "0";
                inPolicyMaster.InternalEndorsementNum = "0";
                inPolicyMaster.ChangeSequenceNum = "0";
                request.InPolicyMaster = inPolicyMaster;
                #endregion

                #region InGroupPolicyParty

                List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRow> gruprow = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRow>();
                List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty> gruplar = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty>();

                eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty grup = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();

                TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
                TeklifSigortali sigortali = teklif.Sigortalilar.FirstOrDefault(f => f.TeklifId == teklif.GenelBilgiler.TeklifId);

                bool MusteriAyniMi = false;
                string EurekoMusteriNo = String.Empty; //Sigortali Sigorta Ettiren Aynı
                string EurekoSEMusteriNo = String.Empty; //Sigorta Ettiren Müsşteri Numarası
                MusteriGenelBilgiler musteriBilgileri = _MusteriService.GetMusteri(sigortali.MusteriKodu);
                MusteriTelefon musteriTelefon = musteriBilgileri.MusteriTelefons.FirstOrDefault(f => f.MusteriKodu == sigortali.MusteriKodu);
                MusteriAdre musteriAdres = musteriBilgileri.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                var EurekoAdres = _CRContext.CR_IlIlceRepository.Find(s => s.IlKodu == musteriAdres.IlKodu && s.IlceKodu == musteriAdres.IlceKodu && s.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                if (sigortaEttiren.MusteriKodu == sigortali.MusteriKodu)
                {
                    EurekoMusteriRequestModel req = new EurekoMusteriRequestModel()
                    {
                        KimlikNo = sigortali.MusteriGenelBilgiler.KimlikNo,
                        IlKodu = EurekoAdres != null ? EurekoAdres.CRIlKodu : "34",
                        IlceKodu = EurekoAdres != null ? EurekoAdres.CRIlceKodu.ToString() : "25",
                        Adres = musteriAdres.Adres,
                        Email = musteriBilgileri.EMail,
                        TVMKodu = teklif.GenelBilgiler.TVMKodu
                    };

                    EurekoMusteriResponseModel model = this.EurekoMusteriKaydet(req);
                    if (model != null) { EurekoMusteriNo = model.MusteriNo; }
                    MusteriAyniMi = true;
                }
                else
                {
                    MusteriAyniMi = false;
                    EurekoMusteriRequestModel req = new EurekoMusteriRequestModel()
                    {
                        KimlikNo = sigortali.MusteriGenelBilgiler.KimlikNo,
                        IlKodu = EurekoAdres != null ? EurekoAdres.CRIlKodu : "34",
                        IlceKodu = EurekoAdres != null ? EurekoAdres.CRIlceKodu.ToString() : "25",
                        Adres = musteriAdres.Adres,
                        Email = musteriBilgileri.EMail,
                        TVMKodu = teklif.GenelBilgiler.TVMKodu
                    };
                    EurekoMusteriResponseModel model = this.EurekoMusteriKaydet(req);
                    EurekoMusteriNo = model != null ? model.MusteriNo : "0";


                    musteriBilgileri = new MusteriGenelBilgiler();
                    musteriBilgileri = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
                    musteriTelefon = new MusteriTelefon();
                    musteriTelefon = musteriBilgileri.MusteriTelefons.FirstOrDefault(f => f.MusteriKodu == sigortaEttiren.MusteriKodu);
                    musteriAdres = new MusteriAdre();
                    musteriAdres = musteriBilgileri.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                    req = new EurekoMusteriRequestModel()
                    {
                        KimlikNo = sigortaEttiren.MusteriGenelBilgiler.KimlikNo,
                        IlKodu = EurekoAdres != null ? EurekoAdres.CRIlKodu : "34",
                        IlceKodu = EurekoAdres != null ? EurekoAdres.CRIlceKodu.ToString() : "25",
                        Adres = musteriAdres.Adres,
                        Email = musteriBilgileri.EMail,
                        TVMKodu = teklif.GenelBilgiler.TVMKodu
                    };

                    EurekoMusteriResponseModel modelSE = this.EurekoMusteriKaydet(req);
                    EurekoSEMusteriNo = modelSE != null ? modelSE.MusteriNo : "0";

                    //EurekoMusteriNo = this.MusteriKaydet(teklif, teklif.GenelBilgiler.TVMKodu, sigortali, null);
                    //EurekoSEMusteriNo = this.MusteriKaydet(teklif, teklif.GenelBilgiler.TVMKodu, null, sigortaEttiren);
                }
                //  Müşteri taraf bilgisidir. Sigortalı -"SG" , Sigorta Ettiren "SE", Prim Ödeyen-"PE" olarak girilmelidir.
                grup.PartyType = "SG";
                grup.PartyId =  EurekoMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 1; //Oran bilgisidir. "1" gönderilmelidir.
                gruplar.Add(grup);

                grup = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "SE";
                grup.PartyId = MusteriAyniMi ? EurekoMusteriNo : EurekoSEMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 0; //Oran bilgisidir. "0" gönderilmelidir.
                gruplar.Add(grup);

                grup = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "PO";
                grup.PartyId =MusteriAyniMi ? EurekoMusteriNo : EurekoSEMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 1; //Oran bilgisidir. "1" gönderilmelidir.
                gruplar.Add(grup);

                eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRow row;
                foreach (var item in gruplar)
                {
                    row = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRow();
                    row.InGListPolicyParty = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                    row.InGListPolicyParty.PartyType = item.PartyType;
                    row.InGListPolicyParty.PartyId = item.PartyId;
                    row.InGListPolicyParty.PartyRate = item.PartyRate;

                    gruprow.Add(row);
                }
                request.InGroupPolicyParty = gruprow.ToArray();

                #endregion

                #region InPolicyPaymentInstallment

                var installment = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyPaymentInstallment();
                installment.PartyId = "0";
                installment.FirstBankCode = "0";
                installment.FirstBranchCode = "0";
                request.InPolicyPaymentInstallment = installment;

                #endregion

                #region InRiskQuestionValues

                var rq = new eurekosigorta.kaskoV3.ExecuteRequestInRiskQuestionValues();

                #region  Vehicle

                rq.SeatNum = (teklif.Arac.KoltukSayisi ?? 0).ToString();
                //var sekil = Convert.ToInt32(teklif.Arac.KullanimSekli);
                //var eurekoKullanimSekli = _AracContext.EurekoAracKullanimTarziRepository.All().Where(s => s.Id == sekil).FirstOrDefault();
                var sekilParts = teklif.Arac.KullanimTarzi.Split('-');
                var sekilKod = sekilParts[0];
                var sekilKod2 = sekilParts[1];

                var EurkeoAracKullanimTarzi = _CRContext.CR_KullanimTarziRepository.Find(s => s.KullanimTarziKodu == sekilKod && s.Kod2 == sekilKod2 && s.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                var EurkeoAracKullanimTarziParts = EurkeoAracKullanimTarzi.TarifeKodu.Split('-');


                if (EurkeoAracKullanimTarziParts != null) rq.VehicleUsage = EurkeoAracKullanimTarziParts[1];
                else rq.VehicleUsage = "A";
                //if (EurkeoAracKullanimTarziParts != null) rq.CommercialUsageType = EurkeoAracKullanimTarziParts[0].ToString();
                //else
                rq.CommercialUsageType = string.Empty;
                rq.VehiclePrice = (teklif.Arac.AracDeger ?? 0);
                rq.ManufactureYear = (teklif.Arac.Model ?? 0).ToString();
                if (string.IsNullOrEmpty(teklif.Arac.MotorNo))
                    rq.VehicleEngineNum = ("MOTOR" + teklif.GenelBilgiler.TVMKodu.ToString() + teklif.GenelBilgiler.TeklifNo.ToString()).PadLeft(10, '0');
                else
                    rq.VehicleEngineNum = teklif.Arac.MotorNo;
                if (string.IsNullOrEmpty(teklif.Arac.SasiNo))
                    rq.VehilceChassisNum = ("SASI" + teklif.GenelBilgiler.TVMKodu.ToString() + teklif.GenelBilgiler.TeklifNo.ToString()).PadLeft(17, '0');
                else
                    rq.VehilceChassisNum = teklif.Arac.SasiNo;
                rq.NoClaimYear = "0";
                rq.PlateCity = teklif.Arac.PlakaKodu;
                rq.PlateDetail = teklif.Arac.PlakaNo;
                bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
                if (eskiPoliceVar)
                {
                    rq.InsurersFirstPolicy = "H";
                    rq.PrevInsCompany = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, string.Empty);
                    rq.PrevPolicyNum = teklif.ReadSoru(TrafikSorular.Eski_Police_No, string.Empty);
                    rq.PrevAgenyCode = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, string.Empty);
                    rq.PrevRenewalNum = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, string.Empty);
                }
                else
                {
                    rq.InsurersFirstPolicy = "E";
                    rq.NoClaimYear = "0";
                }
                rq.VehicleValueCode = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                rq.CommercialCoefficient = 100;
                rq.LossPayeeExist = "H";
                rq.EarthquakeFloodDeduct = "H";
                rq.AbroadPeriod = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "6");
                rq.VdfAdditionalRisk = "H";
                rq.VehicleLicenceCode = teklif.Arac.TescilSeriKod;
                rq.VehicleLicenceNum = teklif.Arac.TescilSeriNo;
                if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                    rq.NotaryReferenceNum = teklif.Arac.AsbisNo;
                if (request.InPolicyLossPayee != null) rq.LossPayeeExist = "E";
                else rq.LossPayeeExist = "H";

                #endregion

                #region IMM FK

                //   bool immDahil = teklif.ReadSoru(KaskoSorular.IMMManeviDahil, false);
                string imm = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, "0");
                string fk = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, "0");
                string eurekoImmKodu = "2B";
                string eurekoFkKodu = "19";

                //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(imm), sekilKod, sekilKod2);
                CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();
                if (IMMBedel != null)
                {
                    //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                    CRKademeNo = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.EUREKO, IMMBedel.BedeniSahis, IMMBedel.Kombine, sekilKod, sekilKod2);
                    if (CRKademeNo != null)
                    {
                        if (IMMBedel.BedeniSahis > 0)
                        {
                            var parts = CRKademeNo.Kademe.Split('-');
                            //Ekrandan seçilen değer manevi dahil olarak seçildiyse Eureko Sigortaya Manevi Dahil Kodu gönderiliyor.
                            if(IMMBedel.Text.Contains("Manevi Dahil"))
                                eurekoImmKodu = parts[0]; // Manevi Dahil Kod
                            else
                                eurekoImmKodu = parts[1];
                        }
                    }
                }

                CR_KaskoFK CRKademeNu = new CR_KaskoFK();
                var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fk), sekilKod, sekilKod2);

                if (FKBedel != null)
                {
                    CRKademeNu = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.EUREKO, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, sekilKod, sekilKod2);
                    if (CRKademeNu != null)
                    {
                        eurekoFkKodu = CRKademeNu.Kademe;
                    }

                }

                rq.ImmLevel = eurekoImmKodu;
                rq.FkkLevel = eurekoFkKodu;


                #endregion

                #region Akesuar

                var sesgoruntu = teklif.ReadSoru(KaskoSorular.Teminat_Ekstra_Aksesuar_VarYok, false);
                List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR).ToList<TeklifAracEkSoru>();
                var Aksesuarlar = new List<MapfreAksesuarModel>();
                if (Aksesuarlar != null && aksesuarlar.Count > 0)
                {
                    List<CR_AracEkSoru> eksorular = _CRService.GetAracEkSoru(TeklifUretimMerkezleri.EUREKO, MapfreKaskoEkSoruTipleri.AKSESUAR);
                    foreach (var item in aksesuarlar)
                    {
                        var soru = eksorular.FirstOrDefault(f => f.SoruKodu == item.SoruKodu);
                        if (soru != null)
                        {
                            Aksesuarlar.Add(new MapfreAksesuarModel()
                            {
                                AksesuarTip = soru.SoruAdi,
                                Aciklama = item.Aciklama,
                                Bedel = (int)item.Bedel
                            });
                        }
                    }
                }
                int aksesuarBedel = 0;
                foreach (var item in Aksesuarlar)
                {
                    aksesuarBedel += item.Bedel;
                }
                var ElektronikCihaz_Teminati = teklif.ReadSoru(KaskoSorular.Teminat_ElektronikCihaz_VarYok, false);
                List<TeklifAracEkSoru> elekCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ).ToList<TeklifAracEkSoru>();
                var Cihazlar = new List<MapfreAksesuarModel>();
                bool lpgMi = false;
                int lpgBedel = 0;
                if (elekCihazlar != null && elekCihazlar.Count > 0)
                {
                    List<CR_AracEkSoru> cihazlar = _CRService.GetAracEkSoru(TeklifUretimMerkezleri.EUREKO, MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ);
                    foreach (var item in elekCihazlar)
                    {
                        var soru = cihazlar.FirstOrDefault(f => f.SoruKodu == item.SoruKodu);
                        if (soru != null)
                        {
                            if (soru.SoruKodu == "5")
                            {
                                lpgMi = true;
                                lpgBedel = (int)item.Bedel;
                            }
                            Cihazlar.Add(new MapfreAksesuarModel()
                            {
                                AksesuarTip = soru.SoruAdi,
                                Aciklama = item.Aciklama,
                                Bedel = (int)item.Bedel

                            });
                        }
                    }
                }
                int elektronikBedel = 0;
                foreach (var item in Cihazlar)
                {
                    elektronikBedel += item.Bedel;
                }

                if (lpgMi)
                { elektronikBedel = elektronikBedel - lpgBedel; }
                rq.AudioVideoAccessories = aksesuarBedel;
                rq.OtherAccessories = elektronikBedel;
                rq.TankLpgPrice = lpgBedel;

                #endregion

                #region SpecialPricing

                rq.SpecialDiscount = "100";
                rq.DbyInd = "H";
                //rq.AgencyDiscountInd = teklif.ReadSoru(KaskoSorular.Indirim, "H");
                ////if (teklif.ReadSoru(KaskoSorular.Indirim, false))
                ////{
                ////    rq.AgencyDiscountInd = "E";
                ////}
                ////else
                ////{
                rq.AgencyDiscountInd = "H";
                ////}
                rq.DiscountPackage = "1";
                //if (teklif.ReadSoru(KaskoSorular.KomisyonluIndirim, false))
                //{
                //    rq.DiscountRequest = "2";
                //}
                //else
                //{
                rq.DiscountRequest = "1";
                //}

                //if (teklif.ReadSoru(KaskoSorular.EkKomisyonluIndirim, false))
                //{
                //    rq.SpecialPricing = "E";
                //}
                //else
                //{
                rq.SpecialPricing = "H";
                //}

                #endregion

                request.InRiskQuestionValues = rq;

                //request.InRiskQuestionValues = new eurekosigorta.kaskoV3.ExecuteRequestInRiskQuestionValues();
                //// ==== Koltuk Sayısı ==== //
                //AracTip aracTip = _AracContext.AracTipRepository.FindById(new object[] { teklif.Arac.Marka, teklif.Arac.AracinTipi });
                //if (aracTip != null)
                //{
                //    request.InRiskQuestionValues.SeatNum = aracTip.KisiSayisi.ToString();
                //}

                //string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                //string kullanimTarziKodu = String.Empty;
                //string kod2 = String.Empty;
                //if (parts.Length == 2)
                //{
                //    kullanimTarziKodu = parts[0];
                //    kod2 = parts[1];
                //    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.EUREKO &&
                //                                                                                  f.KullanimTarziKodu == kullanimTarziKodu &&
                //                                                                                  f.Kod2 == kod2)
                //                                                                                  .SingleOrDefault<CR_KullanimTarzi>();
                //    if (kullanimTarzi != null)
                //    {
                //        string[] tarz = kullanimTarzi.TarifeKodu.Split('-');
                //        request.InRiskQuestionValues.VehicleUsage = tarz[1];
                //    }
                //    else request.InRiskQuestionValues.VehicleUsage = "A";
                //}

                //AracModel aracModel = _AracContext.AracModelRepository.Filter(s => s.MarkaKodu == teklif.Arac.Marka &&
                //                                                               s.Model == teklif.Arac.Model &&
                //                                                               s.TipKodu == teklif.Arac.AracinTipi).FirstOrDefault();
                //if (aracModel != null && aracModel.Fiyat.HasValue)
                //    request.InRiskQuestionValues.VehiclePrice = aracModel.Fiyat.Value;

                //request.InRiskQuestionValues.ManufactureYear = aracModel.Model.ToString();
                //request.InRiskQuestionValues.VehicleEngineNum = teklif.Arac.MotorNo;
                //request.InRiskQuestionValues.VehilceChassisNum = teklif.Arac.SasiNo;
                //request.InRiskQuestionValues.NoClaimYear = "0";
                //request.InRiskQuestionValues.PlateCity = teklif.Arac.PlakaKodu;
                //request.InRiskQuestionValues.PlateDetail = teklif.Arac.PlakaNo;

                //bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);

                //string oncekiSirketKodu = String.Empty;
                //string oncekiPoliceNo = String.Empty;
                //string oncekiAcenteNo = String.Empty;
                //string oncekiYenilemeNo = String.Empty;

                //if (eskiPoliceVar)
                //{
                //    request.InRiskQuestionValues.InsurersFirstPolicy = "H";

                //    oncekiSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                //    oncekiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                //    oncekiAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                //    oncekiYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);

                //    request.InRiskQuestionValues.PrevInsCompany = oncekiSirketKodu;
                //    request.InRiskQuestionValues.PrevPolicyNum = oncekiPoliceNo;
                //    request.InRiskQuestionValues.PrevAgenyCode = oncekiAcenteNo;
                //    request.InRiskQuestionValues.PrevRenewalNum = oncekiYenilemeNo;
                //}
                //else
                //{
                //    request.InRiskQuestionValues.InsurersFirstPolicy = "E";
                //    request.InRiskQuestionValues.NoClaimYear = "0";
                //}

                //request.InRiskQuestionValues.VehicleValueCode = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                //request.InRiskQuestionValues.ImmLevel = "2B";
                //request.InRiskQuestionValues.FkkLevel = "19";
                //request.InRiskQuestionValues.AudioVideoAccessories = 0;
                //request.InRiskQuestionValues.OtherAccessories = 0;
                //request.InRiskQuestionValues.CommercialCoefficient = 100;
                //request.InRiskQuestionValues.LossPayeeExist = "H";
                //request.InRiskQuestionValues.SpecialDiscount = "100";

                ////---Deprem Var Mı
                //bool Deprem = teklif.ReadSoru(KaskoSorular.Deprem_VarYok, false);

                //if (Deprem)
                //{
                //    request.InRiskQuestionValues.EarthquakeFloodDeduct = "E";
                //}
                //else
                //{
                //    request.InRiskQuestionValues.EarthquakeFloodDeduct = "H";
                //}

                ////Yurt Dışı Teminatı
                //bool YurtDisi = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_VarYok, false);
                //string EurekoYurtDisiSuresi = String.Empty;
                //if (YurtDisi)
                //{
                //    string YurtDisiSuresi = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "1");
                //    if (!String.IsNullOrEmpty(YurtDisiSuresi))
                //    {
                //        switch (YurtDisiSuresi)
                //        {                               //Eureko Sigorta Kodları set ediliyor
                //            case "1": EurekoYurtDisiSuresi = "1"; break; //0-15 gün
                //            case "2": EurekoYurtDisiSuresi = "2"; break; //15gün- 1 ay
                //            case "3": EurekoYurtDisiSuresi = "3"; break; //1-3 ay
                //            case "4": EurekoYurtDisiSuresi = "4"; break; //3-6 ay
                //            case "5": EurekoYurtDisiSuresi = "5"; break; //6ay- 1 yıl
                //        }
                //    }
                //}
                //else
                //{
                //    EurekoYurtDisiSuresi = "6"; //yok
                //}

                //request.InRiskQuestionValues.AbroadPeriod = EurekoYurtDisiSuresi;
                ////request.InRiskQuestionValues.OldPlateCity = "";
                ////request.InRiskQuestionValues.OldPlateDetail = "";

                ////request.InRiskQuestionValues.PrevNoClaimYear = "";
                ////request.InRiskQuestionValues.VendorName = "";
                //request.InRiskQuestionValues.DiscountPackage = "1";
                //request.InRiskQuestionValues.TankLpgPrice = 0; //Kasa/tank/lpg bedeli girilmelidir. Yoksa "0" değeri gönderilmelidir.
                //request.InRiskQuestionValues.SpecialPricing = "H";
                //request.InRiskQuestionValues.VdfAdditionalRisk = "H";
                //request.InRiskQuestionValues.DbyInd = "E";
                //request.InRiskQuestionValues.VehicleLicenceCode = teklif.Arac.TescilSeriKod;//Araç ruhsat seri numarası bilgisidir.  Bilinmiyor ise boş gönderilebilir.
                //request.InRiskQuestionValues.VehicleLicenceNum = teklif.Arac.TescilSeriNo; //Araç ruhsat tescil no bilgisidir. Bilinmiyor ise boş gönderilebilir.

                //if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))

                //    request.InRiskQuestionValues.NotaryReferenceNum = teklif.Arac.AsbisNo;//Zorunlu değil

                #endregion

                #region Kloz

                var klozRowListAks = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow>();
                foreach (var item in Aksesuarlar)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = item.AksesuarTip + "-" + (item.Aciklama ?? string.Empty) + "-" + item.Bedel.ToString();
                    klozRow.InGListPolicyClause = kloz;
                    klozRowListAks.Add(klozRow);
                }
                var klozRowListCihazlar = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow>();
                foreach (var item in Cihazlar)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = item.AksesuarTip + "-" + (item.Aciklama ?? string.Empty) + "-" + item.Bedel.ToString();
                    klozRow.InGListPolicyClause = kloz;
                    klozRowListCihazlar.Add(klozRow);
                }
                var klozRowList = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow>();
                if (klozRowListAks.Count > 0)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = "SES VE GÖRÜNTÜ CİHAZLARI(AKSESUARLARI )";
                    klozRow.InGListPolicyClause = kloz;
                    klozRowList.Add(klozRow);
                    klozRowList.AddRange(klozRowListAks);
                }
                if (klozRowListCihazlar.Count > 0)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = "DİĞER AKSESUAR SEÇENEKLERİ ";
                    klozRow.InGListPolicyClause = kloz;
                    klozRowList.Add(klozRow);
                    klozRowList.AddRange(klozRowListCihazlar);
                }
                if (klozRowList.Count() > 0)
                    request.InGroupPolicyClause = klozRowList.ToArray();

                #endregion

                #region Risk Adress

                var policyAdres = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyAddress();
                policyAdres.PartyId = EurekoMusteriNo;
                if (sigortali.MusteriGenelBilgiler.KimlikNo.Length == 10)
                    policyAdres.AddressType = "I";
                else
                    policyAdres.AddressType = "E";
                policyAdres.AddressText = musteriAdres.Adres.Length > 100 ? musteriAdres.Adres.Substring(0, 100) : musteriAdres.Adres;
                policyAdres.CityCode = EurekoAdres == null ? EurekoAdres.CRIlKodu : "34";
                policyAdres.CountryCode = "TUR";
                policyAdres.DistrictCode = EurekoAdres == null ? EurekoAdres.CRIlceKodu.ToString().PadLeft(2, '0') : "25";
                policyAdres.VicinityVillageCode = "000";// (musteriAdres.SemtKoyKodu ?? 0).ToString().PadLeft(3, '0');
                policyAdres.VicinityVillageType = "S";// (string.IsNullOrEmpty(musteriAdres.SemtKoyTipKodu)) ? "K" : musteriAdres.SemtKoyTipKodu;
                request.InPolicyAddress = policyAdres;

                #endregion


                //#region InPolicyLossPayee

                //request.InPolicyLossPayee = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyLossPayee();
                //request.InPolicyLossPayee.LossPayeeContractNum = "";
                //request.InPolicyLossPayee.LossPayeeEndDate = "0";
                //request.InPolicyLossPayee.LossPayeeId = EurekoDMMusteriNo;
                //request.InPolicyLossPayee.LossPayeeStartDate = "0";
                //request.InPolicyLossPayee.LossPayeeRate = 0;
                //request.InPolicyLossPayee.LossPayeeCurrencyCode = "";
                //request.InPolicyLossPayee.LossPayeeAmount = 0;
                //// request.Request.InPolicyLossPayee.ExplanationText = "100";

                //#endregion

                #region Service Call

                this.BeginLog(request, typeof(eurekosigorta.kaskoV3.ExecuteRequest), WebServisIstekTipleri.Teklif);

                response = kaskoClient.Execute(request);
                kaskoClient.Dispose();
                eureko.policeteklifbilgi.ExecuteResponseResponse InfoResponse = new eureko.policeteklifbilgi.ExecuteResponseResponse();
                eureko.teminat.ExecuteResponseResponse teminatResponse = new eureko.teminat.ExecuteResponseResponse();

                bool InfoResonseSuccess = false;
                bool teminatResonseSuccess = false;

                if (response.OutHeader.IsSuccessfull == "false")
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.OutHeader.ResponseMessage);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());


                    eureko.policeteklifbilgi.PolicyGenInfoReadV3Service infoclient = new eureko.policeteklifbilgi.PolicyGenInfoReadV3Service();

                    infoclient.Url = konfigTeklifPoliceBilgi[Konfig.EUREKO_PoliceTeklifBilgiServiceURL];
                    infoclient.Timeout = 150000;

                    eureko.teminat.PolicyComponentReadService teminatClient = new eureko.teminat.PolicyComponentReadService();
                    teminatClient.Url = konfigTeminat[Konfig.EUREKO_PoliceTeminatServiceURL];
                    teminatClient.Timeout = 150000;

                    eureko.policeteklifbilgi.ExecuteRequest InfoRequest = new eureko.policeteklifbilgi.ExecuteRequest();
                    eureko.teminat.ExecuteRequest teminatRequest = new eureko.teminat.ExecuteRequest();

                    //Teklif Genel Bilgileri ve Vergileri WS den okuma
                    #region GenInfoReadInHeader

                    InfoRequest.InHeader = new eureko.policeteklifbilgi.ExecuteRequestInHeader();
                    InfoRequest.InHeader.CompanyId = servisKullanici.CompanyId;
                    InfoRequest.InHeader.UserId = servisKullanici.KullaniciAdi;
                    InfoRequest.InHeader.Password = servisKullanici.Sifre;
                    InfoRequest.InHeader.PlatformType = (eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                    InfoRequest.InHeader.MessageId = "100";

                    #endregion

                    #region GenInfoReadInPolicyMaster

                    InfoRequest.InPolicyMaster = new eureko.policeteklifbilgi.ExecuteRequestInPolicyMaster();
                    InfoRequest.InPolicyMaster.CompanyCode = "GS";
                    InfoRequest.InPolicyMaster.PolicyGroupNum = "0";
                    InfoRequest.InPolicyMaster.PolicyNum = response.OutPolicyMaster.PolicyNum;
                    InfoRequest.InPolicyMaster.RenewalNum = "0";
                    InfoRequest.InPolicyMaster.EndorsementNum = "0";
                    InfoRequest.InPolicyMaster.InternalEndorsementNum = "0";
                    InfoRequest.InPolicyMaster.ChangeSequenceNum = response.OutPolicyMaster.ChangeSequenceNum;
                    InfoRequest.InPolicyMaster.AgencyCode = servisKullanici.PartajNo_;
                    InfoRequest.InPolicyMaster.SubAgencyCode = servisKullanici.SubAgencyCode;
                    InfoRequest.InPolicyMaster.SourceId = servisKullanici.SourceId;

                    #endregion

                    #region PoliceTeklifBilgiResponse

                    this.BeginLog(InfoRequest, InfoRequest.GetType(), WebServisIstekTipleri.ESTeklifPoliceRead);

                    InfoResponse = infoclient.Execute(InfoRequest);

                    if (InfoResponse.OutHeader.IsSuccessfull == "false")
                    {
                        this.EndLog(InfoResponse, false, InfoResponse.GetType());
                        this.AddHata(InfoResponse.OutHeader.ResponseMessage);
                    }
                    else
                    {
                        this.EndLog(InfoResponse, true, InfoResponse.GetType());
                        InfoResonseSuccess = true;
                    }

                    #endregion

                    //Teklif Teminat Bilgierini WS den okuma

                    #region TeminatInHeader

                    teminatRequest.InHeader = new eureko.teminat.ExecuteRequestInHeader();
                    teminatRequest.InHeader.CompanyId = servisKullanici.CompanyId;
                    teminatRequest.InHeader.UserId = servisKullanici.KullaniciAdi;
                    teminatRequest.InHeader.Password = servisKullanici.Sifre;
                    teminatRequest.InHeader.PlatformType = (eureko.teminat.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eureko.teminat.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                    teminatRequest.InHeader.MessageId = "100";

                    #endregion

                    #region TeminatInPolicyMaster

                    teminatRequest.InPolicyMaster = new eureko.teminat.ExecuteRequestInPolicyMaster();
                    teminatRequest.InPolicyMaster.CompanyCode = "GS";
                    teminatRequest.InPolicyMaster.PolicyGroupNum = "0";
                    teminatRequest.InPolicyMaster.PolicyNum = response.OutPolicyMaster.PolicyNum;
                    teminatRequest.InPolicyMaster.RenewalNum = "0";
                    teminatRequest.InPolicyMaster.EndorsementNum = "0";
                    teminatRequest.InPolicyMaster.InternalEndorsementNum = "0";
                    teminatRequest.InPolicyMaster.ChangeSequenceNum = response.OutPolicyMaster.ChangeSequenceNum;

                    #endregion

                    #region TeminatResponse

                    this.BeginLog(teminatRequest, teminatRequest.GetType(), WebServisIstekTipleri.ESTeklifPoliceteminatRead);

                    teminatResponse = teminatClient.Execute(teminatRequest);

                    if (teminatResponse.OutHeader.IsSuccessfull == "false")
                    {
                        this.EndLog(teminatResponse, false, teminatResponse.GetType());
                        this.AddHata(teminatResponse.OutHeader.ResponseMessage);
                    }
                    else
                    {
                        this.EndLog(teminatResponse, true, teminatResponse.GetType());
                        teminatResonseSuccess = true;
                    }

                    #endregion
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
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;

                if (InfoResonseSuccess)
                {
                    #region Genel bilgiler
                    this.GenelBilgiler.BrutPrim = InfoResponse.OutPolicyMaster.TotalGrossPremiumAmount;
                    this.GenelBilgiler.NetPrim = InfoResponse.OutPolicyMaster.TotalNetPremiumAmount;
                    this.GenelBilgiler.ToplamVergi = InfoResponse.OutPolicyMaster.TotalTaxAmount;
                    this.GenelBilgiler.ToplamKomisyon = InfoResponse.OutPolicyMaster.AgencyTotalCommissionAmount;
                    this.GenelBilgiler.TUMTeklifNo = InfoResponse.OutPolicyMaster.PolicyNum;
                    this.GenelBilgiler.ToplamKomisyon = InfoResponse.OutPolicyMaster.AgencyTotalCommissionAmount;
                    this.GenelBilgiler.DovizKurBedeli = InfoResponse.OutPolicyMaster.ExchangeRate;

                    if (InfoResponse.OutPolicyMaster.CurrencyCode == "TL")
                        this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;


                    this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                    #endregion

                    #region Vergiler

                    decimal ES_GiderVergisi = InfoResponse.OutComponentTaxTotals.GdvTl;
                    decimal ES_YSVergisi = InfoResponse.OutComponentTaxTotals.YsvTl;
                    decimal ES_THFVergisi = InfoResponse.OutComponentTaxTotals.ThfTl;
                    decimal ES_GarantiFonuVergisi = InfoResponse.OutComponentTaxTotals.GrfTl;

                    this.AddVergi(KaskoVergiler.GiderVergisi, ES_GiderVergisi);
                    //this.AddVergi(KaskoVergiler., ES_YSVergisi);
                    //this.AddVergi(KaskoVergiler.GiderVergisi, ES_THFVergisi);
                    //this.AddVergi(KaskoVergiler.GiderVergisi, ES_GarantiFonuVergisi);

                    #endregion

                }

                if (teminatResonseSuccess)
                {
                    #region Teminatlar

                    //---KASKO YANGIN

                    decimal tutar = 0;
                    decimal brutPrim = 0;
                    decimal netPrim = 0;
                    decimal vergi = 0;

                    var KaskoYangin = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.KaskoYangin);

                    if (KaskoYangin != null)
                    {
                        tutar = KaskoYangin.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = KaskoYangin.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = KaskoYangin.OutPolicyTax.TaxAmount;
                        brutPrim = KaskoYangin.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.Arac_Yanmasi, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---KASKO HIRSIZLIK

                    var KaskoHirsizlik = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.KaskoHirsizlik);

                    if (KaskoHirsizlik != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = KaskoHirsizlik.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = KaskoHirsizlik.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = KaskoHirsizlik.OutPolicyTax.TaxAmount;
                        brutPrim = KaskoHirsizlik.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.Arac_Calinmasi, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---GLKH Hareketleri

                    var GLHH = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.GLHHareketleri);

                    if (GLHH != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = GLHH.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = GLHH.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = GLHH.OutPolicyTax.TaxAmount;
                        brutPrim = GLHH.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.GLKHHT_Koltuk_Ferdi_Kaza, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---TERÖRİZM

                    var GLHHT = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.Terorizm);

                    if (GLHHT != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = GLHHT.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = GLHHT.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = GLHHT.OutPolicyTax.TaxAmount;
                        brutPrim = GLHHT.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.GLKHHT, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---ÖLÜM

                    var Olum = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.Olum);

                    if (Olum != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = Olum.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = Olum.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = Olum.OutPolicyTax.TaxAmount;
                        brutPrim = Olum.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.KFK_Olum, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---SÜREKLİ SAKATLIK

                    var SurekliSakatlik = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.SurekliSakatlik);

                    if (Olum != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = SurekliSakatlik.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = SurekliSakatlik.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = SurekliSakatlik.OutPolicyTax.TaxAmount;
                        brutPrim = SurekliSakatlik.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---3. ŞAHIS BEDENİ ZARARLARI

                    var SahisBasinaBedeni = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.SahisBedeniZararlar);

                    if (SahisBasinaBedeni != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = SahisBasinaBedeni.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = SahisBasinaBedeni.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = SahisBasinaBedeni.OutPolicyTax.TaxAmount;
                        brutPrim = SahisBasinaBedeni.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---3. ŞAHIS MADDİ ZARARLARI

                    var SahisBasinamaddi = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.SahisMaddiZararlar);

                    if (SahisBasinamaddi != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = SahisBasinamaddi.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = SahisBasinamaddi.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = SahisBasinamaddi.OutPolicyTax.TaxAmount;
                        brutPrim = SahisBasinamaddi.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---E.SİGORTA ASİSTANS

                    var ESigortaAsistans = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.ESigortaAsistans);

                    if (ESigortaAsistans != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = ESigortaAsistans.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = ESigortaAsistans.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = ESigortaAsistans.OutPolicyTax.TaxAmount;
                        brutPrim = ESigortaAsistans.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.Asistans_Hizmeti_7_24_Yardim, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---DEPREM 

                    var DepremTeminati = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.Deprem);

                    if (DepremTeminati != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = DepremTeminati.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = DepremTeminati.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = DepremTeminati.OutPolicyTax.TaxAmount;
                        brutPrim = DepremTeminati.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.Deprem, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---SEYLAP

                    var Seylap = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.Seylap);

                    if (Seylap != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = Seylap.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = Seylap.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = Seylap.OutPolicyTax.TaxAmount;
                        brutPrim = Seylap.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.Seylap, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---HUKUKSAL KORUMA

                    var HukuksalKoruma = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.HukuksalKoruma);

                    if (HukuksalKoruma != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = HukuksalKoruma.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = HukuksalKoruma.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = HukuksalKoruma.OutPolicyTax.TaxAmount;
                        brutPrim = HukuksalKoruma.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.Hukuksal_Koruma, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //---HASARSILIK KORUMA

                    var HasarsizlikKoruma = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.HasarsizlikKoruma);

                    if (HasarsizlikKoruma != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = HasarsizlikKoruma.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = HasarsizlikKoruma.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = HasarsizlikKoruma.OutPolicyTax.TaxAmount;
                        brutPrim = HasarsizlikKoruma.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.HasarsizlikKoruma, tutar, vergi, netPrim, brutPrim, 0);
                    }


                    //---KEMİRGEN HAYVAN ZARARLARI

                    var KemirgenKayvanZarari = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_KaskoTeminatlar.KemirgenHayvanTeminati);

                    if (KemirgenKayvanZarari != null)
                    {
                        tutar = 0;
                        brutPrim = 0;
                        netPrim = 0;
                        vergi = 0;

                        tutar = KemirgenKayvanZarari.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = KemirgenKayvanZarari.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = KemirgenKayvanZarari.OutPolicyTax.TaxAmount;
                        brutPrim = KemirgenKayvanZarari.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(KaskoTeminatlar.Hayvanlarin_Verecegi_Zarar, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    #endregion
                }

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                #endregion

                #region Web servis cevapları

                if (MusteriAyniMi)
                {
                    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaliMusteriNo, EurekoMusteriNo);
                    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaEttirenMusteriNo, EurekoMusteriNo);
                }
                else
                {
                    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaliMusteriNo, EurekoMusteriNo);
                    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaEttirenMusteriNo, EurekoSEMusteriNo);

                }

                this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_ChangeSeqNum, response.OutPolicyMaster.ChangeSequenceNum);
                //if (EurekoDMMusteriNo != "0")
                //    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_DainiMurteinMusteriNo, EurekoDMMusteriNo);

                #endregion


                var url = GetPDFAndBilgilendirmePDF(true, response.OutPolicyMaster.ChangeSequenceNum, response.OutPolicyMaster.PolicyNum, response.OutPolicyMaster.PolicyGroupNum, response.OutPolicyMaster.RenewalNum, response.OutPolicyMaster.EndorsementNum, response.OutPolicyMaster.InternalEndorsementNum);
                this.GenelBilgiler.PDFDosyasi = url.pdfURL;
                _Log.Info("Teklif_PDF url: {0}", this.GenelBilgiler.PDFDosyasi);
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                kaskoClient.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            eurekosigorta.kaskoV3.AutoInsPolicyCreateV3Service kaskoPoliceClient = new eurekosigorta.kaskoV3.AutoInsPolicyCreateV3Service();

            try
            {
                #region Veri Hazırlama GENEL
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEurekoKasko);
                KonfigTable konfigPlatformType = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPlatformType);
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon musteriTelefon = sigortali.MusteriTelefons.FirstOrDefault(f => f.MusteriKodu == sigortali.MusteriKodu);
                MusteriAdre musteriAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                var EurekoAdres = _CRContext.CR_IlIlceRepository.Find(s => s.IlKodu == musteriAdres.IlKodu && s.IlceKodu == musteriAdres.IlceKodu && s.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                #endregion

                #region Policelestirme Request
                               
                kaskoPoliceClient.Url = konfig[Konfig.EUREKO_KaskoServiceURL];
                kaskoPoliceClient.Timeout = 150000;
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.EUREKO });

                eurekosigorta.kaskoV3.ExecuteRequest request = new eurekosigorta.kaskoV3.ExecuteRequest();
                eurekosigorta.kaskoV3.ExecuteResponseResponse response = new eurekosigorta.kaskoV3.ExecuteResponseResponse();

                string ChangeSequenceNum = this.ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_ChangeSeqNum, "0");
                string SEMusteriNo = this.ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaEttirenMusteriNo, "0");
                string SMusteriNo = this.ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaliMusteriNo, "0");
                string DMMusteriNo = this.ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_DainiMurteinMusteriNo, "0");

                #region In Header

                var header = new eurekosigorta.kaskoV3.ExecuteRequestInHeader();
                header.CompanyId = servisKullanici.CompanyId;
                header.UserId = servisKullanici.KullaniciAdi;
                header.Password = servisKullanici.Sifre;
                header.PlatformType = (eurekosigorta.kaskoV3.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eurekosigorta.kaskoV3.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                header.MessageId = "100";
                request.InHeader = header;
                #endregion

                #region InGlobalInfo

                request.InGlobalInfo = new eurekosigorta.kaskoV3.ExecuteRequestInGlobalInfo();
                //Teklif için "PROPOSAL", poliçe için "POLICY" girilmelidir
                request.InGlobalInfo.OperationCode = "PROPOSALTOPOLICY";
                request.InGlobalInfo.RaAndAgencySameFlag = "H";

                bool dainiMurtein = teklif.ReadSoru(KaskoSorular.DainiMurtein_VarYok, false);
                if (dainiMurtein)
                {
                    string kimlikNo = teklif.ReadSoru(KaskoSorular.DainiMurtein_KimlikNo, string.Empty);
                    if (!string.IsNullOrEmpty(kimlikNo))
                    {
                        var lossPayee = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyLossPayee();
                        lossPayee.LossPayeeAmount = 10;
                        lossPayee.LossPayeeContractNum = "1";
                        lossPayee.LossPayeeCurrencyCode = "TL";
                        lossPayee.LossPayeeEndDate = teklif.GenelBilgiler.BitisTarihi.ToString("yyyyMMdd");
                        lossPayee.LossPayeeId = kimlikNo;
                        lossPayee.LossPayeeRate = 1;
                        lossPayee.LossPayeeStartDate = teklif.GenelBilgiler.BaslamaTarihi.ToString("yyyyMMdd");
                        request.InPolicyLossPayee = lossPayee;
                    }
                }
                #endregion

                #region InPolicyMaster

                request.InPolicyMaster = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyMaster();
                request.InPolicyMaster.CompanyCode = "GS";
                request.InPolicyMaster.AgencyCode = servisKullanici.PartajNo_;        
                request.InPolicyMaster.SubAgencyCode = servisKullanici.SubAgencyCode; 
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                request.InPolicyMaster.PolicyStartDate = polBaslangic.ToString("yyyyMMdd");
                request.InPolicyMaster.PolicyEndDate = polBaslangic.AddYears(1).ToString("yyyyMMdd");
                request.InPolicyMaster.ProcessingDate = DateTime.Now.ToString("yyyyMMdd");
                request.InPolicyMaster.CurrencyCode = "TL";
                request.InPolicyMaster.ProductCode = EUREKO_Urunkodlari.KASKO;
                request.InPolicyMaster.TariffCode = "10";      //Tarife Kodu bilgisidir. Bu kodlar ürünlerde farklılaşma yapıldığında değişebilmektedir. Kasko mevcut için mevcutta "10" gönderilmelidir.
                if (odeme.OdemeSekli == OdemeSekilleri.Pesin) 
                    request.InPolicyMaster.PaymentType = "AL5";
                else
                {
                    if (odeme.TaksitSayisi == 2)
                        request.InPolicyMaster.PaymentType = "AL3";
                    else if (odeme.TaksitSayisi == 5)
                        request.InPolicyMaster.PaymentType = "A11";
                    else if (odeme.TaksitSayisi == 8)
                        request.InPolicyMaster.PaymentType = "A67";
                    else if (odeme.TaksitSayisi == 9)
                        request.InPolicyMaster.PaymentType = "KK9";

                }
                var TUMTeklif = _TeklifService.GetTeklifListe(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu).FirstOrDefault(s => s.GenelBilgiler.TUMKodu == TeklifUretimMerkezleri.EUREKO);
                request.InPolicyMaster.SourceId = servisKullanici.SourceId;
                request.InPolicyMaster.ExternalKeyText = string.Empty;
                request.InPolicyMaster.PolicyGroupNum = "0";
                request.InPolicyMaster.PolicyNum = TUMTeklif.GenelBilgiler.TUMTeklifNo;
                request.InPolicyMaster.RenewalNum = this.ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_RenewalNum, "0");
                request.InPolicyMaster.EndorsementNum = this.ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_EndorsementNum, "0");
                request.InPolicyMaster.InternalEndorsementNum = this.ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_InternalEndorsementNum, "0");
                request.InPolicyMaster.ChangeSequenceNum = ChangeSequenceNum;

                #endregion

                #region InGroupPolicyParty

                List<ExecuteRequestInGroupPolicyPartyRow> gruprow = new List<ExecuteRequestInGroupPolicyPartyRow>();
                List<ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty> gruplar = new List<ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty>();

                ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();

                //  Müşteri taraf bilgisidir. Sigortalı -"SG" , Sigorta Ettiren "SE", Prim Ödeyen-"PE" olarak girilmelidir.
                grup.PartyType = "SG";
                grup.PartyId = SMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 1; //Oran bilgisidir. "1" gönderilmelidir.
                gruplar.Add(grup);

                grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "SE";
                grup.PartyId = (Convert.ToInt32(SEMusteriNo) > 0) ? SEMusteriNo : SMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 0; //Oran bilgisidir. "0" gönderilmelidir.
                gruplar.Add(grup);

                grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "PO";
                grup.PartyId = (Convert.ToInt32(SEMusteriNo) > 0) ? SEMusteriNo : SMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 1; //Oran bilgisidir. "1" gönderilmelidir.
                gruplar.Add(grup);

                ExecuteRequestInGroupPolicyPartyRow row;
                foreach (var item in gruplar)
                {
                    row = new ExecuteRequestInGroupPolicyPartyRow();
                    row.InGListPolicyParty = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                    row.InGListPolicyParty.PartyType = item.PartyType;
                    row.InGListPolicyParty.PartyId = item.PartyId;
                    row.InGListPolicyParty.PartyRate = item.PartyRate;

                    gruprow.Add(row);
                }
                request.InGroupPolicyParty = gruprow.ToArray();

                #endregion

                #region InPolicyPaymentInstallment

                TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;

                request.InPolicyPaymentInstallment = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyPaymentInstallment();
                request.InPolicyPaymentInstallment.PartyId = (Convert.ToInt32(SEMusteriNo) > 0) ? SEMusteriNo : SMusteriNo;
                request.InPolicyPaymentInstallment.FirstBankCode = "0";
                request.InPolicyPaymentInstallment.FirstBranchCode = "0";

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    request.InPolicyPaymentInstallment.PartyId = (Convert.ToInt32(SEMusteriNo) > 0) ? SEMusteriNo : SMusteriNo; //SEMusteriNo;
                    request.InPolicyPaymentInstallment.FirstCreditCardCode = odeme.KrediKarti.KartNo;
                    request.InPolicyPaymentInstallment.FirstCvcNum ="";
                    request.InPolicyPaymentInstallment.FirstExpireDate = odeme.KrediKarti.SKA + odeme.KrediKarti.SKY.Substring(2, 2);
                }

                #endregion

                #region InRiskQuestionValues

                var rq = new eurekosigorta.kaskoV3.ExecuteRequestInRiskQuestionValues();

                #region  Vehicle

                rq.SeatNum = (teklif.Arac.KoltukSayisi ?? 0).ToString();
                //var sekil = Convert.ToInt32(teklif.Arac.KullanimSekli);
                //var eurekoKullanimSekli = _AracContext.EurekoAracKullanimTarziRepository.All().Where(s => s.Id == sekil).FirstOrDefault();
                var sekilParts = teklif.Arac.KullanimTarzi.Split('-');
                var sekilKod = sekilParts[0];
                var sekilKod2 = sekilParts[1];

                var EurkeoAracKullanimTarzi = _CRContext.CR_KullanimTarziRepository.Find(s => s.KullanimTarziKodu == sekilKod && s.Kod2 == sekilKod2 && s.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                var EurkeoAracKullanimTarziParts = EurkeoAracKullanimTarzi.TarifeKodu.Split('-');
                if (EurkeoAracKullanimTarziParts != null) rq.VehicleUsage = EurkeoAracKullanimTarziParts[1];
                else rq.VehicleUsage = "A";
                //if (EurkeoAracKullanimTarziParts != null) rq.CommercialUsageType = EurkeoAracKullanimTarziParts[0].ToString();
                //else
                rq.CommercialUsageType = string.Empty;
                rq.VehiclePrice = (teklif.Arac.AracDeger ?? 0);
                rq.ManufactureYear = (teklif.Arac.Model ?? 0).ToString();
                if (string.IsNullOrEmpty(teklif.Arac.MotorNo))
                    rq.VehicleEngineNum = ("MOTOR" + teklif.GenelBilgiler.TVMKodu.ToString() + teklif.GenelBilgiler.TeklifNo.ToString()).PadLeft(10, '0');
                else
                    rq.VehicleEngineNum = teklif.Arac.MotorNo;
                if (string.IsNullOrEmpty(teklif.Arac.SasiNo))
                    rq.VehilceChassisNum = ("SASI" + teklif.GenelBilgiler.TVMKodu.ToString() + teklif.GenelBilgiler.TeklifNo.ToString()).PadLeft(17, '0');
                else
                    rq.VehilceChassisNum = teklif.Arac.SasiNo;
                rq.NoClaimYear = "0";
                rq.PlateCity = teklif.Arac.PlakaKodu;
                rq.PlateDetail = teklif.Arac.PlakaNo;
                bool eskiPoliceVar = teklif.ReadSoru(KaskoSorular.Eski_Police_VarYok, false);
                if (eskiPoliceVar)
                {
                    rq.InsurersFirstPolicy = "H";
                    rq.PrevInsCompany = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, string.Empty);
                    rq.PrevPolicyNum = teklif.ReadSoru(TrafikSorular.Eski_Police_No, string.Empty);
                    rq.PrevAgenyCode = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, string.Empty);
                    rq.PrevRenewalNum = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, string.Empty);
                }
                else
                {
                    rq.InsurersFirstPolicy = "E";
                    rq.NoClaimYear = "0";
                }
                rq.VehicleValueCode = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                rq.CommercialCoefficient = 100;
                rq.LossPayeeExist = "H";
                rq.EarthquakeFloodDeduct = "H";
                rq.AbroadPeriod = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "6");
                rq.VdfAdditionalRisk = "H";
                rq.VehicleLicenceCode = teklif.Arac.TescilSeriKod;
                rq.VehicleLicenceNum = teklif.Arac.TescilSeriNo;
                if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                    rq.NotaryReferenceNum = teklif.Arac.AsbisNo;
                if (request.InPolicyLossPayee != null) rq.LossPayeeExist = "E";
                else rq.LossPayeeExist = "H";

                #endregion

                #region IMM FK

                //   bool immDahil = teklif.ReadSoru(KaskoSorular.IMMManeviDahil, false);
                string imm = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, "0");
                string fk = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, "0");
                string eurekoImmKodu = "2B";
                string eurekoFkKodu = "19";

                //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(imm), sekilKod, sekilKod2);
                CR_KaskoIMM CRKademeNo = new CR_KaskoIMM();
                if (IMMBedel != null)
                {
                    //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                    CRKademeNo = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.EUREKO, IMMBedel.BedeniSahis, IMMBedel.Kombine, sekilKod, sekilKod2);
                    if (CRKademeNo != null)
                    {
                        if (IMMBedel.BedeniSahis == 0)
                        {
                            var parts = CRKademeNo.Kademe.Split('-');
                            eurekoImmKodu = parts[1];
                        }
                        else
                        {
                            eurekoImmKodu = "0";
                        }
                    }
                }


                CR_KaskoFK CRKademeNu = new CR_KaskoFK();
                var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fk), sekilKod, sekilKod2);

                if (FKBedel != null)
                {
                    CRKademeNu = _CRService.GetCRKaskoFKBedel(TeklifUretimMerkezleri.EUREKO, FKBedel.Vefat, FKBedel.Tedavi, FKBedel.Kombine, sekilKod, sekilKod2);
                    if (CRKademeNu != null)
                    {
                        eurekoFkKodu = CRKademeNu.Kademe;
                    }

                }

                rq.ImmLevel = eurekoImmKodu;
                rq.FkkLevel = eurekoFkKodu;


                #endregion

                #region Akesuar

                var sesgoruntu = teklif.ReadSoru(KaskoSorular.Teminat_Ekstra_Aksesuar_VarYok, false);
                List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR).ToList<TeklifAracEkSoru>();
                var Aksesuarlar = new List<MapfreAksesuarModel>();
                if (Aksesuarlar != null && aksesuarlar.Count > 0)
                {
                    List<CR_AracEkSoru> eksorular = _CRService.GetAracEkSoru(TeklifUretimMerkezleri.EUREKO, MapfreKaskoEkSoruTipleri.AKSESUAR);
                    foreach (var item in aksesuarlar)
                    {
                        var soru = eksorular.FirstOrDefault(f => f.SoruKodu == item.SoruKodu);
                        if (soru != null)
                        {
                            Aksesuarlar.Add(new MapfreAksesuarModel()
                            {
                                AksesuarTip = soru.SoruAdi,
                                Aciklama = item.Aciklama,
                                Bedel = (int)item.Bedel
                            });
                        }
                    }
                }
                int aksesuarBedel = 0;
                foreach (var item in Aksesuarlar)
                {
                    aksesuarBedel += item.Bedel;
                }
                var ElektronikCihaz_Teminati = teklif.ReadSoru(KaskoSorular.Teminat_ElektronikCihaz_VarYok, false);
                List<TeklifAracEkSoru> elekCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ).ToList<TeklifAracEkSoru>();
                var Cihazlar = new List<MapfreAksesuarModel>();
                bool lpgMi = false;
                int lpgBedel = 0;
                if (elekCihazlar != null && elekCihazlar.Count > 0)
                {
                    List<CR_AracEkSoru> cihazlar = _CRService.GetAracEkSoru(TeklifUretimMerkezleri.EUREKO, MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ);
                    foreach (var item in elekCihazlar)
                    {
                        var soru = cihazlar.FirstOrDefault(f => f.SoruKodu == item.SoruKodu);
                        if (soru != null)
                        {
                            if (soru.SoruKodu == "5")
                            {
                                lpgMi = true;
                                lpgBedel = (int)item.Bedel;
                            }
                            Cihazlar.Add(new MapfreAksesuarModel()
                            {
                                AksesuarTip = soru.SoruAdi,
                                Aciklama = item.Aciklama,
                                Bedel = (int)item.Bedel

                            });
                        }
                    }
                }
                int elektronikBedel = 0;
                foreach (var item in Cihazlar)
                {
                    elektronikBedel += item.Bedel;
                }

                if (lpgMi)
                { elektronikBedel = elektronikBedel - lpgBedel; }
                rq.AudioVideoAccessories = aksesuarBedel;
                rq.OtherAccessories = elektronikBedel;
                rq.TankLpgPrice = lpgBedel;

                #endregion

                #region SpecialPricing

                rq.SpecialDiscount = "100";
                rq.DbyInd = "H";              
                rq.AgencyDiscountInd = "H";           
                rq.DiscountPackage = "1";             
                rq.DiscountRequest = "1";              
                rq.SpecialPricing = "H";
              
                #endregion

                request.InRiskQuestionValues = rq;
            
                #endregion

                #region Kloz

                var klozRowListAks = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow>();
                foreach (var item in Aksesuarlar)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = item.AksesuarTip + "-" + (item.Aciklama ?? string.Empty) + "-" + item.Bedel.ToString();
                    klozRow.InGListPolicyClause = kloz;
                    klozRowListAks.Add(klozRow);
                }
                var klozRowListCihazlar = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow>();
                foreach (var item in Cihazlar)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = item.AksesuarTip + "-" + (item.Aciklama ?? string.Empty) + "-" + item.Bedel.ToString();
                    klozRow.InGListPolicyClause = kloz;
                    klozRowListCihazlar.Add(klozRow);
                }
                var klozRowList = new List<eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow>();
                if (klozRowListAks.Count > 0)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = "SES VE GÖRÜNTÜ CİHAZLARI(AKSESUARLARI )";
                    klozRow.InGListPolicyClause = kloz;
                    klozRowList.Add(klozRow);
                    klozRowList.AddRange(klozRowListAks);
                }
                if (klozRowListCihazlar.Count > 0)
                {
                    var klozRow = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRow();
                    var kloz = new eurekosigorta.kaskoV3.ExecuteRequestInGroupPolicyClauseRowInGListPolicyClause();
                    kloz.ClauseText = "DİĞER AKSESUAR SEÇENEKLERİ ";
                    klozRow.InGListPolicyClause = kloz;
                    klozRowList.Add(klozRow);
                    klozRowList.AddRange(klozRowListCihazlar);
                }
                if (klozRowList.Count() > 0)
                    request.InGroupPolicyClause = klozRowList.ToArray();

                #endregion

                #region Risk Adress

                var policyAdres = new eurekosigorta.kaskoV3.ExecuteRequestInPolicyAddress();
                policyAdres.PartyId = SMusteriNo;
                if (sigortali.KimlikNo.Length == 10)
                    policyAdres.AddressType = "I";
                else
                    policyAdres.AddressType = "E";
                policyAdres.AddressText = musteriAdres.Adres.Length > 100 ? musteriAdres.Adres.Substring(0, 100) : musteriAdres.Adres;
                policyAdres.CityCode = EurekoAdres == null ? EurekoAdres.CRIlKodu : "34";
                policyAdres.CountryCode = "TUR";
                policyAdres.DistrictCode = EurekoAdres == null ? EurekoAdres.CRIlceKodu.ToString().PadLeft(2, '0') : "25";
                policyAdres.VicinityVillageCode = "000";// (musteriAdres.SemtKoyKodu ?? 0).ToString().PadLeft(3, '0');
                policyAdres.VicinityVillageType = "S";// (string.IsNullOrEmpty(musteriAdres.SemtKoyTipKodu)) ? "K" : musteriAdres.SemtKoyTipKodu;
                request.InPolicyAddress = policyAdres;

                #endregion

                #region Service Call

                this.BeginLog(request, typeof(eurekosigorta.kaskoV3.ExecuteRequest), WebServisIstekTipleri.Police);

                response = kaskoPoliceClient.Execute(request);
                kaskoPoliceClient.Dispose();

                eureko.policeteklifbilgi.ExecuteResponseResponse InfoResponse = new eureko.policeteklifbilgi.ExecuteResponseResponse();
                eureko.teminat.ExecuteResponseResponse teminatResponse = new eureko.teminat.ExecuteResponseResponse();

                if (response.OutHeader.IsSuccessfull == "false")
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.OutHeader.ResponseMessage);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());

                    this.GenelBilgiler.TUMPoliceNo = response.OutPolicyMaster.PolicyNum;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    var url = this.GetPDFAndBilgilendirmePDF(false, ChangeSequenceNum, response.OutPolicyMaster.PolicyNum, response.OutPolicyMaster.PolicyGroupNum, response.OutPolicyMaster.RenewalNum, response.OutPolicyMaster.EndorsementNum, response.OutPolicyMaster.InternalEndorsementNum);
                    this.GenelBilgiler.PDFPolice = url.pdfURL;
                    this.GenelBilgiler.PDFBilgilendirme = url.bilgilendirmeURL;

                    _Log.Info("Police_PDF url: {0}", this.GenelBilgiler.PDFPolice);
                    _Log.Info("Police_Bilgilendirme_PDF url: {0}", this.GenelBilgiler.PDFBilgilendirme);
                    var cevap = this.GenelBilgiler.TeklifWebServisCevaps.Where(c => c.CevapKodu == Common.WebServisCevaplar.EUREKO_PolicyGroupNum).FirstOrDefault();
                    if (cevap == null)
                    {
                        cevap = new TeklifWebServisCevap();
                        cevap.CevapKodu = Common.WebServisCevaplar.EUREKO_PolicyGroupNum;
                        cevap.Cevap = response.OutPolicyMaster.PolicyGroupNum;
                        cevap.CevapTipi = SoruCevapTipleri.Metin;
                        this.GenelBilgiler.TeklifWebServisCevaps.Add(cevap);
                    }
                    else
                    {
                        cevap.Cevap = response.OutPolicyMaster.PolicyGroupNum;
                    }


                    #region Police InfoResponse

                    #region Konfig

                    var infClient = new eureko.policeteklifbilgi.PolicyGenInfoReadV3Service();
                    var konfigInfoClient = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOTeklifPoliceBilgi);
                    infClient.Url = konfigInfoClient[Konfig.EUREKO_PoliceTeklifBilgiServiceURL];
                    infClient.Timeout = 150000;
                    var infRequest = new eureko.policeteklifbilgi.ExecuteRequest();

                    #endregion

                    #region Header

                    var inHeader = new eureko.policeteklifbilgi.ExecuteRequestInHeader()
                    {
                        CompanyId = servisKullanici.CompanyId,
                        UserId = servisKullanici.KullaniciAdi,
                        Password = servisKullanici.Sifre,
                        PlatformType = (eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]),
                        MessageId = "100"
                    };
                    infRequest.InHeader = inHeader;

                    #endregion

                    #region PolicyMaster

                    var policyMaster = new eureko.policeteklifbilgi.ExecuteRequestInPolicyMaster()
                    {
                        CompanyCode = "GS",
                        PolicyGroupNum = "0",
                        PolicyNum = response.OutPolicyMaster.PolicyNum,
                        RenewalNum = "0",
                        EndorsementNum = "0",
                        InternalEndorsementNum = "0",
                        ChangeSequenceNum = response.OutPolicyMaster.ChangeSequenceNum,
                        AgencyCode = servisKullanici.PartajNo_,
                        SubAgencyCode = servisKullanici.SubAgencyCode,
                        SourceId = servisKullanici.SourceId
                    };
                    infRequest.InPolicyMaster = policyMaster;

                    #endregion

                    #region Response

                    this.BeginLog(infRequest, infRequest.GetType(), WebServisIstekTipleri.ESTeklifPoliceRead);

                    var infResponse = infClient.Execute(infRequest);
                    bool InfoResonseSuccess = false;
                    if (infResponse.OutHeader.IsSuccessfull == "false")
                    {
                        this.EndLog(infResponse, false, infResponse.GetType());
                        this.AddHata(infResponse.OutHeader.ResponseMessage);
                    }
                    else
                    {
                        this.EndLog(infResponse, true, infResponse.GetType());
                        InfoResonseSuccess = true;
                    }
                    if (InfoResonseSuccess)
                    {
                        this.GenelBilgiler.BrutPrim = infResponse.OutPolicyMaster.TotalGrossPremiumAmount;
                        this.GenelBilgiler.NetPrim = infResponse.OutPolicyMaster.TotalNetPremiumAmount;
                        this.GenelBilgiler.ToplamKomisyon = infResponse.OutPolicyMaster.AgencyTotalCommissionAmount;
                        this.GenelBilgiler.ToplamVergi = infResponse.OutPolicyMaster.TotalTaxAmount;
                        this.GenelBilgiler.DovizKurBedeli = infResponse.OutPolicyMaster.ExchangeRate;
                    }

                    #endregion

                    #endregion

                    if (odeme.OdemeTipi != OdemeTipleri.Nakit)
                    {
                        #region Dekont
                        string AdiSoyadi = SigortaEttiren.MusteriGenelBilgiler.AdiUnvan + "" + sigortaEttiren.MusteriGenelBilgiler.SoyadiUnvan;
                        string kradiKartNo = "";
                        string CVC = "";
                        string krediKartTipi = "";
                        string SktAyYil = "";
                        string KKAdiSoyadi = "";
                        if (odeme.KrediKarti != null)
                        {
                            kradiKartNo = odeme.KrediKarti.KartNo.Substring(0, 4) + " " + odeme.KrediKarti.KartNo.Substring(4, 2) + "** **** " + odeme.KrediKarti.KartNo.Substring(12, 4);
                            CVC = odeme.KrediKarti.CVC;
                            if (odeme.KrediKarti.KartNo.Substring(0, 1) == "5")
                                krediKartTipi = "MASTER CARD";
                            else
                                krediKartTipi = "VISA";

                            SktAyYil = odeme.KrediKarti.SKA + "/" + odeme.KrediKarti.SKY;
                            KKAdiSoyadi = odeme.KrediKarti.KartSahibi;
                        }

                        string islemTarihi = TurkeyDateTime.Today.ToString("dd/MM/yyyy");
                        string OdemeTip = String.Empty;
                        string UrunAdi = String.Empty;
                        string ToplamTutari = String.Empty;
                        if (InfoResonseSuccess)
                        {
                            var Payment = infResponse.OutGlobalInfo;
                            OdemeTip = Payment.PaymentTypeName.Trim();
                            ToplamTutari = infResponse.OutPolicyMaster.TotalGrossPremiumAmount.ToString();
                        }

                        UrunAdi = "ZORUNLU TRAFİK";
                        string AcenteNo = servisKullanici.PartajNo_;

                        string PolNo = response.OutPolicyMaster.PolicyNum; ;
                        string KimlikNo = sigortaEttiren.MusteriGenelBilgiler.KimlikNo;
                        string AcenteUnvan = _AktifkullaniciService.TVMUnvani;

                        EurokoPrimOdemeTalimati pdf = new EurokoPrimOdemeTalimati();

                        var form = new KrediKartiFormModel()
                        {
                            IslemTarihi = islemTarihi,
                            KrediKartiTuru = krediKartTipi,
                            KrediKartiNo = kradiKartNo,
                            SonKullanmaTarihi = SktAyYil,
                            TaksitSayisi = OdemeTip,
                            CVV2 = CVC,
                            AdiSoyadi = AdiSoyadi,
                            AcenteNo = AcenteNo,
                            AcenteAdi = AcenteNo + " - " + AcenteUnvan,
                            UrunAdi = UrunAdi,
                            PoliceNo = PolNo,
                            Tarih = islemTarihi,
                            KartSahibininAdiSoyadi = KKAdiSoyadi,
                            KimlikNo = KimlikNo,
                            ToplamTutar = ToplamTutari,
                        };

                        if (InfoResonseSuccess)
                        {
                            Odemeler plan;
                            List<Odemeler> OdemePlanis = new List<Odemeler>();
                            var PaymetPlan = infResponse.OutGroupPaymentSchedule;
                            var sayac2 = 1;
                            foreach (var item in PaymetPlan)
                            {
                                plan = new Odemeler();
                                plan.Tarihi = item.OutPolicyPaymentSchedule.DueDate.Substring(6, 2) + "." + item.OutPolicyPaymentSchedule.DueDate.Substring(4, 2) + "." + item.OutPolicyPaymentSchedule.DueDate.Substring(0, 4);
                                plan.Tutari = item.OutPolicyPaymentSchedule.InstallmentAmount.ToString("N");
                                plan.Taksitler = sayac2.ToString();
                                sayac2++;
                                OdemePlanis.Add(plan);
                            }
                            form.OdemePlani = OdemePlanis.ToArray();
                        }

                        var DekontPdfUrl = pdf.PdfDocument(form);
                        this.GenelBilgiler.PDFDekont = DekontPdfUrl;

                        #endregion
                    }
                }
                #endregion

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
            catch (Exception ex)
            {
                #region Hata Log

                kaskoPoliceClient.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }

        public URL GetPDFAndBilgilendirmePDF(bool teklifMi, string ChangeSequenceNum, string PolicyNum, string PolicyGroupNum, string RenewalNum, string EndorsementNum, string InternalEndorsementNum)
        {
            URL url = new URL();
            KonfigTable konfigPDF = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPDF);
            eureko.basim.GetPdfServiceService pdfClient = new eureko.basim.GetPdfServiceService();
            pdfClient.Url = konfigPDF[Konfig.EUREKO_PolicePDFServiceURL];
            pdfClient.Timeout = 150000;
            eureko.basim.PdfInputBean requestPDF = new eureko.basim.PdfInputBean();

            requestPDF.polNum = PolicyNum;
            requestPDF.polGroupNum = PolicyGroupNum;
            requestPDF.chgSeqNum = ChangeSequenceNum;
            requestPDF.renewalNum = RenewalNum;
            requestPDF.endrsNum = EndorsementNum;
            requestPDF.intEndrsNum = InternalEndorsementNum;

            byte[] teklifPDF;
            byte[] biglendirmePDF;
            string bilgilendirmePDFURL = String.Empty;
            string teklifPDFURL = String.Empty;

            if (teklifMi)
            {
                teklifPDF = pdfClient.GetPdf(requestPDF);

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("Eureko_Kasko_Teklif_PDF_{0}.pdf", System.Guid.NewGuid().ToString());
                teklifPDFURL = storage.UploadFile("kasko", fileName, teklifPDF);
            }
            else
            {
                teklifPDF = pdfClient.GetPdf(requestPDF);
                biglendirmePDF = pdfClient.GetPdfForInfoForm(requestPDF);

                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName = String.Format("Eureko_Kasko_Police_PDF_{0}.pdf", System.Guid.NewGuid().ToString());
                teklifPDFURL = storage.UploadFile("kasko", fileName, teklifPDF);

                ITeklifPDFStorage storage2 = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                string fileName2 = String.Format("Eureko_Kasko_Police_Bilgilendirme_{0}.pdf", System.Guid.NewGuid().ToString());
                bilgilendirmePDFURL = storage2.UploadFile("kasko", fileName2, biglendirmePDF);
            }

            url.pdfURL = teklifPDFURL;
            url.bilgilendirmeURL = bilgilendirmePDFURL;

            return url;
        }

        public EurekoMusteriResponseModel EurekoMusteriKaydet(EurekoMusteriRequestModel pModel)
        {
            #region Variable
            var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(pModel.TVMKodu);
            var kullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.EUREKO });
            //  var kullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { 100, TeklifUretimMerkezleri.EUREKO });
            var konfigMusteriServis = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOMusteriV2);
            var konfigPlatformType = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPlatformType);

            var client = new CustomerProcessV2Service();
            client.Url = konfigMusteriServis[Konfig.EUREKO_MusteriServisURLV2];
            client.Timeout = 150000;
            var req = new eurekosigorta.musteriV2.ExecuteRequest();

            #endregion

            #region Header

            var header = new eurekosigorta.musteriV2.ExecuteRequestInHeader();
            header.CompanyId = kullanici.CompanyId;
            header.UserId = kullanici.KullaniciAdi;
            header.Password = kullanici.Sifre;
            header.PlatformType = (eurekosigorta.musteriV2.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eurekosigorta.musteriV2.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
            header.MessageId = "100";
            req.InHeader = header;

            #endregion

            #region Customer

            var customer = new ExecuteRequestInCustomer();
            customer.CustomerType = pModel.KimlikNo.Length == 10 ? "T" : "G";
            customer.TaxNum = pModel.KimlikNo.Length == 10 ? pModel.KimlikNo : "0";
            customer.MernisNum = pModel.KimlikNo.Length == 11 ? pModel.KimlikNo : "0";
            customer.CitizenText = "TUR";
            req.InCustomer = customer;

            #endregion

            #region Adress Customer

            var adrCustomer = new ExecuteRequestInAddressCustomer();
            adrCustomer.CityCodeNum = pModel.IlKodu ?? "34";
            adrCustomer.CountyCodeText = ((pModel.IlceKodu == "0" || pModel.IlceKodu == "00") ? "22" : pModel.IlceKodu).PadLeft(2, '0');
            adrCustomer.VillageType = "K";
            adrCustomer.VillageCodeText = "000";
            adrCustomer.ZipCodeNumber = pModel.IlKodu + "000";
            adrCustomer.AddressType = "I";
            if (pModel.Adres != null) adrCustomer.Address = pModel.Adres.Length > 70 ? pModel.Adres.Substring(0, 70) : pModel.Adres;
            else
                adrCustomer.Address = "İSTANBUL BEŞİKTAŞ MERKEZ";
            adrCustomer.CountryCodeText = "TUR";
            req.InAddressCustomer = adrCustomer;

            #endregion

            #region Policy Master

            var policyMaster = new eurekosigorta.musteriV2.ExecuteRequestInPolicyMaster();
            policyMaster.SourceId = "BABONLNE";
            policyMaster.AgencyCode = "1125";
            policyMaster.SubAgencyCode = "0";
            req.InPolicyMaster = policyMaster;

            #endregion

            #region Phone
            if (!string.IsNullOrEmpty(pModel.Telefon) && pModel.Telefon.Length == 14)
            {
                string _tpye = (pModel.Telefon.Substring(3, 1) == "5") ? "C" : "T";
                string _area = pModel.Telefon.Substring(3, 3);
                string _num = pModel.Telefon.Substring(7, 7);
                req.InPhoneCustomer = new ExecuteRequestInPhoneCustomer()
                {
                    CountryCodeNum = "90",
                    RecordType = _tpye,
                    AreaCodeNum = _area,
                    PhoneNum = _num,
                };
            }
            #endregion

            #region EMail
            if (!string.IsNullOrEmpty(pModel.Email))
            {
                req.InEmailCustomer = new ExecuteRequestInEmailCustomer()
                {
                    Address = pModel.Email
                };
            }
            #endregion

            #region Return

            var result = new EurekoMusteriResponseModel();
            try
            {
                this.BeginLog(req, req.GetType(), WebServisIstekTipleri.MusteriKayit);
                var _serReq = serialize(req);
                var response = client.Execute(req);
                var _serRes = serialize(response);
                if (response.OutHeader.IsSuccessfull == "false")
                {
                    result.Hata = response.OutHeader.ResponseMessage;
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.OutHeader.ResponseMessage);
                }
                else
                {
                    this.EndLog(response, false, response.GetType());
                    var oc = response.OutCustomer;
                    result.Adi = oc.NameText;
                    result.Soyadi = oc.SurnameText;
                    result.MusteriNo = oc.CustomerNum;
                    result.Cinsiyet = oc.SexCode;
                    if (oc.BirthDate.Length == 8)
                    {
                        var dt = oc.BirthDate;
                        var y = Convert.ToInt32(dt.Substring(0, 4));
                        var m = Convert.ToInt32(dt.Substring(4, 2));
                        var d = Convert.ToInt32(dt.Substring(6, 2));
                        result.DogumTarihi = new DateTime(y, m, d);
                    }
                    else
                        result.DogumTarihi = new DateTime();
                }
            }
            catch (Exception ex)
            {
                result.Hata = ex.Message;
            }

            return result;

            #endregion
        }

        public string serialize(Object obj)
        {
            string istek = String.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    XmlSerializer s = new XmlSerializer(obj.GetType());

                    s.Serialize(xmlWriter, obj);
                }

                istek = Encoding.UTF8.GetString(ms.ToArray());
            }
            return istek;
        }

        #region Model

        public class MapfreAksesuarModel
        {
            public string AksesuarTip { get; set; }
            public string Aciklama { get; set; }
            public int Bedel { get; set; }
            public int LPGBedel { get; set; }
        }

        #endregion
    }
}
