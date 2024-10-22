using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class JsonOptions
    {
        public string[] options { get; set; }
    }

    public static class JsonHelper
    {
        public static JsonOptions GetOptions(IList items, string columnName)
        {
            JsonOptions options = new JsonOptions();
            options.options = new string[items.Count];

            if(items.Count > 0)
            {
                object firstItem = items[0];
                PropertyInfo[] properties = firstItem.GetType().GetProperties();
                PropertyInfo itemProperty = properties.FirstOrDefault(f => f.Name == columnName);

                int index = 0;
                foreach (var item in items)
                {
                    options.options[index] = itemProperty.GetValue(item, null).ToString();
                    index++;
                }
            }

            return options;
        }
    }
}