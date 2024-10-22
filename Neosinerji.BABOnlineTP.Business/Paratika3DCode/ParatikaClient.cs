using log4net;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Neosinerji.BABOnlineTP.Business.Paratika3DCode
{
    public class ParatikaClient
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ParatikaClient));
        private ParatikaUtil paratikaUtil = new ParatikaUtil();
        private ParatikaService paratikaService = new ParatikaService();
        
        public void SETRequestParametre(string brutPrim,string ilAdi,string sigortaliCepTel, string sigortaliEmail, string sigortaliAdi,string sigortaliAcikAdres,string guidId, string returnNeoUrl) {
            ParatikaUtil paratikaUtill = new ParatikaUtil(brutPrim, ilAdi, sigortaliCepTel, sigortaliEmail,sigortaliAdi,sigortaliAcikAdres, guidId, returnNeoUrl);

            paratikaUtil = paratikaUtill;
        }
        /**
        * Sample API sessionToken operation to the Paratika API
        * 
        */
        public JObject sessionToken(out string hataMesaji)
        {
            JObject paratikaResponse = null;
            hataMesaji = "";
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                System.Net.ServicePointManager.Expect100Continue = false;
                Console.WriteLine("A sample SESSIONTOKEN operation is started.");
                long started = DateTime.Now.Ticks;
                Dictionary<String, String> requestParameters = new Dictionary<String, String>();
                paratikaUtil.prepareSessionTokenParameters(requestParameters);
                String requestData = paratikaUtil.convertToRequestData(requestParameters);
                String response = paratikaService.getConnection("url", requestData,out bool hata);
                if (hata)
                {
                    hataMesaji = response;
                }
                paratikaResponse = JObject.Parse(response);
                checkResponse(paratikaResponse, response, "SESSIONTOKEN");
                hataMesaji = checkResponse(paratikaResponse, response, "SESSIONTOKEN");
            }
            catch (Exception e)
            {
                log.Info(" ParatikaClient --> sessionToken() --> Exception " + e.ToString());
                hataMesaji = e.ToString();
            }
            return paratikaResponse;
        }

        public JObject sessionTokenGuid(string guidId)
        {
            JObject paratikaResponse = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                System.Net.ServicePointManager.Expect100Continue = false;
                Console.WriteLine("A sample SESSIONTOKEN operation is started.");
                long started = DateTime.Now.Ticks;           

                Dictionary<String, String> requestParametersGuid = new Dictionary<String, String>();
                paratikaUtil.prepareSessionTokenParametersGuid(requestParametersGuid);
                requestParametersGuid.Add("MERCHANTPAYMENTID", guidId);
                String requestDataGuid = paratikaUtil.convertToRequestData(requestParametersGuid);
                String responseGuid = paratikaService.getConnectionGuid(ConfigurationManager.AppSettings["url"], requestDataGuid);

                paratikaResponse = JObject.Parse(responseGuid);
                checkResponse(paratikaResponse, responseGuid, "SESSIONTOKEN");
            }
            catch (Exception e)
            {
                log.Info(" ParatikaClient --> sessionToken() --> Exception " + e.ToString());
            }
            return paratikaResponse;
        }

        /**
         * Sample API query session operation to the Paratika API in order to see orderItems and extra parameters
         * 
         */
        public JObject querySession()
        {
            JObject paratikaResponse = null;
            try
            {
                String action = "QUERYSESSION";
                string hataMesaji = "";
                JObject doSessionToken = this.sessionToken(out hataMesaji);
                String sessionToken = (String)doSessionToken.GetValue("sessionToken");
                Console.WriteLine("A sample QUERYSESSION operation is started.");
                long started = DateTime.Now.Ticks;
                System.Uri url = new System.Uri("https://vpos.paratika.com.tr/paratika/api/v2");
                Dictionary<String, String> requestParameters = new Dictionary<String, String>();
                requestParameters.Add("ACTION", action);
                requestParameters.Add("SESSIONTOKEN", sessionToken);
                String requestData = paratikaUtil.convertToRequestData(requestParameters);
                String response = paratikaService.getConnection("https://vpos.paratika.com.tr/paratika/api/v2", requestData,out bool hata);
                paratikaResponse = JObject.Parse(response);
                checkResponse(paratikaResponse, response, action);
            }
            catch (Exception e)
            {
                log.Info(" ParatikaClient --> querySession() --> Exception " + e.ToString());
            }
            return paratikaResponse;
        }


        /**
         * Sample API sale operation to the Paratika API
         * 
         */
        public JObject sale()
        {
            JObject paratikaResponse = null;
            
            try
            {
                String action = "SALE";
                Console.WriteLine("A sample SALE operation is started.");
                long started = DateTime.Now.Ticks;
                Dictionary<String, String> requestParameters = new Dictionary<String, String>();
                requestParameters.Add("action", action);
                paratikaUtil.preparePrimaryTrxParameters(requestParameters);
                String requestData = paratikaUtil.convertToRequestData(requestParameters);
                String response = paratikaService.getConnection("https://vpos.paratika.com.tr/paratika/api/v2", requestData,out bool hata);
                paratikaResponse = JObject.Parse(response);
                checkResponse(paratikaResponse, response, action);
            }
            catch (Exception e)
            {
                log.Info(" ParatikaClient --> sale() --> Exception " + e.ToString());
            }
            
            return paratikaResponse;
        }

        /**
         * Sample API preauth operation to the Paratika API
         * 
         */
        public JObject preauth()
        {
            JObject paratikaResponse = null;
            try
            {
                String action = ConfigurationManager.AppSettings["action_preauth"];
                Console.WriteLine("A sample PREAUTH operation is started.");
                long started = DateTime.Now.Ticks;
                System.Uri url = new System.Uri("https://vpos.paratika.com.tr/paratika/api/v2");
                Dictionary<String, String> requestParameters = new Dictionary<String, String>();
                requestParameters.Add("ACTION", action);
                paratikaUtil.preparePrimaryTrxParameters(requestParameters);
                String requestData = paratikaUtil.convertToRequestData(requestParameters);
                String response = paratikaService.getConnection("https://vpos.paratika.com.tr/paratika/api/v2", requestData,out bool hata);
                paratikaResponse = JObject.Parse(response);
                checkResponse(paratikaResponse, response, action);
            }
            catch (Exception e)
            {
                log.Info(" ParatikaClient --> preauth() --> Exception " + e.ToString());
            }
            return paratikaResponse;
        }

        private string checkResponse(JObject json, String response, String action)
        {
            string HataMesaji = "";
            if (response != null
                && !"00".Equals(json.GetValue("responseCode").ToString(), StringComparison.InvariantCultureIgnoreCase)
                && !"Approved".Equals(json.GetValue("responseMsg").ToString(), StringComparison.InvariantCultureIgnoreCase))
            
            {
                HataMesaji = "ErrorCode : " + json.GetValue("errorCode") + "ErrorMessage  : " + json.GetValue("errorMsg");               
            }
            return HataMesaji;
        }
    }
}
