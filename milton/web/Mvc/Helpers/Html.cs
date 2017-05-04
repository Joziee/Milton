using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Milton.Web.Mvc.Helpers
{
    public static partial class Html
    {
        private const string JS_VIEW_DATA_NAME = "RenderJavaScript";
        private const string CSS_VIEW_DATA_NAME = "RenderStyle";

        public static MvcHtmlString RenderStyles(this HtmlHelper htmlHelper)
        {
            StringBuilder result = new StringBuilder();

            List<string> styleList = htmlHelper.ViewContext.HttpContext.Items[Html.CSS_VIEW_DATA_NAME] as List<String>;

            if (styleList != null)
            {
                foreach (String script in styleList)
                {
                    result.AppendLine(String.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" />", script));
                }
            }

            return MvcHtmlString.Create(result.ToString());
        }

        public static void AddStyle(this HtmlHelper htmlHelper, String styleURL)
        {
            List<String> styleList = htmlHelper.ViewContext.HttpContext.Items[Html.CSS_VIEW_DATA_NAME] as List<String>;

            if (styleList != null)
            {
                if (!styleList.Contains(styleURL))
                {
                    styleList.Add(styleURL);
                }
            }
            else
            {
                styleList = new List<String>();
                styleList.Add(styleURL);
                htmlHelper.ViewContext.HttpContext.Items.Add(Html.CSS_VIEW_DATA_NAME, styleList);
            }
        }

        public static void AddJavaScript(this HtmlHelper htmlHelper, String scriptURL)
        {
            List<string> scriptList = htmlHelper.ViewContext.HttpContext.Items[Html.JS_VIEW_DATA_NAME] as List<String>;
            if (scriptList != null)
            {
                if (!scriptList.Contains(scriptURL))
                {
                    scriptList.Add(scriptURL);
                }
            }
            else
            {
                scriptList = new List<String>();
                scriptList.Add(scriptURL);
                htmlHelper.ViewContext.HttpContext.Items.Add(Html.JS_VIEW_DATA_NAME, scriptList);
            }
        }

        public static MvcHtmlString RenderJavaScripts(this HtmlHelper HtmlHelper)
        {
            StringBuilder result = new StringBuilder();

            List<string> scriptList = HtmlHelper.ViewContext.HttpContext.Items[Html.JS_VIEW_DATA_NAME] as List<String>;
            if (scriptList != null)
            {
                foreach (string script in scriptList)
                {
                    result.AppendLine(string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", script));
                }
            }

            return MvcHtmlString.Create(result.ToString());
        }

        public static string ToPointString(this decimal value)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            nfi.NumberGroupSeparator = ",";

            return value.ToString("n", nfi);
        }

        public static string SplitCamelCase(this string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }

        public static string AreaContent(this UrlHelper urlHelper, string resourceName)
        {
            var areaName = (string)urlHelper.RequestContext.RouteData.DataTokens["area"];
            return urlHelper.Action("Index", "Resource", new { resourceName = resourceName, area = areaName });
        }

        public static string SlugifyUrl(this string url)
        {
            //First to lower case 
            url = url.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(url);

            url = Encoding.ASCII.GetString(bytes);

            //Replace spaces 
            url = Regex.Replace(url, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars 
            url = Regex.Replace(url, @"[^\w\s\p{Pd}]", "", RegexOptions.Compiled);

            //Trim dashes from end 
            url = url.Trim('-', '_');

            //Replace double occurences of - or \_ 
            url = Regex.Replace(url, @"([-_]){2,}", "-", RegexOptions.Compiled);

            return url;
        }
    }
}
