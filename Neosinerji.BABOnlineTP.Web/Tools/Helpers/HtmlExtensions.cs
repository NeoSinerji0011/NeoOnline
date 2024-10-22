using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Text;
using System.Reflection;
using System.Web.Script.Serialization;
using Neosinerji.BABOnlineTP.Web.Content.Lang;

namespace System.Web.Mvc.Html
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// Textbox'ın üzerine gelindiğinde yada aktif olduğunda tooltip göstermesi için kullanılır.
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString TextBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                Expression<Func<TModel, TValue>> expression,
                                                                object htmlAttributes,
                                                                string tooltip)
        {
            MvcHtmlString html = default(MvcHtmlString);

            RouteValueDictionary routeValues = new RouteValueDictionary(htmlAttributes);
            routeValues.Add("rel", "tooltip");
            routeValues.Add("data-placement", "right");
            routeValues.Add("data-html", "true");
            routeValues.Add("data-trigger", "hover focus");
            routeValues.Add("data-original-title", tooltip);

            html = Html.InputExtensions.TextBoxFor(htmlHelper, expression, routeValues);

            return html;
        }

        /// <summary>
        /// Verilen SelectlistItem listesine uygun radio button grubunu oluşturur
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="listOfValues"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonListFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> listOfValues)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var sb = new StringBuilder();

            if (listOfValues != null)
            {
                // Create a radio button for each item in the list 
                foreach (SelectListItem item in listOfValues)
                {
                    // Generate an id to be given to the radio button field 
                    var id = string.Format("{0}_{1}", metaData.PropertyName, item.Value);

                    // Create and populate a radio button using the existing html helpers
                    var radio = htmlHelper.RadioButtonFor(expression, item.Value, new { id = id }).ToHtmlString();

                    sb.AppendLine("<label class='radio inline'>");
                    sb.AppendLine(radio);
                    sb.AppendLine(item.Text);
                    sb.AppendLine("</label>");
                }
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// Alan için datepicker textbox oluşturur
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString DateTextBoxFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                   Expression<Func<TModel, TValue>> expression,
                                                                   object htmlAttributes)
        {
            Func<TModel, TValue> method = expression.Compile();
            TValue value = method(htmlHelper.ViewData.Model);

            string defaultDate = String.Empty;
            if (value is DateTime && value != null)
            {
                DateTime dateValue = Convert.ToDateTime(value);

                if (dateValue != DateTime.MinValue)
                    defaultDate = dateValue.ToString("dd.MM.yyyy");
            }
            var name = ExpressionHelper.GetExpressionText(expression);            
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            string htmlId = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div id='{0}_control' class='input-append datepicker date' data-date='{1}' data-date-format='"+ babonline.date_format + "' date-language='"+ babonline.date_language + "'>", htmlId, defaultDate);
            sb.AppendLine();

            var dateInput = new TagBuilder("input");
            dateInput.Attributes["name"] = htmlName;
            dateInput.Attributes["id"] = htmlId;
            dateInput.Attributes["class"] = "m-wrap text-box single-line date-custom";
            dateInput.Attributes["type"] = "text";
            dateInput.Attributes["date-language"] = babonline.date_language;
            dateInput.Attributes["value"] = defaultDate;
            dateInput.Attributes["data-val-required"] = Neosinerji.BABOnlineTP.Web.Content.Lang.babonline.Message_Required;
            dateInput.Attributes["data-val"] = "true";
            //dateInput.Attributes["readonly"] = "readonly";
            dateInput.Attributes["placeholder"] = Neosinerji.BABOnlineTP.Web.Content.Lang.babonline.ddmmYYYY;
            dateInput.Attributes["style"] = "width:80px;";
            dateInput.Attributes["data-val-date"] = Neosinerji.BABOnlineTP.Web.Content.Lang.babonline.Message_DateFormat;

            sb.AppendLine(dateInput.ToString(TagRenderMode.SelfClosing));
            // sb.AppendLine("    <span class=\"add-on\"><i class=\"\"></i></span>");
            //sb.AppendLine("    <span class=\"add-on\"><i class=\"icon-th\"></i></span>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }
        /// Alan için datepicker textbox oluşturur
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString DateTextBoxNotValidFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                   Expression<Func<TModel, TValue>> expression,
                                                                   object htmlAttributes)
        {
            Func<TModel, TValue> method = expression.Compile();
            TValue value = method(htmlHelper.ViewData.Model);

            string defaultDate = String.Empty;
            if (value is DateTime && value != null)
            {
                DateTime dateValue = Convert.ToDateTime(value);

                if (dateValue != DateTime.MinValue)
                    defaultDate = dateValue.ToString("dd.MM.yyyy");
            }
            var name = ExpressionHelper.GetExpressionText(expression);
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            string htmlId = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div id='{0}_control' class='input-append datepicker date' data-date='{1}' data-date-format='"+ babonline.date_format + "' date-language='"+ babonline.date_language + "'>", htmlId, defaultDate);
            sb.AppendLine();

            var dateInput = new TagBuilder("input");
            dateInput.Attributes["name"] = htmlName;
            dateInput.Attributes["id"] = htmlId;
            dateInput.Attributes["class"] = "m-wrap text-box single-line date-custom";
            dateInput.Attributes["type"] = "text";
            dateInput.Attributes["date-language"] = babonline.date_language;
            dateInput.Attributes["value"] = defaultDate;
            dateInput.Attributes["data-val"] = "false";
            //dateInput.Attributes["readonly"] = "readonly";
            dateInput.Attributes["placeholder"] = Neosinerji.BABOnlineTP.Web.Content.Lang.babonline.ddmmYYYY;
            dateInput.Attributes["style"] = "width:80px;";
            dateInput.Attributes["data-val-date"] = Neosinerji.BABOnlineTP.Web.Content.Lang.babonline.Message_DateFormat;

            sb.AppendLine(dateInput.ToString(TagRenderMode.SelfClosing));
            // sb.AppendLine("    <span class=\"add-on\"><i class=\"\"></i></span>");
            //sb.AppendLine("    <span class=\"add-on\"><i class=\"icon-th\"></i></span>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Alan için datepicker textbox oluşturur
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString DateTextBoxNotRequiredFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                            Expression<Func<TModel, TValue>> expression,
                                                                            object htmlAttributes)
        {
            Func<TModel, TValue> method = expression.Compile();
            TValue value = method(htmlHelper.ViewData.Model);

            string defaultDate = String.Empty;
            if (value is DateTime && value != null)
            {
                DateTime dateValue = Convert.ToDateTime(value);

                if (dateValue != DateTime.MinValue)
                    defaultDate = dateValue.ToString("dd.MM.yyyy");
            }
            var name = ExpressionHelper.GetExpressionText(expression);
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            string htmlId = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);

            StringBuilder sb = new StringBuilder();
                
            sb.AppendFormat("<div id='{0}_control' class='input-append datepicker date' data-date='{1}' data-date-format='"+ babonline.date_format + "' date-language='"+ babonline.date_language+ "'>", htmlId, defaultDate);
            sb.AppendLine();

            var dateInput = new TagBuilder("input");
            dateInput.Attributes["name"] = htmlName;
            dateInput.Attributes["id"] = htmlId;
            dateInput.Attributes["class"] = "m-wrap text-box single-line date-custom";
            dateInput.Attributes["type"] = "text";
            dateInput.Attributes["date-language"] = babonline.date_language;
            dateInput.Attributes["value"] = defaultDate;
            dateInput.Attributes["placeholder"] = Neosinerji.BABOnlineTP.Web.Content.Lang.babonline.ddmmYYYY;
            dateInput.Attributes["style"] = "width:80px;";
            dateInput.Attributes["data-val-date"] = Neosinerji.BABOnlineTP.Web.Content.Lang.babonline.Message_DateFormat;

            sb.AppendLine(dateInput.ToString(TagRenderMode.SelfClosing));
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Alan için tvm arama alanını oluşturur
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString TVMFinderFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                 Expression<Func<TModel, TValue>> expression,
                                                                 string TVMUnvani)
        {
            MvcHtmlString hidden = Html.InputExtensions.HiddenFor(htmlHelper, expression);

            var name = ExpressionHelper.GetExpressionText(expression);
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine(hidden.ToHtmlString());
            sb.AppendLine("<div class='input-append'>");
            sb.AppendFormat("<input style='width:80%; font-size: 13px'id='{0}_text' type='text' disabled='disabled' placeholder='{1}' value='{2}'/>", htmlName, babonline.TVM_Select, TVMUnvani);

            sb.AppendLine();
            sb.AppendLine("<div class='btn-group' style='margin-bottom:0px !important;'>");
            sb.AppendLine("<button id='tvm-select-btn' class='btn btn-success' style='display:none;' type='button'><i class='icon-zoom-in icon-white'></i></button>");
            sb.AppendLine("<button id='tvm-select-cancel-btn' class='btn btn-danger' style='display:none;' type='button'><i class='icon-remove icon-white'></i></button>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div id='tmv-list-container' style='display:none;width:700px'></div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Kullanıcı arama alanını oluşturur
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString TVMKullaniciFinderFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                          Expression<Func<TModel, TValue>> expression,
                                                                          string KullaniciAdiSoyadi)
        {
            MvcHtmlString hidden = Html.InputExtensions.HiddenFor(htmlHelper, expression);

            var name = ExpressionHelper.GetExpressionText(expression);
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine(hidden.ToHtmlString());
            sb.AppendLine("<div class='input-append'>");
            sb.AppendFormat("<input style='width:80%; font-size: 13px' id='{0}_text' type='text' disabled='disabled' placeholder='{1}' value='{2}'/>", htmlName, babonline.User_Select, KullaniciAdiSoyadi);
            sb.AppendLine();
            sb.AppendLine("<div class='btn-group' style='margin-bottom:10px;'>");
            sb.AppendLine("<button id='user-select-btn' class='btn btn-success' style='display:none;margin-top:10px;' type='button'><i class='icon-zoom-in icon-white'></i></button>");
            sb.AppendLine("<button id='user-select-cancel-btn' class='btn btn-danger' style='display:none;margin-top:10px;' type='button'><i class='icon-remove icon-white'></i></button>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div id='user-list-container' style='display:none;width:700px'></div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Kullanıcı arama alanını oluşturur
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString MusteriFinderFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                     Expression<Func<TModel, TValue>> expression,
                                                                     Expression<Func<TModel, string>> kimlikNo,
                                                                     Expression<Func<TModel, string>> adUnvan)
        {
            MvcHtmlString hidden = Html.InputExtensions.HiddenFor(htmlHelper, expression);

            var name = ExpressionHelper.GetExpressionText(expression);
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);

            var kimlik = ExpressionHelper.GetExpressionText(kimlikNo);
            string kimlikNoName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(kimlik);
            string kimlikNoId = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(kimlik);

            Func<TModel, string> method = kimlikNo.Compile();
            string kimlikValue = method(htmlHelper.ViewData.Model);

            var ad = ExpressionHelper.GetExpressionText(adUnvan);
            string adName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(ad);
            string adId = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ad);

            Func<TModel, string> method1 = adUnvan.Compile();
            string adValue = method1(htmlHelper.ViewData.Model);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(hidden.ToHtmlString());
            sb.AppendLine("<div class='input-append'>");
            sb.AppendFormat("<input style='width:80%; font-size: 13px; margin-right:10px;' id='{0}' name='{1}' type='text' autocompletetype='disabled' maxlength='11' value='{2}'/>", kimlikNoId, kimlikNoName, kimlikValue);
            sb.AppendFormat("<input style='width:300px;' id='{0}' name='{1}' type='text' disabled='disabled' value='{2}'/>", adId, adName, adValue);
            sb.AppendLine();
            sb.AppendLine("<div class='btn-group' style='margin-bottom:10px;'>");
            sb.AppendFormat("<button id='{0}_SelectBtn' class='btn btn-success' style='margin-top:10px;' type='button'><i class='icon-zoom-in icon-white'></i></button>", htmlName);
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Müşteri arama alanını oluşturur
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString MusteriFinderFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper,
                                                                     Expression<Func<TModel, TValue>> expression,
                                                                     string adUnvan)
        {
            MvcHtmlString hidden = Html.InputExtensions.HiddenFor(htmlHelper, expression);

            var name = ExpressionHelper.GetExpressionText(expression);
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine(hidden.ToHtmlString());
            sb.AppendLine("<div class='input-append'>");
            sb.AppendFormat("<input style='width:80%; font-size: 13px' id='{0}_text' type='text' disabled='disabled' placeholder='{1}' value='{2}'/>", htmlName, babonline.SelectCustomer, adUnvan);
            sb.AppendLine();
            sb.AppendLine("<div class='btn-group' style='margin:0 !important; padding:0 !important;'>");
            sb.AppendLine("<button id='user-select-btn' class='btn btn-success' style='display:none;' type='button'><i class='icon-zoom-in icon-white'></i></button>");
            sb.AppendLine("<button id='user-select-cancel-btn' class='btn btn-danger' style='display:none;' type='button'><i class='icon-remove icon-white'></i></button>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div id='user-list-container' style='display:none;width:700px'></div>");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString SwitchButtonFor<TModel>(this HtmlHelper<TModel> htmlHelper,
                                                             Expression<Func<TModel, bool>> expression,
                                                             string[] labels,
                                                             object htmlAttributes)
        {
            Func<TModel, bool> method = expression.Compile();
            bool value = method(htmlHelper.ViewData.Model);

            var name = ExpressionHelper.GetExpressionText(expression);
            string htmlid = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(name);
            string htmlName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);

            StringBuilder sb = new StringBuilder();
            var div = new TagBuilder("div");
            div.Attributes["id"] = htmlid + "_control";
            div.Attributes["class"] = "switcher switcher-small";
            div.Attributes["data-on"] = "success";
            div.Attributes["data-off"] = "danger";
            div.Attributes["data-on-label"] = labels[0];
            div.Attributes["data-off-label"] = labels[1];

            RouteValueDictionary routeValues = new RouteValueDictionary(htmlAttributes);
            foreach (var item in routeValues)
            {
                div.Attributes[item.Key] = item.Value.ToString();
            }

            sb.AppendLine(div.ToString(TagRenderMode.StartTag));

            var checkBox = new TagBuilder("input");
            checkBox.Attributes["name"] = htmlName;
            checkBox.Attributes["id"] = htmlid;
            checkBox.Attributes["type"] = "checkbox";

            if (value)
                checkBox.Attributes["checked"] = "checked";

            sb.AppendLine(checkBox.ToString(TagRenderMode.SelfClosing));
            sb.AppendLine("</div>");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString BootstrapYesNo<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
        {
            Func<TModel, bool> method = expression.Compile();
            bool value = method(htmlHelper.ViewData.Model);

            StringBuilder sb = new StringBuilder();
            sb.Append("<label class='control'>");

            if(value)
            {
                sb.Append("<span class='label label-success'>");
                sb.Append(babonline.Yes);
                sb.Append("</span></label>");
            }
            else
            {
                sb.Append("<span class='label label-important'>");
                sb.Append(babonline.No);
                sb.Append("</span></label>");
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static string JsonSerialize(this HtmlHelper htmlHelper, object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }
    }
}