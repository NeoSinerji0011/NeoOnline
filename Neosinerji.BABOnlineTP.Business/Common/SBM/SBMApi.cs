using HtmlAgilityPack;
using Neosinerji.BABOnlineTP.Database.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Business.Common.SBM
{
    public class SBMApi
    {
        ITVMService _TVMService;
        IAktifKullaniciService _AktifKullaniciService;


        private const string getCookiesRequestUrl = "https://online.sbm.org.tr/sbm-admin/cweb/public/home.sbm";
        private const string loginRequestUrl = "https://online.sbm.org.tr/sbm-admin/cweb/public/submitLogin.sbm";
        private const string policeSorguPageRequestUrl = "https://online.sbm.org.tr/trm-police/trafik/policeSorgu.sbm";
        private const string queryFormRequestUrl = "https://online.sbm.org.tr/trm-police/trafik/list.sbm";
        private string firstPolicyDetailsRequestUrl;

        private string ProxyHost;
        private int ProxyPort;

        public SBMApi(string proxyHost, int proxyPort)
        {
            _AktifKullaniciService = DependencyResolver.Current.GetService<IAktifKullaniciService>();
            _TVMService = DependencyResolver.Current.GetService<ITVMService>();

            this.ProxyHost = proxyHost;
            this.ProxyPort = proxyPort;
        }

        public SBMTrafficPolicyDetail getLastTrafficPolicyDetail(string kimlikNo, int plakaIlKodu, string plakaNo)
        {
            OtoLoginSigortaSirketKullanicilar sbmWebServisKullanici = _TVMService.GetNeoConnectKullanici(_AktifKullaniciService.TVMKodu, 65);

            RestResponseCookie JSESSIONIDCookie;
            RestResponseCookie NSCCookie;
            RestResponseCookie rmbtkn;
            RestResponseCookie SbmSessionId;
            RestResponseCookie lang;
            RestResponseCookie workingdep;
            RestResponseCookie loginApps;
            RestResponseCookie LastSessionTime;
            RestResponseCookie NSC2Cookie;
                

            #region Step 1 - Get Cookies
            RestClient sbmTrafficClient = new RestClient();
            sbmTrafficClient.FollowRedirects = false;
            sbmTrafficClient.Proxy = new WebProxy(ProxyHost, ProxyPort);
            sbmTrafficClient.BaseUrl = new Uri(getCookiesRequestUrl);
            RestRequest getCookiesRequest = new RestRequest(Method.GET);
            IRestResponse getCookiesResponse = sbmTrafficClient.Execute(getCookiesRequest);
            JSESSIONIDCookie = (from cookie in getCookiesResponse.Cookies where cookie.Name == "JSESSIONID" select cookie).FirstOrDefault();
            NSCCookie = (from cookie in getCookiesResponse.Cookies where cookie.Name == "NSC_mc_qspe_tcnpomjof_tcn-benjo" select cookie).FirstOrDefault();
            #endregion

            #region Step 2 - Login
            sbmTrafficClient.BaseUrl = new Uri(loginRequestUrl);
            RestRequest loginRequest = new RestRequest(Method.POST);
            loginRequest.AddCookie(JSESSIONIDCookie.Name, JSESSIONIDCookie.Value);
            loginRequest.AddCookie(NSCCookie.Name, NSCCookie.Value);
            loginRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            //loginRequest.AddParameter("username", sbmWebServisKullanici.KullaniciAdi);
            loginRequest.AddParameter("username", "27847413916");
            //loginRequest.AddParameter("password", sbmWebServisKullanici.Sifre);
            loginRequest.AddParameter("password", "MTNKNR01");
            IRestResponse loginResponse = sbmTrafficClient.Execute(loginRequest);

            if (loginResponse.StatusCode != HttpStatusCode.Found) // Login Unsuccessful
            {
                throw new SbmLoginUnsuccessfulException("Login Response Status Code : " + loginResponse.StatusCode);
            }


            JSESSIONIDCookie = (from cookie in loginResponse.Cookies where cookie.Name == "JSESSIONID" select cookie).FirstOrDefault();
            NSCCookie = (from cookie in loginResponse.Cookies where cookie.Name == "NSC_mc_qspe_tcnpomjof_tcn-benjo" select cookie).FirstOrDefault();
            rmbtkn = (from cookie in loginResponse.Cookies where cookie.Name == "rmbtkn" select cookie).FirstOrDefault();
            SbmSessionId = (from cookie in loginResponse.Cookies where cookie.Name == "SbmSessionId" select cookie).FirstOrDefault();
            lang = (from cookie in loginResponse.Cookies where cookie.Name == "lang" select cookie).FirstOrDefault();

            #endregion
                
            #region Step 3 - Get Police Sorgu Page
            sbmTrafficClient.BaseUrl = new Uri(policeSorguPageRequestUrl);
            RestRequest policeSorguPageRequest = new RestRequest(Method.GET);

            policeSorguPageRequest.AddCookie(NSCCookie.Name, NSCCookie.Value);
            policeSorguPageRequest.AddCookie(rmbtkn.Name, rmbtkn.Value);
            policeSorguPageRequest.AddCookie(SbmSessionId.Name, SbmSessionId.Value);
            policeSorguPageRequest.AddCookie(lang.Name, lang.Value);

            IRestResponse policeSorguPageResponse = sbmTrafficClient.Execute(policeSorguPageRequest);

            JSESSIONIDCookie = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "JSESSIONID" select cookie).FirstOrDefault();
            NSCCookie = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "NSC_mc_qspe_tcnpomjof_tcn-benjo" select cookie).FirstOrDefault();
            rmbtkn = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "rmbtkn" select cookie).FirstOrDefault();
            SbmSessionId = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "SbmSessionId" select cookie).FirstOrDefault();
            lang = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "lang" select cookie).FirstOrDefault();
            workingdep = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "workingdep" select cookie).FirstOrDefault();
            loginApps = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "loginApps" select cookie).FirstOrDefault();
            LastSessionTime = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "LastSessionTime" select cookie).FirstOrDefault();
            NSC2Cookie = (from cookie in policeSorguPageResponse.Cookies where cookie.Name == "NSC_mc_qspe_tcnpomjof_usn-qpmjdf" select cookie).FirstOrDefault();

            #endregion

            #region Step 4 - Query Form
            sbmTrafficClient.BaseUrl = new Uri(queryFormRequestUrl);
            RestRequest queryFormRequest = new RestRequest(Method.POST);
            queryFormRequest.AddHeader("content-type", "application/x-www-form-urlencoded");
            queryFormRequest.AddParameter("kimlikTipiForPlaka", "1");
            queryFormRequest.AddParameter("kimlikNoForPlaka", kimlikNo);
            queryFormRequest.AddParameter("selectedSection", "24");
            queryFormRequest.AddParameter("plakaIlKoduForKimlik", plakaIlKodu.ToString("D3"));
            queryFormRequest.AddParameter("plakaNoForKimlik", plakaNo);

            queryFormRequest.AddCookie(JSESSIONIDCookie.Name, JSESSIONIDCookie.Value);
            //queryFormRequest.AddCookie(NSCCookie.Name, NSCCookie.Value);
            //queryFormRequest.AddCookie(rmbtkn.Name, rmbtkn.Value);
            //queryFormRequest.AddCookie(SbmSessionId.Name, SbmSessionId.Value);
            //queryFormRequest.AddCookie(lang.Name, lang.Value);
            queryFormRequest.AddCookie(workingdep.Name, workingdep.Value);
            queryFormRequest.AddCookie(loginApps.Name, loginApps.Value);
            queryFormRequest.AddCookie(LastSessionTime.Name, LastSessionTime.Value);
            queryFormRequest.AddCookie(NSC2Cookie.Name, NSC2Cookie.Value);

            IRestResponse queryFormResponse = sbmTrafficClient.Execute(queryFormRequest);

            HtmlNode.ElementsFlags["form"] = HtmlElementFlag.Closed;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(queryFormResponse.Content);
            var trafikPoliceTable = doc.GetElementbyId("trafikPolice");
            try
            {

                HtmlNode firstPolicy = doc.DocumentNode.SelectNodes("//a")[88];
                string hrefValue = (from attribute in firstPolicy.Attributes where attribute.Name == "href" select attribute).FirstOrDefault().Value;
                firstPolicyDetailsRequestUrl = "https://online.sbm.org.tr" + hrefValue;
            }
            catch (Exception)
            {
                return null;
            }

            #endregion

            #region Step 5 - See Policy Details
            sbmTrafficClient.BaseUrl = new Uri(firstPolicyDetailsRequestUrl);
            RestRequest policyDetailsRequest = new RestRequest(Method.GET);
            policyDetailsRequest.AddCookie(workingdep.Name, workingdep.Value);
            policyDetailsRequest.AddCookie(loginApps.Name, loginApps.Value);
            policyDetailsRequest.AddCookie(LastSessionTime.Name, LastSessionTime.Value);
            policyDetailsRequest.AddCookie(NSC2Cookie.Name, NSC2Cookie.Value);
            policyDetailsRequest.AddCookie(JSESSIONIDCookie.Name, JSESSIONIDCookie.Value);
            IRestResponse policyDetailsResponse = sbmTrafficClient.Execute(policyDetailsRequest);

            HtmlDocument policyDetailDoc = new HtmlDocument();
            policyDetailDoc.LoadHtml(policyDetailsResponse.Content);
            HtmlNodeCollection policyDetailNodes = policyDetailDoc.DocumentNode.SelectNodes("//div[contains(@class, 'field field--output')]");
            SBMTrafficPolicyDetail lastTrafficPolicyDetail = mapDivValues(policyDetailNodes);

            #endregion

            return lastTrafficPolicyDetail;
        }

        private string GetValueOfDivByLabelInnerHtml(string labelInnerHtml, HtmlNodeCollection nodes)
        {
            string value = null;

            HtmlNode labelParentNode = null;
            foreach(HtmlNode node in nodes)
            {
                labelParentNode = (from childNode in node.ChildNodes where childNode.InnerHtml == labelInnerHtml select childNode.ParentNode).FirstOrDefault() ?? labelParentNode;
            }
            if (labelParentNode != null)
            {
                value = (from childNode in labelParentNode.ChildNodes where childNode.Name == "div" select childNode.InnerText).FirstOrDefault().Trim();
            }

            return value;
        }

        private SBMTrafficPolicyDetail mapDivValues(HtmlNodeCollection policyDetailNodes)
        {
            SBMTrafficPolicyDetail trafficPolicyDetail = new SBMTrafficPolicyDetail();
            #region Poliçe Bilgileri
            trafficPolicyDetail.SbmTramerNo = GetValueOfDivByLabelInnerHtml("Sbm Tramer No:", policyDetailNodes);
            trafficPolicyDetail.AcenteNo = GetValueOfDivByLabelInnerHtml("Acente No:", policyDetailNodes);
            trafficPolicyDetail.YenilemeNo = GetValueOfDivByLabelInnerHtml("Yenileme No:", policyDetailNodes);
            trafficPolicyDetail.EkTuru = GetValueOfDivByLabelInnerHtml("Ek Türü:", policyDetailNodes);
            trafficPolicyDetail.TCKimlikNo = GetValueOfDivByLabelInnerHtml("TC Kimlik No:", policyDetailNodes);
            trafficPolicyDetail.PasaportNo = GetValueOfDivByLabelInnerHtml("Pasaport No:", policyDetailNodes);
            trafficPolicyDetail.GuncellemeTarihi = GetValueOfDivByLabelInnerHtml("Güncelleme Tarihi:", policyDetailNodes);
            trafficPolicyDetail.BitisTarihi = GetValueOfDivByLabelInnerHtml("Bitiş Tarihi:", policyDetailNodes);
            trafficPolicyDetail.EkBaslangicTarihi = GetValueOfDivByLabelInnerHtml("Ek Başlangıç Tarihi:", policyDetailNodes);
            trafficPolicyDetail.SistemTarihi = GetValueOfDivByLabelInnerHtml("Sistem Tarihi:", policyDetailNodes);
            trafficPolicyDetail.TahakkukIptal = GetValueOfDivByLabelInnerHtml("Tahakkuk/İptal:", policyDetailNodes);

            trafficPolicyDetail.SigortaSirketi = GetValueOfDivByLabelInnerHtml("Sigorta Şirketi:", policyDetailNodes);
            trafficPolicyDetail.PoliceNo = GetValueOfDivByLabelInnerHtml("Poliçe No:", policyDetailNodes);
            trafficPolicyDetail.PoliceEkNo = GetValueOfDivByLabelInnerHtml("Poliçe Ek No:", policyDetailNodes);
            trafficPolicyDetail.Sigortali = GetValueOfDivByLabelInnerHtml("Sigortalı:", policyDetailNodes);
            trafficPolicyDetail.VergiKimlikNo = GetValueOfDivByLabelInnerHtml("Vergi Kimlik No:", policyDetailNodes);
            trafficPolicyDetail.OlusturmaTarihi = GetValueOfDivByLabelInnerHtml("Oluşturma Tarihi:", policyDetailNodes);
            trafficPolicyDetail.BaslangicTarihi = GetValueOfDivByLabelInnerHtml("Başlangıç Tarihi:", policyDetailNodes);
            trafficPolicyDetail.TanzimTarihi = GetValueOfDivByLabelInnerHtml("Tanzim Tarihi:", policyDetailNodes);
            trafficPolicyDetail.EkBitisTarihi = GetValueOfDivByLabelInnerHtml("Ek BitişTarihi:", policyDetailNodes);
            trafficPolicyDetail.SistemSaati = GetValueOfDivByLabelInnerHtml("Sistem Saati:", policyDetailNodes);
            trafficPolicyDetail.HavuzaDahil = GetValueOfDivByLabelInnerHtml("Havuza Dahil:", policyDetailNodes);
            #endregion

            #region Sigortalı Bilgileri
            trafficPolicyDetail.Adres = GetValueOfDivByLabelInnerHtml("Adres:", policyDetailNodes);
            trafficPolicyDetail.IkametIlce = GetValueOfDivByLabelInnerHtml("İkamet İlçe:", policyDetailNodes);
            trafficPolicyDetail.IkametIl = GetValueOfDivByLabelInnerHtml("İkamet İl:", policyDetailNodes);
            #endregion

            #region Önceki Poliçe Bilgileri
            trafficPolicyDetail.OncekiSigortaSirketi = GetValueOfDivByLabelInnerHtml("Önceki Sigorta Şirketi:", policyDetailNodes);
            trafficPolicyDetail.OncekiPoliceNo = GetValueOfDivByLabelInnerHtml("Önceki Poliçe No:", policyDetailNodes);
            trafficPolicyDetail.OncekiAcenteNo = GetValueOfDivByLabelInnerHtml("Önceki Acente No:", policyDetailNodes);
            trafficPolicyDetail.OncekiYenilemeNo = GetValueOfDivByLabelInnerHtml("Önceki Yenileme No:", policyDetailNodes);
            #endregion

            #region Araç Bilgileri
            trafficPolicyDetail.AracTarifeGrupKodu = GetValueOfDivByLabelInnerHtml("Araç Tarife Grup Kodu:", policyDetailNodes);
            trafficPolicyDetail.SasiNo = GetValueOfDivByLabelInnerHtml("Şasi No:", policyDetailNodes);
            trafficPolicyDetail.ModelYili = GetValueOfDivByLabelInnerHtml("Model Yılı:", policyDetailNodes);
            trafficPolicyDetail.AracTipi = GetValueOfDivByLabelInnerHtml("Araç Tipi:", policyDetailNodes);
            trafficPolicyDetail.YolcuSayisi = GetValueOfDivByLabelInnerHtml("Yolcu Sayısı:", policyDetailNodes);
            trafficPolicyDetail.ImalatYeri = GetValueOfDivByLabelInnerHtml("İmalat Yeri:", policyDetailNodes);
            trafficPolicyDetail.SilindirHacmi = GetValueOfDivByLabelInnerHtml("Silindir Hacmi:", policyDetailNodes);
            trafficPolicyDetail.YukKapasitesi = GetValueOfDivByLabelInnerHtml("Yük Kapasitesi:", policyDetailNodes);
            trafficPolicyDetail.AyaktaYolcuAdedi = GetValueOfDivByLabelInnerHtml("Ayakta Yolcu Adedi:", policyDetailNodes);
            trafficPolicyDetail.Plaka = GetValueOfDivByLabelInnerHtml("Plaka:", policyDetailNodes);
            trafficPolicyDetail.MotorNo = GetValueOfDivByLabelInnerHtml("Motor No:", policyDetailNodes);
            trafficPolicyDetail.AracMarkasi = GetValueOfDivByLabelInnerHtml("Araç Markası:", policyDetailNodes);
            trafficPolicyDetail.TescilTarihi = GetValueOfDivByLabelInnerHtml("Tescil Tarihi:", policyDetailNodes);
            trafficPolicyDetail.KullanimSekli = GetValueOfDivByLabelInnerHtml("Kullanım Şekli:", policyDetailNodes);
            trafficPolicyDetail.Renk = GetValueOfDivByLabelInnerHtml("Renk:", policyDetailNodes);
            trafficPolicyDetail.MotorGucu = GetValueOfDivByLabelInnerHtml("Motor Gücü:", policyDetailNodes);
            trafficPolicyDetail.TrafigeCikisTarihi = GetValueOfDivByLabelInnerHtml("Trafiğe Çıkış Tarihi:", policyDetailNodes);
            #endregion

            #region Teminat ve Prim Bilgileri
            trafficPolicyDetail.AracBasinaMaddiTeminat = GetValueOfDivByLabelInnerHtml("Araç Başına Maddi Teminat:", policyDetailNodes);
            trafficPolicyDetail.KisiBasinaTedaviTeminati = GetValueOfDivByLabelInnerHtml("Kişi Başına Tedavi Teminatı:", policyDetailNodes);
            trafficPolicyDetail.KisiBasinaOlumSakatlikTeminati = GetValueOfDivByLabelInnerHtml("Kişi Başına ÖlümSakatlık Teminatı:", policyDetailNodes);
            trafficPolicyDetail.TemelTarifePrimi = GetValueOfDivByLabelInnerHtml("Temel Tarife Primi:", policyDetailNodes);
            trafficPolicyDetail.GiderVergisi = GetValueOfDivByLabelInnerHtml("Gider Vergisi:", policyDetailNodes);
            trafficPolicyDetail.TrafikHizmetleriGelistirmeFonu = GetValueOfDivByLabelInnerHtml("Trafik Hizmetleri Geliştirme Fonu:", policyDetailNodes);
            trafficPolicyDetail.HavuzPrimi = GetValueOfDivByLabelInnerHtml("Havuz Primi:", policyDetailNodes);
            //trafficPolicyDetail.KazaBasinaMaddiTeminat = GetValueOfDivByLabelInnerHtml("Kaza Başına Maddi Teminat:", policyDetailNodes);
            trafficPolicyDetail.KazaBasinaTedaviTeminati = GetValueOfDivByLabelInnerHtml("Kaza Başına Tedavi Teminatı:", policyDetailNodes);
            trafficPolicyDetail.KazaBasinaOlumSakatlikTeminati = GetValueOfDivByLabelInnerHtml("Kaza Başına ÖlümSakatlık Teminatı:", policyDetailNodes);
            trafficPolicyDetail.NetPrim = GetValueOfDivByLabelInnerHtml("Net Prim:", policyDetailNodes);
            trafficPolicyDetail.GarantiFon = GetValueOfDivByLabelInnerHtml("Garanti Fon:", policyDetailNodes);
            trafficPolicyDetail.BrutPrim = GetValueOfDivByLabelInnerHtml("Brüt Prim:", policyDetailNodes);
            #endregion

            #region İndirim/Sürprim Bilgileri
            trafficPolicyDetail.HasarsizlikIndirimi = GetValueOfDivByLabelInnerHtml("Hasarsizlik İndirim (%):", policyDetailNodes);
            trafficPolicyDetail.HasarlılıkSurprim = GetValueOfDivByLabelInnerHtml("Hasarlılık Sürprim (%):", policyDetailNodes);
            trafficPolicyDetail.TarifeBasamakKodu = GetValueOfDivByLabelInnerHtml("Tarife Basamak Kodu:", policyDetailNodes);
            trafficPolicyDetail.GecikmedenDolayiSurprim = GetValueOfDivByLabelInnerHtml("Gecikmeden Dolayı Sürprim (%):", policyDetailNodes);
            trafficPolicyDetail.GecikmedenDolayiSurprim = GetValueOfDivByLabelInnerHtml("Gecikmeden Dolayı Sürprim (%):", policyDetailNodes);
            trafficPolicyDetail.ZKYTMSIndirim = GetValueOfDivByLabelInnerHtml("ZKYTMS İndirim (%):", policyDetailNodes);
            #endregion

            #region tramer 
            trafficPolicyDetail.TramerBelgeNo = GetValueOfDivByLabelInnerHtml("Tramer Belge No:", policyDetailNodes);
            trafficPolicyDetail.TramerBelgeTarihi = GetValueOfDivByLabelInnerHtml("Tramer Belge Tarihi:", policyDetailNodes);
            #endregion

            return trafficPolicyDetail;
        }

    }

    public class SbmLoginUnsuccessfulException : Exception
    {
        public SbmLoginUnsuccessfulException(string message)
           : base(message)
        {
        }
    }
}
