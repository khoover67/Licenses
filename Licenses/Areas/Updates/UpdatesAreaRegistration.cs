using System.Web.Mvc;

namespace Licenses.Areas.Updates
{
    public class UpdatesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Updates";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Updates_default",
                "Updates/{controller}/{action}/{idClient}/{idProduct}",
                new { action = "Index", idClient = UrlParameter.Optional, idProduct = UrlParameter.Optional }
            );
        }
    }
}