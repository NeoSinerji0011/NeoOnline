
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public class Konfig
    {
        //E-Posta config
        public const string EMailFromAddress = "EMailFromAddress";
        public const string EMailFromDisplayName = "EMailFromDisplayName";
        public const string EMailSMTPAddress = "EMailSMTPAddress";
        public const string EMailSMTPPort = "EMailSMTPPort";
        public const string EMailSMTPUserName = "EMailSMTPUserName";
        public const string EMailSMTPPassword = "EMailSMTPPassword";
        public const string EMailEnableSSL = "EMailEnableSSL";

        //E-Posta config
        public const string LilyumEMailFromAddress = "LilyumEMailFromAddress";
        public const string LilyumEMailFromDisplayName = "LilyumEMailFromDisplayName";
        public const string LilyumEMailSMTPAddress = "LilyumEMailSMTPAddress";
        public const string LilyumEMailSMTPPort = "LilyumEMailSMTPPort";
        public const string LilyumEMailSMTPUserName = "LilyumEMailSMTPUserName";
        public const string LilyumEMailSMTPPassword = "LilyumEMailSMTPPassword";
        public const string LilyumEMailEnableSSL = "LilyumEMailEnableSSL";

        //Aegon E-Posta config
        public const string AegonFromAddress = "AegonFromAddress";
        public const string AegonFromDisplayName = "AegonFromDisplayName";
        public const string AegonSMTPAddress = "AegonSMTPAddress";
        public const string AegonSMTPPort = "AegonSMTPPort";
        public const string AegonSMTPUserName = "AegonSMTPUserName";
        public const string AegonSMTPPassword = "AegonSMTPPassword";
        public const string AegonEnableSSL = "AegonEnableSSL";

        //HDI Config
        public const string HDI_UygulamaPlakaSor = "HDI_UygulamaPlakaSor"; //Trafik
        public const string HDI_UygulamaPlakaSorKasko = "HDI_UygulamaPlakaSorKasko";
        public const string HDI_UygulamaTrafik = "HDI_UygulamaTrafik";
        public const string HDI_UygulamaKasko = "HDI_UygulamaKasko";
        public const string HDI_UygulamaDask = "HDI_UygulamaDask";
        public const string HDI_UygulamaKonut = "HDI_UygulamaKonut";
        public const string HDI_UygulamaIsYeri = "HDI_UygulamaIsYeri";
        public const string HDI_UygulamaDekont = "HDI_UygulamaDekont";
        public const string HDI_ServiceURL = "HDI_ServiceURL";
        public const string HDI_PDFServiceURL = "HDI_PDFServiceURL";
        public const string HDI_DaskServiceURL = "HDI_DaskServiceURL";
        public const string HDI_UAVTServiceURL = "HDI_UAVTServiceURL";
        public const string HDI_PoliceTransferServiceURL = "HDI_PoliceTransferServiceURL";
        public const string HDI_TahsilatKapatmaServiceURL = "HDI_TahsilatKapatmaServiceURL";

        //koru lilyum ferdi kaza 
        public const string KoruLilyum_ServiceURL = "KoruLilyum_ServiceURL";


        //MAPFRE Config
        public const string MAPFRE_ServiceURL = "MAPFRE_ServiceURL";
        public const string MAPFRE_ServiceURL_Bab = "MAPFRE_ServiceURL_Bab";
        public const string MAPFRE_UtilURL = "MAPFRE_UtilURL";
        public const string MAPFRE_KaskoTeklifTimeOut = "MAPFRE_KaskoTeklifTimeOut";
        public const string MAPFRE_KaskoPoliceTimeOut = "MAPFRE_KaskoPoliceTimeOut";
        public const string MAPFRE_TrafikTeklifTimeOut = "MAPFRE_TrafikTeklifTimeOut";
        public const string MAPFRE_TrafikPoliceTimeOut = "MAPFRE_TrafikPoliceTimeOut";
        public const string MAPFRE_PoliceTransferURL = "MAPFRE_PoliceTransferURL";
        public const string MAPFRE_TahsilatPoliceTransferURL = "MAPFRE_TahsilatPoliceTransferURL";
        public const string MAPFREDASK_OtomatikPoliceTransferURL = "MAPFREDASK_OtomatikPoliceTransferURL";


        //ANADOULU Config
        public const string ANADOLU_ServiceUrl = "ANADOLU_ServiceUrl";

        //AEGON Config
        public const string AEGON_ServiceURL = "AEGON_ServiceURL";

        //Ethica Config
        public const string ETHICA_ServiceURL = "ETHICA_ServiceURL";

        //Paritus
        public const string Paritus_ApiKey = "Paritus_ApiKey";
        public const string Paritus_ServiceURL = "Paritus_ServiceURL";

        //Muhasebe
        public const string MuhasebeURL = "MuhasebeURL";

        //RAY Config
        public const string RAY_ServiceURL = "RAY_ServiceURL";
        public const string RAY_UserName = "RAY_UserName";
        public const string RAY_Password = "RAY_Password";


        //SOMPO JAPAN

        public const string SOMPOJAPAN_TrafikServiceURL = "SOMPOJAPAN_ServiceURL";
        public const string SOMPOJAPAN_UserName = "SOMPOJAPAN_UserName";
        public const string SOMPOJAPAN_Password = "SOMPOJAPAN_Password";
        public const string SOMPOJAPAN_CascoServiceURL = "SOMPOJAPAN_CascoServiceURL";

        //SOMPO JAPAN V2
        public const string SOMPOJAPAN_CascoServiceURLV2 = "SOMPOJAPAN_CascoServiceURLV2";
        public const string SOMPOJAPAN_TrafikServiceURLV2 = "SOMPOJAPAN_TrafikServiceURLV2";
        public const string SOMPOJAPAN_CommonServiceURLV2 = "SOMPOJAPAN_CommonServiceURLV2";
       
        //AK

        public const string AK_TrafikServiceURL = "AK_ServiceURL";
        public const string AK_CascoServiceURL = "AK_ServiceURL";
        public const string AK_UserName = "AK_UserName";
        public const string AK_Password = "AK_Password";

        // public const string AK_CascoServiceURL = AK_CascoServiceURL";

        //AK V2
        public const string AK_CascoServiceURLV2 = "AK_CascoServiceURLV2";
        public const string AK_TrafikServiceURLV2 = "AK_TrafikServiceURLV2";
        public const string AK_CommonServiceURLV2 = "AK_CommonServiceURLV2";
        public const string AK_PoliceBasim = "AK_PoliceBasim";


        //TURK NIPPON 
        public const string TURKNIPPON_KaskoServiceURL = "TURKNIPPON_ServiceURL";
        public const string TURKNIPPON_UserName = "TURKNIPPON_UserName";
        public const string TURKNIPPON_Password = "TURKNIPPON_Password";
        public const string TURKNIPPON_TSS_ServisURL = "TURKNIPPON_TSS_ServisURL";
        public const string TURKNIPPON_DASK_ServisURL = "TURKNIPPON_DASK_ServisURL";
        public const string TURKNIPPON_SEYAHAT_ServisURL = "TURKNIPON_Seyahat_ServiceURL";
        public const string TURKNIPPON_SeyahatPassword = "TURKNIPPON_SeyahatPassword";
        public const string TURKNIPPON_SeyahatUser = "TURKNIPPON_SeyahatUser";

        //EUREKO
        public const string EUREKO_KaskoServiceURL = "EUREKO_KaskoServiceURL";
        public const string EUREKO_PlatformType = "EUREKO_PlatformType";
        public const string EUREKO_MusteriServisURL = "EUREKO_MusteriServisURL";
        public const string EUREKO_PoliceTeklifBilgiServiceURL = "EUREKO_PoliceTeklifBilgiServiceURL";
        public const string EUREKO_PoliceTeminatServiceURL = "EUREKO_PoliceTeminatServiceURL";
        public const string EUREKO_PolicePDFServiceURL = "EUREKO_PolicePDFServiceURL";
        public const string EUREKO_TrafikServiceURL = "EUREKO_TrafikServiceURL";
        public const string EUREKO_MusteriServisURLV2 = "EUREKO_MusteriServisURLV2";

        //AXA
        public const string AXA_ServiceURL = "AXA_ServiceURL";
        public const string AXA_USERNAME = "AXA_USERNAME";
        public const string AXA_PASSWORD = "AXA_PASSWORD";
        public const string AXA_PoliceBasim = "AXA_PoliceBasim";
        public const string AXA_Oto_Pol_Transfer = "AXA_Oto_Pol_Transfer";
        public const string AxaOtoPolHayatTransfer = "AxaOtoPolHayatTransfer";

       

        //ERGO
        public const string ERGO_KaskoServiceURL = "ERGO_KaskoServiceURL";
        public const string ERGO_InfoServiceURL = "ERGO_InfoServiceURL";

        //groupama ws 7
        public const string GROUPAMA_ServiceURL = "GROUPAMA_ServiceURLV7";
        //AEGON SSO LOGIN
        public const string AEGON_SSO_LOGIN_AppId = "AEGON_SSO_LOGIN_AppId";
        public const string AEGON_SSO_LOGIN_Secret = "AEGON_SSO_LOGIN_Secret";
        public const string AEGON_SSO_LOGIN_Login = "AEGON_SSO_LOGIN_Login";
        public const string AEGON_SSO_LOGIN_LogOff = "AEGON_SSO_LOGIN_LogOff";

        //Gulf Sigorta
        public const string GULF_KaskoServisURL = "GULF_KaskoServisURL";
        public const string GULF_MusteriServisURL = "GULF_MusteriServisURL";
        public const string GULF_PlakaSorguServisURL = "GULF_PlakaSorguServisURL";

        //Doğa sigorta poliçe transfer WS i
        public const string Doga_PolTransferServiceURL = "Doga_PolTransferServiceURL";
        //PLAKA SORGULAMA
        public const string TestMi = "TestMi";


        //Unico Servis Adresleri
        public const string Unico_PolicyServiceURL = "Unico_PolicyServiceURL";
        public const string Unico_UtilityServiceURL = "Unico_UtilityServiceURL";
        public const string Unico_SecurityServiceURL = "Unico_SecurityServiceURL";

        //Palaka Sorgulama
        public static string[] BundleTestMi = new string[]
        {
            Konfig.TestMi,
        };
        //Palaka Sorgulama
        public static string[] BundleGulf = new string[]
        {
            Konfig.GULF_KaskoServisURL,
            Konfig.GULF_MusteriServisURL,
            Konfig.GULF_PlakaSorguServisURL,
        };

        //E-Posta Konfigurasyon
        public static string[] BundleEMail = new string[]
        {
            Konfig.EMailFromAddress,
            Konfig.EMailFromDisplayName,
            Konfig.EMailSMTPAddress,
            Konfig.EMailSMTPPort,
            Konfig.EMailSMTPUserName,
            Konfig.EMailSMTPPassword,
            Konfig.EMailEnableSSL
        };
        //E-Posta Konfigurasyon
        public static string[] BundleLilyumEMail = new string[]
        {
            Konfig.LilyumEMailFromAddress,
            Konfig.LilyumEMailFromDisplayName,
            Konfig.LilyumEMailSMTPAddress,
            Konfig.LilyumEMailSMTPPort,
            Konfig.LilyumEMailSMTPUserName,
            Konfig.LilyumEMailSMTPPassword,
            Konfig.LilyumEMailEnableSSL
        };

        //E-Posta Konfigurasyon
        public static string[] BundleAegonEMail = new string[]
        {
            Konfig.AegonFromAddress,
            Konfig.AegonFromDisplayName,
            Konfig.AegonSMTPAddress,
            Konfig.AegonSMTPPort,
            Konfig.AegonSMTPUserName,
            Konfig.AegonSMTPPassword,
            Konfig.AegonEnableSSL
        };

        /// <summary>

        //koru lilyum ferdi kaza
        public static string[] BundleKoruLilyumFerdiKaza = new string[]
   {
            Konfig.KoruLilyum_ServiceURL
   };


        /// HDI Plaka sorgulama
        /// </summary>
        public static string[] BundleHDIPlaka = new string[]
        {
            Konfig.HDI_ServiceURL,
            Konfig.HDI_PoliceTransferServiceURL,
            Konfig.HDI_PDFServiceURL,
            Konfig.HDI_UygulamaPlakaSor,
            Konfig.HDI_UygulamaPlakaSorKasko,
            Konfig.HDI_TahsilatKapatmaServiceURL
        };


        public static string[] BundleHDITrafik = new string[]
        {
            Konfig.HDI_ServiceURL,
            Konfig.HDI_UygulamaTrafik
        };

        public static string[] BundleHDIKasko = new string[]
        {
            Konfig.HDI_ServiceURL,
            Konfig.HDI_UygulamaKasko
        };

        public static string[] BundleHDIDask = new string[]
        {
            Konfig.HDI_ServiceURL,
            Konfig.HDI_UygulamaDask
        };

        public static string[] BundleHDIKonut = new string[]
        {
            Konfig.HDI_ServiceURL,
            Konfig.HDI_UygulamaKonut
        };

        public static string[] BundleHDIIsYeri = new string[]
        {
            Konfig.HDI_ServiceURL,
            Konfig.HDI_UygulamaIsYeri
        };

        /// <summary>
        /// MAPFRE
        /// </summary>
        public static string[] BundleMAPFRETrafik = new string[]
        {
            Konfig.MAPFRE_ServiceURL,
            Konfig.MAPFRE_ServiceURL_Bab,
            Konfig.MAPFRE_UtilURL,
            Konfig.MAPFRE_TrafikTeklifTimeOut,
            Konfig.MAPFRE_TrafikPoliceTimeOut,
            Konfig.MAPFRE_PoliceTransferURL,
            Konfig.MAPFRE_TahsilatPoliceTransferURL,
            Konfig.MAPFREDASK_OtomatikPoliceTransferURL
        };

        /// <summary>
        /// MAPFRE
        /// </summary>
        public static string[] BundleMAPFREKasko = new string[]
        {
            Konfig.MAPFRE_ServiceURL,
            Konfig.MAPFRE_ServiceURL_Bab,
            Konfig.MAPFRE_KaskoTeklifTimeOut,
            Konfig.MAPFRE_KaskoPoliceTimeOut
        };

        /// <summary>
        /// ANADOLU
        /// </summary>
        public static string[] BundleANADOLUTrafik = new string[]
        {
            Konfig.ANADOLU_ServiceUrl
        };

        /// <summary>
        /// ANADOLU
        /// </summary>
        public static string[] BundleANADOLUKasko = new string[]
        {
            Konfig.ANADOLU_ServiceUrl
        };


        /// <summary>
        /// RAY
        /// </summary>
        public static string[] BundleRAYService = new string[]
        {
            Konfig.RAY_ServiceURL
           
           
        };

        /// <summary>
        /// RAY
        /// </summary>
        public static string[] RAYKullaniciBilgileri = new string[]
        {
             Konfig.RAY_UserName,
            Konfig.RAY_Password
           
        };

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleSOMPOJAPANTrafik = new string[]
        {
            Konfig.SOMPOJAPAN_TrafikServiceURL          
           
        };

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleSOMPOJAPANKasko = new string[]
        {
            Konfig.SOMPOJAPAN_CascoServiceURL
        };

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleSOMPOJAPANKaskoV2 = new string[]
        {
            Konfig.SOMPOJAPAN_CascoServiceURLV2
        };

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleSOMPOJAPANTrafikV2 = new string[]
        {
            Konfig.SOMPOJAPAN_TrafikServiceURLV2
        };

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleSOMPOJAPANCommonV2 = new string[]
        {
            Konfig.SOMPOJAPAN_CommonServiceURLV2
        };


        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] SOMPOJAPANKullaniciBilgileri = new string[]
        {
             Konfig.SOMPOJAPAN_UserName,
            Konfig.SOMPOJAPAN_Password
           
        };





       

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleAKKasko = new string[]
        {
            Konfig.AK_CascoServiceURL
        };

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleAKKaskoV2 = new string[]
        {
            Konfig.AK_CascoServiceURLV2
        };

       

        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] BundleAKCommonV2 = new string[]
        {
            Konfig.AK_CommonServiceURLV2
        };


        /// <summary>
        /// SOMPOJAPAN
        /// </summary>
        public static string[] AKKullaniciBilgileri = new string[]
        {
             Konfig.AK_UserName,
            Konfig.AK_Password

        };






        /// <summary>
        /// TURK NIPPON
        /// </summary>
        public static string[] BundleTURKNIPPONKasko = new string[]
        {
            Konfig.TURKNIPPON_KaskoServiceURL
        };
        public static string[] BundleTURKNIPPONDASK = new string[]
        {
            Konfig.TURKNIPPON_DASK_ServisURL
        };
          public static string[] BundleTURKNIPPONTSS = new string[]
        {
            Konfig.TURKNIPPON_TSS_ServisURL
        };
        public static string[] BundleTURKNIPPONSEYAHAT = new string[]
       {
            Konfig.TURKNIPPON_SEYAHAT_ServisURL
       };


        /// <summary>
        /// TURK NIPPON
        /// </summary>
        public static string[] TURKNIPPONKullaniciBilgileri = new string[]
        {
             Konfig.TURKNIPPON_UserName,
            Konfig.TURKNIPPON_Password
           
        };


        /// <summary>
        /// AEGON SSO LOGIN
        /// </summary>
        public static string[] BundleAEGON_SSO_LOGIN = new string[]
        {
            Konfig.AEGON_SSO_LOGIN_AppId,
            Konfig.AEGON_SSO_LOGIN_Secret
        };


        /// <summary>
        /// RAY
        /// </summary>
        public static string[] BundleEurekoKasko = new string[]
        {
            Konfig.EUREKO_KaskoServiceURL
           
           
        };

        /// <summary>
        /// EUREKO
        /// </summary>
        public static string[] BundleEUREKOPlatformType = new string[]
        {
            Konfig.EUREKO_PlatformType
           
        };

        public static string[] BundleEUREKOPDF = new string[]
        {
            Konfig.EUREKO_PolicePDFServiceURL
           
        };


        /// <summary>
        /// EUREKO
        /// </summary>
        public static string[] BundleEurekoTrafik = new string[]
        {
            Konfig.EUREKO_TrafikServiceURL
           
           
        };

        /// <summary>
        /// EUREKO
        /// </summary>
        /// 
        public static string[] BundleEUREKOMusteri = new string[]
        {
            Konfig.EUREKO_MusteriServisURL
           
        };
        /// <summary>
        /// EUREKO
        /// </summary>
        public static string[] BundleEUREKOTeklifPoliceBilgi = new string[]
        {
            Konfig.EUREKO_PoliceTeklifBilgiServiceURL
           
        };

        /// <summary>
        /// EUREKO
        /// </summary>
        public static string[] BundleEUREKOPolieTeminat = new string[]
        {
            Konfig.EUREKO_PoliceTeminatServiceURL
           
        };

        public static string[] BundleEUREKOMusteriV2 = new string[]
        {
            Konfig.EUREKO_MusteriServisURLV2
           
        };

        /// <summary>
        /// AXA
        /// </summary>
        public static string[] BundleAXAService= new string[]
        {
            Konfig.AXA_ServiceURL,
            Konfig.AXA_USERNAME,
            Konfig.AXA_PASSWORD,
            Konfig.AXA_PoliceBasim,
            Konfig.AXA_Oto_Pol_Transfer,
            Konfig.AxaOtoPolHayatTransfer

        };
        /// <summary>
        /// ERGO
        /// </summary>
        public static string[] BundleERGOService = new string[]
        {
            Konfig.ERGO_KaskoServiceURL,
           Konfig.ERGO_InfoServiceURL
        };

        /// <summary>
        /// GROUPAMA
        /// </summary>
        public static string[] BundleGROUPAMAService = new string[]
        {
            Konfig.GROUPAMA_ServiceURL
        };
        public static string[] BundleDogaPolTransfer = new string[]
        {
            Konfig.Doga_PolTransferServiceURL

        };
        public static string[] BundleEthicaPolTransfer = new string[]
       {
            Konfig.ETHICA_ServiceURL

       };
        public static string[] BundleUnico = new string[]
       {
            Konfig.Unico_PolicyServiceURL,
            Konfig.Unico_SecurityServiceURL,
            Konfig.Unico_UtilityServiceURL

       };
    }
}
