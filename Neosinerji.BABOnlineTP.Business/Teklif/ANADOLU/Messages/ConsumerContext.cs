using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ConsumerContext
    {
        public ConsumerContext(string user)
        {
            this.lang = "tr";
            this.consumerCode = "acnt";
            this.userName = user;
            this.messageId = System.Guid.NewGuid().ToString();
            this.messageGroupId = System.Guid.NewGuid().ToString();
            this.organizationUnitCode = "999999";
            //CLOUD PROD İP
            this.ipAddress = "";// this.ipAddress; //"52.178.109.32";//"168.63.78.35";"81.214.50.9"
        }

        public new string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<as:consumerContext xsi:type=\"as:IntWebAppContextType\" xmlns:as=\"http://as.com/Technical/EA/Header/Schema/V1\">");
            sb.AppendFormat("<lang>{0}</lang>", this.lang);
            sb.AppendFormat("<consumerCode>{0}</consumerCode>", this.consumerCode);
            sb.AppendFormat("<userName>{0}</userName>", this.userName);
            sb.AppendFormat("<messageId>{0}</messageId>", this.messageId);
            sb.AppendFormat("<messageGroupId>{0}</messageGroupId>", this.messageGroupId);
            sb.Append("<customerContext>");
            sb.Append("<customer>");
            sb.Append("<customerNumber>0</customerNumber>");
            sb.Append("</customer>");
            sb.Append("</customerContext>");
            sb.AppendFormat("<clientRequestTimestamp>{0}</clientRequestTimestamp>", TurkeyDateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            sb.AppendFormat("<organizationUnitCode>{0}</organizationUnitCode>", this.organizationUnitCode);
            sb.AppendFormat("<ipAddress>{0}</ipAddress>", this.ipAddress);
            sb.Append("</as:consumerContext>");

            return sb.ToString();
        }

        public string lang { get; set; }
        public string consumerCode { get; set; }
        public string userName { get; set; }
        public string messageId { get; set; }
        public string messageGroupId { get; set; }
        public string organizationUnitCode { get; set; }
        public string ipAddress { get; set; }
    }
}
