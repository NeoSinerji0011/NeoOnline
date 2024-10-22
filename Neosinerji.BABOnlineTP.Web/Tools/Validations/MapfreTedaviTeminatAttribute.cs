using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace Neosinerji.BABOnlineTP.Web.Tools
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapfreTedaviTeminatAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            var properties = TypeDescriptor.GetProperties(value);
            var olumSakatlik = properties.Find("OlumSakatlikTeminat", true);
            var tedavi = properties.Find("Tedavi", true);
            var tedaviTeminat = properties.Find("TedaviTeminat", true);

            if (tedavi != null)
            {
                if (Convert.ToBoolean(tedavi.GetValue(value)))
                {
                    decimal olumSakatlikTutar = 0;
                    if (olumSakatlik != null)
                    {
                        object val = olumSakatlik.GetValue(value);
                        decimal.TryParse(val.ToString(), out olumSakatlikTutar);
                    }
                    decimal tedaviTutar = 0;
                    if (tedaviTeminat != null)
                    {
                        object val = tedaviTeminat.GetValue(value);
                        decimal.TryParse(val.ToString(), out tedaviTutar);
                    }

                    decimal t = (decimal)olumSakatlikTutar * 0.1M;
                    if (tedaviTutar > t)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new MapfreTedaviTeminatClientValidationRule("Tedavi teminat tutarı, Ölüm sakatlık teminatının %10'unu geçemez.");
            yield return rule;
        }
    }
}