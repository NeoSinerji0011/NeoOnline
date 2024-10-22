using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neosinerji.BABOnlineTP.Business.sompojapantrafik; 
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Business.Common.SOMPOJAPANCommon;


using OutSourceLib;
using System.Net; 

namespace Neosinerji.BABOnlineTP.Business.SOMPOJAPAN
{

    public class SOMPOJAPANTrafik : Teklif, ISOMPOJAPANTrafik
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
        IParametreContext _Context;
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullaniciService;

        [InjectionConstructor]
        public SOMPOJAPANTrafik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IParametreContext context, ITVMService TVMService,IAktifKullaniciService aktifKullaniciService)
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
            _Context = context;
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
            sompojapan.trafik.Traffic clntTrafik = new sompojapan.trafik.Traffic();
            try
            {
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.SOMPOJAPAN });

                #region Yeni ws

                clntTrafik.IdentityHeaderValue = new sompojapan.trafik.IdentityHeader()
                {
                    KullaniciAdi = servisKullanici.KullaniciAdi,
                    KullaniciParola = servisKullanici.Sifre,
                    KullaniciIP = this.IpGetir(_AktifKullaniciService.TVMKodu),
                    KullaniciTipi = Neosinerji.BABOnlineTP.Business.sompojapan.trafik.ClientType.ACENTE
                };

                KonfigTable konfigTrafik = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANTrafikV2);
                clntTrafik.Url = konfigTrafik[Konfig.SOMPOJAPAN_TrafikServiceURLV2];
                clntTrafik.Timeout = 150000;

                #region Veri Hazırlama GENEL

                string MUSTERI_NO = String.Empty;

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                MusteriAdre adress = _MusteriService.GetDefaultAdres(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);

                #endregion

                #region Arac Bilgileri

                sompojapan.trafik.ProposalParameters trafik = new sompojapan.trafik.ProposalParameters();

                string rakam = "";
                string harf = "";
                foreach (char ch in teklif.Arac.PlakaNo)
                {
                    if (Char.IsDigit(ch))
                        rakam += ch;
                    if (Char.IsLetter(ch))
                        harf += ch;
                }

                trafik.Insured = this.SigortaliBilgileri(teklif, clntTrafik);
                trafik.Customer = this.SigortaEttirenBilgileri(teklif, clntTrafik);

                trafik.PlateState = teklif.Arac.PlakaKodu;
                trafik.PlateChar = harf;
                trafik.PlateNumber = rakam;
                trafik.TypeCode = 0;


                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                List<sompojapan.trafik.CustomParameter> parametersList = new List<sompojapan.trafik.CustomParameter>();
                sompojapan.trafik.CustomParameter parameter = new sompojapan.trafik.CustomParameter();
                var sompoJapanMarkaModel = "";
                string tramerIslemTipi = teklif.ReadSoru(TrafikSorular.TramerIslemTipi, String.Empty);
                if (tramerIslemTipi == "11")
                {
                    eskiPoliceVar = false;
                }
                if (!eskiPoliceVar)
                {
                    var sompoJapanMarkaList = clntTrafik.BrandsWithCodeExt();
                    clntTrafik.Dispose();
                    string aracTipi = teklif.Arac.AracinTipi.Trim(); //babonline arac tipi

                    if (aracTipi.Length < 3)
                    {
                        aracTipi = "0" + aracTipi;
                    }
                    if (sompoJapanMarkaList.Result != null)
                    {
                        var sompoJapanMarka = sompoJapanMarkaList.Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue;
                        if (sompoJapanMarka != null)
                        {
                            string markaModel = sompoJapanMarka.Trim() + aracTipi;
                            sompoJapanMarkaModel = clntTrafik.ModelsViaBrandCodeExt(sompoJapanMarka.ToString()).Result.FirstOrDefault(k => k.ItemValue.Trim() == markaModel).ItemValue.Trim();
                            clntTrafik.Dispose();
                            if (sompoJapanMarkaModel != null)
                            {
                                parameter.code = SompoJapan_TrafikSoruTipleri.MarkaModel;
                                parameter.value = sompoJapanMarkaModel;
                                parametersList.Add(parameter);
                            }
                        }
                    }

                    parameter = new sompojapan.trafik.CustomParameter();
                    parameter.code = SompoJapan_TrafikSoruTipleri.TrafigeCikisTarihi;
                    parameter.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
                    parametersList.Add(parameter);

                    string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                    if (parts.Length == 2)
                    {
                        string kullanimTarziKodu = parts[0];
                        string kod2 = parts[1];
                        CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.SOMPOJAPAN &&
                                                                                                      f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                      f.Kod2 == kod2)
                                                                                                      .SingleOrDefault<CR_KullanimTarzi>();

                        if (kullanimTarzi != null)
                        {
                            var sompoTarifeKoduListe = clntTrafik.TariffGroupCodeExt();
                            string sompoTarifeKodu = "";
                            if (sompoTarifeKoduListe.Result != null)
                            {
                                sompoTarifeKodu = sompoTarifeKoduListe.Result.FirstOrDefault(s => s.ItemValue == kullanimTarzi.TarifeKodu).ItemValue;
                            }
                            clntTrafik.Dispose();
                            parameter = new sompojapan.trafik.CustomParameter();
                            parameter.code = SompoJapan_TrafikSoruTipleri.TrafikGrupKodu;
                            if (kullanimTarzi != null)
                                parameter.value = sompoTarifeKodu;
                            else parameter.value = "1";
                            parametersList.Add(parameter);
                        }
                    }
                }

                //Koltuk sayısıs
                AracTip aracTip = _AracContext.AracTipRepository.FindById(new object[] { teklif.Arac.Marka, teklif.Arac.AracinTipi });
                parameter = new sompojapan.trafik.CustomParameter();
                parameter.code = SompoJapan_TrafikSoruTipleri.Koltukadedi;
                if (aracTip != null)
                {
                    parameter.value = aracTip.KisiSayisi.ToString();
                }
                else parameter.value = "5";

                parametersList.Add(parameter);

                //Tescil veya asbis no gönderiliyor
                string TescilSeriNo = teklif.Arac.TescilSeriNo;
                string TescilSeriKod = teklif.Arac.TescilSeriKod;
                if (!String.IsNullOrEmpty(TescilSeriNo) && !String.IsNullOrEmpty(TescilSeriKod))
                {
                    parameter = new sompojapan.trafik.CustomParameter();
                    parameter.code = SompoJapan_TrafikSoruTipleri.TescilBelgeKod;
                    parameter.value = TescilSeriKod;
                    parametersList.Add(parameter);

                    parameter = new sompojapan.trafik.CustomParameter();
                    parameter.code = SompoJapan_TrafikSoruTipleri.AsbisTescilSeriNo;
                    parameter.value = TescilSeriNo;
                    parametersList.Add(parameter);
                }
                else if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                {
                    parameter = new sompojapan.trafik.CustomParameter();
                    parameter.code = SompoJapan_TrafikSoruTipleri.AsbisTescilSeriNo;
                    parameter.value = teklif.Arac.AsbisNo;
                    parametersList.Add(parameter);
                }


                trafik.Parameters = parametersList.ToArray();
                #endregion

                #region Service call

                this.BeginLog(trafik, trafik.GetType(), WebServisIstekTipleri.Teklif);

                sompojapan.trafik.ProposalOutput result = clntTrafik.PrimHesapla(trafik);
                clntTrafik.Dispose();
                if (result.RESULT.ERROR != null)
                {
                    this.EndLog(result, true, result.GetType());
                    this.AddHata(result.RESULT.ERROR.ERROR_DESCRIPTION);
                }
                else
                {
                    this.EndLog(result, true, result.GetType());
                }
                #endregion

                #endregion

                #region Eski ws

                //#region Veri Hazırlama GENEL

                //string MUSTERI_NO = String.Empty;

                //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANTrafik);

                //sompojapantrafik.Traffic clnt = new sompojapantrafik.Traffic();
                //clnt.Url = konfig[Konfig.SOMPOJAPAN_TrafikServiceURL];
                //clnt.Timeout = 150000;

                //MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                //MusteriAdre adress = _MusteriService.GetDefaultAdres(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                //Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);

                //#endregion

                //#region Arac Bilgileri

                //TrafficParameters trafik = new TrafficParameters();
                //trafik.TcKimlikNo = sigortali.KimlikNo;
                //trafik.PlakaIlKodu = teklif.Arac.PlakaKodu;

                //string rakam = "";
                //string harf = "";
                //foreach (char ch in teklif.Arac.PlakaNo)
                //{
                //    if (Char.IsDigit(ch))
                //        rakam += ch;
                //    if (Char.IsLetter(ch))
                //        harf += ch;
                //}

                //trafik.PlakaHarf = harf;
                //trafik.PlakaRakam = rakam;
                //trafik.Kullanici = servisKullanici.KullaniciAdi;
                //trafik.Sifre = servisKullanici.Sifre;
                //trafik.Partaj = Convert.ToInt32(servisKullanici.PartajNo_);
                //trafik.WinsureKullaniciAdi = servisKullanici.KullaniciAdi;

                //if (sigortali.KimlikNo.Length == 11)
                //    trafik.MusteriTipi = "O";
                //else trafik.MusteriTipi = "T";

                //if (String.IsNullOrEmpty(teklif.Arac.TescilSeriKod) && String.IsNullOrEmpty(teklif.Arac.TescilSeriNo) && !String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                //{
                //    trafik.EgmTescilBelgeSeriKod = "";
                //    trafik.EgmTescilBelgeSeriNo = teklif.Arac.AsbisNo;
                //}
                //else
                //{
                //    trafik.EgmTescilBelgeSeriKod = teklif.Arac.TescilSeriKod;
                //    trafik.EgmTescilBelgeSeriNo = teklif.Arac.TescilSeriNo;
                //}

                //bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                //List<sompojapantrafik.CustomParameter> parametersList = new List<sompojapantrafik.CustomParameter>();
                //sompojapantrafik.CustomParameter parameter = new sompojapantrafik.CustomParameter();
                //if (!eskiPoliceVar)
                //{
                //    string sompoJapanMarka = clnt.BrandsWithCode(servisKullanici.KullaniciAdi, servisKullanici.Sifre).Result.FirstOrDefault(k => k.ItemValue == teklif.Arac.Marka).ItemValue;
                //    string aracTipi = teklif.Arac.AracinTipi.Trim(); //babonline arac tipi

                //    if (aracTipi.Length < 3)
                //    {
                //        aracTipi = "0" + aracTipi;
                //    }

                //    string markaModel = sompoJapanMarka.Trim() + aracTipi;
                //    var sompoJapanMarkaModel = clnt.ModelsViaBrandCode(servisKullanici.KullaniciAdi, servisKullanici.Sifre, sompoJapanMarka).Result.FirstOrDefault(k => k.ItemValue.Trim() == markaModel).ItemValue;
                //    if (sompoJapanMarkaModel != null)
                //    {
                //        parameter.code = SompoJapan_TrafikSoruTipleri.MarkaModel;
                //        parameter.value = sompoJapanMarkaModel;
                //        parametersList.Add(parameter);
                //    }
                //    parameter = new sompojapantrafik.CustomParameter();
                //    parameter.code = SompoJapan_TrafikSoruTipleri.TrafigeCikisTarihi;
                //    parameter.value = teklif.Arac.TrafikCikisTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/");
                //    parametersList.Add(parameter);

                //    string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                //    if (parts.Length == 2)
                //    {
                //        string kullanimTarziKodu = parts[0];
                //        string kod2 = parts[1];
                //        CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.SOMPOJAPAN &&
                //                                                                                      f.KullanimTarziKodu == kullanimTarziKodu &&
                //                                                                                      f.Kod2 == kod2)
                //                                                                                      .SingleOrDefault<CR_KullanimTarzi>();

                //        if (kullanimTarzi != null)
                //        {
                //            string sompoTarifeKodu = clnt.TariffGroupCode(servisKullanici.KullaniciAdi, servisKullanici.Sifre).Result.FirstOrDefault(s => s.ItemValue == kullanimTarzi.TarifeKodu).ItemValue;
                //            parameter = new sompojapantrafik.CustomParameter();
                //            parameter.code = SompoJapan_TrafikSoruTipleri.TrafikGrupKodu;
                //            if (kullanimTarzi != null)
                //                parameter.value = sompoTarifeKodu;
                //            else parameter.value = "1";
                //            parametersList.Add(parameter);
                //        }
                //    }

                //}

                ////Koltuk sayısıs
                //AracTip aracTip = _AracContext.AracTipRepository.FindById(new object[] { teklif.Arac.Marka, teklif.Arac.AracinTipi });
                //parameter = new sompojapantrafik.CustomParameter();
                //parameter.code = SompoJapan_TrafikSoruTipleri.Koltukadedi;
                //if (aracTip != null)
                //{
                //    parameter.value = aracTip.KisiSayisi.ToString();
                //}
                //else parameter.value = "5";

                //parametersList.Add(parameter);
                //trafik.Parameters = parametersList.ToArray();
                //#endregion

                //#region Adres Bilgileri

                //sompojapantrafik.MethodResultOfListOfAddressItem il = clnt.Cities(servisKullanici.KullaniciAdi, servisKullanici.Sifre);
                //List<sompojapantrafik.Address> addressList = new List<sompojapantrafik.Address>();
                //sompojapantrafik.Address address = new sompojapantrafik.Address();

                //address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Il;
                //address.ADDRESS_DATA = il.Result.FirstOrDefault(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).ItemName;

                //addressList.Add(address);

                //sompojapantrafik.MethodResultOfListOfAddressItem ilceler = clnt.Towns(servisKullanici.KullaniciAdi, servisKullanici.Sifre, Convert.ToInt32(adress.IlKodu));
                //address = new sompojapantrafik.Address();
                //address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Ilce;
                //address.ADDRESS_DATA = ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName;
                //addressList.Add(address);

                //if (!String.IsNullOrEmpty(adress.Mahalle))
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                //    address.ADDRESS_DATA = adress.Mahalle;
                //    addressList.Add(address);

                //}
                //else if (!String.IsNullOrEmpty(adress.Semt))
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Semt;
                //    address.ADDRESS_DATA = adress.Semt;
                //    addressList.Add(address);
                //}
                //else
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                //    address.ADDRESS_DATA = "Mah.";
                //    addressList.Add(address);
                //}

                //if (!String.IsNullOrEmpty(adress.Cadde))
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                //    address.ADDRESS_DATA = adress.Cadde;
                //    addressList.Add(address);
                //}
                //else if (!String.IsNullOrEmpty(adress.Sokak))
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Sokak;
                //    address.ADDRESS_DATA = adress.Sokak;
                //    addressList.Add(address);
                //}
                //else
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                //    address.ADDRESS_DATA = "Cad.";
                //    addressList.Add(address);
                //}
                //if (!String.IsNullOrEmpty(adress.Apartman))
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Apartmani;
                //    address.ADDRESS_DATA = adress.Apartman;
                //    addressList.Add(address);
                //}

                //if (!String.IsNullOrEmpty(adress.BinaNo))
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.BinaNo;
                //    address.ADDRESS_DATA = adress.BinaNo;
                //    addressList.Add(address);
                //}

                //if (!String.IsNullOrEmpty(adress.DaireNo))
                //{
                //    address = new sompojapantrafik.Address();
                //    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.DaireNo;
                //    address.ADDRESS_DATA = adress.DaireNo;
                //    addressList.Add(address);
                //}

                //trafik.Addresses = addressList.ToArray();
                //#endregion

                //#region Service call

                //this.BeginLog(trafik, trafik.GetType(), WebServisIstekTipleri.Teklif);

                //sompojapantrafik.ProposalOutput result = clnt.PrimHesapla4(trafik);

                //if (result.RESULT.ERROR != null)
                //{
                //    this.EndLog(result, true, result.GetType());
                //    this.AddHata(result.RESULT.ERROR.ERROR_DESCRIPTION);
                //}
                //else
                //{
                //    this.EndLog(result, true, result.GetType());
                //}
                //#endregion
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
                    PolicyNumber = result.PROPOSAL_NO.Value,
                    ProductName = sompojapan.common.ProductName.ZorunluMaliMesuliyet,
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
                        fileName = String.Format("SOMPOJAPAN_Trafik_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                        url = storage.UploadFile("trafik", fileName, data);
                        this.GenelBilgiler.PDFDosyasi = url;
                        _Log.Info("Teklif_PDF url: {0}", url);
                    }
                    else
                    {
                        this.AddHata("PDF dosyası alınamadı.");
                    }
                }
                #endregion

                this.GenelBilgiler.TUMTeklifNo = result.PROPOSAL_NO.ToString();
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = result.PAYMENT.GROSS_PREMIUM;

                #region Sompo Japan Vergiler
                var Vergiler = result.PAYMENT.TAXES.ToList();

                decimal SOMPOJAPAN_THGF = 0;
                decimal SOMPOJAPAN_GiderVergi = 0;
                decimal SOMPOJAPAN_GuvenceHesabi = 0;
                decimal SOMPOJAPAN_Komisyonlar = 0;
                for (int i = 0; i < Vergiler.Count; i++)
                {
                    if (i == 0) SOMPOJAPAN_THGF = result.PAYMENT.TAXES[i].DEDUCTION_AMOUNT;
                    else if (i == 1) SOMPOJAPAN_GiderVergi = result.PAYMENT.TAXES[i].DEDUCTION_AMOUNT;
                    else if (i == 2) SOMPOJAPAN_GuvenceHesabi = result.PAYMENT.TAXES[i].DEDUCTION_AMOUNT;
                    else SOMPOJAPAN_Komisyonlar = result.PAYMENT.TAXES[i].DEDUCTION_AMOUNT;
                }


                #endregion

                this.GenelBilgiler.ToplamVergi = SOMPOJAPAN_THGF + SOMPOJAPAN_GiderVergi + SOMPOJAPAN_GuvenceHesabi;
                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.GecikmeZammiYuzdesi = 0;

                #region Hasarsılık indirim oranı set ediliyor

                if (result.QUESTIONS != null && result.QUESTIONS.Length > 0)
                {
                    var hasarsizlikKademe = result.QUESTIONS.FirstOrDefault(s => s.QUESTION_CODE == 225);
                    string HasarsizlikIndirim = hasarsizlikKademe.ANSWER.Trim();
                    if (!String.IsNullOrEmpty(HasarsizlikIndirim))
                    {
                        if (HasarsizlikIndirim == "1")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 60;
                        }
                        else if (HasarsizlikIndirim == "2")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 40;
                        }
                        else if (HasarsizlikIndirim == "3")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 20;
                        }
                        else if (HasarsizlikIndirim == "4")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        }
                        else if (HasarsizlikIndirim == "5")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 10;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        }
                        else if (HasarsizlikIndirim == "6")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 40;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        }
                        else if (HasarsizlikIndirim == "7")
                        {
                            this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 60;
                            this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                        }

                    }
                }

                #endregion

                this.GenelBilgiler.ZKYTMSYüzdesi = 0;
                this.GenelBilgiler.ToplamKomisyon = SOMPOJAPAN_Komisyonlar;

                //// ==== Güncellenicek. ==== //
                //this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                //this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                #endregion

                #region Vergiler
                this.AddVergi(TrafikVergiler.THGFonu, SOMPOJAPAN_THGF);
                this.AddVergi(TrafikVergiler.GiderVergisi, SOMPOJAPAN_GiderVergi);
                this.AddVergi(TrafikVergiler.GarantiFonu, SOMPOJAPAN_GuvenceHesabi);
                #endregion

                #region Teminatlar

                var TedaviKisiBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KisiBasinaTedaviMasraflari);
                decimal TeminatBedeli = 0;
                decimal BrutPrim = 0;
                if (TedaviKisiBasina != null)
                {
                    TeminatBedeli = TedaviKisiBasina.COVER_AMOUNT.HasValue ? TedaviKisiBasina.COVER_AMOUNT.Value : 0;
                    BrutPrim = TedaviKisiBasina.GROSS_PREMIUM.HasValue ? TedaviKisiBasina.GROSS_PREMIUM.Value : 0;
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                }

                var TedaviKazaBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KazaBasinaTedaviMasraflari);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (TedaviKazaBasina != null)
                {
                    TeminatBedeli = TedaviKazaBasina.COVER_AMOUNT.HasValue ? TedaviKazaBasina.COVER_AMOUNT.Value : 0;
                    BrutPrim = TedaviKazaBasina.GROSS_PREMIUM.HasValue ? TedaviKazaBasina.GROSS_PREMIUM.Value : 0;
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                }

                var OlumSakatlikKisiBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KisiBasiSakatOlum);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (OlumSakatlikKisiBasina != null)
                {
                    TeminatBedeli = OlumSakatlikKisiBasina.COVER_AMOUNT.HasValue ? OlumSakatlikKisiBasina.COVER_AMOUNT.Value : 0;
                    BrutPrim = OlumSakatlikKisiBasina.GROSS_PREMIUM.HasValue ? OlumSakatlikKisiBasina.GROSS_PREMIUM.Value : 0;
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                }

                var OlumSakatlikKazaBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.KazaBasiSakatOlum);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (OlumSakatlikKazaBasina != null)
                {
                    TeminatBedeli = OlumSakatlikKazaBasina.COVER_AMOUNT.HasValue ? OlumSakatlikKazaBasina.COVER_AMOUNT.Value : 0;
                    BrutPrim = OlumSakatlikKazaBasina.GROSS_PREMIUM.HasValue ? OlumSakatlikKazaBasina.GROSS_PREMIUM.Value : 0;
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                }

                var MaddiAracBasina = result.COVERS.FirstOrDefault(t => t.COVER_CODE == SOMPOJAPAN_TrafikTeminatlar.AracBasinaMaddi);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (MaddiAracBasina != null)
                {
                    TeminatBedeli = MaddiAracBasina.COVER_AMOUNT.HasValue ? MaddiAracBasina.COVER_AMOUNT.Value : 0;
                    BrutPrim = MaddiAracBasina.GROSS_PREMIUM.HasValue ? MaddiAracBasina.GROSS_PREMIUM.Value : 0;
                    this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, TeminatBedeli, 0, 0, BrutPrim, 0);
                }

                this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Tedavi_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kisi_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.IMM_Olum_Sakatlik_Kaza_Basina, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Asistans, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Tedavi, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Ferdi_Kaza_Koltuk_Ölüm_Sakatlık, 0, 0, 0, 0, 0);
                this.AddTeminat(TrafikTeminatlar.Trafik, 0, 0, 0, 0, 0);
                #endregion

                #region Ödeme Planı
                if (this.GenelBilgiler.TaksitSayisi == 1 || this.GenelBilgiler.TaksitSayisi == 0)
                {
                    this.AddOdemePlani(1, this.GenelBilgiler.BaslamaTarihi, this.GenelBilgiler.BrutPrim.Value, (teklif.GenelBilgiler.OdemeTipi ?? 0));
                }
                #endregion

                #region Web servis cevapları
                this.AddWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Teklif_Police_No, result.PROPOSAL_NO.ToString());
                this.AddWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Trafik_Session_No, result.RESPONSE_MESSAGE_NO.ToString());
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                #region Hata Log

                clntTrafik.Abort();

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
                #region Yeni WS

                #region Veri Hazırlama GENEL
                KonfigTable konfigCommon = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANCommonV2);
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.SOMPOJAPAN });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();
                // Cryptography sifrele = new Cryptography();
                Neosinerji.BABOnlineTP.Business.SOMPOJAPAN.Messages.Crypto crypto = new Neosinerji.BABOnlineTP.Business.SOMPOJAPAN.Messages.Crypto();
                sompojapan.common.Common client = new sompojapan.common.Common();
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
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
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

                if (response.RESULT != null)
                {
                    if (response.RESULT.ERROR != null)
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata(response.RESULT.ERROR.ERROR_DESCRIPTION);
                    }
                    else
                    {
                        this.EndLog(response, false, response.GetType());
                        this.AddHata("");
                    }
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
                            fileName = String.Format("SOMPOJAPAN_Trafik_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                            url = storage.UploadFile("trafik", fileName, data);
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
                            fileName = String.Format("SOMPOJAPAN_Trafik_Bilgilendirme_Formu_{0}.pdf", System.Guid.NewGuid().ToString());
                            url = storage.UploadFile("trafik", fileName, data);
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

                #region Eski ws

                //#region Veri Hazırlama GENEL
                //KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANTrafik);
                //ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.SOMPOJAPAN });
                //MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                //MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();
                //Cryptography sifrele = new Cryptography();

                //sompojapantrafik.Traffic clnt = new sompojapantrafik.Traffic();
                //clnt.Url = konfig[Konfig.SOMPOJAPAN_TrafikServiceURL];
                //clnt.Timeout = 150000;

                //KonfigTable konfigKasko = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKasko);
                //sompojapankasko.Casco cascoclnt = new sompojapankasko.Casco();
                //cascoclnt.Url = konfigKasko[Konfig.SOMPOJAPAN_CascoServiceURL];
                //cascoclnt.Timeout = 150000;

                //string TUMProposalNo = this.ReadWebServisCevap(Common.WebServisCevaplar.RAY_Teklif_Police_No, "0");

                //#endregion

                //#region Genel Bilgiler

                //List<sompojapantrafik.CustomParameter> parameters = new List<sompojapantrafik.CustomParameter>();
                //sompojapantrafik.CustomParameter parametre = new sompojapantrafik.CustomParameter();
                //parametre.code = "GSM";
                //parametre.value = telefon.Numara.Substring(3, 3) + telefon.Numara.Substring(7, 7);

                //parameters.Add(parametre);

                //this.BeginLog(parameters, parameters.GetType(), WebServisIstekTipleri.Police);

                ////Kredi kartı bilgileri Loglanmıyor. Şifrelenecek alanlar
                //string cardHolderName = String.Empty;
                //string cardNumber = String.Empty;
                //string month = String.Empty;
                //string year = String.Empty;
                //string cvv = String.Empty;
                //int taksitSayisi = odeme.TaksitSayisi;
                //if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                //{
                //    cardHolderName = sifrele.Encrypt(odeme.KrediKarti.KartSahibi);
                //    cardNumber = sifrele.Encrypt(odeme.KrediKarti.KartNo);
                //    month = sifrele.Encrypt(odeme.KrediKarti.SKA);
                //    year = sifrele.Encrypt(odeme.KrediKarti.SKY);
                //    cvv = sifrele.Encrypt(odeme.KrediKarti.CVC);
                //}
                //#endregion

                //#region Service Call

                //sompojapantrafik.ConfirmResponse response = clnt.ConfirmProposal(servisKullanici.KullaniciAdi, servisKullanici.Sifre, Convert.ToInt64(TUMProposalNo), cardHolderName, cardNumber, month, year, cvv, taksitSayisi, parameters.ToArray());

                //#endregion

                //#region Hata Kontrol ve Kayıt

                //if (!response.success)
                //{
                //    this.EndLog(response, false, response.GetType());
                //    this.AddHata(response.description);
                //}
                //else
                //{
                //    string sompoPoliceNo = response.policynumber.ToString();
                //    long sompoMessageNo = Convert.ToInt64(ReadSoru(Common.WebServisCevaplar.SOMPOJAPAN_Trafik_Session_No, "0"));
                //    this.GenelBilgiler.TUMPoliceNo = sompoPoliceNo;
                //    this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                //    casco.PrintResponse printResponse = cascoclnt.GetDocument(servisKullanici.KullaniciAdi, servisKullanici.Sifre, sompoMessageNo, response.policynumber);

                //    if (!printResponse.success)
                //    {
                //        this.AddHata(response.description);
                //    }
                //    else
                //    {
                //        var policePDF = printResponse.downloadurl;
                //        if (policePDF != null)
                //        {
                //            WebClient myClient = new WebClient();
                //            byte[] data = myClient.DownloadData(policePDF);

                //            IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                //            string fileName = String.Empty;
                //            string url = String.Empty;

                //            fileName = String.Format("SOMPOJAPAN_Trafik_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                //            url = storage.UploadFile("trafik", fileName, data);
                //            this.GenelBilgiler.PDFPolice = url;

                //            _Log.Info("Police_PDF url: {0}", url);
                //        }
                //        else
                //        {
                //            this.AddHata("PDF dosyası alınamadı.");
                //            return;
                //        }
                //    }
                //}
                //#endregion
                #endregion

                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
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
                client.Dispose();
                #endregion

                #endregion

                #region Eski ws

                //KonfigTable konfigKasko = _KonfigurasyonService.GetKonfig(Konfig.BundleSOMPOJAPANKasko);

                //sompojapankasko.Casco cascoClnt = new sompojapankasko.Casco();
                //cascoClnt.Url = konfigKasko[Konfig.SOMPOJAPAN_CascoServiceURL];
                //cascoClnt.Timeout = 150000;

                //ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                //TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { teklif.GenelBilgiler.TVMKodu, TeklifUretimMerkezleri.SOMPOJAPAN });

                //#region Servis call
                //string sompoPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Teklif_Police_No, "0");
                //string sompoSessionNo = this.ReadWebServisCevap(Common.WebServisCevaplar.SOMPOJAPAN_Kasko_Session_No, "0");

                //request.kullaniciAdi = servisKullanici.KullaniciAdi;
                //request.sifre = servisKullanici.Sifre;
                //request.sessionNo = Convert.ToInt64(sompoSessionNo);
                //request.policeNo = Convert.ToInt64(sompoPoliceNo);

                //this.BeginLog(request, typeof(SompoDekontRequest), WebServisIstekTipleri.DekontPDF);

                //PrintResponse response = cascoClnt.GetPaymentSlip(request.kullaniciAdi,
                //                                             request.sifre,
                //                                             request.sessionNo,
                //                                             request.policeNo);

                //this.EndLog(response, true, typeof(PrintResponse));
                //_TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                //#endregion
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

                        string fileName = String.Format("sompo_dekont_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string url = storage.UploadFile("kasko", fileName, data);
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
                _Log.Error("SompoJapanTrafik.DekontPDF", ex);

                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        private sompojapan.trafik.Unit SigortaliBilgileri(ITeklif teklif, sompojapan.trafik.Traffic clnt)
        {
            MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriAdre adress = _MusteriService.GetDefaultAdres(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
            MusteriTelefon cepTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon evTel = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
            sompojapan.trafik.Unit unit = new sompojapan.trafik.Unit();
            sompojapan.trafik.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
            sompojapan.trafik.MethodResultOfListOfAddressItem ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
            try
            {
                List<sompojapan.trafik.Address> addressList = new List<sompojapan.trafik.Address>();

                sompojapan.trafik.Address address = new sompojapan.trafik.Address();
                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Il;
                address.ADDRESS_DATA = il != null ? il.Result.FirstOrDefault(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).ItemName : "";
                addressList.Add(address);


                address = new sompojapan.trafik.Address();
                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Ilce;

                if (ilceler != null)
                {
                    var sompoIlce = ilceler.Result.Where(S => S.ItemName == ilce.IlceAdi).FirstOrDefault();
                    if (sompoIlce != null)
                    {
                        address.ADDRESS_DATA = sompoIlce.ItemName;
                    }
                    else
                    {
                        throw new Exception("Ekrandan girilen ilçe Sompo Japan Sigorta da bulunamadı !");
                    }
                }


                addressList.Add(address);

                if (!String.IsNullOrEmpty(adress.Mahalle))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = adress.Mahalle;
                    addressList.Add(address);

                }
                else if (!String.IsNullOrEmpty(adress.Semt))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Semt;
                    address.ADDRESS_DATA = adress.Semt;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = "Mah.";
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.Cadde))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = adress.Cadde;
                    addressList.Add(address);
                }
                else if (!String.IsNullOrEmpty(adress.Sokak))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Sokak;
                    address.ADDRESS_DATA = adress.Sokak;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = "Cad.";
                    addressList.Add(address);
                }
                if (!String.IsNullOrEmpty(adress.Apartman))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Apartmani;
                    address.ADDRESS_DATA = adress.Apartman;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.BinaNo))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.BinaNo;
                    address.ADDRESS_DATA = adress.BinaNo;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.DaireNo))
                {
                    address = new sompojapan.trafik.Address();
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

                if (evTel != null)
                {
                    unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                    unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                    unit.PHONE_NUMBER = Convert.ToInt32(evTel.Numara.Substring(7, 7));
                }
                if (cepTel != null)
                {
                    unit.GSM_COUNTRY_CODE = cepTel.Numara.Substring(0, 2);
                    unit.GSM_CODE = cepTel.Numara.Substring(3, 3);
                    unit.GSM_NUMBER = cepTel.Numara.Substring(7, 7);
                }
                unit.EMAIL_ADDRESS = sigortali.EMail;
                unit.GENDER = sigortali.Cinsiyet;
                return unit;
            }
            catch (Exception ex)
            {
                if (il.Description != "Success")
                {
                    throw new Exception(!string.IsNullOrEmpty(il.Description) ? il.Description : ex.Message);
                }
                else if (ilceler.Description != "Success")
                {
                    throw new Exception(!string.IsNullOrEmpty(ilceler.Description) ? ilceler.Description : ex.Message);
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }


        }

        private sompojapan.trafik.Unit SigortaEttirenBilgileri(ITeklif teklif, sompojapan.trafik.Traffic clnt)
        {
            TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;

            MusteriGenelBilgiler SEGenelBilgiler = _MusteriService.GetMusteri(sigortaEttiren.MusteriKodu);
            MusteriAdre adress = _MusteriService.GetDefaultAdres(sigortaEttiren.MusteriKodu);
            MusteriTelefon cepTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
            MusteriTelefon evTel = SEGenelBilgiler.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Ev);
            Ilce ilce = _UlkeService.GetIlce(adress.IlceKodu.Value);
            sompojapan.trafik.Unit unit = new sompojapan.trafik.Unit();

            sompojapan.trafik.MethodResultOfListOfAddressItem il = clnt.CitiesExt();
            sompojapan.trafik.MethodResultOfListOfAddressItem ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
            clnt.Dispose();
            try
            {
                List<sompojapan.trafik.Address> addressList = new List<sompojapan.trafik.Address>();
                sompojapan.trafik.Address address = new sompojapan.trafik.Address();

                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Il;
                address.ADDRESS_DATA = il != null ? il.Result.FirstOrDefault(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).ItemName : "";

                addressList.Add(address);

                address = new sompojapan.trafik.Address();
                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Ilce;
                address.ADDRESS_DATA = ilceler != null ? ilceler.Result.FirstOrDefault(S => S.ItemName == ilce.IlceAdi).ItemName : "";
                addressList.Add(address);

                if (!String.IsNullOrEmpty(adress.Mahalle))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = adress.Mahalle;
                    addressList.Add(address);

                }
                else if (!String.IsNullOrEmpty(adress.Semt))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Semt;
                    address.ADDRESS_DATA = adress.Semt;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Mahallesi;
                    address.ADDRESS_DATA = "Mah.";
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.Cadde))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = adress.Cadde;
                    addressList.Add(address);
                }
                else if (!String.IsNullOrEmpty(adress.Sokak))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Sokak;
                    address.ADDRESS_DATA = adress.Sokak;
                    addressList.Add(address);
                }
                else
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Cadde;
                    address.ADDRESS_DATA = "Cad.";
                    addressList.Add(address);
                }
                if (!String.IsNullOrEmpty(adress.Apartman))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Apartmani;
                    address.ADDRESS_DATA = adress.Apartman;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.BinaNo))
                {
                    address = new sompojapan.trafik.Address();
                    address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.BinaNo;
                    address.ADDRESS_DATA = adress.BinaNo;
                    addressList.Add(address);
                }

                if (!String.IsNullOrEmpty(adress.DaireNo))
                {
                    address = new sompojapan.trafik.Address();
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
                    unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                    unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                    unit.PHONE_NUMBER = Convert.ToInt32(evTel.Numara.Substring(7, 7));
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
            catch (Exception ex)
            {
                if (il.Description != "Success")
                {
                    throw new Exception(!string.IsNullOrEmpty(il.Description) ? il.Description : ex.Message);
                }
                else if (ilceler.Description != "Success")
                {
                    throw new Exception(!string.IsNullOrEmpty(ilceler.Description) ? ilceler.Description : ex.Message);
                }
                else
                {
                    throw new Exception(ex.Message);
                }
            }
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
            try
            {

                List<sompojapan.common.Address> addressList = new List<sompojapan.common.Address>();
                sompojapan.common.Address address = new sompojapan.common.Address();

                address.ADDRESS_TYPE = SOMPOJAPAN_AdresKodlari.Il;
                address.ADDRESS_DATA = il.Result.FirstOrDefault(s => s.ItemCode == Convert.ToInt32(adress.IlKodu)).ItemName;

                addressList.Add(address);

                sompojapan.trafik.MethodResultOfListOfAddressItem ilceler = clnt.TownsExt(Convert.ToInt32(adress.IlKodu));
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
                unit.OCCUPATION = SompoJapan_KaskoSoruCevapTipleri.Diger;
                unit.NATIONALITY = 1;

                if (evTel != null)
                {
                    unit.PHONE_COUNTRY_CODE = Convert.ToInt16(evTel.Numara.Substring(0, 2));
                    unit.PHONE_CODE = Convert.ToInt16(evTel.Numara.Substring(3, 3));
                    unit.PHONE_NUMBER = Convert.ToInt32(evTel.Numara.Substring(7, 7));
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
            catch (Exception ex)
            {
                clnt.Abort();
                throw new Exception(!string.IsNullOrEmpty(il.Description) ? il.Description : ex.Message);
            }
        }

        private sompojapan.common.Policy GetPolicy(ITeklif teklif)
        {
            sompojapan.common.Policy policy = new sompojapan.common.Policy()
            {
                ProductName = sompojapan.common.ProductName.ZorunluMaliMesuliyet,
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
                ProductName = sompojapan.common.ProductName.ZorunluMaliMesuliyet,
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
            return gonderilenIp;

        }
    }
}
