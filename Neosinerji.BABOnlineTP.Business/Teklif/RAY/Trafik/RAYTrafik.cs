using System;
using System.Collections.Generic;
using System.Globalization;
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
using Neosinerji.BABOnlineTP.Business.Common.RAY;
using System.Xml.Serialization;
using Neosinerji.BABOnlineTP.Business.RAY.Messages;
using Neosinerji.BABOnlineTP.Business.RAY;
using System.Xml;
using System.Security.Cryptography;
using INET.Crypto;
using Neosinerji.BABOnlineTP.Business.RAY.Message;
using Neosinerji.BABOnlineTP.Business.RAY.Trafik;

namespace Neosinerji.BABOnlineTP.Business.RAY
{

    public class RAYTrafik : Teklif, IRAYTrafik
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
        [InjectionConstructor]
        public RAYTrafik(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService, ITVMContext tvmContext, ILogService log, ITeklifService teklifService, IParametreContext context, ITVMService TVMService )
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
        }


        public override int TUMKodu
        {
            get
            {
                return TeklifUretimMerkezleri.RAY;
            }
        }

        public override void Hesapla(ITeklif teklif)
        {
            ray.InsurerServices clnt = new ray.InsurerServices();
            try
            {
                #region Veri Hazırlama GENEL
                string MUSTERI_NO = String.Empty;

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);
                clnt.Url = konfig[Konfig.RAY_ServiceURL];
                clnt.Timeout = 150000;
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.RAY });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);

                RAY_TrafikTeklifKayit_Request trafikreq = new RAY_TrafikTeklifKayit_Request();
                RAY_TrafikTeklifKayit_Response trafikres = new RAY_TrafikTeklifKayit_Response();

                trafikreq.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                trafikreq.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);               
                trafikreq.CLIENT_IP = this.IpGetir(teklif.GenelBilgiler.TVMKodu);
                trafikreq.PROCESS_ID = RAY_ProcesTipleri.TeklifKayit;
                trafikreq.FIRM_CODE = "2";
                trafikreq.COMPANY_CODE = "2";
                trafikreq.PRODUCT_NO = RAY_UrunKodlari.Trafik;
                trafikreq.POLICY_NO = "0";
                trafikreq.RENEWAL_NO = "0";
                trafikreq.ENDORS_NO = "0";
                trafikreq.ENDORS_TYPE_CODE = "";
                trafikreq.PROD_CANCEL = "T";
                trafikreq.CANCEL_REASON_CODE = "0";
                trafikreq.CHANNEL = servisKullanici.PartajNo_;
                trafikreq.ISSUE_CHANNEL = servisKullanici.PartajNo_;
                // -- Müşteri RAY Sigorta Database inden Sorgulanıyor ve Yoksa RAY Sigorta sistemine müşteri kayıt ediliyor...
                MUSTERI_NO = MusteriNo(teklif, sigortali, teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value, konfig[Konfig.RAY_ServiceURL]);
                trafikreq.CLIENT_NO = MUSTERI_NO;
                trafikreq.INSURED_NO = MUSTERI_NO;                

                DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);

                trafikreq.BEG_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                trafikreq.END_DATE = polBaslangic.AddYears(1).ToString("dd/MM/yyyy").Replace(".", "/");
                trafikreq.ISSUE_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                //trafikreq.ISSUE_DATE = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/");
                trafikreq.CONFIRM_DATE = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/");  //polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                trafikreq.TRANSFER_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                trafikreq.PROPOSAL_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                trafikreq.CUR_TYPE = "YTL";
                trafikreq.EXCHANGE_RATE = "1";
                trafikreq.PAYMENT_TYPE = "0";
                trafikreq.PLATE = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                trafikreq.CATEGORY1 = "";
                trafikreq.CATEGORY2 = "";
                trafikreq.DESCRIPTION = "SigortaProTeklifi";
                trafikreq.QUERY_METHOD = "T";

                #region Trafik Teklifi Sorular
                List<QUESTION> sorular = new List<QUESTION>();
                QUESTION question = new QUESTION();

                string aracMarkaKodu = teklif.Arac.Marka + teklif.Arac.AracinTipi;
                if (teklif.Arac.Model > 2002)
                {
                    question = SoruEkle(RAY_TrafikSoruTipleri.AracMarkaKodu, aracMarkaKodu);
                }
                else
                {
                    question = SoruEkle(RAY_KaskoSorular.AracMarkaKodu, "999999");
                }
                sorular.Add(question);
               

                string aracKullanimSekli = String.Empty; ;
                switch (teklif.Arac.KullanimSekli)
                {
                    case "0": aracKullanimSekli = RAY_AracKullanimSeklilleri.Ticari; break;
                    case "1": aracKullanimSekli = RAY_AracKullanimSeklilleri.Resmi; break;
                    case "2": aracKullanimSekli = RAY_AracKullanimSeklilleri.Ozel; break;
                }

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.KullanimSekli, aracKullanimSekli);
                sorular.Add(question);

                string RAYKullanimTarziKodu = String.Empty;
                string[] parts = teklif.Arac.KullanimTarzi.Split('-');
                if (parts.Length == 2)
                {
                    string kullanimTarziKodu = parts[0];
                    string kod2 = parts[1];
                    CR_KullanimTarzi kullanimTarzi = _CRContext.CR_KullanimTarziRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.RAY &&
                                                                                                    f.KullanimTarziKodu == kullanimTarziKodu &&
                                                                                                    f.Kod2 == kod2)
                                                                                                    .SingleOrDefault<CR_KullanimTarzi>();

                    if (kullanimTarzi != null)
                        RAYKullanimTarziKodu = kullanimTarzi.TarifeKodu;
                }

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.KullanimTarzi, RAYKullanimTarziKodu);
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.Model, teklif.Arac.Model.HasValue ? teklif.Arac.Model.Value.ToString() : String.Empty);
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.MotorNo, teklif.Arac.MotorNo);
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.SasiNo, teklif.Arac.SasiNo);
                sorular.Add(question);

                if (!String.IsNullOrEmpty(teklif.Arac.TescilSeriNo) && !String.IsNullOrEmpty(teklif.Arac.TescilSeriKod))
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.TescilBelgeNo, teklif.Arac.TescilSeriNo);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.TescilBelgeSeriKod, teklif.Arac.TescilSeriKod);
                    sorular.Add(question);
                }
                else if (!String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.TescilBelgeSeriKod, teklif.Arac.AsbisNo);
                    sorular.Add(question);
                }

                //question = new QUESTION();
                //if (teklif.Arac.PlakaNo != "YK" || teklif.Arac.PlakaNo != "G 9999" || eskiPoliceVar) question = SoruEkle(RAY_TrafikSoruTipleri.YeniIsletenMi, "H");
                //else question = SoruEkle(RAY_TrafikSoruTipleri.YeniIsletenMi, "E");
                //sorular.Add(question);

                #region Önceki Poliçe
                bool eskiPoliceVar = teklif.ReadSoru(TrafikSorular.Eski_Police_VarYok, false);
                string oncekiAcenteNo = String.Empty;
                string oncekiPoliceNo = String.Empty;
                string oncekiSirketKodu = String.Empty;
                string oncekiYenilemeNo = String.Empty;
                string OncekiPoliceMotorNo = String.Empty;
                string OncekiPoliceSasiNo = String.Empty;

                if (eskiPoliceVar)
                {
                    oncekiAcenteNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Acente_No, String.Empty);
                    oncekiPoliceNo = teklif.ReadSoru(TrafikSorular.Eski_Police_No, String.Empty);
                    oncekiSirketKodu = teklif.ReadSoru(TrafikSorular.Eski_Police_Sigorta_Sirketi, String.Empty);
                    oncekiYenilemeNo = teklif.ReadSoru(TrafikSorular.Eski_Police_Yenileme_No, String.Empty);
                    OncekiPoliceMotorNo = teklif.Arac.MotorNo;
                    OncekiPoliceSasiNo = teklif.Arac.SasiNo;

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiAcenteNo, oncekiAcenteNo);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiPoliceNo, oncekiPoliceNo);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiSirketKodu, oncekiSirketKodu);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.OncekiYenilemeNo, oncekiYenilemeNo);
                    sorular.Add(question);

                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.YeniIsletenMi, "H");
                    sorular.Add(question);
                }
                else
                {
                    question = new QUESTION();
                    question = SoruEkle(RAY_TrafikSoruTipleri.YeniIsletenMi, "E");
                    sorular.Add(question);
                }
                #endregion

                question = new QUESTION();
                if (String.IsNullOrEmpty(teklif.Arac.AsbisNo))
                    question = SoruEkle(RAY_TrafikSoruTipleri.ASBISMI, "H");
                else
                    question = SoruEkle(RAY_TrafikSoruTipleri.ASBISMI, "E");
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.AracGrubuIsMakinesiMi, "H"); //?
                sorular.Add(question);

                question = new QUESTION();
                var marka = _AracContext.AracMarkaRepository.Filter(f => f.MarkaKodu == teklif.Arac.Marka).FirstOrDefault();
                if (marka != null)
                    question = SoruEkle(RAY_TrafikSoruTipleri.AracMarka, marka.MarkaAdi);
                else
                    question = SoruEkle(RAY_TrafikSoruTipleri.AracMarka, "");
                sorular.Add(question);

                question = new QUESTION();
                var tip = _AracContext.AracTipRepository.Filter(f => f.MarkaKodu == teklif.Arac.Marka && f.TipKodu == teklif.Arac.AracinTipi).FirstOrDefault();
                if (tip != null)
                    question = SoruEkle(RAY_TrafikSoruTipleri.Tip, tip.TipAdi);
                else
                    question = SoruEkle(RAY_TrafikSoruTipleri.Tip, "");
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.YerliYabanci, "1"); //?
                sorular.Add(question);

                question = new QUESTION();
                if (teklif.Arac.TrafikTescilTarihi.HasValue)
                    question = SoruEkle(RAY_TrafikSoruTipleri.TrafikTescilTarihi, teklif.Arac.TrafikTescilTarihi.Value.ToString("dd/MM/yyyy").Replace(".", "/"));
                else
                    question = SoruEkle(RAY_TrafikSoruTipleri.TrafikTescilTarihi, "");

                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.OncekiPoliceMotorNo, OncekiPoliceMotorNo);
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_TrafikSoruTipleri.OncekiPoliceSasiNo, OncekiPoliceSasiNo);
                sorular.Add(question);

                question = new QUESTION();
                question = SoruEkle(RAY_KaskoSorular.UrunSecimi, "1");
                sorular.Add(question);

                trafikreq.QUESTION = sorular.ToArray();

                #endregion

                this.BeginLog(trafikreq, trafikreq.GetType(), WebServisIstekTipleri.Teklif);

                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_TrafikTeklifKayit_Request));
                StringWriter Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(Output, trafikreq);
                string responseTrafik = clnt.ProductionIntegrator(Output.ToString());
                clnt.Dispose();
                Output.Close();

                _Serialize = new XmlSerializer(typeof(RAY_TrafikTeklifKayit_Response));
                string url ="";
                using (TextReader reader = new StringReader(responseTrafik))
                {
                    var trafikHata = responseTrafik.Contains("Error");
                    if (!trafikHata)
                    {
                        trafikres = (RAY_TrafikTeklifKayit_Response)_Serialize.Deserialize(reader);
                        reader.ReadToEnd();
                        this.EndLog(trafikres, true, trafikres.GetType());

                        RAY_PoliceBasimi_Request pdfRequest = new RAY_PoliceBasimi_Request();
                        RAY_PoliceBasimi_Response pdfResponse = new RAY_PoliceBasimi_Response();

                        pdfRequest.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                        pdfRequest.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                        pdfRequest.PROCESS_ID = RAY_ProcesTipleri.PolicePDF;
                        pdfRequest.FIRM_CODE = "2";
                        pdfRequest.COMPANY_CODE = "2";
                        pdfRequest.PRODUCT_NO = RAY_UrunKodlari.Trafik;
                        pdfRequest.POLICY_NO = trafikres.POLMAS.POLICY_NO;
                        pdfRequest.RENEWAL_NO = "0";
                        pdfRequest.ENDORS_NO = "0";
                        pdfRequest.PRINT_TYPE = RAY_PDFBasimTipleri.PolicePDF; //Police PDF

                        this.BeginLog(pdfRequest, pdfRequest.GetType(), WebServisIstekTipleri.Police);

                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Request));
                        Output = new StringWriter(new StringBuilder());

                        _Serialize.Serialize(Output, pdfRequest);
                        string responseTeklifPDF = clnt.ProductionIntegrator(Output.ToString());
                        clnt.Dispose();
                        Output.Close();

                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Response));
                        using (TextReader readerPDF = new StringReader(responseTeklifPDF))
                        {

                            pdfResponse = (RAY_PoliceBasimi_Response)_Serialize.Deserialize(readerPDF);
                            readerPDF.ReadToEnd();
                            this.EndLog(pdfResponse, true, pdfResponse.GetType());

                            if (pdfResponse.STATUS_CODE != "0")
                            {
                                WebClient myClient = new WebClient();
                                byte[] data = myClient.DownloadData(pdfResponse.LINK);

                                ITeklifPDFStorage storage = DependencyResolver.Current.GetService<ITeklifPDFStorage>();
                                string fileName = String.Format("RAY_Trafik_Teklif_{0}.pdf", System.Guid.NewGuid().ToString());
                               url = storage.UploadFile("trafik", fileName, data);
                                
                                _Log.Info("Teklif_PDF url: {0}", url);
                            }
                            else
                            {
                                this.EndLog(pdfResponse, false, pdfResponse.GetType());
                                this.AddHata(pdfResponse.STATUS_DESC);
                            }
                        }

                    }
                    else
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_TrafikTeklifKayitHata_Response));
                        using (StringReader readerTrafikHata = new StringReader(responseTrafik))
                        {
                            RAY_TrafikTeklifKayitHata_Response trafikResponseHata = new RAY_TrafikTeklifKayitHata_Response();
                            trafikResponseHata = (RAY_TrafikTeklifKayitHata_Response)_Serialize.Deserialize(readerTrafikHata);
                            readerTrafikHata.ReadToEnd();

                            this.EndLog(trafikResponseHata, false, trafikResponseHata.GetType());
                            this.AddHata(trafikResponseHata.Error.ErrDesc);
                        }
                    }
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

                this.GenelBilgiler.TUMTeklifNo = trafikres.POLMAS.POLICY_NO;
                this.GenelBilgiler.PDFDosyasi = url;
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = this.GenelBilgiler.BaslamaTarihi;
                this.GenelBilgiler.BitisTarihi = this.GenelBilgiler.BaslamaTarihi.AddYears(1);
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = RAYMessages.ToDecimal(trafikres.POLMAS.GROSS_PREMIUM);

                var PoldidList = trafikres.POLMAS.POLDID.ToList<POLDID>();

                decimal RAY_THGF = 0;
                decimal RAY_GiderVergi = 0;
                decimal RAY_GuvenceHesabi = 0;
                decimal RAY_KaynakKomisyonu = 0;

                for (int i = 0; i < PoldidList.Count; i++)
                {
                    var DeductionCode = trafikres.POLMAS.POLDID[i].DEDUCTION_CODE;
                    var DeductionTypeCode = trafikres.POLMAS.POLDID[i].DEDUCTION_TYPE_CODE;

                    if (DeductionTypeCode == RAY_TrafikKesitiTipKodlari.Vergiler && DeductionCode == RAY_TrafikVergiKesintiKodlari.THGF)
                        RAY_THGF = !String.IsNullOrEmpty(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) ? RAYMessages.ToDecimal(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) : 0;

                    if (DeductionTypeCode == RAY_TrafikKesitiTipKodlari.Vergiler && DeductionCode == RAY_TrafikVergiKesintiKodlari.GiderVergisi)
                        RAY_GiderVergi = !String.IsNullOrEmpty(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) ? RAYMessages.ToDecimal(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) : 0;

                    if (DeductionTypeCode == RAY_TrafikKesitiTipKodlari.Vergiler && DeductionCode == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi)
                        RAY_GuvenceHesabi = !String.IsNullOrEmpty(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) ? RAYMessages.ToDecimal(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) : 0;

                    if (DeductionTypeCode == RAY_TrafikKesitiTipKodlari.Komisyonlar && DeductionCode == RAY_TrafikKomisyonKesintiKodlari.KaynakKomisyonu)
                        RAY_KaynakKomisyonu = !String.IsNullOrEmpty(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) ? RAYMessages.ToDecimal(trafikres.POLMAS.POLDID[i].DEDUCTION_AMOUNT) : 0;
                }

                string RAY_GecikmeOrani = String.Empty;
                decimal RAY_HasarsizlikOrani = 0;
                string RAY_ZKYTMSIndirimi = String.Empty;

                var gecikmeOrani = trafikres.POLMAS.PSRCVP.FirstOrDefault(f => f.QUESTION_CODE == RAY_TrafikResponseSoruKodlari.GecikmeOrani);
                if (gecikmeOrani != null)
                    RAY_GecikmeOrani = gecikmeOrani.ANSWER;

                var hasarsizlikOrani = trafikres.POLMAS.PSRCVP.FirstOrDefault(f => f.QUESTION_CODE == RAY_TrafikResponseSoruKodlari.HasarsizlikOrani);
                if (hasarsizlikOrani != null)
                    RAY_HasarsizlikOrani = !String.IsNullOrEmpty(hasarsizlikOrani.ANSWER) ? RAYMessages.ToDecimal(hasarsizlikOrani.ANSWER) : 0;

                var ZKYTMSIndirimi = trafikres.POLMAS.PSRCVP.FirstOrDefault(f => f.QUESTION_CODE == RAY_TrafikResponseSoruKodlari.ZKYTMSIndirimi);
                if (ZKYTMSIndirimi != null)
                    RAY_ZKYTMSIndirimi = ZKYTMSIndirimi.ANSWER;

                this.GenelBilgiler.ToplamVergi = RAY_THGF + RAY_GiderVergi + RAY_GuvenceHesabi;
                this.GenelBilgiler.NetPrim = this.GenelBilgiler.BrutPrim - this.GenelBilgiler.ToplamVergi;
                this.GenelBilgiler.TaksitSayisi = 1;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.TL;
                this.GenelBilgiler.GecikmeZammiYuzdesi = RAYMessages.ToDecimal(RAY_GecikmeOrani);
                this.GenelBilgiler.ZKYTMSYüzdesi = RAYMessages.ToDecimal(RAY_ZKYTMSIndirimi);
                this.GenelBilgiler.ToplamKomisyon = RAY_KaynakKomisyonu;

                if (RAY_HasarsizlikOrani > 0)
                {
                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = 0;
                    this.GenelBilgiler.HasarSurprimYuzdesi = RAY_HasarsizlikOrani;
                }
                else if (RAY_HasarsizlikOrani < 0)
                {
                    this.GenelBilgiler.HasarsizlikIndirimYuzdesi = Math.Abs(RAY_HasarsizlikOrani);
                    this.GenelBilgiler.HasarSurprimYuzdesi = 0;
                }

                // ==== Güncellenicek. ==== //
                //this.GenelBilgiler.OdemeSekli = OdemeSekilleri.Pesin;
                //this.GenelBilgiler.OdemeTipi = OdemeTipleri.KrediKarti;

                //Odeme Bilgileri
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                #endregion

                #region Vergiler
                this.AddVergi(TrafikVergiler.THGFonu, RAY_THGF);
                this.AddVergi(TrafikVergiler.GiderVergisi, RAY_GiderVergi);
                this.AddVergi(TrafikVergiler.GarantiFonu, 0);
                #endregion

                #region Teminatlar

                var ZorunluMS = trafikres.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.ZorunluMaliSorumluluk);
                decimal TeminatBedeli = 0;
                decimal BrutPrim = 0;
                if (ZorunluMS != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(ZorunluMS.COVER_AMOUNT);

                    #region Teminat Vergileri
                    decimal ToplamVergi = 0;
                    var GiderVergi = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.ZorunluMaliSorumluluk &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    var GuvenceHesab = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.ZorunluMaliSorumluluk &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi);

                    var THGF = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.ZorunluMaliSorumluluk &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.THGF);
                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);
                    if (GuvenceHesab != null)
                        ToplamVergi += RAYMessages.ToDecimal(GuvenceHesab.DEDUCTION_AMOUNT);
                    if (THGF != null)
                        ToplamVergi += RAYMessages.ToDecimal(THGF.DEDUCTION_AMOUNT);
                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(TrafikTeminatlar.IMM_Maddi_Kisi_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(ZorunluMS.NET_PREMIUM), BrutPrim, 0);
                }

                var MaddiAracBasina = trafikres.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiAracBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (MaddiAracBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(MaddiAracBasina.COVER_AMOUNT);

                    #region Teminat Vergileri
                    decimal ToplamVergi = 0;

                    var GiderVergi = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiAracBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    var GuvenceHesab = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiAracBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi);

                    var THGF = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiAracBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.THGF);
                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);
                    if (GuvenceHesab != null)
                        ToplamVergi += RAYMessages.ToDecimal(GuvenceHesab.DEDUCTION_AMOUNT);
                    if (THGF != null)
                        ToplamVergi += RAYMessages.ToDecimal(THGF.DEDUCTION_AMOUNT);
                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(TrafikTeminatlar.Maddi_Arac_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(MaddiAracBasina.NET_PREMIUM), BrutPrim, 0);
                }


                var MaddiKazaBasina = trafikres.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiKazaBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (MaddiKazaBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(MaddiKazaBasina.COVER_AMOUNT);

                    #region Teminat Vergileri
                    decimal ToplamVergi = 0;

                    var GiderVergi = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiKazaBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    var GuvenceHesab = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiKazaBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi);

                    var THGF = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.MaddiKazaBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.THGF);
                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);
                    if (GuvenceHesab != null)
                        ToplamVergi += RAYMessages.ToDecimal(GuvenceHesab.DEDUCTION_AMOUNT);
                    if (THGF != null)
                        ToplamVergi += RAYMessages.ToDecimal(THGF.DEDUCTION_AMOUNT);
                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(TrafikTeminatlar.Maddi_Kaza_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(MaddiKazaBasina.NET_PREMIUM), BrutPrim, 0);
                }
                var OlumSakatlikKisiBasina = trafikres.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKisiBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (OlumSakatlikKisiBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(OlumSakatlikKisiBasina.COVER_AMOUNT);

                    #region Teminat Vergileri
                    decimal ToplamVergi = 0;

                    var GiderVergi = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKisiBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    var GuvenceHesab = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKisiBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi);

                    var THGF = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKisiBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.THGF);
                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);
                    if (GuvenceHesab != null)
                        ToplamVergi += RAYMessages.ToDecimal(GuvenceHesab.DEDUCTION_AMOUNT);
                    if (THGF != null)
                        ToplamVergi += RAYMessages.ToDecimal(THGF.DEDUCTION_AMOUNT);
                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kisi_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(OlumSakatlikKisiBasina.NET_PREMIUM), TeminatBedeli, 0);
                }
                var OlumSakatlikKazaBasina = trafikres.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKazaBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (OlumSakatlikKazaBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(OlumSakatlikKazaBasina.COVER_AMOUNT);

                    #region Teminat Vergileri
                    decimal ToplamVergi = 0;

                    var GiderVergi = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKazaBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    var GuvenceHesab = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKazaBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi);

                    var THGF = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.OlumSakatlikKazaBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.THGF);
                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);
                    if (GuvenceHesab != null)
                        ToplamVergi += RAYMessages.ToDecimal(GuvenceHesab.DEDUCTION_AMOUNT);
                    if (THGF != null)
                        ToplamVergi += RAYMessages.ToDecimal(THGF.DEDUCTION_AMOUNT);
                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(TrafikTeminatlar.Olum_Sakatlik_Kaza_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(OlumSakatlikKazaBasina.NET_PREMIUM), BrutPrim, 0);
                }
                var TedaviKisiBasina = trafikres.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKisiBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (TedaviKisiBasina != null)
                {

                    TeminatBedeli = RAYMessages.ToDecimal(TedaviKisiBasina.COVER_AMOUNT);

                    #region Teminat Vergileri
                    decimal ToplamVergi = 0;

                    var GiderVergi = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKisiBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    var GuvenceHesab = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKisiBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi);

                    var THGF = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKisiBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.THGF);
                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);
                    if (GuvenceHesab != null)
                        ToplamVergi += RAYMessages.ToDecimal(GuvenceHesab.DEDUCTION_AMOUNT);
                    if (THGF != null)
                        ToplamVergi += RAYMessages.ToDecimal(THGF.DEDUCTION_AMOUNT);
                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kisi_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(TedaviKisiBasina.NET_PREMIUM), BrutPrim, 0);
                }
                var TedaviKazaBasina = trafikres.POLMAS.POLTEM.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKazaBasina);
                TeminatBedeli = 0;
                BrutPrim = 0;
                if (TedaviKazaBasina != null)
                {
                    TeminatBedeli = RAYMessages.ToDecimal(TedaviKazaBasina.COVER_AMOUNT);

                    #region Teminat Vergileri
                    decimal ToplamVergi = 0;

                    var GiderVergi = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKazaBasina &&
                                                                              f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                              f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GiderVergisi);

                    var GuvenceHesab = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKazaBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.GuvenceHesabi);

                    var THGF = trafikres.POLMAS.PLTDID.FirstOrDefault(f => f.COVER_CODE == RAY_TrafikTeminatlar.SaglikGideriKazaBasina &&
                                                                             f.DEDUCTION_TYPE_CODE == RAY_TrafikKesitiTipKodlari.Vergiler &&
                                                                             f.DEDUCTION_CODE == RAY_TrafikVergiKesintiKodlari.THGF);
                    if (GiderVergi != null)
                        ToplamVergi = RAYMessages.ToDecimal(GiderVergi.DEDUCTION_AMOUNT);
                    if (GuvenceHesab != null)
                        ToplamVergi += RAYMessages.ToDecimal(GuvenceHesab.DEDUCTION_AMOUNT);
                    if (THGF != null)
                        ToplamVergi += RAYMessages.ToDecimal(THGF.DEDUCTION_AMOUNT);
                    #endregion

                    BrutPrim = TeminatBedeli + ToplamVergi;
                    this.AddTeminat(TrafikTeminatlar.Tedavi_Kaza_Basina, TeminatBedeli, ToplamVergi, RAYMessages.ToDecimal(TedaviKazaBasina.NET_PREMIUM), BrutPrim, 0);
                }

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
                this.AddWebServisCevap(Common.WebServisCevaplar.RAY_Teklif_Police_No, trafikres.POLMAS.POLICY_NO);
                this.AddWebServisCevap(Common.WebServisCevaplar.RAY_Musteri_No, MUSTERI_NO);
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
            ray.InsurerServices clnt = new ray.InsurerServices();
            try
            {
                #region Veri Hazırlama GENEL
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);

                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                #endregion

                #region Policelestirme Request


                clnt.Url = konfig[Konfig.RAY_ServiceURL];
                clnt.Timeout = 150000;
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.RAY });

                RAY_TrafikPolicelestirme_Request request = new RAY_TrafikPolicelestirme_Request();
                RAY_TrafikPolicelestirme_Response response = new RAY_TrafikPolicelestirme_Response();
                string SigortaliNo = this.ReadWebServisCevap(Common.WebServisCevaplar.RAY_Musteri_No, "0");
                string TUMPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.RAY_Teklif_Police_No, "0");

                request.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                request.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                request.CLIENT_IP = this.IpGetir(teklif.GenelBilgiler.TVMKodu);
                request.PRODUCT_NO = RAY_UrunKodlari.Trafik;
                request.PROCESS_ID = RAY_ProcesTipleri.Policelestirme;
                request.FIRM_CODE = "2";
                request.COMPANY_CODE = "2";
                request.POLICY_NO = TUMPoliceNo;
                request.RENEWAL_NO = "0";
                request.ENDORS_NO = "0";
                request.CHANNEL = servisKullanici.PartajNo_;
                request.ISSUE_CHANNEL = servisKullanici.PartajNo_;
                request.QUERY_METHOD = "O";
                request.PAYMENT_TYPE = "0";

                if (odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    request.PAYMENTS = new PAYMENTS();
                    request.PAYMENTS.CREDIT_CARD_NAME = odeme.KrediKarti.KartSahibi;
                    request.PAYMENTS.CREDIT_CARD_NO = odeme.KrediKarti.KartNo.Substring(0, 6);
                    request.PAYMENTS.CREDIT_CARD_CVV = "XXX";
                    request.PAYMENTS.CREDIT_CARD_VALID_MONTH = "99";
                    request.PAYMENTS.CREDIT_CARD_VALID_YEAR = "9999";
                    request.PAYMENTS.INSTALLMENT_NUMBER = odeme.TaksitSayisi.ToString();
                }

                this.BeginLog(request, request.GetType(), WebServisIstekTipleri.Police);

                //Kredi kartı bilgileri Loglanmıyor. Şifrelenecek alanlar
                if (odeme.KrediKarti != null && odeme.OdemeTipi == OdemeTipleri.KrediKarti)
                {
                    request.ENDORS_TYPE_CODE = "0";
                    request.PROD_CANCEL = "T";
                    request.CANCEL_REASON_CODE = "0";

                    DateTime polBaslangic = teklif.ReadSoru(TrafikSorular.Police_Baslangic_Tarihi, TurkeyDateTime.Today);
                    request.BEG_DATE = polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                    request.END_DATE = polBaslangic.AddYears(1).ToString("dd/MM/yyyy").Replace(".", "/");
                    request.CONFIRM_DATE = DateTime.Now.ToString("dd/MM/yyyy").Replace(".", "/"); //polBaslangic.ToString("dd/MM/yyyy").Replace(".", "/");
                    request.CUR_TYPE = "TL";
                    request.EXCHANGE_RATE = "1";
                    request.PAYMENT_TYPE = "0";
                    request.QUERY_METHOD = "A";
                    request.DESCRIPTION = "";

                    request.CLIENT_NO = SigortaliNo;
                    request.INSURED_NO = SigortaliNo;
                    request.PRINTOUT_TYPE = "0";
                    request.PLATE = teklif.Arac.PlakaKodu + teklif.Arac.PlakaNo;
                    request.HAS_INST_CARD = "1";


                    string[] adparts = odeme.KrediKarti.KartSahibi.Split(' ');

                    if (adparts.Length == 1)
                    {
                        request.PAYMENTS.CREDIT_CARD_NAME = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.KartSahibi);
                    }
                    else if (adparts.Length == 2)
                    {
                        request.PAYMENTS.CREDIT_CARD_NAME = INETTripleDesCrypto.EncryptMessage(adparts[0]);
                        request.PAYMENTS.CREDIT_CARD_SURNAME = INETTripleDesCrypto.EncryptMessage(adparts[1]);
                    }
                    else if (adparts.Length == 3)
                    {
                        request.PAYMENTS.CREDIT_CARD_NAME = INETTripleDesCrypto.EncryptMessage(adparts[0] + " " + adparts[1]);
                        request.PAYMENTS.CREDIT_CARD_SURNAME = INETTripleDesCrypto.EncryptMessage(adparts[2]);
                    }


                    request.PAYMENTS.CREDIT_CARD_NO = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.KartNo);
                    request.PAYMENTS.CREDIT_CARD_VALID_MONTH = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.SKA);
                    request.PAYMENTS.CREDIT_CARD_VALID_YEAR = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.SKY);
                    request.PAYMENTS.CREDIT_CARD_CVV = INETTripleDesCrypto.EncryptMessage(odeme.KrediKarti.CVC);
                    request.PAYMENTS.INSTALLMENT_NUMBER = odeme.TaksitSayisi.ToString();
                    request.PAYMENTS.CC_PREFIX = odeme.KrediKarti.KartNo.Substring(0, 6);
                    if (odeme.KrediKarti.KartNo.Substring(0, 1) == "5")
                        request.PAYMENTS.CREDIT_CARD_TYPE = "2"; //Master Card
                    else request.PAYMENTS.CREDIT_CARD_TYPE = "1"; //Visa
                }

                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_TrafikPolicelestirme_Request));
                StringWriter Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(Output, request);
                string responsePolice = clnt.ProductionIntegrator(Output.ToString());
                clnt.Dispose();
                Output.Close();

                _Serialize = new XmlSerializer(typeof(RAY_TrafikPolicelestirme_Response));

                using (TextReader reader = new StringReader(responsePolice))
                {
                    var policeHata = responsePolice.Contains("Error");
                    if (!policeHata)
                    {
                        response = (RAY_TrafikPolicelestirme_Response)_Serialize.Deserialize(reader);
                        reader.ReadToEnd();
                        this.EndLog(response, true, response.GetType());

                        this.GenelBilgiler.TUMPoliceNo = response.POLMAS.POLICY_NO;
                        this.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;

                        RAY_PoliceBasimi_Request pdfRequest = new RAY_PoliceBasimi_Request();
                        RAY_PoliceBasimi_Response pdfResponse = new RAY_PoliceBasimi_Response();

                        pdfRequest.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                        pdfRequest.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                        pdfRequest.PROCESS_ID = RAY_ProcesTipleri.PolicePDF;
                        pdfRequest.FIRM_CODE = "2";
                        pdfRequest.COMPANY_CODE = "2";
                        pdfRequest.PRODUCT_NO = RAY_UrunKodlari.Trafik;
                        pdfRequest.POLICY_NO = response.POLMAS.POLICY_NO;
                        pdfRequest.RENEWAL_NO = "0";
                        pdfRequest.ENDORS_NO = "0";
                        pdfRequest.PRINT_TYPE = RAY_PDFBasimTipleri.PolicePDF; //Police PDF

                        this.BeginLog(pdfRequest, pdfRequest.GetType(), WebServisIstekTipleri.Police);

                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Request));
                        Output = new StringWriter(new StringBuilder());

                        _Serialize.Serialize(Output, pdfRequest);
                        string responsePolicePDF = clnt.ProductionIntegrator(Output.ToString());
                        clnt.Dispose();
                        Output.Close();

                        _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Response));
                        using (TextReader readerPDF = new StringReader(responsePolicePDF))
                        {

                            pdfResponse = (RAY_PoliceBasimi_Response)_Serialize.Deserialize(readerPDF);
                            readerPDF.ReadToEnd();
                            this.EndLog(pdfResponse, true, pdfResponse.GetType());

                            if (pdfResponse.STATUS_CODE != "0")
                            {
                                WebClient myClient = new WebClient();
                                byte[] data = myClient.DownloadData(pdfResponse.LINK);

                                IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                                string fileName = String.Format("RAY_Trafik_Police_{0}.pdf", System.Guid.NewGuid().ToString());
                                string url = storage.UploadFile("trafik", fileName, data);
                                this.GenelBilgiler.PDFPolice = url;

                                _Log.Info("Police_PDF url: {0}", url);
                            }
                            else
                            {
                                this.EndLog(pdfResponse, false, pdfResponse.GetType());
                                this.AddHata(pdfResponse.STATUS_DESC);
                            }
                        }
                    }
                    else if (responsePolice.Contains("INET_ERROR"))
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_TrafikPolicelestirmeHata_Response));
                        using (StringReader readerPoliceHata2 = new StringReader(responsePolice))
                        {
                            RAY_TrafikPolicelestirmeHata_Response policeResponseHata = new RAY_TrafikPolicelestirmeHata_Response();
                            policeResponseHata = (RAY_TrafikPolicelestirmeHata_Response)_Serialize.Deserialize(readerPoliceHata2);
                            readerPoliceHata2.ReadToEnd();

                            this.EndLog(policeResponseHata, false, policeResponseHata.GetType());
                            this.AddHata(policeResponseHata.ERROR_MESSAGE);
                        }
                    }
                    else
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_TrafikPolicelestirmeHata2_Response));
                        using (StringReader readerPoliceHata = new StringReader(responsePolice))
                        {
                            RAY_TrafikPolicelestirmeHata2_Response policeResponseHata = new RAY_TrafikPolicelestirmeHata2_Response();
                            policeResponseHata = (RAY_TrafikPolicelestirmeHata2_Response)_Serialize.Deserialize(readerPoliceHata);
                            readerPoliceHata.ReadToEnd();

                            this.EndLog(policeResponseHata, false, policeResponseHata.GetType());
                            this.AddHata(policeResponseHata.Error.ErrDesc);
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

        public string MusteriNo(ITeklif teklif, MusteriGenelBilgiler sigortali, int tvmKodu, string servisUrl)
        {
            #region Müşteri No
            string MusteriNo = String.Empty;
            ray.InsurerServices clnt = new ray.InsurerServices();
            try
            {
                var tvmKod = _TVMService.GetServisKullaniciTVMKodu(tvmKodu);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKod, TeklifUretimMerkezleri.RAY });

                clnt.Url = servisUrl;
                clnt.Timeout = 150000;

                #region RAY Müşteri Database Sorgu

                RAY_Musteri_DatabaseSorgu_Request musDatabaseSorguReq = new RAY_Musteri_DatabaseSorgu_Request();
                RAY_Musteri_DatabaseSorgu_Response musDatabaseSorguRes = new RAY_Musteri_DatabaseSorgu_Response();

                musDatabaseSorguReq.PROCESS_ID = RAY_ProcesTipleri.MusteriDataBaseSorgula;
                musDatabaseSorguReq.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                musDatabaseSorguReq.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                musDatabaseSorguReq.CHANNEL = servisKullanici.PartajNo_;

                if (sigortali.KimlikNo.Length == 11)
                {
                    musDatabaseSorguReq.CITIZENSHIP_NUMBER = sigortali.KimlikNo;
                    musDatabaseSorguReq.TAX_NUMBER = "";
                }
                else if (sigortali.KimlikNo.Length == 10)
                {
                    musDatabaseSorguReq.CITIZENSHIP_NUMBER = "";
                    musDatabaseSorguReq.TAX_NUMBER = sigortali.KimlikNo;
                }
                musDatabaseSorguReq.FOREIGN_CITIZENSHIP_NUMBER = "";
                this.BeginLog(musDatabaseSorguReq, musDatabaseSorguReq.GetType(), WebServisIstekTipleri.MusteriKayit);
                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_Musteri_DatabaseSorgu_Request));
                StringWriter _Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(_Output, musDatabaseSorguReq);
                string responseMusteriDatabaseSorgu = clnt.ProductionIntegrator(_Output.ToString());
                clnt.Dispose();
                _Output.Close();
                #endregion

                #region RAY Sigortaya Müşteri Kaydet

                _Serialize = new XmlSerializer(typeof(RAY_Musteri_DatabaseSorgu_Response));
                using (StringReader reader = new StringReader(responseMusteriDatabaseSorgu))
                {
                    var hata = responseMusteriDatabaseSorgu.Contains("INFO");//Müşteri db de kayıtlı değil
                    var hata2 = responseMusteriDatabaseSorgu.Contains("ERROR"); // diğer hatalar

                    if (hata2)
                    {
                        _Serialize = new XmlSerializer(typeof(RAY_Musteri_KaydetmeHata_Response));
                        using (StringReader readerMusteriHata = new StringReader(responseMusteriDatabaseSorgu))
                        {
                            RAY_Musteri_KaydetmeHata_Response musterihatares = new RAY_Musteri_KaydetmeHata_Response();

                            musterihatares = (RAY_Musteri_KaydetmeHata_Response)_Serialize.Deserialize(readerMusteriHata);
                            readerMusteriHata.ReadToEnd();
                            this.EndLog(musterihatares, false, musterihatares.GetType());
                            this.AddHata(musterihatares.ERROR_DESC);
                        }
                    }

                    else if (!hata)
                    {
                        musDatabaseSorguRes = (RAY_Musteri_DatabaseSorgu_Response)_Serialize.Deserialize(reader);
                        reader.ReadToEnd();
                        MusteriNo = musDatabaseSorguRes.UNSMAS.UNIT_NO;
                        this.EndLog(musDatabaseSorguRes, true, musDatabaseSorguRes.GetType());
                    }
                    else
                    {

                        this.EndLog(musDatabaseSorguRes, true, musDatabaseSorguRes.GetType());

                        RAY_Musteri_Kaydetme_Request musKayReq = new RAY_Musteri_Kaydetme_Request();
                        RAY_Musteri_Kaydetme_Response musKaydRes = new RAY_Musteri_Kaydetme_Response();

                        MusteriTelefon telefon = sigortali.MusteriTelefons.FirstOrDefault();

                        #region Genel Bilgiler
                        musKayReq.UNSMAS = new UNSMAS();
                        musKayReq.UNSMAS.PROCESS_ID = RAY_ProcesTipleri.MusteriKaydetme;

                        musKayReq.UNSMAS.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                        musKayReq.UNSMAS.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                        musKayReq.UNSMAS.CHANNEL = servisKullanici.PartajNo_;
                        musKayReq.UNSMAS.CLIENT_IP = this.IpGetir(teklif.GenelBilgiler.TVMKodu);
                        if (sigortali.MusteriTipKodu == MusteriTipleri.TCMusteri)
                        {
                            musKayReq.UNSMAS.CITIZENSHIP_NUMBER = sigortali.KimlikNo;
                            musKayReq.UNSMAS.GENDER = sigortali.Cinsiyet;
                            musKayReq.UNSMAS.BIRTH_DATE = sigortali.DogumTarihi.HasValue ? sigortali.DogumTarihi.Value.ToString("dd/MM/yyyy") : null;
                            musKayReq.UNSMAS.FOREIGN_CITIZENSHIP_NUMBER = "";
                            musKayReq.UNSMAS.TAX_NUMBER = ""; //Vergi No
                        }
                        else if (sigortali.MusteriTipKodu == MusteriTipleri.TuzelMusteri || sigortali.MusteriTipKodu == MusteriTipleri.SahisFirmasi)
                        {
                            musKayReq.UNSMAS.TAX_NUMBER = sigortali.KimlikNo; //Vergi No
                            musKayReq.UNSMAS.GENDER = "";
                            musKayReq.UNSMAS.BIRTH_DATE = "";
                            musKayReq.UNSMAS.CITIZENSHIP_NUMBER = "";
                            musKayReq.UNSMAS.FOREIGN_CITIZENSHIP_NUMBER = "";

                        }
                        else if (sigortali.MusteriTipKodu == MusteriTipleri.YabanciMusteri)
                        {
                            musKayReq.UNSMAS.FOREIGN_CITIZENSHIP_NUMBER = sigortali.KimlikNo;
                            musKayReq.UNSMAS.BIRTH_DATE = sigortali.DogumTarihi.HasValue ? sigortali.DogumTarihi.Value.ToString("dd/mm/yyyy") : null;
                            musKayReq.UNSMAS.CITIZENSHIP_NUMBER = "";
                            musKayReq.UNSMAS.TAX_NUMBER = ""; //Vergi No
                            musKayReq.UNSMAS.GENDER = sigortali.Cinsiyet;

                        }

                        musKayReq.UNSMAS.TAX_OFFICE = "";
                        musKayReq.UNSMAS.IDENTITY_NO = "";
                        musKayReq.UNSMAS.COUNTRY_CODE = "90";
                        musKayReq.UNSMAS.NATIONALITY = "90";
                        musKayReq.UNSMAS.PERSONAL_COMMERCIAL = "0";
                        musKayReq.UNSMAS.FIRM_NAME = "";
                        musKayReq.UNSMAS.NAME = sigortali.AdiUnvan;
                        musKayReq.UNSMAS.SURNAME = sigortali.SoyadiUnvan;
                        musKayReq.UNSMAS.BIRTH_PLACE = "";
                        musKayReq.UNSMAS.EMAIL = sigortali.EMail;
                        musKayReq.UNSMAS.URL_ADDRESS1 = "";
                        musKayReq.UNSMAS.WORK_AREA = "3";
                        musKayReq.UNSMAS.SECTOR = "5";
                        musKayReq.UNSMAS.FATHER_NAME = ""; //?
                        musKayReq.UNSMAS.MOTHER_NAME = ""; //?
                        musKayReq.UNSMAS.CONNECT_ADDRESS = "1";
                        musKayReq.UNSMAS.MARITAL_STATUS = "X";
                        musKayReq.UNSMAS.OCCUPATION = "3";
                        musKayReq.UNSMAS.RESIDENT_IN_STATE = "E";

                        musKayReq.UNSMAS.EMPLOYEE_COUNT = "10";
                        musKayReq.UNSMAS.BLACK_LIST_CODE = "0";
                        musKayReq.UNSMAS.BLACK_LIST_ENTRY_REASON = "";
                        musKayReq.UNSMAS.BLACK_LIST_ENRTY_DATE = DateTime.Now.ToString("dd/MM/yyyy");
                        musKayReq.UNSMAS.SCORE = "5";

                        #endregion

                        #region Telefon

                        if (telefon != null)
                        {
                            musKayReq.UNSMAS.GSM_COUNTRY_CODE1 = telefon.Numara.Substring(0, 2);
                            musKayReq.UNSMAS.GSM_CODE1 = telefon.Numara.Substring(3, 3);
                            musKayReq.UNSMAS.GSM_NUMBER1 = telefon.Numara.Substring(7, 7);

                            musKayReq.UNSMAS.PHONE_COUNTRY_CODE1 = telefon.Numara.Substring(0, 2);
                            musKayReq.UNSMAS.PHONE_CODE1 = telefon.Numara.Substring(3, 3);
                            musKayReq.UNSMAS.PHONE_NUMBER1 = telefon.Numara.Substring(7, 7);
                            musKayReq.UNSMAS.PHONE_LINE1 = "56";

                            musKayReq.UNSMAS.FAX_COUNTRY_CODE1 = telefon.Numara.Substring(0, 2);
                            musKayReq.UNSMAS.FAX_CODE1 = telefon.Numara.Substring(3, 3);
                            musKayReq.UNSMAS.FAX_NUMBER1 = telefon.Numara.Substring(7, 7);
                            musKayReq.UNSMAS.FAX_LINE1 = "12";
                        }
                        else
                        {
                            musKayReq.UNSMAS.GSM_COUNTRY_CODE1 = "90";
                            musKayReq.UNSMAS.GSM_CODE1 = "999";
                            musKayReq.UNSMAS.GSM_NUMBER1 = "9999999";

                            musKayReq.UNSMAS.PHONE_COUNTRY_CODE1 = "90";
                            musKayReq.UNSMAS.PHONE_CODE1 = "999";
                            musKayReq.UNSMAS.PHONE_NUMBER1 = "9999999";
                            musKayReq.UNSMAS.PHONE_LINE1 = "56";

                            musKayReq.UNSMAS.PHONE_COUNTRY_CODE1 = "90";
                            musKayReq.UNSMAS.PHONE_CODE1 = "999";
                            musKayReq.UNSMAS.PHONE_NUMBER1 = "9999999";
                            musKayReq.UNSMAS.PHONE_LINE1 = "12";
                        }
                        #endregion

                        #region Adres
                        List<UNSADRR> adress = new List<UNSADRR>();
                        musKayReq.UNSADR = new UNSADRR[2];


                        UNSADRR unsadr = new UNSADRR();

                        MusteriAdre adres = sigortali.MusteriAdres.FirstOrDefault(f => f.Varsayilan == true);
                        if (adres != null)
                        {
                            CR_IlIlce ililce = _CRContext.CR_IlIlceRepository.Filter(f => f.TUMKodu == TeklifUretimMerkezleri.RAY &&
                                                                                     f.IlKodu == adres.IlKodu &&
                                                                                     f.IlceKodu == adres.IlceKodu)
                                                                       .SingleOrDefault<CR_IlIlce>();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Ulke;

                            if (adres.UlkeKodu == "TUR") unsadr.ADR_DATA = RAY_UlkeKodlari.Turkiye;
                            else unsadr.ADR_DATA = RAY_UlkeKodlari.Yabanci;

                            adress.Add(unsadr);

                            string il = String.Empty;
                            string ilce = String.Empty;

                            if (ililce != null)
                            {
                                il = ililce.CRIlAdi;
                                ilce = ililce.CRIlceAdi;
                            }
                            else
                            {
                                il = "";
                                ilce = "";
                            }
                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Il;
                            unsadr.ADR_DATA = il;
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Ilce;
                            unsadr.ADR_DATA = ilce;
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Cadde;
                            unsadr.ADR_DATA = adres.Cadde.ToUpper();
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Mahalle;
                            unsadr.ADR_DATA = adres.Mahalle.ToUpper();
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Sokak;
                            unsadr.ADR_DATA = adres.Sokak.ToUpper();
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.Bina;
                            if (!String.IsNullOrEmpty(adres.BinaNo))
                                unsadr.ADR_DATA = adres.BinaNo;
                            else unsadr.ADR_DATA = "1";
                            adress.Add(unsadr);

                            unsadr = new UNSADRR();
                            unsadr.WHICH_ADRESS = "1";
                            unsadr.ADR_TYPE = RAY_AdresTipleri.PostaKodu;
                            unsadr.ADR_DATA = adres.PostaKodu.ToString();
                            adress.Add(unsadr);
                        }
                        musKayReq.UNSADR = adress.ToArray();
                        #endregion

                        #region Sorular
                        List<USRCVP> lists = new List<USRCVP>();
                        musKayReq.USRCVP = new USRCVP[1];
                        USRCVP usrcvp = new USRCVP();

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "24";//İrtibat Kurulabilecek Kişi
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "27";//İrtibat Kurulabilecek Kişinin mail adresi
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "23";//Araç Sayısı
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);

                        usrcvp = new USRCVP();
                        usrcvp.QUESTION_CODE = "26";//Şirket Türü, zorunludur.
                        usrcvp.ANSWER = "";
                        lists.Add(usrcvp);
                        musKayReq.USRCVP = lists.ToArray();
                        #endregion

                        this.BeginLog(musKayReq, musKayReq.GetType(), WebServisIstekTipleri.MusteriKayit);

                        _Serialize = new XmlSerializer(typeof(RAY_Musteri_Kaydetme_Request));
                        _Output = new StringWriter(new StringBuilder());

                        _Serialize.Serialize(_Output, musKayReq);
                        string responseMusteri = clnt.ProductionIntegrator(_Output.ToString());
                        clnt.Dispose();
                        _Output.Close();

                        _Serialize = new XmlSerializer(typeof(RAY_Musteri_Kaydetme_Response));
                        using (StringReader readerMus = new StringReader(responseMusteri))
                        {
                            var error = responseMusteri.Contains("ERROR");
                            if (!error)
                            {
                                musKaydRes = (RAY_Musteri_Kaydetme_Response)_Serialize.Deserialize(readerMus);
                                readerMus.ReadToEnd();
                                MusteriNo = musKaydRes.INFO_DESC;

                                this.EndLog(musKaydRes, true, musKaydRes.GetType());
                            }
                            else
                            {
                                _Serialize = new XmlSerializer(typeof(RAY_Musteri_KaydetmeHata_Response));
                                using (StringReader readerMusteriHata = new StringReader(responseMusteri))
                                {
                                    RAY_Musteri_KaydetmeHata_Response musterihatares = new RAY_Musteri_KaydetmeHata_Response();

                                    musterihatares = (RAY_Musteri_KaydetmeHata_Response)_Serialize.Deserialize(readerMusteriHata);
                                    readerMusteriHata.ReadToEnd();
                                    this.EndLog(musterihatares, false, musterihatares.GetType());
                                    this.AddHata(musterihatares.ERROR_DESC);
                                }
                            }
                        }
                    }
                #endregion

                }
            }

            catch (Exception ex)
            {
                #region Hata Log
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);

                #endregion
            }
            #endregion

            return MusteriNo;
        }

        private QUESTION SoruEkle(string soruKodu, string cevap)
        {
            QUESTION question = new QUESTION();
            question.QUESTION_CODE = soruKodu;
            question.ANSWER = cevap;
            return question;
        }

        public override void DekontPDF()
        {
            ray.InsurerServices clnt = new ray.InsurerServices();
            try
            {
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleRAYService);

                clnt.Url = konfig[Konfig.RAY_ServiceURL];
                clnt.Timeout = 150000;

                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.RAY });

                RAY_PoliceBasimi_Request pdfRequest = new RAY_PoliceBasimi_Request();
                RAY_PoliceBasimi_Response pdfResponse = new RAY_PoliceBasimi_Response();

                string RAYPoliceNo = this.ReadWebServisCevap(Common.WebServisCevaplar.RAY_Teklif_Police_No, "0");

                pdfRequest.USER_NAME = INETTripleDesCrypto.EncryptMessage(servisKullanici.KullaniciAdi);
                pdfRequest.PASSWORD = INETTripleDesCrypto.EncryptMessage(servisKullanici.Sifre);
                pdfRequest.PROCESS_ID = RAY_ProcesTipleri.PolicePDF;
                pdfRequest.FIRM_CODE = "2";
                pdfRequest.COMPANY_CODE = "2";
                pdfRequest.PRODUCT_NO = RAY_UrunKodlari.Trafik;
                pdfRequest.POLICY_NO = RAYPoliceNo;
                pdfRequest.RENEWAL_NO = "0";
                pdfRequest.ENDORS_NO = "0";
                pdfRequest.PRINT_TYPE = RAY_PDFBasimTipleri.DekontPDF; //Dekont PDF

                this.BeginLog(pdfRequest, pdfRequest.GetType(), WebServisIstekTipleri.DekontPDF);
                StringWriter Output = new StringWriter(new StringBuilder());
                XmlSerializer _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Request));
                Output = new StringWriter(new StringBuilder());

                _Serialize.Serialize(Output, pdfRequest);
                string responsePolicePDF = clnt.ProductionIntegrator(Output.ToString());
                clnt.Dispose();
                Output.Close();

                _Serialize = new XmlSerializer(typeof(RAY_PoliceBasimi_Response));
                using (TextReader readerPDF = new StringReader(responsePolicePDF))
                {
                    pdfResponse = (RAY_PoliceBasimi_Response)_Serialize.Deserialize(readerPDF);
                    readerPDF.ReadToEnd();
                    this.EndLog(pdfResponse, true, pdfResponse.GetType());

                    if (pdfResponse.STATUS_CODE != "0")
                    {
                        WebClient myClient = new WebClient();
                        byte[] data = myClient.DownloadData(pdfResponse.LINK);

                        IPolicePDFStorage storage = DependencyResolver.Current.GetService<IPolicePDFStorage>();
                        string fileName = String.Format("RAY_Trafik_Police_Dekont_PDF_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                        string url = storage.UploadFile("trafik", fileName, data);
                        this.GenelBilgiler.PDFDekont = url;

                        _Log.Info("Dekont_PDF url: {0}", url);
                        _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                    }
                    else
                    {
                        this.EndLog(pdfResponse, false, pdfResponse.GetType());
                        this.AddHata("PDF dosyası alınamadı.");
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                clnt.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        public string IpGetir(int tvmKodu)
        {
            string cagriIP = "82.222.165.62";
            string gonderilenIp = String.Empty;
            gonderilenIp = ClientIPNo;  //Birebir ip  "88.249.209.253";
            var tvmDetay = _TVMContext.TVMDetayRepository.Find(s => s.Kodu == tvmKodu);
            if (tvmDetay != null)
            {
                var bagliOlduguTVMKodu = tvmDetay.BagliOlduguTVMKodu;
                if (bagliOlduguTVMKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.NeosinerjiTVMKodu || tvmKodu == NeosinerjiTVM.CagriTVMKodu || tvmKodu == NeosinerjiTVM.CagriBrokerTVMKodu)
                {
                    gonderilenIp = cagriIP;
                }
            }
            return gonderilenIp;
        }
    }
}