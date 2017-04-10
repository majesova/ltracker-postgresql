using AppFramework.Security.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppFramework.Security.HtmlHelpers
{
    public static class MenuHelper
    {

        public static HtmlString BoostrapMenu(this HtmlHelper html, ICollection<AppMenuItem> items) {

            var request = HttpContext.Current.Request;

            UrlHelper urlHelper = new UrlHelper(html.ViewContext.RequestContext);

            var sb = new StringBuilder();
            if (items.Count == 0) return new HtmlString(string.Empty);

            foreach (var menuItem in items)
            {
                var nodeString = CreateNode(menuItem, request, urlHelper);
                sb.Append(nodeString);
            }
            return new HtmlString(sb.ToString());
        }

        internal static string CreateNode(AppMenuItem item, HttpRequest request, UrlHelper urlHelper) {

            var sb = new StringBuilder();
            if (item.Children == null) {
                item.Children = new List<AppMenuItem>();
            }
            if  (item.Children.Count == 0 )
            {
                //Nodo simple
                var serverPath = urlHelper.Content($"~/{item.PathToResource}");
                var nodeString = $"<li><a href=\"{serverPath}\">{item.Name}</a></li>";
                sb.Append(nodeString);
            }
            else {
                sb.Append("<li class= \"dropdown\">");
                    var serverPath = urlHelper.Content($"~/{item.PathToResource}");
                    sb.Append($"<a href=\"{serverPath}\" class=\"dropdown-toggle\" data-toggle=\"dropdown\" role=\"button\" aria-haspopup=\"true\" aria-expanded=\"false\">{item.Name}<span class=\"caret\"></span></a>");
                    sb.Append("<ul class=\"dropdown-menu\">");
                        foreach (var child in item.Children) {
                            serverPath = urlHelper.Content($"~/{child.PathToResource}");
                            var nodeString = CreateNode(child, request, urlHelper);
                            sb.Append(nodeString);
                        }
                    sb.Append("</ul>");
                sb.Append("</li>");
            }

            return sb.ToString();
        }

       
        

    }
}
