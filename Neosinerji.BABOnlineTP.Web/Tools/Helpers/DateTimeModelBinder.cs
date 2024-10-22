using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class DateTimeModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value != null)
            {
                DateTime result = DateTime.MinValue;
                if (DateTime.TryParseExact(value.AttemptedValue, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                {

                    return result;
                }
                else
                {
                    bindingContext.ModelState.AddModelError(
                        bindingContext.ModelName,
                        string.Format("{0} geçerli bir tarih değil", value.AttemptedValue)
                    );
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }

    public class NullableDateTimeModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value != null && !String.IsNullOrEmpty(value.AttemptedValue))
            {
                System.Globalization.CultureInfo cultureinfo = new System.Globalization.CultureInfo("tr-TR");
                var date = DateTime.Parse(value.AttemptedValue, cultureinfo);
                // var date = value.ConvertTo(typeof(DateTime), CultureInfo.CurrentCulture);
                // return base.BindModel(controllerContext, bindingContext);
                return date;
            }
            return null;
        }
    }

    //public class NullableDateTimeModelBinder : DefaultModelBinder
    //{
    //    public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

    //        if (value != null)
    //        {
    //            DateTime convert = DateTime.MinValue;
    //            DateTime? result = DateTime.MinValue;
    //            if (DateTime.TryParseExact(value.AttemptedValue, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out convert))
    //            {
    //                result = convert;
    //                return result;
    //            }
    //            else
    //            {
    //                bindingContext.ModelState.AddModelError(
    //                    bindingContext.ModelName,
    //                    string.Format("{0} geçerli bir tarih değil", value.AttemptedValue)
    //                );
    //            }
    //        }

    //        return base.BindModel(controllerContext, bindingContext);
    //    }
    //}
}