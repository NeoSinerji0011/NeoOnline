using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Neosinerji.BABOnlineTP.Database.Repository;
using System;
using Neosinerji.BABOnlineTP.Business.eurekosigorta.trafikV4;
using System.Collections.Generic;
using EurekoSigorta_Business.EUREKO;
using Neosinerji.BABOnlineTP.Business.eurekosigorta.musteriV2;
using System.Xml;
using System.Xml.Serialization;
using EurekoSigorta_Business.Common.EUREKO;

namespace Neosinerji.BABOnlineTP.Business.EUREKO
{

    public class EUREKOTrafik : Teklif, IEUREKOTrafik
    {
        ICRContext _CRContext;
        IMusteriService _MusteriService;
        IAracContext _AracContext;
        IKonfigurasyonService _KonfigurasyonService;
        ITVMContext _TVMContext;
        ILogService _Log;
        ITeklifService _TeklifService;
        IAktifKullaniciService _AktifkullaniciService;
        ITVMService _TVMService; 

        [InjectionConstructor]
        public EUREKOTrafik(ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IAktifKullaniciService aktifkullanici, ITVMService TVMService )
            : base()
        {
            _CRContext = crContext;
            _MusteriService = musteriService;
            _AracContext = aracContext;
            _KonfigurasyonService = konfigurasyonService;
            _TVMContext = tvmContext;
            _Log = log;
            _TeklifService = teklifService;
            _AktifkullaniciService = aktifkullanici;
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
            eurekosigorta.trafikV4.ProposalAndPolicySrV4Service trafikClient = new eurekosigorta.trafikV4.ProposalAndPolicySrV4Service();

            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEurekoTrafik);
                KonfigTable konfigPlatformType = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPlatformType);
                trafikClient.Url = konfig[Konfig.EUREKO_TrafikServiceURL];
                trafikClient.Timeout = 150000;

                eurekosigorta.trafikV4.ExecuteRequest request = new eurekosigorta.trafikV4.ExecuteRequest();

                #region Veri Hazırlama

                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.EUREKO });
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                #region InHeader

                request.InHeader = new eurekosigorta.trafikV4.ExecuteRequestInHeader();
                request.InHeader.CompanyId = servisKullanici.CompanyId;
                request.InHeader.UserId = servisKullanici.KullaniciAdi;
                request.InHeader.Password = servisKullanici.Sifre;
                request.InHeader.PlatformType = (eurekosigorta.trafikV4.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eurekosigorta.trafikV4.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                request.InHeader.MessageId = "100";

                #endregion

                #region InOperationGlobalInfo

                request.InOperationGlobalInfo = new ExecuteRequestInOperationGlobalInfo();
                request.InOperationGlobalInfo.OperationCode = "PROPOSAL";

                #endregion

                #region InPolicyMaster

                request.InPolicyMaster = new eurekosigorta.trafikV4.ExecuteRequestInPolicyMaster();
                request.InPolicyMaster.AgencyCode = servisKullanici.PartajNo_;
                request.InPolicyMaster.SubAgencyCode = servisKullanici.SubAgencyCode;
                request.InPolicyMaster.SourceId = servisKullanici.SourceId;
                request.InPolicyMaster.ProductCode = "T10";
                request.InPolicyMaster.TariffCode = "10";
                request.InPolicyMaster.PolicyStartDate = polBaslangic.ToString("yyyyMMdd");
                request.InPolicyMaster.PolicyEndDate = polBaslangic.AddYears(1).ToString("yyyyMMdd");
                request.InPolicyMaster.ProcessingDate = polBaslangic.ToString("yyyyMMdd");
                request.InPolicyMaster.CurrencyCode = "TL";
                request.InPolicyMaster.CompanyCode = "GS";
                //request.InPolicyMaster.PaymentType = "AL5";
                //request.InPolicyMaster.ExternalKeyText = "0";

                #endregion

                #region InPolicyLossPayee
                //request.InPolicyLossPayee = new ExecuteRequestInPolicyLossPayee();
                //request.InPolicyLossPayee.ExplanationText = "";
                //request.InPolicyLossPayee.LossPayeeAmount = 0;
                //request.InPolicyLossPayee.LossPayeeContractNum = "";
                //request.InPolicyLossPayee.LossPayeeCurrencyCode = "TL";
                //request.InPolicyLossPayee.LossPayeeEndDate = "";
                //request.InPolicyLossPayee.LossPayeeId = "";
                //request.InPolicyLossPayee.LossPayeeRate = 0;
                //request.InPolicyLossPayee.LossPayeeStartDate = "";
                #endregion

                #region InGlobalInfo

                //request.InGlobalInfo = new ExecuteRequestInGlobalInfo();
                //request.InGlobalInfo.RaAndAgencySameFlag = "H";

                #endregion

                #region InPolicyPaymentInstallment
                //request.InPolicyPaymentInstallment = new ExecuteRequestInPolicyPaymentInstallment();
                //request.InPolicyPaymentInstallment.PartyId = "0";
                //request.InPolicyPaymentInstallment.FirstAccountCode = "";
                //request.InPolicyPaymentInstallment.FirstBankCode = "0";
                //request.InPolicyPaymentInstallment.FirstBranchCode = "0";
                //request.InPolicyPaymentInstallment.FirstCreditCardCode = "";
                //request.InPolicyPaymentInstallment.FirstCvcNum = "";
                //request.InPolicyPaymentInstallment.FirstExpireDate = "";
                #endregion

                //müsteri oluşturulduktan sonra
                #region InGListPolicyParty

                List<ExecuteRequestInGroupPolicyPartyRow> gruprow = new List<ExecuteRequestInGroupPolicyPartyRow>();
                List<ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty> gruplar = new List<ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty>();

                ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();

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
                grup.PartyId = EurekoMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 1; //Oran bilgisidir. "1" gönderilmelidir.
                gruplar.Add(grup);

                grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "SE";
                grup.PartyId = MusteriAyniMi ? EurekoMusteriNo : EurekoSEMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 0; //Oran bilgisidir. "0" gönderilmelidir.
                gruplar.Add(grup);

                grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "PO";
                grup.PartyId = MusteriAyniMi ? EurekoMusteriNo : EurekoSEMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
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

                #region InGListRiskQuestion

                var rqList = new List<ExecuteRequestInGroupRisqQuestionsRow>();

                #region Marka Model

                var rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.MarkaModel,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.MMAracDegerkodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.Marka + teklif.Arac.AracinTipi
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region Model Yılı

                //AracModel aracModel = _AracContext.AracModelRepository.Filter(s => s.MarkaKodu == teklif.Arac.Marka &&
                //s.Model == teklif.Arac.Model &&
                //s.TipKodu == teklif.Arac.AracinTipi).FirstOrDefault();
                //if (aracModel != null && aracModel.Fiyat.HasValue)
                //{
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.MarkaModel,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.MMAracUretimYili
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.Model.ToString()
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }
                //}

                #endregion

                #region Kullanım Tarzı

                //string EurekoKKTarzi = teklif.Arac.KullanimTarzi.PadLeft(2, '0');
                var sekilParts = teklif.Arac.KullanimTarzi.Split('-');
                var sekilKod = sekilParts[0];
                var sekilKod2 = sekilParts[1];

                var EurkeoArackullanimTarzi = _CRContext.CR_KullanimTarziRepository.Find(s => s.KullanimTarziKodu == sekilKod && s.Kod2 == sekilKod2 && s.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                var parts = EurkeoArackullanimTarzi.TarifeKodu.Split('-');


                // var eurekoKullanimSekli =  _AracContext.EurekoAracKullanimTarziRepository.All().Where(s => s.Id == sekil).FirstOrDefault();
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.KullanimTarzi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.KTAracKodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = parts[0] //eurekoKullanimSekli.AracTipKodu.ToString().PadLeft(2, '0')
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region Arac Kullanim Sekli

                string EurekoKKSekli = string.Empty;
                int _kullanimSekli = 0;
                int.TryParse(teklif.Arac.KullanimSekli, out _kullanimSekli);
                //var eurekoKullanimSekli = _AracContext.EurekoAracKullanimTarziRepository.All().Where(s => s.Id == _kullanimSekli).FirstOrDefault();
                //if (eurekoKullanimSekli != null)
                //{
                // EurekoKKSekli = eurekoKullanimSekli.AracKullanimSekliKodu;
                //}
                // else { EurekoKKSekli = "A"; }
                EurekoKKSekli = parts[1];
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.KullanimTarzi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.KTKullanimSekli
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = EurekoKKSekli
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region PlakaIl

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.PlakaIl,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.PlakaIl
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.PlakaKodu
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region PlakaDetay

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.PlakaDetay,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.PlakaDetay
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.PlakaNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region Koltuk Sayısı

                AracTip aracTip = _AracContext.AracTipRepository.FindById(new object[] { teklif.Arac.Marka, teklif.Arac.AracinTipi });
                var kisiSayisi = 0;
                if (aracTip != null)
                {
                    kisiSayisi = aracTip.KisiSayisi.Value;
                }
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.KoltukAdedi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.KoltukAdedi
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.KoltukSayisi > 0 ? teklif.Arac.KoltukSayisi.ToString() : kisiSayisi > 0 ? kisiSayisi.ToString() : "5"
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }


                #endregion

                #region TrafikTescilTarihi

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.TrafikTescilTarihi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.TrafikTescilTarihi
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TrafikTescilTarihi.HasValue ? ((DateTime)teklif.Arac.TrafikTescilTarihi).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy")
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region TrafikCikisTarihi

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.TrafikCikisTarihi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.TrafikCikisTarihi
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TrafikCikisTarihi.HasValue ? ((DateTime)teklif.Arac.TrafikCikisTarihi).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy")
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region SasiNo

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.SasiNo,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.SasiNo
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.SasiNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region MotorNo

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.MotorNo,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.MotorNo
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.MotorNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region RuhsatTescilKodu

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.RuhsatTescilKodu,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.RuhsatTescilKodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TescilSeriKod
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region RuhsatTescilRefNo

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.RuhsatTescilRefNo,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.RuhsatTescilRefNo
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TescilSeriNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region TariffLevelCode

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.TarifBasamakKodu,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.TarifBasamakKodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_TariffLevelCode, "4")
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region ÖPB

                var opb = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);

                #region IlkPoliçemi

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.IlkPoliceMi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.IlkPoliceMi_Detay
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = opb ? "H" : "E"
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                if (opb)
                {
                    #region SigortaSirketi

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_SigortaSirketi,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_SigortaSirketi_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion

                    #region Poliçe No

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_PoliceNo,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_PoliceNo_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_No, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion

                    #region Acente No

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_AcenteNo,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_AcenteNo_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion

                    #region Yenileme No

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_YenilemeNo,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_YenilemeNo_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion
                }

                #endregion

                request.InGroupRisqQuestions = rqList.ToArray();

                #endregion

                #region Policy Adres

                var policyAdres = new ExecuteRequestInPolicyAddress();
                policyAdres.PartyId = EurekoMusteriNo;
                if (sigortali.MusteriGenelBilgiler.KimlikNo.Length == 10)
                    policyAdres.AddressType = "I";
                else
                    policyAdres.AddressType = "E";

                policyAdres.AddressText = musteriAdres.Adres.Length > 100 ? musteriAdres.Adres.Substring(0, 100) : musteriAdres.Adres;
                policyAdres.CityCode = EurekoAdres == null ? EurekoAdres.CRIlKodu : "34";
                policyAdres.DistrictCode = EurekoAdres == null ? EurekoAdres.CRIlceKodu.ToString().PadLeft(2, '0') : "25";
                policyAdres.CountryCode = "TUR";
                policyAdres.VicinityVillageCode = "000"; //(musteriAdres.SemtKoyKodu ?? 0).ToString().PadLeft(3, '0');
                policyAdres.VicinityVillageType = "S";// (string.IsNullOrEmpty(musteriAdres.SemtKoyTipKodu)) ? "K" : musteriAdres.SemtKoyTipKodu;
                request.InPolicyAddress = policyAdres;

                #endregion

                #endregion

                #region Servis call

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Teklif);

                eurekosigorta.trafikV4.ExecuteResponseResponse response = trafikClient.Execute(request);
                trafikClient.Dispose();
                eureko.policeteklifbilgi.ExecuteResponseResponse InfoResponse = new eureko.policeteklifbilgi.ExecuteResponseResponse();
                bool InfoResonseSuccess = false;
                eureko.teminat.ExecuteResponseResponse teminatResponse = new eureko.teminat.ExecuteResponseResponse();
                bool teminatResponseSuccess = false;

                if (response.OutHeader.IsSuccessfull == "false")
                {
                    this.EndLog(response, false, response.GetType());
                    this.AddHata(response.OutHeader.ResponseMessage);
                }
                else
                {
                    this.EndLog(response, true, response.GetType());

                    #region GenInfoRequest

                    eureko.policeteklifbilgi.PolicyGenInfoReadV3Service InfoClient = new eureko.policeteklifbilgi.PolicyGenInfoReadV3Service();
                    KonfigTable konfigInfoClient = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOTeklifPoliceBilgi);
                    InfoClient.Url = konfigInfoClient[Konfig.EUREKO_PoliceTeklifBilgiServiceURL];
                    InfoClient.Timeout = 150000;

                    eureko.policeteklifbilgi.ExecuteRequest InfoRequest = new eureko.policeteklifbilgi.ExecuteRequest();


                    #region genInfoReadInHeader

                    InfoRequest.InHeader = new eureko.policeteklifbilgi.ExecuteRequestInHeader();
                    InfoRequest.InHeader.CompanyId = servisKullanici.CompanyId;
                    InfoRequest.InHeader.UserId = servisKullanici.KullaniciAdi;
                    InfoRequest.InHeader.Password = servisKullanici.Sifre;
                    InfoRequest.InHeader.PlatformType = (eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                    InfoRequest.InHeader.MessageId = "100";

                    #endregion

                    #region genInfoReadInPolicyMaster

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

                    #endregion

                    #region GenInfoResponse

                    this.BeginLog(InfoRequest, InfoRequest.GetType(), WebServisIstekTipleri.ESTeklifPoliceRead);

                    InfoResponse = InfoClient.Execute(InfoRequest);

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

                    #region Teminat Request

                    eureko.teminat.PolicyComponentReadService teminatClient = new eureko.teminat.PolicyComponentReadService();
                    KonfigTable konfigTeminatClient = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPolieTeminat);
                    teminatClient.Url = konfigTeminatClient[Konfig.EUREKO_PoliceTeminatServiceURL];
                    teminatClient.Timeout = 150000;

                    eureko.teminat.ExecuteRequest teminatRequest = new eureko.teminat.ExecuteRequest();
                    #region Teminat InHeader

                    teminatRequest.InHeader = new eureko.teminat.ExecuteRequestInHeader();
                    teminatRequest.InHeader.CompanyId = servisKullanici.CompanyId;
                    teminatRequest.InHeader.UserId = servisKullanici.KullaniciAdi;
                    teminatRequest.InHeader.Password = servisKullanici.Sifre;
                    teminatRequest.InHeader.PlatformType = (eureko.teminat.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eureko.teminat.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                    teminatRequest.InHeader.MessageId = "100";
                    #endregion

                    #region Teminat InPolicyMaster

                    teminatRequest.InPolicyMaster = new eureko.teminat.ExecuteRequestInPolicyMaster();
                    teminatRequest.InPolicyMaster.CompanyCode = response.OutPolicyMaster.CompanyCode;
                    teminatRequest.InPolicyMaster.PolicyGroupNum = response.OutPolicyMaster.PolicyGroupNum;
                    teminatRequest.InPolicyMaster.PolicyNum = response.OutPolicyMaster.PolicyNum;
                    teminatRequest.InPolicyMaster.RenewalNum = response.OutPolicyMaster.RenewalNum;
                    teminatRequest.InPolicyMaster.EndorsementNum = response.OutPolicyMaster.EndorsementNum;
                    teminatRequest.InPolicyMaster.InternalEndorsementNum = response.OutPolicyMaster.InternalEndorsementNum;
                    teminatRequest.InPolicyMaster.ChangeSequenceNum = response.OutPolicyMaster.ChangeSequenceNum;
                    #endregion

                    #endregion

                    #region Teminat Response

                    this.BeginLog(teminatRequest, teminatRequest.GetType(), WebServisIstekTipleri.ESTeklifPoliceteminatRead);

                    teminatResponse = teminatClient.Execute(teminatRequest);
                    teminatClient.Dispose();
                    if (teminatResponse.OutHeader.IsSuccessfull == "false")
                    {
                        this.EndLog(teminatResponse, false, teminatResponse.GetType());
                        this.AddHata(teminatResponse.OutHeader.ResponseMessage);
                    }
                    else
                    {
                        this.EndLog(teminatResponse, true, teminatResponse.GetType());
                        teminatResponseSuccess = true;
                    }

                    #endregion


                }

                #endregion

                #region Basarılı Kontrol
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

                this.GenelBilgiler.TUMTeklifNo = response.OutPolicyMaster.PolicyNum;
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;


                decimal GrfTl = 0;
                decimal ThfTl = 0;
                decimal YsvTl = 0;
                decimal GdvTl = 0;
                if (InfoResonseSuccess)
                {

                    this.GenelBilgiler.BrutPrim = InfoResponse.OutPolicyMaster.TotalGrossPremiumAmount;
                    this.GenelBilgiler.NetPrim = InfoResponse.OutPolicyMaster.TotalNetPremiumAmount;
                    this.GenelBilgiler.ToplamKomisyon = InfoResponse.OutPolicyMaster.AgencyTotalCommissionAmount;
                    this.GenelBilgiler.ToplamVergi = InfoResponse.OutPolicyMaster.TotalTaxAmount;
                    this.GenelBilgiler.DovizKurBedeli = InfoResponse.OutPolicyMaster.ExchangeRate;
                    GrfTl = InfoResponse.OutComponentTaxTotals.GrfTl;
                    ThfTl = InfoResponse.OutComponentTaxTotals.ThfTl;
                    YsvTl = InfoResponse.OutComponentTaxTotals.YsvTl;
                    GdvTl = InfoResponse.OutComponentTaxTotals.GdvTl;
                }





                //this.GenelBilgiler.TaksitSayisi = 1;
                //this.GenelBilgiler.GecikmeZammiYuzdesi = RAYMessages.ToDecimal(RAY_GecikmeOrani);
                //this.GenelBilgiler.ZKYTMSYüzdesi = RAYMessages.ToDecimal(RAY_ZKYTMSIndirimi);


                // ==== Güncellenicek. ==== //
                //this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                //this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                // Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                #endregion

                //trafikvergilerde ysv yok ??
                #region Vergiler
                this.AddVergi(TrafikVergiler.THGFonu, ThfTl);
                this.AddVergi(TrafikVergiler.GiderVergisi, GdvTl);
                this.AddVergi(TrafikVergiler.GarantiFonu, GrfTl);
                //this.AddVergi(TrafikVergiler.YanginVergisi, YsvTl);
                #endregion

                //tüm teminatlar bunlar mı??
                #region Teminatlar
                if (teminatResponseSuccess)
                {
                    decimal tutar = 0;
                    decimal brutPrim = 0;
                    decimal netPrim = 0;
                    decimal vergi = 0;

                    var ucuncuSahisMaddiZararlar = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.UcuncuSahisMaddiZararlar);

                    if (ucuncuSahisMaddiZararlar != null)
                    {
                        tutar = ucuncuSahisMaddiZararlar.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = ucuncuSahisMaddiZararlar.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = ucuncuSahisMaddiZararlar.OutPolicyTax.TaxAmount;
                        brutPrim = ucuncuSahisMaddiZararlar.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    var UcuncuSahisBedeniZararlar = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.UcuncuSahisBedeniZararlar);

                    if (UcuncuSahisBedeniZararlar != null)
                    {
                        tutar = UcuncuSahisBedeniZararlar.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = UcuncuSahisBedeniZararlar.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = UcuncuSahisBedeniZararlar.OutPolicyTax.TaxAmount;
                        brutPrim = UcuncuSahisBedeniZararlar.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    var UcuncuSahisTedaviMasrafları = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.UcuncuSahisTedaviMasraflari);

                    if (UcuncuSahisTedaviMasrafları != null)
                    {
                        tutar = UcuncuSahisTedaviMasrafları.OutPolicySumInsured.SumInsuredAmount;
                        netPrim = UcuncuSahisTedaviMasrafları.OutPolicySumInsured.AccruePremiumAmount;
                        vergi = UcuncuSahisTedaviMasrafları.OutPolicyTax.TaxAmount;
                        brutPrim = UcuncuSahisTedaviMasrafları.OutGrossPolicySumInsured.AccruePremiumAmount;
                        this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, tutar, vergi, netPrim, brutPrim, 0);
                    }

                    //var SGK = teminatResponse.OutGroup.FirstOrDefault(f => f.OutPolicySumInsured.CoverageMatrixNum == EUREKO_TrafikTeminatlar.SGK);

                    //if (SGK != null)
                    //{
                    //    tutar = SGK.OutPolicySumInsured.SumInsuredAmount;
                    //    netPrim = SGK.OutPolicySumInsured.AccruePremiumAmount;
                    //    vergi = SGK.OutPolicyTax.TaxAmount;
                    //    brutPrim = SGK.OutGrossPolicySumInsured.AccruePremiumAmount;
                    //    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, tutar, vergi, netPrim, brutPrim, 0);
                    //}

                }

                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                #endregion

                #region Web servis cevapları

                this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_ChangeSeqNum, response.OutPolicyMaster.ChangeSequenceNum);
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


                #endregion



                #endregion


                var url = GetPDFAndBilgilendirmePDF(true, response.OutPolicyMaster.ChangeSequenceNum, response.OutPolicyMaster.PolicyNum, response.OutPolicyMaster.PolicyGroupNum, response.OutPolicyMaster.RenewalNum, response.OutPolicyMaster.EndorsementNum, response.OutPolicyMaster.InternalEndorsementNum);
                this.GenelBilgiler.PDFDosyasi = url.pdfURL;
                _Log.Info("Teklif_PDF url: {0}", this.GenelBilgiler.PDFDosyasi);


            }
            catch (Exception ex)
            {
                #region Hata Log

                trafikClient.Abort();
                this.Import(teklif);
                this.GenelBilgiler.Basarili = false;

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void Policelestir(Odeme odeme)
        {
            ProposalAndPolicySrV4Service trafikClient = new ProposalAndPolicySrV4Service();

            try
            {
                #region Veri Hazırlama GENEL
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                var sigortaEttiren = teklif.SigortaEttiren;
                #endregion

                #region Policelestirme
                              
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleEurekoTrafik);
                KonfigTable konfigPlatformType = _KonfigurasyonService.GetKonfig(Konfig.BundleEUREKOPlatformType);
                trafikClient.Url = konfig[Konfig.EUREKO_TrafikServiceURL];
                trafikClient.Timeout = 150000;

                eurekosigorta.trafikV4.ExecuteRequest requestP = new eurekosigorta.trafikV4.ExecuteRequest();
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.EUREKO });
                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                string SEMusteriNo = ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaEttirenMusteriNo, "0");
                string MusteriNo = ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_SigortaliMusteriNo, "0");
                var TUMTeklif = _TeklifService.GetTeklifListe(teklif.TeklifNo, teklif.GenelBilgiler.TVMKodu).FirstOrDefault(s => s.GenelBilgiler.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                #region Veri Hazırlama

                #region InHeader

                requestP.InHeader = new eurekosigorta.trafikV4.ExecuteRequestInHeader();
                requestP.InHeader.CompanyId = servisKullanici.CompanyId;
                requestP.InHeader.UserId = servisKullanici.KullaniciAdi;
                requestP.InHeader.Password = servisKullanici.Sifre;
                requestP.InHeader.PlatformType = (eurekosigorta.trafikV4.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eurekosigorta.trafikV4.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]);
                requestP.InHeader.MessageId = "100";

                #endregion

                #region InOperationGlobalInfo

                requestP.InOperationGlobalInfo = new ExecuteRequestInOperationGlobalInfo();
                requestP.InOperationGlobalInfo.OperationCode = "POLICY";

                #endregion

                #region InPolicyMaster

                requestP.InPolicyMaster = new eurekosigorta.trafikV4.ExecuteRequestInPolicyMaster();
                requestP.InPolicyMaster.AgencyCode = servisKullanici.PartajNo_;
                requestP.InPolicyMaster.SubAgencyCode = servisKullanici.SubAgencyCode;
                requestP.InPolicyMaster.SourceId = servisKullanici.SourceId;
                requestP.InPolicyMaster.ProductCode = "T10";
                requestP.InPolicyMaster.TariffCode = "10";
                requestP.InPolicyMaster.PolicyStartDate = polBaslangic.ToString("yyyyMMdd");
                requestP.InPolicyMaster.PolicyEndDate = polBaslangic.AddYears(1).ToString("yyyyMMdd");
                requestP.InPolicyMaster.ProcessingDate = polBaslangic.ToString("yyyyMMdd");
                requestP.InPolicyMaster.CurrencyCode = "TL";
                requestP.InPolicyMaster.CompanyCode = "GS";

                if (odeme.OdemeSekli == OdemeSekilleri.Pesin) requestP.InPolicyMaster.PaymentType = "AL5";
                else
                {
                    if (odeme.TaksitSayisi == 2)
                        requestP.InPolicyMaster.PaymentType = "AL3";
                    else if (odeme.TaksitSayisi == 5)
                        requestP.InPolicyMaster.PaymentType = "A11";
                    else if (odeme.TaksitSayisi == 8)
                        requestP.InPolicyMaster.PaymentType = "A67";
                    else if (odeme.TaksitSayisi == 9)
                        requestP.InPolicyMaster.PaymentType = "KK9";

                }

                #endregion

                #region OPERATIONCODE-POLICY

                requestP.InPolicyMaster.ExternalKeyText = TUMTeklif.GenelBilgiler.TUMTeklifNo;
                requestP.InPolicyMaster.PolicyGroupNum = "0";
                requestP.InPolicyMaster.PolicyNum = "0";
                requestP.InPolicyMaster.RenewalNum = "0";
                requestP.InPolicyMaster.EndorsementNum = "0";
                requestP.InPolicyMaster.InternalEndorsementNum = "0";
                requestP.InPolicyMaster.ChangeSequenceNum = "0";

                #endregion

                #region InPolicyLossPayee

                //requestP.InPolicyLossPayee = new ExecuteRequestInPolicyLossPayee();
                //request.InPolicyLossPayee.ExplanationText = "";
                //request.InPolicyLossPayee.LossPayeeAmount = 0;
                //request.InPolicyLossPayee.LossPayeeContractNum = "";
                //requestP.InPolicyLossPayee.LossPayeeCurrencyCode = "TL";
                //request.InPolicyLossPayee.LossPayeeEndDate = "";
                //request.InPolicyLossPayee.LossPayeeId = "";
                //request.InPolicyLossPayee.LossPayeeRate = 0;
                //request.InPolicyLossPayee.LossPayeeStartDate = "";

                #endregion

                #region InGlobalInfo

                requestP.InGlobalInfo = new ExecuteRequestInGlobalInfo();
                requestP.InGlobalInfo.RaAndAgencySameFlag = "H";

                #endregion

                #region InPolicyPaymentInstallment

                requestP.InPolicyPaymentInstallment = new ExecuteRequestInPolicyPaymentInstallment();
                requestP.InPolicyPaymentInstallment.PartyId = "0";
                requestP.InPolicyPaymentInstallment.FirstBankCode = "0";
                requestP.InPolicyPaymentInstallment.FirstBranchCode = "0";
                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    requestP.InPolicyPaymentInstallment.PartyId = (Convert.ToInt32(SEMusteriNo) > 0) ? SEMusteriNo : MusteriNo;
                    requestP.InPolicyPaymentInstallment.FirstCreditCardCode = odeme.KrediKarti.KartNo;
                    requestP.InPolicyPaymentInstallment.FirstCvcNum = odeme.KrediKarti.CVC;
                    requestP.InPolicyPaymentInstallment.FirstExpireDate = odeme.KrediKarti.SKA + odeme.KrediKarti.SKY.Substring(2, 2);

                }
                #endregion

                #region InGListPolicyParty

                List<ExecuteRequestInGroupPolicyPartyRow> gruprow = new List<ExecuteRequestInGroupPolicyPartyRow>();
                List<ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty> gruplar = new List<ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty>();

                ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();

                //  Müşteri taraf bilgisidir. Sigortalı -"SG" , Sigorta Ettiren "SE", Prim Ödeyen-"PE" olarak girilmelidir.
                grup.PartyType = "SG";
                grup.PartyId = MusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 1; //Oran bilgisidir. "1" gönderilmelidir.
                gruplar.Add(grup);

                grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "SE";
                grup.PartyId = SEMusteriNo; //Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
                grup.PartyRate = 0; //Oran bilgisidir. "0" gönderilmelidir.
                gruplar.Add(grup);

                grup = new ExecuteRequestInGroupPolicyPartyRowInGListPolicyParty();
                grup.PartyType = "PO";
                grup.PartyId = SEMusteriNo;//Müşteri oluşturma servisinden dönen ES Müşteri numarası girilmelidir.
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
                requestP.InGroupPolicyParty = gruprow.ToArray();

                #endregion

                #region InGListRiskQuestion

                var rqList = new List<ExecuteRequestInGroupRisqQuestionsRow>();

                #region Marka Model

                var rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.MarkaModel,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.MMAracDegerkodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.Marka + teklif.Arac.AracinTipi
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region Model Yılı

                //AracModel aracModel = _AracContext.AracModelRepository.Filter(s => s.MarkaKodu == teklif.Arac.Marka &&
                //s.Model == teklif.Arac.Model &&
                //s.TipKodu == teklif.Arac.AracinTipi).FirstOrDefault();
                //if (aracModel != null && aracModel.Fiyat.HasValue)
                //{
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.MarkaModel,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.MMAracUretimYili
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.Model.ToString()
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }
                //}

                #endregion

                #region Kullanım Tarzı

                //string EurekoKKTarzi = teklif.Arac.KullanimTarzi.PadLeft(2, '0');
                var sekilParts = teklif.Arac.KullanimTarzi.Split('-');
                var sekilKod = sekilParts[0];
                var sekilKod2 = sekilParts[1];

                var EurkeoArackullanimTarzi = _CRContext.CR_KullanimTarziRepository.Find(s => s.KullanimTarziKodu == sekilKod && s.Kod2 == sekilKod2 && s.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                var parts = EurkeoArackullanimTarzi.TarifeKodu.Split('-');


                // var eurekoKullanimSekli =  _AracContext.EurekoAracKullanimTarziRepository.All().Where(s => s.Id == sekil).FirstOrDefault();
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.KullanimTarzi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.KTAracKodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = parts[0] //eurekoKullanimSekli.AracTipKodu.ToString().PadLeft(2, '0')
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region Arac Kullanim Sekli

                string EurekoKKSekli = string.Empty;
                int _kullanimSekli = 0;
                int.TryParse(teklif.Arac.KullanimSekli, out _kullanimSekli);
                //var eurekoKullanimSekli = _AracContext.EurekoAracKullanimTarziRepository.All().Where(s => s.Id == _kullanimSekli).FirstOrDefault();
                //if (eurekoKullanimSekli != null)
                //{
                // EurekoKKSekli = eurekoKullanimSekli.AracKullanimSekliKodu;
                //}
                // else { EurekoKKSekli = "A"; }
                EurekoKKSekli = parts[1];
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.KullanimTarzi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.KTKullanimSekli
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = EurekoKKSekli
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region PlakaIl

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.PlakaIl,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.PlakaIl
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.PlakaKodu
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region PlakaDetay

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.PlakaDetay,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.PlakaDetay
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.PlakaNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region Koltuk Sayısı

                //AracTip aracTip = _AracContext.AracTipRepository.FindById(new object[] { teklif.Arac.Marka, teklif.Arac.AracinTipi });
                //if (aracTip != null)
                //{
                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.KoltukAdedi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.KoltukAdedi
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.KoltukSayisi.ToString()
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }
                //}

                #endregion

                #region TrafikTescilTarihi

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.TrafikTescilTarihi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.TrafikTescilTarihi
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TrafikTescilTarihi.HasValue ? ((DateTime)teklif.Arac.TrafikTescilTarihi).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy")
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region TrafikCikisTarihi

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.TrafikCikisTarihi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.TrafikCikisTarihi
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TrafikCikisTarihi.HasValue ? ((DateTime)teklif.Arac.TrafikCikisTarihi).ToString("dd/MM/yyyy") : DateTime.Now.ToString("dd/MM/yyyy")
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region SasiNo

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.SasiNo,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.SasiNo
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.SasiNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region MotorNo

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.MotorNo,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.MotorNo
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.MotorNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region RuhsatTescilKodu

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.RuhsatTescilKodu,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.RuhsatTescilKodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TescilSeriKod
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region RuhsatTescilRefNo

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.RuhsatTescilRefNo,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.RuhsatTescilRefNo
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = teklif.Arac.TescilSeriNo
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region TariffLevelCode

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.TarifBasamakKodu,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.TarifBasamakKodu
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = ReadWebServisCevap(Common.WebServisCevaplar.EUREKO_TariffLevelCode, "4")
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                #region ÖPB

                var opb = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);

                #region IlkPoliçemi

                rq = new ExecuteRequestInGroupRisqQuestionsRow()
                {
                    InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                    {
                        RiskQuestionNum = EUREKO_TrafikSoruTipNo.IlkPoliceMi,
                        RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.IlkPoliceMi_Detay
                    },
                    InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                    {
                        RqValue = opb ? "H" : "E"
                    }
                };
                if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                {
                    rqList.Add(rq);
                }

                #endregion

                if (opb)
                {
                    #region SigortaSirketi

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_SigortaSirketi,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_SigortaSirketi_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion

                    #region Poliçe No

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_PoliceNo,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_PoliceNo_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_No, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion

                    #region Acente No

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_AcenteNo,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_AcenteNo_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion

                    #region Yenileme No

                    rq = new ExecuteRequestInGroupRisqQuestionsRow()
                    {
                        InGListRiskQuestion = new ExecuteRequestInGroupRisqQuestionsRowInGListRiskQuestion()
                        {
                            RiskQuestionNum = EUREKO_TrafikSoruTipNo.OPB_YenilemeNo,
                            RiskDetailQuestionNum = EUREKO_TrafikSoruTipDetayNo.OPB_YenilemeNo_Detay
                        },
                        InGListGlobalInfo = new ExecuteRequestInGroupRisqQuestionsRowInGListGlobalInfo()
                        {
                            RqValue = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, string.Empty)
                        }
                    };
                    if (!string.IsNullOrEmpty(rq.InGListGlobalInfo.RqValue))
                    {
                        rqList.Add(rq);
                    }

                    #endregion
                }

                #endregion

                requestP.InGroupRisqQuestions = rqList.ToArray();

                #endregion

                #region Policy Adres

                var policyAdres = new ExecuteRequestInPolicyAddress();
                policyAdres.PartyId = MusteriNo;
                if (sigortali.KimlikNo.Length == 10)
                    policyAdres.AddressType = "I";
                else
                    policyAdres.AddressType = "E";
                MusteriGenelBilgiler musteriBilgileri = _MusteriService.GetMusteri(sigortali.MusteriKodu);
                MusteriTelefon musteriTelefon = musteriBilgileri.MusteriTelefons.FirstOrDefault(f => f.MusteriKodu == sigortali.MusteriKodu);
                MusteriAdre musteriAdres = musteriBilgileri.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);


                var EurekoAdres = _CRContext.CR_IlIlceRepository.Find(s => s.IlKodu == musteriAdres.IlKodu && s.IlceKodu == musteriAdres.IlceKodu && s.TUMKodu == TeklifUretimMerkezleri.EUREKO);

                policyAdres.AddressText = musteriAdres.Adres.Length > 100 ? musteriAdres.Adres.Substring(0, 100) : musteriAdres.Adres;
                policyAdres.CityCode = EurekoAdres == null ? EurekoAdres.CRIlKodu : "34";
                policyAdres.DistrictCode = EurekoAdres == null ? EurekoAdres.CRIlceKodu.ToString().PadLeft(2, '0') : "25";
                policyAdres.CountryCode = "TUR";
                policyAdres.VicinityVillageCode = "000"; //(musteriAdres.SemtKoyKodu ?? 0).ToString().PadLeft(3, '0');
                policyAdres.VicinityVillageType = "S";// (string.IsNullOrEmpty(musteriAdres.SemtKoyTipKodu)) ? "K" : musteriAdres.SemtKoyTipKodu;
                requestP.InPolicyAddress = policyAdres;

                #endregion

                #endregion

                #endregion

                this.BeginLog(requestP, requestP.GetType(), WebServisIstekTipleri.Police);

                eurekosigorta.trafikV4.ExecuteResponseResponse responseP = trafikClient.Execute(requestP);
                trafikClient.Dispose();

                if (responseP.OutHeader.IsSuccessfull == "false")
                {
                    this.EndLog(responseP, false, responseP.GetType());
                    this.AddHata(responseP.OutHeader.ResponseMessage);
                }
                else
                {
                    this.EndLog(responseP, true, responseP.GetType());

                    this.AddWebServisCevap(Common.WebServisCevaplar.EUREKO_ChangeSeqNum, responseP.OutPolicyMaster.ChangeSequenceNum);
                    this.GenelBilgiler.TUMPoliceNo = responseP.OutPolicyMaster.PolicyNum;
                    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                    var url = GetPDFAndBilgilendirmePDF(false, responseP.OutPolicyMaster.ChangeSequenceNum, responseP.OutPolicyMaster.PolicyNum,
                              responseP.OutPolicyMaster.PolicyGroupNum, responseP.OutPolicyMaster.RenewalNum, responseP.OutPolicyMaster.EndorsementNum,
                              responseP.OutPolicyMaster.InternalEndorsementNum);
                    this.GenelBilgiler.PDFDosyasi = url.pdfURL;
                    this.GenelBilgiler.PDFBilgilendirme = url.bilgilendirmeURL;
                    _Log.Info("Police_PDF url: {0}", this.GenelBilgiler.PDFDosyasi);
                    _Log.Info("Police_Bilgilendirme_PDF url: {0}", this.GenelBilgiler.PDFBilgilendirme);
                    var cevap = this.GenelBilgiler.TeklifWebServisCevaps.Where(c => c.CevapKodu == Common.WebServisCevaplar.EUREKO_PolicyGroupNum).FirstOrDefault();
                    if (cevap == null)
                    {
                        cevap = new TeklifWebServisCevap();
                        cevap.CevapKodu = Common.WebServisCevaplar.EUREKO_PolicyGroupNum;
                        cevap.Cevap = responseP.OutPolicyMaster.PolicyGroupNum;
                        cevap.CevapTipi = SoruCevapTipleri.Metin;
                        this.GenelBilgiler.TeklifWebServisCevaps.Add(cevap);
                    }
                    else
                    {
                        cevap.Cevap = responseP.OutPolicyMaster.PolicyGroupNum;
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

                    var header = new eureko.policeteklifbilgi.ExecuteRequestInHeader()
                    {
                        CompanyId = servisKullanici.CompanyId,
                        UserId = servisKullanici.KullaniciAdi,
                        Password = servisKullanici.Sifre,
                        PlatformType = (eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType)Enum.Parse(typeof(eureko.policeteklifbilgi.ExecuteRequestInHeaderPlatformType), konfigPlatformType[Konfig.EUREKO_PlatformType]),
                        MessageId = "100"
                    };
                    infRequest.InHeader = header;

                    #endregion

                    #region PolicyMaster

                    var policyMaster = new eureko.policeteklifbilgi.ExecuteRequestInPolicyMaster()
                    {
                        CompanyCode = "GS",
                        PolicyGroupNum = "0",
                        PolicyNum = responseP.OutPolicyMaster.PolicyNum,
                        RenewalNum = "0",
                        EndorsementNum = "0",
                        InternalEndorsementNum = "0",
                        ChangeSequenceNum = responseP.OutPolicyMaster.ChangeSequenceNum,
                        AgencyCode = servisKullanici.PartajNo_,
                        SubAgencyCode = servisKullanici.SubAgencyCode,
                        SourceId = servisKullanici.SourceId
                    };
                    infRequest.InPolicyMaster = policyMaster;

                    #endregion

                    #region Response

                    this.BeginLog(infRequest, infRequest.GetType(), WebServisIstekTipleri.ESTeklifPoliceRead);

                    var infResponse = infClient.Execute(infRequest);
                    infClient.Dispose();
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

                        string PolNo = responseP.OutPolicyMaster.PolicyNum; ;
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

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
            }
            catch (Exception ex)
            {
                #region Hata Log

                trafikClient.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
                #endregion
            }
        }

        public override void DekontPDF()
        {
            try
            {

            }
            catch (Exception ex)
            {
                #region Hata Log
                _Log.Error(ex);
                this.EndLog(ex.Message, false);
                throw;
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
    }
}
