using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ERGO.Messages
{
    public class ERGOService
    {
        public ERGOService(string username, string password)
        {
            this.UserName = username;
            this.Password = password;
        }
        public string GetServiceMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.Append("<soapenv:Envelope  xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\"   xmlns:xsd=\"xmlns:pol=\"http://policy.thirdparty.service.insyst.ergo.com/\">");
            sb.Append("<soapenv:Header>");
            sb.Append("<wsse:Security soapenv:actor=\"\" soapenv:mustUnderstand=\"0\" xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            sb.Append("<wsse:UsernameToken xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");
            sb.AppendFormat("<Username>{0}</Username>", this.UserName);
            sb.AppendFormat("<Password>{0}</Password>", this.Password);
            sb.Append("</wsse:UsernameToken>");
            sb.Append("</soapenv:Header>");
            sb.Append("<soapenv:Body soapenv:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">");          
            sb.Append("</soapenv:Body>");
            sb.Append("</soapenv:Envelope>");

            return sb.ToString();
        }

        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
