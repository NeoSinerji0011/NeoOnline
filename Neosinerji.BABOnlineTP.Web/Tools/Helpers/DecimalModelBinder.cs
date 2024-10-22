using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neosinerji.BABOnlineTP.Web.Tools.Helpers
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (value != null)
            {
                decimal result;

                var isNullableAndNull = (bindingContext.ModelMetadata.IsNullableValueType &&
                                        string.IsNullOrEmpty(value.AttemptedValue));


                if (!isNullableAndNull && decimal.TryParse(value.AttemptedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    return result;
                }
                else
                {
                    bindingContext.ModelState.AddModelError(
                        bindingContext.ModelName,
                        string.Format("{0} formatı uygun değil", value.AttemptedValue)
                    );
                }
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}