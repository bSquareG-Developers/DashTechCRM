using DashTechCRM.Models;
using System;
using System.Data;
using System.Web.Mvc;

namespace DashTechCRM.Areas.OnBoarding.Controllers
{
    public class dashboardController : Controller
    {
        // GET: OnBoarding/dashboard
        DataTable dt = new DataTable();
        ConnectionDB dl = new ConnectionDB();
        public ActionResult Index()
        {
            return View();
        }

        public string GetOnBoardingDashBoard()
        {
            try
            {
                dt = dl.GetDataTable("GetOnBoardingDashBoard");
                return CommonHelperClass._serializeDatatable(dt);
            }
            catch (Exception e)
            {
                CommonHelperClass.InsertErrorLog(e.Message, "OnBoarding/dashboard/GetOnBoardingDashBoard");
                throw;
            }
        }
    }
}
