using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.HDI
{
    public class HDIMessage
    {
        private static NumberFormatInfo _numberFormat;
        private static NumberFormatInfo NumberFormat
        {
            get
            {
                if (_numberFormat == null)
                {
                    _numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    _numberFormat.NumberDecimalSeparator = ".";
                }

                return _numberFormat;
            }
        }

        public static DateTime ToDateTime(string value)
        {
            if (String.IsNullOrEmpty(value) || value.Length != 8)
                return DateTime.MinValue;

            DateTime dt = DateTime.MinValue;

            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            else return DateTime.MinValue;
        }

        public static DateTime ToDateTimeForKasko(string value)
        {
            if (String.IsNullOrEmpty(value) || value.Length != 8)
                return DateTime.MinValue;

            DateTime dt = DateTime.MinValue;

            if (DateTime.TryParseExact(value, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            else return DateTime.MinValue;
        }

        public static string ToHDIDate(DateTime value)
        {
            if (value == DateTime.MinValue)
                return "20000101";

            return value.ToString("ddMMyyyy");
        }

        public static string ToHDIDateForDask(DateTime value)
        {
            if (value == DateTime.MinValue)
                return "20000101";

            return value.ToString("yyyyMMdd");
        }

        public static decimal ToDecimal(string value)
        {
            if (String.IsNullOrEmpty(value))
                return decimal.Zero;

            decimal result = 0;
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint, NumberFormat, out result))
            {
                return result;
            }

            return decimal.Zero;
        }

        public static string ToHDITelefon(string telefonNo)
        {
            if (!String.IsNullOrEmpty(telefonNo))
            {
                string[] parts = telefonNo.Split('-');

                if (parts.Length == 3)
                {
                    string UluslararasiKod = parts[0];
                    string AlanKodu = parts[1];
                    string Numara = parts[2];

                    return AlanKodu + Numara;
                }
            }

            return String.Empty;
        }

        public static T FromXml<T>(string xml)
        {
            T result;
            xml = xml.Replace("<?xml version=\"1.0\" encoding=\"iso-8859-9\"?>", "")
                     .Replace("\r\n", "");

            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(xml);
                ms.Write(buffer, 0, buffer.Length);
                ms.Position = 0;
                using (XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    result = (T)xs.Deserialize(ms);
                }
            }

            return result;
        }

        public override string ToString()
        {
            string output = String.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                using (HDIXmlTextWriter xmlWriter = new HDIXmlTextWriter(ms, Encoding.UTF8))
                {
                    XmlSerializer s = new XmlSerializer(this.GetType());
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    s.Serialize(xmlWriter, this, ns);
                }

                output = Encoding.UTF8.GetString(ms.ToArray());
            }

            return output;
        }

        public TResponse HttpRequest<TResponse>(string serviceUrl, string requestXml, out string responseXml)
        {
            Encoding encoding = Encoding.GetEncoding("ISO-8859-9");

            //Türkçe karakterleri ISO-8859-9 olarak ayarla
            requestXml = requestXml.Replace("ç", HttpUtility.UrlEncode("ç", encoding))
                                   .Replace("Ç", HttpUtility.UrlEncode("Ç", encoding))
                                   .Replace("ğ", HttpUtility.UrlEncode("ğ", encoding))
                                   .Replace("Ğ", HttpUtility.UrlEncode("Ğ", encoding))
                                   .Replace("ı", HttpUtility.UrlEncode("ı", encoding))
                                   .Replace("İ", HttpUtility.UrlEncode("İ", encoding))
                                   .Replace("ş", HttpUtility.UrlEncode("ş", encoding))
                                   .Replace("Ş", HttpUtility.UrlEncode("Ş", encoding))
                                   .Replace("ö", HttpUtility.UrlEncode("ö", encoding))
                                   .Replace("Ö", HttpUtility.UrlEncode("Ö", encoding))
                                   .Replace("ü", HttpUtility.UrlEncode("ü", encoding))
                                   .Replace("Ü", HttpUtility.UrlEncode("Ü", encoding));

            //enter karakterlerini kaldır
            requestXml = requestXml.Replace("\r\n", string.Empty);

            requestXml = requestXml.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(serviceUrl + requestXml);
            webRequest.Timeout = 150000;
            webRequest.Method = "GET";
            webRequest.ContentType = "text/xml";


            HttpWebResponse webresponse = (HttpWebResponse)webRequest.GetResponse();

            // Turkish:1254 codepage i kullanıyor.
            Encoding responseEncoding = Encoding.GetEncoding(1254);

            using (StreamReader reader = new StreamReader(webresponse.GetResponseStream(), responseEncoding))
            {
                CultureInfo culture = new CultureInfo("tr-TR");
                responseXml = reader.ReadToEnd().ToString(culture);
            }

            responseXml = responseXml.Trim();
            IWEBServiceLogStorage storage = DependencyResolver.Current.GetService<IWEBServiceLogStorage>();
            storage.UploadFile("plakaSorgulama", "plakaSorgulama" + System.Guid.NewGuid().ToString(), responseXml);

            return HDIMessage.FromXml<TResponse>(responseXml);
        }

        public TResponse HttpRequestForDask<TResponse>(HDIMessage request, out string responseXml, string ServiceURL)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(ServiceURL);
            webRequest.Timeout = 150000;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            string rs = request.ClassToString();
            byte[] byteArray = Encoding.UTF8.GetBytes(rs);

            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            HttpWebResponse webresponse = (HttpWebResponse)webRequest.GetResponse();


            string charset = webresponse.CharacterSet;
            Encoding responseEncoding = Encoding.GetEncoding(charset);

            using (StreamReader reader = new StreamReader(webresponse.GetResponseStream(), responseEncoding))
            {
                CultureInfo culture = new CultureInfo("tr-TR");
                responseXml = reader.ReadToEnd().ToString(culture);
            }

            responseXml = responseXml.Trim();
            IWEBServiceLogStorage storage = DependencyResolver.Current.GetService<IWEBServiceLogStorage>();
            storage.UploadFile("plakaSorgulama", "plakaSorgulama" + System.Guid.NewGuid().ToString(), responseXml);

            return HDIMessage.FromXml<TResponse>(responseXml);
        }
    }

    public class HDIXmlTextWriter : XmlTextWriter
    {
        public HDIXmlTextWriter(Stream stream, Encoding enc)
            : base(stream, enc)
        {
        }
        public HDIXmlTextWriter(String str, Encoding enc)
            : base(str, enc)
        {
        }

        public override void WriteEndElement()
        {
            base.WriteFullEndElement();
        }
    }
}
