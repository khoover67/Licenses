using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Licenses
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Details",
                url: "{controller}/{action}/{id}/{idProduct}/{idUpdate}/{idDetail}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional, idProduct = UrlParameter.Optional, idUpdate = UrlParameter.Optional, idDetail = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Updates",
                url: "{controller}/{action}/{id}/{idProduct}/{idUpdate}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional, idProduct = UrlParameter.Optional, idUpdate = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Products",
                url: "{controller}/{action}/{id}/{idProduct}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional, idProduct = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
