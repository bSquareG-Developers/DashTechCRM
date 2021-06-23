using System.Web.Mvc;

namespace DashTechCRM.Areas.ExpertCVCoach
{
    public class ExpertCVCoachAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ExpertCVCoach";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ExpertCVCoach_default",
                "ExpertCVCoach/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}