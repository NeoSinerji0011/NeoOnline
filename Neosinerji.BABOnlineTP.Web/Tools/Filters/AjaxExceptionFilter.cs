using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace Neosinerji.BABOnlineTP.Web.Tools
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AjaxExceptionAttribute : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest()) return;

            string message = babonline.Message_UnknownError;

            if (filterContext.Exception is DbUpdateException)
            {
                DbUpdateException exception = filterContext.Exception as DbUpdateException;
                SqlException s = exception.InnerException.InnerException as SqlException;

                if (s.Number == 2627)
                {
                    message = babonline.Message_DublicateRecord;
                }
            }
            else if (filterContext.Exception != null)
            {
                message = filterContext.Exception.Message;
            }

            filterContext.Result = AjaxError(message, filterContext);

            //Let the system know that the exception has been handled
            filterContext.ExceptionHandled = true;
        }

        protected JsonResult AjaxError(string message, ExceptionContext filterContext)
        {
            //Set the response status code to 500
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //Needed for IIS7.0
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            return new JsonResult
            {
                Data = new { message = message },
                ContentEncoding = System.Text.Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}