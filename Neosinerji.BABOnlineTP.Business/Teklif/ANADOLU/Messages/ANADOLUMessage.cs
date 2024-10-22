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

namespace Neosinerji.BABOnlineTP.Business.ANADOLU
{
    public class ANADOLUMessage
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

    public class AnadoluOdemeTipleri
    {
        public const string Diger = "DGR0003200";
        public const string KrediKartiTalimati = "DGR0010173";
        public const string KrediKartiBlokeliTaksit = "DGR0010170";
        public const string KrediKartiBlokeliTaksitGenel = "DGR0019679";
        public const string Nakit = "DGR0001251";
        public const string OtomatikOdemeTalimati = "DGR0001252";
        public const string SanalPost = "DGR0010172";
        public const string SanalPostMailOrder = "DGR0010174";
    }

    public class AnadoluOdemeTipiSanalPost
    {
        public const string Pesin = "TR00008647";
    }

    public class AnadoluOdemeTipiNakit
    {
        public const string Pesin = "TR00008647";
    }

    public class AnadoluOdemeTipiKrediK_Blokeli_T_Genel
    {
        public const string BlokeliTaksit_2 = "TR00017299";
        public const string BlokeliTaksit_3 = "TR00001287";
        public const string BlokeliTaksit_4 = "TR00017300";
        public const string BlokeliTaksit_5 = "TR00017301";
        public const string BlokeliTaksit_6 = "TR00017302";
        public const string BlokeliTaksit_7 = "TR00017303";
        public const string BlokeliTaksit_8 = "TR00016406";
        public const string BlokeliTaksit_9 = "TR00017304";
        public const string BlokeliTaksit_10 = "TR00017305";
    }

    /*
     "DGR0001252" - Otomatik Ödeme Talimatı
    "TR00000195"-"%25 Peşin Kalan 5 Eşit Taksit","6"
    "TR00000196"-"8 Eşit Taksit","8"
    "TR00000350"-"2 Eşit Taksit","2"
    "TR00000351"-"3 Eşit Taksit","3"
    "TR00000352"-"4 Eşit Taksit","4"
    "TR00000353"-"%25 Peşin Kalan 4 Eşit Taksit","5"
    "TR00008647"-"Peşin","1”
     */

    public class AnadoluOdemeTipi_OtomatikOdemeTalimati
    {
        public const string Pesin_1 = "TR00008647";
        public const string EsitTaksit_2 = "TR00000350";
        public const string EsitTaksit_3 = "TR00000351";
        public const string EsitTaksit_4 = "TR00000352";
        public const string EsitTaksit_8 = "TR00000196";
        public const string Yuzde25Pesin_Kalan4Taksit_5 = "TR00000353";
        public const string Yuzde25Pesin_Kalan5Taksit_6 = "TR00000195";
    }

    public class AnadoluTrafikPaketleri
    {
        public const string EkstraTrafikSigortasi = "DGR0150003";
        public const string SuperTrafikSigortasi = "DGR0150004";
        public const string SinirsizTrafikSigortasi = "DGR0150005";
        public const string TrafikSigortasi = "DGR0150006";
    }
    public class AnadoluTaksitSayısı
    {
        public const string PesinBlokeli3Taksit = "TR00017553";
        public const string SanalPosPesin = "TR00000169";
        public const string NakitPesin = "TR00000169";
    }

    public class AnadoluTrafikKombineLimitleri
    {
        public const decimal IkiYuzElliBin = 250000;
        public const decimal BirMilyon = 1000000;
    }
}
