using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Tools
{
    public class TelefonClientValidationRule : ModelClientValidationRule
    {
        public TelefonClientValidationRule(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ValidationType = "telefon";
        }
    }
}