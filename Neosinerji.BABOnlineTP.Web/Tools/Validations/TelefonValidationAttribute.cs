using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Models;

namespace Neosinerji.BABOnlineTP.Web.Tools
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class TelefonValidationAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            bool result = false;

            if (value is TelefonModel && value != null)
            {
                TelefonModel model = value as TelefonModel;

                if (String.IsNullOrEmpty(model.AlanKodu) && String.IsNullOrEmpty(model.Numara))
                {
                    result = true;
                }

                else if (!String.IsNullOrEmpty(model.UluslararasiKod) && model.UluslararasiKod.Length > 0 &&
                   !String.IsNullOrEmpty(model.AlanKodu) && model.AlanKodu.Length == 3 &&
                   !String.IsNullOrEmpty(model.Numara) && model.Numara.Length == 7)
                {
                    result = true;
                }
            }

            return result;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new TelefonClientValidationRule(BABOnlineTP.Web.Content.Lang.babonline.Message_PhoneNumber);
            yield return rule;
        }
    }
}