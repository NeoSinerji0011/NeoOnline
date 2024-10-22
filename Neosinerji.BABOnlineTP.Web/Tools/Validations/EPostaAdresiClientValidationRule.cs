using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Tools
{
    public class EPostaAdresiClientValidationRule : ModelClientValidationRule
    {
        public EPostaAdresiClientValidationRule(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ValidationType = "email";
        }
    }
}