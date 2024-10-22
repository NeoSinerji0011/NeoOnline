using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;


namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MapfreSorguResponse
    {
        [XmlIgnore]
        protected List<KeyValuePair<string, string>> XmlValues { get; set; }

        protected string XmlValueForKey(string key)
        {
            KeyValuePair<string, string> item = this.XmlValues.FirstOrDefault(f => f.Key == key);

            return item.Value;
        }

        public static T ReadXml<T>(string xml, string name, string cdataName, Type returnType) where T : MapfreSorguResponse
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("ns1", "http://services.ws.genel.mtr.mapfre.com");

            XmlNode node = doc.SelectSingleNode(String.Format("//ns1:{0}", name), nsmgr);

            if (node != null && node.HasChildNodes)
            {
                XmlNode n = node.SelectSingleNode(cdataName);
                if (n != null)
                {
                    XmlSerializer xs = new XmlSerializer(returnType);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(n.InnerText);
                        ms.Write(buffer, 0, buffer.Length);
                        ms.Position = 0;

                        using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8))
                        {
                            return xs.Deserialize(ms) as T;
                        }
                    }
                }
            }
            else
            {
                XmlNode faultNode = doc.SelectSingleNode("//faultstring");

                if (faultNode != null)
                {
                    XmlQualifiedName code = new XmlQualifiedName("faultCode");
                    throw new System.Web.Services.Protocols.SoapException(faultNode.InnerText, code);
                }
                else
                {
                    XmlQualifiedName code = new XmlQualifiedName("faultCode");
                    throw new System.Web.Services.Protocols.SoapException("Bilinmeyen hata", code);
                }
            }

            return null;
        }

        /// <summary>
        /// 03/01/2012 formatındaki string değeri datetime tipine çevirir
        /// </summary>
        public static DateTime ToDateTime(string dateValue)
        {
            string[] parts = dateValue.Split('/');

            if (parts.Length == 3)
            {
                int day = int.Parse(parts[0]);
                int mon = int.Parse(parts[1]);
                int year = int.Parse(parts[2]);

                return new DateTime(year, mon, day);
            }

            return DateTime.MinValue;
        }
        public static DateTime ToDateTime2(string dateValue)
        {
            int year = int.Parse(dateValue.Substring(0, 4));
                int mon = int.Parse(dateValue.Substring(4,2));
                int day = int.Parse(dateValue.Substring(6,2));

                return new DateTime(year, mon, day);       

        }

        public static DateTime ToDateTime3(string dateValue)
        {
            int year = int.Parse(dateValue.Substring(4, 4));
            int mon = int.Parse(dateValue.Substring(2, 2));
            int day = int.Parse(dateValue.Substring(0, 2));

            return new DateTime(year, mon, day);

        }

        public static DateTime FromJavaTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime utc = epoch.AddSeconds(unixTime / 1000);

            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utc, turkeyTimeZone);
        }
    }
}
