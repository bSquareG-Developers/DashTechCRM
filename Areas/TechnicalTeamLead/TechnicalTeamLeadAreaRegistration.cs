using System.Web.Mvc;

namespace DashTechCRM.Areas.TechnicalTeamLead
{
    public class TechnicalTeamLeadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TechnicalTeamLead";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TechnicalTeamLead_default",
                "TechnicalTeamLead/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}