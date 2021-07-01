using System.Web.Mvc;

namespace DashTechCRM.Areas.JuniorRecruiterMarketing
{
    public class JuniorRecruiterMarketingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "JuniorRecruiterMarketing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "JuniorRecruiterMarketing_default",
                "JuniorRecruiterMarketing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}