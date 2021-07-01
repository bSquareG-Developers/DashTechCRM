using System.Web.Mvc;

namespace DashTechCRM.Areas.TechnicalTeamManager
{
    public class TechnicalTeamManagerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TechnicalTeamManager";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TechnicalTeamManager_default",
                "TechnicalTeamManager/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}