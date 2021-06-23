using System.Web.Mvc;

namespace DashTechCRM.Areas.SalesTeamLead
{
    public class SalesTeamLeadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SalesTeamLead";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SalesTeamLead_default",
                "SalesTeamLead/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}