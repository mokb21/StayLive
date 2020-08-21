using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace StayLive.Helpers
{
    public static class HMTLHelperExtensions
    {
        public static string Empty(this UrlHelper url)
        {
            return "javascript:void(0)";
        }

        public static string IsSelected(this HtmlHelper html, string area = "", string cssClass = null)
        {

            if (String.IsNullOrEmpty(cssClass))
                cssClass = "active";

            string currentAction = ((string)html.ViewContext.RouteData.Values["action"]).ToLower();
            string currentController = ((string)html.ViewContext.RouteData.Values["controller"]).ToLower();
            string currentArea = string.Empty;
            if (html.ViewContext.RouteData.DataTokens.Count > 0)
                currentArea = ((string)html.ViewContext.RouteData.DataTokens["area"]).ToLower();

            return area.ToLower() == currentArea.ToLower() ? cssClass : String.Empty;
        }

        public static IHtmlString Breadcrumb(this HtmlHelper html, string[] pages, string[] links = null)
        {
            var request = HttpContext.Current.Request;
            var urlBase = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, (new System.Web.Mvc.UrlHelper(request.RequestContext)).Content("~"));
            StringBuilder breadcrumb = new StringBuilder();
            breadcrumb.Append("<ol class='breadcrumb'>");
            breadcrumb.Append("<li class='breadcrumb-item'><i class='fa fa-home m-r-10 fs20'></i><a href='" + urlBase + "'>Home</a></li>");
            for (int i = 0; i < pages.Length; i++)
            {
                if (i == pages.Length - 1)
                    breadcrumb.Append("<li class='breadcrumb-item active'>" + pages[i] + "</li>");
                else
                    breadcrumb.Append("<li class='breadcrumb-item'><a href='" + links[i] + "'>" + pages[i] + "</a></li>");
            }
            breadcrumb.Append("</ol>");

            return MvcHtmlString.Create(breadcrumb.ToString());
        }

        public static string DrawImage(this UrlHelper url, string type, params string[] param)
        {
            try
            {

                string link = (HttpContext.Current.Request.IsSecureConnection ? "https://" : "http://") +
                    HttpContext.Current.Request.Url.Authority + "/" +
                HttpContext.Current.Request.ApplicationPath + "/Helpers/DrawImage.ashx?T=" + type;
                if (param != null && param.Length > 0)
                {
                    for (int i = 1; i < param.Length; i++)
                    {
                        link += "&" + param[i - 1] + "=" + param[i];
                    }

                }
                return link;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }

        public static IHtmlString DisabledTextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
        object htmlAttributes, bool disabled)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            if (disabled)
            {
                attributes["disabled"] = "disabled";
            }
            return htmlHelper.TextBoxFor(expression, attributes);
        }

    }
}