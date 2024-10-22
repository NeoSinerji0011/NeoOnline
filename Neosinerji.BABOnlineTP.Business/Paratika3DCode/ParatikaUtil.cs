using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Neosinerji.BABOnlineTP.Business.Paratika3DCode
{
    public class ParatikaUtil
    {
        private string _brutPrim;
        private string _ilAdi;
        private string _sigortaliCepTel;
        private string _sigortaliEmail;
        private string _sigortaliUnvani;
        private string _guidId;
        private string _returnNeoUrl;
        private string _sigortaliAcikAdres;
        public ParatikaUtil(string brutPrim,string ilAdi, string sigortaliCepTel, string sigortaliEmail,string sigortaliAdi,string sigortaliAcikAdres, string guidid,string returnNeoUrl)
        {
            _brutPrim = brutPrim;
            _ilAdi = ilAdi;
            _sigortaliCepTel = sigortaliCepTel;
            _sigortaliEmail = sigortaliEmail;
            _sigortaliUnvani = sigortaliAdi;
            _guidId = guidid;
            _returnNeoUrl = returnNeoUrl;
            _sigortaliAcikAdres = sigortaliAcikAdres;
        }
        public ParatikaUtil()
        {
        }
        private static readonly ILog log = LogManager.GetLogger(typeof(ParatikaUtil));
        /**
         * This section takes the input fields and converts them to the proper format for an HTTP post request. For example: "A=x&B=y"
         * 
         * @param requestParameters
         *            Request parameters
         * @return URL encoded request data
         * @throws UnsupportedEncodingException
         */

        public String convertToRequestData(Dictionary<String, String> requestParameters)
        {
            StringBuilder requestData = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in requestParameters)
            {

                var key = HttpUtility.UrlEncode(entry.Key);
                var value = HttpUtility.UrlEncode(entry.Value);
                requestData.Append(key + "=" + value + "&");

            }
            return requestData.ToString();
        }

        private String encodeParameter(String parameterValue)
        {
            String encodedValue = System.String.Empty;
            try
            {
                if (parameterValue != null)
                {
                    encodedValue = HttpUtility.UrlEncode(parameterValue);
                }
            }
            catch
            {
                log.Info(" ParatikaUtil --> encodeParameter(parameterValue) --> UnsupportedEncodingException ");

            }
            return encodedValue;
        }

        public void prepareSessionTokenParameters(Dictionary<String, String> requestParameters)
        {
            requestParameters.Add("ACTION", "SESSIONTOKEN");
            requestParameters.Add("MERCHANTPAYMENTID", _guidId);
            requestParameters.Add("AMOUNT", _brutPrim);
            requestParameters.Add("CURRENCY", "TRY");
            requestParameters.Add("SESSIONTYPE", "PAYMENTSESSION");
            requestParameters.Add("RETURNURL", _returnNeoUrl);
            JArray oItems = new JArray();
            JObject item = new JObject();
            item.Add("code", "T00D3AITCC");
            item.Add("name", "Lilyum");
            item.Add("quantity", 1);
            item.Add("description", "Lilyum Kartı");
            item.Add("amount", _brutPrim);
            oItems.Add(item);
            requestParameters.Add("ORDERITEMS", encodeParameter(oItems.ToString()));
            JObject extra = new JObject();
            extra.Add("IntegrationModel", "API");
            extra.Add("AlwaysSaveCard", "true");
            requestParameters.Add("EXTRA", encodeParameter(extra.ToString()));
            addMerchantAuthParams(requestParameters);
            addCardAndCustomerParams(requestParameters);
            addBillToAndShipToParams(requestParameters);

        }

        public void preparePrimaryTrxParameters(Dictionary<String, String> requestParameters)
        {
            Random rsg = new Random();
            double d_amount = rsg.NextDouble();
            String amount = String.Format("{0:0.00}", d_amount);
            requestParameters.Add("AMOUNT", "1" + amount);
            requestParameters.Add("CURRENCY", "TRY");
            requestParameters.Add("MERCHANTPAYMENTID", Guid.NewGuid().ToString());
            addMerchantAuthParams(requestParameters);
            addCardAndCustomerParams(requestParameters);
            addBillToAndShipToParams(requestParameters);
        }

        private void addMerchantAuthParams(Dictionary<String, String> requestParameters)
        {
            requestParameters.Add("MERCHANT", "10001864");
            requestParameters.Add("MERCHANTUSER", "lilyumkart@lilyumkart.com");
            requestParameters.Add("MERCHANTPASSWORD", "Neo1234?");
        }
        private void addMerchantAuthParamsGuid(Dictionary<String, String> requestParameters)
        {
            requestParameters.Add("ACTION", "QUERYTRANSACTION");
            requestParameters.Add("MERCHANT", "10001864");
            requestParameters.Add("MERCHANTUSER", "lilyumkart@lilyumkart.com");
            requestParameters.Add("MERCHANTPASSWORD", "Neo1234?");
            //requestParameters.Add("MERCHANTPAYMENTID", _guidId);
        }
        public void prepareSessionTokenParametersGuid(Dictionary<String, String> requestParameters)
        {
            //requestParameters.Add("MERCHANTPAYMENTID", _guidId);
            addMerchantAuthParamsGuid(requestParameters);
        }
        public void addCardAndCustomerParams(Dictionary<String, String> requestParameters)
        {
            requestParameters.Add("CUSTOMER", Guid.NewGuid().ToString());
            requestParameters.Add("CUSTOMERNAME", _sigortaliUnvani);
            requestParameters.Add("CUSTOMEREMAIL", _sigortaliEmail);
            requestParameters.Add("CUSTOMERIP", "127.0.0.1");
            requestParameters.Add("CUSTOMERPHONE", _sigortaliCepTel);
            requestParameters.Add("CUSTOMERBIRTHDAY", "19-04-1980");
            requestParameters.Add("CUSTOMERUSERAGENT", "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:56.0) Gecko/20100101 Firefox/56.0");
            requestParameters.Add("NAMEONCARD", "");
            requestParameters.Add("CARDPAN", "");
            requestParameters.Add("CARDEXPIRY", "");
            requestParameters.Add("CARDCVV", "");
        }

    
        public void addBillToAndShipToParams(Dictionary<String, String> requestParameters)
        {

            //var teklif = _TeklifService.GetAnaTeklif(teklif.GenelBilgiler.TeklifNo, this.GenelBilgiler.TVMKodu);
    
            
            requestParameters.Add("BILLTOADDRESSLINE", _sigortaliAcikAdres);
            requestParameters.Add("BILLTOCITY", _ilAdi);
            requestParameters.Add("BILLTOCOUNTRY", "Türkiye");
            requestParameters.Add("BILLTOPOSTALCODE", "06000");
            requestParameters.Add("BILLTOPHONE", _sigortaliCepTel);  //"+903120000001"
            requestParameters.Add("SHIPTOADDRESSLINE", _sigortaliAcikAdres);
            requestParameters.Add("SHIPTOCITY", _ilAdi);
            requestParameters.Add("SHIPTOCOUNTRY", "Türkiye");
            requestParameters.Add("SHIPTOPOSTALCODE", "06000");
            requestParameters.Add("SHIPTOPHONE", _sigortaliCepTel); //"+903120000001"
        }

    }
}
