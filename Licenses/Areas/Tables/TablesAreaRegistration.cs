using System.Web.Mvc;

namespace Licenses.Areas.Tables
{
    public class TablesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Tables";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Tables_default",
                "Tables/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
            //context.MapRoute(
            //    "Tables_default",
            //    "Tables/{controller}/{action}/{idCount}/{idUnit}",
            //    new { action = "Index", idCount = UrlParameter.Optional, idUnit = UrlParameter.Optional } 
            //);
        }
    }
}