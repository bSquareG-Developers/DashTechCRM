using System.Web.Mvc;

namespace DashTechCRM.Areas.OnBoarding
{
    public class OnBoardingAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "OnBoarding";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "OnBoarding_default",
                "OnBoarding/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}