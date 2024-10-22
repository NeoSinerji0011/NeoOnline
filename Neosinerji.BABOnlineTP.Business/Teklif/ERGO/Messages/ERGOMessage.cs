using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.ERGO.Messages
{
    public class ERGOMessage
    {
        public virtual string GetSoapAction()
        {
            return "http://as.com/server/cms/";
        }

        public virtual string GetMessageBody()
        {
            return String.Empty;
        }

        public virtual void ReadXml(string xml, string name)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("cms", "http://as.com/server/cms/");

            XmlNode node = doc.SelectSingleNode(String.Format("//cms:{0}", name), nsmgr);

            if (node != null && node.HasChildNodes)
            {
                XmlNodeList nodes = node.ChildNodes;
                this.XmlValues = new List<KeyValuePair<string, string>>();
                foreach (XmlNode n in nodes)
                {
                    this.XmlValues.Add(new KeyValuePair<string, string>(n.Name, n.InnerText));
                }
            }
            else
            {
                XmlNode faultNode = doc.SelectSingleNode("//faultstring");
                XmlNode detail = doc.SelectSingleNode("//detail").FirstChild.ChildNodes[3];
                if (detail != null)
                {
                    XmlQualifiedName code = new XmlQualifiedName("detail");
                    throw new System.Web.Services.Protocols.SoapException(detail.InnerText, code);
                }
                else
                {
                    XmlQualifiedName code = new XmlQualifiedName("detail");
                    throw new System.Web.Services.Protocols.SoapException("Bilinmeyen hata", code);
                }

                //if (faultNode != null)
                //{
                //    XmlQualifiedName code = new XmlQualifiedName("faultCode");
                //    throw new System.Web.Services.Protocols.SoapException(faultNode.InnerText, code);
                //}
                //else
                //{
                //    XmlQualifiedName code = new XmlQualifiedName("faultCode");
                //    throw new System.Web.Services.Protocols.SoapException("Bilinmeyen hata", code);
                //}
            }
        }

        protected string MsgParameter(string nodeName, string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Format("<{0} xsi:type=\"xsd:string\" />", nodeName);
            else
                return String.Format("<{0} xsi:type=\"xsd:string\">{1}</{0}>", nodeName, value);
        }

        [XmlIgnore]
        protected List<KeyValuePair<string, string>> XmlValues { get; set; }

        protected string XmlValueForKey(string key)
        {
            KeyValuePair<string, string> item = this.XmlValues.FirstOrDefault(f => f.Key == key);

            return item.Value;
        }

        private static NumberFormatInfo _numberFormat;
        [XmlIgnore]
        private static NumberFormatInfo NumberFormat
        {
            get
            {
                if (_numberFormat == null)
                {
                    _numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    _numberFormat.NumberDecimalSeparator = ".";
                    _numberFormat.NumberDecimalDigits = 2;
                }

                return _numberFormat;
            }
        }

        public static decimal ToDecimal(string value)
        {
            if (String.IsNullOrEmpty(value))
                return decimal.Zero;

            decimal result = 0;
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, NumberFormat, out result))
            {
                return result;
            }

            return decimal.Zero;
        }

        public static string ToDecimalString(decimal value)
        {
            return Convert.ToString(value, NumberFormat);
        }
    }
}
