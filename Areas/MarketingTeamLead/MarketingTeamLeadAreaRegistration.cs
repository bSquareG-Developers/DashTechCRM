using System.Web.Mvc;

namespace DashTechCRM.Areas.MarketingTeamLead
{
    public class MarketingTeamLeadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "MarketingTeamLead";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "MarketingTeamLead_default",
                "MarketingTeamLead/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}