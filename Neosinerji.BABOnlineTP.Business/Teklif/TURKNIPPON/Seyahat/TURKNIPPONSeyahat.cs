using Microsoft.Practices.Unity;
using Neosinerji.BABOnlineTP.Business.Common;
using Neosinerji.BABOnlineTP.Business.Common.TURKNIPPON;
using Neosinerji.BABOnlineTP.Business.turknippon.seyahat;
using Neosinerji.BABOnlineTP.Database;
using Neosinerji.BABOnlineTP.Database.Models;
using Newtonsoft.Json.Linq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;


namespace Neosinerji.BABOnlineTP.Business.TURKNIPPON.Seyahat
{
    public class TURKNIPPONSeyahat : Teklif, ITURKNIPPONSeyahat
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
        public TURKNIPPONSeyahat(ICRService crService, ICRContext crContext, IMusteriService musteriService, IAracContext aracContext, IKonfigurasyonService konfigurasyonService,
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

        public ScopeOutput[] GetScopeList(bool isDomestic)
        {
            TravelService travelService = new TravelService();
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT);
            travelService.Url = konfig[Konfig.TURKNIPPON_SEYAHAT_ServisURL];

            return travelService.GetScopeOrPocket(isDomestic);
        }
        public AlternativeOutput[] GetAlternativeList(bool isDomestic, int ScopeOrPocket)
        {
            TravelService travelService = new TravelService();
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT);
            travelService.Url = konfig[Konfig.TURKNIPPON_SEYAHAT_ServisURL];

            return travelService.GetAlternative(isDomestic, ScopeOrPocket);
        }
        public CountryOutput[] GetCountryList(int Alternative)
        {
            TravelService travelService = new TravelService();
            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT);
            travelService.Url = konfig[Konfig.TURKNIPPON_SEYAHAT_ServisURL];

            return travelService.GetCountry(Alternative);
        }
        public JObject MergePDFFiles(JObject jsonObject)
        {
            bool isDomestic = (bool)jsonObject["IsDomestic"];
            JArray pdfFilesArray = (JArray)jsonObject["PdfFilesArray"];

            WebClient webClient = new WebClient();
            PdfDocument mergedPdf = new PdfDocument();

            foreach(JObject pdfFile in pdfFilesArray)
            {
                Stream stream = new MemoryStream(webClient.DownloadData((string)pdfFile["Url"]));
                PdfDocument currentPdf = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                if(isDomestic)
                {
                    bool isLastPdf = (pdfFile.Equals(pdfFilesArray.Last()));

                    if(!isLastPdf)
                    {
                        mergedPdf.AddPage(currentPdf.Pages[0]); // If not last pdf just pick first page of the  policy
                    }
                    else
                    {
                        CopyPages(currentPdf, mergedPdf); // If this is the last pdf then copy all pages to merged pdf
                    }
                }
                else
                {
                    CopyPages(currentPdf, mergedPdf); // If this is an abroad travel policy then copy all pages
                }
            }

            PolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<PolicePDFStorage>();
            string pdfFilename = String.Format("TURKNIPPON_Seyahat_Toplu_Police_{0}.pdf", Guid.NewGuid().ToString("N"));
            MemoryStream mergedPdfStream = new MemoryStream();
            mergedPdf.Save(mergedPdfStream);
            string mergedPdfUrl = pdfStorage.UploadFile("seyahat", pdfFilename, mergedPdfStream.ToArray());

            JObject mergedPdfJson = new JObject();
            if (String.IsNullOrEmpty(mergedPdfUrl))
            {
                mergedPdfJson["MergedPdfUrl"] = "";
                mergedPdfJson["IsSuccess"] = false;
            }
            else
            {
                mergedPdfJson["MergedPdfUrl"] = mergedPdfUrl;
                mergedPdfJson["IsSuccess"] = true;
            }
            return mergedPdfJson;
        }
        void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }
        public JObject CreateOfferRecord(JObject insuredsJson)
        {
            int preparerTVMCode = (int)insuredsJson["preparerTVMCode"];
            int preparerTVMUserCode = (int)insuredsJson["preparerTVMUserCode"];
            int insurerCustomerCode = (int)insuredsJson["insurerCustomerCode"];
            ITeklif teklif = Teklif.Create(
                UrunKodlari.YurtDisiSeyehatSaglik,
                preparerTVMCode,
                preparerTVMUserCode,
                insurerCustomerCode,
                _AktifKullaniciService.TVMKodu,
                _AktifKullaniciService.KullaniciKodu
                );

            JArray insuredArray = (JArray)insuredsJson["insureds"];
            foreach (JObject insured in insuredArray)
            {
                teklif.AddSigortali((int)insured["CustomerCode"]);
            }
            teklif.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Teklif;
            teklif = _TeklifService.Create(teklif);
            JObject jObject = new JObject();
            if (teklif != null)
            {
                jObject["Status"] = true;
            }
            else
            {
                jObject["Status"] = false;
            }

            return jObject;
        }
        public JObject CreatePolicyRecord(JObject insuredsJson)
        {
            int preparerTVMCode = (int)insuredsJson["PreparerTVMCode"];
            int preparerTVMUserCode = (int)insuredsJson["PreparerTVMUserCode"];
            int insurerCustomerCode = (int)insuredsJson["InsurerCustomerCode"];
            ITeklif teklif = Teklif.Create(
                UrunKodlari.YurtDisiSeyehatSaglik,
                preparerTVMCode,
                preparerTVMUserCode,
                insurerCustomerCode,
                _AktifKullaniciService.TVMKodu,
                _AktifKullaniciService.KullaniciKodu
                );

            JArray insuredArray = (JArray)insuredsJson["Insureds"];
            bool isMergedPolicy = (bool)insuredsJson["IsMergedPolicy"];
            bool isDomestic = (bool)insuredsJson["IsDomestic"];
            int insuredCount = (int)insuredsJson["InsuredCount"];

            foreach (JObject insured in insuredArray)
            {
                teklif.AddSigortali((int)insured["CustomerCode"]);
            }
            var englishPdfUrl = (string)insuredsJson["EnglishPolicyPdfUrl"];
            if (!String.IsNullOrEmpty(englishPdfUrl))
            {
                teklif.GenelBilgiler.TeklifDokumans.Add(new TeklifDokuman
                {
                    DokumanAdi = "İngilizce Poliçe PDF",
                    DokumanTipi = 1,
                    DokumanURL = englishPdfUrl,
                    KayitTarihi = DateTime.Now,
                });
            }

            teklif.GenelBilgiler.PDFPolice = (string)insuredsJson["TurkishPolicyPdfUrl"];
            teklif.GenelBilgiler.TeklifDurumKodu = TeklifDurumlari.Police;
            teklif.GenelBilgiler.TUMKodu = 11;
            teklif.GenelBilgiler.TUMPoliceNo = isMergedPolicy ? "0" : (string)insuredsJson["PolicyNo"];
            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("tr-TR");
            teklif.GenelBilgiler.BaslamaTarihi = DateTime.Parse((string)insuredsJson["BeginDate"], cultureinfo);
            teklif.GenelBilgiler.BitisTarihi = DateTime.Parse((string)insuredsJson["EndDate"], cultureinfo);
            teklif.GenelBilgiler.BrutPrim = (decimal)insuredsJson["Premium"];
            teklif.GenelBilgiler.NetPrim = (decimal)insuredsJson["Premium"];
            teklif.GenelBilgiler.DovizKurBedeli = (decimal)insuredsJson["ExchangeRate"];
            //teklif.AddSoru(SeyehatSaglikSorular.Kisi_Sayisi, (string)insuredsJson["InsuredCount"]);
            //teklif.AddSoru(SeyehatSaglikSorular.Toplu_Police_Mi, (bool)insuredsJson["IsMergedPolicy"]);
            string descriptionText = "";


            if (isMergedPolicy)
            {
                descriptionText += "Toplu Poliçe <br>Sigortalı Adedi : " + insuredCount + " <br>";
            }

            if(isDomestic)
            {
                descriptionText += "Yurtiçi Seyahat <br>";
            }
            else
            {
                descriptionText += "Yurtdışı Seyahat <br>";
            }
            teklif.AddSoru(SeyehatSaglikSorular.Aciklama, descriptionText);

            teklif = _TeklifService.Create(teklif);
            JObject jObject = new JObject();
            if (teklif != null)
            {
                jObject["Status"] = true;
            }
            else
            {
                jObject["Status"] = false;
            }

            return jObject;
        }

        public TravelOutput Print(JObject insuredJson)
        {
            List<TVMWebServisKullanicilari> webServisKullanicilari = _TVMService.GetListTVMWebServisKullanicilari(_AktifKullaniciService.TVMKodu);
            string username = webServisKullanicilari.FirstOrDefault().KullaniciAdi;
            string partaj = webServisKullanicilari.FirstOrDefault().PartajNo_;
            string proxyIP = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[0];
            string proxyPort = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[1];

            #region Policy Info
            TravelInput travelInput = new TravelInput();
            travelInput.TrackingCode = (string)insuredJson["TrackingCode"];
            travelInput.UnitNo = (long)insuredJson["UnitNo"];
            travelInput.PolicyNo = (long)insuredJson["PolicyNo"];
            travelInput.ClientNo = (long)insuredJson["ClientNo"];
            travelInput.UseCreditCard = true;
            travelInput.IsTestMode = false;
            travelInput.Channel = Convert.ToInt32(partaj);
            travelInput.Username = username;

            travelInput.IsDomestic = (bool)insuredJson["IsDomestic"];
            travelInput.Scope = (int)insuredJson["Scope"];
            travelInput.TravelPocket = (int)insuredJson["TravelPocket"];
            travelInput.Alternative = (int)insuredJson["Alternative"];
            travelInput.Country = (int)insuredJson["Country"];
            travelInput.PersonalOrParticular = "B";
            travelInput.PrintType = (int)insuredJson["PrintType"];
            #endregion

            #region Dummy Credit Card
            travelInput.CreditCard = new CreditCardInput();
            travelInput.UseCreditCard = true;
            travelInput.CreditCard.CardType = 0;
            travelInput.CreditCard.CardNumber = "1111222233334444";
            travelInput.CreditCard.Month = "01";
            travelInput.CreditCard.Year = "2000";
            travelInput.CreditCard.CVV = "100";
            travelInput.CreditCard.CardHolderFirstname = "DUMMY";
            travelInput.CreditCard.CardHolderLastname = "DUMMY";
            travelInput.CreditCard.Installment = 1;
            #endregion

            this.BeginLog(travelInput, typeof(TravelInput), WebServisIstekTipleri.Police);

            TravelService travelService = new TravelService();
            travelService.Proxy = new WebProxy(proxyIP, Convert.ToInt32(proxyPort));
            travelService.Timeout = -1;

            travelService.Url = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT)[Konfig.TURKNIPPON_SEYAHAT_ServisURL];
            TravelOutput travelOutput = travelService.Print(travelInput);
            travelService.Dispose();
            if (!travelOutput.IsSuccess)
            {
                this.EndLog(travelOutput, false, travelOutput.GetType());
                this.AddHata(travelOutput.StatusDescription);
            }
            else
            {
                this.EndLog(travelOutput, true, travelOutput.GetType());
                PolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<PolicePDFStorage>();

                byte[] pdfData = new WebClient().DownloadData(travelOutput.PrintDownloadUrl);
                string pdfFilename = String.Format("TURKNIPPON_Seyahat_Tekli_Police_{0}_{1}.pdf", travelInput.PolicyNo, Guid.NewGuid().ToString("N"));
                string pdfUrl = pdfStorage.UploadFile("seyahat", pdfFilename, pdfData);
                //this.GenelBilgiler.PDFPolice = pdfUrl;
                _Log.Info("Police_PDF url: {0}", pdfUrl);
                travelOutput.PrintDownloadUrl = pdfUrl;
                //_TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
            }
            return travelOutput;
        }

        public TravelOutput Approve(JObject insuredJson)
        {
            List<TVMWebServisKullanicilari> webServisKullanicilari = _TVMService.GetListTVMWebServisKullanicilari(_AktifKullaniciService.TVMKodu);
            string username = webServisKullanicilari.FirstOrDefault().KullaniciAdi;
            string partaj = webServisKullanicilari.FirstOrDefault().PartajNo_;
            string proxyIP = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[0];
            string proxyPort = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[1];

            TravelService travelService = new TravelService();
            travelService.Proxy = new WebProxy(proxyIP, Convert.ToInt32(proxyPort));
            travelService.Timeout = -1;

            #region Veri Hazırlama

            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT);

            travelService.Url = konfig[Konfig.TURKNIPPON_SEYAHAT_ServisURL];


            TravelInput travelInput = new TravelInput();
            travelInput.IsTestMode = false;
            travelInput.UseCreditCard = true;
            travelInput.Channel = Convert.ToInt32(partaj);
            travelInput.Username = username;

            travelInput.TrackingCode = (string)insuredJson["TrackingCode"];
            travelInput.UnitNo = (long)insuredJson["UnitNo"];
            travelInput.PolicyNo = (long)insuredJson["PolicyNo"];
            travelInput.ClientNo = (long)insuredJson["ClientNo"];
            travelInput.PersonalOrParticular = "B";
            travelInput.IsDomestic = (bool)insuredJson["IsDomestic"];

            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("tr-TR");
            travelInput.BeginDate = DateTime.Parse((string)insuredJson["BeginDate"], cultureinfo);
            travelInput.EndDate = DateTime.Parse((string)insuredJson["EndDate"], cultureinfo);
            travelInput.Country = (int)insuredJson["Country"];
            travelInput.Scope = (int)insuredJson["Scope"];
            travelInput.Alternative = (int)insuredJson["Alternative"];
            travelInput.TravelPocket = (int)insuredJson["TravelPocket"];
            #endregion


            #region Credit Card
            JObject creditCard = (JObject)insuredJson["CreditCard"];
            travelInput.CreditCard = new CreditCardInput();
            travelInput.UseCreditCard = true;
            if (((string)creditCard["CardNumber"]).Substring(0, 1) == "5")
                travelInput.CreditCard.CardType = 2;
            else
                travelInput.CreditCard.CardType = 1;

            travelInput.CreditCard.CardNumber = Regex.Replace((string)creditCard["CardNumber"], @"\s+", "");
            travelInput.CreditCard.Month = (string)creditCard["Month"];
            travelInput.CreditCard.Year = (string)creditCard["Year"];
            travelInput.CreditCard.CVV = (string)creditCard["CVV"];
            travelInput.CreditCard.CardHolderFirstname = (string)creditCard["CardHolderFirstname"];
            travelInput.CreditCard.CardHolderLastname = (string)creditCard["CardHolderLastname"];
            travelInput.CreditCard.Installment = 1;
            #endregion



            #region Service Call
            TravelOutput approveResult = new TravelOutput();
            this.BeginLog(travelInput, travelInput.GetType(), WebServisIstekTipleri.Police);
            approveResult = travelService.Approve(travelInput);
            travelService.Dispose();

            #endregion

            #region Varsa Hata Kaydı

            if (!approveResult.IsSuccess)
            {
                if (!String.IsNullOrEmpty(approveResult.StatusDescription))
                {
                    this.EndLog(approveResult, false, approveResult.GetType());
                    this.AddHata(approveResult.StatusDescription);
                }
            }
            else
            {
                this.EndLog(approveResult, true, approveResult.GetType());
            }
            #endregion
            return approveResult;
        }

        public TravelOutput Compute(JObject insuredJson)
        {
            List<TVMWebServisKullanicilari> webServisKullanicilari = _TVMService.GetListTVMWebServisKullanicilari(_AktifKullaniciService.TVMKodu);
            string username = webServisKullanicilari.FirstOrDefault().KullaniciAdi;
            string partaj = webServisKullanicilari.FirstOrDefault().PartajNo_;
            string proxyIP = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[0];
            string proxyPort = webServisKullanicilari.FirstOrDefault().KullaniciAdi2.Split(':')[1];

            TravelService travelService = new TravelService();
            travelService.Proxy = new WebProxy(proxyIP, Convert.ToInt32(proxyPort));
            travelService.Timeout = -1;

            KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT);
            travelService.Url = konfig[Konfig.TURKNIPPON_SEYAHAT_ServisURL];


            string insurerType = (string)insuredJson["InsurerType"];
            string insurerIdentityNo = (string)insuredJson["InsurerIdentityNo"];

            TravelInput travelInput = new TravelInput();
            if(insurerType == "Private")
            {
                travelInput.ClientCitizenshipNumber = insurerIdentityNo;
            }
            else
            {
                travelInput.ClientTaxNumber = insurerIdentityNo;
            }

            travelInput.IsTestMode = false;
            travelInput.UseCreditCard = true;
            travelInput.Channel = Convert.ToInt32(partaj);
            travelInput.Username = username;
            travelInput.CitizenshipNumber = (string)insuredJson["CitizenshipNumber"];

            System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("tr-TR");
            travelInput.BeginDate = DateTime.Parse((string)insuredJson["BeginDate"], cultureinfo);
            travelInput.EndDate = DateTime.Parse((string)insuredJson["EndDate"], cultureinfo);
            travelInput.IsDomestic = (bool)insuredJson["IsDomestic"];
            travelInput.Scope = (int)insuredJson["Scope"];
            travelInput.TravelPocket = (int)insuredJson["TravelPocket"];
            if(travelInput.IsDomestic == false) // Abroad travels now have plancode for "METİN SİGORTA A.Ş."
            {
                travelInput.PlanCode = (string)insuredJson["PlanCode"];
            }
            travelInput.Alternative = (int)insuredJson["Alternative"];
            travelInput.Country = (int)insuredJson["Country"];
            travelInput.IsSkiing = (bool)insuredJson["IsSkiing"];
            travelInput.PersonalOrParticular = "B";

            #region Service Call
            TravelOutput proposalResult = new TravelOutput();
            this.BeginLog(travelInput, travelInput.GetType(), WebServisIstekTipleri.Teklif);
            proposalResult = travelService.Proposal(travelInput);
            travelService.Dispose();

            #endregion

            #region Varsa Hata Kaydı

            if (!proposalResult.IsSuccess)
            {
                if (!String.IsNullOrEmpty(proposalResult.StatusDescription))
                {
                    this.EndLog(proposalResult, false, proposalResult.GetType());
                    this.AddHata(proposalResult.StatusDescription);
                }
            }
            else
            {
                this.EndLog(proposalResult, true, proposalResult.GetType());
            }
            #endregion
            return proposalResult;
        }

        public override void Hesapla(ITeklif teklif)
        {
            try
            {
                TravelService travelService = new TravelService();
                travelService.Timeout = 250000;

                #region Ana Bilgiler
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT);
                travelService.Url = konfig[Konfig.TURKNIPPON_SEYAHAT_ServisURL];
                List<TVMWebServisKullanicilari> webServisKullanicilari = _TVMService.GetListTVMWebServisKullanicilari(_AktifKullaniciService.TVMKodu);
                string username = webServisKullanicilari.FirstOrDefault().KullaniciAdi;
                string partaj = webServisKullanicilari.FirstOrDefault().PartajNo_;

                TeklifSigortaEttiren sigortaEttiren = teklif.SigortaEttiren;
                #endregion
                TravelInput travelInput = new TravelInput();
                travelInput.IsTestMode = false;
                travelInput.UseCreditCard = true;
                travelInput.Channel = Convert.ToInt32(partaj);
                travelInput.Username = username;
                //travelInput.CitizenshipNumber = sigortali.KimlikNo;
                travelInput.BeginDate = teklif.ReadSoruNullableDateTime(SeyehatSaglikSorular.Seyehat_Baslangic_Tarihi);
                travelInput.EndDate = teklif.ReadSoruNullableDateTime(SeyehatSaglikSorular.Seyehat_Bitis_Tarihi);
                travelInput.IsDomestic = teklif.ReadSoruNullableBool(SeyehatSaglikSorular.NipponIsDomestic);
                travelInput.Scope = teklif.ReadSoruNullableInt(SeyehatSaglikSorular.NipponScope);
                travelInput.TravelPocket = teklif.ReadSoruNullableInt(SeyehatSaglikSorular.NipponTravelPocket);
                travelInput.Alternative = teklif.ReadSoruNullableInt(SeyehatSaglikSorular.NipponAlternative);
                travelInput.Country = teklif.ReadSoruNullableInt(SeyehatSaglikSorular.NipponCountry);
                travelInput.IsSkiing = teklif.ReadSoru(SeyehatSaglikSorular.Kayak_Teminati_Varmi, false);
                travelInput.PersonalOrParticular = "B";

                #region Service Call
                TravelOutput proposalResult = new TravelOutput();
                this.BeginLog(travelInput, travelInput.GetType(), WebServisIstekTipleri.Teklif);
                proposalResult = travelService.Proposal(travelInput);
                travelService.Dispose();

                #endregion

                #region Varsa Hata Kaydı

                if (!proposalResult.IsSuccess)
                {
                    if (!String.IsNullOrEmpty(proposalResult.StatusDescription))
                    {
                        this.EndLog(proposalResult, false, proposalResult.GetType());
                        this.AddHata(proposalResult.StatusDescription);
                    }
                }
                else
                {
                    this.EndLog(proposalResult, true, proposalResult.GetType());
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

                #region Genel bilgile
                this.Import(teklif);
                this.GenelBilgiler.Basarili = true;
                this.GenelBilgiler.BaslamaTarihi = proposalResult.BeginDate;
                this.GenelBilgiler.BitisTarihi = proposalResult.EndDate;
                this.GenelBilgiler.GecerlilikBitisTarihi = TurkeyDateTime.Today;
                this.GenelBilgiler.BrutPrim = proposalResult.Premium;
                this.GenelBilgiler.NetPrim = proposalResult.Premium;
                this.GenelBilgiler.ToplamVergi = 0;
                this.GenelBilgiler.DovizTL = DovizTLTipleri.Doviz;
                this.GenelBilgiler.DovizKurBedeli = proposalResult.ExchangeRate;
                this.GenelBilgiler.OdemeTipi = teklif.GenelBilgiler.OdemeTipi;
                this.GenelBilgiler.OdemeSekli = teklif.GenelBilgiler.OdemeSekli;
                this.GenelBilgiler.TaksitSayisi = teklif.GenelBilgiler.TaksitSayisi;
                this.GenelBilgiler.TUMTeklifNo = proposalResult.PolicyNo.ToString();
                #endregion

                #region Web servis cevapları

                string musterino = proposalResult.UnitNo.ToString();
                string clientNo = proposalResult.ClientNo.ToString();

                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, proposalResult.PolicyNo.ToString());
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, musterino);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Client_No, clientNo);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, proposalResult.TrackingCode);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_PersonalOrParticular, proposalResult.Input.PersonalOrParticular);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_YurticiVeyaYurtdisi, (proposalResult.Input.IsDomestic ?? false));
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_PoliceBaslangicTarihi, proposalResult.Input.BeginDate ?? DateTime.Today);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_PoliceBitisTarihi, proposalResult.Input.EndDate ?? DateTime.Today);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_UlkeKodu, proposalResult.Input.Country ?? 0);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_KapsamKodu, proposalResult.Input.Scope ?? 0);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_PaketKodu, proposalResult.Input.TravelPocket ?? 0);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_AlternatifKodu, proposalResult.Input.Alternative ?? 0);
                this.AddWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_EuroKuru, proposalResult.ExchangeRate);
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
            TravelService travelService = new turknippon.seyahat.TravelService();
            try
            {
                #region Veri Hazırlama

                KonfigTable konfig = _KonfigurasyonService.GetKonfig(Konfig.BundleTURKNIPPONSEYAHAT);

                travelService.Url = konfig[Konfig.TURKNIPPON_SEYAHAT_ServisURL];
                travelService.Timeout = 150000;
                TURKNIPPON_Proposal_Response result = new TURKNIPPON_Proposal_Response();
                ITeklif teklif = _TeklifService.GetAnaTeklif(this.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
                var tvmKodu = _TVMService.GetServisKullaniciTVMKodu(teklif.GenelBilgiler.KaydiEKleyenTVMKodu.Value);
                TVMWebServisKullanicilari servisKullanici = _TVMContext.TVMWebServisKullanicilariRepository.FindById(new object[] { tvmKodu, TeklifUretimMerkezleri.TURKNIPPON });
                MusteriGenelBilgiler sigortali = _MusteriService.GetMusteri(teklif.Sigortalilar.FirstOrDefault().MusteriKodu);
                MusteriTelefon Telefon = sigortali.MusteriTelefons.FirstOrDefault(t => t.IletisimNumaraTipi == IletisimNumaraTipleri.Cep);
                TravelInput request = new TravelInput();
                TravelOutput response = new TravelOutput();

                KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
                string testMi = konfigTestMi[Konfig.TestMi];
                #endregion

                #region Genel Bilgiler

                string IslemTakipKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_IslemTakipKodu, "0");
                string SigortaliMusteriNo = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_SigortaliUnit_No, "0");
                string PoliceNumarasi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Teklif_Police_No, "0");
                string ClientNumarasi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Client_No, "0");
                string PersonalOrParticular = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_PersonalOrParticular, "B");
                bool YurticiVeyaYurtdisi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_YurticiVeyaYurtdisi, false);
                DateTime PoliceBaslangicTarihi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_PoliceBaslangicTarihi, TurkeyDateTime.Today);
                DateTime PoliceBitisTarihi = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_PoliceBitisTarihi, TurkeyDateTime.Today);
                int UlkeKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_UlkeKodu, 0);
                int KapsamKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_KapsamKodu, 0);
                int AlternatifKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_AlternatifKodu, 0);
                int PaketKodu = this.ReadWebServisCevap(Common.WebServisCevaplar.TURKNIPPON_Seyahat_PaketKodu, 0);

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
                request.Channel = 30476;//Convert.ToInt32(servisKullanici.PartajNo_);
                request.Username = "30476001";//servisKullanici.KullaniciAdi;
                request.TrackingCode = IslemTakipKodu;
                request.UnitNo = Convert.ToInt64(SigortaliMusteriNo);
                request.PolicyNo = Convert.ToInt64(PoliceNumarasi);
                request.ClientNo = Convert.ToInt64(ClientNumarasi);
                request.PersonalOrParticular = PersonalOrParticular;
                request.IsDomestic = YurticiVeyaYurtdisi;
                request.BeginDate = PoliceBaslangicTarihi;
                request.EndDate = PoliceBitisTarihi;
                request.Country = Convert.ToInt32(UlkeKodu);
                request.Scope = Convert.ToInt32(KapsamKodu);
                request.Alternative = Convert.ToInt32(AlternatifKodu);
                request.TravelPocket = Convert.ToInt32(PaketKodu);

                #endregion


                #region Service Call
                this.BeginLog(request, typeof(TravelInput), WebServisIstekTipleri.Police);

                //KrediKarti bilgileri loglanmıyor..
                #region KrediKartı Bilgileri
                request.CreditCard = new CreditCardInput();
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
                    request.CreditCard.Installment = 1;//odeme.TaksitSayisi;

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

                response = travelService.Approve(request);
                travelService.Dispose();
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

                    int[] basimTipi = new int[3];
                    basimTipi[0] = TurkNippon_BaskiTipleri.TeklifPolice;
                    //basimTipi[1] = TurkNippon_BaskiTipleri.BilgilendirmeFormu;
                    this.PDfGetir(servisKullanici, travelService, IslemTakipKodu, PoliceNumarasi, SigortaliMusteriNo, basimTipi, false, odeme, response);

                }
                this.GenelBilgiler.WEBServisLogs = this.Log;
                _TeklifService.UpdateGenelBilgiler(this.GenelBilgiler);
                #endregion
            }
            catch (Exception ex)
            {
                _Log.Error("TurkNipponKasko.Policelestir", ex);
                travelService.Abort();
                this.EndLog(ex.Message, false);
                this.AddHata(ex.Message);
            }
        }

        public void PDfGetir(TVMWebServisKullanicilari servisKullanici, TravelService travelService, string islemTakipKodu, string policeNo, string musteriNo, int[] basimtipi, bool teklif, Odeme odeme, TravelOutput response)
        {
            KonfigTable konfigTestMi = _KonfigurasyonService.GetKonfig(Konfig.BundleTestMi);
            string testMi = konfigTestMi[Konfig.TestMi];
            TravelInput travelInput = new TravelInput();
            TravelOutput travelOutput = new TravelOutput();

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

            travelInput.TrackingCode = islemTakipKodu;
            travelInput.UnitNo = Convert.ToInt64(musteriNo);
            travelInput.PolicyNo = Convert.ToInt64(policeNo);
            travelInput.ClientNo = Convert.ToInt64(musteriNo);
            travelInput.UseCreditCard = true;
            travelInput.IsTestMode = false;
            travelInput.Channel = 30476;//Convert.ToInt32(servisKullanici.PartajNo_);
            travelInput.Username = "30476001";//servisKullanici.KullaniciAdi;
            travelInput.IsDomestic = response.Input.IsDomestic;
            travelInput.Scope = response.Input.Scope;
            travelInput.TravelPocket = response.Input.TravelPocket;
            travelInput.Alternative = response.Input.Alternative;
            travelInput.Country = response.Input.Country;
            travelInput.IsSkiing = response.Input.IsSkiing;
            travelInput.PersonalOrParticular = response.Input.PersonalOrParticular;
            travelInput.TrackingCode = response.Input.TrackingCode;
            travelInput.PolicyNo = response.Input.PolicyNo;

            travelInput.CreditCard = new CreditCardInput();
            if (odeme.KrediKarti.KartNo.Substring(0, 1) == "5")
                travelInput.CreditCard.CardType = 2;
            else travelInput.CreditCard.CardType = 1;
            travelInput.CreditCard.CardNumber = odeme.KrediKarti.KartNo;
            travelInput.CreditCard.Month = odeme.KrediKarti.SKA;
            travelInput.CreditCard.Year = odeme.KrediKarti.SKY;
            travelInput.CreditCard.CVV = odeme.KrediKarti.CVC;
            travelInput.CreditCard.CardHolderFirstname = adi;
            travelInput.CreditCard.CardHolderLastname = soyadi;
            travelInput.CreditCard.Installment = 1;//odeme.TaksitSayisi;

            foreach (var item in basimtipi)
            {
                if (item > 0)
                {
                    travelInput.PrintType = item;

                    this.BeginLog(travelInput, typeof(TravelInput), WebServisIstekTipleri.Police);
                    travelOutput = travelService.Print(travelInput);
                    travelService.Dispose();
                    if (!travelOutput.IsSuccess)
                    {
                        this.EndLog(travelOutput, false, travelOutput.GetType());
                        this.AddHata(travelOutput.StatusDescription);
                    }
                    else
                    {
                        this.EndLog(travelOutput, true, travelOutput.GetType());
                        PolicePDFStorage pdfStorage = DependencyResolver.Current.GetService<PolicePDFStorage>();

                        var policePDF = travelOutput.PrintDownloadUrl;
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
                                    fileName = String.Format("TURKNIPPON_Seyahat_Teklif_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                    url = storage.UploadFile("seyahat", fileName, data);
                                    this.GenelBilgiler.PDFDosyasi = url;
                                    _Log.Info("Teklif_PDF url: {0}", url);
                                }
                                else
                                {
                                    fileName = String.Format("TURKNIPPON_Seyahat_Police_{0}.pdf", System.Guid.NewGuid().ToString("N"));
                                    url = pdfStorage.UploadFile("seyahat", fileName, data);
                                    this.GenelBilgiler.PDFPolice = url;
                                    _Log.Info("Teklif_PDF url: {0}", url);
                                }
                            }
                            if (item == TurkNippon_BaskiTipleri.BilgilendirmeFormu)
                            {
                                fileName = String.Format("TURKNIPPON_Seyahat_Police_Bilgilendirme_{0}.pdf", System.Guid.NewGuid().ToString("N"));
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
    }
}
