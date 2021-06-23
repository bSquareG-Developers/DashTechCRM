using System.Web.Mvc;

namespace DashTechCRM.Areas.SalesAssociate
{
    public class SalesAssociateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SalesAssociate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SalesAssociate_default",
                "SalesAssociate/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}