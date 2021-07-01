using System.Web.Mvc;

namespace DashTechCRM.Areas.commontest
{
    public class commontestAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "commontest";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "commontest_default",
                "commontest/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}