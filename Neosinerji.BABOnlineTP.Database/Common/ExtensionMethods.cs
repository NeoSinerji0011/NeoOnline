using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace System.Common
{
    public static class MyExtensionMethods
    {
        //Türkiyedeki saati alır 
        public static DateTime Turkey(this DateTime date)
        {
            DateTime result = TurkeyDateTime.Now;

            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

            result = TimeZoneInfo.ConvertTime(TurkeyDateTime.Now, turkeyTimeZone);


            return result;
        }


        //Bir nesyeli string hale (user=deneme&paswprd=test& ) getirir.
        public static string ClassToString(this Object o)
        {
            string result = "";

            Type type = o.GetType();
            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo property in type.GetProperties())
            {
                sb.Append(property.Name).Append("=");
                sb.Append(property.GetValue(o, null) == null ? "" : property.GetValue(o, null).ToString()).Append("&");
            }

            result = sb.ToString();

            return result;
        }
    }

    public class TurkeyDateTime
    {
        public static DateTime Now
        {
            get
            {
                return Turkey();
            }
        }

        public static DateTime Today
        {
            get
            {
                return Turkey().Date;
            }
        }

        private static DateTime Turkey()
        {
            DateTime result = DateTime.Now;

            var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

            result = TimeZoneInfo.ConvertTime(DateTime.Now, turkeyTimeZone);

            return result;
        }
    }
}
