using DashTechCRM.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DashTechCRM.Areas.MarketingManager.Controllers
{
    public class SubmissionReportController : Controller
    {
        // GET: MarketingManager/SubmissionReport
        public ActionResult Index()
        {
            return View();
        }

        #region Json Results
        public JsonResult GetSubmissionDetailsTeamWise()
        {
            UserObject user = Session["userInfor"] as UserObject;
            string query = string.Format(@"");
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubmission()
        {
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}