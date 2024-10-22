using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace System.Web.Mvc
{
    public static class ModelStateDictionaryExtension
    {
        public static void TraceErros(this ModelStateDictionary dictionary)
        {
            foreach (var key in dictionary.Keys)
            {
                ModelState state = dictionary[key];
                foreach (var error in state.Errors)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("key : {0}, error : {1}", key, error.ErrorMessage));
                }
            }
        }

        public static void RemoveFor<TModel>(this ModelStateDictionary modelState, Expression<Func<TModel, object>> expression)
        {
            string expressionText = ExpressionHelper.GetExpressionText(expression);

            foreach (var ms in modelState.ToArray())
            {
                if (ms.Key.StartsWith(expressionText + "."))
                {
                    modelState.Remove(ms);
                }
            }
        }
    }
}