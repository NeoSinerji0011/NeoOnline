using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.unicosecurity;
using Neosinerji.BABOnlineTP.Business.unicosigorta.previva;
using Neosinerji.BABOnlineTP.Business.unicosigorta.utilityService;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;

namespace Neosinerji.BABOnlineTP.Business.UNICO.Kasko
{
    public class UNICOKasko : Teklif, IUNICOKasko
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
        public UNICOKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext,
            IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService tVMService)

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
            _TVMService = tVMService;
        }

        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.UNICO;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleUnico);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.UNICO });

                #region Unico ApiKey Üret
                string appSecurityKey = "";
                string authenticationKey = "";
                if (teklif.GenelBilgiler.KaydiEKleyenTVMKodu.HasValue)
                {
                    _TVMService.UnicoApiKeyVarMi(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, out appSecurityKey, out authenticationKey);
                }

                if (String.IsNullOrEmpty(authenticationKey))
                {
                    SecurityService unicosecurity = new SecurityService();
                    unicosecurity.Url = "https://previva.unicosigorta.com.tr/Public/SecurityService.svc";// konfig[Konfig.Unico_SecurityServiceURL];
                    unicosecurity.Timeout = 150000;
                    unicosecurity.BaseEntity entity = new unicosecurity.BaseEntity();
                    SecurityServiceRequest request = new SecurityServiceRequest();
                    request.appSecurityKey = appSecurityKey;
                    request.KullaniciAdi = servisKullanici.KullaniciAdi;
                    request.Sifre = servisKullanici.Sifre;
                    //request.entity = entity;
                    this.BeginLog(request, request.GetType(), WebServisIstekTipleri.UnicoSecurityKey);

                    //------------------Konfigurasyon tablosundan çekilecek Genç Brokerlık Kullanıcı Bilgileri
                    var securityResponse = unicosecurity.GetAuthenticationKey(appSecurityKey, servisKullanici.KullaniciAdi, servisKullanici.Sifre, ref entity);
                    unicosecurity.Dispose();

                    if (entity.IsSuccessful)
                    {
                        authenticationKey = securityResponse;
                        this.EndLog(securityResponse, true, securityResponse.GetType());
                    }
                    else
                    {
                        if (securityResponse != null)
                        {
                            this.EndLog(securityResponse, false, securityResponse.GetType());
                        }
                        else
                        {
                            this.EndLog(entity, false, entity.GetType());
                        }

                        this.AddHata(entity.AlertText);
                    }
                }
                else
                {
                    authenticationKey = appSecurityKey;
                }
                #endregion

                #region Unico Araç Bilgisi Sor

                UtilityService utilityService = new UtilityService();
                utilityService.Url = "https://previva.unicosigorta.com.tr/Public/UtilityService.svc";// konfig[Konfig.Unico_UtilityServiceURL];
                utilityService.Timeout = 150000;
                string productCode = UnicoUrunKodlari.HususiKasko;


                string plate = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                string plateType = "";
                string queryInput = "";
                string queryInputType = "";
                string renewFirmCode = "";
                string renewAgentCode = "";
                string renewPolicyNo = "";
                string renewNo = "";
                string registrySerial = "";
                string registryNo = "";
                string mark = "";
                string model = "";
                string modelYear = "";
                string licencePlate = "";
                string insuredByNo = "";
                string insuredByName = "";
                string insuredNo = "";
                string insuredName = "";
                DateTime? eDate = null;
                int alertNo = 0;
                string alertString = "";

                if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod))
                {
                    registrySerial = teklif.Arac.TescilSeriKod;
                    registryNo = teklif.Arac.TescilSeriNo;
                }
                else if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                {
                    registryNo = teklif.Arac.AsbisNo;
                }
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                //  MusteriAdre sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);

                if (!String.IsNullOrEmpty(sigortali.KimlikNo))
                {
                    queryInput = sigortali.KimlikNo;
                    if (sigortali.KimlikNo.Length == 11)
                    {
                        queryInputType = UnicoKimlikTipleri.Sahis;
                    }
                    else if (sigortali.KimlikNo.Length == 10)
                    {
                        queryInputType = UnicoKimlikTipleri.Tuzel;
                        productCode = UnicoUrunKodlari.TicariKasko;
                    }
                }
                bool QueryOnlinePolicyResult = false;

                List<KeyValuePairOfstringstring> statList = new List<KeyValuePairOfstringstring>();
                KeyValuePairOfstringstring statItem = new KeyValuePairOfstringstring();
                statItem.key = "";
                statItem.value = "";
                statList.Add(statItem);
                KeyValuePairOfstringstring[] stats = statList.ToArray();
                KeyValuePairOfstringstring[] informations = statList.ToArray();

                utilityService.QueryOnlinePolicy(authenticationKey, productCode, plate, plateType, ref queryInput, ref queryInputType, ref renewFirmCode, ref renewAgentCode,
                                                 ref renewPolicyNo, ref renewNo, registrySerial, registryNo, ref mark, ref model, ref modelYear, ref licencePlate,
                                                 ref insuredByNo, ref insuredByName, ref insuredNo, ref insuredName, ref eDate, ref stats, ref informations,
                                                 ref alertNo, ref alertString, out QueryOnlinePolicyResult, out QueryOnlinePolicyResult);
                //utilityService.Dispose(); //stats in yerine direkt null yazamıyorum
                #endregion

                PolicyService client = new PolicyService();
                client.Url = "https://previva.unicosigorta.com.tr/Public/PolicyService.svc";// konfig[Konfig.Unico_PolicyServiceURL];
                client.Timeout = 150000;

                #region Unico Müşteri Sorgulama
                bool Musteri = false;
                string UnicoMusteriNo = "";
                string verifiedGSM = "";
                GetCustomerNo getCustomerNo = new GetCustomerNo();
                getCustomerNo.QueryInput = sigortali.KimlikNo;

                if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                {
                    getCustomerNo.QueryType = Convert.ToByte(UnicoKimlikTipleri.Sahis);
                }
                else
                {
                    getCustomerNo.QueryType = Convert.ToByte(UnicoKimlikTipleri.Tuzel);
                }
                this.BeginLog(getCustomerNo, getCustomerNo.GetType(), WebServisIstekTipleri.MusteriKayit);

                client.GetCustomerNo(authenticationKey, appSecurityKey, verifiedGSM, ref getCustomerNo, out Musteri);

                if (Musteri)
                {
                    this.EndLog(getCustomerNo, true, getCustomerNo.GetType());
                    UnicoMusteriNo = getCustomerNo.CustomerNo;
                }
                #endregion

                #region Unico Önceki Poliçe Bilgi 
                string oncekipolno = "";
                string oncekisigortasirketi = "";
                string oncekiacenteno = "";
                string oncekiyenilemeno = "";
                if (QueryOnlinePolicyResult && !String.IsNullOrEmpty(renewPolicyNo))
                {
                    oncekipolno = renewPolicyNo;
                    oncekisigortasirketi = renewFirmCode;
                    oncekiacenteno = renewAgentCode;
                    oncekiyenilemeno = renewNo;
                }

                #endregion

                #region Poliçe Yenileme ise / İstatistikler
                EXT_WS_ISTDEG_REC statistic = new EXT_WS_ISTDEG_REC();
                List<EXT_WS_ISTDEG_REC> statistics = new List<EXT_WS_ISTDEG_REC>();
                Policy1 policy = new Policy1();
                policy.AppSecurityKey = appSecurityKey;
                string hkKademesi = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, String.Empty);
                if (!String.IsNullOrEmpty(oncekipolno))
                {
                    policy.QueryRenewFirmNo = oncekisigortasirketi;
                    policy.QueryRenewAgentNo = oncekiacenteno;
                    policy.QueryRenewPolicyNo = oncekipolno;
                    policy.QueryRenewNo = oncekiyenilemeno;

                    policy.QueryLicencePlate = licencePlate;
                    policy.QueryRegistrySerial = registrySerial;
                    policy.QueryRegistryNo = registryNo;
                    policy.QueryIdentityInput = queryInput;
                    policy.QueryIdentityType = queryInputType;
                    policy.SelectedTAHSILAT_KOD = "PESIN";

                    for (int i = stats.Count(); i <= 0; i--)
                    {
                        statistic = new EXT_WS_ISTDEG_REC();
                        statistic.DEG_KOD = stats[i].value;
                        statistic.IST_KOD = stats[i].key;
                        statistics.Add(statistic);
                    }
                    policy.Statistics = statistics.ToArray();
                }
                //else
                //{
                //Zorunlu İstatistik Default Değerler
                string hasarsizlikKorumaKlozuDgrKodu = "003";//HAYIR
                string immDgrKodu = "001";//20-60-20
                string KilitBedeliDgrKodu = "003";//800TL
                string KiralikAracMiDgrKodu = "000";//HAYIR
                string DepremSelDgrKodu = "002"; //DEPREM SEL MUAFİYETSİZ
                string ManeviTazminatDgrKodu = "H";//HAYIR
                string SurucuKursuDgrKodu = "000"; //HAYIR
                string SurucuSayisiDgrKodu = "001"; //1
                string IkameAracSecimiDgrKodu = "001"; //2X7 GÜN STANDART
                string ServisSartiDgrKodu = "001"; //SERBEST
                string GenisletilmisCam = "003"; //HAYIR
                string HukusalKorumaLimiti = "001"; //HAYIR
                string KaskoMuafiyeti = "002"; //YOK
                string TEBUyesiMi = "002"; //HAYIR
                string TekSurucuMu = "001"; //HAYIR

                string servisTuru = teklif.ReadSoru(KaskoSorular.Servis_Turu, String.Empty);
                if (servisTuru == KaskoServisTurleri.Anlasmali_Ozel_Servis) ServisSartiDgrKodu = ServisSartlari.AnlasmaliOzelServis;
                else if (servisTuru == KaskoServisTurleri.AvantajServisAgi) ServisSartiDgrKodu = ServisSartlari.AvantajServisAgi;
                else if (servisTuru == KaskoServisTurleri.Tamirhane) ServisSartiDgrKodu = ServisSartlari.Tamirhane;
                else if (servisTuru == KaskoServisTurleri.AnlasmaliServis) ServisSartiDgrKodu = ServisSartlari.AnlasmaliServis;

                string kemirgenHayvanZarari = "001";
                bool hayvanZarari = teklif.ReadSoru(KaskoSorular.Hayvanlarin_Verecegi_Zarar_ZarYok, false);
                if (hayvanZarari)
                {
                    kemirgenHayvanZarari = "000";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = kemirgenHayvanZarari;
                statistic.IST_KOD = UNICO_Istatistikler.KEMIRGEN_HASARI;
                statistics.Add(statistic);

                string engelliAraci = "0";
                bool engelliAracimi = teklif.ReadSoru(KaskoSorular.EngelliAracimi, false);
                if (engelliAracimi)
                {
                    engelliAraci = "1";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = engelliAraci.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.ENGELLI_ARACIMI;
                statistics.Add(statistic);

                bool GenisletilmisCamMi = teklif.ReadSoru(KaskoSorular.UnicoGenisletilmisCam, false);
                if (GenisletilmisCamMi)
                {
                    GenisletilmisCam = "2";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = GenisletilmisCam.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.GENISLETILMIS_CAM;
                statistics.Add(statistic);

                //Default Değer Gönderilenler

                bool hasarsizlikKorumaKlz = teklif.ReadSoru(KaskoSorular.UnicoHasarsizlikKorumaKlozu, false);
                if (hasarsizlikKorumaKlz)
                {
                    hasarsizlikKorumaKlozuDgrKodu = "2";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = hasarsizlikKorumaKlozuDgrKodu.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.HASARSIZLIK_KORUMA_KLOZU;
                statistics.Add(statistic);

                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = immDgrKodu;
                statistic.IST_KOD = UNICO_Istatistikler.IMM_BEDENI_MADDI;
                statistics.Add(statistic);

                string kilitBedelDegeri = teklif.ReadSoru(KaskoSorular.UnicokilitBedeli, KilitBedeliDgrKodu);
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = kilitBedelDegeri.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.KILIT_BEDELI;
                statistics.Add(statistic);

                string kaskoMuafiyeti = teklif.ReadSoru(KaskoSorular.UnicoKaskoMuafiyeti, KaskoMuafiyeti);
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = kaskoMuafiyeti.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.KASKO_MUAFIYETI;
                statistics.Add(statistic);

                bool kiralikAracMi = teklif.ReadSoru(KaskoSorular.UnicoKiralikAraciMi, false);
                if (kiralikAracMi)
                {
                    KiralikAracMiDgrKodu = "1";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = KiralikAracMiDgrKodu;
                statistic.IST_KOD = UNICO_Istatistikler.KIRALIK_ARAC_MI;
                statistics.Add(statistic);

                bool tekSurucuMu = teklif.ReadSoru(KaskoSorular.UnicoTekSurucuMu, false);
                if (tekSurucuMu)
                {
                    TekSurucuMu = "2";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = TekSurucuMu;
                statistic.IST_KOD = UNICO_Istatistikler.TEK_SURUCU_MU;
                statistics.Add(statistic);

                DepremSelDgrKodu = teklif.ReadSoru(KaskoSorular.UnicoDepremSelMuafiyeti, DepremSelDgrKodu);
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = DepremSelDgrKodu.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.DEP_SEL_MUAFIYETI;
                statistics.Add(statistic);

                bool TEBUyesimi = teklif.ReadSoru(KaskoSorular.UnicoTEBUyesiMi, false);
                if (TEBUyesimi)
                {
                    TEBUyesiMi = "1";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = TEBUyesiMi;
                statistic.IST_KOD = UNICO_Istatistikler.TEB_UYESIMI;
                statistics.Add(statistic);

                bool surucuKursuAraciMi = teklif.ReadSoru(KaskoSorular.UnicoSurucuKursuAraciMi, false);
                if (surucuKursuAraciMi)
                {
                    SurucuKursuDgrKodu = "1";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = SurucuKursuDgrKodu;
                statistic.IST_KOD = UNICO_Istatistikler.SURUCU_KURSU_ARACI_MI;
                statistics.Add(statistic);

                string surucuSayisi = teklif.ReadSoru(KaskoSorular.UnicoSurucuSayisi, SurucuSayisiDgrKodu);
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = surucuSayisi.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.SURUCU_SAYISI;
                statistics.Add(statistic);

                string IkameAracSecenegi = teklif.ReadSoru(KaskoSorular.UnicoIkameSecenegi, IkameAracSecimiDgrKodu);
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = IkameAracSecenegi.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.IKAME_ARAC_SECIMI;
                statistics.Add(statistic);

                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = ServisSartiDgrKodu;
                statistic.IST_KOD = UNICO_Istatistikler.SERVIS_SARTI;
                statistics.Add(statistic);

                ////Default Değer Olmayanlar
                //statistic = new EXT_WS_ISTDEG_REC();
                //var yurtDisi = teklif.ReadSoru(KaskoSorular.YurtdisiKasko, false);
                //if (yurtDisi)
                //{
                //    statistic.DEG_KOD = "000";//Evet
                //}
                //else
                //{
                //    statistic.DEG_KOD = "001";//Hayır
                //}
                //statistic.IST_KOD = UNICO_Istatistikler.YURT_DISI_TEM_VARMI;
                //statistics.Add(statistic);

                //if (yurtDisi)
                //{
                //    statistic.DEG_KOD = "002";//yurtdışı
                //}
                //else
                //{
                //    statistic.DEG_KOD = "001";//yurtiçi
                //}
                //statistic.IST_KOD = UNICO_Istatistikler.YURT_ICI_YURT_DISI;
                //statistics.Add(statistic);

                statistic = new EXT_WS_ISTDEG_REC();
                if (String.IsNullOrEmpty(mark))
                {
                    int aracMarka = Convert.ToInt32(teklif.Arac.Marka);
                    mark = aracMarka.ToString();
                }
                statistic.DEG_KOD = mark;
                statistic.IST_KOD = UNICO_Istatistikler.ARACIN_MARKASI;
                statistics.Add(statistic);

                statistic = new EXT_WS_ISTDEG_REC();
                bool yeniDegerKlozu = teklif.ReadSoru(KaskoSorular.UnicoYeniDegerklozu, false);
                if (yeniDegerKlozu)
                {
                    statistic.DEG_KOD = "002"; //Evet
                }
                else
                {
                    statistic.DEG_KOD = "003";//Hayır
                }

                statistic.IST_KOD = UNICO_Istatistikler.YENI_DEGER_KLOZU;
                statistics.Add(statistic);

                statistic = new EXT_WS_ISTDEG_REC();
                if (!String.IsNullOrEmpty(modelYear))
                {
                    statistic.DEG_KOD = modelYear;
                }
                else
                {
                    if (teklif.Arac.Model.HasValue)
                    {
                        if (teklif.Arac.Model >= 2000)
                        {
                            modelYear = teklif.Arac.Model.Value.ToString().Substring(1, 3);
                        }
                        else
                        {
                            modelYear = "0" + teklif.Arac.Model.Value.ToString().Substring(2, 2);
                        }
                    }
                    else
                    {
                        modelYear = "0";
                    }
                }
                statistic.DEG_KOD = modelYear;
                statistic.IST_KOD = UNICO_Istatistikler.MODEL_YILI;
                statistics.Add(statistic);
                string UnicoKullanimTarziKodu = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                string kullanimTarziKodu = String.Empty;
                string kod2 = String.Empty;
                if (parts.Length == 2)
                {
                    kullanimTarziKodu = parts[0];
                    kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.UNICO &&
                                                                                                    f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                    f.Kod2 == kod2)
                                                                                                    .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                        UnicoKullanimTarziKodu = kullanimTarzi.TarifeKodu;
                }

                statistic = new EXT_WS_ISTDEG_REC();
                if (!String.IsNullOrEmpty(model))
                {
                    statistic.DEG_KOD = model;
                }
                else
                {
                    string aracTipi = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                    if (String.IsNullOrEmpty(model))
                    {
                        var aracTipList = utilityService.GetListSource(authenticationKey, "MODEL", ref alertNo, ref alertString, UnicoKullanimTarziKodu, mark, modelYear, "444", null, null);
                        foreach (var item in aracTipList.Data)
                        {
                            if (item.value.Contains(aracTipi))
                            {
                                statistic.DEG_KOD = item.key;
                                break;
                            }
                        }
                    }
                }
                statistic.IST_KOD = UNICO_Istatistikler.ARACIN_TIPI;
                statistics.Add(statistic);


                if (!String.IsNullOrEmpty(hkKademesi))
                {
                    var hkBedel = _TeklifService.getHkKademesi(Convert.ToInt32(hkKademesi));
                    var unicoHkKademesi = _TeklifService.getHukuksalKorumaBedel(TeklifUretimMerkezleri.UNICO, hkBedel);
                    if (unicoHkKademesi != null)
                    {
                        HukusalKorumaLimiti = unicoHkKademesi.DegerKodu;
                    }
                }

                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = HukusalKorumaLimiti.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.HUKUKSAL_KORUMA_LIMITI;
                statistics.Add(statistic);

                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = teklif.Arac.KoltukSayisi.HasValue ? (teklif.Arac.KoltukSayisi.Value - 1).ToString() : "4";
                statistic.IST_KOD = UNICO_Istatistikler.YOLCU_SAYISI;
                statistics.Add(statistic);

                string unicoMeslekKodu = teklif.ReadSoru(KaskoSorular.UNICOMeslekKodu, "0");
                //Ekrandan girilen gönderilecek
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = unicoMeslekKodu.PadLeft(3, '0');
                statistic.IST_KOD = UNICO_Istatistikler.MESLEK_INDIRIMI;
                statistics.Add(statistic);


                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = "001";
                statistic.IST_KOD = UNICO_Istatistikler.AKSESUAR_TURU;
                statistics.Add(statistic);
                string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                bool ManeviDahil = false;
                decimal immBedel = 0;
                KaskoIMM CR_IMMBedel = new KaskoIMM();
                CR_KaskoIMM CRIMMKombineBedel = new CR_KaskoIMM();
                var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);
                if (IMMBedel != null)
                {
                    //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                    CR_IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);
                    if (CR_IMMBedel != null)
                    {
                        CRIMMKombineBedel = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.UNICO, IMMBedel.BedeniSahis, IMMBedel.Kombine, "111", "10");
                        if (CRIMMKombineBedel != null)
                        {
                            immDgrKodu = CRIMMKombineBedel.Kademe;
                        }
                    }
                    if (CR_IMMBedel.Text.Contains("Manevi Dahil")) ManeviDahil = true;
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = immDgrKodu;
                statistic.IST_KOD = UNICO_Istatistikler.IMM_BEDENI_MADDI;
                statistics.Add(statistic);

                if (ManeviDahil)
                {

                    ManeviTazminatDgrKodu = "E";
                }
                else
                {
                    ManeviTazminatDgrKodu = "H";
                }
                statistic = new EXT_WS_ISTDEG_REC();
                statistic.DEG_KOD = ManeviTazminatDgrKodu;
                statistic.IST_KOD = UNICO_Istatistikler.MANEVI_TAZMINAT;
                statistics.Add(statistic);
                policy.Statistics = statistics.ToArray();
                //}
                #endregion

                #region Matbular
                List<EXT_WS_POLICE_BLG_REC> infos = new List<EXT_WS_POLICE_BLG_REC>();
                EXT_WS_POLICE_BLG_REC info = new EXT_WS_POLICE_BLG_REC();

                if (informations != null)
                {
                    foreach (var item in informations)
                    {
                        info = new EXT_WS_POLICE_BLG_REC()
                        {
                            ACIKLAMA = item.value,
                            BILGI_SIRA_NO = Convert.ToDecimal(item.key)
                        };
                        infos.Add(info);
                    }
                }
                else
                {
                    info = new EXT_WS_POLICE_BLG_REC()
                    {
                        BILGI_SIRA_NO = UNICO_Matbular.PLAKA,
                        ACIKLAMA = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo
                    };
                    infos.Add(info);
                    info = new EXT_WS_POLICE_BLG_REC()
                    {
                        BILGI_SIRA_NO = UNICO_Matbular.MOTORNO,
                        ACIKLAMA = teklif.Arac.MotorNo
                    };
                    infos.Add(info);
                    info = new EXT_WS_POLICE_BLG_REC()
                    {
                        BILGI_SIRA_NO = UNICO_Matbular.SASINO,
                        ACIKLAMA = teklif.Arac.SasiNo
                    };
                    infos.Add(info);
                    //info = new EXT_WS_POLICE_BLG_REC()
                    //{
                    //    BILGI_SIRA_NO = UNICO_Matbular.HASINDRMORANI,
                    //    ACIKLAMA = ""
                    //};
                    //infos.Add(info);
                    info = new EXT_WS_POLICE_BLG_REC()
                    {
                        BILGI_SIRA_NO = UNICO_Matbular.TESCILTARIHI,
                        ACIKLAMA = teklif.Arac.TrafikTescilTarihi.HasValue ? teklif.Arac.TrafikTescilTarihi.Value.ToString() : ""
                    };
                    infos.Add(info);
                    //info = new EXT_WS_POLICE_BLG_REC()
                    //{
                    //    BILGI_SIRA_NO = UNICO_Matbular.POLICESURESI,
                    //    ACIKLAMA = ""
                    //};
                    //infos.Add(info);
                }
                if (infos.Count > 0)
                {
                    EXT_WS_POLICE_BLG_TYP bilgi = new EXT_WS_POLICE_BLG_TYP();
                    bilgi.m_EXT_WS_POLICE_BLG_REC = infos.ToArray();
                    policy.Informations = bilgi;
                }
                #endregion

                #region Sigortalı / Sigorta Ettiren

                policy.Insured = new EXT_WS_POLICE_SIGLI_REC();
                policy.Insured.IL = "034";
                policy.Insured.ILCE = "0NT";
                policy.Insured.SIGORTALI_NO = UnicoMusteriNo;
                policy.Insured.ULKE_KODU = 1;

                policy.InsuredBy = new EXT_WS_POLICE_ETT_REC();
                policy.InsuredBy.IL = "034";
                policy.InsuredBy.ILCE = "0NT";
                policy.InsuredBy.SIG_ETTIREN_NO = UnicoMusteriNo;
                policy.InsuredBy.ULKE_KODU = 1;

                policy.Item = new EXT_WS_POLICE_REC();
                policy.Item.DOVIZ_KUR = 1;
                policy.Item.POL_CINSI = "N";
                policy.Item.DOVIZ_CINS = "TL";

                policy.Item.KAYNAK_KODU = Convert.ToDecimal(servisKullanici.SubAgencyCode);
                policy.Item.SUBE_KOD = Convert.ToDecimal(servisKullanici.SourceId);
                policy.Item.TANZIM_TARIH = TurkeyDateTime.Now;
                policy.Item.TARIFE_KOD = productCode;
                policy.Item.TEKLIF_ONAY = UnicoTeklifTipleri.Teklif;
                policy.Item.ACENTA_NO = servisKullanici.PartajNo_;
                #endregion                             

                #region Teminatlar

                List<EXT_WS_POLICE_TEM_REC> teminats = new List<EXT_WS_POLICE_TEM_REC>();
                EXT_WS_POLICE_TEM_REC teminat = new EXT_WS_POLICE_TEM_REC();

                //teminat = new EXT_WS_POLICE_TEM_REC();
                //teminat.TEMINAT_KODU = UNICO_TeminatKodlari.SELSU_BASKINI;
                //teminat.BEDEL = 0;
                //teminat.ZEYL_TURU = "X";
                //teminats.Add(teminat);
                //teminat = new EXT_WS_POLICE_TEM_REC();
                //teminat.TEMINAT_KODU = UNICO_TeminatKodlari.IKAME_ARAC;
                //teminat.BEDEL = 0;
                //teminat.ZEYL_TURU = "X";
                //teminats.Add(teminat);
                bool GLKHHT = teklif.ReadSoru(KaskoSorular.GLKHHT, false);
                if (GLKHHT)
                {
                    teminat = new EXT_WS_POLICE_TEM_REC();
                    teminat.TEMINAT_KODU = UNICO_TeminatKodlari.GLKHHKNH_TEROR;
                    teminats.Add(teminat);
                }
                bool SelSu = teklif.ReadSoru(KaskoSorular.Sel_Su_VarYok, false);
                if (SelSu)
                {
                    teminat = new EXT_WS_POLICE_TEM_REC();
                    teminat.TEMINAT_KODU = UNICO_TeminatKodlari.SELSU_BASKINI;
                    teminats.Add(teminat);
                }
                bool AnahtarKaybetme = teklif.ReadSoru(KaskoSorular.Anahtar_Kaybi_VarYok, false);
                if (AnahtarKaybetme)
                {
                    teminat = new EXT_WS_POLICE_TEM_REC();
                    teminat.TEMINAT_KODU = UNICO_TeminatKodlari.ANAHTARLA_CALINMA;
                    teminats.Add(teminat);
                }

                teminat = new EXT_WS_POLICE_TEM_REC();
                teminat.TEMINAT_KODU = UNICO_TeminatKodlari.HUKUKSAL_KORUMA;
                teminat.BEDEL = 10000;//Default değer

                // string hkKademesi = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, "");
                if (!String.IsNullOrEmpty(hkKademesi))
                {
                    var hkBedel = _TeklifService.getHkKademesi(Convert.ToInt32(hkKademesi));
                    var unicoHkKademesi = _TeklifService.getHukuksalKorumaBedel(TeklifUretimMerkezleri.UNICO, hkBedel);
                    teminat.BEDEL = unicoHkKademesi.Bedel;
                }
                teminat.ZEYL_TURU = "X";
                teminats.Add(teminat);

                teminat = new EXT_WS_POLICE_TEM_REC();
                teminat.TEMINAT_KODU = UNICO_TeminatKodlari.KILIT_SISTEMI;
                teminat.BEDEL = 0;
                teminat.ZEYL_TURU = "X";
                teminats.Add(teminat);
                teminat = new EXT_WS_POLICE_TEM_REC();
                teminat.TEMINAT_KODU = UNICO_TeminatKodlari.KASKO;
                teminat.BEDEL = 0;
                teminat.ZEYL_TURU = "X";
                teminats.Add(teminat);

                bool hataliAkaryakit = teklif.ReadSoru(KaskoSorular.HataliAkaryakitAlimi, false);

                if (GenisletilmisCamMi)
                {
                    teminat = new EXT_WS_POLICE_TEM_REC();
                    teminat.TEMINAT_KODU = UNICO_TeminatKodlari.CAM_KIRILMASI;
                    teminat.BEDEL = 0;
                    teminat.ZEYL_TURU = "X";
                    teminats.Add(teminat);
                }
                else
                {
                    teminat = new EXT_WS_POLICE_TEM_REC();
                    teminat.TEMINAT_KODU = UNICO_TeminatKodlari.CAM_KIRILMASI;
                    teminat.BEDEL = 0;
                    teminat.ZEYL_TURU = "X";
                    teminats.Add(teminat);
                }
                if (hataliAkaryakit)
                {
                    teminat = new EXT_WS_POLICE_TEM_REC();
                    teminat.TEMINAT_KODU = UNICO_TeminatKodlari.YANLIS_AKARYAKIT_DOLUMU;
                    teminat.BEDEL = 0;
                    teminat.ZEYL_TURU = "X";
                    teminats.Add(teminat);
                }

                //teminat = new EXT_WS_POLICE_TEM_REC();
                //teminat.TEMINAT_KODU = UNICO_TeminatKodlari.DEPREM;
                //teminat.BEDEL = 0;
                //teminat.ZEYL_TURU = "X";
                //teminats.Add(teminat);
                //teminat = new EXT_WS_POLICE_TEM_REC();
                //teminat.TEMINAT_KODU = UNICO_TeminatKodlari.SELVE_SU_BASKINI;
                //teminat.BEDEL = 0;
                //teminat.ZEYL_TURU = "X";
                //teminats.Add(teminat);
                //teminat = new EXT_WS_POLICE_TEM_REC();
                //teminat.TEMINAT_KODU = UNICO_TeminatKodlari.KASKOLAY_YARDIM;
                //teminat.BEDEL = 0;
                //teminat.ZEYL_TURU = "X";
                //teminats.Add(teminat);
                //teminat = new EXT_WS_POLICE_TEM_REC();
                //teminat.TEMINAT_KODU = UNICO_TeminatKodlari.MINI_ONARIM;
                //teminat.BEDEL = 0;
                //teminat.ZEYL_TURU = "X";
                //teminats.Add(teminat);

                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                KaskoFK kaskoFK = new KaskoFK();
                if (!String.IsNullOrEmpty(fkKademe))
                {
                    kaskoFK = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);

                }

                teminat = new EXT_WS_POLICE_TEM_REC();
                teminat.TEMINAT_KODU = UNICO_TeminatKodlari.IHTMALI_SORUMLULUK;
                teminat.BEDEL = CR_IMMBedel != null ? CR_IMMBedel.BedeniSahis : 0;
                teminat.ZEYL_TURU = "X";
                teminats.Add(teminat);

                teminat = new EXT_WS_POLICE_TEM_REC();
                teminat.TEMINAT_KODU = UNICO_TeminatKodlari.OLUMMALUL_SB;
                teminat.BEDEL = CR_IMMBedel != null ? CR_IMMBedel.BedeniSahis : 0;
                teminat.ZEYL_TURU = "X";
                teminats.Add(teminat);

                teminat = new EXT_WS_POLICE_TEM_REC();
                teminat.TEMINAT_KODU = UNICO_TeminatKodlari.OLUMMALUL_KB;
                teminat.BEDEL = CR_IMMBedel != null ? CR_IMMBedel.BedeniKaza : 0;
                teminat.ZEYL_TURU = "X";
                teminats.Add(teminat);

                teminat = new EXT_WS_POLICE_TEM_REC();
                teminat.TEMINAT_KODU = UNICO_TeminatKodlari.MADDI_HASAR;
                teminat.BEDEL = CR_IMMBedel != null ? CR_IMMBedel.Maddi : 0;
                teminat.ZEYL_TURU = "X";
                teminats.Add(teminat);

                if (kaskoFK != null)
                {
                    if (kaskoFK.Kombine != null && kaskoFK.Kombine != 0)
                    {
                        teminat = new EXT_WS_POLICE_TEM_REC();
                        teminat.TEMINAT_KODU = UNICO_TeminatKodlari.KOLTUK_FERDI_KAZA;
                        teminat.BEDEL = kaskoFK.Kombine;
                        teminat.ZEYL_TURU = "X";
                        teminats.Add(teminat);
                    }
                    else
                    {
                        teminat = new EXT_WS_POLICE_TEM_REC();
                        teminat.TEMINAT_KODU = UNICO_TeminatKodlari.KOLTUK_FERDI_KAZA;
                        teminat.BEDEL = kaskoFK.Sakatlik;
                        teminat.ZEYL_TURU = "X";
                        teminats.Add(teminat);

                        teminat = new EXT_WS_POLICE_TEM_REC();
                        teminat.TEMINAT_KODU = UNICO_TeminatKodlari.FERDI_KAZA;
                        teminat.BEDEL = kaskoFK.Vefat;
                        teminat.ZEYL_TURU = "X";
                        teminats.Add(teminat);

                        teminat = new EXT_WS_POLICE_TEM_REC();
                        teminat.TEMINAT_KODU = UNICO_TeminatKodlari.TEDAVI_MASRAFLARI;
                        teminat.BEDEL = kaskoFK.Tedavi;
                        teminat.ZEYL_TURU = "X";
                        teminats.Add(teminat);
                    }

                }

                policy.CoverageLimits = teminats.ToArray();

                #endregion

                #region Genel bilgiler

                if (teklif.GenelBilgiler.OdemeTipi == OdemeTipleri.Nakit)
                {
                    policy.ODM = UnicoOdemeTipleri.Nakit;
                }
                else
                {
                    policy.ODM = UnicoOdemeTipleri.KrediKarti;
                }
                policy.OnlinePolicyEnabled = true;
                policy.PolicyStatus = UnicoTeklifTipleri.Teklif;


                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                DateTime startDate = new DateTime(polBaslangic.Year, polBaslangic.Month, polBaslangic.Day);
                polBaslangic = polBaslangic.AddYears(1);
                DateTime endDate = new DateTime(polBaslangic.Year, polBaslangic.Month, polBaslangic.Day);
                #endregion

                #region Service Call
                bool CreatePolicyResult;
                this.BeginLog(policy, policy.GetType(), WebServisIstekTipleri.Teklif);
                client.CreatePolicy(authenticationKey, ref policy, startDate, ref endDate, 0, out CreatePolicyResult);
                client.Dispose();
                if (CreatePolicyResult)
                {
                    this.EndLog(policy, true, policy.GetType());
                }
                else
                {
                    if (policy.AlertNo != 0)
                    {
                        this.EndLog(policy, false, policy.GetType());
                        this.AddHata(policy.AlertText);
                    }
                }
                #endregion

                if (!this.Basarili)
                {
                    this.Import(teklif);
                    this.GenelBilgiler.Basarili = false;
                    return;
                }
                this.Import(teklif);

                if (!String.IsNullOrEmpty(policy.RelatedPolicyNumber))
                {
                    this.GenelBilgiler.TUMTeklifNo = policy.RelatedPolicyNumber;
                }

                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = policy.TotalGrossPremium;
                this.GenelBilgiler.NetPrim = policy.TotalPremium;
                this.GenelBilgiler.ToplamKomisyon = policy.TotalCommission;
                this.GenelBilgiler.ToplamVergi = policy.TotalTax;

                bool generateNotification = true;
                string unicoPolcyNo = "";
                string unicoEskiPolcyNo = "";
                if (policy.Item != null)
                {
                    unicoPolcyNo = policy.Item.CARI_POL_NO.HasValue ? policy.Item.CARI_POL_NO.Value.ToString() : "";
                    unicoEskiPolcyNo = policy.Item.ESKI_POL_NO.HasValue ? policy.Item.ESKI_POL_NO.Value.ToString() : "";
                }

                var responsePDF = client.GetPolicyReport(appSecurityKey, unicoPolcyNo, "0", generateNotification, servisKullanici.KullaniciAdi, unicoEskiPolcyNo);
                byte[] pdfUrlArray = new byte[1];
                byte[] BilgilendirmePDF = new byte[1];
                pdfUrlArray = responsePDF.POLICE_OBJE;
                BilgilendirmePDF = responsePDF.BILGILENDIRME_OBJE;

                if (pdfUrlArray != null)
                {
                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Empty;
                    string url = String.Empty;
                    fileName = String.Format("UNICO_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                    url = storage.UploadFile("kasko", fileName, pdfUrlArray);
                    teklif.GenelBilgiler.PDFDosyasi = url;
                }
                if (BilgilendirmePDF != null)
                {
                    ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                    string fileName = String.Empty;
                    string url = String.Empty;
                    fileName = String.Format("UNICO_Kasko_Police_BilgilendirmeFormu{0}.pdf", System.Guid.NewGuid().ToString());
                    url = storage.UploadFile("kasko", fileName, pdfUrlArray);
                    teklif.GenelBilgiler.PDFBilgilendirme = url;
                }

                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_AuthenticationKey, authenticationKey);
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_licencePlate, licencePlate);
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_registrySerial, registrySerial);
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_registryNo, registryNo);
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_queryInput, queryInput);
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_queryInputType, queryInputType);
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_startDate, startDate);
                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_endDate, endDate);
                //this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_appSecurityKey, appSecurityKey);

                if (!String.IsNullOrEmpty(oncekipolno))
                {
                    this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekipolno, oncekipolno);
                    this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekisigortasirketi, oncekisigortasirketi);
                    this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekiacenteno, oncekiacenteno);
                    this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekiyenilemeno, oncekiyenilemeno);
                }
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
            //try
            //{
            //    #region Veri Hazırlama GENEL

            //    ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);

            //    KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleUnico);

            //    MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

            //    #endregion

            //    PolicyService client = new PolicyService();
            //    client.Url = "https://previva.unicosigorta.com.tr/Public/PolicyService.svc";// konfig[Konfig.Unico_PolicyServiceURL];
            //    client.Timeout = 150000;
            //    var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
            //    TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.UNICO });

            //    string authenticationKey = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_AuthenticationKey, "0");
            //    string oncekipolno = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_oncekipolno, "0");
            //    string oncekisigortasirketi = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_oncekisigortasirketi, "0");
            //    string oncekiacenteno = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_oncekiacenteno, "0");
            //    string oncekiyenilemeno = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_oncekiyenilemeno, "0");
            //    string  licencePlate =  this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_licencePlate, "0");
            //    string registrySerial = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_registrySerial, "0");
            //    string registryNo = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_registryNo, "0");
            //    string queryInput = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_queryInput, "0");
            //    string queryInputType = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_queryInputType, "0");
            //    DateTime startDate = Convert.ToDateTime(this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_startDate, "0"));
            //    DateTime endDate = Convert.ToDateTime(this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_endDate, "0"));
            //    //string appSecurityKey = this.ReadWebServisCevap(Common.WebServisCevaplar.UNICO_appSecurityKey, "0");




            //    #region Poliçe Yenileme ise / İstatistikler

            //    EXT_WS_ISTDEG_REC statistic = new EXT_WS_ISTDEG_REC();
            //    List<EXT_WS_ISTDEG_REC> statistics = new List<EXT_WS_ISTDEG_REC>();
            //    Policy1 policy = new Policy1();
            //    policy.AppSecurityKey = authenticationKey;
            //    string hkKademesi = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, String.Empty);
            //    if (!String.IsNullOrEmpty(oncekipolno))
            //    {
            //        policy.QueryRenewFirmNo = oncekisigortasirketi;
            //        policy.QueryRenewAgentNo = oncekiacenteno;
            //        policy.QueryRenewPolicyNo = oncekipolno;
            //        policy.QueryRenewNo = oncekiyenilemeno;

            //        policy.QueryLicencePlate = licencePlate;
            //        policy.QueryRegistrySerial = registrySerial;
            //        policy.QueryRegistryNo = registryNo;
            //        policy.QueryIdentityInput = queryInput;
            //        policy.QueryIdentityType = queryInputType;
            //        policy.SelectedTAHSILAT_KOD = "PESIN";


            //        List<KeyValuePairOfstringstring> statList = new List<KeyValuePairOfstringstring>();
            //        KeyValuePairOfstringstring statItem = new KeyValuePairOfstringstring();
            //        statItem.key = "";
            //        statItem.value = "";
            //        statList.Add(statItem);
            //        KeyValuePairOfstringstring[] stats = statList.ToArray();

            //        for (int i = stats.Count(); i <= 0; i--)
            //        {
            //            statistic = new EXT_WS_ISTDEG_REC();
            //            statistic.DEG_KOD = stats[i].value;
            //            statistic.IST_KOD = stats[i].key;
            //            statistics.Add(statistic);
            //        }
            //        policy.Statistics = statistics.ToArray();
            //    }
            //    #endregion

            //    #region Service Call

            //    bool CreatePolicyResult;
            //    this.BeginLog(policy, policy.GetType(), WebServisIstekTipleri.Teklif);
            //    client.CreatePolicy(authenticationKey, ref policy, startDate, ref endDate, 1, out CreatePolicyResult);
            //    client.Dispose();
            //    if (CreatePolicyResult)
            //    {
            //        this.EndLog(policy, true, policy.GetType());
            //    }
            //    else
            //    {
            //        if (policy.AlertNo != 0)
            //        {
            //            this.EndLog(policy, false, policy.GetType());
            //            this.AddHata(policy.AlertText);
            //        }
            //    }
            //    #endregion

            //    #region Ödeme Bilgileri

            //    PolicyPayment policyPayment = new PolicyPayment();

            //    if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
            //    {
            //        policyPayment.CreditCardNo = odeme.KrediKarti.KartNo.Substring(0, 6);
            //        policyPayment.CvcCode = "XXX";
            //        policyPayment.ExpirationDate = "999999";
            //        policyPayment.OwnerName = odeme.KrediKarti.KartSahibi;
            //        policyPayment.SupplementNo = "0";
            //    }
            //    byte taksitsayisi = odeme.TaksitSayisi;


            //    if (odeme.OdemeSekli == OdemeSekilleri.Vadeli)
            //    {
            //        switch (odeme.OdemeTipi)
            //        {
            //            case OdemeTipleri.Nakit: policyPayment.PaymentType = UnicoOdemeTipleri.Nakit; break;
            //            case OdemeTipleri.KrediKarti: policyPayment.PaymentType = UnicoOdemeTipleri.KrediKarti; break;
            //            //OdemeTiplerine eklendi. BankaHesabi
            //            case OdemeTipleri.BankaHesabi: policyPayment.PaymentType = UnicoOdemeTipleri.BankaKarti; break;
            //        }

            //        if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
            //        {
            //            bool MakePolicyPaymentResult;
            //            bool MaketPolicyPaymentResultSpecified;
            //            this.BeginLog(policy, policy.GetType(), WebServisIstekTipleri.Teklif);
            //            //Payment doldurulan yer. 
            //            policyPayment.CreditCardNo = odeme.KrediKarti.KartNo;
            //            policyPayment.CvcCode = odeme.KrediKarti.CVC;
            //            policyPayment.ExpirationDate = odeme.KrediKarti.SKA + "/" + odeme.KrediKarti.SKY;
            //            policyPayment.OwnerName = odeme.KrediKarti.KartSahibi;
            //            policyPayment.SupplementNo = "0";
            //            client.MakePolicyPayment(authenticationKey,ref policyPayment,out MakePolicyPaymentResult, out MaketPolicyPaymentResultSpecified);
            //            client.Dispose();
            //            if (MakePolicyPaymentResult)
            //            {
            //                this.EndLog(policy, true, policy.GetType());
            //            }
            //            else
            //            {
            //                if (policy.AlertNo != 0)
            //                {
            //                    this.EndLog(policy, false, policy.GetType());
            //                    this.AddHata(policy.AlertText);
            //                }
            //            }

            //            if(this.Hatalar.Count == 0)
            //            {
            //                this.GenelBilgiler.TUMPoliceNo = policy.RelatedPolicyNumber;
            //                this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
            //                this.GenelBilgiler.Basarili = true;
            //                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
            //                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
            //                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
            //                this.GenelBilgiler.BrutPrim = policy.TotalGrossPremium;
            //                this.GenelBilgiler.NetPrim = policy.TotalPremium;
            //                this.GenelBilgiler.ToplamKomisyon = policy.TotalCommission;
            //                this.GenelBilgiler.ToplamVergi = policy.TotalTax;

            //            }

            //            this.GenelBilgiler.WEBServisLogs = this.Log;
            //            _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);

            //            bool generateNotification = true;
            //            string unicoPolcyNo = "";
            //            string unicoEskiPolcyNo = "";
            //            if (policy.Item != null)
            //            {
            //                unicoPolcyNo = policy.Item.CARI_POL_NO.HasValue ? policy.Item.CARI_POL_NO.Value.ToString() : "";
            //                unicoEskiPolcyNo = policy.Item.ESKI_POL_NO.HasValue ? policy.Item.ESKI_POL_NO.Value.ToString() : "";
            //            }
            //            var responsePDF = client.GetPolicyReport(policy.AppSecurityKey, unicoPolcyNo, "0", generateNotification, servisKullanici.KullaniciAdi, unicoEskiPolcyNo);
            //            byte[] pdfUrlArray = new byte[1];
            //            byte[] BilgilendirmePDF = new byte[1];
            //            pdfUrlArray = responsePDF.POLICE_OBJE;
            //            BilgilendirmePDF = responsePDF.BILGILENDIRME_OBJE;

            //            if (pdfUrlArray != null)
            //            {
            //                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
            //                string fileName = String.Empty;
            //                string url = String.Empty;
            //                fileName = String.Format("UNICO_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
            //                url = storage.UploadFile("kasko", fileName, pdfUrlArray);
            //                teklif.GenelBilgiler.PDFDosyasi = url;
            //            }
            //            if (BilgilendirmePDF != null)
            //            {
            //                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
            //                string fileName = String.Empty;
            //                string url = String.Empty;
            //                fileName = String.Format("UNICO_Kasko_Police_BilgilendirmeFormu{0}.pdf", System.Guid.NewGuid().ToString());
            //                url = storage.UploadFile("kasko", fileName, pdfUrlArray);
            //                teklif.GenelBilgiler.PDFBilgilendirme = url;
            //            }

            //            this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
            //            this.GenelBilgiler.TaksitSayisi = 1;
            //            this.GenelBilgiler.ZKYTMSYüzdesi = 0;
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_AuthenticationKey, authenticationKey);
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_licencePlate, licencePlate);
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_registrySerial, registrySerial);
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_registryNo, registryNo);
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_queryInput, queryInput);
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_queryInputType, queryInputType);
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_startDate, startDate);
            //            this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_endDate, endDate);

            //            if (!String.IsNullOrEmpty(oncekipolno))
            //            {
            //                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekipolno, oncekipolno);
            //                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekisigortasirketi, oncekisigortasirketi);
            //                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekiacenteno, oncekiacenteno);
            //                this.AddWebServisCevap(Common.WebServisCevaplar.UNICO_oncekiyenilemeno, oncekiyenilemeno);
            //            }
            //            ////// ==== Güncellenicek. ==== //
            //            if (this.GenelBilgiler.TaksitSayisi == 1)
            //            {
            //                this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        policyPayment.PaymentType = UnicoOdemeTipleri.Nakit;
            //    }

            //    #endregion

            //}
            //catch (Exception ex)
            //{
            //    #region Hata Log
            //    this.EndLog(ex.Message, false);
            //    this.AddHata(ex.Message);

            //    this.GenelBilgiler.WEBServisLogs = this.Log;
            //    _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
            //    #endregion
            //}
        }

        public class SecurityServiceRequest
        {
            public string appSecurityKey { get; set; }
            public string KullaniciAdi { get; set; }
            public string Sifre { get; set; }
            //public unicosecurity.BaseEntity entity = new unicosecurity.BaseEntity();
        }
    }
}
