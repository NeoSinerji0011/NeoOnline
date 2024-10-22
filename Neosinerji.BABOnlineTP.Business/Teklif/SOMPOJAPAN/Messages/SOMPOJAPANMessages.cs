using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.SOMPOJAPAN.Messages
{
    public class SOMPOJAPANMessages
    {
        private static NumberFormatInfo _numberFormat;
        [XmlIgnore]
        private static NumberFormatInfo NumberFormat
        {
            get
            {
                if (_numberFormat == null)
                {
                    _numberFormat = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
                    _numberFormat.NumberDecimalSeparator = ",";
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
