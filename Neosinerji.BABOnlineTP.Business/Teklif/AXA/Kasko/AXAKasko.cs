using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.AXAPoliceBasim;
//using Neosinerji.BABOnlineTP.Business.AxaTeklifNewService;
using Neosinerji.BABOnlineTP.Business.Common;
//using Neosinerji.BABOnlineTP.Business.ServiceReferenceAxaNew;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.AxaKasko;
using System.Xml.Serialization;
//using axaService = Neosinerji.BABOnlineTP.Business.ServiceReferenceAxaNew;
//using Neosinerji.BABOnlineTP.Business.AxaTeklifNewService;

namespace Neosinerji.BABOnlineTP.Business.AXA
{
    public class AXAKasko : Teklif, IAXAKasko
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
        public AXAKasko(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, ITVMService TVMService)
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
                return TeklifUretimMerkezleri.AXA;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                #region yeni service
                //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAXAService);
                //var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AXA });
                //List<CPartnerBilgi> cPartnerList = new List<CPartnerBilgi>();
                //List<CTeminat> teminatList = new List<CTeminat>();
                //DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                //#region Eski Poliçe Bilgileri Hazırlama

                //bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);

                //string TramerPoliceNo = "";
                //string TramerAcenteNo = "";
                //string TramerSirketKodu = "";
                //string TramerYenilemeNo = "";
                //if (eskiPoliceVar)
                //{
                //    TramerPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                //    TramerAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                //    TramerSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                //    TramerYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                //}

                //#endregion

                //#region Sigortali / Sigorta Ettiren Bilgileri Hazırlama

                //MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                //MusteriAdre sigortaliAdres = null;
                //if (sigortali != null)
                //{
                //    sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                //}

                //var sigEttiren = teklif.SigortaEttiren;
                //MusteriAdre sigEttirenAdres = null;
                //if (sigEttiren != null)
                //{
                //    sigEttirenAdres = sigEttiren.MusteriGenelBilgiler.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                //}

                //string axaSigIlKodu = String.Empty;
                //string axaSigIlceKodu = String.Empty;

                //string axaSigEttirenIlKodu = String.Empty;
                //string axaSigEttirenIlceKodu = String.Empty;
                //MusteriTelefon sigortaliCepTelefon = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
                //MusteriTelefon sigortaliEvTelefon = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Ev).FirstOrDefault();
                //string MusteriEvTel = "";
                //string MusteriCepTel = "";

                //if (sigortaliCepTelefon == null)
                //{
                //    sigortaliCepTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                //}
                //if (sigortaliEvTelefon == null)
                //{
                //    sigortaliEvTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                //}
                //if (sigortaliCepTelefon != null)
                //{
                //    if (!String.IsNullOrEmpty(sigortaliCepTelefon.Numara) && sigortaliCepTelefon.Numara.Trim() != "90" && sigortaliCepTelefon.Numara.Length >= 10)
                //    {
                //        string telefon = sigortaliCepTelefon.Numara;
                //        if (telefon.Length == 14)
                //        {
                //            MusteriCepTel = sigortaliCepTelefon.Numara.Substring(1, 1) + "(";
                //            MusteriCepTel += sigortaliCepTelefon.Numara.Substring(3, 3) + ")";
                //            MusteriCepTel += sigortaliCepTelefon.Numara.Substring(7, 7);
                //        }
                //    }
                //    else
                //    {
                //        MusteriCepTel = "0(555)9999999";
                //    }
                //}
                //if (sigortaliEvTelefon != null)
                //{
                //    if (!String.IsNullOrEmpty(sigortaliEvTelefon.Numara) && sigortaliEvTelefon.Numara.Trim() != "90" && sigortaliEvTelefon.Numara.Length >= 10)
                //    {
                //        string telefon = sigortaliEvTelefon.Numara;
                //        if (telefon.Length == 14)
                //        {
                //            MusteriEvTel = sigortaliEvTelefon.Numara.Substring(1, 1) + "(";
                //            MusteriEvTel += sigortaliEvTelefon.Numara.Substring(3, 3) + ")";
                //            MusteriEvTel += sigortaliEvTelefon.Numara.Substring(7, 7);
                //        }
                //    }
                //    else
                //    {
                //        MusteriEvTel = "0(212)9999999";
                //    }
                //}

                //bool MusteriAyniMi = false;
                //if (sigEttiren.MusteriKodu == sigortali.MusteriKodu)
                //{
                //    MusteriAyniMi = true;
                //    if (sigortaliAdres != null)
                //    {
                //        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                //                                                                 f.IlKodu == sigortaliAdres.IlKodu &&
                //                                                                 f.IlceKodu == sigortaliAdres.IlceKodu)
                //                                                   .SingleOrDefault<CR_IlIlce>();

                //        axaSigIlKodu = ililce.CRIlKodu;
                //        axaSigIlceKodu = ililce.CRIlceKodu;
                //    }
                //}
                //else
                //{
                //    MusteriAyniMi = false;

                //    if (sigEttirenAdres != null)
                //    {

                //        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                //                                                                 f.IlKodu == sigEttirenAdres.IlKodu &&
                //                                                                 f.IlceKodu == sigEttirenAdres.IlceKodu)
                //                                                   .SingleOrDefault<CR_IlIlce>();

                //        axaSigEttirenIlKodu = ililce.CRIlKodu;
                //        axaSigEttirenIlceKodu = ililce.CRIlceKodu;
                //    }
                //}

                //#endregion

                //AxaExternalProductionClient axaClient = new AxaExternalProductionClient();
                ////AxaExternalProductionService axaClient = new AxaExternalProductionService();
                //axaClient.ClientCredentials.Windows.ClientCredential = new NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);
                ////new NetworkCredential(servisKullanici.KullaniciAdi, servisKullanici.Sifre, "AXA");
                ////axaClient.Url= konfig[Konfig.AXA_ServiceURL];
                ////axaClient. = 150000;
                //CFiyatCalismasiParametre cFiyatCalismasiParametre = new CFiyatCalismasiParametre()
                //{
                //    ClientIPAddress = "85.105.78.56", //this.IpGetir(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value),
                //    //Doviz_Kod = "",
                //    //  MatbuBilgiler = ,
                //    TarihBilgileri = new CTarihBilgileri()
                //    {
                //        Tanzim_Tarih = TurkeyDateTime.Now,
                //        Teklif_Tarih = TurkeyDateTime.Now,
                //        Baslama_Tarih = polBaslangic,
                //        Bitis_Tarih = polBaslangic.AddYears(1),
                //    },
                //    //MTKod = "",
                //    OtomatikYenileme = CEvetHayirTpe.Hayir,
                //    //OtorizasyonBilgi = new COtorizasyonBilgi()
                //    //{
                //    //Otorizasyon = CEvetHayirTpe.Hayir,
                //    //OtorizasyonMail = sigortali.EMail,

                //    //},
                //    //PoliceCinsi = "",
                //    RefKullanici = "S4968850",//"S71440575",//servisKullanici.KullaniciAdi,
                //    Riziko = new ServiceReferenceAxaNew.CMusteri()
                //    {
                //        //  BilgiMesajlari="",
                //        GenelBilgiler = new ServiceReferenceAxaNew.CMusteriGenelBilgiler()
                //        {
                //            AdUnvan1 = sigortali.AdiUnvan,
                //            AdUnvan2 = "",
                //            Cinsiyet = "",
                //            TcNo = sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri ? sigortali.KimlikNo : "",
                //            VergiNo = sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri ? sigortali.KimlikNo : "",
                //            Soyad = sigortali.SoyadiUnvan,
                //            UlkeKod = "TÜR"

                //        },
                //        //PartnerBilgiler = cPartnerList.ToArray(),
                //        UavtAdres = new ServiceReferenceAxaNew.CMusteri_Uavt_Adres()
                //        {
                //            Ulke = new CUlke()
                //            {
                //                Ulke_Kod = "TÜR",
                //                Ulke_Ad = "TÜRKİYE",
                //            },
                //            AcikAdres = sigortaliAdres.Adres,
                //            // AcentaId = 0,
                //            // Aciklama = "",
                //            Aktif = true,
                //            //AxaAdresNo=0,
                //            BagimsizBolum = new CUAVTBagimsizBolum(),
                //            Bina = new CUAVTBina(),
                //            Bucak = new CUAVTBucak(),
                //            CSBM = new CUAVTCSBM(),
                //            Il = new CIl()
                //            {
                //                Il_Kod = axaSigIlKodu,
                //                Il_Hyt_Kod = axaSigIlKodu,
                //                Il_Ad = ""
                //            },
                //            Ilce = new CUAVTIlce()
                //            {
                //                //  Kod =  !String.IsNullOrEmpty(axaSigIlceKodu)? Convert.ToDecimal(axaSigIlceKodu):0,
                //                Adi = "",
                //            },
                //            Koy = new CUAVTKoy(),
                //            Mahalle = new CUAVTMahalle(),
                //            PostaKodu = "34060",
                //            TCdenUvatAdresGetir = true,
                //            YazismaAdresi = CEvetHayirTpe.Hayir,
                //            // GeoCode_Id=0,

                //        }
                //    },
                //    SigortaEttiren = new ServiceReferenceAxaNew.CMusteri()
                //    {
                //        GenelBilgiler = new ServiceReferenceAxaNew.CMusteriGenelBilgiler()
                //        {
                //            AdUnvan1 = sigEttiren.MusteriGenelBilgiler.AdiUnvan,
                //            AdUnvan2 = "",
                //            Cinsiyet = "",
                //            TcNo = sigEttiren.MusteriGenelBilgiler.MusteriTipKodu == MusteriTipleri.TCMusteri ? sigEttiren.MusteriGenelBilgiler.KimlikNo : "",
                //            VergiNo = sigEttiren.MusteriGenelBilgiler.MusteriTipKodu == MusteriTipleri.TuzelMusteri ? sigEttiren.MusteriGenelBilgiler.KimlikNo : "",
                //            Soyad = sigEttiren.MusteriGenelBilgiler.SoyadiUnvan,
                //            UlkeKod = "TÜR"
                //        }
                //    ,
                //        UavtAdres = new CMusteri_Uavt_Adres()
                //        {
                //            Ulke = new CUlke()
                //            {
                //                Ulke_Kod = "TÜR",
                //                Ulke_Ad = "TÜRKİYE",
                //            },
                //            AcikAdres = sigEttirenAdres.Adres,
                //            // AcentaId = 0,
                //            // Aciklama = "",
                //            Aktif = true,
                //            //AxaAdresNo=0,
                //            BagimsizBolum = new CUAVTBagimsizBolum(),
                //            Bina = new CUAVTBina(),
                //            Bucak = new CUAVTBucak(),
                //            CSBM = new CUAVTCSBM(),
                //            Il = new CIl()
                //            {
                //                Il_Kod = MusteriAyniMi ? axaSigIlKodu : axaSigEttirenIlKodu,
                //                Il_Hyt_Kod = MusteriAyniMi ? axaSigIlKodu : axaSigEttirenIlKodu,
                //                Il_Ad = ""
                //            },
                //            Ilce = new CUAVTIlce()
                //            {
                //                //  Kod = MusteriAyniMi ? (!String.IsNullOrEmpty(axaSigIlceKodu)? Convert.ToDecimal(axaSigIlceKodu ): 0 ): !String.IsNullOrEmpty(axaSigEttirenIlceKodu)? Convert.ToDecimal(axaSigEttirenIlceKodu):0,
                //                Adi = "",
                //            },
                //            Koy = new CUAVTKoy(),
                //            Mahalle = new CUAVTMahalle(),
                //            PostaKodu = "34060",
                //            TCdenUvatAdresGetir = true,
                //            YazismaAdresi = CEvetHayirTpe.Hayir,
                //            // GeoCode_Id=0,
                //        }
                //    },
                //    Sigortali = new ServiceReferenceAxaNew.CMusteri()
                //    {
                //        GenelBilgiler = new ServiceReferenceAxaNew.CMusteriGenelBilgiler()
                //        {
                //            AdUnvan1 = sigortali.AdiUnvan,
                //            AdUnvan2 = "",
                //            Cinsiyet = "",
                //            TcNo = sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri ? sigortali.KimlikNo : "",
                //            VergiNo = sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri ? sigortali.KimlikNo : "",
                //            Soyad = sigortali.SoyadiUnvan,
                //            UlkeKod = "TÜR"

                //        },
                //        UavtAdres = new ServiceReferenceAxaNew.CMusteri_Uavt_Adres()
                //        {
                //            Ulke = new CUlke()
                //            {
                //                Ulke_Kod = "TÜR",
                //                Ulke_Ad = "TÜRKİYE",
                //            },
                //            AcikAdres = sigortaliAdres.Adres,
                //            // AcentaId = 0,
                //            // Aciklama = "",
                //            Aktif = true,
                //            //AxaAdresNo=0,
                //            BagimsizBolum = new CUAVTBagimsizBolum(),
                //            Bina = new CUAVTBina(),
                //            Bucak = new CUAVTBucak(),
                //            CSBM = new CUAVTCSBM(),
                //            Il = new CIl()
                //            {
                //                Il_Kod = axaSigIlKodu,
                //                Il_Hyt_Kod = axaSigEttirenIlKodu,
                //                Il_Ad = ""
                //            },
                //            Ilce = new CUAVTIlce()
                //            {
                //                //Kod = !String.IsNullOrEmpty(axaSigIlceKodu) ? Convert.ToDecimal(axaSigIlceKodu) : 0,
                //                Adi = "",
                //            },
                //            Koy = new CUAVTKoy(),
                //            Mahalle = new CUAVTMahalle(),
                //            PostaKodu = "34060",
                //            TCdenUvatAdresGetir = true,
                //            YazismaAdresi = CEvetHayirTpe.Hayir,
                //            // GeoCode_Id=0,
                //        }
                //    },
                //    //Sorular
                //    SubeKaynakAcente = new CSubeKaynakAcente()
                //    {
                //        AcenteNo = servisKullanici.PartajNo_,
                //        SubeKod = 0,
                //        KaynakKod = 300,
                //    },
                //    //TeknikPersonel = "",
                //    TramerBilgileri = new CTramerBilgileri()
                //    {
                //        TramerAcenteNo = TramerAcenteNo,
                //        TramerPoliceNo = TramerPoliceNo,
                //        TramerSirketKod = TramerSirketKodu,
                //        TramerYenilemeNo = TramerYenilemeNo,
                //        PlakaNo = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo
                //    },
                //    //TurceEkAciklamalar="",
                //    //IngEkAciklamalar=""
                //    UretimYeri = 0,
                //    UrunKodu = AXA_UrunKodlari.Kasko,

                //};
                //List<ServiceReferenceAxaNew.CSoru> soruList = new List<ServiceReferenceAxaNew.CSoru>();
                //List<ServiceReferenceAxaNew.CMatbuBilgi> cmatubList = new List<ServiceReferenceAxaNew.CMatbuBilgi>();
                //#region Teminatlar

                //CTeminat teminat = new CTeminat()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    TeminatKod = AXA_KaskoTeminatlar.AnahtarKaybiZararlari,
                //};
                //teminatList.Add(teminat);

                //string kullanimT = String.Empty;
                //string kod2 = String.Empty;
                //string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                //if (parts.Length == 2)
                //{
                //    kullanimT = parts[0];
                //    kod2 = parts[1];
                //}

                //string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                //if (!String.IsNullOrEmpty(fkKademe))
                //{
                //    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);
                //    if (FKBedel != null)
                //    {
                //        teminat = new CTeminat()
                //        {
                //            Adet = 5,
                //            Bedel = FKBedel.Tedavi,
                //            Iptal = false,
                //            TeminatKod = AXA_KaskoTeminatlar.TedaviMasraflari

                //        };
                //        teminatList.Add(teminat);

                //        teminat = new CTeminat()
                //        {
                //            Adet = 5,
                //            Bedel = FKBedel.Sakatlik,
                //            Iptal = false,
                //            TeminatKod = AXA_KaskoTeminatlar.FerdiKazaOlum

                //        };
                //        teminatList.Add(teminat);
                //        teminat = new CTeminat()
                //        {
                //            Adet = 5,
                //            Bedel = FKBedel.Vefat,
                //            Iptal = false,
                //            TeminatKod = AXA_KaskoTeminatlar.FerdiKaza

                //        };
                //        teminatList.Add(teminat);
                //    }
                //}
                //else
                //{
                //    //fk seçilmediğinde alt limitler gönderilecek zorunlu alan göndermezsek hata geliyor
                //    teminat = new CTeminat()
                //    {
                //        Adet = 3,
                //        Bedel = 350,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.TedaviMasraflari

                //    };
                //    teminatList.Add(teminat);

                //    teminat = new CTeminat()
                //    {
                //        Adet = 3,
                //        Bedel = 3500,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.FerdiKazaOlum

                //    };
                //    teminatList.Add(teminat);
                //    teminat = new CTeminat()
                //    {
                //        Adet = 3,
                //        Bedel = 3500,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.FerdiKaza

                //    };
                //    teminatList.Add(teminat);
                //}

                //teminat = new CTeminat()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    TeminatKod = AXA_KaskoTeminatlar.HukuksalKoruma

                //};
                //teminatList.Add(teminat);

                //teminat = new CTeminat()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    TeminatKod = AXA_KaskoTeminatlar.Asistans

                //};
                //teminatList.Add(teminat);

                //#endregion

                //#region Seçimli Default

                //decimal IMMSoru = 0;
                //decimal IMMKombineSoru = 0;
                //string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                //KaskoIMM CR_IMMBedel = new KaskoIMM();
                //CR_KaskoIMM CRIMMKombineBedel = new CR_KaskoIMM();
                //if (!String.IsNullOrEmpty(immKademe))
                //{
                //    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);
                //    if (IMMBedel != null)
                //    {


                //        //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                //        CR_IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                //        if (CR_IMMBedel != null)
                //        {
                //            //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                //            CRIMMKombineBedel = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.RAY, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                //        }
                //        if (CRIMMKombineBedel != null)
                //        {
                //            IMMSoru = CRIMMKombineBedel.BedeniSahis.Value;
                //            IMMKombineSoru = CRIMMKombineBedel.Kombine.Value;
                //        }

                //        if (IMMSoru > 0)
                //        {
                //            teminat = new CTeminat()
                //            {
                //                Adet = 1,
                //                Bedel = IMMSoru,
                //                Iptal = false,
                //                TeminatKod = AXA_KaskoTeminatlar.IMS3Limit

                //            };
                //            teminatList.Add(teminat);

                //        }
                //        if (IMMKombineSoru > 0)
                //        {
                //            teminat = new CTeminat()
                //            {
                //                Adet = 1,
                //                Bedel = IMMKombineSoru,
                //                Iptal = false,
                //                TeminatKod = AXA_KaskoTeminatlar.IMSKombine

                //            };
                //            teminatList.Add(teminat);
                //        }

                //    }
                //}
                //else
                //{
                //    teminat = new CTeminat()
                //    {
                //        Adet = 1,
                //        Bedel = 0,
                //        Iptal = false,
                //        TeminatKod = AXA_KaskoTeminatlar.IMS3Limit

                //    };
                //    teminatList.Add(teminat);
                //}
                //#endregion

                //#region İstatistikler

                //string AXAKullanimTarzi = String.Empty;
                //string AXAKullanimSekli = String.Empty;

                //if (parts.Length == 2)
                //{
                //    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                //                                                                                  f.KullanimTarziKodu == kullanimT &&
                //                                                                                  f.Kod2 == kod2)
                //                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                //    if (kullanimTarzi != null)
                //    {
                //        string[] part = kullanimTarzi.TarifeKodu.Split('-');
                //        if (part.Length > 0)
                //        {
                //            AXAKullanimTarzi = part[0];
                //            AXAKullanimSekli = part[1];
                //        }
                //    }
                //}



                //CSoru soru = new CSoru()
                //{
                //    CevapKod = AXAKullanimTarzi,
                //    SoruKod = AXA_KaskoSoruKodlari.KullanimTarzi

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = AXAKullanimSekli,
                //    SoruKod = AXA_KaskoSoruKodlari.KullanimSekli

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.Marka,
                //    SoruKod = AXA_KaskoSoruKodlari.Marka

                //};
                //soruList.Add(soru);


                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.AracinTipi,
                //    SoruKod = AXA_KaskoSoruKodlari.MarkaTip

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.Model.ToString().Substring(2, 2),
                //    SoruKod = AXA_KaskoSoruKodlari.Model

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = eskiPoliceVar ? AXA_KayitTipleri.Hayir : AXA_KayitTipleri.Evet,
                //    SoruKod = AXA_KaskoSoruKodlari.PlakaYeniKayitMi

                //};
                //soruList.Add(soru);

                //soru = new CSoru()
                //{
                //    CevapKod = teklif.Arac.PlakaKodu,
                //    SoruKod = AXA_KaskoSoruKodlari.PlakaIlKodu

                //};
                //soruList.Add(soru);

                //string cevapkod = "";
                //if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                //{
                //    switch (teklif.GenelBilgiler.TaksitSayisi.Value)
                //    {
                //        case 2: cevapkod = AXA_OdemeSekilleri.Iki; break;
                //        case 3: cevapkod = AXA_OdemeSekilleri.Uc; break;
                //        case 4: cevapkod = AXA_OdemeSekilleri.Dort; break;
                //        case 5: cevapkod = AXA_OdemeSekilleri.Bes; break;
                //        case 6: cevapkod = AXA_OdemeSekilleri.Alti; break;
                //        case 7: cevapkod = AXA_OdemeSekilleri.Yedi; break;
                //        default:
                //            cevapkod = AXA_OdemeSekilleri.Pesin;
                //            break;
                //    }
                //}
                //else
                //{
                //    cevapkod = AXA_OdemeSekilleri.Pesin;
                //}
                //soru = new CSoru()
                //{
                //    CevapKod = cevapkod,
                //    SoruKod = AXA_KaskoSoruKodlari.OdemeSekli

                //};
                //soruList.Add(soru);

                //if (AXAKullanimTarzi == "1")
                //{
                //    string ikameTuru = teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                //    string ikameCevap = "";
                //    if (!String.IsNullOrEmpty(ikameTuru))
                //    {
                //        switch (ikameTuru)
                //        {
                //            case "ABC07": ikameCevap = AXA_İkameSecenekleri.Yedi; break;
                //            case "ABC14": ikameCevap = AXA_İkameSecenekleri.OnBes; break;
                //        }
                //    }
                //    //Sadece Husisi Araçlar için
                //    soru = new CSoru()
                //    {
                //        CevapKod = ikameCevap,
                //        SoruKod = AXA_KaskoSoruKodlari.KiralikAracSuresi

                //    };
                //    soruList.Add(soru);
                //}
                //if (CRIMMKombineBedel != null && CRIMMKombineBedel.BedeniSahis > 0)
                //{
                //    string[] part = CRIMMKombineBedel.Kademe.Split('-');
                //    string kademe = part[0];
                //    soru = new CSoru()
                //    {
                //        CevapKod = kademe,
                //        SoruKod = AXA_KaskoSoruKodlari.IMS3lü
                //    };
                //    soruList.Add(soru);
                //}

                //if (CRIMMKombineBedel != null && CRIMMKombineBedel.Kombine > 0)
                //{
                //    soru = new CSoru()
                //    {
                //        CevapKod = CRIMMKombineBedel.Kademe,
                //        SoruKod = AXA_KaskoSoruKodlari.IMSKombine

                //    };
                //    soruList.Add(soru);
                //}

                ////ekranda olmayan sorular   
                //soru = new CSoru()
                //{
                //    CevapKod = "0",
                //    SoruKod = AXA_KaskoSoruKodlari.MarkaKasko

                //};
                //soruList.Add(soru);
                ////soru = new CSoru()
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.OnarimSecimi

                ////};
                //soru = new CSoru()
                //{
                //    CevapKod = "0",
                //    SoruKod = AXA_KaskoSoruKodlari.YeniDegerKlozu

                //};
                //soruList.Add(soru);
                //soru = new CSoru()
                //{
                //    CevapKod = "0",
                //    SoruKod = AXA_KaskoSoruKodlari.DepremSelKoasuransi

                //};
                //soruList.Add(soru);
                //soru = new CSoru()
                //{
                //    CevapKod = "4",
                //    SoruKod = AXA_KaskoSoruKodlari.MuafiyetTutari

                //};
                //soruList.Add(soru);
                ////soru = new CSoru()
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.CamFilmiLogoVarMi

                ////};
                ////soru = new CSoru()
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.DainiMurtein

                ////};
                ////soru = new CSoru()
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.SigortaliListesi

                ////};
                //soru = new CSoru()
                //{
                //    CevapKod = "0",
                //    SoruKod = AXA_KaskoSoruKodlari.EngelliAraciMi

                //};
                //soruList.Add(soru);
                //soru = new CSoru()  /////???????
                //{
                //    CevapKod = "2",
                //    SoruKod = AXA_KaskoSoruKodlari.SorumlulukLimiti

                //};
                //soruList.Add(soru);
                ////soru = new CSoru()
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.DigerAksesuar

                ////};
                ////soru = new CSoru()  /////???????
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.KasaTank

                ////};
                ////soru = new CSoru()
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.EmtiaCinsi

                ////};
                ////soru = new CSoru()    /////???????
                ////{
                ////    CevapKod = "",
                ////    SoruKod = AXA_KaskoSoruKodlari.YurtDisiSuresi
                ////};
                //soru = new CSoru()
                //{
                //    CevapKod = "1",
                //    SoruKod = AXA_KaskoSoruKodlari.ManeviTazminat

                //};
                //soruList.Add(soru);
                //soru = new CSoru()
                //{
                //    CevapKod = "T",
                //    SoruKod = AXA_KaskoSoruKodlari.AsistansSecimi

                //};
                //soruList.Add(soru);

                //#endregion

                //#region  Matbular

                //CMatbuBilgi matbu = new CMatbuBilgi();
                //matbu.BilgiAdi = "MOTOR NO";
                //matbu.Aciklama = teklif.Arac.MotorNo;
                //cmatubList.Add(matbu);

                //matbu = new CMatbuBilgi();
                //matbu.BilgiAdi = "ŞASİ NO";
                //matbu.Aciklama = teklif.Arac.SasiNo;
                //cmatubList.Add(matbu);

                //matbu = new CMatbuBilgi();
                //matbu.BilgiAdi = "PLAKA NO";
                //matbu.Aciklama = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                //cmatubList.Add(matbu);

                //matbu = new CMatbuBilgi();
                //matbu.BilgiAdi = "MARKA TİPİ";
                //matbu.Aciklama = teklif.Arac.Marka;
                //cmatubList.Add(matbu);


                //matbu = new CMatbuBilgi();
                //matbu.BilgiAdi = "Ruhsat Belge/ASBIS Numarası";
                //if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod))
                //{
                //    matbu.Aciklama = teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo;
                //}
                //else
                //{
                //    matbu.Aciklama = teklif.Arac.AsbisNo;
                //}
                //cmatubList.Add(matbu);

                //#endregion
                //cFiyatCalismasiParametre.MatbuBilgiler = cmatubList.ToArray();
                //cFiyatCalismasiParametre.Sorular = soruList.ToArray();
                //cFiyatCalismasiParametre.Teminatlar = teminatList.ToArray();


                //#region Servis           

                //this.BeginLog(cFiyatCalismasiParametre, cFiyatCalismasiParametre.GetType(), WebServisIstekTipleri.Teklif);
                //var response = axaClient.FiyatCalismasiOlustur(cFiyatCalismasiParametre);

                //var fiyatCalisma = response.FiyatCalismalari.FirstOrDefault();

                //if (response.HataMesajlari != null)
                //{
                //    this.EndLog(response, false, response.GetType());
                //    for (int i = 0; i < response.HataMesajlari.Count(); i++)
                //    {
                //        this.AddHata(response.HataMesajlari[i].ToString());
                //    }
                //}
                //else
                //{
                //    this.EndLog(response, true, response.GetType());
                //    KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                //    string testMi = konfigTestMi[Konfig.TestMi];

                //    if (!Convert.ToBoolean(testMi))
                //    {
                //        #region PDF URL                      
                //        AXAPoliceBasim.AxaPoliceBasim pdfClient = new AXAPoliceBasim.AxaPoliceBasim();
                //        pdfClient.Url = konfig[Konfig.AXA_PoliceBasim];
                //        pdfClient.Timeout = 150000;
                //        pdfClient.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);



                //        AOCPoliceBasimParametre pdfReq = new AOCPoliceBasimParametre()
                //        {
                //            BasimTipi = BasimType.Binary,
                //            DilSecimi = BasimDil.Turkce,
                //            Kullanici = servisKullanici.KullaniciAdi,
                //            PoliceNo = fiyatCalisma.AnaBilgiler.FiyatCalismaNo.Value,
                //            ZeylSiraNo = 0
                //        };

                //        var responsePDF = pdfClient.PoliceBasim(pdfReq);
                //        pdfClient.Dispose();

                //        if (responsePDF.HataMesajlari.ToList().Count > 0)
                //        {
                //            this.EndLog(responsePDF, false, responsePDF.GetType());

                //            for (int i = 0; i < responsePDF.HataMesajlari.Count(); i++)
                //            {
                //                this.AddHata(responsePDF.HataMesajlari[i].ToString());
                //            }

                //            this.AddHata("PDF dosyası alınamadı.");
                //        }
                //        else
                //        {
                //            this.EndLog(responsePDF, true, responsePDF.GetType());
                //            byte[] data = responsePDF.Binary;

                //            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                //            string fileName = String.Empty;
                //            fileName = String.Format("AXA_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                //            //pdfUrl = storage.UploadFile("kasko", fileName, data);

                //            //_Log.Info("Teklif_PDF url: {0}", pdfUrl);
                //        }
                //        #endregion
                //    }
                //}

                //#endregion

                //#region TeklifKaydet

                //#region Basarı Kontrol

                //if (!this.Basarili)
                //{
                //    this.Import(teklif);
                //    this.GenelBilgiler.Basarili = false;
                //    return;
                //}

                //#endregion

                //#region Teklif kaydı

                //#region Genel bilgiler

                //this.Import(teklif);

                //this.GenelBilgiler.Basarili = true;
                //this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                //this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                //this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                //this.GenelBilgiler.BrutPrim = fiyatCalisma.AnaBilgiler.PrimBorcu;
                //this.GenelBilgiler.TUMTeklifNo = fiyatCalisma.AnaBilgiler.FiyatCalismaNo.Value.ToString();
                //// this.GenelBilgiler.PDFDosyasi = pdfUrl;


                //#region Vergiler
                //decimal BSV = 0;
                //decimal GH = 0;
                //decimal THG = 0;
                ////foreach (var item in response.Vergiler)
                ////{
                ////    if (item.VergiKod == "BSV")
                ////    {
                ////        BSV = item.Tutar.HasValue ? item.Tutar.Value : 0;
                ////    }
                ////    else if (item.VergiKod == "GH")
                ////    {
                ////        GH = item.Tutar.HasValue ? item.Tutar.Value : 0;
                ////    }
                ////    else if (item.VergiKod == "THG")
                ////    {
                ////        THG = item.Tutar.HasValue ? item.Tutar.Value : 0;
                ////    }
                ////}

                //this.AddVergi(TrafikVergiler.THGFonu, THG);
                //this.AddVergi(TrafikVergiler.GiderVergisi, BSV);
                //this.AddVergi(TrafikVergiler.GarantiFonu, GH);

                //this.GenelBilgiler.ToplamVergi = BSV + GH + THG;
                //#endregion

                //this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                //this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                ////this.GenelBilgiler.GecikmeZammiYuzdesi = ANADOLUMessage.ToDecimal(response.GECIKME_SURPRIM_ORANI) * 100;
                ////this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                ////this.GenelBilgiler.ZKYTMSYüzdesi = 0;

                ////Odeme Bilgileri
                //this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                //this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                //this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                ////if (response.Sorular != null)
                ////{
                ////    foreach (var item in response.Sorular)
                ////    {
                ////        if (item.SoruKod == AXA_KaskoSoruKodlari.HasarsizlikIndirimi)
                ////        {
                ////            if (item.CevapKod != "1")
                ////            {
                ////                string hasarsizlikIndirimYuzdesi = String.Empty;
                ////                foreach (char ch in item.CevapAd)
                ////                {
                ////                    if (Char.IsDigit(Convert.ToChar(ch)))
                ////                    {
                ////                        hasarsizlikIndirimYuzdesi += ch;
                ////                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToInt32(hasarsizlikIndirimYuzdesi);
                ////                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                ////                    }
                ////                }
                ////            }
                ////            else
                ////            {
                ////                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                ////                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                ////            }
                ////        }
                ////    }
                ////}
                //#region Taksitler

                ////if (response.Taksitler.Count() > 0)
                ////{
                ////    this.GenelBilgiler.ToplamKomisyon = response.Taksitler.FirstOrDefault().Komisyon;
                ////}
                ////if (response.Taksitler.Count() == 1)
                ////{
                ////    this.GenelBilgiler.TaksitSayisi = 1;
                ////}
                ////else
                ////{
                ////    this.GenelBilgiler.TaksitSayisi = (byte)response.Taksitler.Count();
                ////}
                //#endregion

                //#endregion

                //#region Teminatlar

                ////if (response.Teminatlar.Count() > 0)
                ////{
                ////    foreach (var item in response.Teminatlar)
                ////    {
                ////        if (item.TeminatKod == AXA_KaskoTeminatlar.Arac)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.Kasko, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.AnahtarKaybiZararlari)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.Anahtar_Kaybi, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        //else if (item.TeminatKod == AXA_KaskoTeminatlar.IMS3Limit)
                ////        //{
                ////        //    this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        //}
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.BedeniZararKazaBasi)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.KazaBasMaddiZarar)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.BedeniZararSahisBasi)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.FerdiKazaOlum)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.KFK_Olum, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.FerdiKazaSurekliSakatlik)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.TedaviMasraflari)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.HukuksalKorumaMotorluArac)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////        else if (item.TeminatKod == AXA_KaskoTeminatlar.HukuksalKorumaSurucu)
                ////        {
                ////            this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                ////        }
                ////    }
                ////}
                //#endregion

                //#region Ödeme Planı

                //if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                //{
                //    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                //}

                //#endregion

                //#endregion

                //#endregion
                #endregion

                #region Eski service
                string pdfUrl = String.Empty;
                #region Veri Hazırlama

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAXAService);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.AXA });
                AxaApplicationWebSrvV01 client = new AxaApplicationWebSrvV01();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.Url = konfig[Konfig.AXA_ServiceURL];
                client.Timeout = 150000;
                client.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);
            
                List<CPartnerBilgi> parners = new List<CPartnerBilgi>();
                List<string> mesajlar = new List<string>();
                mesajlar.Add("deneme");

                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                //string polNo=teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                //CYenilemeDetay detay = new CYenilemeDetay();
                //detay.PoliceNo =Convert.ToDecimal(polNo);
                //detay.TanzimTarihi = polBaslangic;
                //detay.RefKullanici = servisKullanici.KullaniciAdi;
                //var rs=client.PoliceYenile(detay);

                #region Sigortali / Sigorta Ettiren Bilgileri Hazırlama

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriAdre sigortaliAdres = null;
                if (sigortali != null)
                {
                    sigortaliAdres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                }

                var sigEttiren = teklif.SigortaEttiren;
                MusteriAdre sigEttirenAdres = null;
                if (sigEttiren != null)
                {
                    sigEttirenAdres = sigEttiren.MusteriGenelBilgiler.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                }

                string axaSigIlKodu = String.Empty;
                string axaSigIlceKodu = String.Empty;

                string axaSigEttirenIlKodu = String.Empty;
                string axaSigEttirenIlceKodu = String.Empty;
                MusteriTelefon sigortaliCepTelefon = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Cep).FirstOrDefault();
                MusteriTelefon sigortaliEvTelefon = sigortali.MusteriTelefons.Where(s => s.IletisimNumaraTipi == IletisimNumaraTipleri.Ev).FirstOrDefault();
                string MusteriEvTel = "";
                string MusteriCepTel = "";

                if (sigortaliCepTelefon == null)
                {
                    sigortaliCepTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                }
                if (sigortaliEvTelefon == null)
                {
                    sigortaliEvTelefon = sigortali.MusteriTelefons.FirstOrDefault();
                }
                if (sigortaliCepTelefon != null)
                {
                    if (!String.IsNullOrEmpty(sigortaliCepTelefon.Numara) && sigortaliCepTelefon.Numara.Trim() != "90" && sigortaliCepTelefon.Numara.Length >= 10)
                    {
                        string telefon = sigortaliCepTelefon.Numara;
                        if (telefon.Length == 14)
                        {
                            MusteriCepTel = sigortaliCepTelefon.Numara.Substring(1, 1) + "(";
                            MusteriCepTel += sigortaliCepTelefon.Numara.Substring(3, 3) + ")";
                            MusteriCepTel += sigortaliCepTelefon.Numara.Substring(7, 7);
                        }
                    }
                    else
                    {
                        MusteriCepTel = "0(555)9999999";
                    }
                }
                if (sigortaliEvTelefon != null)
                {
                    if (!String.IsNullOrEmpty(sigortaliEvTelefon.Numara) && sigortaliEvTelefon.Numara.Trim() != "90" && sigortaliEvTelefon.Numara.Length >= 10)
                    {
                        string telefon = sigortaliEvTelefon.Numara;
                        if (telefon.Length == 14)
                        {
                            MusteriEvTel = sigortaliEvTelefon.Numara.Substring(1, 1) + "(";
                            MusteriEvTel += sigortaliEvTelefon.Numara.Substring(3, 3) + ")";
                            MusteriEvTel += sigortaliEvTelefon.Numara.Substring(7, 7);
                        }
                    }
                    else
                    {
                        MusteriEvTel = "0(212)9999999";
                    }
                }

                bool MusteriAyniMi = false;
                if (sigEttiren.MusteriKodu == sigortali.MusteriKodu)
                {
                    MusteriAyniMi = true;
                    if (sigortaliAdres != null)
                    {
                        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                                                                                 f.IlKodu == sigortaliAdres.IlKodu &&
                                                                                 f.IlceKodu == sigortaliAdres.IlceKodu)
                                                                   .SingleOrDefault<CR_IlIlce>();
                        if (ililce != null)
                        {
                            axaSigIlKodu = ililce.CRIlKodu;
                            axaSigIlceKodu = ililce.CRIlceKodu;
                        }
                    }
                }
                else
                {
                    MusteriAyniMi = false;

                    if (sigEttirenAdres != null)
                    {

                        CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                                                                                 f.IlKodu == sigEttirenAdres.IlKodu &&
                                                                                 f.IlceKodu == sigEttirenAdres.IlceKodu)
                                                                   .SingleOrDefault<CR_IlIlce>();
                        if (ililce != null)
                        {
                            axaSigEttirenIlKodu = ililce.CRIlKodu;
                            axaSigEttirenIlceKodu = ililce.CRIlceKodu;
                        }
                    }
                }

                #endregion

                #region Eski Poliçe Bilgileri Hazırlama

                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);

                string TramerPoliceNo = "";
                string TramerAcenteNo = "";
                string TramerSirketKodu = "";
                string TramerYenilemeNo = "";
                if (eskiPoliceVar)
                {
                    TramerPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                    TramerAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                    TramerSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    TramerYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                }

                #endregion

                #region Partner Bilgileri Hazırlama
                List<CMatbuBilgi> matBuList = new List<CMatbuBilgi>();

                #endregion

                #region Police Aciklama Bilgileri Hazırlama
                List<string> aciklamalar = new List<string>();
                #endregion

                #region Request

                #region Police Header
                bool tcMi = true;
                if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                {
                    tcMi = false;
                }
                CPoliceBaslik3 policeBaslik = new CPoliceBaslik3()
                {
                    SubeKod = 0,
                    KaynakKod = 300,
                    AcenteNo = servisKullanici.PartajNo_,
                    TaliKaynak = 0,
                    UrunKodu = AXA_UrunKodlari.Kasko,
                    Doviz_Kod = "TL",
                    Teklif_Tarih = DateTime.Now,
                    Tanzim_Tarih = DateTime.Now,
                    Baslama_Tarih = polBaslangic,
                    Bitis_Tarih = polBaslangic.AddYears(1),
                    PlakaNo = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo,
                    TescilBelgeNo = teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo,
                    OtomatikYenileme = 0,
                    RefKullanici = servisKullanici.KullaniciAdi2,
                    UretimYeri = 0,

                    //Hasarsızlık bilgisinin sorgulanması için gönderiliyor
                    TramerPoliceNo = TramerPoliceNo,
                    TramerSirketKod = TramerSirketKodu,
                    TramerAcenteNo = TramerAcenteNo,
                    TramerYenilemeNo = TramerYenilemeNo,

                    ClientIPAddress = this.IpGetir(teklif.GenelBilgiler.TVMKodu),
                    //EskiPlakaNo = "",
                    //MTKod = "",
                    //OtrMail = "",
                    //PoliceCinsi = "", // boş olabilir
                    //TaliAcente = "",                  
                    // IngEkAciklamalar = aciklamalar.ToArray(),                  
                    //TeknikPersonel = "",
                    // TurceEkAciklamalar = aciklamalar.ToArray(),

                    RizikoAdresi = new CMusteriAdres()
                    {
                        //UavtAdresId=0,
                        Ulke_Kod = "TÜR",
                        Il_Kod = axaSigIlKodu,
                        Ilce_Kod = axaSigIlceKodu,
                        Semt_Kod = "000",
                        Mahalle = ".",
                        Bina_No = "1",
                        Daire_No = "1",
                        Posta_Kod = "340600",
                        Han_Apt_Fab = ".",

                    },
                    SigortaEttiren = new CMusteriAxa()
                    {
                        Adres = new CMusteriAdres()
                        {
                            //Zorunlu alanlar.
                            Ulke_Kod = "TÜR",
                            Il_Kod = MusteriAyniMi ? axaSigIlKodu : axaSigEttirenIlKodu,
                            Ilce_Kod = MusteriAyniMi ? axaSigIlceKodu : axaSigEttirenIlceKodu,
                            Semt_Kod = "000",
                            Mahalle = MusteriAyniMi ? sigortaliAdres.Mahalle : sigEttirenAdres.Mahalle,
                            Sokak = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Sokak : sigEttirenAdres.Sokak) : ".",
                            Cadde = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.Sokak) ? sigortaliAdres.Cadde : sigEttirenAdres.Cadde) : ".",
                            Daire_No = MusteriAyniMi ? (String.IsNullOrEmpty(sigortaliAdres.DaireNo) ? sigortaliAdres.DaireNo : sigEttirenAdres.DaireNo) : "1",
                            //-------Zorunlu alanlar

                            //Posta_Kod = "",
                            //Han_Apt_Fab = "",
                            //Bina_No = "13",
                            //UavtAdresId = 0,
                        },
                        GenelBilgiler = new CMusteriGenelBilgilerAxa()
                        {

                            UlkeKod = "TÜR",
                        },
                        IletisimBilgileri = new CMusteriIletisimBilgi()
                        {
                            Email = MusteriAyniMi ? sigortali.EMail : sigEttiren.MusteriGenelBilgiler.EMail,// "deneme@deneme.com",
                            Fax_No = "",
                            GSM_No = MusteriCepTel,
                            Tel_No = MusteriEvTel,
                        },
                        //Mesajlar = mesajlar.ToArray(),
                        //PartnerBilgiler = parners.ToArray()

                    },
                    Sigortali = new CMusteriAxa()
                    {
                        Adres = new CMusteriAdres()
                        {
                            //Zorunlu alanlar.
                            Ulke_Kod = "TÜR",
                            Il_Kod = axaSigIlKodu,
                            Ilce_Kod = axaSigIlceKodu,
                            Semt_Kod = "000",
                            Mahalle = !String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Mahalle : ".",
                            Sokak = !String.IsNullOrEmpty(sigortaliAdres.Mahalle) ? sigortaliAdres.Sokak : ".",
                            Cadde = !String.IsNullOrEmpty(sigortaliAdres.Sokak) ? sigortaliAdres.Cadde : ".",
                            Daire_No = !String.IsNullOrEmpty(sigortaliAdres.DaireNo) ? sigortaliAdres.DaireNo : "1",
                            //-------Zorunlu alanlar

                            //Posta_Kod = "",
                            //Han_Apt_Fab = "",
                            //Bina_No = "13",
                            //UavtAdresId = 0,
                        },
                        GenelBilgiler = new CMusteriGenelBilgilerAxa()
                        {

                            UlkeKod = "TÜR",
                        },
                        IletisimBilgileri = new CMusteriIletisimBilgi()
                        {
                            Email = sigortali.EMail,//"deneme@deneme.com",
                            Fax_No = "",//"0(212)9999999",
                            GSM_No = MusteriCepTel,//"0(543)9999999",
                            Tel_No = MusteriEvTel,//"0(212)9999999",
                        },
                        // Mesajlar = mesajlar.ToArray(),
                        // PartnerBilgiler = parners.ToArray()
                    },


                };

                if (tcMi)
                {
                    policeBaslik.SigortaEttiren.GenelBilgiler.TcNo = MusteriAyniMi ? sigortali.KimlikNo : sigEttiren.MusteriGenelBilgiler.KimlikNo;
                    policeBaslik.Sigortali.GenelBilgiler.TcNo = sigortali.KimlikNo;
                }
                else
                {
                    policeBaslik.SigortaEttiren.GenelBilgiler.VergiNo = MusteriAyniMi ? sigortali.KimlikNo : sigEttiren.MusteriGenelBilgiler.KimlikNo;
                    policeBaslik.Sigortali.GenelBilgiler.VergiNo = sigortali.KimlikNo;
                }

                #endregion

                #region Poliçe Detail

                CPoliceDetay policeDetay = new CPoliceDetay();

                #region Teminatlar

                List<CTeminat> teminatList = new List<CTeminat>();

                #region Zorunlu Teminatlar

                CTeminat teminat = new CTeminat()
                {
                    Adet = 1,
                    Bedel = 0,
                    Iptal = false,
                    TeminatKod = AXA_KaskoTeminatlar.Arac

                };
                teminatList.Add(teminat);

                teminat = new CTeminat()
                {
                    Adet = 1,
                    Bedel = 0,
                    Iptal = false,
                    TeminatKod = AXA_KaskoTeminatlar.AnahtarKaybiZararlari,

                };
                teminatList.Add(teminat);

                string kullanimT = String.Empty;
                string kod2 = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    kullanimT = parts[0];
                    kod2 = parts[1];
                }

                string fkKademe = teklif.ReadSoru(KaskoSorular.Teminat_FK_Kademe, String.Empty);
                if (!String.IsNullOrEmpty(fkKademe))
                {
                    var FKBedel = _CRService.GetKaskoFKBedel(Convert.ToInt32(fkKademe), parts[0], parts[1]);
                    if (FKBedel != null)
                    {
                        teminat = new CTeminat()
                        {
                            Adet = 1,
                            Bedel = FKBedel.Tedavi,
                            Iptal = false,
                            TeminatKod = AXA_KaskoTeminatlar.TedaviMasraflari

                        };
                        teminatList.Add(teminat);

                        teminat = new CTeminat()
                        {
                            Adet = 1,
                            Bedel = FKBedel.Sakatlik,
                            Iptal = false,
                            TeminatKod = AXA_KaskoTeminatlar.FerdiKazaOlum

                        };
                        teminatList.Add(teminat);
                        teminat = new CTeminat()
                        {
                            Adet = 1,
                            Bedel = FKBedel.Vefat,
                            Iptal = false,
                            TeminatKod = AXA_KaskoTeminatlar.FerdiKaza

                        };
                        teminatList.Add(teminat);
                    }
                }
              
                decimal hukukBedel = teklif.ReadSoru(KaskoSorular.Hukuksal_Koruma_Kademesi, 0);


                teminat = new CTeminat()
                {
                    Adet = 1,
                    Bedel = hukukBedel,
                    Iptal = false,
                    TeminatKod = AXA_KaskoTeminatlar.HukuksalKoruma

                };
                teminatList.Add(teminat);

                teminat = new CTeminat()
                {
                    Adet = 1,
                    Bedel = 0,
                    Iptal = false,
                    TeminatKod = AXA_KaskoTeminatlar.Asistans

                };
                teminatList.Add(teminat);

                #endregion

                #region Seçimli Default
                bool ManeviDahil = false;
                decimal IMMSoru = 0;
                decimal IMMKombineSoru = 0;
                string immKademe = teklif.ReadSoru(KaskoSorular.Teminat_IMM_Kademe, String.Empty);
                KaskoIMM CR_IMMBedel = new KaskoIMM();
                CR_KaskoIMM CRIMMKombineBedel = new CR_KaskoIMM();
                if (!String.IsNullOrEmpty(immKademe))
                {
                    var IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);
                    if (IMMBedel != null)
                    {
                        //KaskoIMM tablosundan ekran girilen değerin bedeli alınıyor
                        CR_IMMBedel = _CRService.GetKaskoIMMBedel(Convert.ToInt32(immKademe), parts[0], parts[1]);

                        if (CR_IMMBedel.Text.Contains("Manevi Dahil")) ManeviDahil = true;

                        if (CR_IMMBedel != null)
                        {
                            //CR_KaskoIMM tablosundan ekran girilen değerin bedelin kademe kodu alınıyor
                            CRIMMKombineBedel = _CRService.GetCRKaskoIMMBedel(TeklifUretimMerkezleri.AXA, IMMBedel.BedeniSahis, IMMBedel.Kombine, parts[0], parts[1]);
                        }
                        if (CRIMMKombineBedel != null)
                        {
                            IMMSoru = CRIMMKombineBedel.BedeniSahis.Value;
                            IMMKombineSoru = CRIMMKombineBedel.Kombine.Value;
                        }

                        if (IMMSoru > 0)
                        {
                            teminat = new CTeminat()
                            {
                                Adet = 1,
                                Bedel = IMMSoru,
                                Iptal = false,
                                TeminatKod = AXA_KaskoTeminatlar.IMS3Limit

                            };
                            teminatList.Add(teminat);

                        }
                        if (IMMKombineSoru > 0)
                        {
                            teminat = new CTeminat()
                            {
                                Adet = 1,
                                Bedel = IMMKombineSoru,
                                Iptal = false,
                                TeminatKod = AXA_KaskoTeminatlar.IMSKombine

                            };
                            teminatList.Add(teminat);
                        }

                    }
                }
                else
                {
                    teminat = new CTeminat()
                    {
                        Adet = 1,
                        Bedel = 0,
                        Iptal = false,
                        TeminatKod = AXA_KaskoTeminatlar.IMS3Limit

                    };
                    teminatList.Add(teminat);
                }

                #endregion

                #region Seçimli
                var aracaBagliKaravan = teklif.ReadSoru(KaskoSorular.AxaAracaBagliKaravanVarMi, false);
                if (aracaBagliKaravan)
                {
                    var aracaBagliKaravanBedel = teklif.ReadSoru(KaskoSorular.AxaAracaBagliKaravanBedeli, "0");
                    teminat = new CTeminat()
                    {
                        Adet = 1,
                        Bedel = Convert.ToDecimal(aracaBagliKaravanBedel),
                        Iptal = false,
                        TeminatKod = AXA_KaskoTeminatlar.AracaBagliKaravan
                    };
                }
                teminatList.Add(teminat);

                var kullanimGelirKaybi = teklif.ReadSoru(KaskoSorular.KullanimGelirKaybiVarMi, false);
                if (kullanimGelirKaybi)
                {
                    var kullanimGelirKaybiBedel = teklif.ReadSoru(KaskoSorular.KullanimGelirKaybiBedel, "0");
                    teminat = new CTeminat()
                    {
                        Adet = 1,
                        Bedel = Convert.ToDecimal(kullanimGelirKaybiBedel),
                        Iptal = false,
                        TeminatKod = AXA_KaskoTeminatlar.KullanimGelirKaybi
                    };
                }
                teminatList.Add(teminat);

                var elektrikliArac = teklif.ReadSoru(KaskoSorular.ElektrikliArac, false);
                if (elektrikliArac)
                {
                    var elektrikliAracBedel = teklif.ReadSoru(KaskoSorular.ElektrikliAracBedeli, "0");
                    teminat = new CTeminat()
                    {
                        Adet = 1,
                        Bedel = Convert.ToDecimal(elektrikliAracBedel),
                        Iptal = false,
                        TeminatKod = AXA_KaskoTeminatlar.ElektrikliAracBedeli
                    };
                }


                decimal? KasaBedeli = 0;
                decimal digerAksesuarBedel = 0;
                #region Araç aksesuarlar
                if (teklif.AracEkSorular.Count > 0)
                {
                    #region Aksesuarlar
                    List<TeklifAracEkSoru> aksesuarlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.AKSESUAR ||
                                                                                         w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                                                             .ToList<TeklifAracEkSoru>();

                    if (aksesuarlar.Count > 0)
                    {
                        foreach (TeklifAracEkSoru item in aksesuarlar)
                        {
                            if (item.SoruKodu == MapfreAksesuarTipleri.Kasa)
                            {
                                KasaBedeli = item.Bedel;
                            }
                            else
                            {
                                digerAksesuarBedel += item.Bedel.HasValue ? item.Bedel.Value : 0;
                            }
                        }
                    }
                    #endregion

                    #region Elektronik Cihaz Listesi
                    //List<TeklifAracEkSoru> elekCihazlar = teklif.AracEkSorular.Where(w => w.SoruTipi == MapfreKaskoEkSoruTipleri.ELEKTRONIK_CIHAZ)
                    //                                                          .ToList<TeklifAracEkSoru>();
                    //if (elekCihazlar.Count > 0)
                    //{
                    //    foreach (TeklifAracEkSoru item in elekCihazlar)
                    //    {
                    //        //element.listElements = new mapfre.SahaWS[3];
                    //        //element.listElements[0] = new mapfre.SahaWS() { p_cod_campo = "COD_ELEKTRONIK_CIHAZ", p_val_campo = item.SoruKodu };
                    //        //element.listElements[1] = new mapfre.SahaWS() { p_cod_campo = "TXT_ELEKTRONIK_CIHAZ_ACIKLAMA", p_val_campo = item.Aciklama };
                    //        //element.listElements[2] = new mapfre.SahaWS() { p_cod_campo = "VAL_ELEKTRONIK_CIHAZ_BEDEL", p_val_campo = Convert.ToString(item.Bedel) };

                    //    }
                    //}
                    #endregion
                }
                #endregion

                if (KasaBedeli != 0)
                {
                    teminat = new CTeminat()
                    {
                        Adet = 1,
                        Bedel = KasaBedeli,
                        Iptal = false,
                        TeminatKod = AXA_KaskoTeminatlar.KasaTank,
                    };
                    teminatList.Add(teminat);
                }
                if (digerAksesuarBedel != 0)
                {
                    teminat = new CTeminat()
                    {
                        Adet = 1,
                        Bedel = digerAksesuarBedel,
                        Iptal = false,
                        TeminatKod = AXA_KaskoTeminatlar.DigerAksesuarlar,
                    };
                    teminatList.Add(teminat);
                }

                //teminat = new CTeminat()
                //{
                //    Adet = 1,
                //    Bedel = 0,
                //    Iptal = false,
                //    TeminatKod = AXA_KaskoTeminatlar.TasinanYuk

                //};
                //teminatList.Add(teminat);

                var yurtDisiKasko = teklif.ReadSoru(KaskoSorular.YurtdisiKasko, false);
                if (yurtDisiKasko)
                {
                    var yurtDisiSuresi = teklif.ReadSoru(KaskoSorular.Yurt_Disi_Teminati_Suresi, "");
                    if (!string.IsNullOrEmpty(yurtDisiSuresi))
                    {
                        teminat = new CTeminat()
                        {
                            Adet = 1,
                            Bedel = 0,
                            Iptal = false,
                            TeminatKod = AXA_KaskoTeminatlar.YurtDisiKasko

                        };
                        teminatList.Add(teminat);
                    }
                }

                string HayatLimiti = teklif.ReadSoru(KaskoSorular.AxaHayatTeminatLimiti, "");
                if (!String.IsNullOrEmpty(HayatLimiti))
                {
                    decimal bedel = 0;

                    if (HayatLimiti == "1")
                    {
                        bedel = AxaHayatLimiti.BesBin;
                    }
                    else
                    {
                        bedel = AxaHayatLimiti.YediBinBesYuz;
                    }
                    teminat = new CTeminat()
                    {
                        Adet = 1,
                        Bedel = bedel,
                        Iptal = false,
                        TeminatKod = AXA_KaskoTeminatlar.HayatTeminati

                    };
                    teminatList.Add(teminat);
                }

                string AsistansHizmeti = teklif.ReadSoru(KaskoSorular.AxaAsistansHizmeti, "T");
                teminat = new CTeminat()
                {
                    Adet = 1,
                    Bedel = 0,
                    Iptal = false,
                    TeminatKod = AXA_KaskoTeminatlar.Asistans

                };
                teminatList.Add(teminat);
                #endregion

                policeDetay.Teminatlar = teminatList.ToArray();

                #endregion

                #region İstatistikler

                string AXAKullanimTarzi = String.Empty;
                string AXAKullanimSekli = String.Empty;

                if (parts.Length == 2)
                {
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.AXA &&
                                                                                                  f.KullanimTarziKodu == kullanimT &&
                                                                                                  f.Kod2 == kod2)
                                                                                                  .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                    {
                        string[] part = kullanimTarzi.TarifeKodu.Split('-');
                        if (part.Length > 0)
                        {
                            AXAKullanimTarzi = part[0];
                            AXAKullanimSekli = part[1];
                        }
                    }
                }

                List<CSoru> soruList = new List<CSoru>();

                CSoru soru = new CSoru()
                {
                    CevapKod = AXAKullanimTarzi,
                    SoruKod = AXA_KaskoSoruKodlari.KullanimTarzi

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = AXAKullanimSekli,
                    SoruKod = AXA_KaskoSoruKodlari.KullanimSekli

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = teklif.Arac.Marka,
                    SoruKod = AXA_KaskoSoruKodlari.Marka

                };
                soruList.Add(soru);


                soru = new CSoru()
                {
                    CevapKod = teklif.Arac.AracinTipi.PadLeft(3, '0'),
                    SoruKod = AXA_KaskoSoruKodlari.MarkaTip

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = teklif.Arac.Model.ToString().Substring(2, 2),
                    SoruKod = AXA_KaskoSoruKodlari.Model

                };
                soruList.Add(soru);
                bool yeniKayitMi = teklif.ReadSoru(KaskoSorular.AxaPlakaYeniKayitMi, false);
                soru = new CSoru()
                {
                    CevapKod = yeniKayitMi ? AXA_KayitTipleri.Evet : AXA_KayitTipleri.Hayir,
                    SoruKod = AXA_KaskoSoruKodlari.PlakaYeniKayitMi

                };
                soruList.Add(soru);

                soru = new CSoru()
                {
                    CevapKod = teklif.Arac.PlakaKodu,
                    SoruKod = AXA_KaskoSoruKodlari.PlakaIlKodu

                };
                soruList.Add(soru);

                string cevapkod = "";
                if (teklif.GenelBilgiler.OdemeSekli == OdemeSekilleri.Vadeli)
                {
                    switch (teklif.GenelBilgiler.TaksitSayisi.Value)
                    {
                        case 2: cevapkod = AXA_OdemeSekilleri.Iki; break;
                        case 3: cevapkod = AXA_OdemeSekilleri.Uc; break;
                        case 4: cevapkod = AXA_OdemeSekilleri.Dort; break;
                        case 5: cevapkod = AXA_OdemeSekilleri.Bes; break;
                        case 6: cevapkod = AXA_OdemeSekilleri.Alti; break;
                        case 7: cevapkod = AXA_OdemeSekilleri.Yedi; break;
                        default:
                            cevapkod = AXA_OdemeSekilleri.Pesin;
                            break;
                    }
                }
                else
                {
                    cevapkod = AXA_OdemeSekilleri.Pesin;
                }
                soru = new CSoru()
                {
                    CevapKod = cevapkod,
                    SoruKod = AXA_KaskoSoruKodlari.OdemeSekli

                };
                soruList.Add(soru);

                if (AXAKullanimTarzi == "1")
                {
                    //string ikameTuru = teklif.ReadSoru(KaskoSorular.AxaIkameSecimi, String.Empty);
                    //if (!String.IsNullOrEmpty(ikameTuru))
                    //{
                    //    //Sadece Husisi Araçlar için
                    //    soru = new CSoru()
                    //    {
                    //        CevapKod = ikameTuru,
                    //        SoruKod = AXA_KaskoSoruKodlari.KiralikAracSuresi
                    //    };
                    //    soruList.Add(soru);
                    //}
                    if (teklif.ReadSoru(KaskoSorular.Ikame_Arac_Teminati_VarYok, false))
                    {
                        string ikame = teklif.ReadSoru(KaskoSorular.Ikame_Turu, String.Empty);
                        switch (ikame)
                        {
                            case "ABC07":
                                {
                                    soru = new CSoru()
                                    {
                                        CevapKod = "7", //7GÜN
                                        SoruKod = AXA_KaskoSoruKodlari.KiralikAracSuresi
                                    };
                                    soruList.Add(soru);
                                }
                                break;
                            case "ABC14":
                                {
                                    soru = new CSoru()
                                    {
                                        CevapKod = "15", //15GÜN
                                        SoruKod = AXA_KaskoSoruKodlari.KiralikAracSuresi
                                    };
                                    soruList.Add(soru);
                                }
                                break;
                        }
                    }

                }
                if (CRIMMKombineBedel != null && CRIMMKombineBedel.BedeniSahis > 0)
                {
                    string[] part = CRIMMKombineBedel.Kademe.Split('-');
                    string kademe = part[0];
                    soru = new CSoru()
                    {
                        CevapKod = kademe,
                        SoruKod = AXA_KaskoSoruKodlari.IMS3lü
                    };
                    soruList.Add(soru);
                }

                if (CRIMMKombineBedel != null && CRIMMKombineBedel.Kombine > 0)
                {
                    string[] part = CRIMMKombineBedel.Kademe.Split('-');
                    string kademe = part[0];
                    soru = new CSoru()
                    {
                        CevapKod = kademe,
                        SoruKod = AXA_KaskoSoruKodlari.IMSKombine

                    };
                    soruList.Add(soru);
                }
                if (KasaBedeli != 0)
                {
                    soru = new CSoru()  /////???????
                    {
                        CevapKod = "2", //1Sonradan Monte 2 Orjinal
                        SoruKod = AXA_KaskoSoruKodlari.KasaTank

                    };
                    soruList.Add(soru);
                }
                //ekranda olmayan sorular 
                if (teklif.Arac.Marka=="053") //Frod Araçlarda zorunlu istiyor. Diğer markalarda zorunlu değil
                {
                    soru = new CSoru()
                    {
                        CevapKod = "5",
                        SoruKod = AXA_KaskoSoruKodlari.MarkaKasko

                    };
                    soruList.Add(soru);
                }

                //soru = new CSoru()
                //{
                //    CevapKod = "",
                //    SoruKod = AXA_KaskoSoruKodlari.DainiMurtein

                //};
                //soru = new CSoru()
                //{
                //    CevapKod = "",
                //    SoruKod = AXA_KaskoSoruKodlari.SigortaliListesi

                //};               

                if (digerAksesuarBedel != 0)
                {
                    soru = new CSoru()
                    {
                        CevapKod = "1",
                        SoruKod = AXA_KaskoSoruKodlari.DigerAksesuar
                    };
                    soruList.Add(soru);
                }


                //soru = new CSoru()
                //{
                //    CevapKod = "",
                //    SoruKod = AXA_KaskoSoruKodlari.EmtiaCinsi

                //};
                //var yurtDisiKasko = teklif.ReadSoru(KaskoSorular.YurtdisiKasko,false);
                if (yurtDisiKasko)
                {
                    soru = new CSoru()    /////???????
                    {
                        CevapKod = "1",
                        SoruKod = AXA_KaskoSoruKodlari.YurtDisiSuresi
                    };
                    soruList.Add(soru);
                }

                if (ManeviDahil)
                {
                    soru = new CSoru()
                    {
                        CevapKod = "1",//Evet
                        SoruKod = AXA_KaskoSoruKodlari.ManeviTazminat

                    };
                    soruList.Add(soru);
                }
                else
                {
                    soru = new CSoru()
                    {
                        CevapKod = "0",//Hayır
                        SoruKod = AXA_KaskoSoruKodlari.ManeviTazminat

                    };
                    soruList.Add(soru);
                }


                soru = new CSoru()
                {
                    CevapKod = AsistansHizmeti,
                    SoruKod = AXA_KaskoSoruKodlari.AsistansSecimi

                };
                soruList.Add(soru);

                if (!String.IsNullOrEmpty(HayatLimiti))
                {
                    soru = new CSoru()
                    {
                        CevapKod = HayatLimiti,
                        SoruKod = AXA_KaskoSoruKodlari.HayatTeminatLimiti //Sadece Şahıs Müşterileri için verilebilir.
                    };
                    soruList.Add(soru);

                }

                if (teklif.Arac.Model >= TurkeyDateTime.Now.Year - 1)
                {

                    //Araç 1 Yaş Soruları
                    var OnarimSecimi = teklif.ReadSoru(KaskoSorular.AxaOnarimSecimi, "");
                    if (OnarimSecimi != "")
                    {
                        soru = new CSoru()
                        {
                            CevapKod = OnarimSecimi,
                            SoruKod = AXA_KaskoSoruKodlari.OnarimSecimi
                        };
                        soruList.Add(soru);
                    }

                    var PlakaYeniKayitMi = teklif.ReadSoru(KaskoSorular.AxaPlakaYeniKayitMi, false);
                    string PlakaYeniMi = "2";
                    if (PlakaYeniKayitMi)
                    {
                        PlakaYeniMi = "1";
                    }
                    soru = new CSoru()
                    {
                        CevapKod = PlakaYeniMi,
                        SoruKod = AXA_KaskoSoruKodlari.PlakaYeniKayitMi
                    };
                    soruList.Add(soru);
                    soru = new CSoru()
                    {
                        CevapKod = teklif.Arac.PlakaKodu,
                        SoruKod = AXA_KaskoSoruKodlari.PlakaIlKodu
                    };
                    soruList.Add(soru);
                    soru = new CSoru()
                    {
                        CevapKod = teklif.ReadSoru(KaskoSorular.AxaYeniDegerKlozu, ""),
                        SoruKod = AXA_KaskoSoruKodlari.YeniDegerKlozu
                    };
                    soruList.Add(soru);
                    string depremSel = teklif.ReadSoru(KaskoSorular.AxaDepremSelKoasuransi, "");
                    if (depremSel != "")
                    {
                        soru = new CSoru()
                        {
                            CevapKod = depremSel,
                            SoruKod = AXA_KaskoSoruKodlari.DepremSelKoasuransi
                        };
                    }
                    
                    soruList.Add(soru);
                    string AxaMuafiyet = teklif.ReadSoru(KaskoSorular.AxaMuafiyetTutari, "");
                    if (AxaMuafiyet!="")
                    {
                        soru = new CSoru()
                        {
                            CevapKod = AxaMuafiyet,
                            SoruKod = AXA_KaskoSoruKodlari.MuafiyetTutari
                        };
                        soruList.Add(soru);
                    }
                   

                    soru = new CSoru()
                    {
                        CevapKod = teklif.ReadSoru(KaskoSorular.AxaSorumlulukLimiti, ""),
                        SoruKod = AXA_KaskoSoruKodlari.SorumlulukLimiti
                    };
                    soruList.Add(soru);
                    bool engelliAracimi = teklif.ReadSoru(KaskoSorular.EngelliAracimi, false);
                    string engelliAraci = "0";
                    if (engelliAracimi)
                    {
                        engelliAraci = "1";
                    }
                    soru = new CSoru()
                    {
                        CevapKod = engelliAraci,
                        SoruKod = AXA_KaskoSoruKodlari.EngelliAraciMi

                    };
                    soruList.Add(soru);

                    var CamFilmiLogoVarMi = teklif.ReadSoru(KaskoSorular.AxaCamFilmiLogo, false);
                    string CamFilmiLogo = "0";
                    if (CamFilmiLogoVarMi)
                    {
                        CamFilmiLogo = "1";
                    }
                    soru = new CSoru()
                    {
                        CevapKod = CamFilmiLogo,
                        SoruKod = AXA_KaskoSoruKodlari.CamFilmiLogoVarMi
                    };
                    soruList.Add(soru);
                }
                if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri)
                {
                    soru = new CSoru()
                    {
                        CevapKod = "28", //BÜRO VE YAZIHANELER
                        SoruKod = AXA_KaskoSoruKodlari.FaliyetKodu
                    };
                    soruList.Add(soru);
                }

                policeDetay.Sorular = soruList.ToArray();

                #endregion

                #region  Matbular

                CMatbuBilgi matbu = new CMatbuBilgi();
                matbu.BilgiAdi = "MOTOR NO";
                matbu.Aciklama = teklif.Arac.MotorNo;
                matBuList.Add(matbu);

                matbu = new CMatbuBilgi();
                matbu.BilgiAdi = "ŞASİ NO";
                matbu.Aciklama = teklif.Arac.SasiNo;
                matBuList.Add(matbu);

                matbu = new CMatbuBilgi();
                matbu.BilgiAdi = "PLAKA NO";
                matbu.Aciklama = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                matBuList.Add(matbu);

                matbu = new CMatbuBilgi();
                matbu.BilgiAdi = "MARKA TİPİ";
                matbu.Aciklama = teklif.Arac.Marka;
                matBuList.Add(matbu);


                matbu = new CMatbuBilgi();
                matbu.BilgiAdi = "Ruhsat Belge/ASBIS Numarası";
                if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriKod))
                {
                    matbu.Aciklama = teklif.Arac.TescilSeriKod + teklif.Arac.TescilSeriNo;
                }
                else
                {
                    matbu.Aciklama = teklif.Arac.AsbisNo;
                }
                matBuList.Add(matbu);


                if (elektrikliArac)
                {
                    var elektrikliAracPil = teklif.ReadSoru(KaskoSorular.ElektrikliAracPilId, "0");
                    matbu = new CMatbuBilgi();
                    matbu.BilgiAdi = "Pil ID No:";
                    matbu.Aciklama = elektrikliAracPil;
                    matBuList.Add(matbu);
                }
                policeBaslik.MatbuBilgiler = matBuList.ToArray();

                #endregion

                #endregion

                #endregion

                #region Servis
                Request req = new Request();
                req.CPoliceBaslik3 = policeBaslik;
                req.CPoliceDetay = policeDetay;

                this.BeginLog(req, req.GetType(), WebServisIstekTipleri.Teklif);
                var response = client.TeklifHazirla(policeBaslik, policeDetay);
                client.Dispose();


                if (response.HataMesajlari != null && (response.TeklifNumarasi == null || response.TeklifNumarasi == 0))
                {
                    this.EndLog(response, false, response.GetType());
                    for (int i = 0; i < response.HataMesajlari.Count(); i++)
                    {
                        this.AddHata(response.HataMesajlari[i].ToString());
                    }
                }
                else
                {
                    this.EndLog(response, true, response.GetType());
                    KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                    string testMi = konfigTestMi[Konfig.TestMi];
                    if (!Convert.ToBoolean(testMi))
                    {
                        #region PDF URL                      
                        AXAPoliceBasim.AxaPoliceBasim pdfClient = new AXAPoliceBasim.AxaPoliceBasim();
                        pdfClient.Url = konfig[Konfig.AXA_PoliceBasim];
                        pdfClient.Timeout = 150000;
                        pdfClient.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);

                        AOCPoliceBasimParametre pdfReq = new AOCPoliceBasimParametre()
                        {
                            BasimTipi = BasimType.Binary,
                            DilSecimi = BasimDil.Turkce,
                            Kullanici = servisKullanici.KullaniciAdi2,
                            PoliceNo = response.TeklifNumarasi.Value,
                            ZeylSiraNo = 0
                        };

                        var responsePDF = pdfClient.PoliceBasim(pdfReq);
                        pdfClient.Dispose();

                        if (responsePDF.HataMesajlari.ToList().Count > 0)
                        {
                            //this.EndLog(responsePDF, false, responsePDF.GetType());

                            //for (int i = 0; i < responsePDF.HataMesajlari.Count(); i++)
                            //{
                            //    this.AddHata(responsePDF.HataMesajlari[i].ToString());
                            //}

                            //this.AddHata("PDF dosyası alınamadı.");
                        }
                        else
                        {
                            this.EndLog(responsePDF, true, responsePDF.GetType());
                            byte[] data = responsePDF.Binary;

                            ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                            string fileName = String.Empty;
                            fileName = String.Format("AXA_Kasko_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                            pdfUrl = storage.UploadFile("kasko", fileName, data);

                            _Log.Info("Teklif_PDF url: {0}", pdfUrl);
                        }
                        #endregion
                    }
                }

                #endregion

                #region TeklifKaydet

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
                this.GenelBilgiler.BrutPrim = response.PrimBorcu;
                this.GenelBilgiler.TUMTeklifNo = response.TeklifNumarasi.ToString();
                this.GenelBilgiler.PDFDosyasi = pdfUrl;


                #region Vergiler
                decimal BSV = 0;
                decimal GH = 0;
                decimal THG = 0;
                foreach (var item in response.Vergiler)
                {
                    if (item.VergiKod == "BSV")
                    {
                        BSV = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    }
                    else if (item.VergiKod == "GH")
                    {
                        GH = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    }
                    else if (item.VergiKod == "THG")
                    {
                        THG = item.Tutar.HasValue ? item.Tutar.Value : 0;
                    }
                }

                this.AddVergi(TrafikVergiler.THGFonu, THG);
                this.AddVergi(TrafikVergiler.GiderVergisi, BSV);
                this.AddVergi(TrafikVergiler.GarantiFonu, GH);

                this.GenelBilgiler.ToplamVergi = BSV + GH + THG;
                #endregion

                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                //this.GenelBilgiler.GecikmeZammiYuzdesi = ANADOLUMessage.ToDecimal(response.GECIKME_SURPRIM_ORANI) * 100;
                //this.GenelBilgiler.GecikmeZammiYuzdesi = 0;
                //this.GenelBilgiler.ZKYTMSYüzdesi = 0;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;

                if (response.Sorular != null)
                {
                    foreach (var item in response.Sorular)
                    {
                        if (item.SoruKod == AXA_KaskoSoruKodlari.HasarsizlikIndirimi)
                        {
                            if (item.CevapKod != "1")
                            {
                                string hasarsizlikIndirimYuzdesi = String.Empty;
                                foreach (char ch in item.CevapAd)
                                {
                                    if (Char.IsDigit(Convert.ToChar(ch)))
                                    {
                                        hasarsizlikIndirimYuzdesi += ch;
                                        this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Convert.ToInt32(hasarsizlikIndirimYuzdesi);
                                        this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                                    }
                                }
                            }
                            else
                            {
                                this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                                this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                            }
                        }
                    }
                }
                #region Taksitler

                if (response.Taksitler.Count() > 0)
                {
                    this.GenelBilgiler.ToplamKomisyon = response.Taksitler.FirstOrDefault().Komisyon;
                }
                if (response.Taksitler.Count() == 1)
                {
                    this.GenelBilgiler.TaksitSayisi = 1;
                }
                else
                {
                    this.GenelBilgiler.TaksitSayisi = (byte)response.Taksitler.Count();
                }
                #endregion

                #endregion

                #region Teminatlar

                if (response.Teminatlar.Count() > 0)
                {
                    foreach (var item in response.Teminatlar)
                    {
                        if (item.TeminatKod == AXA_KaskoTeminatlar.Arac)
                        {
                            this.AddTeminat(KaskoTeminatlar.Kasko, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.AnahtarKaybiZararlari)
                        {
                            this.AddTeminat(KaskoTeminatlar.Anahtar_Kaybi, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        //else if (item.TeminatKod == AXA_KaskoTeminatlar.IMS3Limit)
                        //{
                        //    this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        //}
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.BedeniZararKazaBasi)
                        {
                            this.AddTeminat(KaskoTeminatlar.IMM_Kaza_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.KazaBasMaddiZarar)
                        {
                            this.AddTeminat(KaskoTeminatlar.IMM_Maddi_Hasar, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.BedeniZararSahisBasi)
                        {
                            this.AddTeminat(KaskoTeminatlar.IMM_Sahis_Basina, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.FerdiKazaOlum)
                        {
                            this.AddTeminat(KaskoTeminatlar.KFK_Olum, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.FerdiKazaSurekliSakatlik)
                        {
                            this.AddTeminat(KaskoTeminatlar.KFK_Surekli_Sakatlik, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.TedaviMasraflari)
                        {
                            this.AddTeminat(KaskoTeminatlar.KFK_Tedavi, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.HukuksalKorumaMotorluArac)
                        {
                            this.AddTeminat(KaskoTeminatlar.HK_Motorlu_Araca_Bagli, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                        else if (item.TeminatKod == AXA_KaskoTeminatlar.HukuksalKorumaSurucu)
                        {
                            this.AddTeminat(KaskoTeminatlar.HK_Surucuye_Bagli, Convert.ToDecimal(item.Bedel), 0, item.NetPrim.Value, 0, Convert.ToInt32(item.Adet));
                        }
                    }
                }
                #endregion

                #region Ödeme Planı

                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                #endregion
                #region Web Servis Cevaplar


                string AxaUyariMesaji = "";
                if (response.UyariMesajlari != null)
                {
                    for (int i = 0; i < response.UyariMesajlari.Count(); i++)
                    {
                        AxaUyariMesaji += response.UyariMesajlari[i];
                    }
                    if (AxaUyariMesaji.Length <= 1000)
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifUyariMesaji, AxaUyariMesaji);
                    }
                    else
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifUyariMesaji, AxaUyariMesaji.Substring(0, 999));
                    }
                }
                string AxaBilgiMesaji = "";
                if (response.BilgiMesajlari != null)
                {
                    for (int i = 0; i < response.BilgiMesajlari.Count(); i++)
                    {
                        AxaBilgiMesaji += response.BilgiMesajlari[i];
                    }
                    if (AxaBilgiMesaji.Length <= 1000)
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AxaBilgiMesaji);
                    }
                    else
                    {
                        this.AddWebServisCevap(Common.WebServisCevaplar.TeklifBilgiMesaji, AxaBilgiMesaji.Substring(0, 999));
                    }
                }

                #endregion
                #endregion

                #endregion


                #endregion
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
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleAXAService);

                AxaApplicationWebSrvV01 client = new AxaApplicationWebSrvV01();
                client.Url = konfig[Konfig.AXA_ServiceURL];
                client.Timeout = 150000;
                client.Credentials = new System.Net.NetworkCredential(konfig[Konfig.AXA_USERNAME], konfig[Konfig.AXA_PASSWORD]);

              

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);

                bool KrediKartMi = false;

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti || odeme.OdemeTipi == OdemeTipleri.BlokeliKrediKarti)
                {
                    KrediKartMi = true;
                }

                List<AOCMustTahsilatBilgileri> list = new List<AOCMustTahsilatBilgileri>();
                if (KrediKartMi)
                {

                    string[] isim = odeme.KrediKarti.KartSahibi.Split(' ');

                    AOCMustTahsilatBilgileri item = new AOCMustTahsilatBilgileri()
                    {
                        SiraNo = 0,
                        Tutar = teklif.GenelBilgiler.BrutPrim,
                        CardNumber = odeme.KrediKarti.KartNo,
                        LastExpDate = TurkeyDateTime.Now,
                        CVC = odeme.KrediKarti.CVC,
                        BorcluIlkAd = isim[0],
                        BorcluOrtaAd = isim.Length == 3 ? isim[1] : "",
                        BorcluSoyadAd = isim.Length == 3 ? isim[2] : isim[1],
                        Load = null
                    };
                    list.Add(item);
                }

                CIslemDetay req = new CIslemDetay()
                {
                    IslemTip = AXA_IslemTipleri.Police,
                    MustTahsilatBilgileriList = new AOCMustTahsilatBilgileri_List()
                    {
                        Items = KrediKartMi ? list.ToArray() : null,

                    },
                    PoliceNo = Convert.ToDecimal(teklif.GenelBilgiler.TUMTeklifNo),
                    //RedNedeni="",
                    RefKullanici = "",
                    ZeylNo = 0,

                };
                this.BeginLog(req, req.GetType(), WebServisIstekTipleri.Police);
                var response = client.TeklifOnayRedIptal(req);


            }
            catch (Exception ex)
            {
                #region Hata Log
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
        }
        public string IpGetir(int tvmKodu)
        {
            //string cagriIP = "82.222.165.62";
            string cagriIP = "85.105.78.56";
            string gonderilenIp = String.Empty;
            gonderilenIp = ClientIPNo;
            var tvmDetay = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == tvmKodu);
            if (tvmDetay != null)
            {
                var bagliOlduguTVMKodu = tvmDetay.BagliOlduguTVMKodu;
                if (bagliOlduguTVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.CagriTVMKodu || tvmKodu == NeosinerjiTVM.CagriBrokerTVMKodu)
                {
                    gonderilenIp = cagriIP;
                }
            }
            // gonderilenIp = "85.105.78.56";
            return gonderilenIp;
           // return "81.214.50.9";//Mb Grup ip
        }
        public class Request
        {
            [XmlElement]
            public CPoliceBaslik3 CPoliceBaslik3 { get; set; }

            [XmlElement]
            public CPoliceDetay CPoliceDetay { get; set; }
        }

    }
}
