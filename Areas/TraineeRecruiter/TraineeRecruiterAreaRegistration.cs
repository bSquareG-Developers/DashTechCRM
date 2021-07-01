using System.Web.Mvc;

namespace DashTechCRM.Areas.TraineeRecruiter
{
    public class TraineeRecruiterAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "TraineeRecruiter";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "TraineeRecruiter_default",
                "TraineeRecruiter/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}