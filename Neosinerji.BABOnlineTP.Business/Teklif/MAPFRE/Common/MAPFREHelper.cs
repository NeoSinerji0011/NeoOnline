using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Neosinerji.BABOnlineTP.Business.MAPFRE
{
    public class MAPFREHelper
    {
        public static long ToMAPFREDateLong(DateTime value)
        {
            return (value.Year * 10000) + (value.Month * 100) + (value.Day);
        }

        public static DateTime ToDateTime(long value)
        {
            string strvalue = value.ToString();

            if (String.IsNullOrEmpty(strvalue) || strvalue.Length != 8)
                return DateTime.MinValue;

            int year = Convert.ToInt32(strvalue.Substring(0, 4));
            int month = Convert.ToInt32(strvalue.Substring(4, 2));
            int day = Convert.ToInt32(strvalue.Substring(6, 2));

            return new DateTime(year, month, day);
        }

        public static DateTime ToDateTime(long? value)
        {
            if (value.HasValue)
            {
                string strvalue = value.Value.ToString();

                if (String.IsNullOrEmpty(strvalue) || strvalue.Length != 8)
                    return DateTime.MinValue;

                int year = Convert.ToInt32(strvalue.Substring(0, 4));
                int month = Convert.ToInt32(strvalue.Substring(4, 2));
                int day = Convert.ToInt32(strvalue.Substring(6, 2));

                return new DateTime(year, month, day);
            }

            return DateTime.MinValue;
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
    }
}
