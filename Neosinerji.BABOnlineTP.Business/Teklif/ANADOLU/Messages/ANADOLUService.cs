using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Web.Mvc;
namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ANADOLUService
    {
        public ANADOLUService(string esbnUserName, string esbnPassword, string swepUserName, string swepPassword, string clientIP)
        {
            this.Url = "https://swepsrvc.anadolusigorta.com.tr/AS_IA_GatewayWeb/sca/AS_IA_GatewayExport_WS_SOAP11";
            this.SwepUserName = swepUserName;
            this.SwepPassword = swepPassword;
            this.EsbnUserName = esbnUserName;
            this.EsbnPassword = esbnPassword;
            this.ClientIP = clientIP;// "88.249.209.253"
            this.consumerContext = new ConsumerContext(esbnUserName);
            this.wsseSecurity = new WsseSecurity(esbnUserName, esbnPassword);
        }

        public string GetServiceMessage(ANADOLUMessage messageType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<soapenv:Envelope xmlns:cms=\"http://as.com/server/cms/\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            sb.Append("<soapenv:Header>");
            sb.Append(this.consumerContext.ToString());
            sb.Append(this.wsseSecurity.ToString());
            sb.Append("</soapenv:Header>");
            sb.Append("<soapenv:Body soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");
            sb.Append(messageType.GetMessageBody());
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");

            return sb.ToString();
        }

        public T CallService<T>(ANADOLUMessage messageType) where T : ANADOLUMessage, new()
        {
            using (var wb = new ANADOLUWebClient())
            {
                try
                {
                    wb.Timeout = 240000;
                    wb.Headers.Clear();
                    wb.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                    wb.Headers.Add("SOAPAction", "\"" + messageType.GetSoapAction() + "\"");
                    string hostAddresss = (new Uri(this.Url)).Host;
                    wb.Headers.Add("Host", hostAddresss);
                    wb.Headers.Add("SWEPUsername", this.SwepUserName);
                    wb.Headers.Add("SWEPPassword", this.SwepPassword);
                    //if (this.ClientIP == null)
                    //{
                    //    this.ClientIP = TeklifUrunFactory.GetClientIP();

                    //}
                    wb.Headers.Add("X-Forwarded-For", this.ClientIP); //Acentenin tobb da kayıtlı olan ip adresi gönderilmeli this.ClientIP "88.249.209.253"(birebir) this.ClientIP

                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] data = encoding.GetBytes(this.GetServiceMessage(messageType));

                    var response = wb.UploadData(this.Url, "POST", data);
                    string responseString = encoding.GetString(response);

                    T res = new T();
                    res.ReadXml(responseString, "");

                    return res;
                }
                catch (WebException webex)
                {
                    string text = "";
                    WebResponse errResp = webex.Response;
                    using (Stream respStream = errResp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream);
                        text = reader.ReadToEnd();
                    }
                    T res = new T();
                    res.ReadXml(text, "");
                    return res;
                }
            }
        }

        public string Url { get; set; }
        public ConsumerContext consumerContext { get; set; }
        public WsseSecurity wsseSecurity { get; set; }
        public string EsbnUserName { get; set; }
        public string EsbnPassword { get; set; }
        public string SwepUserName { get; set; }
        public string SwepPassword { get; set; }
        public string ClientIP { get; set; }

    }
}
