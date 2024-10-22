using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Neosinerji.BABOnlineTP.Web.Areas.Teklif.Models;

namespace Neosinerji.BABOnlineTP.Web.Tools.Validations
{
    public class KrediKartiValidationRule : ModelClientValidationRule
    {
        public KrediKartiValidationRule(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ValidationType = "kredikarti";
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class KrediKartiValidationAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            bool result = false;

            if (value is KrediKartiModel && value != null)
            {
                KrediKartiModel model = value as KrediKartiModel;

                if (!String.IsNullOrEmpty(model.KK1) && model.KK1.Length == 4 &&
                    !String.IsNullOrEmpty(model.KK2) && model.KK2.Length == 4 &&
                    !String.IsNullOrEmpty(model.KK3) && model.KK3.Length == 4 &&
                    !String.IsNullOrEmpty(model.KK4) && model.KK4.Length == 4)
                {
                    result = true;
                }
            }

            return result;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new KrediKartiValidationRule(BABOnlineTP.Web.Content.Lang.babonline.Message_CreditCard_Validation);
            yield return rule;
        }
    }
}