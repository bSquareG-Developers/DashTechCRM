using System.Web.Mvc;

namespace DashTechCRM.Areas.SeniorRecruiterMarketing
{
    public class SeniorRecruiterMarketingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SeniorRecruiterMarketing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SeniorRecruiterMarketing_default",
                "SeniorRecruiterMarketing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}