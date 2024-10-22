using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MapfreSorgu
    {
        private List<KeyValuePair<string, string>> _ParameterXML;
        private string _ApiCode;
        private string _UserName;
        private string _Password;
        private string _Ip;
        public string Url { get; set; }

        public MapfreSorgu(string apiCode, string userName, string password)
        {
            this._ApiCode = apiCode;
            this._UserName = userName;
            this._Password = password;
            this._Ip = String.Empty;
            _ParameterXML = new List<KeyValuePair<string, string>>();
        }

        public MapfreSorgu(string apiCode, string userName, string password, string ip)
        {
            this._ApiCode = apiCode;
            this._UserName = userName;
            this._Password = password;
            this._Ip = ip;
            _ParameterXML = new List<KeyValuePair<string, string>>();
        }

        public void AddParameter(string name, string value)
        {
            this._ParameterXML.Add(new KeyValuePair<string, string>(name, value));
        }

        public string GetServiceMessage(string fonksiyon, bool logmsg)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<soapenv:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ser=\"http://services.ws.genel.mtr.mapfre.com\">");
            sb.Append("<soapenv:Body>");
            if (logmsg)
            {
                sb.Append(this.GetMessageBodyLog(fonksiyon));
            }
            else
            {
                sb.Append(this.GetMessageBody(fonksiyon));
            }
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");

            return sb.ToString();
        }

        public string GetMessageBody(string fonksiyon)
        {
            StringBuilder sb = new StringBuilder();
            //_Ip = "88.249.64.40";
            sb.AppendFormat("<ser:{0} soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">", fonksiyon);
            sb.AppendFormat("<apiCode xsi:type=\"xsd:string\">{0}</apiCode>", this._ApiCode);
            sb.AppendFormat("<userName xsi:type=\"xsd:string\">{0}</userName>", this._UserName);
            sb.AppendFormat("<passWord xsi:type=\"xsd:string\">{0}</passWord>", this._Password);
            if (fonksiyon == "validateUserWithIP")
            {
                sb.AppendFormat("<ip xsi:type=\"xsd:string\">{0}</ip>", this._Ip);
            }


            if (this._ParameterXML.Count > 0)
            {
                sb.Append("<parameterXML xsi:type=\"xsd:string\">");
                sb.Append("<![CDATA[<parameters>");

                foreach (var item in this._ParameterXML)
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
                }

                sb.Append("</parameters>]]>");
                sb.Append("</parameterXML>");
            }

            sb.AppendFormat("</ser:{0}>", fonksiyon);

            return sb.ToString();
        }

        public string GetMessageBodyLog(string fonksiyon)
        {
            StringBuilder sb = new StringBuilder();
            //_Ip = "88.249.64.40";
            sb.AppendFormat("<ser:{0} soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">", fonksiyon);
            string apiCode = String.Empty;
            string userName = String.Empty;
            string password = String.Empty;
            if(!String.IsNullOrEmpty(this._ApiCode) &&  this._ApiCode.Length > 4)
            {
                apiCode = this._ApiCode.Substring(0, 4) + "".PadLeft(this._ApiCode.Length - 4, 'x');
            }
            if (!String.IsNullOrEmpty(this._UserName) && this._UserName.Length > 0)
            {
                userName = this._UserName[0] + "".PadLeft(this._UserName.Length - 2, 'x') + this._UserName[this._UserName.Length - 1];
            }
            if (!String.IsNullOrEmpty(this._Password) && this._Password.Length > 0)
            {
                password = this._Password[0] + "".PadLeft(this._Password.Length - 2, 'x') + this._Password[this._Password.Length - 1];
            }
            sb.AppendFormat("<apiCode xsi:type=\"xsd:string\">{0}</apiCode>", apiCode);
            userName = "A1133841";
            password = "123456hasan";
            sb.AppendFormat("<userName xsi:type=\"xsd:string\">{0}</userName>", userName);
            sb.AppendFormat("<passWord xsi:type=\"xsd:string\">{0}</passWord>", password);
            if (fonksiyon == "validateUserWithIP")
            {
                sb.AppendFormat("<ip xsi:type=\"xsd:string\">{0}</ip>", this._Ip);
            }

            if (this._ParameterXML.Count > 0)
            {
                sb.Append("<parameterXML xsi:type=\"xsd:string\">");
                sb.Append("<![CDATA[<parameters>");

                foreach (var item in this._ParameterXML)
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
                }

                sb.Append("</parameters>]]>");
                sb.Append("</parameterXML>");
            }

            sb.AppendFormat("</ser:{0}>", fonksiyon);

            return sb.ToString();
        }

        public T CallService<T>(string fonksiyon) where T : MapfreSorguResponse
        {
            using (var wb = new WebClient())
            {
                wb.Headers.Clear();
                wb.Headers.Add("Content-Type", "text/xml; charset=utf-8");
                wb.Headers.Add("SOAPAction", "");
                string hostAddresss = (new Uri(this.Url)).Host;
                wb.Headers.Add("Host", hostAddresss);

                UTF8Encoding encoding = new UTF8Encoding();
                string request = this.GetServiceMessage(fonksiyon, false);
                byte[] data = encoding.GetBytes(request);

                var response = wb.UploadData(this.Url, "POST", data);
                wb.Dispose();
                string responseString = encoding.GetString(response);

                string nameResponse = fonksiyon + "Response";
                string cdataNameResponse = fonksiyon + "Return";
                T res = MapfreSorguResponse.ReadXml<T>(responseString, nameResponse, cdataNameResponse, typeof(T));

                return res;
            }
        }

        protected string UserName
        {
            get
            {
                return this._UserName;
            }
        }

        protected string Password
        {
            get
            {
                return this._Password;
            }
        }
    }
}
