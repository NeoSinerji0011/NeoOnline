using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class WsseSecurity
    {
        public WsseSecurity(string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\">");
            sb.Append("<wsse:UsernameToken>");
            sb.AppendFormat("<wsse:Username>{0}</wsse:Username>", this.Username);
            sb.AppendFormat("<wsse:Password>{0}</wsse:Password>", this.Password);
            sb.Append("</wsse:UsernameToken>");
            sb.Append("</wsse:Security>");

            return sb.ToString();
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
