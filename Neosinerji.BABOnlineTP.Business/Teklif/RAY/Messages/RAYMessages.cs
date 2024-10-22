using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Neosinerji.BABOnlineTP.Business.RAY
{
    public class RAYMessages
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
            CultureInfo invC = CultureInfo.InvariantCulture;
            result = Convert.ToDecimal(value, invC);

            return result;
        }

    }

}
