using System.Web.Mvc;

namespace DashTechCRM.Areas.common
{
    public class commonAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "common";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "common_default",
                "common/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}