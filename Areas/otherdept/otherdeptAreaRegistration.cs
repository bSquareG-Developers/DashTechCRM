using System.Web.Mvc;

namespace DashTechCRM.Areas.otherdept
{
    public class otherdeptAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "otherdept";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "otherdept_default",
                "otherdept/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}